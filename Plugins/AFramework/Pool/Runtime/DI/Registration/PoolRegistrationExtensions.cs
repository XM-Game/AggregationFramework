// ==========================================================
// 文件名：PoolRegistrationExtensions.cs
// 命名空间: AFramework.Pool.DI
// 依赖: AFramework.DI, AFramework.Pool
// 功能: 对象池依赖注入注册扩展
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.Pool.DI
{
    /// <summary>
    /// 对象池注册扩展方法
    /// Pool Registration Extension Methods
    /// </summary>
    public static class PoolRegistrationExtensions
    {
        #region 泛型对象池注册 Generic Pool Registration

        /// <summary>
        /// 注册对象池
        /// Register object pool
        /// </summary>
        public static IRegistrationBuilder RegisterObjectPool<T>(
            this IContainerBuilder builder,
            IPoolCreationPolicy<T> creationPolicy = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool warmup = false) where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                var policy = creationPolicy ?? new FactoryCreationPolicy<T>(() => Activator.CreateInstance<T>());
                var pool = new ObjectPool<T>(policy, maxCapacity);

                if (warmup)
                {
                    pool.Warmup(initialCapacity);
                }

                return pool;
            }).Scoped();
        }

        /// <summary>
        /// 注册并发对象池
        /// Register concurrent object pool
        /// </summary>
        public static IRegistrationBuilder RegisterConcurrentObjectPool<T>(
            this IContainerBuilder builder,
            IPoolCreationPolicy<T> creationPolicy = null,
            int initialCapacity = 10,
            int maxCapacity = 100) where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                var policy = creationPolicy ?? new FactoryCreationPolicy<T>(() => Activator.CreateInstance<T>());
                return new ConcurrentObjectPool<T>(policy, maxCapacity);
            }).Scoped();
        }

        #endregion

        #region Unity 对象池注册 Unity Pool Registration

#if UNITY_2022_3_OR_NEWER
        /// <summary>
        /// 注册 GameObject 对象池
        /// Register GameObject object pool
        /// </summary>
        public static IRegistrationBuilder RegisterGameObjectPool(
            this IContainerBuilder builder,
            UnityEngine.GameObject prefab,
            UnityEngine.Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool warmup = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory(typeof(GameObjectPool), resolver =>
            {
                var pool = new GameObjectPool(prefab, parent, initialCapacity, maxCapacity);

                if (warmup)
                {
                    pool.Warmup(initialCapacity);
                }

                return pool;
            }).Scoped();
        }

        /// <summary>
        /// 注册组件对象池
        /// Register component object pool
        /// </summary>
        public static IRegistrationBuilder RegisterComponentPool<T>(
            this IContainerBuilder builder,
            T prefab,
            UnityEngine.Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool warmup = false) where T : UnityEngine.Component
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                var pool = new ComponentPool<T>(prefab, parent, initialCapacity, maxCapacity);

                if (warmup)
                {
                    pool.Warmup(initialCapacity);
                }

                return pool;
            }).Scoped();
        }
