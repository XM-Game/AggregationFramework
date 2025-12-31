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
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using System.Runtime.InteropServices;

namespace MemoryPack {

/// <summary>
/// MemoryPackSerializer 类
/// </summary>
public static partial class MemoryPackSerializer
{


    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>字节数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Serialize(Type type, object? value, MemoryPackSerializerOptions? options = default)
    {
        var state = threadStaticState;
        if (state == null)
        {
            state = threadStaticState = new SerializerWriterThreadStaticState();
        }
        state.Init(options);

        try
        {
            var writer = new MemoryPackWriter(ref Unsafe.As<ReusableLinkedArrayBufferWriter, IBufferWriter<byte>>(ref state.BufferWriter), state.BufferWriter.DangerousGetFirstBuffer(), state.OptionalState);
            Serialize(type, ref writer, value);
            return state.BufferWriter.ToArrayAndReset();
        }
        finally
        {
            state.Reset();
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="bufferWriter">缓冲区写入器</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>void</returns>
    public static unsafe void Serialize(Type type, in IBufferWriter<byte> bufferWriter, object? value, MemoryPackSerializerOptions? options = default)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        var state = threadStaticWriterOptionalState;
        if (state == null)
        {
            state = threadStaticWriterOptionalState = new MemoryPackWriterOptionalState();
        }
        state.Init(options);

        try
        {
            var writer = new MemoryPackWriter(ref Unsafe.AsRef(bufferWriter), state);
            Serialize(type, ref writer, value);
        }
        finally
        {
            state.Reset();
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    /// <returns>void</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Type type, ref MemoryPackWriter writer, object? value)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        writer.GetFormatter(type).Serialize(ref writer, ref value);
        writer.Flush();
    }

    /// <summary>
    /// 异步序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="stream">流</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static async ValueTask SerializeAsync(Type type, Stream stream, object? value, MemoryPackSerializerOptions? options = default, CancellationToken cancellationToken = default)
    {
        var tempWriter = ReusableLinkedArrayBufferWriterPool.Rent();
        try
        {
            SerializeToTempWriter(tempWriter, type, value, options);
            await tempWriter.WriteToAndResetAsync(stream, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableLinkedArrayBufferWriterPool.Return(tempWriter);
        }
    }

    /// <summary>
    /// 序列化到临时写入器
    /// </summary>
    /// <param name="bufferWriter">缓冲区写入器</param>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>void</returns>
    static void SerializeToTempWriter(ReusableLinkedArrayBufferWriter bufferWriter, Type type, object? value, MemoryPackSerializerOptions? options)
    {
        var state = threadStaticWriterOptionalState;
        if (state == null)
        {
            state = threadStaticWriterOptionalState = new MemoryPackWriterOptionalState();
        }
        state.Init(options);

        var writer = new MemoryPackWriter(ref Unsafe.As<ReusableLinkedArrayBufferWriter, IBufferWriter<byte>>(ref bufferWriter), state);

        try
        {
            Serialize(type, ref writer, value);
        }
        finally
        {
            state.Reset();
        }
    }

    // Deserialize
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="options">选项</param>
    /// <returns>值</returns>
    public static object? Deserialize(Type type, ReadOnlySpan<byte> buffer, MemoryPackSerializerOptions? options = default)
    {
        object? value = default;
        Deserialize(type, buffer, ref value, options);
        return value;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>长度</returns>
    public static int Deserialize(Type type, ReadOnlySpan<byte> buffer, ref object? value, MemoryPackSerializerOptions? options = default)
    {
        var state = threadStaticReaderOptionalState;
        if (state == null)
        {
            state = threadStaticReaderOptionalState = new MemoryPackReaderOptionalState();
        }
        state.Init(options);

        var reader = new MemoryPackReader(buffer, state);
        try
        {
            reader.GetFormatter(type).Deserialize(ref reader, ref value);
            return reader.Consumed;
        }
        finally
        {
            reader.Dispose();
            state.Reset();
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="options">选项</param>
    /// <returns>值</returns>
    public static object? Deserialize(Type type, in ReadOnlySequence<byte> buffer, MemoryPackSerializerOptions? options = default)
    {
        object? value = default;
        Deserialize(type, buffer, ref value, options);
        return value;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>长度</returns>
    public static int Deserialize(Type type, in ReadOnlySequence<byte> buffer, ref object? value, MemoryPackSerializerOptions? options = default)
    {
        var state = threadStaticReaderOptionalState;
        if (state == null)
        {
            state = threadStaticReaderOptionalState = new MemoryPackReaderOptionalState();
        }
        state.Init(options);

        var reader = new MemoryPackReader(buffer, state);
        try
        {
            reader.GetFormatter(type).Deserialize(ref reader, ref value);
            return reader.Consumed;
        }
        finally
        {
            reader.Dispose();
            state.Reset();
        }
    }

    /// <summary>
    /// 异步反序列化
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="stream">流</param>
    /// <param name="options">选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>值</returns>
    public static async ValueTask<object?> DeserializeAsync(Type type, Stream stream, MemoryPackSerializerOptions? options = default, CancellationToken cancellationToken = default)
    {
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> streamBuffer))
        {
            cancellationToken.ThrowIfCancellationRequested();
            object? value = default;
            var bytesRead = Deserialize(type, streamBuffer.AsSpan(checked((int)ms.Position)), ref value, options);

            // Emulate that we had actually "read" from the stream.
            ms.Seek(bytesRead, SeekOrigin.Current);

            return value;
        }

        // 创建可重用只读序列构建器
        var builder = ReusableReadOnlySequenceBuilderPool.Rent();
        try
        {
            // 租用64K的缓冲区
            var buffer = ArrayPool<byte>.Shared.Rent(65536); // initial 64K
            var offset = 0;
            do
            {
                // 如果偏移量等于缓冲区长度，则添加缓冲区并返回池
                if (offset == buffer.Length)
                {
                    builder.Add(buffer, returnToPool: true);
                    // 租用新的缓冲区
                    buffer = ArrayPool<byte>.Shared.Rent(MathEx.NewArrayCapacity(buffer.Length));
                    offset = 0;
                }

                // 读取数据
                int read = 0;
                try
                {
                    read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // 缓冲区没有添加到构建器，所以返回这里。
                    ArrayPool<byte>.Shared.Return(buffer);
                    throw;
                }

                // 增加偏移量
                offset += read;

                if (read == 0)
                {
                    // 添加缓冲区并返回池
                    builder.Add(buffer.AsMemory(0, offset), returnToPool: true);
                    break;

                }
            } while (true);

            // 如果只有一个缓冲区，则可以避免ReadOnlySequence构建成本
            if (builder.TryGetSingleMemory(out var memory))
            {
                // 反序列化
                return Deserialize(type, memory.Span, options);
            }
            else
            {
                // 构建只读序列
                var seq = builder.Build();
                // 反序列化
                var result = Deserialize(type, seq, options);
                return result;
            }
        }
        finally
        {
            // 重置构建器
            builder.Reset();
        }
    }
}

}