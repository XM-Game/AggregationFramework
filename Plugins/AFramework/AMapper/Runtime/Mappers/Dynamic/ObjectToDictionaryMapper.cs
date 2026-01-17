// ==========================================================
// 文件名：ObjectToDictionaryMapper.cs
// 命名空间: AFramework.AMapper.Dynamic
// 依赖: System, System.Collections.Generic, System.Linq.Expressions
// 功能: 对象到字典映射器，将强类型对象转换为字典
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Dynamic
{
    /// <summary>
    /// 对象到字典映射器
    /// <para>将强类型对象转换为 Dictionary&lt;string, object&gt;</para>
    /// <para>Object to dictionary mapper that converts strongly typed object to Dictionary&lt;string, object&gt;</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>JSON 序列化：DTO -> Dictionary</item>
    /// <item>动态数据导出：对象 -> 配置字典</item>
    /// <item>强类型到弱类型转换</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理对象到字典的映射</item>
    /// <item>反射读取：读取所有公共可读属性</item>
    /// <item>类型擦除：所有值转换为 object</item>
    /// </list>
    /// </remarks>
    public sealed class ObjectToDictionaryMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须是 Dictionary<string, object> 或 IDictionary<string, object>
            if (!typePair.DestinationType.IsGenericType)
                return false;

            var destGenericDef = typePair.DestinationType.GetGenericTypeDefinition();
            if (destGenericDef != typeof(Dictionary<,>) && destGenericDef != typeof(IDictionary<,>))
                return false;

            var destArgs = typePair.DestinationType.GetGenericArguments();
            if (destArgs[0] != typeof(string))
                return false;

            // 源类型必须是类（非字典、非集合）
            return typePair.SourceType.IsClass &&
                   !typePair.SourceType.IsAbstract &&
                   typePair.SourceType != typeof(string);
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
            var mappedExpression = CreateObjectToDictionaryExpression(
                sourceExpression,
                typePair.SourceType,
                typePair.DestinationType);

            return Expression.Condition(nullCheck, nullValue, mappedExpression);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            return Array.Empty<TypePair>();
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建对象到字典的映射表达式
        /// </summary>
        private Expression CreateObjectToDictionaryExpression(
            Expression sourceExpression,
            Type sourceType,
            Type destType)
        {
            // 创建字典实例
            var dictVariable = Expression.Variable(destType, "dict");
            var dictConstructor = typeof(Dictionary<string, object>).GetConstructor(Type.EmptyTypes);
            var newDict = Expression.New(dictConstructor);
            var assignDict = Expression.Assign(dictVariable, Expression.Convert(newDict, destType));

            // 获取可读属性
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            // 创建属性添加表达式列表
            var expressions = new System.Collections.Generic.List<Expression> { assignDict };

            // 获取字典的 Add 方法
            var addMethod = typeof(Dictionary<string, object>).GetMethod("Add");

            foreach (var property in properties)
            {
                // 读取属性值
                var propertyExpression = Expression.Property(sourceExpression, property);
                
                // 装箱为 object
                Expression boxedValue;
                if (property.PropertyType.IsValueType)
                {
                    boxedValue = Expression.Convert(propertyExpression, typeof(object));
                }
                else
                {
                    boxedValue = propertyExpression;
                }

                // dict.Add(propertyName, value)
                var addCall = Expression.Call(
                    Expression.Convert(dictVariable, typeof(Dictionary<string, object>)),
                    addMethod,
                    Expression.Constant(property.Name),
                    boxedValue);

                expressions.Add(addCall);
            }

            // 返回字典
            expressions.Add(dictVariable);

            // 创建块表达式
            return Expression.Block(new[] { dictVariable }, expressions);
        }

        #endregion
    }
}
