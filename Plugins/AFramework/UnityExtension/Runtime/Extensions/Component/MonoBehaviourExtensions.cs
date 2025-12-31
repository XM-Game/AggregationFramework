// ==========================================================
// 文件名：MonoBehaviourExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System, System.Collections
// ==========================================================

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// MonoBehaviour 扩展方法
    /// <para>提供 MonoBehaviour 的协程和生命周期操作扩展</para>
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        #region 协程操作

        /// <summary>
        /// 延迟执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="delay">延迟时间 (秒)</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine DelayedCall(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(DelayedCallCoroutine(delay, action));
        }

        private static IEnumerator DelayedCallCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        /// <summary>
        /// 延迟执行操作 (不受时间缩放影响)
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="delay">延迟时间 (秒)</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine DelayedCallUnscaled(this MonoBehaviour mono, float delay, Action action)
        {
            return mono.StartCoroutine(DelayedCallUnscaledCoroutine(delay, action));
        }

        private static IEnumerator DelayedCallUnscaledCoroutine(float delay, Action action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
        }

        /// <summary>
        /// 在下一帧执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine NextFrame(this MonoBehaviour mono, Action action)
        {
            return mono.StartCoroutine(NextFrameCoroutine(action));
        }

        private static IEnumerator NextFrameCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        /// <summary>
        /// 在帧末执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine EndOfFrame(this MonoBehaviour mono, Action action)
        {
            return mono.StartCoroutine(EndOfFrameCoroutine(action));
        }

        private static IEnumerator EndOfFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        /// <summary>
        /// 在固定更新后执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine AfterFixedUpdate(this MonoBehaviour mono, Action action)
        {
            return mono.StartCoroutine(AfterFixedUpdateCoroutine(action));
        }

        private static IEnumerator AfterFixedUpdateCoroutine(Action action)
        {
            yield return new WaitForFixedUpdate();
            action?.Invoke();
        }

        #endregion

        #region 条件等待

        /// <summary>
        /// 等待条件满足后执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="condition">条件函数</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine WaitUntil(this MonoBehaviour mono, Func<bool> condition, Action action)
        {
            return mono.StartCoroutine(WaitUntilCoroutine(condition, action));
        }

        private static IEnumerator WaitUntilCoroutine(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }

        /// <summary>
        /// 等待条件不满足后执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="condition">条件函数</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>协程实例</returns>
        public static Coroutine WaitWhile(this MonoBehaviour mono, Func<bool> condition, Action action)
        {
            return mono.StartCoroutine(WaitWhileCoroutine(condition, action));
        }

        private static IEnumerator WaitWhileCoroutine(Func<bool> condition, Action action)
        {
            yield return new WaitWhile(condition);
            action?.Invoke();
        }

        #endregion

        #region 重复执行

        /// <summary>
        /// 重复执行操作
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="interval">间隔时间 (秒)</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="repeatCount">重复次数 (-1 表示无限)</param>
        /// <returns>协程实例</returns>
        public static Coroutine Repeat(this MonoBehaviour mono, float interval, Action action, int repeatCount = -1)
        {
            return mono.StartCoroutine(RepeatCoroutine(interval, action, repeatCount));
        }

        private static IEnumerator RepeatCoroutine(float interval, Action action, int repeatCount)
        {
            var wait = new WaitForSeconds(interval);
            int count = 0;
            while (repeatCount < 0 || count < repeatCount)
            {
                yield return wait;
                action?.Invoke();
                count++;
            }
        }

        /// <summary>
        /// 重复执行操作 (带索引)
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="interval">间隔时间 (秒)</param>
        /// <param name="action">要执行的操作 (参数为当前索引)</param>
        /// <param name="repeatCount">重复次数</param>
        /// <returns>协程实例</returns>
        public static Coroutine Repeat(this MonoBehaviour mono, float interval, Action<int> action, int repeatCount)
        {
            return mono.StartCoroutine(RepeatWithIndexCoroutine(interval, action, repeatCount));
        }

        private static IEnumerator RepeatWithIndexCoroutine(float interval, Action<int> action, int repeatCount)
        {
            var wait = new WaitForSeconds(interval);
            for (int i = 0; i < repeatCount; i++)
            {
                yield return wait;
                action?.Invoke(i);
            }
        }

        #endregion

        #region 协程管理

        /// <summary>
        /// 停止所有协程
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoBehaviour StopAllCoroutinesAndReturn(this MonoBehaviour mono)
        {
            mono.StopAllCoroutines();
            return mono;
        }

        /// <summary>
        /// 安全停止协程
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="coroutine">要停止的协程</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static MonoBehaviour StopCoroutineSafe(this MonoBehaviour mono, Coroutine coroutine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }
            return mono;
        }

        /// <summary>
        /// 重启协程
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="coroutine">要重启的协程引用</param>
        /// <param name="routine">协程方法</param>
        /// <returns>新的协程实例</returns>
        public static Coroutine RestartCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, IEnumerator routine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }
            coroutine = mono.StartCoroutine(routine);
            return coroutine;
        }

        #endregion

        #region 启用状态

        /// <summary>
        /// 启用 MonoBehaviour
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Enable<T>(this T mono) where T : MonoBehaviour
        {
            mono.enabled = true;
            return mono;
        }

        /// <summary>
        /// 禁用 MonoBehaviour
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Disable<T>(this T mono) where T : MonoBehaviour
        {
            mono.enabled = false;
            return mono;
        }

        /// <summary>
        /// 切换 MonoBehaviour 启用状态
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ToggleEnabled<T>(this T mono) where T : MonoBehaviour
        {
            mono.enabled = !mono.enabled;
            return mono;
        }

        /// <summary>
        /// 设置 MonoBehaviour 启用状态
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <param name="enabled">是否启用</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetEnabled<T>(this T mono, bool enabled) where T : MonoBehaviour
        {
            mono.enabled = enabled;
            return mono;
        }

        #endregion

        #region 检查状态

        /// <summary>
        /// 检查 MonoBehaviour 是否启用且 GameObject 激活
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>如果启用且激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledAndActive(this MonoBehaviour mono)
        {
            return mono.enabled && mono.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 检查 MonoBehaviour 是否可以运行 (启用且 GameObject 在层级中激活)
        /// </summary>
        /// <param name="mono">目标 MonoBehaviour</param>
        /// <returns>如果可以运行返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanRun(this MonoBehaviour mono)
        {
            return mono.isActiveAndEnabled;
        }

        #endregion
    }
}
