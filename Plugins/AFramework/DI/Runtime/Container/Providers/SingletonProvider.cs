// ==========================================================
// 文件名：SingletonProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Threading
// 功能: 单例实例提供者，确保容器生命周期内只创建一个实例
// ==========================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace AFramework.DI
{
    /// <summary>
    /// 单例实例提供者
    /// <para>确保在容器的整个生命周期内只创建一个实例，所有解析请求返回同一实例</para>
    /// <para>Singleton instance provider that ensures only one instance is created during container lifetime</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>线程安全：使用双重检查锁定模式确保线程安全的实例创建</item>
    /// <item>延迟初始化：首次请求时才创建实例</item>
    /// <item>自动释放：容器销毁时自动释放实例（如果实现了 IDisposable）</item>
    /// </list>
    /// 
    /// 适用场景：
    /// <list type="bullet">
    /// <item>配置服务、日志服务等全局共享服务</item>
    /// <item>初始化成本高的服务</item>
    /// <item>需要维护全局状态的服务</item>
    /// </list>
    /// </remarks>
    public sealed class SingletonProvider : InstanceProviderBase
    {
        #region 字段 / Fields

        /// <summary>
        /// 缓存的单例实例
        /// </summary>
        private object _instance;

        /// <summary>
        /// 实例是否已创建的标志
        /// </summary>
        private volatile bool _isCreated;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public override Lifetime Lifetime => Lifetime.Singleton;

        /// <inheritdoc/>
        public override bool HasInstance => _isCreated;

        /// <summary>
        /// 获取缓存的实例（仅用于诊断）
        /// <para>Get the cached instance (for diagnostics only)</para>
        /// </summary>
        internal object CachedInstance => _instance;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建单例提供者
        /// </summary>
        /// <param name="instanceType">实例类型 / Instance type</param>
        /// <param name="injector">注入器 / Injector</param>
        /// <param name="parameters">注册参数 / Registration parameters</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        public SingletonProvider(
            Type instanceType,
            IInjector injector = null,
            IReadOnlyList<IInjectParameter> parameters = null,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
            : base(instanceType, injector, parameters, onActivatedCallbacks)
        {
        }

        #endregion

        #region IInstanceProvider 实现 / IInstanceProvider Implementation

        /// <inheritdoc/>
        /// <remarks>
        /// 使用双重检查锁定模式（Double-Checked Locking）确保：
        /// <list type="bullet">
        /// <item>线程安全的实例创建</item>
        /// <item>最小化锁竞争</item>
        /// <item>只创建一个实例</item>
        /// </list>
        /// </remarks>
        public override object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            ValidateResolver(resolver);

            // 第一次检查（无锁）
            if (_isCreated)
            {
                return _instance;
            }

            // 加锁创建实例
            lock (_syncRoot)
            {
                // 第二次检查（有锁）
                if (_isCreated)
                {
                    return _instance;
                }

                // 创建实例
                _instance = CreateNewInstance(resolver, parameters);
                
                // 使用内存屏障确保实例完全初始化后才设置标志
                Thread.MemoryBarrier();
                _isCreated = true;

                return _instance;
            }
        }

        /// <inheritdoc/>
        public override void DisposeInstance(object instance)
        {
            lock (_syncRoot)
            {
                if (_instance != null && ReferenceEquals(_instance, instance))
                {
                    base.DisposeInstance(_instance);
                    _instance = null;
                    _isCreated = false;
                }
            }
        }

        /// <inheritdoc/>
        public override string GetDiagnosticInfo()
        {
            var instanceInfo = _isCreated 
                ? $", Instance={_instance?.GetType().Name ?? "null"}" 
                : ", Instance=NotCreated";
            return $"SingletonProvider[Type={_instanceType.Name}{instanceInfo}]";
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 重置提供者状态（仅用于测试）
        /// <para>Reset provider state (for testing only)</para>
        /// </summary>
        internal void Reset()
        {
            lock (_syncRoot)
            {
                if (_instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _instance = null;
                _isCreated = false;
            }
        }

        #endregion
    }
}
