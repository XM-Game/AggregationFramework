// ==========================================================
// 文件名：IPoolDiagnostics.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 定义对象池诊断接口，提供池的调试和诊断功能
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池诊断接口
    /// 提供池的调试和诊断功能
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 单一职责原则：仅负责诊断和调试
    /// - 接口隔离原则：诊断功能独立，不影响池性能
    /// - 开闭原则：可扩展诊断规则
    /// 
    /// 诊断功能：
    /// - 泄漏检测：追踪未归还的对象
    /// - 性能分析：识别性能瓶颈
    /// - 健康检查：评估池的健康状态
    /// - 异常追踪：记录和分析异常
    /// 
    /// 使用场景：
    /// - 开发调试：定位池使用问题
    /// - 性能优化：分析池性能瓶颈
    /// - 生产监控：实时监控池健康状态
    /// - 问题排查：追踪异常和泄漏
    /// 
    /// 注意：
    /// - 诊断功能会增加性能开销，建议仅在开发环境启用
    /// - 生产环境可选择性启用轻量级诊断
    /// </remarks>
    public interface IPoolDiagnostics
    {
        #region 泄漏检测

        /// <summary>
        /// 获取是否启用泄漏检测
        /// </summary>
        bool LeakDetectionEnabled { get; set; }

        /// <summary>
        /// 获取泄漏检测超时时间（秒）
        /// </summary>
        /// <remarks>
        /// 对象获取后超过此时间未归还，视为潜在泄漏
        /// </remarks>
        float LeakDetectionTimeout { get; set; }

        /// <summary>
        /// 获取所有活跃对象的信息
        /// </summary>
        /// <returns>活跃对象信息列表</returns>
        IReadOnlyList<ActiveObjectInfo> GetActiveObjects();

        /// <summary>
        /// 获取可能泄漏的对象信息
        /// </summary>
        /// <returns>泄漏对象信息列表</returns>
        IReadOnlyList<ActiveObjectInfo> GetPotentialLeaks();

        #endregion

        #region 性能分析

        /// <summary>
        /// 获取是否启用性能分析
        /// </summary>
        bool ProfilingEnabled { get; set; }

        /// <summary>
        /// 获取性能分析报告
        /// </summary>
        /// <returns>性能分析报告</returns>
        PerformanceReport GetPerformanceReport();

        /// <summary>
        /// 开始性能采样
        /// </summary>
        void StartProfiling();

        /// <summary>
        /// 停止性能采样
        /// </summary>
        void StopProfiling();

        #endregion

        #region 健康检查

        /// <summary>
        /// 执行健康检查
        /// </summary>
        /// <returns>健康检查结果</returns>
        HealthCheckResult PerformHealthCheck();

        /// <summary>
        /// 获取健康检查规则列表
        /// </summary>
        IReadOnlyList<IHealthCheckRule> HealthCheckRules { get; }

        /// <summary>
        /// 添加自定义健康检查规则
        /// </summary>
        /// <param name="rule">健康检查规则</param>
        void AddHealthCheckRule(IHealthCheckRule rule);

        /// <summary>
        /// 移除健康检查规则
        /// </summary>
        /// <param name="rule">健康检查规则</param>
        void RemoveHealthCheckRule(IHealthCheckRule rule);

        #endregion

        #region 异常追踪

        /// <summary>
        /// 获取是否启用异常追踪
        /// </summary>
        bool ExceptionTrackingEnabled { get; set; }

        /// <summary>
        /// 获取异常历史记录
        /// </summary>
        /// <returns>异常记录列表</returns>
        IReadOnlyList<ExceptionRecord> GetExceptionHistory();

        /// <summary>
        /// 清空异常历史记录
        /// </summary>
        void ClearExceptionHistory();

        #endregion

        #region 诊断报告

        /// <summary>
        /// 生成完整的诊断报告
        /// </summary>
        /// <returns>诊断报告</returns>
        DiagnosticReport GenerateReport();

        /// <summary>
        /// 导出诊断报告为 JSON 格式
        /// </summary>
        /// <returns>JSON 字符串</returns>
        string ExportReportAsJson();

        #endregion
    }

    /// <summary>
    /// 活跃对象信息
    /// </summary>
    public readonly struct ActiveObjectInfo
    {
        /// <summary>
        /// 对象引用（弱引用，避免阻止 GC）
        /// </summary>
        public readonly WeakReference ObjectReference;

        /// <summary>
        /// 对象获取时间
        /// </summary>
        public readonly DateTime GetTime;

        /// <summary>
        /// 对象活跃时长
        /// </summary>
        public readonly TimeSpan ActiveDuration;

        /// <summary>
        /// 获取时的调用堆栈（可选，性能开销大）
        /// </summary>
        public readonly string StackTrace;

        /// <summary>
        /// 是否为潜在泄漏
        /// </summary>
        public readonly bool IsPotentialLeak;

        /// <summary>
        /// 初始化活跃对象信息
        /// </summary>
        public ActiveObjectInfo(
            object obj,
            DateTime getTime,
            string stackTrace = null)
        {
            ObjectReference = new WeakReference(obj);
            GetTime = getTime;
            ActiveDuration = DateTime.Now - getTime;
            StackTrace = stackTrace;
            IsPotentialLeak = false;
        }
    }

    /// <summary>
    /// 性能分析报告
    /// </summary>
    public readonly struct PerformanceReport
    {
        /// <summary>
        /// 平均获取时间（毫秒）
        /// </summary>
        public readonly double AverageGetTime;

        /// <summary>
        /// 最大获取时间（毫秒）
        /// </summary>
        public readonly double MaxGetTime;

        /// <summary>
        /// 最小获取时间（毫秒）
        /// </summary>
        public readonly double MinGetTime;

        /// <summary>
        /// 平均归还时间（毫秒）
        /// </summary>
        public readonly double AverageReturnTime;

        /// <summary>
        /// 获取操作的 P95 延迟（毫秒）
        /// </summary>
        public readonly double GetP95Latency;

        /// <summary>
        /// 获取操作的 P99 延迟（毫秒）
        /// </summary>
        public readonly double GetP99Latency;

        /// <summary>
        /// 采样数量
        /// </summary>
        public readonly int SampleCount;

        /// <summary>
        /// 采样时长
        /// </summary>
        public readonly TimeSpan SampleDuration;

        /// <summary>
        /// 初始化性能报告
        /// Initialize performance report
        /// </summary>
        public PerformanceReport(
            double averageGetTime,
            double maxGetTime,
            double minGetTime,
            double averageReturnTime,
            double getP95Latency,
            double getP99Latency,
            int sampleCount,
            TimeSpan sampleDuration)
        {
            AverageGetTime = averageGetTime;
            MaxGetTime = maxGetTime;
            MinGetTime = minGetTime;
            AverageReturnTime = averageReturnTime;
            GetP95Latency = getP95Latency;
            GetP99Latency = getP99Latency;
            SampleCount = sampleCount;
            SampleDuration = sampleDuration;
        }
    }

    /// <summary>
    /// 健康检查结果
    /// </summary>
    public readonly struct HealthCheckResult
    {
        /// <summary>
        /// 健康状态
        /// </summary>
        public readonly PoolHealthStatus Status;

        /// <summary>
        /// 检查时间
        /// </summary>
        public readonly DateTime CheckTime;

        /// <summary>
        /// 问题列表
        /// </summary>
        public readonly IReadOnlyList<HealthIssue> Issues;

        /// <summary>
        /// 建议列表
        /// </summary>
        public readonly IReadOnlyList<string> Recommendations;

        /// <summary>
        /// 初始化健康检查结果
        /// </summary>
        public HealthCheckResult(
            PoolHealthStatus status,
            DateTime checkTime,
            IReadOnlyList<HealthIssue> issues,
            IReadOnlyList<string> recommendations)
        {
            Status = status;
            CheckTime = checkTime;
            Issues = issues;
            Recommendations = recommendations;
        }
    }

    /// <summary>
    /// 健康问题
    /// </summary>
    public readonly struct HealthIssue
    {
        /// <summary>
        /// 问题严重程度
        /// </summary>
        public readonly HealthIssueSeverity Severity;

        /// <summary>
        /// 问题描述
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// 问题来源规则
        /// </summary>
        public readonly string RuleName;

        /// <summary>
        /// 初始化健康问题
        /// </summary>
        public HealthIssue(HealthIssueSeverity severity, string description, string ruleName)
        {
            Severity = severity;
            Description = description;
            RuleName = ruleName;
        }
    }

    /// <summary>
    /// 健康问题严重程度
    /// </summary>
    public enum HealthIssueSeverity
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info = 0,

        /// <summary>
        /// 警告
        /// </summary>
        Warning = 1,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 2,

        /// <summary>
        /// 严重错误
        /// </summary>
        Critical = 3,
    }

    /// <summary>
    /// 健康检查规则接口
    /// </summary>
    public interface IHealthCheckRule
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 规则描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 执行健康检查
        /// </summary>
        /// <param name="statistics">池统计信息</param>
        /// <returns>健康问题列表</returns>
        IEnumerable<HealthIssue> Check(IPoolStatistics statistics);
    }

    /// <summary>
    /// 异常记录
    /// </summary>
    public readonly struct ExceptionRecord
    {
        /// <summary>
        /// 异常时间
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// 异常类型
        /// </summary>
        public readonly string ExceptionType;

        /// <summary>
        /// 异常消息
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// 异常堆栈
        /// </summary>
        public readonly string StackTrace;

        /// <summary>
        /// 操作类型（Get/Return/Create/Destroy）
        /// </summary>
        public readonly string Operation;

        /// <summary>
        /// 初始化异常记录
        /// </summary>
        public ExceptionRecord(
            DateTime timestamp,
            string exceptionType,
            string message,
            string stackTrace,
            string operation)
        {
            Timestamp = timestamp;
            ExceptionType = exceptionType;
            Message = message;
            StackTrace = stackTrace;
            Operation = operation;
        }
    }

    /// <summary>
    /// 诊断报告
    /// </summary>
    public readonly struct DiagnosticReport
    {
        /// <summary>
        /// 报告生成时间
        /// </summary>
        public readonly DateTime GeneratedTime;

        /// <summary>
        /// 统计信息快照
        /// </summary>
        public readonly PoolStatisticsSnapshot Statistics;

        /// <summary>
        /// 性能报告
        /// </summary>
        public readonly PerformanceReport Performance;

        /// <summary>
        /// 健康检查结果
        /// </summary>
        public readonly HealthCheckResult HealthCheck;

        /// <summary>
        /// 活跃对象数量
        /// </summary>
        public readonly int ActiveObjectCount;

        /// <summary>
        /// 潜在泄漏数量
        /// </summary>
        public readonly int PotentialLeakCount;

        /// <summary>
        /// 异常数量
        /// </summary>
        public readonly int ExceptionCount;

        /// <summary>
        /// 初始化诊断报告
        /// </summary>
        public DiagnosticReport(
            DateTime generatedTime,
            PoolStatisticsSnapshot statistics,
            PerformanceReport performance,
            HealthCheckResult healthCheck,
            int activeObjectCount,
            int potentialLeakCount,
            int exceptionCount)
        {
            GeneratedTime = generatedTime;
            Statistics = statistics;
            Performance = performance;
            HealthCheck = healthCheck;
            ActiveObjectCount = activeObjectCount;
            PotentialLeakCount = potentialLeakCount;
            ExceptionCount = exceptionCount;
        }
    }
}
