// ==========================================================
// 文件名：IObjectMapper.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions
// 功能: 定义对象映射器接口，用于内置和自定义类型映射器
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 对象映射器接口
    /// <para>定义内置和自定义类型映射器的契约</para>
    /// <para>Object mapper interface for built-in and custom type mappers</para>
    /// </summary>
    /// <remarks>
    /// AMapper 使用责任链模式处理类型映射。
    /// 每个 IObjectMapper 实现负责处理特定类型的映射。
    /// 
    /// 内置映射器包括：
    /// <list type="bullet">
    /// <item>AssignableMapper：可赋值类型映射</item>
    /// <item>CollectionMapper：集合类型映射</item>
    /// <item>EnumMapper：枚举类型映射</item>
    /// <item>NullableMapper：可空类型映射</item>
    /// <item>UnityTypeMapper：Unity 类型映射</item>
    /// </list>
    /// 
    /// 自定义映射器示例：
    /// <code>
    /// public class CustomMapper : IObjectMapper
    /// {
    ///     public bool IsMatch(TypePair typePair)
    ///     {
    ///         return typePair.SourceType == typeof(MySource) 
    ///             &amp;&amp; typePair.DestinationType == typeof(MyDest);
    ///     }
    ///     
    ///     public Expression MapExpression(IMapperConfiguration config, TypePair typePair, 
    ///         Expression sourceExpression, Expression destExpression, Expression contextExpression)
    ///     {
    ///         // 返回映射表达式
    ///         return Expression.New(typeof(MyDest));
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public interface IObjectMapper
    {
        /// <summary>
        /// 判断此映射器是否匹配指定的类型对
        /// <para>Determine if this mapper matches the specified type pair</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        bool IsMatch(TypePair typePair);

        /// <summary>
        /// 生成映射表达式
        /// <para>Generate the mapping expression</para>
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <param name="sourceExpression">源对象表达式 / Source object expression</param>
        /// <param name="destinationExpression">目标对象表达式（可能为 null） / Destination object expression (may be null)</param>
        /// <param name="contextExpression">上下文表达式 / Context expression</param>
        /// <returns>映射表达式 / Mapping expression</returns>
        /// <remarks>
        /// 此方法应返回一个表达式，该表达式在执行时产生目标类型的实例。
        /// 表达式将被编译为委托并缓存以提高性能。
        /// </remarks>
        Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression);

        /// <summary>
        /// 获取关联的类型对（用于递归映射）
        /// <para>Get associated type pairs for recursive mapping</para>
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>关联的类型对数组 / Array of associated type pairs</returns>
        /// <remarks>
        /// 此方法用于配置验证，返回此映射器可能递归映射的类型对。
        /// 例如，CollectionMapper 会返回元素类型的类型对。
        /// 默认实现返回空数组。
        /// </remarks>
        TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }
    }
}
