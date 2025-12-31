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
using static System.Runtime.CompilerServices.Unsafe;
using System.Runtime.InteropServices;
using System.Text;
#if NET7_0_OR_GREATER
using System.Text.Unicode;
#endif

namespace MemoryPack {

#if NET7_0_OR_GREATER
using static GC;
using static MemoryMarshal;
#else
using static MemoryPack.Internal.MemoryMarshalEx;
#endif
/// <summary>
/// 中文注释 MemoryPackReader 序列化器
/// 用于从二进制数据中读取数据
/// 支持读取各种类型的数据
/// 支持读取各种长度的数据
/// 支持读取各种类型的数据
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref partial struct MemoryPackReader
{
    ReadOnlySequence<byte> bufferSource;  // 缓冲区源
    readonly long totalLength;  // 总长度
#if NET7_0_OR_GREATER
    ref byte bufferReference;  // 缓冲区引用
#else
    ReadOnlySpan<byte> bufferReference;  // 缓冲区引用
#endif
    int bufferLength;  // 缓冲区长度
    byte[]? rentBuffer;  // 租用的缓冲区，用于读取数据
    int advancedCount;  // 已读取的数据长度
    int consumed;     // 已消费的数据长度
    readonly MemoryPackReaderOptionalState optionalState;  // 可选状态

    public int Consumed => consumed;  // 已消费的数据长度
    public long Remaining => totalLength - consumed;  // 剩余的数据长度
    public MemoryPackReaderOptionalState OptionalState => optionalState;  // 可选状态
    public MemoryPackSerializerOptions Options => optionalState.Options;  // 选项
     /// <summary>
     /// 构造函数
     /// </summary>
     /// <param name="sequence">序列</param>
     /// <param name="optionalState">可选状态</param>
     /// <returns>MemoryPackReader</returns>
    public MemoryPackReader(in ReadOnlySequence<byte> sequence, MemoryPackReaderOptionalState optionalState) 
    {
        this.bufferSource = sequence.IsSingleSegment ? ReadOnlySequence<byte>.Empty : sequence;
        var span = sequence.FirstSpan;
#if NET7_0_OR_GREATER
        this.bufferReference = ref MemoryMarshal.GetReference(span);
#else
        this.bufferReference = span;
#endif
        this.bufferLength = span.Length;
        this.advancedCount = 0;
        this.consumed = 0;
        this.rentBuffer = null;
        this.totalLength = sequence.Length;
        this.optionalState = optionalState;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="buffer">缓冲区</param>
    /// <param name="optionalState">可选状态</param>
    /// <returns>MemoryPackReader</returns>
    public MemoryPackReader(ReadOnlySpan<byte> buffer, MemoryPackReaderOptionalState optionalState)
    {
        this.bufferSource = ReadOnlySequence<byte>.Empty;
#if NET7_0_OR_GREATER
        this.bufferReference = ref MemoryMarshal.GetReference(buffer);
#else
        this.bufferReference = buffer;
#endif
        this.bufferLength = buffer.Length;
        this.advancedCount = 0;
        this.consumed = 0;
        this.rentBuffer = null;
        this.totalLength = buffer.Length;
        this.optionalState = optionalState;
    }

    // buffer operations
    /// <summary>
    /// 获取缓冲区引用
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>缓冲区引用</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref byte GetSpanReference(int sizeHint)
    {
        if (sizeHint <= bufferLength)
        {
#if NET7_0_OR_GREATER
            return ref bufferReference;
#else
            return ref MemoryMarshal.GetReference(bufferReference);
#endif
        }

        return ref GetNextSpan(sizeHint);
    }

    /// <summary>
    /// 获取下一个缓冲区
    /// </summary>
    /// <param name="sizeHint">大小提示</param>
    /// <returns>下一个缓冲区</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    ref byte GetNextSpan(int sizeHint)
    {
        if (rentBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(rentBuffer);
            rentBuffer = null;
        }

        if (Remaining == 0)
        {
            MemoryPackSerializationException.ThrowSequenceReachedEnd();
        }

        try
        {
            bufferSource = bufferSource.Slice(advancedCount);
        }
        catch (ArgumentOutOfRangeException)
        {
            MemoryPackSerializationException.ThrowSequenceReachedEnd();
        }

        advancedCount = 0;

        if (sizeHint <= Remaining)
        {
            if (sizeHint <= bufferSource.FirstSpan.Length)
            {
#if NET7_0_OR_GREATER
                bufferReference = ref MemoryMarshal.GetReference(bufferSource.FirstSpan);
                bufferLength = bufferSource.FirstSpan.Length;
                return ref bufferReference;
#else
                bufferReference = bufferSource.FirstSpan;
                bufferLength = bufferSource.FirstSpan.Length;
                return ref MemoryMarshal.GetReference(bufferReference);
#endif
            }

            rentBuffer = ArrayPool<byte>.Shared.Rent(sizeHint);
            bufferSource.Slice(0, sizeHint).CopyTo(rentBuffer);
            var span = rentBuffer.AsSpan(0, sizeHint);
#if NET7_0_OR_GREATER
            bufferReference = ref MemoryMarshal.GetReference(span);
            bufferLength = span.Length;
            return ref bufferReference;
#else
            bufferReference = span;
            bufferLength = span.Length;
            return ref MemoryMarshal.GetReference(bufferReference);
#endif
        }

        MemoryPackSerializationException.ThrowSequenceReachedEnd();
#if NET7_0_OR_GREATER
        return ref bufferReference; // dummy.
#else
        return ref MemoryMarshal.GetReference(bufferReference);
#endif
    }

    /// <summary>
    /// 前进
    /// </summary>
    /// <param name="count">前进的距离</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        if (count == 0) return;

        var rest = bufferLength - count;
        if (rest < 0)
        {
            if (TryAdvanceSequence(count))
            {
                return;
            }
        }

        bufferLength = rest;
#if NET7_0_OR_GREATER
        bufferReference = ref Unsafe.Add(ref bufferReference, count);
#else
        bufferReference = bufferReference.Slice(count);
#endif
        advancedCount += count;
        consumed += count;
    }

    /// <summary>
    /// 尝试前进
    /// </summary>
    /// <param name="count">前进的距离</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    bool TryAdvanceSequence(int count)
    {
        var rest = bufferSource.Length - count;
        if (rest < 0)
        {
            MemoryPackSerializationException.ThrowInvalidAdvance();
        }

        bufferSource = bufferSource.Slice(advancedCount + count);
#if NET7_0_OR_GREATER
        bufferReference = ref MemoryMarshal.GetReference(bufferSource.FirstSpan);
#else
        bufferReference = bufferSource.FirstSpan;
#endif
        bufferLength = bufferSource.FirstSpan.Length;
        advancedCount = 0;
        consumed += count;
        return true;
    }

    /// <summary>
    /// 获取剩余的源
    /// </summary>
    /// <param name="singleSource">单个源</param>
    /// <param name="remainingSource">剩余的源</param>
    public void GetRemainingSource(out ReadOnlySpan<byte> singleSource, out ReadOnlySequence<byte> remainingSource)
    {
        if (bufferSource.IsEmpty)
        {
            remainingSource = ReadOnlySequence<byte>.Empty;
#if NET7_0_OR_GREATER
            singleSource = MemoryMarshal.CreateReadOnlySpan(ref bufferReference, bufferLength);
#else
            singleSource = bufferReference;
#endif
            return;
        }
        else
        {
            if (bufferSource.IsSingleSegment)
            {
                remainingSource = ReadOnlySequence<byte>.Empty;
                singleSource = bufferSource.FirstSpan.Slice(advancedCount);
                return;
            }

            singleSource = default;
            remainingSource = bufferSource.Slice(advancedCount);
            if (remainingSource.IsSingleSegment)
            {
                singleSource = remainingSource.FirstSpan;
                remainingSource = ReadOnlySequence<byte>.Empty;
                return;
            }
            return;
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (rentBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(rentBuffer);
        }
    }

    /// <summary>
    /// 获取格式化器
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>格式化器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMemoryPackFormatter GetFormatter(Type type)
    {
        return MemoryPackFormatterProvider.GetFormatter(type);
    }

    /// <summary>
    /// 获取格式化器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>格式化器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMemoryPackFormatter<T> GetFormatter<T>()
    {
        return MemoryPackFormatterProvider.GetFormatter<T>();
    }

    // read methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadObjectHeader(out byte memberCount)
    {
        memberCount = GetSpanReference(1);
        Advance(1);
        return memberCount != MemoryPackCode.NullObject;
    }

    /// <summary>
    /// 尝试读取联合头
    /// </summary>
    /// <param name="tag">标签</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadUnionHeader(out ushort tag)
    {
        var firstTag = GetSpanReference(1);
        Advance(1);
        if (firstTag < MemoryPackCode.WideTag)
        {
            tag = firstTag;
            return true;
        }
        else if (firstTag == MemoryPackCode.WideTag)
        {
            ReadUnmanaged(out tag);
            return true;
        }
        else
        {
            tag = 0;
            return false;
        }
    }

    /// <summary>
    /// 尝试读取集合头
    /// </summary>
    /// <param name="length">长度</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadCollectionHeader(out int length)
    {
        length = Unsafe.ReadUnaligned<int>(ref GetSpanReference(4));
        Advance(4);

        // If collection-length is larger than buffer-length, it is invalid data.
        if (Remaining < length)
        {
            MemoryPackSerializationException.ThrowInsufficientBufferUnless(length);
        }

        return length != MemoryPackCode.NullCollection;
    }

    /// <summary>
    /// 尝试读取空对象
    /// </summary>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PeekIsNull()
    {
        var code = GetSpanReference(1);
        return code == MemoryPackCode.NullObject;
    }

    /// <summary>
    /// 尝试读取对象头
    /// </summary>
    /// <param name="memberCount">成员数量</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekObjectHeader(out byte memberCount)
    {
        memberCount = GetSpanReference(1);
        return memberCount != MemoryPackCode.NullObject;
    }

    /// <summary>
    /// 尝试读取联合头
    /// </summary>
    /// <param name="tag">标签</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekUnionHeader(out ushort tag)
    {
        var firstTag = GetSpanReference(1);
        if (firstTag < MemoryPackCode.WideTag)
        {
            tag = firstTag;
            return true;
        }
        else if (firstTag == MemoryPackCode.WideTag)
        {
            ref var spanRef = ref GetSpanReference(sizeof(ushort) + 1); // skip firstTag
            tag = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref spanRef, 1));
            return true;
        }
        else
        {
            tag = 0;
            return false;
        }
    }

