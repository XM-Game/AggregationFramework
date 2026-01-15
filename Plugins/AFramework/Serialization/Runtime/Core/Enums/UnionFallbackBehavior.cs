// ==========================================================
// 文件名：UnionFallbackBehavior.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 联合类型回退行为枚举
    /// <para>定义遇到未知派生类型时的处理策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [UnionBase(FallbackBehavior = UnionFallbackBehavior.ReturnNull)]
    /// public interface IAnimal { }
    /// </code>
    /// </remarks>
    public enum UnionFallbackBehavior : byte
    {
        /// <summary>
        /// 抛出异常（默认）
        /// <para>遇到未知类型时抛出异常</para>
        /// </summary>
        ThrowException = 0,

        /// <summary>
        /// 返回 null
        /// <para>遇到未知类型时返回 null</para>
        /// </summary>
        ReturnNull = 1,

        /// <summary>
        /// 返回默认值
        /// <para>遇到未知类型时返回类型的默认值</para>
        /// </summary>
        ReturnDefault = 2,

        /// <summary>
        /// 跳过
        /// <para>遇到未知类型时跳过该字段</para>
        /// </summary>
        Skip = 3,

        /// <summary>
        /// 使用回退类型
        /// <para>遇到未知类型时使用指定的回退类型</para>
        /// </summary>
        UseFallbackType = 4
    }

    /// <summary>
    /// UnionFallbackBehavior 扩展方法
    /// </summary>
    public static class UnionFallbackBehaviorExtensions
    {
        /// <summary>
        /// 检查是否会抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WillThrow(this UnionFallbackBehavior behavior)
        {
            return behavior == UnionFallbackBehavior.ThrowException;
        }

        /// <summary>
        /// 检查是否返回空值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnsEmpty(this UnionFallbackBehavior behavior)
        {
            return behavior == UnionFallbackBehavior.ReturnNull ||
                   behavior == UnionFallbackBehavior.ReturnDefault;
        }

        /// <summary>
        /// 检查是否需要回退类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresFallbackType(this UnionFallbackBehavior behavior)
        {
            return behavior == UnionFallbackBehavior.UseFallbackType;
        }

        /// <summary>
        /// 获取行为的中文描述
        /// </summary>
        public static string GetDescription(this UnionFallbackBehavior behavior)
        {
            return behavior switch
            {
                UnionFallbackBehavior.ThrowException => "抛出异常",
                UnionFallbackBehavior.ReturnNull => "返回空值",
                UnionFallbackBehavior.ReturnDefault => "返回默认值",
                UnionFallbackBehavior.Skip => "跳过字段",
                UnionFallbackBehavior.UseFallbackType => "使用回退类型",
                _ => "未知行为"
            };
        }
    }
}
