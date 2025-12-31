// ==========================================================
// 文件名：RaycastHitExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// RaycastHit 扩展方法
    /// <para>提供射线检测结果的实用功能扩展</para>
    /// </summary>
    public static class RaycastHitExtensions
    {
        #region 组件获取

        /// <summary>
        /// 获取命中对象的组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this RaycastHit hit) where T : Component
        {
            return hit.collider != null ? hit.collider.GetComponent<T>() : null;
        }

        /// <summary>
        /// 尝试获取命中对象的组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this RaycastHit hit, out T component) where T : Component
        {
            if (hit.collider != null)
            {
                return hit.collider.TryGetComponent(out component);
            }
            component = null;
            return false;
        }

        /// <summary>
        /// 获取命中对象的 GameObject
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetGameObject(this RaycastHit hit)
        {
            return hit.collider != null ? hit.collider.gameObject : null;
        }

        /// <summary>
        /// 获取命中对象的 Transform
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetTransform(this RaycastHit hit)
        {
            return hit.collider != null ? hit.collider.transform : null;
        }

        /// <summary>
        /// 获取命中对象的 Rigidbody
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rigidbody GetRigidbody(this RaycastHit hit)
        {
            return hit.rigidbody;
        }

        #endregion

        #region 检查

        /// <summary>
        /// 检查是否命中有效对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this RaycastHit hit)
        {
            return hit.collider != null;
        }

        /// <summary>
        /// 检查命中对象是否有指定标签
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTag(this RaycastHit hit, string tag)
        {
            return hit.collider != null && hit.collider.CompareTag(tag);
        }

        /// <summary>
        /// 检查命中对象是否在指定层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayer(this RaycastHit hit, int layer)
        {
            return hit.collider != null && hit.collider.gameObject.layer == layer;
        }

        /// <summary>
        /// 检查命中对象是否在指定层掩码中
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayerMask(this RaycastHit hit, LayerMask layerMask)
        {
            return hit.collider != null && ((1 << hit.collider.gameObject.layer) & layerMask) != 0;
        }

        /// <summary>
        /// 检查命中对象是否有指定组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this RaycastHit hit) where T : Component
        {
            return hit.collider != null && hit.collider.GetComponent<T>() != null;
        }

        #endregion

        #region 反射计算

        /// <summary>
        /// 获取反射方向
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="inDirection">入射方向</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetReflectDirection(this RaycastHit hit, Vector3 inDirection)
        {
            return Vector3.Reflect(inDirection.normalized, hit.normal);
        }

        /// <summary>
        /// 获取反射射线
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="inDirection">入射方向</param>
        public static Ray GetReflectRay(this RaycastHit hit, Vector3 inDirection)
        {
            Vector3 reflectDir = Vector3.Reflect(inDirection.normalized, hit.normal);
            return new Ray(hit.point, reflectDir);
        }

        #endregion

        #region 位置偏移

        /// <summary>
        /// 获取沿法线偏移的点
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="offset">偏移距离</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPointAlongNormal(this RaycastHit hit, float offset)
        {
            return hit.point + hit.normal * offset;
        }

        /// <summary>
        /// 获取稍微偏离表面的点 (避免穿透)
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="skinWidth">皮肤宽度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetSafePoint(this RaycastHit hit, float skinWidth = 0.01f)
        {
            return hit.point + hit.normal * skinWidth;
        }

        #endregion

        #region 调试

        /// <summary>
        /// 绘制命中点和法线 (调试用)
        /// </summary>
        public static void DrawDebug(this RaycastHit hit, float normalLength = 1f, float duration = 0f)
        {
            if (!hit.IsValid()) return;

            Debug.DrawLine(hit.point, hit.point + hit.normal * normalLength, Color.green, duration);
            Debug.DrawLine(hit.point - Vector3.right * 0.1f, hit.point + Vector3.right * 0.1f, Color.red, duration);
            Debug.DrawLine(hit.point - Vector3.up * 0.1f, hit.point + Vector3.up * 0.1f, Color.red, duration);
            Debug.DrawLine(hit.point - Vector3.forward * 0.1f, hit.point + Vector3.forward * 0.1f, Color.red, duration);
        }

        #endregion
    }

    /// <summary>
    /// RaycastHit2D 扩展方法
    /// <para>提供 2D 射线检测结果的实用功能扩展</para>
    /// </summary>
    public static class RaycastHit2DExtensions
    {
        #region 组件获取

        /// <summary>
        /// 获取命中对象的组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this RaycastHit2D hit) where T : Component
        {
            return hit.collider != null ? hit.collider.GetComponent<T>() : null;
        }

        /// <summary>
        /// 尝试获取命中对象的组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this RaycastHit2D hit, out T component) where T : Component
        {
            if (hit.collider != null)
            {
                return hit.collider.TryGetComponent(out component);
            }
            component = null;
            return false;
        }

        /// <summary>
        /// 获取命中对象的 GameObject
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject GetGameObject(this RaycastHit2D hit)
        {
            return hit.collider != null ? hit.collider.gameObject : null;
        }

        /// <summary>
        /// 获取命中对象的 Transform
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetTransform(this RaycastHit2D hit)
        {
            return hit.collider != null ? hit.collider.transform : null;
        }

        /// <summary>
        /// 获取命中对象的 Rigidbody2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rigidbody2D GetRigidbody(this RaycastHit2D hit)
        {
            return hit.rigidbody;
        }

        #endregion

        #region 检查

        /// <summary>
        /// 检查是否命中有效对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this RaycastHit2D hit)
        {
            return hit.collider != null;
        }

        /// <summary>
        /// 检查命中对象是否有指定标签
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTag(this RaycastHit2D hit, string tag)
        {
            return hit.collider != null && hit.collider.CompareTag(tag);
        }

        /// <summary>
        /// 检查命中对象是否在指定层
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayer(this RaycastHit2D hit, int layer)
        {
            return hit.collider != null && hit.collider.gameObject.layer == layer;
        }

        /// <summary>
        /// 检查命中对象是否在指定层掩码中
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInLayerMask(this RaycastHit2D hit, LayerMask layerMask)
        {
            return hit.collider != null && ((1 << hit.collider.gameObject.layer) & layerMask) != 0;
        }

        /// <summary>
        /// 检查命中对象是否有指定组件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this RaycastHit2D hit) where T : Component
        {
            return hit.collider != null && hit.collider.GetComponent<T>() != null;
        }

        #endregion

        #region 反射计算

        /// <summary>
        /// 获取反射方向
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="inDirection">入射方向</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetReflectDirection(this RaycastHit2D hit, Vector2 inDirection)
        {
            return Vector2.Reflect(inDirection.normalized, hit.normal);
        }

        #endregion

        #region 位置偏移

        /// <summary>
        /// 获取沿法线偏移的点
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="offset">偏移距离</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPointAlongNormal(this RaycastHit2D hit, float offset)
        {
            return hit.point + hit.normal * offset;
        }

        /// <summary>
        /// 获取稍微偏离表面的点 (避免穿透)
        /// </summary>
        /// <param name="hit">射线命中信息</param>
        /// <param name="skinWidth">皮肤宽度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSafePoint(this RaycastHit2D hit, float skinWidth = 0.01f)
        {
            return hit.point + hit.normal * skinWidth;
        }

        #endregion

        #region 调试

        /// <summary>
        /// 绘制命中点和法线 (调试用)
        /// </summary>
        public static void DrawDebug(this RaycastHit2D hit, float normalLength = 1f, float duration = 0f)
        {
            if (!hit.IsValid()) return;

            Debug.DrawLine(hit.point, hit.point + hit.normal * normalLength, Color.green, duration);
            Debug.DrawLine(hit.point - Vector2.right * 0.1f, hit.point + Vector2.right * 0.1f, Color.red, duration);
            Debug.DrawLine(hit.point - Vector2.up * 0.1f, hit.point + Vector2.up * 0.1f, Color.red, duration);
        }

        #endregion
    }
}
