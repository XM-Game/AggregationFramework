# MemoryPack.Core 功能自述

## 概述

MemoryPack.Core 是 MemoryPack 序列化框架的核心运行时库，提供了高性能的二进制序列化和反序列化功能。该库专为追求极致性能的场景设计，通过零拷贝、内存直接操作等技术实现高效的序列化处理。

## 核心组件

### 序列化器（MemoryPackSerializer）

MemoryPackSerializer 是框架的主要入口点，提供了多种序列化和反序列化方法：

- **同步序列化**：支持将对象序列化为字节数组或写入到 IBufferWriter
- **异步序列化**：支持异步流式序列化，适用于网络传输和文件操作
- **非泛型序列化**：支持基于 Type 类型的动态序列化
- **泛型序列化**：提供类型安全的泛型序列化方法

序列化器内部使用线程静态状态来优化性能，避免重复分配缓冲区，并支持自定义序列化选项配置。

### 写入器（MemoryPackWriter）

MemoryPackWriter 是序列化过程中的核心写入器，负责将数据写入到缓冲区：

- **缓冲区管理**：智能管理缓冲区分配和扩展，支持 IBufferWriter 接口
- **对象序列化**：支持对象头、成员计数、引用ID等元数据的写入
- **集合序列化**：支持数组、列表等集合类型的序列化，包括空集合和空值处理
- **字符串序列化**：支持 UTF-8 和 UTF-16 两种编码方式
- **非托管类型优化**：针对非托管类型提供零拷贝的高性能序列化路径
- **深度限制**：内置递归深度检查，防止循环引用导致的无限递归

写入器提供了丰富的写入方法，包括对象头、联合类型头、集合头等，并支持变长整数编码以优化数据大小。

### 读取器（MemoryPackReader）

MemoryPackReader 是反序列化过程中的核心读取器，负责从字节流中读取数据：

- **序列读取**：支持从 ReadOnlySequence 和 ReadOnlySpan 读取数据
- **缓冲区管理**：智能处理跨段数据读取，自动管理临时缓冲区
- **对象反序列化**：支持对象头解析、成员计数读取、引用ID解析
- **集合反序列化**：支持数组、列表等集合类型的反序列化
- **字符串反序列化**：自动识别 UTF-8 和 UTF-16 编码并正确解码
- **非托管类型优化**：针对非托管类型提供零拷贝的高性能反序列化路径
- **预览功能**：支持预览下一个数据项而不实际消费数据

读取器提供了丰富的读取方法，包括对象头、联合类型头、集合头等，并支持变长整数解码。

### 格式化器系统（Formatter System）

格式化器系统是 MemoryPack 的扩展机制，允许为特定类型定义自定义序列化行为：

#### 接口定义

- **IMemoryPackable**：实现此接口的类型可以定义自己的序列化逻辑，支持静态抽象方法（.NET 7+）
- **IMemoryPackFormatter**：非泛型格式化器接口，用于动态类型序列化
- **IMemoryPackFormatter<T>**：泛型格式化器接口，提供类型安全的序列化
- **MemoryPackFormatter<T>**：抽象基类，简化自定义格式化器的实现

#### 格式化器提供者（MemoryPackFormatterProvider）

格式化器提供者负责管理和查找类型对应的格式化器：

- **自动注册**：自动注册所有已知类型的格式化器
- **动态创建**：为未注册的类型动态创建格式化器
- **泛型支持**：支持泛型类型的格式化器创建和缓存
- **集合支持**：自动为集合类型创建格式化器
- **扩展点**：提供注册自定义格式化器的接口

格式化器提供者维护了一个类型到格式化器的映射缓存，确保高效的格式化器查找。

### 属性系统（Attributes）

MemoryPack 提供了丰富的属性来控制序列化行为：

#### 类型级别属性

