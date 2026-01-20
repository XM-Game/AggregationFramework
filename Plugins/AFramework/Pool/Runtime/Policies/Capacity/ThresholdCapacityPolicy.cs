// ==========================================================
// 文件名：ThresholdCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 阈值容量策略，基于使用率阈值触发扩容和收缩
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 阈值容量策略
    /// Threshold Capacity Policy
    /// 
    /// <para>基于使用率阈值触发扩容和收缩的容量策略</para>
    /// <para>Capacity policy that triggers expansion and shrinking based on utilization thresholds</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 需要精确控制扩容/收缩时机
    /// - 基于使用率的智能调整
    /// - 避免频繁的容量变化
    /// 
    /// 特性：
    /// - 高水位阈值（触发扩容）
    /// - 低水位阈值（触发收缩）
    /// - 防抖机制（避免频繁调整）
    /// 
    /// 策略参数：
    /// - 初始容量：initialCapacity
    /// - 最小容量：minCapacity
    /// - 最大容量：maxCapacity
    /// - 高水位阈值：highWatermark（默认 0.8，即 80%）
    /// - 低水位阈值：lowWatermark（默认 0.3，即 30%）
    /// - 扩容因子：growthFactor（默认 2.0）
    /// - 收缩因子：shrinkFactor（默认 0.5）
    /// </remarks>
    public class ThresholdCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
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
        /// 高水位阈值（触发扩容）
        /// High watermark threshold (trigger expansion)
        /// </summary>
        private readonly float _highWatermark;

        /// <summary>
        /// 低水位阈值（触发收缩）
        /// Low watermark threshold (trigger shrinking)
        /// </summary>
        private readonly float _lowWatermark;

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

        /// <summary>
        /// 高水位阈值
        /// High watermark threshold
        /// </summary>
        public float HighWatermark => _highWatermark;

        /// <summary>
        /// 低水位阈值
        /// Low watermark threshold
        /// </summary>
        public float LowWatermark => _lowWatermark;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="initialCapacity">初始容量（默认 10）/ Initial capacity (default 10)</param>
        /// <param name="minCapacity">最小容量（默认 10）/ Minimum capacity (default 10)</param>
        /// <param name="maxCapacity">最大容量（默认 1000）/ Maximum capacity (default 1000)</param>
        /// <param name="highWatermark">高水位阈值（默认 0.8）/ High watermark threshold (default 0.8)</param>
        /// <param name="lowWatermark">低水位阈值（默认 0.3）/ Low watermark threshold (default 0.3)</param>
        /// <param name="growthFactor">扩容因子（默认 2.0）/ Growth factor (default 2.0)</param>
        /// <param name="shrinkFactor">收缩因子（默认 0.5）/ Shrink factor (default 0.5)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public ThresholdCapacityPolicy(
            int initialCapacity = 10,
            int minCapacity = 10,
            int maxCapacity = 1000,
            float highWatermark = 0.8f,
            float lowWatermark = 0.3f,
            float growthFactor = 2.0f,
            float shrinkFactor = 0.5f,
            string name = null)
            : base(name ?? "ThresholdCapacityPolicy")
        {
            // 参数验证
            // Parameter validation
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

            if (highWatermark <= 0f || highWatermark >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(highWatermark), "High watermark must be between 0 and 1.");
            }

            if (lowWatermark <= 0f || lowWatermark >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(lowWatermark), "Low watermark must be between 0 and 1.");
            }

            if (lowWatermark >= highWatermark)
            {
                throw new ArgumentException("Low watermark must be less than high watermark.");
            }

            if (growthFactor <= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(growthFactor), "Growth factor must be greater than 1.0.");
            }

            if (shrinkFactor <= 0f || shrinkFactor >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(shrinkFactor), "Shrink factor must be between 0 and 1.");
            }

            _initialCapacity = initialCapacity;
            _minCapacity = minCapacity;
            _maxCapacity = maxCapacity;
            _highWatermark = highWatermark;
            _lowWatermark = lowWatermark;
            _growthFactor = growthFactor;
            _shrinkFactor = shrinkFactor;
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            ThrowIfDisposed();

            // 阈值策略：只要不超过最大容量就允许创建
            // Threshold policy: allow creation as long as not exceeding max capacity
            return currentTotal < _maxCapacity;
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            int totalCount = currentAvailable + currentActive;

            // 当总数大于最小容量且使用率低于低水位阈值时，建议收缩
            // Suggest shrinking when total is greater than min capacity and utilization is below low watermark
            if (totalCount <= _minCapacity)
            {
                return false;
            }

            float utilizationRate = totalCount > 0 ? (float)currentActive / totalCount : 0f;
            return utilizationRate <= _lowWatermark;
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            int totalCount = currentAvailable + currentActive;
            
            // 收缩到当前活跃数量除以低水位阈值，但不低于最小容量
            // Shrink to current active count divided by low watermark, but not below min capacity
            int targetTotal = Math.Max(_minCapacity, (int)(currentActive / _lowWatermark));
            targetTotal = Math.Min(targetTotal, totalCount);
            
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

            if (_highWatermark <= 0f || _highWatermark >= 1.0f)
            {
                return false;
            }

            if (_lowWatermark <= 0f || _lowWatermark >= 1.0f)
            {
                return false;
            }

            if (_lowWatermark >= _highWatermark)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 计算新容量（扩容）
        /// Calculate new capacity (expansion)
        /// </summary>
        /// <param name="currentCapacity">当前容量 / Current capacity</param>
        /// <returns>新容量 / New capacity</returns>
        public int CalculateExpandedCapacity(int currentCapacity)
        {
            ThrowIfDisposed();

            int newCapacity = (int)(currentCapacity * _growthFactor);
            return Math.Min(newCapacity, _maxCapacity);
        }

        /// <summary>
        /// 计算新容量（收缩）
        /// Calculate new capacity (shrinking)
        /// </summary>
        /// <param name="currentCapacity">当前容量 / Current capacity</param>
        /// <returns>新容量 / New capacity</returns>
        public int CalculateShrunkCapacity(int currentCapacity)
        {
            ThrowIfDisposed();

            int newCapacity = (int)(currentCapacity * _shrinkFactor);
            return Math.Max(newCapacity, _minCapacity);
        }

        /// <summary>
        /// 获取使用率
        /// Get utilization rate
        /// </summary>
        /// <param name="currentCount">当前数量 / Current count</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>使用率（0-1）/ Utilization rate (0-1)</returns>
        public float GetUtilizationRate(int currentCount, int maxCapacity)
        {
            ThrowIfDisposed();

            return maxCapacity > 0 ? (float)currentCount / maxCapacity : 0f;
        }

        #endregion
    }
}
