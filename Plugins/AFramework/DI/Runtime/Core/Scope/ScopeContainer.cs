// ==========================================================
// 文件名：ScopeContainer.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 作用域容器，提供独立的作用域实例管理
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 作用域容器
    /// <para>提供独立的作用域实例缓存和生命周期管理，继承父容器的注册但拥有独立的 Scoped 实例</para>
    /// <para>Scope container that provides independent scoped instance caching and lifecycle management</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于作用域级别的实例管理</item>
    /// <item>组合优于继承：通过组合父容器实现功能复用</item>
    /// <item>资源管理：作用域销毁时自动释放所有 Scoped 实例</item>
    /// </list>
    /// 
    /// 核心特性：
    /// <list type="bullet">
    /// <item>独立的 Scoped 实例缓存</item>
    /// <item>共享父容器的 Singleton 实例</item>
    /// <item>支持嵌套作用域</item>
    /// <item>自动资源释放</item>
    /// </list>
    /// </remarks>
    public sealed class ScopeContainer : IObjectResolver
    {
        #region 字段 / Fields

        /// <summary>
        /// 父容器引用
        /// </summary>
        private readonly IObjectResolver _parent;

        /// <summary>
        /// 作用域实例缓存
        /// </summary>
        private readonly ScopeInstanceCache _instanceCache;

        /// <summary>
        /// 可释放对象追踪器
        /// </summary>
        private readonly DisposableTracker _disposableTracker;

        /// <summary>
        /// 作用域管理器引用（可选）
        /// </summary>
        private readonly ScopeManager _scopeManager;

        /// <summary>
        /// 作用域名称（用于诊断）
        /// </summary>
        private readonly string _scopeName;

        /// <summary>
        /// 作用域ID
        /// </summary>
        private readonly Guid _scopeId;

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
        /// 创建时间
        /// </summary>
        private readonly DateTime _createdAt;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public IObjectResolver Parent => _parent;

        /// <summary>
        /// 获取作用域ID
        /// <para>Get the scope ID</para>
        /// </summary>
        public Guid ScopeId => _scopeId;

        /// <summary>
        /// 获取作用域名称
        /// <para>Get the scope name</para>
        /// </summary>
        public string ScopeName => _scopeName;

        /// <summary>
        /// 获取是否已释放
        /// <para>Get whether the scope has been disposed</para>
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 获取作用域实例数量
        /// <para>Get the count of scoped instances</para>
        /// </summary>
        public int ScopedInstanceCount => _instanceCache.Count;

        /// <summary>
        /// 获取创建时间
        /// <para>Get the creation time</para>
        /// </summary>
        public DateTime CreatedAt => _createdAt;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建作用域容器实例
        /// </summary>
        /// <param name="parent">父容器 / Parent container</param>
        /// <param name="scopeName">作用域名称 / Scope name</param>
        /// <param name="scopeManager">作用域管理器 / Scope manager</param>
        /// <param name="enableDiagnostics">是否启用诊断 / Whether to enable diagnostics</param>
        /// <exception cref="ArgumentNullException">当 parent 为 null 时抛出</exception>
        public ScopeContainer(
            IObjectResolver parent,
            string scopeName = null,
            ScopeManager scopeManager = null,
            bool enableDiagnostics = false)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _scopeName = scopeName ?? $"Scope_{Guid.NewGuid():N}".Substring(0, 16);
            _scopeManager = scopeManager;
            _enableDiagnostics = enableDiagnostics;
            _scopeId = Guid.NewGuid();
            _createdAt = DateTime.UtcNow;

            _instanceCache = new ScopeInstanceCache(enableDiagnostics);
            _disposableTracker = new DisposableTracker(enableDiagnostics);

            // 注册自身到作用域管理器
            _scopeManager?.RegisterScope(this);

            if (_enableDiagnostics)
            {
                LogDiagnostic($"作用域容器已创建: {_scopeName} (ID: {_scopeId})");
            }
        }

        #endregion

        #region IObjectResolver 实现 / IObjectResolver Implementation

        /// <inheritdoc/>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <inheritdoc/>
        public object Resolve(Type type)
        {
            ThrowIfDisposed();

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // 委托给父容器解析，父容器会根据生命周期返回正确的实例
            // 对于 Scoped 生命周期，需要在当前作用域缓存
            return _parent.Resolve(type);
        }

        /// <inheritdoc/>
        public T ResolveOrDefault<T>()
        {
            ThrowIfDisposed();
            return _parent.ResolveOrDefault<T>();
        }

        /// <inheritdoc/>
        public object ResolveOrDefault(Type type)
        {
            ThrowIfDisposed();
            return _parent.ResolveOrDefault(type);
        }

        /// <inheritdoc/>
        public bool TryResolve<T>(out T instance)
        {
            if (_isDisposed)
            {
                instance = default;
                return false;
            }
            return _parent.TryResolve(out instance);
        }

        /// <inheritdoc/>
        public bool TryResolve(Type type, out object instance)
        {
            if (_isDisposed)
            {
                instance = null;
                return false;
            }
            return _parent.TryResolve(type, out instance);
        }

        /// <inheritdoc/>
        public IEnumerable<T> ResolveAll<T>()
        {
            ThrowIfDisposed();
            return _parent.ResolveAll<T>();
        }

        /// <inheritdoc/>
        public IEnumerable<object> ResolveAll(Type type)
        {
            ThrowIfDisposed();
            return _parent.ResolveAll(type);
        }

        /// <inheritdoc/>
        public T ResolveKeyed<T>(object key)
        {
            ThrowIfDisposed();
            return _parent.ResolveKeyed<T>(key);
        }

        /// <inheritdoc/>
        public object ResolveKeyed(Type type, object key)
        {
            ThrowIfDisposed();
            return _parent.ResolveKeyed(type, key);
        }

        /// <inheritdoc/>
        public bool TryResolveKeyed<T>(object key, out T instance)
        {
            if (_isDisposed)
            {
                instance = default;
                return false;
            }
            return _parent.TryResolveKeyed(key, out instance);
        }

        /// <inheritdoc/>
        public void Inject(object instance)
        {
            ThrowIfDisposed();
            _parent.Inject(instance);
        }

        /// <inheritdoc/>
        public void Inject(object instance, IReadOnlyList<IInjectParameter> parameters)
        {
            ThrowIfDisposed();
            _parent.Inject(instance, parameters);
        }

        /// <inheritdoc/>
        public T Instantiate<T>()
        {
            ThrowIfDisposed();
            return _parent.Instantiate<T>();
        }

        /// <inheritdoc/>
        public object Instantiate(Type type)
        {
            ThrowIfDisposed();
            return _parent.Instantiate(type);
        }

        /// <inheritdoc/>
        public T Instantiate<T>(IReadOnlyList<IInjectParameter> parameters)
        {
            ThrowIfDisposed();
            return _parent.Instantiate<T>(parameters);
        }

        /// <inheritdoc/>
        public object Instantiate(Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            ThrowIfDisposed();
            return _parent.Instantiate(type, parameters);
        }

        /// <inheritdoc/>
        public IObjectResolver CreateScope()
        {
            return CreateScope(null);
        }

        /// <inheritdoc/>
        public IObjectResolver CreateScope(Action<IContainerBuilder> configuration)
        {
            ThrowIfDisposed();

            // 创建子作用域
            var childScope = new ScopeContainer(
                this,
                $"{_scopeName}_Child",
                _scopeManager,
                _enableDiagnostics);

            if (_enableDiagnostics)
            {
                LogDiagnostic($"创建子作用域: {childScope.ScopeName}");
            }

            return childScope;
        }

        /// <inheritdoc/>
        public bool IsRegistered<T>()
        {
            ThrowIfDisposed();
            return _parent.IsRegistered<T>();
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type type)
        {
            ThrowIfDisposed();
            return _parent.IsRegistered(type);
        }

        /// <inheritdoc/>
        public bool IsRegisteredKeyed<T>(object key)
        {
            ThrowIfDisposed();
            return _parent.IsRegisteredKeyed<T>(key);
        }

        #endregion

        #region 作用域实例管理 / Scoped Instance Management

        /// <summary>
        /// 获取或创建作用域实例
        /// <para>Get or create a scoped instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>作用域实例 / Scoped instance</returns>
        public object GetOrCreateScopedInstance(Type serviceType, Func<object> factory)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return _instanceCache.GetOrCreate(serviceType, () =>
            {
                var instance = factory();
                
                // 跟踪可释放对象
                _disposableTracker.Track(instance, Lifetime.Scoped);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"创建作用域实例: {serviceType.Name}");
                }

                return instance;
            });
        }

        /// <summary>
        /// 获取或创建键值作用域实例
        /// <para>Get or create a keyed scoped instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>作用域实例 / Scoped instance</returns>
        public object GetOrCreateKeyedScopedInstance(Type serviceType, object key, Func<object> factory)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return _instanceCache.GetOrCreateKeyed(serviceType, key, () =>
            {
                var instance = factory();
                
                // 跟踪可释放对象
                _disposableTracker.Track(instance, Lifetime.Scoped);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"创建键值作用域实例: {serviceType.Name} (Key: {key})");
                }

                return instance;
            });
        }

        /// <summary>
        /// 检查是否存在作用域实例
        /// <para>Check if a scoped instance exists</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasScopedInstance(Type serviceType)
        {
            if (_isDisposed || serviceType == null)
                return false;

            return _instanceCache.Contains(serviceType);
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放作用域容器及其管理的所有实例
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
                    var lifetime = DateTime.UtcNow - _createdAt;
                    LogDiagnostic($"开始释放作用域: {_scopeName}, 生存时间: {lifetime.TotalSeconds:F2}s");
                }

                // 从作用域管理器注销
                _scopeManager?.UnregisterScope(this);

                // 释放所有跟踪的对象
                _disposableTracker.Dispose();

                // 清空实例缓存
                _instanceCache.Clear();

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"作用域已释放: {_scopeName}");
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
            var lifetime = DateTime.UtcNow - _createdAt;
            return $"ScopeContainer[Name={_scopeName}, ID={_scopeId:N}, " +
                   $"Instances={_instanceCache.Count}, " +
                   $"Disposables={_disposableTracker.Count}, " +
                   $"Lifetime={lifetime.TotalSeconds:F2}s, " +
                   $"Disposed={_isDisposed}]";
        }

        private void LogDiagnostic(string message)
        {
            if (_enableDiagnostics)
            {
                UnityEngine.Debug.Log($"[AFramework.DI.Scope] {message}");
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(_scopeName,
                    $"作用域 '{_scopeName}' 已被释放，无法执行操作。\n" +
                    $"Scope '{_scopeName}' has been disposed, cannot perform operation.");
            }
        }

        #endregion
    }
}
