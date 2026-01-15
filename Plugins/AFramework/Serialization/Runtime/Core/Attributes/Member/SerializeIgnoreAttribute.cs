// ==========================================================
// 文件名：SerializeIgnoreAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化忽略特性
    /// <para>标记字段或属性不参与序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     public int Id;
    ///     
    ///     [SerializeIgnore]
    ///     public string CachedDisplayName;
    ///     
    ///     [SerializeIgnore(IgnoreMode = SerializeIgnoreMode.OnSerialize)]
    ///     public DateTime LastModified;
    ///     
    ///     [SerializeIgnore(Reason = "运行时计算值")]
    ///     public float CalculatedScore;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeIgnoreAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置忽略模式
        /// <para>默认值：<see cref="SerializeIgnoreMode.Always"/></para>
        /// </summary>
        public SerializeIgnoreMode IgnoreMode { get; set; } = SerializeIgnoreMode.Always;

        /// <summary>
        /// 获取或设置忽略原因（用于文档和调试）
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 获取或设置条件属性名称
        /// <para>当 IgnoreMode 为 Conditional 时使用</para>
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 获取或设置是否在继承类中也忽略
        /// <para>默认值：true</para>
        /// </summary>
        public bool InheritIgnore { get; set; } = true;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeIgnoreAttribute"/> 的新实例
        /// </summary>
        public SerializeIgnoreAttribute() { }

        /// <summary>
        /// 初始化 <see cref="SerializeIgnoreAttribute"/> 的新实例
        /// </summary>
        /// <param name="ignoreMode">忽略模式</param>
        public SerializeIgnoreAttribute(SerializeIgnoreMode ignoreMode)
        {
            IgnoreMode = ignoreMode;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeIgnoreAttribute"/> 的新实例
        /// </summary>
        /// <param name="reason">忽略原因</param>
        public SerializeIgnoreAttribute(string reason)
        {
            Reason = reason;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否应在序列化时忽略
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldIgnoreOnSerialize()
        {
            return IgnoreMode == SerializeIgnoreMode.Always ||
                   IgnoreMode == SerializeIgnoreMode.OnSerialize;
        }

        /// <summary>
        /// 检查是否应在反序列化时忽略
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldIgnoreOnDeserialize()
        {
            return IgnoreMode == SerializeIgnoreMode.Always ||
                   IgnoreMode == SerializeIgnoreMode.OnDeserialize;
        }

        /// <summary>
        /// 检查是否为条件忽略
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConditional()
        {
            return IgnoreMode == SerializeIgnoreMode.Conditional ||
                   !string.IsNullOrEmpty(Condition);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[SerializeIgnore({IgnoreMode})]";

        #endregion
    }
}
