// ==========================================================
// 文件名：JsonFormat.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// JSON 格式定义
    /// <para>定义 JSON 序列化的格式规范和字符常量</para>
    /// <para>提供 JSON 语法字符、转义序列、格式化选项等</para>
    /// </summary>
    /// <remarks>
    /// JSON 格式遵循 RFC 8259 规范:
    /// <code>
    /// value = null / true / false / number / string / array / object
    /// object = "{" [member *("," member)] "}"
    /// member = string ":" value
    /// array = "[" [value *("," value)] "]"
    /// </code>
    /// 
    /// 使用示例:
    /// <code>
    /// // 检查字符类型
    /// if (JsonFormat.IsWhitespace(ch))
    ///     continue;
    /// 
    /// // 获取转义字符
    /// char escaped = JsonFormat.GetEscapeChar('n'); // 返回 '\n'
    /// </code>
    /// </remarks>
    public static class JsonFormat
    {
        #region 结构字符常量

        /// <summary>对象开始 '{'</summary>
        public const char ObjectStart = '{';

        /// <summary>对象结束 '}'</summary>
        public const char ObjectEnd = '}';

        /// <summary>数组开始 '['</summary>
        public const char ArrayStart = '[';

        /// <summary>数组结束 ']'</summary>
        public const char ArrayEnd = ']';

        /// <summary>名称分隔符 ':'</summary>
        public const char NameSeparator = ':';

        /// <summary>值分隔符 ','</summary>
        public const char ValueSeparator = ',';

        /// <summary>字符串引号 '"'</summary>
        public const char Quote = '"';

        /// <summary>转义字符 '\'</summary>
        public const char Escape = '\\';

        #endregion

        #region 字面量常量

        /// <summary>null 字面量</summary>
        public const string NullLiteral = "null";

        /// <summary>true 字面量</summary>
        public const string TrueLiteral = "true";

        /// <summary>false 字面量</summary>
        public const string FalseLiteral = "false";

        /// <summary>正无穷大 (非标准 JSON)</summary>
        public const string PositiveInfinity = "Infinity";

        /// <summary>负无穷大 (非标准 JSON)</summary>
        public const string NegativeInfinity = "-Infinity";

        /// <summary>非数字 (非标准 JSON)</summary>
        public const string NaN = "NaN";

        #endregion

        #region 空白字符常量

        /// <summary>空格</summary>
        public const char Space = ' ';

        /// <summary>水平制表符</summary>
        public const char Tab = '\t';

        /// <summary>换行符</summary>
        public const char LineFeed = '\n';

        /// <summary>回车符</summary>
        public const char CarriageReturn = '\r';

        #endregion

        #region 数字字符常量

        /// <summary>负号</summary>
        public const char Minus = '-';

        /// <summary>正号</summary>
        public const char Plus = '+';

        /// <summary>小数点</summary>
        public const char DecimalPoint = '.';

        /// <summary>指数标记 (小写)</summary>
        public const char ExponentLower = 'e';

        /// <summary>指数标记 (大写)</summary>
        public const char ExponentUpper = 'E';

        /// <summary>十六进制前缀 (小写)</summary>
        public const char HexPrefixLower = 'x';

        /// <summary>十六进制前缀 (大写)</summary>
        public const char HexPrefixUpper = 'X';

        #endregion

        #region 转义序列常量

        /// <summary>转义引号 \"</summary>
        public const string EscapedQuote = "\\\"";

        /// <summary>转义反斜杠 \\</summary>
        public const string EscapedBackslash = "\\\\";

        /// <summary>转义斜杠 \/</summary>
        public const string EscapedSlash = "\\/";

        /// <summary>转义退格 \b</summary>
        public const string EscapedBackspace = "\\b";

        /// <summary>转义换页 \f</summary>
        public const string EscapedFormFeed = "\\f";

        /// <summary>转义换行 \n</summary>
        public const string EscapedLineFeed = "\\n";

        /// <summary>转义回车 \r</summary>
        public const string EscapedCarriageReturn = "\\r";

        /// <summary>转义制表符 \t</summary>
        public const string EscapedTab = "\\t";

        /// <summary>Unicode 转义前缀 \u</summary>
        public const string UnicodeEscapePrefix = "\\u";

        #endregion

        #region 格式化常量

        /// <summary>默认缩进字符串 (2空格)</summary>
        public const string DefaultIndent = "  ";

        /// <summary>制表符缩进</summary>
        public const string TabIndent = "\t";

        /// <summary>4空格缩进</summary>
        public const string FourSpaceIndent = "    ";

        /// <summary>默认换行符</summary>
        public const string DefaultNewLine = "\n";

        /// <summary>Windows 换行符</summary>
        public const string WindowsNewLine = "\r\n";

        /// <summary>最大嵌套深度</summary>
        public const int MaxDepth = 64;

        /// <summary>默认初始缓冲区大小</summary>
        public const int DefaultBufferSize = 256;

        /// <summary>最大字符串长度</summary>
        public const int MaxStringLength = 100 * 1024 * 1024; // 100MB

        #endregion

        #region 字符检测方法

        /// <summary>
        /// 检查是否为 JSON 空白字符
        /// </summary>
        /// <param name="ch">要检查的字符</param>
        /// <returns>如果是空白字符返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(char ch)
        {
            return ch == Space || ch == Tab || ch == LineFeed || ch == CarriageReturn;
        }

        /// <summary>
        /// 检查是否为 JSON 空白字符
        /// </summary>
        /// <param name="b">要检查的字节</param>
        /// <returns>如果是空白字符返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(byte b)
        {
            return b == (byte)Space || b == (byte)Tab || 
                   b == (byte)LineFeed || b == (byte)CarriageReturn;
        }

        /// <summary>
        /// 检查是否为数字字符 (0-9)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        /// <summary>
        /// 检查是否为十六进制字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHexDigit(char ch)
        {
            return (ch >= '0' && ch <= '9') ||
                   (ch >= 'a' && ch <= 'f') ||
                   (ch >= 'A' && ch <= 'F');
        }

        /// <summary>
        /// 检查是否为数字开始字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumberStart(char ch)
        {
            return IsDigit(ch) || ch == Minus;
        }

        /// <summary>
        /// 检查是否为值开始字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueStart(char ch)
        {
            return ch == Quote ||           // 字符串
                   ch == ObjectStart ||     // 对象
                   ch == ArrayStart ||      // 数组
                   ch == 't' ||             // true
                   ch == 'f' ||             // false
                   ch == 'n' ||             // null
                   IsNumberStart(ch);       // 数字
        }

        /// <summary>
        /// 检查字符是否需要转义
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedsEscape(char ch)
        {
            return ch == Quote || ch == Escape || ch < 0x20 || ch == 0x7F;
        }

        /// <summary>
        /// 检查是否为控制字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsControlChar(char ch)
        {
            return ch < 0x20 || ch == 0x7F;
        }

        #endregion

        #region 转义处理方法

        /// <summary>
        /// 获取转义字符对应的实际字符
        /// </summary>
        /// <param name="escaped">转义字符 (不含反斜杠)</param>
        /// <returns>实际字符，如果无效返回 '\0'</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetUnescapedChar(char escaped)
        {
            return escaped switch
            {
                '"' => '"',
                '\\' => '\\',
                '/' => '/',
                'b' => '\b',
                'f' => '\f',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                _ => '\0'
            };
        }

        /// <summary>
        /// 获取字符的转义序列
        /// </summary>
        /// <param name="ch">要转义的字符</param>
        /// <returns>转义序列，如果不需要转义返回 null</returns>
        public static string GetEscapeSequence(char ch)
        {
            return ch switch
            {
                '"' => EscapedQuote,
                '\\' => EscapedBackslash,
                '\b' => EscapedBackspace,
                '\f' => EscapedFormFeed,
                '\n' => EscapedLineFeed,
                '\r' => EscapedCarriageReturn,
                '\t' => EscapedTab,
                _ when ch < 0x20 => $"\\u{(int)ch:X4}",
                _ => null
            };
        }

        /// <summary>
        /// 将十六进制字符转换为数值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HexToInt(char ch)
        {
            if (ch >= '0' && ch <= '9') return ch - '0';
            if (ch >= 'a' && ch <= 'f') return ch - 'a' + 10;
            if (ch >= 'A' && ch <= 'F') return ch - 'A' + 10;
            return -1;
        }

        /// <summary>
        /// 将数值转换为十六进制字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char IntToHex(int value)
        {
            return (char)(value < 10 ? '0' + value : 'a' + value - 10);
        }

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证 JSON 字符串是否有效
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>如果有效返回 true</returns>
        public static bool IsValidJson(ReadOnlySpan<char> json)
        {
            if (json.IsEmpty)
                return false;

            // 跳过前导空白
            int index = 0;
            while (index < json.Length && IsWhitespace(json[index]))
                index++;

            if (index >= json.Length)
                return false;

            // JSON 必须以 { 或 [ 开始
            char first = json[index];
            if (first != ObjectStart && first != ArrayStart)
                return false;

            // 简单的括号匹配检查
            int braceCount = 0;
            int bracketCount = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = index; i < json.Length; i++)
            {
                char ch = json[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (ch == Escape && inString)
                {
                    escaped = true;
                    continue;
                }

                if (ch == Quote)
                {
                    inString = !inString;
                    continue;
                }

                if (inString)
                    continue;

                switch (ch)
                {
                    case ObjectStart:
                        braceCount++;
                        break;
                    case ObjectEnd:
                        braceCount--;
                        if (braceCount < 0) return false;
                        break;
                    case ArrayStart:
                        bracketCount++;
                        break;
                    case ArrayEnd:
                        bracketCount--;
                        if (bracketCount < 0) return false;
                        break;
                }
            }

            return braceCount == 0 && bracketCount == 0 && !inString;
        }

        #endregion
    }
}
