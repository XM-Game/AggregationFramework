// ==========================================================
// 文件名：DateOnlyExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// 说明: DateOnly 类型仅在 .NET 6+ 中可用，Unity 2022.3 使用 .NET Standard 2.1 不支持
// ==========================================================

// DateOnly 仅在 .NET 6+ 中可用，Unity 当前版本不支持
#if NET6_0_OR_GREATER
using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// DateOnly 扩展方法集合（.NET 6+ / Unity 2022.2+）
    /// </summary>
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// 判断是否为今天
        /// </summary>
        public static bool IsToday(this DateOnly date)
        {
            return date == DateOnly.FromDateTime(DateTime.Today);
        }

        /// <summary>
        /// 判断是否为周末
        /// </summary>
        public static bool IsWeekend(this DateOnly date)
        {
            var dayOfWeek = date.DayOfWeek;
            return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// 判断是否为工作日
        /// </summary>
        public static bool IsWeekday(this DateOnly date)
        {
            return !IsWeekend(date);
        }

        /// <summary>
        /// 获取月初日期
        /// </summary>
        public static DateOnly StartOfMonth(this DateOnly date)
        {
            return new DateOnly(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 获取月末日期
        /// </summary>
        public static DateOnly EndOfMonth(this DateOnly date)
        {
            return StartOfMonth(date).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取周初日期（周一）
        /// </summary>
        public static DateOnly StartOfWeek(this DateOnly date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-diff);
        }

        /// <summary>
        /// 获取周末日期（周日）
        /// </summary>
        public static DateOnly EndOfWeek(this DateOnly date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return StartOfWeek(date, startOfWeek).AddDays(6);
        }

        /// <summary>
        /// 转换为 DateTime
        /// </summary>
        public static DateTime ToDateTime(this DateOnly date, TimeOnly time = default)
        {
            return date.ToDateTime(time);
        }

        /// <summary>
        /// 计算年龄
        /// </summary>
        public static int CalculateAge(this DateOnly birthDate)
        {
            return CalculateAge(birthDate, DateOnly.FromDateTime(DateTime.Today));
        }

        /// <summary>
        /// 计算指定日期时的年龄
        /// </summary>
        public static int CalculateAge(this DateOnly birthDate, DateOnly referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;

            if (referenceDate.Month < birthDate.Month ||
                (referenceDate.Month == birthDate.Month && referenceDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }
    }
}
#endif
