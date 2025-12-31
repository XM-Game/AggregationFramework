#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
    /// <summary>
    /// AsyncStartTrigger 扩展方法
    /// </summary>
    /// <remarks>
    /// 提供 GetAsyncStartTrigger 方法，用于获取 AsyncStartTrigger 组件
    /// </remarks>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取 AsyncStartTrigger 组件
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>AsyncStartTrigger 组件</returns>
        public static AsyncStartTrigger GetAsyncStartTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncStartTrigger>(gameObject);
        }

        /// <summary>
        /// 获取 AsyncStartTrigger 组件
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>AsyncStartTrigger 组件</returns>
        public static AsyncStartTrigger GetAsyncStartTrigger(this Component component)
        {
            return component.gameObject.GetAsyncStartTrigger();
        }
    }

    [DisallowMultipleComponent]
    /// <summary>
    /// AsyncStartTrigger 组件
    /// </summary>
    /// <remarks>
    /// 继承自 AsyncTriggerBase<AsyncUnit>，用于在 Start 事件触发时调用
    /// </remarks>
    public sealed class AsyncStartTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// 是否已经调用
        /// </summary>
        bool called;

        /// <summary>
        /// Start 事件触发时调用
        /// </summary>
        /// <remarks>
        /// 设置 called 为 true，并调用 RaiseEvent(AsyncUnit.Default)
        /// </remarks>
        void Start()
        {
            called = true;
            RaiseEvent(AsyncUnit.Default);
        }

        public UniTask StartAsync()
        {
            if (called) return UniTask.CompletedTask;

            return ((IAsyncOneShotTrigger)new AsyncTriggerHandler<AsyncUnit>(this, true)).OneShotAsync();
        }
    }
}