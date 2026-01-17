// ==========================================================
// 文件名：EnumToEnumMapper.cs
// 命名空间: AFramework.AMapper.Enum
// 依赖: System, System.Linq.Expressions
// 功能: 枚举到枚举映射器，处理枚举类型之间的转换
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper.Enum
{
    /// <summary>
    /// 枚举到枚举映射器
    /// <para>处理枚举类型之间的转换（按名称或按值）</para>
    /// <para>Enum to enum mapper for converting between enum types</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>相同枚举类型：Status -> Status</item>
    /// <item>不同枚举类型（按值）：Status1.Active(1) -> Status2.Enabled(1)</item>
    /// <item>不同枚举类型（按名称）：Status1.Active -> Status2.Active</item>
    /// <item>枚举与数值：Status.Active -> int(1)</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理枚举类型之间的转换</item>
    /// <item>按值转换：默认使用底层数值进行转换</item>
    /// <item>性能优化：使用表达式树编译，避免反射</item>
    /// </list>
    /// 
    /// 转换逻辑：
    /// <code>
    /// // 按值转换
    /// Status1 source = Status1.Active; // 值为 1
    /// Status2 dest = (Status2)(int)source; // 转换为 Status2 的值 1
    /// </code>
    /// </remarks>
    public sealed class EnumToEnumMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配枚举到枚举
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;

            // 源类型和目标类型都必须是枚举
            return sourceType.IsEnum && destType.IsEnum;
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
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;

            // 处理可空类型
            if (System.Nullable.GetUnderlyingType(typePair.SourceType) != null)
            {
                // Nullable<Enum> -> Nullable<Enum> 或 Nullable<Enum> -> Enum
                return CreateNullableEnumConversion(sourceExpression, typePair.SourceType, typePair.DestinationType);
            }

            // 非可空枚举转换
            // 步骤：Enum1 -> int -> Enum2
            var underlyingType = System.Enum.GetUnderlyingType(sourceType);
            var toIntExpression = Expression.Convert(sourceExpression, underlyingType);
            var toEnumExpression = Expression.Convert(toIntExpression, destType);

            // 如果目标类型是 Nullable<Enum>，需要再转换一次
            if (System.Nullable.GetUnderlyingType(typePair.DestinationType) != null)
            {
                return Expression.Convert(toEnumExpression, typePair.DestinationType);
            }

            return toEnumExpression;
        }

        /// <summary>
        /// 获取关联的类型对
        /// </summary>
        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建可空枚举转换表达式
        /// </summary>
        private Expression CreateNullableEnumConversion(Expression sourceExpression, Type sourceType, Type destType)
        {
            var sourceUnderlying = System.Nullable.GetUnderlyingType(sourceType);
            var destUnderlying = System.Nullable.GetUnderlyingType(destType) ?? destType;

            // 获取 Nullable<T>.HasValue 和 Nullable<T>.Value 属性
            var hasValueProperty = sourceType.GetProperty("HasValue");
            var valueProperty = sourceType.GetProperty("Value");

            // source.HasValue
            var hasValueExpression = Expression.Property(sourceExpression, hasValueProperty);

            // source.Value
            var valueExpression = Expression.Property(sourceExpression, valueProperty);

            // 转换枚举值
            var underlyingType = System.Enum.GetUnderlyingType(sourceUnderlying);
            var toIntExpression = Expression.Convert(valueExpression, underlyingType);
            var toEnumExpression = Expression.Convert(toIntExpression, destUnderlying);

            // 如果目标类型是 Nullable<Enum>，包装为可空类型
            Expression convertedValue = toEnumExpression;
            if (System.Nullable.GetUnderlyingType(destType) != null)
            {
                convertedValue = Expression.Convert(toEnumExpression, destType);
            }

            // default(TDestination)
            var defaultValue = Expression.Default(destType);

            // source.HasValue ? (TDestination)source.Value : default(TDestination)
            return Expression.Condition(hasValueExpression, convertedValue, defaultValue);
        }

        #endregion
    }
}
