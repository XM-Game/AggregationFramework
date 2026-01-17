// ==========================================================
// 文件名：AssignableMapper.cs
// 命名空间: AFramework.AMapper.Primitive
// 依赖: System, System.Linq.Expressions
// 功能: 可赋值类型映射器，处理直接赋值兼容的类型
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper.Primitive
{
    /// <summary>
    /// 可赋值类型映射器
    /// <para>处理源类型可直接赋值给目标类型的映射</para>
    /// <para>Assignable type mapper for types that can be directly assigned</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>相同类型：string -> string</item>
    /// <item>继承关系：Derived -> Base</item>
    /// <item>接口实现：List&lt;T&gt; -> IEnumerable&lt;T&gt;</item>
    /// <item>协变/逆变：IEnumerable&lt;Derived&gt; -> IEnumerable&lt;Base&gt;</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理可赋值类型，不涉及转换逻辑</item>
    /// <item>性能优先：直接赋值，无额外开销</item>
    /// <item>最高优先级：在映射器链中最先匹配</item>
    /// </list>
    /// </remarks>
    public sealed class AssignableMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配可赋值类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 检查目标类型是否可从源类型赋值
            // 包括：相同类型、继承关系、接口实现、协变/逆变
            return typePair.DestinationType.IsAssignableFrom(typePair.SourceType);
        }

        /// <summary>
        /// 生成映射表达式
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <param name="sourceExpression">源对象表达式 / Source object expression</param>
        /// <param name="destinationExpression">目标对象表达式 / Destination object expression</param>
        /// <param name="contextExpression">上下文表达式 / Context expression</param>
        /// <returns>映射表达式 / Mapping expression</returns>
        /// <remarks>
        /// 生成的表达式逻辑：
        /// <code>
        /// // 如果源类型是值类型或目标类型是值类型，直接返回源对象
        /// return source;
        /// 
        /// // 如果都是引用类型，需要处理 null 情况
        /// return source ?? default(TDestination);
        /// </code>
        /// </remarks>
        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            // 对于可赋值类型，直接返回源表达式
            // 如果需要类型转换（如 Derived -> Base），添加 Convert 表达式
            if (sourceExpression.Type != typePair.DestinationType)
            {
                return Expression.Convert(sourceExpression, typePair.DestinationType);
            }

            return sourceExpression;
        }

        /// <summary>
        /// 获取关联的类型对
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>关联的类型对数组 / Array of associated type pairs</returns>
        /// <remarks>
        /// 可赋值类型映射不涉及递归映射，返回空数组
        /// </remarks>
        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }
    }
}
