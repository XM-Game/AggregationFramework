// ==========================================================
// 文件名：PoolHealthChecker.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, System.Collections.Generic, AFramework.Pool
// 功能: 对象池健康检查器
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池健康检查器
    /// Pool Health Checker
    /// </summary>
    /// <remarks>
    /// 执行一系列健康检查规则，评估池的健康状态
    /// Executes a series of health check rules to assess pool health status
    /// </remarks>
    public class PoolHealthChecker
    {
        #region 字段 Fields

        private readonly List<IHealthCheckRule> _rules;
        private readonly object _lock = new object();

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取健康检查规则列表
        /// Get health check rules list
        /// </summary>
        public IReadOnlyList<IHealthCheckRule> Rules
        {
            get
            {
                lock (_lock)
                {
                    return _rules.ToArray();
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化健康检查器
        /// Initialize health checker
        /// </summary>
        public PoolHealthChecker()
        {
            _rules = new List<IHealthCheckRule>();
            
            // 添加默认规则
            // Add default rules
            AddDefaultRules();
        }

        #endregion

        #region 规则管理 Rule Management

        /// <summary>
        /// 添加健康检查规则
        /// Add health check rule
        /// </summary>
        public void AddRule(IHealthCheckRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            lock (_lock)
            {
                _rules.Add(rule);
            }
        }

        /// <summary>
        /// 移除健康检查规则
        /// Remove health check rule
        /// </summary>
        public bool RemoveRule(IHealthCheckRule rule)
        {
            if (rule == null) return false;

            lock (_lock)
            {
                return _rules.Remove(rule);
            }
        }

        /// <summary>
        /// 清空所有规则
        /// Clear all rules
        /// </summary>
        public void ClearRules()
        {
            lock (_lock)
            {
                _rules.Clear();
            }
        }

        /// <summary>
        /// 添加默认规则
        /// Add default rules
        /// </summary>
        private void AddDefaultRules()
        {
            AddRule(new LowHitRateRule());
            AddRule(new HighUsageRateRule());
            AddRule(new MemoryLeakRule());
            AddRule(new ExcessiveCapacityRule());
        }

        #endregion

        #region 健康检查 Health Check

        /// <summary>
        /// 执行健康检查
        /// Perform health check
        /// </summary>
        public HealthCheckResult PerformCheck(IPoolStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));

            var issues = new List<HealthIssue>();
            var recommendations = new List<string>();

            lock (_lock)
            {
                foreach (var rule in _rules)
                {
                    try
                    {
                        var ruleIssues = rule.Check(statistics);
                        if (ruleIssues != null)
                        {
                            issues.AddRange(ruleIssues);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 规则执行失败，记录为警告
                        // Rule execution failed, record as warning
                        issues.Add(new HealthIssue(
                            HealthIssueSeverity.Warning,
                            $"健康检查规则执行失败 Health check rule failed: {rule.Name} - {ex.Message}",
                            rule.Name
                        ));
                    }
                }
            }

            // 确定整体健康状态
            // Determine overall health status
            var status = DetermineHealthStatus(issues);

            // 生成建议
            // Generate recommendations
            recommendations.AddRange(GenerateRecommendations(issues, statistics));

            return new HealthCheckResult(
                status,
                DateTime.UtcNow,
                issues.AsReadOnly(),
                recommendations.AsReadOnly()
            );
        }

        /// <summary>
        /// 确定健康状态
        /// Determine health status
        /// </summary>
        private PoolHealthStatus DetermineHealthStatus(List<HealthIssue> issues)
        {
            if (issues.Count == 0)
                return PoolHealthStatus.Healthy;

            var maxSeverity = issues.Max(i => i.Severity);

            return maxSeverity switch
            {
                HealthIssueSeverity.Critical => PoolHealthStatus.Critical,
                HealthIssueSeverity.Error => PoolHealthStatus.Unhealthy,
                HealthIssueSeverity.Warning => PoolHealthStatus.Warning,
                _ => PoolHealthStatus.Healthy
            };
        }

        /// <summary>
        /// 生成建议
        /// Generate recommendations
        /// </summary>
        private List<string> GenerateRecommendations(List<HealthIssue> issues, IPoolStatistics statistics)
        {
            var recommendations = new List<string>();

            // 根据问题类型生成建议
            // Generate recommendations based on issue types
            foreach (var issue in issues)
            {
                if (issue.Description.Contains("命中率") || issue.Description.Contains("hit rate"))
                {
                    recommendations.Add("建议增加池的初始容量或预热对象数量 Recommend increasing initial capacity or warmup count");
                }
                else if (issue.Description.Contains("使用率") || issue.Description.Contains("usage rate"))
                {
                    recommendations.Add("建议增加池的最大容量 Recommend increasing maximum capacity");
                }
                else if (issue.Description.Contains("内存") || issue.Description.Contains("memory"))
                {
                    recommendations.Add("建议启用定期清理策略 Recommend enabling periodic cleanup policy");
                }
                else if (issue.Description.Contains("容量") || issue.Description.Contains("capacity"))
                {
                    recommendations.Add("建议减少池的初始容量 Recommend reducing initial capacity");
                }
            }

            return recommendations.Distinct().ToList();
        }

        #endregion
    }

    #region 默认健康检查规则 Default Health Check Rules

    /// <summary>
    /// 低命中率规则
    /// Low Hit Rate Rule
    /// </summary>
    internal class LowHitRateRule : IHealthCheckRule
    {
        public string Name => "LowHitRate";
        public string Description => "检查池的命中率是否过低 Check if pool hit rate is too low";

        public IEnumerable<HealthIssue> Check(IPoolStatistics statistics)
        {
            var hitRate = statistics.HitRate;

            if (hitRate < 0.3 && statistics.TotalGets > 100)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Error,
                    $"命中率过低 Hit rate too low: {hitRate:P2}",
                    Name
                );
            }
            else if (hitRate < 0.5 && statistics.TotalGets > 100)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Warning,
                    $"命中率较低 Hit rate low: {hitRate:P2}",
                    Name
                );
            }
        }
    }

    /// <summary>
    /// 高使用率规则
    /// High Usage Rate Rule
    /// </summary>
    internal class HighUsageRateRule : IHealthCheckRule
    {
        public string Name => "HighUsageRate";
        public string Description => "检查池的使用率是否过高 Check if pool usage rate is too high";

        public IEnumerable<HealthIssue> Check(IPoolStatistics statistics)
        {
            var usageRate = statistics.CurrentTotal > 0
                ? (double)statistics.CurrentActive / statistics.CurrentTotal
                : 0.0;

            if (usageRate > 0.95)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Error,
                    $"使用率过高 Usage rate too high: {usageRate:P2}",
                    Name
                );
            }
            else if (usageRate > 0.85)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Warning,
                    $"使用率较高 Usage rate high: {usageRate:P2}",
                    Name
                );
            }
        }
    }

    /// <summary>
    /// 内存泄漏规则
    /// Memory Leak Rule
    /// </summary>
    internal class MemoryLeakRule : IHealthCheckRule
    {
        public string Name => "MemoryLeak";
        public string Description => "检查是否存在内存泄漏迹象 Check for signs of memory leak";

        public IEnumerable<HealthIssue> Check(IPoolStatistics statistics)
        {
            // 检查活跃对象数是否持续增长
            // Check if active object count is continuously growing
            var activeCount = statistics.CurrentActive;
            var totalCount = statistics.CurrentTotal;

            if (activeCount > totalCount * 0.9 && statistics.TotalReturns < statistics.TotalGets * 0.5)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Critical,
                    $"疑似内存泄漏 Suspected memory leak: 活跃对象 Active={activeCount}, 归还率 Return rate={statistics.TotalReturns}/{statistics.TotalGets}",
                    Name
                );
            }
        }
    }

    /// <summary>
    /// 过度容量规则
    /// Excessive Capacity Rule
    /// </summary>
    internal class ExcessiveCapacityRule : IHealthCheckRule
    {
        public string Name => "ExcessiveCapacity";
        public string Description => "检查池容量是否过大 Check if pool capacity is excessive";

        public IEnumerable<HealthIssue> Check(IPoolStatistics statistics)
        {
            var idleCount = statistics.CurrentIdle;
            var totalCount = statistics.CurrentTotal;

            if (idleCount > 100 && idleCount > totalCount * 0.8)
            {
                yield return new HealthIssue(
                    HealthIssueSeverity.Info,
                    $"空闲对象过多 Too many idle objects: {idleCount}/{totalCount}",
                    Name
                );
            }
        }
    }

    #endregion
}
