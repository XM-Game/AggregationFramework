// ==========================================================
// 文件名：WarmupConfig.cs
// 命名空间: AFramework.Pool.Warming
// 依赖: System
// 功能: 对象池预热配置
// ==========================================================

using System;

namespace AFramework.Pool.Warming
{
    /// <summary>
    /// 预热策略枚举
    /// Warmup Strategy Enum
    /// </summary>
    public enum WarmupStrategy
    {
        /// <summary>
        /// 立即预热
        /// Immediate warmup
        /// </summary>
        Immediate,

        /// <summary>
        /// 延迟预热
        /// Deferred warmup
        /// </summary>
        Deferred,

        /// <summary>
        /// 按需预热
        /// On-demand warmup
        /// </summary>
        OnDemand,

        /// <summary>
        /// 分帧预热
        /// Frame distributed warmup
        /// </summary>
        FrameDistributed,

        /// <summary>
        /// 异步预热
        /// Async warmup
        /// </summary>
        Async
    }

    /// <summary>
    /// 对象池预热配置
    /// Object Pool Warmup Configuration
    /// </summary>
    /// <remarks>
    /// 配置对象池的预热行为
    /// Configures the warmup behavior of object pools
    /// </remarks>
    public class WarmupConfig
    {
        /// <summary>
        /// 预热策略
        /// Warmup strategy
        /// </summary>
        public WarmupStrategy Strategy { get; set; } = WarmupStrategy.Immediate;

        /// <summary>
        /// 预热数量
        /// Warmup count
        /// </summary>
        public int Count { get; set; } = 10;

        /// <summary>
        /// 是否自动预热
        /// Whether to auto warmup
        /// </summary>
        public bool AutoWarmup { get; set; } = true;

        /// <summary>
        /// 预热延迟（毫秒）
        /// Warmup delay (milliseconds)
        /// </summary>
        public int DelayMilliseconds { get; set; } = 0;

        /// <summary>
        /// 创建默认配置
        /// Create default configuration
        /// </summary>
        public static WarmupConfig Default => new WarmupConfig
        {
            Strategy = WarmupStrategy.Immediate,
            Count = 10,
            AutoWarmup = true,
            DelayMilliseconds = 0
        };

        /// <summary>
        /// 创建立即预热配置
        /// Create immediate warmup configuration
        /// </summary>
        public static WarmupConfig Immediate(int count) => new WarmupConfig
        {
            Strategy = WarmupStrategy.Immediate,
            Count = count,
            AutoWarmup = true,
            DelayMilliseconds = 0
        };

        /// <summary>
        /// 创建延迟预热配置
        /// Create deferred warmup configuration
        /// </summary>
        public static WarmupConfig Deferred(int count, int delayMs) => new WarmupConfig
        {
            Strategy = WarmupStrategy.Deferred,
            Count = count,
            AutoWarmup = true,
            DelayMilliseconds = delayMs
        };

        /// <summary>
        /// 创建按需预热配置
        /// Create on-demand warmup configuration
        /// </summary>
        public static WarmupConfig OnDemand(int count) => new WarmupConfig
        {
            Strategy = WarmupStrategy.OnDemand,
            Count = count,
            AutoWarmup = false,
            DelayMilliseconds = 0
        };
    }
}
