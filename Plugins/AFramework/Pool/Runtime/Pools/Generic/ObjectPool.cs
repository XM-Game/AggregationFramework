// ==========================================================
// 文件名：ObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 标准泛型对象池实现，基于 Stack<T>，平衡性能与功能
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 标准泛型对象池
    /// Standard Generic Object Pool
    /// 
    /// <para>基于 Stack<T> 实现的标准对象池，平衡性能与功能</para>
    /// <para>Standard object pool based on Stack<T>, balancing performance and functionality</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// <para><strong>设计模式 Design Patterns:</strong></para>
    /// <list type="bullet">
    /// <item><description>对象池模式 (Object Pool Pattern) - 重用对象减少 GC</description></item>
    /// <item><description>策略模式 (Strategy Pattern) - 可配置的创建/清理/容量策略</description></item>
    /// <item><description>模板方法模式 (Template Method Pattern) - 定义池操作骨架</description></item>
    /// </list>
    /// 
    /// <para><strong>特性 Features:</strong></para>
    /// <list type="bullet">
    /// <item><description>基于 Stack&lt;T&gt; 实现，LIFO 顺序</description></item>
    /// <item><description>支持所有策略（创建/清理/容量）</description></item>
    /// <item><description>线程安全（使用 lock）</description></item>
    /// <item><description>适合大多数场景</description></item>
    /// </list>
    /// 
    /// <para><strong>性能 Performance:</strong></para>
    /// <list type="bullet">
    /// <item><description>Get/Return: O(1) 时间复杂度</description></item>
    /// <item><description>内存占用: 低（仅 Stack 开销）</description></item>
    /// <item><description>GC 压力: 低（对象重用）</description></item>
    /// <item><description>线程安全: 是（lock 同步）</description></item>
    /// </list>
    /// 
    /// <para><strong>使用场景 Use Cases:</strong></para>
    /// <list type="bullet">
    /// <item><description>频繁创建销毁的对象（子弹、特效、敌人）</description></item>
    /// <item><description>需要控制内存占用的场景</description></item>
    /// <item><description>需要统计和诊断功能的场景</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <para><strong>基础使用示例 Basic Usage:</strong></para>
    /// <code>
    /// // 创建对象池
    /// var pool = new ObjectPool&lt;Enemy&gt;(
    ///     createFunc: () => new Enemy(),
    ///     maxCapacity: 100,
    ///     name: "EnemyPool"
    /// );
    /// 
    /// // 获取对象
    /// var enemy = pool.Get();
    /// enemy.Initialize();
    /// 
    /// // 使用对象
    /// enemy.Attack();
    /// 
    /// // 归还对象
    /// pool.Return(enemy);
    /// 
    /// // 清理池
    /// pool.Dispose();
    /// </code>
    /// 
    /// <para><strong>使用策略示例 Using Policies:</strong></para>
    /// <code>
    /// // 创建带策略的对象池
    /// var creationPolicy = new DefaultCreationPolicy&lt;Enemy&gt;();
    /// var cleanupPolicy = new ResetCleanupPolicy&lt;Enemy&gt;();
    /// var capacityPolicy = new DynamicCapacityPolicy(minCapacity: 10, maxCapacity: 100);
    /// 
    /// var pool = new ObjectPool&lt;Enemy&gt;(
    ///     creationPolicy: creationPolicy,
    ///     maxCapacity: 100,
    ///     cleanupPolicy: cleanupPolicy,
    ///     capacityPolicy: capacityPolicy,
    ///     name: "EnemyPool"
    /// );
    /// 
    /// // 预热池
    /// pool.Warmup(20);
    /// 
    /// // 使用 using 语句自动归还
    /// using (var rental = pool.Rent())
    /// {
    ///     var enemy = rental.Value;
    ///     enemy.Attack();
    /// } // 自动归还
    /// </code>
    /// 
    /// <para><strong>批量操作示例 Batch Operations:</strong></para>
    /// <code>
    /// // 批量获取
    /// var enemies = pool.GetMany(10);
    /// foreach (var enemy in enemies)
    /// {
    ///     enemy.Attack();
    /// }
    /// 
    /// // 批量归还
    /// int returnedCount = pool.ReturnMany(enemies);
    /// </code>
    /// </example>
    public class ObjectPool<T> : ObjectPoolBase<T>
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
        /// <item><description>内存分配: Stack&lt;T&gt; 预分配 maxCapacity 容量</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pool = new ObjectPool&lt;Enemy&gt;(
        ///     creationPolicy: new DefaultCreationPolicy&lt;Enemy&gt;(),
        ///     maxCapacity: 100,
        ///     cleanupPolicy: new ResetCleanupPolicy&lt;Enemy&gt;(),
        ///     name: "EnemyPool"
        /// );
        /// </code>
        /// </example>
        public ObjectPool(
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
        /// <param name="name">池名称（可选）/ Pool name (optional)</param>
        /// <exception cref="ArgumentNullException">createFunc 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxCapacity 小于等于 0</exception>
        /// <remarks>
        /// <para>简化构造函数，使用函数委托作为创建策略</para>
        /// <para>Simplified constructor using function delegate as creation policy</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var pool = new ObjectPool&lt;Enemy&gt;(
        ///     createFunc: () => new Enemy(),
        ///     maxCapacity: 100,
        ///     name: "EnemyPool"
        /// );
        /// </code>
        /// </example>
        public ObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            string name = null)
            : this(new FuncCreationPolicy<T>(createFunc), maxCapacity, null, null, name)
        {
        }

        #endregion

        #region Initialization

        /// <inheritdoc />
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为预热数量</description></item>
        /// <item><description>内存分配: 创建 n 个对象</description></item>
        /// </list>
        /// </remarks>
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
        /// <exception cref="ObjectDisposedException">池已被销毁</exception>
        /// <exception cref="PoolCreationException">对象创建失败</exception>
        /// <remarks>
        /// <para><strong>性能 Performance:</strong></para>
        /// <list type="bullet">
        /// <item><description>时间复杂度: O(n)，n 为预热数量</description></item>
        /// <item><description>内存分配: 创建 n 个对象</description></item>
        /// <item><description>线程安全: 是</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // 预热池，预创建 20 个对象
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
                    _objects.Push(obj);
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
        /// <item><description>内存分配: 无（从栈中弹出）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// </remarks>
        protected override bool TryGetFromPool(out T obj)
        {
            if (_objects.Count > 0)
            {
                obj = _objects.Pop();
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
        /// <item><description>内存分配: 无（压入栈）</description></item>
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

            _objects.Push(obj);
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
                T obj = _objects.Pop();
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
        /// <item><description>时间复杂度: O(n)，n 为池中对象数量</description></item>
        /// <item><description>内存分配: 无（仅释放资源）</description></item>
        /// <item><description>线程安全: 由调用方保证（在 lock 内调用）</description></item>
        /// </list>
        /// <para><strong>注意 Note:</strong></para>
        /// <para>此方法会清空栈并调用基类的 OnDispose，确保所有资源被正确释放</para>
        /// <para>This method clears the stack and calls base OnDispose to ensure all resources are properly released</para>
        /// </remarks>
        protected override void OnDispose()
        {
            base.OnDispose();
            _objects.Clear();
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
                int removedCount = 0;
                while (_objects.Count > targetCount)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statistics.IncrementTotalDestroyed();
                    removedCount++;
                }
                return removedCount;
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
        /// <example>
        /// <code>
        /// // 内部使用示例（用户通常不直接使用）
        /// var policy = new FuncCreationPolicy&lt;Enemy&gt;(() => new Enemy());
        /// var enemy = policy.Create();
        /// </code>
        /// </example>
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
