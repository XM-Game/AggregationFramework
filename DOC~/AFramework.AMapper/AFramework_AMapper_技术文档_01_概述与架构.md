# AFramework.AMapper 技术文档

## 第一部分：概述与核心架构

---

## 1. 项目概述

### 1.1 什么是 AMapper

AMapper 是 AFramework 聚合框架中的轻量级对象-对象映射组件，专为 Unity 游戏开发设计。它基于约定的映射机制，自动完成源类型到目标类型的属性映射，大幅减少手写映射代码的工作量，同时针对 Unity 运行时环境进行了深度优化。

### 1.2 设计目标

| 目标 | 说明 |
|------|------|
| Unity 原生兼容 | 完全兼容 Unity 2022.3 LTS 至 Unity 6.x 全版本 |
| 零外部依赖 | 不依赖任何第三方库，纯 C# 实现 |
| 高性能 | 表达式树编译、对象池复用、零 GC 分配优化 |
| 轻量级 | 核心代码精简，按需加载，最小化内存占用 |
| 易于集成 | 与 AFramework.DI 依赖注入框架无缝集成 |
| 游戏友好 | 支持 Unity 特有类型（Vector3、Quaternion、Color 等） |

### 1.3 核心价值

- **消除样板代码**：自动处理属性名称匹配的映射逻辑
- **约定优于配置**：遵循命名约定时几乎零配置
- **类型安全**：编译时类型检查，运行时配置验证
- **高度可扩展**：支持自定义转换器、解析器、值转换器等
- **Unity 深度集成**：支持 ScriptableObject、MonoBehaviour、Unity 数学类型

### 1.4 版本信息

- **框架版本**：1.0.0
- **目标框架**：.NET Standard 2.1
- **Unity 版本**：Unity 2022.3 LTS 及以上
- **许可证**：MIT License

### 1.5 与 AutoMapper 的对比

| 特性 | AMapper | AutoMapper |
|------|---------|------------|
| 目标平台 | Unity 专用 | 通用 .NET |
| 外部依赖 | 无 | Microsoft.Extensions.* |
| IL2CPP 兼容 | 完全兼容 | 部分功能受限 |
| Unity 类型支持 | 原生支持 | 需要额外配置 |
| 包体积 | < 50KB | > 500KB |
| LINQ 投影 | 简化版本 | 完整支持 |
| 许可证 | MIT（免费） | 商业许可 |

---

## 2. 核心架构

### 2.1 架构层次

AMapper 的架构分为以下核心层次：

