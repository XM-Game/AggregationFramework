// ==========================================================
// 文件名：TransientProvider.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 瞬态实例提供者，每次请求创建新实例
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 瞬态实例提供者
    /// <para>每次解析请求都创建新的实例，容器不跟踪实例生命周期</para>
    /// <para>Transient instance provider that creates a new instance for each resolution request</para>
    /// </summary>
    /// <remarks>
    /// 特性：
    /// <list type="bullet">
    /// <item>无缓存：每次请求都创建新实例</item>
    /// <item>无跟踪：容器不管理实例生命周期</item>
    /// <item>调用者负责：使用者需要自行管理实例的释放</item>
    /// </list>
    /// 
    /// 适用场景：
    /// <list type="bullet">
    /// <item>轻量级无状态对象</item>
    /// <item>命令对象、DTO</item>
    /// <item>每次需要全新状态的服务</item>
    /// </list>
    /// 
    /// 注意事项：
    /// <list type="bullet">
    /// <item>Singleton/Scoped 服务不应直接依赖 Transient 服务</item>
    /// <item>如需在长生命周期服务中使用，应通过 Func&lt;T&gt; 或工厂</item>
    /// </list>
    /// </remarks>
    public sealed class TransientProvider : InstanceProviderBase
    {
        #region 字段 / Fields

        /// <summary>
        /// 创建的实例计数（仅用于诊断）
        /// </summary>
        private int _instanceCount;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public override Lifetime Lifetime => Lifetime.Transient;

        /// <inheritdoc/>
        /// <remarks>
        /// 瞬态提供者不缓存实例，始终返回 false
        /// </remarks>
        public override bool HasInstance => false;

        /// <summary>
        /// 获取已创建的实例数量（仅用于诊断）
        /// <para>Get the count of created instances (for diagnostics only)</para>
        /// </summary>
        public int InstanceCount => _instanceCount;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建瞬态提供者
        /// </summary>
        /// <param name="instanceType">实例类型 / Instance type</param>
        /// <param name="injector">注入器 / Injector</param>
        /// <param name="parameters">注册参数 / Registration parameters</param>
        /// <param name="onActivatedCallbacks">激活回调 / Activation callbacks</param>
        public TransientProvider(
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
        /// 每次调用都创建新实例，不进行任何缓存
        /// </remarks>
        public override object GetInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
        {
            ValidateResolver(resolver);

            var instance = CreateNewInstance(resolver, parameters);
            
            // 增加计数（线程安全）
            System.Threading.Interlocked.Increment(ref _instanceCount);

            return instance;
        }

        /// <inheritdoc/>
        public override string GetDiagnosticInfo()
        {
            return $"TransientProvider[Type={_instanceType.Name}, CreatedCount={_instanceCount}]";
        }

        #endregion
    }
}
