// ==========================================================
// 文件名：PooledObjectBase.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 池化对象抽象基类，提供生命周期回调的默认实现
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 池化对象抽象基类
    /// Pooled Object Abstract Base Class
    /// 
    /// <para>提供池化对象生命周期回调的默认实现</para>
    /// <para>Provides default implementation of pooled object lifecycle callbacks</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：模板方法模式 (Template Method Pattern)
    /// - 定义对象生命周期钩子
    /// - 提供默认实现，子类按需重写
    /// - 支持状态追踪和验证
    /// </remarks>
    public abstract class PooledObjectBase : IPooledObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// 对象是否在池中
        /// Whether the object is in pool
        /// </summary>
        protected bool _isInPool;

        /// <summary>
        /// 对象是否已销毁
        /// Whether the object has been disposed
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// 获取次数
        /// Get count
        /// </summary>
        protected int _getCount;

        /// <summary>
        /// 归还次数
        /// Return count
        /// </summary>
        protected int _returnCount;

        #endregion

        #region Properties

        /// <summary>
        /// 对象是否在池中
        /// Whether the object is in pool
        /// </summary>
        public bool IsInPool => _isInPool;

        /// <summary>
        /// 对象是否已销毁
        /// Whether the object has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        /// <summary>
        /// 获取次数
        /// Get count
        /// </summary>
        public int GetCount => _getCount;

        /// <summary>
        /// 归还次数
        /// Return count
        /// </summary>
        public int ReturnCount => _returnCount;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        protected PooledObjectBase()
        {
            _isInPool = false;
            _disposed = false;
            _getCount = 0;
            _returnCount = 0;
        }

        #endregion

        #region IPooledObject Implementation

        /// <inheritdoc />
        public virtual void OnCreate()
        {
            // 子类可重写此方法执行自定义创建逻辑
            // Subclasses can override this method to perform custom create logic
        }

        /// <inheritdoc />
        public virtual void OnGet()
        {
            ThrowIfDisposed();

            if (!_isInPool)
            {
                throw new InvalidOperationException("Object is not in pool, cannot get.");
            }

            _isInPool = false;
            _getCount++;

            OnGetCore();
        }

        /// <inheritdoc />
        public virtual void OnReturn()
        {
            ThrowIfDisposed();

            if (_isInPool)
            {
                throw new InvalidOperationException("Object is already in pool, cannot return.");
            }

            _isInPool = true;
            _returnCount++;

            OnReturnCore();
        }

        /// <inheritdoc />
        public virtual void OnDestroy()
        {
            ThrowIfDisposed();

            OnDestroyCore();
        }

        #endregion

        #region Core Lifecycle Methods (子类重写 / Override in subclasses)

        /// <summary>
        /// 对象从池中获取时的核心逻辑（子类重写）
        /// Core logic when object is retrieved from pool (override in subclasses)
        /// </summary>
        /// <remarks>
        /// 在此方法中执行对象激活逻辑，例如：
        /// - 重置状态
        /// - 启用组件
        /// - 订阅事件
        /// </remarks>
        protected virtual void OnGetCore()
        {
            // 子类可重写此方法执行自定义获取逻辑
            // Subclasses can override this method to perform custom get logic
        }

        /// <summary>
        /// 对象归还到池中时的核心逻辑（子类重写）
        /// Core logic when object is returned to pool (override in subclasses)
        /// </summary>
        /// <remarks>
        /// 在此方法中执行对象清理逻辑，例如：
        /// - 清空数据
        /// - 禁用组件
        /// - 取消订阅事件
        /// </remarks>
        protected virtual void OnReturnCore()
        {
            // 子类可重写此方法执行自定义归还逻辑
            // Subclasses can override this method to perform custom return logic
        }

        /// <summary>
        /// 对象销毁时的核心逻辑（子类重写）
        /// Core logic when object is destroyed (override in subclasses)
        /// </summary>
        /// <remarks>
        /// 在此方法中执行对象销毁逻辑，例如：
        /// - 释放资源
        /// - 清理引用
        /// - 取消订阅
        /// </remarks>
        protected virtual void OnDestroyCore()
        {
            // 子类可重写此方法执行自定义销毁逻辑
            // Subclasses can override this method to perform custom destroy logic
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// 检查对象是否已销毁，如果已销毁则抛出异常
        /// Check if the object is disposed, throw exception if disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name, "Pooled object has been disposed.");
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// 销毁对象
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 销毁对象（可重写）
        /// Dispose the object (overridable)
        /// </summary>
        /// <param name="disposing">是否正在销毁托管资源 / Whether disposing managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    OnDestroy();
                }
                catch (Exception ex)
                {
                    // 记录异常但不抛出，避免阻止销毁流程
                    // Log exception but don't throw, avoid blocking dispose process
                    System.Diagnostics.Debug.WriteLine($"Exception during dispose: {ex}");
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// 析构函数
        /// Finalizer
        /// </summary>
        ~PooledObjectBase()
        {
            Dispose(false);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 重置对象状态
        /// Reset object state
        /// </summary>
        /// <remarks>
        /// 将对象恢复到初始状态，通常在归还到池中时调用
        /// Restore object to initial state, usually called when returned to pool
        /// </remarks>
        public virtual void Reset()
        {
            // 子类可重写此方法执行自定义重置逻辑
            // Subclasses can override this method to perform custom reset logic
        }

        /// <summary>
        /// 获取对象的字符串表示
        /// Get the string representation of the object
        /// </summary>
        public override string ToString()
        {
            return $"[{GetType().Name}] InPool={_isInPool}, GetCount={_getCount}, ReturnCount={_returnCount}";
        }

        #endregion
    }
}
