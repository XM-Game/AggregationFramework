// ==========================================================
// 文件名：QuaternionMathExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Quaternion 数学扩展方法
    /// <para>提供 Quaternion 的高级数学运算扩展</para>
    /// </summary>
    public static class QuaternionMathExtensions
    {
        #region 数学运算

        /// <summary>
        /// 计算两个四元数的点积
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Quaternion a, Quaternion b)
        {
            return Quaternion.Dot(a, b);
        }

        /// <summary>
        /// 获取四元数的模长
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this Quaternion q)
        {
            return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        }

        /// <summary>
        /// 获取四元数的模长平方
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(this Quaternion q)
        {
            return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
        }

        /// <summary>
        /// 归一化四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Normalize(this Quaternion q)
        {
            float mag = q.Magnitude();
            if (mag < float.Epsilon) return Quaternion.identity;
            return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        /// <summary>
        /// 获取逆四元数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Inverse(this Quaternion q)
        {
            return Quaternion.Inverse(q);
        }

        /// <summary>
        /// 计算两个四元数之间的差值 (from 到 to 的旋转)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Difference(this Quaternion from, Quaternion to)
        {
            return Quaternion.Inverse(from) * to;
        }

        /// <summary>
        /// 计算两个四元数的中点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Midpoint(this Quaternion a, Quaternion b)
        {
            return Quaternion.Slerp(a, b, 0.5f);
        }

        #endregion

        #region 朝向计算

        /// <summary>
        /// 创建朝向目标的旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookAt(Vector3 forward)
        {
            return Quaternion.LookRotation(forward);
        }

        /// <summary>
        /// 创建朝向目标的旋转 (指定上方向)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookAt(Vector3 forward, Vector3 up)
        {
            return Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// 从一个方向旋转到另一个方向
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(from, to);
        }

        /// <summary>
        /// 创建从当前旋转朝向目标点的旋转
        /// </summary>
        public static Quaternion LookAtPoint(this Quaternion q, Vector3 currentPosition, Vector3 targetPoint)
        {
            Vector3 direction = (targetPoint - currentPosition).normalized;
            if (direction.sqrMagnitude < float.Epsilon) return q;
            return Quaternion.LookRotation(direction);
        }

        /// <summary>
        /// 创建从当前旋转朝向目标点的旋转 (仅水平)
        /// </summary>
        public static Quaternion LookAtPointFlat(this Quaternion q, Vector3 currentPosition, Vector3 targetPoint)
        {
            Vector3 direction = targetPoint - currentPosition;
            direction.y = 0f;
            if (direction.sqrMagnitude < float.Epsilon) return q;
            return Quaternion.LookRotation(direction.normalized);
        }

        #endregion

        #region 平滑旋转

        /// <summary>
        /// 平滑阻尼旋转
        /// </summary>
        public static Quaternion SmoothDamp(this Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime)
        {
            float dot = Quaternion.Dot(current, target);
            float sign = dot > 0f ? 1f : -1f;
            target.x *= sign;
            target.y *= sign;
            target.z *= sign;
            target.w *= sign;

            Vector4 result = new Vector4(
                Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime),
                Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime),
                Mathf.SmoothDamp(current.z, target.z, ref velocity.z, smoothTime),
                Mathf.SmoothDamp(current.w, target.w, ref velocity.w, smoothTime)
            ).normalized;

            return new Quaternion(result.x, result.y, result.z, result.w);
        }

        /// <summary>
        /// 指数衰减旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ExpDecay(this Quaternion current, Quaternion target, float decay, float deltaTime)
        {
            return Quaternion.Slerp(current, target, 1f - Mathf.Exp(-decay * deltaTime));
        }

        /// <summary>
        /// 弹簧旋转
        /// </summary>
        public static Quaternion Spring(this Quaternion current, Quaternion target, ref Vector4 velocity, float frequency, float damping, float deltaTime)
        {
            float dot = Quaternion.Dot(current, target);
            float sign = dot > 0f ? 1f : -1f;

            Vector4 currentV = new Vector4(current.x, current.y, current.z, current.w);
            Vector4 targetV = new Vector4(target.x * sign, target.y * sign, target.z * sign, target.w * sign);

            float omega = frequency * 2f * Mathf.PI;
            float zeta = damping;
            float exp = Mathf.Exp(-zeta * omega * deltaTime);

            Vector4 delta = currentV - targetV;
            Vector4 result;

            if (zeta < 1f)
            {
                float omegaD = omega * Mathf.Sqrt(1f - zeta * zeta);
                result = targetV + exp * (delta * Mathf.Cos(omegaD * deltaTime) +
                    (velocity + delta * (zeta * omega)) / omegaD * Mathf.Sin(omegaD * deltaTime));
                velocity = -exp * ((delta * (zeta * omega) + velocity) * Mathf.Cos(omegaD * deltaTime) +
                    (delta * omegaD - (velocity + delta * (zeta * omega)) * (zeta * omega) / omegaD) * Mathf.Sin(omegaD * deltaTime));
            }
            else
            {
                result = targetV + exp * (delta + (velocity + delta * omega) * deltaTime);
                velocity = exp * (velocity - (velocity + delta * omega) * omega * deltaTime);
            }

            result = result.normalized;
            return new Quaternion(result.x, result.y, result.z, result.w);
        }

        #endregion

        #region 角度限制

        /// <summary>
        /// 限制旋转角度
        /// </summary>
        public static Quaternion ClampAngle(this Quaternion q, Quaternion reference, float maxAngle)
        {
            float angle = Quaternion.Angle(reference, q);
            if (angle <= maxAngle) return q;
            return Quaternion.Slerp(reference, q, maxAngle / angle);
        }

        /// <summary>
        /// 限制旋转速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ClampAngularVelocity(this Quaternion from, Quaternion to, float maxDegreesPerSecond, float deltaTime)
        {
            return Quaternion.RotateTowards(from, to, maxDegreesPerSecond * deltaTime);
        }

        #endregion

        #region 随机旋转

        /// <summary>
        /// 生成随机旋转
        /// </summary>
        public static Quaternion Random()
        {
            float u1 = UnityEngine.Random.value;
            float u2 = UnityEngine.Random.value;
            float u3 = UnityEngine.Random.value;

            float sqrt1MinusU1 = Mathf.Sqrt(1f - u1);
            float sqrtU1 = Mathf.Sqrt(u1);
            float twoPiU2 = 2f * Mathf.PI * u2;
            float twoPiU3 = 2f * Mathf.PI * u3;

            return new Quaternion(
                sqrt1MinusU1 * Mathf.Sin(twoPiU2),
                sqrt1MinusU1 * Mathf.Cos(twoPiU2),
                sqrtU1 * Mathf.Sin(twoPiU3),
                sqrtU1 * Mathf.Cos(twoPiU3)
            );
        }

        /// <summary>
        /// 在当前旋转基础上添加随机偏移
        /// </summary>
        public static Quaternion AddRandomOffset(this Quaternion q, float maxAngle)
        {
            Vector3 randomAxis = UnityEngine.Random.onUnitSphere;
            float randomAngle = UnityEngine.Random.Range(-maxAngle, maxAngle);
            return q * Quaternion.AngleAxis(randomAngle, randomAxis);
        }

        /// <summary>
        /// 生成随机 Y 轴旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion RandomYaw()
        {
            return Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        }

        #endregion

        #region 分解

        /// <summary>
        /// 分解为摇摆和扭转 (Swing-Twist 分解)
        /// </summary>
        public static (Quaternion swing, Quaternion twist) DecomposeSwingTwist(this Quaternion q, Vector3 twistAxis)
        {
            Vector3 rotationAxis = new Vector3(q.x, q.y, q.z);
            Vector3 projection = Vector3.Project(rotationAxis, twistAxis);

            Quaternion twistQuat = new Quaternion(projection.x, projection.y, projection.z, q.w);
            twistQuat.Normalize();
            Quaternion swing = q * Quaternion.Inverse(twistQuat);

            return (swing, twistQuat);
        }

        /// <summary>
        /// 提取绕指定轴的旋转分量
        /// </summary>
        public static float ExtractAngleAroundAxis(this Quaternion q, Vector3 axis)
        {
            axis = axis.normalized;
            Vector3 rotationAxis = new Vector3(q.x, q.y, q.z);
            float dot = Vector3.Dot(rotationAxis, axis);
            Vector3 projected = axis * dot;

            Quaternion twist = new Quaternion(projected.x, projected.y, projected.z, q.w);
            twist.Normalize();
            twist.ToAngleAxis(out float angle, out Vector3 _);

            return angle;
        }

        #endregion
    }
}
