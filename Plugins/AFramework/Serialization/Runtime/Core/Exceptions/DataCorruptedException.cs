// ==========================================================
// 文件名：DataCorruptedException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 数据损坏异常
    /// <para>当序列化数据损坏或无效时抛出</para>
    /// </summary>
    /// <remarks>
    /// 数据损坏的常见原因:
    /// <list type="bullet">
    /// <item>文件传输过程中损坏</item>
    /// <item>存储介质故障</item>
    /// <item>数据被意外修改</item>
    /// <item>校验和验证失败</item>
    /// <item>无效的魔数/文件头</item>
    /// <item>数据截断</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     var data = serializer.Deserialize&lt;GameSave&gt;(bytes);
    /// }
    /// catch (DataCorruptedException ex)
    /// {
    ///     Console.WriteLine($"数据损坏类型: {ex.CorruptionType}");
    ///     Console.WriteLine($"损坏位置: {ex.CorruptionOffset}");
    ///     
    ///     if (ex.ExpectedChecksum.HasValue)
    ///     {
    ///         Console.WriteLine($"期望校验和: {ex.ExpectedChecksum}");
    ///         Console.WriteLine($"实际校验和: {ex.ActualChecksum}");
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class DataCorruptedException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息
        /// </summary>
        private const string DefaultMessage = "序列化数据已损坏";

        #endregion

        #region 字段

        /// <summary>
        /// 损坏类型
        /// </summary>
        private readonly DataCorruptionType _corruptionType;

        /// <summary>
        /// 损坏位置偏移
        /// </summary>
        private readonly long _corruptionOffset;

        /// <summary>
        /// 期望的校验和
        /// </summary>
        private readonly uint? _expectedChecksum;

        /// <summary>
        /// 实际的校验和
        /// </summary>
        private readonly uint? _actualChecksum;

        /// <summary>
        /// 期望的魔数
        /// </summary>
        private readonly uint? _expectedMagicNumber;

        /// <summary>
        /// 实际的魔数
        /// </summary>
        private readonly uint? _actualMagicNumber;

        /// <summary>
        /// 损坏的数据片段（用于调试）
        /// </summary>
        private readonly byte[] _corruptedDataSample;

        #endregion

        #region 属性

        /// <summary>
        /// 获取损坏类型
        /// </summary>
        /// <value>数据损坏的具体类型</value>
        public DataCorruptionType CorruptionType => _corruptionType;

        /// <summary>
        /// 获取损坏位置偏移
        /// </summary>
        /// <value>检测到损坏的数据偏移位置，-1 表示未知</value>
        public long CorruptionOffset => _corruptionOffset;

        /// <summary>
        /// 获取期望的校验和
        /// </summary>
        /// <value>数据头中记录的校验和，null 表示不适用</value>
        public uint? ExpectedChecksum => _expectedChecksum;

        /// <summary>
        /// 获取实际的校验和
        /// </summary>
        /// <value>计算得到的实际校验和，null 表示不适用</value>
        public uint? ActualChecksum => _actualChecksum;

        /// <summary>
        /// 获取期望的魔数
        /// </summary>
        /// <value>期望的文件头魔数，null 表示不适用</value>
        public uint? ExpectedMagicNumber => _expectedMagicNumber;

        /// <summary>
        /// 获取实际的魔数
        /// </summary>
        /// <value>实际读取的魔数，null 表示不适用</value>
        public uint? ActualMagicNumber => _actualMagicNumber;

        /// <summary>
        /// 获取损坏的数据样本
        /// </summary>
        /// <value>损坏位置附近的数据片段，用于调试</value>
        public byte[] CorruptedDataSample => _corruptedDataSample;

        /// <summary>
        /// 检查是否为校验和错误
        /// </summary>
        public bool IsChecksumError => _corruptionType == DataCorruptionType.ChecksumMismatch;

        /// <summary>
        /// 检查是否为魔数错误
        /// </summary>
        public bool IsMagicNumberError => _corruptionType == DataCorruptionType.InvalidMagicNumber;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建默认数据损坏异常
        /// </summary>
        public DataCorruptedException()
            : this(DataCorruptionType.Unknown, DefaultMessage)
        {
        }

        /// <summary>
        /// 使用损坏类型创建异常
        /// </summary>
        /// <param name="corruptionType">损坏类型</param>
        public DataCorruptedException(DataCorruptionType corruptionType)
            : this(corruptionType, GetDefaultMessage(corruptionType))
        {
        }

        /// <summary>
        /// 使用损坏类型和消息创建异常
        /// </summary>
        /// <param name="corruptionType">损坏类型</param>
        /// <param name="message">错误消息</param>
        public DataCorruptedException(DataCorruptionType corruptionType, string message)
            : this(corruptionType, message, -1)
        {
        }

        /// <summary>
        /// 使用损坏类型、消息和位置创建异常
        /// </summary>
        /// <param name="corruptionType">损坏类型</param>
        /// <param name="message">错误消息</param>
        /// <param name="offset">损坏位置</param>
        public DataCorruptedException(DataCorruptionType corruptionType, string message, long offset)
            : base(
                GetErrorCode(corruptionType),
                message ?? GetDefaultMessage(corruptionType),
                null,
                null,
                offset)
        {
            _corruptionType = corruptionType;
            _corruptionOffset = offset;
        }

        /// <summary>
        /// 使用校验和信息创建异常
        /// </summary>
        /// <param name="expectedChecksum">期望校验和</param>
        /// <param name="actualChecksum">实际校验和</param>
        /// <param name="offset">损坏位置</param>
        public DataCorruptedException(uint expectedChecksum, uint actualChecksum, long offset = -1)
            : base(
                SerializeErrorCode.ChecksumFailed,
                $"校验和验证失败: 期望 0x{expectedChecksum:X8}，实际 0x{actualChecksum:X8}",
                null,
                null,
                offset)
        {
            _corruptionType = DataCorruptionType.ChecksumMismatch;
            _corruptionOffset = offset;
            _expectedChecksum = expectedChecksum;
            _actualChecksum = actualChecksum;
        }

        /// <summary>
        /// 使用魔数信息创建异常
        /// </summary>
        /// <param name="expectedMagic">期望魔数</param>
        /// <param name="actualMagic">实际魔数</param>
        public DataCorruptedException(uint expectedMagic, uint actualMagic)
            : base(
                SerializeErrorCode.InvalidMagicNumber,
                $"无效的文件头标识: 期望 0x{expectedMagic:X8}，实际 0x{actualMagic:X8}",
                null,
                null,
                0)
        {
            _corruptionType = DataCorruptionType.InvalidMagicNumber;
            _corruptionOffset = 0;
            _expectedMagicNumber = expectedMagic;
            _actualMagicNumber = actualMagic;
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="corruptionType">损坏类型</param>
        /// <param name="message">错误消息</param>
        /// <param name="offset">损坏位置</param>
        /// <param name="corruptedDataSample">损坏数据样本</param>
        /// <param name="innerException">内部异常</param>
        public DataCorruptedException(
            DataCorruptionType corruptionType,
            string message,
            long offset,
            byte[] corruptedDataSample,
            Exception innerException = null)
            : base(
                GetErrorCode(corruptionType),
                message ?? GetDefaultMessage(corruptionType),
                null,
                null,
                offset,
                innerException)
        {
            _corruptionType = corruptionType;
            _corruptionOffset = offset;
            _corruptedDataSample = corruptedDataSample;
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private DataCorruptedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _corruptionType = (DataCorruptionType)info.GetInt32(nameof(CorruptionType));
            _corruptionOffset = info.GetInt64(nameof(CorruptionOffset));
            
            if (info.GetBoolean("HasExpectedChecksum"))
                _expectedChecksum = info.GetUInt32(nameof(ExpectedChecksum));
            if (info.GetBoolean("HasActualChecksum"))
                _actualChecksum = info.GetUInt32(nameof(ActualChecksum));
            if (info.GetBoolean("HasExpectedMagicNumber"))
                _expectedMagicNumber = info.GetUInt32(nameof(ExpectedMagicNumber));
            if (info.GetBoolean("HasActualMagicNumber"))
                _actualMagicNumber = info.GetUInt32(nameof(ActualMagicNumber));
            
            _corruptedDataSample = (byte[])info.GetValue(nameof(CorruptedDataSample), typeof(byte[]));
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(CorruptionType), (int)_corruptionType);
            info.AddValue(nameof(CorruptionOffset), _corruptionOffset);
            
            info.AddValue("HasExpectedChecksum", _expectedChecksum.HasValue);
            if (_expectedChecksum.HasValue)
                info.AddValue(nameof(ExpectedChecksum), _expectedChecksum.Value);
            
            info.AddValue("HasActualChecksum", _actualChecksum.HasValue);
            if (_actualChecksum.HasValue)
                info.AddValue(nameof(ActualChecksum), _actualChecksum.Value);
            
            info.AddValue("HasExpectedMagicNumber", _expectedMagicNumber.HasValue);
            if (_expectedMagicNumber.HasValue)
                info.AddValue(nameof(ExpectedMagicNumber), _expectedMagicNumber.Value);
            
            info.AddValue("HasActualMagicNumber", _actualMagicNumber.HasValue);
            if (_actualMagicNumber.HasValue)
                info.AddValue(nameof(ActualMagicNumber), _actualMagicNumber.Value);
            
            info.AddValue(nameof(CorruptedDataSample), _corruptedDataSample);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取损坏类型对应的错误码
        /// </summary>
        private static SerializeErrorCode GetErrorCode(DataCorruptionType type)
        {
            return type switch
            {
                DataCorruptionType.ChecksumMismatch => SerializeErrorCode.ChecksumFailed,
                DataCorruptionType.InvalidMagicNumber => SerializeErrorCode.InvalidMagicNumber,
                DataCorruptionType.DataTruncated => SerializeErrorCode.DataTruncated,
                DataCorruptionType.UnexpectedEndOfData => SerializeErrorCode.UnexpectedEndOfData,
                DataCorruptionType.InvalidDataFormat => SerializeErrorCode.InvalidDataFormat,
                _ => SerializeErrorCode.DataCorrupted
            };
        }

        /// <summary>
        /// 获取损坏类型的默认消息
        /// </summary>
        private static string GetDefaultMessage(DataCorruptionType type)
        {
            return type switch
            {
                DataCorruptionType.ChecksumMismatch => "数据校验和验证失败",
                DataCorruptionType.InvalidMagicNumber => "无效的文件头标识",
                DataCorruptionType.DataTruncated => "数据被截断",
                DataCorruptionType.UnexpectedEndOfData => "意外的数据结束",
                DataCorruptionType.InvalidDataFormat => "无效的数据格式",
                DataCorruptionType.InvalidHeader => "无效的数据头",
                DataCorruptionType.InvalidTypeCode => "无效的类型码",
                DataCorruptionType.InvalidLength => "无效的长度值",
                DataCorruptionType.InvalidReference => "无效的对象引用",
                DataCorruptionType.DecompressionFailed => "数据解压失败",
                DataCorruptionType.DecryptionFailed => "数据解密失败",
                _ => "序列化数据已损坏"
            };
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 数据损坏诊断信息 ===");
            builder.AppendLine($"损坏类型: {_corruptionType}");
            builder.AppendLine($"损坏位置: {(_corruptionOffset >= 0 ? $"0x{_corruptionOffset:X}" : "未知")}");
            
            if (_expectedChecksum.HasValue && _actualChecksum.HasValue)
            {
                builder.AppendLine($"期望校验和: 0x{_expectedChecksum.Value:X8}");
                builder.AppendLine($"实际校验和: 0x{_actualChecksum.Value:X8}");
            }
            
            if (_expectedMagicNumber.HasValue && _actualMagicNumber.HasValue)
            {
                builder.AppendLine($"期望魔数: 0x{_expectedMagicNumber.Value:X8}");
                builder.AppendLine($"实际魔数: 0x{_actualMagicNumber.Value:X8}");
            }
            
            if (_corruptedDataSample != null && _corruptedDataSample.Length > 0)
            {
                builder.AppendLine("损坏数据样本 (十六进制):");
                builder.AppendLine($"  {BitConverter.ToString(_corruptedDataSample).Replace("-", " ")}");
            }
            
            builder.AppendLine("可能的原因:");
            builder.AppendLine(GetPossibleCauses());
            
            return builder.ToString();
        }

        /// <summary>
        /// 获取可能的损坏原因
        /// </summary>
        private string GetPossibleCauses()
        {
            return _corruptionType switch
            {
                DataCorruptionType.ChecksumMismatch =>
                    "  - 数据在传输过程中被修改\n  - 存储介质故障\n  - 文件被意外编辑",
                DataCorruptionType.InvalidMagicNumber =>
                    "  - 文件不是有效的序列化数据\n  - 文件格式不正确\n  - 文件头被损坏",
                DataCorruptionType.DataTruncated =>
                    "  - 文件未完整保存\n  - 传输中断\n  - 磁盘空间不足",
                DataCorruptionType.UnexpectedEndOfData =>
                    "  - 数据不完整\n  - 读取超出数据边界\n  - 长度字段错误",
                DataCorruptionType.DecompressionFailed =>
                    "  - 压缩数据损坏\n  - 压缩算法不匹配\n  - 压缩字典丢失",
                DataCorruptionType.DecryptionFailed =>
                    "  - 加密密钥错误\n  - 加密数据损坏\n  - 加密算法不匹配",
                _ =>
                    "  - 数据文件损坏\n  - 版本不兼容\n  - 内存错误"
            };
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建校验和失败异常
        /// </summary>
        public static DataCorruptedException ChecksumFailed(uint expected, uint actual, long offset = -1)
        {
            return new DataCorruptedException(expected, actual, offset);
        }

        /// <summary>
        /// 创建无效魔数异常
        /// </summary>
        public static DataCorruptedException InvalidMagic(uint expected, uint actual)
        {
            return new DataCorruptedException(expected, actual);
        }

        /// <summary>
        /// 创建数据截断异常
        /// </summary>
        public static DataCorruptedException Truncated(long expectedLength, long actualLength)
        {
            return new DataCorruptedException(
                DataCorruptionType.DataTruncated,
                $"数据被截断: 期望 {expectedLength} 字节，实际 {actualLength} 字节",
                actualLength);
        }

        /// <summary>
        /// 创建意外结束异常
        /// </summary>
        public static DataCorruptedException UnexpectedEnd(long position, int expectedBytes)
        {
            return new DataCorruptedException(
                DataCorruptionType.UnexpectedEndOfData,
                $"在位置 {position} 处需要读取 {expectedBytes} 字节，但数据已结束",
                position);
        }

        /// <summary>
        /// 创建无效格式异常
        /// </summary>
        public static DataCorruptedException InvalidFormat(string detail, long offset = -1)
        {
            return new DataCorruptedException(
                DataCorruptionType.InvalidDataFormat,
                $"无效的数据格式: {detail}",
                offset);
        }

        #endregion
    }

    /// <summary>
    /// 数据损坏类型枚举
    /// </summary>
    public enum DataCorruptionType
    {
        /// <summary>
        /// 未知损坏
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 校验和不匹配
        /// </summary>
        ChecksumMismatch = 1,

        /// <summary>
        /// 无效的魔数
        /// </summary>
        InvalidMagicNumber = 2,

        /// <summary>
        /// 数据被截断
        /// </summary>
        DataTruncated = 3,

        /// <summary>
        /// 意外的数据结束
        /// </summary>
        UnexpectedEndOfData = 4,

        /// <summary>
        /// 无效的数据格式
        /// </summary>
        InvalidDataFormat = 5,

        /// <summary>
        /// 无效的数据头
        /// </summary>
        InvalidHeader = 6,

        /// <summary>
        /// 无效的类型码
        /// </summary>
        InvalidTypeCode = 7,

        /// <summary>
        /// 无效的长度值
        /// </summary>
        InvalidLength = 8,

        /// <summary>
        /// 无效的对象引用
        /// </summary>
        InvalidReference = 9,

        /// <summary>
        /// 解压失败
        /// </summary>
        DecompressionFailed = 10,

        /// <summary>
        /// 解密失败
        /// </summary>
        DecryptionFailed = 11
    }
}
