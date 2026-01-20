// ==========================================================
// 文件名：ResetCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 重置清理策略，调用对象的 Reset 方法
// ==========================================================

using System;
using System.Reflection;

namespace AFramework.Pool
{
    /// <summary>
    /// 重置清理策略
    /// Reset Cleanup Policy
    /// 
    /// <para>调用对象的 Reset 方法进行清理的策略</para>
    /// <para>Policy that cleans up objects by calling their Reset method</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 对象实现了 Reset 方法
    /// - 需要重置对象状态
    /// - 对象可复用但需要清理
    /// 
    /// 支持的 Reset 方法：
    /// - public void Reset()
    /// - 通过反射调用
    /// 
    /// 注意：
    /// - 如果对象没有 Reset 方法，将不执行任何操作
    /// - 使用反射，性能略低
    /// </remarks>
    public class ResetCleanupPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T>
    {
        #region Fields

        /// <summary>
        /// Reset 方法信息（缓存）
        /// Reset method info (cached)
        /// </summary>
        private readonly MethodInfo _resetMethod;

        /// <summary>
        /// 是否有 Reset 方法
        /// Whether has Reset method
        /// </summary>
        private readonly bool _hasResetMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        public ResetCleanupPolicy(string name = null)
            : base(name ?? "ResetCleanupPolicy")
        {
            // 查找 Reset 方法
            // Find Reset method
            _resetMethod = typeof(T).GetMethod("Reset", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            _hasResetMethod = _resetMethod != null;
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            if (_hasResetMethod)
            {
                try
                {
                    _resetMethod.Invoke(obj, null);
                }
                catch (Exception ex)
                {
                    throw new PoolException($"Failed to call Reset method on object of type {typeof(T).Name}.", ex);
                }
            }
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 销毁时也调用 Reset 方法（如果有）
            // Also call Reset method on destroy (if exists)
            if (_hasResetMethod)
            {
                try
                {
                    _resetMethod.Invoke(obj, null);
                }
                catch (Exception ex)
                {
                    throw new PoolException($"Failed to call Reset method on object of type {typeof(T).Name}.", ex);
                }
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 策略始终有效，即使没有 Reset 方法
            // Policy is always valid, even without Reset method
            return true;
        }

        #endregion
    }
}
