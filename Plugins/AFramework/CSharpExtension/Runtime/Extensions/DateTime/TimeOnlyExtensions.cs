// ==========================================================
// 文件名：TimeOnlyExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// 说明: TimeOnly 类型仅在 .NET 6+ 中可用，Unity 2022.3 使用 .NET Standard 2.1 不支持
// ==========================================================

// TimeOnly 仅在 .NET 6+ 中可用，Unity 当前版本不支持
#if NET6_0_OR_GREATER
using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// TimeOnly 扩展方法集合（.NET 6+ / Unity 2022.2+）
    /// </summary>
    public static class TimeOnlyExtensions
    {
        /// <summary>
        /// 判断是否在指定时间范围内
        /// </summary>
        public static bool IsBetween(this TimeOnly time, TimeOnly start, TimeOnly end)
        {
            if (start <= end)
            {
                return time >= start && time <= end;
            }
            else
            {
                // 跨越午夜的情况
                return time >= start || time <= end;
            }
        }

        /// <summary>
        /// 判断是否为上午
        /// </summary>
        public static bool IsAM(this TimeOnly time)
        {
            return time.Hour < 12;
        }

        /// <summary>
        /// 判断是否为下午
        /// </summary>
        public static bool IsPM(this TimeOnly time)
        {
            return time.Hour >= 12;
        }

        /// <summary>
        /// 转换为 12 小时制字符串
        /// </summary>
        public static string To12HourFormat(this TimeOnly time)
        {
            int hour = time.Hour % 12;
            if (hour == 0) hour = 12;

            string period = time.Hour < 12 ? "AM" : "PM";
            return $"{hour:D2}:{time.Minute:D2}:{time.Second:D2} {period}";
        }

        /// <summary>
        /// 转换为 TimeSpan
        /// </summary>
        public static TimeSpan ToTimeSpan(this TimeOnly time)
        {
            return time.ToTimeSpan();
        }

        /// <summary>
        /// 添加小时
        /// </summary>
        public static TimeOnly AddHours(this TimeOnly time, double hours)
        {
            return time.Add(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// 添加分钟
        /// </summary>
        public static TimeOnly AddMinutes(this TimeOnly time, double minutes)
        {
            return time.Add(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// 添加秒
        /// </summary>
        public static TimeOnly AddSeconds(this TimeOnly time, double seconds)
        {
            return time.Add(TimeSpan.FromSeconds(seconds));
        }
    }
}
#endif
