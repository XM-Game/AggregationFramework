// ==========================================================
// 文件名：TypePair.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义类型对结构体，用于映射配置的键和缓存索引
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型对结构体
    /// <para>表示源类型和目标类型的配对，用于映射配置的键</para>
    /// <para>Type pair struct representing source and destination type pairing</para>
    /// </summary>
    /// <remarks>
    /// TypePair 是 AMapper 内部使用的核心数据结构，用于：
    /// <list type="bullet">
    /// <item>映射配置的键</item>
    /// <item>执行计划缓存的索引</item>
    /// <item>类型映射查找</item>
    /// </list>
    /// 
    /// 作为值类型（struct），TypePair 具有以下优势：
    /// <list type="bullet">
    /// <item>无堆分配，减少 GC 压力</item>
    /// <item>高效的相等性比较</item>
    /// <item>适合作为字典键</item>
    /// </list>
    /// </remarks>
    public readonly struct TypePair : IEquatable<TypePair>
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// 获取目标类型
        /// <para>Get the destination type</para>
        /// </summary>
        public Type DestinationType { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建类型对实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <exception cref="ArgumentNullException">当 sourceType 或 destinationType 为 null 时抛出</exception>
        public TypePair(Type sourceType, Type destinationType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
        }

        #endregion

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建类型对
        /// <para>Create a type pair</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>类型对实例 / Type pair instance</returns>
        public static TypePair Create<TSource, TDestination>()
        {
            return new TypePair(typeof(TSource), typeof(TDestination));
        }

        /// <summary>
        /// 创建类型对
        /// <para>Create a type pair</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>类型对实例 / Type pair instance</returns>
        public static TypePair Create(Type sourceType, Type destinationType)
        {
            return new TypePair(sourceType, destinationType);
        }

        #endregion

        #region 相等性比较 / Equality

        /// <summary>
        /// 判断是否与另一个类型对相等
        /// <para>Determine if equal to another type pair</para>
        /// </summary>
        /// <param name="other">另一个类型对 / Another type pair</param>
        /// <returns>是否相等 / Whether equal</returns>
        public bool Equals(TypePair other)
        {
            return SourceType == other.SourceType && DestinationType == other.DestinationType;
        }

        /// <summary>
        /// 判断是否与另一个对象相等
        /// <para>Determine if equal to another object</para>
        /// </summary>
        /// <param name="obj">另一个对象 / Another object</param>
        /// <returns>是否相等 / Whether equal</returns>
        public override bool Equals(object obj)
        {
            return obj is TypePair other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// <para>Get hash code</para>
        /// </summary>
        /// <returns>哈希码 / Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // 使用质数进行哈希计算，减少冲突
                return (SourceType.GetHashCode() * 397) ^ DestinationType.GetHashCode();
            }
        }

        /// <summary>
        /// 相等运算符
        /// <para>Equality operator</para>
        /// </summary>
        public static bool operator ==(TypePair left, TypePair right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不等运算符
        /// <para>Inequality operator</para>
        /// </summary>
        public static bool operator !=(TypePair left, TypePair right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region 字符串表示 / String Representation

        /// <summary>
        /// 获取字符串表示
        /// <para>Get string representation</para>
        /// </summary>
        /// <returns>字符串表示 / String representation</returns>
        public override string ToString()
        {
            return $"{SourceType.Name} -> {DestinationType.Name}";
        }

        /// <summary>
        /// 获取完整的字符串表示（包含命名空间）
        /// <para>Get full string representation with namespace</para>
        /// </summary>
        /// <returns>完整字符串表示 / Full string representation</returns>
        public string ToFullString()
        {
            return $"{SourceType.FullName} -> {DestinationType.FullName}";
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 创建反向类型对
        /// <para>Create reverse type pair</para>
        /// </summary>
        /// <returns>反向类型对 / Reverse type pair</returns>
        public TypePair Reverse()
        {
            return new TypePair(DestinationType, SourceType);
        }

        /// <summary>
        /// 检查是否为相同类型映射
        /// <para>Check if mapping between same types</para>
        /// </summary>
        /// <returns>是否为相同类型 / Whether same types</returns>
        public bool IsSameType()
        {
            return SourceType == DestinationType;
        }

        /// <summary>
        /// 检查目标类型是否可从源类型赋值
        /// <para>Check if destination is assignable from source</para>
        /// </summary>
        /// <returns>是否可赋值 / Whether assignable</returns>
        public bool IsAssignable()
        {
            return DestinationType.IsAssignableFrom(SourceType);
        }

        #endregion
    }
}
