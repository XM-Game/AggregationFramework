// ==========================================================
// 文件名：Vector4Extensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Vector4 扩展方法
    /// <para>提供 Vector4 的数学运算和实用操作扩展</para>
    /// </summary>
    public static class Vector4Extensions
    {
        #region 分量操作

        /// <summary>设置 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 WithX(this Vector4 v, float x) => new Vector4(x, v.y, v.z, v.w);

        /// <summary>设置 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 WithY(this Vector4 v, float y) => new Vector4(v.x, y, v.z, v.w);

        /// <summary>设置 Z 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 WithZ(this Vector4 v, float z) => new Vector4(v.x, v.y, z, v.w);

        /// <summary>设置 W 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 WithW(this Vector4 v, float w) => new Vector4(v.x, v.y, v.z, w);

        /// <summary>增加 X 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddX(this Vector4 v, float x) => new Vector4(v.x + x, v.y, v.z, v.w);

        /// <summary>增加 Y 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddY(this Vector4 v, float y) => new Vector4(v.x, v.y + y, v.z, v.w);

        /// <summary>增加 Z 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddZ(this Vector4 v, float z) => new Vector4(v.x, v.y, v.z + z, v.w);

        /// <summary>增加 W 分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 AddW(this Vector4 v, float w) => new Vector4(v.x, v.y, v.z, v.w + w);

        #endregion

        #region 数学运算

        /// <summary>取绝对值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Abs(this Vector4 v) => new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));

        /// <summary>向下取整</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Floor(this Vector4 v) => new Vector4(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z), Mathf.Floor(v.w));

        /// <summary>向上取整</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Ceil(this Vector4 v) => new Vector4(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z), Mathf.Ceil(v.w));

        /// <summary>四舍五入</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Round(this Vector4 v) => new Vector4(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z), Mathf.Round(v.w));

        /// <summary>钳制向量分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(this Vector4 v, float min, float max)
        {
            return new Vector4(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max), Mathf.Clamp(v.z, min, max), Mathf.Clamp(v.w, min, max));
        }

        /// <summary>获取最大分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Vector4 v) => Mathf.Max(Mathf.Max(v.x, v.y), Mathf.Max(v.z, v.w));

        /// <summary>获取最小分量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Vector4 v) => Mathf.Min(Mathf.Min(v.x, v.y), Mathf.Min(v.z, v.w));

        /// <summary>获取分量之和</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(this Vector4 v) => v.x + v.y + v.z + v.w;

        /// <summary>获取分量之积</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Product(this Vector4 v) => v.x * v.y * v.z * v.w;

        /// <summary>获取分量平均值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Average(this Vector4 v) => (v.x + v.y + v.z + v.w) * 0.25f;

        /// <summary>分量相乘</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Multiply(this Vector4 a, Vector4 b) => Vector4.Scale(a, b);

        #endregion

        #region 距离和插值

        /// <summary>计算到另一个点的距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector4 from, Vector4 to) => Vector4.Distance(from, to);

        /// <summary>计算到另一个点的平方距离</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistanceTo(this Vector4 from, Vector4 to) => (to - from).sqrMagnitude;

        /// <summary>线性插值到目标</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 LerpTo(this Vector4 from, Vector4 to, float t) => Vector4.Lerp(from, to, t);

        /// <summary>无限制线性插值到目标</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 LerpUnclampedTo(this Vector4 from, Vector4 to, float t) => Vector4.LerpUnclamped(from, to, t);

        /// <summary>向目标移动</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 MoveTowards(this Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            return Vector4.MoveTowards(current, target, maxDistanceDelta);
        }

        #endregion

        #region 检查和比较

        /// <summary>检查是否为零向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector4 v) => v.sqrMagnitude < float.Epsilon;

        /// <summary>检查是否近似为零向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector4 v, float tolerance = 0.0001f) => v.sqrMagnitude < tolerance * tolerance;

        /// <summary>检查是否近似相等</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Vector4 a, Vector4 b, float tolerance = 0.0001f)
        {
            return (a - b).sqrMagnitude < tolerance * tolerance;
        }

        /// <summary>检查是否为有效向量</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this Vector4 v)
        {
            return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsNaN(v.w) &&
                   !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z) && !float.IsInfinity(v.w);
        }

        #endregion

        #region 转换

        /// <summary>转换为 Vector2</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector4 v) => new Vector2(v.x, v.y);

        /// <summary>转换为 Vector3</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector4 v) => new Vector3(v.x, v.y, v.z);

        /// <summary>转换为 Color</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this Vector4 v) => new Color(v.x, v.y, v.z, v.w);

        /// <summary>转换为 Quaternion</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToQuaternion(this Vector4 v) => new Quaternion(v.x, v.y, v.z, v.w);

        #endregion
    }
}
