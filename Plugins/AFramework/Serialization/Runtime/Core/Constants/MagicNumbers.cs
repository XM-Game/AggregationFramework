// ==========================================================
// 文件名：MagicNumbers.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 魔数常量定义
    /// <para>提供序列化数据格式识别的魔数常量</para>
    /// <para>包含文件头标识、格式签名、校验值等魔数</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 验证文件头魔数
    /// if (header != MagicNumbers.BinaryFormat)
    ///     throw new InvalidDataException("无效的序列化数据格式");
    /// 
    /// // 写入魔数
    /// writer.WriteUInt32(MagicNumbers.BinaryFormat);
    /// 
    /// // 检查压缩格式
    /// if (MagicNumbers.IsLZ4Magic(header))
    ///     return DecompressLZ4(data);
    /// </code>
    /// </remarks>
    public static class MagicNumbers
    {
        #region 主格式魔数

        /// <summary>
        /// AFramework 二进制序列化格式魔数
        /// <para>ASCII: "AFRS" (AFramework Serialization)</para>
        /// </summary>
        public const uint BinaryFormat = 0x53524641; // "AFRS" (Little Endian)

        /// <summary>
        /// AFramework 二进制序列化格式魔数 (大端)
        /// </summary>
        public const uint BinaryFormatBE = 0x41465253; // "AFRS" (Big Endian)

        /// <summary>
        /// AFramework YAML 格式标识
        /// <para>ASCII: "AFYA" (AFramework YAML)</para>
        /// </summary>
        public const uint YamlFormat = 0x41594641; // "AFYA"

        /// <summary>
        /// AFramework JSON 格式标识
        /// <para>ASCII: "AFJS" (AFramework JSON)</para>
        /// </summary>
        public const uint JsonFormat = 0x534A4641; // "AFJS"

        /// <summary>
        /// AFramework 扩展格式标识
        /// <para>ASCII: "AFEX" (AFramework Extension)</para>
        /// </summary>
        public const uint ExtensionFormat = 0x58454641; // "AFEX"

        /// <summary>
        /// AFramework 流式格式标识
        /// <para>ASCII: "AFST" (AFramework Stream)</para>
        /// </summary>
        public const uint StreamFormat = 0x54534641; // "AFST"

        #endregion

        #region 压缩格式魔数

        /// <summary>
        /// 压缩格式魔数集合
        /// <para>定义各种压缩算法的文件头魔数</para>
        /// </summary>
        public static class Compression
        {
            /// <summary>LZ4 帧格式魔数</summary>
            public const uint LZ4Frame = 0x184D2204;

            /// <summary>LZ4 块格式魔数</summary>
            public const uint LZ4Block = 0x184C2102;

            /// <summary>Zstd 魔数</summary>
            public const uint Zstd = 0xFD2FB528;

            /// <summary>Gzip 魔数 (前两字节)</summary>
            public const ushort Gzip = 0x8B1F;

            /// <summary>Deflate 魔数 (CMF + FLG)</summary>
            public const ushort Deflate = 0x9C78;

            /// <summary>Brotli 魔数 (无标准魔数，使用自定义)</summary>
            public const uint Brotli = 0x42524F54; // "BROT"

            /// <summary>Snappy 魔数</summary>
            public const uint Snappy = 0x73736E70; // "snps"

            /// <summary>XZ 魔数</summary>
            public const uint XZ = 0x587A37FD;

            /// <summary>LZMA 魔数</summary>
            public const uint LZMA = 0x414D5A4C; // "LZMA"
        }

        #endregion

        #region 加密格式魔数

        /// <summary>
        /// 加密格式魔数集合
        /// <para>定义加密数据的标识魔数</para>
        /// </summary>
        public static class Encryption
        {
            /// <summary>AES 加密数据标识</summary>
            public const uint AES = 0x53454141; // "AAES"

            /// <summary>XOR 加密数据标识</summary>
            public const uint XOR = 0x524F5841; // "AXOR"

            /// <summary>ChaCha20 加密数据标识</summary>
            public const uint ChaCha20 = 0x41484343; // "CCHA"

            /// <summary>未加密数据标识</summary>
            public const uint None = 0x454E4F4E; // "NONE"
        }

        #endregion

        #region 校验算法魔数

        /// <summary>
        /// 校验算法魔数集合
        /// <para>定义校验算法的标识魔数</para>
        /// </summary>
        public static class Checksum
        {
            /// <summary>CRC32 校验标识</summary>
            public const uint CRC32 = 0x32335243; // "CRC32" 简化

            /// <summary>CRC64 校验标识</summary>
            public const uint CRC64 = 0x34365243; // "CRC64" 简化

            /// <summary>XXHash32 校验标识</summary>
            public const uint XXHash32 = 0x32485858; // "XXH2"

            /// <summary>XXHash64 校验标识</summary>
            public const uint XXHash64 = 0x34485858; // "XXH4"

            /// <summary>XXHash128 校验标识</summary>
            public const uint XXHash128 = 0x38485858; // "XXH8"

            /// <summary>MD5 校验标识</summary>
            public const uint MD5 = 0x3544444D; // "MD5"

            /// <summary>SHA256 校验标识</summary>
            public const uint SHA256 = 0x36353253; // "S256"

            /// <summary>无校验标识</summary>
            public const uint None = 0x454E4F4E; // "NONE"
        }

        #endregion

        #region 数据块魔数

        /// <summary>
        /// 数据块魔数集合
        /// <para>定义数据块类型的标识魔数</para>
        /// </summary>
        public static class Block
        {
            /// <summary>数据块开始</summary>
            public const uint Start = 0x4B4C4253; // "SBLK"

            /// <summary>数据块结束</summary>
            public const uint End = 0x4B4C4245; // "EBLK"

            /// <summary>元数据块</summary>
            public const uint Metadata = 0x4154454D; // "META"

            /// <summary>索引块</summary>
            public const uint Index = 0x58444E49; // "INDX"

            /// <summary>数据块</summary>
            public const uint Data = 0x41544144; // "DATA"

            /// <summary>类型信息块</summary>
            public const uint TypeInfo = 0x45505954; // "TYPE"

            /// <summary>字符串表块</summary>
            public const uint StringTable = 0x52545453; // "STRT"

            /// <summary>引用表块</summary>
            public const uint ReferenceTable = 0x53464552; // "REFS"

            /// <summary>扩展块</summary>
            public const uint Extension = 0x54584558; // "EXTN"

            /// <summary>填充块</summary>
            public const uint Padding = 0x44415050; // "PADD"
        }

        #endregion

        #region 特殊标记魔数

        /// <summary>
        /// 特殊标记魔数集合
        /// <para>定义特殊数据标记的魔数</para>
        /// </summary>
        public static class Marker
        {
            /// <summary>空值标记</summary>
            public const byte Null = 0xC0;

            /// <summary>对象引用标记</summary>
            public const byte ObjectRef = 0xC1;

            /// <summary>类型引用标记</summary>
            public const byte TypeRef = 0xC2;

            /// <summary>字符串引用标记</summary>
            public const byte StringRef = 0xC3;

            /// <summary>数组开始标记</summary>
            public const byte ArrayStart = 0xD0;

            /// <summary>对象开始标记</summary>
            public const byte ObjectStart = 0xD1;

            /// <summary>映射开始标记</summary>
            public const byte MapStart = 0xD2;

            /// <summary>结构结束标记</summary>
            public const byte StructEnd = 0xDF;

            /// <summary>流结束标记</summary>
            public const byte EndOfStream = 0xFE;

            /// <summary>无效/保留标记</summary>
            public const byte Invalid = 0xFF;
        }

        #endregion

        #region 版本标记魔数

        /// <summary>
        /// 版本标记魔数集合
        /// <para>定义版本相关的标识魔数</para>
        /// </summary>
        public static class Version
        {
            /// <summary>版本 1.0 标识</summary>
            public const ushort V1_0 = 0x0100;

            /// <summary>版本 1.1 标识</summary>
            public const ushort V1_1 = 0x0101;

            /// <summary>版本 2.0 标识</summary>
            public const ushort V2_0 = 0x0200;

            /// <summary>当前版本</summary>
            public const ushort Current = V1_0;

            /// <summary>最小支持版本</summary>
            public const ushort MinSupported = V1_0;
        }

        #endregion

        #region 常用文件格式魔数

        /// <summary>
        /// 常用文件格式魔数集合
        /// <para>定义常见文件格式的魔数，用于格式检测</para>
        /// </summary>
        public static class FileFormat
        {
            /// <summary>PNG 图片格式</summary>
            public const ulong PNG = 0x0A1A0A0D474E5089;

            /// <summary>JPEG 图片格式</summary>
            public const ushort JPEG = 0xD8FF;

            /// <summary>GIF 图片格式</summary>
            public const uint GIF = 0x38464947; // "GIF8"

            /// <summary>BMP 图片格式</summary>
            public const ushort BMP = 0x4D42; // "BM"

            /// <summary>ZIP 压缩格式</summary>
            public const uint ZIP = 0x04034B50;

            /// <summary>PDF 文档格式</summary>
            public const uint PDF = 0x46445025; // "%PDF"

            /// <summary>Unity AssetBundle 格式</summary>
           // public const ulong UnityAssetBundle = 0x00000000FS696E55; // "UnityFS"

            /// <summary>MessagePack 格式 (fixmap)</summary>
            public const byte MessagePackFixMap = 0x80;

            /// <summary>MessagePack 格式 (fixarray)</summary>
            public const byte MessagePackFixArray = 0x90;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 检查是否为有效的 AFramework 序列化格式
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>如果是有效格式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidAFrameworkFormat(uint magic)
        {
            return magic == BinaryFormat || 
                   magic == BinaryFormatBE ||
                   magic == YamlFormat || 
                   magic == JsonFormat ||
                   magic == ExtensionFormat ||
                   magic == StreamFormat;
        }

        /// <summary>
        /// 检查是否为二进制格式
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>如果是二进制格式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinaryFormat(uint magic)
        {
            return magic == BinaryFormat || magic == BinaryFormatBE;
        }

        /// <summary>
        /// 检查是否为大端字节序
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>如果是大端字节序返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBigEndian(uint magic)
        {
            return magic == BinaryFormatBE;
        }

        /// <summary>
        /// 检查是否为 LZ4 压缩格式
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>如果是 LZ4 格式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLZ4Magic(uint magic)
        {
            return magic == Compression.LZ4Frame || magic == Compression.LZ4Block;
        }

        /// <summary>
        /// 检查是否为 Gzip 压缩格式
        /// </summary>
        /// <param name="firstTwoBytes">前两个字节</param>
        /// <returns>如果是 Gzip 格式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGzipMagic(ushort firstTwoBytes)
        {
            return firstTwoBytes == Compression.Gzip;
        }

        /// <summary>
        /// 检查是否为 Zstd 压缩格式
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>如果是 Zstd 格式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZstdMagic(uint magic)
        {
            return magic == Compression.Zstd;
        }

        /// <summary>
        /// 获取压缩算法名称
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>压缩算法名称</returns>
        public static string GetCompressionName(uint magic)
        {
            if (magic == Compression.LZ4Frame || magic == Compression.LZ4Block)
                return "LZ4";
            if (magic == Compression.Zstd)
                return "Zstd";
            if (magic == Compression.Brotli)
                return "Brotli";
            if (magic == Compression.Snappy)
                return "Snappy";
            if (magic == Compression.XZ)
                return "XZ";
            if (magic == Compression.LZMA)
                return "LZMA";
            
            return "Unknown";
        }

        /// <summary>
        /// 获取加密算法名称
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>加密算法名称</returns>
        public static string GetEncryptionName(uint magic)
        {
            if (magic == Encryption.AES)
                return "AES";
            if (magic == Encryption.XOR)
                return "XOR";
            if (magic == Encryption.ChaCha20)
                return "ChaCha20";
            if (magic == Encryption.None)
                return "None";
            
            return "Unknown";
        }

        /// <summary>
        /// 获取校验算法名称
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <returns>校验算法名称</returns>
        public static string GetChecksumName(uint magic)
        {
            if (magic == Checksum.CRC32)
                return "CRC32";
            if (magic == Checksum.CRC64)
                return "CRC64";
            if (magic == Checksum.XXHash32)
                return "XXHash32";
            if (magic == Checksum.XXHash64)
                return "XXHash64";
            if (magic == Checksum.XXHash128)
                return "XXHash128";
            if (magic == Checksum.MD5)
                return "MD5";
            if (magic == Checksum.SHA256)
                return "SHA256";
            if (magic == Checksum.None)
                return "None";
            
            return "Unknown";
        }

        /// <summary>
        /// 检查版本是否受支持
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns>如果版本受支持返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVersionSupported(ushort version)
        {
            return version >= Version.MinSupported && version <= Version.Current;
        }

        /// <summary>
        /// 交换字节序
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>字节序交换后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SwapEndianness(uint value)
        {
            return ((value & 0x000000FF) << 24) |
                   ((value & 0x0000FF00) << 8) |
                   ((value & 0x00FF0000) >> 8) |
                   ((value & 0xFF000000) >> 24);
        }

        /// <summary>
        /// 交换字节序 (16位)
        /// </summary>
        /// <param name="value">原始值</param>
        /// <returns>字节序交换后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SwapEndianness(ushort value)
        {
            return (ushort)(((value & 0x00FF) << 8) | ((value & 0xFF00) >> 8));
        }

        /// <summary>
        /// 从字节数组读取魔数
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <returns>魔数值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadMagic(ReadOnlySpan<byte> data)
        {
            if (data.Length < 4) return 0;
            return BitConverter.ToUInt32(data);
        }

        /// <summary>
        /// 将魔数写入字节数组
        /// </summary>
        /// <param name="magic">魔数值</param>
        /// <param name="destination">目标数组</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMagic(uint magic, Span<byte> destination)
        {
            if (destination.Length < 4) return;
            BitConverter.TryWriteBytes(destination, magic);
        }

        #endregion
    }
}
