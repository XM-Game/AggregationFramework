// ==========================================================
// 文件名：SerializerBaseT.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Buffers
// ==========================================================

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 泛型序列化器抽象基类
    /// <para>提供 ISerializer&lt;T&gt; 接口的基础实现</para>
    /// <para>提供类型安全的序列化操作，避免装箱拆箱</para>
    /// </summary>
    /// <typeparam name="T">要序列化的类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 继承自 SerializerBase，复用基础功能
    /// 2. 提供泛型 API，避免装箱拆箱开销
    /// 3. 支持类型特化的优化实现
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerSerializer : SerializerBase&lt;Player&gt;
    /// {
    ///     protected override int SerializeTyped(Player value, Span&lt;byte&gt; buffer, SerializeOptions options)
    ///     {
    ///         // 实现 Player 类型的序列化逻辑
    ///     }
    ///     
    ///     protected override Player DeserializeTyped(ReadOnlySpan&lt;byte&gt; data, DeserializeOptions options)
    ///     {
    ///         // 实现 Player 类型的反序列化逻辑
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class SerializerBase<T> : SerializerBase, ISerializer<T>
    {
        #region 静态字段

        /// <summary>类型专用格式化器</summary>
        protected readonly IFormatter<T> _formatter;

        /// <summary>目标类型</summary>
        private static readonly Type _targetType = typeof(T);

        /// <summary>是否为值类型</summary>
        private static readonly bool _isValueType = typeof(T).IsValueType;

        /// <summary>是否为非托管类型 (延迟初始化)</summary>
        private static readonly Lazy<bool> _isUnmanaged = new Lazy<bool>(() => FormatterHelper.IsUnmanagedType(_targetType));

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建泛型序列化器基类实例
        /// </summary>
        protected SerializerBase() : base(null)
        {
            _formatter = null;
        }

        /// <summary>
        /// 创建泛型序列化器基类实例
        /// </summary>
        /// <param name="resolver">格式化器解析器</param>
        protected SerializerBase(IFormatterResolver resolver) : base(resolver)
        {
            _formatter = resolver?.GetFormatter<T>();
        }

        /// <summary>
        /// 创建泛型序列化器基类实例
        /// </summary>
        /// <param name="formatter">类型专用格式化器</param>
        protected SerializerBase(IFormatter<T> formatter) : base((IFormatterResolver)null)
        {
            _formatter = formatter;
        }

        #endregion

        #region ISerializer<T> 实现 - 序列化方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Serialize(T value)
        {
            return Serialize(value, DefaultSerializeOptions);
        }

        /// <inheritdoc/>
        public byte[] Serialize(T value, SerializeOptions options)
        {
            ThrowIfDisposed();

            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
                return SerializeNull(options);

            // 预估大小
            var estimatedSize = GetSerializedSize(value);
            if (estimatedSize <= 0)
                estimatedSize = DefaultInitialBufferSize;

            // 从池中租用缓冲区
            var buffer = ArrayPool<byte>.Shared.Rent(estimatedSize);
            try
            {
                var written = SerializeTyped(value, buffer, options);

                // 复制到精确大小的数组
                var result = new byte[written];
                Buffer.BlockCopy(buffer, 0, result, 0, written);
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Serialize(T value, Span<byte> buffer)
        {
            return Serialize(value, buffer, DefaultSerializeOptions);
        }

        /// <inheritdoc/>
        public int Serialize(T value, Span<byte> buffer, SerializeOptions options)
        {
            ThrowIfDisposed();

            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
                return SerializeNullToBuffer(buffer, options);

            return SerializeTyped(value, buffer, options);
        }

        /// <inheritdoc/>
        public SerializeResult SerializeWithResult(T value, SerializeOptions options = default)
        {
            ThrowIfDisposed();

            if (options.Equals(default(SerializeOptions)))
                options = DefaultSerializeOptions;

            var startTime = DateTime.UtcNow;

            try
            {
                var data = Serialize(value, options);
                var elapsed = DateTime.UtcNow - startTime;
                var statistics = SerializeStatistics.Create(
                    elapsed.Ticks,
                    data?.Length ?? 0,
                    bytesBeforeCompression: data?.Length ?? 0
                );
                return SerializeResult.Success(data, statistics);
            }
            catch (Exception ex)
            {
                return SerializeResult.FromException(ex);
            }
        }

        #endregion

        #region ISerializer<T> 实现 - 反序列化方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Deserialize(ReadOnlySpan<byte> data)
        {
            return Deserialize(data, DefaultDeserializeOptions);
        }

        /// <inheritdoc/>
        public T Deserialize(ReadOnlySpan<byte> data, DeserializeOptions options)
        {
            ThrowIfDisposed();

            if (data.IsEmpty)
                return default;

            // 检查是否为 null 标记
            if (IsNullData(data))
                return default;

            return DeserializeTyped(data, options);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeserializeTo(ReadOnlySpan<byte> data, ref T target)
        {
            DeserializeTo(data, ref target, DefaultDeserializeOptions);
        }

        /// <inheritdoc/>
        public void DeserializeTo(ReadOnlySpan<byte> data, ref T target, DeserializeOptions options)
        {
            ThrowIfDisposed();

            if (data.IsEmpty)
                return;

            // 检查是否为 null 标记
            if (IsNullData(data))
            {
                target = default;
                return;
            }

            DeserializeToTyped(data, ref target, options);
        }

        /// <inheritdoc/>
        public DeserializeResult<T> DeserializeWithResult(ReadOnlySpan<byte> data, DeserializeOptions options = default)
        {
            ThrowIfDisposed();

            if (options.Equals(default(DeserializeOptions)))
                options = DefaultDeserializeOptions;

            var startTime = DateTime.UtcNow;

            try
            {
                var value = Deserialize(data, options);
                var elapsed = DateTime.UtcNow - startTime;
                var statistics = DeserializeStatistics.Create(elapsed.Ticks, data.Length);
                return DeserializeResult<T>.Success(value, statistics);
            }
            catch (Exception ex)
            {
                return DeserializeResult<T>.FromException(ex);
            }
        }

        #endregion

        #region ISerializer<T> 实现 - 辅助方法

        /// <inheritdoc/>
        public virtual int GetSerializedSize(T value)
        {
            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
                return 1; // null 标记

            // 如果有大小感知的格式化器，使用它
            if (_formatter is ISizedFormatter<T> sizedFormatter)
                return sizedFormatter.GetSerializedSize(value);

            // 非托管类型可以直接计算大小
            if (_isUnmanaged.Value)
                return Unsafe.SizeOf<T>();

            // 默认估算
            return EstimateSizeTyped(value);
        }

        /// <inheritdoc/>
        public bool TrySerialize(T value, Span<byte> buffer, out int bytesWritten)
        {
            bytesWritten = 0;

            try
            {
                bytesWritten = Serialize(value, buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool TryDeserialize(ReadOnlySpan<byte> data, out T value)
        {
            value = default;

            try
            {
                value = Deserialize(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Clone(T value)
        {
            // 值类型不需要 null 检查
            if (!_isValueType && value == null)
                return default;

            // 通过序列化/反序列化实现深拷贝
            var data = Serialize(value);
            return Deserialize(data);
        }

        #endregion

        #region SerializerBase 重写

        /// <inheritdoc/>
        protected sealed override int SerializeCore(object value, Span<byte> buffer, SerializeOptions options)
        {
            return SerializeTyped((T)value, buffer, options);
        }

        /// <inheritdoc/>
        protected sealed override object DeserializeCore(ReadOnlySpan<byte> data, Type type, DeserializeOptions options)
        {
            if (type != _targetType && !type.IsAssignableFrom(_targetType))
            {
                throw new InvalidOperationException(
                    $"类型不匹配: 期望 {_targetType.Name}，实际 {type.Name}");
            }

            return DeserializeTyped(data, options);
        }

        /// <inheritdoc/>
        protected sealed override void DeserializeToCore(ReadOnlySpan<byte> data, object target, DeserializeOptions options)
        {
            var typedTarget = (T)target;
            DeserializeToTyped(data, ref typedTarget, options);
        }

        /// <inheritdoc/>
        public sealed override int GetSerializedSize(object value)
        {
            return GetSerializedSize((T)value);
        }

        /// <inheritdoc/>
        public sealed override bool CanSerialize(Type type)
        {
            return type == _targetType || _targetType.IsAssignableFrom(type);
        }

        #endregion

        #region 抽象方法 - 子类必须实现

        /// <summary>
        /// 类型化的核心序列化逻辑
        /// </summary>
        protected abstract int SerializeTyped(T value, Span<byte> buffer, SerializeOptions options);

        /// <summary>
        /// 类型化的核心反序列化逻辑
        /// </summary>
        protected abstract T DeserializeTyped(ReadOnlySpan<byte> data, DeserializeOptions options);

        #endregion

        #region 虚方法 - 子类可选重写

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        protected virtual void DeserializeToTyped(ReadOnlySpan<byte> data, ref T target, DeserializeOptions options)
        {
            target = DeserializeTyped(data, options);
        }

        /// <summary>
        /// 估算类型化对象的序列化大小
        /// </summary>
        protected virtual int EstimateSizeTyped(T value)
        {
            return DefaultInitialBufferSize;
        }

        #endregion
    }
}
