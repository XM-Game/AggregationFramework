// ==========================================================
// 文件名：ComponentExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Component 扩展方法
    /// <para>提供 Component 的基础操作扩展</para>
    /// </summary>
    public static class ComponentExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查组件是否为 null 或已被销毁
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>如果为 null 或已销毁返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrDestroyed(this Component component)
        {
            return component == null;
        }

        /// <summary>
        /// 检查组件是否有效 (非 null 且未被销毁)
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>如果有效返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Component component)
        {
            return component != null;
        }

        #endregion

        #region 获取或添加组件

        /// <summary>
        /// 获取组件，如果不存在则添加
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>组件实例</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            var result = component.GetComponent<T>();
            if (result == null)
            {
                result = component.gameObject.AddComponent<T>();
            }
            return result;
        }

        /// <summary>
        /// 获取组件，如果不存在则添加 (非泛型版本)
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="type">组件类型</param>
        /// <returns>组件实例</returns>
        public static Component GetOrAddComponent(this Component component, Type type)
        {
            var result = component.GetComponent(type);
            if (result == null)
            {
                result = component.gameObject.AddComponent(type);
            }
            return result;
        }

        #endregion

        #region 安全获取组件

        /// <summary>
        /// 尝试获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <param name="result">获取到的组件</param>
        /// <returns>如果获取成功返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponent<T>();
            return result != null;
        }

        /// <summary>
        /// 获取组件，如果不存在返回 null
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>组件实例或 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentOrNull<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>();
        }

        /// <summary>
        /// 获取组件，如果不存在抛出异常
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>组件实例</returns>
        /// <exception cref="MissingComponentException">组件不存在时抛出</exception>
        public static T GetComponentOrThrow<T>(this Component component) where T : Component
        {
            var result = component.GetComponent<T>();
            if (result == null)
            {
                throw new MissingComponentException($"GameObject '{component.gameObject.name}' 缺少组件 '{typeof(T).Name}'");
            }
            return result;
        }

        #endregion

        #region 检查组件

        /// <summary>
        /// 检查是否具有指定组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>如果具有组件返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        /// <summary>
        /// 检查是否具有指定组件 (非泛型版本)
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="type">组件类型</param>
        /// <returns>如果具有组件返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent(this Component component, Type type)
        {
            return component.GetComponent(type) != null;
        }

        #endregion

        #region 移除组件

        /// <summary>
        /// 移除指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <param name="immediate">是否立即销毁</param>
        /// <returns>如果移除成功返回 true</returns>
        public static bool RemoveComponent<T>(this Component component, bool immediate = false) where T : Component
        {
            var target = component.GetComponent<T>();
            if (target == null) return false;

            if (immediate)
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
            else
            {
                UnityEngine.Object.Destroy(target);
            }
            return true;
        }

        /// <summary>
        /// 安全移除组件 (根据运行模式选择销毁方式)
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>如果移除成功返回 true</returns>
        public static bool RemoveComponentSafe<T>(this Component component) where T : Component
        {
            var target = component.GetComponent<T>();
            if (target == null) return false;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(target);
                return true;
            }
