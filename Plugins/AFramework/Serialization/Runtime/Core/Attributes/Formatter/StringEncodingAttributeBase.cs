// ==========================================================
// 文件名：StringEncodingAttributeBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 字符串编码特性基类
    /// <para>为 UTF-8 和 UTF-16 字符串特性提供共同功能</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>提供字符串编码的通用配置</item>
    ///   <item>支持长度限制、null 处理、空白修剪等</item>
    ///   <item>支持长度前缀配置</item>
    /// </list>
    /// </remarks>
    public abstract class StringEncodingAttributeBase : Attribute
    {
        #region 常量

        /// <summary>无长度限制</summary>
        public const int NoLengthLimit = -1;

        #endregion

        #region 字段

        /// <summary>是否以 null 结尾</summary>
        protected bool _nullTerminated;

        /// <summary>是否修剪空白字符</summary>
        protected bool _trimWhitespace;

        /// <summary>是否允许 null 值</summary>
        protected bool _allowNull = true;

        /// <summary>null 值的替代字符串</summary>
        protected string _nullReplacement;

        /// <summary>是否使用长度前缀</summary>
        protected bool _useLengthPrefix = true;

        /// <summary>长度前缀字节数</summary>
        protected int _lengthPrefixBytes = 4;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置是否以 null 结尾
        /// <para>默认值：false</para>
        /// </summary>
        public bool NullTerminated
        {
            get => _nullTerminated;
            set => _nullTerminated = value;
        }

        /// <summary>
        /// 获取或设置是否修剪空白字符
        /// <para>默认值：false</para>
        /// </summary>
        public bool TrimWhitespace
        {
            get => _trimWhitespace;
            set => _trimWhitespace = value;
        }

        /// <summary>
        /// 获取或设置是否允许 null 值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowNull
        {
            get => _allowNull;
            set => _allowNull = value;
        }

        /// <summary>
        /// 获取或设置 null 值的替代字符串
        /// <para>当 AllowNull 为 false 时使用</para>
        /// </summary>
        public string NullReplacement
        {
            get => _nullReplacement;
            set => _nullReplacement = value;
        }

        /// <summary>
        /// 获取或设置是否使用长度前缀
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseLengthPrefix
        {
            get => _useLengthPrefix;
            set => _useLengthPrefix = value;
        }

        /// <summary>
        /// 获取或设置长度前缀字节数
        /// <para>默认值：4（支持最大 4GB）</para>
        /// <para>可选值：1, 2, 4</para>
        /// </summary>
        public int LengthPrefixBytes
        {
            get => _lengthPrefixBytes;
            set
            {
                if (value != 1 && value != 2 && value != 4)
                    throw new ArgumentException("长度前缀字节数必须是 1、2 或 4", nameof(value));
                _lengthPrefixBytes = value;
            }
        }

        /// <summary>获取编码类型</summary>
        public abstract StringEncoding Encoding { get; }

        #endregion

        #region 公共方法

        /// <summary>检查是否有长度限制</summary>
        public abstract bool HasLengthLimit();

        /// <summary>获取最大支持长度</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxSupportedLength()
        {
            return _lengthPrefixBytes switch
            {
                1 => byte.MaxValue,
                2 => ushort.MaxValue,
                4 => int.MaxValue,
                _ => int.MaxValue
            };
        }

        /// <summary>预处理字符串</summary>
        public string PreprocessString(string value)
        {
            if (value == null)
            {
                if (!_allowNull && _nullReplacement != null)
                    return _nullReplacement;
                return value;
            }

            if (_trimWhitespace)
                value = value.Trim();

            return value;
        }

        /// <summary>验证字符串长度</summary>
        public abstract bool ValidateLength(string value);

        /// <summary>计算编码后的字节长度</summary>
        public abstract int CalculateEncodedLength(string value);

        /// <summary>获取配置摘要信息</summary>
        public abstract string GetSummary();

        #endregion

        #region 辅助方法

        /// <summary>构建摘要信息的通用部分</summary>
        protected void AppendCommonSummary(System.Collections.Generic.List<string> parts)
        {
            if (_nullTerminated) parts.Add("NullTerm");
            if (_trimWhitespace) parts.Add("Trim");
            if (!_allowNull) parts.Add("NotNull");
            if (!_useLengthPrefix) parts.Add("NoPrefix");
            if (_lengthPrefixBytes != 4) parts.Add($"Prefix={_lengthPrefixBytes}B");
        }

        #endregion
    }
}
