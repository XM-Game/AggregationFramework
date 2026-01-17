# AFramework.AMapper 技术文档

## 第二部分：映射配置详解

---

## 1. 基础映射配置

### 1.1 CreateMap 方法

CreateMap 是定义类型映射的核心方法，用于建立源类型到目标类型的映射关系。

**基本特性**：
- 左侧为源类型，右侧为目标类型
- 默认按属性名称匹配
- 支持泛型和非泛型两种形式

**MemberList 参数**：
- `MemberList.Destination`（默认）：验证所有目标成员都有映射
- `MemberList.Source`：验证所有源成员都被使用
- `MemberList.None`：跳过验证

### 1.2 MappingProfile 配置

MappingProfile 是组织映射配置的推荐方式，提供配置隔离和复用能力。

**Profile 特性**：
- 继承 MappingProfile 基类
- 在构造函数中定义映射
- 支持独立的命名约定
- 支持独立的全局忽略规则
- 可通过程序集扫描自动注册

**Profile 配置选项**：

| 选项 | 说明 |
|------|------|
| SourceMemberNamingConvention | 源成员命名约定 |
| DestinationMemberNamingConvention | 目标成员命名约定 |
| AllowNullDestinationValues | 允许空目标值 |
| AllowNullCollections | 允许空集合 |
| ShouldMapProperty | 属性过滤器 |
| ShouldMapField | 字段过滤器 |

**Profile 方法**：

| 方法 | 说明 |
|------|------|
| CreateMap | 创建类型映射 |
| RecognizePrefixes | 识别源成员前缀 |
| RecognizePostfixes | 识别源成员后缀 |
| RecognizeDestinationPrefixes | 识别目标成员前缀 |
| RecognizeDestinationPostfixes | 识别目标成员后缀 |
| ReplaceMemberName | 成员名称替换 |
| AddGlobalIgnore | 全局忽略 |
| DisableConstructorMapping | 禁用构造函数映射 |

**配置作用域**：
- Profile 内的配置仅影响该 Profile 的映射
- 根配置（MapperConfiguration）影响所有映射
- Profile 配置可覆盖全局配置

### 1.3 程序集扫描

AMapper 支持自动扫描程序集中的 Profile 类。

**扫描方式**：
- 按程序集扫描：`cfg.AddMaps(assembly)`
- 按标记类型扫描：`cfg.AddMaps(typeof(GameProfile))`
- 按命名空间扫描：`cfg.AddMapsFromNamespace("Game.Mapping")`

---

## 2. 成员映射配置

### 2.1 ForMember 配置

ForMember 用于配置单个目标成员的映射规则。

**常用选项**：

| 选项 | 说明 |
|------|------|
| MapFrom | 指定源成员或表达式 |
| Ignore | 忽略该成员 |
| Condition | 条件映射 |
| PreCondition | 前置条件 |
| NullSubstitute | 空值替换 |
| ConvertUsing | 使用值转换器 |
| UseDestinationValue | 使用目标现有值 |
| SetMappingOrder | 设置映射顺序 |

### 2.2 MapFrom 详解

MapFrom 是最常用的成员映射配置，支持多种形式。

**表达式形式**：
- Lambda 表达式指定源成员
- 支持嵌套属性访问
- 支持计算表达式

**值解析器形式**：
- 使用 IValueResolver 接口
- 支持依赖注入
- 适合复杂解析逻辑

**字符串路径形式**：
- 使用点号分隔的路径字符串
- 运行时解析
- 适合动态配置

### 2.3 Ignore 配置

Ignore 用于忽略目标成员，使其不参与映射和验证。

**使用场景**：
- 目标成员由其他方式填充
- 目标成员不需要映射
- 避免配置验证错误

### 2.4 条件映射

条件映射允许根据运行时条件决定是否执行映射。

**Condition vs PreCondition**：

| 特性 | Condition | PreCondition |
|------|-----------|--------------|
| 执行时机 | 源值解析后 | 源值解析前 |
| 性能 | 较低 | 较高 |
| 可访问数据 | 源值、目标值 | 仅源对象 |

**执行顺序**：
1. PreCondition 评估
2. 源值解析
3. Condition 评估
4. 目标值赋值

---

## 3. 扁平化映射

### 3.1 自动扁平化

AMapper 自动支持将嵌套对象扁平化到目标类型。

**匹配规则**：
- PascalCase 分词匹配
- 支持属性链（Customer.Name → CustomerName）
- 支持 Get 前缀方法（GetTotal() → Total）

**禁用扁平化**：
- 使用 ExactMatchNamingConvention
- 设置 DestinationMemberNamingConvention

