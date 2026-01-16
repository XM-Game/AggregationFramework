// ==========================================================
// 文件名：ContainerBuilderExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 提供容器构建器的扩展方法，简化常用注册模式

// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AFramework.DI
{
    /// <summary>
    /// 容器构建器扩展方法
    /// <para>提供便捷的注册方法，简化常用的依赖注入配置模式</para>
    /// <para>Extension methods for container builder that simplify common registration patterns</para>
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        #region 单例注册扩展 / Singleton Registration Extensions

        /// <summary>
        /// 注册单例服务
        /// <para>Register a singleton service</para>
        /// </summary>
        /// <typeparam name="T">服务类型 / Service type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>注册构建器 / Registration builder</returns>
        public static IRegistrationBuilder RegisterSingleton<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>().Singleton();
        }

        /// <summary>
        /// 注册单例服务（接口-实现映射）
        /// <para>Register a singleton service with interface-implementation mapping</para>
        /// </summary>
        public static IRegistrationBuilder RegisterSingleton<TInterface, TImplementation>(this IContainerBuilder builder)
            where TImplementation : TInterface
        {
            return builder.Register<TInterface, TImplementation>().Singleton();
        }

        /// <summary>
        /// 注册单例实例
        /// <para>Register a singleton instance</para>
        /// </summary>
        public static IRegistrationBuilder RegisterSingletonInstance<T>(this IContainerBuilder builder, T instance)
            where T : class
        {
            return builder.RegisterInstance(instance).Singleton();
        }

        /// <summary>
        /// 注册单例工厂
        /// <para>Register a singleton factory</para>
        /// </summary>
        public static IRegistrationBuilder RegisterSingletonFactory<T>(this IContainerBuilder builder, 
            Func<IObjectResolver, T> factory) where T : class
        {
            return builder.RegisterFactory(factory).Singleton();
        }

        #endregion

        #region 作用域注册扩展 / Scoped Registration Extensions

        /// <summary>
        /// 注册作用域服务
        /// <para>Register a scoped service</para>
        /// </summary>
        public static IRegistrationBuilder RegisterScoped<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>().Scoped();
        }

        /// <summary>
        /// 注册作用域服务（接口-实现映射）
        /// <para>Register a scoped service with interface-implementation mapping</para>
        /// </summary>
        public static IRegistrationBuilder RegisterScoped<TInterface, TImplementation>(this IContainerBuilder builder)
            where TImplementation : TInterface
        {
            return builder.Register<TInterface, TImplementation>().Scoped();
        }

        /// <summary>
        /// 注册作用域工厂
        /// <para>Register a scoped factory</para>
        /// </summary>
        public static IRegistrationBuilder RegisterScopedFactory<T>(this IContainerBuilder builder,
            Func<IObjectResolver, T> factory) where T : class
        {
            return builder.RegisterFactory(factory).Scoped();
        }

        #endregion

        #region 瞬态注册扩展 / Transient Registration Extensions

        /// <summary>
        /// 注册瞬态服务
        /// <para>Register a transient service</para>
        /// </summary>
        public static IRegistrationBuilder RegisterTransient<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>().Transient();
        }

        /// <summary>
        /// 注册瞬态服务（接口-实现映射）
        /// <para>Register a transient service with interface-implementation mapping</para>
        /// </summary>
        public static IRegistrationBuilder RegisterTransient<TInterface, TImplementation>(this IContainerBuilder builder)
            where TImplementation : TInterface
        {
            return builder.Register<TInterface, TImplementation>().Transient();
        }

        /// <summary>
        /// 注册瞬态工厂
        /// <para>Register a transient factory</para>
        /// </summary>
        public static IRegistrationBuilder RegisterTransientFactory<T>(this IContainerBuilder builder,
            Func<IObjectResolver, T> factory) where T : class
        {
            return builder.RegisterFactory(factory).Transient();
        }

        #endregion

        #region 批量注册扩展 / Batch Registration Extensions

        /// <summary>
        /// 从程序集注册所有实现指定接口的类型
        /// <para>Register all types implementing the specified interface from an assembly</para>
        /// </summary>
        /// <typeparam name="TInterface">接口类型 / Interface type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="assembly">程序集 / Assembly</param>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder RegisterAssemblyTypes<TInterface>(
            this IContainerBuilder builder,
            Assembly assembly,
            Lifetime lifetime = Lifetime.Transient)
        {
            return RegisterAssemblyTypes(builder, assembly, typeof(TInterface), lifetime);
        }

        /// <summary>
        /// 从程序集注册所有实现指定接口的类型
        /// <para>Register all types implementing the specified interface from an assembly</para>
        /// </summary>
        public static IContainerBuilder RegisterAssemblyTypes(
            this IContainerBuilder builder,
            Assembly assembly,
            Type interfaceType,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => interfaceType.IsAssignableFrom(t));

            foreach (var type in types)
            {
                builder.Register(interfaceType, type).WithLifetime(lifetime);
            }

            return builder;
        }

        /// <summary>
        /// 从程序集注册所有带有指定特性的类型
        /// <para>Register all types with the specified attribute from an assembly</para>
        /// </summary>
        public static IContainerBuilder RegisterAssemblyTypesWithAttribute<TAttribute>(
            this IContainerBuilder builder,
            Assembly assembly,
            Lifetime lifetime = Lifetime.Transient)
            where TAttribute : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttribute<TAttribute>() != null);

            foreach (var type in types)
            {
                builder.Register(type).AsSelfAndImplementedInterfaces().WithLifetime(lifetime);
            }

            return builder;
        }

        /// <summary>
        /// 注册多个类型
        /// <para>Register multiple types</para>
        /// </summary>
        public static IContainerBuilder RegisterTypes(
            this IContainerBuilder builder,
            IEnumerable<Type> types,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                builder.Register(type).AsSelfAndImplementedInterfaces().WithLifetime(lifetime);
            }

            return builder;
        }

        #endregion

        #region 键值注册扩展 / Keyed Registration Extensions

        /// <summary>
        /// 注册带键值的服务
        /// <para>Register a keyed service</para>
        /// </summary>
        public static IRegistrationBuilder RegisterKeyed<TInterface, TImplementation>(
            this IContainerBuilder builder,
            object key)
            where TImplementation : TInterface
        {
            return builder.Register<TInterface, TImplementation>().Keyed(key);
        }

        /// <summary>
        /// 注册带枚举键值的服务
        /// <para>Register a service with enum key</para>
        /// </summary>
        public static IRegistrationBuilder RegisterKeyed<TInterface, TImplementation, TKey>(
            this IContainerBuilder builder,
            TKey key)
            where TImplementation : TInterface
            where TKey : Enum
        {
            return builder.Register<TInterface, TImplementation>().Keyed(key);
        }

        #endregion

        #region 装饰器注册扩展 / Decorator Registration Extensions

        /// <summary>
        /// 注册装饰器
        /// <para>Register a decorator</para>
        /// </summary>
        /// <typeparam name="TInterface">服务接口 / Service interface</typeparam>
        /// <typeparam name="TDecorator">装饰器类型 / Decorator type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>注册构建器 / Registration builder</returns>
        /// <remarks>
        /// 装饰器模式允许在不修改原有实现的情况下扩展功能。
        /// 装饰器必须接受 TInterface 作为构造函数参数。
        /// </remarks>
        public static IRegistrationBuilder RegisterDecorator<TInterface, TDecorator>(
            this IContainerBuilder builder)
            where TInterface : class
            where TDecorator : class, TInterface
        {
            // 装饰器需要特殊处理，使用工厂模式
            return builder.RegisterFactory<TInterface>(resolver =>
            {
                // 获取被装饰的实例
                // 注意：这里需要确保原始服务已注册
                var decorated = resolver.Resolve<TInterface>();
                
                // 创建装饰器实例
                return resolver.Instantiate<TDecorator>(new IInjectParameter[]
                {
                    new TypedParameter(typeof(TInterface), decorated)
                });
            });
        }

        #endregion

        #region 条件注册扩展 / Conditional Registration Extensions

        /// <summary>
        /// 条件注册（当条件为真时注册）
        /// <para>Register when condition is true</para>
        /// </summary>
        public static IContainerBuilder RegisterIf<T>(
            this IContainerBuilder builder,
            bool condition,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (condition)
            {
                builder.Register<T>().WithLifetime(lifetime);
            }
            return builder;
        }

        /// <summary>
        /// 条件注册（当条件为真时注册）
        /// <para>Register when condition is true</para>
        /// </summary>
        public static IContainerBuilder RegisterIf<TInterface, TImplementation>(
            this IContainerBuilder builder,
            bool condition,
            Lifetime lifetime = Lifetime.Transient)
            where TImplementation : TInterface
        {
            if (condition)
            {
                builder.Register<TInterface, TImplementation>().WithLifetime(lifetime);
            }
            return builder;
        }

        /// <summary>
        /// 条件注册（使用谓词）
        /// <para>Register when predicate returns true</para>
        /// </summary>
        public static IContainerBuilder RegisterIf<T>(
            this IContainerBuilder builder,
            Func<bool> predicate,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (predicate())
            {
                builder.Register<T>().WithLifetime(lifetime);
            }
            return builder;
        }

        #endregion

        #region 自动注册扩展 / Auto Registration Extensions

        /// <summary>
        /// 自动注册类型及其所有实现的接口
        /// <para>Auto register type with all implemented interfaces</para>
        /// </summary>
        public static IRegistrationBuilder RegisterAuto<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>().AsSelfAndImplementedInterfaces();
        }

        /// <summary>
        /// 自动注册单例类型及其所有实现的接口
        /// <para>Auto register singleton type with all implemented interfaces</para>
        /// </summary>
        public static IRegistrationBuilder RegisterAutoSingleton<T>(this IContainerBuilder builder)
        {
            return builder.Register<T>().AsSelfAndImplementedInterfaces().Singleton();
        }

        #endregion

        #region 泛型注册扩展 / Generic Registration Extensions

        /// <summary>
        /// 注册开放泛型类型
        /// <para>Register open generic type</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="serviceType">开放泛型服务类型 / Open generic service type</param>
        /// <param name="implementationType">开放泛型实现类型 / Open generic implementation type</param>
        /// <param name="lifetime">生命周期 / Lifetime (default: Transient)</param>
        /// <returns>容器构建器 / Container builder</returns>
        /// <remarks>
        /// 示例 / Example：
        /// <code>
        /// // 注册开放泛型
        /// builder.RegisterOpenGeneric(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;));
        /// 
        /// // 解析时自动构造封闭类型
        /// var userRepo = container.Resolve&lt;IRepository&lt;User&gt;&gt;();
        /// var orderRepo = container.Resolve&lt;IRepository&lt;Order&gt;&gt;();
        /// </code>
        /// </remarks>
        public static IContainerBuilder RegisterOpenGeneric(
            this IContainerBuilder builder,
            Type serviceType,
            Type implementationType,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            if (!serviceType.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"服务类型 {serviceType.Name} 必须是开放泛型类型。\n" +
                    $"Service type {serviceType.Name} must be an open generic type.", 
                    nameof(serviceType));
            if (!implementationType.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"实现类型 {implementationType.Name} 必须是开放泛型类型。\n" +
                    $"Implementation type {implementationType.Name} must be an open generic type.", 
                    nameof(implementationType));

            // 验证泛型参数数量匹配
            var serviceGenericArgs = serviceType.GetGenericArguments();
            var implGenericArgs = implementationType.GetGenericArguments();
            if (serviceGenericArgs.Length != implGenericArgs.Length)
            {
                throw new ArgumentException(
                    $"服务类型和实现类型的泛型参数数量不匹配。\n" +
                    $"Generic argument count mismatch between service and implementation types.");
            }

            // 使用 ContainerBuilder 的内部方法注册开放泛型
            if (builder is ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterOpenGenericInternal(serviceType, implementationType, lifetime);
            }
            else
            {
                throw new NotSupportedException(
                    "当前容器构建器不支持开放泛型注册。\n" +
                    "Current container builder does not support open generic registration.");
            }

            return builder;
        }

        /// <summary>
        /// 注册开放泛型类型（泛型方法）
        /// <para>Register open generic type (generic method)</para>
        /// </summary>
        /// <typeparam name="TService">开放泛型服务类型 / Open generic service type</typeparam>
        /// <typeparam name="TImplementation">开放泛型实现类型 / Open generic implementation type</typeparam>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <param name="lifetime">生命周期 / Lifetime (default: Transient)</param>
        /// <returns>容器构建器 / Container builder</returns>
        public static IContainerBuilder RegisterOpenGeneric<TService, TImplementation>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
        {
            return RegisterOpenGeneric(builder, typeof(TService), typeof(TImplementation), lifetime);
        }

        /// <summary>
        /// 注册开放泛型单例
        /// <para>Register open generic singleton</para>
        /// </summary>
        public static IContainerBuilder RegisterOpenGenericSingleton(
            this IContainerBuilder builder,
            Type serviceType,
            Type implementationType)
        {
            return RegisterOpenGeneric(builder, serviceType, implementationType, Lifetime.Singleton);
        }

        /// <summary>
        /// 注册开放泛型作用域服务
        /// <para>Register open generic scoped service</para>
        /// </summary>
        public static IContainerBuilder RegisterOpenGenericScoped(
            this IContainerBuilder builder,
            Type serviceType,
            Type implementationType)
        {
            return RegisterOpenGeneric(builder, serviceType, implementationType, Lifetime.Scoped);
        }

        #endregion

        #region 模块注册扩展 / Module Registration Extensions

        /// <summary>
        /// 使用配置委托注册服务
        /// <para>Register services using configuration delegate</para>
        /// </summary>
        public static IContainerBuilder Configure(
            this IContainerBuilder builder,
            Action<IContainerBuilder> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            configuration(builder);
            return builder;
        }

        /// <summary>
        /// 使用多个配置委托注册服务
        /// <para>Register services using multiple configuration delegates</para>
        /// </summary>
        public static IContainerBuilder Configure(
            this IContainerBuilder builder,
            params Action<IContainerBuilder>[] configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            foreach (var config in configurations)
            {
                config?.Invoke(builder);
            }
            return builder;
        }

        #endregion

        #region 验证扩展 / Validation Extensions

        /// <summary>
        /// 验证所有注册是否可以解析
        /// <para>Validate that all registrations can be resolved</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <returns>验证结果 / Validation result</returns>
        public static ValidationResult ValidateRegistrations(this IContainerBuilder builder)
        {
            var errors = new List<string>();
            var registrations = builder.GetRegistrations();

            foreach (var registration in registrations)
            {
                if (registration == null) continue;

                // 检查实现类型是否可实例化
                var implType = registration.ImplementationType;
                if (implType != null)
                {
                    if (implType.IsAbstract || implType.IsInterface)
                    {
                        errors.Add($"类型 {implType.Name} 无法实例化（抽象类或接口）");
                    }

                    // 检查是否有可用的构造函数
                    var constructors = implType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                    if (constructors.Length == 0)
                    {
                        errors.Add($"类型 {implType.Name} 没有公共构造函数");
                    }
                }
            }

            return new ValidationResult(errors.Count == 0, errors);
        }

        #endregion
    }

    #region 验证结果 / Validation Result

    /// <summary>
    /// 注册验证结果
    /// <para>Registration validation result</para>
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// 获取验证是否成功
        /// <para>Get whether validation succeeded</para>
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// 获取错误列表
        /// <para>Get error list</para>
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// 创建验证结果实例
        /// </summary>
        public ValidationResult(bool isValid, IEnumerable<string> errors = null)
        {
            IsValid = isValid;
            Errors = errors?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// 如果验证失败则抛出异常
        /// <para>Throw exception if validation failed</para>
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                throw new RegistrationException(
                    "注册验证失败 / Registration validation failed:\n" +
                    string.Join("\n", Errors));
            }
        }
    }

    #endregion
}
