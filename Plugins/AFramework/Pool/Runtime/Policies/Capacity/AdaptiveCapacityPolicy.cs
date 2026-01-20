// ==========================================================
// 文件名：AdaptiveCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 自适应容量策略，根据使用情况动态调整容量
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 自适应容量策略
    /// Adaptive Capacity Policy
    /// 
    /// <para>根据使用情况动态调整池容量</para>
    /// <para>Dynamically adjusts pool capacity based on usage patterns</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：策略模式 + 自适应算法
    /// 使用场景：
    /// - 负载波动较大的场景
    /// - 需要自动优化内存使用
    /// - 长时间运行的应用
    /// 
    /// 自适应策略：
    /// - 监控峰值使用量
    /// - 根据历史数据预测容量
    /// - 自动扩容和缩容
    /// </remarks>
    public class AdaptiveCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

        private int _minCapacity;
        private int _maxCapacity;
        private int _currentCapacity;
        private int _peakUsage;
        private readonly float _growthFactor;
        private readonly float _shrinkThreshold;

        #endregion

        #region Properties

        /// <inheritdoc />
        public int MinCapacity => _minCapacity;

        /// <inheritdoc />
        public int MaxCapacity => _maxCapacity;

        /// <inheritdoc />
        public int InitialCapacity => _minCapacity;

        /// <summary>
        /// 当前推荐容量
        /// Current recommended capacity
        /// </summary>
        public int CurrentCapacity => _currentCapacity;

        /// <summary>
        /// 峰值使用量
        /// Peak usage
        /// </summary>
        public int PeakUsage => _peakUsage;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="minCapacity">最小容量 / Minimum capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity (-1 表示无限制)</param>
        /// <param name="growthFactor">增长因子 / Growth factor (default: 1.5)</param>
        /// <param name="shrinkThreshold">收缩阈值 / Shrink threshold (default: 0.3)</param>
        public AdaptiveCapacityPolicy(
            int minCapacity = 10,
            int maxCapacity = -1,
            float growthFactor = 1.5f,
            float shrinkThreshold = 0.3f)
            : base("AdaptiveCapacity")
        {
            _minCapacity = Math.Max(0, minCapacity);
            _maxCapacity = maxCapacity;
            _currentCapacity = _minCapacity;
            _peakUsage = 0;
            _growthFactor = Math.Max(1.1f, growthFactor);
            _shrinkThreshold = Math.Clamp(shrinkThreshold, 0.1f, 0.9f);
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            // 更新峰值使用量
            // Update peak usage
            if (currentActive > _peakUsage)
            {
                _peakUsage = currentActive;
                
                // 根据峰值调整推荐容量
                // Adjust recommended capacity based on peak
                _currentCapacity = (int)(_peakUsage * _growthFactor);
                
                if (_maxCapacity > 0)
                {
                    _currentCapacity = Math.Min(_currentCapacity, _maxCapacity);
                }
                
                _currentCapacity = Math.Max(_currentCapacity, _minCapacity);
            }

            // 判断是否可以创建
            // Determine if creation is allowed
            if (_maxCapacity < 0)
            {
                return true;
            }

            return currentTotal < _maxCapacity;
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            int currentTotal = currentAvailable + currentActive;
            
            // 如果当前总数超过推荐容量，且空闲对象占比过高
            // If current total exceeds recommended capacity and idle ratio is high
            if (currentTotal > _currentCapacity)
            {
                float idleRatio = currentTotal > 0 ? (float)currentAvailable / currentTotal : 0;
                return idleRatio > (1.0f - _shrinkThreshold);
            }

            return false;
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            // 收缩到推荐容量
            // Shrink to recommended capacity
            int targetTotal = Math.Max(_currentCapacity, _minCapacity);
            int targetAvailable = Math.Max(0, targetTotal - currentActive);
            
            return targetAvailable;
        }

        /// <inheritdoc />
        public bool Validate()
        {
            if (_minCapacity < 0)
            {
                return false;
            }

            if (_maxCapacity >= 0 && _maxCapacity < _minCapacity)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 重置统计信息
        /// Reset statistics
        /// </summary>
        public void ResetStatistics()
        {
            _peakUsage = 0;
            _currentCapacity = _minCapacity;
        }

        /// <summary>
        /// 设置最小容量
        /// Set minimum capacity
        /// </summary>
        /// <param name="minCapacity">最小容量 / Minimum capacity</param>
        public void SetMinCapacity(int minCapacity)
        {
            _minCapacity = Math.Max(0, minCapacity);
            _currentCapacity = Math.Max(_currentCapacity, _minCapacity);
        }

        /// <summary>
        /// 设置最大容量
        /// Set maximum capacity
        /// </summary>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        public void SetMaxCapacity(int maxCapacity)
        {
            _maxCapacity = maxCapacity;
            
            if (_maxCapacity > 0)
            {
                _currentCapacity = Math.Min(_currentCapacity, _maxCapacity);
            }
        }

        #endregion
    }
}
