#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
#if !UNITY_2019_1_OR_NEWER || UNITASK_UGUI_SUPPORT
using UnityEngine.EventSystems;
#endif

namespace Cysharp.Threading.Tasks.Triggers
{
#region AnimatorIK

    /// <summary>
    /// AnimatorIK 异步处理接口
    /// 提供将 Unity 的 OnAnimatorIK 消息转换为可等待的异步任务的能力
    /// 返回动画层的索引值
    /// </summary>
    public interface IAsyncOnAnimatorIKHandler
    {
        /// <summary>
        /// 等待下一次 OnAnimatorIK 调用
        /// </summary>
        /// <returns>UniTask&lt;int&gt;，在下次 OnAnimatorIK 调用时完成，返回动画层索引</returns>
        UniTask<int> OnAnimatorIKAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnAnimatorIK 接口实现
    /// 注意：此触发器返回 int 类型（动画层索引），而不是 AsyncUnit
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnAnimatorIKHandler
    {
        /// <summary>
        /// 实现 IAsyncOnAnimatorIKHandler 接口
        /// 返回类型为 UniTask&lt;int&gt;，会传递动画层索引
        /// </summary>
        UniTask<int> IAsyncOnAnimatorIKHandler.OnAnimatorIKAsync()
        {
            core.Reset();
            return new UniTask<int>((IUniTaskSource<int>)(object)this, core.Version);
        }
    }

    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncAnimatorIKTrigger 组件
        /// </summary>
        public static AsyncAnimatorIKTrigger GetAsyncAnimatorIKTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncAnimatorIKTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncAnimatorIKTrigger 组件
        /// </summary>
        public static AsyncAnimatorIKTrigger GetAsyncAnimatorIKTrigger(this Component component)
        {
            return component.gameObject.GetAsyncAnimatorIKTrigger();
        }
    }

    /// <summary>
    /// AnimatorIK 异步触发器
    /// 将 Unity 的 OnAnimatorIK 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnAnimatorIK 用于处理动画的逆向运动学（IK）计算
    /// - 每次调用时会传递动画层索引（layerIndex）
    /// - 返回的 UniTask 会包含该层索引值
    /// 
    /// 使用示例：
    /// <code>
    /// var layerIndex = await animator.GetAsyncAnimatorIKTrigger().OnAnimatorIKAsync();
    /// Debug.Log($"IK called for layer: {layerIndex}");
    /// </code>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AsyncAnimatorIKTrigger : AsyncTriggerBase<int>
    {
        /// <summary>
        /// Unity 的 OnAnimatorIK 回调方法
        /// </summary>
        /// <param name="layerIndex">动画层索引</param>
        void OnAnimatorIK(int layerIndex)
        {
            RaiseEvent(layerIndex); // 传递层索引给等待的任务
        }

        /// <summary>
        /// 获取 OnAnimatorIK 异步处理器（不使用 callOnce 模式）
        /// </summary>
        public IAsyncOnAnimatorIKHandler GetOnAnimatorIKAsyncHandler()
        {
            return new AsyncTriggerHandler<int>(this, false);
        }

        /// <summary>
        /// 获取 OnAnimatorIK 异步处理器（带取消令牌）
        /// </summary>
        public IAsyncOnAnimatorIKHandler GetOnAnimatorIKAsyncHandler(CancellationToken cancellationToken)
        {
            return new AsyncTriggerHandler<int>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnAnimatorIK 调用，并获取动画层索引
        /// </summary>
        /// <returns>UniTask&lt;int&gt;，包含动画层索引</returns>
        public UniTask<int> OnAnimatorIKAsync()
        {
            return ((IAsyncOnAnimatorIKHandler)new AsyncTriggerHandler<int>(this, true)).OnAnimatorIKAsync();
        }

        /// <summary>
        /// 等待下一次 OnAnimatorIK 调用（带取消令牌）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask&lt;int&gt;，包含动画层索引</returns>
        public UniTask<int> OnAnimatorIKAsync(CancellationToken cancellationToken)
        {
            return ((IAsyncOnAnimatorIKHandler)new AsyncTriggerHandler<int>(this, cancellationToken, true)).OnAnimatorIKAsync();
        }
    }
#endregion
#region AnimatorMove

    /// <summary>
    /// AnimatorMove 异步处理接口
    /// 提供将 Unity 的 OnAnimatorMove 消息转换为可等待的异步任务的能力
    /// 在 Animator 处理完根运动后调用，用于处理基于根运动的角色移动
    /// </summary>
    public interface IAsyncOnAnimatorMoveHandler
    {
        /// <summary>
        /// 等待下一次 OnAnimatorMove 调用
        /// </summary>
        /// <returns>UniTask，在下次 OnAnimatorMove 调用时完成</returns>
        UniTask OnAnimatorMoveAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnAnimatorMove 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnAnimatorMove 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnAnimatorMoveHandler
    {
        /// <summary>
        /// 实现 IAsyncOnAnimatorMoveHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在下次 OnAnimatorMove 调用时完成
        /// </summary>
        UniTask IAsyncOnAnimatorMoveHandler.OnAnimatorMoveAsync()
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
        /// 获取或添加 AsyncAnimatorMoveTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncAnimatorMoveTrigger 组件实例</returns>
        public static AsyncAnimatorMoveTrigger GetAsyncAnimatorMoveTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncAnimatorMoveTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncAnimatorMoveTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncAnimatorMoveTrigger 组件实例</returns>
        public static AsyncAnimatorMoveTrigger GetAsyncAnimatorMoveTrigger(this Component component)
        {
            return component.gameObject.GetAsyncAnimatorMoveTrigger();
        }
    }

    /// <summary>
    /// AnimatorMove 异步触发器
    /// 将 Unity 的 OnAnimatorMove 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnAnimatorMove 在 Animator 处理完根运动（Root Motion）后，在 Update 之后、LateUpdate 之前调用
    /// - 用于处理基于动画根运动的角色移动，通常与 Animator.ApplyRootMotion 配合使用
    /// - 在此方法中，可以通过 animator.deltaPosition 和 animator.deltaRotation 获取根运动的变化量
    /// - 常用于第三人称角色控制器、NPC 移动等场景
    /// 
    /// 使用示例：
    /// <code>
    /// await animator.GetAsyncAnimatorMoveTrigger().OnAnimatorMoveAsync();
    /// // 获取根运动的变化量并应用到角色移动
    /// Vector3 deltaPos = animator.deltaPosition;
    /// Quaternion deltaRot = animator.deltaRotation;
    /// characterController.Move(deltaPos);
    /// transform.rotation *= deltaRot;
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 Animator 组件已启用 Apply Root Motion
    /// - 此方法每帧都会被调用（如果 Animator 有根运动）
    /// - 不应在此方法中进行耗时操作，否则会影响性能
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncAnimatorMoveTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnAnimatorMove 回调方法
        /// 当 Animator 处理完根运动后，Unity 会自动调用此方法
        /// </summary>
        void OnAnimatorMove()
        {
            // 触发事件，通知所有等待 OnAnimatorMoveAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnAnimatorMove 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnAnimatorMove 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnAnimatorMoveHandler 接口实例</returns>
        public IAsyncOnAnimatorMoveHandler GetOnAnimatorMoveAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnAnimatorMove 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnAnimatorMoveHandler 接口实例</returns>
        public IAsyncOnAnimatorMoveHandler GetOnAnimatorMoveAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnAnimatorMove 调用
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在下次 OnAnimatorMove 调用时完成</returns>
        public UniTask OnAnimatorMoveAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnAnimatorMoveHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnAnimatorMoveAsync();
        }

        /// <summary>
        /// 等待下一次 OnAnimatorMove 调用（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在下次 OnAnimatorMove 调用时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnAnimatorMoveAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnAnimatorMoveHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnAnimatorMoveAsync();
        }
    }
#endregion
#region AudioFilterRead

