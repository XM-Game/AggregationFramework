// ==========================================================
// 文件名：NativePool.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：原生对象池，提供对象复用机制以减少内存分配
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// 对象池工厂接口
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface IPoolFactory<T> where T : unmanaged
    {
        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <returns>新创建的对象</returns>
        T Create();
    }

    /// <summary>
    /// 原生对象池
    /// 提供对象复用机制以减少内存分配和GC压力
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TFactory">工厂类型</typeparam>
    [GenerateTestsForBurstCompatibility]
    public struct NativePool<T, TFactory> : IDisposable
        where T : unmanaged
        where TFactory : struct, IPoolFactory<T>
    {
        #region 字段

        private NativeQueue<T> m_Pool;
        private TFactory m_Factory;
        private int m_MaxSize;

        #endregion

        #region 属性

        /// <summary>
        /// 获取池中可用对象数量
        /// </summary>
        public int AvailableCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Pool.IsCreated ? m_Pool.Count : 0;
        }

        /// <summary>
        /// 检查池是否已创建
        /// </summary>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Pool.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="maxSize">最大池大小（0表示无限制）</param>
        /// <param name="factory">对象工厂</param>
        /// <param name="allocator">分配器</param>
        public NativePool(int initialCapacity, int maxSize, TFactory factory, Allocator allocator)
        {
            m_Pool = new NativeQueue<T>(allocator);
            m_Factory = factory;
            m_MaxSize = maxSize;
        }

        /// <summary>
        /// 创建对象池（无大小限制）
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="factory">对象工厂</param>
        /// <param name="allocator">分配器</param>
        public NativePool(int initialCapacity, TFactory factory, Allocator allocator)
            : this(initialCapacity, 0, factory, allocator)
        {
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <returns>对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            if (m_Pool.TryDequeue(out var item))
            {
                return item;
            }
            return m_Factory.Create();
        }

        /// <summary>
        /// 将对象返回到池中
        /// </summary>
        /// <param name="item">要返回的对象</param>
        /// <returns>如果成功返回池中返回true，如果池已满返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Return(T item)
        {
            if (m_MaxSize > 0 && m_Pool.Count >= m_MaxSize)
                return false;

            m_Pool.Enqueue(item);
            return true;
        }

        /// <summary>
        /// 清空池
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            while (m_Pool.TryDequeue(out _)) { }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_Pool.IsCreated)
            {
                m_Pool.Dispose();
            }
        }

        #endregion
    }
}

