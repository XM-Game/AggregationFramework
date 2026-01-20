# AFramework.Pool 对象池系统 - 代码目录结构

> **版本**: 1.0.0  
> **更新日期**: 2026-01-19  
> **适用 Unity 版本**: 2022.3 LTS ~ Unity 6.x

---

## 📋 目录概览

```
Assets/Plugins/AFramework/Pool/
├── Runtime/                                    # 运行时核心代码
│   ├── Core/                                   # 核心接口与抽象
│   │   ├── Interfaces/                         # 接口定义
│   │   ├── Abstracts/                          # 抽象基类
│   │   ├── Enums/                              # 枚举定义
│   │   └── Exceptions/                         # 异常类型
│   ├── Pools/                                  # 对象池实现
│   │   ├── Generic/                            # 泛型对象池
│   │   ├── Unity/                              # Unity 专用池
│   │   └── Specialized/                        # 特化池实现
│   ├── Policies/                               # 池策略
│   │   ├── Creation/                           # 创建策略
│   │   ├── Cleanup/                            # 清理策略
│   │   └── Capacity/                           # 容量策略
│   ├── Tracking/                               # 追踪与诊断
│   │   ├── Statistics/                         # 统计信息
│   │   └── Diagnostics/                        # 诊断工具
│   ├── Warming/                                # 预热系统
│   ├── DI/                                     # 依赖注入集成
│   │   ├── Registration/                       # 注册扩展
│   │   ├── Installers/                         # 安装器
│   │   └── Resolution/                         # 解析适配
│   ├── Extensions/                             # 扩展方法
│   └── Utilities/                              # 工具类
├── Editor/                                     # 编辑器工具
│   ├── Inspectors/                             # 自定义检视面板
│   ├── Windows/                                # 编辑器窗口
│   ├── Debuggers/                              # 调试工具
│   └── Utilities/                              # 编辑器工具类
├── Tests/                                      # 单元测试
│   ├── Runtime/                                # 运行时测试
│   └── Editor/                                 # 编辑器测试
└── Samples~/                                   # 示例代码
    ├── BasicUsage/                             # 基础用法
    ├── UnityIntegration/                       # Unity 集成
    └── AdvancedScenarios/                      # 高级场景
```

---

## 📦 一、Runtime 核心模块

### 1.1 Core - 核心接口与抽象

#### **Interfaces/ - 接口定义**

```
IObjectPool.cs                    # 对象池核心接口
IObjectPool<T>.cs                 # 泛型对象池接口
IPooledObject.cs                  # 池化对象接口（生命周期回调）
IPoolPolicy.cs                    # 池策略接口
IPoolCreationPolicy<T>.cs         # 创建策略接口
IPoolCleanupPolicy<T>.cs          # 清理策略接口
IPoolCapacityPolicy.cs            # 容量策略接口
IPoolStatistics.cs                # 统计信息接口
IPoolDiagnostics.cs               # 诊断接口
IPoolWarmer.cs                    # 预热接口
```

**设计要点**:
- 遵循接口隔离原则（ISP），接口职责单一
- 支持协变/逆变（`IObjectPool<out T>`）
- 提供同步和异步版本（`Get/GetAsync`）

#### **Abstracts/ - 抽象基类**

```
ObjectPoolBase.cs                 # 对象池抽象基类
ObjectPoolBase<T>.cs              # 泛型对象池抽象基类
PoolPolicyBase<T>.cs              # 池策略抽象基类
PooledObjectBase.cs               # 池化对象抽象基类
```

**设计要点**:
- 实现模板方法模式，定义池操作骨架
- 提供默认实现，减少子类重复代码
- 支持生命周期钩子（OnGet/OnReturn/OnDestroy）

#### **Enums/ - 枚举定义**

```
PoolCapacityMode.cs               # 容量模式（固定/动态/无限）
PoolCleanupMode.cs                # 清理模式（手动/自动/定时）
PoolCreationMode.cs               # 创建模式（懒加载/预热/按需）
PoolState.cs                      # 池状态（未初始化/运行中/已销毁）
PoolWarningLevel.cs               # 警告级别（无/低/中/高）
```

