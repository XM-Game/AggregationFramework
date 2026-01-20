// ==========================================================
// 文件名：PoolWarningLevel.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 池警告级别枚举，用于诊断和监控
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 池警告级别
    /// Pool Warning Level
    /// 
    /// <para>定义对象池的警告级别，用于诊断和监控</para>
    /// <para>Defines warning levels for object pools, used for diagnostics and monitoring</para>
    /// </summary>
    /// <remarks>
    /// 警告级别用于：
    /// - 性能监控
    /// - 容量预警
    /// - 异常使用检测
    /// - 资源泄漏提示
    /// </remarks>
    [Flags]
    public enum PoolWarningLevel
    {
        /// <summary>
        /// 无警告
        /// No warning
        /// </summary>
        None = 0,

        /// <summary>
        /// 低级警告 - 信息性提示
        /// Low warning - Informational hint
        /// 
        /// <para>例如：池使用率较低、建议收缩</para>
        /// <para>Example: Low pool utilization, suggest shrinking</para>
        /// </summary>
        Low = 1 << 0,

        /// <summary>
        /// 中级警告 - 需要关注
        /// Medium warning - Needs attention
        /// 
        /// <para>例如：池使用率接近上限、频繁扩容</para>
        /// <para>Example: Pool utilization near limit, frequent expansion</para>
        /// </summary>
        Medium = 1 << 1,

        /// <summary>
        /// 高级警告 - 需要立即处理
        /// High warning - Needs immediate action
        /// 
        /// <para>例如：池已满、对象泄漏、性能严重下降</para>
        /// <para>Example: Pool full, object leak, severe performance degradation</para>
        /// </summary>
        High = 1 << 2,

        /// <summary>
        /// 严重警告 - 系统可能崩溃
        /// Critical warning - System may crash
        /// 
        /// <para>例如：内存溢出、死锁、严重泄漏</para>
        /// <para>Example: Memory overflow, deadlock, severe leak</para>
        /// </summary>
        Critical = 1 << 3,

        /// <summary>
        /// 所有警告级别
        /// All warning levels
        /// </summary>
        All = Low | Medium | High | Critical
    }

    /// <summary>
    /// 池警告级别扩展方法
    /// Pool Warning Level Extension Methods
    /// </summary>
    public static class PoolWarningLevelExtensions
    {
        /// <summary>
        /// 检查是否包含指定警告级别
        /// Check if contains specified warning level
        /// </summary>
        /// <param name="level">当前级别 / Current level</param>
        /// <param name="target">目标级别 / Target level</param>
        /// <returns>是否包含 / Whether contains</returns>
        public static bool HasLevel(this PoolWarningLevel level, PoolWarningLevel target)
        {
            return (level & target) == target;
        }

        /// <summary>
        /// 获取警告级别的严重程度（0-3）
        /// Get severity of warning level (0-3)
        /// </summary>
        /// <param name="level">警告级别 / Warning level</param>
        /// <returns>严重程度 / Severity</returns>
        public static int GetSeverity(this PoolWarningLevel level)
        {
            if (level.HasLevel(PoolWarningLevel.Critical))
                return 3;
            if (level.HasLevel(PoolWarningLevel.High))
                return 2;
            if (level.HasLevel(PoolWarningLevel.Medium))
                return 1;
            if (level.HasLevel(PoolWarningLevel.Low))
                return 0;
            return -1;
        }

        /// <summary>
        /// 获取警告级别的显示名称
        /// Get display name of warning level
        /// </summary>
        /// <param name="level">警告级别 / Warning level</param>
        /// <returns>显示名称 / Display name</returns>
        public static string GetDisplayName(this PoolWarningLevel level)
        {
            return level switch
            {
                PoolWarningLevel.None => "无警告 / No Warning",
                PoolWarningLevel.Low => "低级警告 / Low Warning",
                PoolWarningLevel.Medium => "中级警告 / Medium Warning",
                PoolWarningLevel.High => "高级警告 / High Warning",
                PoolWarningLevel.Critical => "严重警告 / Critical Warning",
                _ => "未知 / Unknown"
            };
        }
    }
}
