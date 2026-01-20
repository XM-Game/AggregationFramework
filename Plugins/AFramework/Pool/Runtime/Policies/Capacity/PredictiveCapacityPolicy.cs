// ==========================================================
// 文件名：PredictiveCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 预测容量策略，基于历史数据预测未来容量需求
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool
{
    /// <summary>
    /// 预测容量策略
    /// Predictive Capacity Policy
    /// 
    /// <para>基于历史数据预测未来容量需求</para>
    /// <para>Predicts future capacity needs based on historical data</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：策略模式 + 预测算法
    /// 使用场景：
    /// - 有明显使用模式的场景
    /// - 需要提前准备资源
    /// - 减少临时创建开销
    /// 
    /// 预测算法：
    /// - 滑动窗口统计
    /// - 移动平均
    /// - 趋势分析
    /// </remarks>
    public class PredictiveCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

        private readonly int _minCapacity;
        private readonly int _maxCapacity;
        private readonly Queue<int> _usageHistory;
        private readonly int _historySize;
        private readonly float _predictionFactor;

        #endregion

        #region Properties

        /// <inheritdoc />
        public int MinCapacity => _minCapacity;

        /// <inheritdoc />
        public int MaxCapacity => _maxCapacity;

        /// <inheritdoc />
        public int InitialCapacity => _minCapacity;

        /// <summary>
        /// 预测容量
        /// Predicted capacity
        /// </summary>
        public int PredictedCapacity { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="minCapacity">最小容量 / Minimum capacity</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity (-1 表示无限制)</param>
        /// <param name="historySize">历史记录大小 / History size (default: 10)</param>
        /// <param name="predictionFactor">预测因子 / Prediction factor (default: 1.2)</param>
        public PredictiveCapacityPolicy(
            int minCapacity = 10,
            int maxCapacity = -1,
            int historySize = 10,
            float predictionFactor = 1.2f)
            : base("PredictiveCapacity")
        {
            _minCapacity = Math.Max(0, minCapacity);
            _maxCapacity = maxCapacity;
            _historySize = Math.Max(1, historySize);
            _predictionFactor = Math.Max(1.0f, predictionFactor);
            _usageHistory = new Queue<int>(_historySize);
            PredictedCapacity = _minCapacity;
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            // 记录使用情况
            // Record usage
            RecordUsage(currentActive);

            // 更新预测容量
            // Update predicted capacity
            UpdatePrediction();

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
            
            // 如果当前总数超过预测容量的 1.5 倍
            // If current total exceeds 1.5x predicted capacity
            return currentTotal > (int)(PredictedCapacity * 1.5f);
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            // 收缩到预测容量
            // Shrink to predicted capacity
            int targetTotal = Math.Max(PredictedCapacity, _minCapacity);
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

        #region Private Methods

        /// <summary>
        /// 记录使用情况
        /// Record usage
        /// </summary>
        private void RecordUsage(int activeCount)
        {
            _usageHistory.Enqueue(activeCount);

            // 保持历史记录大小
            // Maintain history size
            while (_usageHistory.Count > _historySize)
            {
                _usageHistory.Dequeue();
            }
        }

        /// <summary>
        /// 更新预测容量
        /// Update predicted capacity
        /// </summary>
        private void UpdatePrediction()
        {
            if (_usageHistory.Count == 0)
            {
                PredictedCapacity = _minCapacity;
                return;
            }

            // 计算移动平均
            // Calculate moving average
            float average = (float)_usageHistory.Average();

            // 计算最大值
            // Calculate maximum
            int max = _usageHistory.Max();

            // 预测容量 = (平均值 + 最大值) / 2 * 预测因子
            // Predicted capacity = (average + max) / 2 * prediction factor
            float predicted = ((average + max) / 2.0f) * _predictionFactor;

            // 应用边界
            // Apply boundaries
            int predictedInt = (int)predicted;
            predictedInt = Math.Max(predictedInt, _minCapacity);
            
            if (_maxCapacity > 0)
            {
                predictedInt = Math.Min(predictedInt, _maxCapacity);
            }

            PredictedCapacity = predictedInt;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 重置历史记录
        /// Reset history
        /// </summary>
        public void ResetHistory()
        {
            _usageHistory.Clear();
            PredictedCapacity = _minCapacity;
        }

        /// <summary>
        /// 获取使用历史
        /// Get usage history
        /// </summary>
        /// <returns>使用历史数组 / Usage history array</returns>
        public int[] GetUsageHistory()
        {
            return _usageHistory.ToArray();
        }

        #endregion
    }
}