    /// <summary>
    /// AudioFilterRead 异步处理接口
    /// 提供将 Unity 的 OnAudioFilterRead 消息转换为可等待的异步任务的能力
    /// 返回音频数据和通道数的元组，用于音频实时处理和特效
    /// </summary>
    public interface IAsyncOnAudioFilterReadHandler
    {
        /// <summary>
        /// 等待下一次 OnAudioFilterRead 调用
        /// </summary>
        /// <returns>UniTask&lt;(float[] data, int channels)&gt;，在下次 OnAudioFilterRead 调用时完成，返回音频数据数组和通道数</returns>
        UniTask<(float[] data, int channels)> OnAudioFilterReadAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnAudioFilterRead 接口实现
    /// 返回类型为 UniTask&lt;(float[] data, int channels)&gt;，会传递音频数据和通道数
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnAudioFilterReadHandler
    {
        /// <summary>
        /// 实现 IAsyncOnAudioFilterReadHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在下次 OnAudioFilterRead 调用时完成
        /// </summary>
        UniTask<(float[] data, int channels)> IAsyncOnAudioFilterReadHandler.OnAudioFilterReadAsync()
        {
            // 重置触发器核心状态，清除之前的等待状态
            core.Reset();
            // 创建并返回一个新的 UniTask，绑定到当前处理器实例和版本号
            // 版本号用于检测任务是否已被新的等待替换，确保任务的有效性
            return new UniTask<(float[] data, int channels)>((IUniTaskSource<(float[] data, int channels)>)(object)this, core.Version);
        }
    }

    /// <summary>
    /// 异步触发器扩展方法集合
    /// 提供便捷的方法来获取或创建触发器组件
    /// </summary>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取或添加 AsyncAudioFilterReadTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncAudioFilterReadTrigger 组件实例</returns>
        public static AsyncAudioFilterReadTrigger GetAsyncAudioFilterReadTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncAudioFilterReadTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncAudioFilterReadTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncAudioFilterReadTrigger 组件实例</returns>
        public static AsyncAudioFilterReadTrigger GetAsyncAudioFilterReadTrigger(this Component component)
        {
            return component.gameObject.GetAsyncAudioFilterReadTrigger();
        }
    }

