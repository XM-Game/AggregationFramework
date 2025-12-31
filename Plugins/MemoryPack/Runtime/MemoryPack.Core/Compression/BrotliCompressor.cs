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
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using System.Runtime.InteropServices;

namespace MemoryPack.Compression {

#if !NET7_0_OR_GREATER
#pragma warning disable CS8602
#endif

/// <summary>
/// Brotli压缩/解压缩器
/// </summary>
public
#if NET7_0_OR_GREATER
    struct
#else
    class
#endif
    BrotliCompressor : IBufferWriter<byte>, IDisposable
{
    ReusableLinkedArrayBufferWriter? bufferWriter;
    readonly int quality;
    readonly int window;

#if NET7_0_OR_GREATER

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public BrotliCompressor()
        : this(CompressionLevel.Fastest)
    {

    }

#endif

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    public BrotliCompressor(CompressionLevel compressionLevel)
        : this(BrotliUtils.GetQualityFromCompressionLevel(compressionLevel), BrotliUtils.WindowBits_Default)
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="window">窗口大小</param>
    public BrotliCompressor(CompressionLevel compressionLevel, int window)
        : this(BrotliUtils.GetQualityFromCompressionLevel(compressionLevel), window)
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="quality">质量</param>
    /// <param name="window">窗口大小</param>
    public BrotliCompressor(int quality = 1, int window = 22)
    {
        this.bufferWriter = ReusableLinkedArrayBufferWriterPool.Rent();
        this.quality = quality;
        this.window = window;
    }

    /// <summary>
    /// 前进
    /// </summary>
    /// <param name="count">前进的距离</param>
    void IBufferWriter<byte>.Advance(int count)
    {
        ThrowIfDisposed();
        bufferWriter.Advance(count);
    }

    /// <summary>
    /// 获取内存
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>内存</returns>
    Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
    {
        ThrowIfDisposed();
        return bufferWriter.GetMemory(sizeHint);
    }

    /// <summary>
    /// 获取Span
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>Span</returns>
    Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
    {
        ThrowIfDisposed();
        return bufferWriter.GetSpan(sizeHint);
    }

    /// <summary>
    /// 转换为字节数组
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] ToArray()
    {
        ThrowIfDisposed();

        using var encoder = new BrotliEncoder(quality, window);

        var maxLength = BrotliUtils.BrotliEncoderMaxCompressedSize(bufferWriter.TotalWritten);

        // 分配内存
        var finalBuffer = ArrayPool<byte>.Shared.Rent(maxLength);
        try
        {
            // 写入的计数
            var writtenCount = 0;
            // 目标Span
            var destination = finalBuffer.AsSpan(0, maxLength);
            foreach (var source in bufferWriter)
            {
                var status = encoder.Compress(source.Span, destination, out var bytesConsumed, out var bytesWritten, isFinalBlock: false);
                if (status != OperationStatus.Done)
                {
                    MemoryPackSerializationException.ThrowCompressionFailed(status);
                }

                if (bytesConsumed != source.Span.Length)
                {
                    MemoryPackSerializationException.ThrowCompressionFailed();
                }

                if (bytesWritten > 0)
                {
                    destination = destination.Slice(bytesWritten);
                    writtenCount += bytesWritten;
                }
            }

            // 完成压缩
            var finalStatus = encoder.Compress(ReadOnlySpan<byte>.Empty, destination, out var consumed, out var written, isFinalBlock: true);
            writtenCount += written;

            return finalBuffer.AsSpan(0, writtenCount).ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(finalBuffer);
        }
    }

    /// <summary>
    /// 复制到目标
    /// </summary>
    /// <param name="destBufferWriter">目标</param>
    public void CopyTo(in IBufferWriter<byte> destBufferWriter)
        
