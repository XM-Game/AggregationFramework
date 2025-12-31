// ==========================================================
// 文件名：ColorExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Color 扩展方法
    /// <para>提供 Color 的分量操作、数学运算和实用功能扩展</para>
    /// </summary>
    public static class ColorExtensions
    {
        #region 分量操作

        /// <summary>
        /// 设置 R 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithR(this Color c, float r) => new Color(r, c.g, c.b, c.a);

        /// <summary>
        /// 设置 G 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithG(this Color c, float g) => new Color(c.r, g, c.b, c.a);

        /// <summary>
        /// 设置 B 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithB(this Color c, float b) => new Color(c.r, c.g, b, c.a);

        /// <summary>
        /// 设置 A 分量 (透明度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);

        /// <summary>
        /// 设置 RGB 分量，保持 Alpha 不变
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithRGB(this Color c, float r, float g, float b) => new Color(r, g, b, c.a);

        /// <summary>
        /// 增加 R 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddR(this Color c, float r) => new Color(c.r + r, c.g, c.b, c.a);

        /// <summary>
        /// 增加 G 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddG(this Color c, float g) => new Color(c.r, c.g + g, c.b, c.a);

        /// <summary>
        /// 增加 B 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddB(this Color c, float b) => new Color(c.r, c.g, c.b + b, c.a);

        /// <summary>
        /// 增加 A 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AddA(this Color c, float a) => new Color(c.r, c.g, c.b, c.a + a);

        /// <summary>
        /// 乘以 R 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MultiplyR(this Color c, float r) => new Color(c.r * r, c.g, c.b, c.a);

        /// <summary>
        /// 乘以 G 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MultiplyG(this Color c, float g) => new Color(c.r, c.g * g, c.b, c.a);

        /// <summary>
        /// 乘以 B 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MultiplyB(this Color c, float b) => new Color(c.r, c.g, c.b * b, c.a);

        /// <summary>
        /// 乘以 A 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MultiplyA(this Color c, float a) => new Color(c.r, c.g, c.b, c.a * a);

        /// <summary>
        /// 乘以 RGB 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color MultiplyRGB(this Color c, float factor) => new Color(c.r * factor, c.g * factor, c.b * factor, c.a);

        #endregion

        #region 数学运算

        /// <summary>
        /// 钳制颜色分量到 [0, 1] 范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Clamp01(this Color c)
        {
            return new Color(
                Mathf.Clamp01(c.r),
                Mathf.Clamp01(c.g),
                Mathf.Clamp01(c.b),
                Mathf.Clamp01(c.a));
        }

        /// <summary>
        /// 钳制颜色分量到指定范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Clamp(this Color c, float min, float max)
        {
            return new Color(
                Mathf.Clamp(c.r, min, max),
                Mathf.Clamp(c.g, min, max),
                Mathf.Clamp(c.b, min, max),
                Mathf.Clamp(c.a, min, max));
        }

        /// <summary>
        /// 反转颜色 (RGB)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Invert(this Color c) => new Color(1f - c.r, 1f - c.g, 1f - c.b, c.a);

        /// <summary>
        /// 反转颜色 (包含 Alpha)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color InvertWithAlpha(this Color c) => new Color(1f - c.r, 1f - c.g, 1f - c.b, 1f - c.a);

        /// <summary>
        /// 获取灰度值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetGrayscale(this Color c) => c.grayscale;

        /// <summary>
        /// 转换为灰度颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToGrayscale(this Color c)
        {
            float gray = c.grayscale;
            return new Color(gray, gray, gray, c.a);
        }

        /// <summary>
        /// 获取亮度值 (感知亮度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLuminance(this Color c)
        {
            // 使用 ITU-R BT.709 标准
            return 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
        }

        /// <summary>
        /// 获取最大分量值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxComponent(this Color c) => Mathf.Max(c.r, Mathf.Max(c.g, c.b));

        /// <summary>
        /// 获取最小分量值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinComponent(this Color c) => Mathf.Min(c.r, Mathf.Min(c.g, c.b));

        #endregion

        #region 亮度和饱和度调整

        /// <summary>
        /// 调整亮度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="factor">亮度因子 (1.0 = 不变, &gt;1 = 更亮, &lt;1 = 更暗)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AdjustBrightness(this Color c, float factor)
        {
            return new Color(
                Mathf.Clamp01(c.r * factor),
                Mathf.Clamp01(c.g * factor),
                Mathf.Clamp01(c.b * factor),
                c.a);
        }

        /// <summary>
        /// 调整对比度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="factor">对比度因子 (1.0 = 不变)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AdjustContrast(this Color c, float factor)
        {
            return new Color(
                Mathf.Clamp01((c.r - 0.5f) * factor + 0.5f),
                Mathf.Clamp01((c.g - 0.5f) * factor + 0.5f),
                Mathf.Clamp01((c.b - 0.5f) * factor + 0.5f),
                c.a);
        }

        /// <summary>
        /// 调整饱和度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="factor">饱和度因子 (0 = 灰度, 1 = 不变, &gt;1 = 更饱和)</param>
        public static Color AdjustSaturation(this Color c, float factor)
        {
            float gray = c.grayscale;
            return new Color(
                Mathf.Clamp01(gray + (c.r - gray) * factor),
                Mathf.Clamp01(gray + (c.g - gray) * factor),
                Mathf.Clamp01(gray + (c.b - gray) * factor),
                c.a);
        }

        /// <summary>
        /// 使颜色变亮
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="amount">变亮量 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lighten(this Color c, float amount)
        {
            return new Color(
                Mathf.Clamp01(c.r + amount),
                Mathf.Clamp01(c.g + amount),
                Mathf.Clamp01(c.b + amount),
                c.a);
        }

        /// <summary>
        /// 使颜色变暗
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="amount">变暗量 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Darken(this Color c, float amount)
        {
            return new Color(
                Mathf.Clamp01(c.r - amount),
                Mathf.Clamp01(c.g - amount),
                Mathf.Clamp01(c.b - amount),
                c.a);
        }

        #endregion

        #region 插值

        /// <summary>
        /// 线性插值到目标颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color LerpTo(this Color from, Color to, float t) => Color.Lerp(from, to, t);

        /// <summary>
        /// 无限制线性插值到目标颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color LerpUnclampedTo(this Color from, Color to, float t) => Color.LerpUnclamped(from, to, t);

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Color a, Color b, float tolerance = 0.001f)
        {
            return Mathf.Abs(a.r - b.r) < tolerance &&
                   Mathf.Abs(a.g - b.g) < tolerance &&
                   Mathf.Abs(a.b - b.b) < tolerance &&
                   Mathf.Abs(a.a - b.a) < tolerance;
        }

        /// <summary>
        /// 检查是否为透明
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransparent(this Color c) => c.a <= 0f;

        /// <summary>
        /// 检查是否为不透明
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpaque(this Color c) => c.a >= 1f;

        /// <summary>
        /// 检查是否为黑色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBlack(this Color c, float tolerance = 0.001f)
        {
            return c.r < tolerance && c.g < tolerance && c.b < tolerance;
        }

        /// <summary>
        /// 检查是否为白色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhite(this Color c, float tolerance = 0.001f)
        {
            return c.r > 1f - tolerance && c.g > 1f - tolerance && c.b > 1f - tolerance;
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Color32
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 ToColor32(this Color c) => c;

        /// <summary>
        /// 转换为十六进制字符串 (不含 Alpha)
        /// </summary>
        public static string ToHexRGB(this Color c)
        {
            return ColorUtility.ToHtmlStringRGB(c);
        }

        /// <summary>
        /// 转换为十六进制字符串 (含 Alpha)
        /// </summary>
        public static string ToHexRGBA(this Color c)
        {
            return ColorUtility.ToHtmlStringRGBA(c);
        }

        /// <summary>
        /// 转换为 Vector3 (RGB)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Color c) => new Vector3(c.r, c.g, c.b);

        /// <summary>
        /// 转换为 Vector4 (RGBA)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);

        #endregion
    }
}
