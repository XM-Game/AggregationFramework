// ==========================================================
// 文件名：ScopedPoolManager.cs
// 命名空间: AFramework.Pool.DI
// 依赖: System, System.Collections.Generic, AFramework.DI
// 功能: 作用域对象池管理器
// ==========================================================

using System;
using System.Collections.Generic;
using AFramework.DI;


namespace AFramework.Pool.DI
{
    /// <summary>
    /// 作用域对象池管理器
    /// Scoped Pool Manager
    /// </summary>
    /// <remarks>
    /// 管理特定作用域内的对象池，支持作用域销毁时自动清理
    /// Manages object pools within a specific scope, supports automatic cleanup on scope disposal
    /// </remarks>
    public class ScopedPoolManager : IDisposable
    {
        #region 字段 Fields

        private readonly IObjectResolver _resolver;
        private readonly Dictionary<Type, object> _scopedPools;
        private readonly object _lock = new object();
        private bool _disposed;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取作用域内池的数量
        /// Get count of pools in scope
        /// </summary>
        public int PoolCount
        {
            get
            {
                lock (_lock)
                {
                    return _scopedPools.Count;
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化作用域池管理器
        /// Initialize scoped pool manager
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        public ScopedPoolManager(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _scopedPools = new Dictionary<Type, object>();
        }

        #endregion

        #region 池管理 Pool Management

        /// <summary>
        /// 获取或创建作用域池
        /// Get or create scoped pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="factory">池工厂（可选）/ Pool factory (optional)</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> GetOrCreatePool<T>(Func<IObjectPool<T>> factory = null) where T : class
        {
            ThrowIfDisposed();

            var type = typeof(T);

            lock (_lock)
            {
                // 尝试从作用域获取
                // Try to get from scope
                if (_scopedPools.TryGetValue(type, out var cached))
                {
                    return (IObjectPool<T>)cached;
                }

                // 创建新池
                // Create new pool
                IObjectPool<T> pool;

                if (factory != null)
                {
                    pool = factory();
                }
                else
                {
                    // 尝试从容器解析
                    // Try to resolve from container
                    try
                    {
                        pool = _resolver.Resolve<IObjectPool<T>>();
                    }
                    catch
                    {
                        // 创建默认池
                        // Create default pool
                        pool = new ObjectPool<T>(() => Activator.CreateInstance<T>(), 100);
                    }
                }

                _scopedPools[type] = pool;
                return pool;
            }
        }

        /// <summary>
        /// 移除作用域池
        /// Remove scoped pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="dispose">是否销毁池 / Whether to dispose pool</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemovePool<T>(bool dispose = true) where T : class
        {
            ThrowIfDisposed();

            var type = typeof(T);

            lock (_lock)
            {
                if (_scopedPools.TryGetValue(type, out var pool))
                {
                    _scopedPools.Remove(type);

                    if (dispose && pool is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 清空所有作用域池
        /// Clear all scoped pools
        /// </summary>
        /// <param name="dispose">是否销毁池 / Whether to dispose pools</param>
        public void ClearPools(bool dispose = true)
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (dispose)
                {
                    foreach (var pool in _scopedPools.Values)
                    {
                        if (pool is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }

                _scopedPools.Clear();
            }
        }

        #endregion

        #region 便捷方法 Convenience Methods

        /// <summary>
        /// 从作用域池获取对象
        /// Get object from scoped pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <returns>池化对象 / Pooled object</returns>
        public T Get<T>() where T : class
        {
            var pool = GetOrCreatePool<T>();
            return pool.Get();
        }

        /// <summary>
        /// 将对象归还到作用域池
        /// Return object to scoped pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="obj">要归还的对象 / Object to return</param>
        public void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            var pool = GetOrCreatePool<T>();
            pool.Return(obj);
        }

        /// <summary>
        /// 预热作用域池
        /// Warmup scoped pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="count">预热数量 / Warmup count</param>
        public void Warmup<T>(int count) where T : class
        {
            var pool = GetOrCreatePool<T>();
            pool.Warmup(count);
        }

        #endregion

        #region IDisposable 实现

        /// <summary>
        /// 销毁作用域池管理器
        /// Dispose scoped pool manager
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            ClearPools(dispose: true);
            _disposed = true;
        }

        /// <summary>
        /// 检查是否已销毁
        /// Check if disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ScopedPoolManager));
            }
        }

        #endregion
    }
}