    {
        ThrowIfDisposed();

        var encoder = new BrotliEncoder(quality, window);
        try
        {
            var writtenNotAdvanced = 0;
            foreach (var item in bufferWriter)
            {
                writtenNotAdvanced = CompressCore(ref encoder, item.Span, ref Unsafe.AsRef(destBufferWriter), initialLength: null, isFinalBlock: false);
            }

            // call BrotliEncoderOperation.Finish
            var finalBlockLength = (writtenNotAdvanced == 0) ? null : (int?)(writtenNotAdvanced + 10);
            CompressCore(ref encoder, ReadOnlySpan<byte>.Empty, ref Unsafe.AsRef(destBufferWriter), initialLength: finalBlockLength, isFinalBlock: true);
        }
        finally
        {
            encoder.Dispose();
        }
    }

    /// <summary>
    /// 异步复制到目标
    /// </summary>
    /// <param name="stream">目标流</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>值任务</returns>
    public async ValueTask CopyToAsync(Stream stream, int bufferSize = 65535, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        using var encoder = new BrotliEncoder(quality, window);

        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            foreach (var item in bufferWriter)
            {
                var source = item;
                var lastResult = OperationStatus.DestinationTooSmall;
                while (lastResult == OperationStatus.DestinationTooSmall)
                {
                    lastResult = encoder.Compress(source.Span, buffer, out int bytesConsumed, out int bytesWritten, isFinalBlock: false);
                    if (lastResult == OperationStatus.InvalidData) MemoryPackSerializationException.ThrowCompressionFailed();
                    if (bytesWritten > 0)
                    {
                        await stream.WriteAsync(buffer.AsMemory(0, bytesWritten), cancellationToken).ConfigureAwait(false);
                    }
                    if (bytesConsumed > 0)
                    {
                        source = source.Slice(bytesConsumed);
                    }
                }
            }

            // call BrotliEncoderOperation.Finish
            var finalStatus = encoder.Compress(ReadOnlySpan<byte>.Empty, buffer, out var consumed, out var written, isFinalBlock: true);
            if (written > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, written), cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// 复制到MemoryPackWriter
    /// </summary>
    /// <param name="memoryPackWriter">MemoryPackWriter</param>
    public void CopyTo(ref MemoryPackWriter memoryPackWriter)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        ThrowIfDisposed();

        var encoder = new BrotliEncoder(quality, window);
        try
        {
            var writtenNotAdvanced = 0;
            foreach (var item in bufferWriter)
            {
                writtenNotAdvanced = CompressCore(ref encoder, item.Span, ref memoryPackWriter, initialLength: null, isFinalBlock: false);
            }

            // call BrotliEncoderOperation.Finish
            var finalBlockLength = (writtenNotAdvanced == 0) ? null : (int?)(writtenNotAdvanced + 10);
            CompressCore(ref encoder, ReadOnlySpan<byte>.Empty, ref memoryPackWriter, initialLength: finalBlockLength, isFinalBlock: true);
        }
        finally
        {
            encoder.Dispose();
        }
    }

    /// <summary>
    /// 压缩核心
    /// </summary>
    /// <param name="encoder">BrotliEncoder</param>
    /// <param name="source">源数据</param>
    /// <param name="destBufferWriter">目标</param>
    /// <param name="initialLength">初始长度</param>
    /// <param name="isFinalBlock">是否是最终块</param>
    static int CompressCore(ref BrotliEncoder encoder, ReadOnlySpan<byte> source, ref IBufferWriter<byte> destBufferWriter, int? initialLength, bool isFinalBlock)
        
    {
        var writtenNotAdvanced = 0;

        var lastResult = OperationStatus.DestinationTooSmall;
        while (lastResult == OperationStatus.DestinationTooSmall)
        {
            var dest = destBufferWriter.GetSpan(initialLength ?? source.Length);

            lastResult = encoder.Compress(source, dest, out int bytesConsumed, out int bytesWritten, isFinalBlock: isFinalBlock);
            writtenNotAdvanced += bytesConsumed;

            if (lastResult == OperationStatus.InvalidData) MemoryPackSerializationException.ThrowCompressionFailed();
            if (bytesWritten > 0)
            {
                destBufferWriter.Advance(bytesWritten);
                writtenNotAdvanced = 0;
            }
            if (bytesConsumed > 0)
            {
                source = source.Slice(bytesConsumed);
            }
        }

        return writtenNotAdvanced;
    }

