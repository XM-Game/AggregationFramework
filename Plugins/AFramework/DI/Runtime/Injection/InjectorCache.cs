// ==========================================================
// 文件名：InjectorCache.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 提供注入器的缓存管理，支持自定义注入器

// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 注入器缓存
    /// <para>管理和缓存注入器实例，支持自定义注入器扩展</para>
    /// <para>Injector cache that manages and caches injector instances</para>
    /// </summary>
    public sealed class InjectorCache
    {
        #region 单例 / Singleton

        private static readonly Lazy<InjectorCache> _instance = 
            new Lazy<InjectorCache>(() => new InjectorCache());

        /// <summary>
        /// 获取缓存实例
        /// <para>Get the cache instance</para>
        /// </summary>
        public static InjectorCache Instance => _instance.Value;

        #endregion

        #region 字段 / Fields

        private readonly Dictionary<Type, IInjector> _customInjectors;
        private readonly object _syncRoot = new object();
        private IInjector _defaultInjector;

        #endregion

        #region 构造函数 / Constructor

        private InjectorCache()
        {
            _customInjectors = new Dictionary<Type, IInjector>();
            _defaultInjector = Injector.Instance;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取默认注入器
        /// <para>Get the default injector</para>
        /// </summary>
        public IInjector GetDefaultInjector()
        {
            return _defaultInjector;
        }

        /// <summary>
        /// 设置默认注入器
        /// <para>Set the default injector</para>
        /// </summary>
        /// <param name="injector">注入器实例 / Injector instance</param>
        public void SetDefaultInjector(IInjector injector)
        {
            _defaultInjector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        /// <summary>
        /// 获取指定类型的注入器
        /// <para>Get injector for a specific type</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>注入器实例 / Injector instance</returns>
        public IInjector GetInjector(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (_syncRoot)
            {
                if (_customInjectors.TryGetValue(type, out var injector))
                {
                    return injector;
                }
            }

            return _defaultInjector;
        }

        /// <summary>
        /// 注册自定义注入器
        /// <para>Register a custom injector for a specific type</para>
        /// </summary>
        /// <typeparam name="T">目标类型 / Target type</typeparam>
        /// <param name="injector">注入器实例 / Injector instance</param>
        public void RegisterInjector<T>(IInjector injector)
        {
            RegisterInjector(typeof(T), injector);
        }

        /// <summary>
        /// 注册自定义注入器
        /// <para>Register a custom injector for a specific type</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <param name="injector">注入器实例 / Injector instance</param>
        public void RegisterInjector(Type type, IInjector injector)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (injector == null)
                throw new ArgumentNullException(nameof(injector));

            lock (_syncRoot)
            {
                _customInjectors[type] = injector;
            }
        }

        /// <summary>
        /// 移除自定义注入器
        /// <para>Remove a custom injector</para>
        /// </summary>
        /// <typeparam name="T">目标类型 / Target type</typeparam>
        /// <returns>是否成功移除 / Whether removal succeeded</returns>
        public bool RemoveInjector<T>()
        {
            return RemoveInjector(typeof(T));
        }

        /// <summary>
        /// 移除自定义注入器
        /// <para>Remove a custom injector</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>是否成功移除 / Whether removal succeeded</returns>
        public bool RemoveInjector(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (_syncRoot)
            {
                return _customInjectors.Remove(type);
            }
        }

        /// <summary>
        /// 清除所有自定义注入器
        /// <para>Clear all custom injectors</para>
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot)
            {
                _customInjectors.Clear();
            }
        }

        /// <summary>
        /// 检查是否存在自定义注入器
        /// <para>Check if a custom injector exists</para>
        /// </summary>
        /// <typeparam name="T">目标类型 / Target type</typeparam>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasCustomInjector<T>()
        {
            return HasCustomInjector(typeof(T));
        }

        /// <summary>
        /// 检查是否存在自定义注入器
        /// <para>Check if a custom injector exists</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>是否存在 / Whether exists</returns>
        public bool HasCustomInjector(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (_syncRoot)
            {
                return _customInjectors.ContainsKey(type);
            }
        }

        /// <summary>
        /// 获取自定义注入器数量
        /// <para>Get the number of custom injectors</para>
        /// </summary>
        public int CustomInjectorCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _customInjectors.Count;
                }
            }
        }

        #endregion
    }
}
