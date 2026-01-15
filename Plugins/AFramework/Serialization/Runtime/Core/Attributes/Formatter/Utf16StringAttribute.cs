// ==========================================================
// 文件名：Utf16StringAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// UTF-16 字符串特性
    /// <para>指定字符串字段使用 UTF-16 编码进行序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>强制使用 UTF-16 编码序列化字符串</item>
    ///   <item>支持大端/小端字节序配置</item>
    ///   <item>支持 BOM（字节顺序标记）配置</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class ChineseText
    /// {
    ///     // 使用 UTF-16 编码（默认小端）
    ///     [Utf16String]
    ///     public string Text;
    ///     
    ///     // 使用大端字节序
    ///     [Utf16String(Endianness = Endianness.Big)]
    ///     public string BigEndianText;
    /// }
    /// </code>
    /// 
    /// <para><b>性能优势：</b></para>
    /// <list type="bullet">
    ///   <item>UTF-16 对中文等 BMP 字符更高效（2 字节/字符）</item>
    ///   <item>与 .NET 内部字符串表示一致，无需转换</item>
    ///   <item>适合中文、日文、韩文等亚洲语言文本</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class Utf16StringAttribute : StringEncodingAttributeBase
    {
        #region 常量

        /// <summary>默认最大字符数（32K 字符）</summary>
        public const int DefaultMaxCharCount = 32768;

        /// <summary>UTF-16 LE BOM 字节序列</summary>
        public static readonly byte[] BomLittleEndian = { 0xFF, 0xFE };

        /// <summary>UTF-16 BE BOM 字节序列</summary>
        public static readonly byte[] BomBigEndian = { 0xFE, 0xFF };

        #endregion

        #region 字段

        private int _maxCharCount = NoLengthLimit;
        private Endianness _endianness = Endianness.Little;
        private bool _includeBom;
        private bool _useZeroCopy;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置最大字符数
        /// <para>默认值：-1（无限制）</para>
        /// </summary>
        public int MaxCharCount
        {
            get => _maxCharCount;
            set => _maxCharCount = value;
        }

        /// <summary>
        /// 获取或设置字节序
        /// <para>默认值：<see cref="Endianness.Little"/></para>
        /// </summary>
        public Endianness Endianness
        {
            get => _endianness;
            set => _endianness = value;
        }

        /// <summary>
        /// 获取或设置是否包含 BOM
        /// <para>默认值：false</para>
        /// </summary>
        public bool IncludeBom
        {
            get => _includeBom;
            set => _includeBom = value;
        }

        /// <summary>
        /// 获取或设置是否使用零拷贝优化
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseZeroCopy
        {
            get => _useZeroCopy;
            set => _useZeroCopy = value;
        }

        /// <inheritdoc/>
        public override StringEncoding Encoding => _endianness == Endianness.Big
            ? StringEncoding.UTF16BE
            : StringEncoding.UTF16LE;

        #endregion

        #region 构造函数

        /// <summary>初始化 <see cref="Utf16StringAttribute"/> 的新实例</summary>
        public Utf16StringAttribute() { }

        /// <summary>初始化 <see cref="Utf16StringAttribute"/> 的新实例</summary>
        /// <param name="maxCharCount">最大字符数</param>
        public Utf16StringAttribute(int maxCharCount)
        {
            _maxCharCount = maxCharCount;
        }

        /// <summary>初始化 <see cref="Utf16StringAttribute"/> 的新实例</summary>
        /// <param name="endianness">字节序</param>
        public Utf16StringAttribute(Endianness endianness)
        {
            _endianness = endianness;
        }

        #endregion

        #region 公共方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool HasLengthLimit() => _maxCharCount > 0;

        /// <summary>获取 BOM 字节序列</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBom() => _endianness == Endianness.Big ? BomBigEndian : BomLittleEndian;

        /// <summary>获取最大支持字符数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxSupportedCharCount()
        {
            return _lengthPrefixBytes switch
            {
                1 => byte.MaxValue / 2,
                2 => ushort.MaxValue / 2,
                4 => int.MaxValue / 2,
                _ => int.MaxValue / 2
            };
        }

        /// <inheritdoc/>
        public override int CalculateEncodedLength(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            var length = value.Length * 2;
            if (_includeBom) length += 2;
            if (_nullTerminated) length += 2;
            return length;
        }

        /// <inheritdoc/>
        public override bool ValidateLength(string value)
        {
            if (!HasLengthLimit()) return true;
            if (string.IsNullOrEmpty(value)) return true;
            return value.Length <= _maxCharCount;
        }

        /// <summary>检查是否可以使用零拷贝</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanUseZeroCopy()
        {
            return _useZeroCopy &&
                   _endianness == Endianness.Little &&
                   BitConverter.IsLittleEndian;
        }

        /// <inheritdoc/>
        public override string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string>
            {
                _endianness == Endianness.Big ? "UTF-16BE" : "UTF-16LE"
            };

            if (HasLengthLimit()) parts.Add($"MaxChars={_maxCharCount}");
            if (_includeBom) parts.Add("BOM");
            if (_useZeroCopy) parts.Add("ZeroCopy");
            AppendCommonSummary(parts);

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var encoding = _endianness == Endianness.Big ? "BE" : "LE";
            return HasLengthLimit()
                ? $"[Utf16String({encoding}, MaxCharCount={_maxCharCount})]"
                : $"[Utf16String({encoding})]";
        }

        #endregion
    }
}