### 3.2 IncludeMembers

IncludeMembers 提供更精细的扁平化控制。

**特性**：
- 将子对象成员映射到目标
- 需要子类型到目标类型的映射
- 支持多个子对象
- 按参数顺序优先匹配

**与传统扁平化的区别**：
- 传统扁平化：不需要子类型映射
- IncludeMembers：复用已有的子类型映射配置

---

## 4. 投影映射

### 4.1 ForMember 投影

投影用于将源属性转换为不同结构的目标属性。

**使用场景**：
- 属性名称不匹配
- 需要计算值
- 需要类型转换

### 4.2 ForPath 配置

ForPath 用于映射到嵌套目标属性。

**特性**：
- 支持深层嵌套路径
- 自动创建中间对象
- 与 ReverseMap 集成

---

## 5. 嵌套映射

### 5.1 自动嵌套映射

当源和目标都有复杂类型属性时，AMapper 自动应用嵌套映射。

**要求**：
- 必须配置子类型的映射
- 配置顺序无关紧要
- 支持任意深度嵌套

### 5.2 集合映射

AMapper 自动处理集合类型的映射。

**支持的集合类型**：
- 数组（T[]）
- List\<T\>
- IEnumerable\<T\>
- ICollection\<T\>
- IList\<T\>
- Dictionary\<TKey, TValue\>
- HashSet\<T\>
- Unity NativeArray\<T\>（需要特殊处理）

**集合映射特性**：
- 只需配置元素类型映射
- 自动处理集合转换
- 默认清空目标集合
- 空源集合映射为空集合（非 null）

### 5.3 空集合处理

**默认行为**：
- null 源集合 → 空集合（非 null）
- 遵循 Unity 最佳实践

**自定义行为**：
- AllowNullCollections = true：允许 null 集合
- 可全局、Profile 级或成员级配置

---

## 6. 继承映射

### 6.1 Include 配置

Include 用于从基类映射继承配置到派生类映射。

**配置方式**：
- 在基类映射中使用 Include
- 在派生类映射中使用 IncludeBase
- 使用 IncludeAllDerived 自动包含所有派生类

**继承内容**：
- 自定义成员映射
- 忽略配置
- 前置/后置映射动作
- 值转换器

### 6.2 运行时多态

AMapper 支持运行时多态映射。

**工作原理**：
- 根据源对象的实际类型选择映射
- 需要配置 Include 关系
- 自动选择最具体的映射

### 6.3 As 配置

As 用于将基类映射重定向到派生类映射。

**使用场景**：
- 简单的类型替换
- 不需要完整的继承配置

### 6.4 继承优先级

映射属性的优先级（从高到低）：
1. 显式映射（MapFrom）
2. 继承的显式映射
3. 忽略配置
4. 约定映射

---

## 7. 反向映射

### 7.1 ReverseMap 配置

ReverseMap 自动创建反向映射配置。

**特性**：
- 自动反转 MapFrom 配置
- 支持反扁平化（Unflattening）
- 可自定义反向映射

### 7.2 反扁平化

反扁平化将扁平属性映射回嵌套结构。

**自动反扁平化**：
- CustomerName → Customer.Name
- 基于原始 MapFrom 配置

**自定义反扁平化**：
- 使用 ForPath 配置
- 使用 Ignore 禁用

### 7.3 ReverseMap 限制

- 默认 MemberList.None（不验证）
- 复杂映射可能需要手动配置
- 某些配置不能自动反转

---

## 8. 命名约定

### 8.1 内置命名约定

| 约定 | 说明 | 示例 |
|------|------|------|
| PascalCaseNamingConvention | PascalCase 命名（默认） | PlayerName |
| CamelCaseNamingConvention | camelCase 命名 | playerName |
| SnakeCaseNamingConvention | snake_case 命名 | player_name |
| ExactMatchNamingConvention | 精确匹配，禁用扁平化 | - |

**命名约定工作原理**：
- Split 方法将成员名称分割为单词数组
- SeparatorCharacter 定义单词连接符
- 用于源和目标成员名称的匹配

### 8.2 前缀/后缀识别

**源成员前缀**：
- 默认识别 "Get" 前缀
- RecognizePrefixes 添加自定义前缀
- ClearPrefixes 清除默认前缀

**源成员后缀**：
- RecognizePostfixes 添加后缀识别

**目标成员前缀/后缀**：
- RecognizeDestinationPrefixes：目标前缀
- RecognizeDestinationPostfixes：目标后缀

### 8.3 成员名称替换

ReplaceMemberName 用于替换成员名称中的字符或单词。

**使用场景**：
- 特殊字符替换（如 "_" → ""）
- 单词翻译
- 命名规范转换

