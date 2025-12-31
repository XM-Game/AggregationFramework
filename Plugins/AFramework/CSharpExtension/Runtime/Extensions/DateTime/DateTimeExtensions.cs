// ==========================================================
// 文件名：DateTimeExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// DateTime 扩展方法
    /// <para>提供日期时间的常用操作扩展，包括格式化、计算、比较等功能</para>
    /// </summary>
    public static class DateTimeExtensions
    {
        #region 格式化操作

        /// <summary>
        /// 转换为 ISO 8601 格式字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString(StringConstants.DateTimeFormats.ISO8601);
        }

        /// <summary>
        /// 转换为短日期字符串 (yyyy-MM-dd)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToShortDateString(this DateTime dateTime)
        {
            return dateTime.ToString(StringConstants.DateTimeFormats.ShortDate);
        }

        /// <summary>
        /// 转换为长日期字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLongDateString(this DateTime dateTime)
        {
            return dateTime.ToString(StringConstants.DateTimeFormats.LongDate);
        }

        /// <summary>
        /// 转换为文件名安全的字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFileNameSafe(this DateTime dateTime)
        {
            return dateTime.ToString(StringConstants.DateTimeFormats.FileNameSafe);
        }

        /// <summary>
        /// 转换为日志时间戳格式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLogTimestamp(this DateTime dateTime)
        {
            return dateTime.ToString(StringConstants.DateTimeFormats.LogTimestamp);
        }

        #endregion

        #region 日期计算

        /// <summary>
        /// 获取当天的开始时间（00:00:00）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// 获取当天的结束时间（23:59:59.999）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 获取本周的开始时间（周一 00:00:00）
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-diff).Date;
        }

        /// <summary>
        /// 获取本周的结束时间（周日 23:59:59.999）
        /// </summary>
        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return dateTime.StartOfWeek(startOfWeek).AddDays(7).AddMilliseconds(-1);
        }

        /// <summary>
        /// 获取本月的开始时间（1号 00:00:00）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 获取本月的结束时间（最后一天 23:59:59.999）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return dateTime.StartOfMonth().AddMonths(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 获取本年的开始时间（1月1日 00:00:00）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime StartOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        /// <summary>
        /// 获取本年的结束时间（12月31日 23:59:59.999）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime EndOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999);
        }

        #endregion

        #region 日期判断

        /// <summary>
        /// 检查是否为今天
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// 检查是否为昨天
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// 检查是否为明天
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// 检查是否为本周
        /// </summary>
        public static bool IsThisWeek(this DateTime dateTime)
        {
            var now = DateTime.Now;
            return dateTime >= now.StartOfWeek() && dateTime <= now.EndOfWeek();
        }

        /// <summary>
        /// 检查是否为本月
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsThisMonth(this DateTime dateTime)
        {
            var now = DateTime.Now;
            return dateTime.Year == now.Year && dateTime.Month == now.Month;
        }

        /// <summary>
        /// 检查是否为本年
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsThisYear(this DateTime dateTime)
        {
            return dateTime.Year == DateTime.Now.Year;
        }

        /// <summary>
        /// 检查是否为周末
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// 检查是否为工作日
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWeekday(this DateTime dateTime)
        {
            return !dateTime.IsWeekend();
        }

        /// <summary>
        /// 检查是否为过去的时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPast(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }

        /// <summary>
        /// 检查是否为未来的时间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFuture(this DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        #endregion

        #region 时间差计算

        /// <summary>
        /// 计算与当前时间的时间差
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan TimeFromNow(this DateTime dateTime)
        {
            return dateTime - DateTime.Now;
        }

        /// <summary>
        /// 计算距离当前时间的时间差
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan TimeUntilNow(this DateTime dateTime)
        {
            return DateTime.Now - dateTime;
        }

        /// <summary>
        /// 获取年龄（按年计算）
        /// </summary>
        public static int GetAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// 计算两个日期之间的天数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DaysBetween(this DateTime from, DateTime to)
        {
            return (to.Date - from.Date).Days;
        }

        /// <summary>
        /// 计算两个日期之间的月数
        /// </summary>
        public static int MonthsBetween(this DateTime from, DateTime to)
        {
            return ((to.Year - from.Year) * 12) + to.Month - from.Month;
        }

        /// <summary>
        /// 计算两个日期之间的年数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int YearsBetween(this DateTime from, DateTime to)
        {
            return to.Year - from.Year;
        }

        #endregion

        #region 友好时间显示

        /// <summary>
        /// 转换为友好的时间描述（如"刚刚"、"5分钟前"等）
        /// </summary>
        public static string ToFriendlyString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "刚刚";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} 分钟前";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} 小时前";

            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} 天前";

            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} 周前";

            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} 个月前";

            return $"{(int)(timeSpan.TotalDays / 365)} 年前";
        }

        #endregion

        #region Unix 时间戳

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 转换为 Unix 时间戳（秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// 转换为 Unix 时间戳（毫秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// 从 Unix 时间戳（秒）转换为 DateTime
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// 从 Unix 时间戳（毫秒）转换为 DateTime
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromUnixTimestampMilliseconds(long timestamp)
        {
            return UnixEpoch.AddMilliseconds(timestamp).ToLocalTime();
        }

        #endregion

        #region 日期修改

        /// <summary>
        /// 设置时间部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime SetTime(this DateTime dateTime, int hour, int minute, int second = 0, int millisecond = 0)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, millisecond, dateTime.Kind);
        }

        /// <summary>
        /// 设置日期部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime SetDate(this DateTime dateTime, int year, int month, int day)
        {
            return new DateTime(year, month, day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        /// <summary>
        /// 添加工作日
        /// </summary>
        public static DateTime AddWorkdays(this DateTime dateTime, int workdays)
        {
            int direction = workdays < 0 ? -1 : 1;
            int daysToAdd = Math.Abs(workdays);
            var result = dateTime;

            while (daysToAdd > 0)
            {
                result = result.AddDays(direction);
                if (result.IsWeekday())
                    daysToAdd--;
            }

            return result;
        }

        #endregion

        #region 范围检查

        /// <summary>
        /// 检查日期是否在指定范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBetween(this DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }

        #endregion
    }
}
