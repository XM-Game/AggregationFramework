# UniTask Runtime 文件组织结构说明

本文档说明了 UniTask Runtime 模块的文件组织结构，帮助开发者快速定位和理解各个功能模块。

## 目录结构

```
Runtime/
├── Core/                          # 核心类型和接口
│   ├── UniTask.cs                # UniTask 核心结构体
│   ├── UniTaskVoid.cs            # UniTaskVoid 结构体
│   ├── UniTaskCompletionSource.cs # UniTask 完成源
│   ├── IUniTaskSource.cs         # UniTask 源接口
│   ├── IUniTaskAsyncEnumerable.cs # 异步可枚举接口
│   ├── MoveNextSource.cs         # 移动下一个源基类
│   └── AsyncUnit.cs              # 异步单元类型
│
├── Extensions/                    # UniTask 扩展方法
│   ├── UniTaskExtensions.cs      # 主要扩展方法
│   ├── UniTaskExtensions.Shorthand.cs # 简写扩展
│   ├── UniTask.WhenAll.cs        # WhenAll 方法
│   ├── UniTask.WhenAny.cs        # WhenAny 方法
│   ├── UniTask.WhenEach.cs       # WhenEach 方法
│   ├── UniTask.Delay.cs          # 延迟方法
│   ├── UniTask.Run.cs            # 运行方法
│   ├── UniTask.Threading.cs      # 线程相关方法
│   ├── UniTask.WaitUntil.cs      # 等待直到条件
│   ├── UniTask.Factory.cs        # 工厂方法
│   ├── UniTask.Bridge.cs         # 桥接方法
│   ├── UniTask.AsValueTask.cs    # ValueTask 转换
│   ├── CancellationTokenExtensions.cs      # 取消令牌扩展
│   ├── CancellationTokenSourceExtensions.cs # 取消令牌源扩展
│   ├── EnumerableAsyncExtensions.cs         # 可枚举异步扩展
│   ├── EnumeratorAsyncExtensions.cs         # 枚举器异步扩展
│   └── ExceptionExtensions.cs              # 异常扩展
│
├── Unity/                         # Unity 引擎集成
│   ├── UnityAsyncExtensions.cs   # Unity 异步扩展（主要）
│   ├── UnityAsyncExtensions.uGUI.cs        # uGUI 扩展
│   ├── UnityAsyncExtensions.MonoBehaviour.cs # MonoBehaviour 扩展
│   ├── UnityAsyncExtensions.Jobs.cs         # Jobs 扩展
│   ├── UnityAsyncExtensions.AsyncInstantiate.cs # 异步实例化
│   ├── UnityAsyncExtensions.AsyncGPUReadback.cs # GPU 读取回退
│   ├── UnityAsyncExtensions.AssetBundleRequestAllAssets.cs # AssetBundle 扩展
│   ├── UnityBindingExtensions.cs  # Unity 绑定扩展
│   ├── UnityAwaitableExtensions.cs # Unity 可等待扩展
│   └── UnityWebRequestException.cs # WebRequest 异常
│
├── Infrastructure/                # 基础设施和工具类
│   ├── PlayerLoopHelper.cs      # Unity PlayerLoop 辅助类
│   ├── PlayerLoopTimer.cs       # PlayerLoop 定时器
│   ├── TaskPool.cs              # 任务对象池
│   ├── Progress.cs               # 进度报告
│   ├── TimeoutController.cs     # 超时控制器
│   ├── TriggerEvent.cs           # 触发事件
│   ├── AsyncLazy.cs             # 异步延迟初始化
│   ├── Channel.cs                # 通道（生产者-消费者）
│   └── CancellationTokenEqualityComparer.cs # 取消令牌相等比较器
│
├── Scheduling/                    # 调度相关
│   ├── UniTaskScheduler.cs      # UniTask 调度器
│   └── UniTaskSynchronizationContext.cs # 同步上下文
│
├── Observable/                    # 可观察序列（响应式编程）
│   ├── UniTaskObservableExtensions.cs # 可观察扩展
│   └── AsyncReactiveProperty.cs  # 异步响应式属性
│
├── Triggers/                      # Unity 触发器
│   ├── AsyncTriggerBase.cs      # 触发器基类
│   ├── AsyncAwakeTrigger.cs     # Awake 触发器
│   ├── AsyncStartTrigger.cs     # Start 触发器
│   ├── AsyncDestroyTrigger.cs  # Destroy 触发器
│   └── MonoBehaviourMessagesTriggers.cs # MonoBehaviour 消息触发器
│
├── Linq/                          # LINQ 操作符
│   ├── [各种 LINQ 操作符文件]
│   └── UnityExtensions/          # Unity LINQ 扩展
│       ├── EveryUpdate.cs       # 每帧更新
│       ├── EveryValueChanged.cs # 值变化监听
│       └── Timer.cs              # 定时器
│
├── Internal/                      # 内部实现
│   └── [内部工具和辅助类]
│
├── CompilerServices/              # 编译器服务
│   ├── AsyncUniTaskMethodBuilder.cs      # 异步方法构建器
│   ├── AsyncUniTaskVoidMethodBuilder.cs # Void 方法构建器
│   └── StateMachineRunner.cs            # 状态机运行器
│
├── External/                      # 外部库集成
│   ├── Addressables/            # Addressables 集成
│   ├── DOTween/                 # DOTween 集成
│   └── TextMeshPro/            # TextMeshPro 集成
│
├── UniTask.asmdef                # 程序集定义文件
└── _InternalVisibleTo.cs        # 内部可见性配置
```

## 文件夹说明

### Core（核心）
包含 UniTask 框架的核心类型和接口定义。这些是框架的基础，其他模块都依赖于这些核心类型。

### Extensions（扩展方法）
包含 UniTask 的各种扩展方法，提供丰富的功能支持，如任务组合、延迟、线程操作等。

### Unity（Unity 集成）
专门为 Unity 引擎提供的扩展方法，包括 Unity API 的异步包装、uGUI 支持、Jobs 系统集成等。

### Infrastructure（基础设施）
提供框架运行所需的基础设施，如 PlayerLoop 集成、对象池、进度报告、超时控制等。

### Scheduling（调度）
处理任务的调度和执行上下文，包括异常处理和同步上下文管理。

### Observable（可观察序列）
提供响应式编程支持，包括可观察序列扩展和响应式属性。

### Triggers（触发器）
Unity MonoBehaviour 生命周期事件的异步触发器，方便在 Unity 中使用异步操作。

### Linq（LINQ 操作符）
完整的异步 LINQ 操作符集合，支持声明式数据流处理。

### Internal（内部实现）
框架内部的实现细节，通常不需要直接使用。

### CompilerServices（编译器服务）
编译器相关的服务，用于支持 async/await 语法。

### External（外部库集成）
与第三方库的集成，如 Addressables、DOTween、TextMeshPro 等。

## 使用建议

1. **核心功能**：从 `Core/` 文件夹开始了解框架的基础类型
2. **扩展方法**：查看 `Extensions/` 文件夹了解可用的扩展功能
3. **Unity 集成**：Unity 项目重点关注 `Unity/` 和 `Triggers/` 文件夹
4. **响应式编程**：需要响应式功能时查看 `Observable/` 文件夹
5. **数据流处理**：使用 LINQ 操作符时查看 `Linq/` 文件夹

## 注意事项

- 文件移动后，Unity 会自动更新 .meta 文件
- 如果遇到编译错误，可能需要重新导入程序集
- 建议在 Unity 编辑器中刷新资源（Assets > Refresh）

