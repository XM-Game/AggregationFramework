// ==========================================================
// 文件名：DiagnosticsInfo.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 诊断信息数据
    /// <para>存储容器的诊断和调试信息</para>
    /// </summary>
    public sealed class DiagnosticsInfo
    {
        #region 属性

        /// <summary>
        /// 容器名称
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 注册信息列表
        /// </summary>
        public List<RegistrationDiagnostics> Registrations { get; } = new();

        /// <summary>
        /// 解析记录列表
        /// </summary>
        public List<ResolutionRecord> ResolutionRecords { get; } = new();

        /// <summary>
        /// 生命周期验证问题
        /// </summary>
        public List<LifetimeIssue> LifetimeIssues { get; } = new();

        /// <summary>
        /// 总注册数量
        /// </summary>
        public int TotalRegistrations => Registrations.Count;

        /// <summary>
        /// 单例数量
        /// </summary>
        public int SingletonCount { get; set; }

        /// <summary>
        /// 作用域数量
        /// </summary>
        public int ScopedCount { get; set; }

        /// <summary>
        /// 瞬态数量
        /// </summary>
        public int TransientCount { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 生成摘要报告
        /// </summary>
        public string GenerateSummary()
        {
            return $"容器诊断信息 [{ContainerName}]\n" +
                   $"创建时间: {CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                   $"总注册数: {TotalRegistrations}\n" +
                   $"  - 单例: {SingletonCount}\n" +
                   $"  - 作用域: {ScopedCount}\n" +
                   $"  - 瞬态: {TransientCount}\n" +
                   $"解析记录: {ResolutionRecords.Count}\n" +
                   $"生命周期问题: {LifetimeIssues.Count}";
        }

        #endregion
    }

    /// <summary>
    /// 注册诊断信息
    /// </summary>
    public sealed class RegistrationDiagnostics
    {
        /// <summary>服务类型</summary>
        public Type ServiceType { get; set; }

        /// <summary>实现类型</summary>
        public Type ImplementationType { get; set; }

        /// <summary>生命周期</summary>
        public Lifetime Lifetime { get; set; }

        /// <summary>注册键值</summary>
        public object Key { get; set; }

        /// <summary>是否为工厂注册</summary>
        public bool IsFactory { get; set; }

        /// <summary>是否为实例注册</summary>
        public bool IsInstance { get; set; }

        /// <summary>依赖类型列表</summary>
        public List<Type> Dependencies { get; } = new();

        /// <summary>注册来源（安装器名称等）</summary>
        public string Source { get; set; }

        public override string ToString()
        {
            var impl = ImplementationType?.Name ?? "Factory/Instance";
            return $"{ServiceType.Name} -> {impl} [{Lifetime}]";
        }
    }

    /// <summary>
    /// 解析记录
    /// </summary>
    public sealed class ResolutionRecord
    {
        /// <summary>请求的服务类型</summary>
        public Type ServiceType { get; set; }

        /// <summary>解析时间</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>解析耗时（毫秒）</summary>
        public double DurationMs { get; set; }

        /// <summary>是否成功</summary>
        public bool Success { get; set; }

        /// <summary>错误信息</summary>
        public string Error { get; set; }

        /// <summary>是否命中缓存</summary>
        public bool CacheHit { get; set; }

        /// <summary>调用堆栈</summary>
        public string CallStack { get; set; }
    }

    /// <summary>
    /// 生命周期问题
    /// </summary>
    public sealed class LifetimeIssue
    {
        /// <summary>问题严重级别</summary>
        public IssueSeverity Severity { get; set; }

        /// <summary>问题类型</summary>
        public LifetimeIssueType IssueType { get; set; }

        /// <summary>相关服务类型</summary>
        public Type ServiceType { get; set; }

        /// <summary>依赖的服务类型</summary>
        public Type DependencyType { get; set; }

        /// <summary>问题描述</summary>
        public string Message { get; set; }

        /// <summary>建议修复方案</summary>
        public string Suggestion { get; set; }

        public override string ToString()
        {
            return $"[{Severity}] {IssueType}: {Message}";
        }
    }

    /// <summary>
    /// 问题严重级别
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>信息</summary>
        Info,
        /// <summary>警告</summary>
        Warning,
        /// <summary>错误</summary>
        Error
    }

    /// <summary>
    /// 生命周期问题类型
    /// </summary>
    public enum LifetimeIssueType
    {
        /// <summary>单例依赖作用域服务（俘获依赖）</summary>
        CaptiveDependency,
        /// <summary>循环依赖</summary>
        CircularDependency,
        /// <summary>缺失依赖</summary>
        MissingDependency,
        /// <summary>重复注册</summary>
        DuplicateRegistration,
        /// <summary>生命周期不匹配</summary>
        LifetimeMismatch
    }
}
