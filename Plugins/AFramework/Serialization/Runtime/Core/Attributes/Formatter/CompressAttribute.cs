// ==========================================================
// 文件名：CompressAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 压缩标记特性
    /// <para>指定字段或类型在序列化时进行数据压缩</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>为特定字段或类型启用数据压缩</item>
    ///   <item>支持多种压缩算法（LZ4、Brotli、Zstd、Gzip）</item>
    ///   <item>支持压缩级别配置</item>
    ///   <item>支持最小压缩阈值</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class GameSave
    /// {
    ///     // 使用默认压缩（LZ4）
    ///     [Compress]
    ///     public byte[] LargeData;
    ///     
    ///     // 使用 Brotli 高压缩率
    ///     [Compress(CompressionAlgorithm.Brotli, CompressionLevel.SmallestSize)]
    ///     public string CompressedText;
    ///     
    ///     // 仅压缩大于 1KB 的数据
    ///     [Compress(MinSizeToCompress = 1024)]
    ///     public byte[] OptionalCompress;
    /// }
    /// </code>
    /// 
    /// <para><b>压缩算法对比：</b></para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>算法</term>
    ///     <description>压缩率 | 速度 | 适用场景</description>
    ///   </listheader>
    ///   <item><term>LZ4</term><description>中等 | 极快 | 实时数据、网络传输</description></item>
    ///   <item><term>Brotli</term><description>高 | 慢 | 静态资源、存档</description></item>
    ///   <item><term>Zstd</term><description>高 | 快 | 通用场景</description></item>
    ///   <item><term>Gzip</term><description>中等 | 中等 | 兼容性要求高</description></item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class CompressAttribute : Attribute
    {
        #region 常量

        /// <summary>默认最小压缩大小（256 字节）</summary>
        public const int DefaultMinSizeToCompress = 256;

        /// <summary>无最小大小限制</summary>
        public const int NoMinSize = 0;

        /// <summary>压缩头魔数</summary>
        public const uint CompressionMagic = 0x435A4950; // "CZIP"

        #endregion

        #region 字段

        private CompressionAlgorithm _algorithm = CompressionAlgorithm.LZ4;
        private CompressionLevel _level = CompressionLevel.Optimal;
        private int _minSizeToCompress = DefaultMinSizeToCompress;
        private bool _includeHeader = true;
        private bool _storeOriginalSize = true;
        private bool _enableChecksum;
        private float _maxCompressionRatio = 0.95f;
        private bool _allowSkipCompression = true;
        private int _bufferSize = 65536;
        private bool _useStreaming;
        private int _dictionarySize;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置压缩算法
        /// <para>默认值：<see cref="CompressionAlgorithm.LZ4"/></para>
        /// </summary>
        public CompressionAlgorithm Algorithm
        {
            get => _algorithm;
            set => _algorithm = value;
        }

        /// <summary>
        /// 获取或设置压缩级别
        /// <para>默认值：<see cref="CompressionLevel.Optimal"/></para>
        /// </summary>
        public CompressionLevel Level
        {
            get => _level;
            set => _level = value;
        }

        /// <summary>
        /// 获取或设置最小压缩大小（字节）
        /// <para>默认值：256</para>
        /// </summary>
        public int MinSizeToCompress
        {
            get => _minSizeToCompress;
            set => _minSizeToCompress = value;
        }

        /// <summary>
        /// 获取或设置是否包含压缩头
        /// <para>默认值：true</para>
        /// </summary>
        public bool IncludeHeader
        {
            get => _includeHeader;
            set => _includeHeader = value;
        }

        /// <summary>
        /// 获取或设置是否存储原始大小
        /// <para>默认值：true</para>
        /// </summary>
        public bool StoreOriginalSize
        {
            get => _storeOriginalSize;
            set => _storeOriginalSize = value;
        }

        /// <summary>
        /// 获取或设置是否启用校验和
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableChecksum
        {
            get => _enableChecksum;
            set => _enableChecksum = value;
        }

        /// <summary>
        /// 获取或设置最大压缩比阈值
        /// <para>默认值：0.95</para>
        /// </summary>
        public float MaxCompressionRatio
        {
            get => _maxCompressionRatio;
            set => _maxCompressionRatio = Math.Clamp(value, 0.1f, 1.0f);
        }

        /// <summary>
        /// 获取或设置是否允许跳过压缩
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowSkipCompression
        {
            get => _allowSkipCompression;
            set => _allowSkipCompression = value;
        }

        /// <summary>
        /// 获取或设置压缩缓冲区大小
        /// <para>默认值：65536（64KB）</para>
        /// </summary>
        public int BufferSize
        {
            get => _bufferSize;
            set => _bufferSize = Math.Max(1024, value);
        }

        /// <summary>
        /// 获取或设置是否使用流式压缩
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseStreaming
        {
            get => _useStreaming;
            set => _useStreaming = value;
        }

        /// <summary>
        /// 获取或设置字典大小（仅用于 Zstd）
        /// <para>默认值：0（使用默认字典）</para>
        /// </summary>
        public int DictionarySize
        {
            get => _dictionarySize;
            set => _dictionarySize = value;
        }

        #endregion

        #region 构造函数

        /// <summary>初始化 <see cref="CompressAttribute"/> 的新实例</summary>
        public CompressAttribute() { }

        /// <summary>初始化 <see cref="CompressAttribute"/> 的新实例</summary>
        /// <param name="algorithm">压缩算法</param>
        public CompressAttribute(CompressionAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        /// <summary>初始化 <see cref="CompressAttribute"/> 的新实例</summary>
        /// <param name="algorithm">压缩算法</param>
        /// <param name="level">压缩级别</param>
        public CompressAttribute(CompressionAlgorithm algorithm, CompressionLevel level)
        {
            _algorithm = algorithm;
            _level = level;
        }

        #endregion

        #region 公共方法

        /// <summary>检查数据是否应该被压缩</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldCompress(int dataSize) => dataSize >= _minSizeToCompress;

        /// <summary>检查压缩结果是否有效</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompressionEffective(int originalSize, int compressedSize)
        {
            if (!_allowSkipCompression) return true;
            return compressedSize < originalSize * _maxCompressionRatio;
        }

        /// <summary>获取压缩头大小</summary>
        public int GetHeaderSize()
        {
            if (!_includeHeader) return 0;

            var size = 4 + 1 + 1; // 魔数 + 算法标识 + 标志位
            if (_storeOriginalSize) size += 4;
            if (_enableChecksum) size += 4;
            return size;
        }

        /// <summary>估算压缩后大小</summary>
        public int EstimateCompressedSize(int originalSize)
        {
            var ratio = _algorithm switch
            {
                CompressionAlgorithm.LZ4 => 0.6f,
                CompressionAlgorithm.Brotli => 0.3f,
                CompressionAlgorithm.Zstd => 0.4f,
                CompressionAlgorithm.Gzip => 0.5f,
                CompressionAlgorithm.Deflate => 0.5f,
                _ => 0.7f
            };

            ratio = _level switch
            {
                CompressionLevel.Fastest => ratio * 1.3f,
                CompressionLevel.Fast => ratio * 1.1f,
                CompressionLevel.SmallestSize => ratio * 0.8f,
                _ => ratio
            };

            return (int)(originalSize * ratio) + GetHeaderSize();
        }

        /// <summary>获取配置摘要信息</summary>
        public string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { _algorithm.ToString() };

            if (_level != CompressionLevel.Optimal) parts.Add(_level.ToString());
            if (_minSizeToCompress != DefaultMinSizeToCompress) parts.Add($"MinSize={_minSizeToCompress}");
            if (!_includeHeader) parts.Add("NoHeader");
            if (_enableChecksum) parts.Add("Checksum");
            if (_useStreaming) parts.Add("Streaming");

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _level != CompressionLevel.Optimal
                ? $"[Compress({_algorithm}, {_level})]"
                : $"[Compress({_algorithm})]";
        }

        #endregion
    }
}
