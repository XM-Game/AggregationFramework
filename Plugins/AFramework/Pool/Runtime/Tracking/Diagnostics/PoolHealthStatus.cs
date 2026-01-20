// ==========================================================
// 文件名：PoolHealthStatus.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 对象池健康状态枚举
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池健康状态
    /// Pool Health Status
    /// </summary>
    public enum PoolHealthStatus
    {
        /// <summary>
        /// 健康 - 池运行正常
        /// Healthy - Pool is operating normally
        /// </summary>
        Healthy = 0,

        /// <summary>
        /// 警告 - 存在潜在问题但不影响功能
        /// Warning - Potential issues exist but functionality is not affected
        /// </summary>
        Warning = 1,

        /// <summary>
        /// 不健康 - 存在影响性能的问题
        /// Unhealthy - Issues affecting performance exist
        /// </summary>
        Unhealthy = 2,

        /// <summary>
        /// 严重 - 存在严重问题，可能导致功能异常
        /// Critical - Severe issues exist that may cause functional abnormalities
        /// </summary>
        Critical = 3,
    }
}