    /// <summary>
    /// AudioFilterRead 异步触发器
    /// 将 Unity 的 OnAudioFilterRead 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnAudioFilterRead 在音频系统需要处理音频数据时调用，用于实时音频处理
    /// - 返回的元组包含：音频采样数据数组（float[]）和通道数（int）
    /// - 可以在回调中修改音频数据，实现实时音频特效，如均衡器、混响、失真等
    /// - 此方法在音频线程中调用，频率取决于音频采样率和缓冲区大小
    /// - 常用于实现自定义音频特效、音频可视化、实时音频分析等场景
    /// 
    /// 使用示例：
    /// <code>
    /// var (audioData, channels) = await audioSource.GetAsyncAudioFilterReadTrigger().OnAudioFilterReadAsync();
    /// // 对音频数据进行处理，例如添加低音增强效果
    /// for (int i = 0; i &lt; audioData.Length; i++)
    /// {
    ///     audioData[i] *= 1.2f; // 增强音频
    /// }
    /// </code>
    /// 
    /// 注意事项：
    /// - 此方法在音频线程中调用，必须快速执行，避免阻塞音频处理
    /// - 不应在此方法中进行耗时操作、Unity API 调用或分配大量内存
    /// - 修改后的 audioData 数组会直接用于音频输出
    /// - 需要确保 GameObject 上有 AudioSource 或 AudioListener 组件
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncAudioFilterReadTrigger : AsyncTriggerBase<(float[] data, int channels)>
    {
        /// <summary>
        /// Unity 的 OnAudioFilterRead 回调方法
        /// 当音频系统需要处理音频数据时，Unity 会自动调用此方法
        /// </summary>
        /// <param name="data">音频采样数据数组，包含当前音频块的采样值（范围通常在 -1.0 到 1.0 之间）</param>
        /// <param name="channels">音频通道数（1=单声道，2=立体声，6=5.1环绕声等）</param>
        void OnAudioFilterRead(float[] data, int channels)
        {
            // 触发事件，将音频数据和通道数传递给等待 OnAudioFilterReadAsync 的任务
            RaiseEvent((data, channels));
        }

        /// <summary>
        /// 获取 OnAudioFilterRead 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnAudioFilterReadHandler 接口实例</returns>
        public IAsyncOnAudioFilterReadHandler GetOnAudioFilterReadAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，类型参数为元组（传递音频数据和通道数），callOnce 设置为 false
            return new AsyncTriggerHandler<(float[] data, int channels)>(this, false);
        }

        /// <summary>
        /// 获取 OnAudioFilterRead 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnAudioFilterReadHandler 接口实例</returns>
        public IAsyncOnAudioFilterReadHandler GetOnAudioFilterReadAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<(float[] data, int channels)>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待下一次 OnAudioFilterRead 调用，并获取音频数据和通道数
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask&lt;(float[] data, int channels)&gt;，包含音频数据数组和通道数</returns>
        public UniTask<(float[] data, int channels)> OnAudioFilterReadAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnAudioFilterReadHandler)new AsyncTriggerHandler<(float[] data, int channels)>(this, true)).OnAudioFilterReadAsync();
        }

        /// <summary>
        /// 等待下一次 OnAudioFilterRead 调用（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask&lt;(float[] data, int channels)&gt;，包含音频数据数组和通道数，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask<(float[] data, int channels)> OnAudioFilterReadAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnAudioFilterReadHandler)new AsyncTriggerHandler<(float[] data, int channels)>(this, cancellationToken, true)).OnAudioFilterReadAsync();
        }
    }
#endregion
#region BecameInvisible

    /// <summary>
    /// BecameInvisible 异步处理接口
    /// 提供将 Unity 的 OnBecameInvisible 消息转换为可等待的异步任务的能力
    /// 当对象变得不可见时触发，用于渲染优化和可见性检测
    /// </summary>
    public interface IAsyncOnBecameInvisibleHandler
    {
        /// <summary>
        /// 等待对象变为不可见
        /// </summary>
        /// <returns>UniTask，在对象变为不可见时完成</returns>
        UniTask OnBecameInvisibleAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnBecameInvisible 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnBecameInvisible 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnBecameInvisibleHandler
    {
        /// <summary>
        /// 实现 IAsyncOnBecameInvisibleHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在对象变为不可见时完成
        /// </summary>
        UniTask IAsyncOnBecameInvisibleHandler.OnBecameInvisibleAsync()
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
        /// 获取或添加 AsyncBecameInvisibleTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncBecameInvisibleTrigger 组件实例</returns>
        public static AsyncBecameInvisibleTrigger GetAsyncBecameInvisibleTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncBecameInvisibleTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncBecameInvisibleTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncBecameInvisibleTrigger 组件实例</returns>
        public static AsyncBecameInvisibleTrigger GetAsyncBecameInvisibleTrigger(this Component component)
        {
            return component.gameObject.GetAsyncBecameInvisibleTrigger();
        }
    }

    /// <summary>
    /// BecameInvisible 异步触发器
    /// 将 Unity 的 OnBecameInvisible 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnBecameInvisible 当对象的任何 Renderer 组件变得对任何摄像机不可见时调用
    /// - 不可见指对象移出摄像机的视锥体，或被其他对象完全遮挡
    /// - 常用于性能优化：当对象不可见时停止更新、暂停动画、禁用 AI 等
    /// - 也用于实现对象池管理、动态加载卸载等功能
    /// 
    /// 使用示例：
    /// <code>
    /// await renderer.GetAsyncBecameInvisibleTrigger().OnBecameInvisibleAsync();
    /// Debug.Log("对象已不可见，停止更新");
    /// StopUpdating(); // 停止不必要的更新以提高性能
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 GameObject 上有 Renderer 组件（如 MeshRenderer、SpriteRenderer 等）
    /// - 对象可能在场景中但仍不可见（被遮挡或移出视锥体）
    /// - 此方法不会在对象被禁用时调用，只会在可见性状态改变时调用
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncBecameInvisibleTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnBecameInvisible 回调方法
        /// 当对象的任何 Renderer 变得对任何摄像机不可见时，Unity 会自动调用此方法
        /// </summary>
        void OnBecameInvisible()
        {
            // 触发事件，通知所有等待 OnBecameInvisibleAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnBecameInvisible 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnBecameInvisible 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnBecameInvisibleHandler 接口实例</returns>
        public IAsyncOnBecameInvisibleHandler GetOnBecameInvisibleAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnBecameInvisible 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnBecameInvisibleHandler 接口实例</returns>
        public IAsyncOnBecameInvisibleHandler GetOnBecameInvisibleAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待对象变为不可见
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在对象变为不可见时完成</returns>
        public UniTask OnBecameInvisibleAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnBecameInvisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnBecameInvisibleAsync();
        }

        /// <summary>
        /// 等待对象变为不可见（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在对象变为不可见时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnBecameInvisibleAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnBecameInvisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnBecameInvisibleAsync();
        }
    }
