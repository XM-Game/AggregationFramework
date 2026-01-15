// ==========================================================
// 文件名：SerializeIncludeAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化包含特性
    /// <para>显式标记字段或属性参与序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     [SerializeInclude]
    ///     private string _secretKey;
    ///     
    ///     [SerializeInclude(Order = 0)]
    ///     private int _version;
    ///     
    ///     [SerializeInclude(Name = "hp")]
    ///     public float Health;
    ///     
    ///     [SerializeInclude(Order = 1, Name = "exp", AllowNull = false)]
    ///     private float _experience;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeIncludeAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 未指定顺序
        /// </summary>
        public const int UnspecifiedOrder = -1;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置序列化顺序
        /// <para>默认值：-1（未指定，使用声明顺序）</para>
        /// </summary>
        public int Order { get; set; } = UnspecifiedOrder;

        /// <summary>
        /// 获取或设置序列化名称
        /// <para>默认值：null（使用成员名称）</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置是否允许空值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowNull { get; set; } = true;

        /// <summary>
        /// 获取或设置是否为必需成员
        /// <para>默认值：false</para>
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 获取或设置包含模式
        /// <para>默认值：<see cref="SerializeIncludeMode.Always"/></para>
        /// </summary>
        public SerializeIncludeMode IncludeMode { get; set; } = SerializeIncludeMode.Always;

        /// <summary>
        /// 获取或设置条件属性名称
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 获取或设置成员描述（用于文档和调试）
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeIncludeAttribute"/> 的新实例
        /// </summary>
        public SerializeIncludeAttribute() { }

        /// <summary>
        /// 初始化 <see cref="SerializeIncludeAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">序列化顺序</param>
        public SerializeIncludeAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeIncludeAttribute"/> 的新实例
        /// </summary>
        /// <param name="name">序列化名称</param>
        public SerializeIncludeAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeIncludeAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">序列化顺序</param>
        /// <param name="name">序列化名称</param>
        public SerializeIncludeAttribute(int order, string name)
        {
            Order = order;
            Name = name;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否指定了顺序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasOrder() => Order != UnspecifiedOrder;

        /// <summary>
        /// 检查是否指定了名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasName() => !string.IsNullOrEmpty(Name);

        /// <summary>
        /// 检查是否为条件包含
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConditional()
        {
            return IncludeMode == SerializeIncludeMode.Conditional ||
                   !string.IsNullOrEmpty(Condition);
        }

        /// <summary>
        /// 检查是否应在序列化时包含
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldIncludeOnSerialize()
        {
            return IncludeMode != SerializeIncludeMode.OnDeserialize;
        }

        /// <summary>
        /// 检查是否应在反序列化时包含
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldIncludeOnDeserialize()
        {
            return IncludeMode != SerializeIncludeMode.OnSerialize;
        }

        /// <summary>
        /// 获取有效的序列化名称
        /// </summary>
        /// <param name="memberName">成员原始名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetEffectiveName(string memberName)
        {
            return HasName() ? Name : memberName;
        }

        /// <summary>
        /// 获取有效的序列化顺序
        /// </summary>
        /// <param name="defaultOrder">默认顺序</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEffectiveOrder(int defaultOrder)
        {
            return HasOrder() ? Order : defaultOrder;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (HasOrder() && HasName())
                return $"[SerializeInclude({Order}, \"{Name}\")]";
            if (HasOrder())
                return $"[SerializeInclude({Order})]";
            if (HasName())
                return $"[SerializeInclude(\"{Name}\")]";
            return "[SerializeInclude]";
        }

        #endregion
    }
}
