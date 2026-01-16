// ==========================================================
// 文件名：ContainerBuilderUnityExtensions.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: IContainerBuilder 的 Unity 组件注册扩展方法
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// IContainerBuilder Unity 扩展方法
    /// <para>提供 Unity 组件注册的便捷 API</para>
    /// <para>Extension methods for IContainerBuilder providing Unity component registration</para>
    /// </summary>
    public static class ContainerBuilderUnityExtensions
    {
        #region 组件实例注册 / Component Instance Registration

        /// <summary>
        /// 注册现有组件实例
        /// <para>Register an existing component instance</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="component">组件实例 / Component instance</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponent<T>(
            this IContainerBuilder builder, 
            T component) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            return builder.RegisterInstance(component).AsSelf();
        }

        /// <summary>
        /// 注册现有组件实例并映射到接口
        /// <para>Register an existing component instance and map to interface</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TComponent">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="component">组件实例 / Component instance</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponent<TInterface, TComponent>(
            this IContainerBuilder builder, 
            TComponent component) 
            where TComponent : Component, TInterface
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            return builder.RegisterInstance(component).As<TInterface>();
        }

        #endregion

        #region 层级查找注册 / Hierarchy Search Registration

        /// <summary>
        /// 在层级中查找并注册组件
        /// <para>Find and register a component in hierarchy</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="searchRoot">搜索根 Transform / Search root Transform</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentInHierarchy<T>(
            this IContainerBuilder builder, 
            Transform searchRoot = null) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory<T>(resolver =>
            {
                T component;
                if (searchRoot != null)
                {
                    component = searchRoot.GetComponentInChildren<T>(true);
                }
                else
                {
                    component = UnityEngine.Object.FindObjectOfType<T>();
                }

                if (component == null)
                {
                    throw new ResolutionException(
                        $"未找到类型为 '{typeof(T).Name}' 的组件。\n" +
                        $"Component of type '{typeof(T).Name}' not found.");
                }

                resolver.Inject(component);
                return component;
            }).Singleton();
        }

        /// <summary>
        /// 在层级中查找并注册组件（映射到接口）
        /// <para>Find and register a component in hierarchy (map to interface)</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TComponent">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="searchRoot">搜索根 Transform / Search root Transform</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentInHierarchy<TInterface, TComponent>(
            this IContainerBuilder builder, 
            Transform searchRoot = null) 
            where TInterface : class
            where TComponent : Component, TInterface
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory<TInterface>(resolver =>
            {
                TComponent component;
                if (searchRoot != null)
                {
                    component = searchRoot.GetComponentInChildren<TComponent>(true);
                }
                else
                {
                    component = UnityEngine.Object.FindObjectOfType<TComponent>();
                }

                if (component == null)
                {
                    throw new ResolutionException(
                        $"未找到类型为 '{typeof(TComponent).Name}' 的组件。\n" +
                        $"Component of type '{typeof(TComponent).Name}' not found.");
                }

                resolver.Inject(component);
                return component;
            }).Singleton();
        }

        #endregion

        #region 新 GameObject 注册 / New GameObject Registration

        /// <summary>
        /// 在新 GameObject 上创建并注册组件
        /// <para>Create and register a component on a new GameObject</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="name">GameObject 名称 / GameObject name</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentOnNewGameObject<T>(
            this IContainerBuilder builder, 
            string name = null) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var goName = name ?? typeof(T).Name;

            return builder.RegisterFactory<T>(resolver =>
            {
                var go = new GameObject(goName);
                var component = go.AddComponent<T>();
                resolver.Inject(component);
                return component;
            }).Singleton();
        }

        /// <summary>
        /// 在新 GameObject 上创建并注册组件（映射到接口）
        /// <para>Create and register a component on a new GameObject (map to interface)</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TComponent">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="name">GameObject 名称 / GameObject name</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentOnNewGameObject<TInterface, TComponent>(
            this IContainerBuilder builder, 
            string name = null) 
            where TInterface : class
            where TComponent : Component, TInterface
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var goName = name ?? typeof(TComponent).Name;

            return builder.RegisterFactory<TInterface>(resolver =>
            {
                var go = new GameObject(goName);
                var component = go.AddComponent<TComponent>();
                resolver.Inject(component);
                return component;
            }).Singleton();
        }

        /// <summary>
        /// 在新 GameObject 上创建并注册组件（带父对象）
        /// <para>Create and register a component on a new GameObject under a parent</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <param name="name">GameObject 名称 / GameObject name</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentOnNewGameObject<T>(
            this IContainerBuilder builder, 
            Transform parent, 
            string name = null) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var goName = name ?? typeof(T).Name;

            return builder.RegisterFactory<T>(resolver =>
            {
                var go = new GameObject(goName);
                if (parent != null)
                {
                    go.transform.SetParent(parent, false);
                }
                var component = go.AddComponent<T>();
                resolver.Inject(component);
                return component;
            }).Singleton();
        }

        #endregion

        #region 预制体注册 / Prefab Registration

        /// <summary>
        /// 从预制体实例化并注册组件
        /// <para>Instantiate from prefab and register component</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentFromPrefab<T>(
            this IContainerBuilder builder, 
            T prefab) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory<T>(resolver =>
            {
                var instance = UnityEngine.Object.Instantiate(prefab);
                GameObjectInjector.Inject(resolver, instance.gameObject, true);
                return instance;
            }).Singleton();
        }

        /// <summary>
        /// 从预制体实例化并注册组件（映射到接口）
        /// <para>Instantiate from prefab and register component (map to interface)</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <typeparam name="TComponent">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentFromPrefab<TInterface, TComponent>(
            this IContainerBuilder builder, 
            TComponent prefab) 
            where TInterface : class
            where TComponent : Component, TInterface
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory<TInterface>(resolver =>
            {
                var instance = UnityEngine.Object.Instantiate(prefab);
                GameObjectInjector.Inject(resolver, instance.gameObject, true);
                return instance;
            }).Singleton();
        }

        /// <summary>
        /// 从 GameObject 预制体实例化并注册组件
        /// <para>Instantiate from GameObject prefab and register component</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="prefab">GameObject 预制体 / GameObject prefab</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentFromPrefab<T>(
            this IContainerBuilder builder, 
            GameObject prefab) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory<T>(resolver =>
            {
                var instance = UnityEngine.Object.Instantiate(prefab);
                var component = instance.GetComponent<T>();
                
                if (component == null)
                {
                    throw new ResolutionException(
                        $"预制体 '{prefab.name}' 上未找到类型为 '{typeof(T).Name}' 的组件。\n" +
                        $"Component of type '{typeof(T).Name}' not found on prefab '{prefab.name}'.");
                }

                GameObjectInjector.Inject(resolver, instance, true);
                return component;
            }).Singleton();
        }

        /// <summary>
        /// 从预制体实例化并注册组件（带父对象）
        /// <para>Instantiate from prefab under a parent and register component</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterComponentFromPrefab<T>(
            this IContainerBuilder builder, 
            T prefab, 
            Transform parent) 
            where T : Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory<T>(resolver =>
            {
                var instance = parent != null
                    ? UnityEngine.Object.Instantiate(prefab, parent)
                    : UnityEngine.Object.Instantiate(prefab);
                    
                GameObjectInjector.Inject(resolver, instance.gameObject, true);
                return instance;
            }).Singleton();
        }

        #endregion
    }
}
