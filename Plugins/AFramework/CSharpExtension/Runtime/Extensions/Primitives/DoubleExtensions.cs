// ==========================================================
// 文件名：DoubleExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// double 扩展方法集合
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static double Clamp(this double value, double min, double max)
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
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 判断是否近似相等（考虑浮点误差）
        /// </summary>
        public static bool Approximately(this double value, double other, double epsilon = 1e-10)
        {
            return Math.Abs(value - other) < epsilon;
        }

        /// <summary>
        /// 判断是否为零（考虑浮点误差）
        /// </summary>
        public static bool IsZero(this double value, double epsilon = 1e-10)
        {
            return Math.Abs(value) < epsilon;
        }

        /// <summary>
        /// 四舍五入到指定小数位数
        /// </summary>
        public static double Round(this double value, int decimals = 0)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        public static double Ceiling(this double value)
        {
            return Math.Ceiling(value);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        public static double Floor(this double value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// 计算平方
        /// </summary>
        public static double Squared(this double value)
        {
            return value * value;
        }

        /// <summary>
        /// 计算平方根
        /// </summary>
        public static double Sqrt(this double value)
        {
            return Math.Sqrt(value);
        }

        /// <summary>
        /// 转换为百分比字符串
        /// </summary>
        public static string ToPercentageString(this double value, int decimals = 2)
        {
            return $"{(value * 100).ToString($"F{decimals}")}%";
        }

        /// <summary>
        /// 判断是否为 NaN
        /// </summary>
        public static bool IsNaN(this double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// 判断是否为无穷大
        /// </summary>
        public static bool IsInfinity(this double value)
        {
            return double.IsInfinity(value);
        }

        /// <summary>
        /// 判断是否为有效数字
        /// </summary>
        public static bool IsValid(this double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        public static double Lerp(this double from, double to, double t)
        {
            t = t.Clamp(0, 1);
            return from + (to - from) * t;
        }

        /// <summary>
        /// 反向线性插值（获取插值参数）
        /// </summary>
        public static double InverseLerp(this double value, double from, double to)
        {
            if (Math.Abs(to - from) < 1e-10)
                return 0;

            return ((value - from) / (to - from)).Clamp(0, 1);
        }
    }
}
