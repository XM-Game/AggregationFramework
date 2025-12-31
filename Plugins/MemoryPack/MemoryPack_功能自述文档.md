# MemoryPack 序列化框架功能自述文档

## 概述

MemoryPack 是一个专为 C# 和 Unity 设计的高性能二进制序列化框架。它通过零编码技术和极致性能优化，为应用程序提供了快速、高效的序列化和反序列化能力。MemoryPack 的设计理念是性能优先，通过直接内存操作、零拷贝技术、SIMD 优化等手段，实现了比传统序列化框架快数倍甚至数十倍的性能。

## 核心特性

### 一、极致性能

MemoryPack 的核心优势在于其卓越的性能表现：

#### 1. 零拷贝序列化
对于非托管类型（如整数、浮点数、结构体等），MemoryPack 直接进行内存拷贝，无需任何中间转换或编码过程。这种零拷贝技术使得序列化速度接近内存拷贝的理论极限。

#### 2. 直接内存操作
框架大量使用 `System.Runtime.CompilerServices.Unsafe` 和 `MemoryMarshal` 进行直接内存操作，避免了装箱、拆箱、反射等性能开销。

#### 3. 缓冲区复用
使用线程静态状态和对象池机制，减少内存分配和垃圾回收压力。每次序列化操作都会复用已分配的缓冲区，显著提升性能。

#### 4. 内联优化
关键路径的方法都使用内联优化，减少函数调用开销。大量使用 `MethodImplOptions.AggressiveInlining` 特性。

#### 5. SIMD 优化
在支持的平台上，MemoryPack 会使用 SIMD 指令加速字符串和数组操作，进一步提升性能。

#### 6. 非托管类型优化
对于非托管类型数组，MemoryPack 提供了专门的优化路径，直接进行块拷贝，性能接近原生内存操作。

### 二、类型支持

MemoryPack 提供了广泛的类型支持：

#### 1. 基础类型
支持所有 .NET 基础类型，包括整数类型（byte, sbyte, short, ushort, int, uint, long, ulong）、浮点类型（float, double, decimal）、布尔类型、字符类型、日期时间类型、GUID 等。

#### 2. 字符串类型
支持 UTF-8 和 UTF-16 两种编码方式，可以根据需要选择最适合的编码。还支持字符串内化、压缩等高级功能。

#### 3. 集合类型
完整支持所有常见集合类型：
- 数组（一维、多维）
- 列表（List<T>）
- 字典（Dictionary<TKey, TValue>）
- 集合（HashSet<T>, SortedSet<T>）
- 队列和栈
- 元组（Tuple）
- 键值对（KeyValuePair）

#### 4. 可空类型
完整支持可空值类型（Nullable<T>）的序列化。

#### 5. 自定义类型
通过属性和接口，可以轻松定义自定义类型的序列化行为。

#### 6. 泛型类型
完整支持泛型类型的序列化，包括开放泛型和封闭泛型。

#### 7. Unity 类型
针对 Unity 平台，提供了特殊类型的支持，如 AnimationCurve、Gradient、RectOffset 等。

### 三、高级功能

MemoryPack 提供了丰富的高级功能：

#### 1. 版本容错
支持向后兼容的版本升级。当数据结构发生变化时，旧版本的数据仍然可以正确反序列化，新字段会被忽略或使用默认值。

#### 2. 循环引用支持
可以序列化包含循环引用的对象图。框架会自动检测和处理循环引用，避免无限递归。

#### 3. 多态序列化
通过联合类型（Union Type）支持多态序列化。可以在序列化时保存类型信息，反序列化时自动恢复正确的类型。

#### 4. 自定义格式化器
允许为特定类型定义自定义序列化逻辑。可以通过实现接口或使用属性来指定自定义格式化器。

#### 5. 压缩支持
内置了 Brotli 压缩格式化器，可以对特定字段或类型进行压缩，减少序列化数据的大小。

#### 6. 位打包
提供了位打包格式化器，可以将布尔数组压缩为位数组，大幅减少数据大小。

#### 7. 字符串内化
支持字符串内化，可以自动将重复的字符串引用到字符串池中，减少内存占用。

### 四、序列化模式

MemoryPack 支持多种序列化模式：

#### 1. 对象模式（Object）
标准的对象序列化模式，适用于大多数场景。使用顺序布局，性能最优。

#### 2. 版本容错模式（VersionTolerant）
支持向后兼容的版本升级。使用显式布局，可以处理字段顺序变化和字段增减。

#### 3. 循环引用模式（CircularReference）
支持包含循环引用的对象图序列化。使用显式布局和引用跟踪。

