// ==========================================================
// 文件名：IObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池核心接口（非泛型版本），提供类型无关的对象池基础操作
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池核心接口（非泛型版本）
    /// <para>提供类型无关的对象池基础操作</para>
    /// <para>Object pool core interface (non-generic version) providing type-agnostic basic operations</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>接口隔离原则（ISP）：提供最小化的核心操作</item>
    /// <item>单一职责原则（SRP）：仅负责对象的获取和归还</item>
    /// <item>依赖倒置原则（DIP）：面向接口编程，不依赖具体实现</item>
    /// </list>
    /// </remarks>
    public interface IObjectPool : IDisposable
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取池中对象的类型
        /// <para>Get the type of objects in the pool</para>
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// 获取池的当前状态
        /// <para>Get the current state of the pool</para>
        /// </summary>
        PoolState State { get; }

        /// <summary>
        /// 获取池中当前可用对象的数量
        /// <para>Get the count of currently available objects in the pool</para>
        /// </summary>
        int AvailableCount { get; }

        /// <summary>
        /// 获取池中当前活跃对象的数量（已被获取但未归还）
        /// <para>Get the count of currently active objects (acquired but not returned)</para>
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// 获取池的总容量（可用 + 活跃）
        /// <para>Get the total capacity of the pool (available + active)</para>
        /// </summary>
        int TotalCount { get; }

        #endregion

        #region 核心操作 / Core Operations

        /// <summary>
        /// 从池中获取一个对象（非泛型版本）
        /// <para>Get an object from the pool (non-generic version)</para>
        /// </summary>
        /// <returns>池化对象实例 / Pooled object instance</returns>
        /// <exception cref="PoolDisposedException">池已被销毁 / Pool has been disposed</exception>
        /// <exception cref="PoolCapacityExceededException">池容量已满且无法扩容 / Pool capacity exceeded and cannot expand</exception>
        object Get();

        /// <summary>
        /// 将对象归还到池中（非泛型版本）
        /// <para>Return an object to the pool (non-generic version)</para>
        /// </summary>
        /// <param name="obj">要归还的对象 / Object to return</param>
        /// <returns>如果成功归还返回 true，否则返回 false / True if successfully returned, otherwise false</returns>
        /// <exception cref="ArgumentNullException">obj 为 null / obj is null</exception>
        /// <exception cref="PoolDisposedException">池已被销毁 / Pool has been disposed</exception>
        bool Return(object obj);

        #endregion

        #region 管理操作 / Management Operations

        /// <summary>
        /// 清空池中所有可用对象
        /// <para>Clear all available objects in the pool</para>
        /// </summary>
        /// <remarks>
        /// 注意：此操作不会影响已获取但未归还的活跃对象
        /// <para>Note: This operation does not affect active objects that have been acquired but not returned</para>
        /// </remarks>
        void Clear();

        /// <summary>
        /// 预热池，预先创建指定数量的对象
        /// <para>Warm up the pool by pre-creating specified number of objects</para>
        /// </summary>
        /// <param name="count">要预创建的对象数量 / Number of objects to pre-create</param>
        /// <exception cref="ArgumentOutOfRangeException">count 小于 0 / count is less than 0</exception>
        void Warmup(int count);

        /// <summary>
        /// 收缩池，移除多余的空闲对象
        /// <para>Shrink the pool by removing excess idle objects</para>
        /// </summary>
        /// <param name="targetCount">目标保留的空闲对象数量 / Target count of idle objects to retain</param>
        /// <returns>实际移除的对象数量 / Actual number of objects removed</returns>
        int Shrink(int targetCount);

        #endregion
    }
}
