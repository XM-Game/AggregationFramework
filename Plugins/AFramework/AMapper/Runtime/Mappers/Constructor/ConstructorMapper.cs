// ==========================================================
// 文件名：ConstructorMapper.cs
// 命名空间: AFramework.AMapper.Constructor
// 依赖: System, System.Linq, System.Linq.Expressions
// 功能: 构造函数映射器，使用构造函数参数映射创建目标对象
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Constructor
{
    /// <summary>
    /// 构造函数映射器
    /// <para>使用构造函数参数映射创建目标对象</para>
    /// <para>Constructor mapper that creates destination object using constructor parameter mapping</para>
    /// </summary>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>目标类型有参数化构造函数</item>
    /// <item>构造函数参数与源对象属性匹配</item>
    /// <item>不可变对象（Immutable Object）</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅处理构造函数参数映射</item>
    /// <item>约定优于配置：按名称匹配参数</item>
    /// <item>优先级：最低优先级，作为兜底映射器</item>
    /// </list>
    /// 
    /// 匹配规则：
    /// <list type="bullet">
    /// <item>构造函数参数名与源属性名匹配（忽略大小写）</item>
    /// <item>参数类型与源属性类型兼容</item>
    /// <item>选择参数最多的构造函数</item>
    /// </list>
    /// </remarks>
    public sealed class ConstructorMapper : IObjectMapper
    {
        public bool IsMatch(TypePair typePair)
        {
            // 目标类型必须有公共构造函数
            if (typePair.DestinationType.IsAbstract || typePair.DestinationType.IsInterface)
                return false;

            // 查找合适的构造函数
            var constructor = FindBestConstructor(typePair.SourceType, typePair.DestinationType);
            return constructor != null;
        }

        public Expression MapExpression(
            IMapperConfiguration configuration,
            TypePair typePair,
            Expression sourceExpression,
            Expression destinationExpression,
            Expression contextExpression)
        {
            var constructor = FindBestConstructor(typePair.SourceType, typePair.DestinationType);
            
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"未找到类型 {typePair.DestinationType.Name} 的合适构造函数");
            }

            // 创建构造函数参数表达式
            var parameters = constructor.GetParameters();
            var sourceProperties = typePair.SourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var argumentExpressions = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var sourceProperty = sourceProperties.FirstOrDefault(p =>
                    string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase) &&
                    parameter.ParameterType.IsAssignableFrom(p.PropertyType));

                if (sourceProperty != null)
                {
                    // 从源对象读取属性值
                    var propertyExpression = Expression.Property(sourceExpression, sourceProperty);
                    
                    // 如果类型不完全匹配，添加转换
                    if (propertyExpression.Type != parameter.ParameterType)
                    {
                        argumentExpressions[i] = Expression.Convert(propertyExpression, parameter.ParameterType);
                    }
                    else
                    {
                        argumentExpressions[i] = propertyExpression;
                    }
                }
                else
                {
                    // 参数没有对应的源属性，使用默认值
                    argumentExpressions[i] = Expression.Default(parameter.ParameterType);
                }
            }

            // 调用构造函数
            return Expression.New(constructor, argumentExpressions);
        }

        public TypePair[] GetAssociatedTypes(IMapperConfiguration configuration, TypePair typePair)
        {
            // 构造函数映射可能涉及参数类型的递归映射
            var constructor = FindBestConstructor(typePair.SourceType, typePair.DestinationType);
            if (constructor == null)
                return Array.Empty<TypePair>();

            var parameters = constructor.GetParameters();
            var sourceProperties = typePair.SourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var associatedTypes = new System.Collections.Generic.List<TypePair>();

            foreach (var parameter in parameters)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(p =>
                    string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

                if (sourceProperty != null && sourceProperty.PropertyType != parameter.ParameterType)
                {
                    associatedTypes.Add(new TypePair(sourceProperty.PropertyType, parameter.ParameterType));
                }
            }

            return associatedTypes.ToArray();
        }

        #region 私有方法 / Private Methods

        /// <summary>
        /// 查找最佳构造函数
        /// </summary>
        private static ConstructorInfo FindBestConstructor(Type sourceType, Type destType)
        {
            var constructors = destType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
                return null;

            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 选择参数最多且所有参数都能匹配的构造函数
            ConstructorInfo bestConstructor = null;
            int maxMatchedParams = 0;

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                int matchedParams = 0;

                foreach (var parameter in parameters)
                {
                    var sourceProperty = sourceProperties.FirstOrDefault(p =>
                        string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase) &&
                        parameter.ParameterType.IsAssignableFrom(p.PropertyType));

                    if (sourceProperty != null)
                    {
                        matchedParams++;
                    }
                }

                // 如果所有参数都能匹配，且匹配数量更多，选择此构造函数
                if (matchedParams == parameters.Length && matchedParams > maxMatchedParams)
                {
                    bestConstructor = constructor;
                    maxMatchedParams = matchedParams;
                }
            }

            // 如果没有找到完全匹配的，返回参数最少的构造函数（通常是默认构造函数）
            return bestConstructor ?? constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();
        }

        #endregion
    }
}
