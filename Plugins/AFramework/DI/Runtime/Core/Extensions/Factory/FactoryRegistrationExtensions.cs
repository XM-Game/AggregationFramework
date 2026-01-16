// ==========================================================
// 文件名：FactoryRegistrationExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 工厂注册扩展方法
    /// <para>提供 IFactory&lt;T&gt; 的便捷注册方式</para>
    /// </summary>
    public static class FactoryRegistrationExtensions
    {
        #region 无参数工厂

        /// <summary>
        /// 注册工厂 IFactory&lt;T&gt;
        /// <para>允许在运行时创建 T 的新实例</para>
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <returns>注册构建器</returns>
        /// <example>
        /// <code>
        /// builder.RegisterFactory&lt;Enemy&gt;();
        /// 
        /// // 使用时
        /// public class EnemySpawner
        /// {
        ///     private readonly IFactory&lt;Enemy&gt; _enemyFactory;
        ///     
        ///     public EnemySpawner(IFactory&lt;Enemy&gt; enemyFactory)
        ///     {
        ///         _enemyFactory = enemyFactory;
        ///     }
        ///     
        ///     public Enemy SpawnEnemy()
        ///     {
        ///         return _enemyFactory.Create();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterFactory<T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IFactory<T>>(resolver => 
                new Factory<T>(resolver))
                .Singleton();
        }

        #endregion

        #region 带参数工厂

        /// <summary>
        /// 注册带参数的工厂 IFactory&lt;TParam, T&gt;
        /// </summary>
        public static IRegistrationBuilder RegisterFactory<TParam, T>(this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IFactory<TParam, T>>(resolver => 
                new Factory<TParam, T>(resolver))
                .Singleton();
        }

        /// <summary>
        /// 注册带两个参数的工厂 IFactory&lt;TParam1, TParam2, T&gt;
        /// </summary>
        public static IRegistrationBuilder RegisterFactory<TParam1, TParam2, T>(
            this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IFactory<TParam1, TParam2, T>>(resolver => 
                new Factory<TParam1, TParam2, T>(resolver))
                .Singleton();
        }

        /// <summary>
        /// 注册带三个参数的工厂 IFactory&lt;TParam1, TParam2, TParam3, T&gt;
        /// </summary>
        public static IRegistrationBuilder RegisterFactory<TParam1, TParam2, TParam3, T>(
            this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<IFactory<TParam1, TParam2, TParam3, T>>(resolver => 
                new Factory<TParam1, TParam2, TParam3, T>(resolver))
                .Singleton();
        }

        #endregion

        #region 自动注册

        /// <summary>
        /// 注册类型并自动注册其工厂
        /// <para>同时注册 T 和 IFactory&lt;T&gt;</para>
        /// </summary>
        public static IContainerBuilder RegisterWithFactory<T>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
            where T : class
        {
            builder.Register<T>().WithLifetime(lifetime);
            builder.RegisterFactory<T>();
            return builder;
        }

        /// <summary>
        /// 注册类型并自动注册其工厂（接口-实现映射）
        /// </summary>
        public static IContainerBuilder RegisterWithFactory<TInterface, TImplementation>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            builder.Register<TInterface, TImplementation>().WithLifetime(lifetime);
            builder.RegisterFactory<TImplementation>();
            return builder;
        }

        #endregion
    }
}
