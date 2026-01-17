// ==========================================================
// 文件名：EnumToStringMapper.cs
// 命名空间: AFramework.AMapper.Enum
// 依赖: System, System.Linq.Expressions
// 功能: 枚举到字符串映射器，将枚举值转换为字符串
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Enum
{
    /// <summary>
    /// 枚举到字符串映射器
    /// <para>将枚举值转换为字符串（枚举名称）</para>
    /// <para>Enum to string mapper that converts enum value to string</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>枚举 -> 字符串：Status.Active -> "Active"</item>
    /// <item>Nullable&lt;Enum&gt; -> 字符串：Status?.Active -> "Active"</item>
    /// <item>null -> null</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理枚举到字符串的转换</item>
    /// <item>简单直接：调用 ToString() 方法</item>
    /// <item>null 安全：可空枚举为 null 时返回 null</item>
    /// </list>
    /// 
    /// 转换逻辑：
    /// <code>
    /// // 调用 ToString()
    /// Status source = Status.Active;
    /// string dest = source.ToString(); // "Active"
    /// </code>
    /// </remarks>
    public sealed class EnumToStringMapper : IObjectMapper
    {
        private static readonly MethodInfo _toStringMethod = typeof(object).GetMethod(nameof(ToString));

        /// <summary>
        /// 判断是否匹配枚举到字符串
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须是 string
            if (typePair.DestinationType != typeof(string))
                return false;

            // 源类型必须是枚举或 Nullable<Enum>
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;
            return sourceType.IsEnum;
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
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType);

            // 处理可空枚举
            if (sourceType != null)
            {
                // Nullable<Enum> -> string
                return CreateNullableEnumToStringExpression(sourceExpression, typePair.SourceType);
            }

            // 非可空枚举直接调用 ToString()
            // 需要装箱为 object
            var boxedSource = Expression.Convert(sourceExpression, typeof(object));
            return Expression.Call(boxedSource, _toStringMethod);
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
        /// 创建可空枚举到字符串的转换表达式
        /// </summary>
        private Expression CreateNullableEnumToStringExpression(Expression sourceExpression, Type sourceType)
        {
            // 获取 Nullable<T>.HasValue 和 Nullable<T>.Value 属性
            var hasValueProperty = sourceType.GetProperty("HasValue");
            var valueProperty = sourceType.GetProperty("Value");

            // source.HasValue
            var hasValueExpression = Expression.Property(sourceExpression, hasValueProperty);

            // source.Value
            var valueExpression = Expression.Property(sourceExpression, valueProperty);

            // source.Value.ToString()
            var boxedValue = Expression.Convert(valueExpression, typeof(object));
            var toStringCall = Expression.Call(boxedValue, _toStringMethod);

            // null
            var nullValue = Expression.Constant(null, typeof(string));

            // source.HasValue ? source.Value.ToString() : null
            return Expression.Condition(hasValueExpression, toStringCall, nullValue);
        }

        #endregion
    }
}
