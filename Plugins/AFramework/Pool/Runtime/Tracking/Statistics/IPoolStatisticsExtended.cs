// ==========================================================
// 文件名：IPoolStatisticsExtended.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 扩展的对象池统计信息接口，提供详细的性能分析数据
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 扩展的对象池统计信息接口
    /// Extended Pool Statistics Interface
    /// 
    /// <para>提供详细的性能分析和分布统计</para>
    /// <para>Provides detailed performance analysis and distribution statistics</para>
    /// </summary>
    public interface IPoolStatisticsExtended : IPoolStatistics
    {
        #region 延迟分布统计 Latency Distribution

        /// <summary>
        /// 获取 P50 延迟（中位数，毫秒）
        /// Get P50 latency (median, milliseconds)
        /// </summary>
        double GetLatencyP50 { get; }

        /// <summary>
        /// 获取 P95 延迟（毫秒）
        /// Get P95 latency (milliseconds)
        /// </summary>
        double GetLatencyP95 { get; }

        /// <summary>
        /// 获取 P99 延迟（毫秒）
        /// Get P99 latency (milliseconds)
        /// </summary>
        double GetLatencyP99 { get; }

        /// <summary>
        /// 归还 P50 延迟（中位数，毫秒）
        /// Return P50 latency (median, milliseconds)
        /// </summary>
        double ReturnLatencyP50 { get; }

        /// <summary>
        /// 归还 P95 延迟（毫秒）
        /// Return P95 latency (milliseconds)
        /// </summary>
        double ReturnLatencyP95 { get; }

        /// <summary>
        /// 归还 P99 延迟（毫秒）
        /// Return P99 latency (milliseconds)
        /// </summary>
        double ReturnLatencyP99 { get; }

        #endregion

        #region 对象生命周期统计 Object Lifecycle Statistics

        /// <summary>
        /// 平均对象生命周期（秒）
        /// Average object lifetime (seconds)
        /// </summary>
        double AverageObjectLifetime { get; }

        /// <summary>
        /// 最短对象生命周期（秒）
        /// Minimum object lifetime (seconds)
        /// </summary>
        double MinObjectLifetime { get; }

        /// <summary>
        /// 最长对象生命周期（秒）
        /// Maximum object lifetime (seconds)
        /// </summary>
        double MaxObjectLifetime { get; }

        /// <summary>
        /// 对象重用次数分布
        /// Object reuse count distribution
        /// </summary>
        IReadOnlyDictionary<int, int> ReuseCountDistribution { get; }

        #endregion

        #region 操作频率统计 Operation Frequency Statistics

        /// <summary>
        /// 每秒获取操作数（QPS）
        /// Gets per second (QPS)
        /// </summary>
        double GetsPerSecond { get; }

        /// <summary>
        /// 每秒归还操作数
        /// Returns per second
        /// </summary>
        double ReturnsPerSecond { get; }

        /// <summary>
        /// 峰值 QPS
        /// Peak QPS
        /// </summary>
        double PeakQPS { get; }

        /// <summary>
        /// 最近 1 分钟的操作频率
        /// Operation frequency in last 1 minute
        /// </summary>
        OperationFrequency LastMinuteFrequency { get; }

        /// <summary>
        /// 最近 5 分钟的操作频率
        /// Operation frequency in last 5 minutes
        /// </summary>
        OperationFrequency LastFiveMinutesFrequency { get; }

        #endregion

        #region 详细报告 Detailed Reports

        /// <summary>
        /// 生成详细统计报告
        /// Generate detailed statistics report
        /// </summary>
        /// <returns>详细报告 / Detailed report</returns>
        DetailedStatisticsReport GenerateDetailedReport();

        /// <summary>
        /// 导出统计数据为 JSON
        /// Export statistics as JSON
        /// </summary>
        /// <returns>JSON 字符串 / JSON string</returns>
        string ExportAsJson();

        /// <summary>
        /// 导出统计数据为 CSV
        /// Export statistics as CSV
        /// </summary>
        /// <returns>CSV 字符串 / CSV string</returns>
        string ExportAsCsv();

        #endregion
    }

    /// <summary>
    /// 操作频率统计
    /// Operation Frequency Statistics
    /// </summary>
    public readonly struct OperationFrequency
    {
        /// <summary>
        /// 获取操作数
        /// Get operations count
        /// </summary>
        public readonly long Gets;

        /// <summary>
        /// 归还操作数
        /// Return operations count
        /// </summary>
        public readonly long Returns;

        /// <summary>
        /// 创建操作数
        /// Create operations count
        /// </summary>
        public readonly long Creates;

        /// <summary>
        /// 销毁操作数
        /// Destroy operations count
        /// </summary>
        public readonly long Destroys;

        /// <summary>
        /// 时间窗口（秒）
        /// Time window (seconds)
        /// </summary>
        public readonly double TimeWindowSeconds;

        /// <summary>
        /// 平均 QPS
        /// Average QPS
        /// </summary>
        public double AverageQPS => TimeWindowSeconds > 0 ? Gets / TimeWindowSeconds : 0;

        /// <summary>
        /// 初始化操作频率统计
        /// Initialize operation frequency statistics
        /// </summary>
        public OperationFrequency(long gets, long returns, long creates, long destroys, double timeWindowSeconds)
        {
            Gets = gets;
            Returns = returns;
            Creates = creates;
            Destroys = destroys;
            TimeWindowSeconds = timeWindowSeconds;
        }
    }

    /// <summary>
    /// 详细统计报告
    /// Detailed Statistics Report
    /// </summary>
    public readonly struct DetailedStatisticsReport
    {
        /// <summary>
        /// 报告生成时间
        /// Report generation time
        /// </summary>
        public readonly DateTime GeneratedTime;

        /// <summary>
        /// 基础统计快照
        /// Basic statistics snapshot
        /// </summary>
        public readonly PoolStatisticsSnapshot BasicStatistics;

        /// <summary>
        /// 延迟分布
        /// Latency distribution
        /// </summary>
        public readonly LatencyDistribution GetLatencyDistribution;

        /// <summary>
        /// 归还延迟分布
        /// Return latency distribution
        /// </summary>
        public readonly LatencyDistribution ReturnLatencyDistribution;

        /// <summary>
        /// 生命周期统计
        /// Lifecycle statistics
        /// </summary>
        public readonly LifecycleStatistics Lifecycle;

        /// <summary>
        /// 频率统计
        /// Frequency statistics
        /// </summary>
        public readonly FrequencyStatistics Frequency;

        /// <summary>
        /// 初始化详细统计报告
        /// Initialize detailed statistics report
        /// </summary>
        public DetailedStatisticsReport(
            DateTime generatedTime,
            PoolStatisticsSnapshot basicStatistics,
            LatencyDistribution getLatencyDistribution,
            LatencyDistribution returnLatencyDistribution,
            LifecycleStatistics lifecycle,
            FrequencyStatistics frequency)
        {
            GeneratedTime = generatedTime;
            BasicStatistics = basicStatistics;
            GetLatencyDistribution = getLatencyDistribution;
            ReturnLatencyDistribution = returnLatencyDistribution;
            Lifecycle = lifecycle;
            Frequency = frequency;
        }
    }

    /// <summary>
    /// 延迟分布统计
    /// Latency Distribution Statistics
    /// </summary>
    public readonly struct LatencyDistribution
    {
        /// <summary>最小值（毫秒）/ Minimum (ms)</summary>
        public readonly double Min;

        /// <summary>最大值（毫秒）/ Maximum (ms)</summary>
        public readonly double Max;

        /// <summary>平均值（毫秒）/ Average (ms)</summary>
        public readonly double Average;

        /// <summary>中位数 P50（毫秒）/ Median P50 (ms)</summary>
        public readonly double P50;

        /// <summary>P95（毫秒）/ P95 (ms)</summary>
        public readonly double P95;

        /// <summary>P99（毫秒）/ P99 (ms)</summary>
        public readonly double P99;

        /// <summary>标准差（毫秒）/ Standard deviation (ms)</summary>
        public readonly double StdDev;

        /// <summary>
        /// 初始化延迟分布统计
        /// Initialize latency distribution statistics
        /// </summary>
        public LatencyDistribution(double min, double max, double average, double p50, double p95, double p99, double stdDev)
        {
            Min = min;
            Max = max;
            Average = average;
            P50 = p50;
            P95 = p95;
            P99 = p99;
            StdDev = stdDev;
        }
    }

    /// <summary>
    /// 生命周期统计
    /// Lifecycle Statistics
    /// </summary>
    public readonly struct LifecycleStatistics
    {
        /// <summary>平均生命周期（秒）/ Average lifetime (seconds)</summary>
        public readonly double AverageLifetime;

        /// <summary>最短生命周期（秒）/ Minimum lifetime (seconds)</summary>
        public readonly double MinLifetime;

        /// <summary>最长生命周期（秒）/ Maximum lifetime (seconds)</summary>
        public readonly double MaxLifetime;

        /// <summary>平均重用次数 / Average reuse count</summary>
        public readonly double AverageReuseCount;

        /// <summary>最大重用次数 / Maximum reuse count</summary>
        public readonly int MaxReuseCount;

        /// <summary>
        /// 初始化生命周期统计
        /// Initialize lifecycle statistics
        /// </summary>
        public LifecycleStatistics(double avgLifetime, double minLifetime, double maxLifetime, double avgReuseCount, int maxReuseCount)
        {
            AverageLifetime = avgLifetime;
            MinLifetime = minLifetime;
            MaxLifetime = maxLifetime;
            AverageReuseCount = avgReuseCount;
            MaxReuseCount = maxReuseCount;
        }
    }

    /// <summary>
    /// 频率统计
    /// Frequency Statistics
    /// </summary>
    public readonly struct FrequencyStatistics
    {
        /// <summary>当前 QPS / Current QPS</summary>
        public readonly double CurrentQPS;

        /// <summary>峰值 QPS / Peak QPS</summary>
        public readonly double PeakQPS;

        /// <summary>最近 1 分钟频率 / Last 1 minute frequency</summary>
        public readonly OperationFrequency LastMinute;

        /// <summary>最近 5 分钟频率 / Last 5 minutes frequency</summary>
        public readonly OperationFrequency LastFiveMinutes;

        /// <summary>
        /// 初始化频率统计
        /// Initialize frequency statistics
        /// </summary>
        public FrequencyStatistics(double currentQPS, double peakQPS, OperationFrequency lastMinute, OperationFrequency lastFiveMinutes)
        {
            CurrentQPS = currentQPS;
            PeakQPS = peakQPS;
            LastMinute = lastMinute;
            LastFiveMinutes = lastFiveMinutes;
        }
    }
}