#### **Exceptions/ - 异常类型**

```
PoolException.cs                  # 池异常基类
PoolCapacityExceededException.cs  # 容量超限异常
PoolDisposedException.cs          # 池已销毁异常
PoolCreationException.cs          # 创建失败异常
PoolReturnException.cs            # 归还失败异常
```

---

### 1.2 Pools - 对象池实现

#### **Generic/ - 泛型对象池**

```
ObjectPool<T>.cs                  # 标准泛型对象池
ConcurrentObjectPool<T>.cs        # 线程安全对象池
StackObjectPool<T>.cs             # 基于栈的对象池（高性能）
QueueObjectPool<T>.cs             # 基于队列的对象池（FIFO）
BoundedObjectPool<T>.cs           # 有界对象池（固定容量）
DynamicObjectPool<T>.cs           # 动态扩容对象池
```

**设计要点**:
- `ObjectPool<T>`: 默认实现，平衡性能与功能
- `ConcurrentObjectPool<T>`: 使用 `ConcurrentBag<T>` 实现线程安全
- `StackObjectPool<T>`: 零 GC 分配，适合热路径
- `BoundedObjectPool<T>`: 严格容量控制，防止内存泄漏

#### **Unity/ - Unity 专用池**————————————

```
GameObjectPool.cs                 # GameObject 对象池
ComponentPool<T>.cs               # 组件对象池（T : Component）
PrefabPool.cs                     # Prefab 实例池
ParticleSystemPool.cs             # 粒子系统池（自动回收）
AudioSourcePool.cs                # 音频源池
CanvasPool.cs                     # UI Canvas 池
TransformPool.cs                  # Transform 池（用于层级管理）
```

**设计要点**:
- 集成 Unity 生命周期（`OnEnable/OnDisable`）
- 支持场景切换自动清理（`DontDestroyOnLoad`）
- 提供 Addressables 集成（异步加载）
- 自动处理父子关系（`SetParent`）

#### **Specialized/ - 特化池实现**

```
StringBuilderPool.cs              # StringBuilder 池
ArrayPool<T>.cs                   # 数组池（租借模式）
ListPool<T>.cs                    # List<T> 池
DictionaryPool<TKey, TValue>.cs   # Dictionary 池
HashSetPool<T>.cs                 # HashSet 池
MemoryPool<T>.cs                  # Memory<T> 池（Span 支持）
```

**设计要点**:
- 针对常用数据结构优化
- 支持租借模式（Rent/Return）
- 自动清理内部状态（Clear）

---

### 1.3 Policies - 池策略

#### **Creation/ - 创建策略**

```
IPoolCreationPolicy<T>.cs         # 创建策略接口
DefaultCreationPolicy<T>.cs       # 默认创建策略（new T()）
FactoryCreationPolicy<T>.cs       # 工厂创建策略
ActivatorCreationPolicy<T>.cs     # Activator 创建策略
UnityInstantiatePolicy.cs         # Unity Instantiate 策略
AddressableCreationPolicy.cs      # Addressables 异步创建策略
```

**设计要点**:
- 支持无参构造、工厂方法、依赖注入
- Unity 策略支持 Prefab 实例化
- 异步策略返回 `UniTask<T>`

#### **Cleanup/ - 清理策略**

```
IPoolCleanupPolicy<T>.cs          # 清理策略接口
DefaultCleanupPolicy<T>.cs        # 默认清理策略（无操作）
ResetCleanupPolicy<T>.cs          # 重置清理策略（调用 Reset）
DisposeCleanupPolicy<T>.cs        # 销毁清理策略（调用 Dispose）
UnityDestroyPolicy.cs             # Unity Destroy 策略
TimedCleanupPolicy<T>.cs          # 定时清理策略
IdleCleanupPolicy<T>.cs           # 空闲清理策略（LRU）
```

**设计要点**:
- 支持 `IDisposable` 自动调用
- Unity 策略调用 `Object.Destroy`
- 定时清理使用 `UniTask.Delay`

