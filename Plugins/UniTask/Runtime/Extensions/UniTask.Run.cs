#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    /// <summary>
    /// UniTask 运行方法
    /// </summary>
    public partial struct UniTask
    {
        #region OBSOLETE_RUN 过时方法

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask Run(Action action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask Run(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, state, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask Run(Func<UniTask> action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask Run(Func<object, UniTask> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(action, state, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask<T> Run<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask<T> Run<T>(Func<UniTask<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask<T> Run<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, state, configureAwait, cancellationToken);
        }

        [Obsolete("UniTask.Run is similar as Task.Run, it uses ThreadPool. For equivalent behaviour, use UniTask.RunOnThreadPool instead. If you don't want to use ThreadPool, you can use UniTask.Void(async void) or UniTask.Create(async UniTask) too.")]
        public static UniTask<T> Run<T>(Func<object, UniTask<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            return RunOnThreadPool(func, state, configureAwait, cancellationToken);
        }

        #endregion

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="action">动作</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask RunOnThreadPool(Action action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
            await UniTask.SwitchToThreadPool();   // 切换到后台线程
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
            if (configureAwait)   // 如果需要等待
            {
                try
                {
                    action();   // 执行动作
                }
                finally
                {
                    await UniTask.Yield();   // 等待
                }
            }
            else
            {
                action();   // 执行动作
            }
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="action">动作</param>
        /// <param name="state">状态</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask RunOnThreadPool(Action<object> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            await UniTask.SwitchToThreadPool();   // 切换到后台线程

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            if (configureAwait)   // 如果需要等待
            {
                try
                {
                    action(state);   // 执行动作
                }
                finally
                {
                    await UniTask.Yield();   // 等待
                }
            }
            else
            {
                action(state);   // 执行动作
            }

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="action">动作</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask RunOnThreadPool(Func<UniTask> action, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            await UniTask.SwitchToThreadPool();   // 切换到后台线程

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求 

            if (configureAwait)   // 如果需要等待
            {
                try
                {
                    await action();   // 执行动作
                }
                finally
                {
                    await UniTask.Yield();   // 等待
                }
            }
            else
            {
                await action();   // 执行动作
            }

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="action">动作</param>
        /// <param name="state">状态</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask RunOnThreadPool(Func<object, UniTask> action, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            await UniTask.SwitchToThreadPool();   // 切换到后台线程

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            if (configureAwait)   // 如果需要等待
            {
                try
                {
                    await action(state);   // 执行动作
                }
                finally
                {
                    await UniTask.Yield();   // 等待
                }
            }
            else
            {
                await action(state);   // 执行动作
            }

            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="func">函数</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask<T> RunOnThreadPool<T>(Func<T> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return func();
                }
                finally
                {
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                return func();
            }
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="func">函数</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask<T> RunOnThreadPool<T>(Func<UniTask<T>> func, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();   // 抛出取消请求

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return await func();
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                var result = await func();
                cancellationToken.ThrowIfCancellationRequested();
                return result;
            }
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="func">函数</param>
        /// <param name="state">状态</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask<T> RunOnThreadPool<T>(Func<object, T> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return func(state);
                }
                finally
                {
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                return func(state);
            }
        }

        /// <summary>在后台线程执行动作并返回主线程 if configureAwait = true.</summary>
        /// <param name="func">函数</param>
        /// <param name="state">状态</param>
        /// <param name="configureAwait">是否等待</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>UniTask</returns>
        public static async UniTask<T> RunOnThreadPool<T>(Func<object, UniTask<T>> func, object state, bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return await func(state);
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                var result = await func(state);
                cancellationToken.ThrowIfCancellationRequested();
                return result;
            }
        }
    }
}

