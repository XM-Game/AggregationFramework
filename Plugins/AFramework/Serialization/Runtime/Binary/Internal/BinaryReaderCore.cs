// ==========================================================
// 文件名：BinaryReaderCore.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// 二进制读取器核心实现
    /// <para>提供高性能的二进制数据读取功能</para>
    /// <para>支持零拷贝读取、VarInt 解码、边界检查</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用 ReadOnlySpan 避免数据复制
    /// 2. 内联关键方法提升性能
    /// 3. 提供安全的边界检查
    /// 
    /// 使用示例:
    /// <code>
    /// var reader = new BinaryReaderCore(data);
    /// int value = reader.ReadInt32();
    /// string str = reader.ReadString();
    /// </code>
    /// </remarks>
    internal ref struct BinaryReaderCore
    {
        #region 字段

        /// <summary>数据缓冲区</summary>
        private readonly ReadOnlySpan<byte> _buffer;

        /// <summary>当前读取位置</summary>
        private int _position;

        /// <summary>UTF-8 编码器</summary>
        private static readonly Encoding s_utf8 = Encoding.UTF8;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建二进制读取器
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        public BinaryReaderCore(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        /// <summary>
        /// 创建二进制读取器 (从指定位置开始)
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="offset">起始偏移量</param>
        public BinaryReaderCore(ReadOnlySpan<byte> buffer, int offset)
        {
            _buffer = buffer;
            _position = offset;
        }

        #endregion

        #region 属性

        /// <summary>当前读取位置</summary>
        public int Position => _position;

        /// <summary>数据总长度</summary>
        public int Length => _buffer.Length;

        /// <summary>剩余可读字节数</summary>
        public int Remaining => _buffer.Length - _position;

        /// <summary>是否已到达末尾</summary>
        public bool IsEnd => _position >= _buffer.Length;

        /// <summary>剩余数据</summary>
        public ReadOnlySpan<byte> RemainingSpan => _buffer.Slice(_position);

        #endregion

        #region 基础类型读取

        /// <summary>读取布尔值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            EnsureAvailable(1);
            return _buffer[_position++] != 0;
        }

        /// <summary>读取字节</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            EnsureAvailable(1);
            return _buffer[_position++];
        }

        /// <summary>读取有符号字节</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            EnsureAvailable(1);
            return (sbyte)_buffer[_position++];
        }

        /// <summary>读取 16 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            EnsureAvailable(2);
            var value = Unsafe.ReadUnaligned<short>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 2;
            return value;
        }

        /// <summary>读取 16 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            EnsureAvailable(2);
            var value = Unsafe.ReadUnaligned<ushort>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 2;
            return value;
        }

        /// <summary>读取 32 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            EnsureAvailable(4);
            var value = Unsafe.ReadUnaligned<int>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 4;
            return value;
        }

        /// <summary>读取 32 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            EnsureAvailable(4);
            var value = Unsafe.ReadUnaligned<uint>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 4;
            return value;
        }

        /// <summary>读取 64 位整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            EnsureAvailable(8);
            var value = Unsafe.ReadUnaligned<long>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 8;
            return value;
        }

        /// <summary>读取 64 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            EnsureAvailable(8);
            var value = Unsafe.ReadUnaligned<ulong>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 8;
            return value;
        }

        /// <summary>读取单精度浮点数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
        {
            EnsureAvailable(4);
            var value = Unsafe.ReadUnaligned<float>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 4;
            return value;
        }

        /// <summary>读取双精度浮点数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            EnsureAvailable(8);
            var value = Unsafe.ReadUnaligned<double>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 8;
            return value;
        }

        /// <summary>读取十进制数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ReadDecimal()
        {
            EnsureAvailable(16);
            var bits = new int[4];
            bits[0] = ReadInt32();
            bits[1] = ReadInt32();
            bits[2] = ReadInt32();
            bits[3] = ReadInt32();
            return new decimal(bits);
        }

        /// <summary>读取字符</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            EnsureAvailable(2);
            var value = Unsafe.ReadUnaligned<char>(ref Unsafe.AsRef(_buffer[_position]));
            _position += 2;
            return value;
        }

        #endregion

        #region VarInt 读取

        /// <summary>读取变长 32 位整数 (ZigZag 编码)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadVarInt32()
        {
            var value = VarIntDecoder.ReadVarInt32(_buffer.Slice(_position), out int bytesRead);
            _position += bytesRead;
            return value;
        }

        /// <summary>读取变长 32 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadVarUInt32()
        {
            var value = VarIntDecoder.ReadVarUInt32(_buffer.Slice(_position), out int bytesRead);
            _position += bytesRead;
            return value;
        }

        /// <summary>读取变长 64 位整数 (ZigZag 编码)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadVarInt64()
        {
            var value = VarIntDecoder.ReadVarInt64(_buffer.Slice(_position), out int bytesRead);
            _position += bytesRead;
            return value;
        }

        /// <summary>读取变长 64 位无符号整数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadVarUInt64()
        {
            var value = VarIntDecoder.ReadVarUInt64(_buffer.Slice(_position), out int bytesRead);
            _position += bytesRead;
            return value;
        }

        #endregion

        #region 字符串读取

        /// <summary>读取字符串 (带长度前缀)</summary>
        public string ReadString()
        {
            int length = ReadVarInt32();

            if (length < 0)
                return null; // null 标记

            if (length == 0)
                return string.Empty;

            EnsureAvailable(length);
            var str = s_utf8.GetString(_buffer.Slice(_position, length));
            _position += length;
            return str;
        }

        /// <summary>读取字符串 (可为 null)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadNullableString()
        {
            return ReadString();
        }

        /// <summary>读取固定长度字符串</summary>
        public string ReadFixedString(int maxByteLength)
        {
            EnsureAvailable(maxByteLength);
            var span = _buffer.Slice(_position, maxByteLength);

            // 查找实际字符串结束位置 (null 终止符)
            int actualLength = span.IndexOf((byte)0);
            if (actualLength < 0)
                actualLength = maxByteLength;

            var str = s_utf8.GetString(span.Slice(0, actualLength));
            _position += maxByteLength;
            return str;
        }

        /// <summary>读取指定长度的字符串 (不带长度前缀)</summary>
        public string ReadStringRaw(int byteLength)
        {
            if (byteLength <= 0)
                return string.Empty;

            EnsureAvailable(byteLength);
            var str = s_utf8.GetString(_buffer.Slice(_position, byteLength));
            _position += byteLength;
            return str;
        }

        #endregion

        #region 字节数组读取

        /// <summary>读取字节数组 (带长度前缀)</summary>
        public byte[] ReadBytes()
        {
            int length = ReadVarInt32();

            if (length < 0)
                return null;

            if (length == 0)
                return Array.Empty<byte>();

            EnsureAvailable(length);
            var result = _buffer.Slice(_position, length).ToArray();
            _position += length;
            return result;
        }

        /// <summary>读取字节数组到指定缓冲区</summary>
        public int ReadBytes(Span<byte> destination)
        {
            int length = ReadVarInt32();

            if (length <= 0)
                return 0;

            if (destination.Length < length)
                throw new ArgumentException("目标缓冲区太小", nameof(destination));

            EnsureAvailable(length);
            _buffer.Slice(_position, length).CopyTo(destination);
            _position += length;
            return length;
        }

        /// <summary>读取原始字节 (不带长度前缀)</summary>
        public byte[] ReadRaw(int count)
        {
            if (count <= 0)
                return Array.Empty<byte>();

            EnsureAvailable(count);
            var result = _buffer.Slice(_position, count).ToArray();
            _position += count;
            return result;
        }

        /// <summary>读取原始字节到指定缓冲区</summary>
        public void ReadRaw(Span<byte> destination)
        {
            if (destination.IsEmpty)
                return;

            EnsureAvailable(destination.Length);
            _buffer.Slice(_position, destination.Length).CopyTo(destination);
            _position += destination.Length;
        }

        /// <summary>获取原始字节的只读引用 (零拷贝)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> GetSpan(int count)
        {
            EnsureAvailable(count);
            var span = _buffer.Slice(_position, count);
            _position += count;
            return span;
        }

        #endregion

        #region 特殊类型读取

        /// <summary>读取 GUID</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid ReadGuid()
        {
            EnsureAvailable(16);
            var guid = new Guid(_buffer.Slice(_position, 16));
            _position += 16;
            return guid;
        }

        /// <summary>读取日期时间</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ReadDateTime()
        {
            return DateTime.FromBinary(ReadInt64());
        }

        /// <summary>读取时间跨度</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan ReadTimeSpan()
        {
            return TimeSpan.FromTicks(ReadInt64());
        }

        /// <summary>读取日期时间偏移</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTimeOffset ReadDateTimeOffset()
        {
            long ticks = ReadInt64();
            short offsetMinutes = ReadInt16();
            return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
        }

        #endregion

        #region 非托管类型读取

        /// <summary>读取非托管类型值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadUnmanaged<T>() where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            EnsureAvailable(size);
            var value = Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(_buffer[_position]));
            _position += size;
            return value;
        }

        /// <summary>读取非托管类型数组</summary>
        public T[] ReadUnmanagedArray<T>() where T : unmanaged
        {
            int count = ReadVarInt32();

            if (count <= 0)
                return count < 0 ? null : Array.Empty<T>();

            int byteSize = count * Unsafe.SizeOf<T>();
            EnsureAvailable(byteSize);

            var result = new T[count];
            var bytes = MemoryMarshal.AsBytes(result.AsSpan());
            _buffer.Slice(_position, byteSize).CopyTo(bytes);
            _position += byteSize;
            return result;
        }

        /// <summary>读取非托管类型数组到指定缓冲区</summary>
        public void ReadUnmanagedArray<T>(Span<T> destination) where T : unmanaged
        {
            int count = ReadVarInt32();

            if (count <= 0)
                return;

            if (destination.Length < count)
                throw new ArgumentException("目标缓冲区太小", nameof(destination));

            int byteSize = count * Unsafe.SizeOf<T>();
            EnsureAvailable(byteSize);

            var bytes = MemoryMarshal.AsBytes(destination.Slice(0, count));
            _buffer.Slice(_position, byteSize).CopyTo(bytes);
            _position += byteSize;
        }

        #endregion

        #region 集合头读取

        /// <summary>读取数组头 (元素数量)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadArrayHeader()
        {
            return ReadVarInt32();
        }

        /// <summary>读取字典头 (键值对数量)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadMapHeader()
        {
            return ReadVarInt32();
        }

        #endregion

        #region 类型码读取

        /// <summary>读取类型码</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadTypeCode()
        {
            return ReadByte();
        }

        /// <summary>检查下一个值是否为 null</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNull()
        {
            if (_position < _buffer.Length && _buffer[_position] == BinaryFormat.Null)
            {
                _position++;
                return true;
            }
            return false;
        }

        /// <summary>预览下一个类型码 (不移动位置)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekTypeCode()
        {
            EnsureAvailable(1);
            return _buffer[_position];
        }

        #endregion

        #region 位置控制

        /// <summary>跳过指定字节数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Skip(int count)
        {
            if (count <= 0)
                return;

            EnsureAvailable(count);
            _position += count;
        }

        /// <summary>推进读取位置</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            Skip(count);
        }

        /// <summary>设置读取位置</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Seek(int position)
        {
            if (position < 0 || position > _buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            _position = position;
        }

        /// <summary>重置到起始位置</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _position = 0;
        }

        /// <summary>预览下一个字节 (不移动位置)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte PeekByte()
        {
            EnsureAvailable(1);
            return _buffer[_position];
        }

        /// <summary>预览指定数量的字节 (不移动位置)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> Peek(int count)
        {
            EnsureAvailable(count);
            return _buffer.Slice(_position, count);
        }

        #endregion

        #region 边界检查

        /// <summary>
        /// 确保有足够的数据可读
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAvailable(int count)
        {
            if (_position + count > _buffer.Length)
            {
                throw new InvalidOperationException(
                    $"读取越界: 位置 {_position}, 需要 {count} 字节, 剩余 {Remaining} 字节");
            }
        }

        /// <summary>
        /// 检查是否有足够的数据可读
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAvailable(int count)
        {
            return _position + count <= _buffer.Length;
        }

        #endregion
    }
}
