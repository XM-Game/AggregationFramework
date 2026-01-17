// ==========================================================
// 文件名：IMappingExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions
// 功能: 定义类型映射配置表达式接口，提供成员级别的映射配置
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型映射配置表达式接口（非泛型）
    /// <para>Type mapping configuration expression interface (non-generic)</para>
    /// </summary>
    public interface IMappingExpression
    {
        /// <summary>
        /// 获取类型映射
        /// <para>Get the type map</para>
        /// </summary>
        ITypeMap TypeMap { get; }
    }

    /// <summary>
    /// 类型映射配置表达式接口
    /// <para>提供流式 API 用于配置单个类型对的映射规则</para>
    /// <para>Type mapping configuration expression interface providing fluent API for configuring mapping rules</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <remarks>
    /// IMappingExpression 用于配置单个类型对的映射规则，支持：
    /// <list type="bullet">
    /// <item>成员映射配置（ForMember）</item>
    /// <item>路径映射配置（ForPath）</item>
    /// <item>构造函数配置（ConstructUsing）</item>
    /// <item>类型转换器配置（ConvertUsing）</item>
    /// <item>前置/后置映射动作</item>
    /// </list>
    /// </remarks>
    public interface IMappingExpression<TSource, TDestination> : IMappingExpression
    {
        #region 成员映射 / Member Mapping

        /// <summary>
        /// 配置目标成员的映射规则
        /// <para>Configure mapping rules for a destination member</para>
        /// </summary>
        /// <typeparam name="TMember">成员类型 / Member type</typeparam>
        /// <param name="destinationMember">目标成员表达式 / Destination member expression</param>
        /// <param name="memberOptions">成员配置委托 / Member configuration delegate</param>
        /// <returns>当前映射表达式（支持链式调用）/ Current mapping expression for chaining</returns>
        IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);

        /// <summary>
        /// 配置所有成员的映射规则
        /// <para>Configure mapping rules for all members</para>
        /// </summary>
        /// <param name="memberOptions">成员配置委托 / Member configuration delegate</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ForAllMembers(
            Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions);

        /// <summary>
        /// 配置所有其他成员的映射规则
        /// <para>Configure mapping rules for all other members</para>
        /// </summary>
        /// <param name="memberOptions">成员配置委托 / Member configuration delegate</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ForAllOtherMembers(
            Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions);

        #endregion

        #region 路径映射 / Path Mapping

        /// <summary>
        /// 配置嵌套路径的映射规则
        /// <para>Configure mapping rules for a nested path</para>
        /// </summary>
        /// <typeparam name="TMember">成员类型 / Member type</typeparam>
        /// <param name="destinationPath">目标路径表达式 / Destination path expression</param>
        /// <param name="pathOptions">路径配置委托 / Path configuration delegate</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ForPath<TMember>(
            Expression<Func<TDestination, TMember>> destinationPath,
            Action<IPathConfigurationExpression<TSource, TDestination, TMember>> pathOptions);

        #endregion

        #region 构造函数配置 / Constructor Configuration

        /// <summary>
        /// 配置构造函数参数映射
        /// <para>Configure constructor parameter mapping</para>
        /// </summary>
        /// <param name="parameterName">参数名称 / Parameter name</param>
        /// <param name="parameterOptions">参数配置委托 / Parameter configuration delegate</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ForCtorParam(
            string parameterName,
            Action<ICtorParamConfigurationExpression<TSource>> parameterOptions);

        /// <summary>
        /// 使用自定义表达式构造目标对象
        /// <para>Use custom expression to construct destination object</para>
        /// </summary>
        /// <param name="ctor">构造表达式 / Construction expression</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConstructUsing(
            Expression<Func<TSource, TDestination>> ctor);

        /// <summary>
        /// 使用自定义函数构造目标对象
        /// <para>Use custom function to construct destination object</para>
        /// </summary>
        /// <param name="ctor">构造函数 / Construction function</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConstructUsing(
            Func<TSource, ResolutionContext, TDestination> ctor);

        /// <summary>
        /// 禁用构造函数映射
        /// <para>Disable constructor mapping</para>
        /// </summary>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> DisableCtorValidation();

        #endregion

        #region 类型转换器 / Type Converter

        /// <summary>
        /// 使用类型转换器进行映射
        /// <para>Use type converter for mapping</para>
        /// </summary>
        /// <typeparam name="TConverter">转换器类型 / Converter type</typeparam>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConvertUsing<TConverter>()
            where TConverter : ITypeConverter<TSource, TDestination>;

        /// <summary>
        /// 使用转换表达式进行映射
        /// <para>Use conversion expression for mapping</para>
        /// </summary>
        /// <param name="converter">转换表达式 / Conversion expression</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConvertUsing(
            Expression<Func<TSource, TDestination>> converter);

        /// <summary>
        /// 使用转换函数进行映射
        /// <para>Use conversion function for mapping</para>
        /// </summary>
        /// <param name="converter">转换函数 / Conversion function</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConvertUsing(
            Func<TSource, TDestination> converter);

        /// <summary>
        /// 使用带上下文的转换函数进行映射
        /// <para>Use conversion function with context for mapping</para>
        /// </summary>
        /// <param name="converter">转换函数 / Conversion function</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> ConvertUsing(
            Func<TSource, TDestination, ResolutionContext, TDestination> converter);

        #endregion

        #region 映射动作 / Mapping Actions

        /// <summary>
        /// 添加映射前动作
        /// <para>Add before map action</para>
        /// </summary>
        /// <param name="beforeFunction">前置动作 / Before action</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> BeforeMap(
            Action<TSource, TDestination> beforeFunction);

        /// <summary>
        /// 添加带上下文的映射前动作
        /// <para>Add before map action with context</para>
        /// </summary>
        /// <param name="beforeFunction">前置动作 / Before action</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> BeforeMap(
            Action<TSource, TDestination, ResolutionContext> beforeFunction);

        /// <summary>
        /// 添加映射前动作（使用 IMappingAction）
        /// <para>Add before map action using IMappingAction</para>
        /// </summary>
        /// <typeparam name="TMappingAction">映射动作类型 / Mapping action type</typeparam>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> BeforeMap<TMappingAction>()
            where TMappingAction : IMappingAction<TSource, TDestination>;

        /// <summary>
        /// 添加映射后动作
        /// <para>Add after map action</para>
        /// </summary>
        /// <param name="afterFunction">后置动作 / After action</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> AfterMap(
            Action<TSource, TDestination> afterFunction);

        /// <summary>
        /// 添加带上下文的映射后动作
        /// <para>Add after map action with context</para>
        /// </summary>
        /// <param name="afterFunction">后置动作 / After action</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> AfterMap(
            Action<TSource, TDestination, ResolutionContext> afterFunction);

        /// <summary>
        /// 添加映射后动作（使用 IMappingAction）
        /// <para>Add after map action using IMappingAction</para>
        /// </summary>
        /// <typeparam name="TMappingAction">映射动作类型 / Mapping action type</typeparam>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> AfterMap<TMappingAction>()
            where TMappingAction : IMappingAction<TSource, TDestination>;

        #endregion

        #region 继承映射 / Inheritance Mapping

        /// <summary>
        /// 包含派生类型映射
        /// <para>Include derived type mapping</para>
        /// </summary>
        /// <typeparam name="TDerivedSource">派生源类型 / Derived source type</typeparam>
        /// <typeparam name="TDerivedDestination">派生目标类型 / Derived destination type</typeparam>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> Include<TDerivedSource, TDerivedDestination>()
            where TDerivedSource : TSource
            where TDerivedDestination : TDestination;

        /// <summary>
        /// 包含基类型映射
        /// <para>Include base type mapping</para>
        /// </summary>
        /// <typeparam name="TBaseSource">基源类型 / Base source type</typeparam>
        /// <typeparam name="TBaseDestination">基目标类型 / Base destination type</typeparam>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> IncludeBase<TBaseSource, TBaseDestination>();

        /// <summary>
        /// 包含成员类型
        /// <para>Include member types</para>
        /// </summary>
        /// <param name="memberExpressions">成员表达式 / Member expressions</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> IncludeMembers(
            params Expression<Func<TSource, object>>[] memberExpressions);

        #endregion

        #region 映射选项 / Mapping Options

        /// <summary>
        /// 设置最大映射深度
        /// <para>Set maximum mapping depth</para>
        /// </summary>
        /// <param name="depth">最大深度 / Maximum depth</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> MaxDepth(int depth);

        /// <summary>
        /// 保留引用（处理循环引用）
        /// <para>Preserve references (handle circular references)</para>
        /// </summary>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> PreserveReferences();

        /// <summary>
        /// 设置映射条件
        /// <para>Set mapping condition</para>
        /// </summary>
        /// <param name="condition">条件表达式 / Condition expression</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> Condition(
            Func<TSource, TDestination, bool> condition);

        /// <summary>
        /// 添加值转换器
        /// <para>Add value transformer</para>
        /// </summary>
        /// <typeparam name="TValue">值类型 / Value type</typeparam>
        /// <param name="transformer">转换器 / Transformer</param>
        /// <returns>当前映射表达式 / Current mapping expression</returns>
        IMappingExpression<TSource, TDestination> AddTransform<TValue>(
            Expression<Func<TValue, TValue>> transformer);

        #endregion

        #region 反向映射 / Reverse Mapping

        /// <summary>
        /// 创建反向映射
        /// <para>Create reverse mapping</para>
        /// </summary>
        /// <returns>反向映射表达式 / Reverse mapping expression</returns>
        IMappingExpression<TDestination, TSource> ReverseMap();

        #endregion
    }
}