```
┌─────────────────────────────────────────────────────────────┐
│                      API 层 (IAMapper)                       │
│  - Map<TDestination>()                                       │
│  - Map(source, destination)                                  │
│  - ProjectTo<TDestination>()                                 │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   配置层 (MapperConfiguration)               │
│  - Profile 管理                                              │
│  - TypeMap 注册                                              │
│  - 执行计划编译                                               │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   类型映射层 (TypeMap)                       │
│  - MemberMap（成员映射）                                      │
│  - ConstructorMap（构造函数映射）                             │
│  - PathMap（路径映射）                                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   执行层 (Execution)                         │
│  - ExpressionBuilder（表达式构建）                            │
│  - ExecutionPlanBuilder（执行计划构建）                       │
│  - ObjectFactory（对象工厂）                                  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   映射器层 (Mappers)                         │
│  - CollectionMapper（集合映射器）                             │
│  - UnityTypeMapper（Unity 类型映射器）                        │
│  - EnumMapper（枚举映射器）                                   │
│  - ConvertMapper（转换映射器）                                │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 核心组件

#### 2.2.1 IMapperConfiguration / MapperConfiguration

配置提供者是 AMapper 的核心配置容器，负责：

- 存储所有类型映射配置
- 管理 Profile 实例
- 编译执行计划
- 验证配置有效性
- 创建 Mapper 实例

**关键特性**：
- 线程安全
- 不可变（创建后不能修改）
- 支持延迟编译和预编译
- 单例模式（推荐每个应用一个实例）

#### 2.2.2 IAMapper / AMapper

映射器是执行实际映射操作的组件：

- 执行源对象到目标对象的转换
- 支持泛型和非泛型映射方法
- 支持映射到现有对象
- 支持简化版 LINQ 投影

**关键特性**：
- 轻量级，可按需创建
- 支持运行时映射选项
- 集成 AFramework.DI 依赖注入

#### 2.2.3 MappingProfile

Profile 是组织映射配置的推荐方式：

- 继承自 MappingProfile 基类
- 在构造函数中定义映射
- 支持命名约定配置
- 支持全局忽略规则
- 配置作用域隔离

#### 2.2.4 TypeMap

TypeMap 是单个源-目标类型对的映射配置：

- 包含所有成员映射（MemberMap）
- 包含构造函数映射（ConstructorMap）
- 包含路径映射（PathMap）
- 存储前置/后置映射动作
- 管理值转换器配置
- 支持继承映射

#### 2.2.5 MemberMap

MemberMap 定义单个成员的映射规则：

- 源成员到目标成员的映射
- 条件映射配置（Condition/PreCondition）
- 值解析器配置（IValueResolver）
- 空值替换配置（NullSubstitute）
- 映射顺序配置（MappingOrder）
- 值转换器配置（ValueTransformers）

#### 2.2.6 ConstructorMap

ConstructorMap 管理构造函数参数映射：

- 存储 ConstructorInfo 引用
- 管理 ConstructorParameterMap 集合
- CanResolve 属性判断所有参数是否可解析
- 支持从继承映射应用配置

### 2.3 执行流程

#### 2.3.1 配置阶段

1. 创建 MapperConfiguration 实例
2. 扫描并注册 Profile
3. 解析 CreateMap 配置
4. 构建 TypeMap 集合
5. 处理继承映射（Include/IncludeBase）
6. 密封配置（Seal）
7. 可选：预编译映射（CompileMappings）

#### 2.3.2 映射阶段

1. 接收 Map 调用
2. 解析源/目标类型对
3. 查找或创建 TypeMap
4. 获取或编译执行计划
5. 执行映射表达式
6. 返回目标对象

### 2.4 执行计划

AMapper 使用表达式树（Expression Tree）构建执行计划：

- 延迟编译：首次映射时编译
- 缓存机制：编译后的委托被缓存
- 可调试：可通过 BuildExecutionPlan 查看
- 高性能：编译后的代码接近手写性能

**执行计划包含**：
- 源对象空值检查
- 目标对象创建（构造函数或工厂）
- 成员映射表达式
- 条件映射逻辑
- 值转换器调用
- 循环引用处理（PreserveReferences）
- 深度检查（MaxDepth）

---

## 3. 目录结构

```
AFramework/
└── AMapper/
    ├── Runtime/
    │   ├── Core/                       # 核心接口与基类
    │   │   ├── Interfaces/
    │   │   │   ├── IAMapper.cs                 # 映射器接口
    │   │   │   ├── IMapperConfiguration.cs     # 配置接口
    │   │   │   ├── ITypeMap.cs                 # 类型映射接口
    │   │   │   ├── IMemberMap.cs               # 成员映射接口
    │   │   │   ├── IValueResolver.cs           # 值解析器接口
    │   │   │   ├── ITypeConverter.cs           # 类型转换器接口
    │   │   │   ├── IValueConverter.cs          # 值转换器接口
    │   │   │   ├── IMappingAction.cs           # 映射动作接口
    │   │   │   └── IObjectMapper.cs            # 对象映射器接口
    │   │   ├── Attributes/
    │   │   │   ├── AutoMapAttribute.cs         # 自动映射特性
    │   │   │   ├── IgnoreAttribute.cs          # 忽略特性
    │   │   │   ├── MapFromAttribute.cs         # 映射来源特性
    │   │   │   └── MapToAttribute.cs           # 映射目标特性
    │   │   └── Exceptions/
    │   │       ├── MappingException.cs         # 映射异常
    │   │       └── ConfigurationException.cs   # 配置异常
    │   │
    │   ├── Configuration/              # 配置系统
    │   │   ├── MapperConfiguration.cs          # 配置容器
    │   │   ├── MapperConfigurationBuilder.cs   # 配置构建器
    │   │   ├── MappingProfile.cs               # 配置文件基类
    │   │   ├── MappingExpression.cs            # 映射表达式配置
    │   │   ├── MemberConfigurationExpression.cs # 成员配置表达式
    │   │   ├── TypeMap.cs                      # 类型映射
    │   │   ├── MemberMap.cs                    # 成员映射
    │   │   ├── ConstructorMap.cs               # 构造函数映射
    │   │   ├── PathMap.cs                      # 路径映射
    │   │   └── Conventions/
    │   │       ├── INamingConvention.cs        # 命名约定接口
    │   │       ├── PascalCaseNamingConvention.cs
    │   │       ├── CamelCaseNamingConvention.cs
    │   │       └── SnakeCaseNamingConvention.cs
    │   │
    │   ├── Execution/                  # 执行引擎
    │   │   ├── AMapper.cs                      # 映射器实现
    │   │   ├── ExecutionPlanBuilder.cs         # 执行计划构建器
    │   │   ├── ExpressionBuilder.cs            # 表达式构建工具
    │   │   ├── ObjectFactory.cs                # 对象工厂
    │   │   ├── MappingContext.cs               # 映射上下文
    │   │   └── MappingOptions.cs               # 映射选项
    │   │
    │   ├── Mappers/                    # 内置映射器
    │   │   ├── MapperRegistry.cs               # 映射器注册表
    │   │   ├── AssignableMapper.cs             # 可赋值类型映射器
    │   │   ├── CollectionMapper.cs             # 集合映射器
    │   │   ├── ArrayMapper.cs                  # 数组映射器
    │   │   ├── DictionaryMapper.cs             # 字典映射器
    │   │   ├── EnumMapper.cs                   # 枚举映射器
    │   │   ├── NullableMapper.cs               # 可空类型映射器
    │   │   ├── ConvertMapper.cs                # 转换映射器
    │   │   ├── StringMapper.cs                 # 字符串映射器
    │   │   └── ConstructorMapper.cs            # 构造函数映射器
    │   │
    │   ├── Resolvers/                  # 值解析器
    │   │   ├── MemberPathResolver.cs           # 成员路径解析器
    │   │   ├── ExpressionResolver.cs           # 表达式解析器
    │   │   ├── FuncResolver.cs                 # 委托解析器
    │   │   └── ClassValueResolver.cs           # 类值解析器
    │   │
    │   ├── Unity/                      # Unity 特定功能
    │   │   ├── Mappers/
    │   │   │   ├── UnityTypeMapper.cs          # Unity 类型映射器
    │   │   │   ├── Vector3Mapper.cs            # Vector3 映射器
    │   │   │   ├── QuaternionMapper.cs         # Quaternion 映射器
    │   │   │   ├── ColorMapper.cs              # Color 映射器
    │   │   │   └── TransformMapper.cs          # Transform 映射器
    │   │   ├── Converters/
    │   │   │   ├── Vector3Converter.cs         # Vector3 转换器
    │   │   │   ├── QuaternionConverter.cs      # Quaternion 转换器
    │   │   │   └── ColorConverter.cs           # Color 转换器
    │   │   └── Integration/
    │   │       ├── ScriptableObjectProfile.cs  # ScriptableObject 配置
    │   │       └── MonoBehaviourInjector.cs    # MonoBehaviour 注入
    │   │
    │   ├── Extensions/                 # 扩展功能
    │   │   ├── QueryableExtensions.cs          # LINQ 扩展
    │   │   ├── CollectionExtensions.cs         # 集合扩展
    │   │   └── TypeExtensions.cs               # 类型扩展
    │   │
    │   ├── Internal/                   # 内部工具
    │   │   ├── TypePair.cs                     # 类型对
    │   │   ├── TypeDetails.cs                  # 类型详情缓存
    │   │   ├── MemberPath.cs                   # 成员路径
    │   │   ├── ReflectionHelper.cs             # 反射辅助
    │   │   ├── ObjectPool.cs                   # 对象池
    │   │   └── ConcurrentCache.cs              # 并发缓存
    │   │
    │   └── DI/                         # 依赖注入集成
    │       ├── AMapperInstaller.cs             # 安装器
    │       └── AMapperExtensions.cs            # DI 扩展方法
    │
    └── Editor/                         # 编辑器工具
        ├── AMapperWindow.cs                    # 映射配置窗口
        ├── TypeMapInspector.cs                 # TypeMap 检视器
        └── ExecutionPlanViewer.cs              # 执行计划查看器
