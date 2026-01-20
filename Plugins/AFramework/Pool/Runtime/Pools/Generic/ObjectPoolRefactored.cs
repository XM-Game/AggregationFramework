// ==========================================================
// 文件名：ObjectPoolRefactored.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 重构后的标准泛型对象池实现，使用扁平化基类
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 重构后的标准泛型对象池
    /// Refactored Standard Generic Object Pool
    /// 
    /// <para>基于扁平化基类的标准对象池实现</para>
    /// <para>Standard object pool implementation based on flattened base class</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 改进点：
    /// 1. 继承自扁平化基类，减少继承层次
    /// 2. 实现更简洁，只需关注核心池逻辑
    /// 3. 统计、预热等功能由组件提供
    /// 
    /// 特性：
    /// - 基于 Stack<T> 实现，LIFO 顺序
    /// - 线程安全（使用 lock）
    /// - 适合大多数场景
    /// </remarks>
    public class ObjectPoolRefactored<T> : ObjectPoolBaseRefactored<T>
    {
        #region Fields

        /// <summary>
        /// 对象栈（空闲对象）
        /// Object stack (idle objects)
        /// </summary>
        private readonly Stack<T> _objects;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        private readonly int _maxCapacity;

        /// <summary>
        /// 当前活跃对象数量
        /// Current active object count
        /// </summary>
        private int _activeCount;

        #endregion

        #region Properties

        /// <inheritdoc />
        public override int TotalCount => _objects.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _objects.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="cleanupPolicy">清理策略 / Cleanup policy</param>
        /// <param name="capacityPolicy">容量策略 / Capacity policy</param>
        /// <param name="name">池名称 / Pool name</param>
        public ObjectPoolRefactored(
            IPoolCreationPolicy<T> creationPolicy,
            int maxCapacity = 100,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            IPoolCapacityPolicy capacityPolicy = null,
            string name = null)
            : base(creationPolicy, cleanupPolicy, capacityPolicy, name)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than 0.");
            }

            _maxCapacity = maxCapacity;
            _objects = new Stack<T>(maxCapacity);
            _activeCount = 0;
        }

        /// <summary>
        /// 构造函数（简化版）
        /// Constructor (simplified)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="name">池名称 / Pool name</param>
        public ObjectPoolRefactored(
            Func<T> createFunc,
            int maxCapacity = 100,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, null, null, name)
        {
        }

        #endregion

        #region Core Pool Operations

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T obj)
        {
            if (_objects.Count > 0)
            {
                obj = _objects.Pop();
                _activeCount++;
                return true;
            }

            obj = default;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(T obj)
        {
            // 检查容量限制
            // Check capacity limit
            if (_objects.Count >= _maxCapacity)
            {
                return false;
            }

            _objects.Push(obj);
            _activeCount--;
            return true;
        }

        /// <inheritdoc />
        protected override void AddObjectToPool(T obj)
        {
            if (_objects.Count < _maxCapacity)
            {
                _objects.Push(obj);
            }
        }

        #endregion

        #region Warmup and Shrink

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
                int targetCount = Math.Min(count, _maxCapacity);

                for (int i = _objects.Count; i < targetCount; i++)
                {
                    T obj = CreateObject();
                    _objects.Push(obj);
                    _statisticsComponent?.RecordCreation();
                }

                _statisticsComponent?.SetIdleCount(_objects.Count);
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

            lock (_lock)
            {
                int removedCount = 0;
                while (_objects.Count > targetCount)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statisticsComponent?.RecordDestruction();
                    removedCount++;
                }

                _statisticsComponent?.SetIdleCount(_objects.Count);
                return removedCount;
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
                _statisticsComponent?.RecordDestruction();
            }

            _statisticsComponent?.SetIdleCount(0);
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            _objects.Clear();
            _activeCount = 0;
            base.OnDispose();
        }

        #endregion

        #region Nested Class: FuncCreationPolicy

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        private class FuncCreationPolicy<TObj> : IPoolCreationPolicy<TObj>
        {
            private readonly Func<TObj> _createFunc;

            public string Name => "FuncCreationPolicy";
            public string Description => "Function-based creation policy";

            public FuncCreationPolicy(Func<TObj> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            public TObj Create()
            {
                return _createFunc();
            }

            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion
    }
}
