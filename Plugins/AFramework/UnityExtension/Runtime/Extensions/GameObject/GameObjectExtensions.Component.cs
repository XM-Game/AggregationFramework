// ==========================================================
// 文件名：GameObjectExtensions.Component.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// GameObject 组件操作扩展方法
    /// </summary>
    public static partial class GameObjectExtensions
    {
        #region 获取或添加组件

        /// <summary>
        /// 获取组件，如果不存在则添加
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件实例</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// 获取组件，如果不存在则添加 (非泛型版本)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="type">组件类型</param>
        /// <returns>组件实例</returns>
        public static Component GetOrAddComponent(this GameObject gameObject, Type type)
        {
            var component = gameObject.GetComponent(type);
            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }
            return component;
        }

        #endregion

        #region 安全获取组件

        /// <summary>
        /// 尝试获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="component">获取到的组件</param>
        /// <returns>如果获取成功返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponent<T>();
            return component != null;
        }

        /// <summary>
        /// 获取组件，如果不存在返回 null (与 GetComponent 相同，但更明确语义)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件实例或 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentOrNull<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// 获取组件，如果不存在抛出异常
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件实例</returns>
        /// <exception cref="MissingComponentException">组件不存在时抛出</exception>
        public static T GetComponentOrThrow<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                throw new MissingComponentException($"GameObject '{gameObject.name}' 缺少组件 '{typeof(T).Name}'");
            }
            return component;
        }

        #endregion

        #region 检查组件

        /// <summary>
        /// 检查是否具有指定组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果具有组件返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// 检查是否具有指定组件 (非泛型版本)
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="type">组件类型</param>
        /// <returns>如果具有组件返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponent(type) != null;
        }

        /// <summary>
        /// 检查是否具有所有指定组件
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="types">组件类型数组</param>
        /// <returns>如果具有所有组件返回 true</returns>
        public static bool HasAllComponents(this GameObject gameObject, params Type[] types)
        {
            foreach (var type in types)
            {
                if (gameObject.GetComponent(type) == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查是否具有任意一个指定组件
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="types">组件类型数组</param>
        /// <returns>如果具有任意一个组件返回 true</returns>
        public static bool HasAnyComponent(this GameObject gameObject, params Type[] types)
        {
            foreach (var type in types)
            {
                if (gameObject.GetComponent(type) != null)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 移除组件

        /// <summary>
        /// 移除指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="immediate">是否立即销毁</param>
        /// <returns>如果移除成功返回 true</returns>
        public static bool RemoveComponent<T>(this GameObject gameObject, bool immediate = false) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null) return false;

            if (immediate)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
            else
            {
                UnityEngine.Object.Destroy(component);
            }
            return true;
        }

        /// <summary>
        /// 移除所有指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="immediate">是否立即销毁</param>
        /// <returns>移除的组件数量</returns>
        public static int RemoveComponents<T>(this GameObject gameObject, bool immediate = false) where T : Component
        {
            var components = gameObject.GetComponents<T>();
            foreach (var component in components)
            {
                if (immediate)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                }
                else
                {
                    UnityEngine.Object.Destroy(component);
                }
            }
            return components.Length;
        }

        /// <summary>
        /// 安全移除组件 (根据运行模式选择销毁方式)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>如果移除成功返回 true</returns>
        public static bool RemoveComponentSafe<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null) return false;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(component);
                return true;
            }
