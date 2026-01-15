// ==========================================================
// 文件名：DefaultValueSource.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 默认值来源枚举
    /// </summary>
    public enum DefaultValueSource : byte
    {
        /// <summary>
        /// 显式指定的值
        /// </summary>
        Explicit = 0,

        /// <summary>
        /// 工厂方法
        /// </summary>
        FactoryMethod = 1,

        /// <summary>
        /// 默认构造函数
        /// </summary>
        DefaultConstructor = 2,

        /// <summary>
        /// 类型默认值
        /// </summary>
        TypeDefault = 3,

        /// <summary>
        /// 配置文件
        /// </summary>
        Configuration = 4,

        /// <summary>
        /// 运行时计算
        /// </summary>
        Runtime = 5
    }

    /// <summary>
    /// DefaultValueSource 扩展方法
    /// </summary>
    public static class DefaultValueSourceExtensions
    {
        /// <summary>
        /// 获取来源的中文描述
        /// </summary>
        public static string GetDescription(this DefaultValueSource source)
        {
            return source switch
            {
                DefaultValueSource.Explicit => "显式指定",
                DefaultValueSource.FactoryMethod => "工厂方法",
                DefaultValueSource.DefaultConstructor => "默认构造函数",
                DefaultValueSource.TypeDefault => "类型默认值",
                DefaultValueSource.Configuration => "配置文件",
                DefaultValueSource.Runtime => "运行时计算",
                _ => "未知来源"
            };
        }

        /// <summary>
        /// 检查是否需要运行时解析
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresRuntimeResolution(this DefaultValueSource source)
        {
            return source == DefaultValueSource.FactoryMethod ||
                   source == DefaultValueSource.DefaultConstructor ||
                   source == DefaultValueSource.Configuration ||
                   source == DefaultValueSource.Runtime;
        }

        /// <summary>
        /// 检查是否可缓存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCacheable(this DefaultValueSource source)
        {
            return source == DefaultValueSource.Explicit ||
                   source == DefaultValueSource.TypeDefault;
        }
    }
}
