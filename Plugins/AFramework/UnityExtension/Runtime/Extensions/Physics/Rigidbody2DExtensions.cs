// ==========================================================
// 文件名：Rigidbody2DExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Rigidbody2D 扩展方法
    /// <para>提供 Rigidbody2D 的物理操作和实用功能扩展</para>
    /// </summary>
    public static class Rigidbody2DExtensions
    {
        #region 速度操作

        /// <summary>
        /// 设置速度 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVelocityX(this Rigidbody2D rb, float x)
        {
            rb.velocity = new Vector2(x, rb.velocity.y);
        }

        /// <summary>
        /// 设置速度 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVelocityY(this Rigidbody2D rb, float y)
        {
            rb.velocity = new Vector2(rb.velocity.x, y);
        }

        /// <summary>
        /// 增加速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddVelocity(this Rigidbody2D rb, Vector2 velocity)
        {
            rb.velocity += velocity;
        }

        /// <summary>
        /// 增加速度 X 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddVelocityX(this Rigidbody2D rb, float x)
        {
            rb.velocity = new Vector2(rb.velocity.x + x, rb.velocity.y);
        }

        /// <summary>
        /// 增加速度 Y 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddVelocityY(this Rigidbody2D rb, float y)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + y);
        }

        /// <summary>
        /// 钳制速度大小
        /// </summary>
        public static void ClampVelocity(this Rigidbody2D rb, float maxSpeed)
        {
            if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        /// <summary>
        /// 钳制水平速度
        /// </summary>
        public static void ClampVelocityX(this Rigidbody2D rb, float maxSpeed)
        {
            float clampedX = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
            rb.velocity = new Vector2(clampedX, rb.velocity.y);
        }

        /// <summary>
        /// 钳制垂直速度
        /// </summary>
        public static void ClampVelocityY(this Rigidbody2D rb, float maxSpeed)
        {
            float clampedY = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);
            rb.velocity = new Vector2(rb.velocity.x, clampedY);
        }

        /// <summary>
        /// 停止所有运动
        /// </summary>
        public static void Stop(this Rigidbody2D rb)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        /// <summary>
        /// 停止水平运动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopHorizontal(this Rigidbody2D rb)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        /// <summary>
        /// 停止垂直运动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopVertical(this Rigidbody2D rb)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        /// <summary>
        /// 反转水平速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FlipVelocityX(this Rigidbody2D rb)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }

        /// <summary>
        /// 反转垂直速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FlipVelocityY(this Rigidbody2D rb)
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }

        #endregion

        #region 力操作

        /// <summary>
        /// 施加冲量使物体达到目标速度
        /// </summary>
        public static void SetVelocityWithImpulse(this Rigidbody2D rb, Vector2 targetVelocity)
        {
            Vector2 velocityChange = targetVelocity - rb.velocity;
            rb.AddForce(velocityChange * rb.mass, ForceMode2D.Impulse);
        }

        /// <summary>
        /// 施加力使物体朝向目标点移动
        /// </summary>
        public static void AddForceTowards(this Rigidbody2D rb, Vector2 targetPosition, float force, ForceMode2D mode = ForceMode2D.Force)
        {
            Vector2 direction = ((Vector2)rb.transform.position - targetPosition).normalized;
            rb.AddForce(-direction * force, mode);
        }

        /// <summary>
        /// 施加跳跃力
        /// </summary>
        public static void Jump(this Rigidbody2D rb, float jumpForce, bool resetVerticalVelocity = true)
        {
            if (resetVerticalVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        /// <summary>
        /// 施加阻力
        /// </summary>
        public static void ApplyDrag(this Rigidbody2D rb, float dragCoefficient)
        {
            rb.AddForce(-rb.velocity * dragCoefficient, ForceMode2D.Force);
        }

        #endregion

        #region 位置和旋转

        /// <summary>
        /// 移动到目标位置 (物理安全)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToPosition(this Rigidbody2D rb, Vector2 position)
        {
            rb.MovePosition(position);
        }

        /// <summary>
        /// 旋转到目标角度 (物理安全)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToRotation(this Rigidbody2D rb, float angle)
        {
            rb.MoveRotation(angle);
        }

        /// <summary>
        /// 平滑移动到目标位置
        /// </summary>
        public static void SmoothMoveToPosition(this Rigidbody2D rb, Vector2 targetPosition, float smoothTime)
        {
            Vector2 direction = targetPosition - rb.position;
            rb.velocity = direction / smoothTime;
        }

        /// <summary>
        /// 朝向目标点
        /// </summary>
        public static void LookAt(this Rigidbody2D rb, Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - rb.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.MoveRotation(angle);
        }

        /// <summary>
        /// 平滑朝向目标点
        /// </summary>
        public static void SmoothLookAt(this Rigidbody2D rb, Vector2 targetPosition, float rotationSpeed)
        {
            Vector2 direction = targetPosition - rb.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        #endregion

        #region 状态检查

        /// <summary>
        /// 检查是否静止
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStationary(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.sqrMagnitude < threshold * threshold &&
                   Mathf.Abs(rb.angularVelocity) < threshold;
        }

        /// <summary>
        /// 检查是否在移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMoving(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.sqrMagnitude > threshold * threshold;
        }

        /// <summary>
        /// 检查是否在旋转
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRotating(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return Mathf.Abs(rb.angularVelocity) > threshold;
        }

        /// <summary>
        /// 检查是否向右移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMovingRight(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.x > threshold;
        }

        /// <summary>
        /// 检查是否向左移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMovingLeft(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.x < -threshold;
        }

        /// <summary>
        /// 检查是否向上移动
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMovingUp(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.y > threshold;
        }

        /// <summary>
        /// 检查是否向下移动 (下落)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMovingDown(this Rigidbody2D rb, float threshold = 0.01f)
        {
            return rb.velocity.y < -threshold;
        }

        /// <summary>
        /// 获取动能
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetKineticEnergy(this Rigidbody2D rb)
        {
            return 0.5f * rb.mass * rb.velocity.sqrMagnitude;
        }

        /// <summary>
        /// 获取动量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetMomentum(this Rigidbody2D rb)
        {
            return rb.mass * rb.velocity;
        }

        #endregion

        #region 约束操作

        /// <summary>
        /// 冻结位置
        /// </summary>
        public static void FreezePosition(this Rigidbody2D rb)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        }

        /// <summary>
        /// 冻结旋转
        /// </summary>
        public static void FreezeRotation(this Rigidbody2D rb)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        }

        /// <summary>
        /// 冻结所有
        /// </summary>
        public static void FreezeAll(this Rigidbody2D rb)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        /// <summary>
        /// 解除所有约束
        /// </summary>
        public static void UnfreezeAll(this Rigidbody2D rb)
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }

        /// <summary>
        /// 设置为运动学模式
        /// </summary>
        public static void SetKinematic(this Rigidbody2D rb, bool isKinematic)
        {
            rb.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            if (isKinematic)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        /// <summary>
        /// 设置为静态模式
        /// </summary>
        public static void SetStatic(this Rigidbody2D rb)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        /// <summary>
        /// 设置为动态模式
        /// </summary>
        public static void SetDynamic(this Rigidbody2D rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        #endregion
    }
}
