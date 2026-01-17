# AFramework.AMapper 技术文档

## 第四部分：LINQ 投影与查询扩展

---

## 1. ProjectTo 概述

### 1.1 什么是 ProjectTo

ProjectTo 是 AMapper 的 LINQ 扩展方法，用于将 IQueryable 或 IEnumerable 投影到 DTO 类型。

**核心优势**：
- 直接生成投影表达式
- 只处理需要的属性
- 避免不必要的数据加载
- 在数据源层面完成转换

### 1.2 ProjectTo vs Map

| 特性 | ProjectTo | Map |
|------|-----------|-----|
| 执行位置 | 数据源/查询提供程序 | 内存 |
| 数据加载 | 按需加载 | 全量加载 |
| 性能 | 更优（大数据量） | 灵活 |
| 功能支持 | 受限于 LINQ 提供程序 | 完整功能 |

### 1.3 适用场景

**推荐使用 ProjectTo**：
- 数据库查询（配合 ORM）
- 大数据集处理
- 只读数据展示
- API 响应构建

**推荐使用 Map**：
- 内存对象转换
- 需要完整映射功能
- 复杂业务逻辑
- 双向映射

---

## 2. 基本用法

### 2.1 简单投影

**使用方式**：
- source.ProjectTo\<TDestination\>(configuration)
- mapper.ProjectTo\<TDestination\>(source)

**要求**：
- ProjectTo 应是 LINQ 链的最后一个调用
- 在 ProjectTo 之前应用过滤和排序

### 2.2 自定义投影

使用 MapFrom 配置自定义投影表达式。

**支持的表达式**：
- 属性访问
- 方法调用（需 LINQ 提供程序支持）
- 聚合函数
- 条件表达式

### 2.3 字符串转换

AMapper 自动添加 ToString() 转换。

**适用场景**：
- 枚举到字符串
- 数值到字符串
- 任何类型到字符串

---

## 3. 高级投影功能

### 3.1 嵌套投影

支持嵌套对象的投影。

**特性**：
- 自动应用子类型映射
- 生成嵌套 SELECT
- 避免 N+1 问题

### 3.2 集合投影

支持集合属性的投影。

**生成的查询**：
- 使用子查询
- 自动 JOIN
- 高效的数据加载

### 3.3 聚合投影

支持 LINQ 聚合函数。

**支持的聚合**：

| 聚合 | 说明 |
|------|------|
| Count() | 计数 |
| Sum() | 求和 |
| Average() | 平均值 |
| Min() | 最小值 |
| Max() | 最大值 |

**自动匹配**：
- PropertyCount → Property.Count()
- PropertySum → Property.Sum()

---

## 4. 显式展开

### 4.1 ExplicitExpansion

控制哪些成员在投影中展开。

**配置方式**：
- ForMember(d => d.Member, opt => opt.ExplicitExpansion())

**使用场景**：
- 按需加载
- 性能优化
- 减少数据传输

### 4.2 展开参数

ProjectTo 时指定要展开的成员。

**表达式形式**：
- ProjectTo\<T\>(config, dest => dest.Member)

**字符串形式**：
- ProjectTo\<T\>(config, null, "Member")

**集合成员**：
- dest => dest.Items.Select(i => i.SubProperty)

---

## 5. 参数化投影

### 5.1 运行时参数

投影表达式中使用运行时参数。

**配置方式**：
- 在 MapFrom 中捕获闭包变量
- 变量名作为参数名

### 5.2 参数传递

ProjectTo 时传递参数值。

**匿名对象形式**：
- ProjectTo\<T\>(config, new { paramName = value })

**字典形式**：
- ProjectTo\<T\>(config, new Dictionary\<string, object\> { ... })

---

## 6. 自定义类型转换

### 6.1 表达式形式的 ConvertUsing

投影支持表达式形式的类型转换。

**配置方式**：
- ConvertUsing(src => new Dest { ... })

**限制**：
- 必须是表达式
- 受 LINQ 提供程序限制

### 6.2 自定义构造函数

使用 ConstructUsing 自定义目标构造。

**配置方式**：
- ConstructUsing(src => new Dest(src.Value))

**使用场景**：
- 非默认构造函数
- 构造时计算

---

## 7. 空值安全

### 7.1 EnableNullPropagationForQueryMapping

启用查询映射的空值传播。

