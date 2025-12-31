// ==========================================================
// 文件名：RectExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Rect 扩展方法
    /// <para>提供 Rect 的几何操作和实用功能扩展</para>
    /// </summary>
    public static class RectExtensions
    {
        #region 属性设置

        /// <summary>
        /// 设置 X 坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithX(this Rect r, float x) => new Rect(x, r.y, r.width, r.height);

        /// <summary>
        /// 设置 Y 坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithY(this Rect r, float y) => new Rect(r.x, y, r.width, r.height);

        /// <summary>
        /// 设置宽度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithWidth(this Rect r, float width) => new Rect(r.x, r.y, width, r.height);

        /// <summary>
        /// 设置高度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithHeight(this Rect r, float height) => new Rect(r.x, r.y, r.width, height);

        /// <summary>
        /// 设置位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithPosition(this Rect r, Vector2 position) => new Rect(position.x, position.y, r.width, r.height);

        /// <summary>
        /// 设置位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithPosition(this Rect r, float x, float y) => new Rect(x, y, r.width, r.height);

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithSize(this Rect r, Vector2 size) => new Rect(r.x, r.y, size.x, size.y);

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithSize(this Rect r, float width, float height) => new Rect(r.x, r.y, width, height);

        /// <summary>
        /// 设置中心点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithCenter(this Rect r, Vector2 center)
        {
            return new Rect(center.x - r.width * 0.5f, center.y - r.height * 0.5f, r.width, r.height);
        }

        #endregion

        #region 偏移和缩放

        /// <summary>
        /// 偏移矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Offset(this Rect r, Vector2 offset) => new Rect(r.x + offset.x, r.y + offset.y, r.width, r.height);

        /// <summary>
        /// 偏移矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Offset(this Rect r, float x, float y) => new Rect(r.x + x, r.y + y, r.width, r.height);

        /// <summary>
        /// 缩放矩形 (从中心)
        /// </summary>
        public static Rect Scale(this Rect r, float scale)
        {
            float newWidth = r.width * scale;
            float newHeight = r.height * scale;
            float offsetX = (r.width - newWidth) * 0.5f;
            float offsetY = (r.height - newHeight) * 0.5f;
            return new Rect(r.x + offsetX, r.y + offsetY, newWidth, newHeight);
        }

        /// <summary>
        /// 缩放矩形 (从中心)
        /// </summary>
        public static Rect Scale(this Rect r, Vector2 scale)
        {
            float newWidth = r.width * scale.x;
            float newHeight = r.height * scale.y;
            float offsetX = (r.width - newWidth) * 0.5f;
            float offsetY = (r.height - newHeight) * 0.5f;
            return new Rect(r.x + offsetX, r.y + offsetY, newWidth, newHeight);
        }

        /// <summary>
        /// 缩放矩形 (从左上角)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ScaleFromOrigin(this Rect r, float scale)
        {
            return new Rect(r.x, r.y, r.width * scale, r.height * scale);
        }

        /// <summary>
        /// 扩展矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Expand(this Rect r, float amount)
        {
            return new Rect(r.x - amount, r.y - amount, r.width + amount * 2f, r.height + amount * 2f);
        }

        /// <summary>
        /// 扩展矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Expand(this Rect r, float horizontal, float vertical)
        {
            return new Rect(r.x - horizontal, r.y - vertical, r.width + horizontal * 2f, r.height + vertical * 2f);
        }

        /// <summary>
        /// 扩展矩形边界 (四边独立)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Expand(this Rect r, float left, float right, float top, float bottom)
        {
            return new Rect(r.x - left, r.y - top, r.width + left + right, r.height + top + bottom);
        }

        /// <summary>
        /// 收缩矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Shrink(this Rect r, float amount) => r.Expand(-amount);

        /// <summary>
        /// 收缩矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Shrink(this Rect r, float horizontal, float vertical) => r.Expand(-horizontal, -vertical);

        #endregion

        #region 分割

        /// <summary>
        /// 水平分割矩形
        /// </summary>
        /// <param name="r">原始矩形</param>
        /// <param name="ratio">分割比例 [0, 1]</param>
        /// <returns>(左侧矩形, 右侧矩形)</returns>
        public static (Rect left, Rect right) SplitHorizontal(this Rect r, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            float splitWidth = r.width * ratio;
            return (
                new Rect(r.x, r.y, splitWidth, r.height),
                new Rect(r.x + splitWidth, r.y, r.width - splitWidth, r.height)
            );
        }

        /// <summary>
        /// 垂直分割矩形
        /// </summary>
        /// <param name="r">原始矩形</param>
        /// <param name="ratio">分割比例 [0, 1]</param>
        /// <returns>(上方矩形, 下方矩形)</returns>
        public static (Rect top, Rect bottom) SplitVertical(this Rect r, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            float splitHeight = r.height * ratio;
            return (
                new Rect(r.x, r.y, r.width, splitHeight),
                new Rect(r.x, r.y + splitHeight, r.width, r.height - splitHeight)
            );
        }

        /// <summary>
        /// 从左侧切割指定宽度
        /// </summary>
        public static (Rect cut, Rect remaining) CutLeft(this Rect r, float width)
        {
            width = Mathf.Min(width, r.width);
            return (
                new Rect(r.x, r.y, width, r.height),
                new Rect(r.x + width, r.y, r.width - width, r.height)
            );
        }

        /// <summary>
        /// 从右侧切割指定宽度
        /// </summary>
        public static (Rect remaining, Rect cut) CutRight(this Rect r, float width)
        {
            width = Mathf.Min(width, r.width);
            return (
                new Rect(r.x, r.y, r.width - width, r.height),
                new Rect(r.xMax - width, r.y, width, r.height)
            );
        }

        /// <summary>
        /// 从顶部切割指定高度
        /// </summary>
        public static (Rect cut, Rect remaining) CutTop(this Rect r, float height)
        {
            height = Mathf.Min(height, r.height);
            return (
                new Rect(r.x, r.y, r.width, height),
                new Rect(r.x, r.y + height, r.width, r.height - height)
            );
        }

        /// <summary>
        /// 从底部切割指定高度
        /// </summary>
        public static (Rect remaining, Rect cut) CutBottom(this Rect r, float height)
        {
            height = Mathf.Min(height, r.height);
            return (
                new Rect(r.x, r.y, r.width, r.height - height),
                new Rect(r.x, r.yMax - height, r.width, height)
            );
        }

        #endregion

        #region 几何运算

        /// <summary>
        /// 获取两个矩形的交集
        /// </summary>
        public static Rect Intersection(this Rect a, Rect b)
        {
            float xMin = Mathf.Max(a.xMin, b.xMin);
            float yMin = Mathf.Max(a.yMin, b.yMin);
            float xMax = Mathf.Min(a.xMax, b.xMax);
            float yMax = Mathf.Min(a.yMax, b.yMax);

            if (xMax < xMin || yMax < yMin)
                return Rect.zero;

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// 获取包含两个矩形的最小矩形
        /// </summary>
        public static Rect Union(this Rect a, Rect b)
        {
            float xMin = Mathf.Min(a.xMin, b.xMin);
            float yMin = Mathf.Min(a.yMin, b.yMin);
            float xMax = Mathf.Max(a.xMax, b.xMax);
            float yMax = Mathf.Max(a.yMax, b.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// 扩展矩形以包含指定点
        /// </summary>
        public static Rect Encapsulate(this Rect r, Vector2 point)
        {
            float xMin = Mathf.Min(r.xMin, point.x);
            float yMin = Mathf.Min(r.yMin, point.y);
            float xMax = Mathf.Max(r.xMax, point.x);
            float yMax = Mathf.Max(r.yMax, point.y);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// 将点钳制到矩形内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampPoint(this Rect r, Vector2 point)
        {
            return new Vector2(
                Mathf.Clamp(point.x, r.xMin, r.xMax),
                Mathf.Clamp(point.y, r.yMin, r.yMax));
        }

        /// <summary>
        /// 获取矩形内的随机点
        /// </summary>
        public static Vector2 RandomPoint(this Rect r)
        {
            return new Vector2(
                UnityEngine.Random.Range(r.xMin, r.xMax),
                UnityEngine.Random.Range(r.yMin, r.yMax));
        }

        /// <summary>
        /// 获取到点的最近距离
        /// </summary>
        public static float DistanceToPoint(this Rect r, Vector2 point)
        {
            Vector2 closest = r.ClampPoint(point);
            return Vector2.Distance(point, closest);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否与另一个矩形重叠
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps(this Rect a, Rect b, bool allowInverse = false)
        {
            return a.Overlaps(b, allowInverse);
        }

        /// <summary>
        /// 检查是否完全包含另一个矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsRect(this Rect outer, Rect inner)
        {
            return outer.xMin <= inner.xMin && outer.xMax >= inner.xMax &&
                   outer.yMin <= inner.yMin && outer.yMax >= inner.yMax;
        }

        /// <summary>
        /// 检查是否为有效矩形 (宽高大于 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Rect r) => r.width > 0f && r.height > 0f;

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Rect a, Rect b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a.x - b.x) < tolerance &&
                   Mathf.Abs(a.y - b.y) < tolerance &&
                   Mathf.Abs(a.width - b.width) < tolerance &&
                   Mathf.Abs(a.height - b.height) < tolerance;
        }

        /// <summary>
        /// 获取宽高比
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAspectRatio(this Rect r) => r.height > 0f ? r.width / r.height : 0f;

        /// <summary>
        /// 获取面积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetArea(this Rect r) => r.width * r.height;

        /// <summary>
        /// 获取周长
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPerimeter(this Rect r) => 2f * (r.width + r.height);

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 RectInt
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt ToRectInt(this Rect r)
        {
            return new RectInt(
                Mathf.RoundToInt(r.x),
                Mathf.RoundToInt(r.y),
                Mathf.RoundToInt(r.width),
                Mathf.RoundToInt(r.height));
        }

        /// <summary>
        /// 转换为 RectInt (向下取整)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt FloorToRectInt(this Rect r)
        {
            return new RectInt(
                Mathf.FloorToInt(r.x),
                Mathf.FloorToInt(r.y),
                Mathf.FloorToInt(r.width),
                Mathf.FloorToInt(r.height));
        }

        /// <summary>
        /// 转换为 Bounds (Z = 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds ToBounds(this Rect r)
        {
            return new Bounds(
                new Vector3(r.center.x, r.center.y, 0f),
                new Vector3(r.width, r.height, 0f));
        }

        /// <summary>
        /// 获取四个角点
        /// </summary>
        public static Vector2[] GetCorners(this Rect r)
        {
            return new Vector2[]
            {
                new Vector2(r.xMin, r.yMin),
                new Vector2(r.xMax, r.yMin),
                new Vector2(r.xMax, r.yMax),
                new Vector2(r.xMin, r.yMax)
            };
        }

        #endregion
    }
}
