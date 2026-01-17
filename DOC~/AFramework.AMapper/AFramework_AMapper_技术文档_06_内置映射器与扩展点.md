# AFramework.AMapper 技术文档

## 第六部分：内置映射器与扩展点

---

## 1. 内置映射器概述

### 1.1 映射器链

AMapper 使用责任链模式处理类型映射。MapperRegistry 定义了内置映射器的执行顺序。

**执行顺序**：
1. 检查已配置的 TypeMap
2. 按优先级遍历内置映射器链
3. 找到第一个匹配的映射器执行

**映射器注册顺序**（决定优先级）：

| 优先级 | 映射器 | 说明 |
|--------|--------|------|
| 1 | UnityTypeMapper | Unity 类型映射（最高优先级） |
| 2 | CollectionMapper | 集合映射 |
| 3 | ArrayMapper | 数组映射 |
| 4 | DictionaryMapper | 字典映射 |
| 5 | AssignableMapper | 可赋值类型 |
| 6 | NullableSourceMapper | 可空源类型 |
| 7 | NullableDestinationMapper | 可空目标类型 |
| 8 | StringMapper | 字符串转换 |
| 9 | EnumMapper | 枚举映射 |
| 10 | ConvertMapper | System.Convert 转换 |
| 11 | ParseStringMapper | Parse 方法转换 |
| 12 | ConstructorMapper | 构造函数映射 |
| 13 | ConversionOperatorMapper | 转换运算符 |

### 1.2 IObjectMapper 接口

所有映射器实现 IObjectMapper 接口。

**接口方法**：

| 方法 | 说明 |
|------|------|
| IsMatch | 判断是否匹配类型对 |
| MapExpression | 生成映射表达式 |
| GetAssociatedTypes | 获取关联类型（用于递归映射） |

**设计特点**：
- 表达式驱动：返回 Expression 而非直接执行
- 类型安全：编译时类型检查
- 可组合：支持嵌套映射

---

## 2. 内置映射器详解

### 2.1 UnityTypeMapper

处理 Unity 特有类型的映射，是 AMapper 的特色功能。

**支持类型**：

| 类型 | 映射方式 |
|------|---------|
| Vector2/3/4 | 组件级映射 |
| Vector2Int/3Int | 组件级映射 |
| Quaternion | 组件级映射 |
| Color/Color32 | 组件级映射 |
| Rect/RectInt | 组件级映射 |
| Bounds/BoundsInt | 组件级映射 |
| Matrix4x4 | 组件级映射 |
| Ray/Ray2D | 组件级映射 |
| Plane | 组件级映射 |

**特殊处理**：
- Vector3 ↔ Vector2：自动截断/扩展
- Color ↔ Color32：自动转换
- Quaternion ↔ Vector3：欧拉角转换

### 2.2 CollectionMapper

处理集合类型映射，是最复杂的内置映射器。

**支持类型**：
- List\<T\>、IList\<T\>、IList
- IEnumerable\<T\>、ICollection\<T\>
- ReadOnlyCollection\<T\>
- HashSet\<T\>、ISet\<T\>
- Queue\<T\>、Stack\<T\>
- LinkedList\<T\>

**核心处理逻辑**：
- 元素映射：递归调用配置的元素类型映射
- 空集合检查：自动处理 null 和空集合
- 深度检查：支持 MaxDepth 限制

**特殊处理**：
- MustUseDestination：使用现有目标集合
- 只读集合：先创建可变集合再包装

### 2.3 ArrayMapper

处理数组类型映射。

**支持类型**：
- 一维数组 T[]
- 多维数组 T[,]、T[,,]
- 交错数组 T[][]

**核心处理逻辑**：
- 一维数组：直接映射
- 多维数组：使用 MultidimensionalArrayFiller 填充
- 交错数组：递归映射

### 2.4 DictionaryMapper

处理字典类型映射。

**支持类型**：
- Dictionary\<K,V\>
- IDictionary\<K,V\>
- ReadOnlyDictionary\<K,V\>
- IReadOnlyDictionary\<K,V\>
- ConcurrentDictionary\<K,V\>

**映射规则**：
- 键和值分别映射
- 支持键类型转换
- 支持值类型转换

### 2.5 AssignableMapper

处理可直接赋值的类型。

**匹配条件**：
- 目标类型可从源类型赋值（IsAssignableFrom）
- 非集合类型（排除 IEnumerable）

