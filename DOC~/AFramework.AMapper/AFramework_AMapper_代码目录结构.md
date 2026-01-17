# AFramework.AMapper 代码目录结构文档

## 文档说明

本文档详细描述 AMapper 对象映射框架的代码目录结构，包括各模块的职责划分、文件组织方式和命名规范。

---

## 1. 顶层目录结构

```
Assets/Plugins/AFramework/AMapper/
├── Runtime/                    # 运行时代码
├── Editor/                     # 编辑器工具
└── AFramework.AMapper.asmdef   # 程序集定义文件
```

| 目录 | 说明 |
|------|------|
| Runtime | 运行时核心代码，包含映射器、配置、执行引擎等 |
| Editor | Unity 编辑器扩展工具，仅在编辑器环境下使用 |

---

## 2. Runtime 目录结构

### 2.1 总体布局

```
Runtime/
├── Core/                       # 核心层：接口、特性、异常
├── Configuration/              # 配置层：映射配置系统
├── Execution/                  # 执行层：映射执行引擎
├── Mappers/                    # 映射器层：内置类型映射器
├── Resolvers/                  # 解析器层：值解析器实现
├── Unity/                      # Unity层：Unity 特定功能
├── Extensions/                 # 扩展层：扩展方法
├── Internal/                   # 内部层：内部工具类
└── DI/                         # 集成层：依赖注入集成
```

---

### 2.2 Core 目录（核心层）

核心层定义框架的基础契约，包括接口、特性和异常类型。

```
Core/
├── Interfaces/                 # 核心接口定义
│   ├── IAMapper                # 映射器主接口
│   ├── IMapperConfiguration    # 映射配置接口
│   ├── ITypeMap                # 类型映射接口
│   ├── IMemberMap              # 成员映射接口
│   ├── IValueResolver          # 值解析器接口
│   ├── IMemberValueResolver    # 成员值解析器接口
│   ├── ITypeConverter          # 类型转换器接口
│   ├── IValueConverter         # 值转换器接口
│   ├── IMappingAction          # 映射动作接口
│   ├── IObjectMapper           # 对象映射器接口
│   └── IMappingOperationOptions # 映射操作选项接口
│
├── Attributes/                 # 特性定义
│   ├── AutoMapAttribute        # 自动映射特性
│   ├── IgnoreAttribute         # 忽略映射特性
│   ├── MapFromAttribute        # 指定映射来源特性
│   ├── MapToAttribute          # 指定映射目标特性
│   ├── NullSubstituteAttribute # 空值替换特性
│   └── MappingOrderAttribute   # 映射顺序特性
│
└── Exceptions/                 # 异常类型
    ├── MappingException        # 映射执行异常
    ├── ConfigurationException  # 配置验证异常
    └── DuplicateTypeMapException # 重复配置异常
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Interfaces | 定义框架核心契约，供外部实现和内部使用 |
| Attributes | 提供声明式映射配置能力 |
| Exceptions | 定义框架特定异常类型，便于错误处理 |

---

### 2.3 Configuration 目录（配置层）

配置层负责映射配置的定义、解析和管理。

```
Configuration/
├── MapperConfiguration         # 映射配置容器（核心入口）
├── MapperConfigurationBuilder  # 配置构建器
├── MappingProfile              # 映射配置文件基类
├── ProfileMap                  # Profile 运行时表示
│
├── TypeMap/                    # 类型映射
│   ├── TypeMap                 # 类型映射配置
│   ├── TypeMapConfiguration    # 类型映射配置表达式
│   └── TypePair                # 类型对结构体
│
├── MemberMap/                  # 成员映射
│   ├── MemberMap               # 成员映射配置
│   ├── MemberConfigurationExpression # 成员配置表达式
│   ├── PathMap                 # 路径映射（ForPath）
│   └── PathConfigurationExpression # 路径配置表达式
│
├── ConstructorMap/             # 构造函数映射
│   ├── ConstructorMap          # 构造函数映射配置
│   ├── ConstructorParameterMap # 构造函数参数映射
│   └── CtorParamConfigurationExpression # 参数配置表达式
│
├── Conventions/                # 命名约定
│   ├── INamingConvention       # 命名约定接口
│   ├── PascalCaseNamingConvention # PascalCase 约定
│   ├── CamelCaseNamingConvention # camelCase 约定
│   ├── SnakeCaseNamingConvention # snake_case 约定
│   ├── ExactMatchNamingConvention # 精确匹配约定
│   └── MemberConfiguration     # 成员匹配配置
│
└── Validation/                 # 配置验证
    ├── ConfigurationValidator  # 配置验证器
    └── ValidationContext       # 验证上下文
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| 根目录 | 配置容器和 Profile 基类 |
| TypeMap | 类型级别的映射配置 |
| MemberMap | 成员级别的映射配置 |
| ConstructorMap | 构造函数参数映射配置 |
| Conventions | 命名约定和成员匹配规则 |
| Validation | 配置验证逻辑 |

