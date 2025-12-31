#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region Disable

    /// <summary>
    /// Disable 异步处理接口
    /// 提供将 Unity 的 OnDisable 消息转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnDisableHandler
    {
        /// <summary>
        /// 等待下一次 OnDisable 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnDisable 调用时完成</returns>
        UniTask OnDisableAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnDisable 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnDisableHandler
    {
        /// <summary>
        /// 实现 IAsyncOnDisableHandler 接口
        /// </summary>
        UniTask IAsyncOnDisableHandler.OnDisableAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncDisableTrigger 组件
        /// </summary>
        public static AsyncDisableTrigger GetAsyncDisableTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDisableTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncDisableTrigger 组件
        /// </summary>
        public static AsyncDisableTrigger GetAsyncDisableTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDisableTrigger();
        }
    }

    /// <summary>
    /// Disable 异步触发器
    /// 将 Unity 的 OnDisable 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnDisable 在 GameObject 或 Component 被禁用时调用
    /// - 可以用于等待对象被停用，例如等待 UI 面板隐藏
    /// 
    /// 使用示例：
    /// <code>
    /// // 等待对象被禁用
    /// await gameObject.GetAsyncDisableTrigger().OnDisableAsync();
    /// Debug.Log("对象已禁用");
    /// </code>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncDisableTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnDisable 回调方法
        /// </summary>
        void OnDisable()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnDisable 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnDisableHandler GetOnDisableAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnDisable 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnDisableHandler GetOnDisableAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnDisable 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnDisable 调用时完成</returns>
        public UniTask OnDisableAsync()
        {
            return ((IAsyncOnDisableHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnDisableAsync();
        }

        /// <summary>
        /// 等待下一次 OnDisable 调用（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask，在下次 OnDisable 调用时完成</returns>
        public UniTask OnDisableAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnDisableHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnDisableAsync();
        }
    }
