// ==========================================================
// 文件名：QuaternionExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Quaternion 扩展方法
    /// <para>提供 Quaternion 的基础操作和转换扩展</para>
    /// </summary>
    public static class QuaternionExtensions
    {
        #region 欧拉角操作

        /// <summary>
        /// 设置 X 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion WithEulerX(this Quaternion q, float x)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(x, euler.y, euler.z);
        }

        /// <summary>
        /// 设置 Y 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion WithEulerY(this Quaternion q, float y)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, y, euler.z);
        }

        /// <summary>
        /// 设置 Z 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion WithEulerZ(this Quaternion q, float z)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, euler.y, z);
        }

        /// <summary>
        /// 增加 X 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion AddEulerX(this Quaternion q, float x)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x + x, euler.y, euler.z);
        }

        /// <summary>
        /// 增加 Y 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion AddEulerY(this Quaternion q, float y)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, euler.y + y, euler.z);
        }

        /// <summary>
        /// 增加 Z 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion AddEulerZ(this Quaternion q, float z)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, euler.y, euler.z + z);
        }

        /// <summary>
        /// 获取 X 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetEulerX(this Quaternion q) => q.eulerAngles.x;

        /// <summary>
        /// 获取 Y 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetEulerY(this Quaternion q) => q.eulerAngles.y;

        /// <summary>
        /// 获取 Z 轴欧拉角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetEulerZ(this Quaternion q) => q.eulerAngles.z;

        #endregion

        #region 方向向量

        /// <summary>
        /// 获取前方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Forward(this Quaternion q) => q * Vector3.forward;

        /// <summary>
        /// 获取后方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Back(this Quaternion q) => q * Vector3.back;

        /// <summary>
        /// 获取上方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Up(this Quaternion q) => q * Vector3.up;

        /// <summary>
        /// 获取下方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Down(this Quaternion q) => q * Vector3.down;

        /// <summary>
        /// 获取右方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Right(this Quaternion q) => q * Vector3.right;

        /// <summary>
        /// 获取左方向量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Left(this Quaternion q) => q * Vector3.left;

        #endregion

        #region 旋转操作

        /// <summary>
        /// 绕 X 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateX(this Quaternion q, float angle)
        {
            return q * Quaternion.Euler(angle, 0f, 0f);
        }

        /// <summary>
        /// 绕 Y 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateY(this Quaternion q, float angle)
        {
            return q * Quaternion.Euler(0f, angle, 0f);
        }

        /// <summary>
        /// 绕 Z 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateZ(this Quaternion q, float angle)
        {
            return q * Quaternion.Euler(0f, 0f, angle);
        }

        /// <summary>
        /// 绕任意轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateAround(this Quaternion q, Vector3 axis, float angle)
        {
            return q * Quaternion.AngleAxis(angle, axis);
        }

        /// <summary>
        /// 绕世界 X 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateWorldX(this Quaternion q, float angle)
        {
            return Quaternion.Euler(angle, 0f, 0f) * q;
        }

        /// <summary>
        /// 绕世界 Y 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateWorldY(this Quaternion q, float angle)
        {
            return Quaternion.Euler(0f, angle, 0f) * q;
        }

        /// <summary>
        /// 绕世界 Z 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateWorldZ(this Quaternion q, float angle)
        {
            return Quaternion.Euler(0f, 0f, angle) * q;
        }

        #endregion

        #region 插值

        /// <summary>
        /// 球面插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion SlerpTo(this Quaternion from, Quaternion to, float t)
        {
            return Quaternion.Slerp(from, to, t);
        }

        /// <summary>
        /// 无限制球面插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion SlerpUnclampedTo(this Quaternion from, Quaternion to, float t)
        {
            return Quaternion.SlerpUnclamped(from, to, t);
        }

        /// <summary>
        /// 线性插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LerpTo(this Quaternion from, Quaternion to, float t)
        {
            return Quaternion.Lerp(from, to, t);
        }

        /// <summary>
        /// 无限制线性插值到目标
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LerpUnclampedTo(this Quaternion from, Quaternion to, float t)
        {
            return Quaternion.LerpUnclamped(from, to, t);
        }

        /// <summary>
        /// 向目标旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RotateTowards(this Quaternion from, Quaternion to, float maxDegreesDelta)
        {
            return Quaternion.RotateTowards(from, to, maxDegreesDelta);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否为单位四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentity(this Quaternion q)
        {
            return Mathf.Approximately(q.x, 0f) && Mathf.Approximately(q.y, 0f) &&
                   Mathf.Approximately(q.z, 0f) && Mathf.Approximately(q.w, 1f);
        }

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Quaternion a, Quaternion b, float tolerance = 0.0001f)
        {
            return Quaternion.Angle(a, b) < tolerance;
        }

        /// <summary>
        /// 检查是否为有效四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Quaternion q)
        {
            return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) &&
                   !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
        }

        /// <summary>
        /// 获取到另一个四元数的角度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleTo(this Quaternion from, Quaternion to)
        {
            return Quaternion.Angle(from, to);
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Vector4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);

        /// <summary>
        /// 转换为欧拉角 Vector3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToEulerAngles(this Quaternion q) => q.eulerAngles;

        /// <summary>
        /// 转换为轴角表示
        /// </summary>
        public static (Vector3 axis, float angle) ToAxisAngle(this Quaternion q)
        {
            q.ToAngleAxis(out float angle, out Vector3 axis);
            return (axis, angle);
        }

        /// <summary>
        /// 获取共轭四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Conjugate(this Quaternion q) => new Quaternion(-q.x, -q.y, -q.z, q.w);

        #endregion

        #region 约束

        /// <summary>
        /// 仅保留 Y 轴旋转 (水平旋转)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FlattenToY(this Quaternion q)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(0f, euler.y, 0f);
        }

        /// <summary>
        /// 仅保留 X 轴旋转 (俯仰)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FlattenToX(this Quaternion q)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, 0f, 0f);
        }

        /// <summary>
        /// 仅保留 Z 轴旋转 (翻滚)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FlattenToZ(this Quaternion q)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(0f, 0f, euler.z);
        }

        /// <summary>
        /// 钳制 X 轴欧拉角
        /// </summary>
        public static Quaternion ClampEulerX(this Quaternion q, float min, float max)
        {
            var euler = q.eulerAngles;
            float x = euler.x > 180f ? euler.x - 360f : euler.x;
            x = Mathf.Clamp(x, min, max);
            return Quaternion.Euler(x, euler.y, euler.z);
        }

        /// <summary>
        /// 钳制 Y 轴欧拉角
        /// </summary>
        public static Quaternion ClampEulerY(this Quaternion q, float min, float max)
        {
            var euler = q.eulerAngles;
            float y = euler.y > 180f ? euler.y - 360f : euler.y;
            y = Mathf.Clamp(y, min, max);
            return Quaternion.Euler(euler.x, y, euler.z);
        }

        /// <summary>
        /// 钳制 Z 轴欧拉角
        /// </summary>
        public static Quaternion ClampEulerZ(this Quaternion q, float min, float max)
        {
            var euler = q.eulerAngles;
            float z = euler.z > 180f ? euler.z - 360f : euler.z;
            z = Mathf.Clamp(z, min, max);
            return Quaternion.Euler(euler.x, euler.y, z);
        }

        #endregion
    }
}
