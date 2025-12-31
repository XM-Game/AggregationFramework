// ==========================================================
// 文件名：NativePriorityQueue.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：原生优先队列，提供基于堆的优先队列实现
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// 优先队列比较器接口
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public interface IPriorityComparer<T> where T : unmanaged
    {
        /// <summary>
        /// 比较两个元素的优先级
        /// </summary>
        /// <param name="a">第一个元素</param>
        /// <param name="b">第二个元素</param>
        /// <returns>如果a的优先级高于b返回正数，相等返回0，否则返回负数</returns>
        int Compare(T a, T b);
    }

    /// <summary>
    /// 原生优先队列
    /// 提供基于最小堆的优先队列实现
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <typeparam name="TComparer">比较器类型</typeparam>
    [GenerateTestsForBurstCompatibility]
    public struct NativePriorityQueue<T, TComparer> : IDisposable
        where T : unmanaged
        where TComparer : struct, IPriorityComparer<T>
    {
        #region 字段

        private NativeList<T> m_Heap;
        private TComparer m_Comparer;

        #endregion

        #region 属性

        /// <summary>
        /// 获取队列中元素数量
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Heap.IsCreated ? m_Heap.Length : 0;
        }

        /// <summary>
        /// 检查队列是否为空
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !m_Heap.IsCreated || m_Heap.Length == 0;
        }

        /// <summary>
        /// 检查队列是否已创建
        /// </summary>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Heap.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建优先队列
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="comparer">比较器</param>
        /// <param name="allocator">分配器</param>
        public NativePriorityQueue(int initialCapacity, TComparer comparer, Allocator allocator)
        {
            m_Heap = new NativeList<T>(initialCapacity, allocator);
            m_Comparer = comparer;
        }

        /// <summary>
        /// 创建优先队列（使用默认容量）
        /// </summary>
        /// <param name="comparer">比较器</param>
        /// <param name="allocator">分配器</param>
        public NativePriorityQueue(TComparer comparer, Allocator allocator)
        {
            m_Heap = new NativeList<T>(allocator);
            m_Comparer = comparer;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item">要添加的元素</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            m_Heap.Add(item);
            HeapifyUp(m_Heap.Length - 1);
        }

        /// <summary>
        /// 出队（移除并返回优先级最高的元素）
        /// </summary>
        /// <returns>优先级最高的元素</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法执行出队操作");

            var top = m_Heap[0];
            m_Heap[0] = m_Heap[m_Heap.Length - 1];
            m_Heap.RemoveAtSwapBack(m_Heap.Length - 1);
            
            if (m_Heap.Length > 0)
            {
                HeapifyDown(0);
            }

            return top;
        }

        /// <summary>
        /// 尝试出队
        /// </summary>
        /// <param name="item">输出的优先级最高的元素</param>
        /// <returns>如果成功出队返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out T item)
        {
            item = default;
            if (IsEmpty)
                return false;

            item = Dequeue();
            return true;
        }

        /// <summary>
        /// 查看优先级最高的元素（不移除）
        /// </summary>
        /// <returns>优先级最高的元素</returns>
        /// <exception cref="InvalidOperationException">队列为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("优先队列为空，无法查看顶部元素");

            return m_Heap[0];
        }

        /// <summary>
        /// 尝试查看优先级最高的元素
        /// </summary>
        /// <param name="item">输出的优先级最高的元素</param>
        /// <returns>如果队列非空返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeek(out T item)
        {
            item = default;
            if (IsEmpty)
                return false;

            item = Peek();
            return true;
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_Heap.Clear();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_Heap.IsCreated)
            {
                m_Heap.Dispose();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 向上堆化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (m_Comparer.Compare(m_Heap[index], m_Heap[parent]) >= 0)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        /// <summary>
        /// 向下堆化
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int smallest = index;
                int left = 2 * index + 1;
                int right = 2 * index + 2;

                if (left < m_Heap.Length && m_Comparer.Compare(m_Heap[left], m_Heap[smallest]) < 0)
                    smallest = left;

                if (right < m_Heap.Length && m_Comparer.Compare(m_Heap[right], m_Heap[smallest]) < 0)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        /// <summary>
        /// 交换两个元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i, int j)
        {
            var temp = m_Heap[i];
            m_Heap[i] = m_Heap[j];
            m_Heap[j] = temp;
        }

        #endregion
    }
}

