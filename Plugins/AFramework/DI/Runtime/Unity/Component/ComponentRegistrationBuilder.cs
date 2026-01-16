// ==========================================================
// 文件名：ComponentRegistrationBuilder.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: Unity 组件注册构建器，提供链式 API 配置组件注册
// ==========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 组件注册构建器
    /// <para>提供链式 API 配置 Unity 组件的注册</para>
    /// <para>Builder for configuring Unity component registration with fluent API</para>
    /// </summary>
    public sealed class ComponentRegistrationBuilder
    {
        #region 字段 / Fields

        private readonly ComponentRegistration _registration;
        private readonly IContainerBuilder _containerBuilder;
        private readonly List<Type> _serviceTypes = new List<Type>();

        #endregion

        #region 构造函数 / Constructor

        internal ComponentRegistrationBuilder(
            ComponentRegistration registration, 
            IContainerBuilder containerBuilder)
        {
            _registration = registration ?? throw new ArgumentNullException(nameof(registration));
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
        }

        #endregion

        #region 服务类型映射 / Service Type Mapping

        /// <summary>
        /// 将组件映射到指定的服务类型
        /// <para>Map the component to the specified service type</para>
        /// </summary>
        /// <typeparam name="TInterface">服务类型 / Service type</typeparam>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder As<TInterface>()
        {
            return As(typeof(TInterface));
        }

        /// <summary>
        /// 将组件映射到指定的服务类型（非泛型版本）
        /// <para>Map the component to the specified service type (non-generic version)</para>
        /// </summary>
        /// <param name="interfaceType">服务类型 / Service type</param>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder As(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            if (!interfaceType.IsAssignableFrom(_registration.ComponentType))
            {
                throw new ArgumentException(
                    $"组件类型 '{_registration.ComponentType.Name}' 未实现接口 '{interfaceType.Name}'。\n" +
                    $"Component type '{_registration.ComponentType.Name}' does not implement '{interfaceType.Name}'.",
                    nameof(interfaceType));
            }

            if (!_serviceTypes.Contains(interfaceType))
            {
                _serviceTypes.Add(interfaceType);
            }

            return this;
        }

        /// <summary>
        /// 将组件映射到自身类型
        /// <para>Map the component to its own type</para>
        /// </summary>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder AsSelf()
        {
            if (!_serviceTypes.Contains(_registration.ComponentType))
            {
                _serviceTypes.Add(_registration.ComponentType);
            }
            return this;
        }

        /// <summary>
        /// 将组件映射到所有实现的接口
        /// <para>Map the component to all implemented interfaces</para>
        /// </summary>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder AsImplementedInterfaces()
        {
            var interfaces = _registration.ComponentType.GetInterfaces();
            foreach (var iface in interfaces)
            {
                // 跳过系统接口
                if (iface.Namespace?.StartsWith("System") == true)
                    continue;
                if (iface.Namespace?.StartsWith("UnityEngine") == true)
                    continue;

                if (!_serviceTypes.Contains(iface))
                {
                    _serviceTypes.Add(iface);
                }
            }
            return this;
        }

        #endregion

        #region 生命周期配置 / Lifetime Configuration

        /// <summary>
        /// 设置为单例生命周期
        /// <para>Set the lifetime to Singleton</para>
        /// </summary>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder Singleton()
        {
            _registration.Lifetime = Lifetime.Singleton;
            return this;
        }

        /// <summary>
        /// 设置为作用域生命周期
        /// <para>Set the lifetime to Scoped</para>
        /// </summary>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder Scoped()
        {
            _registration.Lifetime = Lifetime.Scoped;
            return this;
        }

        #endregion

        #region 配置选项 / Configuration Options

        /// <summary>
        /// 设置是否包含子对象（用于 FindInHierarchy）
        /// <para>Set whether to include children (for FindInHierarchy)</para>
        /// </summary>
        /// <param name="include">是否包含 / Whether to include</param>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder IncludeChildren(bool include = true)
        {
            _registration.IncludeChildren = include;
            return this;
        }

        /// <summary>
        /// 设置 GameObject 名称（用于 NewGameObject）
        /// <para>Set the GameObject name (for NewGameObject)</para>
        /// </summary>
        /// <param name="name">名称 / Name</param>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder WithName(string name)
        {
            _registration.GameObjectName = name;
            return this;
        }

        /// <summary>
        /// 设置父 Transform
        /// <para>Set the parent Transform</para>
        /// </summary>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>当前构建器实例 / Current builder instance</returns>
        public ComponentRegistrationBuilder UnderTransform(Transform parent)
        {
            _registration.ParentTransform = parent;
            return this;
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 完成构建并注册到容器
        /// </summary>
        internal void Build()
        {
            // 确保至少有一个服务类型
            if (_serviceTypes.Count == 0)
            {
                _serviceTypes.Add(_registration.ComponentType);
            }

            _registration.ServiceTypes = _serviceTypes.ToArray();

            // 根据注册类型创建工厂
            Func<IObjectResolver, object> factory = CreateFactory();

            // 注册到容器
            var builder = _containerBuilder.RegisterFactory(_registration.ServiceTypes[0], factory);

            // 设置生命周期
            builder.WithLifetime(_registration.Lifetime);

            // 添加其他服务类型映射
            for (int i = 1; i < _registration.ServiceTypes.Length; i++)
            {
                builder.As(_registration.ServiceTypes[i]);
            }
        }

        /// <summary>
        /// 创建组件工厂
        /// </summary>
        private Func<IObjectResolver, object> CreateFactory()
        {
            switch (_registration.RegistrationType)
            {
                case ComponentRegistrationType.Instance:
                    return _ => _registration.ExistingInstance;

                case ComponentRegistrationType.FindInHierarchy:
                    return resolver => FindComponent(resolver);

                case ComponentRegistrationType.NewGameObject:
                    return resolver => CreateOnNewGameObject(resolver);

                case ComponentRegistrationType.FromPrefab:
                    return resolver => InstantiateFromPrefab(resolver);

                default:
                    throw new NotSupportedException(
                        $"不支持的组件注册类型: {_registration.RegistrationType}");
            }
        }

        /// <summary>
        /// 在层级中查找组件
        /// </summary>
        private Component FindComponent(IObjectResolver resolver)
        {
            var root = _registration.SearchRoot;
            Component component;

            if (root != null)
            {
                component = _registration.IncludeChildren
                    ? root.GetComponentInChildren(_registration.ComponentType, true)
                    : root.GetComponent(_registration.ComponentType);
            }
            else
            {
                component = UnityEngine.Object.FindObjectOfType(_registration.ComponentType) as Component;
            }

            if (component == null)
            {
                throw new ResolutionException(
                    $"未找到类型为 '{_registration.ComponentType.Name}' 的组件。\n" +
                    $"Component of type '{_registration.ComponentType.Name}' not found.");
            }

            // 注入依赖
            resolver.Inject(component);
            return component;
        }

        /// <summary>
        /// 在新 GameObject 上创建组件
        /// </summary>
        private Component CreateOnNewGameObject(IObjectResolver resolver)
        {
            var go = new GameObject(_registration.GameObjectName);
            
            if (_registration.ParentTransform != null)
            {
                go.transform.SetParent(_registration.ParentTransform, false);
            }

            var component = go.AddComponent(_registration.ComponentType);
            
            // 注入依赖
            resolver.Inject(component);
            return component;
        }

        /// <summary>
        /// 从预制体实例化
        /// </summary>
        private Component InstantiateFromPrefab(IObjectResolver resolver)
        {
            var go = _registration.ParentTransform != null
                ? UnityEngine.Object.Instantiate(_registration.Prefab, _registration.ParentTransform)
                : UnityEngine.Object.Instantiate(_registration.Prefab);

            var component = go.GetComponent(_registration.ComponentType);
            
            if (component == null)
            {
                throw new ResolutionException(
                    $"预制体 '{_registration.Prefab.name}' 上未找到类型为 '{_registration.ComponentType.Name}' 的组件。\n" +
                    $"Component of type '{_registration.ComponentType.Name}' not found on prefab '{_registration.Prefab.name}'.");
            }

            // 注入 GameObject 上的所有组件
            GameObjectInjector.Inject(resolver, go, true);
            return component;
        }

        #endregion
    }
}
