// ==========================================================
// 文件名：AnimationCurveExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// AnimationCurve 扩展方法
    /// <para>提供 AnimationCurve 的创建、修改和查询扩展</para>
    /// </summary>
    public static class AnimationCurveExtensions
    {
        #region 预设曲线创建

        /// <summary>
        /// 创建线性曲线 (0,0) -> (1,1)
        /// </summary>
        public static AnimationCurve CreateLinear()
        {
            return AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }

        /// <summary>
        /// 创建线性曲线
        /// </summary>
        public static AnimationCurve CreateLinear(float startTime, float startValue, float endTime, float endValue)
        {
            return AnimationCurve.Linear(startTime, startValue, endTime, endValue);
        }

        /// <summary>
        /// 创建缓入曲线 (Ease In)
        /// </summary>
        public static AnimationCurve CreateEaseIn()
        {
            return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }

        /// <summary>
        /// 创建缓出曲线 (Ease Out)
        /// </summary>
        public static AnimationCurve CreateEaseOut()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 2f, 2f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 创建缓入缓出曲线 (Ease In Out)
        /// </summary>
        public static AnimationCurve CreateEaseInOut()
        {
            return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }

        /// <summary>
        /// 创建常量曲线
        /// </summary>
        public static AnimationCurve CreateConstant(float value)
        {
            return AnimationCurve.Constant(0f, 1f, value);
        }

        /// <summary>
        /// 创建弹跳曲线
        /// </summary>
        public static AnimationCurve CreateBounce()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f));
            curve.AddKey(new Keyframe(0.4f, 1f));
            curve.AddKey(new Keyframe(0.6f, 0.7f));
            curve.AddKey(new Keyframe(0.8f, 1f));
            curve.AddKey(new Keyframe(0.9f, 0.9f));
            curve.AddKey(new Keyframe(1f, 1f));
            return curve;
        }

        /// <summary>
        /// 创建弹性曲线
        /// </summary>
        public static AnimationCurve CreateElastic()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f));
            curve.AddKey(new Keyframe(0.3f, 1.2f));
            curve.AddKey(new Keyframe(0.5f, 0.9f));
            curve.AddKey(new Keyframe(0.7f, 1.05f));
            curve.AddKey(new Keyframe(0.85f, 0.98f));
            curve.AddKey(new Keyframe(1f, 1f));
            return curve;
        }

        /// <summary>
        /// 创建正弦波曲线
        /// </summary>
        /// <param name="frequency">频率</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="duration">持续时间</param>
        /// <param name="sampleCount">采样点数</param>
        public static AnimationCurve CreateSineWave(float frequency = 1f, float amplitude = 1f, float duration = 1f, int sampleCount = 20)
        {
            var curve = new AnimationCurve();
            for (int i = 0; i <= sampleCount; i++)
            {
                float t = (float)i / sampleCount * duration;
                float value = Mathf.Sin(t * frequency * Mathf.PI * 2f) * amplitude;
                curve.AddKey(t, value);
            }
            return curve;
        }

        #endregion

        #region 曲线修改

        /// <summary>
        /// 反转曲线 (时间轴)
        /// </summary>
        public static AnimationCurve Reverse(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            float maxTime = keys[keys.Length - 1].time;
            Keyframe[] reversedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                int reverseIndex = keys.Length - 1 - i;
                reversedKeys[i] = new Keyframe(
                    maxTime - keys[reverseIndex].time,
                    keys[reverseIndex].value,
                    -keys[reverseIndex].outTangent,
                    -keys[reverseIndex].inTangent);
            }

            return new AnimationCurve(reversedKeys);
        }

        /// <summary>
        /// 反转曲线 (值轴)
        /// </summary>
        public static AnimationCurve InvertValues(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            Keyframe[] invertedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                invertedKeys[i] = new Keyframe(
                    keys[i].time,
                    1f - keys[i].value,
                    -keys[i].inTangent,
                    -keys[i].outTangent);
            }

            return new AnimationCurve(invertedKeys);
        }

        /// <summary>
        /// 缩放曲线时间
        /// </summary>
        public static AnimationCurve ScaleTime(this AnimationCurve curve, float scale)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            Keyframe[] scaledKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                scaledKeys[i] = new Keyframe(
                    keys[i].time * scale,
                    keys[i].value,
                    keys[i].inTangent / scale,
                    keys[i].outTangent / scale);
            }

            return new AnimationCurve(scaledKeys);
        }

        /// <summary>
        /// 缩放曲线值
        /// </summary>
        public static AnimationCurve ScaleValue(this AnimationCurve curve, float scale)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            Keyframe[] scaledKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                scaledKeys[i] = new Keyframe(
                    keys[i].time,
                    keys[i].value * scale,
                    keys[i].inTangent * scale,
                    keys[i].outTangent * scale);
            }

            return new AnimationCurve(scaledKeys);
        }

        /// <summary>
        /// 偏移曲线值
        /// </summary>
        public static AnimationCurve OffsetValue(this AnimationCurve curve, float offset)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            Keyframe[] offsetKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                offsetKeys[i] = new Keyframe(
                    keys[i].time,
                    keys[i].value + offset,
                    keys[i].inTangent,
                    keys[i].outTangent);
            }

            return new AnimationCurve(offsetKeys);
        }

        /// <summary>
        /// 钳制曲线值
        /// </summary>
        public static AnimationCurve ClampValues(this AnimationCurve curve, float min, float max)
        {
            if (curve == null || curve.length == 0) return new AnimationCurve();

            Keyframe[] keys = curve.keys;
            Keyframe[] clampedKeys = new Keyframe[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                clampedKeys[i] = new Keyframe(
                    keys[i].time,
                    Mathf.Clamp(keys[i].value, min, max),
                    keys[i].inTangent,
                    keys[i].outTangent);
            }

            return new AnimationCurve(clampedKeys);
        }

        #endregion

        #region 曲线查询

        /// <summary>
        /// 获取曲线持续时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetDuration(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return 0f;
            return curve.keys[curve.length - 1].time - curve.keys[0].time;
        }

        /// <summary>
        /// 获取曲线起始时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetStartTime(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return 0f;
            return curve.keys[0].time;
        }

        /// <summary>
        /// 获取曲线结束时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetEndTime(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0) return 0f;
            return curve.keys[curve.length - 1].time;
        }

        /// <summary>
        /// 获取曲线最小值
        /// </summary>
        public static float GetMinValue(this AnimationCurve curve, int sampleCount = 100)
        {
            if (curve == null || curve.length == 0) return 0f;

            float minValue = float.MaxValue;
            float startTime = curve.GetStartTime();
            float duration = curve.GetDuration();

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = startTime + (float)i / sampleCount * duration;
                float value = curve.Evaluate(t);
                if (value < minValue) minValue = value;
            }

            return minValue;
        }

        /// <summary>
        /// 获取曲线最大值
        /// </summary>
        public static float GetMaxValue(this AnimationCurve curve, int sampleCount = 100)
        {
            if (curve == null || curve.length == 0) return 0f;

            float maxValue = float.MinValue;
            float startTime = curve.GetStartTime();
            float duration = curve.GetDuration();

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = startTime + (float)i / sampleCount * duration;
                float value = curve.Evaluate(t);
                if (value > maxValue) maxValue = value;
            }

            return maxValue;
        }

        /// <summary>
        /// 获取曲线平均值
        /// </summary>
        public static float GetAverageValue(this AnimationCurve curve, int sampleCount = 100)
        {
            if (curve == null || curve.length == 0) return 0f;

            float sum = 0f;
            float startTime = curve.GetStartTime();
            float duration = curve.GetDuration();

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = startTime + (float)i / sampleCount * duration;
                sum += curve.Evaluate(t);
            }

            return sum / (sampleCount + 1);
        }

        /// <summary>
        /// 获取归一化时间的值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EvaluateNormalized(this AnimationCurve curve, float normalizedTime)
        {
            if (curve == null || curve.length == 0) return 0f;

            float startTime = curve.GetStartTime();
            float duration = curve.GetDuration();
            return curve.Evaluate(startTime + normalizedTime * duration);
        }

        #endregion

        #region 曲线组合

        /// <summary>
        /// 连接两条曲线
        /// </summary>
        public static AnimationCurve Concatenate(this AnimationCurve first, AnimationCurve second)
        {
            if (first == null || first.length == 0) return second;
            if (second == null || second.length == 0) return first;

            float firstEndTime = first.GetEndTime();
            float secondStartTime = second.GetStartTime();
            float timeOffset = firstEndTime - secondStartTime;

            var result = new AnimationCurve(first.keys);

            foreach (var key in second.keys)
            {
                result.AddKey(new Keyframe(
                    key.time + timeOffset,
                    key.value,
                    key.inTangent,
                    key.outTangent));
            }

            return result;
        }

        /// <summary>
        /// 混合两条曲线
        /// </summary>
        public static AnimationCurve Blend(this AnimationCurve curveA, AnimationCurve curveB, float blend, int sampleCount = 50)
        {
            if (curveA == null) return curveB;
            if (curveB == null) return curveA;

            float startTime = Mathf.Min(curveA.GetStartTime(), curveB.GetStartTime());
            float endTime = Mathf.Max(curveA.GetEndTime(), curveB.GetEndTime());
            float duration = endTime - startTime;

            var result = new AnimationCurve();

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = startTime + (float)i / sampleCount * duration;
                float valueA = curveA.Evaluate(t);
                float valueB = curveB.Evaluate(t);
                float blendedValue = Mathf.Lerp(valueA, valueB, blend);
                result.AddKey(t, blendedValue);
            }

            return result;
        }

        #endregion

        #region 复制

        /// <summary>
        /// 复制曲线
        /// </summary>
        public static AnimationCurve Clone(this AnimationCurve curve)
        {
            if (curve == null) return null;
            return new AnimationCurve(curve.keys)
            {
                preWrapMode = curve.preWrapMode,
                postWrapMode = curve.postWrapMode
            };
        }

        #endregion
    }
}
