// ==========================================================
// 文件名：MathUtility.Interpolation.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Runtime.CompilerServices
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    public static partial class MathUtility
    {
        #region 线性插值

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">结束值</param>
        /// <param name="t">插值因子 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        /// <summary>
        /// 线性插值（不限制 t 范围）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// 反向线性插值，计算 value 在 [a, b] 范围内的位置
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float value)
        {
            if (Approximately(a, b)) return 0f;
            return Clamp01((value - a) / (b - a));
        }

        /// <summary>
        /// 反向线性插值（不限制结果范围）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (Approximately(a, b)) return 0f;
            return (value - a) / (b - a);
        }

        /// <summary>
        /// 角度线性插值（处理角度环绕）
        /// </summary>
        public static float LerpAngle(float a, float b, float t)
        {
            float delta = Repeat(b - a, 360f);
            if (delta > 180f) delta -= 360f;
            return a + delta * Clamp01(t);
        }

        #endregion

        #region 平滑插值

        /// <summary>
        /// 平滑步进插值 (Hermite 插值)
        /// </summary>
        /// <param name="edge0">下边界</param>
        /// <param name="edge1">上边界</param>
        /// <param name="x">输入值</param>
        public static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        /// <summary>
        /// 更平滑的步进插值 (Ken Perlin 改进版)
        /// </summary>
        public static float SmootherStep(float edge0, float edge1, float x)
        {
            float t = Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        /// <summary>
        /// 平滑阻尼插值（适用于相机跟随等场景）
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="currentVelocity">当前速度（引用参数，会被修改）</param>
        /// <param name="smoothTime">平滑时间</param>
        /// <param name="deltaTime">时间增量</param>
        /// <param name="maxSpeed">最大速度</param>
        public static float SmoothDamp(float current, float target, ref float currentVelocity, 
            float smoothTime, float deltaTime, float maxSpeed = float.MaxValue)
        {
            smoothTime = Max(0.0001f, smoothTime);
            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
            
            float change = current - target;
            float originalTo = target;
            
            float maxChange = maxSpeed * smoothTime;
            change = Clamp(change, -maxChange, maxChange);
            target = current - change;
            
            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            
            float output = target + (change + temp) * exp;
            
            if (originalTo - current > 0f == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }
            
            return output;
        }

        #endregion

        #region 缓动函数 (Easing)

        /// <summary>
        /// 二次缓入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInQuad(float t) => t * t;

        /// <summary>
        /// 二次缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutQuad(float t) => t * (2f - t);

        /// <summary>
        /// 二次缓入缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
        }

        /// <summary>
        /// 三次缓入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInCubic(float t) => t * t * t;

        /// <summary>
        /// 三次缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutCubic(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }

        /// <summary>
        /// 三次缓入缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f;
        }

        /// <summary>
        /// 正弦缓入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInSine(float t)
        {
            return 1f - (float)Math.Cos(t * HalfPI);
        }

        /// <summary>
        /// 正弦缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutSine(float t)
        {
            return (float)Math.Sin(t * HalfPI);
        }

        /// <summary>
        /// 正弦缓入缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutSine(float t)
        {
            return 0.5f * (1f - (float)Math.Cos(PI * t));
        }

        /// <summary>
        /// 指数缓入
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInExpo(float t)
        {
            return t == 0f ? 0f : (float)Math.Pow(2f, 10f * (t - 1f));
        }

        /// <summary>
        /// 指数缓出
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutExpo(float t)
        {
            return t == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * t);
        }

        /// <summary>
        /// 弹性缓出
        /// </summary>
        public static float EaseOutElastic(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            float p = 0.3f;
            return (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t - p / 4f) * TwoPI / p) + 1f;
        }

        /// <summary>
        /// 弹跳缓出
        /// </summary>
        public static float EaseOutBounce(float t)
        {
            if (t < 1f / 2.75f)
                return 7.5625f * t * t;
            if (t < 2f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return 7.5625f * t * t + 0.75f;
            }
            if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return 7.5625f * t * t + 0.9375f;
            }
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }

        #endregion

        #region 贝塞尔曲线

        /// <summary>
        /// 二次贝塞尔曲线插值
        /// </summary>
        /// <param name="p0">起点</param>
        /// <param name="p1">控制点</param>
        /// <param name="p2">终点</param>
        /// <param name="t">插值因子 [0, 1]</param>
        public static float QuadraticBezier(float p0, float p1, float p2, float t)
        {
            float u = 1f - t;
            return u * u * p0 + 2f * u * t * p1 + t * t * p2;
        }

        /// <summary>
        /// 三次贝塞尔曲线插值
        /// </summary>
        /// <param name="p0">起点</param>
        /// <param name="p1">控制点1</param>
        /// <param name="p2">控制点2</param>
        /// <param name="p3">终点</param>
        /// <param name="t">插值因子 [0, 1]</param>
        public static float CubicBezier(float p0, float p1, float p2, float p3, float t)
        {
            float u = 1f - t;
            float uu = u * u;
            float uuu = uu * u;
            float tt = t * t;
            float ttt = tt * t;
            return uuu * p0 + 3f * uu * t * p1 + 3f * u * tt * p2 + ttt * p3;
        }

        #endregion

        #region Catmull-Rom 样条

        /// <summary>
        /// Catmull-Rom 样条插值
        /// </summary>
        /// <param name="p0">前一个点</param>
        /// <param name="p1">起点</param>
        /// <param name="p2">终点</param>
        /// <param name="p3">后一个点</param>
        /// <param name="t">插值因子 [0, 1]</param>
        public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
        {
            float tt = t * t;
            float ttt = tt * t;
            return 0.5f * (
                2f * p1 +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * tt +
                (-p0 + 3f * p1 - 3f * p2 + p3) * ttt
            );
        }

        #endregion
    }
}
