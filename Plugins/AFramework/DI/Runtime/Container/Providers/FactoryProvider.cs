// ==========================================================
// 文件名：FactoryProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Threading
// 功能: 工厂实例提供者，使用工厂函数创建实例
// ==========================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace AFramework.DI
{
    /// <summary>
    /// 工厂实例提供者
    /// <para>使用用户提供的工厂函数创建实例，支持自定义创建逻辑</para>
    /// <para>Factory instance provider that uses user-provided factory function to create instances</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>灵活创建：支持复杂的实例创建逻辑</item>
    /// <item>生命周期可配置：可与任意生命周期组合使用</item>
    /// <item>延迟解析：工厂函数在首次请求时才执行</item>
    /// </list>
    /// 
    /// 适用场景：
    /// <list type="bullet">
    /// <item>需要复杂初始化逻辑的服务</item>
    /// <item>需要根据运行时条件创建不同实例</item>
    /// <item>集成第三方库创建的对象</item>
    /// </list>
    /// </remarks>
    public sealed class FactoryProvider : IInstanceProvider
    {
        #region 字段 / Fields

        private readonly Func<IObjectResolver, object> _factory;
        private readonly Type _instanceType;
        private readonly Lifetime _lifetime;
        private readonly IReadOnlyList<Action<object>> _onActivatedCallbacks;
        private readonly object _syncRoot = new object();

        // 单例/作用域缓存
        private object _cachedInstance;
        private volatile bool _isCreated;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public Lifetime Lifetime => _lifetime;

        /// <inheritdoc/>
        public Type InstanceType => _instanceType;

        /// <inheritdoc/>
        public bool HasInstance => _isCreated;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建工厂提供者
        /// </summary>
        /// <param name="factory">工厂函数 / Factory function</param>
        /// <param name="instanceType">实例类型 / Instance type</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        /// <exception cref="ArgumentNullException">当 factory 为 null 时抛出</exception>
        public FactoryProvider(
            Func<IObjectResolver, object> factory,
            Type instanceType,
            Lifetime lifetime = Lifetime.Transient,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _instanceType = instanceType ?? typeof(object);
            _lifetime = lifetime;
            _onActivatedCallbacks = onActivatedCallbacks ?? Array.Empty<Action<object>>();
        }

        #endregion

        #region IInstanceProvider 实现 / IInstanceProvider Implementation

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver)
        {
            return GetInstance(resolver, null);
        }

        /// <inheritdoc/>
        public object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // 瞬态：每次创建新实例
            if (_lifetime == Lifetime.Transient)
            {
                return CreateAndActivate(resolver);
            }

            // 单例/作用域：使用缓存
            if (_isCreated)
            {
                return _cachedInstance;
            }

            lock (_syncRoot)
            {
                if (_isCreated)
                {
                    return _cachedInstance;
                }

                _cachedInstance = CreateAndActivate(resolver);
                Thread.MemoryBarrier();
                _isCreated = true;

                return _cachedInstance;
            }
        }

        /// <inheritdoc/>
        public void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning(
                        $"[AFramework.DI] 释放工厂实例时发生异常: {ex.Message}\n" +
                        $"Exception disposing factory instance: {ex.Message}");
                }
            }

            lock (_syncRoot)
            {
                if (ReferenceEquals(_cachedInstance, instance))
                {
                    _cachedInstance = null;
                    _isCreated = false;
                }
            }
        }

        /// <inheritdoc/>
        public string GetDiagnosticInfo()
        {
            return $"FactoryProvider[Type={_instanceType.Name}, Lifetime={_lifetime}, HasInstance={_isCreated}]";
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建实例并执行激活回调
        /// </summary>
        private object CreateAndActivate(IObjectResolver resolver)
        {
            object instance;
            
            try
            {
                instance = _factory(resolver);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(
                    $"工厂函数创建类型 {_instanceType.Name} 的实例时发生错误。\n" +
                    $"Factory function failed to create instance of type {_instanceType.Name}.",
                    ex);
            }

            if (instance == null)
            {
                throw new ResolutionException(
                    $"工厂函数返回了 null，无法创建类型 {_instanceType.Name} 的实例。\n" +
                    $"Factory function returned null for type {_instanceType.Name}.");
            }

            // 执行激活回调
            InvokeActivationCallbacks(instance);

            return instance;
        }

        /// <summary>
        /// 执行激活回调
        /// </summary>
        private void InvokeActivationCallbacks(object instance)
        {
            if (_onActivatedCallbacks == null || _onActivatedCallbacks.Count == 0)
                return;

            foreach (var callback in _onActivatedCallbacks)
            {
                try
                {
                    callback?.Invoke(instance);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(
                        $"[AFramework.DI] 执行激活回调时发生异常: {ex.Message}\n" +
                        $"Exception in activation callback: {ex.Message}");
                    throw;
                }
            }
        }

        #endregion
    }
}
