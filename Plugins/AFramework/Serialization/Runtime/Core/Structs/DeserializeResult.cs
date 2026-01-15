// ==========================================================
// 文件名：DeserializeResult.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 反序列化结果结构体
    /// <para>封装反序列化操作的结果，包含成功/失败状态、值和统计信息</para>
    /// <para>提供函数式 API 进行结果处理</para>
    /// </summary>
    /// <typeparam name="T">反序列化的目标类型</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 检查结果
    /// var result = serializer.Deserialize&lt;Player&gt;(data);
    /// if (result.IsSuccess)
    /// {
    ///     Player player = result.Value;
    ///     Console.WriteLine($"反序列化成功，读取: {result.BytesRead} 字节");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"反序列化失败: {result.ErrorMessage}");
    /// }
    /// 
    /// // 函数式处理
    /// var player = result.GetValueOrDefault(Player.Default);
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct DeserializeResult<T> : IEquatable<DeserializeResult<T>>
    {
        #region 字段

        private readonly T _value;
        private readonly SerializeErrorCode _errorCode;
        private readonly string _errorMessage;
        private readonly DeserializeStatistics _statistics;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建成功结果
        /// </summary>
        private DeserializeResult(T value, DeserializeStatistics statistics)
        {
            _value = value;
            _errorCode = SerializeErrorCode.Success;
            _errorMessage = null;
            _statistics = statistics;
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        private DeserializeResult(SerializeErrorCode errorCode, string errorMessage)
        {
            _value = default;
            _errorCode = errorCode;
            _errorMessage = errorMessage ?? errorCode.GetDescription();
            _statistics = default;
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="value">反序列化的值</param>
        /// <param name="statistics">统计信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> Success(T value, DeserializeStatistics statistics = default)
        {
            return new DeserializeResult<T>(value, statistics);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="errorMessage">错误信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> Failure(SerializeErrorCode errorCode, string errorMessage = null)
        {
            return new DeserializeResult<T>(errorCode, errorMessage);
        }

        /// <summary>
        /// 从异常创建失败结果
        /// </summary>
        /// <param name="exception">异常</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> FromException(Exception exception)
        {
            var errorCode = exception switch
            {
                ArgumentNullException => SerializeErrorCode.NullReference,
                ArgumentException => SerializeErrorCode.InvalidArgument,
                NotSupportedException => SerializeErrorCode.NotSupported,
                InvalidOperationException => SerializeErrorCode.InvalidOperation,
                TimeoutException => SerializeErrorCode.Timeout,
                OutOfMemoryException => SerializeErrorCode.OutOfMemory,
                System.IO.EndOfStreamException => SerializeErrorCode.UnexpectedEndOfData,
                _ => SerializeErrorCode.InternalError
            };
            return new DeserializeResult<T>(errorCode, exception.Message);
        }

        #endregion

        #region 属性

        /// <summary>是否成功</summary>
        public bool IsSuccess => _errorCode == SerializeErrorCode.Success;

        /// <summary>是否失败</summary>
        public bool IsFailure => _errorCode != SerializeErrorCode.Success;

        /// <summary>错误码</summary>
        public SerializeErrorCode ErrorCode => _errorCode;

        /// <summary>错误信息</summary>
        public string ErrorMessage => _errorMessage;

        /// <summary>统计信息</summary>
        public DeserializeStatistics Statistics => _statistics;

        /// <summary>
        /// 反序列化的值 (如果失败则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当结果为失败时抛出</exception>
        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException($"反序列化失败: {_errorMessage}");
                return _value;
            }
        }

        /// <summary>读取的字节数</summary>
        public int BytesRead => _statistics.BytesRead;

        #endregion

        #region 值访问方法

        /// <summary>
        /// 获取值或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(T defaultValue = default)
        {
            return IsSuccess ? _value : defaultValue;
        }

        /// <summary>
        /// 获取值或通过工厂方法创建默认值
        /// </summary>
        /// <param name="defaultFactory">默认值工厂方法</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(Func<T> defaultFactory)
        {
            return IsSuccess ? _value : defaultFactory();
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="value">输出值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(out T value)
        {
            value = _value;
            return IsSuccess;
        }

        /// <summary>
        /// 尝试获取错误信息
        /// </summary>
        /// <param name="errorCode">输出错误码</param>
        /// <param name="errorMessage">输出错误信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetError(out SerializeErrorCode errorCode, out string errorMessage)
        {
            errorCode = _errorCode;
            errorMessage = _errorMessage;
            return IsFailure;
        }

        #endregion

        #region 函数式操作

        /// <summary>
        /// 映射成功值到另一个类型
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DeserializeResult<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return IsSuccess
                ? DeserializeResult<TResult>.Success(mapper(_value), _statistics)
                : DeserializeResult<TResult>.Failure(_errorCode, _errorMessage);
        }

        /// <summary>
        /// 扁平映射
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DeserializeResult<TResult> FlatMap<TResult>(Func<T, DeserializeResult<TResult>> mapper)
        {
            return IsSuccess ? mapper(_value) : DeserializeResult<TResult>.Failure(_errorCode, _errorMessage);
        }

        /// <summary>
        /// 如果成功则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public DeserializeResult<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess)
                action(_value);
            return this;
        }

        /// <summary>
        /// 如果失败则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public DeserializeResult<T> OnFailure(Action<SerializeErrorCode, string> action)
        {
            if (IsFailure)
                action(_errorCode, _errorMessage);
            return this;
        }

        /// <summary>
        /// 匹配成功和失败的情况
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Match<TResult>(Func<T, TResult> success, Func<SerializeErrorCode, string, TResult> failure)
        {
            return IsSuccess ? success(_value) : failure(_errorCode, _errorMessage);
        }

        /// <summary>
        /// 匹配成功和失败的情况 (无返回值)
        /// </summary>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        public void Match(Action<T> success, Action<SerializeErrorCode, string> failure)
        {
            if (IsSuccess)
                success(_value);
            else
                failure(_errorCode, _errorMessage);
        }

        /// <summary>
        /// 如果失败则返回备选值
        /// </summary>
        /// <param name="alternative">备选值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Or(T alternative)
        {
            return IsSuccess ? _value : alternative;
        }

        /// <summary>
        /// 如果失败则返回备选结果
        /// </summary>
        /// <param name="alternative">备选结果</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DeserializeResult<T> Or(DeserializeResult<T> alternative)
        {
            return IsSuccess ? this : alternative;
        }

        /// <summary>
        /// 如果失败则抛出异常
        /// </summary>
        /// <exception cref="InvalidOperationException">当结果为失败时抛出</exception>
        public DeserializeResult<T> ThrowIfFailure()
        {
            if (IsFailure)
                throw _errorCode.ToException(_errorMessage);
            return this;
        }

        /// <summary>
        /// 确保满足条件
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="errorCode">不满足条件时的错误码</param>
        /// <param name="errorMessage">不满足条件时的错误信息</param>
        public DeserializeResult<T> Ensure(Func<T, bool> predicate, SerializeErrorCode errorCode, string errorMessage = null)
        {
            if (!IsSuccess)
                return this;
            return predicate(_value) ? this : Failure(errorCode, errorMessage);
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(DeserializeResult<T> other)
        {
            if (_errorCode != other._errorCode)
                return false;
            if (IsFailure)
                return _errorMessage == other._errorMessage;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is DeserializeResult<T> other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            return IsSuccess
                ? HashCode.Combine(true, _value)
                : HashCode.Combine(false, _errorCode, _errorMessage);
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(DeserializeResult<T> left, DeserializeResult<T> right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(DeserializeResult<T> left, DeserializeResult<T> right) => !left.Equals(right);

        /// <summary>布尔转换</summary>
        public static bool operator true(DeserializeResult<T> result) => result.IsSuccess;

        /// <summary>布尔转换</summary>
        public static bool operator false(DeserializeResult<T> result) => result.IsFailure;

        /// <summary>从值隐式转换为成功结果</summary>
        public static implicit operator DeserializeResult<T>(T value) => Success(value);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return IsSuccess
                ? $"DeserializeResult<{typeof(T).Name}>.Success({_value})"
                : $"DeserializeResult<{typeof(T).Name}>.Failure({_errorCode}: {_errorMessage})";
        }

        #endregion
    }

    #region 非泛型辅助类

    /// <summary>
    /// 反序列化结果静态辅助方法
    /// </summary>
    public static class DeserializeResult
    {
        /// <summary>
        /// 创建成功结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> Success<T>(T value, DeserializeStatistics statistics = default)
        {
            return DeserializeResult<T>.Success(value, statistics);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> Failure<T>(SerializeErrorCode errorCode, string errorMessage = null)
        {
            return DeserializeResult<T>.Failure(errorCode, errorMessage);
        }

        /// <summary>
        /// 从异常创建失败结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeResult<T> FromException<T>(Exception exception)
        {
            return DeserializeResult<T>.FromException(exception);
        }
    }

    #endregion

    #region 反序列化统计信息

    /// <summary>
    /// 反序列化统计信息结构体
    /// <para>记录反序列化过程的性能和数据统计</para>
    /// </summary>
    [Serializable]
    public readonly struct DeserializeStatistics : IEquatable<DeserializeStatistics>
    {
        #region 字段

        private readonly long _elapsedTicks;
        private readonly int _bytesRead;
        private readonly int _bytesAfterDecompression;
        private readonly int _objectCount;
        private readonly int _maxDepth;
        private readonly int _stringCount;
        private readonly int _collectionCount;
        private readonly int _skippedFields;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建统计信息
        /// </summary>
        public DeserializeStatistics(
            long elapsedTicks,
            int bytesRead,
            int bytesAfterDecompression,
            int objectCount,
            int maxDepth,
            int stringCount,
            int collectionCount,
            int skippedFields)
        {
            _elapsedTicks = elapsedTicks;
            _bytesRead = bytesRead;
            _bytesAfterDecompression = bytesAfterDecompression;
            _objectCount = objectCount;
            _maxDepth = maxDepth;
            _stringCount = stringCount;
            _collectionCount = collectionCount;
            _skippedFields = skippedFields;
        }

        #endregion

        #region 属性

        /// <summary>耗时 (Ticks)</summary>
        public long ElapsedTicks => _elapsedTicks;

        /// <summary>耗时 (毫秒)</summary>
        public double ElapsedMilliseconds => _elapsedTicks / (double)TimeSpan.TicksPerMillisecond;

        /// <summary>读取字节数</summary>
        public int BytesRead => _bytesRead;

        /// <summary>解压后字节数</summary>
        public int BytesAfterDecompression => _bytesAfterDecompression;

        /// <summary>对象数量</summary>
        public int ObjectCount => _objectCount;

        /// <summary>最大深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>字符串数量</summary>
        public int StringCount => _stringCount;

        /// <summary>集合数量</summary>
        public int CollectionCount => _collectionCount;

        /// <summary>跳过的字段数量</summary>
        public int SkippedFields => _skippedFields;

        /// <summary>吞吐量 (字节/秒)</summary>
        public double ThroughputBytesPerSecond => ElapsedMilliseconds > 0
            ? _bytesRead / ElapsedMilliseconds * 1000
            : 0;

        #endregion

        #region 工厂方法

        /// <summary>空统计信息</summary>
        public static DeserializeStatistics Empty => default;

        /// <summary>
        /// 创建统计信息
        /// </summary>
        public static DeserializeStatistics Create(
            long elapsedTicks,
            int bytesRead,
            int bytesAfterDecompression = 0,
            int objectCount = 0,
            int maxDepth = 0,
            int stringCount = 0,
            int collectionCount = 0,
            int skippedFields = 0)
        {
            return new DeserializeStatistics(elapsedTicks, bytesRead,
                bytesAfterDecompression > 0 ? bytesAfterDecompression : bytesRead,
                objectCount, maxDepth, stringCount, collectionCount, skippedFields);
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(DeserializeStatistics other)
        {
            return _elapsedTicks == other._elapsedTicks &&
                   _bytesRead == other._bytesRead &&
                   _objectCount == other._objectCount;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is DeserializeStatistics other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_elapsedTicks, _bytesRead, _objectCount);

        /// <summary>相等运算符</summary>
        public static bool operator ==(DeserializeStatistics left, DeserializeStatistics right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(DeserializeStatistics left, DeserializeStatistics right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"Statistics(Bytes={_bytesRead}, Time={ElapsedMilliseconds:F2}ms, Objects={_objectCount})";
        }

        #endregion
    }

    #endregion

    #region EqualityComparer 辅助类

    /// <summary>
    /// EqualityComparer 辅助类
    /// </summary>
    internal static class EqualityComparer<T>
    {
        public static readonly System.Collections.Generic.EqualityComparer<T> Default =
            System.Collections.Generic.EqualityComparer<T>.Default;
    }

    #endregion
}
