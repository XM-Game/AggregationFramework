// ==========================================================
// 文件名：BinarySerializerOptions.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 二进制序列化器选项
    /// <para>配置二进制序列化的行为参数</para>
    /// <para>支持流畅 API 和预设配置</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认选项
    /// var options = BinarySerializerOptions.Default;
    /// 
    /// // 使用预设配置
    /// var networkOptions = BinarySerializerOptions.ForNetwork;
    /// var storageOptions = BinarySerializerOptions.ForStorage;
    /// 
    /// // 自定义配置
    /// var options = BinarySerializerOptions.Default
    ///     .WithHeader(true)
    ///     .WithChecksum(true)
    ///     .WithStringIntern(true);
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct BinarySerializerOptions : IEquatable<BinarySerializerOptions>
    {
        #region 字段

        private readonly bool _includeHeader;
        private readonly bool _includeChecksum;
        private readonly bool _enableStringIntern;
        private readonly bool _enableCircularReference;
        private readonly bool _useVarInt;
        private readonly bool _includeTypeInfo;
        private readonly int _maxDepth;
        private readonly int _maxStringLength;
        private readonly int _maxArrayLength;
        private readonly int _initialBufferSize;
        private readonly Endianness _endianness;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建二进制序列化器选项
        /// </summary>
        private BinarySerializerOptions(
            bool includeHeader,
            bool includeChecksum,
            bool enableStringIntern,
            bool enableCircularReference,
            bool useVarInt,
            bool includeTypeInfo,
            int maxDepth,
            int maxStringLength,
            int maxArrayLength,
            int initialBufferSize,
            Endianness endianness)
        {
            _includeHeader = includeHeader;
            _includeChecksum = includeChecksum;
            _enableStringIntern = enableStringIntern;
            _enableCircularReference = enableCircularReference;
            _useVarInt = useVarInt;
            _includeTypeInfo = includeTypeInfo;
            _maxDepth = maxDepth;
            _maxStringLength = maxStringLength;
            _maxArrayLength = maxArrayLength;
            _initialBufferSize = initialBufferSize;
            _endianness = endianness;
        }

        #endregion

        #region 属性

        /// <summary>是否包含数据头</summary>
        public bool IncludeHeader => _includeHeader;

        /// <summary>是否包含校验和</summary>
        public bool IncludeChecksum => _includeChecksum;

        /// <summary>是否启用字符串内化</summary>
        public bool EnableStringIntern => _enableStringIntern;

        /// <summary>是否启用循环引用检测</summary>
        public bool EnableCircularReference => _enableCircularReference;

        /// <summary>是否使用 VarInt 编码整数</summary>
        public bool UseVarInt => _useVarInt;

        /// <summary>是否包含类型信息</summary>
        public bool IncludeTypeInfo => _includeTypeInfo;

        /// <summary>最大序列化深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>最大字符串长度</summary>
        public int MaxStringLength => _maxStringLength;

        /// <summary>最大数组长度</summary>
        public int MaxArrayLength => _maxArrayLength;

        /// <summary>初始缓冲区大小</summary>
        public int InitialBufferSize => _initialBufferSize;

        /// <summary>字节序</summary>
        public Endianness Endianness => _endianness;

        #endregion

        #region 预设配置

        /// <summary>
        /// 默认选项
        /// <para>高性能模式，无头部，使用 VarInt</para>
        /// </summary>
        public static BinarySerializerOptions Default => new BinarySerializerOptions(
            includeHeader: false,
            includeChecksum: false,
            enableStringIntern: false,
            enableCircularReference: false,
            useVarInt: true,
            includeTypeInfo: false,
            maxDepth: 64,
            maxStringLength: 10 * 1024 * 1024, // 10MB
            maxArrayLength: 10 * 1024 * 1024,  // 10M 元素
            initialBufferSize: 256,
            endianness: Endianness.Little
        );

        /// <summary>
        /// 网络通信优化选项
        /// <para>紧凑格式，带校验和</para>
        /// </summary>
        public static BinarySerializerOptions ForNetwork => new BinarySerializerOptions(
            includeHeader: false,
            includeChecksum: true,
            enableStringIntern: true,
            enableCircularReference: false,
            useVarInt: true,
            includeTypeInfo: false,
            maxDepth: 32,
            maxStringLength: 1024 * 1024, // 1MB
            maxArrayLength: 100000,
            initialBufferSize: 512,
            endianness: Endianness.Little
        );

        /// <summary>
        /// 存储优化选项
        /// <para>带头部，支持版本检测</para>
        /// </summary>
        public static BinarySerializerOptions ForStorage => new BinarySerializerOptions(
            includeHeader: true,
            includeChecksum: true,
            enableStringIntern: true,
            enableCircularReference: true,
            useVarInt: true,
            includeTypeInfo: true,
            maxDepth: 128,
            maxStringLength: 100 * 1024 * 1024, // 100MB
            maxArrayLength: 100 * 1024 * 1024,
            initialBufferSize: 1024,
            endianness: Endianness.Little
        );

        /// <summary>
        /// 游戏存档选项
        /// <para>完整模式，支持循环引用</para>
        /// </summary>
        public static BinarySerializerOptions ForSaveGame => new BinarySerializerOptions(
            includeHeader: true,
            includeChecksum: true,
            enableStringIntern: true,
            enableCircularReference: true,
            useVarInt: true,
            includeTypeInfo: true,
            maxDepth: 256,
            maxStringLength: 10 * 1024 * 1024,
            maxArrayLength: 10 * 1024 * 1024,
            initialBufferSize: 4096,
            endianness: Endianness.Little
        );

        /// <summary>
        /// 最小化选项
        /// <para>最紧凑格式，无额外开销</para>
        /// </summary>
        public static BinarySerializerOptions Minimal => new BinarySerializerOptions(
            includeHeader: false,
            includeChecksum: false,
            enableStringIntern: false,
            enableCircularReference: false,
            useVarInt: true,
            includeTypeInfo: false,
            maxDepth: 32,
            maxStringLength: 1024 * 1024,
            maxArrayLength: 100000,
            initialBufferSize: 128,
            endianness: Endianness.Little
        );

        /// <summary>
        /// 调试选项
        /// <para>包含完整信息，便于调试</para>
        /// </summary>
        public static BinarySerializerOptions ForDebug => new BinarySerializerOptions(
            includeHeader: true,
            includeChecksum: true,
            enableStringIntern: false,
            enableCircularReference: true,
            useVarInt: false, // 使用固定大小便于调试
            includeTypeInfo: true,
            maxDepth: 512,
            maxStringLength: int.MaxValue,
            maxArrayLength: int.MaxValue,
            initialBufferSize: 1024,
            endianness: Endianness.Little
        );

        #endregion

        #region With 方法

        /// <summary>修改是否包含头部</summary>
        public BinarySerializerOptions WithHeader(bool include) =>
            new BinarySerializerOptions(include, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改是否包含校验和</summary>
        public BinarySerializerOptions WithChecksum(bool include) =>
            new BinarySerializerOptions(_includeHeader, include, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改是否启用字符串内化</summary>
        public BinarySerializerOptions WithStringIntern(bool enable) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, enable, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改是否启用循环引用检测</summary>
        public BinarySerializerOptions WithCircularReference(bool enable) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, enable,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改是否使用 VarInt</summary>
        public BinarySerializerOptions WithVarInt(bool use) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                use, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改是否包含类型信息</summary>
        public BinarySerializerOptions WithTypeInfo(bool include) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, include, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改最大深度</summary>
        public BinarySerializerOptions WithMaxDepth(int maxDepth) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改最大字符串长度</summary>
        public BinarySerializerOptions WithMaxStringLength(int maxLength) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, maxLength, _maxArrayLength, _initialBufferSize, _endianness);

        /// <summary>修改最大数组长度</summary>
        public BinarySerializerOptions WithMaxArrayLength(int maxLength) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, maxLength, _initialBufferSize, _endianness);

        /// <summary>修改初始缓冲区大小</summary>
        public BinarySerializerOptions WithInitialBufferSize(int size) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, size, _endianness);

        /// <summary>修改字节序</summary>
        public BinarySerializerOptions WithEndianness(Endianness endianness) =>
            new BinarySerializerOptions(_includeHeader, _includeChecksum, _enableStringIntern, _enableCircularReference,
                _useVarInt, _includeTypeInfo, _maxDepth, _maxStringLength, _maxArrayLength, _initialBufferSize, endianness);

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为通用序列化选项
        /// </summary>
        public SerializeOptions ToSerializeOptions()
        {
            var flags = SerializeFlags.None;
            if (_includeTypeInfo) flags |= SerializeFlags.IncludeTypeInfo;
            if (_enableStringIntern) flags |= SerializeFlags.EnableStringIntern;

            return SerializeOptions.Create()
                .WithMaxDepth(_maxDepth)
                .WithEndianness(_endianness)
                .WithChecksum(_includeChecksum ? ChecksumAlgorithm.CRC32 : ChecksumAlgorithm.None)
                .WithFlags(flags)
                .Build();
        }

        /// <summary>
        /// 从通用序列化选项创建
        /// </summary>
        public static BinarySerializerOptions FromSerializeOptions(SerializeOptions options)
        {
            return Default
                .WithChecksum(options.IsChecksumEnabled)
                .WithStringIntern(options.EnableStringIntern)
                .WithTypeInfo(options.IncludeTypeInfo)
                .WithMaxDepth(options.MaxDepth)
                .WithEndianness(options.Endianness);
        }

        /// <summary>
        /// 获取头部标志
        /// </summary>
        internal BinaryHeaderFlags GetHeaderFlags()
        {
            var flags = BinaryHeaderFlags.None;

            if (_includeChecksum) flags |= BinaryHeaderFlags.HasChecksum;
            if (_includeTypeInfo) flags |= BinaryHeaderFlags.HasTypeInfo;
            if (_enableStringIntern) flags |= BinaryHeaderFlags.StringIntern;
            if (_enableCircularReference) flags |= BinaryHeaderFlags.CircularReference;
            if (_useVarInt) flags |= BinaryHeaderFlags.UseVarInt;
            if (_endianness == Endianness.Big) flags |= BinaryHeaderFlags.BigEndian;

            return flags;
        }

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (_maxDepth <= 0)
            {
                errorMessage = "最大深度必须大于 0";
                return false;
            }

            if (_maxStringLength <= 0)
            {
                errorMessage = "最大字符串长度必须大于 0";
                return false;
            }

            if (_maxArrayLength <= 0)
            {
                errorMessage = "最大数组长度必须大于 0";
                return false;
            }

            if (_initialBufferSize <= 0)
            {
                errorMessage = "初始缓冲区大小必须大于 0";
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(BinarySerializerOptions other)
        {
            return _includeHeader == other._includeHeader &&
                   _includeChecksum == other._includeChecksum &&
                   _enableStringIntern == other._enableStringIntern &&
                   _enableCircularReference == other._enableCircularReference &&
                   _useVarInt == other._useVarInt &&
                   _includeTypeInfo == other._includeTypeInfo &&
                   _maxDepth == other._maxDepth &&
                   _maxStringLength == other._maxStringLength &&
                   _maxArrayLength == other._maxArrayLength &&
                   _initialBufferSize == other._initialBufferSize &&
                   _endianness == other._endianness;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is BinarySerializerOptions other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_includeHeader);
            hash.Add(_includeChecksum);
            hash.Add(_enableStringIntern);
            hash.Add(_enableCircularReference);
            hash.Add(_useVarInt);
            hash.Add(_includeTypeInfo);
            hash.Add(_maxDepth);
            hash.Add(_endianness);
            return hash.ToHashCode();
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(BinarySerializerOptions left, BinarySerializerOptions right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(BinarySerializerOptions left, BinarySerializerOptions right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"BinarySerializerOptions(Header={_includeHeader}, Checksum={_includeChecksum}, VarInt={_useVarInt})";
        }

        #endregion
    }
}
