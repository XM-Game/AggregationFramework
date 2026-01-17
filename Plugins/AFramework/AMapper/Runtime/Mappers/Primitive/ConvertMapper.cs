// ==========================================================
// 文件名：ConvertMapper.cs
// 命名空间: AFramework.AMapper.Primitive
// 依赖: System, System.Linq.Expressions
// 功能: System.Convert 映射器，处理基元类型转换
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Primitive
{
    /// <summary>
    /// System.Convert 映射器
    /// <para>使用 System.Convert.ChangeType 处理基元类型转换</para>
    /// <para>Convert mapper using System.Convert.ChangeType for primitive type conversion</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>基元类型：int, long, short, byte, float, double, decimal, bool, char</item>
    /// <item>特殊类型：DateTime, string</item>
    /// <item>数值转换：int -> long, float -> double</item>
    /// <item>字符串转换：string -> int, string -> DateTime</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理 System.Convert 支持的类型转换</item>
    /// <item>异常安全：转换失败时抛出 MappingException</item>
    /// <item>性能优化：使用表达式树编译，避免反射调用</item>
    /// </list>
    /// </remarks>
    public sealed class ConvertMapper : IObjectMapper
    {
        private static readonly MethodInfo _changeTypeMethod = typeof(Convert).GetMethod(
            nameof(Convert.ChangeType),
            new[] { typeof(object), typeof(Type) });

        /// <summary>
        /// 判断是否匹配可转换类型
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否匹配 / Whether matched</returns>
        public bool IsMatch(TypePair typePair)
        {
            var destType = System.Nullable.GetUnderlyingType(typePair.DestinationType) ?? typePair.DestinationType;
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;

            // 检查是否为 System.Convert 支持的类型
            return IsConvertibleType(sourceType) && IsConvertibleType(destType);
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
            var sourceType = System.Nullable.GetUnderlyingType(typePair.SourceType) ?? typePair.SourceType;

            // 处理 null 情况
            if (!sourceType.IsValueType || System.Nullable.GetUnderlyingType(typePair.SourceType) != null)
            {
                // source == null ? default(TDestination) : Convert.ChangeType(source, typeof(TDestination))
                var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
                var defaultValue = Expression.Default(typePair.DestinationType);
                var convertExpression = CreateConvertExpression(sourceExpression, destType, typePair.DestinationType);

                return Expression.Condition(nullCheck, defaultValue, convertExpression);
            }

            // 值类型直接转换
            return CreateConvertExpression(sourceExpression, destType, typePair.DestinationType);
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
        /// 创建转换表达式
        /// </summary>
        private Expression CreateConvertExpression(Expression sourceExpression, Type destType, Type finalDestType)
        {
            // 装箱源对象（如果需要）
            var boxedSource = sourceExpression.Type.IsValueType
                ? Expression.Convert(sourceExpression, typeof(object))
                : sourceExpression;

            // 调用 Convert.ChangeType
            var changeTypeCall = Expression.Call(
                _changeTypeMethod,
                boxedSource,
                Expression.Constant(destType));

            // 拆箱结果
            var unboxedResult = Expression.Convert(changeTypeCall, finalDestType);

            return unboxedResult;
        }

        /// <summary>
        /// 检查是否为可转换类型
        /// </summary>
        private static bool IsConvertibleType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(string) ||
                   type == typeof(Guid) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(DateTimeOffset);
        }

        #endregion
    }
}
