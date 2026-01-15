// ==========================================================
// 文件名：SerializeErrorCode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化错误码枚举
    /// <para>定义序列化/反序列化过程中可能发生的错误类型</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 检查错误类型
    /// if (result.ErrorCode == SerializeErrorCode.TypeNotSupported)
    ///     // 处理不支持的类型
    /// 
    /// // 获取错误描述
    /// string message = errorCode.GetDescription();
    /// 
    /// // 检查是否为致命错误
    /// if (errorCode.IsFatal())
    ///     throw new SerializationException(errorCode);
    /// </code>
    /// </remarks>
    public enum SerializeErrorCode : int
    {
        #region 成功 (0)

        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 0,

        #endregion

        #region 通用错误 (1-99)

        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// 操作被取消
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// 操作超时
        /// </summary>
        Timeout = 3,

        /// <summary>
        /// 无效参数
        /// </summary>
        InvalidArgument = 4,

        /// <summary>
        /// 空引用
        /// </summary>
        NullReference = 5,

        /// <summary>
        /// 操作不支持
        /// </summary>
        NotSupported = 6,

        /// <summary>
        /// 无效操作
        /// </summary>
        InvalidOperation = 7,

        /// <summary>
        /// 对象已释放
        /// </summary>
        ObjectDisposed = 8,

        #endregion

        #region 类型错误 (100-199)

        /// <summary>
        /// 类型不支持序列化
        /// </summary>
        TypeNotSupported = 100,

        /// <summary>
        /// 类型未找到
        /// </summary>
        TypeNotFound = 101,

        /// <summary>
        /// 类型不匹配
        /// </summary>
        TypeMismatch = 102,

        /// <summary>
        /// 格式化器未找到
        /// </summary>
        FormatterNotFound = 103,

        /// <summary>
        /// 无效的类型码
        /// </summary>
        InvalidTypeCode = 104,

        /// <summary>
        /// 循环类型引用
        /// </summary>
        CircularTypeReference = 105,

        /// <summary>
        /// 泛型类型错误
        /// </summary>
        GenericTypeError = 106,

        /// <summary>
        /// 联合类型未注册
        /// </summary>
        UnionTypeNotRegistered = 107,

        /// <summary>
        /// 构造函数未找到
        /// </summary>
        ConstructorNotFound = 108,

        #endregion

        #region 数据错误 (200-299)

        /// <summary>
        /// 数据损坏
        /// </summary>
        DataCorrupted = 200,

        /// <summary>
        /// 无效的魔数
        /// </summary>
        InvalidMagicNumber = 201,

        /// <summary>
        /// 版本不匹配
        /// </summary>
        VersionMismatch = 202,

        /// <summary>
        /// 校验和失败
        /// </summary>
        ChecksumFailed = 203,

        /// <summary>
        /// 数据截断
        /// </summary>
        DataTruncated = 204,

        /// <summary>
        /// 意外的数据结束
        /// </summary>
        UnexpectedEndOfData = 205,

        /// <summary>
        /// 无效的数据格式
        /// </summary>
        InvalidDataFormat = 206,

        /// <summary>
        /// 数据过大
        /// </summary>
        DataTooLarge = 207,

        /// <summary>
        /// 必需字段缺失
        /// </summary>
        RequiredFieldMissing = 208,

        /// <summary>
        /// 无效的字段值
        /// </summary>
        InvalidFieldValue = 209,

        /// <summary>
        /// 重复的字段
        /// </summary>
        DuplicateField = 210,

        #endregion

        #region 缓冲区错误 (300-399)

        /// <summary>
        /// 缓冲区溢出
        /// </summary>
        BufferOverflow = 300,

        /// <summary>
        /// 缓冲区不足
        /// </summary>
        BufferUnderflow = 301,

        /// <summary>
        /// 内存分配失败
        /// </summary>
        OutOfMemory = 302,

        /// <summary>
        /// 缓冲区池耗尽
        /// </summary>
        BufferPoolExhausted = 303,

        /// <summary>
        /// 无效的缓冲区位置
        /// </summary>
        InvalidBufferPosition = 304,

        #endregion

        #region 引用错误 (400-499)

        /// <summary>
        /// 循环引用检测
        /// </summary>
        CircularReference = 400,

        /// <summary>
        /// 无效的对象引用
        /// </summary>
        InvalidObjectReference = 401,

        /// <summary>
        /// 对象引用未找到
        /// </summary>
        ObjectReferenceNotFound = 402,

        /// <summary>
        /// 超出最大引用深度
        /// </summary>
        MaxDepthExceeded = 403,

        /// <summary>
        /// 超出最大对象数量
        /// </summary>
        MaxObjectCountExceeded = 404,

        #endregion

        #region 压缩/加密错误 (500-599)

        /// <summary>
        /// 压缩失败
        /// </summary>
        CompressionFailed = 500,

        /// <summary>
        /// 解压失败
        /// </summary>
        DecompressionFailed = 501,

        /// <summary>
        /// 加密失败
        /// </summary>
        EncryptionFailed = 502,

        /// <summary>
        /// 解密失败
        /// </summary>
        DecryptionFailed = 503,

        /// <summary>
        /// 无效的加密密钥
        /// </summary>
        InvalidEncryptionKey = 504,

        /// <summary>
        /// 认证失败
        /// </summary>
        AuthenticationFailed = 505,

        #endregion

        #region IO 错误 (600-699)

        /// <summary>
        /// IO 错误
        /// </summary>
        IOError = 600,

        /// <summary>
        /// 文件未找到
        /// </summary>
        FileNotFound = 601,

        /// <summary>
        /// 访问被拒绝
        /// </summary>
        AccessDenied = 602,

        /// <summary>
        /// 流已关闭
        /// </summary>
        StreamClosed = 603,

        /// <summary>
        /// 流不可读
        /// </summary>
        StreamNotReadable = 604,

        /// <summary>
        /// 流不可写
        /// </summary>
        StreamNotWritable = 605,

        /// <summary>
        /// 流不可定位
        /// </summary>
        StreamNotSeekable = 606,

        #endregion

        #region 版本兼容错误 (700-799)

        /// <summary>
        /// 版本过旧
        /// </summary>
        VersionTooOld = 700,

        /// <summary>
        /// 版本过新
        /// </summary>
        VersionTooNew = 701,

        /// <summary>
        /// 迁移失败
        /// </summary>
        MigrationFailed = 702,

        /// <summary>
        /// 不兼容的架构变更
        /// </summary>
        IncompatibleSchemaChange = 703,

        #endregion

        #region 内部错误 (900-999)

        /// <summary>
        /// 内部错误
        /// </summary>
        InternalError = 900,

        /// <summary>
        /// 断言失败
        /// </summary>
        AssertionFailed = 901,

        /// <summary>
        /// 状态无效
        /// </summary>
        InvalidState = 902,

        /// <summary>
        /// 未实现
        /// </summary>
        NotImplemented = 903

        #endregion
    }

    /// <summary>
    /// SerializeErrorCode 扩展方法
    /// </summary>
    public static class SerializeErrorCodeExtensions
    {
        #region 错误分类方法

        /// <summary>
        /// 检查是否成功
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuccess(this SerializeErrorCode code)
        {
            return code == SerializeErrorCode.Success;
        }

        /// <summary>
        /// 检查是否为错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsError(this SerializeErrorCode code)
        {
            return code != SerializeErrorCode.Success;
        }

        /// <summary>
        /// 检查是否为致命错误 (不可恢复)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFatal(this SerializeErrorCode code)
        {
            return code == SerializeErrorCode.DataCorrupted ||
                   code == SerializeErrorCode.OutOfMemory ||
                   code == SerializeErrorCode.InternalError ||
                   code == SerializeErrorCode.AssertionFailed;
        }

        /// <summary>
        /// 检查是否为可重试错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRetryable(this SerializeErrorCode code)
        {
            return code == SerializeErrorCode.Timeout ||
                   code == SerializeErrorCode.IOError ||
                   code == SerializeErrorCode.BufferPoolExhausted;
        }

        /// <summary>
        /// 检查是否为类型相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypeError(this SerializeErrorCode code)
        {
            return (int)code >= 100 && (int)code < 200;
        }

        /// <summary>
        /// 检查是否为数据相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataError(this SerializeErrorCode code)
        {
            return (int)code >= 200 && (int)code < 300;
        }

        /// <summary>
        /// 检查是否为缓冲区相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBufferError(this SerializeErrorCode code)
        {
            return (int)code >= 300 && (int)code < 400;
        }

        /// <summary>
        /// 检查是否为引用相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceError(this SerializeErrorCode code)
        {
            return (int)code >= 400 && (int)code < 500;
        }

        /// <summary>
        /// 检查是否为压缩/加密相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSecurityError(this SerializeErrorCode code)
        {
            return (int)code >= 500 && (int)code < 600;
        }

        /// <summary>
        /// 检查是否为 IO 相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIOError(this SerializeErrorCode code)
        {
            return (int)code >= 600 && (int)code < 700;
        }

        /// <summary>
        /// 检查是否为版本相关错误
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVersionError(this SerializeErrorCode code)
        {
            return (int)code >= 700 && (int)code < 800;
        }

        #endregion

        #region 错误描述方法

        /// <summary>
        /// 获取错误码的中文描述
        /// </summary>
        public static string GetDescription(this SerializeErrorCode code)
        {
            return code switch
            {
                // 成功
                SerializeErrorCode.Success => "操作成功",

                // 通用错误
                SerializeErrorCode.Unknown => "未知错误",
                SerializeErrorCode.Cancelled => "操作被取消",
                SerializeErrorCode.Timeout => "操作超时",
                SerializeErrorCode.InvalidArgument => "无效参数",
                SerializeErrorCode.NullReference => "空引用",
                SerializeErrorCode.NotSupported => "操作不支持",
                SerializeErrorCode.InvalidOperation => "无效操作",
                SerializeErrorCode.ObjectDisposed => "对象已释放",

                // 类型错误
                SerializeErrorCode.TypeNotSupported => "类型不支持序列化",
                SerializeErrorCode.TypeNotFound => "类型未找到",
                SerializeErrorCode.TypeMismatch => "类型不匹配",
                SerializeErrorCode.FormatterNotFound => "格式化器未找到",
                SerializeErrorCode.InvalidTypeCode => "无效的类型码",
                SerializeErrorCode.CircularTypeReference => "循环类型引用",
                SerializeErrorCode.GenericTypeError => "泛型类型错误",
                SerializeErrorCode.UnionTypeNotRegistered => "联合类型未注册",
                SerializeErrorCode.ConstructorNotFound => "构造函数未找到",

                // 数据错误
                SerializeErrorCode.DataCorrupted => "数据损坏",
                SerializeErrorCode.InvalidMagicNumber => "无效的文件头标识",
                SerializeErrorCode.VersionMismatch => "版本不匹配",
                SerializeErrorCode.ChecksumFailed => "校验和验证失败",
                SerializeErrorCode.DataTruncated => "数据被截断",
                SerializeErrorCode.UnexpectedEndOfData => "意外的数据结束",
                SerializeErrorCode.InvalidDataFormat => "无效的数据格式",
                SerializeErrorCode.DataTooLarge => "数据过大",
                SerializeErrorCode.RequiredFieldMissing => "必需字段缺失",
                SerializeErrorCode.InvalidFieldValue => "无效的字段值",
                SerializeErrorCode.DuplicateField => "重复的字段",

                // 缓冲区错误
                SerializeErrorCode.BufferOverflow => "缓冲区溢出",
                SerializeErrorCode.BufferUnderflow => "缓冲区不足",
                SerializeErrorCode.OutOfMemory => "内存分配失败",
                SerializeErrorCode.BufferPoolExhausted => "缓冲区池耗尽",
                SerializeErrorCode.InvalidBufferPosition => "无效的缓冲区位置",

                // 引用错误
                SerializeErrorCode.CircularReference => "检测到循环引用",
                SerializeErrorCode.InvalidObjectReference => "无效的对象引用",
                SerializeErrorCode.ObjectReferenceNotFound => "对象引用未找到",
                SerializeErrorCode.MaxDepthExceeded => "超出最大嵌套深度",
                SerializeErrorCode.MaxObjectCountExceeded => "超出最大对象数量",

                // 压缩/加密错误
                SerializeErrorCode.CompressionFailed => "压缩失败",
                SerializeErrorCode.DecompressionFailed => "解压失败",
                SerializeErrorCode.EncryptionFailed => "加密失败",
                SerializeErrorCode.DecryptionFailed => "解密失败",
                SerializeErrorCode.InvalidEncryptionKey => "无效的加密密钥",
                SerializeErrorCode.AuthenticationFailed => "认证失败",

                // IO 错误
                SerializeErrorCode.IOError => "IO 错误",
                SerializeErrorCode.FileNotFound => "文件未找到",
                SerializeErrorCode.AccessDenied => "访问被拒绝",
                SerializeErrorCode.StreamClosed => "流已关闭",
                SerializeErrorCode.StreamNotReadable => "流不可读",
                SerializeErrorCode.StreamNotWritable => "流不可写",
                SerializeErrorCode.StreamNotSeekable => "流不可定位",

                // 版本兼容错误
                SerializeErrorCode.VersionTooOld => "数据版本过旧",
                SerializeErrorCode.VersionTooNew => "数据版本过新",
                SerializeErrorCode.MigrationFailed => "版本迁移失败",
                SerializeErrorCode.IncompatibleSchemaChange => "不兼容的架构变更",

                // 内部错误
                SerializeErrorCode.InternalError => "内部错误",
                SerializeErrorCode.AssertionFailed => "断言失败",
                SerializeErrorCode.InvalidState => "状态无效",
                SerializeErrorCode.NotImplemented => "功能未实现",

                _ => $"未知错误码: {(int)code}"
            };
        }

        /// <summary>
        /// 获取错误码的英文名称
        /// </summary>
        public static string GetName(this SerializeErrorCode code)
        {
            return code.ToString();
        }

        /// <summary>
        /// 获取错误分类名称
        /// </summary>
        public static string GetCategory(this SerializeErrorCode code)
        {
            int value = (int)code;
            return value switch
            {
                0 => "成功",
                >= 1 and < 100 => "通用错误",
                >= 100 and < 200 => "类型错误",
                >= 200 and < 300 => "数据错误",
                >= 300 and < 400 => "缓冲区错误",
                >= 400 and < 500 => "引用错误",
                >= 500 and < 600 => "安全错误",
                >= 600 and < 700 => "IO错误",
                >= 700 and < 800 => "版本错误",
                >= 900 => "内部错误",
                _ => "未知分类"
            };
        }

        #endregion

        #region 异常转换方法

        /// <summary>
        /// 转换为异常
        /// </summary>
        public static Exception ToException(this SerializeErrorCode code, string message = null)
        {
            var description = message ?? code.GetDescription();
            
            return code switch
            {
                SerializeErrorCode.Success => null,
                SerializeErrorCode.InvalidArgument => new ArgumentException(description),
                SerializeErrorCode.NullReference => new ArgumentNullException(null, description),
                SerializeErrorCode.NotSupported => new NotSupportedException(description),
                SerializeErrorCode.InvalidOperation => new InvalidOperationException(description),
                SerializeErrorCode.ObjectDisposed => new ObjectDisposedException(null, description),
                SerializeErrorCode.OutOfMemory => new OutOfMemoryException(description),
                SerializeErrorCode.Timeout => new TimeoutException(description),
                SerializeErrorCode.FileNotFound => new System.IO.FileNotFoundException(description),
                SerializeErrorCode.IOError => new System.IO.IOException(description),
                SerializeErrorCode.NotImplemented => new NotImplementedException(description),
                _ => new InvalidOperationException($"[{code}] {description}")
            };
        }

        #endregion
    }
}
