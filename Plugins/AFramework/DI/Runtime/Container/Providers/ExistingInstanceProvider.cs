// ==========================================================
// 文件名：ExistingInstanceProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 现有实例提供者，返回预先创建的实例
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 现有实例提供者
    /// <para>返回用户预先创建的实例，不负责实例的创建</para>
    /// <para>Existing instance provider that returns a pre-created instance</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>预设实例：使用用户提供的现有实例</item>
    /// <item>单例语义：始终返回同一实例</item>
    /// <item>可选释放：可配置是否由容器管理实例释放</item>
    /// </list>
    /// 
    /// 适用场景：
    /// <list type="bullet">
    /// <item>注册已存在的对象（如 MonoBehaviour）</item>
    /// <item>注册配置对象或外部创建的服务</item>
    /// <item>测试时注入 Mock 对象</item>
    /// </list>
    /// </remarks>
    public sealed class ExistingInstanceProvider : IInstanceProvider
    {
        #region 字段 / Fields

        private readonly object _instance;
        private readonly Type _instanceType;
        private readonly bool _ownsInstance;
        private readonly IReadOnlyList<Action<object>> _onActivatedCallbacks;
        private bool _isActivated;
        private readonly object _syncRoot = new object();

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        /// <remarks>
        /// 现有实例始终被视为单例
        /// </remarks>
        public Lifetime Lifetime => Lifetime.Singleton;

        /// <inheritdoc/>
        public Type InstanceType => _instanceType;

        /// <inheritdoc/>
        public bool HasInstance => _instance != null;

        /// <summary>
        /// 获取是否由容器管理实例释放
        /// <para>Get whether the container manages instance disposal</para>
        /// </summary>
        public bool OwnsInstance => _ownsInstance;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建现有实例提供者
        /// </summary>
        /// <param name="instance">现有实例 / Existing instance</param>
        /// <param name="ownsInstance">是否由容器管理释放 / Whether container manages disposal</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        /// <exception cref="ArgumentNullException">当 instance 为 null 时抛出</exception>
        public ExistingInstanceProvider(
            object instance,
            bool ownsInstance = false,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instanceType = instance.GetType();
            _ownsInstance = ownsInstance;
            _onActivatedCallbacks = onActivatedCallbacks ?? Array.Empty<Action<object>>();
        }

        /// <summary>
        /// 创建现有实例提供者（指定服务类型）
        /// </summary>
        /// <param name="instance">现有实例 / Existing instance</param>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="ownsInstance">是否由容器管理释放 / Whether container manages disposal</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        public ExistingInstanceProvider(
            object instance,
            Type serviceType,
            bool ownsInstance = false,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _instanceType = serviceType ?? instance.GetType();
            _ownsInstance = ownsInstance;
            _onActivatedCallbacks = onActivatedCallbacks ?? Array.Empty<Action<object>>();

            // 验证实例类型兼容性
            if (!_instanceType.IsInstanceOfType(instance))
            {
                throw new ArgumentException(
                    $"实例类型 {instance.GetType().Name} 与服务类型 {_instanceType.Name} 不兼容。\n" +
                    $"Instance type {instance.GetType().Name} is not compatible with service type {_instanceType.Name}.",
                    nameof(instance));
            }
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
            // 首次获取时执行激活回调
            if (!_isActivated && _onActivatedCallbacks.Count > 0)
            {
                lock (_syncRoot)
                {
                    if (!_isActivated)
                    {
                        InvokeActivationCallbacks();
                        _isActivated = true;
                    }
                }
            }

            return _instance;
        }

        /// <inheritdoc/>
        public void DisposeInstance(object instance)
        {
            // 仅当容器拥有实例时才释放
            if (!_ownsInstance)
                return;

            if (!ReferenceEquals(_instance, instance))
                return;

            if (_instance is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning(
                        $"[AFramework.DI] 释放现有实例时发生异常: {ex.Message}\n" +
                        $"Exception disposing existing instance: {ex.Message}");
                }
            }
        }

        /// <inheritdoc/>
        public string GetDiagnosticInfo()
        {
            return $"ExistingInstanceProvider[Type={_instanceType.Name}, OwnsInstance={_ownsInstance}]";
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 执行激活回调
        /// </summary>
        private void InvokeActivationCallbacks()
        {
            foreach (var callback in _onActivatedCallbacks)
            {
                try
                {
                    callback?.Invoke(_instance);
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
