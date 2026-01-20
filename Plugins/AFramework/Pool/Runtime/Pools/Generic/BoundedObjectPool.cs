// ==========================================================
// 文件名：BoundedObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 有界对象池，严格容量控制，防止内存泄漏
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 有界对象池
    /// Bounded Object Pool
    /// 
    /// <para>严格容量控制的对象池，防止内存泄漏</para>
    /// <para>Object pool with strict capacity control to prevent memory leaks</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 特性：
    /// - 严格容量限制
    /// - 支持阻塞/拒绝策略
    /// - 防止内存泄漏
    /// - 适合资源受限场景
    /// 
    /// 策略：
    /// - Reject: 超出容量时拒绝归还，销毁对象
    /// - Block: 超出容量时阻塞等待（可选超时）
    /// 
    /// 性能：
    /// - Get/Return: O(1)
    /// - 内存占用: 固定
    /// - 适合资源受限场景
    /// </remarks>
    public class BoundedObjectPool<T> : ObjectPoolBase<T>
    {
        #region Enums

        /// <summary>
        /// 容量超限策略
        /// Capacity exceeded policy
        /// </summary>
        public enum CapacityExceededPolicy
        {
            /// <summary>拒绝归还，销毁对象 / Reject return, destroy object</summary>
            Reject,

            /// <summary>阻塞等待空闲位置 / Block and wait for available slot</summary>
            Block
        }

        #endregion

        #region Fields

        /// <summary>
        /// 对象队列（空闲对象）
        /// Object queue (idle objects)
        /// </summary>
        private readonly Queue<T> _objects;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        private readonly int _maxCapacity;

        /// <summary>
        /// 容量超限策略
        /// Capacity exceeded policy
        /// </summary>
        private readonly CapacityExceededPolicy _exceededPolicy;

        /// <summary>
        /// 当前活跃对象数量
        /// Current active object count
        /// </summary>
        private int _activeCount;

        /// <summary>
        /// 阻塞超时时间（毫秒）
        /// Block timeout (milliseconds)
        /// </summary>
        private readonly int _blockTimeout;

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
        /// 容量超限策略
        /// Capacity exceeded policy
        /// </summary>
        public CapacityExceededPolicy ExceededPolicy => _exceededPolicy;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="exceededPolicy">容量超限策略 / Capacity exceeded policy</param>
        /// <param name="blockTimeout">阻塞超时时间（毫秒，-1 表示无限等待）/ Block timeout (milliseconds, -1 for infinite)</param>
        /// <param name="cleanupPolicy">清理策略 / Cleanup policy</param>
        /// <param name="name">池名称 / Pool name</param>
        public BoundedObjectPool(
            IPoolCreationPolicy<T> creationPolicy,
            int maxCapacity = 100,
            CapacityExceededPolicy exceededPolicy = CapacityExceededPolicy.Reject,
            int blockTimeout = -1,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            string name = null)
            : base(creationPolicy, cleanupPolicy, null, name)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than 0.");
            }

            _maxCapacity = maxCapacity;
            _exceededPolicy = exceededPolicy;
            _blockTimeout = blockTimeout;
            _objects = new Queue<T>(maxCapacity);
            _activeCount = 0;
        }

        /// <summary>
        /// 构造函数（简化版）
        /// Constructor (simplified)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="exceededPolicy">容量超限策略 / Capacity exceeded policy</param>
        /// <param name="name">池名称 / Pool name</param>
        public BoundedObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            CapacityExceededPolicy exceededPolicy = CapacityExceededPolicy.Reject,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, exceededPolicy, -1, null, name)
        {
        }

        #endregion

        #region Warmup/Shrink Implementation

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
                    _objects.Enqueue(obj);
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

            lock (_lock)
            {
                int removed = 0;
                while (_objects.Count > targetCount)
                {
                    T obj = _objects.Dequeue();
                    DestroyObject(obj);
                    _statistics.IncrementTotalDestroyed();
                    removed++;
                }

                // 通知等待的线程
                // Notify waiting threads
                System.Threading.Monitor.PulseAll(_lock);

                return removed;
            }
        }

        #endregion

        #region Get/Return Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T obj)
        {
            if (_objects.Count > 0)
            {
                obj = _objects.Dequeue();
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
                // 根据策略处理
                // Handle according to policy
                switch (_exceededPolicy)
                {
                    case CapacityExceededPolicy.Reject:
                        // 拒绝归还
                        // Reject return
                        return false;

                    case CapacityExceededPolicy.Block:
                        // 阻塞等待（简化实现，实际应使用信号量）
                        // Block and wait (simplified implementation, should use semaphore in production)
                        if (!WaitForAvailableSlot())
                        {
                            return false;
                        }
                        break;
                }
            }

            _objects.Enqueue(obj);
            _activeCount--;
            return true;
        }

        /// <summary>
        /// 等待可用位置
        /// Wait for available slot
        /// </summary>
        /// <returns>是否成功等待到位置 / Whether successfully waited for slot</returns>
        private bool WaitForAvailableSlot()
        {
            // 简化实现：使用 Monitor.Wait
            // Simplified implementation: use Monitor.Wait
            if (_blockTimeout < 0)
            {
                // 无限等待
                // Infinite wait
                while (_objects.Count >= _maxCapacity)
                {
                    System.Threading.Monitor.Wait(_lock);
                }
                return true;
            }
            else
            {
                // 超时等待
                // Timeout wait
                DateTime startTime = DateTime.UtcNow;
                while (_objects.Count >= _maxCapacity)
                {
                    int remainingTimeout = _blockTimeout - (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    if (remainingTimeout <= 0)
                    {
                        return false;
                    }

                    System.Threading.Monitor.Wait(_lock, remainingTimeout);
                }
                return true;
            }
        }

        #endregion

        #region Clear Implementation

        /// <inheritdoc />
        protected override void OnClear()
        {
            while (_objects.Count > 0)
            {
                T obj = _objects.Dequeue();
                DestroyObject(obj);
                _statistics.IncrementTotalDestroyed();
            }

            // 通知等待的线程
            // Notify waiting threads
            System.Threading.Monitor.PulseAll(_lock);
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            base.OnDispose();

            _objects.Clear();

            // 通知等待的线程
            // Notify waiting threads
            System.Threading.Monitor.PulseAll(_lock);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 检查是否已满
        /// Check if the pool is full
        /// </summary>
        /// <returns>是否已满 / Whether full</returns>
        public bool IsFull()
        {
            lock (_lock)
            {
                return _objects.Count >= _maxCapacity;
            }
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
