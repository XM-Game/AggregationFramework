# MemoryPack 序列化框架 API 使用手册

## 目录

1. [基础 API](#基础-api)
2. [序列化 API](#序列化-api)
3. [反序列化 API](#反序列化-api)
4. [属性 API](#属性-api)
5. [格式化器 API](#格式化器-api)
6. [选项 API](#选项-api)
7. [Unity 集成 API](#unity-集成-api)
8. [高级功能 API](#高级功能-api)

---

## 基础 API

### MemoryPackSerializer

主要的序列化器类，提供了序列化和反序列化的入口方法。

```csharp
using MemoryPack;

// 序列化对象为字节数组
byte[] data = MemoryPackSerializer.Serialize(myObject);

// 反序列化字节数组为对象
MyClass obj = MemoryPackSerializer.Deserialize<MyClass>(data);
```

---

## 序列化 API

### Serialize - 序列化为字节数组

```csharp
// 基本用法
byte[] data = MemoryPackSerializer.Serialize(myObject);

// 指定选项
var options = MemoryPackSerializerOptions.Utf8;
byte[] data = MemoryPackSerializer.Serialize(myObject, options);
```

### Serialize - 序列化到 IBufferWriter

```csharp
using System.Buffers;

var bufferWriter = new ArrayBufferWriter<byte>();
MemoryPackSerializer.Serialize(bufferWriter, myObject, options);
byte[] data = bufferWriter.WrittenSpan.ToArray();
```

### Serialize - 序列化到 MemoryPackWriter

```csharp
var bufferWriter = new ArrayBufferWriter<byte>();
var writer = new MemoryPackWriter(ref bufferWriter, optionalState);
MemoryPackSerializer.Serialize(ref writer, myObject);
writer.Flush();
```

### SerializeAsync - 异步序列化到流

```csharp
using System.IO;

await MemoryPackSerializer.SerializeAsync(stream, myObject, options, cancellationToken);
```

### Serialize - 非泛型序列化

```csharp
// 使用 Type 类型进行序列化
Type type = typeof(MyClass);
byte[] data = MemoryPackSerializer.Serialize(type, myObject, options);
```

---

## 反序列化 API

### Deserialize - 从字节数组反序列化

```csharp
// 基本用法
MyClass obj = MemoryPackSerializer.Deserialize<MyClass>(data);

// 指定选项
var options = MemoryPackSerializerOptions.Utf8;
MyClass obj = MemoryPackSerializer.Deserialize<MyClass>(data, options);
```

### Deserialize - 从 ReadOnlySpan 反序列化

```csharp
ReadOnlySpan<byte> span = data;
MyClass obj = MemoryPackSerializer.Deserialize<MyClass>(span);
```

### Deserialize - 从 ReadOnlySequence 反序列化

```csharp
ReadOnlySequence<byte> sequence = ...;
MyClass obj = MemoryPackSerializer.Deserialize<MyClass>(sequence);
```

### Deserialize - 反序列化到现有对象

```csharp
MyClass obj = new MyClass();
int bytesRead = MemoryPackSerializer.Deserialize(data, ref obj);
```

### DeserializeAsync - 异步从流反序列化

```csharp
MyClass obj = await MemoryPackSerializer.DeserializeAsync<MyClass>(stream, options, cancellationToken);
```

### Deserialize - 非泛型反序列化

```csharp
// 使用 Type 类型进行反序列化
Type type = typeof(MyClass);
object obj = MemoryPackSerializer.Deserialize(type, data, options);
```

---

## 属性 API

### MemoryPackableAttribute

标记类型为可序列化。

```csharp
[MemoryPackable]
public partial class MyClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// 指定生成类型
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class MyClass
{
}

// 指定序列化布局
[MemoryPackable(SerializeLayout.Explicit)]
public partial class MyClass
{
}

// 组合使用
[MemoryPackable(GenerateType.CircularReference, SerializeLayout.Explicit)]
public partial class MyClass
{
}
```

### GenerateType 枚举

```csharp
public enum GenerateType
{
    Object,              // 对象模式（默认）
    VersionTolerant,     // 版本容错模式
    CircularReference,   // 循环引用模式
    Collection,          // 集合模式
    NoGenerate           // 不生成模式
}
```

### SerializeLayout 枚举

```csharp
public enum SerializeLayout
{
    Sequential,  // 顺序布局（默认）
    Explicit      // 显式布局
}
```

### MemoryPackIgnoreAttribute

忽略字段或属性。

```csharp
[MemoryPackable]
public partial class MyClass
{
    public int Id { get; set; }
    
    [MemoryPackIgnore]
    public string TempData { get; set; }  // 不会被序列化
}
```

### MemoryPackIncludeAttribute

显式包含字段或属性（用于版本容错模式）。

```csharp
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class MyClass
{
    [MemoryPackInclude]
    public int Id { get; set; }
    
    [MemoryPackInclude]
    public string Name { get; set; }
}
```

### MemoryPackOrderAttribute

指定序列化顺序。

```csharp
[MemoryPackable]
public partial class MyClass
{
    [MemoryPackOrder(0)]
    public int Id { get; set; }
    
    [MemoryPackOrder(1)]
    public string Name { get; set; }
}
```

### MemoryPackConstructorAttribute

标记用于反序列化的构造函数。

```csharp
[MemoryPackable]
public partial class MyClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [MemoryPackConstructor]
    public MyClass(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
```

### 生命周期属性

```csharp
[MemoryPackable]
public partial class MyClass
{
    [MemoryPackOnSerializing]
    void OnSerializing()
    {
        // 序列化前调用
    }
    
    [MemoryPackOnSerialized]
    void OnSerialized()
    {
        // 序列化后调用
    }
    
    [MemoryPackOnDeserializing]
    void OnDeserializing()
    {
        // 反序列化前调用
    }
    
    [MemoryPackOnDeserialized]
    void OnDeserialized()
    {
        // 反序列化后调用
    }
}
```

### MemoryPackUnionAttribute

定义联合类型，支持多态序列化。

```csharp
[MemoryPackable]
[MemoryPackUnion(0, typeof(ClassA))]
[MemoryPackUnion(1, typeof(ClassB))]
public partial interface IBase
{
}

[MemoryPackable]
public partial class ClassA : IBase
{
    public int ValueA { get; set; }
}

[MemoryPackable]
public partial class ClassB : IBase
{
    public string ValueB { get; set; }
}
```

### 自定义格式化器属性

```csharp
[MemoryPackable]
public partial class MyClass
{
    // UTF-8 编码
    [Utf8StringFormatter]
    public string Utf8String { get; set; }
    
    // UTF-16 编码
    [Utf16StringFormatter]
    public string Utf16String { get; set; }
    
    // 字符串内化
    [InternStringFormatter]
    public string InternedString { get; set; }
    
    // Brotli 压缩
    [BrotliFormatter]
    public byte[] CompressedData { get; set; }
    
    // 位打包
    [BitPackFormatter]
    public bool[] PackedBools { get; set; }
}
```

---

## 格式化器 API

### IMemoryPackable<T> 接口

实现自定义序列化逻辑（.NET 7+）。

```csharp
#if NET7_0_OR_GREATER
public struct MyStruct : IMemoryPackable<MyStruct>
{
    public int Value { get; set; }
    
    static void IMemoryPackable<MyStruct>.Serialize(ref MemoryPackWriter writer, ref MyStruct value)
    {
        writer.WriteUnmanaged(value.Value);
    }
    
    static void IMemoryPackable<MyStruct>.Deserialize(ref MemoryPackReader reader, ref MyStruct value)
    {
        reader.ReadUnmanaged(out int val);
        value.Value = val;
    }
}
#endif
```

### IMemoryPackFormatter<T> 接口

实现自定义格式化器。

```csharp
public class MyCustomFormatter : MemoryPackFormatter<MyClass>
{
    public override void Serialize(ref MemoryPackWriter writer, ref MyClass? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
            return;
        }
        
        writer.WriteObjectHeader(2);
        writer.WriteUnmanaged(value.Id);
        writer.WriteValue(value.Name);
    }
    
    public override void Deserialize(ref MemoryPackReader reader, ref MyClass? value)
    {
        if (!reader.TryReadObjectHeader(out var count))
        {
            value = null;
            return;
        }
        
        reader.ReadUnmanaged(out int id);
        reader.ReadValue<string>(out var name);
        
        value = new MyClass { Id = id, Name = name };
    }
}
```

### 注册自定义格式化器

```csharp
// 通过属性注册
[MemoryPackable]
public partial class MyClass
{
    [MemoryPackCustomFormatter(typeof(MyCustomFormatter))]
    public MyNestedClass Nested { get; set; }
}

// 通过代码注册（需要访问内部 API）
// MemoryPackFormatterProvider.RegisterFormatter(new MyCustomFormatter());
```

---

## 选项 API

### MemoryPackSerializerOptions

序列化选项配置。

```csharp
// 默认选项（UTF-8）
var options = MemoryPackSerializerOptions.Default;

// UTF-8 选项
var options = MemoryPackSerializerOptions.Utf8;

// UTF-16 选项
var options = MemoryPackSerializerOptions.Utf16;

// 自定义选项
var options = new MemoryPackSerializerOptions
{
    StringEncoding = StringEncoding.Utf8,
    ServiceProvider = serviceProvider  // 支持依赖注入
};
```

### StringEncoding 枚举

```csharp
public enum StringEncoding : byte
{
    Utf16,
    Utf8
}
```

---

## Unity 集成 API

### Unity 类型支持

MemoryPack 自动支持以下 Unity 类型：

- `AnimationCurve`
- `Gradient`
- `RectOffset`
- `Vector2`, `Vector3`, `Vector4`
- `Quaternion`
- `Color`, `Color32`
- `Rect`
- `Bounds`
- `Matrix4x4`

```csharp
[MemoryPackable]
public partial class UnityData
{
    public AnimationCurve Curve { get; set; }
    public Gradient Gradient { get; set; }
    public Vector3 Position { get; set; }
    public Color Color { get; set; }
}
```

### ProviderInitializer

Unity 格式化器提供者初始化器。

```csharp
// 在 Unity 启动时自动初始化
// 无需手动调用
```

---

## 高级功能 API

### 版本容错序列化

```csharp
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class VersionedClass
{
    [MemoryPackInclude]
    public int Id { get; set; }
    
    [MemoryPackInclude]
    public string Name { get; set; }
    
    // 新版本添加的字段，旧版本会忽略
    [MemoryPackInclude]
    public string NewField { get; set; }
}
```

### 循环引用序列化

```csharp
[MemoryPackable(GenerateType.CircularReference)]
public partial class Node
{
    public int Value { get; set; }
    public Node? Next { get; set; }  // 可以形成循环引用
}
```

### 压缩序列化

```csharp
[MemoryPackable]
public partial class CompressedData
{
    // 使用 Brotli 压缩
    [BrotliFormatter(CompressionLevel.Fastest)]
    public byte[] CompressedBytes { get; set; }
    
    // 压缩字符串
    [BrotliStringFormatter(CompressionLevel.Optimal)]
    public string CompressedString { get; set; }
    
    // 压缩自定义类型
    [BrotliFormatter<MyClass>(CompressionLevel.SmallestSize)]
    public MyClass CompressedObject { get; set; }
}
```

### 位打包序列化

```csharp
[MemoryPackable]
public partial class PackedData
{
    // 布尔数组压缩为位数组
    [BitPackFormatter]
    public bool[] PackedBools { get; set; }
}
```

### 字符串内化

```csharp
[MemoryPackable]
public partial class InternedData
{
    // 字符串自动内化到字符串池
    [InternStringFormatter]
    public string InternedString { get; set; }
}
```

### 字典忽略大小写

```csharp
[MemoryPackable]
public partial class CaseInsensitiveData
{
    // 字典键忽略大小写
    [OrdinalIgnoreCaseStringDictionaryFormatter<string>]
    public Dictionary<string, string> CaseInsensitiveDict { get; set; }
}
```

---

## 完整示例

### 示例 1：基础序列化

```csharp
using MemoryPack;

[MemoryPackable]
public partial class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

// 序列化
var person = new Person { Id = 1, Name = "Alice", Age = 30 };
byte[] data = MemoryPackSerializer.Serialize(person);

// 反序列化
Person deserialized = MemoryPackSerializer.Deserialize<Person>(data);
```

### 示例 2：集合序列化

```csharp
[MemoryPackable]
public partial class Company
{
    public string Name { get; set; }
    public List<Person> Employees { get; set; }
    public Dictionary<string, Person> EmployeeMap { get; set; }
}

var company = new Company
{
    Name = "Acme Corp",
    Employees = new List<Person> { person1, person2 },
    EmployeeMap = new Dictionary<string, Person> { { "alice", person1 } }
};

byte[] data = MemoryPackSerializer.Serialize(company);
Company deserialized = MemoryPackSerializer.Deserialize<Company>(data);
```

### 示例 3：版本容错

```csharp
// 版本 1
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class UserV1
{
    [MemoryPackInclude]
    public int Id { get; set; }
    
    [MemoryPackInclude]
    public string Name { get; set; }
}

// 版本 2（添加新字段）
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class UserV2
{
    [MemoryPackInclude]
    public int Id { get; set; }
    
    [MemoryPackInclude]
    public string Name { get; set; }
    
    [MemoryPackInclude]  // 新字段
    public string Email { get; set; }
}

// 旧版本数据可以反序列化为新版本（Email 为 null）
// 新版本数据可以反序列化为旧版本（Email 被忽略）
```

### 示例 4：循环引用

```csharp
[MemoryPackable(GenerateType.CircularReference)]
public partial class TreeNode
{
    public int Value { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }
}

var root = new TreeNode { Value = 1 };
root.Left = new TreeNode { Value = 2, Left = root };  // 循环引用

byte[] data = MemoryPackSerializer.Serialize(root);
TreeNode deserialized = MemoryPackSerializer.Deserialize<TreeNode>(data);
```

### 示例 5：多态序列化

```csharp
[MemoryPackable]
[MemoryPackUnion(0, typeof(Circle))]
[MemoryPackUnion(1, typeof(Rectangle))]
public partial interface IShape
{
}

[MemoryPackable]
public partial class Circle : IShape
{
    public float Radius { get; set; }
}

[MemoryPackable]
public partial class Rectangle : IShape
{
    public float Width { get; set; }
    public float Height { get; set; }
}

IShape shape = new Circle { Radius = 5.0f };
byte[] data = MemoryPackSerializer.Serialize(shape);
IShape deserialized = MemoryPackSerializer.Deserialize<IShape>(data);
```

### 示例 6：Unity 类型

```csharp
using UnityEngine;
using MemoryPack;

[MemoryPackable]
public partial class UnityGameData
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public AnimationCurve Curve { get; set; }
    public Gradient Gradient { get; set; }
    public Color Color { get; set; }
}

var data = new UnityGameData
{
    Position = new Vector3(1, 2, 3),
    Rotation = Quaternion.identity,
    Curve = AnimationCurve.Linear(0, 0, 1, 1),
    Gradient = new Gradient(),
    Color = Color.red
};

byte[] serialized = MemoryPackSerializer.Serialize(data);
UnityGameData deserialized = MemoryPackSerializer.Deserialize<UnityGameData>(serialized);
```

### 示例 7：异步序列化

```csharp
using System.IO;
using System.Threading.Tasks;

// 异步序列化到流
await MemoryPackSerializer.SerializeAsync(stream, myObject, options, cancellationToken);

// 异步从流反序列化
MyClass obj = await MemoryPackSerializer.DeserializeAsync<MyClass>(stream, options, cancellationToken);
```

### 示例 8：自定义格式化器

```csharp
public class DateTimeOffsetFormatter : MemoryPackFormatter<DateTimeOffset>
{
    public override void Serialize(ref MemoryPackWriter writer, ref DateTimeOffset? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
            return;
        }
        
        writer.WriteUnmanaged(value.Value.Ticks);
        writer.WriteUnmanaged(value.Value.Offset.Ticks);
    }
    
    public override void Deserialize(ref MemoryPackReader reader, ref DateTimeOffset? value)
    {
        if (!reader.TryReadObjectHeader(out var count))
        {
            value = null;
            return;
        }
        
        reader.ReadUnmanaged(out long ticks);
        reader.ReadUnmanaged(out long offsetTicks);
        
        value = new DateTimeOffset(ticks, new TimeSpan(offsetTicks));
    }
}
```

---

## 性能优化建议

### 1. 使用非托管类型

对于性能关键的类型，尽量使用非托管类型（结构体、整数、浮点数等），可以获得零拷贝性能。

### 2. 避免不必要的选项

使用默认选项可以获得最佳性能，只有在需要时才指定选项。

### 3. 复用缓冲区

对于频繁序列化的场景，可以复用 IBufferWriter 实例。

### 4. 使用对象池

对于高并发场景，可以考虑使用对象池来减少内存分配。

### 5. 避免循环引用

如果不需要循环引用支持，使用 Object 模式而不是 CircularReference 模式。

---

## 注意事项

1. **代码生成**：MemoryPack 使用源代码生成器，需要编译时生成序列化代码。
2. **部分类**：使用 `[MemoryPackable]` 的类型必须是 `partial class`。
3. **版本兼容**：使用 VersionTolerant 模式时，字段顺序变化不会影响兼容性。
4. **循环引用**：使用 CircularReference 模式时，性能会略有下降。
5. **Unity 支持**：Unity 类型需要 Unity 2021.2 或更高版本。
6. **.NET 版本**：某些高级功能（如 IMemoryPackable）需要 .NET 7 或更高版本。

---

## 总结

MemoryPack 提供了丰富的 API 来满足各种序列化需求。通过合理使用这些 API，可以构建高性能、类型安全的序列化系统。本手册涵盖了所有主要的 API 使用方法，可以作为开发参考。

