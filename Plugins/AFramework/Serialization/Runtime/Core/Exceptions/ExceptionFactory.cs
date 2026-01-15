// ==========================================================
// 文件名：ExceptionFactory.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化异常工厂
    /// <para>提供统一的异常创建入口，简化异常抛出代码</para>
    /// </summary>
    /// <remarks>
    /// 设计目的:
    /// <list type="bullet">
    /// <item>统一异常创建逻辑</item>
    /// <item>减少重复代码</item>
    /// <item>便于异常消息本地化</item>
    /// <item>支持条件抛出（ThrowIf 模式）</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// // 直接抛出
    /// throw ExceptionFactory.FormatterNotFound&lt;MyType&gt;();
    /// 
    /// // 条件抛出
    /// ExceptionFactory.ThrowIfNull(value, nameof(value));
    /// ExceptionFactory.ThrowIfBufferOverflow(capacity, position, required);
    /// </code>
    /// </remarks>
    public static class ExceptionFactory
    {
        #region 格式化器异常

        /// <summary>
        /// 创建格式化器未找到异常
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>格式化器未找到异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormatterNotFoundException FormatterNotFound<T>()
        {
            return FormatterNotFoundException.ForType<T>();
        }

        /// <summary>
        /// 创建格式化器未找到异常
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>格式化器未找到异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormatterNotFoundException FormatterNotFound(Type type)
        {
            return new FormatterNotFoundException(type);
        }

        #endregion

        #region 类型异常

        /// <summary>
        /// 创建类型不支持异常
        /// </summary>
        /// <typeparam name="T">不支持的类型</typeparam>
        /// <returns>类型不支持异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeNotSupportedException TypeNotSupported<T>()
        {
            return TypeNotSupportedException.ForType<T>();
        }

        /// <summary>
        /// 创建类型不支持异常
        /// </summary>
        /// <param name="type">不支持的类型</param>
        /// <returns>类型不支持异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeNotSupportedException TypeNotSupported(Type type)
        {
            return new TypeNotSupportedException(type);
        }

        /// <summary>
        /// 创建类型不支持异常（带原因）
        /// </summary>
        /// <param name="type">不支持的类型</param>
        /// <param name="reason">不支持的原因</param>
        /// <returns>类型不支持异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeNotSupportedException TypeNotSupported(Type type, TypeNotSupportedReason reason)
        {
            return new TypeNotSupportedException(type, reason);
        }

        #endregion

        #region 版本异常

        /// <summary>
        /// 创建版本不匹配异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <returns>版本不匹配异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VersionMismatchException VersionMismatch(int dataVersion, int expectedVersion)
        {
            return new VersionMismatchException(dataVersion, expectedVersion);
        }

        /// <summary>
        /// 创建版本过旧异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="minVersion">最小支持版本</param>
        /// <param name="currentVersion">当前版本</param>
        /// <returns>版本不匹配异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VersionMismatchException VersionTooOld(int dataVersion, int minVersion, int currentVersion)
        {
            return VersionMismatchException.TooOld(dataVersion, minVersion, currentVersion);
        }

        /// <summary>
        /// 创建版本过新异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="maxVersion">最大支持版本</param>
        /// <param name="currentVersion">当前版本</param>
        /// <returns>版本不匹配异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VersionMismatchException VersionTooNew(int dataVersion, int maxVersion, int currentVersion)
        {
            return VersionMismatchException.TooNew(dataVersion, maxVersion, currentVersion);
        }

        #endregion

        #region 数据异常

        /// <summary>
        /// 创建数据损坏异常
        /// </summary>
        /// <param name="type">损坏类型</param>
        /// <returns>数据损坏异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataCorruptedException DataCorrupted(DataCorruptionType type)
        {
            return new DataCorruptedException(type);
        }

        /// <summary>
        /// 创建校验和失败异常
        /// </summary>
        /// <param name="expected">期望校验和</param>
        /// <param name="actual">实际校验和</param>
        /// <returns>数据损坏异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataCorruptedException ChecksumFailed(uint expected, uint actual)
        {
            return DataCorruptedException.ChecksumFailed(expected, actual);
        }

        /// <summary>
        /// 创建无效魔数异常
        /// </summary>
        /// <param name="expected">期望魔数</param>
        /// <param name="actual">实际魔数</param>
        /// <returns>数据损坏异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataCorruptedException InvalidMagicNumber(uint expected, uint actual)
        {
            return DataCorruptedException.InvalidMagic(expected, actual);
        }

        /// <summary>
        /// 创建数据截断异常
        /// </summary>
        /// <param name="expectedLength">期望长度</param>
        /// <param name="actualLength">实际长度</param>
        /// <returns>数据损坏异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataCorruptedException DataTruncated(long expectedLength, long actualLength)
        {
            return DataCorruptedException.Truncated(expectedLength, actualLength);
        }

        #endregion

        #region 缓冲区异常

        /// <summary>
        /// 创建写入溢出异常
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        /// <param name="position">当前位置</param>
        /// <param name="writeSize">写入大小</param>
        /// <returns>缓冲区溢出异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferOverflowException WriteOverflow(long capacity, long position, long writeSize)
        {
            return BufferOverflowException.WriteOverflow(capacity, position, writeSize);
        }

        /// <summary>
        /// 创建读取溢出异常
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        /// <param name="position">当前位置</param>
        /// <param name="readSize">读取大小</param>
        /// <returns>缓冲区溢出异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferOverflowException ReadOverflow(long capacity, long position, long readSize)
        {
            return BufferOverflowException.ReadOverflow(capacity, position, readSize);
        }

        /// <summary>
        /// 创建内存不足异常
        /// </summary>
        /// <param name="requestedSize">请求的大小</param>
        /// <returns>缓冲区溢出异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BufferOverflowException OutOfMemory(long requestedSize)
        {
            return BufferOverflowException.OutOfMemory(requestedSize);
        }

        #endregion

        #region 循环引用异常

        /// <summary>
        /// 创建循环引用异常
        /// </summary>
        /// <param name="type">循环引用的类型</param>
        /// <returns>循环引用异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CircularReferenceException CircularReference(Type type)
        {
            return new CircularReferenceException(type);
        }

        /// <summary>
        /// 创建自引用异常
        /// </summary>
        /// <typeparam name="T">自引用的类型</typeparam>
        /// <returns>循环引用异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CircularReferenceException SelfReference<T>()
        {
            return CircularReferenceException.SelfReference<T>();
        }

        #endregion

        #region 反序列化异常

        /// <summary>
        /// 创建反序列化异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="reason">失败原因</param>
        /// <returns>反序列化异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializationException DeserializationFailed(Type targetType, DeserializationFailureReason reason)
        {
            return new DeserializationException(targetType, reason);
        }

        /// <summary>
        /// 创建必需字段缺失异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>反序列化异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializationException RequiredFieldMissing(Type targetType, string fieldName)
        {
            return DeserializationException.RequiredFieldMissing(targetType, fieldName);
        }

        #endregion

        #region 条件抛出方法 (ThrowIf)

        /// <summary>
        /// 如果值为 null 则抛出异常
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <param name="paramName">参数名称</param>
        /// <exception cref="ArgumentNullException">值为 null 时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// 如果缓冲区溢出则抛出异常
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        /// <param name="position">当前位置</param>
        /// <param name="required">需要的大小</param>
        /// <exception cref="BufferOverflowException">溢出时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfBufferOverflow(long capacity, long position, long required)
        {
            if (position + required > capacity)
            {
                throw WriteOverflow(capacity, position, required);
            }
        }

        /// <summary>
        /// 如果读取溢出则抛出异常
        /// </summary>
        /// <param name="length">数据长度</param>
        /// <param name="position">当前位置</param>
        /// <param name="required">需要读取的大小</param>
        /// <exception cref="BufferOverflowException">溢出时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfReadOverflow(long length, long position, long required)
        {
            if (position + required > length)
            {
                throw ReadOverflow(length, position, required);
            }
        }

        /// <summary>
        /// 如果版本不匹配则抛出异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="minVersion">最小版本</param>
        /// <param name="maxVersion">最大版本</param>
        /// <param name="currentVersion">当前版本</param>
        /// <exception cref="VersionMismatchException">版本不匹配时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfVersionMismatch(int dataVersion, int minVersion, int maxVersion, int currentVersion)
        {
            if (dataVersion < minVersion)
            {
                throw VersionTooOld(dataVersion, minVersion, currentVersion);
            }
            if (dataVersion > maxVersion)
            {
                throw VersionTooNew(dataVersion, maxVersion, currentVersion);
            }
        }

        /// <summary>
        /// 如果校验和不匹配则抛出异常
        /// </summary>
        /// <param name="expected">期望校验和</param>
        /// <param name="actual">实际校验和</param>
        /// <exception cref="DataCorruptedException">校验和不匹配时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfChecksumMismatch(uint expected, uint actual)
        {
            if (expected != actual)
            {
                throw ChecksumFailed(expected, actual);
            }
        }

        /// <summary>
        /// 如果魔数不匹配则抛出异常
        /// </summary>
        /// <param name="expected">期望魔数</param>
        /// <param name="actual">实际魔数</param>
        /// <exception cref="DataCorruptedException">魔数不匹配时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfInvalidMagicNumber(uint expected, uint actual)
        {
            if (expected != actual)
            {
                throw InvalidMagicNumber(expected, actual);
            }
        }

        #endregion

        #region 通用序列化异常

        /// <summary>
        /// 创建通用序列化异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        /// <returns>序列化异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializationException Create(SerializeErrorCode errorCode, string message = null)
        {
            return new SerializationException(errorCode, message ?? errorCode.GetDescription());
        }

        /// <summary>
        /// 包装现有异常为序列化异常
        /// </summary>
        /// <param name="exception">原始异常</param>
        /// <param name="errorCode">错误码</param>
        /// <returns>序列化异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializationException Wrap(Exception exception, SerializeErrorCode errorCode = SerializeErrorCode.Unknown)
        {
            return SerializationException.Wrap(exception, errorCode);
        }

        #endregion
    }
}
