# UniTask 扩展功能说明（Runtime/Extensions）

本文概述 `Plugins/UniTask/Runtime/Extensions` 目录下的主要功能点，帮助在 Unity 6.3 项目中快速了解可用的异步工具与使用场景。本文不包含代码示例，重点说明能力与适用性。

## 核心能力概览
- 任务桥接：在 UniTask 与 `Task`/`ValueTask`/Unity 协程之间进行相互转换，便于与现有库和协程接口互操作。
- 组合与并发：提供 WhenAny/WhenAll/WhenEach 等批量组合 API（含大量自动生成的重载），支持同时等待多个任务的完成或任何一个完成。
- 等待与计时：Yield、NextFrame、固定/不受时间缩放的 Delay、基于帧或时间的超时、等待条件/值变化等多种等待模式，覆盖游戏主循环的典型节奏。
- 线程与上下文切换：在主线程、线程池、任务池、指定同步上下文之间安全切换，并提供作用域式返回（ReturnToXXX）。
- 取消与生命周期：扩展 `CancellationToken`/`CancellationTokenSource` 的创建、链接、延时取消、GameObject 销毁联动，以及注册时避免 ExecutionContext 捕获。
- 集合与枚举器支持：将 IEnumerator/可枚举的 UniTask 序列与 UniTask 工作流互通，便于从协程或 LINQ 管道过渡到异步任务。
- 工厂与简写：`UniTask.Factory` 及 `.Shorthand` 提供创建、链接与常用快捷写法，减少样板代码。

## 任务转换与桥接
- Task/Task<T> 与 UniTask/UniTask<T> 的双向转换；AsyncLazy 支持惰性异步初始化。
- ValueTask 与 UniTask 互转（在支持的平台下使用 IValueTaskSource 适配）。
- 将 UniTask 转为 IEnumerator 用于协程，或将 IEnumerator 包装为 UniTask（含取消与自定义 PlayerLoop）。

## 调度与线程切换
- SwitchToMainThread / ReturnToMainThread：从后台切回主线程或在离开作用域后回到主线程。
- SwitchToThreadPool / SwitchToTaskPool：在后台线程执行工作；RunOnThreadPool 系列包装线程池执行并可选择返回主线程。
- SwitchToSynchronizationContext / ReturnToSynchronizationContext：在自定义同步上下文间切换，适配外部消息循环。
- Post：向指定 PlayerLoopTiming 排队 continuation，方便在主循环安全调度。

## 延迟与等待工具
- Yield/NextFrame：对齐不同 PlayerLoopTiming，确保下一帧或指定时机继续。
- WaitForEndOfFrame、WaitForFixedUpdate：覆盖典型渲染/物理阶段。
- Delay/DelayFrame/Timeout：支持受/不受时间缩放的延时、帧计数延时、超时取消以及实时计时模式。
- WaitUntil/WaitWhile：基于条件持续等待，可指定取消策略与 PlayerLoopTiming。
- WaitUntilValueChanged：监视对象属性/值的变化（包含 UnityEngine.Object 安全检查）。

## 取消与生命周期管理
- UniTask/UniTask<T> 转换为 CancellationToken，或从 CancellationToken 获得可 await 的 UniTask。
- RegisterWithoutCaptureExecutionContext：注册回调时避免 ExecutionContext 开销，适用于高频场景。
- CancelAfterSlim：为 `CancellationTokenSource` 添加基于时间/帧的延时取消（支持 DeltaTime/Unscaled/Realtime）。
- RegisterRaiseCancelOnDestroy：与 GameObject/Component 生命周期联动，在销毁时自动触发取消。
- AddTo：将 IDisposable 绑定到取消令牌，令取消时自动释放资源。

## 组合与批量操作
- WhenAll/WhenAny/WhenEach：等待所有、任意或依次返回完成项，覆盖不同参数数量的泛型重载以减少手动数组/列表分配。
- AttachExternalCancellation / Timeout 系列：为已有任务附加外部取消或超时逻辑，避免无主任务悬挂。

## 集合与协程辅助
- EnumeratorAsyncExtensions：让 IEnumerator 可直接 await，或安全地转换为 UniTask（可带取消、PlayerLoop 配置）。
- EnumerableAsyncExtensions：为 `IEnumerable<UniTask>` 提供 LINQ 选择器的重载，便于在 LINQ 管道中组合异步任务。

## 异常与实用扩展
- ExceptionExtensions：提供 `IsOperationCanceledException` 辅助判断。
- ToCancellationToken / ToCancellationToken(link) 等：在任务完成后自动触发取消，简化等待到取消令牌的转换。

## 使用建议
- 在主循环相关逻辑使用基于 PlayerLoopTiming 的等待 API，确保时序与 Unity 帧同步。
- 后台计算优先使用 `RunOnThreadPool` 或 `SwitchToThreadPool`，完成后再切回主线程处理 Unity 对象。
- 组合等待（WhenAll/WhenAny）优先选用内置重载，减少分配并提升可读性。
- 使用 `RegisterWithoutCaptureExecutionContext`、`CancelAfterSlim` 等扩展在高频或生命周期敏感场景降低开销并确保资源回收。

