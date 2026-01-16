// ==========================================================
// 文件名：CollectionRegistrationExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Linq
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.DI
{
    /// <summary>
    /// 集合注册扩展方法
    /// <para>提供集合解析的便捷注册方式</para>
    /// </summary>
    public static class CollectionRegistrationExtensions
    {
        /// <summary>
        /// 注册集合解析器
        /// <para>允许解析同一接口的所有实现</para>
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <returns>注册构建器</returns>
        /// <example>
        /// <code>
        /// // 注册多个实现
        /// builder.Register&lt;IPlugin, PluginA&gt;();
        /// builder.Register&lt;IPlugin, PluginB&gt;();
        /// builder.Register&lt;IPlugin, PluginC&gt;();
        /// builder.RegisterCollection&lt;IPlugin&gt;();
        /// 
        /// // 使用时
        /// public class PluginManager
        /// {
        ///     private readonly IEnumerable&lt;IPlugin&gt; _plugins;
        ///     
        ///     public PluginManager(IEnumerable&lt;IPlugin&gt; plugins)
        ///     {
        ///         _plugins = plugins;
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterCollection<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<CollectionResolver<T>>(resolver => 
                new CollectionResolver<T>(resolver))
                .Singleton();
        }

        /// <summary>
        /// 注册 IEnumerable&lt;T&gt; 解析
        /// </summary>
        public static IRegistrationBuilder RegisterEnumerable<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IEnumerable<T>>(resolver => 
                resolver.ResolveAll<T>())
                .Transient();
        }

        /// <summary>
        /// 注册 IReadOnlyList&lt;T&gt; 解析
        /// </summary>
        public static IRegistrationBuilder RegisterReadOnlyList<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IReadOnlyList<T>>(resolver => 
                resolver.ResolveAll<T>().ToList())
                .Transient();
        }

        /// <summary>
        /// 注册 T[] 数组解析
        /// </summary>
        public static IRegistrationBuilder RegisterArray<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<T[]>(resolver => 
                resolver.ResolveAll<T>().ToArray())
                .Transient();
        }

        /// <summary>
        /// 批量注册多个实现到同一接口
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <param name="implementationTypes">实现类型数组</param>
        /// <param name="lifetime">生命周期</param>
        public static IContainerBuilder RegisterMultiple<TInterface>(
            this IContainerBuilder builder,
            Type[] implementationTypes,
            Lifetime lifetime = Lifetime.Transient)
            where TInterface : class
        {
            foreach (var implType in implementationTypes)
            {
                if (!typeof(TInterface).IsAssignableFrom(implType))
                {
                    throw new ArgumentException(
                        $"类型 {implType.Name} 未实现接口 {typeof(TInterface).Name}");
                }
                builder.Register(typeof(TInterface), implType).WithLifetime(lifetime);
            }
            return builder;
        }
    }
}
