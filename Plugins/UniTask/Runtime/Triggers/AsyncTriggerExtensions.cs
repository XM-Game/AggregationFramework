#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks.Triggers;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UniTaskCancellationExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 GetCancellationTokenOnDestroy 方法，用于获取取消令牌
    /// </remarks>
    public static class UniTaskCancellationExtensions
    {
#if UNITY_2022_2_OR_NEWER

        /// <summary>
        /// 获取取消令牌
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour</param>
        /// <returns>取消令牌</returns>
        public static CancellationToken GetCancellationTokenOnDestroy(this MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.destroyCancellationToken;
        }

#endif

        /// <summary>
        /// 获取取消令牌
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>取消令牌</returns>
        public static CancellationToken GetCancellationTokenOnDestroy(this GameObject gameObject)
        {
            return gameObject.GetAsyncDestroyTrigger().CancellationToken;
        }

        /// <summary>
        /// 获取取消令牌
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>取消令牌</returns>
        public static CancellationToken GetCancellationTokenOnDestroy(this Component component)
        {
#if UNITY_2022_2_OR_NEWER
            if (component is MonoBehaviour mb)
            {
                return mb.destroyCancellationToken;
            }
#endif

            return component.GetAsyncDestroyTrigger().CancellationToken;
        }
    }
}

namespace Cysharp.Threading.Tasks.Triggers
{
    /// <summary>
    /// AsyncTriggerExtensions 类
    /// </summary>
    /// <remarks>
    /// 提供 GetOrAddComponent 方法，用于获取或添加组件
    /// </remarks>
    public static partial class AsyncTriggerExtensions
    {
        // Util.

        /// <summary>
        /// 获取或添加组件
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>组件</returns>
        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
#if UNITY_2019_2_OR_NEWER
            if (!gameObject.TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }
#else
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
#endif

            return component;
        }

        // Special for single operation.

        /// <summary>
        /// 当游戏对象销毁时调用
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>UniTask</returns>
        public static UniTask OnDestroyAsync(this GameObject gameObject)
        {
            return gameObject.GetAsyncDestroyTrigger().OnDestroyAsync();
        }

        /// <summary>
        /// 当组件销毁时调用
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>UniTask</returns>
        public static UniTask OnDestroyAsync(this Component component)
        {
            return component.GetAsyncDestroyTrigger().OnDestroyAsync();
        }

        /// <summary>
        /// 当游戏对象 Start 时调用
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>UniTask</returns>
        public static UniTask StartAsync(this GameObject gameObject)
        {
            return gameObject.GetAsyncStartTrigger().StartAsync();
        }

        /// <summary>
        /// 当组件 Start 时调用
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>UniTask</returns>
        public static UniTask StartAsync(this Component component)
        {
            return component.GetAsyncStartTrigger().StartAsync();
        }

        /// <summary>
        /// 当游戏对象 Awake 时调用
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <returns>UniTask</returns>
        public static UniTask AwakeAsync(this GameObject gameObject)
        {
            return gameObject.GetAsyncAwakeTrigger().AwakeAsync();
        }

        /// <summary>
        /// 当组件 Awake 时调用
        /// </summary>
        /// <param name="component">组件</param>
        /// <returns>UniTask</returns>
        public static UniTask AwakeAsync(this Component component)
        {
            return component.GetAsyncAwakeTrigger().AwakeAsync();
        }
    }
}

