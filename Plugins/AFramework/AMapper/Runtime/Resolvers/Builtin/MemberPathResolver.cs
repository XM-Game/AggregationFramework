// ==========================================================
// 文件名：MemberPathResolver.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System, System.Linq.Expressions
// 功能: 成员路径解析器，通过表达式路径解析嵌套成员值
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 成员路径解析器
    /// <para>通过表达式路径解析嵌套成员值</para>
    /// <para>Member path resolver that resolves nested member values through expression path</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>嵌套属性访问：source.Address.City</item>
    /// <item>链式调用：source.GetManager().GetDepartment().Name</item>
    /// <item>复杂表达式：source.Items.FirstOrDefault()?.Name</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责表达式路径解析</item>
    /// <item>null 安全：自动处理路径中的 null 值</item>
    /// <item>性能优化：编译表达式为委托，避免反射</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 配置
    /// CreateMap&lt;Order, OrderDto&gt;()
    ///     .ForMember(d => d.CustomerCity, opt => opt.MapFrom(s => s.Customer.Address.City));
    /// 
    /// // 内部使用 MemberPathResolver 解析 s.Customer.Address.City
    /// </code>
    /// </remarks>
    public sealed class MemberPathResolver<TSource, TDestination, TDestMember> : 
        ValueResolverBase<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private readonly Func<TSource, TDestMember> _compiledExpression;
        private readonly LambdaExpression _sourceExpression;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建成员路径解析器
        /// </summary>
        /// <param name="sourceExpression">源成员表达式 / Source member expression</param>
        /// <exception cref="ArgumentNullException">当 sourceExpression 为 null 时抛出</exception>
        public MemberPathResolver(Expression<Func<TSource, TDestMember>> sourceExpression)
        {
            _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            _compiledExpression = sourceExpression.Compile();
        }

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        protected override TDestMember Resolve(
            TSource source,
            TDestination destination,
            TDestMember destMember,
            ResolutionContext context)
        {
            if (source == null)
            {
                return default(TDestMember);
            }

            try
            {
                // 执行编译后的表达式
                return _compiledExpression(source);
            }
            catch (NullReferenceException)
            {
                // 路径中存在 null 值，返回默认值
                return default(TDestMember);
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"解析成员路径时发生错误。表达式：{_sourceExpression}",
                    ex);
            }
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取源表达式
        /// <para>Get source expression</para>
        /// </summary>
        public LambdaExpression GetSourceExpression()
        {
            return _sourceExpression;
        }

        #endregion
    }

    /// <summary>
    /// 成员路径解析器（非泛型版本）
    /// <para>Member path resolver (non-generic version)</para>
    /// </summary>
    /// <remarks>
    /// 用于运行时动态创建解析器的场景。
    /// </remarks>
    public sealed class MemberPathResolver : IValueResolver
    {
        #region 私有字段 / Private Fields

        private readonly Delegate _compiledExpression;
        private readonly LambdaExpression _sourceExpression;
        private readonly Type _sourceType;
        private readonly Type _destMemberType;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建成员路径解析器
        /// </summary>
        /// <param name="sourceExpression">源成员表达式 / Source member expression</param>
        /// <exception cref="ArgumentNullException">当 sourceExpression 为 null 时抛出</exception>
        public MemberPathResolver(LambdaExpression sourceExpression)
        {
            _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            _compiledExpression = sourceExpression.Compile();
            _sourceType = sourceExpression.Parameters[0].Type;
            _destMemberType = sourceExpression.ReturnType;
        }

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        public object Resolve(
            object source,
            object destination,
            object destMember,
            ResolutionContext context)
        {
            if (source == null)
            {
                return _destMemberType.IsValueType ? Activator.CreateInstance(_destMemberType) : null;
            }

            if (!_sourceType.IsInstanceOfType(source))
            {
                throw new ArgumentException(
                    $"源对象类型不匹配。期望 {_sourceType.Name}，实际 {source.GetType().Name}",
                    nameof(source));
            }

            try
            {
                // 执行编译后的表达式
                return _compiledExpression.DynamicInvoke(source);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is NullReferenceException)
            {
                // 路径中存在 null 值，返回默认值
                return _destMemberType.IsValueType ? Activator.CreateInstance(_destMemberType) : null;
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"解析成员路径时发生错误。表达式：{_sourceExpression}",
                    ex is TargetInvocationException tie ? tie.InnerException : ex);
            }
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取源表达式
        /// <para>Get source expression</para>
        /// </summary>
        public LambdaExpression GetSourceExpression()
        {
            return _sourceExpression;
        }

        #endregion
    }
}
