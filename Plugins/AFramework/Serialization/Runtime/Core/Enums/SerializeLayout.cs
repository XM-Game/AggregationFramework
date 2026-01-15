// ==========================================================
// 文件名：SerializeLayout.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化布局方式枚举
    /// <para>定义类型成员的序列化布局策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用顺序布局
    /// [Serializable(Layout = SerializeLayout.Sequential)]
    /// public class PlayerData
    /// {
    ///     public int Id;      // 序列化顺序 0
    ///     public string Name; // 序列化顺序 1
    /// }
    /// 
    /// // 使用显式布局
    /// [Serializable(Layout = SerializeLayout.Explicit)]
    /// public class ConfigData
    /// {
    ///     [SerializeOrder(1)] public string Name;
    ///     [SerializeOrder(0)] public int Version;
    /// }
    /// </code>
    /// </remarks>
    public enum SerializeLayout : byte
    {
        /// <summary>
        /// 顺序布局 (默认)
        /// <para>按字段/属性声明顺序序列化</para>
        /// <para>特点：简单直观、性能最优、顺序敏感</para>
        /// <para>注意：字段顺序变化会导致反序列化失败</para>
        /// </summary>
        Sequential = 0,

        /// <summary>
        /// 显式布局
        /// <para>使用 [SerializeOrder] 特性指定顺序</para>
        /// <para>特点：顺序可控、支持重排、需要标记</para>
        /// <para>适用：需要精确控制序列化顺序的场景</para>
        /// </summary>
        Explicit = 1,

        /// <summary>
        /// 名称布局
        /// <para>按成员名称字母顺序序列化</para>
        /// <para>特点：顺序稳定、跨版本兼容、略有开销</para>
        /// <para>适用：需要稳定顺序但不想手动标记</para>
        /// </summary>
        Alphabetical = 2,

        /// <summary>
        /// 键值布局
        /// <para>序列化时包含字段名称作为键</para>
        /// <para>特点：自描述、版本容错、数据较大</para>
        /// <para>适用：版本容错模式、配置文件</para>
        /// </summary>
        KeyValue = 3,

        /// <summary>
        /// 索引布局
        /// <para>使用数字索引标识字段</para>
        /// <para>特点：紧凑、版本容错、需要索引映射</para>
        /// <para>适用：需要版本容错但追求紧凑的场景</para>
        /// </summary>
        Indexed = 4,

        /// <summary>
        /// 自动布局
        /// <para>根据序列化模式自动选择最佳布局</para>
        /// <para>Object模式使用Sequential，VersionTolerant使用KeyValue</para>
        /// </summary>
        Auto = 5
    }

    /// <summary>
    /// SerializeLayout 扩展方法
    /// </summary>
    public static class SerializeLayoutExtensions
    {
        #region 特性检查方法

        /// <summary>
        /// 检查是否为顺序敏感布局
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>如果顺序敏感返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrderSensitive(this SerializeLayout layout)
        {
            return layout == SerializeLayout.Sequential ||
                   layout == SerializeLayout.Explicit ||
                   layout == SerializeLayout.Alphabetical;
        }

        /// <summary>
        /// 检查是否支持版本容错
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>如果支持版本容错返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SupportsVersionTolerance(this SerializeLayout layout)
        {
            return layout == SerializeLayout.KeyValue ||
                   layout == SerializeLayout.Indexed;
        }

        /// <summary>
        /// 检查是否包含字段标识
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>如果包含字段标识返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFieldIdentifier(this SerializeLayout layout)
        {
            return layout == SerializeLayout.KeyValue ||
                   layout == SerializeLayout.Indexed;
        }

        /// <summary>
        /// 检查是否需要显式标记
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>如果需要显式标记返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresExplicitMarking(this SerializeLayout layout)
        {
            return layout == SerializeLayout.Explicit ||
                   layout == SerializeLayout.Indexed;
        }

        /// <summary>
        /// 检查是否为自动布局
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>如果是自动布局返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAuto(this SerializeLayout layout)
        {
            return layout == SerializeLayout.Auto;
        }

        #endregion

        #region 信息获取方法

        /// <summary>
        /// 获取布局的中文描述
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>中文描述</returns>
        public static string GetDescription(this SerializeLayout layout)
        {
            return layout switch
            {
                SerializeLayout.Sequential => "顺序布局",
                SerializeLayout.Explicit => "显式布局",
                SerializeLayout.Alphabetical => "字母顺序布局",
                SerializeLayout.KeyValue => "键值布局",
                SerializeLayout.Indexed => "索引布局",
                SerializeLayout.Auto => "自动布局",
                _ => "未知布局"
            };
        }

        /// <summary>
        /// 获取预估的数据紧凑度 (1-5，5为最紧凑)
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>紧凑度等级</returns>
        public static int GetCompactnessLevel(this SerializeLayout layout)
        {
            return layout switch
            {
                SerializeLayout.Sequential => 5,
                SerializeLayout.Explicit => 5,
                SerializeLayout.Alphabetical => 5,
                SerializeLayout.Indexed => 4,
                SerializeLayout.KeyValue => 2,
                SerializeLayout.Auto => 4,
                _ => 0
            };
        }

        /// <summary>
        /// 获取预估的灵活性等级 (1-5，5为最灵活)
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <returns>灵活性等级</returns>
        public static int GetFlexibilityLevel(this SerializeLayout layout)
        {
            return layout switch
            {
                SerializeLayout.KeyValue => 5,
                SerializeLayout.Indexed => 4,
                SerializeLayout.Explicit => 3,
                SerializeLayout.Alphabetical => 2,
                SerializeLayout.Sequential => 1,
                SerializeLayout.Auto => 4,
                _ => 0
            };
        }

        #endregion

        #region 布局解析方法

        /// <summary>
        /// 根据序列化模式解析自动布局
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <param name="mode">序列化模式</param>
        /// <returns>解析后的实际布局</returns>
        public static SerializeLayout ResolveAuto(this SerializeLayout layout, SerializeMode mode)
        {
            if (layout != SerializeLayout.Auto)
                return layout;

            return mode switch
            {
                SerializeMode.Object => SerializeLayout.Sequential,
                SerializeMode.Compact => SerializeLayout.Sequential,
                SerializeMode.VersionTolerant => SerializeLayout.KeyValue,
                SerializeMode.Full => SerializeLayout.KeyValue,
                SerializeMode.Debug => SerializeLayout.KeyValue,
                SerializeMode.CircularReference => SerializeLayout.Sequential,
                SerializeMode.Polymorphic => SerializeLayout.KeyValue,
                SerializeMode.Streaming => SerializeLayout.Sequential,
                _ => SerializeLayout.Sequential
            };
        }

        /// <summary>
        /// 获取推荐的布局方式
        /// </summary>
        /// <param name="needsVersionTolerance">是否需要版本容错</param>
        /// <param name="needsCompactness">是否需要紧凑</param>
        /// <returns>推荐的布局方式</returns>
        public static SerializeLayout GetRecommended(bool needsVersionTolerance, bool needsCompactness)
        {
            if (needsVersionTolerance)
            {
                return needsCompactness ? SerializeLayout.Indexed : SerializeLayout.KeyValue;
            }
            return SerializeLayout.Sequential;
        }

        #endregion
    }
}
