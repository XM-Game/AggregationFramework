// ==========================================================
// 文件名：DictionaryExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Dictionary 扩展方法集合
    /// </summary>
    public static class DictionaryExtensions
    {
        #region 安全访问

        /// <summary>
        /// 安全获取字典值，如果键不存在则返回默认值
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 获取或添加值，如果键不存在则添加并返回新值
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                dictionary[key] = value;
            }

            return value;
        }

        /// <summary>
        /// 尝试添加键值对，如果键已存在则返回 false
        /// </summary>
        public static bool TryAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.ContainsKey(key))
                return false;

            dictionary[key] = value;
            return true;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量添加键值对
        /// </summary>
        public static void AddRange<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            IEnumerable<KeyValuePair<TKey, TValue>> items,
            bool overwrite = false)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                if (overwrite || !dictionary.ContainsKey(item.Key))
                {
                    dictionary[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// 批量移除键
        /// </summary>
        public static int RemoveRange<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            IEnumerable<TKey> keys)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            int count = 0;
            foreach (var key in keys)
            {
                if (dictionary.Remove(key))
                    count++;
            }

            return count;
        }

        #endregion

        #region 查询操作

        /// <summary>
        /// 判断字典是否为空或 null
        /// </summary>
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        /// <summary>
        /// 根据值查找键
        /// </summary>
        public static TKey FindKeyByValue<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var kvp in dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                    return kvp.Key;
            }

            return default;
        }

        #endregion
    }
}