#endif

        #endregion

        #region 特化池注册 Specialized Pool Registration

        /// <summary>
        /// 注册 StringBuilder 池
        /// Register StringBuilder pool
        /// </summary>
        public static IRegistrationBuilder RegisterStringBuilderPool(
            this IContainerBuilder builder,
            int defaultCapacity = 256,
            int maxRetainedCapacity = 4096)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory(typeof(StringBuilderPool), resolver =>
            {
                return new StringBuilderPool(defaultCapacity, maxRetainedCapacity);
            }).Scoped();
        }

        /// <summary>
        /// 注册 List 池
        /// Register List pool
        /// </summary>
        public static IRegistrationBuilder RegisterListPool<T>(
            this IContainerBuilder builder,
            int defaultCapacity = 16)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactory(typeof(ListPool<T>), resolver =>
            {
                return new ListPool<T>(defaultCapacity);
            }).Scoped();
        }

        #endregion

        #region 流式配置注册 Fluent Configuration Registration

        /// <summary>
        /// 注册对象池（流式配置）
        /// Register object pool (fluent configuration)
        /// </summary>
        public static IRegistrationBuilder RegisterPool<T>(
            this IContainerBuilder builder,
            System.Action<PoolConfigurationBuilder<T>> configure = null) where T : class, new()
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var configBuilder = new PoolConfigurationBuilder<T>();
            configure?.Invoke(configBuilder);

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                return configBuilder.Build();
            }).Singleton();
        }

        /// <summary>
        /// 注册带工厂的对象池（流式配置）
        /// Register object pool with factory (fluent configuration)
        /// </summary>
        public static IRegistrationBuilder RegisterPoolWithFactory<T>(
            this IContainerBuilder builder,
            Func<T> factory,
            System.Action<PoolConfigurationBuilder<T>> configure = null) where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var configBuilder = new PoolConfigurationBuilder<T>();
            configBuilder.WithFactory(factory);
            configure?.Invoke(configBuilder);

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                return configBuilder.Build();
            }).Singleton();
        }

        /// <summary>
        /// 注册带预热的对象池
        /// Register object pool with warmup
        /// </summary>
        public static IRegistrationBuilder RegisterPoolWithWarmup<T>(
            this IContainerBuilder builder,
            Warming.WarmupConfig warmupConfig,
            System.Action<PoolConfigurationBuilder<T>> configure = null) where T : class, new()
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (warmupConfig == null)
                throw new ArgumentNullException(nameof(warmupConfig));

            var configBuilder = new PoolConfigurationBuilder<T>();
            configure?.Invoke(configBuilder);

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                var pool = configBuilder.Build();
                if (warmupConfig.AutoWarmup)
                {
                    pool.Warmup(warmupConfig.Count);
                }
                return pool;
            }).Scoped();
        }

        /// <summary>
        /// 注册使用容器解析的对象池
        /// Register object pool using container resolution
        /// </summary>
        public static IRegistrationBuilder RegisterPoolWithContainerFactory<T>(
            this IContainerBuilder builder,
            System.Action<PoolConfigurationBuilder<T>> configure = null) where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var configBuilder = new PoolConfigurationBuilder<T>();
            configure?.Invoke(configBuilder);

            return builder.RegisterFactory(typeof(IObjectPool<T>), resolver =>
            {
                configBuilder.WithFactory(() => resolver.Resolve<T>());
                return configBuilder.Build();
            }).Scoped();
        }

        /// <summary>
        /// 批量注册对象池
        /// Batch register object pools
        /// </summary>
        public static void RegisterPools(
            this IContainerBuilder builder,
            params Type[] types)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (types == null || types.Length == 0)
                return;

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var poolType = typeof(IObjectPool<>).MakeGenericType(type);
                var objectPoolType = typeof(ObjectPool<>).MakeGenericType(type);

                builder.RegisterFactory(poolType, resolver =>
                {
                    return Activator.CreateInstance(objectPoolType, 
                        new object[] { (Func<object>)(() => Activator.CreateInstance(type)), 100 });
                }).Scoped();
            }
        }

        #endregion
    }

    #region 池配置构建器 Pool Configuration Builder

    /// <summary>
    /// 池配置构建器
    /// Pool Configuration Builder
    /// </summary>
    public class PoolConfigurationBuilder<T> where T : class
    {
        private Func<T> _factory;
        private int _initialCapacity = 10;
        private int _maxCapacity = 100;
        private bool _enableDiagnostics = false;

        /// <summary>
        /// 设置对象工厂
        /// Set object factory
        /// </summary>
        public PoolConfigurationBuilder<T> WithFactory(Func<T> factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        /// 设置初始容量
        /// Set initial capacity
        /// </summary>
        public PoolConfigurationBuilder<T> WithInitialCapacity(int capacity)
        {
            _initialCapacity = capacity;
            return this;
        }

        /// <summary>
        /// 设置最大容量
        /// Set maximum capacity
        /// </summary>
        public PoolConfigurationBuilder<T> WithMaxCapacity(int capacity)
        {
            _maxCapacity = capacity;
            return this;
        }

        /// <summary>
        /// 启用诊断
        /// Enable diagnostics
        /// </summary>
        public PoolConfigurationBuilder<T> WithDiagnostics(bool enable)
        {
            _enableDiagnostics = enable;
            return this;
        }

        /// <summary>
        /// 构建对象池
        /// Build object pool
        /// </summary>
        public IObjectPool<T> Build()
        {
            if (_factory == null)
            {
                // 尝试使用无参构造函数
                // Try to use parameterless constructor
                var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    _factory = () => (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    throw new InvalidOperationException($"Type {typeof(T).Name} does not have a parameterless constructor. Please provide a factory.");
                }
            }

            return new ObjectPool<T>(_factory, _maxCapacity);
        }
    }

    #endregion
}
