// ==========================================================
// 文件名：ConversionOperatorMapper.cs
// 命名空间: AFramework.AMapper.Constructor
// 依赖: System, System.Linq, System.Linq.Expressions
// 功能: 转换运算符映射器，处理隐式/显式转换运算符
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Constructor
{
    /// <summary>
    /// 转换运算符映射器
    /// <para>处理类型的隐式/显式转换运算符（op_Implicit / op_Explicit）</para>
    /// <para>Conversion operator mapper for implicit/explicit conversion operators</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>隐式转换：public static implicit operator Dest(Source source)</item>
    /// <item>显式转换：public static explicit operator Dest(Source source)</item>
    /// <item>自定义类型转换</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理转换运算符</item>
    /// <item>优先级：隐式转换优先于显式转换</item>
    /// <item>性能优化：直接调用转换运算符方法</item>
    /// </list>
    /// </remarks>
    public sealed class ConversionOperatorMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            return FindConversionOperator(typePair.SourceType, typePair.DestinationType) != null;
        }

        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            var conversionMethod = FindConversionOperator(typePair.SourceType, typePair.DestinationType);
            
            if (conversionMethod == null)
            {
                throw new InvalidOperationException(
                    $"未找到从 {typePair.SourceType.Name} 到 {typePair.DestinationType.Name} 的转换运算符");
            }

            // 调用转换运算符
            return Expression.Call(conversionMethod, sourceExpression);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 查找转换运算符
        /// </summary>
        private static MethodInfo FindConversionOperator(Type sourceType, Type destType)
        {
            // 优先查找隐式转换运算符
            var implicitOperator = FindOperator(sourceType, destType, "op_Implicit") ??
                                   FindOperator(destType, destType, "op_Implicit");

            if (implicitOperator != null)
                return implicitOperator;

            // 查找显式转换运算符
            return FindOperator(sourceType, destType, "op_Explicit") ??
                   FindOperator(destType, destType, "op_Explicit");
        }

        /// <summary>
        /// 查找指定的运算符方法
        /// </summary>
        private static MethodInfo FindOperator(Type type, Type returnType, string operatorName)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == operatorName &&
                    m.ReturnType == returnType &&
                    m.GetParameters().Length == 1);
        }

        #endregion
    }
}
