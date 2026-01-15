// ==========================================================
// 文件名：JsonWriter.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Text;
using AFramework.Serialization.Internal;

namespace AFramework.Serialization
{
    /// <summary>
    /// JSON 写入器
    /// <para>提供高性能的 JSON 文本生成功能</para>
    /// <para>支持格式化输出、流式写入</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用 JsonStringBuilder 减少内存分配
    /// 2. 支持缩进和美化输出
    /// 3. 自动处理逗号和结构字符
    /// 
    /// 使用示例:
    /// <code>
    /// using var writer = new JsonWriter(JsonSerializerOptions.Pretty);
    /// 
    /// writer.WriteStartObject();
    /// writer.WritePropertyName("name");
    /// writer.WriteString("John");
    /// writer.WritePropertyName("age");
    /// writer.WriteNumber(30);
    /// writer.WriteEndObject();
    /// 
    /// string json = writer.ToString();
    /// </code>
    /// </remarks>
    public sealed class JsonWriter : IDisposable
    {
        #region 字段

        private readonly JsonStringBuilder _builder;
        private readonly JsonSerializerOptions _options;
        private int _depth;
        private bool _needsComma;
        private readonly bool[] _isArrayStack;
        private int _stackTop;
        private bool _disposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 写入器
        /// </summary>
        /// <param name="options">序列化选项</param>
        public JsonWriter(JsonSerializerOptions options = default)
        {
            _options = options.Equals(default) ? JsonSerializerOptions.Default : options;
            _builder = new JsonStringBuilder(_options.InitialBufferSize);
            _depth = 0;
            _needsComma = false;
            _isArrayStack = new bool[_options.MaxDepth];
            _stackTop = -1;
            _disposed = false;
        }

        #endregion

        #region 属性

        /// <summary>当前深度</summary>
        public int Depth => _depth;

        /// <summary>已写入的字符数</summary>
        public int Length => _builder.Length;

        /// <summary>序列化选项</summary>
        public JsonSerializerOptions Options => _options;

        #endregion

        #region 结构写入方法

        /// <summary>
        /// 写入对象开始 {
        /// </summary>
        public void WriteStartObject()
        {
            CheckDepth();
            WriteCommaIfNeeded();
            _builder.Append(JsonFormat.ObjectStart);
            PushContainer(isArray: false);
            _needsComma = false;
        }

        /// <summary>
        /// 写入对象结束 }
        /// </summary>
        public void WriteEndObject()
        {
            PopContainer();
            WriteNewLineAndIndentIfNeeded();
            _builder.Append(JsonFormat.ObjectEnd);
            _needsComma = true;
        }

        /// <summary>
        /// 写入数组开始 [
        /// </summary>
        public void WriteStartArray()
        {
            CheckDepth();
            WriteCommaIfNeeded();
            _builder.Append(JsonFormat.ArrayStart);
            PushContainer(isArray: true);
            _needsComma = false;
        }

        /// <summary>
        /// 写入数组结束 ]
        /// </summary>
        public void WriteEndArray()
        {
            PopContainer();
            if (_options.WriteIndentedArrays)
                WriteNewLineAndIndentIfNeeded();
            _builder.Append(JsonFormat.ArrayEnd);
            _needsComma = true;
        }

        /// <summary>
        /// 写入属性名
        /// </summary>
        /// <param name="name">属性名</param>
        public void WritePropertyName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeeded();

            if (_options.NamingPolicy != JsonPropertyNamingPolicy.None)
            {
                _builder.AppendPropertyName(name, _options.NamingPolicy);
            }
            else
            {
                _builder.AppendPropertyName(name);
            }

            if (_options.Indented)
                _builder.Append(' ');

            _needsComma = false;
        }

        #endregion

        #region 值写入方法