#endif
            UnityEngine.Object.Destroy(component);
            return true;
        }

        #endregion

        #region 批量添加组件

        /// <summary>
        /// 添加多个组件
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="types">组件类型数组</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject AddComponents(this GameObject gameObject, params Type[] types)
        {
            foreach (var type in types)
            {
                gameObject.AddComponent(type);
            }
            return gameObject;
        }

        /// <summary>
        /// 添加组件并返回组件实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="component">添加的组件实例</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject AddComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.AddComponent<T>();
            return gameObject;
        }

        #endregion

        #region 组件启用/禁用

        /// <summary>
        /// 启用指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型 (必须是 Behaviour)</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject EnableComponent<T>(this GameObject gameObject) where T : Behaviour
        {
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                component.enabled = true;
            }
            return gameObject;
        }

        /// <summary>
        /// 禁用指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型 (必须是 Behaviour)</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject DisableComponent<T>(this GameObject gameObject) where T : Behaviour
        {
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                component.enabled = false;
            }
            return gameObject;
        }

        /// <summary>
        /// 切换指定类型组件的启用状态
        /// </summary>
        /// <typeparam name="T">组件类型 (必须是 Behaviour)</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject ToggleComponent<T>(this GameObject gameObject) where T : Behaviour
        {
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                component.enabled = !component.enabled;
            }
            return gameObject;
        }

        /// <summary>
        /// 设置指定类型组件的启用状态
        /// </summary>
        /// <typeparam name="T">组件类型 (必须是 Behaviour)</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="enabled">是否启用</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject SetComponentEnabled<T>(this GameObject gameObject, bool enabled) where T : Behaviour
        {
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                component.enabled = enabled;
            }
            return gameObject;
        }

        #endregion

        #region 在子级/父级中获取组件

        /// <summary>
        /// 仅在子级中获取组件 (不包含自身)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件实例或 null</returns>
        public static T GetComponentInChildrenOnly<T>(this GameObject gameObject) where T : Component
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var component = gameObject.transform.GetChild(i).GetComponentInChildren<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        /// <summary>
        /// 仅在子级中获取所有组件 (不包含自身)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件列表</returns>
        public static List<T> GetComponentsInChildrenOnly<T>(this GameObject gameObject) where T : Component
        {
            var result = new List<T>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var components = gameObject.transform.GetChild(i).GetComponentsInChildren<T>();
                result.AddRange(components);
            }
            return result;
        }

        /// <summary>
        /// 仅在父级中获取组件 (不包含自身)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件实例或 null</returns>
        public static T GetComponentInParentOnly<T>(this GameObject gameObject) where T : Component
        {
            var parent = gameObject.transform.parent;
            return parent != null ? parent.GetComponentInParent<T>() : null;
        }

        /// <summary>
        /// 在指定深度内获取子级组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="maxDepth">最大深度</param>
        /// <returns>组件实例或 null</returns>
        public static T GetComponentInChildrenWithDepth<T>(this GameObject gameObject, int maxDepth) where T : Component
        {
            return GetComponentInChildrenWithDepthInternal<T>(gameObject.transform, maxDepth, 0);
        }

        private static T GetComponentInChildrenWithDepthInternal<T>(Transform transform, int maxDepth, int currentDepth) where T : Component
        {
            if (currentDepth > maxDepth) return null;

            var component = transform.GetComponent<T>();
            if (component != null) return component;

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = GetComponentInChildrenWithDepthInternal<T>(transform.GetChild(i), maxDepth, currentDepth + 1);
                if (result != null) return result;
            }

            return null;
        }

        #endregion

        #region 组件计数

        /// <summary>
        /// 获取指定类型组件的数量
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentCount<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponents<T>().Length;
        }

        /// <summary>
        /// 获取所有组件的数量
        /// </summary>
        /// <param name="gameObject">目标 GameObject</param>
        /// <returns>组件数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetComponentCount(this GameObject gameObject)
        {
            return gameObject.GetComponents<Component>().Length;
        }

        #endregion

        #region 组件操作

        /// <summary>
        /// 对指定类型的组件执行操作
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject WithComponent<T>(this GameObject gameObject, Action<T> action) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component != null)
            {
                action(component);
            }
            return gameObject;
        }

        /// <summary>
        /// 对所有指定类型的组件执行操作
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">目标 GameObject</param>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static GameObject WithComponents<T>(this GameObject gameObject, Action<T> action) where T : Component
        {
            var components = gameObject.GetComponents<T>();
            foreach (var component in components)
            {
                action(component);
            }
            return gameObject;
        }

        /// <summary>
        /// 复制组件到另一个 GameObject
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="gameObject">源 GameObject</param>
        /// <param name="target">目标 GameObject</param>
        /// <returns>复制的组件实例</returns>
        public static T CopyComponentTo<T>(this GameObject gameObject, GameObject target) where T : Component
        {
            var source = gameObject.GetComponent<T>();
            if (source == null) return null;

            var copy = target.AddComponent<T>();
            var type = typeof(T);
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                field.SetValue(copy, field.GetValue(source));
            }
            return copy;
        }

        #endregion
    }
}