#### **Capacity/ - 容量策略**

```
IPoolCapacityPolicy.cs            # 容量策略接口
FixedCapacityPolicy.cs            # 固定容量策略
DynamicCapacityPolicy.cs          # 动态扩容策略
UnboundedCapacityPolicy.cs        # 无界容量策略
ThresholdCapacityPolicy.cs        # 阈值容量策略
```

**设计要点**:
- 固定容量：超出时抛异常或阻塞
- 动态扩容：按倍数增长（1.5x/2x）
- 阈值策略：达到阈值时触发清理

---

### 1.4 Tracking - 追踪与诊断

#### **Statistics/ - 统计信息**

```
PoolStatistics.cs                 # 池统计信息
PoolMetrics.cs                    # 池性能指标
PoolUsageSnapshot.cs              # 池使用快照
StatisticsCollector.cs            # 统计收集器
```

**统计指标**:
- 总创建数 / 总销毁数
- 当前活跃数 / 当前空闲数
- 命中率 / 未命中率
- 平均获取时间 / 平均归还时间
- 峰值使用量 / 内存占用

#### **Diagnostics/ - 诊断工具**

```
PoolDiagnostics.cs                # 池诊断工具
PoolHealthChecker.cs              # 池健康检查
PoolLeakDetector.cs               # 泄漏检测器
PoolPerformanceProfiler.cs        # 性能分析器
```

**诊断功能**:
- 泄漏检测（未归还对象追踪）
- 性能瓶颈分析
- 容量预警
- 异常使用模式检测

---

### 1.5 Warming - 预热系统

```
IPoolWarmer.cs                    # 预热接口
PoolWarmer.cs                     # 预热实现
WarmupStrategy.cs                 # 预热策略
AsyncPoolWarmer.cs                # 异步预热器
ScenePoolWarmer.cs                # 场景预热器（Unity）
```

**预热策略**:
- 立即预热：启动时同步创建
- 延迟预热：首次使用前创建
- 分帧预热：分散到多帧避免卡顿
- 异步预热：后台线程创建

---

### 1.6 DI - 依赖注入集成

#### **Registration/ - 注册扩展**

```
PoolRegistrationExtensions.cs     # 池注册扩展方法
PoolServiceCollectionExtensions.cs # ServiceCollection 扩展
```

**扩展方法**:
```csharp
// 注册对象池
builder.RegisterObjectPool<Enemy>(
    creationPolicy: new FactoryCreationPolicy<Enemy>(...),
    capacity: 100,
    warmup: true
);

// 注册 GameObject 池
builder.RegisterGameObjectPool(
    prefab: enemyPrefab,
    capacity: 50,
    parent: poolRoot
);
```

#### **Installers/ - 安装器**

```
PoolInstaller.cs                  # 池安装器基类
GameObjectPoolInstaller.cs        # GameObject 池安装器
ComponentPoolInstaller.cs         # 组件池安装器
```

#### **Resolution/ - 解析适配**

```
PoolResolver.cs                   # 池解析器
PoolFactory.cs                    # 池工厂
ScopedPoolManager.cs              # 作用域池管理器
```

---

### 1.7 Extensions - 扩展方法

```
ObjectPoolExtensions.cs           # 对象池扩展方法
GameObjectPoolExtensions.cs       # GameObject 池扩展
ComponentPoolExtensions.cs        # 组件池扩展
CollectionPoolExtensions.cs       # 集合池扩展
```

**扩展方法示例**:
```csharp
// 使用 using 自动归还
using (var obj = pool.Rent())
{
    // 使用对象
}

// 批量获取
var enemies = pool.GetMany(10);

// 异步获取
var enemy = await pool.GetAsync();
```

---

### 1.8 Utilities - 工具类

```
PoolHelper.cs                     # 池辅助工具
PoolValidator.cs                  # 池验证器
PoolDebugger.cs                   # 池调试器
PoolSerializer.cs                 # 池序列化器（保存/加载状态）
```

---

## 🎨 二、Editor 编辑器模块————————————————————————————

### 2.1 Inspectors - 自定义检视面板