    /// <summary>
    /// 尝试读取集合头
    /// </summary>
    /// <param name="length">长度</param>
    /// <returns>是否成功</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekCollectionHeader(out int length)
    {
        length = Unsafe.ReadUnaligned<int>(ref GetSpanReference(4));

        // If collection-length is larger than buffer-length, it is invalid data.
        if (Remaining < length)
        {
            MemoryPackSerializationException.ThrowInsufficientBufferUnless(length);
        }

        return length != MemoryPackCode.NullCollection;
    }

    /// <summary>
    /// 尝试读取集合头，不验证集合大小，小心使用。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DangerousTryReadCollectionHeader(out int length)
    {
        length = Unsafe.ReadUnaligned<int>(ref GetSpanReference(4));
        Advance(4);

        return length != MemoryPackCode.NullCollection;
    }

    /// <summary>
    /// 读取字符串
    /// </summary>
    /// <returns>字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? ReadString()
    {
        if (!TryReadCollectionHeader(out var length))
        {
            return null;
        }
        if (length == 0)
        {
            return "";
        }

        if (length > 0)
        {
            return ReadUtf16(length);
        }
        else
        {
            return ReadUtf8(length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取UTF16字符串
    /// </summary>
    /// <param name="length">长度</param>
    /// <returns>UTF16字符串</returns>
    string ReadUtf16(int length)
    {
        var byteCount = checked(length * 2);
        ref var src = ref GetSpanReference(byteCount);

        var str = new string(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, char>(ref src), length));

        Advance(byteCount);

        return str;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] // non default, no inline
    /// <summary>
    /// 读取UTF8字符串
    /// </summary>
    /// <param name="utf8Length">UTF8长度</param>
    /// <returns>UTF8字符串</returns>
    string ReadUtf8(int utf8Length)
    {
        // (int ~utf8-byte-count, int utf16-length, utf8-bytes)
        // already read utf8 length, but it is complement.

        utf8Length = ~utf8Length;

        ref var spanRef = ref GetSpanReference(utf8Length + 4); // + read utf16 length

        string str;
        var utf16Length = Unsafe.ReadUnaligned<int>(ref spanRef);

        if (utf16Length <= 0)
        {
            var src = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref spanRef, 4), utf8Length);
            str = Encoding.UTF8.GetString(src);
        }
        else
        {
            // check malformed utf16Length
            var max = unchecked((Remaining + 1) * 3);
            if (max < 0) max = int.MaxValue;
            if (max < utf16Length)
            {
                MemoryPackSerializationException.ThrowInsufficientBufferUnless(utf8Length);
            }


#if NET7_0_OR_GREATER
            // regular path, know decoded UTF16 length will gets faster decode result
            unsafe
            {
                fixed (byte* p = &Unsafe.Add(ref spanRef, 4))
                {
                    str = string.Create(utf16Length, ((IntPtr)p, utf8Length), static (dest, state) =>
                    {
                        var src = MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>((byte*)state.Item1), state.Item2);
                        var status = Utf8.ToUtf16(src, dest, out var bytesRead, out var charsWritten, replaceInvalidSequences: false);
                        if (status != OperationStatus.Done)
                        {
                            MemoryPackSerializationException.ThrowFailedEncoding(status);
                        }
                    });
                }
            }
#else
            var src = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref spanRef, 4), utf8Length);
            str = Encoding.UTF8.GetString(src);