#endregion
#region BecameVisible

    /// <summary>
    /// BecameVisible 异步处理接口
    /// 提供将 Unity 的 OnBecameVisible 消息转换为可等待的异步任务的能力
    /// 当对象变得可见时触发，用于可见性检测和资源初始化
    /// </summary>
    public interface IAsyncOnBecameVisibleHandler
    {
        /// <summary>
        /// 等待对象变为可见
        /// </summary>
        /// <returns>UniTask，在对象变为可见时完成</returns>
        UniTask OnBecameVisibleAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnBecameVisible 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnBecameVisible 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnBecameVisibleHandler
    {
        /// <summary>
        /// 实现 IAsyncOnBecameVisibleHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在对象变为可见时完成
        /// </summary>
        UniTask IAsyncOnBecameVisibleHandler.OnBecameVisibleAsync()
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
        /// 获取或添加 AsyncBecameVisibleTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncBecameVisibleTrigger 组件实例</returns>
        public static AsyncBecameVisibleTrigger GetAsyncBecameVisibleTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncBecameVisibleTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncBecameVisibleTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncBecameVisibleTrigger 组件实例</returns>
        public static AsyncBecameVisibleTrigger GetAsyncBecameVisibleTrigger(this Component component)
        {
            return component.gameObject.GetAsyncBecameVisibleTrigger();
        }
    }

    /// <summary>
    /// BecameVisible 异步触发器
    /// 将 Unity 的 OnBecameVisible 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnBecameVisible 当对象的任何 Renderer 组件对任何摄像机变为可见时调用
    /// - 可见指对象进入摄像机的视锥体，且不被完全遮挡
    /// - 常用于延迟初始化：当对象首次可见时加载资源、启动动画、激活 AI 等
    /// - 也用于实现对象池的延迟激活、LOD 系统、动态加载等功能
    /// 
    /// 使用示例：
    /// <code>
    /// await renderer.GetAsyncBecameVisibleTrigger().OnBecameVisibleAsync();
    /// Debug.Log("对象已可见，开始加载资源");
    /// await LoadResources(); // 延迟加载资源，提高初始加载速度
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 GameObject 上有 Renderer 组件（如 MeshRenderer、SpriteRenderer 等）
    /// - 对象可能在场景中被激活但不可见（未进入视锥体或被遮挡）
    /// - 此方法不会在对象被启用时调用，只会在可见性状态改变时调用
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncBecameVisibleTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnBecameVisible 回调方法
        /// 当对象的任何 Renderer 对任何摄像机变为可见时，Unity 会自动调用此方法
        /// </summary>
        void OnBecameVisible()
        {
            // 触发事件，通知所有等待 OnBecameVisibleAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnBecameVisible 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnBecameVisible 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnBecameVisibleHandler 接口实例</returns>
        public IAsyncOnBecameVisibleHandler GetOnBecameVisibleAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnBecameVisible 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnBecameVisibleHandler 接口实例</returns>
        public IAsyncOnBecameVisibleHandler GetOnBecameVisibleAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待对象变为可见
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在对象变为可见时完成</returns>
        public UniTask OnBecameVisibleAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnBecameVisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnBecameVisibleAsync();
        }

        /// <summary>
        /// 等待对象变为可见（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在对象变为可见时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnBecameVisibleAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnBecameVisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnBecameVisibleAsync();
        }
    }
