// ==========================================================
// 文件名：IMapperConfigurationExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Reflection
// 功能: 定义映射配置表达式接口，提供流式配置 API
// ==========================================================

using System;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射配置表达式接口
    /// <para>提供流式 API 用于配置映射规则</para>
    /// <para>Mapper configuration expression interface providing fluent API for configuring mapping rules</para>
    /// </summary>
    /// <remarks>
    /// IMapperConfigurationExpression 是配置 AMapper 的主要入口点，支持：
    /// <list type="bullet">
    /// <item>创建类型映射（CreateMap）</item>
    /// <item>添加 Profile（AddProfile）</item>
    /// <item>配置全局选项</item>
    /// <item>注册自定义映射器</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// var config = new MapperConfiguration(cfg =>
    /// {
    ///     cfg.AddProfile&lt;GameProfile&gt;();
    ///     cfg.CreateMap&lt;Player, PlayerDto&gt;();
    /// });
    /// </code>
    /// </remarks>
    public interface IMapperConfigurationExpression
    {
        #region 类型映射创建 / Type Map Creation

        /// <summary>
        /// 创建类型映射配置
        /// <para>Create a type mapping configuration</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>类型映射配置表达式 / Type map configuration expression</returns>
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();

        /// <summary>
        /// 创建类型映射配置（非泛型版本）
        /// <para>Create a type mapping configuration (non-generic version)</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>类型映射配置表达式 / Type map configuration expression</returns>
        IMappingExpression CreateMap(Type sourceType, Type destinationType);

        #endregion

        #region Profile 管理 / Profile Management

        /// <summary>
        /// 添加映射配置文件
        /// <para>Add a mapping profile</para>
        /// </summary>
        /// <typeparam name="TProfile">Profile 类型 / Profile type</typeparam>
        void AddProfile<TProfile>() where TProfile : MappingProfile, new();

        /// <summary>
        /// 添加映射配置文件实例
        /// <para>Add a mapping profile instance</para>
        /// </summary>
        /// <param name="profile">Profile 实例 / Profile instance</param>
        void AddProfile(MappingProfile profile);

        /// <summary>
        /// 添加映射配置文件（按类型）
        /// <para>Add a mapping profile by type</para>
        /// </summary>
        /// <param name="profileType">Profile 类型 / Profile type</param>
        void AddProfile(Type profileType);

        /// <summary>
        /// 从程序集中添加所有 Profile
        /// <para>Add all profiles from an assembly</para>
        /// </summary>
        /// <param name="assembly">程序集 / Assembly</param>
        void AddProfiles(Assembly assembly);

        /// <summary>
        /// 从多个程序集中添加所有 Profile
        /// <para>Add all profiles from multiple assemblies</para>
        /// </summary>
        /// <param name="assemblies">程序集数组 / Array of assemblies</param>
        void AddProfiles(params Assembly[] assemblies);

        #endregion

        #region 全局配置 / Global Configuration

        /// <summary>
        /// 配置全局成员匹配规则
        /// <para>Configure global member matching rules</para>
        /// </summary>
        /// <param name="configure">配置委托 / Configuration delegate</param>
        void AddGlobalIgnore(string propertyNameStartingWith);

        /// <summary>
        /// 设置源成员命名约定
        /// <para>Set source member naming convention</para>
        /// </summary>
        /// <param name="convention">命名约定 / Naming convention</param>
        void SourceMemberNamingConvention(INamingConvention convention);

        /// <summary>
        /// 设置目标成员命名约定
        /// <para>Set destination member naming convention</para>
        /// </summary>
        /// <param name="convention">命名约定 / Naming convention</param>
        void DestinationMemberNamingConvention(INamingConvention convention);

        /// <summary>
        /// 允许空目标值
        /// <para>Allow null destination values</para>
        /// </summary>
        void AllowNullDestinationValues();

        /// <summary>
        /// 禁止空目标值
        /// <para>Disallow null destination values</para>
        /// </summary>
        void DisallowNullDestinationValues();

        /// <summary>
        /// 允许空集合
        /// <para>Allow null collections</para>
        /// </summary>
        void AllowNullCollections();

        /// <summary>
        /// 禁止空集合（返回空集合而非 null）
        /// <para>Disallow null collections (return empty collection instead of null)</para>
        /// </summary>
        void DisallowNullCollections();

        #endregion

        #region 自定义映射器 / Custom Mappers

        /// <summary>
        /// 添加自定义对象映射器
        /// <para>Add a custom object mapper</para>
        /// </summary>
        /// <param name="mapper">对象映射器 / Object mapper</param>
        void AddMapper(IObjectMapper mapper);

        /// <summary>
        /// 在指定位置插入自定义对象映射器
        /// <para>Insert a custom object mapper at specified position</para>
        /// </summary>
        /// <param name="index">位置索引 / Position index</param>
        /// <param name="mapper">对象映射器 / Object mapper</param>
        void InsertMapper(int index, IObjectMapper mapper);

        #endregion

        #region 服务提供者 / Service Provider

        /// <summary>
        /// 设置服务提供者
        /// <para>Set the service provider</para>
        /// </summary>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        void SetServiceProvider(IServiceProvider serviceProvider);

        #endregion
    }
}
