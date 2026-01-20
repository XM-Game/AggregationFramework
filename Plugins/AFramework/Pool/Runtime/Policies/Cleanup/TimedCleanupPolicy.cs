// ==========================================================
// 文件名：TimedCleanupPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Collections.Generic
// 功能: 定时清理策略，定期清理空闲对象
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// 定时清理策略
    /// Timed Cleanup Policy
    /// 
    /// <para>定期清理空闲对象的策略，基于时间戳</para>
    /// <para>Policy that periodically cleans up idle objects based on timestamps</para>
    /// </summary>
    /// <typeparam name="T">对象类型 / Object type</typeparam>
    /// <remarks>
    /// 使用场景：
    /// - 需要定期清理空闲对象
    /// - 防止对象长时间占用内存
    /// - 自动内存管理
    /// 
    /// 特性：
    /// - 记录对象归还时间
    /// - 超过阈值时标记为可清理
    /// - 支持自定义清理间隔
    /// 
    /// 注意：
    /// - 需要外部定期调用清理检查
    /// - 适合配合定时器使用
    /// - 不适合频繁归还的场景
    /// </remarks>
    public class TimedCleanupPolicy<T> : PoolPolicyBase<T>, IPoolCleanupPolicy<T>
    {
        #region Fields

        /// <summary>
        /// 对象时间戳字典
        /// Object timestamp dictionary
        /// </summary>
        private readonly Dictionary<T, DateTime> _timestamps;

        /// <summary>
        /// 清理阈值（秒）
        /// Cleanup threshold (seconds)
        /// </summary>
        private readonly double _cleanupThresholdSeconds;

        /// <summary>
        /// 是否在归还时清理
        /// Whether to cleanup on return
        /// </summary>
        private readonly bool _cleanupOnReturn;

        #endregion

        #region Properties

        /// <summary>
        /// 清理阈值（秒）
        /// Cleanup threshold (seconds)
        /// </summary>
        public double CleanupThresholdSeconds => _cleanupThresholdSeconds;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="cleanupThresholdSeconds">清理阈值（秒，默认 60）/ Cleanup threshold (seconds, default 60)</param>
        /// <param name="cleanupOnReturn">是否在归还时清理（默认 false）/ Whether to cleanup on return (default false)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public TimedCleanupPolicy(double cleanupThresholdSeconds = 60.0, bool cleanupOnReturn = false, string name = null)
            : base(name ?? "TimedCleanupPolicy")
        {
            if (cleanupThresholdSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cleanupThresholdSeconds), "Cleanup threshold must be greater than 0.");
            }

            _cleanupThresholdSeconds = cleanupThresholdSeconds;
            _cleanupOnReturn = cleanupOnReturn;
            _timestamps = new Dictionary<T, DateTime>();
        }

        #endregion

        #region IPoolCleanupPolicy Implementation

        /// <inheritdoc />
        public void OnReturn(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 记录归还时间
            // Record return time
            _timestamps[obj] = DateTime.UtcNow;

            // 如果启用归还时清理，检查是否需要清理
            // If cleanup on return is enabled, check if cleanup is needed
            if (_cleanupOnReturn)
            {
                PerformCleanupCheck(obj);
            }
        }

        /// <inheritdoc />
        public void OnDestroy(T obj)
        {
            ThrowIfDisposed();
            ThrowIfInvalidObject(obj);

            // 从时间戳字典中移除
            // Remove from timestamp dictionary
            _timestamps.Remove(obj);

            // 清理对象
            // Cleanup object
            CleanupObject(obj);
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证清理阈值
            // Validate cleanup threshold
            return _cleanupThresholdSeconds > 0;
        }

        #endregion

        #region Cleanup Methods

        /// <summary>
        /// 执行清理检查
        /// Perform cleanup check
        /// </summary>
        /// <param name="obj">要检查的对象 / Object to check</param>
        private void PerformCleanupCheck(T obj)
        {
            if (_timestamps.TryGetValue(obj, out DateTime timestamp))
            {
                TimeSpan elapsed = DateTime.UtcNow - timestamp;
                if (elapsed.TotalSeconds >= _cleanupThresholdSeconds)
                {
                    // 超过阈值，执行清理
                    // Exceeded threshold, perform cleanup
                    CleanupObject(obj);
                    _timestamps.Remove(obj);
                }
            }
        }

        /// <summary>
        /// 清理对象
        /// Cleanup object
        /// </summary>
        /// <param name="obj">要清理的对象 / Object to cleanup</param>
        protected virtual void CleanupObject(T obj)
        {
            // 子类可重写此方法执行自定义清理逻辑
            // Subclasses can override this method to perform custom cleanup logic

            // 如果对象实现了 IDisposable，调用 Dispose
            // If object implements IDisposable, call Dispose
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 检查所有对象并清理过期对象
        /// Check all objects and cleanup expired ones
        /// </summary>
        /// <returns>清理的对象列表 / List of cleaned up objects</returns>
        public List<T> CheckAndCleanupExpired()
        {
            ThrowIfDisposed();

            List<T> expiredObjects = new List<T>();
            DateTime now = DateTime.UtcNow;

            foreach (var kvp in _timestamps)
            {
                TimeSpan elapsed = now - kvp.Value;
                if (elapsed.TotalSeconds >= _cleanupThresholdSeconds)
                {
                    expiredObjects.Add(kvp.Key);
                }
            }

            // 清理过期对象
            // Cleanup expired objects
            foreach (var obj in expiredObjects)
            {
                CleanupObject(obj);
                _timestamps.Remove(obj);
            }

            return expiredObjects;
        }

        /// <summary>
        /// 获取对象的空闲时间（秒）
        /// Get object idle time (seconds)
        /// </summary>
        /// <param name="obj">对象 / Object</param>
        /// <returns>空闲时间（秒），如果对象不存在返回 -1 / Idle time (seconds), returns -1 if object doesn't exist</returns>
        public double GetIdleTime(T obj)
        {
            ThrowIfDisposed();

            if (_timestamps.TryGetValue(obj, out DateTime timestamp))
            {
                return (DateTime.UtcNow - timestamp).TotalSeconds;
            }

            return -1;
        }

        #endregion

        #region Dispose Override

        /// <inheritdoc />
        protected override void OnDispose()
        {
            _timestamps.Clear();
            base.OnDispose();
        }

        #endregion
    }
}
