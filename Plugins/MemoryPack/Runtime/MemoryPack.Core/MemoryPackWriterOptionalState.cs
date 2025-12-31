using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace MemoryPack {
/// <summary>
/// MemoryPackWriterOptionalState池
/// </summary>
public static class MemoryPackWriterOptionalStatePool
{
    static readonly ConcurrentQueue<MemoryPackWriterOptionalState> queue = new ConcurrentQueue<MemoryPackWriterOptionalState>();

    /// <summary>
    /// 租用MemoryPackWriterOptionalState
    /// </summary>
    /// <param name="options">选项</param>
    /// <returns>MemoryPackWriterOptionalState</returns>
    public static MemoryPackWriterOptionalState Rent(MemoryPackSerializerOptions? options)
    {
        if (!queue.TryDequeue(out var state))
        {
            state = new MemoryPackWriterOptionalState();
        }

        state.Init(options);
        return state;
    }

    /// <summary>
    /// 返回MemoryPackWriterOptionalState
    /// </summary>
    /// <param name="state">MemoryPackWriterOptionalState</param>
    /// <returns>void</returns>
    internal static void Return(MemoryPackWriterOptionalState state)
    {
        state.Reset();
        queue.Enqueue(state);
    }
}

/// <summary>
/// MemoryPackWriterOptionalState 可选状态
/// </summary>
public sealed class MemoryPackWriterOptionalState : IDisposable
{
    internal static readonly MemoryPackWriterOptionalState NullState = new MemoryPackWriterOptionalState(true);

    /// <summary>
    /// 下一个ID
    /// </summary>
    uint nextId;
    /// <summary>
    /// 对象到引用ID的映射
    /// </summary>
    readonly Dictionary<object, uint> objectToRef;

    /// <summary>
    /// 选项
    /// </summary>
    public MemoryPackSerializerOptions Options { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    internal MemoryPackWriterOptionalState()
    {
        objectToRef = new Dictionary<object, uint>(ReferenceEqualityComparer.Instance);
        Options = null!;
        nextId = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_">是否空状态</param>
    MemoryPackWriterOptionalState(bool _)
    {
        objectToRef = null!;
        Options = MemoryPackSerializerOptions.Default;
        nextId = 0;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="options">选项</param>
    /// <returns>void</returns>
    internal void Init(MemoryPackSerializerOptions? options)
    {
        Options = options ?? MemoryPackSerializerOptions.Default;
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns>void</returns>
    public void Reset()
    {
        objectToRef.Clear();
        Options = null!;
        nextId = 0;
    }

    /// <summary>
    /// 获取或添加引用
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>是否存在引用, ID</returns>
    public (bool existsReference, uint id) GetOrAddReference(object value)
    {
#if NET7_0_OR_GREATER
        ref var id = ref CollectionsMarshal.GetValueRefOrAddDefault(objectToRef, value, out var exists);
        if (exists)
        {
            return (true, id);
        }
        else
        {
            id = nextId++;
            return (false, id);
        }
#else
        if (objectToRef.TryGetValue(value, out var id))
        {
            return (true, id);
        }
        else
        {
            id = nextId++;
            objectToRef.Add(value, id);
            return (false, id);
        }
#endif
    }

    /// <summary>
    /// 释放
    /// </summary>
    /// <returns>void</returns>
    void IDisposable.Dispose()
    {
        MemoryPackWriterOptionalStatePool.Return(this);
    }

    // ReferenceEqualityComparer is exsits in .NET 6 but NetStandard 2.1 does not.
    /// <summary>
    /// ReferenceEqualityComparer 类
    /// </summary>
    sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        ReferenceEqualityComparer() { }

        /// <summary>
        /// 实例
        /// </summary>
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="x">对象</param>
        /// <param name="y">对象</param>
        /// <returns>是否相等</returns>
        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        /// <summary>
        /// 获取哈希码</summary>
        /// <param name="obj">对象</param>
        /// <returns>哈希码</returns>
        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}

}