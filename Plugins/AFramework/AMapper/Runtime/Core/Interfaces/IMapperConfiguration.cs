// ==========================================================
// 文件名：IMapperConfiguration.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 定义映射配置容器接口，管理类型映射配置
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射配置接口
    /// <para>定义映射配置容器的核心能力，包括类型映射管理、配置验证和映射器创建</para>
    /// <para>Mapper configuration interface that defines configuration management capabilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>不可变性：配置创建后不可修改，保证线程安全</item>
    /// <item>单例模式：推荐每个应用程序使用一个配置实例</item>
    /// <item>延迟编译：执行计划在首次使用时编译</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// var config = new MapperConfiguration(cfg =>
    /// {
    ///     cfg.AddProfile&lt;GameProfile&gt;();
    ///     cfg.CreateMap&lt;Player, PlayerDto&gt;();
    /// });
    /// 
    /// config.AssertConfigurationIsValid();
    /// var mapper = config.CreateMapper();
    /// </code>
    /// </remarks>
    public interface IMapperConfiguration
    {
        #region 映射器创建 / Mapper Creation

        /// <summary>
        /// 创建映射器实例
        /// <para>Create a mapper instance</para>
        /// </summary>
        /// <returns>映射器实例 / Mapper instance</returns>
        /// <remarks>
        /// 映射器实例是轻量级的，可以按需创建多个。
        /// 所有映射器共享同一个配置实例。
        /// </remarks>
        IAMapper CreateMapper();

        /// <summary>
        /// 创建映射器实例，使用指定的服务提供者
        /// <para>Create a mapper instance with specified service provider</para>
        /// </summary>
        /// <param name="serviceProvider">服务提供者，用于解析依赖 / Service provider for dependency resolution</param>
        /// <returns>映射器实例 / Mapper instance</returns>
        IAMapper CreateMapper(IServiceProvider serviceProvider);

        #endregion

        #region 类型映射查询 / TypeMap Query

        /// <summary>
        /// 获取指定类型对的类型映射配置
        /// <para>Get the type map for specified type pair</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>类型映射配置，如果不存在则返回 null / Type map or null if not found</returns>
        ITypeMap FindTypeMap<TSource, TDestination>();

        /// <summary>
        /// 获取指定类型对的类型映射配置（非泛型版本）
        /// <para>Get the type map for specified type pair (non-generic version)</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>类型映射配置，如果不存在则返回 null / Type map or null if not found</returns>
        ITypeMap FindTypeMap(Type sourceType, Type destinationType);

        /// <summary>
        /// 获取所有已配置的类型映射
        /// <para>Get all configured type maps</para>
        /// </summary>
        /// <returns>类型映射集合 / Collection of type maps</returns>
        IReadOnlyCollection<ITypeMap> GetAllTypeMaps();

        /// <summary>
        /// 检查指定类型对是否已配置映射
        /// <para>Check if mapping is configured for specified type pair</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>是否已配置 / Whether configured</returns>
        bool HasTypeMap<TSource, TDestination>();

        /// <summary>
        /// 检查指定类型对是否已配置映射（非泛型版本）
        /// <para>Check if mapping is configured for specified type pair (non-generic version)</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>是否已配置 / Whether configured</returns>
        bool HasTypeMap(Type sourceType, Type destinationType);

        #endregion

        #region 执行计划 / Execution Plan

        /// <summary>
        /// 获取指定类型对的映射执行计划表达式
        /// <para>Get the mapping execution plan expression for specified type pair</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>执行计划表达式 / Execution plan expression</returns>
        /// <remarks>
        /// 此方法用于调试和诊断，可以查看 AMapper 生成的映射表达式。
        /// </remarks>
        LambdaExpression BuildExecutionPlan<TSource, TDestination>();

        /// <summary>
        /// 获取指定类型对的映射执行计划表达式（非泛型版本）
        /// <para>Get the mapping execution plan expression (non-generic version)</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>执行计划表达式 / Execution plan expression</returns>
        LambdaExpression BuildExecutionPlan(Type sourceType, Type destinationType);

        /// <summary>
        /// 预编译所有映射
        /// <para>Pre-compile all mappings</para>
        /// </summary>
        /// <remarks>
        /// 此方法会编译所有已配置的映射，避免首次映射时的编译开销。
        /// 建议在应用程序启动或加载界面时调用。
        /// </remarks>
        void CompileMappings();

        #endregion

        #region 配置验证 / Configuration Validation

        /// <summary>
        /// 验证配置有效性，如果无效则抛出异常
        /// <para>Assert that configuration is valid, throw exception if not</para>
        /// </summary>
        /// <exception cref="ConfigurationException">当配置无效时抛出 / Thrown when configuration is invalid</exception>
        /// <remarks>
        /// 验证内容包括：
        /// <list type="bullet">
        /// <item>所有目标成员都有映射源</item>
        /// <item>构造函数参数可以解析</item>
        /// <item>类型转换可行</item>
        /// <item>无重复配置</item>
        /// </list>
        /// 建议在开发阶段调用此方法。
        /// </remarks>
        void AssertConfigurationIsValid();

        /// <summary>
        /// 验证指定类型映射的配置有效性
        /// <para>Assert that specified type map configuration is valid</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <exception cref="ConfigurationException">当配置无效时抛出 / Thrown when configuration is invalid</exception>
        void AssertConfigurationIsValid<TSource, TDestination>();

        #endregion

        #region 内置映射器 / Built-in Mappers

        /// <summary>
        /// 获取内置对象映射器列表
        /// <para>Get the list of built-in object mappers</para>
        /// </summary>
        IReadOnlyList<IObjectMapper> Mappers { get; }

        #endregion

        #region 服务提供者 / Service Provider

        /// <summary>
        /// 获取或设置服务提供者
        /// <para>Get or set the service provider</para>
        /// </summary>
        /// <remarks>
        /// 服务提供者用于解析自定义值解析器、类型转换器等的依赖。
        /// 与 AFramework.DI 集成时会自动设置。
        /// </remarks>
        IServiceProvider ServiceProvider { get; }

        #endregion
    }
}
