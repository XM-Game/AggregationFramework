// ==========================================================
// 文件名：SerializationException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化异常基类
    /// <para>所有序列化相关异常的基类，提供统一的错误处理机制</para>
    /// </summary>
    /// <remarks>
    /// 设计原则:
    /// <list type="bullet">
    /// <item>继承自 <see cref="Exception"/>，支持标准异常处理</item>
    /// <item>包含 <see cref="SerializeErrorCode"/> 错误码，便于程序化错误处理</item>
    /// <item>支持异常链，保留原始异常信息</item>
    /// <item>提供丰富的上下文信息用于调试</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     serializer.Serialize(data);
    /// }
    /// catch (SerializationException ex)
    /// {
    ///     // 根据错误码处理
    ///     switch (ex.ErrorCode)
    ///     {
    ///         case SerializeErrorCode.TypeNotSupported:
    ///             // 处理类型不支持
    ///             break;
    ///         case SerializeErrorCode.BufferOverflow:
    ///             // 处理缓冲区溢出
    ///             break;
    ///     }
    ///     
    ///     // 或检查错误分类
    ///     if (ex.ErrorCode.IsTypeError())
    ///         // 处理类型相关错误
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public class SerializationException : Exception
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息
        /// </summary>
        private const string DefaultMessage = "序列化操作发生错误";

        /// <summary>
        /// 默认英文错误消息
        /// </summary>
        private const string DefaultMessageEn = "Serialization operation failed";

        #endregion

        #region 字段

        /// <summary>
        /// 错误码
        /// </summary>
        private readonly SerializeErrorCode _errorCode;

        /// <summary>
        /// 相关类型（可选）
        /// </summary>
        private readonly Type _relatedType;

        /// <summary>
        /// 字段/属性名称（可选）
        /// </summary>
        private readonly string _memberName;

        /// <summary>
        /// 数据位置（可选）
        /// </summary>
        private readonly long _position;

        #endregion

        #region 属性

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <value>序列化错误码，用于程序化错误处理</value>
        public SerializeErrorCode ErrorCode => _errorCode;

        /// <summary>
        /// 获取相关类型
        /// </summary>
        /// <value>导致异常的类型，可能为 null</value>
        public Type RelatedType => _relatedType;

        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <value>导致异常的字段或属性名称，可能为 null</value>
        public string MemberName => _memberName;

        /// <summary>
        /// 获取数据位置
        /// </summary>
        /// <value>发生错误时的数据流位置，-1 表示未知</value>
        public long Position => _position;

        /// <summary>
        /// 检查是否为致命错误
        /// </summary>
        /// <value>true 表示不可恢复的错误</value>
        public bool IsFatal => _errorCode.IsFatal();

        /// <summary>
        /// 检查是否可重试
        /// </summary>
        /// <value>true 表示可以重试操作</value>
        public bool IsRetryable => _errorCode.IsRetryable();

        /// <summary>
        /// 获取错误分类
        /// </summary>
        /// <value>错误所属的分类名称</value>
        public string ErrorCategory => _errorCode.GetCategory();

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建默认序列化异常
        /// </summary>
        public SerializationException()
            : this(SerializeErrorCode.Unknown, DefaultMessage)
        {
        }

        /// <summary>
        /// 使用指定消息创建序列化异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public SerializationException(string message)
            : this(SerializeErrorCode.Unknown, message)
        {
        }

        /// <summary>
        /// 使用指定消息和内部异常创建序列化异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public SerializationException(string message, Exception innerException)
            : this(SerializeErrorCode.Unknown, message, innerException)
        {
        }

        /// <summary>
        /// 使用错误码创建序列化异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        public SerializationException(SerializeErrorCode errorCode)
            : this(errorCode, errorCode.GetDescription())
        {
        }

        /// <summary>
        /// 使用错误码和消息创建序列化异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        public SerializationException(SerializeErrorCode errorCode, string message)
            : base(FormatMessage(errorCode, message))
        {
            _errorCode = errorCode;
            _position = -1;
        }

        /// <summary>
        /// 使用错误码、消息和内部异常创建序列化异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public SerializationException(SerializeErrorCode errorCode, string message, Exception innerException)
            : base(FormatMessage(errorCode, message), innerException)
        {
            _errorCode = errorCode;
            _position = -1;
        }

        /// <summary>
        /// 使用完整上下文信息创建序列化异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误消息</param>
        /// <param name="relatedType">相关类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="position">数据位置</param>
        /// <param name="innerException">内部异常</param>
        public SerializationException(
            SerializeErrorCode errorCode,
            string message,
            Type relatedType,
            string memberName = null,
            long position = -1,
            Exception innerException = null)
            : base(FormatDetailedMessage(errorCode, message, relatedType, memberName, position), innerException)
        {
            _errorCode = errorCode;
            _relatedType = relatedType;
            _memberName = memberName;
            _position = position;
        }

        /// <summary>
        /// 序列化构造函数（用于跨 AppDomain 传递）
        /// </summary>
        /// <param name="info">序列化信息</param>
        /// <param name="context">流上下文</param>
        protected SerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _errorCode = (SerializeErrorCode)info.GetInt32(nameof(ErrorCode));
            _memberName = info.GetString(nameof(MemberName));
            _position = info.GetInt64(nameof(Position));
            
            var typeName = info.GetString(nameof(RelatedType));
            if (!string.IsNullOrEmpty(typeName))
            {
                _relatedType = Type.GetType(typeName);
            }
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        /// <param name="info">序列化信息</param>
        /// <param name="context">流上下文</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            info.AddValue(nameof(ErrorCode), (int)_errorCode);
            info.AddValue(nameof(MemberName), _memberName);
            info.AddValue(nameof(Position), _position);
            info.AddValue(nameof(RelatedType), _relatedType?.AssemblyQualifiedName);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 格式化错误消息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatMessage(SerializeErrorCode errorCode, string message)
        {
            return $"[{errorCode}] {message}";
        }

        /// <summary>
        /// 格式化详细错误消息
        /// </summary>
        private static string FormatDetailedMessage(
            SerializeErrorCode errorCode,
            string message,
            Type relatedType,
            string memberName,
            long position)
        {
            var builder = new System.Text.StringBuilder();
            builder.Append($"[{errorCode}] {message}");

            if (relatedType != null)
            {
                builder.Append($" | 类型: {relatedType.FullName}");
            }

            if (!string.IsNullOrEmpty(memberName))
            {
                builder.Append($" | 成员: {memberName}");
            }

            if (position >= 0)
            {
                builder.Append($" | 位置: {position}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取详细的调试信息
        /// </summary>
        /// <returns>包含完整上下文的调试字符串</returns>
        public string GetDebugInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 序列化异常详情 ===");
            builder.AppendLine($"错误码: {_errorCode} ({(int)_errorCode})");
            builder.AppendLine($"错误分类: {ErrorCategory}");
            builder.AppendLine($"错误描述: {_errorCode.GetDescription()}");
            builder.AppendLine($"消息: {Message}");
            
            if (_relatedType != null)
            {
                builder.AppendLine($"相关类型: {_relatedType.FullName}");
            }
            
            if (!string.IsNullOrEmpty(_memberName))
            {
                builder.AppendLine($"成员名称: {_memberName}");
            }
            
            if (_position >= 0)
            {
                builder.AppendLine($"数据位置: {_position}");
            }
            
            builder.AppendLine($"是否致命: {IsFatal}");
            builder.AppendLine($"是否可重试: {IsRetryable}");
            
            if (InnerException != null)
            {
                builder.AppendLine($"内部异常: {InnerException.GetType().Name} - {InnerException.Message}");
            }
            
            builder.AppendLine($"堆栈跟踪:\n{StackTrace}");
            
            return builder.ToString();
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 从错误码创建异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns>对应的序列化异常</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializationException FromErrorCode(SerializeErrorCode errorCode)
        {
            return new SerializationException(errorCode);
        }

        /// <summary>
        /// 从错误码和类型创建异常
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="type">相关类型</param>
        /// <returns>对应的序列化异常</returns>
        public static SerializationException FromErrorCode(SerializeErrorCode errorCode, Type type)
        {
            return new SerializationException(
                errorCode,
                errorCode.GetDescription(),
                type);
        }

        /// <summary>
        /// 包装现有异常
        /// </summary>
        /// <param name="exception">原始异常</param>
        /// <param name="errorCode">错误码</param>
        /// <returns>包装后的序列化异常</returns>
        public static SerializationException Wrap(Exception exception, SerializeErrorCode errorCode = SerializeErrorCode.Unknown)
        {
            if (exception is SerializationException serEx)
            {
                return serEx;
            }

            return new SerializationException(errorCode, exception.Message, exception);
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 返回异常的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}: [{_errorCode}] {Message}";
        }

        #endregion
    }
}
