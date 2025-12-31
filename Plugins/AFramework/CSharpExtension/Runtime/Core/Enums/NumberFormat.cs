// ==========================================================
// 文件名：NumberFormat.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Globalization;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 数字格式枚举
    /// <para>定义数字的显示格式</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// string formatted = 1234567.89.Format(NumberFormat.WithThousandsSeparator);
    /// // 结果: "1,234,567.89"
    /// 
    /// string bytes = 1536.FormatBytes(); // "1.5 KB"
    /// </code>
    /// </remarks>
    [Serializable]
    public enum NumberFormat
    {
        /// <summary>默认格式</summary>
        Default = 0,

        /// <summary>整数格式 (无小数)</summary>
        Integer = 1,

        /// <summary>带千位分隔符</summary>
        WithThousandsSeparator = 2,

        /// <summary>固定小数位数 (2位)</summary>
        FixedDecimal2 = 3,

        /// <summary>固定小数位数 (4位)</summary>
        FixedDecimal4 = 4,

        /// <summary>百分比格式</summary>
        Percentage = 5,

        /// <summary>百分比格式 (带小数)</summary>
        PercentageWithDecimal = 6,

        /// <summary>科学计数法</summary>
        Scientific = 7,

        /// <summary>十六进制 (小写)</summary>
        HexLowercase = 8,

        /// <summary>十六进制 (大写)</summary>
        HexUppercase = 9,

        /// <summary>货币格式</summary>
        Currency = 10,

        /// <summary>紧凑格式 (K, M, B)</summary>
        Compact = 11,

        /// <summary>字节格式 (B, KB, MB, GB)</summary>
        Bytes = 12,

        /// <summary>带正负号</summary>
        WithSign = 13
    }

    /// <summary>
    /// NumberFormat 扩展方法
    /// </summary>
    public static class NumberFormatExtensions
    {
        #region 格式化方法

        /// <summary>
        /// 格式化整数
        /// </summary>
        /// <param name="format">数字格式</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(this NumberFormat format, int value)
        {
            return format switch
            {
                NumberFormat.Default => value.ToString(),
                NumberFormat.Integer => value.ToString("D"),
                NumberFormat.WithThousandsSeparator => value.ToString("N0"),
                NumberFormat.FixedDecimal2 => value.ToString("F2"),
                NumberFormat.FixedDecimal4 => value.ToString("F4"),
                NumberFormat.Percentage => (value / 100.0).ToString("P0"),
                NumberFormat.PercentageWithDecimal => (value / 100.0).ToString("P2"),
                NumberFormat.Scientific => value.ToString("E2"),
                NumberFormat.HexLowercase => value.ToString("x"),
                NumberFormat.HexUppercase => value.ToString("X"),
                NumberFormat.Currency => value.ToString("C"),
                NumberFormat.Compact => FormatCompact(value),
                NumberFormat.Bytes => FormatBytes(value),
                NumberFormat.WithSign => FormatWithSign(value),
                _ => value.ToString()
            };
        }

        /// <summary>
        /// 格式化长整数
        /// </summary>
        /// <param name="format">数字格式</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(this NumberFormat format, long value)
        {
            return format switch
            {
                NumberFormat.Default => value.ToString(),
                NumberFormat.Integer => value.ToString("D"),
                NumberFormat.WithThousandsSeparator => value.ToString("N0"),
                NumberFormat.FixedDecimal2 => value.ToString("F2"),
                NumberFormat.FixedDecimal4 => value.ToString("F4"),
                NumberFormat.Percentage => (value / 100.0).ToString("P0"),
                NumberFormat.PercentageWithDecimal => (value / 100.0).ToString("P2"),
                NumberFormat.Scientific => value.ToString("E2"),
                NumberFormat.HexLowercase => value.ToString("x"),
                NumberFormat.HexUppercase => value.ToString("X"),
                NumberFormat.Currency => value.ToString("C"),
                NumberFormat.Compact => FormatCompact(value),
                NumberFormat.Bytes => FormatBytes(value),
                NumberFormat.WithSign => FormatWithSign(value),
                _ => value.ToString()
            };
        }

        /// <summary>
        /// 格式化浮点数
        /// </summary>
        /// <param name="format">数字格式</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(this NumberFormat format, float value)
        {
            return format switch
            {
                NumberFormat.Default => value.ToString(CultureInfo.InvariantCulture),
                NumberFormat.Integer => ((int)value).ToString("D"),
                NumberFormat.WithThousandsSeparator => value.ToString("N2"),
                NumberFormat.FixedDecimal2 => value.ToString("F2"),
                NumberFormat.FixedDecimal4 => value.ToString("F4"),
                NumberFormat.Percentage => value.ToString("P0"),
                NumberFormat.PercentageWithDecimal => value.ToString("P2"),
                NumberFormat.Scientific => value.ToString("E2"),
                NumberFormat.HexLowercase => ((int)value).ToString("x"),
                NumberFormat.HexUppercase => ((int)value).ToString("X"),
                NumberFormat.Currency => value.ToString("C"),
                NumberFormat.Compact => FormatCompact(value),
                NumberFormat.Bytes => FormatBytes((long)value),
                NumberFormat.WithSign => FormatWithSign(value),
                _ => value.ToString(CultureInfo.InvariantCulture)
            };
        }

        /// <summary>
        /// 格式化双精度浮点数
        /// </summary>
        /// <param name="format">数字格式</param>
        /// <param name="value">要格式化的值</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(this NumberFormat format, double value)
        {
            return format switch
            {
                NumberFormat.Default => value.ToString(CultureInfo.InvariantCulture),
                NumberFormat.Integer => ((long)value).ToString("D"),
                NumberFormat.WithThousandsSeparator => value.ToString("N2"),
                NumberFormat.FixedDecimal2 => value.ToString("F2"),
                NumberFormat.FixedDecimal4 => value.ToString("F4"),
                NumberFormat.Percentage => value.ToString("P0"),
                NumberFormat.PercentageWithDecimal => value.ToString("P2"),
                NumberFormat.Scientific => value.ToString("E2"),
                NumberFormat.HexLowercase => ((long)value).ToString("x"),
                NumberFormat.HexUppercase => ((long)value).ToString("X"),
                NumberFormat.Currency => value.ToString("C"),
                NumberFormat.Compact => FormatCompact(value),
                NumberFormat.Bytes => FormatBytes((long)value),
                NumberFormat.WithSign => FormatWithSign(value),
                _ => value.ToString(CultureInfo.InvariantCulture)
            };
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 格式化为紧凑格式 (K, M, B, T)
        /// </summary>
        private static string FormatCompact(double value)
        {
            double absValue = Math.Abs(value);
            string sign = value < 0 ? "-" : "";

            if (absValue >= 1_000_000_000_000)
                return $"{sign}{absValue / 1_000_000_000_000:F1}T";
            if (absValue >= 1_000_000_000)
                return $"{sign}{absValue / 1_000_000_000:F1}B";
            if (absValue >= 1_000_000)
                return $"{sign}{absValue / 1_000_000:F1}M";
            if (absValue >= 1_000)
                return $"{sign}{absValue / 1_000:F1}K";

            return value.ToString("F0");
        }

        /// <summary>
        /// 格式化为字节格式 (B, KB, MB, GB, TB)
        /// </summary>
        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return order == 0 
                ? $"{len:F0} {sizes[order]}" 
                : $"{len:F2} {sizes[order]}";
        }

        /// <summary>
        /// 格式化带正负号
        /// </summary>
        private static string FormatWithSign(double value)
        {
            if (value > 0)
                return $"+{value:F2}";
            if (value < 0)
                return value.ToString("F2");
            return "0";
        }

        /// <summary>
        /// 获取格式的描述
        /// </summary>
        /// <param name="format">数字格式</param>
        /// <returns>描述字符串</returns>
        public static string GetDescription(this NumberFormat format)
        {
            return format switch
            {
                NumberFormat.Default => "默认",
                NumberFormat.Integer => "整数",
                NumberFormat.WithThousandsSeparator => "千位分隔",
                NumberFormat.FixedDecimal2 => "两位小数",
                NumberFormat.FixedDecimal4 => "四位小数",
                NumberFormat.Percentage => "百分比",
                NumberFormat.PercentageWithDecimal => "百分比(小数)",
                NumberFormat.Scientific => "科学计数",
                NumberFormat.HexLowercase => "十六进制(小写)",
                NumberFormat.HexUppercase => "十六进制(大写)",
                NumberFormat.Currency => "货币",
                NumberFormat.Compact => "紧凑格式",
                NumberFormat.Bytes => "字节格式",
                NumberFormat.WithSign => "带符号",
                _ => "未知"
            };
        }

        #endregion
    }
}
