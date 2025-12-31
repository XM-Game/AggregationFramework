// ==========================================================
// 文件名：AudioClipExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AudioClip 扩展方法
    /// <para>提供 AudioClip 的查询和数据操作扩展</para>
    /// </summary>
    public static class AudioClipExtensions
    {
        #region 属性查询

        /// <summary>
        /// 获取音频长度 (秒)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLength(this AudioClip clip)
        {
            return clip != null ? clip.length : 0f;
        }

        /// <summary>
        /// 获取采样数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSamples(this AudioClip clip)
        {
            return clip != null ? clip.samples : 0;
        }

        /// <summary>
        /// 获取采样率
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFrequency(this AudioClip clip)
        {
            return clip != null ? clip.frequency : 0;
        }

        /// <summary>
        /// 获取声道数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetChannels(this AudioClip clip)
        {
            return clip != null ? clip.channels : 0;
        }

        /// <summary>
        /// 检查是否为单声道
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMono(this AudioClip clip)
        {
            return clip != null && clip.channels == 1;
        }

        /// <summary>
        /// 检查是否为立体声
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStereo(this AudioClip clip)
        {
            return clip != null && clip.channels == 2;
        }

        /// <summary>
        /// 检查是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this AudioClip clip)
        {
            return clip != null && clip.length > 0f && clip.samples > 0;
        }

        /// <summary>
        /// 获取加载状态
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AudioDataLoadState GetLoadState(this AudioClip clip)
        {
            return clip != null ? clip.loadState : AudioDataLoadState.Unloaded;
        }

        /// <summary>
        /// 检查是否已加载
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLoaded(this AudioClip clip)
        {
            return clip != null && clip.loadState == AudioDataLoadState.Loaded;
        }

        /// <summary>
        /// 获取加载类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AudioClipLoadType GetLoadType(this AudioClip clip)
        {
            return clip != null ? clip.loadType : AudioClipLoadType.DecompressOnLoad;
        }

        /// <summary>
        /// 检查是否为流式加载
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStreaming(this AudioClip clip)
        {
            return clip != null && clip.loadType == AudioClipLoadType.Streaming;
        }

        #endregion

        #region 时间转换

        /// <summary>
        /// 将时间转换为采样位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TimeToSample(this AudioClip clip, float time)
        {
            if (clip == null || clip.frequency <= 0) return 0;
            return Mathf.RoundToInt(time * clip.frequency);
        }

        /// <summary>
        /// 将采样位置转换为时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SampleToTime(this AudioClip clip, int sample)
        {
            if (clip == null || clip.frequency <= 0) return 0f;
            return (float)sample / clip.frequency;
        }

        /// <summary>
        /// 将归一化时间转换为采样位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NormalizedTimeToSample(this AudioClip clip, float normalizedTime)
        {
            if (clip == null) return 0;
            return Mathf.RoundToInt(normalizedTime * clip.samples);
        }

        /// <summary>
        /// 将采样位置转换为归一化时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SampleToNormalizedTime(this AudioClip clip, int sample)
        {
            if (clip == null || clip.samples <= 0) return 0f;
            return (float)sample / clip.samples;
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 获取音频数据
        /// </summary>
        public static float[] GetData(this AudioClip clip)
        {
            if (clip == null || clip.samples <= 0) return Array.Empty<float>();

            float[] data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);
            return data;
        }

        /// <summary>
        /// 获取指定范围的音频数据
        /// </summary>
        public static float[] GetData(this AudioClip clip, int offsetSamples, int sampleCount)
        {
            if (clip == null || clip.samples <= 0) return Array.Empty<float>();

            int actualCount = Mathf.Min(sampleCount, clip.samples - offsetSamples);
            if (actualCount <= 0) return Array.Empty<float>();

            float[] data = new float[actualCount * clip.channels];
            clip.GetData(data, offsetSamples);
            return data;
        }

        /// <summary>
        /// 获取峰值
        /// </summary>
        public static float GetPeakValue(this AudioClip clip)
        {
            if (clip == null || clip.samples <= 0) return 0f;

            float[] data = clip.GetData();
            float peak = 0f;

            for (int i = 0; i < data.Length; i++)
            {
                float abs = Mathf.Abs(data[i]);
                if (abs > peak) peak = abs;
            }

            return peak;
        }

        /// <summary>
        /// 获取 RMS (均方根) 值
        /// </summary>
        public static float GetRMSValue(this AudioClip clip)
        {
            if (clip == null || clip.samples <= 0) return 0f;

            float[] data = clip.GetData();
            float sum = 0f;

            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i] * data[i];
            }

            return Mathf.Sqrt(sum / data.Length);
        }

        /// <summary>
        /// 获取分贝值
        /// </summary>
        public static float GetDecibelValue(this AudioClip clip)
        {
            float rms = clip.GetRMSValue();
            if (rms <= 0f) return -80f;
            return 20f * Mathf.Log10(rms);
        }

        #endregion

        #region 波形分析

        /// <summary>
        /// 获取波形数据 (用于可视化)
        /// </summary>
        /// <param name="clip">音频片段</param>
        /// <param name="resolution">分辨率 (采样点数)</param>
        public static float[] GetWaveform(this AudioClip clip, int resolution = 100)
        {
            if (clip == null || clip.samples <= 0 || resolution <= 0)
                return Array.Empty<float>();

            float[] data = clip.GetData();
            float[] waveform = new float[resolution];
            int samplesPerPoint = data.Length / resolution;

            for (int i = 0; i < resolution; i++)
            {
                int startSample = i * samplesPerPoint;
                int endSample = Mathf.Min(startSample + samplesPerPoint, data.Length);

                float maxValue = 0f;
                for (int j = startSample; j < endSample; j++)
                {
                    float abs = Mathf.Abs(data[j]);
                    if (abs > maxValue) maxValue = abs;
                }

                waveform[i] = maxValue;
            }

            return waveform;
        }

        /// <summary>
        /// 获取立体声波形数据 (左右声道分离)
        /// </summary>
        public static (float[] left, float[] right) GetStereoWaveform(this AudioClip clip, int resolution = 100)
        {
            if (clip == null || clip.samples <= 0 || resolution <= 0 || clip.channels < 2)
                return (Array.Empty<float>(), Array.Empty<float>());

            float[] data = clip.GetData();
            float[] leftWaveform = new float[resolution];
            float[] rightWaveform = new float[resolution];
            int samplesPerPoint = clip.samples / resolution;

            for (int i = 0; i < resolution; i++)
            {
                int startSample = i * samplesPerPoint;
                int endSample = Mathf.Min(startSample + samplesPerPoint, clip.samples);

                float maxLeft = 0f;
                float maxRight = 0f;

                for (int j = startSample; j < endSample; j++)
                {
                    int dataIndex = j * clip.channels;
                    float absLeft = Mathf.Abs(data[dataIndex]);
                    float absRight = Mathf.Abs(data[dataIndex + 1]);

                    if (absLeft > maxLeft) maxLeft = absLeft;
                    if (absRight > maxRight) maxRight = absRight;
                }

                leftWaveform[i] = maxLeft;
                rightWaveform[i] = maxRight;
            }

            return (leftWaveform, rightWaveform);
        }

        #endregion

        #region 加载控制

        /// <summary>
        /// 加载音频数据
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LoadData(this AudioClip clip)
        {
            return clip != null && clip.LoadAudioData();
        }

        /// <summary>
        /// 卸载音频数据
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool UnloadData(this AudioClip clip)
        {
            return clip != null && clip.UnloadAudioData();
        }

        #endregion
    }
}
