// ==========================================================
// 文件名：Memoization.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 记忆化工具类
    /// <para>提供函数结果缓存功能，避免重复计算</para>
    /// </summary>
    public static class Memoization
    {
        #region 单参数记忆化

        /// <summary>
        /// 创建记忆化函数（单参数）
        /// </summary>
        public static Func<T, TResult> Memoize<T, TResult>(Func<T, TResult> func)
        {
            var cache = new Dictionary<T, TResult>();
            return arg =>
            {
                if (!cache.TryGetValue(arg, out var result))
                {
                    result = func(arg);
                    cache[arg] = result;
                }
                return result;
            };
        }

        /// <summary>
        /// 创建线程安全的记忆化函数（单参数）
        /// </summary>
        public static Func<T, TResult> MemoizeThreadSafe<T, TResult>(Func<T, TResult> func)
        {
            var cache = new System.Collections.Concurrent.ConcurrentDictionary<T, TResult>();
            return arg => cache.GetOrAdd(arg, func);
        }

        /// <summary>
        /// 创建带容量限制的记忆化函数（LRU 策略）
        /// </summary>
        public static Func<T, TResult> MemoizeWithCapacity<T, TResult>(Func<T, TResult> func, int capacity)
        {
            var cache = new LruCache<T, TResult>(capacity);
            return arg =>
            {
                if (cache.TryGet(arg, out var result))
                    return result;
                result = func(arg);
                cache.Set(arg, result);
                return result;
            };
        }

        #endregion

        #region 双参数记忆化

        /// <summary>
        /// 创建记忆化函数（双参数）
        /// </summary>
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            var cache = new Dictionary<(T1, T2), TResult>();
            return (arg1, arg2) =>
            {
                var key = (arg1, arg2);
                if (!cache.TryGetValue(key, out var result))
                {
                    result = func(arg1, arg2);
                    cache[key] = result;
                }
                return result;
            };
        }

        /// <summary>
        /// 创建线程安全的记忆化函数（双参数）
        /// </summary>
        public static Func<T1, T2, TResult> MemoizeThreadSafe<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            var cache = new System.Collections.Concurrent.ConcurrentDictionary<(T1, T2), TResult>();
            return (arg1, arg2) => cache.GetOrAdd((arg1, arg2), key => func(key.Item1, key.Item2));
        }

        #endregion

        #region 三参数记忆化

        /// <summary>
        /// 创建记忆化函数（三参数）
        /// </summary>
        public static Func<T1, T2, T3, TResult> Memoize<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
        {
            var cache = new Dictionary<(T1, T2, T3), TResult>();
            return (arg1, arg2, arg3) =>
            {
                var key = (arg1, arg2, arg3);
                if (!cache.TryGetValue(key, out var result))
                {
                    result = func(arg1, arg2, arg3);
                    cache[key] = result;
                }
                return result;
            };
        }

        #endregion

        #region 无参数记忆化

        /// <summary>
        /// 创建记忆化函数（无参数，只计算一次）
        /// </summary>
        public static Func<TResult> MemoizeOnce<TResult>(Func<TResult> func)
        {
            var computed = false;
            var result = default(TResult);
            return () =>
            {
                if (!computed)
                {
                    result = func();
                    computed = true;
                }
                return result;
            };
        }

        /// <summary>
        /// 创建线程安全的记忆化函数（无参数）
        /// </summary>
        public static Func<TResult> MemoizeOnceThreadSafe<TResult>(Func<TResult> func)
        {
            var lazy = new Lazy<TResult>(func);
            return () => lazy.Value;
        }

        #endregion

        #region 带过期时间的记忆化

        /// <summary>
        /// 创建带过期时间的记忆化函数
        /// </summary>
        public static Func<T, TResult> MemoizeWithExpiry<T, TResult>(Func<T, TResult> func, TimeSpan expiry)
        {
            var cache = new Dictionary<T, (TResult Value, DateTime Expiry)>();
            return arg =>
            {
                var now = DateTime.UtcNow;
                if (cache.TryGetValue(arg, out var entry) && entry.Expiry > now)
                    return entry.Value;
                
                var result = func(arg);
                cache[arg] = (result, now + expiry);
                return result;
            };
        }

        #endregion

        #region 可清除的记忆化

        /// <summary>
        /// 创建可清除缓存的记忆化函数
        /// </summary>
        public static MemoizedFunc<T, TResult> MemoizeWithClear<T, TResult>(Func<T, TResult> func)
        {
            return new MemoizedFunc<T, TResult>(func);
        }

        #endregion

        #region LRU 缓存

        /// <summary>
        /// LRU（最近最少使用）缓存
        /// </summary>
        private class LruCache<TKey, TValue>
        {
            private readonly int _capacity;
            private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _cache;
            private readonly LinkedList<(TKey Key, TValue Value)> _lruList;

            public LruCache(int capacity)
            {
                _capacity = capacity;
                _cache = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity);
                _lruList = new LinkedList<(TKey, TValue)>();
            }

            public bool TryGet(TKey key, out TValue value)
            {
                if (_cache.TryGetValue(key, out var node))
                {
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                    value = node.Value.Value;
                    return true;
                }
                value = default;
                return false;
            }

            public void Set(TKey key, TValue value)
            {
                if (_cache.TryGetValue(key, out var existingNode))
                {
                    _lruList.Remove(existingNode);
                    existingNode.Value = (key, value);
                    _lruList.AddFirst(existingNode);
                }
                else
                {
                    if (_cache.Count >= _capacity)
                    {
                        var lastNode = _lruList.Last;
                        _cache.Remove(lastNode.Value.Key);
                        _lruList.RemoveLast();
                    }
                    var newNode = new LinkedListNode<(TKey, TValue)>((key, value));
                    _lruList.AddFirst(newNode);
                    _cache[key] = newNode;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 可清除缓存的记忆化函数
    /// </summary>
    public class MemoizedFunc<T, TResult>
    {
        private readonly Func<T, TResult> _func;
        private readonly Dictionary<T, TResult> _cache;

        public MemoizedFunc(Func<T, TResult> func)
        {
            _func = func;
            _cache = new Dictionary<T, TResult>();
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        public TResult Invoke(T arg)
        {
            if (!_cache.TryGetValue(arg, out var result))
            {
                result = _func(arg);
                _cache[arg] = result;
            }
            return result;
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear() => _cache.Clear();

        /// <summary>
        /// 清除指定键的缓存
        /// </summary>
        public bool Remove(T key) => _cache.Remove(key);

        /// <summary>
        /// 缓存数量
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// 检查是否已缓存
        /// </summary>
        public bool IsCached(T key) => _cache.ContainsKey(key);

        /// <summary>
        /// 隐式转换为 Func
        /// </summary>
        public static implicit operator Func<T, TResult>(MemoizedFunc<T, TResult> memoized)
        {
            return memoized.Invoke;
        }
    }
}
