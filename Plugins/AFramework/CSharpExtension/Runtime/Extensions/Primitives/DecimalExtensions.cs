// ==========================================================
// 文件名：DecimalExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// decimal 扩展方法集合
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            if (min > max)
                throw new ArgumentException("最小值不能大于最大值");

            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 转换为绝对值
        /// </summary>
        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 四舍五入到指定小数位数
        /// </summary>
        public static decimal Round(this decimal value, int decimals = 0)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        public static decimal Ceiling(this decimal value)
        {
            return Math.Ceiling(value);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        public static decimal Floor(this decimal value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// 转换为货币字符串
        /// </summary>
        public static string ToCurrencyString(this decimal value, string currencySymbol = "¥")
        {
            return $"{currencySymbol}{value:N2}";
        }

        /// <summary>
        /// 转换为百分比字符串
        /// </summary>
        public static string ToPercentageString(this decimal value, int decimals = 2)
        {
            return $"{(value * 100).ToString($"F{decimals}")}%";
        }

        /// <summary>
        /// 判断是否为零
        /// </summary>
        public static bool IsZero(this decimal value)
        {
            return value == 0m;
        }

        /// <summary>
        /// 判断是否为正数
        /// </summary>
        public static bool IsPositive(this decimal value)
        {
            return value > 0m;
        }

        /// <summary>
        /// 判断是否为负数
        /// </summary>
        public static bool IsNegative(this decimal value)
        {
            return value < 0m;
        }
    }
}
