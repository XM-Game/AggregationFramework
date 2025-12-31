#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
    /// <summary>
    /// AsyncDestroyTrigger 扩展方法
    /// </summary>
    /// <remarks>
    /// 提供 GetAsyncDestroyTrigger 方法，用于获取 AsyncDestroyTrigger 组件
    /// </remarks>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取 AsyncDestroyTrigger 组件
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>AsyncDestroyTrigger 组件</returns>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDestroyTrigger>(gameObject);
        }

        /// <summary>
        /// 获取 AsyncDestroyTrigger 组件
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>AsyncDestroyTrigger 组件</returns>
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDestroyTrigger();
        }
    }

    [DisallowMultipleComponent]
    /// <summary>
    /// AsyncDestroyTrigger 组件
    /// </summary>
    /// <remarks>
    /// 继承自 MonoBehaviour，用于在对象销毁时调用
    /// </remarks>
    public sealed class AsyncDestroyTrigger : MonoBehaviour
    {
        /// <summary>
        /// 是否已经调用 Awake
        /// </summary>
        bool awakeCalled = false;
        /// <summary>
        /// 是否已经调用 OnDestroy
        /// </summary>
        bool called = false;
        CancellationTokenSource cancellationTokenSource;

        public CancellationToken CancellationToken
        {
            get
            {
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    if (!awakeCalled)
                    {
                        PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
                    }
                }
                return cancellationTokenSource.Token;
            }
        }

        void Awake()
        {
            awakeCalled = true;
        }

        void OnDestroy()
        {
            called = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public UniTask OnDestroyAsync()
        {
            if (called) return UniTask.CompletedTask;

            var tcs = new UniTaskCompletionSource();

            // OnDestroy = Called Cancel.
            CancellationToken.RegisterWithoutCaptureExecutionContext(state =>
            {
                var tcs2 = (UniTaskCompletionSource)state;
                tcs2.TrySetResult();
            }, tcs);

            return tcs.Task;
        }

        /// <summary>
        /// AwakeMonitor 类
        /// </summary>
        /// <remarks>
        /// 继承自 IPlayerLoopItem，用于在 Awake 事件触发时调用
        /// </remarks>
        class AwakeMonitor : IPlayerLoopItem
        {
            readonly AsyncDestroyTrigger trigger;

            /// <summary>
            /// AwakeMonitor 构造函数
            /// </summary>
            /// <param name="trigger">AsyncDestroyTrigger</param>
            public AwakeMonitor(AsyncDestroyTrigger trigger)
            {
                this.trigger = trigger;
            }

            /// <summary>
            /// 移动下一个
            /// </summary>
            /// <returns>是否移动下一个</returns>
            public bool MoveNext()
            {
                if (trigger.called || trigger.awakeCalled) return false;
                if (trigger == null)
                {
                    trigger.OnDestroy();
                    return false;
                }
                return true;
            }
        }
    }
}

