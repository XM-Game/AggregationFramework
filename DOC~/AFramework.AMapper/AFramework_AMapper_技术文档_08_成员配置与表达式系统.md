# AFramework.AMapper 技术文档

## 第八部分：成员配置与表达式系统

---

## 1. 成员配置表达式概述

### 1.1 MemberConfigurationExpression

MemberConfigurationExpression 是配置单个成员映射的核心类，提供流畅的 API 来定义属性映射规则。

**泛型版本**：
- MemberConfigurationExpression\<TSource, TDestination, TMember\>
- 提供强类型的配置方法
- 编译时类型检查

**非泛型版本**：
- MemberConfigurationExpression
- 用于运行时动态配置
- 支持 Type 参数

### 1.2 IMemberMapConfiguration 接口

定义成员映射配置的契约。

**接口成员**：

| 成员 | 说明 |
|------|------|
| Configure(TypeMap) | 应用配置到 TypeMap |
| DestinationMember | 目标成员信息 |
| SourceExpression | 源表达式 |
| GetDestinationExpression() | 获取目标表达式 |
| Reverse() | 创建反向配置 |
| Ignored | 是否忽略 |

---

## 2. 值来源配置

### 2.1 MapFrom 方法族

指定属性值的来源。

**表达式形式**：
- MapFrom\<TSourceMember\>(Expression\<Func\<TSource, TSourceMember\>\>)
- 支持 LINQ 投影
- 编译时优化

**委托形式**：
- MapFrom\<TResult\>(Func\<TSource, TDestination, TResult\>)
- MapFrom\<TResult\>(Func\<TSource, TDestination, TMember, TResult\>)
- MapFrom\<TResult\>(Func\<TSource, TDestination, TMember, MappingContext, TResult\>)
- 运行时灵活性
- 可访问目标对象和上下文

**字符串路径形式**：
- MapFrom(string sourceMembersPath)
- 支持点号分隔的路径
- 运行时解析

### 2.2 值解析器配置

使用自定义值解析器。

**类型指定**：
- MapFrom\<TValueResolver\>()
- 通过依赖注入获取实例

**实例指定**：
- MapFrom(IValueResolver\<TSource, TDestination, TMember\> valueResolver)
- 直接提供实例

**带源成员**：
- MapFrom\<TValueResolver, TSourceMember\>(Expression\<Func\<TSource, TSourceMember\>\>)
- MapFrom\<TValueResolver, TSourceMember\>(string sourceMemberName)
- 指定解析器的输入源

### 2.3 值转换器配置

使用值转换器进行类型转换。

**ConvertUsing 方法**：
- ConvertUsing\<TValueConverter, TSourceMember\>()
- ConvertUsing\<TSourceMember\>(IValueConverter\<TSourceMember, TMember\>)
- 专注于值转换逻辑

**与 MapFrom 的区别**：
- ValueConverter 只关注值转换
- ValueResolver 可访问完整上下文
- ValueConverter 更简单、更专注

---

## 3. 条件映射

### 3.1 Condition 方法

定义映射执行的条件。

**重载形式**：
- Condition(Func\<TSource, bool\>)
- Condition(Func\<TSource, TDestination, bool\>)
- Condition(Func\<TSource, TDestination, TMember, bool\>)
- Condition(Func\<TSource, TDestination, TMember, TMember, bool\>)
- Condition(Func\<TSource, TDestination, TMember, TMember, MappingContext, bool\>)

**执行时机**：
- 在值解析之后执行
- 条件为 false 时跳过赋值
- 目标保持原值

### 3.2 PreCondition 方法

定义映射的前置条件。

**重载形式**：
- PreCondition(Func\<TSource, bool\>)
- PreCondition(Func\<MappingContext, bool\>)
- PreCondition(Func\<TSource, MappingContext, bool\>)
- PreCondition(Func\<TSource, TDestination, MappingContext, bool\>)

**执行时机**：
- 在值解析之前执行
- 条件为 false 时跳过整个映射
- 性能优化：避免不必要的值解析

**与 Condition 的区别**：

| 特性 | PreCondition | Condition |
|------|--------------|-----------|
| 执行时机 | 解析前 | 解析后 |
| 性能 | 更高 | 较低 |
| 可访问数据 | 源对象、上下文 | 源值、目标值 |

---

## 4. 空值处理

### 4.1 NullSubstitute

指定源值为 null 时的替代值。

**使用方式**：
- NullSubstitute(object nullSubstitute)
- 提供默认值

**适用场景**：
- 避免 null 传播
- 提供业务默认值

### 4.2 AllowNull / DoNotAllowNull

控制是否允许 null 值。

**AllowNull**：
- 允许 null 值映射到目标
- 默认行为

**DoNotAllowNull**：
- 不允许 null 值
- 尝试创建默认实例

### 4.3 UseDestinationValue / DoNotUseDestinationValue

控制是否使用目标现有值。

**UseDestinationValue**：
- 使用目标属性的现有值
- 适合集合追加场景

**DoNotUseDestinationValue**：
- 不使用目标现有值
- 总是创建新值

---

## 5. 映射控制

### 5.1 Ignore 方法

忽略目标成员的映射。

**使用方式**：
- Ignore()
- Ignore(bool ignorePaths)

**ignorePaths 参数**：
- true：同时忽略相关的 PathMap
- false：只忽略当前成员

### 5.2 SetMappingOrder

设置映射执行顺序。

**使用方式**：
- SetMappingOrder(int mappingOrder)

**用途**：
- 控制属性映射顺序
- 处理属性间依赖

### 5.3 MapAtRuntime

禁用内联优化。

