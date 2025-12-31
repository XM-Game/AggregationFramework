// ==========================================================
// 文件名：AnimationExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Animation 扩展方法 (Legacy 动画系统)
    /// <para>提供 Animation 组件的实用功能扩展</para>
    /// </summary>
    public static class AnimationExtensions
    {
        #region 播放控制

        /// <summary>
        /// 安全播放动画
        /// </summary>
        public static bool PlaySafe(this Animation animation, string clipName)
        {
            if (animation == null || string.IsNullOrEmpty(clipName)) return false;

            if (animation.GetClip(clipName) != null)
            {
                animation.Play(clipName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全交叉淡入动画
        /// </summary>
        public static bool CrossFadeSafe(this Animation animation, string clipName, float fadeLength = 0.3f)
        {
            if (animation == null || string.IsNullOrEmpty(clipName)) return false;

            if (animation.GetClip(clipName) != null)
            {
                animation.CrossFade(clipName, fadeLength);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全混合动画
        /// </summary>
        public static bool BlendSafe(this Animation animation, string clipName, float targetWeight = 1f, float fadeLength = 0.3f)
        {
            if (animation == null || string.IsNullOrEmpty(clipName)) return false;

            if (animation.GetClip(clipName) != null)
            {
                animation.Blend(clipName, targetWeight, fadeLength);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 播放动画并等待完成
        /// </summary>
        public static void PlayAndWait(this Animation animation, string clipName, Action onComplete)
        {
            if (animation == null || string.IsNullOrEmpty(clipName)) return;

            AnimationClip clip = animation.GetClip(clipName);
            if (clip != null)
            {
                animation.Play(clipName);
                // 注意：实际使用时需要配合协程或 UniTask
            }
        }

        #endregion

        #region 状态查询

        /// <summary>
        /// 检查是否正在播放指定动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlayingClip(this Animation animation, string clipName)
        {
            return animation != null && animation.IsPlaying(clipName);
        }

        /// <summary>
        /// 检查是否正在播放任何动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlayingAny(this Animation animation)
        {
            return animation != null && animation.isPlaying;
        }

        /// <summary>
        /// 检查是否有指定动画片段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasClip(this Animation animation, string clipName)
        {
            return animation != null && animation.GetClip(clipName) != null;
        }

        /// <summary>
        /// 获取当前播放的动画状态
        /// </summary>
        public static AnimationState GetCurrentState(this Animation animation)
        {
            if (animation == null) return null;

            foreach (AnimationState state in animation)
            {
                if (animation.IsPlaying(state.name))
                {
                    return state;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前播放的动画名称
        /// </summary>
        public static string GetCurrentClipName(this Animation animation)
        {
            var state = animation.GetCurrentState();
            return state?.name;
        }

        /// <summary>
        /// 获取当前播放进度 (归一化时间)
        /// </summary>
        public static float GetCurrentNormalizedTime(this Animation animation)
        {
            var state = animation.GetCurrentState();
            return state?.normalizedTime ?? 0f;
        }

        #endregion

        #region 动画片段管理

        /// <summary>
        /// 添加动画片段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddClipSafe(this Animation animation, AnimationClip clip, string clipName)
        {
            if (animation != null && clip != null && !string.IsNullOrEmpty(clipName))
            {
                animation.AddClip(clip, clipName);
            }
        }

        /// <summary>
        /// 移除动画片段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveClipSafe(this Animation animation, string clipName)
        {
            if (animation != null && !string.IsNullOrEmpty(clipName))
            {
                animation.RemoveClip(clipName);
            }
        }

        /// <summary>
        /// 获取所有动画片段名称
        /// </summary>
        public static string[] GetAllClipNames(this Animation animation)
        {
            if (animation == null) return Array.Empty<string>();

            int count = animation.GetClipCount();
            string[] names = new string[count];
            int index = 0;

            foreach (AnimationState state in animation)
            {
                if (index < count)
                {
                    names[index++] = state.name;
                }
            }

            return names;
        }

        #endregion

        #region 动画状态操作

        /// <summary>
        /// 设置动画速度
        /// </summary>
        public static void SetSpeed(this Animation animation, string clipName, float speed)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.speed = speed;
            }
        }

        /// <summary>
        /// 设置动画权重
        /// </summary>
        public static void SetWeight(this Animation animation, string clipName, float weight)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.weight = weight;
            }
        }

        /// <summary>
        /// 设置动画层
        /// </summary>
        public static void SetLayer(this Animation animation, string clipName, int layer)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.layer = layer;
            }
        }

        /// <summary>
        /// 设置动画循环模式
        /// </summary>
        public static void SetWrapMode(this Animation animation, string clipName, WrapMode wrapMode)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.wrapMode = wrapMode;
            }
        }

        /// <summary>
        /// 设置动画时间
        /// </summary>
        public static void SetTime(this Animation animation, string clipName, float time)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.time = time;
            }
        }

        /// <summary>
        /// 设置动画归一化时间
        /// </summary>
        public static void SetNormalizedTime(this Animation animation, string clipName, float normalizedTime)
        {
            if (animation == null) return;

            AnimationState state = animation[clipName];
            if (state != null)
            {
                state.normalizedTime = normalizedTime;
            }
        }

        #endregion

        #region 全局控制

        /// <summary>
        /// 暂停所有动画
        /// </summary>
        public static void PauseAll(this Animation animation)
        {
            if (animation == null) return;

            foreach (AnimationState state in animation)
            {
                state.speed = 0f;
            }
        }

        /// <summary>
        /// 恢复所有动画
        /// </summary>
        public static void ResumeAll(this Animation animation)
        {
            if (animation == null) return;

            foreach (AnimationState state in animation)
            {
                state.speed = 1f;
            }
        }

        /// <summary>
        /// 停止所有动画
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopAll(this Animation animation)
        {
            animation?.Stop();
        }

        /// <summary>
        /// 倒带所有动画
        /// </summary>
        public static void RewindAll(this Animation animation)
        {
            animation?.Rewind();
        }

        #endregion
    }
}
