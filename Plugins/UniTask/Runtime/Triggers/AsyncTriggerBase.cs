#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
    /// <summary>
    /// 所有异步触发器的基类：
    /// - 将 Unity 生命周期/事件转换为 IUniTaskAsyncEnumerable<T> 流，可被 await/异步 foreach。
    /// - 统一管理处理器列表（Add/Remove）、取消与销毁时的完成通知。
    /// - 通过内部 AsyncTriggerEnumerator 提供 MoveNextAsync/DisposeAsync，无需额外 GC。
    /// </summary>
    public abstract class AsyncTriggerBase<T> : MonoBehaviour, IUniTaskAsyncEnumerable<T>
    {
        TriggerEvent<T> triggerEvent;

        internal protected bool calledAwake;
        internal protected bool calledDestroy;

        void Awake()
        {
            calledAwake = true;
        }

        void OnDestroy()
        {
            if (calledDestroy) return;
            calledDestroy = true;

            triggerEvent.SetCompleted();
        }

        /// <summary>
        /// 注册新的触发处理器；若 Awake 尚未调用，则通过 PlayerLoop 监视保证安全执行。
        /// </summary>
        internal void AddHandler(ITriggerHandler<T> handler)
        {
            if (!calledAwake)
            {
                PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
            }

            triggerEvent.Add(handler);
        }

        /// <summary>
        /// 解除触发处理器注册；同样在 Awake 前通过 PlayerLoop 监视。
        /// </summary>
        internal void RemoveHandler(ITriggerHandler<T> handler)
        {
            if (!calledAwake)
            {
                PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
            }

            triggerEvent.Remove(handler);
        }

        /// <summary>
        /// 在派生触发器中调用，向所有等待者广播一个值。
        /// </summary>
        protected void RaiseEvent(T value)
        {
            triggerEvent.SetResult(value);
        }

        /// <summary>
        /// 生成异步枚举器，支持取消令牌；每次 MoveNextAsync 返回是否还有事件。
        /// </summary>
        public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncTriggerEnumerator(this, cancellationToken);
        }

        /// <summary>
        /// 每个订阅者对应的枚举器：负责注册到父触发器、监听取消、返回事件值。
        /// 继承 MoveNextSource 实现无分配的 await。
        /// </summary>
        sealed class AsyncTriggerEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, ITriggerHandler<T>
        {
            static Action<object> cancellationCallback = CancellationCallback;

            readonly AsyncTriggerBase<T> parent;
            CancellationToken cancellationToken;
            CancellationTokenRegistration registration;
            bool called;
            bool isDisposed;

            public AsyncTriggerEnumerator(AsyncTriggerBase<T> parent, CancellationToken cancellationToken)
            {
                this.parent = parent;
                this.cancellationToken = cancellationToken;
            }

            public void OnCanceled(CancellationToken cancellationToken = default)
            {
                completionSource.TrySetCanceled(cancellationToken);
            }

            public void OnNext(T value)
            {
                Current = value;
                completionSource.TrySetResult(true);
            }

            public void OnCompleted()
            {
                completionSource.TrySetResult(false);
            }

            public void OnError(Exception ex)
            {
                completionSource.TrySetException(ex);
            }

            static void CancellationCallback(object state)
            {
                var self = (AsyncTriggerEnumerator)state;
                self.DisposeAsync().Forget(); // sync

                self.completionSource.TrySetCanceled(self.cancellationToken);
            }

            public T Current { get; private set; }
            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public UniTask<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                completionSource.Reset();

                if (!called)
                {
                    called = true;

                    TaskTracker.TrackActiveTask(this, 3);
                    parent.AddHandler(this);
                    if (cancellationToken.CanBeCanceled)
                    {
                        registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                    }
                }

                return new UniTask<bool>(this, completionSource.Version);
            }

            public UniTask DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    TaskTracker.RemoveTracking(this);
                    registration.Dispose();
                    parent.RemoveHandler(this);
                }

                return default;
            }
        }

        /// <summary>
        /// 在 PlayerLoop 中等待 Awake 调用完成，防止在 Awake 之前注册/注销触发器导致异常。
        /// </summary>
        class AwakeMonitor : IPlayerLoopItem
        {
            readonly AsyncTriggerBase<T> trigger;

            public AwakeMonitor(AsyncTriggerBase<T> trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.calledAwake) return false;
                if (trigger == null)
                {
                    trigger.OnDestroy();
                    return false;
                }
                return true;
            }
        }
    }

    /// <summary>
    /// IAsyncOneShotTrigger 接口
    /// </summary>
    /// <remarks>
    /// 用于表示一次性的异步触发器
    /// </remarks>
    public interface IAsyncOneShotTrigger
    /// <summary>
    /// 执行一次异步触发
    /// </summary>
    /// <returns>UniTask</returns>
    {
        UniTask OneShotAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOneShotTrigger
    {
        UniTask IAsyncOneShotTrigger.OneShotAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)this, core.Version);
        }
    }
    /// <summary>
    /// AsyncTriggerHandler 类
    /// </summary>
    /// <remarks>
    /// 继承自 IUniTaskSource<T>, ITriggerHandler<T>, IDisposable
    /// </remarks>
    public sealed partial class AsyncTriggerHandler<T> : IUniTaskSource<T>, ITriggerHandler<T>, IDisposable
    {
        /// <summary>
        /// 取消回调
        /// </summary>
        /// <param name="state">状态</param>
        static Action<object> cancellationCallback = CancellationCallback;

        readonly AsyncTriggerBase<T> trigger;   // 触发器

        CancellationToken cancellationToken;       // 取消令牌
        CancellationTokenRegistration registration; // 取消令牌注册
        bool isDisposed;                         // 是否已销毁
        bool callOnce;                           // 是否只调用一次

        UniTaskCompletionSourceCore<T> core;      // 完成源

        internal CancellationToken CancellationToken => cancellationToken; // 取消令牌

        ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; } // 前一个触发器
        ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; } // 下一个触发器

        /// <summary>
        /// AsyncTriggerHandler 构造函数
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="callOnce">是否只调用一次</param>
        internal AsyncTriggerHandler(AsyncTriggerBase<T> trigger, bool callOnce)
        {
            if (cancellationToken.IsCancellationRequested) // 如果取消令牌已取消
            {
                isDisposed = true; // 设置已销毁
                return;
            }

            this.trigger = trigger; // 设置触发器
            this.cancellationToken = default;
            this.registration = default;
            this.callOnce = callOnce;

            trigger.AddHandler(this);

            TaskTracker.TrackActiveTask(this, 3);
        }

        /// <summary>
        /// AsyncTriggerHandler 构造函数
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="callOnce">是否只调用一次</param>
        internal AsyncTriggerHandler(AsyncTriggerBase<T> trigger, CancellationToken cancellationToken, bool callOnce)
        {
            if (cancellationToken.IsCancellationRequested) // 如果取消令牌已取消
            {
                isDisposed = true; // 设置已销毁
                return;
            }

            this.trigger = trigger; // 设置触发器
            this.cancellationToken = cancellationToken;
            this.callOnce = callOnce; // 设置是否只调用一次

            trigger.AddHandler(this);

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }

        static void CancellationCallback(object state)
        {
            var self = (AsyncTriggerHandler<T>)state;
            self.Dispose();

            self.core.TrySetCanceled(self.cancellationToken);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                trigger.RemoveHandler(this);
            }
        }

        T IUniTaskSource<T>.GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="value">结果</param>
        void ITriggerHandler<T>.OnNext(T value)
        {
            core.TrySetResult(value);
        }

        /// <summary>
        /// 设置取消
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        void ITriggerHandler<T>.OnCanceled(CancellationToken cancellationToken)
        {
            core.TrySetCanceled(cancellationToken);
        }

        /// <summary>
        /// 设置完成
        /// </summary>
        void ITriggerHandler<T>.OnCompleted()
        {
            core.TrySetCanceled(CancellationToken.None);
        }

        /// <summary>
        /// 设置错误
        /// </summary>
        /// <param name="ex">异常</param>
        void ITriggerHandler<T>.OnError(Exception ex)
        {
            core.TrySetException(ex);
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="token">令牌</param>
        void IUniTaskSource.GetResult(short token)
        {
            ((IUniTaskSource<T>)this).GetResult(token);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="token">令牌</param>
        UniTaskStatus IUniTaskSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        /// <summary>
        /// 获取不安全状态
        /// </summary>
        UniTaskStatus IUniTaskSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        /// <summary>
        /// 设置完成
        /// </summary>
        /// <param name="continuation">完成委托</param>
        /// <param name="state">状态</param>
        /// <param name="token">令牌</param>
        void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }
    }
}