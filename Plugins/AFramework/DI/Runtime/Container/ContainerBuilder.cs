 // ==========================================================
// 文件名：ContainerBuilder.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 实现容器构建器，提供服务注册和容器构建能力

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 容器构建器实现类
    /// <para>提供服务注册和容器构建能力，是构建 DI 容器的入口</para>
    /// <para>Container builder implementation that provides registration and building capabilities</para>
    /// </summary>
    public sealed class ContainerBuilder : IContainerBuilder
    {
        #region 字段 / Fields

        private readonly List<RegistrationBuilder> _registrationBuilders = new List<RegistrationBuilder>();
        private readonly HashSet<Type> _registeredTypes = new HashSet<Type>();
        private readonly List<(Type ServiceType, Type ImplementationType, Lifetime Lifetime)> _openGenericRegistrations 
            = new List<(Type, Type, Lifetime)>();
        private IObjectResolver _parent;
        private bool _enableLifetimeValidation;
        private bool _enableDiagnostics;
        private bool _isBuilt;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建容器构建器实例
        /// </summary>
        public ContainerBuilder()
        {
        }

        /// <summary>
        /// 创建容器构建器实例（带父容器）
        /// </summary>
        /// <param name="parent">父容器</param>
        public ContainerBuilder(IObjectResolver parent)
        {
            _parent = parent;
        }

        #endregion

        #region 类型注册 / Type Registration

        /// <inheritdoc/>
        public IRegistrationBuilder Register<T>()
        {
            return Register(typeof(T));
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            var builder = new RegistrationBuilder(this, typeof(TImplementation));
            builder.As<TInterface>();
            _registrationBuilders.Add(builder);
            _registeredTypes.Add(typeof(TInterface));
            return builder;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Register(Type implementationType)
        {
            ValidateNotBuilt();
            
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            if (implementationType.IsAbstract || implementationType.IsInterface)
                throw RegistrationException.TypeNotInstantiable(implementationType);

            var builder = new RegistrationBuilder(this, implementationType);
            _registrationBuilders.Add(builder);
            _registeredTypes.Add(implementationType);
            return builder;
        }

        /// <inheritdoc/>
        public IRegistrationBuilder Register(Type interfaceType, Type implementationType)
        {
            ValidateNotBuilt();
            
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            if (!interfaceType.IsAssignableFrom(implementationType))
                throw RegistrationException.TypeNotAssignable(interfaceType, implementationType);

            var builder = new RegistrationBuilder(this, implementationType);
            builder.As(interfaceType);
            _registrationBuilders.Add(builder);
            _registeredTypes.Add(interfaceType);
            return builder;
        }

        #endregion

        #region 实例注册 / Instance Registration

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterInstance<T>(T instance) where T : class
        {
            return RegisterInstance(typeof(T), instance);
        }

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterInstance(Type interfaceType, object instance)
        {
            ValidateNotBuilt();
            
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var builder = new RegistrationBuilder(this, interfaceType, instance);
            _registrationBuilders.Add(builder);
            _registeredTypes.Add(interfaceType);
            return builder;
        }

        #endregion

        #region 工厂注册 / Factory Registration

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterFactory<T>(Func<IObjectResolver, T> factory) where T : class
        {
            return RegisterFactory(typeof(T), r => factory(r));
        }

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterFactory(Type interfaceType, Func<IObjectResolver, object> factory)
        {
            ValidateNotBuilt();
            
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var builder = new RegistrationBuilder(this, interfaceType, factory);
            _registrationBuilders.Add(builder);
            _registeredTypes.Add(interfaceType);
            return builder;
        }

        #endregion

        #region 安装器 / Installers

        /// <inheritdoc/>
        public IContainerBuilder UseInstaller<TInstaller>() where TInstaller : IInstaller, new()
        {
            var installer = new TInstaller();
            return UseInstaller(installer);
        }

        /// <inheritdoc/>
        public IContainerBuilder UseInstaller(IInstaller installer)
        {
            ValidateNotBuilt();
            
            if (installer == null)
                throw new ArgumentNullException(nameof(installer));

            installer.Install(this);
            return this;
        }

        /// <inheritdoc/>
        public IContainerBuilder UseInstallers(IEnumerable<IInstaller> installers)
        {
            if (installers == null)
                throw new ArgumentNullException(nameof(installers));

            foreach (var installer in installers)
            {
                UseInstaller(installer);
            }
            return this;
        }

        #endregion

        #region 条件注册 / Conditional Registration

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterIfNotRegistered<T>()
        {
            if (IsRegistered<T>())
            {
                // 返回一个空操作的构建器
                return new NullRegistrationBuilder();
            }
            return Register<T>();
        }

        /// <inheritdoc/>
        public IRegistrationBuilder RegisterIfNotRegistered<TInterface, TImplementation>() 
            where TImplementation : TInterface
        {
            if (IsRegistered<TInterface>())
            {
                return new NullRegistrationBuilder();
            }
            return Register<TInterface, TImplementation>();
        }

        #endregion

        #region 构建 / Build

        /// <inheritdoc/>
        public IObjectResolver Build()
        {
            ValidateNotBuilt();
            _isBuilt = true;

            // 构建所有注册信息
            var registrations = new List<IRegistration>();
            foreach (var builder in _registrationBuilders)
            {
                var registration = builder.Build();
                registrations.Add(registration);
            }

            // 创建注册表
            var registry = new Registry(registrations);

            // 添加开放泛型注册
            foreach (var (serviceType, implType, lifetime) in _openGenericRegistrations)
            {
                registry.AddOpenGeneric(serviceType, implType, lifetime);
            }
            
            // 冻结注册表，构建高性能查找结构
            registry.Freeze();

            // 创建容器
            var container = new Container(registry, _parent, _enableDiagnostics);

            // 生命周期验证
            if (_enableLifetimeValidation)
            {
                ValidateLifetimes(registry);
            }

            return container;
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <inheritdoc/>
        public IReadOnlyList<IRegistration> GetRegistrations()
        {
            var registrations = new List<IRegistration>();
            foreach (var builder in _registrationBuilders)
            {
                registrations.Add(builder.Build());
            }
            return registrations;
        }

        /// <inheritdoc/>
        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type type)
        {
            return _registeredTypes.Contains(type);
        }

        #endregion

        #region 配置 / Configuration

        /// <inheritdoc/>
        public IContainerBuilder SetParent(IObjectResolver parent)
        {
            ValidateNotBuilt();
            _parent = parent;
            return this;
        }

        /// <inheritdoc/>
        public IContainerBuilder EnableLifetimeValidation(bool enabled = true)
        {
            ValidateNotBuilt();
            _enableLifetimeValidation = enabled;
            return this;
        }

        /// <inheritdoc/>
        public IContainerBuilder EnableDiagnostics(bool enabled = true)
        {
            ValidateNotBuilt();
            _enableDiagnostics = enabled;
            return this;
        }

        #endregion

        #region 私有方法 / Private Methods

        private void ValidateNotBuilt()
        {
            if (_isBuilt)
            {
                throw new InvalidOperationException(
                    "容器已构建，无法修改注册。\n" +
                    "Container has been built, cannot modify registrations.");
            }
        }

        private void ValidateLifetimes(Registry registry)
        {
            // 执行构建时循环依赖检测
            DependencyAnalyzer.AnalyzeCircularDependencies(registry);
            
            // TODO: 实现生命周期验证逻辑
            // 检测 Singleton 依赖 Scoped 等问题
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 注册开放泛型类型（内部使用）
        /// <para>Register open generic type (internal use)</para>
        /// </summary>
        /// <param name="serviceType">开放泛型服务类型 / Open generic service type</param>
        /// <param name="implementationType">开放泛型实现类型 / Open generic implementation type</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        internal void RegisterOpenGenericInternal(Type serviceType, Type implementationType, Lifetime lifetime)
        {
            ValidateNotBuilt();
            _openGenericRegistrations.Add((serviceType, implementationType, lifetime));
            _registeredTypes.Add(serviceType);
        }

        #endregion
    }

    /// <summary>
    /// 空操作注册构建器（用于条件注册时类型已存在的情况）
    /// </summary>
    internal sealed class NullRegistrationBuilder : IRegistrationBuilder
    {
        public IRegistrationBuilder As<TInterface>() => this;
        public IRegistrationBuilder As(Type interfaceType) => this;
        public IRegistrationBuilder AsSelf() => this;
        public IRegistrationBuilder AsImplementedInterfaces() => this;
        public IRegistrationBuilder AsSelfAndImplementedInterfaces() => this;
        public IRegistrationBuilder Singleton() => this;
        public IRegistrationBuilder Scoped() => this;
        public IRegistrationBuilder Transient() => this;
        public IRegistrationBuilder WithLifetime(Lifetime lifetime) => this;
        public IRegistrationBuilder WithParameter(string name, object value) => this;
        public IRegistrationBuilder WithParameter<TParam>(TParam value) => this;
        public IRegistrationBuilder WithParameter(Type type, object value) => this;
        public IRegistrationBuilder WithParameter<TParam>(Func<IObjectResolver, TParam> factory) => this;
        public IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> factory) => this;
        public IRegistrationBuilder WithParameter(IInjectParameter parameter) => this;
        public IRegistrationBuilder Keyed(object key) => this;
        public IRegistrationBuilder Keyed<TKey>(TKey key) where TKey : Enum => this;
        public IRegistrationBuilder WithMetadata(string key, object value) => this;
        public IRegistrationBuilder OnActivated(Action<object> callback) => this;
        public IRegistrationBuilder OnActivated<T>(Action<T> callback) => this;
        public IRegistration Build() => null;
    }
}
