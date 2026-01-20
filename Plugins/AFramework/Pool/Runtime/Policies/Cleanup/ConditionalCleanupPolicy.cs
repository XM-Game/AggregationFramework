// ==========================================================
// 文件名：ConditionalCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 条件清理策略，根据条件选择不同的清理策略
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 条件清理策略
    /// Conditional Cleanup Policy
    /// 
    /// <para>根据条件动态选择清理策略</para>
    /// <para>Dynamically selects cleanup policy based on conditions</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：策略模式 + 条件分支
    /// 使用场景：
    /// - 根据对象状态选择不同清理方式
    /// - 开发/生产环境使用不同清理策略
    /// - 根据性能需求动态调整清理力度
    /// </remarks>
    public class ConditionalCleanupPolicy<T> : PoolPolicyBase, IPoolCleanupPolicy<T>
    {
        #region Fields

        private readonly Func<T, bool> _condition;
        private readonly IPoolCleanupPolicy<T> _truePolicy;
        private readonly IPoolCleanupPolicy<T> _falsePolicy;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="condition">条件判断函数 / Condition function</param>
        /// <param name="truePolicy">条件为真时的策略 / Policy when condition is true</param>
        /// <param name="falsePolicy">条件为假时的策略 / Policy when condition is false</param>
        public ConditionalCleanupPolicy(
            Func<T, bool> condition,
            IPoolCleanupPolicy<T> truePolicy,
            IPoolCleanupPolicy<T> falsePolicy)
            : base("ConditionalCleanup")
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _truePolicy = truePolicy ?? throw new ArgumentNullException(nameof(truePolicy));
            _falsePolicy = falsePolicy ?? throw new ArgumentNullException(nameof(falsePolicy));
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            var policy = _condition(obj) ? _truePolicy : _falsePolicy;
            policy.OnReturn(obj);
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            var policy = _condition(obj) ? _truePolicy : _falsePolicy;
            policy.OnDestroy(obj);
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _truePolicy.Validate() && _falsePolicy.Validate();
        }

        #endregion
    }
}
