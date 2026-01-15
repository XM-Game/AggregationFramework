// ==========================================================
// 文件名：FormatterBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器抽象基类
    /// <para>提供 IFormatter 接口的基础实现</para>
    /// <para>处理运行时类型的序列化和反序列化</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 提供非泛型格式化器的基础实现
    /// 2. 支持运行时类型处理
    /// 3. 提供类型验证和错误处理
    /// 4. 子类通过重写抽象方法实现具体逻辑
    /// 
    /// 使用示例:
    /// <code>
    /// public class DynamicFormatter : FormatterBase
    /// {
    ///     public override Type TargetType => typeof(object);
    ///     
    ///     protected override void SerializeCore(ISerializeWriter writer, object value, SerializeOptions options)
    ///     {
    ///         // 实现动态类型序列化逻辑
    ///     }
    ///     
    ///     protected override object DeserializeCore(ISerializeReader reader, DeserializeOptions options)
    ///     {
    ///         // 实现动态类型反序列化逻辑
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class FormatterBase : IFormatter
    {
        #region 属性

        /// <summary>
        /// 获取目标类型
        /// </summary>
        public abstract Type TargetType { get; }

        /// <summary>
        /// 获取格式化器名称
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// 获取格式化器描述
        /// </summary>
        public virtual string Description => $"{TargetType.Name} 格式化器";

        /// <summary>
        /// 是否支持 null 值
        /// </summary>
        public virtual bool SupportsNull => !TargetType.IsValueType;

        /// <summary>
        /// 是否为固定大小类型
        /// </summary>
        public virtual bool IsFixedSize => false;

        /// <summary>
        /// 固定大小 (如果 IsFixedSize 为 true)
        /// </summary>
        public virtual int FixedSize => -1;

        #endregion

        #region IFormatter 实现

        /// <inheritdoc/>
        public void Serialize(ISerializeWriter writer, object value, SerializeOptions options)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // null 值处理
            if (value == null)
            {
                if (!SupportsNull)
                    throw new ArgumentNullException(nameof(value), $"类型 {TargetType.Name} 不支持 null 值");

                SerializeNull(writer, options);
                return;
            }

            // 类型验证
            ValidateType(value.GetType());

            // 执行序列化
            SerializeCore(writer, value, options);
        }

        /// <inheritdoc/>
        public object Deserialize(ISerializeReader reader, DeserializeOptions options)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // 检查 null 标记
            if (TryDeserializeNull(reader, options, out var isNull) && isNull)
                return GetDefaultValue();

            // 执行反序列化
            return DeserializeCore(reader, options);
        }

        /// <inheritdoc/>
        public void DeserializeTo(ISerializeReader reader, object target, DeserializeOptions options)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            // 类型验证
            ValidateType(target.GetType());

            // 检查 null 标记
            if (TryDeserializeNull(reader, options, out var isNull) && isNull)
                return; // null 数据，不修改目标对象

            // 执行反序列化
            DeserializeToCore(reader, target, options);
        }

        /// <inheritdoc/>
        public virtual int GetSerializedSize(object value)
        {
            if (value == null)
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
        protected abstract void SerializeCore(ISerializeWriter writer, object value, SerializeOptions options);

        /// <summary>
        /// 核心反序列化逻辑
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract object DeserializeCore(ISerializeReader reader, DeserializeOptions options);

        #endregion

        #region 虚方法 - 子类可选重写

        /// <summary>
        /// 反序列化到现有对象
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="target">目标对象</param>
        /// <param name="options">反序列化选项</param>
        protected virtual void DeserializeToCore(ISerializeReader reader, object target, DeserializeOptions options)
        {
            // 默认实现：反序列化后复制属性
            var deserialized = DeserializeCore(reader, options);
            FormatterHelper.CopyProperties(deserialized, target, TargetType);
        }

        /// <summary>
        /// 序列化 null 值
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="options">序列化选项</param>
        protected virtual void SerializeNull(ISerializeWriter writer, SerializeOptions options)
        {
            writer.WriteNullObjectHeader();
        }

        /// <summary>
        /// 尝试反序列化 null 值
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <param name="isNull">是否为 null</param>
        /// <returns>如果成功检测返回 true</returns>
        protected virtual bool TryDeserializeNull(ISerializeReader reader, DeserializeOptions options, out bool isNull)
        {
            isNull = reader.TryReadNullObjectHeader();
            return true;
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <returns>类型的默认值</returns>
        protected virtual object GetDefaultValue()
        {
            return TargetType.IsValueType ? Activator.CreateInstance(TargetType) : null;
        }

        /// <summary>
        /// 估算序列化大小
        /// </summary>
        /// <param name="value">要序列化的对象</param>
        /// <returns>估算的字节数</returns>
        protected virtual int EstimateSize(object value)
        {
            return BufferConstants.Writer.InitialBufferSize;
        }

        /// <summary>
        /// 验证类型
        /// </summary>
        /// <param name="type">要验证的类型</param>
        protected virtual void ValidateType(Type type)
        {
            if (!TargetType.IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"类型不匹配: 期望 {TargetType.Name} 或其派生类型，实际 {type.Name}",
                    nameof(type));
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建类型实例
        /// </summary>
        /// <returns>新实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object CreateInstance()
        {
            return Activator.CreateInstance(TargetType);
        }

        /// <summary>
        /// 创建类型实例 (带参数)
        /// </summary>
        /// <param name="args">构造函数参数</param>
        /// <returns>新实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object CreateInstance(params object[] args)
        {
            return Activator.CreateInstance(TargetType, args);
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"{Name} [{TargetType.Name}]";
        }

        #endregion
    }
}
