// ==========================================================
// 文件名：PoolDiagnostics.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, System.Collections.Generic, AFramework.Pool
// 功能: 对象池诊断工具完整实现
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池诊断工具完整实现
    /// Complete Pool Diagnostics Tool Implementation
    /// </summary>
    /// <remarks>
    /// 集成泄漏检测、性能分析、健康检查和异常追踪功能
    /// Integrates leak detection, performance analysis, health check and exception tracking
    /// </remarks>
    public class PoolDiagnostics : IPoolDiagnostics
    {
        #region 字段 Fields

        private readonly IPoolStatistics _statistics;
        private readonly PoolLeakDetector _leakDetector;
        private readonly PoolPerformanceProfiler _profiler;
        private readonly PoolHealthChecker _healthChecker;
        private readonly List<ExceptionRecord> _exceptionHistory;
        private readonly int _maxExceptionHistory;
        private readonly object _lock = new object();

        #endregion

        #region 属性 Properties - IPoolDiagnostics 实现

        public bool LeakDetectionEnabled { get; set; }

        public float LeakDetectionTimeout
        {
            get => (float)_leakDetector.LeakTimeout.TotalSeconds;
            set => _leakDetector.LeakTimeout = TimeSpan.FromSeconds(value);
        }

        public bool ProfilingEnabled
        {
            get => _profiler.IsEnabled;
            set
            {
                if (value && !_profiler.IsEnabled)
                    _profiler.Start();
                else if (!value && _profiler.IsEnabled)
                    _profiler.Stop();
            }
        }

        public bool ExceptionTrackingEnabled { get; set; }

        public IReadOnlyList<IHealthCheckRule> HealthCheckRules => _healthChecker.Rules;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化诊断工具
        /// Initialize diagnostics tool
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <param name="leakTimeout">泄漏超时（默认 5 分钟）/ Leak timeout (default 5 minutes)</param>
        /// <param name="maxExceptionHistory">最大异常历史记录数（默认 100）/ Maximum exception history count (default 100)</param>
        public PoolDiagnostics(
            IPoolStatistics statistics,
            TimeSpan? leakTimeout = null,
            int maxExceptionHistory = 100)
        {
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
            _leakDetector = new PoolLeakDetector(leakTimeout, captureStackTrace: false);
            _profiler = new PoolPerformanceProfiler();
            _healthChecker = new PoolHealthChecker();
            _exceptionHistory = new List<ExceptionRecord>();
            _maxExceptionHistory = maxExceptionHistory;

            LeakDetectionEnabled = true;
            ExceptionTrackingEnabled = true;
        }

        #endregion

        #region 泄漏检测 Leak Detection

        public IReadOnlyList<ActiveObjectInfo> GetActiveObjects()
        {
            if (!LeakDetectionEnabled)
                return Array.Empty<ActiveObjectInfo>();

            return _leakDetector.GetActiveObjects();
        }

        public IReadOnlyList<ActiveObjectInfo> GetPotentialLeaks()
        {
            if (!LeakDetectionEnabled)
                return Array.Empty<ActiveObjectInfo>();

            var leaks = _leakDetector.DetectLeaks();
            return leaks.Select(leak => new ActiveObjectInfo(
                leak.Object,
                leak.GetTime,
                leak.StackTrace
            )).ToArray();
        }

        /// <summary>
        /// 记录对象获取（内部使用）
        /// Record object get (internal use)
        /// </summary>
        internal void RecordGet(object obj)
        {
            if (LeakDetectionEnabled)
            {
                _leakDetector.TrackGet(obj);
            }
        }

        /// <summary>
        /// 记录对象归还（内部使用）
        /// Record object return (internal use)
        /// </summary>
        internal void RecordReturn(object obj)
        {
            if (LeakDetectionEnabled)
            {
                _leakDetector.TrackReturn(obj);
            }
        }

        #endregion

        #region 性能分析 Performance Profiling

        public AFramework.Pool.PerformanceReport GetPerformanceReport()
        {
            var report = _profiler.GenerateReport();
            
            // 将 PoolPerformanceProfiler 的报告转换为 IPoolDiagnostics 的报告格式
            // Convert PoolPerformanceProfiler report to IPoolDiagnostics report format
            return new AFramework.Pool.PerformanceReport(
                report.AverageGetTime,
                report.MaxGetTime,
                report.MinGetTime,
                report.AverageReturnTime,
                report.GetP95Latency,
                report.GetP99Latency,
                report.SampleCount,
                report.SampleDuration
            );
        }

        public void StartProfiling()
        {
            _profiler.Start();
        }

        public void StopProfiling()
        {
            _profiler.Stop();
        }

        /// <summary>
        /// 记录获取操作性能（内部使用）
        /// Record get operation performance (internal use)
        /// </summary>
        internal void RecordGetPerformance(double elapsedMs, bool isHit)
        {
            if (ProfilingEnabled)
            {
                _profiler.RecordGet(elapsedMs, isHit);
            }
        }

        /// <summary>
        /// 记录归还操作性能（内部使用）
        /// Record return operation performance (internal use)
        /// </summary>
        internal void RecordReturnPerformance(double elapsedMs)
        {
            if (ProfilingEnabled)
            {
                _profiler.RecordReturn(elapsedMs);
            }
        }

        #endregion

        #region 健康检查 Health Check

        public HealthCheckResult PerformHealthCheck()
        {
            return _healthChecker.PerformCheck(_statistics);
        }

        public void AddHealthCheckRule(IHealthCheckRule rule)
        {
            _healthChecker.AddRule(rule);
        }

        public void RemoveHealthCheckRule(IHealthCheckRule rule)
        {
            _healthChecker.RemoveRule(rule);
        }

        #endregion

        #region 异常追踪 Exception Tracking

        public IReadOnlyList<ExceptionRecord> GetExceptionHistory()
        {
            lock (_lock)
            {
                return _exceptionHistory.ToArray();
            }
        }

        public void ClearExceptionHistory()
        {
            lock (_lock)
            {
                _exceptionHistory.Clear();
            }
        }

        /// <summary>
        /// 记录异常（内部使用）
        /// Record exception (internal use)
        /// </summary>
        internal void RecordException(Exception exception, string operation)
        {
            if (!ExceptionTrackingEnabled || exception == null)
                return;

            var record = new ExceptionRecord(
                DateTime.UtcNow,
                exception.GetType().Name,
                exception.Message,
                exception.StackTrace,
                operation
            );

            lock (_lock)
            {
                if (_exceptionHistory.Count >= _maxExceptionHistory)
                {
                    _exceptionHistory.RemoveAt(0);
                }
                _exceptionHistory.Add(record);
            }
        }

        #endregion

        #region 诊断报告 Diagnostic Report

        public DiagnosticReport GenerateReport()
        {
            var statsSnapshot = _statistics.CreateSnapshot();
            var perfReport = GetPerformanceReport();
            var healthCheck = PerformHealthCheck();
            var activeObjects = GetActiveObjects();
            var potentialLeaks = GetPotentialLeaks();

            return new DiagnosticReport(
                DateTime.UtcNow,
                statsSnapshot,
                perfReport,
                healthCheck,
                activeObjects.Count,
                potentialLeaks.Count,
                _exceptionHistory.Count
            );
        }

        public string ExportReportAsJson()
        {
            var report = GenerateReport();
            
            // 简单的 JSON 序列化（生产环境建议使用 Newtonsoft.Json 或 System.Text.Json）
            // Simple JSON serialization (recommend using Newtonsoft.Json or System.Text.Json in production)
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"generatedTime\": \"{report.GeneratedTime:O}\",");
            sb.AppendLine($"  \"activeObjectCount\": {report.ActiveObjectCount},");
            sb.AppendLine($"  \"potentialLeakCount\": {report.PotentialLeakCount},");
            sb.AppendLine($"  \"exceptionCount\": {report.ExceptionCount},");
            sb.AppendLine($"  \"healthStatus\": \"{report.HealthCheck.Status}\",");
            sb.AppendLine("  \"statistics\": {");
            sb.AppendLine($"    \"totalCreated\": {report.Statistics.TotalCreated},");
            sb.AppendLine($"    \"totalDestroyed\": {report.Statistics.TotalDestroyed},");
            sb.AppendLine($"    \"currentActive\": {report.Statistics.CurrentActive},");
            sb.AppendLine($"    \"currentIdle\": {report.Statistics.CurrentIdle},");
            sb.AppendLine($"    \"hitRate\": {report.Statistics.HitRate:F4}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"performance\": {");
            sb.AppendLine($"    \"averageGetTime\": {report.Performance.AverageGetTime:F4},");
            sb.AppendLine($"    \"maxGetTime\": {report.Performance.MaxGetTime:F4},");
            sb.AppendLine($"    \"p95Latency\": {report.Performance.GetP95Latency:F4},");
            sb.AppendLine($"    \"p99Latency\": {report.Performance.GetP99Latency:F4}");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        #endregion
    }
}
