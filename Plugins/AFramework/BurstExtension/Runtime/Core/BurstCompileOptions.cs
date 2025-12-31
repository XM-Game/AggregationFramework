// ==========================================================
// 文件名：BurstCompileOptions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：Burst编译选项配置，提供编译优化级别和目标平台配置
// ==========================================================

using System;
using Unity.Burst;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst编译优化级别
    /// </summary>
    public enum BurstOptimizationLevel
    {
        /// <summary>
        /// 无优化（调试模式）
        /// </summary>
        None = 0,

        /// <summary>
        /// 基础优化
        /// </summary>
        Basic = 1,

        /// <summary>
        /// 标准优化（默认）
        /// </summary>
        Standard = 2,

        /// <summary>
        /// 激进优化（可能增加编译时间）
        /// </summary>
        Aggressive = 3
    }

    /// <summary>
    /// Burst浮点精度模式
    /// </summary>
    public enum BurstFloatPrecision
    {
        /// <summary>
        /// 标准精度
        /// </summary>
        Standard = 0,

        /// <summary>
        /// 高精度
        /// </summary>
        High = 1,

        /// <summary>
        /// 低精度（更快但精度较低）
        /// </summary>
        Low = 2,

        /// <summary>
        /// 确定性精度（跨平台一致）
        /// </summary>
        Deterministic = 3
    }

    /// <summary>
    /// Burst浮点模式
    /// </summary>
    public enum BurstFloatMode
    {
        /// <summary>
        /// 默认模式
        /// </summary>
        Default = 0,

        /// <summary>
        /// 严格模式（IEEE 754兼容）
        /// </summary>
        Strict = 1,

        /// <summary>
        /// 确定性模式
        /// </summary>
        Deterministic = 2,

        /// <summary>
        /// 快速模式（允许重排序和近似）
        /// </summary>
        Fast = 3
    }

    /// <summary>
    /// Burst编译选项配置类
    /// 提供编译时优化选项的统一管理
    /// </summary>
    public readonly struct BurstCompileOptions : IEquatable<BurstCompileOptions>
    {
        #region 字段

        /// <summary>
        /// 优化级别
        /// </summary>
        public readonly BurstOptimizationLevel OptimizationLevel;

        /// <summary>
        /// 浮点精度
        /// </summary>
        public readonly BurstFloatPrecision FloatPrecision;

        /// <summary>
        /// 浮点模式
        /// </summary>
        public readonly BurstFloatMode FloatMode;

        /// <summary>
        /// 是否启用安全检查
        /// </summary>
        public readonly bool EnableSafetyChecks;

        /// <summary>
        /// 是否禁用别名检查
        /// </summary>
        public readonly bool DisableAliasChecks;

        /// <summary>
        /// 是否启用快速数学
        /// </summary>
        public readonly bool EnableFastMath;

        /// <summary>
        /// 是否启用SIMD优化
        /// </summary>
        public readonly bool EnableSimd;

        /// <summary>
        /// 是否启用调试信息
        /// </summary>
        public readonly bool EnableDebugInfo;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建Burst编译选项
        /// </summary>
        public BurstCompileOptions(
            BurstOptimizationLevel optimizationLevel = BurstOptimizationLevel.Standard,
            BurstFloatPrecision floatPrecision = BurstFloatPrecision.Standard,
            BurstFloatMode floatMode = BurstFloatMode.Default,
            bool enableSafetyChecks = true,
            bool disableAliasChecks = false,
            bool enableFastMath = false,
            bool enableSimd = true,
            bool enableDebugInfo = false)
        {
            OptimizationLevel = optimizationLevel;
            FloatPrecision = floatPrecision;
            FloatMode = floatMode;
            EnableSafetyChecks = enableSafetyChecks;
            DisableAliasChecks = disableAliasChecks;
            EnableFastMath = enableFastMath;
            EnableSimd = enableSimd;
            EnableDebugInfo = enableDebugInfo;
        }

        #endregion

        #region 预设配置

        /// <summary>
        /// 默认配置
        /// </summary>
        public static BurstCompileOptions Default => new BurstCompileOptions();

        /// <summary>
        /// 调试配置
        /// </summary>
        public static BurstCompileOptions Debug => new BurstCompileOptions(
            optimizationLevel: BurstOptimizationLevel.None,
            enableSafetyChecks: true,
            enableDebugInfo: true
        );

        /// <summary>
        /// 发布配置（高性能）
        /// </summary>
        public static BurstCompileOptions Release => new BurstCompileOptions(
            optimizationLevel: BurstOptimizationLevel.Aggressive,
            floatMode: BurstFloatMode.Fast,
            enableSafetyChecks: false,
            disableAliasChecks: true,
            enableFastMath: true,
            enableSimd: true
        );

        /// <summary>
        /// 确定性配置（跨平台一致性）
        /// </summary>
        public static BurstCompileOptions Deterministic => new BurstCompileOptions(
            optimizationLevel: BurstOptimizationLevel.Standard,
            floatPrecision: BurstFloatPrecision.Deterministic,
            floatMode: BurstFloatMode.Deterministic,
            enableFastMath: false
        );

        /// <summary>
        /// 高精度配置
        /// </summary>
        public static BurstCompileOptions HighPrecision => new BurstCompileOptions(
            floatPrecision: BurstFloatPrecision.High,
            floatMode: BurstFloatMode.Strict,
            enableFastMath: false
        );

        /// <summary>
        /// 快速配置（牺牲精度换取速度）
        /// </summary>
        public static BurstCompileOptions Fast => new BurstCompileOptions(
            optimizationLevel: BurstOptimizationLevel.Aggressive,
            floatPrecision: BurstFloatPrecision.Low,
            floatMode: BurstFloatMode.Fast,
            enableSafetyChecks: false,
            enableFastMath: true
        );

        #endregion

        #region 构建器方法

        /// <summary>
        /// 设置优化级别
        /// </summary>
        public BurstCompileOptions WithOptimizationLevel(BurstOptimizationLevel level)
        {
            return new BurstCompileOptions(
                level, FloatPrecision, FloatMode,
                EnableSafetyChecks, DisableAliasChecks, EnableFastMath, EnableSimd, EnableDebugInfo
            );
        }

        /// <summary>
        /// 设置浮点精度
        /// </summary>
        public BurstCompileOptions WithFloatPrecision(BurstFloatPrecision precision)
        {
            return new BurstCompileOptions(
                OptimizationLevel, precision, FloatMode,
                EnableSafetyChecks, DisableAliasChecks, EnableFastMath, EnableSimd, EnableDebugInfo
            );
        }

        /// <summary>
        /// 设置浮点模式
        /// </summary>
        public BurstCompileOptions WithFloatMode(BurstFloatMode mode)
        {
            return new BurstCompileOptions(
                OptimizationLevel, FloatPrecision, mode,
                EnableSafetyChecks, DisableAliasChecks, EnableFastMath, EnableSimd, EnableDebugInfo
            );
        }

        /// <summary>
        /// 启用或禁用安全检查
        /// </summary>
        public BurstCompileOptions WithSafetyChecks(bool enabled)
        {
            return new BurstCompileOptions(
                OptimizationLevel, FloatPrecision, FloatMode,
                enabled, DisableAliasChecks, EnableFastMath, EnableSimd, EnableDebugInfo
            );
        }

        /// <summary>
        /// 启用或禁用快速数学
        /// </summary>
        public BurstCompileOptions WithFastMath(bool enabled)
        {
            return new BurstCompileOptions(
                OptimizationLevel, FloatPrecision, FloatMode,
                EnableSafetyChecks, DisableAliasChecks, enabled, EnableSimd, EnableDebugInfo
            );
        }

        /// <summary>
        /// 启用或禁用SIMD
        /// </summary>
        public BurstCompileOptions WithSimd(bool enabled)
        {
            return new BurstCompileOptions(
                OptimizationLevel, FloatPrecision, FloatMode,
                EnableSafetyChecks, DisableAliasChecks, EnableFastMath, enabled, EnableDebugInfo
            );
        }

        #endregion

        #region IEquatable实现

        public bool Equals(BurstCompileOptions other)
        {
            return OptimizationLevel == other.OptimizationLevel &&
                   FloatPrecision == other.FloatPrecision &&
                   FloatMode == other.FloatMode &&
                   EnableSafetyChecks == other.EnableSafetyChecks &&
                   DisableAliasChecks == other.DisableAliasChecks &&
                   EnableFastMath == other.EnableFastMath &&
                   EnableSimd == other.EnableSimd &&
                   EnableDebugInfo == other.EnableDebugInfo;
        }

        public override bool Equals(object obj)
        {
            return obj is BurstCompileOptions other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                OptimizationLevel, FloatPrecision, FloatMode,
                EnableSafetyChecks, DisableAliasChecks, EnableFastMath, EnableSimd, EnableDebugInfo
            );
        }

        public static bool operator ==(BurstCompileOptions left, BurstCompileOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BurstCompileOptions left, BurstCompileOptions right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"BurstCompileOptions(Opt={OptimizationLevel}, Float={FloatMode}, Safety={EnableSafetyChecks})";
        }

        #endregion
    }
}
