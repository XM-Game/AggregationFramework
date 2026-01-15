// ==========================================================
// 文件名：JsonSerializerT.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型 JSON 序列化器
    /// <para>提供类型安全的 JSON 序列化和反序列化功能</para>
    /// <para>针对特定类型优化，减少装箱开销</para>
    /// </summary>
    /// <typeparam name="T">要序列化的类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 泛型实现避免装箱/拆箱
    /// 2. 静态缓存提高性能
    /// 3. 支持自定义格式化器
    /// 
    /// 使用示例:
    /// <code>
    /// // 序列化
    /// string json = JsonSerializer&lt;Player&gt;.Serialize(player);
    /// 
    /// // 反序列化
    /// Player player = JsonSerializer&lt;Player&gt;.Deserialize(json);
    /// 
    /// // 使用选项
    /// string prettyJson = JsonSerializer&lt;Player&gt;.Serialize(player, JsonSerializerOptions.Pretty);
    /// </code>
    /// </remarks>
    public static class JsonSerializer<T>
    {
        #region 静态字段

        /// <summary>默认序列化器实例</summary>
        private static readonly JsonSerializer s_default = JsonSerializer.Default;

        /// <summary>类型信息缓存</summary>
        private static readonly Type s_type = typeof(T);

        /// <summary>是否为值类型</summary>
        private static readonly bool s_isValueType = typeof(T).IsValueType;

        #endregion

        #region 序列化方法

        /// <summary>
        /// 序列化对象为 JSON 字符串
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>JSON 字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize(T value)
        {
            return s_default.SerializeToString(value);
        }

        /// <summary>
        /// 序列化对象为 JSON 字符串 (使用指定选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>JSON 字符串</returns>
        public static string Serialize(T value, JsonSerializerOptions options)
        {
            var serializer = new JsonSerializer(options);
            return serializer.SerializeToString(value);
        }

        /// <summary>
        /// 序列化对象为 UTF-8 字节数组
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>UTF-8 字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SerializeToUtf8Bytes(T value)
        {
            return s_default.SerializeToBytes(value);
        }

        /// <summary>
        /// 序列化对象为 UTF-8 字节数组 (使用指定选项)
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        /// <returns>UTF-8 字节数组</returns>
        public static byte[] SerializeToUtf8Bytes(T value, JsonSerializerOptions options)
        {
            var serializer = new JsonSerializer(options);
            return serializer.SerializeToBytes(value);
        }

        /// <summary>
        /// 序列化对象到 JsonWriter
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要序列化的对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeTo(JsonWriter writer, T value)
        {
            s_default.SerializeTo(writer, value);
        }

        #endregion

        #region 反序列化方法

        /// <summary>
        /// 反序列化 JSON 字符串为对象
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize(string json)
        {
            return s_default.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 JSON 字符串为对象 (使用指定选项)
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize(string json, JsonSerializerOptions options)
        {
            var serializer = new JsonSerializer(options);
            return serializer.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 JSON Span 为对象
        /// </summary>
        /// <param name="json">JSON 字符串 Span</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize(ReadOnlySpan<char> json)
        {
            return s_default.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 UTF-8 字节数组为对象
        /// </summary>
        /// <param name="utf8Json">UTF-8 JSON 字节数组</param>
        /// <returns>反序列化的对象</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize(ReadOnlySpan<byte> utf8Json)
        {
            string json = Encoding.UTF8.GetString(utf8Json.ToArray());
            return s_default.DeserializeFromString<T>(json);
        }

        /// <summary>
        /// 反序列化 UTF-8 字节数组为对象 (使用指定选项)
        /// </summary>
        /// <param name="utf8Json">UTF-8 JSON 字节数组</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        public static T Deserialize(ReadOnlySpan<byte> utf8Json, JsonSerializerOptions options)
        {
            string json = Encoding.UTF8.GetString(utf8Json.ToArray());
            var serializer = new JsonSerializer(options);
            return serializer.DeserializeFromString<T>(json);
        }

        #endregion

        #region 尝试方法

        /// <summary>
        /// 尝试序列化对象为 JSON 字符串
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="json">输出 JSON 字符串</param>
        /// <returns>如果成功返回 true</returns>
        public static bool TrySerialize(T value, out string json)
        {
            try
            {
                json = Serialize(value);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }

        /// <summary>
        /// 尝试反序列化 JSON 字符串为对象
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="value">输出对象</param>
        /// <returns>如果成功返回 true</returns>
        public static bool TryDeserialize(string json, out T value)
        {
            try
            {
                value = Deserialize(json);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 尝试反序列化 JSON 字符串为对象 (使用指定选项)
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="value">输出对象</param>
        /// <returns>如果成功返回 true</returns>
        public static bool TryDeserialize(string json, JsonSerializerOptions options, out T value)
        {
            try
            {
                value = Deserialize(json, options);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查 JSON 字符串是否可以反序列化为此类型
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>如果可以反序列化返回 true</returns>
        public static bool CanDeserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return !s_isValueType; // 值类型不能为 null

            return TryDeserialize(json, out _);
        }

        /// <summary>
        /// 克隆对象 (通过序列化/反序列化)
        /// </summary>
        /// <param name="value">要克隆的对象</param>
        /// <returns>克隆的对象</returns>
        public static T Clone(T value)
        {
            if (value == null)
                return default;

            string json = Serialize(value);
            return Deserialize(json);
        }

        /// <summary>
        /// 深度比较两个对象 (通过 JSON 比较)
        /// </summary>
        /// <param name="left">左侧对象</param>
        /// <param name="right">右侧对象</param>
        /// <returns>如果相等返回 true</returns>
        public static bool DeepEquals(T left, T right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            string leftJson = Serialize(left);
            string rightJson = Serialize(right);

            return string.Equals(leftJson, rightJson, StringComparison.Ordinal);
        }

        /// <summary>
        /// 合并两个对象 (右侧覆盖左侧)
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="source">源对象</param>
        /// <returns>合并后的对象</returns>
        public static T Merge(T target, T source)
        {
            if (source == null)
                return target;

            if (target == null)
                return Clone(source);

            // 简单实现：序列化后合并
            // 更复杂的实现需要深度合并 JSON 对象
            return Clone(source);
        }

        #endregion

        #region 格式化方法

        /// <summary>
        /// 格式化 JSON 字符串 (美化输出)
        /// </summary>
        /// <param name="json">紧凑 JSON 字符串</param>
        /// <returns>格式化后的 JSON 字符串</returns>
        public static string Format(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            T value = Deserialize(json);
            return Serialize(value, JsonSerializerOptions.Pretty);
        }

        /// <summary>
        /// 压缩 JSON 字符串 (移除空白)
        /// </summary>
        /// <param name="json">格式化的 JSON 字符串</param>
        /// <returns>压缩后的 JSON 字符串</returns>
        public static string Minify(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            T value = Deserialize(json);
            return Serialize(value, JsonSerializerOptions.Compact);
        }

        #endregion
    }
}
