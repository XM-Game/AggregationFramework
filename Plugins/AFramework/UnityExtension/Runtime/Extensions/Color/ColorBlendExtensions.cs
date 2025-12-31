// ==========================================================
// 文件名：ColorBlendExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 颜色混合模式枚举
    /// </summary>
    public enum ColorBlendMode
    {
        /// <summary>正常混合</summary>
        Normal,
        /// <summary>正片叠底</summary>
        Multiply,
        /// <summary>滤色</summary>
        Screen,
        /// <summary>叠加</summary>
        Overlay,
        /// <summary>变暗</summary>
        Darken,
        /// <summary>变亮</summary>
        Lighten,
        /// <summary>颜色减淡</summary>
        ColorDodge,
        /// <summary>颜色加深</summary>
        ColorBurn,
        /// <summary>强光</summary>
        HardLight,
        /// <summary>柔光</summary>
        SoftLight,
        /// <summary>差值</summary>
        Difference,
        /// <summary>排除</summary>
        Exclusion,
        /// <summary>相加</summary>
        Add,
        /// <summary>相减</summary>
        Subtract
    }

    /// <summary>
    /// 颜色混合扩展方法
    /// <para>提供各种颜色混合模式的实现</para>
    /// </summary>
    public static class ColorBlendExtensions
    {
        #region 基础混合

        /// <summary>
        /// 使用指定混合模式混合两个颜色
        /// </summary>
        /// <param name="baseColor">基础颜色</param>
        /// <param name="blendColor">混合颜色</param>
        /// <param name="mode">混合模式</param>
        /// <param name="opacity">混合不透明度 [0, 1]</param>
        public static Color Blend(this Color baseColor, Color blendColor, ColorBlendMode mode, float opacity = 1f)
        {
            Color result = mode switch
            {
                ColorBlendMode.Normal => blendColor,
                ColorBlendMode.Multiply => baseColor.BlendMultiply(blendColor),
                ColorBlendMode.Screen => baseColor.BlendScreen(blendColor),
                ColorBlendMode.Overlay => baseColor.BlendOverlay(blendColor),
                ColorBlendMode.Darken => baseColor.BlendDarken(blendColor),
                ColorBlendMode.Lighten => baseColor.BlendLighten(blendColor),
                ColorBlendMode.ColorDodge => baseColor.BlendColorDodge(blendColor),
                ColorBlendMode.ColorBurn => baseColor.BlendColorBurn(blendColor),
                ColorBlendMode.HardLight => baseColor.BlendHardLight(blendColor),
                ColorBlendMode.SoftLight => baseColor.BlendSoftLight(blendColor),
                ColorBlendMode.Difference => baseColor.BlendDifference(blendColor),
                ColorBlendMode.Exclusion => baseColor.BlendExclusion(blendColor),
                ColorBlendMode.Add => baseColor.BlendAdd(blendColor),
                ColorBlendMode.Subtract => baseColor.BlendSubtract(blendColor),
                _ => blendColor
            };

            // 应用不透明度
            if (opacity < 1f)
            {
                result = Color.Lerp(baseColor, result, opacity);
            }

            result.a = baseColor.a;
            return result;
        }

        #endregion

        #region 混合模式实现

        /// <summary>
        /// 正片叠底混合
        /// <para>结果 = 基色 × 混合色</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendMultiply(this Color baseColor, Color blendColor)
        {
            return new Color(
                baseColor.r * blendColor.r,
                baseColor.g * blendColor.g,
                baseColor.b * blendColor.b,
                baseColor.a);
        }

        /// <summary>
        /// 滤色混合
        /// <para>结果 = 1 - (1 - 基色) × (1 - 混合色)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendScreen(this Color baseColor, Color blendColor)
        {
            return new Color(
                1f - (1f - baseColor.r) * (1f - blendColor.r),
                1f - (1f - baseColor.g) * (1f - blendColor.g),
                1f - (1f - baseColor.b) * (1f - blendColor.b),
                baseColor.a);
        }

        /// <summary>
        /// 叠加混合
        /// <para>基色 &lt; 0.5: 2 × 基色 × 混合色</para>
        /// <para>基色 ≥ 0.5: 1 - 2 × (1 - 基色) × (1 - 混合色)</para>
        /// </summary>
        public static Color BlendOverlay(this Color baseColor, Color blendColor)
        {
            return new Color(
                OverlayChannel(baseColor.r, blendColor.r),
                OverlayChannel(baseColor.g, blendColor.g),
                OverlayChannel(baseColor.b, blendColor.b),
                baseColor.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float OverlayChannel(float baseVal, float blendVal)
        {
            return baseVal < 0.5f
                ? 2f * baseVal * blendVal
                : 1f - 2f * (1f - baseVal) * (1f - blendVal);
        }

        /// <summary>
        /// 变暗混合
        /// <para>结果 = min(基色, 混合色)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendDarken(this Color baseColor, Color blendColor)
        {
            return new Color(
                Mathf.Min(baseColor.r, blendColor.r),
                Mathf.Min(baseColor.g, blendColor.g),
                Mathf.Min(baseColor.b, blendColor.b),
                baseColor.a);
        }

        /// <summary>
        /// 变亮混合
        /// <para>结果 = max(基色, 混合色)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendLighten(this Color baseColor, Color blendColor)
        {
            return new Color(
                Mathf.Max(baseColor.r, blendColor.r),
                Mathf.Max(baseColor.g, blendColor.g),
                Mathf.Max(baseColor.b, blendColor.b),
                baseColor.a);
        }

        /// <summary>
        /// 颜色减淡混合
        /// <para>结果 = 基色 / (1 - 混合色)</para>
        /// </summary>
        public static Color BlendColorDodge(this Color baseColor, Color blendColor)
        {
            return new Color(
                ColorDodgeChannel(baseColor.r, blendColor.r),
                ColorDodgeChannel(baseColor.g, blendColor.g),
                ColorDodgeChannel(baseColor.b, blendColor.b),
                baseColor.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ColorDodgeChannel(float baseVal, float blendVal)
        {
            if (blendVal >= 1f) return 1f;
            return Mathf.Clamp01(baseVal / (1f - blendVal));
        }

        /// <summary>
        /// 颜色加深混合
        /// <para>结果 = 1 - (1 - 基色) / 混合色</para>
        /// </summary>
        public static Color BlendColorBurn(this Color baseColor, Color blendColor)
        {
            return new Color(
                ColorBurnChannel(baseColor.r, blendColor.r),
                ColorBurnChannel(baseColor.g, blendColor.g),
                ColorBurnChannel(baseColor.b, blendColor.b),
                baseColor.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ColorBurnChannel(float baseVal, float blendVal)
        {
            if (blendVal <= 0f) return 0f;
            return Mathf.Clamp01(1f - (1f - baseVal) / blendVal);
        }

        /// <summary>
        /// 强光混合
        /// <para>混合色 &lt; 0.5: 2 × 基色 × 混合色</para>
        /// <para>混合色 ≥ 0.5: 1 - 2 × (1 - 基色) × (1 - 混合色)</para>
        /// </summary>
        public static Color BlendHardLight(this Color baseColor, Color blendColor)
        {
            return new Color(
                HardLightChannel(baseColor.r, blendColor.r),
                HardLightChannel(baseColor.g, blendColor.g),
                HardLightChannel(baseColor.b, blendColor.b),
                baseColor.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float HardLightChannel(float baseVal, float blendVal)
        {
            return blendVal < 0.5f
                ? 2f * baseVal * blendVal
                : 1f - 2f * (1f - baseVal) * (1f - blendVal);
        }

        /// <summary>
        /// 柔光混合
        /// </summary>
        public static Color BlendSoftLight(this Color baseColor, Color blendColor)
        {
            return new Color(
                SoftLightChannel(baseColor.r, blendColor.r),
                SoftLightChannel(baseColor.g, blendColor.g),
                SoftLightChannel(baseColor.b, blendColor.b),
                baseColor.a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SoftLightChannel(float baseVal, float blendVal)
        {
            if (blendVal < 0.5f)
            {
                return baseVal - (1f - 2f * blendVal) * baseVal * (1f - baseVal);
            }
            else
            {
                float d = baseVal <= 0.25f
                    ? ((16f * baseVal - 12f) * baseVal + 4f) * baseVal
                    : Mathf.Sqrt(baseVal);
                return baseVal + (2f * blendVal - 1f) * (d - baseVal);
            }
        }

        /// <summary>
        /// 差值混合
        /// <para>结果 = |基色 - 混合色|</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendDifference(this Color baseColor, Color blendColor)
        {
            return new Color(
                Mathf.Abs(baseColor.r - blendColor.r),
                Mathf.Abs(baseColor.g - blendColor.g),
                Mathf.Abs(baseColor.b - blendColor.b),
                baseColor.a);
        }

        /// <summary>
        /// 排除混合
        /// <para>结果 = 基色 + 混合色 - 2 × 基色 × 混合色</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendExclusion(this Color baseColor, Color blendColor)
        {
            return new Color(
                baseColor.r + blendColor.r - 2f * baseColor.r * blendColor.r,
                baseColor.g + blendColor.g - 2f * baseColor.g * blendColor.g,
                baseColor.b + blendColor.b - 2f * baseColor.b * blendColor.b,
                baseColor.a);
        }

        /// <summary>
        /// 相加混合
        /// <para>结果 = min(基色 + 混合色, 1)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendAdd(this Color baseColor, Color blendColor)
        {
            return new Color(
                Mathf.Min(baseColor.r + blendColor.r, 1f),
                Mathf.Min(baseColor.g + blendColor.g, 1f),
                Mathf.Min(baseColor.b + blendColor.b, 1f),
                baseColor.a);
        }

        /// <summary>
        /// 相减混合
        /// <para>结果 = max(基色 - 混合色, 0)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color BlendSubtract(this Color baseColor, Color blendColor)
        {
            return new Color(
                Mathf.Max(baseColor.r - blendColor.r, 0f),
                Mathf.Max(baseColor.g - blendColor.g, 0f),
                Mathf.Max(baseColor.b - blendColor.b, 0f),
                baseColor.a);
        }

        #endregion

        #region Alpha 混合

        /// <summary>
        /// Alpha 混合 (Porter-Duff Over 操作)
        /// </summary>
        /// <param name="background">背景颜色</param>
        /// <param name="foreground">前景颜色</param>
        public static Color AlphaBlend(this Color background, Color foreground)
        {
            float outA = foreground.a + background.a * (1f - foreground.a);

            if (outA < float.Epsilon)
                return Color.clear;

            float invOutA = 1f / outA;
            return new Color(
                (foreground.r * foreground.a + background.r * background.a * (1f - foreground.a)) * invOutA,
                (foreground.g * foreground.a + background.g * background.a * (1f - foreground.a)) * invOutA,
                (foreground.b * foreground.a + background.b * background.a * (1f - foreground.a)) * invOutA,
                outA);
        }

        /// <summary>
        /// 预乘 Alpha
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color PremultiplyAlpha(this Color c)
        {
            return new Color(c.r * c.a, c.g * c.a, c.b * c.a, c.a);
        }

        /// <summary>
        /// 取消预乘 Alpha
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color UnpremultiplyAlpha(this Color c)
        {
            if (c.a < float.Epsilon)
                return Color.clear;

            float invA = 1f / c.a;
            return new Color(c.r * invA, c.g * invA, c.b * invA, c.a);
        }

        #endregion

        #region 渐变和调色板

        /// <summary>
        /// 在颜色数组中进行插值
        /// </summary>
        /// <param name="colors">颜色数组</param>
        /// <param name="t">插值参数 [0, 1]</param>
        public static Color LerpArray(Color[] colors, float t)
        {
            if (colors == null || colors.Length == 0)
                return Color.white;

            if (colors.Length == 1)
                return colors[0];

            t = Mathf.Clamp01(t);
            float scaledT = t * (colors.Length - 1);
            int index = Mathf.FloorToInt(scaledT);
            float localT = scaledT - index;

            if (index >= colors.Length - 1)
                return colors[colors.Length - 1];

            return Color.Lerp(colors[index], colors[index + 1], localT);
        }

        /// <summary>
        /// 混合多个颜色 (平均值)
        /// </summary>
        public static Color Average(params Color[] colors)
        {
            if (colors == null || colors.Length == 0)
                return Color.white;

            float r = 0f, g = 0f, b = 0f, a = 0f;
            foreach (var c in colors)
            {
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }

            float inv = 1f / colors.Length;
            return new Color(r * inv, g * inv, b * inv, a * inv);
        }

        /// <summary>
        /// 加权混合多个颜色
        /// </summary>
        /// <param name="colors">颜色数组</param>
        /// <param name="weights">权重数组</param>
        public static Color WeightedAverage(Color[] colors, float[] weights)
        {
            if (colors == null || colors.Length == 0)
                return Color.white;

            if (weights == null || weights.Length != colors.Length)
                return Average(colors);

            float r = 0f, g = 0f, b = 0f, a = 0f;
            float totalWeight = 0f;

            for (int i = 0; i < colors.Length; i++)
            {
                float w = weights[i];
                r += colors[i].r * w;
                g += colors[i].g * w;
                b += colors[i].b * w;
                a += colors[i].a * w;
                totalWeight += w;
            }

            if (totalWeight < float.Epsilon)
                return Color.white;

            float inv = 1f / totalWeight;
            return new Color(r * inv, g * inv, b * inv, a * inv);
        }

        #endregion
    }
}
