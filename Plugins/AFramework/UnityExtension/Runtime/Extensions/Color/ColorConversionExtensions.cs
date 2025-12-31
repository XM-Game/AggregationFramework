// ==========================================================
// 文件名：ColorConversionExtensions.cs
// 命名空间: AFramework.UnityExtension
// 依赖: UnityEngine, System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AFramework.UnityExtension
{
    /// <summary>
    /// 颜色转换扩展方法
    /// <para>提供 RGB、HSV、HSL 等颜色空间之间的转换</para>
    /// </summary>
    public static class ColorConversionExtensions
    {
        #region RGB <-> HSV

        /// <summary>
        /// 转换为 HSV 颜色空间
        /// </summary>
        /// <param name="c">RGB 颜色</param>
        /// <param name="h">色相 [0, 1]</param>
        /// <param name="s">饱和度 [0, 1]</param>
        /// <param name="v">明度 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToHSV(this Color c, out float h, out float s, out float v)
        {
            Color.RGBToHSV(c, out h, out s, out v);
        }

        /// <summary>
        /// 转换为 HSV 元组
        /// </summary>
        /// <returns>(色相, 饱和度, 明度)</returns>
        public static (float h, float s, float v) ToHSV(this Color c)
        {
            Color.RGBToHSV(c, out float h, out float s, out float v);
            return (h, s, v);
        }

        /// <summary>
        /// 从 HSV 创建颜色
        /// </summary>
        /// <param name="h">色相 [0, 1]</param>
        /// <param name="s">饱和度 [0, 1]</param>
        /// <param name="v">明度 [0, 1]</param>
        /// <param name="a">透明度 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHSV(float h, float s, float v, float a = 1f)
        {
            Color c = Color.HSVToRGB(h, s, v);
            c.a = a;
            return c;
        }

        /// <summary>
        /// 从 HSV 创建颜色 (HDR)
        /// </summary>
        /// <param name="h">色相 [0, 1]</param>
        /// <param name="s">饱和度 [0, 1]</param>
        /// <param name="v">明度 [0, 无限]</param>
        /// <param name="hdr">是否启用 HDR</param>
        /// <param name="a">透明度 [0, 1]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromHSVHDR(float h, float s, float v, bool hdr = true, float a = 1f)
        {
            Color c = Color.HSVToRGB(h, s, v, hdr);
            c.a = a;
            return c;
        }

        #endregion

        #region HSV 分量调整

        /// <summary>
        /// 设置色相
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="hue">新的色相值 [0, 1]</param>
        public static Color WithHue(this Color c, float hue)
        {
            Color.RGBToHSV(c, out _, out float s, out float v);
            Color result = Color.HSVToRGB(hue, s, v);
            result.a = c.a;
            return result;
        }

        /// <summary>
        /// 设置饱和度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="saturation">新的饱和度值 [0, 1]</param>
        public static Color WithSaturation(this Color c, float saturation)
        {
            Color.RGBToHSV(c, out float h, out _, out float v);
            Color result = Color.HSVToRGB(h, saturation, v);
            result.a = c.a;
            return result;
        }

        /// <summary>
        /// 设置明度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="value">新的明度值 [0, 1]</param>
        public static Color WithValue(this Color c, float value)
        {
            Color.RGBToHSV(c, out float h, out float s, out _);
            Color result = Color.HSVToRGB(h, s, value);
            result.a = c.a;
            return result;
        }

        /// <summary>
        /// 偏移色相
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="offset">色相偏移量 [-1, 1]</param>
        public static Color ShiftHue(this Color c, float offset)
        {
            Color.RGBToHSV(c, out float h, out float s, out float v);
            h = (h + offset) % 1f;
            if (h < 0f) h += 1f;
            Color result = Color.HSVToRGB(h, s, v);
            result.a = c.a;
            return result;
        }

        /// <summary>
        /// 获取互补色
        /// </summary>
        public static Color GetComplementary(this Color c)
        {
            return c.ShiftHue(0.5f);
        }

        /// <summary>
        /// 获取三等分色
        /// </summary>
        /// <returns>(原色, 三等分色1, 三等分色2)</returns>
        public static (Color c1, Color c2, Color c3) GetTriadic(this Color c)
        {
            return (c, c.ShiftHue(1f / 3f), c.ShiftHue(2f / 3f));
        }

        /// <summary>
        /// 获取类似色
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="angle">角度偏移 (默认 30 度 = 1/12)</param>
        /// <returns>(原色, 类似色1, 类似色2)</returns>
        public static (Color c1, Color c2, Color c3) GetAnalogous(this Color c, float angle = 1f / 12f)
        {
            return (c, c.ShiftHue(-angle), c.ShiftHue(angle));
        }

        #endregion

        #region RGB <-> HSL

        /// <summary>
        /// 转换为 HSL 颜色空间
        /// </summary>
        /// <param name="c">RGB 颜色</param>
        /// <param name="h">色相 [0, 1]</param>
        /// <param name="s">饱和度 [0, 1]</param>
        /// <param name="l">亮度 [0, 1]</param>
        public static void ToHSL(this Color c, out float h, out float s, out float l)
        {
            float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            float min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            float delta = max - min;

            l = (max + min) * 0.5f;

            if (delta < float.Epsilon)
            {
                h = 0f;
                s = 0f;
            }
            else
            {
                s = l > 0.5f ? delta / (2f - max - min) : delta / (max + min);

                if (Mathf.Approximately(max, c.r))
                {
                    h = ((c.g - c.b) / delta + (c.g < c.b ? 6f : 0f)) / 6f;
                }
                else if (Mathf.Approximately(max, c.g))
                {
                    h = ((c.b - c.r) / delta + 2f) / 6f;
                }
                else
                {
                    h = ((c.r - c.g) / delta + 4f) / 6f;
                }
            }
        }

        /// <summary>
        /// 转换为 HSL 元组
        /// </summary>
        /// <returns>(色相, 饱和度, 亮度)</returns>
        public static (float h, float s, float l) ToHSL(this Color c)
        {
            c.ToHSL(out float h, out float s, out float l);
            return (h, s, l);
        }

        /// <summary>
        /// 从 HSL 创建颜色
        /// </summary>
        /// <param name="h">色相 [0, 1]</param>
        /// <param name="s">饱和度 [0, 1]</param>
        /// <param name="l">亮度 [0, 1]</param>
        /// <param name="a">透明度 [0, 1]</param>
        public static Color FromHSL(float h, float s, float l, float a = 1f)
        {
            float r, g, b;

            if (s < float.Epsilon)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
                float p = 2f * l - q;
                r = HueToRGB(p, q, h + 1f / 3f);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1f / 3f);
            }

            return new Color(r, g, b, a);
        }

        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 0.5f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        /// <summary>
        /// 设置 HSL 亮度
        /// </summary>
        /// <param name="c">原始颜色</param>
        /// <param name="lightness">新的亮度值 [0, 1]</param>
        public static Color WithLightness(this Color c, float lightness)
        {
            c.ToHSL(out float h, out float s, out _);
            return FromHSL(h, s, lightness, c.a);
        }

        #endregion

        #region 十六进制转换

        /// <summary>
        /// 从十六进制字符串解析颜色
        /// </summary>
        /// <param name="hex">十六进制字符串 (支持 #RGB, #RGBA, #RRGGBB, #RRGGBBAA 格式)</param>
        /// <param name="color">解析结果</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParseHex(string hex, out Color color)
        {
            color = Color.white;

            if (string.IsNullOrEmpty(hex))
                return false;

            // 移除 # 前缀
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            // 支持简写格式
            if (hex.Length == 3)
            {
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            }
            else if (hex.Length == 4)
            {
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
            }

            return ColorUtility.TryParseHtmlString("#" + hex, out color);
        }

        /// <summary>
        /// 从十六进制字符串解析颜色
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <param name="defaultColor">解析失败时的默认颜色</param>
        public static Color ParseHex(string hex, Color defaultColor = default)
        {
            return TryParseHex(hex, out Color color) ? color : defaultColor;
        }

        #endregion

        #region 温度转换

        /// <summary>
        /// 从色温创建颜色 (开尔文)
        /// </summary>
        /// <param name="kelvin">色温 (1000K - 40000K)</param>
        /// <returns>对应的颜色</returns>
        public static Color FromTemperature(float kelvin)
        {
            kelvin = Mathf.Clamp(kelvin, 1000f, 40000f);
            float temp = kelvin / 100f;

            float r, g, b;

            // 红色分量
            if (temp <= 66f)
            {
                r = 1f;
            }
            else
            {
                r = temp - 60f;
                r = 329.698727446f * Mathf.Pow(r, -0.1332047592f);
                r = Mathf.Clamp01(r / 255f);
            }

            // 绿色分量
            if (temp <= 66f)
            {
                g = temp;
                g = 99.4708025861f * Mathf.Log(g) - 161.1195681661f;
                g = Mathf.Clamp01(g / 255f);
            }
            else
            {
                g = temp - 60f;
                g = 288.1221695283f * Mathf.Pow(g, -0.0755148492f);
                g = Mathf.Clamp01(g / 255f);
            }

            // 蓝色分量
            if (temp >= 66f)
            {
                b = 1f;
            }
            else if (temp <= 19f)
            {
                b = 0f;
            }
            else
            {
                b = temp - 10f;
                b = 138.5177312231f * Mathf.Log(b) - 305.0447927307f;
                b = Mathf.Clamp01(b / 255f);
            }

            return new Color(r, g, b, 1f);
        }

        #endregion

        #region 线性/Gamma 转换

        /// <summary>
        /// 从 Gamma 空间转换到线性空间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToLinear(this Color c)
        {
            return c.linear;
        }

        /// <summary>
        /// 从线性空间转换到 Gamma 空间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToGamma(this Color c)
        {
            return c.gamma;
        }

        #endregion
    }
}
