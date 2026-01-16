// ==========================================================
// 文件名：ContainerLocalRegistrationExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: ContainerLocal 注册扩展方法
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// ContainerLocal 注册扩展方法
    /// <para>Extension methods for registering ContainerLocal</para>
    /// </summary>
    public static class ContainerLocalRegistrationExtensions
    {
        #region 基础注册 / Basic Registration

        /// <summary>
        /// 注册容器本地值（每个作用域独立实例）
        /// <para>Register container local value (independent instance per scope)</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>注册构建器 / Registration builder</returns>
        /// <remarks>
        /// 每个作用域容器会创建独立的 ContainerLocal 实例，
        /// 子作用域的值修改不会影响父作用域。
        /// </remarks>
        /// <example>
        /// <code>
        /// builder.RegisterContainerLocal&lt;UserContext&gt;();
        /// 
        /// // 使用
        /// var local = container.Resolve&lt;ContainerLocal&lt;UserContext&gt;&gt;();
        /// local.Value = new UserContext();
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterContainerLocal<T>(this IContainerBuilder builder)
        {
            return builder.RegisterFactory(_ => new ContainerLocal<T>()).Scoped();
        }

        /// <summary>
        /// 注册容器本地值（带初始值）
        /// <para>Register container local value with initial value</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="initialValue">初始值 / Initial value</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterContainerLocal<T>(
            this IContainerBuilder builder, 
            T initialValue)
        {
            return builder.RegisterFactory(_ => new ContainerLocal<T>(initialValue)).Scoped();
        }

        /// <summary>
        /// 注册容器本地值（带工厂函数）
        /// <para>Register container local value with factory function</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="factory">值工厂函数 / Value factory function</param>
        /// <returns>注册构建器 / Registration builder</returns>
        /// <remarks>
        /// 工厂函数在首次访问 Value 属性时调用，实现延迟初始化。
        /// </remarks>
        /// <example>
        /// <code>
        /// builder.RegisterContainerLocal&lt;ExpensiveResource&gt;(() => new ExpensiveResource());
        /// 
        /// // 首次访问时才创建
        /// var local = container.Resolve&lt;ContainerLocal&lt;ExpensiveResource&gt;&gt;();
        /// var resource = local.Value; // 此时才调用工厂函数
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterContainerLocal<T>(
            this IContainerBuilder builder, 
            Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return builder.RegisterFactory(_ => new ContainerLocal<T>(factory)).Scoped();
        }

        /// <summary>
        /// 注册容器本地值（带依赖注入的工厂函数）
        /// <para>Register container local value with DI-aware factory function</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="factory">带容器参数的工厂函数 / Factory function with resolver parameter</param>
        /// <returns>注册构建器 / Registration builder</returns>
        /// <remarks>
        /// 工厂函数可以使用容器解析其他依赖来创建值。
        /// </remarks>
        /// <example>
        /// <code>
        /// builder.RegisterContainerLocal&lt;DbContext&gt;(resolver => 
        /// {
        ///     var config = resolver.Resolve&lt;IConfiguration&gt;();
        ///     return new DbContext(config.ConnectionString);
        /// });
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterContainerLocal<T>(
            this IContainerBuilder builder, 
            Func<IObjectResolver, T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return builder.RegisterFactory(resolver => 
                new ContainerLocal<T>(() => factory(resolver))).Scoped();
        }

        #endregion

        #region 单例注册 / Singleton Registration

        /// <summary>
        /// 注册单例容器本地值（所有作用域共享同一实例）
        /// <para>Register singleton container local value (shared across all scopes)</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>注册构建器 / Registration builder</returns>
        /// <remarks>
        /// 注意：单例模式下所有作用域共享同一个 ContainerLocal 实例，
        /// 这意味着值的修改会影响所有作用域。通常应使用 Scoped 生命周期。
        /// </remarks>
        public static IRegistrationBuilder RegisterContainerLocalSingleton<T>(this IContainerBuilder builder)
        {
            return builder.RegisterFactory(_ => new ContainerLocal<T>()).Singleton();
        }

        /// <summary>
        /// 注册单例容器本地值（带初始值）
        /// <para>Register singleton container local value with initial value</para>
        /// </summary>
        public static IRegistrationBuilder RegisterContainerLocalSingleton<T>(
            this IContainerBuilder builder, 
            T initialValue)
        {
            return builder.RegisterFactory(_ => new ContainerLocal<T>(initialValue)).Singleton();
        }

        #endregion

        #region 便捷方法 / Convenience Methods

        /// <summary>
        /// 注册容器本地值并同时注册值类型的解析
        /// <para>Register container local and also register value type resolution</para>
        /// </summary>
        /// <typeparam name="T">值类型 / Value type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="factory">值工厂函数 / Value factory function</param>
        /// <returns>容器构建器 / Container builder</returns>
        /// <remarks>
        /// 这个方法同时注册 ContainerLocal&lt;T&gt; 和 T 的解析，
        /// 解析 T 时会自动从 ContainerLocal&lt;T&gt; 获取值。
        /// </remarks>
        /// <example>
        /// <code>
        /// builder.RegisterContainerLocalWithValue&lt;UserContext&gt;(() => new UserContext());
        /// 
        /// // 可以直接解析 UserContext
        /// var context = container.Resolve&lt;UserContext&gt;();
        /// 
        /// // 也可以解析 ContainerLocal 来修改值
        /// var local = container.Resolve&lt;ContainerLocal&lt;UserContext&gt;&gt;();
        /// local.Value = new UserContext { UserId = 123 };
        /// </code>
        /// </example>
        public static IContainerBuilder RegisterContainerLocalWithValue<T>(
            this IContainerBuilder builder, 
            Func<T> factory) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            // 注册 ContainerLocal<T>
            builder.RegisterFactory(_ => new ContainerLocal<T>(factory)).Scoped();

            // 注册 T 的解析，从 ContainerLocal<T> 获取值
            builder.RegisterFactory<T>(resolver =>
            {
                var local = resolver.Resolve<ContainerLocal<T>>();
                return local.Value;
            }).Scoped();

            return builder;
        }

        /// <summary>
        /// 注册容器本地值并同时注册值类型的解析（带依赖注入）
        /// <para>Register container local and value type resolution with DI</para>
        /// </summary>
        public static IContainerBuilder RegisterContainerLocalWithValue<T>(
            this IContainerBuilder builder, 
            Func<IObjectResolver, T> factory) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            // 注册 ContainerLocal<T>
            builder.RegisterFactory(resolver => 
                new ContainerLocal<T>(() => factory(resolver))).Scoped();

            // 注册 T 的解析
            builder.RegisterFactory<T>(resolver =>
            {
                var local = resolver.Resolve<ContainerLocal<T>>();
                return local.Value;
            }).Scoped();

            return builder;
        }

        #endregion
    }
}
