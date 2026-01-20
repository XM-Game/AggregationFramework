// ==========================================================
// 文件名：DisposeCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 销毁清理策略，调用对象的 Dispose 方法
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 销毁清理策略
    /// Dispose Cleanup Policy
    /// 
    /// <para>调用对象的 Dispose 方法进行清理的策略</para>
    /// <para>Policy that cleans up objects by calling their Dispose method</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 对象实现了 IDisposable 接口
    /// - 需要释放非托管资源
    /// - 对象包含文件句柄、数据库连接等
    /// 
    /// 注意：
    /// - 仅适用于实现 IDisposable 的对象
    /// - 调用 Dispose 后对象不应再使用
    /// - 通常用于池销毁时，而非归还时
    /// 
    /// 警告：
    /// - 不要在归还到池时使用此策略
    /// - Dispose 后的对象无法复用
    /// - 仅用于池清空或销毁场景
    /// </remarks>
    public class DisposeCleanupPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T> where T : IDisposable
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        public DisposeCleanupPolicy(string name = null)
            : base(name ?? "DisposeCleanupPolicy")
        {
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            // 归还时不执行 Dispose，仅在销毁时执行
            // Do not dispose on return, only on destroy
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            try
            {
                obj?.Dispose();
            }
            catch (Exception ex)
            {
                throw new PoolException($"Failed to dispose object of type {typeof(T).Name}.", ex);
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证类型是否实现 IDisposable
            // Validate if type implements IDisposable
            return typeof(IDisposable).IsAssignableFrom(typeof(T));
        }

        #endregion
    }
}
