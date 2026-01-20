// ==========================================================
// 文件名：QueueObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 基于队列的对象池，FIFO 顺序，适合需要公平调度的场景
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 基于队列的对象池
    /// Queue-Based Object Pool
    /// 
    /// <para>基于 Queue&lt;T&gt; 实现的 FIFO 对象池，适合需要公平调度的场景</para>
    /// <para>FIFO object pool based on Queue&lt;T&gt;, suitable for scenarios requiring fair scheduling</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// <para><strong>设计模式 Design Patterns:</strong></para>
    /// <list type="bullet">
    /// <item><description>对象池模式 (Object Pool Pattern) - 重用对象减少 GC</description></item>
    /// <item><description>策略模式 (Strategy Pattern) - 可配置的创建/清理/容量策略</description></item>
    /// </list>
    /// 
    /// <para><strong>特性 Features:</strong></para>
    /// <list type="bullet">
    /// <item><description>基于 Queue&lt;T&gt; 实现，FIFO 顺序（先进先出）</description></item>
    /// <item><description>公平调度，避免对象饥饿</description></item>
    /// <item><description>适合需要均匀使用对象的场景</description></item>
    /// <item><description>线程安全（使用 lock）</description></item>
    /// </list>
    /// 
    /// <para><strong>性能 Performance:</strong></para>
    /// <list type="bullet">
    /// <item><description>Get/Return: O(1) 时间复杂度</description></item>
    /// <item><description>内存占用: 低（仅 Queue 开销）</description></item>
    /// <item><description>GC 压力: 低（对象重用）</description></item>
    /// <item><description>线程安全: 是（lock 同步）</description></item>
    /// </list>
    /// 
    /// <para><strong>使用场景 Use Cases:</strong></para>
    /// <list type="bullet">
    /// <item><description>连接池（公平分配连接）</description></item>
    /// <item><description>任务队列（公平调度任务）</description></item>
    /// <item><description>资源池（均匀使用资源，避免某些资源过度使用）</description></item>
    /// <item><description>需要 FIFO 顺序的场景</description></item>
    /// </list>
    /// 
    /// <para><strong>与 ObjectPool 的区别 Difference from ObjectPool:</strong></para>
    /// <list type="bullet">
    /// <item><description>ObjectPool: LIFO（后进先出），最近归还的对象优先被获取</description></item>
    /// <item><description>QueueObjectPool: FIFO（先进先出），最早归还的对象优先被获取</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para><strong>基础使用示例 Basic Usage:</strong></para>
    /// <code>
    /// // 创建队列对象池
    /// var pool = new QueueObjectPool&lt;Connection&gt;(
    ///     createFunc: () => new Connection(),
    ///     maxCapacity: 100,
    ///     name: "ConnectionPool"
    /// );
    /// 
    /// // 预热池
    /// pool.Warmup(10);
    /// 
    /// // 获取连接（FIFO 顺序）
    /// var conn = pool.Get();
    /// conn.Execute();
    /// 
    /// // 归还连接
    /// pool.Return(conn);
    /// </code>
    /// 
    /// <para><strong>公平调度示例 Fair Scheduling Example:</strong></para>
    /// <code>
    /// // 任务队列池，确保任务公平调度
    /// var taskPool = new QueueObjectPool&lt;Task&gt;(
    ///     createFunc: () => new Task(),
    ///     maxCapacity: 50,
    ///     name: "TaskPool"
    /// );
    /// 
    /// // 查看队列头部任务（不移除）
    /// if (taskPool.TryPeek(out var nextTask))
    /// {
    ///     Debug.Log($"Next task: {nextTask.Name}");
    /// }
    /// 
    /// // 获取任务执行
    /// var task = taskPool.Get();
    /// task.Execute();
    /// taskPool.Return(task);
    /// </code>
    /// </example>
    public class QueueObjectPool<T> : ObjectPoolBase<T>
    {
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
        /// 当前活跃对象数量
        /// Current active object count
        /// </summary>
        private int _activeCount;

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

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="cleanupPolicy">清理策略（可选）/ Cleanup policy (optional)</param>
        /// <param name="capacityPolicy">容量策略（可选）/ Capacity policy (optional)</param>
        /// <param name="name">池名称（可选）/ Pool name (optional)</param>
        /// <exception cref="ArgumentNullException">creationPolicy 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxCapacity 小于等于 0</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: Queue&lt;T&gt; 预分配 maxCapacity 容量</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pool = new QueueObjectPool&lt;Connection&gt;(
        ///     creationPolicy: new DefaultCreationPolicy&lt;Connection&gt;(),
        ///     maxCapacity: 100,
        ///     cleanupPolicy: new ResetCleanupPolicy&lt;Connection&gt;(),
        ///     name: "ConnectionPool"
        /// );
        /// </code>
        /// </example>
        public QueueObjectPool(
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
            _objects = new Queue<T>(maxCapacity);
            _activeCount = 0;
        }

        /// <summary>
        /// 构造函数（简化版）
        /// Constructor (simplified)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="name">池名称（可选）/ Pool name (optional)</param>
        /// <exception cref="ArgumentNullException">createFunc 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxCapacity 小于等于 0</exception>
        /// <remarks>
        /// <para>简化构造函数，使用函数委托作为创建策略</para>
        /// <para>Simplified constructor using function delegate as creation policy</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pool = new QueueObjectPool&lt;Connection&gt;(
        ///     createFunc: () => new Connection(),
        ///     maxCapacity: 100,
        ///     name: "ConnectionPool"
        /// );
        /// </code>
        /// </example>
        public QueueObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, null, null, name)
        {
        }

        #endregion

        #region Initialization

        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="PoolCreationException">对象创建失败</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为预热数量</description></item>
        /// <item><description>内存分配: 创建 n 个对象</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>预热数量会被限制在最大容量内</para>
        /// <para>Warmup count will be capped at maximum capacity</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 预热池，预创建 20 个连接
        /// pool.Warmup(20);
        /// </code>
        /// </example>
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
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="InvalidOperationException">池未激活</exception>
        /// <exception cref="ArgumentOutOfRangeException">targetCount 小于 0</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为需要移除的对象数量</description></item>
        /// <item><description>内存分配: 无（仅释放对象）</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// <para><strong>使用场景 Use Cases:</strong></para>
        /// <list type="bullet">
        /// <item><description>内存压力大时减少池容量</description></item>
        /// <item><description>游戏场景切换时释放不需要的对象</description></item>
        /// <item><description>动态调整池大小以适应当前负载</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 将池缩减到 10 个对象
        /// int removed = pool.Shrink(10);
        /// Debug.Log($"Removed {removed} objects from pool");
        /// </code>
        /// </example>
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
                return removed;
            }
        }

        #endregion

        #region Core Operations - Get/Return Implementation

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 无（从队列中出队）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>FIFO 顺序，最早归还的对象优先被获取</para>
        /// <para>FIFO order, earliest returned object is retrieved first</para>
        /// </remarks>
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
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 无（入队）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// </remarks>
        protected override bool TryReturnToPool(T obj)
        {
            // 检查容量限制
            // Check capacity limit
            if (_objects.Count >= _maxCapacity)
            {
                return false;
            }

            // 检查容量策略
            // Check capacity policy
            if (_capacityPolicy != null && !_capacityPolicy.CanReturn(_objects.Count, _maxCapacity))
            {
                return false;
            }

            _objects.Enqueue(obj);
            _activeCount--;
            return true;
        }

        #endregion

        #region Clear Implementation

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为池中对象数量</description></item>
        /// <item><description>内存分配: 无（仅释放对象）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>此方法会销毁所有空闲对象，但不影响已获取的活跃对象</para>
        /// <para>This method destroys all idle objects but does not affect active objects</para>
        /// </remarks>
        protected override void OnClear()
        {
            while (_objects.Count > 0)
            {
                T obj = _objects.Dequeue();
                DestroyObject(obj);
                _statistics.IncrementTotalDestroyed();
            }
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为队列容量</description></item>
        /// <item><description>内存分配: 无（仅释放资源）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>此方法会清空队列并调用基类的 OnDispose，确保所有资源被正确释放</para>
        /// <para>This method clears the queue and calls base OnDispose to ensure all resources are properly released</para>
        /// </remarks>
        protected override void OnDispose()
        {
            base.OnDispose();
            _objects.Clear();
        }

        #endregion

        #region Queue-Specific Operations

        /// <summary>
        /// 查看队列头部对象（不移除）
        /// Peek at the head object (without removing)
        /// </summary>
        /// <returns>队列头部对象 / Head object</returns>
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="InvalidOperationException">池为空或未激活</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 无</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// <para><strong>使用场景 Use Cases:</strong></para>
        /// <list type="bullet">
        /// <item><description>检查下一个可用对象而不获取它</description></item>
        /// <item><description>预览队列头部对象的状态</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 查看下一个连接
        /// var nextConn = pool.Peek();
        /// Debug.Log($"Next connection: {nextConn.Id}");
        /// </code>
        /// </example>
        public T Peek()
        {
            ThrowIfNotActive();

            lock (_lock)
            {
                if (_objects.Count == 0)
                {
                    throw new InvalidOperationException($"Pool '{_name}' is empty.");
                }

                return _objects.Peek();
            }
        }

        /// <summary>
        /// 尝试查看队列头部对象（不移除）
        /// Try to peek at the head object (without removing)
        /// </summary>
        /// <param name="obj">队列头部对象 / Head object</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="InvalidOperationException">池未激活</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 无</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>此方法不会抛出异常，如果队列为空则返回 false</para>
        /// <para>This method does not throw exceptions, returns false if queue is empty</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 安全地查看下一个连接
        /// if (pool.TryPeek(out var nextConn))
        /// {
        ///     Debug.Log($"Next connection: {nextConn.Id}");
        /// }
        /// else
        /// {
        ///     Debug.Log("Pool is empty");
        /// }
        /// </code>
        /// </example>
        public bool TryPeek(out T obj)
        {
            ThrowIfNotActive();

            lock (_lock)
            {
                if (_objects.Count > 0)
                {
                    obj = _objects.Peek();
                    return true;
                }

                obj = default;
                return false;
            }
        }

        #endregion

        #region Nested Class: FuncCreationPolicy

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        /// <typeparam name="TObj">对象类型 / Object type</typeparam>
        /// <remarks>
        /// <para><strong>设计模式 Design Pattern:</strong></para>
        /// <para>策略模式 (Strategy Pattern) - 将创建逻辑封装为策略对象</para>
        /// 
        /// <para><strong>用途 Purpose:</strong></para>
        /// <para>简化对象池构造，允许使用函数委托作为创建策略</para>
        /// <para>Simplifies pool construction by allowing function delegates as creation policies</para>
        /// 
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>Create: O(1) + 函数执行时间</description></item>
        /// <item><description>Validate: O(1)</description></item>
        /// <item><description>内存占用: 极低（仅存储函数引用）</description></item>
        /// </list>
        /// </remarks>
        private class FuncCreationPolicy<TObj> : IPoolCreationPolicy<TObj>
        {
            private readonly Func<TObj> _createFunc;

            /// <summary>
            /// 策略名称
            /// Policy name
            /// </summary>
            public string Name => "FuncCreationPolicy";

            /// <summary>
            /// 策略描述
            /// Policy description
            /// </summary>
            public string Description => "Function-based creation policy";

            /// <summary>
            /// 构造函数
            /// Constructor
            /// </summary>
            /// <param name="createFunc">创建函数 / Create function</param>
            /// <exception cref="ArgumentNullException">createFunc 为 null</exception>
            public FuncCreationPolicy(Func<TObj> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            /// <summary>
            /// 创建对象
            /// Create object
            /// </summary>
            /// <returns>新创建的对象 / Newly created object</returns>
            public TObj Create()
            {
                return _createFunc();
            }

            /// <summary>
            /// 验证策略
            /// Validate policy
            /// </summary>
            /// <returns>策略是否有效 / Whether the policy is valid</returns>
            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion
    }
}
