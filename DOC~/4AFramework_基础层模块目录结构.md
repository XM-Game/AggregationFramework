# AFramework 基础层模块目录结构设计

> 版本：1.0.0  
> 适用 Unity 版本：2022.3 LTS ~ Unity 6.x  
> 最后更新：2025年12月31日

---

## 目录

1. [目录设计原则](#1-目录设计原则)
2. [CSharpExtension 模块](#2-csharpextension-模块)
3. [BurstExtension 模块](#3-burstextension-模块)
4. [UnityExtension 模块](#4-unityextension-模块)
5. [模块间依赖关系](#5-模块间依赖关系)
6. [文件命名规范](#6-文件命名规范)

---

## 1. 目录设计原则

### 1.1 核心原则

| 原则 | 描述 |
|------|------|
| 单一职责 | 每个文件只包含一个主要类型或紧密相关的类型集合 |
| 层次清晰 | 目录层级不超过3层，避免过深嵌套 |
| 功能分组 | 按功能领域组织文件，便于查找和维护 |
| 编辑器分离 | Editor代码与Runtime代码严格分离 |
| 测试独立 | 测试代码放置在独立的Tests目录 |

### 1.2 目录结构模板

```
ModuleName/
├── Runtime/                    # 运行时代码
│   ├── Core/                   # 核心实现
│   ├── Extensions/             # 扩展方法
│   ├── Utilities/              # 工具类
│   ├── Interfaces/             # 接口定义
│   ├── Attributes/             # 特性定义
│   └── ModuleName.asmdef       # 程序集定义
├── Editor/                     # 编辑器代码
│   └── ModuleName.Editor.asmdef
├── Tests/                      # 测试代码
│   ├── Runtime/                # 运行时测试
│   └── Editor/                 # 编辑器测试
├── Documentation~/             # 模块文档
├── Samples~/                   # 示例代码
├── package.json                # 包配置
├── README.md                   # 模块说明
├── CHANGELOG.md                # 变更日志
└── LICENSE.md                  # 许可证
```

---

## 2. CSharpExtension 模块

### 2.1 模块概述

| 属性 | 值 |
|------|-----|
| 命名空间 | AFramework.CSharpExtension |
| 程序集 | AFramework.CSharpExtension |
| 依赖 | 无 |
| 定位 | C#基础类型扩展库，框架基石 |

### 2.2 完整目录结构

```
Plugins/AFramework/CSharpExtension/
│
├── Runtime/
│   │
│   ├── Core/
│   │   ├── Constants/
│   │   │   ├── MathConstants.cs              # 数学常量定义
│   │   │   ├── StringConstants.cs            # 字符串常量定义
│   │   │   └── TimeConstants.cs              # 时间常量定义
│   │   │
│   │   ├── Enums/
│   │   │   ├── ComparisonType.cs             # 比较类型枚举
│   │   │   ├── SortOrder.cs                  # 排序顺序枚举
│   │   │   ├── StringMatchMode.cs            # 字符串匹配模式
│   │   │   └── NumberFormat.cs               # 数字格式枚举
│   │   │
│   │   └── Structs/
│   │       ├── Range.cs                      # 范围结构体
│   │       ├── Optional.cs                   # 可选值结构体 (类似Nullable增强)
│   │       ├── Result.cs                     # 结果结构体 (成功/失败)
│   │       ├── Either.cs                     # 二选一结构体
│   │       └── Unit.cs                       # 空值单元结构体
│   │
│   ├── Extensions/
│   │   │
│   │   ├── Collections/
│   │   │   ├── ArrayExtensions.cs            # 数组扩展方法
│   │   │   ├── ListExtensions.cs             # List<T>扩展方法
│   │   │   ├── DictionaryExtensions.cs       # Dictionary扩展方法
│   │   │   ├── HashSetExtensions.cs          # HashSet扩展方法
│   │   │   ├── QueueExtensions.cs            # Queue扩展方法
│   │   │   ├── StackExtensions.cs            # Stack扩展方法
│   │   │   ├── EnumerableExtensions.cs       # IEnumerable扩展方法
│   │   │   ├── CollectionExtensions.cs       # ICollection扩展方法
│   │   │   ├── ReadOnlyExtensions.cs         # 只读集合扩展方法
│   │   │   └── SpanExtensions.cs             # Span<T>/Memory<T>扩展方法
│   │   │
│   │   ├── Primitives/
│   │   │   ├── BoolExtensions.cs             # bool扩展方法
│   │   │   ├── ByteExtensions.cs             # byte/sbyte扩展方法
│   │   │   ├── IntExtensions.cs              # int/uint扩展方法
│   │   │   ├── LongExtensions.cs             # long/ulong扩展方法
│   │   │   ├── FloatExtensions.cs            # float扩展方法
│   │   │   ├── DoubleExtensions.cs           # double扩展方法
│   │   │   ├── DecimalExtensions.cs          # decimal扩展方法
│   │   │   └── CharExtensions.cs             # char扩展方法
│   │   │
│   │   ├── Strings/
│   │   │   ├── StringExtensions.cs           # 字符串基础扩展
│   │   │   ├── StringBuilderExtensions.cs    # StringBuilder扩展
│   │   │   ├── StringValidationExtensions.cs # 字符串验证扩展
│   │   │   ├── StringConversionExtensions.cs # 字符串转换扩展
│   │   │   ├── StringFormatExtensions.cs     # 字符串格式化扩展
│   │   │   └── StringSearchExtensions.cs     # 字符串搜索扩展
│   │   │
│   │   ├── DateTime/
│   │   │   ├── DateTimeExtensions.cs         # DateTime扩展方法
│   │   │   ├── TimeSpanExtensions.cs         # TimeSpan扩展方法
│   │   │   ├── DateOnlyExtensions.cs         # DateOnly扩展方法
│   │   │   └── TimeOnlyExtensions.cs         # TimeOnly扩展方法
│   │   │
│   │   ├── Types/
│   │   │   ├── TypeExtensions.cs             # Type扩展方法
│   │   │   ├── ObjectExtensions.cs           # object扩展方法
│   │   │   ├── EnumExtensions.cs             # Enum扩展方法
│   │   │   ├── DelegateExtensions.cs         # Delegate扩展方法
│   │   │   ├── ExceptionExtensions.cs        # Exception扩展方法
│   │   │   └── AttributeExtensions.cs        # Attribute扩展方法
│   │   │
│   │   ├── Async/————————————————————————————————————————————————————————————————————————————————————————
│   │   │   ├── TaskExtensions.cs             # Task扩展方法
│   │   │   ├── ValueTaskExtensions.cs        # ValueTask扩展方法
│   │   │   ├── CancellationTokenExtensions.cs# CancellationToken扩展
│   │   │   └── AsyncEnumerableExtensions.cs  # IAsyncEnumerable扩展
│   │   │
│   │   ├── IO/
│   │   │   ├── StreamExtensions.cs           # Stream扩展方法
│   │   │   ├── BinaryReaderExtensions.cs     # BinaryReader扩展
│   │   │   ├── BinaryWriterExtensions.cs     # BinaryWriter扩展
│   │   │   ├── PathExtensions.cs             # 路径字符串扩展
│   │   │   └── FileInfoExtensions.cs         # FileInfo扩展方法
│   │   │
│   │   └── Reflection/
│   │       ├── MemberInfoExtensions.cs       # MemberInfo扩展方法
│   │       ├── MethodInfoExtensions.cs       # MethodInfo扩展方法
│   │       ├── PropertyInfoExtensions.cs     # PropertyInfo扩展方法
│   │       ├── FieldInfoExtensions.cs        # FieldInfo扩展方法
│   │       └── AssemblyExtensions.cs         # Assembly扩展方法
│   │
│   ├── Utilities/——————————————————————————————————————————————————————
│   │   │
│   │   ├── Math/
│   │   │   ├── MathUtility.cs                # 数学工具类
│   │   │   ├── MathUtility.Interpolation.cs  # 插值计算 (partial)
│   │   │   ├── MathUtility.Clamping.cs       # 范围限制 (partial)
│   │   │   ├── MathUtility.Comparison.cs     # 数值比较 (partial)
│   │   │   ├── MathUtility.Rounding.cs       # 舍入计算 (partial)
│   │   │   └── BitUtility.cs                 # 位运算工具类
│   │   │
│   │   ├── String/————————————————————————————————————————————————————————————————————>
│   │   │   ├── StringUtility.cs              # 字符串工具类
│   │   │   ├── StringBuilderPool.cs          # StringBuilder对象池
│   │   │   ├── StringHashUtility.cs          # 字符串哈希工具
│   │   │   └── EncodingUtility.cs            # 编码工具类
│   │   │
│   │   ├── Collection/
│   │   │   ├── ArrayUtility.cs               # 数组工具类
│   │   │   ├── ListPool.cs                   # List<T>对象池
│   │   │   ├── DictionaryPool.cs             # Dictionary对象池
│   │   │   ├── HashSetPool.cs                # HashSet对象池
│   │   │   └── CollectionUtility.cs          # 集合工具类
│   │   │
│   │   ├── Reflection/
│   │   │   ├── ReflectionUtility.cs          # 反射工具类
│   │   │   ├── TypeCache.cs                  # 类型缓存
│   │   │   ├── ExpressionUtility.cs          # 表达式树工具
│   │   │   └── DelegateFactory.cs            # 委托工厂
│   │   │
│   │   ├── Threading/
│   │   │   ├── ThreadUtility.cs              # 线程工具类
│   │   │   ├── SpinLockSlim.cs               # 轻量自旋锁
│   │   │   ├── AtomicOperations.cs           # 原子操作工具
│   │   │   └── ThreadSafeCounter.cs          # 线程安全计数器
│   │   │
│   │   ├── Random/
│   │   │   ├── RandomUtility.cs              # 随机数工具类
│   │   │   ├── PcgRandom.cs                  # PCG随机数生成器
│   │   │   ├── XorShiftRandom.cs             # XorShift随机数生成器
│   │   │   └── WeightedRandom.cs             # 权重随机选择器
│   │   │
│   │   ├── Hash/
│   │   │   ├── HashUtility.cs                # 哈希工具类
│   │   │   ├── XxHash.cs                     # xxHash实现
│   │   │   ├── Fnv1aHash.cs                  # FNV-1a哈希实现
│   │   │   └── CombineHash.cs                # 哈希组合工具__________________________________________^
│   │   │
│   │   ├── Validation/
│   │   │   ├── Guard.cs                      # 参数守卫类
│   │   │   ├── Ensure.cs                     # 确保条件类
│   │   │   ├── Validator.cs                  # 验证器基类
│   │   │   └── ValidationResult.cs           # 验证结果
│   │   │
│   │   └── Functional/
│   │       ├── FunctionalUtility.cs          # 函数式工具
│   │       ├── Memoization.cs                # 记忆化工具
│   │       ├── Pipe.cs                       # 管道操作
│   │       └── Curry.cs                      # 柯里化工具
│   │
│   ├── Interfaces/_______________________________________________________________>
│   │   ├── IPoolable.cs                      # 可池化接口
│   │   ├── IResettable.cs                    # 可重置接口
│   │   ├── IDeepCloneable.cs                 # 深拷贝接口
│   │   ├── IValidator.cs                     # 验证器接口
│   │   └── IDisposableEx.cs                  # 增强Disposable接口
│   │
│   ├── Attributes/
│   │   ├── NotNullAttribute.cs               # 非空标记特性
│   │   ├── RangeAttribute.cs                 # 范围限制特性
│   │   ├── RegexAttribute.cs                 # 正则验证特性
│   │   ├── PureAttribute.cs                  # 纯函数标记特性
│   │   └── ThreadSafeAttribute.cs            # 线程安全标记特性
│   │
│   ├── Pooling/
│   │   ├── ObjectPool.cs                     # 泛型对象池
│   │   ├── ArrayPool.cs                      # 数组池封装
│   │   ├── PooledObject.cs                   # 池化对象包装器
│   │   └── PoolPolicy.cs                     # 池策略配置
│   │
│   ├── Disposables/
│   │   ├── DisposableBase.cs                 # Disposable基类
│   │   ├── CompositeDisposable.cs            # 组合Disposable
│   │   ├── SingleAssignmentDisposable.cs     # 单次赋值Disposable
│   │   └── RefCountDisposable.cs             # 引用计数Disposable
│   │
│   ├── Internal/
│   │   ├── ThrowHelper.cs                    # 异常抛出辅助类
│   │   ├── HashHelpers.cs                    # 哈希辅助类
│   │   ├── CollectionHelpers.cs              # 集合辅助类
│   │   └── UnsafeUtility.cs                  # 不安全代码辅助类
│   │
│   ├── AssemblyInfo.cs                       # 程序集信息
│   └── AFramework.CSharpExtension.asmdef     # 程序集定义文件
│
├── Editor/
│   ├── Inspectors/
│   │   └── OptionalDrawer.cs                 # Optional类型绘制器
│   │
│   ├── Utilities/
│   │   └── EditorReflectionUtility.cs        # 编辑器反射工具
│   │
│   ├── AssemblyInfo.cs
│   └── AFramework.CSharpExtension.Editor.asmdef
│
├── Tests/
│   ├── Runtime/
│   │   ├── Extensions/
│   │   │   ├── ArrayExtensionsTests.cs
│   │   │   ├── ListExtensionsTests.cs
│   │   │   ├── DictionaryExtensionsTests.cs
│   │   │   ├── StringExtensionsTests.cs
│   │   │   ├── IntExtensionsTests.cs
│   │   │   ├── FloatExtensionsTests.cs
│   │   │   └── ...
│   │   │
│   │   ├── Utilities/
│   │   │   ├── MathUtilityTests.cs
│   │   │   ├── StringUtilityTests.cs
│   │   │   ├── RandomUtilityTests.cs
│   │   │   └── ...
│   │   │
│   │   ├── Pooling/
│   │   │   ├── ObjectPoolTests.cs
│   │   │   └── ArrayPoolTests.cs
│   │   │
│   │   └── AFramework.CSharpExtension.Tests.asmdef
│   │
│   └── Editor/
│       └── AFramework.CSharpExtension.Editor.Tests.asmdef
│
├── Documentation~/
│   ├── index.md                              # 文档首页
│   ├── getting-started.md                    # 快速入门
│   ├── api/                                  # API文档
│   │   ├── extensions.md
│   │   ├── utilities.md
│   │   └── pooling.md
│   └── images/                               # 文档图片
│
├── Samples~/
│   ├── BasicUsage/
│   │   ├── CollectionExtensionsSample.cs
│   │   ├── StringExtensionsSample.cs
│   │   └── MathUtilitySample.cs
│   │
│   └── AdvancedUsage/
│       ├── ObjectPoolSample.cs
│       ├── FunctionalSample.cs
│       └── ReflectionSample.cs
│
├── package.json
├── README.md
├── CHANGELOG.md
└── LICENSE.md
```

### 2.3 核心文件说明

#### 2.3.1 扩展方法文件内容规范

| 文件 | 主要方法 | 设计要点 |
|------|----------|----------|
| ArrayExtensions.cs | Fill, Shuffle, RandomElement, IndexOf, Contains, Resize | 零分配、边界检查 |
| ListExtensions.cs | AddRange, RemoveAll, Shuffle, BinarySearch, Sort | 批量操作优化 |
| DictionaryExtensions.cs | GetOrAdd, GetOrDefault, TryAdd, AddOrUpdate | 线程安全考虑 |
| StringExtensions.cs | IsNullOrEmpty, Truncate, Repeat, Reverse, ToTitleCase | 空安全处理 |
| IntExtensions.cs | Clamp, InRange, Abs, Sign, IsBetween | 内联优化 |
| FloatExtensions.cs | Approximately, Lerp, InverseLerp, Remap, Clamp01 | 精度处理 |

#### 2.3.2 工具类文件内容规范

| 文件 | 主要功能 | 设计要点 |
|------|----------|----------|
| MathUtility.cs | 数学计算、插值、范围限制 | SIMD优化、内联 |
| StringUtility.cs | 字符串处理、格式化 | StringBuilder池化 |
| RandomUtility.cs | 随机数生成、权重选择 | 确定性随机支持 |
| Guard.cs | 参数验证、前置条件检查 | 清晰异常信息 |
| ReflectionUtility.cs | 反射操作简化 | 缓存优化 |



---

## 3. BurstExtension 模块

### 3.1 模块概述

| 属性 | 值 |
|------|-----|
| 命名空间 | AFramework.Burst |
| 程序集 | AFramework.Burst |
| 依赖 | com.unity.burst 1.8.x, com.unity.collections |
| 定位 | Burst编译器集成支持，高性能计算基础 |

### 3.2 完整目录结构

```
Plugins/AFramework/BurstExtension/
│
├── Runtime/
│   │
│   ├── Core/
│   │   ├── BurstRuntime.cs                   # Burst运行时检测与配置
│   │   ├── BurstCompileOptions.cs            # 编译选项配置
│   │   └── BurstFeatureDetection.cs          # CPU特性检测 (SIMD支持)
│   │
│   ├── Jobs/
│   │   │
│   │   ├── Base/
│   │   │   ├── JobBase.cs                    # Job基类封装
│   │   │   ├── ParallelJobBase.cs            # 并行Job基类
│   │   │   ├── JobForBase.cs                 # IJobFor基类封装
│   │   │   └── JobParallelForBase.cs         # IJobParallelFor基类封装
│   │   │
│   │   ├── Scheduling/
│   │   │   ├── JobScheduler.cs               # Job调度器
│   │   │   ├── JobHandle.Extensions.cs       # JobHandle扩展方法
│   │   │   ├── JobBatcher.cs                 # Job批处理器
│   │   │   └── DependencyGraph.cs            # 依赖图管理
│   │   │
│   │   ├── Common/
│   │   │   ├── CopyJob.cs                    # 数据复制Job
│   │   │   ├── ClearJob.cs                   # 数据清除Job
│   │   │   ├── FillJob.cs                    # 数据填充Job
│   │   │   ├── TransformJob.cs               # 数据转换Job
│   │   │   ├── FilterJob.cs                  # 数据过滤Job
│   │   │   ├── SortJob.cs                    # 排序Job
│   │   │   ├── SearchJob.cs                  # 搜索Job
│   │   │   └── AggregateJob.cs               # 聚合计算Job
│   │   │
│   │   └── Parallel/
│   │       ├── ParallelSort.cs               # 并行排序
│   │       ├── ParallelReduce.cs             # 并行归约
│   │       ├── ParallelScan.cs               # 并行扫描 (前缀和)
│   │       └── ParallelFilter.cs             # 并行过滤
│   │
│   ├── FunctionPointers/
│   │   ├── FunctionPointerUtility.cs         # 函数指针工具类
│   │   ├── BurstFunctionPointer.cs           # Burst函数指针封装
│   │   ├── FunctionPointerCache.cs           # 函数指针缓存
│   │   └── DelegateCompiler.cs               # 委托编译为函数指针
│   │
│   ├── SharedStatic/
│   │   ├── SharedStaticUtility.cs            # SharedStatic工具类
│   │   ├── SharedStaticWrapper.cs            # SharedStatic封装器
│   │   ├── SharedStaticArray.cs              # 共享静态数组
│   │   └── SharedStaticDictionary.cs         # 共享静态字典
│   │
│   ├── NativeContainers/
│   │   │
│   │   ├── Extensions/
│   │   │   ├── NativeArrayExtensions.cs      # NativeArray扩展
│   │   │   ├── NativeListExtensions.cs       # NativeList扩展
│   │   │   ├── NativeHashMapExtensions.cs    # NativeHashMap扩展
│   │   │   ├── NativeQueueExtensions.cs      # NativeQueue扩展
│   │   │   └── NativeSliceExtensions.cs      # NativeSlice扩展
│   │   │
│   │   ├── Custom/
│   │   │   ├── NativeRingBuffer.cs           # 原生环形缓冲区
│   │   │   ├── NativeStack.cs                # 原生栈
│   │   │   ├── NativePriorityQueue.cs        # 原生优先队列
│   │   │   ├── NativePool.cs                 # 原生对象池
│   │   │   └── NativeBitArray.cs             # 原生位数组
│   │   │
│   │   └── Unsafe/
│   │       ├── UnsafeUtilityEx.cs            # 不安全操作扩展
│   │       ├── MemoryUtility.cs              # 内存操作工具
│   │       └── AllocatorUtility.cs           # 分配器工具
│   │
│   ├── SIMD/
│   │   ├── SimdUtility.cs                    # SIMD工具类
│   │   ├── SimdMath.cs                       # SIMD数学运算
│   │   ├── SimdVector.cs                     # SIMD向量操作
│   │   └── IntrinsicsWrapper.cs              # Burst Intrinsics封装
│   │
│   ├── Mathematics/
│   │   ├── MathematicsExtensions.cs          # Unity.Mathematics扩展
│   │   ├── Float4Extensions.cs               # float4扩展方法
│   │   ├── Int4Extensions.cs                 # int4扩展方法
│   │   ├── QuaternionExtensions.cs           # quaternion扩展方法
│   │   └── MatrixExtensions.cs               # float4x4扩展方法
│   │
│   ├── Debugging/
│   │   ├── BurstDebug.cs                     # Burst调试工具
│   │   ├── BurstAssert.cs                    # Burst兼容断言
│   │   ├── BurstProfiler.cs                  # Burst性能分析
│   │   └── JobDebugger.cs                    # Job调试器
│   │
│   ├── Attributes/
│   │   ├── BurstCompileExAttribute.cs        # 增强的BurstCompile特性
│   │   ├── NoAliasAttribute.cs               # 无别名标记
│   │   ├── ReadOnlyAttribute.cs              # 只读标记
│   │   └── WriteOnlyAttribute.cs             # 只写标记
│   │
│   ├── Internal/
│   │   ├── BurstHelpers.cs                   # Burst辅助类
│   │   ├── JobHelpers.cs                     # Job辅助类
│   │   └── NativeHelpers.cs                  # Native辅助类
│   │
│   ├── AssemblyInfo.cs
│   └── AFramework.Burst.asmdef
│
├── Editor/——————————————————————————————————————
│   │
│   ├── Inspectors/
│   │   ├── BurstJobInspector.cs              # Job检查器
│   │   └── NativeContainerInspector.cs       # Native容器检查器
│   │
│   ├── Tools/
│   │   ├── BurstInspectorWindow.cs           # Burst检查窗口
│   │   ├── JobProfilerWindow.cs              # Job性能分析窗口
│   │   └── NativeMemoryViewer.cs             # Native内存查看器
│   │
│   ├── CodeGen/
│   │   ├── JobCodeGenerator.cs               # Job代码生成器
│   │   └── BurstCodeAnalyzer.cs              # Burst代码分析器
│   │
│   ├── AssemblyInfo.cs
│   └── AFramework.Burst.Editor.asmdef
│
├── Tests/
│   ├── Runtime/
│   │   ├── Jobs/
│   │   │   ├── JobBaseTests.cs
│   │   │   ├── ParallelJobTests.cs
│   │   │   └── JobSchedulerTests.cs
│   │   │
│   │   ├── NativeContainers/
│   │   │   ├── NativeRingBufferTests.cs
│   │   │   ├── NativePriorityQueueTests.cs
│   │   │   └── NativePoolTests.cs
│   │   │
│   │   ├── Performance/
│   │   │   ├── SortPerformanceTests.cs
│   │   │   ├── SimdPerformanceTests.cs
│   │   │   └── JobPerformanceTests.cs
│   │   │
│   │   └── AFramework.Burst.Tests.asmdef
│   │
│   └── Editor/
│       └── AFramework.Burst.Editor.Tests.asmdef
│
├── Documentation~/
│   ├── index.md
│   ├── getting-started.md
│   ├── jobs-guide.md
│   ├── simd-optimization.md
│   └── best-practices.md
│
├── Samples~/
│   ├── BasicJobs/
│   │   ├── SimpleJobSample.cs
│   │   ├── ParallelJobSample.cs
│   │   └── JobDependencySample.cs
│   │
│   ├── AdvancedJobs/
│   │   ├── ParallelSortSample.cs
│   │   ├── SimdMathSample.cs
│   │   └── FunctionPointerSample.cs
│   │
│   └── Performance/
│       ├── BenchmarkSample.cs
│       └── OptimizationSample.cs
│
├── package.json
├── README.md
├── CHANGELOG.md
└── LICENSE.md
```

### 3.3 核心文件说明

#### 3.3.1 Job封装文件内容规范

| 文件 | 主要功能 | 设计要点 |
|------|----------|----------|
| JobBase.cs | IJob基类封装 | 简化Job定义、自动调度 |
| ParallelJobBase.cs | IJobParallelFor基类 | 批处理大小优化 |
| JobScheduler.cs | Job调度管理 | 依赖管理、批量调度 |
| JobBatcher.cs | Job批处理 | 减少调度开销 |

#### 3.3.2 Native容器扩展规范

| 文件 | 主要方法 | 设计要点 |
|------|----------|----------|
| NativeArrayExtensions.cs | Fill, Copy, Sort, BinarySearch | Burst兼容、零分配 |
| NativeListExtensions.cs | AddRange, RemoveAt, Sort | 容量管理优化 |
| NativeRingBuffer.cs | Enqueue, Dequeue, Peek | 无锁设计、固定容量 |
| NativePriorityQueue.cs | Push, Pop, Peek | 堆实现、泛型比较 |



---

## 4. UnityExtension 模块

### 4.1 模块概述

| 属性 | 值 |
|------|-----|
| 命名空间 | AFramework.UnityExtension |
| 程序集 | AFramework.UnityExtension |
| 依赖 | AFramework.CSharpExtension |
| 定位 | Unity特定类型扩展库，Unity开发增强 |

### 4.2 完整目录结构

```
Plugins/AFramework/UnityExtension/
│
├── Runtime/
│   │
│   ├── Core/
│   │   │
│   │   ├── Constants/
│   │   │   ├── LayerConstants.cs             # 层级常量定义
│   │   │   ├── TagConstants.cs               # 标签常量定义
│   │   │   ├── SortingLayerConstants.cs      # 排序层常量
│   │   │   └── PhysicsConstants.cs           # 物理常量定义
│   │   │
│   │   ├── Enums/
│   │   │   ├── Axis.cs                       # 轴向枚举
│   │   │   ├── Direction.cs                  # 方向枚举
│   │   │   ├── Plane.cs                      # 平面枚举
│   │   │   ├── Space2D.cs                    # 2D空间枚举
│   │   │   └── UpdateMode.cs                 # 更新模式枚举
│   │   │
│   │   └── Structs/
│   │       ├── TransformData.cs              # Transform数据结构
│   │       ├── RectData.cs                   # Rect数据结构
│   │       ├── BoundsInt2D.cs                # 2D整数边界
│   │       └── ColorHSV.cs                   # HSV颜色结构
│   │
│   ├── Extensions/
│   │   │
│   │   ├── Transform/
│   │   │   ├── TransformExtensions.cs        # Transform基础扩展
│   │   │   ├── TransformExtensions.Position.cs   # 位置操作 (partial)
│   │   │   ├── TransformExtensions.Rotation.cs   # 旋转操作 (partial)
│   │   │   ├── TransformExtensions.Scale.cs      # 缩放操作 (partial)
│   │   │   ├── TransformExtensions.Hierarchy.cs  # 层级操作 (partial)
│   │   │   └── TransformExtensions.Search.cs     # 查找操作 (partial)
│   │   │
│   │   ├── GameObject/
│   │   │   ├── GameObjectExtensions.cs       # GameObject基础扩展
│   │   │   ├── GameObjectExtensions.Component.cs # 组件操作 (partial)
│   │   │   ├── GameObjectExtensions.Hierarchy.cs # 层级操作 (partial)
│   │   │   ├── GameObjectExtensions.Layer.cs     # 层级设置 (partial)
│   │   │   └── GameObjectExtensions.Active.cs    # 激活控制 (partial)
│   │   │
│   │   ├── Component/
│   │   │   ├── ComponentExtensions.cs        # Component基础扩展
│   │   │   ├── MonoBehaviourExtensions.cs    # MonoBehaviour扩展
│   │   │   ├── BehaviourExtensions.cs        # Behaviour扩展
│   │   │   └── RendererExtensions.cs         # Renderer扩展
│   │   │
│   │   ├── Vector/
│   │   │   ├── Vector2Extensions.cs          # Vector2扩展方法
│   │   │   ├── Vector3Extensions.cs          # Vector3扩展方法
│   │   │   ├── Vector4Extensions.cs          # Vector4扩展方法
│   │   │   ├── Vector2IntExtensions.cs       # Vector2Int扩展方法
│   │   │   ├── Vector3IntExtensions.cs       # Vector3Int扩展方法
│   │   │   └── VectorConversionExtensions.cs # 向量转换扩展
│   │   │
│   │   ├── Quaternion/
│   │   │   ├── QuaternionExtensions.cs       # Quaternion扩展方法
│   │   │   └── QuaternionMathExtensions.cs   # 四元数数学扩展
│   │   │
│   │   ├── Color/
│   │   │   ├── ColorExtensions.cs            # Color扩展方法
│   │   │   ├── Color32Extensions.cs          # Color32扩展方法
│   │   │   ├── ColorConversionExtensions.cs  # 颜色转换扩展
│   │   │   └── ColorBlendExtensions.cs       # 颜色混合扩展
│   │   │
│   │   ├── Rect/
│   │   │   ├── RectExtensions.cs             # Rect扩展方法
│   │   │   ├── RectIntExtensions.cs          # RectInt扩展方法
│   │   │   ├── RectTransformExtensions.cs    # RectTransform扩展
│   │   │   └── BoundsExtensions.cs           # Bounds扩展方法
│   │   │
│   │   ├── Physics/
│   │   │   ├── RigidbodyExtensions.cs        # Rigidbody扩展
│   │   │   ├── Rigidbody2DExtensions.cs      # Rigidbody2D扩展
│   │   │   ├── ColliderExtensions.cs         # Collider扩展
│   │   │   ├── Collider2DExtensions.cs       # Collider2D扩展
│   │   │   ├── RaycastHitExtensions.cs       # RaycastHit扩展
│   │   │   └── PhysicsMaterialExtensions.cs  # 物理材质扩展
│   │   │
│   │   ├── Animation/
│   │   │   ├── AnimatorExtensions.cs         # Animator扩展
│   │   │   ├── AnimationExtensions.cs        # Animation扩展
│   │   │   ├── AnimationCurveExtensions.cs   # AnimationCurve扩展
│   │   │   └── AnimationClipExtensions.cs    # AnimationClip扩展
│   │   │
│   │   ├── Audio/
│   │   │   ├── AudioSourceExtensions.cs      # AudioSource扩展
│   │   │   ├── AudioClipExtensions.cs        # AudioClip扩展
│   │   │   └── AudioMixerExtensions.cs       # AudioMixer扩展
│   │   │
│   │   ├── UI/_________________________________________________________________
│   │   │   ├── CanvasExtensions.cs           # Canvas扩展
│   │   │   ├── CanvasGroupExtensions.cs      # CanvasGroup扩展
│   │   │   ├── GraphicExtensions.cs          # Graphic扩展
│   │   │   ├── ImageExtensions.cs            # Image扩展
│   │   │   ├── TextExtensions.cs             # Text/TMP扩展
│   │   │   ├── ButtonExtensions.cs           # Button扩展
│   │   │   ├── ScrollRectExtensions.cs       # ScrollRect扩展
│   │   │   └── LayoutGroupExtensions.cs      # LayoutGroup扩展
│   │   │
│   │   ├── Rendering/___________________________________________________
│   │   │   ├── MaterialExtensions.cs         # Material扩展
│   │   │   ├── ShaderExtensions.cs           # Shader扩展
│   │   │   ├── TextureExtensions.cs          # Texture扩展
│   │   │   ├── Texture2DExtensions.cs        # Texture2D扩展
│   │   │   ├── SpriteExtensions.cs           # Sprite扩展
│   │   │   ├── MeshExtensions.cs             # Mesh扩展
│   │   │   └── CameraExtensions.cs           # Camera扩展
│   │   │
│   │   ├── Input/
│   │   │   ├── InputExtensions.cs            # Input扩展
│   │   │   ├── TouchExtensions.cs            # Touch扩展
│   │   │   └── KeyCodeExtensions.cs          # KeyCode扩展
│   │   │
│   │   └── Misc/
│   │       ├── LayerMaskExtensions.cs        # LayerMask扩展
│   │       ├── SceneExtensions.cs            # Scene扩展
│   │       ├── AsyncOperationExtensions.cs   # AsyncOperation扩展
│   │       └── WaitForExtensions.cs          # WaitFor扩展
│   │
│   ├── Utilities/_________________________________________________________________
│   │   │
│   │   ├── Math/
│   │   │   ├── UnityMathUtility.cs           # Unity数学工具
│   │   │   ├── VectorMathUtility.cs          # 向量数学工具
│   │   │   ├── QuaternionMathUtility.cs      # 四元数数学工具
│   │   │   ├── GeometryUtility.cs            # 几何计算工具
│   │   │   ├── InterpolationUtility.cs       # 插值工具
│   │   │   └── EasingUtility.cs              # 缓动函数工具
│   │   │
│   │   ├── Random/
│   │   │   ├── UnityRandomUtility.cs         # Unity随机工具
│   │   │   ├── RandomPointUtility.cs         # 随机点生成工具
│   │   │   └── NoiseUtility.cs               # 噪声生成工具
│   │   │
│   │   ├── Physics/
│   │   │   ├── PhysicsUtility.cs             # 物理工具类
│   │   │   ├── Physics2DUtility.cs           # 2D物理工具类
│   │   │   ├── RaycastUtility.cs             # 射线检测工具
│   │   │   ├── OverlapUtility.cs             # 重叠检测工具
│   │   │   └── CollisionUtility.cs           # 碰撞工具类
│   │   │
│   │   ├── Transform/
│   │   │   ├── TransformUtility.cs           # Transform工具类
│   │   │   ├── HierarchyUtility.cs           # 层级工具类
│   │   │   └── TransformPathUtility.cs       # Transform路径工具
│   │   │
│   │   ├── GameObject/
│   │   │   ├── GameObjectUtility.cs          # GameObject工具类
│   │   │   ├── PrefabUtility.Runtime.cs      # 运行时Prefab工具
│   │   │   └── InstantiateUtility.cs         # 实例化工具
│   │   │
│   │   ├── Color/
│   │   │   ├── ColorUtility.cs               # 颜色工具类
│   │   │   ├── GradientUtility.cs            # 渐变工具类
│   │   │   └── PaletteUtility.cs             # 调色板工具
│   │   │
│   │   ├── Gizmos/
│   │   │   ├── GizmosUtility.cs              # Gizmos绘制工具
│   │   │   ├── GizmosEx.cs                   # Gizmos扩展绘制
│   │   │   ├── DebugDrawUtility.cs           # Debug绘制工具
│   │   │   └── HandlesUtility.Runtime.cs     # 运行时Handles
│   │   │
│   │   ├── Layer/
│   │   │   ├── LayerUtility.cs               # 层级工具类
│   │   │   ├── LayerMaskUtility.cs           # LayerMask工具
│   │   │   └── SortingLayerUtility.cs        # 排序层工具
│   │   │
│   │   ├── Screen/
│   │   │   ├── ScreenUtility.cs              # 屏幕工具类
│   │   │   ├── ResolutionUtility.cs          # 分辨率工具
│   │   │   └── SafeAreaUtility.cs            # 安全区域工具
│   │   │
│   │   ├── Time/
│   │   │   ├── TimeUtility.cs                # 时间工具类
│   │   │   ├── FrameRateUtility.cs           # 帧率工具
│   │   │   └── TimeScaleUtility.cs           # 时间缩放工具
│   │   │
│   │   ├── Application/
│   │   │   ├── ApplicationUtility.cs         # 应用程序工具
│   │   │   ├── PlatformUtility.cs            # 平台检测工具
│   │   │   └── PathUtility.Unity.cs          # Unity路径工具
│   │   │
│   │   └── Coroutine/
│   │       ├── CoroutineUtility.cs           # 协程工具类
│   │       ├── CoroutineRunner.cs            # 协程运行器
│   │       ├── WaitForUtility.cs             # 等待工具类
│   │       └── CoroutineExtensions.cs        # 协程扩展
│   │
│   ├── Components/_______________________________
│   │   │
│   │   ├── Lifecycle/
│   │   │   ├── SingletonBehaviour.cs         # 单例MonoBehaviour
│   │   │   ├── PersistentBehaviour.cs        # 持久化MonoBehaviour
│   │   │   ├── AutoDestroyBehaviour.cs       # 自动销毁组件
│   │   │   └── LifecycleEvents.cs            # 生命周期事件组件
│   │   │
│   │   ├── Transform/
│   │   │   ├── FollowTarget.cs               # 跟随目标组件
│   │   │   ├── LookAtTarget.cs               # 朝向目标组件
│   │   │   ├── ConstrainPosition.cs          # 位置约束组件
│   │   │   └── SmoothFollow.cs               # 平滑跟随组件
│   │   │
│   │   ├── Physics/
│   │   │   ├── TriggerEvents.cs              # 触发器事件组件
│   │   │   ├── CollisionEvents.cs            # 碰撞事件组件
│   │   │   └── PhysicsEvents2D.cs            # 2D物理事件组件
│   │   │
│   │   ├── UI/
│   │   │   ├── SafeAreaAdapter.cs            # 安全区域适配器
│   │   │   ├── AspectRatioFitter.Ex.cs       # 宽高比适配器扩展
│   │   │   └── CanvasScalerHelper.cs         # Canvas缩放辅助
│   │   │
│   │   └── Debug/
│   │       ├── FPSDisplay.cs                 # FPS显示组件
│   │       ├── DebugConsole.cs               # 调试控制台组件
│   │       └── GizmoDrawer.cs                # Gizmo绘制组件
│   │
│   ├── Attributes/
│   │   ├── ReadOnlyAttribute.cs              # 只读字段特性
│   │   ├── ConditionalFieldAttribute.cs      # 条件显示特性
│   │   ├── MinMaxSliderAttribute.cs          # 范围滑块特性
│   │   ├── TagSelectorAttribute.cs           # 标签选择特性
│   │   ├── LayerSelectorAttribute.cs         # 层级选择特性
│   │   ├── SceneNameAttribute.cs             # 场景名称特性
│   │   ├── RequireInterfaceAttribute.cs      # 接口要求特性
│   │   ├── ButtonAttribute.cs                # 按钮特性
│   │   ├── FoldoutGroupAttribute.cs          # 折叠组特性
│   │   └── InfoBoxAttribute.cs               # 信息框特性
│   │
│   ├── Interfaces/
│   │   ├── IInitializable.cs                 # 可初始化接口
│   │   ├── ITickable.cs                      # 可Tick接口
│   │   ├── IFixedTickable.cs                 # 固定Tick接口
│   │   ├── ILateTickable.cs                  # 延迟Tick接口
│   │   ├── IPausable.cs                      # 可暂停接口
│   │   └── IPoolableGameObject.cs            # 可池化GameObject接口
│   │
│   ├── Internal/
│   │   ├── UnityMainThread.cs                # 主线程调度
│   │   ├── ComponentCache.cs                 # 组件缓存
│   │   └── TransformCache.cs                 # Transform缓存
│   │
│   ├── AssemblyInfo.cs
│   └── AFramework.UnityExtension.asmdef
│
├── Editor/
│   │
│   ├── Inspectors/
│   │   ├── ReadOnlyDrawer.cs                 # 只读特性绘制器
│   │   ├── ConditionalFieldDrawer.cs         # 条件字段绘制器
│   │   ├── MinMaxSliderDrawer.cs             # 范围滑块绘制器
│   │   ├── TagSelectorDrawer.cs              # 标签选择绘制器
│   │   ├── LayerSelectorDrawer.cs            # 层级选择绘制器
│   │   ├── SceneNameDrawer.cs                # 场景名称绘制器
│   │   ├── ButtonDrawer.cs                   # 按钮绘制器
│   │   ├── FoldoutGroupDrawer.cs             # 折叠组绘制器
│   │   └── InfoBoxDrawer.cs                  # 信息框绘制器
│   │
│   ├── Windows/
│   │   ├── TransformCopyWindow.cs            # Transform复制窗口
│   │   ├── HierarchySearchWindow.cs          # 层级搜索窗口
│   │   ├── ComponentFinderWindow.cs          # 组件查找窗口
│   │   └── MissingReferenceFinderWindow.cs   # 丢失引用查找窗口
│   │
│   ├── Utilities/
│   │   ├── EditorGUIUtilityEx.cs             # EditorGUI扩展
│   │   ├── EditorGUILayoutEx.cs              # EditorGUILayout扩展
│   │   ├── SerializedPropertyExtensions.cs   # SerializedProperty扩展
│   │   ├── EditorPrefsUtility.cs             # EditorPrefs工具
│   │   └── AssetDatabaseUtility.cs           # AssetDatabase工具
│   │
│   ├── Hierarchy/
│   │   ├── HierarchyIcons.cs                 # 层级图标扩展
│   │   ├── HierarchyColorizer.cs             # 层级着色器
│   │   └── HierarchyDivider.cs               # 层级分隔线
│   │
│   ├── Shortcuts/
│   │   ├── TransformShortcuts.cs             # Transform快捷键
│   │   ├── HierarchyShortcuts.cs             # 层级快捷键
│   │   └── SelectionShortcuts.cs             # 选择快捷键
│   │
│   ├── AssemblyInfo.cs
│   └── AFramework.UnityExtension.Editor.asmdef
│
├── Tests/
│   ├── Runtime/
│   │   ├── Extensions/
│   │   │   ├── TransformExtensionsTests.cs
│   │   │   ├── GameObjectExtensionsTests.cs
│   │   │   ├── VectorExtensionsTests.cs
│   │   │   ├── ColorExtensionsTests.cs
│   │   │   └── ...
│   │   │
│   │   ├── Utilities/
│   │   │   ├── UnityMathUtilityTests.cs
│   │   │   ├── PhysicsUtilityTests.cs
│   │   │   ├── ColorUtilityTests.cs
│   │   │   └── ...
│   │   │
│   │   ├── Components/
│   │   │   ├── SingletonBehaviourTests.cs
│   │   │   ├── FollowTargetTests.cs
│   │   │   └── ...
│   │   │
│   │   └── AFramework.UnityExtension.Tests.asmdef
│   │
│   └── Editor/
│       └── AFramework.UnityExtension.Editor.Tests.asmdef
│
├── Documentation~/
│   ├── index.md
│   ├── getting-started.md
│   ├── api/
│   │   ├── extensions.md
│   │   ├── utilities.md
│   │   ├── components.md
│   │   └── attributes.md
│   └── images/
│
├── Samples~/
│   ├── BasicUsage/
│   │   ├── TransformExtensionsSample.cs
│   │   ├── GameObjectExtensionsSample.cs
│   │   ├── VectorMathSample.cs
│   │   └── ColorUtilitySample.cs
│   │
│   ├── Components/
│   │   ├── SingletonSample.cs
│   │   ├── FollowTargetSample.cs
│   │   └── TriggerEventsSample.cs
│   │
│   └── Editor/
│       ├── CustomAttributesSample.cs
│       └── EditorWindowSample.cs
│
├── package.json
├── README.md
├── CHANGELOG.md
└── LICENSE.md
```



### 4.3 核心文件说明

#### 4.3.1 Transform扩展文件内容规范

| 文件 | 主要方法 | 设计要点 |
|------|----------|----------|
| TransformExtensions.Position.cs | SetPositionX/Y/Z, AddPosition, GetWorldPosition | 链式调用、空间转换 |
| TransformExtensions.Rotation.cs | SetRotationX/Y/Z, LookAt2D, RotateAround | 欧拉角/四元数兼容 |
| TransformExtensions.Scale.cs | SetScaleX/Y/Z, SetUniformScale | 局部/世界缩放 |
| TransformExtensions.Hierarchy.cs | GetPath, SetParentAndReset, DestroyChildren | 层级操作优化 |
| TransformExtensions.Search.cs | FindDeep, FindByPath, GetComponentInParentOrChildren | 递归搜索优化 |

#### 4.3.2 工具类文件内容规范

| 文件 | 主要功能 | 设计要点 |
|------|----------|----------|
| UnityMathUtility.cs | 向量计算、角度转换、距离计算 | 内联优化 |
| PhysicsUtility.cs | 射线检测、重叠检测、碰撞预测 | 结果缓存、批量检测 |
| ColorUtility.cs | 颜色转换、混合、调整 | HSV/RGB/Hex转换 |
| GizmosUtility.cs | 形状绘制、网格绘制、调试可视化 | 条件编译 |
| CoroutineUtility.cs | 协程管理、延迟执行、序列执行 | 取消支持 |

---

## 5. 模块间依赖关系

### 5.1 依赖图

```
┌─────────────────────────────────────────────────────────────────┐
│                        模块依赖关系                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                    ┌─────────────────────┐                      │
│                    │   UnityExtension    │                      │
│                    │ (AFramework.UnityExtension)                │
│                    └──────────┬──────────┘                      │
│                               │                                 │
│                               │ 依赖                            │
│                               ▼                                 │
│                    ┌─────────────────────┐                      │
│                    │  CSharpExtension    │                      │
│                    │(AFramework.CSharpExtension)                │
│                    └─────────────────────┘                      │
│                                                                 │
│                    ┌─────────────────────┐                      │
│                    │   BurstExtension    │                      │
│                    │  (AFramework.Burst) │                      │
│                    └──────────┬──────────┘                      │
│                               │                                 │
│                               │ 依赖                            │
│                               ▼                                 │
│                    ┌─────────────────────┐                      │
│                    │   Unity Packages    │                      │
│                    │ - com.unity.burst   │                      │
│                    │ - com.unity.collections                    │
│                    │ - com.unity.mathematics                    │
│                    └─────────────────────┘                      │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 程序集引用配置

#### CSharpExtension.asmdef
```json
{
    "name": "AFramework.CSharpExtension",
    "rootNamespace": "AFramework.CSharpExtension",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": true,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": true
}
```

#### BurstExtension.asmdef
```json
{
    "name": "AFramework.Burst",
    "rootNamespace": "AFramework.Burst",
    "references": [
        "Unity.Burst",
        "Unity.Collections",
        "Unity.Mathematics"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": true,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

#### UnityExtension.asmdef
```json
{
    "name": "AFramework.UnityExtension",
    "rootNamespace": "AFramework.UnityExtension",
    "references": [
        "AFramework.CSharpExtension"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

---

## 6. 文件命名规范

### 6.1 命名规则

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| 扩展方法类 | {类型名}Extensions.cs | StringExtensions.cs |
| 工具类 | {功能}Utility.cs | MathUtility.cs |
| 接口 | I{名称}.cs | IPoolable.cs |
| 特性 | {名称}Attribute.cs | ReadOnlyAttribute.cs |
| 枚举 | {名称}.cs | SortOrder.cs |
| 结构体 | {名称}.cs | Range.cs |
| 常量类 | {类型}Constants.cs | MathConstants.cs |
| 测试类 | {被测类}Tests.cs | StringExtensionsTests.cs |

### 6.2 Partial类命名

| 规则 | 示例 |
|------|------|
| 主文件 | TransformExtensions.cs |
| 分部文件 | TransformExtensions.{功能}.cs |
| 功能分类 | Position, Rotation, Scale, Hierarchy |

### 6.3 目录命名

| 目录类型 | 命名规则 | 示例 |
|----------|----------|------|
| 功能目录 | PascalCase | Extensions, Utilities |
| 子功能目录 | PascalCase | Collections, Primitives |
| 测试目录 | Tests | Tests/Runtime, Tests/Editor |
| 文档目录 | 带~后缀 | Documentation~, Samples~ |

---

## 附录

### A. 版本兼容性预处理指令

```csharp
// Unity版本检测
#if UNITY_2022_3_OR_NEWER
    // Unity 2022.3+ 特有代码
#endif

#if UNITY_2023_1_OR_NEWER
    // Unity 2023.1+ 特有代码
#endif

#if UNITY_6000_0_OR_NEWER
    // Unity 6.x 特有代码
#endif

// 平台检测
#if UNITY_EDITOR
    // 编辑器专用代码
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
    // 运行时代码
#endif

// Burst检测
#if UNITY_BURST_EXPERIMENTAL_LOOP_INTRINSICS
    // Burst实验性特性
#endif
```

### B. 文件头模板

```csharp
// ==========================================================
// 文件名：{FileName}.cs
// 命名空间：AFramework.{Module}
// 创建时间：{Date}
// 功能描述：{Description}
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.{Module}
{
    /// <summary>
    /// {类描述}
    /// </summary>
    public static class {ClassName}
    {
        // 实现代码
    }
}
```

---

> 文档结束
> 
> Aggregation Framework (AFramework) - 基础层模块目录结构设计
> 
> 版权所有 © 2025
