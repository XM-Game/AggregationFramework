// ==========================================================
// 文件名：EnumerableExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: IEnumerable 扩展方法，提供通用集合操作
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.AMapper
{
    /// <summary>
    /// IEnumerable 扩展方法
    /// <para>提供通用集合操作和辅助功能</para>
    /// <para>IEnumerable extension methods providing common collection operations</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一集合操作</item>
    /// <item>流畅接口：支持链式调用</item>
    /// <item>性能优化：使用延迟执行</item>
    /// </list>
    /// </remarks>
    public static class EnumerableExtensions
    {
        #region 空值处理 / Null Handling

        /// <summary>
        /// 确保集合不为 null（为 null 时返回空集合）
        /// <para>Ensure collection is not null (return empty if null)</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>非 null 集合 / Non-null collection</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// 过滤 null 元素
        /// <para>Filter null elements</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>不包含 null 的集合 / Collection without nulls</returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Where(item => item != null);
        }

        #endregion

        #region 批处理 / Batch Processing

        /// <summary>
        /// 将集合分批
        /// <para>Batch collection into chunks</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="batchSize">批大小 / Batch size</param>
        /// <returns>批集合 / Batched collection</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize), "批大小必须大于 0 / Batch size must be greater than 0");

            var batch = new List<T>(batchSize);
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }

        #endregion

        #region 去重 / Distinct

        /// <summary>
        /// 根据键去重
        /// <para>Distinct by key</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TKey">键类型 / Key type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="keySelector">键选择器 / Key selector</param>
        /// <returns>去重后的集合 / Distinct collection</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var seenKeys = new HashSet<TKey>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (seenKeys.Add(key))
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region 遍历 / Iteration

        /// <summary>
        /// 对每个元素执行操作
        /// <para>Execute action for each element</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="action">操作 / Action</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// 对每个元素执行操作（带索引）
        /// <para>Execute action for each element (with index)</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="action">操作 / Action</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            int index = 0;
            foreach (var item in source)
            {
                action(item, index++);
            }
        }

        #endregion

        #region 条件操作 / Conditional Operations

        /// <summary>
        /// 条件过滤（满足条件时应用过滤器）
        /// <para>Conditional where (apply filter if condition is met)</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="condition">条件 / Condition</param>
        /// <param name="predicate">过滤器 / Predicate</param>
        /// <returns>过滤后的集合 / Filtered collection</returns>
        public static IEnumerable<T> WhereIf<T>(
            this IEnumerable<T> source,
            bool condition,
            Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return condition ? source.Where(predicate) : source;
        }

        #endregion

        #region 集合判断 / Collection Checks

        /// <summary>
        /// 判断集合是否为空或 null
        /// <para>Check if collection is null or empty</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>是否为空或 null / Whether is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 判断集合是否有元素
        /// <para>Check if collection has any elements</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>是否有元素 / Whether has elements</returns>
        public static bool HasAny<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        #endregion

        #region 转换 / Conversion

        /// <summary>
        /// 转换为 HashSet
        /// <para>Convert to HashSet</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>HashSet / HashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new HashSet<T>(source);
        }

        /// <summary>
        /// 转换为 HashSet（带比较器）
        /// <para>Convert to HashSet (with comparer)</para>
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="comparer">比较器 / Comparer</param>
        /// <returns>HashSet / HashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new HashSet<T>(source, comparer);
        }

        #endregion
    }
}
