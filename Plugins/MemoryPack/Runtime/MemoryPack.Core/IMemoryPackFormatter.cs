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

namespace MemoryPack {

/// <summary>
/// 格式化器接口
/// </summary>
[Preserve]
public interface IMemoryPackFormatter
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    void Serialize(ref MemoryPackWriter writer, ref object? value)
#if NET7_0_OR_GREATER
        ;
#else
        ;
#endif
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    void Deserialize(ref MemoryPackReader reader, ref object? value);
}

/// <summary>
/// 泛型格式化器接口
/// </summary>
[Preserve]
public interface IMemoryPackFormatter<T>
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    void Serialize(ref MemoryPackWriter writer, ref T? value)
#if NET7_0_OR_GREATER
        ;
#else
        ;
#endif
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    void Deserialize(ref MemoryPackReader reader, ref T? value);
}

/// <summary>
/// 抽象基类
/// </summary>
[Preserve]
public abstract class MemoryPackFormatter<T> : IMemoryPackFormatter<T>, IMemoryPackFormatter
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    public abstract void Serialize(ref MemoryPackWriter writer, ref T? value)
#if NET7_0_OR_GREATER
        ;
#else
        ;
#endif
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    public abstract void Deserialize(ref MemoryPackReader reader, ref T? value);

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    [Preserve]
    void IMemoryPackFormatter.Serialize(ref MemoryPackWriter writer, ref object? value)
    {
        var v = (value == null)
            ? default(T?)
            : (T?)value;
        Serialize(ref writer, ref v);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    [Preserve]
    void IMemoryPackFormatter.Deserialize(ref MemoryPackReader reader, ref object? value)
    {
        var v = (value == null)
            ? default(T?)
            : (T?)value;
        Deserialize(ref reader, ref v);
        value = v;
    }
}

}