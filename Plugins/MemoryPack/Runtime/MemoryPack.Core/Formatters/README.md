# MemoryPack.Formatters 功能自述

## 概述

Formatters 文件夹包含了 MemoryPack 序列化框架中所有内置格式化器的实现。格式化器是序列化系统的核心组件，负责将特定类型的数据转换为二进制格式，以及从二进制格式还原数据。这些格式化器针对不同的数据类型进行了优化，提供了高效、类型安全的序列化能力。

## 格式化器分类

### 基础类型格式化器

#### UnmanagedFormatter（非托管类型格式化器）

非托管类型格式化器用于处理所有非托管类型（值类型）的序列化。非托管类型包括：
- 所有基础数值类型（整数、浮点数、布尔值等）
- 枚举类型
- 指针类型
- 仅包含非托管类型字段的用户定义结构体

该格式化器使用零拷贝技术，直接进行内存操作，提供最高的序列化性能。对于非托管类型，序列化过程无需任何中间转换，直接将内存中的字节复制到输出缓冲区。

#### DangerousUnmanagedFormatter（危险非托管类型格式化器）

这是非托管类型格式化器的变体，用于处理可能包含引用但被标记为非托管的类型。使用此格式化器需要谨慎，因为它绕过了类型安全检查。

### 字符串格式化器

#### StringFormatter（字符串格式化器）

默认的字符串格式化器，根据序列化选项自动选择 UTF-8 或 UTF-16 编码方式。这是最常用的字符串格式化器，适用于大多数场景。

#### Utf8StringFormatter（UTF-8 字符串格式化器）

强制使用 UTF-8 编码序列化字符串。UTF-8 编码通常比 UTF-16 更节省空间，特别适合包含大量 ASCII 字符的字符串。序列化格式包含 UTF-8 字节长度、UTF-16 字符长度和 UTF-8 字节数据。

#### Utf16StringFormatter（UTF-16 字符串格式化器）

强制使用 UTF-16 编码序列化字符串。UTF-16 编码对于包含大量非 ASCII 字符的字符串可能更高效。序列化格式包含字符长度和 UTF-16 字节数据。

#### InternStringFormatter（字符串内化格式化器）

在反序列化时对字符串进行内化处理。字符串内化可以将相同的字符串实例合并，节省内存空间。适用于需要大量重复字符串的场景。

#### BrotliStringFormatter（Brotli 压缩字符串格式化器）

使用 Brotli 压缩算法压缩字符串后再序列化。该格式化器支持配置压缩级别、窗口大小和解压缩大小限制。适用于需要传输大量文本数据的场景，可以显著减少数据大小。

### 可空类型格式化器

#### NullableFormatter（可空类型格式化器）

处理可空值类型的序列化。对于非托管类型的可空值，使用优化的非托管序列化路径；对于包含引用的可空值，使用对象头标记空值状态。该格式化器确保可空类型的序列化与对应的非可空类型保持兼容。

### 数组格式化器

#### ArrayFormatter（数组格式化器）

处理一维数组的序列化。支持任意元素类型的数组，包括引用类型和值类型。序列化格式包含数组长度和所有元素的数据。

#### UnmanagedArrayFormatter（非托管数组格式化器）

专门处理非托管类型数组的格式化器。使用零拷贝技术，直接将数组的内存内容复制到输出缓冲区，提供最高的性能。

#### DangerousUnmanagedArrayFormatter（危险非托管数组格式化器）

非托管数组格式化器的变体，绕过类型安全检查，需要谨慎使用。

#### MultiDimensionalArrayFormatters（多维数组格式化器）

支持二维、三维和四维数组的序列化：
- **TwoDimensionalArrayFormatter**：处理二维数组（T[,]）
- **ThreeDimensionalArrayFormatter**：处理三维数组（T[,,]）
- **FourDimensionalArrayFormatter**：处理四维数组（T[,,,]）

多维数组的序列化格式包含各维度的长度信息和所有元素的数据。对于非托管类型，使用零拷贝优化。

### 数组类似类型格式化器

#### ArraySegmentFormatter（数组段格式化器）

处理 ArraySegment 类型的序列化。ArraySegment 表示数组的一个连续段，序列化时只序列化段内的数据。

#### MemoryFormatter（内存格式化器）

处理 Memory 类型的序列化。Memory 类型表示连续内存区域的可变视图，序列化时序列化内存中的所有数据。

#### ReadOnlyMemoryFormatter（只读内存格式化器）

处理 ReadOnlyMemory 类型的序列化。ReadOnlyMemory 表示连续内存区域的只读视图。

#### ReadOnlySequenceFormatter（只读序列格式化器）

