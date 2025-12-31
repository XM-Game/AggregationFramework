using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
using MemoryPack.Internal;

namespace MemoryPack.Formatters {
/// <summary>
/// 延迟加载类型格式化器
/// </summary>
/// <typeparam name="T">类型</typeparam>
[Preserve]
public sealed class LazyFormatter<T> : MemoryPackFormatter<Lazy<T?>>
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">MemoryPackWriter</param>
    /// <param name="value">延迟加载类型</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Serialize(ref MemoryPackWriter writer, ref Lazy<T?>? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
            return;
        }

        writer.WriteObjectHeader(1);
        writer.WriteValue(value.Value);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">MemoryPackReader</param>
    /// <param name="value">延迟加载类型</param>
    /// <returns>void</returns>
    [Preserve]
    public override void Deserialize(ref MemoryPackReader reader, ref Lazy<T?>? value)
    {
        if (!reader.TryReadObjectHeader(out var count))
        {
            value = null;
            return;
        }

        if (count != 1) MemoryPackSerializationException.ThrowInvalidPropertyCount(1, count);

        var v = reader.ReadValue<T>();
        value = new Lazy<T?>(v);
    }
}

}