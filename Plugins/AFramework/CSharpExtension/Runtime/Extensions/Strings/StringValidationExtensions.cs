// ==========================================================
// 文件名：StringValidationExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Text.RegularExpressions
// ==========================================================

using System;
using System.Text.RegularExpressions;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串验证扩展方法集合
    /// </summary>
    public static class StringValidationExtensions
    {
        #region 基础验证

        /// <summary>
        /// 判断字符串是否为 null 或空
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串是否为 null、空或仅包含空白字符
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 判断字符串是否有值（非 null 且非空）
        /// </summary>
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        #endregion

        #region 格式验证

        /// <summary>
        /// 判断是否为有效的电子邮件地址
        /// </summary>
        public static bool IsValidEmail(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 判断是否为有效的 URL
        /// </summary>
        public static bool IsValidUrl(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return Uri.TryCreate(str, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// 判断是否为有效的 IP 地址
        /// </summary>
        public static bool IsValidIPAddress(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            const string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 判断是否为有效的手机号码（中国大陆）
        /// </summary>
        public static bool IsValidPhoneNumber(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            const string pattern = @"^1[3-9]\d{9}$";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 判断是否为有效的身份证号码（中国大陆）
        /// </summary>
        public static bool IsValidIDCard(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            // 18位身份证号码
            const string pattern = @"^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$";
            return Regex.IsMatch(str, pattern);
        }

        #endregion

        #region 内容验证

        /// <summary>
        /// 判断是否只包含数字
        /// </summary>
        public static bool IsNumeric(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return Regex.IsMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 判断是否只包含字母
        /// </summary>
        public static bool IsAlpha(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return Regex.IsMatch(str, @"^[a-zA-Z]+$");
        }

        /// <summary>
        /// 判断是否只包含字母和数字
        /// </summary>
        public static bool IsAlphaNumeric(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return Regex.IsMatch(str, @"^[a-zA-Z0-9]+$");
        }

        /// <summary>
        /// 判断是否只包含小写字母
        /// </summary>
        public static bool IsLowerCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return str == str.ToLower() && Regex.IsMatch(str, @"[a-z]");
        }

        /// <summary>
        /// 判断是否只包含大写字母
        /// </summary>
        public static bool IsUpperCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return str == str.ToUpper() && Regex.IsMatch(str, @"[A-Z]");
        }

        #endregion

        #region 长度验证

        /// <summary>
        /// 判断长度是否在指定范围内
        /// </summary>
        public static bool IsLengthInRange(this string str, int minLength, int maxLength)
        {
            if (str == null)
                return false;

            return str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 判断长度是否等于指定值
        /// </summary>
        public static bool IsLengthEqual(this string str, int length)
        {
            return str?.Length == length;
        }

        #endregion

        #region 正则验证

        /// <summary>
        /// 判断是否匹配指定的正则表达式
        /// </summary>
        public static bool IsMatch(this string str, string pattern)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
                return false;

            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 判断是否匹配指定的正则表达式（带选项）
        /// </summary>
        public static bool IsMatch(this string str, string pattern, RegexOptions options)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
                return false;

            return Regex.IsMatch(str, pattern, options);
        }

        #endregion
    }
}