- **MemoryPackableAttribute**：标记类型为可序列化，支持多种生成模式（对象、版本容错、循环引用、集合等）
- **MemoryPackUnionAttribute**：定义联合类型，支持多态序列化
- **MemoryPackUnionFormatterAttribute**：标记联合类型的格式化器
- **GenerateTypeScriptAttribute**：标记需要生成 TypeScript 类型定义

#### 成员级别属性

- **MemoryPackIgnoreAttribute**：标记字段或属性在序列化时忽略
- **MemoryPackIncludeAttribute**：显式包含字段或属性（用于版本容错模式）
- **MemoryPackOrderAttribute**：指定序列化顺序
- **MemoryPackAllowSerializeAttribute**：允许序列化特定成员
- **MemoryPackConstructorAttribute**：标记用于反序列化的构造函数

#### 序列化生命周期属性

- **MemoryPackOnSerializingAttribute**：序列化前调用的方法
- **MemoryPackOnSerializedAttribute**：序列化后调用的方法
- **MemoryPackOnDeserializingAttribute**：反序列化前调用的方法
- **MemoryPackOnDeserializedAttribute**：反序列化后调用的方法

#### 自定义格式化器属性

- **MemoryPackCustomFormatterAttribute**：为字段或属性指定自定义格式化器
- **Utf8StringFormatterAttribute**：使用 UTF-8 编码序列化字符串
- **Utf16StringFormatterAttribute**：使用 UTF-16 编码序列化字符串
- **InternStringFormatterAttribute**：字符串内化格式化器
- **BrotliFormatterAttribute**：Brotli 压缩格式化器
- **BitPackFormatterAttribute**：位打包格式化器

### 序列化选项（MemoryPackSerializerOptions）

序列化选项允许配置序列化行为：

- **字符串编码**：支持 UTF-8 和 UTF-16 两种编码方式
- **服务提供者**：支持依赖注入，允许在序列化过程中访问服务

### 可选状态管理（Optional State）

可选状态用于支持高级序列化特性：

#### 写入器可选状态（MemoryPackWriterOptionalState）

- **循环引用支持**：维护对象到引用ID的映射，支持循环引用的序列化
- **对象池**：提供对象池机制，减少分配开销
- **选项管理**：管理序列化选项的传递

#### 读取器可选状态（MemoryPackReaderOptionalState）

- **循环引用支持**：维护引用ID到对象的映射，支持循环引用的反序列化
- **对象池**：提供对象池机制，减少分配开销
- **选项管理**：管理序列化选项的传递

### 变长整数编码（VarInt）

VarInt 编码用于优化整数类型的序列化大小：

- **智能编码**：根据数值大小自动选择最紧凑的编码方式
- **类型支持**：支持所有整数类型（byte, sbyte, short, ushort, int, uint, long, ulong）
- **范围优化**：小数值使用单字节编码，大数值使用多字节编码

### 异常处理（MemoryPackSerializationException）

MemoryPack 提供了专门的异常类型来处理序列化过程中的错误：

- **参数验证错误**：成员计数、缓冲区范围等验证失败
- **类型注册错误**：未注册的类型或格式化器创建失败
- **数据格式错误**：无效的二进制数据格式
- **编码错误**：UTF-8/UTF-16 编码解码失败
- **压缩错误**：Brotli 压缩解压失败
- **深度限制错误**：递归深度超过限制
- **循环引用错误**：循环引用处理相关错误

### 格式化器实现（Formatters）

MemoryPack.Core 提供了大量内置格式化器：

#### 基础类型格式化器

- **UnmanagedFormatter**：所有非托管类型的格式化器（整数、浮点数、日期时间、GUID 等）
- **StringFormatter**：字符串格式化器
- **NullableFormatter**：可空类型格式化器

#### 集合类型格式化器

- **ArrayFormatter**：数组格式化器
- **GenericCollectionFormatter**：泛型集合格式化器
- **GenericSetFormatter**：泛型集合格式化器
- **GenericDictionaryFormatter**：泛型字典格式化器
- **MultiDimensionalArrayFormatters**：多维数组格式化器（2D、3D、4D）
- **TupleFormatter**：元组格式化器

