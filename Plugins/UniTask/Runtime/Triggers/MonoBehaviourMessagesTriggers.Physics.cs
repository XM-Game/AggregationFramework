#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region CollisionEnter
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 碰撞进入异步处理接口
    /// 提供将 Unity 的 OnCollisionEnter 消息转换为可等待的异步任务的能力
    /// 返回碰撞信息（Collision），包含碰撞对象、接触点、碰撞力等信息
    /// </summary>
    public interface IAsyncOnCollisionEnterHandler
    {
        /// <summary>
        /// 等待下一次 OnCollisionEnter 调用
        /// </summary>
        /// <returns>UniTask&lt;Collision&gt;，在发生碰撞时完成，返回碰撞信息</returns>
        UniTask<Collision> OnCollisionEnterAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnCollisionEnter 接口实现
    /// 返回类型为 UniTask&lt;Collision&gt;
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionEnterHandler
    {
        /// <summary>
        /// 实现 IAsyncOnCollisionEnterHandler 接口
        /// </summary>
        UniTask<Collision> IAsyncOnCollisionEnterHandler.OnCollisionEnterAsync()
        {
            core.Reset();
            return new UniTask<Collision>((IUniTaskSource<Collision>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionEnterTrigger 组件
        /// 注意：此触发器需要物理系统支持（通过条件编译宏控制）
        /// </summary>
        public static AsyncCollisionEnterTrigger GetAsyncCollisionEnterTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionEnterTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionEnterTrigger 组件
        /// </summary>
        public static AsyncCollisionEnterTrigger GetAsyncCollisionEnterTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionEnterTrigger();
        }
    }

    /// <summary>
    /// 碰撞进入异步触发器（3D 物理）
    /// 将 Unity 的 OnCollisionEnter 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - 当使用 3D 物理系统（非触发器 Collider）时，对象发生碰撞时触发
    /// - 需要目标 GameObject 上存在 Rigidbody 和 Collider（非触发器）
    /// - 返回的 Collision 对象包含碰撞详细信息（碰撞对象、接触点、碰撞力等）
    /// 
    /// 使用示例：
    /// <code>
    /// var collision = await gameObject.GetAsyncCollisionEnterTrigger().OnCollisionEnterAsync();
    /// Debug.Log($"碰撞对象: {collision.gameObject.name}");
    /// Debug.Log($"接触点数量: {collision.contacts.Length}");
    /// </code>
    /// 
    /// 注意：此触发器仅在支持物理系统的平台/Unity 版本下编译
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncCollisionEnterTrigger : AsyncTriggerBase<Collision>
    {
        /// <summary>
        /// Unity 的 OnCollisionEnter 回调方法
        /// </summary>
        /// <param name="coll">碰撞信息，包含碰撞对象和碰撞详情</param>
        void OnCollisionEnter(Collision coll)
        {
            RaiseEvent(coll); // 传递碰撞信息给等待的任务
        }

        /// <summary>
        /// 获取 OnCollisionEnter 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnCollisionEnterHandler GetOnCollisionEnterAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision>(this, false);
        }

        /// <summary>
        /// 获取 OnCollisionEnter 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnCollisionEnterHandler GetOnCollisionEnterAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次碰撞进入事件，并获取碰撞信息
        /// </summary>
        /// <returns>UniTask&lt;Collision&gt;，包含碰撞信息</returns>
        public UniTask<Collision> OnCollisionEnterAsync()
        {
            return ((IAsyncOnCollisionEnterHandler)new AsyncTriggerHandler<Collision>(this, true)).OnCollisionEnterAsync();
        }

        /// <summary>
        /// 等待下一次碰撞进入事件（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;Collision&gt;，包含碰撞信息</returns>
        public UniTask<Collision> OnCollisionEnterAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionEnterHandler)new AsyncTriggerHandler<Collision>(this, cancellationToken, true)).OnCollisionEnterAsync();
        }
    }
#endif
#endregion
#region CollisionEnter2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 2D 碰撞进入异步处理接口
    /// 提供将 Unity 的 OnCollisionEnter2D 消息转换为可等待的异步任务的能力
    /// 返回 Collision2D 对象，包含碰撞的详细信息
    /// </summary>
    public interface IAsyncOnCollisionEnter2DHandler
    {
        /// <summary>
        /// 等待下一次 2D 碰撞进入事件
        /// </summary>
        /// <returns>UniTask&lt;Collision2D&gt;，在发生 2D 碰撞时完成，返回碰撞信息</returns>
        UniTask<Collision2D> OnCollisionEnter2DAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnCollisionEnter2D 接口实现
    /// 返回类型为 UniTask&lt;Collision2D&gt;，会传递 2D 碰撞信息
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionEnter2DHandler
    {
        /// <summary>
        /// 实现 IAsyncOnCollisionEnter2DHandler 接口
        /// 重置核心状态并返回一个新的 UniTask&lt;Collision2D&gt;
        /// </summary>
        UniTask<Collision2D> IAsyncOnCollisionEnter2DHandler.OnCollisionEnter2DAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask&lt;Collision2D&gt;，绑定到当前处理器实例和版本号
            return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionEnter2DTrigger 组件到指定的 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncCollisionEnter2DTrigger 组件实例</returns>
        public static AsyncCollisionEnter2DTrigger GetAsyncCollisionEnter2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionEnter2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionEnter2DTrigger 组件到指定 Component 所属的 GameObject
        /// </summary>
        /// <param name="component">目标 Component</param>
        /// <returns>AsyncCollisionEnter2DTrigger 组件实例</returns>
        public static AsyncCollisionEnter2DTrigger GetAsyncCollisionEnter2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionEnter2DTrigger();
        }
    }

    /// <summary>
    /// 2D 碰撞进入异步触发器
    /// 将 Unity 的 OnCollisionEnter2D 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnCollisionEnter2D 在对象与其他碰撞体发生 2D 碰撞时调用（仅首次接触时）
    /// - 需要目标对象有 Collider2D 组件，且不是触发器（Is Trigger = false）
    /// - 返回的 Collision2D 包含碰撞点、相对速度、接触点等信息
    /// - 常用于伤害计算、音效播放、特效生成等场景
    /// 
    /// 使用示例：
    /// <code>
    /// var collision = await gameObject.GetAsyncCollisionEnter2DTrigger().OnCollisionEnter2DAsync();
    /// Debug.Log($"与 {collision.gameObject.name} 发生碰撞");
    /// // 处理碰撞逻辑
    /// </code>
    /// 
    /// 注意事项：
    /// - 仅在使用 2D 物理系统时有效（Physics2D）
    /// - 需要确保 Collider2D 不是触发器
    /// - 仅在首次碰撞时触发，后续持续接触不会再次触发
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncCollisionEnter2DTrigger : AsyncTriggerBase<Collision2D>
    {
        /// <summary>
        /// Unity 的 OnCollisionEnter2D 回调方法
        /// 当对象与其他碰撞体发生 2D 碰撞时，Unity 会自动调用此方法
        /// </summary>
        /// <param name="coll">碰撞信息，包含碰撞对象、接触点、相对速度等</param>
        void OnCollisionEnter2D(Collision2D coll)
        {
            // 触发事件，将碰撞信息传递给等待 OnCollisionEnter2DAsync 的任务
            RaiseEvent((coll));
        }

        /// <summary>
        /// 获取 OnCollisionEnter2D 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnCollisionEnter2DHandler GetOnCollisionEnter2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision2D>(this, false);
        }

        /// <summary>
        /// 获取 OnCollisionEnter2D 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnCollisionEnter2DHandler GetOnCollisionEnter2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 2D 碰撞进入事件，并获取碰撞信息
        /// </summary>
        /// <returns>UniTask&lt;Collision2D&gt;，包含碰撞信息</returns>
        public UniTask<Collision2D> OnCollisionEnter2DAsync()
        {
            return ((IAsyncOnCollisionEnter2DHandler)new AsyncTriggerHandler<Collision2D>(this, true)).OnCollisionEnter2DAsync();
        }

        /// <summary>
        /// 等待下一次 2D 碰撞进入事件（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;Collision2D&gt;，包含碰撞信息</returns>
        public UniTask<Collision2D> OnCollisionEnter2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionEnter2DHandler)new AsyncTriggerHandler<Collision2D>(this, cancellationToken, true)).OnCollisionEnter2DAsync();
        }
    }
