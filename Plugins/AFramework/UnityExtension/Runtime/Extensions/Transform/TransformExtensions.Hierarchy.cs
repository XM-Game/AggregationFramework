// ==========================================================
// 文件名：TransformExtensions.Hierarchy.cs
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
    /// Transform 层级操作扩展方法
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 父子关系操作

        /// <summary>
        /// 设置父级并保持世界空间变换不变
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="parent">新父级</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetParentKeepWorld(this Transform transform, Transform parent)
        {
            transform.SetParent(parent, true);
            return transform;
        }

        /// <summary>
        /// 设置父级并重置本地变换
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="parent">新父级</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetParentAndReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform;
        }

        /// <summary>
        /// 移除父级 (设置为根级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform Unparent(this Transform transform, bool worldPositionStays = true)
        {
            transform.SetParent(null, worldPositionStays);
            return transform;
        }

        /// <summary>
        /// 检查是否为根级 Transform (无父级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果是根级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRoot(this Transform transform)
        {
            return transform.parent == null;
        }

        /// <summary>
        /// 检查是否为叶子节点 (无子级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果是叶子节点返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLeaf(this Transform transform)
        {
            return transform.childCount == 0;
        }

        /// <summary>
        /// 检查是否有子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果有子级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasChildren(this Transform transform)
        {
            return transform.childCount > 0;
        }

        /// <summary>
        /// 检查是否为指定 Transform 的子级 (直接或间接)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="potentialParent">潜在父级</param>
        /// <returns>如果是子级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsChildOf(this Transform transform, Transform potentialParent)
        {
            return transform.IsChildOf(potentialParent);
        }

        /// <summary>
        /// 检查是否为指定 Transform 的直接子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="potentialParent">潜在父级</param>
        /// <returns>如果是直接子级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDirectChildOf(this Transform transform, Transform potentialParent)
        {
            return transform.parent == potentialParent;
        }

        /// <summary>
        /// 检查是否为指定 Transform 的父级 (直接或间接)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="potentialChild">潜在子级</param>
        /// <returns>如果是父级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsParentOf(this Transform transform, Transform potentialChild)
        {
            return potentialChild.IsChildOf(transform);
        }

        /// <summary>
        /// 检查是否为兄弟节点 (同一父级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="other">另一个 Transform</param>
        /// <returns>如果是兄弟节点返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSiblingOf(this Transform transform, Transform other)
        {
            return transform.parent == other.parent;
        }

        #endregion

        #region 子级索引操作

        /// <summary>
        /// 将 Transform 移动到兄弟节点的第一个位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetAsFirstSibling(this Transform transform)
        {
            transform.SetSiblingIndex(0);
            return transform;
        }

        /// <summary>
        /// 将 Transform 移动到兄弟节点的最后一个位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MoveToLastSibling(this Transform transform)
        {
            transform.SetAsLastSibling();
            return transform;
        }

        /// <summary>
        /// 将 Transform 在兄弟节点中向前移动一位
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform MoveSiblingUp(this Transform transform)
        {
            var index = transform.GetSiblingIndex();
            if (index > 0)
            {
                transform.SetSiblingIndex(index - 1);
            }
            return transform;
        }

        /// <summary>
        /// 将 Transform 在兄弟节点中向后移动一位
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform MoveSiblingDown(this Transform transform)
        {
            var index = transform.GetSiblingIndex();
            var parent = transform.parent;
            if (parent != null && index < parent.childCount - 1)
            {
                transform.SetSiblingIndex(index + 1);
            }
            return transform;
        }

        /// <summary>
        /// 检查是否为第一个兄弟节点
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果是第一个兄弟节点返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFirstSibling(this Transform transform)
        {
            return transform.GetSiblingIndex() == 0;
        }

        /// <summary>
        /// 检查是否为最后一个兄弟节点
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果是最后一个兄弟节点返回 true</returns>
        public static bool IsLastSibling(this Transform transform)
        {
            var parent = transform.parent;
            if (parent == null) return true;
            return transform.GetSiblingIndex() == parent.childCount - 1;
        }

        #endregion

        #region 获取子级

        /// <summary>
        /// 获取第一个子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>第一个子级，如果没有子级返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetFirstChild(this Transform transform)
        {
            return transform.childCount > 0 ? transform.GetChild(0) : null;
        }

        /// <summary>
        /// 获取最后一个子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>最后一个子级，如果没有子级返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetLastChild(this Transform transform)
        {
            var count = transform.childCount;
            return count > 0 ? transform.GetChild(count - 1) : null;
        }

        /// <summary>
        /// 安全获取子级 (索引越界返回 null)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="index">子级索引</param>
        /// <returns>子级 Transform，如果索引越界返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetChildSafe(this Transform transform, int index)
        {
            return index >= 0 && index < transform.childCount ? transform.GetChild(index) : null;
        }

        /// <summary>
        /// 获取所有直接子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>直接子级列表</returns>
        public static List<Transform> GetChildren(this Transform transform)
        {
            var children = new List<Transform>(transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }
            return children;
        }

        /// <summary>
        /// 获取所有直接子级 (使用现有列表，避免分配)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="result">结果列表</param>
        public static void GetChildren(this Transform transform, List<Transform> result)
        {
            result.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                result.Add(transform.GetChild(i));
            }
        }

        /// <summary>
        /// 获取所有子级 (递归，包括所有后代)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>所有子级列表</returns>
        public static List<Transform> GetAllChildren(this Transform transform, bool includeSelf = false)
        {
            var result = new List<Transform>();
            if (includeSelf)
            {
                result.Add(transform);
            }
            GetAllChildrenRecursive(transform, result);
            return result;
        }

        private static void GetAllChildrenRecursive(Transform transform, List<Transform> result)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                result.Add(child);
                GetAllChildrenRecursive(child, result);
            }
        }

        /// <summary>
        /// 获取所有子级 (递归，使用现有列表)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="result">结果列表</param>
        /// <param name="includeSelf">是否包含自身</param>
        public static void GetAllChildren(this Transform transform, List<Transform> result, bool includeSelf = false)
        {
            result.Clear();
            if (includeSelf)
            {
                result.Add(transform);
            }
            GetAllChildrenRecursive(transform, result);
        }

        #endregion

        #region 获取父级

        /// <summary>
        /// 获取根级 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>根级 Transform</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetRoot(this Transform transform)
        {
            return transform.root;
        }

        /// <summary>
        /// 获取指定层级的父级 (0 = 自身, 1 = 父级, 2 = 祖父级...)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="level">层级数</param>
        /// <returns>指定层级的父级，如果层级超出返回 null</returns>
        public static Transform GetParentAtLevel(this Transform transform, int level)
        {
            var current = transform;
            for (int i = 0; i < level && current != null; i++)
            {
                current = current.parent;
            }
            return current;
        }

        /// <summary>
        /// 获取所有父级 (从直接父级到根级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>所有父级列表</returns>
        public static List<Transform> GetAllParents(this Transform transform, bool includeSelf = false)
        {
            var result = new List<Transform>();
            if (includeSelf)
            {
                result.Add(transform);
            }
            var current = transform.parent;
            while (current != null)
            {
                result.Add(current);
                current = current.parent;
            }
            return result;
        }

        /// <summary>
        /// 获取层级深度 (根级为 0)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>层级深度</returns>
        public static int GetDepth(this Transform transform)
        {
            int depth = 0;
            var current = transform.parent;
            while (current != null)
            {
                depth++;
                current = current.parent;
            }
            return depth;
        }

        #endregion

        #region 获取兄弟节点

        /// <summary>
        /// 获取下一个兄弟节点
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>下一个兄弟节点，如果没有返回 null</returns>
        public static Transform GetNextSibling(this Transform transform)
        {
            var parent = transform.parent;
            if (parent == null) return null;
            
            var index = transform.GetSiblingIndex();
            return index < parent.childCount - 1 ? parent.GetChild(index + 1) : null;
        }

        /// <summary>
        /// 获取上一个兄弟节点
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>上一个兄弟节点，如果没有返回 null</returns>
        public static Transform GetPreviousSibling(this Transform transform)
        {
            var parent = transform.parent;
            if (parent == null) return null;
            
            var index = transform.GetSiblingIndex();
            return index > 0 ? parent.GetChild(index - 1) : null;
        }

        /// <summary>
        /// 获取所有兄弟节点 (不包含自身)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>所有兄弟节点列表</returns>
        public static List<Transform> GetSiblings(this Transform transform)
        {
            var result = new List<Transform>();
            var parent = transform.parent;
            if (parent == null) return result;
            
            for (int i = 0; i < parent.childCount; i++)
            {
                var sibling = parent.GetChild(i);
                if (sibling != transform)
                {
                    result.Add(sibling);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取兄弟节点数量 (不包含自身)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>兄弟节点数量</returns>
        public static int GetSiblingCount(this Transform transform)
        {
            var parent = transform.parent;
            return parent != null ? parent.childCount - 1 : 0;
        }

        #endregion

        #region 子级操作

        /// <summary>
        /// 销毁所有子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="immediate">是否立即销毁 (编辑器模式下使用)</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform DestroyAllChildren(this Transform transform, bool immediate = false)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (immediate)
                {
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }
            return transform;
        }

        /// <summary>
        /// 分离所有子级 (设置为根级)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform DetachAllChildren(this Transform transform)
        {
            transform.DetachChildren();
            return transform;
        }

        /// <summary>
        /// 对所有直接子级执行操作
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ForEachChild(this Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action(transform.GetChild(i));
            }
            return transform;
        }

        /// <summary>
        /// 对所有子级执行操作 (递归)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ForEachChildRecursive(this Transform transform, Action<Transform> action, bool includeSelf = false)
        {
            if (includeSelf)
            {
                action(transform);
            }
            ForEachChildRecursiveInternal(transform, action);
            return transform;
        }

        private static void ForEachChildRecursiveInternal(Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                action(child);
                ForEachChildRecursiveInternal(child, action);
            }
        }

        /// <summary>
        /// 对所有直接子级执行操作 (带索引)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="action">要执行的操作 (Transform, 索引)</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ForEachChildWithIndex(this Transform transform, Action<Transform, int> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action(transform.GetChild(i), i);
            }
            return transform;
        }

        /// <summary>
        /// 排序子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="comparison">比较函数</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform SortChildren(this Transform transform, Comparison<Transform> comparison)
        {
            var children = transform.GetChildren();
            children.Sort(comparison);
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
            return transform;
        }

        /// <summary>
        /// 按名称排序子级
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="descending">是否降序</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform SortChildrenByName(this Transform transform, bool descending = false)
        {
            return transform.SortChildren((a, b) =>
            {
                var result = string.Compare(a.name, b.name, StringComparison.Ordinal);
                return descending ? -result : result;
            });
        }

        /// <summary>
        /// 反转子级顺序
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ReverseChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).SetAsFirstSibling();
            }
            return transform;
        }

        /// <summary>
        /// 随机打乱子级顺序
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ShuffleChildren(this Transform transform)
        {
            var count = transform.childCount;
            for (int i = count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                transform.GetChild(j).SetSiblingIndex(i);
            }
            return transform;
        }

        #endregion

        #region 路径操作

        /// <summary>
        /// 获取从根级到当前 Transform 的路径
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="separator">路径分隔符</param>
        /// <returns>路径字符串</returns>
        public static string GetPath(this Transform transform, string separator = "/")
        {
            var path = transform.name;
            var current = transform.parent;
            while (current != null)
            {
                path = current.name + separator + path;
                current = current.parent;
            }
            return path;
        }

        /// <summary>
        /// 获取相对于指定父级的路径
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="relativeTo">相对的父级</param>
        /// <param name="separator">路径分隔符</param>
        /// <returns>相对路径字符串，如果不是子级返回 null</returns>
        public static string GetRelativePath(this Transform transform, Transform relativeTo, string separator = "/")
        {
            if (!transform.IsChildOf(relativeTo)) return null;
            
            var path = transform.name;
            var current = transform.parent;
            while (current != null && current != relativeTo)
            {
                path = current.name + separator + path;
                current = current.parent;
            }
            return path;
        }

        #endregion
    }
}
