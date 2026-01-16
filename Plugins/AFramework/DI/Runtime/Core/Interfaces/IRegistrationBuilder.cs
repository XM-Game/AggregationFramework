// ==========================================================
// 文件名：IRegistrationBuilder.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义注册构建器接口，提供链式配置注册信息的能力

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 注册构建器接口
    /// <para>提供链式 API 配置服务注册的各项属性，包括服务类型映射、生命周期和参数注入</para>
    /// <para>Registration builder interface that provides fluent API for configuring service registration</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>建造者模式：通过链式调用逐步配置注册信息</item>
    /// <item>流畅接口：所有配置方法返回 this，支持链式调用</item>
    /// <item>不可变性：每次调用返回新的构建器状态</item>
    /// </list>
    /// </remarks>
    public interface IRegistrationBuilder
    {
        #region 服务类型映射 / Service Type Mapping

        /// <summary>
        /// 将注册映射到指定的服务类型
        /// <para>Map the registration to the specified service type</para>
        /// </summary>
        /// <typeparam name="TInterface">服务类型 / Service type</typeparam>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder As<TInterface>();

        /// <summary>
        /// 将注册映射到指定的服务类型（非泛型版本）
        /// <para>Map the registration to the specified service type (non-generic version)</para>
        /// </summary>
        /// <param name="interfaceType">服务类型 / Service type</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder As(Type interfaceType);

        /// <summary>
        /// 将注册映射到实现类型自身
        /// <para>Map the registration to the implementation type itself</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder AsSelf();

        /// <summary>
        /// 将注册映射到实现类型实现的所有接口
        /// <para>Map the registration to all interfaces implemented by the implementation type</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        /// <remarks>
        /// 此方法会自动发现实现类型的所有接口，并为每个接口创建映射。
        /// 不包括系统接口（如 IDisposable）。
        /// </remarks>
        IRegistrationBuilder AsImplementedInterfaces();

        /// <summary>
        /// 将注册映射到实现类型自身及其所有接口
        /// <para>Map the registration to self and all implemented interfaces</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder AsSelfAndImplementedInterfaces();

        #endregion

        #region 生命周期配置 / Lifetime Configuration

        /// <summary>
        /// 设置为单例生命周期
        /// <para>Set the lifetime to Singleton</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        /// <remarks>
        /// 单例：容器生命周期内只创建一个实例，所有解析请求返回同一实例。
        /// </remarks>
        IRegistrationBuilder Singleton();

        /// <summary>
        /// 设置为作用域生命周期
        /// <para>Set the lifetime to Scoped</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        /// <remarks>
        /// 作用域：每个作用域内只创建一个实例，不同作用域拥有不同实例。
        /// </remarks>
        IRegistrationBuilder Scoped();

        /// <summary>
        /// 设置为瞬态生命周期
        /// <para>Set the lifetime to Transient</para>
        /// </summary>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        /// <remarks>
        /// 瞬态：每次解析都创建新实例，容器不跟踪实例生命周期。
        /// </remarks>
        IRegistrationBuilder Transient();

        /// <summary>
        /// 设置生命周期类型
        /// <para>Set the lifetime type</para>
        /// </summary>
        /// <param name="lifetime">生命周期类型 / Lifetime type</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithLifetime(Lifetime lifetime);

        #endregion

        #region 参数注入 / Parameter Injection

        /// <summary>
        /// 添加按名称匹配的参数
        /// <para>Add a parameter matched by name</para>
        /// </summary>
        /// <param name="name">参数名称 / Parameter name</param>
        /// <param name="value">参数值 / Parameter value</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter(string name, object value);

        /// <summary>
        /// 添加按类型匹配的参数
        /// <para>Add a parameter matched by type</para>
        /// </summary>
        /// <typeparam name="TParam">参数类型 / Parameter type</typeparam>
        /// <param name="value">参数值 / Parameter value</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter<TParam>(TParam value);

        /// <summary>
        /// 添加按类型匹配的参数（非泛型版本）
        /// <para>Add a parameter matched by type (non-generic version)</para>
        /// </summary>
        /// <param name="type">参数类型 / Parameter type</param>
        /// <param name="value">参数值 / Parameter value</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter(Type type, object value);

        /// <summary>
        /// 添加工厂函数参数
        /// <para>Add a factory function parameter</para>
        /// </summary>
        /// <typeparam name="TParam">参数类型 / Parameter type</typeparam>
        /// <param name="factory">工厂函数 / Factory function</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter<TParam>(Func<IObjectResolver, TParam> factory);

        /// <summary>
        /// 添加按名称匹配的工厂函数参数
        /// <para>Add a factory function parameter matched by name</para>
        /// </summary>
        /// <param name="name">参数名称 / Parameter name</param>
        /// <param name="factory">工厂函数 / Factory function</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> factory);

        /// <summary>
        /// 添加注入参数
        /// <para>Add an injection parameter</para>
        /// </summary>
        /// <param name="parameter">注入参数 / Injection parameter</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithParameter(IInjectParameter parameter);

        #endregion

        #region 键值注册 / Keyed Registration

        /// <summary>
        /// 设置注册键值
        /// <para>Set the registration key</para>
        /// </summary>
        /// <param name="key">键值 / Key</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder Keyed(object key);

        /// <summary>
        /// 设置注册键值（使用枚举）
        /// <para>Set the registration key using enum</para>
        /// </summary>
        /// <typeparam name="TKey">枚举类型 / Enum type</typeparam>
        /// <param name="key">枚举键值 / Enum key</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder Keyed<TKey>(TKey key) where TKey : Enum;

        #endregion

        #region 元数据 / Metadata

        /// <summary>
        /// 添加元数据
        /// <para>Add metadata</para>
        /// </summary>
        /// <param name="key">元数据键 / Metadata key</param>
        /// <param name="value">元数据值 / Metadata value</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder WithMetadata(string key, object value);

        #endregion

        #region 回调 / Callbacks

        /// <summary>
        /// 添加实例创建后的回调
        /// <para>Add a callback after instance creation</para>
        /// </summary>
        /// <param name="callback">回调委托 / Callback delegate</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder OnActivated(Action<object> callback);

        /// <summary>
        /// 添加实例创建后的回调（泛型版本）
        /// <para>Add a callback after instance creation (generic version)</para>
        /// </summary>
        /// <typeparam name="T">实例类型 / Instance type</typeparam>
        /// <param name="callback">回调委托 / Callback delegate</param>
        /// <returns>当前构建器实例，支持链式调用 / Current builder instance for chaining</returns>
        IRegistrationBuilder OnActivated<T>(Action<T> callback);

        #endregion

        #region 构建 / Build

        /// <summary>
        /// 构建注册信息
        /// <para>Build the registration</para>
        /// </summary>
        /// <returns>构建完成的注册信息 / Built registration</returns>
        IRegistration Build();

        #endregion
    }
}
