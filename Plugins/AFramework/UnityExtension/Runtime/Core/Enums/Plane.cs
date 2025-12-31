// ==========================================================
// 文件名：Plane.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 平面枚举
    /// <para>表示 3D 空间中的三个基本平面</para>
    /// </summary>
    [Serializable]
    public enum PlaneType
    {
        /// <summary>XY 平面 (垂直于 Z 轴，正面朝前)</summary>
        XY = 0,

        /// <summary>XZ 平面 (垂直于 Y 轴，水平面)</summary>
        XZ = 1,

        /// <summary>YZ 平面 (垂直于 X 轴，侧面)</summary>
        YZ = 2
    }

    /// <summary>
    /// PlaneType 扩展方法
    /// </summary>
    public static class PlaneTypeExtensions
    {
        /// <summary>
        /// 获取平面的法线向量
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <returns>法线向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetNormal(this PlaneType plane)
        {
            return plane switch
            {
                PlaneType.XY => Vector3.forward,
                PlaneType.XZ => Vector3.up,
                PlaneType.YZ => Vector3.right,
                _ => Vector3.up
            };
        }

        /// <summary>
        /// 获取平面包含的两个轴
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <returns>包含的轴标志</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AxisFlags GetAxes(this PlaneType plane)
        {
            return plane switch
            {
                PlaneType.XY => AxisFlags.XY,
                PlaneType.XZ => AxisFlags.XZ,
                PlaneType.YZ => AxisFlags.YZ,
                _ => AxisFlags.XZ
            };
        }

        /// <summary>
        /// 获取平面垂直的轴
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <returns>垂直轴</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Axis GetPerpendicularAxis(this PlaneType plane)
        {
            return plane switch
            {
                PlaneType.XY => Axis.Z,
                PlaneType.XZ => Axis.Y,
                PlaneType.YZ => Axis.X,
                _ => Axis.Y
            };
        }

        /// <summary>
        /// 将 3D 向量投影到平面上
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <param name="vector">3D 向量</param>
        /// <returns>投影后的向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Project(this PlaneType plane, Vector3 vector)
        {
            return plane switch
            {
                PlaneType.XY => new Vector3(vector.x, vector.y, 0f),
                PlaneType.XZ => new Vector3(vector.x, 0f, vector.z),
                PlaneType.YZ => new Vector3(0f, vector.y, vector.z),
                _ => vector
            };
        }

        /// <summary>
        /// 将 3D 向量转换为平面上的 2D 向量
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <param name="vector">3D 向量</param>
        /// <returns>2D 向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this PlaneType plane, Vector3 vector)
        {
            return plane switch
            {
                PlaneType.XY => new Vector2(vector.x, vector.y),
                PlaneType.XZ => new Vector2(vector.x, vector.z),
                PlaneType.YZ => new Vector2(vector.y, vector.z),
                _ => new Vector2(vector.x, vector.z)
            };
        }

        /// <summary>
        /// 将 2D 向量转换为平面上的 3D 向量
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <param name="vector">2D 向量</param>
        /// <param name="perpendicularValue">垂直轴的值</param>
        /// <returns>3D 向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this PlaneType plane, Vector2 vector, float perpendicularValue = 0f)
        {
            return plane switch
            {
                PlaneType.XY => new Vector3(vector.x, vector.y, perpendicularValue),
                PlaneType.XZ => new Vector3(vector.x, perpendicularValue, vector.y),
                PlaneType.YZ => new Vector3(perpendicularValue, vector.x, vector.y),
                _ => new Vector3(vector.x, perpendicularValue, vector.y)
            };
        }

        /// <summary>
        /// 计算点到平面的距离
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <param name="point">点</param>
        /// <param name="planePosition">平面位置 (垂直轴上的值)</param>
        /// <returns>距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDistance(this PlaneType plane, Vector3 point, float planePosition = 0f)
        {
            float value = plane switch
            {
                PlaneType.XY => point.z,
                PlaneType.XZ => point.y,
                PlaneType.YZ => point.x,
                _ => point.y
            };
            return Mathf.Abs(value - planePosition);
        }

        /// <summary>
        /// 创建 Unity Plane 结构
        /// </summary>
        /// <param name="planeType">平面类型</param>
        /// <param name="distance">平面到原点的距离</param>
        /// <returns>Unity Plane</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnityEngine.Plane CreatePlane(this PlaneType planeType, float distance = 0f)
        {
            return new UnityEngine.Plane(planeType.GetNormal(), distance);
        }

        /// <summary>
        /// 获取平面的第一个轴向量
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <returns>第一个轴向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetFirstAxis(this PlaneType plane)
        {
            return plane switch
            {
                PlaneType.XY => Vector3.right,
                PlaneType.XZ => Vector3.right,
                PlaneType.YZ => Vector3.up,
                _ => Vector3.right
            };
        }

        /// <summary>
        /// 获取平面的第二个轴向量
        /// </summary>
        /// <param name="plane">平面类型</param>
        /// <returns>第二个轴向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetSecondAxis(this PlaneType plane)
        {
            return plane switch
            {
                PlaneType.XY => Vector3.up,
                PlaneType.XZ => Vector3.forward,
                PlaneType.YZ => Vector3.forward,
                _ => Vector3.forward
            };
        }
    }
}
