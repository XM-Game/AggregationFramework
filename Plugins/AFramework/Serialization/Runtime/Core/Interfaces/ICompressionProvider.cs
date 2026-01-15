// ==========================================================
// 文件名：ICompressionProvider.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 压缩提供者接口
    /// <para>定义数据压缩和解压缩的操作</para>
    /// <para>支持多种压缩算法的统一抽象</para>
    /// </summary>
    /// <remarks>
    /// 压缩提供者用于在序列化后对数据进行压缩，减少存储和传输大小。
    /// 
    /// 使用示例:
    /// <code>
    /// ICompressionProvider compressor = new LZ4CompressionProvider();
    /// 
    /// // 压缩数据
    /// byte[] compressed = compressor.Compress(originalData);
    /// 
    /// // 解压数据
    /// byte[] decompressed = compressor.Decompress(compressed);
    /// 
    /// // 使用压缩级别
    /// byte[] highCompressed = compressor.Compress(data, CompressionLevel.SmallestSize);
    /// </code>
    /// </remarks>
    public interface ICompressionProvider
    {
        /// <summary>
        /// 获取压缩算法类型
        /// </summary>
        CompressionAlgorithm Algorithm { get; }

        /// <summary>
        /// 获取默认压缩级别
        /// </summary>
        CompressionLevel DefaultLevel { get; }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>压缩后的数据</returns>
        byte[] Compress(ReadOnlySpan<byte> source);

        /// <summary>
        /// 压缩数据 (指定压缩级别)
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="level">压缩级别</param>
        /// <returns>压缩后的数据</returns>
        byte[] Compress(ReadOnlySpan<byte> source, CompressionLevel level);

        /// <summary>
        /// 压缩数据到目标缓冲区
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="destination">目标缓冲区</param>
        /// <param name="level">压缩级别</param>
        /// <returns>压缩后的字节数</returns>
        int Compress(ReadOnlySpan<byte> source, Span<byte> destination, CompressionLevel level = default);

        /// <summary>
        /// 尝试压缩数据到目标缓冲区
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="destination">目标缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <param name="level">压缩级别</param>
        /// <returns>如果成功返回 true</returns>
        bool TryCompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten, CompressionLevel level = default);

        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="source">压缩数据</param>
        /// <returns>解压后的数据</returns>
        byte[] Decompress(ReadOnlySpan<byte> source);

        /// <summary>
        /// 解压数据 (已知原始大小)
        /// </summary>
        /// <param name="source">压缩数据</param>
        /// <param name="originalSize">原始数据大小</param>
        /// <returns>解压后的数据</returns>
        byte[] Decompress(ReadOnlySpan<byte> source, int originalSize);

        /// <summary>
        /// 解压数据到目标缓冲区
        /// </summary>
        /// <param name="source">压缩数据</param>
        /// <param name="destination">目标缓冲区</param>
        /// <returns>解压后的字节数</returns>
        int Decompress(ReadOnlySpan<byte> source, Span<byte> destination);

        /// <summary>
        /// 尝试解压数据到目标缓冲区
        /// </summary>
        /// <param name="source">压缩数据</param>
        /// <param name="destination">目标缓冲区</param>
        /// <param name="bytesWritten">写入的字节数</param>
        /// <returns>如果成功返回 true</returns>
        bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten);

        /// <summary>
        /// 获取压缩后的最大可能大小
        /// </summary>
        /// <param name="sourceSize">源数据大小</param>
        /// <returns>压缩后的最大可能大小</returns>
        int GetMaxCompressedSize(int sourceSize);

        /// <summary>
        /// 检查数据是否已压缩 (通过魔数检测)
        /// </summary>
        /// <param name="data">要检查的数据</param>
        /// <returns>如果已压缩返回 true</returns>
        bool IsCompressed(ReadOnlySpan<byte> data);
    }

    /// <summary>
    /// 流式压缩提供者接口
    /// <para>支持流式压缩和解压缩操作</para>
    /// </summary>
    public interface IStreamingCompressionProvider : ICompressionProvider
    {
        /// <summary>
        /// 创建压缩流
        /// </summary>
        /// <param name="destination">目标流</param>
        /// <param name="level">压缩级别</param>
        /// <returns>压缩流</returns>
        System.IO.Stream CreateCompressionStream(System.IO.Stream destination, CompressionLevel level = default);

        /// <summary>
        /// 创建解压流
        /// </summary>
        /// <param name="source">源流</param>
        /// <returns>解压流</returns>
        System.IO.Stream CreateDecompressionStream(System.IO.Stream source);
    }

    /// <summary>
    /// 压缩结果结构体
    /// </summary>
    public readonly struct CompressionResult
    {
        /// <summary>压缩后的数据</summary>
        public readonly byte[] Data;

        /// <summary>原始大小</summary>
        public readonly int OriginalSize;

        /// <summary>压缩后大小</summary>
        public readonly int CompressedSize;

        /// <summary>压缩率 (0-1，越小压缩效果越好)</summary>
        public float CompressionRatio => OriginalSize > 0 ? (float)CompressedSize / OriginalSize : 1.0f;

        /// <summary>节省的字节数</summary>
        public int BytesSaved => OriginalSize - CompressedSize;

        /// <summary>是否有效压缩 (压缩后更小)</summary>
        public bool IsEffective => CompressedSize < OriginalSize;

        /// <summary>
        /// 创建压缩结果
        /// </summary>
        public CompressionResult(byte[] data, int originalSize, int compressedSize)
        {
            Data = data;
            OriginalSize = originalSize;
            CompressedSize = compressedSize;
        }

        /// <summary>
        /// 创建成功的压缩结果
        /// </summary>
        public static CompressionResult Success(byte[] data, int originalSize)
        {
            return new CompressionResult(data, originalSize, data?.Length ?? 0);
        }

        /// <summary>
        /// 创建失败的压缩结果 (返回原始数据)
        /// </summary>
        public static CompressionResult NoCompression(byte[] originalData)
        {
            return new CompressionResult(originalData, originalData?.Length ?? 0, originalData?.Length ?? 0);
        }
    }
}
