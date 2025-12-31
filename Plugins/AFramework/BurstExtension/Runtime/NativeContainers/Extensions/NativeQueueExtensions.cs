// ==========================================================
// 文件名：NativeQueueExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：NativeQueue扩展方法，提供便捷的队列操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// NativeQueue扩展方法
    /// 提供便捷的队列操作、查询和转换功能
    /// </summary>
    public static class NativeQueueExtensions
    {
        #region 查询操作

        /// <summary>
        /// 检查队列是否为空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <returns>如果队列为空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this NativeQueue<T> queue) where T : unmanaged
        {
            return !queue.IsCreated || queue.Count == 0;
        }

        /// <summary>
        /// 尝试查看队列头部元素（不移除）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="value">输出的头部元素</param>
        /// <returns>如果队列非空返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPeek<T>(this NativeQueue<T> queue, out T value) where T : unmanaged
        {
            value = default;
            if (IsEmpty(queue))
                return false;
            
            // NativeQueue 没有直接的 Peek 方法，需要先出队再入队
            // 这里使用 TryDequeue 然后重新入队
            if (queue.TryDequeue(out value))
            {
                queue.Enqueue(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 安全出队（带空值检查）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="value">输出的元素</param>
        /// <returns>如果成功出队返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDequeueSafe<T>(this NativeQueue<T> queue, out T value) where T : unmanaged
        {
            return queue.TryDequeue(out value);
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量入队
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="items">要入队的元素数组</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnqueueRange<T>(this NativeQueue<T> queue, NativeArray<T> items) where T : unmanaged
        {
            if (!items.IsCreated)
                return;
            for (int i = 0; i < items.Length; i++)
            {
                queue.Enqueue(items[i]);
            }
        }

        /// <summary>
        /// 批量出队
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="count">要出队的元素数量</param>
        /// <param name="allocator">分配器</param>
        /// <returns>出队的元素列表</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<T> DequeueRange<T>(
            this NativeQueue<T> queue, 
            int count, 
            Allocator allocator) where T : unmanaged
        {
            var result = new NativeList<T>(count, allocator);
            int actualCount = Math.Min(count, queue.Count);
            for (int i = 0; i < actualCount; i++)
            {
                if (queue.TryDequeue(out var value))
                {
                    result.Add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this NativeQueue<T> queue) where T : unmanaged
        {
            while (queue.TryDequeue(out _)) { }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为NativeList（保持队列顺序）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeList（队列会被清空）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<T> ToNativeList<T>(this NativeQueue<T> queue, Allocator allocator) where T : unmanaged
        {
            var list = new NativeList<T>(queue.Count, allocator);
            while (queue.TryDequeue(out var value))
            {
                list.Add(value);
            }
            return list;
        }

        /// <summary>
        /// 转换为NativeArray（保持队列顺序，队列会被清空）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeArray</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> ToNativeArray<T>(this NativeQueue<T> queue, Allocator allocator) where T : unmanaged
        {
            var list = ToNativeList(queue, allocator);
            var array = new NativeArray<T>(list.Length, allocator);
            NativeArray<T>.Copy(list.AsArray(), array);
            list.Dispose();
            return array;
        }

        #endregion
    }
}

