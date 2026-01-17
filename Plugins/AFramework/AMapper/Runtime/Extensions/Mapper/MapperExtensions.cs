// ==========================================================
// 文件名：MapperExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: IAMapper 扩展方法，提供便捷的映射操作
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.AMapper
{
    /// <summary>
    /// IAMapper 扩展方法
    /// <para>提供便捷的映射操作，包括集合映射、条件映射等</para>
    /// <para>IAMapper extension methods providing convenient mapping operations</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一映射场景</item>
    /// <item>流畅接口：支持链式调用</item>
    /// <item>性能优化：避免不必要的中间对象创建</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 映射集合
    /// var dtos = mapper.MapList&lt;PlayerDto&gt;(players);
    /// 
    /// // 条件映射
    /// var dto = mapper.MapIf(player, p => p.IsActive);
    /// 
    /// // 映射并执行操作
    /// var dto = mapper.MapAndDo(player, d => d.CreatedAt = DateTime.Now);
    /// </code>
    /// </remarks>
    public static class MapperExtensions
    {
        #region 集合映射 / Collection Mapping

        /// <summary>
        /// 映射集合到列表
        /// <para>Map collection to list</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>目标列表 / Destination list</returns>
        public static List<TDestination> MapList<TDestination>(this IAMapper mapper, IEnumerable<object> source)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return new List<TDestination>();

            return source.Select(item => mapper.Map<TDestination>(item)).ToList();
        }

        /// <summary>
        /// 映射集合到列表（强类型源）
        /// <para>Map collection to list (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>目标列表 / Destination list</returns>
        public static List<TDestination> MapList<TSource, TDestination>(this IAMapper mapper, IEnumerable<TSource> source)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return new List<TDestination>();

            return source.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        }

        /// <summary>
        /// 映射集合到数组
        /// <para>Map collection to array</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>目标数组 / Destination array</returns>
        public static TDestination[] MapArray<TDestination>(this IAMapper mapper, IEnumerable<object> source)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return Array.Empty<TDestination>();

            return source.Select(item => mapper.Map<TDestination>(item)).ToArray();
        }

        /// <summary>
        /// 映射集合到数组（强类型源）
        /// <para>Map collection to array (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源集合 / Source collection</param>
        /// <returns>目标数组 / Destination array</returns>
        public static TDestination[] MapArray<TSource, TDestination>(this IAMapper mapper, IEnumerable<TSource> source)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return Array.Empty<TDestination>();

            return source.Select(item => mapper.Map<TSource, TDestination>(item)).ToArray();
        }

        #endregion

        #region 条件映射 / Conditional Mapping

        /// <summary>
        /// 条件映射（满足条件时映射，否则返回默认值）
        /// <para>Conditional mapping (map if condition is met, otherwise return default)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="condition">条件 / Condition</param>
        /// <returns>目标对象或默认值 / Destination object or default</returns>
        public static TDestination MapIf<TSource, TDestination>(
            this IAMapper mapper,
            TSource source,
            Func<TSource, bool> condition)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (source == null || !condition(source))
                return default;

            return mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 条件映射（满足条件时映射，否则返回指定默认值）
        /// <para>Conditional mapping (map if condition is met, otherwise return specified default)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="condition">条件 / Condition</param>
        /// <param name="defaultValue">默认值 / Default value</param>
        /// <returns>目标对象或默认值 / Destination object or default</returns>
        public static TDestination MapIfOrDefault<TSource, TDestination>(
            this IAMapper mapper,
            TSource source,
            Func<TSource, bool> condition,
            TDestination defaultValue)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (source == null || !condition(source))
                return defaultValue;

            return mapper.Map<TSource, TDestination>(source);
        }

        #endregion

        #region 映射后操作 / Post-Mapping Actions

        /// <summary>
        /// 映射并执行操作
        /// <para>Map and execute action</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="action">操作 / Action</param>
        /// <returns>目标对象 / Destination object</returns>
        public static TDestination MapAndDo<TSource, TDestination>(
            this IAMapper mapper,
            TSource source,
            Action<TDestination> action)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var destination = mapper.Map<TSource, TDestination>(source);
            if (destination != null)
            {
                action(destination);
            }

            return destination;
        }

        /// <summary>
        /// 映射并执行操作（带源对象参数）
        /// <para>Map and execute action (with source parameter)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="action">操作 / Action</param>
        /// <returns>目标对象 / Destination object</returns>
        public static TDestination MapAndDo<TSource, TDestination>(
            this IAMapper mapper,
            TSource source,
            Action<TSource, TDestination> action)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var destination = mapper.Map<TSource, TDestination>(source);
            if (destination != null)
            {
                action(source, destination);
            }

            return destination;
        }

        #endregion

        #region 空值安全映射 / Null-Safe Mapping

        /// <summary>
        /// 空值安全映射（源为 null 时返回 null）
        /// <para>Null-safe mapping (return null if source is null)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>目标对象或 null / Destination object or null</returns>
        public static TDestination MapOrNull<TSource, TDestination>(this IAMapper mapper, TSource source)
            where TDestination : class
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return null;

            return mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 空值安全映射（源为 null 时返回默认值）
        /// <para>Null-safe mapping (return default if source is null)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="defaultValue">默认值 / Default value</param>
        /// <returns>目标对象或默认值 / Destination object or default</returns>
        public static TDestination MapOrDefault<TSource, TDestination>(
            this IAMapper mapper,
            TSource source,
            TDestination defaultValue)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (source == null)
                return defaultValue;

            return mapper.Map<TSource, TDestination>(source);
        }

        #endregion

        #region 批量映射 / Batch Mapping

        /// <summary>
        /// 批量映射到现有对象
        /// <para>Batch map to existing objects</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="sources">源集合 / Source collection</param>
        /// <param name="destinations">目标集合 / Destination collection</param>
        public static void MapToExisting<TSource, TDestination>(
            this IAMapper mapper,
            IEnumerable<TSource> sources,
            IList<TDestination> destinations)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            if (destinations == null)
                throw new ArgumentNullException(nameof(destinations));

            var sourceArray = sources.ToArray();
            var count = Math.Min(sourceArray.Length, destinations.Count);

            for (int i = 0; i < count; i++)
            {
                mapper.Map(sourceArray[i], destinations[i]);
            }
        }

        #endregion
    }
}
