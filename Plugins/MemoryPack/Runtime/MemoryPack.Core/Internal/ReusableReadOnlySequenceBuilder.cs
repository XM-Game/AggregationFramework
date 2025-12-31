using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace MemoryPack.Internal {

/// <summary>
/// ReusableReadOnlySequenceBuilderPool 类
/// </summary>
internal static class ReusableReadOnlySequenceBuilderPool
{
    static readonly ConcurrentQueue<ReusableReadOnlySequenceBuilder> queue = new();

    /// <summary>
    /// 租用
    /// </summary>
    /// <returns>ReusableReadOnlySequenceBuilder</returns>
    public static ReusableReadOnlySequenceBuilder Rent()
    {
        if (queue.TryDequeue(out var builder))
        {
            return builder;
        }
        return new ReusableReadOnlySequenceBuilder();
    }

    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="builder">ReusableReadOnlySequenceBuilder</param>
    public static void Return(ReusableReadOnlySequenceBuilder builder)
    {
        builder.Reset();
        queue.Enqueue(builder);
    }
}

/// <summary>
/// ReusableReadOnlySequenceBuilder 类
/// </summary>
internal sealed class ReusableReadOnlySequenceBuilder
{
    readonly Stack<Segment> segmentPool;
    readonly List<Segment> list;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ReusableReadOnlySequenceBuilder()
    {
        list = new();
        segmentPool = new Stack<Segment>();
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="buffer">缓冲区</param>
    /// <param name="returnToPool">是否返回池</param>
    public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool)
    {
        if (!segmentPool.TryPop(out var segment))
        {
            segment = new Segment();
        }

        segment.SetBuffer(buffer, returnToPool);
        list.Add(segment);
    }

    /// <summary>
    /// 尝试获取单个内存
    /// </summary>
    /// <param name="memory">内存</param>
    /// <returns>是否成功</returns>
    public bool TryGetSingleMemory(out ReadOnlyMemory<byte> memory)
    {
        if (list.Count == 1)
        {
            memory = list[0].Memory;
            return true;
        }
        memory = default;
        return false;
    }

    /// <summary>
    /// 构建
    /// </summary>
    /// <returns>ReadOnlySequence</returns>
    public ReadOnlySequence<byte> Build()
    {
        if (list.Count == 0)
        {
            return ReadOnlySequence<byte>.Empty;
        }

        if (list.Count == 1)
        {
            return new ReadOnlySequence<byte>(list[0].Memory);
        }

        long running = 0;
#if NET7_0_OR_GREATER
        var span = CollectionsMarshal.AsSpan(list);
        for (int i = 0; i < span.Length; i++)
        {
            var next = i < span.Length - 1 ? span[i + 1] : null;
            span[i].SetRunningIndexAndNext(running, next);
            running += span[i].Memory.Length;
        }
        var firstSegment = span[0];
        var lastSegment = span[span.Length - 1];
#else
        var span = list;
        for (int i = 0; i < span.Count; i++)
        {
            var next = i < span.Count - 1 ? span[i + 1] : null;
            span[i].SetRunningIndexAndNext(running, next);
            running += span[i].Memory.Length;
        }
        var firstSegment = span[0];
        var lastSegment = span[span.Count - 1];
#endif
        return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
#if NET7_0_OR_GREATER
        var span = CollectionsMarshal.AsSpan(list);
#else
        var span = list;
#endif
        foreach (var item in span)
        {
            item.Reset();
            segmentPool.Push(item);
        }
        list.Clear();
    }

    /// <summary>
    /// Segment 类
    /// </summary>
    class Segment : ReadOnlySequenceSegment<byte>
    {
        bool returnToPool;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Segment()
        {
            returnToPool = false;
        }

        /// <summary>
        /// 设置缓冲区
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="returnToPool">是否返回池</param>
        public void SetBuffer(ReadOnlyMemory<byte> buffer, bool returnToPool)
        {
            Memory = buffer;
            this.returnToPool = returnToPool;
        }

        public void Reset()
        {
            if (returnToPool)
            {
                if (MemoryMarshal.TryGetArray(Memory, out var segment) && segment.Array != null)
                {
                    ArrayPool<byte>.Shared.Return(segment.Array, clearArray: false);
                }
            }
            Memory = default;
            RunningIndex = 0;
            Next = null;
        }

        public void SetRunningIndexAndNext(long runningIndex, Segment? nextSegment)
        {
            RunningIndex = runningIndex;
            Next = nextSegment;
        }
    }
}

}