**使用方式**：
- MapAtRuntime()

**效果**：
- 成员映射不内联到执行计划
- 运行时动态解析
- 减少执行计划大小

### 5.4 ExplicitExpansion

控制 ProjectTo 的展开行为。

**使用方式**：
- ExplicitExpansion(bool value)

**效果**：
- true：需要显式指定才展开
- false：默认展开

---

## 6. 值转换器

### 6.1 AddTransform

添加值转换器。

**使用方式**：
- AddTransform(Expression\<Func\<TMember, TMember\>\>)

**用途**：
- 对映射结果进行后处理
- 字符串修剪、格式化等

**执行顺序**：
- 在映射完成后执行
- 可叠加多个转换器

---

## 7. 反向映射支持

### 7.1 Reverse 方法

创建反向映射配置。

**实现逻辑**：
- 交换源和目标
- 处理 MapFrom 表达式
- 处理成员路径

**限制**：
- 复杂表达式可能无法反转
- 需要手动验证反向映射

---

## 8. 配置应用流程

### 8.1 Configure 方法

将配置应用到 TypeMap。

**流程**：
1. 解析目标成员（处理泛型）
2. 查找或创建 MemberMap
3. 应用所有 MemberMapActions

### 8.2 MemberMapActions

存储配置动作的列表。

**特点**：
- 延迟执行
- 按顺序应用
- 支持组合配置

---

## 9. 路径配置表达式

### 9.1 PathConfigurationExpression

配置嵌套路径映射（ForPath）。

**用途**：
- 映射到嵌套属性
- 反扁平化

**特点**：
- 自动创建中间对象
- 支持深层路径

### 9.2 与 ForMember 的区别

**ForMember**：
- 配置直接成员
- 不创建中间对象

**ForPath**：
- 配置嵌套路径
- 自动处理中间对象

---

## 10. 构造函数参数配置

### 10.1 CtorParamConfigurationExpression

配置构造函数参数映射。

**配置方法**：
- MapFrom：指定参数值来源
- 与属性配置类似的 API

**特点**：
- 参数名称匹配
- 支持自定义解析

---

## 11. Unity 特定配置

### 11.1 Unity 类型成员配置

支持 Unity 类型的特殊配置。

**Vector 配置**：
- MapFromX/Y/Z：单独映射分量
- MapFromMagnitude：映射长度
- MapFromNormalized：映射归一化向量

**Color 配置**：
- MapFromR/G/B/A：单独映射通道
- MapFromHex：从十六进制映射
- MapFromHSV：从 HSV 映射

### 11.2 组件引用配置

支持 Unity 组件引用的配置。

**配置方式**：
- MapFromComponent\<T\>()：从组件获取
- MapFromChild\<T\>(string path)：从子对象获取

### 11.3 资源引用配置

支持 Unity 资源引用的配置。

**配置方式**：
- MapFromResource\<T\>(string path)：从 Resources 加载
- MapFromAddressable\<T\>(string key)：从 Addressables 加载

---

## 12. 表达式系统内部

### 12.1 表达式构建流程

**构建步骤**：
1. 解析源表达式
2. 生成空值检查
3. 应用类型转换
4. 生成赋值表达式
5. 包装条件逻辑

### 12.2 表达式优化

**优化策略**：
- 常量折叠
- 空值检查合并
- 内联简单表达式

### 12.3 表达式缓存

**缓存机制**：
- 按 TypePair 缓存
- 按成员路径缓存
- 延迟编译

---

## 13. 最佳实践

### 13.1 配置组织

**推荐**：
- 使用 Profile 组织相关映射
- 保持配置简洁
- 利用约定减少配置

### 13.2 性能考虑

**建议**：
- 优先使用表达式形式的 MapFrom
- 避免复杂的条件逻辑
- 使用 PreCondition 优化性能

### 13.3 可维护性

**原则**：
- 保持映射简单
- 复杂逻辑移至服务层
- 充分测试配置

---

## 附录：快速参考

### A. 常用配置方法

| 方法 | 说明 |
|------|------|
| CreateMap\<S, D\>() | 创建映射 |
| ForMember() | 配置成员 |
| MapFrom() | 指定源 |
| Ignore() | 忽略成员 |
| Condition() | 条件映射 |
| PreCondition() | 前置条件 |
| ConvertUsing() | 类型转换 |
| ReverseMap() | 反向映射 |
| Include() | 包含派生类 |
| IncludeBase() | 包含基类 |

### B. 常用接口

| 接口 | 说明 |
|------|------|
| IAMapper | 映射器 |
| IMapperConfiguration | 配置提供者 |
| IValueResolver | 值解析器 |
| ITypeConverter | 类型转换器 |
| IValueConverter | 值转换器 |
| IMappingAction | 映射动作 |
| MappingProfile | 配置文件基类 |

### C. 常用特性

| 特性 | 说明 |
|------|------|
| AutoMapAttribute | 类型映射 |
| IgnoreAttribute | 忽略成员 |
| MapFromAttribute | 源成员 |
| MapToAttribute | 目标成员 |
| NullSubstituteAttribute | 空值替换 |

### D. Unity 类型支持

| 类型 | 支持状态 |
|------|---------|
| Vector2/3/4 | ✅ 完全支持 |
| Quaternion | ✅ 完全支持 |
| Color/Color32 | ✅ 完全支持 |
| Rect/Bounds | ✅ 完全支持 |
| Matrix4x4 | ✅ 完全支持 |
| Transform | ✅ 数据映射 |
| GameObject | ⚠️ 引用映射 |
| ScriptableObject | ✅ 完全支持 |

---

*文档完结*
