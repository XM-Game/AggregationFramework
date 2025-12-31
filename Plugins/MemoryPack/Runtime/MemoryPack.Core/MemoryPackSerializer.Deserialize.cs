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
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using System.Runtime.InteropServices;

namespace MemoryPack {
    /// <summary>
    /// MemoryPackSerializer 类
    /// </summary>
public static partial class MemoryPackSerializer
{
    [ThreadStatic]
    static MemoryPackReaderOptionalState? threadStaticReaderOptionalState; // 线程静态读取器可选状态

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="buffer">缓冲区</param>
    /// <param name="options">选项</param>
    /// <returns>值</returns>
    public static T? Deserialize<T>(ReadOnlySpan<byte> buffer, MemoryPackSerializerOptions? options = default)
    {
        T? value = default;
        Deserialize(buffer, ref value, options);
        return value;
    }
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="buffer">缓冲区</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>长度</returns>
    public static int Deserialize<T>(ReadOnlySpan<byte> buffer, ref T? value, MemoryPackSerializerOptions? options = default)
    {
        // 是否是引用或包含引用
        // 如果是，则分配内存
        // 如果不是，则直接复制
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // 如果缓冲区长度小于类型大小，则抛出异常
            if (buffer.Length < Unsafe.SizeOf<T>())
            {
                MemoryPackSerializationException.ThrowInvalidRange(Unsafe.SizeOf<T>(), buffer.Length);
            }
            // 读取值
            value = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(buffer));
            return Unsafe.SizeOf<T>();
        }

        // 获取线程静态读取器可选状态
        var state = threadStaticReaderOptionalState;
        // 如果线程静态读取器可选状态为空，则初始化
        if (state == null)
        {
            // 初始化线程静态读取器可选状态
            state = threadStaticReaderOptionalState = new MemoryPackReaderOptionalState();
        }
        // 初始化线程静态读取器可选状态
        state.Init(options);

        // 创建读取器
        var reader = new MemoryPackReader(buffer, state);
        try
        {
            // 读取值
            reader.ReadValue(ref value);
            return reader.Consumed;
        }
        finally
        {
            // 释放读取器
            reader.Dispose();
            state.Reset();
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="options">选项</param>
    /// <returns>值</returns>
    public static T? Deserialize<T>(in ReadOnlySequence<byte> buffer, MemoryPackSerializerOptions? options = default)
    {
        T? value = default;
        Deserialize<T>(buffer, ref value);
        return value;
    }
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T">类型</param>
    /// <param name="buffer">缓冲区</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>长度</returns>
    public static int Deserialize<T>(in ReadOnlySequence<byte> buffer, ref T? value, MemoryPackSerializerOptions? options = default)
    {
        // 获取线程静态读取器可选状态
        var state = threadStaticReaderOptionalState;
        // 如果线程静态读取器可选状态为空，则初始化
        if (state == null)
        {
            state = threadStaticReaderOptionalState = new MemoryPackReaderOptionalState();
        }
        state.Init(options);

        var reader = new MemoryPackReader(buffer, state);
        try
        {
            reader.ReadValue(ref value);
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
    /// <typeparam name="T">类型</param>
    /// <param name="stream">流</param>
    /// <param name="options">选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>值</returns>
    public static async ValueTask<T?> DeserializeAsync<T>(Stream stream, MemoryPackSerializerOptions? options = default, CancellationToken cancellationToken = default)
    {
        // 如果流是MemoryStream，则尝试获取缓冲区
        if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> streamBuffer))
        {
            cancellationToken.ThrowIfCancellationRequested();
            // 读取值
            T? value = default;
            var bytesRead = Deserialize<T>(streamBuffer.AsSpan(checked((int)ms.Position)), ref value, options);

            // 模拟实际从流中读取
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
                    // buffer is not added in builder, so return here.
                    ArrayPool<byte>.Shared.Return(buffer);
                    throw;
                }

                offset += read;

                if (read == 0)
                {
                    builder.Add(buffer.AsMemory(0, offset), returnToPool: true);
                    break;

                }
            } while (true);

            // 如果只有一个缓冲区，则可以避免ReadOnlySequence构建成本
            if (builder.TryGetSingleMemory(out var memory))
            {
                // 反序列化
                return Deserialize<T>(memory.Span, options);
            }
            else
            {
                // 构建只读序列
                var seq = builder.Build();
                // 反序列化
                var result = Deserialize<T>(seq, options);
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