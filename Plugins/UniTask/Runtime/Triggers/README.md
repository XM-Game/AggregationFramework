# UniTask 异步触发器说明（Triggers 文件夹）

本目录提供 UniTask 在 Unity 中对 MonoBehaviour 消息的异步化封装，方便以 `await` 方式等待各类事件。以下概览按功能模块梳理，文档不含代码。

## 基础设施
- AsyncTriggerBase：统一的异步事件派发基类，负责注册、事件缓存、取消与释放。
- AsyncTriggerHandler：具体事件的处理器，实现单次/多次等待、取消令牌支持。
- AsyncTriggerExtensions：为 `GameObject`/`Component` 提供便捷获取或添加触发器组件的扩展方法。

## 生命周期与更新
- ApplicationLifecycle：封装 OnApplicationFocus、OnApplicationPause、OnApplicationQuit 等应用级事件。
- ComponentLifecycle：封装 Awake、OnEnable、Start、OnDisable、OnDestroy 等组件生命周期。
- CoreUpdate：封装 Update、LateUpdate、FixedUpdate、AnimatorMove、AnimatorIK 等核心更新与动画回调。
- OtherLifecycle：覆盖渲染与编辑相关事件，如 OnBecameVisible/Invisible、OnWillRenderObject、OnValidate、Reset 等。

## 物理与碰撞（Physics）
- 支持 3D/2D 碰撞与触发器的 Enter/Stay/Exit 系列事件。
- 覆盖 CharacterController、关节断裂、粒子碰撞与触发、相机渲染回调、OnRenderImage、OnGUI、Gizmos 等。
- 每类事件均提供无取消与带取消令牌的异步等待入口。

## UGUI 事件（UGUI）
- 基于 UnityEngine.EventSystems，封装 Pointer、Drag、Drop、Scroll、Select、Submit、Move 等 UI 事件。
- 与 UI 组件配合，使用方式与物理触发器一致，可直接 await 对应事件。

## 模板与生成
- MonoBehaviourMessagesTriggers.tt：T4 模板，用于批量生成各类 MonoBehaviour 消息的触发器代码。
- 生成文件按功能拆分：Physics、UGUI、OtherLifecycle、ApplicationLifecycle、ComponentLifecycle、CoreUpdate。

## 使用要点
- 将触发器组件添加到目标对象后，可通过扩展方法获取，并以 async/await 等待对应事件。
- 支持可取消的异步等待，适用于需要超时或中断的场景。
- 触发器组件均标记 DisallowMultipleComponent，避免重复挂载。

## 典型场景
- 物理交互：等待触发器进入/退出、碰撞、粒子事件以驱动游戏逻辑。
- UI 操作：等待点击、拖拽、滚动、提交等 UI 事件，简化异步表单或交互流程。
- 生命周期：在组件启停或应用暂停/退出时执行异步收尾与资源管理。
- 渲染与编辑：在渲染前后或 Gizmos 绘制时插入异步任务，用于可视化或诊断。

## 维护建议
- 如需扩展新的 MonoBehaviour 消息，优先修改 T4 模板以保持生成一致性。
- 对于已有触发器，确保事件名、注释与 Unity 消息语义保持一致，必要时补充取消令牌的支持说明。

