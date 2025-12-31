// ==========================================================
// 文件名：BoundsExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Bounds 扩展方法
    /// <para>提供 Bounds 的几何操作和实用功能扩展</para>
    /// </summary>
    public static class BoundsExtensions
    {
        #region 属性设置

        /// <summary>
        /// 设置中心点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds WithCenter(this Bounds b, Vector3 center) => new Bounds(center, b.size);

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds WithSize(this Bounds b, Vector3 size) => new Bounds(b.center, size);

        /// <summary>
        /// 设置半尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds WithExtents(this Bounds b, Vector3 extents) => new Bounds(b.center, extents * 2f);

        #endregion

        #region 偏移和缩放

        /// <summary>
        /// 偏移边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Offset(this Bounds b, Vector3 offset) => new Bounds(b.center + offset, b.size);

        /// <summary>
        /// 缩放边界 (从中心)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Scale(this Bounds b, float scale) => new Bounds(b.center, b.size * scale);

        /// <summary>
        /// 缩放边界 (从中心)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Scale(this Bounds b, Vector3 scale)
        {
            return new Bounds(b.center, Vector3.Scale(b.size, scale));
        }

        /// <summary>
        /// 扩展边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Expand(this Bounds b, float amount)
        {
            Bounds result = b;
            result.Expand(amount);
            return result;
        }

        /// <summary>
        /// 扩展边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Expand(this Bounds b, Vector3 amount)
        {
            Bounds result = b;
            result.Expand(amount);
            return result;
        }

        /// <summary>
        /// 收缩边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Shrink(this Bounds b, float amount) => b.Expand(-amount);

        #endregion

        #region 几何运算

        /// <summary>
        /// 获取两个边界的交集
        /// </summary>
        public static Bounds Intersection(this Bounds a, Bounds b)
        {
            Vector3 min = Vector3.Max(a.min, b.min);
            Vector3 max = Vector3.Min(a.max, b.max);

            if (min.x > max.x || min.y > max.y || min.z > max.z)
                return new Bounds(Vector3.zero, Vector3.zero);

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;
            return new Bounds(center, size);
        }

        /// <summary>
        /// 获取包含两个边界的最小边界
        /// </summary>
        public static Bounds Union(this Bounds a, Bounds b)
        {
            Vector3 min = Vector3.Min(a.min, b.min);
            Vector3 max = Vector3.Max(a.max, b.max);
            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;
            return new Bounds(center, size);
        }

        /// <summary>
        /// 扩展边界以包含指定点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Encapsulate(this Bounds b, Vector3 point)
        {
            Bounds result = b;
            result.Encapsulate(point);
            return result;
        }

        /// <summary>
        /// 扩展边界以包含另一个边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds Encapsulate(this Bounds b, Bounds other)
        {
            Bounds result = b;
            result.Encapsulate(other);
            return result;
        }

        /// <summary>
        /// 将点钳制到边界内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampPoint(this Bounds b, Vector3 point)
        {
            return new Vector3(
                Mathf.Clamp(point.x, b.min.x, b.max.x),
                Mathf.Clamp(point.y, b.min.y, b.max.y),
                Mathf.Clamp(point.z, b.min.z, b.max.z));
        }

        /// <summary>
        /// 获取边界内的随机点
        /// </summary>
        public static Vector3 RandomPoint(this Bounds b)
        {
            return new Vector3(
                UnityEngine.Random.Range(b.min.x, b.max.x),
                UnityEngine.Random.Range(b.min.y, b.max.y),
                UnityEngine.Random.Range(b.min.z, b.max.z));
        }

        /// <summary>
        /// 获取边界表面上的随机点
        /// </summary>
        public static Vector3 RandomPointOnSurface(this Bounds b)
        {
            // 随机选择一个面
            int face = UnityEngine.Random.Range(0, 6);
            Vector3 point = b.RandomPoint();

            switch (face)
            {
                case 0: point.x = b.min.x; break; // 左
                case 1: point.x = b.max.x; break; // 右
                case 2: point.y = b.min.y; break; // 下
                case 3: point.y = b.max.y; break; // 上
                case 4: point.z = b.min.z; break; // 前
                case 5: point.z = b.max.z; break; // 后
            }

            return point;
        }

        /// <summary>
        /// 获取到点的最近距离
        /// </summary>
        public static float DistanceToPoint(this Bounds b, Vector3 point)
        {
            Vector3 closest = b.ClosestPoint(point);
            return Vector3.Distance(point, closest);
        }

        /// <summary>
        /// 获取到另一个边界的最近距离
        /// </summary>
        public static float DistanceToBounds(this Bounds a, Bounds b)
        {
            if (a.Intersects(b))
                return 0f;

            Vector3 closestOnA = a.ClosestPoint(b.center);
            Vector3 closestOnB = b.ClosestPoint(closestOnA);
            return Vector3.Distance(closestOnA, closestOnB);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否完全包含另一个边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsBounds(this Bounds outer, Bounds inner)
        {
            return outer.Contains(inner.min) && outer.Contains(inner.max);
        }

        /// <summary>
        /// 检查是否为有效边界 (尺寸大于 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Bounds b)
        {
            return b.size.x > 0f && b.size.y > 0f && b.size.z > 0f;
        }

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Bounds a, Bounds b, float tolerance = 0.0001f)
        {
            return (a.center - b.center).sqrMagnitude < tolerance * tolerance &&
                   (a.size - b.size).sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>
        /// 获取体积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetVolume(this Bounds b) => b.size.x * b.size.y * b.size.z;

        /// <summary>
        /// 获取表面积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSurfaceArea(this Bounds b)
        {
            return 2f * (b.size.x * b.size.y + b.size.y * b.size.z + b.size.z * b.size.x);
        }

        /// <summary>
        /// 获取对角线长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDiagonalLength(this Bounds b) => b.size.magnitude;

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 BoundsInt
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsInt ToBoundsInt(this Bounds b)
        {
            Vector3Int min = Vector3Int.RoundToInt(b.min);
            Vector3Int size = Vector3Int.RoundToInt(b.size);
            return new BoundsInt(min, size);
        }

        /// <summary>
        /// 转换为 Rect (XY 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRectXY(this Bounds b)
        {
            return new Rect(b.min.x, b.min.y, b.size.x, b.size.y);
        }

        /// <summary>
        /// 转换为 Rect (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRectXZ(this Bounds b)
        {
            return new Rect(b.min.x, b.min.z, b.size.x, b.size.z);
        }

        /// <summary>
        /// 获取八个角点
        /// </summary>
        public static Vector3[] GetCorners(this Bounds b)
        {
            Vector3 min = b.min;
            Vector3 max = b.max;
            return new Vector3[]
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, max.y, max.z),
                new Vector3(min.x, max.y, max.z)
            };
        }

        /// <summary>
        /// 获取六个面的中心点
        /// </summary>
        public static Vector3[] GetFaceCenters(this Bounds b)
        {
            Vector3 c = b.center;
            Vector3 e = b.extents;
            return new Vector3[]
            {
                new Vector3(c.x - e.x, c.y, c.z), // 左
                new Vector3(c.x + e.x, c.y, c.z), // 右
                new Vector3(c.x, c.y - e.y, c.z), // 下
                new Vector3(c.x, c.y + e.y, c.z), // 上
                new Vector3(c.x, c.y, c.z - e.z), // 前
                new Vector3(c.x, c.y, c.z + e.z)  // 后
            };
        }

        #endregion

        #region 变换

        /// <summary>
        /// 应用变换矩阵
        /// </summary>
        public static Bounds Transform(this Bounds b, Matrix4x4 matrix)
        {
            Vector3[] corners = b.GetCorners();
            Bounds result = new Bounds(matrix.MultiplyPoint3x4(corners[0]), Vector3.zero);

            for (int i = 1; i < corners.Length; i++)
            {
                result.Encapsulate(matrix.MultiplyPoint3x4(corners[i]));
            }

            return result;
        }

        /// <summary>
        /// 应用 Transform 变换
        /// </summary>
        public static Bounds TransformBounds(this Bounds localBounds, Transform transform)
        {
            return localBounds.Transform(transform.localToWorldMatrix);
        }

        /// <summary>
        /// 应用逆 Transform 变换
        /// </summary>
        public static Bounds InverseTransformBounds(this Bounds worldBounds, Transform transform)
        {
            return worldBounds.Transform(transform.worldToLocalMatrix);
        }

        #endregion
    }
}
