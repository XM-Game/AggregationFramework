// ==========================================================
// 文件名：KeyedRegistrationExtensions.cs
// 命名空间: AFramework.DI
// 依赖: System
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 键值注册扩展方法
    /// <para>提供键值解析的便捷注册方式</para>
    /// </summary>
    public static class KeyedRegistrationExtensions
    {
        /// <summary>
        /// 注册键值解析器
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <returns>注册构建器</returns>
        /// <example>
        /// <code>
        /// // 注册带键值的服务
        /// builder.Register&lt;IDatabase, SqlDatabase&gt;().Keyed("sql");
        /// builder.Register&lt;IDatabase, MongoDatabase&gt;().Keyed("mongo");
        /// builder.RegisterKeyedResolver&lt;string, IDatabase&gt;();
        /// 
        /// // 使用时
        /// public class DataService
        /// {
        ///     private readonly KeyedResolver&lt;string, IDatabase&gt; _dbResolver;
        ///     
        ///     public DataService(KeyedResolver&lt;string, IDatabase&gt; dbResolver)
        ///     {
        ///         _dbResolver = dbResolver;
        ///     }
        ///     
        ///     public void UseDatabase(string dbType)
        ///     {
        ///         var db = _dbResolver.Resolve(dbType);
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IRegistrationBuilder RegisterKeyedResolver<TKey, T>(
            this IContainerBuilder builder)
            where T : class
        {
            return builder.RegisterFactory<KeyedResolver<TKey, T>>(resolver => 
                new KeyedResolver<TKey, T>(resolver))
                .Singleton();
        }

        /// <summary>
        /// 注册枚举键值解析器
        /// </summary>
        public static IRegistrationBuilder RegisterEnumKeyedResolver<TEnum, T>(
            this IContainerBuilder builder)
            where TEnum : Enum
            where T : class
        {
            return builder.RegisterFactory<EnumKeyedResolver<TEnum, T>>(resolver => 
                new EnumKeyedResolver<TEnum, T>(resolver))
                .Singleton();
        }

        /// <summary>
        /// 批量注册枚举键值服务
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <param name="builder">容器构建器</param>
        /// <param name="mappings">枚举值到实现类型的映射</param>
        /// <param name="lifetime">生命周期</param>
        public static IContainerBuilder RegisterEnumKeyed<TEnum, TInterface>(
            this IContainerBuilder builder,
            (TEnum key, Type implementationType)[] mappings,
            Lifetime lifetime = Lifetime.Transient)
            where TEnum : Enum
            where TInterface : class
        {
            foreach (var (key, implType) in mappings)
            {
                if (!typeof(TInterface).IsAssignableFrom(implType))
                {
                    throw new ArgumentException(
                        $"类型 {implType.Name} 未实现接口 {typeof(TInterface).Name}");
                }
                builder.Register(typeof(TInterface), implType)
                    .Keyed(key)
                    .WithLifetime(lifetime);
            }
            return builder;
        }

        /// <summary>
        /// 注册带字符串键值的服务
        /// </summary>
        public static IRegistrationBuilder RegisterKeyed<TInterface, TImplementation>(
            this IContainerBuilder builder,
            string key,
            Lifetime lifetime = Lifetime.Transient)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            return builder.Register<TInterface, TImplementation>()
                .Keyed(key)
                .WithLifetime(lifetime);
        }

        /// <summary>
        /// 注册带枚举键值的服务
        /// </summary>
        public static IRegistrationBuilder RegisterKeyed<TInterface, TImplementation, TEnum>(
            this IContainerBuilder builder,
            TEnum key,
            Lifetime lifetime = Lifetime.Transient)
            where TInterface : class
            where TImplementation : class, TInterface
            where TEnum : Enum
        {
            return builder.Register<TInterface, TImplementation>()
                .Keyed(key)
                .WithLifetime(lifetime);
        }
    }
}
