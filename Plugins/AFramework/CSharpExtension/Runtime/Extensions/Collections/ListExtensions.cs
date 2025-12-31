// ==========================================================
// 文件名：ListExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// List 扩展方法
    /// <para>提供 List 的常用操作扩展，包括安全访问、随机、批量操作等功能</para>
    /// </summary>
    public static class ListExtensions
    {
        #region 安全访问

        /// <summary>
        /// 安全获取 List 元素，索引越界时返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>(this List<T> list, int index, T defaultValue = default)
        {
            if (list == null || index < 0 || index >= list.Count)
                return defaultValue;
            return list[index];
        }

        /// <summary>
        /// 尝试获取 List 元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this List<T> list, int index, out T value)
        {
            if (list != null && index >= 0 && index < list.Count)
            {
                value = list[index];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 检查 List 是否为空或 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// 检查 List 是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }

        /// <summary>
        /// 检查索引是否在 List 范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex<T>(this List<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }

        /// <summary>
        /// 获取最后一个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new InvalidOperationException("List is null or empty.");
            return list[list.Count - 1];
        }

        /// <summary>
        /// 尝试获取最后一个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryLast<T>(this List<T> list, out T value)
        {
            if (list.HasElements())
            {
                value = list[list.Count - 1];
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region 随机操作

        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取随机元素
        /// </summary>
        public static T Random<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new ArgumentException("List is null or empty.", nameof(list));
            return list[_random.Next(list.Count)];
        }

        /// <summary>
        /// 尝试获取随机元素
        /// </summary>
        public static bool TryRandom<T>(this List<T> list, out T value)
        {
            if (list.HasElements())
            {
                value = list[_random.Next(list.Count)];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 打乱 List 顺序（Fisher-Yates 洗牌算法）
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null || list.Count <= 1)
                return;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// 获取打乱后的新 List
        /// </summary>
        public static List<T> Shuffled<T>(this List<T> list)
        {
            if (list == null)
                return new List<T>();

            var result = new List<T>(list);
            result.Shuffle();
            return result;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量添加元素
        /// </summary>
        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            if (list == null || items == null)
                return;
            list.AddRange(items);
        }

        /// <summary>
        /// 添加元素（如果不存在）
        /// </summary>
        public static bool AddIfNotContains<T>(this List<T> list, T item)
        {
            if (list == null)
                return false;

            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量添加元素（如果不存在）
        /// </summary>
        public static int AddRangeIfNotContains<T>(this List<T> list, IEnumerable<T> items)
        {
            if (list == null || items == null)
                return 0;

            int count = 0;
            foreach (var item in items)
            {
                if (list.AddIfNotContains(item))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        public static int RemoveAll<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null || predicate == null)
                return 0;
            return list.RemoveAll(new Predicate<T>(predicate));
        }

        /// <summary>
        /// 移除最后一个元素
        /// </summary>
        public static bool RemoveLast<T>(this List<T> list)
        {
            if (list.HasElements())
            {
                list.RemoveAt(list.Count - 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除并返回最后一个元素（类似栈的 Pop）
        /// </summary>
        public static T PopLast<T>(this List<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new InvalidOperationException("List is null or empty.");

            var last = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return last;
        }

        /// <summary>
        /// 尝试移除并返回最后一个元素
        /// </summary>
        public static bool TryPopLast<T>(this List<T> list, out T value)
        {
            if (list.HasElements())
            {
                value = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找第一个满足条件的元素索引
        /// </summary>
        public static int FindIndex<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null || predicate == null)
                return -1;
            return list.FindIndex(new Predicate<T>(predicate));
        }

        /// <summary>
        /// 查找最后一个满足条件的元素索引
        /// </summary>
        public static int FindLastIndex<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null || predicate == null)
                return -1;
            return list.FindLastIndex(new Predicate<T>(predicate));
        }

        /// <summary>
        /// 查找所有满足条件的元素索引
        /// </summary>
        public static List<int> FindAllIndices<T>(this List<T> list, Func<T, bool> predicate)
        {
            var indices = new List<int>();
            if (list == null || predicate == null)
                return indices;

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    indices.Add(i);
            }
            return indices;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this List<T> list)
        {
            return list == null ? Array.Empty<T>() : list.ToArray();
        }

        /// <summary>
        /// 转换为 HashSet
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>(this List<T> list)
        {
            return list == null ? new HashSet<T>() : new HashSet<T>(list);
        }

        /// <summary>
        /// 映射为新 List
        /// </summary>
        public static List<TResult> Map<TSource, TResult>(this List<TSource> list, Func<TSource, TResult> selector)
        {
            if (list == null || selector == null)
                return new List<TResult>();

            var result = new List<TResult>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(selector(list[i]));
            }
            return result;
        }

        #endregion

        #region 切片操作

        /// <summary>
        /// 获取 List 切片
        /// </summary>
        public static List<T> Slice<T>(this List<T> list, int startIndex, int count)
        {
            if (list == null || startIndex < 0 || count < 0)
                return new List<T>();

            startIndex = Math.Max(0, Math.Min(startIndex, list.Count));
            count = Math.Min(count, list.Count - startIndex);

            return list.GetRange(startIndex, count);
        }

        /// <summary>
        /// 获取前 N 个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Take<T>(this List<T> list, int count)
        {
            return list.Slice(0, count);
        }

        /// <summary>
        /// 跳过前 N 个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Skip<T>(this List<T> list, int count)
        {
            if (list == null)
                return new List<T>();
            return list.Slice(count, list.Count - count);
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对 List 元素执行操作
        /// </summary>
        public static void ForEach<T>(this List<T> list, Action<T> action)
        {
            if (list == null || action == null)
                return;
            list.ForEach(action);
        }

        /// <summary>
        /// 对 List 元素执行操作（带索引）
        /// </summary>
        public static void ForEach<T>(this List<T> list, Action<T, int> action)
        {
            if (list == null || action == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                action(list[i], i);
            }
        }

        #endregion

        #region 去重操作

        /// <summary>
        /// 移除重复元素（保持原顺序）
        /// </summary>
        public static void RemoveDuplicates<T>(this List<T> list)
        {
            if (list == null || list.Count <= 1)
                return;

            var seen = new HashSet<T>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!seen.Add(list[i]))
                    list.RemoveAt(i);
            }
        }

        /// <summary>
        /// 获取去重后的新 List
        /// </summary>
        public static List<T> Distinct<T>(this List<T> list)
        {
            if (list == null)
                return new List<T>();

            var result = new List<T>(list);
            result.RemoveDuplicates();
            return result;
        }

        #endregion

        #region 交换操作

        /// <summary>
        /// 交换两个元素的位置
        /// </summary>
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            if (list == null || !list.IsValidIndex(index1) || !list.IsValidIndex(index2))
                return;

            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        #endregion

        #region 分块操作

        /// <summary>
        /// 将 List 分块
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">List</param>
        /// <param name="chunkSize">块大小</param>
        /// <returns>分块后的 List 集合</returns>
        public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        {
            var result = new List<List<T>>();
            if (list == null || chunkSize <= 0)
                return result;

            for (int i = 0; i < list.Count; i += chunkSize)
            {
                int count = Math.Min(chunkSize, list.Count - i);
                result.Add(list.GetRange(i, count));
            }
            return result;
        }

        #endregion
    }
}
