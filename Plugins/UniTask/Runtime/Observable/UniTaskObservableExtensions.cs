#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UniTask 和 IObservable (Rx) 之间的桥接扩展方法
    /// 提供 UniTask 与 Reactive Extensions 之间的互操作支持
    /// </summary>
    public static class UniTaskObservableExtensions
    {
        /// <summary>
        /// 将 IObservable 转换为 UniTask
        /// 订阅可观察序列，等待序列完成并返回结果
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="source">源可观察序列</param>
        /// <param name="useFirstValue">如果为 true，使用第一个值并立即完成；如果为 false，等待序列完成并使用最后一个值</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>表示序列结果的 UniTask</returns>
        public static UniTask<T> ToUniTask<T>(this IObservable<T> source, bool useFirstValue = false, CancellationToken cancellationToken = default)
        {
            var promise = new UniTaskCompletionSource<T>();
            var disposable = new SingleAssignmentDisposable();

            // 根据 useFirstValue 参数选择不同的观察者实现
            var observer = useFirstValue
                ? (IObserver<T>)new FirstValueToUniTaskObserver<T>(promise, disposable, cancellationToken)
                : (IObserver<T>)new ToUniTaskObserver<T>(promise, disposable, cancellationToken);

            try
            {
                // 订阅可观察序列
                disposable.Disposable = source.Subscribe(observer);
            }
            catch (Exception ex)
            {
                // 如果订阅时发生异常，直接设置异常结果
                promise.TrySetException(ex);
            }

            return promise.Task;
        }

        /// <summary>
        /// 将 UniTask 转换为 IObservable
        /// 创建一个可观察序列，当任务完成时产生值
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="task">源 UniTask</param>
        /// <returns>可观察序列，当任务完成时产生结果值</returns>
        public static IObservable<T> ToObservable<T>(this UniTask<T> task)
        {
            // 如果任务已经完成，直接返回结果或异常
            if (task.Status.IsCompleted())
            {
                try
                {
                    // 任务成功完成，返回包含结果的 ReturnObservable
                    return new ReturnObservable<T>(task.GetAwaiter().GetResult());
                }
                catch (Exception ex)
                {
                    // 任务失败，返回抛出异常的 ThrowObservable
                    return new ThrowObservable<T>(ex);
                }
            }

            // 任务未完成，创建 AsyncSubject 并异步等待任务完成
            var subject = new AsyncSubject<T>();
            Fire(subject, task).Forget();
            return subject;
        }

        /// <summary>
        /// 将 UniTask 转换为 IObservable
        /// 理想情况下应该返回 IObservable[Unit]，但 Cysharp.Threading.Tasks 没有 Unit 类型，所以返回 AsyncUnit
        /// </summary>
        /// <param name="task">源 UniTask</param>
        /// <returns>可观察序列，当任务完成时产生 AsyncUnit.Default</returns>
        public static IObservable<AsyncUnit> ToObservable(this UniTask task)
        {
            // 如果任务已经完成，直接返回结果或异常
            if (task.Status.IsCompleted())
            {
                try
                {
                    // 任务成功完成，返回 AsyncUnit.Default
                    task.GetAwaiter().GetResult();
                    return new ReturnObservable<AsyncUnit>(AsyncUnit.Default);
                }
                catch (Exception ex)
                {
                    // 任务失败，返回抛出异常的 ThrowObservable
                    return new ThrowObservable<AsyncUnit>(ex);
                }
            }

            // 任务未完成，创建 AsyncSubject 并异步等待任务完成
            var subject = new AsyncSubject<AsyncUnit>();
            Fire(subject, task).Forget();
            return subject;
        }

        /// <summary>
        /// 异步等待任务完成并通知 AsyncSubject
        /// 当任务完成时，将结果发送给观察者并完成序列
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="subject">AsyncSubject，用于通知观察者</param>
        /// <param name="task">要等待的 UniTask</param>
        static async UniTaskVoid Fire<T>(AsyncSubject<T> subject, UniTask<T> task)
        {
            T value;
            try
            {
                // 等待任务完成
                value = await task;
            }
            catch (Exception ex)
            {
                // 任务失败，通知观察者错误
                subject.OnError(ex);
                return;
            }

            // 任务成功，发送结果并完成序列
            subject.OnNext(value);
            subject.OnCompleted();
        }

        /// <summary>
        /// 异步等待任务完成并通知 AsyncSubject（无返回值版本）
        /// 当任务完成时，发送 AsyncUnit.Default 给观察者并完成序列
        /// </summary>
        /// <param name="subject">AsyncSubject，用于通知观察者</param>
        /// <param name="task">要等待的 UniTask</param>
        static async UniTaskVoid Fire(AsyncSubject<AsyncUnit> subject, UniTask task)
        {
            try
            {
                // 等待任务完成
                await task;
            }
            catch (Exception ex)
            {
                // 任务失败，通知观察者错误
                subject.OnError(ex);
                return;
            }

            // 任务成功，发送 AsyncUnit.Default 并完成序列
            subject.OnNext(AsyncUnit.Default);
            subject.OnCompleted();
        }

        /// <summary>
        /// 将可观察序列转换为 UniTask 的观察者实现
        /// 等待序列完成，使用最后一个值作为结果
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        class ToUniTaskObserver<T> : IObserver<T>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static readonly Action<object> callback = OnCanceled;

            /// <summary>
            /// UniTask 完成源，用于设置任务结果
            /// </summary>
            readonly UniTaskCompletionSource<T> promise;
            
            /// <summary>
            /// 单次赋值可释放对象，用于管理订阅
            /// </summary>
            readonly SingleAssignmentDisposable disposable;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            readonly CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            readonly CancellationTokenRegistration registration;

            /// <summary>
            /// 是否已接收到值
            /// </summary>
            bool hasValue;
            
            /// <summary>
            /// 最新的值（最后一个值）
            /// </summary>
            T latestValue;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="promise">UniTask 完成源</param>
            /// <param name="disposable">单次赋值可释放对象</param>
            /// <param name="cancellationToken">取消令牌</param>
            public ToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
            {
                this.promise = promise;
                this.disposable = disposable;
                this.cancellationToken = cancellationToken;

                // 如果取消令牌可以取消，注册取消回调
                if (this.cancellationToken.CanBeCanceled)
                {
                    this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(callback, this);
                }
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">ToUniTaskObserver 实例</param>
            static void OnCanceled(object state)
            {
                var self = (ToUniTaskObserver<T>)state;
                // 释放订阅并取消任务
                self.disposable.Dispose();
                self.promise.TrySetCanceled(self.cancellationToken);
            }

            /// <summary>
            /// 当序列产生新值时调用
            /// 更新最新值，但不立即完成任务
            /// </summary>
            /// <param name="value">新值</param>
            public void OnNext(T value)
            {
                hasValue = true;
                latestValue = value;
            }

            /// <summary>
            /// 当序列发生错误时调用
            /// 设置任务异常并清理资源
            /// </summary>
            /// <param name="error">异常</param>
            public void OnError(Exception error)
            {
                try
                {
                    promise.TrySetException(error);
                }
                finally
                {
                    // 确保资源被释放
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            /// <summary>
            /// 当序列完成时调用
            /// 如果有值，使用最后一个值完成任务；否则抛出异常
            /// </summary>
            public void OnCompleted()
            {
                try
                {
                    if (hasValue)
                    {
                        // 使用最后一个值完成任务
                        promise.TrySetResult(latestValue);
                    }
                    else
                    {
                        // 序列为空，抛出异常
                        promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
                    }
                }
                finally
                {
                    // 确保资源被释放
                    registration.Dispose();
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// 将可观察序列转换为 UniTask 的观察者实现（使用第一个值）
        /// 当接收到第一个值时立即完成任务，不等待序列结束
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        class FirstValueToUniTaskObserver<T> : IObserver<T>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static readonly Action<object> callback = OnCanceled;

            /// <summary>
            /// UniTask 完成源，用于设置任务结果
            /// </summary>
            readonly UniTaskCompletionSource<T> promise;
            
            /// <summary>
            /// 单次赋值可释放对象，用于管理订阅
            /// </summary>
            readonly SingleAssignmentDisposable disposable;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            readonly CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            readonly CancellationTokenRegistration registration;

            /// <summary>
            /// 是否已接收到值
            /// </summary>
            bool hasValue;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="promise">UniTask 完成源</param>
            /// <param name="disposable">单次赋值可释放对象</param>
            /// <param name="cancellationToken">取消令牌</param>
            public FirstValueToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
            {
                this.promise = promise;
                this.disposable = disposable;
                this.cancellationToken = cancellationToken;

                // 如果取消令牌可以取消，注册取消回调
                if (this.cancellationToken.CanBeCanceled)
                {
                    this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(callback, this);
                }
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">FirstValueToUniTaskObserver 实例</param>
            static void OnCanceled(object state)
            {
                var self = (FirstValueToUniTaskObserver<T>)state;
                // 释放订阅并取消任务
                self.disposable.Dispose();
                self.promise.TrySetCanceled(self.cancellationToken);
            }

            /// <summary>
            /// 当序列产生新值时调用
            /// 如果是第一个值，立即完成任务并清理资源
            /// </summary>
            /// <param name="value">新值</param>
            public void OnNext(T value)
            {
                hasValue = true;
                try
                {
                    // 立即使用第一个值完成任务
                    promise.TrySetResult(value);
                }
                finally
                {
                    // 立即清理资源，不再等待后续值
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            /// <summary>
            /// 当序列发生错误时调用
            /// 设置任务异常并清理资源
            /// </summary>
            /// <param name="error">异常</param>
            public void OnError(Exception error)
            {
                try
                {
                    promise.TrySetException(error);
                }
                finally
                {
                    // 确保资源被释放
                    registration.Dispose();
                    disposable.Dispose();
                }
            }

            /// <summary>
            /// 当序列完成时调用
            /// 如果从未接收到值，抛出异常
            /// </summary>
            public void OnCompleted()
            {
                try
                {
                    if (!hasValue)
                    {
                        // 序列完成但从未产生值，抛出异常
                        promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
                    }
                }
                finally
                {
                    // 确保资源被释放
                    registration.Dispose();
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// 返回单个值的可观察序列实现
        /// 订阅时立即发送值并完成序列
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        class ReturnObservable<T> : IObservable<T>
        {
            /// <summary>
            /// 要返回的值
            /// </summary>
            readonly T value;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value">要返回的值</param>
            public ReturnObservable(T value)
            {
                this.value = value;
            }

            /// <summary>
            /// 订阅可观察序列
            /// 立即发送值并完成序列
            /// </summary>
            /// <param name="observer">观察者</param>
            /// <returns>空的可释放对象</returns>
            public IDisposable Subscribe(IObserver<T> observer)
            {
                // 立即发送值
                observer.OnNext(value);
                // 立即完成序列
                observer.OnCompleted();
                return EmptyDisposable.Instance;
            }
        }

        /// <summary>
        /// 抛出异常的可观察序列实现
        /// 订阅时立即发送异常
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        class ThrowObservable<T> : IObservable<T>
        {
            /// <summary>
            /// 要抛出的异常
            /// </summary>
            readonly Exception value;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value">要抛出的异常</param>
            public ThrowObservable(Exception value)
            {
                this.value = value;
            }

            /// <summary>
            /// 订阅可观察序列
            /// 立即发送异常
            /// </summary>
            /// <param name="observer">观察者</param>
            /// <returns>空的可释放对象</returns>
            public IDisposable Subscribe(IObserver<T> observer)
            {
                // 立即发送异常
                observer.OnError(value);
                return EmptyDisposable.Instance;
            }
        }
    }
}

namespace Cysharp.Threading.Tasks.Internal
{
    // ========== Rx 桥接类 ==========

    /// <summary>
    /// 空的可释放对象实现
    /// 用于表示不需要释放操作的订阅
    /// </summary>
    internal class EmptyDisposable : IDisposable
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static EmptyDisposable Instance = new EmptyDisposable();

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        EmptyDisposable()
        {

        }

        /// <summary>
        /// 释放方法（空实现）
        /// </summary>
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// 单次赋值的可释放对象
    /// 确保 Disposable 只能被设置一次，用于管理订阅的生命周期
    /// </summary>
    internal sealed class SingleAssignmentDisposable : IDisposable
    {
        /// <summary>
        /// 同步锁对象
        /// </summary>
        readonly object gate = new object();
        
        /// <summary>
        /// 当前持有的可释放对象
        /// </summary>
        IDisposable current;
        
        /// <summary>
        /// 是否已释放
        /// </summary>
        bool disposed;

        /// <summary>
        /// 获取是否已释放
        /// </summary>
        public bool IsDisposed { get { lock (gate) { return disposed; } } }

        /// <summary>
        /// 获取或设置可释放对象
        /// 只能设置一次，如果已设置或已释放则抛出异常
        /// </summary>
        public IDisposable Disposable
        {
            get
            {
                return current;
            }
            set
            {
                var old = default(IDisposable);
                bool alreadyDisposed;
                lock (gate)
                {
                    alreadyDisposed = disposed;
                    old = current;
                    if (!alreadyDisposed)
                    {
                        // 如果值为 null，直接返回
                        if (value == null) return;
                        // 设置新值
                        current = value;
                    }
                }

                // 如果已经释放，立即释放新设置的值
                if (alreadyDisposed && value != null)
                {
                    value.Dispose();
                    return;
                }

                // 如果已经设置过，抛出异常
                if (old != null) throw new InvalidOperationException("Disposable is already set");
            }
        }

        /// <summary>
        /// 释放资源
        /// 释放当前持有的可释放对象
        /// </summary>
        public void Dispose()
        {
            IDisposable old = null;

            lock (gate)
            {
                if (!disposed)
                {
                    disposed = true;
                    old = current;
                    current = null;
                }
            }

            // 在锁外释放，避免死锁
            if (old != null) old.Dispose();
        }
    }

    /// <summary>
    /// 异步主题实现
    /// 类似于 Rx.NET 的 AsyncSubject，只在完成时发送最后一个值
    /// 用于将 UniTask 转换为 IObservable
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal sealed class AsyncSubject<T> : IObservable<T>, IObserver<T>
    {
        /// <summary>
        /// 观察者列表的同步锁
        /// </summary>
        object observerLock = new object();

        /// <summary>
        /// 最后一个值
        /// </summary>
        T lastValue;
        
        /// <summary>
        /// 是否已接收到值
        /// </summary>
        bool hasValue;
        
        /// <summary>
        /// 是否已停止（完成或错误）
        /// </summary>
        bool isStopped;
        
        /// <summary>
        /// 是否已释放
        /// </summary>
        bool isDisposed;
        
        /// <summary>
        /// 最后一个错误
        /// </summary>
        Exception lastError;
        
        /// <summary>
        /// 输出观察者（可能是单个观察者、观察者列表或空观察者）
        /// </summary>
        IObserver<T> outObserver = EmptyObserver<T>.Instance;

        /// <summary>
        /// 获取值（仅在完成时可用）
        /// </summary>
        /// <exception cref="InvalidOperationException">如果尚未完成</exception>
        /// <exception cref="Exception">如果完成时发生错误</exception>
        public T Value
        {
            get
            {
                ThrowIfDisposed();
                if (!isStopped) throw new InvalidOperationException("AsyncSubject is not completed yet");
                if (lastError != null) ExceptionDispatchInfo.Capture(lastError).Throw();
                return lastValue;
            }
        }

        /// <summary>
        /// 是否有观察者
        /// </summary>
        public bool HasObservers
        {
            get
            {
                return !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;
            }
        }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get { return isStopped; } }

        /// <summary>
        /// 完成序列
        /// 如果有值，发送最后一个值并完成；否则直接完成
        /// </summary>
        public void OnCompleted()
        {
            IObserver<T> old;
            T v;
            bool hv;
            lock (observerLock)
            {
                ThrowIfDisposed();
                // 如果已经停止，直接返回
                if (isStopped) return;

                // 保存当前观察者并清空
                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                v = lastValue;
                hv = hasValue;
            }

            // 在锁外通知观察者
            if (hv)
            {
                // 有值，发送最后一个值并完成
                old.OnNext(v);
                old.OnCompleted();
            }
            else
            {
                // 无值，直接完成
                old.OnCompleted();
            }
        }

        /// <summary>
        /// 报告错误
        /// </summary>
        /// <param name="error">异常</param>
        /// <exception cref="ArgumentNullException">如果 error 为 null</exception>
        public void OnError(Exception error)
        {
            if (error == null) throw new ArgumentNullException("error");

            IObserver<T> old;
            lock (observerLock)
            {
                ThrowIfDisposed();
                // 如果已经停止，直接返回
                if (isStopped) return;

                // 保存当前观察者并清空
                old = outObserver;
                outObserver = EmptyObserver<T>.Instance;
                isStopped = true;
                lastError = error;
            }

            // 在锁外通知观察者错误
            old.OnError(error);
        }

        /// <summary>
        /// 发送下一个值
        /// 只保存值，不立即发送给观察者（等待完成时发送）
        /// </summary>
        /// <param name="value">值</param>
        public void OnNext(T value)
        {
            lock (observerLock)
            {
                ThrowIfDisposed();
                // 如果已经停止，忽略新值
                if (isStopped) return;

                // 保存值
                this.hasValue = true;
                this.lastValue = value;
            }
        }

        /// <summary>
        /// 订阅可观察序列
        /// 如果已完成，立即通知观察者；否则将观察者添加到列表
        /// </summary>
        /// <param name="observer">观察者</param>
        /// <returns>订阅对象，可用于取消订阅</returns>
        /// <exception cref="ArgumentNullException">如果 observer 为 null</exception>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);
            var v = default(T);
            var hv = false;

            lock (observerLock)
            {
                ThrowIfDisposed();
                if (!isStopped)
                {
                    // 序列尚未完成，将观察者添加到列表
                    var listObserver = outObserver as ListObserver<T>;
                    if (listObserver != null)
                    {
                        // 已经是列表，添加到列表
                        outObserver = listObserver.Add(observer);
                    }
                    else
                    {
                        var current = outObserver;
                        if (current is EmptyObserver<T>)
                        {
                            // 当前为空观察者，直接设置为新观察者
                            outObserver = observer;
                        }
                        else
                        {
                            // 当前是单个观察者，创建列表
                            outObserver = new ListObserver<T>(new ImmutableList<IObserver<T>>(new[] { current, observer }));
                        }
                    }

                    // 返回订阅对象，允许取消订阅
                    return new Subscription(this, observer);
                }

                // 序列已完成，保存状态以便在锁外通知
                ex = lastError;
                v = lastValue;
                hv = hasValue;
            }

            // 在锁外通知观察者
            if (ex != null)
            {
                // 有错误，通知错误
                observer.OnError(ex);
            }
            else if (hv)
            {
                // 有值，发送值并完成
                observer.OnNext(v);
                observer.OnCompleted();
            }
            else
            {
                // 无值，直接完成
                observer.OnCompleted();
            }

            // 已完成，返回空的可释放对象
            return EmptyDisposable.Instance;
        }

        /// <summary>
        /// 释放资源
        /// 标记为已释放并清理状态
        /// </summary>
        public void Dispose()
        {
            lock (observerLock)
            {
                isDisposed = true;
                outObserver = DisposedObserver<T>.Instance;
                lastError = null;
                lastValue = default(T);
            }
        }

        /// <summary>
        /// 如果已释放则抛出异常
        /// </summary>
        /// <exception cref="ObjectDisposedException">如果已释放</exception>
        void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }
        
        /// <summary>
        /// 订阅对象实现
        /// 用于管理单个观察者的订阅，支持取消订阅
        /// </summary>
        class Subscription : IDisposable
        {
            /// <summary>
            /// 同步锁对象
            /// </summary>
            readonly object gate = new object();
            
            /// <summary>
            /// 父级 AsyncSubject
            /// </summary>
            AsyncSubject<T> parent;
            
            /// <summary>
            /// 要取消订阅的目标观察者
            /// </summary>
            IObserver<T> unsubscribeTarget;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="parent">父级 AsyncSubject</param>
            /// <param name="unsubscribeTarget">要取消订阅的目标观察者</param>
            public Subscription(AsyncSubject<T> parent, IObserver<T> unsubscribeTarget)
            {
                this.parent = parent;
                this.unsubscribeTarget = unsubscribeTarget;
            }

            /// <summary>
            /// 取消订阅
            /// 从父级 AsyncSubject 的观察者列表中移除当前观察者
            /// </summary>
            public void Dispose()
            {
                lock (gate)
                {
                    if (parent != null)
                    {
                        lock (parent.observerLock)
                        {
                            var listObserver = parent.outObserver as ListObserver<T>;
                            if (listObserver != null)
                            {
                                // 是列表，从列表中移除
                                parent.outObserver = listObserver.Remove(unsubscribeTarget);
                            }
                            else
                            {
                                // 是单个观察者，设置为空观察者
                                parent.outObserver = EmptyObserver<T>.Instance;
                            }

                            // 清空引用
                            unsubscribeTarget = null;
                            parent = null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 观察者列表实现
    /// 用于管理多个观察者，将通知广播给所有观察者
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal class ListObserver<T> : IObserver<T>
    {
        /// <summary>
        /// 不可变观察者列表
        /// </summary>
        private readonly ImmutableList<IObserver<T>> _observers;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="observers">观察者列表</param>
        public ListObserver(ImmutableList<IObserver<T>> observers)
        {
            _observers = observers;
        }

        /// <summary>
        /// 通知所有观察者序列已完成
        /// </summary>
        public void OnCompleted()
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnCompleted();
            }
        }

        /// <summary>
        /// 通知所有观察者发生错误
        /// </summary>
        /// <param name="error">异常</param>
        public void OnError(Exception error)
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnError(error);
            }
        }

        /// <summary>
        /// 通知所有观察者新值
        /// </summary>
        /// <param name="value">值</param>
        public void OnNext(T value)
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnNext(value);
            }
        }

        /// <summary>
        /// 添加观察者
        /// 返回新的 ListObserver 实例（不可变）
        /// </summary>
        /// <param name="observer">要添加的观察者</param>
        /// <returns>新的 ListObserver 实例</returns>
        internal IObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(_observers.Add(observer));
        }

        /// <summary>
        /// 移除观察者
        /// 如果移除后只剩一个观察者，返回单个观察者；否则返回新的 ListObserver 实例
        /// </summary>
        /// <param name="observer">要移除的观察者</param>
        /// <returns>移除后的观察者（可能是单个观察者或新的 ListObserver）</returns>
        internal IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(_observers.Data, observer);
            // 如果未找到，返回自身
            if (i < 0)
                return this;

            // 如果只剩两个观察者，移除后返回另一个
            if (_observers.Data.Length == 2)
            {
                return _observers.Data[1 - i];
            }
            else
            {
                // 创建新的列表（移除指定观察者）
                return new ListObserver<T>(_observers.Remove(observer));
            }
        }
    }

    /// <summary>
    /// 空观察者实现
    /// 所有方法都是空操作，用于表示没有观察者的状态
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal class EmptyObserver<T> : IObserver<T>
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static readonly EmptyObserver<T> Instance = new EmptyObserver<T>();

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        EmptyObserver()
        {

        }

        /// <summary>
        /// 完成通知（空操作）
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// 错误通知（空操作）
        /// </summary>
        /// <param name="error">异常</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// 值通知（空操作）
        /// </summary>
        /// <param name="value">值</param>
        public void OnNext(T value)
        {
        }
    }

    /// <summary>
    /// 抛出异常的观察者实现
    /// 用于在已释放的 AsyncSubject 上调用方法时抛出异常
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal class ThrowObserver<T> : IObserver<T>
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        ThrowObserver()
        {

        }

        /// <summary>
        /// 完成通知（空操作）
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// 错误通知（重新抛出异常）
        /// </summary>
        /// <param name="error">异常</param>
        public void OnError(Exception error)
        {
            ExceptionDispatchInfo.Capture(error).Throw();
        }

        /// <summary>
        /// 值通知（空操作）
        /// </summary>
        /// <param name="value">值</param>
        public void OnNext(T value)
        {
        }
    }

    /// <summary>
    /// 已释放的观察者实现
    /// 所有方法都抛出 ObjectDisposedException
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal class DisposedObserver<T> : IObserver<T>
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        DisposedObserver()
        {

        }

        /// <summary>
        /// 完成通知（抛出异常）
        /// </summary>
        /// <exception cref="ObjectDisposedException">总是抛出</exception>
        public void OnCompleted()
        {
            throw new ObjectDisposedException("");
        }

        /// <summary>
        /// 错误通知（抛出异常）
        /// </summary>
        /// <param name="error">异常（忽略）</param>
        /// <exception cref="ObjectDisposedException">总是抛出</exception>
        public void OnError(Exception error)
        {
            throw new ObjectDisposedException("");
        }

        /// <summary>
        /// 值通知（抛出异常）
        /// </summary>
        /// <param name="value">值（忽略）</param>
        /// <exception cref="ObjectDisposedException">总是抛出</exception>
        public void OnNext(T value)
        {
            throw new ObjectDisposedException("");
        }
    }

    /// <summary>
    /// 不可变列表实现
    /// 用于存储观察者列表，所有修改操作都返回新实例
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    internal class ImmutableList<T>
    {
        /// <summary>
        /// 空列表单例
        /// </summary>
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        /// <summary>
        /// 内部数据数组
        /// </summary>
        T[] data;

        /// <summary>
        /// 获取数据数组
        /// </summary>
        public T[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// 私有构造函数，创建空列表
        /// </summary>
        ImmutableList()
        {
            data = new T[0];
        }

        /// <summary>
        /// 构造函数，使用指定的数据数组创建列表
        /// </summary>
        /// <param name="data">数据数组</param>
        public ImmutableList(T[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// 添加元素
        /// 返回包含新元素的新列表实例
        /// </summary>
        /// <param name="value">要添加的值</param>
        /// <returns>新的不可变列表</returns>
        public ImmutableList<T> Add(T value)
        {
            var newData = new T[data.Length + 1];
            Array.Copy(data, newData, data.Length);
            newData[data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        /// <summary>
        /// 移除元素
        /// 返回不包含指定元素的新列表实例
        /// </summary>
        /// <param name="value">要移除的值</param>
        /// <returns>新的不可变列表</returns>
        public ImmutableList<T> Remove(T value)
        {
            var i = IndexOf(value);
            // 如果未找到，返回自身
            if (i < 0) return this;

            var length = data.Length;
            // 如果只有一个元素，返回空列表
            if (length == 1) return Empty;

            // 创建新数组，复制除指定元素外的所有元素
            var newData = new T[length - 1];

            Array.Copy(data, 0, newData, 0, i);
            Array.Copy(data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }

        /// <summary>
        /// 查找元素的索引
        /// </summary>
        /// <param name="value">要查找的值</param>
        /// <returns>元素的索引，如果未找到返回 -1</returns>
        /// <remarks>
        /// ImmutableList 仅用于存储 IObserver，不用担心装箱问题
        /// </remarks>
        public int IndexOf(T value)
        {
            for (var i = 0; i < data.Length; ++i)
            {
                // 使用 object.Equals 进行比较（适用于 IObserver 引用比较）
                if (object.Equals(data[i], value)) return i;
            }
            return -1;
        }
    }
}

