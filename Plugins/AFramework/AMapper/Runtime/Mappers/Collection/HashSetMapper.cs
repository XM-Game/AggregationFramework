// ==========================================================
// 文件名：HashSetMapper.cs
// 命名空间: AFramework.AMapper.Collection
// 依赖: System, System.Collections.Generic, System.Linq, System.Linq.Expressions
// 功能: HashSet 映射器，处理 HashSet<T> 之间的映射
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AFramework.AMapper.Collection
{
    /// <summary>
    /// HashSet 映射器
    /// <para>处理 HashSet&lt;T&gt; 之间的映射</para>
    /// <para>HashSet mapper for HashSet&lt;T&gt; mapping</para>
    /// </summary>
    public sealed class HashSetMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            if (!typePair.DestinationType.IsGenericType)
                return false;

            var destGenericDef = typePair.DestinationType.GetGenericTypeDefinition();
            if (destGenericDef != typeof(HashSet<>) && destGenericDef != typeof(ISet<>))
                return false;

            if (typePair.SourceType.IsGenericType)
            {
                var sourceGenericDef = typePair.SourceType.GetGenericTypeDefinition();
                return sourceGenericDef == typeof(HashSet<>) ||
                       sourceGenericDef == typeof(ISet<>) ||
                       sourceGenericDef == typeof(IEnumerable<>);
            }

            return false;
        }

        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            var sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            var destElementType = typePair.DestinationType.GetGenericArguments()[0];

            var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
            var nullValue = Expression.Constant(null, typePair.DestinationType);

            var mappedExpression = CreateHashSetMappingExpression(
                configuration,
                sourceExpression,
                sourceElementType,
                destElementType,
                contextExpression);

            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            var sourceElementType = typePair.SourceType.GetGenericArguments()[0];
            var destElementType = typePair.DestinationType.GetGenericArguments()[0];
            return new[] { new TypePair(sourceElementType, destElementType) };
        }

        #region 私有方法 / Private Methods

        private Expression CreateHashSetMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceElementType,
            Type destElementType,
            Expression contextExpression)
        {
            if (sourceElementType == destElementType)
            {
                var hashSetConstructorSame = typeof(HashSet<>).MakeGenericType(sourceElementType)
                    .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(sourceElementType) });
                return Expression.New(hashSetConstructorSame, sourceExpression);
            }

            var itemParameter = Expression.Parameter(sourceElementType, "item");
            var mapMethod = typeof(IAMapper).GetMethod(nameof(IAMapper.Map), new[] { typeof(object) })
                .MakeGenericMethod(destElementType);
            var mapperProperty = typeof(MappingContext).GetProperty(nameof(MappingContext.Mapper));
            var mapperExpression = Expression.Property(contextExpression, mapperProperty);
            var mapCall = Expression.Call(mapperExpression, mapMethod, Expression.Convert(itemParameter, typeof(object)));
            var mapLambda = Expression.Lambda(mapCall, itemParameter);

            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == nameof(Enumerable.Select) && m.GetParameters().Length == 2)
                .MakeGenericMethod(sourceElementType, destElementType);
            var selectCall = Expression.Call(selectMethod, sourceExpression, mapLambda);

            var hashSetConstructorDiff = typeof(HashSet<>).MakeGenericType(destElementType)
                .GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(destElementType) });
            return Expression.New(hashSetConstructorDiff, selectCall);
        }

        #endregion
    }
}
