// ==========================================================
// 文件名：DictionaryMapper.cs
// 命名空间: AFramework.AMapper.Collection
// 依赖: System, System.Collections.Generic, System.Linq, System.Linq.Expressions
// 功能: 字典映射器，处理 Dictionary<TKey, TValue> 之间的映射
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Collection
{
    /// <summary>
    /// 字典映射器
    /// <para>处理 Dictionary&lt;TKey, TValue&gt; 之间的映射</para>
    /// <para>Dictionary mapper for Dictionary&lt;TKey, TValue&gt; mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>Dictionary&lt;K, V&gt; -> Dictionary&lt;K, V&gt;：相同键值类型</item>
    /// <item>Dictionary&lt;K1, V1&gt; -> Dictionary&lt;K2, V2&gt;：不同键值类型</item>
    /// <item>IDictionary&lt;K, V&gt; -> Dictionary&lt;K, V&gt;</item>
    /// <item>null -> null</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理字典类型的映射</item>
    /// <item>性能优化：使用 ToDictionary 一次性构建</item>
    /// <item>递归映射：键和值类型通过映射器递归映射</item>
    /// </list>
    /// </remarks>
    public sealed class DictionaryMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            if (!typePair.DestinationType.IsGenericType)
                return false;

            var destGenericDef = typePair.DestinationType.GetGenericTypeDefinition();
            if (destGenericDef != typeof(Dictionary<,>) && destGenericDef != typeof(IDictionary<,>))
                return false;

            if (typePair.SourceType.IsGenericType)
            {
                var sourceGenericDef = typePair.SourceType.GetGenericTypeDefinition();
                return sourceGenericDef == typeof(Dictionary<,>) ||
                       sourceGenericDef == typeof(IDictionary<,>);
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
            var sourceArgs = typePair.SourceType.GetGenericArguments();
            var destArgs = typePair.DestinationType.GetGenericArguments();

            var sourceKeyType = sourceArgs[0];
            var sourceValueType = sourceArgs[1];
            var destKeyType = destArgs[0];
            var destValueType = destArgs[1];

            var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
            var nullValue = Expression.Constant(null, typePair.DestinationType);

            var mappedExpression = CreateDictionaryMappingExpression(
                configuration,
                sourceExpression,
                sourceKeyType,
                sourceValueType,
                destKeyType,
                destValueType,
                contextExpression);

            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            var sourceArgs = typePair.SourceType.GetGenericArguments();
            var destArgs = typePair.DestinationType.GetGenericArguments();

            return new[]
            {
                new TypePair(sourceArgs[0], destArgs[0]), // Key
                new TypePair(sourceArgs[1], destArgs[1])  // Value
            };
        }

        #region 私有方法 / Private Methods

        private Expression CreateDictionaryMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceKeyType,
            Type sourceValueType,
            Type destKeyType,
            Type destValueType,
            Expression contextExpression)
        {
            // 如果键值类型都相同，使用构造函数复制
            if (sourceKeyType == destKeyType && sourceValueType == destValueType)
            {
                var dictType = typeof(Dictionary<,>).MakeGenericType(sourceKeyType, sourceValueType);
                var constructor = dictType.GetConstructor(new[] { typeof(IDictionary<,>).MakeGenericType(sourceKeyType, sourceValueType) });
                return Expression.New(constructor, sourceExpression);
            }

            // 使用 ToDictionary 映射
            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(sourceKeyType, sourceValueType);
            var kvpParameter = Expression.Parameter(kvpType, "kvp");

            // 获取 Key 和 Value 属性
            var keyProperty = kvpType.GetProperty("Key");
            var valueProperty = kvpType.GetProperty("Value");
            var keyExpression = Expression.Property(kvpParameter, keyProperty);
            var valueExpression = Expression.Property(kvpParameter, valueProperty);

            // 映射 Key 和 Value
            var mappedKey = CreateMappingExpression(configuration, keyExpression, sourceKeyType, destKeyType, contextExpression);
            var mappedValue = CreateMappingExpression(configuration, valueExpression, sourceValueType, destValueType, contextExpression);

            // 创建 lambda
            var keyLambda = Expression.Lambda(mappedKey, kvpParameter);
            var valueLambda = Expression.Lambda(mappedValue, kvpParameter);

            // 调用 ToDictionary
            var toDictionaryMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == nameof(Enumerable.ToDictionary) && m.GetParameters().Length == 3)
                .MakeGenericMethod(kvpType, destKeyType, destValueType);

            return Expression.Call(toDictionaryMethod, sourceExpression, keyLambda, valueLambda);
        }

        private Expression CreateMappingExpression(
            IMapperConfiguration configuration,
            Expression sourceExpression,
            Type sourceType,
            Type destType,
            Expression contextExpression)
        {
            if (sourceType == destType)
                return sourceExpression;

            var mapMethod = typeof(IAMapper).GetMethod(nameof(IAMapper.Map), new[] { typeof(object) })
                .MakeGenericMethod(destType);
            var mapperProperty = typeof(MappingContext).GetProperty(nameof(MappingContext.Mapper));
            var mapperExpression = Expression.Property(contextExpression, mapperProperty);

            return Expression.Call(mapperExpression, mapMethod, Expression.Convert(sourceExpression, typeof(object)));
        }

        #endregion
    }
}
