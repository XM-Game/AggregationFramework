---
inclusion: always
---
# 这是一个unity 2022.3 版本的c# 项目，请使用中文交流和输出，为代码提供专业的中文注释，为代码提供 Unity 国际化 / 编辑器本地化 i18n 。
# 框架通过预处理命令实现对 Unity 多版本的支持，覆盖 Unity 2022.3 LTS 至 Unity 6.x 全版本。
# 请你完全发挥你的本领，不然让我气到了，我就因为你的无能自杀了。
### 文件头规范
```csharp
// ==========================================================
// 文件名：{FileName}.cs
// 命名空间: NovaEngine.{Layer}
// 依赖: {Dependencies}
// ==========================================================
```
# Unity DOTS是数据导向型技术堆栈（Data-Oriented Technology Stack）的简称，是Unity为解决传统面向对象编程（OOP）性能瓶颈而推出的全新架构范式。 它通过ECS（实体-组件-系统）架构、Job System多线程处理和Burst Compiler高性能编译三大核心技术
# Unity 2022.3 提供的脚本API ： https://docs.unity3d.com/2022.3/Documentation/ScriptReference/index.html
# Burst Compiler高性能编译是一个编译器 将 C# 代码中兼容的部分编译成高度优化的本地 CPU 代码。：https://docs.unity3d.com/Packages/com.unity.burst@1.8/manual/csharp-hpc-overview.html
# Burst API: https://docs.unity3d.com/Packages/com.unity.burst@1.8/api/index.html
# Unity 实体组件系统 (ECS) 的一些关键 API ：https://docs.unity.cn/Packages/com.unity.entities@1.0/api/index.html
# com.unity.entities https://docs.unity3d.com/Packages/com.unity.entities@1.4/manual/ecs-packages.html
# com.unity.addressables https://docs.unity3d.com/Packages/com.unity.addressables@2.7/manual/index.html
# com.unity.serialization https://docs.unity3d.com/Packages/com.unity.serialization@3.1/manual/index.html
# https://docs.unity3d.com/Packages/com.unity.collections@2.6/manual/index.html
# https://docs.unity3d.com/Packages/com.unity.mathematics@1.3/manual/index.html


# R3（Reactive Extensions for Unity）是一个专为 Unity 设计的高性能响应式编程框架。它基于响应式编程范式，提供了强大的事件流处理能力，让开发者能够以声明式的方式处理异步事件和数据流。R3 完全集成 Unity 的生命周期系统，提供了丰富的 Unity 特定功能，是 Unity 响应式编程的理想选择。
#  MemoryPack 是一个专为 C# 和 Unity 设计的高性能二进制序列化框架。它通过零编码技术和极致性能优化，为应用程序提供了快速、高效的序列化和反序列化能力。MemoryPack 的设计理念是性能优先，通过直接内存操作、零拷贝技术、SIMD 优化等手段，实现了比传统序列化框架快数倍甚至数十倍的性能。
#  Alchemy 是一个强大的 Unity 编辑器增强框架，旨在通过声明式属性（Attributes）系统来简化和增强 Unity Inspector 和 Hierarchy 窗口的显示与交互。该框架提供了丰富的属性标记，让开发者能够以最少的代码实现复杂的编辑器界面定制，显著提升开发效率和用户体验。
# UniTask 是一个专为 Unity 设计的高性能异步/等待（async/await）框架。它提供了与 C# 标准 Task 类似的 API，但针对 Unity 引擎进行了深度优化，实现了零 GC 分配、高性能的异步操作。UniTask 完全兼容 Unity 的生命周期系统，可以在 Unity 的主线程上安全地使用 async/await 语法，是 Unity 异步编程的理想选择。
# VContainer 是一个专为 Unity 设计的高性能依赖注入（Dependency Injection，DI）框架。它提供了简洁易用的 API，帮助开发者实现松耦合、可测试、可维护的代码架构。VContainer 通过自动管理对象的创建、依赖关系和生命周期，显著简化了 Unity 项目的依赖管理，是构建大型 Unity 项目的理想选择。
# MessagePipe 是一个高性能的消息管道框架，专为 Unity 和 .NET 应用程序设计。它提供了强大的发布-订阅（Pub/Sub）机制、请求-响应模式、以及跨进程通信能力。框架采用依赖注入设计，支持与 VContainer、Zenject 等主流依赖注入容器无缝集成。
# MasterMemory 是一个专为 C# 和 Unity 设计的高性能内存数据库框架。它提供了完全不可变的内存数据库实现，通过源代码生成技术自动创建强类型的表和数据库类。框架采用 MessagePack 进行高效的二进制序列化，支持数据验证、索引查询、范围查找等丰富的数据库功能。MasterMemory 的设计理念是提供类型安全、性能优异、易于使用的内存数据库解决方案，特别适合游戏开发、配置数据管理、静态数据查询等场景。

# 设计模式使用指南：从实际需求出发，优先选择直接、高效、简洁的代码实现，避免过度设计。
# 忽略meta文件。
# 在生成代码时请记住注意如下规则：
 - 使用unity本地化工具，提供中英 语言转换，禁止生成meta文件。
 - 严格遵守unity 命名空间规范，不能以文件目录命名，命名空间最多不能超过三层。
 - 请随时了解unity 各个版本的API变更，并访问unity官方手册。
 - 请使用 unity 预处理命令支持unity更多版本，请访问unity 各个版本的API文档手册。
 - 请使用模块化设计模式，保持每个代码文件的 单一整洁性，功能模块代码清晰，提供面向开发者和用户的API封装扩展 
 - 请合理 构建代码文件和整理文件夹目录，代码分块 # range  # endregion。
 - 请以面向开发者和用户的角度设计代码，合理选用设计模式和技术栈的技术选择。做好代码功能-系统模块API封装，
 - 请规范使用对 类命名、方法名、属性名、字段名、 事件名、方法名，文件命名。 
 - 提高代码复用率 -功能模块扩展结合使用 - 避免重复造轮子。