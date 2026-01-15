// ==========================================================
// 文件名：PaddingPosition.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 填充位置枚举
    /// <para>定义固定大小数据的填充位置</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [FixedSize(32, PaddingPosition = PaddingPosition.End)]
    /// public string Name;
    /// </code>
    /// </remarks>
    public enum PaddingPosition : byte
    {
        /// <summary>
        /// 在末尾填充
        /// <para>数据在前，填充在后</para>
        /// </summary>
        End = 0,

        /// <summary>
        /// 在开头填充
        /// <para>填充在前，数据在后</para>
        /// </summary>
        Start = 1,

        /// <summary>
        /// 两端填充（居中）
        /// <para>数据居中，两端填充</para>
        /// </summary>
        Both = 2
    }

    /// <summary>
    /// PaddingPosition 扩展方法
    /// </summary>
    public static class PaddingPositionExtensions
    {
        /// <summary>
        /// 获取位置的中文描述
        /// </summary>
        /// <param name="position">填充位置</param>
        /// <returns>中文描述</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDescription(this PaddingPosition position)
        {
            return position switch
            {
                PaddingPosition.End => "末尾填充",
                PaddingPosition.Start => "开头填充",
                PaddingPosition.Both => "两端填充",
                _ => "未知位置"
            };
        }

        /// <summary>
        /// 计算填充分布
        /// </summary>
        /// <param name="position">填充位置</param>
        /// <param name="totalPadding">总填充量</param>
        /// <param name="startPadding">开头填充量（输出）</param>
        /// <param name="endPadding">末尾填充量（输出）</param>
        public static void CalculatePadding(
            this PaddingPosition position,
            int totalPadding,
            out int startPadding,
            out int endPadding)
        {
            switch (position)
            {
                case PaddingPosition.Start:
                    startPadding = totalPadding;
                    endPadding = 0;
                    break;
                case PaddingPosition.Both:
                    startPadding = totalPadding / 2;
                    endPadding = totalPadding - startPadding;
                    break;
                case PaddingPosition.End:
                default:
                    startPadding = 0;
                    endPadding = totalPadding;
                    break;
            }
        }
    }
}
