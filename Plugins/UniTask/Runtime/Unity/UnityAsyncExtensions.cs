#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks.Internal;
#if ENABLE_UNITYWEBREQUEST && (!UNITY_2019_1_OR_NEWER || UNITASK_WEBREQUEST_SUPPORT)
using UnityEngine.Networking;
#endif

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 UnityAsyncExtensions 方法，用于扩展 Unity 引擎的异步操作
    /// </remarks>
    public static partial class UnityAsyncExtensions
    {
        #region AsyncOperation

#if !UNITY_2023_1_OR_NEWER
        // from Unity2023.1.0a15, AsyncOperationAwaitableExtensions.GetAwaiter is defined in UnityEngine.
        /// <summary>
        /// 获取 AsyncOperation 的等待器
        /// </summary>
        /// <param name="asyncOperation">AsyncOperation</param>
        /// <returns>AsyncOperationAwaiter</returns>
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new AsyncOperationAwaiter(asyncOperation);
        }
#endif

        /// <summary>
        /// 添加取消令牌
        /// </summary>
        /// <param name="asyncOperation">AsyncOperation</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static UniTask WithCancellation(this AsyncOperation asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 添加取消令牌和立即取消
        /// </summary>
        /// <param name="asyncOperation">AsyncOperation</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask WithCancellation(this AsyncOperation asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 AsyncOperation 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">AsyncOperation</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask ToUniTask(this AsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled(cancellationToken);
            if (asyncOperation.isDone) return UniTask.CompletedTask;
            return new UniTask(AsyncOperationConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// AsyncOperationAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 AsyncOperationAwaiter
        /// </remarks>
        public struct AsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            AsyncOperation asyncOperation;
            Action<AsyncOperation> continuationAction;

            public AsyncOperationAwaiter(AsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public bool IsCompleted => asyncOperation.isDone;

            public void GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    asyncOperation = null;
                }
                else
                {
                    asyncOperation = null;
                }
            }

            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        /// <summary>
        /// AsyncOperationConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 提供 Create 方法，用于创建 AsyncOperationConfiguredSource
        /// </remarks>
        sealed class AsyncOperationConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<AsyncOperationConfiguredSource>
        {
            static TaskPool<AsyncOperationConfiguredSource> pool;
            AsyncOperationConfiguredSource nextNode;
            public ref AsyncOperationConfiguredSource NextNode => ref nextNode;

            static AsyncOperationConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncOperationConfiguredSource), () => pool.Size);
            }

            AsyncOperation asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<AsyncUnit> core;

            Action<AsyncOperation> continuationAction;

            AsyncOperationConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 AsyncOperationConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">AsyncOperation</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(AsyncOperation asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncOperationConfiguredSource();
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
                        var source = (AsyncOperationConfiguredSource)state;
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
            /// <param name="token">令牌</param>
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


            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
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
                    core.TrySetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
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
            /// 延续
            /// </summary>
            /// <param name="_">AsyncOperation</param>
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
                    core.TrySetResult(AsyncUnit.Default);
                }
            }
        }

        #endregion

        #region ResourceRequest

        /// <summary>
        /// 获取 ResourceRequest 的等待器
        /// </summary>
        /// <param name="asyncOperation">ResourceRequest</param>
        /// <returns>ResourceRequestAwaiter</returns>
        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new ResourceRequestAwaiter(asyncOperation);
        }

        public static UniTask<UnityEngine.Object> WithCancellation(this ResourceRequest asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        public static UniTask<UnityEngine.Object> WithCancellation(this ResourceRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 ResourceRequest 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">ResourceRequest</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask<UnityEngine.Object></returns>
        public static UniTask<UnityEngine.Object> ToUniTask(this ResourceRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<UnityEngine.Object>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.asset);
            return new UniTask<UnityEngine.Object>(ResourceRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// ResourceRequestAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 ResourceRequestAwaiter
        /// </remarks>
        public struct ResourceRequestAwaiter : ICriticalNotifyCompletion
        {
            ResourceRequest asyncOperation;
            Action<AsyncOperation> continuationAction;

            public ResourceRequestAwaiter(ResourceRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public bool IsCompleted => asyncOperation.isDone;

            /// <summary>
            /// 获取结果
            /// </summary>
            /// <returns>UnityEngine.Object</returns>
            public UnityEngine.Object GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    var result = asyncOperation.asset;
                    asyncOperation = null;
                    return result;
                }
                else
                {
                    var result = asyncOperation.asset;
                    asyncOperation = null;
                    return result;
                }
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        /// <summary>
        /// ResourceRequestConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 提供 Create 方法，用于创建 ResourceRequestConfiguredSource
        /// </remarks>
        sealed class ResourceRequestConfiguredSource : IUniTaskSource<UnityEngine.Object>, IPlayerLoopItem, ITaskPoolNode<ResourceRequestConfiguredSource>
        {
            static TaskPool<ResourceRequestConfiguredSource> pool;
            ResourceRequestConfiguredSource nextNode;
            public ref ResourceRequestConfiguredSource NextNode => ref nextNode;

            static ResourceRequestConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(ResourceRequestConfiguredSource), () => pool.Size);
            }

            ResourceRequest asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<UnityEngine.Object> core;

            Action<AsyncOperation> continuationAction;

            ResourceRequestConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 ResourceRequestConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">ResourceRequest</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<UnityEngine.Object></returns>
            public static IUniTaskSource<UnityEngine.Object> Create(ResourceRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<UnityEngine.Object>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new ResourceRequestConfiguredSource();
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
                        var source = (ResourceRequestConfiguredSource)state;
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
            /// <param name="token">令牌</param>
            /// <returns>UnityEngine.Object</returns>
            public UnityEngine.Object GetResult(short token)
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
            /// <param name="token">令牌</param>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
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
                    core.TrySetResult(asyncOperation.asset);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
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
            /// 延续
            /// </summary>
            /// <param name="_">AsyncOperation</param>
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
                    core.TrySetResult(asyncOperation.asset);
                }
            }
        }

        #endregion

#if UNITASK_ASSETBUNDLE_SUPPORT
        #region AssetBundleRequest

        /// <summary>
        /// 获取 AssetBundleRequest 的等待器
        /// </summary>
        /// <param name="asyncOperation">AssetBundleRequest</param>
        /// <returns>AssetBundleRequestAwaiter</returns>
        public static AssetBundleRequestAwaiter GetAwaiter(this AssetBundleRequest asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new AssetBundleRequestAwaiter(asyncOperation);
        }

        public static UniTask<UnityEngine.Object> WithCancellation(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        public static UniTask<UnityEngine.Object> WithCancellation(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 AssetBundleRequest 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">AssetBundleRequest</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask<UnityEngine.Object></returns>
        public static UniTask<UnityEngine.Object> ToUniTask(this AssetBundleRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<UnityEngine.Object>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.asset);
            return new UniTask<UnityEngine.Object>(AssetBundleRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// AssetBundleRequestAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 AssetBundleRequestAwaiter
        /// </remarks>
        public struct AssetBundleRequestAwaiter : ICriticalNotifyCompletion
        {
            AssetBundleRequest asyncOperation;
            Action<AsyncOperation> continuationAction;

            public AssetBundleRequestAwaiter(AssetBundleRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public bool IsCompleted => asyncOperation.isDone;

            public UnityEngine.Object GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    var result = asyncOperation.asset;
                    asyncOperation = null;
                    return result;
                }
                else
                {
                    var result = asyncOperation.asset;
                    asyncOperation = null;
                    return result;
                }
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        /// <summary>
        /// AssetBundleRequestConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 提供 Create 方法，用于创建 AssetBundleRequestConfiguredSource
        /// </remarks>
        sealed class AssetBundleRequestConfiguredSource : IUniTaskSource<UnityEngine.Object>, IPlayerLoopItem, ITaskPoolNode<AssetBundleRequestConfiguredSource>
        {
            static TaskPool<AssetBundleRequestConfiguredSource> pool;
            AssetBundleRequestConfiguredSource nextNode;
            public ref AssetBundleRequestConfiguredSource NextNode => ref nextNode;

            static AssetBundleRequestConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AssetBundleRequestConfiguredSource), () => pool.Size);
            }

            AssetBundleRequest asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<UnityEngine.Object> core;

            Action<AsyncOperation> continuationAction;

            AssetBundleRequestConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 AssetBundleRequestConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">AssetBundleRequest</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<UnityEngine.Object></returns>
            public static IUniTaskSource<UnityEngine.Object> Create(AssetBundleRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<UnityEngine.Object>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AssetBundleRequestConfiguredSource();
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
                        var source = (AssetBundleRequestConfiguredSource)state;
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
            /// <param name="token">令牌</param>
            /// <returns>UnityEngine.Object</returns>
            public UnityEngine.Object GetResult(short token)
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
            /// <param name="token">令牌</param>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
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
                    core.TrySetResult(asyncOperation.asset);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
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
            /// 延续
            /// </summary>
            /// <param name="_">AsyncOperation</param>
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
                    core.TrySetResult(asyncOperation.asset);
                }
            }
        }

        #endregion
#endif

#if UNITASK_ASSETBUNDLE_SUPPORT
        #region AssetBundleCreateRequest

        /// <summary>
        /// 获取 AssetBundleCreateRequest 的等待器
        /// </summary>
        /// <param name="asyncOperation">AssetBundleCreateRequest</param>
        /// <returns>AssetBundleCreateRequestAwaiter</returns>
        public static AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new AssetBundleCreateRequestAwaiter(asyncOperation);
        }

        /// <summary>
        /// 将 AssetBundleCreateRequest 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">AssetBundleCreateRequest</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask<AssetBundle></returns>
        public static UniTask<AssetBundle> WithCancellation(this AssetBundleCreateRequest asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        public static UniTask<AssetBundle> WithCancellation(this AssetBundleCreateRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 AssetBundleCreateRequest 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">AssetBundleCreateRequest</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public static UniTask<AssetBundle> ToUniTask(this AssetBundleCreateRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<AssetBundle>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.assetBundle);
            return new UniTask<AssetBundle>(AssetBundleCreateRequestConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// AssetBundleCreateRequestAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 AssetBundleCreateRequestAwaiter
        /// </remarks>
        public struct AssetBundleCreateRequestAwaiter : ICriticalNotifyCompletion
        {
            AssetBundleCreateRequest asyncOperation;
            Action<AsyncOperation> continuationAction;

            public AssetBundleCreateRequestAwaiter(AssetBundleCreateRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            /// <summary>
            /// 获取是否完成
            /// </summary>
            /// <returns>bool</returns>
            public bool IsCompleted => asyncOperation.isDone;

            /// <summary>
            /// 获取结果
            /// </summary>
            /// <returns>AssetBundle</returns>
            public AssetBundle GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    var result = asyncOperation.assetBundle;
                    asyncOperation = null;
                    return result;
                }
                else
                {
                    var result = asyncOperation.assetBundle;
                    asyncOperation = null;
                    return result;
                }
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        /// <summary>
        /// AssetBundleCreateRequestConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 提供 Create 方法，用于创建 AssetBundleCreateRequestConfiguredSource
        /// </remarks>
        sealed class AssetBundleCreateRequestConfiguredSource : IUniTaskSource<AssetBundle>, IPlayerLoopItem, ITaskPoolNode<AssetBundleCreateRequestConfiguredSource>
        {
            static TaskPool<AssetBundleCreateRequestConfiguredSource> pool;
            AssetBundleCreateRequestConfiguredSource nextNode;
            public ref AssetBundleCreateRequestConfiguredSource NextNode => ref nextNode;

            static AssetBundleCreateRequestConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AssetBundleCreateRequestConfiguredSource), () => pool.Size);
            }

            AssetBundleCreateRequest asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<AssetBundle> core;

            Action<AsyncOperation> continuationAction;

            AssetBundleCreateRequestConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 AssetBundleCreateRequestConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">AssetBundleCreateRequest</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<AssetBundle></returns>
            public static IUniTaskSource<AssetBundle> Create(AssetBundleCreateRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<AssetBundle>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AssetBundleCreateRequestConfiguredSource();
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
                        var source = (AssetBundleCreateRequestConfiguredSource)state;
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
            /// <param name="token">令牌</param>
            /// <returns>AssetBundle</returns>
            public AssetBundle GetResult(short token)
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
            /// <param name="token">令牌</param>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
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
                    core.TrySetResult(asyncOperation.assetBundle);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
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
            /// 延续
            /// </summary>
            /// <param name="_">AsyncOperation</param>
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
                    core.TrySetResult(asyncOperation.assetBundle);
                }
            }
        }

        #endregion
