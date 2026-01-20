// ==========================================================
// 文件名：ConcurrentObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Concurrent
// 功能: 线程安全的并发对象池，基于 ConcurrentBag<T>，支持无锁并发访问
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AFramework.Pool
{
    /// <summary>
    /// 线程安全的并发对象池
    /// Thread-Safe Concurrent Object Pool
    /// 
    /// <para>基于 ConcurrentBag<T> 实现的无锁并发对象池</para>
    /// <para>Lock-free concurrent object pool based on ConcurrentBag<T></para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 特性：
    /// - 基于 ConcurrentBag<T> 实现
    /// - 无锁并发访问（Lock-Free）
    /// - 线程安全，适合多线程场景
    /// - 支持高并发读写
    /// 
    /// 性能：
    /// - Get/Return: O(1) 平均
    /// - 线程安全开销: 低
    /// - 适合高并发场景
    /// 
    /// 注意：
    /// - ConcurrentBag 不保证 FIFO/LIFO 顺序
    /// - 内存占用略高于 Stack<T>
    /// </remarks>
    public class ConcurrentObjectPool<T> : ObjectPoolBase<T>
    {
        #region Fields

        /// <summary>
        /// 并发对象袋（空闲对象）
        /// Concurrent object bag (idle objects)
        /// </summary>
        private readonly ConcurrentBag<T> _objects;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        private readonly int _maxCapacity;

        /// <summary>
        /// 当前活跃对象数量（原子操作）
        /// Current active object count (atomic)
        /// </summary>
        private int _activeCount;

        /// <summary>
        /// 当前空闲对象数量（原子操作）
        /// Current idle object count (atomic)
        /// </summary>
        private int _idleCount;

        #endregion

        #region Properties

        /// <inheritdoc />
        public override Type ObjectType => typeof(T);

        /// <inheritdoc />
        public override int TotalCount => _idleCount + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _idleCount;

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
        public ConcurrentObjectPool(
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
            _objects = new ConcurrentBag<T>();
            _activeCount = 0;
            _idleCount = 0;
        }

        /// <summary>
        /// 构造函数（简化版）
        /// Constructor (simplified)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="name">池名称 / Pool name</param>
        public ConcurrentObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, null, null, name)
        {
        }

        #endregion

        #region Initialization

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 如果有容量策略，执行预热
            // If capacity policy exists, perform warmup
            if (_capacityPolicy != null)
            {
                int warmupCount = _capacityPolicy.GetInitialCapacity();
                Warmup(warmupCount);
            }
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            if (count <= 0)
            {
                return;
            }

            int targetCount = Math.Min(count, _maxCapacity);

            for (int i = 0; i < targetCount; i++)
            {
                T obj = CreateObject();
                _objects.Add(obj);
                Interlocked.Increment(ref _idleCount);
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

            while (_idleCount > targetCount && _objects.TryTake(out T obj))
            {
                DestroyObject(obj);
                _statistics.IncrementTotalDestroyed();
                Interlocked.Decrement(ref _idleCount);
                removedCount++;
            }

            return removedCount;
        }

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T obj)
        {
            if (_objects.TryTake(out obj))
            {
                Interlocked.Decrement(ref _idleCount);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(T obj)
        {
            // 检查容量限制（原子操作）
            // Check capacity limit (atomic)
            int currentIdle = _idleCount;
            if (currentIdle >= _maxCapacity)
            {
                return false;
            }

            // 检查容量策略
            // Check capacity policy
            if (_capacityPolicy != null && !_capacityPolicy.CanReturn(currentIdle, _maxCapacity))
            {
                return false;
            }

            _objects.Add(obj);
            Interlocked.Increment(ref _idleCount);
            return true;
        }

        #endregion

        #region Object Lifecycle Override

        /// <inheritdoc />
        protected override void OnObjectGet(T obj)
        {
            base.OnObjectGet(obj);
            Interlocked.Increment(ref _activeCount);
        }

        /// <inheritdoc />
        protected override void OnObjectReturn(T obj)
        {
            base.OnObjectReturn(obj);
            Interlocked.Decrement(ref _activeCount);
        }

        #endregion

        #region Clear Implementation

        /// <inheritdoc />
        protected override void OnClear()
        {
            while (_objects.TryTake(out T obj))
            {
                DestroyObject(obj);
                _statistics.IncrementTotalDestroyed();
                Interlocked.Decrement(ref _idleCount);
            }
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            base.OnDispose();

            // 清空并发袋
            // Clear concurrent bag
            while (_objects.TryTake(out _))
            {
                // 仅清空，不销毁（已在 OnClear 中处理）
                // Just clear, don't destroy (already handled in OnClear)
            }
        }

        #endregion

        #region Get/Return Implementation

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
