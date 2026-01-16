// ==========================================================
// 文件名：CollectionResolver.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.DI
{
    /// <summary>
    /// 集合解析器
    /// <para>解析同一接口的所有实现</para>
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    public sealed class CollectionResolver<T> where T : class
    {
        private readonly IObjectResolver _resolver;
        private List<T> _cachedInstances;

        public CollectionResolver(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// 解析所有实现
        /// </summary>
        /// <returns>所有实现的集合</returns>
        public IEnumerable<T> ResolveAll()
        {
            return _resolver.ResolveAll<T>();
        }

        /// <summary>
        /// 解析所有实现并缓存
        /// </summary>
        public IReadOnlyList<T> ResolveAllCached()
        {
            return _cachedInstances ??= _resolver.ResolveAll<T>().ToList();
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _cachedInstances = null;
        }

        /// <summary>
        /// 获取实现数量
        /// </summary>
        public int Count => ResolveAllCached().Count;
    }
}
