// ==========================================================
// 文件名：OutOfRangeHandling.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 超出范围处理方式枚举
    /// <para>定义数值超出有效范围时的处理策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [BitPack(10, OutOfRangeHandling = OutOfRangeHandling.Clamp)]
    /// public int Health;
    /// </code>
    /// </remarks>
    public enum OutOfRangeHandling : byte
    {
        /// <summary>
        /// 钳制到有效范围
        /// <para>超出范围的值会被限制到最近的边界值</para>
        /// </summary>
        Clamp = 0,

        /// <summary>
        /// 包装到有效范围（循环）
        /// <para>超出范围的值会循环回到范围内</para>
        /// </summary>
        Wrap = 1,

        /// <summary>
        /// 抛出异常
        /// <para>超出范围时抛出 ArgumentOutOfRangeException</para>
        /// </summary>
        Throw = 2,

        /// <summary>
        /// 忽略（不检查）
        /// <para>不进行范围检查，可能导致数据截断</para>
        /// </summary>
        Ignore = 3
    }

    /// <summary>
    /// OutOfRangeHandling 扩展方法
    /// </summary>
    public static class OutOfRangeHandlingExtensions
    {
        /// <summary>
        /// 获取处理方式的中文描述
        /// </summary>
        /// <param name="handling">处理方式</param>
        /// <returns>中文描述</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDescription(this OutOfRangeHandling handling)
        {
            return handling switch
            {
                OutOfRangeHandling.Clamp => "钳制到有效范围",
                OutOfRangeHandling.Wrap => "包装到有效范围",
                OutOfRangeHandling.Throw => "抛出异常",
                OutOfRangeHandling.Ignore => "忽略检查",
                _ => "未知处理方式"
            };
        }

        /// <summary>
        /// 检查是否需要进行范围检查
        /// </summary>
        /// <param name="handling">处理方式</param>
        /// <returns>如果需要检查返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresCheck(this OutOfRangeHandling handling)
        {
            return handling != OutOfRangeHandling.Ignore;
        }

        /// <summary>
        /// 检查是否会抛出异常
        /// </summary>
        /// <param name="handling">处理方式</param>
        /// <returns>如果会抛出异常返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WillThrow(this OutOfRangeHandling handling)
        {
            return handling == OutOfRangeHandling.Throw;
        }
    }
}
