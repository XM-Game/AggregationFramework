// ==========================================================
// 文件名：AsyncPoolWarmer.cs
// 命名空间: AFramework.Pool.Warming
// 依赖: System, System.Threading, AFramework.Pool
// 功能: 异步对象池预热器
// ==========================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AFramework.Pool.Warming
{
    /// <summary>
    /// 异步对象池预热器
    /// Asynchronous Pool Warmer
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
    /// <remarks>
    /// 在后台线程执行预热，不阻塞主线程
    /// Executes warmup in background thread without blocking main thread
    /// </remarks>
    public class AsyncPoolWarmer<T> where T : class
    {
        #region 字段 Fields

        private readonly IObjectPool<T> _pool;
        private readonly int _batchSize;
        private readonly int _delayMs;

        private CancellationTokenSource _cts;
        private Task _warmupTask;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取是否正在预热
        /// Whether warmup is in progress
        /// </summary>
        public bool IsWarming => _warmupTask != null && !_warmupTask.IsCompleted;

        /// <summary>
        /// 获取当前进度
        /// Current progress
        /// </summary>
        public float Progress { get; private set; }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化异步预热器
        /// Initialize async warmer
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        /// <param name="batchSize">批次大小（默认 10）/ Batch size (default 10)</param>
        /// <param name="delayMs">批次间延迟（毫秒，默认 0）/ Delay between batches (ms, default 0)</param>
        public AsyncPoolWarmer(IObjectPool<T> pool, int batchSize = 10, int delayMs = 0)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _batchSize = Math.Max(1, batchSize);
            _delayMs = Math.Max(0, delayMs);
        }

        #endregion

        #region 预热方法 Warmup Methods

        /// <summary>
        /// 开始异步预热
        /// Start async warmup
        /// </summary>
        /// <param name="count">预热对象数量 / Number of objects to warmup</param>
        /// <param name="onProgress">进度回调 / Progress callback</param>
        /// <param name="onComplete">完成回调 / Completion callback</param>
        /// <returns>预热任务 / Warmup task</returns>
        public Task WarmupAsync(
            int count,
            Action<float> onProgress = null,
            Action onComplete = null)
        {
            if (IsWarming)
            {
                throw new InvalidOperationException("预热已在进行中 / Warmup is already in progress");
            }

            _cts = new CancellationTokenSource();
            _warmupTask = WarmupInternalAsync(count, onProgress, onComplete, _cts.Token);
            return _warmupTask;
        }

        /// <summary>
        /// 取消预热
        /// Cancel warmup
        /// </summary>
        public void Cancel()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// 等待预热完成
        /// Wait for warmup completion
        /// </summary>
        public async Task WaitForCompletionAsync()
        {
            if (_warmupTask != null)
            {
                await _warmupTask;
            }
        }

        #endregion

        #region 内部方法 Internal Methods

        /// <summary>
        /// 内部异步预热实现
        /// Internal async warmup implementation
        /// </summary>
        private async Task WarmupInternalAsync(
            int count,
            Action<float> onProgress,
            Action onComplete,
            CancellationToken cancellationToken)
        {
            Progress = 0f;
            int created = 0;

            try
            {
                // 分批创建对象
                // Create objects in batches
                while (created < count && !cancellationToken.IsCancellationRequested)
                {
                    int batchCount = Math.Min(_batchSize, count - created);

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < batchCount; i++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            try
                            {
                                var obj = _pool.Get();
                                _pool.Return(obj);
                            }
                            catch (Exception)
                            {
                                // 忽略单个对象创建失败
                                // Ignore individual object creation failure
                            }
                        }
                    }, cancellationToken);

                    created += batchCount;
                    Progress = (float)created / count;

                    // 触发进度回调（在主线程中）
                    // Trigger progress callback (in main thread)
                    onProgress?.Invoke(Progress);

                    // 批次间延迟
                    // Delay between batches
                    if (_delayMs > 0 && created < count)
                    {
                        await Task.Delay(_delayMs, cancellationToken);
                    }
                }

                Progress = 1f;
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // 预热被取消
                // Warmup cancelled
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        #endregion
    }
}
