// ==========================================================
// 文件名：Collider2DExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Collider2D 扩展方法
    /// <para>提供 2D 碰撞体的实用功能扩展</para>
    /// </summary>
    public static class Collider2DExtensions
    {
        #region 边界操作

        /// <summary>
        /// 获取世界空间边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds GetWorldBounds(this Collider2D collider)
        {
            return collider.bounds;
        }

        /// <summary>
        /// 获取边界中心点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetCenter(this Collider2D collider)
        {
            return collider.bounds.center;
        }

        /// <summary>
        /// 获取边界尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSize(this Collider2D collider)
        {
            return collider.bounds.size;
        }

        /// <summary>
        /// 获取边界半尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetExtents(this Collider2D collider)
        {
            return collider.bounds.extents;
        }

        #endregion

        #region 点检测

        /// <summary>
        /// 检查点是否在碰撞体内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(this Collider2D collider, Vector2 point)
        {
            return collider.OverlapPoint(point);
        }

        /// <summary>
        /// 获取碰撞体上最近的点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetClosestPoint(this Collider2D collider, Vector2 point)
        {
            return collider.ClosestPoint(point);
        }

        /// <summary>
        /// 获取到点的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistanceToPoint(this Collider2D collider, Vector2 point)
        {
            Vector2 closest = collider.ClosestPoint(point);
            return Vector2.Distance(point, closest);
        }

        #endregion

        #region 随机点

        /// <summary>
        /// 获取边界内的随机点
        /// </summary>
        public static Vector2 GetRandomPointInBounds(this Collider2D collider)
        {
            Bounds bounds = collider.bounds;
            return new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y));
        }

        /// <summary>
        /// 获取碰撞体内的随机点 (通过多次采样)
        /// </summary>
        public static Vector2 GetRandomPointInside(this Collider2D collider, int maxAttempts = 100)
        {
            Bounds bounds = collider.bounds;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 point = new Vector2(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y));

                if (collider.OverlapPoint(point))
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
        /// 检查是否为 BoxCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoxCollider2D(this Collider2D collider)
        {
            return collider is BoxCollider2D;
        }

        /// <summary>
        /// 检查是否为 CircleCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCircleCollider2D(this Collider2D collider)
        {
            return collider is CircleCollider2D;
        }

        /// <summary>
        /// 检查是否为 CapsuleCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCapsuleCollider2D(this Collider2D collider)
        {
            return collider is CapsuleCollider2D;
        }

        /// <summary>
        /// 检查是否为 PolygonCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPolygonCollider2D(this Collider2D collider)
        {
            return collider is PolygonCollider2D;
        }

        /// <summary>
        /// 尝试获取为 BoxCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsBox(this Collider2D collider, out BoxCollider2D boxCollider)
        {
            boxCollider = collider as BoxCollider2D;
            return boxCollider != null;
        }

        /// <summary>
        /// 尝试获取为 CircleCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsCircle(this Collider2D collider, out CircleCollider2D circleCollider)
        {
            circleCollider = collider as CircleCollider2D;
            return circleCollider != null;
        }

        /// <summary>
        /// 尝试获取为 CapsuleCollider2D
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetAsCapsule(this Collider2D collider, out CapsuleCollider2D capsuleCollider)
        {
            capsuleCollider = collider as CapsuleCollider2D;
            return capsuleCollider != null;
        }

        #endregion

        #region 启用/禁用

        /// <summary>
        /// 切换启用状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(this Collider2D collider)
        {
            collider.enabled = !collider.enabled;
        }

        /// <summary>
        /// 设置为触发器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAsTrigger(this Collider2D collider, bool isTrigger = true)
        {
            collider.isTrigger = isTrigger;
        }

        #endregion

        #region BoxCollider2D 扩展

        /// <summary>
        /// 获取 BoxCollider2D 的世界空间角点
        /// </summary>
        public static Vector2[] GetWorldCorners(this BoxCollider2D box)
        {
            Vector2 center = box.offset;
            Vector2 extents = box.size * 0.5f;
            Transform t = box.transform;

            Vector2[] corners = new Vector2[4];
            corners[0] = t.TransformPoint(center + new Vector2(-extents.x, -extents.y));
            corners[1] = t.TransformPoint(center + new Vector2(extents.x, -extents.y));
            corners[2] = t.TransformPoint(center + new Vector2(extents.x, extents.y));
            corners[3] = t.TransformPoint(center + new Vector2(-extents.x, extents.y));

            return corners;
        }

        /// <summary>
        /// 获取 BoxCollider2D 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetWorldCenter(this BoxCollider2D box)
        {
            return box.transform.TransformPoint(box.offset);
        }

        /// <summary>
        /// 获取 BoxCollider2D 的世界空间尺寸
        /// </summary>
        public static Vector2 GetWorldSize(this BoxCollider2D box)
        {
            Vector3 scale = box.transform.lossyScale;
            return new Vector2(box.size.x * Mathf.Abs(scale.x), box.size.y * Mathf.Abs(scale.y));
        }

        #endregion

        #region CircleCollider2D 扩展

        /// <summary>
        /// 获取 CircleCollider2D 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetWorldCenter(this CircleCollider2D circle)
        {
            return circle.transform.TransformPoint(circle.offset);
        }

        /// <summary>
        /// 获取 CircleCollider2D 的世界空间半径
        /// </summary>
        public static float GetWorldRadius(this CircleCollider2D circle)
        {
            Vector3 scale = circle.transform.lossyScale;
            return circle.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
        }

        #endregion

        #region CapsuleCollider2D 扩展

        /// <summary>
        /// 获取 CapsuleCollider2D 的世界空间中心
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetWorldCenter(this CapsuleCollider2D capsule)
        {
            return capsule.transform.TransformPoint(capsule.offset);
        }

        /// <summary>
        /// 获取 CapsuleCollider2D 的两个端点 (世界空间)
        /// </summary>
        public static (Vector2 point1, Vector2 point2) GetWorldEndPoints(this CapsuleCollider2D capsule)
        {
            Vector2 center = capsule.offset;
            float radius = capsule.direction == CapsuleDirection2D.Vertical
                ? capsule.size.x * 0.5f
                : capsule.size.y * 0.5f;

            float halfHeight = capsule.direction == CapsuleDirection2D.Vertical
                ? (capsule.size.y * 0.5f) - radius
                : (capsule.size.x * 0.5f) - radius;

            halfHeight = Mathf.Max(0f, halfHeight);

            Vector2 direction = capsule.direction == CapsuleDirection2D.Vertical
                ? Vector2.up
                : Vector2.right;

            Transform t = capsule.transform;
            Vector2 point1 = t.TransformPoint(center + direction * halfHeight);
            Vector2 point2 = t.TransformPoint(center - direction * halfHeight);

            return (point1, point2);
        }

        #endregion

        #region 重叠检测

        /// <summary>
        /// 检查是否与另一个碰撞体重叠
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOverlapping(this Collider2D collider, Collider2D other)
        {
            return collider.IsTouching(other);
        }

        /// <summary>
        /// 获取与指定碰撞体的距离信息
        /// </summary>
        public static ColliderDistance2D GetDistance(this Collider2D collider, Collider2D other)
        {
            return collider.Distance(other);
        }

        #endregion
    }
}
