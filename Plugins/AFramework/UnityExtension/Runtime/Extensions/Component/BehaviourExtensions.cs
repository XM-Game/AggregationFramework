// ==========================================================
// 文件名：BehaviourExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Behaviour 扩展方法
    /// <para>提供 Behaviour 的启用状态操作扩展</para>
    /// </summary>
    public static class BehaviourExtensions
    {
        #region 启用状态控制

        /// <summary>
        /// 启用 Behaviour
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Enable<T>(this T behaviour) where T : Behaviour
        {
            behaviour.enabled = true;
            return behaviour;
        }

        /// <summary>
        /// 禁用 Behaviour
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Disable<T>(this T behaviour) where T : Behaviour
        {
            behaviour.enabled = false;
            return behaviour;
        }

        /// <summary>
        /// 切换 Behaviour 启用状态
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ToggleEnabled<T>(this T behaviour) where T : Behaviour
        {
            behaviour.enabled = !behaviour.enabled;
            return behaviour;
        }

        /// <summary>
        /// 设置 Behaviour 启用状态
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <param name="enabled">是否启用</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetEnabled<T>(this T behaviour, bool enabled) where T : Behaviour
        {
            behaviour.enabled = enabled;
            return behaviour;
        }

        #endregion

        #region 条件启用

        /// <summary>
        /// 根据条件设置启用状态
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetEnabledIf<T>(this T behaviour, bool condition) where T : Behaviour
        {
            behaviour.enabled = condition;
            return behaviour;
        }

        /// <summary>
        /// 根据条件函数设置启用状态
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetEnabledIf<T>(this T behaviour, Func<bool> predicate) where T : Behaviour
        {
            behaviour.enabled = predicate();
            return behaviour;
        }

        /// <summary>
        /// 如果条件为真则启用
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnableIf<T>(this T behaviour, bool condition) where T : Behaviour
        {
            if (condition)
            {
                behaviour.enabled = true;
            }
            return behaviour;
        }

        /// <summary>
        /// 如果条件为真则禁用
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DisableIf<T>(this T behaviour, bool condition) where T : Behaviour
        {
            if (condition)
            {
                behaviour.enabled = false;
            }
            return behaviour;
        }

        #endregion

        #region 状态检查

        /// <summary>
        /// 检查 Behaviour 是否启用
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>如果启用返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabled(this Behaviour behaviour)
        {
            return behaviour.enabled;
        }

        /// <summary>
        /// 检查 Behaviour 是否禁用
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>如果禁用返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisabled(this Behaviour behaviour)
        {
            return !behaviour.enabled;
        }

        /// <summary>
        /// 检查 Behaviour 是否启用且 GameObject 激活
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>如果启用且激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnabledAndActive(this Behaviour behaviour)
        {
            return behaviour.enabled && behaviour.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 检查 Behaviour 是否可以运行 (启用且 GameObject 在层级中激活)
        /// </summary>
        /// <param name="behaviour">目标 Behaviour</param>
        /// <returns>如果可以运行返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanRun(this Behaviour behaviour)
        {
            return behaviour.isActiveAndEnabled;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 启用所有 Behaviour
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        public static void EnableAll<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = true;
            }
        }

        /// <summary>
        /// 禁用所有 Behaviour
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        public static void DisableAll<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = false;
            }
        }

        /// <summary>
        /// 设置所有 Behaviour 的启用状态
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        /// <param name="enabled">是否启用</param>
        public static void SetEnabledAll<T>(this IEnumerable<T> behaviours, bool enabled) where T : Behaviour
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = enabled;
            }
        }

        /// <summary>
        /// 切换所有 Behaviour 的启用状态
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        public static void ToggleEnabledAll<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = !behaviour.enabled;
            }
        }

        #endregion

        #region 统计

        /// <summary>
        /// 获取启用的 Behaviour 数量
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        /// <returns>启用的数量</returns>
        public static int CountEnabled<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            int count = 0;
            foreach (var behaviour in behaviours)
            {
                if (behaviour.enabled)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取禁用的 Behaviour 数量
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        /// <returns>禁用的数量</returns>
        public static int CountDisabled<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            int count = 0;
            foreach (var behaviour in behaviours)
            {
                if (!behaviour.enabled)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取所有启用的 Behaviour
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        /// <returns>启用的 Behaviour 列表</returns>
        public static List<T> GetEnabled<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            var result = new List<T>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour.enabled)
                {
                    result.Add(behaviour);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有禁用的 Behaviour
        /// </summary>
        /// <param name="behaviours">Behaviour 集合</param>
        /// <returns>禁用的 Behaviour 列表</returns>
        public static List<T> GetDisabled<T>(this IEnumerable<T> behaviours) where T : Behaviour
        {
            var result = new List<T>();
            foreach (var behaviour in behaviours)
            {
                if (!behaviour.enabled)
                {
                    result.Add(behaviour);
                }
            }
            return result;
        }

        #endregion
    }
}
