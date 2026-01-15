// ==========================================================
// 文件名：EmptyStringHandling.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 空字符串处理方式枚举
    /// <para>定义空字符串在序列化时的处理策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [InternString(EmptyStringHandling = EmptyStringHandling.Intern)]
    /// public string Tag;
    /// </code>
    /// </remarks>
    public enum EmptyStringHandling : byte
    {
        /// <summary>
        /// 内化空字符串（使用 string.Empty）
        /// <para>所有空字符串共享同一实例</para>
        /// </summary>
        Intern = 0,

        /// <summary>
        /// 转换为 null
        /// <para>空字符串被视为 null 处理</para>
        /// </summary>
        Null = 1,

        /// <summary>
        /// 保持原样（不内化）
        /// <para>保留原始空字符串实例</para>
        /// </summary>
        Keep = 2
    }

    /// <summary>
    /// EmptyStringHandling 扩展方法
    /// </summary>
    public static class EmptyStringHandlingExtensions
    {
        /// <summary>
        /// 获取处理方式的中文描述
        /// </summary>
        /// <param name="handling">处理方式</param>
        /// <returns>中文描述</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDescription(this EmptyStringHandling handling)
        {
            return handling switch
            {
                EmptyStringHandling.Intern => "内化为 string.Empty",
                EmptyStringHandling.Null => "转换为 null",
                EmptyStringHandling.Keep => "保持原样",
                _ => "未知处理方式"
            };
        }

        /// <summary>
        /// 处理空字符串
        /// </summary>
        /// <param name="handling">处理方式</param>
        /// <param name="value">原始值（应为空字符串）</param>
        /// <returns>处理后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Process(this EmptyStringHandling handling, string value)
        {
            if (!string.IsNullOrEmpty(value))
                return value;

            return handling switch
            {
                EmptyStringHandling.Intern => string.Empty,
                EmptyStringHandling.Null => null,
                EmptyStringHandling.Keep => value,
                _ => value
            };
        }
    }
}
