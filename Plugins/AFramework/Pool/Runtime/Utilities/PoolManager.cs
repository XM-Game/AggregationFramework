// ==========================================================
// 文件名：PoolManager.cs
// 命名空间: AFramework.Pool.Utilities
// 依赖: System, AFramework.Pool
// 功能: 全局对象池管理器，统一管理所有对象池
// ==========================================================

using System;
using System.Collections.Generic;


namespace AFramework.Pool.Utilities
{
    /// <summary>
    /// 全局对象池管理器
    /// Global Pool Manager
    /// </summary>
    /// <remarks>
    /// 提供全局对象池管理功能：
    /// - 池注册和查找
    /// - 统一生命周期管理
    /// - 全局统计信息
    /// - 批量操作
    /// Provides global pool management features:
    /// - Pool registration and lookup
    /// - Unified lifecycle management
    /// - Global statistics
    /// - Batch operations
    /// </remarks>
    public class PoolManager : IDisposable
    {
        #region 字段 Fields

        private readonly Dictionary<string, IObjectPool> _pools;
        private readonly Dictionary<Type, IObjectPool> _poolsByType;
        private bool _isDisposed;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 池数量
        /// Pool count
        /// </summary>
        public int PoolCount => _pools.Count;

        /// <summary>
        /// 是否已释放
        /// Whether disposed
        /// </summary>
        public bool IsDisposed => _isDisposed;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建池管理器
        /// Create pool manager
        /// </summary>
        public PoolManager()
        {
            _pools = new Dictionary<string, IObjectPool>();
            _poolsByType = new Dictionary<Type, IObjectPool>();
            _isDisposed = false;
        }

        #endregion

        #region 注册方法 Registration Methods

        /// <summary>
        /// 注册对象池
        /// Register object pool
        /// </summary>
        /// <param name="name">池名称 Pool name</param>
        /// <param name="pool">对象池实例 Pool instance</param>
        public void RegisterPool(string name, IObjectPool pool)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            ThrowIfDisposed();

            _pools[name] = pool;
        }

        /// <summary>
        /// 注册泛型对象池
        /// Register generic object pool
        /// </summary>
        public void RegisterPool<T>(string name, IObjectPool<T> pool) where T : class
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            ThrowIfDisposed();

            _pools[name] = pool;
            _poolsByType[typeof(T)] = pool;
        }

        /// <summary>
        /// 注销对象池
        /// Unregister object pool
        /// </summary>
        public bool UnregisterPool(string name)
        {
            ThrowIfDisposed();

            if (_pools.TryGetValue(name, out var pool))
            {
                _pools.Remove(name);

                // 从类型字典中移除 Remove from type dictionary
                var typeToRemove = Type.EmptyTypes;
                foreach (var kvp in _poolsByType)
                {
                    if (kvp.Value == pool)
                    {
                        typeToRemove = new[] { kvp.Key };
                        break;
                    }
                }

                foreach (var type in typeToRemove)
                {
                    _poolsByType.Remove(type);
                }

                return true;
            }

            return false;
        }

        #endregion

        #region 查找方法 Lookup Methods

        /// <summary>
        /// 获取对象池
        /// Get object pool
        /// </summary>
        public IObjectPool GetPool(string name)
        {
            ThrowIfDisposed();

            if (_pools.TryGetValue(name, out var pool))
            {
                return pool;
            }

            throw new KeyNotFoundException($"未找到名为 '{name}' 的对象池 Pool named '{name}' not found");
        }

        /// <summary>
        /// 获取泛型对象池
        /// Get generic object pool
        /// </summary>
        public IObjectPool<T> GetPool<T>() where T : class
        {
            ThrowIfDisposed();

            if (_poolsByType.TryGetValue(typeof(T), out var pool))
            {
                return pool as IObjectPool<T>;
            }

            throw new KeyNotFoundException($"未找到类型 '{typeof(T).Name}' 的对象池 Pool for type '{typeof(T).Name}' not found");
        }

        /// <summary>
        /// 尝试获取对象池
        /// Try get object pool
        /// </summary>
        public bool TryGetPool(string name, out IObjectPool pool)
        {
            ThrowIfDisposed();
            return _pools.TryGetValue(name, out pool);
        }

        /// <summary>
        /// 尝试获取泛型对象池
        /// Try get generic object pool
        /// </summary>
        public bool TryGetPool<T>(out IObjectPool<T> pool) where T : class
        {
            ThrowIfDisposed();

            if (_poolsByType.TryGetValue(typeof(T), out var objPool))
            {
                pool = objPool as IObjectPool<T>;
                return pool != null;
            }

            pool = null;
            return false;
        }

        #endregion

        #region 批量操作 Batch Operations

        /// <summary>
        /// 清空所有池
        /// Clear all pools
        /// </summary>
        public void ClearAll()
        {
            ThrowIfDisposed();

            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
        }

        /// <summary>
        /// 获取所有池名称
        /// Get all pool names
        /// </summary>
        public IEnumerable<string> GetAllPoolNames()
        {
            ThrowIfDisposed();
            return _pools.Keys;
        }

        /// <summary>
        /// 获取所有池
        /// Get all pools
        /// </summary>
        public IEnumerable<IObjectPool> GetAllPools()
        {
            ThrowIfDisposed();
            return _pools.Values;
        }

        #endregion

        #region 统计方法 Statistics Methods

        /// <summary>
        /// 获取全局统计信息
        /// Get global statistics
        /// </summary>
        public PoolManagerStatistics GetStatistics()
        {
            ThrowIfDisposed();

            var stats = new PoolManagerStatistics
            {
                TotalPools = _pools.Count,
                TotalActiveObjects = 0,
                TotalIdleObjects = 0
            };

            foreach (var pool in _pools.Values)
            {
                stats.TotalActiveObjects += pool.ActiveCount;
                stats.TotalIdleObjects += pool.AvailableCount;
            }

            return stats;
        }

        #endregion

        #region IDisposable 实现 IDisposable Implementation

        /// <summary>
        /// 释放资源
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            foreach (var pool in _pools.Values)
            {
                if (pool is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _pools.Clear();
            _poolsByType.Clear();
            _isDisposed = true;
        }

        #endregion

        #region 私有方法 Private Methods

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(PoolManager));
            }
        }

        #endregion

        #region 静态实例 Static Instance

        private static readonly Lazy<PoolManager> _instance = new Lazy<PoolManager>(() => new PoolManager());

        /// <summary>
        /// 全局单例实例
        /// Global singleton instance
        /// </summary>
        public static PoolManager Instance => _instance.Value;

        #endregion
    }

    #region 统计信息 Statistics

    /// <summary>
    /// 池管理器统计信息
    /// Pool Manager Statistics
    /// </summary>
    public class PoolManagerStatistics
    {
        /// <summary>
        /// 总池数量
        /// Total pool count
        /// </summary>
        public int TotalPools { get; set; }

        /// <summary>
        /// 总活跃对象数
        /// Total active object count
        /// </summary>
        public int TotalActiveObjects { get; set; }

        /// <summary>
        /// 总空闲对象数
        /// Total idle object count
        /// </summary>
        public int TotalIdleObjects { get; set; }

        /// <summary>
        /// 总对象数
        /// Total object count
        /// </summary>
        public int TotalObjects => TotalActiveObjects + TotalIdleObjects;

        public override string ToString()
        {
            return $"PoolManager: Pools={TotalPools}, Active={TotalActiveObjects}, Idle={TotalIdleObjects}, Total={TotalObjects}";
        }
    }

    #endregion
}
