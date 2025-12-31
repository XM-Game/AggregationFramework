// ==========================================================
// 文件名：TransformExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 组件扩展方法
    /// <para>提供 Transform 的基础操作扩展，包括重置、复制、空间转换等功能</para>
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 重置操作

        /// <summary>
        /// 重置 Transform 的本地位置、旋转和缩放为默认值
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform;
        }

        /// <summary>
        /// 重置 Transform 的本地位置为零点
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ResetPosition(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            return transform;
        }

        /// <summary>
        /// 重置 Transform 的本地旋转为单位四元数
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ResetRotation(this Transform transform)
        {
            transform.localRotation = Quaternion.identity;
            return transform;
        }

        /// <summary>
        /// 重置 Transform 的本地缩放为 (1,1,1)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ResetScale(this Transform transform)
        {
            transform.localScale = Vector3.one;
            return transform;
        }

        #endregion

        #region 复制操作

        /// <summary>
        /// 复制另一个 Transform 的位置、旋转和缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="source">源 Transform</param>
        /// <param name="useWorldSpace">是否使用世界空间坐标</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform CopyFrom(this Transform transform, Transform source, bool useWorldSpace = true)
        {
            if (source == null) return transform;

            if (useWorldSpace)
            {
                transform.position = source.position;
                transform.rotation = source.rotation;
                transform.localScale = source.lossyScale;
            }
            else
            {
                transform.localPosition = source.localPosition;
                transform.localRotation = source.localRotation;
                transform.localScale = source.localScale;
            }
            return transform;
        }

        /// <summary>
        /// 仅复制另一个 Transform 的位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="source">源 Transform</param>
        /// <param name="useWorldSpace">是否使用世界空间坐标</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CopyPositionFrom(this Transform transform, Transform source, bool useWorldSpace = true)
        {
            if (source == null) return transform;

            if (useWorldSpace)
                transform.position = source.position;
            else
                transform.localPosition = source.localPosition;
            return transform;
        }

        /// <summary>
        /// 仅复制另一个 Transform 的旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="source">源 Transform</param>
        /// <param name="useWorldSpace">是否使用世界空间坐标</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CopyRotationFrom(this Transform transform, Transform source, bool useWorldSpace = true)
        {
            if (source == null) return transform;

            if (useWorldSpace)
                transform.rotation = source.rotation;
            else
                transform.localRotation = source.localRotation;
            return transform;
        }

        /// <summary>
        /// 仅复制另一个 Transform 的缩放
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="source">源 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CopyScaleFrom(this Transform transform, Transform source)
        {
            if (source == null) return transform;
            transform.localScale = source.localScale;
            return transform;
        }

        #endregion

        #region 方向向量

        /// <summary>
        /// 获取 Transform 的前方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>前方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetForward(this Transform transform) => transform.forward;

        /// <summary>
        /// 获取 Transform 的后方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>后方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetBack(this Transform transform) => -transform.forward;

        /// <summary>
        /// 获取 Transform 的右方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>右方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRight(this Transform transform) => transform.right;

        /// <summary>
        /// 获取 Transform 的左方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>左方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLeft(this Transform transform) => -transform.right;

        /// <summary>
        /// 获取 Transform 的上方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>上方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetUp(this Transform transform) => transform.up;

        /// <summary>
        /// 获取 Transform 的下方向量 (世界空间)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>下方向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetDown(this Transform transform) => -transform.up;

        #endregion

        #region 距离计算

        /// <summary>
        /// 计算到另一个 Transform 的距离
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="other">目标 Transform</param>
        /// <returns>两者之间的距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Transform transform, Transform other)
        {
            return Vector3.Distance(transform.position, other.position);
        }

        /// <summary>
        /// 计算到指定位置的距离
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <returns>到目标位置的距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Transform transform, Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        /// <summary>
        /// 计算到另一个 Transform 的平方距离 (性能优化，避免开方运算)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="other">目标 Transform</param>
        /// <returns>两者之间的平方距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistanceTo(this Transform transform, Transform other)
        {
            return (transform.position - other.position).sqrMagnitude;
        }

        /// <summary>
        /// 计算到指定位置的平方距离 (性能优化，避免开方运算)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <returns>到目标位置的平方距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistanceTo(this Transform transform, Vector3 position)
        {
            return (transform.position - position).sqrMagnitude;
        }

        /// <summary>
        /// 计算 XZ 平面上到另一个 Transform 的距离 (忽略 Y 轴)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="other">目标 Transform</param>
        /// <returns>XZ 平面上的距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalDistanceTo(this Transform transform, Transform other)
        {
            var pos1 = transform.position;
            var pos2 = other.position;
            var dx = pos1.x - pos2.x;
            var dz = pos1.z - pos2.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>
        /// 计算 XZ 平面上到指定位置的距离 (忽略 Y 轴)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <returns>XZ 平面上的距离</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalDistanceTo(this Transform transform, Vector3 position)
        {
            var pos = transform.position;
            var dx = pos.x - position.x;
            var dz = pos.z - position.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        #endregion

        #region 方向计算

        /// <summary>
        /// 获取指向另一个 Transform 的方向向量 (已归一化)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <returns>指向目标的单位方向向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 DirectionTo(this Transform transform, Transform target)
        {
            return (target.position - transform.position).normalized;
        }

        /// <summary>
        /// 获取指向指定位置的方向向量 (已归一化)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <returns>指向目标的单位方向向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 DirectionTo(this Transform transform, Vector3 position)
        {
            return (position - transform.position).normalized;
        }

        /// <summary>
        /// 获取 XZ 平面上指向另一个 Transform 的方向向量 (已归一化，Y 分量为 0)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <returns>XZ 平面上指向目标的单位方向向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 HorizontalDirectionTo(this Transform transform, Transform target)
        {
            var direction = target.position - transform.position;
            direction.y = 0f;
            return direction.normalized;
        }

        /// <summary>
        /// 获取 XZ 平面上指向指定位置的方向向量 (已归一化，Y 分量为 0)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <returns>XZ 平面上指向目标的单位方向向量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 HorizontalDirectionTo(this Transform transform, Vector3 position)
        {
            var direction = position - transform.position;
            direction.y = 0f;
            return direction.normalized;
        }

        #endregion

        #region 范围检测

        /// <summary>
        /// 检查另一个 Transform 是否在指定范围内
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="other">目标 Transform</param>
        /// <param name="range">范围半径</param>
        /// <returns>如果在范围内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this Transform transform, Transform other, float range)
        {
            return transform.SqrDistanceTo(other) <= range * range;
        }

        /// <summary>
        /// 检查指定位置是否在范围内
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="position">目标位置</param>
        /// <param name="range">范围半径</param>
        /// <returns>如果在范围内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this Transform transform, Vector3 position, float range)
        {
            return transform.SqrDistanceTo(position) <= range * range;
        }

        #endregion
    }
}
