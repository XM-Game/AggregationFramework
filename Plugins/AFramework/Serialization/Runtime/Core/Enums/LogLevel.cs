// ==========================================================
// 文件名：LogLevel.cs
// 命名空间: AFramework.Serialization
// 依赖: 无
// ==========================================================

namespace AFramework.Serialization
{
    /// <summary>
    /// 日志级别
    /// <para>定义序列化系统的日志输出级别</para>
    /// </summary>
    public enum LogLevel : byte
    {
        /// <summary>
        /// 跟踪 - 最详细的日志级别
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 调试 - 调试信息
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 信息 - 一般信息
        /// </summary>
        Information = 2,

        /// <summary>
        /// 警告 - 警告信息
        /// </summary>
        Warning = 3,

        /// <summary>
        /// 错误 - 错误信息
        /// </summary>
        Error = 4,

        /// <summary>
        /// 严重 - 严重错误
        /// </summary>
        Critical = 5,

        /// <summary>
        /// 无日志 - 禁用日志
        /// </summary>
        None = 6
    }

    /// <summary>
    /// LogLevel 扩展方法
    /// </summary>
    public static class LogLevelExtensions
    {
        /// <summary>
        /// 获取日志级别的中文描述
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns>中文描述</returns>
        public static string GetDescription(this LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "跟踪",
                LogLevel.Debug => "调试",
                LogLevel.Information => "信息",
                LogLevel.Warning => "警告",
                LogLevel.Error => "错误",
                LogLevel.Critical => "严重",
                LogLevel.None => "无",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取日志级别的简写
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns>简写字符串</returns>
        public static string GetShortName(this LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "TRC",
                LogLevel.Debug => "DBG",
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "CRT",
                LogLevel.None => "---",
                _ => "???"
            };
        }

        /// <summary>
        /// 检查是否应该记录指定级别的日志
        /// </summary>
        /// <param name="currentLevel">当前配置的最小级别</param>
        /// <param name="messageLevel">消息级别</param>
        /// <returns>如果应该记录返回 true</returns>
        public static bool ShouldLog(this LogLevel currentLevel, LogLevel messageLevel)
        {
            return messageLevel >= currentLevel && currentLevel != LogLevel.None;
        }
    }
}
