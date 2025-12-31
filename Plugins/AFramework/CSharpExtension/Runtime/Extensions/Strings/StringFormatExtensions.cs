// ==========================================================
// 文件名：StringFormatExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Globalization;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串格式化扩展方法集合
    /// </summary>
    public static class StringFormatExtensions
    {
        #region 大小写转换

        /// <summary>
        /// 转换为首字母大写
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// 转换为驼峰命名（camelCase）
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            str = str.Trim();
            if (str.Length == 0)
                return str;

            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 转换为帕斯卡命名（PascalCase）
        /// </summary>
        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            str = str.Trim();
            if (str.Length == 0)
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 转换为蛇形命名（snake_case）
        /// </summary>
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsUpper(c) && i > 0)
                {
                    sb.Append('_');
                }
                sb.Append(char.ToLower(c));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 转换为短横线命名（kebab-case）
        /// </summary>
        public static string ToKebabCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsUpper(c) && i > 0)
                {
                    sb.Append('-');
                }
                sb.Append(char.ToLower(c));
            }

            return sb.ToString();
        }

        #endregion

        #region 截断和填充

        /// <summary>
        /// 截断字符串到指定长度，超出部分用省略号替代
        /// </summary>
        public static string Truncate(this string str, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), "最大长度必须大于 0");

            if (str.Length <= maxLength)
                return str;

            int truncateLength = maxLength - suffix.Length;
            if (truncateLength <= 0)
                return suffix.Substring(0, maxLength);

            return str.Substring(0, truncateLength) + suffix;
        }

        /// <summary>
        /// 左填充到指定长度
        /// </summary>
        public static string PadLeft(this string str, int totalWidth, char paddingChar = ' ')
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.PadLeft(totalWidth, paddingChar);
        }

        /// <summary>
        /// 右填充到指定长度
        /// </summary>
        public static string PadRight(this string str, int totalWidth, char paddingChar = ' ')
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.PadRight(totalWidth, paddingChar);
        }

        /// <summary>
        /// 居中对齐（两侧填充）
        /// </summary>
        public static string PadCenter(this string str, int totalWidth, char paddingChar = ' ')
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length >= totalWidth)
                return str;

            int leftPadding = (totalWidth - str.Length) / 2;
            int rightPadding = totalWidth - str.Length - leftPadding;

            return new string(paddingChar, leftPadding) + str + new string(paddingChar, rightPadding);
        }

        #endregion

        #region 移除操作

        /// <summary>
        /// 移除所有空白字符
        /// </summary>
        public static string RemoveWhiteSpace(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 移除指定字符
        /// </summary>
        public static string Remove(this string str, params char[] chars)
        {
            if (string.IsNullOrEmpty(str) || chars == null || chars.Length == 0)
                return str;

            var sb = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                bool shouldRemove = false;
                foreach (char removeChar in chars)
                {
                    if (c == removeChar)
                    {
                        shouldRemove = true;
                        break;
                    }
                }

                if (!shouldRemove)
                    sb.Append(c);
            }

            return sb.ToString();
        }

        #endregion

        #region 重复和反转

        /// <summary>
        /// 重复字符串 n 次
        /// </summary>
        public static string Repeat(this string str, int count)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "重复次数不能为负数");

            if (count == 0 || str.Length == 0)
                return string.Empty;

            var sb = new StringBuilder(str.Length * count);
            for (int i = 0; i < count; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        public static string Reverse(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        #endregion

        #region 格式化

        /// <summary>
        /// 格式化字符串（类似 string.Format）
        /// </summary>
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            return string.Format(format, args);
        }

        /// <summary>
        /// 如果字符串为空则返回默认值
        /// </summary>
        public static string IfEmpty(this string str, string defaultValue)
        {
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        /// <summary>
        /// 如果字符串为空或空白则返回默认值
        /// </summary>
        public static string IfWhiteSpace(this string str, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(str) ? defaultValue : str;
        }

        #endregion
    }
}
