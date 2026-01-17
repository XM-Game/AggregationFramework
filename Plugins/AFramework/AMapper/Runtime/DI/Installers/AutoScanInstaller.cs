// ==========================================================
// 文件名：AutoScanInstaller.cs
// 命名空间: AFramework.AMapper.DI
// 依赖: System, System.Reflection, AFramework.DI
// 功能: 自动扫描安装器，自动发现并注册 MappingProfile
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AFramework.DI;

namespace AFramework.AMapper.DI
{
    /// <summary>
    /// 自动扫描安装器
    /// <para>自动扫描程序集，发现并注册所有 MappingProfile</para>
    /// <para>Auto-scan installer that automatically discovers and registers all MappingProfiles</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>约定优于配置：自动发现 Profile，减少手动配置</item>
    /// <item>单一职责：仅负责 Profile 的自动扫描和注册</item>
    /// <item>性能优化：支持指定程序集范围，避免全局扫描</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 扫描当前程序集
    /// builder.UseInstaller(new AutoScanInstaller());
    /// 
    /// // 扫描指定程序集
    /// builder.UseInstaller(new AutoScanInstaller(typeof(MyProfile).Assembly));
    /// 
    /// // 扫描多个程序集
    /// builder.UseInstaller(new AutoScanInstaller(
    ///     typeof(Profile1).Assembly,
    ///     typeof(Profile2).Assembly));
    /// </code>
    /// 
    /// 注意事项：
    /// <list type="bullet">
    /// <item>仅扫描公共的、非抽象的 MappingProfile 子类</item>
    /// <item>Profile 必须有无参构造函数</item>
    /// <item>扫描在安装时执行，不会影响运行时性能</item>
    /// </list>
    /// </remarks>
    public class AutoScanInstaller : AMapperInstaller
    {
        #region 私有字段 / Private Fields

        private readonly Assembly[] _assembliesToScan;
        private readonly Func<Type, bool> _profileFilter;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建自动扫描安装器（扫描调用程序集）
        /// <para>Create auto-scan installer (scan calling assembly)</para>
        /// </summary>
        public AutoScanInstaller()
            : this(Assembly.GetCallingAssembly())
        {
        }

        /// <summary>
        /// 创建自动扫描安装器（扫描指定程序集）
        /// <para>Create auto-scan installer (scan specified assemblies)</para>
        /// </summary>
        /// <param name="assembliesToScan">要扫描的程序集 / Assemblies to scan</param>
        public AutoScanInstaller(params Assembly[] assembliesToScan)
            : this(null, assembliesToScan)
        {
        }

        /// <summary>
        /// 创建自动扫描安装器（扫描指定程序集，使用自定义过滤器）
        /// <para>Create auto-scan installer (scan specified assemblies with custom filter)</para>
        /// </summary>
        /// <param name="profileFilter">Profile 过滤器 / Profile filter</param>
        /// <param name="assembliesToScan">要扫描的程序集 / Assemblies to scan</param>
        public AutoScanInstaller(Func<Type, bool> profileFilter, params Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan != null && assembliesToScan.Length > 0
                ? assembliesToScan
                : new[] { Assembly.GetCallingAssembly() };

            _profileFilter = profileFilter ?? DefaultProfileFilter;
        }

        #endregion

        #region 受保护方法 / Protected Methods

        /// <summary>
        /// 配置映射器
        /// <para>Configure mapper</para>
        /// </summary>
        /// <param name="cfg">映射器配置表达式 / Mapper configuration expression</param>
        protected override void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));

            // 扫描并注册所有 Profile
            var profiles = ScanProfiles();
            foreach (var profile in profiles)
            {
                cfg.AddProfile(profile);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 扫描 Profile
        /// <para>Scan profiles</para>
        /// </summary>
        /// <returns>Profile 实例列表 / List of profile instances</returns>
        private List<MappingProfile> ScanProfiles()
        {
            var profiles = new List<MappingProfile>();

            foreach (var assembly in _assembliesToScan)
            {
                try
                {
                    // 获取程序集中的所有类型
                    var types = assembly.GetTypes();

                    // 筛选 MappingProfile 子类
                    var profileTypes = types
                        .Where(t => typeof(MappingProfile).IsAssignableFrom(t))
                        .Where(t => !t.IsAbstract && t.IsClass)
                        .Where(t => t.IsPublic || t.IsNestedPublic)
                        .Where(_profileFilter);

                    // 创建 Profile 实例
                    foreach (var profileType in profileTypes)
                    {
                        try
                        {
                            var profile = (MappingProfile)Activator.CreateInstance(profileType);
                            profiles.Add(profile);
                        }
                        catch (Exception ex)
                        {
                            throw new AMapperException(
                                $"无法创建 Profile 实例：{profileType.FullName}。" +
                                $"确保 Profile 有公共的无参构造函数。/ " +
                                $"Failed to create profile instance: {profileType.FullName}. " +
                                $"Ensure the profile has a public parameterless constructor.",
                                ex);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // 处理类型加载异常
                    throw new AMapperException(
                        $"扫描程序集 {assembly.FullName} 时发生类型加载异常 / " +
                        $"Type load exception occurred while scanning assembly {assembly.FullName}",
                        ex);
                }
            }

            return profiles;
        }

        /// <summary>
        /// 默认 Profile 过滤器
        /// <para>Default profile filter</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否包含 / Whether to include</returns>
        private static bool DefaultProfileFilter(Type type)
        {
            // 排除泛型类型定义
            if (type.IsGenericTypeDefinition)
                return false;

            // 排除编译器生成的类型
            if (type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                return false;

            return true;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 创建扫描当前程序集的安装器
        /// <para>Create installer that scans current assembly</para>
        /// </summary>
        /// <returns>安装器实例 / Installer instance</returns>
        public static AutoScanInstaller FromCallingAssembly()
        {
            return new AutoScanInstaller(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// 创建扫描指定类型所在程序集的安装器
        /// <para>Create installer that scans assembly containing specified type</para>
        /// </summary>
        /// <typeparam name="T">类型标记 / Type marker</typeparam>
        /// <returns>安装器实例 / Installer instance</returns>
        public static AutoScanInstaller FromAssemblyOf<T>()
        {
            return new AutoScanInstaller(typeof(T).Assembly);
        }

        /// <summary>
        /// 创建扫描指定类型所在程序集的安装器
        /// <para>Create installer that scans assembly containing specified type</para>
        /// </summary>
        /// <param name="type">类型标记 / Type marker</param>
        /// <returns>安装器实例 / Installer instance</returns>
        public static AutoScanInstaller FromAssemblyOf(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new AutoScanInstaller(type.Assembly);
        }

        /// <summary>
        /// 创建扫描所有已加载程序集的安装器
        /// <para>Create installer that scans all loaded assemblies</para>
        /// </summary>
        /// <returns>安装器实例 / Installer instance</returns>
        /// <remarks>
        /// 警告：扫描所有程序集可能影响启动性能，建议仅在开发环境使用。
        /// </remarks>
        public static AutoScanInstaller FromAllAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return new AutoScanInstaller(assemblies);
        }

        #endregion
    }
}
