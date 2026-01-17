# AFramework.AMapper 技术文档

## 第三部分：高级映射功能

---

## 1. 自定义类型转换器

### 1.1 ITypeConverter 接口

类型转换器用于完全控制一种类型到另一种类型的转换。

**接口定义**：

| 成员 | 说明 |
|------|------|
| TSource | 源类型 |
| TDestination | 目标类型 |
| Convert | 执行转换逻辑的方法 |

**特性**：
- 全局作用域
- 一次配置，处处生效
- 适合类型间差异大的转换

### 1.2 ConvertUsing 配置

**配置方式**：
- 函数委托形式
- 类型转换器实例
- 类型转换器类型（支持 DI）

**使用场景**：
- string → int 转换
- string → DateTime 转换
- string → Enum 转换
- 任何需要完全自定义的类型转换

### 1.3 类型转换器 vs 值解析器

| 特性 | 类型转换器 | 值解析器 |
|------|-----------|---------|
| 作用域 | 全局 | 成员级 |
| 配置位置 | CreateMap | ForMember |
| 适用场景 | 类型级转换 | 属性级转换 |
| 复用性 | 自动复用 | 需显式配置 |

---

## 2. 自定义值解析器

### 2.1 IValueResolver 接口

值解析器用于为特定目标成员提供自定义值。

**接口定义**：

| 成员 | 说明 |
|------|------|
| TSource | 源类型 |
| TDestination | 目标类型 |
| TDestMember | 目标成员类型 |
| Resolve | 解析值的方法 |

**MappingContext**：
- 包含映射上下文信息
- 可访问源/目标类型
- 可访问映射选项
- 可访问 DI 容器

### 2.2 IMemberValueResolver 接口

成员值解析器接收源成员值作为输入。

**接口定义**：

| 成员 | 说明 |
|------|------|
| TSource | 源类型 |
| TDestination | 目标类型 |
| TSourceMember | 源成员类型 |
| TDestMember | 目标成员类型 |

**使用场景**：
- 需要访问特定源成员
- 跨多个映射复用解析逻辑

### 2.3 值解析器配置

**配置方式**：
- MapFrom\<TValueResolver\>()
- MapFrom(typeof(CustomResolver))
- MapFrom(resolverInstance)
- MapFrom((src, dest, destMember, context) => ...)

**自定义源成员**：
- MapFrom\<TResolver, TSourceMember\>(src => src.Member)
- 提高解析器复用性

### 2.4 解析器与条件

**执行顺序**：
1. 尝试解析目标值
2. 评估条件
3. 条件通过则赋值

**注意事项**：
- 解析在条件评估前执行
- 解析器不应抛出异常
- 使用 PreCondition 避免不必要的解析

---

## 3. 值转换器

### 3.1 IValueConverter 接口

值转换器是类型转换器和值解析器的中间形态。

**接口定义**：

| 成员 | 说明 |
|------|------|
| TSourceMember | 源成员类型 |
| TDestMember | 目标成员类型 |
| Convert | 转换值的方法 |

**特性**：
- 成员级作用域
- 只关注值转换
- 不访问源/目标对象

### 3.2 ConvertUsing 配置

**配置方式**：
- ConvertUsing(converterInstance)
- ConvertUsing\<TConverter, TSourceMember\>()
- ConvertUsing(converter, src => src.Member)

**使用场景**：
- 格式化转换（货币、日期）
- 简单类型转换
- 跨多个映射复用

---

## 4. 值转换器（Value Transformers）

### 4.1 概念

值转换器在赋值前对值进行额外转换。

**作用层级**：
- 全局级
- Profile 级
- Map 级
- Member 级

### 4.2 配置方式

**全局配置**：
- cfg.ValueTransformers.Add\<string\>(val => val.Trim())

**Map 级配置**：
- CreateMap\<S, D\>().AddTransform\<string\>(val => val.Trim())

**使用场景**：
- 字符串修剪
- 默认值处理
- 格式标准化

---

## 5. 构造函数映射

### 5.1 自动构造函数映射

AMapper 自动匹配构造函数参数与源成员。

**匹配规则**：
- 按参数名称匹配
- 支持 PascalCase 到 camelCase
- 支持嵌套属性

### 5.2 ForCtorParam 配置

用于自定义构造函数参数映射。

**使用场景**：
- 参数名称不匹配
- 需要自定义值来源
- 需要忽略某个参数

### 5.3 禁用构造函数映射

**全局禁用**：
- cfg.DisableConstructorMapping()

**Profile 级禁用**：
- DisableConstructorMapping()

### 5.4 构造函数过滤

ShouldUseConstructor 控制可用的构造函数。

**使用场景**：
- 只使用公共构造函数
- 排除特定构造函数
- Record 类型映射

---

## 6. 前置/后置映射动作

### 6.1 BeforeMap / AfterMap

在映射前后执行自定义逻辑。

**配置方式**：
- BeforeMap((src, dest) => ...)
- AfterMap((src, dest) => ...)

**使用场景**：
- 初始化目标对象
- 清理/验证数据
- 日志记录
- Unity 组件初始化

### 6.2 IMappingAction 接口

封装映射动作为可复用类。

**接口定义**：

