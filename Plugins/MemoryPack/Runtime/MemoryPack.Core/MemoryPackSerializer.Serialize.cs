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

#if NET7_0_OR_GREATER
using static MemoryMarshal;
using static GC;
#else
using static MemoryPack.Internal.MemoryMarshalEx;
#endif
/// <summary>
/// MemoryPackSerializer 类
/// </summary>
public static partial class MemoryPackSerializer
{
    /// <summary>
    /// 线程静态序列化状态
    /// </summary>
    [ThreadStatic]
    static SerializerWriterThreadStaticState? threadStaticState;
    /// <summary>
    /// 线程静态写入器可选状态
    /// </summary>
    [ThreadStatic]
    static MemoryPackWriterOptionalState? threadStaticWriterOptionalState;

    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>字节数组</returns>
    public static byte[] Serialize<T>(in T? value, MemoryPackSerializerOptions? options = default)
    {
        // 是否是引用或包含引用
        // 如果是，则分配内存
        // 如果不是，则直接复制
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            var array = AllocateUninitializedArray<byte>(Unsafe.SizeOf<T>());
            Unsafe.WriteUnaligned(ref GetArrayDataReference(array), value);
            return array;
        }
#if NET7_0_OR_GREATER
        var typeKind = TypeHelpers.TryGetUnmanagedSZArrayElementSizeOrMemoryPackableFixedSize<T>(out var elementSize);
        if (typeKind == TypeHelpers.TypeKind.None)
        {
            // do nothing
        }
        else if (typeKind == TypeHelpers.TypeKind.UnmanagedSZArray)
        {
            if (value == null)
            {
                return MemoryPackCode.NullCollectionData.ToArray();
            }
            // 获取源数组
            var srcArray = ((Array)(object)value!);
            var length = srcArray.Length;
            // 如果长度为0，则返回空数组
            if (length == 0)
            {
                return new byte[4] { 0, 0, 0, 0 };
            }

            // 计算数据大小
            var dataSize = elementSize * length;
            // 分配目标数组
            var destArray = AllocateUninitializedArray<byte>(dataSize + 4);
            ref var head = ref MemoryMarshal.GetArrayDataReference(destArray);

            // 写入长度
            Unsafe.WriteUnaligned(ref head, length);
            // 复制数据
            Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref head, 4), ref MemoryMarshal.GetArrayDataReference(srcArray), (uint)dataSize);

            return destArray;
        }
        else if (typeKind == TypeHelpers.TypeKind.FixedSizeMemoryPackable)
        {
            // 分配缓冲区
            var buffer = new byte[(value == null) ? 1 : elementSize];
            // 创建固定数组写入器
            var bufferWriter = new FixedArrayBufferWriter(buffer);
            // 创建写入器
            var writer = new MemoryPackWriter<FixedArrayBufferWriter>(ref bufferWriter, buffer, MemoryPackWriterOptionalState.NullState);
            Serialize(ref writer, value);
            return bufferWriter.GetFilledBuffer();
        }
#endif

        var state = threadStaticState;
        if (state == null)
        {
            state = threadStaticState = new SerializerWriterThreadStaticState();
        }
        state.Init(options);

        try
        {
            var writer = new MemoryPackWriter(ref Unsafe.As<ReusableLinkedArrayBufferWriter, IBufferWriter<byte>>(ref state.BufferWriter), state.BufferWriter.DangerousGetFirstBuffer(), state.OptionalState);
            // 序列化
            Serialize(ref writer, value);
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
    /// <typeparam name="T">类型</typeparam>
    /// <param name="bufferWriter">缓冲区写入器</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <returns>void</returns>
    public static unsafe void Serialize<T>(in IBufferWriter<byte> bufferWriter, in T? value, MemoryPackSerializerOptions? options = default)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        // 是否是引用或包含引用
        // 如果是，则分配内存
        // 如果不是，则直接复制
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // 获取缓冲区
            var buffer = bufferWriter.GetSpan(Unsafe.SizeOf<T>());
            // 写入值
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), value);
            bufferWriter.Advance(Unsafe.SizeOf<T>());
            return;
        }
#if NET7_0_OR_GREATER
        var typeKind = TypeHelpers.TryGetUnmanagedSZArrayElementSizeOrMemoryPackableFixedSize<T>(out var elementSize);
        if (typeKind == TypeHelpers.TypeKind.UnmanagedSZArray)
        {
            if (value == null)
            {
                var span = bufferWriter.GetSpan(4);
                MemoryPackCode.NullCollectionData.CopyTo(span);
                bufferWriter.Advance(4);
                return;
            }

            var srcArray = ((Array)(object)value!);
            var length = srcArray.Length;
            if (length == 0)
            {
                var span = bufferWriter.GetSpan(4);
                MemoryPackCode.ZeroCollectionData.CopyTo(span);
                bufferWriter.Advance(4);
                return;
            }
            var dataSize = elementSize * length;
            var destSpan = bufferWriter.GetSpan(dataSize + 4);
            ref var head = ref MemoryMarshal.GetReference(destSpan);

            Unsafe.WriteUnaligned(ref head, length);
            Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref head, 4), ref MemoryMarshal.GetArrayDataReference(srcArray), (uint)dataSize);

            bufferWriter.Advance(dataSize + 4);
            return;
        }
#endif

        var state = threadStaticWriterOptionalState;
        if (state == null)
        {
            state = threadStaticWriterOptionalState = new MemoryPackWriterOptionalState();
        }
        state.Init(options);

        try
        {
            var writer = new MemoryPackWriter(ref Unsafe.AsRef(bufferWriter), state);
            Serialize(ref writer, value);
        }
        finally
        {
            state.Reset();
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    /// <returns>void</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize<T>(ref MemoryPackWriter writer, in T? value)
#if NET7_0_OR_GREATER
        
#else
        
#endif
    {
        // 写入值
        writer.WriteValue(value);
        // 刷新
        writer.Flush();
    }

    /// <summary>
    /// 异步序列化
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="stream">流</param>
    /// <param name="value">值</param>
    /// <param name="options">选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static async ValueTask SerializeAsync<T>(Stream stream, T? value, MemoryPackSerializerOptions? options = default, CancellationToken cancellationToken = default)
    {
        // 租用临时写入器
        var tempWriter = ReusableLinkedArrayBufferWriterPool.Rent();
        try
        {
            // 序列化
            Serialize(tempWriter, value, options);
            // 写入流
            await tempWriter.WriteToAndResetAsync(stream, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ReusableLinkedArrayBufferWriterPool.Return(tempWriter);
        }
    }

    /// <summary>
    /// 序列化写入器线程静态状态
    /// </summary>
    sealed class SerializerWriterThreadStaticState
    {
        /// <summary>
        /// 缓冲区写入器
        /// </summary>
        public ReusableLinkedArrayBufferWriter BufferWriter;
        /// <summary>
        /// 可选状态
        /// </summary>
        public MemoryPackWriterOptionalState OptionalState;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SerializerWriterThreadStaticState()
        {
            BufferWriter = new ReusableLinkedArrayBufferWriter(useFirstBuffer: true, pinned: true);
            OptionalState = new MemoryPackWriterOptionalState();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="options">选项</param>
        public void Init(MemoryPackSerializerOptions? options)
        {
            OptionalState.Init(options);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            BufferWriter.Reset();
            OptionalState.Reset();
        }
    }
}

}