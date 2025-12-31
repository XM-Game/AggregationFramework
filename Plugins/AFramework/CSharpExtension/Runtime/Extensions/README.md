# AFramework C# 扩展方法库

本模块提供了丰富的 C# 扩展方法，涵盖基础类型、集合、IO、反射等常用操作。

## 目录结构

```
Extensions/
├── Collections/          # 集合扩展
│   ├── ArrayExtensions.cs
│   ├── CollectionExtensions.cs
│   ├── DictionaryExtensions.cs
│   ├── EnumerableExtensions.cs
│   ├── HashSetExtensions.cs
│   ├── ListExtensions.cs
│   ├── QueueExtensions.cs
│   ├── ReadOnlyExtensions.cs
│   ├── SpanExtensions.cs
│   └── StackExtensions.cs
│
├── DateTime/             # 日期时间扩展
│   ├── DateOnlyExtensions.cs
│   ├── DateTimeExtensions.cs
│   ├── TimeOnlyExtensions.cs
│   └── TimeSpanExtensions.cs
│
├── IO/                   # IO 扩展
│   ├── StreamExtensions.cs
│   ├── BinaryReaderExtensions.cs
│   ├── BinaryWriterExtensions.cs
│   ├── PathExtensions.cs
│   └── FileInfoExtensions.cs
│
├── Primitives/           # 基元类型扩展
│   ├── BoolExtensions.cs
│   ├── ByteExtensions.cs
│   ├── CharExtensions.cs
│   ├── DecimalExtensions.cs
│   ├── DoubleExtensions.cs
│   ├── FloatExtensions.cs
│   ├── IntExtensions.cs
│   └── LongExtensions.cs
│
├── Reflection/           # 反射扩展
│   ├── MemberInfoExtensions.cs
│   ├── MethodInfoExtensions.cs
│   ├── PropertyInfoExtensions.cs
│   ├── FieldInfoExtensions.cs
│   └── AssemblyExtensions.cs
│
├── Strings/              # 字符串扩展
│   ├── StringExtensions.cs
│   ├── StringBuilderExtensions.cs
│   ├── StringConversionExtensions.cs
│   ├── StringFormatExtensions.cs
│   ├── StringSearchExtensions.cs
│   └── StringValidationExtensions.cs
│
└── Types/                # 类型扩展
    ├── TypeExtensions.cs
    ├── ObjectExtensions.cs
    ├── EnumExtensions.cs
    ├── DelegateExtensions.cs
    ├── ExceptionExtensions.cs
    └── AttributeExtensions.cs
```

## 使用示例

### Types 扩展

```csharp
using AFramework.CSharpExtension;

// TypeExtensions
typeof(int).IsNumeric();           // true
typeof(List<int>).IsCollection();  // true
typeof(int?).IsNullable();         // true
typeof(MyClass).GetFriendlyName(); // "MyClass"

// DelegateExtensions
Action action = () => DoSomething();
action.SafeInvoke();               // 安全调用，忽略异常
action.InvokeWithRetry(3, 100);    // 重试3次，间隔100ms

// ExceptionExtensions
exception.GetInnermostException(); // 获取最内层异常
exception.GetFullMessage();        // 获取完整异常消息
exception.ToLogString();           // 转换为日志友好字符串

// AttributeExtensions
memberInfo.HasAttribute<MyAttribute>();
memberInfo.GetAttribute<DescriptionAttribute>();
type.GetMembersWithAttribute<SerializeField>();
```

### IO 扩展

```csharp
using AFramework.CSharpExtension;

// StreamExtensions
stream.ReadAllBytes();             // 读取所有字节
stream.ReadAsString();             // 读取为字符串
stream.WriteString("Hello");       // 写入字符串
stream.Reset();                    // 重置位置

// BinaryReaderExtensions
reader.ReadInt32Safe();            // 安全读取
reader.ReadDateTime();             // 读取 DateTime
reader.ReadStringArrayWithLength(); // 读取带长度前缀的数组

// PathExtensions
path.CombinePath("sub", "file.txt");
path.GetFileNameWithoutExtension();
path.ChangeExtension(".json");
path.IsValidPath();
path.ToUnixPath();

// FileInfoExtensions
file.ReadAllText();
file.ComputeMD5Hash();
file.GetFileSizeString();          // "1.5 MB"
```

### Reflection 扩展

```csharp
using AFramework.CSharpExtension;

// MemberInfoExtensions
member.GetMemberType();            // 获取成员类型
member.IsPublic();                 // 检查是否公共
member.GetValue(obj);              // 获取值
member.SetValue(obj, value);       // 设置值

// MethodInfoExtensions
method.InvokeSafe(obj, args);      // 安全调用
method.GetParameterTypes();        // 获取参数类型
method.IsAsyncMethod();            // 检查是否异步
method.CreateAction(target);       // 创建委托

// PropertyInfoExtensions
property.GetValueSafe(obj);        // 安全获取值
property.HasPublicGetter();        // 检查公共 getter
property.IsReadOnly();             // 检查只读

// FieldInfoExtensions
field.GetValue<int>(obj);          // 获取指定类型值
field.IsBackingField();            // 检查是否后备字段
field.IsWritable();                // 检查是否可写

// AssemblyExtensions
assembly.GetTypesImplementing<IService>();
assembly.GetTypesWithAttribute<MyAttribute>();
assembly.ReadEmbeddedResourceAsString("config.json");
```

## 设计原则

1. **零分配优先**：尽可能避免堆分配，使用 `AggressiveInlining` 优化
2. **安全调用**：提供 Safe 版本方法，捕获异常返回默认值
3. **链式调用**：支持流畅的链式 API 设计
4. **空值安全**：所有方法都处理 null 输入
5. **性能优化**：热点方法使用内联优化
