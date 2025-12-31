// ==========================================================
// 文件名：AnimatorExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Animator 扩展方法
    /// <para>提供 Animator 的参数操作和状态查询扩展</para>
    /// </summary>
    public static class AnimatorExtensions
    {
        #region 参数设置 (安全版本)

        /// <summary>
        /// 安全设置 Bool 参数 (参数不存在时不报错)
        /// </summary>
        public static bool TrySetBool(this Animator animator, string name, bool value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            int hash = Animator.StringToHash(name);
            if (HasParameter(animator, hash, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Bool 参数 (使用哈希)
        /// </summary>
        public static bool TrySetBool(this Animator animator, int hash, bool value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            if (HasParameter(animator, hash, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Float 参数
        /// </summary>
        public static bool TrySetFloat(this Animator animator, string name, float value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            int hash = Animator.StringToHash(name);
            if (HasParameter(animator, hash, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Float 参数 (使用哈希)
        /// </summary>
        public static bool TrySetFloat(this Animator animator, int hash, float value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            if (HasParameter(animator, hash, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Integer 参数
        /// </summary>
        public static bool TrySetInteger(this Animator animator, string name, int value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            int hash = Animator.StringToHash(name);
            if (HasParameter(animator, hash, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Integer 参数 (使用哈希)
        /// </summary>
        public static bool TrySetInteger(this Animator animator, int hash, int value)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            if (HasParameter(animator, hash, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(hash, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Trigger 参数
        /// </summary>
        public static bool TrySetTrigger(this Animator animator, string name)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            int hash = Animator.StringToHash(name);
            if (HasParameter(animator, hash, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(hash);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全设置 Trigger 参数 (使用哈希)
        /// </summary>
        public static bool TrySetTrigger(this Animator animator, int hash)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            if (HasParameter(animator, hash, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(hash);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全重置 Trigger 参数
        /// </summary>
        public static bool TryResetTrigger(this Animator animator, string name)
        {
            if (animator == null || !animator.isActiveAndEnabled) return false;

            int hash = Animator.StringToHash(name);
            if (HasParameter(animator, hash, AnimatorControllerParameterType.Trigger))
            {
                animator.ResetTrigger(hash);
                return true;
            }
            return false;
        }

        #endregion

        #region 参数检查

        /// <summary>
        /// 检查是否有指定参数
        /// </summary>
        public static bool HasParameter(this Animator animator, string name)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return false;

            int hash = Animator.StringToHash(name);
            foreach (var param in animator.parameters)
            {
                if (param.nameHash == hash)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否有指定参数 (使用哈希)
        /// </summary>
        public static bool HasParameter(Animator animator, int hash, AnimatorControllerParameterType type)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return false;

            foreach (var param in animator.parameters)
            {
                if (param.nameHash == hash && param.type == type)
                    return true;
            }
            return false;
        }

        #endregion

        #region 状态查询

        /// <summary>
        /// 检查当前是否在指定状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInState(this Animator animator, string stateName, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        /// <summary>
        /// 检查当前是否在指定状态 (使用哈希)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInState(this Animator animator, int stateHash, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateHash;
        }

        /// <summary>
        /// 检查是否正在过渡
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInTransition(this Animator animator, int layerIndex = 0)
        {
            return animator.IsInTransition(layerIndex);
        }

        /// <summary>
        /// 获取当前状态的归一化时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetCurrentStateNormalizedTime(this Animator animator, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
        }

        /// <summary>
        /// 获取当前状态的长度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetCurrentStateLength(this Animator animator, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).length;
        }

        /// <summary>
        /// 检查当前状态是否播放完成
        /// </summary>
        public static bool IsCurrentStateFinished(this Animator animator, int layerIndex = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return !stateInfo.loop && stateInfo.normalizedTime >= 1f;
        }

        /// <summary>
        /// 获取当前状态的剩余时间
        /// </summary>
        public static float GetCurrentStateRemainingTime(this Animator animator, int layerIndex = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            float normalizedRemaining = 1f - (stateInfo.normalizedTime % 1f);
            return normalizedRemaining * stateInfo.length;
        }

        #endregion

        #region 播放控制

        /// <summary>
        /// 播放指定状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayState(this Animator animator, string stateName, int layerIndex = 0)
        {
            animator.Play(stateName, layerIndex);
        }

        /// <summary>
        /// 播放指定状态 (从指定时间开始)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayState(this Animator animator, string stateName, float normalizedTime, int layerIndex = 0)
        {
            animator.Play(stateName, layerIndex, normalizedTime);
        }

        /// <summary>
        /// 交叉淡入指定状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CrossFadeState(this Animator animator, string stateName, float transitionDuration, int layerIndex = 0)
        {
            animator.CrossFade(stateName, transitionDuration, layerIndex);
        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pause(this Animator animator)
        {
            animator.speed = 0f;
        }

        /// <summary>
        /// 恢复动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resume(this Animator animator)
        {
            animator.speed = 1f;
        }

        /// <summary>
        /// 设置动画速度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSpeed(this Animator animator, float speed)
        {
            animator.speed = speed;
        }

        /// <summary>
        /// 重置所有触发器
        /// </summary>
        public static void ResetAllTriggers(this Animator animator)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return;

            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(param.nameHash);
                }
            }
        }

        #endregion

        #region 层操作

        /// <summary>
        /// 设置层权重
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLayerWeightSafe(this Animator animator, int layerIndex, float weight)
        {
            if (layerIndex >= 0 && layerIndex < animator.layerCount)
            {
                animator.SetLayerWeight(layerIndex, weight);
            }
        }

        /// <summary>
        /// 获取层权重
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLayerWeightSafe(this Animator animator, int layerIndex)
        {
            if (layerIndex >= 0 && layerIndex < animator.layerCount)
            {
                return animator.GetLayerWeight(layerIndex);
            }
            return 0f;
        }

        /// <summary>
        /// 淡入层
        /// </summary>
        public static void FadeInLayer(this Animator animator, int layerIndex, float targetWeight, float duration)
        {
            // 注意：这个方法需要在协程或 Update 中调用
            float currentWeight = animator.GetLayerWeight(layerIndex);
            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime / duration);
            animator.SetLayerWeight(layerIndex, newWeight);
        }

        #endregion

        #region IK 操作

        /// <summary>
        /// 设置 IK 位置权重
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIKPositionWeightSafe(this Animator animator, AvatarIKGoal goal, float weight)
        {
            if (animator.isHuman)
            {
                animator.SetIKPositionWeight(goal, weight);
            }
        }

        /// <summary>
        /// 设置 IK 旋转权重
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIKRotationWeightSafe(this Animator animator, AvatarIKGoal goal, float weight)
        {
            if (animator.isHuman)
            {
                animator.SetIKRotationWeight(goal, weight);
            }
        }

        /// <summary>
        /// 设置 IK 目标
        /// </summary>
        public static void SetIKTarget(this Animator animator, AvatarIKGoal goal, Vector3 position, Quaternion rotation, float positionWeight = 1f, float rotationWeight = 1f)
        {
            if (!animator.isHuman) return;

            animator.SetIKPositionWeight(goal, positionWeight);
            animator.SetIKRotationWeight(goal, rotationWeight);
            animator.SetIKPosition(goal, position);
            animator.SetIKRotation(goal, rotation);
        }

        /// <summary>
        /// 设置注视目标
        /// </summary>
        public static void SetLookAtTarget(this Animator animator, Vector3 position, float weight = 1f)
        {
            if (!animator.isHuman) return;

            animator.SetLookAtWeight(weight);
            animator.SetLookAtPosition(position);
        }

        #endregion
    }
}
