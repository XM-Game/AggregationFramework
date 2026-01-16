// ==========================================================
// 文件名：LifetimeScopeExtensions.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: LifetimeScope 扩展方法，提供便捷的容器操作 API
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// LifetimeScope 扩展方法
    /// <para>提供便捷的容器操作和 Unity 集成功能</para>
    /// <para>Extension methods for LifetimeScope providing convenient container operations</para>
    /// </summary>
    public static class LifetimeScopeExtensions
    {
        #region 解析扩展 / Resolution Extensions

        /// <summary>
        /// 从 LifetimeScope 解析服务
        /// <para>Resolve a service from LifetimeScope</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <returns>服务实例 / Service instance</returns>
        public static T Resolve<T>(this LifetimeScope scope)
        {
            EnsureBuilt(scope);
            return scope.Container.Resolve<T>();
        }

        /// <summary>
        /// 从 LifetimeScope 解析服务（非泛型版本）
        /// <para>Resolve a service from LifetimeScope (non-generic version)</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>服务实例 / Service instance</returns>
        public static object Resolve(this LifetimeScope scope, Type type)
        {
            EnsureBuilt(scope);
            return scope.Container.Resolve(type);
        }

        /// <summary>
        /// 尝试从 LifetimeScope 解析服务
        /// <para>Try to resolve a service from LifetimeScope</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="instance">解析的实例 / Resolved instance</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool TryResolve<T>(this LifetimeScope scope, out T instance)
        {
            if (!scope.IsBuilt || scope.Container == null)
            {
                instance = default;
                return false;
            }
            return scope.Container.TryResolve(out instance);
        }

        #endregion

        #region 注入扩展 / Injection Extensions

        /// <summary>
        /// 向现有实例注入依赖
        /// <para>Inject dependencies into an existing instance</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="instance">要注入的实例 / Instance to inject</param>
        public static void Inject(this LifetimeScope scope, object instance)
        {
            EnsureBuilt(scope);
            scope.Container.Inject(instance);
        }

        /// <summary>
        /// 向 GameObject 上的所有组件注入依赖
        /// <para>Inject dependencies into all components on a GameObject</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="gameObject">目标 GameObject / Target GameObject</param>
        /// <param name="includeChildren">是否包含子对象 / Whether to include children</param>
        public static void InjectGameObject(
            this LifetimeScope scope, 
            GameObject gameObject, 
            bool includeChildren = true)
        {
            EnsureBuilt(scope);
            GameObjectInjector.Inject(scope.Container, gameObject, includeChildren);
        }

        #endregion

        #region 实例化扩展 / Instantiation Extensions

        /// <summary>
        /// 创建实例并注入依赖
        /// <para>Create an instance and inject dependencies</para>
        /// </summary>
        /// <typeparam name="T">要创建的类型 / Type to create</typeparam>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <returns>创建的实例 / Created instance</returns>
        public static T Instantiate<T>(this LifetimeScope scope)
        {
            EnsureBuilt(scope);
            return scope.Container.Instantiate<T>();
        }

        /// <summary>
        /// 实例化预制体并注入依赖
        /// <para>Instantiate a prefab and inject dependencies</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject InstantiatePrefab(this LifetimeScope scope, GameObject prefab)
        {
            EnsureBuilt(scope);
            return PrefabInjector.Instantiate(scope.Container, prefab);
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带位置和旋转）
        /// <para>Instantiate a prefab with position and rotation, then inject dependencies</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="position">位置 / Position</param>
        /// <param name="rotation">旋转 / Rotation</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject InstantiatePrefab(
            this LifetimeScope scope, 
            GameObject prefab, 
            Vector3 position, 
            Quaternion rotation)
        {
            EnsureBuilt(scope);
            return PrefabInjector.Instantiate(scope.Container, prefab, position, rotation);
        }

        /// <summary>
        /// 实例化预制体并注入依赖（带父对象）
        /// <para>Instantiate a prefab under a parent, then inject dependencies</para>
        /// </summary>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <param name="parent">父 Transform / Parent Transform</param>
        /// <returns>实例化的 GameObject / Instantiated GameObject</returns>
        public static GameObject InstantiatePrefab(
            this LifetimeScope scope, 
            GameObject prefab, 
            Transform parent)
        {
            EnsureBuilt(scope);
            return PrefabInjector.Instantiate(scope.Container, prefab, parent);
        }

        /// <summary>
        /// 实例化预制体并获取指定组件
        /// <para>Instantiate a prefab and get a specific component</para>
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="scope">LifetimeScope 实例 / LifetimeScope instance</param>
        /// <param name="prefab">预制体 / Prefab</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T InstantiatePrefab<T>(this LifetimeScope scope, T prefab) 
            where T : Component
        {
            EnsureBuilt(scope);
            return PrefabInjector.Instantiate(scope.Container, prefab);
        }

        #endregion

        #region 子作用域扩展 / Child Scope Extensions

        /// <summary>
        /// 创建子作用域
        /// <para>Create a child scope</para>
        /// </summary>
        /// <param name="scope">父 LifetimeScope / Parent LifetimeScope</param>
        /// <returns>子作用域解析器 / Child scope resolver</returns>
        public static IObjectResolver CreateChildScope(this LifetimeScope scope)
        {
            EnsureBuilt(scope);
            return scope.Container.CreateScope();
        }

        /// <summary>
        /// 创建子作用域（带配置）
        /// <para>Create a child scope with configuration</para>
        /// </summary>
        /// <param name="scope">父 LifetimeScope / Parent LifetimeScope</param>
        /// <param name="configuration">配置委托 / Configuration delegate</param>
        /// <returns>子作用域解析器 / Child scope resolver</returns>
        public static IObjectResolver CreateChildScope(
            this LifetimeScope scope, 
            Action<IContainerBuilder> configuration)
        {
            EnsureBuilt(scope);
            return scope.Container.CreateScope(configuration);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 确保 LifetimeScope 已构建
        /// </summary>
        private static void EnsureBuilt(LifetimeScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            if (!scope.IsBuilt || scope.Container == null)
            {
                throw new InvalidOperationException(
                    $"LifetimeScope '{scope.name}' 尚未构建。请先调用 Build() 或启用 AutoRun。\n" +
                    $"LifetimeScope '{scope.name}' is not built. Call Build() first or enable AutoRun.");
            }
        }

        #endregion
    }
}
