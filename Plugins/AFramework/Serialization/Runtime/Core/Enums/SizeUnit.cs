// ==========================================================
// 文件名：SizeUnit.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 大小单位枚举
    /// <para>定义固定大小特性中大小的计量单位</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [FixedSize(64, SizeUnit = SizeUnit.Bytes)]
    /// public byte[] Data;
    /// 
    /// [FixedSize(100, SizeUnit = SizeUnit.Elements)]
    /// public int[] Scores;
    /// </code>
    /// </remarks>
    public enum SizeUnit : byte
    {
        /// <summary>
        /// 元素数量（用于数组、集合）
        /// <para>大小表示元素的个数</para>
        /// </summary>
        Elements = 0,

        /// <summary>
        /// 字节数
        /// <para>大小表示总字节数</para>
        /// </summary>
        Bytes = 1,

        /// <summary>
        /// 字符数（用于字符串）
        /// <para>大小表示字符的个数</para>
        /// </summary>
        Characters = 2,

        /// <summary>
        /// 位数
        /// <para>大小表示总位数</para>
        /// </summary>
        Bits = 3
    }

    /// <summary>
    /// SizeUnit 扩展方法
    /// </summary>
    public static class SizeUnitExtensions
    {
        /// <summary>
        /// 获取单位的中文描述
        /// </summary>
        /// <param name="unit">大小单位</param>
        /// <returns>中文描述</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDescription(this SizeUnit unit)
        {
            return unit switch
            {
                SizeUnit.Elements => "元素",
                SizeUnit.Bytes => "字节",
                SizeUnit.Characters => "字符",
                SizeUnit.Bits => "位",
                _ => "未知单位"
            };
        }

        /// <summary>
        /// 获取单位的缩写
        /// </summary>
        /// <param name="unit">大小单位</param>
        /// <returns>缩写</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAbbreviation(this SizeUnit unit)
        {
            return unit switch
            {
                SizeUnit.Elements => "elem",
                SizeUnit.Bytes => "B",
                SizeUnit.Characters => "char",
                SizeUnit.Bits => "bit",
                _ => "?"
            };
        }

        /// <summary>
        /// 转换大小到字节数
        /// </summary>
        /// <param name="unit">大小单位</param>
        /// <param name="size">大小值</param>
        /// <param name="elementSize">元素大小（字节，仅用于 Elements 单位）</param>
        /// <returns>字节数</returns>
        public static int ToBytes(this SizeUnit unit, int size, int elementSize = 1)
        {
            return unit switch
            {
                SizeUnit.Elements => size * elementSize,
                SizeUnit.Bytes => size,
                SizeUnit.Characters => size * 2, // UTF-16 假设
                SizeUnit.Bits => (size + 7) / 8,
                _ => size
            };
        }
    }
}