```

---

## 4. 快速开始

### 4.1 基本配置

```csharp
// 1. 创建配置
var configuration = new MapperConfiguration(cfg => 
{
    cfg.CreateMap<PlayerData, PlayerDto>();
});

// 2. 验证配置（开发阶段）
#if UNITY_EDITOR
configuration.AssertConfigurationIsValid();
#endif

// 3. 创建映射器
var mapper = configuration.CreateMapper();

// 4. 执行映射
var dto = mapper.Map<PlayerDto>(playerData);
```

### 4.2 使用 Profile 配置

```csharp
public class GameProfile : MappingProfile
{
    public GameProfile()
    {
        // 玩家数据映射
        CreateMap<PlayerData, PlayerDto>();
        CreateMap<InventoryItem, ItemDto>();
        
        // 带自定义配置的映射
        CreateMap<Enemy, EnemyDto>()
            .ForMember(d => d.HealthPercent, 
                       opt => opt.MapFrom(s => s.CurrentHealth / s.MaxHealth));
    }
}

// 注册 Profile
var configuration = new MapperConfiguration(cfg => 
{
    cfg.AddProfile<GameProfile>();
});
```

### 4.3 与 AFramework.DI 集成

```csharp
public class GameInstaller : InstallerBase
{
    public override void Install(IContainerBuilder builder)
    {
        // 注册 AMapper
        builder.RegisterAMapper(cfg =>
        {
            cfg.AddProfile<GameProfile>();
            cfg.AddProfile<UIProfile>();
        });
    }
}