| 成员 | 说明 |
|------|------|
| TSource | 源类型 |
| TDestination | 目标类型 |
| Process | 执行动作的方法 |

**特性**：
- 支持依赖注入
- 可复用
- 可测试

### 6.3 运行时映射选项

Map 调用时指定前置/后置动作。

**使用场景**：
- 需要上下文信息
- 动态配置
- 一次性逻辑

---

## 7. 空值处理

### 7.1 NullSubstitute

空值替换在源值为 null 时提供替代值。

**配置方式**：
- NullSubstitute("默认值")
- NullSubstitute(defaultObject)

**特性**：
- 替代值类型为源成员类型
- 替代值会经过后续映射/转换

### 7.2 AllowNullDestinationValues

控制是否允许目标属性为 null。

**默认行为**：
- 允许 null 目标值

**配置级别**：
- 全局
- Profile
- Member（AllowNull / DoNotAllowNull）

### 7.3 AllowNullCollections

控制集合属性的 null 处理。

**默认行为**：
- null 源集合 → 空集合

**自定义行为**：
- AllowNullCollections = true

---

## 8. 循环引用处理

### 8.1 PreserveReferences

处理对象图中的循环引用。

**配置方式**：
- PreserveReferences(true)

**工作原理**：
- 跟踪已映射对象
- 遇到相同对象时复用
- 避免无限递归

### 8.2 MaxDepth

限制映射深度。

**配置方式**：
- MaxDepth(n)

**使用场景**：
- 防止深度递归
- 性能优化
- 避免栈溢出

### 8.3 性能考虑

- PreserveReferences 有性能开销
- MaxDepth 可能导致数据不完整
- 优先考虑重新设计对象模型

---

## 9. 动态对象映射

### 9.1 Dictionary 映射

支持 Dictionary\<string, object\> 到对象的映射。

**特性**：
- 键名匹配属性名
- 支持点号表示嵌套（"Inner.Property"）
- 自动类型转换

**使用场景**：
- JSON 反序列化结果
- 配置数据映射
- 动态数据结构

### 9.2 匿名类型映射

支持匿名类型的映射。

**特性**：
- 按属性名称匹配
- 运行时动态解析

---

## 10. 枚举映射

### 10.1 内置枚举映射

AMapper 内置枚举映射支持。

**映射规则**：
- 优先按名称匹配
- 其次按数值匹配
- 支持字符串到枚举

### 10.2 枚举映射配置

**配置方式**：
- MapByValue：按数值映射
- MapByName：按名称映射
- MapValue：自定义映射

### 10.3 枚举映射验证

EnableEnumMappingValidation 启用枚举映射验证。

**验证内容**：
- 所有源枚举值都有目标
- 映射配置完整性

---

## 11. 运行时映射选项

### 11.1 IMappingOperationOptions

Map 调用时传递运行时选项。

**可用选项**：

| 选项 | 说明 |
|------|------|
| BeforeMap | 前置映射动作 |
| AfterMap | 后置映射动作 |
| Items | 键值对数据字典 |
| State | 状态对象 |
| ServiceProvider | 服务提供者 |

### 11.2 Items 字典

传递键值对数据到映射过程。

**使用方式**：
- Map 时：opt.Items["Key"] = value
- 解析器中：context.Items["Key"]

### 11.3 State 对象

传递单个状态对象。

**特性**：
- 与 Items 互斥
- 类型安全
- 性能更优

---

## 12. 服务定位

### 12.1 ServiceProvider 配置

配置服务提供者用于解析依赖。

**全局配置**：
- cfg.ServiceProvider = container

**运行时配置**：
- new AMapper(config, serviceProvider)

### 12.2 与 AFramework.DI 集成

AMapper 与 AFramework.DI 深度集成。

**自动注入**：
- 值解析器自动注入
- 类型转换器自动注入
- 映射动作自动注入

---

## 13. 异常处理

### 13.1 MappingException

映射执行时的异常包装类。

**包含信息**：

| 属性 | 说明 |
|------|------|
| SourceType | 源类型 |
| DestinationType | 目标类型 |
| MemberMap | 成员映射配置 |
| InnerException | 根本原因 |

### 13.2 ConfigurationException

配置验证时的异常。

**触发场景**：
- 未映射的目标成员
- 无法构造目标类型
- 类型转换不可行

### 13.3 DuplicateTypeMapException

重复配置检测异常。

**触发场景**：
- 同一类型对在多个 Profile 中配置

---

## 14. Unity 特定高级功能

### 14.1 组件映射

支持 Unity 组件之间的映射。

**特性**：
- 自动处理组件引用
- 支持 GetComponent 解析
- 支持组件复制

### 14.2 资源引用映射

支持 Unity 资源引用的映射。

**支持类型**：
- Sprite
- Texture2D
- AudioClip
- Material
- Prefab 引用

### 14.3 Addressables 集成

支持 Addressables 资源的映射。

**特性**：
- 异步加载支持
- 资源句柄映射
- 自动释放管理

### 14.4 序列化数据映射

支持 Unity 序列化数据的映射。

**特性**：
- JsonUtility 兼容
- MemoryPack 集成
- 二进制序列化支持

---

*下一部分：LINQ 投影与查询扩展*
