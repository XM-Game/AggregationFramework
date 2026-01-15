// ==========================================================
// 文件名：BurstSafetyLevel.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// Burst 安全检查级别枚举
    /// <para>定义 Burst 编译时的安全检查策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [BurstSerializable(SafetyLevel = BurstSafetyLevel.Minimal)]
    /// public struct FastData { }
    /// </code>
    /// </remarks>
    public enum BurstSafetyLevel : byte
    {
        /// <summary>
        /// 默认安全级别
        /// <para>使用 Burst 编译器的默认安全检查设置</para>
        /// </summary>
        Default = 0,

        /// <summary>
        /// 完全安全检查
        /// <para>启用所有安全检查，适合开发调试阶段</para>
        /// </summary>
        Full = 1,

        /// <summary>
        /// 最小安全检查（高性能）
        /// <para>仅保留关键安全检查，适合性能敏感场景</para>
        /// </summary>
        Minimal = 2,

        /// <summary>
        /// 禁用安全检查（最高性能）
        /// <para>禁用所有安全检查，仅用于发布版本</para>
        /// <para>警告：可能导致未定义行为</para>
        /// </summary>
        Disabled = 3
    }

    /// <summary>
    /// BurstSafetyLevel 扩展方法
    /// </summary>
    public static class BurstSafetyLevelExtensions
    {
        /// <summary>
        /// 检查是否启用安全检查
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSafetyChecks(this BurstSafetyLevel level)
        {
            return level != BurstSafetyLevel.Disabled;
        }

        /// <summary>
        /// 检查是否为完全安全模式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFullSafety(this BurstSafetyLevel level)
        {
            return level == BurstSafetyLevel.Full;
        }

        /// <summary>
        /// 检查是否适合发布版本
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReleaseReady(this BurstSafetyLevel level)
        {
            return level == BurstSafetyLevel.Minimal ||
                   level == BurstSafetyLevel.Disabled;
        }

        /// <summary>
        /// 获取级别的中文描述
        /// </summary>
        public static string GetDescription(this BurstSafetyLevel level)
        {
            return level switch
            {
                BurstSafetyLevel.Default => "默认安全级别",
                BurstSafetyLevel.Full => "完全安全检查",
                BurstSafetyLevel.Minimal => "最小安全检查",
                BurstSafetyLevel.Disabled => "禁用安全检查",
                _ => "未知级别"
            };
        }

        /// <summary>
        /// 获取预估的性能等级 (1-5，5为最高)
        /// </summary>
        public static int GetPerformanceLevel(this BurstSafetyLevel level)
        {
            return level switch
            {
                BurstSafetyLevel.Disabled => 5,
                BurstSafetyLevel.Minimal => 4,
                BurstSafetyLevel.Default => 3,
                BurstSafetyLevel.Full => 2,
                _ => 3
            };
        }
    }
}
