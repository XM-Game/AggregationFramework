// ==========================================================
// 文件名：PoolPolicyBase.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 池策略抽象基类，提供策略模式的基础实现框架
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 池策略抽象基类
    /// Pool Policy Abstract Base Class
    /// 
    /// <para>提供池策略的基础实现框架，简化自定义策略开发</para>
    /// <para>Provides basic implementation framework for pool policies, simplifying custom policy development</para>
    /// </summary>
    /// <remarks>
    /// 设计模式：策略模式 (Strategy Pattern)
    /// - 封装算法族，使它们可以互相替换
    /// - 提供默认实现，减少子类重复代码
    /// - 支持策略组合和链式调用
    /// </remarks>
    public abstract class PoolPolicyBase : IPoolPolicy, IDisposable
    {
        #region Fields

        /// <summary>
        /// 策略名称
        /// Policy name
        /// </summary>
        protected string _name;

        /// <summary>
        /// 是否已销毁
        /// Whether the policy has been disposed
        /// </summary>
        protected bool _disposed;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name
        {
            get => _name;
            protected set => _name = value;
        }

        /// <inheritdoc />
        public virtual string Description => $"{GetType().Name} policy";

        /// <summary>
        /// 是否已销毁
        /// Whether the policy has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        protected PoolPolicyBase(string name = null)
        {
            _name = name ?? GetType().Name;
            _disposed = false;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// 检查策略是否已销毁，如果已销毁则抛出异常
        /// Check if the policy is disposed, throw exception if disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(_name, $"Policy '{_name}' has been disposed.");
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// 销毁策略
        /// Dispose the policy
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 销毁策略（可重写）
        /// Dispose the policy (overridable)
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
                OnDispose();
            }

            _disposed = true;
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
        ~PoolPolicyBase()
        {
            Dispose(false);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 获取策略的字符串表示
        /// Get the string representation of the policy
        /// </summary>
        public override string ToString()
        {
            return $"[{GetType().Name}] Name={_name}";
        }

        #endregion
    }

    /// <summary>
    /// 泛型池策略抽象基类
    /// Generic Pool Policy Abstract Base Class
    /// </summary>
    /// <typeparam name="T">策略应用的对象类型 / Type of objects the policy applies to</typeparam>
    public abstract class PoolPolicyBase<T> : PoolPolicyBase
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="name">策略名称 / Policy name</param>
        protected PoolPolicyBase(string name = null) : base(name)
        {
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// 验证对象是否有效
        /// Validate if the object is valid
        /// </summary>
        /// <param name="obj">要验证的对象 / Object to validate</param>
        /// <returns>是否有效 / Whether valid</returns>
        protected virtual bool ValidateObject(T obj)
        {
            return obj != null;
        }

        /// <summary>
        /// 检查对象是否有效，如果无效则抛出异常
        /// Check if the object is valid, throw exception if invalid
        /// </summary>
        /// <param name="obj">要检查的对象 / Object to check</param>
        /// <param name="paramName">参数名称 / Parameter name</param>
        protected void ThrowIfInvalidObject(T obj, string paramName = "obj")
        {
            if (!ValidateObject(obj))
            {
                throw new ArgumentException($"Invalid object for policy '{_name}'.", paramName);
            }
        }

        #endregion
    }
}
