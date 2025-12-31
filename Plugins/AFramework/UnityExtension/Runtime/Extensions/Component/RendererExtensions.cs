// ==========================================================
// 文件名：RendererExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System.Collections.Generic
// ==========================================================

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Renderer 扩展方法
    /// <para>提供 Renderer 的可见性和材质操作扩展</para>
    /// </summary>
    public static class RendererExtensions
    {
        #region 可见性控制

        /// <summary>
        /// 显示渲染器
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Show<T>(this T renderer) where T : Renderer
        {
            renderer.enabled = true;
            return renderer;
        }

        /// <summary>
        /// 隐藏渲染器
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Hide<T>(this T renderer) where T : Renderer
        {
            renderer.enabled = false;
            return renderer;
        }

        /// <summary>
        /// 切换渲染器可见性
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ToggleVisibility<T>(this T renderer) where T : Renderer
        {
            renderer.enabled = !renderer.enabled;
            return renderer;
        }

        /// <summary>
        /// 设置渲染器可见性
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="visible">是否可见</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetVisible<T>(this T renderer, bool visible) where T : Renderer
        {
            renderer.enabled = visible;
            return renderer;
        }

        /// <summary>
        /// 检查渲染器是否可见
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>如果可见返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVisible(this Renderer renderer)
        {
            return renderer.enabled && renderer.isVisible;
        }

        #endregion

        #region 材质操作

        /// <summary>
        /// 设置主材质
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="material">材质</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetMaterial<T>(this T renderer, Material material) where T : Renderer
        {
            renderer.material = material;
            return renderer;
        }

        /// <summary>
        /// 设置共享材质
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="material">材质</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetSharedMaterial<T>(this T renderer, Material material) where T : Renderer
        {
            renderer.sharedMaterial = material;
            return renderer;
        }

        /// <summary>
        /// 设置材质颜色
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="color">颜色</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetColor<T>(this T renderer, Color color) where T : Renderer
        {
            renderer.material.color = color;
            return renderer;
        }

        /// <summary>
        /// 设置材质颜色 (使用属性名)
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="color">颜色</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetColor<T>(this T renderer, string propertyName, Color color) where T : Renderer
        {
            renderer.material.SetColor(propertyName, color);
            return renderer;
        }

        /// <summary>
        /// 获取材质颜色
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>材质颜色</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetColor(this Renderer renderer)
        {
            return renderer.material.color;
        }

        /// <summary>
        /// 设置材质透明度
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="alpha">透明度 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static T SetAlpha<T>(this T renderer, float alpha) where T : Renderer
        {
            var color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
            return renderer;
        }

        /// <summary>
        /// 获取材质透明度
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>透明度</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAlpha(this Renderer renderer)
        {
            return renderer.material.color.a;
        }

        #endregion

        #region 材质属性

        /// <summary>
        /// 设置材质浮点属性
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetFloat<T>(this T renderer, string propertyName, float value) where T : Renderer
        {
            renderer.material.SetFloat(propertyName, value);
            return renderer;
        }

        /// <summary>
        /// 设置材质整数属性
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetInt<T>(this T renderer, string propertyName, int value) where T : Renderer
        {
            renderer.material.SetInt(propertyName, value);
            return renderer;
        }

        /// <summary>
        /// 设置材质纹理
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="texture">纹理</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetTexture<T>(this T renderer, string propertyName, Texture texture) where T : Renderer
        {
            renderer.material.SetTexture(propertyName, texture);
            return renderer;
        }

        /// <summary>
        /// 设置主纹理
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="texture">纹理</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetMainTexture<T>(this T renderer, Texture texture) where T : Renderer
        {
            renderer.material.mainTexture = texture;
            return renderer;
        }

        #endregion

        #region 排序层

        /// <summary>
        /// 设置排序层
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="sortingLayerName">排序层名称</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetSortingLayer<T>(this T renderer, string sortingLayerName) where T : Renderer
        {
            renderer.sortingLayerName = sortingLayerName;
            return renderer;
        }

        /// <summary>
        /// 设置排序层 ID
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="sortingLayerID">排序层 ID</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetSortingLayerID<T>(this T renderer, int sortingLayerID) where T : Renderer
        {
            renderer.sortingLayerID = sortingLayerID;
            return renderer;
        }

        /// <summary>
        /// 设置排序顺序
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="sortingOrder">排序顺序</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SetSortingOrder<T>(this T renderer, int sortingOrder) where T : Renderer
        {
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        #endregion

        #region 边界

        /// <summary>
        /// 获取世界空间边界
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>世界空间边界</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds GetBounds(this Renderer renderer)
        {
            return renderer.bounds;
        }

        /// <summary>
        /// 获取边界中心点
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>边界中心点</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetBoundsCenter(this Renderer renderer)
        {
            return renderer.bounds.center;
        }

        /// <summary>
        /// 获取边界尺寸
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <returns>边界尺寸</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetBoundsSize(this Renderer renderer)
        {
            return renderer.bounds.size;
        }

        /// <summary>
        /// 检查点是否在边界内
        /// </summary>
        /// <param name="renderer">目标渲染器</param>
        /// <param name="point">点</param>
        /// <returns>如果在边界内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(this Renderer renderer, Vector3 point)
        {
            return renderer.bounds.Contains(point);
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 显示所有渲染器
        /// </summary>
        /// <param name="renderers">渲染器集合</param>
        public static void ShowAll<T>(this IEnumerable<T> renderers) where T : Renderer
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        /// <summary>
        /// 隐藏所有渲染器
        /// </summary>
        /// <param name="renderers">渲染器集合</param>
        public static void HideAll<T>(this IEnumerable<T> renderers) where T : Renderer
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
        }

        /// <summary>
        /// 设置所有渲染器的颜色
        /// </summary>
        /// <param name="renderers">渲染器集合</param>
        /// <param name="color">颜色</param>
        public static void SetColorAll<T>(this IEnumerable<T> renderers, Color color) where T : Renderer
        {
            foreach (var renderer in renderers)
            {
                renderer.material.color = color;
            }
        }

        /// <summary>
        /// 设置所有渲染器的透明度
        /// </summary>
        /// <param name="renderers">渲染器集合</param>
        /// <param name="alpha">透明度</param>
        public static void SetAlphaAll<T>(this IEnumerable<T> renderers, float alpha) where T : Renderer
        {
            foreach (var renderer in renderers)
            {
                var color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }

        #endregion
    }
}
