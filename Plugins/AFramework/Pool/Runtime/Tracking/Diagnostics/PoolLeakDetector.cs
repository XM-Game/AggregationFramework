// ==========================================================
// 文件名：PoolLeakDetector.cs
// 命名空间: AFramework.Pool.Tracking
// 依赖: System, System.Collections.Generic
// 功能: 对象池泄漏检测器
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.Pool.Tracking
{
    /// <summary>
    /// 对象池泄漏检测器
    /// Pool Leak Detector
    /// </summary>
    /// <remarks>
    /// 追踪活跃对象，检测未归还的对象（潜在泄漏）
    /// Tracks active objects and detects unreturned objects (potential leaks)
    /// </remarks>
    public class PoolLeakDetector
    {
        #region 字段 Fields

        private readonly Dictionary<int, LeakTrackingInfo> _activeObjects;
        private readonly object _lock = new object();
        private TimeSpan _leakTimeout;
        private bool _captureStackTrace;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 获取或设置泄漏超时时间
        /// Get or set leak timeout
        /// </summary>
        public TimeSpan LeakTimeout
        {
            get => _leakTimeout;
            set => _leakTimeout = value;
        }

        /// <summary>
        /// 获取或设置是否捕获堆栈跟踪
        /// Get or set whether to capture stack trace
        /// </summary>
        /// <remarks>
        /// 捕获堆栈跟踪会增加性能开销，建议仅在调试时启用
        /// Capturing stack trace increases performance overhead, recommend enabling only during debugging
        /// </remarks>
        public bool CaptureStackTrace
        {
            get => _captureStackTrace;
            set => _captureStackTrace = value;
        }

        /// <summary>
        /// 获取当前活跃对象数量
        /// Get current active object count
        /// </summary>
        public int ActiveObjectCount
        {
            get
            {
                lock (_lock)
                {
                    return _activeObjects.Count;
                }
            }
        }

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 初始化泄漏检测器
        /// Initialize leak detector
        /// </summary>
        /// <param name="leakTimeout">泄漏超时时间（默认 5 分钟）/ Leak timeout (default 5 minutes)</param>
        /// <param name="captureStackTrace">是否捕获堆栈跟踪 / Whether to capture stack trace</param>
        public PoolLeakDetector(TimeSpan? leakTimeout = null, bool captureStackTrace = false)
        {
            _activeObjects = new Dictionary<int, LeakTrackingInfo>();
            _leakTimeout = leakTimeout ?? TimeSpan.FromMinutes(5);
            _captureStackTrace = captureStackTrace;
        }

        #endregion

        #region 追踪方法 Tracking Methods

        /// <summary>
        /// 记录对象获取
        /// Record object get
        /// </summary>
        public void TrackGet(object obj)
        {
            if (obj == null) return;

            var hashCode = obj.GetHashCode();
            var stackTrace = _captureStackTrace ? Environment.StackTrace : null;

            lock (_lock)
            {
                _activeObjects[hashCode] = new LeakTrackingInfo(
                    new WeakReference(obj),
                    DateTime.UtcNow,
                    stackTrace
                );
            }
        }

        /// <summary>
        /// 记录对象归还
        /// Record object return
        /// </summary>
        public void TrackReturn(object obj)
        {
            if (obj == null) return;

            var hashCode = obj.GetHashCode();

            lock (_lock)
            {
                _activeObjects.Remove(hashCode);
            }
        }

        #endregion

        #region 检测方法 Detection Methods

        /// <summary>
        /// 检测潜在泄漏
        /// Detect potential leaks
        /// </summary>
        public IReadOnlyList<LeakInfo> DetectLeaks()
        {
            var leaks = new List<LeakInfo>();
            var now = DateTime.UtcNow;

            lock (_lock)
            {
                // 清理已被 GC 回收的对象
                // Clean up objects that have been GC collected
                var deadKeys = _activeObjects
                    .Where(kvp => !kvp.Value.ObjectReference.IsAlive)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in deadKeys)
                {
                    _activeObjects.Remove(key);
                }

                // 检测超时对象
                // Detect timeout objects
                foreach (var kvp in _activeObjects)
                {
                    var info = kvp.Value;
                    var duration = now - info.GetTime;

                    if (duration > _leakTimeout && info.ObjectReference.IsAlive)
                    {
                        leaks.Add(new LeakInfo(
                            info.ObjectReference.Target,
                            info.GetTime,
                            duration,
                            info.StackTrace
                        ));
                    }
                }
            }

            return leaks;
        }

        /// <summary>
        /// 获取所有活跃对象信息
        /// Get all active object information
        /// </summary>
        public IReadOnlyList<ActiveObjectInfo> GetActiveObjects()
        {
            var activeObjects = new List<ActiveObjectInfo>();
            var now = DateTime.UtcNow;

            lock (_lock)
            {
                foreach (var kvp in _activeObjects)
                {
                    var info = kvp.Value;
                    if (info.ObjectReference.IsAlive)
                    {
                        var duration = now - info.GetTime;
                        var isPotentialLeak = duration > _leakTimeout;

                        activeObjects.Add(new ActiveObjectInfo(
                            info.ObjectReference.Target,
                            info.GetTime,
                            info.StackTrace
                        ));
                    }
                }
            }

            return activeObjects;
        }

        #endregion

        #region 管理方法 Management Methods

        /// <summary>
        /// 清空所有追踪数据
        /// Clear all tracking data
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _activeObjects.Clear();
            }
        }

        #endregion

        #region 内部类型 Internal Types

        /// <summary>
        /// 泄漏追踪信息
        /// Leak Tracking Information
        /// </summary>
        private class LeakTrackingInfo
        {
            public readonly WeakReference ObjectReference;
            public readonly DateTime GetTime;
            public readonly string StackTrace;

            public LeakTrackingInfo(WeakReference objectReference, DateTime getTime, string stackTrace)
            {
                ObjectReference = objectReference;
                GetTime = getTime;
                StackTrace = stackTrace;
            }
        }

        #endregion
    }

    /// <summary>
    /// 泄漏信息
    /// Leak Information
    /// </summary>
    public readonly struct LeakInfo
    {
        /// <summary>
        /// 泄漏对象
        /// Leaked object
        /// </summary>
        public readonly object Object;

        /// <summary>
        /// 对象获取时间
        /// Object get time
        /// </summary>
        public readonly DateTime GetTime;

        /// <summary>
        /// 泄漏持续时间
        /// Leak duration
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// 获取时的堆栈跟踪
        /// Stack trace at get time
        /// </summary>
        public readonly string StackTrace;

        /// <summary>
        /// 对象类型名称
        /// Object type name
        /// </summary>
        public string TypeName => Object?.GetType().Name ?? "Unknown";

        /// <summary>
        /// 初始化泄漏信息
        /// Initialize leak information
        /// </summary>
        public LeakInfo(object obj, DateTime getTime, TimeSpan duration, string stackTrace)
        {
            Object = obj;
            GetTime = getTime;
            Duration = duration;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// 转换为字符串表示
        /// Convert to string representation
        /// </summary>
        public override string ToString()
        {
            return $"[Leak] {TypeName} - Duration: {Duration.TotalMinutes:F2} min, GetTime: {GetTime:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
