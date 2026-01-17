// ==========================================================
// 文件名：CollectionExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 集合扩展方法，提供集合映射辅助功能
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.AMapper
{
    /// <summary>
    /// 集合扩展方法
    /// <para>提供集合映射、转换等辅助功能</para>
    /// <para>Collection extension methods providing mapping and conversion utilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一集合操作</item>
    /// <item>流畅接口：支持链式调用</item>
    /// <item>性能优化：使用延迟执行和流式处理</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 映射集合
    /// var dtos = players.MapTo&lt;PlayerDto&gt;(mapper);
    /// 
    /// // 映射并过滤
    /// var activeDtos = players
    ///     .Where(p => p.IsActive)
    ///     .MapTo&lt;PlayerDto&gt;(mapper);
    /// </code>
    /// </remarks>
    public static class CollectionExtensions
    {
        #region 集合映射 / Collection Mapping

        /// <summary>
        /// 映射集合到目标类型
        /// <para>Map collection to destination type</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标集合 / Destination collection</returns>
        public static IEnumerable<TDestination> MapTo<TDestination>(
            this IEnumerable<object> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TDestination>(item));
        }

        /// <summary>
        /// 映射集合到目标类型（强类型源）
        /// <para>Map collection to destination type (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标集合 / Destination collection</returns>
        public static IEnumerable<TDestination> MapTo<TSource, TDestination>(
            this IEnumerable<TSource> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TSource, TDestination>(item));
        }

        /// <summary>
        /// 映射集合到列表
        /// <para>Map collection to list</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标列表 / Destination list</returns>
        public static List<TDestination> MapToList<TDestination>(
            this IEnumerable<object> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TDestination>(item)).ToList();
        }

        /// <summary>
        /// 映射集合到列表（强类型源）
        /// <para>Map collection to list (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标列表 / Destination list</returns>
        public static List<TDestination> MapToList<TSource, TDestination>(
            this IEnumerable<TSource> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        }

        /// <summary>
        /// 映射集合到数组
        /// <para>Map collection to array</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标数组 / Destination array</returns>
        public static TDestination[] MapToArray<TDestination>(
            this IEnumerable<object> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TDestination>(item)).ToArray();
        }

        /// <summary>
        /// 映射集合到数组（强类型源）
        /// <para>Map collection to array (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标数组 / Destination array</returns>
        public static TDestination[] MapToArray<TSource, TDestination>(
            this IEnumerable<TSource> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.Select(item => mapper.Map<TSource, TDestination>(item)).ToArray();
        }

        #endregion

        #region 字典映射 / Dictionary Mapping

        /// <summary>
        /// 映射字典的值
        /// <para>Map dictionary values</para>
        /// </summary>
        /// <typeparam name="TKey">键类型 / Key type</typeparam>
        /// <typeparam name="TSourceValue">源值类型 / Source value type</typeparam>
        /// <typeparam name="TDestValue">目标值类型 / Destination value type</typeparam>
        /// <param name="source">源字典 / Source dictionary</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标字典 / Destination dictionary</returns>
        public static Dictionary<TKey, TDestValue> MapValues<TKey, TSourceValue, TDestValue>(
            this IDictionary<TKey, TSourceValue> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.ToDictionary(
                kvp => kvp.Key,
                kvp => mapper.Map<TSourceValue, TDestValue>(kvp.Value));
        }

        /// <summary>
        /// 映射字典的键
        /// <para>Map dictionary keys</para>
        /// </summary>
        /// <typeparam name="TSourceKey">源键类型 / Source key type</typeparam>
        /// <typeparam name="TDestKey">目标键类型 / Destination key type</typeparam>
        /// <typeparam name="TValue">值类型 / Value type</typeparam>
        /// <param name="source">源字典 / Source dictionary</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标字典 / Destination dictionary</returns>
        public static Dictionary<TDestKey, TValue> MapKeys<TSourceKey, TDestKey, TValue>(
            this IDictionary<TSourceKey, TValue> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.ToDictionary(
                kvp => mapper.Map<TSourceKey, TDestKey>(kvp.Key),
                kvp => kvp.Value);
        }

        /// <summary>
        /// 映射字典的键和值
        /// <para>Map dictionary keys and values</para>
        /// </summary>
        /// <typeparam name="TSourceKey">源键类型 / Source key type</typeparam>
        /// <typeparam name="TDestKey">目标键类型 / Destination key type</typeparam>
        /// <typeparam name="TSourceValue">源值类型 / Source value type</typeparam>
        /// <typeparam name="TDestValue">目标值类型 / Destination value type</typeparam>
        /// <param name="source">源字典 / Source dictionary</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>目标字典 / Destination dictionary</returns>
        public static Dictionary<TDestKey, TDestValue> MapDictionary<TSourceKey, TDestKey, TSourceValue, TDestValue>(
            this IDictionary<TSourceKey, TSourceValue> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return source.ToDictionary(
                kvp => mapper.Map<TSourceKey, TDestKey>(kvp.Key),
                kvp => mapper.Map<TSourceValue, TDestValue>(kvp.Value));
        }

        #endregion

        #region 条件映射 / Conditional Mapping

        /// <summary>
        /// 映射满足条件的元素
        /// <para>Map elements that meet condition</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="predicate">条件 / Predicate</param>
        /// <returns>目标集合 / Destination collection</returns>
        public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
            this IEnumerable<TSource> source,
            IAMapper mapper,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return source
                .Where(predicate)
                .Select(item => mapper.Map<TSource, TDestination>(item));
        }

        #endregion

        #region 分页映射 / Paging Mapping

        /// <summary>
        /// 映射分页数据
        /// <para>Map paged data</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源集合 / Source collection</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="pageIndex">页索引（从 0 开始）/ Page index (0-based)</param>
        /// <param name="pageSize">页大小 / Page size</param>
        /// <returns>目标集合 / Destination collection</returns>
        public static IEnumerable<TDestination> MapPage<TSource, TDestination>(
            this IEnumerable<TSource> source,
            IAMapper mapper,
            int pageIndex,
            int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "页索引不能为负数 / Page index cannot be negative");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "页大小必须大于 0 / Page size must be greater than 0");

            return source
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(item => mapper.Map<TSource, TDestination>(item));
        }

        #endregion
    }
}
