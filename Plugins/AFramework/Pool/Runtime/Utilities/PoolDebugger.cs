// ==========================================================
// 文件名：PoolDebugger.cs
// 命名空间: AFramework.Pool.Utilities
// 依赖: System, System.Text, AFramework.Pool
// 功能: 对象池调试器，提供池状态监控和调试信息输出
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.Pool.Utilities
{
    /// <summary>
    /// 对象池调试器
    /// Pool Debugger
    /// </summary>
    /// <remarks>
    /// 提供对象池的调试功能，包括：
    /// - 状态监控和日志输出
    /// - 性能指标可视化
    /// - 泄漏检测和追踪
    /// - 调试信息格式化
    /// Provides debugging features for object pool, including:
    /// - State monitoring and logging
    /// - Performance metrics visualization
    /// - Leak detection and tracking
    /// - Debug information formatting
    /// </remarks>
    public static class PoolDebugger
    {
        #region 状态监控 State Monitoring

        /// <summary>
        /// 获取池状态摘要
        /// Get pool state summary
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <returns>状态摘要字符串 / State summary string</returns>
        public static string GetStateSummary(IObjectPool pool)
        {
            if (pool == null)
                return "Pool: null";

            var sb = new StringBuilder();
            sb.AppendLine($"=== Pool State Summary ===");
            sb.AppendLine($"Type: {pool.ObjectType?.Name ?? "Unknown"}");
            sb.AppendLine($"State: {pool.State}");
            sb.AppendLine($"Active: {pool.ActiveCount}");
            sb.AppendLine($"Available: {pool.AvailableCount}");
            sb.AppendLine($"Total: {pool.TotalCount}");
            sb.AppendLine($"Usage: {GetUsagePercentage(pool):F2}%");

            return sb.ToString();
        }

        /// <summary>
        /// 获取池统计信息摘要
        /// Get pool statistics summary
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        /// <returns>统计摘要字符串 / Statistics summary string</returns>
        public static string GetStatisticsSummary(IPoolStatistics statistics)
        {
            if (statistics == null)
                return "Statistics: null";

            var sb = new StringBuilder();
            sb.AppendLine($"=== Pool Statistics Summary ===");
            sb.AppendLine($"Created: {statistics.TotalCreated}");
            sb.AppendLine($"Destroyed: {statistics.TotalDestroyed}");
            sb.AppendLine($"Gets: {statistics.TotalGets}");
            sb.AppendLine($"Returns: {statistics.TotalReturns}");
            sb.AppendLine($"Hits: {statistics.Hits}");
            sb.AppendLine($"Misses: {statistics.Misses}");
            sb.AppendLine($"Hit Rate: {statistics.HitRate:P2}");
            sb.AppendLine($"Avg Get Time: {statistics.AverageGetTime:F4}ms");
            sb.AppendLine($"Avg Return Time: {statistics.AverageReturnTime:F4}ms");
            sb.AppendLine($"Peak Active: {statistics.PeakActive}");
            sb.AppendLine($"Peak Total: {statistics.PeakTotal}");
            sb.AppendLine($"Memory: {FormatBytes(statistics.EstimatedMemoryUsage)}");
            sb.AppendLine($"Uptime: {statistics.Uptime}");

            return sb.ToString();
        }

        /// <summary>
        /// 获取池诊断信息摘要
        /// Get pool diagnostics summary
        /// </summary>
        /// <param name="diagnostics">诊断工具 / Diagnostics</param>
        /// <returns>诊断摘要字符串 / Diagnostics summary string</returns>
        public static string GetDiagnosticsSummary(IPoolDiagnostics diagnostics)
        {
            if (diagnostics == null)
                return "Diagnostics: null";

            var sb = new StringBuilder();
            sb.AppendLine($"=== Pool Diagnostics Summary ===");
            sb.AppendLine($"Leak Detection: {(diagnostics.LeakDetectionEnabled ? "Enabled" : "Disabled")}");
            sb.AppendLine($"Profiling: {(diagnostics.ProfilingEnabled ? "Enabled" : "Disabled")}");
            sb.AppendLine($"Exception Tracking: {(diagnostics.ExceptionTrackingEnabled ? "Enabled" : "Disabled")}");

            var activeObjects = diagnostics.GetActiveObjects();
            var potentialLeaks = diagnostics.GetPotentialLeaks();
            var exceptions = diagnostics.GetExceptionHistory();

            sb.AppendLine($"Active Objects: {activeObjects.Count}");
            sb.AppendLine($"Potential Leaks: {potentialLeaks.Count}");
            sb.AppendLine($"Exceptions: {exceptions.Count}");

            if (potentialLeaks.Count > 0)
            {
                sb.AppendLine($"\n⚠️ WARNING: {potentialLeaks.Count} potential memory leaks detected!");
            }

            return sb.ToString();
        }

        #endregion

        #region 性能可视化 Performance Visualization

        /// <summary>
        /// 生成性能报告文本
        /// Generate performance report text
        /// </summary>
        /// <param name="report">性能报告 / Performance report</param>
        /// <returns>报告文本 / Report text</returns>
        public static string FormatPerformanceReport(PerformanceReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== Performance Report ===");
            sb.AppendLine($"Sample Duration: {report.SampleDuration}");
            sb.AppendLine($"Sample Count: {report.SampleCount}");
            sb.AppendLine();
            sb.AppendLine($"Get Operations:");
            sb.AppendLine($"  Avg Time: {report.AverageGetTime:F4}ms");
            sb.AppendLine($"  Min Time: {report.MinGetTime:F4}ms");
            sb.AppendLine($"  Max Time: {report.MaxGetTime:F4}ms");
            sb.AppendLine($"  P95: {report.GetP95Latency:F4}ms");
            sb.AppendLine($"  P99: {report.GetP99Latency:F4}ms");
            sb.AppendLine();
            sb.AppendLine($"Return Operations:");
            sb.AppendLine($"  Avg Time: {report.AverageReturnTime:F4}ms");

            return sb.ToString();
        }

        /// <summary>
        /// 生成健康检查报告文本
        /// Generate health check report text
        /// </summary>
        /// <param name="result">健康检查结果 / Health check result</param>
        /// <returns>报告文本 / Report text</returns>
        public static string FormatHealthCheckResult(HealthCheckResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== Health Check Result ===");
            sb.AppendLine($"Status: {GetStatusIcon(result.Status)} {result.Status}");
            sb.AppendLine($"Check Time: {result.CheckTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            if (result.Issues != null && result.Issues.Count > 0)
            {
                sb.AppendLine($"Issues ({result.Issues.Count}):");
                foreach (var issue in result.Issues)
                {
                    sb.AppendLine($"  {GetSeverityIcon(issue.Severity)} [{issue.Severity}] {issue.Description}");
                    if (!string.IsNullOrEmpty(issue.RuleName))
                    {
                        sb.AppendLine($"    💡 Rule: {issue.RuleName}");
                    }
                }
            }
            else
            {
                sb.AppendLine("✅ No issues found");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成泄漏检测报告文本
        /// Generate leak detection report text
        /// </summary>
        /// <param name="leaks">泄漏对象列表 / Leak object list</param>
        /// <returns>报告文本 / Report text</returns>
        public static string FormatLeakReport(IReadOnlyList<ActiveObjectInfo> leaks)
        {
            if (leaks == null || leaks.Count == 0)
                return "✅ No memory leaks detected";

            var sb = new StringBuilder();
            sb.AppendLine($"=== Memory Leak Report ===");
            sb.AppendLine($"⚠️ {leaks.Count} potential memory leaks detected!");
            sb.AppendLine();

            for (int i = 0; i < Math.Min(leaks.Count, 10); i++)
            {
                var leak = leaks[i];
                var duration = DateTime.UtcNow - leak.GetTime;
                var obj = leak.ObjectReference?.Target;
                sb.AppendLine($"Leak #{i + 1}:");
                sb.AppendLine($"  Object: {obj?.GetType().Name ?? "null"}");
                sb.AppendLine($"  Get Time: {leak.GetTime:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  Duration: {duration}");
                if (!string.IsNullOrEmpty(leak.StackTrace))
                {
                    sb.AppendLine($"  Stack Trace:");
                    var lines = leak.StackTrace.Split('\n');
                    foreach (var line in lines.Take(5))
                    {
                        sb.AppendLine($"    {line.Trim()}");
                    }
                }
                sb.AppendLine();
            }

            if (leaks.Count > 10)
            {
                sb.AppendLine($"... and {leaks.Count - 10} more leaks");
            }

            return sb.ToString();
        }

        #endregion

        #region 调试信息格式化 Debug Information Formatting

        /// <summary>
        /// 格式化字节大小
        /// Format bytes size
        /// </summary>
        /// <param name="bytes">字节数 / Bytes</param>
        /// <returns>格式化字符串 / Formatted string</returns>
        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:F2} {sizes[order]}";
        }

        /// <summary>
        /// 格式化时间跨度
        /// Format time span
        /// </summary>
        /// <param name="timeSpan">时间跨度 / Time span</param>
        /// <returns>格式化字符串 / Formatted string</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.TotalDays:F2} days";
            if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.TotalHours:F2} hours";
            if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.TotalMinutes:F2} minutes";
            if (timeSpan.TotalSeconds >= 1)
                return $"{timeSpan.TotalSeconds:F2} seconds";
            return $"{timeSpan.TotalMilliseconds:F2} ms";
        }

        /// <summary>
        /// 获取使用率百分比
        /// Get usage percentage
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <returns>使用率百分比 / Usage percentage</returns>
        public static float GetUsagePercentage(IObjectPool pool)
        {
            if (pool == null || pool.TotalCount == 0)
                return 0f;

            return (float)pool.ActiveCount / pool.TotalCount * 100f;
        }

        /// <summary>
        /// 生成使用率进度条
        /// Generate usage progress bar
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="width">进度条宽度 / Progress bar width</param>
        /// <returns>进度条字符串 / Progress bar string</returns>
        public static string GenerateUsageBar(IObjectPool pool, int width = 20)
        {
            if (pool == null)
                return "[" + new string(' ', width) + "]";

            float percentage = GetUsagePercentage(pool) / 100f;
            int filled = (int)(percentage * width);
            int empty = width - filled;

            string bar = "[" + new string('█', filled) + new string('░', empty) + "]";
            return $"{bar} {percentage:P0}";
        }

        #endregion

        #region 日志输出 Logging

        /// <summary>
        /// 日志级别
        /// Log Level
        /// </summary>
        public enum LogLevel
        {
            /// <summary>调试 / Debug</summary>
            Debug,
            /// <summary>信息 / Info</summary>
            Info,
            /// <summary>警告 / Warning</summary>
            Warning,
            /// <summary>错误 / Error</summary>
            Error
        }

        /// <summary>
        /// 日志输出委托
        /// Log output delegate
        /// </summary>
        public static Action<LogLevel, string> LogHandler { get; set; }

        /// <summary>
        /// 输出调试日志
        /// Output debug log
        /// </summary>
        /// <param name="message">消息 / Message</param>
        public static void LogDebug(string message)
        {
            LogHandler?.Invoke(LogLevel.Debug, $"[Pool Debug] {message}");
        }

        /// <summary>
        /// 输出信息日志
        /// Output info log
        /// </summary>
        /// <param name="message">消息 / Message</param>
        public static void LogInfo(string message)
        {
            LogHandler?.Invoke(LogLevel.Info, $"[Pool Info] {message}");
        }

        /// <summary>
        /// 输出警告日志
        /// Output warning log
        /// </summary>
        /// <param name="message">消息 / Message</param>
        public static void LogWarning(string message)
        {
            LogHandler?.Invoke(LogLevel.Warning, $"[Pool Warning] {message}");
        }

        /// <summary>
        /// 输出错误日志
        /// Output error log
        /// </summary>
        /// <param name="message">消息 / Message</param>
        public static void LogError(string message)
        {
            LogHandler?.Invoke(LogLevel.Error, $"[Pool Error] {message}");
        }

        /// <summary>
        /// 输出池状态日志
        /// Output pool state log
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        public static void LogPoolState(IObjectPool pool)
        {
            if (pool == null)
            {
                LogWarning("Pool is null");
                return;
            }

            LogInfo(GetStateSummary(pool));
        }

        /// <summary>
        /// 输出池统计日志
        /// Output pool statistics log
        /// </summary>
        /// <param name="statistics">统计信息 / Statistics</param>
        public static void LogPoolStatistics(IPoolStatistics statistics)
        {
            if (statistics == null)
            {
                LogWarning("Statistics is null");
                return;
            }

            LogInfo(GetStatisticsSummary(statistics));
        }

        #endregion

        #region 辅助方法 Helper Methods

        /// <summary>
        /// 获取状态图标
        /// Get status icon
        /// </summary>
        private static string GetStatusIcon(PoolHealthStatus status)
        {
            return status switch
            {
                PoolHealthStatus.Healthy => "✅",
                PoolHealthStatus.Warning => "⚠️",
                PoolHealthStatus.Unhealthy => "⚠️",
                PoolHealthStatus.Critical => "❌",
                _ => "❓"
            };
        }

        /// <summary>
        /// 获取严重性图标
        /// Get severity icon
        /// </summary>
        private static string GetSeverityIcon(HealthIssueSeverity severity)
        {
            return severity switch
            {
                HealthIssueSeverity.Info => "ℹ️",
                HealthIssueSeverity.Warning => "⚠️",
                HealthIssueSeverity.Error => "❌",
                HealthIssueSeverity.Critical => "🔥",
                _ => "❓"
            };
        }

        #endregion

        #region 比较和分析 Comparison and Analysis

        /// <summary>
        /// 比较两个统计快照
        /// Compare two statistics snapshots
        /// </summary>
        /// <param name="before">之前的快照 / Before snapshot</param>
        /// <param name="after">之后的快照 / After snapshot</param>
        /// <returns>比较报告 / Comparison report</returns>
        public static string CompareSnapshots(PoolStatisticsSnapshot before, PoolStatisticsSnapshot after)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== Statistics Comparison ===");
            sb.AppendLine($"Time Range: {before.SnapshotTime:HH:mm:ss} -> {after.SnapshotTime:HH:mm:ss}");
            sb.AppendLine($"Duration: {FormatTimeSpan(after.SnapshotTime - before.SnapshotTime)}");
            sb.AppendLine();

            sb.AppendLine($"Object Counts:");
            sb.AppendLine($"  Created: {before.TotalCreated} -> {after.TotalCreated} ({GetDelta(before.TotalCreated, after.TotalCreated)})");
            sb.AppendLine($"  Destroyed: {before.TotalDestroyed} -> {after.TotalDestroyed} ({GetDelta(before.TotalDestroyed, after.TotalDestroyed)})");
            sb.AppendLine($"  Active: {before.CurrentActive} -> {after.CurrentActive} ({GetDelta(before.CurrentActive, after.CurrentActive)})");
            sb.AppendLine($"  Idle: {before.CurrentIdle} -> {after.CurrentIdle} ({GetDelta(before.CurrentIdle, after.CurrentIdle)})");
            sb.AppendLine();

            sb.AppendLine($"Operations:");
            sb.AppendLine($"  Gets: {before.TotalGets} -> {after.TotalGets} ({GetDelta(before.TotalGets, after.TotalGets)})");
            sb.AppendLine($"  Returns: {before.TotalReturns} -> {after.TotalReturns} ({GetDelta(before.TotalReturns, after.TotalReturns)})");
            sb.AppendLine($"  Hit Rate: {before.HitRate:P2} -> {after.HitRate:P2} ({GetDeltaPercent(before.HitRate, after.HitRate)})");
            sb.AppendLine();

            sb.AppendLine($"Performance:");
            sb.AppendLine($"  Avg Get Time: {before.AverageGetTime:F4}ms -> {after.AverageGetTime:F4}ms ({GetDeltaPercent(before.AverageGetTime, after.AverageGetTime)})");
            sb.AppendLine($"  Avg Return Time: {before.AverageReturnTime:F4}ms -> {after.AverageReturnTime:F4}ms ({GetDeltaPercent(before.AverageReturnTime, after.AverageReturnTime)})");

            return sb.ToString();
        }

        /// <summary>
        /// 获取增量字符串
        /// Get delta string
        /// </summary>
        private static string GetDelta(long before, long after)
        {
            long delta = after - before;
            return delta >= 0 ? $"+{delta}" : delta.ToString();
        }

        /// <summary>
        /// 获取增量字符串
        /// Get delta string
        /// </summary>
        private static string GetDelta(int before, int after)
        {
            int delta = after - before;
            return delta >= 0 ? $"+{delta}" : delta.ToString();
        }

        /// <summary>
        /// 获取百分比增量字符串
        /// Get percentage delta string
        /// </summary>
        private static string GetDeltaPercent(double before, double after)
        {
            if (before == 0)
                return "N/A";

            double delta = (after - before) / before * 100;
            return delta >= 0 ? $"+{delta:F2}%" : $"{delta:F2}%";
        }

        #endregion
    }
}
