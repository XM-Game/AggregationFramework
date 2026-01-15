// ==========================================================
// 文件名：Utf8StringAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// UTF-8 字符串特性
    /// <para>指定字符串字段使用 UTF-8 编码进行序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>强制使用 UTF-8 编码序列化字符串</item>
    ///   <item>支持 BOM（字节顺序标记）配置</item>
    ///   <item>支持最大长度限制</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class LocalizedText
    /// {
    ///     // 使用 UTF-8 编码
    ///     [Utf8String]
    ///     public string Text;
    ///     
    ///     // 限制最大长度
    ///     [Utf8String(MaxLength = 256)]
    ///     public string ShortText;
    /// }
    /// </code>
    /// 
    /// <para><b>性能优势：</b></para>
    /// <list type="bullet">
    ///   <item>UTF-8 对 ASCII 字符更紧凑（1 字节/字符）</item>
    ///   <item>适合英文为主的文本内容</item>
    ///   <item>网络传输和存储更高效</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class Utf8StringAttribute : StringEncodingAttributeBase
    {
        #region 常量

        /// <summary>默认最大长度（64KB）</summary>
        public const int DefaultMaxLength = 65536;

        /// <summary>UTF-8 BOM 字节序列</summary>
        public static readonly byte[] Bom = { 0xEF, 0xBB, 0xBF };

        #endregion

        #region 字段

        private int _maxLength = NoLengthLimit;
        private bool _includeBom;
        private bool _validateEncoding = true;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置最大长度（字节数）
        /// <para>默认值：-1（无限制）</para>
        /// </summary>
        public int MaxLength
        {
            get => _maxLength;
            set => _maxLength = value;
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
        /// 获取或设置是否验证 UTF-8 有效性
        /// <para>默认值：true</para>
        /// </summary>
        public bool ValidateEncoding
        {
            get => _validateEncoding;
            set => _validateEncoding = value;
        }

        /// <inheritdoc/>
        public override StringEncoding Encoding => StringEncoding.UTF8;

        #endregion

        #region 构造函数

        /// <summary>初始化 <see cref="Utf8StringAttribute"/> 的新实例</summary>
        public Utf8StringAttribute() { }

        /// <summary>初始化 <see cref="Utf8StringAttribute"/> 的新实例</summary>
        /// <param name="maxLength">最大长度（字节数）</param>
        public Utf8StringAttribute(int maxLength)
        {
            _maxLength = maxLength;
        }

        #endregion

        #region 公共方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool HasLengthLimit() => _maxLength > 0;

        /// <inheritdoc/>
        public override int CalculateEncodedLength(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            var length = System.Text.Encoding.UTF8.GetByteCount(value);
            if (_includeBom) length += Bom.Length;
            if (_nullTerminated) length += 1;
            return length;
        }

        /// <inheritdoc/>
        public override bool ValidateLength(string value)
        {
            if (!HasLengthLimit()) return true;
            return CalculateEncodedLength(value) <= _maxLength;
        }

        /// <inheritdoc/>
        public override string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { "UTF-8" };

            if (HasLengthLimit()) parts.Add($"MaxLen={_maxLength}");
            if (_includeBom) parts.Add("BOM");
            AppendCommonSummary(parts);

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasLengthLimit()
                ? $"[Utf8String(MaxLength={_maxLength})]"
                : "[Utf8String]";
        }

        #endregion
    }
}
