// ==========================================================
// 文件名：ParseStringMapper.cs
// 命名空间: AFramework.AMapper.Primitive
// 依赖: System, System.Linq.Expressions
// 功能: Parse 方法映射器，将字符串解析为目标类型
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Primitive
{
    /// <summary>
    /// Parse 方法映射器
    /// <para>使用类型的 Parse 方法将字符串解析为目标类型</para>
    /// <para>Parse method mapper that parses string to target type using Parse method</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>string -> int：使用 int.Parse</item>
    /// <item>string -> DateTime：使用 DateTime.Parse</item>
    /// <item>string -> Guid：使用 Guid.Parse</item>
    /// <item>string -> TimeSpan：使用 TimeSpan.Parse</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理字符串到其他类型的解析</item>
    /// <item>约定优于配置：自动查找 Parse 方法</item>
    /// <item>异常安全：解析失败时抛出 MappingException</item>
    /// </list>
    /// 
    /// Parse 方法要求：
    /// <list type="bullet">
    /// <item>方法名：Parse</item>
    /// <item>参数：单个 string 参数</item>
    /// <item>返回类型：目标类型</item>
    /// <item>静态方法</item>
    /// </list>
    /// </remarks>
    public sealed class ParseStringMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配 Parse 方法
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 源类型必须是 string
            if (typePair.SourceType != typeof(string))
                return false;

            // 目标类型必须有 Parse(string) 静态方法
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;
            return HasParseMethod(destType);
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
            var parseMethod = GetParseMethod(destType);

            if (parseMethod == null)
            {
                throw new InvalidOperationException($"类型 {destType.Name} 没有 Parse(string) 方法");
            }

            // 处理 null 或空字符串
            // source == null || source == "" ? default(TDestination) : Type.Parse(source)
            var nullOrEmptyCheck = Expression.OrElse(
                Expression.Equal(sourceExpression, Expression.Constant(null)),
                Expression.Equal(sourceExpression, Expression.Constant(string.Empty)));

            var defaultValue = Expression.Default(typePair.DestinationType);
            var parseCall = Expression.Call(parseMethod, sourceExpression);

            // 如果目标类型是 Nullable<T>，需要转换
            Expression parseResult = parseCall;
            if (System.Nullable.GetUnderlyingType(typePair.DestinationType) != null)
            {
                parseResult = Expression.Convert(parseCall, typePair.DestinationType);
            }

            return Expression.Condition(nullOrEmptyCheck, defaultValue, parseResult);
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
        /// 检查类型是否有 Parse 方法
        /// </summary>
        private static bool HasParseMethod(Type type)
        {
            return GetParseMethod(type) != null;
        }

        /// <summary>
        /// 获取 Parse 方法
        /// </summary>
        private static MethodInfo GetParseMethod(Type type)
        {
            return type.GetMethod(
                "Parse",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);
        }

        #endregion
    }
}
