// ==========================================================
// 文件名：ChainedCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 链式容量策略，组合多个容量策略
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool
{
    /// <summary>
    /// 链式容量策略
    /// Chained Capacity Policy
    /// 
    /// <para>组合多个容量策略，按优先级执行</para>
    /// <para>Combines multiple capacity policies with priority</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：责任链模式 + 策略模式
    /// 使用场景：
    /// - 需要多层容量控制
    /// - 组合不同的容量策略
    /// - 实现复杂的容量管理逻辑
    /// 
    /// 执行策略：
    /// - 所有策略都同意才能创建
    /// - 任一策略要求收缩就收缩
    /// - 取最小的收缩目标
    /// </remarks>
    public class ChainedCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

        private readonly List<IPoolCapacityPolicy> _policies;

        #endregion

        #region Properties

        /// <inheritdoc />
        public int MinCapacity => _policies.Max(p => p.MinCapacity);

        /// <inheritdoc />
        public int MaxCapacity
        {
            get
            {
                var maxCapacities = _policies
                    .Select(p => p.MaxCapacity)
                    .Where(c => c >= 0)
                    .ToList();

                return maxCapacities.Count > 0 ? maxCapacities.Min() : -1;
            }
        }

        /// <inheritdoc />
        public int InitialCapacity => _policies.Max(p => p.InitialCapacity);

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="policies">容量策略链 / Chain of capacity policies</param>
        public ChainedCapacityPolicy(IEnumerable<IPoolCapacityPolicy> policies)
            : base("ChainedCapacity")
        {
            if (policies == null)
            {
                throw new ArgumentNullException(nameof(policies));
            }

            _policies = policies.ToList();

            if (_policies.Count == 0)
            {
                throw new ArgumentException("至少需要一个容量策略 / At least one policy is required", nameof(policies));
            }
        }

        /// <summary>
        /// 构造函数（可变参数）
        /// Constructor (params)
        /// </summary>
        /// <param name="policies">容量策略数组 / Array of capacity policies</param>
        public ChainedCapacityPolicy(params IPoolCapacityPolicy[] policies)
            : this((IEnumerable<IPoolCapacityPolicy>)policies)
        {
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            // 所有策略都同意才能创建
            // All policies must agree to create
            return _policies.All(p => p.CanCreate(currentTotal, currentActive));
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            // 任一策略要求收缩就收缩
            // Shrink if any policy requires it
            return _policies.Any(p => p.ShouldShrink(currentAvailable, currentActive));
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            // 取最小的收缩目标（最激进的收缩）
            // Take the minimum shrink target (most aggressive shrink)
            return _policies
                .Select(p => p.CalculateShrinkTarget(currentAvailable, currentActive))
                .Min();
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _policies.All(p => p.Validate());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 添加容量策略
        /// Add capacity policy
        /// </summary>
        /// <param name="policy">容量策略 / Capacity policy</param>
        public void AddPolicy(IPoolCapacityPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policies.Add(policy);
        }

        /// <summary>
        /// 移除容量策略
        /// Remove capacity policy
        /// </summary>
        /// <param name="policy">容量策略 / Capacity policy</param>
        /// <returns>是否成功移除 / Whether successfully removed</returns>
        public bool RemovePolicy(IPoolCapacityPolicy policy)
        {
            if (_policies.Count <= 1)
            {
                throw new InvalidOperationException("不能移除最后一个策略 / Cannot remove the last policy");
            }

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
