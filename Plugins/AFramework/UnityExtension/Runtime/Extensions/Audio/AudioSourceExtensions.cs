// ==========================================================
// 文件名：AudioSourceExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AudioSource 扩展方法
    /// <para>提供 AudioSource 的播放控制和实用功能扩展</para>
    /// </summary>
    public static class AudioSourceExtensions
    {
        #region 播放控制

        /// <summary>
        /// 安全播放音频
        /// </summary>
        public static void PlaySafe(this AudioSource source)
        {
            if (source != null && source.clip != null)
            {
                source.Play();
            }
        }

        /// <summary>
        /// 安全播放指定音频片段
        /// </summary>
        public static void PlaySafe(this AudioSource source, AudioClip clip)
        {
            if (source != null && clip != null)
            {
                source.clip = clip;
                source.Play();
            }
        }

        /// <summary>
        /// 播放一次性音效 (不影响当前播放)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShot(this AudioSource source, AudioClip clip, float volumeScale = 1f)
        {
            if (source != null && clip != null)
            {
                source.PlayOneShot(clip, volumeScale);
            }
        }

        /// <summary>
        /// 延迟播放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayDelayed(this AudioSource source, float delay)
        {
            if (source != null && source.clip != null)
            {
                source.PlayDelayed(delay);
            }
        }

        /// <summary>
        /// 在指定时间播放 (DSP 时间)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayScheduled(this AudioSource source, double time)
        {
            if (source != null && source.clip != null)
            {
                source.PlayScheduled(time);
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PauseSafe(this AudioSource source)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }

        /// <summary>
        /// 恢复播放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnPauseSafe(this AudioSource source)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopSafe(this AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// 切换播放/暂停状态
        /// </summary>
        public static void TogglePlayPause(this AudioSource source)
        {
            if (source == null) return;

            if (source.isPlaying)
            {
                source.Pause();
            }
            else
            {
                source.UnPause();
            }
        }

        /// <summary>
        /// 从头开始播放
        /// </summary>
        public static void Restart(this AudioSource source)
        {
            if (source == null || source.clip == null) return;

            source.Stop();
            source.time = 0f;
            source.Play();
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置音量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVolume(this AudioSource source, float volume)
        {
            if (source != null)
            {
                source.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// 静音
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mute(this AudioSource source)
        {
            if (source != null)
            {
                source.mute = true;
            }
        }

        /// <summary>
        /// 取消静音
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unmute(this AudioSource source)
        {
            if (source != null)
            {
                source.mute = false;
            }
        }

        /// <summary>
        /// 切换静音状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToggleMute(this AudioSource source)
        {
            if (source != null)
            {
                source.mute = !source.mute;
            }
        }

        /// <summary>
        /// 渐变音量 (需要在 Update 中调用)
        /// </summary>
        public static bool FadeVolume(this AudioSource source, float targetVolume, float fadeSpeed)
        {
            if (source == null) return true;

            source.volume = Mathf.MoveTowards(source.volume, targetVolume, fadeSpeed * Time.deltaTime);
            return Mathf.Approximately(source.volume, targetVolume);
        }

        /// <summary>
        /// 淡入
        /// </summary>
        public static void FadeIn(this AudioSource source, float duration, float targetVolume = 1f)
        {
            if (source == null) return;

            source.volume = 0f;
            if (!source.isPlaying)
            {
                source.Play();
            }
            // 注意：实际淡入需要配合协程或 Update
        }

        /// <summary>
        /// 淡出
        /// </summary>
        public static void FadeOut(this AudioSource source, float duration)
        {
            if (source == null) return;
            // 注意：实际淡出需要配合协程或 Update
        }

        #endregion

        #region 音高和速度

        /// <summary>
        /// 设置音高
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPitch(this AudioSource source, float pitch)
        {
            if (source != null)
            {
                source.pitch = pitch;
            }
        }

        /// <summary>
        /// 随机化音高
        /// </summary>
        public static void RandomizePitch(this AudioSource source, float minPitch = 0.9f, float maxPitch = 1.1f)
        {
            if (source != null)
            {
                source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            }
        }

        /// <summary>
        /// 重置音高
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetPitch(this AudioSource source)
        {
            if (source != null)
            {
                source.pitch = 1f;
            }
        }

        #endregion

        #region 时间控制

        /// <summary>
        /// 设置播放时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTime(this AudioSource source, float time)
        {
            if (source != null && source.clip != null)
            {
                source.time = Mathf.Clamp(time, 0f, source.clip.length);
            }
        }

        /// <summary>
        /// 设置归一化播放时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetNormalizedTime(this AudioSource source, float normalizedTime)
        {
            if (source != null && source.clip != null)
            {
                source.time = Mathf.Clamp01(normalizedTime) * source.clip.length;
            }
        }

        /// <summary>
        /// 获取归一化播放时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetNormalizedTime(this AudioSource source)
        {
            if (source == null || source.clip == null || source.clip.length <= 0f)
                return 0f;

            return source.time / source.clip.length;
        }

        /// <summary>
        /// 获取剩余播放时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRemainingTime(this AudioSource source)
        {
            if (source == null || source.clip == null)
                return 0f;

            return source.clip.length - source.time;
        }

        /// <summary>
        /// 跳转到指定采样位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSample(this AudioSource source, int sample)
        {
            if (source != null && source.clip != null)
            {
                source.timeSamples = Mathf.Clamp(sample, 0, source.clip.samples - 1);
            }
        }

        #endregion

        #region 状态查询

        /// <summary>
        /// 检查是否正在播放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlayingSafe(this AudioSource source)
        {
            return source != null && source.isPlaying;
        }

        /// <summary>
        /// 检查是否有音频片段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasClip(this AudioSource source)
        {
            return source != null && source.clip != null;
        }

        /// <summary>
        /// 检查是否静音
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMuted(this AudioSource source)
        {
            return source != null && source.mute;
        }

        /// <summary>
        /// 检查是否循环
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLooping(this AudioSource source)
        {
            return source != null && source.loop;
        }

        /// <summary>
        /// 检查播放是否完成 (非循环)
        /// </summary>
        public static bool IsFinished(this AudioSource source)
        {
            if (source == null || source.clip == null)
                return true;

            if (source.loop)
                return false;

            return !source.isPlaying && source.time >= source.clip.length - 0.01f;
        }

        #endregion

        #region 3D 音频

        /// <summary>
        /// 设置为 2D 音频
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set2D(this AudioSource source)
        {
            if (source != null)
            {
                source.spatialBlend = 0f;
            }
        }

        /// <summary>
        /// 设置为 3D 音频
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set3D(this AudioSource source)
        {
            if (source != null)
            {
                source.spatialBlend = 1f;
            }
        }

        /// <summary>
        /// 设置空间混合
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSpatialBlend(this AudioSource source, float blend)
        {
            if (source != null)
            {
                source.spatialBlend = Mathf.Clamp01(blend);
            }
        }

        /// <summary>
        /// 设置最小/最大距离
        /// </summary>
        public static void SetDistance(this AudioSource source, float minDistance, float maxDistance)
        {
            if (source != null)
            {
                source.minDistance = minDistance;
                source.maxDistance = maxDistance;
            }
        }

        /// <summary>
        /// 设置多普勒效果级别
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDopplerLevel(this AudioSource source, float level)
        {
            if (source != null)
            {
                source.dopplerLevel = Mathf.Clamp01(level);
            }
        }

        #endregion

        #region 配置

        /// <summary>
        /// 配置为背景音乐
        /// </summary>
        public static void ConfigureAsBGM(this AudioSource source)
        {
            if (source == null) return;

            source.loop = true;
            source.spatialBlend = 0f;
            source.playOnAwake = false;
            source.priority = 0;
        }

        /// <summary>
        /// 配置为音效
        /// </summary>
        public static void ConfigureAsSFX(this AudioSource source, bool is3D = false)
        {
            if (source == null) return;

            source.loop = false;
            source.spatialBlend = is3D ? 1f : 0f;
            source.playOnAwake = false;
            source.priority = 128;
        }

        /// <summary>
        /// 配置为环境音
        /// </summary>
        public static void ConfigureAsAmbient(this AudioSource source)
        {
            if (source == null) return;

            source.loop = true;
            source.spatialBlend = 1f;
            source.playOnAwake = true;
            source.priority = 200;
            source.rolloffMode = AudioRolloffMode.Linear;
        }

        /// <summary>
        /// 复制配置
        /// </summary>
        public static void CopySettingsFrom(this AudioSource target, AudioSource source)
        {
            if (target == null || source == null) return;

            target.volume = source.volume;
            target.pitch = source.pitch;
            target.loop = source.loop;
            target.mute = source.mute;
            target.spatialBlend = source.spatialBlend;
            target.minDistance = source.minDistance;
            target.maxDistance = source.maxDistance;
            target.rolloffMode = source.rolloffMode;
            target.dopplerLevel = source.dopplerLevel;
            target.spread = source.spread;
            target.priority = source.priority;
            target.panStereo = source.panStereo;
            target.reverbZoneMix = source.reverbZoneMix;
        }

        #endregion
    }
}
