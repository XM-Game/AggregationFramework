using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Formatters;
using MemoryPack.Internal;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

// Array and Array-like type formatters
// T[]
// T[] where T: unmnaged
// Memory
// ReadOnlyMemory
// ArraySegment
// ReadOnlySequence

namespace MemoryPack
{   
    /// <summary>
    /// 内存打包格式化器提供者
    /// </summary>
    public static partial class MemoryPackFormatterProvider
    {
        /// <summary>
        /// 数组类似格式化器
        /// </summary>
        static readonly Dictionary<Type, Type> ArrayLikeFormatters = new Dictionary<Type, Type>(4)
        {
            // If T[], choose UnmanagedArrayFormatter or DangerousUnmanagedTypeArrayFormatter or ArrayFormatter
            { typeof(ArraySegment<>), typeof(ArraySegmentFormatter<>) },
            { typeof(Memory<>), typeof(MemoryFormatter<>) },
            { typeof(ReadOnlyMemory<>), typeof(ReadOnlyMemoryFormatter<>) },
            { typeof(ReadOnlySequence<>), typeof(ReadOnlySequenceFormatter<>) },
        };
    }
}

namespace MemoryPack.Formatters
{
    /// <summary>
    /// 未管理数组格式化器
    /// </summary>
    [Preserve]
    public sealed class UnmanagedArrayFormatter<T> : MemoryPackFormatter<T[]>
            where T : unmanaged
    {
        /// <summary>
        /// 序列化未管理数组
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">未管理数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref T[]? value)
        {
            writer.WriteUnmanagedArray(value);
        }

        /// <summary>
        /// 反序列化未管理数组
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">未管理数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref T[]? value)
        {
            reader.ReadUnmanagedArray<T>(ref value);
        }
    }

    /// <summary>
    /// 危险未管理数组格式化器
    /// </summary>
    [Preserve]
    public sealed class DangerousUnmanagedArrayFormatter<T> : MemoryPackFormatter<T[]>
    {
        /// <summary>
        /// 序列化危险未管理数组
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">危险未管理数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref T[]? value)
        {
            writer.DangerousWriteUnmanagedArray(value);
        }

        /// <summary>
        /// 反序列化危险未管理数组
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">危险未管理数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref T[]? value)
        {
            reader.DangerousReadUnmanagedArray<T>(ref value);
        }
    }

    /// <summary>
    /// 数组格式化器
    /// </summary>
    [Preserve]
    public sealed class ArrayFormatter<T> : MemoryPackFormatter<T?[]>
    {
        /// <summary>
        /// 序列化数组
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref T?[]? value)
        {
            writer.WriteArray(value);
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">数组</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref T?[]? value)
        {
            reader.ReadArray(ref value);
        }
    }

    /// <summary>
    /// 数组段格式化器
    /// </summary>
    [Preserve]
    public sealed class ArraySegmentFormatter<T> : MemoryPackFormatter<ArraySegment<T?>>
    {
        /// <summary>
        /// 序列化数组段
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">数组段</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref ArraySegment<T?> value)
        {
            writer.WriteSpan(value.AsMemory().Span);
        }

        /// <summary>
        /// 反序列化数组段
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">数组段</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref ArraySegment<T?> value)
        {
            var array = reader.ReadArray<T>();
            value = (array == null) ? default : (ArraySegment<T?>)array;
        }
    }

    /// <summary>
    /// 内存格式化器
    /// </summary>
    [Preserve]
    public sealed class MemoryFormatter<T> : MemoryPackFormatter<Memory<T?>>
    {
        /// <summary>
        /// 序列化内存
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">内存</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref Memory<T?> value)
        {
            writer.WriteSpan(value.Span);
        }

        /// <summary>
        /// 反序列化内存
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">内存</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref Memory<T?> value)
        {
            value = reader.ReadArray<T>();
        }
    }

    /// <summary>
    /// 只读内存格式化器
    /// </summary>
    [Preserve]
    public sealed class ReadOnlyMemoryFormatter<T> : MemoryPackFormatter<ReadOnlyMemory<T?>>
    {
        /// <summary>
        /// 序列化只读内存
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">只读内存</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref ReadOnlyMemory<T?> value)
        {
            writer.WriteSpan(value.Span);
        }

        /// <summary>
        /// 反序列化只读内存
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">只读内存</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref ReadOnlyMemory<T?> value)
        {
            value = reader.ReadArray<T>();
        }
    }

    /// <summary>
    /// 只读序列格式化器
    /// </summary>
    [Preserve]
    public sealed class ReadOnlySequenceFormatter<T> : MemoryPackFormatter<ReadOnlySequence<T?>>
    {
        /// <summary>
        /// 序列化只读序列
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">只读序列</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref ReadOnlySequence<T?> value)
        {
            if (value.IsSingleSegment)
            {
                writer.WriteSpan(value.FirstSpan);
                return;
            }

            writer.WriteCollectionHeader(checked((int)value.Length));
            foreach (var memory in value)
            {
                writer.WriteSpanWithoutLengthHeader(memory.Span);
            }
        }

        /// <summary>
        /// 反序列化只读序列
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">只读序列</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref ReadOnlySequence<T?> value)
        {
            var array = reader.ReadArray<T>();
            value = (array == null) ? default : new ReadOnlySequence<T?>(array);
        }
    }

    /// <summary>
    /// 内存池格式化器
    /// </summary>
    [Preserve]
    public sealed class MemoryPoolFormatter<T> : MemoryPackFormatter<Memory<T?>>
    {
        /// <summary>
        /// 序列化内存池
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">内存池</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref Memory<T?> value)
        {
            writer.WriteSpan(value.Span);
        }

        /// <summary>
        /// 反序列化内存池
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">内存池</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref Memory<T?> value)
        {
            if (!reader.TryReadCollectionHeader(out var length))
            {
                value = null;
                return;
            }

            if (length == 0)
            {
                value = Memory<T?>.Empty;
                return;
            }

            var memory = ArrayPool<T?>.Shared.Rent(length).AsMemory(0, length);
            var span = memory.Span;
            reader.ReadSpanWithoutReadLengthHeader(length, ref span);
            value = memory;
        }
    }

    /// <summary>
    /// 只读内存池格式化器
    /// </summary>
    [Preserve]
    public sealed class ReadOnlyMemoryPoolFormatter<T> : MemoryPackFormatter<ReadOnlyMemory<T?>>
    {
        /// <summary>
        /// 序列化只读内存池
        /// </summary>
        /// <param name="writer">MemoryPackWriter</param>
        /// <param name="value">只读内存池</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Serialize(ref MemoryPackWriter writer, ref ReadOnlyMemory<T?> value)
        {
            writer.WriteSpan(value.Span);
        }

        /// <summary>
        /// 反序列化只读内存池
        /// </summary>
        /// <param name="reader">MemoryPackReader</param>
        /// <param name="value">只读内存池</param>
        /// <returns>void</returns>
        [Preserve]
        public override void Deserialize(ref MemoryPackReader reader, ref ReadOnlyMemory<T?> value)
        {
            if (!reader.TryReadCollectionHeader(out var length))
            {
                value = null;
                return;
            }

            if (length == 0)
            {
                value = Memory<T?>.Empty;
                return;
            }
            // 分配内存
            var memory = ArrayPool<T?>.Shared.Rent(length).AsMemory(0, length);
            // 创建Span
            var span = memory.Span;
            // 读取Span
            reader.ReadSpanWithoutReadLengthHeader(length, ref span);
            // 设置值
            value = memory;
        }
    }
}