**配置方式**：
- cfg.EnableNullPropagationForQueryMapping = true

**效果**：
- 自动添加空值检查
- 避免 NullReferenceException

### 7.2 空值处理策略

**处理内容**：
- 成员访问：属性/字段访问
- 方法调用：方法调用（包括扩展方法）
- 自动插入空值检查条件

**处理策略**：
- 对于集合类型：使用空集合替代 null
- 对于引用类型：使用 Coalesce 或条件表达式
- 对于值类型：直接返回（无需检查）

---

## 8. 递归查询

### 8.1 RecursiveQueriesMaxDepth

配置递归查询的最大深度。

**配置方式**：
- configuration.RecursiveQueriesMaxDepth = n

**使用场景**：
- 自引用类型
- 树形结构
- 层级数据

**注意事项**：
- 尽量避免递归模型
- 性能影响较大

---

## 9. 多态投影

### 9.1 Include/IncludeBase

投影支持运行时多态。

**配置方式**：
- 与 Map 相同的 Include 配置
- 自动选择正确的投影

### 9.2 限制

- 需要 LINQ 提供程序支持
- 某些 ORM 可能不支持

---

## 10. 投影限制

### 10.1 不支持的功能

以下功能在 ProjectTo 中不可用：

| 功能 | 原因 |
|------|------|
| Condition | LINQ 提供程序限制 |
| SetMappingOrder | 无意义 |
| UseDestinationValue | 无目标对象 |
| MapFrom（Func 形式） | 无法转换为表达式 |
| Before/AfterMap | 无执行时机 |
| 自定义解析器 | 无法转换为查询 |
| 自定义类型转换器（Func） | 无法转换为表达式 |
| ForPath | LINQ 限制 |
| 值转换器 | 内存操作 |

### 10.2 支持的功能

| 功能 | 说明 |
|------|------|
| MapFrom（表达式） | 转换为查询 |
| ConvertUsing（表达式） | 转换为查询 |
| Ignore | 不查询该属性 |
| NullSubstitute | COALESCE |
| IncludeMembers | 支持 |
| Include/IncludeBase | 多态支持 |

---

## 11. 性能优化

### 11.1 查询优化建议

**最佳实践**：
- 在 ProjectTo 前应用过滤
- 使用分页
- 避免过深嵌套
- 使用 ExplicitExpansion

### 11.2 避免 N+1

ProjectTo 自动避免 N+1 问题。

**原理**：
- 生成单个查询
- 使用 JOIN 或子查询
- 不触发延迟加载

### 11.3 缓存机制

投影表达式被缓存以提高性能。

**缓存策略**：
- 按类型对缓存
- 首次投影时构建
- 后续调用复用

---

## 12. 调试投影

### 12.1 查看生成的表达式

检查 IQueryable.Expression 属性。

**方法**：
- var expression = query.ProjectTo\<T\>().Expression

### 12.2 常见问题

**表达式不支持**：
- 检查 MapFrom 表达式
- 简化复杂逻辑
- 移至内存处理

**性能问题**：
- 检查生成的查询
- 优化查询结构
- 考虑分页

---

## 13. Unity 特定投影

### 13.1 ScriptableObject 集合投影

支持 ScriptableObject 集合的投影。

**特性**：
- 自动处理资源引用
- 支持 AssetDatabase 查询
- 编辑器模式优化

### 13.2 配置数据投影

支持游戏配置数据的投影。

**使用场景**：
- 配置表数据转换
- 运行时数据视图
- 编辑器数据展示

### 13.3 MasterMemory 集成

支持 MasterMemory 数据库的投影。

**特性**：
- 高性能内存查询
- 类型安全投影
- 索引优化

---

## 14. 集合扩展方法

### 14.1 MapToList

将集合映射为 List。

**使用方式**：
- source.MapToList\<TDestination\>(mapper)

### 14.2 MapToArray

将集合映射为数组。

**使用方式**：
- source.MapToArray\<TDestination\>(mapper)

### 14.3 MapToDictionary

将集合映射为字典。

**使用方式**：
- source.MapToDictionary\<TKey, TValue\>(mapper, keySelector)

### 14.4 MapToHashSet

将集合映射为 HashSet。

**使用方式**：
- source.MapToHashSet\<TDestination\>(mapper)

---

*下一部分：依赖注入与集成*
