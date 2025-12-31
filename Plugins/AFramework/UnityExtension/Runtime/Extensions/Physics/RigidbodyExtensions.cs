// ==========================================================
// 文件名：RigidbodyExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Rigidbody 扩展方法
    /// <para>提供 Rigidbody 的物理操作和实用功能扩展</para>
    /// </summary>
    public static class RigidbodyExtensions
    {
        #region 速度操作

        /// <summary>
        /// 设置速度 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVelocityX(this Rigidbody rb, float x)
        {
            rb.velocity = new Vector3(x, rb.velocity.y, rb.velocity.z);
        }

        /// <summary>
        /// 设置速度 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVelocityY(this Rigidbody rb, float y)
        {
            rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
        }

        /// <summary>
        /// 设置速度 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVelocityZ(this Rigidbody rb, float z)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, z);
        }

        /// <summary>
        /// 设置水平速度 (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHorizontalVelocity(this Rigidbody rb, Vector2 velocity)
        {
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
        }

        /// <summary>
        /// 设置水平速度 (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHorizontalVelocity(this Rigidbody rb, float x, float z)
        {
            rb.velocity = new Vector3(x, rb.velocity.y, z);
        }

        /// <summary>
        /// 获取水平速度 (XZ 平面)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetHorizontalVelocity(this Rigidbody rb)
        {
            return new Vector2(rb.velocity.x, rb.velocity.z);
        }

        /// <summary>
        /// 获取水平速度大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetHorizontalSpeed(this Rigidbody rb)
        {
            Vector3 v = rb.velocity;
            return Mathf.Sqrt(v.x * v.x + v.z * v.z);
        }

        /// <summary>
        /// 增加速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddVelocity(this Rigidbody rb, Vector3 velocity)
        {
            rb.velocity += velocity;
        }

        /// <summary>
        /// 钳制速度大小
        /// </summary>
        public static void ClampVelocity(this Rigidbody rb, float maxSpeed)
        {
            if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        /// <summary>
        /// 钳制水平速度大小
        /// </summary>
        public static void ClampHorizontalVelocity(this Rigidbody rb, float maxSpeed)
        {
            Vector3 v = rb.velocity;
            float horizontalSqr = v.x * v.x + v.z * v.z;

            if (horizontalSqr > maxSpeed * maxSpeed)
            {
                float scale = maxSpeed / Mathf.Sqrt(horizontalSqr);
                rb.velocity = new Vector3(v.x * scale, v.y, v.z * scale);
            }
        }

        /// <summary>
        /// 停止所有运动
        /// </summary>
        public static void Stop(this Rigidbody rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// 停止水平运动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopHorizontal(this Rigidbody rb)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        /// <summary>
        /// 停止垂直运动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopVertical(this Rigidbody rb)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }

        #endregion

        #region 角速度操作

        /// <summary>
        /// 设置角速度 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAngularVelocityX(this Rigidbody rb, float x)
        {
            rb.angularVelocity = new Vector3(x, rb.angularVelocity.y, rb.angularVelocity.z);
        }

        /// <summary>
        /// 设置角速度 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAngularVelocityY(this Rigidbody rb, float y)
        {
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, y, rb.angularVelocity.z);
        }

        /// <summary>
        /// 设置角速度 Z 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAngularVelocityZ(this Rigidbody rb, float z)
        {
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y, z);
        }

        /// <summary>
        /// 停止旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopRotation(this Rigidbody rb)
        {
            rb.angularVelocity = Vector3.zero;
        }

        #endregion

        #region 力操作

        /// <summary>
        /// 施加冲量使物体达到目标速度
        /// </summary>
        public static void SetVelocityWithImpulse(this Rigidbody rb, Vector3 targetVelocity)
        {
            Vector3 velocityChange = targetVelocity - rb.velocity;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        /// <summary>
        /// 施加力使物体朝向目标点移动
        /// </summary>
        public static void AddForceTowards(this Rigidbody rb, Vector3 targetPosition, float force, ForceMode mode = ForceMode.Force)
        {
            Vector3 direction = (targetPosition - rb.position).normalized;
            rb.AddForce(direction * force, mode);
        }

        /// <summary>
        /// 施加爆炸力
        /// </summary>
        public static void AddExplosionForceEx(this Rigidbody rb, float force, Vector3 explosionPosition, float radius, float upwardsModifier = 0f)
        {
            rb.AddExplosionForce(force, explosionPosition, radius, upwardsModifier);
        }

        /// <summary>
        /// 施加阻力
        /// </summary>
        public static void ApplyDrag(this Rigidbody rb, float dragCoefficient)
        {
            rb.AddForce(-rb.velocity * dragCoefficient, ForceMode.Force);
        }

        #endregion

        #region 位置和旋转

        /// <summary>
        /// 移动到目标位置 (物理安全)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToPosition(this Rigidbody rb, Vector3 position)
        {
            rb.MovePosition(position);
        }

        /// <summary>
        /// 旋转到目标旋转 (物理安全)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToRotation(this Rigidbody rb, Quaternion rotation)
        {
            rb.MoveRotation(rotation);
        }

        /// <summary>
        /// 平滑移动到目标位置
        /// </summary>
        public static void SmoothMoveToPosition(this Rigidbody rb, Vector3 targetPosition, float smoothTime)
        {
            Vector3 direction = targetPosition - rb.position;
            rb.velocity = direction / smoothTime;
        }

        /// <summary>
        /// 朝向目标点
        /// </summary>
        public static void LookAt(this Rigidbody rb, Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - rb.position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                rb.MoveRotation(Quaternion.LookRotation(direction));
            }
        }

        /// <summary>
        /// 平滑朝向目标点
        /// </summary>
        public static void SmoothLookAt(this Rigidbody rb, Vector3 targetPosition, float rotationSpeed)
        {
            Vector3 direction = targetPosition - rb.position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            }
        }

        #endregion

        #region 状态检查

        /// <summary>
        /// 检查是否静止
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStationary(this Rigidbody rb, float threshold = 0.01f)
        {
            return rb.velocity.sqrMagnitude < threshold * threshold &&
                   rb.angularVelocity.sqrMagnitude < threshold * threshold;
        }

        /// <summary>
        /// 检查是否在移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoving(this Rigidbody rb, float threshold = 0.01f)
        {
            return rb.velocity.sqrMagnitude > threshold * threshold;
        }

        /// <summary>
        /// 检查是否在旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRotating(this Rigidbody rb, float threshold = 0.01f)
        {
            return rb.angularVelocity.sqrMagnitude > threshold * threshold;
        }

        /// <summary>
        /// 获取动能
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetKineticEnergy(this Rigidbody rb)
        {
            return 0.5f * rb.mass * rb.velocity.sqrMagnitude;
        }

        /// <summary>
        /// 获取动量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetMomentum(this Rigidbody rb)
        {
            return rb.mass * rb.velocity;
        }

        #endregion

        #region 约束操作

        /// <summary>
        /// 冻结位置
        /// </summary>
        public static void FreezePosition(this Rigidbody rb)
        {
            rb.constraints |= RigidbodyConstraints.FreezePosition;
        }

        /// <summary>
        /// 冻结旋转
        /// </summary>
        public static void FreezeRotation(this Rigidbody rb)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotation;
        }

        /// <summary>
        /// 冻结所有
        /// </summary>
        public static void FreezeAll(this Rigidbody rb)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        /// <summary>
        /// 解除所有约束
        /// </summary>
        public static void UnfreezeAll(this Rigidbody rb)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        /// <summary>
        /// 设置为运动学模式
        /// </summary>
        public static void SetKinematic(this Rigidbody rb, bool isKinematic)
        {
            rb.isKinematic = isKinematic;
            if (isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        #endregion
    }
}
