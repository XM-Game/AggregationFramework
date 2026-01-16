// ==========================================================
// 文件名：OpenGenericProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Collections.Concurrent
// 功能: 开放泛型实例提供者，支持运行时构造封闭泛型类型
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 开放泛型实例提供者
    /// <para>支持开放泛型类型注册，运行时自动构造封闭泛型类型</para>
    /// <para>Open generic instance provider that constructs closed generic types at runtime</para>
    /// </summary>
    /// <remarks>
    /// 使用示例：
    /// <code>
    /// // 注册开放泛型
    /// builder.RegisterOpenGeneric(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;));
    /// 
    /// // 解析时自动构造封闭类型
    /// var userRepo = container.Resolve&lt;IRepository&lt;User&gt;&gt;();
    /// var orderRepo = container.Resolve&lt;IRepository&lt;Order&gt;&gt;();
    /// </code>
    /// </remarks>
    internal sealed class OpenGenericProvider : IInstanceProvider
    {
        #region 字段 / Fields

        private readonly Type _openGenericServiceType;
        private readonly Type _openGenericImplementationType;
        private readonly Lifetime _lifetime;
        
        // 缓存已构造的封闭泛型注册
        private readonly ConcurrentDictionary<Type, Registration> _closedRegistrations;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建开放泛型提供者
        /// </summary>
        /// <param name="openGenericServiceType">开放泛型服务类型 / Open generic service type</param>
        /// <param name="openGenericImplementationType">开放泛型实现类型 / Open generic implementation type</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        public OpenGenericProvider(
            Type openGenericServiceType,
            Type openGenericImplementationType,
            Lifetime lifetime)
        {
            if (openGenericServiceType == null)
                throw new ArgumentNullException(nameof(openGenericServiceType));
            if (openGenericImplementationType == null)
                throw new ArgumentNullException(nameof(openGenericImplementationType));

            if (!openGenericServiceType.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"类型 {openGenericServiceType.Name} 不是开放泛型类型。\n" +
                    $"Type {openGenericServiceType.Name} is not an open generic type.",
                    nameof(openGenericServiceType));

            if (!openGenericImplementationType.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"类型 {openGenericImplementationType.Name} 不是开放泛型类型。\n" +
                    $"Type {openGenericImplementationType.Name} is not an open generic type.",
                    nameof(openGenericImplementationType));

            _openGenericServiceType = openGenericServiceType;
            _openGenericImplementationType = openGenericImplementationType;
            _lifetime = lifetime;
            _closedRegistrations = new ConcurrentDictionary<Type, Registration>();
        }

        #endregion

        #region IInstanceProvider 实现 / IInstanceProvider Implementation

        /// <inheritdoc/>
        public Lifetime Lifetime => _lifetime;

        /// <inheritdoc/>
        public Type InstanceType => _openGenericImplementationType;

        /// <inheritdoc/>
        public bool HasInstance => false;

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver)
        {
            throw new InvalidOperationException(
                "开放泛型提供者不能直接获取实例，请使用 GetClosedRegistration 方法。\n" +
                "Open generic provider cannot get instance directly, use GetClosedRegistration method.");
        }

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            throw new InvalidOperationException(
                "开放泛型提供者不能直接获取实例，请使用 GetClosedRegistration 方法。\n" +
                "Open generic provider cannot get instance directly, use GetClosedRegistration method.");
        }

        /// <inheritdoc/>
        public void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <inheritdoc/>
        public string GetDiagnosticInfo()
        {
            return $"OpenGenericProvider[Service={_openGenericServiceType.Name}, " +
                   $"Implementation={_openGenericImplementationType.Name}, " +
                   $"Lifetime={_lifetime}, " +
                   $"CachedClosedTypes={_closedRegistrations.Count}]";
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取封闭泛型类型的注册
        /// <para>Get registration for closed generic type</para>
        /// </summary>
        /// <param name="closedServiceType">封闭泛型服务类型 / Closed generic service type</param>
        /// <param name="typeArguments">类型参数 / Type arguments</param>
        /// <returns>封闭泛型注册 / Closed generic registration</returns>
        public Registration GetClosedRegistration(Type closedServiceType, Type[] typeArguments)
        {
            return _closedRegistrations.GetOrAdd(closedServiceType, _ =>
            {
                // 构造封闭泛型实现类型 / Construct closed generic implementation type
                var closedImplementationType = _openGenericImplementationType.MakeGenericType(typeArguments);

                // 创建新的注册 / Create new registration
                // 参数顺序: serviceType, implementationType, lifetime, key, factory, existingInstance, 
                //          serviceTypes, parameters, metadata, onActivatedCallbacks
                var registration = new Registration(
                    closedServiceType,           // serviceType
                    closedImplementationType,    // implementationType
                    _lifetime,                   // lifetime
                    null,                        // key
                    null,                        // factory
                    null,                        // existingInstance
                    new[] { closedServiceType }, // serviceTypes
                    null,                        // parameters
                    null,                        // metadata
                    null);                       // onActivatedCallbacks

                return registration;
            });
        }

        /// <summary>
        /// 检查是否可以处理指定的封闭泛型类型
        /// <para>Check if can handle the specified closed generic type</para>
        /// </summary>
        /// <param name="closedServiceType">封闭泛型服务类型 / Closed generic service type</param>
        /// <returns>是否可以处理 / Whether can handle</returns>
        public bool CanHandle(Type closedServiceType)
        {
            if (!closedServiceType.IsGenericType)
                return false;

            var genericDefinition = closedServiceType.GetGenericTypeDefinition();
            return genericDefinition == _openGenericServiceType;
        }

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取开放泛型服务类型
        /// </summary>
        public Type OpenGenericServiceType => _openGenericServiceType;

        /// <summary>
        /// 获取开放泛型实现类型
        /// </summary>
        public Type OpenGenericImplementationType => _openGenericImplementationType;

        #endregion
    }
}
