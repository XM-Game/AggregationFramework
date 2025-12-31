using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// 只读异步响应式属性接口
    /// 类似于 Rx.NET 的 BehaviorSubject，但支持异步枚举
    /// 当值发生变化时，会通知所有订阅者
    /// </summary>
    /// <typeparam name="T">属性值的类型</typeparam>
    public interface IReadOnlyAsyncReactiveProperty<T> : IUniTaskAsyncEnumerable<T>
    {
        /// <summary>
        /// 获取当前值
        /// </summary>
        T Value { get; }
        
        /// <summary>
        /// 返回一个不包含当前值的异步序列
        /// 只会在值发生变化时产生新值，不会立即返回当前值
        /// </summary>
        /// <returns>不包含当前值的异步序列</returns>
        IUniTaskAsyncEnumerable<T> WithoutCurrent();
        
        /// <summary>
        /// 等待下一个值的变化
        /// 当属性值发生变化时，返回新的值
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>等待下一个值的 UniTask</returns>
        UniTask<T> WaitAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 异步响应式属性接口
    /// 继承自只读接口，增加了设置值的能力
    /// </summary>
    /// <typeparam name="T">属性值的类型</typeparam>
    public interface IAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>
    {
        /// <summary>
        /// 获取或设置当前值
        /// 设置值时会触发所有订阅者
        /// </summary>
        new T Value { get; set; }
    }

    /// <summary>
    /// 异步响应式属性实现类
    /// 类似于 Rx.NET 的 BehaviorSubject，维护一个当前值，并在值变化时通知订阅者
    /// 支持 Unity 序列化（Unity 2018.3+）
    /// </summary>
    /// <typeparam name="T">属性值的类型</typeparam>
    [Serializable]
    public class AsyncReactiveProperty<T> : IAsyncReactiveProperty<T>, IDisposable
    {
        /// <summary>
        /// 触发事件，用于管理订阅者列表并通知值变化
        /// </summary>
        TriggerEvent<T> triggerEvent;

#if UNITY_2018_3_OR_NEWER
        /// <summary>
        /// Unity 序列化字段，在 Unity 编辑器中可见
        /// </summary>
        [UnityEngine.SerializeField]
#endif
        /// <summary>
        /// 最新的属性值
        /// </summary>
        T latestValue;

        /// <summary>
        /// 获取或设置当前值
        /// 设置值时会立即触发所有订阅者
        /// </summary>
        public T Value
        {
            get
            {
                return latestValue;
            }
            set
            {
                // 更新最新值
                this.latestValue = value;
                // 通知所有订阅者值已变化
                triggerEvent.SetResult(value);
            }
        }

        /// <summary>
        /// 构造函数，初始化异步响应式属性
        /// </summary>
        /// <param name="value">初始值</param>
        public AsyncReactiveProperty(T value)
        {
            this.latestValue = value;
            this.triggerEvent = default;
        }

        /// <summary>
        /// 返回一个不包含当前值的异步序列
        /// 只会在值发生变化时产生新值，不会立即返回当前值
        /// </summary>
        /// <returns>不包含当前值的异步序列</returns>
        public IUniTaskAsyncEnumerable<T> WithoutCurrent()
        {
            return new WithoutCurrentEnumerable(this);
        }

        /// <summary>
        /// 获取异步枚举器
        /// 枚举时会立即返回当前值，然后等待后续的值变化
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            // publishCurrentValue = true 表示首次调用时返回当前值
            return new Enumerator(this, cancellationToken, true);
        }

        /// <summary>
        /// 释放资源，标记序列完成
        /// 调用后所有订阅者会收到完成通知
        /// </summary>
        public void Dispose()
        {
            triggerEvent.SetCompleted();
        }

        /// <summary>
        /// 隐式转换操作符
        /// 允许直接将 AsyncReactiveProperty 当作 T 类型使用
        /// </summary>
        /// <param name="value">异步响应式属性</param>
        public static implicit operator T(AsyncReactiveProperty<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// 返回当前值的字符串表示
        /// </summary>
        /// <returns>当前值的字符串</returns>
        public override string ToString()
        {
            if (isValueType) return latestValue.ToString();
            return latestValue?.ToString();
        }

        /// <summary>
        /// 等待下一个值的变化
        /// 当属性值发生变化时，返回新的值
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>等待下一个值的 UniTask</returns>
        public UniTask<T> WaitAsync(CancellationToken cancellationToken = default)
        {
            return new UniTask<T>(WaitAsyncSource.Create(this, cancellationToken, out var token), token);
        }

        /// <summary>
        /// 静态字段：指示类型 T 是否为值类型
        /// 用于优化 ToString 方法
        /// </summary>
        static bool isValueType;

        /// <summary>
        /// 静态构造函数，初始化类型信息
        /// </summary>
        static AsyncReactiveProperty()
        {
            isValueType = typeof(T).IsValueType;
        }

        /// <summary>
        /// WaitAsync 方法的内部实现类
        /// 使用对象池优化性能，实现等待下一个值变化的功能
        /// </summary>
        sealed class WaitAsyncSource : IUniTaskSource<T>, ITriggerHandler<T>, ITaskPoolNode<WaitAsyncSource>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static Action<object> cancellationCallback = CancellationCallback;

            /// <summary>
            /// 对象池，用于重用 WaitAsyncSource 实例
            /// </summary>
            static TaskPool<WaitAsyncSource> pool;
            
            /// <summary>
            /// 对象池链表的下一个节点
            /// </summary>
            WaitAsyncSource nextNode;
            
            /// <summary>
            /// 实现 ITaskPoolNode 接口，返回下一个节点的引用
            /// </summary>
            ref WaitAsyncSource ITaskPoolNode<WaitAsyncSource>.NextNode => ref nextNode;

            /// <summary>
            /// 静态构造函数，注册对象池大小获取器
            /// </summary>
            static WaitAsyncSource()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitAsyncSource), () => pool.Size);
            }

            /// <summary>
            /// 父级异步响应式属性
            /// </summary>
            AsyncReactiveProperty<T> parent;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            CancellationTokenRegistration cancellationTokenRegistration;
            
            /// <summary>
            /// UniTask 完成源核心，用于管理异步操作的状态
            /// </summary>
            UniTaskCompletionSourceCore<T> core;

            /// <summary>
            /// 私有构造函数，只能通过对象池或 Create 方法创建
            /// </summary>
            WaitAsyncSource()
            {
            }

            /// <summary>
            /// 创建 WaitAsyncSource 实例
            /// 优先从对象池获取，如果池为空则创建新实例
            /// </summary>
            /// <param name="parent">父级异步响应式属性</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="token">输出的版本令牌</param>
            /// <returns>UniTask 源</returns>
            public static IUniTaskSource<T> Create(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
            {
                // 如果已经取消，直接返回已取消的完成源
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                // 尝试从对象池获取实例
                if (!pool.TryPop(out var result))
                {
                    // 池为空，创建新实例
                    result = new WaitAsyncSource();
                }

                // 初始化实例
                result.parent = parent;
                result.cancellationToken = cancellationToken;

                // 如果取消令牌可以取消，注册取消回调
                if (cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, result);
                }

                // 将当前实例添加到父属性的订阅者列表
                result.parent.triggerEvent.Add(result);

                // 跟踪任务（仅在编辑器模式下）
                TaskTracker.TrackActiveTask(result, 3);

                // 返回版本令牌
                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// 尝试将实例返回到对象池
            /// 清理所有状态并重置实例
            /// </summary>
            /// <returns>是否成功返回到池中</returns>
            bool TryReturn()
            {
                // 移除任务跟踪
                TaskTracker.RemoveTracking(this);
                // 重置完成源
                core.Reset();
                // 释放取消令牌注册
                cancellationTokenRegistration.Dispose();
                cancellationTokenRegistration = default;
                // 从父属性的订阅者列表中移除
                parent.triggerEvent.Remove(this);
                // 清空引用
                parent = null;
                cancellationToken = default;
                // 尝试推回对象池
                return pool.TryPush(this);
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">WaitAsyncSource 实例</param>
            static void CancellationCallback(object state)
            {
                var self = (WaitAsyncSource)state;
                self.OnCanceled(self.cancellationToken);
            }

            // IUniTaskSource

            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    TryReturn();
                }
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            // ITriggerHandler

            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            public void OnCanceled(CancellationToken cancellationToken)
            {
                core.TrySetCanceled(cancellationToken);
            }

            public void OnCompleted()
            {
                // Complete as Cancel.
                core.TrySetCanceled(CancellationToken.None);
            }

            public void OnError(Exception ex)
            {
                core.TrySetException(ex);
            }

            public void OnNext(T value)
            {
                core.TrySetResult(value);
            }
        }

        /// <summary>
        /// 不包含当前值的可枚举包装类
        /// 用于实现 WithoutCurrent 方法
        /// </summary>
        sealed class WithoutCurrentEnumerable : IUniTaskAsyncEnumerable<T>
        {
            /// <summary>
            /// 父级异步响应式属性
            /// </summary>
            readonly AsyncReactiveProperty<T> parent;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="parent">父级异步响应式属性</param>
            public WithoutCurrentEnumerable(AsyncReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// 获取异步枚举器
            /// publishCurrentValue = false 表示不立即返回当前值
            /// </summary>
            /// <param name="cancellationToken">取消令牌</param>
            /// <returns>异步枚举器</returns>
            public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new Enumerator(parent, cancellationToken, false);
            }
        }

        /// <summary>
        /// 异步响应式属性的枚举器实现
        /// 负责枚举属性值的变化序列
        /// </summary>
        sealed class Enumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, ITriggerHandler<T>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static Action<object> cancellationCallback = CancellationCallback;

            /// <summary>
            /// 父级异步响应式属性
            /// </summary>
            readonly AsyncReactiveProperty<T> parent;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            readonly CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            readonly CancellationTokenRegistration cancellationTokenRegistration;
            
            /// <summary>
            /// 当前枚举的值
            /// </summary>
            T value;
            
            /// <summary>
            /// 是否已释放
            /// </summary>
            bool isDisposed;
            
            /// <summary>
            /// 是否为首次调用
            /// 如果为 true，首次调用 MoveNextAsync 时会立即返回当前值
            /// </summary>
            bool firstCall;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="parent">父级异步响应式属性</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="publishCurrentValue">是否在首次调用时发布当前值</param>
            public Enumerator(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
            {
                this.parent = parent;
                this.cancellationToken = cancellationToken;
                this.firstCall = publishCurrentValue;

                // 将当前枚举器添加到父属性的订阅者列表
                parent.triggerEvent.Add(this);
                // 跟踪任务（仅在编辑器模式下）
                TaskTracker.TrackActiveTask(this, 3);

                // 如果取消令牌可以取消，注册取消回调
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            /// <summary>
            /// 获取当前枚举的值
            /// </summary>
            public T Current => value;

            /// <summary>
            /// 触发器链表的前一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            
            /// <summary>
            /// 触发器链表的下一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            /// <summary>
            /// 移动到下一个元素
            /// 如果是首次调用且 publishCurrentValue 为 true，立即返回当前值
            /// 否则等待下一个值变化
            /// </summary>
            /// <returns>如果还有下一个元素返回 true，否则返回 false</returns>
            public UniTask<bool> MoveNextAsync()
            {
                // 首次调用时，如果 publishCurrentValue 为 true，立即返回当前值
                if (firstCall)
                {
                    firstCall = false;
                    value = parent.Value;
                    return CompletedTasks.True;
                }

                // 重置完成源，等待下一个值变化
                completionSource.Reset();
                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 释放枚举器资源
            /// 从父属性的订阅者列表中移除
            /// </summary>
            /// <returns>释放完成的 UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    // 移除任务跟踪
                    TaskTracker.RemoveTracking(this);
                    // 取消完成源
                    completionSource.TrySetCanceled(cancellationToken);
                    // 从父属性的订阅者列表中移除
                    parent.triggerEvent.Remove(this);
                }
                return default;
            }

            /// <summary>
            /// 当属性值变化时调用
            /// 更新当前值并完成 MoveNextAsync
            /// </summary>
            /// <param name="value">新值</param>
            public void OnNext(T value)
            {
                this.value = value;
                completionSource.TrySetResult(true);
            }

            /// <summary>
            /// 当操作被取消时调用
            /// </summary>
            /// <param name="cancellationToken">取消令牌</param>
            public void OnCanceled(CancellationToken cancellationToken)
            {
                DisposeAsync().Forget();
            }

            /// <summary>
            /// 当序列完成时调用
            /// </summary>
            public void OnCompleted()
            {
                completionSource.TrySetResult(false);
            }

            /// <summary>
            /// 当发生错误时调用
            /// </summary>
            /// <param name="ex">异常</param>
            public void OnError(Exception ex)
            {
                completionSource.TrySetException(ex);
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">Enumerator 实例</param>
            static void CancellationCallback(object state)
            {
                var self = (Enumerator)state;
                self.DisposeAsync().Forget();
            }
        }
    }

    /// <summary>
    /// 只读异步响应式属性实现类
    /// 从异步序列中消费值并维护当前值，类似于 AsyncReactiveProperty 但只能读取
    /// 适用于从外部数据源（如网络、文件等）创建响应式属性的场景
    /// </summary>
    /// <typeparam name="T">属性值的类型</typeparam>
    public class ReadOnlyAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>, IDisposable
    {
        /// <summary>
        /// 触发事件，用于管理订阅者列表并通知值变化
        /// </summary>
        TriggerEvent<T> triggerEvent;

        /// <summary>
        /// 最新的属性值
        /// </summary>
        T latestValue;
        
        /// <summary>
        /// 用于消费源序列的枚举器
        /// </summary>
        IUniTaskAsyncEnumerator<T> enumerator;

        /// <summary>
        /// 获取当前值（只读）
        /// </summary>
        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        /// <summary>
        /// 构造函数，从异步序列创建只读响应式属性，并指定初始值
        /// </summary>
        /// <param name="initialValue">初始值</param>
        /// <param name="source">源异步序列</param>
        /// <param name="cancellationToken">取消令牌</param>
        public ReadOnlyAsyncReactiveProperty(T initialValue, IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            latestValue = initialValue;
            // 异步消费序列，不等待完成
            ConsumeEnumerator(source, cancellationToken).Forget();
        }

        /// <summary>
        /// 构造函数，从异步序列创建只读响应式属性
        /// </summary>
        /// <param name="source">源异步序列</param>
        /// <param name="cancellationToken">取消令牌</param>
        public ReadOnlyAsyncReactiveProperty(IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            // 异步消费序列，不等待完成
            ConsumeEnumerator(source, cancellationToken).Forget();
        }

        /// <summary>
        /// 消费源异步序列
        /// 每当序列产生新值时，更新 latestValue 并通知所有订阅者
        /// </summary>
        /// <param name="source">源异步序列</param>
        /// <param name="cancellationToken">取消令牌</param>
        async UniTaskVoid ConsumeEnumerator(IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            enumerator = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                // 遍历序列，消费所有值
                while (await enumerator.MoveNextAsync())
                {
                    var value = enumerator.Current;
                    // 更新最新值
                    this.latestValue = value;
                    // 通知所有订阅者值已变化
                    triggerEvent.SetResult(value);
                }
            }
            finally
            {
                // 确保枚举器被正确释放
                await enumerator.DisposeAsync();
                enumerator = null;
            }
        }

        /// <summary>
        /// 返回一个不包含当前值的异步序列
        /// 只会在值发生变化时产生新值，不会立即返回当前值
        /// </summary>
        /// <returns>不包含当前值的异步序列</returns>
        public IUniTaskAsyncEnumerable<T> WithoutCurrent()
        {
            return new WithoutCurrentEnumerable(this);
        }

        /// <summary>
        /// 获取异步枚举器
        /// 枚举时会立即返回当前值，然后等待后续的值变化
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>异步枚举器</returns>
        public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            // publishCurrentValue = true 表示首次调用时返回当前值
            return new Enumerator(this, cancellationToken, true);
        }

        /// <summary>
        /// 释放资源
        /// 停止消费源序列并标记序列完成
        /// </summary>
        public void Dispose()
        {
            // 如果枚举器还在运行，停止它
            if (enumerator != null)
            {
                enumerator.DisposeAsync().Forget();
            }

            // 标记序列完成，通知所有订阅者
            triggerEvent.SetCompleted();
        }

        /// <summary>
        /// 隐式转换操作符
        /// 允许直接将 ReadOnlyAsyncReactiveProperty 当作 T 类型使用
        /// </summary>
        /// <param name="value">只读异步响应式属性</param>
        public static implicit operator T(ReadOnlyAsyncReactiveProperty<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// 返回当前值的字符串表示
        /// </summary>
        /// <returns>当前值的字符串</returns>
        public override string ToString()
        {
            if (isValueType) return latestValue.ToString();
            return latestValue?.ToString();
        }

        /// <summary>
        /// 等待下一个值的变化
        /// 当属性值发生变化时，返回新的值
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>等待下一个值的 UniTask</returns>
        public UniTask<T> WaitAsync(CancellationToken cancellationToken = default)
        {
            return new UniTask<T>(WaitAsyncSource.Create(this, cancellationToken, out var token), token);
        }

        /// <summary>
        /// 静态字段：指示类型 T 是否为值类型
        /// 用于优化 ToString 方法
        /// </summary>
        static bool isValueType;

        /// <summary>
        /// 静态构造函数，初始化类型信息
        /// </summary>
        static ReadOnlyAsyncReactiveProperty()
        {
            isValueType = typeof(T).IsValueType;
        }

        /// <summary>
        /// ReadOnlyAsyncReactiveProperty 的 WaitAsync 方法的内部实现类
        /// 使用对象池优化性能，实现等待下一个值变化的功能
        /// </summary>
        sealed class WaitAsyncSource : IUniTaskSource<T>, ITriggerHandler<T>, ITaskPoolNode<WaitAsyncSource>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static Action<object> cancellationCallback = CancellationCallback;

            /// <summary>
            /// 对象池，用于重用 WaitAsyncSource 实例
            /// </summary>
            static TaskPool<WaitAsyncSource> pool;
            
            /// <summary>
            /// 对象池链表的下一个节点
            /// </summary>
            WaitAsyncSource nextNode;
            
            /// <summary>
            /// 实现 ITaskPoolNode 接口，返回下一个节点的引用
            /// </summary>
            ref WaitAsyncSource ITaskPoolNode<WaitAsyncSource>.NextNode => ref nextNode;

            /// <summary>
            /// 静态构造函数，注册对象池大小获取器
            /// </summary>
            static WaitAsyncSource()
            {
                TaskPool.RegisterSizeGetter(typeof(WaitAsyncSource), () => pool.Size);
            }

            /// <summary>
            /// 父级只读异步响应式属性
            /// </summary>
            ReadOnlyAsyncReactiveProperty<T> parent;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            CancellationTokenRegistration cancellationTokenRegistration;
            
            /// <summary>
            /// UniTask 完成源核心，用于管理异步操作的状态
            /// </summary>
            UniTaskCompletionSourceCore<T> core;

            /// <summary>
            /// 私有构造函数，只能通过对象池或 Create 方法创建
            /// </summary>
            WaitAsyncSource()
            {
            }

            /// <summary>
            /// 创建 WaitAsyncSource 实例
            /// 优先从对象池获取，如果池为空则创建新实例
            /// </summary>
            /// <param name="parent">父级只读异步响应式属性</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="token">输出的版本令牌</param>
            /// <returns>UniTask 源</returns>
            public static IUniTaskSource<T> Create(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
            {
                // 如果已经取消，直接返回已取消的完成源
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
                }

                // 尝试从对象池获取实例
                if (!pool.TryPop(out var result))
                {
                    // 池为空，创建新实例
                    result = new WaitAsyncSource();
                }

                // 初始化实例
                result.parent = parent;
                result.cancellationToken = cancellationToken;

                // 如果取消令牌可以取消，注册取消回调
                if (cancellationToken.CanBeCanceled)
                {
                    result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, result);
                }

                // 将当前实例添加到父属性的订阅者列表
                result.parent.triggerEvent.Add(result);

                // 跟踪任务（仅在编辑器模式下）
                TaskTracker.TrackActiveTask(result, 3);

                // 返回版本令牌
                token = result.core.Version;
                return result;
            }

            /// <summary>
            /// 尝试将实例返回到对象池
            /// 清理所有状态并重置实例
            /// </summary>
            /// <returns>是否成功返回到池中</returns>
            bool TryReturn()
            {
                // 移除任务跟踪
                TaskTracker.RemoveTracking(this);
                // 重置完成源
                core.Reset();
                // 释放取消令牌注册
                cancellationTokenRegistration.Dispose();
                cancellationTokenRegistration = default;
                // 从父属性的订阅者列表中移除
                parent.triggerEvent.Remove(this);
                // 清空引用
                parent = null;
                cancellationToken = default;
                // 尝试推回对象池
                return pool.TryPush(this);
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">WaitAsyncSource 实例</param>
            static void CancellationCallback(object state)
            {
                var self = (WaitAsyncSource)state;
                self.OnCanceled(self.cancellationToken);
            }

            // ========== IUniTaskSource 接口实现 ==========

            /// <summary>
            /// 获取异步操作的结果
            /// 完成后会将实例返回到对象池
            /// </summary>
            /// <param name="token">版本令牌</param>
            /// <returns>结果值</returns>
            public T GetResult(short token)
            {
                try
                {
                    return core.GetResult(token);
                }
                finally
                {
                    // 无论成功还是失败，都尝试返回到对象池
                    TryReturn();
                }
            }

            /// <summary>
            /// IUniTaskSource 接口的 GetResult 实现
            /// </summary>
            /// <param name="token">版本令牌</param>
            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }

            /// <summary>
            /// 设置完成时的回调
            /// </summary>
            /// <param name="continuation">延续回调</param>
            /// <param name="state">状态对象</param>
            /// <param name="token">版本令牌</param>
            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            /// <summary>
            /// 获取异步操作的状态
            /// </summary>
            /// <param name="token">版本令牌</param>
            /// <returns>任务状态</returns>
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            /// <summary>
            /// 不安全地获取异步操作的状态（不检查版本令牌）
            /// </summary>
            /// <returns>任务状态</returns>
            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            // ========== ITriggerHandler 接口实现 ==========

            /// <summary>
            /// 触发器链表的前一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            
            /// <summary>
            /// 触发器链表的下一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            /// <summary>
            /// 当操作被取消时调用
            /// </summary>
            /// <param name="cancellationToken">取消令牌</param>
            public void OnCanceled(CancellationToken cancellationToken)
            {
                core.TrySetCanceled(cancellationToken);
            }

            /// <summary>
            /// 当序列完成时调用
            /// 完成时作为取消处理
            /// </summary>
            public void OnCompleted()
            {
                // 完成时作为取消处理
                core.TrySetCanceled(CancellationToken.None);
            }

            /// <summary>
            /// 当发生错误时调用
            /// </summary>
            /// <param name="ex">异常</param>
            public void OnError(Exception ex)
            {
                core.TrySetException(ex);
            }

            /// <summary>
            /// 当下一个值到达时调用
            /// 这是 WaitAsync 的核心：当属性值变化时，完成等待并返回新值
            /// </summary>
            /// <param name="value">新值</param>
            public void OnNext(T value)
            {
                core.TrySetResult(value);
            }
        }

        /// <summary>
        /// 不包含当前值的可枚举包装类（用于 ReadOnlyAsyncReactiveProperty）
        /// 用于实现 WithoutCurrent 方法
        /// </summary>
        sealed class WithoutCurrentEnumerable : IUniTaskAsyncEnumerable<T>
        {
            /// <summary>
            /// 父级只读异步响应式属性
            /// </summary>
            readonly ReadOnlyAsyncReactiveProperty<T> parent;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="parent">父级只读异步响应式属性</param>
            public WithoutCurrentEnumerable(ReadOnlyAsyncReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// 获取异步枚举器
            /// publishCurrentValue = false 表示不立即返回当前值
            /// </summary>
            /// <param name="cancellationToken">取消令牌</param>
            /// <returns>异步枚举器</returns>
            public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new Enumerator(parent, cancellationToken, false);
            }
        }

        /// <summary>
        /// 只读异步响应式属性的枚举器实现
        /// 负责枚举属性值的变化序列
        /// </summary>
        sealed class Enumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, ITriggerHandler<T>
        {
            /// <summary>
            /// 取消回调的静态委托，避免每次创建新委托
            /// </summary>
            static Action<object> cancellationCallback = CancellationCallback;

            /// <summary>
            /// 父级只读异步响应式属性
            /// </summary>
            readonly ReadOnlyAsyncReactiveProperty<T> parent;
            
            /// <summary>
            /// 取消令牌
            /// </summary>
            readonly CancellationToken cancellationToken;
            
            /// <summary>
            /// 取消令牌注册，用于取消时清理资源
            /// </summary>
            readonly CancellationTokenRegistration cancellationTokenRegistration;
            
            /// <summary>
            /// 当前枚举的值
            /// </summary>
            T value;
            
            /// <summary>
            /// 是否已释放
            /// </summary>
            bool isDisposed;
            
            /// <summary>
            /// 是否为首次调用
            /// 如果为 true，首次调用 MoveNextAsync 时会立即返回当前值
            /// </summary>
            bool firstCall;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="parent">父级只读异步响应式属性</param>
            /// <param name="cancellationToken">取消令牌</param>
            /// <param name="publishCurrentValue">是否在首次调用时发布当前值</param>
            public Enumerator(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
            {
                this.parent = parent;
                this.cancellationToken = cancellationToken;
                this.firstCall = publishCurrentValue;

                // 将当前枚举器添加到父属性的订阅者列表
                parent.triggerEvent.Add(this);
                // 跟踪任务（仅在编辑器模式下）
                TaskTracker.TrackActiveTask(this, 3);

                // 如果取消令牌可以取消，注册取消回调
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            /// <summary>
            /// 获取当前枚举的值
            /// </summary>
            public T Current => value;
            
            /// <summary>
            /// 触发器链表的前一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Prev { get; set; }
            
            /// <summary>
            /// 触发器链表的下一个节点
            /// </summary>
            ITriggerHandler<T> ITriggerHandler<T>.Next { get; set; }

            /// <summary>
            /// 移动到下一个元素
            /// 如果是首次调用且 publishCurrentValue 为 true，立即返回当前值
            /// 否则等待下一个值变化
            /// </summary>
            /// <returns>如果还有下一个元素返回 true，否则返回 false</returns>
            public UniTask<bool> MoveNextAsync()
            {
                // 首次调用时，如果 publishCurrentValue 为 true，立即返回当前值
                if (firstCall)
                {
                    firstCall = false;
                    value = parent.Value;
                    return CompletedTasks.True;
                }

                // 重置完成源，等待下一个值变化
                completionSource.Reset();
                return new UniTask<bool>(this, completionSource.Version);
            }

            /// <summary>
            /// 释放枚举器资源
            /// 从父属性的订阅者列表中移除
            /// </summary>
            /// <returns>释放完成的 UniTask</returns>
            public UniTask DisposeAsync()
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    // 移除任务跟踪
                    TaskTracker.RemoveTracking(this);
                    // 取消完成源
                    completionSource.TrySetCanceled(cancellationToken);
                    // 从父属性的订阅者列表中移除
                    parent.triggerEvent.Remove(this);
                }
                return default;
            }

            /// <summary>
            /// 当属性值变化时调用
            /// 更新当前值并完成 MoveNextAsync
            /// </summary>
            /// <param name="value">新值</param>
            public void OnNext(T value)
            {
                this.value = value;
                completionSource.TrySetResult(true);
            }

            /// <summary>
            /// 当操作被取消时调用
            /// </summary>
            /// <param name="cancellationToken">取消令牌</param>
            public void OnCanceled(CancellationToken cancellationToken)
            {
                DisposeAsync().Forget();
            }

            /// <summary>
            /// 当序列完成时调用
            /// </summary>
            public void OnCompleted()
            {
                completionSource.TrySetResult(false);
            }

            /// <summary>
            /// 当发生错误时调用
            /// </summary>
            /// <param name="ex">异常</param>
            public void OnError(Exception ex)
            {
                completionSource.TrySetException(ex);
            }

            /// <summary>
            /// 取消回调的静态方法
            /// 当取消令牌被触发时调用
            /// </summary>
            /// <param name="state">Enumerator 实例</param>
            static void CancellationCallback(object state)
            {
                var self = (Enumerator)state;
                self.DisposeAsync().Forget();
            }
        }
    }

    /// <summary>
    /// 状态扩展方法类
    /// 提供将异步序列转换为只读异步响应式属性的扩展方法
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        /// 将异步序列转换为只读异步响应式属性
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="source">源异步序列</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>只读异步响应式属性</returns>
        public static ReadOnlyAsyncReactiveProperty<T> ToReadOnlyAsyncReactiveProperty<T>(this IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            return new ReadOnlyAsyncReactiveProperty<T>(source, cancellationToken);
        }

        /// <summary>
        /// 将异步序列转换为只读异步响应式属性，并指定初始值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="source">源异步序列</param>
        /// <param name="initialValue">初始值</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>只读异步响应式属性</returns>
        public static ReadOnlyAsyncReactiveProperty<T> ToReadOnlyAsyncReactiveProperty<T>(this IUniTaskAsyncEnumerable<T> source, T initialValue, CancellationToken cancellationToken)
        {
            return new ReadOnlyAsyncReactiveProperty<T>(initialValue, source, cancellationToken);
        }
    }
}