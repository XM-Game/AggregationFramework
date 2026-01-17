// ==========================================================
// 文件名：NullableSourceMapper.cs
// 命名空间: AFramework.AMapper.Nullable
// 依赖: System, System.Linq.Expressions
// 功能: 可空源类型映射器，处理 Nullable<T> -> T 的映射
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Nullable
{
    /// <summary>
    /// 可空源类型映射器
    /// <para>处理 Nullable&lt;T&gt; -> T 的映射</para>
    /// <para>Nullable source mapper for Nullable&lt;T&gt; -> T mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>int? -> int：提取值或使用默认值</item>
    /// <item>DateTime? -> DateTime：提取值或使用默认值</item>
    /// <item>Guid? -> Guid：提取值或使用默认值</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理可空类型到非可空类型的映射</item>
    /// <item>null 安全：源对象为 null 时返回目标类型的默认值</item>
    /// <item>性能优化：使用 Nullable&lt;T&gt;.Value 属性，避免装箱</item>
    /// </list>
    /// 
    /// 映射逻辑：
    /// <code>
    /// // source.HasValue ? source.Value : default(T)
    /// int? source = 123;
    /// int dest = source ?? 0;
    /// </code>
    /// </remarks>
    public sealed class NullableSourceMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配可空源类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            var sourceUnderlying = System.Nullable.GetUnderlyingType(typePair.SourceType);
            
            // 源类型必须是 Nullable<T>
            if (sourceUnderlying == null)
                return false;

            // 目标类型必须是非可空类型，且与源类型的基础类型兼容
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;
            return destType.IsAssignableFrom(sourceUnderlying);
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
            var sourceUnderlying = System.Nullable.GetUnderlyingType(typePair.SourceType);
            
            // 获取 Nullable<T>.HasValue 和 Nullable<T>.Value 属性
            var hasValueProperty = typePair.SourceType.GetProperty("HasValue");
            var valueProperty = typePair.SourceType.GetProperty("Value");

            // source.HasValue
            var hasValueExpression = Expression.Property(sourceExpression, hasValueProperty);

            // source.Value
            var valueExpression = Expression.Property(sourceExpression, valueProperty);

            // 如果需要类型转换
            Expression convertedValue = valueExpression;
            if (valueExpression.Type != typePair.DestinationType)
            {
                convertedValue = Expression.Convert(valueExpression, typePair.DestinationType);
            }

            // default(TDestination)
            var defaultValue = Expression.Default(typePair.DestinationType);

            // source.HasValue ? source.Value : default(TDestination)
            return Expression.Condition(hasValueExpression, convertedValue, defaultValue);
        }

        /// <summary>
        /// 获取关联的类型对
        /// </summary>
        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }
    }
}
