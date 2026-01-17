// ==========================================================
// 文件名：StringMapper.cs
// 命名空间: AFramework.AMapper.Primitive
// 依赖: System, System.Linq.Expressions
// 功能: 字符串映射器，将任意类型转换为字符串
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Primitive
{
    /// <summary>
    /// 字符串映射器
    /// <para>将任意类型转换为字符串（调用 ToString 方法）</para>
    /// <para>String mapper that converts any type to string using ToString method</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>任意类型 -> string</item>
    /// <item>值类型 -> string：int -> "123"</item>
    /// <item>引用类型 -> string：object -> "ObjectName"</item>
    /// <item>null -> null</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理到字符串的转换</item>
    /// <item>null 安全：源对象为 null 时返回 null</item>
    /// <item>性能优化：使用表达式树编译，避免反射调用</item>
    /// </list>
    /// </remarks>
    public sealed class StringMapper : IObjectMapper
    {
        private static readonly MethodInfo _toStringMethod = typeof(object).GetMethod(nameof(ToString));

        /// <summary>
        /// 判断是否匹配字符串类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须是 string
            return typePair.DestinationType == typeof(string);
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
            // 处理 null 情况
            if (!typePair.SourceType.IsValueType)
            {
                // source == null ? null : source.ToString()
                var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
                var nullValue = Expression.Constant(null, typeof(string));
                var toStringCall = CreateToStringExpression(sourceExpression);

                return Expression.Condition(nullCheck, nullValue, toStringCall);
            }

            // 值类型直接调用 ToString
            return CreateToStringExpression(sourceExpression);
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
        /// 创建 ToString 表达式
        /// </summary>
        private Expression CreateToStringExpression(Expression sourceExpression)
        {
            // 如果源类型是值类型，需要装箱
            if (sourceExpression.Type.IsValueType)
            {
                var boxedSource = Expression.Convert(sourceExpression, typeof(object));
                return Expression.Call(boxedSource, _toStringMethod);
            }

            // 引用类型直接调用
            return Expression.Call(sourceExpression, _toStringMethod);
        }

        #endregion
    }
}