```
ObjectPoolInspector.cs            # 对象池检视面板
GameObjectPoolInspector.cs        # GameObject 池检视面板
PoolStatisticsDrawer.cs           # 统计信息绘制器
```

**功能**:
- 实时显示池状态
- 可视化统计图表
- 手动触发预热/清理
- 泄漏检测按钮

### 2.2 Windows - 编辑器窗口

```
PoolManagerWindow.cs              # 池管理器窗口
PoolProfilerWindow.cs             # 池性能分析窗口
PoolDebuggerWindow.cs             # 池调试窗口
```

**功能**:
- 全局池列表
- 实时性能监控
- 内存占用分析
- 泄漏对象追踪

### 2.3 Debuggers - 调试工具

```
PoolVisualDebugger.cs             # 可视化调试器
PoolLeakVisualizer.cs             # 泄漏可视化
PoolHierarchyDebugger.cs          # 层级调试器
```

### 2.4 Utilities - 编辑器工具类

```
PoolEditorHelper.cs               # 编辑器辅助工具
PoolAssetProcessor.cs             # 资源处理器
PoolMenuItems.cs                  # 菜单项
```

---

## 🧪 三、Tests 测试模块

### 3.1 Runtime - 运行时测试

```
ObjectPoolTests.cs                # 对象池基础测试
ConcurrentPoolTests.cs            # 并发测试
GameObjectPoolTests.cs            # GameObject 池测试
PoolPolicyTests.cs                # 策略测试
PoolStatisticsTests.cs            # 统计测试
PoolLeakTests.cs                  # 泄漏测试
PoolPerformanceTests.cs           # 性能测试
```

### 3.2 Editor - 编辑器测试

```
PoolInspectorTests.cs             # 检视面板测试
PoolWindowTests.cs                # 窗口测试
```

---

## 📚 四、Samples~ 示例代码

### 4.1 BasicUsage - 基础用法

```
01_SimpleObjectPool.cs            # 简单对象池
02_CustomCreationPolicy.cs        # 自定义创建策略
03_PoolStatistics.cs              # 统计信息
04_PoolWarming.cs                 # 预热
```

### 4.2 UnityIntegration - Unity 集成

```
01_GameObjectPool.cs              # GameObject 池
02_PrefabPool.cs                  # Prefab 池
03_ParticleSystemPool.cs          # 粒子系统池
04_BulletPoolExample.cs           # 子弹池示例
05_EnemyPoolExample.cs            # 敌人池示例
```

### 4.3 AdvancedScenarios - 高级场景

```
01_DIIntegration.cs               # 依赖注入集成
02_AsyncPooling.cs                # 异步池化
03_CustomPoolPolicy.cs            # 自定义池策略
04_PoolLeakDetection.cs           # 泄漏检测
05_PerformanceOptimization.cs     # 性能优化
```

---

## 🔧 五、配置文件

```
AFramework.Pool.asmdef            # 程序集定义
AFramework.Pool.Editor.asmdef     # 编辑器程序集定义
package.json                      # UPM 包配置
```

---

## 📊 六、架构设计原则

### 6.1 SOLID 原则应用

| 原则 | 应用 |
|------|------|
| SRP | 每个池类职责单一（创建/清理/统计分离） |
| OCP | 通过策略模式支持扩展，无需修改核心代码 |
| LSP | 所有池实现可替换 `IObjectPool<T>` |
| ISP | 接口细粒度拆分（创建/清理/统计独立） |
| DIP | 依赖抽象接口，不依赖具体实现 |

### 6.2 设计模式应用

| 模式 | 应用场景 |
|------|----------|
| 对象池模式 | 核心实现 |
| 策略模式 | 创建/清理/容量策略 |
| 工厂模式 | 对象创建 |
| 模板方法模式 | 池操作骨架 |
| 观察者模式 | 统计事件通知 |
| 装饰器模式 | 池功能扩展 |
| 单例模式 | 全局池管理器（可选） |

### 6.3 性能优化策略