#endif

#if ENABLE_UNITYWEBREQUEST && (!UNITY_2019_1_OR_NEWER || UNITASK_WEBREQUEST_SUPPORT)
        #region UnityWebRequestAsyncOperation

        /// <summary>
        /// 获取 UnityWebRequestAsyncOperation 的等待器
        /// </summary>
        /// <param name="asyncOperation">UnityWebRequestAsyncOperation</param>
        /// <returns>UnityWebRequestAsyncOperationAwaiter</returns>
        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
        }

        public static UniTask<UnityWebRequest> WithCancellation(this UnityWebRequestAsyncOperation asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        public static UniTask<UnityWebRequest> WithCancellation(this UnityWebRequestAsyncOperation asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 将 UnityWebRequestAsyncOperation 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">UnityWebRequestAsyncOperation</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public static UniTask<UnityWebRequest> ToUniTask(this UnityWebRequestAsyncOperation asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<UnityWebRequest>(cancellationToken);
            if (asyncOperation.isDone)
            {
                if (asyncOperation.webRequest.IsError())
                {
                    return UniTask.FromException<UnityWebRequest>(new UnityWebRequestException(asyncOperation.webRequest));
                }
                return UniTask.FromResult(asyncOperation.webRequest);
            }
            return new UniTask<UnityWebRequest>(UnityWebRequestAsyncOperationConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// UnityWebRequestAsyncOperationAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 UnityWebRequestAsyncOperationAwaiter
        /// </remarks>
        public struct UnityWebRequestAsyncOperationAwaiter : ICriticalNotifyCompletion
        {
            UnityWebRequestAsyncOperation asyncOperation;
            Action<AsyncOperation> continuationAction;

            public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            /// <summary>
            /// 获取是否完成
            /// </summary>
            /// <returns>bool</returns>
            public bool IsCompleted => asyncOperation.isDone;

            /// <summary>
            /// 获取结果
            /// </summary>
            /// <returns>UnityWebRequest</returns>
            public UnityWebRequest GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    var result = asyncOperation.webRequest;
                    asyncOperation = null;
                    if (result.IsError())
                    {
                        throw new UnityWebRequestException(result);
                    }
                    return result;
                }
                else
                {
                    var result = asyncOperation.webRequest;
                    asyncOperation = null;
                    if (result.IsError())
                    {
                        throw new UnityWebRequestException(result);
                    }
                    return result;
                }
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void OnCompleted(Action continuation)
            {
                UnsafeOnCompleted(continuation);
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            public void UnsafeOnCompleted(Action continuation)
            {
                Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
                continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
                asyncOperation.completed += continuationAction;
            }
        }

        /// <summary>
        /// UnityWebRequestAsyncOperationConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 提供 Create 方法，用于创建 UnityWebRequestAsyncOperationConfiguredSource
        /// </remarks>
        sealed class UnityWebRequestAsyncOperationConfiguredSource : IUniTaskSource<UnityWebRequest>, IPlayerLoopItem, ITaskPoolNode<UnityWebRequestAsyncOperationConfiguredSource>
        {
            static TaskPool<UnityWebRequestAsyncOperationConfiguredSource> pool;
            UnityWebRequestAsyncOperationConfiguredSource nextNode;
            public ref UnityWebRequestAsyncOperationConfiguredSource NextNode => ref nextNode;

            static UnityWebRequestAsyncOperationConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(UnityWebRequestAsyncOperationConfiguredSource), () => pool.Size);
            }

            UnityWebRequestAsyncOperation asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<UnityWebRequest> core;

            Action<AsyncOperation> continuationAction;

            UnityWebRequestAsyncOperationConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 UnityWebRequestAsyncOperationConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">UnityWebRequestAsyncOperation</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<UnityWebRequest></returns>
            public static IUniTaskSource<UnityWebRequest> Create(UnityWebRequestAsyncOperation asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<UnityWebRequest>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new UnityWebRequestAsyncOperationConfiguredSource();
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
                        var source = (UnityWebRequestAsyncOperationConfiguredSource)state;
                        source.asyncOperation.webRequest.Abort();
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
            /// <param name="token">令牌</param>
            /// <returns>UnityWebRequest</returns>
            public UnityWebRequest GetResult(short token)
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
            /// <param name="token">令牌</param>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <param name="token">令牌</param>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 获取状态
            /// </summary>
            /// <returns>UniTaskStatus</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            /// <summary>
            /// 完成
            /// </summary>
            /// <param name="continuation">延续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动到下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                // Already completed
                if (completed || asyncOperation == null)
                {
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    asyncOperation.webRequest.Abort();
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null)
                {
                    progress.Report(asyncOperation.progress);
                }

                if (asyncOperation.isDone)
                {
                    if (asyncOperation.webRequest.IsError())
                    {
                        core.TrySetException(new UnityWebRequestException(asyncOperation.webRequest));
                    }
                    else
                    {
                        core.TrySetResult(asyncOperation.webRequest);
                    }
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
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
            /// 延续
            /// </summary>
            /// <param name="_">AsyncOperation</param>
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
                else if (asyncOperation.webRequest.IsError())
                {
                    core.TrySetException(new UnityWebRequestException(asyncOperation.webRequest));
                }
                else
                {
                    core.TrySetResult(asyncOperation.webRequest);
                }
            }
        }

        #endregion
#endif

    }
}