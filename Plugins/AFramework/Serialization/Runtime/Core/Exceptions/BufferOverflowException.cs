// ==========================================================
// 文件名：BufferOverflowException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 缓冲区溢出异常
    /// <para>当序列化操作超出缓冲区容量时抛出</para>
    /// </summary>
    /// <remarks>
    /// 缓冲区溢出的常见原因:
    /// <list type="bullet">
    /// <item>写入数据超出预分配缓冲区大小</item>
    /// <item>读取位置超出数据边界</item>
    /// <item>缓冲区池耗尽</item>
    /// <item>内存分配失败</item>
    /// </list>
    /// 
    /// 解决方案:
    /// <list type="number">
    /// <item>增加初始缓冲区大小</item>
    /// <item>使用自动扩容的缓冲区</item>
    /// <item>分块处理大数据</item>
    /// <item>检查数据大小是否合理</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     writer.WriteBytes(largeData);
    /// }
    /// catch (BufferOverflowException ex)
    /// {
    ///     Console.WriteLine($"缓冲区容量: {ex.BufferCapacity}");
    ///     Console.WriteLine($"当前位置: {ex.CurrentPosition}");
    ///     Console.WriteLine($"需要写入: {ex.RequiredSize}");
    ///     
    ///     // 使用更大的缓冲区重试
    ///     var largerBuffer = new byte[ex.RequiredSize * 2];
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class BufferOverflowException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息
        /// </summary>
        private const string DefaultMessage = "缓冲区溢出";

        #endregion

        #region 字段

        /// <summary>
        /// 缓冲区容量
        /// </summary>
        private readonly long _bufferCapacity;

        /// <summary>
        /// 当前位置
        /// </summary>
        private readonly long _currentPosition;

        /// <summary>
        /// 需要的大小
        /// </summary>
        private readonly long _requiredSize;

        /// <summary>
        /// 溢出类型
        /// </summary>
        private readonly BufferOverflowType _overflowType;

        /// <summary>
        /// 操作类型
        /// </summary>
        private readonly BufferOperationType _operationType;

        #endregion

        #region 属性

        /// <summary>
        /// 获取缓冲区容量
        /// </summary>
        /// <value>缓冲区的总容量（字节）</value>
        public long BufferCapacity => _bufferCapacity;

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <value>发生溢出时的缓冲区位置</value>
        public long CurrentPosition => _currentPosition;

        /// <summary>
        /// 获取需要的大小
        /// </summary>
        /// <value>操作需要的字节数</value>
        public long RequiredSize => _requiredSize;

        /// <summary>
        /// 获取溢出类型
        /// </summary>
        /// <value>缓冲区溢出的具体类型</value>
        public BufferOverflowType OverflowType => _overflowType;

        /// <summary>
        /// 获取操作类型
        /// </summary>
        /// <value>导致溢出的操作类型</value>
        public BufferOperationType OperationType => _operationType;

        /// <summary>
        /// 获取溢出量
        /// </summary>
        /// <value>超出缓冲区的字节数</value>
        public long OverflowAmount => Math.Max(0, (_currentPosition + _requiredSize) - _bufferCapacity);

        /// <summary>
        /// 获取剩余空间
        /// </summary>
        /// <value>缓冲区剩余可用空间</value>
        public long RemainingSpace => Math.Max(0, _bufferCapacity - _currentPosition);

        /// <summary>
        /// 获取建议的缓冲区大小
        /// </summary>
        /// <value>建议的新缓冲区大小</value>
        public long SuggestedBufferSize => Math.Max(_bufferCapacity * 2, _currentPosition + _requiredSize);

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建默认缓冲区溢出异常
        /// </summary>
        public BufferOverflowException()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// 使用缓冲区信息创建异常
        /// </summary>
        /// <param name="bufferCapacity">缓冲区容量</param>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="requiredSize">需要的大小</param>
        public BufferOverflowException(long bufferCapacity, long currentPosition, long requiredSize)
            : this(bufferCapacity, currentPosition, requiredSize, BufferOverflowType.WriteOverflow)
        {
        }

        /// <summary>
        /// 使用缓冲区信息和溢出类型创建异常
        /// </summary>
        /// <param name="bufferCapacity">缓冲区容量</param>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="requiredSize">需要的大小</param>
        /// <param name="overflowType">溢出类型</param>
        public BufferOverflowException(
            long bufferCapacity,
            long currentPosition,
            long requiredSize,
            BufferOverflowType overflowType)
            : this(bufferCapacity, currentPosition, requiredSize, overflowType, BufferOperationType.Unknown)
        {
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="bufferCapacity">缓冲区容量</param>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="requiredSize">需要的大小</param>
        /// <param name="overflowType">溢出类型</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="innerException">内部异常</param>
        public BufferOverflowException(
            long bufferCapacity,
            long currentPosition,
            long requiredSize,
            BufferOverflowType overflowType,
            BufferOperationType operationType,
            Exception innerException = null)
            : base(
                GetErrorCode(overflowType),
                FormatMessage(bufferCapacity, currentPosition, requiredSize, overflowType),
                null,
                null,
                currentPosition,
                innerException)
        {
            _bufferCapacity = bufferCapacity;
            _currentPosition = currentPosition;
            _requiredSize = requiredSize;
            _overflowType = overflowType;
            _operationType = operationType;
        }

        /// <summary>
        /// 使用自定义消息创建异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public BufferOverflowException(string message, Exception innerException = null)
            : base(SerializeErrorCode.BufferOverflow, message ?? DefaultMessage, innerException)
        {
            _overflowType = BufferOverflowType.Unknown;
            _operationType = BufferOperationType.Unknown;
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private BufferOverflowException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _bufferCapacity = info.GetInt64(nameof(BufferCapacity));
            _currentPosition = info.GetInt64(nameof(CurrentPosition));
            _requiredSize = info.GetInt64(nameof(RequiredSize));
            _overflowType = (BufferOverflowType)info.GetInt32(nameof(OverflowType));
            _operationType = (BufferOperationType)info.GetInt32(nameof(OperationType));
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(BufferCapacity), _bufferCapacity);
            info.AddValue(nameof(CurrentPosition), _currentPosition);
            info.AddValue(nameof(RequiredSize), _requiredSize);
            info.AddValue(nameof(OverflowType), (int)_overflowType);
            info.AddValue(nameof(OperationType), (int)_operationType);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取溢出类型对应的错误码
        /// </summary>
        private static SerializeErrorCode GetErrorCode(BufferOverflowType type)
        {
            return type switch
            {
                BufferOverflowType.ReadOverflow => SerializeErrorCode.BufferUnderflow,
                BufferOverflowType.PoolExhausted => SerializeErrorCode.BufferPoolExhausted,
                BufferOverflowType.OutOfMemory => SerializeErrorCode.OutOfMemory,
                _ => SerializeErrorCode.BufferOverflow
            };
        }

        /// <summary>
        /// 格式化错误消息
        /// </summary>
        private static string FormatMessage(
            long capacity,
            long position,
            long required,
            BufferOverflowType type)
        {
            var overflow = Math.Max(0, (position + required) - capacity);
            
            return type switch
            {
                BufferOverflowType.WriteOverflow =>
                    $"写入溢出: 缓冲区容量 {capacity} 字节，当前位置 {position}，需要写入 {required} 字节，溢出 {overflow} 字节",
                BufferOverflowType.ReadOverflow =>
                    $"读取溢出: 缓冲区容量 {capacity} 字节，当前位置 {position}，需要读取 {required} 字节，超出 {overflow} 字节",
                BufferOverflowType.PoolExhausted =>
                    $"缓冲区池耗尽: 请求 {required} 字节的缓冲区失败",
                BufferOverflowType.OutOfMemory =>
                    $"内存分配失败: 无法分配 {required} 字节的缓冲区",
                BufferOverflowType.CapacityExceeded =>
                    $"超出最大容量: 请求 {required} 字节，最大允许 {capacity} 字节",
                _ =>
                    $"缓冲区溢出: 容量 {capacity}，位置 {position}，需要 {required}"
            };
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 缓冲区溢出诊断信息 ===");
            builder.AppendLine($"溢出类型: {_overflowType}");
            builder.AppendLine($"操作类型: {_operationType}");
            builder.AppendLine($"缓冲区容量: {_bufferCapacity:N0} 字节");
            builder.AppendLine($"当前位置: {_currentPosition:N0} 字节");
            builder.AppendLine($"需要大小: {_requiredSize:N0} 字节");
            builder.AppendLine($"剩余空间: {RemainingSpace:N0} 字节");
            builder.AppendLine($"溢出量: {OverflowAmount:N0} 字节");
            builder.AppendLine($"建议缓冲区大小: {SuggestedBufferSize:N0} 字节");
            
            builder.AppendLine("建议:");
            builder.AppendLine(GetSuggestions());
            
            return builder.ToString();
        }

        /// <summary>
        /// 获取解决建议
        /// </summary>
        private string GetSuggestions()
        {
            return _overflowType switch
            {
                BufferOverflowType.WriteOverflow =>
                    "  - 增加初始缓冲区大小\n  - 使用自动扩容的 BufferWriter\n  - 分块写入大数据",
                BufferOverflowType.ReadOverflow =>
                    "  - 检查数据完整性\n  - 验证数据长度字段\n  - 确保数据未被截断",
                BufferOverflowType.PoolExhausted =>
                    "  - 增加缓冲区池大小\n  - 及时归还缓冲区\n  - 检查是否有缓冲区泄漏",
                BufferOverflowType.OutOfMemory =>
                    "  - 减少单次处理的数据量\n  - 使用流式处理\n  - 检查内存泄漏",
                BufferOverflowType.CapacityExceeded =>
                    "  - 调整最大容量限制\n  - 分块处理数据\n  - 使用流式序列化",
                _ =>
                    "  - 检查缓冲区配置\n  - 验证数据大小"
            };
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建写入溢出异常
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        /// <param name="position">当前位置</param>
        /// <param name="writeSize">写入大小</param>
        /// <returns>缓冲区溢出异常</returns>
        public static BufferOverflowException WriteOverflow(long capacity, long position, long writeSize)
        {
            return new BufferOverflowException(
                capacity,
                position,
                writeSize,
                BufferOverflowType.WriteOverflow,
                BufferOperationType.Write);
        }

        /// <summary>
        /// 创建读取溢出异常
        /// </summary>
        /// <param name="capacity">缓冲区容量</param>
        /// <param name="position">当前位置</param>
        /// <param name="readSize">读取大小</param>
        /// <returns>缓冲区溢出异常</returns>
        public static BufferOverflowException ReadOverflow(long capacity, long position, long readSize)
        {
            return new BufferOverflowException(
                capacity,
                position,
                readSize,
                BufferOverflowType.ReadOverflow,
                BufferOperationType.Read);
        }

        /// <summary>
        /// 创建缓冲区池耗尽异常
        /// </summary>
        /// <param name="requestedSize">请求的大小</param>
        /// <returns>缓冲区溢出异常</returns>
        public static BufferOverflowException PoolExhausted(long requestedSize)
        {
            return new BufferOverflowException(
                0,
                0,
                requestedSize,
                BufferOverflowType.PoolExhausted,
                BufferOperationType.Allocate);
        }

        /// <summary>
        /// 创建内存不足异常
        /// </summary>
        /// <param name="requestedSize">请求的大小</param>
        /// <param name="innerException">内部异常</param>
        /// <returns>缓冲区溢出异常</returns>
        public static BufferOverflowException OutOfMemory(long requestedSize, Exception innerException = null)
        {
            return new BufferOverflowException(
                0,
                0,
                requestedSize,
                BufferOverflowType.OutOfMemory,
                BufferOperationType.Allocate,
                innerException);
        }

        /// <summary>
        /// 创建超出最大容量异常
        /// </summary>
        /// <param name="maxCapacity">最大容量</param>
        /// <param name="requestedSize">请求的大小</param>
        /// <returns>缓冲区溢出异常</returns>
        public static BufferOverflowException CapacityExceeded(long maxCapacity, long requestedSize)
        {
            return new BufferOverflowException(
                maxCapacity,
                0,
                requestedSize,
                BufferOverflowType.CapacityExceeded,
                BufferOperationType.Resize);
        }

        #endregion
    }

    /// <summary>
    /// 缓冲区溢出类型枚举
    /// </summary>
    public enum BufferOverflowType
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 写入溢出
        /// </summary>
        WriteOverflow = 1,

        /// <summary>
        /// 读取溢出
        /// </summary>
        ReadOverflow = 2,

        /// <summary>
        /// 缓冲区池耗尽
        /// </summary>
        PoolExhausted = 3,

        /// <summary>
        /// 内存不足
        /// </summary>
        OutOfMemory = 4,

        /// <summary>
        /// 超出最大容量
        /// </summary>
        CapacityExceeded = 5
    }

    /// <summary>
    /// 缓冲区操作类型枚举
    /// </summary>
    public enum BufferOperationType
    {
        /// <summary>
        /// 未知操作
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 读取操作
        /// </summary>
        Read = 1,

        /// <summary>
        /// 写入操作
        /// </summary>
        Write = 2,

        /// <summary>
        /// 分配操作
        /// </summary>
        Allocate = 3,

        /// <summary>
        /// 调整大小操作
        /// </summary>
        Resize = 4,

        /// <summary>
        /// 定位操作
        /// </summary>
        Seek = 5
    }
}
