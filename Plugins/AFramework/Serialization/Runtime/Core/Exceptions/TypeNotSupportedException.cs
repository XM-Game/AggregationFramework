// ==========================================================
// 文件名：TypeNotSupportedException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 类型不支持序列化异常
    /// <para>当尝试序列化不支持的类型时抛出</para>
    /// </summary>
    /// <remarks>
    /// 不支持序列化的类型包括:
    /// <list type="bullet">
    /// <item>指针类型 (IntPtr*, void*)</item>
    /// <item>委托类型 (Delegate, Action, Func)</item>
    /// <item>动态类型 (dynamic) - 部分场景</item>
    /// <item>COM 对象</item>
    /// <item>包含不可序列化字段的类型</item>
    /// <item>某些运行时类型 (RuntimeType, MethodInfo 等)</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     serializer.Serialize(unsupportedObject);
    /// }
    /// catch (TypeNotSupportedException ex)
    /// {
    ///     Console.WriteLine($"类型 {ex.UnsupportedType.Name} 不支持序列化");
    ///     Console.WriteLine($"原因: {ex.Reason}");
    ///     
    ///     // 检查是否有替代方案
    ///     if (ex.AlternativeType != null)
    ///     {
    ///         Console.WriteLine($"建议使用: {ex.AlternativeType.Name}");
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class TypeNotSupportedException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息模板
        /// </summary>
        private const string DefaultMessageTemplate = "类型 '{0}' 不支持序列化";

        #endregion

        #region 字段

        /// <summary>
        /// 不支持的类型
        /// </summary>
        private readonly Type _unsupportedType;

        /// <summary>
        /// 不支持的原因
        /// </summary>
        private readonly TypeNotSupportedReason _reason;

        /// <summary>
        /// 替代类型建议
        /// </summary>
        private readonly Type _alternativeType;

        /// <summary>
        /// 详细原因说明
        /// </summary>
        private readonly string _reasonDetail;

        #endregion

        #region 属性

        /// <summary>
        /// 获取不支持的类型
        /// </summary>
        /// <value>无法序列化的类型</value>
        public Type UnsupportedType => _unsupportedType;

        /// <summary>
        /// 获取不支持的原因
        /// </summary>
        /// <value>类型不支持序列化的原因枚举</value>
        public TypeNotSupportedReason Reason => _reason;

        /// <summary>
        /// 获取替代类型建议
        /// </summary>
        /// <value>可以替代使用的类型，可能为 null</value>
        public Type AlternativeType => _alternativeType;

        /// <summary>
        /// 获取详细原因说明
        /// </summary>
        /// <value>不支持原因的详细描述</value>
        public string ReasonDetail => _reasonDetail;

        #endregion

        #region 构造函数

        /// <summary>
        /// 使用不支持的类型创建异常
        /// </summary>
        /// <param name="unsupportedType">不支持的类型</param>
        public TypeNotSupportedException(Type unsupportedType)
            : this(unsupportedType, DetermineReason(unsupportedType))
        {
        }

        /// <summary>
        /// 使用不支持的类型和原因创建异常
        /// </summary>
        /// <param name="unsupportedType">不支持的类型</param>
        /// <param name="reason">不支持的原因</param>
        public TypeNotSupportedException(Type unsupportedType, TypeNotSupportedReason reason)
            : this(unsupportedType, reason, null, null)
        {
        }

        /// <summary>
        /// 使用不支持的类型、原因和自定义消息创建异常
        /// </summary>
        /// <param name="unsupportedType">不支持的类型</param>
        /// <param name="reason">不支持的原因</param>
        /// <param name="message">自定义消息</param>
        public TypeNotSupportedException(Type unsupportedType, TypeNotSupportedReason reason, string message)
            : this(unsupportedType, reason, message, null)
        {
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="unsupportedType">不支持的类型</param>
        /// <param name="reason">不支持的原因</param>
        /// <param name="message">自定义消息</param>
        /// <param name="alternativeType">替代类型</param>
        /// <param name="innerException">内部异常</param>
        public TypeNotSupportedException(
            Type unsupportedType,
            TypeNotSupportedReason reason,
            string message,
            Type alternativeType,
            Exception innerException = null)
            : base(
                SerializeErrorCode.TypeNotSupported,
                message ?? string.Format(DefaultMessageTemplate, unsupportedType?.FullName ?? "null"),
                unsupportedType,
                null,
                -1,
                innerException)
        {
            _unsupportedType = unsupportedType;
            _reason = reason;
            _alternativeType = alternativeType;
            _reasonDetail = GetReasonDescription(reason);
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private TypeNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var typeName = info.GetString(nameof(UnsupportedType));
            if (!string.IsNullOrEmpty(typeName))
            {
                _unsupportedType = Type.GetType(typeName);
            }
            
            _reason = (TypeNotSupportedReason)info.GetInt32(nameof(Reason));
            _reasonDetail = info.GetString(nameof(ReasonDetail));
            
            var altTypeName = info.GetString(nameof(AlternativeType));
            if (!string.IsNullOrEmpty(altTypeName))
            {
                _alternativeType = Type.GetType(altTypeName);
            }
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnsupportedType), _unsupportedType?.AssemblyQualifiedName);
            info.AddValue(nameof(Reason), (int)_reason);
            info.AddValue(nameof(ReasonDetail), _reasonDetail);
            info.AddValue(nameof(AlternativeType), _alternativeType?.AssemblyQualifiedName);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 自动判断类型不支持的原因
        /// </summary>
        private static TypeNotSupportedReason DetermineReason(Type type)
        {
            if (type == null)
                return TypeNotSupportedReason.Unknown;

            if (type.IsPointer)
                return TypeNotSupportedReason.PointerType;

            if (typeof(Delegate).IsAssignableFrom(type))
                return TypeNotSupportedReason.DelegateType;

            if (type.IsCOMObject)
                return TypeNotSupportedReason.ComObject;

            if (type == typeof(IntPtr) || type == typeof(UIntPtr))
                return TypeNotSupportedReason.PointerType;

            if (type.IsGenericTypeDefinition)
                return TypeNotSupportedReason.OpenGenericType;

            if (type.ContainsGenericParameters)
                return TypeNotSupportedReason.OpenGenericType;

            // 检查是否为运行时类型
            if (type.FullName?.StartsWith("System.Reflection") == true ||
                type.FullName?.StartsWith("System.Runtime") == true)
                return TypeNotSupportedReason.RuntimeType;

            return TypeNotSupportedReason.Unknown;
        }

        /// <summary>
        /// 获取原因的详细描述
        /// </summary>
        private static string GetReasonDescription(TypeNotSupportedReason reason)
        {
            return reason switch
            {
                TypeNotSupportedReason.PointerType => 
                    "指针类型无法序列化，因为指针值在不同进程/机器间没有意义",
                TypeNotSupportedReason.DelegateType => 
                    "委托类型无法序列化，因为它们引用方法和目标对象",
                TypeNotSupportedReason.ComObject => 
                    "COM 对象无法序列化，因为它们是非托管资源",
                TypeNotSupportedReason.OpenGenericType => 
                    "开放泛型类型无法序列化，必须使用具体的类型参数",
                TypeNotSupportedReason.RuntimeType => 
                    "运行时类型（如 MethodInfo、Type）需要特殊处理",
                TypeNotSupportedReason.ContainsUnsupportedMember => 
                    "类型包含不可序列化的成员",
                TypeNotSupportedReason.NoDefaultConstructor => 
                    "类型没有可访问的默认构造函数",
                TypeNotSupportedReason.SecurityRestriction => 
                    "由于安全限制，类型不允许序列化",
                TypeNotSupportedReason.CircularTypeDefinition => 
                    "类型定义存在循环依赖",
                _ => "类型不支持序列化，原因未知"
            };
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 类型不支持诊断信息 ===");
            builder.AppendLine($"不支持的类型: {_unsupportedType?.FullName ?? "null"}");
            builder.AppendLine($"原因: {_reason}");
            builder.AppendLine($"详细说明: {_reasonDetail}");
            
            if (_unsupportedType != null)
            {
                builder.AppendLine("类型信息:");
                builder.AppendLine($"  - 是否指针: {_unsupportedType.IsPointer}");
                builder.AppendLine($"  - 是否委托: {typeof(Delegate).IsAssignableFrom(_unsupportedType)}");
                builder.AppendLine($"  - 是否 COM: {_unsupportedType.IsCOMObject}");
                builder.AppendLine($"  - 是否泛型定义: {_unsupportedType.IsGenericTypeDefinition}");
                builder.AppendLine($"  - 基类型: {_unsupportedType.BaseType?.Name ?? "无"}");
            }

            if (_alternativeType != null)
            {
                builder.AppendLine($"建议替代类型: {_alternativeType.FullName}");
            }

            return builder.ToString();
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 为泛型类型创建异常
        /// </summary>
        /// <typeparam name="T">不支持的类型</typeparam>
        /// <returns>类型不支持异常</returns>
        public static TypeNotSupportedException ForType<T>()
        {
            return new TypeNotSupportedException(typeof(T));
        }

        /// <summary>
        /// 为指针类型创建异常
        /// </summary>
        /// <param name="pointerType">指针类型</param>
        /// <returns>类型不支持异常</returns>
        public static TypeNotSupportedException ForPointer(Type pointerType)
        {
            return new TypeNotSupportedException(pointerType, TypeNotSupportedReason.PointerType);
        }

        /// <summary>
        /// 为委托类型创建异常
        /// </summary>
        /// <param name="delegateType">委托类型</param>
        /// <returns>类型不支持异常</returns>
        public static TypeNotSupportedException ForDelegate(Type delegateType)
        {
            return new TypeNotSupportedException(delegateType, TypeNotSupportedReason.DelegateType);
        }

        /// <summary>
        /// 为包含不支持成员的类型创建异常
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">不支持的成员名称</param>
        /// <returns>类型不支持异常</returns>
        public static TypeNotSupportedException ForUnsupportedMember(Type type, string memberName)
        {
            return new TypeNotSupportedException(
                type,
                TypeNotSupportedReason.ContainsUnsupportedMember,
                $"类型 '{type.FullName}' 包含不可序列化的成员 '{memberName}'",
                null);
        }

        #endregion
    }

    /// <summary>
    /// 类型不支持序列化的原因枚举
    /// </summary>
    public enum TypeNotSupportedReason
    {
        /// <summary>
        /// 未知原因
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 指针类型
        /// </summary>
        PointerType = 1,

        /// <summary>
        /// 委托类型
        /// </summary>
        DelegateType = 2,

        /// <summary>
        /// COM 对象
        /// </summary>
        ComObject = 3,

        /// <summary>
        /// 开放泛型类型
        /// </summary>
        OpenGenericType = 4,

        /// <summary>
        /// 运行时类型
        /// </summary>
        RuntimeType = 5,

        /// <summary>
        /// 包含不支持的成员
        /// </summary>
        ContainsUnsupportedMember = 6,

        /// <summary>
        /// 没有默认构造函数
        /// </summary>
        NoDefaultConstructor = 7,

        /// <summary>
        /// 安全限制
        /// </summary>
        SecurityRestriction = 8,

        /// <summary>
        /// 循环类型定义
        /// </summary>
        CircularTypeDefinition = 9
    }
}
