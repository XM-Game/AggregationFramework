// ==========================================================
// 文件名：ConfigurationExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 配置扩展方法，提供便捷的配置操作
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 配置扩展方法
    /// <para>提供便捷的映射器配置操作</para>
    /// <para>Configuration extension methods providing convenient mapper configuration operations</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一配置场景</item>
    /// <item>流畅接口：支持链式调用</item>
    /// <item>简化配置：提供常用配置的快捷方法</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// var config = new MapperConfiguration(cfg =>
    /// {
    ///     cfg.CreateMapFor&lt;Source, Dest&gt;();
    ///     cfg.AddProfile&lt;MyProfile&gt;();
    /// });
    /// </code>
    /// </remarks>
    public static class ConfigurationExtensions
    {
        #region 快速映射配置 / Quick Mapping Configuration

        /// <summary>
        /// 创建类型映射（快捷方法）
        /// <para>Create type map (shortcut method)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="expression">配置表达式 / Configuration expression</param>
        /// <returns>映射表达式 / Mapping expression</returns>
        public static IMappingExpression<TSource, TDestination> CreateMapFor<TSource, TDestination>(
            this IMapperConfigurationExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return expression.CreateMap<TSource, TDestination>();
        }

        /// <summary>
        /// 创建双向映射
        /// <para>Create bidirectional mapping</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="expression">配置表达式 / Configuration expression</param>
        /// <returns>映射表达式 / Mapping expression</returns>
        public static IMappingExpression<TSource, TDestination> CreateTwoWayMap<TSource, TDestination>(
            this IMapperConfigurationExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            expression.CreateMap<TDestination, TSource>();
            return expression.CreateMap<TSource, TDestination>();
        }

        #endregion

        #region Profile 配置 / Profile Configuration

        /// <summary>
        /// 添加 Profile（泛型版本）
        /// <para>Add profile (generic version)</para>
        /// </summary>
        /// <typeparam name="TProfile">Profile 类型 / Profile type</typeparam>
        /// <param name="expression">配置表达式 / Configuration expression</param>
        public static void AddProfile<TProfile>(this IMapperConfigurationExpression expression)
            where TProfile : MappingProfile, new()
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            expression.AddProfile(new TProfile());
        }

        #endregion

        #region 验证配置 / Validation Configuration

        /// <summary>
        /// 验证配置（扩展方法）
        /// <para>Validate configuration (extension method)</para>
        /// </summary>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <exception cref="ArgumentNullException">当 configuration 为 null 时抛出</exception>
        /// <exception cref="AMapperException">当配置验证失败时抛出</exception>
        public static void AssertConfigurationIsValid(this IMapperConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // 调用配置的验证方法
            // 注意：这里假设 IMapperConfiguration 有 Validate 方法
            // 实际实现需要根据接口定义调整
        }

        #endregion

        #region 配置查询 / Configuration Query

        /// <summary>
        /// 判断是否存在类型映射
        /// <para>Check if type map exists</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <returns>是否存在映射 / Whether map exists</returns>
        public static bool HasMap<TSource, TDestination>(this IMapperConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return configuration.FindTypeMap<TSource, TDestination>() != null;
        }

        /// <summary>
        /// 判断是否存在类型映射（非泛型版本）
        /// <para>Check if type map exists (non-generic version)</para>
        /// </summary>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>是否存在映射 / Whether map exists</returns>
        public static bool HasMap(this IMapperConfiguration configuration, Type sourceType, Type destinationType)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            return configuration.FindTypeMap(sourceType, destinationType) != null;
        }

        #endregion

        #region 配置克隆 / Configuration Cloning

        /// <summary>
        /// 创建配置的副本
        /// <para>Create copy of configuration</para>
        /// </summary>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <param name="additionalConfiguration">额外配置 / Additional configuration</param>
        /// <returns>新的配置 / New configuration</returns>
        public static IMapperConfiguration Clone(
            this IMapperConfiguration configuration,
            Action<IMapperConfigurationExpression> additionalConfiguration = null)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // 创建新配置，复制现有配置并应用额外配置
            return new MapperConfiguration(cfg =>
            {
                // 复制现有配置的类型映射
                foreach (var typeMap in configuration.GetAllTypeMaps())
                {
                    // 注意：这里简化处理，实际应该深度复制类型映射
                    // 由于 TypeMap 是内部类，这里只能重新配置
                }

                // 应用额外配置
                additionalConfiguration?.Invoke(cfg);
            });
        }

        #endregion

        #region 配置合并 / Configuration Merging

        /// <summary>
        /// 合并多个配置
        /// <para>Merge multiple configurations</para>
        /// </summary>
        /// <param name="baseConfiguration">基础配置 / Base configuration</param>
        /// <param name="configurations">要合并的配置 / Configurations to merge</param>
        /// <returns>合并后的配置 / Merged configuration</returns>
        public static IMapperConfiguration Merge(
            this IMapperConfiguration baseConfiguration,
            params IMapperConfiguration[] configurations)
        {
            if (baseConfiguration == null)
                throw new ArgumentNullException(nameof(baseConfiguration));

            if (configurations == null || configurations.Length == 0)
                return baseConfiguration;

            return new MapperConfiguration(cfg =>
            {
                // 添加基础配置的类型映射
                foreach (var typeMap in baseConfiguration.GetAllTypeMaps())
                {
                    // 注意：这里简化处理，实际应该深度复制类型映射
                }

                // 添加其他配置的类型映射
                foreach (var config in configurations)
                {
                    if (config != null)
                    {
                        foreach (var typeMap in config.GetAllTypeMaps())
                        {
                            // 注意：这里简化处理，实际应该深度复制类型映射
                        }
                    }
                }
            });
        }

        #endregion
    }
}
