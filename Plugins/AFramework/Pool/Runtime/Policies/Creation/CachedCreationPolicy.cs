// ==========================================================
// 文件名：CachedCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 缓存创建策略，缓存创建结果以提高性能
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 缓存创建策略
    /// Cached Creation Policy
    /// 
    /// <para>缓存创建的对象模板，通过克隆提高创建性能</para>
    /// <para>Caches created object templates and clones them for better performance</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：原型模式 + 策略模式
    /// 使用场景：
    /// - 对象创建成本高（如复杂初始化）
    /// - 对象可以被克隆
    /// - 需要提高创建性能
    /// 
    /// 注意：
    /// - T 必须实现 ICloneable 或提供克隆函数
    /// - 缓存大小可配置
    /// </remarks>
    public class CachedCreationPolicy<T> : PoolPolicyBase, IPoolCreationPolicy<T>
    {
        #region Fields

        private readonly IPoolCreationPolicy<T> _innerPolicy;
        private readonly Func<T, T> _cloneFunc;
        private readonly Queue<T> _cache;
        private readonly int _maxCacheSize;
        private readonly object _lock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="innerPolicy">内部创建策略 / Inner creation policy</param>
        /// <param name="cloneFunc">克隆函数 / Clone function</param>
        /// <param name="maxCacheSize">最大缓存大小 / Maximum cache size</param>
        public CachedCreationPolicy(
            IPoolCreationPolicy<T> innerPolicy,
            Func<T, T> cloneFunc,
            int maxCacheSize = 10)
            : base("CachedCreation")
        {
            _innerPolicy = innerPolicy ?? throw new ArgumentNullException(nameof(innerPolicy));
            _cloneFunc = cloneFunc ?? throw new ArgumentNullException(nameof(cloneFunc));
            _maxCacheSize = Math.Max(1, maxCacheSize);
            _cache = new Queue<T>(_maxCacheSize);
        }

        /// <summary>
        /// 构造函数（使用 ICloneable）
        /// Constructor (using ICloneable)
        /// </summary>
        /// <param name="innerPolicy">内部创建策略 / Inner creation policy</param>
        /// <param name="maxCacheSize">最大缓存大小 / Maximum cache size</param>
        public CachedCreationPolicy(
            IPoolCreationPolicy<T> innerPolicy,
            int maxCacheSize = 10)
            : this(innerPolicy, CloneUsingICloneable, maxCacheSize)
        {
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            lock (_lock)
            {
                // 尝试从缓存获取
                // Try to get from cache
                if (_cache.Count > 0)
                {
                    T template = _cache.Dequeue();
                    return _cloneFunc(template);
                }

                // 缓存为空，创建新对象
                // Cache is empty, create new object
                T newObj = _innerPolicy.Create();

                // 如果缓存未满，保存一份到缓存
                // If cache is not full, save a copy to cache
                if (_cache.Count < _maxCacheSize)
                {
                    _cache.Enqueue(newObj);
                }

                return _cloneFunc(newObj);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _innerPolicy.Validate();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 清空缓存
        /// Clear cache
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// 获取当前缓存大小
        /// Get current cache size
        /// </summary>
        public int CacheSize
        {
            get
            {
                lock (_lock)
                {
                    return _cache.Count;
                }
            }
        }

        /// <summary>
        /// 预热缓存
        /// Prewarm cache
        /// </summary>
        /// <param name="count">预热数量 / Prewarm count</param>
        public void PrewarmCache(int count)
        {
            lock (_lock)
            {
                count = Math.Min(count, _maxCacheSize - _cache.Count);

                for (int i = 0; i < count; i++)
                {
                    T obj = _innerPolicy.Create();
                    _cache.Enqueue(obj);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 使用 ICloneable 克隆对象
        /// Clone object using ICloneable
        /// </summary>
        private static T CloneUsingICloneable(T obj)
        {
            if (obj is ICloneable cloneable)
            {
                return (T)cloneable.Clone();
            }

            throw new NotSupportedException(
                $"类型 {typeof(T).Name} 不支持 ICloneable 接口 / Type {typeof(T).Name} does not implement ICloneable");
        }

        #endregion
    }
}
