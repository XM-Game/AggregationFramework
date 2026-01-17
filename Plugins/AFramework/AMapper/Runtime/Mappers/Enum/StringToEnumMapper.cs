// ==========================================================
// 文件名：StringToEnumMapper.cs
// 命名空间: AFramework.AMapper.Enum
// 依赖: System, System.Linq.Expressions
// 功能: 字符串到枚举映射器，将字符串解析为枚举值
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Enum
{
    /// <summary>
    /// 字符串到枚举映射器
    /// <para>将字符串解析为枚举值（按名称或按值）</para>
    /// <para>String to enum mapper that parses string to enum value</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>按名称解析："Active" -> Status.Active</item>
    /// <item>按值解析："1" -> Status.Active</item>
    /// <item>忽略大小写："active" -> Status.Active</item>
    /// <item>null 或空字符串 -> default(Enum)</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理字符串到枚举的转换</item>
    /// <item>容错性：忽略大小写，支持数值字符串</item>
    /// <item>异常安全：解析失败时抛出 MappingException</item>
    /// </list>
    /// 
    /// 解析逻辑：
    /// <code>
    /// // 使用 Enum.Parse
    /// string source = "Active";
    /// Status dest = (Status)Enum.Parse(typeof(Status), source, true);
    /// </code>
    /// </remarks>
    public sealed class StringToEnumMapper : IObjectMapper
    {
        private static readonly MethodInfo _enumParseMethod = typeof(System.Enum).GetMethod(
            nameof(System.Enum.Parse),
            new[] { typeof(Type), typeof(string), typeof(bool) });

        /// <summary>
        /// 判断是否匹配字符串到枚举
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 源类型必须是 string
            if (typePair.SourceType != typeof(string))
                return false;

            // 目标类型必须是枚举或 Nullable<Enum>
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;
            return destType.IsEnum;
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
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;

            // 处理 null 或空字符串
            // source == null || source == "" ? default(TDestination) : Enum.Parse(typeof(TEnum), source, true)
            var nullOrEmptyCheck = Expression.OrElse(
                Expression.Equal(sourceExpression, Expression.Constant(null)),
                Expression.Equal(sourceExpression, Expression.Constant(string.Empty)));

            var defaultValue = Expression.Default(typePair.DestinationType);

            // Enum.Parse(typeof(TEnum), source, true)
            var parseCall = Expression.Call(
                _enumParseMethod,
                Expression.Constant(destType),
                sourceExpression,
                Expression.Constant(true)); // ignoreCase = true

            // 转换为目标枚举类型
            var convertedValue = Expression.Convert(parseCall, destType);

            // 如果目标类型是 Nullable<Enum>，需要再转换一次
            if (System.Nullable.GetUnderlyingType(typePair.DestinationType) != null)
            {
                convertedValue = Expression.Convert(convertedValue, typePair.DestinationType);
            }

            return Expression.Condition(nullOrEmptyCheck, defaultValue, convertedValue);
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
