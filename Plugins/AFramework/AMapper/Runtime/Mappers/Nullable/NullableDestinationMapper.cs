// ==========================================================
// 文件名：NullableDestinationMapper.cs
// 命名空间: AFramework.AMapper.Nullable
// 依赖: System, System.Linq.Expressions
// 功能: 可空目标类型映射器，处理 T -> Nullable<T> 的映射
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper.Nullable
{
    /// <summary>
    /// 可空目标类型映射器
    /// <para>处理 T -> Nullable&lt;T&gt; 的映射</para>
    /// <para>Nullable destination mapper for T -> Nullable&lt;T&gt; mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>int -> int?：包装为可空类型</item>
    /// <item>DateTime -> DateTime?：包装为可空类型</item>
    /// <item>Guid -> Guid?：包装为可空类型</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理非可空类型到可空类型的映射</item>
    /// <item>简单直接：直接包装，无需额外逻辑</item>
    /// <item>性能优化：使用隐式转换，避免装箱</item>
    /// </list>
    /// 
    /// 映射逻辑：
    /// <code>
    /// // 直接赋值，C# 会自动转换
    /// int source = 123;
    /// int? dest = source; // 隐式转换
    /// </code>
    /// </remarks>
    public sealed class NullableDestinationMapper : IObjectMapper
    {
        /// <summary>
        /// 判断是否匹配可空目标类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            var destUnderlying = System.Nullable.GetUnderlyingType(typePair.DestinationType);
            
            // 目标类型必须是 Nullable<T>
            if (destUnderlying == null)
                return false;

            // 源类型必须是非可空类型，且与目标类型的基础类型兼容
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;
            return destUnderlying.IsAssignableFrom(sourceType);
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
            // 对于 T -> Nullable<T>，直接转换即可
            // C# 会自动处理隐式转换
            if (sourceExpression.Type != typePair.DestinationType)
            {
                return Expression.Convert(sourceExpression, typePair.DestinationType);
            }

            return sourceExpression;
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
