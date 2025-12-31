// ==========================================================
// 文件名：LongExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// long/ulong 扩展方法集合
    /// </summary>
    public static class LongExtensions
    {
        #region long 扩展

        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this long value, long min, long max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static long Clamp(this long value, long min, long max)
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
        public static long Abs(this long value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 判断是否为偶数
        /// </summary>
        public static bool IsEven(this long value)
        {
            return (value & 1) == 0;
        }

        /// <summary>
        /// 判断是否为奇数
        /// </summary>
        public static bool IsOdd(this long value)
        {
            return (value & 1) == 1;
        }

        /// <summary>
        /// 转换为文件大小字符串（字节）
        /// </summary>
        public static string ToFileSizeString(this long bytes, int decimals = 2)
        {
            if (bytes < 0)
                return "Invalid";

            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
            double size = bytes;
            int order = 0;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size.ToString($"F{decimals}")} {sizes[order]}";
        }

        /// <summary>
        /// 转换为时间戳字符串（毫秒）
        /// </summary>
        public static DateTime ToDateTime(this long milliseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(milliseconds);
        }

        #endregion

        #region ulong 扩展

        /// <summary>
        /// 判断是否在指定范围内
        /// </summary>
        public static bool IsInRange(this ulong value, ulong min, ulong max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            if (min > max)
                throw new ArgumentException("最小值不能大于最大值");

            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// 判断是否为偶数
        /// </summary>
        public static bool IsEven(this ulong value)
        {
            return (value & 1) == 0;
        }

        /// <summary>
        /// 判断是否为奇数
        /// </summary>
        public static bool IsOdd(this ulong value)
        {
            return (value & 1) == 1;
        }

        #endregion
    }
}
