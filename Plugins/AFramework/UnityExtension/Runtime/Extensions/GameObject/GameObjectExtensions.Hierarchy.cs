// ==========================================================
// 文件名：GameObjectExtensions.Hierarchy.cs
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
    /// GameObject 层级操作扩展方法
    /// </summary>
    public static partial class GameObjectExtensions
    {
        #region 子级获取

        /// <summary>
        /// 获取子级数量
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>子级数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetChildCount(this GameObject gameObject)
        {
            return gameObject.transform.childCount;
        }

        /// <summary>
        /// 检查是否有子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果有子级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasChildren(this GameObject gameObject)
        {
            return gameObject.transform.childCount > 0;
        }

        /// <summary>
        /// 获取指定索引的子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="index">子级索引</param>
        /// <returns>子级 GameObject</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetChild(this GameObject gameObject, int index)
        {
            return gameObject.transform.GetChild(index).gameObject;
        }

        /// <summary>
        /// 安全获取指定索引的子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="index">子级索引</param>
        /// <returns>子级 GameObject，如果索引越界返回 null</returns>
        public static GameObject GetChildSafe(this GameObject gameObject, int index)
        {
            var transform = gameObject.transform;
            if (index < 0 || index >= transform.childCount) return null;
            return transform.GetChild(index).gameObject;
        }

        /// <summary>
        /// 获取第一个子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>第一个子级 GameObject，如果没有子级返回 null</returns>
        public static GameObject GetFirstChild(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            return transform.childCount > 0 ? transform.GetChild(0).gameObject : null;
        }

        /// <summary>
        /// 获取最后一个子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>最后一个子级 GameObject，如果没有子级返回 null</returns>
        public static GameObject GetLastChild(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var count = transform.childCount;
            return count > 0 ? transform.GetChild(count - 1).gameObject : null;
        }

        /// <summary>
        /// 获取所有直接子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>子级 GameObject 列表</returns>
        public static List<GameObject> GetChildren(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var result = new List<GameObject>(transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                result.Add(transform.GetChild(i).gameObject);
            }
            return result;
        }

        /// <summary>
        /// 获取所有子级 GameObject (递归)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>所有子级 GameObject 列表</returns>
        public static List<GameObject> GetAllChildren(this GameObject gameObject, bool includeSelf = false)
        {
            var result = new List<GameObject>();
            if (includeSelf)
            {
                result.Add(gameObject);
            }
            GetAllChildrenRecursive(gameObject.transform, result);
            return result;
        }

        private static void GetAllChildrenRecursive(Transform transform, List<GameObject> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                result.Add(child.gameObject);
                GetAllChildrenRecursive(child, result);
            }
        }

        #endregion

        #region 子级查找

        /// <summary>
        /// 按名称查找子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="name">子级名称</param>
        /// <returns>找到的 GameObject，如果没找到返回 null</returns>
        public static GameObject FindChild(this GameObject gameObject, string name)
        {
            var transform = gameObject.transform.Find(name);
            return transform != null ? transform.gameObject : null;
        }

        /// <summary>
        /// 递归按名称查找子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="name">子级名称</param>
        /// <returns>找到的 GameObject，如果没找到返回 null</returns>
        public static GameObject FindChildDeep(this GameObject gameObject, string name)
        {
            var transform = gameObject.transform.FindDeep(name);
            return transform != null ? transform.gameObject : null;
        }

        /// <summary>
        /// 按条件查找子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的 GameObject，如果没找到返回 null</returns>
        public static GameObject FindChild(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (predicate(child))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归按条件查找子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的 GameObject，如果没找到返回 null</returns>
        public static GameObject FindChildDeep(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (predicate(child))
                {
                    return child;
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = transform.GetChild(i).gameObject.FindChildDeep(predicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 按条件查找所有子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的所有 GameObject 列表</returns>
        public static List<GameObject> FindChildren(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var result = new List<GameObject>();
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                if (predicate(child))
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 递归按条件查找所有子级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="predicate">条件谓词</param>
        /// <returns>找到的所有 GameObject 列表</returns>
        public static List<GameObject> FindChildrenDeep(this GameObject gameObject, Func<GameObject, bool> predicate)
        {
            var result = new List<GameObject>();
            FindChildrenDeepInternal(gameObject.transform, predicate, result);
            return result;
        }

        private static void FindChildrenDeepInternal(Transform transform, Func<GameObject, bool> predicate, List<GameObject> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (predicate(child.gameObject))
                {
                    result.Add(child.gameObject);
                }
                FindChildrenDeepInternal(child, predicate, result);
            }
        }

        #endregion

        #region 子级操作

        /// <summary>
        /// 销毁所有子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="immediate">是否立即销毁</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject DestroyAllChildren(this GameObject gameObject, bool immediate = false)
        {
            var transform = gameObject.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i).gameObject;
                if (immediate)
                {
                    UnityEngine.Object.DestroyImmediate(child);
                }
                else
                {
                    UnityEngine.Object.Destroy(child);
                }
            }
            return gameObject;
        }

        /// <summary>
        /// 分离所有子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject DetachAllChildren(this GameObject gameObject)
        {
            gameObject.transform.DetachChildren();
            return gameObject;
        }

        /// <summary>
        /// 对所有直接子级执行操作
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ForEachChild(this GameObject gameObject, Action<GameObject> action)
        {
            var transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                action(transform.GetChild(i).gameObject);
            }
            return gameObject;
        }

        /// <summary>
        /// 对所有子级执行操作 (递归)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ForEachChildRecursive(this GameObject gameObject, Action<GameObject> action, bool includeSelf = false)
        {
            if (includeSelf)
            {
                action(gameObject);
            }
            ForEachChildRecursiveInternal(gameObject.transform, action);
            return gameObject;
        }

        private static void ForEachChildRecursiveInternal(Transform transform, Action<GameObject> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                action(child.gameObject);
                ForEachChildRecursiveInternal(child, action);
            }
        }

        #endregion

        #region 兄弟节点

        /// <summary>
        /// 获取兄弟节点索引
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>兄弟节点索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSiblingIndex(this GameObject gameObject)
        {
            return gameObject.transform.GetSiblingIndex();
        }

        /// <summary>
        /// 设置兄弟节点索引
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="index">索引</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetSiblingIndex(this GameObject gameObject, int index)
        {
            gameObject.transform.SetSiblingIndex(index);
            return gameObject;
        }

        /// <summary>
        /// 设置为第一个兄弟节点
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetAsFirstSibling(this GameObject gameObject)
        {
            gameObject.transform.SetAsFirstSibling();
            return gameObject;
        }

        /// <summary>
        /// 设置为最后一个兄弟节点
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetAsLastSibling(this GameObject gameObject)
        {
            gameObject.transform.SetAsLastSibling();
            return gameObject;
        }

        /// <summary>
        /// 获取下一个兄弟节点
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>下一个兄弟节点，如果没有返回 null</returns>
        public static GameObject GetNextSibling(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var parent = transform.parent;
            if (parent == null) return null;

            var index = transform.GetSiblingIndex();
            return index < parent.childCount - 1 ? parent.GetChild(index + 1).gameObject : null;
        }

        /// <summary>
        /// 获取上一个兄弟节点
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>上一个兄弟节点，如果没有返回 null</returns>
        public static GameObject GetPreviousSibling(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var parent = transform.parent;
            if (parent == null) return null;

            var index = transform.GetSiblingIndex();
            return index > 0 ? parent.GetChild(index - 1).gameObject : null;
        }

        /// <summary>
        /// 获取所有兄弟节点
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>兄弟节点列表</returns>
        public static List<GameObject> GetSiblings(this GameObject gameObject, bool includeSelf = false)
        {
            var result = new List<GameObject>();
            var transform = gameObject.transform;
            var parent = transform.parent;
            if (parent == null) return result;

            for (int i = 0; i < parent.childCount; i++)
            {
                var sibling = parent.GetChild(i).gameObject;
                if (includeSelf || sibling != gameObject)
                {
                    result.Add(sibling);
                }
            }
            return result;
        }

        #endregion

        #region 层级检查

        /// <summary>
        /// 检查是否为根级 GameObject
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果是根级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRoot(this GameObject gameObject)
        {
            return gameObject.transform.parent == null;
        }

        /// <summary>
        /// 检查是否为叶子节点 (无子级)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果是叶子节点返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLeaf(this GameObject gameObject)
        {
            return gameObject.transform.childCount == 0;
        }

        /// <summary>
        /// 检查是否为指定 GameObject 的子级
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="potentialParent">潜在父级</param>
        /// <returns>如果是子级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsChildOf(this GameObject gameObject, GameObject potentialParent)
        {
            return gameObject.transform.IsChildOf(potentialParent.transform);
        }

        /// <summary>
        /// 获取层级深度
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>层级深度 (根级为 0)</returns>
        public static int GetDepth(this GameObject gameObject)
        {
            int depth = 0;
            var current = gameObject.transform.parent;
            while (current != null)
            {
                depth++;
                current = current.parent;
            }
            return depth;
        }

        /// <summary>
        /// 获取层级路径
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="separator">路径分隔符</param>
        /// <returns>层级路径字符串</returns>
        public static string GetPath(this GameObject gameObject, string separator = "/")
        {
            return gameObject.transform.GetPath(separator);
        }

        #endregion
    }
}