#endregion
#region BeforeTransformParentChanged

    /// <summary>
    /// BeforeTransformParentChanged 异步处理接口
    /// 提供将 Unity 的 OnBeforeTransformParentChanged 消息转换为可等待的异步任务的能力
    /// 在 Transform 的父级改变之前调用，用于在父级改变前执行清理或保存操作
    /// </summary>
    public interface IAsyncOnBeforeTransformParentChangedHandler
    {
        /// <summary>
        /// 等待 Transform 父级改变前的通知
        /// </summary>
        /// <returns>UniTask，在 Transform 父级即将改变时完成</returns>
        UniTask OnBeforeTransformParentChangedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnBeforeTransformParentChanged 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnBeforeTransformParentChanged 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnBeforeTransformParentChangedHandler
    {
        /// <summary>
        /// 实现 IAsyncOnBeforeTransformParentChangedHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在 Transform 父级即将改变时完成
        /// </summary>
        UniTask IAsyncOnBeforeTransformParentChangedHandler.OnBeforeTransformParentChangedAsync()
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
        /// 获取或添加 AsyncBeforeTransformParentChangedTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncBeforeTransformParentChangedTrigger 组件实例</returns>
        public static AsyncBeforeTransformParentChangedTrigger GetAsyncBeforeTransformParentChangedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncBeforeTransformParentChangedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncBeforeTransformParentChangedTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncBeforeTransformParentChangedTrigger 组件实例</returns>
        public static AsyncBeforeTransformParentChangedTrigger GetAsyncBeforeTransformParentChangedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncBeforeTransformParentChangedTrigger();
        }
    }

    /// <summary>
    /// BeforeTransformParentChanged 异步触发器
    /// 将 Unity 的 OnBeforeTransformParentChanged 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnBeforeTransformParentChanged 在 Transform 的父级即将改变之前调用
    /// - 此时父级尚未改变，可以访问旧的父级信息并执行清理或保存操作
    /// - 常用于保存相对于当前父级的本地坐标、取消对旧父级的订阅等
    /// - 与 OnTransformParentChanged 配合使用，分别处理父级改变前后的事务
    /// 
    /// 使用示例：
    /// <code>
    /// await transform.GetAsyncBeforeTransformParentChangedTrigger().OnBeforeTransformParentChangedAsync();
    /// // 保存当前相对于父级的本地坐标
    /// Vector3 savedLocalPosition = transform.localPosition;
    /// Quaternion savedLocalRotation = transform.localRotation;
    /// // 现在可以安全地改变父级了
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要在 Transform 改变父级时才会触发，例如调用 transform.SetParent()
    /// - 此方法在父级改变之前调用，此时 transform.parent 仍然是旧的父级
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncBeforeTransformParentChangedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnBeforeTransformParentChanged 回调方法
        /// 当 Transform 的父级即将改变时，Unity 会自动调用此方法
        /// </summary>
        void OnBeforeTransformParentChanged()
        {
            // 触发事件，通知所有等待 OnBeforeTransformParentChangedAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnBeforeTransformParentChanged 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnBeforeTransformParentChanged 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnBeforeTransformParentChangedHandler 接口实例</returns>
        public IAsyncOnBeforeTransformParentChangedHandler GetOnBeforeTransformParentChangedAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnBeforeTransformParentChanged 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnBeforeTransformParentChangedHandler 接口实例</returns>
        public IAsyncOnBeforeTransformParentChangedHandler GetOnBeforeTransformParentChangedAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待 Transform 父级改变前的通知
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在 Transform 父级即将改变时完成</returns>
        public UniTask OnBeforeTransformParentChangedAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnBeforeTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnBeforeTransformParentChangedAsync();
        }

        /// <summary>
        /// 等待 Transform 父级改变前的通知（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在 Transform 父级即将改变时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnBeforeTransformParentChangedAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnBeforeTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnBeforeTransformParentChangedAsync();
        }
    }
