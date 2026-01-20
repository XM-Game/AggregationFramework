// ==========================================================
// 文件名：ObjectPoolBaseRefactored.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 重构后的泛型对象池抽象基类（扁平化继承结构）
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 重构后的泛型对象池抽象基类
    /// Refactored Generic Object Pool Abstract Base Class
    /// 
    /// <para>扁平化继承结构，使用组合模式集成功能组件</para>
    /// <para>Flattened inheritance structure, integrating functional components using composition pattern</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 改进点：
    /// 1. 合并非泛型和泛型基类，减少继承层次
    /// 2. 使用组合模式集成统计、预热等功能
    /// 3. 明确抽象层次，简化子类实现
    /// 
    /// 设计模式：
    /// - 模板方法模式：定义算法骨架
    /// - 组合模式：功能模块化
    /// - 策略模式：可插拔策略
    /// </remarks>
    public abstract class ObjectPoolBaseRefactored<T> : IObjectPool<T>, IDisposable
    {
        #region Fields

        /// <summary>
        /// 池状态
        /// Pool state
        /// </summary>
        protected PoolState _state;

        /// <summary>
        /// 池名称
        /// Pool name
        /// </summary>
        protected string _name;

        /// <summary>
        /// 是否已销毁
        /// Whether the pool has been disposed
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// 线程安全锁
        /// Thread-safe lock
        /// </summary>
        protected readonly object _lock = new object();

        /// <summary>
        /// 创建策略
        /// Creation policy
        /// </summary>
        protected readonly IPoolCreationPolicy<T> _creationPolicy;

        /// <summary>
        /// 清理策略
        /// Cleanup policy
        /// </summary>
        protected readonly IPoolCleanupPolicy<T> _cleanupPolicy;

        /// <summary>
        /// 功能组件集合
        /// Functional components collection
        /// </summary>
        protected readonly Dictionary<string, IPoolComponent> _components;

        /// <summary>
        /// 统计组件
        /// Statistics component
        /// </summary>
        protected PoolStatisticsComponent _statisticsComponent;

        /// <summary>
        /// 预热组件
        /// Warmup component
        /// </summary>
        protected PoolWarmupComponent<T> _warmupComponent;

        #endregion

        #region Properties

        /// <inheritdoc />
        public Type ObjectType => typeof(T);

        /// <inheritdoc />
        public string Name
        {
            get => _name;
            protected set => _name = value;
        }

        /// <inheritdoc />
        public PoolState State
        {
            get => _state;
            protected set => _state = value;
        }

        /// <inheritdoc />
        public abstract int AvailableCount { get; }

        /// <inheritdoc />
        public abstract int ActiveCount { get; }

        /// <inheritdoc />
        public abstract int TotalCount { get; }

        /// <summary>
        /// 是否已销毁
        /// Whether the pool has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        /// <summary>
        /// 创建策略
        /// Creation policy
        /// </summary>
        public IPoolCreationPolicy<T> CreationPolicy => _creationPolicy;

        /// <summary>
        /// 清理策略
        /// Cleanup policy
        /// </summary>
        public IPoolCleanupPolicy<T> CleanupPolicy => _cleanupPolicy;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="cleanupPolicy">清理策略 / Cleanup policy</param>
        /// <param name="capacityPolicy">容量策略 / Capacity policy</param>
        /// <param name="name">池名称 / Pool name</param>
        protected ObjectPoolBaseRefactored(
            IPoolCreationPolicy<T> creationPolicy,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            IPoolCapacityPolicy capacityPolicy = null,
            string name = null)
        {
            _creationPolicy = creationPolicy ?? throw new ArgumentNullException(nameof(creationPolicy));
            _cleanupPolicy = cleanupPolicy;
            _name = name ?? GetType().Name;
            _state = PoolState.Uninitialized;
            _disposed = false;
            _components = new Dictionary<string, IPoolComponent>();

            // 初始化统计组件
            // Initialize statistics component
            _statisticsComponent = new PoolStatisticsComponent();
            AddComponent(_statisticsComponent);

            // 初始化预热组件（如果有容量策略）
            // Initialize warmup component (if capacity policy exists)
            if (capacityPolicy != null)
            {
                _warmupComponent = new PoolWarmupComponent<T>(
                    creationPolicy,
                    capacityPolicy,
                    obj => AddObjectToPool(obj)
                );
                AddComponent(_warmupComponent);
            }
        }

        #endregion

        #region Component Management

        /// <summary>
        /// 添加组件
        /// Add component
        /// </summary>
        /// <param name="component">组件 / Component</param>
        protected void AddComponent(IPoolComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            _components[component.Name] = component;
        }

        /// <summary>
        /// 移除组件
        /// Remove component
        /// </summary>
        /// <param name="componentName">组件名称 / Component name</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        protected bool RemoveComponent(string componentName)
        {
            if (_components.TryGetValue(componentName, out var component))
            {
                component.Dispose();
                return _components.Remove(componentName);
            }
            return false;
        }

        /// <summary>
        /// 获取组件
        /// Get component
        /// </summary>
        /// <typeparam name="TComponent">组件类型 / Component type</typeparam>
        /// <param name="componentName">组件名称 / Component name</param>
        /// <returns>组件实例 / Component instance</returns>
        protected TComponent GetComponent<TComponent>(string componentName) where TComponent : IPoolComponent
        {
            if (_components.TryGetValue(componentName, out var component))
            {
                return (TComponent)component;
            }
            return default;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// 初始化对象池
        /// Initialize the object pool
        /// </summary>
        public void Initialize()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (_state != PoolState.Uninitialized)
                {
                    throw new InvalidOperationException($"Pool '{_name}' is already initialized.");
                }

                try
                {
                    // 初始化所有组件
                    // Initialize all components
                    foreach (var component in _components.Values)
                    {
                        component.Initialize();
                    }

                    // 调用子类初始化钩子
                    // Invoke subclass initialization hook
                    OnInitialize();

                    _state = PoolState.Active;
                }
                catch (Exception ex)
                {
                    _state = PoolState.Disposed;
                    throw new PoolException($"Failed to initialize pool '{_name}'.", ex);
                }
            }
        }

        /// <summary>
        /// 初始化钩子方法（子类重写）
        /// Initialization hook method (override in subclasses)
        /// </summary>
        protected virtual void OnInitialize()
        {
            // 子类可重写此方法执行自定义初始化逻辑
            // Subclasses can override this method to perform custom initialization logic
        }

        /// <summary>
        /// 清空对象池
        /// Clear the object pool
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (_state != PoolState.Active)
                {
                    throw new InvalidOperationException($"Pool '{_name}' is not active.");
                }

                OnClear();
            }
        }

        /// <summary>
        /// 清空钩子方法（子类必须实现）
        /// Clear hook method (must be implemented by subclasses)
        /// </summary>
        protected abstract void OnClear();

        #endregion

        #region Get/Return Methods

        /// <inheritdoc />
        public T Get()
        {
            ThrowIfNotActive();

            lock (_lock)
            {
                T obj = default;

                try
                {
                    // 尝试从池中获取对象
                    // Try to get object from pool
                    if (TryGetFromPool(out obj))
                    {
                        _statisticsComponent?.RecordHit();
                    }
                    else
                    {
                        // 池中无可用对象，创建新对象
                        // No available object in pool, create new one
                        obj = CreateObject();
                        _statisticsComponent?.RecordMiss();
                        _statisticsComponent?.RecordCreation();
                    }

                    // 调用对象获取回调
                    // Invoke object get callback
                    OnObjectGet(obj);

                    _statisticsComponent?.RecordActivation();
                    _statisticsComponent?.SetIdleCount(AvailableCount);

                    return obj;
                }
                catch (Exception ex)
                {
                    throw new PoolCreationException($"Failed to get object from pool '{_name}'.", ex);
                }
            }
        }

        /// <inheritdoc />
        public bool Return(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            ThrowIfNotActive();

            lock (_lock)
            {
                try
                {
                    // 调用对象归还回调
                    // Invoke object return callback
                    OnObjectReturn(obj);

                    // 尝试将对象归还到池中
                    // Try to return object to pool
                    bool canReturn = _warmupComponent?.CanReturn(AvailableCount, TotalCount) ?? true;

                    if (canReturn && TryReturnToPool(obj))
                    {
                        _statisticsComponent?.RecordReturn();
                        _statisticsComponent?.SetIdleCount(AvailableCount);
                        return true;
                    }
                    else
                    {
                        // 池已满或不接受归还，销毁对象
                        // Pool is full or doesn't accept return, destroy object
                        DestroyObject(obj);
                        _statisticsComponent?.RecordDestruction();
                        _statisticsComponent?.RecordReturn();
                        _statisticsComponent?.SetIdleCount(AvailableCount);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new PoolReturnException($"Failed to return object to pool '{_name}'.", ex);
                }
            }
        }

        /// <inheritdoc />
        object IObjectPool.Get()
        {
            return Get();
        }

        /// <inheritdoc />
        bool IObjectPool.Return(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is T typedObj)
            {
                return Return(typedObj);
            }
            else
            {
                throw new ArgumentException($"Object type mismatch. Expected {typeof(T).Name}, got {obj.GetType().Name}.", nameof(obj));
            }
        }

        #endregion

        #region Batch Operations

        /// <inheritdoc />
        public virtual T[] GetMany(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            }

            T[] objects = new T[count];
            for (int i = 0; i < count; i++)
            {
                objects[i] = Get();
            }
            return objects;
        }

        /// <inheritdoc />
        public virtual int ReturnMany(T[] objects)
        {
            if (objects == null)
            {
                throw new ArgumentNullException(nameof(objects));
            }

            int returnedCount = 0;
            foreach (var obj in objects)
            {
                if (obj != null && Return(obj))
                {
                    returnedCount++;
                }
            }
            return returnedCount;
        }

        #endregion

        #region Advanced Operations

        /// <inheritdoc />
        public virtual bool TryGet(out T obj)
        {
            try
            {
                obj = Get();
                return true;
            }
            catch
            {
                obj = default;
                return false;
            }
        }

        /// <inheritdoc />
        public virtual PooledObjectRental<T> Rent()
        {
            T obj = Get();
            return new PooledObjectRental<T>(this, obj);
        }

        /// <inheritdoc />
        public abstract void Warmup(int count);

        /// <inheritdoc />
        public abstract int Shrink(int targetCount);

        #endregion

        #region Abstract Methods

        /// <summary>
        /// 尝试从池中获取对象
        /// Try to get object from pool
        /// </summary>
        /// <param name="obj">获取的对象 / Retrieved object</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        protected abstract bool TryGetFromPool(out T obj);

        /// <summary>
        /// 尝试将对象归还到池中
        /// Try to return object to pool
        /// </summary>
        /// <param name="obj">要归还的对象 / Object to return</param>
        /// <returns>是否成功归还 / Whether successfully returned</returns>
        protected abstract bool TryReturnToPool(T obj);

        /// <summary>
        /// 添加对象到池中（用于预热）
        /// Add object to pool (for warmup)
        /// </summary>
        /// <param name="obj">要添加的对象 / Object to add</param>
        protected abstract void AddObjectToPool(T obj);

        #endregion

        #region Object Lifecycle Methods

        /// <summary>
        /// 创建对象
        /// Create object
        /// </summary>
        /// <returns>创建的对象 / Created object</returns>
        protected virtual T CreateObject()
        {
            if (_creationPolicy == null)
            {
                throw new InvalidOperationException($"Creation policy is not set for pool '{_name}'.");
            }

            T obj = _creationPolicy.Create();

            if (obj == null)
            {
                throw new PoolCreationException($"Creation policy returned null for pool '{_name}'.");
            }

            return obj;
        }

        /// <summary>
        /// 销毁对象
        /// Destroy object
        /// </summary>
        /// <param name="obj">要销毁的对象 / Object to destroy</param>
        protected virtual void DestroyObject(T obj)
        {
            if (obj == null)
            {
                return;
            }

            // 如果对象实现了 IPooledObject，调用 OnDestroy
            // If object implements IPooledObject, call OnDestroy
            if (obj is IPooledObject pooledObject)
            {
                pooledObject.OnDestroy();
            }

            // 调用清理策略的销毁方法
            // Invoke cleanup policy's destroy method
            _cleanupPolicy?.OnDestroy(obj);

            // 如果对象实现了 IDisposable，调用 Dispose
            // If object implements IDisposable, call Dispose
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 对象获取回调
        /// Object get callback
        /// </summary>
        /// <param name="obj">获取的对象 / Retrieved object</param>
        protected virtual void OnObjectGet(T obj)
        {
            // 如果对象实现了 IPooledObject，调用 OnGet
            // If object implements IPooledObject, call OnGet
            if (obj is IPooledObject pooledObject)
            {
                pooledObject.OnGet();
            }
        }

        /// <summary>
        /// 对象归还回调
        /// Object return callback
        /// </summary>
        /// <param name="obj">归还的对象 / Returned object</param>
        protected virtual void OnObjectReturn(T obj)
        {
            // 如果对象实现了 IPooledObject，调用 OnReturn
            // If object implements IPooledObject, call OnReturn
            if (obj is IPooledObject pooledObject)
            {
                pooledObject.OnReturn();
            }

            // 调用清理策略的归还方法
            // Invoke cleanup policy's return method
            _cleanupPolicy?.OnReturn(obj);
        }

        #endregion

        #region Statistics Methods

        /// <summary>
        /// 获取统计信息
        /// Get statistics
        /// </summary>
        /// <returns>统计信息 / Statistics</returns>
        public virtual IPoolStatistics GetStatistics()
        {
            lock (_lock)
            {
                if (_statisticsComponent == null)
                    return null;

                var snapshot = _statisticsComponent.CreateSnapshot();
                return new PoolStatisticsSnapshotAdapter(snapshot);
            }
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// 快照适配器，将快照转换为统计接口
        /// Snapshot Adapter, converts snapshot to statistics interface
        /// </summary>
        private class PoolStatisticsSnapshotAdapter : IPoolStatistics
        {
            private readonly PoolStatisticsSnapshot _snapshot;

            public PoolStatisticsSnapshotAdapter(PoolStatisticsSnapshot snapshot)
            {
                _snapshot = snapshot;
            }

            public long TotalCreated => _snapshot.TotalCreated;
            public long TotalDestroyed => _snapshot.TotalDestroyed;
            public int CurrentActive => _snapshot.CurrentActive;
            public int CurrentIdle => _snapshot.CurrentIdle;
            public int CurrentTotal => _snapshot.CurrentTotal;
            public long TotalGets => _snapshot.TotalGets;
            public long TotalReturns => _snapshot.TotalReturns;
            public long Hits => _snapshot.Hits;
            public long Misses => _snapshot.Misses;
            public double HitRate => _snapshot.HitRate;
            public double MissRate => _snapshot.MissRate;
            public double AverageGetTime => _snapshot.AverageGetTime;
            public double AverageReturnTime => _snapshot.AverageReturnTime;
            public int PeakActive => _snapshot.PeakActive;
            public int PeakTotal => _snapshot.PeakTotal;
            public long EstimatedMemoryUsage => _snapshot.EstimatedMemoryUsage;
            public DateTime CreatedTime => _snapshot.CreatedTime;
            public TimeSpan Uptime => _snapshot.Uptime;
            public DateTime? LastGetTime => null;
            public DateTime? LastReturnTime => null;

            public PoolStatisticsSnapshot CreateSnapshot() => _snapshot;
            public void Reset() { /* 快照不支持重置 */ }
        }

        #endregion
        #region Validation Methods

        /// <summary>
        /// 检查池是否已销毁，如果已销毁则抛出异常
        /// Check if the pool is disposed, throw exception if disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new PoolDisposedException($"Pool '{_name}' has been disposed.");
            }
        }

        /// <summary>
        /// 检查池是否处于活跃状态，如果不是则抛出异常
        /// Check if the pool is active, throw exception if not active
        /// </summary>
        protected void ThrowIfNotActive()
        {
            ThrowIfDisposed();

            if (_state != PoolState.Active)
            {
                throw new InvalidOperationException($"Pool '{_name}' is not active. Current state: {_state}");
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// 销毁对象池
        /// Dispose the object pool
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 销毁对象池（可重写）
        /// Dispose the object pool (overridable)
        /// </summary>
        /// <param name="disposing">是否正在销毁托管资源 / Whether disposing managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                lock (_lock)
                {
                    try
                    {
                        // 清空池中所有对象
                        // Clear all objects in pool
                        OnClear();

                        // 释放所有组件
                        // Release all components
                        foreach (var component in _components.Values)
                        {
                            component.Dispose();
                        }
                        _components.Clear();

                        // 释放策略资源
                        // Release policy resources
                        if (_creationPolicy is IDisposable creationDisposable)
                        {
                            creationDisposable.Dispose();
                        }

                        if (_cleanupPolicy is IDisposable cleanupDisposable)
                        {
                            cleanupDisposable.Dispose();
                        }

                        // 调用子类销毁钩子
                        // Invoke subclass dispose hook
                        OnDispose();
                    }
                    finally
                    {
                        _state = PoolState.Disposed;
                        _disposed = true;
                    }
                }
            }
        }

        /// <summary>
        /// 销毁钩子方法（子类重写）
        /// Dispose hook method (override in subclasses)
        /// </summary>
        protected virtual void OnDispose()
        {
            // 子类可重写此方法执行自定义清理逻辑
            // Subclasses can override this method to perform custom cleanup logic
        }

        /// <summary>
        /// 析构函数
        /// Finalizer
        /// </summary>
        ~ObjectPoolBaseRefactored()
        {
            Dispose(false);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 获取池的字符串表示
        /// Get the string representation of the pool
        /// </summary>
        public override string ToString()
        {
            return $"[{GetType().Name}] Name={_name}, State={_state}, Total={TotalCount}, Active={ActiveCount}, Available={AvailableCount}";
        }

        #endregion
    }
}
