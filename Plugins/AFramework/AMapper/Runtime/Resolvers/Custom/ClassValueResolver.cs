// ==========================================================
// 文件名：ClassValueResolver.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 类值解析器包装，将类型解析器包装为实例解析器
// ==========================================================

using System;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 类值解析器包装
    /// <para>将类型解析器包装为实例解析器，支持延迟创建和依赖注入</para>
    /// <para>Class value resolver wrapper that wraps type resolver as instance resolver</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <typeparam name="TResolver">解析器类型 / Resolver type</typeparam>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>代理模式：包装解析器类型，延迟创建实例</item>
    /// <item>单一职责：仅负责解析器实例的创建和调用</item>
    /// <item>依赖注入：支持从容器获取解析器实例</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 配置
    /// CreateMap&lt;Person, PersonDto&gt;()
    ///     .ForMember(d => d.FullName, opt => opt.MapFrom&lt;FullNameResolver&gt;());
    /// 
    /// // 内部使用 ClassValueResolver 包装 FullNameResolver
    /// </code>
    /// </remarks>
    public sealed class ClassValueResolver<TSource, TDestination, TDestMember, TResolver> :
        ValueResolverBase<TSource, TDestination, TDestMember>
        where TResolver : class, IValueResolver<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private TResolver _resolver;
        private readonly object _lock = new object();

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        protected override TDestMember Resolve(
            TSource source,
            TDestination destination,
            TDestMember destMember,
            ResolutionContext context)
        {
            // 延迟创建解析器实例
            if (_resolver == null)
            {
                lock (_lock)
                {
                    if (_resolver == null)
                    {
                        _resolver = CreateResolverInstance(context);
                    }
                }
            }

            // 调用解析器
            return _resolver.Resolve(source, destination, destMember, context);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建解析器实例
        /// </summary>
        private TResolver CreateResolverInstance(ResolutionContext context)
        {
            // 尝试从上下文的服务提供者获取
            var serviceProvider = context?.ServiceProvider;
            if (serviceProvider != null)
            {
                var resolver = serviceProvider.GetService(typeof(TResolver)) as TResolver;
                if (resolver != null)
                {
                    return resolver;
                }
            }

            // 使用工厂创建
            return ResolverFactory.CreateResolver<TResolver>(serviceProvider);
        }

        #endregion
    }

    /// <summary>
    /// 类成员值解析器包装
    /// <para>Class member value resolver wrapper</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <typeparam name="TResolver">解析器类型 / Resolver type</typeparam>
    public sealed class ClassMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember, TResolver> :
        MemberValueResolverBase<TSource, TDestination, TSourceMember, TDestMember>
        where TResolver : class, IMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember>
    {
        #region 私有字段 / Private Fields

        private TResolver _resolver;
        private readonly object _lock = new object();

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        protected override TDestMember Resolve(
            TSource source,
            TDestination destination,
            TSourceMember sourceMember,
            TDestMember destMember,
            ResolutionContext context)
        {
            // 延迟创建解析器实例
            if (_resolver == null)
            {
                lock (_lock)
                {
                    if (_resolver == null)
                    {
                        _resolver = CreateResolverInstance(context);
                    }
                }
            }

            // 调用解析器
            return _resolver.Resolve(source, destination, sourceMember, destMember, context);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建解析器实例
        /// </summary>
        private TResolver CreateResolverInstance(ResolutionContext context)
        {
            // 尝试从上下文的服务提供者获取
            var serviceProvider = context?.ServiceProvider;
            if (serviceProvider != null)
            {
                var resolver = serviceProvider.GetService(typeof(TResolver)) as TResolver;
                if (resolver != null)
                {
                    return resolver;
                }
            }

            // 使用工厂创建
            return ResolverFactory.CreateResolver<TResolver>(serviceProvider);
        }

        #endregion
    }

    /// <summary>
    /// 类值解析器包装（非泛型版本）
    /// <para>Class value resolver wrapper (non-generic version)</para>
    /// </summary>
    /// <remarks>
    /// 用于运行时动态创建解析器的场景。
    /// </remarks>
    public sealed class ClassValueResolver : IValueResolver
    {
        #region 私有字段 / Private Fields

        private readonly Type _resolverType;
        private IValueResolver _resolver;
        private readonly object _lock = new object();

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建类值解析器包装
        /// </summary>
        /// <param name="resolverType">解析器类型 / Resolver type</param>
        /// <exception cref="ArgumentNullException">当 resolverType 为 null 时抛出</exception>
        /// <exception cref="ArgumentException">当 resolverType 不实现 IValueResolver 时抛出</exception>
        public ClassValueResolver(Type resolverType)
        {
            _resolverType = resolverType ?? throw new ArgumentNullException(nameof(resolverType));

            if (!typeof(IValueResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException(
                    $"解析器类型必须实现 IValueResolver 接口。类型：{resolverType.Name}",
                    nameof(resolverType));
            }
        }

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        public object Resolve(
            object source,
            object destination,
            object destMember,
            ResolutionContext context)
        {
            // 延迟创建解析器实例
            if (_resolver == null)
            {
                lock (_lock)
                {
                    if (_resolver == null)
                    {
                        _resolver = CreateResolverInstance(context);
                    }
                }
            }

            // 调用解析器
            return _resolver.Resolve(source, destination, destMember, context);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 创建解析器实例
        /// </summary>
        private IValueResolver CreateResolverInstance(ResolutionContext context)
        {
            // 尝试从上下文的服务提供者获取
            var serviceProvider = context?.ServiceProvider;
            if (serviceProvider != null)
            {
                var resolver = serviceProvider.GetService(_resolverType) as IValueResolver;
                if (resolver != null)
                {
                    return resolver;
                }
            }

            // 使用工厂创建
            var instance = ResolverFactory.CreateResolver(_resolverType, serviceProvider);
            if (instance is IValueResolver valueResolver)
            {
                return valueResolver;
            }

            throw new InvalidOperationException(
                $"创建的解析器实例不是 IValueResolver 类型。类型：{_resolverType.Name}");
        }

        #endregion
    }
}
