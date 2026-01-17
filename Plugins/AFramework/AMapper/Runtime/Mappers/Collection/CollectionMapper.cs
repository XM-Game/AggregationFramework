// ==========================================================
// 文件名：CollectionMapper.cs
// 命名空间: AFramework.AMapper.Collection
// 依赖: System, System.Collections.Generic, System.Linq.Expressions
// 功能: 通用集合映射器，处理 IEnumerable<T> 之间的映射
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Collection
{
    /// <summary>
    /// 通用集合映射器
    /// <para>处理 IEnumerable&lt;T&gt; 之间的映射</para>
    /// <para>Generic collection mapper for IEnumerable&lt;T&gt; mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>IEnumerable&lt;T&gt; -> IEnumerable&lt;T&gt;</item>
    /// <item>ICollection&lt;T&gt; -> ICollection&lt;T&gt;</item>
    /// <item>IList&lt;T&gt; -> IList&lt;T&gt;</item>
    /// <item>元素类型转换：IEnumerable&lt;Source&gt; -> IEnumerable&lt;Dest&gt;</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理通用集合接口的映射</item>
    /// <item>延迟执行：使用 LINQ Select 实现延迟映射</item>
    /// <item>递归映射：元素类型通过映射器递归映射</item>
    /// </list>
    /// 
    /// 映射逻辑：
    /// <code>
    /// // 使用 LINQ Select 映射元素
    /// IEnumerable&lt;Source&gt; source = ...;
    /// IEnumerable&lt;Dest&gt; dest = source.Select(item => mapper.Map&lt;Dest&gt;(item));
    /// </code>
    /// </remarks>
    public sealed class CollectionMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配集合类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 检查是否为泛型集合接口
            if (!typePair.SourceType.IsGenericType || !typePair.DestinationType.IsGenericType)
                return false;

            var sourceGenericDef = typePair.SourceType.GetGenericTypeDefinition();
            var destGenericDef = typePair.DestinationType.GetGenericTypeDefinition();

            // 支持的集合接口
            var supportedInterfaces = new[]
            {
                typeof(IEnumerable<>),
                typeof(ICollection<>),
                typeof(IList<>),
                typeof(IReadOnlyCollection<>),
                typeof(IReadOnlyList<>)
            };

            return supportedInterfaces.Contains(sourceGenericDef) &&
                   supportedInterfaces.Contains(destGenericDef);
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
            var mappedExpression = CreateMappingExpression(
                configuration,
                sourceExpression,
                sourceElementType,
                destElementType,
                contextExpression);

            // source == null ? null : source.Select(...)
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
        /// 创建映射表达式
        /// </summary>
        private Expression CreateMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceElementType,
            Type destElementType,
            Expression contextExpression)
        {
            // 如果元素类型相同，直接返回
            if (sourceElementType == destElementType)
            {
                return sourceExpression;
            }

            // 使用 LINQ Select 映射元素
            // source.Select(item => mapper.Map<TDest>(item))
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

            return selectCall;
        }

        #endregion
    }
}
