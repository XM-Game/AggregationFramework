// ==========================================================
// 文件名：Vector3Extensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Vector3 扩展方法
    /// <para>提供 Vector3 的数学运算和实用操作扩展</para>
    /// </summary>
    public static class Vector3Extensions
    {
        #region 分量操作

        /// <summary>
        /// 设置 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

        /// <summary>
        /// 设置 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        /// <summary>
        /// 设置 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        /// <summary>
        /// 设置 XY 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithXY(this Vector3 v, float x, float y) => new Vector3(x, y, v.z);

        /// <summary>
        /// 设置 XZ 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithXZ(this Vector3 v, float x, float z) => new Vector3(x, v.y, z);

        /// <summary>
        /// 设置 YZ 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithYZ(this Vector3 v, float y, float z) => new Vector3(v.x, y, z);

        /// <summary>
        /// 增加 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddX(this Vector3 v, float x) => new Vector3(v.x + x, v.y, v.z);

        /// <summary>
        /// 增加 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddY(this Vector3 v, float y) => new Vector3(v.x, v.y + y, v.z);

        /// <summary>
        /// 增加 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddZ(this Vector3 v, float z) => new Vector3(v.x, v.y, v.z + z);

        /// <summary>
        /// 乘以 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MultiplyX(this Vector3 v, float x) => new Vector3(v.x * x, v.y, v.z);

        /// <summary>
        /// 乘以 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MultiplyY(this Vector3 v, float y) => new Vector3(v.x, v.y * y, v.z);

        /// <summary>
        /// 乘以 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MultiplyZ(this Vector3 v, float z) => new Vector3(v.x, v.y, v.z * z);

        /// <summary>
        /// 取反 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NegateX(this Vector3 v) => new Vector3(-v.x, v.y, v.z);

        /// <summary>
        /// 取反 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NegateY(this Vector3 v) => new Vector3(v.x, -v.y, v.z);

        /// <summary>
        /// 取反 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NegateZ(this Vector3 v) => new Vector3(v.x, v.y, -v.z);

        /// <summary>
        /// 将 Y 分量设为 0 (投影到 XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flat(this Vector3 v) => new Vector3(v.x, 0f, v.z);

        /// <summary>
        /// 将 Y 分量设为指定值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FlatWithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        #endregion

        #region 数学运算

        /// <summary>
        /// 取绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        /// <summary>
        /// 向下取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Floor(this Vector3 v) => new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));

        /// <summary>
        /// 向上取整
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Ceil(this Vector3 v) => new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));

        /// <summary>
        /// 四舍五入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Round(this Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

        /// <summary>
        /// 钳制向量分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(this Vector3 v, float min, float max)
        {
            return new Vector3(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max));
        }

        /// <summary>
        /// 钳制向量分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
        }

        /// <summary>
        /// 钳制向量长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampMagnitude(this Vector3 v, float maxLength) => Vector3.ClampMagnitude(v, maxLength);

        /// <summary>
        /// 获取最大分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector3 v) => Mathf.Max(v.x, Mathf.Max(v.y, v.z));

        /// <summary>
        /// 获取最小分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector3 v) => Mathf.Min(v.x, Mathf.Min(v.y, v.z));

        /// <summary>
        /// 获取分量之和
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector3 v) => v.x + v.y + v.z;

        /// <summary>
        /// 获取分量之积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Product(this Vector3 v) => v.x * v.y * v.z;

        /// <summary>
        /// 获取分量平均值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Average(this Vector3 v) => (v.x + v.y + v.z) / 3f;

        /// <summary>
        /// 分量相乘
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Multiply(this Vector3 a, Vector3 b) => Vector3.Scale(a, b);

        /// <summary>
        /// 分量相除
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Divide(this Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

        #endregion

        #region 方向和角度

        /// <summary>
        /// 获取到另一个向量的角度 (度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleTo(this Vector3 from, Vector3 to) => Vector3.Angle(from, to);

        /// <summary>
        /// 获取到另一个向量的有符号角度 (度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngleTo(this Vector3 from, Vector3 to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        /// <summary>
        /// 获取指向目标的方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 DirectionTo(this Vector3 from, Vector3 to) => (to - from).normalized;

        /// <summary>
        /// 获取指向目标的方向 (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 DirectionToFlat(this Vector3 from, Vector3 to)
        {
            return new Vector3(to.x - from.x, 0f, to.z - from.z).normalized;
        }

        /// <summary>
        /// 绕轴旋转
        /// </summary>
        public static Vector3 RotateAround(this Vector3 v, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * v;
        }

        /// <summary>
        /// 绕 X 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateX(this Vector3 v, float angle) => Quaternion.Euler(angle, 0f, 0f) * v;

        /// <summary>
        /// 绕 Y 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateY(this Vector3 v, float angle) => Quaternion.Euler(0f, angle, 0f) * v;

        /// <summary>
        /// 绕 Z 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateZ(this Vector3 v, float angle) => Quaternion.Euler(0f, 0f, angle) * v;

        /// <summary>
        /// 投影到平面
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(v, planeNormal);
        }

        /// <summary>
        /// 投影到向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ProjectOnVector(this Vector3 v, Vector3 onNormal) => Vector3.Project(v, onNormal);

        /// <summary>
        /// 反射
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Reflect(this Vector3 v, Vector3 normal) => Vector3.Reflect(v, normal);

        #endregion

        #region 距离和插值

        /// <summary>
        /// 计算到另一个点的距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector3 from, Vector3 to) => Vector3.Distance(from, to);

        /// <summary>
        /// 计算到另一个点的平方距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistanceTo(this Vector3 from, Vector3 to) => (to - from).sqrMagnitude;

        /// <summary>
        /// 计算到另一个点的水平距离 (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalDistanceTo(this Vector3 from, Vector3 to)
        {
            float dx = to.x - from.x;
            float dz = to.z - from.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>
        /// 计算曼哈顿距离
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ManhattanDistanceTo(this Vector3 from, Vector3 to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y) + Mathf.Abs(to.z - from.z);
        }

        /// <summary>
        /// 线性插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LerpTo(this Vector3 from, Vector3 to, float t) => Vector3.Lerp(from, to, t);

        /// <summary>
        /// 无限制线性插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LerpUnclampedTo(this Vector3 from, Vector3 to, float t)
        {
            return Vector3.LerpUnclamped(from, to, t);
        }

        /// <summary>
        /// 球面插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SlerpTo(this Vector3 from, Vector3 to, float t) => Vector3.Slerp(from, to, t);

        /// <summary>
        /// 向目标移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            return Vector3.MoveTowards(current, target, maxDistanceDelta);
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        public static Vector3 SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref velocity, smoothTime);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否为零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector3 v) => v.sqrMagnitude < float.Epsilon;

        /// <summary>
        /// 检查是否近似为零向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector3 v, float tolerance = 0.0001f)
        {
            return v.sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return (a - b).sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>
        /// 检查是否为有效向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Vector3 v)
        {
            return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) &&
                   !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Vector2 (XY)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 v) => new Vector2(v.x, v.y);

        /// <summary>
        /// 转换为 Vector2 (XZ)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);

        /// <summary>
        /// 转换为 Vector3Int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        /// <summary>
        /// 转换为 Vector3Int (向下取整)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int FloorToInt(this Vector3 v)
        {
            return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
        }

        /// <summary>
        /// 转换为 Vector3Int (向上取整)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int CeilToInt(this Vector3 v)
        {
            return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
        }

        #endregion
    }
}
