#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region ApplicationFocus

    /// <summary>
    /// 应用焦点异步处理接口
    /// 提供将 Unity 的 OnApplicationFocus 消息转换为可等待的异步任务的能力
    /// 返回布尔值，表示应用是否获得焦点
    /// </summary>
    public interface IAsyncOnApplicationFocusHandler
    {
        /// <summary>
        /// 等待下一次应用焦点变化
        /// </summary>
        /// <returns>UniTask&lt;bool&gt;，在焦点变化时完成，返回 true 表示获得焦点，false 表示失去焦点</returns>
        UniTask<bool> OnApplicationFocusAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnApplicationFocus 接口实现
    /// 返回类型为 UniTask&lt;bool&gt;
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnApplicationFocusHandler
    {
        /// <summary>
        /// 实现 IAsyncOnApplicationFocusHandler 接口
        /// </summary>
        UniTask<bool> IAsyncOnApplicationFocusHandler.OnApplicationFocusAsync()
        {
            core.Reset();
            return new UniTask<bool>((IUniTaskSource<bool>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncApplicationFocusTrigger 组件
        /// </summary>
        public static AsyncApplicationFocusTrigger GetAsyncApplicationFocusTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncApplicationFocusTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncApplicationFocusTrigger 组件
        /// </summary>
        public static AsyncApplicationFocusTrigger GetAsyncApplicationFocusTrigger(this Component component)
        {
            return component.gameObject.GetAsyncApplicationFocusTrigger();
        }
    }

    /// <summary>
    /// 应用焦点异步触发器
    /// 将 Unity 的 OnApplicationFocus 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnApplicationFocus 在应用获得或失去焦点时调用（例如用户切换窗口、最小化应用）
    /// - 返回的布尔值表示应用是否获得焦点：true 为获得焦点，false 为失去焦点
    /// - 常用于处理应用暂停/恢复时的逻辑，如暂停游戏、保存数据等
    /// 
    /// 使用示例：
    /// <code>
    /// var hasFocus = await gameObject.GetAsyncApplicationFocusTrigger().OnApplicationFocusAsync();
    /// if (hasFocus)
    /// {
    ///     Debug.Log("应用获得焦点");
    /// }
    /// else
    /// {
    ///     Debug.Log("应用失去焦点");
    /// }
    /// </code>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncApplicationFocusTrigger : AsyncTriggerBase<bool>
    {
        /// <summary>
        /// Unity 的 OnApplicationFocus 回调方法
        /// </summary>
        /// <param name="hasFocus">true 表示应用获得焦点，false 表示失去焦点</param>
        void OnApplicationFocus(bool hasFocus)
        {
            RaiseEvent(hasFocus); // 传递焦点状态给等待的任务
        }

        /// <summary>
        /// 获取 OnApplicationFocus 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnApplicationFocusHandler GetOnApplicationFocusAsyncHandler()
        {
            return new AsyncTriggerHandler<bool>(this, false);
        }

        /// <summary>
        /// 获取 OnApplicationFocus 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnApplicationFocusHandler GetOnApplicationFocusAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<bool>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次应用焦点变化，并获取焦点状态
        /// </summary>
        /// <returns>UniTask&lt;bool&gt;，包含焦点状态</returns>
        public UniTask<bool> OnApplicationFocusAsync()
        {
            return ((IAsyncOnApplicationFocusHandler)new AsyncTriggerHandler<bool>(this, true)).OnApplicationFocusAsync();
        }

        /// <summary>
        /// 等待下一次应用焦点变化（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;bool&gt;，包含焦点状态</returns>
        public UniTask<bool> OnApplicationFocusAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnApplicationFocusHandler)new AsyncTriggerHandler<bool>(this, cancellationToken, true)).OnApplicationFocusAsync();
        }
    }
#endregion
#region ApplicationPause

    /// <summary>
    /// 应用暂停异步处理接口
    /// 提供将 Unity 的 OnApplicationPause 消息转换为可等待的异步任务的能力
    /// 返回布尔值，表示应用是否处于暂停状态（true=暂停，false=恢复）
    /// </summary>
    public interface IAsyncOnApplicationPauseHandler
    {
        /// <summary>
        /// 等待下一次应用暂停状态变化
        /// </summary>
        /// <returns>UniTask&lt;bool&gt;，在暂停状态变化时完成，返回 true 表示应用暂停，false 表示应用恢复</returns>
        UniTask<bool> OnApplicationPauseAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnApplicationPause 接口实现
    /// 返回类型为 UniTask&lt;bool&gt;，会传递暂停状态
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnApplicationPauseHandler
    {
        /// <summary>
        /// 实现 IAsyncOnApplicationPauseHandler 接口
        /// 重置核心状态并返回一个新的 UniTask&lt;bool&gt;，该任务会在下次 OnApplicationPause 调用时完成
        /// </summary>
        UniTask<bool> IAsyncOnApplicationPauseHandler.OnApplicationPauseAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask&lt;bool&gt;，绑定到当前处理器实例和版本号
            // 版本号用于检测任务是否已被新的等待替换，确保任务的有效性
            return new UniTask<bool>((IUniTaskSource<bool>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncApplicationPauseTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncApplicationPauseTrigger 组件实例</returns>
        public static AsyncApplicationPauseTrigger GetAsyncApplicationPauseTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncApplicationPauseTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncApplicationPauseTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncApplicationPauseTrigger 组件实例</returns>
        public static AsyncApplicationPauseTrigger GetAsyncApplicationPauseTrigger(this Component component)
        {
            return component.gameObject.GetAsyncApplicationPauseTrigger();
        }
    }

    /// <summary>
    /// 应用暂停异步触发器
    /// 将 Unity 的 OnApplicationPause 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnApplicationPause 在应用进入后台（暂停）或返回前台（恢复）时调用
    /// - 在移动设备上，当用户按 Home 键或切换到其他应用时会触发暂停
    /// - 返回的布尔值表示应用是否处于暂停状态：true 为暂停，false 为恢复
    /// - 常用于保存游戏数据、暂停游戏逻辑、停止不必要的更新等场景
    /// 
    /// 使用示例：
    /// <code>
    /// var isPaused = await gameObject.GetAsyncApplicationPauseTrigger().OnApplicationPauseAsync();
    /// if (isPaused)
    /// {
    ///     Debug.Log("应用已暂停，保存游戏数据...");
    ///     SaveGame();
    /// }
    /// else
    /// {
    ///     Debug.Log("应用已恢复");
    /// }
    /// </code>
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncApplicationPauseTrigger : AsyncTriggerBase<bool>
    {
        /// <summary>
        /// Unity 的 OnApplicationPause 回调方法
        /// 当应用进入后台或返回前台时，Unity 会自动调用此方法
        /// </summary>
        /// <param name="pauseStatus">true 表示应用正在暂停（进入后台），false 表示应用正在恢复（返回前台）</param>
        void OnApplicationPause(bool pauseStatus)
        {
            // 触发事件，将暂停状态传递给等待 OnApplicationPauseAsync 的任务
            RaiseEvent((pauseStatus));
        }

        /// <summary>
        /// 获取 OnApplicationPause 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnApplicationPauseHandler 接口实例</returns>
        public IAsyncOnApplicationPauseHandler GetOnApplicationPauseAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，类型参数为 bool（传递暂停状态），callOnce 设置为 false
            return new AsyncTriggerHandler<bool>(this, false);
        }

        /// <summary>
        /// 获取 OnApplicationPause 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnApplicationPauseHandler 接口实例</returns>
        public IAsyncOnApplicationPauseHandler GetOnApplicationPauseAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<bool>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次应用暂停状态变化，并获取暂停状态
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask&lt;bool&gt;，包含暂停状态（true=暂停，false=恢复）</returns>
        public UniTask<bool> OnApplicationPauseAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnApplicationPauseHandler)new AsyncTriggerHandler<bool>(this, true)).OnApplicationPauseAsync();
        }

        /// <summary>
        /// 等待下一次应用暂停状态变化（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask&lt;bool&gt;，包含暂停状态，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask<bool> OnApplicationPauseAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnApplicationPauseHandler)new AsyncTriggerHandler<bool>(this, cancellationToken, true)).OnApplicationPauseAsync();
        }
    }
