// ==========================================================
// 文件名：InitializableFormatterBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 可初始化的格式化器基类
    /// <para>支持延迟初始化和依赖注入</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 支持延迟初始化，避免循环依赖
    /// 2. 线程安全的初始化机制
    /// 3. 初始化后才能执行序列化操作
    /// 
    /// 使用示例:
    /// <code>
    /// public class ComplexFormatter : InitializableFormatterBase
    /// {
    ///     private IFormatter&lt;SubType&gt; _subFormatter;
    ///     
    ///     public override Type TargetType => typeof(ComplexType);
    ///     
    ///     protected override void OnInitialize(IFormatterResolver resolver)
    ///     {
    ///         _subFormatter = resolver.GetFormatter&lt;SubType&gt;();
    ///     }
    ///     
    ///     protected override void SerializeInitialized(ISerializeWriter writer, object value, SerializeOptions options)
    ///     {
    ///         // 使用 _subFormatter 进行序列化
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class InitializableFormatterBase : FormatterBase, IInitializableFormatter
    {
        #region 字段

        /// <summary>是否已初始化</summary>
        private volatile bool _initialized;

        /// <summary>初始化锁</summary>
        private readonly object _initLock = new object();

        #endregion

        #region 属性

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _initialized;

        #endregion

        #region IInitializableFormatter 实现

        /// <inheritdoc/>
        public void Initialize(IFormatterResolver resolver)
        {
            if (_initialized)
                return;

            lock (_initLock)
            {
                if (_initialized)
                    return;

                OnInitialize(resolver);
                _initialized = true;
            }
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 初始化回调
        /// </summary>
        /// <param name="resolver">格式化器解析器</param>
        protected abstract void OnInitialize(IFormatterResolver resolver);

        /// <summary>
        /// 初始化后的序列化逻辑
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">序列化选项</param>
        protected abstract void SerializeInitialized(ISerializeWriter writer, object value, SerializeOptions options);

        /// <summary>
        /// 初始化后的反序列化逻辑
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract object DeserializeInitialized(ISerializeReader reader, DeserializeOptions options);

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected sealed override void SerializeCore(ISerializeWriter writer, object value, SerializeOptions options)
        {
            EnsureInitialized();
            SerializeInitialized(writer, value, options);
        }

        /// <inheritdoc/>
        protected sealed override object DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            EnsureInitialized();
            return DeserializeInitialized(reader, options);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 确保已初始化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException(
                    $"格式化器 {Name} 尚未初始化，请先调用 Initialize 方法");
            }
        }

        #endregion
    }
}
