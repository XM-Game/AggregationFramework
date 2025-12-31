// asmdef Version Defines, enabled when com.unity.addressables is imported.

#if UNITASK_ADDRESSABLE_SUPPORT

using Cysharp.Threading.Tasks.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// Addressables 异步扩展：将 Unity Addressables 的 AsyncOperationHandle 系列与 UniTask 互操作，
    /// 提供 await 支持、取消/进度汇报、自动释放 handle 等能力。
    /// </summary>
    public static class AddressablesAsyncExtensions
    {
#region AsyncOperationHandle

        /// <summary>
        /// 允许直接对 AsyncOperationHandle 使用 await（返回 UniTaskAwaiter）。
        /// </summary>
        public static UniTask.Awaiter GetAwaiter(this AsyncOperationHandle handle)
        {
            return ToUniTask(handle).GetAwaiter();
        }

        /// <summary>
        /// 将 AsyncOperationHandle 转为 UniTask，并附加取消/立即取消/自动释放参数。
        /// </summary>
        public static UniTask WithCancellation(this AsyncOperationHandle handle, CancellationToken cancellationToken, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            return ToUniTask(handle, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately, autoReleaseWhenCanceled: autoReleaseWhenCanceled);
        }

        /// <summary>
        /// 将 AsyncOperationHandle 转为 UniTask，可配置进度回调、PlayerLoop 定时、取消、立即取消、取消时自动释放。
        /// </summary>
        public static UniTask ToUniTask(this AsyncOperationHandle handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled(cancellationToken);

            if (!handle.IsValid())
            {
                // autoReleaseHandle:true handle is invalid(immediately internal handle == null) so return completed.
                return UniTask.CompletedTask;
            }

            if (handle.IsDone)
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    return UniTask.FromException(handle.OperationException);
                }
                return UniTask.CompletedTask;
            }

            return new UniTask(AsyncOperationHandleConfiguredSource.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out var token), token);
        }

        /// <summary>
        /// AsyncOperationHandle 的 Awaiter，实现 await 协议并在完成时检查失败状态。
        /// </summary>
        public struct AsyncOperationHandleAwaiter : ICriticalNotifyCompletion
        {
            AsyncOperationHandle handle;
            Action<AsyncOperationHandle> continuationAction;

            public AsyncOperationHandleAwaiter(AsyncOperationHandle handle)
            {
                this.handle = handle;
                this.continuationAction = null;
            }

            public bool IsCompleted => handle.IsDone;

            public void GetResult()
            {
                if (continuationAction != null)
                {
                    handle.Completed -= continuationAction;
                    continuationAction = null;
                }

                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    var e = handle.OperationException;
                    handle = default;
                    ExceptionDispatchInfo.Capture(e).Throw();
                }

                var result = handle.Result;
                handle = default;
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperationHandle>.Create(continuation);
                handle.Completed += continuationAction;
            }
        }

        /// <summary>
        /// 非泛型 handle 的内部源：负责挂接完成/取消、进度回调，并与 PlayerLoop 协作。
        /// 使用 TaskPool 复用以减少 GC。
        /// </summary>
        sealed class AsyncOperationHandleConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource>
        {
            static TaskPool<AsyncOperationHandleConfiguredSource> pool;
            AsyncOperationHandleConfiguredSource nextNode;
            public ref AsyncOperationHandleConfiguredSource NextNode => ref nextNode;

            static AsyncOperationHandleConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource), () => pool.Size);
            }

            readonly Action<AsyncOperationHandle> completedCallback;
            AsyncOperationHandle handle;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            IProgress<float> progress;
            bool autoReleaseWhenCanceled;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<AsyncUnit> core;

            AsyncOperationHandleConfiguredSource()
            {
                completedCallback = HandleCompleted;
            }

            /// <summary>
            /// 创建并初始化源：注册取消、进度与完成回调，加入 PlayerLoop 调度。
            /// </summary>
            public static IUniTaskSource Create(AsyncOperationHandle handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, bool autoReleaseWhenCanceled, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncOperationHandleConfiguredSource();
                }

                result.handle = handle;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
                result.completed = false;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (AsyncOperationHandleConfiguredSource)state;
                        if (promise.autoReleaseWhenCanceled && promise.handle.IsValid())
                        {
                            Addressables.Release(promise.handle);
                        }
                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                handle.Completed += result.completedCallback;

                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// handle 完成回调：解除监听，依据状态设置结果/异常/取消，并按需释放。
            /// </summary>
            void HandleCompleted(AsyncOperationHandle _)
            {
                if (handle.IsValid())
                {
                    handle.Completed -= completedCallback;
                }

                if (completed)
                {
                    return;
                }
                
                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }
                    core.TrySetCanceled(cancellationToken);
                }
                else if (handle.Status == AsyncOperationStatus.Failed)
                {
                    core.TrySetException(handle.OperationException);
                }
                else
                {
                    core.TrySetResult(AsyncUnit.Default);
                }
            }

            public void GetResult(short token)
            {
                try
                {
                    core.GetResult(token);
                }
                finally
                {
                    if (!(cancelImmediately && cancellationToken.IsCancellationRequested))
                    {
                        TryReturn();
                    }
                    else
                    {
                        TaskTracker.RemoveTracking(this);
                    }
                }
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// PlayerLoop 驱动，检查取消并汇报进度。
            /// </summary>
            public bool MoveNext()
            {
                if (completed)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completed = true;
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null && handle.IsValid())
                {
                    progress.Report(handle.GetDownloadStatus().Percent);
                }

                return true;
            }

            /// <summary>
            /// 归还到对象池并重置内部状态。
            /// </summary>
            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                handle = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }
        }

