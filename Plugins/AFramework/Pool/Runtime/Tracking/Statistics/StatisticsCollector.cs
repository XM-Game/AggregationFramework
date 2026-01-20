// ==========================================================
// 文件名：StatisticsCollector.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, System.Collections.Generic
// 功能: 统计信息收集器
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 统计信息收集器
    /// Statistics Collector
    /// </summary>
    /// <remarks>
    /// 定期收集池的使用快照，用于趋势分析和历史记录
    /// Periodically collects pool usage snapshots for trend analysis and historical records
    /// </remarks>
    public class StatisticsCollector
    {
        #region 字段 Fields

        private readonly IPoolStatistics _statistics;
        private readonly List<PoolUsageSnapshot> _snapshots;
        private readonly int _maxSnapshots;
        private readonly object _lock = new object();

        private DateTime _lastCollectionTime;
        private TimeSpan _collectionInterval;
        private bool _isAutoCollecting;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取快照数量
        /// Get snapshot count
        /// </summary>
        public int SnapshotCount
        {
            get
            {
                lock (_lock)
                {
                    return _snapshots.Count;
                }
            }
        }

        /// <summary>
        /// 获取或设置收集间隔
        /// Get or set collection interval
        /// </summary>
        public TimeSpan CollectionInterval
        {
            get => _collectionInterval;
            set => _collectionInterval = value;
        }

        /// <summary>
        /// 获取或设置是否自动收集
        /// Get or set whether auto-collecting
        /// </summary>
        public bool IsAutoCollecting
        {
            get => _isAutoCollecting;
            set => _isAutoCollecting = value;
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化统计信息收集器
        /// Initialize statistics collector
        /// </summary>
        /// <param name="statistics">统计信息源 / Statistics source</param>
        /// <param name="maxSnapshots">最大快照数（默认 100）/ Maximum snapshot count (default 100)</param>
        /// <param name="collectionInterval">收集间隔（默认 1 分钟）/ Collection interval (default 1 minute)</param>
        public StatisticsCollector(
            IPoolStatistics statistics,
            int maxSnapshots = 100,
            TimeSpan? collectionInterval = null)
        {
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
            _maxSnapshots = maxSnapshots;
            _snapshots = new List<PoolUsageSnapshot>(maxSnapshots);
            _collectionInterval = collectionInterval ?? TimeSpan.FromMinutes(1);
            _lastCollectionTime = DateTime.UtcNow;
            _isAutoCollecting = false;
        }

        #endregion

        #region 收集方法 Collection Methods

        /// <summary>
        /// 手动收集快照
        /// Manually collect snapshot
        /// </summary>
        public void CollectSnapshot()
        {
            var snapshot = new PoolUsageSnapshot(_statistics);

            lock (_lock)
            {
                if (_snapshots.Count >= _maxSnapshots)
                {
                    _snapshots.RemoveAt(0);
                }
                _snapshots.Add(snapshot);
            }

            _lastCollectionTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新自动收集（需要在主循环中调用）
        /// Update auto-collection (needs to be called in main loop)
        /// </summary>
        public void Update()
        {
            if (!_isAutoCollecting) return;

            var now = DateTime.UtcNow;
            if (now - _lastCollectionTime >= _collectionInterval)
            {
                CollectSnapshot();
            }
        }

        #endregion

        #region 查询方法 Query Methods

        /// <summary>
        /// 获取所有快照
        /// Get all snapshots
        /// </summary>
        public IReadOnlyList<PoolUsageSnapshot> GetSnapshots()
        {
            lock (_lock)
            {
                return _snapshots.ToArray();
            }
        }

        /// <summary>
        /// 获取指定时间范围内的快照
        /// Get snapshots within specified time range
        /// </summary>
        public IReadOnlyList<PoolUsageSnapshot> GetSnapshots(DateTime startTime, DateTime endTime)
        {
            lock (_lock)
            {
                return _snapshots
                    .Where(s => s.Timestamp >= startTime && s.Timestamp <= endTime)
                    .ToArray();
            }
        }

        /// <summary>
        /// 获取最近的快照
        /// Get recent snapshots
        /// </summary>
        /// <param name="count">数量 / Count</param>
        public IReadOnlyList<PoolUsageSnapshot> GetRecentSnapshots(int count)
        {
            lock (_lock)
            {
                return _snapshots
                    .Skip(Math.Max(0, _snapshots.Count - count))
                    .ToArray();
            }
        }

        /// <summary>
        /// 获取最新快照
        /// Get latest snapshot
        /// </summary>
        public PoolUsageSnapshot? GetLatestSnapshot()
        {
            lock (_lock)
            {
                return _snapshots.Count > 0 ? _snapshots[_snapshots.Count - 1] : (PoolUsageSnapshot?)null;
            }
        }

        #endregion

        #region 分析方法 Analysis Methods

        /// <summary>
        /// 计算平均使用率
        /// Calculate average usage rate
        /// </summary>
        public double CalculateAverageUsageRate()
        {
            lock (_lock)
            {
                if (_snapshots.Count == 0) return 0.0;
                return _snapshots.Average(s => s.UsageRate);
            }
        }

        /// <summary>
        /// 计算峰值使用率
        /// Calculate peak usage rate
        /// </summary>
        public double CalculatePeakUsageRate()
        {
            lock (_lock)
            {
                if (_snapshots.Count == 0) return 0.0;
                return _snapshots.Max(s => s.UsageRate);
            }
        }

        /// <summary>
        /// 计算平均命中率
        /// Calculate average hit rate
        /// </summary>
        public double CalculateAverageHitRate()
        {
            lock (_lock)
            {
                if (_snapshots.Count == 0) return 0.0;
                return _snapshots.Average(s => s.HitRate);
            }
        }

        /// <summary>
        /// 计算内存使用趋势
        /// Calculate memory usage trend
        /// </summary>
        /// <returns>正值表示增长，负值表示下降 / Positive value indicates growth, negative indicates decline</returns>
        public double CalculateMemoryUsageTrend()
        {
            lock (_lock)
            {
                if (_snapshots.Count < 2) return 0.0;

                var first = _snapshots[0].MemoryUsage;
                var last = _snapshots[_snapshots.Count - 1].MemoryUsage;

                return last - first;
            }
        }

        /// <summary>
        /// 检测异常使用模式
        /// Detect abnormal usage patterns
        /// </summary>
        /// <returns>异常描述列表 / List of abnormality descriptions</returns>
        public List<string> DetectAbnormalPatterns()
        {
            var abnormalities = new List<string>();

            lock (_lock)
            {
                if (_snapshots.Count < 10) return abnormalities;

                // 检测持续低命中率
                // Detect sustained low hit rate
                var recentHitRate = _snapshots.Skip(_snapshots.Count - 10).Average(s => s.HitRate);
                if (recentHitRate < 0.5)
                {
                    abnormalities.Add($"持续低命中率 Sustained low hit rate: {recentHitRate:P2}");
                }

                // 检测持续高使用率
                // Detect sustained high usage rate
                var recentUsageRate = _snapshots.Skip(_snapshots.Count - 10).Average(s => s.UsageRate);
                if (recentUsageRate > 0.9)
                {
                    abnormalities.Add($"持续高使用率 Sustained high usage rate: {recentUsageRate:P2}");
                }

                // 检测内存持续增长
                // Detect sustained memory growth
                var memoryTrend = CalculateMemoryUsageTrend();
                if (memoryTrend > 0)
                {
                    var growthRate = (double)memoryTrend / _snapshots[0].MemoryUsage;
                    if (growthRate > 0.5)
                    {
                        abnormalities.Add($"内存持续增长 Sustained memory growth: {growthRate:P2}");
                    }
                }
            }

            return abnormalities;
        }

        #endregion

        #region 管理方法 Management Methods

        /// <summary>
        /// 清空所有快照
        /// Clear all snapshots
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _snapshots.Clear();
            }
        }

        #endregion
    }
}
