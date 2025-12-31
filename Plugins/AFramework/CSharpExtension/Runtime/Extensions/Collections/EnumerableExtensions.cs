// ==========================================================
// 文件名：EnumerableExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// IEnumerable 扩展方法
    /// <para>提供 LINQ 增强扩展，包括批量操作、分组、去重等功能</para>
    /// </summary>
    public static class EnumerableExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查集合是否为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 检查集合是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对集合元素执行操作
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 对集合元素执行操作（带索引）
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null || action == null)
                return;

            int index = 0;
            foreach (var item in source)
            {
                action(item, index++);
            }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 HashSet
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return source == null ? new HashSet<T>() : new HashSet<T>(source);
        }

        /// <summary>
        /// 转换为 Queue
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return source == null ? new Queue<T>() : new Queue<T>(source);
        }

        /// <summary>
        /// 转换为 Stack
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        {
            return source == null ? new Stack<T>() : new Stack<T>(source);
        }

        /// <summary>
        /// 转换为字典（自动处理重复键）
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
        {
            var result = new Dictionary<TKey, TValue>();
            if (source == null || keySelector == null || valueSelector == null)
                return result;

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (key != null && !result.ContainsKey(key))
                    result[key] = valueSelector(item);
            }
            return result;
        }

        #endregion

        #region 分块操作

        /// <summary>
        /// 将集合分块
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null || chunkSize <= 0)
                yield break;

            var chunk = new List<T>(chunkSize);
            foreach (var item in source)
            {
                chunk.Add(item);
                if (chunk.Count == chunkSize)
                {
                    yield return chunk;
                    chunk = new List<T>(chunkSize);
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }

        /// <summary>
        /// 将集合分批处理
        /// </summary>
        public static void Batch<T>(this IEnumerable<T> source, int batchSize, Action<IEnumerable<T>> action)
        {
            if (source == null || action == null || batchSize <= 0)
                return;

            foreach (var batch in source.Chunk(batchSize))
            {
                action(batch);
            }
        }

        #endregion

        #region 去重操作

        /// <summary>
        /// 根据指定键去重
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null || keySelector == null)
                yield break;

            var seenKeys = new HashSet<TKey>();
            foreach (var item in source)
            {
                if (seenKeys.Add(keySelector(item)))
                    yield return item;
            }
        }

        #endregion

        #region 随机操作


        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取随机元素
        /// </summary>
        public static T Random<T>(this IEnumerable<T> source)
        {
            if (source.IsNullOrEmpty())
                throw new InvalidOperationException("Sequence contains no elements.");

            var list = source as IList<T> ?? source.ToList();
            return list[_random.Next(list.Count)];
        }

        /// <summary>
        /// 尝试获取随机元素
        /// </summary>
        public static bool TryRandom<T>(this IEnumerable<T> source, out T value)
        {
            if (source.HasElements())
            {
                var list = source as IList<T> ?? source.ToList();
                value = list[_random.Next(list.Count)];
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 获取多个随机元素
        /// </summary>
        public static IEnumerable<T> RandomElements<T>(this IEnumerable<T> source, int count)
        {
            if (source.IsNullOrEmpty() || count <= 0)
                return Enumerable.Empty<T>();

            var list = source as IList<T> ?? source.ToList();
            count = Math.Min(count, list.Count);

            var indices = new HashSet<int>();
            while (indices.Count < count)
            {
                indices.Add(_random.Next(list.Count));
            }

            return indices.Select(i => list[i]);
        }

        /// <summary>
        /// 打乱集合顺序
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source.IsNullOrEmpty())
                yield break;

            var list = source.ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找第一个满足条件的元素索引
        /// </summary>
        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null || predicate == null)
                return -1;

            int index = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 查找所有满足条件的元素索引
        /// </summary>
        public static IEnumerable<int> FindAllIndices<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null || predicate == null)
                yield break;

            int index = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                    yield return index;
                index++;
            }
        }

        #endregion

        #region 最值操作

        /// <summary>
        /// 获取最大值元素
        /// </summary>
        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (source.IsNullOrEmpty() || keySelector == null)
                throw new InvalidOperationException("Sequence contains no elements.");

            T maxItem = default;
            TKey maxKey = default;
            bool first = true;

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxItem = item;
                    maxKey = key;
                    first = false;
                }
            }

            return maxItem;
        }

        /// <summary>
        /// 获取最小值元素
        /// </summary>
        public static T MinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (source.IsNullOrEmpty() || keySelector == null)
                throw new InvalidOperationException("Sequence contains no elements.");

            T minItem = default;
            TKey minKey = default;
            bool first = true;

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (first || key.CompareTo(minKey) < 0)
                {
                    minItem = item;
                    minKey = key;
                    first = false;
                }
            }

            return minItem;
        }

        #endregion

        #region 字符串操作

        /// <summary>
        /// 连接字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinString<T>(this IEnumerable<T> source, string separator = ", ")
        {
            return source == null ? string.Empty : string.Join(separator, source);
        }

        /// <summary>
        /// 连接字符串（带选择器）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string JoinString<T>(this IEnumerable<T> source, Func<T, string> selector, string separator = ", ")
        {
            return source == null || selector == null ? string.Empty : string.Join(separator, source.Select(selector));
        }

        #endregion

        #region 分页操作

        /// <summary>
        /// 分页获取数据
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source == null || pageIndex < 0 || pageSize <= 0)
                return Enumerable.Empty<T>();

            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        #endregion

        #region 条件操作

        /// <summary>
        /// 条件过滤（当条件为 true 时才应用过滤）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition && predicate != null ? source.Where(predicate) : source;
        }

        #endregion

        #region 集合比较

        /// <summary>
        /// 检查两个集合是否包含相同元素（忽略顺序）
        /// </summary>
        public static bool SequenceEqualIgnoreOrder<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null && second == null)
                return true;
            if (first == null || second == null)
                return false;

            var firstSet = first.ToHashSet();
            var secondSet = second.ToHashSet();

            return firstSet.SetEquals(secondSet);
        }

        #endregion

        #region 空值过滤

        /// <summary>
        /// 过滤掉 null 值
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
        {
            if (source == null)
                yield break;

            foreach (var item in source)
            {
                if (item != null)
                    yield return item;
            }
        }

        #endregion

        #region 统计操作

        /// <summary>
        /// 计算满足条件的元素数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source == null || predicate == null ? 0 : source.Count(predicate);
        }

        /// <summary>
        /// 检查是否有多个元素
        /// </summary>
        public static bool HasMultiple<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return false;

            using (var enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext() && enumerator.MoveNext();
            }
        }

        /// <summary>
        /// 检查是否只有一个元素
        /// </summary>
        public static bool HasSingle<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return false;

            using (var enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext() && !enumerator.MoveNext();
            }
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 安全的 FirstOrDefault（避免 null 集合异常）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstOrDefaultSafe<T>(this IEnumerable<T> source, T defaultValue = default)
        {
            return source.IsNullOrEmpty() ? defaultValue : source.FirstOrDefault();
        }

        /// <summary>
        /// 安全的 LastOrDefault（避免 null 集合异常）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LastOrDefaultSafe<T>(this IEnumerable<T> source, T defaultValue = default)
        {
            return source.IsNullOrEmpty() ? defaultValue : source.LastOrDefault();
        }

        #endregion

        #region 集合操作

        /// <summary>
        /// 添加元素到集合末尾
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] items)
        {
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }

            if (items != null)
            {
                foreach (var item in items)
                    yield return item;
            }
        }

        /// <summary>
        /// 添加元素到集合开头
        /// </summary>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] items)
        {
            if (items != null)
            {
                foreach (var item in items)
                    yield return item;
            }

            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
        }

        #endregion

        #region 交集并集差集

        /// <summary>
        /// 获取交集（使用自定义比较器）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> IntersectBy<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keySelector)
        {
            if (first == null || second == null || keySelector == null)
                return Enumerable.Empty<T>();

            var secondKeys = new HashSet<TKey>(second.Select(keySelector));
            return first.Where(item => secondKeys.Contains(keySelector(item)));
        }

        /// <summary>
        /// 获取差集（使用自定义比较器）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ExceptBy<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keySelector)
        {
            if (first == null || keySelector == null)
                return Enumerable.Empty<T>();
            if (second == null)
                return first;

            var secondKeys = new HashSet<TKey>(second.Select(keySelector));
            return first.Where(item => !secondKeys.Contains(keySelector(item)));
        }

        #endregion
    }
}
