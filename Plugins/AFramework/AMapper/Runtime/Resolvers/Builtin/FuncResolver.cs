// ==========================================================
// 文件名：FuncResolver.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 委托解析器，通过委托函数计算目标成员值
// ==========================================================

using System;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 委托解析器
    /// <para>通过委托函数计算目标成员值</para>
    /// <para>Func resolver that calculates destination member value through delegate function</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>简单函数：source => source.FirstName + " " + source.LastName</item>
    /// <item>外部服务调用：source => _service.GetValue(source.Id)</item>
    /// <item>复杂业务逻辑：source => CalculateDiscount(source)</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责委托函数调用</item>
    /// <item>灵活性：支持任意委托签名</item>
    /// <item>简洁性：无需创建独立的解析器类</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 配置
    /// CreateMap&lt;Person, PersonDto&gt;()
    ///     .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName + " " + s.LastName));
    /// </code>
    /// </remarks>
    public sealed class FuncResolver<TSource, TDestination, TDestMember> :
        ValueResolverBase<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private readonly Func<TSource, TDestMember> _func;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建委托解析器（源对象参数）
        /// </summary>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <exception cref="ArgumentNullException">当 func 为 null 时抛出</exception>
        public FuncResolver(Func<TSource, TDestMember> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
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
            try
            {
                // 执行委托函数
                return _func(source);
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"执行委托函数时发生错误。函数：{_func.Method.Name}",
                    ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// 委托解析器（源对象和目标对象参数）
    /// <para>Func resolver with source and destination parameters</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    public sealed class FuncResolver<TSource, TDestination, TDestMember, TResult> :
        ValueResolverBase<TSource, TDestination, TDestMember>
        where TResult : TDestMember
    {
        #region 私有字段 / Private Fields

        private readonly Func<TSource, TDestination, TResult> _func;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建委托解析器（源对象和目标对象参数）
        /// </summary>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <exception cref="ArgumentNullException">当 func 为 null 时抛出</exception>
        public FuncResolver(Func<TSource, TDestination, TResult> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
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
            try
            {
                // 执行委托函数
                return _func(source, destination);
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"执行委托函数时发生错误。函数：{_func.Method.Name}",
                    ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// 委托解析器（包含解析上下文参数）
    /// <para>Func resolver with resolution context parameter</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    public sealed class FuncResolverWithContext<TSource, TDestination, TDestMember> :
        ValueResolverBase<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private readonly Func<TSource, TDestination, ResolutionContext, TDestMember> _func;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建委托解析器（包含解析上下文参数）
        /// </summary>
        /// <param name="func">委托函数 / Delegate function</param>
        /// <exception cref="ArgumentNullException">当 func 为 null 时抛出</exception>
        public FuncResolverWithContext(Func<TSource, TDestination, ResolutionContext, TDestMember> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
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
            try
            {
                // 执行委托函数
                return _func(source, destination, context);
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"执行委托函数时发生错误。函数：{_func.Method.Name}",
                    ex);
            }
        }

        #endregion
    }
}
