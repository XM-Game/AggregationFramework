// ==========================================================
// 文件名：PoolFactory.cs
// 命名空间: AFramework.Pool.DI
// 依赖: System, AFramework.DI, AFramework.Pool
// 功能: 对象池工厂
// ==========================================================

using System;
using AFramework.DI;

namespace AFramework.Pool.DI
{
    /// <summary>
    /// 对象池工厂
    /// Pool Factory
    /// </summary>
    /// <remarks>
    /// 提供动态创建对象池的工厂方法
    /// Provides factory methods to dynamically create object pools
    /// </remarks>
    public class PoolFactory
    {
        #region 字段 Fields

        private readonly IObjectResolver _resolver;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化池工厂
        /// Initialize pool factory
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        public PoolFactory(IObjectResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region 创建方法 Creation Methods

        /// <summary>
        /// 创建对象池
        /// Create object pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> CreatePool<T>(int maxCapacity = 100) where T : class, new()
        {
            return new ObjectPool<T>(() => new T(), maxCapacity);
        }

        /// <summary>
        /// 创建带工厂的对象池
        /// Create object pool with factory
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="factory">对象工厂 / Object factory</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> CreatePoolWithFactory<T>(
            Func<T> factory,
            int maxCapacity = 100) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return new ObjectPool<T>(factory, maxCapacity);
        }

        /// <summary>
        /// 创建使用容器解析的对象池
        /// Create object pool using container resolution
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> CreatePoolWithContainerFactory<T>(
            int maxCapacity = 100) where T : class
        {
            return new ObjectPool<T>(
                () => _resolver.Resolve<T>(),
                maxCapacity
            );
        }

        /// <summary>
        /// 创建栈对象池
        /// Create stack object pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> CreateStackPool<T>(int maxCapacity = 100) where T : class, new()
        {
            return new StackObjectPool<T>(() => new T(), maxCapacity);
        }

        /// <summary>
        /// 创建队列对象池
        /// Create queue object pool
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <returns>对象池实例 / Object pool instance</returns>
        public IObjectPool<T> CreateQueuePool<T>(int maxCapacity = 100) where T : class, new()
        {
            return new QueueObjectPool<T>(() => new T(), maxCapacity);
        }

        #endregion

        #region 配置方法 Configuration Methods

        /// <summary>
        /// 创建池构建器
        /// Create pool builder
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
        /// <returns>池构建器 / Pool builder</returns>
        public PoolFactoryBuilder<T> CreateBuilder<T>() where T : class
        {
            return new PoolFactoryBuilder<T>(_resolver);
        }

        #endregion
    }

    /// <summary>
    /// 对象池工厂构建器
    /// Pool Factory Builder
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
    public class PoolFactoryBuilder<T> where T : class
    {
        private readonly IObjectResolver _resolver;
        private Func<T> _factory;
        private int _maxCapacity = 100;
        private PoolType _poolType = PoolType.Default;

        /// <summary>
        /// 池类型枚举
        /// Pool type enum
        /// </summary>
        private enum PoolType
        {
            Default,
            Stack,
            Queue
        }

        /// <summary>
        /// 初始化构建器
        /// Initialize builder
        /// </summary>
        internal PoolFactoryBuilder(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        /// <summary>
        /// 设置对象工厂
        /// Set object factory
        /// </summary>
        public PoolFactoryBuilder<T> WithFactory(Func<T> factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        /// 使用容器解析工厂
        /// Use container resolution factory
        /// </summary>
        public PoolFactoryBuilder<T> WithContainerFactory()
        {
            _factory = () => _resolver.Resolve<T>();
            return this;
        }

        /// <summary>
        /// 设置最大容量
        /// Set maximum capacity
        /// </summary>
        public PoolFactoryBuilder<T> WithMaxCapacity(int capacity)
        {
            _maxCapacity = capacity;
            return this;
        }

        /// <summary>
        /// 使用栈池
        /// Use stack pool
        /// </summary>
        public PoolFactoryBuilder<T> AsStackPool()
        {
            _poolType = PoolType.Stack;
            return this;
        }

        /// <summary>
        /// 使用队列池
        /// Use queue pool
        /// </summary>
        public PoolFactoryBuilder<T> AsQueuePool()
        {
            _poolType = PoolType.Queue;
            return this;
        }

        /// <summary>
        /// 构建对象池
        /// Build object pool
        /// </summary>
        public IObjectPool<T> Build()
        {
            // 如果没有工厂，尝试使用默认构造函数
            if (_factory == null)
            {
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

            switch (_poolType)
            {
                case PoolType.Stack:
                    return new StackObjectPool<T>(_factory, _maxCapacity);

                case PoolType.Queue:
                    return new QueueObjectPool<T>(_factory, _maxCapacity);

                default:
                    return new ObjectPool<T>(_factory, _maxCapacity);
            }
        }
    }
}
