#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

#if UNITY_2018_4 || UNITY_2019_4_OR_NEWER
#if UNITASK_ASSETBUNDLE_SUPPORT

using Cysharp.Threading.Tasks.Internal;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 AwaitForAllAssets 方法，用于等待 AssetBundleRequest 完成
    /// </remarks>
    public static partial class UnityAsyncExtensions
    {
        /// <summary>
        /// 等待 AssetBundleRequest 完成
        /// </summary>
        /// <param name="asyncOperation">AssetBundleRequest</param>
        /// <returns>AssetBundleRequestAllAssetsAwaiter</returns>
        public static AssetBundleRequestAllAssetsAwaiter AwaitForAllAssets(this AssetBundleRequest asyncOperation)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            return new AssetBundleRequestAllAssetsAwaiter(asyncOperation);
        }

        public static UniTask<UnityEngine.Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken)
        {
            return AwaitForAllAssets(asyncOperation, null, PlayerLoopTiming.Update, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 等待 AssetBundleRequest 完成
        /// </summary>
        /// <param name="asyncOperation">AssetBundleRequest</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask<UnityEngine.Object[]></returns>
        public static UniTask<UnityEngine.Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return AwaitForAllAssets(asyncOperation, progress: null, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }

        /// <summary>
        /// 等待 AssetBundleRequest 完成
        /// </summary>
        /// <param name="asyncOperation">AssetBundleRequest</param>
        /// <param name="progress">进度</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask<UnityEngine.Object[]></returns>
        public static UniTask<UnityEngine.Object[]> AwaitForAllAssets(this AssetBundleRequest asyncOperation, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            Error.ThrowArgumentNullException(asyncOperation, nameof(asyncOperation));
            if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled<UnityEngine.Object[]>(cancellationToken);
            if (asyncOperation.isDone) return UniTask.FromResult(asyncOperation.allAssets);
            return new UniTask<UnityEngine.Object[]>(AssetBundleRequestAllAssetsConfiguredSource.Create(asyncOperation, timing, progress, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// AssetBundleRequestAllAssetsAwaiter 结构体
        /// </summary>
        /// <remarks>
        /// 提供 GetAwaiter 方法，用于获取 AssetBundleRequestAllAssetsAwaiter
        /// </remarks>
        public struct AssetBundleRequestAllAssetsAwaiter : ICriticalNotifyCompletion
        {
            AssetBundleRequest asyncOperation;
            Action<AsyncOperation> continuationAction;

            public AssetBundleRequestAllAssetsAwaiter(AssetBundleRequest asyncOperation)
            {
                this.asyncOperation = asyncOperation;
                this.continuationAction = null;
            }

            public AssetBundleRequestAllAssetsAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted => asyncOperation.isDone;

            public UnityEngine.Object[] GetResult()
            {
                if (continuationAction != null)
                {
                    asyncOperation.completed -= continuationAction;
                    continuationAction = null;
                    var result = asyncOperation.allAssets;
                    asyncOperation = null;
                    return result;
                }
                else
                {
                    var result = asyncOperation.allAssets;
                    asyncOperation = null;
                    return result;
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
        /// AssetBundleRequestAllAssetsConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 实现 IUniTaskSource<UnityEngine.Object[]> 和 IPlayerLoopItem 接口
        /// </remarks>
        sealed class AssetBundleRequestAllAssetsConfiguredSource : IUniTaskSource<UnityEngine.Object[]>, IPlayerLoopItem, ITaskPoolNode<AssetBundleRequestAllAssetsConfiguredSource>
        {
            static TaskPool<AssetBundleRequestAllAssetsConfiguredSource> pool;
            AssetBundleRequestAllAssetsConfiguredSource nextNode;
            public ref AssetBundleRequestAllAssetsConfiguredSource NextNode => ref nextNode;

            static AssetBundleRequestAllAssetsConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AssetBundleRequestAllAssetsConfiguredSource), () => pool.Size);
            }

            AssetBundleRequest asyncOperation;
            IProgress<float> progress;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            bool completed;

            UniTaskCompletionSourceCore<UnityEngine.Object[]> core;

            Action<AsyncOperation> continuationAction;

            AssetBundleRequestAllAssetsConfiguredSource()
            {
                continuationAction = Continuation;
            }

            /// <summary>
            /// 创建 AssetBundleRequestAllAssetsConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">AssetBundleRequest</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="progress">进度</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<UnityEngine.Object[]></returns>
            public static IUniTaskSource<UnityEngine.Object[]> Create(AssetBundleRequest asyncOperation, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<UnityEngine.Object[]>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AssetBundleRequestAllAssetsConfiguredSource();
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
                        var source = (AssetBundleRequestAllAssetsConfiguredSource)state;
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
            /// <returns>UnityEngine.Object[]</returns>
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
                    core.TrySetResult(asyncOperation.allAssets);
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
                    core.TrySetResult(asyncOperation.allAssets);
                }
            }
        }
    }
}

#endif
#endif