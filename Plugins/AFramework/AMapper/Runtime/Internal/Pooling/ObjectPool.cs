// ==========================================================
// 文件名：ObjectPool.cs
// 命名空间: AFramework.AMapper.Internal
// 依赖: System, System.Collections.Generic
// 功能: 通用对象池，减少GC压力
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper.Internal
{
    /// <summary>
    /// 通用对象池
    /// <para>减少频繁创建对象带来的GC压力</para>
    /// <para>Generic object pool to reduce GC pressure from frequent object creation</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    public sealed class ObjectPool<T> where T : class
    {
        #region 私有字段 / Private Fields

        private readonly Stack<T> _pool;
        private readonly Func<T> _factory;
        private readonly Action<T> _reset;
        private readonly int _maxSize;
        private readonly object _lock = new object();

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="factory">对象工厂 / Object factory</param>
        /// <param name="reset">重置方法 / Reset method</param>
        /// <param name="maxSize">最大容量 / Maximum size</param>
        public ObjectPool(Func<T> factory, Action<T> reset = null, int maxSize = 32)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _reset = reset;
            _maxSize = maxSize;
            _pool = new Stack<T>(maxSize);
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取对象
        /// <para>Get object from pool</para>
        /// </summary>
        /// <returns>对象实例 / Object instance</returns>
        public T Get()
        {
            lock (_lock)
            {
                if (_pool.Count > 0)
                {
                    return _pool.Pop();
                }
            }

            return _factory();
        }

        /// <summary>
        /// 归还对象
        /// <para>Return object to pool</para>
        /// </summary>
        /// <param name="item">对象实例 / Object instance</param>
        public void Return(T item)
        {
            if (item == null)
                return;

            _reset?.Invoke(item);

            lock (_lock)
            {
                if (_pool.Count < _maxSize)
                {
                    _pool.Push(item);
                }
            }
        }

        /// <summary>
        /// 清空对象池
        /// <para>Clear pool</para>
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _pool.Clear();
            }
        }

        /// <summary>
        /// 获取当前池中对象数量
        /// <para>Get current count in pool</para>
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _pool.Count;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// List 对象池
    /// <para>List object pool</para>
    /// </summary>
    /// <typeparam name="T">元素类型 / Element type</typeparam>
    public static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> _pool = new(
            () => new List<T>(),
            list => list.Clear(),
            16
        );

        /// <summary>
        /// 获取 List
        /// <para>Get list from pool</para>
        /// </summary>
        public static List<T> Get() => _pool.Get();

        /// <summary>
        /// 归还 List
        /// <para>Return list to pool</para>
        /// </summary>
        public static void Return(List<T> list) => _pool.Return(list);
    }

    /// <summary>
    /// Dictionary 对象池
    /// <para>Dictionary object pool</para>
    /// </summary>
    /// <typeparam name="TKey">键类型 / Key type</typeparam>
    /// <typeparam name="TValue">值类型 / Value type</typeparam>
    public static class DictionaryPool<TKey, TValue>
    {
        private static readonly ObjectPool<Dictionary<TKey, TValue>> _pool = new(
            () => new Dictionary<TKey, TValue>(),
            dict => dict.Clear(),
            16
        );

        /// <summary>
        /// 获取 Dictionary
        /// <para>Get dictionary from pool</para>
        /// </summary>
        public static Dictionary<TKey, TValue> Get() => _pool.Get();

        /// <summary>
        /// 归还 Dictionary
        /// <para>Return dictionary to pool</para>
        /// </summary>
        public static void Return(Dictionary<TKey, TValue> dict) => _pool.Return(dict);
    }

    /// <summary>
    /// 池化对象包装器
    /// <para>Pooled object wrapper for using statement</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    public readonly struct PooledObject<T> : IDisposable where T : class
    {
        private readonly T _value;
        private readonly Action<T> _returnAction;

        /// <summary>
        /// 获取池化对象
        /// <para>Get pooled object</para>
        /// </summary>
        public T Value => _value;

        internal PooledObject(T value, Action<T> returnAction)
        {
            _value = value;
            _returnAction = returnAction;
        }

        /// <summary>
        /// 释放并归还对象
        /// <para>Dispose and return object</para>
        /// </summary>
        public void Dispose()
        {
            _returnAction?.Invoke(_value);
        }
    }
}
