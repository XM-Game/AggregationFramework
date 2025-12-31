// ==========================================================
// 文件名：QueueExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Queue 扩展方法
    /// <para>提供队列的常用操作扩展，包括批量操作、安全访问等功能</para>
    /// </summary>
    public static class QueueExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查队列是否为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this Queue<T> queue)
        {
            return queue == null || queue.Count == 0;
        }

        /// <summary>
        /// 检查队列是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this Queue<T> queue)
        {
            return queue != null && queue.Count > 0;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量入队
        /// </summary>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (queue == null || items == null)
                return;

            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// 批量入队（参数数组）
        /// </summary>
        public static void EnqueueRange<T>(this Queue<T> queue, params T[] items)
        {
            queue.EnqueueRange((IEnumerable<T>)items);
        }

        /// <summary>
        /// 批量出队
        /// </summary>
        public static List<T> DequeueRange<T>(this Queue<T> queue, int count)
        {
            var result = new List<T>();
            if (queue == null || count <= 0)
                return result;

            count = Math.Min(count, queue.Count);
            for (int i = 0; i < count; i++)
            {
                result.Add(queue.Dequeue());
            }
            return result;
        }

        /// <summary>
        /// 出队所有元素
        /// </summary>
        public static List<T> DequeueAll<T>(this Queue<T> queue)
        {
            var result = new List<T>();
            if (queue == null)
                return result;

            while (queue.Count > 0)
            {
                result.Add(queue.Dequeue());
            }
            return result;
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 尝试出队
        /// </summary>
        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.HasElements())
            {
                value = queue.Dequeue();
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试查看队首元素
        /// </summary>
        public static bool TryPeek<T>(this Queue<T> queue, out T value)
        {
            if (queue.HasElements())
            {
                value = queue.Peek();
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 安全出队（队列为空时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DequeueOrDefault<T>(this Queue<T> queue, T defaultValue = default)
        {
            return queue.HasElements() ? queue.Dequeue() : defaultValue;
        }

        /// <summary>
        /// 安全查看队首（队列为空时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PeekOrDefault<T>(this Queue<T> queue, T defaultValue = default)
        {
            return queue.HasElements() ? queue.Peek() : defaultValue;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ToList<T>(this Queue<T> queue)
        {
            return queue == null ? new List<T>() : new List<T>(queue);
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this Queue<T> queue)
        {
            return queue == null ? Array.Empty<T>() : queue.ToArray();
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对队列元素执行操作（不出队）
        /// </summary>
        public static void ForEach<T>(this Queue<T> queue, Action<T> action)
        {
            if (queue == null || action == null)
                return;

            foreach (var item in queue)
            {
                action(item);
            }
        }

        #endregion
    }
}
