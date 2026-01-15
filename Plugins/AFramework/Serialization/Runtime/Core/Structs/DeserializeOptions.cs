// ==========================================================
// 文件名：DeserializeOptions.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 反序列化选项结构体
    /// <para>配置反序列化行为的各种选项参数</para>
    /// <para>支持流畅 API 和 Builder 模式构建</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认选项
    /// var options = DeserializeOptions.Default;
    /// 
    /// // 使用 Builder 模式
    /// var options = DeserializeOptions.Create()
    ///     .WithMaxDepth(128)
    ///     .WithVersionTolerance(true)
    ///     .WithTypeResolver(customResolver)
    ///     .Build();
    /// 
    /// // 使用预设配置
    /// var safeOptions = DeserializeOptions.Safe;
    /// var strictOptions = DeserializeOptions.Strict;
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct DeserializeOptions : IEquatable<DeserializeOptions>
    {
        #region 字段

        private readonly int _maxDepth;
        private readonly int _maxObjectCount;
        private readonly int _maxStringLength;
        private readonly int _maxCollectionCount;
        private readonly int _timeoutMs;
        private readonly StringEncoding _stringEncoding;
        private readonly Endianness _expectedEndianness;
        private readonly DeserializeFlags _flags;
        private readonly byte[] _decryptionKey;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建反序列化选项
        /// </summary>
        private DeserializeOptions(
            int maxDepth,
            int maxObjectCount,
            int maxStringLength,
            int maxCollectionCount,
            int timeoutMs,
            StringEncoding stringEncoding,
            Endianness expectedEndianness,
            DeserializeFlags flags,
            byte[] decryptionKey)
        {
            _maxDepth = maxDepth;
            _maxObjectCount = maxObjectCount;
            _maxStringLength = maxStringLength;
            _maxCollectionCount = maxCollectionCount;
            _timeoutMs = timeoutMs;
            _stringEncoding = stringEncoding;
            _expectedEndianness = expectedEndianness;
            _flags = flags;
            _decryptionKey = decryptionKey;
        }

        #endregion

        #region 属性

        /// <summary>最大反序列化深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>最大对象数量</summary>
        public int MaxObjectCount => _maxObjectCount;

        /// <summary>最大字符串长度</summary>
        public int MaxStringLength => _maxStringLength;

        /// <summary>最大集合元素数量</summary>
        public int MaxCollectionCount => _maxCollectionCount;

        /// <summary>超时时间 (毫秒)</summary>
        public int TimeoutMs => _timeoutMs;

        /// <summary>字符串编码</summary>
        public StringEncoding StringEncoding => _stringEncoding;

        /// <summary>期望的字节序</summary>
        public Endianness ExpectedEndianness => _expectedEndianness;

        /// <summary>反序列化标志</summary>
        public DeserializeFlags Flags => _flags;

        /// <summary>解密密钥 (只读)</summary>
        public ReadOnlySpan<byte> DecryptionKey => _decryptionKey ?? Array.Empty<byte>();

        /// <summary>是否有解密密钥</summary>
        public bool HasDecryptionKey => _decryptionKey != null && _decryptionKey.Length > 0;

        #endregion

        #region 标志属性

        /// <summary>是否启用版本容错</summary>
        public bool EnableVersionTolerance => (_flags & DeserializeFlags.EnableVersionTolerance) != 0;

        /// <summary>是否验证校验和</summary>
        public bool ValidateChecksum => (_flags & DeserializeFlags.ValidateChecksum) != 0;

        /// <summary>是否忽略未知字段</summary>
        public bool IgnoreUnknownFields => (_flags & DeserializeFlags.IgnoreUnknownFields) != 0;

        /// <summary>是否使用默认值填充缺失字段</summary>
        public bool UseDefaultForMissing => (_flags & DeserializeFlags.UseDefaultForMissing) != 0;

        /// <summary>是否启用类型验证</summary>
        public bool EnableTypeValidation => (_flags & DeserializeFlags.EnableTypeValidation) != 0;

        /// <summary>是否允许不安全类型</summary>
        public bool AllowUnsafeTypes => (_flags & DeserializeFlags.AllowUnsafeTypes) != 0;

        /// <summary>是否启用延迟加载</summary>
        public bool EnableLazyLoading => (_flags & DeserializeFlags.EnableLazyLoading) != 0;

        /// <summary>是否启用进度回调</summary>
        public bool EnableProgressCallback => (_flags & DeserializeFlags.EnableProgressCallback) != 0;

        /// <summary>是否启用取消支持</summary>
        public bool EnableCancellation => (_flags & DeserializeFlags.EnableCancellation) != 0;

        /// <summary>是否为严格模式</summary>
        public bool IsStrictMode => (_flags & DeserializeFlags.StrictMode) != 0;

        /// <summary>是否为宽松模式</summary>
        public bool IsLenientMode => (_flags & DeserializeFlags.LenientMode) != 0;

        #endregion

        #region 预设配置

        /// <summary>
        /// 默认选项
        /// <para>平衡安全性和性能</para>
        /// </summary>
        public static DeserializeOptions Default => new DeserializeOptions(
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            maxStringLength: SerializeConstants.SizeLimit.MaxStringLength,
            maxCollectionCount: SerializeConstants.SizeLimit.MaxCollectionCount,
            timeoutMs: SerializeConstants.Timeout.DefaultDeserializeMs,
            stringEncoding: StringEncoding.UTF8,
            expectedEndianness: Endianness.Native,
            flags: DeserializeFlags.ValidateChecksum | DeserializeFlags.EnableTypeValidation,
            decryptionKey: null
        );

        /// <summary>
        /// 安全选项
        /// <para>严格限制，防止恶意数据攻击</para>
        /// </summary>
        public static DeserializeOptions Safe => new DeserializeOptions(
            maxDepth: 32,
            maxObjectCount: 10000,
            maxStringLength: 1024 * 1024, // 1MB
            maxCollectionCount: 10000,
            timeoutMs: SerializeConstants.Timeout.FastOperationMs,
            stringEncoding: StringEncoding.UTF8,
            expectedEndianness: Endianness.Little,
            flags: DeserializeFlags.ValidateChecksum | DeserializeFlags.EnableTypeValidation | DeserializeFlags.StrictMode,
            decryptionKey: null
        );

        /// <summary>
        /// 严格选项
        /// <para>遇到任何问题立即报错</para>
        /// </summary>
        public static DeserializeOptions Strict => new DeserializeOptions(
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            maxStringLength: SerializeConstants.SizeLimit.MaxStringLength,
            maxCollectionCount: SerializeConstants.SizeLimit.MaxCollectionCount,
            timeoutMs: SerializeConstants.Timeout.DefaultDeserializeMs,
            stringEncoding: StringEncoding.UTF8,
            expectedEndianness: Endianness.Native,
            flags: DeserializeFlags.ValidateChecksum | DeserializeFlags.EnableTypeValidation | DeserializeFlags.StrictMode,
            decryptionKey: null
        );

        /// <summary>
        /// 宽松选项
        /// <para>尽可能忽略错误，恢复数据</para>
        /// </summary>
        public static DeserializeOptions Lenient => new DeserializeOptions(
            maxDepth: SerializeConstants.MaxSerializeDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            maxStringLength: SerializeConstants.SizeLimit.MaxStringLength,
            maxCollectionCount: SerializeConstants.SizeLimit.MaxCollectionCount,
            timeoutMs: SerializeConstants.Timeout.Infinite,
            stringEncoding: StringEncoding.UTF8,
            expectedEndianness: Endianness.Native,
            flags: DeserializeFlags.EnableVersionTolerance | DeserializeFlags.IgnoreUnknownFields | 
                   DeserializeFlags.UseDefaultForMissing | DeserializeFlags.LenientMode,
            decryptionKey: null
        );

        /// <summary>
        /// 版本容错选项
        /// <para>支持数据版本迁移</para>
        /// </summary>
        public static DeserializeOptions VersionTolerant => new DeserializeOptions(
            maxDepth: SerializeConstants.DefaultMaxDepth,
            maxObjectCount: SerializeConstants.SizeLimit.MaxObjectCount,
            maxStringLength: SerializeConstants.SizeLimit.MaxStringLength,
            maxCollectionCount: SerializeConstants.SizeLimit.MaxCollectionCount,
            timeoutMs: SerializeConstants.Timeout.DefaultDeserializeMs,
            stringEncoding: StringEncoding.UTF8,
            expectedEndianness: Endianness.Native,
            flags: DeserializeFlags.EnableVersionTolerance | DeserializeFlags.IgnoreUnknownFields | 
                   DeserializeFlags.UseDefaultForMissing | DeserializeFlags.ValidateChecksum,
            decryptionKey: null
        );

        #endregion

        #region Builder 模式

        /// <summary>
        /// 创建选项构建器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeserializeOptionsBuilder Create() => new DeserializeOptionsBuilder();

        /// <summary>
        /// 从现有选项创建构建器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DeserializeOptionsBuilder ToBuilder() => new DeserializeOptionsBuilder(this);

        #endregion

        #region With 方法 (不可变修改)

        /// <summary>修改最大深度</summary>
        public DeserializeOptions WithMaxDepth(int maxDepth) =>
            new DeserializeOptions(maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改最大对象数量</summary>
        public DeserializeOptions WithMaxObjectCount(int maxObjectCount) =>
            new DeserializeOptions(_maxDepth, maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改最大字符串长度</summary>
        public DeserializeOptions WithMaxStringLength(int maxStringLength) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改最大集合数量</summary>
        public DeserializeOptions WithMaxCollectionCount(int maxCollectionCount) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改超时时间</summary>
        public DeserializeOptions WithTimeout(int timeoutMs) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改字符串编码</summary>
        public DeserializeOptions WithStringEncoding(StringEncoding encoding) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, encoding, _expectedEndianness, _flags, _decryptionKey);

        /// <summary>修改期望字节序</summary>
        public DeserializeOptions WithExpectedEndianness(Endianness endianness) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, endianness, _flags, _decryptionKey);

        /// <summary>修改标志</summary>
        public DeserializeOptions WithFlags(DeserializeFlags flags) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, flags, _decryptionKey);

        /// <summary>添加标志</summary>
        public DeserializeOptions AddFlags(DeserializeFlags flags) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags | flags, _decryptionKey);

        /// <summary>移除标志</summary>
        public DeserializeOptions RemoveFlags(DeserializeFlags flags) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags & ~flags, _decryptionKey);

        /// <summary>设置解密密钥</summary>
        public DeserializeOptions WithDecryptionKey(byte[] key) =>
            new DeserializeOptions(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, key);

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        public bool IsValid() => IsValid(out _);

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (!SerializeConstants.IsValidDepth(_maxDepth))
            {
                errorMessage = $"最大深度 {_maxDepth} 超出有效范围";
                return false;
            }

            if (_maxObjectCount <= 0)
            {
                errorMessage = "最大对象数量必须大于 0";
                return false;
            }

            if (_maxStringLength <= 0)
            {
                errorMessage = "最大字符串长度必须大于 0";
                return false;
            }

            if (_maxCollectionCount <= 0)
            {
                errorMessage = "最大集合数量必须大于 0";
                return false;
            }

            if (_timeoutMs < -1)
            {
                errorMessage = "超时时间必须大于等于 -1";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证数据大小是否在限制范围内
        /// </summary>
        /// <param name="stringLength">字符串长度</param>
        /// <param name="collectionCount">集合数量</param>
        /// <param name="depth">当前深度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ValidateLimits(int stringLength, int collectionCount, int depth)
        {
            return stringLength <= _maxStringLength &&
                   collectionCount <= _maxCollectionCount &&
                   depth <= _maxDepth;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(DeserializeOptions other)
        {
            return _maxDepth == other._maxDepth &&
                   _maxObjectCount == other._maxObjectCount &&
                   _maxStringLength == other._maxStringLength &&
                   _maxCollectionCount == other._maxCollectionCount &&
                   _timeoutMs == other._timeoutMs &&
                   _stringEncoding == other._stringEncoding &&
                   _expectedEndianness == other._expectedEndianness &&
                   _flags == other._flags;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is DeserializeOptions other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags);
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(DeserializeOptions left, DeserializeOptions right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(DeserializeOptions left, DeserializeOptions right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"DeserializeOptions(MaxDepth={_maxDepth}, Flags={_flags})";
        }

        #endregion

        #region 内部构建方法

        /// <summary>内部构建方法</summary>
        internal static DeserializeOptions Build(
            int maxDepth, int maxObjectCount, int maxStringLength, int maxCollectionCount,
            int timeoutMs, StringEncoding stringEncoding, Endianness expectedEndianness,
            DeserializeFlags flags, byte[] decryptionKey)
        {
            return new DeserializeOptions(maxDepth, maxObjectCount, maxStringLength, maxCollectionCount,
                timeoutMs, stringEncoding, expectedEndianness, flags, decryptionKey);
        }

        #endregion
    }

    #region 反序列化标志枚举

    /// <summary>
    /// 反序列化标志枚举
    /// <para>定义反序列化行为的各种开关选项</para>
    /// </summary>
    [Flags]
    public enum DeserializeFlags : uint
    {
        /// <summary>无标志</summary>
        None = 0,

        /// <summary>启用版本容错</summary>
        EnableVersionTolerance = 1 << 0,

        /// <summary>验证校验和</summary>
        ValidateChecksum = 1 << 1,

        /// <summary>忽略未知字段</summary>
        IgnoreUnknownFields = 1 << 2,

        /// <summary>使用默认值填充缺失字段</summary>
        UseDefaultForMissing = 1 << 3,

        /// <summary>启用类型验证</summary>
        EnableTypeValidation = 1 << 4,

        /// <summary>允许不安全类型</summary>
        AllowUnsafeTypes = 1 << 5,

        /// <summary>启用延迟加载</summary>
        EnableLazyLoading = 1 << 6,

        /// <summary>启用进度回调</summary>
        EnableProgressCallback = 1 << 7,

        /// <summary>启用取消支持</summary>
        EnableCancellation = 1 << 8,

        /// <summary>严格模式</summary>
        StrictMode = 1 << 9,

        /// <summary>宽松模式</summary>
        LenientMode = 1 << 10,

        /// <summary>自动检测字节序</summary>
        AutoDetectEndianness = 1 << 11,

        /// <summary>自动检测编码</summary>
        AutoDetectEncoding = 1 << 12,

        /// <summary>启用数据修复</summary>
        EnableDataRepair = 1 << 13,

        /// <summary>跳过损坏数据</summary>
        SkipCorruptedData = 1 << 14
    }

    #endregion

    #region 选项构建器

    /// <summary>
    /// 反序列化选项构建器
    /// </summary>
    public sealed class DeserializeOptionsBuilder
    {
        private int _maxDepth = SerializeConstants.DefaultMaxDepth;
        private int _maxObjectCount = SerializeConstants.SizeLimit.MaxObjectCount;
        private int _maxStringLength = SerializeConstants.SizeLimit.MaxStringLength;
        private int _maxCollectionCount = SerializeConstants.SizeLimit.MaxCollectionCount;
        private int _timeoutMs = SerializeConstants.Timeout.DefaultDeserializeMs;
        private StringEncoding _stringEncoding = StringEncoding.UTF8;
        private Endianness _expectedEndianness = Endianness.Native;
        private DeserializeFlags _flags = DeserializeFlags.ValidateChecksum | DeserializeFlags.EnableTypeValidation;
        private byte[] _decryptionKey;

        /// <summary>创建默认构建器</summary>
        public DeserializeOptionsBuilder() { }

        /// <summary>从现有选项创建构建器</summary>
        public DeserializeOptionsBuilder(DeserializeOptions options)
        {
            _maxDepth = options.MaxDepth;
            _maxObjectCount = options.MaxObjectCount;
            _maxStringLength = options.MaxStringLength;
            _maxCollectionCount = options.MaxCollectionCount;
            _timeoutMs = options.TimeoutMs;
            _stringEncoding = options.StringEncoding;
            _expectedEndianness = options.ExpectedEndianness;
            _flags = options.Flags;
            _decryptionKey = options.HasDecryptionKey ? options.DecryptionKey.ToArray() : null;
        }

        /// <summary>设置最大深度</summary>
        public DeserializeOptionsBuilder WithMaxDepth(int maxDepth) { _maxDepth = maxDepth; return this; }

        /// <summary>设置最大对象数量</summary>
        public DeserializeOptionsBuilder WithMaxObjectCount(int count) { _maxObjectCount = count; return this; }

        /// <summary>设置最大字符串长度</summary>
        public DeserializeOptionsBuilder WithMaxStringLength(int length) { _maxStringLength = length; return this; }

        /// <summary>设置最大集合数量</summary>
        public DeserializeOptionsBuilder WithMaxCollectionCount(int count) { _maxCollectionCount = count; return this; }

        /// <summary>设置超时时间</summary>
        public DeserializeOptionsBuilder WithTimeout(int timeoutMs) { _timeoutMs = timeoutMs; return this; }

        /// <summary>设置字符串编码</summary>
        public DeserializeOptionsBuilder WithStringEncoding(StringEncoding encoding) { _stringEncoding = encoding; return this; }

        /// <summary>设置期望字节序</summary>
        public DeserializeOptionsBuilder WithExpectedEndianness(Endianness endianness) { _expectedEndianness = endianness; return this; }

        /// <summary>设置标志</summary>
        public DeserializeOptionsBuilder WithFlags(DeserializeFlags flags) { _flags = flags; return this; }

        /// <summary>添加标志</summary>
        public DeserializeOptionsBuilder AddFlags(DeserializeFlags flags) { _flags |= flags; return this; }

        /// <summary>移除标志</summary>
        public DeserializeOptionsBuilder RemoveFlags(DeserializeFlags flags) { _flags &= ~flags; return this; }

        /// <summary>设置解密密钥</summary>
        public DeserializeOptionsBuilder WithDecryptionKey(byte[] key) { _decryptionKey = key; return this; }

        /// <summary>启用版本容错</summary>
        public DeserializeOptionsBuilder EnableVersionTolerance() => AddFlags(DeserializeFlags.EnableVersionTolerance);

        /// <summary>启用严格模式</summary>
        public DeserializeOptionsBuilder EnableStrictMode() => AddFlags(DeserializeFlags.StrictMode);

        /// <summary>启用宽松模式</summary>
        public DeserializeOptionsBuilder EnableLenientMode() => AddFlags(DeserializeFlags.LenientMode);

        /// <summary>忽略未知字段</summary>
        public DeserializeOptionsBuilder IgnoreUnknownFields() => AddFlags(DeserializeFlags.IgnoreUnknownFields);

        /// <summary>构建选项</summary>
        public DeserializeOptions Build()
        {
            return DeserializeOptions.Build(_maxDepth, _maxObjectCount, _maxStringLength, _maxCollectionCount,
                _timeoutMs, _stringEncoding, _expectedEndianness, _flags, _decryptionKey);
        }
    }

    #endregion
}
