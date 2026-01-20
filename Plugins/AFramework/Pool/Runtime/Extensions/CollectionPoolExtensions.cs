// ==========================================================
// 文件名：CollectionPoolExtensions.cs
// 命名空间: AFramework.Pool.Extensions
// 依赖: System, System.Collections.Generic
// 功能: 集合池扩展方法
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool.Extensions
{
    /// <summary>
    /// 集合池扩展方法
    /// Collection Pool Extension Methods
    /// </summary>
    /// <remarks>
    /// 提供常用集合类型的池化扩展方法
    /// Provides pooling extension methods for common collection types
    /// </remarks>
    public static class CollectionPoolExtensions
    {
        #region List 扩展 List Extensions

        /// <summary>
        /// 从 List 池获取列表
        /// Get list from List pool
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">列表池 / List pool</param>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        /// <returns>列表实例 / List instance</returns>
        public static List<T> GetList<T>(this IObjectPool<List<T>> pool, int capacity = 0)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var list = pool.Get();

            if (capacity > 0 && list.Capacity < capacity)
            {
                list.Capacity = capacity;
            }

            return list;
        }

        /// <summary>
        /// 归还列表到池（自动清空）
        /// Return list to pool (automatically clear)
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">列表池 / List pool</param>
        /// <param name="list">列表实例 / List instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnList<T>(this IObjectPool<List<T>> pool, List<T> list, bool clear = true)
        {
            if (pool == null || list == null)
                return;

            if (clear)
            {
                list.Clear();
            }

            pool.Return(list);
        }

        /// <summary>
        /// 使用列表（自动归还）
        /// Use list (automatically return)
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">列表池 / List pool</param>
        /// <param name="action">操作委托 / Action delegate</param>
        public static void UseList<T>(this IObjectPool<List<T>> pool, Action<List<T>> action)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var list = pool.Get();
            try
            {
                action(list);
            }
            finally
            {
                list.Clear();
                pool.Return(list);
            }
        }

        #endregion

        #region Dictionary 扩展 Dictionary Extensions

        /// <summary>
        /// 从 Dictionary 池获取字典
        /// Get dictionary from Dictionary pool
        /// </summary>
        /// <typeparam name="TKey">键类型 / Key type</typeparam>
        /// <typeparam name="TValue">值类型 / Value type</typeparam>
        /// <param name="pool">字典池 / Dictionary pool</param>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        /// <returns>字典实例 / Dictionary instance</returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(
            this IObjectPool<Dictionary<TKey, TValue>> pool,
            int capacity = 0)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            return pool.Get();
        }

        /// <summary>
        /// 归还字典到池（自动清空）
        /// Return dictionary to pool (automatically clear)
        /// </summary>
        /// <typeparam name="TKey">键类型 / Key type</typeparam>
        /// <typeparam name="TValue">值类型 / Value type</typeparam>
        /// <param name="pool">字典池 / Dictionary pool</param>
        /// <param name="dictionary">字典实例 / Dictionary instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnDictionary<TKey, TValue>(
            this IObjectPool<Dictionary<TKey, TValue>> pool,
            Dictionary<TKey, TValue> dictionary,
            bool clear = true)
        {
            if (pool == null || dictionary == null)
                return;

            if (clear)
            {
                dictionary.Clear();
            }

            pool.Return(dictionary);
        }

        /// <summary>
        /// 使用字典（自动归还）
        /// Use dictionary (automatically return)
        /// </summary>
        /// <typeparam name="TKey">键类型 / Key type</typeparam>
        /// <typeparam name="TValue">值类型 / Value type</typeparam>
        /// <param name="pool">字典池 / Dictionary pool</param>
        /// <param name="action">操作委托 / Action delegate</param>
        public static void UseDictionary<TKey, TValue>(
            this IObjectPool<Dictionary<TKey, TValue>> pool,
            Action<Dictionary<TKey, TValue>> action)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var dictionary = pool.Get();
            try
            {
                action(dictionary);
            }
            finally
            {
                dictionary.Clear();
                pool.Return(dictionary);
            }
        }

        #endregion

        #region HashSet 扩展 HashSet Extensions

        /// <summary>
        /// 从 HashSet 池获取集合
        /// Get hashset from HashSet pool
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">集合池 / HashSet pool</param>
        /// <returns>集合实例 / HashSet instance</returns>
        public static HashSet<T> GetHashSet<T>(this IObjectPool<HashSet<T>> pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            return pool.Get();
        }

        /// <summary>
        /// 归还集合到池（自动清空）
        /// Return hashset to pool (automatically clear)
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">集合池 / HashSet pool</param>
        /// <param name="hashSet">集合实例 / HashSet instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnHashSet<T>(
            this IObjectPool<HashSet<T>> pool,
            HashSet<T> hashSet,
            bool clear = true)
        {
            if (pool == null || hashSet == null)
                return;

            if (clear)
            {
                hashSet.Clear();
            }

            pool.Return(hashSet);
        }

        #endregion

        #region Queue 扩展 Queue Extensions

        /// <summary>
        /// 从 Queue 池获取队列
        /// Get queue from Queue pool
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">队列池 / Queue pool</param>
        /// <returns>队列实例 / Queue instance</returns>
        public static Queue<T> GetQueue<T>(this IObjectPool<Queue<T>> pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            return pool.Get();
        }

        /// <summary>
        /// 归还队列到池（自动清空）
        /// Return queue to pool (automatically clear)
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">队列池 / Queue pool</param>
        /// <param name="queue">队列实例 / Queue instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnQueue<T>(
            this IObjectPool<Queue<T>> pool,
            Queue<T> queue,
            bool clear = true)
        {
            if (pool == null || queue == null)
                return;

            if (clear)
            {
                queue.Clear();
            }

            pool.Return(queue);
        }

        #endregion

        #region Stack 扩展 Stack Extensions

        /// <summary>
        /// 从 Stack 池获取栈
        /// Get stack from Stack pool
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">栈池 / Stack pool</param>
        /// <returns>栈实例 / Stack instance</returns>
        public static Stack<T> GetStack<T>(this IObjectPool<Stack<T>> pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            return pool.Get();
        }

        /// <summary>
        /// 归还栈到池（自动清空）
        /// Return stack to pool (automatically clear)
        /// </summary>
        /// <typeparam name="T">元素类型 / Element type</typeparam>
        /// <param name="pool">栈池 / Stack pool</param>
        /// <param name="stack">栈实例 / Stack instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnStack<T>(
            this IObjectPool<Stack<T>> pool,
            Stack<T> stack,
            bool clear = true)
        {
            if (pool == null || stack == null)
                return;

            if (clear)
            {
                stack.Clear();
            }

            pool.Return(stack);
        }

        #endregion

        #region StringBuilder 扩展 StringBuilder Extensions

        /// <summary>
        /// 从 StringBuilder 池获取字符串构建器
        /// Get StringBuilder from StringBuilder pool
        /// </summary>
        /// <param name="pool">字符串构建器池 / StringBuilder pool</param>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        /// <returns>字符串构建器实例 / StringBuilder instance</returns>
        public static System.Text.StringBuilder GetStringBuilder(
            this IObjectPool<System.Text.StringBuilder> pool,
            int capacity = 0)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var sb = pool.Get();

            if (capacity > 0 && sb.Capacity < capacity)
            {
                sb.Capacity = capacity;
            }

            return sb;
        }

        /// <summary>
        /// 归还字符串构建器到池（自动清空）
        /// Return StringBuilder to pool (automatically clear)
        /// </summary>
        /// <param name="pool">字符串构建器池 / StringBuilder pool</param>
        /// <param name="sb">字符串构建器实例 / StringBuilder instance</param>
        /// <param name="clear">是否清空 / Whether to clear</param>
        public static void ReturnStringBuilder(
            this IObjectPool<System.Text.StringBuilder> pool,
            System.Text.StringBuilder sb,
            bool clear = true)
        {
            if (pool == null || sb == null)
                return;

            if (clear)
            {
                sb.Clear();
            }

            pool.Return(sb);
        }

        /// <summary>
        /// 使用字符串构建器并获取结果
        /// Use StringBuilder and get result
        /// </summary>
        /// <param name="pool">字符串构建器池 / StringBuilder pool</param>
        /// <param name="action">操作委托 / Action delegate</param>
        /// <returns>构建的字符串 / Built string</returns>
        public static string UseStringBuilder(
            this IObjectPool<System.Text.StringBuilder> pool,
            Action<System.Text.StringBuilder> action)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var sb = pool.Get();
            try
            {
                action(sb);
                return sb.ToString();
            }
            finally
            {
                sb.Clear();
                pool.Return(sb);
            }
        }

        #endregion
    }
}
