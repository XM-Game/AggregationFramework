# AFramework.Serialization 序列化系统目录结构文档

## 文档信息

| 项目 | 内容 |
|------|------|
| 模块名称 | AFramework.Serialization |
| 版本 | 1.0.0 |
| 适用 Unity 版本 | 2022.3 LTS ~ Unity 6.x |
| 命名空间 | AFramework.Serialization |
| 文档状态 | 设计阶段 |

---

## 第一章：目录结构总览

### 1.1 顶层目录结构

```
Plugins/AFramework/Serialization/
├── Editor/                          # 编辑器扩展模块
│   ├── Inspectors/                  # Inspector 面板扩展
│   ├── Windows/                     # 编辑器窗口
│   ├── Tools/                       # 编辑器工具
│   ├── Analyzers/                   # 代码分析器
│   └── AFramework.Serialization.Editor.asmdef
│
├── Runtime/                         # 运行时核心模块
│   ├── Core/                        # 核心层
│   ├── Serializers/                 # 序列化器实现
│   ├── Formatters/                  # 格式化器
│   ├── Buffers/                     # 缓冲区管理
│   ├── Compression/                 # 压缩模块
│   ├── Security/                    # 安全模块
│   ├── Unity/                       # Unity 集成
│   ├── ECS/                         # ECS 集成
│   ├── Utilities/                   # 工具类
│   └── AFramework.Serialization.asmdef
│
├── Generator/                       # 源代码生成器
│   ├── Analyzers/                   # 代码分析器
│   ├── Generators/                  # 代码生成器
│   ├── Templates/                   # 代码模板
│   └── AFramework.Serialization.Generator.csproj
│
├── Tests/                           # 测试模块
│   ├── Editor/                      # 编辑器测试
│   ├── Runtime/                     # 运行时测试
│   └── Performance/                 # 性能测试
│
├── Samples~/                        # 示例项目
│   ├── BasicUsage/                  # 基础用法示例
│   ├── AdvancedFeatures/            # 高级功能示例
│   ├── NetworkSync/                 # 网络同步示例
│   └── SaveSystem/                  # 存档系统示例
│
├── Documentation~/                  # 文档资源
│   ├── API/                         # API 文档
│   ├── Guides/                      # 使用指南
│   └── Images/                      # 文档图片
│
├── package.json                     # UPM 包配置
├── README.md                        # 模块说明
├── CHANGELOG.md                     # 更新日志
└── LICENSE.md                       # 许可证
```

