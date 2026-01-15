// ==========================================================
// 文件名：JsonTokenizer.cs
// 命名空间: AFramework.Serialization.Internal
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// JSON 分词器
    /// <para>将 JSON 文本分解为标记 (Token) 序列</para>
    /// <para>支持流式解析，低内存分配</para>
    /// </summary>
    /// <remarks>
    /// 分词器是 JSON 解析的第一阶段，负责识别:
    /// - 结构字符: { } [ ] : ,
    /// - 字面量: null, true, false
    /// - 字符串: "..."
    /// - 数字: 整数、浮点数、科学计数法
    /// 
    /// 使用示例:
    /// <code>
    /// var tokenizer = new JsonTokenizer(jsonText);
    /// while (tokenizer.Read())
    /// {
    ///     switch (tokenizer.TokenType)
    ///     {
    ///         case JsonTokenType.PropertyName:
    ///             string name = tokenizer.GetString();
    ///             break;
    ///         case JsonTokenType.Number:
    ///             double value = tokenizer.GetDouble();
    ///             break;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    internal ref struct JsonTokenizer
    {
        #region 字段

        private readonly ReadOnlySpan<char> _json;
        private int _position;
        private int _tokenStart;
        private int _tokenLength;
        private JsonTokenType _tokenType;
        private readonly JsonSerializerOptions _options;
        private int _depth;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 分词器
        /// </summary>
        /// <param name="json">JSON 文本</param>
        /// <param name="options">序列化选项</param>
        public JsonTokenizer(ReadOnlySpan<char> json, JsonSerializerOptions options = default)
        {
            _json = json;
            _position = 0;
            _tokenStart = 0;
            _tokenLength = 0;
            _tokenType = JsonTokenType.None;
            _options = options.Equals(default) ? JsonSerializerOptions.Default : options;
            _depth = 0;
        }

        #endregion

        #region 属性

        /// <summary>当前标记类型</summary>
        public JsonTokenType TokenType => _tokenType;

        /// <summary>当前位置</summary>
        public int Position => _position;

        /// <summary>当前深度</summary>
        public int Depth => _depth;

        /// <summary>是否已到达末尾</summary>
        public bool IsEnd => _position >= _json.Length;

        /// <summary>当前标记的原始文本</summary>
        public ReadOnlySpan<char> TokenSpan => _json.Slice(_tokenStart, _tokenLength);

        /// <summary>剩余文本</summary>
        public ReadOnlySpan<char> Remaining => _json.Slice(_position);

        #endregion

        #region 读取方法

        /// <summary>
        /// 读取下一个标记
        /// </summary>
        /// <returns>如果成功读取返回 true</returns>
        public bool Read()
        {
            SkipWhitespaceAndComments();

            if (IsEnd)
            {
                _tokenType = JsonTokenType.EndOfDocument;
                return false;
            }

            char ch = _json[_position];
            _tokenStart = _position;

            switch (ch)
            {
                case JsonFormat.ObjectStart:
                    _tokenType = JsonTokenType.StartObject;
                    _tokenLength = 1;
                    _position++;
                    _depth++;
                    return true;

                case JsonFormat.ObjectEnd:
                    _tokenType = JsonTokenType.EndObject;
                    _tokenLength = 1;
                    _position++;
                    _depth--;
                    return true;

                case JsonFormat.ArrayStart:
                    _tokenType = JsonTokenType.StartArray;
                    _tokenLength = 1;
                    _position++;
                    _depth++;
                    return true;

                case JsonFormat.ArrayEnd:
                    _tokenType = JsonTokenType.EndArray;
                    _tokenLength = 1;
                    _position++;
                    _depth--;
                    return true;

                case JsonFormat.NameSeparator:
                    _tokenType = JsonTokenType.NameSeparator;
                    _tokenLength = 1;
                    _position++;
                    return true;

                case JsonFormat.ValueSeparator:
                    _tokenType = JsonTokenType.ValueSeparator;
                    _tokenLength = 1;
                    _position++;
                    return true;

                case JsonFormat.Quote:
                    return ReadString();

                case 't':
                    return ReadLiteral(JsonFormat.TrueLiteral, JsonTokenType.True);

                case 'f':
                    return ReadLiteral(JsonFormat.FalseLiteral, JsonTokenType.False);

                case 'n':
                    return ReadLiteral(JsonFormat.NullLiteral, JsonTokenType.Null);

                default:
                    if (JsonFormat.IsNumberStart(ch))
                        return ReadNumber();

                    // 处理非标准 JSON (NaN, Infinity)
                    if (_options.AllowNaN)
                    {
                        if (ch == 'N' && TryReadLiteral(JsonFormat.NaN))
                        {
                            _tokenType = JsonTokenType.Number;
                            return true;
                        }
                        if (ch == 'I' && TryReadLiteral(JsonFormat.PositiveInfinity))
                        {
                            _tokenType = JsonTokenType.Number;
                            return true;
                        }
                    }

                    throw new JsonParseException($"意外的字符 '{ch}'", _position);
            }
        }

        /// <summary>
        /// 跳过当前值
        /// </summary>
        public void Skip()
        {
            if (_tokenType == JsonTokenType.StartObject || _tokenType == JsonTokenType.StartArray)
            {
                int depth = 1;
                while (depth > 0 && Read())
                {
                    if (_tokenType == JsonTokenType.StartObject || _tokenType == JsonTokenType.StartArray)
                        depth++;
                    else if (_tokenType == JsonTokenType.EndObject || _tokenType == JsonTokenType.EndArray)
                        depth--;
                }
            }
        }

        #endregion

        #region 值获取方法

        /// <summary>
        /// 获取字符串值
        /// </summary>
        public string GetString()
        {
            if (_tokenType != JsonTokenType.String && _tokenType != JsonTokenType.PropertyName)
                throw new InvalidOperationException($"当前标记不是字符串: {_tokenType}");

            // 跳过引号
            var content = _json.Slice(_tokenStart + 1, _tokenLength - 2);
            return JsonEscaper.Unescape(content);
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        public bool GetBoolean()
        {
            return _tokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                _ => throw new InvalidOperationException($"当前标记不是布尔值: {_tokenType}")
            };
        }

        /// <summary>
        /// 获取整数值
        /// </summary>
        public int GetInt32()
        {
            if (_tokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"当前标记不是数字: {_tokenType}");

            return JsonNumberParser.ParseInt32(TokenSpan);
        }

        /// <summary>
        /// 获取长整数值
        /// </summary>
        public long GetInt64()
        {
            if (_tokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"当前标记不是数字: {_tokenType}");

            return JsonNumberParser.ParseInt64(TokenSpan);
        }

        /// <summary>
        /// 获取单精度浮点值
        /// </summary>
        public float GetSingle()
        {
            if (_tokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"当前标记不是数字: {_tokenType}");

            return JsonNumberParser.ParseSingle(TokenSpan, _options.AllowNaN);
        }

        /// <summary>
        /// 获取双精度浮点值
        /// </summary>
        public double GetDouble()
        {
            if (_tokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"当前标记不是数字: {_tokenType}");

            return JsonNumberParser.ParseDouble(TokenSpan, _options.AllowNaN);
        }

        /// <summary>
        /// 获取十进制值
        /// </summary>
        public decimal GetDecimal()
        {
            if (_tokenType != JsonTokenType.Number)
                throw new InvalidOperationException($"当前标记不是数字: {_tokenType}");

            return JsonNumberParser.ParseDecimal(TokenSpan);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 跳过空白字符和注释
        /// </summary>
        private void SkipWhitespaceAndComments()
        {
            while (_position < _json.Length)
            {
                char ch = _json[_position];

                if (JsonFormat.IsWhitespace(ch))
                {
                    _position++;
                    continue;
                }

                // 处理注释
                if (_options.AllowComments && ch == '/')
                {
                    if (_position + 1 < _json.Length)
                    {
                        char next = _json[_position + 1];
                        if (next == '/')
                        {
                            // 单行注释
                            _position += 2;
                            while (_position < _json.Length && _json[_position] != '\n')
                                _position++;
                            continue;
                        }
                        if (next == '*')
                        {
                            // 多行注释
                            _position += 2;
                            while (_position + 1 < _json.Length)
                            {
                                if (_json[_position] == '*' && _json[_position + 1] == '/')
                                {
                                    _position += 2;
                                    break;
                                }
                                _position++;
                            }
                            continue;
                        }
                    }
                }

                break;
            }
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        private bool ReadString()
        {
            _position++; // 跳过开始引号
            int start = _position;

            while (_position < _json.Length)
            {
                char ch = _json[_position];

                if (ch == JsonFormat.Quote)
                {
                    _tokenLength = _position - _tokenStart + 1;
                    _tokenType = JsonTokenType.String;
                    _position++; // 跳过结束引号
                    return true;
                }

                if (ch == JsonFormat.Escape)
                {
                    _position++;
                    if (_position >= _json.Length)
                        throw new JsonParseException("字符串未正确结束", _position);

                    char escaped = _json[_position];
                    if (escaped == 'u')
                    {
                        // Unicode 转义需要4个十六进制字符
                        if (_position + 4 >= _json.Length)
                            throw new JsonParseException("无效的 Unicode 转义序列", _position);
                        _position += 4;
                    }
                }

                _position++;
            }

            throw new JsonParseException("字符串未正确结束", start);
        }

        /// <summary>
        /// 读取数字
        /// </summary>
        private bool ReadNumber()
        {
            int start = _position;

            // 可选负号
            if (_position < _json.Length && _json[_position] == JsonFormat.Minus)
                _position++;

            // 整数部分
            if (_position < _json.Length && _json[_position] == '0')
            {
                _position++;
            }
            else
            {
                if (_position >= _json.Length || !JsonFormat.IsDigit(_json[_position]))
                    throw new JsonParseException("无效的数字格式", _position);

                while (_position < _json.Length && JsonFormat.IsDigit(_json[_position]))
                    _position++;
            }

            // 小数部分
            if (_position < _json.Length && _json[_position] == JsonFormat.DecimalPoint)
            {
                _position++;
                if (_position >= _json.Length || !JsonFormat.IsDigit(_json[_position]))
                    throw new JsonParseException("小数点后必须有数字", _position);

                while (_position < _json.Length && JsonFormat.IsDigit(_json[_position]))
                    _position++;
            }

            // 指数部分
            if (_position < _json.Length && 
                (_json[_position] == JsonFormat.ExponentLower || _json[_position] == JsonFormat.ExponentUpper))
            {
                _position++;

                // 可选符号
                if (_position < _json.Length && 
                    (_json[_position] == JsonFormat.Plus || _json[_position] == JsonFormat.Minus))
                    _position++;

                if (_position >= _json.Length || !JsonFormat.IsDigit(_json[_position]))
                    throw new JsonParseException("指数部分必须有数字", _position);

                while (_position < _json.Length && JsonFormat.IsDigit(_json[_position]))
                    _position++;
            }

            _tokenLength = _position - start;
            _tokenType = JsonTokenType.Number;
            return true;
        }

        /// <summary>
        /// 读取字面量
        /// </summary>
        private bool ReadLiteral(string literal, JsonTokenType type)
        {
            if (!TryReadLiteral(literal))
                throw new JsonParseException($"期望 '{literal}'", _position);

            _tokenType = type;
            return true;
        }

        /// <summary>
        /// 尝试读取字面量
        /// </summary>
        private bool TryReadLiteral(string literal)
        {
            if (_position + literal.Length > _json.Length)
                return false;

            for (int i = 0; i < literal.Length; i++)
            {
                if (_json[_position + i] != literal[i])
                    return false;
            }

            _tokenLength = literal.Length;
            _position += literal.Length;
            return true;
        }

        #endregion
    }

    #region JSON 标记类型

    /// <summary>
    /// JSON 标记类型
    /// </summary>
    internal enum JsonTokenType
    {
        /// <summary>无/初始状态</summary>
        None,

        /// <summary>对象开始 {</summary>
        StartObject,

        /// <summary>对象结束 }</summary>
        EndObject,

        /// <summary>数组开始 [</summary>
        StartArray,

        /// <summary>数组结束 ]</summary>
        EndArray,

        /// <summary>属性名</summary>
        PropertyName,

        /// <summary>字符串值</summary>
        String,

        /// <summary>数字值</summary>
        Number,

        /// <summary>true</summary>
        True,

        /// <summary>false</summary>
        False,

        /// <summary>null</summary>
        Null,

        /// <summary>名称分隔符 :</summary>
        NameSeparator,

        /// <summary>值分隔符 ,</summary>
        ValueSeparator,

        /// <summary>文档结束</summary>
        EndOfDocument
    }

    #endregion

    #region JSON 解析异常

    /// <summary>
    /// JSON 解析异常
    /// </summary>
    public class JsonParseException : SerializationException
    {
        /// <summary>错误位置</summary>
        public int Position { get; }

        /// <summary>
        /// 创建 JSON 解析异常
        /// </summary>
        public JsonParseException(string message, int position)
            : base($"{message} (位置: {position})", SerializeErrorCode.InvalidFormat)
        {
            Position = position;
        }
    }

    #endregion
}
