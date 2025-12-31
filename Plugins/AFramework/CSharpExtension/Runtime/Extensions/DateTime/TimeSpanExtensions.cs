// ==========================================================
// 文件名：TimeSpanExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// TimeSpan 扩展方法
    /// <para>提供时间间隔的常用操作扩展，包括格式化、比较等功能</para>
    /// </summary>
    public static class TimeSpanExtensions
    {
        #region 格式化操作

        /// <summary>
        /// 转换为友好的时间描述
        /// </summary>
        public static string ToFriendlyString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 1)
                return $"{timeSpan.TotalMilliseconds:F0} 毫秒";

            if (timeSpan.TotalMinutes < 1)
                return $"{timeSpan.TotalSeconds:F0} 秒";

            if (timeSpan.TotalHours < 1)
                return $"{timeSpan.TotalMinutes:F0} 分钟";

            if (timeSpan.TotalDays < 1)
                return $"{timeSpan.TotalHours:F1} 小时";

            if (timeSpan.TotalDays < 7)
                return $"{timeSpan.TotalDays:F1} 天";

            if (timeSpan.TotalDays < 30)
                return $"{timeSpan.TotalDays / 7:F1} 周";

            if (timeSpan.TotalDays < 365)
                return $"{timeSpan.TotalDays / 30:F1} 个月";

            return $"{timeSpan.TotalDays / 365:F1} 年";
        }

        /// <summary>
        /// 转换为简短格式（HH:mm:ss）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToShortString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// 转换为完整格式（包含天数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFullString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"d\.hh\:mm\:ss");
        }

        /// <summary>
        /// 转换为精确格式（包含毫秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToPreciseString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss\.fff");
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 检查是否为正值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(this TimeSpan timeSpan)
        {
            return timeSpan > TimeSpan.Zero;
        }

        /// <summary>
        /// 检查是否为负值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(this TimeSpan timeSpan)
        {
            return timeSpan < TimeSpan.Zero;
        }

        /// <summary>
        /// 检查是否为零
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this TimeSpan timeSpan)
        {
            return timeSpan == TimeSpan.Zero;
        }

        #endregion

        #region 数学操作

        /// <summary>
        /// 获取绝对值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Abs(this TimeSpan timeSpan)
        {
            return timeSpan < TimeSpan.Zero ? timeSpan.Negate() : timeSpan;
        }

        /// <summary>
        /// 乘以倍数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Multiply(this TimeSpan timeSpan, double multiplier)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks * multiplier));
        }

        /// <summary>
        /// 除以倍数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Divide(this TimeSpan timeSpan, double divisor)
        {
            if (Math.Abs(divisor) < double.Epsilon)
                throw new DivideByZeroException("Cannot divide TimeSpan by zero.");
            return TimeSpan.FromTicks((long)(timeSpan.Ticks / divisor));
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为总秒数（整数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToSeconds(this TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// 转换为总分钟数（整数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToMinutes(this TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalMinutes;
        }

        /// <summary>
        /// 转换为总小时数（整数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToHours(this TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalHours;
        }

        /// <summary>
        /// 转换为总天数（整数）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToDays(this TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalDays;
        }

        #endregion

        #region 范围检查

        /// <summary>
        /// 检查是否在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this TimeSpan timeSpan, TimeSpan min, TimeSpan max)
        {
            return timeSpan >= min && timeSpan <= max;
        }

        /// <summary>
        /// 限制在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Clamp(this TimeSpan timeSpan, TimeSpan min, TimeSpan max)
        {
            if (timeSpan < min) return min;
            if (timeSpan > max) return max;
            return timeSpan;
        }

        #endregion

        #region 时间单位判断

        /// <summary>
        /// 检查是否小于 1 秒
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanSecond(this TimeSpan timeSpan)
        {
            return timeSpan.TotalSeconds < 1;
        }

        /// <summary>
        /// 检查是否小于 1 分钟
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanMinute(this TimeSpan timeSpan)
        {
            return timeSpan.TotalMinutes < 1;
        }

        /// <summary>
        /// 检查是否小于 1 小时
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanHour(this TimeSpan timeSpan)
        {
            return timeSpan.TotalHours < 1;
        }

        /// <summary>
        /// 检查是否小于 1 天
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanDay(this TimeSpan timeSpan)
        {
            return timeSpan.TotalDays < 1;
        }

        #endregion

        #region 延迟操作

        /// <summary>
        /// 从现在开始延迟指定时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromNow(this TimeSpan timeSpan)
        {
            return DateTime.Now + timeSpan;
        }

        /// <summary>
        /// 从现在开始提前指定时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime Ago(this TimeSpan timeSpan)
        {
            return DateTime.Now - timeSpan;
        }

        #endregion
    }
}
