// ==========================================================
// 文件名：RectData.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Rect 数据结构
    /// <para>增强版矩形数据，提供更多便捷属性和方法</para>
    /// </summary>
    [Serializable]
    public struct RectData : IEquatable<RectData>
    {
        #region 字段

        /// <summary>X 坐标</summary>
        public float X;

        /// <summary>Y 坐标</summary>
        public float Y;

        /// <summary>宽度</summary>
        public float Width;

        /// <summary>高度</summary>
        public float Height;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 RectData
        /// </summary>
        public RectData(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 从位置和尺寸创建
        /// </summary>
        public RectData(Vector2 position, Vector2 size)
        {
            X = position.x;
            Y = position.y;
            Width = size.x;
            Height = size.y;
        }

        /// <summary>
        /// 从 Rect 创建
        /// </summary>
        public RectData(Rect rect)
        {
            X = rect.x;
            Y = rect.y;
            Width = rect.width;
            Height = rect.height;
        }

        #endregion

        #region 静态属性

        /// <summary>零矩形</summary>
        public static RectData Zero => new RectData(0, 0, 0, 0);

        /// <summary>单位矩形 (0,0,1,1)</summary>
        public static RectData One => new RectData(0, 0, 1, 1);

        #endregion

        #region 位置属性

        /// <summary>位置</summary>
        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set { X = value.x; Y = value.y; }
        }

        /// <summary>尺寸</summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = value.x; Height = value.y; }
        }

        /// <summary>中心点</summary>
        public Vector2 Center
        {
            get => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);
            set { X = value.x - Width * 0.5f; Y = value.y - Height * 0.5f; }
        }

        /// <summary>最小点 (左下角)</summary>
        public Vector2 Min
        {
            get => new Vector2(X, Y);
            set { X = value.x; Y = value.y; }
        }

        /// <summary>最大点 (右上角)</summary>
        public Vector2 Max
        {
            get => new Vector2(X + Width, Y + Height);
            set { Width = value.x - X; Height = value.y - Y; }
        }

        /// <summary>左边界</summary>
        public float Left => X;

        /// <summary>右边界</summary>
        public float Right => X + Width;

        /// <summary>底边界</summary>
        public float Bottom => Y;

        /// <summary>顶边界</summary>
        public float Top => Y + Height;

        /// <summary>面积</summary>
        public float Area => Width * Height;

        /// <summary>周长</summary>
        public float Perimeter => 2 * (Width + Height);

        /// <summary>宽高比</summary>
        public float AspectRatio => Height > 0 ? Width / Height : 0;

        #endregion

        #region 角点属性

        /// <summary>左下角</summary>
        public Vector2 BottomLeft => new Vector2(X, Y);

        /// <summary>右下角</summary>
        public Vector2 BottomRight => new Vector2(X + Width, Y);

        /// <summary>左上角</summary>
        public Vector2 TopLeft => new Vector2(X, Y + Height);

        /// <summary>右上角</summary>
        public Vector2 TopRight => new Vector2(X + Width, Y + Height);

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从中心点和尺寸创建
        /// </summary>
        public static RectData FromCenterAndSize(Vector2 center, Vector2 size)
        {
            return new RectData(
                center.x - size.x * 0.5f,
                center.y - size.y * 0.5f,
                size.x,
                size.y
            );
        }

        /// <summary>
        /// 从两个点创建 (自动计算边界)
        /// </summary>
        public static RectData FromMinMax(Vector2 min, Vector2 max)
        {
            float x = Mathf.Min(min.x, max.x);
            float y = Mathf.Min(min.y, max.y);
            float w = Mathf.Abs(max.x - min.x);
            float h = Mathf.Abs(max.y - min.y);
            return new RectData(x, y, w, h);
        }

        /// <summary>
        /// 从 RectTransform 创建
        /// </summary>
        public static RectData FromRectTransform(RectTransform rectTransform)
        {
            return new RectData(rectTransform.rect);
        }

        #endregion

        #region 检测方法

        /// <summary>
        /// 检查点是否在矩形内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vector2 point)
        {
            return point.x >= X && point.x <= X + Width &&
                   point.y >= Y && point.y <= Y + Height;
        }

        /// <summary>
        /// 检查点是否在矩形内 (不包含边界)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsExclusive(Vector2 point)
        {
            return point.x > X && point.x < X + Width &&
                   point.y > Y && point.y < Y + Height;
        }

        /// <summary>
        /// 检查是否与另一个矩形重叠
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RectData other)
        {
            return X < other.X + other.Width && X + Width > other.X &&
                   Y < other.Y + other.Height && Y + Height > other.Y;
        }

        /// <summary>
        /// 检查是否完全包含另一个矩形
        /// </summary>
        public bool Contains(RectData other)
        {
            return X <= other.X && X + Width >= other.X + other.Width &&
                   Y <= other.Y && Y + Height >= other.Y + other.Height;
        }

        #endregion

        #region 变换方法

        /// <summary>
        /// 扩展矩形
        /// </summary>
        /// <param name="amount">扩展量</param>
        /// <returns>扩展后的矩形</returns>
        public RectData Expand(float amount)
        {
            return new RectData(
                X - amount,
                Y - amount,
                Width + amount * 2,
                Height + amount * 2
            );
        }

        /// <summary>
        /// 扩展矩形 (分别指定水平和垂直)
        /// </summary>
        public RectData Expand(float horizontal, float vertical)
        {
            return new RectData(
                X - horizontal,
                Y - vertical,
                Width + horizontal * 2,
                Height + vertical * 2
            );
        }

        /// <summary>
        /// 收缩矩形
        /// </summary>
        public RectData Shrink(float amount) => Expand(-amount);

        /// <summary>
        /// 移动矩形
        /// </summary>
        public RectData Translate(Vector2 offset)
        {
            return new RectData(X + offset.x, Y + offset.y, Width, Height);
        }

        /// <summary>
        /// 缩放矩形
        /// </summary>
        public RectData Scale(float scale)
        {
            return FromCenterAndSize(Center, Size * scale);
        }

        /// <summary>
        /// 缩放矩形 (分别指定 X 和 Y)
        /// </summary>
        public RectData Scale(Vector2 scale)
        {
            return FromCenterAndSize(Center, Vector2.Scale(Size, scale));
        }

        /// <summary>
        /// 将点限制在矩形内
        /// </summary>
        public Vector2 ClampPoint(Vector2 point)
        {
            return new Vector2(
                Mathf.Clamp(point.x, X, X + Width),
                Mathf.Clamp(point.y, Y, Y + Height)
            );
        }

        /// <summary>
        /// 获取最近的边界点
        /// </summary>
        public Vector2 GetClosestPoint(Vector2 point)
        {
            if (Contains(point))
                return point;
            return ClampPoint(point);
        }

        #endregion

        #region 组合方法

        /// <summary>
        /// 获取两个矩形的交集
        /// </summary>
        public static RectData Intersect(RectData a, RectData b)
        {
            float x = Mathf.Max(a.X, b.X);
            float y = Mathf.Max(a.Y, b.Y);
            float right = Mathf.Min(a.Right, b.Right);
            float top = Mathf.Min(a.Top, b.Top);

            if (right < x || top < y)
                return Zero;

            return new RectData(x, y, right - x, top - y);
        }

        /// <summary>
        /// 获取包含两个矩形的最小矩形
        /// </summary>
        public static RectData Union(RectData a, RectData b)
        {
            float x = Mathf.Min(a.X, b.X);
            float y = Mathf.Min(a.Y, b.Y);
            float right = Mathf.Max(a.Right, b.Right);
            float top = Mathf.Max(a.Top, b.Top);

            return new RectData(x, y, right - x, top - y);
        }

        #endregion

        #region 插值方法

        /// <summary>
        /// 线性插值
        /// </summary>
        public static RectData Lerp(RectData a, RectData b, float t)
        {
            return new RectData(
                Mathf.Lerp(a.X, b.X, t),
                Mathf.Lerp(a.Y, b.Y, t),
                Mathf.Lerp(a.Width, b.Width, t),
                Mathf.Lerp(a.Height, b.Height, t)
            );
        }

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为 Unity Rect
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect ToRect() => new Rect(X, Y, Width, Height);

        /// <summary>
        /// 从 Unity Rect 隐式转换
        /// </summary>
        public static implicit operator RectData(Rect rect) => new RectData(rect);

        /// <summary>
        /// 转换为 Unity Rect
        /// </summary>
        public static implicit operator Rect(RectData data) => data.ToRect();

        #endregion

        #region IEquatable 实现

        public bool Equals(RectData other)
        {
            return Mathf.Approximately(X, other.X) &&
                   Mathf.Approximately(Y, other.Y) &&
                   Mathf.Approximately(Width, other.Width) &&
                   Mathf.Approximately(Height, other.Height);
        }

        public override bool Equals(object obj) => obj is RectData other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        public static bool operator ==(RectData left, RectData right) => left.Equals(right);
        public static bool operator !=(RectData left, RectData right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        public override string ToString() => $"RectData({X}, {Y}, {Width}, {Height})";

        #endregion
    }
}
