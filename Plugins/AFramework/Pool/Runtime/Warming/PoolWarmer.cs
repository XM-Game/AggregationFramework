// ==========================================================
// 文件名：PoolWarmer.cs
// 命名空间: AFramework.Pool.Warming
// 依赖: System, System.Threading, AFramework.Pool
// 功能: 对象池预热器实现
// ==========================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AFramework.Pool.Warming
{
    /// <summary>
    /// 对象池预热器实现
    /// Pool Warmer Implementation
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Pooled object type</typeparam>
    /// <remarks>
    /// 提供多种预热策略，支持同步、异步和分帧预热
    /// Provides multiple warmup strategies, supports synchronous, asynchronous and frame-distributed warmup
    /// </remarks>
    public class PoolWarmer<T> : IPoolWarmer where T : class
    {
        #region 字段 Fields

        private readonly IObjectPool<T> _pool;
        private readonly object _lock = new object();

        // 分帧预热状态
        // Frame-distributed warmup state
        private bool _isWarmingUp;
        private int _targetCount;
        private int _currentCount;
        private int _objectsPerFrame;
        private Action<float> _onProgress;
        private Action _onComplete;

        #endregion

        #region 属性 Properties

        public bool IsWarmedUp { get; private set; }

        public float Progress
        {
            get
            {
                lock (_lock)
                {
                    if (_targetCount == 0) return 1.0f;
                    return (float)_currentCount / _targetCount;
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化预热器
        /// Initialize warmer
        /// </summary>
        /// <param name="pool">对象池 / Object pool</param>
        public PoolWarmer(IObjectPool<T> pool)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            IsWarmedUp = false;
        }

        #endregion

        #region 同步预热 Synchronous Warmup

        public void Warmup(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "预热数量不能小于 0 / Count cannot be less than 0");

            if (count == 0)
            {
                IsWarmedUp = true;
                return;
            }

            // 直接调用池的预热方法
            // Directly call pool's warmup method
            _pool.Warmup(count);

            lock (_lock)
            {
                IsWarmedUp = true;
                _currentCount = count;
                _targetCount = count;
            }
        }

        #endregion

        #region 异步预热 Asynchronous Warmup

        public async Task WarmupAsync(int count, CancellationToken cancellationToken = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "预热数量不能小于 0 / Count cannot be less than 0");

            if (count == 0)
            {
                IsWarmedUp = true;
                return;
            }

            lock (_lock)
            {
                _targetCount = count;
                _currentCount = 0;
            }

            // 在后台线程执行预热
            // Execute warmup in background thread
            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // 获取并立即归还对象，触发创建
                        // Get and immediately return object to trigger creation
                        var obj = _pool.Get();
                        _pool.Return(obj);

                        lock (_lock)
                        {
                            _currentCount++;
                        }
                    }
                    catch (Exception)
                    {
                        // 忽略单个对象创建失败
                        // Ignore individual object creation failure
                    }
                }

                lock (_lock)
                {
                    IsWarmedUp = true;
                }
            }, cancellationToken);
        }

        #endregion

        #region 分帧预热 Frame-Distributed Warmup

        public void WarmupFrameDistributed(
            int count,
            int objectsPerFrame = 1,
            Action<float> onProgress = null,
            Action onComplete = null)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "预热数量不能小于 0 / Count cannot be less than 0");

            if (objectsPerFrame <= 0)
                throw new ArgumentOutOfRangeException(nameof(objectsPerFrame), "每帧对象数必须大于 0 / Objects per frame must be greater than 0");

            lock (_lock)
            {
                _isWarmingUp = true;
                _targetCount = count;
                _currentCount = 0;
                _objectsPerFrame = objectsPerFrame;
                _onProgress = onProgress;
                _onComplete = onComplete;
                IsWarmedUp = false;
            }
        }

        public void UpdateWarmup()
        {
            if (!_isWarmingUp) return;

            lock (_lock)
            {
                if (_currentCount >= _targetCount)
                {
                    // 预热完成
                    // Warmup completed
                    _isWarmingUp = false;
                    IsWarmedUp = true;
                    _onComplete?.Invoke();
                    return;
                }

                // 本帧创建对象
                // Create objects in this frame
                int objectsToCreate = Math.Min(_objectsPerFrame, _targetCount - _currentCount);

                for (int i = 0; i < objectsToCreate; i++)
                {
                    try
                    {
                        var obj = _pool.Get();
                        _pool.Return(obj);
                        _currentCount++;
                    }
                    catch (Exception)
                    {
                        // 忽略单个对象创建失败
                        // Ignore individual object creation failure
                    }
                }

                // 触发进度回调
                // Trigger progress callback
                _onProgress?.Invoke(Progress);
            }
        }

        public void CancelWarmup()
        {
            lock (_lock)
            {
                _isWarmingUp = false;
                _currentCount = 0;
                _targetCount = 0;
                _onProgress = null;
                _onComplete = null;
            }
        }

        #endregion

        #region 容量管理 Capacity Management

        public int Shrink(int targetCount)
        {
            if (targetCount < 0)
                throw new ArgumentOutOfRangeException(nameof(targetCount), "目标数量不能小于 0 / Target count cannot be less than 0");

            // 委托给池的 Shrink 方法
            // Delegate to pool's Shrink method
            if (_pool is IObjectPool poolWithShrink)
            {
                return poolWithShrink.Shrink(targetCount);
            }

            return 0;
        }

        #endregion
    }

    /// <summary>
    /// 非泛型对象池预热器
    /// Non-generic Object Pool Warmer
    /// </summary>
    /// <remarks>
    /// 用于不知道具体类型的场景
    /// Used for scenarios where the specific type is unknown
    /// </remarks>
    public class PoolWarmer : IPoolWarmer
    {
        #region 字段 Fields

        private readonly IObjectPool _pool;
        private readonly object _lock = new object();

        private bool _isWarmingUp;
        private int _targetCount;
        private int _currentCount;
        private int _objectsPerFrame;
        private Action<float> _onProgress;
        private Action _onComplete;

        #endregion

        #region 属性 Properties

        public bool IsWarmedUp { get; private set; }

        public float Progress
        {
            get
            {
                lock (_lock)
                {
                    if (_targetCount == 0) return 1.0f;
                    return (float)_currentCount / _targetCount;
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化预热器
        /// Initialize warmer
        /// </summary>
        public PoolWarmer(IObjectPool pool)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            IsWarmedUp = false;
        }

        #endregion

        #region 同步预热 Synchronous Warmup

        public void Warmup(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
            {
                IsWarmedUp = true;
                return;
            }

            _pool.Warmup(count);

            lock (_lock)
            {
                IsWarmedUp = true;
                _currentCount = count;
                _targetCount = count;
            }
        }

        #endregion

        #region 异步预热 Asynchronous Warmup

        public async Task WarmupAsync(int count, CancellationToken cancellationToken = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
            {
                IsWarmedUp = true;
                return;
            }

            lock (_lock)
            {
                _targetCount = count;
                _currentCount = 0;
            }

            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        var obj = _pool.Get();
                        _pool.Return(obj);

                        lock (_lock)
                        {
                            _currentCount++;
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }

                lock (_lock)
                {
                    IsWarmedUp = true;
                }
            }, cancellationToken);
        }

        #endregion

        #region 分帧预热 Frame-Distributed Warmup

        public void WarmupFrameDistributed(
            int count,
            int objectsPerFrame = 1,
            Action<float> onProgress = null,
            Action onComplete = null)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (objectsPerFrame <= 0)
                throw new ArgumentOutOfRangeException(nameof(objectsPerFrame));

            lock (_lock)
            {
                _isWarmingUp = true;
                _targetCount = count;
                _currentCount = 0;
                _objectsPerFrame = objectsPerFrame;
                _onProgress = onProgress;
                _onComplete = onComplete;
                IsWarmedUp = false;
            }
        }

        public void UpdateWarmup()
        {
            if (!_isWarmingUp) return;

            lock (_lock)
            {
                if (_currentCount >= _targetCount)
                {
                    _isWarmingUp = false;
                    IsWarmedUp = true;
                    _onComplete?.Invoke();
                    return;
                }

                int objectsToCreate = Math.Min(_objectsPerFrame, _targetCount - _currentCount);

                for (int i = 0; i < objectsToCreate; i++)
                {
                    try
                    {
                        var obj = _pool.Get();
                        _pool.Return(obj);
                        _currentCount++;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }

                _onProgress?.Invoke(Progress);
            }
        }

        public void CancelWarmup()
        {
            lock (_lock)
            {
                _isWarmingUp = false;
                _currentCount = 0;
                _targetCount = 0;
                _onProgress = null;
                _onComplete = null;
            }
        }

        #endregion

        #region 容量管理 Capacity Management

        public int Shrink(int targetCount)
        {
            if (targetCount < 0)
                throw new ArgumentOutOfRangeException(nameof(targetCount));

            // 委托给池的 Shrink 方法
            // Delegate to pool's Shrink method
            return _pool.Shrink(targetCount);
        }

        #endregion
    }
}
