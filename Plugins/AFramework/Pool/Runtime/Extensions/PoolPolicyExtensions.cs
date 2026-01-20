// ==========================================================
// 文件名：PoolPolicyExtensions.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池策略扩展方法，提供便捷的策略组合和创建
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池策略扩展方法
    /// Pool Policy Extension Methods
    /// 
    /// <para>提供便捷的策略组合和创建方法</para>
    /// <para>Provides convenient methods for policy composition and creation</para>
    /// </summary>
    public static class PoolPolicyExtensions
    {
        #region Creation Policy Extensions

        /// <summary>
        /// 创建条件创建策略
        /// Create conditional creation policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="truePolicy">条件为真时的策略 / Policy when condition is true</param>
        /// <param name="condition">条件判断函数 / Condition function</param>
        /// <param name="falsePolicy">条件为假时的策略 / Policy when condition is false</param>
        /// <returns>条件创建策略 / Conditional creation policy</returns>
        public static ConditionalCreationPolicy<T> When<T>(
            this IPoolCreationPolicy<T> truePolicy,
            Func<bool> condition,
            IPoolCreationPolicy<T> falsePolicy)
        {
            return new ConditionalCreationPolicy<T>(condition, truePolicy, falsePolicy);
        }

        /// <summary>
        /// 创建链式创建策略
        /// Create chained creation policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="firstPolicy">第一个策略 / First policy</param>
        /// <param name="fallbackPolicy">备用策略 / Fallback policy</param>
        /// <returns>链式创建策略 / Chained creation policy</returns>
        public static ChainedCreationPolicy<T> ThenFallbackTo<T>(
            this IPoolCreationPolicy<T> firstPolicy,
            IPoolCreationPolicy<T> fallbackPolicy)
        {
            return new ChainedCreationPolicy<T>(firstPolicy, fallbackPolicy);
        }

        /// <summary>
        /// 创建缓存创建策略
        /// Create cached creation policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="policy">内部策略 / Inner policy</param>
        /// <param name="cloneFunc">克隆函数 / Clone function</param>
        /// <param name="maxCacheSize">最大缓存大小 / Maximum cache size</param>
        /// <returns>缓存创建策略 / Cached creation policy</returns>
        public static CachedCreationPolicy<T> WithCache<T>(
            this IPoolCreationPolicy<T> policy,
            Func<T, T> cloneFunc,
            int maxCacheSize = 10)
        {
            return new CachedCreationPolicy<T>(policy, cloneFunc, maxCacheSize);
        }

        /// <summary>
        /// 创建缓存创建策略（使用 ICloneable）
        /// Create cached creation policy (using ICloneable)
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="policy">内部策略 / Inner policy</param>
        /// <param name="maxCacheSize">最大缓存大小 / Maximum cache size</param>
        /// <returns>缓存创建策略 / Cached creation policy</returns>
        public static CachedCreationPolicy<T> WithCache<T>(
            this IPoolCreationPolicy<T> policy,
            int maxCacheSize = 10)
        {
            return new CachedCreationPolicy<T>(policy, maxCacheSize);
        }

        #endregion

        #region Cleanup Policy Extensions

        /// <summary>
        /// 创建条件清理策略
        /// Create conditional cleanup policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="truePolicy">条件为真时的策略 / Policy when condition is true</param>
        /// <param name="condition">条件判断函数 / Condition function</param>
        /// <param name="falsePolicy">条件为假时的策略 / Policy when condition is false</param>
        /// <returns>条件清理策略 / Conditional cleanup policy</returns>
        public static ConditionalCleanupPolicy<T> When<T>(
            this IPoolCleanupPolicy<T> truePolicy,
            Func<T, bool> condition,
            IPoolCleanupPolicy<T> falsePolicy)
        {
            return new ConditionalCleanupPolicy<T>(condition, truePolicy, falsePolicy);
        }

        /// <summary>
        /// 创建链式清理策略
        /// Create chained cleanup policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="firstPolicy">第一个策略 / First policy</param>
        /// <param name="secondPolicy">第二个策略 / Second policy</param>
        /// <returns>链式清理策略 / Chained cleanup policy</returns>
        public static ChainedCleanupPolicy<T> ThenCleanWith<T>(
            this IPoolCleanupPolicy<T> firstPolicy,
            IPoolCleanupPolicy<T> secondPolicy)
        {
            return new ChainedCleanupPolicy<T>(firstPolicy, secondPolicy);
        }

        /// <summary>
        /// 创建选择性清理策略
        /// Create selective cleanup policy
        /// </summary>
        /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
        /// <param name="policy">内部策略 / Inner policy</param>
        /// <param name="shouldCleanOnReturn">归还时是否清理 / Whether to clean on return</param>
        /// <param name="shouldCleanOnDestroy">销毁时是否清理 / Whether to clean on destroy</param>
        /// <returns>选择性清理策略 / Selective cleanup policy</returns>
        public static SelectiveCleanupPolicy<T> OnlyWhen<T>(
            this IPoolCleanupPolicy<T> policy,
            Func<T, bool> shouldCleanOnReturn = null,
            Func<T, bool> shouldCleanOnDestroy = null)
        {
            return new SelectiveCleanupPolicy<T>(policy, shouldCleanOnReturn, shouldCleanOnDestroy);
        }

        #endregion

        #region Capacity Policy Extensions

        /// <summary>
        /// 创建链式容量策略
        /// Create chained capacity policy
        /// </summary>
        /// <param name="firstPolicy">第一个策略 / First policy</param>
        /// <param name="secondPolicy">第二个策略 / Second policy</param>
        /// <returns>链式容量策略 / Chained capacity policy</returns>
        public static ChainedCapacityPolicy CombineWith(
            this IPoolCapacityPolicy firstPolicy,
            IPoolCapacityPolicy secondPolicy)
        {
            return new ChainedCapacityPolicy(firstPolicy, secondPolicy);
        }

        #endregion

        #region Validation Extensions

        /// <summary>
        /// 验证并返回创建策略
        /// Validate and return creation policy
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="policy">策略 / Policy</param>
        /// <returns>策略 / Policy</returns>
        /// <exception cref="InvalidOperationException">策略验证失败 / Policy validation failed</exception>
        public static IPoolCreationPolicy<T> EnsureValid<T>(this IPoolCreationPolicy<T> policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            if (!policy.Validate())
            {
                throw new InvalidOperationException(
                    $"策略验证失败 / Policy validation failed: {policy.Name}");
            }

            return policy;
        }

        /// <summary>
        /// 验证并返回清理策略
        /// Validate and return cleanup policy
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="policy">策略 / Policy</param>
        /// <returns>策略 / Policy</returns>
        /// <exception cref="InvalidOperationException">策略验证失败 / Policy validation failed</exception>
        public static IPoolCleanupPolicy<T> EnsureValid<T>(this IPoolCleanupPolicy<T> policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            if (!policy.Validate())
            {
                throw new InvalidOperationException(
                    $"策略验证失败 / Policy validation failed: {policy.Name}");
            }

            return policy;
        }

        /// <summary>
        /// 验证并返回容量策略
        /// Validate and return capacity policy
        /// </summary>
        /// <param name="policy">策略 / Policy</param>
        /// <returns>策略 / Policy</returns>
        /// <exception cref="InvalidOperationException">策略验证失败 / Policy validation failed</exception>
        public static IPoolCapacityPolicy EnsureValid(this IPoolCapacityPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            if (!policy.Validate())
            {
                throw new InvalidOperationException(
                    $"策略验证失败 / Policy validation failed: {policy.Name}");
            }

            return policy;
        }

        #endregion
    }
}