#### 4. 集合模式（Collection）
专门为集合类型优化的序列化模式。

#### 5. 不生成模式（NoGenerate）
不自动生成序列化代码，完全依赖自定义格式化器。

### 五、序列化布局

MemoryPack 支持两种序列化布局：

#### 1. 顺序布局（Sequential）
按照字段在类中的声明顺序进行序列化。性能最优，但不支持版本容错。

#### 2. 显式布局（Explicit）
使用字段名或顺序号进行序列化。支持版本容错，但性能略低。

### 六、属性系统

MemoryPack 提供了丰富的属性来控制序列化行为：

#### 1. 类型级别属性
- `MemoryPackableAttribute`：标记类型为可序列化
- `MemoryPackUnionAttribute`：定义联合类型
- `GenerateTypeScriptAttribute`：生成 TypeScript 类型定义

#### 2. 成员级别属性
- `MemoryPackIgnoreAttribute`：忽略字段或属性
- `MemoryPackIncludeAttribute`：显式包含字段或属性
- `MemoryPackOrderAttribute`：指定序列化顺序
- `MemoryPackAllowSerializeAttribute`：允许序列化特定成员
- `MemoryPackConstructorAttribute`：标记用于反序列化的构造函数

#### 3. 生命周期属性
- `MemoryPackOnSerializingAttribute`：序列化前调用
- `MemoryPackOnSerializedAttribute`：序列化后调用
- `MemoryPackOnDeserializingAttribute`：反序列化前调用
- `MemoryPackOnDeserializedAttribute`：反序列化后调用

#### 4. 自定义格式化器属性
- `MemoryPackCustomFormatterAttribute`：指定自定义格式化器
- `Utf8StringFormatterAttribute`：使用 UTF-8 编码
- `Utf16StringFormatterAttribute`：使用 UTF-16 编码
- `InternStringFormatterAttribute`：字符串内化
- `BrotliFormatterAttribute`：Brotli 压缩
- `BitPackFormatterAttribute`：位打包

### 七、格式化器系统

MemoryPack 提供了强大的格式化器系统：

#### 1. 接口定义
- `IMemoryPackable<T>`：实现此接口可以定义自己的序列化逻辑（.NET 7+）
- `IMemoryPackFormatter`：非泛型格式化器接口
- `IMemoryPackFormatter<T>`：泛型格式化器接口
- `MemoryPackFormatter<T>`：抽象基类

#### 2. 格式化器提供者
`MemoryPackFormatterProvider` 负责管理和查找类型对应的格式化器：
- 自动注册所有已知类型的格式化器
- 为未注册的类型动态创建格式化器
- 支持泛型类型的格式化器创建和缓存
- 提供注册自定义格式化器的接口

#### 3. 内置格式化器
MemoryPack 提供了大量内置格式化器，覆盖所有常见类型：
- 基础类型格式化器
- 集合类型格式化器
- 特殊类型格式化器（Uri、TimeZoneInfo、BigInteger 等）
- Unity 类型格式化器

### 八、压缩功能

MemoryPack 提供了压缩相关的功能：

#### 1. Brotli 压缩
支持 Brotli 算法的压缩和解压缩，可以显著减少序列化数据的大小。

#### 2. 压缩格式化器
为特定类型提供压缩格式化器，可以在序列化时自动压缩数据。

#### 3. 压缩级别配置
支持不同的压缩级别和窗口大小配置，可以根据需要在压缩率和性能之间平衡。

#### 4. 解压缩大小限制
防止解压缩时内存溢出，可以设置解压缩大小的上限。

### 九、Unity 集成

MemoryPack 针对 Unity 平台进行了特殊优化：

#### 1. Unity 类型支持
提供了 Unity 特有类型的格式化器，如 AnimationCurve、Gradient、RectOffset 等。

#### 2. 代码生成
通过源代码生成器自动生成序列化代码，无需运行时反射，性能最优。

#### 3. IL2CPP 兼容
完全兼容 IL2CPP，可以在所有 Unity 支持的平台上使用。

#### 4. AOT 支持
支持 AOT 编译，可以在不支持 JIT 的平台上使用。

### 十、性能优化技术

MemoryPack 使用了多种性能优化技术：

#### 1. 线程静态状态
使用线程静态变量存储序列化状态，避免线程同步开销。

#### 2. 对象池
使用对象池复用缓冲区写入器和读取器，减少内存分配。

#### 3. 变长整数编码
使用 VarInt 编码优化整数类型的序列化大小，小数值使用单字节编码。

#### 4. 深度限制
内置递归深度检查，防止循环引用导致的无限递归。

