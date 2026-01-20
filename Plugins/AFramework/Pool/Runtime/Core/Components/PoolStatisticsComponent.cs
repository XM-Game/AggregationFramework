// ==========================================================
// 文件名：PoolStatisticsComponent.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Threading
// 功能: 对象池统计组件，负责收集和管理池的统计信息
// ==========================================================

using System;
using System.Threading;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池统计组件
    /// Object Pool Statistics Component
    /// 
    /// <para>负责收集和管理对象池的统计信息</para>
    /// <para>Responsible for collecting and managing object pool statistics</para>
    /// </summary>
    public class PoolStatisticsComponent : IPoolComponent
    {
        #region Fields

        private long _totalCreated;
        private long _totalDestroyed;
        private long _totalGets;
        private long _totalReturns;
        private long _hitCount;
        private long _missCount;
        private int _activeCount;
        private int _idleCount;
        private int _peakActive;
        private int _peakTotal;
        private readonly DateTime _createdTime;
        private DateTime? _lastGetTime;
        private DateTime? _lastReturnTime;
        private bool _isEnabled;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name => "Statistics";

        /// <inheritdoc />
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        /// <summary>
        /// 总创建数
        /// Total created count
        /// </summary>
        public long TotalCreated => _totalCreated;

        /// <summary>
        /// 总销毁数
        /// Total destroyed count
        /// </summary>
        public long TotalDestroyed => _totalDestroyed;

        /// <summary>
        /// 当前活跃数
        /// Current active count
        /// </summary>
        public int CurrentActive => _activeCount;

        /// <summary>
        /// 当前空闲数
        /// Current idle count
        /// </summary>
        public int CurrentIdle => _idleCount;

        /// <summary>
        /// 当前总数
        /// Current total count
        /// </summary>
        public int CurrentTotal => _activeCount + _idleCount;

        /// <summary>
        /// 总获取次数
        /// Total get count
        /// </summary>
        public long TotalGets => _totalGets;

        /// <summary>
        /// 总归还次数
        /// Total return count
        /// </summary>
        public long TotalReturns => _totalReturns;

        /// <summary>
        /// 命中次数
        /// Hit count
        /// </summary>
        public long Hits => _hitCount;

        /// <summary>
        /// 未命中次数
        /// Miss count
        /// </summary>
        public long Misses => _missCount;

        /// <summary>
        /// 命中率
        /// Hit rate
        /// </summary>
        public double HitRate => (_hitCount + _missCount) > 0 ? (double)_hitCount / (_hitCount + _missCount) : 0;

        /// <summary>
        /// 未命中率
        /// Miss rate
        /// </summary>
        public double MissRate => (_hitCount + _missCount) > 0 ? (double)_missCount / (_hitCount + _missCount) : 0;

        /// <summary>
        /// 峰值活跃数
        /// Peak active count
        /// </summary>
        public int PeakActive => _peakActive;

        /// <summary>
        /// 峰值总数
        /// Peak total count
        /// </summary>
        public int PeakTotal => _peakTotal;

        /// <summary>
        /// 创建时间
        /// Created time
        /// </summary>
        public DateTime CreatedTime => _createdTime;

        /// <summary>
        /// 运行时长
        /// Uptime
        /// </summary>
        public TimeSpan Uptime => DateTime.UtcNow - _createdTime;

        /// <summary>
        /// 最后获取时间
        /// Last get time
        /// </summary>
        public DateTime? LastGetTime => _lastGetTime;

        /// <summary>
        /// 最后归还时间
        /// Last return time
        /// </summary>
        public DateTime? LastReturnTime => _lastReturnTime;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        public PoolStatisticsComponent()
        {
            _createdTime = DateTime.UtcNow;
            _isEnabled = true;
        }

        #endregion

        #region IPoolComponent Implementation

        /// <inheritdoc />
        public void Initialize()
        {
            // 统计组件无需特殊初始化
            // Statistics component doesn't need special initialization
        }

        /// <inheritdoc />
        public void Reset()
        {
            _totalGets = 0;
            _totalReturns = 0;
            _hitCount = 0;
            _missCount = 0;
            _peakActive = _activeCount;
            _peakTotal = _activeCount + _idleCount;
            _lastGetTime = null;
            _lastReturnTime = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // 统计组件无需释放资源
            // Statistics component doesn't need to release resources
        }

        #endregion

        #region Statistics Methods

        /// <summary>
        /// 记录对象创建
        /// Record object creation
        /// </summary>
        public void RecordCreation()
        {
            if (!_isEnabled) return;
            Interlocked.Increment(ref _totalCreated);
            UpdatePeaks();
        }

        /// <summary>
        /// 记录对象销毁
        /// Record object destruction
        /// </summary>
        public void RecordDestruction()
        {
            if (!_isEnabled) return;
            Interlocked.Increment(ref _totalDestroyed);
        }

        /// <summary>
        /// 记录命中（从池中获取）
        /// Record hit (get from pool)
        /// </summary>
        public void RecordHit()
        {
            if (!_isEnabled) return;
            Interlocked.Increment(ref _hitCount);
            Interlocked.Increment(ref _totalGets);
            _lastGetTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 记录未命中（创建新对象）
        /// Record miss (create new object)
        /// </summary>
        public void RecordMiss()
        {
            if (!_isEnabled) return;
            Interlocked.Increment(ref _missCount);
            Interlocked.Increment(ref _totalGets);
            _lastGetTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 记录对象激活
        /// Record object activation
        /// </summary>
        public void RecordActivation()
        {
            if (!_isEnabled) return;
            Interlocked.Increment(ref _activeCount);
            UpdatePeaks();
        }

        /// <summary>
        /// 记录对象归还
        /// Record object return
        /// </summary>
        public void RecordReturn()
        {
            if (!_isEnabled) return;
            Interlocked.Decrement(ref _activeCount);
            Interlocked.Increment(ref _totalReturns);
            _lastReturnTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 设置空闲对象数
        /// Set idle object count
        /// </summary>
        /// <param name="count">空闲对象数 / Idle object count</param>
        public void SetIdleCount(int count)
        {
            if (!_isEnabled) return;
            _idleCount = count;
            UpdatePeaks();
        }

        /// <summary>
        /// 创建统计快照
        /// Create statistics snapshot
        /// </summary>
        /// <returns>统计快照 / Statistics snapshot</returns>
        public PoolStatisticsSnapshot CreateSnapshot()
        {
            // 创建适配器并返回快照
            // Create adapter and return snapshot
            var adapter = new PoolStatisticsAdapter(this);
            return new PoolStatisticsSnapshot(adapter);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 更新峰值统计
        /// Update peak statistics
        /// </summary>
        private void UpdatePeaks()
        {
            int currentActive = _activeCount;
            int currentTotal = _activeCount + _idleCount;

            if (currentActive > _peakActive)
                _peakActive = currentActive;

            if (currentTotal > _peakTotal)
                _peakTotal = currentTotal;
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// 统计信息适配器
        /// Statistics Adapter
        /// </summary>
        private class PoolStatisticsAdapter : IPoolStatistics
        {
            private readonly PoolStatisticsComponent _component;

            public PoolStatisticsAdapter(PoolStatisticsComponent component)
            {
                _component = component;
            }

            public long TotalCreated => _component._totalCreated;
            public long TotalDestroyed => _component._totalDestroyed;
            public int CurrentActive => _component._activeCount;
            public int CurrentIdle => _component._idleCount;
            public int CurrentTotal => _component._activeCount + _component._idleCount;
            public long TotalGets => _component._totalGets;
            public long TotalReturns => _component._totalReturns;
            public long Hits => _component._hitCount;
            public long Misses => _component._missCount;
            public double HitRate => _component.HitRate;
            public double MissRate => _component.MissRate;
            public double AverageGetTime => 0; // 简化实现
            public double AverageReturnTime => 0; // 简化实现
            public int PeakActive => _component._peakActive;
            public int PeakTotal => _component._peakTotal;
            public long EstimatedMemoryUsage => 0; // 简化实现
            public DateTime CreatedTime => _component._createdTime;
            public TimeSpan Uptime => _component.Uptime;
            public DateTime? LastGetTime => _component._lastGetTime;
            public DateTime? LastReturnTime => _component._lastReturnTime;

            public PoolStatisticsSnapshot CreateSnapshot()
            {
                return new PoolStatisticsSnapshot(this);
            }

            public void Reset()
            {
                _component.Reset();
            }
        }

        #endregion
    }
}
