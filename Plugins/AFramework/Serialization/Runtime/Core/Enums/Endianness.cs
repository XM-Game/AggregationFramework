// ==========================================================
// 文件名：Endianness.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 字节序枚举
    /// <para>定义多字节数据的字节排列顺序</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用小端序
    /// var options = new SerializeOptions { Endianness = Endianness.Little };
    /// 
    /// // 检查是否需要字节交换
    /// if (Endianness.Big.NeedsSwap())
    ///     value = BinaryPrimitives.ReverseEndianness(value);
    /// </code>
    /// </remarks>
    public enum Endianness : byte
    {
        /// <summary>
        /// 小端序 (Little Endian)
        /// <para>低位字节在前，高位字节在后</para>
        /// <para>特点：x86/x64/ARM 原生格式</para>
        /// <para>适用：大多数现代处理器</para>
        /// </summary>
        Little = 0,

        /// <summary>
        /// 大端序 (Big Endian)
        /// <para>高位字节在前，低位字节在后</para>
        /// <para>特点：网络字节序、人类可读</para>
        /// <para>适用：网络协议、跨平台交换</para>
        /// </summary>
        Big = 1,

        /// <summary>
        /// 本机字节序
        /// <para>使用当前系统的原生字节序</para>
        /// <para>特点：零转换开销、平台相关</para>
        /// <para>适用：本地存储、同平台通信</para>
        /// </summary>
        Native = 2
    }

    /// <summary>
    /// Endianness 扩展方法
    /// </summary>
    public static class EndiannessExtensions
    {
        /// <summary>
        /// 缓存的系统字节序
        /// </summary>
        private static readonly bool s_isLittleEndian = BitConverter.IsLittleEndian;

        #region 字节序检查方法

        /// <summary>
        /// 检查是否为小端序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLittleEndian(this Endianness endianness)
        {
            return endianness == Endianness.Little ||
                   (endianness == Endianness.Native && s_isLittleEndian);
        }

        /// <summary>
        /// 检查是否为大端序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBigEndian(this Endianness endianness)
        {
            return endianness == Endianness.Big ||
                   (endianness == Endianness.Native && !s_isLittleEndian);
        }

        /// <summary>
        /// 检查是否需要字节交换
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedsSwap(this Endianness endianness)
        {
            if (endianness == Endianness.Native) return false;
            return endianness == Endianness.Big ? s_isLittleEndian : !s_isLittleEndian;
        }

        /// <summary>
        /// 检查是否为本机字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNative(this Endianness endianness)
        {
            if (endianness == Endianness.Native) return true;
            return endianness == Endianness.Little ? s_isLittleEndian : !s_isLittleEndian;
        }

        #endregion

        #region 字节序转换方法

        /// <summary>
        /// 解析为实际字节序 (将 Native 解析为具体值)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Endianness Resolve(this Endianness endianness)
        {
            if (endianness != Endianness.Native) return endianness;
            return s_isLittleEndian ? Endianness.Little : Endianness.Big;
        }

        /// <summary>
        /// 获取相反的字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Endianness GetOpposite(this Endianness endianness)
        {
            var resolved = endianness.Resolve();
            return resolved == Endianness.Little ? Endianness.Big : Endianness.Little;
        }

        /// <summary>
        /// 获取系统本机字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Endianness GetSystemEndianness()
        {
            return s_isLittleEndian ? Endianness.Little : Endianness.Big;
        }

        /// <summary>
        /// 获取网络字节序 (大端)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Endianness GetNetworkEndianness()
        {
            return Endianness.Big;
        }

        #endregion

        #region 字节交换方法

        /// <summary>
        /// 交换 16 位整数的字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SwapBytes(ushort value)
        {
            return (ushort)((value >> 8) | (value << 8));
        }

        /// <summary>
        /// 交换 32 位整数的字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SwapBytes(uint value)
        {
            return ((value >> 24) & 0x000000FF) |
                   ((value >> 8) & 0x0000FF00) |
                   ((value << 8) & 0x00FF0000) |
                   ((value << 24) & 0xFF000000);
        }

        /// <summary>
        /// 交换 64 位整数的字节序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SwapBytes(ulong value)
        {
            return ((value >> 56) & 0x00000000000000FF) |
                   ((value >> 40) & 0x000000000000FF00) |
                   ((value >> 24) & 0x0000000000FF0000) |
                   ((value >> 8) & 0x00000000FF000000) |
                   ((value << 8) & 0x000000FF00000000) |
                   ((value << 24) & 0x0000FF0000000000) |
                   ((value << 40) & 0x00FF000000000000) |
                   ((value << 56) & 0xFF00000000000000);
        }

        /// <summary>
        /// 根据字节序条件交换 16 位整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ConvertToEndianness(this Endianness endianness, ushort value)
        {
            return endianness.NeedsSwap() ? SwapBytes(value) : value;
        }

        /// <summary>
        /// 根据字节序条件交换 32 位整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertToEndianness(this Endianness endianness, uint value)
        {
            return endianness.NeedsSwap() ? SwapBytes(value) : value;
        }

        /// <summary>
        /// 根据字节序条件交换 64 位整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ConvertToEndianness(this Endianness endianness, ulong value)
        {
            return endianness.NeedsSwap() ? SwapBytes(value) : value;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取字节序的中文描述
        /// </summary>
        public static string GetDescription(this Endianness endianness)
        {
            return endianness switch
            {
                Endianness.Little => "小端序 (Little Endian)",
                Endianness.Big => "大端序 (Big Endian)",
                Endianness.Native => $"本机字节序 ({(s_isLittleEndian ? "小端" : "大端")})",
                _ => "未知字节序"
            };
        }

        /// <summary>
        /// 获取字节序的简短名称
        /// </summary>
        public static string GetShortName(this Endianness endianness)
        {
            return endianness switch
            {
                Endianness.Little => "LE",
                Endianness.Big => "BE",
                Endianness.Native => s_isLittleEndian ? "LE" : "BE",
                _ => "??"
            };
        }

        #endregion
    }
}
