// ==========================================================
// 文件名：RectIntExtensions.cs
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
    /// RectInt 扩展方法
    /// <para>提供 RectInt 的几何操作和实用功能扩展</para>
    /// </summary>
    public static class RectIntExtensions
    {
        #region 属性设置

        /// <summary>
        /// 设置 X 坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithX(this RectInt r, int x) => new RectInt(x, r.y, r.width, r.height);

        /// <summary>
        /// 设置 Y 坐标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithY(this RectInt r, int y) => new RectInt(r.x, y, r.width, r.height);

        /// <summary>
        /// 设置宽度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithWidth(this RectInt r, int width) => new RectInt(r.x, r.y, width, r.height);

        /// <summary>
        /// 设置高度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithHeight(this RectInt r, int height) => new RectInt(r.x, r.y, r.width, height);

        /// <summary>
        /// 设置位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithPosition(this RectInt r, Vector2Int position)
        {
            return new RectInt(position.x, position.y, r.width, r.height);
        }

        /// <summary>
        /// 设置尺寸
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt WithSize(this RectInt r, Vector2Int size)
        {
            return new RectInt(r.x, r.y, size.x, size.y);
        }

        #endregion

        #region 偏移和缩放

        /// <summary>
        /// 偏移矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt Offset(this RectInt r, Vector2Int offset)
        {
            return new RectInt(r.x + offset.x, r.y + offset.y, r.width, r.height);
        }

        /// <summary>
        /// 偏移矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt Offset(this RectInt r, int x, int y)
        {
            return new RectInt(r.x + x, r.y + y, r.width, r.height);
        }

        /// <summary>
        /// 扩展矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt Expand(this RectInt r, int amount)
        {
            return new RectInt(r.x - amount, r.y - amount, r.width + amount * 2, r.height + amount * 2);
        }

        /// <summary>
        /// 扩展矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt Expand(this RectInt r, int horizontal, int vertical)
        {
            return new RectInt(r.x - horizontal, r.y - vertical, r.width + horizontal * 2, r.height + vertical * 2);
        }

        /// <summary>
        /// 收缩矩形边界
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt Shrink(this RectInt r, int amount) => r.Expand(-amount);

        #endregion

        #region 分割

        /// <summary>
        /// 水平分割矩形
        /// </summary>
        /// <param name="r">原始矩形</param>
        /// <param name="splitX">分割位置 (相对于矩形左边)</param>
        /// <returns>(左侧矩形, 右侧矩形)</returns>
        public static (RectInt left, RectInt right) SplitHorizontal(this RectInt r, int splitX)
        {
            splitX = Mathf.Clamp(splitX, 0, r.width);
            return (
                new RectInt(r.x, r.y, splitX, r.height),
                new RectInt(r.x + splitX, r.y, r.width - splitX, r.height)
            );
        }

        /// <summary>
        /// 垂直分割矩形
        /// </summary>
        /// <param name="r">原始矩形</param>
        /// <param name="splitY">分割位置 (相对于矩形顶部)</param>
        /// <returns>(上方矩形, 下方矩形)</returns>
        public static (RectInt top, RectInt bottom) SplitVertical(this RectInt r, int splitY)
        {
            splitY = Mathf.Clamp(splitY, 0, r.height);
            return (
                new RectInt(r.x, r.y, r.width, splitY),
                new RectInt(r.x, r.y + splitY, r.width, r.height - splitY)
            );
        }

        #endregion

        #region 几何运算

        /// <summary>
        /// 获取两个矩形的交集
        /// </summary>
        public static RectInt Intersection(this RectInt a, RectInt b)
        {
            int xMin = Mathf.Max(a.xMin, b.xMin);
            int yMin = Mathf.Max(a.yMin, b.yMin);
            int xMax = Mathf.Min(a.xMax, b.xMax);
            int yMax = Mathf.Min(a.yMax, b.yMax);

            if (xMax < xMin || yMax < yMin)
                return new RectInt(0, 0, 0, 0);

            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// 获取包含两个矩形的最小矩形
        /// </summary>
        public static RectInt Union(this RectInt a, RectInt b)
        {
            int xMin = Mathf.Min(a.xMin, b.xMin);
            int yMin = Mathf.Min(a.yMin, b.yMin);
            int xMax = Mathf.Max(a.xMax, b.xMax);
            int yMax = Mathf.Max(a.yMax, b.yMax);
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// 扩展矩形以包含指定点
        /// </summary>
        public static RectInt Encapsulate(this RectInt r, Vector2Int point)
        {
            int xMin = Mathf.Min(r.xMin, point.x);
            int yMin = Mathf.Min(r.yMin, point.y);
            int xMax = Mathf.Max(r.xMax, point.x);
            int yMax = Mathf.Max(r.yMax, point.y);
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// 将点钳制到矩形内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ClampPoint(this RectInt r, Vector2Int point)
        {
            return new Vector2Int(
                Mathf.Clamp(point.x, r.xMin, r.xMax - 1),
                Mathf.Clamp(point.y, r.yMin, r.yMax - 1));
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否与另一个矩形重叠
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps(this RectInt a, RectInt b)
        {
            return a.xMin < b.xMax && a.xMax > b.xMin &&
                   a.yMin < b.yMax && a.yMax > b.yMin;
        }

        /// <summary>
        /// 检查是否完全包含另一个矩形
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsRect(this RectInt outer, RectInt inner)
        {
            return outer.xMin <= inner.xMin && outer.xMax >= inner.xMax &&
                   outer.yMin <= inner.yMin && outer.yMax >= inner.yMax;
        }

        /// <summary>
        /// 检查是否包含指定点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(this RectInt r, Vector2Int point)
        {
            return point.x >= r.xMin && point.x < r.xMax &&
                   point.y >= r.yMin && point.y < r.yMax;
        }

        /// <summary>
        /// 检查是否为有效矩形 (宽高大于 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this RectInt r) => r.width > 0 && r.height > 0;

        /// <summary>
        /// 获取面积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArea(this RectInt r) => r.width * r.height;

        /// <summary>
        /// 获取周长
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPerimeter(this RectInt r) => 2 * (r.width + r.height);

        #endregion

        #region 遍历

        /// <summary>
        /// 遍历矩形内的所有点
        /// </summary>
        public static IEnumerable<Vector2Int> AllPositions(this RectInt r)
        {
            for (int y = r.yMin; y < r.yMax; y++)
            {
                for (int x = r.xMin; x < r.xMax; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        /// <summary>
        /// 遍历矩形边界上的所有点
        /// </summary>
        public static IEnumerable<Vector2Int> BorderPositions(this RectInt r)
        {
            // 上边
            for (int x = r.xMin; x < r.xMax; x++)
                yield return new Vector2Int(x, r.yMin);

            // 右边 (不含角点)
            for (int y = r.yMin + 1; y < r.yMax - 1; y++)
                yield return new Vector2Int(r.xMax - 1, y);

            // 下边
            if (r.height > 1)
            {
                for (int x = r.xMax - 1; x >= r.xMin; x--)
                    yield return new Vector2Int(x, r.yMax - 1);
            }

            // 左边 (不含角点)
            if (r.width > 1)
            {
                for (int y = r.yMax - 2; y > r.yMin; y--)
                    yield return new Vector2Int(r.xMin, y);
            }
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Rect
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this RectInt r)
        {
            return new Rect(r.x, r.y, r.width, r.height);
        }

        /// <summary>
        /// 转换为 BoundsInt (Z = 0, 深度 = 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundsInt ToBoundsInt(this RectInt r)
        {
            return new BoundsInt(r.x, r.y, 0, r.width, r.height, 1);
        }

        /// <summary>
        /// 获取四个角点
        /// </summary>
        public static Vector2Int[] GetCorners(this RectInt r)
        {
            return new Vector2Int[]
            {
                new Vector2Int(r.xMin, r.yMin),
                new Vector2Int(r.xMax - 1, r.yMin),
                new Vector2Int(r.xMax - 1, r.yMax - 1),
                new Vector2Int(r.xMin, r.yMax - 1)
            };
        }

        /// <summary>
        /// 获取中心点 (浮点数)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetCenterFloat(this RectInt r)
        {
            return new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);
        }

        #endregion
    }
}
