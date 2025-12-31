using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace MemoryPack {

/// <summary>
/// 可序列化属性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackableAttribute : Attribute
{
    /// <summary>
    /// 生成类型
    /// </summary>
    public GenerateType GenerateType { get; }
    /// <summary>
    /// 序列化布局
    /// </summary>
    public SerializeLayout SerializeLayout { get; }

    // ctor parameter is parsed in MemoryPackGenerator.Parser TypeMeta for detect which ctor used in MemoryPack.Generator.
    // if modify ctor, be careful.

    /// <summary>
    /// [生成类型, (版本容错或循环引用) ? 显式布局 : 顺序布局]
    /// </summary>
    /// <param name="generateType">生成类型</param>
    public MemoryPackableAttribute(GenerateType generateType = GenerateType.Object)
    {
        this.GenerateType = generateType;
        this.SerializeLayout = (generateType == GenerateType.VersionTolerant || generateType == GenerateType.CircularReference)
            ? SerializeLayout.Explicit
            : SerializeLayout.Sequential;
    }
    /// <summary>
    /// [生成类型.对象, 序列化布局]
    /// </summary>
    /// <param name="serializeLayout">序列化布局</param>
    /// <summary>
    /// [GenerateType.Object, serializeLayout]
    /// </summary>
    /// <param name="serializeLayout">序列化布局</param>
    public MemoryPackableAttribute(SerializeLayout serializeLayout)
    {
        this.GenerateType = GenerateType.Object;
        this.SerializeLayout = serializeLayout;
    }

    /// <summary>
    /// [生成类型, 序列化布局]
    /// </summary>
    /// <param name="generateType">生成类型</param>
    /// <param name="serializeLayout">序列化布局</param>
    public MemoryPackableAttribute(GenerateType generateType, SerializeLayout serializeLayout)
    {
        this.GenerateType = generateType;
        this.SerializeLayout = serializeLayout;
    }
}

/// <summary>
/// 生成类型
/// </summary>
public enum GenerateType
{   
    /// <summary>
    /// 对象
    /// </summary>
    Object,
    /// <summary>
    /// 版本容错
    /// </summary>
    VersionTolerant,
    /// <summary>
    /// 循环引用
    /// </summary>
    CircularReference,
    /// <summary>
    /// 集合
    /// </summary>
    Collection,
    /// <summary>
    /// 不生成
    /// </summary>
    NoGenerate
}

/// <summary>
/// 序列化布局
/// </summary>
public enum SerializeLayout
{   
    /// <summary>
    /// 顺序布局
    /// </summary>
    Sequential, // default
    /// <summary>
    /// 显式布局
    /// </summary>
    Explicit
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
/// <summary>
/// 联合类型属性
/// </summary>
public sealed class MemoryPackUnionAttribute : Attribute
{
    /// <summary>
    /// 标签
    /// </summary>
    public ushort Tag { get; }
    /// <summary>
    /// 类型
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// [标签, 类型]
    /// </summary>
    /// <param name="tag">标签</param>
    /// <param name="type">类型</param>
    public MemoryPackUnionAttribute(ushort tag, Type type)
    {
        this.Tag = tag;
        this.Type = type;
    }
}

/// <summary>
/// 联合类型格式化器属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackUnionFormatterAttribute : Attribute
{
    /// <summary>
    /// 类型
    /// </summary>
    public Type Type { get; }
    /// <summary>
    /// [类型]
    /// </summary>
    /// <param name="type">类型</param>
    public MemoryPackUnionFormatterAttribute(Type type)
    {
        this.Type = type;
    }
}

/// <summary>
/// 允许序列化属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackAllowSerializeAttribute : Attribute
{
}

/// <summary>
/// 序列化顺序属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackOrderAttribute : Attribute
{
    /// <summary>
    /// 顺序
    /// </summary>
    public int Order { get; }
    /// <summary>
    /// [顺序]
    /// </summary>
    /// <param name="order">顺序</param>
    public MemoryPackOrderAttribute(int order)
    {
        this.Order = order;
    }
}

#if !UNITY_2021_2_OR_NEWER

/// <summary>
/// 自定义格式化器属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class MemoryPackCustomFormatterAttribute<T> : Attribute
{
    /// <summary>
    /// 获取格式化器
    /// </summary>
    /// <returns>格式化器</returns>
    public abstract IMemoryPackFormatter<T> GetFormatter();
}

/// <summary>
/// 自定义格式化器属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class MemoryPackCustomFormatterAttribute<TFormatter, T> : Attribute
    where TFormatter : IMemoryPackFormatter<T>
{
    /// <summary>
    /// 获取格式化器
    /// </summary>
    /// <returns>格式化器</returns>
    public abstract TFormatter GetFormatter();
}

#endif

// similar naming as System.Text.Json attribtues
// https://docs.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonattribute

/// <summary>
/// 忽略属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackIgnoreAttribute : Attribute
{
}

/// <summary>
/// 包含属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackIncludeAttribute : Attribute
{
}

/// <summary>
/// 构造函数属性
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackConstructorAttribute : Attribute
{
}

/// <summary>
/// 序列化前属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackOnSerializingAttribute : Attribute
{
}

/// <summary>
/// 序列化后属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackOnSerializedAttribute : Attribute
{
}

/// <summary>
/// 反序列化前属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackOnDeserializingAttribute : Attribute
{
}

/// <summary>
/// 反序列化后属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class MemoryPackOnDeserializedAttribute : Attribute
{
}

// Others
/// <summary>
/// 生成TypeScript属性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class GenerateTypeScriptAttribute : Attribute
{
}

}