// ==========================================================
// 文件名：SerializerConfiguration.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化器配置
    /// <para>定义序列化器的行为和选项</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>配置序列化模式和布局</item>
    ///   <item>配置版本容错和多态支持</item>
    ///   <item>配置压缩和加密选项</item>
    ///   <item>配置性能相关参数</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 使用预设配置
    /// var config = SerializerConfiguration.Default;
    /// var tolerant = SerializerConfiguration.VersionTolerant;
    /// 
    /// // 使用建造者模式
    /// var config = SerializerConfigurationBuilder.Create()
    ///     .WithMode(SerializeMode.VersionTolerant)
    ///     .WithCompression(CompressionAlgorithm.LZ4)
    ///     .Build();
    /// </code>
    /// </remarks>
    public sealed class SerializerConfiguration : ConfigurationBase<SerializerConfiguration>
    {
        #region 静态实例

        /// <summary>默认配置（高性能模式）</summary>
        public static SerializerConfiguration Default { get; } = new SerializerConfiguration().AsReadOnly();

        /// <summary>版本容错配置</summary>
        public static SerializerConfiguration VersionTolerant { get; } = new SerializerConfiguration
        {
            _mode = SerializeMode.VersionTolerant,
            _layout = SerializeLayout.KeyValue,
            _preserveUnknownFields = true
        }.AsReadOnly();

        /// <summary>紧凑配置（最小体积）</summary>
        public static SerializerConfiguration Compact { get; } = new SerializerConfiguration
        {
            _mode = SerializeMode.Object,
            _layout = SerializeLayout.Sequential,
            _enableCompression = true,
            _compressionAlgorithm = CompressionAlgorithm.LZ4
        }.AsReadOnly();

        #endregion

        #region 字段 - 基本配置

        private SerializeMode _mode = SerializeMode.Object;
        private SerializeLayout _layout = SerializeLayout.Sequential;
        private Endianness _endianness = Endianness.Little;
        private StringEncoding _defaultStringEncoding = StringEncoding.UTF8;

        #endregion

        #region 字段 - 版本控制

        private bool _enableVersioning = true;
        private bool _preserveUnknownFields;
        private bool _strictVersionCheck;
        private Version _minSupportedVersion;

        #endregion

        #region 字段 - 多态支持

        private bool _enablePolymorphism = true;
        private bool _includeTypeInfo;
        private TypeNameHandling _typeNameHandling = TypeNameHandling.Auto;

        #endregion

        #region 字段 - 循环引用

        private bool _enableCircularReference;
        private int _maxReferenceDepth = 64;

        #endregion

        #region 字段 - 压缩配置

        private bool _enableCompression;
        private CompressionAlgorithm _compressionAlgorithm = CompressionAlgorithm.None;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;
        private int _compressionThreshold = 256;

        #endregion

        #region 字段 - 加密配置

        private bool _enableEncryption;
        private EncryptionAlgorithm _encryptionAlgorithm = EncryptionAlgorithm.None;

        #endregion

        #region 字段 - 校验配置

        private bool _enableChecksum;
        private ChecksumAlgorithm _checksumAlgorithm = ChecksumAlgorithm.None;

        #endregion

        #region 字段 - 性能配置

        private bool _usePooledBuffers = true;
        private int _initialBufferSize = 4096;
        private int _maxBufferSize = 1024 * 1024;
        private bool _enableParallelProcessing;

        #endregion

        #region 字段 - 回调配置

        private bool _invokeCallbacks = true;
        private bool _continueOnCallbackError;

        #endregion

        #region 字段 - 调试配置

        private bool _enableDiagnostics;
        private bool _throwOnError = true;

        #endregion

        #region 属性 - 基本配置

        /// <summary>
        /// 获取或设置序列化模式
        /// <para>默认值：<see cref="SerializeMode.Object"/></para>
        /// </summary>
        public SerializeMode Mode
        {
            get => _mode;
            set { ThrowIfReadOnly(); _mode = value; }
        }

        /// <summary>
        /// 获取或设置序列化布局
        /// <para>默认值：<see cref="SerializeLayout.Sequential"/></para>
        /// </summary>
        public SerializeLayout Layout
        {
            get => _layout;
            set { ThrowIfReadOnly(); _layout = value; }
        }

        /// <summary>
        /// 获取或设置字节序
        /// <para>默认值：<see cref="Endianness.Little"/></para>
        /// </summary>
        public Endianness Endianness
        {
            get => _endianness;
            set { ThrowIfReadOnly(); _endianness = value; }
        }

        /// <summary>
        /// 获取或设置默认字符串编码
        /// <para>默认值：<see cref="StringEncoding.UTF8"/></para>
        /// </summary>
        public StringEncoding DefaultStringEncoding
        {
            get => _defaultStringEncoding;
            set { ThrowIfReadOnly(); _defaultStringEncoding = value; }
        }

        #endregion

        #region 属性 - 版本控制

        /// <summary>
        /// 获取或设置是否启用版本控制
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableVersioning
        {
            get => _enableVersioning;
            set { ThrowIfReadOnly(); _enableVersioning = value; }
        }

        /// <summary>
        /// 获取或设置是否保留未知字段
        /// <para>默认值：false</para>
        /// </summary>
        public bool PreserveUnknownFields
        {
            get => _preserveUnknownFields;
            set { ThrowIfReadOnly(); _preserveUnknownFields = value; }
        }

        /// <summary>
        /// 获取或设置是否严格版本检查
        /// <para>默认值：false</para>
        /// </summary>
        public bool StrictVersionCheck
        {
            get => _strictVersionCheck;
            set { ThrowIfReadOnly(); _strictVersionCheck = value; }
        }

        /// <summary>
        /// 获取或设置最小支持版本
        /// </summary>
        public Version MinSupportedVersion
        {
            get => _minSupportedVersion;
            set { ThrowIfReadOnly(); _minSupportedVersion = value; }
        }

        #endregion

        #region 属性 - 多态支持

        /// <summary>
        /// 获取或设置是否启用多态支持
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnablePolymorphism
        {
            get => _enablePolymorphism;
            set { ThrowIfReadOnly(); _enablePolymorphism = value; }
        }

        /// <summary>
        /// 获取或设置是否包含类型信息
        /// <para>默认值：false</para>
        /// </summary>
        public bool IncludeTypeInfo
        {
            get => _includeTypeInfo;
            set { ThrowIfReadOnly(); _includeTypeInfo = value; }
        }

        /// <summary>
        /// 获取或设置类型名称处理方式
        /// <para>默认值：<see cref="TypeNameHandling.Auto"/></para>
        /// </summary>
        public TypeNameHandling TypeNameHandling
        {
            get => _typeNameHandling;
            set { ThrowIfReadOnly(); _typeNameHandling = value; }
        }

        #endregion

        #region 属性 - 循环引用

        /// <summary>
        /// 获取或设置是否启用循环引用处理
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableCircularReference
        {
            get => _enableCircularReference;
            set { ThrowIfReadOnly(); _enableCircularReference = value; }
        }

        /// <summary>
        /// 获取或设置最大引用深度
        /// <para>默认值：64</para>
        /// </summary>
        public int MaxReferenceDepth
        {
            get => _maxReferenceDepth;
            set { ThrowIfReadOnly(); _maxReferenceDepth = Math.Max(1, value); }
        }

        #endregion

        #region 属性 - 压缩配置

        /// <summary>
        /// 获取或设置是否启用压缩
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableCompression
        {
            get => _enableCompression;
            set { ThrowIfReadOnly(); _enableCompression = value; }
        }

        /// <summary>
        /// 获取或设置压缩算法
        /// <para>默认值：<see cref="CompressionAlgorithm.None"/></para>
        /// </summary>
        public CompressionAlgorithm CompressionAlgorithm
        {
            get => _compressionAlgorithm;
            set { ThrowIfReadOnly(); _compressionAlgorithm = value; }
        }

        /// <summary>
        /// 获取或设置压缩级别
        /// <para>默认值：<see cref="CompressionLevel.Optimal"/></para>
        /// </summary>
        public CompressionLevel CompressionLevel
        {
            get => _compressionLevel;
            set { ThrowIfReadOnly(); _compressionLevel = value; }
        }

        /// <summary>
        /// 获取或设置压缩阈值（字节）
        /// <para>默认值：256</para>
        /// </summary>
        public int CompressionThreshold
        {
            get => _compressionThreshold;
            set { ThrowIfReadOnly(); _compressionThreshold = Math.Max(0, value); }
        }

        #endregion

        #region 属性 - 加密配置

        /// <summary>
        /// 获取或设置是否启用加密
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableEncryption
        {
            get => _enableEncryption;
            set { ThrowIfReadOnly(); _enableEncryption = value; }
        }

        /// <summary>
        /// 获取或设置加密算法
        /// <para>默认值：<see cref="EncryptionAlgorithm.None"/></para>
        /// </summary>
        public EncryptionAlgorithm EncryptionAlgorithm
        {
            get => _encryptionAlgorithm;
            set { ThrowIfReadOnly(); _encryptionAlgorithm = value; }
        }

        #endregion

        #region 属性 - 校验配置

        /// <summary>
        /// 获取或设置是否启用校验和
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableChecksum
        {
            get => _enableChecksum;
            set { ThrowIfReadOnly(); _enableChecksum = value; }
        }

        /// <summary>
        /// 获取或设置校验算法
        /// <para>默认值：<see cref="ChecksumAlgorithm.None"/></para>
        /// </summary>
        public ChecksumAlgorithm ChecksumAlgorithm
        {
            get => _checksumAlgorithm;
            set { ThrowIfReadOnly(); _checksumAlgorithm = value; }
        }

        #endregion

        #region 属性 - 性能配置

        /// <summary>
        /// 获取或设置是否使用池化缓冲区
        /// <para>默认值：true</para>
        /// </summary>
        public bool UsePooledBuffers
        {
            get => _usePooledBuffers;
            set { ThrowIfReadOnly(); _usePooledBuffers = value; }
        }

        /// <summary>
        /// 获取或设置初始缓冲区大小
        /// <para>默认值：4096</para>
        /// </summary>
        public int InitialBufferSize
        {
            get => _initialBufferSize;
            set { ThrowIfReadOnly(); _initialBufferSize = Math.Max(64, value); }
        }

        /// <summary>
        /// 获取或设置最大缓冲区大小
        /// <para>默认值：1MB</para>
        /// </summary>
        public int MaxBufferSize
        {
            get => _maxBufferSize;
            set { ThrowIfReadOnly(); _maxBufferSize = Math.Max(_initialBufferSize, value); }
        }

        /// <summary>
        /// 获取或设置是否启用并行处理
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableParallelProcessing
        {
            get => _enableParallelProcessing;
            set { ThrowIfReadOnly(); _enableParallelProcessing = value; }
        }

        #endregion

        #region 属性 - 回调配置

        /// <summary>
        /// 获取或设置是否调用回调方法
        /// <para>默认值：true</para>
        /// </summary>
        public bool InvokeCallbacks
        {
            get => _invokeCallbacks;
            set { ThrowIfReadOnly(); _invokeCallbacks = value; }
        }

        /// <summary>
        /// 获取或设置回调错误时是否继续
        /// <para>默认值：false</para>
        /// </summary>
        public bool ContinueOnCallbackError
        {
            get => _continueOnCallbackError;
            set { ThrowIfReadOnly(); _continueOnCallbackError = value; }
        }

        #endregion

        #region 属性 - 调试配置

        /// <summary>
        /// 获取或设置是否启用诊断
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableDiagnostics
        {
            get => _enableDiagnostics;
            set { ThrowIfReadOnly(); _enableDiagnostics = value; }
        }

        /// <summary>
        /// 获取或设置错误时是否抛出异常
        /// <para>默认值：true</para>
        /// </summary>
        public bool ThrowOnError
        {
            get => _throwOnError;
            set { ThrowIfReadOnly(); _throwOnError = value; }
        }

        #endregion

        #region 公共方法

        /// <inheritdoc/>
        public override string GetSummary() =>
            $"Serializer[Mode={_mode}, Layout={_layout}, Compress={_enableCompression}]";

        #endregion

        #region 保护方法

        /// <inheritdoc/>
        protected override bool OnValidate(ref string error)
        {
            if (_initialBufferSize > _maxBufferSize)
            {
                error = "初始缓冲区大小不能大于最大缓冲区大小";
                return false;
            }

            if (_enableEncryption && _encryptionAlgorithm == EncryptionAlgorithm.None)
            {
                error = "启用加密时必须指定加密算法";
                return false;
            }

            if (_enableCompression && _compressionAlgorithm == CompressionAlgorithm.None)
            {
                error = "启用压缩时必须指定压缩算法";
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void CopyTo(SerializerConfiguration target)
        {
            target._mode = _mode;
            target._layout = _layout;
            target._endianness = _endianness;
            target._defaultStringEncoding = _defaultStringEncoding;
            target._enableVersioning = _enableVersioning;
            target._preserveUnknownFields = _preserveUnknownFields;
            target._strictVersionCheck = _strictVersionCheck;
            target._minSupportedVersion = _minSupportedVersion;
            target._enablePolymorphism = _enablePolymorphism;
            target._includeTypeInfo = _includeTypeInfo;
            target._typeNameHandling = _typeNameHandling;
            target._enableCircularReference = _enableCircularReference;
            target._maxReferenceDepth = _maxReferenceDepth;
            target._enableCompression = _enableCompression;
            target._compressionAlgorithm = _compressionAlgorithm;
            target._compressionLevel = _compressionLevel;
            target._compressionThreshold = _compressionThreshold;
            target._enableEncryption = _enableEncryption;
            target._encryptionAlgorithm = _encryptionAlgorithm;
            target._enableChecksum = _enableChecksum;
            target._checksumAlgorithm = _checksumAlgorithm;
            target._usePooledBuffers = _usePooledBuffers;
            target._initialBufferSize = _initialBufferSize;
            target._maxBufferSize = _maxBufferSize;
            target._enableParallelProcessing = _enableParallelProcessing;
            target._invokeCallbacks = _invokeCallbacks;
            target._continueOnCallbackError = _continueOnCallbackError;
            target._enableDiagnostics = _enableDiagnostics;
            target._throwOnError = _throwOnError;
        }

        #endregion
    }
}
