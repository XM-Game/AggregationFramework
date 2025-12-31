using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Buffers;
using System.Runtime.CompilerServices;

namespace MemoryPack.Internal {

/// <summary>
/// FixedArrayBufferWriter 结构体
/// </summary>
internal struct FixedArrayBufferWriter : IBufferWriter<byte>
{
    byte[] buffer;
    int written;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="buffer">缓冲区</param>
    public FixedArrayBufferWriter(byte[] buffer)
    {
        this.buffer = buffer;
        this.written = 0;
    }

    /// <summary>
    /// 前进
    /// </summary>
    /// <param name="count">计数</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        this.written += count;
    }

    /// <summary>
    /// 获取内存
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>内存</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        var memory = buffer.AsMemory(written);
        if (memory.Length >= sizeHint)
        {
            return memory;
        }

        MemoryPackSerializationException.ThrowMessage("Requested invalid sizeHint.");
        return memory;
    }

    /// <summary>
    /// 获取跨度
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>跨度</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        var span = buffer.AsSpan(written);
        if (span.Length >= sizeHint)
        {
            return span;
        }

        MemoryPackSerializationException.ThrowMessage("Requested invalid sizeHint.");
        return span;
    }

    /// <summary>
    /// 获取填充的缓冲区
    /// </summary>
    /// <returns>填充的缓冲区</returns>
    public byte[] GetFilledBuffer()
    {
        if (written != buffer.Length)
        {
            MemoryPackSerializationException.ThrowMessage("Not filled buffer.");
        }

        return buffer;
    }
}

}