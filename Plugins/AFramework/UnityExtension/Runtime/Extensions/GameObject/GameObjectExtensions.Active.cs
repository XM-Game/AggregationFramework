// ==========================================================
// 文件名：GameObjectExtensions.Active.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// GameObject 激活控制扩展方法
    /// </summary>
    public static partial class GameObjectExtensions
    {
        #region 基础激活控制

        /// <summary>
        /// 激活 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Activate(this GameObject gameObject)
        {
            gameObject.SetActive(true);
            return gameObject;
        }

        /// <summary>
        /// 停用 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Deactivate(this GameObject gameObject)
        {
            gameObject.SetActive(false);
            return gameObject;
        }

        /// <summary>
        /// 切换 GameObject 激活状态
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject ToggleActive(this GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
            return gameObject;
        }

        /// <summary>
        /// 设置 GameObject 激活状态
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="active">是否激活</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetActive(this GameObject gameObject, bool active)
        {
            gameObject.SetActive(active);
            return gameObject;
        }

        #endregion

        #region 激活状态检查

        /// <summary>
        /// 检查 GameObject 是否激活 (自身状态)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果自身激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveSelf(this GameObject gameObject)
        {
            return gameObject.activeSelf;
        }

        /// <summary>
        /// 检查 GameObject 是否在层级中激活 (考虑父级状态)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果在层级中激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy(this GameObject gameObject)
        {
            return gameObject.activeInHierarchy;
        }

        /// <summary>
        /// 检查 GameObject 是否停用
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果停用返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInactive(this GameObject gameObject)
        {
            return !gameObject.activeSelf;
        }

        #endregion

        #region 条件激活

        /// <summary>
        /// 根据条件设置激活状态
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetActiveIf(this GameObject gameObject, bool condition)
        {
            gameObject.SetActive(condition);
            return gameObject;
        }

        /// <summary>
        /// 根据条件函数设置激活状态
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetActiveIf(this GameObject gameObject, Func<bool> predicate)
        {
            gameObject.SetActive(predicate());
            return gameObject;
        }

        /// <summary>
        /// 根据条件函数设置激活状态 (传入 GameObject)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetActiveIf(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            gameObject.SetActive(predicate(gameObject));
            return gameObject;
        }

        /// <summary>
        /// 如果条件为真则激活
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject ActivateIf(this GameObject gameObject, bool condition)
        {
            if (condition)
            {
                gameObject.SetActive(true);
            }
            return gameObject;
        }

        /// <summary>
        /// 如果条件为真则停用
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="condition">条件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject DeactivateIf(this GameObject gameObject, bool condition)
        {
            if (condition)
            {
                gameObject.SetActive(false);
            }
            return gameObject;
        }

        #endregion

        #region 子级激活控制

        /// <summary>
        /// 激活所有直接子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateChildren(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            return gameObject;
        }

        /// <summary>
        /// 停用所有直接子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject DeactivateChildren(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            return gameObject;
        }

        /// <summary>
        /// 递归激活所有子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateAllChildren(this GameObject gameObject, bool includeSelf = false)
        {
            if (includeSelf)
            {
                gameObject.SetActive(true);
            }
            ActivateAllChildrenInternal(gameObject.transform);
            return gameObject;
        }

        private static void ActivateAllChildrenInternal(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(true);
                ActivateAllChildrenInternal(child);
            }
        }

        /// <summary>
        /// 递归停用所有子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject DeactivateAllChildren(this GameObject gameObject, bool includeSelf = false)
        {
            if (includeSelf)
            {
                gameObject.SetActive(false);
            }
            DeactivateAllChildrenInternal(gameObject.transform);
            return gameObject;
        }

        private static void DeactivateAllChildrenInternal(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(false);
                DeactivateAllChildrenInternal(child);
            }
        }

        /// <summary>
        /// 设置所有子级的激活状态
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="active">是否激活</param>
        /// <param name="recursive">是否递归</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject SetChildrenActive(this GameObject gameObject, bool active, bool recursive = false)
        {
            if (recursive)
            {
                return active ? gameObject.ActivateAllChildren() : gameObject.DeactivateAllChildren();
            }
            return active ? gameObject.ActivateChildren() : gameObject.DeactivateChildren();
        }

        #endregion

        #region 选择性激活

        /// <summary>
        /// 仅激活指定索引的子级，停用其他子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="index">要激活的子级索引</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateOnlyChild(this GameObject gameObject, int index)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == index);
            }
            return gameObject;
        }

        /// <summary>
        /// 仅激活指定名称的子级，停用其他子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="name">要激活的子级名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateOnlyChild(this GameObject gameObject, string name)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(child.name == name);
            }
            return gameObject;
        }

        /// <summary>
        /// 仅激活满足条件的子级，停用其他子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateOnlyChildWhere(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                child.SetActive(predicate(child));
            }
            return gameObject;
        }

        /// <summary>
        /// 激活满足条件的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ActivateChildrenWhere(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (predicate(child))
                {
                    child.SetActive(true);
                }
            }
            return gameObject;
        }

        /// <summary>
        /// 停用满足条件的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject DeactivateChildrenWhere(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (predicate(child))
                {
                    child.SetActive(false);
                }
            }
            return gameObject;
        }

        #endregion

        #region 激活状态统计

        /// <summary>
        /// 获取激活的子级数量
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>激活的子级数量</returns>
        public static int GetActiveChildCount(this GameObject gameObject)
        {
            int count = 0;
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取停用的子级数量
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>停用的子级数量</returns>
        public static int GetInactiveChildCount(this GameObject gameObject)
        {
            int count = 0;
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取所有激活的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>激活的子级列表</returns>
        public static List<GameObject> GetActiveChildren(this GameObject gameObject)
        {
            var result = new List<GameObject>();
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (child.activeSelf)
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有停用的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>停用的子级列表</returns>
        public static List<GameObject> GetInactiveChildren(this GameObject gameObject)
        {
            var result = new List<GameObject>();
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (!child.activeSelf)
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 检查是否有任何激活的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果有激活的子级返回 true</returns>
        public static bool HasActiveChildren(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否所有子级都激活
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果所有子级都激活返回 true</returns>
        public static bool AreAllChildrenActive(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
