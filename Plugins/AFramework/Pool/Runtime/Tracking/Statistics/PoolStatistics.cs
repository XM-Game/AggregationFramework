// ==========================================================
// 文件名：PoolStatistics.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, AFramework.Pool
// 功能: 对象池统计信息实现
// ==========================================================

using System;
using System.Threading;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池统计信息实现
    /// Pool Statistics Implementation
    /// </summary>
    /// <remarks>
    /// 线程安全的统计信息收集器
    /// Thread-safe statistics collector
    /// </remarks>
    public class PoolStatistics : IPoolStatistics
    {
        #region 字段 Fields

        // 对象计数
        private long _totalCreated;
        private long _totalDestroyed;
        private int _currentActive;
        private int _currentIdle;

        // 操作计数
        private long _totalGets;
        private long _totalReturns;
        private long _hits;
        private long _misses;

        // 性能指标
        private long _totalGetTimeMs;
        private long _totalReturnTimeMs;

        // 容量指标
        private int _peakActive;
        private int _peakTotal;

        // 时间信息
        private readonly DateTime _createdTime;
        private DateTime? _lastGetTime;
        private DateTime? _lastReturnTime;

        #endregion

        #region 属性 Properties

        public long TotalCreated => Interlocked.Read(ref _totalCreated);
        public long TotalDestroyed => Interlocked.Read(ref _totalDestroyed);
        public int CurrentActive => Volatile.Read(ref _currentActive);
        public int CurrentIdle => Volatile.Read(ref _currentIdle);
        public int CurrentTotal => CurrentActive + CurrentIdle;

        public long TotalGets => Interlocked.Read(ref _totalGets);
        public long TotalReturns => Interlocked.Read(ref _totalReturns);
        public long Hits => Interlocked.Read(ref _hits);
        public long Misses => Interlocked.Read(ref _misses);

        public double HitRate
        {
            get
            {
                var total = TotalGets;
                return total > 0 ? (double)Hits / total : 0.0;
            }
        }

        public double MissRate
        {
            get
            {
                var total = TotalGets;
                return total > 0 ? (double)Misses / total : 0.0;
            }
        }

        public double AverageGetTime
        {
            get
            {
                var gets = TotalGets;
                return gets > 0 ? (double)Interlocked.Read(ref _totalGetTimeMs) / gets : 0.0;
            }
        }

        public double AverageReturnTime
        {
            get
            {
                var returns = TotalReturns;
                return returns > 0 ? (double)Interlocked.Read(ref _totalReturnTimeMs) / returns : 0.0;
            }
        }

        public int PeakActive => Volatile.Read(ref _peakActive);
        public int PeakTotal => Volatile.Read(ref _peakTotal);

        public long EstimatedMemoryUsage
        {
            get
            {
                // 粗略估算：假设每个对象平均占用 1KB
                // Rough estimation: assume each object takes 1KB on average
                return CurrentTotal * 1024L;
            }
        }

        public DateTime CreatedTime => _createdTime;
        public TimeSpan Uptime => DateTime.UtcNow - _createdTime;
        public DateTime? LastGetTime => _lastGetTime;
        public DateTime? LastReturnTime => _lastReturnTime;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化统计信息
        /// Initialize statistics
        /// </summary>
        public PoolStatistics()
        {
            _createdTime = DateTime.UtcNow;
        }

        #endregion

        #region 记录方法 Recording Methods

        /// <summary>
        /// 记录对象创建
        /// Record object creation
        /// </summary>
        public void RecordCreate()
        {
            Interlocked.Increment(ref _totalCreated);
            UpdateCurrentIdle(1);
        }

        /// <summary>
        /// 记录对象销毁
        /// Record object destruction
        /// </summary>
        public void RecordDestroy()
        {
            Interlocked.Increment(ref _totalDestroyed);
            UpdateCurrentIdle(-1);
        }

        /// <summary>
        /// 记录对象获取
        /// Record object get
        /// </summary>
        /// <param name="isHit">是否命中（从池中获取）/ Whether hit (get from pool)</param>
        /// <param name="elapsedMs">耗时（毫秒）/ Elapsed time (milliseconds)</param>
        public void RecordGet(bool isHit, long elapsedMs = 0)
        {
            Interlocked.Increment(ref _totalGets);
            
            if (isHit)
            {
                Interlocked.Increment(ref _hits);
                UpdateCurrentIdle(-1);
            }
            else
            {
                Interlocked.Increment(ref _misses);
            }

            UpdateCurrentActive(1);
            
            if (elapsedMs > 0)
            {
                Interlocked.Add(ref _totalGetTimeMs, elapsedMs);
            }

            _lastGetTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 记录对象归还
        /// Record object return
        /// </summary>
        /// <param name="elapsedMs">耗时（毫秒）/ Elapsed time (milliseconds)</param>
        public void RecordReturn(long elapsedMs = 0)
        {
            Interlocked.Increment(ref _totalReturns);
            UpdateCurrentActive(-1);
            UpdateCurrentIdle(1);

            if (elapsedMs > 0)
            {
                Interlocked.Add(ref _totalReturnTimeMs, elapsedMs);
            }

            _lastReturnTime = DateTime.UtcNow;
        }

        #endregion

        #region 更新方法 Update Methods

        /// <summary>
        /// 更新当前活跃对象数
        /// Update current active count
        /// </summary>
        private void UpdateCurrentActive(int delta)
        {
            var newActive = Interlocked.Add(ref _currentActive, delta);
            
            // 更新峰值
            // Update peak
            int currentPeak;
            do
            {
                currentPeak = Volatile.Read(ref _peakActive);
                if (newActive <= currentPeak) break;
            }
            while (Interlocked.CompareExchange(ref _peakActive, newActive, currentPeak) != currentPeak);

            UpdatePeakTotal();
        }

        /// <summary>
        /// 更新当前空闲对象数
        /// Update current idle count
        /// </summary>
        private void UpdateCurrentIdle(int delta)
        {
            Interlocked.Add(ref _currentIdle, delta);
            UpdatePeakTotal();
        }

        /// <summary>
        /// 更新峰值总对象数
        /// Update peak total count
        /// </summary>
        private void UpdatePeakTotal()
        {
            var newTotal = CurrentTotal;
            
            int currentPeak;
            do
            {
                currentPeak = Volatile.Read(ref _peakTotal);
                if (newTotal <= currentPeak) break;
            }
            while (Interlocked.CompareExchange(ref _peakTotal, newTotal, currentPeak) != currentPeak);
        }

        #endregion

        #region 快照操作 Snapshot Operations

        /// <summary>
        /// 创建统计信息快照
        /// Create statistics snapshot
        /// </summary>
        public PoolStatisticsSnapshot CreateSnapshot()
        {
            return new PoolStatisticsSnapshot(this);
        }

        /// <summary>
        /// 重置统计信息
        /// Reset statistics
        /// </summary>
        public void Reset()
        {
            // 重置操作计数
            // Reset operation counts
            Interlocked.Exchange(ref _totalGets, 0);
            Interlocked.Exchange(ref _totalReturns, 0);
            Interlocked.Exchange(ref _hits, 0);
            Interlocked.Exchange(ref _misses, 0);

            // 重置性能指标
            // Reset performance metrics
            Interlocked.Exchange(ref _totalGetTimeMs, 0);
            Interlocked.Exchange(ref _totalReturnTimeMs, 0);

            // 重置峰值（保留当前值作为新基准）
            // Reset peaks (keep current values as new baseline)
            Interlocked.Exchange(ref _peakActive, CurrentActive);
            Interlocked.Exchange(ref _peakTotal, CurrentTotal);

            // 重置时间信息
            // Reset time information
            _lastGetTime = null;
            _lastReturnTime = null;
        }

        #endregion
    }
}
