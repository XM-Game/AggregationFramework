// ==========================================================
// 文件名：ComponentPoolExtensions.cs
// 命名空间: AFramework.Pool.Extensions
// 依赖: UnityEngine, AFramework.Pool
// 功能: Unity 组件对象池扩展方法
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.Pool.Extensions
{
    /// <summary>
    /// 组件对象池扩展方法
    /// Component Pool Extension Methods
    /// </summary>
    /// <remarks>
    /// 提供便捷的 Unity 组件池操作扩展方法，包括：
    /// - 组件获取/归还扩展
    /// - 组件查找扩展
    /// - 组件激活/停用扩展
    /// - GameObject 关联操作
    /// Provides convenient extension methods for Unity component pool operations, including:
    /// - Component get/return extensions
    /// - Component finding extensions
    /// - Component activation/deactivation extensions
    /// - GameObject related operations
    /// </remarks>
    public static class ComponentPoolExtensions
    {
        #region 组件获取与归还扩展 Component Get & Return Extensions

        /// <summary>
        /// 获取组件并设置位置
        /// Get component and set position
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T Get<T>(this IObjectPool<T> pool, Vector3 position) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            component.transform.position = position;
            return component;
        }

        /// <summary>
        /// 获取组件并设置位置和旋转
        /// Get component and set position and rotation
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <param name="rotation">世界坐标旋转 / World rotation</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T Get<T>(this IObjectPool<T> pool, Vector3 position, Quaternion rotation) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            var transform = component.transform;
            transform.position = position;
            transform.rotation = rotation;
            return component;
        }

        /// <summary>
        /// 获取组件并设置父对象
        /// Get component and set parent
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="parent">父级 Transform / Parent transform</param>
        /// <param name="worldPositionStays">是否保持世界坐标 / Whether to keep world position</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T Get<T>(this IObjectPool<T> pool, Transform parent, bool worldPositionStays = false) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            component.transform.SetParent(parent, worldPositionStays);
            return component;
        }

        /// <summary>
        /// 获取组件并设置完整 Transform 信息
        /// Get component and set complete Transform information
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="position">世界坐标位置 / World position</param>
        /// <param name="rotation">世界坐标旋转 / World rotation</param>
        /// <param name="parent">父级 Transform / Parent transform</param>
        /// <param name="worldPositionStays">是否保持世界坐标 / Whether to keep world position</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T Get<T>(
            this IObjectPool<T> pool,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool worldPositionStays = false) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            var transform = component.transform;
            transform.SetParent(parent, worldPositionStays);
            transform.position = position;
            transform.rotation = rotation;
            return component;
        }

        /// <summary>
        /// 归还组件（通过 GameObject）
        /// Return component (by GameObject)
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="gameObject">GameObject 实例 / GameObject instance</param>
        public static void ReturnByGameObject<T>(this IObjectPool<T> pool, GameObject gameObject) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (gameObject == null)
                return;

            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                pool.Return(component);
            }
        }

        /// <summary>
        /// 延迟归还组件（指定秒数后自动归还）
        /// Return component after delay (automatically return after specified seconds)
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="component">组件实例 / Component instance</param>
        /// <param name="delay">延迟秒数 / Delay in seconds</param>
        public static void ReturnAfter<T>(this IObjectPool<T> pool, T component, float delay) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (delay <= 0)
            {
                pool.Return(component);
                return;
            }

            // 使用 MonoBehaviour 协程实现延迟归还
            // Use MonoBehaviour coroutine for delayed return
            var helper = component.GetComponent<ComponentPoolReturnHelper>() ?? component.gameObject.AddComponent<ComponentPoolReturnHelper>();
            helper.ReturnAfter(pool, component, delay);
        }

        #endregion

        #region 组件查找扩展 Component Finding Extensions

        /// <summary>
        /// 获取组件并查找子组件
        /// Get component and find child component
        /// </summary>
        /// <typeparam name="T">池组件类型 / Pool component type</typeparam>
        /// <typeparam name="TChild">子组件类型 / Child component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="childComponent">输出子组件 / Output child component</param>
        /// <returns>池组件实例 / Pool component instance</returns>
        public static T GetWithChild<T, TChild>(this IObjectPool<T> pool, out TChild childComponent)
            where T : Component
            where TChild : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            childComponent = component.GetComponentInChildren<TChild>();
            return component;
        }

        /// <summary>
        /// 获取组件并查找所有子组件
        /// Get component and find all child components
        /// </summary>
        /// <typeparam name="T">池组件类型 / Pool component type</typeparam>
        /// <typeparam name="TChild">子组件类型 / Child component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="childComponents">输出子组件数组 / Output child component array</param>
        /// <returns>池组件实例 / Pool component instance</returns>
        public static T GetWithChildren<T, TChild>(this IObjectPool<T> pool, out TChild[] childComponents)
            where T : Component
            where TChild : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            childComponents = component.GetComponentsInChildren<TChild>();
            return component;
        }

        #endregion

        #region 激活与停用扩展 Activation & Deactivation Extensions

        /// <summary>
        /// 获取并激活组件的 GameObject
        /// Get and activate component's GameObject
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="active">是否激活 / Whether to activate</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T GetAndSetActive<T>(this IObjectPool<T> pool, bool active = true) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            component.gameObject.SetActive(active);
            return component;
        }

        /// <summary>
        /// 停用并归还组件
        /// Deactivate and return component
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="component">组件实例 / Component instance</param>
        public static void DeactivateAndReturn<T>(this IObjectPool<T> pool, T component) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (component == null)
                return;

            component.gameObject.SetActive(false);
            pool.Return(component);
        }

        /// <summary>
        /// 启用组件并获取
        /// Enable component and get
        /// </summary>
        /// <typeparam name="T">组件类型（必须是 Behaviour）/ Component type (must be Behaviour)</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="enabled">是否启用 / Whether to enable</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T GetAndSetEnabled<T>(this IObjectPool<T> pool, bool enabled = true) where T : Behaviour
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            component.enabled = enabled;
            return component;
        }

        /// <summary>
        /// 禁用并归还组件
        /// Disable and return component
        /// </summary>
        /// <typeparam name="T">组件类型（必须是 Behaviour）/ Component type (must be Behaviour)</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="component">组件实例 / Component instance</param>
        public static void DisableAndReturn<T>(this IObjectPool<T> pool, T component) where T : Behaviour
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (component == null)
                return;

            component.enabled = false;
            pool.Return(component);
        }

        #endregion

        #region Transform 操作扩展 Transform Operation Extensions

        /// <summary>
        /// 获取组件并重置 Transform
        /// Get component and reset Transform
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="resetPosition">是否重置位置 / Whether to reset position</param>
        /// <param name="resetRotation">是否重置旋转 / Whether to reset rotation</param>
        /// <param name="resetScale">是否重置缩放 / Whether to reset scale</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T GetAndResetTransform<T>(
            this IObjectPool<T> pool,
            bool resetPosition = true,
            bool resetRotation = true,
            bool resetScale = true) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            var transform = component.transform;

            if (resetPosition)
                transform.localPosition = Vector3.zero;

            if (resetRotation)
                transform.localRotation = Quaternion.identity;

            if (resetScale)
                transform.localScale = Vector3.one;

            return component;
        }

        /// <summary>
        /// 获取组件并设置本地 Transform
        /// Get component and set local Transform
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="localPosition">本地位置 / Local position</param>
        /// <param name="localRotation">本地旋转 / Local rotation</param>
        /// <param name="localScale">本地缩放 / Local scale</param>
        /// <returns>组件实例 / Component instance</returns>
        public static T GetWithLocalTransform<T>(
            this IObjectPool<T> pool,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            var transform = component.transform;
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
            return component;
        }

        #endregion

        #region 批量操作扩展 Batch Operation Extensions

        /// <summary>
        /// 批量获取组件
        /// Get components in batch
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="count">数量 / Count</param>
        /// <param name="parent">父级 Transform（可选）/ Parent transform (optional)</param>
        /// <returns>组件数组 / Component array</returns>
        public static T[] GetMany<T>(this IObjectPool<T> pool, int count, Transform parent = null) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var components = new T[count];
            for (int i = 0; i < count; i++)
            {
                components[i] = pool.Get();
                if (parent != null)
                {
                    components[i].transform.SetParent(parent, false);
                }
            }

            return components;
        }

        /// <summary>
        /// 批量归还组件
        /// Return components in batch
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="components">组件数组 / Component array</param>
        public static void ReturnMany<T>(this IObjectPool<T> pool, params T[] components) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (components == null || components.Length == 0)
                return;

            foreach (var component in components)
            {
                if (component != null)
                {
                    pool.Return(component);
                }
            }
        }

        #endregion

        #region 条件操作扩展 Conditional Operation Extensions

        /// <summary>
        /// 尝试获取组件
        /// Try to get component
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="component">输出组件 / Output component</param>
        /// <returns>是否成功获取 / Whether successfully got</returns>
        public static bool TryGet<T>(this IObjectPool<T> pool, out T component) where T : Component
        {
            component = null;

            if (pool == null)
                return false;

            try
            {
                component = pool.Get();
                return component != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全归还组件（忽略异常）
        /// Safely return component (ignore exceptions)
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="component">组件实例 / Component instance</param>
        /// <returns>是否成功归还 / Whether successfully returned</returns>
        public static bool TryReturn<T>(this IObjectPool<T> pool, T component) where T : Component
        {
            if (pool == null || component == null)
                return false;

            try
            {
                pool.Return(component);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region GameObject 关联操作 GameObject Related Operations

        /// <summary>
        /// 获取组件的 GameObject
        /// Get component's GameObject
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <returns>GameObject 实例 / GameObject instance</returns>
        public static GameObject GetGameObject<T>(this IObjectPool<T> pool) where T : Component
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var component = pool.Get();
            return component.gameObject;
        }

        /// <summary>
        /// 通过 GameObject 归还组件
        /// Return component by GameObject
        /// </summary>
        /// <typeparam name="T">组件类型 / Component type</typeparam>
        /// <param name="pool">组件池 / Component pool</param>
        /// <param name="gameObject">GameObject 实例 / GameObject instance</param>
        /// <returns>是否成功归还 / Whether successfully returned</returns>
        public static bool ReturnFromGameObject<T>(this IObjectPool<T> pool, GameObject gameObject) where T : Component
        {
            if (pool == null || gameObject == null)
                return false;

            var component = gameObject.GetComponent<T>();
            if (component == null)
                return false;

            try
            {
                pool.Return(component);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    #region 辅助类 Helper Classes

    /// <summary>
    /// 组件池归还辅助组件（用于延迟归还）
    /// Component Pool Return Helper Component (for delayed return)
    /// </summary>
    internal class ComponentPoolReturnHelper : MonoBehaviour
    {
        private System.Collections.IEnumerator _returnCoroutine;

        /// <summary>
        /// 延迟归还
        /// Return after delay
        /// </summary>
        public void ReturnAfter<T>(IObjectPool<T> pool, T component, float delay) where T : Component
        {
            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
            }

            _returnCoroutine = ReturnCoroutine(pool, component, delay);
            StartCoroutine(_returnCoroutine);
        }

        private System.Collections.IEnumerator ReturnCoroutine<T>(IObjectPool<T> pool, T component, float delay) where T : Component
        {
            yield return new WaitForSeconds(delay);

            if (pool != null && component != null)
            {
                pool.Return(component);
            }

            _returnCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
                _returnCoroutine = null;
            }
        }
    }

    #endregion
}