#### 5. 缓冲区管理
智能管理缓冲区分配和扩展，支持 IBufferWriter 接口，可以写入到任何缓冲区。

#### 6. 跨段数据读取
智能处理跨段数据读取，自动管理临时缓冲区，支持 ReadOnlySequence。

## 技术架构

### 1. 核心组件

#### MemoryPackSerializer
主要的序列化器类，提供了序列化和反序列化的入口方法。支持同步和异步操作，支持泛型和非泛型调用。

#### MemoryPackWriter
序列化过程中的核心写入器，负责将数据写入到缓冲区。提供了丰富的写入方法，支持对象、集合、字符串等各种类型的写入。

#### MemoryPackReader
反序列化过程中的核心读取器，负责从字节流中读取数据。提供了丰富的读取方法，支持对象、集合、字符串等各种类型的读取。

#### MemoryPackFormatterProvider
格式化器提供者，负责管理和查找类型对应的格式化器。维护了类型到格式化器的映射缓存。

### 2. 内部工具

#### 缓冲区管理
- `ReusableLinkedArrayBufferWriter`：可重用的链接数组缓冲区写入器
- `FixedArrayBufferWriter`：固定数组缓冲区写入器
- `ReusableReadOnlySequenceBuilder`：可重用的只读序列构建器

#### 内存操作
- `MemoryMarshalEx`：跨平台的内存操作辅助方法
- `MathEx`：数学辅助方法
- `TypeHelpers`：类型检查和元数据查询辅助方法

#### 对象池
- `ReusableLinkedArrayBufferWriterPool`：缓冲区写入器对象池
- `ReusableReadOnlySequenceBuilderPool`：只读序列构建器对象池

### 3. 可选状态管理

#### MemoryPackWriterOptionalState
写入器的可选状态，用于支持高级功能：
- 循环引用支持
- 对象池管理
- 选项传递

#### MemoryPackReaderOptionalState
读取器的可选状态，用于支持高级功能：
- 循环引用支持
- 对象池管理
- 选项传递

## 使用场景

MemoryPack 适用于以下场景：

### 1. 高性能网络通信
游戏服务器、实时通信系统等对性能要求极高的场景。MemoryPack 的零拷贝技术和极致性能优化，可以显著减少网络通信的延迟和 CPU 占用。

### 2. 数据持久化
数据库序列化、文件存储等需要高效序列化的场景。MemoryPack 可以快速将对象序列化为二进制数据，减少 I/O 操作时间。

### 3. 缓存系统
分布式缓存、内存缓存等需要频繁序列化和反序列化的场景。MemoryPack 的高性能可以显著提升缓存系统的吞吐量。

### 4. 消息队列
消息序列化、事件存储等需要高效序列化的场景。MemoryPack 可以快速序列化消息对象，减少消息队列的处理时间。

### 5. Unity 游戏开发
游戏数据序列化、网络同步、存档系统等 Unity 游戏开发场景。MemoryPack 针对 Unity 进行了特殊优化，提供了 Unity 类型支持。

### 6. 微服务通信
微服务之间的数据交换、API 响应序列化等场景。MemoryPack 的高性能可以显著提升微服务的响应速度。

## 设计原则

### 1. 性能优先
所有设计都以性能为第一考虑。通过零拷贝、直接内存操作、内联优化等技术，实现极致性能。

### 2. 类型安全
提供强类型的 API 接口，编译时类型检查，减少运行时错误。

### 3. 易于扩展
提供清晰的扩展点和接口，可以轻松实现自定义格式化器。

### 4. 零依赖
核心库不依赖外部库，只依赖 .NET 标准库。

### 5. 跨平台
支持所有 .NET 支持的平台，包括 .NET Framework、.NET Core、.NET 5+、Unity 等。

### 6. 向后兼容
支持版本容错，可以处理数据结构的变化，保持向后兼容。

## 性能对比

根据官方基准测试，MemoryPack 的性能表现：

- 比 MessagePack 快 2-3 倍
- 比 System.Text.Json 快 3-5 倍
- 比 Protobuf 快 2-4 倍
- 比 BinaryFormatter 快 10-20 倍

对于非托管类型，性能接近内存拷贝的理论极限。

## 总结

MemoryPack 是一个高性能、功能丰富的二进制序列化框架。它通过精心设计的架构和优化，为应用程序提供了快速、高效的序列化能力。无论是简单的数据传输还是复杂的对象图序列化，MemoryPack 都能提供优秀的性能和灵活性。对于追求极致性能的场景，MemoryPack 是一个理想的选择。

