// ==========================================================
// 文件名：AudioMixerExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, UnityEngine.Audio, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AudioMixer 扩展方法
    /// <para>提供 AudioMixer 的参数控制和实用功能扩展</para>
    /// </summary>
    public static class AudioMixerExtensions
    {
        #region 音量控制

        /// <summary>
        /// 设置音量 (分贝)
        /// </summary>
        /// <param name="mixer">混音器</param>
        /// <param name="exposedParameter">暴露的参数名</param>
        /// <param name="volumeDb">音量 (分贝, -80 到 20)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetVolumeDb(this AudioMixer mixer, string exposedParameter, float volumeDb)
        {
            if (mixer == null) return false;
            return mixer.SetFloat(exposedParameter, Mathf.Clamp(volumeDb, -80f, 20f));
        }

        /// <summary>
        /// 设置音量 (线性, 0-1)
        /// </summary>
        /// <param name="mixer">混音器</param>
        /// <param name="exposedParameter">暴露的参数名</param>
        /// <param name="volumeLinear">线性音量 (0 到 1)</param>
        public static bool SetVolumeLinear(this AudioMixer mixer, string exposedParameter, float volumeLinear)
        {
            if (mixer == null) return false;

            // 将线性音量转换为分贝
            float volumeDb = volumeLinear > 0f
                ? 20f * Mathf.Log10(volumeLinear)
                : -80f;

            return mixer.SetFloat(exposedParameter, Mathf.Clamp(volumeDb, -80f, 20f));
        }

        /// <summary>
        /// 获取音量 (分贝)
        /// </summary>
        public static float GetVolumeDb(this AudioMixer mixer, string exposedParameter)
        {
            if (mixer == null) return -80f;

            if (mixer.GetFloat(exposedParameter, out float volumeDb))
            {
                return volumeDb;
            }
            return -80f;
        }

        /// <summary>
        /// 获取音量 (线性, 0-1)
        /// </summary>
        public static float GetVolumeLinear(this AudioMixer mixer, string exposedParameter)
        {
            float volumeDb = mixer.GetVolumeDb(exposedParameter);

            // 将分贝转换为线性音量
            if (volumeDb <= -80f) return 0f;
            return Mathf.Pow(10f, volumeDb / 20f);
        }

        /// <summary>
        /// 静音
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Mute(this AudioMixer mixer, string exposedParameter)
        {
            return mixer.SetVolumeDb(exposedParameter, -80f);
        }

        /// <summary>
        /// 取消静音 (恢复到 0dB)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unmute(this AudioMixer mixer, string exposedParameter)
        {
            return mixer.SetVolumeDb(exposedParameter, 0f);
        }

        #endregion

        #region 参数操作

        /// <summary>
        /// 安全设置浮点参数
        /// </summary>
        public static bool TrySetFloat(this AudioMixer mixer, string exposedParameter, float value)
        {
            if (mixer == null || string.IsNullOrEmpty(exposedParameter)) return false;
            return mixer.SetFloat(exposedParameter, value);
        }

        /// <summary>
        /// 安全获取浮点参数
        /// </summary>
        public static bool TryGetFloat(this AudioMixer mixer, string exposedParameter, out float value)
        {
            value = 0f;
            if (mixer == null || string.IsNullOrEmpty(exposedParameter)) return false;
            return mixer.GetFloat(exposedParameter, out value);
        }

        /// <summary>
        /// 获取浮点参数 (带默认值)
        /// </summary>
        public static float GetFloatOrDefault(this AudioMixer mixer, string exposedParameter, float defaultValue = 0f)
        {
            if (mixer == null) return defaultValue;

            if (mixer.GetFloat(exposedParameter, out float value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 重置参数到默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ClearFloat(this AudioMixer mixer, string exposedParameter)
        {
            if (mixer == null) return false;
            return mixer.ClearFloat(exposedParameter);
        }

        #endregion

        #region 快照操作

        /// <summary>
        /// 查找快照
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AudioMixerSnapshot FindSnapshotSafe(this AudioMixer mixer, string snapshotName)
        {
            if (mixer == null || string.IsNullOrEmpty(snapshotName)) return null;
            return mixer.FindSnapshot(snapshotName);
        }

        /// <summary>
        /// 过渡到快照
        /// </summary>
        public static bool TransitionToSnapshot(this AudioMixer mixer, string snapshotName, float transitionTime)
        {
            if (mixer == null) return false;

            var snapshot = mixer.FindSnapshot(snapshotName);
            if (snapshot == null) return false;

            snapshot.TransitionTo(transitionTime);
            return true;
        }

        /// <summary>
        /// 过渡到多个快照 (加权混合)
        /// </summary>
        public static bool TransitionToSnapshots(this AudioMixer mixer, string[] snapshotNames, float[] weights, float transitionTime)
        {
            if (mixer == null || snapshotNames == null || weights == null) return false;
            if (snapshotNames.Length != weights.Length) return false;

            var snapshots = new AudioMixerSnapshot[snapshotNames.Length];
            for (int i = 0; i < snapshotNames.Length; i++)
            {
                snapshots[i] = mixer.FindSnapshot(snapshotNames[i]);
                if (snapshots[i] == null) return false;
            }

            mixer.TransitionToSnapshots(snapshots, weights, transitionTime);
            return true;
        }

        #endregion

        #region 常用效果参数

        /// <summary>
        /// 设置低通滤波器截止频率
        /// </summary>
        public static bool SetLowpassCutoff(this AudioMixer mixer, string exposedParameter, float frequency)
        {
            return mixer.TrySetFloat(exposedParameter, Mathf.Clamp(frequency, 10f, 22000f));
        }

        /// <summary>
        /// 设置高通滤波器截止频率
        /// </summary>
        public static bool SetHighpassCutoff(this AudioMixer mixer, string exposedParameter, float frequency)
        {
            return mixer.TrySetFloat(exposedParameter, Mathf.Clamp(frequency, 10f, 22000f));
        }

        /// <summary>
        /// 设置混响湿度
        /// </summary>
        public static bool SetReverbWet(this AudioMixer mixer, string exposedParameter, float wetLevel)
        {
            // 混响湿度通常以分贝表示
            float wetDb = wetLevel > 0f ? 20f * Mathf.Log10(wetLevel) : -80f;
            return mixer.TrySetFloat(exposedParameter, Mathf.Clamp(wetDb, -80f, 0f));
        }

        /// <summary>
        /// 设置音高偏移
        /// </summary>
        public static bool SetPitch(this AudioMixer mixer, string exposedParameter, float pitch)
        {
            return mixer.TrySetFloat(exposedParameter, Mathf.Clamp(pitch, 0.5f, 2f));
        }

        #endregion
    }

    /// <summary>
    /// AudioMixerGroup 扩展方法
    /// </summary>
    public static class AudioMixerGroupExtensions
    {
        /// <summary>
        /// 获取所属的 AudioMixer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AudioMixer GetMixer(this AudioMixerGroup group)
        {
            return group?.audioMixer;
        }

        /// <summary>
        /// 检查是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this AudioMixerGroup group)
        {
            return group != null && group.audioMixer != null;
        }
    }

    /// <summary>
    /// AudioMixerSnapshot 扩展方法
    /// </summary>
    public static class AudioMixerSnapshotExtensions
    {
        /// <summary>
        /// 立即切换到此快照
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransitionImmediate(this AudioMixerSnapshot snapshot)
        {
            snapshot?.TransitionTo(0f);
        }

        /// <summary>
        /// 平滑过渡到此快照
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransitionSmooth(this AudioMixerSnapshot snapshot, float duration = 1f)
        {
            snapshot?.TransitionTo(duration);
        }

        /// <summary>
        /// 获取所属的 AudioMixer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AudioMixer GetMixer(this AudioMixerSnapshot snapshot)
        {
            return snapshot?.audioMixer;
        }

        /// <summary>
        /// 检查是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this AudioMixerSnapshot snapshot)
        {
            return snapshot != null && snapshot.audioMixer != null;
        }
    }
}
