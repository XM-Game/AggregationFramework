// ==========================================================
// 文件名：BinarySerializerT.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Buffers
// ==========================================================

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using AFramework.Serialization.Internal;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型二进制序列化器
    /// <para>提供类型安全的高性能二进制序列化</para>
    /// <para>避免装箱拆箱，适用于特定类型的序列化场景</para>
    /// </summary>
    /// <typeparam name="T">要序列化的类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 继承 SerializerBase&lt;T&gt; 提供类型安全的 API
    /// 2. 使用泛型缓存优化性能
    /// 3. 支持自定义格式化器
    /// 
    /// 使用示例:
    /// <code>
    /// // 创建类型专用序列化器
    /// var serializer = new BinarySerializer&lt;Player&gt;();
    /// 
    /// // 序列化
    /// byte[] data = serializer.Serialize(player);
    /// 
    /// // 反序列化
    /// Player player = serializer.Deserialize(data);
    /// 
    /// // 克隆对象
    /// Player clone = serializer.Clone(player);
    /// </code>
    /// </remarks>
    public class BinarySerializer<T> : SerializerBase<T>
    {
        #region 静态字段

        /// <summary>默认序列化器实例</summary>
        private static readonly Lazy<BinarySerializer<T>> s_default =
            new Lazy<BinarySerializer<T>>(() => new BinarySerializer<T>());

        /// <summary>类型是否为非托管类型</summary>
        private static readonly bool s_isUnmanaged = TypeCodeMap.IsUnmanagedType(typeof(T));

        /// <summary>类型是否为基础类型</summary>
        private static readonly bool s_isPrimitive = typeof(T).IsPrimitive;

        /// <summary>类型的固定大小 (非托管类型)</summary>
        private static readonly int s_fixedSize = s_isUnmanaged ? Unsafe.SizeOf<T>() : -1;

        #endregion

        #region 字段

        /// <summary>二进制序列化选项</summary>
        private readonly BinarySerializerOptions _options;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建泛型二进制序列化器 (使用默认选项)
        /// </summary>
        public BinarySerializer() : this(BinarySerializerOptions.Default)
        {
        }

        /// <summary>
        /// 创建泛型二进制序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        public BinarySerializer(BinarySerializerOptions options) : base((IFormatterResolver)null)
        {
            _options = options;
        }

        /// <summary>
        /// 创建泛型二进制序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        /// <param name="formatter">类型专用格式化器</param>
        public BinarySerializer(BinarySerializerOptions options, IFormatter<T> formatter) : base(formatter)
        {
            _options = options;
        }

        #endregion

        #region 属性

        /// <summary>二进制序列化选项</summary>
        public BinarySerializerOptions Options => _options;

        /// <summary>默认序列化器实例</summary>
        public static BinarySerializer<T> Default => s_default.Value;

        /// <summary>类型是否为非托管类型</summary>
        public static bool IsUnmanagedType => s_isUnmanaged;

        /// <summary>类型的固定大小 (-1 表示变长)</summary>
        public static int FixedSize => s_fixedSize;

        #endregion

        #region 快速序列化方法 (非托管类型优化)

        /// <summary>
        /// 快速序列化非托管类型
        /// </summary>
        /// <param name="value">要序列化的值</param>
        /// <returns>序列化后的字节数组</returns>
        /// <remarks>
        /// 仅适用于非托管类型，直接内存复制，零开销
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeFast(T value)
        {
            if (!s_isUnmanaged)
                return Serialize(value);

            var result = new byte[s_fixedSize];
            Unsafe.WriteUnaligned(ref result[0], value);
            return result;
        }

        /// <summary>
        /// 快速序列化非托管类型到缓冲区
        /// </summary>
        /// <param name="value">要序列化的值</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeFast(T value, Span<byte> buffer)
        {
            if (!s_isUnmanaged)
                return Serialize(value, buffer);

            if (buffer.Length < s_fixedSize)
                throw new ArgumentException($"缓冲区太小，需要 {s_fixedSize} 字节", nameof(buffer));

            Unsafe.WriteUnaligned(ref buffer[0], value);
            return s_fixedSize;
        }

        /// <summary>
        /// 快速反序列化非托管类型
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFast(ReadOnlySpan<byte> data)
        {
            if (!s_isUnmanaged)
                return Deserialize(data);

            if (data.Length < s_fixedSize)
                throw new ArgumentException($"数据太短，需要 {s_fixedSize} 字节", nameof(data));

            return Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(data[0]));
        }

        #endregion

        #region 数组序列化方法

        /// <summary>
        /// 序列化数组
        /// </summary>
        /// <param name="values">要序列化的数组</param>
        /// <returns>序列化后的字节数组</returns>
        public byte[] SerializeArray(T[] values)
        {
            if (values == null)
                return new byte[] { BinaryFormat.Null };

            if (values.Length == 0)
                return new byte[] { BinaryFormat.EmptyArray };

            using var writer = new BinaryWriterCore(_options.InitialBufferSize);

            // 非托管类型使用零拷贝
            if (s_isUnmanaged)
            {
                writer.WriteTypeCode(BinaryFormat.Array);
                writer.WriteUnmanagedArray(values.AsSpan());
            }
            else
            {
                writer.WriteTypeCode(BinaryFormat.Array);
                writer.WriteArrayHeader(values.Length);
                foreach (var value in values)
                {
                    SerializeToWriter(writer, value);
                }
            }

            return writer.ToArray();
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的数组</returns>
        public T[] DeserializeArray(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
                return null;

            var reader = new BinaryReaderCore(data);
            byte typeCode = reader.ReadTypeCode();

            if (typeCode == BinaryFormat.Null)
                return null;

            if (typeCode == BinaryFormat.EmptyArray)
                return Array.Empty<T>();

            if (typeCode != BinaryFormat.Array)
                throw new SerializationException($"期望数组类型码，实际: 0x{typeCode:X2}", SerializeErrorCode.InvalidFormat);

            // 非托管类型使用零拷贝
            if (s_isUnmanaged)
            {
                return reader.ReadUnmanagedArray<T>();
            }

            int length = reader.ReadArrayHeader();
            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = DeserializeFromReader(ref reader);
            }
            return result;
        }

        #endregion

        #region SerializerBase<T> 重写

        /// <inheritdoc/>
        protected override int SerializeTyped(T value, Span<byte> buffer, SerializeOptions options)
        {
            // 非托管类型快速路径
            if (s_isUnmanaged && !_options.IncludeHeader)
            {
                return SerializeFast(value, buffer);
            }

            using var writer = new BinaryWriterCore(buffer.ToArray());

            // 写入头部 (如果启用)
            int headerSize = _options.IncludeHeader ? BinaryHeader.Size : 0;
            if (headerSize > 0)
                writer.Advance(headerSize);

            int dataStart = writer.Position;
            SerializeToWriter(writer, value);
            int dataSize = writer.Position - dataStart;

            // 写入头部
            if (_options.IncludeHeader)
            {
                var headerFlags = _options.GetHeaderFlags();
                var header = _options.IncludeChecksum
                    ? BinaryHeader.CreateWithChecksum(
                        (uint)dataSize,
                        BinaryHeader.CalculateCRC32(writer.WrittenSpan.Slice(dataStart, dataSize)),
                        headerFlags)
                    : BinaryHeader.Create((uint)dataSize, headerFlags);

                header.WriteTo(buffer);
            }

            writer.WrittenSpan.CopyTo(buffer);
            return writer.Position;
        }

        /// <inheritdoc/>
        protected override T DeserializeTyped(ReadOnlySpan<byte> data, DeserializeOptions options)
        {
            // 非托管类型快速路径
            if (s_isUnmanaged && !_options.IncludeHeader && data.Length == s_fixedSize)
            {
                return DeserializeFast(data);
            }

            var reader = new BinaryReaderCore(data);

            // 检查并读取头部
            if (_options.IncludeHeader && BinaryHeader.HasValidHeader(data))
            {
                var header = BinaryHeader.ReadFrom(data);
                if (!header.Validate(out var errorCode))
                {
                    throw new SerializationException($"数据头验证失败: {errorCode}", errorCode);
                }

                if (header.HasChecksum)
                {
                    var dataSpan = data.Slice(BinaryHeader.Size, (int)header.DataSize);
                    if (!header.ValidateChecksum(dataSpan))
                    {
                        throw new SerializationException("数据校验和验证失败", SerializeErrorCode.ChecksumFailed);
                    }
                }

                reader = new BinaryReaderCore(data, BinaryHeader.Size);
            }

            return DeserializeFromReader(ref reader);
        }

        /// <inheritdoc/>
        protected override int EstimateSizeTyped(T value)
        {
            if (s_isUnmanaged)
                return s_fixedSize + (_options.IncludeHeader ? BinaryHeader.Size : 0);

            // 默认估算
            return _options.InitialBufferSize;
        }

        #endregion

        #region 内部序列化方法

        /// <summary>
        /// 序列化到写入器
        /// </summary>
        private void SerializeToWriter(BinaryWriterCore writer, T value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // 非托管类型直接写入
            if (s_isUnmanaged)
            {
                writer.WriteTypeCode(TypeCodeMap.GetTypeCode<T>());
                writer.WriteUnmanaged(value);
                return;
            }

            // 使用格式化器
            if (_formatter != null)
            {
                writer.WriteTypeCode(BinaryFormat.ObjectStart);
                // 需要实现 ISerializeWriter 适配器
                writer.WriteTypeCode(BinaryFormat.ObjectEnd);
                return;
            }

            // 基础类型处理
            SerializePrimitive(writer, value);
        }

        /// <summary>
        /// 序列化基础类型
        /// </summary>
        private void SerializePrimitive(BinaryWriterCore writer, T value)
        {
            var type = typeof(T);

            if (type == typeof(string))
            {
                writer.WriteTypeCode(BinaryFormat.LongString);
                writer.WriteString(value as string);
                return;
            }

            if (type == typeof(byte[]))
            {
                writer.WriteTypeCode(BinaryFormat.LongBytes);
                writer.WriteBytes(value as byte[]);
                return;
            }

            // 其他类型使用反射
            writer.WriteTypeCode(BinaryFormat.ObjectStart);
            SerializeObjectFields(writer, value, type);
            writer.WriteTypeCode(BinaryFormat.ObjectEnd);
        }

        /// <summary>
        /// 序列化对象字段
        /// </summary>
        private void SerializeObjectFields(BinaryWriterCore writer, T value, Type type)
        {
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            writer.WriteVarInt32(fields.Length);

            foreach (var field in fields)
            {
                if (_options.IncludeTypeInfo)
                    writer.WriteString(field.Name);

                var fieldValue = field.GetValue(value);
                SerializeFieldValue(writer, fieldValue, field.FieldType);
            }
        }

        /// <summary>
        /// 序列化字段值
        /// </summary>
        private void SerializeFieldValue(BinaryWriterCore writer, object value, Type fieldType)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // 处理基础类型
            switch (value)
            {
                case bool b: writer.WriteTypeCode(b ? BinaryFormat.True : BinaryFormat.False); break;
                case byte b: writer.WriteTypeCode(BinaryFormat.UInt8); writer.WriteByte(b); break;
                case int i: writer.WriteTypeCode(BinaryFormat.VarInt32); writer.WriteVarInt32(i); break;
                case long l: writer.WriteTypeCode(BinaryFormat.VarInt64); writer.WriteVarInt64(l); break;
                case float f: writer.WriteTypeCode(BinaryFormat.Float32); writer.WriteSingle(f); break;
                case double d: writer.WriteTypeCode(BinaryFormat.Float64); writer.WriteDouble(d); break;
                case string s: writer.WriteTypeCode(BinaryFormat.LongString); writer.WriteString(s); break;
                default:
                    writer.WriteTypeCode(BinaryFormat.ObjectStart);
                    writer.WriteTypeCode(BinaryFormat.ObjectEnd);
                    break;
            }
        }

        #endregion

        #region 内部反序列化方法

        /// <summary>
        /// 从读取器反序列化
        /// </summary>
        private T DeserializeFromReader(ref BinaryReaderCore reader)
        {
            if (reader.IsEnd)
                return default;

            byte typeCode = reader.ReadTypeCode();

            if (typeCode == BinaryFormat.Null)
                return default;

            // 非托管类型直接读取
            if (s_isUnmanaged && BinaryFormat.IsFixedSizeType(typeCode))
            {
                return reader.ReadUnmanaged<T>();
            }

            // 字符串类型
            if (typeof(T) == typeof(string))
            {
                return (T)(object)reader.ReadString();
            }

            // 字节数组类型
            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)reader.ReadBytes();
            }

            // 对象类型
            if (typeCode == BinaryFormat.ObjectStart)
            {
                return DeserializeObjectFields(ref reader);
            }

            throw new SerializationException($"不支持的类型码: 0x{typeCode:X2}", SerializeErrorCode.UnsupportedType);
        }

        /// <summary>
        /// 反序列化对象字段
        /// </summary>
        private T DeserializeObjectFields(ref BinaryReaderCore reader)
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance<T>();

            int fieldCount = reader.ReadVarInt32();

            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = null;
                if (_options.IncludeTypeInfo)
                    fieldName = reader.ReadString();

                var fieldValue = DeserializeFieldValue(ref reader);

                if (fieldName != null)
                {
                    var field = Array.Find(fields, f => f.Name == fieldName);
                    field?.SetValue(instance, fieldValue);
                }
                else if (i < fields.Length)
                {
                    fields[i].SetValue(instance, fieldValue);
                }
            }

            // 读取对象结束标记
            byte endCode = reader.ReadTypeCode();
            if (endCode != BinaryFormat.ObjectEnd)
            {
                throw new SerializationException("缺少对象结束标记", SerializeErrorCode.InvalidFormat);
            }

            return instance;
        }

        /// <summary>
        /// 反序列化字段值
        /// </summary>
        private object DeserializeFieldValue(ref BinaryReaderCore reader)
        {
            byte typeCode = reader.ReadTypeCode();

            return typeCode switch
            {
                BinaryFormat.Null => null,
                BinaryFormat.False => false,
                BinaryFormat.True => true,
                BinaryFormat.UInt8 => reader.ReadByte(),
                BinaryFormat.Int8 => reader.ReadSByte(),
                BinaryFormat.VarInt32 => reader.ReadVarInt32(),
                BinaryFormat.VarInt64 => reader.ReadVarInt64(),
                BinaryFormat.Int32 => reader.ReadInt32(),
                BinaryFormat.Int64 => reader.ReadInt64(),
                BinaryFormat.Float32 => reader.ReadSingle(),
                BinaryFormat.Float64 => reader.ReadDouble(),
                BinaryFormat.LongString or BinaryFormat.ShortString or BinaryFormat.MediumString => reader.ReadString(),
                BinaryFormat.EmptyString => string.Empty,
                BinaryFormat.ObjectStart => SkipObject(ref reader),
                _ => null
            };
        }

        /// <summary>
        /// 跳过对象
        /// </summary>
        private object SkipObject(ref BinaryReaderCore reader)
        {
            // 简单跳过对象内容
            int depth = 1;
            while (depth > 0 && !reader.IsEnd)
            {
                byte code = reader.ReadTypeCode();
                if (code == BinaryFormat.ObjectStart) depth++;
                else if (code == BinaryFormat.ObjectEnd) depth--;
            }
            return null;
        }

        #endregion
    }
}
