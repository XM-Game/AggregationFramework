// ==========================================================
// 文件名：DynamicObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 动态扩容对象池，自动扩容和收缩，适合负载波动场景
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 动态扩容对象池
    /// Dynamic Object Pool
    /// 
    /// <para>支持自动扩容和收缩的对象池，适合负载波动场景</para>
    /// <para>Object pool with automatic expansion and shrinking, suitable for fluctuating load scenarios</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 特性：
    /// - 自动扩容（按倍数增长）
    /// - 自动收缩（定时清理空闲对象）
    /// - 智能容量管理
    /// - 适合负载波动场景
    /// 
    /// 扩容策略：
    /// - 初始容量：initialCapacity
    /// - 扩容因子：growthFactor（默认 2.0）
    /// - 最大容量：maxCapacity
    /// 
    /// 收缩策略：
    /// - 空闲阈值：idleThreshold（默认 50%）
    /// - 收缩因子：shrinkFactor（默认 0.5）
    /// - 最小容量：minCapacity
    /// 
    /// 性能：
    /// - Get/Return: O(1) 平均
    /// - 扩容时: O(n)
    /// - 内存占用: 动态
    /// </remarks>
    public class DynamicObjectPool<T> : ObjectPoolBase<T>
    {
        #region Fields

        /// <summary>
        /// 对象栈（空闲对象）
        /// Object stack (idle objects)
        /// </summary>
        private Stack<T> _objects;

        /// <summary>
        /// 当前容量
        /// Current capacity
        /// </summary>
        private int _currentCapacity;

        /// <summary>
        /// 初始容量
        /// Initial capacity
        /// </summary>
        private readonly int _initialCapacity;

        /// <summary>
        /// 最小容量
        /// Minimum capacity
        /// </summary>
        private readonly int _minCapacity;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        private readonly int _maxCapacity;

        /// <summary>
        /// 扩容因子
        /// Growth factor
        /// </summary>
        private readonly float _growthFactor;

        /// <summary>
        /// 收缩因子
        /// Shrink factor
        /// </summary>
        private readonly float _shrinkFactor;

        /// <summary>
        /// 空闲阈值（触发收缩）
        /// Idle threshold (trigger shrinking)
        /// </summary>
        private readonly float _idleThreshold;

        /// <summary>
        /// 当前活跃对象数量
        /// Current active object count
        /// </summary>
        private int _activeCount;

        /// <summary>
        /// 扩容次数
        /// Expansion count
        /// </summary>
        private int _expansionCount;

        /// <summary>
        /// 收缩次数
        /// Shrink count
        /// </summary>
        private int _shrinkCount;

        #endregion

        #region Properties

        /// <inheritdoc />
        public override Type ObjectType => typeof(T);

        /// <inheritdoc />
        public override int TotalCount => _objects.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _objects.Count;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        public int MaxCapacity => _maxCapacity;

        /// <summary>
        /// 最小容量
        /// Minimum capacity
        /// </summary>
        public int MinCapacity => _minCapacity;

        /// <summary>
        /// 扩容次数
        /// Expansion count
        /// </summary>
        public int ExpansionCount => _expansionCount;

        /// <summary>
        /// 收缩次数
        /// Shrink count
        /// </summary>
        public int ShrinkCount => _shrinkCount;

        /// <summary>
        /// 使用率（活跃对象 / 当前容量）
        /// Utilization rate (active objects / current capacity)
        /// </summary>
        public float UtilizationRate => _currentCapacity > 0 ? (float)_activeCount / _currentCapacity : 0f;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="initialCapacity">初始容量 / Initial capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="minCapacity">最小容量 / Minimum capacity</param>
        /// <param name="growthFactor">扩容因子 / Growth factor</param>
        /// <param name="shrinkFactor">收缩因子 / Shrink factor</param>
        /// <param name="idleThreshold">空闲阈值 / Idle threshold</param>
        /// <param name="cleanupPolicy">清理策略 / Cleanup policy</param>
        /// <param name="name">池名称 / Pool name</param>
        public DynamicObjectPool(
            IPoolCreationPolicy<T> creationPolicy,
            int initialCapacity = 10,
            int maxCapacity = 1000,
            int minCapacity = 10,
            float growthFactor = 2.0f,
            float shrinkFactor = 0.5f,
            float idleThreshold = 0.5f,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            string name = null)
            : base(creationPolicy, cleanupPolicy, null, name)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be greater than 0.");
            }

            if (maxCapacity < initialCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than or equal to initial capacity.");
            }

            if (minCapacity <= 0 || minCapacity > initialCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(minCapacity), "Min capacity must be greater than 0 and less than or equal to initial capacity.");
            }

            if (growthFactor <= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(growthFactor), "Growth factor must be greater than 1.0.");
            }

            if (shrinkFactor <= 0f || shrinkFactor >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(shrinkFactor), "Shrink factor must be between 0 and 1.");
            }

            if (idleThreshold <= 0f || idleThreshold >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(idleThreshold), "Idle threshold must be between 0 and 1.");
            }

            _initialCapacity = initialCapacity;
            _currentCapacity = initialCapacity;
            _minCapacity = minCapacity;
            _maxCapacity = maxCapacity;
            _growthFactor = growthFactor;
            _shrinkFactor = shrinkFactor;
            _idleThreshold = idleThreshold;
            _objects = new Stack<T>(initialCapacity);
            _activeCount = 0;
            _expansionCount = 0;
            _shrinkCount = 0;
        }

        /// <summary>
        /// 构造函数（简化版）
        /// Constructor (simplified)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="initialCapacity">初始容量 / Initial capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="name">池名称 / Pool name</param>
        public DynamicObjectPool(
            Func<T> createFunc,
            int initialCapacity = 10,
            int maxCapacity = 1000,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), initialCapacity, maxCapacity, initialCapacity, 2.0f, 0.5f, 0.5f, null, name)
        {
        }

        #endregion

        #region Initialization

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 预热到初始容量的一半
            // Warmup to half of initial capacity
            Warmup(_initialCapacity / 2);
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            if (count <= 0)
            {
                return;
            }

            lock (_lock)
            {
                int targetCount = Math.Min(count, _currentCapacity);

                for (int i = _objects.Count; i < targetCount; i++)
                {
                    T obj = CreateObject();
                    _objects.Push(obj);
                }
            }
        }

        /// <inheritdoc />
        public override int Shrink(int targetCount)
        {
            ThrowIfNotActive();

            if (targetCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetCount), "Target count must be non-negative.");
            }

            int removedCount = 0;

            lock (_lock)
            {
                while (_objects.Count > targetCount)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statistics.IncrementTotalDestroyed();
                    removedCount++;
                }
            }

            return removedCount;
        }

        #endregion

        #region Get/Return Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T obj)
        {
            if (_objects.Count > 0)
            {
                obj = _objects.Pop();
                _activeCount++;
                return true;
            }

            // 池为空，检查是否需要扩容
            // Pool is empty, check if expansion is needed
            if (_activeCount >= _currentCapacity && _currentCapacity < _maxCapacity)
            {
                ExpandCapacity();
            }

            obj = default;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(T obj)
        {
            // 检查是否需要收缩
            // Check if shrinking is needed
            if (ShouldShrink())
            {
                ShrinkCapacity();
            }

            // 检查容量限制
            // Check capacity limit
            if (_objects.Count >= _currentCapacity)
            {
                return false;
            }

            _objects.Push(obj);
            _activeCount--;
            return true;
        }

        #endregion

        #region Capacity Management

        /// <summary>
        /// 扩容
        /// Expand capacity
        /// </summary>
        private void ExpandCapacity()
        {
            int newCapacity = Math.Min((int)(_currentCapacity * _growthFactor), _maxCapacity);

            if (newCapacity > _currentCapacity)
            {
                _currentCapacity = newCapacity;
                _expansionCount++;

                // 记录扩容事件
                // Log expansion event
                System.Diagnostics.Debug.WriteLine($"Pool '{_name}' expanded to {_currentCapacity}");
            }
        }

        /// <summary>
        /// 收缩
        /// Shrink capacity
        /// </summary>
        private void ShrinkCapacity()
        {
            int newCapacity = Math.Max((int)(_currentCapacity * _shrinkFactor), _minCapacity);

            if (newCapacity < _currentCapacity)
            {
                // 销毁多余的空闲对象
                // Destroy excess idle objects
                int excessCount = _objects.Count - newCapacity;
                for (int i = 0; i < excessCount && _objects.Count > 0; i++)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statistics.IncrementTotalDestroyed();
                }

                _currentCapacity = newCapacity;
                _shrinkCount++;

                // 记录收缩事件
                // Log shrink event
                System.Diagnostics.Debug.WriteLine($"Pool '{_name}' shrunk to {_currentCapacity}");
            }
        }

        /// <summary>
        /// 检查是否应该收缩
        /// Check if should shrink
        /// </summary>
        /// <returns>是否应该收缩 / Whether should shrink</returns>
        private bool ShouldShrink()
        {
            // 条件：空闲对象超过阈值，且当前容量大于最小容量
            // Condition: Idle objects exceed threshold, and current capacity is greater than minimum capacity
            float idleRate = _currentCapacity > 0 ? (float)_objects.Count / _currentCapacity : 0f;
            return idleRate > _idleThreshold && _currentCapacity > _minCapacity;
        }

        /// <summary>
        /// 手动触发收缩
        /// Manually trigger shrinking
        /// </summary>
        public void TriggerShrink()
        {
            ThrowIfNotActive();

            lock (_lock)
            {
                if (ShouldShrink())
                {
                    ShrinkCapacity();
                }
            }
        }

        #endregion

        #region Clear Implementation

        /// <inheritdoc />
        protected override void OnClear()
        {
            while (_objects.Count > 0)
            {
                T obj = _objects.Pop();
                DestroyObject(obj);
                _statistics.IncrementTotalDestroyed();
            }

            // 重置容量到初始值
            // Reset capacity to initial value
            _currentCapacity = _initialCapacity;
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            base.OnDispose();
            _objects.Clear();
            _objects = null;
        }

        #endregion

        #region Statistics Methods

        /// <summary>
        /// 获取容量统计信息
        /// Get capacity statistics
        /// </summary>
        /// <returns>容量统计信息 / Capacity statistics</returns>
        public CapacityStatistics GetCapacityStatistics()
        {
            lock (_lock)
            {
                return new CapacityStatistics
                {
                    CurrentCapacity = _currentCapacity,
                    MinCapacity = _minCapacity,
                    MaxCapacity = _maxCapacity,
                    InitialCapacity = _initialCapacity,
                    ExpansionCount = _expansionCount,
                    ShrinkCount = _shrinkCount,
                    UtilizationRate = UtilizationRate,
                    IdleRate = _currentCapacity > 0 ? (float)_objects.Count / _currentCapacity : 0f
                };
            }
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// 容量统计信息
        /// Capacity statistics
        /// </summary>
        public class CapacityStatistics
        {
            /// <summary>当前容量 / Current capacity</summary>
            public int CurrentCapacity { get; set; }

            /// <summary>最小容量 / Minimum capacity</summary>
            public int MinCapacity { get; set; }

            /// <summary>最大容量 / Maximum capacity</summary>
            public int MaxCapacity { get; set; }

            /// <summary>初始容量 / Initial capacity</summary>
            public int InitialCapacity { get; set; }

            /// <summary>扩容次数 / Expansion count</summary>
            public int ExpansionCount { get; set; }

            /// <summary>收缩次数 / Shrink count</summary>
            public int ShrinkCount { get; set; }

            /// <summary>使用率 / Utilization rate</summary>
            public float UtilizationRate { get; set; }

            /// <summary>空闲率 / Idle rate</summary>
            public float IdleRate { get; set; }

            public override string ToString()
            {
                return $"Capacity: {CurrentCapacity} (Min: {MinCapacity}, Max: {MaxCapacity}), " +
                       $"Expansions: {ExpansionCount}, Shrinks: {ShrinkCount}, " +
                       $"Utilization: {UtilizationRate:P}, Idle: {IdleRate:P}";
            }
        }

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        private class FuncCreationPolicy<TObj> : IPoolCreationPolicy<TObj>
        {
            private readonly Func<TObj> _createFunc;

            /// <inheritdoc />
            public string Name => "FuncCreationPolicy";

            /// <inheritdoc />
            public string Description => "Function-based creation policy";

            public FuncCreationPolicy(Func<TObj> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            public TObj Create()
            {
                return _createFunc();
            }

            /// <inheritdoc />
            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion
    }
}
