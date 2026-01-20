// ==========================================================
// 文件名：SelectiveCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 选择性清理策略，根据对象状态选择性执行清理
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 选择性清理策略
    /// Selective Cleanup Policy
    /// 
    /// <para>根据对象状态选择性执行清理操作</para>
    /// <para>Selectively performs cleanup based on object state</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 设计模式：策略模式 + 过滤器模式
    /// 使用场景：
    /// - 某些对象不需要清理
    /// - 根据对象状态决定清理力度
    /// - 性能优化（跳过不必要的清理）
    /// </remarks>
    public class SelectiveCleanupPolicy<T> : PoolPolicyBase, IPoolCleanupPolicy<T>
    {
        #region Fields

        private readonly IPoolCleanupPolicy<T> _innerPolicy;
        private readonly Func<T, bool> _shouldCleanOnReturn;
        private readonly Func<T, bool> _shouldCleanOnDestroy;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="innerPolicy">内部清理策略 / Inner cleanup policy</param>
        /// <param name="shouldCleanOnReturn">归还时是否清理的判断函数 / Function to determine if cleanup on return</param>
        /// <param name="shouldCleanOnDestroy">销毁时是否清理的判断函数 / Function to determine if cleanup on destroy</param>
        public SelectiveCleanupPolicy(
            IPoolCleanupPolicy<T> innerPolicy,
            Func<T, bool> shouldCleanOnReturn = null,
            Func<T, bool> shouldCleanOnDestroy = null)
            : base("SelectiveCleanup")
        {
            _innerPolicy = innerPolicy ?? throw new ArgumentNullException(nameof(innerPolicy));
            _shouldCleanOnReturn = shouldCleanOnReturn ?? (_ => true);
            _shouldCleanOnDestroy = shouldCleanOnDestroy ?? (_ => true);
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            if (_shouldCleanOnReturn(obj))
            {
                _innerPolicy.OnReturn(obj);
            }
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            if (_shouldCleanOnDestroy(obj))
            {
                _innerPolicy.OnDestroy(obj);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _innerPolicy.Validate();
        }

        #endregion
    }
}