#endregion
#region OnCanvasGroupChanged

    /// <summary>
    /// CanvasGroupChanged 异步处理接口
    /// 提供将 Unity 的 OnCanvasGroupChanged 消息转换为可等待的异步任务的能力
    /// 当 CanvasGroup 的状态改变时触发，用于 UI 交互状态检测
    /// </summary>
    public interface IAsyncOnCanvasGroupChangedHandler
    {
        /// <summary>
        /// 等待 CanvasGroup 状态改变
        /// </summary>
        /// <returns>UniTask，在 CanvasGroup 状态改变时完成</returns>
        UniTask OnCanvasGroupChangedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnCanvasGroupChanged 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnCanvasGroupChanged 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnCanvasGroupChangedHandler
    {
        /// <summary>
        /// 实现 IAsyncOnCanvasGroupChangedHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在 CanvasGroup 状态改变时完成
        /// </summary>
        UniTask IAsyncOnCanvasGroupChangedHandler.OnCanvasGroupChangedAsync()
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
        /// 获取或添加 AsyncOnCanvasGroupChangedTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncOnCanvasGroupChangedTrigger 组件实例</returns>
        public static AsyncOnCanvasGroupChangedTrigger GetAsyncOnCanvasGroupChangedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncOnCanvasGroupChangedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncOnCanvasGroupChangedTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncOnCanvasGroupChangedTrigger 组件实例</returns>
        public static AsyncOnCanvasGroupChangedTrigger GetAsyncOnCanvasGroupChangedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncOnCanvasGroupChangedTrigger();
        }
    }

    /// <summary>
    /// CanvasGroupChanged 异步触发器
    /// 将 Unity 的 OnCanvasGroupChanged 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnCanvasGroupChanged 当 CanvasGroup 的属性（如 alpha、interactable、blocksRaycasts 等）改变时调用
    /// - CanvasGroup 用于控制一组 UI 元素的交互性和可见性
    /// - 常用于检测 UI 面板的状态变化、响应 CanvasGroup 的设置改变等场景
    /// - 可用于实现复杂的 UI 状态机、条件渲染、交互控制等功能
    /// 
    /// 使用示例：
    /// <code>
    /// await canvasGroup.GetAsyncOnCanvasGroupChangedTrigger().OnCanvasGroupChangedAsync();
    /// Debug.Log("CanvasGroup 状态已改变");
    /// // 检查新的 CanvasGroup 状态并更新 UI
    /// if (!canvasGroup.interactable)
    /// {
    ///     DisableUI();
    /// }
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 GameObject 或其父级上有 CanvasGroup 组件
    /// - 此方法在 CanvasGroup 属性改变时调用，包括 alpha、interactable、blocksRaycasts 等
    /// - 常用于 UI 元素的动态启用/禁用、淡入淡出效果等场景
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncOnCanvasGroupChangedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnCanvasGroupChanged 回调方法
        /// 当 CanvasGroup 的状态改变时，Unity 会自动调用此方法
        /// </summary>
        void OnCanvasGroupChanged()
        {
            // 触发事件，通知所有等待 OnCanvasGroupChangedAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnCanvasGroupChanged 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnCanvasGroupChanged 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnCanvasGroupChangedHandler 接口实例</returns>
        public IAsyncOnCanvasGroupChangedHandler GetOnCanvasGroupChangedAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnCanvasGroupChanged 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnCanvasGroupChangedHandler 接口实例</returns>
        public IAsyncOnCanvasGroupChangedHandler GetOnCanvasGroupChangedAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待 CanvasGroup 状态改变
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在 CanvasGroup 状态改变时完成</returns>
        public UniTask OnCanvasGroupChangedAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnCanvasGroupChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnCanvasGroupChangedAsync();
        }

        /// <summary>
        /// 等待 CanvasGroup 状态改变（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在 CanvasGroup 状态改变时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnCanvasGroupChangedAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnCanvasGroupChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnCanvasGroupChangedAsync();
        }
    }
#endregion
#region TransformChildrenChanged

    /// <summary>
    /// TransformChildrenChanged 异步处理接口
    /// 提供将 Unity 的 OnTransformChildrenChanged 消息转换为可等待的异步任务的能力
    /// 当 Transform 的子级列表改变时触发，用于层次结构变化检测
    /// </summary>
    public interface IAsyncOnTransformChildrenChangedHandler
    {
        /// <summary>
        /// 等待 Transform 子级列表改变
        /// </summary>
        /// <returns>UniTask，在 Transform 子级列表改变时完成</returns>
        UniTask OnTransformChildrenChangedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnTransformChildrenChanged 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnTransformChildrenChanged 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnTransformChildrenChangedHandler
    {
        /// <summary>
        /// 实现 IAsyncOnTransformChildrenChangedHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在 Transform 子级列表改变时完成
        /// </summary>
        UniTask IAsyncOnTransformChildrenChangedHandler.OnTransformChildrenChangedAsync()
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
        /// 获取或添加 AsyncTransformChildrenChangedTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncTransformChildrenChangedTrigger 组件实例</returns>
        public static AsyncTransformChildrenChangedTrigger GetAsyncTransformChildrenChangedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTransformChildrenChangedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTransformChildrenChangedTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncTransformChildrenChangedTrigger 组件实例</returns>
        public static AsyncTransformChildrenChangedTrigger GetAsyncTransformChildrenChangedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTransformChildrenChangedTrigger();
        }
    }

    /// <summary>
    /// TransformChildrenChanged 异步触发器
    /// 将 Unity 的 OnTransformChildrenChanged 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnTransformChildrenChanged 当 Transform 的子级列表改变时调用（添加或删除子对象）
    /// - 不会在子对象被移动或重新排序时调用，只在子级数量改变时调用
    /// - 常用于检测层次结构变化、动态更新 UI、对象池管理、场景图构建等场景
    /// - 可以用于实现动态菜单生成、列表更新、层次结构验证等功能
    /// 
    /// 使用示例：
    /// <code>
    /// await container.GetAsyncTransformChildrenChangedTrigger().OnTransformChildrenChangedAsync();
    /// Debug.Log("子级列表已改变");
    /// // 重新计算布局或更新 UI
    /// RefreshLayout();
    /// </code>
    /// 
    /// 注意事项：
    /// - 仅在子级被添加或删除时触发，子级的 Transform 属性改变不会触发
    /// - 子级被禁用或启用不会触发此方法，只有添加/删除才会触发
    /// - 常用于需要响应层次结构变化的系统，如 UI 布局系统、场景管理器等
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncTransformChildrenChangedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnTransformChildrenChanged 回调方法
        /// 当 Transform 的子级列表改变时，Unity 会自动调用此方法
        /// </summary>
        void OnTransformChildrenChanged()
        {
            // 触发事件，通知所有等待 OnTransformChildrenChangedAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnTransformChildrenChanged 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnTransformChildrenChanged 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnTransformChildrenChangedHandler 接口实例</returns>
        public IAsyncOnTransformChildrenChangedHandler GetOnTransformChildrenChangedAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnTransformChildrenChanged 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnTransformChildrenChangedHandler 接口实例</returns>
        public IAsyncOnTransformChildrenChangedHandler GetOnTransformChildrenChangedAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待 Transform 子级列表改变
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在 Transform 子级列表改变时完成</returns>
        public UniTask OnTransformChildrenChangedAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnTransformChildrenChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnTransformChildrenChangedAsync();
        }

        /// <summary>
        /// 等待 Transform 子级列表改变（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在 Transform 子级列表改变时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnTransformChildrenChangedAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnTransformChildrenChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnTransformChildrenChangedAsync();
        }
    }
