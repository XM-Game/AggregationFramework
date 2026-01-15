// ==========================================================
// 文件名：VarIntDecoder.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 变长整数解码器
    /// <para>提供高性能的变长整数 (VarInt) 解码功能</para>
    /// <para>支持 32 位和 64 位整数的变长解码</para>
    /// </summary>
    /// <remarks>
    /// VarInt 解码规则:
    /// - 每个字节的最高位 (MSB) 表示是否有后续字节
    /// - 低 7 位存储实际数据
    /// - 按小端序组合各字节的数据位
    /// 
    /// 使用示例:
    /// <code>
    /// // 解码
    /// int value = VarIntDecoder.ReadVarInt32(buffer, out int bytesRead);
    /// 
    /// // 带边界检查的解码
    /// if (VarIntDecoder.TryReadVarInt32(buffer, out int value, out int bytesRead))
    ///     ProcessValue(value);
    /// </code>
    /// </remarks>
    public static class VarIntDecoder
    {
        #region 常量

        /// <summary>继续位掩码 (最高位)</summary>
        private const byte ContinuationBit = 0x80;

        /// <summary>数据位掩码 (低7位)</summary>
        private const byte DataMask = 0x7F;

        /// <summary>数据位数</summary>
        private const int DataBits = 7;

        /// <summary>32位整数最大编码字节数</summary>
        public const int MaxVarInt32Bytes = 5;

        /// <summary>64位整数最大编码字节数</summary>
        public const int MaxVarInt64Bytes = 10;

        #endregion

        #region 32位无符号整数解码

        /// <summary>
        /// 解码 32 位无符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadVarUInt32(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            uint result = 0;
            int shift = 0;
            bytesRead = 0;

            while (bytesRead < buffer.Length)
            {
                byte b = buffer[bytesRead++];
                result |= (uint)(b & DataMask) << shift;

                // 检查继续位
                if ((b & ContinuationBit) == 0)
                    return result;

                shift += DataBits;

                // 防止溢出
                if (shift >= 35)
                    throw new OverflowException("VarInt32 编码超过最大长度");
            }

            throw new InvalidOperationException("VarInt32 编码不完整");
        }

        /// <summary>
        /// 尝试解码 32 位无符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="value">解码的值</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadVarUInt32(ReadOnlySpan<byte> buffer, out uint value, out int bytesRead)
        {
            value = 0;
            bytesRead = 0;
            int shift = 0;

            while (bytesRead < buffer.Length && bytesRead < MaxVarInt32Bytes)
            {
                byte b = buffer[bytesRead++];
                value |= (uint)(b & DataMask) << shift;

                if ((b & ContinuationBit) == 0)
                    return true;

                shift += DataBits;
            }

            // 编码不完整或溢出
            value = 0;
            bytesRead = 0;
            return false;
        }

        /// <summary>
        /// 解码 32 位无符号整数 (从指定位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadVarUInt32(ReadOnlySpan<byte> buffer, int offset, out int bytesRead)
        {
            return ReadVarUInt32(buffer.Slice(offset), out bytesRead);
        }

        #endregion

        #region 32位有符号整数解码

        /// <summary>
        /// 解码 32 位有符号整数 (ZigZag 编码)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadVarInt32(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            uint zigzag = ReadVarUInt32(buffer, out bytesRead);
            // ZigZag 解码: 将无符号数还原为有符号数
            return (int)((zigzag >> 1) ^ -(int)(zigzag & 1));
        }

        /// <summary>
        /// 尝试解码 32 位有符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="value">解码的值</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadVarInt32(ReadOnlySpan<byte> buffer, out int value, out int bytesRead)
        {
            if (TryReadVarUInt32(buffer, out uint zigzag, out bytesRead))
            {
                value = (int)((zigzag >> 1) ^ -(int)(zigzag & 1));
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// 解码 32 位有符号整数 (从指定位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadVarInt32(ReadOnlySpan<byte> buffer, int offset, out int bytesRead)
        {
            return ReadVarInt32(buffer.Slice(offset), out bytesRead);
        }

        #endregion

        #region 64位无符号整数解码

        /// <summary>
        /// 解码 64 位无符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadVarUInt64(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            ulong result = 0;
            int shift = 0;
            bytesRead = 0;

            while (bytesRead < buffer.Length)
            {
                byte b = buffer[bytesRead++];
                result |= (ulong)(b & DataMask) << shift;

                if ((b & ContinuationBit) == 0)
                    return result;

                shift += DataBits;

                if (shift >= 70)
                    throw new OverflowException("VarInt64 编码超过最大长度");
            }

            throw new InvalidOperationException("VarInt64 编码不完整");
        }

        /// <summary>
        /// 尝试解码 64 位无符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="value">解码的值</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadVarUInt64(ReadOnlySpan<byte> buffer, out ulong value, out int bytesRead)
        {
            value = 0;
            bytesRead = 0;
            int shift = 0;

            while (bytesRead < buffer.Length && bytesRead < MaxVarInt64Bytes)
            {
                byte b = buffer[bytesRead++];
                value |= (ulong)(b & DataMask) << shift;

                if ((b & ContinuationBit) == 0)
                    return true;

                shift += DataBits;
            }

            value = 0;
            bytesRead = 0;
            return false;
        }

        /// <summary>
        /// 解码 64 位无符号整数 (从指定位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadVarUInt64(ReadOnlySpan<byte> buffer, int offset, out int bytesRead)
        {
            return ReadVarUInt64(buffer.Slice(offset), out bytesRead);
        }

        #endregion

        #region 64位有符号整数解码

        /// <summary>
        /// 解码 64 位有符号整数 (ZigZag 编码)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadVarInt64(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            ulong zigzag = ReadVarUInt64(buffer, out bytesRead);
            return (long)((zigzag >> 1) ^ -(long)(zigzag & 1));
        }

        /// <summary>
        /// 尝试解码 64 位有符号整数
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="value">解码的值</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadVarInt64(ReadOnlySpan<byte> buffer, out long value, out int bytesRead)
        {
            if (TryReadVarUInt64(buffer, out ulong zigzag, out bytesRead))
            {
                value = (long)((zigzag >> 1) ^ -(long)(zigzag & 1));
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// 解码 64 位有符号整数 (从指定位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>解码的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadVarInt64(ReadOnlySpan<byte> buffer, int offset, out int bytesRead)
        {
            return ReadVarInt64(buffer.Slice(offset), out bytesRead);
        }

        #endregion

        #region 原始 VarInt 解码 (无 ZigZag)

        /// <summary>
        /// 解码 32 位有符号整数 (原始格式，无 ZigZag)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadRawVarInt32(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            return (int)ReadVarUInt32(buffer, out bytesRead);
        }

        /// <summary>
        /// 解码 64 位有符号整数 (原始格式，无 ZigZag)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadRawVarInt64(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            return (long)ReadVarUInt64(buffer, out bytesRead);
        }

        #endregion

        #region 长度前缀解码

        /// <summary>
        /// 解码长度前缀
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>长度值，-1 表示 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadLengthPrefix(ReadOnlySpan<byte> buffer, out int bytesRead)
        {
            return ReadVarInt32(buffer, out bytesRead);
        }

        /// <summary>
        /// 尝试解码长度前缀
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="length">长度值</param>
        /// <param name="bytesRead">读取的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadLengthPrefix(ReadOnlySpan<byte> buffer, out int length, out int bytesRead)
        {
            return TryReadVarInt32(buffer, out length, out bytesRead);
        }

        #endregion

        #region 批量解码

        /// <summary>
        /// 批量解码 32 位无符号整数数组
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="values">目标数组</param>
        /// <returns>读取的总字节数</returns>
        public static int ReadVarUInt32Array(ReadOnlySpan<byte> buffer, Span<uint> values)
        {
            int totalRead = 0;
            var remaining = buffer;

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = ReadVarUInt32(remaining, out int bytesRead);
                totalRead += bytesRead;
                remaining = remaining.Slice(bytesRead);
            }

            return totalRead;
        }

        /// <summary>
        /// 批量解码 32 位有符号整数数组
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="values">目标数组</param>
        /// <returns>读取的总字节数</returns>
        public static int ReadVarInt32Array(ReadOnlySpan<byte> buffer, Span<int> values)
        {
            int totalRead = 0;
            var remaining = buffer;

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = ReadVarInt32(remaining, out int bytesRead);
                totalRead += bytesRead;
                remaining = remaining.Slice(bytesRead);
            }

            return totalRead;
        }

        #endregion

        #region 差分解码 (Delta Decoding)

        /// <summary>
        /// 使用差分解码读取有序整数数组
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="values">目标数组</param>
        /// <returns>读取的总字节数</returns>
        public static int ReadDeltaEncodedArray(ReadOnlySpan<byte> buffer, Span<int> values)
        {
            if (values.IsEmpty)
                return 0;

            int totalRead = 0;
            var remaining = buffer;

            // 读取第一个值
            values[0] = ReadVarInt32(remaining, out int bytesRead);
            totalRead += bytesRead;
            remaining = remaining.Slice(bytesRead);

            // 读取差值并累加
            for (int i = 1; i < values.Length; i++)
            {
                int delta = ReadVarInt32(remaining, out bytesRead);
                values[i] = values[i - 1] + delta;
                totalRead += bytesRead;
                remaining = remaining.Slice(bytesRead);
            }

            return totalRead;
        }

        #endregion

        #region 跳过操作

        /// <summary>
        /// 跳过一个 VarInt
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <returns>跳过的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SkipVarInt(ReadOnlySpan<byte> buffer)
        {
            int index = 0;
            while (index < buffer.Length && (buffer[index] & ContinuationBit) != 0)
            {
                index++;
            }
            return index + 1; // 包含最后一个字节
        }

        /// <summary>
        /// 跳过多个 VarInt
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="count">要跳过的数量</param>
        /// <returns>跳过的总字节数</returns>
        public static int SkipVarInts(ReadOnlySpan<byte> buffer, int count)
        {
            int totalSkipped = 0;
            var remaining = buffer;

            for (int i = 0; i < count; i++)
            {
                int skipped = SkipVarInt(remaining);
                totalSkipped += skipped;
                remaining = remaining.Slice(skipped);
            }

            return totalSkipped;
        }

        #endregion

        #region 预览操作

        /// <summary>
        /// 预览 VarInt 的大小 (不解码)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <returns>VarInt 的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PeekVarIntSize(ReadOnlySpan<byte> buffer)
        {
            return SkipVarInt(buffer);
        }

        /// <summary>
        /// 预览 VarInt 值 (不移动位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <returns>VarInt 值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PeekVarInt32(ReadOnlySpan<byte> buffer)
        {
            return ReadVarInt32(buffer, out _);
        }

        /// <summary>
        /// 预览 VarUInt 值 (不移动位置)
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <returns>VarUInt 值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PeekVarUInt32(ReadOnlySpan<byte> buffer)
        {
            return ReadVarUInt32(buffer, out _);
        }

        #endregion
    }
}
