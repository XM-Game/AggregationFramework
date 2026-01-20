// ==========================================================
// 文件名：ScenePoolWarmer.cs
// 命名空间: AFramework.Pool.Warming
// 依赖: UnityEngine, System, AFramework.Pool
// 功能: Unity 场景对象池预热器
// ==========================================================

#if UNITY_2022_3_OR_NEWER || UNITY_2021_3_OR_NEWER || UNITY_2020_3_OR_NEWER

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Pool.Warming
{
    /// <summary>
    /// Unity 场景对象池预热器
    /// Unity Scene Pool Warmer
    /// </summary>
    /// <remarks>
    /// 在场景加载时自动预热对象池，支持分帧预热避免卡顿
    /// Automatically warms up object pools on scene load, supports frame-distributed warmup to avoid stuttering
    /// </remarks>
    public class ScenePoolWarmer : MonoBehaviour
    {
        #region 序列化字段 Serialized Fields

        [Header("预热配置 / Warmup Configuration")]
        [Tooltip("预热策略 / Warmup strategy")]
        [SerializeField] private WarmupStrategy _strategy = WarmupStrategy.FrameDistributed;

        [Tooltip("预热对象数量 / Number of objects to warmup")]
        [SerializeField] private int _warmupCount = 10;

        [Tooltip("每帧创建对象数 / Objects per frame")]
        [SerializeField] private int _objectsPerFrame = 1;

        [Tooltip("是否在 Start 时自动预热 / Auto warmup on Start")]
        [SerializeField] private bool _autoWarmup = true;

        [Header("调试信息 / Debug Info")]
        [Tooltip("显示预热进度 / Show warmup progress")]
        [SerializeField] private bool _showProgress = true;

        #endregion

        #region 字段 Fields

        private readonly List<IPoolWarmer> _warmers = new List<IPoolWarmer>();
        private Coroutine _warmupCoroutine;
        private bool _isWarmedUp;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取是否已预热完成
        /// Whether warmup is completed
        /// </summary>
        public bool IsWarmedUp => _isWarmedUp;

        /// <summary>
        /// 获取预热进度（0.0 - 1.0）
        /// Warmup progress (0.0 - 1.0)
        /// </summary>
        public float Progress
        {
            get
            {
                if (_warmers.Count == 0) return 1.0f;

                float totalProgress = 0f;
                foreach (var warmer in _warmers)
                {
                    totalProgress += warmer.Progress;
                }
                return totalProgress / _warmers.Count;
            }
        }

        #endregion

        #region Unity 生命周期 Unity Lifecycle

        private void Start()
        {
            if (_autoWarmup)
            {
                StartWarmup();
            }
        }

        private void OnDestroy()
        {
            StopWarmup();
        }

        #endregion

        #region 公共方法 Public Methods

        /// <summary>
        /// 注册对象池预热器
        /// Register pool warmer
        /// </summary>
        public void RegisterWarmer(IPoolWarmer warmer)
        {
            if (warmer == null)
                throw new ArgumentNullException(nameof(warmer));

            if (!_warmers.Contains(warmer))
            {
                _warmers.Add(warmer);
            }
        }

        /// <summary>
        /// 注销对象池预热器
        /// Unregister pool warmer
        /// </summary>
        public void UnregisterWarmer(IPoolWarmer warmer)
        {
            _warmers.Remove(warmer);
        }

        /// <summary>
        /// 开始预热
        /// Start warmup
        /// </summary>
        public void StartWarmup()
        {
            if (_isWarmedUp || _warmupCoroutine != null)
                return;

            switch (_strategy)
            {
                case WarmupStrategy.Immediate:
                    WarmupImmediate();
                    break;

                case WarmupStrategy.FrameDistributed:
                    _warmupCoroutine = StartCoroutine(WarmupFrameDistributedCoroutine());
                    break;

                case WarmupStrategy.Async:
                    _warmupCoroutine = StartCoroutine(WarmupAsyncCoroutine());
                    break;

                default:
                    // 默认延迟预热不需要立即执行
                    // Default lazy warmup doesn't need immediate execution
                    _isWarmedUp = true;
                    break;
            }
        }

        /// <summary>
        /// 停止预热
        /// Stop warmup
        /// </summary>
        public void StopWarmup()
        {
            if (_warmupCoroutine != null)
            {
                StopCoroutine(_warmupCoroutine);
                _warmupCoroutine = null;
            }

            foreach (var warmer in _warmers)
            {
                warmer.CancelWarmup();
            }
        }

        #endregion

        #region 预热实现 Warmup Implementation

        /// <summary>
        /// 立即预热
        /// Immediate warmup
        /// </summary>
        private void WarmupImmediate()
        {
            foreach (var warmer in _warmers)
            {
                warmer.Warmup(_warmupCount);
            }

            _isWarmedUp = true;

            if (_showProgress)
            {
                Debug.Log($"[ScenePoolWarmer] 立即预热完成 Immediate warmup completed: {_warmers.Count} pools");
            }
        }

        /// <summary>
        /// 分帧预热协程
        /// Frame-distributed warmup coroutine
        /// </summary>
        private IEnumerator WarmupFrameDistributedCoroutine()
        {
            if (_showProgress)
            {
                Debug.Log($"[ScenePoolWarmer] 开始分帧预热 Starting frame-distributed warmup: {_warmers.Count} pools");
            }

            // 启动所有预热器
            // Start all warmers
            foreach (var warmer in _warmers)
            {
                warmer.WarmupFrameDistributed(_warmupCount, _objectsPerFrame);
            }

            // 等待所有预热器完成
            // Wait for all warmers to complete
            while (true)
            {
                bool allCompleted = true;

                foreach (var warmer in _warmers)
                {
                    warmer.UpdateWarmup();

                    if (!warmer.IsWarmedUp)
                    {
                        allCompleted = false;
                    }
                }

                if (_showProgress)
                {
                    Debug.Log($"[ScenePoolWarmer] 预热进度 Warmup progress: {Progress:P2}");
                }

                if (allCompleted)
                    break;

                yield return null;
            }

            _isWarmedUp = true;
            _warmupCoroutine = null;

            if (_showProgress)
            {
                Debug.Log($"[ScenePoolWarmer] 分帧预热完成 Frame-distributed warmup completed");
            }
        }

        /// <summary>
        /// 异步预热协程
        /// Async warmup coroutine
        /// </summary>
        private IEnumerator WarmupAsyncCoroutine()
        {
            if (_showProgress)
            {
                Debug.Log($"[ScenePoolWarmer] 开始异步预热 Starting async warmup: {_warmers.Count} pools");
            }

            // 启动所有异步预热任务
            // Start all async warmup tasks
            var tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();

            foreach (var warmer in _warmers)
            {
                var task = warmer.WarmupAsync(_warmupCount);
                tasks.Add(task);
            }

            // 等待所有任务完成
            // Wait for all tasks to complete
            while (tasks.Count > 0)
            {
                for (int i = tasks.Count - 1; i >= 0; i--)
                {
                    if (tasks[i].IsCompleted)
                    {
                        tasks.RemoveAt(i);
                    }
                }

                if (_showProgress && tasks.Count > 0)
                {
                    Debug.Log($"[ScenePoolWarmer] 异步预热进度 Async warmup progress: {Progress:P2}");
                }

                yield return null;
            }

            _isWarmedUp = true;
            _warmupCoroutine = null;

            if (_showProgress)
            {
                Debug.Log($"[ScenePoolWarmer] 异步预热完成 Async warmup completed");
            }
        }

        #endregion

        #region 编辑器辅助 Editor Helpers

#if UNITY_EDITOR
        [ContextMenu("立即预热 / Warmup Immediately")]
        private void EditorWarmupImmediate()
        {
            _strategy = WarmupStrategy.Immediate;
            StartWarmup();
        }

        [ContextMenu("分帧预热 / Warmup Frame-Distributed")]
        private void EditorWarmupFrameDistributed()
        {
            _strategy = WarmupStrategy.FrameDistributed;
            StartWarmup();
        }

        [ContextMenu("停止预热 / Stop Warmup")]
        private void EditorStopWarmup()
        {
            StopWarmup();
        }
#endif

        #endregion
    }
}

#endif
