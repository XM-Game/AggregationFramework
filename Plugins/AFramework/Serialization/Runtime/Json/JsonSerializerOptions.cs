// ==========================================================
// 文件名：JsonSerializerOptions.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Text;

namespace AFramework.Serialization
{
    /// <summary>
    /// JSON 序列化器选项
    /// <para>配置 JSON 序列化的行为参数</para>
    /// <para>支持流畅 API 和预设配置</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 使用默认选项
    /// var options = JsonSerializerOptions.Default;
    /// 
    /// // 使用预设配置
    /// var prettyOptions = JsonSerializerOptions.Pretty;
    /// var compactOptions = JsonSerializerOptions.Compact;
    /// 
    /// // 自定义配置
    /// var options = JsonSerializerOptions.Default
    ///     .WithIndented(true)
    ///     .WithCamelCase(true)
    ///     .WithIgnoreNull(true);
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct JsonSerializerOptions : IEquatable<JsonSerializerOptions>
    {
        #region 字段

        private readonly bool _indented;
        private readonly bool _camelCaseNaming;
        private readonly bool _ignoreNullValues;
        private readonly bool _ignoreDefaultValues;
        private readonly bool _includeFields;
        private readonly bool _allowTrailingCommas;
        private readonly bool _allowComments;
        private readonly bool _writeIndentedArrays;
        private readonly bool _escapeNonAscii;
        private readonly bool _allowNaN;
        private readonly int _maxDepth;
        private readonly int _initialBufferSize;
        private readonly string _indentString;
        private readonly string _newLine;
        private readonly JsonNumberHandling _numberHandling;
        private readonly JsonPropertyNamingPolicy _namingPolicy;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 序列化器选项
        /// </summary>
        private JsonSerializerOptions(
            bool indented,
            bool camelCaseNaming,
            bool ignoreNullValues,
            bool ignoreDefaultValues,
            bool includeFields,
            bool allowTrailingCommas,
            bool allowComments,
            bool writeIndentedArrays,
            bool escapeNonAscii,
            bool allowNaN,
            int maxDepth,
            int initialBufferSize,
            string indentString,
            string newLine,
            JsonNumberHandling numberHandling,
            JsonPropertyNamingPolicy namingPolicy)
        {
            _indented = indented;
            _camelCaseNaming = camelCaseNaming;
            _ignoreNullValues = ignoreNullValues;
            _ignoreDefaultValues = ignoreDefaultValues;
            _includeFields = includeFields;
            _allowTrailingCommas = allowTrailingCommas;
            _allowComments = allowComments;
            _writeIndentedArrays = writeIndentedArrays;
            _escapeNonAscii = escapeNonAscii;
            _allowNaN = allowNaN;
            _maxDepth = maxDepth;
            _initialBufferSize = initialBufferSize;
            _indentString = indentString ?? JsonFormat.DefaultIndent;
            _newLine = newLine ?? JsonFormat.DefaultNewLine;
            _numberHandling = numberHandling;
            _namingPolicy = namingPolicy;
        }

        #endregion

        #region 属性

        /// <summary>是否格式化输出 (带缩进和换行)</summary>
        public bool Indented => _indented;

        /// <summary>是否使用驼峰命名</summary>
        public bool CamelCaseNaming => _camelCaseNaming;

        /// <summary>是否忽略 null 值</summary>
        public bool IgnoreNullValues => _ignoreNullValues;

        /// <summary>是否忽略默认值</summary>
        public bool IgnoreDefaultValues => _ignoreDefaultValues;

        /// <summary>是否包含字段 (默认只序列化属性)</summary>
        public bool IncludeFields => _includeFields;

        /// <summary>是否允许尾随逗号</summary>
        public bool AllowTrailingCommas => _allowTrailingCommas;

        /// <summary>是否允许注释</summary>
        public bool AllowComments => _allowComments;

        /// <summary>是否对数组也使用缩进</summary>
        public bool WriteIndentedArrays => _writeIndentedArrays;

        /// <summary>是否转义非 ASCII 字符</summary>
        public bool EscapeNonAscii => _escapeNonAscii;

        /// <summary>是否允许 NaN 和 Infinity</summary>
        public bool AllowNaN => _allowNaN;

        /// <summary>最大序列化深度</summary>
        public int MaxDepth => _maxDepth;

        /// <summary>初始缓冲区大小</summary>
        public int InitialBufferSize => _initialBufferSize;

        /// <summary>缩进字符串</summary>
        public string IndentString => _indentString;

        /// <summary>换行符</summary>
        public string NewLine => _newLine;

        /// <summary>数字处理方式</summary>
        public JsonNumberHandling NumberHandling => _numberHandling;

        /// <summary>属性命名策略</summary>
        public JsonPropertyNamingPolicy NamingPolicy => _namingPolicy;

        #endregion

        #region 预设配置

        /// <summary>
        /// 默认选项
        /// <para>紧凑格式，高性能</para>
        /// </summary>
        public static JsonSerializerOptions Default => new JsonSerializerOptions(
            indented: false,
            camelCaseNaming: false,
            ignoreNullValues: false,
            ignoreDefaultValues: false,
            includeFields: false,
            allowTrailingCommas: false,
            allowComments: false,
            writeIndentedArrays: false,
            escapeNonAscii: false,
            allowNaN: false,
            maxDepth: JsonFormat.MaxDepth,
            initialBufferSize: JsonFormat.DefaultBufferSize,
            indentString: JsonFormat.DefaultIndent,
            newLine: JsonFormat.DefaultNewLine,
            numberHandling: JsonNumberHandling.Strict,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        /// <summary>
        /// 美化输出选项
        /// <para>带缩进和换行，便于阅读</para>
        /// </summary>
        public static JsonSerializerOptions Pretty => new JsonSerializerOptions(
            indented: true,
            camelCaseNaming: false,
            ignoreNullValues: false,
            ignoreDefaultValues: false,
            includeFields: false,
            allowTrailingCommas: false,
            allowComments: false,
            writeIndentedArrays: true,
            escapeNonAscii: false,
            allowNaN: false,
            maxDepth: JsonFormat.MaxDepth,
            initialBufferSize: JsonFormat.DefaultBufferSize,
            indentString: JsonFormat.DefaultIndent,
            newLine: JsonFormat.DefaultNewLine,
            numberHandling: JsonNumberHandling.Strict,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        /// <summary>
        /// 紧凑输出选项
        /// <para>最小化输出，忽略 null 值</para>
        /// </summary>
        public static JsonSerializerOptions Compact => new JsonSerializerOptions(
            indented: false,
            camelCaseNaming: false,
            ignoreNullValues: true,
            ignoreDefaultValues: true,
            includeFields: false,
            allowTrailingCommas: false,
            allowComments: false,
            writeIndentedArrays: false,
            escapeNonAscii: false,
            allowNaN: false,
            maxDepth: JsonFormat.MaxDepth,
            initialBufferSize: JsonFormat.DefaultBufferSize,
            indentString: null,
            newLine: null,
            numberHandling: JsonNumberHandling.Strict,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        /// <summary>
        /// Web API 选项
        /// <para>驼峰命名，忽略 null，适合 REST API</para>
        /// </summary>
        public static JsonSerializerOptions WebApi => new JsonSerializerOptions(
            indented: false,
            camelCaseNaming: true,
            ignoreNullValues: true,
            ignoreDefaultValues: false,
            includeFields: false,
            allowTrailingCommas: false,
            allowComments: false,
            writeIndentedArrays: false,
            escapeNonAscii: false,
            allowNaN: false,
            maxDepth: 32,
            initialBufferSize: 512,
            indentString: null,
            newLine: null,
            numberHandling: JsonNumberHandling.Strict,
            namingPolicy: JsonPropertyNamingPolicy.CamelCase
        );

        /// <summary>
        /// 宽松解析选项
        /// <para>允许注释、尾随逗号、NaN</para>
        /// </summary>
        public static JsonSerializerOptions Lenient => new JsonSerializerOptions(
            indented: false,
            camelCaseNaming: false,
            ignoreNullValues: false,
            ignoreDefaultValues: false,
            includeFields: true,
            allowTrailingCommas: true,
            allowComments: true,
            writeIndentedArrays: false,
            escapeNonAscii: false,
            allowNaN: true,
            maxDepth: 128,
            initialBufferSize: JsonFormat.DefaultBufferSize,
            indentString: JsonFormat.DefaultIndent,
            newLine: JsonFormat.DefaultNewLine,
            numberHandling: JsonNumberHandling.AllowReadingFromString,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        /// <summary>
        /// 游戏配置选项
        /// <para>包含字段，允许注释，美化输出</para>
        /// </summary>
        public static JsonSerializerOptions GameConfig => new JsonSerializerOptions(
            indented: true,
            camelCaseNaming: false,
            ignoreNullValues: false,
            ignoreDefaultValues: false,
            includeFields: true,
            allowTrailingCommas: true,
            allowComments: true,
            writeIndentedArrays: true,
            escapeNonAscii: false,
            allowNaN: true,
            maxDepth: 64,
            initialBufferSize: 1024,
            indentString: JsonFormat.TabIndent,
            newLine: JsonFormat.DefaultNewLine,
            numberHandling: JsonNumberHandling.AllowNamedFloatingPointLiterals,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        /// <summary>
        /// 调试选项
        /// <para>完整信息，美化输出</para>
        /// </summary>
        public static JsonSerializerOptions Debug => new JsonSerializerOptions(
            indented: true,
            camelCaseNaming: false,
            ignoreNullValues: false,
            ignoreDefaultValues: false,
            includeFields: true,
            allowTrailingCommas: true,
            allowComments: true,
            writeIndentedArrays: true,
            escapeNonAscii: false,
            allowNaN: true,
            maxDepth: 256,
            initialBufferSize: 1024,
            indentString: JsonFormat.FourSpaceIndent,
            newLine: JsonFormat.DefaultNewLine,
            numberHandling: JsonNumberHandling.AllowNamedFloatingPointLiterals,
            namingPolicy: JsonPropertyNamingPolicy.None
        );

        #endregion

        #region With 方法

        /// <summary>修改是否格式化输出</summary>
        public JsonSerializerOptions WithIndented(bool indented) =>
            new JsonSerializerOptions(indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否使用驼峰命名</summary>
        public JsonSerializerOptions WithCamelCase(bool camelCase) =>
            new JsonSerializerOptions(_indented, camelCase, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling,
                camelCase ? JsonPropertyNamingPolicy.CamelCase : JsonPropertyNamingPolicy.None);

        /// <summary>修改是否忽略 null 值</summary>
        public JsonSerializerOptions WithIgnoreNull(bool ignoreNull) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, ignoreNull, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否忽略默认值</summary>
        public JsonSerializerOptions WithIgnoreDefault(bool ignoreDefault) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, ignoreDefault,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否包含字段</summary>
        public JsonSerializerOptions WithIncludeFields(bool includeFields) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否允许尾随逗号</summary>
        public JsonSerializerOptions WithAllowTrailingCommas(bool allow) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, allow, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否允许注释</summary>
        public JsonSerializerOptions WithAllowComments(bool allow) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, allow, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改是否允许 NaN</summary>
        public JsonSerializerOptions WithAllowNaN(bool allow) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                allow, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改最大深度</summary>
        public JsonSerializerOptions WithMaxDepth(int maxDepth) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改初始缓冲区大小</summary>
        public JsonSerializerOptions WithInitialBufferSize(int size) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, size, _indentString, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改缩进字符串</summary>
        public JsonSerializerOptions WithIndentString(string indent) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, indent, _newLine, _numberHandling, _namingPolicy);

        /// <summary>修改换行符</summary>
        public JsonSerializerOptions WithNewLine(string newLine) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, newLine, _numberHandling, _namingPolicy);

        /// <summary>修改数字处理方式</summary>
        public JsonSerializerOptions WithNumberHandling(JsonNumberHandling handling) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, handling, _namingPolicy);

        /// <summary>修改命名策略</summary>
        public JsonSerializerOptions WithNamingPolicy(JsonPropertyNamingPolicy policy) =>
            new JsonSerializerOptions(_indented, _camelCaseNaming, _ignoreNullValues, _ignoreDefaultValues,
                _includeFields, _allowTrailingCommas, _allowComments, _writeIndentedArrays, _escapeNonAscii,
                _allowNaN, _maxDepth, _initialBufferSize, _indentString, _newLine, _numberHandling, policy);

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证选项是否有效
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (_maxDepth <= 0)
            {
                errorMessage = "最大深度必须大于 0";
                return false;
            }

            if (_initialBufferSize <= 0)
            {
                errorMessage = "初始缓冲区大小必须大于 0";
                return false;
            }

            return true;
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(JsonSerializerOptions other)
        {
            return _indented == other._indented &&
                   _camelCaseNaming == other._camelCaseNaming &&
                   _ignoreNullValues == other._ignoreNullValues &&
                   _ignoreDefaultValues == other._ignoreDefaultValues &&
                   _includeFields == other._includeFields &&
                   _allowTrailingCommas == other._allowTrailingCommas &&
                   _allowComments == other._allowComments &&
                   _maxDepth == other._maxDepth &&
                   _numberHandling == other._numberHandling &&
                   _namingPolicy == other._namingPolicy;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is JsonSerializerOptions other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_indented);
            hash.Add(_camelCaseNaming);
            hash.Add(_ignoreNullValues);
            hash.Add(_ignoreDefaultValues);
            hash.Add(_includeFields);
            hash.Add(_maxDepth);
            hash.Add(_numberHandling);
            hash.Add(_namingPolicy);
            return hash.ToHashCode();
        }

        /// <summary>相等运算符</summary>
        public static bool operator ==(JsonSerializerOptions left, JsonSerializerOptions right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(JsonSerializerOptions left, JsonSerializerOptions right) => !left.Equals(right);

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"JsonSerializerOptions(Indented={_indented}, CamelCase={_camelCaseNaming}, IgnoreNull={_ignoreNullValues})";
        }

        #endregion
    }

    #region 辅助枚举

    /// <summary>
    /// JSON 数字处理方式
    /// </summary>
    [Flags]
    public enum JsonNumberHandling
    {
        /// <summary>严格模式 (默认)</summary>
        Strict = 0,

        /// <summary>允许从字符串读取数字</summary>
        AllowReadingFromString = 1,

        /// <summary>将数字写入为字符串</summary>
        WriteAsString = 2,

        /// <summary>允许 NaN、Infinity 等特殊浮点值</summary>
        AllowNamedFloatingPointLiterals = 4
    }

    /// <summary>
    /// JSON 属性命名策略
    /// </summary>
    public enum JsonPropertyNamingPolicy
    {
        /// <summary>保持原样</summary>
        None = 0,

        /// <summary>驼峰命名 (camelCase)</summary>
        CamelCase = 1,

        /// <summary>蛇形命名 (snake_case)</summary>
        SnakeCase = 2,

        /// <summary>短横线命名 (kebab-case)</summary>
        KebabCase = 3,

        /// <summary>全大写 (UPPER_CASE)</summary>
        UpperCase = 4,

        /// <summary>全小写 (lowercase)</summary>
        LowerCase = 5
    }

    #endregion
}
