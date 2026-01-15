// ==========================================================
// 文件名：SerializeIgnoreMode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化忽略模式枚举
    /// </summary>
    public enum SerializeIgnoreMode : byte
    {
        /// <summary>
        /// 始终忽略（默认）
        /// <para>序列化和反序列化时都忽略此成员</para>
        /// </summary>
        Always = 0,

        /// <summary>
        /// 仅序列化时忽略
        /// <para>序列化时跳过，反序列化时正常读取</para>
        /// <para>适用：只读数据、服务端下发数据</para>
        /// </summary>
        OnSerialize = 1,

        /// <summary>
        /// 仅反序列化时忽略
        /// <para>序列化时正常写入，反序列化时跳过</para>
        /// <para>适用：只写数据、客户端上报数据</para>
        /// </summary>
        OnDeserialize = 2,

        /// <summary>
        /// 条件忽略
        /// <para>根据指定条件决定是否忽略</para>
        /// <para>需要配合 Condition 属性使用</para>
        /// </summary>
        Conditional = 3
    }

    /// <summary>
    /// SerializeIgnoreMode 扩展方法
    /// </summary>
    public static class SerializeIgnoreModeExtensions
    {
        /// <summary>
        /// 获取模式的中文描述
        /// </summary>
        /// <param name="mode">忽略模式</param>
        /// <returns>中文描述</returns>
        public static string GetDescription(this SerializeIgnoreMode mode)
        {
            return mode switch
            {
                SerializeIgnoreMode.Always => "始终忽略",
                SerializeIgnoreMode.OnSerialize => "序列化时忽略",
                SerializeIgnoreMode.OnDeserialize => "反序列化时忽略",
                SerializeIgnoreMode.Conditional => "条件忽略",
                _ => "未知模式"
            };
        }

        /// <summary>
        /// 检查是否影响序列化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AffectsSerialize(this SerializeIgnoreMode mode)
        {
            return mode == SerializeIgnoreMode.Always ||
                   mode == SerializeIgnoreMode.OnSerialize ||
                   mode == SerializeIgnoreMode.Conditional;
        }

        /// <summary>
        /// 检查是否影响反序列化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AffectsDeserialize(this SerializeIgnoreMode mode)
        {
            return mode == SerializeIgnoreMode.Always ||
                   mode == SerializeIgnoreMode.OnDeserialize ||
                   mode == SerializeIgnoreMode.Conditional;
        }
    }
}
