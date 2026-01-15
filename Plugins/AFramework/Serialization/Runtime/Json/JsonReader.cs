// ==========================================================
// 文件名：JsonReader.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AFramework.Serialization.Internal;

namespace AFramework.Serialization
{
    /// <summary>
    /// JSON 读取器
    /// <para>提供高性能的 JSON 文本解析功能</para>
    /// <para>支持流式读取、类型安全的值获取</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 基于 JsonTokenizer 实现流式解析
    /// 2. 支持前向只读遍历
    /// 3. 提供类型安全的值获取方法
    /// 
    /// 使用示例:
    /// <code>
    /// var reader = new JsonReader(jsonText);
    /// 
    /// reader.ReadStartObject();
    /// while (reader.ReadPropertyName(out string name))
    /// {
    ///     switch (name)
    ///     {
    ///         case "name":
    ///             string value = reader.ReadString();
    ///             break;
    ///         case "age":
    ///             int age = reader.ReadInt32();
    ///             break;
    ///     }
    /// }
    /// reader.ReadEndObject();
    /// </code>
    /// </remarks>
    public ref struct JsonReader
    {
        #region 字段

        private JsonTokenizer _tokenizer;
        private readonly JsonSerializerOptions _options;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 读取器
        /// </summary>
        /// <param name="json">JSON 文本</param>
        /// <param name="options">序列化选项</param>
        public JsonReader(ReadOnlySpan<char> json, JsonSerializerOptions options = default)
        {
            _options = options.Equals(default) ? JsonSerializerOptions.Default : options;
            _tokenizer = new JsonTokenizer(json, _options);
        }

        /// <summary>
        /// 创建 JSON 读取器
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="options">序列化选项</param>
        public JsonReader(string json, JsonSerializerOptions options = default)
            : this(json.AsSpan(), options)
        {
        }

        #endregion

        #region 属性

        /// <summary>当前标记类型</summary>
        public JsonTokenType TokenType => _tokenizer.TokenType;

        /// <summary>当前深度</summary>
        public int Depth => _tokenizer.Depth;

        /// <summary>当前位置</summary>
        public int Position => _tokenizer.Position;

        /// <summary>是否已到达末尾</summary>
        public bool IsEnd => _tokenizer.IsEnd;

        #endregion

        #region 读取方法

        /// <summary>
        /// 读取下一个标记
        /// </summary>
        /// <returns>如果成功读取返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Read()
        {
            return _tokenizer.Read();
        }

        /// <summary>
        /// 读取对象开始
        /// </summary>
        public void ReadStartObject()
        {
            if (!Read() || TokenType != JsonTokenType.StartObject)
                throw new JsonParseException("期望对象开始 '{'", Position);
        }

        /// <summary>
        /// 读取对象结束
        /// </summary>
        public void ReadEndObject()
        {
            // 跳过可能的尾随逗号
            if (_options.AllowTrailingCommas && TokenType == JsonTokenType.ValueSeparator)
                Read();

            if (TokenType != JsonTokenType.EndObject)
                throw new JsonParseException("期望对象结束 '}'", Position);
        }

        /// <summary>
        /// 读取数组开始
        /// </summary>
        public void ReadStartArray()
        {
            if (!Read() || TokenType != JsonTokenType.StartArray)
                throw new JsonParseException("期望数组开始 '['", Position);
        }

        /// <summary>
        /// 读取数组结束
        /// </summary>
        public void ReadEndArray()
        {
            // 跳过可能的尾随逗号
            if (_options.AllowTrailingCommas && TokenType == JsonTokenType.ValueSeparator)
                Read();

            if (TokenType != JsonTokenType.EndArray)
                throw new JsonParseException("期望数组结束 ']'", Position);
        }

        /// <summary>
        /// 尝试读取对象开始
        /// </summary>
        public bool TryReadStartObject()
        {
            if (Read() && TokenType == JsonTokenType.StartObject)
                return true;
            return false;
        }

        /// <summary>
        /// 尝试读取数组开始
        /// </summary>
        public bool TryReadStartArray()
        {
            if (Read() && TokenType == JsonTokenType.StartArray)
                return true;
            return false;
        }

        /// <summary>
        /// 读取属性名
        /// </summary>
        /// <param name="name">输出属性名</param>
        /// <returns>如果成功读取返回 true，如果遇到对象结束返回 false</returns>
        public bool ReadPropertyName(out string name)
        {
            name = null;

            if (!Read())
                return false;

            // 跳过逗号
            if (TokenType == JsonTokenType.ValueSeparator)
            {
                if (!Read())
                    return false;
            }

            // 检查对象结束
            if (TokenType == JsonTokenType.EndObject)
                return false;

            // 期望字符串 (属性名)
            if (TokenType != JsonTokenType.String)
                throw new JsonParseException("期望属性名", Position);

            name = _tokenizer.GetString();

            // 读取冒号
            if (!Read() || TokenType != JsonTokenType.NameSeparator)
                throw new JsonParseException("期望 ':'", Position);

            return true;
        }

        /// <summary>
        /// 检查是否有更多数组元素
        /// </summary>
        public bool HasMoreArrayElements()
        {
            if (!Read())
                return false;

            // 跳过逗号
            if (TokenType == JsonTokenType.ValueSeparator)
            {
                if (!Read())
                    return false;
            }

            // 检查数组结束
            return TokenType != JsonTokenType.EndArray;
        }

        #endregion

        #region 值读取方法

        /// <summary>
        /// 读取 null
        /// </summary>
        public void ReadNull()
        {
            if (!Read() || TokenType != JsonTokenType.Null)
                throw new JsonParseException("期望 null", Position);
        }

        /// <summary>
        /// 尝试读取 null
        /// </summary>
        public bool TryReadNull()
        {
            if (Read() && TokenType == JsonTokenType.Null)
                return true;
            return false;
        }

        /// <summary>
        /// 读取布尔值
        /// </summary>
        public bool ReadBoolean()
        {
            if (!Read())
                throw new JsonParseException("期望布尔值", Position);

            return TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                _ => throw new JsonParseException("期望布尔值", Position)
            };
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        public string ReadString()
        {
            if (!Read())
                throw new JsonParseException("期望字符串", Position);

            if (TokenType == JsonTokenType.Null)
                return null;

            if (TokenType != JsonTokenType.String)
                throw new JsonParseException("期望字符串", Position);

            return _tokenizer.GetString();
        }

        /// <summary>
        /// 读取 32 位整数
        /// </summary>
        public int ReadInt32()
        {
            if (!Read() || TokenType != JsonTokenType.Number)
                throw new JsonParseException("期望数字", Position);

            return _tokenizer.GetInt32();
        }

        /// <summary>
        /// 读取 64 位整数
        /// </summary>
        public long ReadInt64()
        {
            if (!Read() || TokenType != JsonTokenType.Number)
                throw new JsonParseException("期望数字", Position);

            return _tokenizer.GetInt64();
        }

        /// <summary>
        /// 读取单精度浮点数
        /// </summary>
        public float ReadSingle()
        {
            if (!Read() || TokenType != JsonTokenType.Number)
                throw new JsonParseException("期望数字", Position);

            return _tokenizer.GetSingle();
        }

        /// <summary>
        /// 读取双精度浮点数
        /// </summary>
        public double ReadDouble()
        {
            if (!Read() || TokenType != JsonTokenType.Number)
                throw new JsonParseException("期望数字", Position);

            return _tokenizer.GetDouble();
        }

        /// <summary>
        /// 读取十进制数
        /// </summary>
        public decimal ReadDecimal()
        {
            if (!Read() || TokenType != JsonTokenType.Number)
                throw new JsonParseException("期望数字", Position);

            return _tokenizer.GetDecimal();
        }

        #endregion

        #region 特殊类型读取

        /// <summary>
        /// 读取 DateTime
        /// </summary>
        public DateTime ReadDateTime()
        {
            string str = ReadString();
            if (str == null)
                throw new JsonParseException("DateTime 不能为 null", Position);

            if (DateTime.TryParse(str, out DateTime result))
                return result;

            throw new JsonParseException($"无效的 DateTime 格式: {str}", Position);
        }

        /// <summary>
        /// 读取 DateTimeOffset
        /// </summary>
        public DateTimeOffset ReadDateTimeOffset()
        {
            string str = ReadString();
            if (str == null)
                throw new JsonParseException("DateTimeOffset 不能为 null", Position);

            if (DateTimeOffset.TryParse(str, out DateTimeOffset result))
                return result;

            throw new JsonParseException($"无效的 DateTimeOffset 格式: {str}", Position);
        }

        /// <summary>
        /// 读取 TimeSpan
        /// </summary>
        public TimeSpan ReadTimeSpan()
        {
            string str = ReadString();
            if (str == null)
                throw new JsonParseException("TimeSpan 不能为 null", Position);

            if (TimeSpan.TryParse(str, out TimeSpan result))
                return result;

            throw new JsonParseException($"无效的 TimeSpan 格式: {str}", Position);
        }

        /// <summary>
        /// 读取 Guid
        /// </summary>
        public Guid ReadGuid()
        {
            string str = ReadString();
            if (str == null)
                throw new JsonParseException("Guid 不能为 null", Position);

            if (Guid.TryParse(str, out Guid result))
                return result;

            throw new JsonParseException($"无效的 Guid 格式: {str}", Position);
        }

        /// <summary>
        /// 读取 Base64 字节数组
        /// </summary>
        public byte[] ReadBase64()
        {
            string str = ReadString();
            if (str == null)
                return null;

            try
            {
                return Convert.FromBase64String(str);
            }
            catch (FormatException)
            {
                throw new JsonParseException("无效的 Base64 格式", Position);
            }
        }

        /// <summary>
        /// 读取枚举值
        /// </summary>
        public T ReadEnum<T>() where T : struct, Enum
        {
            if (!Read())
                throw new JsonParseException("期望枚举值", Position);

            // 字符串形式
            if (TokenType == JsonTokenType.String)
            {
                string str = _tokenizer.GetString();
                if (Enum.TryParse<T>(str, true, out T result))
                    return result;
                throw new JsonParseException($"无效的枚举值: {str}", Position);
            }

            // 数字形式
            if (TokenType == JsonTokenType.Number)
            {
                int value = _tokenizer.GetInt32();
                return (T)Enum.ToObject(typeof(T), value);
            }

            throw new JsonParseException("期望枚举值 (字符串或数字)", Position);
        }

        #endregion

        #region 跳过方法

        /// <summary>
        /// 跳过当前值
        /// </summary>
        public void Skip()
        {
            if (!Read())
                return;

            SkipCurrentValue();
        }

        /// <summary>
        /// 跳过当前值 (已读取标记)
        /// </summary>
        private void SkipCurrentValue()
        {
            switch (TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    _tokenizer.Skip();
                    break;

                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                    // 简单值，已经读取完毕
                    break;

                default:
                    throw new JsonParseException($"意外的标记类型: {TokenType}", Position);
            }
        }

        /// <summary>
        /// 跳过属性值
        /// </summary>
        public void SkipPropertyValue()
        {
            Skip();
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 读取当前值为 object (动态类型)
        /// </summary>
        public object ReadValue()
        {
            if (!Read())
                return null;

            return ReadCurrentValue();
        }

        /// <summary>
        /// 读取当前值 (已读取标记)
        /// </summary>
        private object ReadCurrentValue()
        {
            switch (TokenType)
            {
                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.String:
                    return _tokenizer.GetString();

                case JsonTokenType.Number:
                    // 尝试解析为整数，否则解析为浮点数
                    var span = _tokenizer.TokenSpan;
                    if (JsonNumberParser.IsInteger(span))
                    {
                        if (JsonNumberParser.TryParseInt32(span, out int intValue))
                            return intValue;
                        if (JsonNumberParser.TryParseInt64(span, out long longValue))
                            return longValue;
                    }
                    return _tokenizer.GetDouble();

                case JsonTokenType.StartObject:
                    return ReadObjectAsDictionary();

                case JsonTokenType.StartArray:
                    return ReadArrayAsList();

                default:
                    throw new JsonParseException($"意外的标记类型: {TokenType}", Position);
            }
        }

        /// <summary>
        /// 读取对象为字典
        /// </summary>
        private Dictionary<string, object> ReadObjectAsDictionary()
        {
            var dict = new Dictionary<string, object>();

            while (true)
            {
                if (!Read())
                    throw new JsonParseException("对象未正确结束", Position);

                if (TokenType == JsonTokenType.ValueSeparator)
                    continue;

                if (TokenType == JsonTokenType.EndObject)
                    break;

                if (TokenType != JsonTokenType.String)
                    throw new JsonParseException("期望属性名", Position);

                string key = _tokenizer.GetString();

                if (!Read() || TokenType != JsonTokenType.NameSeparator)
                    throw new JsonParseException("期望 ':'", Position);

                if (!Read())
                    throw new JsonParseException("期望值", Position);

                dict[key] = ReadCurrentValue();
            }

            return dict;
        }

        /// <summary>
        /// 读取数组为列表
        /// </summary>
        private List<object> ReadArrayAsList()
        {
            var list = new List<object>();

            while (true)
            {
                if (!Read())
                    throw new JsonParseException("数组未正确结束", Position);

                if (TokenType == JsonTokenType.ValueSeparator)
                    continue;

                if (TokenType == JsonTokenType.EndArray)
                    break;

                list.Add(ReadCurrentValue());
            }

            return list;
        }

        #endregion
    }
}
