// ==========================================================
// 文件名：StringConversionExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Globalization;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串转换扩展方法集合
    /// </summary>
    public static class StringConversionExtensions
    {
        #region 数值转换

        /// <summary>
        /// 转换为 int，失败返回默认值
        /// </summary>
        public static int ToInt(this string str, int defaultValue = 0)
        {
            return int.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 long，失败返回默认值
        /// </summary>
        public static long ToLong(this string str, long defaultValue = 0)
        {
            return long.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 float，失败返回默认值
        /// </summary>
        public static float ToFloat(this string str, float defaultValue = 0f)
        {
            return float.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 double，失败返回默认值
        /// </summary>
        public static double ToDouble(this string str, double defaultValue = 0.0)
        {
            return double.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 decimal，失败返回默认值
        /// </summary>
        public static decimal ToDecimal(this string str, decimal defaultValue = 0m)
        {
            return decimal.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 bool，失败返回默认值
        /// </summary>
        public static bool ToBool(this string str, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;

            str = str.Trim().ToLower();

            // 支持多种格式
            if (str == "1" || str == "true" || str == "yes" || str == "是")
                return true;

            if (str == "0" || str == "false" || str == "no" || str == "否")
                return false;

            return bool.TryParse(str, out var result) ? result : defaultValue;
        }

        #endregion

        #region 日期时间转换

        /// <summary>
        /// 转换为 DateTime，失败返回默认值
        /// </summary>
        public static DateTime ToDateTime(this string str, DateTime defaultValue = default)
        {
            return DateTime.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 转换为 DateTime（指定格式），失败返回默认值
        /// </summary>
        public static DateTime ToDateTime(this string str, string format, DateTime defaultValue = default)
        {
            return DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out var result) ? result : defaultValue;
        }

        #endregion

        #region 枚举转换

        /// <summary>
        /// 转换为枚举，失败返回默认值
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string str, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;

            return Enum.TryParse<TEnum>(str, true, out var result) ? result : defaultValue;
        }

        #endregion

        #region 字节转换

        /// <summary>
        /// 转换为字节数组（UTF-8 编码）
        /// </summary>
        public static byte[] ToBytes(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 转换为字节数组（指定编码）
        /// </summary>
        public static byte[] ToBytes(this string str, System.Text.Encoding encoding)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            return encoding.GetBytes(str);
        }

        /// <summary>
        /// 从 Base64 字符串转换为字节数组
        /// </summary>
        public static byte[] FromBase64(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("字符串不能为空", nameof(str));

            return Convert.FromBase64String(str);
        }

        /// <summary>
        /// 从十六进制字符串转换为字节数组
        /// </summary>
        public static byte[] FromHexString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("字符串不能为空", nameof(str));

            str = str.Replace(" ", "").Replace("-", "");

            if (str.Length % 2 != 0)
                throw new ArgumentException("十六进制字符串长度必须为偶数", nameof(str));

            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        #endregion

        #region 可空类型转换

        /// <summary>
        /// 转换为可空 int
        /// </summary>
        public static int? ToNullableInt(this string str)
        {
            return int.TryParse(str, out var result) ? result : (int?)null;
        }

        /// <summary>
        /// 转换为可空 long
        /// </summary>
        public static long? ToNullableLong(this string str)
        {
            return long.TryParse(str, out var result) ? result : (long?)null;
        }

        /// <summary>
        /// 转换为可空 float
        /// </summary>
        public static float? ToNullableFloat(this string str)
        {
            return float.TryParse(str, out var result) ? result : (float?)null;
        }

        /// <summary>
        /// 转换为可空 double
        /// </summary>
        public static double? ToNullableDouble(this string str)
        {
            return double.TryParse(str, out var result) ? result : (double?)null;
        }

        /// <summary>
        /// 转换为可空 decimal
        /// </summary>
        public static decimal? ToNullableDecimal(this string str)
        {
            return decimal.TryParse(str, out var result) ? result : (decimal?)null;
        }

        /// <summary>
        /// 转换为可空 DateTime
        /// </summary>
        public static DateTime? ToNullableDateTime(this string str)
        {
            return DateTime.TryParse(str, out var result) ? result : (DateTime?)null;
        }

        #endregion
    }
}
