// ==========================================================
// 文件名：PoolPolicyPresets.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池策略预设，提供常见场景的策略组合
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池策略预设
    /// Object Pool Policy Presets
    /// 
    /// <para>为常见场景提供预定义的策略组合，简化配置</para>
    /// <para>Provides predefined policy combinations for common scenarios to simplify configuration</para>
    /// </summary>
    /// <remarks>
    /// 改进点：
    /// - 减少配置负担，开箱即用
    /// - 提供最佳实践的策略组合
    /// - 降低使用门槛
    /// </remarks>
    public static class PoolPolicyPresets
    {
        #region 简单对象池预设

        /// <summary>
        /// 创建简单对象池的生命周期策略
        /// Create lifecycle policy for simple object pool
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        /// <remarks>
        /// 适用场景：
        /// - 简单的 C# 对象
        /// - 有无参构造函数
        /// - 无特殊清理需求
        /// </remarks>
        public static IPoolLifecyclePolicy<T> Simple<T>() where T : new()
        {
            return new DefaultLifecyclePolicy<T>();
        }

        /// <summary>
        /// 创建简单对象池的生命周期策略（使用工厂函数）
        /// Create lifecycle policy for simple object pool (using factory function)
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        public static IPoolLifecyclePolicy<T> Simple<T>(Func<T> createFunc)
        {
            return new FuncLifecyclePolicy<T>(createFunc);
        }

        #endregion

        #region 可重置对象池预设

        /// <summary>
        /// 创建可重置对象池的生命周期策略
        /// Create lifecycle policy for resettable object pool
        /// </summary>
        /// <typeparam name="T">对象类型（必须实现 IResettable）/ Object type (must implement IResettable)</typeparam>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        /// <remarks>
        /// 适用场景：
        /// - 对象实现了 IResettable 接口
        /// - 需要在归还时重置状态
        /// </remarks>
        public static IPoolLifecyclePolicy<T> Resettable<T>() where T : IResettable, new()
        {
            return new FuncLifecyclePolicy<T>(
                createFunc: () => new T(),
                returnAction: obj => obj.Reset(),
                destroyAction: obj =>
                {
                    if (obj is IDisposable disposable)
                        disposable.Dispose();
                }
            );
        }

        #endregion

        #region 集合对象池预设

        /// <summary>
        /// 创建集合对象池的生命周期策略
        /// Create lifecycle policy for collection object pool
        /// </summary>
        /// <typeparam name="T">集合类型 / Collection type</typeparam>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="clearAction">清空操作 / Clear action</param>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        /// <remarks>
        /// 适用场景：
        /// - List、Dictionary、HashSet 等集合
        /// - 需要在归还时清空集合
        /// </remarks>
        public static IPoolLifecyclePolicy<T> Collection<T>(
            Func<T> createFunc,
            Action<T> clearAction)
        {
            return new FuncLifecyclePolicy<T>(
                createFunc: createFunc,
                returnAction: clearAction,
                destroyAction: null
            );
        }

        #endregion

        #region 一次性对象池预设

        /// <summary>
        /// 创建一次性对象池的生命周期策略
        /// Create lifecycle policy for disposable object pool
        /// </summary>
        /// <typeparam name="T">对象类型（必须实现 IDisposable）/ Object type (must implement IDisposable)</typeparam>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        /// <remarks>
        /// 适用场景：
        /// - 对象实现了 IDisposable 接口
        /// - 需要在销毁时释放资源
        /// </remarks>
        public static IPoolLifecyclePolicy<T> Disposable<T>(Func<T> createFunc) where T : IDisposable
        {
            return new FuncLifecyclePolicy<T>(
                createFunc: createFunc,
                returnAction: null,
                destroyAction: obj => obj.Dispose()
            );
        }

        #endregion

        #region 自定义预设

        /// <summary>
        /// 创建自定义生命周期策略
        /// Create custom lifecycle policy
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="returnAction">归还时的清理操作 / Cleanup action on return</param>
        /// <param name="destroyAction">销毁时的清理操作 / Cleanup action on destroy</param>
        /// <returns>生命周期策略 / Lifecycle policy</returns>
        public static IPoolLifecyclePolicy<T> Custom<T>(
            Func<T> createFunc,
            Action<T> returnAction = null,
            Action<T> destroyAction = null)
        {
            return new FuncLifecyclePolicy<T>(createFunc, returnAction, destroyAction);
        }

        #endregion
    }

    /// <summary>
    /// 可重置接口
    /// Resettable Interface
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        /// 重置对象状态
        /// Reset object state
        /// </summary>
        void Reset();
    }
}
