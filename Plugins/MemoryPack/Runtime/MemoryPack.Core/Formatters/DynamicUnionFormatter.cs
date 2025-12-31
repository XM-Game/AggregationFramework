using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace MemoryPack.Formatters {
/// <summary>
/// 动态联合体格式化器
/// </summary>
/// <typeparam name="T">类型</typeparam>
public sealed class DynamicUnionFormatter<T> : MemoryPackFormatter<T>
    where T : class
{
    /// <summary>
    /// 类型到标签的映射
    /// </summary>
    readonly Dictionary<Type, ushort> typeToTag;
    /// <summary>
    /// 标签到类型的映射
    /// </summary>
    readonly Dictionary<ushort, Type> tagToType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="memoryPackUnions">联合体</param>
    public DynamicUnionFormatter(params (ushort Tag, Type Type)[] memoryPackUnions)
    {
        typeToTag = memoryPackUnions.ToDictionary(x => x.Type, x => x.Tag);
        tagToType = memoryPackUnions.ToDictionary(x => x.Tag, x => x.Type);
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="value">值</param>
    public override void Serialize(ref MemoryPackWriter writer, ref T? value)
    {
        if (value == null)
        {
            writer.WriteNullUnionHeader();
            return;
        }

        var type = value.GetType();
        if (typeToTag.TryGetValue(type, out var tag))
        {
            writer.WriteUnionHeader(tag);
            writer.WriteValue(type, value);
        }
        else
        {
            MemoryPackSerializationException.ThrowNotFoundInUnionType(type, typeof(T));
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">值</param>
    public override void Deserialize(ref MemoryPackReader reader, ref T? value)
    {
        if (!reader.TryReadUnionHeader(out var tag))
        {
            value = default;
            return;
        }
        
        if (tagToType.TryGetValue(tag, out var type))
        {
            object? v = value;
            reader.ReadValue(type, ref v);
            value = (T?)v;
        }
        else
        {
            MemoryPackSerializationException.ThrowInvalidTag(tag, typeof(T));
        }
    }
}

}