#### 特殊类型格式化器

- **KeyValuePairFormatter**：键值对格式化器
- **LazyFormatter**：延迟加载类型格式化器
- **VersionFormatter**：版本号格式化器
- **UriFormatter**：URI 格式化器
- **TimeZoneInfoFormatter**：时区信息格式化器
- **BigIntegerFormatter**：大整数格式化器
- **BitArrayFormatter**：位数组格式化器
- **StringBuilderFormatter**：字符串构建器格式化器
- **TypeFormatter**：类型格式化器
- **CultureInfoFormatter**：文化信息格式化器
- **DynamicUnionFormatter**：动态联合类型格式化器

### 压缩功能（Compression）

MemoryPack 提供了压缩相关的功能：

- **Brotli 压缩**：支持 Brotli 算法的压缩和解压缩
- **压缩格式化器**：为特定类型提供压缩格式化器
- **压缩级别配置**：支持不同的压缩级别和窗口大小配置
- **解压缩大小限制**：防止解压缩时内存溢出

### 内部工具（Internal）

内部工具类提供了框架运行所需的基础功能：

- **内存操作扩展**：跨平台的内存操作辅助方法
- **类型辅助**：类型检查和元数据查询辅助方法
- **数学扩展**：数组容量计算等数学辅助方法
- **缓冲区写入器**：可重用的缓冲区写入器实现
- **序列构建器**：可重用的只读序列构建器实现
- **保留属性**：用于代码链接的属性标记

## 主要特性

### 性能优化

- **零拷贝序列化**：对于非托管类型，直接进行内存拷贝，无需中间转换
- **缓冲区复用**：使用线程静态状态和对象池减少内存分配
- **内联优化**：大量使用内联方法减少函数调用开销
- **SIMD 优化**：在支持的平台上使用 SIMD 指令加速操作

### 类型支持

- **基础类型**：支持所有 .NET 基础类型
- **集合类型**：支持数组、列表、字典、集合等所有常见集合类型
- **自定义类型**：通过属性和接口支持自定义类型的序列化
- **泛型类型**：完整支持泛型类型的序列化
- **可空类型**：支持可空值类型的序列化

### 高级功能

- **版本容错**：支持向后兼容的版本升级
- **循环引用**：支持包含循环引用的对象图序列化
- **多态序列化**：通过联合类型支持多态序列化
- **自定义格式化器**：允许为特定类型定义自定义序列化逻辑
- **压缩支持**：内置压缩格式化器支持

### 平台兼容性

- **.NET 标准 2.1+**：支持 .NET Standard 2.1 及更高版本
- **.NET 7+ 优化**：在 .NET 7+ 上提供额外的性能优化
- **Unity 支持**：针对 Unity 平台进行了特殊优化和适配

## 使用场景

MemoryPack.Core 适用于以下场景：

- **高性能网络通信**：游戏服务器、实时通信系统
- **数据持久化**：数据库序列化、文件存储
- **缓存系统**：分布式缓存、内存缓存
- **消息队列**：消息序列化、事件存储
- **Unity 游戏开发**：游戏数据序列化、网络同步

## 设计原则

- **性能优先**：所有设计都以性能为第一考虑
- **类型安全**：提供强类型的 API 接口
- **易于扩展**：提供清晰的扩展点和接口
- **零依赖**：核心库不依赖外部库
- **跨平台**：支持所有 .NET 支持的平台

## 总结

MemoryPack.Core 是一个高性能、功能丰富的二进制序列化框架核心库。它通过精心设计的架构和优化，为应用程序提供了快速、高效的序列化能力。无论是简单的数据传输还是复杂的对象图序列化，MemoryPack.Core 都能提供优秀的性能和灵活性。

