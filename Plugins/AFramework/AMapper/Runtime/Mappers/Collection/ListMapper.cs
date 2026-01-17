// ==========================================================
// 文件名：ListMapper.cs
// 命名空间: AFramework.AMapper.Collection
// 依赖: System, System.Collections.Generic, System.Linq, System.Linq.Expressions
// 功能: List 映射器，处理 List<T> 之间的映射
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Collection
{
    /// <summary>
    /// List 映射器
    /// <para>处理 List&lt;T&gt; 之间的映射</para>
    /// <para>List mapper for List&lt;T&gt; mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>List&lt;T&gt; -> List&lt;T&gt;：相同元素类型</item>
    /// <item>List&lt;Source&gt; -> List&lt;Dest&gt;：不同元素类型</item>
    /// <item>IEnumerable&lt;T&gt; -> List&lt;T&gt;：集合到 List</item>
    /// <item>null -> null</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理 List 类型的映射</item>
    /// <item>性能优化：使用 ToList() 一次性分配容量</item>
    /// <item>递归映射：元素类型通过映射器递归映射</item>
    /// </list>
    /// 
    /// 映射逻辑：
    /// <code>
    /// // 使用 LINQ Select + ToList
    /// List&lt;Source&gt; source = ...;
    /// List&lt;Dest&gt; dest = source.Select(item => mapper.Map&lt;Dest&gt;(item)).ToList();
    /// </code>
    /// </remarks>
    public sealed class ListMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配 List 类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须是 List<T>
            if (!typePair.DestinationType.IsGenericType)
                return false;

            var destGenericDef = typePair.DestinationType.GetGenericTypeDefinition();
            if (destGenericDef != typeof(List<>))
                return false;

            // 源类型必须是 List<T> 或可枚举集合
            if (typePair.SourceType.IsGenericType)
            {
                var sourceGenericDef = typePair.SourceType.GetGenericTypeDefinition();
                return sourceGenericDef == typeof(List<>) ||
                       sourceGenericDef == typeof(IEnumerable<>) ||
                       sourceGenericDef == typeof(ICollection<>) ||
                       sourceGenericDef == typeof(IList<>);
            }

            return false;
        }

        /// <summary>
        /// 生成映射表达式
        /// </summary>
        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            var sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            var destElementType = typePair.DestinationType.GetGenericArguments()[0];

            // 处理 null 情况
            var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
            var nullValue = Expression.Constant(null, typePair.DestinationType);

            // 创建映射表达式
            var mappedExpression = CreateListMappingExpression(
                configuration,
                sourceExpression,
                sourceElementType,
                destElementType,
                contextExpression);

            // source == null ? null : source.Select(...).ToList()
            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        /// <summary>
        /// 获取关联的类型对
        /// </summary>
        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            var sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            var destElementType = typePair.DestinationType.GetGenericArguments()[0];

            // 返回元素类型对，用于递归映射
            return new[] { new TypePair(sourceElementType, destElementType) };
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建 List 映射表达式
        /// </summary>
        private Expression CreateListMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceElementType,
            Type destElementType,
            Expression contextExpression)
        {
            // 如果元素类型相同，使用构造函数复制
            if (sourceElementType == destElementType)
            {
                // new List<T>(source)
                var listConstructor = typeof(List<>).MakeGenericType(sourceElementType)
                    .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(sourceElementType) });
                return Expression.New(listConstructor, sourceExpression);
            }

            // 使用 LINQ Select 映射元素
            // source.Select(item => mapper.Map<TDest>(item)).ToList()
            var itemParameter = Expression.Parameter(sourceElementType, "item");
            
            // 获取 Map 方法
            var mapMethod = typeof(IAMapper).GetMethod(
                nameof(IAMapper.Map),
                new[] { typeof(object) })
                .MakeGenericMethod(destElementType);

            // 获取 Mapper 实例
            var mapperProperty = typeof(MappingContext).GetProperty(nameof(MappingContext.Mapper));
            var mapperExpression = Expression.Property(contextExpression, mapperProperty);

            // mapper.Map<TDest>(item)
            var mapCall = Expression.Call(
                mapperExpression,
                mapMethod,
                Expression.Convert(itemParameter, typeof(object)));

            // 创建 lambda：item => mapper.Map<TDest>(item)
            var mapLambda = Expression.Lambda(mapCall, itemParameter);

            // 调用 Enumerable.Select
            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == nameof(Enumerable.Select) && m.GetParameters().Length == 2)
                .MakeGenericMethod(sourceElementType, destElementType);

            var selectCall = Expression.Call(selectMethod, sourceExpression, mapLambda);

            // 调用 ToList()
            var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))
                .MakeGenericMethod(destElementType);

            return Expression.Call(toListMethod, selectCall);
        }

        #endregion
    }
}
