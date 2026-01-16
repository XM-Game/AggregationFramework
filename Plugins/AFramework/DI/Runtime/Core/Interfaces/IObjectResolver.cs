// ==========================================================
// 文件名：IObjectResolver.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 定义对象解析器的核心接口，提供依赖解析和实例化能力

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 对象解析器接口
    /// <para>定义依赖注入容器的核心解析能力，包括服务解析、实例注入和对象实例化</para>
    /// <para>Object resolver interface that defines core resolution capabilities of DI container</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责对象解析，不涉及注册逻辑</item>
    /// <item>接口隔离：提供细粒度的解析方法，消费者按需使用</item>
    /// <item>依赖倒置：高层模块依赖此抽象接口，而非具体容器实现</item>
    /// </list>
    /// </remarks>
    public interface IObjectResolver : IDisposable
    {
        #region 基础解析方法 / Basic Resolution Methods

        /// <summary>
        /// 解析指定类型的服务实例
        /// <para>Resolve a service instance of the specified type</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <returns>服务实例 / Service instance</returns>
        /// <exception cref="ResolutionException">当服务未注册或解析失败时抛出 / Thrown when service is not registered or resolution fails</exception>
        T Resolve<T>();

        /// <summary>
        /// 解析指定类型的服务实例（非泛型版本）
        /// <para>Resolve a service instance of the specified type (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>服务实例 / Service instance</returns>
        /// <exception cref="ArgumentNullException">当 type 为 null 时抛出 / Thrown when type is null</exception>
        /// <exception cref="ResolutionException">当服务未注册或解析失败时抛出 / Thrown when service is not registered or resolution fails</exception>
        object Resolve(Type type);

        #endregion

        #region 可选解析方法 / Optional Resolution Methods

        /// <summary>
        /// 解析指定类型的服务实例，如果未注册则返回默认值
        /// <para>Resolve a service instance, or return default if not registered</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <returns>服务实例或默认值 / Service instance or default value</returns>
        T ResolveOrDefault<T>();

        /// <summary>
        /// 解析指定类型的服务实例，如果未注册则返回默认值（非泛型版本）
        /// <para>Resolve a service instance, or return default if not registered (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>服务实例或 null / Service instance or null</returns>
        object ResolveOrDefault(Type type);

        /// <summary>
        /// 尝试解析指定类型的服务实例
        /// <para>Try to resolve a service instance of the specified type</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="instance">解析成功时输出服务实例 / Output service instance when resolution succeeds</param>
        /// <returns>解析是否成功 / Whether resolution succeeded</returns>
        bool TryResolve<T>(out T instance);

        /// <summary>
        /// 尝试解析指定类型的服务实例（非泛型版本）
        /// <para>Try to resolve a service instance of the specified type (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="instance">解析成功时输出服务实例 / Output service instance when resolution succeeds</param>
        /// <returns>解析是否成功 / Whether resolution succeeded</returns>
        bool TryResolve(Type type, out object instance);

        #endregion

        #region 集合解析方法 / Collection Resolution Methods

        /// <summary>
        /// 解析指定类型的所有服务实例
        /// <para>Resolve all service instances of the specified type</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <returns>服务实例集合 / Collection of service instances</returns>
        /// <remarks>
        /// 当存在多个相同类型的注册时，返回所有实例。
        /// 如果没有注册，返回空集合而非抛出异常。
        /// </remarks>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// 解析指定类型的所有服务实例（非泛型版本）
        /// <para>Resolve all service instances of the specified type (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>服务实例集合 / Collection of service instances</returns>
        IEnumerable<object> ResolveAll(Type type);

        #endregion

        #region 键值解析方法 / Keyed Resolution Methods

        /// <summary>
        /// 根据键值解析指定类型的服务实例
        /// <para>Resolve a service instance by key</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="key">注册键值 / Registration key</param>
        /// <returns>服务实例 / Service instance</returns>
        /// <exception cref="ResolutionException">当指定键值的服务未注册时抛出 / Thrown when keyed service is not registered</exception>
        T ResolveKeyed<T>(object key);

        /// <summary>
        /// 根据键值解析指定类型的服务实例（非泛型版本）
        /// <para>Resolve a service instance by key (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">注册键值 / Registration key</param>
        /// <returns>服务实例 / Service instance</returns>
        object ResolveKeyed(Type type, object key);

        /// <summary>
        /// 尝试根据键值解析指定类型的服务实例
        /// <para>Try to resolve a service instance by key</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="key">注册键值 / Registration key</param>
        /// <param name="instance">解析成功时输出服务实例 / Output service instance when resolution succeeds</param>
        /// <returns>解析是否成功 / Whether resolution succeeded</returns>
        bool TryResolveKeyed<T>(object key, out T instance);

        /// <summary>
        /// 尝试根据键值解析指定类型的服务实例（非泛型版本）
        /// <para>Try to resolve a service instance by key (non-generic version)</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">注册键值 / Registration key</param>
        /// <param name="instance">解析成功时输出服务实例 / Output service instance when resolution succeeds</param>
        /// <returns>解析是否成功 / Whether resolution succeeded</returns>
        bool TryResolveKeyed(Type type, object key, out object instance);

        #endregion

        #region 注入方法 / Injection Methods

        /// <summary>
        /// 向现有实例注入依赖
        /// <para>Inject dependencies into an existing instance</para>
        /// </summary>
        /// <param name="instance">要注入的实例 / Instance to inject into</param>
        /// <exception cref="ArgumentNullException">当 instance 为 null 时抛出 / Thrown when instance is null</exception>
        /// <remarks>
        /// 此方法会处理实例上所有标记了 [Inject] 特性的成员：
        /// <list type="bullet">
        /// <item>属性注入</item>
        /// <item>字段注入</item>
        /// <item>方法注入</item>
        /// </list>
        /// </remarks>
        void Inject(object instance);

        /// <summary>
        /// 向现有实例注入依赖，并指定额外的注入参数
        /// <para>Inject dependencies into an existing instance with additional parameters</para>
        /// </summary>
        /// <param name="instance">要注入的实例 / Instance to inject into</param>
        /// <param name="parameters">额外的注入参数 / Additional injection parameters</param>
        void Inject(object instance, IReadOnlyList<IInjectParameter> parameters);

        #endregion

        #region 实例化方法 / Instantiation Methods

        /// <summary>
        /// 创建指定类型的实例并注入依赖
        /// <para>Create an instance of the specified type and inject dependencies</para>
        /// </summary>
        /// <typeparam name="T">要创建的类型 / Type to create</typeparam>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        /// <remarks>
        /// 此方法用于创建未在容器中注册的类型实例。
        /// 构造函数参数会从容器中解析。
        /// </remarks>
        T Instantiate<T>();

        /// <summary>
        /// 创建指定类型的实例并注入依赖（非泛型版本）
        /// <para>Create an instance of the specified type and inject dependencies (non-generic version)</para>
        /// </summary>
        /// <param name="type">要创建的类型 / Type to create</param>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        object Instantiate(Type type);

        /// <summary>
        /// 创建指定类型的实例，使用额外参数并注入依赖
        /// <para>Create an instance with additional parameters and inject dependencies</para>
        /// </summary>
        /// <typeparam name="T">要创建的类型 / Type to create</typeparam>
        /// <param name="parameters">额外的构造参数 / Additional constructor parameters</param>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        T Instantiate<T>(IReadOnlyList<IInjectParameter> parameters);

        /// <summary>
        /// 创建指定类型的实例，使用额外参数并注入依赖（非泛型版本）
        /// <para>Create an instance with additional parameters and inject dependencies (non-generic version)</para>
        /// </summary>
        /// <param name="type">要创建的类型 / Type to create</param>
        /// <param name="parameters">额外的构造参数 / Additional constructor parameters</param>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        object Instantiate(Type type, IReadOnlyList<IInjectParameter> parameters);

        #endregion

        #region 作用域方法 / Scope Methods

        /// <summary>
        /// 创建子作用域
        /// <para>Create a child scope</para>
        /// </summary>
        /// <returns>子作用域解析器 / Child scope resolver</returns>
        /// <remarks>
        /// 子作用域继承父作用域的所有注册，但拥有独立的 Scoped 实例缓存。
        /// 子作用域销毁时，其 Scoped 实例也会被销毁。
        /// </remarks>
        IObjectResolver CreateScope();

        /// <summary>
        /// 创建子作用域，并使用指定的配置
        /// <para>Create a child scope with specified configuration</para>
        /// </summary>
        /// <param name="configuration">作用域配置委托 / Scope configuration delegate</param>
        /// <returns>子作用域解析器 / Child scope resolver</returns>
        IObjectResolver CreateScope(Action<IContainerBuilder> configuration);

        #endregion

        #region 容器信息 / Container Information

        /// <summary>
        /// 获取父容器（如果存在）
        /// <para>Get the parent container if exists</para>
        /// </summary>
        IObjectResolver Parent { get; }

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

        /// <summary>
        /// 检查指定键值的类型是否已注册
        /// <para>Check if the specified keyed type is registered</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="key">注册键值 / Registration key</param>
        /// <returns>是否已注册 / Whether registered</returns>
        bool IsRegisteredKeyed<T>(object key);

        #endregion
    }
}