    static int CompressCore(ref BrotliEncoder encoder, ReadOnlySpan<byte> source, ref MemoryPackWriter destBufferWriter, int? initialLength, bool isFinalBlock)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        var writtenNotAdvanced = 0;

        var lastResult = OperationStatus.DestinationTooSmall;
        while (lastResult == OperationStatus.DestinationTooSmall)
        {
            ref var spanRef = ref destBufferWriter.GetSpanReference(initialLength ?? source.Length);
            var dest = MemoryMarshal.CreateSpan(ref spanRef, destBufferWriter.BufferLength);

            lastResult = encoder.Compress(source, dest, out int bytesConsumed, out int bytesWritten, isFinalBlock: isFinalBlock);
            writtenNotAdvanced += bytesConsumed;

            if (lastResult == OperationStatus.InvalidData) MemoryPackSerializationException.ThrowCompressionFailed();
            if (bytesWritten > 0)
            {
                destBufferWriter.Advance(bytesWritten);
                writtenNotAdvanced = 0;
            }
            if (bytesConsumed > 0)
            {
                source = source.Slice(bytesConsumed);
            }
        }

        return writtenNotAdvanced;
    }

    public void Dispose()
    {
        if (bufferWriter == null) return;

        bufferWriter.Reset();
        ReusableLinkedArrayBufferWriterPool.Return(bufferWriter);
        bufferWriter = null!;
    }

#if NET7_0_OR_GREATER
    [MemberNotNull(nameof(bufferWriter))]
#endif
    void ThrowIfDisposed()
    {
        if (bufferWriter == null)
        {
            throw new ObjectDisposedException(null);
        }
    }
}

/// <summary>
/// Brotli工具类
/// </summary>
internal static partial class BrotliUtils
{
    /// <summary>
    /// 窗口大小最小值
    /// </summary>
    public const int WindowBits_Min = 10;
    /// <summary>
    /// 窗口大小默认值
    /// </summary>
    public const int WindowBits_Default = 22;
    /// <summary>
    /// 窗口大小最大值
    /// </summary>
    public const int WindowBits_Max = 24;
    /// <summary>
    /// 质量最小值
    /// </summary>
    public const int Quality_Min = 0;
    /// <summary>
    /// 质量默认值
    /// </summary>
    public const int Quality_Default = 4;
    /// <summary>
    /// 质量最大值
    /// </summary>
    public const int Quality_Max = 11;
    /// <summary>
    /// 最大输入大小
    /// </summary>
    public const int MaxInputSize = int.MaxValue - 515; // 515 is the max compressed extra bytes

    /// <summary>
    /// 从压缩级别获取质量
    /// </summary>
    /// <param name="compressionLevel">压缩级别</param>
    /// <returns>质量</returns>
    internal static int GetQualityFromCompressionLevel(CompressionLevel compressionLevel) =>
        compressionLevel switch
        {
            CompressionLevel.NoCompression => Quality_Min,
            CompressionLevel.Fastest => 1,
            CompressionLevel.Optimal => Quality_Default,
#if NET7_0_OR_GREATER
            CompressionLevel.SmallestSize => Quality_Max,
#endif
            _ => throw new ArgumentException()
        };


    // https://github.com/dotnet/runtime/issues/35142
    // BrotliEncoder.GetMaxCompressedLength is broken in .NET 7
    // port from encode.c https://github.com/google/brotli/blob/3914999fcc1fda92e750ef9190aa6db9bf7bdb07/c/enc/encode.c#L1200
    /// <summary>
    /// 获取最大压缩大小
    /// </summary>
    /// <param name="input_size">输入大小</param>
    /// <returns>最大压缩大小</returns>
    internal static int BrotliEncoderMaxCompressedSize(int input_size)
    {
        var num_large_blocks = input_size >> 14;
        var overhead = 2 + (4 * num_large_blocks) + 3 + 1;
        var result = input_size + overhead;
        if (input_size == 0) return 2;
        return (result < input_size) ? 0 : result;
    }
}

}