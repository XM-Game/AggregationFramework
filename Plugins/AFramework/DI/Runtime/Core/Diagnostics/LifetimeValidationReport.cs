// ==========================================================
// 文件名：LifetimeValidationReport.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Text
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.DI
{
    /// <summary>
    /// 生命周期验证报告
    /// <para>验证容器注册的生命周期配置是否正确</para>
    /// </summary>
    public sealed class LifetimeValidationReport
    {
        #region 属性

        /// <summary>
        /// 验证是否通过
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// 错误列表
        /// </summary>
        public List<LifetimeIssue> Errors { get; } = new();

        /// <summary>
        /// 警告列表
        /// </summary>
        public List<LifetimeIssue> Warnings { get; } = new();

        /// <summary>
        /// 信息列表
        /// </summary>
        public List<LifetimeIssue> Infos { get; } = new();

        /// <summary>
        /// 验证时间
        /// </summary>
        public DateTime ValidatedAt { get; } = DateTime.Now;

        #endregion

        #region 方法

        /// <summary>
        /// 添加问题
        /// </summary>
        public void AddIssue(LifetimeIssue issue)
        {
            switch (issue.Severity)
            {
                case IssueSeverity.Error:
                    Errors.Add(issue);
                    break;
                case IssueSeverity.Warning:
                    Warnings.Add(issue);
                    break;
                case IssueSeverity.Info:
                    Infos.Add(issue);
                    break;
            }
        }

        /// <summary>
        /// 生成文本报告
        /// </summary>
        public string GenerateReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== 生命周期验证报告 ===");
            sb.AppendLine($"验证时间: {ValidatedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"状态: {(IsValid ? "✓ 通过" : "✗ 失败")}");
            sb.AppendLine();

            if (Errors.Count > 0)
            {
                sb.AppendLine($"错误 ({Errors.Count}):");
                foreach (var error in Errors)
                {
                    sb.AppendLine($"  [ERROR] {error.Message}");
                    if (!string.IsNullOrEmpty(error.Suggestion))
                        sb.AppendLine($"          建议: {error.Suggestion}");
                }
                sb.AppendLine();
            }

            if (Warnings.Count > 0)
            {
                sb.AppendLine($"警告 ({Warnings.Count}):");
                foreach (var warning in Warnings)
                {
                    sb.AppendLine($"  [WARN] {warning.Message}");
                    if (!string.IsNullOrEmpty(warning.Suggestion))
                        sb.AppendLine($"         建议: {warning.Suggestion}");
                }
                sb.AppendLine();
            }

            if (Infos.Count > 0)
            {
                sb.AppendLine($"信息 ({Infos.Count}):");
                foreach (var info in Infos)
                {
                    sb.AppendLine($"  [INFO] {info.Message}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 如果验证失败则抛出异常
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                var messages = Errors.Select(e => e.Message);
                throw new RegistrationException(
                    "生命周期验证失败:\n" + string.Join("\n", messages));
            }
        }

        #endregion
    }

    /// <summary>
    /// 生命周期验证器
    /// </summary>
    public static class LifetimeValidator
    {
        /// <summary>
        /// 验证注册的生命周期配置
        /// </summary>
        public static LifetimeValidationReport Validate(IEnumerable<IRegistration> registrations)
        {
            var report = new LifetimeValidationReport();
            var graphBuilder = new DependencyGraphBuilder();

            graphBuilder.Build(registrations);
            var issues = graphBuilder.DetectLifetimeIssues();

            foreach (var issue in issues)
            {
                report.AddIssue(issue);
            }

            return report;
        }
    }
}
