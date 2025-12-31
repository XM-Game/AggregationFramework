// ==========================================================
// 文件名：Result.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 结果结构体
    /// <para>表示操作的成功或失败结果，用于替代异常处理</para>
    /// <para>提供函数式编程风格的错误处理 API</para>
    /// </summary>
    /// <typeparam name="T">成功时的值类型</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 成功结果
    /// Result&lt;int&gt; success = Result&lt;int&gt;.Success(42);
    /// 
    /// // 失败结果
    /// Result&lt;int&gt; failure = Result&lt;int&gt;.Failure("操作失败");
    /// 
    /// // 模式匹配
    /// string message = result.Match(
    ///     value => $"成功: {value}",
    ///     error => $"失败: {error}"
    /// );
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        #region 字段

        private readonly T _value;
        private readonly string _error;
        private readonly bool _isSuccess;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建成功结果
        /// </summary>
        private Result(T value)
        {
            _value = value;
            _error = null;
            _isSuccess = true;
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        private Result(string error)
        {
            _value = default;
            _error = error ?? "Unknown error";
            _isSuccess = false;
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="value">成功值</param>
        /// <returns>成功的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Success(T value) => new Result<T>(value);

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns>失败的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Failure(string error) => new Result<T>(error);

        /// <summary>
        /// 从异常创建失败结果
        /// </summary>
        /// <param name="exception">异常</param>
        /// <returns>失败的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> FromException(Exception exception)
        {
            return new Result<T>(exception?.Message ?? "Unknown exception");
        }

        /// <summary>
        /// 尝试执行操作并返回结果
        /// </summary>
        /// <param name="func">要执行的操作</param>
        /// <returns>操作结果</returns>
        public static Result<T> Try(Func<T> func)
        {
            try
            {
                return Success(func());
            }
            catch (Exception ex)
            {
                return FromException(ex);
            }
        }

        #endregion

        #region 属性

        /// <summary>是否成功</summary>
        public bool IsSuccess => _isSuccess;

        /// <summary>是否失败</summary>
        public bool IsFailure => !_isSuccess;

        /// <summary>
        /// 获取成功值 (如果失败则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当结果为失败时抛出</exception>
        public T Value
        {
            get
            {
                if (!_isSuccess)
                    throw new InvalidOperationException($"Result is failure: {_error}");
                return _value;
            }
        }

        /// <summary>
        /// 获取错误信息 (如果成功则返回 null)
        /// </summary>
        public string Error => _error;

        #endregion

        #region 值访问方法

        /// <summary>
        /// 获取值或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果成功返回值，否则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(T defaultValue = default)
        {
            return _isSuccess ? _value : defaultValue;
        }

        /// <summary>
        /// 获取值或通过工厂方法创建默认值
        /// </summary>
        /// <param name="defaultFactory">默认值工厂方法</param>
        /// <returns>如果成功返回值，否则返回工厂方法创建的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(Func<T> defaultFactory)
        {
            return _isSuccess ? _value : defaultFactory();
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="value">输出值</param>
        /// <returns>如果成功返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(out T value)
        {
            value = _value;
            return _isSuccess;
        }

        /// <summary>
        /// 尝试获取错误信息
        /// </summary>
        /// <param name="error">输出错误信息</param>
        /// <returns>如果失败返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetError(out string error)
        {
            error = _error;
            return !_isSuccess;
        }

        #endregion

        #region 函数式操作

        /// <summary>
        /// 映射成功值到另一个类型
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return _isSuccess 
                ? Result<TResult>.Success(mapper(_value)) 
                : Result<TResult>.Failure(_error);
        }

        /// <summary>
        /// 扁平映射 (用于链式 Result 操作)
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<TResult> FlatMap<TResult>(Func<T, Result<TResult>> mapper)
        {
            return _isSuccess ? mapper(_value) : Result<TResult>.Failure(_error);
        }

        /// <summary>
        /// 映射错误信息
        /// </summary>
        /// <param name="mapper">错误映射函数</param>
        /// <returns>映射后的 Result</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> MapError(Func<string, string> mapper)
        {
            return _isSuccess ? this : Failure(mapper(_error));
        }

        /// <summary>
        /// 如果成功则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Result<T> OnSuccess(Action<T> action)
        {
            if (_isSuccess)
                action(_value);
            return this;
        }

        /// <summary>
        /// 如果失败则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Result<T> OnFailure(Action<string> action)
        {
            if (!_isSuccess)
                action(_error);
            return this;
        }

        /// <summary>
        /// 匹配成功和失败的情况
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Match<TResult>(Func<T, TResult> success, Func<string, TResult> failure)
        {
            return _isSuccess ? success(_value) : failure(_error);
        }

        /// <summary>
        /// 匹配成功和失败的情况 (无返回值)
        /// </summary>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        public void Match(Action<T> success, Action<string> failure)
        {
            if (_isSuccess)
                success(_value);
            else
                failure(_error);
        }

        /// <summary>
        /// 如果失败则返回备选值
        /// </summary>
        /// <param name="alternative">备选值</param>
        /// <returns>如果成功返回值，否则返回备选值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Or(T alternative)
        {
            return _isSuccess ? _value : alternative;
        }

        /// <summary>
        /// 如果失败则返回备选 Result
        /// </summary>
        /// <param name="alternative">备选 Result</param>
        /// <returns>如果成功返回自身，否则返回备选</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result<T> Or(Result<T> alternative)
        {
            return _isSuccess ? this : alternative;
        }

        /// <summary>
        /// 确保满足条件，否则返回失败
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="error">不满足条件时的错误信息</param>
        /// <returns>如果满足条件返回自身，否则返回失败</returns>
        public Result<T> Ensure(Func<T, bool> predicate, string error)
        {
            if (!_isSuccess)
                return this;
            return predicate(_value) ? this : Failure(error);
        }

        /// <summary>
        /// 转换为 Optional
        /// </summary>
        /// <returns>如果成功返回 Some，否则返回 None</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> ToOptional()
        {
            return _isSuccess ? Optional<T>.Some(_value) : Optional<T>.None;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否与另一个 Result 相等
        /// </summary>
        public bool Equals(Result<T> other)
        {
            if (_isSuccess != other._isSuccess)
                return false;
            if (_isSuccess)
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            return _error == other._error;
        }

        /// <summary>
        /// 判断是否与另一个对象相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Result<T> other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return _isSuccess 
                ? HashCode.Combine(true, _value) 
                : HashCode.Combine(false, _error);
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

        /// <summary>从值隐式转换为成功 Result</summary>
        public static implicit operator Result<T>(T value) => Success(value);

        /// <summary>布尔转换 (用于 if 语句)</summary>
        public static bool operator true(Result<T> result) => result._isSuccess;

        /// <summary>布尔转换 (用于 if 语句)</summary>
        public static bool operator false(Result<T> result) => !result._isSuccess;

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return _isSuccess ? $"Success({_value})" : $"Failure({_error})";
        }

        #endregion
    }
}
