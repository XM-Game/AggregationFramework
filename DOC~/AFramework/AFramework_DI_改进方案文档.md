# AFramework.DI 依赖注入框架改进方案文档

> 版本：1.0.0  
> 基于对比：VContainer  
> 适用 Unity 版本：2022.3 LTS ~ Unity 6.x  
> 最后更新：2026年1月

---

## 目录

1. [文档概述](#1-文档概述)
2. [现状分析](#2-现状分析)
3. [对比总结](#3-对比总结)
4. [改进方案](#4-改进方案)
5. [实施路线图](#5-实施路线图)
6. [风险评估](#6-风险评估)
7. [验收标准](#7-验收标准)

---

## 1. 文档概述

### 1.1 文档目的

本文档旨在通过与业界成熟的 VContainer 框架进行全面对比分析，识别 AFramework.DI 在架构设计、性能优化、功能完整性等方面的改进空间，并提出系统性的改进方案和实施路线图。

### 1.2 对比对象

| 框架 | 定位 | 成熟度 |
|------|------|--------|
| AFramework.DI | AFramework 生态核心 DI 模块 | 开发中 |
| VContainer | Unity 专用高性能 DI 框架 | 成熟稳定 |

### 1.3 评估维度

- 架构设计合理性
- 运行时性能表现
- 内存分配与 GC 压力
- 功能完整性
- 扩展性与可维护性
- Unity 集成深度

---

## 2. 现状分析

### 2.1 AFramework.DI 架构概览

AFramework.DI 采用分层模块化架构，包含以下核心层级：


**核心层 (Core)**
- 接口定义：IObjectResolver、IContainerBuilder、IRegistration 等
- 特性标记：InjectAttribute、KeyAttribute、OptionalAttribute 等
- 枚举与异常：Lifetime、DIException 系列

**容器层 (Container)**
- Container：依赖注入容器主体实现
- ContainerBuilder：容器构建器
- Registry：注册表管理
- Registration：注册信息封装

**注入层 (Injection)**
- Injector：注入器核心实现
- ConstructorInjector：构造函数注入
- PropertyInjector：属性注入
- FieldInjector：字段注入
- MethodInjector：方法注入
- InjectionInfoCache：注入信息缓存

**提供者层 (Providers)**
- SingletonProvider：单例实例提供者
- ScopedProvider：作用域实例提供者
- TransientProvider：瞬态实例提供者
- FactoryProvider：工厂实例提供者

**Unity 集成层**
- LifetimeScope：生命周期作用域组件
- EntryPointRunner：入口点运行器
- PrefabInjector：预制体注入器

### 2.2 VContainer 架构概览

VContainer 采用扁平化高性能架构：

**容器系统**
- Container：根容器
- ScopedContainer：作用域容器（独立实现）
- Registry：基于自定义哈希表的注册表

**注入系统**
- InjectorCache：静态注入器缓存
- ReflectionInjector：反射注入器
- TypeAnalyzer：类型分析器（含循环依赖检测）

**性能优化组件**
- FixedTypeObjectKeyHashtable：固定大小类型键哈希表
- CappedArrayPool：有上限的数组池
- RuntimeTypeCache：运行时类型缓存

**Unity 集成**
- LifetimeScope：生命周期作用域
- 入口点接口系列
- 组件注册扩展

### 2.3 关键差异识别

通过深入分析两个框架的实现，识别出以下关键差异点：

**数据结构选择**
- AFramework.DI 使用标准 Dictionary 作为注册表和缓存
- VContainer 使用自定义 FixedTypeObjectKeyHashtable，针对类型查找优化

**线程安全策略**
- AFramework.DI 使用 lock 同步锁保护共享资源
- VContainer 使用 ConcurrentDictionary 配合 Lazy 实现无锁并发

**内存管理**
- AFramework.DI 每次创建实例时分配新数组
- VContainer 使用 CappedArrayPool 复用参数数组

**编译优化**
- AFramework.DI 未使用内联优化标记
- VContainer 在热路径方法上使用 AggressiveInlining

**循环依赖检测**
- AFramework.DI 在运行时解析过程中检测
- VContainer 在容器构建时静态检测

---

## 3. 对比总结

### 3.1 性能对比

| 维度 | AFramework.DI | VContainer | 差距评估 |
|------|---------------|------------|----------|
| 类型查找 | O(1) 哈希查找 | O(1) 优化哈希 | 中等 |
| 并发性能 | lock 阻塞 | 无锁并发 | 较大 |
| 内存分配 | 每次分配 | 池化复用 | 较大 |
| 方法调用 | 普通调用 | 内联优化 | 中等 |
| 启动时间 | 顺序构建 | 可并行构建 | 中等 |

### 3.2 功能对比

| 功能 | AFramework.DI | VContainer |
|------|---------------|------------|
| 构造函数注入 | 完整支持 | 完整支持 |
| 字段注入 | 完整支持 | 完整支持 |
| 属性注入 | 完整支持 | 完整支持 |
| 方法注入 | 完整支持 | 完整支持 |
| 键值注入 | 完整支持 | 完整支持 |
| 集合解析 | 扩展支持 | 内置支持 |
| 开放泛型 | 部分支持 | 完整支持 |
| 循环依赖检测 | 运行时 | 构建时 |
| 源代码生成 | 不支持 | 支持 |
| 诊断追踪 | 支持 | 支持 |
| ContainerLocal | 不支持 | 支持 |
| 并行构建 | 不支持 | 可选支持 |

### 3.3 架构对比

| 维度 | AFramework.DI | VContainer |
|------|---------------|------------|
| 模块化程度 | 高（12层） | 中（扁平） |
| 代码组织 | 清晰分层 | 性能优先 |
| 扩展性 | 良好 | 良好 |
| 学习曲线 | 中等 | 中等 |
| 文档完整性 | 完整中文 | 完整英文 |

### 3.4 优势与不足

**AFramework.DI 优势**
- 模块化架构清晰，易于理解和维护
- 完整的中文文档和注释
- 与 AFramework 生态深度集成
- 编辑器本地化支持
- 丰富的扩展方法

**AFramework.DI 不足**
- 性能优化不足，存在 GC 压力
- 缺少编译时优化手段
- 开放泛型支持不完整
- 循环依赖检测时机较晚

---

## 4. 改进方案

### 4.1 高优先级改进

#### 4.1.1 并发数据结构优化

**现状问题**
当前注入信息缓存使用 Dictionary 配合 lock 同步锁，在高并发场景下存在性能瓶颈。多线程同时请求注入信息时，会产生锁竞争，导致线程阻塞。

**改进目标**
- 消除锁竞争，提升并发性能
- 保持线程安全性
- 减少上下文切换开销

**改进方案**
将 InjectionInfoCache 中的 Dictionary + lock 模式替换为 ConcurrentDictionary，利用其内置的细粒度锁机制实现高效并发访问。同时使用 GetOrAdd 方法确保原子性操作。

**预期收益**
- 并发性能提升 30% 以上
- 消除锁竞争导致的线程阻塞
- 代码更简洁

**影响范围**
- InjectionInfoCache 类
- InjectorCache 类
- Container 类中的实例缓存

---

#### 4.1.2 自定义高性能哈希表

**现状问题**
注册表使用标准 Dictionary，每次查找需要计算哈希值并处理哈希冲突。对于 DI 容器这种高频查找场景，存在优化空间。

**改进目标**
- 优化类型查找性能
- 减少哈希冲突
- 支持 Type + Key 复合键高效查找

**改进方案**
参考 VContainer 的 FixedTypeObjectKeyHashtable 设计，实现专用的类型键哈希表：

- 使用 RuntimeHelpers.GetHashCode 获取类型的运行时哈希值，避免重复计算
- 采用开放寻址或链地址法处理冲突
- 构建时确定容量，运行时只读，无需扩容
- 支持 Type 和 Key 的复合键查找

**预期收益**
- 类型查找性能提升 20% 以上
- 减少哈希计算开销
- 更好的缓存局部性

**影响范围**
- 新增 TypeKeyHashtable 类
- Registry 类重构
- Container 类的注册表访问

---

#### 4.1.3 参数数组池化

**现状问题**
每次通过构造函数创建实例时，都会分配新的参数数组用于存储解析后的依赖。这些数组在方法返回后立即成为垃圾，增加 GC 压力。

**改进目标**
- 减少参数数组的内存分配
- 降低 GC 压力
- 提升实例创建性能

**改进方案**
实现有上限的数组池（CappedArrayPool），用于复用参数数组：

- 按数组长度分桶管理
- 设置池大小上限，避免内存泄漏
- 使用 ThreadStatic 或 ThreadLocal 避免线程竞争
- 提供 Rent 和 Return 方法

**预期收益**
- 减少 80% 以上的参数数组分配
- 显著降低 GC 频率
- 提升高频创建场景的性能

**影响范围**
- 新增 ArrayPool 类
- ConstructorInjector 类
- MethodInjector 类
- ReflectionInjector 类

---

#### 4.1.4 热路径内联优化

**现状问题**
解析方法是 DI 容器的热路径，频繁调用但未使用内联优化，存在方法调用开销。

**改进目标**
- 减少热路径方法调用开销
- 提升解析性能

**改进方案**
在以下热路径方法上添加 MethodImpl(MethodImplOptions.AggressiveInlining) 特性：

- Resolve 系列方法
- TryResolve 系列方法
- GetInstance 方法
- 哈希表查找方法
- 缓存访问方法

**预期收益**
- 热路径性能提升 10-15%
- 减少方法调用栈深度

**影响范围**
- Container 类
- IInstanceProvider 实现类
- TypeKeyHashtable 类
- InjectionInfoCache 类

---

### 4.2 中优先级改进

#### 4.2.1 构建时循环依赖检测

**现状问题**
当前循环依赖在运行时解析过程中检测，问题发现较晚，且每次解析都有检测开销。

**改进目标**
- 在容器构建时检测循环依赖
- 提供清晰的依赖链错误信息
- 消除运行时检测开销

**改进方案**
在 ContainerBuilder.Build 方法中添加静态循环依赖检测：

- 构建完成后遍历所有注册
- 使用深度优先搜索检测依赖图中的环
- 检测到环时抛出异常，包含完整依赖链信息
- 支持通过工厂注册打破循环

**预期收益**
- 问题更早发现，调试更容易
- 消除运行时检测开销
- 提供更好的错误诊断信息

**影响范围**
- 新增 TypeAnalyzer 类或扩展现有类
- ContainerBuilder 类
- 异常信息增强

---

#### 4.2.2 开放泛型完整支持

**现状问题**
当前开放泛型支持不完整，无法在运行时根据请求的封闭泛型类型自动创建对应实例。

**改进目标**
- 支持开放泛型类型注册
- 运行时自动构造封闭泛型类型
- 缓存已构造的封闭泛型注册

**改进方案**
实现 OpenGenericInstanceProvider：

- 注册时存储开放泛型类型定义
- 解析时检测请求类型是否为封闭泛型
- 匹配开放泛型注册，提取类型参数
- 使用 MakeGenericType 构造封闭类型
- 缓存构造结果避免重复构造

**预期收益**
- 支持泛型仓储、泛型服务等常见模式
- 减少重复注册代码
- 提升框架通用性

**影响范围**
- 新增 OpenGenericInstanceProvider 类
- Registry 类扩展
- ContainerBuilderExtensions 扩展

---

#### 4.2.3 ContainerLocal 支持

**现状问题**
缺少作用域本地值的支持，无法在不同作用域中为同一类型提供不同实例。

**改进目标**
- 支持作用域本地值
- 每个作用域可以有独立的实例
- 与现有作用域系统无缝集成

**改进方案**
实现 ContainerLocal 泛型类：

- ContainerLocal 包装一个值类型
- 每个作用域容器维护自己的 ContainerLocal 实例
- 解析时自动创建作用域本地实例
- 支持从父作用域继承或独立创建

**预期收益**
- 支持更灵活的作用域数据隔离
- 简化多租户、多场景数据管理
- 与 VContainer 功能对齐

**影响范围**
- 新增 ContainerLocal 类
- 新增 ContainerLocalInstanceProvider 类
- Registry 类扩展

---

#### 4.2.4 集合解析内置支持

**现状问题**
集合解析通过扩展方法实现，需要显式注册，不够便捷。

**改进目标**
- 内置支持 IEnumerable、IReadOnlyList、数组类型解析
- 自动收集同类型的所有注册
- 无需显式注册集合类型

**改进方案**
在 Registry 中内置集合解析逻辑：

- 注册时自动追踪同类型的多个注册
- 解析集合类型时自动聚合所有匹配注册
- 支持 IEnumerable、IReadOnlyList、T[] 三种形式
- 保持注册顺序

**预期收益**
- 简化插件系统、策略模式等场景的使用
- 减少样板代码
- 与 VContainer 行为一致

**影响范围**
- Registry 类重构
- 新增 CollectionInstanceProvider 类
- 移除或保留现有扩展方法作为兼容

---

### 4.3 低优先级改进

#### 4.3.1 源代码生成器支持

**现状问题**
所有注入信息都通过运行时反射获取，存在性能开销和 AOT 兼容性问题。

**改进目标**
- 编译时生成注入器代码
- 消除运行时反射开销
- 提升 AOT 平台兼容性

**改进方案**
开发 Roslyn Source Generator：

- 分析标记了 Inject 特性的类型
- 生成对应的 IInjector 实现类
- 运行时优先使用生成的注入器
- 回退到反射注入器作为兜底

**预期收益**
- 消除反射开销，性能提升显著
- 改善 IL2CPP、AOT 平台兼容性
- 编译时发现注入配置错误

**影响范围**
- 新增 Source Generator 项目
- InjectorCache 类扩展
- 构建流程调整

---

#### 4.3.2 并行容器构建

**现状问题**
容器构建过程顺序执行，大型项目启动时间较长。

**改进目标**
- 支持并行构建注册信息
- 缩短大型项目启动时间
- 可配置开关

**改进方案**
在 ContainerBuilder.Build 中添加并行构建选项：

- 使用 Parallel.For 并行构建 Registration
- 通过预处理宏或配置开关控制
- 确保线程安全
- 最后顺序合并结果

**预期收益**
- 大型项目启动时间缩短 30-50%
- 充分利用多核 CPU

**影响范围**
- ContainerBuilder 类
- 新增配置选项
- 预处理宏定义

---

#### 4.3.3 诊断追踪增强

**现状问题**
诊断信息收集不够完整，缺少解析追踪和性能分析。

**改进目标**
- 完整的解析链追踪
- 性能指标收集
- 可视化诊断工具

**改进方案**
增强诊断系统：

- 添加 DiagnosticsCollector 类
- 在解析路径中插入追踪点
- 收集解析时间、调用次数等指标
- 提供编辑器窗口可视化展示

**预期收益**
- 更容易定位性能问题
- 更好的依赖关系可视化
- 提升开发体验

**影响范围**
- 新增 DiagnosticsCollector 类
- Container 类扩展
- 编辑器工具开发

---

#### 4.3.4 异常信息增强

**现状问题**
部分异常信息不够详细，难以快速定位问题。

**改进目标**
- 提供更详细的错误上下文
- 包含依赖链信息
- 支持中英双语

**改进方案**
增强异常类：

- 在 ResolutionException 中包含完整解析链
- 在 CircularDependencyException 中包含循环路径
- 统一异常消息格式
- 提供问题解决建议

**预期收益**
- 缩短问题排查时间
- 提升开发体验
- 降低学习成本

**影响范围**
- 异常类重构
- 解析过程中收集上下文信息

---

## 5. 实施路线图

### 5.1 第一阶段：性能基础优化（2-3周）

**目标**：解决最关键的性能问题，建立性能基准

**任务清单**
1. 并发数据结构优化
   - 重构 InjectionInfoCache 使用 ConcurrentDictionary
   - 重构 InjectorCache 使用 ConcurrentDictionary
   - 优化 Container 实例缓存

2. 热路径内联优化
   - 识别所有热路径方法
   - 添加 AggressiveInlining 特性
   - 性能测试验证

3. 参数数组池化
   - 实现 CappedArrayPool 类
   - 集成到 ConstructorInjector
   - 集成到 MethodInjector

**交付物**
- 优化后的缓存实现
- 数组池实现
- 性能对比报告

---

### 5.2 第二阶段：数据结构优化（2-3周）——————————————————————————————

**目标**：实现高性能注册表，进一步提升查找性能

**任务清单**
1. 自定义哈希表实现
   - 设计 TypeKeyHashtable 接口
   - 实现固定大小哈希表
   - 支持 Type + Key 复合键

2. Registry 重构
   - 使用新哈希表替换 Dictionary
   - 优化注册流程
   - 保持 API 兼容

3. 性能验证
   - 基准测试
   - 与 VContainer 对比
   - 优化调整

**交付物**
- TypeKeyHashtable 实现
- 重构后的 Registry
- 性能测试报告

---

### 5.3 第三阶段：功能完善（3-4周）

**目标**：补齐功能短板，提升框架完整性

**任务清单**
1. 构建时循环依赖检测
   - 实现 TypeAnalyzer 类
   - 集成到构建流程
   - 增强错误信息

2. 开放泛型支持
   - 实现 OpenGenericInstanceProvider
   - 扩展 Registry 支持
   - 添加注册扩展方法

3. 集合解析内置支持
   - 实现 CollectionInstanceProvider
   - Registry 自动聚合逻辑
   - 兼容现有扩展方法

4. ContainerLocal 支持
   - 实现 ContainerLocal 类
   - 实现对应 Provider
   - 集成到作用域系统

**交付物**
- 完整的功能实现
- 单元测试覆盖
- 使用文档更新

---

### 5.4 第四阶段：高级特性（4-6周）————————————————————

**目标**：实现高级优化特性，达到业界领先水平

**任务清单**
1. 源代码生成器
   - 设计生成器架构
   - 实现类型分析
   - 实现代码生成
   - 集成到构建流程

2. 并行容器构建
   - 实现并行构建逻辑
   - 添加配置开关
   - 性能测试验证

3. 诊断系统增强
   - 实现 DiagnosticsCollector
   - 开发编辑器可视化工具
   - 性能分析功能

**交付物**
- Source Generator 包
- 并行构建支持
- 诊断工具

---

### 5.5 里程碑时间线

| 里程碑 | 时间 | 主要内容 |
|--------|------|----------|
| M1 | 第3周 | 性能基础优化完成 |
| M2 | 第6周 | 数据结构优化完成 |
| M3 | 第10周 | 功能完善完成 |
| M4 | 第16周 | 高级特性完成 |

---

## 6. 风险评估

### 6.1 技术风险

**风险1：并发数据结构引入新问题**
- 可能性：中
- 影响：高
- 缓解措施：充分的并发测试，保留回退选项

**风险2：自定义哈希表实现复杂度**
- 可能性：中
- 影响：中
- 缓解措施：参考成熟实现，充分测试边界情况

**风险3：源代码生成器兼容性**
- 可能性：高
- 影响：中
- 缓解措施：保留反射回退，渐进式推广

### 6.2 兼容性风险

**风险4：API 变更影响现有用户**
- 可能性：低
- 影响：高
- 缓解措施：保持 API 兼容，内部重构

**风险5：Unity 版本兼容性**
- 可能性：中
- 影响：中
- 缓解措施：使用预处理宏，多版本测试

### 6.3 进度风险

**风险6：工作量估算偏差**
- 可能性：中
- 影响：中
- 缓解措施：预留缓冲时间，优先级排序

---

## 7. 验收标准

### 7.1 性能指标

| 指标 | 当前值 | 目标值 | 测试方法 |
|------|--------|--------|----------|
| 单次解析耗时 | 基准 | 降低 30% | 基准测试 |
| 并发解析吞吐 | 基准 | 提升 50% | 压力测试 |
| GC 分配 | 基准 | 降低 80% | 内存分析 |
| 启动时间 | 基准 | 降低 30% | 启动测试 |

### 7.2 功能验收

| 功能 | 验收标准 |
|------|----------|
| 并发安全 | 多线程压力测试无死锁、无数据竞争 |
| 开放泛型 | 支持常见泛型模式，自动构造封闭类型 |
| 循环依赖检测 | 构建时检测，提供清晰错误信息 |
| 集合解析 | 自动聚合同类型注册，支持三种集合形式 |
| ContainerLocal | 作用域隔离正确，生命周期管理正确 |

### 7.3 质量验收

| 维度 | 验收标准 |
|------|----------|
| 单元测试覆盖率 | 核心模块 80% 以上 |
| 文档完整性 | API 文档、使用指南、迁移指南 |
| 代码规范 | 符合项目编码规范，通过静态分析 |
| 兼容性 | Unity 2022.3 ~ Unity 6.x 全版本通过 |

---

## 附录

### A. 参考资料

- VContainer 官方文档
- Microsoft.Extensions.DependencyInjection 源码
- Unity 性能优化最佳实践

### B. 术语表

| 术语 | 说明 |
|------|------|
| DI | Dependency Injection，依赖注入 |
| IoC | Inversion of Control，控制反转 |
| Singleton | 单例生命周期 |
| Scoped | 作用域生命周期 |
| Transient | 瞬态生命周期 |
| AggressiveInlining | 积极内联优化 |
| Source Generator | 源代码生成器 |

### C. 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0.0 | 2026-01 | 初始版本 |


---

## 8. 实施进度记录

### 8.1 第一阶段：性能基础优化 ✅ 已完成

**完成日期**：2026年1月

**已完成任务**：

1. **并发数据结构优化** ✅
   - InjectionInfoCache 已改用 ConcurrentDictionary
   - 添加 AggressiveInlining 优化

2. **热路径内联优化** ✅
   - Container.Resolve 系列方法已添加 AggressiveInlining
   - ConstructorInjector、MethodInjector、FieldInjector、PropertyInjector 已优化
   - Registry 查找方法已优化

3. **参数数组池化** ✅
   - 新增 `CappedArrayPool.cs` - ThreadStatic 数组池实现
   - 已集成到 ConstructorInjector
   - 已集成到 MethodInjector
   - 已集成到 Container.InstantiateType 和 InjectMethods

**新增文件**：
- `Plugins/AFramework/DI/Runtime/Core/Internal/CappedArrayPool.cs`

---

### 8.2 第二阶段：数据结构优化 ✅ 已完成

**完成日期**：2026年1月

**已完成任务**：

1. **自定义哈希表实现** ✅
   - 新增 `TypeKeyHashtable.cs` - 高性能类型键哈希表
   - 使用 RuntimeHelpers.GetHashCode 优化类型哈希
   - 支持 Type + Key 复合键查找

2. **Registry 重构** ✅
   - 添加 Freeze() 方法支持构建时/运行时两种模式
   - 构建完成后使用 TypeKeyHashtable 进行只读查找
   - 保持 API 兼容性

**新增文件**：
- `Plugins/AFramework/DI/Runtime/Core/Internal/TypeKeyHashtable.cs`

---

### 8.3 第三阶段：功能完善 ✅ 已完成

**完成日期**：2026年1月

**已完成任务**：

1. **构建时循环依赖检测** ✅
   - 新增 `DependencyAnalyzer.cs` - 使用 DFS 算法检测循环依赖
   - 已集成到 ContainerBuilder.ValidateLifetimes
   - 提供清晰的依赖链错误信息

2. **开放泛型完整支持** ✅
   - 新增 `OpenGenericProvider.cs` - 开放泛型实例提供者
   - Registry 添加 AddOpenGeneric 和 TryGetOpenGeneric 方法
   - Container 添加开放泛型解析支持
   - ContainerBuilderExtensions 添加完整的开放泛型注册方法：
     - `RegisterOpenGeneric(Type, Type, Lifetime)`
     - `RegisterOpenGeneric<TService, TImplementation>(Lifetime)`
     - `RegisterOpenGenericSingleton(Type, Type)`
     - `RegisterOpenGenericScoped(Type, Type)`

3. **集合解析内置支持** ✅
   - Container 添加内置集合类型自动解析
   - 支持 `IEnumerable<T>`、`IReadOnlyList<T>`、`IReadOnlyCollection<T>`、`IList<T>`、`List<T>`、`T[]`
   - 无需显式注册，自动聚合同类型的所有注册
   - 保留现有 CollectionRegistrationExtensions 作为兼容

**新增文件**：
- `Plugins/AFramework/DI/Runtime/Core/Internal/DependencyAnalyzer.cs`
- `Plugins/AFramework/DI/Runtime/Container/Providers/OpenGenericProvider.cs`

**修改文件**：
- `Plugins/AFramework/DI/Runtime/Container/Registry.cs` - 添加开放泛型支持
- `Plugins/AFramework/DI/Runtime/Container/Container.cs` - 添加开放泛型和集合解析
- `Plugins/AFramework/DI/Runtime/Container/ContainerBuilder.cs` - 添加开放泛型注册内部方法
- `Plugins/AFramework/DI/Runtime/Container/ContainerBuilderExtensions.cs` - 改进开放泛型注册扩展

---

### 8.4 第四阶段：高级特性 🔄 部分完成

**已完成任务**：

1. **ContainerLocal 支持** ✅
   - 新增 `ContainerLocal.cs` - 容器本地值泛型类
   - 新增 `ContainerLocalRegistrationExtensions.cs` - 注册扩展方法
   - 支持每个作用域独立实例
   - 支持延迟初始化（工厂函数）
   - 支持依赖注入的工厂函数
   - 支持同时注册值类型解析

**待实施任务**：

1. **源代码生成器** ⏳
   - 设计生成器架构
   - 实现类型分析
   - 实现代码生成
   - 集成到构建流程

2. **并行容器构建** ⏳
   - 实现并行构建逻辑
   - 添加配置开关
   - 性能测试验证

3. **诊断系统增强** ⏳
   - 实现 DiagnosticsCollector
   - 开发编辑器可视化工具
   - 性能分析功能

---

### 8.5 改进总结

**已完成的主要改进**：

| 改进项 | 状态 | 预期收益 |
|--------|------|----------|
| ConcurrentDictionary 并发优化 | ✅ | 并发性能提升 30%+ |
| AggressiveInlining 热路径优化 | ✅ | 热路径性能提升 10-15% |
| CappedArrayPool 数组池化 | ✅ | 减少 80%+ 参数数组分配 |
| TypeKeyHashtable 高性能哈希表 | ✅ | 类型查找性能提升 20%+ |
| Registry Freeze 模式 | ✅ | 运行时只读，更好的缓存局部性 |
| DependencyAnalyzer 循环依赖检测 | ✅ | 构建时发现问题，消除运行时开销 |
| OpenGenericProvider 开放泛型 | ✅ | 支持泛型仓储等常见模式 |
| 内置集合解析 | ✅ | 无需显式注册，自动聚合 |
| ContainerLocal 作用域本地值 | ✅ | 每个作用域独立实例 |

**新增文件（第四阶段）**：
- `Plugins/AFramework/DI/Runtime/Core/Extensions/ContainerLocal/ContainerLocal.cs`
- `Plugins/AFramework/DI/Runtime/Core/Extensions/ContainerLocal/ContainerLocalRegistrationExtensions.cs`

**代码质量**：
- 所有新增和修改的文件均通过编译检查
- 保持 API 向后兼容
- 完整的中英双语注释
- 符合 Unity 2022.3 ~ Unity 6.x 版本要求
