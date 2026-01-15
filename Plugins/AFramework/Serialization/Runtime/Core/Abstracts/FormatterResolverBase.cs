// ==========================================================
// 文件名：FormatterResolverBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器解析器抽象基类
    /// <para>提供 IFormatterResolver 接口的基础实现</para>
    /// <para>支持格式化器缓存和线程安全访问</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 使用 ConcurrentDictionary 实现线程安全缓存
    /// 2. 支持泛型和非泛型两种访问方式
    /// 3. 子类通过重写 GetFormatterCore 实现具体解析逻辑
    /// 
    /// 使用示例:
    /// <code>
    /// public class CustomResolver : FormatterResolverBase
    /// {
    ///     protected override IFormatter&lt;T&gt; GetFormatterCore&lt;T&gt;()
    ///     {
    ///         if (typeof(T) == typeof(Player))
    ///             return (IFormatter&lt;T&gt;)(object)new PlayerFormatter();
    ///         return null;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class FormatterResolverBase : IFormatterResolver
    {
        #region 字段

        /// <summary>格式化器缓存</summary>
        private readonly ConcurrentDictionary<Type, object> _cache;

        /// <summary>是否启用缓存</summary>
        private readonly bool _enableCache;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建格式化器解析器基类实例
        /// </summary>
        protected FormatterResolverBase() : this(true)
        {
        }

        /// <summary>
        /// 创建格式化器解析器基类实例
        /// </summary>
        /// <param name="enableCache">是否启用缓存</param>
        protected FormatterResolverBase(bool enableCache)
        {
            _enableCache = enableCache;
            if (enableCache)
                _cache = new ConcurrentDictionary<Type, object>();
        }

        #endregion

        #region IFormatterResolver 实现

        /// <inheritdoc/>
        public IFormatter<T> GetFormatter<T>()
        {
            var type = typeof(T);

            if (_enableCache)
            {
                return (IFormatter<T>)_cache.GetOrAdd(type, _ => GetFormatterCore<T>());
            }

            return GetFormatterCore<T>();
        }

        /// <inheritdoc/>
        public IFormatter GetFormatter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_enableCache)
            {
                return (IFormatter)_cache.GetOrAdd(type, t => GetFormatterCore(t));
            }

            return GetFormatterCore(type);
        }

        /// <inheritdoc/>
        public bool TryGetFormatter<T>(out IFormatter<T> formatter)
        {
            formatter = GetFormatter<T>();
            return formatter != null;
        }

        /// <inheritdoc/>
        public bool TryGetFormatter(Type type, out IFormatter formatter)
        {
            formatter = GetFormatter(type);
            return formatter != null;
        }

        /// <inheritdoc/>
        public virtual bool CanResolve(Type type)
        {
            if (type == null)
                return false;

            // 检查缓存
            if (_enableCache && _cache.ContainsKey(type))
                return true;

            // 检查是否可以创建
            return CanResolveCore(type);
        }

        #endregion

        #region 抽象方法 - 子类必须实现

        /// <summary>
        /// 获取指定类型的格式化器 (核心实现)
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>格式化器实例，如果未找到返回 null</returns>
        protected abstract IFormatter<T> GetFormatterCore<T>();

        #endregion

        #region 虚方法 - 子类可选重写

        /// <summary>
        /// 获取指定类型的格式化器 (非泛型核心实现)
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>格式化器实例，如果未找到返回 null</returns>
        protected virtual IFormatter GetFormatterCore(Type type)
        {
            // 默认实现：通过反射调用泛型方法
            var method = typeof(FormatterResolverBase).GetMethod(
                nameof(GetFormatterCore),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(type);
                return genericMethod.Invoke(this, null) as IFormatter;
            }

            return null;
        }

        /// <summary>
        /// 检查是否可以解析指定类型 (核心实现)
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果可以解析返回 true</returns>
        protected virtual bool CanResolveCore(Type type)
        {
            return GetFormatter(type) != null;
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 清除缓存
        /// </summary>
        protected void ClearCache()
        {
            _cache?.Clear();
        }

        /// <summary>
        /// 获取缓存数量
        /// </summary>
        protected int CacheCount => _cache?.Count ?? 0;

        /// <summary>
        /// 从缓存中移除指定类型
        /// </summary>
        /// <param name="type">要移除的类型</param>
        /// <returns>如果成功移除返回 true</returns>
        protected bool RemoveFromCache(Type type)
        {
            return _cache?.TryRemove(type, out _) ?? false;
        }

        #endregion
    }

    /// <summary>
    /// 可注册的格式化器解析器基类
    /// <para>支持动态注册和注销格式化器</para>
    /// </summary>
    public abstract class RegisterableFormatterResolverBase : FormatterResolverBase, IRegisterableFormatterResolver
    {
        #region 字段

        /// <summary>已注册的格式化器</summary>
        private readonly ConcurrentDictionary<Type, object> _registeredFormatters;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建可注册的格式化器解析器基类实例
        /// </summary>
        protected RegisterableFormatterResolverBase() : base(true)
        {
            _registeredFormatters = new ConcurrentDictionary<Type, object>();
        }

        #endregion

        #region IRegisterableFormatterResolver 实现

        /// <inheritdoc/>
        public void Register<T>(IFormatter<T> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _registeredFormatters[typeof(T)] = formatter;
            ClearCache();
        }

        /// <inheritdoc/>
        public void Register(Type type, IFormatter formatter)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _registeredFormatters[type] = formatter;
            ClearCache();
        }

        /// <inheritdoc/>
        public bool Unregister<T>()
        {
            return Unregister(typeof(T));
        }

        /// <inheritdoc/>
        public bool Unregister(Type type)
        {
            if (type == null)
                return false;

            var removed = _registeredFormatters.TryRemove(type, out _);
            if (removed)
                ClearCache();

            return removed;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _registeredFormatters.Clear();
            ClearCache();
        }

        /// <inheritdoc/>
        public int RegisteredCount => _registeredFormatters.Count;

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected sealed override IFormatter<T> GetFormatterCore<T>()
        {
            // 首先检查已注册的格式化器
            if (_registeredFormatters.TryGetValue(typeof(T), out var registered))
            {
                return registered as IFormatter<T>;
            }

            // 调用子类实现
            return GetFormatterCoreInternal<T>();
        }

        /// <inheritdoc/>
        protected sealed override IFormatter GetFormatterCore(Type type)
        {
            // 首先检查已注册的格式化器
            if (_registeredFormatters.TryGetValue(type, out var registered))
            {
                return registered as IFormatter;
            }

            // 调用基类实现
            return base.GetFormatterCore(type);
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 获取格式化器的内部实现
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>格式化器实例</returns>
        protected abstract IFormatter<T> GetFormatterCoreInternal<T>();

        #endregion
    }

    /// <summary>
    /// 组合格式化器解析器
    /// <para>支持多个解析器链式组合</para>
    /// </summary>
    public class CompositeFormatterResolver : FormatterResolverBase, ICompositeFormatterResolver
    {
        #region 字段

        /// <summary>子解析器列表</summary>
        private readonly System.Collections.Generic.List<IFormatterResolver> _resolvers;

        /// <summary>解析器锁</summary>
        private readonly object _resolverLock = new object();

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建组合格式化器解析器
        /// </summary>
        public CompositeFormatterResolver() : base(true)
        {
            _resolvers = new System.Collections.Generic.List<IFormatterResolver>();
        }

        /// <summary>
        /// 创建组合格式化器解析器
        /// </summary>
        /// <param name="resolvers">子解析器</param>
        public CompositeFormatterResolver(params IFormatterResolver[] resolvers) : this()
        {
            if (resolvers != null)
                _resolvers.AddRange(resolvers);
        }

        #endregion

        #region ICompositeFormatterResolver 实现

        /// <inheritdoc/>
        public void AddResolver(IFormatterResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            lock (_resolverLock)
            {
                _resolvers.Add(resolver);
            }

            ClearCache();
        }

        /// <inheritdoc/>
        public bool RemoveResolver(IFormatterResolver resolver)
        {
            if (resolver == null)
                return false;

            bool removed;
            lock (_resolverLock)
            {
                removed = _resolvers.Remove(resolver);
            }

            if (removed)
                ClearCache();

            return removed;
        }

        /// <inheritdoc/>
        public IFormatterResolver[] GetResolvers()
        {
            lock (_resolverLock)
            {
                return _resolvers.ToArray();
            }
        }

        /// <inheritdoc/>
        public int ResolverCount
        {
            get
            {
                lock (_resolverLock)
                {
                    return _resolvers.Count;
                }
            }
        }

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected override IFormatter<T> GetFormatterCore<T>()
        {
            lock (_resolverLock)
            {
                foreach (var resolver in _resolvers)
                {
                    var formatter = resolver.GetFormatter<T>();
                    if (formatter != null)
                        return formatter;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        protected override IFormatter GetFormatterCore(Type type)
        {
            lock (_resolverLock)
            {
                foreach (var resolver in _resolvers)
                {
                    var formatter = resolver.GetFormatter(type);
                    if (formatter != null)
                        return formatter;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        protected override bool CanResolveCore(Type type)
        {
            lock (_resolverLock)
            {
                foreach (var resolver in _resolvers)
                {
                    if (resolver.CanResolve(type))
                        return true;
                }
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// 静态格式化器解析器
    /// <para>使用静态字典存储格式化器，适用于全局单例场景</para>
    /// </summary>
    public sealed class StaticFormatterResolver : IFormatterResolver, IRegisterableFormatterResolver
    {
        #region 单例

        /// <summary>默认实例</summary>
        public static readonly StaticFormatterResolver Instance = new StaticFormatterResolver();

        #endregion

        #region 字段

        /// <summary>格式化器缓存</summary>
        private static readonly ConcurrentDictionary<Type, object> _formatters
            = new ConcurrentDictionary<Type, object>();

        #endregion

        #region 构造函数

        private StaticFormatterResolver()
        {
        }

        #endregion

        #region IFormatterResolver 实现

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFormatter<T> GetFormatter<T>()
        {
            if (_formatters.TryGetValue(typeof(T), out var formatter))
                return formatter as IFormatter<T>;

            return null;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFormatter GetFormatter(Type type)
        {
            if (_formatters.TryGetValue(type, out var formatter))
                return formatter as IFormatter;

            return null;
        }

        /// <inheritdoc/>
        public bool TryGetFormatter<T>(out IFormatter<T> formatter)
        {
            formatter = GetFormatter<T>();
            return formatter != null;
        }

        /// <inheritdoc/>
        public bool TryGetFormatter(Type type, out IFormatter formatter)
        {
            formatter = GetFormatter(type);
            return formatter != null;
        }

        /// <inheritdoc/>
        public bool CanResolve(Type type)
        {
            return _formatters.ContainsKey(type);
        }

        #endregion

        #region IRegisterableFormatterResolver 实现

        /// <inheritdoc/>
        public void Register<T>(IFormatter<T> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _formatters[typeof(T)] = formatter;
        }

        /// <inheritdoc/>
        public void Register(Type type, IFormatter formatter)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _formatters[type] = formatter;
        }

        /// <inheritdoc/>
        public bool Unregister<T>()
        {
            return _formatters.TryRemove(typeof(T), out _);
        }

        /// <inheritdoc/>
        public bool Unregister(Type type)
        {
            return _formatters.TryRemove(type, out _);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _formatters.Clear();
        }

        /// <inheritdoc/>
        public int RegisteredCount => _formatters.Count;

        #endregion
    }
}
