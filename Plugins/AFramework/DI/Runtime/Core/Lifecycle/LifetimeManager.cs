// ==========================================================
// 文件名：LifetimeManager.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Threading
// 功能: 生命周期管理器，统一管理服务实例的生命周期
// ==========================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace AFramework.DI
{
    /// <summary>
    /// 生命周期管理器
    /// <para>统一管理依赖注入容器中服务实例的生命周期，包括创建、缓存和释放</para>
    /// <para>Lifetime manager that manages service instance lifecycle including creation, caching and disposal</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于生命周期管理，不涉及解析逻辑</item>
    /// <item>策略模式：根据不同生命周期类型采用不同的管理策略</item>
    /// <item>线程安全：所有操作都是线程安全的</item>
    /// </list>
    /// 
    /// 核心功能：
    /// <list type="bullet">
    /// <item>实例缓存管理（单例/作用域）</item>
    /// <item>生命周期验证</item>
    /// <item>可释放对象追踪</item>
    /// <item>诊断信息收集</item>
    /// </list>
    /// </remarks>
    public sealed class LifetimeManager : IDisposable
    {
        #region 字段 / Fields

        /// <summary>
        /// 单例实例缓存
        /// </summary>
        private readonly Dictionary<Type, object> _singletonCache;

        /// <summary>
        /// 键值单例实例缓存
        /// </summary>
        private readonly Dictionary<(Type, object), object> _keyedSingletonCache;

        /// <summary>
        /// 可释放对象追踪器
        /// </summary>
        private readonly DisposableTracker _disposableTracker;

        /// <summary>
        /// 生命周期验证器
        /// </summary>
        private readonly LifetimeValidator _lifetimeValidator;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 是否启用诊断
        /// </summary>
        private readonly bool _enableDiagnostics;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private volatile bool _isDisposed;

        /// <summary>
        /// 实例创建计数
        /// </summary>
        private int _instanceCreationCount;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取单例实例数量
        /// <para>Get the count of singleton instances</para>
        /// </summary>
        public int SingletonCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _singletonCache.Count;
                }
            }
        }

        /// <summary>
        /// 获取键值单例实例数量
        /// <para>Get the count of keyed singleton instances</para>
        /// </summary>
        public int KeyedSingletonCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _keyedSingletonCache.Count;
                }
            }
        }

        /// <summary>
        /// 获取可释放对象数量
        /// <para>Get the count of disposable objects</para>
        /// </summary>
        public int DisposableCount => _disposableTracker.Count;

        /// <summary>
        /// 获取实例创建总数
        /// <para>Get the total count of instance creations</para>
        /// </summary>
        public int InstanceCreationCount => _instanceCreationCount;

        /// <summary>
        /// 获取是否已释放
        /// <para>Get whether the manager has been disposed</para>
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 获取生命周期验证器
        /// <para>Get the lifetime validator</para>
        /// </summary>
        public LifetimeValidator Validator => _lifetimeValidator;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建生命周期管理器实例
        /// </summary>
        /// <param name="enableDiagnostics">是否启用诊断 / Whether to enable diagnostics</param>
        /// <param name="enableLifetimeValidation">是否启用生命周期验证 / Whether to enable lifetime validation</param>
        public LifetimeManager(bool enableDiagnostics = false, bool enableLifetimeValidation = true)
        {
            _enableDiagnostics = enableDiagnostics;
            _singletonCache = new Dictionary<Type, object>();
            _keyedSingletonCache = new Dictionary<(Type, object), object>();
            _disposableTracker = new DisposableTracker(enableDiagnostics);
            _lifetimeValidator = new LifetimeValidator(enableLifetimeValidation);
        }

        #endregion

        #region 单例管理 / Singleton Management

        /// <summary>
        /// 获取或创建单例实例
        /// <para>Get or create a singleton instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>单例实例 / Singleton instance</returns>
        /// <exception cref="ObjectDisposedException">当管理器已释放时抛出</exception>
        public object GetOrCreateSingleton(Type serviceType, Func<object> factory)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            // 第一次检查（无锁）
            if (_singletonCache.TryGetValue(serviceType, out var existing))
            {
                return existing;
            }

            lock (_syncRoot)
            {
                // 第二次检查（有锁）
                if (_singletonCache.TryGetValue(serviceType, out existing))
                {
                    return existing;
                }

                // 创建实例
                var instance = factory();
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"工厂函数为类型 '{serviceType.FullName}' 返回了 null。\n" +
                        $"Factory returned null for type '{serviceType.FullName}'.");
                }

                // 缓存实例
                _singletonCache[serviceType] = instance;

                // 跟踪可释放对象
                _disposableTracker.Track(instance, Lifetime.Singleton);

                // 增加计数
                Interlocked.Increment(ref _instanceCreationCount);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"创建单例实例: {serviceType.Name}");
                }

                return instance;
            }
        }

        /// <summary>
        /// 获取或创建键值单例实例
        /// <para>Get or create a keyed singleton instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>单例实例 / Singleton instance</returns>
        public object GetOrCreateKeyedSingleton(Type serviceType, object key, Func<object> factory)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var cacheKey = (serviceType, key);

            // 第一次检查（无锁）
            if (_keyedSingletonCache.TryGetValue(cacheKey, out var existing))
            {
                return existing;
            }

            lock (_syncRoot)
            {
                // 第二次检查（有锁）
                if (_keyedSingletonCache.TryGetValue(cacheKey, out existing))
                {
                    return existing;
                }

                // 创建实例
                var instance = factory();
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"工厂函数为类型 '{serviceType.FullName}' (键值: '{key}') 返回了 null。\n" +
                        $"Factory returned null for type '{serviceType.FullName}' (key: '{key}').");
                }

                // 缓存实例
                _keyedSingletonCache[cacheKey] = instance;

                // 跟踪可释放对象
                _disposableTracker.Track(instance, Lifetime.Singleton);

                // 增加计数
                Interlocked.Increment(ref _instanceCreationCount);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"创建键值单例实例: {serviceType.Name} (Key: {key})");
                }

                return instance;
            }
        }

        /// <summary>
        /// 尝试获取单例实例
        /// <para>Try to get a singleton instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="instance">输出的实例 / Output instance</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public bool TryGetSingleton(Type serviceType, out object instance)
        {
            if (_isDisposed || serviceType == null)
            {
                instance = null;
                return false;
            }

            lock (_syncRoot)
            {
                return _singletonCache.TryGetValue(serviceType, out instance);
            }
        }

        /// <summary>
        /// 尝试获取键值单例实例
        /// <para>Try to get a keyed singleton instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <param name="instance">输出的实例 / Output instance</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public bool TryGetKeyedSingleton(Type serviceType, object key, out object instance)
        {
            if (_isDisposed || serviceType == null || key == null)
            {
                instance = null;
                return false;
            }

            lock (_syncRoot)
            {
                return _keyedSingletonCache.TryGetValue((serviceType, key), out instance);
            }
        }

        /// <summary>
        /// 检查是否存在单例实例
        /// <para>Check if a singleton instance exists</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasSingleton(Type serviceType)
        {
            if (_isDisposed || serviceType == null)
                return false;

            lock (_syncRoot)
            {
                return _singletonCache.ContainsKey(serviceType);
            }
        }

        #endregion

        #region 瞬态管理 / Transient Management

        /// <summary>
        /// 创建瞬态实例
        /// <para>Create a transient instance</para>
        /// </summary>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <param name="trackDisposable">是否跟踪可释放对象 / Whether to track disposable</param>
        /// <returns>新创建的实例 / Newly created instance</returns>
        /// <remarks>
        /// 瞬态实例默认不被容器跟踪，调用者负责释放。
        /// 如果需要容器跟踪，设置 trackDisposable 为 true。
        /// </remarks>
        public object CreateTransient(Func<object> factory, bool trackDisposable = false)
        {
            ThrowIfDisposed();

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var instance = factory();

            // 增加计数
            Interlocked.Increment(ref _instanceCreationCount);

            // 可选跟踪
            if (trackDisposable)
            {
                _disposableTracker.Track(instance, Lifetime.Transient);
            }

            return instance;
        }

        #endregion

        #region 实例注册 / Instance Registration

        /// <summary>
        /// 注册现有实例为单例
        /// <para>Register an existing instance as singleton</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="instance">实例 / Instance</param>
        /// <param name="ownsInstance">是否拥有实例（负责释放）/ Whether owns the instance</param>
        public void RegisterSingletonInstance(Type serviceType, object instance, bool ownsInstance = true)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_syncRoot)
            {
                if (_singletonCache.ContainsKey(serviceType))
                {
                    throw new InvalidOperationException(
                        $"类型 '{serviceType.FullName}' 的单例实例已存在。\n" +
                        $"Singleton instance for type '{serviceType.FullName}' already exists.");
                }

                _singletonCache[serviceType] = instance;

                if (ownsInstance)
                {
                    _disposableTracker.Track(instance, Lifetime.Singleton);
                }
            }
        }

        #endregion

        #region 生命周期验证 / Lifetime Validation

        /// <summary>
        /// 验证依赖的生命周期
        /// <para>Validate dependency lifetime</para>
        /// </summary>
        /// <param name="consumerLifetime">消费者生命周期 / Consumer lifetime</param>
        /// <param name="dependencyLifetime">依赖生命周期 / Dependency lifetime</param>
        /// <param name="consumerType">消费者类型 / Consumer type</param>
        /// <param name="dependencyType">依赖类型 / Dependency type</param>
        public void ValidateDependencyLifetime(
            Lifetime consumerLifetime,
            Lifetime dependencyLifetime,
            Type consumerType,
            Type dependencyType)
        {
            _lifetimeValidator.ValidateDependency(
                consumerLifetime, dependencyLifetime, consumerType, dependencyType);
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放生命周期管理器及其管理的所有实例
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"开始释放生命周期管理器，共 {_disposableTracker.Count} 个可释放对象");
                }

                // 释放所有跟踪的对象
                _disposableTracker.DisposeAll();

                // 清空缓存
                _singletonCache.Clear();
                _keyedSingletonCache.Clear();

                if (_enableDiagnostics)
                {
                    LogDiagnostic("生命周期管理器已释放");
                }
            }
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取诊断信息
        /// <para>Get diagnostic information</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        public string GetDiagnosticInfo()
        {
            return $"LifetimeManager[" +
                   $"Singletons={SingletonCount}, " +
                   $"KeyedSingletons={KeyedSingletonCount}, " +
                   $"Disposables={DisposableCount}, " +
                   $"TotalCreated={_instanceCreationCount}]";
        }

        /// <summary>
        /// 获取所有单例类型
        /// <para>Get all singleton types</para>
        /// </summary>
        /// <returns>单例类型集合 / Collection of singleton types</returns>
        public IReadOnlyCollection<Type> GetSingletonTypes()
        {
            lock (_syncRoot)
            {
                return new List<Type>(_singletonCache.Keys);
            }
        }

        private void LogDiagnostic(string message)
        {
            if (_enableDiagnostics)
            {
                UnityEngine.Debug.Log($"[AFramework.DI.Lifecycle] {message}");
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(LifetimeManager),
                    "生命周期管理器已被释放，无法执行操作。\n" +
                    "Lifetime manager has been disposed, cannot perform operation.");
            }
        }

        #endregion
    }
}
