// ==========================================================
// 文件名：JsonEscaper.cs
// 命名空间: AFramework.Serialization.Internal
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// JSON 转义处理器
    /// <para>处理 JSON 字符串的转义和反转义</para>
    /// <para>支持 Unicode 转义序列</para>
    /// </summary>
    /// <remarks>
    /// JSON 字符串转义规则 (RFC 8259):
    /// - \" 双引号
    /// - \\ 反斜杠
    /// - \/ 斜杠 (可选)
    /// - \b 退格
    /// - \f 换页
    /// - \n 换行
    /// - \r 回车
    /// - \t 制表符
    /// - \uXXXX Unicode 字符
    /// 
    /// 使用示例:
    /// <code>
    /// // 转义
    /// string escaped = JsonEscaper.Escape("Hello\nWorld");
    /// // 结果: "Hello\\nWorld"
    /// 
    /// // 反转义
    /// string unescaped = JsonEscaper.Unescape("Hello\\nWorld");
    /// // 结果: "Hello\nWorld"
    /// </code>
    /// </remarks>
    internal static class JsonEscaper
    {
        #region 常量

        /// <summary>需要转义的字符查找表</summary>
        private static readonly bool[] s_needsEscaping = CreateNeedsEscapingTable();

        /// <summary>转义字符映射表</summary>
        private static readonly char[] s_escapeChars = CreateEscapeCharsTable();

        #endregion

        #region 初始化

        /// <summary>
        /// 创建需要转义的字符查找表
        /// </summary>
        private static bool[] CreateNeedsEscapingTable()
        {
            var table = new bool[128];

            // 控制字符 (0x00 - 0x1F)
            for (int i = 0; i < 0x20; i++)
                table[i] = true;

            // 特殊字符
            table['"'] = true;
            table['\\'] = true;

            return table;
        }

        /// <summary>
        /// 创建转义字符映射表
        /// </summary>
        private static char[] CreateEscapeCharsTable()
        {
            var table = new char[128];

            table['"'] = '"';
            table['\\'] = '\\';
            table['/'] = '/';
            table['b'] = '\b';
            table['f'] = '\f';
            table['n'] = '\n';
            table['r'] = '\r';
            table['t'] = '\t';

            return table;
        }

        #endregion

        #region 转义方法

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="escapeNonAscii">是否转义非 ASCII 字符</param>
        /// <returns>转义后的字符串</returns>
        public static string Escape(string value, bool escapeNonAscii = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Escape(value.AsSpan(), escapeNonAscii);
        }

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="escapeNonAscii">是否转义非 ASCII 字符</param>
        /// <returns>转义后的字符串</returns>
        public static string Escape(ReadOnlySpan<char> value, bool escapeNonAscii = false)
        {
            if (value.IsEmpty)
                return string.Empty;

            // 快速检查是否需要转义
            if (!NeedsEscaping(value, escapeNonAscii))
                return value.ToString();

            var sb = new StringBuilder(value.Length + 16);
            EscapeTo(value, sb, escapeNonAscii);
            return sb.ToString();
        }

        /// <summary>
        /// 转义字符串到 StringBuilder
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="sb">目标 StringBuilder</param>
        /// <param name="escapeNonAscii">是否转义非 ASCII 字符</param>
        public static void EscapeTo(ReadOnlySpan<char> value, StringBuilder sb, bool escapeNonAscii = false)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                // ASCII 范围内的字符
                if (ch < 128)
                {
                    if (s_needsEscaping[ch])
                    {
                        sb.Append('\\');
                        sb.Append(GetEscapeChar(ch));
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                // 非 ASCII 字符
                else if (escapeNonAscii)
                {
                    // 处理代理对
                    if (char.IsHighSurrogate(ch) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                    {
                        sb.Append($"\\u{(int)ch:X4}");
                        i++;
                        sb.Append($"\\u{(int)value[i]:X4}");
                    }
                    else
                    {
                        sb.Append($"\\u{(int)ch:X4}");
                    }
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }

        /// <summary>
        /// 转义字符串到 JsonStringBuilder
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="builder">目标构建器</param>
        /// <param name="escapeNonAscii">是否转义非 ASCII 字符</param>
        public static void EscapeTo(ReadOnlySpan<char> value, JsonStringBuilder builder, bool escapeNonAscii = false)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch < 128)
                {
                    if (s_needsEscaping[ch])
                    {
                        builder.Append('\\');
                        builder.Append(GetEscapeChar(ch));
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
                else if (escapeNonAscii)
                {
                    if (char.IsHighSurrogate(ch) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                    {
                        builder.AppendUnicodeEscape(ch);
                        i++;
                        builder.AppendUnicodeEscape(value[i]);
                    }
                    else
                    {
                        builder.AppendUnicodeEscape(ch);
                    }
                }
                else
                {
                    builder.Append(ch);
                }
            }
        }

        /// <summary>
        /// 获取字符的转义字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetEscapeChar(char ch)
        {
            return ch switch
            {
                '"' => '"',
                '\\' => '\\',
                '\b' => 'b',
                '\f' => 'f',
                '\n' => 'n',
                '\r' => 'r',
                '\t' => 't',
                _ => 'u' // 使用 Unicode 转义
            };
        }

        /// <summary>
        /// 检查字符串是否需要转义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedsEscaping(ReadOnlySpan<char> value, bool escapeNonAscii = false)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch < 128)
                {
                    if (s_needsEscaping[ch])
                        return true;
                }
                else if (escapeNonAscii)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 反转义方法

        /// <summary>
        /// 反转义字符串
        /// </summary>
        /// <param name="value">转义后的字符串</param>
        /// <returns>原始字符串</returns>
        public static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Unescape(value.AsSpan());
        }

        /// <summary>
        /// 反转义字符串
        /// </summary>
        /// <param name="value">转义后的字符串</param>
        /// <returns>原始字符串</returns>
        public static string Unescape(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return string.Empty;

            // 快速检查是否包含转义字符
            int escapeIndex = value.IndexOf('\\');
            if (escapeIndex < 0)
                return value.ToString();

            var sb = new StringBuilder(value.Length);

            // 复制转义字符之前的部分
            for (int i = 0; i < escapeIndex; i++)
                sb.Append(value[i]);

            // 处理剩余部分
            for (int i = escapeIndex; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch == '\\' && i + 1 < value.Length)
                {
                    char next = value[i + 1];

                    // Unicode 转义
                    if (next == 'u' && i + 5 < value.Length)
                    {
                        int codePoint = ParseHex4(value.Slice(i + 2, 4));
                        if (codePoint >= 0)
                        {
                            char unescaped = (char)codePoint;

                            // 处理代理对
                            if (char.IsHighSurrogate(unescaped) && i + 11 < value.Length &&
                                value[i + 6] == '\\' && value[i + 7] == 'u')
                            {
                                int lowSurrogate = ParseHex4(value.Slice(i + 8, 4));
                                if (lowSurrogate >= 0 && char.IsLowSurrogate((char)lowSurrogate))
                                {
                                    sb.Append(unescaped);
                                    sb.Append((char)lowSurrogate);
                                    i += 11;
                                    continue;
                                }
                            }

                            sb.Append(unescaped);
                            i += 5;
                            continue;
                        }
                    }

                    // 标准转义字符
                    if (next < 128 && s_escapeChars[next] != 0)
                    {
                        sb.Append(s_escapeChars[next]);
                        i++;
                        continue;
                    }

                    // 未知转义，保留原样
                    sb.Append(ch);
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 解析4位十六进制数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ParseHex4(ReadOnlySpan<char> hex)
        {
            if (hex.Length < 4)
                return -1;

            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                int digit = JsonFormat.HexToInt(hex[i]);
                if (digit < 0)
                    return -1;
                result = (result << 4) | digit;
            }
            return result;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 计算转义后的长度
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="escapeNonAscii">是否转义非 ASCII 字符</param>
        /// <returns>转义后的长度</returns>
        public static int GetEscapedLength(ReadOnlySpan<char> value, bool escapeNonAscii = false)
        {
            int length = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch < 128)
                {
                    if (s_needsEscaping[ch])
                    {
                        // 控制字符使用 \uXXXX 格式
                        if (ch < 0x20 && ch != '\b' && ch != '\f' && ch != '\n' && ch != '\r' && ch != '\t')
                            length += 6;
                        else
                            length += 2;
                    }
                    else
                    {
                        length++;
                    }
                }
                else if (escapeNonAscii)
                {
                    length += 6; // \uXXXX
                }
                else
                {
                    length++;
                }
            }

            return length;
        }

        #endregion
    }
}
