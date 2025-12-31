// ==========================================================
// 文件名：Space2D.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 2D 空间类型枚举
    /// <para>定义 2D 游戏中使用的坐标空间类型</para>
    /// </summary>
    [Serializable]
    public enum Space2D
    {
        /// <summary>XY 平面 (标准 2D，Z 轴为深度)</summary>
        XY = 0,

        /// <summary>XZ 平面 (俯视 2D，Y 轴为高度)</summary>
        XZ = 1
    }

    /// <summary>
    /// 2D 锚点位置枚举
    /// <para>定义 UI 或 Sprite 的锚点位置</para>
    /// </summary>
    [Serializable]
    public enum AnchorPosition2D
    {
        /// <summary>中心</summary>
        Center = 0,

        /// <summary>左上</summary>
        TopLeft = 1,

        /// <summary>上中</summary>
        TopCenter = 2,

        /// <summary>右上</summary>
        TopRight = 3,

        /// <summary>左中</summary>
        MiddleLeft = 4,

        /// <summary>右中</summary>
        MiddleRight = 5,

        /// <summary>左下</summary>
        BottomLeft = 6,

        /// <summary>下中</summary>
        BottomCenter = 7,

        /// <summary>右下</summary>
        BottomRight = 8
    }

    /// <summary>
    /// 2D 旋转方向枚举
    /// </summary>
    [Serializable]
    public enum RotationDirection2D
    {
        /// <summary>无旋转</summary>
        None = 0,

        /// <summary>顺时针</summary>
        Clockwise = 1,

        /// <summary>逆时针</summary>
        CounterClockwise = 2
    }

    /// <summary>
    /// 2D 翻转类型枚举
    /// </summary>
    [Flags]
    [Serializable]
    public enum FlipType2D
    {
        /// <summary>无翻转</summary>
        None = 0,

        /// <summary>水平翻转</summary>
        Horizontal = 1 << 0,

        /// <summary>垂直翻转</summary>
        Vertical = 1 << 1,

        /// <summary>双向翻转</summary>
        Both = Horizontal | Vertical
    }

    /// <summary>
    /// Space2D 扩展方法
    /// </summary>
    public static class Space2DExtensions
    {
        /// <summary>
        /// 转换为 PlaneType
        /// </summary>
        public static PlaneType ToPlaneType(this Space2D space)
        {
            return space == Space2D.XY ? PlaneType.XY : PlaneType.XZ;
        }
    }

    /// <summary>
    /// AnchorPosition2D 扩展方法
    /// </summary>
    public static class AnchorPosition2DExtensions
    {
        /// <summary>
        /// 获取锚点的归一化位置 (0-1)
        /// </summary>
        /// <param name="anchor">锚点位置</param>
        /// <returns>归一化位置向量</returns>
        public static UnityEngine.Vector2 ToNormalizedPosition(this AnchorPosition2D anchor)
        {
            return anchor switch
            {
                AnchorPosition2D.TopLeft => new UnityEngine.Vector2(0f, 1f),
                AnchorPosition2D.TopCenter => new UnityEngine.Vector2(0.5f, 1f),
                AnchorPosition2D.TopRight => new UnityEngine.Vector2(1f, 1f),
                AnchorPosition2D.MiddleLeft => new UnityEngine.Vector2(0f, 0.5f),
                AnchorPosition2D.Center => new UnityEngine.Vector2(0.5f, 0.5f),
                AnchorPosition2D.MiddleRight => new UnityEngine.Vector2(1f, 0.5f),
                AnchorPosition2D.BottomLeft => new UnityEngine.Vector2(0f, 0f),
                AnchorPosition2D.BottomCenter => new UnityEngine.Vector2(0.5f, 0f),
                AnchorPosition2D.BottomRight => new UnityEngine.Vector2(1f, 0f),
                _ => new UnityEngine.Vector2(0.5f, 0.5f)
            };
        }

        /// <summary>
        /// 获取锚点的偏移方向 (相对于中心)
        /// </summary>
        /// <param name="anchor">锚点位置</param>
        /// <returns>偏移方向向量 (-1 到 1)</returns>
        public static UnityEngine.Vector2 ToOffset(this AnchorPosition2D anchor)
        {
            var normalized = anchor.ToNormalizedPosition();
            return new UnityEngine.Vector2(
                (normalized.x - 0.5f) * 2f,
                (normalized.y - 0.5f) * 2f
            );
        }

        /// <summary>
        /// 检查是否在左侧
        /// </summary>
        public static bool IsLeft(this AnchorPosition2D anchor)
        {
            return anchor == AnchorPosition2D.TopLeft ||
                   anchor == AnchorPosition2D.MiddleLeft ||
                   anchor == AnchorPosition2D.BottomLeft;
        }

        /// <summary>
        /// 检查是否在右侧
        /// </summary>
        public static bool IsRight(this AnchorPosition2D anchor)
        {
            return anchor == AnchorPosition2D.TopRight ||
                   anchor == AnchorPosition2D.MiddleRight ||
                   anchor == AnchorPosition2D.BottomRight;
        }

        /// <summary>
        /// 检查是否在顶部
        /// </summary>
        public static bool IsTop(this AnchorPosition2D anchor)
        {
            return anchor == AnchorPosition2D.TopLeft ||
                   anchor == AnchorPosition2D.TopCenter ||
                   anchor == AnchorPosition2D.TopRight;
        }

        /// <summary>
        /// 检查是否在底部
        /// </summary>
        public static bool IsBottom(this AnchorPosition2D anchor)
        {
            return anchor == AnchorPosition2D.BottomLeft ||
                   anchor == AnchorPosition2D.BottomCenter ||
                   anchor == AnchorPosition2D.BottomRight;
        }

        /// <summary>
        /// 获取水平对称的锚点
        /// </summary>
        public static AnchorPosition2D FlipHorizontal(this AnchorPosition2D anchor)
        {
            return anchor switch
            {
                AnchorPosition2D.TopLeft => AnchorPosition2D.TopRight,
                AnchorPosition2D.TopRight => AnchorPosition2D.TopLeft,
                AnchorPosition2D.MiddleLeft => AnchorPosition2D.MiddleRight,
                AnchorPosition2D.MiddleRight => AnchorPosition2D.MiddleLeft,
                AnchorPosition2D.BottomLeft => AnchorPosition2D.BottomRight,
                AnchorPosition2D.BottomRight => AnchorPosition2D.BottomLeft,
                _ => anchor
            };
        }

        /// <summary>
        /// 获取垂直对称的锚点
        /// </summary>
        public static AnchorPosition2D FlipVertical(this AnchorPosition2D anchor)
        {
            return anchor switch
            {
                AnchorPosition2D.TopLeft => AnchorPosition2D.BottomLeft,
                AnchorPosition2D.TopCenter => AnchorPosition2D.BottomCenter,
                AnchorPosition2D.TopRight => AnchorPosition2D.BottomRight,
                AnchorPosition2D.BottomLeft => AnchorPosition2D.TopLeft,
                AnchorPosition2D.BottomCenter => AnchorPosition2D.TopCenter,
                AnchorPosition2D.BottomRight => AnchorPosition2D.TopRight,
                _ => anchor
            };
        }
    }

    /// <summary>
    /// RotationDirection2D 扩展方法
    /// </summary>
    public static class RotationDirection2DExtensions
    {
        /// <summary>
        /// 获取旋转方向的符号 (+1 顺时针, -1 逆时针, 0 无)
        /// </summary>
        public static int GetSign(this RotationDirection2D direction)
        {
            return direction switch
            {
                RotationDirection2D.Clockwise => 1,
                RotationDirection2D.CounterClockwise => -1,
                _ => 0
            };
        }

        /// <summary>
        /// 获取相反的旋转方向
        /// </summary>
        public static RotationDirection2D GetOpposite(this RotationDirection2D direction)
        {
            return direction switch
            {
                RotationDirection2D.Clockwise => RotationDirection2D.CounterClockwise,
                RotationDirection2D.CounterClockwise => RotationDirection2D.Clockwise,
                _ => RotationDirection2D.None
            };
        }
    }
}
