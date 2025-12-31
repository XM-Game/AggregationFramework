using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;
using System.Buffers;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using System.Runtime.InteropServices;

namespace MemoryPack.Compression {

/// <summary>
/// Brotli格式化器
/// </summary>
/// <remarks>
/// 序列化 as (uncompressedLength, compressedLength, values...)
/// </remarks>

[Preserve]
public sealed class BrotliFormatter : MemoryPackFormatter<byte[]>
{
    internal const int DefaultDecompssionSizeLimit = 1024 * 1024 * 128; // 128MB

    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly BrotliFormatter Default = new BrotliFormatter();

    readonly System.IO.Compression.CompressionLevel compressionLevel;
    readonly int window;
    readonly int decompressionSizeLimit;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public BrotliFormatter()
        : this(System.IO.Compression.CompressionLevel.Fastest)
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    public BrotliFormatter(System.IO.Compression.CompressionLevel compressionLevel)
        : this(compressionLevel, BrotliUtils.WindowBits_Default)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="window">窗口大小</param>
    public BrotliFormatter(System.IO.Compression.CompressionLevel compressionLevel, int window)
        : this(compressionLevel, window, DefaultDecompssionSizeLimit)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="window">窗口大小</param>
    /// <param name="decompressionSizeLimit">解压缩大小限制</param>
    public BrotliFormatter(System.IO.Compression.CompressionLevel compressionLevel, int window, int decompressionSizeLimit)
    {
        this.compressionLevel = compressionLevel;
        this.window = window;
        this.decompressionSizeLimit = decompressionSizeLimit;
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref byte[]? value)
    {
        if (value == null)
        {
            writer.WriteNullCollectionHeader();
            return;
        }

        if (value.Length == 0)
        {
            writer.WriteCollectionHeader(0);
            return;
        }

        var quality = BrotliUtils.GetQualityFromCompressionLevel(compressionLevel);

        using var encoder = new BrotliEncoder(quality, window);

        var maxLength = BrotliUtils.BrotliEncoderMaxCompressedSize(value.Length);

        ref var head = ref writer.GetSpanReference(maxLength + 8);

        var dest = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref head, 8), maxLength);
        var status = encoder.Compress(value.AsSpan(), dest, out var bytesConsumed, out var bytesWritten, isFinalBlock: true);
        if (status != OperationStatus.Done)
        {
            MemoryPackSerializationException.ThrowCompressionFailed(status);
        }

        if (bytesConsumed != value.Length)
        {
            MemoryPackSerializationException.ThrowCompressionFailed();
        }

        // write to buffer header (uncompressedLength, compressedLength, values...)
        Unsafe.WriteUnaligned(ref head, value.Length);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref head, 4), bytesWritten);

        writer.Advance(bytesWritten + 8);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref byte[]? value)
    {
        var uncompressedLength = reader.ReadUnmanaged<int>();

        reader.DangerousReadUnmanagedSpanView<byte>(out var isNull, out var compressedBuffer);

        if (isNull)
        {
            value = null;
            return;
        }

        if (compressedBuffer.Length == 0)
        {
            value = Array.Empty<byte>();
            return;
        }

        // security, require to check length
        if (decompressionSizeLimit < uncompressedLength)
        {
            MemoryPackSerializationException.ThrowDecompressionSizeLimitExceeded(decompressionSizeLimit, uncompressedLength);
        }

        if (value == null || value.Length != uncompressedLength)
        {
            value = new byte[uncompressedLength];
        }

        using var decoder = new BrotliDecoder();

        var status = decoder.Decompress(compressedBuffer, value, out var bytesConsumed, out var bytesWritten);
        if (status != OperationStatus.Done)
        {
            MemoryPackSerializationException.ThrowCompressionFailed(status);
        }

        if (bytesConsumed != compressedBuffer.Length || bytesWritten != value.Length)
        {
            MemoryPackSerializationException.ThrowCompressionFailed();
        }
    }
}

/// <summary>
/// Brotli格式化器
/// </summary>
/// <remarks>
/// 序列化 as (uncompressedLength, compressedLength, values...)
/// </remarks>
[Preserve]
public sealed class BrotliFormatter<T> : MemoryPackFormatter<T>
{
    internal const int DefaultDecompssionSizeLimit = 1024 * 1024 * 128; // 128MB

    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly BrotliFormatter Default = new BrotliFormatter();

    readonly System.IO.Compression.CompressionLevel compressionLevel;
    readonly int window;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public BrotliFormatter()
        : this(System.IO.Compression.CompressionLevel.Fastest)
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    public BrotliFormatter(System.IO.Compression.CompressionLevel compressionLevel)
        : this(compressionLevel, BrotliUtils.WindowBits_Default)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="window">窗口大小</param>
    public BrotliFormatter(System.IO.Compression.CompressionLevel compressionLevel, int window)
    {
        this.compressionLevel = compressionLevel;
        this.window = window;
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref T? value)
    {
        var compressor = new BrotliCompressor(compressionLevel, window);
        try
        {
            var coWriter = new MemoryPackWriter(ref Unsafe.As<BrotliCompressor, IBufferWriter<byte>>(ref compressor), writer.OptionalState);

            coWriter.WriteValue(value);
            coWriter.Flush();

            compressor.CopyTo(ref writer);
        }
        finally
        {
            compressor.Dispose();
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref T? value)
    {
        using var decompressor = new BrotliDecompressor();

        reader.GetRemainingSource(out var singleSource, out var remainingSource);

        int consumed;
        ReadOnlySequence<byte> decompressedSource;
        if (singleSource.Length != 0)
        {
            decompressedSource = decompressor.Decompress(singleSource, out consumed);
        }
        else
        {
            decompressedSource = decompressor.Decompress(remainingSource, out consumed);
        }

        using var coReader = new MemoryPackReader(decompressedSource, reader.OptionalState);
        coReader.ReadValue(ref value);

        reader.Advance(consumed);
    }
}

}