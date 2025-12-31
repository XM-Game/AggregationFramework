// ==========================================================
// 文件名：BoundsInt2D.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 2D 整数边界结构
    /// <para>表示 2D 空间中的整数坐标边界框</para>
    /// <para>常用于 Tilemap、网格系统等场景</para>
    /// </summary>
    [Serializable]
    public struct BoundsInt2D : IEquatable<BoundsInt2D>, IEnumerable<Vector2Int>
    {
        #region 字段

        /// <summary>X 坐标</summary>
        public int X;

        /// <summary>Y 坐标</summary>
        public int Y;

        /// <summary>宽度</summary>
        public int Width;

        /// <summary>高度</summary>
        public int Height;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 BoundsInt2D
        /// </summary>
        public BoundsInt2D(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 从位置和尺寸创建
        /// </summary>
        public BoundsInt2D(Vector2Int position, Vector2Int size)
        {
            X = position.x;
            Y = position.y;
            Width = size.x;
            Height = size.y;
        }

        #endregion

        #region 静态属性

        /// <summary>零边界</summary>
        public static BoundsInt2D Zero => new BoundsInt2D(0, 0, 0, 0);

        /// <summary>单位边界 (0,0,1,1)</summary>
        public static BoundsInt2D One => new BoundsInt2D(0, 0, 1, 1);

        #endregion

        #region 位置属性

        /// <summary>位置</summary>
        public Vector2Int Position
        {
            get => new Vector2Int(X, Y);
            set { X = value.x; Y = value.y; }
        }

        /// <summary>尺寸</summary>
        public Vector2Int Size
        {
            get => new Vector2Int(Width, Height);
            set { Width = value.x; Height = value.y; }
        }

        /// <summary>最小点</summary>
        public Vector2Int Min => new Vector2Int(X, Y);

        /// <summary>最大点 (不包含)</summary>
        public Vector2Int Max => new Vector2Int(X + Width, Y + Height);

        /// <summary>中心点 (浮点)</summary>
        public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        /// <summary>中心点 (整数，向下取整)</summary>
        public Vector2Int CenterInt => new Vector2Int(X + Width / 2, Y + Height / 2);

        /// <summary>左边界</summary>
        public int Left => X;

        /// <summary>右边界 (不包含)</summary>
        public int Right => X + Width;

        /// <summary>底边界</summary>
        public int Bottom => Y;

        /// <summary>顶边界 (不包含)</summary>
        public int Top => Y + Height;

        /// <summary>面积 (格子数量)</summary>
        public int Area => Width * Height;

        /// <summary>周长</summary>
        public int Perimeter => 2 * (Width + Height);

        #endregion

        #region 角点属性

        /// <summary>左下角</summary>
        public Vector2Int BottomLeft => new Vector2Int(X, Y);

        /// <summary>右下角</summary>
        public Vector2Int BottomRight => new Vector2Int(X + Width - 1, Y);

        /// <summary>左上角</summary>
        public Vector2Int TopLeft => new Vector2Int(X, Y + Height - 1);

        /// <summary>右上角</summary>
        public Vector2Int TopRight => new Vector2Int(X + Width - 1, Y + Height - 1);

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从两个点创建
        /// </summary>
        public static BoundsInt2D FromMinMax(Vector2Int min, Vector2Int max)
        {
            int x = Mathf.Min(min.x, max.x);
            int y = Mathf.Min(min.y, max.y);
            int w = Mathf.Abs(max.x - min.x);
            int h = Mathf.Abs(max.y - min.y);
            return new BoundsInt2D(x, y, w, h);
        }

        /// <summary>
        /// 从中心点和半径创建
        /// </summary>
        public static BoundsInt2D FromCenterAndExtents(Vector2Int center, Vector2Int extents)
        {
            return new BoundsInt2D(
                center.x - extents.x,
                center.y - extents.y,
                extents.x * 2,
                extents.y * 2
            );
        }

        /// <summary>
        /// 从 BoundsInt 创建 (忽略 Z)
        /// </summary>
        public static BoundsInt2D FromBoundsInt(BoundsInt bounds)
        {
            return new BoundsInt2D(bounds.x, bounds.y, bounds.size.x, bounds.size.y);
        }

        #endregion

        #region 检测方法

        /// <summary>
        /// 检查点是否在边界内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vector2Int point)
        {
            return point.x >= X && point.x < X + Width &&
                   point.y >= Y && point.y < Y + Height;
        }

        /// <summary>
        /// 检查点是否在边界内 (包含边界)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsInclusive(Vector2Int point)
        {
            return point.x >= X && point.x <= X + Width &&
                   point.y >= Y && point.y <= Y + Height;
        }

        /// <summary>
        /// 检查是否与另一个边界重叠
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(BoundsInt2D other)
        {
            return X < other.X + other.Width && X + Width > other.X &&
                   Y < other.Y + other.Height && Y + Height > other.Y;
        }

        /// <summary>
        /// 检查是否完全包含另一个边界
        /// </summary>
        public bool Contains(BoundsInt2D other)
        {
            return X <= other.X && X + Width >= other.X + other.Width &&
                   Y <= other.Y && Y + Height >= other.Y + other.Height;
        }

        /// <summary>
        /// 检查边界是否有效 (宽高大于0)
        /// </summary>
        public bool IsValid => Width > 0 && Height > 0;

        #endregion

        #region 变换方法

        /// <summary>
        /// 扩展边界
        /// </summary>
        public BoundsInt2D Expand(int amount)
        {
            return new BoundsInt2D(
                X - amount,
                Y - amount,
                Width + amount * 2,
                Height + amount * 2
            );
        }

        /// <summary>
        /// 收缩边界
        /// </summary>
        public BoundsInt2D Shrink(int amount) => Expand(-amount);

        /// <summary>
        /// 移动边界
        /// </summary>
        public BoundsInt2D Translate(Vector2Int offset)
        {
            return new BoundsInt2D(X + offset.x, Y + offset.y, Width, Height);
        }

        /// <summary>
        /// 将点限制在边界内
        /// </summary>
        public Vector2Int ClampPoint(Vector2Int point)
        {
            return new Vector2Int(
                Mathf.Clamp(point.x, X, X + Width - 1),
                Mathf.Clamp(point.y, Y, Y + Height - 1)
            );
        }

        #endregion

        #region 组合方法

        /// <summary>
        /// 获取两个边界的交集
        /// </summary>
        public static BoundsInt2D Intersect(BoundsInt2D a, BoundsInt2D b)
        {
            int x = Mathf.Max(a.X, b.X);
            int y = Mathf.Max(a.Y, b.Y);
            int right = Mathf.Min(a.Right, b.Right);
            int top = Mathf.Min(a.Top, b.Top);

            if (right <= x || top <= y)
                return Zero;

            return new BoundsInt2D(x, y, right - x, top - y);
        }

        /// <summary>
        /// 获取包含两个边界的最小边界
        /// </summary>
        public static BoundsInt2D Union(BoundsInt2D a, BoundsInt2D b)
        {
            int x = Mathf.Min(a.X, b.X);
            int y = Mathf.Min(a.Y, b.Y);
            int right = Mathf.Max(a.Right, b.Right);
            int top = Mathf.Max(a.Top, b.Top);

            return new BoundsInt2D(x, y, right - x, top - y);
        }

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为 BoundsInt (Z = 0, depth = 1)
        /// </summary>
        public BoundsInt ToBoundsInt(int z = 0, int depth = 1)
        {
            return new BoundsInt(X, Y, z, Width, Height, depth);
        }

        /// <summary>
        /// 转换为 RectInt
        /// </summary>
        public RectInt ToRectInt() => new RectInt(X, Y, Width, Height);

        /// <summary>
        /// 转换为 RectData
        /// </summary>
        public RectData ToRectData() => new RectData(X, Y, Width, Height);

        #endregion

        #region 枚举器

        /// <summary>
        /// 获取所有位置的枚举器
        /// </summary>
        public IEnumerator<Vector2Int> GetEnumerator()
        {
            for (int y = Y; y < Y + Height; y++)
            {
                for (int x = X; x < X + Width; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 获取所有位置
        /// </summary>
        public IEnumerable<Vector2Int> AllPositions()
        {
            for (int y = Y; y < Y + Height; y++)
            {
                for (int x = X; x < X + Width; x++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        /// <summary>
        /// 获取边界上的所有位置
        /// </summary>
        public IEnumerable<Vector2Int> BorderPositions()
        {
            // 底边
            for (int x = X; x < X + Width; x++)
                yield return new Vector2Int(x, Y);

            // 顶边
            for (int x = X; x < X + Width; x++)
                yield return new Vector2Int(x, Y + Height - 1);

            // 左边 (不含角)
            for (int y = Y + 1; y < Y + Height - 1; y++)
                yield return new Vector2Int(X, y);

            // 右边 (不含角)
            for (int y = Y + 1; y < Y + Height - 1; y++)
                yield return new Vector2Int(X + Width - 1, y);
        }

        #endregion

        #region IEquatable 实现

        public bool Equals(BoundsInt2D other)
        {
            return X == other.X && Y == other.Y && 
                   Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj) => obj is BoundsInt2D other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        public static bool operator ==(BoundsInt2D left, BoundsInt2D right) => left.Equals(right);
        public static bool operator !=(BoundsInt2D left, BoundsInt2D right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        public override string ToString() => $"BoundsInt2D({X}, {Y}, {Width}, {Height})";

        #endregion
    }
}
