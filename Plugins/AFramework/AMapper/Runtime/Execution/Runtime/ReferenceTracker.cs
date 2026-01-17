// ==========================================================
// 文件名：ReferenceTracker.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 引用跟踪器，处理循环引用和对象缓存
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.AMapper
{
    /// <summary>
    /// 引用跟踪器
    /// <para>跟踪映射过程中的对象引用，处理循环引用</para>
    /// <para>Reference tracker for tracking object references and handling circular references</para>
    /// </summary>
    /// <remarks>
    /// ReferenceTracker 用于解决对象图中的循环引用问题：
    /// <list type="bullet">
    /// <item>缓存已映射的对象，避免重复映射</item>
    /// <item>检测循环引用，防止无限递归</item>
    /// <item>支持按类型分组的缓存策略</item>
    /// </list>
    /// 
    /// 使用场景：
    /// <code>
    /// // 启用引用保留
    /// CreateMap&lt;Parent, ParentDto&gt;()
    ///     .PreserveReferences();
    /// </code>
    /// </remarks>
    public sealed class ReferenceTracker : IDisposable
    {
        #region 私有字段 / Private Fields

        private readonly Dictionary<ReferenceKey, object> _cache;
        private readonly HashSet<object> _processingSet;
        private bool _disposed;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建引用跟踪器
        /// </summary>
        public ReferenceTracker()
        {
            _cache = new Dictionary<ReferenceKey, object>(ReferenceKeyComparer.Instance);
            _processingSet = new HashSet<object>(ReferenceEqualityComparer.Instance);
        }

        /// <summary>
        /// 创建引用跟踪器（指定初始容量）
        /// </summary>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        public ReferenceTracker(int capacity)
        {
            _cache = new Dictionary<ReferenceKey, object>(capacity, ReferenceKeyComparer.Instance);
            _processingSet = new HashSet<object>(ReferenceEqualityComparer.Instance);
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 尝试获取已缓存的目标对象
        /// <para>Try to get cached destination object</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <returns>是否找到缓存 / Whether cache found</returns>
        public bool TryGetDestination(object source, Type destinationType, out object destination)
        {
            ThrowIfDisposed();

            if (source == null)
            {
                destination = null;
                return false;
            }

            var key = new ReferenceKey(source, destinationType);
            return _cache.TryGetValue(key, out destination);
        }

        /// <summary>
        /// 尝试获取已缓存的目标对象（泛型版本）
        /// <para>Try to get cached destination object (generic version)</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <returns>是否找到缓存 / Whether cache found</returns>
        public bool TryGetDestination<TDestination>(object source, out TDestination destination)
        {
            if (TryGetDestination(source, typeof(TDestination), out var obj))
            {
                destination = (TDestination)obj;
                return true;
            }

            destination = default;
            return false;
        }

        /// <summary>
        /// 缓存目标对象
        /// <para>Cache destination object</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="destination">目标对象 / Destination object</param>
        public void CacheDestination(object source, Type destinationType, object destination)
        {
            ThrowIfDisposed();

            if (source == null || destination == null)
                return;

            var key = new ReferenceKey(source, destinationType);
            _cache[key] = destination;
        }

        /// <summary>
        /// 缓存目标对象（泛型版本）
        /// <para>Cache destination object (generic version)</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        public void CacheDestination<TDestination>(object source, TDestination destination)
        {
            CacheDestination(source, typeof(TDestination), destination);
        }

        /// <summary>
        /// 检查是否已缓存
        /// <para>Check if cached</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>是否已缓存 / Whether cached</returns>
        public bool IsCached(object source, Type destinationType)
        {
            ThrowIfDisposed();

            if (source == null)
                return false;

            var key = new ReferenceKey(source, destinationType);
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// 标记对象正在处理中（用于检测循环引用）
        /// <para>Mark object as being processed (for circular reference detection)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>如果对象已在处理中返回 false / Returns false if object is already being processed</returns>
        public bool TryStartProcessing(object source)
        {
            ThrowIfDisposed();

            if (source == null)
                return true;

            return _processingSet.Add(source);
        }

        /// <summary>
        /// 标记对象处理完成
        /// <para>Mark object processing as complete</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        public void EndProcessing(object source)
        {
            ThrowIfDisposed();

            if (source != null)
            {
                _processingSet.Remove(source);
            }
        }

        /// <summary>
        /// 检查对象是否正在处理中
        /// <para>Check if object is being processed</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>是否正在处理 / Whether being processed</returns>
        public bool IsProcessing(object source)
        {
            ThrowIfDisposed();

            if (source == null)
                return false;

            return _processingSet.Contains(source);
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear cache</para>
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            _cache.Clear();
            _processingSet.Clear();
        }

        /// <summary>
        /// 获取缓存数量
        /// <para>Get cache count</para>
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// 获取正在处理的对象数量
        /// <para>Get count of objects being processed</para>
        /// </summary>
        public int ProcessingCount => _processingSet.Count;

        #endregion

        #region IDisposable 实现 / Implementation

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _cache.Clear();
            _processingSet.Clear();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ReferenceTracker));
            }
        }

        #endregion

        #region 嵌套类型 / Nested Types

        /// <summary>
        /// 引用键
        /// <para>Reference key</para>
        /// </summary>
        private readonly struct ReferenceKey : IEquatable<ReferenceKey>
        {
            public readonly object Source;
            public readonly Type DestinationType;

            public ReferenceKey(object source, Type destinationType)
            {
                Source = source;
                DestinationType = destinationType;
            }

            public bool Equals(ReferenceKey other)
            {
                return ReferenceEquals(Source, other.Source) && DestinationType == other.DestinationType;
            }

            public override bool Equals(object obj)
            {
                return obj is ReferenceKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = RuntimeHelpers.GetHashCode(Source);
                    hash = (hash * 397) ^ (DestinationType?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }

        /// <summary>
        /// 引用键比较器
        /// <para>Reference key comparer</para>
        /// </summary>
        private sealed class ReferenceKeyComparer : IEqualityComparer<ReferenceKey>
        {
            public static readonly ReferenceKeyComparer Instance = new ReferenceKeyComparer();

            private ReferenceKeyComparer() { }

            public bool Equals(ReferenceKey x, ReferenceKey y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ReferenceKey obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// 引用相等性比较器
        /// <para>Reference equality comparer</para>
        /// </summary>
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            private ReferenceEqualityComparer() { }

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        #endregion
    }
}
