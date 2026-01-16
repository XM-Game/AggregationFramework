// ==========================================================
// 文件名：LifetimeScope.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, AFramework.DI
// ==========================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 生命周期作用域
    /// <para>Unity MonoBehaviour 组件，用于管理依赖注入容器的生命周期</para>
    /// <para>Lifetime scope component for managing DI container lifecycle in Unity</para>
    /// </summary>
    /// <remarks>
    /// 使用方式：
    /// <list type="bullet">
    /// <item>继承此类并重写 Configure 方法来注册服务</item>
    /// <item>支持父子容器层级关系</item>
    /// <item>自动管理容器的创建和销毁</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class GameLifetimeScope : LifetimeScope
    /// {
    ///     protected override void Configure(IContainerBuilder builder)
    ///     {
    ///         builder.Register&lt;IGameService, GameService&gt;(Lifetime.Singleton);
    ///         builder.Register&lt;IPlayerService, PlayerService&gt;(Lifetime.Scoped);
    ///     }
    /// }
    /// </code>
    /// </example>
    [DefaultExecutionOrder(-5000)]
    public class LifetimeScope : MonoBehaviour, IDisposable
    {
        #region 字段

        /// <summary>
        /// 父作用域引用
        /// </summary>
        [SerializeField]
        private ParentReference _parentReference;

        /// <summary>
        /// 是否自动运行
        /// </summary>
        [SerializeField]
        private bool _autoRun = true;

        /// <summary>
        /// 安装器列表
        /// </summary>
        [SerializeField]
        private List<MonoBehaviour> _monoInstallers = new();

        /// <summary>
        /// ScriptableObject 安装器列表
        /// </summary>
        [SerializeField]
        private List<ScriptableObject> _scriptableObjectInstallers = new();

        /// <summary>
        /// 容器实例
        /// </summary>
        private IObjectResolver _container;

        /// <summary>
        /// 父作用域
        /// </summary>
        private LifetimeScope _parent;

        /// <summary>
        /// 是否已构建
        /// </summary>
        private bool _isBuilt;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool _isDisposed;

        #endregion

        #region 属性

        /// <summary>
        /// 获取容器实例
        /// </summary>
        public IObjectResolver Container => _container;

        /// <summary>
        /// 获取父作用域
        /// </summary>
        public LifetimeScope Parent => _parent;

        /// <summary>
        /// 是否已构建
        /// </summary>
        public bool IsBuilt => _isBuilt;

        /// <summary>
        /// 是否为根作用域
        /// </summary>
        public bool IsRoot => _parent == null;

        #endregion

        #region Unity 生命周期

        protected virtual void Awake()
        {
            if (_autoRun)
            {
                Build();
            }
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 构建容器
        /// </summary>
        public void Build()
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"[LifetimeScope] {name} 已经构建过了", this);
                return;
            }

            // 解析父作用域
            ResolveParent();

            // 创建容器构建器
            var builder = CreateBuilder();

            // 配置服务
            ConfigureServices(builder);

            // 构建容器
            _container = builder.Build();
            _isBuilt = true;

            // 触发构建完成回调
            OnContainerBuilt(_container);
        }

        /// <summary>
        /// 手动设置父作用域
        /// </summary>
        /// <param name="parent">父作用域</param>
        public void SetParent(LifetimeScope parent)
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"[LifetimeScope] {name} 已构建，无法更改父作用域", this);
                return;
            }
            _parent = parent;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (_container is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _container = null;
            _isBuilt = false;
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 配置容器
        /// <para>子类重写此方法来注册服务</para>
        /// </summary>
        /// <param name="builder">容器构建器</param>
        protected virtual void Configure(IContainerBuilder builder)
        {
            // 子类重写此方法
        }

        /// <summary>
        /// 容器构建完成回调
        /// </summary>
        /// <param name="container">构建的容器</param>
        protected virtual void OnContainerBuilt(IObjectResolver container)
        {
            // 子类可重写此方法
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 解析父作用域
        /// </summary>
        private void ResolveParent()
        {
            if (_parent != null) return;

            // 从 ParentReference 获取
            if (_parentReference != null)
            {
                _parent = _parentReference.GetParent(this);
            }

            // 从父级 GameObject 查找
            if (_parent == null && transform.parent != null)
            {
                _parent = transform.parent.GetComponentInParent<LifetimeScope>();
            }
        }

        /// <summary>
        /// 创建容器构建器
        /// </summary>
        private IContainerBuilder CreateBuilder()
        {
            IObjectResolver parentContainer = _parent?.Container;
            return new ContainerBuilder(parentContainer);
        }

        /// <summary>
        /// 配置所有服务
        /// </summary>
        private void ConfigureServices(IContainerBuilder builder)
        {
            // 注册自身
            builder.RegisterInstance(this);

            // 执行 MonoBehaviour 安装器
            foreach (var installer in _monoInstallers)
            {
                if (installer is IInstaller monoInstaller)
                {
                    monoInstaller.Install(builder);
                }
            }

            // 执行 ScriptableObject 安装器
            foreach (var installer in _scriptableObjectInstallers)
            {
                if (installer is IInstaller soInstaller)
                {
                    soInstaller.Install(builder);
                }
            }

            // 执行子类配置
            Configure(builder);
        }

        #endregion
    }
}
