using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Linq
{   
    /// <summary>
    /// UniTaskAsyncEnumerable 类
    /// </summary>
    public static partial class UniTaskAsyncEnumerable
    {   
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="dueTime">延迟时间</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> Timer(TimeSpan dueTime, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false, bool cancelImmediately = false)
        {
            return new Timer(dueTime, null, updateTiming, ignoreTimeScale, cancelImmediately);
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="dueTime">延迟时间</param>
        /// <param name="period">周期时间</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> Timer(TimeSpan dueTime, TimeSpan period, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false, bool cancelImmediately = false)
        {
            return new Timer(dueTime, period, updateTiming, ignoreTimeScale, cancelImmediately);
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="period">周期时间</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> Interval(TimeSpan period, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false, bool cancelImmediately = false)
        {
            return new Timer(period, period, updateTiming, ignoreTimeScale, cancelImmediately);
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="dueTimeFrameCount">延迟帧数</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> TimerFrame(int dueTimeFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            if (dueTimeFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. dueTimeFrameCount:" + dueTimeFrameCount);
            }

            return new TimerFrame(dueTimeFrameCount, null, updateTiming, cancelImmediately);
        }
        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="dueTimeFrameCount">延迟帧数</param>
        /// <param name="periodFrameCount">周期帧数</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> TimerFrame(int dueTimeFrameCount, int periodFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            if (dueTimeFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. dueTimeFrameCount:" + dueTimeFrameCount);
            }
            if (periodFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus periodFrameCount. periodFrameCount:" + dueTimeFrameCount);
            }

            return new TimerFrame(dueTimeFrameCount, periodFrameCount, updateTiming, cancelImmediately);
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="intervalFrameCount">周期帧数</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTaskAsyncEnumerable</returns>
        public static IUniTaskAsyncEnumerable<AsyncUnit> IntervalFrame(int intervalFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            if (intervalFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus intervalFrameCount. intervalFrameCount:" + intervalFrameCount);
            }
            return new TimerFrame(intervalFrameCount, intervalFrameCount, updateTiming, cancelImmediately);
        }
    }

    /// <summary>
    /// Timer 类
    /// </summary>
    internal class Timer : IUniTaskAsyncEnumerable<AsyncUnit>
    {
        readonly PlayerLoopTiming updateTiming; // 更新时机
        readonly TimeSpan dueTime; // 延迟时间
        readonly TimeSpan? period; // 周期时间
        readonly bool ignoreTimeScale; // 是否忽略时间缩放
        readonly bool cancelImmediately; // 是否立即取消

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dueTime">延迟时间</param>
        /// <param name="period">周期时间</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public Timer(TimeSpan dueTime, TimeSpan? period, PlayerLoopTiming updateTiming, bool ignoreTimeScale, bool cancelImmediately)
        {
            this.updateTiming = updateTiming;
            this.dueTime = dueTime;
            this.period = period;
            this.ignoreTimeScale = ignoreTimeScale;
            this.cancelImmediately = cancelImmediately;
        }
        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>IUniTaskAsyncEnumerator</returns>
        public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Timer(dueTime, period, updateTiming, ignoreTimeScale, cancellationToken, cancelImmediately);   
        }
        /// <summary>
        /// _Timer 类
        /// </summary>
        class _Timer : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IPlayerLoopItem
        {
            readonly float dueTime; // 延迟时间
            readonly float? period; // 周期时间
            readonly PlayerLoopTiming updateTiming; // 更新时机
            readonly bool ignoreTimeScale; // 是否忽略时间缩放
            readonly CancellationToken cancellationToken; // 取消令牌
            readonly CancellationTokenRegistration cancellationTokenRegistration; // 取消令牌注册
            int initialFrame; // 初始帧
            float elapsed; // 已过时间
            bool dueTimePhase; // 是否到达延迟时间
            bool completed; // 是否完成
            bool disposed; // 是否已释放
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="dueTime">延迟时间</param>
            /// <param name="period">周期时间</param>
            /// <param name="updateTiming">更新时机</param>
            /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            public _Timer(TimeSpan dueTime, TimeSpan? period, PlayerLoopTiming updateTiming, bool ignoreTimeScale, CancellationToken cancellationToken, bool cancelImmediately)
            {
                this.dueTime = (float)dueTime.TotalSeconds;
                this.period = (period == null) ? null : (float?)period.Value.TotalSeconds;

                if (this.dueTime <= 0) this.dueTime = 0;
                if (this.period != null)
                {
                    if (this.period <= 0) this.period = 1;
                }

                this.initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                this.dueTimePhase = true;
                this.updateTiming = updateTiming;
                this.ignoreTimeScale = ignoreTimeScale;
                this.cancellationToken = cancellationToken;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_Timer)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }
                
                TaskTracker.TrackActiveTask(this, 2);
                PlayerLoopHelper.AddAction(updateTiming, this);
            }

            public AsyncUnit Current => default;
            /// <summary>
            /// 移动到下一个值
            /// </summary>
            /// <returns>是否成功</returns>
            public UniTask<bool> MoveNextAsync()
            {
                // return false instead of throw
                if (disposed || completed) return CompletedTasks.False;

                // reset value here.
                this.elapsed = 0;

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
                if (disposed)
                {
                    completionSource.TrySetResult(false);
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                    return false;
                }                

                if (dueTimePhase)
                {
                    if (elapsed == 0)
                    {
                        // skip in initial frame.
                        if (initialFrame == Time.frameCount)
                        {
                            return true;
                        }
                    }

                    elapsed += (ignoreTimeScale) ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;

                    if (elapsed >= dueTime)
                    {
                        dueTimePhase = false;
                        completionSource.TrySetResult(true);
                    }
                }
                else
                {
                    if (period == null)
                    {
                        completed = true;
                        completionSource.TrySetResult(false);
                        return false;
                    }

                    elapsed += (ignoreTimeScale) ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;

                    if (elapsed >= period)
                    {
                        completionSource.TrySetResult(true);
                    }
                }

                return true;
            }
        }
    }

    /// <summary>
    /// TimerFrame 类
    /// </summary>
    internal class TimerFrame : IUniTaskAsyncEnumerable<AsyncUnit>
    {
        readonly PlayerLoopTiming updateTiming; // 更新时机
        readonly int dueTimeFrameCount; // 延迟帧数
        readonly int? periodFrameCount; // 周期帧数
        readonly bool cancelImmediately;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dueTimeFrameCount">延迟帧数</param>
        /// <param name="periodFrameCount">周期帧数</param>
        /// <param name="updateTiming">更新时机</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        public TimerFrame(int dueTimeFrameCount, int? periodFrameCount, PlayerLoopTiming updateTiming, bool cancelImmediately)
        {
            this.updateTiming = updateTiming;
            this.dueTimeFrameCount = dueTimeFrameCount;
            this.periodFrameCount = periodFrameCount;
            this.cancelImmediately = cancelImmediately;
        }
        /// <summary>
        /// 获取异步枚举器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>IUniTaskAsyncEnumerator</returns>
        public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _TimerFrame(dueTimeFrameCount, periodFrameCount, updateTiming, cancellationToken, cancelImmediately);
        }
        /// <summary>
        /// _TimerFrame 类
        /// </summary>
        class _TimerFrame : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IPlayerLoopItem
        {
            readonly int dueTimeFrameCount; // 延迟帧数
            readonly int? periodFrameCount;
            readonly CancellationToken cancellationToken; // 取消令牌
            readonly CancellationTokenRegistration cancellationTokenRegistration; // 取消令牌注册
            int initialFrame; // 初始帧
            int currentFrame; // 当前帧
            bool dueTimePhase; // 是否到达延迟时间
            bool completed; // 是否完成
            bool disposed; // 是否已释放

            public _TimerFrame(int dueTimeFrameCount, int? periodFrameCount, PlayerLoopTiming updateTiming, CancellationToken cancellationToken, bool cancelImmediately)
            {
                if (dueTimeFrameCount <= 0) dueTimeFrameCount = 0;
                if (periodFrameCount != null)
                {
                    if (periodFrameCount <= 0) periodFrameCount = 1;
                }

                this.initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                this.dueTimePhase = true;
                this.dueTimeFrameCount = dueTimeFrameCount;
                this.periodFrameCount = periodFrameCount;
                this.cancellationToken = cancellationToken;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var source = (_TimerFrame)state;
                        source.completionSource.TrySetCanceled(source.cancellationToken);
                    }, this);
                }

                TaskTracker.TrackActiveTask(this, 2);
                PlayerLoopHelper.AddAction(updateTiming, this);
            }

            public AsyncUnit Current => default;

            public UniTask<bool> MoveNextAsync()
            {
                if (disposed || completed) return CompletedTasks.False;

                if (cancellationToken.IsCancellationRequested)
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }

                // reset value here.
                this.currentFrame = 0;
                completionSource.Reset();
                return new UniTask<bool>(this, completionSource.Version);
            }

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

                if (dueTimePhase)
                {
                    if (currentFrame == 0)
                    {
                        if (dueTimeFrameCount == 0)
                        {
                            dueTimePhase = false;
                            completionSource.TrySetResult(true);
                            return true;
                        }

                        // skip in initial frame.
                        if (initialFrame == Time.frameCount)
                        {
                            return true;
                        }
                    }

                    if (++currentFrame >= dueTimeFrameCount)
                    {
                        dueTimePhase = false;
                        completionSource.TrySetResult(true);
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (periodFrameCount == null)
                    {
                        completed = true;
                        completionSource.TrySetResult(false);
                        return false;
                    }

                    if (++currentFrame >= periodFrameCount)
                    {
                        completionSource.TrySetResult(true);
                    }
                }

                return true;
            }
        }
    }
}