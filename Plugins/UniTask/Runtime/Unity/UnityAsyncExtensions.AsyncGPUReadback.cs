 #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UnityEngine.Rendering;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UnityAsyncExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 GetAwaiter 方法，用于获取 AsyncGPUReadbackRequest 的异步等待器
    /// </remarks>
    public static partial class UnityAsyncExtensions
    {
        #region AsyncGPUReadbackRequest

        /// <summary>
        /// 获取 AsyncGPUReadbackRequest 的异步等待器
        /// </summary>
        /// <param name="asyncOperation">AsyncGPUReadbackRequest</param>
        /// <returns>UniTask<AsyncGPUReadbackRequest>.Awaiter</returns>
        public static UniTask<AsyncGPUReadbackRequest>.Awaiter GetAwaiter(this AsyncGPUReadbackRequest asyncOperation)
        {
            return ToUniTask(asyncOperation).GetAwaiter();
        }

        /// <summary>
        /// 将 AsyncGPUReadbackRequest 转换为 UniTask，并附加取消令牌
        /// </summary>
        /// <param name="asyncOperation">AsyncGPUReadbackRequest</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask<AsyncGPUReadbackRequest></returns>
        public static UniTask<AsyncGPUReadbackRequest> WithCancellation(this AsyncGPUReadbackRequest asyncOperation, CancellationToken cancellationToken)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken);
        }

        public static UniTask<AsyncGPUReadbackRequest> WithCancellation(this AsyncGPUReadbackRequest asyncOperation, CancellationToken cancellationToken, bool cancelImmediately)
        {
            return ToUniTask(asyncOperation, cancellationToken: cancellationToken, cancelImmediately: cancelImmediately);
        }
        /// <summary>
        /// 将 AsyncGPUReadbackRequest 转换为 UniTask
        /// </summary>
        /// <param name="asyncOperation">AsyncGPUReadbackRequest</param>
        /// <param name="timing">时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask<AsyncGPUReadbackRequest></returns>
        public static UniTask<AsyncGPUReadbackRequest> ToUniTask(this AsyncGPUReadbackRequest asyncOperation, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            if (asyncOperation.done) return UniTask.FromResult(asyncOperation);
            return new UniTask<AsyncGPUReadbackRequest>(AsyncGPUReadbackRequestAwaiterConfiguredSource.Create(asyncOperation, timing, cancellationToken, cancelImmediately, out var token), token);
        }
        
        /// <summary>
        /// AsyncGPUReadbackRequestAwaiterConfiguredSource 类
        /// </summary>
        /// <remarks>
        /// 实现 IUniTaskSource<AsyncGPUReadbackRequest> 和 IPlayerLoopItem 接口
        /// </remarks>
        sealed class AsyncGPUReadbackRequestAwaiterConfiguredSource : IUniTaskSource<AsyncGPUReadbackRequest>, IPlayerLoopItem, ITaskPoolNode<AsyncGPUReadbackRequestAwaiterConfiguredSource>
        {
            static TaskPool<AsyncGPUReadbackRequestAwaiterConfiguredSource> pool;
            AsyncGPUReadbackRequestAwaiterConfiguredSource nextNode;
            public ref AsyncGPUReadbackRequestAwaiterConfiguredSource NextNode => ref nextNode;

            static AsyncGPUReadbackRequestAwaiterConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(AsyncGPUReadbackRequestAwaiterConfiguredSource), () => pool.Size);
            }

            AsyncGPUReadbackRequest asyncOperation;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;
            UniTaskCompletionSourceCore<AsyncGPUReadbackRequest> core;

            AsyncGPUReadbackRequestAwaiterConfiguredSource()
            {
            }

            /// <summary>
            /// 创建 AsyncGPUReadbackRequestAwaiterConfiguredSource
            /// </summary>
            /// <param name="asyncOperation">AsyncGPUReadbackRequest</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource<AsyncGPUReadbackRequest></returns>
            public static IUniTaskSource<AsyncGPUReadbackRequest> Create(AsyncGPUReadbackRequest asyncOperation, PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<AsyncGPUReadbackRequest>.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new AsyncGPUReadbackRequestAwaiterConfiguredSource();
                }

                result.asyncOperation = asyncOperation;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (AsyncGPUReadbackRequestAwaiterConfiguredSource)state;
                        promise.core.TrySetCanceled(promise.cancellationToken);
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
            /// <returns>AsyncGPUReadbackRequest</returns>
            public AsyncGPUReadbackRequest GetResult(short token)
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
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (asyncOperation.hasError)
                {
                    core.TrySetException(new Exception("AsyncGPUReadbackRequest.hasError = true"));
                    return false;
                }

                if (asyncOperation.done)
                {
                    core.TrySetResult(asyncOperation);
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
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }

        #endregion
    }
}