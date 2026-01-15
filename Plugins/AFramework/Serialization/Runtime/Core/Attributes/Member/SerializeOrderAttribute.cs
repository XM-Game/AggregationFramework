// ==========================================================
// 文件名：SerializeOrderAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化顺序特性
    /// <para>指定字段或属性的序列化顺序</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable(Layout = SerializeLayout.Explicit)]
    /// public class PlayerData
    /// {
    ///     [SerializeOrder(0)]
    ///     public int Version;
    ///     
    ///     [SerializeOrder(1)]
    ///     public int Id;
    ///     
    ///     [SerializeOrder(2, Group = 1)]
    ///     public string Name;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeOrderAttribute : Attribute, IComparable<SerializeOrderAttribute>
    {
        #region 常量

        /// <summary>
        /// 未指定顺序
        /// </summary>
        public const int UnspecifiedOrder = -1;

        /// <summary>
        /// 最小顺序值
        /// </summary>
        public const int MinOrder = 0;

        /// <summary>
        /// 默认分组
        /// </summary>
        public const int DefaultGroup = 0;

        /// <summary>
        /// 默认优先级
        /// </summary>
        public const int DefaultPriority = 0;

        #endregion

        #region 属性

        /// <summary>
        /// 获取序列化顺序
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 获取或设置分组
        /// <para>默认值：0</para>
        /// </summary>
        public int Group { get; set; } = DefaultGroup;

        /// <summary>
        /// 获取或设置优先级（值越大优先级越高）
        /// <para>默认值：0</para>
        /// </summary>
        public int Priority { get; set; } = DefaultPriority;

        /// <summary>
        /// 获取或设置是否为首成员
        /// <para>默认值：false</para>
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 获取或设置是否为末成员
        /// <para>默认值：false</para>
        /// </summary>
        public bool IsLast { get; set; }

        /// <summary>
        /// 获取或设置前置成员名称
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// 获取或设置后置成员名称
        /// </summary>
        public string After { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeOrderAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">序列化顺序（必须大于等于 0）</param>
        /// <exception cref="ArgumentOutOfRangeException">顺序值小于 0</exception>
        public SerializeOrderAttribute(int order)
        {
            if (order < MinOrder)
                throw new ArgumentOutOfRangeException(nameof(order),
                    $"序列化顺序必须大于等于 {MinOrder}");

            Order = order;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeOrderAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">序列化顺序</param>
        /// <param name="group">分组</param>
        public SerializeOrderAttribute(int order, int group) : this(order)
        {
            Group = group;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有相对位置约束
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRelativeConstraint()
        {
            return !string.IsNullOrEmpty(Before) || !string.IsNullOrEmpty(After);
        }

        /// <summary>
        /// 检查是否有位置标记
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasPositionMarker() => IsFirst || IsLast;

        /// <summary>
        /// 获取排序键
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Group, int Order, int InversePriority) GetSortKey()
        {
            return (Group, Order, -Priority);
        }

        /// <summary>
        /// 比较两个顺序特性
        /// </summary>
        public int CompareTo(SerializeOrderAttribute other)
        {
            if (other == null) return 1;

            // 首先比较首/末标记
            if (IsFirst != other.IsFirst)
                return IsFirst ? -1 : 1;
            if (IsLast != other.IsLast)
                return IsLast ? 1 : -1;

            // 然后比较分组
            var groupCompare = Group.CompareTo(other.Group);
            if (groupCompare != 0) return groupCompare;

            // 然后比较顺序
            var orderCompare = Order.CompareTo(other.Order);
            if (orderCompare != 0) return orderCompare;

            // 最后比较优先级（优先级高的排前面）
            return other.Priority.CompareTo(Priority);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Group != DefaultGroup
                ? $"[SerializeOrder({Order}, Group = {Group})]"
                : $"[SerializeOrder({Order})]";
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 创建首成员顺序特性
        /// </summary>
        public static SerializeOrderAttribute CreateFirst(int order = 0)
        {
            return new SerializeOrderAttribute(order) { IsFirst = true };
        }

        /// <summary>
        /// 创建末成员顺序特性
        /// </summary>
        public static SerializeOrderAttribute CreateLast(int order = 0)
        {
            return new SerializeOrderAttribute(order) { IsLast = true };
        }

        /// <summary>
        /// 比较两个顺序特性
        /// </summary>
        public static int Compare(SerializeOrderAttribute a, SerializeOrderAttribute b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.CompareTo(b);
        }

        #endregion
    }
}
