#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
    /// <summary>
    /// AsyncAwakeTrigger 扩展方法
    /// </summary>
    /// <remarks>
    /// 提供 GetAsyncAwakeTrigger 方法，用于获取 AsyncAwakeTrigger 组件
    /// </remarks>
    public static partial class AsyncTriggerExtensions
    {
        /// <summary>
        /// 获取 AsyncAwakeTrigger 组件
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>AsyncAwakeTrigger 组件</returns>
        public static AsyncAwakeTrigger GetAsyncAwakeTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncAwakeTrigger>(gameObject);
        }

        /// <summary>
        /// 获取 AsyncAwakeTrigger 组件
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>AsyncAwakeTrigger 组件</returns>
        public static AsyncAwakeTrigger GetAsyncAwakeTrigger(this Component component)
        {
            return component.gameObject.GetAsyncAwakeTrigger();
        }
    }

    [DisallowMultipleComponent]
    /// <summary>
    /// AsyncAwakeTrigger 组件
    /// </summary>
    /// <remarks>
    /// 继承自 AsyncTriggerBase<AsyncUnit>，用于在 Awake 事件触发时调用
    /// </remarks>
    public sealed class AsyncAwakeTrigger : AsyncTriggerBase<AsyncUnit>
    {
        /// <summary>
        /// Awake 事件触发时调用
        /// </summary>
        /// <returns>UniTask</returns>
        public UniTask AwakeAsync()
        {
            if (calledAwake) return UniTask.CompletedTask;

            return ((IAsyncOneShotTrigger)new AsyncTriggerHandler<AsyncUnit>(this, true)).OneShotAsync();
        }
    }
}

