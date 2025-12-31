using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using System.Collections.Concurrent;

namespace MemoryPack {

/// <summary>
/// MemoryPackReaderOptionalState池
/// </summary>
public static class MemoryPackReaderOptionalStatePool
{
    static readonly ConcurrentQueue<MemoryPackReaderOptionalState> queue = new ConcurrentQueue<MemoryPackReaderOptionalState>();

    /// <summary>
    /// 租用MemoryPackReaderOptionalState
    /// </summary>
    /// <param name="options">选项</param>
    /// <returns>MemoryPackReaderOptionalState</returns>
    public static MemoryPackReaderOptionalState Rent(MemoryPackSerializerOptions? options)
    {
        if (!queue.TryDequeue(out var state))
        {
            state = new MemoryPackReaderOptionalState();
        }

        state.Init(options);
        return state;
    }

    /// <summary>
    /// 返回MemoryPackReaderOptionalState
    /// </summary>
    /// <param name="state">MemoryPackReaderOptionalState</param>
    /// <returns>void</returns>
    internal static void Return(MemoryPackReaderOptionalState state)
    {
        state.Reset();
        queue.Enqueue(state);
    }
}

/// <summary>
/// MemoryPackReaderOptionalState 可选状态
/// </summary>
public sealed class MemoryPackReaderOptionalState : IDisposable
{
    readonly Dictionary<uint, object> refToObject;
    /// <summary>
    /// 选项
    /// </summary>
    public MemoryPackSerializerOptions Options { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    internal MemoryPackReaderOptionalState()
    {
        refToObject = new Dictionary<uint, object>();
        Options = null!;
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
    /// 获取对象引用
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>对象</returns>
    public object GetObjectReference(uint id)
    {
        if (refToObject.TryGetValue(id, out var value))
        {
            return value;
        }
        MemoryPackSerializationException.ThrowMessage("对象引用不存在，ID:" + id);
        return null!;
    }

    /// <summary>
    /// 添加对象引用
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="value">对象</param>
    /// <returns>void</returns>
    public void AddObjectReference(uint id, object value)
    {
        if (!refToObject.TryAdd(id, value))
        {
            MemoryPackSerializationException.ThrowMessage("对象引用已存在，ID:" + id);
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns>void</returns>
    public void Reset()
    {
        refToObject.Clear();
        Options = null!;
    }

    /// <summary>
    /// 释放
    /// </summary>
    /// <returns>void</returns>
    void IDisposable.Dispose()
    {
        MemoryPackReaderOptionalStatePool.Return(this);
    }
}

}