#endregion
#region TransformParentChanged

    /// <summary>
    /// TransformParentChanged 异步处理接口
    /// 提供将 Unity 的 OnTransformParentChanged 消息转换为可等待的异步任务的能力
    /// 当 Transform 的父级改变时触发，用于层次结构变化检测
    /// </summary>
    public interface IAsyncOnTransformParentChangedHandler
    {
        /// <summary>
        /// 等待 Transform 父级改变
        /// </summary>
        /// <returns>UniTask，在 Transform 父级改变时完成</returns>
        UniTask OnTransformParentChangedAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnTransformParentChanged 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnTransformParentChanged 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnTransformParentChangedHandler
    {
        /// <summary>
        /// 实现 IAsyncOnTransformParentChangedHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在 Transform 父级改变时完成
        /// </summary>
        UniTask IAsyncOnTransformParentChangedHandler.OnTransformParentChangedAsync()
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
        /// 获取或添加 AsyncTransformParentChangedTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncTransformParentChangedTrigger 组件实例</returns>
        public static AsyncTransformParentChangedTrigger GetAsyncTransformParentChangedTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncTransformParentChangedTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncTransformParentChangedTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncTransformParentChangedTrigger 组件实例</returns>
        public static AsyncTransformParentChangedTrigger GetAsyncTransformParentChangedTrigger(this Component component)
        {
            return component.gameObject.GetAsyncTransformParentChangedTrigger();
        }
    }

    /// <summary>
    /// TransformParentChanged 异步触发器
    /// 将 Unity 的 OnTransformParentChanged 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnTransformParentChanged 在 Transform 的父级改变之后调用
    /// - 此时父级已经改变，可以访问新的父级信息并执行相应的操作
    /// - 与 OnBeforeTransformParentChanged 配合使用，分别处理父级改变前后的事务
    /// - 常用于层次结构管理、坐标系统更新、场景图重建、UI 面板管理等场景
    /// 
    /// 使用示例：
    /// <code>
    /// await transform.GetAsyncTransformParentChangedTrigger().OnTransformParentChangedAsync();
    /// Debug.Log("父级已改变");
    /// // 更新相对于新父级的本地坐标
    /// if (transform.parent != null)
    /// {
    ///     UpdateLocalPosition();
    /// }
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要在 Transform 改变父级时才会触发，例如调用 transform.SetParent()
    /// - 此方法在父级改变之后调用，此时 transform.parent 已经是新的父级
    /// - 常用于处理依赖父级变换的系统，如 UI 布局、动画系统、物理模拟等
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncTransformParentChangedTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnTransformParentChanged 回调方法
        /// 当 Transform 的父级改变时，Unity 会自动调用此方法
        /// </summary>
        void OnTransformParentChanged()
        {
            // 触发事件，通知所有等待 OnTransformParentChangedAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnTransformParentChanged 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnTransformParentChanged 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnTransformParentChangedHandler 接口实例</returns>
        public IAsyncOnTransformParentChangedHandler GetOnTransformParentChangedAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnTransformParentChanged 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnTransformParentChangedHandler 接口实例</returns>
        public IAsyncOnTransformParentChangedHandler GetOnTransformParentChangedAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待 Transform 父级改变
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在 Transform 父级改变时完成</returns>
        public UniTask OnTransformParentChangedAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnTransformParentChangedAsync();
        }

        /// <summary>
        /// 等待 Transform 父级改变（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在 Transform 父级改变时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnTransformParentChangedAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnTransformParentChangedAsync();
        }
    }
