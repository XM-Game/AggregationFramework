#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Cysharp.Threading.Tasks.Internal;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// DelayType 枚举
    /// </summary>
    public enum DelayType
    {
        /// <summary>使用 Time.deltaTime.</summary>
        DeltaTime,
        /// <summary>忽略时间尺度, 使用 Time.unscaledDeltaTime.</summary>
        UnscaledDeltaTime,
        /// <summary>使用 Stopwatch.GetTimestamp().</summary>
        Realtime
    }
    /// <summary>
    /// UniTask 结构体
    /// </summary>
    public partial struct UniTask
    {
        /// <summary>
        /// 等待
        /// </summary>
        /// <returns>YieldAwaitable</returns>
        public static YieldAwaitable Yield()
        {
            // optimized for single continuation
            return new YieldAwaitable(PlayerLoopTiming.Update);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="timing">时间循环类型</param>
        /// <returns>YieldAwaitable</returns>
        public static YieldAwaitable Yield(PlayerLoopTiming timing)
        {
            // optimized for single continuation
            return new YieldAwaitable(timing);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask Yield(CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return new UniTask(YieldPromise.Create(PlayerLoopTiming.Update, cancellationToken, cancelImmediately, out var token), token);
        }

        public static UniTask Yield(PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return new UniTask(YieldPromise.Create(timing, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// 类似于 UniTask.Yield 但保证在下一帧运行.
        /// </summary>
        public static UniTask NextFrame()
        {
            return new UniTask(NextFramePromise.Create(PlayerLoopTiming.Update, CancellationToken.None, false, out var token), token);
        }

        /// <summary>
        /// 类似于 UniTask.Yield 但保证在下一帧运行.
        /// </summary>
        public static UniTask NextFrame(PlayerLoopTiming timing)
        {
            return new UniTask(NextFramePromise.Create(timing, CancellationToken.None, false, out var token), token);
        }

        /// <summary>
        /// 类似于 UniTask.Yield 但保证在下一帧运行.
        /// </summary>
        public static UniTask NextFrame(CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return new UniTask(NextFramePromise.Create(PlayerLoopTiming.Update, cancellationToken, cancelImmediately, out var token), token);
        }

        /// <summary>
        /// 类似于 UniTask.Yield 但保证在下一帧运行.
        /// </summary>
        public static UniTask NextFrame(PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return new UniTask(NextFramePromise.Create(timing, cancellationToken, cancelImmediately, out var token), token);
        }

#if UNITY_2023_1_OR_NEWER
        /// <summary>
        /// 等待帧结束
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask WaitForEndOfFrame(CancellationToken cancellationToken = default)
        {
            await Awaitable.EndOfFrameAsync(cancellationToken);
        }
#else        
        [Obsolete("Use WaitForEndOfFrame(MonoBehaviour) instead or UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate). Equivalent for coroutine's WaitForEndOfFrame requires MonoBehaviour(runner of Coroutine).")]
        /// <summary>
        /// 等待帧结束
        /// </summary>
        /// <returns>YieldAwaitable</returns>
        public static YieldAwaitable WaitForEndOfFrame()
        {
            return UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
        /// <summary>
        /// 等待帧结束
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        [Obsolete("Use WaitForEndOfFrame(MonoBehaviour) instead or UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate). Equivalent for coroutine's WaitForEndOfFrame requires MonoBehaviour(runner of Coroutine).")]
        public static UniTask WaitForEndOfFrame(CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken, cancelImmediately);
        }
#endif        

        /// <summary>
        /// 等待帧结束
        /// </summary>
        /// <param name="coroutineRunner">协程运行器</param>
        /// <returns>UniTask</returns>
        public static UniTask WaitForEndOfFrame(MonoBehaviour coroutineRunner)
        {
            var source = WaitForEndOfFramePromise.Create(coroutineRunner, CancellationToken.None, false, out var token);
            return new UniTask(source, token);
        }

        /// <summary>
        /// 等待帧结束
        /// </summary>
        /// <param name="coroutineRunner">协程运行器</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask WaitForEndOfFrame(MonoBehaviour coroutineRunner, CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            var source = WaitForEndOfFramePromise.Create(coroutineRunner, cancellationToken, cancelImmediately, out var token);
            return new UniTask(source, token);
        }

        /// <summary>
        /// 类似于 UniTask.Yield(PlayerLoopTiming.LastFixedUpdate).
        /// </summary>
        public static YieldAwaitable WaitForFixedUpdate()
        {
            // use LastFixedUpdate instead of FixedUpdate
            // https://github.com/Cysharp/UniTask/issues/377
            return UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
        }

        /// <summary>
        /// 类似于 UniTask.Yield(PlayerLoopTiming.LastFixedUpdate, cancellationToken).
        /// </summary>
        public static UniTask WaitForFixedUpdate(CancellationToken cancellationToken, bool cancelImmediately = false)
        {
            return UniTask.Yield(PlayerLoopTiming.LastFixedUpdate, cancellationToken, cancelImmediately);
        }
        /// <summary>
        /// 等待秒数
        /// </summary>
        /// <param name="duration">秒数</param>
        /// <param name="ignoreTimeScale">是否忽略时间尺度</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
		public static UniTask WaitForSeconds(float duration, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
		{
			return Delay(Mathf.RoundToInt(1000 * duration), ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
		}

        /// <summary>
        /// 等待秒数
        /// </summary>
        /// <param name="duration">秒数</param>
        /// <param name="ignoreTimeScale">是否忽略时间尺度</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
		public static UniTask WaitForSeconds(int duration, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
		{
			return Delay(1000 * duration, ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
		}
        /// <summary>
        /// 延迟帧数
        /// </summary>
        /// <param name="delayFrameCount">延迟帧数</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
		public static UniTask DelayFrame(int delayFrameCount, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            if (delayFrameCount < 0)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. delayFrameCount:" + delayFrameCount);
            }

            return new UniTask(DelayFramePromise.Create(delayFrameCount, delayTiming, cancellationToken, cancelImmediately, out var token), token);
        }
        /// <summary>
        /// 延迟时间
        /// </summary>
        /// <param name="millisecondsDelay">延迟时间</param>
        /// <param name="ignoreTimeScale">是否忽略时间尺度</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask Delay(int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            var delayTimeSpan = TimeSpan.FromMilliseconds(millisecondsDelay);
            return Delay(delayTimeSpan, ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
        }
        /// <summary>
        /// 延迟时间
        /// </summary>
        /// <param name="delayTimeSpan">延迟时间</param>
        /// <param name="ignoreTimeScale">是否忽略时间尺度</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask Delay(TimeSpan delayTimeSpan, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            var delayType = ignoreTimeScale ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            return Delay(delayTimeSpan, delayType, delayTiming, cancellationToken, cancelImmediately);
        }
        /// <summary>
        /// 延迟时间
        /// </summary>
        /// <param name="millisecondsDelay">延迟时间</param>
        /// <param name="delayType">延迟类型</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask Delay(int millisecondsDelay, DelayType delayType, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            var delayTimeSpan = TimeSpan.FromMilliseconds(millisecondsDelay);
            return Delay(delayTimeSpan, delayType, delayTiming, cancellationToken, cancelImmediately);
        }

        /// <summary>
        /// 延迟时间
        /// </summary>
        /// <param name="delayTimeSpan">延迟时间</param>
        /// <param name="delayType">延迟类型</param>
        /// <param name="delayTiming">延迟时间循环类型</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="cancelImmediately">是否立即取消</param>
        /// <returns>UniTask</returns>
        public static UniTask Delay(TimeSpan delayTimeSpan, DelayType delayType, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            if (delayTimeSpan < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("Delay does not allow minus delayTimeSpan. delayTimeSpan:" + delayTimeSpan);
            }

#if UNITY_EDITOR
            // force use Realtime.
            if (PlayerLoopHelper.IsMainThread && !UnityEditor.EditorApplication.isPlaying)
            {
                delayType = DelayType.Realtime;
            }
#endif

            switch (delayType)
            {
                case DelayType.UnscaledDeltaTime:
                    {
                        return new UniTask(DelayIgnoreTimeScalePromise.Create(delayTimeSpan, delayTiming, cancellationToken, cancelImmediately, out var token), token);
                    }
                case DelayType.Realtime:
                    {
                        return new UniTask(DelayRealtimePromise.Create(delayTimeSpan, delayTiming, cancellationToken, cancelImmediately, out var token), token);
                    }
                case DelayType.DeltaTime:
                default:
                    {
                        return new UniTask(DelayPromise.Create(delayTimeSpan, delayTiming, cancellationToken, cancelImmediately, out var token), token);
                    }
            }
        }

        /// <summary>
        /// YieldPromise 类
        /// </summary>
        sealed class YieldPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<YieldPromise>
        {
            static TaskPool<YieldPromise> pool;   // 任务池
            YieldPromise nextNode;   // 下一个节点
            public ref YieldPromise NextNode => ref nextNode;   // 下一个节点引用

            static YieldPromise()   // 静态构造函数
            {
                TaskPool.RegisterSizeGetter(typeof(YieldPromise), () => pool.Size);
            }

            CancellationToken cancellationToken;   // 取消令牌
            CancellationTokenRegistration cancellationTokenRegistration;   // 取消令牌注册
            bool cancelImmediately;   // 是否立即取消
            UniTaskCompletionSourceCore<object> core;   // 任务完成源核心

            YieldPromise()   // 构造函数
            {
            }
            /// <summary>
            /// 创建任务
            /// </summary>
            /// <param name="timing">时间循环类型</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new YieldPromise();
                }

                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;
                
                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (YieldPromise)state;
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
            /// <param name="continuation">继续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                core.TrySetResult(null);
                return false;
            }

            /// <summary>
            /// 尝试返回
            /// </summary>
            /// <returns>bool</returns>
            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }

        /// <summary>
        /// NextFramePromise 类 等待下一帧
        /// </summary>
        sealed class NextFramePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<NextFramePromise>
        {
            static TaskPool<NextFramePromise> pool;   // 任务池
            NextFramePromise nextNode;   // 下一个节点
            public ref NextFramePromise NextNode => ref nextNode;

            static NextFramePromise()
            {
                TaskPool.RegisterSizeGetter(typeof(NextFramePromise), () => pool.Size);
            }

            int frameCount;
            UniTaskCompletionSourceCore<AsyncUnit> core;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;

            NextFramePromise()
            {
            }

            public static IUniTaskSource Create(PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new NextFramePromise();
                }

                result.frameCount = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (NextFramePromise)state;
                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
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

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (frameCount == Time.frameCount)
                {
                    return true;
                }

                core.TrySetResult(AsyncUnit.Default);
                return false;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }
        }
        /// <summary>
        /// WaitForEndOfFramePromise 类 等待帧结束
        /// </summary>
        sealed class WaitForEndOfFramePromise : IUniTaskSource, ITaskPoolNode<WaitForEndOfFramePromise>, System.Collections.IEnumerator
        {
            static TaskPool<WaitForEndOfFramePromise> pool;   // 任务池
            WaitForEndOfFramePromise nextNode;   // 下一个节点
            public ref WaitForEndOfFramePromise NextNode => ref nextNode;

            static WaitForEndOfFramePromise()   // 静态构造函数
            {
                TaskPool.RegisterSizeGetter(typeof(WaitForEndOfFramePromise), () => pool.Size);
            }

            UniTaskCompletionSourceCore<object> core;   // 任务完成源核心
            CancellationToken cancellationToken;   // 取消令牌
            CancellationTokenRegistration cancellationTokenRegistration;   // 取消令牌注册
            bool cancelImmediately;   // 是否立即取消

            WaitForEndOfFramePromise()   // 构造函数
            {
            }
            /// <summary>
            /// 创建任务
            /// </summary>
            /// <param name="coroutineRunner">协程运行器</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(MonoBehaviour coroutineRunner, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new WaitForEndOfFramePromise();
                }

                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (WaitForEndOfFramePromise)state;
                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                coroutineRunner.StartCoroutine(result);

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

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                Reset(); // Reset Enumerator
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                return pool.TryPush(this);
            }

            // Coroutine Runner implementation

            static readonly WaitForEndOfFrame waitForEndOfFrameYieldInstruction = new WaitForEndOfFrame();
            bool isFirst = true;

            object IEnumerator.Current => waitForEndOfFrameYieldInstruction;

            bool IEnumerator.MoveNext()
            {
                if (isFirst)
                {
                    isFirst = false;
                    return true; // start WaitForEndOfFrame
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                core.TrySetResult(null);
                return false;
            }

            public void Reset()
            {
                isFirst = true;
            }
        }

        sealed class DelayFramePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<DelayFramePromise>
        {
            static TaskPool<DelayFramePromise> pool;
            DelayFramePromise nextNode;
            public ref DelayFramePromise NextNode => ref nextNode;

            static DelayFramePromise()
            {
                TaskPool.RegisterSizeGetter(typeof(DelayFramePromise), () => pool.Size);
            }

            int initialFrame;
            int delayFrameCount;
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;
            bool cancelImmediately;

            int currentFrameCount;
            UniTaskCompletionSourceCore<AsyncUnit> core;

            DelayFramePromise()
            {
            }

            public static IUniTaskSource Create(int delayFrameCount, PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new DelayFramePromise();
                }

                result.delayFrameCount = delayFrameCount;
                result.cancellationToken = cancellationToken;
                result.initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (DelayFramePromise)state;
                        promise.core.TrySetCanceled(promise.cancellationToken);
                    }, result);
                }

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                token = result.core.Version;
                return result;
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

            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (currentFrameCount == 0)
                {
                    if (delayFrameCount == 0) // same as Yield
                    {
                        core.TrySetResult(AsyncUnit.Default);
                        return false;
                    }

                    // skip in initial frame.
                    if (initialFrame == Time.frameCount)
                    {
#if UNITY_EDITOR
                        // force use Realtime.
                        if (PlayerLoopHelper.IsMainThread && !UnityEditor.EditorApplication.isPlaying)
                        {
                            //goto ++currentFrameCount
                        }
                        else
                        {
                            return true;
                        }
#else
                        return true;
#endif
                    }
                }

                if (++currentFrameCount >= delayFrameCount)
                {
                    core.TrySetResult(AsyncUnit.Default);
                    return false;
                }

                return true;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                currentFrameCount = default;
                delayFrameCount = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }

        /// <summary>  
        /// DelayPromise 类 延迟
        /// </summary>
        sealed class DelayPromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<DelayPromise>
        {
            static TaskPool<DelayPromise> pool;   // 任务池
            DelayPromise nextNode;   // 下一个节点
            public ref DelayPromise NextNode => ref nextNode;

            static DelayPromise()   // 静态构造函数
            {
                TaskPool.RegisterSizeGetter(typeof(DelayPromise), () => pool.Size);
            }

            int initialFrame;   // 初始帧
            float delayTimeSpan;   // 延迟时间
            float elapsed;   // 已过时间
            CancellationToken cancellationToken;
            CancellationTokenRegistration cancellationTokenRegistration;   // 取消令牌注册
            bool cancelImmediately;

            UniTaskCompletionSourceCore<object> core;   // 任务完成源核心

            DelayPromise()   // 构造函数
            {
            }
            /// <summary>
            /// 创建任务
            /// </summary>
            /// <param name="delayTimeSpan">延迟时间</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(TimeSpan delayTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new DelayPromise();
                }

                result.elapsed = 0.0f;
                result.delayTimeSpan = (float)delayTimeSpan.TotalSeconds;
                result.cancellationToken = cancellationToken;
                result.initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (DelayPromise)state;
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
            /// <param name="continuation">继续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (elapsed == 0.0f)
                {
                    if (initialFrame == Time.frameCount)
                    {
                        return true;
                    }
                }

                elapsed += Time.deltaTime;
                if (elapsed >= delayTimeSpan)
                {
                    core.TrySetResult(null);
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
                delayTimeSpan = default;
                elapsed = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }

        /// <summary>
        /// DelayIgnoreTimeScalePromise 类 忽略时间尺度
        /// </summary>
        sealed class DelayIgnoreTimeScalePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<DelayIgnoreTimeScalePromise>
        {
            static TaskPool<DelayIgnoreTimeScalePromise> pool;   // 任务池
            DelayIgnoreTimeScalePromise nextNode;   // 下一个节点
            public ref DelayIgnoreTimeScalePromise NextNode => ref nextNode;

            static DelayIgnoreTimeScalePromise()   // 静态构造函数
            {
                TaskPool.RegisterSizeGetter(typeof(DelayIgnoreTimeScalePromise), () => pool.Size);
            }

            float delayFrameTimeSpan;   // 延迟帧时间
            float elapsed;   // 已过时间
            int initialFrame;   // 初始帧
            CancellationToken cancellationToken;   // 取消令牌
            CancellationTokenRegistration cancellationTokenRegistration;   // 取消令牌注册
            bool cancelImmediately;   // 是否立即取消

            UniTaskCompletionSourceCore<object> core;   // 任务完成源核心

            DelayIgnoreTimeScalePromise()   // 构造函数
            {
            }
            /// <summary>
            /// 创建任务
            /// </summary>
            /// <param name="delayFrameTimeSpan">延迟帧时间</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(TimeSpan delayFrameTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new DelayIgnoreTimeScalePromise();
                }

                result.elapsed = 0.0f;
                result.delayFrameTimeSpan = (float)delayFrameTimeSpan.TotalSeconds;
                result.initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (DelayIgnoreTimeScalePromise)state;
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
            /// <param name="continuation">继续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (elapsed == 0.0f)
                {
                    if (initialFrame == Time.frameCount)
                    {
                        return true;
                    }
                }

                elapsed += Time.unscaledDeltaTime;
                if (elapsed >= delayFrameTimeSpan)
                {
                    core.TrySetResult(null);
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
                delayFrameTimeSpan = default;
                elapsed = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }

        /// <summary>
        /// DelayRealtimePromise 类
        /// </summary>
        sealed class DelayRealtimePromise : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<DelayRealtimePromise>
        {
            static TaskPool<DelayRealtimePromise> pool;   // 任务池
            DelayRealtimePromise nextNode;   // 下一个节点
            public ref DelayRealtimePromise NextNode => ref nextNode;

            static DelayRealtimePromise()   // 静态构造函数
            {
                TaskPool.RegisterSizeGetter(typeof(DelayRealtimePromise), () => pool.Size);
            }

            long delayTimeSpanTicks;   // 延迟时间戳
            ValueStopwatch stopwatch;   // 停止计时器
            CancellationToken cancellationToken;   // 取消令牌
            CancellationTokenRegistration cancellationTokenRegistration;   // 取消令牌注册
            bool cancelImmediately;   // 是否立即取消

            UniTaskCompletionSourceCore<AsyncUnit> core;   // 任务完成源核心

            DelayRealtimePromise()   // 构造函数
            {
            }
            /// <summary>
            /// 创建任务
            /// </summary>
            /// <param name="delayTimeSpan">延迟时间</param>
            /// <param name="timing">时间循环类型</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="cancelImmediately">是否立即取消</param>
            /// <param name="token">令牌</param>
            /// <returns>IUniTaskSource</returns>
            public static IUniTaskSource Create(TimeSpan delayTimeSpan, PlayerLoopTiming timing, CancellationToken cancellationToken, bool cancelImmediately, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new DelayRealtimePromise();
                }

                result.stopwatch = ValueStopwatch.StartNew();
                result.delayTimeSpanTicks = delayTimeSpan.Ticks;
                result.cancellationToken = cancellationToken;
                result.cancelImmediately = cancelImmediately;

                if (cancelImmediately && cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(state =>
                    {
                        var promise = (DelayRealtimePromise)state;
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
            /// <param name="continuation">继续</param>
            /// <param name="state">状态</param>
            /// <param name="token">令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            /// <returns>bool</returns>
            public bool MoveNext()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (stopwatch.IsInvalid)
                {
                    core.TrySetResult(AsyncUnit.Default);
                    return false;
                }

                if (stopwatch.ElapsedTicks >= delayTimeSpanTicks)
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
                stopwatch = default;
                cancellationToken = default;
                cancellationTokenRegistration.Dispose();
                cancelImmediately = default;
                return pool.TryPush(this);
            }
        }
    }

    /// <summary>
    /// YieldAwaitable 类
    /// </summary>
    public readonly struct YieldAwaitable
    {
        readonly PlayerLoopTiming timing;   // 时间循环类型

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timing">时间循环类型</param>
        public YieldAwaitable(PlayerLoopTiming timing)
        {
            this.timing = timing;
        }

        /// <summary>
        /// 获取等待器
        /// </summary>
        /// <returns>Awaiter</returns>
        public Awaiter GetAwaiter()
        {
            return new Awaiter(timing);
        }

        /// <summary>
        /// 转换为 UniTask
        /// </summary>
        /// <returns>UniTask</returns>
        public UniTask ToUniTask()
        {
            return UniTask.Yield(timing, CancellationToken.None);
        }

        /// <summary>
        /// Awaiter 类
        /// </summary>
        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            readonly PlayerLoopTiming timing;   // 时间循环类型 

            public Awaiter(PlayerLoopTiming timing)
            {
                this.timing = timing;
            }

            public bool IsCompleted => false;

            public void GetResult() { }

            public void OnCompleted(Action continuation)
            {
                PlayerLoopHelper.AddContinuation(timing, continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                PlayerLoopHelper.AddContinuation(timing, continuation);
            }
        }
    }
}
