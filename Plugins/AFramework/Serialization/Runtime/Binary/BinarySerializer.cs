// ==========================================================
// 文件名：BinarySerializer.cs
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
    /// 二进制序列化器
    /// <para>提供高性能的二进制序列化和反序列化功能</para>
    /// <para>支持基础类型、集合、自定义对象的序列化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 继承 SerializerBase 复用基础功能
    /// 2. 使用 VarInt 编码减少数据大小
    /// 3. 支持循环引用检测和字符串内化
    /// 4. 提供静态方法便于快速使用
    /// 
    /// 使用示例:
    /// <code>
    /// // 静态方法 (推荐)
    /// byte[] data = BinarySerializer.Serialize(player);
    /// Player player = BinarySerializer.Deserialize&lt;Player&gt;(data);
    /// 
    /// // 实例方法
    /// var serializer = new BinarySerializer(options);
    /// byte[] data = serializer.Serialize(player);
    /// </code>
    /// </remarks>
    public class BinarySerializer : SerializerBase
    {
        #region 静态字段

        /// <summary>默认序列化器实例</summary>
        private static readonly Lazy<BinarySerializer> s_default = 
            new Lazy<BinarySerializer>(() => new BinarySerializer());

        /// <summary>网络优化序列化器实例</summary>
        private static readonly Lazy<BinarySerializer> s_network = 
            new Lazy<BinarySerializer>(() => new BinarySerializer(BinarySerializerOptions.ForNetwork));

        /// <summary>存储优化序列化器实例</summary>
        private static readonly Lazy<BinarySerializer> s_storage = 
            new Lazy<BinarySerializer>(() => new BinarySerializer(BinarySerializerOptions.ForStorage));

        #endregion

        #region 字段

        /// <summary>二进制序列化选项</summary>
        private readonly BinarySerializerOptions _options;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建二进制序列化器 (使用默认选项)
        /// </summary>
        public BinarySerializer() : this(BinarySerializerOptions.Default)
        {
        }

        /// <summary>
        /// 创建二进制序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        public BinarySerializer(BinarySerializerOptions options) : base(null)
        {
            _options = options;
        }

        /// <summary>
        /// 创建二进制序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        /// <param name="resolver">格式化器解析器</param>
        public BinarySerializer(BinarySerializerOptions options, IFormatterResolver resolver) : base(resolver)
        {
            _options = options;
        }

        #endregion

        #region 属性

        /// <summary>二进制序列化选项</summary>
        public BinarySerializerOptions Options => _options;

        /// <summary>默认序列化器实例</summary>
        public static BinarySerializer Default => s_default.Value;

        /// <summary>网络优化序列化器实例</summary>
        public static BinarySerializer Network => s_network.Value;

        /// <summary>存储优化序列化器实例</summary>
        public static BinarySerializer Storage => s_storage.Value;

        #endregion

        #region 静态序列化方法

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T value)
        {
            return Default.SerializeValue(value);
        }

        /// <summary>
        /// 序列化对象为字节数组 (使用指定选项)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>序列化后的字节数组</returns>
        public static byte[] Serialize<T>(T value, BinarySerializerOptions options)
        {
            using var serializer = new BinarySerializer(options);
            return serializer.SerializeValue(value);
        }

        /// <summary>
        /// 序列化对象到缓冲区
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <returns>写入的字节数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<T>(T value, Span<byte> buffer)
        {
            return Default.SerializeValue(value, buffer);
        }

        #endregion

        #region 静态反序列化方法

        /// <summary>
        /// 反序列化字节数组为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> data)
        {
            return Default.DeserializeValue<T>(data);
        }

        /// <summary>
        /// 反序列化字节数组为对象 (使用指定选项)
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="data">序列化数据</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize<T>(ReadOnlySpan<byte> data, BinarySerializerOptions options)
        {
            using var serializer = new BinarySerializer(options);
            return serializer.DeserializeValue<T>(data);
        }

        /// <summary>
        /// 反序列化字节数组为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(data.AsSpan());
        }

        #endregion

        #region 实例序列化方法

        /// <summary>
        /// 序列化对象为字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        public byte[] SerializeValue<T>(T value)
        {
            using var writer = new BinaryWriterCore(_options.InitialBufferSize);

            // 写入头部 (如果启用)
            if (_options.IncludeHeader)
            {
                // 预留头部空间
                writer.Advance(BinaryHeader.Size);
            }

            int dataStart = writer.Position;

            // 序列化数据
            SerializeValueCore(writer, value);

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

                // 回写头部
                var headerSpan = writer.WrittenSpan.Slice(0, BinaryHeader.Size).ToArray();
                header.WriteTo(headerSpan);
                // 需要重新构建数据
                var result = new byte[writer.Position];
                header.WriteTo(result);
                writer.WrittenSpan.Slice(BinaryHeader.Size).CopyTo(result.AsSpan(BinaryHeader.Size));
                return result;
            }

            return writer.ToArray();
        }

        /// <summary>
        /// 序列化对象到缓冲区
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <returns>写入的字节数</returns>
        public int SerializeValue<T>(T value, Span<byte> buffer)
        {
            using var writer = new BinaryWriterCore(buffer.ToArray());

            int headerSize = _options.IncludeHeader ? BinaryHeader.Size : 0;
            if (headerSize > 0)
                writer.Advance(headerSize);

            int dataStart = writer.Position;
            SerializeValueCore(writer, value);
            int dataSize = writer.Position - dataStart;

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

        #endregion

        #region 实例反序列化方法

        /// <summary>
        /// 反序列化字节数组为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="data">序列化数据</param>
        /// <returns>反序列化的对象</returns>
        public T DeserializeValue<T>(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
                return default;

            var reader = new BinaryReaderCore(data);

            // 检查并读取头部
            if (_options.IncludeHeader && BinaryHeader.HasValidHeader(data))
            {
                var header = BinaryHeader.ReadFrom(data);
                if (!header.Validate(out var errorCode))
                {
                    throw new SerializationException($"数据头验证失败: {errorCode}", errorCode);
                }

                // 验证校验和
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

            return DeserializeValueCore<T>(ref reader);
        }

        #endregion

        #region 核心序列化逻辑

        /// <summary>
        /// 核心序列化逻辑
        /// </summary>
        private void SerializeValueCore<T>(BinaryWriterCore writer, T value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = typeof(T);

            // 处理基础类型
            if (TrySerializePrimitive(writer, value, type))
                return;

            // 处理字符串
            if (value is string str)
            {
                writer.WriteTypeCode(BinaryFormat.LongString);
                writer.WriteString(str);
                return;
            }

            // 处理字节数组
            if (value is byte[] bytes)
            {
                writer.WriteTypeCode(BinaryFormat.LongBytes);
                writer.WriteBytes(bytes);
                return;
            }

            // 处理数组
            if (type.IsArray)
            {
                SerializeArray(writer, value as Array);
                return;
            }

            // 使用格式化器
            if (_resolver != null)
            {
                var formatter = _resolver.GetFormatter<T>();
                if (formatter != null)
                {
                    writer.WriteTypeCode(BinaryFormat.ObjectStart);
                    // 这里需要实现 ISerializeWriter 适配器
                    // formatter.Serialize(writerAdapter, value, options);
                    writer.WriteTypeCode(BinaryFormat.ObjectEnd);
                    return;
                }
            }

            // 默认对象序列化
            SerializeObject(writer, value, type);
        }

        /// <summary>
        /// 尝试序列化基础类型
        /// </summary>
        private bool TrySerializePrimitive<T>(BinaryWriterCore writer, T value, Type type)
        {
            switch (value)
            {
                case bool b:
                    writer.WriteTypeCode(b ? BinaryFormat.True : BinaryFormat.False);
                    return true;

                case byte b:
                    writer.WriteTypeCode(BinaryFormat.UInt8);
                    writer.WriteByte(b);
                    return true;

                case sbyte sb:
                    writer.WriteTypeCode(BinaryFormat.Int8);
                    writer.WriteSByte(sb);
                    return true;

                case short s:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarInt32);
                        writer.WriteVarInt32(s);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.Int16);
                        writer.WriteInt16(s);
                    }
                    return true;

                case ushort us:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarUInt32);
                        writer.WriteVarUInt32(us);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.UInt16);
                        writer.WriteUInt16(us);
                    }
                    return true;

                case int i:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarInt32);
                        writer.WriteVarInt32(i);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.Int32);
                        writer.WriteInt32(i);
                    }
                    return true;

                case uint ui:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarUInt32);
                        writer.WriteVarUInt32(ui);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.UInt32);
                        writer.WriteUInt32(ui);
                    }
                    return true;

                case long l:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarInt64);
                        writer.WriteVarInt64(l);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.Int64);
                        writer.WriteInt64(l);
                    }
                    return true;

                case ulong ul:
                    if (_options.UseVarInt)
                    {
                        writer.WriteTypeCode(BinaryFormat.VarUInt64);
                        writer.WriteVarUInt64(ul);
                    }
                    else
                    {
                        writer.WriteTypeCode(BinaryFormat.UInt64);
                        writer.WriteUInt64(ul);
                    }
                    return true;

                case float f:
                    writer.WriteTypeCode(BinaryFormat.Float32);
                    writer.WriteSingle(f);
                    return true;

                case double d:
                    writer.WriteTypeCode(BinaryFormat.Float64);
                    writer.WriteDouble(d);
                    return true;

                case decimal dec:
                    writer.WriteTypeCode(BinaryFormat.Decimal);
                    writer.WriteDecimal(dec);
                    return true;

                case char c:
                    writer.WriteTypeCode(BinaryFormat.Char);
                    writer.WriteChar(c);
                    return true;

                case DateTime dt:
                    writer.WriteTypeCode(BinaryFormat.DateTime);
                    writer.WriteDateTime(dt);
                    return true;

                case TimeSpan ts:
                    writer.WriteTypeCode(BinaryFormat.TimeSpan);
                    writer.WriteTimeSpan(ts);
                    return true;

                case Guid g:
                    writer.WriteTypeCode(BinaryFormat.Guid);
                    writer.WriteGuid(g);
                    return true;

                case DateTimeOffset dto:
                    writer.WriteTypeCode(BinaryFormat.DateTimeOffset);
                    writer.WriteDateTimeOffset(dto);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 序列化数组
        /// </summary>
        private void SerializeArray(BinaryWriterCore writer, Array array)
        {
            if (array == null)
            {
                writer.WriteTypeCode(BinaryFormat.Null);
                return;
            }

            if (array.Length == 0)
            {
                writer.WriteTypeCode(BinaryFormat.EmptyArray);
                return;
            }

            writer.WriteTypeCode(BinaryFormat.Array);
            writer.WriteArrayHeader(array.Length);

            var elementType = array.GetType().GetElementType();
            foreach (var item in array)
            {
                SerializeValueCore(writer, item);
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        private void SerializeObject<T>(BinaryWriterCore writer, T value, Type type)
        {
            writer.WriteTypeCode(BinaryFormat.ObjectStart);

            // 获取所有可序列化的字段
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            writer.WriteVarInt32(fields.Length);

            foreach (var field in fields)
            {
                // 写入字段名 (如果启用)
                if (_options.IncludeTypeInfo)
                {
                    writer.WriteString(field.Name);
                }

                // 写入字段值
                var fieldValue = field.GetValue(value);
                SerializeValueCore(writer, fieldValue);
            }

            writer.WriteTypeCode(BinaryFormat.ObjectEnd);
        }

        #endregion

        #region 核心反序列化逻辑

        /// <summary>
        /// 核心反序列化逻辑
        /// </summary>
        private T DeserializeValueCore<T>(ref BinaryReaderCore reader)
        {
            if (reader.IsEnd)
                return default;

            byte typeCode = reader.ReadTypeCode();

            // 处理 null
            if (typeCode == BinaryFormat.Null)
                return default;

            // 处理基础类型
            object result = DeserializePrimitive(ref reader, typeCode);
            if (result != null)
                return (T)result;

            // 处理字符串
            if (typeCode == BinaryFormat.LongString || typeCode == BinaryFormat.ShortString ||
                typeCode == BinaryFormat.MediumString || typeCode == BinaryFormat.EmptyString)
            {
                if (typeCode == BinaryFormat.EmptyString)
                    return (T)(object)string.Empty;
                return (T)(object)reader.ReadString();
            }

            // 处理字节数组
            if (typeCode == BinaryFormat.LongBytes || typeCode == BinaryFormat.ShortBytes ||
                typeCode == BinaryFormat.EmptyBytes)
            {
                if (typeCode == BinaryFormat.EmptyBytes)
                    return (T)(object)Array.Empty<byte>();
                return (T)(object)reader.ReadBytes();
            }

            // 处理数组
            if (typeCode == BinaryFormat.Array || typeCode == BinaryFormat.EmptyArray)
            {
                return DeserializeArray<T>(ref reader, typeCode);
            }

            // 处理对象
            if (typeCode == BinaryFormat.ObjectStart)
            {
                return DeserializeObject<T>(ref reader);
            }

            throw new SerializationException($"不支持的类型码: 0x{typeCode:X2}", SerializeErrorCode.UnsupportedType);
        }

        /// <summary>
        /// 反序列化基础类型
        /// </summary>
        private object DeserializePrimitive(ref BinaryReaderCore reader, byte typeCode)
        {
            return typeCode switch
            {
                BinaryFormat.False => false,
                BinaryFormat.True => true,
                BinaryFormat.Int8 => reader.ReadSByte(),
                BinaryFormat.UInt8 => reader.ReadByte(),
                BinaryFormat.Int16 => reader.ReadInt16(),
                BinaryFormat.UInt16 => reader.ReadUInt16(),
                BinaryFormat.Int32 => reader.ReadInt32(),
                BinaryFormat.UInt32 => reader.ReadUInt32(),
                BinaryFormat.Int64 => reader.ReadInt64(),
                BinaryFormat.UInt64 => reader.ReadUInt64(),
                BinaryFormat.VarInt32 => reader.ReadVarInt32(),
                BinaryFormat.VarUInt32 => reader.ReadVarUInt32(),
                BinaryFormat.VarInt64 => reader.ReadVarInt64(),
                BinaryFormat.VarUInt64 => reader.ReadVarUInt64(),
                BinaryFormat.Float32 => reader.ReadSingle(),
                BinaryFormat.Float64 => reader.ReadDouble(),
                BinaryFormat.Decimal => reader.ReadDecimal(),
                BinaryFormat.Char => reader.ReadChar(),
                BinaryFormat.DateTime => reader.ReadDateTime(),
                BinaryFormat.TimeSpan => reader.ReadTimeSpan(),
                BinaryFormat.Guid => reader.ReadGuid(),
                BinaryFormat.DateTimeOffset => reader.ReadDateTimeOffset(),
                _ => null
            };
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        private T DeserializeArray<T>(ref BinaryReaderCore reader, byte typeCode)
        {
            if (typeCode == BinaryFormat.EmptyArray)
            {
                var elementType = typeof(T).GetElementType() ?? typeof(object);
                return (T)(object)Array.CreateInstance(elementType, 0);
            }

            int length = reader.ReadArrayHeader();
            var targetType = typeof(T);
            var elemType = targetType.IsArray ? targetType.GetElementType() : typeof(object);
            var array = Array.CreateInstance(elemType, length);

            for (int i = 0; i < length; i++)
            {
                var item = DeserializeValueCore<object>(ref reader);
                array.SetValue(item, i);
            }

            return (T)(object)array;
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        private T DeserializeObject<T>(ref BinaryReaderCore reader)
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance(type);

            int fieldCount = reader.ReadVarInt32();

            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = null;
                if (_options.IncludeTypeInfo)
                {
                    fieldName = reader.ReadString();
                }

                var fieldValue = DeserializeValueCore<object>(ref reader);

                if (fieldName != null && i < fields.Length)
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

            return (T)instance;
        }

        #endregion

        #region SerializerBase 重写

        /// <inheritdoc/>
        protected override int SerializeCore(object value, Span<byte> buffer, SerializeOptions options)
        {
            using var writer = new BinaryWriterCore(buffer.ToArray());
            SerializeValueCore(writer, value);
            writer.WrittenSpan.CopyTo(buffer);
            return writer.Position;
        }

        /// <inheritdoc/>
        protected override object DeserializeCore(ReadOnlySpan<byte> data, Type type, DeserializeOptions options)
        {
            var reader = new BinaryReaderCore(data);
            return DeserializeValueCore<object>(ref reader);
        }

        #endregion
    }
}
