// ==========================================================
// 文件名：FuncLifecyclePolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 基于函数的生命周期策略
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 基于函数的生命周期策略
    /// Function-based Lifecycle Policy
    /// 
    /// <para>使用委托函数定义对象的创建和清理逻辑</para>
    /// <para>Uses delegate functions to define object creation and cleanup logic</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    public class FuncLifecyclePolicy<T> : IPoolLifecyclePolicy<T>
    {
        #region Fields

        private readonly Func<T> _createFunc;
        private readonly Action<T> _returnAction;
        private readonly Action<T> _destroyAction;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name => "FuncLifecycle";

        /// <inheritdoc />
        public string Description => "Function-based lifecycle policy";

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="returnAction">归还时的清理操作（可选）/ Cleanup action on return (optional)</param>
        /// <param name="destroyAction">销毁时的清理操作（可选）/ Cleanup action on destroy (optional)</param>
        public FuncLifecyclePolicy(
            Func<T> createFunc,
            Action<T> returnAction = null,
            Action<T> destroyAction = null)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _returnAction = returnAction;
            _destroyAction = destroyAction;
        }

        #endregion

        #region IPoolLifecyclePolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            return _createFunc();
        }

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            _returnAction?.Invoke(obj);
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            _destroyAction?.Invoke(obj);

            // 如果没有自定义销毁逻辑，且对象实现了 IDisposable，调用 Dispose
            // If no custom destroy logic and object implements IDisposable, call Dispose
            if (_destroyAction == null && obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return _createFunc != null;
        }

        #endregion
    }
}
