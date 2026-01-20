// ==========================================================
// 文件名：PoolMetrics.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System
// 功能: 对象池性能指标
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池性能指标
    /// Pool Performance Metrics
    /// </summary>
    /// <remarks>
    /// 提供详细的性能分析数据，包括延迟分布、吞吐量等
    /// Provides detailed performance analysis data including latency distribution, throughput, etc.
    /// </remarks>
    public class PoolMetrics
    {
        #region 字段 Fields

        private readonly List<double> _getLatencies;
        private readonly List<double> _returnLatencies;
        private readonly int _maxSamples;
        private readonly object _lock = new object();

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取操作延迟样本数
        /// Get operation latency sample count
        /// </summary>
        public int GetLatencySampleCount
        {
            get
            {
                lock (_lock)
                {
                    return _getLatencies.Count;
                }
            }
        }

        /// <summary>
        /// 归还操作延迟样本数
        /// Return operation latency sample count
        /// </summary>
        public int ReturnLatencySampleCount
        {
            get
            {
                lock (_lock)
                {
                    return _returnLatencies.Count;
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化性能指标
        /// Initialize performance metrics
        /// </summary>
        /// <param name="maxSamples">最大样本数（默认 1000）/ Maximum sample count (default 1000)</param>
        public PoolMetrics(int maxSamples = 1000)
        {
            _maxSamples = maxSamples;
            _getLatencies = new List<double>(maxSamples);
            _returnLatencies = new List<double>(maxSamples);
        }

        #endregion

        #region 记录方法 Recording Methods

        /// <summary>
        /// 记录获取操作延迟
        /// Record get operation latency
        /// </summary>
        /// <param name="latencyMs">延迟（毫秒）/ Latency (milliseconds)</param>
        public void RecordGetLatency(double latencyMs)
        {
            lock (_lock)
            {
                if (_getLatencies.Count >= _maxSamples)
                {
                    _getLatencies.RemoveAt(0);
                }
                _getLatencies.Add(latencyMs);
            }
        }

        /// <summary>
        /// 记录归还操作延迟
        /// Record return operation latency
        /// </summary>
        /// <param name="latencyMs">延迟（毫秒）/ Latency (milliseconds)</param>
        public void RecordReturnLatency(double latencyMs)
        {
            lock (_lock)
            {
                if (_returnLatencies.Count >= _maxSamples)
                {
                    _returnLatencies.RemoveAt(0);
                }
                _returnLatencies.Add(latencyMs);
            }
        }

        #endregion

        #region 统计方法 Statistics Methods

        /// <summary>
        /// 获取获取操作的平均延迟
        /// Get average latency of get operations
        /// </summary>
        public double GetAverageGetLatency()
        {
            lock (_lock)
            {
                return _getLatencies.Count > 0 ? _getLatencies.Average() : 0.0;
            }
        }

        /// <summary>
        /// 获取获取操作的最小延迟
        /// Get minimum latency of get operations
        /// </summary>
        public double GetMinGetLatency()
        {
            lock (_lock)
            {
                return _getLatencies.Count > 0 ? _getLatencies.Min() : 0.0;
            }
        }

        /// <summary>
        /// 获取获取操作的最大延迟
        /// Get maximum latency of get operations
        /// </summary>
        public double GetMaxGetLatency()
        {
            lock (_lock)
            {
                return _getLatencies.Count > 0 ? _getLatencies.Max() : 0.0;
            }
        }

        /// <summary>
        /// 获取获取操作的百分位延迟
        /// Get percentile latency of get operations
        /// </summary>
        /// <param name="percentile">百分位（0.0 - 1.0）/ Percentile (0.0 - 1.0)</param>
        public double GetPercentileGetLatency(double percentile)
        {
            lock (_lock)
            {
                if (_getLatencies.Count == 0) return 0.0;

                var sorted = _getLatencies.OrderBy(x => x).ToList();
                int index = (int)Math.Ceiling(percentile * sorted.Count) - 1;
                index = Math.Max(0, Math.Min(index, sorted.Count - 1));
                return sorted[index];
            }
        }

        /// <summary>
        /// 获取归还操作的平均延迟
        /// Get average latency of return operations
        /// </summary>
        public double GetAverageReturnLatency()
        {
            lock (_lock)
            {
                return _returnLatencies.Count > 0 ? _returnLatencies.Average() : 0.0;
            }
        }

        /// <summary>
        /// 获取归还操作的百分位延迟
        /// Get percentile latency of return operations
        /// </summary>
        /// <param name="percentile">百分位（0.0 - 1.0）/ Percentile (0.0 - 1.0)</param>
        public double GetPercentileReturnLatency(double percentile)
        {
            lock (_lock)
            {
                if (_returnLatencies.Count == 0) return 0.0;

                var sorted = _returnLatencies.OrderBy(x => x).ToList();
                int index = (int)Math.Ceiling(percentile * sorted.Count) - 1;
                index = Math.Max(0, Math.Min(index, sorted.Count - 1));
                return sorted[index];
            }
        }

        /// <summary>
        /// 获取延迟分布直方图
        /// Get latency distribution histogram
        /// </summary>
        /// <param name="bucketCount">桶数量 / Bucket count</param>
        /// <returns>直方图数据 / Histogram data</returns>
        public LatencyHistogram GetGetLatencyHistogram(int bucketCount = 10)
        {
            lock (_lock)
            {
                if (_getLatencies.Count == 0)
                {
                    return new LatencyHistogram(Array.Empty<double>(), Array.Empty<int>());
                }

                var min = _getLatencies.Min();
                var max = _getLatencies.Max();
                var bucketSize = (max - min) / bucketCount;

                var buckets = new int[bucketCount];
                var bucketRanges = new double[bucketCount];

                for (int i = 0; i < bucketCount; i++)
                {
                    bucketRanges[i] = min + (i + 1) * bucketSize;
                }

                foreach (var latency in _getLatencies)
                {
                    int bucketIndex = (int)((latency - min) / bucketSize);
                    bucketIndex = Math.Min(bucketIndex, bucketCount - 1);
                    buckets[bucketIndex]++;
                }

                return new LatencyHistogram(bucketRanges, buckets);
            }
        }

        /// <summary>
        /// 清空所有样本
        /// Clear all samples
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _getLatencies.Clear();
                _returnLatencies.Clear();
            }
        }

        #endregion
    }

    /// <summary>
    /// 延迟分布直方图
    /// Latency Distribution Histogram
    /// </summary>
    public readonly struct LatencyHistogram
    {
        /// <summary>
        /// 桶范围（上界）
        /// Bucket ranges (upper bounds)
        /// </summary>
        public readonly double[] BucketRanges;

        /// <summary>
        /// 每个桶的样本数
        /// Sample count in each bucket
        /// </summary>
        public readonly int[] BucketCounts;

        /// <summary>
        /// 初始化直方图
        /// Initialize histogram
        /// </summary>
        public LatencyHistogram(double[] bucketRanges, int[] bucketCounts)
        {
            BucketRanges = bucketRanges;
            BucketCounts = bucketCounts;
        }
    }
}
