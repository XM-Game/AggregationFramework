## UniTask 编译器扩展与状态机运行器说明（Runtime/CompilerServices）

本目录包含 UniTask 在 C# 编译/运行时阶段配合工作的核心“胶水层”代码，负责让编译器生成的 async 状态机与 UniTask 类型正确对接，并在 Unity/IL2CPP 环境下高效、安全地运行。这里的类型大多不会被业务代码直接调用，但它们决定了 `async UniTask` / `async UniTask<T>` / `async UniTaskVoid` 的行为、性能和调试体验。

### 整体设计思路
- **自定义 AsyncMethodBuilder**：通过 `AsyncMethodBuilderAttribute` 告诉 C# 编译器，当返回类型是 UniTask 相关类型时，应该使用自定义的 MethodBuilder，而不是默认的 Task/Task&lt;T&gt; Builder。
- **状态机运行器（Runner）与 Promise 分离**：将“驱动状态机的 MoveNext 调用”和“承载结果/异常的 UniTask 源”组合在一起，实现无额外分配的 async 状态流。
- **对象池与 TaskTracker**：所有运行器实例都通过 `TaskPool` 复用，并通过 `TaskTracker` 跟踪活跃任务，降低 GC 压力同时保留调试能力。
- **IL2CPP 兼容性处理**：通过条件编译和延迟归还（ReturnAction + PlayerLoopHelper）绕开 Unity IL2CPP 在特定情况下的虚拟机 Bug。

### AsyncMethodBuilderAttribute
- 为 UniTask 系列类型定义自定义的 AsyncMethodBuilder。
- 作用是让 `async UniTask` / `async UniTask<T>` / `async UniTaskVoid` 在编译器眼中拥有与 `async Task` 同样的语义，但底层实现换成 UniTask 的高性能结构。

### AsyncUniTaskVoidMethodBuilder
- 对应 `async UniTaskVoid` 或与 UniTaskVoid 语义相近的无返回值异步方法。
- 职责：
  - 在第一次遇到 `await` 时，为该状态机分配一个 `AsyncUniTaskVoid<TStateMachine>` 运行器，并将其保存到状态机字段中。
  - 通过 `AwaitOnCompleted` / `AwaitUnsafeOnCompleted` 将 awaiter 的 continuation 绑定到运行器的 `MoveNext` 委托上。
  - 在 `SetResult` 或 `SetException` 时，负责归还运行器到对象池，并在异常情况下将未观察的异常交给 UniTaskScheduler 统一处理。
- 特点：
  - `Task` 属性为 `UniTaskVoid` 占位返回值，主要用于编译器协议而非实际等待。
  - 对 IL2CPP 平台，归还逻辑可能延迟到 PlayerLoop 的特定阶段执行，以避开虚拟机的已知问题。

### AsyncUniTaskMethodBuilder / AsyncUniTaskMethodBuilder<T>
- 对应 `async UniTask` 与 `async UniTask<T>`。
- 主要职责：
  - 懒惰创建状态机运行器：只有在首次遇到真正的异步 `await` 时，才会创建 `AsyncUniTask<TStateMachine>` 或 `AsyncUniTask<TStateMachine,T>` 实例。
  - 在没有任何真正异步等待的情况下，直接返回：
    - 对无返回值方法：返回 `UniTask.CompletedTask`。
    - 对有返回值方法：直接返回保存的 `result` 值。
    - 对异常情况：在还未创建 runner 时，将异常缓存在 builder 中，并生成对应的失败 UniTask。
  - 将 awaiter 的 continuation 绑定到 runner 的 `MoveNext` 上，从而驱动编译器生成的状态机前进。
- 设计意义：
  - 避免在同步完成的 async 方法中创建任何堆对象，极大降低 GC 分配。
  - 将状态机与 UniTask 源统一托管在对象池管理的 Runner 中，便于重用与调试。

### IStateMachineRunner / IStateMachineRunnerPromise 接口族
- **IStateMachineRunner**：
  - 最小运行器接口，只关心 `MoveNext` 与 `Return`。
  - 在 IL2CPP 下还会暴露 `ReturnAction` 用于通过 PlayerLoop 延迟归还。
- **IStateMachineRunnerPromise** 与 **IStateMachineRunnerPromise<T>**：
  - 在 Runner 能力上叠加 “Promise” 功能，即实现 UniTask 源接口，暴露：
    - 用于外部 `await` 的 `Task` / `Task<T>`。
    - `SetResult` / `SetException` 用于由状态机内部写回结果或异常。
  - 这两个接口是编译器生成代码与 UniTask 内部实现之间的重要桥梁。

### AsyncUniTaskVoid<TStateMachine>
- 用于 `async UniTaskVoid` 或类似语义的状态机运行器。
- 核心职责：
  - 持有编译器生成的 `TStateMachine` 结构体实例。
  - 对外提供 `MoveNext` 委托，供 awaiter 在完成时调用，以推进状态机。
  - 实现 `ITaskPoolNode` 接口，支持放入 `TaskPool` 中复用。
  - 实现 `IUniTaskSource` 的空实现，仅允许 `TaskTracker` 统一追踪（不会真正通过它来获取结果）。
- 生命周期：
  - 通过 `SetStateMachine` 静态方法与状态机字段关联。
  - 在状态机执行完毕（或异常）后，调用 `Return` 将自身重置并放回对象池。

### AsyncUniTask<TStateMachine> 与 AsyncUniTask<TStateMachine, T>
- 对应 `async UniTask` 与 `async UniTask<T>` 的运行器 + Promise 实现。
- 共同点：
  - 内部持有状态机实例与 `UniTaskCompletionSourceCore`（无返回值使用 `AsyncUnit`，有返回值使用 `T`）。
  - 对外暴露 `Task` / `Task<T>` 属性，返回一个可被 `await` 的 UniTask 视图。
  - 实现 `GetResult` / `GetStatus` / `OnCompleted` 等接口，供 Awaiter 访问。
  - 完成后通过 `Return` 或 `TryReturn` 将自身重置并归还到对象池。
- IL2CPP 特殊处理：
  - 在某些平台上，为了避免 VM Bug，不在 `GetResult` 的 `finally` 中直接调用 `Return`，
    而是通过 `PlayerLoopHelper.AddContinuation` 在帧循环的特定时机调用预先缓存的 `returnDelegate`。

### StateMachineUtility
- 为调试和兼容性检查提供的辅助工具。
- 通过反射读取编译器生成的 async 状态机内部 `__state` 字段，用于：
  - 检查当前状态机所处状态（例如：-1 表示完成，0/1/2... 为中间状态）。
  - 调试 IL2CPP 下状态机行为是否异常。
- 一般不建议在高频路径中调用，仅用于诊断与调试工具。

### 使用与扩展建议
- 对业务层开发者而言：
  - 不需要手动直接使用本目录中的类型，只需正常编写 `async UniTask` / `async UniTask<T>` 方法即可。
  - 如需理解 UniTask 的性能特性、调试行为或异常传播机制，可以结合本目录源码与注释进行深入阅读。
- 对引擎或框架级开发者而言：
  - 可以参考这些 MethodBuilder 与 Runner 的实现模式，构建自定义的 Task-like 类型或状态机驱动模型。
  - 注意 IL2CPP 平台上的特殊处理与 PlayerLoop 协调逻辑，避免在 AOT 环境下引入新的虚拟机兼容性问题。


