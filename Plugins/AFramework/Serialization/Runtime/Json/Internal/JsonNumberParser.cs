// ==========================================================
// 文件名：JsonNumberParser.cs
// 命名空间: AFramework.Serialization.Internal
// 依赖: System, System.Globalization
// ==========================================================

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// JSON 数字解析器
    /// <para>高性能解析 JSON 数字格式</para>
    /// <para>支持整数、浮点数、科学计数法</para>
    /// </summary>
    /// <remarks>
    /// JSON 数字格式 (RFC 8259):
    /// <code>
    /// number = [ minus ] int [ frac ] [ exp ]
    /// int = zero / ( digit1-9 *DIGIT )
    /// frac = decimal-point 1*DIGIT
    /// exp = e [ minus / plus ] 1*DIGIT
    /// </code>
    /// 
    /// 使用示例:
    /// <code>
    /// int intValue = JsonNumberParser.ParseInt32("123");
    /// double doubleValue = JsonNumberParser.ParseDouble("3.14e10");
    /// </code>
    /// </remarks>
    internal static class JsonNumberParser
    {
        #region 常量

        /// <summary>数字格式信息 (不变文化)</summary>
        private static readonly NumberFormatInfo s_numberFormat = CultureInfo.InvariantCulture.NumberFormat;

        /// <summary>数字样式 (整数)</summary>
        private const NumberStyles IntegerStyle = NumberStyles.Integer | NumberStyles.AllowLeadingSign;

        /// <summary>数字样式 (浮点数)</summary>
        private const NumberStyles FloatStyle = NumberStyles.Float | NumberStyles.AllowExponent;

        #endregion

        #region 整数解析

        /// <summary>
        /// 解析 32 位有符号整数
        /// </summary>
        /// <param name="span">数字文本</param>
        /// <returns>解析结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt32(ReadOnlySpan<char> span)
        {
            // 快速路径：短数字
            if (span.Length <= 10 && TryParseInt32Fast(span, out int fastResult))
                return fastResult;

            // 标准解析
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (int.TryParse(span, IntegerStyle, s_numberFormat, out int result))
                return result;
#else
            if (int.TryParse(span.ToString(), IntegerStyle, s_numberFormat, out int result))
                return result;
#endif

            throw new JsonParseException($"无法解析整数: {span.ToString()}", 0);
        }

        /// <summary>
        /// 解析 64 位有符号整数
        /// </summary>
        /// <param name="span">数字文本</param>
        /// <returns>解析结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseInt64(ReadOnlySpan<char> span)
        {
            // 快速路径：短数字
            if (span.Length <= 18 && TryParseInt64Fast(span, out long fastResult))
                return fastResult;

            // 标准解析
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (long.TryParse(span, IntegerStyle, s_numberFormat, out long result))
                return result;
#else
            if (long.TryParse(span.ToString(), IntegerStyle, s_numberFormat, out long result))
                return result;
#endif

            throw new JsonParseException($"无法解析长整数: {span.ToString()}", 0);
        }

        /// <summary>
        /// 解析 32 位无符号整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt32(ReadOnlySpan<char> span)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (uint.TryParse(span, NumberStyles.Integer, s_numberFormat, out uint result))
                return result;
#else
            if (uint.TryParse(span.ToString(), NumberStyles.Integer, s_numberFormat, out uint result))
                return result;
#endif

            throw new JsonParseException($"无法解析无符号整数: {span.ToString()}", 0);
        }

        /// <summary>
        /// 解析 64 位无符号整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ParseUInt64(ReadOnlySpan<char> span)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (ulong.TryParse(span, NumberStyles.Integer, s_numberFormat, out ulong result))
                return result;
#else
            if (ulong.TryParse(span.ToString(), NumberStyles.Integer, s_numberFormat, out ulong result))
                return result;
#endif

            throw new JsonParseException($"无法解析无符号长整数: {span.ToString()}", 0);
        }

        #endregion

        #region 浮点数解析

        /// <summary>
        /// 解析单精度浮点数
        /// </summary>
        /// <param name="span">数字文本</param>
        /// <param name="allowNaN">是否允许 NaN 和 Infinity</param>
        /// <returns>解析结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ParseSingle(ReadOnlySpan<char> span, bool allowNaN = false)
        {
            // 处理特殊值
            if (allowNaN)
            {
                if (SpanEquals(span, JsonFormat.NaN))
                    return float.NaN;
                if (SpanEquals(span, JsonFormat.PositiveInfinity))
                    return float.PositiveInfinity;
                if (SpanEquals(span, JsonFormat.NegativeInfinity))
                    return float.NegativeInfinity;
            }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (float.TryParse(span, FloatStyle, s_numberFormat, out float result))
                return result;
#else
            if (float.TryParse(span.ToString(), FloatStyle, s_numberFormat, out float result))
                return result;
#endif

            throw new JsonParseException($"无法解析浮点数: {span.ToString()}", 0);
        }

        /// <summary>
        /// 解析双精度浮点数
        /// </summary>
        /// <param name="span">数字文本</param>
        /// <param name="allowNaN">是否允许 NaN 和 Infinity</param>
        /// <returns>解析结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ParseDouble(ReadOnlySpan<char> span, bool allowNaN = false)
        {
            // 处理特殊值
            if (allowNaN)
            {
                if (SpanEquals(span, JsonFormat.NaN))
                    return double.NaN;
                if (SpanEquals(span, JsonFormat.PositiveInfinity))
                    return double.PositiveInfinity;
                if (SpanEquals(span, JsonFormat.NegativeInfinity))
                    return double.NegativeInfinity;
            }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (double.TryParse(span, FloatStyle, s_numberFormat, out double result))
                return result;
#else
            if (double.TryParse(span.ToString(), FloatStyle, s_numberFormat, out double result))
                return result;
#endif

            throw new JsonParseException($"无法解析双精度浮点数: {span.ToString()}", 0);
        }

        /// <summary>
        /// 解析十进制数
        /// </summary>
        /// <param name="span">数字文本</param>
        /// <returns>解析结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ParseDecimal(ReadOnlySpan<char> span)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (decimal.TryParse(span, FloatStyle, s_numberFormat, out decimal result))
                return result;
