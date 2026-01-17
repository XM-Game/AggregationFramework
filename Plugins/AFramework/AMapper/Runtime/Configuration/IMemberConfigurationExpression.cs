// ==========================================================
// 文件名：IMemberConfigurationExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions
// 功能: 定义成员配置表达式接口，提供成员级别的映射配置
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员配置表达式接口
    /// <para>提供流式 API 用于配置单个成员的映射规则</para>
    /// <para>Member configuration expression interface providing fluent API for configuring member mapping rules</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TMember">成员类型 / Member type</typeparam>
    /// <remarks>
    /// IMemberConfigurationExpression 用于配置单个成员的映射规则，支持：
    /// <list type="bullet">
    /// <item>指定映射来源（MapFrom）</item>
    /// <item>忽略成员（Ignore）</item>
    /// <item>条件映射（Condition）</item>
    /// <item>空值替换（NullSubstitute）</item>
    /// <item>值转换器（ConvertUsing）</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// CreateMap&lt;Player, PlayerDto&gt;()
    ///     .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
    ///     .ForMember(d => d.InternalId, opt => opt.Ignore())
    ///     .ForMember(d => d.Status, opt => opt.Condition(s => s.IsActive));
    /// </code>
    /// </remarks>
    public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
    {
        #region 映射来源 / Mapping Source

        /// <summary>
        /// 从源成员表达式映射
        /// <para>Map from source member expression</para>
        /// </summary>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        /// <param name="sourceMember">源成员表达式 / Source member expression</param>
        void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);

        /// <summary>
        /// 从源成员名称映射
        /// <para>Map from source member name</para>
        /// </summary>
        /// <param name="sourceMemberName">源成员名称 / Source member name</param>
        void MapFrom(string sourceMemberName);

        /// <summary>
        /// 使用值解析器映射
        /// <para>Map using value resolver</para>
        /// </summary>
        /// <typeparam name="TValueResolver">值解析器类型 / Value resolver type</typeparam>
        void MapFrom<TValueResolver>() where TValueResolver : IValueResolver<TSource, TDestination, TMember>;

        /// <summary>
        /// 使用值解析器实例映射
        /// <para>Map using value resolver instance</para>
        /// </summary>
        /// <typeparam name="TValueResolver">值解析器类型 / Value resolver type</typeparam>
        /// <param name="resolver">值解析器实例 / Value resolver instance</param>
        void MapFrom<TValueResolver>(TValueResolver resolver) where TValueResolver : IValueResolver<TSource, TDestination, TMember>;

        /// <summary>
        /// 使用成员值解析器映射
        /// <para>Map using member value resolver</para>
        /// </summary>
        /// <typeparam name="TValueResolver">值解析器类型 / Value resolver type</typeparam>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        /// <param name="sourceMember">源成员表达式 / Source member expression</param>
        void MapFrom<TValueResolver, TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
            where TValueResolver : IMemberValueResolver<TSource, TDestination, TSourceMember, TMember>;

        /// <summary>
        /// 使用函数映射
        /// <para>Map using function</para>
        /// </summary>
        /// <param name="resolver">解析函数 / Resolver function</param>
        void MapFrom(Func<TSource, TMember> resolver);

        /// <summary>
        /// 使用带目标的函数映射
        /// <para>Map using function with destination</para>
        /// </summary>
        /// <param name="resolver">解析函数 / Resolver function</param>
        void MapFrom(Func<TSource, TDestination, TMember> resolver);

        /// <summary>
        /// 使用带上下文的函数映射
        /// <para>Map using function with context</para>
        /// </summary>
        /// <param name="resolver">解析函数 / Resolver function</param>
        void MapFrom(Func<TSource, TDestination, TMember, ResolutionContext, TMember> resolver);

        #endregion

        #region 忽略 / Ignore

        /// <summary>
        /// 忽略此成员的映射
        /// <para>Ignore mapping for this member</para>
        /// </summary>
        void Ignore();

        #endregion

        #region 条件映射 / Conditional Mapping

        /// <summary>
        /// 设置映射条件
        /// <para>Set mapping condition</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void Condition(Func<TSource, bool> condition);

        /// <summary>
        /// 设置带目标的映射条件
        /// <para>Set mapping condition with destination</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void Condition(Func<TSource, TDestination, bool> condition);

        /// <summary>
        /// 设置带成员的映射条件
        /// <para>Set mapping condition with member</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void Condition(Func<TSource, TDestination, TMember, bool> condition);

        /// <summary>
        /// 设置带上下文的映射条件
        /// <para>Set mapping condition with context</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void Condition(Func<TSource, TDestination, TMember, ResolutionContext, bool> condition);

        /// <summary>
        /// 设置前置条件
        /// <para>Set pre-condition</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void PreCondition(Func<TSource, bool> condition);

        /// <summary>
        /// 设置带上下文的前置条件
        /// <para>Set pre-condition with context</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void PreCondition(Func<TSource, ResolutionContext, bool> condition);

        #endregion

        #region 空值处理 / Null Handling

        /// <summary>
        /// 设置空值替换值
        /// <para>Set null substitute value</para>
        /// </summary>
        /// <param name="nullSubstitute">替换值 / Substitute value</param>
        void NullSubstitute(TMember nullSubstitute);

        /// <summary>
        /// 允许空值
        /// <para>Allow null value</para>
        /// </summary>
        void AllowNull();

        /// <summary>
        /// 禁止空值
        /// <para>Disallow null value</para>
        /// </summary>
        void DoNotAllowNull();

        #endregion

        #region 值转换 / Value Conversion

        /// <summary>
        /// 使用值转换器
        /// <para>Use value converter</para>
        /// </summary>
        /// <typeparam name="TValueConverter">值转换器类型 / Value converter type</typeparam>
        void ConvertUsing<TValueConverter>() where TValueConverter : IValueConverter<TMember, TMember>;

        /// <summary>
        /// 使用值转换器（指定源类型）
        /// <para>Use value converter with source type</para>
        /// </summary>
        /// <typeparam name="TValueConverter">值转换器类型 / Value converter type</typeparam>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        void ConvertUsing<TValueConverter, TSourceMember>()
            where TValueConverter : IValueConverter<TSourceMember, TMember>;

        /// <summary>
        /// 使用转换表达式
        /// <para>Use conversion expression</para>
        /// </summary>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        /// <param name="converter">转换表达式 / Conversion expression</param>
        void ConvertUsing<TSourceMember>(Expression<Func<TSourceMember, TMember>> converter);

        #endregion

        #region 其他选项 / Other Options

        /// <summary>
        /// 使用目标值（不创建新实例）
        /// <para>Use destination value (don't create new instance)</para>
        /// </summary>
        void UseDestinationValue();

        /// <summary>
        /// 设置映射顺序
        /// <para>Set mapping order</para>
        /// </summary>
        /// <param name="order">顺序值 / Order value</param>
        void SetMappingOrder(int order);

        /// <summary>
        /// 添加值转换器
        /// <para>Add value transformer</para>
        /// </summary>
        /// <param name="transformer">转换器表达式 / Transformer expression</param>
        void AddTransform(Expression<Func<TMember, TMember>> transformer);

        #endregion
    }

    /// <summary>
    /// 路径配置表达式接口
    /// <para>Path configuration expression interface</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TMember">成员类型 / Member type</typeparam>
    public interface IPathConfigurationExpression<TSource, TDestination, TMember>
    {
        /// <summary>
        /// 从源成员表达式映射
        /// <para>Map from source member expression</para>
        /// </summary>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        /// <param name="sourceMember">源成员表达式 / Source member expression</param>
        void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);

        /// <summary>
        /// 忽略此路径的映射
        /// <para>Ignore mapping for this path</para>
        /// </summary>
        void Ignore();

        /// <summary>
        /// 设置映射条件
        /// <para>Set mapping condition</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        void Condition(Func<TSource, bool> condition);
    }

    /// <summary>
    /// 构造函数参数配置表达式接口
    /// <para>Constructor parameter configuration expression interface</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    public interface ICtorParamConfigurationExpression<TSource>
    {
        /// <summary>
        /// 从源成员表达式映射
        /// <para>Map from source member expression</para>
        /// </summary>
        /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
        /// <param name="sourceMember">源成员表达式 / Source member expression</param>
        void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);

        /// <summary>
        /// 从源成员名称映射
        /// <para>Map from source member name</para>
        /// </summary>
        /// <param name="sourceMemberName">源成员名称 / Source member name</param>
        void MapFrom(string sourceMemberName);
    }
}