#endregion
#region WillRenderObject

    /// <summary>
    /// WillRenderObject 异步处理接口
    /// 提供将 Unity 的 OnWillRenderObject 消息转换为可等待的异步任务的能力
    /// 在对象被渲染之前调用，用于渲染前的准备工作
    /// </summary>
    public interface IAsyncOnWillRenderObjectHandler
    {
        /// <summary>
        /// 等待对象即将被渲染的通知
        /// </summary>
        /// <returns>UniTask，在对象即将被渲染时完成</returns>
        UniTask OnWillRenderObjectAsync();
    }

    /// <summary>
    /// AsyncTriggerHandler 的 OnWillRenderObject 接口实现
    /// 通过泛型扩展，让 AsyncTriggerHandler 能够处理 OnWillRenderObject 事件
    /// </summary>
    public partial class AsyncTriggerHandler<T> : IAsyncOnWillRenderObjectHandler
    {
        /// <summary>
        /// 实现 IAsyncOnWillRenderObjectHandler 接口
        /// 重置核心状态并返回一个新的 UniTask，该任务会在对象即将被渲染时完成
        /// </summary>
        UniTask IAsyncOnWillRenderObjectHandler.OnWillRenderObjectAsync()
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
        /// 获取或添加 AsyncWillRenderObjectTrigger 组件到指定的 GameObject
        /// 如果 GameObject 上已存在该组件，则返回现有组件；否则创建新组件并返回
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>AsyncWillRenderObjectTrigger 组件实例</returns>
        public static AsyncWillRenderObjectTrigger GetAsyncWillRenderObjectTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncWillRenderObjectTrigger>(gameObject);
        }
        
        /// <summary>
        /// 获取或添加 AsyncWillRenderObjectTrigger 组件到指定 Component 所属的 GameObject
        /// 这是一个便捷方法，用于从 Component 快速获取触发器
        /// </summary>
        /// <param name="component">目标 Component，触发器将添加到该组件所属的 GameObject 上</param>
        /// <returns>AsyncWillRenderObjectTrigger 组件实例</returns>
        public static AsyncWillRenderObjectTrigger GetAsyncWillRenderObjectTrigger(this Component component)
        {
            return component.gameObject.GetAsyncWillRenderObjectTrigger();
        }
    }

    /// <summary>
    /// WillRenderObject 异步触发器
    /// 将 Unity 的 OnWillRenderObject 消息转换为可等待的异步任务
    /// 
    /// 使用说明：
    /// - OnWillRenderObject 在对象的 Renderer 组件即将被渲染之前调用
    /// - 此方法在渲染管线处理对象之前调用，每帧可能被调用多次（如果对象被多个摄像机看到）
    /// - 常用于动态修改材质属性、更新渲染参数、设置着色器参数等渲染相关的准备工作
    /// - 可用于实现自定义渲染效果、动态材质更新、基于摄像机的渲染调整等功能
    /// 
    /// 使用示例：
    /// <code>
    /// await renderer.GetAsyncWillRenderObjectTrigger().OnWillRenderObjectAsync();
    /// // 在渲染前更新材质属性
    /// material.SetFloat("_Time", Time.time);
    /// material.SetVector("_CameraPosition", Camera.current.transform.position);
    /// </code>
    /// 
    /// 注意事项：
    /// - 需要确保 GameObject 上有 Renderer 组件（如 MeshRenderer、SpriteRenderer 等）
    /// - 此方法每帧可能被调用多次（如果对象被多个摄像机看到）
    /// - 不应在此方法中进行耗时操作，否则会影响渲染性能
    /// - 可以通过 Camera.current 访问当前正在渲染的摄像机
    /// </summary>
    [DisallowMultipleComponent] // 确保同一个 GameObject 上只能有一个该类型的组件
    public sealed class AsyncWillRenderObjectTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Unity 的 OnWillRenderObject 回调方法
        /// 当对象的 Renderer 组件即将被渲染时，Unity 会自动调用此方法
        /// </summary>
        void OnWillRenderObject()
        {
            // 触发事件，通知所有等待 OnWillRenderObjectAsync 的任务
            // 使用 AsyncUnit.Default 作为事件数据，因为 OnWillRenderObject 不需要传递额外信息
            RaiseEvent(AsyncUnit.Default);
        }

        /// <summary>
        /// 获取 OnWillRenderObject 异步处理器（不使用 callOnce 模式）
        /// callOnce=false 表示处理器可以被多次使用，不会自动清理
        /// 通常用于需要重复等待同一个事件的场景
        /// </summary>
        /// <returns>IAsyncOnWillRenderObjectHandler 接口实例</returns>
        public IAsyncOnWillRenderObjectHandler GetOnWillRenderObjectAsyncHandler()
        {
            // 创建新的 AsyncTriggerHandler，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, false);
        }

        /// <summary>
        /// 获取 OnWillRenderObject 异步处理器（带取消令牌，不使用 callOnce 模式）
        /// 允许通过 CancellationToken 取消等待操作
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于取消等待操作</param>
        /// <returns>IAsyncOnWillRenderObjectHandler 接口实例</returns>
        public IAsyncOnWillRenderObjectHandler GetOnWillRenderObjectAsyncHandler(CancellationToken cancellationToken)
        {
            // 创建新的 AsyncTriggerHandler，传入取消令牌，callOnce 设置为 false
            return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
        }

        /// <summary>
        /// 等待对象即将被渲染的通知
        /// 这是一个便捷方法，内部使用 callOnce=true 模式，任务完成后会自动清理
        /// </summary>
        /// <returns>UniTask，在对象即将被渲染时完成</returns>
        public UniTask OnWillRenderObjectAsync()
        {
            // 创建 callOnce=true 的处理器，并调用其异步方法
            // callOnce=true 表示任务完成后会自动清理，适合一次性等待的场景
            return ((IAsyncOnWillRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnWillRenderObjectAsync();
        }

        /// <summary>
        /// 等待对象即将被渲染的通知（带取消令牌）
        /// 允许通过 CancellationToken 取消等待操作，适合需要支持取消的场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌，当令牌被取消时，等待操作会被中断</param>
        /// <returns>UniTask，在对象即将被渲染时完成，或在令牌取消时抛出 OperationCanceledException</returns>
        public UniTask OnWillRenderObjectAsync(CancellationToken cancellationToken)
        {
            // 创建带取消令牌的处理器（callOnce=true），并调用其异步方法
            return ((IAsyncOnWillRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnWillRenderObjectAsync();
        }
    }
#endregion
}