#endregion
#region Enable

    /// <summary>
    /// Enable 异步处理接口
    /// 提供将 Unity 的 OnEnable 消息转换为可等待的异步任务的能力
    /// </summary>
    public interface IAsyncOnEnableHandler
    {
        /// <summary>
        /// 等待下一次 OnEnable 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnEnable 调用时完成</returns>
        UniTask OnEnableAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnEnable 接口实现
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnEnableHandler
    {
        /// <summary>
        /// 实现 IAsyncOnEnableHandler 接口
        /// </summary>
        UniTask IAsyncOnEnableHandler.OnEnableAsync()
        {
            core.Reset();
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncEnableTrigger 组件
        /// </summary>
        public static AsyncEnableTrigger GetAsyncEnableTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncEnableTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncEnableTrigger 组件
        /// </summary>
        public static AsyncEnableTrigger GetAsyncEnableTrigger(this Component component)
        {
            return component.gameObject.GetAsyncEnableTrigger();
        }
    }

    /// <summary>
    /// Enable 异步触发器
    /// 将 Unity 的 OnEnable 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnEnable 在 GameObject 或 Component 被启用时调用
    /// - 可以用于等待对象被激活，例如等待 UI 面板显示
    /// 
    /// 使用示例：
    /// <code>
    /// // 等待对象被启用
    /// await gameObject.GetAsyncEnableTrigger().OnEnableAsync();
    /// Debug.Log("对象已启用");
    /// </code>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncEnableTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnEnable 回调方法
        /// </summary>
        void OnEnable()
        {
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnEnable 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnEnableHandler GetOnEnableAsyncHandler()
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnEnable 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnEnableHandler GetOnEnableAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnEnable 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnEnable 调用时完成</returns>
        public UniTask OnEnableAsync()
        {
            return ((IAsyncOnEnableHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnEnableAsync();
        }

        /// <summary>
        /// 等待下一次 OnEnable 调用（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask，在下次 OnEnable 调用时完成</returns>
        public UniTask OnEnableAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnEnableHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnEnableAsync();
        }
    }
#endregion
#region Validate

    /// <summary>
    /// OnValidate 异步处理接口
    /// 提供将 Unity 的 OnValidate 消息转换为可等待的异步任务的能力
    /// OnValidate 在 Inspector 中修改组件属性时调用，或在脚本重新编译后调用
    /// </summary>
    public interface IAsyncOnValidateHandler
    {
        /// <summary>
        /// 等待下一次 OnValidate 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnValidate 调用时完成</returns>
        UniTask OnValidateAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnValidate 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnValidate 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnValidateHandler
    {
        /// <summary>
        /// 实现 IAsyncOnValidateHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在下次 OnValidate 调用时完成
        /// </summary>
        UniTask IAsyncOnValidateHandler.OnValidateAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask，绑定到当前处理器实例和版本号
            // 版本号用于检测任务是否已被新的等待替换，确保任务的有效性
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncValidateTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncValidateTrigger 组件实例</returns>
        public static AsyncValidateTrigger GetAsyncValidateTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncValidateTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncValidateTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncValidateTrigger 组件实例</returns>
        public static AsyncValidateTrigger GetAsyncValidateTrigger(this Component component)
        {
            return component.gameObject.GetAsyncValidateTrigger();
        }
    }

    /// <summary>
    /// OnValidate 异步触发器
    /// 将 Unity 的 OnValidate 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnValidate 在 Inspector 中修改组件属性值时调用，或在脚本重新编译后调用
    /// - 这是一个编辑器时间的方法，不会在运行时被调用（除非在编辑器中运行）
    /// - 常用于验证和修正 Inspector 中的属性值，确保数据的有效性
    /// 
    /// 使用示例：
    /// <code>
    /// // 等待组件属性被验证（通常在编辑器中）
    /// await component.GetAsyncValidateTrigger().OnValidateAsync();
    /// Debug.Log("组件已通过验证");
    /// </code>
    /// 
    /// 注意事项：
    /// - 此触发器主要用于编辑器工具和 Inspector 相关的异步操作
    /// - 在运行时可能不会触发，除非在编辑器中运行游戏
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncValidateTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnValidate 回调方法
        /// 当在 Inspector 中修改组件属性，或脚本重新编译后，Unity 会自动调用此方法
        /// </summary>
        void OnValidate()
        {
            // 触发事件，通知所有等待 OnValidateAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnValidate 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnValidate 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnValidateHandler 接口实例</returns>
        public IAsyncOnValidateHandler GetOnValidateAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnValidate 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnValidateHandler 接口实例</returns>
        public IAsyncOnValidateHandler GetOnValidateAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnValidate 调用
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在下次 OnValidate 调用时完成</returns>
        public UniTask OnValidateAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnValidateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnValidateAsync();
        }

        /// <summary>
        /// 等待下一次 OnValidate 调用（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在下次 OnValidate 调用时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnValidateAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnValidateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnValidateAsync();
        }
    }
#endregion
#region Reset

    /// <summary>
    /// Reset 异步处理接口
    /// 提供将 Unity 的 Reset 消息转换为可等待的异步任务的能力
    /// Reset 在组件被添加到 GameObject，或在 Inspector 中点击 Reset 按钮时调用
    /// </summary>
    public interface IAsyncResetHandler
    {
        /// <summary>
        /// 等待下一次 Reset 调用
        /// </summary>
        /// <returns>UniTask，在下次 Reset 调用时完成</returns>
        UniTask ResetAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 Reset 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 Reset 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncResetHandler
    {
        /// <summary>
        /// 实现 IAsyncResetHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在下次 Reset 调用时完成
        /// </summary>
        UniTask IAsyncResetHandler.ResetAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask，绑定到当前处理器实例和版本号
            // 版本号用于检测任务是否已被新的等待替换，确保任务的有效性
            return new UniTask((IUniTaskSource)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncResetTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncResetTrigger 组件实例</returns>
        public static AsyncResetTrigger GetAsyncResetTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncResetTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncResetTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncResetTrigger 组件实例</returns>
        public static AsyncResetTrigger GetAsyncResetTrigger(this Component component)
        {
            return component.gameObject.GetAsyncResetTrigger();
        }
    }

    /// <summary>
    /// Reset 异步触发器
    /// 将 Unity 的 Reset 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - Reset 在组件首次被添加到 GameObject 时调用，或在 Inspector 中点击 Reset 按钮时调用
    /// - 通常用于将组件的属性重置为默认值
    /// - 这是一个编辑器时间的方法，主要用于初始化组件状态
    /// 
    /// 使用示例：
    /// <code>
    /// // 等待组件被重置（通常在编辑器中）
    /// await component.GetAsyncResetTrigger().ResetAsync();
    /// Debug.Log("组件已被重置");
    /// </code>
    /// 
    /// 注意事项：
    /// - 此触发器主要用于编辑器工具和组件初始化相关的异步操作
    /// - 在运行时通常不会触发，除非在编辑器中操作组件
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncResetTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 Reset 回调方法
        /// 当组件被添加到 GameObject，或在 Inspector 中点击 Reset 按钮时，Unity 会自动调用此方法
        /// </summary>
        void Reset()
        {
            // 触发事件，通知所有等待 ResetAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 Reset 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 Reset 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncResetHandler 接口实例</returns>
        public IAsyncResetHandler GetResetAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 Reset 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncResetHandler 接口实例</returns>
        public IAsyncResetHandler GetResetAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 Reset 调用
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在下次 Reset 调用时完成</returns>
        public UniTask ResetAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncResetHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).ResetAsync();
        }

        /// <summary>
        /// 等待下一次 Reset 调用（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在下次 Reset 调用时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask ResetAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncResetHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).ResetAsync();
        }
    }
#endregion
}