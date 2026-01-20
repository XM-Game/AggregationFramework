// ==========================================================
// 文件名：DefaultLifecyclePolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 默认生命周期策略，适用于简单对象
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 默认生命周期策略
    /// Default Lifecycle Policy
    /// 
    /// <para>使用 Activator.CreateInstance 创建对象，无特殊清理逻辑</para>
    /// <para>Uses Activator.CreateInstance to create objects, no special cleanup logic</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    public class DefaultLifecyclePolicy<T> : IPoolLifecyclePolicy<T> where T : new()
    {
        #region Properties

        /// <inheritdoc />
        public string Name => "DefaultLifecycle";

        /// <inheritdoc />
        public string Description => "Default lifecycle policy using parameterless constructor";

        #endregion

        #region IPoolLifecyclePolicy Implementation

        /// <inheritdoc />
        public T Create()
        {
            return new T();
        }

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            // 默认无操作
            // Default: no operation
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            // 如果对象实现了 IDisposable，调用 Dispose
            // If object implements IDisposable, call Dispose
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <inheritdoc />
        public bool Validate()
        {
            return typeof(T).GetConstructor(Type.EmptyTypes) != null;
        }

        #endregion
    }
}
