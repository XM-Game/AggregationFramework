// ==========================================================
// 文件名：SerializerBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Buffers
// ==========================================================

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化器抽象基类
    /// <para>提供 ISerializer 接口的基础实现</para>
    /// <para>子类只需实现核心序列化逻辑</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用模板方法模式，定义序列化流程骨架
    /// 2. 提供默认的选项管理和错误处理
    /// 3. 支持缓冲区池化，减少内存分配
    /// 4. 子类通过重写抽象方法实现具体序列化逻辑
    /// 
    /// 使用示例:
    /// <code>
    /// public class BinarySerializer : SerializerBase
    /// {
    ///     protected override int SerializeCore(object value, Span&lt;byte&gt; buffer, SerializeOptions options)
    ///     {
    ///         // 实现二进制序列化逻辑
    ///     }
    ///     
    ///     protected override object DeserializeCore(ReadOnlySpan&lt;byte&gt; data, Type type, DeserializeOptions options)
    ///     {
    ///         // 实现二进制反序列化逻辑
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class SerializerBase : ISerializer, IDisposable
    {
        #region 常量

        /// <summary>默认初始缓冲区大小</summary>
        protected const int DefaultInitialBufferSize = BufferConstants.Writer.InitialBufferSize;

        /// <summary>最大缓冲区大小</summary>
        protected const int MaxBufferSize = BufferConstants.MaxBufferSize;

        #endregion

        #region 字段

        /// <summary>默认序列化选项</summary>
        private SerializeOptions _defaultSerializeOptions;

        /// <summary>默认反序列化选项</summary>
        private DeserializeOptions _defaultDeserializeOptions;

        /// <summary>格式化器解析器</summary>
        protected readonly IFormatterResolver _resolver;

        /// <summary>是否已释放</summary>
        private bool _disposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建序列化器基类实例
        /// </summary>
        protected SerializerBase() : this(null)
        {
        }

        /// <summary>
        /// 创建序列化器基类实例
        /// </summary>
        /// <param name="resolver">格式化器解析器</param>
        protected SerializerBase(IFormatterResolver resolver)
        {
            _resolver = resolver;
            _defaultSerializeOptions = SerializeOptions.Default;
            _defaultDeserializeOptions = DeserializeOptions.Default;
        }

        #endregion

        #region ISerializer 实现 - 序列化方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Serialize(object value)
        {
            return Serialize(value, _defaultSerializeOptions);
        }

        /// <inheritdoc/>
        public byte[] Serialize(object value, SerializeOptions options)
        {
            ThrowIfDisposed();

            if (value == null)
                return SerializeNull(options);

            // 预估大小
            var estimatedSize = GetSerializedSize(value);
            if (estimatedSize <= 0)
                estimatedSize = DefaultInitialBufferSize;

            // 从池中租用缓冲区
            var buffer = ArrayPool<byte>.Shared.Rent(estimatedSize);
            try
            {
                var written = SerializeToBuffer(value, buffer, options);

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
        public int Serialize(object value, Span<byte> buffer)
        {
            return Serialize(value, buffer, _defaultSerializeOptions);
        }

        /// <inheritdoc/>
        public int Serialize(object value, Span<byte> buffer, SerializeOptions options)
        {
            ThrowIfDisposed();

            if (value == null)
                return SerializeNullToBuffer(buffer, options);

            return SerializeToBuffer(value, buffer, options);
        }

        /// <inheritdoc/>
        public SerializeResult SerializeWithResult(object value, SerializeOptions options = default)
        {
            ThrowIfDisposed();

            if (options.Equals(default(SerializeOptions)))
                options = _defaultSerializeOptions;

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

        #region ISerializer 实现 - 反序列化方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Deserialize(ReadOnlySpan<byte> data, Type type)
        {
            return Deserialize(data, type, _defaultDeserializeOptions);
        }

        /// <inheritdoc/>
        public object Deserialize(ReadOnlySpan<byte> data, Type type, DeserializeOptions options)
        {
            ThrowIfDisposed();

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (data.IsEmpty)
                return GetDefaultValue(type);

            // 检查是否为 null 标记
            if (IsNullData(data))
                return null;

            return DeserializeCore(data, type, options);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeserializeTo(ReadOnlySpan<byte> data, object target)
        {
            DeserializeTo(data, target, _defaultDeserializeOptions);
        }

        /// <inheritdoc/>
        public void DeserializeTo(ReadOnlySpan<byte> data, object target, DeserializeOptions options)
        {
            ThrowIfDisposed();

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (data.IsEmpty)
                return;

            DeserializeToCore(data, target, options);
        }

        #endregion

        #region ISerializer 实现 - 辅助方法

        /// <inheritdoc/>
        public virtual int GetSerializedSize(object value)
        {
            if (value == null)
                return 1; // null 标记

            return EstimateSize(value);
        }

        /// <inheritdoc/>
        public virtual bool CanSerialize(Type type)
        {
            if (type == null)
                return false;

            // 检查是否有对应的格式化器
            if (_resolver != null)
                return _resolver.CanResolve(type);

            // 默认支持基础类型
            return SerializerHelper.IsBuiltInType(type);
        }

        /// <inheritdoc/>
        public SerializeOptions DefaultSerializeOptions
        {
            get => _defaultSerializeOptions;
            set => _defaultSerializeOptions = value;
        }

        /// <inheritdoc/>
        public DeserializeOptions DefaultDeserializeOptions
        {
            get => _defaultDeserializeOptions;
            set => _defaultDeserializeOptions = value;
        }

        #endregion

        #region 抽象方法 - 子类必须实现

        /// <summary>
        /// 核心序列化逻辑
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="options">序列化选项</param>
        /// <returns>写入的字节数</returns>
        protected abstract int SerializeCore(object value, Span<byte> buffer, SerializeOptions options);

        /// <summary>
        /// 核心反序列化逻辑
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <param name="type">目标类型</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract object DeserializeCore(ReadOnlySpan<byte> data, Type type, DeserializeOptions options);

        #endregion

        #region 虚方法 - 子类可选重写

        /// <summary>
        /// 反序列化到现有对象的核心逻辑
        /// </summary>
        protected virtual void DeserializeToCore(ReadOnlySpan<byte> data, object target, DeserializeOptions options)
        {
            var deserialized = DeserializeCore(data, target.GetType(), options);
            FormatterHelper.CopyProperties(deserialized, target, target.GetType());
        }

        /// <summary>
        /// 序列化 null 值
        /// </summary>
        protected virtual byte[] SerializeNull(SerializeOptions options)
        {
            return new byte[] { FormatConstants.NullMarker };
        }

        /// <summary>
        /// 序列化 null 值到缓冲区
        /// </summary>
        protected virtual int SerializeNullToBuffer(Span<byte> buffer, SerializeOptions options)
        {
            if (buffer.Length < 1)
                throw new ArgumentException("缓冲区太小", nameof(buffer));

            buffer[0] = FormatConstants.NullMarker;
            return 1;
        }

        /// <summary>
        /// 检查数据是否为 null 标记
        /// </summary>
        protected virtual bool IsNullData(ReadOnlySpan<byte> data)
        {
            return data.Length == 1 && data[0] == FormatConstants.NullMarker;
        }

        /// <summary>
        /// 估算序列化大小
        /// </summary>
        protected virtual int EstimateSize(object value)
        {
            return SerializerHelper.EstimateSize(value);
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        protected virtual object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 释放时的回调
        /// </summary>
        protected virtual void OnDispose()
        {
            // 子类可重写以释放资源
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 序列化到缓冲区（带自动扩容）
        /// </summary>
        private int SerializeToBuffer(object value, Span<byte> buffer, SerializeOptions options)
        {
            try
            {
                return SerializeCore(value, buffer, options);
            }
            catch (ArgumentException) when (buffer.Length < MaxBufferSize)
            {
                // 缓冲区太小，尝试扩容
                var newSize = Math.Min(buffer.Length * 2, MaxBufferSize);
                var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
                try
                {
                    var written = SerializeCore(value, newBuffer, options);
                    if (written <= buffer.Length)
                    {
                        newBuffer.AsSpan(0, written).CopyTo(buffer);
                        return written;
                    }
                    throw new InvalidOperationException(
                        $"序列化数据大小 ({written}) 超过缓冲区容量 ({buffer.Length})");
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(newBuffer);
                }
            }
        }

        /// <summary>
        /// 检查是否已释放
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    OnDispose();

                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~SerializerBase()
        {
            Dispose(false);
        }

        #endregion
    }
}
