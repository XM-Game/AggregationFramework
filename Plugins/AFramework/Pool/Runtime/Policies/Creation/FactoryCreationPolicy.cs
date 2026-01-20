// ==========================================================
// 文件名：FactoryCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 工厂创建策略，使用工厂方法创建对象
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 工厂创建策略
    /// Factory Creation Policy
    /// 
    /// <para>使用工厂方法创建对象的策略</para>
    /// <para>Policy that creates objects using factory method</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 需要自定义创建逻辑
    /// - 对象创建需要参数
    /// - 需要依赖注入
    /// - 复杂对象初始化
    /// 
    /// 优势：
    /// - 灵活的创建逻辑
    /// - 支持依赖注入
    /// - 易于测试
    /// </remarks>
    public class FactoryCreationPolicy<T> : PoolPolicyBase<T>, IPoolCreationPolicy<T>
    {
        #region Fields

        /// <summary>
        /// 工厂方法
        /// Factory method
        /// </summary>
        private readonly Func<T> _factory;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="factory">工厂方法 / Factory method</param>
        /// <param name="name">策略名称 / Policy name</param>
        public FactoryCreationPolicy(Func<T> factory, string name = null)
            : base(name ?? "FactoryCreationPolicy")
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            ThrowIfDisposed();

            try
            {
                T obj = _factory();

                if (obj == null)
                {
                    throw new PoolCreationException($"Factory method returned null for type {typeof(T).Name}.");
                }

                return obj;
            }
            catch (PoolCreationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PoolCreationException($"Failed to create object using factory method for type {typeof(T).Name}.", ex);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证工厂方法是否为 null
            // Validate if factory method is null
            return _factory != null;
        }

        #endregion
    }
}
