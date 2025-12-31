UniTask Runtime 功能总览（无代码版）

本自述覆盖 Runtime 目录全部功能子模块，便于快速定位职责与典型用途。内容仅为文字概览，不含代码与示例。

1) Core（核心）
- 定义 UniTask/UniTaskVoid、完成源、异步可枚举与任务源接口，奠定 await 调度与结果传递的基础。
- 提供 AsyncUnit 等轻量结构，统一异步语义。

2) Extensions（扩展方法）
- 为 UniTask 提供组合、延迟、并发、线程切换、工厂方法、条件等待等常用能力。
- 包含取消令牌与集合相关的辅助扩展，降低样板代码。

3) Unity（Unity 集成）
- 将常用 Unity API 异步化，包括协程、资源加载、GPU 读回、实例化、Jobs、WebRequest、uGUI 等。
- 提供等待器与异常类型，便于与引擎生命周期衔接。

4) Infrastructure（基础设施）
- PlayerLoop 集成、任务池、定时器、超时控制、进度报告、通道等运行时支撑。
- 统一的触发事件与延迟初始化工具，提升性能与可维护性。

5) Scheduling（调度）
- 自定义调度器与同步上下文，处理线程切换、异常转发和执行隔离。

6) Observable（可观察序列）
- 面向响应式场景的可观察扩展与异步响应式属性，支持基于事件的数据流。

7) Triggers（触发器）
- 将 MonoBehaviour 消息转为可等待的异步触发器，覆盖生命周期、物理碰撞与触发器、渲染回调、Gizmos、OnGUI、粒子与 UI 事件等。
- 通过扩展方法便捷获取或添加触发器组件，支持取消令牌和单次/多次等待。

8) Linq（异步 LINQ）
- 提供异步序列的 LINQ 操作符及 Unity 特化操作（如每帧更新、值变化、定时器），用于声明式数据流处理。

9) CompilerServices（编译器服务）
- 异步方法构建器与状态机运行器，支撑 C# async/await 与 UniTask 的编译期生成。

10) External（第三方集成）
- 针对 Addressables、DOTween、TextMeshPro 等常见库的异步封装，便于在统一任务模型下使用。

11) Internal（内部实现）
- 内部工具与适配层，隐藏实现细节，通常不直接被业务调用。

12) 程序集与可见性
- UniTask.asmdef 定义程序集边界；_InternalVisibleTo.cs 管理内部可见性。

使用建议
- 框架入门：先阅读 Core 与 Extensions，了解任务模型与常用操作。
- Unity 项目：重点关注 Unity 与 Triggers，可直接将引擎事件转为 await。
- 高级场景：并发调度用 Scheduling；数据流与 UI 交互可用 Observable 与 Linq。
- 第三方库：在 External 目录查找对应封装，减少重复适配工作。

维护提示
- 新增引擎事件的异步封装时，保持与现有命名和取消模式一致，必要时更新触发器模板。
- 变更核心或基础设施组件时，注意与调度、触发器及外部集成的兼容性。

