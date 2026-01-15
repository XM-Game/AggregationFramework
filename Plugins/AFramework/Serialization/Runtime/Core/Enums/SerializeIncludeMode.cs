// ==========================================================
// 文件名：SerializeIncludeMode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化包含模式枚举
    /// </summary>
    public enum SerializeIncludeMode : byte
    {
        /// <summary>
        /// 始终包含（默认）
        /// <para>序列化和反序列化时都包含此成员</para>
        /// </summary>
        Always = 0,

        /// <summary>
        /// 仅序列化时包含
        /// <para>序列化时包含，反序列化时跳过</para>
        /// <para>适用：只写数据</para>
        /// </summary>
        OnSerialize = 1,

        /// <summary>
        /// 仅反序列化时包含
        /// <para>序列化时跳过，反序列化时包含</para>
        /// <para>适用：只读数据</para>
        /// </summary>
        OnDeserialize = 2,

        /// <summary>
        /// 条件包含
        /// <para>根据指定条件决定是否包含</para>
        /// <para>需要配合 Condition 属性使用</para>
        /// </summary>
        Conditional = 3,

        /// <summary>
        /// 非空时包含
        /// <para>仅当值不为 null/default 时包含</para>
        /// <para>适用：可选字段</para>
        /// </summary>
        WhenNotNull = 4,

        /// <summary>
        /// 非默认值时包含
        /// <para>仅当值不等于类型默认值时包含</para>
        /// <para>适用：优化数据大小</para>
        /// </summary>
        WhenNotDefault = 5
    }

    /// <summary>
    /// SerializeIncludeMode 扩展方法
    /// </summary>
    public static class SerializeIncludeModeExtensions
    {
        /// <summary>
        /// 获取模式的中文描述
        /// </summary>
        public static string GetDescription(this SerializeIncludeMode mode)
        {
            return mode switch
            {
                SerializeIncludeMode.Always => "始终包含",
                SerializeIncludeMode.OnSerialize => "序列化时包含",
                SerializeIncludeMode.OnDeserialize => "反序列化时包含",
                SerializeIncludeMode.Conditional => "条件包含",
                SerializeIncludeMode.WhenNotNull => "非空时包含",
                SerializeIncludeMode.WhenNotDefault => "非默认值时包含",
                _ => "未知模式"
            };
        }

        /// <summary>
        /// 检查是否需要值检查
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresValueCheck(this SerializeIncludeMode mode)
        {
            return mode == SerializeIncludeMode.WhenNotNull ||
                   mode == SerializeIncludeMode.WhenNotDefault ||
                   mode == SerializeIncludeMode.Conditional;
        }

        /// <summary>
        /// 检查是否影响序列化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AffectsSerialize(this SerializeIncludeMode mode)
        {
            return mode != SerializeIncludeMode.OnDeserialize;
        }

        /// <summary>
        /// 检查是否影响反序列化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AffectsDeserialize(this SerializeIncludeMode mode)
        {
            return mode != SerializeIncludeMode.OnSerialize;
        }
    }
}
