// ==========================================================
// 文件名：AMapperRegistrationExtensions.cs
// 命名空间: AFramework.AMapper.DI
// 依赖: System, AFramework.DI
// 功能: AMapper 与 AFramework.DI 的集成扩展方法
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.AMapper.DI
{
    /// <summary>
    /// AMapper 注册扩展方法
    /// <para>提供 AMapper 与 AFramework.DI 容器的集成</para>
    /// <para>Extension methods for integrating AMapper with AFramework.DI container</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>流畅接口：支持链式调用</item>
    /// <item>简化配置：提供多种便捷的注册方法</item>
    /// <item>类型安全：使用泛型确保类型安全</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 基本注册
    /// builder.RegisterAMapper(cfg =>
    /// {
    ///     cfg.AddProfile&lt;GameProfile&gt;();
    ///     cfg.CreateMap&lt;Source, Dest&gt;();
    /// });
    /// 
    /// // 使用 Profile 类型
    /// builder.RegisterAMapper(typeof(GameProfile), typeof(UIProfile));
    /// 
    /// // 使用泛型 Profile
    /// builder.RegisterAMapper&lt;GameProfile&gt;();
    /// 
    /// // 使用安装器
    /// builder.UseAMapperInstaller&lt;MyMapperInstaller&gt;();
    /// 
    /// // 自动扫描
    /// builder.UseAMapperAutoScan();
    /// </code>
    /// </remarks>
    public static class AMapperRegistrationExtensions
    {
        #region 基础注册方法 / Basic Registration Methods

        /// <summary>
        /// 注册 AMapper 到 DI 容器
        /// <para>Register AMapper to DI container</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="configure">配置委托 / Configuration delegate</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder RegisterAMapper(
            this IContainerBuilder builder,
            Action<IMapperConfigurationExpression> configure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            // 创建映射器配置
            var configuration = new MapperConfiguration(configure);

            // 注册配置（单例）
            builder.RegisterInstance<IMapperConfiguration>(configuration);

            // 注册映射器（单例）
            builder.RegisterFactory<IAMapper>(resolver =>
            {
                var config = resolver.Resolve<IMapperConfiguration>() as MapperConfiguration;
                return new Mapper(config, new ServiceProviderAdapter(resolver));
            }).Singleton();

            // 注册作用域映射器工厂（单例）
            builder.RegisterFactory<IScopedMapperFactory>(resolver =>
                new ScopedMapperFactory(resolver)).Singleton();

            return builder;
        }

        /// <summary>
        /// 注册 AMapper 到 DI 容器（使用 Profile 类型）
        /// <para>Register AMapper to DI container using Profile types</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="profileTypes">Profile 类型数组 / Profile type array</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder RegisterAMapper(
            this IContainerBuilder builder,
            params Type[] profileTypes)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return RegisterAMapper(builder, cfg =>
            {
                foreach (var profileType in profileTypes)
                {
                    if (!typeof(MappingProfile).IsAssignableFrom(profileType))
                    {
                        throw new ArgumentException(
                            $"类型 {profileType.Name} 必须继承自 MappingProfile / " +
                            $"Type {profileType.Name} must inherit from MappingProfile");
                    }

                    var profile = (MappingProfile)Activator.CreateInstance(profileType);
                    cfg.AddProfile(profile);
                }
            });
        }

        /// <summary>
        /// 注册 AMapper 到 DI 容器（使用泛型 Profile）
        /// <para>Register AMapper to DI container using generic Profile</para>
        /// </summary>
        /// <typeparam name="TProfile">Profile 类型 / Profile type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder RegisterAMapper<TProfile>(this IContainerBuilder builder)
            where TProfile : MappingProfile, new()
        {
            return RegisterAMapper(builder, typeof(TProfile));
        }

        #endregion

        #region 安装器注册方法 / Installer Registration Methods

        /// <summary>
        /// 使用 AMapper 安装器
        /// <para>Use AMapper installer</para>
        /// </summary>
        /// <typeparam name="TInstaller">安装器类型 / Installer type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder UseAMapperInstaller<TInstaller>(this IContainerBuilder builder)
            where TInstaller : AMapperInstaller, new()
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseInstaller<TInstaller>();
        }

        /// <summary>
        /// 使用 AMapper 安装器实例
        /// <para>Use AMapper installer instance</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="installer">安装器实例 / Installer instance</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder UseAMapperInstaller(
            this IContainerBuilder builder,
            AMapperInstaller installer)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (installer == null)
                throw new ArgumentNullException(nameof(installer));

            return builder.UseInstaller(installer);
        }

        #endregion

        #region 自动扫描方法 / Auto-Scan Methods

        /// <summary>
        /// 使用自动扫描注册 AMapper
        /// <para>Use auto-scan to register AMapper</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>容器构建器 / Container builder</returns>
        /// <remarks>
        /// 自动扫描调用程序集中的所有 MappingProfile。
        /// </remarks>
        public static IContainerBuilder UseAMapperAutoScan(this IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseInstaller(AutoScanInstaller.FromCallingAssembly());
        }

        /// <summary>
        /// 使用自动扫描注册 AMapper（扫描指定类型所在程序集）
        /// <para>Use auto-scan to register AMapper (scan assembly containing specified type)</para>
        /// </summary>
        /// <typeparam name="T">类型标记 / Type marker</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder UseAMapperAutoScan<T>(this IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseInstaller(AutoScanInstaller.FromAssemblyOf<T>());
        }

        /// <summary>
        /// 使用自动扫描注册 AMapper（扫描所有已加载程序集）
        /// <para>Use auto-scan to register AMapper (scan all loaded assemblies)</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>容器构建器 / Container builder</returns>
        /// <remarks>
        /// 警告：扫描所有程序集可能影响启动性能，建议仅在开发环境使用。
        /// </remarks>
        public static IContainerBuilder UseAMapperAutoScanAll(this IContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseInstaller(AutoScanInstaller.FromAllAssemblies());
        }

        #endregion
    }
}
