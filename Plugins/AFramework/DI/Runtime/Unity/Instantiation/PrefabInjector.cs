// ==========================================================
// 文件名：PrefabInjector.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: 预制体注入器，提供预制体实例化和依赖注入功能
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 预制体注入器
    /// <para>提供预制体实例化和自动依赖注入功能</para>
    /// <para>Provides prefab instantiation with automatic dependency injection</para>
    /// </summary>
    /// <remarks>
    /// 功能特点：
    /// <list type="bullet">
    /// <item>实例化预制体后自动注入依赖</item>
    /// <item>支持多种实例化重载（位置、旋转、父对象）</item>
    /// <item>递归注入子对象上的组件</item>
    /// </list>
    /// </remarks>
    public static class PrefabInjector
    {
        #region GameObject 实例化 / GameObject Instantiation

        /// <summary>
        /// 实例化预制体并注入依赖
        /// <para>Instantiate a prefab and inject dependencies</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject Instantiate(IObjectResolver resolver, GameObject prefab)
        {
            ValidateArguments(resolver, prefab);

            var instance = UnityEngine.Object.Instantiate(prefab);
            InjectAll(resolver, instance);
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
            IObjectResolver resolver, 
            GameObject prefab, 
            Transform parent)
        {
            ValidateArguments(resolver, prefab);

            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            InjectAll(resolver, instance);
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
            IObjectResolver resolver, 
            GameObject prefab, 
            Vector3 position, 
            Quaternion rotation)
        {
            ValidateArguments(resolver, prefab);

            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            InjectAll(resolver, instance);
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
            IObjectResolver resolver, 
            GameObject prefab, 
            Vector3 position, 
            Quaternion rotation, 
            Transform parent)
        {
            ValidateArguments(resolver, prefab);

            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
            InjectAll(resolver, instance);
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
        public static T Instantiate<T>(IObjectResolver resolver, T prefab) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab);
            InjectAll(resolver, instance.gameObject);
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
            IObjectResolver resolver, 
            T prefab, 
            Transform parent) 
            where T : Component
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            InjectAll(resolver, instance.gameObject);
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
            IObjectResolver resolver, 
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
            InjectAll(resolver, instance.gameObject);
            return instance;
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 验证参数
        /// </summary>
        private static void ValidateArguments(IObjectResolver resolver, GameObject prefab)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));
        }

        /// <summary>
        /// 注入 GameObject 上的所有组件
        /// </summary>
        private static void InjectAll(IObjectResolver resolver, GameObject gameObject)
        {
            GameObjectInjector.Inject(resolver, gameObject, true);
        }

        #endregion
    }
}
