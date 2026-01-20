// ==========================================================
// 文件名：IPoolLifecyclePolicy.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 对象池生命周期策略接口，合并创建和清理策略
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池生命周期策略接口
    /// Object Pool Lifecycle Policy Interface
    /// 
    /// <para>合并创建和清理策略，简化策略配置</para>
    /// <para>Merges creation and cleanup policies to simplify strategy configuration</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 改进点：
    /// - 将创建策略和清理策略合并为一个接口
    /// - 减少策略对象数量，降低配置复杂度
    /// - 保持功能完整性，不影响扩展性
    /// 
    /// 设计理念：
    /// - 创建和清理通常是成对出现的
    /// - 合并后更符合对象生命周期的概念
    /// - 简化了策略的配置和管理
    /// </remarks>
    public interface IPoolLifecyclePolicy<T> : IPoolPolicy
    {
        #region 创建操作

        /// <summary>
        /// 创建新对象
        /// Create new object
        /// </summary>
        /// <returns>创建的对象 / Created object</returns>
        T Create();

        #endregion

        #region 清理操作

        /// <summary>
        /// 对象归还时的清理操作
        /// Cleanup operation when object is returned
        /// </summary>
        /// <param name="obj">要清理的对象 / Object to clean</param>
        void OnReturn(T obj);

        /// <summary>
        /// 对象销毁时的清理操作
        /// Cleanup operation when object is destroyed
        /// </summary>
        /// <param name="obj">要销毁的对象 / Object to destroy</param>
        void OnDestroy(T obj);

        #endregion

        #region 验证

        /// <summary>
        /// 验证策略是否可用
        /// Validate if the policy is available
        /// </summary>
        /// <returns>是否可用 / Whether available</returns>
        bool Validate();

        #endregion
    }
}
