// ==========================================================
// 文件名：ObjectPoolBaseT.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 泛型对象池抽象基类，提供类型安全的池操作模板方法
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 泛型对象池抽象基类
    /// Generic Object Pool Abstract Base Class
    /// 
    /// <para>提供类型安全的对象池基础实现框架</para>
    /// <para>Provides type-safe basic implementation framework for object pools</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：模板方法模式 (Template Method Pattern)
    /// - 定义泛型池操作骨架
    /// - 集成策略模式（创建/清理/容量策略）
    /// - 支持生命周期回调（OnGet/OnReturn/OnDestroy/OnReset）
    /// - 支持对象状态验证和追踪
    /// - 支持容量预测和自适应调整
    /// - 支持错误恢复和重试机制
    /// </remarks>
    public abstract class ObjectPoolBase<T> : ObjectPoolBase, IObjectPool<T>
    {
        #region Fields

        /// <summary>
        /// 创建策略
        /// Creation policy
        /// </summary>
        protected IPoolCreationPolicy<T> _creationPolicy;

        /// <summary>
        /// 清理策略
        /// Cleanup policy
        /// </summary>
        protected IPoolCleanupPolicy<T> _cleanupPolicy;

        /// <summary>
        /// 容量策略
        /// Capacity policy
        /// </summary>
        protected IPoolCapacityPolicy _capacityPolicy;

        /// <summary>
        /// 统计信息
        /// Statistics
        /// </summary>
        protected PoolStatisticsData _statistics;

        /// <summary>
        /// 创建重试次数
        /// Creation retry count
        /// </summary>
        protected int _creationRetryCount = 3;

        /// <summary>
        /// 归还重试次数
        /// Return retry count
        /// </summary>
        protected int _returnRetryCount = 2;

        /// <summary>
        /// 容量调整间隔（秒）
        /// Capacity adjustment interval (seconds)
        /// </summary>
        protected float _capacityAdjustmentInterval = 60f;

        /// <summary>
        /// 上次容量调整时间
        /// Last capacity adjustment time
        /// </summary>
        protected DateTime _lastCapacityAdjustment;

        /// <summary>
        /// 对象状态追踪字典
        /// Object state tracking dictionary
        /// </summary>
        protected readonly Dictionary<T, ObjectState> _objectStates = new Dictionary<T, ObjectState>();

        #endregion

        #region Properties

        /// <summary>
        /// 创建策略
        /// Creation policy
        /// </summary>
        public IPoolCreationPolicy<T> CreationPolicy
        {
            get => _creationPolicy;
            protected set => _creationPolicy = value;
        }

        /// <summary>
        /// 清理策略
        /// Cleanup policy
        /// </summary>
        public IPoolCleanupPolicy<T> CleanupPolicy
        {
            get => _cleanupPolicy;
            protected set => _cleanupPolicy = value;
        }

        /// <summary>
        /// 容量策略
        /// Capacity policy
        /// </summary>
        public IPoolCapacityPolicy CapacityPolicy
        {
            get => _capacityPolicy;
            protected set => _capacityPolicy = value;
        }

        /// <summary>
        /// 创建重试次数
        /// Creation retry count
        /// </summary>
        public int CreationRetryCount
        {
            get => _creationRetryCount;
            set => _creationRetryCount = Math.Max(0, value);
        }

        /// <summary>
        /// 归还重试次数
        /// Return retry count
        /// </summary>
        public int ReturnRetryCount
        {
            get => _returnRetryCount;
            set => _returnRetryCount = Math.Max(0, value);
        }

        /// <summary>
        /// 容量调整间隔（秒）
        /// Capacity adjustment interval (seconds)
        /// </summary>
        public float CapacityAdjustmentInterval
        {
            get => _capacityAdjustmentInterval;
            set => _capacityAdjustmentInterval = Math.Max(1f, value);
        }

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
        protected ObjectPoolBase(
            IPoolCreationPolicy<T> creationPolicy,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            IPoolCapacityPolicy capacityPolicy = null,
            string name = null)
            : base(name)
        {
            _creationPolicy = creationPolicy ?? throw new ArgumentNullException(nameof(creationPolicy));
            _cleanupPolicy = cleanupPolicy;
            _capacityPolicy = capacityPolicy;
            _statistics = new PoolStatisticsData();
            _lastCapacityAdjustment = DateTime.UtcNow;
        }

        #endregion

        #region Generic Get/Return Methods

        /// <summary>
        /// 从池中获取对象（泛型版本）
        /// Get object from pool (generic version)
        /// </summary>
        /// <returns>池化对象 / Pooled object</returns>
        T IObjectPool<T>.Get()
        {
            ThrowIfNotActive();

            lock (_lock)
            {
                T obj = default;
                Exception lastException = null;
                int retryCount = 0;

                // 带重试机制的对象获取
                // Object acquisition with retry mechanism
                while (retryCount <= _creationRetryCount)
                {
                    try
                    {
                        // 尝试从池中获取对象
                        // Try to get object from pool
                        if (TryGetFromPool(out obj))
                        {
                            _statistics.IncrementHitCount();
                        }
                        else
                        {
                            // 池中无可用对象，创建新对象
                            // No available object in pool, create new one
                            obj = CreateObjectWithRetry();
                            _statistics.IncrementMissCount();
                            _statistics.IncrementTotalCreated();
                        }

                        // 验证对象状态
                        // Validate object state
                        if (!ValidateObject(obj))
                        {
                            throw new PoolCreationException($"Object validation failed for pool '{_name}'.");
                        }

                        // 调用对象获取回调
                        // Invoke object get callback
                        OnObjectGet(obj);

                        // 更新对象状态
                        // Update object state
                        UpdateObjectState(obj, ObjectState.Active);

                        _statistics.IncrementActiveCount();

                        // 检查是否需要调整容量
                        // Check if capacity adjustment is needed
                        CheckAndAdjustCapacity();

                        return obj;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        retryCount++;

                        if (retryCount <= _creationRetryCount)
                        {
                            // 记录重试日志
                            // Log retry attempt
                            OnRetryAttempt(retryCount, _creationRetryCount, ex);
                        }
                    }
                }

                // 所有重试都失败，抛出异常
                // All retries failed, throw exception
                throw new PoolCreationException(
                    $"Failed to get object from pool '{_name}' after {_creationRetryCount} retries.",
                    lastException);
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
                Exception lastException = null;
                int retryCount = 0;

                // 带重试机制的对象归还
                // Object return with retry mechanism
                while (retryCount <= _returnRetryCount)
                {
                    try
                    {
                        // 重置对象状态
                        // Reset object state
                        OnObjectReset(obj);

                        // 调用对象归还回调
                        // Invoke object return callback
                        OnObjectReturn(obj);

                        // 尝试将对象归还到池中
                        // Try to return object to pool
                        if (TryReturnToPool(obj))
                        {
                            // 更新对象状态
                            // Update object state
                            UpdateObjectState(obj, ObjectState.Idle);

                            _statistics.DecrementActiveCount();
                            return true;
                        }
                        else
                        {
                            // 池已满或不接受归还，销毁对象
                            // Pool is full or doesn't accept return, destroy object
                            DestroyObject(obj);
                            RemoveObjectState(obj);
                            _statistics.IncrementTotalDestroyed();
                            _statistics.DecrementActiveCount();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        retryCount++;

                        if (retryCount <= _returnRetryCount)
                        {
                            // 记录重试日志
                            // Log retry attempt
                            OnRetryAttempt(retryCount, _returnRetryCount, ex);
                        }
                        else
                        {
                            // 归还失败，销毁对象
                            // Return failed, destroy object
                            try
                            {
                                DestroyObject(obj);
                                RemoveObjectState(obj);
                                _statistics.IncrementTotalDestroyed();
                                _statistics.DecrementActiveCount();
                            }
                            catch
                            {
                                // 忽略销毁异常
                                // Ignore destroy exception
                            }

                            throw new PoolReturnException(
                                $"Failed to return object to pool '{_name}' after {_returnRetryCount} retries.",
                                lastException);
                        }
                    }
                }

                return false;
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
                objects[i] = ((IObjectPool<T>)this).Get();
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
                obj = ((IObjectPool<T>)this).Get();
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
            T obj = ((IObjectPool<T>)this).Get();
            return new PooledObjectRental<T>(this, obj);
        }

        #endregion

        #region Non-Generic Get/Return Methods (IObjectPool Implementation)

        /// <inheritdoc />
        public override object Get()
        {
            return (object)((IObjectPool<T>)this).Get();
        }

        /// <inheritdoc />
        public override bool Return(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is T typedObj)
            {
                Return(typedObj);
                return true;
            }
            else
            {
                throw new ArgumentException($"Object type mismatch. Expected {typeof(T).Name}, got {obj.GetType().Name}.", nameof(obj));
            }
        }

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

        #endregion

        #region Object Lifecycle Methods

        /// <summary>
        /// 创建对象（带重试机制）
        /// Create object (with retry mechanism)
        /// </summary>
        /// <returns>创建的对象 / Created object</returns>
        protected virtual T CreateObjectWithRetry()
        {
            Exception lastException = null;

            for (int i = 0; i <= _creationRetryCount; i++)
            {
                try
                {
                    return CreateObject();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (i < _creationRetryCount)
                    {
                        OnRetryAttempt(i + 1, _creationRetryCount, ex);
                    }
                }
            }

            throw new PoolCreationException(
                $"Failed to create object for pool '{_name}' after {_creationRetryCount} retries.",
                lastException);
        }

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

            // 初始化对象状态
            // Initialize object state
            UpdateObjectState(obj, ObjectState.Created);

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

        /// <summary>
        /// 对象重置回调
        /// Object reset callback
        /// </summary>
        /// <param name="obj">要重置的对象 / Object to reset</param>
        /// <remarks>
        /// 在对象归还到池之前调用，用于重置对象状态
        /// Called before object is returned to pool, used to reset object state
        /// </remarks>
        protected virtual void OnObjectReset(T obj)
        {
            // 如果对象实现了 IPooledObject，可以添加 OnReset 方法
            // If object implements IPooledObject, can add OnReset method
            // 这里提供扩展点供子类实现
            // Provides extension point for subclasses to implement
        }

        #endregion

        #region Object State Management

        /// <summary>
        /// 验证对象状态
        /// Validate object state
        /// </summary>
        /// <param name="obj">要验证的对象 / Object to validate</param>
        /// <returns>对象是否有效 / Whether object is valid</returns>
        protected virtual bool ValidateObject(T obj)
        {
            if (obj == null)
            {
                return false;
            }

            // 子类可以重写此方法实现自定义验证逻辑
            // Subclasses can override this method to implement custom validation logic
            return true;
        }

        /// <summary>
        /// 获取对象状态
        /// Get object state
        /// </summary>
        /// <param name="obj">对象 / Object</param>
        /// <returns>对象状态 / Object state</returns>
        protected virtual ObjectState GetObjectState(T obj)
        {
            if (obj == null)
            {
                return ObjectState.Unknown;
            }

            lock (_lock)
            {
                return _objectStates.TryGetValue(obj, out var state) ? state : ObjectState.Unknown;
            }
        }

        /// <summary>
        /// 更新对象状态
        /// Update object state
        /// </summary>
        /// <param name="obj">对象 / Object</param>
        /// <param name="state">新状态 / New state</param>
        protected virtual void UpdateObjectState(T obj, ObjectState state)
        {
            if (obj == null)
            {
                return;
            }

            lock (_lock)
            {
                _objectStates[obj] = state;
            }
        }

        /// <summary>
        /// 移除对象状态
        /// Remove object state
        /// </summary>
        /// <param name="obj">对象 / Object</param>
        protected virtual void RemoveObjectState(T obj)
        {
            if (obj == null)
            {
                return;
            }

            lock (_lock)
            {
                _objectStates.Remove(obj);
            }
        }

        #endregion

        #region Capacity Management

        /// <summary>
        /// 检查并调整容量
        /// Check and adjust capacity
        /// </summary>
        protected virtual void CheckAndAdjustCapacity()
        {
            if (_capacityPolicy == null)
            {
                return;
            }

            var now = DateTime.UtcNow;
            var elapsed = (now - _lastCapacityAdjustment).TotalSeconds;

            if (elapsed < _capacityAdjustmentInterval)
            {
                return;
            }

            _lastCapacityAdjustment = now;

            try
            {
                // 预测所需容量
                // Predict required capacity
                int predictedCapacity = PredictCapacity();

                // 调整容量
                // Adjust capacity
                AdjustCapacity(predictedCapacity);
            }
            catch (Exception ex)
            {
                // 记录容量调整失败，但不影响正常操作
                // Log capacity adjustment failure, but don't affect normal operations
                OnCapacityAdjustmentFailed(ex);
            }
        }

        /// <summary>
        /// 预测所需容量
        /// Predict required capacity
        /// </summary>
        /// <returns>预测的容量 / Predicted capacity</returns>
        protected virtual int PredictCapacity()
        {
            // 基于统计信息预测容量
            // Predict capacity based on statistics
            var stats = _statistics;
            
            // 简单策略：峰值活跃数 + 20% 缓冲
            // Simple strategy: peak active count + 20% buffer
            int predicted = (int)(stats.PeakActive * 1.2f);

            // 确保至少有最小容量
            // Ensure minimum capacity
            return Math.Max(predicted, 10);
        }

        /// <summary>
        /// 调整容量
        /// Adjust capacity
        /// </summary>
        /// <param name="targetCapacity">目标容量 / Target capacity</param>
        protected virtual void AdjustCapacity(int targetCapacity)
        {
            if (_capacityPolicy == null)
            {
                return;
            }

            int currentTotal = _statistics.CurrentTotal;
            int currentIdle = _statistics.CurrentIdle;

            // 如果当前空闲对象过多，清理一些
            // If too many idle objects, clean up some
            if (currentIdle > targetCapacity)
            {
                int toRemove = currentIdle - targetCapacity;
                TrimIdleObjects(toRemove);
            }
            // 如果空闲对象不足，预创建一些
            // If not enough idle objects, pre-create some
            else if (currentIdle < targetCapacity / 2)
            {
                int toCreate = (targetCapacity / 2) - currentIdle;
                PrewarmObjects(toCreate);
            }
        }

        /// <summary>
        /// 修剪空闲对象
        /// Trim idle objects
        /// </summary>
        /// <param name="count">要移除的数量 / Count to remove</param>
        protected virtual void TrimIdleObjects(int count)
        {
            // 子类实现具体的修剪逻辑
            // Subclasses implement specific trimming logic
        }

        /// <summary>
        /// 预热对象池
        /// Prewarm object pool
        /// </summary>
        /// <param name="count">要预创建的数量 / Count to pre-create</param>
        protected virtual void PrewarmObjects(int count)
        {
            // 子类实现具体的预热逻辑
            // Subclasses implement specific prewarming logic
        }

        #endregion

        #region Error Recovery

        /// <summary>
        /// 重试回调
        /// Retry callback
        /// </summary>
        /// <param name="currentRetry">当前重试次数 / Current retry count</param>
        /// <param name="maxRetries">最大重试次数 / Maximum retry count</param>
        /// <param name="exception">异常信息 / Exception</param>
        protected virtual void OnRetryAttempt(int currentRetry, int maxRetries, Exception exception)
        {
            // 子类可以重写此方法记录重试日志
            // Subclasses can override this method to log retry attempts
        }

        /// <summary>
        /// 容量调整失败回调
        /// Capacity adjustment failed callback
        /// </summary>
        /// <param name="exception">异常信息 / Exception</param>
        protected virtual void OnCapacityAdjustmentFailed(Exception exception)
        {
            // 子类可以重写此方法记录容量调整失败日志
            // Subclasses can override this method to log capacity adjustment failures
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
                return _statistics.Clone();
            }
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            // 清空池中所有对象
            // Clear all objects in pool
            OnClear();

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

            if (_capacityPolicy is IDisposable capacityDisposable)
            {
                capacityDisposable.Dispose();
            }

            base.OnDispose();
        }

        #endregion

        #region Nested Class: PoolStatisticsData

        /// <summary>
        /// 池统计数据
        /// Pool statistics data
        /// </summary>
        protected class PoolStatisticsData : IPoolStatistics
        {
            private long _totalCreated;
            private long _totalDestroyed;
            private long _totalGets;
            private long _totalReturns;
            private long _hitCount;
            private long _missCount;
            private int _activeCount;
            private int _idleCount;
            private int _peakActive;
            private int _peakTotal;
            private readonly DateTime _createdTime;

            public long TotalCreated => _totalCreated;
            public long TotalDestroyed => _totalDestroyed;
            public int CurrentActive => _activeCount;
            public int CurrentIdle => _idleCount;
            public int CurrentTotal => _activeCount + _idleCount;
            public long TotalGets => _totalGets;
            public long TotalReturns => _totalReturns;
            public long Hits => _hitCount;
            public long Misses => _missCount;
            public double HitRate => (_hitCount + _missCount) > 0 ? (double)_hitCount / (_hitCount + _missCount) : 0;
            public double MissRate => (_hitCount + _missCount) > 0 ? (double)_missCount / (_hitCount + _missCount) : 0;
            public double AverageGetTime => 0; // 简化实现 / Simplified implementation
            public double AverageReturnTime => 0; // 简化实现 / Simplified implementation
            public int PeakActive => _peakActive;
            public int PeakTotal => _peakTotal;
            public long EstimatedMemoryUsage => 0; // 简化实现 / Simplified implementation
            public DateTime CreatedTime => _createdTime;
            public TimeSpan Uptime => DateTime.UtcNow - _createdTime;
            public DateTime? LastGetTime { get; private set; }
            public DateTime? LastReturnTime { get; private set; }

            public PoolStatisticsData()
            {
                _createdTime = DateTime.UtcNow;
            }

            public void IncrementTotalCreated()
            {
                System.Threading.Interlocked.Increment(ref _totalCreated);
                UpdatePeaks();
            }

            public void IncrementTotalDestroyed()
            {
                System.Threading.Interlocked.Increment(ref _totalDestroyed);
            }

            public void IncrementHitCount()
            {
                System.Threading.Interlocked.Increment(ref _hitCount);
                System.Threading.Interlocked.Increment(ref _totalGets);
                LastGetTime = DateTime.UtcNow;
            }

            public void IncrementMissCount()
            {
                System.Threading.Interlocked.Increment(ref _missCount);
                System.Threading.Interlocked.Increment(ref _totalGets);
                LastGetTime = DateTime.UtcNow;
            }

            public void IncrementActiveCount()
            {
                System.Threading.Interlocked.Increment(ref _activeCount);
                UpdatePeaks();
            }

            public void DecrementActiveCount()
            {
                System.Threading.Interlocked.Decrement(ref _activeCount);
                System.Threading.Interlocked.Increment(ref _totalReturns);
                LastReturnTime = DateTime.UtcNow;
            }

            public void SetIdleCount(int count)
            {
                _idleCount = count;
                UpdatePeaks();
            }

            private void UpdatePeaks()
            {
                int currentActive = _activeCount;
                int currentTotal = _activeCount + _idleCount;

                if (currentActive > _peakActive)
                    _peakActive = currentActive;

                if (currentTotal > _peakTotal)
                    _peakTotal = currentTotal;
            }

            public PoolStatisticsSnapshot CreateSnapshot()
            {
                return new PoolStatisticsSnapshot(this);
            }

            public void Reset()
            {
                _totalGets = 0;
                _totalReturns = 0;
                _hitCount = 0;
                _missCount = 0;
                _peakActive = _activeCount;
                _peakTotal = _activeCount + _idleCount;
                LastGetTime = null;
                LastReturnTime = null;
            }

            public PoolStatisticsData Clone()
            {
                return new PoolStatisticsData
                {
                    _totalCreated = _totalCreated,
                    _totalDestroyed = _totalDestroyed,
                    _totalGets = _totalGets,
                    _totalReturns = _totalReturns,
                    _hitCount = _hitCount,
                    _missCount = _missCount,
                    _activeCount = _activeCount,
                    _idleCount = _idleCount,
                    _peakActive = _peakActive,
                    _peakTotal = _peakTotal,
                    LastGetTime = LastGetTime,
                    LastReturnTime = LastReturnTime
                };
            }
        }

        #endregion

        #region Nested Enums

        /// <summary>
        /// 对象状态枚举
        /// Object state enumeration
        /// </summary>
        protected enum ObjectState
        {
            /// <summary>
            /// 未知状态
            /// Unknown state
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 已创建
            /// Created
            /// </summary>
            Created = 1,

            /// <summary>
            /// 活跃中（已被获取）
            /// Active (retrieved from pool)
            /// </summary>
            Active = 2,

            /// <summary>
            /// 空闲中（在池中等待）
            /// Idle (waiting in pool)
            /// </summary>
            Idle = 3,

            /// <summary>
            /// 已销毁
            /// Destroyed
            /// </summary>
            Destroyed = 4
        }

        #endregion
    }
}
