#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region FixedUpdate

    /// <summary>
    /// FixedUpdate 异步处理接口
    /// 提供将 Unity 的 FixedUpdate 消息转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncFixedUpdateHandler
    {
        /// <summary>
        /// 等待下一次 FixedUpdate 调用
        /// </summary>
        /// <returns>UniTask，在下次 FixedUpdate 调用时完成</returns>
        UniTask FixedUpdateAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 FixedUpdate 接口实现
    /// 通过部分类的方式，为每个 Unity 消息类型实现对应的异步处理接口
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncFixedUpdateHandler
    {
        /// <summary>
        /// 实现 IAsyncFixedUpdateHandler 接口，等待下一次 FixedUpdate 调用
        /// </summary>
        UniTask IAsyncFixedUpdateHandler.FixedUpdateAsync()
        {
            core.Reset(); // 重置核心状态
            return new UniTask((IUniTaskSource)(object)this, core.Version); // 创建新的 UniTask
        }
    }

    /// <summary>
    /// 异步触发器扩展方法类
    /// 提供便捷的扩展方法，用于在 GameObject 或 Component 上获取对应的触发器
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncFixedUpdateTrigger 组件到指定的 GameObject
        /// </summary>
        /// <param name="gameObject">目标游戏对象</param>
        /// <returns>AsyncFixedUpdateTrigger 组件实例</returns>
        public static AsyncFixedUpdateTrigger GetAsyncFixedUpdateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncFixedUpdateTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncFixedUpdateTrigger 组件到指定 Component 所在的 GameObject
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>AsyncFixedUpdateTrigger 组件实例</returns>
        public static AsyncFixedUpdateTrigger GetAsyncFixedUpdateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncFixedUpdateTrigger();
        }
    }

    /// <summary>
    /// FixedUpdate 异步触发器
    /// 将 Unity 的 FixedUpdate 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - 在 Unity 的固定时间步长更新循环中被调用（通常用于物理计算）
    /// - 每次 FixedUpdate 调用时会触发等待的任务完成
    /// - 支持取消令牌，可以在任务等待期间取消
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncFixedUpdateTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 FixedUpdate 回调方法
        /// 当 Unity 调用 FixedUpdate 时，触发所有等待的异步任务
        /// </summary>
        void FixedUpdate()
        {
            RaiseEvent(AsyncUnit.Default); // 触发事件，通知所有等待的任务
        }

        /// <summary>
        /// 获取 FixedUpdate 异步处理器（不使用 callOnce 模式）
        /// callOnce = false 表示可以多次等待 FixedUpdate
        /// </summary>
        /// <returns>IAsyncFixedUpdateHandler 接口实例</returns>
        public IAsyncFixedUpdateHandler GetFixedUpdateAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 FixedUpdate 异步处理器（不使用 callOnce 模式，带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncFixedUpdateHandler 接口实例</returns>
        public IAsyncFixedUpdateHandler GetFixedUpdateAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 FixedUpdate 调用（使用 callOnce 模式）
        /// callOnce = true 表示创建新的处理器实例，每次调用都是新的等待
        /// </summary>
        /// <returns>UniTask，在下次 FixedUpdate 调用时完成</returns>
        public UniTask FixedUpdateAsync()
        {
            return ((IAsyncFixedUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).FixedUpdateAsync();
        }

        /// <summary>
        /// 等待下一次 FixedUpdate 调用（使用 callOnce 模式，带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>UniTask，在下次 FixedUpdate 调用时完成，或在取消时取消</returns>
        public UniTask FixedUpdateAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncFixedUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).FixedUpdateAsync();
        }
    }
