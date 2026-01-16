// ==========================================================
// 文件名：InstantiateExtensions.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: IObjectResolver 实例化扩展方法，提供 Unity 对象实例化能力
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// IObjectResolver 实例化扩展方法
    /// <para>为 IObjectResolver 提供 Unity 对象实例化和注入能力</para>
    /// <para>Extension methods for IObjectResolver providing Unity object instantiation</para>
    /// </summary>
    public static class InstantiateExtensions
    {
        #region GameObject 实例化 / GameObject Instantiation

        /// <summary>
        /// 实例化预制体并注入依赖
        /// <para>Instantiate a prefab and inject dependencies</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(this IObjectResolver resolver, GameObject prefab)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab);
            GameObjectInjector.Inject(resolver, instance, true);
            return instance;
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带父对象）
        /// <para>Instantiate a prefab under a parent and inject dependencies</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(
            this IObjectResolver resolver, 
            GameObject prefab, 
            Transform parent)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            GameObjectInjector.Inject(resolver, instance, true);
            return instance;
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带父对象和世界坐标选项）
        /// <para>Instantiate a prefab under a parent with world position option</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <param name="worldPositionStays">是否保持世界坐标 / Whether to keep world position</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(
            this IObjectResolver resolver, 
            GameObject prefab, 
            Transform parent, 
            bool worldPositionStays)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays);
            GameObjectInjector.Inject(resolver, instance, true);
            return instance;
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带位置和旋转）
        /// <para>Instantiate a prefab with position and rotation</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="position">位置 / Position</param>
        /// <param name="rotation">旋转 / Rotation</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(
            this IObjectResolver resolver, 
            GameObject prefab, 
            Vector3 position, 
            Quaternion rotation)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            GameObjectInjector.Inject(resolver, instance, true);
            return instance;
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带位置、旋转和父对象）
        /// <para>Instantiate a prefab with position, rotation and parent</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="position">位置 / Position</param>
        /// <param name="rotation">旋转 / Rotation</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(
            this IObjectResolver resolver, 
            GameObject prefab, 
            Vector3 position, 
            Quaternion rotation, 
            Transform parent)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
            GameObjectInjector.Inject(resolver, instance, true);
            return instance;
        }

        #endregion

        #region Component 实例化 / Component Instantiation

        /// <summary>
        /// 实例化组件预制体并注入依赖
        /// <para>Instantiate a component prefab and inject dependencies</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">组件预制体 / Component prefab</param>
        /// <returns>实例化的组件 / Instantiated component</returns>
        public static T Instantiate<T>(this IObjectResolver resolver, T prefab) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab);
            GameObjectInjector.Inject(resolver, instance.gameObject, true);
            return instance;
        }

        /// <summary>
        /// 实例化组件预制体并注入依赖（带父对象）
        /// <para>Instantiate a component prefab under a parent</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">组件预制体 / Component prefab</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>实例化的组件 / Instantiated component</returns>
        public static T Instantiate<T>(
            this IObjectResolver resolver, 
            T prefab, 
            Transform parent) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            GameObjectInjector.Inject(resolver, instance.gameObject, true);
            return instance;
        }

        /// <summary>
        /// 实例化组件预制体并注入依赖（带位置和旋转）
        /// <para>Instantiate a component prefab with position and rotation</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">组件预制体 / Component prefab</param>
        /// <param name="position">位置 / Position</param>
        /// <param name="rotation">旋转 / Rotation</param>
        /// <returns>实例化的组件 / Instantiated component</returns>
        public static T Instantiate<T>(
            this IObjectResolver resolver, 
            T prefab, 
            Vector3 position, 
            Quaternion rotation) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            GameObjectInjector.Inject(resolver, instance.gameObject, true);
            return instance;
        }

        #endregion

        #region GameObject 注入 / GameObject Injection

        /// <summary>
        /// 向 GameObject 上的所有组件注入依赖
        /// <para>Inject dependencies into all components on a GameObject</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="gameObject">目标 GameObject / Target GameObject</param>
        /// <param name="includeChildren">是否包含子对象 / Whether to include children</param>
        public static void InjectGameObject(
            this IObjectResolver resolver, 
            GameObject gameObject, 
            bool includeChildren = true)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            GameObjectInjector.Inject(resolver, gameObject, includeChildren);
        }

        #endregion

        #region 组件创建 / Component Creation

        /// <summary>
        /// 在新 GameObject 上创建组件并注入依赖
        /// <para>Create a component on a new GameObject and inject dependencies</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="name">GameObject 名称 / GameObject name</param>
        /// <returns>创建的组件 / Created component</returns>
        public static T CreateComponentOnNewGameObject<T>(
            this IObjectResolver resolver, 
            string name = null) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var go = new GameObject(name ?? typeof(T).Name);
            var component = go.AddComponent<T>();
            resolver.Inject(component);
            return component;
        }

        /// <summary>
        /// 在新 GameObject 上创建组件并注入依赖（带父对象）
        /// <para>Create a component on a new GameObject under a parent</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <param name="name">GameObject 名称 / GameObject name</param>
        /// <returns>创建的组件 / Created component</returns>
        public static T CreateComponentOnNewGameObject<T>(
            this IObjectResolver resolver, 
            Transform parent, 
            string name = null) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var go = new GameObject(name ?? typeof(T).Name);
            go.transform.SetParent(parent, false);
            var component = go.AddComponent<T>();
            resolver.Inject(component);
            return component;
        }

        #endregion
    }
}