#endif
#endregion
#region CollisionExit
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 3D 碰撞退出事件（OnCollisionExit）
    /// 返回 Collider 信息，便于在逻辑层以 await 方式处理
    /// </summary>
    public interface IAsyncOnCollisionExitHandler
    {
        /// <summary>
        /// 等待下一次 OnCollisionExit 调用
        /// </summary>
        UniTask<Collision> OnCollisionExitAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionExitHandler
    {
        UniTask<Collision> IAsyncOnCollisionExitHandler.OnCollisionExitAsync()
        {
            core.Reset();
            return new UniTask<Collision>((IUniTaskSource<Collision>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionExitTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncCollisionExitTrigger GetAsyncCollisionExitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionExitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionExitTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncCollisionExitTrigger GetAsyncCollisionExitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionExitTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncCollisionExitTrigger : AsyncTriggerBase<Collision>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnCollisionExit(Collision coll)
        {
            RaiseEvent((coll));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnCollisionExitHandler GetOnCollisionExitAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnCollisionExitHandler GetOnCollisionExitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnCollisionExit，使用 callOnce
        /// </summary>
        public UniTask<Collision> OnCollisionExitAsync()
        {
            return ((IAsyncOnCollisionExitHandler)new AsyncTriggerHandler<Collision>(this, true)).OnCollisionExitAsync();
        }

        /// <summary>
        /// 等待下一次 OnCollisionExit，支持取消
        /// </summary>
        public UniTask<Collision> OnCollisionExitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionExitHandler)new AsyncTriggerHandler<Collision>(this, cancellationToken, true)).OnCollisionExitAsync();
        }
    }
#endif
#endregion
#region CollisionExit2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 异步监听 2D 碰撞退出事件（OnCollisionExit2D）
    /// </summary>
    public interface IAsyncOnCollisionExit2DHandler
    {
        /// <summary>
        /// 等待下一次 OnCollisionExit2D 调用
        /// </summary>
        UniTask<Collision2D> OnCollisionExit2DAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionExit2DHandler
    {
        UniTask<Collision2D> IAsyncOnCollisionExit2DHandler.OnCollisionExit2DAsync()
        {
            core.Reset();
            return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionExit2DTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncCollisionExit2DTrigger GetAsyncCollisionExit2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionExit2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionExit2DTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncCollisionExit2DTrigger GetAsyncCollisionExit2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionExit2DTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncCollisionExit2DTrigger : AsyncTriggerBase<Collision2D>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnCollisionExit2D(Collision2D coll)
        {
            RaiseEvent((coll));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnCollisionExit2DHandler GetOnCollisionExit2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision2D>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnCollisionExit2DHandler GetOnCollisionExit2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnCollisionExit2D，使用 callOnce
        /// </summary>
        public UniTask<Collision2D> OnCollisionExit2DAsync()
        {
            return ((IAsyncOnCollisionExit2DHandler)new AsyncTriggerHandler<Collision2D>(this, true)).OnCollisionExit2DAsync();
        }

        /// <summary>
        /// 等待下一次 OnCollisionExit2D，支持取消
        /// </summary>
        public UniTask<Collision2D> OnCollisionExit2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionExit2DHandler)new AsyncTriggerHandler<Collision2D>(this, cancellationToken, true)).OnCollisionExit2DAsync();
        }
    }
#endif
#endregion
#region CollisionStay
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 3D 碰撞保持事件（OnCollisionStay）
    /// </summary>
    public interface IAsyncOnCollisionStayHandler
    {
        /// <summary>
        /// 等待下一次 OnCollisionStay 调用
        /// </summary>
        UniTask<Collision> OnCollisionStayAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionStayHandler
    {
        UniTask<Collision> IAsyncOnCollisionStayHandler.OnCollisionStayAsync()
        {
            core.Reset();
            return new UniTask<Collision>((IUniTaskSource<Collision>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionStayTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncCollisionStayTrigger GetAsyncCollisionStayTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionStayTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionStayTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncCollisionStayTrigger GetAsyncCollisionStayTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionStayTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncCollisionStayTrigger : AsyncTriggerBase<Collision>
    {
        /// <summary>
        /// Unity 回调入口，持续碰撞时派发事件
        /// </summary>
        void OnCollisionStay(Collision coll)
        {
            RaiseEvent((coll));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnCollisionStayHandler GetOnCollisionStayAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnCollisionStayHandler GetOnCollisionStayAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnCollisionStay，使用 callOnce
        /// </summary>
        public UniTask<Collision> OnCollisionStayAsync()
        {
            return ((IAsyncOnCollisionStayHandler)new AsyncTriggerHandler<Collision>(this, true)).OnCollisionStayAsync();
        }

        /// <summary>
        /// 等待下一次 OnCollisionStay，支持取消
        /// </summary>
        public UniTask<Collision> OnCollisionStayAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionStayHandler)new AsyncTriggerHandler<Collision>(this, cancellationToken, true)).OnCollisionStayAsync();
        }
    }
#endif
#endregion
#region CollisionStay2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 异步监听 2D 碰撞保持事件（OnCollisionStay2D）
    /// </summary>
    public interface IAsyncOnCollisionStay2DHandler
    {
        /// <summary>
        /// 等待下一次 OnCollisionStay2D 调用
        /// </summary>
        UniTask<Collision2D> OnCollisionStay2DAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnCollisionStay2DHandler
    {
        UniTask<Collision2D> IAsyncOnCollisionStay2DHandler.OnCollisionStay2DAsync()
        {
            core.Reset();
            return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCollisionStay2DTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncCollisionStay2DTrigger GetAsyncCollisionStay2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCollisionStay2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCollisionStay2DTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncCollisionStay2DTrigger GetAsyncCollisionStay2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCollisionStay2DTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncCollisionStay2DTrigger : AsyncTriggerBase<Collision2D>
    {
        /// <summary>
        /// Unity 回调入口，持续 2D 碰撞时派发事件
        /// </summary>
        void OnCollisionStay2D(Collision2D coll)
        {
            RaiseEvent((coll));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnCollisionStay2DHandler GetOnCollisionStay2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collision2D>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnCollisionStay2DHandler GetOnCollisionStay2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collision2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnCollisionStay2D，使用 callOnce
        /// </summary>
        public UniTask<Collision2D> OnCollisionStay2DAsync()
        {
            return ((IAsyncOnCollisionStay2DHandler)new AsyncTriggerHandler<Collision2D>(this, true)).OnCollisionStay2DAsync();
        }

        /// <summary>
        /// 等待下一次 OnCollisionStay2D，支持取消
        /// </summary>
        public UniTask<Collision2D> OnCollisionStay2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCollisionStay2DHandler)new AsyncTriggerHandler<Collision2D>(this, cancellationToken, true)).OnCollisionStay2DAsync();
        }
    }
#endif
#endregion
#region ControllerColliderHit
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 CharacterController 碰撞事件（OnControllerColliderHit）
    /// </summary>
    public interface IAsyncOnControllerColliderHitHandler
    {
        /// <summary>
        /// 等待下一次 OnControllerColliderHit 调用
        /// </summary>
        UniTask<ControllerColliderHit> OnControllerColliderHitAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnControllerColliderHitHandler
    {
        UniTask<ControllerColliderHit> IAsyncOnControllerColliderHitHandler.OnControllerColliderHitAsync()
        {
            core.Reset();
            return new UniTask<ControllerColliderHit>((IUniTaskSource<ControllerColliderHit>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncControllerColliderHitTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncControllerColliderHitTrigger GetAsyncControllerColliderHitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncControllerColliderHitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncControllerColliderHitTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncControllerColliderHitTrigger GetAsyncControllerColliderHitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncControllerColliderHitTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncControllerColliderHitTrigger : AsyncTriggerBase<ControllerColliderHit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            RaiseEvent((hit));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnControllerColliderHitHandler GetOnControllerColliderHitAsyncHandler()
        {
            return new AsyncTriggerHandler<ControllerColliderHit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnControllerColliderHitHandler GetOnControllerColliderHitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<ControllerColliderHit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnControllerColliderHit，使用 callOnce
        /// </summary>
        public UniTask<ControllerColliderHit> OnControllerColliderHitAsync()
        {
            return ((IAsyncOnControllerColliderHitHandler)new AsyncTriggerHandler<ControllerColliderHit>(this, true)).OnControllerColliderHitAsync();
        }

        /// <summary>
        /// 等待下一次 OnControllerColliderHit，支持取消
        /// </summary>
        public UniTask<ControllerColliderHit> OnControllerColliderHitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnControllerColliderHitHandler)new AsyncTriggerHandler<ControllerColliderHit>(this, cancellationToken, true)).OnControllerColliderHitAsync();
        }
    }
#endif
#endregion
#region DrawGizmos

    /// <summary>
    /// 异步监听 OnDrawGizmos，用于在编辑/运行时等待绘制回调
    /// </summary>
    public interface IAsyncOnDrawGizmosHandler
    {
        /// <summary>
        /// 等待下一次 OnDrawGizmos 调用
        /// </summary>
        UniTask OnDrawGizmosAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnDrawGizmosHandler
    {
        UniTask IAsyncOnDrawGizmosHandler.OnDrawGizmosAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDrawGizmosTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncDrawGizmosTrigger GetAsyncDrawGizmosTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDrawGizmosTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDrawGizmosTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncDrawGizmosTrigger GetAsyncDrawGizmosTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDrawGizmosTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncDrawGizmosTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnDrawGizmos()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnDrawGizmosHandler GetOnDrawGizmosAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnDrawGizmosHandler GetOnDrawGizmosAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnDrawGizmos，使用 callOnce
        /// </summary>
        public UniTask OnDrawGizmosAsync()
        {
            return ((IAsyncOnDrawGizmosHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnDrawGizmosAsync();
        }

        /// <summary>
        /// 等待下一次 OnDrawGizmos，支持取消
        /// </summary>
        public UniTask OnDrawGizmosAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDrawGizmosHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnDrawGizmosAsync();
        }
    }
#endregion
#region DrawGizmosSelected

    /// <summary>
    /// 异步监听 OnDrawGizmosSelected，仅在选中对象时触发
    /// </summary>
    public interface IAsyncOnDrawGizmosSelectedHandler
    {
        /// <summary>
        /// 等待下一次 OnDrawGizmosSelected 调用
        /// </summary>
        UniTask OnDrawGizmosSelectedAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnDrawGizmosSelectedHandler
    {
        UniTask IAsyncOnDrawGizmosSelectedHandler.OnDrawGizmosSelectedAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDrawGizmosSelectedTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncDrawGizmosSelectedTrigger GetAsyncDrawGizmosSelectedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDrawGizmosSelectedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDrawGizmosSelectedTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncDrawGizmosSelectedTrigger GetAsyncDrawGizmosSelectedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDrawGizmosSelectedTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncDrawGizmosSelectedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，仅在物体被选中时调用
        /// </summary>
        void OnDrawGizmosSelected()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnDrawGizmosSelectedHandler GetOnDrawGizmosSelectedAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnDrawGizmosSelectedHandler GetOnDrawGizmosSelectedAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnDrawGizmosSelected，使用 callOnce
        /// </summary>
        public UniTask OnDrawGizmosSelectedAsync()
        {
            return ((IAsyncOnDrawGizmosSelectedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnDrawGizmosSelectedAsync();
        }

        /// <summary>
        /// 等待下一次 OnDrawGizmosSelected，支持取消
        /// </summary>
        public UniTask OnDrawGizmosSelectedAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDrawGizmosSelectedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnDrawGizmosSelectedAsync();
        }
    }
#endregion
#region GUI

    /// <summary>
    /// 异步监听 OnGUI，用于 IMGUI 绘制时的 await 等待
    /// </summary>
    public interface IAsyncOnGUIHandler
    {
        /// <summary>
        /// 等待下一次 OnGUI 调用
        /// </summary>
        UniTask OnGUIAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnGUIHandler
    {
        UniTask IAsyncOnGUIHandler.OnGUIAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncGUITrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncGUITrigger GetAsyncGUITrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncGUITrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncGUITrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncGUITrigger GetAsyncGUITrigger(this Component component)
        {
            return component.gameObject.GetAsyncGUITrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncGUITrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnGUI()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnGUIHandler GetOnGUIAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnGUIHandler GetOnGUIAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnGUI，使用 callOnce
        /// </summary>
        public UniTask OnGUIAsync()
        {
            return ((IAsyncOnGUIHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnGUIAsync();
        }

        /// <summary>
        /// 等待下一次 OnGUI，支持取消
        /// </summary>
        public UniTask OnGUIAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnGUIHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnGUIAsync();
        }
    }
#endregion
#region JointBreak
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 3D 关节断裂事件（OnJointBreak）
    /// </summary>
    public interface IAsyncOnJointBreakHandler
    {
        /// <summary>
        /// 等待下一次 OnJointBreak 调用，返回断裂力度
        /// </summary>
        UniTask<float> OnJointBreakAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnJointBreakHandler
    {
        UniTask<float> IAsyncOnJointBreakHandler.OnJointBreakAsync()
        {
            core.Reset();
            return new UniTask<float>((IUniTaskSource<float>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncJointBreakTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncJointBreakTrigger GetAsyncJointBreakTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncJointBreakTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncJointBreakTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncJointBreakTrigger GetAsyncJointBreakTrigger(this Component component)
        {
            return component.gameObject.GetAsyncJointBreakTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncJointBreakTrigger : AsyncTriggerBase<float>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnJointBreak(float breakForce)
        {
            RaiseEvent((breakForce));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnJointBreakHandler GetOnJointBreakAsyncHandler()
        {
            return new AsyncTriggerHandler<float>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnJointBreakHandler GetOnJointBreakAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<float>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnJointBreak，使用 callOnce
        /// </summary>
        public UniTask<float> OnJointBreakAsync()
        {
            return ((IAsyncOnJointBreakHandler)new AsyncTriggerHandler<float>(this, true)).OnJointBreakAsync();
        }

        /// <summary>
        /// 等待下一次 OnJointBreak，支持取消
        /// </summary>
        public UniTask<float> OnJointBreakAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnJointBreakHandler)new AsyncTriggerHandler<float>(this, cancellationToken, true)).OnJointBreakAsync();
        }
    }
#endif
#endregion
#region JointBreak2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 异步监听 2D 关节断裂事件（OnJointBreak2D）
    /// </summary>
    public interface IAsyncOnJointBreak2DHandler
    {
        /// <summary>
        /// 等待下一次 OnJointBreak2D 调用，返回断裂的 Joint2D
        /// </summary>
        UniTask<Joint2D> OnJointBreak2DAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnJointBreak2DHandler
    {
        UniTask<Joint2D> IAsyncOnJointBreak2DHandler.OnJointBreak2DAsync()
        {
            core.Reset();
            return new UniTask<Joint2D>((IUniTaskSource<Joint2D>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncJointBreak2DTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncJointBreak2DTrigger GetAsyncJointBreak2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncJointBreak2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncJointBreak2DTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncJointBreak2DTrigger GetAsyncJointBreak2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncJointBreak2DTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncJointBreak2DTrigger : AsyncTriggerBase<Joint2D>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnJointBreak2D(Joint2D brokenJoint)
        {
            RaiseEvent((brokenJoint));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnJointBreak2DHandler GetOnJointBreak2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Joint2D>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnJointBreak2DHandler GetOnJointBreak2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Joint2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnJointBreak2D，使用 callOnce
        /// </summary>
        public UniTask<Joint2D> OnJointBreak2DAsync()
        {
            return ((IAsyncOnJointBreak2DHandler)new AsyncTriggerHandler<Joint2D>(this, true)).OnJointBreak2DAsync();
        }

        /// <summary>
        /// 等待下一次 OnJointBreak2D，支持取消
        /// </summary>
        public UniTask<Joint2D> OnJointBreak2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnJointBreak2DHandler)new AsyncTriggerHandler<Joint2D>(this, cancellationToken, true)).OnJointBreak2DAsync();
        }
    }
#endif
#endregion
#region MouseDown
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标按下事件（OnMouseDown）
    /// </summary>
    public interface IAsyncOnMouseDownHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseDown 调用
        /// </summary>
        UniTask OnMouseDownAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseDownHandler
    {
        UniTask IAsyncOnMouseDownHandler.OnMouseDownAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseDownTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseDownTrigger GetAsyncMouseDownTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseDownTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseDownTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseDownTrigger GetAsyncMouseDownTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseDownTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseDownTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseDown()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseDownHandler GetOnMouseDownAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseDownHandler GetOnMouseDownAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseDown，使用 callOnce
        /// </summary>
        public UniTask OnMouseDownAsync()
        {
            return ((IAsyncOnMouseDownHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseDownAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseDown，支持取消
        /// </summary>
        public UniTask OnMouseDownAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseDownHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseDownAsync();
        }
    }
#endif
#endregion
#region MouseDrag
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标拖拽事件（OnMouseDrag）
    /// </summary>
    public interface IAsyncOnMouseDragHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseDrag 调用
        /// </summary>
        UniTask OnMouseDragAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseDragHandler
    {
        UniTask IAsyncOnMouseDragHandler.OnMouseDragAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseDragTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseDragTrigger GetAsyncMouseDragTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseDragTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseDragTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseDragTrigger GetAsyncMouseDragTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseDragTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseDragTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseDrag()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseDragHandler GetOnMouseDragAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseDragHandler GetOnMouseDragAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseDrag，使用 callOnce
        /// </summary>
        public UniTask OnMouseDragAsync()
        {
            return ((IAsyncOnMouseDragHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseDragAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseDrag，支持取消
        /// </summary>
        public UniTask OnMouseDragAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseDragHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseDragAsync();
        }
    }
#endif
#endregion
#region MouseEnter
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标进入事件（OnMouseEnter）
    /// </summary>
    public interface IAsyncOnMouseEnterHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseEnter 调用
        /// </summary>
        UniTask OnMouseEnterAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseEnterHandler
    {
        UniTask IAsyncOnMouseEnterHandler.OnMouseEnterAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseEnterTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseEnterTrigger GetAsyncMouseEnterTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseEnterTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseEnterTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseEnterTrigger GetAsyncMouseEnterTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseEnterTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseEnterTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseEnter()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseEnterHandler GetOnMouseEnterAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseEnterHandler GetOnMouseEnterAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseEnter，使用 callOnce
        /// </summary>
        public UniTask OnMouseEnterAsync()
        {
            return ((IAsyncOnMouseEnterHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseEnterAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseEnter，支持取消
        /// </summary>
        public UniTask OnMouseEnterAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseEnterHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseEnterAsync();
        }
    }
#endif
#endregion
#region MouseExit
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标离开事件（OnMouseExit）
    /// </summary>
    public interface IAsyncOnMouseExitHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseExit 调用
        /// </summary>
        UniTask OnMouseExitAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseExitHandler
    {
        UniTask IAsyncOnMouseExitHandler.OnMouseExitAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseExitTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseExitTrigger GetAsyncMouseExitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseExitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseExitTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseExitTrigger GetAsyncMouseExitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseExitTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseExitTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseExit()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseExitHandler GetOnMouseExitAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseExitHandler GetOnMouseExitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseExit，使用 callOnce
        /// </summary>
        public UniTask OnMouseExitAsync()
        {
            return ((IAsyncOnMouseExitHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseExitAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseExit，支持取消
        /// </summary>
        public UniTask OnMouseExitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseExitHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseExitAsync();
        }
    }
#endif
#endregion
#region MouseOver
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标悬停事件（OnMouseOver）
    /// </summary>
    public interface IAsyncOnMouseOverHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseOver 调用
        /// </summary>
        UniTask OnMouseOverAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseOverHandler
    {
        UniTask IAsyncOnMouseOverHandler.OnMouseOverAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseOverTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseOverTrigger GetAsyncMouseOverTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseOverTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseOverTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseOverTrigger GetAsyncMouseOverTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseOverTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseOverTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseOver()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseOverHandler GetOnMouseOverAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseOverHandler GetOnMouseOverAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseOver，使用 callOnce
        /// </summary>
        public UniTask OnMouseOverAsync()
        {
            return ((IAsyncOnMouseOverHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseOverAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseOver，支持取消
        /// </summary>
        public UniTask OnMouseOverAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseOverHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseOverAsync();
        }
    }
#endif
#endregion
#region MouseUp
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听鼠标抬起事件（OnMouseUp）
    /// </summary>
    public interface IAsyncOnMouseUpHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseUp 调用
        /// </summary>
        UniTask OnMouseUpAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseUpHandler
    {
        UniTask IAsyncOnMouseUpHandler.OnMouseUpAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseUpTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseUpTrigger GetAsyncMouseUpTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseUpTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseUpTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseUpTrigger GetAsyncMouseUpTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseUpTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseUpTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseUp()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseUpHandler GetOnMouseUpAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseUpHandler GetOnMouseUpAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseUp，使用 callOnce
        /// </summary>
        public UniTask OnMouseUpAsync()
        {
            return ((IAsyncOnMouseUpHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseUpAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseUp，支持取消
        /// </summary>
        public UniTask OnMouseUpAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseUpHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseUpAsync();
        }
    }
#endif
#endregion
#region MouseUpAsButton
#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

    /// <summary>
    /// 异步监听 OnMouseUpAsButton，用于判定点击完成
    /// </summary>
    public interface IAsyncOnMouseUpAsButtonHandler
    {
        /// <summary>
        /// 等待下一次 OnMouseUpAsButton 调用
        /// </summary>
        UniTask OnMouseUpAsButtonAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnMouseUpAsButtonHandler
    {
        UniTask IAsyncOnMouseUpAsButtonHandler.OnMouseUpAsButtonAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMouseUpAsButtonTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncMouseUpAsButtonTrigger GetAsyncMouseUpAsButtonTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMouseUpAsButtonTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMouseUpAsButtonTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncMouseUpAsButtonTrigger GetAsyncMouseUpAsButtonTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMouseUpAsButtonTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncMouseUpAsButtonTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnMouseUpAsButton()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnMouseUpAsButtonHandler GetOnMouseUpAsButtonAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnMouseUpAsButtonHandler GetOnMouseUpAsButtonAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnMouseUpAsButton，使用 callOnce
        /// </summary>
        public UniTask OnMouseUpAsButtonAsync()
        {
            return ((IAsyncOnMouseUpAsButtonHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseUpAsButtonAsync();
        }

        /// <summary>
        /// 等待下一次 OnMouseUpAsButton，支持取消
        /// </summary>
        public UniTask OnMouseUpAsButtonAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMouseUpAsButtonHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseUpAsButtonAsync();
        }
    }
#endif
#endregion
#region ParticleCollision

    /// <summary>
    /// 异步监听粒子碰撞事件（OnParticleCollision）
    /// </summary>
    public interface IAsyncOnParticleCollisionHandler
    {
        /// <summary>
        /// 等待下一次粒子碰撞，返回被碰撞的对象
        /// </summary>
        UniTask<GameObject> OnParticleCollisionAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnParticleCollisionHandler
    {
        UniTask<GameObject> IAsyncOnParticleCollisionHandler.OnParticleCollisionAsync()
        {
            core.Reset();
            return new UniTask<GameObject>((IUniTaskSource<GameObject>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncParticleCollisionTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncParticleCollisionTrigger GetAsyncParticleCollisionTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncParticleCollisionTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncParticleCollisionTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncParticleCollisionTrigger GetAsyncParticleCollisionTrigger(this Component component)
        {
            return component.gameObject.GetAsyncParticleCollisionTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncParticleCollisionTrigger : AsyncTriggerBase<GameObject>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnParticleCollision(GameObject other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnParticleCollisionHandler GetOnParticleCollisionAsyncHandler()
        {
            return new AsyncTriggerHandler<GameObject>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnParticleCollisionHandler GetOnParticleCollisionAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<GameObject>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次粒子碰撞事件，使用 callOnce
        /// </summary>
        public UniTask<GameObject> OnParticleCollisionAsync()
        {
            return ((IAsyncOnParticleCollisionHandler)new AsyncTriggerHandler<GameObject>(this, true)).OnParticleCollisionAsync();
        }

        /// <summary>
        /// 等待下一次粒子碰撞事件，支持取消
        /// </summary>
        public UniTask<GameObject> OnParticleCollisionAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnParticleCollisionHandler)new AsyncTriggerHandler<GameObject>(this, cancellationToken, true)).OnParticleCollisionAsync();
        }
    }
#endregion
#region ParticleSystemStopped

    /// <summary>
    /// 异步监听粒子系统停止事件（OnParticleSystemStopped）
    /// </summary>
    public interface IAsyncOnParticleSystemStoppedHandler
    {
        /// <summary>
        /// 等待下一次粒子系统停止
        /// </summary>
        UniTask OnParticleSystemStoppedAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnParticleSystemStoppedHandler
    {
        UniTask IAsyncOnParticleSystemStoppedHandler.OnParticleSystemStoppedAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncParticleSystemStoppedTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncParticleSystemStoppedTrigger GetAsyncParticleSystemStoppedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncParticleSystemStoppedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncParticleSystemStoppedTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncParticleSystemStoppedTrigger GetAsyncParticleSystemStoppedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncParticleSystemStoppedTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncParticleSystemStoppedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnParticleSystemStopped()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnParticleSystemStoppedHandler GetOnParticleSystemStoppedAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnParticleSystemStoppedHandler GetOnParticleSystemStoppedAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次粒子系统停止，使用 callOnce
        /// </summary>
        public UniTask OnParticleSystemStoppedAsync()
        {
            return ((IAsyncOnParticleSystemStoppedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnParticleSystemStoppedAsync();
        }

        /// <summary>
        /// 等待下一次粒子系统停止，支持取消
        /// </summary>
        public UniTask OnParticleSystemStoppedAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnParticleSystemStoppedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnParticleSystemStoppedAsync();
        }
    }
#endregion
#region ParticleTrigger

    /// <summary>
    /// 异步监听粒子触发事件（OnParticleTrigger）
    /// </summary>
    public interface IAsyncOnParticleTriggerHandler
    {
        /// <summary>
        /// 等待下一次 OnParticleTrigger 调用
        /// </summary>
        UniTask OnParticleTriggerAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnParticleTriggerHandler
    {
        UniTask IAsyncOnParticleTriggerHandler.OnParticleTriggerAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncParticleTriggerTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncParticleTriggerTrigger GetAsyncParticleTriggerTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncParticleTriggerTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncParticleTriggerTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncParticleTriggerTrigger GetAsyncParticleTriggerTrigger(this Component component)
        {
            return component.gameObject.GetAsyncParticleTriggerTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncParticleTriggerTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnParticleTrigger()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnParticleTriggerHandler GetOnParticleTriggerAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnParticleTriggerHandler GetOnParticleTriggerAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次粒子触发事件，使用 callOnce
        /// </summary>
        public UniTask OnParticleTriggerAsync()
        {
            return ((IAsyncOnParticleTriggerHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnParticleTriggerAsync();
        }

        /// <summary>
        /// 等待下一次粒子触发事件，支持取消
        /// </summary>
        public UniTask OnParticleTriggerAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnParticleTriggerHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnParticleTriggerAsync();
        }
    }
#endregion
#region ParticleUpdateJobScheduled
#if UNITY_2019_3_OR_NEWER && (!UNITY_2019_1_OR_NEWER || UNITASK_PARTICLESYSTEM_SUPPORT)

    /// <summary>
    /// 异步监听粒子系统任务调度事件（OnParticleUpdateJobScheduled）
    /// </summary>
    public interface IAsyncOnParticleUpdateJobScheduledHandler
    {
        /// <summary>
        /// 等待下一次 OnParticleUpdateJobScheduled 调用，返回 JobData
        /// </summary>
        UniTask<UnityEngine.ParticleSystemJobs.ParticleSystemJobData> OnParticleUpdateJobScheduledAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnParticleUpdateJobScheduledHandler
    {
        UniTask<UnityEngine.ParticleSystemJobs.ParticleSystemJobData> IAsyncOnParticleUpdateJobScheduledHandler.OnParticleUpdateJobScheduledAsync()
        {
            core.Reset();
            return new UniTask<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>((IUniTaskSource<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncParticleUpdateJobScheduledTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncParticleUpdateJobScheduledTrigger GetAsyncParticleUpdateJobScheduledTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncParticleUpdateJobScheduledTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncParticleUpdateJobScheduledTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncParticleUpdateJobScheduledTrigger GetAsyncParticleUpdateJobScheduledTrigger(this Component component)
        {
            return component.gameObject.GetAsyncParticleUpdateJobScheduledTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncParticleUpdateJobScheduledTrigger : AsyncTriggerBase<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnParticleUpdateJobScheduled(UnityEngine.ParticleSystemJobs.ParticleSystemJobData particles)
        {
            RaiseEvent((particles));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnParticleUpdateJobScheduledHandler GetOnParticleUpdateJobScheduledAsyncHandler()
        {
            return new AsyncTriggerHandler<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnParticleUpdateJobScheduledHandler GetOnParticleUpdateJobScheduledAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnParticleUpdateJobScheduled，使用 callOnce
        /// </summary>
        public UniTask<UnityEngine.ParticleSystemJobs.ParticleSystemJobData> OnParticleUpdateJobScheduledAsync()
        {
            return ((IAsyncOnParticleUpdateJobScheduledHandler)new AsyncTriggerHandler<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>(this, true)).OnParticleUpdateJobScheduledAsync();
        }

        /// <summary>
        /// 等待下一次 OnParticleUpdateJobScheduled，支持取消
        /// </summary>
        public UniTask<UnityEngine.ParticleSystemJobs.ParticleSystemJobData> OnParticleUpdateJobScheduledAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnParticleUpdateJobScheduledHandler)new AsyncTriggerHandler<UnityEngine.ParticleSystemJobs.ParticleSystemJobData>(this, cancellationToken, true)).OnParticleUpdateJobScheduledAsync();
        }
    }
#endif
#endregion
#region PostRender

    /// <summary>
    /// 异步监听摄像机后期渲染事件（OnPostRender）
    /// </summary>
    public interface IAsyncOnPostRenderHandler
    {
        /// <summary>
        /// 等待下一次 OnPostRender 调用
        /// </summary>
        UniTask OnPostRenderAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnPostRenderHandler
    {
        UniTask IAsyncOnPostRenderHandler.OnPostRenderAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPostRenderTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncPostRenderTrigger GetAsyncPostRenderTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPostRenderTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPostRenderTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncPostRenderTrigger GetAsyncPostRenderTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPostRenderTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncPostRenderTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnPostRender()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnPostRenderHandler GetOnPostRenderAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnPostRenderHandler GetOnPostRenderAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnPostRender，使用 callOnce
        /// </summary>
        public UniTask OnPostRenderAsync()
        {
            return ((IAsyncOnPostRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnPostRenderAsync();
        }

        /// <summary>
        /// 等待下一次 OnPostRender，支持取消
        /// </summary>
        public UniTask OnPostRenderAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPostRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnPostRenderAsync();
        }
    }
#endregion
#region PreCull

    /// <summary>
    /// 异步监听摄像机剔除前事件（OnPreCull）
    /// </summary>
    public interface IAsyncOnPreCullHandler
    {
        /// <summary>
        /// 等待下一次 OnPreCull 调用
        /// </summary>
        UniTask OnPreCullAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnPreCullHandler
    {
        UniTask IAsyncOnPreCullHandler.OnPreCullAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPreCullTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncPreCullTrigger GetAsyncPreCullTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPreCullTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPreCullTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncPreCullTrigger GetAsyncPreCullTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPreCullTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncPreCullTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnPreCull()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnPreCullHandler GetOnPreCullAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnPreCullHandler GetOnPreCullAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnPreCull，使用 callOnce
        /// </summary>
        public UniTask OnPreCullAsync()
        {
            return ((IAsyncOnPreCullHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnPreCullAsync();
        }

        /// <summary>
        /// 等待下一次 OnPreCull，支持取消
        /// </summary>
        public UniTask OnPreCullAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPreCullHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnPreCullAsync();
        }
    }
#endregion
#region PreRender

    /// <summary>
    /// 异步监听摄像机渲染前事件（OnPreRender）
    /// </summary>
    public interface IAsyncOnPreRenderHandler
    {
        /// <summary>
        /// 等待下一次 OnPreRender 调用
        /// </summary>
        UniTask OnPreRenderAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnPreRenderHandler
    {
        UniTask IAsyncOnPreRenderHandler.OnPreRenderAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPreRenderTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncPreRenderTrigger GetAsyncPreRenderTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPreRenderTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPreRenderTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncPreRenderTrigger GetAsyncPreRenderTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPreRenderTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncPreRenderTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnPreRender()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnPreRenderHandler GetOnPreRenderAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnPreRenderHandler GetOnPreRenderAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnPreRender，使用 callOnce
        /// </summary>
        public UniTask OnPreRenderAsync()
        {
            return ((IAsyncOnPreRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnPreRenderAsync();
        }

        /// <summary>
        /// 等待下一次 OnPreRender，支持取消
        /// </summary>
        public UniTask OnPreRenderAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPreRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnPreRenderAsync();
        }
    }
#endregion
#region RectTransformDimensionsChange

    /// <summary>
    /// 异步监听 RectTransform 尺寸变化（OnRectTransformDimensionsChange）
    /// </summary>
    public interface IAsyncOnRectTransformDimensionsChangeHandler
    {
        /// <summary>
        /// 等待下一次尺寸变化事件
        /// </summary>
        UniTask OnRectTransformDimensionsChangeAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnRectTransformDimensionsChangeHandler
    {
        UniTask IAsyncOnRectTransformDimensionsChangeHandler.OnRectTransformDimensionsChangeAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncRectTransformDimensionsChangeTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncRectTransformDimensionsChangeTrigger GetAsyncRectTransformDimensionsChangeTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncRectTransformDimensionsChangeTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncRectTransformDimensionsChangeTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncRectTransformDimensionsChangeTrigger GetAsyncRectTransformDimensionsChangeTrigger(this Component component)
        {
            return component.gameObject.GetAsyncRectTransformDimensionsChangeTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncRectTransformDimensionsChangeTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnRectTransformDimensionsChange()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnRectTransformDimensionsChangeHandler GetOnRectTransformDimensionsChangeAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnRectTransformDimensionsChangeHandler GetOnRectTransformDimensionsChangeAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次尺寸变化事件，使用 callOnce
        /// </summary>
        public UniTask OnRectTransformDimensionsChangeAsync()
        {
            return ((IAsyncOnRectTransformDimensionsChangeHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRectTransformDimensionsChangeAsync();
        }

        /// <summary>
        /// 等待下一次尺寸变化事件，支持取消
        /// </summary>
        public UniTask OnRectTransformDimensionsChangeAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnRectTransformDimensionsChangeHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRectTransformDimensionsChangeAsync();
        }
    }
#endregion
#region RectTransformRemoved

    /// <summary>
    /// 异步监听 RectTransform 被移除事件（OnRectTransformRemoved）
    /// </summary>
    public interface IAsyncOnRectTransformRemovedHandler
    {
        /// <summary>
        /// 等待下一次 OnRectTransformRemoved 调用
        /// </summary>
        UniTask OnRectTransformRemovedAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnRectTransformRemovedHandler
    {
        UniTask IAsyncOnRectTransformRemovedHandler.OnRectTransformRemovedAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncRectTransformRemovedTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncRectTransformRemovedTrigger GetAsyncRectTransformRemovedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncRectTransformRemovedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncRectTransformRemovedTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncRectTransformRemovedTrigger GetAsyncRectTransformRemovedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncRectTransformRemovedTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncRectTransformRemovedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnRectTransformRemoved()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnRectTransformRemovedHandler GetOnRectTransformRemovedAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnRectTransformRemovedHandler GetOnRectTransformRemovedAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次移除事件，使用 callOnce
        /// </summary>
        public UniTask OnRectTransformRemovedAsync()
        {
            return ((IAsyncOnRectTransformRemovedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRectTransformRemovedAsync();
        }

        /// <summary>
        /// 等待下一次移除事件，支持取消
        /// </summary>
        public UniTask OnRectTransformRemovedAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnRectTransformRemovedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRectTransformRemovedAsync();
        }
    }
#endregion
#region RenderImage

    /// <summary>
    /// 异步监听后处理渲染事件（OnRenderImage）
    /// </summary>
    public interface IAsyncOnRenderImageHandler
    {
        /// <summary>
        /// 等待下一次 OnRenderImage 调用，返回源/目标 RenderTexture
        /// </summary>
        UniTask<(RenderTexture source, RenderTexture destination)> OnRenderImageAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnRenderImageHandler
    {
        UniTask<(RenderTexture source, RenderTexture destination)> IAsyncOnRenderImageHandler.OnRenderImageAsync()
        {
            core.Reset();
            return new UniTask<(RenderTexture source, RenderTexture destination)>((IUniTaskSource<(RenderTexture source, RenderTexture destination)>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncRenderImageTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncRenderImageTrigger GetAsyncRenderImageTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncRenderImageTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncRenderImageTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncRenderImageTrigger GetAsyncRenderImageTrigger(this Component component)
        {
            return component.gameObject.GetAsyncRenderImageTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncRenderImageTrigger : AsyncTriggerBase<(RenderTexture source, RenderTexture destination)>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RaiseEvent((source, destination));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnRenderImageHandler GetOnRenderImageAsyncHandler()
        {
            return new AsyncTriggerHandler<(RenderTexture source, RenderTexture destination)>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnRenderImageHandler GetOnRenderImageAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<(RenderTexture source, RenderTexture destination)>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnRenderImage，使用 callOnce
        /// </summary>
        public UniTask<(RenderTexture source, RenderTexture destination)> OnRenderImageAsync()
        {
            return ((IAsyncOnRenderImageHandler)new AsyncTriggerHandler<(RenderTexture source, RenderTexture destination)>(this, true)).OnRenderImageAsync();
        }

        /// <summary>
        /// 等待下一次 OnRenderImage，支持取消
        /// </summary>
        public UniTask<(RenderTexture source, RenderTexture destination)> OnRenderImageAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnRenderImageHandler)new AsyncTriggerHandler<(RenderTexture source, RenderTexture destination)>(this, cancellationToken, true)).OnRenderImageAsync();
        }
    }
#endregion
#region RenderObject

    /// <summary>
    /// 异步监听对象渲染事件（OnRenderObject）
    /// </summary>
    public interface IAsyncOnRenderObjectHandler
    {
        /// <summary>
        /// 等待下一次 OnRenderObject 调用
        /// </summary>
        UniTask OnRenderObjectAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnRenderObjectHandler
    {
        UniTask IAsyncOnRenderObjectHandler.OnRenderObjectAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncRenderObjectTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncRenderObjectTrigger GetAsyncRenderObjectTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncRenderObjectTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncRenderObjectTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncRenderObjectTrigger GetAsyncRenderObjectTrigger(this Component component)
        {
            return component.gameObject.GetAsyncRenderObjectTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncRenderObjectTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnRenderObject()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnRenderObjectHandler GetOnRenderObjectAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnRenderObjectHandler GetOnRenderObjectAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnRenderObject，使用 callOnce
        /// </summary>
        public UniTask OnRenderObjectAsync()
        {
            return ((IAsyncOnRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRenderObjectAsync();
        }

        /// <summary>
        /// 等待下一次 OnRenderObject，支持取消
        /// </summary>
        public UniTask OnRenderObjectAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRenderObjectAsync();
        }
    }
#endregion
#region TriggerEnter
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 触发器进入异步处理接口
    /// 提供将 Unity 的 OnTriggerEnter 消息转换为可等待的异步任务的能力
    /// 返回 Collider 对象，表示进入触发器的碰撞体
    /// </summary>
    public interface IAsyncOnTriggerEnterHandler
    {
        /// <summary>
        /// 等待下一次触发器进入事件
        /// </summary>
        /// <returns>UniTask&lt;Collider&gt;，在对象进入触发器时完成，返回碰撞体信息</returns>
        UniTask<Collider> OnTriggerEnterAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnTriggerEnter 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerEnterHandler
    {
        UniTask<Collider> IAsyncOnTriggerEnterHandler.OnTriggerEnterAsync()
        {
            core.Reset();
            return new UniTask<Collider>((IUniTaskSource<Collider>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerEnterTrigger 组件
        /// </summary>
        public static AsyncTriggerEnterTrigger GetAsyncTriggerEnterTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerEnterTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerEnterTrigger 组件
        /// </summary>
        public static AsyncTriggerEnterTrigger GetAsyncTriggerEnterTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerEnterTrigger();
        }
    }

    /// <summary>
    /// 触发器进入异步触发器
    /// 将 Unity 的 OnTriggerEnter 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnTriggerEnter 在对象进入触发器时调用（仅首次进入时）
    /// - 需要目标对象有 Collider 组件，且设置为触发器（Is Trigger = true）
    /// - 返回的 Collider 是进入触发器的碰撞体对象
    /// - 常用于区域检测、拾取物品、传送门、伤害区域等场景
    /// 
    /// 使用示例：
    /// <code>
    /// var collider = await triggerZone.GetAsyncTriggerEnterTrigger().OnTriggerEnterAsync();
    /// Debug.Log($"{collider.name} 进入了触发器区域");
    /// // 处理进入逻辑
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 Collider 设置为触发器（Is Trigger = true）
    /// - 仅在首次进入时触发，后续持续在触发器内不会再次触发
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncTriggerEnterTrigger : AsyncTriggerBase<Collider>
    {
        /// <summary>
        /// Unity 的 OnTriggerEnter 回调方法
        /// </summary>
        /// <param name="other">进入触发器的碰撞体</param>
        void OnTriggerEnter(Collider other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取 OnTriggerEnter 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnTriggerEnterHandler GetOnTriggerEnterAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider>(this, false);
        }

        /// <summary>
        /// 获取 OnTriggerEnter 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnTriggerEnterHandler GetOnTriggerEnterAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次触发器进入事件，并获取碰撞体信息
        /// </summary>
        public UniTask<Collider> OnTriggerEnterAsync()
        {
            return ((IAsyncOnTriggerEnterHandler)new AsyncTriggerHandler<Collider>(this, true)).OnTriggerEnterAsync();
        }

        /// <summary>
        /// 等待下一次触发器进入事件（带取消令牌）
        /// </summary>
        public UniTask<Collider> OnTriggerEnterAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerEnterHandler)new AsyncTriggerHandler<Collider>(this, cancellationToken, true)).OnTriggerEnterAsync();
        }
    }
#endif
#endregion
#region TriggerEnter2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 2D 触发器进入异步处理接口
    /// 提供将 Unity 的 OnTriggerEnter2D 消息转换为可等待的异步任务的能力
    /// 返回 Collider2D 对象，表示进入触发器的 2D 碰撞体
    /// </summary>
    public interface IAsyncOnTriggerEnter2DHandler
    {
        /// <summary>
        /// 等待下一次 2D 触发器进入事件
        /// </summary>
        /// <returns>UniTask&lt;Collider2D&gt;，在对象进入 2D 触发器时完成</returns>
        UniTask<Collider2D> OnTriggerEnter2DAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnTriggerEnter2D 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerEnter2DHandler
    {
        UniTask<Collider2D> IAsyncOnTriggerEnter2DHandler.OnTriggerEnter2DAsync()
        {
            core.Reset();
            return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerEnter2DTrigger 组件
        /// </summary>
        public static AsyncTriggerEnter2DTrigger GetAsyncTriggerEnter2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerEnter2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerEnter2DTrigger 组件
        /// </summary>
        public static AsyncTriggerEnter2DTrigger GetAsyncTriggerEnter2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerEnter2DTrigger();
        }
    }

    /// <summary>
    /// 2D 触发器进入异步触发器
    /// 将 Unity 的 OnTriggerEnter2D 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnTriggerEnter2D 在对象进入 2D 触发器时调用（仅首次进入时）
    /// - 需要目标对象有 Collider2D 组件，且设置为触发器（Is Trigger = true）
    /// - 返回的 Collider2D 是进入触发器的 2D 碰撞体对象
    /// - 常用于 2D 游戏的区域检测、道具拾取、区域触发等场景
    /// 
    /// 注意事项：
    /// - 仅在使用 2D 物理系统时有效（Physics2D）
    /// - 需要确保 Collider2D 设置为触发器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncTriggerEnter2DTrigger : AsyncTriggerBase<Collider2D>
    {
        /// <summary>
        /// Unity 的 OnTriggerEnter2D 回调方法
        /// </summary>
        /// <param name="other">进入触发器的 2D 碰撞体</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取 OnTriggerEnter2D 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnTriggerEnter2DHandler GetOnTriggerEnter2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider2D>(this, false);
        }

        /// <summary>
        /// 获取 OnTriggerEnter2D 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnTriggerEnter2DHandler GetOnTriggerEnter2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 2D 触发器进入事件，并获取碰撞体信息
        /// </summary>
        public UniTask<Collider2D> OnTriggerEnter2DAsync()
        {
            return ((IAsyncOnTriggerEnter2DHandler)new AsyncTriggerHandler<Collider2D>(this, true)).OnTriggerEnter2DAsync();
        }

        /// <summary>
        /// 等待下一次 2D 触发器进入事件（带取消令牌）
        /// </summary>
        public UniTask<Collider2D> OnTriggerEnter2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerEnter2DHandler)new AsyncTriggerHandler<Collider2D>(this, cancellationToken, true)).OnTriggerEnter2DAsync();
        }
    }
#endif
#endregion
#region TriggerExit
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 3D 触发器退出事件（OnTriggerExit）
    /// </summary>
    public interface IAsyncOnTriggerExitHandler
    {
        /// <summary>
        /// 等待下一次 OnTriggerExit 调用，返回离开的碰撞体
        /// </summary>
        UniTask<Collider> OnTriggerExitAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerExitHandler
    {
        UniTask<Collider> IAsyncOnTriggerExitHandler.OnTriggerExitAsync()
        {
            core.Reset();
            return new UniTask<Collider>((IUniTaskSource<Collider>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerExitTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncTriggerExitTrigger GetAsyncTriggerExitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerExitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerExitTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncTriggerExitTrigger GetAsyncTriggerExitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerExitTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncTriggerExitTrigger : AsyncTriggerBase<Collider>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnTriggerExitHandler GetOnTriggerExitAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnTriggerExitHandler GetOnTriggerExitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnTriggerExit，使用 callOnce
        /// </summary>
        public UniTask<Collider> OnTriggerExitAsync()
        {
            return ((IAsyncOnTriggerExitHandler)new AsyncTriggerHandler<Collider>(this, true)).OnTriggerExitAsync();
        }

        /// <summary>
        /// 等待下一次 OnTriggerExit，支持取消
        /// </summary>
        public UniTask<Collider> OnTriggerExitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerExitHandler)new AsyncTriggerHandler<Collider>(this, cancellationToken, true)).OnTriggerExitAsync();
        }
    }
#endif
#endregion
#region TriggerExit2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 异步监听 2D 触发器退出事件（OnTriggerExit2D）
    /// </summary>
    public interface IAsyncOnTriggerExit2DHandler
    {
        /// <summary>
        /// 等待下一次 OnTriggerExit2D 调用，返回离开的 2D 碰撞体
        /// </summary>
        UniTask<Collider2D> OnTriggerExit2DAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerExit2DHandler
    {
        UniTask<Collider2D> IAsyncOnTriggerExit2DHandler.OnTriggerExit2DAsync()
        {
            core.Reset();
            return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerExit2DTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncTriggerExit2DTrigger GetAsyncTriggerExit2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerExit2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerExit2DTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncTriggerExit2DTrigger GetAsyncTriggerExit2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerExit2DTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncTriggerExit2DTrigger : AsyncTriggerBase<Collider2D>
    {
        /// <summary>
        /// Unity 回调入口，触发时派发事件
        /// </summary>
        void OnTriggerExit2D(Collider2D other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnTriggerExit2DHandler GetOnTriggerExit2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider2D>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnTriggerExit2DHandler GetOnTriggerExit2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnTriggerExit2D，使用 callOnce
        /// </summary>
        public UniTask<Collider2D> OnTriggerExit2DAsync()
        {
            return ((IAsyncOnTriggerExit2DHandler)new AsyncTriggerHandler<Collider2D>(this, true)).OnTriggerExit2DAsync();
        }

        /// <summary>
        /// 等待下一次 OnTriggerExit2D，支持取消
        /// </summary>
        public UniTask<Collider2D> OnTriggerExit2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerExit2DHandler)new AsyncTriggerHandler<Collider2D>(this, cancellationToken, true)).OnTriggerExit2DAsync();
        }
    }
#endif
#endregion
#region TriggerStay
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS_SUPPORT

    /// <summary>
    /// 异步监听 3D 触发器持续事件（OnTriggerStay）
    /// </summary>
    public interface IAsyncOnTriggerStayHandler
    {
        /// <summary>
        /// 等待下一次 OnTriggerStay 调用
        /// </summary>
        UniTask<Collider> OnTriggerStayAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerStayHandler
    {
        UniTask<Collider> IAsyncOnTriggerStayHandler.OnTriggerStayAsync()
        {
            core.Reset();
            return new UniTask<Collider>((IUniTaskSource<Collider>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerStayTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncTriggerStayTrigger GetAsyncTriggerStayTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerStayTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerStayTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncTriggerStayTrigger GetAsyncTriggerStayTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerStayTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncTriggerStayTrigger : AsyncTriggerBase<Collider>
    {
        /// <summary>
        /// Unity 回调入口，持续停留时派发事件
        /// </summary>
        void OnTriggerStay(Collider other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnTriggerStayHandler GetOnTriggerStayAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnTriggerStayHandler GetOnTriggerStayAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnTriggerStay，使用 callOnce
        /// </summary>
        public UniTask<Collider> OnTriggerStayAsync()
        {
            return ((IAsyncOnTriggerStayHandler)new AsyncTriggerHandler<Collider>(this, true)).OnTriggerStayAsync();
        }

        /// <summary>
        /// 等待下一次 OnTriggerStay，支持取消
        /// </summary>
        public UniTask<Collider> OnTriggerStayAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerStayHandler)new AsyncTriggerHandler<Collider>(this, cancellationToken, true)).OnTriggerStayAsync();
        }
    }
#endif
#endregion
#region TriggerStay2D
#if !UNITY_2019_1_OR_NEWER || UNITASK_PHYSICS2D_SUPPORT

    /// <summary>
    /// 异步监听 2D 触发器持续事件（OnTriggerStay2D）
    /// </summary>
    public interface IAsyncOnTriggerStay2DHandler
    {
        /// <summary>
        /// 等待下一次 OnTriggerStay2D 调用
        /// </summary>
        UniTask<Collider2D> OnTriggerStay2DAsync();
    }

    public partial class AsyncTriggerHandler<T> : IAsyncOnTriggerStay2DHandler
    {
        UniTask<Collider2D> IAsyncOnTriggerStay2DHandler.OnTriggerStay2DAsync()
        {
            core.Reset();
            return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncTriggerStay2DTrigger 组件（GameObject 扩展）
        /// </summary>
        public static AsyncTriggerStay2DTrigger GetAsyncTriggerStay2DTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTriggerStay2DTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTriggerStay2DTrigger 组件（Component 扩展）
        /// </summary>
        public static AsyncTriggerStay2DTrigger GetAsyncTriggerStay2DTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTriggerStay2DTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncTriggerStay2DTrigger : AsyncTriggerBase<Collider2D>
    {
        /// <summary>
        /// Unity 回调入口，持续停留时派发事件
        /// </summary>
        void OnTriggerStay2D(Collider2D other)
        {
            RaiseEvent((other));
        }

        /// <summary>
        /// 获取一次性异步处理器（不启用 callOnce）
        /// </summary>
        public IAsyncOnTriggerStay2DHandler GetOnTriggerStay2DAsyncHandler()
        {
            return new AsyncTriggerHandler<Collider2D>(this, false);
        }

        /// <summary>
        /// 获取一次性异步处理器（支持取消）
        /// </summary>
        public IAsyncOnTriggerStay2DHandler GetOnTriggerStay2DAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<Collider2D>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnTriggerStay2D，使用 callOnce
        /// </summary>
        public UniTask<Collider2D> OnTriggerStay2DAsync()
        {
            return ((IAsyncOnTriggerStay2DHandler)new AsyncTriggerHandler<Collider2D>(this, true)).OnTriggerStay2DAsync();
        }

        /// <summary>
        /// 等待下一次 OnTriggerStay2D，支持取消
        /// </summary>
        public UniTask<Collider2D> OnTriggerStay2DAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnTriggerStay2DHandler)new AsyncTriggerHandler<Collider2D>(this, cancellationToken, true)).OnTriggerStay2DAsync();
        }
    }
#endif
#endregion
}