// ==========================================================
// 文件名：DefaultCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 默认创建策略，使用无参构造函数创建对象
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 默认创建策略
    /// Default Creation Policy
    /// 
    /// <para>使用无参构造函数创建对象的默认策略</para>
    /// <para>Default policy that creates objects using parameterless constructor</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 对象有公共无参构造函数
    /// - 不需要特殊初始化逻辑
    /// - 简单对象创建
    /// 
    /// 限制：
    /// - 类型 T 必须有无参构造函数
    /// - 不支持抽象类或接口
    /// </remarks>
    public class DefaultCreationPolicy<T> : PoolPolicyBase<T>, IPoolCreationPolicy<T> where T : new()
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        public DefaultCreationPolicy(string name = null)
            : base(name ?? "DefaultCreationPolicy")
        {
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            ThrowIfDisposed();

            try
            {
                return new T();
            }
            catch (Exception ex)
            {
                throw new PoolCreationException($"Failed to create object using default constructor for type {typeof(T).Name}.", ex);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 由于泛型约束 where T : new()，类型始终有无参构造函数
            // Due to generic constraint where T : new(), type always has parameterless constructor
            return true;
        }

        #endregion
    }
}
