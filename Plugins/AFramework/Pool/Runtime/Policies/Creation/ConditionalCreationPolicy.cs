// ==========================================================
// 文件名：ConditionalCreationPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 条件创建策略，根据条件选择不同的创建策略
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 条件创建策略
    /// Conditional Creation Policy
    /// 
    /// <para>根据条件动态选择创建策略</para>
    /// <para>Dynamically selects creation policy based on conditions</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：策略模式 + 条件分支
    /// 使用场景：
    /// - 根据运行时状态选择不同创建方式
    /// - A/B 测试不同创建策略
    /// - 开发/生产环境使用不同策略
    /// </remarks>
    public class ConditionalCreationPolicy<T> : PoolPolicyBase, IPoolCreationPolicy<T>
    {
        #region Fields

        private readonly Func<bool> _condition;
        private readonly IPoolCreationPolicy<T> _truePolicy;
        private readonly IPoolCreationPolicy<T> _falsePolicy;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="condition">条件判断函数 / Condition function</param>
        /// <param name="truePolicy">条件为真时的策略 / Policy when condition is true</param>
        /// <param name="falsePolicy">条件为假时的策略 / Policy when condition is false</param>
        public ConditionalCreationPolicy(
            Func<bool> condition,
            IPoolCreationPolicy<T> truePolicy,
            IPoolCreationPolicy<T> falsePolicy)
            : base("ConditionalCreation")
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _truePolicy = truePolicy ?? throw new ArgumentNullException(nameof(truePolicy));
            _falsePolicy = falsePolicy ?? throw new ArgumentNullException(nameof(falsePolicy));
        }

        #endregion

        #region IPoolCreationPolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            var policy = _condition() ? _truePolicy : _falsePolicy;
            return policy.Create();
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _truePolicy.Validate() && _falsePolicy.Validate();
        }

        #endregion
    }
}