处理 ReadOnlySequence 类型的序列化。ReadOnlySequence 可以表示跨多个内存段的数据序列，支持单段和多段两种序列化模式。

#### MemoryPoolFormatter（内存池格式化器）

处理从内存池分配的内存格式化器。反序列化时从 ArrayPool 租用内存，适用于需要频繁分配和释放内存的场景。

#### ReadOnlyMemoryPoolFormatter（只读内存池格式化器）

处理只读内存池的格式化器，功能与 MemoryPoolFormatter 类似，但返回只读视图。

### 集合类型格式化器

#### CollectionFormatters（集合格式化器集合）

提供了大量 .NET 集合类型的格式化器实现：

**列表类型**：
- **ListFormatter**：处理 List 集合，支持可重用和 Span 优化
- **StackFormatter**：处理 Stack 栈集合
- **QueueFormatter**：处理 Queue 队列集合
- **LinkedListFormatter**：处理 LinkedList 链表集合

**集合类型**：
- **HashSetFormatter**：处理 HashSet 哈希集合
- **SortedSetFormatter**：处理 SortedSet 有序集合
- **PriorityQueueFormatter**：处理 PriorityQueue 优先队列（.NET 7+）

**观察集合类型**：
- **ObservableCollectionFormatter**：处理 ObservableCollection 可观察集合
- **CollectionFormatter**：处理 Collection 基础集合

**并发集合类型**：
- **ConcurrentQueueFormatter**：处理 ConcurrentQueue 并发队列
- **ConcurrentStackFormatter**：处理 ConcurrentStack 并发栈
- **ConcurrentBagFormatter**：处理 ConcurrentBag 并发包
- **ConcurrentDictionaryFormatter**：处理 ConcurrentDictionary 并发字典

**只读集合类型**：
- **ReadOnlyCollectionFormatter**：处理 ReadOnlyCollection 只读集合
- **ReadOnlyObservableCollectionFormatter**：处理 ReadOnlyObservableCollection 只读可观察集合
- **BlockingCollectionFormatter**：处理 BlockingCollection 阻塞集合

**字典类型**：
- **DictionaryFormatter**：处理 Dictionary 字典
- **SortedDictionaryFormatter**：处理 SortedDictionary 有序字典
- **SortedListFormatter**：处理 SortedList 有序列表

所有集合格式化器都支持空值处理，并且在 .NET 7+ 上使用 Span 优化提升性能。

#### GenericCollectionFormatters（泛型集合格式化器）

提供通用的集合格式化器，可以处理任何实现了 ICollection、ISet 或 IDictionary 接口的泛型集合类型：

- **GenericCollectionFormatter**：处理实现了 ICollection 接口的泛型集合
- **GenericSetFormatter**：处理实现了 ISet 接口的泛型集合
- **GenericDictionaryFormatter**：处理实现了 IDictionary 接口的泛型字典

这些格式化器允许为自定义集合类型提供序列化支持，无需编写专门的格式化器。

#### InterfaceCollectionFormatters（接口集合格式化器）

处理集合接口类型的序列化，包括：

- **InterfaceEnumerableFormatter**：处理 IEnumerable 接口
- **InterfaceCollectionFormatter**：处理 ICollection 接口
- **InterfaceReadOnlyCollectionFormatter**：处理 IReadOnlyCollection 接口
- **InterfaceListFormatter**：处理 IList 接口
- **InterfaceReadOnlyListFormatter**：处理 IReadOnlyList 接口
- **InterfaceDictionaryFormatter**：处理 IDictionary 接口
- **InterfaceReadOnlyDictionaryFormatter**：处理 IReadOnlyDictionary 接口
- **InterfaceLookupFormatter**：处理 ILookup 接口
- **InterfaceGroupingFormatter**：处理 IGrouping 接口
- **InterfaceSetFormatter**：处理 ISet 接口
- **InterfaceReadOnlySetFormatter**：处理 IReadOnlySet 接口（.NET 7+）

这些格式化器在序列化时会优化处理数组和 List 类型，反序列化时通常返回 List 或 Dictionary 实例。

### 元组格式化器

#### TupleFormatter（元组格式化器）

支持所有 .NET 元组类型的序列化，包括：
- **Tuple** 类型（引用类型元组）：支持 1 到 8 个元素的元组
- **ValueTuple** 类型（值类型元组）：支持 1 到 8 个元素的元组

元组格式化器将每个元组元素按顺序序列化，使用对象头标记元素数量。反序列化时验证元素数量以确保数据完整性。

### 键值对格式化器

#### KeyValuePairFormatter（键值对格式化器）

处理 KeyValuePair 类型的序列化。该格式化器主要用于字典序列化的内部实现，但也可以直接使用。对于非托管的键值对类型，使用零拷贝优化。

