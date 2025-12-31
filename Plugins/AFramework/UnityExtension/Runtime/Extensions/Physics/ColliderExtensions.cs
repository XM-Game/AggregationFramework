// ==========================================================
// 文件名：ColliderExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Collider 扩展方法
    /// <para>提供 3D 碰撞体的实用功能扩展</para>
    /// </summary>
    public static class ColliderExtensions
    {
        #region 边界操作

        /// <summary>
        /// 获取世界空间边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds GetWorldBounds(this Collider collider)
        {
            return collider.bounds;
        }

        /// <summary>
        /// 获取边界中心点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetCenter(this Collider collider)
        {
            return collider.bounds.center;
        }

        /// <summary>
        /// 获取边界尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetSize(this Collider collider)
        {
            return collider.bounds.size;
        }

        /// <summary>
        /// 获取边界半尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetExtents(this Collider collider)
        {
            return collider.bounds.extents;
        }

        #endregion

        #region 点检测

        /// <summary>
        /// 检查点是否在碰撞体内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(this Collider collider, Vector3 point)
        {
            return collider.bounds.Contains(point);
        }

        /// <summary>
        /// 获取碰撞体上最近的点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetClosestPoint(this Collider collider, Vector3 point)
        {
            return collider.ClosestPoint(point);
        }

        /// <summary>
        /// 获取碰撞体边界上最近的点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetClosestPointOnBounds(this Collider collider, Vector3 point)
        {
            return collider.ClosestPointOnBounds(point);
        }

        /// <summary>
        /// 获取到点的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistanceToPoint(this Collider collider, Vector3 point)
        {
            Vector3 closest = collider.ClosestPoint(point);
            return Vector3.Distance(point, closest);
        }

        #endregion

        #region 随机点

        /// <summary>
        /// 获取边界内的随机点
        /// </summary>
        public static Vector3 GetRandomPointInBounds(this Collider collider)
        {
            Bounds bounds = collider.bounds;
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
        }

        /// <summary>
        /// 获取碰撞体内的随机点 (通过多次采样)
        /// </summary>
        public static Vector3 GetRandomPointInside(this Collider collider, int maxAttempts = 100)
        {
            Bounds bounds = collider.bounds;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 point = new Vector3(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                    UnityEngine.Random.Range(bounds.min.z, bounds.max.z));

                // 检查点是否真的在碰撞体内
                Vector3 closest = collider.ClosestPoint(point);
                if ((closest - point).sqrMagnitude < 0.0001f)
                {
                    return point;
                }
            }

            // 如果找不到内部点，返回中心
            return bounds.center;
        }

        #endregion

        #region 碰撞体类型检查

        /// <summary>
        /// 检查是否为 BoxCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoxCollider(this Collider collider)
        {
            return collider is BoxCollider;
        }

        /// <summary>
        /// 检查是否为 SphereCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSphereCollider(this Collider collider)
        {
            return collider is SphereCollider;
        }

        /// <summary>
        /// 检查是否为 CapsuleCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCapsuleCollider(this Collider collider)
        {
            return collider is CapsuleCollider;
        }

        /// <summary>
        /// 检查是否为 MeshCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMeshCollider(this Collider collider)
        {
            return collider is MeshCollider;
        }

        /// <summary>
        /// 尝试获取为 BoxCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsBox(this Collider collider, out BoxCollider boxCollider)
        {
            boxCollider = collider as BoxCollider;
            return boxCollider != null;
        }

        /// <summary>
        /// 尝试获取为 SphereCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsSphere(this Collider collider, out SphereCollider sphereCollider)
        {
            sphereCollider = collider as SphereCollider;
            return sphereCollider != null;
        }

        /// <summary>
        /// 尝试获取为 CapsuleCollider
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsCapsule(this Collider collider, out CapsuleCollider capsuleCollider)
        {
            capsuleCollider = collider as CapsuleCollider;
            return capsuleCollider != null;
        }

        #endregion

        #region 启用/禁用

        /// <summary>
        /// 切换启用状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(this Collider collider)
        {
            collider.enabled = !collider.enabled;
        }

        /// <summary>
        /// 设置为触发器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAsTrigger(this Collider collider, bool isTrigger = true)
        {
            collider.isTrigger = isTrigger;
        }

        #endregion

        #region BoxCollider 扩展

        /// <summary>
        /// 获取 BoxCollider 的世界空间角点
        /// </summary>
        public static Vector3[] GetWorldCorners(this BoxCollider box)
        {
            Vector3 center = box.center;
            Vector3 extents = box.size * 0.5f;
            Transform t = box.transform;

            Vector3[] corners = new Vector3[8];
            corners[0] = t.TransformPoint(center + new Vector3(-extents.x, -extents.y, -extents.z));
            corners[1] = t.TransformPoint(center + new Vector3(extents.x, -extents.y, -extents.z));
            corners[2] = t.TransformPoint(center + new Vector3(extents.x, -extents.y, extents.z));
            corners[3] = t.TransformPoint(center + new Vector3(-extents.x, -extents.y, extents.z));
            corners[4] = t.TransformPoint(center + new Vector3(-extents.x, extents.y, -extents.z));
            corners[5] = t.TransformPoint(center + new Vector3(extents.x, extents.y, -extents.z));
            corners[6] = t.TransformPoint(center + new Vector3(extents.x, extents.y, extents.z));
            corners[7] = t.TransformPoint(center + new Vector3(-extents.x, extents.y, extents.z));

            return corners;
        }

        /// <summary>
        /// 获取 BoxCollider 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetWorldCenter(this BoxCollider box)
        {
            return box.transform.TransformPoint(box.center);
        }

        /// <summary>
        /// 获取 BoxCollider 的世界空间尺寸
        /// </summary>
        public static Vector3 GetWorldSize(this BoxCollider box)
        {
            Vector3 scale = box.transform.lossyScale;
            return Vector3.Scale(box.size, scale);
        }

        #endregion

        #region SphereCollider 扩展

        /// <summary>
        /// 获取 SphereCollider 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetWorldCenter(this SphereCollider sphere)
        {
            return sphere.transform.TransformPoint(sphere.center);
        }

        /// <summary>
        /// 获取 SphereCollider 的世界空间半径
        /// </summary>
        public static float GetWorldRadius(this SphereCollider sphere)
        {
            Vector3 scale = sphere.transform.lossyScale;
            return sphere.radius * Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
        }

        #endregion

        #region CapsuleCollider 扩展

        /// <summary>
        /// 获取 CapsuleCollider 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetWorldCenter(this CapsuleCollider capsule)
        {
            return capsule.transform.TransformPoint(capsule.center);
        }

        /// <summary>
        /// 获取 CapsuleCollider 的两个端点 (世界空间)
        /// </summary>
        public static (Vector3 point1, Vector3 point2) GetWorldEndPoints(this CapsuleCollider capsule)
        {
            Vector3 center = capsule.center;
            float halfHeight = (capsule.height * 0.5f) - capsule.radius;
            halfHeight = Mathf.Max(0f, halfHeight);

            Vector3 direction = capsule.direction switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                2 => Vector3.forward,
                _ => Vector3.up
            };

            Transform t = capsule.transform;
            Vector3 point1 = t.TransformPoint(center + direction * halfHeight);
            Vector3 point2 = t.TransformPoint(center - direction * halfHeight);

            return (point1, point2);
        }

        #endregion
    }
}
