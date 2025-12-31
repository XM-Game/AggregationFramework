## UniTask 核心类型说明（Runtime/Core）

本目录包含 UniTask 的核心协议与基础类型，定义了任务表示、状态机源接口、CompletionSource、异步枚举协议等关键组件。业务代码通常无需直接引用这些类型，但理解它们有助于掌握 UniTask 的性能特点与行为。

### 任务表示与状态
- `UniTask` / `UniTask<T>`：轻量级 Task 替代，使用结构体 + `IUniTaskSource` 组合实现无/低分配的 await。
- `UniTaskVoid`：对应 `async UniTaskVoid`（或类似 fire-and-forget）语义的任务占位类型。
- `AsyncUnit`：无返回值的统一占位结果类型，用于将 void 语义纳入泛型流程。
- `UniTaskStatus` 及扩展方法：统一的任务状态枚举（Pending/Succeeded/Faulted/Canceled）与快捷判断（IsCompleted/IsCanceled/IsFaulted 等）。

### 异步源接口
- `IUniTaskSource` / `IUniTaskSource<T>`：类似 `IValueTaskSource` 的自定义协议，定义 GetStatus/OnCompleted/GetResult/UnsafeGetStatus 等接口，支撑 UniTask 的 await 行为与无分配调度。
- `MoveNextSource`：抽象基类，封装一个 `UniTaskCompletionSourceCore<bool>`，用于基于 “MoveNext 返回 bool” 语义的 await 支持（如异步迭代器实现中复用）。

### CompletionSource 与 Promise
- `UniTaskCompletionSourceCore<TResult>`：核心的无分配完成源（struct），存储结果/异常/取消状态与 continuation。提供 TrySetResult / TrySetException / TrySetCanceled、GetResult/OnCompleted 等实现，并包含版本号以防止重复消费。
- `UniTaskCompletionSource` / `UniTaskCompletionSource<T>`（在同文件后部定义）：对 Core 的封装，提供面向外部的 `Task` / `Task<T>` 视图，便于手动创建/完成 UniTask。
- 相关 Promise 接口（`IResolvePromise` / `IRejectPromise` / `ICancelPromise` / `IPromise<T>` 等）：为 CompletionSource 的 TrySetX 行为提供统一的接口抽象。
- 异常托管：通过 `ExceptionHolder` 和 `UniTaskScheduler.PublishUnobservedTaskException` 处理未观察的异常，确保异常不会被静默丢弃。

### 异步枚举协议
- `IUniTaskAsyncEnumerable<T>` / `IUniTaskAsyncEnumerator<T>` / `IUniTaskAsyncDisposable`：UniTask 版异步 LINQ/迭代协议（对应 .NET IAsyncEnumerable/IAsyncEnumerator），使用 `UniTask<bool> MoveNextAsync` 约定。
- `IUniTaskOrderedAsyncEnumerable<T>` / `IConnectableUniTaskAsyncEnumerable<T>`：扩展接口，支持排序流与可连接（热流）等高级用法。
- `UniTaskAsyncEnumerableExtensions.WithCancellation`：为枚举流附加外部取消令牌，返回 `UniTaskCancelableAsyncEnumerable<T>` 便捷包装。

### 设计要点
- **低分配**：使用结构体 + 源接口 + 对象池（在编译器服务层的 Runner 中）降低 GC 压力。
- **可组合**：统一的源接口与 CompletionSource 核心，支撑 UniTask 在任务、异步枚举、扩展方法等多场景复用。
- **兼容性**：`IUniTaskSource` 映射到 `IValueTaskSource`（在支持的平台上），便于与 ValueTask 生态互操作。
- **异常与取消安全**：版本号、未观察异常发布、取消令牌包装等机制确保 await/多次 await 的安全性与可诊断性。

### 何时阅读这些类型
- 想理解 UniTask 如何实现无分配 await、如何与编译器生成的状态机对接。
- 需要手工创建可 await 的任务或异步枚举源（例如自定义驱动器/桥接器）。
- 调试异步流程（尤其是异常未观察、重复 await、取消/超时行为）时，查看 CompletionSource 与 IUniTaskSource 的实现有助定位问题。

