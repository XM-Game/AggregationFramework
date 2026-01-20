// ==========================================================
// 文件名：ChainedCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 链式清理策略，按顺序执行多个清理策略
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool
{
    /// <summary>
    /// 链式清理策略
    /// Chained Cleanup Policy
    /// 
    /// <para>按顺序执行多个清理策略</para>
    /// <para>Executes multiple cleanup policies in sequence</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：责任链模式 + 策略模式
    /// 使用场景：
    /// - 需要多步骤清理操作
    /// - 组合多种清理策略
    /// - 分层清理（状态重置 → 资源释放 → 销毁）
    /// </remarks>
    public class ChainedCleanupPolicy<T> : PoolPolicyBase, IPoolCleanupPolicy<T>
    {
        #region Fields

        private readonly List<IPoolCleanupPolicy<T>> _policies;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="policies">清理策略链 / Chain of cleanup policies</param>
        public ChainedCleanupPolicy(IEnumerable<IPoolCleanupPolicy<T>> policies)
            : base("ChainedCleanup")
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            _policies = policies.ToList();

            if (_policies.Count == 0)
            {
                throw new ArgumentException("至少需要一个清理策略 / At least one policy is required", nameof(policies));
            }
        }

        /// <summary>
        /// 构造函数（可变参数）
        /// Constructor (params)
        /// </summary>
        /// <param name="policies">清理策略数组 / Array of cleanup policies</param>
        public ChainedCleanupPolicy(params IPoolCleanupPolicy<T>[] policies)
            : this((IEnumerable<IPoolCleanupPolicy<T>>)policies)
        {
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            foreach (var policy in _policies)
            {
                try
                {
                    policy.OnReturn(obj);
                }
                catch
                {
                    // 继续执行其他策略
                    // Continue with other policies
                }
            }
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            foreach (var policy in _policies)
            {
                try
                {
                    policy.OnDestroy(obj);
                }
                catch
                {
                    // 继续执行其他策略
                    // Continue with other policies
                }
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _policies.All(p => p.Validate());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加清理策略
        /// Add cleanup policy
        /// </summary>
        /// <param name="policy">清理策略 / Cleanup policy</param>
        public void AddPolicy(IPoolCleanupPolicy<T> policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policies.Add(policy);
        }

        /// <summary>
        /// 移除清理策略
        /// Remove cleanup policy
        /// </summary>
        /// <param name="policy">清理策略 / Cleanup policy</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemovePolicy(IPoolCleanupPolicy<T> policy)
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
