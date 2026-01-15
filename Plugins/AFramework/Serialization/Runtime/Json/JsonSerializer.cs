// ==========================================================
// 文件名：JsonSerializer.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Text, System.Reflection, System.Collections
// ==========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.Serialization
{
    /// <summary>
    /// JSON 序列化器
    /// <para>提供高性能的 JSON 序列化和反序列化功能</para>
    /// <para>支持基础类型、集合、自定义对象的序列化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 继承 SerializerBase 复用基础功能
    /// 2. 支持格式化输出和紧凑输出
    /// 3. 支持属性命名策略转换
    /// 4. 提供静态方法便于快速使用
    /// 
    /// 使用示例:
    /// <code>
    /// // 静态方法 (推荐)
    /// string json = JsonSerializer.Serialize(player);
    /// Player player = JsonSerializer.Deserialize&lt;Player&gt;(json);
    /// 
    /// // 实例方法
    /// var serializer = new JsonSerializer(JsonSerializerOptions.Pretty);
    /// string json = serializer.SerializeToString(player);
    /// </code>
    /// </remarks>
    public class JsonSerializer : SerializerBase
    {
        #region 静态字段

        /// <summary>默认序列化器实例</summary>
        private static readonly Lazy<JsonSerializer> s_default =
            new Lazy<JsonSerializer>(() => new JsonSerializer());

        /// <summary>美化输出序列化器实例</summary>
        private static readonly Lazy<JsonSerializer> s_pretty =
            new Lazy<JsonSerializer>(() => new JsonSerializer(JsonSerializerOptions.Pretty));

        /// <summary>紧凑输出序列化器实例</summary>
        private static readonly Lazy<JsonSerializer> s_compact =
            new Lazy<JsonSerializer>(() => new JsonSerializer(JsonSerializerOptions.Compact));

        #endregion

        #region 字段

        /// <summary>JSON 序列化选项</summary>
        private readonly JsonSerializerOptions _options;

        /// <summary>类型成员缓存</summary>
        private static readonly Dictionary<Type, MemberCache> s_memberCache = new Dictionary<Type, MemberCache>();

        /// <summary>缓存锁</summary>
        private static readonly object s_cacheLock = new object();

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建 JSON 序列化器 (使用默认选项)
        /// </summary>
        public JsonSerializer() : this(JsonSerializerOptions.Default)
        {
        }

        /// <summary>
        /// 创建 JSON 序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        public JsonSerializer(JsonSerializerOptions options) : base(null)
        {
            _options = options;
        }

        /// <summary>
        /// 创建 JSON 序列化器
        /// </summary>
        /// <param name="options">序列化选项</param>
        /// <param name="resolver">格式化器解析器</param>
        public JsonSerializer(JsonSerializerOptions options, IFormatterResolver resolver) : base(resolver)
        {
            _options = options;
        }

        #endregion

        #region 属性

        /// <summary>JSON 序列化选项</summary>
        public JsonSerializerOptions Options => _options;

        /// <summary>默认序列化器实例</summary>
        public static JsonSerializer Default => s_default.Value;

        /// <summary>美化输出序列化器实例</summary>
        public static JsonSerializer Pretty => s_pretty.Value;

        /// <summary>紧凑输出序列化器实例</summary>
        public static JsonSerializer Compact => s_compact.Value;

        #endregion

        #region 静态序列化方法

        /// <summary>
        /// 序列化对象为 JSON 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>JSON 字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize<T>(T value)
        {
            return Default.SerializeToString(value);
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串 (使用指定选项)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>JSON 字符串</returns>
        public static string Serialize<T>(T value, JsonSerializerOptions options)
        {
            var serializer = new JsonSerializer(options);
            return serializer.SerializeToString(value);
        }

        /// <summary>
        /// 序列化对象为 UTF-8 字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>UTF-8 字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToUtf8Bytes<T>(T value)
        {
            return Default.SerializeToBytes(value);
        }

        #endregion

        #region 静态反序列化方法

        /// <summary>
        /// 反序列化 JSON 字符串为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(string json)
        {
            return Default.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 JSON 字符串为对象 (使用指定选项)
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize<T>(string json, JsonSerializerOptions options)
        {
            var serializer = new JsonSerializer(options);
            return serializer.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 UTF-8 字节数组为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="utf8Json">UTF-8 JSON 字节数组</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> utf8Json)
        {
            string json = Encoding.UTF8.GetString(utf8Json.ToArray());
            return Default.DeserializeFromString<T>(json);
        }

        #endregion

        #region 实例序列化方法

        /// <summary>
        /// 序列化对象为 JSON 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>JSON 字符串</returns>
        public string SerializeToString<T>(T value)
        {
            using var writer = new JsonWriter(_options);
            SerializeValue(writer, value, typeof(T), 0);
            return writer.ToString();
        }

        /// <summary>
        /// 序列化对象为 UTF-8 字节数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">要序列化的对象</param>
        /// <returns>UTF-8 字节数组</returns>
        public byte[] SerializeToBytes<T>(T value)
        {
            using var writer = new JsonWriter(_options);
            SerializeValue(writer, value, typeof(T), 0);
            return writer.ToUtf8Bytes();
        }

        /// <summary>
        /// 序列化对象到 JsonWriter
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要序列化的对象</param>
        public void SerializeTo<T>(JsonWriter writer, T value)
        {
            SerializeValue(writer, value, typeof(T), 0);
        }

        #endregion

        #region 实例反序列化方法

        /// <summary>
        /// 反序列化 JSON 字符串为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <returns>反序列化的对象</returns>
        public T DeserializeFromString<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var reader = new JsonReader(json, _options);
            return DeserializeValue<T>(ref reader);
        }

        /// <summary>
        /// 反序列化 JSON 字符串为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON 字符串 Span</param>
        /// <returns>反序列化的对象</returns>
        public T DeserializeFromString<T>(ReadOnlySpan<char> json)
        {
            if (json.IsEmpty)
                return default;

            var reader = new JsonReader(json, _options);
            return DeserializeValue<T>(ref reader);
        }

        #endregion

        #region 核心序列化逻辑

        /// <summary>
        /// 序列化值
        /// </summary>
        private void SerializeValue(JsonWriter writer, object value, Type type, int depth)
        {
            // 深度检查
            if (depth > _options.MaxDepth)
                throw new InvalidOperationException($"超出最大嵌套深度 ({_options.MaxDepth})");

            // null 处理
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // 获取实际类型
            var actualType = value.GetType();

            // 基础类型
            if (TrySerializePrimitive(writer, value, actualType))
                return;

            // 字符串
            if (value is string str)
            {
                writer.WriteString(str);
                return;
            }

            // 字典
            if (value is IDictionary dict)
            {
                SerializeDictionary(writer, dict, depth);
                return;
            }

            // 集合/数组
            if (value is IEnumerable enumerable && !(value is string))
            {
                SerializeEnumerable(writer, enumerable, depth);
                return;
            }

            // 对象
            SerializeObject(writer, value, actualType, depth);
        }

        /// <summary>
        /// 尝试序列化基础类型
        /// </summary>
        private bool TrySerializePrimitive(JsonWriter writer, object value, Type type)
        {
            switch (value)
            {
                case bool b:
                    writer.WriteBoolean(b);
                    return true;

                case byte b:
                    writer.WriteNumber(b);
                    return true;

                case sbyte sb:
                    writer.WriteNumber(sb);
                    return true;

                case short s:
                    writer.WriteNumber(s);
                    return true;

                case ushort us:
                    writer.WriteNumber(us);
                    return true;

                case int i:
                    writer.WriteNumber(i);
                    return true;

                case uint ui:
                    writer.WriteNumber(ui);
                    return true;

                case long l:
                    writer.WriteNumber(l);
                    return true;

                case ulong ul:
                    writer.WriteNumber(ul);
                    return true;

                case float f:
                    writer.WriteNumber(f);
                    return true;

                case double d:
                    writer.WriteNumber(d);
                    return true;

                case decimal dec:
                    writer.WriteNumber(dec);
                    return true;

                case char c:
                    writer.WriteString(c.ToString());
                    return true;

                case DateTime dt:
                    writer.WriteDateTime(dt);
                    return true;

                case DateTimeOffset dto:
                    writer.WriteDateTimeOffset(dto);
                    return true;

                case TimeSpan ts:
                    writer.WriteTimeSpan(ts);
                    return true;

                case Guid g:
                    writer.WriteGuid(g);
                    return true;

                case Enum e:
                    writer.WriteString(e.ToString());
                    return true;

                case byte[] bytes:
                    writer.WriteBase64(bytes);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 序列化字典
        /// </summary>
        private void SerializeDictionary(JsonWriter writer, IDictionary dict, int depth)
        {
            writer.WriteStartObject();

            foreach (DictionaryEntry entry in dict)
            {
                string key = entry.Key?.ToString();
                if (key == null)
                    continue;

                object value = entry.Value;

                // 忽略 null 值
                if (_options.IgnoreNullValues && value == null)
                    continue;

                writer.WritePropertyName(key);
                SerializeValue(writer, value, value?.GetType() ?? typeof(object), depth + 1);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// 序列化可枚举集合
        /// </summary>
        private void SerializeEnumerable(JsonWriter writer, IEnumerable enumerable, int depth)
        {
            writer.WriteStartArray();

            foreach (var item in enumerable)
            {
                SerializeValue(writer, item, item?.GetType() ?? typeof(object), depth + 1);
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        private void SerializeObject(JsonWriter writer, object value, Type type, int depth)
        {
            var cache = GetMemberCache(type);

            writer.WriteStartObject();

            foreach (var member in cache.Members)
            {
                object memberValue = member.GetValue(value);

                // 忽略 null 值
                if (_options.IgnoreNullValues && memberValue == null)
                    continue;

                // 忽略默认值
                if (_options.IgnoreDefaultValues && IsDefaultValue(memberValue, member.MemberType))
                    continue;

                writer.WritePropertyName(member.Name);
                SerializeValue(writer, memberValue, member.MemberType, depth + 1);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// 检查是否为默认值
        /// </summary>
        private static bool IsDefaultValue(object value, Type type)
        {
            if (value == null)
                return true;

            if (type.IsValueType)
            {
                object defaultValue = Activator.CreateInstance(type);
                return value.Equals(defaultValue);
            }

            return false;
        }

        #endregion

        #region 核心反序列化逻辑

        /// <summary>
        /// 反序列化值
        /// </summary>
        private T DeserializeValue<T>(ref JsonReader reader)
        {
            return (T)DeserializeValue(ref reader, typeof(T));
        }

        /// <summary>
        /// 反序列化值
        /// </summary>
        private object DeserializeValue(ref JsonReader reader, Type type)
        {
            if (!reader.Read())
                return GetDefaultValue(type);

            return DeserializeCurrentValue(ref reader, type);
        }

        /// <summary>
        /// 反序列化当前值
        /// </summary>
        private object DeserializeCurrentValue(ref JsonReader reader, Type type)
        {
            // null 处理
            if (reader.TokenType == Internal.JsonTokenType.Null)
                return null;

            // 可空类型
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                type = underlyingType;

            // 基础类型
            if (TryDeserializePrimitive(ref reader, type, out object primitiveValue))
                return primitiveValue;

            // 字符串
            if (type == typeof(string))
            {
                if (reader.TokenType != Internal.JsonTokenType.String)
                    throw new Internal.JsonParseException($"期望字符串，实际为 {reader.TokenType}", reader.Position);
                return reader.ReadString();
            }

            // 数组
            if (type.IsArray)
            {
                return DeserializeArray(ref reader, type);
            }

            // 泛型集合
            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();

                if (genericDef == typeof(List<>) || genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>))
                {
                    return DeserializeList(ref reader, type);
                }

                if (genericDef == typeof(Dictionary<,>) || genericDef == typeof(IDictionary<,>))
                {
                    return DeserializeDictionary(ref reader, type);
                }
            }

            // 对象
            if (reader.TokenType == Internal.JsonTokenType.StartObject)
            {
                return DeserializeObject(ref reader, type);
            }

            throw new Internal.JsonParseException($"无法反序列化类型 {type.Name}", reader.Position);
        }

        /// <summary>
        /// 尝试反序列化基础类型
        /// </summary>
        private bool TryDeserializePrimitive(ref JsonReader reader, Type type, out object value)
        {
            value = null;

            // 布尔值
            if (type == typeof(bool))
            {
                if (reader.TokenType == Internal.JsonTokenType.True)
                {
                    value = true;
                    return true;
                }
                if (reader.TokenType == Internal.JsonTokenType.False)
                {
                    value = false;
                    return true;
                }
            }

            // 数字类型
            if (reader.TokenType == Internal.JsonTokenType.Number)
            {
                var span = reader.TokenType == Internal.JsonTokenType.Number ? 
                    default : default; // 需要从 tokenizer 获取

                if (type == typeof(int))
                {
                    value = reader.ReadInt32();
                    return true;
                }
                if (type == typeof(long))
                {
                    value = reader.ReadInt64();
                    return true;
                }
                if (type == typeof(float))
                {
                    value = reader.ReadSingle();
                    return true;
                }
                if (type == typeof(double))
                {
                    value = reader.ReadDouble();
                    return true;
                }
                if (type == typeof(decimal))
                {
                    value = reader.ReadDecimal();
                    return true;
                }
                if (type == typeof(byte))
                {
                    value = (byte)reader.ReadInt32();
                    return true;
                }
                if (type == typeof(sbyte))
                {
                    value = (sbyte)reader.ReadInt32();
                    return true;
                }
                if (type == typeof(short))
                {
                    value = (short)reader.ReadInt32();
                    return true;
                }
                if (type == typeof(ushort))
                {
                    value = (ushort)reader.ReadInt32();
                    return true;
                }
                if (type == typeof(uint))
                {
                    value = (uint)reader.ReadInt64();
                    return true;
                }
                if (type == typeof(ulong))
                {
                    value = (ulong)reader.ReadInt64();
                    return true;
                }
            }

            // 特殊类型 (从字符串解析)
            if (reader.TokenType == Internal.JsonTokenType.String)
            {
                if (type == typeof(DateTime))
                {
                    value = reader.ReadDateTime();
                    return true;
                }
                if (type == typeof(DateTimeOffset))
                {
                    value = reader.ReadDateTimeOffset();
                    return true;
                }
                if (type == typeof(TimeSpan))
                {
                    value = reader.ReadTimeSpan();
                    return true;
                }
                if (type == typeof(Guid))
                {
                    value = reader.ReadGuid();
                    return true;
                }
                if (type == typeof(char))
                {
                    string str = reader.ReadString();
                    value = str?.Length > 0 ? str[0] : '\0';
                    return true;
                }
                if (type.IsEnum)
                {
                    string str = reader.ReadString();
                    value = Enum.Parse(type, str, true);
                    return true;
                }
                if (type == typeof(byte[]))
                {
                    value = reader.ReadBase64();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        private object DeserializeArray(ref JsonReader reader, Type arrayType)
        {
            if (reader.TokenType != Internal.JsonTokenType.StartArray)
                throw new Internal.JsonParseException("期望数组开始 '['", reader.Position);

            var elementType = arrayType.GetElementType();
            var list = new List<object>();

            while (reader.HasMoreArrayElements())
            {
                var item = DeserializeCurrentValue(ref reader, elementType);
                list.Add(item);
            }

            var array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }

            return array;
        }

        /// <summary>
        /// 反序列化列表
        /// </summary>
        private object DeserializeList(ref JsonReader reader, Type listType)
        {
            if (reader.TokenType != Internal.JsonTokenType.StartArray)
                throw new Internal.JsonParseException("期望数组开始 '['", reader.Position);

            var elementType = listType.GetGenericArguments()[0];
            var concreteType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(concreteType);

            while (reader.HasMoreArrayElements())
            {
                var item = DeserializeCurrentValue(ref reader, elementType);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 反序列化字典
        /// </summary>
        private object DeserializeDictionary(ref JsonReader reader, Type dictType)
        {
            if (reader.TokenType != Internal.JsonTokenType.StartObject)
                throw new Internal.JsonParseException("期望对象开始 '{'", reader.Position);

            var keyType = dictType.GetGenericArguments()[0];
            var valueType = dictType.GetGenericArguments()[1];
            var concreteType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var dict = (IDictionary)Activator.CreateInstance(concreteType);

            while (reader.ReadPropertyName(out string key))
            {
                if (!reader.Read())
                    throw new Internal.JsonParseException("期望值", reader.Position);

                object dictKey = keyType == typeof(string) ? key : Convert.ChangeType(key, keyType);
                object dictValue = DeserializeCurrentValue(ref reader, valueType);
                dict[dictKey] = dictValue;
            }

            return dict;
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        private object DeserializeObject(ref JsonReader reader, Type type)
        {
            var instance = Activator.CreateInstance(type);
            var cache = GetMemberCache(type);

            while (reader.ReadPropertyName(out string propertyName))
            {
                if (!reader.Read())
                    throw new Internal.JsonParseException("期望值", reader.Position);

                // 查找成员
                if (cache.MemberMap.TryGetValue(propertyName, out var member))
                {
                    object memberValue = DeserializeCurrentValue(ref reader, member.MemberType);
                    member.SetValue(instance, memberValue);
                }
                else
                {
                    // 跳过未知属性
                    reader.Skip();
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion

        #region 成员缓存

        /// <summary>
        /// 获取类型的成员缓存
        /// </summary>
        private MemberCache GetMemberCache(Type type)
        {
            lock (s_cacheLock)
            {
                if (!s_memberCache.TryGetValue(type, out var cache))
                {
                    cache = new MemberCache(type, _options);
                    s_memberCache[type] = cache;
                }
                return cache;
            }
        }

        /// <summary>
        /// 成员缓存
        /// </summary>
        private class MemberCache
        {
            public List<MemberAccessor> Members { get; }
            public Dictionary<string, MemberAccessor> MemberMap { get; }

            public MemberCache(Type type, JsonSerializerOptions options)
            {
                Members = new List<MemberAccessor>();
                MemberMap = new Dictionary<string, MemberAccessor>(StringComparer.OrdinalIgnoreCase);

                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                // 属性
                foreach (var prop in type.GetProperties(bindingFlags))
                {
                    if (!prop.CanRead || !prop.CanWrite)
                        continue;

                    // 检查忽略特性
                    if (prop.GetCustomAttribute<SerializeIgnoreAttribute>() != null)
                        continue;

                    var accessor = new MemberAccessor(prop, options.NamingPolicy);
                    Members.Add(accessor);
                    MemberMap[accessor.Name] = accessor;
                }

                // 字段 (如果启用)
                if (options.IncludeFields)
                {
                    foreach (var field in type.GetFields(bindingFlags))
                    {
                        if (field.IsInitOnly)
                            continue;

                        if (field.GetCustomAttribute<SerializeIgnoreAttribute>() != null)
                            continue;

                        var accessor = new MemberAccessor(field, options.NamingPolicy);
                        Members.Add(accessor);
                        MemberMap[accessor.Name] = accessor;
                    }
                }
            }
        }

        /// <summary>
        /// 成员访问器
        /// </summary>
        private class MemberAccessor
        {
            public string Name { get; }
            public Type MemberType { get; }
            private readonly PropertyInfo _property;
            private readonly FieldInfo _field;

            public MemberAccessor(PropertyInfo property, JsonPropertyNamingPolicy policy)
            {
                _property = property;
                MemberType = property.PropertyType;

                // 检查自定义名称
                var nameAttr = property.GetCustomAttribute<SerializeNameAttribute>();
                Name = nameAttr?.Name ?? ConvertName(property.Name, policy);
            }

            public MemberAccessor(FieldInfo field, JsonPropertyNamingPolicy policy)
            {
                _field = field;
                MemberType = field.FieldType;

                var nameAttr = field.GetCustomAttribute<SerializeNameAttribute>();
                Name = nameAttr?.Name ?? ConvertName(field.Name, policy);
            }

            public object GetValue(object obj)
            {
                return _property != null ? _property.GetValue(obj) : _field.GetValue(obj);
            }

            public void SetValue(object obj, object value)
            {
                if (_property != null)
                    _property.SetValue(obj, value);
                else
                    _field.SetValue(obj, value);
            }

            private static string ConvertName(string name, JsonPropertyNamingPolicy policy)
            {
                if (policy == JsonPropertyNamingPolicy.CamelCase && !string.IsNullOrEmpty(name))
                {
                    return char.ToLowerInvariant(name[0]) + name.Substring(1);
                }
                return name;
            }
        }

        #endregion

        #region SerializerBase 重写

        /// <inheritdoc/>
        protected override int SerializeCore(object value, Span<byte> buffer, SerializeOptions options)
        {
            string json = SerializeToString(value);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            if (bytes.Length > buffer.Length)
                throw new ArgumentException("缓冲区太小");

            bytes.CopyTo(buffer);
            return bytes.Length;
        }

        /// <inheritdoc/>
        protected override object DeserializeCore(ReadOnlySpan<byte> data, Type type, DeserializeOptions options)
        {
            string json = Encoding.UTF8.GetString(data.ToArray());
            var reader = new JsonReader(json, _options);
            return DeserializeValue(ref reader, type);
        }

        #endregion
    }
}
