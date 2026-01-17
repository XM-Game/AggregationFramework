// ==========================================================
// 文件名：ConcurrentCache.cs
// 命名空间: AFramework.AMapper.Internal
// 依赖: System, System.Collections.Concurrent
// 功能: 线程安全的通用缓存实现
// ==========================================================

using System;
using System.Collections.Concurrent;

namespace AFramework.AMapper.Internal
{
    /// <summary>
    /// 并发缓存
    /// <para>线程安全的通用缓存实现</para>
    /// <para>Thread-safe generic cache implementation</para>
    /// </summary>
    /// <typeparam name="TKey">键类型 / Key type</typeparam>
    /// <typeparam name="TValue">值类型 / Value type</typeparam>
    public sealed class ConcurrentCache<TKey, TValue>
    {
        #region 私有字段 / Private Fields

        private readonly ConcurrentDictionary<TKey, TValue> _cache;
        private readonly Func<TKey, TValue> _valueFactory;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建并发缓存
        /// </summary>
        public ConcurrentCache()
        {
            _cache = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// 创建并发缓存（带值工厂）
        /// </summary>
        /// <param name="valueFactory">值工厂 / Value factory</param>
        public ConcurrentCache(Func<TKey, TValue> valueFactory)
        {
            _cache = new ConcurrentDictionary<TKey, TValue>();
            _valueFactory = valueFactory;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取或添加值
        /// <para>Get or add value</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <param name="valueFactory">值工厂 / Value factory</param>
        /// <returns>值 / Value</returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return _cache.GetOrAdd(key, valueFactory);
        }

        /// <summary>
        /// 获取或添加值（使用默认工厂）
        /// <para>Get or add value (using default factory)</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <returns>值 / Value</returns>
        public TValue GetOrAdd(TKey key)
        {
            if (_valueFactory == null)
                throw new InvalidOperationException("未设置默认值工厂 / Default value factory not set");

            return _cache.GetOrAdd(key, _valueFactory);
        }

        /// <summary>
        /// 尝试获取值
        /// <para>Try to get value</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <param name="value">值 / Value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// 添加或更新值
        /// <para>Add or update value</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <param name="value">值 / Value</param>
        public void Set(TKey key, TValue value)
        {
            _cache[key] = value;
        }

        /// <summary>
        /// 移除值
        /// <para>Remove value</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <returns>是否成功 / Whether successful</returns>
        public bool Remove(TKey key)
        {
            return _cache.TryRemove(key, out _);
        }

        /// <summary>
        /// 检查是否包含键
        /// <para>Check if contains key</para>
        /// </summary>
        /// <param name="key">键 / Key</param>
        /// <returns>是否包含 / Whether contains</returns>
        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear cache</para>
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 获取缓存数量
        /// <para>Get cache count</para>
        /// </summary>
        public int Count => _cache.Count;

        #endregion
    }
}
