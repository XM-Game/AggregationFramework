// ==========================================================
// 文件名：IFormatterT.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型格式化器接口
    /// <para>提供类型安全的序列化和反序列化逻辑</para>
    /// <para>避免装箱拆箱，提供最佳性能</para>
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// public class PlayerFormatter : IFormatter&lt;Player&gt;
    /// {
    ///     public void Serialize(ISerializeWriter writer, Player value, SerializeOptions options)
    ///     {
    ///         writer.WriteString(value.Name);
    ///         writer.WriteInt32(value.Level);
    ///         writer.WriteInt64(value.Experience);
    ///     }
    ///     
    ///     public Player Deserialize(ISerializeReader reader, DeserializeOptions options)
    ///     {
    ///         return new Player
    ///         {
    ///             Name = reader.ReadString(),
    ///             Level = reader.ReadInt32(),
    ///             Experience = reader.ReadInt64()
    ///         };
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public interface IFormatter<T>
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        void Serialize(ISerializeWriter writer, T value, SerializeOptions options);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        T Deserialize(ISerializeReader reader, DeserializeOptions options);
    }

    /// <summary>
    /// 支持原地反序列化的泛型格式化器接口
    /// <para>允许反序列化到现有对象，减少内存分配</para>
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public interface IInPlaceFormatter<T> : IFormatter<T>
    {
        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        void DeserializeTo(ISerializeReader reader, ref T target, DeserializeOptions options);
    }

    /// <summary>
    /// 支持大小预估的泛型格式化器接口
    /// <para>用于预分配缓冲区，提高性能</para>
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public interface ISizedFormatter<T> : IFormatter<T>
    {
        /// <summary>
        /// 获取序列化后的预估大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>预估的字节数</returns>
        int GetSerializedSize(T value);

        /// <summary>
        /// 获取固定大小 (如果类型是固定大小)
        /// </summary>
        /// <returns>固定大小，-1 表示非固定大小</returns>
        int FixedSize { get; }

        /// <summary>
        /// 是否为固定大小类型
        /// </summary>
        bool IsFixedSize { get; }
    }

    /// <summary>
    /// 支持版本迁移的泛型格式化器接口
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public interface IVersionedFormatter<T> : IFormatter<T>
    {
        /// <summary>
        /// 获取当前版本
        /// </summary>
        int CurrentVersion { get; }

        /// <summary>
        /// 获取最小支持版本
        /// </summary>
        int MinSupportedVersion { get; }

        /// <summary>
        /// 序列化对象 (带版本)
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="version">版本号</param>
        /// <param name="options">序列化选项</param>
        void SerializeVersion(ISerializeWriter writer, T value, int version, SerializeOptions options);

        /// <summary>
        /// 反序列化指定版本的数据
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="version">数据版本</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        T DeserializeVersion(ISerializeReader reader, int version, DeserializeOptions options);
    }

    /// <summary>
    /// 格式化器包装器
    /// <para>将泛型格式化器包装为非泛型格式化器</para>
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public sealed class FormatterWrapper<T> : IFormatter
    {
        private readonly IFormatter<T> _formatter;

        /// <summary>
        /// 创建格式化器包装器
        /// </summary>
        /// <param name="formatter">泛型格式化器</param>
        public FormatterWrapper(IFormatter<T> formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        /// <summary>获取目标类型</summary>
        public Type TargetType => typeof(T);

        /// <summary>序列化对象</summary>
        public void Serialize(ISerializeWriter writer, object value, SerializeOptions options)
        {
            _formatter.Serialize(writer, (T)value, options);
        }

        /// <summary>反序列化对象</summary>
        public object Deserialize(ISerializeReader reader, DeserializeOptions options)
        {
            return _formatter.Deserialize(reader, options);
        }

        /// <summary>反序列化到现有对象</summary>
        public void DeserializeTo(ISerializeReader reader, object target, DeserializeOptions options)
        {
            if (_formatter is IInPlaceFormatter<T> inPlaceFormatter)
            {
                var typedTarget = (T)target;
                inPlaceFormatter.DeserializeTo(reader, ref typedTarget, options);
            }
            else
            {
                throw new NotSupportedException($"格式化器 {_formatter.GetType().Name} 不支持原地反序列化");
            }
        }

        /// <summary>获取序列化后的预估大小</summary>
        public int GetSerializedSize(object value)
        {
            if (_formatter is ISizedFormatter<T> sizedFormatter)
            {
                return sizedFormatter.GetSerializedSize((T)value);
            }
            return -1;
        }
    }
}
