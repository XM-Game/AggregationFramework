// AsyncInstantiateOperation was added since Unity 2022.3.20 / 2023.3.0b7
#if UNITY_2022_3 && !(UNITY_2022_3_0 || UNITY_2022_3_1 || UNITY_2022_3_2 || UNITY_2022_3_3 || UNITY_2022_3_4 || UNITY_2022_3_5 || UNITY_2022_3_6 || UNITY_2022_3_7 || UNITY_2022_3_8 || UNITY_2022_3_9 || UNITY_2022_3_10 || UNITY_2022_3_11 || UNITY_2022_3_12 || UNITY_2022_3_13 || UNITY_2022_3_14 || UNITY_2022_3_15 || UNITY_2022_3_16 || UNITY_2022_3_17 || UNITY_2022_3_18 || UNITY_2022_3_19)
#define UNITY_2022_SUPPORT
#endif

#if UNITY_2022_SUPPORT || UNITY_2023_3_OR_NEWER

using Cysharp.Threading.Tasks.Internal;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// AsyncInstantiateOperationExtensions 类
/// 提供将 AsyncInstantiateOperation 转换为 UniTask 的扩展方法
/// </summary>
namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// AsyncInstantiateOperationExtensions 类
    /// 提供将 AsyncInstantiateOperation 转换为 UniTask 的扩展方法
    /// </summary>
    public static class AsyncInstantiateOperationExtensions
    {
        // AsyncInstantiateOperation<T> has GetAwaiter so no need to impl
        // public static UniTask<T[]>.Awaiter GetAwaiter<T>(this AsyncInstantiateOperation<T> operation) where T : Object

        /// <summary>
        /// 将 AsyncInstantiateOperation 转换为 UniTask，并附加取消令牌
        /// </summary>
        public static UniTask<UnityEngine.Object[]> WithCancellation<T>(this AsyncInstantiateOperation asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 将 AsyncInstantiateOperation 转换为 UniTask，并附加取消令牌和立即取消
        /// </summary>
        public static UniTask<UnityEngine.Object[]> WithCancellation<T>(this AsyncInstantiateOperation asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 AsyncInstantiateOperation 转换为 UniTask，并附加进度回调、PlayerLoopTiming、取消令牌和立即取消
        /// </summary>
        public static UniTask<UnityEngine.Object[]> ToUniTask(this AsyncInstantiateOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<UnityEngine.Object[]>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.Result);
            return new UniTask<UnityEngine.Object[]>(AsyncInstantiateOperationConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// 将 AsyncInstantiateOperation<T> 转换为 UniTask，并附加取消令牌
        /// </summary>
        public static UniTask<T[]> WithCancellation<T>(this AsyncInstantiateOperation<T> asyncOperation, CancellationToken cancellationToken)
            where T : UnityEngine.Object
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 将 AsyncInstantiateOperation<T> 转换为 UniTask，并附加取消令牌和立即取消
        /// </summary>
        public static UniTask<T[]> WithCancellation<T>(this AsyncInstantiateOperation<T> asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
            where T : UnityEngine.Object
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 AsyncInstantiateOperation<T> 转换为 UniTask，并附加进度回调、PlayerLoopTiming、取消令牌和立即取消
        /// </summary>
        public static UniTask<T[]> ToUniTask<T>(this AsyncInstantiateOperation<T> asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
            where T : UnityEngine.Object
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<T[]>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.Result);
            return new UniTask<T[]>(AsyncInstantiateOperationConfiguredSource<T>.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// AsyncInstantiateOperationConfiguredSource 类
        /// 提供将 AsyncInstantiateOperation 转换为 UniTask 的扩展方法
        /// </summary>
        sealed class AsyncInstantiateOperationConfiguredSource : IUniTaskSource<UnityEngine.Object[]>, IPlayerLoopItem, ITaskPoolNode<AsyncInstantiateOperationConfiguredSource>
        {
            static TaskPool<AsyncInstantiateOperationConfiguredSource> pool;
            AsyncInstantiateOperationConfiguredSource nextNode;
            /// <summary>
            /// 下一个节点
            /// </summary>
            public ref AsyncInstantiateOperationConfiguredSource NextNode => ref nextNode;

            static AsyncInstantiateOperationConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncInstantiateOperationConfiguredSource), () => pool.Size);
            }

            AsyncInstantiateOperation asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<UnityEngine.Object[]> core;

            /// <summary>
            /// 延续动作
            /// </summary>
            Action<AsyncOperation> continuationAction;

            AsyncInstantiateOperationConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建并初始化源：注册完成/取消/进度回调，加入 PlayerLoop。
            /// </summary>
            public static IUniTaskSource<UnityEngine.Object[]> Create(AsyncInstantiateOperation asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<UnityEngine.Object[]>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncInstantiateOperationConfiguredSource();
                }

                result.asyncOperation = asyncOperation;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                result.completed = false;

                asyncOperation.completed += result.continuationAction;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (AsyncInstantiateOperationConfiguredSource)state;
                        source.core.TrySetCanceled(source.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public UnityEngine.Object[] GetResult(short token)
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

            /// <summary>
            /// 获取结果
            /// </summary>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成时调用
            /// </summary>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            public bool MoveNext()
            {
                // Already completed
                if (completed || asyncOperation == null)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    core.TrySetResult(asyncOperation.Result);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                asyncOperation.completed -= continuationAction;
                asyncOperation = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }

            /// <summary>
            /// 延续动作
            /// </summary>
            void Continuation(AsyncOperation _)
            {
                if (completed)
                {
                    return;
                }
                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                }
                else
                {
                    core.TrySetResult(asyncOperation.Result);
                }
            }
        }

        /// <summary>
        /// AsyncInstantiateOperationConfiguredSource<T> 类
        /// 提供将 AsyncInstantiateOperation<T> 转换为 UniTask 的扩展方法
        /// </summary>
        sealed class AsyncInstantiateOperationConfiguredSource<T> : IUniTaskSource<T[]>, IPlayerLoopItem, ITaskPoolNode<AsyncInstantiateOperationConfiguredSource<T>>
            where T : UnityEngine.Object
        {
            static TaskPool<AsyncInstantiateOperationConfiguredSource<T>> pool;
            AsyncInstantiateOperationConfiguredSource<T> nextNode;
            public ref AsyncInstantiateOperationConfiguredSource<T> NextNode => ref nextNode;

            static AsyncInstantiateOperationConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncInstantiateOperationConfiguredSource<T>), () => pool.Size);
            }

            AsyncInstantiateOperation<T> asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<T[]> core;

            Action<AsyncOperation> continuationAction;

            AsyncInstantiateOperationConfiguredSource()
            {
                continuationAction = Continuation;
            }
            /// <summary>
            /// 创建并初始化源：注册完成/取消/进度回调，加入 PlayerLoop。
            /// </summary>
            public static IUniTaskSource<T[]> Create(AsyncInstantiateOperation<T> asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<T[]>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncInstantiateOperationConfiguredSource<T>();
                }

                result.asyncOperation = asyncOperation;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                result.completed = false;

                asyncOperation.completed += result.continuationAction;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (AsyncInstantiateOperationConfiguredSource<T>)state;
                        source.core.TrySetCanceled(source.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public T[] GetResult(short token)
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

            /// <summary>
            /// 获取结果
            /// </summary>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成时调用
            /// </summary>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            public bool MoveNext()
            {
                // Already completed
                if (completed || asyncOperation == null)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    core.TrySetResult(asyncOperation.Result);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                asyncOperation.completed -= continuationAction;
                asyncOperation = default;
                progress = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }

            /// <summary>
            /// 延续动作
            /// </summary>
            void Continuation(AsyncOperation _)
            {
                if (completed)
                {
                    return;
                }
                completed = true;
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                }
                else
                {
                    core.TrySetResult(asyncOperation.Result);
                }
            }
        }
    }
}

#endif