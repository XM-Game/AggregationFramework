# AFramework.DI 代码开发目录结构

> 版本：1.0.0  
> 命名空间：AFramework.DI  
> 适用 Unity 版本：2022.3 LTS ~ Unity 6.x  
> 最后更新：2026年1月

---

## 目录

1. [目录结构总览](#1-目录结构总览)
2. [核心层 (Core)](#2-核心层-core)
3. [容器层 (Container)](#3-容器层-container)
4. [注入层 (Injection)](#4-注入层-injection)
5. [提供者层 (Providers)](#5-提供者层-providers)
6. [生命周期层 (Lifecycle)](#6-生命周期层-lifecycle)
7. [作用域层 (Scope)](#7-作用域层-scope)
8. [Unity 集成层 (Unity)](#8-unity-集成层-unity)
9. [入口点系统 (EntryPoints)](#9-入口点系统-entrypoints)
10. [诊断系统 (Diagnostics)](#10-诊断系统-diagnostics)
11. [扩展层 (Extensions)](#11-扩展层-extensions)
12. [编辑器层 (Editor)](#12-编辑器层-editor)
13. [程序集定义](#13-程序集定义)
14. [命名规范](#14-命名规范)

---

## 1. 目录结构总览

```
Plugins/
└── AFramework/
    └── DI/
        ├── Runtime/
        │   ├── AFramework.DI.asmdef                    # 运行时程序集定义
        │   │
        │   ├── Core/                                   # 核心层
        │   │   ├── Interfaces/                         # 核心接口定义
        │   │   │   ├── IObjectResolver.cs              # 对象解析器接口
        │   │   │   ├── IContainerBuilder.cs            # 容器构建器接口
        │   │   │   ├── IRegistration.cs                # 注册信息接口
        │   │   │   ├── IRegistrationBuilder.cs         # 注册构建器接口
        │   │   │   ├── IInstanceProvider.cs            # 实例提供者接口
        │   │   │   ├── IInjector.cs                    # 注入器接口
        │   │   │   └── IInjectParameter.cs             # 注入参数接口
        │   │   │
        │   │   ├── Attributes/                         # 特性定义
        │   │   │   ├── InjectAttribute.cs              # [Inject] 注入标记
        │   │   │   ├── KeyAttribute.cs                 # [Key] 键值注入
        │   │   │   ├── OptionalAttribute.cs            # [Optional] 可选依赖
        │   │   │   ├── FromParentAttribute.cs          # [FromParent] 父容器解析
        │   │   │   └── OrderAttribute.cs               # [Order] 执行顺序
        │   │   │
        │   │   ├── Enums/                              # 枚举定义
        │   │   │   ├── Lifetime.cs                     # 生命周期枚举
        │   │   │   └── InjectionType.cs                # 注入类型枚举
        │   │   │
        │   │   └── Exceptions/                         # 异常定义
        │   │       ├── DIException.cs                  # DI 基础异常
        │   │       ├── CircularDependencyException.cs  # 循环依赖异常
        │   │       ├── RegistrationException.cs        # 注册异常
        │   │       └── ResolutionException.cs          # 解析异常
        │   │
        │   ├── Container/                              # 容器层
        │   │   ├── Container.cs                        # 容器实现
        │   │   ├── ContainerBuilder.cs                 # 容器构建器
        │   │   ├── ContainerBuilderExtensions.cs       # 构建器扩展方法
        │   │   ├── Registry.cs                         # 注册表
        │   │   ├── Registration.cs                     # 注册信息
        │   │   └── RegistrationBuilder.cs              # 注册构建器
        │   │
        │   ├── Injection/                              # 注入层
        │   │   ├── Injector.cs                         # 注入器实现
        │   │   ├── InjectorCache.cs                    # 注入器缓存
        │   │   │
        │   │   ├── Parameters/                         # 注入参数
        │   │   │   ├── TypedParameter.cs               # 类型参数
        │   │   │   ├── NamedParameter.cs               # 命名参数
        │   │   │   └── FactoryParameter.cs             # 工厂参数
        │   │   │
        │   │   └── Internal/                           # 内部实现
        │   │       ├── ConstructorInjector.cs          # 构造函数注入器
        │   │       ├── MethodInjector.cs               # 方法注入器
        │   │       ├── PropertyInjector.cs             # 属性注入器
        │   │       ├── FieldInjector.cs                # 字段注入器
        │   │       └── InjectionInfoCache.cs           # 注入信息缓存
        │   │
        │   ├── Providers/                              # 提供者层
        │   │   ├── InstanceProvider.cs                 # 实例提供者基类
        │   │   ├── SingletonProvider.cs                # 单例提供者
        │   │   ├── ScopedProvider.cs                   # 作用域提供者
        │   │   ├── TransientProvider.cs                # 瞬态提供者
        │   │   ├── FactoryProvider.cs                  # 工厂提供者
        │   │   ├── ExistingInstanceProvider.cs         # 现有实例提供者
        │   │   └── ComponentProvider.cs                # Unity 组件提供者
        │   │
        │   ├── Lifecycle/                              # 生命周期层————————————————000——————————
        │   │   ├── LifetimeManager.cs                  # 生命周期管理器
        │   │   ├── DisposableTracker.cs                # 可释放对象追踪器
        │   │   └── LifetimeValidator.cs                # 生命周期验证器
        │   │
        │   ├── Scope/                                  # 作用域层
        │   │   ├── ScopeContainer.cs                   # 作用域容器
        │   │   ├── ScopeManager.cs                     # 作用域管理器
        │   │   └── ScopeInstanceCache.cs               # 作用域实例缓存
        │   │
        │   ├── Unity/                                  # Unity 集成层
        │   │   ├── LifetimeScope.cs                    # 生命周期作用域组件
        │   │   ├── LifetimeScopeExtensions.cs          # 作用域扩展方法
        │   │   │
        │   │   ├── ParentReference/                    # 父容器引用
        │   │   │   ├── ParentReference.cs              # 父引用基类
        │   │   │   ├── ParentReferenceAuto.cs          # 自动查找
        │   │   │   ├── ParentReferenceDirect.cs        # 直接引用
        │   │   │   └── ParentReferenceByType.cs        # 按类型查找
        │   │   │
        │   │   ├── Component/                          # 组件注册
        │   │   │   ├── ComponentRegistration.cs        # 组件注册信息
        │   │   │   ├── ComponentRegistrationBuilder.cs # 组件注册构建器
        │   │   │   └── GameObjectInjector.cs           # GameObject 注入器
        │   │   │
        │   │   └── Instantiation/                      # 实例化增强
        │   │       ├── InstantiateExtensions.cs        # 实例化扩展
        │   │       └── PrefabInjector.cs               # 预制体注入器
        │   │
        │   ├── EntryPoints/                            # 入口点系统
        │   │   ├── Interfaces/                         # 入口点接口
        │   │   │   ├── IInitializable.cs               # 初始化接口
        │   │   │   ├── IPostInitializable.cs           # 后初始化接口
        │   │   │   ├── IAsyncStartable.cs              # 异步启动接口
        │   │   │   ├── IStartable.cs                   # 启动接口
        │   │   │   ├── IPostStartable.cs               # 后启动接口
        │   │   │   ├── ITickable.cs                    # 更新接口
        │   │   │   ├── IPostTickable.cs                # 后更新接口
        │   │   │   ├── IFixedTickable.cs               # 固定更新接口
        │   │   │   ├── IPostFixedTickable.cs           # 后固定更新接口
        │   │   │   ├── ILateTickable.cs                # 延迟更新接口
        │   │   │   └── IPostLateTickable.cs            # 后延迟更新接口
        │   │   │
        │   │   ├── EntryPointDispatcher.cs             # 入口点调度器
        │   │   ├── EntryPointRegistry.cs               # 入口点注册表
        │   │   ├── EntryPointRunner.cs                 # 入口点运行器
        │   │   └── EntryPointExceptionHandler.cs       # 入口点异常处理
        │   │
        │   ├── Installers/                             # 安装器系统
        │   │   ├── IInstaller.cs                       # 安装器接口
        │   │   ├── InstallerBase.cs                    # 安装器基类
        │   │   ├── MonoInstaller.cs                    # MonoBehaviour 安装器
        │   │   └── ScriptableObjectInstaller.cs        # ScriptableObject 安装器
        │   │
        │   ├── Diagnostics/                            # 诊断系统
        │   │   ├── DiagnosticsCollector.cs             # 诊断信息收集器
        │   │   ├── DiagnosticsInfo.cs                  # 诊断信息数据
        │   │   ├── DependencyGraphBuilder.cs           # 依赖图构建器
        │   │   └── LifetimeValidationReport.cs         # 生命周期验证报告
        │   │
        │   ├── Extensions/                             # 扩展层
        │   │   ├── Lazy/                               # 延迟解析
        │   │   │   ├── LazyResolver.cs                 # Lazy<T> 解析器
        │   │   │   └── LazyRegistrationExtensions.cs   # Lazy 注册扩展
        │   │   │
        │   │   ├── Factory/                            # 工厂扩展
        │   │   │   ├── IFactory.cs                     # 工厂接口
        │   │   │   ├── Factory.cs                      # 工厂实现
        │   │   │   └── FactoryRegistrationExtensions.cs# 工厂注册扩展
        │   │   │
        │   │   ├── Collection/                         # 集合解析
        │   │   │   ├── CollectionResolver.cs           # 集合解析器
        │   │   │   └── CollectionRegistrationExtensions.cs # 集合注册扩展
        │   │   │
        │   │   └── Keyed/                              # 键值注入
        │   │       ├── KeyedResolver.cs                # 键值解析器
        │   │       └── KeyedRegistrationExtensions.cs  # 键值注册扩展
        │   │
        │   ├── Integration/                            # 模块集成————————————————————
        │   │   ├── Pool/                               # 对象池集成
        │   │   │   ├── PooledProvider.cs               # 池化提供者
        │   │   │   └── PoolRegistrationExtensions.cs   # 池注册扩展
        │   │   │
        │   │   ├── EventSystem/                        # 事件系统集成
        │   │   │   ├── EventBusRegistration.cs         # 事件总线注册
        │   │   │   └── EventHandlerAutoSubscriber.cs   # 事件处理器自动订阅
        │   │   │
        │   │   ├── ECS/                                # ECS 集成
        │   │   │   ├── WorldRegistration.cs            # World 注册
        │   │   │   ├── SystemRegistration.cs           # System 注册
        │   │   │   └── ECSRegistrationExtensions.cs    # ECS 注册扩展
        │   │   │
        │   │   └── UniTask/                            # UniTask 集成
        │   │       ├── AsyncStartableRunner.cs         # 异步启动运行器
        │   │       └── UniTaskExtensions.cs            # UniTask 扩展
        │   │
        │   └── Internal/                               # 内部工具
        │       ├── TypeHelper.cs                       # 类型辅助工具
        │       ├── ReflectionHelper.cs                 # 反射辅助工具
        │       ├── ReflectionCache.cs                  # 反射缓存
        │       ├── ConcurrentCache.cs                  # 并发缓存
        │       └── ThrowHelper.cs                      # 异常抛出辅助
        │
        └── Editor/
            ├── AFramework.DI.Editor.asmdef             # 编辑器程序集定义
            │
            ├── Windows/                                # 编辑器窗口
            │   ├── DIDebugWindow.cs                    # DI 调试窗口
            │   ├── DependencyGraphWindow.cs            # 依赖图窗口
            │   └── RegistrationListWindow.cs           # 注册列表窗口
            │
            ├── Inspectors/                             # 自定义检视器
            │   ├── LifetimeScopeEditor.cs              # LifetimeScope 检视器
            │   ├── MonoInstallerEditor.cs              # MonoInstaller 检视器
            │   └── ParentReferenceDrawer.cs            # 父引用属性绘制器
            │
            ├── Diagnostics/                            # 编辑器诊断
            │   ├── EditorDiagnosticsPanel.cs           # 编辑器诊断面板
            │   ├── DependencyGraphRenderer.cs          # 依赖图渲染器
            │   └── LifetimeValidationWindow.cs         # 生命周期验证窗口
            │
            ├── Settings/                               # 设置
            │   ├── DISettings.cs                       # DI 设置
            │   └── DISettingsProvider.cs               # 设置提供者
            │
            ├── CodeGen/                                # 代码生成
            │   ├── InjectorCodeGenerator.cs            # 注入器代码生成
            │   └── InstallerTemplateGenerator.cs       # 安装器模板生成
            │
            ├── Localization/                           # 本地化
            │   ├── DIEditorLocalization.cs             # 编辑器本地化
            │   └── Resources/
            │       ├── DI_en.json                      # 英文语言包
            │       └── DI_zh-CN.json                   # 中文语言包
            │
            └── Internal/                               # 内部工具
                ├── EditorIcons.cs                      # 编辑器图标
                ├── EditorStyles.cs                     # 编辑器样式
                └── EditorGUIHelper.cs                  # GUI 辅助工具
```

---

## 2. 核心层 (Core)

核心层定义了框架的基础接口、特性和异常，是整个 DI 系统的基石。

### 2.1 接口定义 (Interfaces/)

| 文件名 | 职责 | 关键成员 |
|--------|------|----------|
| `IObjectResolver.cs` | 对象解析器接口 | `Resolve<T>()`, `ResolveOrDefault<T>()`, `ResolveAll<T>()`, `TryResolve<T>()`, `Inject()`, `Instantiate()` |
| `IContainerBuilder.cs` | 容器构建器接口 | `Register<T>()`, `RegisterInstance<T>()`, `RegisterFactory<T>()`, `UseInstaller<T>()`, `Build()` |
| `IRegistration.cs` | 注册信息接口 | `ServiceType`, `ImplementationType`, `Lifetime`, `Key`, `Provider` |
| `IRegistrationBuilder.cs` | 注册构建器接口 | `As<T>()`, `AsSelf()`, `AsImplementedInterfaces()`, `Singleton()`, `Scoped()`, `Transient()`, `WithParameter()`, `Keyed()` |
| `IInstanceProvider.cs` | 实例提供者接口 | `GetInstance()`, `DisposeInstance()` |
| `IInjector.cs` | 注入器接口 | `Inject()`, `CreateInstance()` |
| `IInjectParameter.cs` | 注入参数接口 | `ParameterType`, `ParameterName`, `GetValue()` |

### 2.2 特性定义 (Attributes/)

| 文件名 | 特性名 | 用途 | 适用目标 |
|--------|--------|------|----------|
| `InjectAttribute.cs` | `[Inject]` | 标记需要注入的成员 | 构造函数、方法、属性、字段 |
| `KeyAttribute.cs` | `[Key]` | 指定键值注入 | 参数、属性、字段 |
| `OptionalAttribute.cs` | `[Optional]` | 标记可选依赖 | 参数、属性、字段 |
| `FromParentAttribute.cs` | `[FromParent]` | 从父容器解析 | 参数、属性、字段 |
| `OrderAttribute.cs` | `[Order]` | 指定执行顺序 | 入口点类 |

### 2.3 枚举定义 (Enums/)

| 文件名 | 枚举名 | 成员 |
|--------|--------|------|
| `Lifetime.cs` | `Lifetime` | `Singleton`, `Scoped`, `Transient` |
| `InjectionType.cs` | `InjectionType` | `Constructor`, `Method`, `Property`, `Field` |

### 2.4 异常定义 (Exceptions/)

| 文件名 | 异常类 | 触发场景 |
|--------|--------|----------|
| `DIException.cs` | `DIException` | DI 系统基础异常 |
| `CircularDependencyException.cs` | `CircularDependencyException` | 检测到循环依赖 |
| `RegistrationException.cs` | `RegistrationException` | 注册配置错误 |
| `ResolutionException.cs` | `ResolutionException` | 解析失败 |

---

## 3. 容器层 (Container)

容器层实现了依赖注入容器的核心功能，包括服务注册、构建和管理。

| 文件名 | 职责 | 关键功能 |
|--------|------|----------|
| `Container.cs` | 容器实现 | 服务解析、作用域创建、实例缓存管理 |
| `ContainerBuilder.cs` | 容器构建器 | 收集注册信息、验证配置、构建容器 |
| `ContainerBuilderExtensions.cs` | 构建器扩展 | 便捷注册方法、批量注册、条件注册 |
| `Registry.cs` | 注册表 | 存储注册信息、类型映射、键值索引 |
| `Registration.cs` | 注册信息 | 服务类型、实现类型、生命周期、提供者 |
| `RegistrationBuilder.cs` | 注册构建器 | 链式配置 API、生命周期设置、参数配置 |

### 3.1 Container.cs 核心结构

```
Container
├── 字段
│   ├── _registry          : Registry           # 注册表
│   ├── _parent            : Container          # 父容器
│   ├── _singletonCache    : ConcurrentDict     # 单例缓存
│   ├── _scopedCache       : ConcurrentDict     # 作用域缓存
│   └── _disposables       : List<IDisposable>  # 可释放对象列表
│
├── 解析方法
│   ├── Resolve<T>()                            # 必需解析
│   ├── ResolveOrDefault<T>()                   # 可选解析
│   ├── ResolveAll<T>()                         # 集合解析
│   ├── TryResolve<T>()                         # 尝试解析
│   └── ResolveKeyed<T>()                       # 键值解析
│
├── 注入方法
│   ├── Inject()                                # 注入现有实例
│   └── Instantiate()                           # 实例化并注入
│
├── 作用域方法
│   ├── CreateScope()                           # 创建子作用域
│   └── BeginScope()                            # 开始作用域
│
└── 生命周期
    └── Dispose()                               # 释放资源
```

---

## 4. 注入层 (Injection)

注入层负责依赖的注入逻辑，支持多种注入方式。

| 文件名 | 职责 | 关键功能 |
|--------|------|----------|
| `Injector.cs` | 注入器实现 | 协调各类型注入器、执行注入流程 |
| `InjectorCache.cs` | 注入器缓存 | 缓存注入器实例、提升性能 |

### 4.1 注入参数 (Parameters/)

| 文件名 | 类名 | 用途 |
|--------|------|------|
| `TypedParameter.cs` | `TypedParameter` | 按类型匹配参数 |
| `NamedParameter.cs` | `NamedParameter` | 按名称匹配参数 |
| `FactoryParameter.cs` | `FactoryParameter` | 工厂函数参数 |

### 4.2 内部注入器 (Internal/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `ConstructorInjector.cs` | `ConstructorInjector` | 构造函数注入实现 |
| `MethodInjector.cs` | `MethodInjector` | 方法注入实现 |
| `PropertyInjector.cs` | `PropertyInjector` | 属性注入实现 |
| `FieldInjector.cs` | `FieldInjector` | 字段注入实现 |
| `InjectionInfoCache.cs` | `InjectionInfoCache` | 注入元数据缓存 |

---

## 5. 提供者层 (Providers)

提供者层实现了不同生命周期的实例创建和管理策略。

| 文件名 | 类名 | 生命周期 | 特点 |
|--------|------|----------|------|
| `InstanceProvider.cs` | `InstanceProvider` | - | 提供者基类 |
| `SingletonProvider.cs` | `SingletonProvider` | Singleton | 容器级单例，线程安全 |
| `ScopedProvider.cs` | `ScopedProvider` | Scoped | 作用域级单例 |
| `TransientProvider.cs` | `TransientProvider` | Transient | 每次创建新实例 |
| `FactoryProvider.cs` | `FactoryProvider` | - | 工厂函数创建 |
| `ExistingInstanceProvider.cs` | `ExistingInstanceProvider` | Singleton | 已存在实例 |
| `ComponentProvider.cs` | `ComponentProvider` | - | Unity 组件提供 |

---

## 6. 生命周期层 (Lifecycle)

生命周期层管理对象的创建、缓存和销毁。

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `LifetimeManager.cs` | `LifetimeManager` | 管理对象生命周期、协调提供者 |
| `DisposableTracker.cs` | `DisposableTracker` | 追踪 IDisposable 对象、确保正确释放 |
| `LifetimeValidator.cs` | `LifetimeValidator` | 验证生命周期配置、检测作用域捕获 |

---

## 7. 作用域层 (Scope)

作用域层实现了容器的层次结构和作用域隔离。

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `ScopeContainer.cs` | `ScopeContainer` | 作用域容器实现、继承父容器注册 |
| `ScopeManager.cs` | `ScopeManager` | 管理作用域层次、查找父作用域 |
| `ScopeInstanceCache.cs` | `ScopeInstanceCache` | 作用域级实例缓存 |

---

## 8. Unity 集成层 (Unity)

Unity 集成层提供与 Unity 引擎的深度集成。

### 8.1 核心组件

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `LifetimeScope.cs` | `LifetimeScope` | 场景容器组件、Unity 生命周期集成 |
| `LifetimeScopeExtensions.cs` | - | LifetimeScope 扩展方法 |

### 8.2 父容器引用 (ParentReference/)

| 文件名 | 类名 | 查找方式 |
|--------|------|----------|
| `ParentReference.cs` | `ParentReference` | 父引用基类 |
| `ParentReferenceAuto.cs` | `ParentReferenceAuto` | 自动向上查找 |
| `ParentReferenceDirect.cs` | `ParentReferenceDirect` | 直接引用 |
| `ParentReferenceByType.cs` | `ParentReferenceByType<T>` | 按类型查找 |

### 8.3 组件注册 (Component/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `ComponentRegistration.cs` | `ComponentRegistration` | 组件注册信息 |
| `ComponentRegistrationBuilder.cs` | `ComponentRegistrationBuilder` | 组件注册构建器 |
| `GameObjectInjector.cs` | `GameObjectInjector` | GameObject 层级注入 |

### 8.4 实例化增强 (Instantiation/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `InstantiateExtensions.cs` | - | Instantiate 扩展方法 |
| `PrefabInjector.cs` | `PrefabInjector` | 预制体实例化注入 |

---

## 9. 入口点系统 (EntryPoints)

入口点系统提供 Unity 生命周期的自动回调机制。

### 9.1 入口点接口 (Interfaces/)

| 文件名 | 接口名 | 调用时机 |
|--------|--------|----------|
| `IInitializable.cs` | `IInitializable` | 容器构建后立即调用 |
| `IPostInitializable.cs` | `IPostInitializable` | 所有 IInitializable 后调用 |
| `IAsyncStartable.cs` | `IAsyncStartable` | 异步启动（支持 UniTask） |
| `IStartable.cs` | `IStartable` | Unity Start 时调用 |
| `IPostStartable.cs` | `IPostStartable` | 所有 IStartable 后调用 |
| `ITickable.cs` | `ITickable` | 每帧 Update 时调用 |
| `IPostTickable.cs` | `IPostTickable` | 所有 ITickable 后调用 |
| `IFixedTickable.cs` | `IFixedTickable` | FixedUpdate 时调用 |
| `IPostFixedTickable.cs` | `IPostFixedTickable` | 所有 IFixedTickable 后调用 |
| `ILateTickable.cs` | `ILateTickable` | LateUpdate 时调用 |
| `IPostLateTickable.cs` | `IPostLateTickable` | 所有 ILateTickable 后调用 |

### 9.2 入口点管理

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `EntryPointDispatcher.cs` | `EntryPointDispatcher` | 调度入口点执行 |
| `EntryPointRegistry.cs` | `EntryPointRegistry` | 注册入口点实例 |
| `EntryPointRunner.cs` | `EntryPointRunner` | 运行入口点（MonoBehaviour） |
| `EntryPointExceptionHandler.cs` | `EntryPointExceptionHandler` | 入口点异常处理 |

---

## 10. 诊断系统 (Diagnostics)

诊断系统提供运行时和编辑器的调试支持。

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `DiagnosticsCollector.cs` | `DiagnosticsCollector` | 收集诊断信息 |
| `DiagnosticsInfo.cs` | `DiagnosticsInfo` | 诊断信息数据结构 |
| `DependencyGraphBuilder.cs` | `DependencyGraphBuilder` | 构建依赖关系图 |
| `LifetimeValidationReport.cs` | `LifetimeValidationReport` | 生命周期验证报告 |

---

## 11. 扩展层 (Extensions)

扩展层提供高级功能和便捷 API。

### 11.1 延迟解析 (Lazy/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `LazyResolver.cs` | `LazyResolver<T>` | Lazy<T> 解析支持 |
| `LazyRegistrationExtensions.cs` | - | Lazy 注册扩展方法 |

### 11.2 工厂扩展 (Factory/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `IFactory.cs` | `IFactory<T>` | 工厂接口 |
| `Factory.cs` | `Factory<T>` | 工厂实现 |
| `FactoryRegistrationExtensions.cs` | - | 工厂注册扩展方法 |

### 11.3 集合解析 (Collection/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `CollectionResolver.cs` | `CollectionResolver` | IEnumerable<T> 解析 |
| `CollectionRegistrationExtensions.cs` | - | 集合注册扩展方法 |

### 11.4 键值注入 (Keyed/)

| 文件名 | 类名 | 职责 |
|--------|------|------|
| `KeyedResolver.cs` | `KeyedResolver` | 键值解析器 |
| `KeyedRegistrationExtensions.cs` | - | 键值注册扩展方法 |

---

## 12. 编辑器层 (Editor)

编辑器层提供 Unity 编辑器的可视化工具和调试支持。

### 12.1 编辑器窗口 (Windows/)

| 文件名 | 窗口名 | 功能 |
|--------|--------|------|
| `DIDebugWindow.cs` | DI Debug | 运行时容器状态查看 |
| `DependencyGraphWindow.cs` | Dependency Graph | 依赖关系可视化 |
| `RegistrationListWindow.cs` | Registration List | 注册列表查看 |

### 12.2 自定义检视器 (Inspectors/)

| 文件名 | 目标类型 | 功能 |
|--------|----------|------|
| `LifetimeScopeEditor.cs` | `LifetimeScope` | 作用域配置检视器 |
| `MonoInstallerEditor.cs` | `MonoInstaller` | 安装器检视器 |
| `ParentReferenceDrawer.cs` | `ParentReference` | 父引用属性绘制 |

### 12.3 诊断工具 (Diagnostics/)

| 文件名 | 类名 | 功能 |
|--------|------|------|
| `EditorDiagnosticsPanel.cs` | `EditorDiagnosticsPanel` | 编辑器诊断面板 |
| `DependencyGraphRenderer.cs` | `DependencyGraphRenderer` | 依赖图渲染 |
| `LifetimeValidationWindow.cs` | `LifetimeValidationWindow` | 生命周期验证窗口 |

### 12.4 设置 (Settings/)

| 文件名 | 类名 | 功能 |
|--------|------|------|
| `DISettings.cs` | `DISettings` | DI 设置数据 |
| `DISettingsProvider.cs` | `DISettingsProvider` | Project Settings 集成 |

### 12.5 代码生成 (CodeGen/)

| 文件名 | 类名 | 功能 |
|--------|------|------|
| `InjectorCodeGenerator.cs` | `InjectorCodeGenerator` | 注入器代码生成 |
| `InstallerTemplateGenerator.cs` | `InstallerTemplateGenerator` | 安装器模板生成 |

### 12.6 本地化 (Localization/)

| 文件名 | 功能 |
|--------|------|
| `DIEditorLocalization.cs` | 编辑器本地化管理 |
| `Resources/DI_en.json` | 英文语言包 |
| `Resources/DI_zh-CN.json` | 中文语言包 |

---

## 13. 程序集定义

### 13.1 运行时程序集 (AFramework.DI.asmdef)

```json
{
    "name": "AFramework.DI",
    "rootNamespace": "AFramework.DI",
    "references": [
        "AFramework.Core",
        "AFramework.CSharpExtension",
        "AFramework.UnityExtension",
        "UniTask"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [
        {
            "name": "com.cysharp.unitask",
            "expression": "2.0.0",
            "define": "AFRAMEWORK_UNITASK"
        },
        {
            "name": "Unity",
            "expression": "2022.3",
            "define": "UNITY_2022_3_OR_NEWER"
        },
        {
            "name": "Unity",
            "expression": "6000.0",
            "define": "UNITY_6_OR_NEWER"
        }
    ],
    "noEngineReferences": false
}
```

### 13.2 编辑器程序集 (AFramework.DI.Editor.asmdef)

```json
{
    "name": "AFramework.DI.Editor",
    "rootNamespace": "AFramework.DI.Editor",
    "references": [
        "AFramework.DI",
        "AFramework.Core",
        "AFramework.Core.Editor"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

---

## 14. 命名规范

### 14.1 命名空间规范

| 层级 | 命名空间 |
|------|----------|
| 核心层 | `AFramework.DI` |
| 容器层 | `AFramework.DI` |
| 注入层 | `AFramework.DI.Injection` |
| 提供者层 | `AFramework.DI.Providers` |
| 生命周期层 | `AFramework.DI.Lifecycle` |
| 作用域层 | `AFramework.DI.Scope` |
| Unity 集成 | `AFramework.DI.Unity` |
| 入口点系统 | `AFramework.DI.EntryPoints` |
| 诊断系统 | `AFramework.DI.Diagnostics` |
| 扩展层 | `AFramework.DI.Extensions` |
| 模块集成 | `AFramework.DI.Integration` |
| 编辑器 | `AFramework.DI.Editor` |

### 14.2 类命名规范

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| 接口 | `I` + 名词/形容词 | `IObjectResolver`, `IInitializable` |
| 抽象类 | 名词 + `Base` | `InstallerBase`, `InstanceProvider` |
| 实现类 | 描述性名词 | `Container`, `SingletonProvider` |
| 特性 | 名词 + `Attribute` | `InjectAttribute`, `KeyAttribute` |
| 异常 | 描述 + `Exception` | `CircularDependencyException` |
| 扩展方法类 | 目标类型 + `Extensions` | `ContainerBuilderExtensions` |
| 编辑器窗口 | 功能 + `Window` | `DIDebugWindow` |
| 检视器 | 目标类型 + `Editor` | `LifetimeScopeEditor` |

### 14.3 方法命名规范

| 方法类型 | 命名规则 | 示例 |
|----------|----------|------|
| 获取方法 | `Get` + 名词 | `GetInstance()`, `GetRegistration()` |
| 解析方法 | `Resolve` + 修饰 | `Resolve<T>()`, `ResolveAll<T>()` |
| 注册方法 | `Register` + 修饰 | `Register<T>()`, `RegisterInstance()` |
| 尝试方法 | `Try` + 动词 | `TryResolve<T>()`, `TryGetValue()` |
| 创建方法 | `Create` + 名词 | `CreateScope()`, `CreateInstance()` |
| 异步方法 | 动词 + `Async` | `StartAsync()`, `InitializeAsync()` |

### 14.4 文件头规范

```csharp
// ==========================================================
// 文件名：{FileName}.cs
// 命名空间: AFramework.DI.{SubNamespace}
// 依赖: {Dependencies}
// 功能: {Brief Description}
// 作者: AFramework Team
// 创建日期: {Date}
// ==========================================================
```

---

## 附录 A：文件数量统计

| 目录 | 文件数量 | 说明 |
|------|----------|------|
| Core/ | 15 | 核心接口、特性、枚举、异常 |
| Container/ | 6 | 容器实现 |
| Injection/ | 9 | 注入系统 |
| Providers/ | 7 | 实例提供者 |
| Lifecycle/ | 3 | 生命周期管理 |
| Scope/ | 3 | 作用域系统 |
| Unity/ | 10 | Unity 集成 |
| EntryPoints/ | 15 | 入口点系统 |
| Installers/ | 4 | 安装器系统 |
| Diagnostics/ | 4 | 诊断系统 |
| Extensions/ | 8 | 扩展功能 |
| Integration/ | 8 | 模块集成 |
| Internal/ | 5 | 内部工具 |
| Editor/ | 15 | 编辑器工具 |
| **总计** | **~112** | 运行时 + 编辑器 |

---

## 附录 B：依赖关系图

```
┌─────────────────────────────────────────────────────────────┐
│                    AFramework.DI 依赖关系                    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│                    ┌─────────────┐                          │
│                    │    Core     │                          │
│                    │ (Interfaces)│                          │
│                    └──────┬──────┘                          │
│                           │                                 │
│           ┌───────────────┼───────────────┐                 │
│           ▼               ▼               ▼                 │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │  Container  │ │  Injection  │ │  Providers  │           │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘           │
│         │               │               │                   │
│         └───────────────┼───────────────┘                   │
│                         ▼                                   │
│                ┌─────────────────┐                          │
│                │    Lifecycle    │                          │
│                │      Scope      │                          │
│                └────────┬────────┘                          │
│                         │                                   │
│                         ▼                                   │
│                ┌─────────────────┐                          │
│                │      Unity      │                          │
│                │   EntryPoints   │                          │
│                └────────┬────────┘                          │
│                         │                                   │
│           ┌─────────────┼─────────────┐                     │
│           ▼             ▼             ▼                     │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │ Extensions  │ │ Integration │ │ Diagnostics │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

> 文档结束
> 
> AFramework.DI 代码开发目录结构
> 
> 版权所有 © 2026 AFramework Team
