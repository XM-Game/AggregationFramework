// ==========================================================
// 文件名：ExpressionBuilder.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Linq.Expressions, System.Reflection
// 功能: 表达式构建工具，提供构建映射表达式的辅助方法
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 表达式构建工具
    /// <para>提供构建映射表达式树的辅助方法</para>
    /// <para>Expression builder providing helper methods for building mapping expression trees</para>
    /// </summary>
    /// <remarks>
    /// ExpressionBuilder 封装了常用的表达式构建操作，包括：
    /// <list type="bullet">
    /// <item>成员访问表达式</item>
    /// <item>类型转换表达式</item>
    /// <item>空值检查表达式</item>
    /// <item>条件表达式</item>
    /// <item>集合映射表达式</item>
    /// </list>
    /// </remarks>
    public static class ExpressionBuilder
    {
        #region 成员访问 / Member Access

        /// <summary>
        /// 构建成员访问表达式
        /// <para>Build member access expression</para>
        /// </summary>
        /// <param name="instance">实例表达式 / Instance expression</param>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>成员访问表达式 / Member access expression</returns>
        public static Expression BuildMemberAccess(Expression instance, MemberInfo member)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member switch
            {
                PropertyInfo prop => Expression.Property(instance, prop),
                FieldInfo field => Expression.Field(instance, field),
                MethodInfo method when method.GetParameters().Length == 0 => Expression.Call(instance, method),
                _ => throw new ArgumentException(
                    $"不支持的成员类型 / Unsupported member type: {member.MemberType}", nameof(member))
            };
        }

        /// <summary>
        /// 构建成员链访问表达式
        /// <para>Build member chain access expression</para>
        /// </summary>
        /// <param name="instance">实例表达式 / Instance expression</param>
        /// <param name="members">成员链 / Member chain</param>
        /// <returns>成员链访问表达式 / Member chain access expression</returns>
        public static Expression BuildMemberChainAccess(Expression instance, IEnumerable<MemberInfo> members)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            var current = instance;
            foreach (var member in members)
            {
                current = BuildMemberAccess(current, member);
            }
            return current;
        }

        /// <summary>
        /// 构建安全的成员链访问表达式（带空值检查）
        /// <para>Build safe member chain access expression with null checks</para>
        /// </summary>
        /// <param name="instance">实例表达式 / Instance expression</param>
        /// <param name="members">成员链 / Member chain</param>
        /// <returns>安全的成员链访问表达式 / Safe member chain access expression</returns>
        public static Expression BuildSafeMemberChainAccess(Expression instance, IEnumerable<MemberInfo> members)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            var memberList = members.ToList();
            if (memberList.Count == 0)
                return instance;

            var current = instance;
            Expression result = null;

            for (int i = 0; i < memberList.Count; i++)
            {
                var member = memberList[i];
                var memberAccess = BuildMemberAccess(current, member);

                // 如果不是最后一个成员且是引用类型，添加空值检查
                if (i < memberList.Count - 1 && !current.Type.IsValueType)
                {
                    var nullCheck = Expression.Equal(current, Expression.Constant(null, current.Type));
                    var defaultValue = Expression.Default(GetMemberType(memberList[memberList.Count - 1]));

                    if (result == null)
                    {
                        result = Expression.Condition(nullCheck, defaultValue, memberAccess);
                    }
                    else
                    {
                        result = Expression.Condition(nullCheck, defaultValue, result);
                    }
                }

                current = memberAccess;
            }

            return result ?? current;
        }

        /// <summary>
        /// 构建成员赋值表达式
        /// <para>Build member assignment expression</para>
        /// </summary>
        /// <param name="instance">实例表达式 / Instance expression</param>
        /// <param name="member">成员信息 / Member info</param>
        /// <param name="value">值表达式 / Value expression</param>
        /// <returns>赋值表达式 / Assignment expression</returns>
        public static Expression BuildMemberAssignment(Expression instance, MemberInfo member, Expression value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var memberAccess = BuildMemberAccess(instance, member);
            var targetType = GetMemberType(member);

            // 类型转换
            if (value.Type != targetType)
            {
                value = BuildTypeConversion(value, targetType);
            }

            return Expression.Assign(memberAccess, value);
        }

        #endregion

        #region 类型转换 / Type Conversion

        /// <summary>
        /// 构建类型转换表达式
        /// <para>Build type conversion expression</para>
        /// </summary>
        /// <param name="source">源表达式 / Source expression</param>
        /// <param name="targetType">目标类型 / Target type</param>
        /// <returns>转换表达式 / Conversion expression</returns>
        public static Expression BuildTypeConversion(Expression source, Type targetType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            var sourceType = source.Type;

            // 相同类型，无需转换
            if (sourceType == targetType)
                return source;

            // 可直接赋值
            if (targetType.IsAssignableFrom(sourceType))
                return source;

            // 处理可空类型
            var sourceUnderlyingType = System.Nullable.GetUnderlyingType(sourceType);
            var targetUnderlyingType = System.Nullable.GetUnderlyingType(targetType);

            // Nullable<T> -> T
            if (sourceUnderlyingType != null && targetUnderlyingType == null)
            {
                var valueProperty = sourceType.GetProperty("Value");
                source = Expression.Property(source, valueProperty);
                sourceType = sourceUnderlyingType;
            }

            // T -> Nullable<T>
            if (sourceUnderlyingType == null && targetUnderlyingType != null)
            {
                if (sourceType == targetUnderlyingType)
                {
                    return Expression.Convert(source, targetType);
                }
            }

            // 尝试隐式转换
            var implicitMethod = FindConversionMethod(sourceType, targetType, "op_Implicit");
            if (implicitMethod != null)
            {
                return Expression.Call(implicitMethod, source);
            }

            // 尝试显式转换
            var explicitMethod = FindConversionMethod(sourceType, targetType, "op_Explicit");
            if (explicitMethod != null)
            {
                return Expression.Call(explicitMethod, source);
            }

            // 使用 Convert
            return Expression.Convert(source, targetType);
        }

        /// <summary>
        /// 构建安全的类型转换表达式（带空值检查）
        /// <para>Build safe type conversion expression with null check</para>
        /// </summary>
        /// <param name="source">源表达式 / Source expression</param>
        /// <param name="targetType">目标类型 / Target type</param>
        /// <returns>安全的转换表达式 / Safe conversion expression</returns>
        public static Expression BuildSafeTypeConversion(Expression source, Type targetType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            var sourceType = source.Type;

            // 值类型不需要空值检查
            if (sourceType.IsValueType && System.Nullable.GetUnderlyingType(sourceType) == null)
            {
                return BuildTypeConversion(source, targetType);
            }

            // 引用类型或可空值类型，添加空值检查
            var nullCheck = Expression.Equal(source, Expression.Constant(null, sourceType));
            var defaultValue = Expression.Default(targetType);
            var conversion = BuildTypeConversion(source, targetType);

            return Expression.Condition(nullCheck, defaultValue, conversion);
        }

        #endregion

        #region 空值处理 / Null Handling

        /// <summary>
        /// 构建空值检查表达式
        /// <para>Build null check expression</para>
        /// </summary>
        /// <param name="expression">要检查的表达式 / Expression to check</param>
        /// <returns>空值检查表达式 / Null check expression</returns>
        public static Expression BuildNullCheck(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return Expression.Equal(expression, Expression.Constant(null, expression.Type));
        }

        /// <summary>
        /// 构建空值合并表达式
        /// <para>Build null coalesce expression</para>
        /// </summary>
        /// <param name="expression">主表达式 / Primary expression</param>
        /// <param name="fallback">后备表达式 / Fallback expression</param>
        /// <returns>空值合并表达式 / Null coalesce expression</returns>
        public static Expression BuildNullCoalesce(Expression expression, Expression fallback)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            // 确保类型兼容
            if (fallback.Type != expression.Type)
            {
                fallback = BuildTypeConversion(fallback, expression.Type);
            }

            return Expression.Coalesce(expression, fallback);
        }

        /// <summary>
        /// 构建空值替换表达式
        /// <para>Build null substitute expression</para>
        /// </summary>
        /// <param name="expression">主表达式 / Primary expression</param>
        /// <param name="substituteValue">替换值 / Substitute value</param>
        /// <returns>空值替换表达式 / Null substitute expression</returns>
        public static Expression BuildNullSubstitute(Expression expression, object substituteValue)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var substituteExpr = Expression.Constant(substituteValue, expression.Type);
            return BuildNullCoalesce(expression, substituteExpr);
        }

        #endregion

        #region 条件表达式 / Conditional Expressions

        /// <summary>
        /// 构建条件映射表达式
        /// <para>Build conditional mapping expression</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        /// <param name="ifTrue">条件为真时的表达式 / Expression when true</param>
        /// <param name="ifFalse">条件为假时的表达式 / Expression when false</param>
        /// <returns>条件表达式 / Conditional expression</returns>
        public static Expression BuildConditional(Expression condition, Expression ifTrue, Expression ifFalse)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (ifTrue == null)
                throw new ArgumentNullException(nameof(ifTrue));
            if (ifFalse == null)
                throw new ArgumentNullException(nameof(ifFalse));

            // 确保类型兼容
            if (ifFalse.Type != ifTrue.Type)
            {
                ifFalse = BuildTypeConversion(ifFalse, ifTrue.Type);
            }

            return Expression.Condition(condition, ifTrue, ifFalse);
        }

        /// <summary>
        /// 构建条件执行表达式（仅在条件为真时执行）
        /// <para>Build conditional execution expression (execute only when true)</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        /// <param name="action">动作表达式 / Action expression</param>
        /// <returns>条件执行表达式 / Conditional execution expression</returns>
        public static Expression BuildConditionalExecution(Expression condition, Expression action)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return Expression.IfThen(condition, action);
        }

        #endregion

        #region 方法调用 / Method Invocation

        /// <summary>
        /// 构建实例方法调用表达式
        /// <para>Build instance method call expression</para>
        /// </summary>
        /// <param name="instance">实例表达式 / Instance expression</param>
        /// <param name="methodName">方法名 / Method name</param>
        /// <param name="arguments">参数 / Arguments</param>
        /// <returns>方法调用表达式 / Method call expression</returns>
        public static Expression BuildMethodCall(Expression instance, string methodName, params Expression[] arguments)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));

            var argTypes = arguments?.Select(a => a.Type).ToArray() ?? Type.EmptyTypes;
            var method = instance.Type.GetMethod(methodName, argTypes);

            if (method == null)
            {
                throw new InvalidOperationException(
                    $"未找到方法 / Method not found: {instance.Type.Name}.{methodName}");
            }

            return Expression.Call(instance, method, arguments ?? Array.Empty<Expression>());
        }

        /// <summary>
        /// 构建静态方法调用表达式
        /// <para>Build static method call expression</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <param name="methodName">方法名 / Method name</param>
        /// <param name="arguments">参数 / Arguments</param>
        /// <returns>方法调用表达式 / Method call expression</returns>
        public static Expression BuildStaticMethodCall(Type type, string methodName, params Expression[] arguments)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));

            var argTypes = arguments?.Select(a => a.Type).ToArray() ?? Type.EmptyTypes;
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, argTypes, null);

            if (method == null)
            {
                throw new InvalidOperationException(
                    $"未找到静态方法 / Static method not found: {type.Name}.{methodName}");
            }

            return Expression.Call(method, arguments ?? Array.Empty<Expression>());
        }

        #endregion

        #region 对象创建 / Object Creation

        /// <summary>
        /// 构建对象创建表达式
        /// <para>Build object creation expression</para>
        /// </summary>
        /// <param name="type">对象类型 / Object type</param>
        /// <returns>创建表达式 / Creation expression</returns>
        public static Expression BuildNew(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                return Expression.New(ctor);
            }

            // 值类型可以使用 default
            if (type.IsValueType)
            {
                return Expression.Default(type);
            }

            throw new InvalidOperationException(
                $"类型没有无参构造函数 / Type has no parameterless constructor: {type.Name}");
        }

        /// <summary>
        /// 构建带参数的对象创建表达式
        /// <para>Build object creation expression with arguments</para>
        /// </summary>
        /// <param name="constructor">构造函数 / Constructor</param>
        /// <param name="arguments">参数 / Arguments</param>
        /// <returns>创建表达式 / Creation expression</returns>
        public static Expression BuildNew(ConstructorInfo constructor, params Expression[] arguments)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            return Expression.New(constructor, arguments ?? Array.Empty<Expression>());
        }

        /// <summary>
        /// 构建数组创建表达式
        /// <para>Build array creation expression</para>
        /// </summary>
        /// <param name="elementType">元素类型 / Element type</param>
        /// <param name="length">长度表达式 / Length expression</param>
        /// <returns>数组创建表达式 / Array creation expression</returns>
        public static Expression BuildNewArray(Type elementType, Expression length)
        {
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));
            if (length == null)
                throw new ArgumentNullException(nameof(length));

            return Expression.NewArrayBounds(elementType, length);
        }

        #endregion

        #region Lambda 构建 / Lambda Building

        /// <summary>
        /// 构建 Lambda 表达式
        /// <para>Build lambda expression</para>
        /// </summary>
        /// <typeparam name="TDelegate">委托类型 / Delegate type</typeparam>
        /// <param name="body">表达式体 / Expression body</param>
        /// <param name="parameters">参数 / Parameters</param>
        /// <returns>Lambda 表达式 / Lambda expression</returns>
        public static Expression<TDelegate> BuildLambda<TDelegate>(
            Expression body, 
            params ParameterExpression[] parameters)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            return Expression.Lambda<TDelegate>(body, parameters);
        }

        /// <summary>
        /// 构建映射函数表达式
        /// <para>Build mapping function expression</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="body">表达式体 / Expression body</param>
        /// <param name="sourceParam">源参数 / Source parameter</param>
        /// <param name="destParam">目标参数 / Destination parameter</param>
        /// <param name="contextParam">上下文参数 / Context parameter</param>
        /// <returns>映射函数表达式 / Mapping function expression</returns>
        public static Expression<Func<TSource, TDestination, ResolutionContext, TDestination>> BuildMapFunc<TSource, TDestination>(
            Expression body,
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            ParameterExpression contextParam)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            return Expression.Lambda<Func<TSource, TDestination, ResolutionContext, TDestination>>(
                body, sourceParam, destParam, contextParam);
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取成员类型
        /// <para>Get member type</para>
        /// </summary>
        /// <param name="member">成员信息 / Member info</param>
        /// <returns>成员类型 / Member type</returns>
        public static Type GetMemberType(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                MethodInfo method => method.ReturnType,
                _ => throw new ArgumentException(
                    $"不支持的成员类型 / Unsupported member type: {member.MemberType}", nameof(member))
            };
        }

        /// <summary>
        /// 查找转换方法
        /// <para>Find conversion method</para>
        /// </summary>
        private static MethodInfo FindConversionMethod(Type sourceType, Type targetType, string methodName)
        {
            // 在源类型中查找
            var method = sourceType.GetMethod(methodName, 
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { sourceType }, null);

            if (method != null && method.ReturnType == targetType)
                return method;

            // 在目标类型中查找
            method = targetType.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { sourceType }, null);

            if (method != null && method.ReturnType == targetType)
                return method;

            return null;
        }

        /// <summary>
        /// 创建参数表达式
        /// <para>Create parameter expression</para>
        /// </summary>
        /// <typeparam name="T">参数类型 / Parameter type</typeparam>
        /// <param name="name">参数名 / Parameter name</param>
        /// <returns>参数表达式 / Parameter expression</returns>
        public static ParameterExpression CreateParameter<T>(string name = null)
        {
            return Expression.Parameter(typeof(T), name ?? typeof(T).Name.ToLowerInvariant());
        }

        /// <summary>
        /// 创建变量表达式
        /// <para>Create variable expression</para>
        /// </summary>
        /// <typeparam name="T">变量类型 / Variable type</typeparam>
        /// <param name="name">变量名 / Variable name</param>
        /// <returns>变量表达式 / Variable expression</returns>
        public static ParameterExpression CreateVariable<T>(string name = null)
        {
            return Expression.Variable(typeof(T), name ?? typeof(T).Name.ToLowerInvariant());
        }

        #endregion
    }
}
