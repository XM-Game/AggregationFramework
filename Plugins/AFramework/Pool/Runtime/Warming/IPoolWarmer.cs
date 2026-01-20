// ==========================================================
// 文件名：IPoolWarmer.cs
// 命名空间: AFramework.Pool
// 依赖: System, System.Threading
// 功能: 对象池预热接口
// ==========================================================

using System;
using System.Threading;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池预热接口
    /// Pool Warmer Interface
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 单一职责原则：仅负责池的预热操作
    /// - 接口隔离原则：提供最小化的预热接口
    /// - 开闭原则：支持多种预热策略扩展
    /// 
    /// 预热目的：
    /// - 避免首次使用时的创建延迟
    /// - 平滑分散对象创建的性能开销
    /// - 提升用户体验，减少卡顿
    /// 
    /// 使用场景：
    /// - 游戏启动时预热常用对象池
    /// - 场景切换前预热场景专用对象
    /// - 关卡加载时预热敌人、子弹等对象
    /// </remarks>
    public interface IPoolWarmer
    {
        #region 属性 Properties

        /// <summary>
        /// 获取预热是否完成
        /// Whether warmup is completed
        /// </summary>
        bool IsWarmedUp { get; }

        /// <summary>
        /// 获取预热进度（0.0 - 1.0）
        /// Warmup progress (0.0 - 1.0)
        /// </summary>
        float Progress { get; }

        #endregion

        #region 同步预热 Synchronous Warmup

        /// <summary>
        /// 同步预热池
        /// Warmup pool synchronously
        /// </summary>
        /// <param name="count">预热对象数量 / Number of objects to warmup</param>
        /// <exception cref="ArgumentOutOfRangeException">count 小于 0 / count is less than 0</exception>
        void Warmup(int count);

        #endregion

        #region 异步预热 Asynchronous Warmup

        /// <summary>
        /// 异步预热池
        /// Warmup pool asynchronously
        /// </summary>
        /// <param name="count">预热对象数量 / Number of objects to warmup</param>
        /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
        /// <returns>异步任务 / Async task</returns>
        System.Threading.Tasks.Task WarmupAsync(int count, CancellationToken cancellationToken = default);

        #endregion

        #region 分帧预热 Frame-Distributed Warmup

        /// <summary>
        /// 开始分帧预热
        /// Start frame-distributed warmup
        /// </summary>
        /// <param name="count">预热对象数量 / Number of objects to warmup</param>
        /// <param name="objectsPerFrame">每帧创建对象数（默认 1）/ Objects per frame (default 1)</param>
        /// <param name="onProgress">进度回调 / Progress callback</param>
        /// <param name="onComplete">完成回调 / Completion callback</param>
        void WarmupFrameDistributed(
            int count,
            int objectsPerFrame = 1,
            Action<float> onProgress = null,
            Action onComplete = null);

        /// <summary>
        /// 更新分帧预热（需要在主循环中调用）
        /// Update frame-distributed warmup (needs to be called in main loop)
        /// </summary>
        void UpdateWarmup();

        /// <summary>
        /// 取消分帧预热
        /// Cancel frame-distributed warmup
        /// </summary>
        void CancelWarmup();

        #endregion

        #region 容量管理 Capacity Management

        /// <summary>
        /// 收缩池，移除多余的空闲对象
        /// Shrink the pool by removing excess idle objects
        /// </summary>
        /// <param name="targetCount">目标保留的空闲对象数量 / Target count of idle objects to retain</param>
        /// <returns>实际移除的对象数量 / Actual number of objects removed</returns>
        int Shrink(int targetCount);

        #endregion
    }
}
