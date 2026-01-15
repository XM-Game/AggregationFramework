// ==========================================================
// 文件名：SerializeResult.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化结果结构体
    /// <para>封装序列化操作的结果，包含成功/失败状态、数据和统计信息</para>
    /// <para>提供函数式 API 进行结果处理</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 检查结果
    /// var result = serializer.Serialize(obj);
    /// if (result.IsSuccess)
    /// {
    ///     byte[] data = result.Data;
    ///     Console.WriteLine($"序列化成功，大小: {result.BytesWritten}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"序列化失败: {result.ErrorMessage}");
    /// }
    /// 
    /// // 函数式处理
    /// result.Match(
    ///     success => SaveToFile(success.Data),
    ///     failure => LogError(failure.ErrorMessage)
    /// );
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct SerializeResult : IEquatable<SerializeResult>
    {
        #region 字段

        private readonly byte[] _data;
        private readonly SerializeErrorCode _errorCode;
        private readonly string _errorMessage;
        private readonly SerializeStatistics _statistics;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建成功结果
        /// </summary>
        private SerializeResult(byte[] data, SerializeStatistics statistics)
        {
            _data = data;
            _errorCode = SerializeErrorCode.Success;
            _errorMessage = null;
            _statistics = statistics;
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        private SerializeResult(SerializeErrorCode errorCode, string errorMessage)
        {
            _data = null;
            _errorCode = errorCode;
            _errorMessage = errorMessage ?? errorCode.GetDescription();
            _statistics = default;
        }

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="statistics">统计信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeResult Success(byte[] data, SerializeStatistics statistics = default)
        {
            return new SerializeResult(data, statistics);
        }

        /// <summary>
        /// 创建成功结果 (从 Span)
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="statistics">统计信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeResult Success(ReadOnlySpan<byte> data, SerializeStatistics statistics = default)
        {
            return new SerializeResult(data.ToArray(), statistics);
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="errorMessage">错误信息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeResult Failure(SerializeErrorCode errorCode, string errorMessage = null)
        {
            return new SerializeResult(errorCode, errorMessage);
        }

        /// <summary>
        /// 从异常创建失败结果
        /// </summary>
        /// <param name="exception">异常</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeResult FromException(Exception exception)
        {
            var errorCode = exception switch
            {
                ArgumentNullException => SerializeErrorCode.NullReference,
                ArgumentException => SerializeErrorCode.InvalidArgument,
                NotSupportedException => SerializeErrorCode.NotSupported,
                InvalidOperationException => SerializeErrorCode.InvalidOperation,
                TimeoutException => SerializeErrorCode.Timeout,
                OutOfMemoryException => SerializeErrorCode.OutOfMemory,
                _ => SerializeErrorCode.InternalError
            };
            return new SerializeResult(errorCode, exception.Message);
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
        public SerializeStatistics Statistics => _statistics;

        /// <summary>
        /// 序列化数据 (如果失败则抛出异常)
        /// </summary>
        /// <exception cref="InvalidOperationException">当结果为失败时抛出</exception>
        public byte[] Data
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException($"序列化失败: {_errorMessage}");
                return _data;
            }
        }

        /// <summary>
        /// 序列化数据 (只读 Span)
        /// </summary>
        public ReadOnlySpan<byte> DataSpan => _data ?? ReadOnlySpan<byte>.Empty;

        /// <summary>写入的字节数</summary>
        public int BytesWritten => _data?.Length ?? 0;

        #endregion

        #region 值访问方法

        /// <summary>
        /// 获取数据或默认值
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetDataOrDefault(byte[] defaultValue = null)
        {
            return IsSuccess ? _data : defaultValue;
        }

        /// <summary>
        /// 尝试获取数据
        /// </summary>
        /// <param name="data">输出数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetData(out byte[] data)
        {
            data = _data;
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
        /// 映射成功数据
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="mapper">映射函数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Map<T>(Func<byte[], T> mapper)
        {
            return IsSuccess ? mapper(_data) : default;
        }

        /// <summary>
        /// 如果成功则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public SerializeResult OnSuccess(Action<byte[]> action)
        {
            if (IsSuccess)
                action(_data);
            return this;
        }

        /// <summary>
        /// 如果失败则执行操作
        /// </summary>
        /// <param name="action">要执行的操作</param>
        public SerializeResult OnFailure(Action<SerializeErrorCode, string> action)
        {
            if (IsFailure)
                action(_errorCode, _errorMessage);
            return this;
        }

        /// <summary>
        /// 匹配成功和失败的情况
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Match<T>(Func<SerializeResult, T> success, Func<SerializeResult, T> failure)
        {
            return IsSuccess ? success(this) : failure(this);
        }

        /// <summary>
        /// 匹配成功和失败的情况 (无返回值)
        /// </summary>
        /// <param name="success">成功时的处理函数</param>
        /// <param name="failure">失败时的处理函数</param>
        public void Match(Action<SerializeResult> success, Action<SerializeResult> failure)
        {
            if (IsSuccess)
                success(this);
            else
                failure(this);
        }

        /// <summary>
        /// 如果失败则抛出异常
        /// </summary>
        /// <exception cref="InvalidOperationException">当结果为失败时抛出</exception>
        public SerializeResult ThrowIfFailure()
        {
            if (IsFailure)
                throw _errorCode.ToException(_errorMessage);
            return this;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeResult other)
        {
            if (_errorCode != other._errorCode)
                return false;
            if (IsFailure)
                return _errorMessage == other._errorMessage;
            return BytesWritten == other.BytesWritten;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeResult other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            return IsSuccess
                ? HashCode.Combine(true, BytesWritten)
                : HashCode.Combine(false, _errorCode, _errorMessage);
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeResult left, SerializeResult right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeResult left, SerializeResult right) => !left.Equals(right);

        /// <summary>布尔转换</summary>
        public static bool operator true(SerializeResult result) => result.IsSuccess;

        /// <summary>布尔转换</summary>
        public static bool operator false(SerializeResult result) => result.IsFailure;

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return IsSuccess
                ? $"SerializeResult.Success(Bytes={BytesWritten})"
                : $"SerializeResult.Failure({_errorCode}: {_errorMessage})";
        }

        #endregion
    }

    #region 序列化统计信息

    /// <summary>
    /// 序列化统计信息结构体
    /// <para>记录序列化过程的性能和数据统计</para>
    /// </summary>
    [Serializable]
    public readonly struct SerializeStatistics : IEquatable<SerializeStatistics>
    {
        #region 字段

        private readonly long _elapsedTicks;
        private readonly int _bytesWritten;
        private readonly int _bytesBeforeCompression;
        private readonly int _objectCount;
        private readonly int _maxDepth;
        private readonly int _stringCount;
        private readonly int _collectionCount;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建统计信息
        /// </summary>
        public SerializeStatistics(
            long elapsedTicks,
            int bytesWritten,
            int bytesBeforeCompression,
            int objectCount,
            int maxDepth,
            int stringCount,
            int collectionCount)
        {
            _elapsedTicks = elapsedTicks;
            _bytesWritten = bytesWritten;
            _bytesBeforeCompression = bytesBeforeCompression;
            _objectCount = objectCount;
            _maxDepth = maxDepth;
            _stringCount = stringCount;
            _collectionCount = collectionCount;
        }

        #endregion

        #region 属性

        /// <summary>耗时 (Ticks)</summary>
        public long ElapsedTicks => _elapsedTicks;

        /// <summary>耗时 (毫秒)</summary>
        public double ElapsedMilliseconds => _elapsedTicks / (double)TimeSpan.TicksPerMillisecond;

        /// <summary>写入字节数</summary>
        public int BytesWritten => _bytesWritten;

        /// <summary>压缩前字节数</summary>
        public int BytesBeforeCompression => _bytesBeforeCompression;

        /// <summary>对象数量</summary>
        public int ObjectCount => _objectCount;

        /// <summary>最大深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>字符串数量</summary>
        public int StringCount => _stringCount;

        /// <summary>集合数量</summary>
        public int CollectionCount => _collectionCount;

        /// <summary>压缩率 (0-1，越小压缩效果越好)</summary>
        public float CompressionRatio => _bytesBeforeCompression > 0
            ? (float)_bytesWritten / _bytesBeforeCompression
            : 1.0f;

        /// <summary>吞吐量 (字节/秒)</summary>
        public double ThroughputBytesPerSecond => ElapsedMilliseconds > 0
            ? _bytesWritten / ElapsedMilliseconds * 1000
            : 0;

        #endregion

        #region 工厂方法

        /// <summary>空统计信息</summary>
        public static SerializeStatistics Empty => default;

        /// <summary>
        /// 创建统计信息
        /// </summary>
        public static SerializeStatistics Create(
            long elapsedTicks,
            int bytesWritten,
            int bytesBeforeCompression = 0,
            int objectCount = 0,
            int maxDepth = 0,
            int stringCount = 0,
            int collectionCount = 0)
        {
            return new SerializeStatistics(elapsedTicks, bytesWritten,
                bytesBeforeCompression > 0 ? bytesBeforeCompression : bytesWritten,
                objectCount, maxDepth, stringCount, collectionCount);
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeStatistics other)
        {
            return _elapsedTicks == other._elapsedTicks &&
                   _bytesWritten == other._bytesWritten &&
                   _objectCount == other._objectCount;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeStatistics other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_elapsedTicks, _bytesWritten, _objectCount);

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeStatistics left, SerializeStatistics right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeStatistics left, SerializeStatistics right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"Statistics(Bytes={_bytesWritten}, Time={ElapsedMilliseconds:F2}ms, Objects={_objectCount})";
        }

        #endregion
    }

    #endregion
}
