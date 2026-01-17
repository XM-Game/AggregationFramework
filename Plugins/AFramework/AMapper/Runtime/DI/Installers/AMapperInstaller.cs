// ==========================================================
// 文件名：AMapperInstaller.cs
// 命名空间: AFramework.AMapper.DI
// 依赖: AFramework.DI
// 功能: AMapper 安装器基类，提供映射器注册的基础功能
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.AMapper.DI
{
    /// <summary>
    /// AMapper 安装器基类
    /// <para>提供 AMapper 映射器注册的基础功能，支持自定义配置</para>
    /// <para>AMapper installer base class providing basic mapper registration functionality</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>模板方法模式：定义注册流程，子类实现具体配置</item>
    /// <item>单一职责：仅负责 AMapper 相关服务的注册</item>
    /// <item>开闭原则：通过继承扩展配置逻辑</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// public class MyMapperInstaller : AMapperInstaller
    /// {
    ///     protected override void ConfigureMapper(IMapperConfigurationExpression cfg)
    ///     {
    ///         cfg.AddProfile&lt;MyMappingProfile&gt;();
    ///         cfg.CreateMap&lt;Source, Dest&gt;();
    ///     }
    /// }
    /// 
    /// // 使用安装器
    /// builder.UseInstaller&lt;MyMapperInstaller&gt;();
    /// </code>
    /// </remarks>
    public abstract class AMapperInstaller : IInstaller
    {
        #region IInstaller 实现 / IInstaller Implementation

        /// <summary>
        /// 安装 AMapper 服务到容器
        /// <para>Install AMapper services to container</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        public virtual void Install(IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            // 创建映射器配置
            var configuration = CreateMapperConfiguration();

            // 注册映射器配置（单例）
            builder.RegisterInstance<IMapperConfiguration>(configuration);

            // 注册映射器（单例）
            builder.RegisterFactory<IAMapper>(resolver =>
            {
                var config = resolver.Resolve<IMapperConfiguration>() as MapperConfiguration;
                return new Mapper(config);
            }).Singleton();

            // 注册额外服务
            InstallAdditionalServices(builder);
        }

        #endregion

        #region 受保护方法 / Protected Methods

        /// <summary>
        /// 创建映射器配置
        /// <para>Create mapper configuration</para>
        /// </summary>
        /// <returns>映射器配置 / Mapper configuration</returns>
        protected virtual IMapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                // 配置映射器
                ConfigureMapper(cfg);

                // 注册 Unity 类型映射器（如果需要）
                if (ShouldRegisterUnityMappers())
                {
                    RegisterUnityMappers(cfg);
                }
            });
        }

        /// <summary>
        /// 配置映射器（由子类实现）
        /// <para>Configure mapper (implemented by subclass)</para>
        /// </summary>
        /// <param name="cfg">映射器配置表达式 / Mapper configuration expression</param>
        protected abstract void ConfigureMapper(IMapperConfigurationExpression cfg);

        /// <summary>
        /// 判断是否应该注册 Unity 类型映射器
        /// <para>Check if Unity type mappers should be registered</para>
        /// </summary>
        /// <returns>是否注册 / Whether to register</returns>
        /// <remarks>
        /// 默认返回 true。
        /// 子类可以重写此方法以控制是否注册 Unity 类型映射器。
        /// </remarks>
        protected virtual bool ShouldRegisterUnityMappers()
        {
            return true;
        }

        /// <summary>
        /// 注册 Unity 类型映射器
        /// <para>Register Unity type mappers</para>
        /// </summary>
        /// <param name="cfg">映射器配置表达式 / Mapper configuration expression</param>
        protected virtual void RegisterUnityMappers(IMapperConfigurationExpression cfg)
        {
#if UNITY_2022_3_OR_NEWER
            // 注册所有 Unity 类型映射器
            Unity.UnityTypeMapperRegistry.RegisterAll(MapperRegistry.Default);
#endif
        }

        /// <summary>
        /// 安装额外服务（由子类实现）
        /// <para>Install additional services (implemented by subclass)</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <remarks>
        /// 子类可以重写此方法以注册额外的服务，如自定义解析器、转换器等。
        /// </remarks>
        protected virtual void InstallAdditionalServices(IContainerBuilder builder)
        {
            // 默认不注册额外服务
        }

        #endregion
    }
}
