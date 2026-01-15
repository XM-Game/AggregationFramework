// ==========================================================
// 文件名：VarIntEncoder.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 变长整数编码器
    /// <para>提供高性能的变长整数 (VarInt) 编码功能</para>
    /// <para>支持 32 位和 64 位整数的变长编码</para>
    /// </summary>
    /// <remarks>
    /// VarInt 编码规则:
    /// - 每个字节的最高位 (MSB) 表示是否有后续字节
    /// - 低 7 位存储实际数据
    /// - 小整数使用更少的字节，大整数使用更多字节
    /// 
    /// 编码示例:
    /// <code>
    /// 值 0-127:      1 字节  (0xxxxxxx)
    /// 值 128-16383:  2 字节  (1xxxxxxx 0xxxxxxx)
    /// 值 16384+:     3+ 字节 (1xxxxxxx 1xxxxxxx ...)
    /// </code>
    /// 
    /// 使用示例:
    /// <code>
    /// // 编码
    /// Span&lt;byte&gt; buffer = stackalloc byte[5];
    /// int written = VarIntEncoder.WriteVarInt32(buffer, 12345);
    /// 
    /// // 获取编码大小
    /// int size = VarIntEncoder.GetVarInt32Size(12345);
    /// </code>
    /// </remarks>
    public static class VarIntEncoder
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

        #region 32位无符号整数编码

        /// <summary>
        /// 编码 32 位无符号整数
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteVarUInt32(Span<byte> buffer, uint value)
        {
            int index = 0;

            // 循环写入，每次写入 7 位
            while (value > DataMask)
            {
                buffer[index++] = (byte)((value & DataMask) | ContinuationBit);
                value >>= DataBits;
            }

            // 写入最后一个字节 (无继续位)
            buffer[index++] = (byte)value;
            return index;
        }

        /// <summary>
        /// 编码 32 位无符号整数 (带边界检查)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteVarUInt32(Span<byte> buffer, uint value, out int bytesWritten)
        {
            bytesWritten = 0;
            int required = GetVarUInt32Size(value);

            if (buffer.Length < required)
                return false;

            bytesWritten = WriteVarUInt32(buffer, value);
            return true;
        }

        /// <summary>
        /// 获取 32 位无符号整数的编码大小
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>编码所需的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarUInt32Size(uint value)
        {
            // 使用位运算快速计算
            if (value < (1u << 7)) return 1;
            if (value < (1u << 14)) return 2;
            if (value < (1u << 21)) return 3;
            if (value < (1u << 28)) return 4;
            return 5;
        }

        #endregion

        #region 32位有符号整数编码

        /// <summary>
        /// 编码 32 位有符号整数 (使用 ZigZag 编码)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteVarInt32(Span<byte> buffer, int value)
        {
            // ZigZag 编码: 将有符号数映射到无符号数
            // 正数 n -> 2n, 负数 n -> -2n-1
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            return WriteVarUInt32(buffer, zigzag);
        }

        /// <summary>
        /// 编码 32 位有符号整数 (带边界检查)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteVarInt32(Span<byte> buffer, int value, out int bytesWritten)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            return TryWriteVarUInt32(buffer, zigzag, out bytesWritten);
        }

        /// <summary>
        /// 获取 32 位有符号整数的编码大小
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>编码所需的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarInt32Size(int value)
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));
            return GetVarUInt32Size(zigzag);
        }

        #endregion

        #region 64位无符号整数编码

        /// <summary>
        /// 编码 64 位无符号整数
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteVarUInt64(Span<byte> buffer, ulong value)
        {
            int index = 0;

            while (value > DataMask)
            {
                buffer[index++] = (byte)((value & DataMask) | ContinuationBit);
                value >>= DataBits;
            }

            buffer[index++] = (byte)value;
            return index;
        }

        /// <summary>
        /// 编码 64 位无符号整数 (带边界检查)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteVarUInt64(Span<byte> buffer, ulong value, out int bytesWritten)
        {
            bytesWritten = 0;
            int required = GetVarUInt64Size(value);

            if (buffer.Length < required)
                return false;

            bytesWritten = WriteVarUInt64(buffer, value);
            return true;
        }

        /// <summary>
        /// 获取 64 位无符号整数的编码大小
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>编码所需的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarUInt64Size(ulong value)
        {
            if (value < (1ul << 7)) return 1;
            if (value < (1ul << 14)) return 2;
            if (value < (1ul << 21)) return 3;
            if (value < (1ul << 28)) return 4;
            if (value < (1ul << 35)) return 5;
            if (value < (1ul << 42)) return 6;
            if (value < (1ul << 49)) return 7;
            if (value < (1ul << 56)) return 8;
            if (value < (1ul << 63)) return 9;
            return 10;
        }

        #endregion

        #region 64位有符号整数编码

        /// <summary>
        /// 编码 64 位有符号整数 (使用 ZigZag 编码)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteVarInt64(Span<byte> buffer, long value)
        {
            // ZigZag 编码
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));
            return WriteVarUInt64(buffer, zigzag);
        }

        /// <summary>
        /// 编码 64 位有符号整数 (带边界检查)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="value">要编码的值</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWriteVarInt64(Span<byte> buffer, long value, out int bytesWritten)
        {
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));
            return TryWriteVarUInt64(buffer, zigzag, out bytesWritten);
        }

        /// <summary>
        /// 获取 64 位有符号整数的编码大小
        /// </summary>
        /// <param name="value">要编码的值</param>
        /// <returns>编码所需的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetVarInt64Size(long value)
        {
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));
            return GetVarUInt64Size(zigzag);
        }

        #endregion

        #region 原始 VarInt 编码 (无 ZigZag)

        /// <summary>
        /// 编码 32 位有符号整数 (原始格式，无 ZigZag)
        /// </summary>
        /// <remarks>
        /// 负数会使用 5 字节，适用于非负数居多的场景
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteRawVarInt32(Span<byte> buffer, int value)
        {
            return WriteVarUInt32(buffer, (uint)value);
        }

        /// <summary>
        /// 编码 64 位有符号整数 (原始格式，无 ZigZag)
        /// </summary>
        /// <remarks>
        /// 负数会使用 10 字节，适用于非负数居多的场景
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteRawVarInt64(Span<byte> buffer, long value)
        {
            return WriteVarUInt64(buffer, (ulong)value);
        }

        #endregion

        #region 长度前缀编码

        /// <summary>
        /// 编码长度前缀 (用于字符串、数组等)
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="length">长度值</param>
        /// <returns>写入的字节数</returns>
        /// <remarks>
        /// 长度值 -1 表示 null
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteLengthPrefix(Span<byte> buffer, int length)
        {
            // -1 表示 null，使用 ZigZag 编码
            return WriteVarInt32(buffer, length);
        }

        /// <summary>
        /// 获取长度前缀的编码大小
        /// </summary>
        /// <param name="length">长度值</param>
        /// <returns>编码所需的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLengthPrefixSize(int length)
        {
            return GetVarInt32Size(length);
        }

        #endregion

        #region 批量编码

        /// <summary>
        /// 批量编码 32 位无符号整数数组
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="values">要编码的值数组</param>
        /// <returns>写入的总字节数</returns>
        public static int WriteVarUInt32Array(Span<byte> buffer, ReadOnlySpan<uint> values)
        {
            int totalWritten = 0;
            var remaining = buffer;

            foreach (var value in values)
            {
                int written = WriteVarUInt32(remaining, value);
                totalWritten += written;
                remaining = remaining.Slice(written);
            }

            return totalWritten;
        }

        /// <summary>
        /// 批量编码 32 位有符号整数数组
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="values">要编码的值数组</param>
        /// <returns>写入的总字节数</returns>
        public static int WriteVarInt32Array(Span<byte> buffer, ReadOnlySpan<int> values)
        {
            int totalWritten = 0;
            var remaining = buffer;

            foreach (var value in values)
            {
                int written = WriteVarInt32(remaining, value);
                totalWritten += written;
                remaining = remaining.Slice(written);
            }

            return totalWritten;
        }

        /// <summary>
        /// 计算批量编码 32 位无符号整数数组所需的大小
        /// </summary>
        /// <param name="values">要编码的值数组</param>
        /// <returns>编码所需的总字节数</returns>
        public static int GetVarUInt32ArraySize(ReadOnlySpan<uint> values)
        {
            int totalSize = 0;
            foreach (var value in values)
            {
                totalSize += GetVarUInt32Size(value);
            }
            return totalSize;
        }

        /// <summary>
        /// 计算批量编码 32 位有符号整数数组所需的大小
        /// </summary>
        /// <param name="values">要编码的值数组</param>
        /// <returns>编码所需的总字节数</returns>
        public static int GetVarInt32ArraySize(ReadOnlySpan<int> values)
        {
            int totalSize = 0;
            foreach (var value in values)
            {
                totalSize += GetVarInt32Size(value);
            }
            return totalSize;
        }

        #endregion

        #region 差分编码 (Delta Encoding)

        /// <summary>
        /// 使用差分编码写入有序整数数组
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="values">有序的值数组</param>
        /// <returns>写入的总字节数</returns>
        /// <remarks>
        /// 差分编码适用于有序数组，存储相邻元素的差值
        /// 对于递增序列，差值通常较小，编码更紧凑
        /// </remarks>
        public static int WriteDeltaEncodedArray(Span<byte> buffer, ReadOnlySpan<int> values)
        {
            if (values.IsEmpty)
                return 0;

            int totalWritten = 0;
            var remaining = buffer;

            // 写入第一个值
            int written = WriteVarInt32(remaining, values[0]);
            totalWritten += written;
            remaining = remaining.Slice(written);

            // 写入差值
            for (int i = 1; i < values.Length; i++)
            {
                int delta = values[i] - values[i - 1];
                written = WriteVarInt32(remaining, delta);
                totalWritten += written;
                remaining = remaining.Slice(written);
            }

            return totalWritten;
        }

        /// <summary>
        /// 计算差分编码数组所需的大小
        /// </summary>
        /// <param name="values">有序的值数组</param>
        /// <returns>编码所需的总字节数</returns>
        public static int GetDeltaEncodedArraySize(ReadOnlySpan<int> values)
        {
            if (values.IsEmpty)
                return 0;

            int totalSize = GetVarInt32Size(values[0]);

            for (int i = 1; i < values.Length; i++)
            {
                int delta = values[i] - values[i - 1];
                totalSize += GetVarInt32Size(delta);
            }

            return totalSize;
        }

        #endregion
    }
}