### 1.2 模块依赖关系图

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        AFramework.Serialization 模块依赖                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    AFramework.Serialization.Editor                       │   │
│  │                         (编辑器扩展模块)                                  │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                      │                                          │
│                                      ▼                                          │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                      AFramework.Serialization                            │   │
│  │                         (运行时核心模块)                                  │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                      │                                          │
│                    ┌─────────────────┼─────────────────┐                        │
│                    ▼                 ▼                 ▼                        │
│  ┌─────────────────────┐  ┌─────────────────┐  ┌─────────────────┐             │
│  │ AFramework.         │  │ AFramework.     │  │ Unity.Burst     │             │
│  │ CSharpExtension     │  │ BurstExtension  │  │ Unity.Entities  │             │
│  │ (C# 扩展模块)       │  │ (Burst 扩展)    │  │ (可选依赖)      │             │
│  └─────────────────────┘  └─────────────────┘  └─────────────────┘             │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 第二章：Runtime 运行时模块详细结构

### 2.1 Core 核心层

```
Runtime/Core/
├── Interfaces/                      # 核心接口定义
│   ├── ISerializer.cs               # 序列化器接口
│   ├── ISerializerT.cs              # 泛型序列化器接口
│   ├── IAsyncSerializer.cs          # 异步序列化器接口
│   ├── IFormatter.cs                # 格式化器接口
│   ├── IFormatterT.cs               # 泛型格式化器接口
│   ├── IFormatterResolver.cs        # 格式化器解析器接口
│   ├── ISerializeWriter.cs          # 序列化写入器接口
│   ├── ISerializeReader.cs          # 序列化读取器接口
│   ├── IBufferPool.cs               # 缓冲区池接口
│   ├── ICompressionProvider.cs      # 压缩提供者接口
│   ├── IEncryptionProvider.cs       # 加密提供者接口
│   └── IVersionMigrator.cs          # 版本迁移器接口
│
├── Abstracts/                       # 抽象基类
│   ├── SerializerBase.cs            # 序列化器基类
│   ├── SerializerBaseT.cs           # 泛型序列化器基类
│   ├── FormatterBase.cs             # 格式化器基类
│   ├── FormatterBaseT.cs            # 泛型格式化器基类
│   ├── FormatterResolverBase.cs     # 格式化器解析器基类
│   └── VersionMigratorBase.cs       # 版本迁移器基类
│
├── Attributes/                      # 特性定义
│   ├── Type/                        # 类型级别特性
│   │   ├── SerializableAttribute.cs         # 可序列化标记
│   │   ├── UnionTypeAttribute.cs            # 联合类型定义
│   │   ├── SerializeVersionAttribute.cs     # 版本标记
│   │   ├── SerializeLayoutAttribute.cs      # 布局方式
│   │   └── GenerateModeAttribute.cs         # 生成模式
│   │
│   ├── Member/                      # 成员级别特性
│   │   ├── SerializeIgnoreAttribute.cs      # 忽略序列化
│   │   ├── SerializeIncludeAttribute.cs     # 包含序列化
│   │   ├── SerializeOrderAttribute.cs       # 序列化顺序
│   │   ├── SerializeRequiredAttribute.cs    # 必需字段
│   │   ├── SerializeDefaultAttribute.cs     # 默认值
│   │   ├── SerializeNameAttribute.cs        # 字段名称
│   │   └── SerializeConstructorAttribute.cs # 构造函数标记
│   │
│   ├── Formatter/                   # 格式化器特性
│   │   ├── CustomFormatterAttribute.cs      # 自定义格式化器
│   │   ├── Utf8StringAttribute.cs           # UTF-8 字符串
│   │   ├── Utf16StringAttribute.cs          # UTF-16 字符串
│   │   ├── InternStringAttribute.cs         # 字符串内化
│   │   ├── CompressAttribute.cs             # 压缩标记
│   │   ├── BitPackAttribute.cs              # 位打包
│   │   └── FixedSizeAttribute.cs            # 固定大小
│   │
│   └── Lifecycle/                   # 生命周期特性
│       ├── OnSerializingAttribute.cs        # 序列化前回调
│       ├── OnSerializedAttribute.cs         # 序列化后回调
│       ├── OnDeserializingAttribute.cs      # 反序列化前回调
│       └── OnDeserializedAttribute.cs       # 反序列化后回调
│
├── Enums/                           # 枚举定义
│   ├── SerializeMode.cs             # 序列化模式
│   ├── GenerateMode.cs              # 生成模式
│   ├── SerializeLayout.cs           # 布局方式
│   ├── CompressionAlgorithm.cs      # 压缩算法
│   ├── CompressionLevel.cs          # 压缩级别
│   ├── EncryptionAlgorithm.cs       # 加密算法
│   ├── ChecksumAlgorithm.cs         # 校验算法
│   ├── StringEncoding.cs            # 字符串编码
│   ├── Endianness.cs                # 字节序
│   └── SerializeErrorCode.cs        # 错误码
│
├── Structs/                         # 结构体定义
│   ├── SerializeOptions.cs          # 序列化选项
│   ├── DeserializeOptions.cs        # 反序列化选项
│   ├── SerializeResult.cs           # 序列化结果
│   ├── DeserializeResult.cs         # 反序列化结果
│   ├── TypeInfo.cs                  # 类型信息
│   ├── MemberInfo.cs                # 成员信息
│   ├── VersionInfo.cs               # 版本信息
│   └── DataHeader.cs                # 数据头信息
│
├── Exceptions/                      # 异常定义
│   ├── SerializationException.cs            # 序列化异常基类
│   ├── FormatterNotFoundException.cs        # 格式化器未找到
│   ├── TypeNotSupportedException.cs         # 类型不支持
│   ├── VersionMismatchException.cs          # 版本不匹配
│   ├── DataCorruptedException.cs            # 数据损坏
│   ├── BufferOverflowException.cs           # 缓冲区溢出
│   ├── CircularReferenceException.cs        # 循环引用
│   └── DeserializationException.cs          # 反序列化异常
│
├── Constants/                       # 常量定义
│   ├── SerializeConstants.cs        # 序列化常量
│   ├── FormatConstants.cs           # 格式常量
│   ├── BufferConstants.cs           # 缓冲区常量
│   └── MagicNumbers.cs              # 魔数定义
│
└── Configuration/                   # 配置管理
    ├── SerializerConfiguration.cs   # 序列化器配置
    ├── FormatterConfiguration.cs    # 格式化器配置
    ├── BufferConfiguration.cs       # 缓冲区配置
    └── GlobalConfiguration.cs       # 全局配置
```



### 2.2 Serializers 序列化器实现

```
Runtime/Serializers/
├── Binary/                          # 二进制序列化器
│   ├── BinarySerializer.cs          # 二进制序列化器主类
│   ├── BinarySerializerT.cs         # 泛型二进制序列化器
│   ├── BinarySerializerOptions.cs   # 二进制序列化选项
│   ├── BinaryFormat.cs              # 二进制格式定义
│   ├── BinaryHeader.cs              # 二进制头信息
│   ├── VarIntEncoder.cs             # 变长整数编码器
│   ├── VarIntDecoder.cs             # 变长整数解码器
│   └── Internal/                    # 内部实现
│       ├── BinaryWriterCore.cs      # 核心写入逻辑
│       ├── BinaryReaderCore.cs      # 核心读取逻辑
│       ├── TypeCodeMap.cs           # 类型码映射
│       └── ObjectTracker.cs         # 对象跟踪器
│
├── Yaml/                            # YAML 序列化器
│   ├── YamlSerializer.cs            # YAML 序列化器主类
│   ├── YamlSerializerT.cs           # 泛型 YAML 序列化器
│   ├── YamlSerializerOptions.cs     # YAML 序列化选项
│   ├── YamlFormat.cs                # YAML 格式定义
│   ├── YamlEmitter.cs               # YAML 发射器
│   ├── YamlParser.cs                # YAML 解析器
│   ├── YamlNode.cs                  # YAML 节点
│   ├── YamlDocument.cs              # YAML 文档
│   └── Internal/                    # 内部实现
│       ├── YamlScanner.cs           # YAML 扫描器
│       ├── YamlTokenizer.cs         # YAML 分词器
│       ├── YamlAnchorManager.cs     # 锚点管理器
│       └── YamlTypeConverter.cs     # 类型转换器
│
├── Json/                            # JSON 序列化器
│   ├── JsonSerializer.cs            # JSON 序列化器主类
│   ├── JsonSerializerT.cs           # 泛型 JSON 序列化器
│   ├── JsonSerializerOptions.cs     # JSON 序列化选项
│   ├── JsonFormat.cs                # JSON 格式定义
│   ├── JsonWriter.cs                # JSON 写入器
│   ├── JsonReader.cs                # JSON 读取器
│   └── Internal/                    # 内部实现
│       ├── JsonTokenizer.cs         # JSON 分词器
│       ├── JsonEscaper.cs           # JSON 转义处理
│       ├── JsonNumberParser.cs      # 数字解析器
│       └── JsonStringBuilder.cs     # 字符串构建器
│__________________________________________________________
├── Modes/                           # 序列化模式实现
│   ├── ObjectMode/                  # Object 模式
│   │   ├── ObjectModeSerializer.cs  # Object 模式序列化器
│   │   ├── ObjectModeWriter.cs      # Object 模式写入器
│   │   └── ObjectModeReader.cs      # Object 模式读取器
│   │
│   ├── VersionTolerant/             # 版本容错模式
│   │   ├── VersionTolerantSerializer.cs     # 版本容错序列化器
│   │   ├── VersionTolerantWriter.cs         # 版本容错写入器
│   │   ├── VersionTolerantReader.cs         # 版本容错读取器
│   │   ├── FieldMatcher.cs                  # 字段匹配器
│   │   └── TypeCompatibilityChecker.cs      # 类型兼容检查器
│   │
│   ├── CircularReference/           # 循环引用模式
│   │   ├── CircularReferenceSerializer.cs   # 循环引用序列化器
│   │   ├── ReferenceTracker.cs              # 引用跟踪器
│   │   ├── ObjectIdGenerator.cs             # 对象 ID 生成器
│   │   └── ReferenceResolver.cs             # 引用解析器
│   │
│   ├── Polymorphic/                 # 多态模式
│   │   ├── PolymorphicSerializer.cs         # 多态序列化器
│   │   ├── UnionTypeRegistry.cs             # 联合类型注册表
│   │   ├── TypeDiscriminator.cs             # 类型鉴别器
│   │   └── DerivedTypeResolver.cs           # 派生类型解析器
│   │
│   └── Streaming/                   # 流式模式
│       ├── StreamingSerializer.cs           # 流式序列化器
│       ├── ChunkedWriter.cs                 # 分块写入器
│       ├── ChunkedReader.cs                 # 分块读取器
│       └── ProgressReporter.cs              # 进度报告器
│
└── Extensions/                      # 序列化器扩展
    ├── SerializerExtensions.cs      # 序列化器扩展方法
    ├── AsyncSerializerExtensions.cs # 异步扩展方法
    ├── StreamExtensions.cs          # 流扩展方法
    └── CloneExtensions.cs           # 克隆扩展方法
```

### 2.3 Formatters 格式化器

```
Runtime/Formatters/
├── Provider/                        # 格式化器提供者
│   ├── FormatterProvider.cs         # 格式化器提供者主类
│   ├── FormatterCache.cs            # 格式化器缓存
│   ├── FormatterFactory.cs          # 格式化器工厂
│   ├── FormatterRegistry.cs         # 格式化器注册表
│   └── ResolverChain.cs             # 解析器链
│
├── Primitives/                      # 基础类型格式化器
│   ├── Numeric/                     # 数值类型
│   │   ├── BooleanFormatter.cs      # bool 格式化器
│   │   ├── ByteFormatter.cs         # byte 格式化器
│   │   ├── SByteFormatter.cs        # sbyte 格式化器
│   │   ├── Int16Formatter.cs        # short 格式化器
│   │   ├── UInt16Formatter.cs       # ushort 格式化器
│   │   ├── Int32Formatter.cs        # int 格式化器
│   │   ├── UInt32Formatter.cs       # uint 格式化器
│   │   ├── Int64Formatter.cs        # long 格式化器
│   │   ├── UInt64Formatter.cs       # ulong 格式化器
│   │   ├── Int128Formatter.cs       # Int128 格式化器
│   │   ├── UInt128Formatter.cs      # UInt128 格式化器
│   │   ├── HalfFormatter.cs         # Half 格式化器
│   │   ├── SingleFormatter.cs       # float 格式化器
│   │   ├── DoubleFormatter.cs       # double 格式化器
│   │   ├── DecimalFormatter.cs      # decimal 格式化器
│   │   ├── CharFormatter.cs         # char 格式化器
│   │   └── IntPtrFormatter.cs       # IntPtr 格式化器
│   │
│   ├── String/                      # 字符串类型
│   │   ├── StringFormatter.cs       # string 格式化器
│   │   ├── Utf8StringFormatter.cs   # UTF-8 字符串格式化器
│   │   ├── Utf16StringFormatter.cs  # UTF-16 字符串格式化器
│   │   ├── InternStringFormatter.cs # 内化字符串格式化器
│   │   ├── CompressedStringFormatter.cs # 压缩字符串格式化器
│   │   └── StringBuilderFormatter.cs    # StringBuilder 格式化器
│   │
│   └── Special/                     # 特殊类型
│       ├── GuidFormatter.cs         # Guid 格式化器
│       ├── DateTimeFormatter.cs     # DateTime 格式化器
│       ├── DateTimeOffsetFormatter.cs   # DateTimeOffset 格式化器
│       ├── TimeSpanFormatter.cs     # TimeSpan 格式化器
│       ├── DateOnlyFormatter.cs     # DateOnly 格式化器
│       ├── TimeOnlyFormatter.cs     # TimeOnly 格式化器
│       ├── UriFormatter.cs          # Uri 格式化器
│       ├── VersionFormatter.cs      # Version 格式化器
│       ├── BigIntegerFormatter.cs   # BigInteger 格式化器
│       ├── ComplexFormatter.cs      # Complex 格式化器
│       └── TypeFormatter.cs         # Type 格式化器
│
├── Collections/                     # 集合类型格式化器
│   ├── Array/                       # 数组类型
│   │   ├── ArrayFormatter.cs        # 一维数组格式化器
│   │   ├── Array2DFormatter.cs      # 二维数组格式化器
│   │   ├── Array3DFormatter.cs      # 三维数组格式化器
│   │   ├── JaggedArrayFormatter.cs  # 交错数组格式化器
│   │   └── ArraySegmentFormatter.cs # ArraySegment 格式化器
│   │
│   ├── List/                        # 列表类型
│   │   ├── ListFormatter.cs         # List<T> 格式化器
│   │   ├── LinkedListFormatter.cs   # LinkedList<T> 格式化器
│   │   ├── ReadOnlyCollectionFormatter.cs   # ReadOnlyCollection 格式化器
│   │   ├── ObservableCollectionFormatter.cs # ObservableCollection 格式化器
│   │   └── ImmutableListFormatter.cs        # ImmutableList 格式化器
│   │
│   ├── Dictionary/                  # 字典类型
│   │   ├── DictionaryFormatter.cs           # Dictionary<K,V> 格式化器
│   │   ├── SortedDictionaryFormatter.cs     # SortedDictionary 格式化器
│   │   ├── ConcurrentDictionaryFormatter.cs # ConcurrentDictionary 格式化器
│   │   ├── ReadOnlyDictionaryFormatter.cs   # ReadOnlyDictionary 格式化器
│   │   ├── ImmutableDictionaryFormatter.cs  # ImmutableDictionary 格式化器
│   │   └── KeyValuePairFormatter.cs         # KeyValuePair 格式化器
│   │
│   ├── Set/                         # 集合类型
│   │   ├── HashSetFormatter.cs      # HashSet<T> 格式化器
│   │   ├── SortedSetFormatter.cs    # SortedSet<T> 格式化器
│   │   ├── ImmutableHashSetFormatter.cs     # ImmutableHashSet 格式化器
│   │   └── ImmutableSortedSetFormatter.cs   # ImmutableSortedSet 格式化器
│   │
│   ├── Queue/                       # 队列类型
│   │   ├── QueueFormatter.cs        # Queue<T> 格式化器
│   │   ├── ConcurrentQueueFormatter.cs      # ConcurrentQueue 格式化器
│   │   ├── PriorityQueueFormatter.cs        # PriorityQueue 格式化器
│   │   └── ImmutableQueueFormatter.cs       # ImmutableQueue 格式化器
│   │
│   ├── Stack/                       # 栈类型
│   │   ├── StackFormatter.cs        # Stack<T> 格式化器
│   │   ├── ConcurrentStackFormatter.cs      # ConcurrentStack 格式化器
│   │   └── ImmutableStackFormatter.cs       # ImmutableStack 格式化器
│   │
│   └── Span/                        # Span 类型
│       ├── SpanFormatter.cs         # Span<T> 格式化器
│       ├── ReadOnlySpanFormatter.cs # ReadOnlySpan<T> 格式化器
│       ├── MemoryFormatter.cs       # Memory<T> 格式化器
│       └── ReadOnlyMemoryFormatter.cs       # ReadOnlyMemory<T> 格式化器
│
├── Tuple/                           # 元组类型格式化器
│   ├── TupleFormatter.cs            # Tuple 格式化器
│   ├── ValueTupleFormatter.cs       # ValueTuple 格式化器
│   ├── Tuple2Formatter.cs           # Tuple<T1,T2> 格式化器
│   ├── Tuple3Formatter.cs           # Tuple<T1,T2,T3> 格式化器
│   ├── Tuple4Formatter.cs           # Tuple<T1,T2,T3,T4> 格式化器
│   ├── Tuple5Formatter.cs           # Tuple<T1,T2,T3,T4,T5> 格式化器
│   ├── Tuple6Formatter.cs           # Tuple<T1,T2,T3,T4,T5,T6> 格式化器
│   ├── Tuple7Formatter.cs           # Tuple<T1,T2,T3,T4,T5,T6,T7> 格式化器
│   └── Tuple8Formatter.cs           # Tuple<T1,...,TRest> 格式化器
│
├── Nullable/                        # 可空类型格式化器
│   ├── NullableFormatter.cs         # Nullable<T> 格式化器
│   └── NullableReferenceFormatter.cs # 可空引用类型格式化器
│
├── Enum/                            # 枚举类型格式化器
│   ├── EnumFormatter.cs             # 枚举格式化器
│   ├── EnumAsStringFormatter.cs     # 枚举字符串格式化器
│   ├── EnumAsIntFormatter.cs        # 枚举整数格式化器
│   └── FlagsEnumFormatter.cs        # 标志枚举格式化器
│
├── Object/                          # 对象类型格式化器
│   ├── ObjectFormatter.cs           # object 格式化器
│   ├── DynamicFormatter.cs          # dynamic 格式化器
│   ├── ExpandoObjectFormatter.cs    # ExpandoObject 格式化器
│   └── AnonymousTypeFormatter.cs    # 匿名类型格式化器
│
├── Interface/                       # 接口类型格式化器
│   ├── IEnumerableFormatter.cs      # IEnumerable 格式化器
│   ├── ICollectionFormatter.cs      # ICollection 格式化器
│   ├── IListFormatter.cs            # IList 格式化器
│   ├── IDictionaryFormatter.cs      # IDictionary 格式化器
│   ├── ISetFormatter.cs             # ISet 格式化器
│   └── IReadOnlyFormatter.cs        # IReadOnly* 格式化器
│
└── Compression/                     # 压缩格式化器
    ├── CompressedFormatter.cs       # 压缩格式化器基类
    ├── BrotliFormatter.cs           # Brotli 压缩格式化器
    ├── LZ4Formatter.cs              # LZ4 压缩格式化器
    ├── ZstdFormatter.cs             # Zstd 压缩格式化器
    ├── GzipFormatter.cs             # Gzip 压缩格式化器
    └── BitPackFormatter.cs          # 位打包格式化器
```



### 2.4 Buffers 缓冲区管理

```
Runtime/Buffers/
├── Writer/                          # 写入器
│   ├── SerializeWriter.cs           # 序列化写入器主类
│   ├── SerializeWriterExtensions.cs # 写入器扩展方法
│   ├── BufferWriter.cs              # 缓冲区写入器
│   ├── PooledBufferWriter.cs        # 池化缓冲区写入器
│   ├── ArrayBufferWriter.cs         # 数组缓冲区写入器
│   ├── StreamBufferWriter.cs        # 流缓冲区写入器
│   └── Internal/                    # 内部实现
│       ├── WriterState.cs           # 写入器状态
│       ├── WriteBuffer.cs           # 写入缓冲区
│       └── WritePosition.cs         # 写入位置
│
├── Reader/                          # 读取器
│   ├── SerializeReader.cs           # 序列化读取器主类
│   ├── SerializeReaderExtensions.cs # 读取器扩展方法
│   ├── BufferReader.cs              # 缓冲区读取器
│   ├── SequenceReader.cs            # 序列读取器
│   ├── SpanReader.cs                # Span 读取器
│   ├── StreamReader.cs              # 流读取器
│   └── Internal/                    # 内部实现
│       ├── ReaderState.cs           # 读取器状态
│       ├── ReadBuffer.cs            # 读取缓冲区
│       └── ReadPosition.cs          # 读取位置
│
├── Pool/                            # 缓冲区池
│   ├── BufferPool.cs                # 缓冲区池主类
│   ├── BufferPoolT.cs               # 泛型缓冲区池
│   ├── ArrayPoolWrapper.cs          # ArrayPool 包装器
│   ├── ThreadLocalPool.cs           # 线程本地池
│   ├── SharedPool.cs                # 共享池
│   ├── PooledArray.cs               # 池化数组
│   ├── PooledMemory.cs              # 池化内存
│   └── Internal/                    # 内部实现
│       ├── PoolBucket.cs            # 池桶
│       ├── PoolStatistics.cs        # 池统计
│       └── PoolConfiguration.cs     # 池配置
│
├── Segment/                         # 分段缓冲区
│   ├── BufferSegment.cs             # 缓冲区段
│   ├── SegmentPool.cs               # 段池
│   ├── SegmentChain.cs              # 段链
│   └── SegmentedBuffer.cs           # 分段缓冲区
│
├── Memory/                          # 内存管理
│   ├── MemoryManager.cs             # 内存管理器
│   ├── MemoryOwner.cs               # 内存所有者
│   ├── MemoryHandle.cs              # 内存句柄
│   ├── PinnedMemory.cs              # 固定内存
│   └── UnmanagedMemory.cs           # 非托管内存
│
└── Utilities/                       # 缓冲区工具
    ├── BufferHelper.cs              # 缓冲区辅助类
    ├── SpanHelper.cs                # Span 辅助类
    ├── MemoryHelper.cs              # Memory 辅助类
    └── SequenceHelper.cs            # Sequence 辅助类
```

### 2.5 Compression 压缩模块

```
Runtime/Compression/
├── Providers/                       # 压缩提供者
│   ├── CompressionProvider.cs       # 压缩提供者基类
│   ├── BrotliProvider.cs            # Brotli 压缩提供者
│   ├── LZ4Provider.cs               # LZ4 压缩提供者
│   ├── ZstdProvider.cs              # Zstd 压缩提供者
│   ├── GzipProvider.cs              # Gzip 压缩提供者
│   ├── DeflateProvider.cs           # Deflate 压缩提供者
│   └── NoCompressionProvider.cs     # 无压缩提供者
│
├── Streams/                         # 压缩流
│   ├── CompressionStream.cs         # 压缩流基类
│   ├── BrotliStream.cs              # Brotli 流
│   ├── LZ4Stream.cs                 # LZ4 流
│   ├── ZstdStream.cs                # Zstd 流
│   └── BufferedCompressionStream.cs # 缓冲压缩流
│
├── Algorithms/                      # 压缩算法
│   ├── LZ4/                         # LZ4 算法
│   │   ├── LZ4Encoder.cs            # LZ4 编码器
│   │   ├── LZ4Decoder.cs            # LZ4 解码器
│   │   ├── LZ4Block.cs              # LZ4 块
│   │   └── LZ4Frame.cs              # LZ4 帧
│   │
│   ├── Zstd/                        # Zstd 算法
│   │   ├── ZstdEncoder.cs           # Zstd 编码器
│   │   ├── ZstdDecoder.cs           # Zstd 解码器
│   │   └── ZstdDictionary.cs        # Zstd 字典
│   │
│   └── BitPack/                     # 位打包
│       ├── BitPackEncoder.cs        # 位打包编码器
│       ├── BitPackDecoder.cs        # 位打包解码器
│       └── BitWriter.cs             # 位写入器
│
└── Utilities/                       # 压缩工具
    ├── CompressionHelper.cs         # 压缩辅助类
    ├── CompressionDetector.cs       # 压缩检测器
    └── CompressionBenchmark.cs      # 压缩基准测试
```

### 2.6 Security 安全模块

```
Runtime/Security/
├── Encryption/                      # 加密
│   ├── EncryptionProvider.cs        # 加密提供者基类
│   ├── AesEncryptionProvider.cs     # AES 加密提供者
│   ├── XorEncryptionProvider.cs     # XOR 加密提供者
│   ├── NoEncryptionProvider.cs      # 无加密提供者
│   └── Internal/                    # 内部实现
│       ├── AesHelper.cs             # AES 辅助类
│       ├── KeyDerivation.cs         # 密钥派生
│       └── IVGenerator.cs           # IV 生成器
│
├── Checksum/                        # 校验
│   ├── ChecksumProvider.cs          # 校验提供者基类
│   ├── Crc32Provider.cs             # CRC32 校验提供者
│   ├── Crc64Provider.cs             # CRC64 校验提供者
│   ├── XXHashProvider.cs            # XXHash 校验提供者
│   ├── Sha256Provider.cs            # SHA256 校验提供者
│   ├── Md5Provider.cs               # MD5 校验提供者
│   └── NoChecksumProvider.cs        # 无校验提供者
│
├── Validation/                      # 数据验证
│   ├── DataValidator.cs             # 数据验证器
│   ├── SchemaValidator.cs           # 模式验证器
│   ├── TypeValidator.cs             # 类型验证器
│   ├── SizeValidator.cs             # 大小验证器
│   └── DepthValidator.cs            # 深度验证器
│
├── Obfuscation/                     # 混淆
│   ├── ObfuscationProvider.cs       # 混淆提供者
│   ├── ByteShuffler.cs              # 字节混洗器
│   └── HeaderObfuscator.cs          # 头部混淆器
│
└── Utilities/                       # 安全工具
    ├── SecureMemory.cs              # 安全内存
    ├── SecureRandom.cs              # 安全随机数
    └── SecureDispose.cs             # 安全释放
```

### 2.7 Unity 集成模块

```
Runtime/Unity/
├── Types/                           # Unity 类型格式化器
│   ├── Math/                        # 数学类型
│   │   ├── Vector2Formatter.cs      # Vector2 格式化器
│   │   ├── Vector3Formatter.cs      # Vector3 格式化器
│   │   ├── Vector4Formatter.cs      # Vector4 格式化器
│   │   ├── Vector2IntFormatter.cs   # Vector2Int 格式化器
│   │   ├── Vector3IntFormatter.cs   # Vector3Int 格式化器
│   │   ├── QuaternionFormatter.cs   # Quaternion 格式化器
│   │   ├── Matrix4x4Formatter.cs    # Matrix4x4 格式化器
│   │   ├── PlaneFormatter.cs        # Plane 格式化器
│   │   └── RayFormatter.cs          # Ray 格式化器
│   │
│   ├── Color/                       # 颜色类型
│   │   ├── ColorFormatter.cs        # Color 格式化器
│   │   ├── Color32Formatter.cs      # Color32 格式化器
│   │   └── GradientFormatter.cs     # Gradient 格式化器
│   │
│   ├── Geometry/                    # 几何类型
│   │   ├── RectFormatter.cs         # Rect 格式化器
│   │   ├── RectIntFormatter.cs      # RectInt 格式化器
│   │   ├── BoundsFormatter.cs       # Bounds 格式化器
│   │   ├── BoundsIntFormatter.cs    # BoundsInt 格式化器
│   │   └── RectOffsetFormatter.cs   # RectOffset 格式化器
│   │
│   ├── Animation/                   # 动画类型
│   │   ├── AnimationCurveFormatter.cs       # AnimationCurve 格式化器
│   │   ├── KeyframeFormatter.cs             # Keyframe 格式化器
│   │   └── WrapModeFormatter.cs             # WrapMode 格式化器
│   │
│   ├── Rendering/                   # 渲染类型
│   │   ├── LayerMaskFormatter.cs    # LayerMask 格式化器
│   │   ├── RenderTextureFormatter.cs # RenderTexture 格式化器
│   │   └── MaterialPropertyFormatter.cs # MaterialProperty 格式化器
│   │
│   └── Misc/                        # 其他类型
│       ├── Hash128Formatter.cs      # Hash128 格式化器
│       ├── RangeIntFormatter.cs     # RangeInt 格式化器
│       └── ResolutionFormatter.cs   # Resolution 格式化器
│
├── Assets/                          # 资产序列化
│   ├── AssetReferenceFormatter.cs   # 资产引用格式化器
│   ├── ScriptableObjectFormatter.cs # ScriptableObject 格式化器
│   ├── TextAssetFormatter.cs        # TextAsset 格式化器
│   └── AddressableFormatter.cs      # Addressable 格式化器
│
├── Scene/                           # 场景序列化
│   ├── GameObjectFormatter.cs       # GameObject 格式化器
│   ├── ComponentFormatter.cs        # Component 格式化器
│   ├── TransformFormatter.cs        # Transform 格式化器
│   └── SceneReferenceFormatter.cs   # 场景引用格式化器
│
├── Integration/                     # Unity 集成
│   ├── UnitySerializerBridge.cs     # Unity 序列化桥接
│   ├── UnityTypeResolver.cs         # Unity 类型解析器
│   ├── UnityFormatterProvider.cs    # Unity 格式化器提供者
│   └── UnityLifecycleHook.cs        # Unity 生命周期钩子
│
└── Utilities/                       # Unity 工具
    ├── UnitySerializeHelper.cs      # Unity 序列化辅助类
    ├── UnityBufferHelper.cs         # Unity 缓冲区辅助类
    └── UnityTypeHelper.cs           # Unity 类型辅助类
```



### 2.8 ECS 集成模块

```
Runtime/ECS/
├── Types/                           # ECS 类型格式化器
│   ├── Entity/                      # 实体类型
│   │   ├── EntityFormatter.cs       # Entity 格式化器
│   │   ├── EntityQueryFormatter.cs  # EntityQuery 格式化器
│   │   └── EntityArchetypeFormatter.cs # EntityArchetype 格式化器
│   │
│   ├── NativeContainers/            # Native 容器
│   │   ├── NativeArrayFormatter.cs          # NativeArray<T> 格式化器
│   │   ├── NativeListFormatter.cs           # NativeList<T> 格式化器
│   │   ├── NativeHashMapFormatter.cs        # NativeHashMap<K,V> 格式化器
│   │   ├── NativeHashSetFormatter.cs        # NativeHashSet<T> 格式化器
│   │   ├── NativeQueueFormatter.cs          # NativeQueue<T> 格式化器
│   │   ├── NativeStackFormatter.cs          # NativeStack<T> 格式化器
│   │   ├── NativeMultiHashMapFormatter.cs   # NativeMultiHashMap 格式化器
│   │   ├── NativeParallelHashMapFormatter.cs # NativeParallelHashMap 格式化器
│   │   └── NativeSliceFormatter.cs          # NativeSlice<T> 格式化器
│   │
│   ├── Blob/                        # Blob 资产
│   │   ├── BlobAssetReferenceFormatter.cs   # BlobAssetReference<T> 格式化器
│   │   ├── BlobArrayFormatter.cs            # BlobArray<T> 格式化器
│   │   ├── BlobStringFormatter.cs           # BlobString 格式化器
│   │   └── BlobPtrFormatter.cs              # BlobPtr<T> 格式化器
│   │
│   ├── Buffer/                      # 动态缓冲区
│   │   ├── DynamicBufferFormatter.cs        # DynamicBuffer<T> 格式化器
│   │   └── BufferElementFormatter.cs        # IBufferElementData 格式化器
│   │
│   └── FixedString/                 # 固定字符串
│       ├── FixedString32Formatter.cs        # FixedString32Bytes 格式化器
│       ├── FixedString64Formatter.cs        # FixedString64Bytes 格式化器
│       ├── FixedString128Formatter.cs       # FixedString128Bytes 格式化器
│       ├── FixedString512Formatter.cs       # FixedString512Bytes 格式化器
│       └── FixedString4096Formatter.cs      # FixedString4096Bytes 格式化器
│
├── World/                           # 世界序列化
│   ├── WorldSerializer.cs           # 世界序列化器
│   ├── EntitySerializer.cs          # 实体序列化器
│   ├── ComponentSerializer.cs       # 组件序列化器
│   ├── SystemSerializer.cs          # 系统序列化器
│   └── ArchetypeSerializer.cs       # 原型序列化器
│
├── Snapshot/                        # 快照系统
│   ├── WorldSnapshot.cs             # 世界快照
│   ├── EntitySnapshot.cs            # 实体快照
│   ├── ComponentSnapshot.cs         # 组件快照
│   ├── SnapshotWriter.cs            # 快照写入器
│   └── SnapshotReader.cs            # 快照读取器
│
├── Burst/                           # Burst 兼容
│   ├── BurstSerializeWriter.cs      # Burst 兼容写入器
│   ├── BurstSerializeReader.cs      # Burst 兼容读取器
│   ├── BurstFormatterT.cs           # Burst 兼容格式化器
│   └── BurstSerializeJob.cs         # Burst 序列化 Job
│
├── Integration/                     # ECS 集成
│   ├── EcsSerializerBridge.cs       # ECS 序列化桥接
│   ├── EcsTypeResolver.cs           # ECS 类型解析器
│   ├── EcsFormatterProvider.cs      # ECS 格式化器提供者
│   └── EntityRemapper.cs            # 实体重映射器
│
└── Utilities/                       # ECS 工具
    ├── EcsSerializeHelper.cs        # ECS 序列化辅助类
    ├── NativeBufferHelper.cs        # Native 缓冲区辅助类
    └── EntityHelper.cs              # 实体辅助类
```

### 2.9 Utilities 工具模块

```
Runtime/Utilities/
├── Memory/                          # 内存工具
│   ├── MemoryUtility.cs             # 内存工具类
│   ├── SpanUtility.cs               # Span 工具类
│   ├── UnsafeUtility.cs             # 非安全工具类
│   ├── MemoryCopy.cs                # 内存拷贝
│   ├── MemoryCompare.cs             # 内存比较
│   └── MemoryMarshal.cs             # 内存封送
│
├── Encoding/                        # 编码工具
│   ├── Utf8Utility.cs               # UTF-8 工具类
│   ├── Utf16Utility.cs              # UTF-16 工具类
│   ├── Base64Utility.cs             # Base64 工具类
│   ├── HexUtility.cs                # 十六进制工具类
│   └── VarIntUtility.cs             # 变长整数工具类
│
├── Reflection/                      # 反射工具
│   ├── TypeUtility.cs               # 类型工具类
│   ├── MemberUtility.cs             # 成员工具类
│   ├── AttributeUtility.cs          # 特性工具类
│   ├── ExpressionUtility.cs         # 表达式工具类
│   └── DynamicMethodUtility.cs      # 动态方法工具类
│
├── Hash/                            # 哈希工具
│   ├── HashUtility.cs               # 哈希工具类
│   ├── Crc32Utility.cs              # CRC32 工具类
│   ├── XXHashUtility.cs             # XXHash 工具类
│   └── FnvHashUtility.cs            # FNV 哈希工具类
│
├── Conversion/                      # 转换工具
│   ├── TypeConverter.cs             # 类型转换器
│   ├── NumericConverter.cs          # 数值转换器
│   ├── StringConverter.cs           # 字符串转换器
│   └── DateTimeConverter.cs         # 日期时间转换器
│
├── Pooling/                         # 对象池工具
│   ├── ObjectPool.cs                # 对象池
│   ├── ObjectPoolT.cs               # 泛型对象池
│   ├── PooledObject.cs              # 池化对象
│   └── PoolPolicy.cs                # 池策略
│
├── Threading/                       # 线程工具
│   ├── ThreadLocalCache.cs          # 线程本地缓存
│   ├── SpinLockSlim.cs              # 轻量自旋锁
│   └── AtomicCounter.cs             # 原子计数器
│
├── Diagnostics/                     # 诊断工具
│   ├── SerializeProfiler.cs         # 序列化分析器
│   ├── MemoryProfiler.cs            # 内存分析器
│   ├── SizeAnalyzer.cs              # 大小分析器
│   └── PerformanceCounter.cs        # 性能计数器
│
└── Extensions/                      # 扩展方法
    ├── ByteExtensions.cs            # 字节扩展
    ├── SpanExtensions.cs            # Span 扩展
    ├── MemoryExtensions.cs          # Memory 扩展
    ├── StreamExtensions.cs          # Stream 扩展
    └── TypeExtensions.cs            # Type 扩展
```

---

## 第三章：Editor 编辑器模块详细结构

### 3.1 编辑器模块总览

```
Editor/
├── Inspectors/                      # Inspector 面板
│   ├── SerializerInspector.cs       # 序列化器 Inspector
│   ├── FormatterInspector.cs        # 格式化器 Inspector
│   ├── SerializeDataInspector.cs    # 序列化数据 Inspector
│   └── ConfigurationInspector.cs    # 配置 Inspector
│
├── Windows/                         # 编辑器窗口
│   ├── SerializeDebugWindow.cs      # 序列化调试窗口
│   ├── DataViewerWindow.cs          # 数据查看器窗口
│   ├── SizeAnalyzerWindow.cs        # 大小分析器窗口
│   ├── PerformanceWindow.cs         # 性能分析窗口
│   ├── FormatterBrowserWindow.cs    # 格式化器浏览器窗口
│   └── ConfigurationWindow.cs       # 配置窗口
│
├── Tools/                           # 编辑器工具
│   ├── SerializeDataConverter.cs    # 数据格式转换工具
│   ├── SchemaGenerator.cs           # 模式生成器
│   ├── MigrationTool.cs             # 迁移工具
│   ├── ValidationTool.cs            # 验证工具
│   └── BenchmarkTool.cs             # 基准测试工具
│
├── Analyzers/                       # 代码分析器
│   ├── SerializableAnalyzer.cs      # 可序列化分析器
│   ├── FormatterAnalyzer.cs         # 格式化器分析器
│   ├── PerformanceAnalyzer.cs       # 性能分析器
│   └── CompatibilityAnalyzer.cs     # 兼容性分析器
│
├── Drawers/                         # 属性绘制器
│   ├── SerializeOptionsDrawer.cs    # 序列化选项绘制器
│   ├── FormatterDrawer.cs           # 格式化器绘制器
│   ├── CompressionDrawer.cs         # 压缩选项绘制器
│   └── VersionDrawer.cs             # 版本绘制器
│
├── Menus/                           # 菜单扩展
│   ├── SerializationMenu.cs         # 序列化菜单
│   ├── ToolsMenu.cs                 # 工具菜单
│   └── ContextMenu.cs               # 上下文菜单
│
├── Settings/                        # 设置
│   ├── SerializationSettings.cs     # 序列化设置
│   ├── SerializationSettingsProvider.cs # 设置提供者
│   └── ProjectSettings.cs           # 项目设置
│
├── Utilities/                       # 编辑器工具类
│   ├── EditorSerializeHelper.cs     # 编辑器序列化辅助类
│   ├── EditorGUIHelper.cs           # 编辑器 GUI 辅助类
│   ├── AssetDatabaseHelper.cs       # 资产数据库辅助类
│   └── EditorPrefsHelper.cs         # 编辑器偏好辅助类
│
├── Localization/                    # 本地化
│   ├── LocalizationManager.cs       # 本地化管理器
│   ├── Strings_en.cs                # 英文字符串
│   ├── Strings_zh.cs                # 中文字符串
│   └── Resources/                   # 本地化资源
│       ├── Localization_en.json     # 英文本地化
│       └── Localization_zh.json     # 中文本地化
│
└── AFramework.Serialization.Editor.asmdef  # 编辑器程序集定义
```



---

## 第四章：Generator 源代码生成器模块

### 4.1 生成器模块结构

```
Generator/
├── Analyzers/                       # 代码分析器
│   ├── SerializableAnalyzer.cs      # 可序列化类型分析器
│   ├── FormatterAnalyzer.cs         # 格式化器分析器
│   ├── AttributeAnalyzer.cs         # 特性分析器
│   ├── TypeAnalyzer.cs              # 类型分析器
│   ├── MemberAnalyzer.cs            # 成员分析器
│   ├── ConstructorAnalyzer.cs       # 构造函数分析器
│   └── DiagnosticAnalyzer.cs        # 诊断分析器
│
├── Generators/                      # 代码生成器
│   ├── FormatterGenerator.cs        # 格式化器生成器
│   ├── SerializerGenerator.cs       # 序列化器生成器
│   ├── UnionTypeGenerator.cs        # 联合类型生成器
│   ├── VersionMigratorGenerator.cs  # 版本迁移器生成器
│   ├── TypeInfoGenerator.cs         # 类型信息生成器
│   └── RegistrationGenerator.cs     # 注册代码生成器
│
├── Templates/                       # 代码模板
│   ├── FormatterTemplate.cs         # 格式化器模板
│   ├── SerializerTemplate.cs        # 序列化器模板
│   ├── UnionTypeTemplate.cs         # 联合类型模板
│   ├── VersionMigratorTemplate.cs   # 版本迁移器模板
│   └── RegistrationTemplate.cs      # 注册模板
│
├── Models/                          # 数据模型
│   ├── TypeModel.cs                 # 类型模型
│   ├── MemberModel.cs               # 成员模型
│   ├── FormatterModel.cs            # 格式化器模型
│   ├── AttributeModel.cs            # 特性模型
│   └── GenerationContext.cs         # 生成上下文
│
├── Emitters/                        # 代码发射器
│   ├── CodeEmitter.cs               # 代码发射器基类
│   ├── FormatterEmitter.cs          # 格式化器发射器
│   ├── SerializeEmitter.cs          # 序列化发射器
│   ├── DeserializeEmitter.cs        # 反序列化发射器
│   └── HelperEmitter.cs             # 辅助代码发射器
│
├── Utilities/                       # 工具类
│   ├── SymbolHelper.cs              # 符号辅助类
│   ├── SyntaxHelper.cs              # 语法辅助类
│   ├── TypeHelper.cs                # 类型辅助类
│   ├── NamingHelper.cs              # 命名辅助类
│   └── DiagnosticHelper.cs          # 诊断辅助类
│
├── Diagnostics/                     # 诊断定义
│   ├── DiagnosticDescriptors.cs     # 诊断描述符
│   ├── DiagnosticIds.cs             # 诊断 ID
│   └── DiagnosticMessages.cs        # 诊断消息
│
├── Extensions/                      # 扩展方法
│   ├── SymbolExtensions.cs          # 符号扩展
│   ├── SyntaxExtensions.cs          # 语法扩展
│   └── TypeSymbolExtensions.cs      # 类型符号扩展
│
└── AFramework.Serialization.Generator.csproj  # 生成器项目文件
```

### 4.2 生成器诊断 ID 规范

| ID 范围 | 类别 | 描述 |
|---------|------|------|
| AFS001-AFS099 | 错误 | 严重错误，阻止代码生成 |
| AFS100-AFS199 | 警告 | 潜在问题，建议修复 |
| AFS200-AFS299 | 信息 | 提示信息 |
| AFS300-AFS399 | 性能 | 性能相关建议 |

---

## 第五章：Tests 测试模块

### 5.1 测试模块结构

```
Tests/
├── Editor/                          # 编辑器测试
│   ├── Core/                        # 核心测试
│   │   ├── InterfaceTests.cs        # 接口测试
│   │   ├── AttributeTests.cs        # 特性测试
│   │   ├── ConfigurationTests.cs    # 配置测试
│   │   └── ExceptionTests.cs        # 异常测试
│   │
│   ├── Serializers/                 # 序列化器测试
│   │   ├── BinarySerializerTests.cs # 二进制序列化器测试
│   │   ├── YamlSerializerTests.cs   # YAML 序列化器测试
│   │   ├── JsonSerializerTests.cs   # JSON 序列化器测试
│   │   └── ModeTests.cs             # 模式测试
│   │
│   ├── Formatters/                  # 格式化器测试
│   │   ├── PrimitiveFormatterTests.cs       # 基础类型测试
│   │   ├── CollectionFormatterTests.cs      # 集合类型测试
│   │   ├── UnityFormatterTests.cs           # Unity 类型测试
│   │   ├── EcsFormatterTests.cs             # ECS 类型测试
│   │   └── CustomFormatterTests.cs          # 自定义格式化器测试
│   │
│   ├── Buffers/                     # 缓冲区测试
│   │   ├── WriterTests.cs           # 写入器测试
│   │   ├── ReaderTests.cs           # 读取器测试
│   │   ├── PoolTests.cs             # 池测试
│   │   └── MemoryTests.cs           # 内存测试
│   │
│   ├── Compression/                 # 压缩测试
│   │   ├── BrotliTests.cs           # Brotli 测试
│   │   ├── LZ4Tests.cs              # LZ4 测试
│   │   ├── ZstdTests.cs             # Zstd 测试
│   │   └── CompressionBenchmark.cs  # 压缩基准测试
│   │
│   ├── Security/                    # 安全测试
│   │   ├── EncryptionTests.cs       # 加密测试
│   │   ├── ChecksumTests.cs         # 校验测试
│   │   └── ValidationTests.cs       # 验证测试
│   │
│   ├── Integration/                 # 集成测试
│   │   ├── UnityIntegrationTests.cs # Unity 集成测试
│   │   ├── EcsIntegrationTests.cs   # ECS 集成测试
│   │   └── EndToEndTests.cs         # 端到端测试
│   │
│   ├── Utilities/                   # 测试工具
│   │   ├── TestDataGenerator.cs     # 测试数据生成器
│   │   ├── TestHelper.cs            # 测试辅助类
│   │   └── AssertExtensions.cs      # 断言扩展
│   │
│   └── AFramework.Serialization.Tests.Editor.asmdef
│
├── Runtime/                         # 运行时测试
│   ├── PlayModeTests.cs             # Play 模式测试
│   ├── RuntimeSerializeTests.cs     # 运行时序列化测试
│   ├── AsyncSerializeTests.cs       # 异步序列化测试
│   └── AFramework.Serialization.Tests.Runtime.asmdef
│
└── Performance/                     # 性能测试
    ├── Benchmarks/                  # 基准测试
    │   ├── SerializeBenchmark.cs    # 序列化基准测试
    │   ├── DeserializeBenchmark.cs  # 反序列化基准测试
    │   ├── MemoryBenchmark.cs       # 内存基准测试
    │   ├── CompressionBenchmark.cs  # 压缩基准测试
    │   └── ComparisonBenchmark.cs   # 对比基准测试
    │
    ├── Stress/                      # 压力测试
    │   ├── LargeDataTests.cs        # 大数据测试
    │   ├── ConcurrencyTests.cs      # 并发测试
    │   └── MemoryPressureTests.cs   # 内存压力测试
    │
    ├── Profiling/                   # 性能分析
    │   ├── ProfilerTests.cs         # 分析器测试
    │   └── AllocationTests.cs       # 分配测试
    │
    └── AFramework.Serialization.Tests.Performance.asmdef
```

---

## 第六章：Samples 示例模块

### 6.1 示例模块结构

```
Samples~/
├── BasicUsage/                      # 基础用法示例
│   ├── Scripts/                     # 脚本
│   │   ├── BasicSerializeExample.cs         # 基础序列化示例
│   │   ├── PrimitiveTypesExample.cs         # 基础类型示例
│   │   ├── CollectionTypesExample.cs        # 集合类型示例
│   │   ├── UnityTypesExample.cs             # Unity 类型示例
│   │   ├── CustomTypeExample.cs             # 自定义类型示例
│   │   └── AttributeUsageExample.cs         # 特性用法示例
│   │
│   ├── Scenes/                      # 场景
│   │   └── BasicUsageScene.unity    # 基础用法场景
│   │
│   └── README.md                    # 示例说明
│
├── AdvancedFeatures/                # 高级功能示例
│   ├── Scripts/                     # 脚本
│   │   ├── VersionTolerantExample.cs        # 版本容错示例
│   │   ├── PolymorphicExample.cs            # 多态序列化示例
│   │   ├── CircularReferenceExample.cs      # 循环引用示例
│   │   ├── CompressionExample.cs            # 压缩示例
│   │   ├── EncryptionExample.cs             # 加密示例
│   │   ├── CustomFormatterExample.cs        # 自定义格式化器示例
│   │   └── StreamingExample.cs              # 流式序列化示例
│   │
│   ├── Scenes/                      # 场景
│   │   └── AdvancedFeaturesScene.unity      # 高级功能场景
│   │
│   └── README.md                    # 示例说明
│
├── NetworkSync/                     # 网络同步示例
│   ├── Scripts/                     # 脚本
│   │   ├── NetworkSerializeExample.cs       # 网络序列化示例
│   │   ├── StateSnapshotExample.cs          # 状态快照示例
│   │   ├── DeltaCompressionExample.cs       # 增量压缩示例
│   │   ├── RpcSerializeExample.cs           # RPC 序列化示例
│   │   └── ReplicationExample.cs            # 复制示例
│   │
│   ├── Scenes/                      # 场景
│   │   └── NetworkSyncScene.unity   # 网络同步场景
│   │
│   └── README.md                    # 示例说明
│
├── SaveSystem/                      # 存档系统示例
│   ├── Scripts/                     # 脚本
│   │   ├── SaveSystemExample.cs             # 存档系统示例
│   │   ├── GameStateExample.cs              # 游戏状态示例
│   │   ├── PlayerDataExample.cs             # 玩家数据示例
│   │   ├── InventoryExample.cs              # 背包示例
│   │   ├── SettingsExample.cs               # 设置示例
│   │   └── MigrationExample.cs              # 迁移示例
│   │
│   ├── Scenes/                      # 场景
│   │   └── SaveSystemScene.unity    # 存档系统场景
│   │
│   └── README.md                    # 示例说明
│
├── EcsIntegration/                  # ECS 集成示例
│   ├── Scripts/                     # 脚本
│   │   ├── EcsSerializeExample.cs           # ECS 序列化示例
│   │   ├── WorldSnapshotExample.cs          # 世界快照示例
│   │   ├── EntitySerializeExample.cs        # 实体序列化示例
│   │   ├── ComponentSerializeExample.cs     # 组件序列化示例
│   │   └── BurstSerializeExample.cs         # Burst 序列化示例
│   │
│   ├── Scenes/                      # 场景
│   │   └── EcsIntegrationScene.unity        # ECS 集成场景
│   │
│   └── README.md                    # 示例说明
│
├── ConfigurationSystem/             # 配置系统示例
│   ├── Scripts/                     # 脚本
│   │   ├── YamlConfigExample.cs             # YAML 配置示例
│   │   ├── JsonConfigExample.cs             # JSON 配置示例
│   │   ├── LocalizationExample.cs           # 本地化示例
│   │   ├── GameBalanceExample.cs            # 游戏平衡示例
│   │   └── HotReloadExample.cs              # 热重载示例
│   │
│   ├── Configs/                     # 配置文件
│   │   ├── GameConfig.yaml          # 游戏配置
│   │   ├── ItemDatabase.yaml        # 物品数据库
│   │   └── Localization.yaml        # 本地化配置
│   │
│   ├── Scenes/                      # 场景
│   │   └── ConfigurationScene.unity # 配置系统场景
│   │
│   └── README.md                    # 示例说明
│
└── PerformanceDemo/                 # 性能演示示例
    ├── Scripts/                     # 脚本
    │   ├── PerformanceComparisonExample.cs  # 性能对比示例
    │   ├── MemoryOptimizationExample.cs     # 内存优化示例
    │   ├── BatchSerializeExample.cs         # 批量序列化示例
    │   └── ZeroAllocExample.cs              # 零分配示例
    │
    ├── Scenes/                      # 场景
    │   └── PerformanceDemoScene.unity       # 性能演示场景
    │
    └── README.md                    # 示例说明
```



---

## 第七章：Documentation 文档模块

### 7.1 文档模块结构

```
Documentation~/
├── API/                             # API 文档
│   ├── Core/                        # 核心 API
│   │   ├── ISerializer.md           # ISerializer 接口文档
│   │   ├── IFormatter.md            # IFormatter 接口文档
│   │   ├── SerializeWriter.md       # SerializeWriter 文档
│   │   ├── SerializeReader.md       # SerializeReader 文档
│   │   └── FormatterProvider.md     # FormatterProvider 文档
│   │
│   ├── Serializers/                 # 序列化器 API
│   │   ├── BinarySerializer.md      # BinarySerializer 文档
│   │   ├── YamlSerializer.md        # YamlSerializer 文档
│   │   └── JsonSerializer.md        # JsonSerializer 文档
│   │
│   ├── Formatters/                  # 格式化器 API
│   │   ├── PrimitiveFormatters.md   # 基础类型格式化器文档
│   │   ├── CollectionFormatters.md  # 集合格式化器文档
│   │   ├── UnityFormatters.md       # Unity 格式化器文档
│   │   └── EcsFormatters.md         # ECS 格式化器文档
│   │
│   ├── Attributes/                  # 特性 API
│   │   ├── TypeAttributes.md        # 类型特性文档
│   │   ├── MemberAttributes.md      # 成员特性文档
│   │   └── FormatterAttributes.md   # 格式化器特性文档
│   │
│   └── Utilities/                   # 工具 API
│       ├── BufferPool.md            # BufferPool 文档
│       ├── CompressionProvider.md   # CompressionProvider 文档
│       └── EncryptionProvider.md    # EncryptionProvider 文档
│
├── Guides/                          # 使用指南
│   ├── GettingStarted.md            # 快速入门
│   ├── BasicUsage.md                # 基础用法
│   ├── AdvancedUsage.md             # 高级用法
│   ├── CustomFormatter.md           # 自定义格式化器
│   ├── VersionMigration.md          # 版本迁移
│   ├── PerformanceOptimization.md   # 性能优化
│   ├── UnityIntegration.md          # Unity 集成
│   ├── EcsIntegration.md            # ECS 集成
│   ├── BestPractices.md             # 最佳实践
│   └── Troubleshooting.md           # 故障排除
│
├── Tutorials/                       # 教程
│   ├── Tutorial01_BasicSerialization.md     # 教程1：基础序列化
│   ├── Tutorial02_CustomTypes.md            # 教程2：自定义类型
│   ├── Tutorial03_Collections.md            # 教程3：集合序列化
│   ├── Tutorial04_Polymorphism.md           # 教程4：多态序列化
│   ├── Tutorial05_Versioning.md             # 教程5：版本控制
│   ├── Tutorial06_Compression.md            # 教程6：数据压缩
│   ├── Tutorial07_SaveSystem.md             # 教程7：存档系统
│   └── Tutorial08_NetworkSync.md            # 教程8：网络同步
│
├── Reference/                       # 参考文档
│   ├── BinaryFormat.md              # 二进制格式规范
│   ├── ErrorCodes.md                # 错误码参考
│   ├── Configuration.md             # 配置参考
│   ├── Diagnostics.md               # 诊断参考
│   └── Changelog.md                 # 更新日志
│
├── Images/                          # 文档图片
│   ├── Architecture/                # 架构图
│   │   ├── OverviewDiagram.png      # 总览图
│   │   ├── LayerDiagram.png         # 分层图
│   │   └── FlowDiagram.png          # 流程图
│   │
│   ├── Screenshots/                 # 截图
│   │   ├── EditorWindow.png         # 编辑器窗口
│   │   ├── Inspector.png            # Inspector 面板
│   │   └── Profiler.png             # 性能分析器
│   │
│   └── Diagrams/                    # 图表
│       ├── ClassDiagram.png         # 类图
│       ├── SequenceDiagram.png      # 序列图
│       └── StateDiagram.png         # 状态图
│
└── Localization/                    # 本地化文档
    ├── zh-CN/                       # 中文文档
    │   ├── GettingStarted.md        # 快速入门
    │   ├── BasicUsage.md            # 基础用法
    │   └── API/                     # API 文档
    │
    └── en-US/                       # 英文文档
        ├── GettingStarted.md        # Getting Started
        ├── BasicUsage.md            # Basic Usage
        └── API/                     # API Documentation
```

---

## 第八章：配置文件

### 8.1 程序集定义文件

#### 8.1.1 Runtime 程序集 (AFramework.Serialization.asmdef)

```json
{
    "name": "AFramework.Serialization",
    "rootNamespace": "AFramework.Serialization",
    "references": [
        "AFramework.CSharpExtension",
        "AFramework.BurstExtension",
        "Unity.Burst",
        "Unity.Collections",
        "Unity.Mathematics",
        "Unity.Entities"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": true,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [
        {
            "name": "com.unity.entities",
            "expression": "1.0.0",
            "define": "AFRAMEWORK_ECS_SUPPORT"
        },
        {
            "name": "com.unity.burst",
            "expression": "1.8.0",
            "define": "AFRAMEWORK_BURST_SUPPORT"
        }
    ],
    "noEngineReferences": false
}
```

#### 8.1.2 Editor 程序集 (AFramework.Serialization.Editor.asmdef)

```json
{
    "name": "AFramework.Serialization.Editor",
    "rootNamespace": "AFramework.Serialization.Editor",
    "references": [
        "AFramework.Serialization",
        "AFramework.CSharpExtension",
        "AFramework.BurstExtension"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": true,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

### 8.2 UPM 包配置 (package.json)

```json
{
    "name": "com.aframework.serialization",
    "version": "1.0.0",
    "displayName": "AFramework Serialization",
    "description": "高性能序列化框架，支持 Binary、YAML、JSON 多种格式，深度集成 Unity 和 ECS。",
    "unity": "2022.3",
    "unityRelease": "0f1",
    "documentationUrl": "https://docs.aframework.com/serialization",
    "changelogUrl": "https://docs.aframework.com/serialization/changelog",
    "licensesUrl": "https://docs.aframework.com/serialization/license",
    "dependencies": {
        "com.aframework.csharpextension": "1.0.0",
        "com.aframework.burstextension": "1.0.0"
    },
    "keywords": [
        "serialization",
        "binary",
        "yaml",
        "json",
        "high-performance",
        "zero-allocation",
        "ecs",
        "burst"
    ],
    "author": {
        "name": "AFramework Team",
        "email": "support@aframework.com",
        "url": "https://aframework.com"
    },
    "repository": {
        "type": "git",
        "url": "https://github.com/aframework/serialization.git"
    },
    "samples": [
        {
            "displayName": "Basic Usage",
            "description": "基础用法示例",
            "path": "Samples~/BasicUsage"
        },
        {
            "displayName": "Advanced Features",
            "description": "高级功能示例",
            "path": "Samples~/AdvancedFeatures"
        },
        {
            "displayName": "Network Sync",
            "description": "网络同步示例",
            "path": "Samples~/NetworkSync"
        },
        {
            "displayName": "Save System",
            "description": "存档系统示例",
            "path": "Samples~/SaveSystem"
        },
        {
            "displayName": "ECS Integration",
            "description": "ECS 集成示例",
            "path": "Samples~/EcsIntegration"
        },
        {
            "displayName": "Configuration System",
            "description": "配置系统示例",
            "path": "Samples~/ConfigurationSystem"
        },
        {
            "displayName": "Performance Demo",
            "description": "性能演示示例",
            "path": "Samples~/PerformanceDemo"
        }
    ]
}
```

---

## 第九章：附录

### 9.1 文件命名规范

| 类别 | 命名规则 | 示例 |
|------|----------|------|
| 接口 | I + PascalCase | ISerializer.cs, IFormatter.cs |
| 抽象类 | PascalCase + Base | SerializerBase.cs, FormatterBase.cs |
| 泛型类 | PascalCase + T | SerializerBaseT.cs, FormatterT.cs |
| 特性 | PascalCase + Attribute | SerializableAttribute.cs |
| 枚举 | PascalCase | SerializeMode.cs, CompressionLevel.cs |
| 异常 | PascalCase + Exception | SerializationException.cs |
| 扩展方法 | PascalCase + Extensions | SerializerExtensions.cs |
| 工具类 | PascalCase + Utility/Helper | MemoryUtility.cs, TypeHelper.cs |
| 测试类 | PascalCase + Tests | BinarySerializerTests.cs |

### 9.2 命名空间规范

| 层级 | 命名空间 | 描述 |
|------|----------|------|
| 根命名空间 | AFramework.Serialization | 序列化模块根命名空间 |
| 核心层 | AFramework.Serialization | 核心接口和基类 |
| 序列化器 | AFramework.Serialization | 序列化器实现 |
| 格式化器 | AFramework.Serialization.Formatters | 格式化器实现 |
| 缓冲区 | AFramework.Serialization.Buffers | 缓冲区管理 |
| 压缩 | AFramework.Serialization.Compression | 压缩模块 |
| 安全 | AFramework.Serialization.Security | 安全模块 |
| Unity | AFramework.Serialization.Unity | Unity 集成 |
| ECS | AFramework.Serialization.ECS | ECS 集成 |
| 工具 | AFramework.Serialization.Utilities | 工具类 |
| 编辑器 | AFramework.Serialization.Editor | 编辑器扩展 |

### 9.3 预处理指令规范

| 指令 | 描述 | 使用场景 |
|------|------|----------|
| AFRAMEWORK_SERIALIZATION | 序列化模块启用 | 模块级别开关 |
| AFRAMEWORK_ECS_SUPPORT | ECS 支持启用 | ECS 相关代码 |
| AFRAMEWORK_BURST_SUPPORT | Burst 支持启用 | Burst 相关代码 |
| AFRAMEWORK_COMPRESSION_LZ4 | LZ4 压缩启用 | LZ4 压缩代码 |
| AFRAMEWORK_COMPRESSION_ZSTD | Zstd 压缩启用 | Zstd 压缩代码 |
| AFRAMEWORK_ENCRYPTION_AES | AES 加密启用 | AES 加密代码 |
| UNITY_2022_3_OR_NEWER | Unity 2022.3+ | 版本特定代码 |
| UNITY_2023_1_OR_NEWER | Unity 2023.1+ | 版本特定代码 |
| UNITY_6000_0_OR_NEWER | Unity 6.x | 版本特定代码 |

### 9.4 文件数量统计

| 模块 | 文件数量（预估） | 代码行数（预估） |
|------|------------------|------------------|
| Core | ~50 | ~5,000 |
| Serializers | ~40 | ~8,000 |
| Formatters | ~100 | ~15,000 |
| Buffers | ~30 | ~4,000 |
| Compression | ~25 | ~3,000 |
| Security | ~20 | ~2,500 |
| Unity | ~40 | ~5,000 |
| ECS | ~35 | ~4,500 |
| Utilities | ~30 | ~3,500 |
| Editor | ~40 | ~6,000 |
| Generator | ~30 | ~5,000 |
| Tests | ~50 | ~8,000 |
| **总计** | **~490** | **~69,500** |

### 9.5 开发优先级

| 优先级 | 模块 | 描述 |
|--------|------|------|
| P0 (核心) | Core, Serializers/Binary, Formatters/Primitives, Buffers | 核心功能，必须首先完成 |
| P0 (核心) | Formatters/Collections, Unity/Types | 基础类型支持 |
| P1 (重要) | Serializers/Yaml, Serializers/Json | 多格式支持 |
| P1 (重要) | Compression, Security | 压缩和安全功能 |
| P1 (重要) | ECS, Unity/Integration | Unity 和 ECS 深度集成 |
| P2 (增强) | Editor, Generator | 编辑器工具和代码生成 |
| P2 (增强) | Tests, Samples | 测试和示例 |
| P3 (文档) | Documentation | 完整文档 |

---

## 文档版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-04 | 初始版本，完整目录结构设计 |

---

*本文档由 AFramework 团队编写，版权所有。*