**行为**：
- 直接赋值引用
- 不创建副本
- 最常见的映射场景

### 2.6 NullableSourceMapper

处理可空源类型到非可空目标类型。

**匹配条件**：
- 源类型为 Nullable\<T\>
- 目标类型为 T 或可从 T 赋值

**行为**：
- 提取 Value 属性
- 自动处理 null 情况
- 支持链式可空类型

### 2.7 NullableDestinationMapper

处理非可空源类型到可空目标类型。

**匹配条件**：
- 目标类型为 Nullable\<T\>
- 源类型为 T 或可转换为 T

**行为**：
- 自动包装为可空类型
- 保留 null 语义

### 2.8 StringMapper

处理任意类型到字符串的转换。

**匹配条件**：
- 目标类型为 string
- 源类型非 string

**行为**：
- 调用 ToString() 方法
- 枚举类型特殊处理（避免装箱）
- 支持自定义格式化

### 2.9 EnumMapper

处理枚举类型之间的转换。

**映射规则**：
- 优先按名称匹配
- 其次按数值匹配
- 支持不同底层类型

**支持转换**：
- 枚举 ↔ 枚举
- 枚举 ↔ 字符串
- 枚举 ↔ 数值

### 2.10 ConvertMapper

使用 System.Convert 类进行类型转换。

**支持类型**：
- 所有基元类型互转
- 数值类型转换（int、long、double 等）
- 布尔类型转换
- DateTime 转换

**实现机制**：
- 使用 Convert.ChangeType
- 编译为直接转换表达式

### 2.11 ParseStringMapper

使用静态 Parse 方法转换字符串。

**支持类型**：
- Guid
- TimeSpan
- DateTimeOffset
- 任何具有 Parse(string) 静态方法的类型

**特点**：
- 无装箱开销
- 编译时确定方法

### 2.12 ConstructorMapper

使用接受源类型参数的构造函数创建目标。

**匹配条件**：
- 目标类型有接受源类型的公共构造函数
- 形如 new Destination(source)

**使用场景**：
- 包装类型
- 值对象转换

### 2.13 ConversionOperatorMapper

使用 C# 转换运算符进行映射。

**支持运算符**：
- implicit operator（隐式转换）
- explicit operator（显式转换）

**匹配顺序**：
- 先检查目标类型的隐式转换
- 再检查源类型的隐式转换
- 然后检查显式转换

---

## 3. 值解析器系统

### 3.1 IValueResolver 接口

值解析器是 AMapper 的核心扩展点，用于自定义属性值的获取逻辑。

**接口定义**：

| 成员 | 说明 |
|------|------|
| GetExpression | 生成值解析表达式 |
| GetSourceMember | 获取源成员信息 |
| ResolvedType | 解析后的类型 |
| SourceMemberName | 源成员名称（可选） |

### 3.2 内置值解析器

**MemberPathResolver**：
- 处理成员路径解析（如 Customer.Address.City）
- 支持多级属性访问
- 自动生成空值检查

**FuncResolver**：
- 处理委托形式的 MapFrom
- 支持访问源、目标、目标成员和上下文
- 运行时执行

**ExpressionResolver**：
- 处理表达式形式的 MapFrom
- 支持 LINQ 投影
- 编译时优化

**ClassValueResolver**：
- 处理自定义 IValueResolver 类
- 支持依赖注入
- 支持泛型解析器

### 3.3 解析器选择逻辑

ExecutionPlanBuilder 按以下优先级选择解析器：
1. 显式配置的 Resolver（MapFrom 指定）
2. 源成员路径（自动匹配或 MapFrom(string)）
3. 目标成员默认值

---

## 4. 自定义映射器

### 4.1 实现 IObjectMapper

创建自定义映射器需要实现 IObjectMapper 接口。

**实现要点**：
- IsMatch：定义类型匹配逻辑，返回 bool
- MapExpression：生成映射表达式树
- GetAssociatedTypes：返回需要递归映射的类型对（可选）

**设计原则**：
- 使用表达式树而非反射
- 考虑空值处理
- 支持嵌套映射

### 4.2 注册自定义映射器

**配置方式**：
- cfg.Mappers.Insert(0, new CustomMapper()) - 最高优先级
- cfg.Mappers.Add(new CustomMapper()) - 最低优先级

**优先级说明**：
- 插入位置决定匹配优先级
- 先匹配的映射器先执行
- 建议根据特殊性决定位置

