// ==========================================================
// 文件名：ScopedMapperFactory.cs
// 命名空间: AFramework.AMapper.DI
// 依赖: System, AFramework.DI
// 功能: 作用域映射器工厂，支持在不同作用域中创建映射器实例
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.AMapper.DI
{
    /// <summary>
    /// 作用域映射器工厂
    /// <para>支持在不同作用域中创建和管理映射器实例</para>
    /// <para>Scoped mapper factory supporting mapper creation and management in different scopes</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>工厂模式：封装映射器的创建逻辑</item>
    /// <item>单一职责：仅负责映射器的创建和作用域管理</item>
    /// <item>依赖注入：通过容器解析依赖</item>
    /// </list>
    /// 
    /// 使用场景：
    /// <list type="bullet">
    /// <item>需要在不同作用域中使用独立的映射器实例</item>
    /// <item>需要动态配置映射器</item>
    /// <item>需要控制映射器的生命周期</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 注册工厂
    /// builder.RegisterFactory&lt;IScopedMapperFactory&gt;(resolver =>
    ///     new ScopedMapperFactory(resolver)).Singleton();
    /// 
    /// // 使用工厂创建映射器
    /// var factory = resolver.Resolve&lt;IScopedMapperFactory&gt;();
    /// var mapper = factory.CreateMapper(cfg =>
    /// {
    ///     cfg.CreateMap&lt;Source, Dest&gt;();
    /// });
    /// </code>
    /// </remarks>
    public sealed class ScopedMapperFactory : IScopedMapperFactory
    {
        #region 私有字段 / Private Fields

        private readonly IObjectResolver _resolver;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建作用域映射器工厂
        /// <para>Create scoped mapper factory</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <exception cref="ArgumentNullException">当 resolver 为 null 时抛出</exception>
        public ScopedMapperFactory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IScopedMapperFactory 实现 / IScopedMapperFactory Implementation

        /// <summary>
        /// 创建映射器
        /// <para>Create mapper</para>
        /// </summary>
        /// <param name="configurationAction">配置操作 / Configuration action</param>
        /// <returns>映射器实例 / Mapper instance</returns>
        public IAMapper CreateMapper(Action<IMapperConfigurationExpression> configurationAction)
        {
            if (configurationAction == null)
                throw new ArgumentNullException(nameof(configurationAction));

            // 创建映射器配置
            var configuration = new MapperConfiguration(configurationAction);

            // 创建映射器
            return new Mapper(configuration, new ServiceProviderAdapter(_resolver));
        }

        /// <summary>
        /// 创建映射器（使用现有配置）
        /// <para>Create mapper (using existing configuration)</para>
        /// </summary>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <returns>映射器实例 / Mapper instance</returns>
        public IAMapper CreateMapper(IMapperConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var mapperConfig = configuration as MapperConfiguration;
            return new Mapper(mapperConfig, new ServiceProviderAdapter(_resolver));
        }

        /// <summary>
        /// 创建作用域映射器
        /// <para>Create scoped mapper</para>
        /// </summary>
        /// <param name="configurationAction">配置操作 / Configuration action</param>
        /// <returns>作用域映射器实例 / Scoped mapper instance</returns>
        /// <remarks>
        /// 作用域映射器会在当前作用域中创建，并在作用域销毁时自动释放。
        /// </remarks>
        public IAMapper CreateScopedMapper(Action<IMapperConfigurationExpression> configurationAction)
        {
            if (configurationAction == null)
                throw new ArgumentNullException(nameof(configurationAction));

            // 创建子作用域
            var scope = _resolver.CreateScope();

            // 在子作用域中创建映射器配置
            var configuration = new MapperConfiguration(configurationAction);

            // 创建映射器
            return new Mapper(configuration, new ServiceProviderAdapter(scope));
        }

        #endregion
    }

    /// <summary>
    /// 作用域映射器工厂接口
    /// <para>Scoped mapper factory interface</para>
    /// </summary>
    public interface IScopedMapperFactory
    {
        /// <summary>
        /// 创建映射器
        /// <para>Create mapper</para>
        /// </summary>
        /// <param name="configurationAction">配置操作 / Configuration action</param>
        /// <returns>映射器实例 / Mapper instance</returns>
        IAMapper CreateMapper(Action<IMapperConfigurationExpression> configurationAction);

        /// <summary>
        /// 创建映射器（使用现有配置）
        /// <para>Create mapper (using existing configuration)</para>
        /// </summary>
        /// <param name="configuration">映射器配置 / Mapper configuration</param>
        /// <returns>映射器实例 / Mapper instance</returns>
        IAMapper CreateMapper(IMapperConfiguration configuration);

        /// <summary>
        /// 创建作用域映射器
        /// <para>Create scoped mapper</para>
        /// </summary>
        /// <param name="configurationAction">配置操作 / Configuration action</param>
        /// <returns>作用域映射器实例 / Scoped mapper instance</returns>
        IAMapper CreateScopedMapper(Action<IMapperConfigurationExpression> configurationAction);
    }
}
