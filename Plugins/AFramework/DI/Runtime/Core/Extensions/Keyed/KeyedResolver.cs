// ==========================================================
// 文件名：KeyedResolver.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 键值解析器
    /// <para>提供按键值解析服务的能力</para>
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="T">服务类型</typeparam>
    public sealed class KeyedResolver<TKey, T> where T : class
    {
        private readonly IObjectResolver _resolver;
        private readonly Dictionary<TKey, T> _cache = new();

        public KeyedResolver(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// 按键值解析服务
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>服务实例</returns>
        public T Resolve(TKey key)
        {
            return _resolver.ResolveKeyed<T>(key);
        }

        /// <summary>
        /// 尝试按键值解析服务
        /// </summary>
        public bool TryResolve(TKey key, out T instance)
        {
            return _resolver.TryResolveKeyed(key, out instance);
        }

        /// <summary>
        /// 按键值解析服务（带缓存）
        /// </summary>
        public T ResolveCached(TKey key)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var instance = Resolve(key);
            _cache[key] = instance;
            return instance;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// 枚举键值解析器
    /// <para>专门用于枚举类型键值的解析器</para>
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <typeparam name="T">服务类型</typeparam>
    public sealed class EnumKeyedResolver<TEnum, T> 
        where TEnum : Enum 
        where T : class
    {
        private readonly IObjectResolver _resolver;
        private readonly Dictionary<TEnum, T> _cache = new();

        public EnumKeyedResolver(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// 按枚举键值解析服务
        /// </summary>
        public T Resolve(TEnum key)
        {
            return _resolver.ResolveKeyed<T>(key);
        }

        /// <summary>
        /// 尝试按枚举键值解析服务
        /// </summary>
        public bool TryResolve(TEnum key, out T instance)
        {
            return _resolver.TryResolveKeyed(key, out instance);
        }

        /// <summary>
        /// 按枚举键值解析服务（带缓存）
        /// </summary>
        public T ResolveCached(TEnum key)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var instance = Resolve(key);
            _cache[key] = instance;
            return instance;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
