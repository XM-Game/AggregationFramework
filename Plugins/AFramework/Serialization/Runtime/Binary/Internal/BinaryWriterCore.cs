// ==========================================================
// 文件名：BinaryWriterCore.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Buffers, System.Text
// ==========================================================

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// 二进制写入器核心实现
    /// <para>提供高性能的二进制数据写入功能</para>
    /// <para>支持自动扩容、零拷贝写入、VarInt 编码</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用 ArrayPool 减少内存分配
    /// 2. 支持 Span/Memory 零拷贝操作
    /// 3. 内联关键方法提升性能
    /// 
    /// 使用示例:
    /// <code>
    /// using var writer = new BinaryWriterCore();
    /// writer.WriteInt32(42);
    /// writer.WriteString("Hello");
    /// byte[] data = writer.ToArray();
    /// </code>
    /// </remarks>
    internal sealed class BinaryWriterCore : IDisposable
    {
        #region 常量

        /// <summary>默认初始缓冲区大小</summary>
        private const int DefaultInitialCapacity = 256;

        /// <summary>最大缓冲区大小</summary>
        private const int MaxBufferSize = 1024 * 1024 * 1024; // 1GB

        /// <summary>增长因子</summary>
        private const float GrowthFactor = 2.0f;

        #endregion

        #region 字段

        /// <summary>内部缓冲区</summary>
        private byte[] _buffer;

        /// <summary>当前写入位置</summary>
        private int _position;

        /// <summary>是否从池中租用</summary>
        private bool _isRented;

        /// <summary>是否已释放</summary>
        private bool _disposed;

        /// <summary>UTF-8 编码器</summary>
        private static readonly Encoding s_utf8 = Encoding.UTF8;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建二进制写入器
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        public BinaryWriterCore(int initialCapacity = DefaultInitialCapacity)
        {
            if (initialCapacity <= 0)
                initialCapacity = DefaultInitialCapacity;

            _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            _isRented = true;
            _position = 0;
        }

        /// <summary>
        /// 使用外部缓冲区创建写入器
        /// </summary>
        /// <param name="buffer">外部缓冲区</param>
        public BinaryWriterCore(byte[] buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _isRented = false;
            _position = 0;
        }

        #endregion

        #region 属性

        /// <summary>当前写入位置</summary>
        public int Position => _position;

        /// <summary>缓冲区容量</summary>
        public int Capacity => _buffer.Length;

        /// <summary>剩余可写空间</summary>
        public int Remaining => _buffer.Length - _position;

        /// <summary>已写入的数据</summary>
        public ReadOnlySpan<byte> WrittenSpan => _buffer.AsSpan(0, _position);

        /// <summary>已写入的内存</summary>
        public ReadOnlyMemory<byte> WrittenMemory => _buffer.AsMemory(0, _position);

        #endregion

        #region 基础类型写入

        /// <summary>写入布尔值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBool(bool value)
        {
            EnsureCapacity(1);
            _buffer[_position++] = value ? (byte)1 : (byte)0;
        }

        /// <summary>写入字节</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            EnsureCapacity(1);
            _buffer[_position++] = value;
        }

        /// <summary>写入有符号字节</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value)
        {
            EnsureCapacity(1);
            _buffer[_position++] = (byte)value;
        }

        /// <summary>写入 16 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value)
        {
            EnsureCapacity(2);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 2;
        }

        /// <summary>写入 16 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
        {
            EnsureCapacity(2);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 2;
        }

        /// <summary>写入 32 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value)
        {
            EnsureCapacity(4);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 4;
        }

        /// <summary>写入 32 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value)
        {
            EnsureCapacity(4);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 4;
        }

        /// <summary>写入 64 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value)
        {
            EnsureCapacity(8);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 8;
        }

        /// <summary>写入 64 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value)
        {
            EnsureCapacity(8);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 8;
        }

        /// <summary>写入单精度浮点数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(float value)
        {
            EnsureCapacity(4);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 4;
        }

        /// <summary>写入双精度浮点数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            EnsureCapacity(8);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 8;
        }

        /// <summary>写入十进制数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDecimal(decimal value)
        {
            EnsureCapacity(16);
            var bits = decimal.GetBits(value);
            WriteInt32(bits[0]);
            WriteInt32(bits[1]);
            WriteInt32(bits[2]);
            WriteInt32(bits[3]);
        }

        /// <summary>写入字符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChar(char value)
        {
            EnsureCapacity(2);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += 2;
        }

        #endregion

        #region VarInt 写入

        /// <summary>写入变长 32 位整数 (ZigZag 编码)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVarInt32(int value)
        {
            EnsureCapacity(VarIntEncoder.MaxVarInt32Bytes);
            _position += VarIntEncoder.WriteVarInt32(_buffer.AsSpan(_position), value);
        }

        /// <summary>写入变长 32 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVarUInt32(uint value)
        {
            EnsureCapacity(VarIntEncoder.MaxVarInt32Bytes);
            _position += VarIntEncoder.WriteVarUInt32(_buffer.AsSpan(_position), value);
        }

        /// <summary>写入变长 64 位整数 (ZigZag 编码)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVarInt64(long value)
        {
            EnsureCapacity(VarIntEncoder.MaxVarInt64Bytes);
            _position += VarIntEncoder.WriteVarInt64(_buffer.AsSpan(_position), value);
        }

        /// <summary>写入变长 64 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVarUInt64(ulong value)
        {
            EnsureCapacity(VarIntEncoder.MaxVarInt64Bytes);
            _position += VarIntEncoder.WriteVarUInt64(_buffer.AsSpan(_position), value);
        }

        #endregion

        #region 字符串写入

        /// <summary>写入字符串 (带长度前缀)</summary>
        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteVarInt32(-1); // null 标记
                return;
            }

            if (value.Length == 0)
            {
                WriteVarInt32(0); // 空字符串
                return;
            }

            // 计算 UTF-8 编码后的字节数
            int maxByteCount = s_utf8.GetMaxByteCount(value.Length);
            EnsureCapacity(VarIntEncoder.MaxVarInt32Bytes + maxByteCount);

            // 先预留长度前缀的位置
            int lengthPosition = _position;
            _position += VarIntEncoder.MaxVarInt32Bytes; // 预留最大空间

            // 编码字符串
            int byteCount = s_utf8.GetBytes(value, _buffer.AsSpan(_position));

            // 回写实际长度
            int lengthSize = VarIntEncoder.WriteVarInt32(_buffer.AsSpan(lengthPosition), byteCount);

            // 如果长度前缀实际使用的空间小于预留空间，需要移动数据
            if (lengthSize < VarIntEncoder.MaxVarInt32Bytes)
            {
                int shift = VarIntEncoder.MaxVarInt32Bytes - lengthSize;
                Buffer.BlockCopy(_buffer, _position, _buffer, lengthPosition + lengthSize, byteCount);
                _position = lengthPosition + lengthSize + byteCount;
            }
            else
            {
                _position += byteCount;
            }
        }

        /// <summary>写入字符串 (不带长度前缀)</summary>
        public void WriteStringRaw(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            int maxByteCount = s_utf8.GetMaxByteCount(value.Length);
            EnsureCapacity(maxByteCount);
            int byteCount = s_utf8.GetBytes(value, _buffer.AsSpan(_position));
            _position += byteCount;
        }

        /// <summary>写入固定长度字符串</summary>
        public void WriteFixedString(string value, int maxLength)
        {
            if (value == null)
                value = string.Empty;

            if (value.Length > maxLength)
                value = value.Substring(0, maxLength);

            int maxByteCount = s_utf8.GetMaxByteCount(maxLength);
            EnsureCapacity(maxByteCount);

            int byteCount = s_utf8.GetBytes(value, _buffer.AsSpan(_position, maxByteCount));
            _position += byteCount;

            // 填充剩余空间
            int padding = maxByteCount - byteCount;
            if (padding > 0)
            {
                _buffer.AsSpan(_position, padding).Clear();
                _position += padding;
            }
        }

        #endregion

        #region 字节数组写入

        /// <summary>写入字节数组 (带长度前缀)</summary>
        public void WriteBytes(ReadOnlySpan<byte> value)
        {
            WriteVarInt32(value.Length);
            if (value.Length > 0)
            {
                EnsureCapacity(value.Length);
                value.CopyTo(_buffer.AsSpan(_position));
                _position += value.Length;
            }
        }

        /// <summary>写入字节数组 (可为 null)</summary>
        public void WriteNullableBytes(byte[] value)
        {
            if (value == null)
            {
                WriteVarInt32(-1);
                return;
            }
            WriteBytes(value);
        }

        /// <summary>写入原始字节 (不带长度前缀)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRaw(ReadOnlySpan<byte> value)
        {
            if (value.IsEmpty)
                return;

            EnsureCapacity(value.Length);
            value.CopyTo(_buffer.AsSpan(_position));
            _position += value.Length;
        }

        #endregion

        #region 特殊类型写入

        /// <summary>写入 GUID</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteGuid(Guid value)
        {
            EnsureCapacity(16);
            value.TryWriteBytes(_buffer.AsSpan(_position));
            _position += 16;
        }

        /// <summary>写入日期时间</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDateTime(DateTime value)
        {
            WriteInt64(value.ToBinary());
        }

        /// <summary>写入时间跨度</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTimeSpan(TimeSpan value)
        {
            WriteInt64(value.Ticks);
        }

        /// <summary>写入日期时间偏移</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDateTimeOffset(DateTimeOffset value)
        {
            WriteInt64(value.Ticks);
            WriteInt16((short)value.Offset.TotalMinutes);
        }

        #endregion

        #region 非托管类型写入

        /// <summary>写入非托管类型值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanaged<T>(T value) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            EnsureCapacity(size);
            Unsafe.WriteUnaligned(ref _buffer[_position], value);
            _position += size;
        }

        /// <summary>写入非托管类型数组</summary>
        public void WriteUnmanagedArray<T>(ReadOnlySpan<T> values) where T : unmanaged
        {
            if (values.IsEmpty)
            {
                WriteVarInt32(0);
                return;
            }

            WriteVarInt32(values.Length);
            int byteSize = values.Length * Unsafe.SizeOf<T>();
            EnsureCapacity(byteSize);

            var bytes = MemoryMarshal.AsBytes(values);
            bytes.CopyTo(_buffer.AsSpan(_position));
            _position += byteSize;
        }

        #endregion

        #region 集合头写入

        /// <summary>写入数组头 (元素数量)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArrayHeader(int count)
        {
            WriteVarInt32(count);
        }

        /// <summary>写入字典头 (键值对数量)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMapHeader(int count)
        {
            WriteVarInt32(count);
        }

        #endregion

        #region 类型码写入

        /// <summary>写入类型码</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTypeCode(byte typeCode)
        {
            WriteByte(typeCode);
        }

        /// <summary>写入 null 标记</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNull()
        {
            WriteByte(BinaryFormat.Null);
        }

        #endregion

        #region 缓冲区管理

        /// <summary>
        /// 确保有足够的容量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int additionalSize)
        {
            int required = _position + additionalSize;
            if (required <= _buffer.Length)
                return;

            Grow(required);
        }

        /// <summary>
        /// 扩展缓冲区
        /// </summary>
        private void Grow(int minCapacity)
        {
            int newCapacity = (int)(_buffer.Length * GrowthFactor);
            if (newCapacity < minCapacity)
                newCapacity = minCapacity;
            if (newCapacity > MaxBufferSize)
                newCapacity = MaxBufferSize;
            if (newCapacity < minCapacity)
                throw new InvalidOperationException($"缓冲区大小超过最大限制 ({MaxBufferSize} 字节)");

            var newBuffer = ArrayPool<byte>.Shared.Rent(newCapacity);
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _position);

            if (_isRented)
                ArrayPool<byte>.Shared.Return(_buffer);

            _buffer = newBuffer;
            _isRented = true;
        }

        /// <summary>
        /// 获取可写入的 Span
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int size)
        {
            EnsureCapacity(size);
            return _buffer.AsSpan(_position, size);
        }

        /// <summary>
        /// 推进写入位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (count < 0 || _position + count > _buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            _position += count;
        }

        /// <summary>
        /// 重置写入位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _position = 0;
        }

        /// <summary>
        /// 获取已写入的数据副本
        /// </summary>
        public byte[] ToArray()
        {
            var result = new byte[_position];
            Buffer.BlockCopy(_buffer, 0, result, 0, _position);
            return result;
        }

        /// <summary>
        /// 复制已写入的数据到目标缓冲区
        /// </summary>
        public int CopyTo(Span<byte> destination)
        {
            if (destination.Length < _position)
                throw new ArgumentException("目标缓冲区太小", nameof(destination));

            _buffer.AsSpan(0, _position).CopyTo(destination);
            return _position;
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_isRented && _buffer != null)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
            }

            _buffer = null;
            _disposed = true;
        }

        #endregion
    }
}
