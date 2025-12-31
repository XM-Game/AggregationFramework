// ==========================================================
// 文件名：NativeStack.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：原生栈，提供后进先出（LIFO）的栈数据结构
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// 原生栈
    /// 提供后进先出（LIFO）的栈数据结构，基于NativeList实现
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [GenerateTestsForBurstCompatibility]
    public struct NativeStack<T> : IDisposable where T : unmanaged
    {
        #region 字段

        private NativeList<T> m_List;

        #endregion

        #region 属性

        /// <summary>
        /// 获取栈中元素数量
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_List.IsCreated ? m_List.Length : 0;
        }

        /// <summary>
        /// 检查栈是否为空
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !m_List.IsCreated || m_List.Length == 0;
        }

        /// <summary>
        /// 检查栈是否已创建
        /// </summary>
        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_List.IsCreated;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建栈
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="allocator">分配器</param>
        public NativeStack(int initialCapacity, Allocator allocator)
        {
            m_List = new NativeList<T>(initialCapacity, allocator);
        }

        /// <summary>
        /// 创建栈（使用默认容量）
        /// </summary>
        /// <param name="allocator">分配器</param>
        public NativeStack(Allocator allocator)
        {
            m_List = new NativeList<T>(allocator);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 入栈（添加到栈顶）
        /// </summary>
        /// <param name="item">要添加的元素</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            m_List.Add(item);
        }

        /// <summary>
        /// 出栈（移除并返回栈顶元素）
        /// </summary>
        /// <returns>栈顶元素</returns>
        /// <exception cref="InvalidOperationException">栈为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("栈为空，无法执行出栈操作");

            var item = m_List[m_List.Length - 1];
            m_List.RemoveAtSwapBack(m_List.Length - 1);
            return item;
        }

        /// <summary>
        /// 尝试出栈
        /// </summary>
        /// <param name="item">输出的栈顶元素</param>
        /// <returns>如果成功出栈返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop(out T item)
        {
            item = default;
            if (IsEmpty)
                return false;

            item = Pop();
            return true;
        }

        /// <summary>
        /// 查看栈顶元素（不移除）
        /// </summary>
        /// <returns>栈顶元素</returns>
        /// <exception cref="InvalidOperationException">栈为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("栈为空，无法查看栈顶元素");

            return m_List[m_List.Length - 1];
        }

        /// <summary>
        /// 尝试查看栈顶元素
        /// </summary>
        /// <param name="item">输出的栈顶元素</param>
        /// <returns>如果栈非空返回true，否则返回false</returns>
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
        /// 清空栈
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_List.Clear();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_List.IsCreated)
            {
                m_List.Dispose();
            }
        }

        #endregion
    }
}

