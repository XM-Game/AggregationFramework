// ==========================================================
// 文件名：ReadOnlyExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 只读集合扩展方法集合
    /// </summary>
    public static class ReadOnlyExtensions
    {
        /// <summary>
        /// 将集合转换为只读集合
        /// </summary>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return new ReadOnlyCollection<T>(list);
        }

        /// <summary>
        /// 判断只读集合是否为空或 null
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 安全获取只读列表元素
        /// </summary>
        public static T GetValueOrDefault<T>(this IReadOnlyList<T> list, int index, T defaultValue = default)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return index >= 0 && index < list.Count ? list[index] : defaultValue;
        }

        /// <summary>
        /// 查找元素索引
        /// </summary>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i], item))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 判断只读字典是否为空或 null
        /// </summary>
        public static bool IsNullOrEmpty<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        /// <summary>
        /// 安全获取只读字典值
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
