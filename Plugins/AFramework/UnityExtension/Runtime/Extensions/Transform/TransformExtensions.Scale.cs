// ==========================================================
// 文件名：TransformExtensions.Scale.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 缩放操作扩展方法
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 本地缩放设置

        /// <summary>
        /// 设置本地缩放 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleX(this Transform transform, float x)
        {
            var scale = transform.localScale;
            scale.x = x;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置本地缩放 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleY(this Transform transform, float y)
        {
            var scale = transform.localScale;
            scale.y = y;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置本地缩放 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="z">Z 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleZ(this Transform transform, float z)
        {
            var scale = transform.localScale;
            scale.z = z;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置本地缩放 XY 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 缩放值</param>
        /// <param name="y">Y 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleXY(this Transform transform, float x, float y)
        {
            var scale = transform.localScale;
            scale.x = x;
            scale.y = y;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置本地缩放 XZ 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 缩放值</param>
        /// <param name="z">Z 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleXZ(this Transform transform, float x, float z)
        {
            var scale = transform.localScale;
            scale.x = x;
            scale.z = z;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置本地缩放 YZ 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 缩放值</param>
        /// <param name="z">Z 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleYZ(this Transform transform, float y, float z)
        {
            var scale = transform.localScale;
            scale.y = y;
            scale.z = z;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 设置统一的本地缩放 (所有轴相同)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="uniformScale">统一缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScaleUniform(this Transform transform, float uniformScale)
        {
            transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
            return transform;
        }

        /// <summary>
        /// 设置本地缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 缩放值</param>
        /// <param name="y">Y 缩放值</param>
        /// <param name="z">Z 缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalScale(this Transform transform, float x, float y, float z)
        {
            transform.localScale = new Vector3(x, y, z);
            return transform;
        }

        #endregion

        #region 缩放增量操作

        /// <summary>
        /// 增加本地缩放 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 缩放增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalScaleX(this Transform transform, float deltaX)
        {
            var scale = transform.localScale;
            scale.x += deltaX;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 增加本地缩放 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaY">Y 缩放增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalScaleY(this Transform transform, float deltaY)
        {
            var scale = transform.localScale;
            scale.y += deltaY;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 增加本地缩放 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaZ">Z 缩放增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalScaleZ(this Transform transform, float deltaZ)
        {
            var scale = transform.localScale;
            scale.z += deltaZ;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 增加本地缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="delta">缩放增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalScale(this Transform transform, Vector3 delta)
        {
            transform.localScale += delta;
            return transform;
        }

        /// <summary>
        /// 统一增加本地缩放 (所有轴相同增量)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="delta">统一缩放增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalScaleUniform(this Transform transform, float delta)
        {
            transform.localScale += new Vector3(delta, delta, delta);
            return transform;
        }

        #endregion

        #region 缩放乘法操作

        /// <summary>
        /// 将本地缩放乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalScale(this Transform transform, float multiplier)
        {
            transform.localScale *= multiplier;
            return transform;
        }

        /// <summary>
        /// 将本地缩放按分量乘以向量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数向量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalScale(this Transform transform, Vector3 multiplier)
        {
            transform.localScale = Vector3.Scale(transform.localScale, multiplier);
            return transform;
        }

        /// <summary>
        /// 将本地缩放 X 分量乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalScaleX(this Transform transform, float multiplier)
        {
            var scale = transform.localScale;
            scale.x *= multiplier;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放 Y 分量乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalScaleY(this Transform transform, float multiplier)
        {
            var scale = transform.localScale;
            scale.y *= multiplier;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放 Z 分量乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalScaleZ(this Transform transform, float multiplier)
        {
            var scale = transform.localScale;
            scale.z *= multiplier;
            transform.localScale = scale;
            return transform;
        }

        #endregion

        #region 缩放钳制

        /// <summary>
        /// 将本地缩放钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小缩放值</param>
        /// <param name="max">最大缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ClampLocalScale(this Transform transform, Vector3 min, Vector3 max)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Clamp(scale.x, min.x, max.x);
            scale.y = Mathf.Clamp(scale.y, min.y, max.y);
            scale.z = Mathf.Clamp(scale.z, min.z, max.z);
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放钳制在统一范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小缩放值</param>
        /// <param name="max">最大缩放值</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ClampLocalScaleUniform(this Transform transform, float min, float max)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Clamp(scale.x, min, max);
            scale.y = Mathf.Clamp(scale.y, min, max);
            scale.z = Mathf.Clamp(scale.z, min, max);
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放 X 分量钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampLocalScaleX(this Transform transform, float min, float max)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Clamp(scale.x, min, max);
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放 Y 分量钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampLocalScaleY(this Transform transform, float min, float max)
        {
            var scale = transform.localScale;
            scale.y = Mathf.Clamp(scale.y, min, max);
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 将本地缩放 Z 分量钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampLocalScaleZ(this Transform transform, float min, float max)
        {
            var scale = transform.localScale;
            scale.z = Mathf.Clamp(scale.z, min, max);
            transform.localScale = scale;
            return transform;
        }

        #endregion

        #region 缩放插值

        /// <summary>
        /// 将本地缩放线性插值到目标缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标缩放</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpLocalScale(this Transform transform, Vector3 target, float t)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, t);
            return transform;
        }

        /// <summary>
        /// 将本地缩放线性插值到目标 Transform 的缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpLocalScale(this Transform transform, Transform target, float t)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, t);
            return transform;
        }

        /// <summary>
        /// 将本地缩放统一插值到目标值
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="targetUniformScale">目标统一缩放值</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpLocalScaleUniform(this Transform transform, float targetUniformScale, float t)
        {
            var target = new Vector3(targetUniformScale, targetUniformScale, targetUniformScale);
            transform.localScale = Vector3.Lerp(transform.localScale, target, t);
            return transform;
        }

        #endregion

        #region 缩放翻转

        /// <summary>
        /// 翻转本地缩放 X 分量 (取反)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FlipLocalScaleX(this Transform transform)
        {
            var scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 翻转本地缩放 Y 分量 (取反)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FlipLocalScaleY(this Transform transform)
        {
            var scale = transform.localScale;
            scale.y = -scale.y;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 翻转本地缩放 Z 分量 (取反)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FlipLocalScaleZ(this Transform transform)
        {
            var scale = transform.localScale;
            scale.z = -scale.z;
            transform.localScale = scale;
            return transform;
        }

        /// <summary>
        /// 翻转所有本地缩放分量 (取反)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FlipLocalScale(this Transform transform)
        {
            transform.localScale = -transform.localScale;
            return transform;
        }

        #endregion

        #region 缩放查询

        /// <summary>
        /// 检查是否为统一缩放 (所有轴缩放值相同)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="tolerance">容差值</param>
        /// <returns>如果是统一缩放返回 true</returns>
        public static bool IsUniformScale(this Transform transform, float tolerance = 0.0001f)
        {
            var scale = transform.localScale;
            return Mathf.Abs(scale.x - scale.y) <= tolerance &&
                   Mathf.Abs(scale.y - scale.z) <= tolerance;
        }

        /// <summary>
        /// 检查是否有负缩放 (任意轴为负值)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>如果有负缩放返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNegativeScale(this Transform transform)
        {
            var scale = transform.localScale;
            return scale.x < 0f || scale.y < 0f || scale.z < 0f;
        }

        /// <summary>
        /// 获取缩放的绝对值
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>缩放绝对值向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetAbsoluteLocalScale(this Transform transform)
        {
            var scale = transform.localScale;
            return new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        }

        /// <summary>
        /// 获取最大缩放分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>最大缩放值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetMaxLocalScale(this Transform transform)
        {
            var scale = transform.localScale;
            return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
        }

        /// <summary>
        /// 获取最小缩放分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>最小缩放值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetMinLocalScale(this Transform transform)
        {
            var scale = transform.localScale;
            return Mathf.Min(scale.x, Mathf.Min(scale.y, scale.z));
        }

        /// <summary>
        /// 获取平均缩放值
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>平均缩放值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAverageLocalScale(this Transform transform)
        {
            var scale = transform.localScale;
            return (scale.x + scale.y + scale.z) / 3f;
        }

        #endregion
    }
}
