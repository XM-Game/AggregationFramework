#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region BeginDrag
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 开始拖拽异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnBeginDrag 事件转换为可等待的异步任务的能力
    /// 返回指针事件数据，包含拖拽开始的位置和相关信息
    /// </summary>
    public interface IAsyncOnBeginDragHandler
    {
        /// <summary>
        /// 等待下一次拖拽开始事件
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在开始拖拽时完成，返回事件数据</returns>
        UniTask<PointerEventData> OnBeginDragAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnBeginDrag 接口实现
    /// 返回类型为 UniTask&lt;PointerEventData&gt;
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnBeginDragHandler
    {
        /// <summary>
        /// 实现 IAsyncOnBeginDragHandler 接口
        /// </summary>
        UniTask<PointerEventData> IAsyncOnBeginDragHandler.OnBeginDragAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask&lt;PointerEventData&gt;，绑定到当前处理器实例和版本号
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncBeginDragTrigger 组件到指定的 GameObject
        /// 注意：此触发器需要 UGUI 系统支持（通过条件编译宏控制）
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncBeginDragTrigger 组件实例</returns>
        public static AsyncBeginDragTrigger GetAsyncBeginDragTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncBeginDragTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncBeginDragTrigger 组件到指定 Component 所属的 GameObject
        /// </summary>
        /// <param name="component">目标 Component</param>
        /// <returns>AsyncBeginDragTrigger 组件实例</returns>
        public static AsyncBeginDragTrigger GetAsyncBeginDragTrigger(this Component component)
        {
            return component.gameObject.GetAsyncBeginDragTrigger();
        }
    }

    /// <summary>
    /// 开始拖拽异步触发器（UGUI）
    /// 将 UGUI 的 OnBeginDrag 事件转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnBeginDrag 在用户开始拖拽 UI 元素时触发（按下鼠标/触摸后开始移动）
    /// - 需要目标 GameObject 上存在实现 IBeginDragHandler 的组件
    /// - 返回的 PointerEventData 包含拖拽开始的位置、指针信息等
    /// - 常用于实现拖拽系统，如背包系统、UI 排序等
    /// 
    /// 使用示例：
    /// <code>
    /// var eventData = await draggableItem.GetAsyncBeginDragTrigger().OnBeginDragAsync();
    /// Debug.Log($"开始拖拽，位置: {eventData.position}");
    /// // 创建拖拽预览、记录初始位置等
    /// </code>
    /// 
    /// 注意：
    /// - 此触发器同时实现 IBeginDragHandler 接口，可直接接收 Unity 的事件系统调用
    /// - 仅在支持 UGUI 系统的平台/Unity 版本下编译
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncBeginDragTrigger : AsyncTriggerBase<PointerEventData>, IBeginDragHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnBeginDrag 回调方法
        /// 当用户开始拖拽 UI 元素时，Unity 事件系统会调用此方法
        /// </summary>
        /// <param name="eventData">指针事件数据，包含拖拽开始的位置等信息</param>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            // 触发事件，将事件数据传递给等待 OnBeginDragAsync 的任务
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnBeginDrag 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnBeginDragHandler GetOnBeginDragAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnBeginDrag 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnBeginDragHandler GetOnBeginDragAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次拖拽开始事件，并获取事件数据
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，包含拖拽事件数据</returns>
        public UniTask<PointerEventData> OnBeginDragAsync()
        {
            return ((IAsyncOnBeginDragHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnBeginDragAsync();
        }

        /// <summary>
        /// 等待下一次拖拽开始事件（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;PointerEventData&gt;，包含拖拽事件数据</returns>
        public UniTask<PointerEventData> OnBeginDragAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnBeginDragHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnBeginDragAsync();
        }
    }
#endif
#endregion
#region Cancel
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 取消操作异步处理接口（UGUI）
    /// 将 UGUI 的 OnCancel 事件转换为可等待的异步任务
    /// </summary>
    public interface IAsyncOnCancelHandler
    {
        /// <summary>
        /// 等待下一次取消事件（通常对应返回/取消按键）
        /// </summary>
        /// <returns>UniTask&lt;BaseEventData&gt;，在触发取消时完成</returns>
        UniTask<BaseEventData> OnCancelAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnCancel 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnCancelHandler
    {
        /// <inheritdoc />
        UniTask<BaseEventData> IAsyncOnCancelHandler.OnCancelAsync()
        {
            core.Reset();
            return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：取消事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncCancelTrigger 组件到 GameObject
        /// </summary>
        public static AsyncCancelTrigger GetAsyncCancelTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncCancelTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncCancelTrigger 组件到 Component 所在的 GameObject
        /// </summary>
        public static AsyncCancelTrigger GetAsyncCancelTrigger(this Component component)
        {
            return component.gameObject.GetAsyncCancelTrigger();
        }
    }

    /// <summary>
    /// 取消事件异步触发器（UGUI）
    /// 监听 OnCancel 并以 UniTask 形式暴露
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncCancelTrigger : AsyncTriggerBase<BaseEventData>, ICancelHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnCancel 回调
        /// </summary>
        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnCancel 异步处理器
        /// </summary>
        public IAsyncOnCancelHandler GetOnCancelAsyncHandler()
        {
            return new AsyncTriggerHandler<BaseEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnCancel 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnCancelHandler GetOnCancelAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次取消事件
        /// </summary>
        public UniTask<BaseEventData> OnCancelAsync()
        {
            return ((IAsyncOnCancelHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnCancelAsync();
        }

        /// <summary>
        /// 等待下一次取消事件（带取消令牌）
        /// </summary>
        public UniTask<BaseEventData> OnCancelAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnCancelHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnCancelAsync();
        }
    }
#endif
#endregion
#region Deselect
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 失去选择异步处理接口（UGUI）
    /// 用于等待 EventSystem 的 OnDeselect 事件
    /// </summary>
    public interface IAsyncOnDeselectHandler
    {
        /// <summary>
        /// 等待下一次失焦/取消选择事件
        /// </summary>
        UniTask<BaseEventData> OnDeselectAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnDeselect 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnDeselectHandler
    {
        /// <inheritdoc />
        UniTask<BaseEventData> IAsyncOnDeselectHandler.OnDeselectAsync()
        {
            core.Reset();
            return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：失去选择事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDeselectTrigger 组件到 GameObject
        /// </summary>
        public static AsyncDeselectTrigger GetAsyncDeselectTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDeselectTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDeselectTrigger 组件到 Component 所在的 GameObject
        /// </summary>
        public static AsyncDeselectTrigger GetAsyncDeselectTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDeselectTrigger();
        }
    }

    /// <summary>
    /// 失去选择异步触发器（UGUI）
    /// 将 OnDeselect 事件转为可等待任务
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncDeselectTrigger : AsyncTriggerBase<BaseEventData>, IDeselectHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnDeselect 回调
        /// </summary>
        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnDeselect 异步处理器
        /// </summary>
        public IAsyncOnDeselectHandler GetOnDeselectAsyncHandler()
        {
            return new AsyncTriggerHandler<BaseEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnDeselect 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnDeselectHandler GetOnDeselectAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次失去选择事件
        /// </summary>
        public UniTask<BaseEventData> OnDeselectAsync()
        {
            return ((IAsyncOnDeselectHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnDeselectAsync();
        }

        /// <summary>
        /// 等待下一次失去选择事件（带取消令牌）
        /// </summary>
        public UniTask<BaseEventData> OnDeselectAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDeselectHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnDeselectAsync();
        }
    }
#endif
#endregion
#region Drag
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 拖拽中异步处理接口（UGUI）
    /// 将 OnDrag 事件转换为可等待的异步任务
    /// </summary>
    public interface IAsyncOnDragHandler
    {
        /// <summary>
        /// 等待下一次拖拽中事件
        /// </summary>
        UniTask<PointerEventData> OnDragAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnDrag 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnDragHandler
    {
        /// <inheritdoc />
        UniTask<PointerEventData> IAsyncOnDragHandler.OnDragAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：拖拽中事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDragTrigger 组件
        /// </summary>
        public static AsyncDragTrigger GetAsyncDragTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDragTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDragTrigger 组件
        /// </summary>
        public static AsyncDragTrigger GetAsyncDragTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDragTrigger();
        }
    }

    /// <summary>
    /// 拖拽中异步触发器（UGUI）
    /// 将 OnDrag 事件封装为 UniTask
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncDragTrigger : AsyncTriggerBase<PointerEventData>, IDragHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnDrag 回调
        /// </summary>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnDrag 异步处理器
        /// </summary>
        public IAsyncOnDragHandler GetOnDragAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnDrag 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnDragHandler GetOnDragAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次拖拽中事件
        /// </summary>
        public UniTask<PointerEventData> OnDragAsync()
        {
            return ((IAsyncOnDragHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnDragAsync();
        }

        /// <summary>
        /// 等待下一次拖拽中事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnDragAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDragHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnDragAsync();
        }
    }
#endif
#endregion
#region Drop
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 拖拽释放异步处理接口（UGUI）
    /// 将 OnDrop 事件转换为可等待的异步任务
    /// </summary>
    public interface IAsyncOnDropHandler
    {
        /// <summary>
        /// 等待下一次释放事件
        /// </summary>
        UniTask<PointerEventData> OnDropAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnDrop 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnDropHandler
    {
        /// <inheritdoc />
        UniTask<PointerEventData> IAsyncOnDropHandler.OnDropAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：释放事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDropTrigger 组件
        /// </summary>
        public static AsyncDropTrigger GetAsyncDropTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDropTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDropTrigger 组件
        /// </summary>
        public static AsyncDropTrigger GetAsyncDropTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDropTrigger();
        }
    }

    /// <summary>
    /// 释放（Drop）异步触发器（UGUI）
    /// 将 OnDrop 事件暴露为 UniTask
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncDropTrigger : AsyncTriggerBase<PointerEventData>, IDropHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnDrop 回调
        /// </summary>
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnDrop 异步处理器
        /// </summary>
        public IAsyncOnDropHandler GetOnDropAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnDrop 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnDropHandler GetOnDropAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次释放事件
        /// </summary>
        public UniTask<PointerEventData> OnDropAsync()
        {
            return ((IAsyncOnDropHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnDropAsync();
        }

        /// <summary>
        /// 等待下一次释放事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnDropAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDropHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnDropAsync();
        }
    }
#endif
#endregion
#region EndDrag
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 拖拽结束异步处理接口（UGUI）
    /// </summary>
    public interface IAsyncOnEndDragHandler
    {
        /// <summary>
        /// 等待下一次拖拽结束事件
        /// </summary>
        UniTask<PointerEventData> OnEndDragAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnEndDrag 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnEndDragHandler
    {
        /// <inheritdoc />
        UniTask<PointerEventData> IAsyncOnEndDragHandler.OnEndDragAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：拖拽结束事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncEndDragTrigger 组件
        /// </summary>
        public static AsyncEndDragTrigger GetAsyncEndDragTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncEndDragTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncEndDragTrigger 组件
        /// </summary>
        public static AsyncEndDragTrigger GetAsyncEndDragTrigger(this Component component)
        {
            return component.gameObject.GetAsyncEndDragTrigger();
        }
    }

    /// <summary>
    /// 拖拽结束异步触发器（UGUI）
    /// 将 OnEndDrag 事件转为可等待任务
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncEndDragTrigger : AsyncTriggerBase<PointerEventData>, IEndDragHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnEndDrag 回调
        /// </summary>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnEndDrag 异步处理器
        /// </summary>
        public IAsyncOnEndDragHandler GetOnEndDragAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnEndDrag 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnEndDragHandler GetOnEndDragAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次拖拽结束事件
        /// </summary>
        public UniTask<PointerEventData> OnEndDragAsync()
        {
            return ((IAsyncOnEndDragHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnEndDragAsync();
        }

        /// <summary>
        /// 等待下一次拖拽结束事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnEndDragAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnEndDragHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnEndDragAsync();
        }
    }
#endif
#endregion
#region InitializePotentialDrag
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 潜在拖拽初始化异步处理接口（UGUI）
    /// </summary>
    public interface IAsyncOnInitializePotentialDragHandler
    {
        /// <summary>
        /// 等待下一次潜在拖拽初始化事件
        /// </summary>
        UniTask<PointerEventData> OnInitializePotentialDragAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnInitializePotentialDrag 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnInitializePotentialDragHandler
    {
        /// <inheritdoc />
        UniTask<PointerEventData> IAsyncOnInitializePotentialDragHandler.OnInitializePotentialDragAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：潜在拖拽初始化事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncInitializePotentialDragTrigger 组件
        /// </summary>
        public static AsyncInitializePotentialDragTrigger GetAsyncInitializePotentialDragTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncInitializePotentialDragTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncInitializePotentialDragTrigger 组件
        /// </summary>
        public static AsyncInitializePotentialDragTrigger GetAsyncInitializePotentialDragTrigger(this Component component)
        {
            return component.gameObject.GetAsyncInitializePotentialDragTrigger();
        }
    }

    /// <summary>
    /// 潜在拖拽初始化异步触发器（UGUI）
    /// 用于捕获 OnInitializePotentialDrag 事件
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncInitializePotentialDragTrigger : AsyncTriggerBase<PointerEventData>, IInitializePotentialDragHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnInitializePotentialDrag 回调
        /// </summary>
        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnInitializePotentialDrag 异步处理器
        /// </summary>
        public IAsyncOnInitializePotentialDragHandler GetOnInitializePotentialDragAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnInitializePotentialDrag 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnInitializePotentialDragHandler GetOnInitializePotentialDragAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次潜在拖拽初始化事件
        /// </summary>
        public UniTask<PointerEventData> OnInitializePotentialDragAsync()
        {
            return ((IAsyncOnInitializePotentialDragHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnInitializePotentialDragAsync();
        }

        /// <summary>
        /// 等待下一次潜在拖拽初始化事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnInitializePotentialDragAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnInitializePotentialDragHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnInitializePotentialDragAsync();
        }
    }
#endif
#endregion
#region Move
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 移动（导航）异步处理接口（UGUI）
    /// 对应方向键/手柄导航触发的 OnMove
    /// </summary>
    public interface IAsyncOnMoveHandler
    {
        /// <summary>
        /// 等待下一次导航移动事件
        /// </summary>
        UniTask<AxisEventData> OnMoveAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnMove 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnMoveHandler
    {
        /// <inheritdoc />
        UniTask<AxisEventData> IAsyncOnMoveHandler.OnMoveAsync()
        {
            core.Reset();
            return new UniTask<AxisEventData>((IUniTaskSource<AxisEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：导航移动事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncMoveTrigger 组件
        /// </summary>
        public static AsyncMoveTrigger GetAsyncMoveTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncMoveTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncMoveTrigger 组件
        /// </summary>
        public static AsyncMoveTrigger GetAsyncMoveTrigger(this Component component)
        {
            return component.gameObject.GetAsyncMoveTrigger();
        }
    }

    /// <summary>
    /// 导航移动异步触发器（UGUI）
    /// 对应方向键/手柄导航的 OnMove 事件
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncMoveTrigger : AsyncTriggerBase<AxisEventData>, IMoveHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnMove 回调
        /// </summary>
        void IMoveHandler.OnMove(AxisEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnMove 异步处理器
        /// </summary>
        public IAsyncOnMoveHandler GetOnMoveAsyncHandler()
        {
            return new AsyncTriggerHandler<AxisEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnMove 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnMoveHandler GetOnMoveAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AxisEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次导航移动事件
        /// </summary>
        public UniTask<AxisEventData> OnMoveAsync()
        {
            return ((IAsyncOnMoveHandler)new AsyncTriggerHandler<AxisEventData>(this, true)).OnMoveAsync();
        }

        /// <summary>
        /// 等待下一次导航移动事件（带取消令牌）
        /// </summary>
        public UniTask<AxisEventData> OnMoveAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnMoveHandler)new AsyncTriggerHandler<AxisEventData>(this, cancellationToken, true)).OnMoveAsync();
        }
    }
#endif
#endregion
#region PointerClick
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 指针点击异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnPointerClick 事件转换为可等待的异步任务的能力
    /// 返回指针事件数据，包含点击位置、点击按钮、点击次数等信息
    /// </summary>
    public interface IAsyncOnPointerClickHandler
    {
        /// <summary>
        /// 等待下一次指针点击事件
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在发生点击时完成，返回事件数据</returns>
        UniTask<PointerEventData> OnPointerClickAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnPointerClick 接口实现
    /// 返回类型为 UniTask&lt;PointerEventData&gt;
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnPointerClickHandler
    {
        /// <summary>
        /// 实现 IAsyncOnPointerClickHandler 接口
        /// </summary>
        UniTask<PointerEventData> IAsyncOnPointerClickHandler.OnPointerClickAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPointerClickTrigger 组件
        /// 注意：此触发器需要 UGUI 系统支持（通过条件编译宏控制）
        /// </summary>
        public static AsyncPointerClickTrigger GetAsyncPointerClickTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPointerClickTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPointerClickTrigger 组件
        /// </summary>
        public static AsyncPointerClickTrigger GetAsyncPointerClickTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPointerClickTrigger();
        }
    }

    /// <summary>
    /// 指针点击异步触发器（UGUI）
    /// 将 UGUI 的 OnPointerClick 事件转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - 当 UI 元素被点击（鼠标或触摸）时触发
    /// - 需要目标 GameObject 上存在实现 IPointerClickHandler 的组件
    /// - 返回的 PointerEventData 包含点击的详细信息（位置、按钮、点击次数等）
    /// 
    /// 使用示例：
    /// <code>
    /// var eventData = await button.GetAsyncPointerClickTrigger().OnPointerClickAsync();
    /// Debug.Log($"点击位置: {eventData.position}");
    /// Debug.Log($"点击按钮: {eventData.button}");
    /// </code>
    /// 
    /// 注意：
    /// - 此触发器同时实现 IPointerClickHandler 接口，可直接接收 Unity 的事件系统调用
    /// - 仅在支持 UGUI 系统的平台/Unity 版本下编译
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncPointerClickTrigger : AsyncTriggerBase<PointerEventData>, IPointerClickHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnPointerClick 回调方法
        /// 当 UI 元素被点击时，Unity 事件系统会调用此方法
        /// </summary>
        /// <param name="eventData">指针事件数据，包含点击位置、按钮等信息</param>
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            RaiseEvent(eventData); // 传递事件数据给等待的任务
        }

        /// <summary>
        /// 获取 OnPointerClick 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnPointerClickHandler GetOnPointerClickAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnPointerClick 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnPointerClickHandler GetOnPointerClickAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次指针点击事件，并获取事件数据
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，包含点击事件数据</returns>
        public UniTask<PointerEventData> OnPointerClickAsync()
        {
            return ((IAsyncOnPointerClickHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerClickAsync();
        }

        /// <summary>
        /// 等待下一次指针点击事件（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;PointerEventData&gt;，包含点击事件数据</returns>
        public UniTask<PointerEventData> OnPointerClickAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPointerClickHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerClickAsync();
        }
    }
#endif
#endregion
#region PointerDown
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 指针按下异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnPointerDown 事件转换为可等待的异步任务的能力
    /// 返回指针事件数据，包含按下位置、按下按钮等信息
    /// </summary>
    public interface IAsyncOnPointerDownHandler
    {
        /// <summary>
        /// 等待下一次指针按下事件
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在发生按下时完成，返回事件数据</returns>
        UniTask<PointerEventData> OnPointerDownAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnPointerDown 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnPointerDownHandler
    {
        UniTask<PointerEventData> IAsyncOnPointerDownHandler.OnPointerDownAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPointerDownTrigger 组件
        /// </summary>
        public static AsyncPointerDownTrigger GetAsyncPointerDownTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPointerDownTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPointerDownTrigger 组件
        /// </summary>
        public static AsyncPointerDownTrigger GetAsyncPointerDownTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPointerDownTrigger();
        }
    }

    /// <summary>
    /// 指针按下异步触发器（UGUI）
    /// 将 UGUI 的 OnPointerDown 事件转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnPointerDown 在指针（鼠标或触摸）按下 UI 元素时触发
    /// - 需要目标 GameObject 上存在实现 IPointerDownHandler 的组件
    /// - 返回的 PointerEventData 包含按下的位置、按钮等信息
    /// - 常用于按钮按下效果、拖拽开始检测等场景
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncPointerDownTrigger : AsyncTriggerBase<PointerEventData>, IPointerDownHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnPointerDown 回调方法
        /// </summary>
        /// <param name="eventData">指针事件数据</param>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnPointerDown 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnPointerDownHandler GetOnPointerDownAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnPointerDown 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnPointerDownHandler GetOnPointerDownAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次指针按下事件，并获取事件数据
        /// </summary>
        public UniTask<PointerEventData> OnPointerDownAsync()
        {
            return ((IAsyncOnPointerDownHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerDownAsync();
        }

        /// <summary>
        /// 等待下一次指针按下事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnPointerDownAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPointerDownHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerDownAsync();
        }
    }
#endif
#endregion
#region PointerEnter
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 指针进入异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnPointerEnter 事件转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnPointerEnterHandler
    {
        /// <summary>
        /// 等待指针进入 UI 元素
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在指针进入时完成</returns>
        UniTask<PointerEventData> OnPointerEnterAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnPointerEnter 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnPointerEnterHandler
    {
        UniTask<PointerEventData> IAsyncOnPointerEnterHandler.OnPointerEnterAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPointerEnterTrigger 组件
        /// </summary>
        public static AsyncPointerEnterTrigger GetAsyncPointerEnterTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPointerEnterTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPointerEnterTrigger 组件
        /// </summary>
        public static AsyncPointerEnterTrigger GetAsyncPointerEnterTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPointerEnterTrigger();
        }
    }

    /// <summary>
    /// 指针进入异步触发器（UGUI）
    /// 将 UGUI 的 OnPointerEnter 事件转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnPointerEnter 在指针（鼠标或触摸）进入 UI 元素时触发
    /// - 常用于悬停效果、提示信息显示等场景
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncPointerEnterTrigger : AsyncTriggerBase<PointerEventData>, IPointerEnterHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnPointerEnter 回调方法
        /// </summary>
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnPointerEnter 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnPointerEnterHandler GetOnPointerEnterAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnPointerEnter 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnPointerEnterHandler GetOnPointerEnterAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待指针进入 UI 元素，并获取事件数据
        /// </summary>
        public UniTask<PointerEventData> OnPointerEnterAsync()
        {
            return ((IAsyncOnPointerEnterHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerEnterAsync();
        }

        /// <summary>
        /// 等待指针进入 UI 元素（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnPointerEnterAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPointerEnterHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerEnterAsync();
        }
    }
#endif
#endregion
#region PointerExit
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 指针离开异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnPointerExit 事件转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnPointerExitHandler
    {
        /// <summary>
        /// 等待指针离开 UI 元素
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在指针离开时完成</returns>
        UniTask<PointerEventData> OnPointerExitAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnPointerExit 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnPointerExitHandler
    {
        UniTask<PointerEventData> IAsyncOnPointerExitHandler.OnPointerExitAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPointerExitTrigger 组件
        /// </summary>
        public static AsyncPointerExitTrigger GetAsyncPointerExitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPointerExitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPointerExitTrigger 组件
        /// </summary>
        public static AsyncPointerExitTrigger GetAsyncPointerExitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPointerExitTrigger();
        }
    }

    /// <summary>
    /// 指针离开异步触发器（UGUI）
    /// 将 UGUI 的 OnPointerExit 事件转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnPointerExit 在指针（鼠标或触摸）离开 UI 元素时触发
    /// - 常用于取消悬停效果、隐藏提示信息等场景
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncPointerExitTrigger : AsyncTriggerBase<PointerEventData>, IPointerExitHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnPointerExit 回调方法
        /// </summary>
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnPointerExit 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnPointerExitHandler GetOnPointerExitAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnPointerExit 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnPointerExitHandler GetOnPointerExitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待指针离开 UI 元素，并获取事件数据
        /// </summary>
        public UniTask<PointerEventData> OnPointerExitAsync()
        {
            return ((IAsyncOnPointerExitHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerExitAsync();
        }

        /// <summary>
        /// 等待指针离开 UI 元素（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnPointerExitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPointerExitHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerExitAsync();
        }
    }
#endif
#endregion
#region PointerUp
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
    /// <summary>
    /// 指针抬起异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnPointerUp 事件转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnPointerUpHandler
    {
        /// <summary>
        /// 等待指针抬起事件
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在指针抬起时完成</returns>
        UniTask<PointerEventData> OnPointerUpAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnPointerUp 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnPointerUpHandler
    {
        UniTask<PointerEventData> IAsyncOnPointerUpHandler.OnPointerUpAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncPointerUpTrigger 组件
        /// </summary>
        public static AsyncPointerUpTrigger GetAsyncPointerUpTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncPointerUpTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncPointerUpTrigger 组件
        /// </summary>
        public static AsyncPointerUpTrigger GetAsyncPointerUpTrigger(this Component component)
        {
            return component.gameObject.GetAsyncPointerUpTrigger();
        }
    }

    /// <summary>
    /// 指针抬起异步触发器（UGUI）
    /// 将 UGUI 的 OnPointerUp 事件转换为可等待的异步任务
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncPointerUpTrigger : AsyncTriggerBase<PointerEventData>, IPointerUpHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnPointerUp 回调方法
        /// </summary>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnPointerUp 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnPointerUpHandler GetOnPointerUpAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnPointerUp 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnPointerUpHandler GetOnPointerUpAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待指针抬起事件，并获取事件数据
        /// </summary>
        public UniTask<PointerEventData> OnPointerUpAsync()
        {
            return ((IAsyncOnPointerUpHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerUpAsync();
        }

        /// <summary>
        /// 等待指针抬起事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnPointerUpAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnPointerUpHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerUpAsync();
        }
    }
#endif
#endregion
#region Scroll
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 滚动异步处理接口（UGUI）
    /// 提供将 UGUI 的 OnScroll 事件转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnScrollHandler
    {
        /// <summary>
        /// 等待滚动事件
        /// </summary>
        /// <returns>UniTask&lt;PointerEventData&gt;，在滚动时完成</returns>
        UniTask<PointerEventData> OnScrollAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnScroll 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnScrollHandler
    {
        /// <summary>
        /// 实现 IAsyncOnScrollHandler 接口
        /// </summary>
        UniTask<PointerEventData> IAsyncOnScrollHandler.OnScrollAsync()
        {
            core.Reset();
            return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncScrollTrigger 组件
        /// </summary>
        public static AsyncScrollTrigger GetAsyncScrollTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncScrollTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncScrollTrigger 组件
        /// </summary>
        public static AsyncScrollTrigger GetAsyncScrollTrigger(this Component component)
        {
            return component.gameObject.GetAsyncScrollTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncScrollTrigger : AsyncTriggerBase<PointerEventData>, IScrollHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnScroll 回调方法
        /// </summary>
        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnScroll 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnScrollHandler GetOnScrollAsyncHandler()
        {
            return new AsyncTriggerHandler<PointerEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnScroll 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnScrollHandler GetOnScrollAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待滚动事件，并获取事件数据
        /// </summary>
        public UniTask<PointerEventData> OnScrollAsync()
        {
            return ((IAsyncOnScrollHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnScrollAsync();
        }

        /// <summary>
        /// 等待滚动事件（带取消令牌）
        /// </summary>
        public UniTask<PointerEventData> OnScrollAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnScrollHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnScrollAsync();
        }
    }
#endif
#endregion
#region Select
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 选择（获得焦点）异步处理接口（UGUI）
    /// </summary>
    public interface IAsyncOnSelectHandler
    {
        /// <summary>
        /// 等待下一次选择事件
        /// </summary>
        UniTask<BaseEventData> OnSelectAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnSelect 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnSelectHandler
    {
        /// <inheritdoc />
        UniTask<BaseEventData> IAsyncOnSelectHandler.OnSelectAsync()
        {
            core.Reset();
            return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：选择事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncSelectTrigger 组件
        /// </summary>
        public static AsyncSelectTrigger GetAsyncSelectTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncSelectTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncSelectTrigger 组件
        /// </summary>
        public static AsyncSelectTrigger GetAsyncSelectTrigger(this Component component)
        {
            return component.gameObject.GetAsyncSelectTrigger();
        }
    }

    /// <summary>
    /// 选择异步触发器（UGUI）
    /// 将 OnSelect 事件暴露为 UniTask
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncSelectTrigger : AsyncTriggerBase<BaseEventData>, ISelectHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnSelect 回调
        /// </summary>
        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnSelect 异步处理器
        /// </summary>
        public IAsyncOnSelectHandler GetOnSelectAsyncHandler()
        {
            return new AsyncTriggerHandler<BaseEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnSelect 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnSelectHandler GetOnSelectAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次选择事件
        /// </summary>
        public UniTask<BaseEventData> OnSelectAsync()
        {
            return ((IAsyncOnSelectHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnSelectAsync();
        }

        /// <summary>
        /// 等待下一次选择事件（带取消令牌）
        /// </summary>
        public UniTask<BaseEventData> OnSelectAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnSelectHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnSelectAsync();
        }
    }
#endif
#endregion
#region Submit
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 提交（Confirm）异步处理接口（UGUI）
    /// </summary>
    public interface IAsyncOnSubmitHandler
    {
        /// <summary>
        /// 等待下一次提交事件
        /// </summary>
        UniTask<BaseEventData> OnSubmitAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnSubmit 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnSubmitHandler
    {
        /// <inheritdoc />
        UniTask<BaseEventData> IAsyncOnSubmitHandler.OnSubmitAsync()
        {
            core.Reset();
            return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：提交事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncSubmitTrigger 组件
        /// </summary>
        public static AsyncSubmitTrigger GetAsyncSubmitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncSubmitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncSubmitTrigger 组件
        /// </summary>
        public static AsyncSubmitTrigger GetAsyncSubmitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncSubmitTrigger();
        }
    }

    /// <summary>
    /// 提交异步触发器（UGUI）
    /// 将 OnSubmit 事件转为可等待任务
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncSubmitTrigger : AsyncTriggerBase<BaseEventData>, ISubmitHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnSubmit 回调
        /// </summary>
        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnSubmit 异步处理器
        /// </summary>
        public IAsyncOnSubmitHandler GetOnSubmitAsyncHandler()
        {
            return new AsyncTriggerHandler<BaseEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnSubmit 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnSubmitHandler GetOnSubmitAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次提交事件
        /// </summary>
        public UniTask<BaseEventData> OnSubmitAsync()
        {
            return ((IAsyncOnSubmitHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnSubmitAsync();
        }

        /// <summary>
        /// 等待下一次提交事件（带取消令牌）
        /// </summary>
        public UniTask<BaseEventData> OnSubmitAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnSubmitHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnSubmitAsync();
        }
    }
#endif
#endregion
#region UpdateSelected
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT

    /// <summary>
    /// 更新选择状态异步处理接口（UGUI）
    /// 对应 EventSystem 的 OnUpdateSelected
    /// </summary>
    public interface IAsyncOnUpdateSelectedHandler
    {
        /// <summary>
        /// 等待下一次选中项更新事件
        /// </summary>
        UniTask<BaseEventData> OnUpdateSelectedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 对 OnUpdateSelected 的接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnUpdateSelectedHandler
    {
        /// <inheritdoc />
        UniTask<BaseEventData> IAsyncOnUpdateSelectedHandler.OnUpdateSelectedAsync()
        {
            core.Reset();
            return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展：选中项更新事件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncUpdateSelectedTrigger 组件
        /// </summary>
        public static AsyncUpdateSelectedTrigger GetAsyncUpdateSelectedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncUpdateSelectedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncUpdateSelectedTrigger 组件
        /// </summary>
        public static AsyncUpdateSelectedTrigger GetAsyncUpdateSelectedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncUpdateSelectedTrigger();
        }
    }

    /// <summary>
    /// 选中项更新异步触发器（UGUI）
    /// 将 OnUpdateSelected 事件转为 UniTask
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncUpdateSelectedTrigger : AsyncTriggerBase<BaseEventData>, IUpdateSelectedHandler
    {
        /// <summary>
        /// Unity 事件系统的 OnUpdateSelected 回调
        /// </summary>
        void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
        {
            RaiseEvent((eventData));
        }

        /// <summary>
        /// 获取 OnUpdateSelected 异步处理器
        /// </summary>
        public IAsyncOnUpdateSelectedHandler GetOnUpdateSelectedAsyncHandler()
        {
            return new AsyncTriggerHandler<BaseEventData>(this, false);
        }

        /// <summary>
        /// 获取 OnUpdateSelected 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnUpdateSelectedHandler GetOnUpdateSelectedAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次选中项更新事件
        /// </summary>
        public UniTask<BaseEventData> OnUpdateSelectedAsync()
        {
            return ((IAsyncOnUpdateSelectedHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnUpdateSelectedAsync();
        }

        /// <summary>
        /// 等待下一次选中项更新事件（带取消令牌）
        /// </summary>
        public UniTask<BaseEventData> OnUpdateSelectedAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnUpdateSelectedHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnUpdateSelectedAsync();
        }
    }
#endif
#endregion
}