// ==========================================================
// 文件名：GlobalConfiguration.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 全局配置
    /// <para>管理序列化系统的全局设置和默认值</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>提供全局默认配置</item>
    ///   <item>管理全局格式化器解析器</item>
    ///   <item>配置全局行为和策略</item>
    ///   <item>线程安全的配置访问</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 配置全局设置
    /// GlobalConfiguration.Default.SerializerConfig.EnableCompression = true;
    /// 
    /// // 使用自定义全局配置
    /// GlobalConfiguration.SetDefault(new GlobalConfiguration
    /// {
    ///     EnableDiagnostics = true
    /// });
    /// </code>
    /// </remarks>
    public sealed class GlobalConfiguration : ConfigurationBase<GlobalConfiguration>
    {
        #region 静态字段

        private static GlobalConfiguration _default;
        private static readonly object _lock = new object();

        /// <summary>版本信息</summary>
        private static readonly Version _version = new Version(1, 0, 0);

        #endregion

        #region 静态属性

        /// <summary>
        /// 获取默认全局配置
        /// </summary>
        public static GlobalConfiguration Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_lock)
                    {
                        _default ??= new GlobalConfiguration();
                    }
                }
                return _default;
            }
        }

        /// <summary>获取版本信息</summary>
        public static Version Version => _version;

        #endregion

        #region 字段 - 子配置

        private SerializerConfiguration _serializerConfig;
        private FormatterConfiguration _formatterConfig;
        private BufferConfiguration _bufferConfig;

        #endregion

        #region 字段 - 全局设置

        private bool _enableDiagnostics;
        private bool _enablePerformanceCounters;
        private bool _throwOnError = true;
        private int _defaultTimeout = 30000;

        #endregion

        #region 字段 - 线程配置

        private bool _useThreadLocalCache = true;
        private int _maxConcurrency = Environment.ProcessorCount;

        #endregion

        #region 字段 - 日志配置

        private Action<string> _logAction;
        private Action<Exception> _errorAction;
        private LogLevel _minLogLevel = LogLevel.Warning;

        #endregion

        #region 字段 - 事件

        private Action<Type> _onFormatterResolved;
        private Action<Type, Exception> _onFormatterError;

        #endregion

        #region 字段 - 状态

        private bool _isInitialized;

        #endregion

        #region 属性 - 子配置

        /// <summary>
        /// 获取或设置序列化器配置
        /// </summary>
        public SerializerConfiguration SerializerConfig
        {
            get => _serializerConfig ??= new SerializerConfiguration();
            set { ThrowIfReadOnly(); _serializerConfig = value ?? new SerializerConfiguration(); }
        }

        /// <summary>
        /// 获取或设置格式化器配置
        /// </summary>
        public FormatterConfiguration FormatterConfig
        {
            get => _formatterConfig ??= new FormatterConfiguration();
            set { ThrowIfReadOnly(); _formatterConfig = value ?? new FormatterConfiguration(); }
        }

        /// <summary>
        /// 获取或设置缓冲区配置
        /// </summary>
        public BufferConfiguration BufferConfig
        {
            get => _bufferConfig ??= new BufferConfiguration();
            set { ThrowIfReadOnly(); _bufferConfig = value ?? new BufferConfiguration(); }
        }

        #endregion

        #region 属性 - 全局设置

        /// <summary>
        /// 获取或设置是否启用诊断
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableDiagnostics
        {
            get => _enableDiagnostics;
            set { ThrowIfReadOnly(); _enableDiagnostics = value; }
        }

        /// <summary>
        /// 获取或设置是否启用性能计数器
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnablePerformanceCounters
        {
            get => _enablePerformanceCounters;
            set { ThrowIfReadOnly(); _enablePerformanceCounters = value; }
        }

        /// <summary>
        /// 获取或设置错误时是否抛出异常
        /// <para>默认值：true</para>
        /// </summary>
        public bool ThrowOnError
        {
            get => _throwOnError;
            set { ThrowIfReadOnly(); _throwOnError = value; }
        }

        /// <summary>
        /// 获取或设置默认超时时间（毫秒）
        /// <para>默认值：30000</para>
        /// </summary>
        public int DefaultTimeout
        {
            get => _defaultTimeout;
            set { ThrowIfReadOnly(); _defaultTimeout = Math.Max(0, value); }
        }

        #endregion

        #region 属性 - 线程配置

        /// <summary>
        /// 获取或设置是否使用线程本地缓存
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseThreadLocalCache
        {
            get => _useThreadLocalCache;
            set { ThrowIfReadOnly(); _useThreadLocalCache = value; }
        }

        /// <summary>
        /// 获取或设置最大并发数
        /// <para>默认值：处理器数量</para>
        /// </summary>
        public int MaxConcurrency
        {
            get => _maxConcurrency;
            set { ThrowIfReadOnly(); _maxConcurrency = Math.Max(1, value); }
        }

        #endregion

        #region 属性 - 日志配置

        /// <summary>
        /// 获取或设置日志输出委托
        /// </summary>
        public Action<string> LogAction
        {
            get => _logAction;
            set { ThrowIfReadOnly(); _logAction = value; }
        }

        /// <summary>
        /// 获取或设置错误处理委托
        /// </summary>
        public Action<Exception> ErrorAction
        {
            get => _errorAction;
            set { ThrowIfReadOnly(); _errorAction = value; }
        }

        /// <summary>
        /// 获取或设置最小日志级别
        /// <para>默认值：<see cref="LogLevel.Warning"/></para>
        /// </summary>
        public LogLevel MinLogLevel
        {
            get => _minLogLevel;
            set { ThrowIfReadOnly(); _minLogLevel = value; }
        }

        #endregion

        #region 属性 - 事件

        /// <summary>
        /// 获取或设置格式化器解析完成事件
        /// </summary>
        public Action<Type> OnFormatterResolved
        {
            get => _onFormatterResolved;
            set { ThrowIfReadOnly(); _onFormatterResolved = value; }
        }

        /// <summary>
        /// 获取或设置格式化器错误事件
        /// </summary>
        public Action<Type, Exception> OnFormatterError
        {
            get => _onFormatterError;
            set { ThrowIfReadOnly(); _onFormatterError = value; }
        }

        #endregion

        #region 属性 - 状态

        /// <summary>获取是否已初始化</summary>
        public bool IsInitialized => _isInitialized;

        #endregion

        #region 静态方法

        /// <summary>
        /// 设置默认全局配置
        /// </summary>
        /// <param name="config">配置实例</param>
        public static void SetDefault(GlobalConfiguration config)
        {
            lock (_lock)
            {
                _default = config ?? throw new ArgumentNullException(nameof(config));
            }
        }

        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public static void ResetDefault()
        {
            lock (_lock)
            {
                _default = new GlobalConfiguration();
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化配置
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;

                // 确保子配置已创建
                _ = SerializerConfig;
                _ = FormatterConfig;
                _ = BufferConfig;

                _isInitialized = true;
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(LogLevel level, string message)
        {
            if (!_minLogLevel.ShouldLog(level)) return;
            _logAction?.Invoke($"[{level.GetShortName()}] {message}");
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="exception">异常</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogError(Exception exception)
        {
            _errorAction?.Invoke(exception);
            if (_throwOnError) throw exception;
        }

        /// <summary>
        /// 触发格式化器解析事件
        /// </summary>
        /// <param name="type">类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseFormatterResolved(Type type) => _onFormatterResolved?.Invoke(type);

        /// <summary>
        /// 触发格式化器错误事件
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="exception">异常</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseFormatterError(Type type, Exception exception) =>
            _onFormatterError?.Invoke(type, exception);

        /// <inheritdoc/>
        public override string GetSummary() =>
            $"Global[Diag={_enableDiagnostics}, Throw={_throwOnError}, ThreadCache={_useThreadLocalCache}]";

        #endregion

        #region 保护方法

        /// <inheritdoc/>
        protected override void OnAsReadOnly()
        {
            _serializerConfig?.AsReadOnly();
            _formatterConfig?.AsReadOnly();
            _bufferConfig?.AsReadOnly();
        }

        /// <inheritdoc/>
        protected override bool OnValidate(ref string error)
        {
            if (_serializerConfig != null && !_serializerConfig.Validate(out error))
                return false;

            if (_bufferConfig != null && !_bufferConfig.Validate(out error))
                return false;

            return true;
        }

        /// <inheritdoc/>
        protected override void CopyTo(GlobalConfiguration target)
        {
            target._serializerConfig = _serializerConfig?.Clone();
            target._formatterConfig = _formatterConfig?.Clone();
            target._bufferConfig = _bufferConfig?.Clone();

            target._enableDiagnostics = _enableDiagnostics;
            target._enablePerformanceCounters = _enablePerformanceCounters;
            target._throwOnError = _throwOnError;
            target._defaultTimeout = _defaultTimeout;
            target._useThreadLocalCache = _useThreadLocalCache;
            target._maxConcurrency = _maxConcurrency;
            target._logAction = _logAction;
            target._errorAction = _errorAction;
            target._minLogLevel = _minLogLevel;
            target._onFormatterResolved = _onFormatterResolved;
            target._onFormatterError = _onFormatterError;
        }

        #endregion
    }
}
