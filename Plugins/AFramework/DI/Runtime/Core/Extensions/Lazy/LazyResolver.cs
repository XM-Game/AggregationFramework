// ==========================================================
// 文件名：LazyResolver.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// Lazy 解析器
    /// <para>提供延迟解析服务的能力，用于打破循环依赖或延迟初始化</para>
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 打破循环依赖
    /// - 延迟初始化昂贵的服务
    /// - 按需解析服务
    /// </remarks>
    public sealed class LazyResolver<T> : Lazy<T> where T : class
    {
        /// <summary>
        /// 创建 Lazy 解析器
        /// </summary>
        /// <param name="resolver">对象解析器</param>
        public LazyResolver(IObjectResolver resolver)
            : base(() => resolver.Resolve<T>())
        {
        }

        /// <summary>
        /// 创建带键值的 Lazy 解析器
        /// </summary>
        /// <param name="resolver">对象解析器</param>
        /// <param name="key">服务键值</param>
        public LazyResolver(IObjectResolver resolver, object key)
            : base(() => resolver.ResolveKeyed<T>(key))
        {
        }
    }

    /// <summary>
    /// Lazy 解析器工厂
    /// <para>用于创建 Lazy&lt;T&gt; 实例</para>
    /// </summary>
    public static class LazyResolverFactory
    {
        /// <summary>
        /// 创建 Lazy 解析器
        /// </summary>
        public static Lazy<T> Create<T>(IObjectResolver resolver) where T : class
        {
            return new LazyResolver<T>(resolver);
        }

        /// <summary>
        /// 创建带键值的 Lazy 解析器
        /// </summary>
        public static Lazy<T> CreateKeyed<T>(IObjectResolver resolver, object key) where T : class
        {
            return new LazyResolver<T>(resolver, key);
        }
    }
}
