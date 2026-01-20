// ==========================================================
// 文件名：PoolWarmupComponent.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池预热组件，负责池的预热和容量管理
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池预热组件
    /// Object Pool Warmup Component
    /// 
    /// <para>负责对象池的预热和容量管理</para>
    /// <para>Responsible for object pool warmup and capacity management</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    public class PoolWarmupComponent<T> : IPoolComponent
    {
        #region Fields

        private readonly IPoolCreationPolicy<T> _creationPolicy;
        private readonly IPoolCapacityPolicy _capacityPolicy;
        private readonly Action<T> _addToPoolAction;
        private bool _isEnabled;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name => "Warmup";

        /// <inheritdoc />
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="creationPolicy">创建策略 / Creation policy</param>
        /// <param name="capacityPolicy">容量策略 / Capacity policy</param>
        /// <param name="addToPoolAction">添加到池的操作 / Add to pool action</param>
        public PoolWarmupComponent(
            IPoolCreationPolicy<T> creationPolicy,
            IPoolCapacityPolicy capacityPolicy,
            Action<T> addToPoolAction)
        {
            _creationPolicy = creationPolicy ?? throw new ArgumentNullException(nameof(creationPolicy));
            _capacityPolicy = capacityPolicy;
            _addToPoolAction = addToPoolAction ?? throw new ArgumentNullException(nameof(addToPoolAction));
            _isEnabled = true;
        }

        #endregion

        #region IPoolComponent Implementation

        /// <inheritdoc />
        public void Initialize()
        {
            if (!_isEnabled || _capacityPolicy == null) return;

            // 执行初始预热
            // Perform initial warmup
            int initialCapacity = _capacityPolicy.GetInitialCapacity();
            if (initialCapacity > 0)
            {
                Warmup(initialCapacity);
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            // 预热组件无需重置状态
            // Warmup component doesn't need to reset state
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // 预热组件无需释放资源
            // Warmup component doesn't need to release resources
        }

        #endregion

        #region Warmup Methods

        /// <summary>
        /// 预热对象池
        /// Warmup the object pool
        /// </summary>
        /// <param name="count">预热数量 / Warmup count</param>
        public void Warmup(int count)
        {
            if (!_isEnabled || count <= 0) return;

            for (int i = 0; i < count; i++)
            {
                T obj = _creationPolicy.Create();
                if (obj != null)
                {
                    _addToPoolAction(obj);
                }
            }
        }

        /// <summary>
        /// 检查是否可以归还对象
        /// Check if object can be returned
        /// </summary>
        /// <param name="currentCount">当前数量 / Current count</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>是否可以归还 / Whether can return</returns>
        public bool CanReturn(int currentCount, int maxCapacity)
        {
            if (!_isEnabled || _capacityPolicy == null) return true;
            return _capacityPolicy.CanReturn(currentCount, maxCapacity);
        }

        #endregion
    }
}
