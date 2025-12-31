// ==========================================================
// 文件名：TransformExtensions.Position.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 位置操作扩展方法
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 世界空间位置设置

        /// <summary>
        /// 设置世界空间 X 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x = x;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 设置世界空间 Y 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionY(this Transform transform, float y)
        {
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 设置世界空间 Z 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionZ(this Transform transform, float z)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 设置世界空间 XY 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <param name="y">Y 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionXY(this Transform transform, float x, float y)
        {
            var pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 设置世界空间 XZ 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionXZ(this Transform transform, float x, float z)
        {
            var pos = transform.position;
            pos.x = x;
            pos.z = z;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 设置世界空间 YZ 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 坐标值</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionYZ(this Transform transform, float y, float z)
        {
            var pos = transform.position;
            pos.y = y;
            pos.z = z;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 使用 Vector2 设置世界空间 XY 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">XY 坐标</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionXY(this Transform transform, Vector2 position)
        {
            var pos = transform.position;
            pos.x = position.x;
            pos.y = position.y;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 使用 Vector2 设置世界空间 XZ 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="position">XZ 坐标 (x=X, y=Z)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetPositionXZ(this Transform transform, Vector2 position)
        {
            var pos = transform.position;
            pos.x = position.x;
            pos.z = position.y;
            transform.position = pos;
            return transform;
        }

        #endregion

        #region 本地空间位置设置

        /// <summary>
        /// 设置本地空间 X 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionX(this Transform transform, float x)
        {
            var pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 设置本地空间 Y 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionY(this Transform transform, float y)
        {
            var pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 设置本地空间 Z 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionZ(this Transform transform, float z)
        {
            var pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 设置本地空间 XY 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <param name="y">Y 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionXY(this Transform transform, float x, float y)
        {
            var pos = transform.localPosition;
            pos.x = x;
            pos.y = y;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 设置本地空间 XZ 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 坐标值</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionXZ(this Transform transform, float x, float z)
        {
            var pos = transform.localPosition;
            pos.x = x;
            pos.z = z;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 设置本地空间 YZ 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 坐标值</param>
        /// <param name="z">Z 坐标值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalPositionYZ(this Transform transform, float y, float z)
        {
            var pos = transform.localPosition;
            pos.y = y;
            pos.z = z;
            transform.localPosition = pos;
            return transform;
        }

        #endregion

        #region 位置增量操作

        /// <summary>
        /// 在世界空间中增加 X 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddPositionX(this Transform transform, float deltaX)
        {
            var pos = transform.position;
            pos.x += deltaX;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加 Y 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaY">Y 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddPositionY(this Transform transform, float deltaY)
        {
            var pos = transform.position;
            pos.y += deltaY;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加 Z 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaZ">Z 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddPositionZ(this Transform transform, float deltaZ)
        {
            var pos = transform.position;
            pos.z += deltaZ;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="delta">位置增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddPosition(this Transform transform, Vector3 delta)
        {
            transform.position += delta;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 增量</param>
        /// <param name="deltaY">Y 增量</param>
        /// <param name="deltaZ">Z 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddPosition(this Transform transform, float deltaX, float deltaY, float deltaZ)
        {
            var pos = transform.position;
            pos.x += deltaX;
            pos.y += deltaY;
            pos.z += deltaZ;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加 X 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalPositionX(this Transform transform, float deltaX)
        {
            var pos = transform.localPosition;
            pos.x += deltaX;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加 Y 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaY">Y 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalPositionY(this Transform transform, float deltaY)
        {
            var pos = transform.localPosition;
            pos.y += deltaY;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加 Z 坐标
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaZ">Z 增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalPositionZ(this Transform transform, float deltaZ)
        {
            var pos = transform.localPosition;
            pos.z += deltaZ;
            transform.localPosition = pos;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="delta">位置增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalPosition(this Transform transform, Vector3 delta)
        {
            transform.localPosition += delta;
            return transform;
        }

        #endregion

        #region 位置乘法操作

        /// <summary>
        /// 将世界空间位置乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyPosition(this Transform transform, float multiplier)
        {
            transform.position *= multiplier;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置按分量乘以向量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数向量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyPosition(this Transform transform, Vector3 multiplier)
        {
            transform.position = Vector3.Scale(transform.position, multiplier);
            return transform;
        }

        /// <summary>
        /// 将本地空间位置乘以标量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="multiplier">乘数</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MultiplyLocalPosition(this Transform transform, float multiplier)
        {
            transform.localPosition *= multiplier;
            return transform;
        }

        #endregion

        #region 位置钳制

        /// <summary>
        /// 将世界空间位置钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform ClampPosition(this Transform transform, Vector3 min, Vector3 max)
        {
            var pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            pos.z = Mathf.Clamp(pos.z, min.z, max.z);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置钳制在 Bounds 范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="bounds">边界范围</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampPosition(this Transform transform, Bounds bounds)
        {
            return transform.ClampPosition(bounds.min, bounds.max);
        }

        /// <summary>
        /// 将世界空间 X 坐标钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampPositionX(this Transform transform, float min, float max)
        {
            var pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, min, max);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间 Y 坐标钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampPositionY(this Transform transform, float min, float max)
        {
            var pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, min, max);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间 Z 坐标钳制在指定范围内
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ClampPositionZ(this Transform transform, float min, float max)
        {
            var pos = transform.position;
            pos.z = Mathf.Clamp(pos.z, min, max);
            transform.position = pos;
            return transform;
        }

        #endregion

        #region 位置取整

        /// <summary>
        /// 将世界空间位置四舍五入到最近的整数
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RoundPosition(this Transform transform)
        {
            var pos = transform.position;
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = Mathf.Round(pos.z);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置向下取整
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FloorPosition(this Transform transform)
        {
            var pos = transform.position;
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            pos.z = Mathf.Floor(pos.z);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置向上取整
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CeilPosition(this Transform transform)
        {
            var pos = transform.position;
            pos.x = Mathf.Ceil(pos.x);
            pos.y = Mathf.Ceil(pos.y);
            pos.z = Mathf.Ceil(pos.z);
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置对齐到指定网格大小
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="gridSize">网格大小</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform SnapPosition(this Transform transform, float gridSize)
        {
            if (gridSize <= 0f) return transform;
            
            var pos = transform.position;
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
            pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
            transform.position = pos;
            return transform;
        }

        /// <summary>
        /// 将世界空间位置对齐到指定网格大小 (各轴独立)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="gridSize">各轴网格大小</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform SnapPosition(this Transform transform, Vector3 gridSize)
        {
            var pos = transform.position;
            if (gridSize.x > 0f) pos.x = Mathf.Round(pos.x / gridSize.x) * gridSize.x;
            if (gridSize.y > 0f) pos.y = Mathf.Round(pos.y / gridSize.y) * gridSize.y;
            if (gridSize.z > 0f) pos.z = Mathf.Round(pos.z / gridSize.z) * gridSize.z;
            transform.position = pos;
            return transform;
        }

        #endregion

        #region 位置插值

        /// <summary>
        /// 将世界空间位置线性插值到目标位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpPosition(this Transform transform, Vector3 target, float t)
        {
            transform.position = Vector3.Lerp(transform.position, target, t);
            return transform;
        }

        /// <summary>
        /// 将世界空间位置线性插值到目标 Transform 的位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpPosition(this Transform transform, Transform target, float t)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, t);
            return transform;
        }

        /// <summary>
        /// 将世界空间位置平滑移动到目标位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="smoothTime">平滑时间</param>
        /// <param name="velocity">当前速度 (引用参数，需要在调用间保持)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SmoothDampPosition(this Transform transform, Vector3 target, float smoothTime, ref Vector3 velocity)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
            return transform;
        }

        /// <summary>
        /// 将世界空间位置向目标位置移动指定距离
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MoveTowards(this Transform transform, Vector3 target, float maxDistanceDelta)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, maxDistanceDelta);
            return transform;
        }

        /// <summary>
        /// 将世界空间位置向目标 Transform 移动指定距离
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform MoveTowards(this Transform transform, Transform target, float maxDistanceDelta)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, maxDistanceDelta);
            return transform;
        }

        #endregion
    }
}