#endregion
#region LateUpdate

    /// <summary>
    /// LateUpdate 异步处理接口
    /// 提供将 Unity 的 LateUpdate 消息转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncLateUpdateHandler
    {
        /// <summary>
        /// 等待下一次 LateUpdate 调用
        /// </summary>
        /// <returns>UniTask，在下次 LateUpdate 调用时完成</returns>
        UniTask LateUpdateAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 LateUpdate 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncLateUpdateHandler
    {
        /// <summary>
        /// 实现 IAsyncLateUpdateHandler 接口
        /// </summary>
        UniTask IAsyncLateUpdateHandler.LateUpdateAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncLateUpdateTrigger 组件到指定的 GameObject
        /// </summary>
        public static AsyncLateUpdateTrigger GetAsyncLateUpdateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncLateUpdateTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncLateUpdateTrigger 组件到指定 Component 所在的 GameObject
        /// </summary>
        public static AsyncLateUpdateTrigger GetAsyncLateUpdateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncLateUpdateTrigger();
        }
    }

    /// <summary>
    /// LateUpdate 异步触发器
    /// 将 Unity 的 LateUpdate 消息转换为可等待的异步任务
    /// LateUpdate 在所有 Update 方法调用完成后执行，通常用于相机跟随、UI 更新等
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncLateUpdateTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 LateUpdate 回调方法
        /// </summary>
        void LateUpdate()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 LateUpdate 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncLateUpdateHandler GetLateUpdateAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 LateUpdate 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncLateUpdateHandler GetLateUpdateAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 LateUpdate 调用
        /// </summary>
        public UniTask LateUpdateAsync()
        {
            return ((IAsyncLateUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).LateUpdateAsync();
        }

        /// <summary>
        /// 等待下一次 LateUpdate 调用（带取消令牌）
        /// </summary>
        public UniTask LateUpdateAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncLateUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).LateUpdateAsync();
        }
    }
#endregion
#region Update

    /// <summary>
    /// Update 异步处理接口
    /// 提供将 Unity 的 Update 消息转换为可等待的异步任务的能力
    /// Update 是 Unity 中最常用的更新循环，每帧调用一次
    /// </summary>
    public interface IAsyncUpdateHandler
    {
        /// <summary>
        /// 等待下一次 Update 调用
        /// </summary>
        /// <returns>UniTask，在下次 Update 调用时完成</returns>
        UniTask UpdateAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 Update 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncUpdateHandler
    {
        /// <summary>
        /// 实现 IAsyncUpdateHandler 接口
        /// </summary>
        UniTask IAsyncUpdateHandler.UpdateAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncUpdateTrigger 组件
        /// Update 是 Unity 中最常用的更新触发器
        /// </summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncUpdateTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncUpdateTrigger 组件
        /// </summary>
        public static AsyncUpdateTrigger GetAsyncUpdateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncUpdateTrigger();
        }
    }

    /// <summary>
    /// Update 异步触发器
    /// 将 Unity 的 Update 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - Update 在每帧渲染前调用，是 Unity 中最常用的更新循环
    /// - 可以用于实现帧级别的异步等待，例如等待下一帧执行某个操作
    /// - 适合用于需要每帧检查条件、延迟一帧执行等场景
    /// 
    /// 使用示例：
    /// <code>
    /// // 等待下一帧
    /// await gameObject.GetAsyncUpdateTrigger().UpdateAsync();
    /// 
    /// // 延迟 N 帧
    /// for (int i = 0; i &lt; 5; i++)
    /// {
    ///     await gameObject.GetAsyncUpdateTrigger().UpdateAsync();
    /// }
    /// </code>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncUpdateTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 Update 回调方法
        /// 每帧调用一次，触发所有等待的异步任务
        /// </summary>
        void Update()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 Update 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncUpdateHandler GetUpdateAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 Update 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncUpdateHandler GetUpdateAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 Update 调用（下一帧）
        /// </summary>
        /// <returns>UniTask，在下次 Update 调用时完成</returns>
        public UniTask UpdateAsync()
        {
            return ((IAsyncUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).UpdateAsync();
        }

        /// <summary>
        /// 等待下一次 Update 调用（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>UniTask，在下次 Update 调用时完成，或在取消时取消</returns>
        public UniTask UpdateAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).UpdateAsync();
        }
    }
#endregion
}