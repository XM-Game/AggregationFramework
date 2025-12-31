// ==========================================================
// 文件名：Range.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 泛型范围结构体
    /// <para>表示一个值的范围 [Min, Max]</para>
    /// <para>支持范围检查、随机值生成、插值等操作</para>
    /// </summary>
    /// <typeparam name="T">值类型，必须实现 IComparable</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 创建整数范围
    /// var intRange = new Range&lt;int&gt;(0, 100);
    /// bool inRange = intRange.Contains(50); // true
    /// 
    /// // 创建浮点数范围
    /// var floatRange = Range&lt;float&gt;.Create(0f, 1f);
    /// float clamped = floatRange.Clamp(1.5f); // 1.0f
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        #region 字段

        /// <summary>范围最小值</summary>
        public readonly T Min;

        /// <summary>范围最大值</summary>
        public readonly T Max;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建范围
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public Range(T min, T max)
        {
            // 确保 min <= max
            if (min.CompareTo(max) > 0)
            {
                Min = max;
                Max = min;
            }
            else
            {
                Min = min;
                Max = max;
            }
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建范围
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>范围实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Range<T> Create(T min, T max) => new Range<T>(min, max);

        /// <summary>
        /// 创建单值范围 (min == max)
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>范围实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Range<T> Single(T value) => new Range<T>(value, value);

        #endregion

        #region 范围检查方法

        /// <summary>
        /// 检查值是否在范围内 (包含边界)
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值在范围内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value)
        {
            return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
        }

        /// <summary>
        /// 检查值是否在范围内 (不包含边界)
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值在范围内返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsExclusive(T value)
        {
            return value.CompareTo(Min) > 0 && value.CompareTo(Max) < 0;
        }

        /// <summary>
        /// 检查值是否小于范围最小值
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值小于最小值返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBelowMin(T value) => value.CompareTo(Min) < 0;

        /// <summary>
        /// 检查值是否大于范围最大值
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值大于最大值返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAboveMax(T value) => value.CompareTo(Max) > 0;

        /// <summary>
        /// 检查另一个范围是否完全包含在此范围内
        /// </summary>
        /// <param name="other">另一个范围</param>
        /// <returns>如果完全包含返回 true</returns>
        public bool Contains(Range<T> other)
        {
            return other.Min.CompareTo(Min) >= 0 && other.Max.CompareTo(Max) <= 0;
        }

        /// <summary>
        /// 检查是否与另一个范围重叠
        /// </summary>
        /// <param name="other">另一个范围</param>
        /// <returns>如果重叠返回 true</returns>
        public bool Overlaps(Range<T> other)
        {
            return Min.CompareTo(other.Max) <= 0 && Max.CompareTo(other.Min) >= 0;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 检查范围是否为单值 (min == max)
        /// </summary>
        public bool IsSingleValue => Min.CompareTo(Max) == 0;

        /// <summary>
        /// 检查范围是否有效 (min &lt;= max)
        /// </summary>
        public bool IsValid => Min.CompareTo(Max) <= 0;

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否与另一个范围相等
        /// </summary>
        public bool Equals(Range<T> other)
        {
            return Min.CompareTo(other.Min) == 0 && Max.CompareTo(other.Max) == 0;
        }

        /// <summary>
        /// 判断是否与另一个对象相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Range<T> other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(Range<T> left, Range<T> right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(Range<T> left, Range<T> right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString() => $"[{Min}, {Max}]";

        #endregion

        #region 解构

        /// <summary>
        /// 解构为最小值和最大值
        /// </summary>
        public void Deconstruct(out T min, out T max)
        {
            min = Min;
            max = Max;
        }

        #endregion
    }


    #region 特化范围类型

    /// <summary>
    /// 整数范围结构体
    /// <para>提供整数特有的范围操作</para>
    /// </summary>
    [Serializable]
    public readonly struct IntRange : IEquatable<IntRange>
    {
        /// <summary>范围最小值</summary>
        public readonly int Min;

        /// <summary>范围最大值</summary>
        public readonly int Max;

        /// <summary>
        /// 创建整数范围
        /// </summary>
        public IntRange(int min, int max)
        {
            if (min > max)
            {
                Min = max;
                Max = min;
            }
            else
            {
                Min = min;
                Max = max;
            }
        }

        /// <summary>范围长度 (包含边界)</summary>
        public int Length => Max - Min + 1;

        /// <summary>范围跨度 (不包含边界)</summary>
        public int Span => Max - Min;

        /// <summary>范围中点</summary>
        public int Center => (Min + Max) / 2;

        /// <summary>检查值是否在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int value) => value >= Min && value <= Max;

        /// <summary>将值限制在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Clamp(int value)
        {
            if (value < Min) return Min;
            if (value > Max) return Max;
            return value;
        }

        /// <summary>获取范围内的随机值</summary>
        public int Random(Random random) => random.Next(Min, Max + 1);

        /// <summary>线性插值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Lerp(float t) => Min + (int)((Max - Min) * t);

        /// <summary>反向线性插值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float InverseLerp(int value)
        {
            if (Max == Min) return 0f;
            return (float)(value - Min) / (Max - Min);
        }

        public bool Equals(IntRange other) => Min == other.Min && Max == other.Max;
        public override bool Equals(object obj) => obj is IntRange other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Min, Max);
        public override string ToString() => $"[{Min}, {Max}]";

        public static bool operator ==(IntRange left, IntRange right) => left.Equals(right);
        public static bool operator !=(IntRange left, IntRange right) => !left.Equals(right);

        public void Deconstruct(out int min, out int max) { min = Min; max = Max; }

        /// <summary>隐式转换为 Range&lt;int&gt;</summary>
        public static implicit operator Range<int>(IntRange range) => new Range<int>(range.Min, range.Max);
    }

    /// <summary>
    /// 浮点数范围结构体
    /// <para>提供浮点数特有的范围操作</para>
    /// </summary>
    [Serializable]
    public readonly struct FloatRange : IEquatable<FloatRange>
    {
        /// <summary>范围最小值</summary>
        public readonly float Min;

        /// <summary>范围最大值</summary>
        public readonly float Max;

        /// <summary>
        /// 创建浮点数范围
        /// </summary>
        public FloatRange(float min, float max)
        {
            if (min > max)
            {
                Min = max;
                Max = min;
            }
            else
            {
                Min = min;
                Max = max;
            }
        }

        /// <summary>范围跨度</summary>
        public float Span => Max - Min;

        /// <summary>范围中点</summary>
        public float Center => (Min + Max) * 0.5f;

        /// <summary>检查值是否在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float value) => value >= Min && value <= Max;

        /// <summary>将值限制在范围内</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Clamp(float value)
        {
            if (value < Min) return Min;
            if (value > Max) return Max;
            return value;
        }

        /// <summary>获取范围内的随机值</summary>
        public float Random(Random random) => Min + (float)random.NextDouble() * Span;

        /// <summary>线性插值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Lerp(float t) => Min + (Max - Min) * t;

        /// <summary>反向线性插值</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float InverseLerp(float value)
        {
            if (Math.Abs(Max - Min) < float.Epsilon) return 0f;
            return (value - Min) / (Max - Min);
        }

        /// <summary>将值从此范围映射到另一个范围</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Remap(float value, FloatRange target)
        {
            float t = InverseLerp(value);
            return target.Lerp(t);
        }

        public bool Equals(FloatRange other) => 
            Math.Abs(Min - other.Min) < float.Epsilon && 
            Math.Abs(Max - other.Max) < float.Epsilon;
        public override bool Equals(object obj) => obj is FloatRange other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Min, Max);
        public override string ToString() => $"[{Min:F2}, {Max:F2}]";

        public static bool operator ==(FloatRange left, FloatRange right) => left.Equals(right);
        public static bool operator !=(FloatRange left, FloatRange right) => !left.Equals(right);

        public void Deconstruct(out float min, out float max) { min = Min; max = Max; }

        /// <summary>隐式转换为 Range&lt;float&gt;</summary>
        public static implicit operator Range<float>(FloatRange range) => new Range<float>(range.Min, range.Max);

        /// <summary>0 到 1 的范围</summary>
        public static readonly FloatRange ZeroToOne = new FloatRange(0f, 1f);

        /// <summary>-1 到 1 的范围</summary>
        public static readonly FloatRange NegativeOneToOne = new FloatRange(-1f, 1f);

        /// <summary>0 到 360 的角度范围</summary>
        public static readonly FloatRange Degrees = new FloatRange(0f, 360f);

        /// <summary>0 到 2π 的弧度范围</summary>
        public static readonly FloatRange Radians = new FloatRange(0f, (float)(2 * Math.PI));
    }

    #endregion
}
