// ==========================================================
// 文件名：ArrayMapper.cs
// 命名空间: AFramework.AMapper.Collection
// 依赖: System, System.Linq, System.Linq.Expressions
// 功能: 数组映射器，处理数组之间的映射
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Collection
{
    /// <summary>
    /// 数组映射器
    /// <para>处理数组之间的映射</para>
    /// <para>Array mapper for array-to-array mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>T[] -> T[]：相同元素类型</item>
    /// <item>Source[] -> Dest[]：不同元素类型</item>
    /// <item>IEnumerable&lt;T&gt; -> T[]：集合到数组</item>
    /// <item>null -> null</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理数组类型的映射</item>
    /// <item>性能优化：使用 ToArray() 一次性分配内存</item>
    /// <item>递归映射：元素类型通过映射器递归映射</item>
    /// </list>
    /// 
    /// 映射逻辑：
    /// <code>
    /// // 使用 LINQ Select + ToArray
    /// Source[] source = ...;
    /// Dest[] dest = source.Select(item => mapper.Map&lt;Dest&gt;(item)).ToArray();
    /// </code>
    /// </remarks>
    public sealed class ArrayMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配数组类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须是数组
            if (!typePair.DestinationType.IsArray)
                return false;

            // 源类型必须是数组或可枚举集合
            return typePair.SourceType.IsArray ||
                   (typePair.SourceType.IsGenericType &&
                    typePair.SourceType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>));
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
            var destElementType = typePair.DestinationType.GetElementType();
            Type sourceElementType;

            // 获取源元素类型
            if (typePair.SourceType.IsArray)
            {
                sourceElementType = typePair.SourceType.GetElementType();
            }
            else
            {
                sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            }

            // 处理 null 情况
            var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
            var nullValue = Expression.Constant(null, typePair.DestinationType);

            // 创建映射表达式
            var mappedExpression = CreateArrayMappingExpression(
                configuration,
                sourceExpression,
                sourceElementType,
                destElementType,
                contextExpression);

            // source == null ? null : source.Select(...).ToArray()
            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        /// <summary>
        /// 获取关联的类型对
        /// </summary>
        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            var destElementType = typePair.DestinationType.GetElementType();
            Type sourceElementType;

            if (typePair.SourceType.IsArray)
            {
                sourceElementType = typePair.SourceType.GetElementType();
            }
            else
            {
                sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            }

            // 返回元素类型对，用于递归映射
            return new[] { new TypePair(sourceElementType, destElementType) };
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建数组映射表达式
        /// </summary>
        private Expression CreateArrayMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceElementType,
            Type destElementType,
            Expression contextExpression)
        {
            // 如果元素类型相同，直接返回（或复制数组）
            if (sourceElementType == destElementType)
            {
                // 使用 ToArray() 创建副本
                var toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))
                    .MakeGenericMethod(sourceElementType);
                return Expression.Call(toArrayMethod, sourceExpression);
            }

            // 使用 LINQ Select 映射元素
            // source.Select(item => mapper.Map<TDest>(item)).ToArray()
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

            // 调用 ToArray()
            var toArrayMethodFinal = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))
                .MakeGenericMethod(destElementType);

            return Expression.Call(toArrayMethodFinal, selectCall);
        }

        #endregion
    }
}
