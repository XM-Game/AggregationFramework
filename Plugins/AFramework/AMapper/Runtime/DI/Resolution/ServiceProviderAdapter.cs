// ==========================================================
// 文件名：ServiceProviderAdapter.cs
// 命名空间: AFramework.AMapper.DI
// 依赖: System, AFramework.DI
// 功能: 服务提供者适配器，将 IObjectResolver 适配为 IServiceProvider
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.AMapper.DI
{
    /// <summary>
    /// 服务提供者适配器
    /// <para>将 IObjectResolver 适配为 IServiceProvider，用于与标准 .NET 服务提供者集成</para>
    /// <para>Service provider adapter that adapts IObjectResolver to IServiceProvider</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>适配器模式：将 IObjectResolver 接口适配为 IServiceProvider</item>
    /// <item>单一职责：仅负责接口适配，不包含业务逻辑</item>
    /// <item>依赖倒置：依赖抽象接口而非具体实现</item>
    /// </list>
    /// 
    /// 使用场景：
    /// <list type="bullet">
    /// <item>与 ASP.NET Core 等框架集成</item>
    /// <item>使用标准 IServiceProvider 接口的第三方库</item>
    /// <item>需要统一服务提供者接口的场景</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// var resolver = builder.Build();
    /// IServiceProvider serviceProvider = new ServiceProviderAdapter(resolver);
    /// 
    /// // 使用标准 IServiceProvider 接口
    /// var service = serviceProvider.GetService(typeof(IMyService));
    /// </code>
    /// </remarks>
    public sealed class ServiceProviderAdapter : IServiceProvider, IDisposable
    {
        #region 私有字段 / Private Fields

        private readonly IObjectResolver _resolver;
        private bool _disposed;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建服务提供者适配器
        /// <para>Create service provider adapter</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <exception cref="ArgumentNullException">当 resolver 为 null 时抛出</exception>
        public ServiceProviderAdapter(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IServiceProvider 实现 / IServiceProvider Implementation

        /// <summary>
        /// 获取指定类型的服务
        /// <para>Get service of specified type</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>服务实例或 null / Service instance or null</returns>
        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();

            if (serviceType == null)
                return null;

            // 尝试解析服务
            if (_resolver.TryResolve(serviceType, out var instance))
            {
                return instance;
            }

            // 如果解析失败，返回 null（符合 IServiceProvider 约定）
            return null;
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放资源
        /// <para>Dispose resources</para>
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _resolver?.Dispose();
            _disposed = true;
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 检查是否已释放
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType().FullName,
                    "服务提供者适配器已释放 / Service provider adapter has been disposed");
            }
        }

        #endregion

        #region 公共属性 / Public Properties

        /// <summary>
        /// 获取底层的对象解析器
        /// <para>Get underlying object resolver</para>
        /// </summary>
        public IObjectResolver Resolver
        {
            get
            {
                ThrowIfDisposed();
                return _resolver;
            }
        }

        #endregion
    }
}
