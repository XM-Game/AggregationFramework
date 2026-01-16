// ==========================================================
// 文件名：IInstanceProvider.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义实例提供者接口，负责服务实例的创建和生命周期管理
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 实例提供者接口
    /// <para>定义服务实例的创建和管理策略，不同的生命周期使用不同的提供者实现</para>
    /// <para>Instance provider interface that defines creation and management strategy for service instances</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>策略模式：不同生命周期对应不同的提供者实现</item>
    /// <item>单一职责：仅负责实例的创建和缓存管理</item>
    /// <item>开闭原则：可通过实现此接口扩展新的生命周期策略</item>
    /// </list>
    /// 
    /// 内置实现：
    /// <list type="bullet">
    /// <item>SingletonProvider：单例提供者，容器级别缓存</item>
    /// <item>ScopedProvider：作用域提供者，作用域级别缓存</item>
    /// <item>TransientProvider：瞬态提供者，每次创建新实例</item>
    /// <item>FactoryProvider：工厂提供者，使用工厂函数创建</item>
    /// <item>ExistingInstanceProvider：现有实例提供者，返回预设实例</item>
    /// </list>
    /// </remarks>
    public interface IInstanceProvider
    {
        #region 实例获取 / Instance Retrieval

        /// <summary>
        /// 获取服务实例
        /// <para>Get a service instance</para>
        /// </summary>
        /// <param name="resolver">对象解析器，用于解析依赖 / Object resolver for resolving dependencies</param>
        /// <returns>服务实例 / Service instance</returns>
        /// <remarks>
        /// 根据提供者的实现策略：
        /// <list type="bullet">
        /// <item>SingletonProvider：首次调用创建实例，后续返回缓存</item>
        /// <item>ScopedProvider：在当前作用域首次调用创建实例，后续返回缓存</item>
        /// <item>TransientProvider：每次调用都创建新实例</item>
        /// </list>
        /// </remarks>
        object GetInstance(IObjectResolver resolver);

        /// <summary>
        /// 获取服务实例，使用指定的注入参数
        /// <para>Get a service instance with specified injection parameters</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">注入参数 / Injection parameters</param>
        /// <returns>服务实例 / Service instance</returns>
        object GetInstance(IObjectResolver resolver, System.Collections.Generic.IReadOnlyList<IInjectParameter> parameters);

        #endregion

        #region 实例释放 / Instance Disposal

        /// <summary>
        /// 释放服务实例
        /// <para>Dispose a service instance</para>
        /// </summary>
        /// <param name="instance">要释放的实例 / Instance to dispose</param>
        /// <remarks>
        /// 如果实例实现了 IDisposable，此方法会调用其 Dispose 方法。
        /// 对于 Transient 实例，调用者负责调用此方法。
        /// 对于 Singleton 和 Scoped 实例，容器/作用域销毁时自动调用。
        /// </remarks>
        void DisposeInstance(object instance);

        #endregion

        #region 提供者信息 / Provider Information

        /// <summary>
        /// 获取提供者管理的生命周期类型
        /// <para>Get the lifetime type managed by this provider</para>
        /// </summary>
        Lifetime Lifetime { get; }

        /// <summary>
        /// 获取提供者创建的实例类型
        /// <para>Get the instance type created by this provider</para>
        /// </summary>
        Type InstanceType { get; }

        /// <summary>
        /// 检查是否已创建实例（仅对 Singleton 和 Scoped 有意义）
        /// <para>Check if an instance has been created (only meaningful for Singleton and Scoped)</para>
        /// </summary>
        bool HasInstance { get; }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取提供者的诊断信息
        /// <para>Get diagnostic information of the provider</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        string GetDiagnosticInfo();

        #endregion
    }
}