---

### 2.4 Execution 目录（执行层）

执行层负责映射的实际执行，包括执行计划构建和运行时映射。

```
Execution/
├── AMapper                     # 映射器实现类
├── MappingContext              # 映射上下文（运行时状态）
├── MappingOptions              # 映射选项
│
├── Planning/                   # 执行计划
│   ├── ExecutionPlanBuilder    # 执行计划构建器
│   ├── ExpressionBuilder       # 表达式构建工具
│   └── MapRequest              # 映射请求结构体
│
├── Factory/                    # 对象创建
│   ├── ObjectFactory           # 对象工厂
│   └── DestinationFactory      # 目标对象工厂
│
└── Runtime/                    # 运行时支持
    ├── ResolutionContext       # 解析上下文
    └── ReferenceTracker        # 引用跟踪器（循环引用处理）
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| 根目录 | 映射器实现和运行时上下文 |
| Planning | 表达式树构建和执行计划生成 |
| Factory | 目标对象实例化 |
| Runtime | 运行时辅助功能 |

---

### 2.5 Mappers 目录（映射器层）

映射器层包含所有内置的类型映射器实现。

```
Mappers/
├── MapperRegistry               # 映射器注册表
│
├── Primitive/                   # 基元类型映射器
│   ├── AssignableMapper         # 可赋值类型映射器
│   ├── ConvertMapper            # System.Convert 映射器
│   ├── StringMapper             # 字符串映射器
│   └── ParseStringMapper        # Parse 方法映射器
│
├── Nullable/                    # 可空类型映射器
│   ├── NullableSourceMapper     # 可空源类型映射器
│   └── NullableDestinationMapper # 可空目标类型映射器
│
├── Enum/                        # 枚举映射器
│   ├── EnumToEnumMapper         # 枚举到枚举映射器
│   ├── StringToEnumMapper       # 字符串到枚举映射器
│   └── EnumToStringMapper       # 枚举到字符串映射器
│
├── Collection/                  # 集合映射器
│   ├── CollectionMapper         # 通用集合映射器
│   ├── ArrayMapper              # 数组映射器
│   ├── ListMapper               # List 映射器
│   ├── DictionaryMapper         # 字典映射器
│   └── HashSetMapper            # HashSet 映射器
│
├── Constructor/                 # 构造函数映射器
│   ├── ConstructorMapper        # 构造函数映射器
│   └── ConversionOperatorMapper # 转换运算符映射器
│
└── Dynamic/                     # 动态类型映射器
    ├── DictionaryToObjectMapper # 字典到对象映射器
    └── ObjectToDictionaryMapper # 对象到字典映射器
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| 根目录 | 映射器注册和管理 |
| Primitive | 基本类型之间的转换 |
| Nullable | 可空类型处理 |
| Enum | 枚举类型转换 |
| Collection | 集合类型映射 |
| Constructor | 基于构造函数的映射 |
| Dynamic | 动态类型映射 |

---

### 2.6 Resolvers 目录（解析器层）

解析器层包含值解析器的实现。

```
Resolvers/
├── Base/                        # 解析器基类
│   ├── ValueResolverBase        # 值解析器基类
│   └── MemberValueResolverBase  # 成员值解析器基类
│
├── Builtin/                     # 内置解析器
│   ├── MemberPathResolver       # 成员路径解析器
│   ├── ExpressionResolver       # 表达式解析器
│   ├── FuncResolver             # 委托解析器
│   └── ConstantResolver         # 常量解析器
│
└── Custom/                      # 自定义解析器支持
    ├── ClassValueResolver       # 类值解析器包装
    └── ResolverFactory          # 解析器工厂
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Base | 解析器抽象基类 |
| Builtin | 框架内置的解析器实现 |
| Custom | 自定义解析器的支持和包装 |

---

### 2.7 Unity 目录（Unity 层）

Unity 层包含 Unity 引擎特定的功能实现。

```
Unity/
├── Mappers/                     # Unity 类型映射器
│   ├── UnityTypeMapperRegistry  # Unity 类型映射器注册表
│   │
│   ├── Math/                    # 数学类型映射器
│   │   ├── Vector2Mapper        # Vector2 映射器
│   │   ├── Vector3Mapper        # Vector3 映射器
│   │   ├── Vector4Mapper        # Vector4 映射器
│   │   ├── Vector2IntMapper     # Vector2Int 映射器
│   │   ├── Vector3IntMapper     # Vector3Int 映射器
│   │   ├── QuaternionMapper     # Quaternion 映射器
│   │   ├── Matrix4x4Mapper      # Matrix4x4 映射器
│   │   └── BoundsMapper         # Bounds 映射器
│   │
│   ├── Graphics/                # 图形类型映射器
│   │   ├── ColorMapper          # Color 映射器
│   │   ├── Color32Mapper        # Color32 映射器
│   │   └── RectMapper           # Rect 映射器
│   │
│   └── Physics/                 # 物理类型映射器
│       ├── RayMapper            # Ray 映射器
│       └── PlaneMapper          # Plane 映射器
│
├── Converters/                  # Unity 类型转换器
│   ├── VectorConverters         # Vector 系列转换器
│   ├── QuaternionConverters     # Quaternion 转换器
│   ├── ColorConverters          # Color 转换器
│   └── TransformConverters      # Transform 数据转换器
│
├── Profiles/                    # Unity 特定 Profile
│   ├── UnityMathProfile         # Unity 数学类型 Profile
│   ├── UnityGraphicsProfile     # Unity 图形类型 Profile
│   └── ScriptableObjectProfile  # ScriptableObject Profile 基类
│
└── Integration/                 # Unity 集成
    ├── MonoBehaviourMapper      # MonoBehaviour 映射支持
    ├── ComponentMapper          # 组件映射支持
    └── AssetReferenceMapper     # 资源引用映射支持
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Mappers | Unity 特有类型的映射器 |
| Converters | Unity 类型之间的转换器 |
| Profiles | 预配置的 Unity 类型映射 |
| Integration | Unity 引擎集成功能 |

