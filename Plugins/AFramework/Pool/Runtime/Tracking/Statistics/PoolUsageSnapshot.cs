// ==========================================================
// 文件名：PoolUsageSnapshot.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System
// 功能: 对象池使用快照
// ==========================================================

using System;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池使用快照
    /// Pool Usage Snapshot
    /// </summary>
    /// <remarks>
    /// 记录特定时间点的池使用情况，用于趋势分析和容量规划
    /// Records pool usage at a specific point in time for trend analysis and capacity planning
    /// </remarks>
    public readonly struct PoolUsageSnapshot
    {
        #region 属性 Properties

        /// <summary>
        /// 快照时间
        /// Snapshot time
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// 活跃对象数
        /// Active object count
        /// </summary>
        public readonly int ActiveCount;

        /// <summary>
        /// 空闲对象数
        /// Idle object count
        /// </summary>
        public readonly int IdleCount;

        /// <summary>
        /// 总对象数
        /// Total object count
        /// </summary>
        public readonly int TotalCount;

        /// <summary>
        /// 使用率（0.0 - 1.0）
        /// Usage rate (0.0 - 1.0)
        /// </summary>
        public readonly double UsageRate;

        /// <summary>
        /// 命中率（0.0 - 1.0）
        /// Hit rate (0.0 - 1.0)
        /// </summary>
        public readonly double HitRate;

        /// <summary>
        /// 估算内存占用（字节）
        /// Estimated memory usage (bytes)
        /// </summary>
        public readonly long MemoryUsage;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化使用快照
        /// Initialize usage snapshot
        /// </summary>
        public PoolUsageSnapshot(
            int activeCount,
            int idleCount,
            double hitRate,
            long memoryUsage)
        {
            Timestamp = DateTime.UtcNow;
            ActiveCount = activeCount;
            IdleCount = idleCount;
            TotalCount = activeCount + idleCount;
            UsageRate = TotalCount > 0 ? (double)activeCount / TotalCount : 0.0;
            HitRate = hitRate;
            MemoryUsage = memoryUsage;
        }

        /// <summary>
        /// 从统计信息创建快照
        /// Create snapshot from statistics
        /// </summary>
        public PoolUsageSnapshot(IPoolStatistics statistics)
        {
            Timestamp = DateTime.UtcNow;
            ActiveCount = statistics.CurrentActive;
            IdleCount = statistics.CurrentIdle;
            TotalCount = statistics.CurrentTotal;
            UsageRate = TotalCount > 0 ? (double)ActiveCount / TotalCount : 0.0;
            HitRate = statistics.HitRate;
            MemoryUsage = statistics.EstimatedMemoryUsage;
        }

        #endregion

        #region 方法 Methods

        /// <summary>
        /// 转换为字符串表示
        /// Convert to string representation
        /// </summary>
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] Active: {ActiveCount}, Idle: {IdleCount}, " +
                   $"Usage: {UsageRate:P2}, HitRate: {HitRate:P2}, Memory: {MemoryUsage / 1024.0:F2} KB";
        }

        #endregion
    }
}
