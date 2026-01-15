// ==========================================================
// 文件名：SerializeMode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化模式枚举
    /// <para>定义序列化器的工作模式，影响序列化行为和性能</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用高性能模式
    /// var options = new SerializeOptions { Mode = SerializeMode.Object };
    /// 
    /// // 使用版本容错模式
    /// var options = new SerializeOptions { Mode = SerializeMode.VersionTolerant };
    /// 
    /// // 检查模式特性
    /// if (mode.SupportsVersionTolerance())
    ///     // 处理版本兼容逻辑
    /// </code>
    /// </remarks>
    public enum SerializeMode : byte
    {
        /// <summary>
        /// Object 模式 (默认)
        /// <para>按字段声明顺序序列化，性能最优</para>
        /// <para>特点：零开销、紧凑格式、不支持版本容错</para>
        /// <para>适用：高性能网络通信、实时数据同步</para>
        /// </summary>
        Object = 0,

        /// <summary>
        /// 版本容错模式
        /// <para>支持字段顺序变化、字段增减</para>
        /// <para>特点：包含字段标识、支持默认值、向后兼容</para>
        /// <para>适用：长期存储数据、配置文件、存档系统</para>
        /// </summary>
        VersionTolerant = 1,

        /// <summary>
        /// 循环引用模式
        /// <para>支持对象图中的循环引用</para>
        /// <para>特点：引用跟踪、对象ID分配、自动解引用</para>
        /// <para>适用：复杂对象图、树形结构、双向关联</para>
        /// </summary>
        CircularReference = 2,

        /// <summary>
        /// 多态模式
        /// <para>支持接口和抽象类的多态序列化</para>
        /// <para>特点：保存类型信息、运行时类型恢复</para>
        /// <para>适用：插件系统、策略模式、工厂模式</para>
        /// </summary>
        Polymorphic = 3,

        /// <summary>
        /// 流式模式
        /// <para>支持大数据流式处理</para>
        /// <para>特点：分块处理、内存友好、进度回调</para>
        /// <para>适用：大型文件、网络流、增量同步</para>
        /// </summary>
        Streaming = 4,

        /// <summary>
        /// 完整模式
        /// <para>组合版本容错、循环引用、多态支持</para>
        /// <para>特点：功能完整、性能适中</para>
        /// <para>适用：通用场景、复杂数据结构</para>
        /// </summary>
        Full = 5,

        /// <summary>
        /// 紧凑模式
        /// <para>最小化序列化数据大小</para>
        /// <para>特点：位打包、变长编码、压缩优化</para>
        /// <para>适用：带宽受限、存储受限场景</para>
        /// </summary>
        Compact = 6,

        /// <summary>
        /// 调试模式
        /// <para>包含额外的调试信息</para>
        /// <para>特点：类型名称、字段名称、校验信息</para>
        /// <para>适用：开发调试、问题排查</para>
        /// </summary>
        Debug = 7
    }

    /// <summary>
    /// SerializeMode 扩展方法
    /// </summary>
    public static class SerializeModeExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否支持版本容错
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果支持版本容错返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsVersionTolerance(this SerializeMode mode)
        {
            return mode == SerializeMode.VersionTolerant ||
                   mode == SerializeMode.Full ||
                   mode == SerializeMode.Debug;
        }

        /// <summary>
        /// 检查是否支持循环引用
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果支持循环引用返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsCircularReference(this SerializeMode mode)
        {
            return mode == SerializeMode.CircularReference ||
                   mode == SerializeMode.Full ||
                   mode == SerializeMode.Debug;
        }

        /// <summary>
        /// 检查是否支持多态
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果支持多态返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsPolymorphism(this SerializeMode mode)
        {
            return mode == SerializeMode.Polymorphic ||
                   mode == SerializeMode.Full ||
                   mode == SerializeMode.Debug;
        }

        /// <summary>
        /// 检查是否为流式模式
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果是流式模式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStreaming(this SerializeMode mode)
        {
            return mode == SerializeMode.Streaming;
        }

        /// <summary>
        /// 检查是否包含调试信息
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果包含调试信息返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasDebugInfo(this SerializeMode mode)
        {
            return mode == SerializeMode.Debug;
        }

        /// <summary>
        /// 检查是否为高性能模式
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>如果是高性能模式返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighPerformance(this SerializeMode mode)
        {
            return mode == SerializeMode.Object || mode == SerializeMode.Compact;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取模式的中文描述
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>中文描述</returns>
        public static string GetDescription(this SerializeMode mode)
        {
            return mode switch
            {
                SerializeMode.Object => "对象模式 (高性能)",
                SerializeMode.VersionTolerant => "版本容错模式",
                SerializeMode.CircularReference => "循环引用模式",
                SerializeMode.Polymorphic => "多态模式",
                SerializeMode.Streaming => "流式模式",
                SerializeMode.Full => "完整模式",
                SerializeMode.Compact => "紧凑模式",
                SerializeMode.Debug => "调试模式",
                _ => "未知模式"
            };
        }

        /// <summary>
        /// 获取模式的英文名称
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>英文名称</returns>
        public static string GetName(this SerializeMode mode)
        {
            return mode switch
            {
                SerializeMode.Object => "Object",
                SerializeMode.VersionTolerant => "VersionTolerant",
                SerializeMode.CircularReference => "CircularReference",
                SerializeMode.Polymorphic => "Polymorphic",
                SerializeMode.Streaming => "Streaming",
                SerializeMode.Full => "Full",
                SerializeMode.Compact => "Compact",
                SerializeMode.Debug => "Debug",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// 获取预估的性能等级 (1-5，5为最高)
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>性能等级</returns>
        public static int GetPerformanceLevel(this SerializeMode mode)
        {
            return mode switch
            {
                SerializeMode.Object => 5,
                SerializeMode.Compact => 4,
                SerializeMode.Streaming => 4,
                SerializeMode.VersionTolerant => 3,
                SerializeMode.Polymorphic => 3,
                SerializeMode.CircularReference => 2,
                SerializeMode.Full => 2,
                SerializeMode.Debug => 1,
                _ => 0
            };
        }

        /// <summary>
        /// 获取预估的功能等级 (1-5，5为最高)
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <returns>功能等级</returns>
        public static int GetFeatureLevel(this SerializeMode mode)
        {
            return mode switch
            {
                SerializeMode.Object => 1,
                SerializeMode.Compact => 2,
                SerializeMode.Streaming => 2,
                SerializeMode.VersionTolerant => 3,
                SerializeMode.Polymorphic => 3,
                SerializeMode.CircularReference => 3,
                SerializeMode.Full => 5,
                SerializeMode.Debug => 5,
                _ => 0
            };
        }

        #endregion

        #region 模式组合方法

        /// <summary>
        /// 获取推荐的网络通信模式
        /// </summary>
        /// <returns>推荐的序列化模式</returns>
        public static SerializeMode GetNetworkRecommended()
        {
            return SerializeMode.Object;
        }

        /// <summary>
        /// 获取推荐的存储模式
        /// </summary>
        /// <returns>推荐的序列化模式</returns>
        public static SerializeMode GetStorageRecommended()
        {
            return SerializeMode.VersionTolerant;
        }

        /// <summary>
        /// 获取推荐的配置文件模式
        /// </summary>
        /// <returns>推荐的序列化模式</returns>
        public static SerializeMode GetConfigRecommended()
        {
            return SerializeMode.VersionTolerant;
        }

        /// <summary>
        /// 获取推荐的游戏存档模式
        /// </summary>
        /// <returns>推荐的序列化模式</returns>
        public static SerializeMode GetSaveGameRecommended()
        {
            return SerializeMode.Full;
        }

        #endregion
    }
}