---

### 2.8 Extensions 目录（扩展层）

扩展层提供各种扩展方法。

```
Extensions/
├── Queryable/                   # LINQ 查询扩展
│   ├── QueryableExtensions      # IQueryable 扩展方法
│   └── ProjectionBuilder        # 投影构建器
│
├── Collection/                  # 集合扩展
│   ├── CollectionExtensions     # 集合映射扩展方法
│   └── EnumerableExtensions     # IEnumerable 扩展方法
│
├── Type/                        # 类型扩展
│   ├── TypeExtensions           # Type 扩展方法
│   └── MemberInfoExtensions     # MemberInfo 扩展方法
│
└── Mapper/                      # 映射器扩展
    ├── MapperExtensions         # IAMapper 扩展方法
    └── ConfigurationExtensions  # 配置扩展方法
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Queryable | LINQ 投影支持 |
| Collection | 集合批量映射 |
| Type | 类型反射辅助 |
| Mapper | 映射器便捷方法 |

---

### 2.9 Internal 目录（内部层）___________

内部层包含框架内部使用的工具类。

```
Internal/
├── Caching/                     # 缓存
│   ├── ConcurrentCache          # 并发缓存
│   ├── TypeDetailsCache         # 类型详情缓存
│   └── ExecutionPlanCache       # 执行计划缓存
│
├── Pooling/                     # 对象池
│   ├── ObjectPool               # 通用对象池
│   ├── ListPool                 # List 对象池
│   └── DictionaryPool           # Dictionary 对象池
│
├── Reflection/                  # 反射工具
│   ├── ReflectionHelper         # 反射辅助类
│   ├── TypeDetails              # 类型详情
│   ├── MemberPath               # 成员路径
│   └── ExpressionHelper         # 表达式辅助类
│
└── Threading/                   # 线程安全
    ├── LockingDictionary        # 锁定字典
    └── LazyInitializer          # 延迟初始化器
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Caching | 各类缓存实现 |
| Pooling | 对象池减少 GC |
| Reflection | 反射相关工具 |
| Threading | 线程安全工具 |

---

### 2.10 DI 目录（集成层）

集成层提供与 AFramework.DI 的集成。

```
DI/
├── Registration/                # 注册扩展
│   ├── AMapperRegistrationExtensions # 注册扩展方法
│   └── ServiceCollectionExtensions # 服务集合扩展
│
├── Installers/                  # 安装器
│   ├── AMapperInstaller         # AMapper 安装器基类
│   └── AutoScanInstaller        # 自动扫描安装器
│
└── Resolution/                  # 解析支持
    ├── ServiceProviderAdapter   # 服务提供者适配器
    └── ScopedMapperFactory      # 作用域映射器工厂
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Registration | DI 容器注册扩展 |
| Installers | 模块化安装器 |
| Resolution | 服务解析适配 |

---

## 3. Editor 目录结构

编辑器目录包含 Unity 编辑器扩展工具。

```
Editor/
├── Windows/                     # 编辑器窗口
│   ├── AMapperWindow            # AMapper 主窗口
│   ├── TypeMapBrowser           # TypeMap 浏览器
│   └── ExecutionPlanViewer      # 执行计划查看器
│
├── Inspectors/                  # 检视器
│   ├── ProfileInspector         # Profile 检视器
│   └── TypeMapInspector         # TypeMap 检视器
│
├── Drawers/                     # 属性绘制器
│   ├── AutoMapAttributeDrawer   # AutoMap 特性绘制器
│   └── MappingConfigDrawer      # 映射配置绘制器
│
└── Utilities/                   # 编辑器工具
    ├── CodeGenerator            # 代码生成器
    └── ValidationRunner         # 验证运行器
