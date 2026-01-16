// ==========================================================
// 文件名：EntryPointRegistry.cs
// 命名空间: AFramework.DI
// 依赖: System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 入口点注册表
    /// <para>管理所有入口点实例的注册和查询</para>
    /// </summary>
    public sealed class EntryPointRegistry : IDisposable
    {
        #region 字段

        private readonly List<IInitializable> _initializables = new();
        private readonly List<IPostInitializable> _postInitializables = new();
        private readonly List<IAsyncStartable> _asyncStartables = new();
        private readonly List<IStartable> _startables = new();
        private readonly List<IPostStartable> _postStartables = new();
        private readonly List<ITickable> _tickables = new();
        private readonly List<IPostTickable> _postTickables = new();
        private readonly List<IFixedTickable> _fixedTickables = new();
        private readonly List<IPostFixedTickable> _postFixedTickables = new();
        private readonly List<ILateTickable> _lateTickables = new();
        private readonly List<IPostLateTickable> _postLateTickables = new();

        private readonly object _lock = new();
        private bool _isDisposed;

        #endregion

        #region 属性 - 只读集合访问

        /// <summary>初始化入口点集合</summary>
        public IReadOnlyList<IInitializable> Initializables => _initializables;

        /// <summary>后初始化入口点集合</summary>
        public IReadOnlyList<IPostInitializable> PostInitializables => _postInitializables;

        /// <summary>异步启动入口点集合</summary>
        public IReadOnlyList<IAsyncStartable> AsyncStartables => _asyncStartables;

        /// <summary>启动入口点集合</summary>
        public IReadOnlyList<IStartable> Startables => _startables;

        /// <summary>后启动入口点集合</summary>
        public IReadOnlyList<IPostStartable> PostStartables => _postStartables;

        /// <summary>更新入口点集合</summary>
        public IReadOnlyList<ITickable> Tickables => _tickables;

        /// <summary>后更新入口点集合</summary>
        public IReadOnlyList<IPostTickable> PostTickables => _postTickables;

        /// <summary>固定更新入口点集合</summary>
        public IReadOnlyList<IFixedTickable> FixedTickables => _fixedTickables;

        /// <summary>后固定更新入口点集合</summary>
        public IReadOnlyList<IPostFixedTickable> PostFixedTickables => _postFixedTickables;

        /// <summary>延迟更新入口点集合</summary>
        public IReadOnlyList<ILateTickable> LateTickables => _lateTickables;

        /// <summary>后延迟更新入口点集合</summary>
        public IReadOnlyList<IPostLateTickable> PostLateTickables => _postLateTickables;

        #endregion

        #region 统计属性

        /// <summary>
        /// 获取注册的入口点总数
        /// </summary>
        public int TotalCount
        {
            get
            {
                lock (_lock)
                {
                    return _initializables.Count +
                           _postInitializables.Count +
                           _asyncStartables.Count +
                           _startables.Count +
                           _postStartables.Count +
                           _tickables.Count +
                           _postTickables.Count +
                           _fixedTickables.Count +
                           _postFixedTickables.Count +
                           _lateTickables.Count +
                           _postLateTickables.Count;
                }
            }
        }

        /// <summary>
        /// 是否有任何更新类型的入口点
        /// </summary>
        public bool HasTickables
        {
            get
            {
                lock (_lock)
                {
                    return _tickables.Count > 0 ||
                           _postTickables.Count > 0 ||
                           _fixedTickables.Count > 0 ||
                           _postFixedTickables.Count > 0 ||
                           _lateTickables.Count > 0 ||
                           _postLateTickables.Count > 0;
                }
            }
        }

        #endregion

        #region 注册方法

        /// <summary>
        /// 注册入口点实例
        /// <para>自动检测实例实现的所有入口点接口并注册</para>
        /// </summary>
        /// <param name="instance">要注册的实例</param>
        public void Register(object instance)
        {
            if (instance == null) return;
            ThrowIfDisposed();

            lock (_lock)
            {
                // 初始化相关
                if (instance is IInitializable initializable)
                    AddIfNotExists(_initializables, initializable);

                if (instance is IPostInitializable postInitializable)
                    AddIfNotExists(_postInitializables, postInitializable);

                // 启动相关
                if (instance is IAsyncStartable asyncStartable)
                    AddIfNotExists(_asyncStartables, asyncStartable);

                if (instance is IStartable startable)
                    AddIfNotExists(_startables, startable);

                if (instance is IPostStartable postStartable)
                    AddIfNotExists(_postStartables, postStartable);

                // 更新相关
                if (instance is ITickable tickable)
                    AddIfNotExists(_tickables, tickable);

                if (instance is IPostTickable postTickable)
                    AddIfNotExists(_postTickables, postTickable);

                // 固定更新相关
                if (instance is IFixedTickable fixedTickable)
                    AddIfNotExists(_fixedTickables, fixedTickable);

                if (instance is IPostFixedTickable postFixedTickable)
                    AddIfNotExists(_postFixedTickables, postFixedTickable);

                // 延迟更新相关
                if (instance is ILateTickable lateTickable)
                    AddIfNotExists(_lateTickables, lateTickable);

                if (instance is IPostLateTickable postLateTickable)
                    AddIfNotExists(_postLateTickables, postLateTickable);
            }
        }

        /// <summary>
        /// 批量注册入口点实例
        /// </summary>
        /// <param name="instances">要注册的实例集合</param>
        public void RegisterRange(IEnumerable<object> instances)
        {
            if (instances == null) return;

            foreach (var instance in instances)
            {
                Register(instance);
            }
        }

        #endregion

        #region 注销方法

        /// <summary>
        /// 注销入口点实例
        /// </summary>
        /// <param name="instance">要注销的实例</param>
        public void Unregister(object instance)
        {
            if (instance == null) return;
            ThrowIfDisposed();

            lock (_lock)
            {
                if (instance is IInitializable initializable)
                    _initializables.Remove(initializable);

                if (instance is IPostInitializable postInitializable)
                    _postInitializables.Remove(postInitializable);

                if (instance is IAsyncStartable asyncStartable)
                    _asyncStartables.Remove(asyncStartable);

                if (instance is IStartable startable)
                    _startables.Remove(startable);

                if (instance is IPostStartable postStartable)
                    _postStartables.Remove(postStartable);

                if (instance is ITickable tickable)
                    _tickables.Remove(tickable);

                if (instance is IPostTickable postTickable)
                    _postTickables.Remove(postTickable);

                if (instance is IFixedTickable fixedTickable)
                    _fixedTickables.Remove(fixedTickable);

                if (instance is IPostFixedTickable postFixedTickable)
                    _postFixedTickables.Remove(postFixedTickable);

                if (instance is ILateTickable lateTickable)
                    _lateTickables.Remove(lateTickable);

                if (instance is IPostLateTickable postLateTickable)
                    _postLateTickables.Remove(postLateTickable);
            }
        }

        /// <summary>
        /// 清空所有注册的入口点
        /// </summary>
        public void Clear()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                _initializables.Clear();
                _postInitializables.Clear();
                _asyncStartables.Clear();
                _startables.Clear();
                _postStartables.Clear();
                _tickables.Clear();
                _postTickables.Clear();
                _fixedTickables.Clear();
                _postFixedTickables.Clear();
                _lateTickables.Clear();
                _postLateTickables.Clear();
            }
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 检查实例是否已注册为任意入口点
        /// </summary>
        /// <param name="instance">要检查的实例</param>
        /// <returns>是否已注册</returns>
        public bool IsRegistered(object instance)
        {
            if (instance == null) return false;

            lock (_lock)
            {
                return (instance is IInitializable i && _initializables.Contains(i)) ||
                       (instance is IPostInitializable pi && _postInitializables.Contains(pi)) ||
                       (instance is IAsyncStartable asy && _asyncStartables.Contains(asy)) ||
                       (instance is IStartable s && _startables.Contains(s)) ||
                       (instance is IPostStartable ps && _postStartables.Contains(ps)) ||
                       (instance is ITickable t && _tickables.Contains(t)) ||
                       (instance is IPostTickable pt && _postTickables.Contains(pt)) ||
                       (instance is IFixedTickable ft && _fixedTickables.Contains(ft)) ||
                       (instance is IPostFixedTickable pft && _postFixedTickables.Contains(pft)) ||
                       (instance is ILateTickable lt && _lateTickables.Contains(lt)) ||
                       (instance is IPostLateTickable plt && _postLateTickables.Contains(plt));
            }
        }

        /// <summary>
        /// 获取指定类型的入口点数量
        /// </summary>
        /// <typeparam name="T">入口点接口类型</typeparam>
        /// <returns>数量</returns>
        public int GetCount<T>() where T : class
        {
            lock (_lock)
            {
                return typeof(T) switch
                {
                    var t when t == typeof(IInitializable) => _initializables.Count,
                    var t when t == typeof(IPostInitializable) => _postInitializables.Count,
                    var t when t == typeof(IAsyncStartable) => _asyncStartables.Count,
                    var t when t == typeof(IStartable) => _startables.Count,
                    var t when t == typeof(IPostStartable) => _postStartables.Count,
                    var t when t == typeof(ITickable) => _tickables.Count,
                    var t when t == typeof(IPostTickable) => _postTickables.Count,
                    var t when t == typeof(IFixedTickable) => _fixedTickables.Count,
                    var t when t == typeof(IPostFixedTickable) => _postFixedTickables.Count,
                    var t when t == typeof(ILateTickable) => _lateTickables.Count,
                    var t when t == typeof(IPostLateTickable) => _postLateTickables.Count,
                    _ => 0
                };
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_lock)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(EntryPointRegistry));
            }
        }

        #endregion

        #region 私有方法

        private static void AddIfNotExists<T>(List<T> list, T item) where T : class
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        #endregion
    }
}
