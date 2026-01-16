// ==========================================================
// 文件名：DisposableTracker.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 可释放对象追踪器，管理需要释放的对象集合
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 可释放对象追踪器
    /// <para>追踪和管理实现了 IDisposable 接口的对象，确保在容器/作用域销毁时正确释放</para>
    /// <para>Disposable tracker that tracks and manages IDisposable objects for proper cleanup</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于可释放对象的追踪和释放</item>
    /// <item>线程安全：所有操作都是线程安全的</item>
    /// <item>LIFO释放：按添加顺序的逆序释放对象</item>
    /// </list>
    /// 
    /// 核心功能：
    /// <list type="bullet">
    /// <item>追踪 IDisposable 对象</item>
    /// <item>按 LIFO 顺序释放</item>
    /// <item>异常安全的释放流程</item>
    /// <item>诊断信息收集</item>
    /// </list>
    /// </remarks>
    public sealed class DisposableTracker : IDisposable
    {
        #region 内部类型 / Internal Types

        /// <summary>
        /// 追踪条目
        /// </summary>
        private readonly struct TrackedEntry
        {
            /// <summary>
            /// 可释放对象
            /// </summary>
            public readonly IDisposable Disposable;

            /// <summary>
            /// 生命周期类型
            /// </summary>
            public readonly Lifetime Lifetime;

            /// <summary>
            /// 追踪时间
            /// </summary>
            public readonly DateTime TrackedAt;

            /// <summary>
            /// 对象类型名称（用于诊断）
            /// </summary>
            public readonly string TypeName;

            public TrackedEntry(IDisposable disposable, Lifetime lifetime)
            {
                Disposable = disposable;
                Lifetime = lifetime;
                TrackedAt = DateTime.UtcNow;
                TypeName = disposable?.GetType().Name ?? "null";
            }
        }

        #endregion

        #region 字段 / Fields

        /// <summary>
        /// 追踪的可释放对象列表
        /// </summary>
        private readonly List<TrackedEntry> _trackedObjects;

        /// <summary>
        /// 用于快速查找的哈希集合
        /// </summary>
        private readonly HashSet<IDisposable> _trackedSet;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 是否启用诊断
        /// </summary>
        private readonly bool _enableDiagnostics;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private volatile bool _isDisposed;

        /// <summary>
        /// 释放过程中发生的异常
        /// </summary>
        private List<Exception> _disposalExceptions;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取追踪的对象数量
        /// <para>Get the count of tracked objects</para>
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _trackedObjects.Count;
                }
            }
        }

        /// <summary>
        /// 获取是否已释放
        /// <para>Get whether the tracker has been disposed</para>
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 获取释放过程中发生的异常
        /// <para>Get exceptions that occurred during disposal</para>
        /// </summary>
        public IReadOnlyList<Exception> DisposalExceptions
        {
            get
            {
                lock (_syncRoot)
                {
                    return _disposalExceptions?.ToArray() ?? Array.Empty<Exception>();
                }
            }
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建可释放对象追踪器实例
        /// </summary>
        /// <param name="enableDiagnostics">是否启用诊断 / Whether to enable diagnostics</param>
        public DisposableTracker(bool enableDiagnostics = false)
        {
            _enableDiagnostics = enableDiagnostics;
            _trackedObjects = new List<TrackedEntry>();
            _trackedSet = new HashSet<IDisposable>();
        }

        #endregion

        #region 追踪方法 / Tracking Methods

        /// <summary>
        /// 追踪可释放对象
        /// <para>Track a disposable object</para>
        /// </summary>
        /// <param name="instance">要追踪的实例 / Instance to track</param>
        /// <param name="lifetime">生命周期类型 / Lifetime type</param>
        /// <returns>是否成功追踪（如果对象不是 IDisposable 则返回 false）/ Whether successfully tracked</returns>
        public bool Track(object instance, Lifetime lifetime)
        {
            if (_isDisposed)
                return false;

            if (instance == null)
                return false;

            // 只追踪 IDisposable 对象
            if (!(instance is IDisposable disposable))
                return false;

            lock (_syncRoot)
            {
                // 避免重复追踪
                if (_trackedSet.Contains(disposable))
                    return false;

                var entry = new TrackedEntry(disposable, lifetime);
                _trackedObjects.Add(entry);
                _trackedSet.Add(disposable);

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"追踪对象: {entry.TypeName} (Lifetime: {lifetime})");
                }

                return true;
            }
        }

        /// <summary>
        /// 追踪可释放对象（泛型版本）
        /// <para>Track a disposable object (generic version)</para>
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <param name="instance">要追踪的实例 / Instance to track</param>
        /// <param name="lifetime">生命周期类型 / Lifetime type</param>
        /// <returns>是否成功追踪 / Whether successfully tracked</returns>
        public bool Track<T>(T instance, Lifetime lifetime) where T : class
        {
            return Track((object)instance, lifetime);
        }

        /// <summary>
        /// 取消追踪对象
        /// <para>Untrack an object</para>
        /// </summary>
        /// <param name="instance">要取消追踪的实例 / Instance to untrack</param>
        /// <returns>是否成功取消追踪 / Whether successfully untracked</returns>
        public bool Untrack(object instance)
        {
            if (_isDisposed || instance == null)
                return false;

            if (!(instance is IDisposable disposable))
                return false;

            lock (_syncRoot)
            {
                if (!_trackedSet.Contains(disposable))
                    return false;

                _trackedSet.Remove(disposable);
                
                // 从列表中移除
                for (int i = _trackedObjects.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_trackedObjects[i].Disposable, disposable))
                    {
                        if (_enableDiagnostics)
                        {
                            LogDiagnostic($"取消追踪对象: {_trackedObjects[i].TypeName}");
                        }
                        _trackedObjects.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 检查对象是否被追踪
        /// <para>Check if an object is being tracked</para>
        /// </summary>
        /// <param name="instance">要检查的实例 / Instance to check</param>
        /// <returns>是否被追踪 / Whether being tracked</returns>
        public bool IsTracked(object instance)
        {
            if (_isDisposed || instance == null)
                return false;

            if (!(instance is IDisposable disposable))
                return false;

            lock (_syncRoot)
            {
                return _trackedSet.Contains(disposable);
            }
        }

        #endregion

        #region 释放方法 / Disposal Methods

        /// <summary>
        /// 释放所有追踪的对象
        /// <para>Dispose all tracked objects</para>
        /// </summary>
        /// <remarks>
        /// 按 LIFO（后进先出）顺序释放对象，确保依赖关系正确处理。
        /// 释放过程中的异常会被捕获并记录，不会中断其他对象的释放。
        /// </remarks>
        public void DisposeAll()
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"开始释放 {_trackedObjects.Count} 个追踪对象");
                }

                // 按 LIFO 顺序释放
                for (int i = _trackedObjects.Count - 1; i >= 0; i--)
                {
                    var entry = _trackedObjects[i];
                    DisposeEntry(entry);
                }

                _trackedObjects.Clear();
                _trackedSet.Clear();

                if (_enableDiagnostics)
                {
                    var exceptionCount = _disposalExceptions?.Count ?? 0;
                    LogDiagnostic($"释放完成，异常数: {exceptionCount}");
                }
            }
        }

        /// <summary>
        /// 释放指定生命周期的所有对象
        /// <para>Dispose all objects of specified lifetime</para>
        /// </summary>
        /// <param name="lifetime">生命周期类型 / Lifetime type</param>
        public void DisposeByLifetime(Lifetime lifetime)
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                var toDispose = new List<TrackedEntry>();

                // 收集要释放的对象
                for (int i = _trackedObjects.Count - 1; i >= 0; i--)
                {
                    if (_trackedObjects[i].Lifetime == lifetime)
                    {
                        toDispose.Add(_trackedObjects[i]);
                        _trackedSet.Remove(_trackedObjects[i].Disposable);
                        _trackedObjects.RemoveAt(i);
                    }
                }

                if (_enableDiagnostics)
                {
                    LogDiagnostic($"释放 {toDispose.Count} 个 {lifetime} 生命周期对象");
                }

                // 按 LIFO 顺序释放
                for (int i = toDispose.Count - 1; i >= 0; i--)
                {
                    DisposeEntry(toDispose[i]);
                }
            }
        }

        /// <summary>
        /// 释放单个条目
        /// </summary>
        private void DisposeEntry(TrackedEntry entry)
        {
            try
            {
                if (_enableDiagnostics)
                {
                    LogDiagnostic($"释放对象: {entry.TypeName}");
                }

                entry.Disposable?.Dispose();
            }
            catch (Exception ex)
            {
                // 记录异常但不抛出，确保其他对象能继续释放
                _disposalExceptions ??= new List<Exception>();
                _disposalExceptions.Add(ex);

                UnityEngine.Debug.LogWarning(
                    $"[AFramework.DI] 释放对象 {entry.TypeName} 时发生异常: {ex.Message}\n" +
                    $"Exception disposing {entry.TypeName}: {ex.Message}");
            }
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放追踪器及其追踪的所有对象
        /// </summary>
        public void Dispose()
        {
            DisposeAll();
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取诊断信息
        /// <para>Get diagnostic information</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        public string GetDiagnosticInfo()
        {
            lock (_syncRoot)
            {
                var singletonCount = 0;
                var scopedCount = 0;
                var transientCount = 0;

                foreach (var entry in _trackedObjects)
                {
                    switch (entry.Lifetime)
                    {
                        case Lifetime.Singleton:
                            singletonCount++;
                            break;
                        case Lifetime.Scoped:
                            scopedCount++;
                            break;
                        case Lifetime.Transient:
                            transientCount++;
                            break;
                    }
                }

                return $"DisposableTracker[Total={_trackedObjects.Count}, " +
                       $"Singleton={singletonCount}, Scoped={scopedCount}, Transient={transientCount}]";
            }
        }

        /// <summary>
        /// 获取所有追踪对象的类型名称
        /// <para>Get type names of all tracked objects</para>
        /// </summary>
        /// <returns>类型名称集合 / Collection of type names</returns>
        public IReadOnlyList<string> GetTrackedTypeNames()
        {
            lock (_syncRoot)
            {
                var names = new List<string>(_trackedObjects.Count);
                foreach (var entry in _trackedObjects)
                {
                    names.Add($"{entry.TypeName} ({entry.Lifetime})");
                }
                return names;
            }
        }

        private void LogDiagnostic(string message)
        {
            if (_enableDiagnostics)
            {
                UnityEngine.Debug.Log($"[AFramework.DI.DisposableTracker] {message}");
            }
        }

        #endregion
    }
}
