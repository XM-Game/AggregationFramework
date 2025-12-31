// ==========================================================
// 文件名：HashSetExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// HashSet 扩展方法
    /// <para>提供 HashSet 的常用操作扩展，包括批量操作、集合运算等功能</para>
    /// </summary>
    public static class HashSetExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查 HashSet 是否为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this HashSet<T> set)
        {
            return set == null || set.Count == 0;
        }

        /// <summary>
        /// 检查 HashSet 是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this HashSet<T> set)
        {
            return set != null && set.Count > 0;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量添加元素
        /// </summary>
        public static int AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            if (set == null || items == null)
                return 0;

            int count = 0;
            foreach (var item in items)
            {
                if (set.Add(item))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 批量添加元素（参数数组）
        /// </summary>
        public static int AddRange<T>(this HashSet<T> set, params T[] items)
        {
            return set.AddRange((IEnumerable<T>)items);
        }

        /// <summary>
        /// 批量移除元素
        /// </summary>
        public static int RemoveRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            if (set == null || items == null)
                return 0;

            int count = 0;
            foreach (var item in items)
            {
                if (set.Remove(item))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        public static int RemoveWhere<T>(this HashSet<T> set, Func<T, bool> predicate)
        {
            if (set == null || predicate == null)
                return 0;

            return set.RemoveWhere(new Predicate<T>(predicate));
        }

        #endregion

        #region 集合运算

        /// <summary>
        /// 添加另一个集合的所有元素（并集）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnionWith<T>(this HashSet<T> set, params T[] items)
        {
            if (set != null && items != null)
                set.UnionWith(items);
        }

        /// <summary>
        /// 保留与另一个集合的交集
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IntersectWith<T>(this HashSet<T> set, params T[] items)
        {
            if (set != null && items != null)
                set.IntersectWith(items);
        }

        /// <summary>
        /// 移除另一个集合中的所有元素（差集）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExceptWith<T>(this HashSet<T> set, params T[] items)
        {
            if (set != null && items != null)
                set.ExceptWith(items);
        }

        /// <summary>
        /// 对称差集（只保留在一个集合中的元素）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SymmetricExceptWith<T>(this HashSet<T> set, params T[] items)
        {
            if (set != null && items != null)
                set.SymmetricExceptWith(items);
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ToList<T>(this HashSet<T> set)
        {
            return set == null ? new List<T>() : new List<T>(set);
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this HashSet<T> set)
        {
            if (set == null)
                return Array.Empty<T>();

            var array = new T[set.Count];
            set.CopyTo(array);
            return array;
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对 HashSet 元素执行操作
        /// </summary>
        public static void ForEach<T>(this HashSet<T> set, Action<T> action)
        {
            if (set == null || action == null)
                return;

            foreach (var item in set)
            {
                action(item);
            }
        }

        #endregion

        #region 随机操作

        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取随机元素
        /// </summary>
        public static T Random<T>(this HashSet<T> set)
        {
            if (set.IsNullOrEmpty())
                throw new InvalidOperationException("HashSet is null or empty.");

            int index = _random.Next(set.Count);
            int current = 0;
            foreach (var item in set)
            {
                if (current == index)
                    return item;
                current++;
            }
            throw new InvalidOperationException("Failed to get random element.");
        }

        /// <summary>
        /// 尝试获取随机元素
        /// </summary>
        public static bool TryRandom<T>(this HashSet<T> set, out T value)
        {
            if (set.HasElements())
            {
                value = set.Random();
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region 克隆操作

        /// <summary>
        /// 克隆 HashSet
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> Clone<T>(this HashSet<T> set)
        {
            return set == null ? new HashSet<T>() : new HashSet<T>(set);
        }

        #endregion
    }
}