#endif
            UnityEngine.Object.Destroy(target);
            return true;
        }

        #endregion

        #region Transform 快捷访问

        /// <summary>
        /// 获取世界空间位置
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>世界空间位置</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPosition(this Component component)
        {
            return component.transform.position;
        }

        /// <summary>
        /// 设置世界空间位置
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="position">新位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetPosition<T>(this T component, Vector3 position) where T : Component
        {
            component.transform.position = position;
            return component;
        }

        /// <summary>
        /// 获取本地空间位置
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>本地空间位置</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLocalPosition(this Component component)
        {
            return component.transform.localPosition;
        }

        /// <summary>
        /// 设置本地空间位置
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="position">新位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetLocalPosition<T>(this T component, Vector3 position) where T : Component
        {
            component.transform.localPosition = position;
            return component;
        }

        /// <summary>
        /// 获取世界空间旋转
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>世界空间旋转</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRotation(this Component component)
        {
            return component.transform.rotation;
        }

        /// <summary>
        /// 设置世界空间旋转
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="rotation">新旋转</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetRotation<T>(this T component, Quaternion rotation) where T : Component
        {
            component.transform.rotation = rotation;
            return component;
        }

        /// <summary>
        /// 获取本地空间缩放
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>本地空间缩放</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLocalScale(this Component component)
        {
            return component.transform.localScale;
        }

        /// <summary>
        /// 设置本地空间缩放
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="scale">新缩放</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetLocalScale<T>(this T component, Vector3 scale) where T : Component
        {
            component.transform.localScale = scale;
            return component;
        }

        #endregion

        #region 父级操作

        /// <summary>
        /// 设置父级
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="parent">父级 Transform</param>
        /// <param name="worldPositionStays">是否保持世界空间位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetParent<T>(this T component, Transform parent, bool worldPositionStays = true) where T : Component
        {
            component.transform.SetParent(parent, worldPositionStays);
            return component;
        }

        /// <summary>
        /// 获取父级组件
        /// </summary>
        /// <typeparam name="TComponent">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <returns>父级组件，如果没有返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent GetParentComponent<TComponent>(this Component component) where TComponent : Component
        {
            var parent = component.transform.parent;
            return parent != null ? parent.GetComponent<TComponent>() : null;
        }

        #endregion

        #region 层级操作

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="layer">层级索引</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetLayer<T>(this T component, int layer) where T : Component
        {
            component.gameObject.layer = layer;
            return component;
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="layerName">层级名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetLayer<T>(this T component, string layerName) where T : Component
        {
            component.gameObject.layer = LayerMask.NameToLayer(layerName);
            return component;
        }

        /// <summary>
        /// 检查是否在指定层级
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="layer">层级索引</param>
        /// <returns>如果在指定层级返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayer(this Component component, int layer)
        {
            return component.gameObject.layer == layer;
        }

        /// <summary>
        /// 检查是否在指定层级掩码中
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="layerMask">层级掩码</param>
        /// <returns>如果在层级掩码中返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayerMask(this Component component, LayerMask layerMask)
        {
            return ((1 << component.gameObject.layer) & layerMask.value) != 0;
        }

        #endregion

        #region 标签操作

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="tag">标签名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetTag<T>(this T component, string tag) where T : Component
        {
            component.gameObject.tag = tag;
            return component;
        }

        /// <summary>
        /// 检查是否具有指定标签
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="tag">标签名称</param>
        /// <returns>如果具有指定标签返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTag(this Component component, string tag)
        {
            return component.CompareTag(tag);
        }

        #endregion

        #region 激活状态

        /// <summary>
        /// 激活 GameObject
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Activate<T>(this T component) where T : Component
        {
            component.gameObject.SetActive(true);
            return component;
        }

        /// <summary>
        /// 停用 GameObject
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deactivate<T>(this T component) where T : Component
        {
            component.gameObject.SetActive(false);
            return component;
        }

        /// <summary>
        /// 设置 GameObject 激活状态
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="active">是否激活</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetActive<T>(this T component, bool active) where T : Component
        {
            component.gameObject.SetActive(active);
            return component;
        }

        /// <summary>
        /// 检查 GameObject 是否激活
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>如果激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive(this Component component)
        {
            return component.gameObject.activeSelf;
        }

        /// <summary>
        /// 检查 GameObject 是否在层级中激活
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <returns>如果在层级中激活返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy(this Component component)
        {
            return component.gameObject.activeInHierarchy;
        }

        #endregion

        #region 销毁操作

        /// <summary>
        /// 销毁组件
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="delay">延迟时间 (秒)</param>
        public static void Destroy(this Component component, float delay = 0f)
        {
            if (delay > 0f)
            {
                UnityEngine.Object.Destroy(component, delay);
            }
            else
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        /// <summary>
        /// 销毁 GameObject
        /// </summary>
        /// <param name="component">目标组件</param>
        /// <param name="delay">延迟时间 (秒)</param>
        public static void DestroyGameObject(this Component component, float delay = 0f)
        {
            if (delay > 0f)
            {
                UnityEngine.Object.Destroy(component.gameObject, delay);
            }
            else
            {
                UnityEngine.Object.Destroy(component.gameObject);
            }
        }

        /// <summary>
        /// 安全销毁组件 (根据运行模式选择销毁方式)
        /// </summary>
        /// <param name="component">目标组件</param>
        public static void DestroySafe(this Component component)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(component);
                return;
            }
#endif
            UnityEngine.Object.Destroy(component);
        }

        #endregion

        #region 子级查找

        /// <summary>
        /// 在子级中查找组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <param name="name">子级名称</param>
        /// <returns>找到的组件，如果没找到返回 null</returns>
        public static T FindComponentInChild<T>(this Component component, string name) where T : Component
        {
            var child = component.transform.Find(name);
            return child != null ? child.GetComponent<T>() : null;
        }

        /// <summary>
        /// 递归在子级中查找组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="component">目标组件</param>
        /// <param name="name">子级名称</param>
        /// <returns>找到的组件，如果没找到返回 null</returns>
        public static T FindComponentInChildDeep<T>(this Component component, string name) where T : Component
        {
            var child = component.transform.FindDeep(name);
            return child != null ? child.GetComponent<T>() : null;
        }

        #endregion
    }
}
