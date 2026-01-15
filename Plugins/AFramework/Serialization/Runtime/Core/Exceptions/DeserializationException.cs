// ==========================================================
// 文件名：DeserializationException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 反序列化异常
    /// <para>当反序列化过程中发生错误时抛出</para>
    /// </summary>
    /// <remarks>
    /// 反序列化错误的常见原因:
    /// <list type="bullet">
    /// <item>数据格式与目标类型不匹配</item>
    /// <item>必需字段缺失</item>
    /// <item>字段值无效或超出范围</item>
    /// <item>构造函数调用失败</item>
    /// <item>类型转换失败</item>
    /// <item>回调方法执行失败</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     var data = serializer.Deserialize&lt;PlayerData&gt;(bytes);
    /// }
    /// catch (DeserializationException ex)
    /// {
    ///     Console.WriteLine($"反序列化失败: {ex.FailureReason}");
    ///     Console.WriteLine($"目标类型: {ex.TargetType?.Name}");
    ///     
    ///     if (ex.FailedMemberName != null)
    ///     {
    ///         Console.WriteLine($"失败的成员: {ex.FailedMemberName}");
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class DeserializationException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息
        /// </summary>
        private const string DefaultMessage = "反序列化失败";

        #endregion

        #region 字段

        /// <summary>
        /// 目标类型
        /// </summary>
        private readonly Type _targetType;

        /// <summary>
        /// 失败原因
        /// </summary>
        private readonly DeserializationFailureReason _failureReason;

        /// <summary>
        /// 失败的成员名称
        /// </summary>
        private readonly string _failedMemberName;

        /// <summary>
        /// 期望的值类型
        /// </summary>
        private readonly Type _expectedValueType;

        /// <summary>
        /// 实际的值类型
        /// </summary>
        private readonly Type _actualValueType;

        /// <summary>
        /// 实际值的字符串表示
        /// </summary>
        private readonly string _actualValueString;

        #endregion

        #region 属性

        /// <summary>
        /// 获取目标类型
        /// </summary>
        /// <value>反序列化的目标类型</value>
        public Type TargetType => _targetType;

        /// <summary>
        /// 获取失败原因
        /// </summary>
        /// <value>反序列化失败的具体原因</value>
        public DeserializationFailureReason FailureReason => _failureReason;

        /// <summary>
        /// 获取失败的成员名称
        /// </summary>
        /// <value>导致失败的字段或属性名称，可能为 null</value>
        public string FailedMemberName => _failedMemberName;

        /// <summary>
        /// 获取期望的值类型
        /// </summary>
        /// <value>成员期望的类型，可能为 null</value>
        public Type ExpectedValueType => _expectedValueType;

        /// <summary>
        /// 获取实际的值类型
        /// </summary>
        /// <value>数据中实际的类型，可能为 null</value>
        public Type ActualValueType => _actualValueType;

        /// <summary>
        /// 获取实际值的字符串表示
        /// </summary>
        /// <value>实际值的调试字符串，可能为 null</value>
        public string ActualValueString => _actualValueString;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建默认反序列化异常
        /// </summary>
        public DeserializationException()
            : this(DefaultMessage)
        {
        }

        /// <summary>
        /// 使用消息创建异常
        /// </summary>
        /// <param name="message">错误消息</param>
        public DeserializationException(string message)
            : base(SerializeErrorCode.Unknown, message ?? DefaultMessage)
        {
            _failureReason = DeserializationFailureReason.Unknown;
        }

        /// <summary>
        /// 使用消息和内部异常创建异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public DeserializationException(string message, Exception innerException)
            : base(SerializeErrorCode.Unknown, message ?? DefaultMessage, innerException)
        {
            _failureReason = DeserializationFailureReason.Unknown;
        }

        /// <summary>
        /// 使用目标类型和失败原因创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="reason">失败原因</param>
        public DeserializationException(Type targetType, DeserializationFailureReason reason)
            : this(targetType, reason, null, null)
        {
        }

        /// <summary>
        /// 使用目标类型、失败原因和成员名称创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="reason">失败原因</param>
        /// <param name="memberName">失败的成员名称</param>
        /// <param name="innerException">内部异常</param>
        public DeserializationException(
            Type targetType,
            DeserializationFailureReason reason,
            string memberName,
            Exception innerException)
            : base(
                GetErrorCode(reason),
                FormatMessage(targetType, reason, memberName),
                targetType,
                memberName,
                -1,
                innerException)
        {
            _targetType = targetType;
            _failureReason = reason;
            _failedMemberName = memberName;
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="reason">失败原因</param>
        /// <param name="memberName">失败的成员名称</param>
        /// <param name="expectedType">期望的值类型</param>
        /// <param name="actualType">实际的值类型</param>
        /// <param name="actualValue">实际值的字符串表示</param>
        /// <param name="innerException">内部异常</param>
        public DeserializationException(
            Type targetType,
            DeserializationFailureReason reason,
            string memberName,
            Type expectedType,
            Type actualType,
            string actualValue = null,
            Exception innerException = null)
            : base(
                GetErrorCode(reason),
                FormatDetailedMessage(targetType, reason, memberName, expectedType, actualType),
                targetType,
                memberName,
                -1,
                innerException)
        {
            _targetType = targetType;
            _failureReason = reason;
            _failedMemberName = memberName;
            _expectedValueType = expectedType;
            _actualValueType = actualType;
            _actualValueString = actualValue;
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private DeserializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _failureReason = (DeserializationFailureReason)info.GetInt32(nameof(FailureReason));
            _failedMemberName = info.GetString(nameof(FailedMemberName));
            _actualValueString = info.GetString(nameof(ActualValueString));
            
            var targetTypeName = info.GetString(nameof(TargetType));
            if (!string.IsNullOrEmpty(targetTypeName))
                _targetType = Type.GetType(targetTypeName);
            
            var expectedTypeName = info.GetString(nameof(ExpectedValueType));
            if (!string.IsNullOrEmpty(expectedTypeName))
                _expectedValueType = Type.GetType(expectedTypeName);
            
            var actualTypeName = info.GetString(nameof(ActualValueType));
            if (!string.IsNullOrEmpty(actualTypeName))
                _actualValueType = Type.GetType(actualTypeName);
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(FailureReason), (int)_failureReason);
            info.AddValue(nameof(FailedMemberName), _failedMemberName);
            info.AddValue(nameof(ActualValueString), _actualValueString);
            info.AddValue(nameof(TargetType), _targetType?.AssemblyQualifiedName);
            info.AddValue(nameof(ExpectedValueType), _expectedValueType?.AssemblyQualifiedName);
            info.AddValue(nameof(ActualValueType), _actualValueType?.AssemblyQualifiedName);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取失败原因对应的错误码
        /// </summary>
        private static SerializeErrorCode GetErrorCode(DeserializationFailureReason reason)
        {
            return reason switch
            {
                DeserializationFailureReason.TypeMismatch => SerializeErrorCode.TypeMismatch,
                DeserializationFailureReason.RequiredFieldMissing => SerializeErrorCode.RequiredFieldMissing,
                DeserializationFailureReason.InvalidFieldValue => SerializeErrorCode.InvalidFieldValue,
                DeserializationFailureReason.ConstructorFailed => SerializeErrorCode.ConstructorNotFound,
                DeserializationFailureReason.TypeNotFound => SerializeErrorCode.TypeNotFound,
                DeserializationFailureReason.InvalidDataFormat => SerializeErrorCode.InvalidDataFormat,
                _ => SerializeErrorCode.Unknown
            };
        }

        /// <summary>
        /// 格式化错误消息
        /// </summary>
        private static string FormatMessage(Type targetType, DeserializationFailureReason reason, string memberName)
        {
            var typeName = targetType?.Name ?? "unknown";
            var reasonDesc = GetReasonDescription(reason);
            
            if (!string.IsNullOrEmpty(memberName))
            {
                return $"反序列化类型 '{typeName}' 的成员 '{memberName}' 失败: {reasonDesc}";
            }
            return $"反序列化类型 '{typeName}' 失败: {reasonDesc}";
        }

        /// <summary>
        /// 格式化详细错误消息
        /// </summary>
        private static string FormatDetailedMessage(
            Type targetType,
            DeserializationFailureReason reason,
            string memberName,
            Type expectedType,
            Type actualType)
        {
            var builder = new System.Text.StringBuilder();
            builder.Append(FormatMessage(targetType, reason, memberName));
            
            if (expectedType != null && actualType != null)
            {
                builder.Append($" (期望: {expectedType.Name}, 实际: {actualType.Name})");
            }
            
            return builder.ToString();
        }

        /// <summary>
        /// 获取失败原因的描述
        /// </summary>
        private static string GetReasonDescription(DeserializationFailureReason reason)
        {
            return reason switch
            {
                DeserializationFailureReason.TypeMismatch => "类型不匹配",
                DeserializationFailureReason.RequiredFieldMissing => "必需字段缺失",
                DeserializationFailureReason.InvalidFieldValue => "字段值无效",
                DeserializationFailureReason.ConstructorFailed => "构造函数调用失败",
                DeserializationFailureReason.CallbackFailed => "回调方法执行失败",
                DeserializationFailureReason.TypeNotFound => "类型未找到",
                DeserializationFailureReason.InvalidDataFormat => "数据格式无效",
                DeserializationFailureReason.ConversionFailed => "类型转换失败",
                DeserializationFailureReason.NullNotAllowed => "不允许空值",
                DeserializationFailureReason.OutOfRange => "值超出范围",
                _ => "未知原因"
            };
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 反序列化异常诊断信息 ===");
            builder.AppendLine($"目标类型: {_targetType?.FullName ?? "unknown"}");
            builder.AppendLine($"失败原因: {_failureReason} - {GetReasonDescription(_failureReason)}");
            
            if (!string.IsNullOrEmpty(_failedMemberName))
            {
                builder.AppendLine($"失败成员: {_failedMemberName}");
            }
            
            if (_expectedValueType != null)
            {
                builder.AppendLine($"期望类型: {_expectedValueType.FullName}");
            }
            
            if (_actualValueType != null)
            {
                builder.AppendLine($"实际类型: {_actualValueType.FullName}");
            }
            
            if (!string.IsNullOrEmpty(_actualValueString))
            {
                builder.AppendLine($"实际值: {_actualValueString}");
            }
            
            builder.AppendLine("建议:");
            builder.AppendLine(GetSuggestions());
            
            return builder.ToString();
        }

        /// <summary>
        /// 获取解决建议
        /// </summary>
        private string GetSuggestions()
        {
            return _failureReason switch
            {
                DeserializationFailureReason.TypeMismatch =>
                    "  - 检查数据版本是否匹配\n  - 使用 VersionTolerant 模式\n  - 验证数据来源",
                DeserializationFailureReason.RequiredFieldMissing =>
                    "  - 检查数据完整性\n  - 为字段添加默认值\n  - 移除 [SerializeRequired] 特性",
                DeserializationFailureReason.InvalidFieldValue =>
                    "  - 验证数据有效性\n  - 添加数据验证逻辑\n  - 检查数据范围",
                DeserializationFailureReason.ConstructorFailed =>
                    "  - 确保类型有可访问的构造函数\n  - 使用 [SerializeConstructor] 标记构造函数\n  - 检查构造函数参数",
                DeserializationFailureReason.TypeNotFound =>
                    "  - 确保程序集已加载\n  - 检查类型名称是否正确\n  - 注册类型映射",
                _ =>
                    "  - 检查数据格式\n  - 验证序列化配置\n  - 查看内部异常详情"
            };
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建类型不匹配异常
        /// </summary>
        public static DeserializationException TypeMismatch<TTarget>(Type actualType)
        {
            return new DeserializationException(
                typeof(TTarget),
                DeserializationFailureReason.TypeMismatch,
                null,
                typeof(TTarget),
                actualType);
        }

        /// <summary>
        /// 创建必需字段缺失异常
        /// </summary>
        public static DeserializationException RequiredFieldMissing(Type targetType, string fieldName)
        {
            return new DeserializationException(
                targetType,
                DeserializationFailureReason.RequiredFieldMissing,
                fieldName,
                null);
        }

        /// <summary>
        /// 创建无效字段值异常
        /// </summary>
        public static DeserializationException InvalidFieldValue(
            Type targetType,
            string fieldName,
            Type expectedType,
            object actualValue)
        {
            return new DeserializationException(
                targetType,
                DeserializationFailureReason.InvalidFieldValue,
                fieldName,
                expectedType,
                actualValue?.GetType(),
                actualValue?.ToString());
        }

        /// <summary>
        /// 创建构造函数失败异常
        /// </summary>
        public static DeserializationException ConstructorFailed(Type targetType, Exception innerException)
        {
            return new DeserializationException(
                targetType,
                DeserializationFailureReason.ConstructorFailed,
                null,
                innerException);
        }

        /// <summary>
        /// 创建回调失败异常
        /// </summary>
        public static DeserializationException CallbackFailed(
            Type targetType,
            string callbackName,
            Exception innerException)
        {
            return new DeserializationException(
                targetType,
                DeserializationFailureReason.CallbackFailed,
                callbackName,
                innerException);
        }

        /// <summary>
        /// 创建类型未找到异常
        /// </summary>
        public static DeserializationException TypeNotFound(string typeName)
        {
            return new DeserializationException(
                null,
                DeserializationFailureReason.TypeNotFound,
                null,
                null)
            {
                // 可以添加额外信息
            };
        }

        #endregion
    }

    /// <summary>
    /// 反序列化失败原因枚举
    /// </summary>
    public enum DeserializationFailureReason
    {
        /// <summary>
        /// 未知原因
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 类型不匹配
        /// </summary>
        TypeMismatch = 1,

        /// <summary>
        /// 必需字段缺失
        /// </summary>
        RequiredFieldMissing = 2,

        /// <summary>
        /// 字段值无效
        /// </summary>
        InvalidFieldValue = 3,

        /// <summary>
        /// 构造函数调用失败
        /// </summary>
        ConstructorFailed = 4,

        /// <summary>
        /// 回调方法执行失败
        /// </summary>
        CallbackFailed = 5,

        /// <summary>
        /// 类型未找到
        /// </summary>
        TypeNotFound = 6,

        /// <summary>
        /// 数据格式无效
        /// </summary>
        InvalidDataFormat = 7,

        /// <summary>
        /// 类型转换失败
        /// </summary>
        ConversionFailed = 8,

        /// <summary>
        /// 不允许空值
        /// </summary>
        NullNotAllowed = 9,

        /// <summary>
        /// 值超出范围
        /// </summary>
        OutOfRange = 10
    }
}
