// ==========================================================
// 文件名：ResolverFactory.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 解析器工厂，创建和管理自定义解析器实例
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 解析器工厂
    /// <para>创建和管理自定义解析器实例</para>
    /// <para>Resolver factory for creating and managing custom resolver instances</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>工厂方法模式：统一创建解析器实例</item>
    /// <item>单例模式：缓存解析器实例，避免重复创建</item>
    /// <item>依赖注入：支持从容器获取解析器实例</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 创建解析器实例
    /// var resolver = ResolverFactory.CreateResolver&lt;MyResolver&gt;();
    /// 
    /// // 从服务提供者创建
    /// var resolver = ResolverFactory.CreateResolver&lt;MyResolver&gt;(serviceProvider);
    /// </code>
    /// </remarks>
    public static class ResolverFactory
    {
        #region 私有字段 / Private Fields

        private static readonly Dictionary<Type, object> _resolverCache = new Dictionary<Type, object>();
        private static readonly object _lock = new object();

        #endregion

        #region 创建方法 / Create Methods

        /// <summary>
        /// 创建解析器实例
        /// <para>Create resolver instance</para>
        /// </summary>
        /// <typeparam name="TResolver">解析器类型 / Resolver type</typeparam>
        /// <returns>解析器实例 / Resolver instance</returns>
        /// <exception cref="InvalidOperationException">当无法创建解析器实例时抛出</exception>
        public static TResolver CreateResolver<TResolver>() where TResolver : class
        {
            return CreateResolver<TResolver>(null);
        }

        /// <summary>
        /// 创建解析器实例（支持依赖注入）
        /// <para>Create resolver instance (with dependency injection support)</para>
        /// </summary>
        /// <typeparam name="TResolver">解析器类型 / Resolver type</typeparam>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        /// <returns>解析器实例 / Resolver instance</returns>
        /// <exception cref="InvalidOperationException">当无法创建解析器实例时抛出</exception>
        public static TResolver CreateResolver<TResolver>(IServiceProvider serviceProvider) where TResolver : class
        {
            var resolverType = typeof(TResolver);

            // 尝试从缓存获取
            lock (_lock)
            {
                if (_resolverCache.TryGetValue(resolverType, out var cachedResolver))
                {
                    return (TResolver)cachedResolver;
                }
            }

            // 尝试从服务提供者获取
            if (serviceProvider != null)
            {
                var resolverFromService = serviceProvider.GetService(resolverType) as TResolver;
                if (resolverFromService != null)
                {
                    CacheResolver(resolverType, resolverFromService);
                    return resolverFromService;
                }
            }

            // 使用 Activator 创建
            try
            {
                var resolver = Activator.CreateInstance<TResolver>();
                CacheResolver(resolverType, resolver);
                return resolver;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"无法创建解析器实例。类型：{resolverType.Name}",
                    ex);
            }
        }

        /// <summary>
        /// 创建解析器实例（非泛型版本）
        /// <para>Create resolver instance (non-generic version)</para>
        /// </summary>
        /// <param name="resolverType">解析器类型 / Resolver type</param>
        /// <returns>解析器实例 / Resolver instance</returns>
        /// <exception cref="ArgumentNullException">当 resolverType 为 null 时抛出</exception>
        /// <exception cref="InvalidOperationException">当无法创建解析器实例时抛出</exception>
        public static object CreateResolver(Type resolverType)
        {
            return CreateResolver(resolverType, null);
        }

        /// <summary>
        /// 创建解析器实例（非泛型版本，支持依赖注入）
        /// <para>Create resolver instance (non-generic version, with dependency injection support)</para>
        /// </summary>
        /// <param name="resolverType">解析器类型 / Resolver type</param>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        /// <returns>解析器实例 / Resolver instance</returns>
        /// <exception cref="ArgumentNullException">当 resolverType 为 null 时抛出</exception>
        /// <exception cref="InvalidOperationException">当无法创建解析器实例时抛出</exception>
        public static object CreateResolver(Type resolverType, IServiceProvider serviceProvider)
        {
            if (resolverType == null)
                throw new ArgumentNullException(nameof(resolverType));

            // 尝试从缓存获取
            lock (_lock)
            {
                if (_resolverCache.TryGetValue(resolverType, out var cachedResolver))
                {
                    return cachedResolver;
                }
            }

            // 尝试从服务提供者获取
            if (serviceProvider != null)
            {
                var resolverFromService = serviceProvider.GetService(resolverType);
                if (resolverFromService != null)
                {
                    CacheResolver(resolverType, resolverFromService);
                    return resolverFromService;
                }
            }

            // 使用 Activator 创建
            try
            {
                var resolver = Activator.CreateInstance(resolverType);
                CacheResolver(resolverType, resolver);
                return resolver;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"无法创建解析器实例。类型：{resolverType.Name}",
                    ex);
            }
        }

        #endregion

        #region 缓存管理 / Cache Management

        /// <summary>
        /// 缓存解析器实例
        /// <para>Cache resolver instance</para>
        /// </summary>
        /// <param name="resolverType">解析器类型 / Resolver type</param>
        /// <param name="resolver">解析器实例 / Resolver instance</param>
        private static void CacheResolver(Type resolverType, object resolver)
        {
            lock (_lock)
            {
                if (!_resolverCache.ContainsKey(resolverType))
                {
                    _resolverCache[resolverType] = resolver;
                }
            }
        }

        /// <summary>
        /// 清除解析器缓存
        /// <para>Clear resolver cache</para>
        /// </summary>
        public static void ClearCache()
        {
            lock (_lock)
            {
                _resolverCache.Clear();
            }
        }

        /// <summary>
        /// 清除指定类型的解析器缓存
        /// <para>Clear resolver cache for specified type</para>
        /// </summary>
        /// <typeparam name="TResolver">解析器类型 / Resolver type</typeparam>
        public static void ClearCache<TResolver>()
        {
            lock (_lock)
            {
                _resolverCache.Remove(typeof(TResolver));
            }
        }

        /// <summary>
        /// 清除指定类型的解析器缓存
        /// <para>Clear resolver cache for specified type</para>
        /// </summary>
        /// <param name="resolverType">解析器类型 / Resolver type</param>
        public static void ClearCache(Type resolverType)
        {
            if (resolverType == null)
                throw new ArgumentNullException(nameof(resolverType));

            lock (_lock)
            {
                _resolverCache.Remove(resolverType);
            }
        }

        #endregion
    }
}
