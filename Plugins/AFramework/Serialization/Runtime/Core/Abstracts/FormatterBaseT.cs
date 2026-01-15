// ==========================================================
// 文件名：FormatterBaseT.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型格式化器抽象基类
    /// <para>提供 IFormatter&lt;T&gt; 接口的基础实现</para>
    /// <para>提供类型安全的序列化操作，避免装箱拆箱</para>
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 提供泛型格式化器的基础实现
    /// 2. 避免装箱拆箱，提供最佳性能
    /// 3. 支持类型特化的优化实现
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerFormatter : FormatterBase&lt;Player&gt;
    /// {
    ///     protected override void SerializeCore(ISerializeWriter writer, Player value, SerializeOptions options)
    ///     {
    ///         writer.WriteString(value.Name);
    ///         writer.WriteInt32(value.Level);
    ///     }
    ///     
    ///     protected override Player DeserializeCore(ISerializeReader reader, DeserializeOptions options)
    ///     {
    ///         return new Player
    ///         {
    ///             Name = reader.ReadString(),
    ///             Level = reader.ReadInt32()
    ///         };
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class FormatterBase<T> : IFormatter<T>, IFormatter
    {
        #region 静态字段

        /// <summary>目标类型</summary>
        private static readonly Type _targetType = typeof(T);

        /// <summary>是否为值类型</summary>
        private static readonly bool _isValueType = typeof(T).IsValueType;

        /// <summary>是否为非托管类型 (延迟初始化)</summary>
        private static readonly Lazy<bool> _isUnmanaged = new Lazy<bool>(() => FormatterHelper.IsUnmanagedType(_targetType));

        #endregion

        #region 属性

        /// <summary>获取目标类型</summary>
        public Type TargetType => _targetType;

        /// <summary>获取格式化器名称</summary>
        public virtual string Name => GetType().Name;

        /// <summary>获取格式化器描述</summary>
        public virtual string Description => $"{_targetType.Name} 格式化器";

        /// <summary>是否支持 null 值</summary>
        public virtual bool SupportsNull => !_isValueType;

        /// <summary>是否为固定大小类型</summary>
        public virtual bool IsFixedSize => _isUnmanaged.Value;

        /// <summary>固定大小 (如果 IsFixedSize 为 true)</summary>
        public virtual int FixedSize => _isUnmanaged.Value ? Unsafe.SizeOf<T>() : -1;

        /// <summary>是否为值类型</summary>
        protected static bool IsValueType => _isValueType;

        /// <summary>是否为非托管类型</summary>
        protected static bool IsUnmanaged => _isUnmanaged.Value;

        #endregion

        #region IFormatter<T> 实现

        /// <inheritdoc/>
        public void Serialize(ISerializeWriter writer, T value, SerializeOptions options)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
            {
                if (!SupportsNull)
                    throw new ArgumentNullException(nameof(value), $"类型 {_targetType.Name} 不支持 null 值");

                SerializeNull(writer, options);
                return;
            }

            SerializeCore(writer, value, options);
        }

        /// <inheritdoc/>
        public T Deserialize(ISerializeReader reader, DeserializeOptions options)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // 检查 null 标记
            if (TryDeserializeNull(reader, options, out var isNull) && isNull)
                return default;

            return DeserializeCore(reader, options);
        }

        #endregion

        #region IFormatter 实现 (非泛型桥接)

        /// <inheritdoc/>
        void IFormatter.Serialize(ISerializeWriter writer, object value, SerializeOptions options)
        {
            Serialize(writer, (T)value, options);
        }

        /// <inheritdoc/>
        object IFormatter.Deserialize(ISerializeReader reader, DeserializeOptions options)
        {
            return Deserialize(reader, options);
        }

        /// <inheritdoc/>
        void IFormatter.DeserializeTo(ISerializeReader reader, object target, DeserializeOptions options)
        {
            if (target is T typedTarget)
            {
                DeserializeTo(reader, ref typedTarget, options);
            }
            else
            {
                throw new ArgumentException(
                    $"目标类型不匹配: 期望 {_targetType.Name}，实际 {target?.GetType().Name ?? "null"}");
            }
        }

        /// <inheritdoc/>
        int IFormatter.GetSerializedSize(object value)
        {
            return GetSerializedSize((T)value);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        public virtual void DeserializeTo(ISerializeReader reader, ref T target, DeserializeOptions options)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // 检查 null 标记
            if (TryDeserializeNull(reader, options, out var isNull) && isNull)
            {
                target = default;
                return;
            }

            DeserializeToCore(reader, ref target, options);
        }

        /// <summary>
        /// 获取序列化后的预估大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>预估的字节数</returns>
        public virtual int GetSerializedSize(T value)
        {
            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
                return 1; // null 标记

            if (IsFixedSize)
                return FixedSize;

            return EstimateSize(value);
        }

        #endregion

        #region 抽象方法 - 子类必须实现

        /// <summary>
        /// 核心序列化逻辑
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象 (非 null)</param>
        /// <param name="options">序列化选项</param>
        protected abstract void SerializeCore(ISerializeWriter writer, T value, SerializeOptions options);

        /// <summary>
        /// 核心反序列化逻辑
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract T DeserializeCore(ISerializeReader reader, DeserializeOptions options);

        #endregion

        #region 虚方法 - 子类可选重写

        /// <summary>
        /// 反序列化到现有对象的核心逻辑
        /// </summary>
        protected virtual void DeserializeToCore(ISerializeReader reader, ref T target, DeserializeOptions options)
        {
            // 默认实现：直接反序列化并赋值
            target = DeserializeCore(reader, options);
        }

        /// <summary>
        /// 序列化 null 值
        /// </summary>
        protected virtual void SerializeNull(ISerializeWriter writer, SerializeOptions options)
        {
            writer.WriteNullObjectHeader();
        }

        /// <summary>
        /// 尝试反序列化 null 值
        /// </summary>
        protected virtual bool TryDeserializeNull(ISerializeReader reader, DeserializeOptions options, out bool isNull)
        {
            isNull = reader.TryReadNullObjectHeader();
            return true;
        }

        /// <summary>
        /// 估算序列化大小
        /// </summary>
        protected virtual int EstimateSize(T value)
        {
            return BufferConstants.Writer.InitialBufferSize;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建类型实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }

        #endregion

        #region 重写方法

        /// <summary>获取字符串表示</summary>
        public override string ToString()
        {
            return $"{Name} [{_targetType.Name}]";
        }

        #endregion
    }

    /// <summary>
    /// 支持原地反序列化的泛型格式化器基类
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public abstract class InPlaceFormatterBase<T> : FormatterBase<T>, IInPlaceFormatter<T>
    {
        /// <inheritdoc/>
        void IInPlaceFormatter<T>.DeserializeTo(ISerializeReader reader, ref T target, DeserializeOptions options)
        {
            DeserializeTo(reader, ref target, options);
        }
    }

    /// <summary>
    /// 支持大小预估的泛型格式化器基类
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    public abstract class SizedFormatterBase<T> : FormatterBase<T>, ISizedFormatter<T>
    {
        /// <inheritdoc/>
        int ISizedFormatter<T>.GetSerializedSize(T value) => GetSerializedSize(value);

        /// <inheritdoc/>
        int ISizedFormatter<T>.FixedSize => FixedSize;

        /// <inheritdoc/>
        bool ISizedFormatter<T>.IsFixedSize => IsFixedSize;
    }

    /// <summary>
    /// 非托管类型格式化器基类
    /// <para>为非托管类型提供零拷贝序列化</para>
    /// </summary>
    /// <typeparam name="T">非托管类型</typeparam>
    public class UnmanagedFormatterBase<T> : FormatterBase<T> where T : unmanaged
    {
        #region 静态字段

        /// <summary>类型大小</summary>
        private static readonly int _size = Unsafe.SizeOf<T>();

        #endregion

        #region 属性

        /// <inheritdoc/>
        public override bool IsFixedSize => true;

        /// <inheritdoc/>
        public override int FixedSize => _size;

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected override void SerializeCore(ISerializeWriter writer, T value, SerializeOptions options)
        {
            writer.WriteUnmanaged(value);
        }

        /// <inheritdoc/>
        protected override T DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            return reader.ReadUnmanaged<T>();
        }

        /// <inheritdoc/>
        public override int GetSerializedSize(T value) => _size;

        #endregion
    }

    /// <summary>
    /// 字符串格式化器基类
    /// </summary>
    public abstract class StringFormatterBase : FormatterBase<string>
    {
        #region 属性

        /// <inheritdoc/>
        public override bool SupportsNull => true;

        /// <inheritdoc/>
        public override bool IsFixedSize => false;

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected override void SerializeCore(ISerializeWriter writer, string value, SerializeOptions options)
        {
            writer.WriteString(value);
        }

        /// <inheritdoc/>
        protected override string DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            return reader.ReadString();
        }

        /// <inheritdoc/>
        public override int GetSerializedSize(string value)
        {
            if (value == null)
                return 1;

            // 长度前缀 (VarInt) + UTF-8 编码的字符串
            var utf8Length = System.Text.Encoding.UTF8.GetByteCount(value);
            var lengthPrefixSize = FormatterHelper.GetVarIntSize(utf8Length);
            return lengthPrefixSize + utf8Length;
        }

        #endregion
    }
}