// 使用注入的映射器
public class PlayerService
{
    private readonly IAMapper _mapper;
    
    public PlayerService(IAMapper mapper)
    {
        _mapper = mapper;
    }
    
    public PlayerDto GetPlayerDto(PlayerData data)
    {
        return _mapper.Map<PlayerDto>(data);
    }
}
```

---

## 5. 设计原则

### 5.1 约定优于配置

AMapper 遵循"约定优于配置"原则：

- 同名属性自动映射
- 支持扁平化（CustomerName → Customer.Name）
- 支持 Get 前缀方法（GetTotal() → Total）
- 支持 PascalCase 分词匹配

### 5.2 不可变配置

MapperConfiguration 创建后不可修改：

- 线程安全
- 可预测行为
- 支持单例模式

### 5.3 表达式驱动

使用表达式树而非反射：

- 编译时类型检查
- 运行时高性能
- IL2CPP 完全兼容

### 5.4 Unity 优先

针对 Unity 运行时优化：

- 零 GC 分配（热路径）
- 对象池复用
- Unity 类型原生支持
- 编辑器工具集成

### 5.5 可扩展性

提供多个扩展点：

- 自定义类型转换器
- 自定义值解析器
- 自定义值转换器
- 自定义映射器
- Unity 类型扩展

---

## 6. 性能考虑

### 6.1 编译开销

- 首次映射有编译开销
- 可使用 CompileMappings() 预编译
- 建议在加载界面预编译

### 6.2 优化建议

- 使用对象池减少 GC
- 避免过深的对象图
- 合理使用 MaxDepth
- 避免不必要的 PreserveReferences

### 6.3 内存管理

- 执行计划被缓存
- 映射器实例轻量级
- 配置实例应为单例
- 使用 Burst 兼容的数据结构

### 6.4 IL2CPP 兼容性

- 避免运行时代码生成
- 预编译所有映射
- 使用 AOT 友好的反射模式
- 支持 link.xml 配置

---

## 7. Unity 版本兼容性

### 7.1 预处理指令

AMapper 使用预处理指令支持多版本 Unity：

```csharp
#if UNITY_2022_3_OR_NEWER
    // Unity 2022.3+ 特定代码
#endif

#if UNITY_6000_0_OR_NEWER
    // Unity 6.x 特定代码
#endif
```

### 7.2 API 兼容性

| Unity 版本 | 支持状态 | 特殊说明 |
|-----------|---------|---------|
| 2022.3 LTS | 完全支持 | 基准版本 |
| 2023.1+ | 完全支持 | - |
| Unity 6.x | 完全支持 | 新 API 优化 |

---

*下一部分：映射配置详解*
