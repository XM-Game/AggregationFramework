// ==========================================================
// 文件名：ColorHSV.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// HSV 颜色结构
    /// <para>使用色相(H)、饱和度(S)、明度(V)表示颜色</para>
    /// <para>比 RGB 更直观地进行颜色调整</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 从 RGB 创建
    /// var hsv = ColorHSV.FromColor(Color.red);
    /// 
    /// // 调整色相
    /// hsv.H = 0.5f; // 变为青色
    /// 
    /// // 转回 RGB
    /// Color rgb = hsv.ToColor();
    /// </code>
    /// </remarks>
    [Serializable]
    public struct ColorHSV : IEquatable<ColorHSV>
    {
        #region 字段

        /// <summary>色相 (0-1，对应 0-360 度)</summary>
        [Range(0f, 1f)]
        public float H;

        /// <summary>饱和度 (0-1)</summary>
        [Range(0f, 1f)]
        public float S;

        /// <summary>明度 (0-1)</summary>
        [Range(0f, 1f)]
        public float V;

        /// <summary>透明度 (0-1)</summary>
        [Range(0f, 1f)]
        public float A;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 HSV 颜色
        /// </summary>
        /// <param name="h">色相 (0-1)</param>
        /// <param name="s">饱和度 (0-1)</param>
        /// <param name="v">明度 (0-1)</param>
        /// <param name="a">透明度 (0-1)</param>
        public ColorHSV(float h, float s, float v, float a = 1f)
        {
            H = Mathf.Clamp01(h);
            S = Mathf.Clamp01(s);
            V = Mathf.Clamp01(v);
            A = Mathf.Clamp01(a);
        }

        #endregion
    


        #region 静态属性

        /// <summary>红色</summary>
        public static ColorHSV Red => new ColorHSV(0f, 1f, 1f);

        /// <summary>绿色</summary>
        public static ColorHSV Green => new ColorHSV(0.333f, 1f, 1f);

        /// <summary>蓝色</summary>
        public static ColorHSV Blue => new ColorHSV(0.667f, 1f, 1f);

        /// <summary>黄色</summary>
        public static ColorHSV Yellow => new ColorHSV(0.167f, 1f, 1f);

        /// <summary>青色</summary>
        public static ColorHSV Cyan => new ColorHSV(0.5f, 1f, 1f);

        /// <summary>品红色</summary>
        public static ColorHSV Magenta => new ColorHSV(0.833f, 1f, 1f);

        /// <summary>白色</summary>
        public static ColorHSV White => new ColorHSV(0f, 0f, 1f);

        /// <summary>黑色</summary>
        public static ColorHSV Black => new ColorHSV(0f, 0f, 0f);

        /// <summary>灰色</summary>
        public static ColorHSV Gray => new ColorHSV(0f, 0f, 0.5f);

        /// <summary>透明</summary>
        public static ColorHSV Clear => new ColorHSV(0f, 0f, 0f, 0f);

        #endregion

        #region 属性

        /// <summary>色相角度 (0-360)</summary>
        public float HueDegrees
        {
            get => H * 360f;
            set => H = Mathf.Repeat(value / 360f, 1f);
        }

        /// <summary>是否为灰度色 (饱和度为0)</summary>
        public bool IsGrayscale => S < 0.001f;

        /// <summary>是否为纯色 (饱和度为1)</summary>
        public bool IsPure => S > 0.999f;

        /// <summary>是否透明</summary>
        public bool IsTransparent => A < 0.001f;

        /// <summary>是否完全不透明</summary>
        public bool IsOpaque => A > 0.999f;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 从 Unity Color 创建
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorHSV FromColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return new ColorHSV(h, s, v, color.a);
        }

        /// <summary>
        /// 从 Color32 创建
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ColorHSV FromColor32(Color32 color)
        {
            return FromColor(color);
        }

        /// <summary>
        /// 从色相角度创建
        /// </summary>
        public static ColorHSV FromHueDegrees(float degrees, float s = 1f, float v = 1f, float a = 1f)
        {
            return new ColorHSV(degrees / 360f, s, v, a);
        }

        #endregion

        #region 转换方法

        /// <summary>
        /// 转换为 Unity Color
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ToColor()
        {
            Color color = Color.HSVToRGB(H, S, V);
            color.a = A;
            return color;
        }

        /// <summary>
        /// 转换为 Color32
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color32 ToColor32()
        {
            return ToColor();
        }

        /// <summary>
        /// 隐式转换为 Color
        /// </summary>
        public static implicit operator Color(ColorHSV hsv) => hsv.ToColor();

        /// <summary>
        /// 隐式转换为 ColorHSV
        /// </summary>
        public static implicit operator ColorHSV(Color color) => FromColor(color);

        #endregion

        #region 颜色调整方法

        /// <summary>
        /// 调整色相
        /// </summary>
        /// <param name="offset">色相偏移量 (0-1)</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV ShiftHue(float offset)
        {
            return new ColorHSV(Mathf.Repeat(H + offset, 1f), S, V, A);
        }

        /// <summary>
        /// 调整饱和度
        /// </summary>
        /// <param name="multiplier">饱和度乘数</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV AdjustSaturation(float multiplier)
        {
            return new ColorHSV(H, Mathf.Clamp01(S * multiplier), V, A);
        }

        /// <summary>
        /// 调整明度
        /// </summary>
        /// <param name="multiplier">明度乘数</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV AdjustValue(float multiplier)
        {
            return new ColorHSV(H, S, Mathf.Clamp01(V * multiplier), A);
        }

        /// <summary>
        /// 调整透明度
        /// </summary>
        /// <param name="alpha">新透明度</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV WithAlpha(float alpha)
        {
            return new ColorHSV(H, S, V, Mathf.Clamp01(alpha));
        }

        /// <summary>
        /// 变亮
        /// </summary>
        /// <param name="amount">变亮量 (0-1)</param>
        /// <returns>变亮后的颜色</returns>
        public ColorHSV Lighten(float amount)
        {
            return new ColorHSV(H, S, Mathf.Clamp01(V + amount), A);
        }

        /// <summary>
        /// 变暗
        /// </summary>
        /// <param name="amount">变暗量 (0-1)</param>
        /// <returns>变暗后的颜色</returns>
        public ColorHSV Darken(float amount)
        {
            return new ColorHSV(H, S, Mathf.Clamp01(V - amount), A);
        }

        /// <summary>
        /// 增加饱和度
        /// </summary>
        /// <param name="amount">增加量 (0-1)</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV Saturate(float amount)
        {
            return new ColorHSV(H, Mathf.Clamp01(S + amount), V, A);
        }

        /// <summary>
        /// 降低饱和度
        /// </summary>
        /// <param name="amount">降低量 (0-1)</param>
        /// <returns>调整后的颜色</returns>
        public ColorHSV Desaturate(float amount)
        {
            return new ColorHSV(H, Mathf.Clamp01(S - amount), V, A);
        }

        /// <summary>
        /// 获取互补色
        /// </summary>
        /// <returns>互补色</returns>
        public ColorHSV GetComplementary()
        {
            return ShiftHue(0.5f);
        }

        /// <summary>
        /// 获取三等分色
        /// </summary>
        /// <returns>三等分色数组</returns>
        public ColorHSV[] GetTriadic()
        {
            return new[]
            {
                this,
                ShiftHue(0.333f),
                ShiftHue(0.667f)
            };
        }

        /// <summary>
        /// 获取类似色
        /// </summary>
        /// <param name="angle">角度偏移 (0-1，默认30度/360)</param>
        /// <returns>类似色数组</returns>
        public ColorHSV[] GetAnalogous(float angle = 0.083f)
        {
            return new[]
            {
                ShiftHue(-angle),
                this,
                ShiftHue(angle)
            };
        }

        /// <summary>
        /// 转换为灰度
        /// </summary>
        /// <returns>灰度颜色</returns>
        public ColorHSV ToGrayscale()
        {
            return new ColorHSV(H, 0f, V, A);
        }

        #endregion

        #region 插值方法

        /// <summary>
        /// 线性插值
        /// </summary>
        public static ColorHSV Lerp(ColorHSV a, ColorHSV b, float t)
        {
            // 处理色相环绕
            float h = LerpHue(a.H, b.H, t);
            return new ColorHSV(
                h,
                Mathf.Lerp(a.S, b.S, t),
                Mathf.Lerp(a.V, b.V, t),
                Mathf.Lerp(a.A, b.A, t)
            );
        }

        /// <summary>
        /// 色相插值 (考虑环绕)
        /// </summary>
        private static float LerpHue(float a, float b, float t)
        {
            float diff = b - a;
            if (Mathf.Abs(diff) > 0.5f)
            {
                if (diff > 0)
                    a += 1f;
                else
                    b += 1f;
            }
            return Mathf.Repeat(Mathf.Lerp(a, b, t), 1f);
        }

        #endregion

        #region IEquatable 实现

        public bool Equals(ColorHSV other)
        {
            return Mathf.Approximately(H, other.H) &&
                   Mathf.Approximately(S, other.S) &&
                   Mathf.Approximately(V, other.V) &&
                   Mathf.Approximately(A, other.A);
        }

        public override bool Equals(object obj) => obj is ColorHSV other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(H, S, V, A);

        public static bool operator ==(ColorHSV left, ColorHSV right) => left.Equals(right);
        public static bool operator !=(ColorHSV left, ColorHSV right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        public override string ToString() => $"ColorHSV(H:{H:F2}, S:{S:F2}, V:{V:F2}, A:{A:F2})";

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        public string ToHexString(bool includeAlpha = false)
        {
            Color color = ToColor();
            if (includeAlpha)
                return ColorUtility.ToHtmlStringRGBA(color);
            return ColorUtility.ToHtmlStringRGB(color);
        }

        #endregion
    }
}
