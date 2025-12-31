// ==========================================================
// 文件名：Direction.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 3D 方向枚举
    /// <para>表示 3D 空间中的六个基本方向</para>
    /// </summary>
    [Serializable]
    public enum Direction3D
    {
        /// <summary>无方向</summary>
        None = 0,

        /// <summary>右 (+X)</summary>
        Right = 1,

        /// <summary>左 (-X)</summary>
        Left = 2,

        /// <summary>上 (+Y)</summary>
        Up = 3,

        /// <summary>下 (-Y)</summary>
        Down = 4,

        /// <summary>前 (+Z)</summary>
        Forward = 5,

        /// <summary>后 (-Z)</summary>
        Back = 6
    }

    /// <summary>
    /// 2D 方向枚举
    /// <para>表示 2D 空间中的四个基本方向</para>
    /// </summary>
    [Serializable]
    public enum Direction2D
    {
        /// <summary>无方向</summary>
        None = 0,

        /// <summary>右 (+X)</summary>
        Right = 1,

        /// <summary>左 (-X)</summary>
        Left = 2,

        /// <summary>上 (+Y)</summary>
        Up = 3,

        /// <summary>下 (-Y)</summary>
        Down = 4
    }

    /// <summary>
    /// 水平方向枚举
    /// </summary>
    [Serializable]
    public enum HorizontalDirection
    {
        /// <summary>无方向</summary>
        None = 0,

        /// <summary>左</summary>
        Left = 1,

        /// <summary>右</summary>
        Right = 2
    }

    /// <summary>
    /// 垂直方向枚举
    /// </summary>
    [Serializable]
    public enum VerticalDirection
    {
        /// <summary>无方向</summary>
        None = 0,

        /// <summary>上</summary>
        Up = 1,

        /// <summary>下</summary>
        Down = 2
    }

    /// <summary>
    /// Direction3D 扩展方法
    /// </summary>
    public static class Direction3DExtensions
    {
        /// <summary>
        /// 转换为单位向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Direction3D direction)
        {
            return direction switch
            {
                Direction3D.Right => Vector3.right,
                Direction3D.Left => Vector3.left,
                Direction3D.Up => Vector3.up,
                Direction3D.Down => Vector3.down,
                Direction3D.Forward => Vector3.forward,
                Direction3D.Back => Vector3.back,
                _ => Vector3.zero
            };
        }

        /// <summary>
        /// 获取相反方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction3D GetOpposite(this Direction3D direction)
        {
            return direction switch
            {
                Direction3D.Right => Direction3D.Left,
                Direction3D.Left => Direction3D.Right,
                Direction3D.Up => Direction3D.Down,
                Direction3D.Down => Direction3D.Up,
                Direction3D.Forward => Direction3D.Back,
                Direction3D.Back => Direction3D.Forward,
                _ => Direction3D.None
            };
        }

        /// <summary>
        /// 获取方向对应的轴
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Axis GetAxis(this Direction3D direction)
        {
            return direction switch
            {
                Direction3D.Right or Direction3D.Left => Axis.X,
                Direction3D.Up or Direction3D.Down => Axis.Y,
                Direction3D.Forward or Direction3D.Back => Axis.Z,
                _ => Axis.X
            };
        }

        /// <summary>
        /// 检查是否为正方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this Direction3D direction)
        {
            return direction == Direction3D.Right || 
                   direction == Direction3D.Up || 
                   direction == Direction3D.Forward;
        }

        /// <summary>
        /// 获取方向的符号 (+1 或 -1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSign(this Direction3D direction)
        {
            return direction.IsPositive() ? 1 : -1;
        }

        /// <summary>
        /// 从向量获取最接近的方向
        /// </summary>
        public static Direction3D FromVector3(Vector3 vector)
        {
            if (vector.sqrMagnitude < 0.0001f)
                return Direction3D.None;

            vector.Normalize();
            float absX = Mathf.Abs(vector.x);
            float absY = Mathf.Abs(vector.y);
            float absZ = Mathf.Abs(vector.z);

            if (absX >= absY && absX >= absZ)
                return vector.x > 0 ? Direction3D.Right : Direction3D.Left;
            if (absY >= absX && absY >= absZ)
                return vector.y > 0 ? Direction3D.Up : Direction3D.Down;
            return vector.z > 0 ? Direction3D.Forward : Direction3D.Back;
        }

        /// <summary>
        /// 顺时针旋转 90 度 (在 XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction3D RotateClockwiseXZ(this Direction3D direction)
        {
            return direction switch
            {
                Direction3D.Forward => Direction3D.Right,
                Direction3D.Right => Direction3D.Back,
                Direction3D.Back => Direction3D.Left,
                Direction3D.Left => Direction3D.Forward,
                _ => direction
            };
        }

        /// <summary>
        /// 逆时针旋转 90 度 (在 XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction3D RotateCounterClockwiseXZ(this Direction3D direction)
        {
            return direction switch
            {
                Direction3D.Forward => Direction3D.Left,
                Direction3D.Left => Direction3D.Back,
                Direction3D.Back => Direction3D.Right,
                Direction3D.Right => Direction3D.Forward,
                _ => direction
            };
        }
    }

    /// <summary>
    /// Direction2D 扩展方法
    /// </summary>
    public static class Direction2DExtensions
    {
        /// <summary>
        /// 转换为单位向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.Right => Vector2.right,
                Direction2D.Left => Vector2.left,
                Direction2D.Up => Vector2.up,
                Direction2D.Down => Vector2.down,
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// 获取相反方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D GetOpposite(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.Right => Direction2D.Left,
                Direction2D.Left => Direction2D.Right,
                Direction2D.Up => Direction2D.Down,
                Direction2D.Down => Direction2D.Up,
                _ => Direction2D.None
            };
        }

        /// <summary>
        /// 顺时针旋转 90 度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D RotateClockwise(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.Up => Direction2D.Right,
                Direction2D.Right => Direction2D.Down,
                Direction2D.Down => Direction2D.Left,
                Direction2D.Left => Direction2D.Up,
                _ => direction
            };
        }

        /// <summary>
        /// 逆时针旋转 90 度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction2D RotateCounterClockwise(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.Up => Direction2D.Left,
                Direction2D.Left => Direction2D.Down,
                Direction2D.Down => Direction2D.Right,
                Direction2D.Right => Direction2D.Up,
                _ => direction
            };
        }

        /// <summary>
        /// 从向量获取最接近的方向
        /// </summary>
        public static Direction2D FromVector2(Vector2 vector)
        {
            if (vector.sqrMagnitude < 0.0001f)
                return Direction2D.None;

            if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y))
                return vector.x > 0 ? Direction2D.Right : Direction2D.Left;
            return vector.y > 0 ? Direction2D.Up : Direction2D.Down;
        }

        /// <summary>
        /// 获取方向对应的角度 (度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToAngle(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.Right => 0f,
                Direction2D.Up => 90f,
                Direction2D.Left => 180f,
                Direction2D.Down => 270f,
                _ => 0f
            };
        }

        /// <summary>
        /// 检查是否为水平方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHorizontal(this Direction2D direction)
        {
            return direction == Direction2D.Left || direction == Direction2D.Right;
        }

        /// <summary>
        /// 检查是否为垂直方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVertical(this Direction2D direction)
        {
            return direction == Direction2D.Up || direction == Direction2D.Down;
        }
    }
}
