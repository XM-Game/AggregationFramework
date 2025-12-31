using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{   
    /// <summary>
    /// UniTaskAsyncEnumerable 类
    /// </summary>
    public static partial class UniTaskAsyncEnumerable
    {
        /// <summary>
        /// 每隔一段时间执行一次
        /// </summary>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> EveryUpdate(PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            return new EveryUpdate(updateTiming, cancelImmediately);
        }
    }

    /// <summary>
    /// EveryUpdate 类
    /// </summary>
    internal class EveryUpdate : IUniTaskAsyncEnumerable<AsyncUnit>
    {
        readonly PlayerLoopTiming updateTiming;
        readonly bool cancelImmediately;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public EveryUpdate(PlayerLoopTiming updateTiming, bool cancelImmediately)
        {
            this.updateTiming = updateTiming;
            this.cancelImmediately = cancelImmediately;
        }
        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _EveryUpdate(updateTiming, cancellationToken, cancelImmediately);
        }

        /// <summary>
        /// _EveryUpdate 类
        /// </summary>
        class _EveryUpdate : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IPlayerLoopItem
        {
            readonly PlayerLoopTiming updateTiming;
            readonly CancellationToken cancellationToken;
            readonly CancellationTokenRegistration cancellationTokenRegistration;

            bool disposed;

            public _EveryUpdate(PlayerLoopTiming updateTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.updateTiming = updateTiming;
                this.cancellationToken = cancellationToken;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_EveryUpdate)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }

                TaskTracker.TrackActiveTask(this, 2);
                PlayerLoopHelper.AddAction(updateTiming, this);
            }
            /// <summary>
            /// 当前值
            /// </summary>
            public AsyncUnit Current => default;
            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public UniTask<bool> MoveNextAsync()
            {
                if (disposed) return CompletedTasks.False;
                
                completionSource.Reset();

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }
                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 释放异步枚举器
            /// </summary>
            /// <returns>UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!disposed)
                {
                    cancellationTokenRegistration.Dispose();
                    disposed = true;
                    TaskTracker.RemoveTracking(this);
                }
                return default;
            }

            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }
                
                if (disposed)
                {
                    completionSource.TrySetResult(false);
                    return false;
                }

                completionSource.TrySetResult(true);
                return true;
            }
        }
    }
}