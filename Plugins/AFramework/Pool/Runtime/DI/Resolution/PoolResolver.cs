// ==========================================================
// 文件名：PoolResolver.cs
// 命名空间: AFramework.Pool.DI
// 依赖: System, AFramework.DI, AFramework.Pool
// 功能: 对象池解析器
// ==========================================================

using System;
using System.Collections.Generic;
using AFramework.DI;

namespace AFramework.Pool.DI
{
    /// <summary>
    /// 对象池解析器
    /// Pool Resolver
    /// </summary>
    /// <remarks>
    /// 提供从容器中解析对象池的便捷方法
    /// Provides convenient methods to resolve object pools from container
    /// </remarks>
    public class PoolResolver
    {
        #region 字段 Fields

        private readonly IObjectResolver _resolver;
        private readonly Dictionary<Type, object> _poolCache;
        private readonly object _lock = new object();

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化池解析器
        /// Initialize pool resolver
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        public PoolResolver(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _poolCache = new Dictionary<Type, object>();
        }

        #endregion

        #region 解析方法 Resolution Methods

        /// <summary>
        /// 解析对象池
        /// Resolve object pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> Resolve<T>() where T : class
        {
            var type = typeof(T);

            lock (_lock)
            {
                // 尝试从缓存获取
                // Try to get from cache
                if (_poolCache.TryGetValue(type, out var cached))
                {
                    return (IObjectPool<T>)cached;
                }

                // 从容器解析
                // Resolve from container
                var pool = _resolver.Resolve<IObjectPool<T>>();

                // 缓存池实例
                // Cache pool instance
                _poolCache[type] = pool;

                return pool;
            }
        }

        /// <summary>
        /// 尝试解析对象池
        /// Try to resolve object pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="pool">对象池实例 / Object pool instance</param>
        /// <returns>是否解析成功 / Whether resolution succeeded</returns>
        public bool TryResolve<T>(out IObjectPool<T> pool) where T : class
        {
            try
            {
                pool = Resolve<T>();
                return pool != null;
            }
            catch
            {
                pool = null;
                return false;
            }
        }

        /// <summary>
        /// 解析所有对象池
        /// Resolve all object pools
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <returns>对象池列表 / Object pool list</returns>
        public IEnumerable<IObjectPool<T>> ResolveAll<T>() where T : class
        {
            return _resolver.ResolveAll<IObjectPool<T>>();
        }

        /// <summary>
        /// 清空缓存
        /// Clear cache
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                _poolCache.Clear();
            }
        }

        #endregion

        #region 便捷方法 Convenience Methods

        /// <summary>
        /// 从池中获取对象
        /// Get object from pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <returns>池化对象 / Pooled object</returns>
        public T Get<T>() where T : class
        {
            var pool = Resolve<T>();
            return pool.Get();
        }

        /// <summary>
        /// 将对象归还到池
        /// Return object to pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="obj">要归还的对象 / Object to return</param>
        public void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            var pool = Resolve<T>();
            pool.Return(obj);
        }

        #endregion
    }
}
