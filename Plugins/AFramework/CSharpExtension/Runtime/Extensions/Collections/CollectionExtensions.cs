// ==========================================================
// 文件名：CollectionExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// ICollection 扩展方法集合
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 批量添加元素
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// 批量移除元素
        /// </summary>
        public static int RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            int count = 0;
            foreach (var item in items)
            {
                if (collection.Remove(item))
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 根据条件移除元素
        /// </summary>
        public static int RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var itemsToRemove = collection.Where(predicate).ToList();
            return RemoveRange(collection, itemsToRemove);
        }

        /// <summary>
        /// 判断集合是否为空或 null
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 判断集合是否包含任意元素
        /// </summary>
        public static bool ContainsAny<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.Any(collection.Contains);
        }

        /// <summary>
        /// 判断集合是否包含所有元素
        /// </summary>
        public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.All(collection.Contains);
        }
    }
}
