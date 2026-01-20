// ==========================================================
// 文件名：StackObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 高性能栈对象池，零 GC 分配，适合热路径使用
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 高性能栈对象池
    /// High-Performance Stack Object Pool
    /// 
    /// <para>基于数组实现的零 GC 分配对象池，适合热路径使用</para>
    /// <para>Zero-GC allocation object pool based on array, suitable for hot paths</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// <para><strong>设计模式 Design Patterns:</strong></para>
    /// <list type="bullet">
    /// <item><description>对象池模式 (Object Pool Pattern) - 重用对象减少 GC</description></item>
    /// <item><description>数据局部性模式 (Data Locality Pattern) - 数组连续存储提高缓存命中率</description></item>
    /// </list>
    /// 
    /// <para><strong>特性 Features:</strong></para>
    /// <list type="bullet">
    /// <item><description>基于固定大小数组实现，LIFO 顺序</description></item>
    /// <item><description>零 GC 分配（热路径）</description></item>
    /// <item><description>极致性能优化，适合高频调用</description></item>
    /// <item><description>固定容量，不支持动态扩容</description></item>
    /// </list>
    /// 
    /// <para><strong>性能 Performance:</strong></para>
    /// <list type="bullet">
    /// <item><description>Get/Return: O(1) 时间复杂度</description></item>
    /// <item><description>内存占用: 固定（预分配数组）</description></item>
    /// <item><description>GC 压力: 零（无额外分配）</description></item>
    /// <item><description>缓存友好: 是（数组连续存储）</description></item>
    /// </list>
    /// 
    /// <para><strong>使用场景 Use Cases:</strong></para>
    /// <list type="bullet">
    /// <item><description>热路径代码（每帧调用）</description></item>
    /// <item><description>性能关键场景（粒子系统、子弹池）</description></item>
    /// <item><description>容量需求稳定的场景</description></item>
    /// </list>
    /// 
    /// <para><strong>限制 Limitations:</strong></para>
    /// <list type="bullet">
    /// <item><description>固定容量，不支持动态扩容</description></item>
    /// <item><description>不适合容量需求波动大的场景</description></item>
    /// <item><description>不支持容量策略（capacityPolicy 参数被忽略）</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para><strong>基础使用示例 Basic Usage:</strong></para>
    /// <code>
    /// // 创建高性能栈对象池
    /// var pool = new StackObjectPool&lt;Bullet&gt;(
    ///     createFunc: () => new Bullet(),
    ///     maxCapacity: 1000,
    ///     name: "BulletPool"
    /// );
    /// 
    /// // 预热池
    /// pool.Warmup(100);
    /// 
    /// // 在 Update 中使用（零 GC）
    /// void Update()
    /// {
    ///     var bullet = pool.Get();
    ///     bullet.Fire();
    ///     // ... 使用后归还
    ///     pool.Return(bullet);
    /// }
    /// </code>
    /// 
    /// <para><strong>性能关键场景示例 Performance-Critical Scenario:</strong></para>
    /// <code>
    /// // 粒子系统使用高性能池
    /// var particlePool = new StackObjectPool&lt;Particle&gt;(
    ///     createFunc: () => new Particle(),
    ///     maxCapacity: 10000,
    ///     name: "ParticlePool"
    /// );
    /// 
    /// // 预热到最大容量
    /// particlePool.Warmup(10000);
    /// 
    /// // 每帧发射大量粒子（零 GC）
    /// void EmitParticles(int count)
    /// {
    ///     for (int i = 0; i &lt; count; i++)
    ///     {
    ///         var particle = particlePool.Get();
    ///         particle.Emit();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class StackObjectPool<T> : ObjectPoolBase<T>
    {
        #region Fields

        /// <summary>
        /// 对象数组
        /// Object array
        /// </summary>
        private readonly T[] _items;

        /// <summary>
        /// 当前栈顶索引
        /// Current stack top index
        /// </summary>
        private int _count;

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
        public override int TotalCount => _count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _count;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="cleanupPolicy">清理策略（可选）/ Cleanup policy (optional)</param>
        /// <param name="name">池名称（可选）/ Pool name (optional)</param>
        /// <exception cref="ArgumentNullException">creationPolicy 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxCapacity 小于等于 0</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 预分配 T[maxCapacity] 数组</description></item>
        /// <item><description>GC 压力: 仅构造时分配一次</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>容量策略参数被忽略，因为此池使用固定容量</para>
        /// <para>Capacity policy parameter is ignored as this pool uses fixed capacity</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pool = new StackObjectPool&lt;Bullet&gt;(
        ///     creationPolicy: new DefaultCreationPolicy&lt;Bullet&gt;(),
        ///     maxCapacity: 1000,
        ///     cleanupPolicy: new ResetCleanupPolicy&lt;Bullet&gt;(),
        ///     name: "BulletPool"
        /// );
        /// </code>
        /// </example>
        public StackObjectPool(
            IPoolCreationPolicy<T> creationPolicy,
            int maxCapacity = 100,
            IPoolCleanupPolicy<T> cleanupPolicy = null,
            string name = null)
            : base(creationPolicy, cleanupPolicy, null, name)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than 0.");
            }

            _maxCapacity = maxCapacity;
            _items = new T[maxCapacity];
            _count = 0;
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
        /// var pool = new StackObjectPool&lt;Bullet&gt;(
        ///     createFunc: () => new Bullet(),
        ///     maxCapacity: 1000,
        ///     name: "BulletPool"
        /// );
        /// </code>
        /// </example>
        public StackObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, null, name)
        {
        }

        #endregion

        #region Initialization

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 无</description></item>
        /// </list>
        /// </remarks>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="PoolCreationException">对象创建失败</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为预热数量</description></item>
        /// <item><description>内存分配: 创建 n 个对象（零 GC 热路径）</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>预热数量会被限制在最大容量内</para>
        /// <para>Warmup count will be capped at maximum capacity</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 预热池到最大容量，避免运行时分配
        /// pool.Warmup(1000);
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

                for (int i = _count; i < targetCount; i++)
                {
                    T obj = CreateObject();
                    _items[_count++] = obj;
                }
            }
        }

        #endregion

        #region Core Operations - Get/Return Implementation

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(1)</description></item>
        /// <item><description>内存分配: 零（从数组中获取）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>清空数组引用以避免内存泄漏</para>
        /// <para>Clears array reference to avoid memory leaks</para>
        /// </remarks>
        protected override bool TryGetFromPool(out T obj)
        {
            if (_count > 0)
            {
                obj = _items[--_count];
                _items[_count] = default; // 清空引用，避免内存泄漏 / Clear reference to avoid memory leak
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
        /// <item><description>内存分配: 零（存入数组）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// </remarks>
        protected override bool TryReturnToPool(T obj)
        {
            // 检查容量限制
            // Check capacity limit
            if (_count >= _maxCapacity)
            {
                return false;
            }

            _items[_count++] = obj;
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
        /// <para>清空所有数组引用以避免内存泄漏</para>
        /// <para>Clears all array references to avoid memory leaks</para>
        /// </remarks>
        protected override void OnClear()
        {
            for (int i = 0; i < _count; i++)
            {
                T obj = _items[i];
                DestroyObject(obj);
                _items[i] = default; // 清空引用 / Clear reference
                _statistics.IncrementTotalDestroyed();
            }

            _count = 0;
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为数组容量</description></item>
        /// <item><description>内存分配: 无（仅释放资源）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>使用 Array.Clear 清空整个数组以避免内存泄漏</para>
        /// <para>Uses Array.Clear to clear entire array to avoid memory leaks</para>
        /// </remarks>
        protected override void OnDispose()
        {
            base.OnDispose();

            // 清空数组引用
            // Clear array references
            Array.Clear(_items, 0, _items.Length);
            _count = 0;
        }

        #endregion

        #region Management Operations

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
        /// // 将池缩减到 100 个对象
        /// int removed = pool.Shrink(100);
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

            int removedCount = 0;

            lock (_lock)
            {
                while (_count > targetCount)
                {
                    T obj = _items[--_count];
                    DestroyObject(obj);
                    _items[_count] = default; // 清空引用 / Clear reference
                    _statistics.IncrementTotalDestroyed();
                    removedCount++;
                }
            }

            return removedCount;
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

            /// <inheritdoc />
            public string Name => "FuncCreationPolicy";

            /// <inheritdoc />
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

            /// <inheritdoc />
            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion
    }
}
