// ==========================================================
// 文件名：BinaryHeader.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 二进制数据头结构体
    /// <para>封装二进制序列化数据的头部信息</para>
    /// <para>提供紧凑的头部格式，支持快速验证和解析</para>
    /// </summary>
    /// <remarks>
    /// 头部布局 (16 字节):
    /// <code>
    /// [0-3]   Magic Number (4 bytes)     - 魔数标识 "AFSB"
    /// [4]     Major Version (1 byte)     - 主版本号
    /// [5]     Minor Version (1 byte)     - 次版本号
    /// [6-7]   Flags (2 bytes)            - 特性标志
    /// [8-11]  Data Size (4 bytes)        - 数据大小
    /// [12-15] Checksum (4 bytes)         - 校验和
    /// </code>
    /// 
    /// 使用示例:
    /// <code>
    /// // 创建头部
    /// var header = BinaryHeader.Create(dataSize, BinaryHeaderFlags.HasChecksum);
    /// 
    /// // 写入头部
    /// header.WriteTo(buffer);
    /// 
    /// // 读取头部
    /// var header = BinaryHeader.ReadFrom(buffer);
    /// if (header.Validate(out var error))
    ///     ProcessData(buffer.Slice(BinaryHeader.Size));
    /// </code>
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = Size)]
    public readonly struct BinaryHeader : IEquatable<BinaryHeader>
    {
        #region 常量

        /// <summary>头部大小 (字节)</summary>
        public const int Size = 16;

        /// <summary>魔数 "AFSB" (AFramework Serialization Binary)</summary>
        public const uint MagicNumber = 0x42534641; // "AFSB" in little-endian

        /// <summary>压缩数据魔数 "AFSC"</summary>
        public const uint CompressedMagic = 0x43534641; // "AFSC"

        /// <summary>加密数据魔数 "AFSE"</summary>
        public const uint EncryptedMagic = 0x45534641; // "AFSE"

        /// <summary>最小数据头大小</summary>
        public const int MinSize = 8;

        #endregion

        #region 字段

        private readonly uint _magic;
        private readonly byte _majorVersion;
        private readonly byte _minorVersion;
        private readonly ushort _flags;
        private readonly uint _dataSize;
        private readonly uint _checksum;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建二进制数据头
        /// </summary>
        private BinaryHeader(
            uint magic,
            byte majorVersion,
            byte minorVersion,
            ushort flags,
            uint dataSize,
            uint checksum)
        {
            _magic = magic;
            _majorVersion = majorVersion;
            _minorVersion = minorVersion;
            _flags = flags;
            _dataSize = dataSize;
            _checksum = checksum;
        }

        #endregion

        #region 属性

        /// <summary>魔数</summary>
        public uint Magic => _magic;

        /// <summary>主版本号</summary>
        public byte MajorVersion => _majorVersion;

        /// <summary>次版本号</summary>
        public byte MinorVersion => _minorVersion;

        /// <summary>版本号 (组合)</summary>
        public ushort Version => (ushort)((_majorVersion << 8) | _minorVersion);

        /// <summary>特性标志</summary>
        public ushort Flags => _flags;

        /// <summary>数据大小 (字节)</summary>
        public uint DataSize => _dataSize;

        /// <summary>校验和</summary>
        public uint Checksum => _checksum;

        /// <summary>总大小 (头部 + 数据)</summary>
        public long TotalSize => Size + _dataSize;

        #endregion

        #region 标志属性

        /// <summary>是否压缩</summary>
        public bool IsCompressed => (_flags & (ushort)BinaryHeaderFlags.Compressed) != 0;

        /// <summary>是否加密</summary>
        public bool IsEncrypted => (_flags & (ushort)BinaryHeaderFlags.Encrypted) != 0;

        /// <summary>是否有校验和</summary>
        public bool HasChecksum => (_flags & (ushort)BinaryHeaderFlags.HasChecksum) != 0;

        /// <summary>是否包含类型信息</summary>
        public bool HasTypeInfo => (_flags & (ushort)BinaryHeaderFlags.HasTypeInfo) != 0;

        /// <summary>是否为大端字节序</summary>
        public bool IsBigEndian => (_flags & (ushort)BinaryHeaderFlags.BigEndian) != 0;

        /// <summary>是否启用字符串内化</summary>
        public bool HasStringIntern => (_flags & (ushort)BinaryHeaderFlags.StringIntern) != 0;

        /// <summary>是否启用循环引用</summary>
        public bool HasCircularReference => (_flags & (ushort)BinaryHeaderFlags.CircularReference) != 0;

        /// <summary>是否有效</summary>
        public bool IsValid => _magic == MagicNumber || _magic == CompressedMagic || _magic == EncryptedMagic;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建标准数据头
        /// </summary>
        /// <param name="dataSize">数据大小</param>
        /// <param name="flags">特性标志</param>
        public static BinaryHeader Create(uint dataSize, BinaryHeaderFlags flags = BinaryHeaderFlags.None)
        {
            var magic = flags.HasFlag(BinaryHeaderFlags.Compressed) ? CompressedMagic :
                        flags.HasFlag(BinaryHeaderFlags.Encrypted) ? EncryptedMagic : MagicNumber;

            return new BinaryHeader(
                magic: magic,
                majorVersion: BinaryFormat.MajorVersion,
                minorVersion: BinaryFormat.MinorVersion,
                flags: (ushort)flags,
                dataSize: dataSize,
                checksum: 0
            );
        }

        /// <summary>
        /// 创建带校验和的数据头
        /// </summary>
        /// <param name="dataSize">数据大小</param>
        /// <param name="checksum">校验和</param>
        /// <param name="flags">特性标志</param>
        public static BinaryHeader CreateWithChecksum(uint dataSize, uint checksum, BinaryHeaderFlags flags = BinaryHeaderFlags.None)
        {
            flags |= BinaryHeaderFlags.HasChecksum;
            var magic = flags.HasFlag(BinaryHeaderFlags.Compressed) ? CompressedMagic :
                        flags.HasFlag(BinaryHeaderFlags.Encrypted) ? EncryptedMagic : MagicNumber;

            return new BinaryHeader(
                magic: magic,
                majorVersion: BinaryFormat.MajorVersion,
                minorVersion: BinaryFormat.MinorVersion,
                flags: (ushort)flags,
                dataSize: dataSize,
                checksum: checksum
            );
        }

        /// <summary>空数据头</summary>
        public static BinaryHeader Empty => default;

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证魔数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateMagic()
        {
            return _magic == MagicNumber || _magic == CompressedMagic || _magic == EncryptedMagic;
        }

        /// <summary>
        /// 验证版本兼容性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateVersion()
        {
            // 主版本必须匹配，次版本可以向后兼容
            return _majorVersion == BinaryFormat.MajorVersion && 
                   _minorVersion <= BinaryFormat.MinorVersion;
        }

        /// <summary>
        /// 验证数据校验和
        /// </summary>
        /// <param name="data">数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateChecksum(ReadOnlySpan<byte> data)
        {
            if (!HasChecksum)
                return true;
            return _checksum == CalculateCRC32(data);
        }

        /// <summary>
        /// 完整验证
        /// </summary>
        /// <param name="errorCode">输出错误码</param>
        public bool Validate(out SerializeErrorCode errorCode)
        {
            if (!ValidateMagic())
            {
                errorCode = SerializeErrorCode.InvalidMagicNumber;
                return false;
            }

            if (!ValidateVersion())
            {
                errorCode = SerializeErrorCode.VersionMismatch;
                return false;
            }

            errorCode = SerializeErrorCode.Success;
            return true;
        }

        /// <summary>
        /// 完整验证 (包含数据校验)
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="errorCode">输出错误码</param>
        public bool Validate(ReadOnlySpan<byte> data, out SerializeErrorCode errorCode)
        {
            if (!Validate(out errorCode))
                return false;

            if (!ValidateChecksum(data))
            {
                errorCode = SerializeErrorCode.ChecksumFailed;
                return false;
            }

            return true;
        }

        #endregion

        #region With 方法

        /// <summary>修改数据大小</summary>
        public BinaryHeader WithDataSize(uint dataSize) =>
            new BinaryHeader(_magic, _majorVersion, _minorVersion, _flags, dataSize, _checksum);

        /// <summary>修改校验和</summary>
        public BinaryHeader WithChecksum(uint checksum) =>
            new BinaryHeader(_magic, _majorVersion, _minorVersion, (ushort)(_flags | (ushort)BinaryHeaderFlags.HasChecksum), _dataSize, checksum);

        /// <summary>添加标志</summary>
        public BinaryHeader AddFlags(BinaryHeaderFlags flags) =>
            new BinaryHeader(_magic, _majorVersion, _minorVersion, (ushort)(_flags | (ushort)flags), _dataSize, _checksum);

        /// <summary>从数据计算并设置校验和</summary>
        public BinaryHeader WithDataChecksum(ReadOnlySpan<byte> data)
        {
            var checksum = CalculateCRC32(data);
            return WithChecksum(checksum);
        }

        #endregion

        #region 序列化方法

        /// <summary>
        /// 写入到字节数组
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        public void WriteTo(Span<byte> buffer)
        {
            if (buffer.Length < Size)
                throw new ArgumentException($"缓冲区大小不足，需要 {Size} 字节", nameof(buffer));

            // 使用小端序写入
            BitConverter.TryWriteBytes(buffer.Slice(0, 4), _magic);
            buffer[4] = _majorVersion;
            buffer[5] = _minorVersion;
            BitConverter.TryWriteBytes(buffer.Slice(6, 2), _flags);
            BitConverter.TryWriteBytes(buffer.Slice(8, 4), _dataSize);
            BitConverter.TryWriteBytes(buffer.Slice(12, 4), _checksum);
        }

        /// <summary>
        /// 从字节数组读取
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        public static BinaryHeader ReadFrom(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < Size)
                throw new ArgumentException($"缓冲区大小不足，需要 {Size} 字节", nameof(buffer));

            return new BinaryHeader(
                magic: BitConverter.ToUInt32(buffer.Slice(0, 4)),
                majorVersion: buffer[4],
                minorVersion: buffer[5],
                flags: BitConverter.ToUInt16(buffer.Slice(6, 2)),
                dataSize: BitConverter.ToUInt32(buffer.Slice(8, 4)),
                checksum: BitConverter.ToUInt32(buffer.Slice(12, 4))
            );
        }

        /// <summary>
        /// 尝试从字节数组读取
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="header">输出数据头</param>
        public static bool TryReadFrom(ReadOnlySpan<byte> buffer, out BinaryHeader header)
        {
            if (buffer.Length < Size)
            {
                header = default;
                return false;
            }

            header = ReadFrom(buffer);
            return header.ValidateMagic();
        }

        /// <summary>
        /// 快速检查缓冲区是否包含有效头部
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValidHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4)
                return false;

            var magic = BitConverter.ToUInt32(buffer.Slice(0, 4));
            return magic == MagicNumber || magic == CompressedMagic || magic == EncryptedMagic;
        }

        #endregion

        #region 校验和计算

        /// <summary>
        /// 计算 CRC32 校验和
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CalculateCRC32(ReadOnlySpan<byte> data)
        {
            uint crc = 0xFFFFFFFF;
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc >> 1) ^ (0xEDB88320 & ~((crc & 1) - 1));
                }
            }
            return ~crc;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(BinaryHeader other)
        {
            return _magic == other._magic &&
                   _majorVersion == other._majorVersion &&
                   _minorVersion == other._minorVersion &&
                   _flags == other._flags &&
                   _dataSize == other._dataSize &&
                   _checksum == other._checksum;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is BinaryHeader other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_magic, _majorVersion, _minorVersion, _flags, _dataSize, _checksum);

        /// <summary>相等运算符</summary>
        public static bool operator ==(BinaryHeader left, BinaryHeader right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(BinaryHeader left, BinaryHeader right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"BinaryHeader(Magic=0x{_magic:X8}, Version={_majorVersion}.{_minorVersion}, Size={_dataSize}, Flags=0x{_flags:X4})";
        }

        #endregion
    }

    #region 头部标志枚举

    /// <summary>
    /// 二进制数据头标志枚举
    /// </summary>
    [Flags]
    public enum BinaryHeaderFlags : ushort
    {
        /// <summary>无标志</summary>
        None = 0,

        /// <summary>已压缩</summary>
        Compressed = 1 << 0,

        /// <summary>已加密</summary>
        Encrypted = 1 << 1,

        /// <summary>有校验和</summary>
        HasChecksum = 1 << 2,

        /// <summary>有类型信息</summary>
        HasTypeInfo = 1 << 3,

        /// <summary>大端字节序</summary>
        BigEndian = 1 << 4,

        /// <summary>字符串内化</summary>
        StringIntern = 1 << 5,

        /// <summary>循环引用</summary>
        CircularReference = 1 << 6,

        /// <summary>版本容错</summary>
        VersionTolerant = 1 << 7,

        /// <summary>包含调试信息</summary>
        DebugInfo = 1 << 8,

        /// <summary>位打包</summary>
        BitPacked = 1 << 9,

        /// <summary>使用 VarInt 编码</summary>
        UseVarInt = 1 << 10,

        /// <summary>包含元数据</summary>
        HasMetadata = 1 << 11
    }

    #endregion
}
