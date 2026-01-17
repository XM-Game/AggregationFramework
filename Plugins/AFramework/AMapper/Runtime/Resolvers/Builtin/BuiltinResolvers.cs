// ==========================================================
// 文件名：BuiltinResolvers.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 内置解析器集合，提供常用解析器的快速访问
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 内置解析器集合
    /// <para>提供常用解析器的快速访问和创建方法</para>
    /// <para>Built-in resolvers collection that provides quick access to common resolvers</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>外观模式：简化解析器的创建和使用</item>
    /// <item>工厂方法：提供统一的创建接口</item>
    /// <item>易用性：减少样板代码，提高开发效率</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 使用成员路径解析器
    /// var resolver = BuiltinResolvers.FromMember&lt;Order, OrderDto, string&gt;(s => s.Customer.Name);
    /// 
    /// // 使用常量解析器
    /// var resolver = BuiltinResolvers.FromConstant&lt;Order, OrderDto, string&gt;("Active");
    /// 
    /// // 使用委托解析器
    /// var resolver = BuiltinResolvers.FromFunc&lt;Order, OrderDto, decimal&gt;(s => s.Price * s.Quantity);
    /// </code>
    /// </remarks>
    public static class BuiltinResolvers
    {
        #region 成员路径解析器 / Member Path Resolver

        /// <summary>
        /// 创建成员路径解析器
        /// <para>Create member path resolver</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="sourceExpression">源成员表达式 / Source member expression</param>
        /// <returns>成员路径解析器 / Member path resolver</returns>
        public static MemberPathResolver<TSource, TDestination, TDestMember> FromMember<TSource, TDestination, TDestMember>(
            Expression<Func<TSource, TDestMember>> sourceExpression)
        {
            return new MemberPathResolver<TSource, TDestination, TDestMember>(sourceExpression);
        }

        /// <summary>
        /// 创建成员路径解析器（非泛型版本）
        /// <para>Create member path resolver (non-generic version)</para>
        /// </summary>
        /// <param name="sourceExpression">源成员表达式 / Source member expression</param>
        /// <returns>成员路径解析器 / Member path resolver</returns>
        public static MemberPathResolver FromMember(LambdaExpression sourceExpression)
        {
            return new MemberPathResolver(sourceExpression);
        }

        #endregion

        #region 表达式解析器 / Expression Resolver

        /// <summary>
        /// 创建表达式解析器（源对象参数）
        /// <para>Create expression resolver (source parameter)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="sourceExpression">源表达式 / Source expression</param>
        /// <returns>表达式解析器 / Expression resolver</returns>
        public static ExpressionResolver<TSource, TDestination, TDestMember> FromExpression<TSource, TDestination, TDestMember>(
            Expression<Func<TSource, TDestMember>> sourceExpression)
        {
            return new ExpressionResolver<TSource, TDestination, TDestMember>(sourceExpression);
        }

        /// <summary>
        /// 创建表达式解析器（源对象和目标对象参数）
        /// <para>Create expression resolver (source and destination parameters)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="sourceExpression">源表达式 / Source expression</param>
        /// <returns>表达式解析器 / Expression resolver</returns>
        public static ExpressionResolver<TSource, TDestination, TDestMember> FromExpression<TSource, TDestination, TDestMember>(
            Expression<Func<TSource, TDestination, TDestMember>> sourceExpression)
        {
            return new ExpressionResolver<TSource, TDestination, TDestMember>(sourceExpression);
        }

        #endregion

        #region 委托解析器 / Func Resolver

        /// <summary>
        /// 创建委托解析器（源对象参数）
        /// <para>Create func resolver (source parameter)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <returns>委托解析器 / Func resolver</returns>
        public static FuncResolver<TSource, TDestination, TDestMember> FromFunc<TSource, TDestination, TDestMember>(
            Func<TSource, TDestMember> func)
        {
            return new FuncResolver<TSource, TDestination, TDestMember>(func);
        }

        /// <summary>
        /// 创建委托解析器（源对象和目标对象参数）
        /// <para>Create func resolver (source and destination parameters)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <typeparam name="TResult">结果类型 / Result type</typeparam>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <returns>委托解析器 / Func resolver</returns>
        public static FuncResolver<TSource, TDestination, TDestMember, TResult> FromFunc<TSource, TDestination, TDestMember, TResult>(
            Func<TSource, TDestination, TResult> func)
            where TResult : TDestMember
        {
            return new FuncResolver<TSource, TDestination, TDestMember, TResult>(func);
        }

        /// <summary>
        /// 创建委托解析器（包含解析上下文参数）
        /// <para>Create func resolver (with resolution context parameter)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <returns>委托解析器 / Func resolver</returns>
        public static FuncResolverWithContext<TSource, TDestination, TDestMember> FromFuncWithContext<TSource, TDestination, TDestMember>(
            Func<TSource, TDestination, ResolutionContext, TDestMember> func)
        {
            return new FuncResolverWithContext<TSource, TDestination, TDestMember>(func);
        }

        #endregion

        #region 常量解析器 / Constant Resolver

        /// <summary>
        /// 创建常量解析器
        /// <para>Create constant resolver</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
        /// <param name="constantValue">常量值 / Constant value</param>
        /// <returns>常量解析器 / Constant resolver</returns>
        public static ConstantResolver<TSource, TDestination, TDestMember> FromConstant<TSource, TDestination, TDestMember>(
            TDestMember constantValue)
        {
            return new ConstantResolver<TSource, TDestination, TDestMember>(constantValue);
        }

        /// <summary>
        /// 创建常量解析器（非泛型版本）
        /// <para>Create constant resolver (non-generic version)</para>
        /// </summary>
        /// <param name="constantValue">常量值 / Constant value</param>
        /// <returns>常量解析器 / Constant resolver</returns>
        public static ConstantResolver FromConstant(object constantValue)
        {
            return new ConstantResolver(constantValue);
        }

        #endregion
    }
}
