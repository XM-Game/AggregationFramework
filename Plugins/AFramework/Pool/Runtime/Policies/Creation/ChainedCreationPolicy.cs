// ==========================================================
// 文件名：ChainedCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 链式创建策略，按顺序尝试多个创建策略直到成功
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool
{
    /// <summary>
    /// 链式创建策略
    /// Chained Creation Policy
    /// 
    /// <para>按顺序尝试多个创建策略，直到成功为止</para>
    /// <para>Tries multiple creation policies in sequence until one succeeds</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：责任链模式 + 策略模式
    /// 使用场景：
    /// - 主策略失败时使用备用策略
    /// - 多种创建方式的降级处理
    /// - 提高创建成功率
    /// </remarks>
    public class ChainedCreationPolicy<T> : PoolPolicyBase, IPoolCreationPolicy<T>
    {
        #region Fields

        private readonly List<IPoolCreationPolicy<T>> _policies;
        private readonly bool _stopOnFirstSuccess;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="policies">创建策略链 / Chain of creation policies</param>
        /// <param name="stopOnFirstSuccess">是否在第一次成功后停止 / Whether to stop on first success</param>
        public ChainedCreationPolicy(
            IEnumerable<IPoolCreationPolicy<T>> policies,
            bool stopOnFirstSuccess = true)
            : base("ChainedCreation")
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            _policies = policies.ToList();

            if (_policies.Count == 0)
            {
                throw new ArgumentException("至少需要一个创建策略 / At least one policy is required", nameof(policies));
            }

            _stopOnFirstSuccess = stopOnFirstSuccess;
        }

        /// <summary>
        /// 构造函数（可变参数）
        /// Constructor (params)
        /// </summary>
        /// <param name="policies">创建策略数组 / Array of creation policies</param>
        public ChainedCreationPolicy(params IPoolCreationPolicy<T>[] policies)
            : this(policies, true)
        {
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            Exception lastException = null;

            foreach (var policy in _policies)
            {
                try
                {
                    T obj = policy.Create();
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    if (_stopOnFirstSuccess)
                    {
                        continue;
                    }
                }
            }

            throw new PoolCreationException(
                $"所有创建策略都失败了 / All creation policies failed. Policies count: {_policies.Count}",
                lastException);
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _policies.All(p => p.Validate());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加创建策略
        /// Add creation policy
        /// </summary>
        /// <param name="policy">创建策略 / Creation policy</param>
        public void AddPolicy(IPoolCreationPolicy<T> policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policies.Add(policy);
        }

        /// <summary>
        /// 移除创建策略
        /// Remove creation policy
        /// </summary>
        /// <param name="policy">创建策略 / Creation policy</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemovePolicy(IPoolCreationPolicy<T> policy)
        {
            return _policies.Remove(policy);
        }

        /// <summary>
        /// 获取策略数量
        /// Get policy count
        /// </summary>
        public int PolicyCount => _policies.Count;

        #endregion
    }
}
