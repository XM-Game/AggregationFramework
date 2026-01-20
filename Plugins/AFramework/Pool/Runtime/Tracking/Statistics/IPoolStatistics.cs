// ==========================================================
// 文件名：IPoolStatistics.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池统计信息接口
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池统计信息接口
    /// Pool Statistics Interface
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 单一职责原则：仅负责统计数据的读取
    /// - 接口隔离原则：统计功能独立，不影响池性能
    /// - 开闭原则：可扩展统计指标
    /// 
    /// 统计指标：
    /// - 对象计数：总创建数、总销毁数、当前活跃数、当前空闲数
    /// - 操作计数：获取次数、归还次数、命中次数、未命中次数
    /// - 性能指标：命中率、平均获取时间、平均归还时间
    /// - 容量指标：峰值使用量、内存占用
    /// 
    /// 使用场景：
    /// - 性能监控：实时监控池的使用情况
    /// - 容量规划：根据统计数据调整池容量
    /// - 问题诊断：分析池的异常行为
    /// </remarks>
    public interface IPoolStatistics
    {
        #region 对象计数 Object Counts

        /// <summary>
        /// 获取总创建对象数
        /// Total number of objects created
        /// </summary>
        long TotalCreated { get; }

        /// <summary>
        /// 获取总销毁对象数
        /// Total number of objects destroyed
        /// </summary>
        long TotalDestroyed { get; }

        /// <summary>
        /// 获取当前活跃对象数（已获取但未归还）
        /// Current number of active objects (acquired but not returned)
        /// </summary>
        int CurrentActive { get; }

        /// <summary>
        /// 获取当前空闲对象数（池中可用）
        /// Current number of idle objects (available in pool)
        /// </summary>
        int CurrentIdle { get; }

        /// <summary>
        /// 获取当前总对象数（活跃 + 空闲）
        /// Current total number of objects (active + idle)
        /// </summary>
        int CurrentTotal { get; }

        #endregion

        #region 操作计数 Operation Counts

        /// <summary>
        /// 获取总获取次数
        /// Total number of get operations
        /// </summary>
        long TotalGets { get; }

        /// <summary>
        /// 获取总归还次数
        /// Total number of return operations
        /// </summary>
        long TotalReturns { get; }

        /// <summary>
        /// 获取命中次数（从池中获取现有对象）
        /// Number of hits (getting existing object from pool)
        /// </summary>
        long Hits { get; }

        /// <summary>
        /// 获取未命中次数（需要创建新对象）
        /// Number of misses (need to create new object)
        /// </summary>
        long Misses { get; }

        #endregion

        #region 性能指标 Performance Metrics

        /// <summary>
        /// 获取命中率（0.0 - 1.0）
        /// Hit rate (0.0 - 1.0)
        /// </summary>
        double HitRate { get; }

        /// <summary>
        /// 获取未命中率（0.0 - 1.0）
        /// Miss rate (0.0 - 1.0)
        /// </summary>
        double MissRate { get; }

        /// <summary>
        /// 获取平均获取时间（毫秒）
        /// Average get time (milliseconds)
        /// </summary>
        double AverageGetTime { get; }

        /// <summary>
        /// 获取平均归还时间（毫秒）
        /// Average return time (milliseconds)
        /// </summary>
        double AverageReturnTime { get; }

        #endregion

        #region 容量指标 Capacity Metrics

        /// <summary>
        /// 获取峰值活跃对象数
        /// Peak number of active objects
        /// </summary>
        int PeakActive { get; }

        /// <summary>
        /// 获取峰值总对象数
        /// Peak total number of objects
        /// </summary>
        int PeakTotal { get; }

        /// <summary>
        /// 获取估算的内存占用（字节）
        /// Estimated memory usage (bytes)
        /// </summary>
        long EstimatedMemoryUsage { get; }

        #endregion

        #region 时间信息 Time Information

        /// <summary>
        /// 获取池创建时间
        /// Pool creation time
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// 获取池运行时长
        /// Pool uptime
        /// </summary>
        TimeSpan Uptime { get; }

        /// <summary>
        /// 获取最后一次获取操作时间
        /// Last get operation time
        /// </summary>
        DateTime? LastGetTime { get; }

        /// <summary>
        /// 获取最后一次归还操作时间
        /// Last return operation time
        /// </summary>
        DateTime? LastReturnTime { get; }

        #endregion

        #region 快照操作 Snapshot Operations

        /// <summary>
        /// 创建统计信息快照
        /// Create statistics snapshot
        /// </summary>
        /// <returns>统计信息快照 / Statistics snapshot</returns>
        PoolStatisticsSnapshot CreateSnapshot();

        /// <summary>
        /// 重置统计信息（保留当前对象计数）
        /// Reset statistics (keep current object counts)
        /// </summary>
        void Reset();

        #endregion
    }

    /// <summary>
    /// 对象池统计信息快照
    /// Pool Statistics Snapshot
    /// </summary>
    /// <remarks>
    /// 不可变的统计数据快照，用于历史记录和对比分析
    /// Immutable statistics snapshot for historical records and comparison analysis
    /// </remarks>
    public readonly struct PoolStatisticsSnapshot
    {
        #region 对象计数 Object Counts

        /// <summary>总创建对象数 / Total created</summary>
        public readonly long TotalCreated;

        /// <summary>总销毁对象数 / Total destroyed</summary>
        public readonly long TotalDestroyed;

        /// <summary>当前活跃对象数 / Current active</summary>
        public readonly int CurrentActive;

        /// <summary>当前空闲对象数 / Current idle</summary>
        public readonly int CurrentIdle;

        /// <summary>当前总对象数 / Current total</summary>
        public readonly int CurrentTotal;

        #endregion

        #region 操作计数 Operation Counts

        /// <summary>总获取次数 / Total gets</summary>
        public readonly long TotalGets;

        /// <summary>总归还次数 / Total returns</summary>
        public readonly long TotalReturns;

        /// <summary>命中次数 / Hits</summary>
        public readonly long Hits;

        /// <summary>未命中次数 / Misses</summary>
        public readonly long Misses;

        #endregion

        #region 性能指标 Performance Metrics

        /// <summary>命中率 / Hit rate</summary>
        public readonly double HitRate;

        /// <summary>未命中率 / Miss rate</summary>
        public readonly double MissRate;

        /// <summary>平均获取时间（毫秒）/ Average get time (ms)</summary>
        public readonly double AverageGetTime;

        /// <summary>平均归还时间（毫秒）/ Average return time (ms)</summary>
        public readonly double AverageReturnTime;

        #endregion

        #region 容量指标 Capacity Metrics

        /// <summary>峰值活跃对象数 / Peak active</summary>
        public readonly int PeakActive;

        /// <summary>峰值总对象数 / Peak total</summary>
        public readonly int PeakTotal;

        /// <summary>估算内存占用（字节）/ Estimated memory usage (bytes)</summary>
        public readonly long EstimatedMemoryUsage;

        #endregion

        #region 时间信息 Time Information

        /// <summary>快照时间 / Snapshot time</summary>
        public readonly DateTime SnapshotTime;

        /// <summary>池创建时间 / Pool created time</summary>
        public readonly DateTime CreatedTime;

        /// <summary>池运行时长 / Pool uptime</summary>
        public readonly TimeSpan Uptime;

        #endregion

        /// <summary>
        /// 初始化统计信息快照
        /// Initialize statistics snapshot
        /// </summary>
        public PoolStatisticsSnapshot(IPoolStatistics statistics)
        {
            SnapshotTime = DateTime.UtcNow;
            CreatedTime = statistics.CreatedTime;
            Uptime = statistics.Uptime;

            TotalCreated = statistics.TotalCreated;
            TotalDestroyed = statistics.TotalDestroyed;
            CurrentActive = statistics.CurrentActive;
            CurrentIdle = statistics.CurrentIdle;
            CurrentTotal = statistics.CurrentTotal;

            TotalGets = statistics.TotalGets;
            TotalReturns = statistics.TotalReturns;
            Hits = statistics.Hits;
            Misses = statistics.Misses;

            HitRate = statistics.HitRate;
            MissRate = statistics.MissRate;
            AverageGetTime = statistics.AverageGetTime;
            AverageReturnTime = statistics.AverageReturnTime;

            PeakActive = statistics.PeakActive;
            PeakTotal = statistics.PeakTotal;
            EstimatedMemoryUsage = statistics.EstimatedMemoryUsage;
        }
    }
}