### 特殊类型格式化器

#### LazyFormatter（延迟加载类型格式化器）

处理 Lazy 类型的序列化。Lazy 类型表示延迟初始化的值，序列化时会立即计算并序列化值，反序列化时创建新的 Lazy 实例。

#### BigIntegerFormatter（大整数格式化器）

处理 BigInteger 类型的序列化。BigInteger 可以表示任意大小的整数。序列化格式包含字节数组长度和字节数据。在支持的平台上使用栈分配优化。

#### BitArrayFormatter（位数组格式化器）

处理 BitArray 类型的序列化。BitArray 表示紧凑的位值数组。序列化格式包含位数组长度和内部整数数组数据。使用不安全代码直接访问 BitArray 的内部结构以提升性能。

#### StringBuilderFormatter（字符串构建器格式化器）

处理 StringBuilder 类型的序列化。StringBuilder 用于高效构建字符串。序列化时使用 UTF-16 编码，支持分块写入以处理大型字符串。反序列化时预分配容量以提升性能。

#### VersionFormatter（版本号格式化器）

处理 Version 类型的序列化。Version 表示程序集或组件的版本号。序列化格式包含主版本号、次版本号、内部版本号和修订版本号。反序列化时正确处理 -1 值（表示未指定的版本组件）。

#### UriFormatter（URI 格式化器）

处理 Uri 类型的序列化。Uri 表示统一资源标识符。序列化时使用原始字符串（OriginalString），反序列化时使用相对或绝对 URI 模式创建 Uri 实例。

#### TimeZoneInfoFormatter（时区信息格式化器）

处理 TimeZoneInfo 类型的序列化。TimeZoneInfo 表示时区信息。序列化时使用 ToSerializedString 方法转换为字符串，反序列化时使用 FromSerializedString 方法还原。

#### TypeFormatter（类型格式化器）

处理 Type 类型的序列化。Type 表示类型声明。序列化时移除程序集限定名中的版本号、文化和公钥令牌信息，只保留类型名和程序集名。反序列化时使用 Type.GetType 方法还原类型。

#### CultureInfoFormatter（文化信息格式化器）

处理 CultureInfo 类型的序列化。CultureInfo 表示特定文化的信息。序列化时使用文化名称（Name），反序列化时使用 GetCultureInfo 方法还原。

### 接口格式化器

#### MemoryPackableFormatter（MemoryPackable 接口格式化器）

处理实现了 IMemoryPackable 接口的类型的序列化。该格式化器直接调用类型的静态序列化方法，提供最高的性能。仅在 .NET 7+ 上可用，因为需要静态抽象接口支持。

#### DynamicUnionFormatter（动态联合类型格式化器）

处理动态联合类型的序列化。联合类型支持多态序列化，可以在运行时根据实际类型选择不同的序列化策略。该格式化器通过标签（Tag）和类型的映射关系实现多态序列化。

## 格式化器特性

### 性能优化

- **零拷贝序列化**：对于非托管类型，直接进行内存拷贝，无需中间转换
- **Span 优化**：在 .NET 7+ 上使用 Span 和 CollectionsMarshal 优化集合操作
- **内联优化**：大量使用内联方法减少函数调用开销
- **缓冲区复用**：集合格式化器支持重用现有集合实例，减少内存分配

### 类型安全

- **泛型支持**：所有格式化器都使用泛型提供类型安全
- **空值处理**：所有格式化器都正确处理空值情况
- **类型验证**：反序列化时验证数据格式和类型匹配

### 兼容性

- **版本兼容**：格式化器设计考虑了向后兼容性
- **平台支持**：针对不同 .NET 版本提供优化实现
- **Unity 支持**：考虑了 Unity 平台的特殊需求

## 使用场景

不同的格式化器适用于不同的使用场景：

- **基础类型格式化器**：适用于所有基础数据类型的序列化
- **字符串格式化器**：根据数据特点选择编码方式或压缩
- **数组格式化器**：适用于需要高效处理数组数据的场景
- **集合格式化器**：适用于各种集合数据结构的序列化
- **特殊类型格式化器**：适用于特定类型的序列化需求

## 扩展性

虽然 MemoryPack 提供了大量内置格式化器，但框架也支持创建自定义格式化器。对于实现了 IMemoryPackable 接口的类型，可以使用代码生成自动创建格式化器；对于其他类型，可以实现 IMemoryPackFormatter 接口创建自定义格式化器。

## 总结

Formatters 文件夹包含了 MemoryPack 序列化框架的核心格式化器实现。这些格式化器覆盖了 .NET 中大部分常用类型，提供了高效、类型安全的序列化能力。通过精心设计的架构和优化，这些格式化器能够在保证类型安全的同时提供卓越的性能。
