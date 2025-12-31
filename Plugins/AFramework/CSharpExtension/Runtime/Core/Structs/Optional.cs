// ==========================================================
// 文件名：Optional.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 可选值结构体
    /// <para>表示一个可能存在或不存在的值，类似于 Nullable 但适用于引用类型</para>
    /// <para>提供函数式编程风格的 API (Map, FlatMap, Filter 等)</para>
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 创建有值的 Optional
    /// var some = Optional&lt;string&gt;.Some("Hello");
    /// 
    /// // 创建无值的 Optional
    /// var none = Optional&lt;string&gt;.None;
    /// 
    /// // 安全访问值
    /// string result = some.GetValueOrDefault("Default");
    /// 
    /// // 函数式操作
    /// var length = some.Map(s => s.Length).GetValueOrDefault(0);
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct Optional<T> : IEquatable<Optional<T>>
    {
        #region 字段

        private readonly T _value;
        private readonly bool _hasValue;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建有值的 Optional
        /// </summary>
        /// <param name="value">值</param>
        private Optional(T value)
        {
            _value = value;
            _hasValue = value != null;
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建有值的 Optional
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>Optional 实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some(T value) => new Optional<T>(value);

        /// <summary>
        /// 创建无值的 Optional
        /// </summary>
        public static Optional<T> None => default;

        /// <summary>
        /// 从可能为 null 的值创建 Optional
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>如果值不为 null 返回 Some，否则返回 None</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> FromNullable(T value)
        {
            return value != null ? Some(value) : None;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue => _hasValue;

        /// <summary>
        /// 是否无值
        /// </summary>
        public bool IsNone => !_hasValue;

        /// <summary>
        /// 获取值 (如果无值则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当无值时抛出</exception>
        public T Value
        {
            get
            {
                if (!_hasValue)
                    throw new InvalidOperationException("Optional has no value.");
                return _value;
            }
        }

        #endregion

        #region 值访问方法

        /// <summary>
        /// 获取值或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果有值返回值，否则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(T defaultValue = default)
        {
            return _hasValue ? _value : defaultValue;
        }

        /// <summary>
        /// 获取值或通过工厂方法创建默认值
        /// </summary>
        /// <param name="defaultFactory">默认值工厂方法</param>
        /// <returns>如果有值返回值，否则返回工厂方法创建的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(Func<T> defaultFactory)
        {
            return _hasValue ? _value : defaultFactory();
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="value">输出值</param>
        /// <returns>如果有值返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(out T value)
        {
            value = _value;
            return _hasValue;
        }

        #endregion

        #region 函数式操作

        /// <summary>
        /// 映射值到另一个类型
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Optional</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return _hasValue 
                ? Optional<TResult>.FromNullable(mapper(_value)) 
                : Optional<TResult>.None;
        }

        /// <summary>
        /// 扁平映射 (用于链式 Optional 操作)
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Optional</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TResult> FlatMap<TResult>(Func<T, Optional<TResult>> mapper)
        {
            return _hasValue ? mapper(_value) : Optional<TResult>.None;
        }

        /// <summary>
        /// 过滤值
        /// </summary>
        /// <param name="predicate">过滤条件</param>
        /// <returns>如果满足条件返回原 Optional，否则返回 None</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Filter(Func<T, bool> predicate)
        {
            return _hasValue && predicate(_value) ? this : None;
        }

        /// <summary>
        /// 如果有值则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Optional<T> IfPresent(Action<T> action)
        {
            if (_hasValue)
                action(_value);
            return this;
        }

        /// <summary>
        /// 如果无值则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Optional<T> IfAbsent(Action action)
        {
            if (!_hasValue)
                action();
            return this;
        }

        /// <summary>
        /// 匹配有值和无值的情况
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="some">有值时的处理函数</param>
        /// <param name="none">无值时的处理函数</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            return _hasValue ? some(_value) : none();
        }

        /// <summary>
        /// 匹配有值和无值的情况 (无返回值)
        /// </summary>
        /// <param name="some">有值时的处理函数</param>
        /// <param name="none">无值时的处理函数</param>
        public void Match(Action<T> some, Action none)
        {
            if (_hasValue)
                some(_value);
            else
                none();
        }

        /// <summary>
        /// 如果无值则返回备选 Optional
        /// </summary>
        /// <param name="alternative">备选 Optional</param>
        /// <returns>如果有值返回自身，否则返回备选</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Or(Optional<T> alternative)
        {
            return _hasValue ? this : alternative;
        }

        /// <summary>
        /// 如果无值则通过工厂方法创建备选 Optional
        /// </summary>
        /// <param name="alternativeFactory">备选 Optional 工厂方法</param>
        /// <returns>如果有值返回自身，否则返回工厂方法创建的 Optional</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Or(Func<Optional<T>> alternativeFactory)
        {
            return _hasValue ? this : alternativeFactory();
        }

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否与另一个 Optional 相等
        /// </summary>
        public bool Equals(Optional<T> other)
        {
            if (!_hasValue && !other._hasValue)
                return true;
            if (_hasValue != other._hasValue)
                return false;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <summary>
        /// 判断是否与另一个对象相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Optional<T> other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return _hasValue ? _value?.GetHashCode() ?? 0 : 0;
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);

        /// <summary>从值隐式转换为 Optional</summary>
        public static implicit operator Optional<T>(T value) => FromNullable(value);

        /// <summary>布尔转换 (用于 if 语句)</summary>
        public static bool operator true(Optional<T> optional) => optional._hasValue;

        /// <summary>布尔转换 (用于 if 语句)</summary>
        public static bool operator false(Optional<T> optional) => !optional._hasValue;

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return _hasValue ? $"Some({_value})" : "None";
        }

        #endregion
    }

    #region 静态辅助类

    /// <summary>
    /// Optional 静态辅助方法
    /// </summary>
    public static class Optional
    {
        /// <summary>
        /// 创建有值的 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some<T>(T value) => Optional<T>.Some(value);

        /// <summary>
        /// 创建无值的 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> None<T>() => Optional<T>.None;

        /// <summary>
        /// 从可能为 null 的值创建 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> FromNullable<T>(T value) => Optional<T>.FromNullable(value);

        /// <summary>
        /// 从 Nullable 值类型创建 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> FromNullable<T>(T? value) where T : struct
        {
            return value.HasValue ? Optional<T>.Some(value.Value) : Optional<T>.None;
        }
    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// Optional 扩展方法
    /// </summary>
    public static class OptionalExtensions
    {
        /// <summary>
        /// 将值转换为 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> ToOptional<T>(this T value) => Optional<T>.FromNullable(value);

        /// <summary>
        /// 将 Nullable 转换为 Optional
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> ToOptional<T>(this T? value) where T : struct
        {
            return value.HasValue ? Optional<T>.Some(value.Value) : Optional<T>.None;
        }
    }

    #endregion

    /// <summary>
    /// EqualityComparer 辅助类 (用于泛型比较)
    /// </summary>
    internal static class EqualityComparer<T>
    {
        public static readonly System.Collections.Generic.EqualityComparer<T> Default = 
            System.Collections.Generic.EqualityComparer<T>.Default;
    }
}
