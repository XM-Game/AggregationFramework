// ==========================================================
// 文件名：IRegistration.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义服务注册信息接口，描述服务的类型、生命周期和提供者

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 注册信息接口
    /// <para>描述一个服务注册的完整信息，包括服务类型、实现类型、生命周期和实例提供者</para>
    /// <para>Registration interface that describes complete information of a service registration</para>
    /// </summary>
    /// <remarks>
    /// 注册信息是不可变的，一旦创建就不能修改。
    /// 这确保了容器构建后注册配置的稳定性。
    /// </remarks>
    public interface IRegistration
    {
        #region 类型信息 / Type Information

        /// <summary>
        /// 获取服务类型（接口或抽象类型）
        /// <para>Get the service type (interface or abstract type)</para>
        /// </summary>
        /// <remarks>
        /// 这是消费者请求解析时使用的类型。
        /// 例如：IService、IRepository&lt;T&gt; 等。
        /// </remarks>
        Type ServiceType { get; }

        /// <summary>
        /// 获取实现类型（具体类型）
        /// <para>Get the implementation type (concrete type)</para>
        /// </summary>
        /// <remarks>
        /// 这是实际创建实例时使用的类型。
        /// 对于工厂注册，此属性可能为 null。
        /// </remarks>
        Type ImplementationType { get; }

        /// <summary>
        /// 获取所有注册的服务类型
        /// <para>Get all registered service types</para>
        /// </summary>
        /// <remarks>
        /// 当使用 AsImplementedInterfaces() 时，一个注册可能对应多个服务类型。
        /// </remarks>
        IReadOnlyList<Type> ServiceTypes { get; }

        #endregion

        #region 生命周期 / Lifetime

        /// <summary>
        /// 获取生命周期类型
        /// <para>Get the lifetime type</para>
        /// </summary>
        Lifetime Lifetime { get; }

        #endregion

        #region 键值 / Key

        /// <summary>
        /// 获取注册键值（如果有）
        /// <para>Get the registration key if any</para>
        /// </summary>
        /// <remarks>
        /// 键值用于区分同一服务类型的多个注册。
        /// 如果未指定键值，此属性为 null。
        /// </remarks>
        object Key { get; }

        /// <summary>
        /// 检查是否有键值
        /// <para>Check if this registration has a key</para>
        /// </summary>
        bool HasKey { get; }

        #endregion

        #region 实例提供者 / Instance Provider

        /// <summary>
        /// 获取实例提供者
        /// <para>Get the instance provider</para>
        /// </summary>
        /// <remarks>
        /// 实例提供者负责创建和管理服务实例。
        /// 不同的生命周期使用不同的提供者实现。
        /// </remarks>
        IInstanceProvider Provider { get; }

        #endregion

        #region 注入参数 / Injection Parameters

        /// <summary>
        /// 获取注入参数列表
        /// <para>Get the list of injection parameters</para>
        /// </summary>
        /// <remarks>
        /// 注入参数用于在创建实例时提供特定的参数值。
        /// </remarks>
        IReadOnlyList<IInjectParameter> Parameters { get; }

        #endregion

        #region 元数据 / Metadata

        /// <summary>
        /// 获取注册的元数据
        /// <para>Get the registration metadata</para>
        /// </summary>
        /// <remarks>
        /// 元数据可用于存储自定义信息，如注册来源、描述等。
        /// </remarks>
        IReadOnlyDictionary<string, object> Metadata { get; }

        /// <summary>
        /// 获取指定键的元数据值
        /// <para>Get metadata value by key</para>
        /// </summary>
        /// <typeparam name="T">元数据值类型 / Metadata value type</typeparam>
        /// <param name="key">元数据键 / Metadata key</param>
        /// <returns>元数据值 / Metadata value</returns>
        T GetMetadata<T>(string key);

        /// <summary>
        /// 尝试获取指定键的元数据值
        /// <para>Try to get metadata value by key</para>
        /// </summary>
        /// <typeparam name="T">元数据值类型 / Metadata value type</typeparam>
        /// <param name="key">元数据键 / Metadata key</param>
        /// <param name="value">输出的元数据值 / Output metadata value</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        bool TryGetMetadata<T>(string key, out T value);

        #endregion
    }
}