#endregion
#region ApplicationQuit

    /// <summary>
    /// 应用退出异步处理接口
    /// 提供将 Unity 的 OnApplicationQuit 消息转换为可等待的异步任务的能力
    /// 在应用退出前执行清理操作
    /// </summary>
    public interface IAsyncOnApplicationQuitHandler
    {
        /// <summary>
        /// 等待应用退出事件
        /// </summary>
        /// <returns>UniTask，在应用退出时完成</returns>
        UniTask OnApplicationQuitAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnApplicationQuit 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnApplicationQuit 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnApplicationQuitHandler
    {
        /// <summary>
        /// 实现 IAsyncOnApplicationQuitHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在应用退出时完成
        /// </summary>
        UniTask IAsyncOnApplicationQuitHandler.OnApplicationQuitAsync()
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
        /// 获取或添加 AsyncApplicationQuitTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncApplicationQuitTrigger 组件实例</returns>
        public static AsyncApplicationQuitTrigger GetAsyncApplicationQuitTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncApplicationQuitTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncApplicationQuitTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncApplicationQuitTrigger 组件实例</returns>
        public static AsyncApplicationQuitTrigger GetAsyncApplicationQuitTrigger(this Component component)
        {
            return component.gameObject.GetAsyncApplicationQuitTrigger();
        }
    }

    /// <summary>
    /// 应用退出异步触发器
    /// 将 Unity 的 OnApplicationQuit 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnApplicationQuit 在应用退出前调用，这是执行清理操作的最后机会
    /// - 在编辑器中，停止播放时会调用此方法；在构建的应用程序中，关闭应用时会调用
    /// - 常用于保存数据、关闭网络连接、释放资源等清理操作
    /// - 注意：在此方法中执行的操作应该是快速完成的，因为应用即将退出
    /// 
    /// 使用示例：
    /// <code>
    /// await gameObject.GetAsyncApplicationQuitTrigger().OnApplicationQuitAsync();
    /// Debug.Log("应用正在退出，保存数据...");
    /// SaveGameData();
    /// </code>
    /// 
    /// 注意事项：
    /// - 在应用退出时，Unity 可能不会等待所有异步操作完成
    /// - 建议在 OnApplicationQuit 中执行同步的清理操作，或使用关键的数据保存机制
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncApplicationQuitTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnApplicationQuit 回调方法
        /// 当应用退出时，Unity 会自动调用此方法
        /// </summary>
        void OnApplicationQuit()
        {
            // 触发事件，通知所有等待 OnApplicationQuitAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnApplicationQuit 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnApplicationQuit 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnApplicationQuitHandler 接口实例</returns>
        public IAsyncOnApplicationQuitHandler GetOnApplicationQuitAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnApplicationQuit 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// 注意：由于应用退出是确定性事件，取消令牌通常不会生效
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作（在应用退出场景中可能不生效）</param>
        /// <returns>IAsyncOnApplicationQuitHandler 接口实例</returns>
        public IAsyncOnApplicationQuitHandler GetOnApplicationQuitAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待应用退出事件
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在应用退出时完成</returns>
        public UniTask OnApplicationQuitAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnApplicationQuitHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnApplicationQuitAsync();
        }

        /// <summary>
        /// 等待应用退出事件（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，但在应用退出场景中通常不适用
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在应用退出时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnApplicationQuitAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnApplicationQuitHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnApplicationQuitAsync();
        }
    }