### 8.4 Unity 命名约定

AMapper 提供 Unity 特定的命名约定支持：

| 约定 | 说明 |
|------|------|
| UnitySerializedFieldConvention | 支持 m_ 前缀的序列化字段 |
| UnityPropertyConvention | 支持 Unity 属性命名风格 |

---

## 9. 全局配置

### 9.1 全局忽略

AddGlobalIgnore 用于全局忽略特定前缀的属性。

**使用场景**：
- 忽略所有以特定前缀开头的属性
- 跨所有映射生效

### 9.2 属性/字段过滤

**ShouldMapProperty**：
- 控制哪些属性参与映射
- 默认：公共属性
- 接收 PropertyInfo 参数

**ShouldMapField**：
- 控制哪些字段参与映射
- 默认：公共字段
- 接收 FieldInfo 参数

### 9.3 构造函数过滤

**ShouldUseConstructor**：
- 控制哪些构造函数可用于目标对象创建
- 默认：所有公共构造函数
- 接收 ConstructorInfo 参数

**DisableConstructorMapping**：
- 完全禁用构造函数映射
- 强制使用默认构造函数

### 9.4 ForAllMaps / ForAllMemberMaps

**ForAllMaps**：
- 对所有 TypeMap 应用配置
- 在 Configure 阶段执行
- 可访问 TypeMap 和 IMappingExpression

**ForAllMemberMaps**：
- 对所有 MemberMap 应用配置
- 支持条件过滤

---

## 10. 配置验证

### 10.1 AssertConfigurationIsValid

验证所有映射配置的有效性。

**验证内容**：
- 所有目标成员都有映射源
- 类型转换可行
- 无循环依赖

**使用建议**：
- 仅在开发/测试阶段使用
- 发布前移除或条件编译

### 10.2 验证选项

**MemberList 选项**：
- Destination：验证目标成员
- Source：验证源成员
- None：跳过验证

**自定义验证**：
- 可扩展验证逻辑
- 添加自定义验证规则

### 10.3 配置编译

**延迟编译**（默认）：
- 首次映射时编译
- 分散编译开销

**预编译**：
- CompileMappings() 方法
- 启动时一次性编译
- 适合大量映射场景
- 建议在 Unity 加载界面执行

---

## 11. 特性映射

### 11.1 AutoMapAttribute

使用特性声明类型映射。

**支持的配置**：

| 属性 | 说明 |
|------|------|
| ReverseMap | 是否创建反向映射 |
| MaxDepth | 最大映射深度 |
| PreserveReferences | 保留引用 |
| DisableCtorValidation | 禁用构造函数验证 |
| IncludeAllDerived | 包含所有派生类 |

### 11.2 成员特性

| 特性 | 说明 |
|------|------|
| IgnoreAttribute | 忽略成员 |
| MapFromAttribute | 指定源成员 |
| MapToAttribute | 指定目标成员 |
| NullSubstituteAttribute | 空值替换 |
| MappingOrderAttribute | 映射顺序 |

### 11.3 特性映射限制

- 不支持表达式配置
- 不支持扁平化配置
- 功能相对有限
- 适合简单映射场景

---

## 12. 开放泛型映射

### 12.1 开放泛型配置

AMapper 支持开放泛型类型的映射配置。

**配置方式**：
- 使用 typeof(Source<>) 语法
- 运行时自动关闭泛型

**特性**：
- 配置一次，适用所有封闭类型
- 支持泛型类型转换器
- 支持泛型值解析器

### 12.2 泛型类型转换器

支持任意数量泛型参数的类型转换器。

**参数映射**：
- 源类型的封闭类型 → 第一个泛型参数
- 目标类型的封闭类型 → 第二个泛型参数

---

## 13. Unity 特定配置

### 13.1 ScriptableObject 映射

AMapper 提供 ScriptableObject 的特殊支持。

**配置方式**：
- 使用 ScriptableObjectProfile 基类
- 自动处理 Unity 资源引用

### 13.2 序列化字段映射

支持 Unity 序列化字段的映射。

**特性**：
- 识别 [SerializeField] 特性
- 支持 m_ 前缀字段
- 支持私有序列化字段

### 13.3 Unity 类型自动映射

以下 Unity 类型自动支持映射：

| 类型 | 映射方式 |
|------|---------|
| Vector2/3/4 | 组件级映射 |
| Quaternion | 组件级映射 |
| Color/Color32 | 组件级映射 |
| Rect | 组件级映射 |
| Bounds | 组件级映射 |
| Matrix4x4 | 组件级映射 |

---

*下一部分：高级映射功能*
