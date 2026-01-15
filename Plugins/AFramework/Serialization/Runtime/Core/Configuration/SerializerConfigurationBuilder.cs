// ==========================================================
// 文件名：SerializerConfigurationBuilder.cs
// 命名空间: AFramework.Serialization
// 依赖: 无
// ==========================================================

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化器配置建造者
    /// <para>使用流畅接口构建 <see cref="SerializerConfiguration"/></para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// var config = SerializerConfigurationBuilder.Create()
    ///     .WithMode(SerializeMode.VersionTolerant)
    ///     .WithCompression(CompressionAlgorithm.LZ4)
    ///     .WithBufferSize(8192, 2 * 1024 * 1024)
    ///     .Build();
    /// </code>
    /// </remarks>
    public sealed class SerializerConfigurationBuilder
    {
        #region 字段

        private readonly SerializerConfiguration _config = new SerializerConfiguration();

        #endregion

        #region 静态方法

        /// <summary>
        /// 创建新的建造者实例
        /// </summary>
        /// <returns>建造者实例</returns>
        public static SerializerConfigurationBuilder Create() => new SerializerConfigurationBuilder();

        #endregion

        #region 基本配置

        /// <summary>
        /// 设置序列化模式
        /// </summary>
        public SerializerConfigurationBuilder WithMode(SerializeMode mode)
        {
            _config.Mode = mode;
            return this;
        }

        /// <summary>
        /// 设置序列化布局
        /// </summary>
        public SerializerConfigurationBuilder WithLayout(SerializeLayout layout)
        {
            _config.Layout = layout;
            return this;
        }

        /// <summary>
        /// 设置字节序
        /// </summary>
        public SerializerConfigurationBuilder WithEndianness(Endianness endianness)
        {
            _config.Endianness = endianness;
            return this;
        }

        /// <summary>
        /// 设置默认字符串编码
        /// </summary>
        public SerializerConfigurationBuilder WithStringEncoding(StringEncoding encoding)
        {
            _config.DefaultStringEncoding = encoding;
            return this;
        }

        #endregion

        #region 压缩配置

        /// <summary>
        /// 启用压缩
        /// </summary>
        /// <param name="algorithm">压缩算法</param>
        /// <param name="level">压缩级别</param>
        public SerializerConfigurationBuilder WithCompression(
            CompressionAlgorithm algorithm,
            CompressionLevel level = CompressionLevel.Optimal)
        {
            _config.EnableCompression = true;
            _config.CompressionAlgorithm = algorithm;
            _config.CompressionLevel = level;
            return this;
        }

        /// <summary>
        /// 设置压缩阈值
        /// </summary>
        /// <param name="threshold">阈值（字节）</param>
        public SerializerConfigurationBuilder WithCompressionThreshold(int threshold)
        {
            _config.CompressionThreshold = threshold;
            return this;
        }

        #endregion

        #region 加密配置

        /// <summary>
        /// 启用加密
        /// </summary>
        /// <param name="algorithm">加密算法</param>
        public SerializerConfigurationBuilder WithEncryption(EncryptionAlgorithm algorithm)
        {
            _config.EnableEncryption = true;
            _config.EncryptionAlgorithm = algorithm;
            return this;
        }

        #endregion

        #region 版本配置

        /// <summary>
        /// 启用版本容错
        /// </summary>
        /// <param name="preserveUnknown">是否保留未知字段</param>
        public SerializerConfigurationBuilder WithVersionTolerance(bool preserveUnknown = true)
        {
            _config.Mode = SerializeMode.VersionTolerant;
            _config.PreserveUnknownFields = preserveUnknown;
            return this;
        }

        /// <summary>
        /// 启用严格版本检查
        /// </summary>
        public SerializerConfigurationBuilder WithStrictVersionCheck()
        {
            _config.StrictVersionCheck = true;
            return this;
        }

        #endregion

        #region 引用配置

        /// <summary>
        /// 启用循环引用处理
        /// </summary>
        /// <param name="maxDepth">最大引用深度</param>
        public SerializerConfigurationBuilder WithCircularReference(int maxDepth = 64)
        {
            _config.EnableCircularReference = true;
            _config.MaxReferenceDepth = maxDepth;
            return this;
        }

        /// <summary>
        /// 启用多态支持
        /// </summary>
        /// <param name="typeNameHandling">类型名称处理方式</param>
        public SerializerConfigurationBuilder WithPolymorphism(TypeNameHandling typeNameHandling = TypeNameHandling.Auto)
        {
            _config.EnablePolymorphism = true;
            _config.TypeNameHandling = typeNameHandling;
            return this;
        }

        #endregion

        #region 缓冲区配置

        /// <summary>
        /// 设置缓冲区大小
        /// </summary>
        /// <param name="initial">初始大小</param>
        /// <param name="max">最大大小</param>
        public SerializerConfigurationBuilder WithBufferSize(int initial, int max)
        {
            _config.InitialBufferSize = initial;
            _config.MaxBufferSize = max;
            return this;
        }

        /// <summary>
        /// 禁用缓冲区池
        /// </summary>
        public SerializerConfigurationBuilder WithoutPooledBuffers()
        {
            _config.UsePooledBuffers = false;
            return this;
        }

        #endregion

        #region 性能配置

        /// <summary>
        /// 启用并行处理
        /// </summary>
        public SerializerConfigurationBuilder WithParallelProcessing()
        {
            _config.EnableParallelProcessing = true;
            return this;
        }

        /// <summary>
        /// 启用诊断
        /// </summary>
        public SerializerConfigurationBuilder WithDiagnostics()
        {
            _config.EnableDiagnostics = true;
            return this;
        }

        #endregion

        #region 构建

        /// <summary>
        /// 构建配置（只读）
        /// </summary>
        /// <returns>只读配置实例</returns>
        public SerializerConfiguration Build()
        {
            return _config.AsReadOnly();
        }

        /// <summary>
        /// 构建配置（可修改）
        /// </summary>
        /// <returns>可修改配置实例</returns>
        public SerializerConfiguration BuildMutable()
        {
            return _config.Clone();
        }

        #endregion
    }
}