#else
            if (decimal.TryParse(span.ToString(), FloatStyle, s_numberFormat, out decimal result))
                return result;
#endif

            throw new JsonParseException($"无法解析十进制数: {span.ToString()}", 0);
        }

        #endregion

        #region 尝试解析方法

        /// <summary>
        /// 尝试解析 32 位整数
        /// </summary>
        public static bool TryParseInt32(ReadOnlySpan<char> span, out int result)
        {
            if (span.Length <= 10 && TryParseInt32Fast(span, out result))
                return true;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return int.TryParse(span, IntegerStyle, s_numberFormat, out result);
#else
            return int.TryParse(span.ToString(), IntegerStyle, s_numberFormat, out result);
#endif
        }

        /// <summary>
        /// 尝试解析 64 位整数
        /// </summary>
        public static bool TryParseInt64(ReadOnlySpan<char> span, out long result)
        {
            if (span.Length <= 18 && TryParseInt64Fast(span, out result))
                return true;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return long.TryParse(span, IntegerStyle, s_numberFormat, out result);
#else
            return long.TryParse(span.ToString(), IntegerStyle, s_numberFormat, out result);
#endif
        }

        /// <summary>
        /// 尝试解析双精度浮点数
        /// </summary>
        public static bool TryParseDouble(ReadOnlySpan<char> span, out double result, bool allowNaN = false)
        {
            if (allowNaN)
            {
                if (SpanEquals(span, JsonFormat.NaN))
                {
                    result = double.NaN;
                    return true;
                }
                if (SpanEquals(span, JsonFormat.PositiveInfinity))
                {
                    result = double.PositiveInfinity;
                    return true;
                }
                if (SpanEquals(span, JsonFormat.NegativeInfinity))
                {
                    result = double.NegativeInfinity;
                    return true;
                }
            }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return double.TryParse(span, FloatStyle, s_numberFormat, out result);
#else
            return double.TryParse(span.ToString(), FloatStyle, s_numberFormat, out result);
#endif
        }

        #endregion

        #region 快速解析路径

        /// <summary>
        /// 快速解析 32 位整数 (无分配)
        /// </summary>
        private static bool TryParseInt32Fast(ReadOnlySpan<char> span, out int result)
        {
            result = 0;

            if (span.IsEmpty)
                return false;

            int index = 0;
            bool negative = false;

            // 处理符号
            if (span[0] == '-')
            {
                negative = true;
                index = 1;
            }
            else if (span[0] == '+')
            {
                index = 1;
            }

            if (index >= span.Length)
                return false;

            // 解析数字
            long value = 0;
            for (; index < span.Length; index++)
            {
                char ch = span[index];
                if (ch < '0' || ch > '9')
                    return false;

                value = value * 10 + (ch - '0');

                // 溢出检查
                if (value > int.MaxValue + (negative ? 1L : 0L))
                    return false;
            }

            result = negative ? -(int)value : (int)value;
            return true;
        }

        /// <summary>
        /// 快速解析 64 位整数 (无分配)
        /// </summary>
        private static bool TryParseInt64Fast(ReadOnlySpan<char> span, out long result)
        {
            result = 0;

            if (span.IsEmpty)
                return false;

            int index = 0;
            bool negative = false;

            // 处理符号
            if (span[0] == '-')
            {
                negative = true;
                index = 1;
            }
            else if (span[0] == '+')
            {
                index = 1;
            }

            if (index >= span.Length)
                return false;

            // 解析数字
            ulong value = 0;
            for (; index < span.Length; index++)
            {
                char ch = span[index];
                if (ch < '0' || ch > '9')
                    return false;

                ulong newValue = value * 10 + (ulong)(ch - '0');

                // 溢出检查
                if (newValue < value)
                    return false;

                value = newValue;
            }

            // 范围检查
            if (negative)
            {
                if (value > (ulong)long.MaxValue + 1)
                    return false;
                result = -(long)value;
            }
            else
            {
                if (value > long.MaxValue)
                    return false;
                result = (long)value;
            }

            return true;
        }

        #endregion

        #region 数字类型检测

        /// <summary>
        /// 检测数字是否为整数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteger(ReadOnlySpan<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                char ch = span[i];
                if (ch == '.' || ch == 'e' || ch == 'E')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 检测数字是否为浮点数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloatingPoint(ReadOnlySpan<char> span)
        {
            return !IsInteger(span);
        }

        /// <summary>
        /// 检测数字是否在 Int32 范围内
        /// </summary>
        public static bool IsInInt32Range(ReadOnlySpan<char> span)
        {
            if (!IsInteger(span))
                return false;

            return TryParseInt32(span, out _);
        }

        /// <summary>
        /// 检测数字是否在 Int64 范围内
        /// </summary>
        public static bool IsInInt64Range(ReadOnlySpan<char> span)
        {
            if (!IsInteger(span))
                return false;

            return TryParseInt64(span, out _);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 比较 Span 和字符串是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SpanEquals(ReadOnlySpan<char> span, string str)
        {
            if (span.Length != str.Length)
                return false;

            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] != str[i])
                    return false;
            }

            return true;
        }

        #endregion
    }
}
