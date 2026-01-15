// ==========================================================
// 文件名：JsonStringBuilder.cs
// 命名空间: AFramework.Serialization.Internal
// 依赖: System, System.Buffers
// ==========================================================

using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.Serialization.Internal
{
    /// <summary>
    /// JSON 字符串构建器
    /// <para>高性能字符串构建，支持池化缓冲区</para>
    /// <para>专为 JSON 序列化优化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用 ArrayPool 减少内存分配
    /// 2. 支持直接写入各种数据类型
    /// 3. 自动扩容，避免频繁重新分配
    /// 
    /// 使用示例:
    /// <code>
    /// using var builder = new JsonStringBuilder(256);
    /// builder.Append('{');
    /// builder.AppendPropertyName("name");
    /// builder.AppendString("John");
    /// builder.Append('}');
    /// string json = builder.ToString();
    /// </code>
    /// </remarks>
    internal sealed class JsonStringBuilder : IDisposable
    {
        #region 常量

        /// <summary>默认初始容量</summary>
        private const int DefaultCapacity = 256;

        /// <summary>最大容量</summary>
        private const int MaxCapacity = 1024 * 1024 * 100; // 100MB

        /// <summary>数字格式信息</summary>
        private static readonly NumberFormatInfo s_numberFormat = CultureInfo.InvariantCulture.NumberFormat;

        #endregion

        #region 字段

        private char[] _buffer;
        private int _position;
        private bool _disposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 字符串构建器
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        public JsonStringBuilder(int initialCapacity = DefaultCapacity)
        {
            if (initialCapacity <= 0)
                initialCapacity = DefaultCapacity;

            _buffer = ArrayPool<char>.Shared.Rent(initialCapacity);
            _position = 0;
            _disposed = false;
        }

        #endregion

        #region 属性

        /// <summary>当前长度</summary>
        public int Length => _position;

        /// <summary>当前容量</summary>
        public int Capacity => _buffer.Length;

        /// <summary>已写入的内容</summary>
        public ReadOnlySpan<char> WrittenSpan => _buffer.AsSpan(0, _position);

        #endregion

        #region 基础追加方法

        /// <summary>
        /// 追加单个字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char ch)
        {
            EnsureCapacity(1);
            _buffer[_position++] = ch;
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            Append(value.AsSpan());
        }

        /// <summary>
        /// 追加字符 Span
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return;

            EnsureCapacity(value.Length);
            value.CopyTo(_buffer.AsSpan(_position));
            _position += value.Length;
        }

        /// <summary>
        /// 追加重复字符
        /// </summary>
        public void Append(char ch, int count)
        {
            if (count <= 0)
                return;

            EnsureCapacity(count);
            for (int i = 0; i < count; i++)
                _buffer[_position++] = ch;
        }

        #endregion

        #region JSON 特定追加方法

        /// <summary>
        /// 追加 null 字面量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendNull()
        {
            Append(JsonFormat.NullLiteral);
        }

        /// <summary>
        /// 追加 true 字面量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendTrue()
        {
            Append(JsonFormat.TrueLiteral);
        }

        /// <summary>
        /// 追加 false 字面量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFalse()
        {
            Append(JsonFormat.FalseLiteral);
        }

        /// <summary>
        /// 追加布尔值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendBool(bool value)
        {
            if (value)
                AppendTrue();
            else
                AppendFalse();
        }

        /// <summary>
        /// 追加 JSON 字符串 (带引号和转义)
        /// </summary>
        public void AppendString(string value)
        {
            if (value == null)
            {
                AppendNull();
                return;
            }

            AppendString(value.AsSpan());
        }

        /// <summary>
        /// 追加 JSON 字符串 (带引号和转义)
        /// </summary>
        public void AppendString(ReadOnlySpan<char> value)
        {
            Append(JsonFormat.Quote);

            if (!value.IsEmpty)
            {
                // 检查是否需要转义
                if (JsonEscaper.NeedsEscaping(value))
                {
                    JsonEscaper.EscapeTo(value, this);
                }
                else
                {
                    Append(value);
                }
            }

            Append(JsonFormat.Quote);
        }

        /// <summary>
        /// 追加属性名
        /// </summary>
        public void AppendPropertyName(string name)
        {
            AppendString(name);
            Append(JsonFormat.NameSeparator);
        }

        /// <summary>
        /// 追加属性名 (带命名策略)
        /// </summary>
        public void AppendPropertyName(string name, JsonPropertyNamingPolicy policy)
        {
            string convertedName = ConvertPropertyName(name, policy);
            AppendPropertyName(convertedName);
        }

        /// <summary>
        /// 追加 Unicode 转义序列
        /// </summary>
        public void AppendUnicodeEscape(char ch)
        {
            EnsureCapacity(6);
            _buffer[_position++] = '\\';
            _buffer[_position++] = 'u';
            _buffer[_position++] = JsonFormat.IntToHex((ch >> 12) & 0xF);
            _buffer[_position++] = JsonFormat.IntToHex((ch >> 8) & 0xF);
            _buffer[_position++] = JsonFormat.IntToHex((ch >> 4) & 0xF);
            _buffer[_position++] = JsonFormat.IntToHex(ch & 0xF);
        }

        #endregion

        #region 数字追加方法

        /// <summary>
        /// 追加整数
        /// </summary>
        public void AppendInt32(int value)
        {
            // 快速路径：小数字
            if (value >= 0 && value < 10)
            {
                Append((char)('0' + value));
                return;
            }

            Span<char> buffer = stackalloc char[16];
            if (value.TryFormat(buffer, out int written, default, s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString(s_numberFormat));
            }
        }

        /// <summary>
        /// 追加长整数
        /// </summary>
        public void AppendInt64(long value)
        {
            Span<char> buffer = stackalloc char[24];
            if (value.TryFormat(buffer, out int written, default, s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString(s_numberFormat));
            }
        }

        /// <summary>
        /// 追加无符号整数
        /// </summary>
        public void AppendUInt32(uint value)
        {
            Span<char> buffer = stackalloc char[16];
            if (value.TryFormat(buffer, out int written, default, s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString(s_numberFormat));
            }
        }

        /// <summary>
        /// 追加无符号长整数
        /// </summary>
        public void AppendUInt64(ulong value)
        {
            Span<char> buffer = stackalloc char[24];
            if (value.TryFormat(buffer, out int written, default, s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString(s_numberFormat));
            }
        }

        /// <summary>
        /// 追加单精度浮点数
        /// </summary>
        public void AppendSingle(float value, bool allowNaN = false)
        {
            if (float.IsNaN(value))
            {
                if (allowNaN)
                    Append(JsonFormat.NaN);
                else
                    throw new ArgumentException("JSON 不支持 NaN 值");
                return;
            }

            if (float.IsPositiveInfinity(value))
            {
                if (allowNaN)
                    Append(JsonFormat.PositiveInfinity);
                else
                    throw new ArgumentException("JSON 不支持 Infinity 值");
                return;
            }

            if (float.IsNegativeInfinity(value))
            {
                if (allowNaN)
                    Append(JsonFormat.NegativeInfinity);
                else
                    throw new ArgumentException("JSON 不支持 -Infinity 值");
                return;
            }

            Span<char> buffer = stackalloc char[32];
            if (value.TryFormat(buffer, out int written, "G9", s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString("G9", s_numberFormat));
            }
        }

        /// <summary>
        /// 追加双精度浮点数
        /// </summary>
        public void AppendDouble(double value, bool allowNaN = false)
        {
            if (double.IsNaN(value))
            {
                if (allowNaN)
                    Append(JsonFormat.NaN);
                else
                    throw new ArgumentException("JSON 不支持 NaN 值");
                return;
            }

            if (double.IsPositiveInfinity(value))
            {
                if (allowNaN)
                    Append(JsonFormat.PositiveInfinity);
                else
                    throw new ArgumentException("JSON 不支持 Infinity 值");
                return;
            }

            if (double.IsNegativeInfinity(value))
            {
                if (allowNaN)
                    Append(JsonFormat.NegativeInfinity);
                else
                    throw new ArgumentException("JSON 不支持 -Infinity 值");
                return;
            }

            Span<char> buffer = stackalloc char[32];
            if (value.TryFormat(buffer, out int written, "G17", s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString("G17", s_numberFormat));
            }
        }

        /// <summary>
        /// 追加十进制数
        /// </summary>
        public void AppendDecimal(decimal value)
        {
            Span<char> buffer = stackalloc char[32];
            if (value.TryFormat(buffer, out int written, default, s_numberFormat))
            {
                Append(buffer.Slice(0, written));
            }
            else
            {
                Append(value.ToString(s_numberFormat));
            }
        }

        #endregion

        #region 格式化方法

        /// <summary>
        /// 追加换行和缩进
        /// </summary>
        public void AppendNewLineAndIndent(string newLine, string indent, int depth)
        {
            Append(newLine);
            for (int i = 0; i < depth; i++)
                Append(indent);
        }

        /// <summary>
        /// 追加缩进
        /// </summary>
        public void AppendIndent(string indent, int depth)
        {
            for (int i = 0; i < depth; i++)
                Append(indent);
        }

        #endregion

        #region 容量管理

        /// <summary>
        /// 确保有足够的容量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int additionalCapacity)
        {
            int required = _position + additionalCapacity;
            if (required <= _buffer.Length)
                return;

            Grow(required);
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private void Grow(int minimumCapacity)
        {
            int newCapacity = Math.Max(_buffer.Length * 2, minimumCapacity);
            newCapacity = Math.Min(newCapacity, MaxCapacity);

            if (newCapacity < minimumCapacity)
                throw new InvalidOperationException($"JSON 字符串超过最大长度限制 ({MaxCapacity})");

            var newBuffer = ArrayPool<char>.Shared.Rent(newCapacity);
            _buffer.AsSpan(0, _position).CopyTo(newBuffer);
            ArrayPool<char>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }

        /// <summary>
        /// 清空内容
        /// </summary>
        public void Clear()
        {
            _position = 0;
        }

        #endregion

        #region 输出方法

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override string ToString()
        {
            return new string(_buffer, 0, _position);
        }

        /// <summary>
        /// 转换为字节数组 (UTF-8)
        /// </summary>
        public byte[] ToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(_buffer, 0, _position);
        }

        /// <summary>
        /// 写入到 Span
        /// </summary>
        public int WriteTo(Span<char> destination)
        {
            if (destination.Length < _position)
                throw new ArgumentException("目标缓冲区太小");

            _buffer.AsSpan(0, _position).CopyTo(destination);
            return _position;
        }

        #endregion

        #region 属性名转换

        /// <summary>
        /// 转换属性名
        /// </summary>
        private static string ConvertPropertyName(string name, JsonPropertyNamingPolicy policy)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return policy switch
            {
                JsonPropertyNamingPolicy.CamelCase => ToCamelCase(name),
                JsonPropertyNamingPolicy.SnakeCase => ToSnakeCase(name),
                JsonPropertyNamingPolicy.KebabCase => ToKebabCase(name),
                JsonPropertyNamingPolicy.UpperCase => name.ToUpperInvariant(),
                JsonPropertyNamingPolicy.LowerCase => name.ToLowerInvariant(),
                _ => name
            };
        }

        /// <summary>
        /// 转换为驼峰命名
        /// </summary>
        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
                return name;

            var chars = name.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);
            return new string(chars);
        }

        /// <summary>
        /// 转换为蛇形命名
        /// </summary>
        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new StringBuilder(name.Length + 4);
            for (int i = 0; i < name.Length; i++)
            {
                char ch = name[i];
                if (char.IsUpper(ch))
                {
                    if (i > 0)
                        sb.Append('_');
                    sb.Append(char.ToLowerInvariant(ch));
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换为短横线命名
        /// </summary>
        private static string ToKebabCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var sb = new StringBuilder(name.Length + 4);
            for (int i = 0; i < name.Length; i++)
            {
                char ch = name[i];
                if (char.IsUpper(ch))
                {
                    if (i > 0)
                        sb.Append('-');
                    sb.Append(char.ToLowerInvariant(ch));
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
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
                ArrayPool<char>.Shared.Return(_buffer);
                _buffer = null;
                _disposed = true;
            }
        }

        #endregion
    }
}
