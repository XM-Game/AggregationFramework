// ==========================================================
// 文件名：DictionaryToObjectMapper.cs
// 命名空间: AFramework.AMapper.Dynamic
// 依赖: System, System.Collections.Generic, System.Linq.Expressions
// 功能: 字典到对象映射器，将字典转换为强类型对象
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Dynamic
{
    /// <summary>
    /// 字典到对象映射器
    /// <para>将 Dictionary&lt;string, object&gt; 转换为强类型对象</para>
    /// <para>Dictionary to object mapper that converts Dictionary&lt;string, object&gt; to strongly typed object</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>JSON 反序列化：Dictionary -> DTO</item>
    /// <item>动态数据映射：配置字典 -> 配置对象</item>
    /// <item>弱类型到强类型转换</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理字典到对象的映射</item>
    /// <item>按名称匹配：字典键与对象属性名匹配</item>
    /// <item>类型安全：自动进行类型转换</item>
    /// </list>
    /// </remarks>
    public sealed class DictionaryToObjectMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            // 源类型必须是 Dictionary<string, object> 或 IDictionary<string, object>
            if (!typePair.SourceType.IsGenericType)
                return false;

            var sourceGenericDef = typePair.SourceType.GetGenericTypeDefinition();
            if (sourceGenericDef != typeof(Dictionary<,>) && sourceGenericDef != typeof(IDictionary<,>))
                return false;

            var sourceArgs = typePair.SourceType.GetGenericArguments();
            if (sourceArgs[0] != typeof(string))
                return false;

            // 目标类型必须是类（非字典、非集合）
            return typePair.DestinationType.IsClass &&
                   !typePair.DestinationType.IsAbstract &&
                   typePair.DestinationType != typeof(string);
        }

        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            // 处理 null 情况
            var nullCheck = Expression.Equal(sourceExpression, Expression.Constant(null));
            var nullValue = Expression.Constant(null, typePair.DestinationType);

            // 创建映射表达式
            var mappedExpression = CreateDictionaryToObjectExpression(
                sourceExpression,
                typePair.DestinationType,
                contextExpression);

            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            // 字典值可能需要递归映射
            var properties = typePair.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            var sourceValueType = typePair.SourceType.GetGenericArguments()[1];
            var associatedTypes = new System.Collections.Generic.List<TypePair>();

            foreach (var property in properties)
            {
                if (property.PropertyType != sourceValueType && !property.PropertyType.IsAssignableFrom(sourceValueType))
                {
                    associatedTypes.Add(new TypePair(sourceValueType, property.PropertyType));
                }
            }

            return associatedTypes.ToArray();
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建字典到对象的映射表达式
        /// </summary>
        private Expression CreateDictionaryToObjectExpression(
            Expression sourceExpression,
            Type destType,
            Expression contextExpression)
        {
            // 创建目标对象实例
            var destVariable = Expression.Variable(destType, "dest");
            var newExpression = Expression.New(destType);
            var assignDest = Expression.Assign(destVariable, newExpression);

            // 获取可写属性
            var properties = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            // 创建属性赋值表达式列表
            var expressions = new System.Collections.Generic.List<Expression> { assignDest };

            // 获取字典的 TryGetValue 方法
            var tryGetValueMethod = sourceExpression.Type.GetMethod("TryGetValue");
            var valueType = sourceExpression.Type.GetGenericArguments()[1];

            foreach (var property in properties)
            {
                // 创建临时变量存储字典值
                var valueVariable = Expression.Variable(valueType, $"value_{property.Name}");
                
                // dict.TryGetValue(propertyName, out value)
                var tryGetValueCall = Expression.Call(
                    sourceExpression,
                    tryGetValueMethod,
                    Expression.Constant(property.Name),
                    valueVariable);

                // 如果找到值，赋值给属性
                // if (dict.TryGetValue(propertyName, out value))
                // {
                //     dest.Property = (PropertyType)value;
                // }
                Expression assignProperty;
                if (property.PropertyType == valueType)
                {
                    assignProperty = Expression.Assign(
                        Expression.Property(destVariable, property),
                        valueVariable);
                }
                else
                {
                    // 需要类型转换
                    var convertedValue = Expression.Convert(valueVariable, property.PropertyType);
                    assignProperty = Expression.Assign(
                        Expression.Property(destVariable, property),
                        convertedValue);
                }

                var ifStatement = Expression.IfThen(tryGetValueCall, assignProperty);
                expressions.Add(Expression.Block(new[] { valueVariable }, ifStatement));
            }

            // 返回目标对象
            expressions.Add(destVariable);

            // 创建块表达式
            return Expression.Block(new[] { destVariable }, expressions);
        }

        #endregion
    }
}
