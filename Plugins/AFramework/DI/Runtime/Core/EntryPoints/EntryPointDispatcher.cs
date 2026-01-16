// ==========================================================
// 文件名：EntryPointDispatcher.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Threading, Cysharp.Threading.Tasks
// ==========================================================

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AFramework.DI
{
    /// <summary>
    /// 入口点调度器
    /// <para>负责按正确顺序执行各类入口点方法</para>
    /// </summary>
    public sealed class EntryPointDispatcher : IDisposable
    {
        #region 字段

        private readonly EntryPointRegistry _registry;
        private readonly EntryPointExceptionHandler _exceptionHandler;
        private readonly CancellationTokenSource _cts;

        private bool _isInitialized;
        private bool _isStarted;
        private bool _isDisposed;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建入口点调度器
        /// </summary>
        /// <param name="registry">入口点注册表</param>
        /// <param name="exceptionHandler">异常处理器（可选，默认使用全局处理器）</param>
        public EntryPointDispatcher(
            EntryPointRegistry registry,
            EntryPointExceptionHandler exceptionHandler = null)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _exceptionHandler = exceptionHandler ?? EntryPointExceptionHandler.Default;
            _cts = new CancellationTokenSource();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否已完成初始化阶段
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 是否已完成启动阶段
        /// </summary>
        public bool IsStarted => _isStarted;

        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationToken CancellationToken => _cts.Token;

        #endregion

        #region 初始化阶段

        /// <summary>
        /// 执行初始化阶段
        /// <para>执行顺序：IInitializable → IPostInitializable</para>
        /// </summary>
        public void Initialize()
        {
            ThrowIfDisposed();

            if (_isInitialized) return;

            // 执行 IInitializable
            ExecuteInitializables();

            // 执行 IPostInitializable
            ExecutePostInitializables();

            _isInitialized = true;
        }

        private void ExecuteInitializables()
        {
            foreach (var initializable in _registry.Initializables)
            {
                try
                {
                    initializable.Initialize();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(initializable, ex, EntryPointPhase.Initialize);
                }
            }
        }

        private void ExecutePostInitializables()
        {
            foreach (var postInitializable in _registry.PostInitializables)
            {
                try
                {
                    postInitializable.PostInitialize();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(postInitializable, ex, EntryPointPhase.PostInitialize);
                }
            }
        }

        #endregion

        #region 启动阶段

        /// <summary>
        /// 异步执行启动阶段
        /// <para>执行顺序：IAsyncStartable → IStartable → IPostStartable</para>
        /// </summary>
        /// <param name="cancellation">取消令牌</param>
        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            ThrowIfDisposed();

            if (_isStarted) return;

            // 合并取消令牌
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellation);
            var token = linkedCts.Token;

            // 执行 IAsyncStartable
            await ExecuteAsyncStartablesAsync(token);

            // 执行 IStartable
            ExecuteStartables();

            // 执行 IPostStartable
            ExecutePostStartables();

            _isStarted = true;
        }

        /// <summary>
        /// 同步执行启动阶段（不包含异步启动）
        /// <para>执行顺序：IStartable → IPostStartable</para>
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            if (_isStarted) return;

            // 执行 IStartable
            ExecuteStartables();

            // 执行 IPostStartable
            ExecutePostStartables();

            _isStarted = true;
        }

        private async UniTask ExecuteAsyncStartablesAsync(CancellationToken cancellation)
        {
            foreach (var asyncStartable in _registry.AsyncStartables)
            {
                if (cancellation.IsCancellationRequested) break;

                try
                {
                    await asyncStartable.StartAsync(cancellation);
                }
                catch (OperationCanceledException)
                {
                    // 取消操作，不作为异常处理
                    break;
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(asyncStartable, ex, EntryPointPhase.AsyncStart);
                }
            }
        }

        private void ExecuteStartables()
        {
            foreach (var startable in _registry.Startables)
            {
                try
                {
                    startable.Start();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(startable, ex, EntryPointPhase.Start);
                }
            }
        }

        private void ExecutePostStartables()
        {
            foreach (var postStartable in _registry.PostStartables)
            {
                try
                {
                    postStartable.PostStart();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(postStartable, ex, EntryPointPhase.PostStart);
                }
            }
        }

        #endregion

        #region 更新阶段 (Tick)

        /// <summary>
        /// 执行每帧更新
        /// <para>执行顺序：ITickable → IPostTickable</para>
        /// </summary>
        public void Tick()
        {
            if (_isDisposed || !_isStarted) return;

            ExecuteTickables();
            ExecutePostTickables();
        }

        private void ExecuteTickables()
        {
            var tickables = _registry.Tickables;
            var count = tickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    tickables[i].Tick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(tickables[i], ex, EntryPointPhase.Tick);
                }
            }
        }

        private void ExecutePostTickables()
        {
            var postTickables = _registry.PostTickables;
            var count = postTickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    postTickables[i].PostTick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(postTickables[i], ex, EntryPointPhase.PostTick);
                }
            }
        }

        #endregion

        #region 固定更新阶段 (FixedTick)

        /// <summary>
        /// 执行固定时间间隔更新
        /// <para>执行顺序：IFixedTickable → IPostFixedTickable</para>
        /// </summary>
        public void FixedTick()
        {
            if (_isDisposed || !_isStarted) return;

            ExecuteFixedTickables();
            ExecutePostFixedTickables();
        }

        private void ExecuteFixedTickables()
        {
            var fixedTickables = _registry.FixedTickables;
            var count = fixedTickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    fixedTickables[i].FixedTick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(fixedTickables[i], ex, EntryPointPhase.FixedTick);
                }
            }
        }

        private void ExecutePostFixedTickables()
        {
            var postFixedTickables = _registry.PostFixedTickables;
            var count = postFixedTickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    postFixedTickables[i].PostFixedTick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(postFixedTickables[i], ex, EntryPointPhase.PostFixedTick);
                }
            }
        }

        #endregion

        #region 延迟更新阶段 (LateTick)

        /// <summary>
        /// 执行延迟更新
        /// <para>执行顺序：ILateTickable → IPostLateTickable</para>
        /// </summary>
        public void LateTick()
        {
            if (_isDisposed || !_isStarted) return;

            ExecuteLateTickables();
            ExecutePostLateTickables();
        }

        private void ExecuteLateTickables()
        {
            var lateTickables = _registry.LateTickables;
            var count = lateTickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    lateTickables[i].LateTick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(lateTickables[i], ex, EntryPointPhase.LateTick);
                }
            }
        }

        private void ExecutePostLateTickables()
        {
            var postLateTickables = _registry.PostLateTickables;
            var count = postLateTickables.Count;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    postLateTickables[i].PostLateTick();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleException(postLateTickables[i], ex, EntryPointPhase.PostLateTick);
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _cts.Cancel();
            _cts.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(EntryPointDispatcher));
            }
        }

        #endregion
    }
}
