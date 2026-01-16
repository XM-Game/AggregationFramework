// ==========================================================
// 文件名：RegistrationBuilder.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 实现注册构建器，提供链式配置注册信息的能力

// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.DI
{
    /// <summary>
    /// 注册构建器实现类
    /// <para>提供链式 API 配置服务注册的各项属性</para>
    /// <para>Registration builder implementation that provides fluent API for configuration</para>
    /// </summary>
    public sealed class RegistrationBuilder : IRegistrationBuilder
    {
        #region 字段 / Fields

        private readonly Type _implementationType;
        private readonly List<Type> _serviceTypes = new List<Type>();
        private readonly List<IInjectParameter> _parameters = new List<IInjectParameter>();
        private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();
        private readonly List<Action<object>> _onActivatedCallbacks = new List<Action<object>>();
        
        private Lifetime _lifetime = Lifetime.Transient;
        private object _key;
        private Func<IObjectResolver, object> _factory;
        private object _existingInstance;
        private readonly ContainerBuilder _containerBuilder;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建注册构建器实例（类型注册）
        /// </summary>
        internal RegistrationBuilder(ContainerBuilder containerBuilder, Type implementationType)
        {
            _containerBuilder = containerBuilder;
            _implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        }

        /// <summary>
        /// 创建注册构建器实例（工厂注册）
        /// </summary>
        internal RegistrationBuilder(ContainerBuilder containerBuilder, Type serviceType, Func<IObjectResolver, object> factory)
        {
            _containerBuilder = containerBuilder;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _serviceTypes.Add(serviceType);
        }

        /// <summary>
        /// 创建注册构建器实例（实例注册）
        /// </summary>
        internal RegistrationBuilder(ContainerBuilder containerBuilder, Type serviceType, object instance)
        {
            _containerBuilder = containerBuilder;
            _existingInstance = instance ?? throw new ArgumentNullException(nameof(instance));
            _implementationType = instance.GetType();
            _serviceTypes.Add(serviceType);
            _lifetime = Lifetime.Singleton; // 实例注册默认为单例
        }

        #endregion

        #region 服务类型映射 / Service Type Mapping

        /// <inheritdoc/>
        public IRegistrationBuilder As<TInterface>()
        {
            return As(typeof(TInterface));
        }

        /// <inheritdoc/>
        public IRegistrationBuilder As(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            // 验证类型兼容性
            var implType = _implementationType ?? _existingInstance?.GetType();
            if (implType != null && !interfaceType.IsAssignableFrom(implType))
            {
                throw RegistrationException.TypeNotAssignable(interfaceType, implType);
            }

            if (!_serviceTypes.Contains(interfaceType))
            {
                _serviceTypes.Add(interfaceType);
            }
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder AsSelf()
        {
            var selfType = _implementationType ?? _existingInstance?.GetType();
            if (selfType != null && !_serviceTypes.Contains(selfType))
            {
                _serviceTypes.Add(selfType);
            }
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder AsImplementedInterfaces()
        {
            var implType = _implementationType ?? _existingInstance?.GetType();
            if (implType == null) return this;

            var interfaces = implType.GetInterfaces()
                .Where(i => !IsSystemInterface(i));

            foreach (var iface in interfaces)
            {
                if (!_serviceTypes.Contains(iface))
                {
                    _serviceTypes.Add(iface);
                }
            }
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder AsSelfAndImplementedInterfaces()
        {
            AsSelf();
            AsImplementedInterfaces();
            return this;
        }

        /// <summary>
        /// 检查是否为系统接口（不应自动注册）
        /// </summary>
        private static bool IsSystemInterface(Type type)
        {
            if (type.Namespace == null) return false;
            
            // 排除系统命名空间的接口
            return type.Namespace.StartsWith("System", StringComparison.Ordinal) ||
                   type.Namespace.StartsWith("Microsoft", StringComparison.Ordinal) ||
                   type.Namespace.StartsWith("Unity", StringComparison.Ordinal);
        }

        #endregion

        #region 生命周期配置 / Lifetime Configuration

        /// <inheritdoc/>
        public IRegistrationBuilder Singleton()
        {
            _lifetime = Lifetime.Singleton;
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Scoped()
        {
            _lifetime = Lifetime.Scoped;
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Transient()
        {
            _lifetime = Lifetime.Transient;
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithLifetime(Lifetime lifetime)
        {
            _lifetime = lifetime;
            return this;
        }

        #endregion

        #region 参数注入 / Parameter Injection

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter(string name, object value)
        {
            _parameters.Add(new NamedParameter(name, value));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter<TParam>(TParam value)
        {
            _parameters.Add(new TypedParameter(typeof(TParam), value));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter(Type type, object value)
        {
            _parameters.Add(new TypedParameter(type, value));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter<TParam>(Func<IObjectResolver, TParam> factory)
        {
            _parameters.Add(new FactoryParameter<TParam>(factory));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> factory)
        {
            _parameters.Add(new NamedFactoryParameter(name, factory));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder WithParameter(IInjectParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            
            _parameters.Add(parameter);
            return this;
        }

        #endregion

        #region 键值注册 / Keyed Registration

        /// <inheritdoc/>
        public IRegistrationBuilder Keyed(object key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Keyed<TKey>(TKey key) where TKey : Enum
        {
            _key = key;
            return this;
        }

        #endregion

        #region 元数据 / Metadata

        /// <inheritdoc/>
        public IRegistrationBuilder WithMetadata(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            
            _metadata[key] = value;
            return this;
        }

        #endregion

        #region 回调 / Callbacks

        /// <inheritdoc/>
        public IRegistrationBuilder OnActivated(Action<object> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            
            _onActivatedCallbacks.Add(callback);
            return this;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder OnActivated<T>(Action<T> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            
            _onActivatedCallbacks.Add(obj => callback((T)obj));
            return this;
        }

        #endregion

        #region 构建 / Build

        /// <inheritdoc/>
        public IRegistration Build()
        {
            // 如果没有指定服务类型，默认使用实现类型
            if (_serviceTypes.Count == 0)
            {
                var defaultType = _implementationType ?? _existingInstance?.GetType();
                if (defaultType != null)
                {
                    _serviceTypes.Add(defaultType);
                }
            }

            var primaryServiceType = _serviceTypes.Count > 0 ? _serviceTypes[0] : null;

            return new Registration(
                serviceType: primaryServiceType,
                implementationType: _implementationType,
                lifetime: _lifetime,
                key: _key,
                factory: _factory,
                existingInstance: _existingInstance,
                serviceTypes: _serviceTypes,
                parameters: _parameters,
                metadata: _metadata,
                onActivatedCallbacks: _onActivatedCallbacks
            );
        }

        #endregion
    }
}
