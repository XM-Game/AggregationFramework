// ==========================================================
// 文件名：ScopedProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 作用域实例提供者，在作用域内共享同一实例
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 作用域实例提供者
    /// <para>在同一作用域内共享实例，不同作用域拥有独立实例</para>
    /// <para>Scoped instance provider that shares instance within the same scope</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>作用域隔离：每个作用域拥有独立的实例</item>
    /// <item>线程安全：使用 ConditionalWeakTable 避免内存泄漏</item>
    /// <item>自动释放：作用域销毁时实例自动释放</item>
    /// </list>
    /// 
    /// 适用场景：
    /// <list type="bullet">
    /// <item>场景级服务（场景切换时需要重置）</item>
    /// <item>请求级服务（每个请求独立状态）</item>
    /// <item>UI 管理器（每个 UI 层级独立）</item>
    /// </list>
    /// </remarks>
    public sealed class ScopedProvider : InstanceProviderBase
    {
        #region 字段 / Fields

        /// <summary>
        /// 作用域实例缓存
        /// <para>使用 ConditionalWeakTable 确保作用域销毁时实例可被 GC 回收</para>
        /// </summary>
        private readonly ConditionalWeakTable<IObjectResolver, object> _scopedInstances;

        /// <summary>
        /// 用于跟踪是否有任何实例被创建
        /// </summary>
        private volatile bool _hasAnyInstance;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public override Lifetime Lifetime => Lifetime.Scoped;

        /// <inheritdoc/>
        public override bool HasInstance => _hasAnyInstance;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建作用域提供者
        /// </summary>
        /// <param name="instanceType">实例类型 / Instance type</param>
        /// <param name="injector">注入器 / Injector</param>
        /// <param name="parameters">注册参数 / Registration parameters</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        public ScopedProvider(
            Type instanceType,
            IInjector injector = null,
            IReadOnlyList<IInjectParameter> parameters = null,
            IReadOnlyList<Action<object>> onActivatedCallbacks = null)
            : base(instanceType, injector, parameters, onActivatedCallbacks)
        {
            _scopedInstances = new ConditionalWeakTable<IObjectResolver, object>();
        }

        #endregion

        #region IInstanceProvider 实现 / IInstanceProvider Implementation

        /// <inheritdoc/>
        public override object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            ValidateResolver(resolver);

            // 尝试从缓存获取
            if (_scopedInstances.TryGetValue(resolver, out var existing))
            {
                return existing;
            }

            // 加锁创建新实例
            lock (_syncRoot)
            {
                // 双重检查
                if (_scopedInstances.TryGetValue(resolver, out existing))
                {
                    return existing;
                }

                // 创建新实例
                var instance = CreateNewInstance(resolver, parameters);
                
                // 缓存到作用域
                _scopedInstances.Add(resolver, instance);
                _hasAnyInstance = true;

                return instance;
            }
        }

        /// <inheritdoc/>
        public override void DisposeInstance(object instance)
        {
            // 作用域实例由作用域容器管理释放
            // 这里仅执行实际的释放逻辑
            base.DisposeInstance(instance);
        }

        /// <inheritdoc/>
        public override string GetDiagnosticInfo()
        {
            return $"ScopedProvider[Type={_instanceType.Name}, HasAnyInstance={_hasAnyInstance}]";
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 移除指定作用域的实例（作用域销毁时调用）
        /// <para>Remove instance for specified scope (called when scope is disposed)</para>
        /// </summary>
        /// <param name="resolver">作用域解析器 / Scope resolver</param>
        internal void RemoveScope(IObjectResolver resolver)
        {
            if (resolver == null) return;

            lock (_syncRoot)
            {
                if (_scopedInstances.TryGetValue(resolver, out var instance))
                {
                    _scopedInstances.Remove(resolver);
                    base.DisposeInstance(instance);
                }
            }
        }

        #endregion
    }
}
