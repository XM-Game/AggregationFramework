// ==========================================================
// 文件名：ObjectPoolBase.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Threading
// 功能: 对象池抽象基类（非泛型），提供池操作的模板方法和通用实现
// ==========================================================

using System;
using System.Threading;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池抽象基类（非泛型）
    /// Object Pool Abstract Base Class (Non-Generic)
    /// 
    /// <para>提供对象池的基础实现框架，定义池操作的模板方法</para>
    /// <para>Provides the basic implementation framework for object pools, defining template methods for pool operations</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：模板方法模式 (Template Method Pattern)
    /// - 定义算法骨架，延迟具体步骤到子类实现
    /// - 提供默认实现，减少子类重复代码
    /// - 支持生命周期钩子（OnInitialize/OnDispose）
    /// </remarks>
    public abstract class ObjectPoolBase : IObjectPool, IDisposable
    {
        #region Fields

        /// <summary>
        /// 池状态
        /// Pool state
        /// </summary>
        protected PoolState _state;

        /// <summary>
        /// 池名称
        /// Pool name
        /// </summary>
        protected string _name;

        /// <summary>
        /// 是否已销毁
        /// Whether the pool has been disposed
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// 线程安全锁
        /// Thread-safe lock
        /// </summary>
        protected readonly object _lock = new object();

        #endregion

        #region Properties

        /// <inheritdoc />
        public abstract Type ObjectType { get; }

        /// <inheritdoc />
        public string Name
        {
            get => _name;
            protected set => _name = value;
        }

        /// <inheritdoc />
        public PoolState State
        {
            get => _state;
            protected set => _state = value;
        }

        /// <inheritdoc />
        public abstract int AvailableCount { get; }

        /// <inheritdoc />
        public abstract int ActiveCount { get; }

        /// <inheritdoc />
        public abstract int TotalCount { get; }

        /// <summary>
        /// 是否已销毁
        /// Whether the pool has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">池名称 / Pool name</param>
        protected ObjectPoolBase(string name = null)
        {
            _name = name ?? GetType().Name;
            _state = PoolState.Uninitialized;
            _disposed = false;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// 初始化对象池
        /// Initialize the object pool
        /// </summary>
        /// <remarks>
        /// 模板方法：定义初始化流程
        /// Template Method: Defines the initialization process
        /// </remarks>
        public void Initialize()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (_state != PoolState.Uninitialized)
                {
                    throw new InvalidOperationException($"Pool '{_name}' is already initialized.");
                }

                try
                {
                    OnInitialize();
                    _state = PoolState.Active;
                }
                catch (Exception ex)
                {
                    _state = PoolState.Disposed;
                    throw new PoolException($"Failed to initialize pool '{_name}'.", ex);
                }
            }
        }

        /// <summary>
        /// 初始化钩子方法（子类重写）
        /// Initialization hook method (override in subclasses)
        /// </summary>
        protected virtual void OnInitialize()
        {
            // 子类可重写此方法执行自定义初始化逻辑
            // Subclasses can override this method to perform custom initialization logic
        }

        /// <summary>
        /// 清空对象池
        /// Clear the object pool
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (_state != PoolState.Active)
                {
                    throw new InvalidOperationException($"Pool '{_name}' is not active.");
                }

                OnClear();
            }
        }

        /// <summary>
        /// 清空钩子方法（子类必须实现）
        /// Clear hook method (must be implemented by subclasses)
        /// </summary>
        protected abstract void OnClear();

        #endregion

        #region Abstract Methods

        /// <inheritdoc />
        public abstract object Get();

        /// <inheritdoc />
        public abstract bool Return(object obj);

        /// <inheritdoc />
        public abstract void Warmup(int count);

        /// <inheritdoc />
        public abstract int Shrink(int targetCount);

        #endregion

        #region Validation Methods

        /// <summary>
        /// 检查池是否已销毁，如果已销毁则抛出异常
        /// Check if the pool is disposed, throw exception if disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new PoolDisposedException($"Pool '{_name}' has been disposed.");
            }
        }

        /// <summary>
        /// 检查池是否处于活跃状态，如果不是则抛出异常
        /// Check if the pool is active, throw exception if not active
        /// </summary>
        protected void ThrowIfNotActive()
        {
            ThrowIfDisposed();

            if (_state != PoolState.Active)
            {
                throw new InvalidOperationException($"Pool '{_name}' is not active. Current state: {_state}");
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// 销毁对象池
        /// Dispose the object pool
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 销毁对象池（可重写）
        /// Dispose the object pool (overridable)
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
                lock (_lock)
                {
                    try
                    {
                        OnDispose();
                    }
                    finally
                    {
                        _state = PoolState.Disposed;
                        _disposed = true;
                    }
                }
            }
        }

        /// <summary>
        /// 销毁钩子方法（子类重写）
        /// Dispose hook method (override in subclasses)
        /// </summary>
        protected virtual void OnDispose()
        {
            // 子类可重写此方法执行自定义清理逻辑
            // Subclasses can override this method to perform custom cleanup logic
        }

        /// <summary>
        /// 析构函数
        /// Finalizer
        /// </summary>
        ~ObjectPoolBase()
        {
            Dispose(false);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 获取池的字符串表示
        /// Get the string representation of the pool
        /// </summary>
        public override string ToString()
        {
            return $"[{GetType().Name}] Name={_name}, State={_state}, Total={TotalCount}, Active={ActiveCount}, Available={AvailableCount}";
        }

        #endregion
    }
}
