// ==========================================================
// 文件名：DataHeader.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化数据头结构体
    /// <para>封装序列化数据的头部信息，用于数据识别和解析</para>
    /// <para>包含魔数、版本、特性标志、数据大小等关键信息</para>
    /// </summary>
    /// <remarks>
    /// 数据头布局 (32 字节):
    /// <code>
    /// [0-3]   Magic Number (4 bytes)     - 魔数标识
    /// [4-5]   Version (2 bytes)          - 版本号
    /// [6-7]   Flags (2 bytes)            - 特性标志
    /// [8-11]  Data Size (4 bytes)        - 数据大小
    /// [12-15] Uncompressed Size (4 bytes)- 未压缩大小
    /// [16-19] Checksum (4 bytes)         - 校验和
    /// [20-23] Type Hash (4 bytes)        - 类型哈希
    /// [24-27] Reserved (4 bytes)         - 保留字段
    /// [28-31] Header Checksum (4 bytes)  - 头部校验和
    /// </code>
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = HeaderSize)]
    public readonly struct DataHeader : IEquatable<DataHeader>
    {
        #region 常量

        /// <summary>数据头大小 (字节)</summary>
        public const int HeaderSize = 32;

        /// <summary>默认魔数</summary>
        public const uint DefaultMagic = 0x41465253; // "AFRS" (AFramework Serialization)

        /// <summary>压缩数据魔数</summary>
        public const uint CompressedMagic = 0x41465243; // "AFRC" (AFramework Compressed)

        /// <summary>加密数据魔数</summary>
        public const uint EncryptedMagic = 0x41465245; // "AFRE" (AFramework Encrypted)

        #endregion

        #region 字段

        private readonly uint _magic;
        private readonly ushort _version;
        private readonly ushort _flags;
        private readonly uint _dataSize;
        private readonly uint _uncompressedSize;
        private readonly uint _checksum;
        private readonly uint _typeHash;
        private readonly uint _reserved;
        private readonly uint _headerChecksum;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建数据头
        /// </summary>
        private DataHeader(
            uint magic,
            ushort version,
            ushort flags,
            uint dataSize,
            uint uncompressedSize,
            uint checksum,
            uint typeHash,
            uint reserved,
            uint headerChecksum)
        {
            _magic = magic;
            _version = version;
            _flags = flags;
            _dataSize = dataSize;
            _uncompressedSize = uncompressedSize;
            _checksum = checksum;
            _typeHash = typeHash;
            _reserved = reserved;
            _headerChecksum = headerChecksum;
        }

        #endregion

        #region 属性

        /// <summary>魔数</summary>
        public uint Magic => _magic;

        /// <summary>版本号</summary>
        public ushort Version => _version;

        /// <summary>特性标志</summary>
        public ushort Flags => _flags;

        /// <summary>数据大小 (字节)</summary>
        public uint DataSize => _dataSize;

        /// <summary>未压缩大小 (字节)</summary>
        public uint UncompressedSize => _uncompressedSize;

        /// <summary>数据校验和</summary>
        public uint Checksum => _checksum;

        /// <summary>类型哈希</summary>
        public uint TypeHash => _typeHash;

        /// <summary>保留字段</summary>
        public uint Reserved => _reserved;

        /// <summary>头部校验和</summary>
        public uint HeaderChecksum => _headerChecksum;

        /// <summary>主版本号</summary>
        public int MajorVersion => _version >> 8;

        /// <summary>次版本号</summary>
        public int MinorVersion => _version & 0xFF;

        /// <summary>压缩率</summary>
        public float CompressionRatio => _uncompressedSize > 0 ? (float)_dataSize / _uncompressedSize : 1.0f;

        /// <summary>总大小 (头部 + 数据)</summary>
        public long TotalSize => HeaderSize + _dataSize;

        #endregion

        #region 标志属性

        /// <summary>是否压缩</summary>
        public bool IsCompressed => (_flags & (ushort)HeaderFlags.Compressed) != 0;

        /// <summary>是否加密</summary>
        public bool IsEncrypted => (_flags & (ushort)HeaderFlags.Encrypted) != 0;

        /// <summary>是否有校验和</summary>
        public bool HasChecksum => (_flags & (ushort)HeaderFlags.HasChecksum) != 0;

        /// <summary>是否包含类型信息</summary>
        public bool HasTypeInfo => (_flags & (ushort)HeaderFlags.HasTypeInfo) != 0;

        /// <summary>是否为版本容错模式</summary>
        public bool IsVersionTolerant => (_flags & (ushort)HeaderFlags.VersionTolerant) != 0;

        /// <summary>是否为大端字节序</summary>
        public bool IsBigEndian => (_flags & (ushort)HeaderFlags.BigEndian) != 0;

        /// <summary>是否有效</summary>
        public bool IsValid => _magic == DefaultMagic || _magic == CompressedMagic || _magic == EncryptedMagic;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建数据头
        /// </summary>
        /// <param name="dataSize">数据大小</param>
        /// <param name="flags">特性标志</param>
        /// <param name="typeHash">类型哈希</param>
        public static DataHeader Create(uint dataSize, HeaderFlags flags = HeaderFlags.None, uint typeHash = 0)
        {
            var magic = flags.HasFlag(HeaderFlags.Compressed) ? CompressedMagic :
                        flags.HasFlag(HeaderFlags.Encrypted) ? EncryptedMagic : DefaultMagic;

            var header = new DataHeader(
                magic: magic,
                version: SerializeConstants.Version.Current,
                flags: (ushort)flags,
                dataSize: dataSize,
                uncompressedSize: dataSize,
                checksum: 0,
                typeHash: typeHash,
                reserved: 0,
                headerChecksum: 0
            );

            return header.WithHeaderChecksum(header.CalculateHeaderChecksum());
        }

        /// <summary>
        /// 创建压缩数据头
        /// </summary>
        /// <param name="compressedSize">压缩后大小</param>
        /// <param name="uncompressedSize">未压缩大小</param>
        /// <param name="flags">特性标志</param>
        /// <param name="typeHash">类型哈希</param>
        public static DataHeader CreateCompressed(uint compressedSize, uint uncompressedSize, HeaderFlags flags = HeaderFlags.None, uint typeHash = 0)
        {
            flags |= HeaderFlags.Compressed;

            var header = new DataHeader(
                magic: CompressedMagic,
                version: SerializeConstants.Version.Current,
                flags: (ushort)flags,
                dataSize: compressedSize,
                uncompressedSize: uncompressedSize,
                checksum: 0,
                typeHash: typeHash,
                reserved: 0,
                headerChecksum: 0
            );

            return header.WithHeaderChecksum(header.CalculateHeaderChecksum());
        }

        /// <summary>空数据头</summary>
        public static DataHeader Empty => default;

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证魔数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateMagic()
        {
            return _magic == DefaultMagic || _magic == CompressedMagic || _magic == EncryptedMagic;
        }

        /// <summary>
        /// 验证版本兼容性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateVersion()
        {
            return SerializeConstants.IsCompatibleVersion(_version);
        }

        /// <summary>
        /// 验证头部校验和
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateHeaderChecksum()
        {
            return _headerChecksum == CalculateHeaderChecksum();
        }

        /// <summary>
        /// 验证数据校验和
        /// </summary>
        /// <param name="data">数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateDataChecksum(ReadOnlySpan<byte> data)
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

            if (!ValidateHeaderChecksum())
            {
                errorCode = SerializeErrorCode.ChecksumFailed;
                return false;
            }

            errorCode = SerializeErrorCode.Success;
            return true;
        }

        #endregion

        #region With 方法

        /// <summary>修改数据大小</summary>
        public DataHeader WithDataSize(uint dataSize) =>
            new DataHeader(_magic, _version, _flags, dataSize, _uncompressedSize, _checksum, _typeHash, _reserved, _headerChecksum);

        /// <summary>修改未压缩大小</summary>
        public DataHeader WithUncompressedSize(uint uncompressedSize) =>
            new DataHeader(_magic, _version, _flags, _dataSize, uncompressedSize, _checksum, _typeHash, _reserved, _headerChecksum);

        /// <summary>修改校验和</summary>
        public DataHeader WithChecksum(uint checksum) =>
            new DataHeader(_magic, _version, (ushort)(_flags | (ushort)HeaderFlags.HasChecksum), _dataSize, _uncompressedSize, checksum, _typeHash, _reserved, _headerChecksum);

        /// <summary>修改类型哈希</summary>
        public DataHeader WithTypeHash(uint typeHash) =>
            new DataHeader(_magic, _version, (ushort)(_flags | (ushort)HeaderFlags.HasTypeInfo), _dataSize, _uncompressedSize, _checksum, typeHash, _reserved, _headerChecksum);

        /// <summary>修改头部校验和</summary>
        public DataHeader WithHeaderChecksum(uint headerChecksum) =>
            new DataHeader(_magic, _version, _flags, _dataSize, _uncompressedSize, _checksum, _typeHash, _reserved, headerChecksum);

        /// <summary>添加标志</summary>
        public DataHeader AddFlags(HeaderFlags flags) =>
            new DataHeader(_magic, _version, (ushort)(_flags | (ushort)flags), _dataSize, _uncompressedSize, _checksum, _typeHash, _reserved, _headerChecksum);

        /// <summary>从数据计算并设置校验和</summary>
        public DataHeader WithDataChecksum(ReadOnlySpan<byte> data)
        {
            var checksum = CalculateCRC32(data);
            return WithChecksum(checksum);
        }

        /// <summary>重新计算并更新头部校验和</summary>
        public DataHeader UpdateHeaderChecksum()
        {
            return WithHeaderChecksum(CalculateHeaderChecksum());
        }

        #endregion

        #region 序列化方法

        /// <summary>
        /// 写入到字节数组
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        public void WriteTo(Span<byte> buffer)
        {
            if (buffer.Length < HeaderSize)
                throw new ArgumentException($"缓冲区大小不足，需要 {HeaderSize} 字节");

            BitConverter.TryWriteBytes(buffer.Slice(0, 4), _magic);
            BitConverter.TryWriteBytes(buffer.Slice(4, 2), _version);
            BitConverter.TryWriteBytes(buffer.Slice(6, 2), _flags);
            BitConverter.TryWriteBytes(buffer.Slice(8, 4), _dataSize);
            BitConverter.TryWriteBytes(buffer.Slice(12, 4), _uncompressedSize);
            BitConverter.TryWriteBytes(buffer.Slice(16, 4), _checksum);
            BitConverter.TryWriteBytes(buffer.Slice(20, 4), _typeHash);
            BitConverter.TryWriteBytes(buffer.Slice(24, 4), _reserved);
            BitConverter.TryWriteBytes(buffer.Slice(28, 4), _headerChecksum);
        }

        /// <summary>
        /// 从字节数组读取
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        public static DataHeader ReadFrom(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < HeaderSize)
                throw new ArgumentException($"缓冲区大小不足，需要 {HeaderSize} 字节");

            return new DataHeader(
                magic: BitConverter.ToUInt32(buffer.Slice(0, 4)),
                version: BitConverter.ToUInt16(buffer.Slice(4, 2)),
                flags: BitConverter.ToUInt16(buffer.Slice(6, 2)),
                dataSize: BitConverter.ToUInt32(buffer.Slice(8, 4)),
                uncompressedSize: BitConverter.ToUInt32(buffer.Slice(12, 4)),
                checksum: BitConverter.ToUInt32(buffer.Slice(16, 4)),
                typeHash: BitConverter.ToUInt32(buffer.Slice(20, 4)),
                reserved: BitConverter.ToUInt32(buffer.Slice(24, 4)),
                headerChecksum: BitConverter.ToUInt32(buffer.Slice(28, 4))
            );
        }

        /// <summary>
        /// 尝试从字节数组读取
        /// </summary>
        /// <param name="buffer">源缓冲区</param>
        /// <param name="header">输出数据头</param>
        public static bool TryReadFrom(ReadOnlySpan<byte> buffer, out DataHeader header)
        {
            if (buffer.Length < HeaderSize)
            {
                header = default;
                return false;
            }

            header = ReadFrom(buffer);
            return header.ValidateMagic();
        }

        #endregion

        #region 校验和计算

        /// <summary>
        /// 计算头部校验和 (不包含 headerChecksum 字段)
        /// </summary>
        private uint CalculateHeaderChecksum()
        {
            uint hash = 2166136261; // FNV-1a offset basis
            hash = (hash ^ _magic) * 16777619;
            hash = (hash ^ _version) * 16777619;
            hash = (hash ^ _flags) * 16777619;
            hash = (hash ^ _dataSize) * 16777619;
            hash = (hash ^ _uncompressedSize) * 16777619;
            hash = (hash ^ _checksum) * 16777619;
            hash = (hash ^ _typeHash) * 16777619;
            hash = (hash ^ _reserved) * 16777619;
            return hash;
        }

        /// <summary>
        /// 计算 CRC32 校验和
        /// </summary>
        private static uint CalculateCRC32(ReadOnlySpan<byte> data)
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
        public bool Equals(DataHeader other)
        {
            return _magic == other._magic &&
                   _version == other._version &&
                   _flags == other._flags &&
                   _dataSize == other._dataSize &&
                   _checksum == other._checksum;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is DataHeader other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_magic, _version, _flags, _dataSize, _checksum);

        /// <summary>相等运算符</summary>
        public static bool operator ==(DataHeader left, DataHeader right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(DataHeader left, DataHeader right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"DataHeader(Magic=0x{_magic:X8}, Version={MajorVersion}.{MinorVersion}, Size={_dataSize}, Flags={_flags:X4})";
        }

        #endregion
    }

    #region 头部标志枚举

    /// <summary>
    /// 数据头标志枚举
    /// </summary>
    [Flags]
    public enum HeaderFlags : ushort
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

        /// <summary>版本容错模式</summary>
        VersionTolerant = 1 << 4,

        /// <summary>循环引用模式</summary>
        CircularReference = 1 << 5,

        /// <summary>多态模式</summary>
        Polymorphic = 1 << 6,

        /// <summary>流式模式</summary>
        Streaming = 1 << 7,

        /// <summary>大端字节序</summary>
        BigEndian = 1 << 8,

        /// <summary>包含调试信息</summary>
        DebugInfo = 1 << 9,

        /// <summary>字符串内化</summary>
        StringIntern = 1 << 10,

        /// <summary>位打包</summary>
        BitPacked = 1 << 11
    }

    #endregion
}
