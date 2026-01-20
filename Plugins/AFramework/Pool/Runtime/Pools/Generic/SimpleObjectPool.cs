// ==========================================================
// 文件名：SimpleObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 简化的对象池实现，使用合并的生命周期策略
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 简化的对象池实现
    /// Simplified Object Pool Implementation
    /// 
    /// <para>使用合并的生命周期策略，简化配置</para>
    /// <para>Uses merged lifecycle policy to simplify configuration</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 改进点：
    /// - 使用单一的生命周期策略替代多个策略
    /// - 容量策略可选，默认使用固定容量
    /// - 配置更简单，代码更清晰
    /// 
    /// 使用示例：
    /// <code>
    /// // 最简单的用法
    /// var pool = new SimpleObjectPool&lt;MyObject&gt;(() => new MyObject());
    /// 
    /// // 使用预设策略
    /// var pool = new SimpleObjectPool&lt;MyObject&gt;(PoolPolicyPresets.Simple&lt;MyObject&gt;());
    /// 
    /// // 自定义策略
    /// var pool = new SimpleObjectPool&lt;MyObject&gt;(
    ///     PoolPolicyPresets.Custom(
    ///         createFunc: () => new MyObject(),
    ///         returnAction: obj => obj.Reset(),
    ///         destroyAction: obj => obj.Dispose()
    ///     )
    /// );
    /// </code>
    /// </remarks>
    public class SimpleObjectPool<T> : IObjectPoolCore<T>, IPoolWarmer, IPoolStatisticsProvider
    {
        #region Fields

        private readonly Stack<T> _objects;
        private readonly IPoolLifecyclePolicy<T> _lifecyclePolicy;
        private readonly IPoolCapacityPolicy _capacityPolicy;
        private readonly int _maxCapacity;
        private readonly string _name;
        private readonly object _lock = new object();

        private PoolState _state;
        private int _activeCount;
        private bool _disposed;

        // 可选的统计组件
        private PoolStatisticsComponent _statistics;

        #endregion

        #region Properties

        /// <inheritdoc />
        public string Name => _name;

        /// <inheritdoc />
        public PoolState State => _state;

        /// <inheritdoc />
        public int AvailableCount => _objects.Count;

        /// <inheritdoc />
        public int ActiveCount => _activeCount;

        /// <summary>
        /// 总对象数
        /// Total object count
        /// </summary>
        public int TotalCount => _objects.Count + _activeCount;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数（使用生命周期策略）
        /// Constructor (using lifecycle policy)
        /// </summary>
        /// <param name="lifecyclePolicy">生命周期策略 / Lifecycle policy</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="capacityPolicy">容量策略（可选）/ Capacity policy (optional)</param>
        /// <param name="enableStatistics">是否启用统计 / Whether to enable statistics</param>
        /// <param name="name">池名称 / Pool name</param>
        public SimpleObjectPool(
            IPoolLifecyclePolicy<T> lifecyclePolicy,
            int maxCapacity = 100,
            IPoolCapacityPolicy capacityPolicy = null,
            bool enableStatistics = true,
            string name = null)
        {
            _lifecyclePolicy = lifecyclePolicy ?? throw new ArgumentNullException(nameof(lifecyclePolicy));
            _maxCapacity = maxCapacity > 0 ? maxCapacity : throw new ArgumentOutOfRangeException(nameof(maxCapacity));
            _capacityPolicy = capacityPolicy;
            _name = name ?? $"SimplePool<{typeof(T).Name}>";
            _objects = new Stack<T>(maxCapacity);
            _state = PoolState.Uninitialized;
            _activeCount = 0;
            _disposed = false;

            // 可选的统计组件
            if (enableStatistics)
            {
                _statistics = new PoolStatisticsComponent();
            }

            // 自动初始化
            Initialize();
        }

        /// <summary>
        /// 构造函数（使用创建函数）
        /// Constructor (using create function)
        /// </summary>
        /// <param name="createFunc">创建函数 / Create function</param>
        /// <param name="maxCapacity">最大容量 / Maximum capacity</param>
        /// <param name="returnAction">归还时的清理操作 / Cleanup action on return</param>
        /// <param name="destroyAction">销毁时的清理操作 / Cleanup action on destroy</param>
        /// <param name="enableStatistics">是否启用统计 / Whether to enable statistics</param>
        /// <param name="name">池名称 / Pool name</param>
        public SimpleObjectPool(
            Func<T> createFunc,
            int maxCapacity = 100,
            Action<T> returnAction = null,
            Action<T> destroyAction = null,
            bool enableStatistics = true,
            string name = null)
            : this(
                new FuncLifecyclePolicy<T>(createFunc, returnAction, destroyAction),
                maxCapacity,
                null,
                enableStatistics,
                name)
        {
        }

        #endregion

        #region Initialization

        private void Initialize()
        {
            lock (_lock)
            {
                if (_state != PoolState.Uninitialized)
                    return;

                _statistics?.Initialize();

                // 如果有容量策略，执行预热
                if (_capacityPolicy != null)
                {
                    int initialCapacity = _capacityPolicy.GetInitialCapacity();
                    if (initialCapacity > 0)
                    {
                        Warmup(initialCapacity);
                    }
                }

                _state = PoolState.Active;
            }
        }

        #endregion

        #region IObjectPoolCore Implementation

        /// <inheritdoc />
        public T Get()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                T obj;

                if (_objects.Count > 0)
                {
                    obj = _objects.Pop();
                    _statistics?.RecordHit();
                }
                else
                {
                    obj = _lifecyclePolicy.Create();
                    _statistics?.RecordMiss();
                    _statistics?.RecordCreation();
                }

                // 调用对象获取回调
                if (obj is IPooledObject pooledObject)
                {
                    pooledObject.OnGet();
                }

                _activeCount++;
                _statistics?.RecordActivation();
                _statistics?.SetIdleCount(_objects.Count);

                return obj;
            }
        }

        /// <inheritdoc />
        public bool Return(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            ThrowIfDisposed();

            lock (_lock)
            {
                // 调用对象归还回调
                if (obj is IPooledObject pooledObject)
                {
                    pooledObject.OnReturn();
                }

                // 调用生命周期策略的归还方法
                _lifecyclePolicy.OnReturn(obj);

                // 检查容量限制
                bool canReturn = _objects.Count < _maxCapacity;
                if (_capacityPolicy != null)
                {
                    canReturn = canReturn && _capacityPolicy.CanReturn(_objects.Count, _maxCapacity);
                }

                if (canReturn)
                {
                    _objects.Push(obj);
                    _activeCount--;
                    _statistics?.RecordReturn();
                    _statistics?.SetIdleCount(_objects.Count);
                    return true;
                }
                else
                {
                    // 池已满，销毁对象
                    DestroyObject(obj);
                    _activeCount--;
                    _statistics?.RecordDestruction();
                    _statistics?.RecordReturn();
                    _statistics?.SetIdleCount(_objects.Count);
                    return false;
                }
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                while (_objects.Count > 0)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statistics?.RecordDestruction();
                }

                _statistics?.SetIdleCount(0);
            }
        }

        #endregion

        #region IPoolWarmer Implementation

        /// <inheritdoc />
        public bool IsWarmedUp { get; private set; }

        /// <inheritdoc />
        public float Progress { get; private set; }

        /// <inheritdoc />
        public void Warmup(int count)
        {
            ThrowIfDisposed();

            if (count <= 0)
            {
                IsWarmedUp = true;
                Progress = 1.0f;
                return;
            }

            lock (_lock)
            {
                int targetCount = Math.Min(count, _maxCapacity);

                for (int i = _objects.Count; i < targetCount; i++)
                {
                    T obj = _lifecyclePolicy.Create();
                    _objects.Push(obj);
                    _statistics?.RecordCreation();
                }

                _statistics?.SetIdleCount(_objects.Count);
                IsWarmedUp = true;
                Progress = 1.0f;
            }
        }

        /// <inheritdoc />
        public System.Threading.Tasks.Task WarmupAsync(int count, System.Threading.CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (count <= 0)
            {
                IsWarmedUp = true;
                Progress = 1.0f;
                return System.Threading.Tasks.Task.CompletedTask;
            }

            return System.Threading.Tasks.Task.Run(() =>
            {
                lock (_lock)
                {
                    int targetCount = Math.Min(count, _maxCapacity);
                    int current = _objects.Count;

                    for (int i = current; i < targetCount; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        T obj = _lifecyclePolicy.Create();
                        _objects.Push(obj);
                        _statistics?.RecordCreation();

                        Progress = (float)(i - current + 1) / (targetCount - current);
                    }

                    _statistics?.SetIdleCount(_objects.Count);
                    IsWarmedUp = true;
                    Progress = 1.0f;
                }
            }, cancellationToken);
        }

        /// <inheritdoc />
        public void WarmupFrameDistributed(int count, int objectsPerFrame = 1, Action<float> onProgress = null, Action onComplete = null)
        {
            // SimpleObjectPool 不支持分帧预热，直接同步预热
            // SimpleObjectPool does not support frame-distributed warmup, use synchronous warmup instead
            Warmup(count);
            onProgress?.Invoke(1.0f);
            onComplete?.Invoke();
        }

        /// <inheritdoc />
        public void UpdateWarmup()
        {
            // SimpleObjectPool 不支持分帧预热，此方法为空实现
            // SimpleObjectPool does not support frame-distributed warmup, this method is empty
        }

        /// <inheritdoc />
        public void CancelWarmup()
        {
            // SimpleObjectPool 不支持分帧预热，此方法为空实现
            // SimpleObjectPool does not support frame-distributed warmup, this method is empty
        }

        /// <inheritdoc />
        public int Shrink(int targetCount)
        {
            ThrowIfDisposed();

            if (targetCount < 0)
                throw new ArgumentOutOfRangeException(nameof(targetCount));

            lock (_lock)
            {
                int removedCount = 0;
                while (_objects.Count > targetCount)
                {
                    T obj = _objects.Pop();
                    DestroyObject(obj);
                    _statistics?.RecordDestruction();
                    removedCount++;
                }

                _statistics?.SetIdleCount(_objects.Count);
                return removedCount;
            }
        }

        #endregion

        #region IPoolStatisticsProvider Implementation

        /// <summary>
        /// 获取统计信息
        /// Get statistics
        /// </summary>
        /// <returns>统计信息 / Statistics</returns>
        public IPoolStatistics GetStatistics()
        {
            lock (_lock)
            {
                if (_statistics == null)
                    return null;

                // 返回快照作为统计信息
                // Return snapshot as statistics
                var snapshot = _statistics.CreateSnapshot();
                return new PoolStatisticsSnapshotAdapter(snapshot);
            }
        }

        #endregion

        #region Private Methods

        private void DestroyObject(T obj)
        {
            if (obj == null)
                return;

            // 调用对象销毁回调
            if (obj is IPooledObject pooledObject)
            {
                pooledObject.OnDestroy();
            }

            // 调用生命周期策略的销毁方法
            _lifecyclePolicy.OnDestroy(obj);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new PoolDisposedException($"Pool '{_name}' has been disposed.");
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_lock)
            {
                try
                {
                    Clear();
                    _statistics?.Dispose();
                }
                finally
                {
                    _state = PoolState.Disposed;
                    _disposed = true;
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// 快照适配器，将快照转换为统计接口
        /// Snapshot Adapter, converts snapshot to statistics interface
        /// </summary>
        private class PoolStatisticsSnapshotAdapter : IPoolStatistics
        {
            private readonly PoolStatisticsSnapshot _snapshot;

            public PoolStatisticsSnapshotAdapter(PoolStatisticsSnapshot snapshot)
            {
                _snapshot = snapshot;
            }

            public long TotalCreated => _snapshot.TotalCreated;
            public long TotalDestroyed => _snapshot.TotalDestroyed;
            public int CurrentActive => _snapshot.CurrentActive;
            public int CurrentIdle => _snapshot.CurrentIdle;
            public int CurrentTotal => _snapshot.CurrentTotal;
            public long TotalGets => _snapshot.TotalGets;
            public long TotalReturns => _snapshot.TotalReturns;
            public long Hits => _snapshot.Hits;
            public long Misses => _snapshot.Misses;
            public double HitRate => _snapshot.HitRate;
            public double MissRate => _snapshot.MissRate;
            public double AverageGetTime => _snapshot.AverageGetTime;
            public double AverageReturnTime => _snapshot.AverageReturnTime;
            public int PeakActive => _snapshot.PeakActive;
            public int PeakTotal => _snapshot.PeakTotal;
            public long EstimatedMemoryUsage => _snapshot.EstimatedMemoryUsage;
            public DateTime CreatedTime => _snapshot.CreatedTime;
            public TimeSpan Uptime => _snapshot.Uptime;
            public DateTime? LastGetTime => null; // 快照不包含此信息
            public DateTime? LastReturnTime => null; // 快照不包含此信息

            public PoolStatisticsSnapshot CreateSnapshot() => _snapshot;
            public void Reset() { /* 快照不支持重置 */ }
        }

        #endregion
    }

    /// <summary>
    /// 统计信息提供者接口
    /// Statistics Provider Interface
    /// </summary>
    public interface IPoolStatisticsProvider
    {
        /// <summary>
        /// 获取统计信息
        /// Get statistics
        /// </summary>
        /// <returns>统计信息 / Statistics</returns>
        IPoolStatistics GetStatistics();
    }
}
