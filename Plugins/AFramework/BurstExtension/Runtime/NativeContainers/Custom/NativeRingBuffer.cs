// ==========================================================
// 文件名：NativeRingBuffer.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：原生环形缓冲区，提供固定大小的循环缓冲区功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AFramework.Burst
{
    /// <summary>
    /// 原生环形缓冲区
    /// 提供固定大小的循环缓冲区，支持高效的入队和出队操作
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [GenerateTestsForBurstCompatibility]
    public struct NativeRingBuffer<T> : IDisposable where T : unmanaged
    {
        #region 字段

        private NativeArray<T> m_Buffer;
        private int m_Head;
        private int m_Tail;
        private int m_Count;
        private int m_Capacity;
        private Allocator m_Allocator;

        #endregion

        #region 属性

        /// <summary>
        /// 获取缓冲区容量
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Capacity;
        }

        /// <summary>
        /// 获取当前元素数量
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Count;
        }

        /// <summary>
        /// 检查缓冲区是否为空
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Count == 0;
        }

        /// <summary>
        /// 检查缓冲区是否已满
        /// </summary>
        public bool IsFull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Count >= m_Capacity;
        }

        /// <summary>
        /// 检查缓冲区是否已创建
        /// </summary>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Buffer.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建环形缓冲区
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="allocator">分配器</param>
        public NativeRingBuffer(int capacity, Allocator allocator)
        {
            if (capacity <= 0)
                throw new ArgumentException("容量必须大于0", nameof(capacity));

            m_Capacity = capacity;
            m_Allocator = allocator;
            m_Buffer = new NativeArray<T>(capacity, allocator);
            m_Head = 0;
            m_Tail = 0;
            m_Count = 0;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 入队（添加到尾部）
        /// </summary>
        /// <param name="item">要添加的元素</param>
        /// <returns>如果成功入队返回true，如果缓冲区已满返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Enqueue(T item)
        {
            if (IsFull)
                return false;

            m_Buffer[m_Tail] = item;
            m_Tail = (m_Tail + 1) % m_Capacity;
            m_Count++;
            return true;
        }

        /// <summary>
        /// 尝试出队（从头部移除）
        /// </summary>
        /// <param name="item">输出的元素</param>
        /// <returns>如果成功出队返回true，如果缓冲区为空返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out T item)
        {
            item = default;
            if (IsEmpty)
                return false;

            item = m_Buffer[m_Head];
            m_Head = (m_Head + 1) % m_Capacity;
            m_Count--;
            return true;
        }

        /// <summary>
        /// 查看头部元素（不移除）
        /// </summary>
        /// <param name="item">输出的元素</param>
        /// <returns>如果缓冲区非空返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(out T item)
        {
            item = default;
            if (IsEmpty)
                return false;

            item = m_Buffer[m_Head];
            return true;
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_Head = 0;
            m_Tail = 0;
            m_Count = 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_Buffer.IsCreated)
            {
                m_Buffer.Dispose();
            }
        }

        #endregion
    }
}

