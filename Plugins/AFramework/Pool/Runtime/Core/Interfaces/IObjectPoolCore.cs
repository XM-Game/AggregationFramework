// ==========================================================
// 文件名：IObjectPoolCore.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 对象池核心接口，仅包含最基本的操作
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池核心接口（精简版）
    /// Object Pool Core Interface (Simplified)
    /// 
    /// <para>仅包含最核心的池操作，降低实现复杂度</para>
    /// <para>Contains only the most core pool operations, reducing implementation complexity</para>
    /// </summary>
    /// <typeparam name="T">池化对象类型 / Type of pooled objects</typeparam>
    /// <remarks>
    /// 改进点：
    /// - 接口最小化，仅保留核心方法
    /// - 降低实现负担
    /// - 便捷方法移至扩展方法
    /// 
    /// 核心方法：
    /// - Get(): 获取对象
    /// - Return(): 归还对象
    /// - Clear(): 清空池
    /// - Dispose(): 销毁池
    /// </remarks>
    public interface IObjectPoolCore<T> : IDisposable
    {
        #region 核心操作

        /// <summary>
        /// 从池中获取对象
        /// Get object from pool
        /// </summary>
        /// <returns>池化对象 / Pooled object</returns>
        T Get();

        /// <summary>
        /// 将对象归还到池中
        /// Return object to pool
        /// </summary>
        /// <param name="obj">要归还的对象 / Object to return</param>
        /// <returns>是否成功归还 / Whether successfully returned</returns>
        bool Return(T obj);

        /// <summary>
        /// 清空对象池
        /// Clear the object pool
        /// </summary>
        void Clear();

        #endregion

        #region 基本属性

        /// <summary>
        /// 池名称
        /// Pool name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 池状态
        /// Pool state
        /// </summary>
        PoolState State { get; }

        /// <summary>
        /// 可用对象数量
        /// Available object count
        /// </summary>
        int AvailableCount { get; }

        /// <summary>
        /// 活跃对象数量
        /// Active object count
        /// </summary>
        int ActiveCount { get; }

        #endregion
    }
}