---

## 5. Unity 类型扩展

### 5.1 Vector 映射器

处理 Unity Vector 类型的映射。

**支持转换**：

| 源类型 | 目标类型 | 转换方式 |
|--------|---------|---------|
| Vector3 | Vector2 | 截断 z 分量 |
| Vector2 | Vector3 | z = 0 |
| Vector3 | float[] | 组件数组 |
| float[] | Vector3 | 数组到向量 |
| Vector3 | Tuple | 元组转换 |

### 5.2 Quaternion 映射器

处理 Unity Quaternion 类型的映射。

**支持转换**：

| 源类型 | 目标类型 | 转换方式 |
|--------|---------|---------|
| Quaternion | Vector3 | 欧拉角 |
| Vector3 | Quaternion | 欧拉角到四元数 |
| Quaternion | float[] | 组件数组 |
| Matrix4x4 | Quaternion | 矩阵提取 |

### 5.3 Color 映射器

处理 Unity Color 类型的映射。

**支持转换**：

| 源类型 | 目标类型 | 转换方式 |
|--------|---------|---------|
| Color | Color32 | 自动转换 |
| Color32 | Color | 自动转换 |
| Color | Vector4 | 组件映射 |
| string | Color | 十六进制解析 |
| Color | string | 十六进制输出 |

### 5.4 Transform 映射器

处理 Unity Transform 数据的映射。

**支持转换**：
- Transform → TransformData（位置、旋转、缩放）
- TransformData → Transform
- Transform → Matrix4x4

---

## 6. 配置验证系统

### 6.1 ConfigurationValidator

ConfigurationValidator 负责验证映射配置的完整性和正确性。

**验证时机**：
- AssertConfigurationIsValid() 调用时
- 可选的启动时验证

**验证内容**：
- 重复的 TypeMap 配置
- 未映射的目标成员
- 构造函数参数匹配
- 类型转换可行性

### 6.2 重复配置检测

检测同一类型对在多个 Profile 中的重复配置。

**检测逻辑**：
- 按 TypePair 分组所有 TypeMapConfig
- 检查同一 TypePair 是否出现多次
- 抛出 DuplicateTypeMapException

### 6.3 成员映射验证

验证目标类型的所有成员都有映射源。

**验证规则**：
- 获取未映射的属性名称
- 检查构造函数参数匹配
- 生成配置错误信息

**跳过验证的情况**：
- 成员被显式 Ignore
- 使用 MemberList.None

### 6.4 自定义验证器

支持扩展验证逻辑。

**ValidationContext**：

| 属性 | 说明 |
|------|------|
| Types | 当前验证的类型对 |
| MemberMap | 当前成员映射 |
| Exceptions | 异常收集列表 |
| TypeMap | 类型映射（可选） |
| ObjectMapper | 对象映射器（可选） |

**注册方式**：
- cfg.Validators.Add(context => { ... })

---

## 7. 执行计划构建

### 7.1 ExecutionPlanBuilder

ExecutionPlanBuilder 是执行计划的核心构建器。

**核心职责**：
- 构建映射 Lambda 表达式
- 处理循环引用检测
- 生成属性映射表达式
- 处理构造函数映射

### 7.2 执行计划结构

**CreateMapperLambda 流程**：
1. 检查自定义 TypeConverter
2. 检测循环引用
3. 创建目标对象
4. 创建赋值逻辑
5. 包装映射函数
6. 处理 IncludedMembers
7. 生成最终 Lambda

### 7.3 循环引用处理

**检测机制**：
- 使用 HashSet\<TypeMap\> 跟踪路径
- 检测到循环时设置 PreserveReferences
- 值类型循环使用 MaxDepth 限制
- 自动禁用内联

**PreserveReferences 行为**：
- 缓存已映射的目标对象
- 检查缓存避免重复映射
- 使用 MappingContext 存储缓存

### 7.4 内联优化

**Inline 机制**：
- 默认内联嵌套映射
- 减少方法调用开销
- 循环引用时禁用内联
- MaxExecutionPlanDepth 控制深度

---

## 8. 对象工厂

### 8.1 ObjectFactory

创建目标对象实例的工厂类。

**创建策略优先级**：
1. CustomCtorFunction：用户自定义构造表达式
2. ConstructorMap：参数化构造函数映射
3. 默认构造函数：无参构造
4. 服务定位器：依赖注入容器

