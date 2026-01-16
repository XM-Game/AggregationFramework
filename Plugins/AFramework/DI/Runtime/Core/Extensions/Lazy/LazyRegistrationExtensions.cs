// ==========================================================
// 文件名：LazyRegistrationExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// Lazy 注册扩展方法
    /// <para>提供 Lazy&lt;T&gt; 的便捷注册方式</para>
    /// </summary>
    public static class LazyRegistrationExtensions
    {
        /// <summary>
        /// 注册 Lazy&lt;T&gt; 服务
        /// <para>允许延迟解析指定类型的服务</para>
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <returns>注册构建器</returns>
        /// <example>
        /// <code>
        /// builder.RegisterLazy&lt;IHeavyService&gt;();
        /// 
        /// // 使用时
        /// public class Consumer
        /// {
        ///     private readonly Lazy&lt;IHeavyService&gt; _lazyService;
        ///     
        ///     public Consumer(Lazy&lt;IHeavyService&gt; lazyService)
        ///     {
        ///         _lazyService = lazyService;
        ///     }
        ///     
        ///     public void DoWork()
        ///     {
        ///         // 首次访问时才解析
        ///         var service = _lazyService.Value;
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterLazy<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<Lazy<T>>(resolver => 
                LazyResolverFactory.Create<T>(resolver))
                .Transient();
        }

        /// <summary>
        /// 注册带键值的 Lazy&lt;T&gt; 服务
        /// </summary>
        public static IRegistrationBuilder RegisterLazyKeyed<T>(
            this IContainerBuilder builder, 
            object key)
            where T : class
        {
            return builder.RegisterFactory<Lazy<T>>(resolver => 
                LazyResolverFactory.CreateKeyed<T>(resolver, key))
                .Keyed(key)
                .Transient();
        }

        /// <summary>
        /// 自动为指定类型注册 Lazy 包装
        /// <para>同时注册 T 和 Lazy&lt;T&gt;</para>
        /// </summary>
        public static IContainerBuilder RegisterWithLazy<T>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
            where T : class
        {
            builder.Register<T>().WithLifetime(lifetime);
            builder.RegisterLazy<T>();
            return builder;
        }

        /// <summary>
        /// 自动为指定类型注册 Lazy 包装（接口-实现映射）
        /// </summary>
        public static IContainerBuilder RegisterWithLazy<TInterface, TImplementation>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            builder.Register<TInterface, TImplementation>().WithLifetime(lifetime);
            builder.RegisterLazy<TInterface>();
            return builder;
        }
    }
}
