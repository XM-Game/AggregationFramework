// ==========================================================
// 文件名：StringExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串扩展方法
    /// <para>提供字符串的常用操作扩展，包括验证、转换、格式化、搜索等功能</para>
    /// </summary>
    public static class StringExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查字符串是否为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 检查字符串是否为 null、空或仅包含空白字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 检查字符串是否有内容（非 null 且非空）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 检查字符串是否有有效内容（非 null、非空且非空白）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValidValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 如果字符串为 null 或空，返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string OrDefault(this string str, string defaultValue = "")
        {
            return str.IsNullOrEmpty() ? defaultValue : str;
        }

        /// <summary>
        /// 如果字符串为 null、空或空白，返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string OrDefaultIfWhiteSpace(this string str, string defaultValue = "")
        {
            return str.IsNullOrWhiteSpace() ? defaultValue : str;
        }

        #endregion

        #region 比较操作

        /// <summary>
        /// 忽略大小写比较
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsIgnoreCase(this string str, string other)
        {
            return string.Equals(str, other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否包含指定字符串（忽略大小写）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 检查是否以指定字符串开头（忽略大小写）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;
            return str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否以指定字符串结尾（忽略大小写）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;
            return str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region 截取操作

        /// <summary>
        /// 安全截取字符串（不会越界）
        /// </summary>
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (str.IsNullOrEmpty() || startIndex < 0)
                return string.Empty;

            if (startIndex >= str.Length)
                return string.Empty;

            length = Math.Min(length, str.Length - startIndex);
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 安全截取字符串（从起始位置到结尾）
        /// </summary>
        public static string SafeSubstring(this string str, int startIndex)
        {
            if (str.IsNullOrEmpty() || startIndex < 0)
                return string.Empty;

            if (startIndex >= str.Length)
                return string.Empty;

            return str.Substring(startIndex);
        }

        /// <summary>
        /// 获取前 N 个字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Left(this string str, int length)
        {
            return str.SafeSubstring(0, length);
        }

        /// <summary>
        /// 获取后 N 个字符
        /// </summary>
        public static string Right(this string str, int length)
        {
            if (str.IsNullOrEmpty() || length <= 0)
                return string.Empty;

            if (length >= str.Length)
                return str;

            return str.Substring(str.Length - length);
        }

        /// <summary>
        /// 截断字符串到指定长度，并添加省略号
        /// </summary>
        public static string Truncate(this string str, int maxLength, string ellipsis = "...")
        {
            if (str.IsNullOrEmpty() || str.Length <= maxLength)
                return str;

            int truncateLength = Math.Max(0, maxLength - ellipsis.Length);
            return str.Substring(0, truncateLength) + ellipsis;
        }

        #endregion

        #region 移除操作

        /// <summary>
        /// 移除前缀
        /// </summary>
        public static string RemovePrefix(this string str, string prefix)
        {
            if (str.IsNullOrEmpty() || prefix.IsNullOrEmpty())
                return str;

            return str.StartsWith(prefix) ? str.Substring(prefix.Length) : str;
        }

        /// <summary>
        /// 移除后缀
        /// </summary>
        public static string RemoveSuffix(this string str, string suffix)
        {
            if (str.IsNullOrEmpty() || suffix.IsNullOrEmpty())
                return str;

            return str.EndsWith(suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        }

        /// <summary>
        /// 移除所有空白字符
        /// </summary>
        public static string RemoveWhiteSpace(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            return Regex.Replace(str, @"\s+", string.Empty);
        }

        /// <summary>
        /// 移除所有指定字符
        /// </summary>
        public static string Remove(this string str, params char[] chars)
        {
            if (str.IsNullOrEmpty() || chars == null || chars.Length == 0)
                return str;

            var sb = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                if (Array.IndexOf(chars, c) < 0)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        #endregion

        #region 替换操作

        /// <summary>
        /// 替换字符串（忽略大小写）
        /// </summary>
        public static string ReplaceIgnoreCase(this string str, string oldValue, string newValue)
        {
            if (str.IsNullOrEmpty() || oldValue.IsNullOrEmpty())
                return str;

            return Regex.Replace(str, Regex.Escape(oldValue), newValue ?? string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 替换多个字符串
        /// </summary>
        public static string ReplaceMultiple(this string str, string[] oldValues, string newValue)
        {
            if (str.IsNullOrEmpty() || oldValues == null || oldValues.Length == 0)
                return str;

            foreach (var oldValue in oldValues)
            {
                if (!oldValue.IsNullOrEmpty())
                    str = str.Replace(oldValue, newValue ?? string.Empty);
            }
            return str;
        }

        #endregion

        #region 格式化操作

        /// <summary>
        /// 格式化字符串（类似 string.Format）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 转换为首字母大写
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// 转换为驼峰命名（camelCase）
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            if (str.Length == 1)
                return str.ToLower();

            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 转换为帕斯卡命名（PascalCase）
        /// </summary>
        public static string ToPascalCase(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            if (str.Length == 1)
                return str.ToUpper();

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 转换为蛇形命名（snake_case）
        /// </summary>
        public static string ToSnakeCase(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            return Regex.Replace(str, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        /// <summary>
        /// 转换为短横线命名（kebab-case）
        /// </summary>
        public static string ToKebabCase(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            return Regex.Replace(str, @"([a-z0-9])([A-Z])", "$1-$2").ToLower();
        }

        #endregion

        #region 反转和重复

        /// <summary>
        /// 反转字符串
        /// </summary>
        public static string Reverse(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;

            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// 重复字符串指定次数
        /// </summary>
        public static string Repeat(this string str, int count)
        {
            if (str.IsNullOrEmpty() || count <= 0)
                return string.Empty;

            if (count == 1)
                return str;

            var sb = new StringBuilder(str.Length * count);
            for (int i = 0; i < count; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }

        #endregion

        #region 分割操作

        /// <summary>
        /// 分割字符串并移除空项
        /// </summary>
        public static string[] SplitAndRemoveEmpty(this string str, params char[] separators)
        {
            if (str.IsNullOrEmpty())
                return Array.Empty<string>();

            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 分割字符串并移除空白项
        /// </summary>
        public static string[] SplitAndTrim(this string str, params char[] separators)
        {
            if (str.IsNullOrEmpty())
                return Array.Empty<string>();

            var parts = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }
            return parts;
        }

        /// <summary>
        /// 按行分割
        /// </summary>
        public static string[] SplitLines(this string str)
        {
            if (str.IsNullOrEmpty())
                return Array.Empty<string>();

            return str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        #endregion

        #region 类型转换

        /// <summary>
        /// 尝试转换为 int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToInt(this string str, out int result)
        {
            return int.TryParse(str, out result);
        }

        /// <summary>
        /// 转换为 int，失败返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this string str, int defaultValue = 0)
        {
            return int.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 尝试转换为 float
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToFloat(this string str, out float result)
        {
            return float.TryParse(str, out result);
        }

        /// <summary>
        /// 转换为 float，失败返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this string str, float defaultValue = 0f)
        {
            return float.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 尝试转换为 bool
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToBool(this string str, out bool result)
        {
            return bool.TryParse(str, out result);
        }

        /// <summary>
        /// 转换为 bool，失败返回默认值
        /// </summary>
        public static bool ToBool(this string str, bool defaultValue = false)
        {
            if (str.IsNullOrWhiteSpace())
                return defaultValue;

            str = str.Trim().ToLower();
            if (str == "1" || str == "true" || str == "yes" || str == "on")
                return true;
            if (str == "0" || str == "false" || str == "no" || str == "off")
                return false;

            return defaultValue;
        }

        /// <summary>
        /// 尝试转换为枚举
        /// </summary>
        public static bool TryToEnum<TEnum>(this string str, out TEnum result) where TEnum : struct
        {
            return Enum.TryParse(str, true, out result);
        }

        /// <summary>
        /// 转换为枚举，失败返回默认值
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string str, TEnum defaultValue = default) where TEnum : struct
        {
            return Enum.TryParse(str, true, out TEnum result) ? result : defaultValue;
        }

        #endregion

        #region 编码操作

        /// <summary>
        /// 转换为字节数组（UTF-8）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this string str)
        {
            return str.IsNullOrEmpty() ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 转换为字节数组（指定编码）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this string str, Encoding encoding)
        {
            return str.IsNullOrEmpty() ? Array.Empty<byte>() : encoding.GetBytes(str);
        }

        /// <summary>
        /// 转换为 Base64 字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBase64(this string str)
        {
            return str.IsNullOrEmpty() ? string.Empty : Convert.ToBase64String(str.ToBytes());
        }

        /// <summary>
        /// 从 Base64 字符串解码
        /// </summary>
        public static string FromBase64(this string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            try
            {
                byte[] bytes = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region 验证操作

        /// <summary>
        /// 检查是否为数字
        /// </summary>
        public static bool IsNumeric(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            return double.TryParse(str, out _);
        }

        /// <summary>
        /// 检查是否为整数
        /// </summary>
        public static bool IsInteger(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            return int.TryParse(str, out _);
        }

        /// <summary>
        /// 检查是否为有效的电子邮件地址
        /// </summary>
        public static bool IsValidEmail(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为有效的 URL
        /// </summary>
        public static bool IsValidUrl(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return false;

            return Uri.TryCreate(str, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// 检查是否只包含字母
        /// </summary>
        public static bool IsAlpha(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            foreach (char c in str)
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否只包含字母和数字
        /// </summary>
        public static bool IsAlphaNumeric(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;

            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }

        #endregion

        #region 哈希操作

        /// <summary>
        /// 计算 MD5 哈希
        /// </summary>
        public static string ToMD5(this string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(str.ToBytes());
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 计算 SHA256 哈希
        /// </summary>
        public static string ToSHA256(this string str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(str.ToBytes());
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        #endregion

        #region 其他操作

        /// <summary>
        /// 计算字符串中指定子串的出现次数
        /// </summary>
        public static int CountOccurrences(this string str, string substring)
        {
            if (str.IsNullOrEmpty() || substring.IsNullOrEmpty())
                return 0;

            int count = 0;
            int index = 0;
            while ((index = str.IndexOf(substring, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += substring.Length;
            }
            return count;
        }

        /// <summary>
        /// 获取字符串的字节长度（UTF-8）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteCount(this string str)
        {
            return str.IsNullOrEmpty() ? 0 : Encoding.UTF8.GetByteCount(str);
        }

        #endregion
    }
}