- **零 GC 分配**: 使用 `ArrayPool`/`StackPool` 避免堆分配
- **线程安全**: `ConcurrentObjectPool` 使用无锁数据结构
- **缓存友好**: 连续内存布局，提高缓存命中率
- **延迟初始化**: 按需创建，避免启动卡顿
- **分帧预热**: 分散创建负载，避免帧率波动

---

## 🎯 七、使用场景映射

| 场景 | 推荐池类型 | 策略配置 |
|------|-----------|----------|
| 子弹系统 | `GameObjectPool` | 固定容量 + 预热 |
| 敌人生成 | `PrefabPool` | 动态扩容 + 懒加载 |
| 粒子特效 | `ParticleSystemPool` | 自动回收 + 定时清理 |
| UI 弹窗 | `CanvasPool` | 固定容量 + 手动清理 |
| 数据结构 | `ListPool<T>` | 无界容量 + 重置清理 |
| 字符串拼接 | `StringBuilderPool` | 固定容量 + 清空清理 |

---

## 📝 八、命名规范

### 8.1 命名空间

```csharp
AFramework.Pool                   // 核心命名空间
AFramework.Pool          // 策略命名空间
AFramework.Pool             // Unity 集成命名空间
AFramework.Pool.DI                // 依赖注入命名空间
AFramework.Pool.Editor            // 编辑器命名空间
```

### 8.2 类命名

- 接口: `IObjectPool`, `IPoolPolicy`
- 抽象类: `ObjectPoolBase`, `PoolPolicyBase`
- 实现类: `ObjectPool<T>`, `GameObjectPool`
- 策略类: `DefaultCreationPolicy`, `FixedCapacityPolicy`
- 扩展类: `ObjectPoolExtensions`, `PoolRegistrationExtensions`

### 8.3 方法命名

- 获取对象: `Get()`, `GetAsync()`, `Rent()`
- 归还对象: `Return()`, `ReturnAsync()`, `Release()`
- 预热: `Warmup()`, `WarmupAsync()`
- 清理: `Clear()`, `Dispose()`
- 统计: `GetStatistics()`, `GetMetrics()`

---

## 🚀 九、版本兼容性

### 9.1 Unity 版本支持

```csharp
#if UNITY_2022_3_OR_NEWER
    // Unity 2022.3+ 特性
#elif UNITY_2021_3_OR_NEWER
    // Unity 2021.3+ 特性
#endif
```

### 9.2 .NET 版本支持

- .NET Standard 2.1
- .NET 6.0+（Unity 6.x）
- C# 9.0+ 特性（记录类型、模式匹配）

---

## 📖 十、文档结构

```
DOC~/AFramework.Pool/
├── AFramework_Pool_代码目录结构.md           # 本文档
├── AFramework_Pool_技术文档_01_概述与架构.md
├── AFramework_Pool_技术文档_02_核心API参考.md
├── AFramework_Pool_技术文档_03_Unity集成指南.md
├── AFramework_Pool_技术文档_04_性能优化指南.md
├── AFramework_Pool_技术文档_05_依赖注入集成.md
├── AFramework_Pool_技术文档_06_调试与诊断.md
└── AFramework_Pool_技术文档_07_最佳实践.md
```

---

## ✅ 十一、质量保证

### 11.1 代码质量

- ✅ 单元测试覆盖率 > 90%
- ✅ 性能测试（基准测试）
- ✅ 内存泄漏测试
- ✅ 并发安全测试
- ✅ XML 文档注释完整

### 11.2 性能指标

- ✅ Get/Return 操作 < 100ns
- ✅ 零 GC 分配（热路径）
- ✅ 线程安全开销 < 10%
- ✅ 内存占用 < 原生实现 1.2x

---

## 🔗 十二、依赖关系

```
AFramework.Pool
├── AFramework.DI (可选)          # 依赖注入集成
├── UniTask (可选)                # 异步支持
├── R3 (可选)                     # 响应式扩展
└── Unity.Collections (可选)      # 高性能集合
```

---

**文档维护**: AFramework 开发团队  
**最后更新**: 2026-01-19  
**许可证**: MIT License
