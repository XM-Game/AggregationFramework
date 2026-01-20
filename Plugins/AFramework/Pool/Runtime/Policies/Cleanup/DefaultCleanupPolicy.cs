// ==========================================================
// 文件名：DefaultCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 默认清理策略，不执行任何清理操作
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 默认清理策略
    /// Default Cleanup Policy
    /// 
    /// <para>不执行任何清理操作的默认策略</para>
    /// <para>Default policy that performs no cleanup operations</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 对象无需清理
    /// - 对象自身管理状态
    /// - 简单值类型对象
    /// 
    /// 注意：
    /// - 不会调用任何清理方法
    /// - 不会重置对象状态
    /// - 适合无状态或自清理对象
    /// </remarks>
    public class DefaultCleanupPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T>
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        public DefaultCleanupPolicy(string name = null)
            : base(name ?? "DefaultCleanupPolicy")
        {
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 默认策略不执行任何操作
            // Default policy performs no operations
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 默认策略不执行任何操作
            // Default policy performs no operations
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 默认策略始终有效
            // Default policy is always valid
            return true;
        }

        #endregion
    }
}
