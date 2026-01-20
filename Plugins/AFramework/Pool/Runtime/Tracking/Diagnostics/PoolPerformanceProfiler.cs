// ==========================================================
// 文件名：PoolPerformanceProfiler.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, System.Collections.Generic, System.Diagnostics
// 功能: 对象池性能分析器
// ==========================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池性能分析器
    /// Pool Performance Profiler
    /// </summary>
    /// <remarks>
    /// 提供详细的性能分析功能，包括延迟分布、吞吐量统计等
    /// Provides detailed performance analysis including latency distribution, throughput statistics, etc.
    /// </remarks>
    public class PoolPerformanceProfiler
    {
        #region 字段 Fields

        private readonly PoolMetrics _metrics;
        private readonly List<PerformanceSample> _samples;
        private readonly int _maxSamples;
        private readonly object _lock = new object();

        private bool _isEnabled;
        private DateTime _profilingStartTime;
        private long _totalOperations;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取或设置是否启用性能分析
        /// Get or set whether profiling is enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        /// <summary>
        /// 获取性能指标
        /// Get performance metrics
        /// </summary>
        public PoolMetrics Metrics => _metrics;

        /// <summary>
        /// 获取采样数量
        /// Get sample count
        /// </summary>
        public int SampleCount
        {
            get
            {
                lock (_lock)
                {
                    return _samples.Count;
                }
            }
        }

        /// <summary>
        /// 获取分析持续时间
        /// Get profiling duration
        /// </summary>
        public TimeSpan ProfilingDuration => _isEnabled ? DateTime.UtcNow - _profilingStartTime : TimeSpan.Zero;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化性能分析器
        /// Initialize performance profiler
        /// </summary>
        /// <param name="maxSamples">最大样本数（默认 10000）/ Maximum sample count (default 10000)</param>
        public PoolPerformanceProfiler(int maxSamples = 10000)
        {
            _maxSamples = maxSamples;
            _metrics = new PoolMetrics(maxSamples);
            _samples = new List<PerformanceSample>(maxSamples);
            _isEnabled = false;
        }

        #endregion

        #region 控制方法 Control Methods

        /// <summary>
        /// 开始性能分析
        /// Start profiling
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                _isEnabled = true;
                _profilingStartTime = DateTime.UtcNow;
                _totalOperations = 0;
                _samples.Clear();
                _metrics.Clear();
            }
        }

        /// <summary>
        /// 停止性能分析
        /// Stop profiling
        /// </summary>
        public void Stop()
        {
            _isEnabled = false;
        }

        /// <summary>
        /// 重置性能分析数据
        /// Reset profiling data
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _samples.Clear();
                _metrics.Clear();
                _totalOperations = 0;
                if (_isEnabled)
                {
                    _profilingStartTime = DateTime.UtcNow;
                }
            }
        }

        #endregion

        #region 记录方法 Recording Methods

        /// <summary>
        /// 记录获取操作
        /// Record get operation
        /// </summary>
        /// <param name="elapsedMs">耗时（毫秒）/ Elapsed time (milliseconds)</param>
        /// <param name="isHit">是否命中 / Whether hit</param>
        public void RecordGet(double elapsedMs, bool isHit)
        {
            if (!_isEnabled) return;

            _metrics.RecordGetLatency(elapsedMs);

            lock (_lock)
            {
                _totalOperations++;

                if (_samples.Count >= _maxSamples)
                {
                    _samples.RemoveAt(0);
                }

                _samples.Add(new PerformanceSample(
                    DateTime.UtcNow,
                    OperationType.Get,
                    elapsedMs,
                    isHit
                ));
            }
        }

        /// <summary>
        /// 记录归还操作
        /// Record return operation
        /// </summary>
        /// <param name="elapsedMs">耗时（毫秒）/ Elapsed time (milliseconds)</param>
        public void RecordReturn(double elapsedMs)
        {
            if (!_isEnabled) return;

            _metrics.RecordReturnLatency(elapsedMs);

            lock (_lock)
            {
                _totalOperations++;

                if (_samples.Count >= _maxSamples)
                {
                    _samples.RemoveAt(0);
                }

                _samples.Add(new PerformanceSample(
                    DateTime.UtcNow,
                    OperationType.Return,
                    elapsedMs,
                    false
                ));
            }
        }

        #endregion

        #region 分析方法 Analysis Methods

        /// <summary>
        /// 生成性能报告
        /// Generate performance report
        /// </summary>
        public PerformanceReport GenerateReport()
        {
            lock (_lock)
            {
                var duration = ProfilingDuration;
                var sampleCount = _samples.Count;

                if (sampleCount == 0)
                {
                    return new PerformanceReport(
                        0, 0, 0, 0, 0, 0, 0, duration
                    );
                }

                var avgGetTime = _metrics.GetAverageGetLatency();
                var minGetTime = _metrics.GetMinGetLatency();
                var maxGetTime = _metrics.GetMaxGetLatency();
                var avgReturnTime = _metrics.GetAverageReturnLatency();
                var p95GetLatency = _metrics.GetPercentileGetLatency(0.95);
                var p99GetLatency = _metrics.GetPercentileGetLatency(0.99);

                return new PerformanceReport(
                    avgGetTime,
                    maxGetTime,
                    minGetTime,
                    avgReturnTime,
                    p95GetLatency,
                    p99GetLatency,
                    sampleCount,
                    duration
                );
            }
        }

        /// <summary>
        /// 计算吞吐量（操作数/秒）
        /// Calculate throughput (operations per second)
        /// </summary>
        public double CalculateThroughput()
        {
            var duration = ProfilingDuration;
            if (duration.TotalSeconds < 0.001) return 0.0;

            return _totalOperations / duration.TotalSeconds;
        }

        /// <summary>
        /// 获取延迟分布直方图
        /// Get latency distribution histogram
        /// </summary>
        public LatencyHistogram GetLatencyHistogram(int bucketCount = 10)
        {
            return _metrics.GetGetLatencyHistogram(bucketCount);
        }

        /// <summary>
        /// 获取时间序列数据
        /// Get time series data
        /// </summary>
        /// <param name="intervalSeconds">时间间隔（秒）/ Time interval (seconds)</param>
        public IReadOnlyList<TimeSeriesPoint> GetTimeSeries(double intervalSeconds = 1.0)
        {
            lock (_lock)
            {
                if (_samples.Count == 0) return Array.Empty<TimeSeriesPoint>();

                var points = new List<TimeSeriesPoint>();
                var startTime = _samples[0].Timestamp;
                var currentBucket = new List<PerformanceSample>();
                var currentBucketStart = startTime;

                foreach (var sample in _samples)
                {
                    var elapsed = (sample.Timestamp - currentBucketStart).TotalSeconds;

                    if (elapsed >= intervalSeconds)
                    {
                        // 完成当前桶
                        // Complete current bucket
                        if (currentBucket.Count > 0)
                        {
                            points.Add(CreateTimeSeriesPoint(currentBucketStart, currentBucket));
                        }

                        // 开始新桶
                        // Start new bucket
                        currentBucket.Clear();
                        currentBucketStart = sample.Timestamp;
                    }

                    currentBucket.Add(sample);
                }

                // 处理最后一个桶
                // Process last bucket
                if (currentBucket.Count > 0)
                {
                    points.Add(CreateTimeSeriesPoint(currentBucketStart, currentBucket));
                }

                return points;
            }
        }

        /// <summary>
        /// 创建时间序列点
        /// Create time series point
        /// </summary>
        private TimeSeriesPoint CreateTimeSeriesPoint(DateTime timestamp, List<PerformanceSample> samples)
        {
            var avgLatency = samples.Average(s => s.LatencyMs);
            var maxLatency = samples.Max(s => s.LatencyMs);
            var operationCount = samples.Count;
            var hitRate = samples.Count(s => s.IsHit) / (double)samples.Count(s => s.OperationType == OperationType.Get);

            return new TimeSeriesPoint(timestamp, avgLatency, maxLatency, operationCount, hitRate);
        }

        #endregion

        #region 内部类型 Internal Types

        /// <summary>
        /// 性能样本
        /// Performance Sample
        /// </summary>
        private readonly struct PerformanceSample
        {
            public readonly DateTime Timestamp;
            public readonly OperationType OperationType;
            public readonly double LatencyMs;
            public readonly bool IsHit;

            public PerformanceSample(DateTime timestamp, OperationType operationType, double latencyMs, bool isHit)
            {
                Timestamp = timestamp;
                OperationType = operationType;
                LatencyMs = latencyMs;
                IsHit = isHit;
            }
        }

        /// <summary>
        /// 操作类型
        /// Operation Type
        /// </summary>
        private enum OperationType
        {
            Get,
            Return
        }

        #endregion
    }

    /// <summary>
    /// 性能报告
    /// Performance Report
    /// </summary>
    public readonly struct PerformanceReport
    {
        /// <summary>平均获取时间（毫秒）/ Average get time (ms)</summary>
        public readonly double AverageGetTime;

        /// <summary>最大获取时间（毫秒）/ Maximum get time (ms)</summary>
        public readonly double MaxGetTime;

        /// <summary>最小获取时间（毫秒）/ Minimum get time (ms)</summary>
        public readonly double MinGetTime;

        /// <summary>平均归还时间（毫秒）/ Average return time (ms)</summary>
        public readonly double AverageReturnTime;

        /// <summary>P95 获取延迟（毫秒）/ P95 get latency (ms)</summary>
        public readonly double GetP95Latency;

        /// <summary>P99 获取延迟（毫秒）/ P99 get latency (ms)</summary>
        public readonly double GetP99Latency;

        /// <summary>采样数量 / Sample count</summary>
        public readonly int SampleCount;

        /// <summary>采样时长 / Sample duration</summary>
        public readonly TimeSpan SampleDuration;

        /// <summary>
        /// 初始化性能报告
        /// Initialize performance report
        /// </summary>
        public PerformanceReport(
            double avgGetTime,
            double maxGetTime,
            double minGetTime,
            double avgReturnTime,
            double getP95Latency,
            double getP99Latency,
            int sampleCount,
            TimeSpan sampleDuration)
        {
            AverageGetTime = avgGetTime;
            MaxGetTime = maxGetTime;
            MinGetTime = minGetTime;
            AverageReturnTime = avgReturnTime;
            GetP95Latency = getP95Latency;
            GetP99Latency = getP99Latency;
            SampleCount = sampleCount;
            SampleDuration = sampleDuration;
        }
    }

    /// <summary>
    /// 时间序列点
    /// Time Series Point
    /// </summary>
    public readonly struct TimeSeriesPoint
    {
        /// <summary>时间戳 / Timestamp</summary>
        public readonly DateTime Timestamp;

        /// <summary>平均延迟（毫秒）/ Average latency (ms)</summary>
        public readonly double AverageLatency;

        /// <summary>最大延迟（毫秒）/ Maximum latency (ms)</summary>
        public readonly double MaxLatency;

        /// <summary>操作数量 / Operation count</summary>
        public readonly int OperationCount;

        /// <summary>命中率 / Hit rate</summary>
        public readonly double HitRate;

        /// <summary>
        /// 初始化时间序列点
        /// Initialize time series point
        /// </summary>
        public TimeSeriesPoint(DateTime timestamp, double avgLatency, double maxLatency, int operationCount, double hitRate)
        {
            Timestamp = timestamp;
            AverageLatency = avgLatency;
            MaxLatency = maxLatency;
            OperationCount = operationCount;
            HitRate = hitRate;
        }
    }
}