**接口类型处理**：

| 接口类型 | 创建类型 |
|---------|---------|
| IDictionary\<K,V\> | Dictionary\<K,V\> |
| IList\<T\> | List\<T\> |
| ICollection\<T\> | List\<T\> |
| IEnumerable\<T\> | List\<T\> |
| ISet\<T\> | HashSet\<T\> |

### 8.2 自定义构造

**ConstructUsing 配置**：
- 表达式形式：编译时优化
- 委托形式：运行时灵活性

**使用场景**：
- 需要特殊初始化逻辑
- 使用工厂方法创建
- 依赖注入集成

---

## 9. 内部工具类

### 9.1 TypeExtensions

类型扩展方法集合。

**主要功能**：

| 方法 | 说明 |
|------|------|
| IsCollection | 判断是否为集合类型 |
| GetElementType | 获取集合元素类型 |
| IsNullableType | 判断是否为可空类型 |
| GetGenericInterface | 获取泛型接口实现 |
| IsUnityType | 判断是否为 Unity 类型 |

### 9.2 ReflectionHelper

反射辅助工具类。

**主要功能**：

| 方法 | 说明 |
|------|------|
| GetMemberPath | 解析成员路径字符串 |
| GetMemberType | 获取成员类型 |
| GetStaticMethod | 获取静态方法 |
| GetInstanceMethod | 获取实例方法 |

### 9.3 TypeDetails

类型详细信息缓存，包含反射信息的高效访问。

**缓存内容**：

| 属性 | 说明 |
|------|------|
| ReadAccessors | 可读成员列表 |
| WriteAccessors | 可写成员列表 |
| Constructors | 构造函数列表 |
| NameToMember | 名称到成员的字典 |

### 9.4 TypePair

类型对结构体，用于映射配置的键。

**用途**：
- 映射配置的键
- 缓存索引
- 类型匹配

**特性**：
- 值类型（struct）
- 实现 IEquatable
- 高效哈希计算

### 9.5 MemberPath

成员路径表示，用于嵌套成员访问。

**用途**：
- 表示嵌套成员访问路径
- 支持 ForPath 配置
- 投影展开控制

### 9.6 ExpressionBuilder

表达式构建工具类，提供大量静态方法和常量。

**核心方法**：

| 方法 | 说明 |
|------|------|
| MapExpression | 生成映射表达式 |
| NullCheckSource | 生成源对象空值检查 |
| ForEach | 生成集合遍历表达式 |
| NullCheck | 生成嵌套成员空值检查链 |
| ApplyTransformers | 应用值转换器 |

---

## 10. 并发与线程安全

### 10.1 ConcurrentCache

线程安全的延迟初始化缓存。

**特性**：
- GetOrAdd 原子操作
- 值工厂只执行一次
- 读取无锁
- 写入时锁定

### 10.2 配置线程安全

MapperConfiguration 是完全线程安全的。

**保证机制**：
- 配置创建后不可变
- 所有缓存使用并发集合
- 无共享可变状态

### 10.3 Mapper 线程安全

AMapper 实例是线程安全的。

**注意事项**：
- 运行时选项不共享
- 每次 Map 调用创建独立的 MappingContext
- 自定义解析器需自行保证线程安全

---

## 11. 对象池集成

### 11.1 ObjectPool 支持

AMapper 集成对象池以减少 GC 分配。

**池化对象**：
- MappingContext
- 临时集合
- 表达式构建器

### 11.2 配置对象池

**配置方式**：
- cfg.UseObjectPool = true
- cfg.ObjectPoolSize = 100

### 11.3 自定义对象池

支持自定义对象池实现。

**接口**：
- IObjectPool\<T\>
- Get() / Return(T)

---

## 12. 扩展最佳实践

### 12.1 何时扩展

**适合扩展的场景**：
- 特殊类型转换需求
- 自定义序列化格式
- 领域特定映射逻辑
- Unity 特定类型支持

### 12.2 扩展原则

**设计原则**：
- 保持简单，单一职责
- 遵循现有模式和约定
- 充分测试边界条件
- 考虑线程安全

### 12.3 性能考虑

**优化建议**：
- 使用表达式树而非反射
- 缓存重复计算结果
- 避免装箱拆箱
- 减少闭包捕获
- 使用对象池

---

*下一部分：调试、测试与最佳实践*
