// ==========================================================
// 文件名：CompressionAlgorithm.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 压缩算法枚举
    /// <para>定义序列化数据的压缩算法类型</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用 LZ4 压缩
    /// var options = new SerializeOptions { Compression = CompressionAlgorithm.LZ4 };
    /// 
    /// // 检查算法特性
    /// if (algorithm.IsHighSpeed())
    ///     // 适合实时场景
    /// </code>
    /// </remarks>
    public enum CompressionAlgorithm : byte
    {
        /// <summary>
        /// 无压缩
        /// <para>不对数据进行压缩处理</para>
        /// <para>特点：零开销、原始大小</para>
        /// <para>适用：已压缩数据、小数据、CPU敏感场景</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// LZ4 压缩
        /// <para>极快的压缩/解压速度，中等压缩率</para>
        /// <para>特点：速度优先、实时友好、内存高效</para>
        /// <para>适用：游戏数据、网络通信、实时存储</para>
        /// </summary>
        LZ4 = 1,

        /// <summary>
        /// Brotli 压缩
        /// <para>高压缩率，解压速度快</para>
        /// <para>特点：压缩率高、解压快、压缩较慢</para>
        /// <para>适用：静态资源、长期存储、带宽受限</para>
        /// </summary>
        Brotli = 2,

        /// <summary>
        /// Zstd 压缩 (Zstandard)
        /// <para>平衡的压缩率和速度</para>
        /// <para>特点：可调级别、字典支持、通用性强</para>
        /// <para>适用：通用场景、大数据、流式处理</para>
        /// </summary>
        Zstd = 3,

        /// <summary>
        /// Gzip 压缩
        /// <para>广泛兼容的压缩格式</para>
        /// <para>特点：兼容性好、标准格式、中等性能</para>
        /// <para>适用：Web传输、跨平台、兼容性要求</para>
        /// </summary>
        Gzip = 4,

        /// <summary>
        /// Deflate 压缩
        /// <para>Gzip 的底层算法</para>
        /// <para>特点：无头部开销、轻量级</para>
        /// <para>适用：嵌入式场景、自定义格式</para>
        /// </summary>
        Deflate = 5,

        /// <summary>
        /// Snappy 压缩
        /// <para>Google 开发的快速压缩算法</para>
        /// <para>特点：极快速度、低压缩率</para>
        /// <para>适用：日志系统、临时存储</para>
        /// </summary>
        Snappy = 6,

        /// <summary>
        /// LZMA 压缩
        /// <para>极高压缩率的算法</para>
        /// <para>特点：最高压缩率、速度较慢</para>
        /// <para>适用：归档、分发包、存储空间极限</para>
        /// </summary>
        LZMA = 7,

        /// <summary>
        /// 自动选择
        /// <para>根据数据特征自动选择最佳算法</para>
        /// </summary>
        Auto = 255
    }

    /// <summary>
    /// CompressionAlgorithm 扩展方法
    /// </summary>
    public static class CompressionAlgorithmExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否为高速压缩算法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighSpeed(this CompressionAlgorithm algorithm)
        {
            return algorithm == CompressionAlgorithm.LZ4 ||
                   algorithm == CompressionAlgorithm.Snappy ||
                   algorithm == CompressionAlgorithm.None;
        }

        /// <summary>
        /// 检查是否为高压缩率算法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighRatio(this CompressionAlgorithm algorithm)
        {
            return algorithm == CompressionAlgorithm.LZMA ||
                   algorithm == CompressionAlgorithm.Brotli ||
                   algorithm == CompressionAlgorithm.Zstd;
        }

        /// <summary>
        /// 检查是否已启用压缩
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(this CompressionAlgorithm algorithm)
        {
            return algorithm != CompressionAlgorithm.None;
        }

        /// <summary>
        /// 检查是否支持流式处理
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsStreaming(this CompressionAlgorithm algorithm)
        {
            return algorithm == CompressionAlgorithm.LZ4 ||
                   algorithm == CompressionAlgorithm.Zstd ||
                   algorithm == CompressionAlgorithm.Gzip ||
                   algorithm == CompressionAlgorithm.Deflate ||
                   algorithm == CompressionAlgorithm.Brotli;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取算法的中文描述
        /// </summary>
        public static string GetDescription(this CompressionAlgorithm algorithm)
        {
            return algorithm switch
            {
                CompressionAlgorithm.None => "无压缩",
                CompressionAlgorithm.LZ4 => "LZ4 (极速)",
                CompressionAlgorithm.Brotli => "Brotli (高压缩)",
                CompressionAlgorithm.Zstd => "Zstandard (平衡)",
                CompressionAlgorithm.Gzip => "Gzip (兼容)",
                CompressionAlgorithm.Deflate => "Deflate (轻量)",
                CompressionAlgorithm.Snappy => "Snappy (快速)",
                CompressionAlgorithm.LZMA => "LZMA (极限压缩)",
                CompressionAlgorithm.Auto => "自动选择",
                _ => "未知算法"
            };
        }

        /// <summary>
        /// 获取预估的压缩速度等级 (1-5，5为最快)
        /// </summary>
        public static int GetSpeedLevel(this CompressionAlgorithm algorithm)
        {
            return algorithm switch
            {
                CompressionAlgorithm.None => 5,
                CompressionAlgorithm.LZ4 => 5,
                CompressionAlgorithm.Snappy => 5,
                CompressionAlgorithm.Zstd => 4,
                CompressionAlgorithm.Deflate => 3,
                CompressionAlgorithm.Gzip => 3,
                CompressionAlgorithm.Brotli => 2,
                CompressionAlgorithm.LZMA => 1,
                _ => 3
            };
        }

        /// <summary>
        /// 获取预估的压缩率等级 (1-5，5为最高)
        /// </summary>
        public static int GetRatioLevel(this CompressionAlgorithm algorithm)
        {
            return algorithm switch
            {
                CompressionAlgorithm.None => 0,
                CompressionAlgorithm.Snappy => 2,
                CompressionAlgorithm.LZ4 => 3,
                CompressionAlgorithm.Deflate => 3,
                CompressionAlgorithm.Gzip => 3,
                CompressionAlgorithm.Zstd => 4,
                CompressionAlgorithm.Brotli => 5,
                CompressionAlgorithm.LZMA => 5,
                _ => 3
            };
        }

        /// <summary>
        /// 获取推荐的压缩算法
        /// </summary>
        public static CompressionAlgorithm GetRecommended(bool prioritizeSpeed, bool needsCompatibility)
        {
            if (needsCompatibility) return CompressionAlgorithm.Gzip;
            return prioritizeSpeed ? CompressionAlgorithm.LZ4 : CompressionAlgorithm.Zstd;
        }

        #endregion
    }
}
