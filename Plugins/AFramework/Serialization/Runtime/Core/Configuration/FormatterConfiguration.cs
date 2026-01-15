// ==========================================================
// 文件名：FormatterConfiguration.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器配置
    /// <para>管理格式化器的注册、解析和缓存策略</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>注册自定义格式化器</item>
    ///   <item>配置格式化器解析优先级</item>
    ///   <item>管理格式化器缓存</item>
    ///   <item>配置类型映射规则</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// var config = new FormatterConfiguration();
    /// 
    /// // 注册自定义格式化器
    /// config.RegisterFormatter&lt;MyType&gt;(new MyTypeFormatter());
    /// 
    /// // 注册格式化器工厂
    /// config.RegisterFormatterFactory(type => 
    /// {
    ///     if (type.IsGenericType)
    ///         return CreateGenericFormatter(type);
    ///     return null;
    /// });
    /// </code>
    /// </remarks>
    public sealed class FormatterConfiguration : ConfigurationBase<FormatterConfiguration>
    {
        #region 静态实例

        /// <summary>默认配置</summary>
        public static FormatterConfiguration Default { get; } = new FormatterConfiguration().AsReadOnly();

        #endregion

        #region 字段 - 格式化器注册

        private readonly Dictionary<Type, object> _formatters = new Dictionary<Type, object>();
        private readonly List<FormatterFactoryEntry> _factories = new List<FormatterFactoryEntry>();
        private readonly List<ResolverEntry> _resolvers = new List<ResolverEntry>();

        #endregion

        #region 字段 - 缓存配置

        private bool _enableCache = true;
        private int _maxCacheSize = 1024;
        private TimeSpan _cacheExpiration = TimeSpan.Zero;

        #endregion

        #region 字段 - 解析配置

        private bool _allowDynamicGeneration = true;
        private bool _throwOnMissingFormatter = true;
        private bool _useReflectionFallback = true;

        #endregion

        #region 字段 - 字符串配置

        private StringEncoding _defaultStringEncoding = StringEncoding.UTF8;
        private bool _internStrings;
        private int _maxStringInternLength = 128;

        #endregion

        #region 字段 - 集合配置

        private int _defaultCollectionCapacity = 16;
        private int _maxCollectionSize = int.MaxValue;
        private bool _preserveCollectionOrder = true;

        #endregion

        #region 字段 - 数值配置

        private bool _useVarInt = true;
        private bool _useZigZagEncoding = true;

        #endregion

        #region 字段 - 日期时间配置

        private DateTimeKind _defaultDateTimeKind = DateTimeKind.Utc;
        private bool _preserveDateTimeKind = true;

        #endregion

        #region 字段 - 枚举配置

        private EnumSerializeMode _enumMode = EnumSerializeMode.Value;

        #endregion

        #region 属性 - 缓存配置

        /// <summary>
        /// 获取或设置是否启用格式化器缓存
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableCache
        {
            get => _enableCache;
            set { ThrowIfReadOnly(); _enableCache = value; }
        }

        /// <summary>
        /// 获取或设置最大缓存大小
        /// <para>默认值：1024</para>
        /// </summary>
        public int MaxCacheSize
        {
            get => _maxCacheSize;
            set { ThrowIfReadOnly(); _maxCacheSize = Math.Max(0, value); }
        }

        /// <summary>
        /// 获取或设置缓存过期时间
        /// <para>默认值：TimeSpan.Zero（永不过期）</para>
        /// </summary>
        public TimeSpan CacheExpiration
        {
            get => _cacheExpiration;
            set { ThrowIfReadOnly(); _cacheExpiration = value; }
        }

        #endregion

        #region 属性 - 解析配置

        /// <summary>
        /// 获取或设置是否允许动态生成格式化器
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowDynamicGeneration
        {
            get => _allowDynamicGeneration;
            set { ThrowIfReadOnly(); _allowDynamicGeneration = value; }
        }

        /// <summary>
        /// 获取或设置找不到格式化器时是否抛出异常
        /// <para>默认值：true</para>
        /// </summary>
        public bool ThrowOnMissingFormatter
        {
            get => _throwOnMissingFormatter;
            set { ThrowIfReadOnly(); _throwOnMissingFormatter = value; }
        }

        /// <summary>
        /// 获取或设置是否使用反射回退
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseReflectionFallback
        {
            get => _useReflectionFallback;
            set { ThrowIfReadOnly(); _useReflectionFallback = value; }
        }

        #endregion

        #region 属性 - 字符串配置

        /// <summary>
        /// 获取或设置默认字符串编码
        /// <para>默认值：<see cref="StringEncoding.UTF8"/></para>
        /// </summary>
        public StringEncoding DefaultStringEncoding
        {
            get => _defaultStringEncoding;
            set { ThrowIfReadOnly(); _defaultStringEncoding = value; }
        }

        /// <summary>
        /// 获取或设置是否内化字符串
        /// <para>默认值：false</para>
        /// </summary>
        public bool InternStrings
        {
            get => _internStrings;
            set { ThrowIfReadOnly(); _internStrings = value; }
        }

        /// <summary>
        /// 获取或设置最大字符串内化长度
        /// <para>默认值：128</para>
        /// </summary>
        public int MaxStringInternLength
        {
            get => _maxStringInternLength;
            set { ThrowIfReadOnly(); _maxStringInternLength = Math.Max(0, value); }
        }

        #endregion

        #region 属性 - 集合配置

        /// <summary>
        /// 获取或设置默认集合容量
        /// <para>默认值：16</para>
        /// </summary>
        public int DefaultCollectionCapacity
        {
            get => _defaultCollectionCapacity;
            set { ThrowIfReadOnly(); _defaultCollectionCapacity = Math.Max(0, value); }
        }

        /// <summary>
        /// 获取或设置最大集合大小
        /// <para>默认值：int.MaxValue</para>
        /// </summary>
        public int MaxCollectionSize
        {
            get => _maxCollectionSize;
            set { ThrowIfReadOnly(); _maxCollectionSize = Math.Max(0, value); }
        }

        /// <summary>
        /// 获取或设置是否保持集合顺序
        /// <para>默认值：true</para>
        /// </summary>
        public bool PreserveCollectionOrder
        {
            get => _preserveCollectionOrder;
            set { ThrowIfReadOnly(); _preserveCollectionOrder = value; }
        }

        #endregion

        #region 属性 - 数值配置

        /// <summary>
        /// 获取或设置是否使用变长整数编码
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseVarInt
        {
            get => _useVarInt;
            set { ThrowIfReadOnly(); _useVarInt = value; }
        }

        /// <summary>
        /// 获取或设置是否使用 ZigZag 编码
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseZigZagEncoding
        {
            get => _useZigZagEncoding;
            set { ThrowIfReadOnly(); _useZigZagEncoding = value; }
        }

        #endregion

        #region 属性 - 日期时间配置

        /// <summary>
        /// 获取或设置默认日期时间类型
        /// <para>默认值：<see cref="DateTimeKind.Utc"/></para>
        /// </summary>
        public DateTimeKind DefaultDateTimeKind
        {
            get => _defaultDateTimeKind;
            set { ThrowIfReadOnly(); _defaultDateTimeKind = value; }
        }

        /// <summary>
        /// 获取或设置是否保持日期时间类型
        /// <para>默认值：true</para>
        /// </summary>
        public bool PreserveDateTimeKind
        {
            get => _preserveDateTimeKind;
            set { ThrowIfReadOnly(); _preserveDateTimeKind = value; }
        }

        #endregion

        #region 属性 - 枚举配置

        /// <summary>
        /// 获取或设置枚举序列化模式
        /// <para>默认值：<see cref="EnumSerializeMode.Value"/></para>
        /// </summary>
        public EnumSerializeMode EnumMode
        {
            get => _enumMode;
            set { ThrowIfReadOnly(); _enumMode = value; }
        }

        #endregion

        #region 属性 - 只读

        /// <summary>获取已注册的格式化器数量</summary>
        public int FormatterCount => _formatters.Count;

        /// <summary>获取已注册的解析器数量</summary>
        public int ResolverCount => _resolvers.Count;

        /// <summary>获取已注册的工厂数量</summary>
        public int FactoryCount => _factories.Count;

        #endregion

        #region 公共方法 - 格式化器注册

        /// <summary>
        /// 注册格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="formatter">格式化器实例</param>
        public void RegisterFormatter<T>(IFormatter<T> formatter)
        {
            ThrowIfReadOnly();
            _formatters[typeof(T)] = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        /// <summary>
        /// 注册格式化器（非泛型）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="formatter">格式化器实例</param>
        public void RegisterFormatter(Type type, object formatter)
        {
            ThrowIfReadOnly();
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            _formatters[type] = formatter;
        }

        /// <summary>
        /// 注册格式化器工厂
        /// </summary>
        /// <param name="factory">工厂委托</param>
        /// <param name="priority">优先级（越小越优先）</param>
        public void RegisterFormatterFactory(Func<Type, object> factory, int priority = 0)
        {
            ThrowIfReadOnly();
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _factories.Add(new FormatterFactoryEntry(factory, priority));
            _factories.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        /// <summary>
        /// 添加格式化器解析器
        /// </summary>
        /// <param name="resolver">解析器实例</param>
        /// <param name="priority">优先级（越小越优先）</param>
        public void AddResolver(IFormatterResolver resolver, int priority = 0)
        {
            ThrowIfReadOnly();
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            _resolvers.Add(new ResolverEntry(resolver, priority));
            _resolvers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        /// <summary>
        /// 尝试获取格式化器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="formatter">输出格式化器</param>
        /// <returns>如果找到返回 true</returns>
        public bool TryGetFormatter<T>(out IFormatter<T> formatter)
        {
            if (_formatters.TryGetValue(typeof(T), out var obj) && obj is IFormatter<T> f)
            {
                formatter = f;
                return true;
            }
            formatter = null;
            return false;
        }

        /// <summary>
        /// 尝试获取格式化器（非泛型）
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="formatter">输出格式化器</param>
        /// <returns>如果找到返回 true</returns>
        public bool TryGetFormatter(Type type, out object formatter) =>
            _formatters.TryGetValue(type, out formatter);

        /// <summary>
        /// 检查是否已注册格式化器
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>如果已注册返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasFormatter(Type type) => _formatters.ContainsKey(type);

        /// <summary>
        /// 移除格式化器
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>如果成功移除返回 true</returns>
        public bool RemoveFormatter(Type type)
        {
            ThrowIfReadOnly();
            return _formatters.Remove(type);
        }

        /// <summary>
        /// 清空所有格式化器
        /// </summary>
        public void ClearFormatters()
        {
            ThrowIfReadOnly();
            _formatters.Clear();
        }

        /// <summary>
        /// 清空所有解析器
        /// </summary>
        public void ClearResolvers()
        {
            ThrowIfReadOnly();
            _resolvers.Clear();
        }

        /// <summary>
        /// 清空所有工厂
        /// </summary>
        public void ClearFactories()
        {
            ThrowIfReadOnly();
            _factories.Clear();
        }

        /// <summary>
        /// 获取所有解析器
        /// </summary>
        /// <returns>解析器列表</returns>
        public IReadOnlyList<IFormatterResolver> GetResolvers()
        {
            var result = new List<IFormatterResolver>(_resolvers.Count);
            foreach (var entry in _resolvers)
                result.Add(entry.Resolver);
            return result;
        }

        /// <inheritdoc/>
        public override string GetSummary() =>
            $"Formatter[Formatters={_formatters.Count}, Resolvers={_resolvers.Count}, Cache={_enableCache}]";

        #endregion

        #region 保护方法

        /// <inheritdoc/>
        protected override void CopyTo(FormatterConfiguration target)
        {
            foreach (var kvp in _formatters)
                target._formatters[kvp.Key] = kvp.Value;

            target._factories.AddRange(_factories);
            target._resolvers.AddRange(_resolvers);

            target._enableCache = _enableCache;
            target._maxCacheSize = _maxCacheSize;
            target._cacheExpiration = _cacheExpiration;
            target._allowDynamicGeneration = _allowDynamicGeneration;
            target._throwOnMissingFormatter = _throwOnMissingFormatter;
            target._useReflectionFallback = _useReflectionFallback;
            target._defaultStringEncoding = _defaultStringEncoding;
            target._internStrings = _internStrings;
            target._maxStringInternLength = _maxStringInternLength;
            target._defaultCollectionCapacity = _defaultCollectionCapacity;
            target._maxCollectionSize = _maxCollectionSize;
            target._preserveCollectionOrder = _preserveCollectionOrder;
            target._useVarInt = _useVarInt;
            target._useZigZagEncoding = _useZigZagEncoding;
            target._defaultDateTimeKind = _defaultDateTimeKind;
            target._preserveDateTimeKind = _preserveDateTimeKind;
            target._enumMode = _enumMode;
        }

        #endregion

        #region 内部类型

        /// <summary>
        /// 格式化器工厂条目
        /// </summary>
        private readonly struct FormatterFactoryEntry
        {
            public readonly Func<Type, object> Factory;
            public readonly int Priority;

            public FormatterFactoryEntry(Func<Type, object> factory, int priority)
            {
                Factory = factory;
                Priority = priority;
            }
        }

        /// <summary>
        /// 解析器条目
        /// </summary>
        private readonly struct ResolverEntry
        {
            public readonly IFormatterResolver Resolver;
            public readonly int Priority;

            public ResolverEntry(IFormatterResolver resolver, int priority)
            {
                Resolver = resolver;
                Priority = priority;
            }
        }

        #endregion
    }
}
