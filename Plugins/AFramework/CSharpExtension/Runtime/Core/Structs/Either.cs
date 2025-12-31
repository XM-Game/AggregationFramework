// ==========================================================
// 文件名：Either.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 二选一结构体
    /// <para>表示两种可能类型中的一种，常用于表示互斥的结果</para>
    /// <para>Left 通常表示错误/异常情况，Right 通常表示正常/成功情况</para>
    /// </summary>
    /// <typeparam name="TLeft">左值类型 (通常为错误类型)</typeparam>
    /// <typeparam name="TRight">右值类型 (通常为成功类型)</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 创建右值 (成功)
    /// Either&lt;string, int&gt; success = Either&lt;string, int&gt;.Right(42);
    /// 
    /// // 创建左值 (错误)
    /// Either&lt;string, int&gt; failure = Either&lt;string, int&gt;.Left("Error");
    /// 
    /// // 模式匹配
    /// string result = either.Match(
    ///     left => $"错误: {left}",
    ///     right => $"成功: {right}"
    /// );
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        #region 字段

        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isRight;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建左值 Either
        /// </summary>
        private Either(TLeft left, bool isLeft)
        {
            _left = left;
            _right = default;
            _isRight = !isLeft;
        }

        /// <summary>
        /// 创建右值 Either
        /// </summary>
        private Either(TRight right)
        {
            _left = default;
            _right = right;
            _isRight = true;
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建左值 Either
        /// </summary>
        /// <param name="value">左值</param>
        /// <returns>包含左值的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight> Left(TLeft value) => new Either<TLeft, TRight>(value, true);

        /// <summary>
        /// 创建右值 Either
        /// </summary>
        /// <param name="value">右值</param>
        /// <returns>包含右值的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight> Right(TRight value) => new Either<TLeft, TRight>(value);

        #endregion

        #region 属性

        /// <summary>是否为左值</summary>
        public bool IsLeft => !_isRight;

        /// <summary>是否为右值</summary>
        public bool IsRight => _isRight;

        /// <summary>
        /// 获取左值 (如果是右值则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当是右值时抛出</exception>
        public TLeft LeftValue
        {
            get
            {
                if (_isRight)
                    throw new InvalidOperationException("Either is Right, not Left.");
                return _left;
            }
        }

        /// <summary>
        /// 获取右值 (如果是左值则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当是左值时抛出</exception>
        public TRight RightValue
        {
            get
            {
                if (!_isRight)
                    throw new InvalidOperationException("Either is Left, not Right.");
                return _right;
            }
        }

        #endregion

        #region 值访问方法

        /// <summary>
        /// 尝试获取左值
        /// </summary>
        /// <param name="value">输出左值</param>
        /// <returns>如果是左值返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetLeft(out TLeft value)
        {
            value = _left;
            return !_isRight;
        }

        /// <summary>
        /// 尝试获取右值
        /// </summary>
        /// <param name="value">输出右值</param>
        /// <returns>如果是右值返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetRight(out TRight value)
        {
            value = _right;
            return _isRight;
        }

        /// <summary>
        /// 获取左值或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果是左值返回左值，否则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLeft GetLeftOrDefault(TLeft defaultValue = default)
        {
            return !_isRight ? _left : defaultValue;
        }

        /// <summary>
        /// 获取右值或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果是右值返回右值，否则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TRight GetRightOrDefault(TRight defaultValue = default)
        {
            return _isRight ? _right : defaultValue;
        }

        #endregion

        #region 函数式操作

        /// <summary>
        /// 匹配左值和右值的情况
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="left">左值处理函数</param>
        /// <param name="right">右值处理函数</param>
        /// <returns>处理结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
        {
            return _isRight ? right(_right) : left(_left);
        }

        /// <summary>
        /// 匹配左值和右值的情况 (无返回值)
        /// </summary>
        /// <param name="left">左值处理函数</param>
        /// <param name="right">右值处理函数</param>
        public void Match(Action<TLeft> left, Action<TRight> right)
        {
            if (_isRight)
                right(_right);
            else
                left(_left);
        }

        /// <summary>
        /// 映射右值到另一个类型
        /// </summary>
        /// <typeparam name="TNewRight">新右值类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<TLeft, TNewRight> Map<TNewRight>(Func<TRight, TNewRight> mapper)
        {
            return _isRight 
                ? Either<TLeft, TNewRight>.Right(mapper(_right)) 
                : Either<TLeft, TNewRight>.Left(_left);
        }

        /// <summary>
        /// 映射左值到另一个类型
        /// </summary>
        /// <typeparam name="TNewLeft">新左值类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapper)
        {
            return _isRight 
                ? Either<TNewLeft, TRight>.Right(_right) 
                : Either<TNewLeft, TRight>.Left(mapper(_left));
        }

        /// <summary>
        /// 双向映射
        /// </summary>
        /// <typeparam name="TNewLeft">新左值类型</typeparam>
        /// <typeparam name="TNewRight">新右值类型</typeparam>
        /// <param name="leftMapper">左值映射函数</param>
        /// <param name="rightMapper">右值映射函数</param>
        /// <returns>映射后的 Either</returns>
        public Either<TNewLeft, TNewRight> BiMap<TNewLeft, TNewRight>(
            Func<TLeft, TNewLeft> leftMapper, 
            Func<TRight, TNewRight> rightMapper)
        {
            return _isRight 
                ? Either<TNewLeft, TNewRight>.Right(rightMapper(_right)) 
                : Either<TNewLeft, TNewRight>.Left(leftMapper(_left));
        }

        /// <summary>
        /// 扁平映射右值
        /// </summary>
        /// <typeparam name="TNewRight">新右值类型</typeparam>
        /// <param name="mapper">映射函数</param>
        /// <returns>映射后的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<TLeft, TNewRight> FlatMap<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> mapper)
        {
            return _isRight ? mapper(_right) : Either<TLeft, TNewRight>.Left(_left);
        }

        /// <summary>
        /// 如果是右值则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Either<TLeft, TRight> IfRight(Action<TRight> action)
        {
            if (_isRight)
                action(_right);
            return this;
        }

        /// <summary>
        /// 如果是左值则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>返回自身以支持链式调用</returns>
        public Either<TLeft, TRight> IfLeft(Action<TLeft> action)
        {
            if (!_isRight)
                action(_left);
            return this;
        }

        /// <summary>
        /// 交换左右值
        /// </summary>
        /// <returns>交换后的 Either</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<TRight, TLeft> Swap()
        {
            return _isRight 
                ? Either<TRight, TLeft>.Left(_right) 
                : Either<TRight, TLeft>.Right(_left);
        }

        /// <summary>
        /// 转换为 Optional (仅保留右值)
        /// </summary>
        /// <returns>如果是右值返回 Some，否则返回 None</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TRight> ToOptional()
        {
            return _isRight ? Optional<TRight>.Some(_right) : Optional<TRight>.None;
        }

        /// <summary>
        /// 转换为 Result (左值作为错误信息)
        /// </summary>
        /// <returns>Result 实例</returns>
        public Result<TRight> ToResult()
        {
            return _isRight 
                ? Result<TRight>.Success(_right) 
                : Result<TRight>.Failure(_left?.ToString() ?? "Unknown error");
        }

        #endregion

        #region IEquatable 实现

        /// <summary>
        /// 判断是否与另一个 Either 相等
        /// </summary>
        public bool Equals(Either<TLeft, TRight> other)
        {
            if (_isRight != other._isRight)
                return false;
            if (_isRight)
                return EqualityComparer<TRight>.Default.Equals(_right, other._right);
            return EqualityComparer<TLeft>.Default.Equals(_left, other._left);
        }

        /// <summary>
        /// 判断是否与另一个对象相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Either<TLeft, TRight> other && Equals(other);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return _isRight 
                ? HashCode.Combine(true, _right) 
                : HashCode.Combine(false, _left);
        }

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right) 
            => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) 
            => !left.Equals(right);

        /// <summary>从右值隐式转换为 Either</summary>
        public static implicit operator Either<TLeft, TRight>(TRight value) => Right(value);

        #endregion

        #region 字符串表示

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return _isRight ? $"Right({_right})" : $"Left({_left})";
        }

        #endregion
    }

    #region 静态辅助类

    /// <summary>
    /// Either 静态辅助方法
    /// </summary>
    public static class Either
    {
        /// <summary>
        /// 创建左值 Either
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) 
            => Either<TLeft, TRight>.Left(value);

        /// <summary>
        /// 创建右值 Either
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) 
            => Either<TLeft, TRight>.Right(value);

        /// <summary>
        /// 尝试执行操作，成功返回右值，失败返回异常作为左值
        /// </summary>
        public static Either<Exception, T> Try<T>(Func<T> func)
        {
            try
            {
                return Either<Exception, T>.Right(func());
            }
            catch (Exception ex)
            {
                return Either<Exception, T>.Left(ex);
            }
        }
    }

    #endregion
}
