// ==========================================================
// 文件名：ScopeInstanceCache.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 作用域实例缓存，管理作用域内的实例存储
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 作用域实例缓存
    /// <para>管理作用域内服务实例的缓存，提供线程安全的实例存取</para>
    /// <para>Scope instance cache that manages service instance caching within a scope</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于实例缓存管理</item>
    /// <item>线程安全：所有操作都是线程安全的</item>
    /// <item>高效查找：使用字典实现 O(1) 查找</item>
    /// </list>
    /// 
    /// 核心功能：
    /// <list type="bullet">
    /// <item>类型到实例的映射</item>
    /// <item>键值实例的映射</item>
    /// <item>线程安全的获取或创建</item>
    /// <item>缓存清理</item>
    /// </list>
    /// </remarks>
    public sealed class ScopeInstanceCache
    {
        #region 字段 / Fields

        /// <summary>
        /// 类型实例缓存
        /// </summary>
        private readonly Dictionary<Type, object> _typeCache;

        /// <summary>
        /// 键值实例缓存
        /// </summary>
        private readonly Dictionary<(Type, object), object> _keyedCache;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 是否启用诊断
        /// </summary>
        private readonly bool _enableDiagnostics;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取缓存的实例数量
        /// <para>Get the count of cached instances</para>
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _typeCache.Count + _keyedCache.Count;
                }
            }
        }

        /// <summary>
        /// 获取类型缓存数量
        /// <para>Get the count of type cache</para>
        /// </summary>
        public int TypeCacheCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _typeCache.Count;
                }
            }
        }

        /// <summary>
        /// 获取键值缓存数量
        /// <para>Get the count of keyed cache</para>
        /// </summary>
        public int KeyedCacheCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _keyedCache.Count;
                }
            }
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建作用域实例缓存
        /// </summary>
        /// <param name="enableDiagnostics">是否启用诊断 / Whether to enable diagnostics</param>
        public ScopeInstanceCache(bool enableDiagnostics = false)
        {
            _enableDiagnostics = enableDiagnostics;
            _typeCache = new Dictionary<Type, object>();
            _keyedCache = new Dictionary<(Type, object), object>();
        }

        #endregion

        #region 类型缓存操作 / Type Cache Operations

        /// <summary>
        /// 获取或创建实例
        /// <para>Get or create an instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>实例 / Instance</returns>
        /// <exception cref="ArgumentNullException">当参数为 null 时抛出</exception>
        public object GetOrCreate(Type type, Func<object> factory)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            // 第一次检查（无锁）
            if (_typeCache.TryGetValue(type, out var existing))
            {
                return existing;
            }

            lock (_syncRoot)
            {
                // 第二次检查（有锁）
                if (_typeCache.TryGetValue(type, out existing))
                {
                    return existing;
                }

                // 创建实例
                var instance = factory();
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"工厂函数为类型 '{type.FullName}' 返回了 null。\n" +
                        $"Factory returned null for type '{type.FullName}'.");
                }

                _typeCache[type] = instance;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"缓存实例: {type.Name}");
                }

                return instance;
            }
        }

        /// <summary>
        /// 尝试获取实例
        /// <para>Try to get an instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="instance">输出的实例 / Output instance</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public bool TryGet(Type type, out object instance)
        {
            if (type == null)
            {
                instance = null;
                return false;
            }

            lock (_syncRoot)
            {
                return _typeCache.TryGetValue(type, out instance);
            }
        }

        /// <summary>
        /// 检查是否包含指定类型的实例
        /// <para>Check if contains an instance of specified type</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>是否包含 / Whether contains</returns>
        public bool Contains(Type type)
        {
            if (type == null)
                return false;

            lock (_syncRoot)
            {
                return _typeCache.ContainsKey(type);
            }
        }

        /// <summary>
        /// 设置实例
        /// <para>Set an instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="instance">实例 / Instance</param>
        public void Set(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (_syncRoot)
            {
                _typeCache[type] = instance;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"设置实例: {type.Name}");
                }
            }
        }

        /// <summary>
        /// 移除实例
        /// <para>Remove an instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool Remove(Type type)
        {
            if (type == null)
                return false;

            lock (_syncRoot)
            {
                var removed = _typeCache.Remove(type);

                if (removed && _enableDiagnostics)
                {
                    LogDiagnostic($"移除实例: {type.Name}");
                }

                return removed;
            }
        }

        #endregion

        #region 键值缓存操作 / Keyed Cache Operations

        /// <summary>
        /// 获取或创建键值实例
        /// <para>Get or create a keyed instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <param name="factory">实例工厂函数 / Instance factory function</param>
        /// <returns>实例 / Instance</returns>
        public object GetOrCreateKeyed(Type type, object key, Func<object> factory)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var cacheKey = (type, key);

            // 第一次检查（无锁）
            if (_keyedCache.TryGetValue(cacheKey, out var existing))
            {
                return existing;
            }

            lock (_syncRoot)
            {
                // 第二次检查（有锁）
                if (_keyedCache.TryGetValue(cacheKey, out existing))
                {
                    return existing;
                }

                // 创建实例
                var instance = factory();
                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"工厂函数为类型 '{type.FullName}' (键值: '{key}') 返回了 null。\n" +
                        $"Factory returned null for type '{type.FullName}' (key: '{key}').");
                }

                _keyedCache[cacheKey] = instance;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"缓存键值实例: {type.Name} (Key: {key})");
                }

                return instance;
            }
        }

        /// <summary>
        /// 尝试获取键值实例
        /// <para>Try to get a keyed instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <param name="instance">输出的实例 / Output instance</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public bool TryGetKeyed(Type type, object key, out object instance)
        {
            if (type == null || key == null)
            {
                instance = null;
                return false;
            }

            lock (_syncRoot)
            {
                return _keyedCache.TryGetValue((type, key), out instance);
            }
        }

        /// <summary>
        /// 检查是否包含指定键值的实例
        /// <para>Check if contains a keyed instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <returns>是否包含 / Whether contains</returns>
        public bool ContainsKeyed(Type type, object key)
        {
            if (type == null || key == null)
                return false;

            lock (_syncRoot)
            {
                return _keyedCache.ContainsKey((type, key));
            }
        }

        /// <summary>
        /// 移除键值实例
        /// <para>Remove a keyed instance</para>
        /// </summary>
        /// <param name="type">服务类型 / Service type</param>
        /// <param name="key">键值 / Key</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemoveKeyed(Type type, object key)
        {
            if (type == null || key == null)
                return false;

            lock (_syncRoot)
            {
                var removed = _keyedCache.Remove((type, key));

                if (removed && _enableDiagnostics)
                {
                    LogDiagnostic($"移除键值实例: {type.Name} (Key: {key})");
                }

                return removed;
            }
        }

        #endregion

        #region 批量操作 / Batch Operations

        /// <summary>
        /// 清空所有缓存
        /// <para>Clear all cache</para>
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot)
            {
                var typeCount = _typeCache.Count;
                var keyedCount = _keyedCache.Count;

                _typeCache.Clear();
                _keyedCache.Clear();

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"清空缓存: 类型缓存 {typeCount} 个, 键值缓存 {keyedCount} 个");
                }
            }
        }

        /// <summary>
        /// 获取所有缓存的类型
        /// <para>Get all cached types</para>
        /// </summary>
        /// <returns>类型集合 / Collection of types</returns>
        public IReadOnlyCollection<Type> GetCachedTypes()
        {
            lock (_syncRoot)
            {
                return new List<Type>(_typeCache.Keys);
            }
        }

        /// <summary>
        /// 获取所有缓存的键值对
        /// <para>Get all cached keyed pairs</para>
        /// </summary>
        /// <returns>键值对集合 / Collection of keyed pairs</returns>
        public IReadOnlyCollection<(Type Type, object Key)> GetCachedKeyedPairs()
        {
            lock (_syncRoot)
            {
                return new List<(Type, object)>(_keyedCache.Keys);
            }
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取诊断信息
        /// <para>Get diagnostic information</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        public string GetDiagnosticInfo()
        {
            lock (_syncRoot)
            {
                return $"ScopeInstanceCache[TypeCache={_typeCache.Count}, KeyedCache={_keyedCache.Count}]";
            }
        }

        /// <summary>
        /// 获取所有缓存实例的类型名称
        /// <para>Get type names of all cached instances</para>
        /// </summary>
        /// <returns>类型名称集合 / Collection of type names</returns>
        public IReadOnlyList<string> GetCachedTypeNames()
        {
            lock (_syncRoot)
            {
                var names = new List<string>();
                
                foreach (var type in _typeCache.Keys)
                {
                    names.Add(type.Name);
                }
                
                foreach (var kvp in _keyedCache.Keys)
                {
                    names.Add($"{kvp.Item1.Name} (Key: {kvp.Item2})");
                }
                
                return names;
            }
        }

        private void LogDiagnostic(string message)
        {
            if (_enableDiagnostics)
            {
                UnityEngine.Debug.Log($"[AFramework.DI.ScopeCache] {message}");
            }
        }

        #endregion
    }
}
