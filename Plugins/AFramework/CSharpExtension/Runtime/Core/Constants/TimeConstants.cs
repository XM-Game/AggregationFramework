// ==========================================================
// 文件名：TimeConstants.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 时间常量定义
    /// <para>提供时间单位转换常量和常用时间值</para>
    /// <para>包含毫秒、秒、分钟、小时、天等时间单位常量</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 时间单位转换
    /// long totalMs = seconds * TimeConstants.MillisecondsPerSecond;
    /// 
    /// // 使用 TimeSpan 常量
    /// var timeout = TimeSpan.FromSeconds(TimeConstants.Seconds.Minute);
    /// 
    /// // 帧率相关
    /// float frameTime = 1f / TimeConstants.FrameRate.FPS60;
    /// </code>
    /// </remarks>
    public static class TimeConstants
    {
        #region 毫秒转换常量

        /// <summary>每秒毫秒数</summary>
        public const int MillisecondsPerSecond = 1000;

        /// <summary>每分钟毫秒数</summary>
        public const int MillisecondsPerMinute = 60000;

        /// <summary>每小时毫秒数</summary>
        public const int MillisecondsPerHour = 3600000;

        /// <summary>每天毫秒数</summary>
        public const int MillisecondsPerDay = 86400000;

        #endregion

        #region 秒转换常量

        /// <summary>每分钟秒数</summary>
        public const int SecondsPerMinute = 60;

        /// <summary>每小时秒数</summary>
        public const int SecondsPerHour = 3600;

        /// <summary>每天秒数</summary>
        public const int SecondsPerDay = 86400;

        /// <summary>每周秒数</summary>
        public const int SecondsPerWeek = 604800;

        #endregion
    


        #region 分钟转换常量

        /// <summary>每小时分钟数</summary>
        public const int MinutesPerHour = 60;

        /// <summary>每天分钟数</summary>
        public const int MinutesPerDay = 1440;

        /// <summary>每周分钟数</summary>
        public const int MinutesPerWeek = 10080;

        #endregion

        #region 小时转换常量

        /// <summary>每天小时数</summary>
        public const int HoursPerDay = 24;

        /// <summary>每周小时数</summary>
        public const int HoursPerWeek = 168;

        #endregion

        #region 天转换常量

        /// <summary>每周天数</summary>
        public const int DaysPerWeek = 7;

        /// <summary>每年天数 (非闰年)</summary>
        public const int DaysPerYear = 365;

        /// <summary>每闰年天数</summary>
        public const int DaysPerLeapYear = 366;

        /// <summary>每月平均天数 (约)</summary>
        public const float AverageDaysPerMonth = 30.4375f;

        #endregion

        #region Tick 转换常量

        /// <summary>每毫秒 Tick 数</summary>
        public const long TicksPerMillisecond = 10000L;

        /// <summary>每秒 Tick 数</summary>
        public const long TicksPerSecond = 10000000L;

        /// <summary>每分钟 Tick 数</summary>
        public const long TicksPerMinute = 600000000L;

        /// <summary>每小时 Tick 数</summary>
        public const long TicksPerHour = 36000000000L;

        /// <summary>每天 Tick 数</summary>
        public const long TicksPerDay = 864000000000L;

        #endregion

        #region 常用秒数常量

        /// <summary>
        /// 常用秒数常量集合
        /// </summary>
        public static class Seconds
        {
            /// <summary>1 分钟的秒数</summary>
            public const int Minute = 60;

            /// <summary>5 分钟的秒数</summary>
            public const int FiveMinutes = 300;

            /// <summary>10 分钟的秒数</summary>
            public const int TenMinutes = 600;

            /// <summary>15 分钟的秒数</summary>
            public const int FifteenMinutes = 900;

            /// <summary>30 分钟的秒数</summary>
            public const int HalfHour = 1800;

            /// <summary>1 小时的秒数</summary>
            public const int Hour = 3600;

            /// <summary>2 小时的秒数</summary>
            public const int TwoHours = 7200;

            /// <summary>6 小时的秒数</summary>
            public const int SixHours = 21600;

            /// <summary>12 小时的秒数</summary>
            public const int HalfDay = 43200;

            /// <summary>1 天的秒数</summary>
            public const int Day = 86400;

            /// <summary>1 周的秒数</summary>
            public const int Week = 604800;
        }

        #endregion

        #region 帧率常量

        /// <summary>
        /// 帧率相关常量
        /// </summary>
        public static class FrameRate
        {
            /// <summary>30 FPS</summary>
            public const int FPS30 = 30;

            /// <summary>60 FPS</summary>
            public const int FPS60 = 60;

            /// <summary>120 FPS</summary>
            public const int FPS120 = 120;

            /// <summary>144 FPS</summary>
            public const int FPS144 = 144;

            /// <summary>240 FPS</summary>
            public const int FPS240 = 240;

            /// <summary>30 FPS 帧时间 (秒)</summary>
            public const float FrameTime30 = 1f / 30f;

            /// <summary>60 FPS 帧时间 (秒)</summary>
            public const float FrameTime60 = 1f / 60f;

            /// <summary>120 FPS 帧时间 (秒)</summary>
            public const float FrameTime120 = 1f / 120f;

            /// <summary>固定物理更新时间步长 (默认 50 FPS)</summary>
            public const float FixedDeltaTime = 0.02f;
        }

        #endregion

        #region TimeSpan 常量

        /// <summary>
        /// 预定义 TimeSpan 常量
        /// </summary>
        public static class Spans
        {
            /// <summary>零时间间隔</summary>
            public static readonly TimeSpan Zero = TimeSpan.Zero;

            /// <summary>1 毫秒</summary>
            public static readonly TimeSpan OneMillisecond = TimeSpan.FromMilliseconds(1);

            /// <summary>100 毫秒</summary>
            public static readonly TimeSpan HundredMilliseconds = TimeSpan.FromMilliseconds(100);

            /// <summary>1 秒</summary>
            public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

            /// <summary>5 秒</summary>
            public static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);

            /// <summary>10 秒</summary>
            public static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10);

            /// <summary>30 秒</summary>
            public static readonly TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);

            /// <summary>1 分钟</summary>
            public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);

            /// <summary>5 分钟</summary>
            public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);

            /// <summary>10 分钟</summary>
            public static readonly TimeSpan TenMinutes = TimeSpan.FromMinutes(10);

            /// <summary>30 分钟</summary>
            public static readonly TimeSpan HalfHour = TimeSpan.FromMinutes(30);

            /// <summary>1 小时</summary>
            public static readonly TimeSpan OneHour = TimeSpan.FromHours(1);

            /// <summary>1 天</summary>
            public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

            /// <summary>1 周</summary>
            public static readonly TimeSpan OneWeek = TimeSpan.FromDays(7);

            /// <summary>无限超时</summary>
            public static readonly TimeSpan Infinite = TimeSpan.FromMilliseconds(-1);
        }

        #endregion
    }
}
