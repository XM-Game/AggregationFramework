// ==========================================================
// 文件名：Color32Extensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// Color32 扩展方法
    /// <para>提供 Color32 的分量操作和实用功能扩展</para>
    /// </summary>
    public static class Color32Extensions
    {
        #region 分量操作

        /// <summary>
        /// 设置 R 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 WithR(this Color32 c, byte r) => new Color32(r, c.g, c.b, c.a);

        /// <summary>
        /// 设置 G 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 WithG(this Color32 c, byte g) => new Color32(c.r, g, c.b, c.a);

        /// <summary>
        /// 设置 B 分量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 WithB(this Color32 c, byte b) => new Color32(c.r, c.g, b, c.a);

        /// <summary>
        /// 设置 A 分量 (透明度)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 WithA(this Color32 c, byte a) => new Color32(c.r, c.g, c.b, a);

        /// <summary>
        /// 设置 RGB 分量，保持 Alpha 不变
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 WithRGB(this Color32 c, byte r, byte g, byte b) => new Color32(r, g, b, c.a);

        /// <summary>
        /// 增加 R 分量 (带钳制)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 AddR(this Color32 c, int r)
        {
            return new Color32((byte)Mathf.Clamp(c.r + r, 0, 255), c.g, c.b, c.a);
        }

        /// <summary>
        /// 增加 G 分量 (带钳制)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 AddG(this Color32 c, int g)
        {
            return new Color32(c.r, (byte)Mathf.Clamp(c.g + g, 0, 255), c.b, c.a);
        }

        /// <summary>
        /// 增加 B 分量 (带钳制)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 AddB(this Color32 c, int b)
        {
            return new Color32(c.r, c.g, (byte)Mathf.Clamp(c.b + b, 0, 255), c.a);
        }

        /// <summary>
        /// 增加 A 分量 (带钳制)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 AddA(this Color32 c, int a)
        {
            return new Color32(c.r, c.g, c.b, (byte)Mathf.Clamp(c.a + a, 0, 255));
        }

        #endregion

        #region 数学运算

        /// <summary>
        /// 反转颜色 (RGB)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 Invert(this Color32 c)
        {
            return new Color32((byte)(255 - c.r), (byte)(255 - c.g), (byte)(255 - c.b), c.a);
        }

        /// <summary>
        /// 反转颜色 (包含 Alpha)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 InvertWithAlpha(this Color32 c)
        {
            return new Color32((byte)(255 - c.r), (byte)(255 - c.g), (byte)(255 - c.b), (byte)(255 - c.a));
        }

        /// <summary>
        /// 获取灰度值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetGrayscale(this Color32 c)
        {
            // 使用整数运算避免浮点数
            return (byte)((c.r * 77 + c.g * 150 + c.b * 29) >> 8);
        }

        /// <summary>
        /// 转换为灰度颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 ToGrayscale(this Color32 c)
        {
            byte gray = c.GetGrayscale();
            return new Color32(gray, gray, gray, c.a);
        }

        /// <summary>
        /// 获取最大分量值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MaxComponent(this Color32 c) => (byte)Mathf.Max(c.r, Mathf.Max(c.g, c.b));

        /// <summary>
        /// 获取最小分量值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MinComponent(this Color32 c) => (byte)Mathf.Min(c.r, Mathf.Min(c.g, c.b));

        #endregion

        #region 亮度调整

        /// <summary>
        /// 使颜色变亮
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="amount">变亮量 [0, 255]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 Lighten(this Color32 c, int amount)
        {
            return new Color32(
                (byte)Mathf.Min(c.r + amount, 255),
                (byte)Mathf.Min(c.g + amount, 255),
                (byte)Mathf.Min(c.b + amount, 255),
                c.a);
        }

        /// <summary>
        /// 使颜色变暗
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="amount">变暗量 [0, 255]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 Darken(this Color32 c, int amount)
        {
            return new Color32(
                (byte)Mathf.Max(c.r - amount, 0),
                (byte)Mathf.Max(c.g - amount, 0),
                (byte)Mathf.Max(c.b - amount, 0),
                c.a);
        }

        #endregion

        #region 插值

        /// <summary>
        /// 线性插值到目标颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 LerpTo(this Color32 from, Color32 to, float t)
        {
            return Color32.Lerp(from, to, t);
        }

        /// <summary>
        /// 无限制线性插值到目标颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 LerpUnclampedTo(this Color32 from, Color32 to, float t)
        {
            return Color32.LerpUnclamped(from, to, t);
        }

        #endregion

        #region 检查和比较

        /// <summary>
        /// 检查是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        /// <summary>
        /// 检查是否近似相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsApproximately(this Color32 a, Color32 b, int tolerance = 1)
        {
            return Mathf.Abs(a.r - b.r) <= tolerance &&
                   Mathf.Abs(a.g - b.g) <= tolerance &&
                   Mathf.Abs(a.b - b.b) <= tolerance &&
                   Mathf.Abs(a.a - b.a) <= tolerance;
        }

        /// <summary>
        /// 检查是否为透明
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTransparent(this Color32 c) => c.a == 0;

        /// <summary>
        /// 检查是否为不透明
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpaque(this Color32 c) => c.a == 255;

        /// <summary>
        /// 检查是否为黑色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBlack(this Color32 c, int tolerance = 1)
        {
            return c.r <= tolerance && c.g <= tolerance && c.b <= tolerance;
        }

        /// <summary>
        /// 检查是否为白色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhite(this Color32 c, int tolerance = 1)
        {
            return c.r >= 255 - tolerance && c.g >= 255 - tolerance && c.b >= 255 - tolerance;
        }

        #endregion

        #region 转换

        /// <summary>
        /// 转换为 Color
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this Color32 c) => c;

        /// <summary>
        /// 转换为十六进制字符串 (不含 Alpha)
        /// </summary>
        public static string ToHexRGB(this Color32 c)
        {
            return $"{c.r:X2}{c.g:X2}{c.b:X2}";
        }

        /// <summary>
        /// 转换为十六进制字符串 (含 Alpha)
        /// </summary>
        public static string ToHexRGBA(this Color32 c)
        {
            return $"{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
        }

        /// <summary>
        /// 转换为 32 位整数 (RGBA)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(this Color32 c)
        {
            return (c.r << 24) | (c.g << 16) | (c.b << 8) | c.a;
        }

        /// <summary>
        /// 转换为 32 位无符号整数 (RGBA)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(this Color32 c)
        {
            return ((uint)c.r << 24) | ((uint)c.g << 16) | ((uint)c.b << 8) | c.a;
        }

        /// <summary>
        /// 从 32 位整数创建 Color32
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 FromInt32(int value)
        {
            return new Color32(
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF));
        }

        #endregion
    }
}