#endregion

#region AsyncOperationHandle_T

        /// <summary>
        /// 允许直接对 AsyncOperationHandle&lt;T&gt; 使用 await。
        /// </summary>
        public static UniTask<T>.Awaiter GetAwaiter<T>(this AsyncOperationHandle<T> handle)
        {
            return ToUniTask(handle).GetAwaiter();
        }

        /// <summary>
        /// 将泛型 handle 转为 UniTask&lt;T&gt;，支持取消/立即取消/自动释放。
        /// </summary>
        public static UniTask<T> WithCancellation<T>(this AsyncOperationHandle<T> handle, CancellationToken cancellationToken, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            return ToUniTask(handle, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately, autoReleaseWhenCanceled: autoReleaseWhenCanceled);
        }

        /// <summary>
        /// 将泛型 handle 转为 UniTask&lt;T&gt;，可配置进度、定时、取消、立即取消、取消时自动释放。
        /// </summary>
        public static UniTask<T> ToUniTask<T>(this AsyncOperationHandle<T> handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
        {
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<T>(cancellationToken);

            if (!handle.IsValid())
            {
                throw new Exception("Attempting to use an invalid operation handle");
            }

            if (handle.IsDone)
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    return UniTask.FromException<T>(handle.OperationException);
                }
                return UniTask.FromResult(handle.Result);
            }

            return new UniTask<T>(AsyncOperationHandleConfiguredSource<T>.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out var token), token);
        }

        /// <summary>
        /// 泛型 handle 的内部源：负责结果/异常/取消传递与进度、PlayerLoop 协作，并复用对象池。
        /// </summary>
        sealed class AsyncOperationHandleConfiguredSource<T> : IUniTaskSource<T>, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource<T>>
        {
            static TaskPool<AsyncOperationHandleConfiguredSource<T>> pool;
            AsyncOperationHandleConfiguredSource<T> nextNode;
            public ref AsyncOperationHandleConfiguredSource<T> NextNode => ref nextNode;

            static AsyncOperationHandleConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource<T>), () => pool.Size);
            }

            readonly Action<AsyncOperationHandle<T>> completedCallback;
            AsyncOperationHandle<T> handle;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            IProgress<float> progress;
            bool autoReleaseWhenCanceled;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<T> core;

            AsyncOperationHandleConfiguredSource()
            {
                completedCallback = HandleCompleted;
            }

            /// <summary>
            /// 创建并初始化源：注册完成/取消/进度回调，加入 PlayerLoop。
            /// </summary>
            public static IUniTaskSource<T> Create(AsyncOperationHandle<T> handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, bool autoReleaseWhenCanceled, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncOperationHandleConfiguredSource<T>();
                }

                result.handle = handle;
                result.cancellationToken = cancellationToken;
                result.completed = false;
                result.progress = progress;
                result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
                result.cancelImmediately = cancelImmediately;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (AsyncOperationHandleConfiguredSource<T>)state;
                        if (promise.autoReleaseWhenCanceled && promise.handle.IsValid())
                        {
                            Addressables.Release(promise.handle);
                        }
                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                handle.Completed += result.completedCallback;

                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// handle 完成回调：解除监听，根据状态设置结果/异常/取消并可选释放。
            /// </summary>
            void HandleCompleted(AsyncOperationHandle<T> argHandle)
            {
                if (handle.IsValid())
                {
                    handle.Completed -= completedCallback;
                }

                if (completed)
                {
                    return;
                }
                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }
                    core.TrySetCanceled(cancellationToken);
                }
                else if (argHandle.Status == AsyncOperationStatus.Failed)
                {
                    core.TrySetException(argHandle.OperationException);
                }
                else
                {
                    core.TrySetResult(argHandle.Result);
                }
            }

            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    if (!(cancelImmediately && cancellationToken.IsCancellationRequested))
                    {
                        TryReturn();
                    }
                    else
                    {
                        TaskTracker.RemoveTracking(this);
                    }
                }
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// PlayerLoop 驱动：检查取消并更新进度。
            /// </summary>
            public bool MoveNext()
            {
                if (completed)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completed = true;
                    if (autoReleaseWhenCanceled && handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null && handle.IsValid())
                {
                    progress.Report(handle.GetDownloadStatus().Percent);
                }

                return true;
            }

            /// <summary>
            /// 重置并归还到对象池。
            /// </summary>
            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                handle = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }
        }

#endregion
    }
}

#endif