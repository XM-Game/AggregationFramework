// ==========================================================
// 文件名：ConcurrentCache.cs
// 命名空间: AFramework.DI.Internal
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI.Internal
{
    /// <summary>
    /// 并发安全缓存
    /// <para>提供线程安全的键值缓存</para>
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    internal sealed class ConcurrentCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly object _lock = new();
        private readonly int _maxCapacity;

        /// <summary>
        /// 创建并发缓存
        /// </summary>
        /// <param name="maxCapacity">最大容量（0表示无限制）</param>
        public ConcurrentCache(int maxCapacity = 0)
        {
            _maxCapacity = maxCapacity;
            _cache = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// 获取或添加缓存项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valueFactory">值工厂函数</param>
        /// <returns>缓存值</returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    return value;
                }

                // 检查容量限制
                if (_maxCapacity > 0 && _cache.Count >= _maxCapacity)
                {
                    _cache.Clear(); // 简单策略：清空缓存
                }

                value = valueFactory(key);
                _cache[key] = value;
                return value;
            }
        }

        /// <summary>
        /// 尝试获取缓存项
        /// </summary>
        public bool TryGet(TKey key, out TValue value)
        {
            lock (_lock)
            {
                return _cache.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// 添加或更新缓存项
        /// </summary>
        public void Set(TKey key, TValue value)
        {
            lock (_lock)
            {
                _cache[key] = value;
            }
        }

        /// <summary>
        /// 移除缓存项
        /// </summary>
        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return _cache.Remove(key);
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// 获取缓存项数量
        /// </summary>
        public int Count
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
        /// 检查是否包含键
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            lock (_lock)
            {
                return _cache.ContainsKey(key);
            }
        }
    }

    /// <summary>
    /// 类型键并发缓存
    /// <para>专门用于 Type 作为键的缓存，优化性能</para>
    /// </summary>
    internal sealed class TypeCache<TValue>
    {
        private readonly ConcurrentCache<Type, TValue> _cache;

        public TypeCache(int maxCapacity = 0)
        {
            _cache = new ConcurrentCache<Type, TValue>(maxCapacity);
        }

        public TValue GetOrAdd(Type type, Func<Type, TValue> valueFactory)
            => _cache.GetOrAdd(type, valueFactory);

        public bool TryGet(Type type, out TValue value)
            => _cache.TryGet(type, out value);

        public void Set(Type type, TValue value)
            => _cache.Set(type, value);

        public void Clear() => _cache.Clear();

        public int Count => _cache.Count;
    }
}
