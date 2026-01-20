// ==========================================================
// 文件名：DynamicCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 动态容量策略，支持自动扩容和收缩
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 动态容量策略
    /// Dynamic Capacity Policy
    /// 
    /// <para>支持自动扩容和收缩的动态容量策略</para>
    /// <para>Dynamic capacity policy with automatic expansion and shrinking</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 负载波动场景
    /// - 需要自动调整容量
    /// - 优化内存使用
    /// - 适应不同负载
    /// 
    /// 特性：
    /// - 自动扩容（按倍数增长）
    /// - 自动收缩（空闲率过高时）
    /// - 智能容量管理
    /// 
    /// 策略参数：
    /// - 初始容量：initialCapacity
    /// - 最小容量：minCapacity
    /// - 最大容量：maxCapacity
    /// - 扩容因子：growthFactor
    /// - 收缩阈值：shrinkThreshold
    /// </remarks>
    public class DynamicCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

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
        /// 收缩阈值（空闲率）
        /// Shrink threshold (idle rate)
        /// </summary>
        private readonly float _shrinkThreshold;

        /// <summary>
        /// 扩容阈值（使用率）
        /// Expansion threshold (utilization rate)
        /// </summary>
        private readonly float _expansionThreshold;

        #endregion

        #region Properties

        /// <summary>
        /// 初始容量
        /// Initial capacity
        /// </summary>
        public int InitialCapacity => _initialCapacity;

        /// <summary>
        /// 最小容量
        /// Minimum capacity
        /// </summary>
        public int MinCapacity => _minCapacity;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        public int MaxCapacity => _maxCapacity;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="initialCapacity">初始容量 / Initial capacity</param>
        /// <param name="minCapacity">最小容量 / Minimum capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="growthFactor">扩容因子（默认 2.0）/ Growth factor (default 2.0)</param>
        /// <param name="shrinkThreshold">收缩阈值（默认 0.5，即空闲率 50%）/ Shrink threshold (default 0.5, i.e. 50% idle rate)</param>
        /// <param name="expansionThreshold">扩容阈值（默认 0.9，即使用率 90%）/ Expansion threshold (default 0.9, i.e. 90% utilization rate)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public DynamicCapacityPolicy(
            int initialCapacity = 10,
            int minCapacity = 10,
            int maxCapacity = 1000,
            float growthFactor = 2.0f,
            float shrinkThreshold = 0.5f,
            float expansionThreshold = 0.9f,
            string name = null)
            : base(name ?? "DynamicCapacityPolicy")
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be greater than 0.");
            }

            if (minCapacity <= 0 || minCapacity > initialCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(minCapacity), "Min capacity must be greater than 0 and less than or equal to initial capacity.");
            }

            if (maxCapacity < initialCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity must be greater than or equal to initial capacity.");
            }

            if (growthFactor <= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(growthFactor), "Growth factor must be greater than 1.0.");
            }

            if (shrinkThreshold <= 0f || shrinkThreshold >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(shrinkThreshold), "Shrink threshold must be between 0 and 1.");
            }

            if (expansionThreshold <= 0f || expansionThreshold >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(expansionThreshold), "Expansion threshold must be between 0 and 1.");
            }

            _initialCapacity = initialCapacity;
            _minCapacity = minCapacity;
            _maxCapacity = maxCapacity;
            _growthFactor = growthFactor;
            _shrinkThreshold = shrinkThreshold;
            _expansionThreshold = expansionThreshold;
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            ThrowIfDisposed();

            // 动态容量策略：只要不超过最大容量就允许创建
            // Dynamic capacity policy: allow creation as long as not exceeding max capacity
            return currentTotal < _maxCapacity;
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 当空闲对象数量超过阈值且总数大于最小容量时，建议收缩
            // Suggest shrinking when available count exceeds threshold and total is greater than min capacity
            int totalCount = currentAvailable + currentActive;
            if (totalCount <= _minCapacity)
            {
                return false;
            }

            // 计算空闲率
            // Calculate idle rate
            float idleRate = totalCount > 0 ? (float)currentAvailable / totalCount : 0f;
            return idleRate > _shrinkThreshold;
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 收缩到最小容量和当前活跃数量的较大值
            // Shrink to the greater of min capacity and current active count
            int targetTotal = Math.Max(_minCapacity, currentActive);
            
            // 返回应保留的可用对象数量
            // Return the number of available objects to keep
            return Math.Max(0, targetTotal - currentActive);
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证容量策略配置
            // Validate capacity policy configuration
            if (_minCapacity < 0)
            {
                return false;
            }

            if (_maxCapacity < _minCapacity)
            {
                return false;
            }

            if (_initialCapacity < _minCapacity || _initialCapacity > _maxCapacity)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
