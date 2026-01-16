// ==========================================================
// 文件名：InstanceProviderBase.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 实例提供者抽象基类，定义通用的实例创建和管理逻辑
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 实例提供者抽象基类
    /// <para>提供实例创建和管理的通用实现，子类只需实现特定的生命周期策略</para>
    /// <para>Abstract base class for instance providers with common implementation</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>模板方法模式：定义实例获取的算法骨架，子类实现具体策略</item>
    /// <item>单一职责：基类处理通用逻辑，子类专注于生命周期管理</item>
    /// <item>开闭原则：通过继承扩展新的生命周期策略</item>
    /// </list>
    /// </remarks>
    public abstract class InstanceProviderBase : IInstanceProvider
    {
        #region 字段 / Fields

        /// <summary>
        /// 实例类型
        /// </summary>
        protected readonly Type _instanceType;

        /// <summary>
        /// 注入器实例
        /// </summary>
        protected readonly IInjector _injector;

        /// <summary>
        /// 注册时的参数列表
        /// </summary>
        protected readonly IReadOnlyList<IInjectParameter> _registrationParameters;

        /// <summary>
        /// 激活回调列表
        /// </summary>
        protected readonly IReadOnlyList<Action<object>> _onActivatedCallbacks;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        protected readonly object _syncRoot = new object();

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public abstract Lifetime Lifetime { get; }

        /// <inheritdoc/>
        public Type InstanceType => _instanceType;

        /// <inheritdoc/>
        public abstract bool HasInstance { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建实例提供者
        /// </summary>
        /// <param name="instanceType">实例类型 / Instance type</param>
        /// <param name="injector">注入器 / Injector (可选，默认使用全局注入器)</param>
        /// <param name="parameters">注册参数 / Registration parameters</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        /// <exception cref="ArgumentNullException">当 instanceType 为 null 时抛出</exception>
        protected InstanceProviderBase(
            Type instanceType,
            IInjector injector = null,
            IReadOnlyList<IInjectParameter> parameters = null,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
        {
            _instanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
            _injector = injector ?? Injector.Instance;
            _registrationParameters = parameters ?? Array.Empty<IInjectParameter>();
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
        public abstract object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters);

        /// <inheritdoc/>
        public virtual void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    // 记录异常但不抛出，确保释放流程继续
                    UnityEngine.Debug.LogWarning(
                        $"[AFramework.DI] 释放实例 {instance.GetType().Name} 时发生异常: {ex.Message}\n" +
                        $"Exception disposing instance {instance.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <inheritdoc/>
        public virtual string GetDiagnosticInfo()
        {
            return $"{GetType().Name}[Type={_instanceType.Name}, Lifetime={Lifetime}, HasInstance={HasInstance}]";
        }

        #endregion

        #region 受保护方法 / Protected Methods

        /// <summary>
        /// 创建新实例
        /// <para>Create a new instance using the injector</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="runtimeParameters">运行时参数 / Runtime parameters</param>
        /// <returns>新创建的实例 / Newly created instance</returns>
        protected virtual object CreateNewInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> runtimeParameters)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // 合并参数：运行时参数优先于注册参数
            var mergedParameters = MergeParameters(runtimeParameters);

            // 使用注入器创建实例
            var instance = _injector.CreateInstance(_instanceType, resolver, mergedParameters);

            // 执行激活回调
            InvokeActivationCallbacks(instance);

            return instance;
        }

        /// <summary>
        /// 合并参数列表
        /// <para>Merge runtime parameters with registration parameters</para>
        /// </summary>
        /// <param name="runtimeParameters">运行时参数 / Runtime parameters</param>
        /// <returns>合并后的参数列表 / Merged parameter list</returns>
        protected IReadOnlyList<IInjectParameter> MergeParameters(IReadOnlyList<IInjectParameter> runtimeParameters)
        {
            // 如果没有注册参数，直接返回运行时参数
            if (_registrationParameters == null || _registrationParameters.Count == 0)
            {
                return runtimeParameters;
            }

            // 如果没有运行时参数，直接返回注册参数
            if (runtimeParameters == null || runtimeParameters.Count == 0)
            {
                return _registrationParameters;
            }

            // 合并两个列表，运行时参数优先
            var merged = new List<IInjectParameter>(runtimeParameters.Count + _registrationParameters.Count);
            merged.AddRange(runtimeParameters);
            merged.AddRange(_registrationParameters);
            return merged;
        }

        /// <summary>
        /// 执行激活回调
        /// <para>Invoke activation callbacks on the instance</para>
        /// </summary>
        /// <param name="instance">实例 / Instance</param>
        protected void InvokeActivationCallbacks(object instance)
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

        /// <summary>
        /// 验证解析器参数
        /// <para>Validate the resolver parameter</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <exception cref="ArgumentNullException">当 resolver 为 null 时抛出</exception>
        protected void ValidateResolver(IObjectResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver),
                    "对象解析器不能为空。\nObject resolver cannot be null.");
            }
        }

        #endregion
    }
}
