// ==========================================================
// 文件名：StringConstants.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串常量定义
    /// <para>提供常用字符串常量，避免硬编码和重复字符串分配</para>
    /// <para>包含空白字符、分隔符、换行符等常用字符串</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用空字符串常量
    /// string result = condition ? value : StringConstants.Empty;
    /// 
    /// // 使用分隔符
    /// string joined = string.Join(StringConstants.Separators.Comma, items);
    /// 
    /// // 使用换行符
    /// string multiLine = line1 + StringConstants.NewLine + line2;
    /// </code>
    /// </remarks>
    public static class StringConstants
    {
        #region 基础字符串常量

        /// <summary>空字符串</summary>
        public const string Empty = "";

        /// <summary>空格</summary>
        public const string Space = " ";

        /// <summary>制表符</summary>
        public const string Tab = "\t";

        /// <summary>换行符 (LF)</summary>
        public const string NewLine = "\n";

        /// <summary>回车符 (CR)</summary>
        public const string CarriageReturn = "\r";

        /// <summary>Windows 换行符 (CRLF)</summary>
        public const string WindowsNewLine = "\r\n";

        /// <summary>空值字符串表示</summary>
        public const string Null = "null";

        /// <summary>未定义字符串表示</summary>
        public const string Undefined = "undefined";

        /// <summary>无/空字符串表示</summary>
        public const string None = "None";

        /// <summary>未知字符串表示</summary>
        public const string Unknown = "Unknown";

        #endregion

        #region 分隔符常量

        /// <summary>
        /// 分隔符常量集合
        /// </summary>
        public static class Separators
        {
            /// <summary>逗号分隔符</summary>
            public const string Comma = ",";

            /// <summary>逗号加空格分隔符</summary>
            public const string CommaSpace = ", ";

            /// <summary>分号分隔符</summary>
            public const string Semicolon = ";";

            /// <summary>冒号分隔符</summary>
            public const string Colon = ":";

            /// <summary>冒号加空格分隔符</summary>
            public const string ColonSpace = ": ";

            /// <summary>竖线分隔符</summary>
            public const string Pipe = "|";

            /// <summary>斜杠分隔符</summary>
            public const string Slash = "/";

            /// <summary>反斜杠分隔符</summary>
            public const string Backslash = "\\";

            /// <summary>点分隔符</summary>
            public const string Dot = ".";

            /// <summary>下划线分隔符</summary>
            public const string Underscore = "_";

            /// <summary>连字符分隔符</summary>
            public const string Hyphen = "-";

            /// <summary>等号分隔符</summary>
            public const string Equals = "=";

            /// <summary>与号分隔符</summary>
            public const string Ampersand = "&";

            /// <summary>问号分隔符</summary>
            public const string QuestionMark = "?";

            /// <summary>井号分隔符</summary>
            public const string Hash = "#";

            /// <summary>@ 符号</summary>
            public const string At = "@";
        }

        #endregion

        #region 括号常量

        /// <summary>
        /// 括号常量集合
        /// </summary>
        public static class Brackets
        {
            /// <summary>左圆括号</summary>
            public const string LeftParen = "(";

            /// <summary>右圆括号</summary>
            public const string RightParen = ")";

            /// <summary>圆括号对</summary>
            public const string Parentheses = "()";

            /// <summary>左方括号</summary>
            public const string LeftSquare = "[";

            /// <summary>右方括号</summary>
            public const string RightSquare = "]";

            /// <summary>方括号对</summary>
            public const string SquareBrackets = "[]";

            /// <summary>左花括号</summary>
            public const string LeftCurly = "{";

            /// <summary>右花括号</summary>
            public const string RightCurly = "}";

            /// <summary>花括号对</summary>
            public const string CurlyBraces = "{}";

            /// <summary>左尖括号</summary>
            public const string LeftAngle = "<";

            /// <summary>右尖括号</summary>
            public const string RightAngle = ">";

            /// <summary>尖括号对</summary>
            public const string AngleBrackets = "<>";
        }

        #endregion

        #region 引号常量

        /// <summary>
        /// 引号常量集合
        /// </summary>
        public static class Quotes
        {
            /// <summary>单引号</summary>
            public const string Single = "'";

            /// <summary>双引号</summary>
            public const string Double = "\"";

            /// <summary>反引号</summary>
            public const string Backtick = "`";
        }

        #endregion
    


        #region 布尔值字符串常量

        /// <summary>
        /// 布尔值字符串常量集合
        /// </summary>
        public static class Boolean
        {
            /// <summary>true (小写)</summary>
            public const string TrueLower = "true";

            /// <summary>false (小写)</summary>
            public const string FalseLower = "false";

            /// <summary>True (首字母大写)</summary>
            public const string TrueCapital = "True";

            /// <summary>False (首字母大写)</summary>
            public const string FalseCapital = "False";

            /// <summary>TRUE (全大写)</summary>
            public const string TrueUpper = "TRUE";

            /// <summary>FALSE (全大写)</summary>
            public const string FalseUpper = "FALSE";

            /// <summary>Yes</summary>
            public const string Yes = "Yes";

            /// <summary>No</summary>
            public const string No = "No";

            /// <summary>1 (数字表示 true)</summary>
            public const string One = "1";

            /// <summary>0 (数字表示 false)</summary>
            public const string Zero = "0";
        }

        #endregion

        #region 数字格式常量

        /// <summary>
        /// 数字格式字符串常量
        /// </summary>
        public static class NumberFormats
        {
            /// <summary>整数格式</summary>
            public const string Integer = "D";

            /// <summary>带千位分隔符的整数</summary>
            public const string IntegerWithSeparator = "N0";

            /// <summary>两位小数</summary>
            public const string TwoDecimals = "F2";

            /// <summary>四位小数</summary>
            public const string FourDecimals = "F4";

            /// <summary>百分比 (无小数)</summary>
            public const string Percent = "P0";

            /// <summary>百分比 (两位小数)</summary>
            public const string PercentTwoDecimals = "P2";

            /// <summary>货币格式</summary>
            public const string Currency = "C";

            /// <summary>货币格式 (两位小数)</summary>
            public const string CurrencyTwoDecimals = "C2";

            /// <summary>科学计数法</summary>
            public const string Scientific = "E";

            /// <summary>十六进制 (小写)</summary>
            public const string HexLower = "x";

            /// <summary>十六进制 (大写)</summary>
            public const string HexUpper = "X";

            /// <summary>十六进制 (8位，大写)</summary>
            public const string Hex8 = "X8";
        }

        #endregion

        #region 日期时间格式常量

        /// <summary>
        /// 日期时间格式字符串常量
        /// </summary>
        public static class DateTimeFormats
        {
            /// <summary>ISO 8601 格式</summary>
            public const string ISO8601 = "yyyy-MM-ddTHH:mm:ss";

            /// <summary>ISO 8601 带时区格式</summary>
            public const string ISO8601WithTimeZone = "yyyy-MM-ddTHH:mm:sszzz";

            /// <summary>短日期格式 (yyyy-MM-dd)</summary>
            public const string ShortDate = "yyyy-MM-dd";

            /// <summary>长日期格式</summary>
            public const string LongDate = "yyyy年MM月dd日";

            /// <summary>短时间格式 (HH:mm)</summary>
            public const string ShortTime = "HH:mm";

            /// <summary>长时间格式 (HH:mm:ss)</summary>
            public const string LongTime = "HH:mm:ss";

            /// <summary>完整日期时间格式</summary>
            public const string Full = "yyyy-MM-dd HH:mm:ss";

            /// <summary>带毫秒的完整格式</summary>
            public const string FullWithMilliseconds = "yyyy-MM-dd HH:mm:ss.fff";

            /// <summary>文件名安全格式</summary>
            public const string FileNameSafe = "yyyyMMdd_HHmmss";

            /// <summary>日志时间戳格式</summary>
            public const string LogTimestamp = "HH:mm:ss.fff";

            /// <summary>年月格式</summary>
            public const string YearMonth = "yyyy-MM";

            /// <summary>月日格式</summary>
            public const string MonthDay = "MM-dd";
        }

        #endregion

        #region 文件路径常量

        /// <summary>
        /// 文件路径相关常量
        /// </summary>
        public static class Path
        {
            /// <summary>当前目录</summary>
            public const string CurrentDirectory = ".";

            /// <summary>父目录</summary>
            public const string ParentDirectory = "..";

            /// <summary>路径分隔符 (Unix)</summary>
            public const char DirectorySeparatorUnix = '/';

            /// <summary>路径分隔符 (Windows)</summary>
            public const char DirectorySeparatorWindows = '\\';

            /// <summary>扩展名分隔符</summary>
            public const char ExtensionSeparator = '.';

            /// <summary>通配符 (任意字符)</summary>
            public const string WildcardAny = "*";

            /// <summary>通配符 (单个字符)</summary>
            public const string WildcardSingle = "?";
        }

        #endregion

        #region 常用文件扩展名

        /// <summary>
        /// 常用文件扩展名常量
        /// </summary>
        public static class FileExtensions
        {
            /// <summary>文本文件</summary>
            public const string Txt = ".txt";

            /// <summary>JSON 文件</summary>
            public const string Json = ".json";

            /// <summary>XML 文件</summary>
            public const string Xml = ".xml";

            /// <summary>YAML 文件</summary>
            public const string Yaml = ".yaml";

            /// <summary>YML 文件</summary>
            public const string Yml = ".yml";

            /// <summary>CSV 文件</summary>
            public const string Csv = ".csv";

            /// <summary>日志文件</summary>
            public const string Log = ".log";

            /// <summary>配置文件</summary>
            public const string Config = ".config";

            /// <summary>二进制文件</summary>
            public const string Bin = ".bin";

            /// <summary>数据文件</summary>
            public const string Dat = ".dat";

            /// <summary>C# 源文件</summary>
            public const string CSharp = ".cs";

            /// <summary>Unity 元数据文件</summary>
            public const string Meta = ".meta";

            /// <summary>Unity 预制体</summary>
            public const string Prefab = ".prefab";

            /// <summary>Unity 场景</summary>
            public const string Unity = ".unity";

            /// <summary>Unity Asset</summary>
            public const string Asset = ".asset";
        }

        #endregion

        #region 字符数组常量

        /// <summary>
        /// 常用字符数组 (用于 Split 等操作)
        /// </summary>
        public static class CharArrays
        {
            /// <summary>空格字符数组</summary>
            public static readonly char[] Space = { ' ' };

            /// <summary>逗号字符数组</summary>
            public static readonly char[] Comma = { ',' };

            /// <summary>分号字符数组</summary>
            public static readonly char[] Semicolon = { ';' };

            /// <summary>冒号字符数组</summary>
            public static readonly char[] Colon = { ':' };

            /// <summary>点字符数组</summary>
            public static readonly char[] Dot = { '.' };

            /// <summary>斜杠字符数组</summary>
            public static readonly char[] Slash = { '/' };

            /// <summary>换行字符数组</summary>
            public static readonly char[] NewLine = { '\n', '\r' };

            /// <summary>空白字符数组</summary>
            public static readonly char[] Whitespace = { ' ', '\t', '\n', '\r' };

            /// <summary>路径分隔符数组</summary>
            public static readonly char[] PathSeparators = { '/', '\\' };

            /// <summary>常用分隔符数组</summary>
            public static readonly char[] CommonSeparators = { ',', ';', '|', '\t' };
        }

        #endregion
    }
}
