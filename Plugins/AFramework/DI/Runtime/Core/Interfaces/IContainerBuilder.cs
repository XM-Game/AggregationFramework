// ==========================================================
// 文件名：IContainerBuilder.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义容器构建器接口，提供服务注册和容器构建能力
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 容器构建器接口
    /// <para>定义依赖注入容器的构建能力，包括服务注册、安装器使用和容器构建</para>
    /// <para>Container builder interface that defines registration and building capabilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>建造者模式：通过链式调用配置容器</item>
    /// <item>组合根模式：所有依赖注册集中在此处</item>
    /// <item>开闭原则：通过安装器扩展注册逻辑</item>
    /// </list>
    /// </remarks>
    public interface IContainerBuilder
    {
        #region 类型注册 / Type Registration

        /// <summary>
        /// 注册指定类型
        /// <para>Register a type</para>
        /// </summary>
        /// <typeparam name="T">要注册的类型 / Type to register</typeparam>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder Register<T>();

        /// <summary>
        /// 注册接口到实现类型的映射
        /// <para>Register an interface to implementation mapping</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TImplementation">实现类型 / Implementation type</typeparam>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder Register<TInterface, TImplementation>() where TImplementation : TInterface;

        /// <summary>
        /// 注册指定类型（非泛型版本）
        /// <para>Register a type (non-generic version)</para>
        /// </summary>
        /// <param name="implementationType">实现类型 / Implementation type</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder Register(Type implementationType);

        /// <summary>
        /// 注册接口到实现类型的映射（非泛型版本）
        /// <para>Register an interface to implementation mapping (non-generic version)</para>
        /// </summary>
        /// <param name="interfaceType">接口类型 / Interface type</param>
        /// <param name="implementationType">实现类型 / Implementation type</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder Register(Type interfaceType, Type implementationType);

        #endregion

        #region 实例注册 / Instance Registration

        /// <summary>
        /// 注册现有实例
        /// <para>Register an existing instance</para>
        /// </summary>
        /// <typeparam name="T">实例类型 / Instance type</typeparam>
        /// <param name="instance">要注册的实例 / Instance to register</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        /// <remarks>
        /// 注册的实例默认为 Singleton 生命周期。
        /// 容器销毁时，如果实例实现了 IDisposable，将自动调用 Dispose。
        /// </remarks>
        IRegistrationBuilder RegisterInstance<T>(T instance) where T : class;

        /// <summary>
        /// 注册现有实例（非泛型版本）
        /// <para>Register an existing instance (non-generic version)</para>
        /// </summary>
        /// <param name="interfaceType">接口类型 / Interface type</param>
        /// <param name="instance">要注册的实例 / Instance to register</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder RegisterInstance(Type interfaceType, object instance);

        #endregion

        #region 工厂注册 / Factory Registration

        /// <summary>
        /// 注册工厂函数
        /// <para>Register a factory function</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="factory">工厂函数 / Factory function</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        /// <remarks>
        /// 工厂函数接收 IObjectResolver 参数，可用于解析其他依赖。
        /// </remarks>
        IRegistrationBuilder RegisterFactory<T>(Func<IObjectResolver, T> factory) where T : class;

        /// <summary>
        /// 注册工厂函数（非泛型版本）
        /// <para>Register a factory function (non-generic version)</para>
        /// </summary>
        /// <param name="interfaceType">接口类型 / Interface type</param>
        /// <param name="factory">工厂函数 / Factory function</param>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder RegisterFactory(Type interfaceType, Func<IObjectResolver, object> factory);

        #endregion

        #region 安装器 / Installers

        /// <summary>
        /// 使用安装器注册服务
        /// <para>Use an installer to register services</para>
        /// </summary>
        /// <typeparam name="TInstaller">安装器类型 / Installer type</typeparam>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IContainerBuilder UseInstaller<TInstaller>() where TInstaller : IInstaller, new();

        /// <summary>
        /// 使用安装器实例注册服务
        /// <para>Use an installer instance to register services</para>
        /// </summary>
        /// <param name="installer">安装器实例 / Installer instance</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IContainerBuilder UseInstaller(IInstaller installer);

        /// <summary>
        /// 使用多个安装器注册服务
        /// <para>Use multiple installers to register services</para>
        /// </summary>
        /// <param name="installers">安装器集合 / Collection of installers</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IContainerBuilder UseInstallers(IEnumerable<IInstaller> installers);

        #endregion

        #region 条件注册 / Conditional Registration

        /// <summary>
        /// 仅当类型未注册时才注册
        /// <para>Register only if the type is not already registered</para>
        /// </summary>
        /// <typeparam name="T">要注册的类型 / Type to register</typeparam>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder RegisterIfNotRegistered<T>();

        /// <summary>
        /// 仅当接口未注册时才注册
        /// <para>Register only if the interface is not already registered</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TImplementation">实现类型 / Implementation type</typeparam>
        /// <returns>注册构建器，用于进一步配置 / Registration builder for further configuration</returns>
        IRegistrationBuilder RegisterIfNotRegistered<TInterface, TImplementation>() where TImplementation : TInterface;

        #endregion

        #region 构建 / Build

        /// <summary>
        /// 构建容器
        /// <para>Build the container</para>
        /// </summary>
        /// <returns>构建完成的对象解析器 / Built object resolver</returns>
        /// <exception cref="RegistrationException">当注册配置无效时抛出 / Thrown when registration configuration is invalid</exception>
        /// <remarks>
        /// 构建过程会：
        /// <list type="bullet">
        /// <item>验证所有注册配置</item>
        /// <item>检测循环依赖</item>
        /// <item>创建容器实例</item>
        /// <item>执行入口点初始化</item>
        /// </list>
        /// </remarks>
        IObjectResolver Build();

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取所有注册信息
        /// <para>Get all registrations</para>
        /// </summary>
        /// <returns>注册信息集合 / Collection of registrations</returns>
        IReadOnlyList<IRegistration> GetRegistrations();

        /// <summary>
        /// 检查指定类型是否已注册
        /// <para>Check if the specified type is registered</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <returns>是否已注册 / Whether registered</returns>
        bool IsRegistered<T>();

        /// <summary>
        /// 检查指定类型是否已注册（非泛型版本）
        /// <para>Check if the specified type is registered (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>是否已注册 / Whether registered</returns>
        bool IsRegistered(Type type);

        #endregion

        #region 配置 / Configuration

        /// <summary>
        /// 设置父容器
        /// <para>Set the parent container</para>
        /// </summary>
        /// <param name="parent">父容器 / Parent container</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IContainerBuilder SetParent(IObjectResolver parent);

        /// <summary>
        /// 启用生命周期验证
        /// <para>Enable lifetime validation</para>
        /// </summary>
        /// <param name="enabled">是否启用 / Whether to enable</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        /// <remarks>
        /// 启用后会在构建时检测生命周期配置问题，如 Singleton 依赖 Scoped。
        /// 建议在开发环境启用，生产环境可禁用以提升性能。
        /// </remarks>
        IContainerBuilder EnableLifetimeValidation(bool enabled = true);

        /// <summary>
        /// 启用诊断信息收集
        /// <para>Enable diagnostics collection</para>
        /// </summary>
        /// <param name="enabled">是否启用 / Whether to enable</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IContainerBuilder EnableDiagnostics(bool enabled = true);

        #endregion
    }
}
