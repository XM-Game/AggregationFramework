// ==========================================================
// 文件名：TransformExtensions.Rotation.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Transform 旋转操作扩展方法
    /// </summary>
    public static partial class TransformExtensions
    {
        #region 世界空间欧拉角设置

        /// <summary>
        /// 设置世界空间欧拉角 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetEulerAngleX(this Transform transform, float x)
        {
            var euler = transform.eulerAngles;
            euler.x = x;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置世界空间欧拉角 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetEulerAngleY(this Transform transform, float y)
        {
            var euler = transform.eulerAngles;
            euler.y = y;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置世界空间欧拉角 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="z">Z 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetEulerAngleZ(this Transform transform, float z)
        {
            var euler = transform.eulerAngles;
            euler.z = z;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置世界空间欧拉角
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 角度值</param>
        /// <param name="y">Y 角度值</param>
        /// <param name="z">Z 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetEulerAngles(this Transform transform, float x, float y, float z)
        {
            transform.eulerAngles = new Vector3(x, y, z);
            return transform;
        }

        #endregion

        #region 本地空间欧拉角设置

        /// <summary>
        /// 设置本地空间欧拉角 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalEulerAngleX(this Transform transform, float x)
        {
            var euler = transform.localEulerAngles;
            euler.x = x;
            transform.localEulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置本地空间欧拉角 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="y">Y 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalEulerAngleY(this Transform transform, float y)
        {
            var euler = transform.localEulerAngles;
            euler.y = y;
            transform.localEulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置本地空间欧拉角 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="z">Z 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalEulerAngleZ(this Transform transform, float z)
        {
            var euler = transform.localEulerAngles;
            euler.z = z;
            transform.localEulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 设置本地空间欧拉角
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="x">X 角度值</param>
        /// <param name="y">Y 角度值</param>
        /// <param name="z">Z 角度值</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SetLocalEulerAngles(this Transform transform, float x, float y, float z)
        {
            transform.localEulerAngles = new Vector3(x, y, z);
            return transform;
        }

        #endregion

        #region 旋转增量操作

        /// <summary>
        /// 在世界空间中增加欧拉角 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddEulerAngleX(this Transform transform, float deltaX)
        {
            var euler = transform.eulerAngles;
            euler.x += deltaX;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加欧拉角 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaY">Y 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddEulerAngleY(this Transform transform, float deltaY)
        {
            var euler = transform.eulerAngles;
            euler.y += deltaY;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 在世界空间中增加欧拉角 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaZ">Z 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddEulerAngleZ(this Transform transform, float deltaZ)
        {
            var euler = transform.eulerAngles;
            euler.z += deltaZ;
            transform.eulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加欧拉角 X 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaX">X 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalEulerAngleX(this Transform transform, float deltaX)
        {
            var euler = transform.localEulerAngles;
            euler.x += deltaX;
            transform.localEulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加欧拉角 Y 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaY">Y 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalEulerAngleY(this Transform transform, float deltaY)
        {
            var euler = transform.localEulerAngles;
            euler.y += deltaY;
            transform.localEulerAngles = euler;
            return transform;
        }

        /// <summary>
        /// 在本地空间中增加欧拉角 Z 分量
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="deltaZ">Z 角度增量</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform AddLocalEulerAngleZ(this Transform transform, float deltaZ)
        {
            var euler = transform.localEulerAngles;
            euler.z += deltaZ;
            transform.localEulerAngles = euler;
            return transform;
        }

        #endregion

        #region 朝向操作

        /// <summary>
        /// 使 Transform 朝向目标位置
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="worldUp">世界上方向 (默认为 Vector3.up)</param>
        /// <returns>返���自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LookAtPosition(this Transform transform, Vector3 target, Vector3? worldUp = null)
        {
            transform.LookAt(target, worldUp ?? Vector3.up);
            return transform;
        }

        /// <summary>
        /// 使 Transform 朝向目标 Transform
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="worldUp">世界上方向 (默认为 Vector3.up)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LookAtTransform(this Transform transform, Transform target, Vector3? worldUp = null)
        {
            transform.LookAt(target, worldUp ?? Vector3.up);
            return transform;
        }

        /// <summary>
        /// 使 Transform 在 XZ 平面上朝向目标位置 (忽略 Y 轴)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform LookAtHorizontal(this Transform transform, Vector3 target)
        {
            var direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            return transform;
        }

        /// <summary>
        /// 使 Transform 在 XZ 平面上朝向目标 Transform (忽略 Y 轴)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LookAtHorizontal(this Transform transform, Transform target)
        {
            return transform.LookAtHorizontal(target.position);
        }

        /// <summary>
        /// 使 Transform 朝向与目标相反的方向 (背对目标)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="worldUp">世界上方向 (默认为 Vector3.up)</param>
        /// <returns>返回自身以支持链式调用</returns>
        public static Transform LookAwayFrom(this Transform transform, Vector3 target, Vector3? worldUp = null)
        {
            var direction = transform.position - target;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction, worldUp ?? Vector3.up);
            }
            return transform;
        }

        /// <summary>
        /// 使 Transform 朝向与目标 Transform 相反的方向 (背对目标)
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="worldUp">世界上方向 (默认为 Vector3.up)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LookAwayFrom(this Transform transform, Transform target, Vector3? worldUp = null)
        {
            return transform.LookAwayFrom(target.position, worldUp);
        }

        #endregion

        #region 旋转插值

        /// <summary>
        /// 将旋转线性插值到目标旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标旋转</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpRotation(this Transform transform, Quaternion target, float t)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, t);
            return transform;
        }

        /// <summary>
        /// 将旋转线性插值到目标 Transform 的旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform LerpRotation(this Transform transform, Transform target, float t)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, t);
            return transform;
        }

        /// <summary>
        /// 将旋转球面插值到目标旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标旋转</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SlerpRotation(this Transform transform, Quaternion target, float t)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, t);
            return transform;
        }

        /// <summary>
        /// 将旋转球面插值到目标 Transform 的旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="t">插值参数 (0-1)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform SlerpRotation(this Transform transform, Transform target, float t)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, t);
            return transform;
        }

        /// <summary>
        /// 将旋转向目标旋转旋转指定角度
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标旋转</param>
        /// <param name="maxDegreesDelta">最大旋转角度</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateTowards(this Transform transform, Quaternion target, float maxDegreesDelta)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxDegreesDelta);
            return transform;
        }

        /// <summary>
        /// 将旋转向目标 Transform 的旋转旋转指定角度
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="maxDegreesDelta">最大旋转角度</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateTowards(this Transform transform, Transform target, float maxDegreesDelta)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, maxDegreesDelta);
            return transform;
        }

        #endregion

        #region 角度计算

        /// <summary>
        /// 计算到目标位置的水平角度 (Y 轴旋转)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标位置</param>
        /// <returns>水平角度 (度)</returns>
        public static float HorizontalAngleTo(this Transform transform, Vector3 target)
        {
            var direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) return 0f;
            return Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        }

        /// <summary>
        /// 计算到目标 Transform 的水平角度 (Y 轴旋转)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <returns>水平角度 (度)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HorizontalAngleTo(this Transform transform, Transform target)
        {
            return transform.HorizontalAngleTo(target.position);
        }

        /// <summary>
        /// 计算到目标位置的垂直角度 (俯仰角)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标位置</param>
        /// <returns>垂直角度 (度)，正值为向上，负值为向下</returns>
        public static float VerticalAngleTo(this Transform transform, Vector3 target)
        {
            var direction = target - transform.position;
            var horizontalDistance = new Vector3(direction.x, 0f, direction.z).magnitude;
            return Mathf.Atan2(direction.y, horizontalDistance) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 计算到目标 Transform 的垂直角度 (俯仰角)
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <returns>垂直角度 (度)，正值为向上，负值为向下</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float VerticalAngleTo(this Transform transform, Transform target)
        {
            return transform.VerticalAngleTo(target.position);
        }

        /// <summary>
        /// 检查目标是否在前方视野内
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标位置</param>
        /// <param name="fieldOfView">视野角度 (度)</param>
        /// <returns>如果在视野内返回 true</returns>
        public static bool IsInFieldOfView(this Transform transform, Vector3 target, float fieldOfView)
        {
            var direction = (target - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, direction);
            return angle <= fieldOfView * 0.5f;
        }

        /// <summary>
        /// 检查目标 Transform 是否在前方视野内
        /// </summary>
        /// <param name="transform">当前 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="fieldOfView">视野角度 (度)</param>
        /// <returns>如果在视野内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInFieldOfView(this Transform transform, Transform target, float fieldOfView)
        {
            return transform.IsInFieldOfView(target.position, fieldOfView);
        }

        #endregion

        #region 轴向旋转

        /// <summary>
        /// 绕世界 X 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundWorldX(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.right, angle, Space.World);
            return transform;
        }

        /// <summary>
        /// 绕世界 Y 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundWorldY(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.up, angle, Space.World);
            return transform;
        }

        /// <summary>
        /// 绕世界 Z 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundWorldZ(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.forward, angle, Space.World);
            return transform;
        }

        /// <summary>
        /// 绕本地 X 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundLocalX(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.right, angle, Space.Self);
            return transform;
        }

        /// <summary>
        /// 绕本地 Y 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundLocalY(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.up, angle, Space.Self);
            return transform;
        }

        /// <summary>
        /// 绕本地 Z 轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundLocalZ(this Transform transform, float angle)
        {
            transform.Rotate(Vector3.forward, angle, Space.Self);
            return transform;
        }

        /// <summary>
        /// 绕指定点和轴旋转
        /// </summary>
        /// <param name="transform">目标 Transform</param>
        /// <param name="point">旋转中心点</param>
        /// <param name="axis">旋转轴</param>
        /// <param name="angle">旋转角度 (度)</param>
        /// <returns>返回自身以支持链式调用</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform RotateAroundPoint(this Transform transform, Vector3 point, Vector3 axis, float angle)
        {
            transform.RotateAround(point, axis, angle);
            return transform;
        }

        #endregion
    }
}