#endregion
#region ServerInitialized

    /// <summary>
    /// 服务器初始化异步处理接口
    /// 提供将 Unity Network 的 OnServerInitialized 消息转换为可等待的异步任务的能力
    /// 在服务器初始化完成时触发，用于网络游戏的服务器端逻辑
    /// </summary>
    public interface IAsyncOnServerInitializedHandler
    {
        /// <summary>
        /// 等待服务器初始化完成
        /// </summary>
        /// <returns>UniTask，在服务器初始化完成时完成</returns>
        UniTask OnServerInitializedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnServerInitialized 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnServerInitialized 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnServerInitializedHandler
    {
        /// <summary>
        /// 实现 IAsyncOnServerInitializedHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在服务器初始化完成时完成
        /// </summary>
        UniTask IAsyncOnServerInitializedHandler.OnServerInitializedAsync()
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
        /// 获取或添加 AsyncServerInitializedTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncServerInitializedTrigger 组件实例</returns>
        public static AsyncServerInitializedTrigger GetAsyncServerInitializedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncServerInitializedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncServerInitializedTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncServerInitializedTrigger 组件实例</returns>
        public static AsyncServerInitializedTrigger GetAsyncServerInitializedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncServerInitializedTrigger();
        }
    }

    /// <summary>
    /// 服务器初始化异步触发器
    /// 将 Unity Network 的 OnServerInitialized 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnServerInitialized 在 Unity Network 服务器初始化完成时调用
    /// - 仅在使用 Unity 的旧版网络系统（Unity Networking/UNet）时有效
    /// - 服务器初始化完成后，可以开始接受客户端连接、初始化游戏世界等操作
    /// 
    /// 使用示例：
    /// <code>
    /// await networkManager.GetAsyncServerInitializedTrigger().OnServerInitializedAsync();
    /// Debug.Log("服务器已初始化，开始接受客户端连接");
    /// StartAcceptingClients();
    /// </code>
    /// 
    /// 注意事项：
    /// - 此触发器仅在使用 Unity 的旧版网络系统时有效
    /// - Unity 推荐使用新的 Netcode for GameObjects 或 Mirror Networking，这些系统可能有不同的初始化机制
    /// - 如果项目不使用 Unity 网络系统，此触发器可能永远不会触发
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncServerInitializedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity Network 的 OnServerInitialized 回调方法
        /// 当 Unity Network 服务器初始化完成时，Unity 会自动调用此方法
        /// </summary>
        void OnServerInitialized()
        {
            // 触发事件，通知所有等待 OnServerInitializedAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnServerInitialized 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnServerInitialized 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnServerInitializedHandler 接口实例</returns>
        public IAsyncOnServerInitializedHandler GetOnServerInitializedAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnServerInitialized 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnServerInitializedHandler 接口实例</returns>
        public IAsyncOnServerInitializedHandler GetOnServerInitializedAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待服务器初始化完成
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在服务器初始化完成时完成</returns>
        public UniTask OnServerInitializedAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnServerInitializedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnServerInitializedAsync();
        }

        /// <summary>
        /// 等待服务器初始化完成（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在服务器初始化完成时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnServerInitializedAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnServerInitializedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnServerInitializedAsync();
        }
    }
#endregion
}