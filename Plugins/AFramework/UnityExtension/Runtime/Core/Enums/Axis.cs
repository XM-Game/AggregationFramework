// ==========================================================
// 文件名：Axis.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 坐标轴枚举
    /// <para>表示 3D 空间中的单一坐标轴</para>
    /// </summary>
    [Serializable]
    public enum Axis
    {
        /// <summary>X 轴 (水平轴，通常为左右方向)</summary>
        X = 0,

        /// <summary>Y 轴 (垂直轴，通常为上下方向)</summary>
        Y = 1,

        /// <summary>Z 轴 (深度轴，通常为前后方向)</summary>
        Z = 2
    }

    /// <summary>
    /// 坐标轴组合枚举 (支持多轴选择)
    /// <para>使用位标志支持多轴组合</para>
    /// </summary>
    [Flags]
    [Serializable]
    public enum AxisFlags
    {
        /// <summary>无轴</summary>
        None = 0,

        /// <summary>X 轴</summary>
        X = 1 << 0,

        /// <summary>Y 轴</summary>
        Y = 1 << 1,

        /// <summary>Z 轴</summary>
        Z = 1 << 2,

        /// <summary>XY 平面 (水平和垂直)</summary>
        XY = X | Y,

        /// <summary>XZ 平面 (水平面)</summary>
        XZ = X | Z,

        /// <summary>YZ 平面 (垂直面)</summary>
        YZ = Y | Z,

        /// <summary>所有轴 (XYZ)</summary>
        All = X | Y | Z
    }

    /// <summary>
    /// Axis 枚举扩展方法
    /// </summary>
    public static class AxisExtensions
    {
        /// <summary>
        /// 获取轴向对应的单位向量
        /// </summary>
        /// <param name="axis">轴向</param>
        /// <returns>单位向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Axis axis)
        {
            return axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.zero
            };
        }

        /// <summary>
        /// 获取轴向对应的 2D 单位向量
        /// </summary>
        /// <param name="axis">轴向</param>
        /// <returns>2D 单位向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Axis axis)
        {
            return axis switch
            {
                Axis.X => Vector2.right,
                Axis.Y => Vector2.up,
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// 获取向量在指定轴向上的分量
        /// </summary>
        /// <param name="axis">轴向</param>
        /// <param name="vector">向量</param>
        /// <returns>分量值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetComponent(this Axis axis, Vector3 vector)
        {
            return axis switch
            {
                Axis.X => vector.x,
                Axis.Y => vector.y,
                Axis.Z => vector.z,
                _ => 0f
            };
        }

        /// <summary>
        /// 设置向量在指定轴向上的分量
        /// </summary>
        /// <param name="axis">轴向</param>
        /// <param name="vector">原向量</param>
        /// <param name="value">新分量值</param>
        /// <returns>修改后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SetComponent(this Axis axis, Vector3 vector, float value)
        {
            return axis switch
            {
                Axis.X => new Vector3(value, vector.y, vector.z),
                Axis.Y => new Vector3(vector.x, value, vector.z),
                Axis.Z => new Vector3(vector.x, vector.y, value),
                _ => vector
            };
        }

        /// <summary>
        /// 获取轴向索引 (0=X, 1=Y, 2=Z)
        /// </summary>
        /// <param name="axis">轴向</param>
        /// <returns>索引值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndex(this Axis axis) => (int)axis;

        /// <summary>
        /// 从索引获取轴向
        /// </summary>
        /// <param name="index">索引 (0-2)</param>
        /// <returns>轴向</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Axis FromIndex(int index) => (Axis)(index % 3);

        /// <summary>
        /// 获取下一个轴向 (X->Y->Z->X)
        /// </summary>
        /// <param name="axis">当前轴向</param>
        /// <returns>下一个轴向</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Axis Next(this Axis axis)
        {
            return axis switch
            {
                Axis.X => Axis.Y,
                Axis.Y => Axis.Z,
                Axis.Z => Axis.X,
                _ => Axis.X
            };
        }

        /// <summary>
        /// 获取上一个轴向 (X->Z->Y->X)
        /// </summary>
        /// <param name="axis">当前轴向</param>
        /// <returns>上一个轴向</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Axis Previous(this Axis axis)
        {
            return axis switch
            {
                Axis.X => Axis.Z,
                Axis.Y => Axis.X,
                Axis.Z => Axis.Y,
                _ => Axis.X
            };
        }
    }

    /// <summary>
    /// AxisFlags 枚举扩展方法
    /// </summary>
    public static class AxisFlagsExtensions
    {
        /// <summary>
        /// 检查是否包含指定轴
        /// </summary>
        /// <param name="flags">轴标志</param>
        /// <param name="axis">要检查的轴</param>
        /// <returns>如果包含返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAxis(this AxisFlags flags, Axis axis)
        {
            var flag = axis switch
            {
                Axis.X => AxisFlags.X,
                Axis.Y => AxisFlags.Y,
                Axis.Z => AxisFlags.Z,
                _ => AxisFlags.None
            };
            return (flags & flag) != 0;
        }

        /// <summary>
        /// 检查是否包含指定轴标志
        /// </summary>
        /// <param name="flags">轴标志</param>
        /// <param name="flag">要检查的标志</param>
        /// <returns>如果包含返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(this AxisFlags flags, AxisFlags flag)
        {
            return (flags & flag) == flag;
        }

        /// <summary>
        /// 获取包含的轴数量
        /// </summary>
        /// <param name="flags">轴标志</param>
        /// <returns>轴数量 (0-3)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this AxisFlags flags)
        {
            int count = 0;
            if ((flags & AxisFlags.X) != 0) count++;
            if ((flags & AxisFlags.Y) != 0) count++;
            if ((flags & AxisFlags.Z) != 0) count++;
            return count;
        }

        /// <summary>
        /// 根据轴标志创建掩码向量 (包含的轴为1，不包含的为0)
        /// </summary>
        /// <param name="flags">轴标志</param>
        /// <returns>掩码向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToMask(this AxisFlags flags)
        {
            return new Vector3(
                (flags & AxisFlags.X) != 0 ? 1f : 0f,
                (flags & AxisFlags.Y) != 0 ? 1f : 0f,
                (flags & AxisFlags.Z) != 0 ? 1f : 0f
            );
        }

        /// <summary>
        /// 应用轴标志掩码到向量
        /// </summary>
        /// <param name="flags">轴标志</param>
        /// <param name="vector">原向量</param>
        /// <returns>应用掩码后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ApplyMask(this AxisFlags flags, Vector3 vector)
        {
            return Vector3.Scale(vector, flags.ToMask());
        }
    }
}
