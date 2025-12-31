// ==========================================================
// 文件名：BitUtility.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 位运算工具类
    /// <para>提供高效的位操作功能，适用于标志位、掩码、位域等场景</para>
    /// </summary>
    public static class BitUtility
    {
        #region 位设置与清除

        /// <summary>
        /// 设置指定位为 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBit(int value, int bitIndex)
        {
            return value | (1 << bitIndex);
        }

        /// <summary>
        /// 清除指定位（设为 0）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClearBit(int value, int bitIndex)
        {
            return value & ~(1 << bitIndex);
        }

        /// <summary>
        /// 切换指定位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToggleBit(int value, int bitIndex)
        {
            return value ^ (1 << bitIndex);
        }

        /// <summary>
        /// 设置指定位为指定值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetBitValue(int value, int bitIndex, bool bitValue)
        {
            return bitValue ? SetBit(value, bitIndex) : ClearBit(value, bitIndex);
        }

        #endregion

        #region 位检查

        /// <summary>
        /// 检查指定位是否为 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }

        /// <summary>
        /// 检查指定位是否为 0
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitClear(int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) == 0;
        }

        /// <summary>
        /// 获取指定位的值（0 或 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBit(int value, int bitIndex)
        {
            return (value >> bitIndex) & 1;
        }

        #endregion

        #region 标志位操作

        /// <summary>
        /// 检查是否包含所有指定标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags(int value, int flags)
        {
            return (value & flags) == flags;
        }

        /// <summary>
        /// 检查是否包含任意指定标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag(int value, int flags)
        {
            return (value & flags) != 0;
        }

        /// <summary>
        /// 添加标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddFlags(int value, int flags)
        {
            return value | flags;
        }

        /// <summary>
        /// 移除标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RemoveFlags(int value, int flags)
        {
            return value & ~flags;
        }

        /// <summary>
        /// 切换标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToggleFlags(int value, int flags)
        {
            return value ^ flags;
        }

        /// <summary>
        /// 设置或清除标志
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetFlags(int value, int flags, bool set)
        {
            return set ? AddFlags(value, flags) : RemoveFlags(value, flags);
        }

        #endregion

        #region 位计数

        /// <summary>
        /// 计算设置为 1 的位数（Population Count）
        /// </summary>
        public static int PopCount(int value)
        {
            uint v = (uint)value;
            v = v - ((v >> 1) & 0x55555555);
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
            return (int)(((v + (v >> 4)) & 0x0F0F0F0F) * 0x01010101 >> 24);
        }

        /// <summary>
        /// 计算设置为 1 的位数（64位）
        /// </summary>
        public static int PopCount(long value)
        {
            ulong v = (ulong)value;
            v = v - ((v >> 1) & 0x5555555555555555UL);
            v = (v & 0x3333333333333333UL) + ((v >> 2) & 0x3333333333333333UL);
            return (int)(((v + (v >> 4)) & 0x0F0F0F0F0F0F0F0FUL) * 0x0101010101010101UL >> 56);
        }

        /// <summary>
        /// 计算前导零的数量
        /// </summary>
        public static int LeadingZeroCount(int value)
        {
            if (value == 0) return 32;
            int count = 0;
            if ((value & 0xFFFF0000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF0000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC0000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x80000000) == 0) { count += 1; }
            return count;
        }

        /// <summary>
        /// 计算尾随零的数量
        /// </summary>
        public static int TrailingZeroCount(int value)
        {
            if (value == 0) return 32;
            int count = 0;
            if ((value & 0x0000FFFF) == 0) { count += 16; value >>= 16; }
            if ((value & 0x000000FF) == 0) { count += 8; value >>= 8; }
            if ((value & 0x0000000F) == 0) { count += 4; value >>= 4; }
            if ((value & 0x00000003) == 0) { count += 2; value >>= 2; }
            if ((value & 0x00000001) == 0) { count += 1; }
            return count;
        }

        #endregion

        #region 位操作

        /// <summary>
        /// 反转所有位
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReverseBits(int value)
        {
            uint v = (uint)value;
            v = ((v >> 1) & 0x55555555) | ((v & 0x55555555) << 1);
            v = ((v >> 2) & 0x33333333) | ((v & 0x33333333) << 2);
            v = ((v >> 4) & 0x0F0F0F0F) | ((v & 0x0F0F0F0F) << 4);
            v = ((v >> 8) & 0x00FF00FF) | ((v & 0x00FF00FF) << 8);
            return (int)((v >> 16) | (v << 16));
        }

        /// <summary>
        /// 循环左移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotateLeft(int value, int shift)
        {
            shift &= 31;
            return (value << shift) | ((int)((uint)value >> (32 - shift)));
        }

        /// <summary>
        /// 循环右移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotateRight(int value, int shift)
        {
            shift &= 31;
            return ((int)((uint)value >> shift)) | (value << (32 - shift));
        }

        /// <summary>
        /// 交换高低字节
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SwapBytes(int value)
        {
            uint v = (uint)value;
            return (int)(((v & 0x000000FF) << 24) |
                        ((v & 0x0000FF00) << 8) |
                        ((v & 0x00FF0000) >> 8) |
                        ((v & 0xFF000000) >> 24));
        }

        #endregion

        #region 位域操作

        /// <summary>
        /// 提取位域
        /// </summary>
        /// <param name="value">源值</param>
        /// <param name="startBit">起始位</param>
        /// <param name="bitCount">位数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExtractBits(int value, int startBit, int bitCount)
        {
            int mask = (1 << bitCount) - 1;
            return (value >> startBit) & mask;
        }

        /// <summary>
        /// 插入位域
        /// </summary>
        /// <param name="target">目标值</param>
        /// <param name="value">要插入的值</param>
        /// <param name="startBit">起始位</param>
        /// <param name="bitCount">位数</param>
        public static int InsertBits(int target, int value, int startBit, int bitCount)
        {
            int mask = (1 << bitCount) - 1;
            int clearMask = ~(mask << startBit);
            return (target & clearMask) | ((value & mask) << startBit);
        }

        /// <summary>
        /// 创建位掩码
        /// </summary>
        /// <param name="bitCount">位数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateMask(int bitCount)
        {
            return (1 << bitCount) - 1;
        }

        /// <summary>
        /// 创建范围位掩码
        /// </summary>
        /// <param name="startBit">起始位</param>
        /// <param name="endBit">结束位（包含）</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CreateRangeMask(int startBit, int endBit)
        {
            return ((1 << (endBit - startBit + 1)) - 1) << startBit;
        }

        #endregion

        #region 2的幂相关

        /// <summary>
        /// 检查是否为2的幂
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// 获取大于等于给定值的最小2的幂
        /// </summary>
        public static int NextPowerOfTwo(int value)
        {
            if (value <= 0) return 1;
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        /// 获取小于等于给定值的最大2的幂
        /// </summary>
        public static int PreviousPowerOfTwo(int value)
        {
            if (value <= 0) return 0;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value - (value >> 1);
        }

        /// <summary>
        /// 计算以2为底的对数（向下取整）
        /// </summary>
        public static int Log2(int value)
        {
            if (value <= 0) return -1;
            return 31 - LeadingZeroCount(value);
        }

        #endregion

        #region 字节操作

        /// <summary>
        /// 获取指定字节
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(int value, int byteIndex)
        {
            return (byte)(value >> (byteIndex * 8));
        }

        /// <summary>
        /// 设置指定字节
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetByte(int value, int byteIndex, byte byteValue)
        {
            int shift = byteIndex * 8;
            int mask = ~(0xFF << shift);
            return (value & mask) | (byteValue << shift);
        }

        /// <summary>
        /// 将4个字节组合为int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PackBytes(byte b0, byte b1, byte b2, byte b3)
        {
            return b0 | (b1 << 8) | (b2 << 16) | (b3 << 24);
        }

        /// <summary>
        /// 将int拆分为4个字节
        /// </summary>
        public static void UnpackBytes(int value, out byte b0, out byte b1, out byte b2, out byte b3)
        {
            b0 = (byte)value;
            b1 = (byte)(value >> 8);
            b2 = (byte)(value >> 16);
            b3 = (byte)(value >> 24);
        }

        #endregion
    }
}
