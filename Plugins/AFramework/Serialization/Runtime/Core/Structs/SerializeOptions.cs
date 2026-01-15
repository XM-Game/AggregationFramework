// ==========================================================
// 文件名：SerializeOptions.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化选项结构体
    /// <para>配置序列化行为的各种选项参数</para>
    /// <para>支持流畅 API 和 Builder 模式构建</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认选项
    /// var options = SerializeOptions.Default;
    /// 
    /// // 使用 Builder 模式
    /// var options = SerializeOptions.Create()
    ///     .WithMode(SerializeMode.VersionTolerant)
    ///     .WithCompression(CompressionAlgorithm.LZ4)
    ///     .WithMaxDepth(128)
    ///     .Build();
    /// 
    /// // 使用预设配置
    /// var networkOptions = SerializeOptions.ForNetwork;
    /// var storageOptions = SerializeOptions.ForStorage;
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct SerializeOptions : IEquatable<SerializeOptions>
    {
        #region 字段

        private readonly SerializeMode _mode;
        private readonly CompressionAlgorithm _compression;
        private readonly CompressionLevel _compressionLevel;
        private readonly EncryptionAlgorithm _encryption;
        private readonly ChecksumAlgorithm _checksum;
        private readonly StringEncoding _stringEncoding;
        private readonly Endianness _endianness;
        private readonly SerializeLayout _layout;
        private readonly int _maxDepth;
        private readonly int _maxObjectCount;
        private readonly int _timeoutMs;
        private readonly SerializeFlags _flags;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建序列化选项
        /// </summary>
        private SerializeOptions(
            SerializeMode mode,
            CompressionAlgorithm compression,
            CompressionLevel compressionLevel,
            EncryptionAlgorithm encryption,
            ChecksumAlgorithm checksum,
            StringEncoding stringEncoding,
            Endianness endianness,
            SerializeLayout layout,
            int maxDepth,
            int maxObjectCount,
            int timeoutMs,
            SerializeFlags flags)
        {
            _mode = mode;
            _compression = compression;
            _compressionLevel = compressionLevel;
            _encryption = encryption;
            _checksum = checksum;
            _stringEncoding = stringEncoding;
            _endianness = endianness;
            _layout = layout;
            _maxDepth = maxDepth;
            _maxObjectCount = maxObjectCount;
            _timeoutMs = timeoutMs;
            _flags = flags;
        }

        #endregion

        #region 属性

        /// <summary>序列化模式</summary>
        public SerializeMode Mode => _mode;

        /// <summary>压缩算法</summary>
        public CompressionAlgorithm Compression => _compression;

        /// <summary>压缩级别</summary>
        public CompressionLevel CompressionLevel => _compressionLevel;

        /// <summary>加密算法</summary>
        public EncryptionAlgorithm Encryption => _encryption;

        /// <summary>校验算法</summary>
        public ChecksumAlgorithm Checksum => _checksum;

        /// <summary>字符串编码</summary>
        public StringEncoding StringEncoding => _stringEncoding;

        /// <summary>字节序</summary>
        public Endianness Endianness => _endianness;

        /// <summary>布局方式</summary>
        public SerializeLayout Layout => _layout;

        /// <summary>最大序列化深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>最大对象数量 (循环引用模式)</summary>
        public int MaxObjectCount => _maxObjectCount;

        /// <summary>超时时间 (毫秒)</summary>
        public int TimeoutMs => _timeoutMs;

        /// <summary>序列化标志</summary>
        public SerializeFlags Flags => _flags;

        #endregion

        #region 标志属性

        /// <summary>是否启用压缩</summary>
        public bool IsCompressionEnabled => _compression != CompressionAlgorithm.None;

        /// <summary>是否启用加密</summary>
        public bool IsEncryptionEnabled => _encryption != EncryptionAlgorithm.None;

        /// <summary>是否启用校验和</summary>
        public bool IsChecksumEnabled => _checksum != ChecksumAlgorithm.None;

        /// <summary>是否包含类型信息</summary>
        public bool IncludeTypeInfo => (_flags & SerializeFlags.IncludeTypeInfo) != 0;

        /// <summary>是否包含字段名称</summary>
        public bool IncludeFieldNames => (_flags & SerializeFlags.IncludeFieldNames) != 0;

        /// <summary>是否忽略空值</summary>
        public bool IgnoreNullValues => (_flags & SerializeFlags.IgnoreNullValues) != 0;

        /// <summary>是否忽略默认值</summary>
        public bool IgnoreDefaultValues => (_flags & SerializeFlags.IgnoreDefaultValues) != 0;

        /// <summary>是否包含私有成员</summary>
        public bool IncludePrivateMembers => (_flags & SerializeFlags.IncludePrivateMembers) != 0;

        /// <summary>是否启用字符串内化</summary>
        public bool EnableStringIntern => (_flags & SerializeFlags.EnableStringIntern) != 0;

        /// <summary>是否启用调试信息</summary>
        public bool EnableDebugInfo => (_flags & SerializeFlags.EnableDebugInfo) != 0;

        #endregion

        #region 预设配置

        /// <summary>
        /// 默认选项
        /// <para>高性能模式，无压缩加密</para>
        /// </summary>
        public static SerializeOptions Default => new SerializeOptions(
            mode: SerializeMode.Object,
            compression: CompressionAlgorithm.None,
            compressionLevel: CompressionLevel.Optimal,
            encryption: EncryptionAlgorithm.None,
            checksum: ChecksumAlgorithm.None,
            stringEncoding: StringEncoding.UTF8,
            endianness: Endianness.Native,
            layout: SerializeLayout.Sequential,
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            timeoutMs: SerializeConstants.Timeout.DefaultSerializeMs,
            flags: SerializeFlags.None
        );

        /// <summary>
        /// 网络通信优化选项
        /// <para>高性能、紧凑格式</para>
        /// </summary>
        public static SerializeOptions ForNetwork => new SerializeOptions(
            mode: SerializeMode.Object,
            compression: CompressionAlgorithm.None,
            compressionLevel: CompressionLevel.Fastest,
            encryption: EncryptionAlgorithm.None,
            checksum: ChecksumAlgorithm.CRC32,
            stringEncoding: StringEncoding.UTF8,
            endianness: Endianness.Little,
            layout: SerializeLayout.Sequential,
            maxDepth: 32,
            maxObjectCount: 10000,
            timeoutMs: SerializeConstants.Timeout.FastOperationMs,
            flags: SerializeFlags.IgnoreNullValues | SerializeFlags.IgnoreDefaultValues
        );

        /// <summary>
        /// 存储优化选项
        /// <para>版本容错、压缩、校验</para>
        /// </summary>
        public static SerializeOptions ForStorage => new SerializeOptions(
            mode: SerializeMode.VersionTolerant,
            compression: CompressionAlgorithm.LZ4,
            compressionLevel: CompressionLevel.Optimal,
            encryption: EncryptionAlgorithm.None,
            checksum: ChecksumAlgorithm.XXHash64,
            stringEncoding: StringEncoding.UTF8,
            endianness: Endianness.Little,
            layout: SerializeLayout.KeyValue,
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            timeoutMs: SerializeConstants.Timeout.DefaultSerializeMs,
            flags: SerializeFlags.IncludeFieldNames
        );

        /// <summary>
        /// 游戏存档选项
        /// <para>完整模式、压缩、加密</para>
        /// </summary>
        public static SerializeOptions ForSaveGame => new SerializeOptions(
            mode: SerializeMode.Full,
            compression: CompressionAlgorithm.LZ4,
            compressionLevel: CompressionLevel.Optimal,
            encryption: EncryptionAlgorithm.AES256,
            checksum: ChecksumAlgorithm.XXHash64,
            stringEncoding: StringEncoding.UTF8,
            endianness: Endianness.Little,
            layout: SerializeLayout.KeyValue,
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            timeoutMs: SerializeConstants.Timeout.DefaultSerializeMs,
            flags: SerializeFlags.IncludeTypeInfo | SerializeFlags.IncludeFieldNames
        );

        /// <summary>
        /// 调试选项
        /// <para>包含完整调试信息</para>
        /// </summary>
        public static SerializeOptions ForDebug => new SerializeOptions(
            mode: SerializeMode.Debug,
            compression: CompressionAlgorithm.None,
            compressionLevel: CompressionLevel.NoCompression,
            encryption: EncryptionAlgorithm.None,
            checksum: ChecksumAlgorithm.CRC32,
            stringEncoding: StringEncoding.UTF8,
            endianness: Endianness.Native,
            layout: SerializeLayout.KeyValue,
            maxDepth: SerializeConstants.MaxSerializeDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            timeoutMs: SerializeConstants.Timeout.Infinite,
            flags: SerializeFlags.IncludeTypeInfo | SerializeFlags.IncludeFieldNames | SerializeFlags.EnableDebugInfo
        );

        #endregion

        #region Builder 模式

        /// <summary>
        /// 创建选项构建器
        /// </summary>
        /// <returns>选项构建器</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeOptionsBuilder Create() => new SerializeOptionsBuilder();

        /// <summary>
        /// 从现有选项创建构建器
        /// </summary>
        /// <returns>选项构建器</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializeOptionsBuilder ToBuilder() => new SerializeOptionsBuilder(this);

        #endregion

        #region With 方法 (不可变修改)

        /// <summary>修改序列化模式</summary>
        public SerializeOptions WithMode(SerializeMode mode) =>
            new SerializeOptions(mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改压缩算法</summary>
        public SerializeOptions WithCompression(CompressionAlgorithm compression) =>
            new SerializeOptions(_mode, compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改压缩级别</summary>
        public SerializeOptions WithCompressionLevel(CompressionLevel level) =>
            new SerializeOptions(_mode, _compression, level, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改加密算法</summary>
        public SerializeOptions WithEncryption(EncryptionAlgorithm encryption) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改校验算法</summary>
        public SerializeOptions WithChecksum(ChecksumAlgorithm checksum) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改字符串编码</summary>
        public SerializeOptions WithStringEncoding(StringEncoding encoding) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                encoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改字节序</summary>
        public SerializeOptions WithEndianness(Endianness endianness) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改布局方式</summary>
        public SerializeOptions WithLayout(SerializeLayout layout) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改最大深度</summary>
        public SerializeOptions WithMaxDepth(int maxDepth) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, maxDepth, _maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改最大对象数量</summary>
        public SerializeOptions WithMaxObjectCount(int maxObjectCount) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, maxObjectCount, _timeoutMs, _flags);

        /// <summary>修改超时时间</summary>
        public SerializeOptions WithTimeout(int timeoutMs) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, timeoutMs, _flags);

        /// <summary>修改标志</summary>
        public SerializeOptions WithFlags(SerializeFlags flags) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, flags);

        /// <summary>添加标志</summary>
        public SerializeOptions AddFlags(SerializeFlags flags) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags | flags);

        /// <summary>移除标志</summary>
        public SerializeOptions RemoveFlags(SerializeFlags flags) =>
            new SerializeOptions(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags & ~flags);

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        /// <returns>如果选项有效返回 true</returns>
        public bool IsValid()
        {
            return IsValid(out _);
        }

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>如果选项有效返回 true</returns>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (!SerializeConstants.IsValidDepth(_maxDepth))
            {
                errorMessage = $"最大深度 {_maxDepth} 超出有效范围 [{SerializeConstants.MinSerializeDepth}, {SerializeConstants.MaxSerializeDepth}]";
                return false;
            }

            if (_maxObjectCount <= 0)
            {
                errorMessage = "最大对象数量必须大于 0";
                return false;
            }

            if (_timeoutMs < -1)
            {
                errorMessage = "超时时间必须大于等于 -1 (无限)";
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeOptions other)
        {
            return _mode == other._mode &&
                   _compression == other._compression &&
                   _compressionLevel == other._compressionLevel &&
                   _encryption == other._encryption &&
                   _checksum == other._checksum &&
                   _stringEncoding == other._stringEncoding &&
                   _endianness == other._endianness &&
                   _layout == other._layout &&
                   _maxDepth == other._maxDepth &&
                   _maxObjectCount == other._maxObjectCount &&
                   _timeoutMs == other._timeoutMs &&
                   _flags == other._flags;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeOptions other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_mode);
            hash.Add(_compression);
            hash.Add(_compressionLevel);
            hash.Add(_encryption);
            hash.Add(_checksum);
            hash.Add(_stringEncoding);
            hash.Add(_endianness);
            hash.Add(_layout);
            hash.Add(_maxDepth);
            hash.Add(_maxObjectCount);
            hash.Add(_timeoutMs);
            hash.Add(_flags);
            return hash.ToHashCode();
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeOptions left, SerializeOptions right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeOptions left, SerializeOptions right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"SerializeOptions(Mode={_mode}, Compression={_compression}, Encryption={_encryption})";
        }

        #endregion

        #region 内部构建方法

        /// <summary>
        /// 内部构建方法 (供 Builder 使用)
        /// </summary>
        internal static SerializeOptions Build(
            SerializeMode mode,
            CompressionAlgorithm compression,
            CompressionLevel compressionLevel,
            EncryptionAlgorithm encryption,
            ChecksumAlgorithm checksum,
            StringEncoding stringEncoding,
            Endianness endianness,
            SerializeLayout layout,
            int maxDepth,
            int maxObjectCount,
            int timeoutMs,
            SerializeFlags flags)
        {
            return new SerializeOptions(mode, compression, compressionLevel, encryption, checksum,
                stringEncoding, endianness, layout, maxDepth, maxObjectCount, timeoutMs, flags);
        }

        #endregion
    }

    #region 序列化标志枚举

    /// <summary>
    /// 序列化标志枚举
    /// <para>定义序列化行为的各种开关选项</para>
    /// </summary>
    [Flags]
    public enum SerializeFlags : uint
    {
        /// <summary>无标志</summary>
        None = 0,

        /// <summary>包含类型信息</summary>
        IncludeTypeInfo = 1 << 0,

        /// <summary>包含字段名称</summary>
        IncludeFieldNames = 1 << 1,

        /// <summary>忽略空值</summary>
        IgnoreNullValues = 1 << 2,

        /// <summary>忽略默认值</summary>
        IgnoreDefaultValues = 1 << 3,

        /// <summary>包含私有成员</summary>
        IncludePrivateMembers = 1 << 4,

        /// <summary>启用字符串内化</summary>
        EnableStringIntern = 1 << 5,

        /// <summary>启用调试信息</summary>
        EnableDebugInfo = 1 << 6,

        /// <summary>使用驼峰命名</summary>
        UseCamelCase = 1 << 7,

        /// <summary>保留引用</summary>
        PreserveReferences = 1 << 8,

        /// <summary>忽略只读属性</summary>
        IgnoreReadOnlyProperties = 1 << 9,

        /// <summary>启用延迟加载</summary>
        EnableLazyLoading = 1 << 10,

        /// <summary>启用进度回调</summary>
        EnableProgressCallback = 1 << 11,

        /// <summary>启用取消支持</summary>
        EnableCancellation = 1 << 12,

        /// <summary>严格模式 (遇到错误立即抛出)</summary>
        StrictMode = 1 << 13,

        /// <summary>宽松模式 (尽可能忽略错误)</summary>
        LenientMode = 1 << 14
    }

    #endregion

    #region 选项构建器

    /// <summary>
    /// 序列化选项构建器
    /// <para>提供流畅 API 构建序列化选项</para>
    /// </summary>
    public sealed class SerializeOptionsBuilder
    {
        private SerializeMode _mode = SerializeMode.Object;
        private CompressionAlgorithm _compression = CompressionAlgorithm.None;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;
        private EncryptionAlgorithm _encryption = EncryptionAlgorithm.None;
        private ChecksumAlgorithm _checksum = ChecksumAlgorithm.None;
        private StringEncoding _stringEncoding = StringEncoding.UTF8;
        private Endianness _endianness = Endianness.Native;
        private SerializeLayout _layout = SerializeLayout.Sequential;
        private int _maxDepth = SerializeConstants.DefaultMaxDepth;
        private int _maxObjectCount = SerializeConstants.SizeLimit.MaxObjectCount;
        private int _timeoutMs = SerializeConstants.Timeout.DefaultSerializeMs;
        private SerializeFlags _flags = SerializeFlags.None;

        /// <summary>创建默认构建器</summary>
        public SerializeOptionsBuilder() { }

        /// <summary>从现有选项创建构建器</summary>
        public SerializeOptionsBuilder(SerializeOptions options)
        {
            _mode = options.Mode;
            _compression = options.Compression;
            _compressionLevel = options.CompressionLevel;
            _encryption = options.Encryption;
            _checksum = options.Checksum;
            _stringEncoding = options.StringEncoding;
            _endianness = options.Endianness;
            _layout = options.Layout;
            _maxDepth = options.MaxDepth;
            _maxObjectCount = options.MaxObjectCount;
            _timeoutMs = options.TimeoutMs;
            _flags = options.Flags;
        }

        /// <summary>设置序列化模式</summary>
        public SerializeOptionsBuilder WithMode(SerializeMode mode) { _mode = mode; return this; }

        /// <summary>设置压缩算法</summary>
        public SerializeOptionsBuilder WithCompression(CompressionAlgorithm compression) { _compression = compression; return this; }

        /// <summary>设置压缩级别</summary>
        public SerializeOptionsBuilder WithCompressionLevel(CompressionLevel level) { _compressionLevel = level; return this; }

        /// <summary>设置加密算法</summary>
        public SerializeOptionsBuilder WithEncryption(EncryptionAlgorithm encryption) { _encryption = encryption; return this; }

        /// <summary>设置校验算法</summary>
        public SerializeOptionsBuilder WithChecksum(ChecksumAlgorithm checksum) { _checksum = checksum; return this; }

        /// <summary>设置字符串编码</summary>
        public SerializeOptionsBuilder WithStringEncoding(StringEncoding encoding) { _stringEncoding = encoding; return this; }

        /// <summary>设置字节序</summary>
        public SerializeOptionsBuilder WithEndianness(Endianness endianness) { _endianness = endianness; return this; }

        /// <summary>设置布局方式</summary>
        public SerializeOptionsBuilder WithLayout(SerializeLayout layout) { _layout = layout; return this; }

        /// <summary>设置最大深度</summary>
        public SerializeOptionsBuilder WithMaxDepth(int maxDepth) { _maxDepth = maxDepth; return this; }

        /// <summary>设置最大对象数量</summary>
        public SerializeOptionsBuilder WithMaxObjectCount(int maxObjectCount) { _maxObjectCount = maxObjectCount; return this; }

        /// <summary>设置超时时间</summary>
        public SerializeOptionsBuilder WithTimeout(int timeoutMs) { _timeoutMs = timeoutMs; return this; }

        /// <summary>设置标志</summary>
        public SerializeOptionsBuilder WithFlags(SerializeFlags flags) { _flags = flags; return this; }

        /// <summary>添加标志</summary>
        public SerializeOptionsBuilder AddFlags(SerializeFlags flags) { _flags |= flags; return this; }

        /// <summary>移除标志</summary>
        public SerializeOptionsBuilder RemoveFlags(SerializeFlags flags) { _flags &= ~flags; return this; }

        /// <summary>启用类型信息</summary>
        public SerializeOptionsBuilder IncludeTypeInfo() => AddFlags(SerializeFlags.IncludeTypeInfo);

        /// <summary>启用字段名称</summary>
        public SerializeOptionsBuilder IncludeFieldNames() => AddFlags(SerializeFlags.IncludeFieldNames);

        /// <summary>忽略空值</summary>
        public SerializeOptionsBuilder IgnoreNullValues() => AddFlags(SerializeFlags.IgnoreNullValues);

        /// <summary>忽略默认值</summary>
        public SerializeOptionsBuilder IgnoreDefaultValues() => AddFlags(SerializeFlags.IgnoreDefaultValues);

        /// <summary>启用调试信息</summary>
        public SerializeOptionsBuilder EnableDebugInfo() => AddFlags(SerializeFlags.EnableDebugInfo);

        /// <summary>构建选项</summary>
        public SerializeOptions Build()
        {
            return SerializeOptions.Build(_mode, _compression, _compressionLevel, _encryption, _checksum,
                _stringEncoding, _endianness, _layout, _maxDepth, _maxObjectCount, _timeoutMs, _flags);
        }
    }

    #endregion
}
