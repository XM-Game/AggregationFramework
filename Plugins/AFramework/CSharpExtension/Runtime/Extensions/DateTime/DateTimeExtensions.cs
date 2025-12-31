// ==========================================================
// 文件名：DateTimeExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// DateTime 扩展方法集合
    /// </summary>
    public static class DateTimeExtensions
    {
        #region 时间戳转换

        /// <summary>
        /// 转换为 Unix 时间戳（秒）
        /// </summary>
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }

        /// <summary>
        /// 转换为 Unix 时间戳（毫秒）
        /// </summary>
        public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// 从 Unix 时间戳（秒）创建 DateTime
        /// </summary>
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// 从 Unix 时间戳（毫秒）创建 DateTime
        /// </summary>
        public static DateTime FromUnixTimestampMilliseconds(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp).ToLocalTime();
        }

        #endregion

        #region 日期判断

        /// <summary>
        /// 判断是否为今天
        /// </summary>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// 判断是否为昨天
        /// </summary>
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// 判断是否为明天
        /// </summary>
        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// 判断是否为周末
        /// </summary>
        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// 判断是否为工作日
        /// </summary>
        public static bool IsWeekday(this DateTime dateTime)
        {
            return !IsWeekend(dateTime);
        }

        /// <summary>
        /// 判断是否在指定日期范围内
        /// </summary>
        public static bool IsBetween(this DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }

        /// <summary>
        /// 判断是否为闰年
        /// </summary>
        public static bool IsLeapYear(this DateTime dateTime)
        {
            return DateTime.IsLeapYear(dateTime.Year);
        }

        #endregion

        #region 日期计算

        /// <summary>
        /// 获取月初日期
        /// </summary>
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }

        /// <summary>
        /// 获取月末日期
        /// </summary>
        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return StartOfMonth(dateTime).AddMonths(1).AddDays(-1).EndOfDay();
        }

        /// <summary>
        /// 获取周初日期（周一）
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-diff).Date;
        }

        /// <summary>
        /// 获取周末日期（周日）
        /// </summary>
        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return StartOfWeek(dateTime, startOfWeek).AddDays(6).EndOfDay();
        }

        /// <summary>
        /// 获取年初日期
        /// </summary>
        public static DateTime StartOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1, 0, 0, 0, dateTime.Kind);
        }

        /// <summary>
        /// 获取年末日期
        /// </summary>
        public static DateTime EndOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999, dateTime.Kind);
        }

        /// <summary>
        /// 获取当天开始时间（00:00:00）
        /// </summary>
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// 获取当天结束时间（23:59:59）
        /// </summary>
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 获取下一个指定星期几的日期
        /// </summary>
        public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
        {
            int daysToAdd = ((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7;
            if (daysToAdd == 0)
                daysToAdd = 7;

            return dateTime.AddDays(daysToAdd);
        }

        /// <summary>
        /// 获取上一个指定星期几的日期
        /// </summary>
        public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
        {
            int daysToSubtract = ((int)dateTime.DayOfWeek - (int)dayOfWeek + 7) % 7;
            if (daysToSubtract == 0)
                daysToSubtract = 7;

            return dateTime.AddDays(-daysToSubtract);
        }

        #endregion

        #region 年龄计算

        /// <summary>
        /// 计算年龄
        /// </summary>
        public static int CalculateAge(this DateTime birthDate)
        {
            return CalculateAge(birthDate, DateTime.Today);
        }

        /// <summary>
        /// 计算指定日期时的年龄
        /// </summary>
        public static int CalculateAge(this DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;

            if (referenceDate.Month < birthDate.Month ||
                (referenceDate.Month == birthDate.Month && referenceDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }

        #endregion

        #region 格式化

        /// <summary>
        /// 转换为友好的时间描述（如：刚刚、5分钟前、昨天等）
        /// </summary>
        public static string ToFriendlyString(this DateTime dateTime)
        {
            TimeSpan span = DateTime.Now - dateTime;

            if (span.TotalSeconds < 60)
                return "刚刚";

            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}分钟前";

            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}小时前";

            if (span.TotalDays < 2)
                return "昨天";

            if (span.TotalDays < 7)
                return $"{(int)span.TotalDays}天前";

            if (span.TotalDays < 30)
                return $"{(int)(span.TotalDays / 7)}周前";

            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)}个月前";

            return $"{(int)(span.TotalDays / 365)}年前";
        }

        /// <summary>
        /// 转换为 ISO 8601 格式字符串
        /// </summary>
        public static string ToIso8601String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        #endregion

        #region 时间设置

        /// <summary>
        /// 设置时间部分
        /// </summary>
        public static DateTime SetTime(this DateTime dateTime, int hour, int minute = 0, int second = 0, int millisecond = 0)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, millisecond, dateTime.Kind);
        }

        /// <summary>
        /// 设置日期部分
        /// </summary>
        public static DateTime SetDate(this DateTime dateTime, int year, int month, int day)
        {
            return new DateTime(year, month, day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        #endregion
    }
}