#endif
        }

        Advance(utf8Length + 4);

        return str;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取未管理类型
    /// </summary>
    /// <typeparam name="T1">类型</typeparam>
    /// <returns>未管理类型</returns>
    public T1 ReadUnmanaged<T1>()
        where T1 : unmanaged
    {
        var size = Unsafe.SizeOf<T1>();
        ref var spanRef = ref GetSpanReference(size);
        var value1 = Unsafe.ReadUnaligned<T1>(ref spanRef);
        Advance(size);
        return value1;
    }

#if NET7_0_OR_GREATER

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取可打包类型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>可打包类型</returns>
    public void ReadPackable<T>(ref T? value)
        where T : IMemoryPackable<T>
    {
        T.Deserialize(ref this, ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取可打包类型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>可打包类型</returns>
    public T? ReadPackable<T>()
        where T : IMemoryPackable<T>
    {
        T? value = default;
        T.Deserialize(ref this, ref value);
        return value;
    }

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取可打包类型
    /// </summary>
    /// <typeparam name="T">类型</param>
    /// <param name="value">值</param>
    /// <returns>可打包类型</returns>
    public void ReadPackable<T>(ref T? value)
        where T : IMemoryPackable<T>
    {
        ReadValue(ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取可打包类型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>可打包类型</returns>
    public T? ReadPackable<T>()
        where T : IMemoryPackable<T>
    {
        return ReadValue<T>();
    }

#endif

    // non packable, get formatter dynamically.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>值</returns>
    public void ReadValue<T>(ref T? value)
    {
        GetFormatter<T>().Deserialize(ref this, ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>值</returns>
    public T? ReadValue<T>()
    {
        T? value = default;
        GetFormatter<T>().Deserialize(ref this, ref value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    /// <returns>值</returns>
    public void ReadValue(Type type, ref object? value)
    {
        GetFormatter(type).Deserialize(ref this, ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>值</returns>
    public object? ReadValue(Type type)
    {
        object? value = default;
        GetFormatter(type).Deserialize(ref this, ref value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <typeparam name="TFormatter">格式化器类型</typeparam>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="formatter">格式化器</param>
    /// <param name="value">值</param>
    /// <returns>值</returns>
    public void ReadValueWithFormatter<TFormatter, T>(TFormatter formatter, ref T? value)
        where TFormatter : IMemoryPackFormatter<T>
    {
        formatter.Deserialize(ref this, ref value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取值
    /// </summary>
    /// <typeparam name="TFormatter">格式化器类型</typeparam>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="formatter">格式化器</param>
    /// <returns>值</returns>
    public T? ReadValueWithFormatter<TFormatter, T>(TFormatter formatter)
        where TFormatter : IMemoryPackFormatter<T>
    {
        T? value = default;
        formatter.Deserialize(ref this, ref value);
        return value;
    }

    #region ReadArray/Span

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>数组</returns>
    public T?[]? ReadArray<T>()
    {
        T?[]? value = default;
        ReadArray(ref value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// 读取数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>数组</returns>
    public void ReadArray<T>(ref T?[]? value)
    {
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            DangerousReadUnmanagedArray(ref value);
            return;
        }

        if (!TryReadCollectionHeader(out var length))
        {
            value = null;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        // T[] support overwrite
        if (value == null || value.Length != length)
        {
            value = new T[length];
        }

        var formatter = GetFormatter<T>();
        for (int i = 0; i < length; i++)
        {
            formatter.Deserialize(ref this, ref value[i]);
        }
    }
    /// <summary>
    /// 读取Span
    /// </summary>
    /// <typeparam name="T">类型</param>
    /// <param name="value">值</param>
    /// <returns>Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadSpan<T>(ref Span<T?> value)
    {
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            DangerousReadUnmanagedSpan(ref value);
            return;
        }

        if (!TryReadCollectionHeader(out var length))
        {
            value = default;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        if (value.Length != length)
        {
            value = new T[length];
        }

        var formatter = GetFormatter<T>();
        for (int i = 0; i < length; i++)
        {
            formatter.Deserialize(ref this, ref value[i]);
        }
    }
     /// <summary>
     /// 读取可打包数组
     /// </summary>
     /// <typeparam name="T">类型</typeparam>
     /// <returns>可打包数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T?[]? ReadPackableArray<T>()
        where T : IMemoryPackable<T>
    {
        T?[]? value = default;
        ReadPackableArray(ref value);
        return value;
    }
    /// <summary>
    /// 读取可打包数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>可打包数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadPackableArray<T>(ref T?[]? value)
        where T : IMemoryPackable<T>
    {
#if !NET7_0_OR_GREATER
        ReadArray(ref value);
        return;
#else
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            DangerousReadUnmanagedArray(ref value);
            return;
        }

        if (!TryReadCollectionHeader(out var length))
        {
            value = null;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        // T[] support overwrite
        if (value == null || value.Length != length)
        {
            value = new T[length];
        }

        for (int i = 0; i < length; i++)
        {
            T.Deserialize(ref this, ref value[i]);
        }
#endif
    }
    /// <summary>
    /// 读取可打包Span
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>可打包Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadPackableSpan<T>(ref Span<T?> value)
        where T : IMemoryPackable<T>
    {
#if !NET7_0_OR_GREATER
        ReadSpan(ref value);
        return;
#else
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            DangerousReadUnmanagedSpan(ref value);
            return;
        }

        if (!TryReadCollectionHeader(out var length))
        {
            value = default;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        if (value.Length != length)
        {
            value = new T[length];
        }

        for (int i = 0; i < length; i++)
        {
            T.Deserialize(ref this, ref value[i]);
        }
#endif
    }

    #endregion

    #region UnmanagedArray/Span
    /// <summary>
    /// 读取未管理数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>未管理数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[]? ReadUnmanagedArray<T>()
        where T : unmanaged
    {
        return DangerousReadUnmanagedArray<T>();
    }
    /// <summary>
    /// 读取未管理数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>未管理数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadUnmanagedArray<T>(ref T[]? value)
        where T : unmanaged
    {
        DangerousReadUnmanagedArray<T>(ref value);
    }
    /// <summary>
    /// 读取未管理Span
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>未管理Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadUnmanagedSpan<T>(ref Span<T> value)
        where T : unmanaged
    {
        DangerousReadUnmanagedSpan<T>(ref value);
    }
    /// <summary>
    /// 读取未管理Span
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>未管理Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T[]? DangerousReadUnmanagedArray<T>()
    {
        if (!TryReadCollectionHeader(out var length))
        {
            return null;
        }

        if (length == 0) return Array.Empty<T>();

        var byteCount = length * Unsafe.SizeOf<T>();
        ref var src = ref GetSpanReference(byteCount);
        var dest = AllocateUninitializedArray<T>(length);
        Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, byte>(ref GetArrayDataReference(dest)), ref src, (uint)byteCount);
        Advance(byteCount);

        return dest;
    }
    /// <summary>
    /// 读取未管理数组
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>未管理数组</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void DangerousReadUnmanagedArray<T>(ref T[]? value)
    {
        if (!TryReadCollectionHeader(out var length))
        {
            value = null;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        var byteCount = length * Unsafe.SizeOf<T>();
        ref var src = ref GetSpanReference(byteCount);

        if (value == null || value.Length != length)
        {
            value = AllocateUninitializedArray<T>(length);
        }

        ref var dest = ref Unsafe.As<T, byte>(ref GetArrayDataReference(value));
        Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)byteCount);

        Advance(byteCount);
    }
    /// <summary>
    /// 读取未管理Span
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns>未管理Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void DangerousReadUnmanagedSpan<T>(ref Span<T> value)
    {
        if (!TryReadCollectionHeader(out var length))
        {
            value = default;
            return;
        }

        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        var byteCount = length * Unsafe.SizeOf<T>();
        ref var src = ref GetSpanReference(byteCount);

        if (value == null || value.Length != length)
        {
            value = AllocateUninitializedArray<T>(length);
        }

        ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value));
        Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)byteCount);

        Advance(byteCount);
    }

    #endregion
    /// <summary>
    /// 读取Span，不读取长度头
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="length">长度</param>
    /// <param name="value">值</param>
    /// <returns>Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadSpanWithoutReadLengthHeader<T>(int length, ref Span<T?> value)
    {
        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }
        // 是否是引用或包含引用
        // 如果是，则分配内存
        // 如果不是，则直接复制
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // 分配内存
            if (value.Length != length)
            {
                value = AllocateUninitializedArray<T>(length);
            }

            // 计算字节数
            var byteCount = length * Unsafe.SizeOf<T>();
            ref var src = ref GetSpanReference(byteCount);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)!);
            // 复制数据
            Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)byteCount);

            Advance(byteCount);
        }
        else
        {
            if (value.Length != length)
            {
                value = new T[length];
            }

            var formatter = GetFormatter<T>();
            for (int i = 0; i < length; i++)
            {
                formatter.Deserialize(ref this, ref value[i]);
            }
        }
    }
    /// <summary>
    /// 读取可打包Span，不读取长度头
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="length">长度</param>
    /// <param name="value">值</param>
    /// <returns>可打包Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadPackableSpanWithoutReadLengthHeader<T>(int length, ref Span<T?> value)
        where T : IMemoryPackable<T>
    {
#if !NET7_0_OR_GREATER
        ReadSpanWithoutReadLengthHeader(length, ref value);
        return;
#else
        // 如果长度为0，则返回空数组
        if (length == 0)
        {
            value = Array.Empty<T>();
            return;
        }

        // 是否是引用或包含引用
        // 如果是，则分配内存
        // 如果不是，则直接复制
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // 分配内存
            if (value.Length != length)
            {
                value = AllocateUninitializedArray<T>(length);
            }

            // 计算字节数
            var byteCount = length * Unsafe.SizeOf<T>();
            ref var src = ref GetSpanReference(byteCount);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)!);
            // 复制数据
            Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint)byteCount);

            Advance(byteCount);
        }
        else
        {
            // 分配内存
            if (value.Length != length)
            {
                value = new T[length];
            }

            // 反序列化
            for (int i = 0; i < length; i++)
            {
                T.Deserialize(ref this, ref value[i]);
            }
        }
#endif
    }
    /// <summary>
    /// 读取未管理Span，不读取长度头
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="isNull">是否为空</param>
    /// <param name="view">Span</param>
    /// <returns>Span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void DangerousReadUnmanagedSpanView<T>(out bool isNull, out ReadOnlySpan<byte> view)
    {
        if (!TryReadCollectionHeader(out var length))
        {
            isNull = true;
            view = default;
            return;
        }

        isNull = false;

        if (length == 0)
        {
            view = Array.Empty<byte>();
            return;
        }
        // 计算字节数
        var byteCount = length * Unsafe.SizeOf<T>();
        ref var src = ref GetSpanReference(byteCount);

        // 创建只读Span
        var span = MemoryMarshal.CreateReadOnlySpan(ref src, byteCount);

        // 前进
        Advance(byteCount);
        // 设置只读Span
        view = span; // safe until call next GetSpanReference
    }
}

}