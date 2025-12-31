// ==========================================================
// 文件名：BurstCompileExAttribute.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：增强的BurstCompile特性，提供更多编译选项
// 依赖：Unity.Burst
// ==========================================================

using System;
using Unity.Burst;

namespace AFramework.Burst
{
    /// <summary>
    /// 增强的BurstCompile特性
    /// 提供预设的编译配置，简化常见场景的Burst优化设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false)]
    public class BurstCompileExAttribute : BurstCompileAttribute
    {
        /// <summary>
        /// 优化级别预设
        /// </summary>
        public enum OptimizationPreset
        {
            /// <summary>
            /// 默认优化（平衡编译时间和性能）
            /// </summary>
            Default,

            /// <summary>
            /// 最大性能优化（可能增加编译时间）
            /// </summary>
            MaxPerformance,

            /// <summary>
            /// 快速编译（牺牲部分性能换取编译速度）
            /// </summary>
            FastCompile,

            /// <summary>
            /// 调试友好（保留更多调试信息）
            /// </summary>
            Debug,

            /// <summary>
            /// 最小代码大小
            /// </summary>
            MinSize
        }

        /// <summary>
        /// 使用默认设置创建特性
        /// </summary>
        public BurstCompileExAttribute() : base()
        {
        }

        /// <summary>
        /// 使用指定优化预设创建特性
        /// </summary>
        /// <param name="preset">优化预设</param>
        public BurstCompileExAttribute(OptimizationPreset preset) : base()
        {
            ApplyPreset(preset);
        }

        /// <summary>
        /// 应用优化预设
        /// </summary>
        private void ApplyPreset(OptimizationPreset preset)
        {
            switch (preset)
            {
                case OptimizationPreset.MaxPerformance:
                    OptimizeFor = OptimizeFor.Performance;
                    FloatMode = FloatMode.Fast;
                    FloatPrecision = FloatPrecision.Low;
                    DisableSafetyChecks = true;
                    break;

                case OptimizationPreset.FastCompile:
                    OptimizeFor = OptimizeFor.Balanced;
                    FloatMode = FloatMode.Default;
                    FloatPrecision = FloatPrecision.Standard;
                    break;

                case OptimizationPreset.Debug:
                    OptimizeFor = OptimizeFor.Balanced;
                    FloatMode = FloatMode.Strict;
                    FloatPrecision = FloatPrecision.High;
                    Debug = true;
                    DisableSafetyChecks = false;
                    break;

                case OptimizationPreset.MinSize:
                    OptimizeFor = OptimizeFor.Size;
                    FloatMode = FloatMode.Default;
                    FloatPrecision = FloatPrecision.Standard;
                    break;

                case OptimizationPreset.Default:
                default:
                    OptimizeFor = OptimizeFor.Balanced;
                    FloatMode = FloatMode.Default;
                    FloatPrecision = FloatPrecision.Standard;
                    break;
            }
        }
    }

    /// <summary>
    /// 高性能Burst编译特性
    /// 预设为最大性能优化
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false)]
    public class BurstCompileHighPerformanceAttribute : BurstCompileAttribute
    {
        /// <summary>
        /// 创建高性能编译特性
        /// </summary>
        public BurstCompileHighPerformanceAttribute() : base()
        {
            OptimizeFor = OptimizeFor.Performance;
            FloatMode = FloatMode.Fast;
            FloatPrecision = FloatPrecision.Low;
            DisableSafetyChecks = true;
        }
    }

    /// <summary>
    /// 精确计算Burst编译特性
    /// 预设为高精度浮点计算
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false)]
    public class BurstCompilePreciseAttribute : BurstCompileAttribute
    {
        /// <summary>
        /// 创建精确计算编译特性
        /// </summary>
        public BurstCompilePreciseAttribute() : base()
        {
            OptimizeFor = OptimizeFor.Balanced;
            FloatMode = FloatMode.Strict;
            FloatPrecision = FloatPrecision.High;
        }
    }

    /// <summary>
    /// 调试友好Burst编译特性
    /// 保留调试信息，便于问题排查
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false)]
    public class BurstCompileDebugAttribute : BurstCompileAttribute
    {
        /// <summary>
        /// 创建调试友好编译特性
        /// </summary>
        public BurstCompileDebugAttribute() : base()
        {
            Debug = true;
            DisableSafetyChecks = false;
            OptimizeFor = OptimizeFor.Balanced;
        }
    }
}