        /// <summary>
        /// 写入 null
        /// </summary>
        public void WriteNull()
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendNull();
            _needsComma = true;
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        public void WriteBoolean(bool value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendBool(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendString(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入字符串 (Span)
        /// </summary>
        public void WriteString(ReadOnlySpan<char> value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendString(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入整数
        /// </summary>
        public void WriteNumber(int value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendInt32(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入长整数
        /// </summary>
        public void WriteNumber(long value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendInt64(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入无符号整数
        /// </summary>
        public void WriteNumber(uint value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendUInt32(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入无符号长整数
        /// </summary>
        public void WriteNumber(ulong value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendUInt64(value);
            _needsComma = true;
        }

        /// <summary>
        /// 写入单精度浮点数
        /// </summary>
        public void WriteNumber(float value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendSingle(value, _options.AllowNaN);
            _needsComma = true;
        }

        /// <summary>
        /// 写入双精度浮点数
        /// </summary>
        public void WriteNumber(double value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendDouble(value, _options.AllowNaN);
            _needsComma = true;
        }

        /// <summary>
        /// 写入十进制数
        /// </summary>
        public void WriteNumber(decimal value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendDecimal(value);
            _needsComma = true;
        }

        #endregion

        #region 便捷写入方法

        /// <summary>
        /// 写入属性 (字符串值)
        /// </summary>
        public void WriteProperty(string name, string value)
        {
            if (_options.IgnoreNullValues && value == null)
                return;

            WritePropertyName(name);
            WriteString(value);
        }

        /// <summary>
        /// 写入属性 (布尔值)
        /// </summary>
        public void WriteProperty(string name, bool value)
        {
            WritePropertyName(name);
            WriteBoolean(value);
        }

        /// <summary>
        /// 写入属性 (整数值)
        /// </summary>
        public void WriteProperty(string name, int value)
        {
            if (_options.IgnoreDefaultValues && value == 0)
                return;

            WritePropertyName(name);
            WriteNumber(value);
        }

        /// <summary>
        /// 写入属性 (长整数值)
        /// </summary>
        public void WriteProperty(string name, long value)
        {
            if (_options.IgnoreDefaultValues && value == 0)
                return;

            WritePropertyName(name);
            WriteNumber(value);
        }

        /// <summary>
        /// 写入属性 (浮点数值)
        /// </summary>
        public void WriteProperty(string name, double value)
        {
            if (_options.IgnoreDefaultValues && value == 0.0)
                return;

            WritePropertyName(name);
            WriteNumber(value);
        }

        /// <summary>
        /// 写入属性 (null 值)
        /// </summary>
        public void WritePropertyNull(string name)
        {
            if (_options.IgnoreNullValues)
                return;

            WritePropertyName(name);
            WriteNull();
        }

        /// <summary>
        /// 写入原始 JSON
        /// </summary>
        /// <param name="json">原始 JSON 文本</param>
        public void WriteRaw(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            WriteCommaIfNeeded();
            _builder.Append(json);
            _needsComma = true;
        }

        /// <summary>
        /// 写入原始 JSON (Span)
        /// </summary>
        public void WriteRaw(ReadOnlySpan<char> json)
        {
            if (json.IsEmpty)
                return;

            WriteCommaIfNeeded();
            _builder.Append(json);
            _needsComma = true;
        }

        #endregion

        #region 特殊类型写入

        /// <summary>
        /// 写入 DateTime
        /// </summary>
        public void WriteDateTime(DateTime value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(value.ToString("O")); // ISO 8601 格式
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入 DateTimeOffset
        /// </summary>
        public void WriteDateTimeOffset(DateTimeOffset value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(value.ToString("O"));
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入 TimeSpan
        /// </summary>
        public void WriteTimeSpan(TimeSpan value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(value.ToString("c")); // 常量格式
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入 Guid
        /// </summary>
        public void WriteGuid(Guid value)
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(value.ToString("D")); // 带连字符格式
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入字节数组 (Base64)
        /// </summary>
        public void WriteBase64(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(Convert.ToBase64String(value));
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入枚举值
        /// </summary>
        public void WriteEnum<T>(T value) where T : struct, Enum
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.Append('"');
            _builder.Append(value.ToString());
            _builder.Append('"');
            _needsComma = true;
        }

        /// <summary>
        /// 写入枚举值 (数字形式)
        /// </summary>
        public void WriteEnumAsNumber<T>(T value) where T : struct, Enum
        {
            WriteCommaIfNeeded();
            WriteNewLineAndIndentIfNeededForValue();
            _builder.AppendInt32(Convert.ToInt32(value));
            _needsComma = true;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 检查深度限制
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckDepth()
        {
            if (_depth >= _options.MaxDepth)
                throw new InvalidOperationException($"超出最大嵌套深度 ({_options.MaxDepth})");
        }

        /// <summary>
        /// 压入容器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushContainer(bool isArray)
        {
            _stackTop++;
            if (_stackTop < _isArrayStack.Length)
                _isArrayStack[_stackTop] = isArray;
            _depth++;
        }

        /// <summary>
        /// 弹出容器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PopContainer()
        {
            if (_stackTop >= 0)
                _stackTop--;
            _depth--;
        }

        /// <summary>
        /// 当前是否在数组中
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsInArray()
        {
            return _stackTop >= 0 && _isArrayStack[_stackTop];
        }

        /// <summary>
        /// 如果需要则写入逗号
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteCommaIfNeeded()
        {
            if (_needsComma)
                _builder.Append(JsonFormat.ValueSeparator);
        }

        /// <summary>
        /// 如果需要则写入换行和缩进
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNewLineAndIndentIfNeeded()
        {
            if (_options.Indented && _depth > 0)
                _builder.AppendNewLineAndIndent(_options.NewLine, _options.IndentString, _depth);
        }

        /// <summary>
        /// 为值写入换行和缩进 (数组元素)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNewLineAndIndentIfNeededForValue()
        {
            if (_options.Indented && _options.WriteIndentedArrays && IsInArray())
                _builder.AppendNewLineAndIndent(_options.NewLine, _options.IndentString, _depth);
        }

        #endregion

        #region 输出方法

        /// <summary>
        /// 获取 JSON 字符串
        /// </summary>
        public override string ToString()
        {
            return _builder.ToString();
        }

        /// <summary>
        /// 获取 UTF-8 字节数组
        /// </summary>
        public byte[] ToUtf8Bytes()
        {
            return _builder.ToUtf8Bytes();
        }

        /// <summary>
        /// 清空写入器
        /// </summary>
        public void Clear()
        {
            _builder.Clear();
            _depth = 0;
            _needsComma = false;
            _stackTop = -1;
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _builder.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}