```

**模块职责**：

| 子目录 | 职责 |
|--------|------|
| Windows | 独立编辑器窗口 |
| Inspectors | 自定义检视器 |
| Drawers | 属性绘制器 |
| Utilities | 编辑器辅助工具 |

---

## 4. 命名空间规范

### 4.1 命名空间层次

```
AFramework.AMapper                      # 根命名空间
AFramework.AMapper.Configuration        # 配置相关
AFramework.AMapper.Execution            # 执行相关
AFramework.AMapper.Mappers              # 内置映射器
AFramework.AMapper.Resolvers            # 值解析器
AFramework.AMapper.Unity                # Unity 特定
AFramework.AMapper.Extensions           # 扩展方法
AFramework.AMapper.Internal             # 内部实现（不对外暴露）
AFramework.AMapper.DI                   # 依赖注入集成
```

### 4.2 命名空间使用规则

| 规则 | 说明 |
|------|------|
| 最多三层 | 命名空间层级不超过三层 |
| 按功能划分 | 不按文件目录命名 |
| Internal 隔离 | 内部实现使用 Internal 命名空间 |
| 公开 API 集中 | 常用类型放在根命名空间 |

---

## 5. 文件命名规范

### 5.1 类文件命名

| 类型 | 命名规则 | 示例 |
|------|---------|------|
| 接口 | I + 名称 | IAMapper、ITypeMap |
| 抽象类 | 名称 + Base | ValueResolverBase |
| 特性 | 名称 + Attribute | AutoMapAttribute |
| 异常 | 名称 + Exception | MappingException |
| 扩展方法 | 目标类型 + Extensions | TypeExtensions |
| 映射器 | 类型名 + Mapper | Vector3Mapper |
| 转换器 | 类型名 + Converter | ColorConverter |

### 5.2 目录命名

| 规则 | 说明 |
|------|------|
| PascalCase | 使用 PascalCase 命名 |
| 单数形式 | 优先使用单数（Exception 除外） |
| 功能描述 | 名称反映目录内容职责 |

---

## 6. 依赖关系

### 6.1 层级依赖

```
┌─────────────────────────────────────────┐
│              DI（集成层）                │
├─────────────────────────────────────────┤
│           Extensions（扩展层）           │
├─────────────────────────────────────────┤
│            Unity（Unity 层）             │
├─────────────────────────────────────────┤
│    Mappers（映射器层）│ Resolvers（解析器层）│
├─────────────────────────────────────────┤
│           Execution（执行层）            │
├─────────────────────────────────────────┤
│         Configuration（配置层）          │
├─────────────────────────────────────────┤
│             Core（核心层）               │
├─────────────────────────────────────────┤
│           Internal（内部层）             │
└─────────────────────────────────────────┘
```

### 6.2 依赖规则

| 规则 | 说明 |
|------|------|
| 上层依赖下层 | 上层模块可依赖下层模块 |
| 禁止反向依赖 | 下层模块不可依赖上层模块 |
| Core 无依赖 | Core 层不依赖其他业务层 |
| Internal 被依赖 | Internal 可被所有层依赖 |

---

## 7. 程序集定义

### 7.1 运行时程序集

```
AFramework.AMapper.asmdef
├── 引用: AFramework.DI
├── 平台: Any
└── 定义符号: AFRAMEWORK_AMAPPER
```

### 7.2 编辑器程序集

```
AFramework.AMapper.Editor.asmdef
├── 引用: AFramework.AMapper
├── 引用: AFramework.DI.Editor
├── 平台: Editor
└── 定义符号: AFRAMEWORK_AMAPPER_EDITOR
```

---

## 8. 版本兼容性

### 8.1 预处理指令

| 指令 | 用途 |
|------|------|
| UNITY_2022_3_OR_NEWER | Unity 2022.3+ 特定代码 |
| UNITY_6000_0_OR_NEWER | Unity 6.x 特定代码 |
| ENABLE_IL2CPP | IL2CPP 编译特定代码 |

### 8.2 条件编译区域

| 区域 | 说明 |
|------|------|
| Unity 版本适配 | 不同 Unity 版本的 API 差异处理 |
| 平台适配 | 不同平台的特殊处理 |
| 编辑器/运行时 | 编辑器专用代码隔离 |

---

*文档版本：1.0.0*
*最后更新：2026年1月*
