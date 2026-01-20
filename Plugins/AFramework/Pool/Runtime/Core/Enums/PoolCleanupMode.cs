// ==========================================================
// 文件名：PoolCleanupMode.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池清理模式枚举
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池清理模式枚举
    /// </summary>
    public enum PoolCleanupMode
    {
        /// <summary>
        /// 手动清理模式
        /// 需要手动调用 Clear() 或 Shrink() 清理
        /// </summary>
        /// <remarks>
        /// 适用场景：
        /// - 需要精确控制清理时机
        /// - 场景切换时手动清理
        /// - 关卡结束时批量清理
        /// </remarks>
        Manual = 0,

        /// <summary>
        /// 自动清理模式
        /// 归还对象时自动检查是否需要清理
        /// </summary>
        /// <remarks>
        /// 适用场景：
        /// - 长时间运行的应用
        /// - 自动内存管理
        /// - 防止内存持续增长
        /// </remarks>
        Auto = 1,

        /// <summary>
        /// 定时清理模式
        /// 定期自动清理空闲对象
        /// </summary>
        /// <remarks>
        /// 适用场景：
        /// - 服务器应用
        /// - 后台服务
        /// - 定期维护
        /// </remarks>
        Timed = 2,

        /// <summary>
        /// 空闲清理模式（LRU）
        /// 清理长时间未使用的对象
        /// </summary>
        /// <remarks>
        /// 适用场景：
        /// - 缓存管理
        /// - 资源优化
        /// - 内存敏感场景
        /// </remarks>
        Idle = 3,
    }
}
