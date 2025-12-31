// ==========================================================
// 文件名：DictionaryExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Dictionary 扩展方法
    /// <para>提供字典的常用操作扩展，包括安全访问、批量操作、转换等功能</para>
    /// </summary>
    public static class DictionaryExtensions
    {
        #region 安全访问

        /// <summary>
        /// 安全获取字典值，键不存在时返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null || key == null)
                return defaultValue;
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 安全获取字典值，键不存在时通过工厂方法创建默认值
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultFactory)
        {
            if (dictionary == null || key == null || defaultFactory == null)
                return default;
            return dictionary.TryGetValue(key, out var value) ? value : defaultFactory();
        }

        /// <summary>
        /// 获取值，键不存在时添加默认值并返回
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null || key == null)
                return defaultValue;

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = defaultValue;
                dictionary[key] = value;
            }
            return value;
        }

        /// <summary>
        /// 获取值，键不存在时通过工厂方法创建并添加
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            if (dictionary == null || key == null || factory == null)
                return default;

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = factory();
                dictionary[key] = value;
            }
            return value;
        }

        /// <summary>
        /// 获取值，键不存在时通过工厂方法创建并添加（工厂方法接收键作为参数）
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            if (dictionary == null || key == null || factory == null)
                return default;

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = factory(key);
                dictionary[key] = value;
            }
            return value;
        }

        /// <summary>
        /// 检查字典是否为空或 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        /// <summary>
        /// 检查字典是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary != null && dictionary.Count > 0;
        }

        #endregion

        #region 添加操作

        /// <summary>
        /// 添加键值对（如果键不存在）
        /// </summary>
        public static bool AddIfNotContains<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null || key == null)
                return false;

            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量添加键值对
        /// </summary>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (dictionary == null || items == null)
                return;

            foreach (var item in items)
            {
                dictionary[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// 批量添加键值对（如果键不存在）
        /// </summary>
        public static int AddRangeIfNotContains<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (dictionary == null || items == null)
                return 0;

            int count = 0;
            foreach (var item in items)
            {
                if (dictionary.AddIfNotContains(item.Key, item.Value))
                    count++;
            }
            return count;
        }

        #endregion

        #region 移除操作

        /// <summary>
        /// 移除所有满足条件的键值对
        /// </summary>
        public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (dictionary == null || predicate == null)
                return 0;

            var keysToRemove = new List<TKey>();
            foreach (var pair in dictionary)
            {
                if (predicate(pair))
                    keysToRemove.Add(pair.Key);
            }

            foreach (var key in keysToRemove)
            {
                dictionary.Remove(key);
            }

            return keysToRemove.Count;
        }

        /// <summary>
        /// 移除所有值满足条件的键值对
        /// </summary>
        public static int RemoveAllByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<TValue, bool> predicate)
        {
            if (dictionary == null || predicate == null)
                return 0;

            return dictionary.RemoveAll(pair => predicate(pair.Value));
        }

        /// <summary>
        /// 尝试移除并返回值
        /// </summary>
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新值（如果键存在）
        /// </summary>
        public static bool UpdateIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null || key == null)
                return false;

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新值（如果键存在），使用更新函数
        /// </summary>
        public static bool UpdateIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updateFunc)
        {
            if (dictionary == null || key == null || updateFunc == null)
                return false;

            if (dictionary.TryGetValue(key, out var oldValue))
            {
                dictionary[key] = updateFunc(oldValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加或更新值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary != null && key != null)
                dictionary[key] = value;
        }

        /// <summary>
        /// 添加或更新值（使用工厂方法和更新函数）
        /// </summary>
        public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> addFactory, Func<TValue, TValue> updateFunc)
        {
            if (dictionary == null || key == null || addFactory == null || updateFunc == null)
                return default;

            if (dictionary.TryGetValue(key, out var oldValue))
            {
                var newValue = updateFunc(oldValue);
                dictionary[key] = newValue;
                return newValue;
            }
            else
            {
                var newValue = addFactory();
                dictionary[key] = newValue;
                return newValue;
            }
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<KeyValuePair<TKey, TValue>> ToList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null ? new List<KeyValuePair<TKey, TValue>>() : new List<KeyValuePair<TKey, TValue>>(dictionary);
        }

        /// <summary>
        /// 转换键为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TKey> KeysToList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null ? new List<TKey>() : new List<TKey>(dictionary.Keys);
        }

        /// <summary>
        /// 转换值为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TValue> ValuesToList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null ? new List<TValue>() : new List<TValue>(dictionary.Values);
        }

        /// <summary>
        /// 映射为新字典
        /// </summary>
        public static Dictionary<TKey, TResult> MapValues<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, Func<TValue, TResult> selector)
        {
            var result = new Dictionary<TKey, TResult>();
            if (dictionary == null || selector == null)
                return result;

            foreach (var pair in dictionary)
            {
                result[pair.Key] = selector(pair.Value);
            }
            return result;
        }

        /// <summary>
        /// 映射为新字典（选择器接收键和值）
        /// </summary>
        public static Dictionary<TKey, TResult> MapValues<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, Func<TKey, TValue, TResult> selector)
        {
            var result = new Dictionary<TKey, TResult>();
            if (dictionary == null || selector == null)
                return result;

            foreach (var pair in dictionary)
            {
                result[pair.Key] = selector(pair.Key, pair.Value);
            }
            return result;
        }

        #endregion

        #region 合并操作

        /// <summary>
        /// 合并另一个字典（覆盖已存在的键）
        /// </summary>
        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
        {
            if (dictionary == null || other == null)
                return;

            foreach (var pair in other)
            {
                dictionary[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// 合并另一个字典（不覆盖已存在的键）
        /// </summary>
        public static void MergeIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
        {
            if (dictionary == null || other == null)
                return;

            foreach (var pair in other)
            {
                dictionary.AddIfNotContains(pair.Key, pair.Value);
            }
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找第一个满足条件的键值对
        /// </summary>
        public static Optional<KeyValuePair<TKey, TValue>> Find<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (dictionary == null || predicate == null)
                return Optional<KeyValuePair<TKey, TValue>>.None;

            foreach (var pair in dictionary)
            {
                if (predicate(pair))
                    return Optional<KeyValuePair<TKey, TValue>>.Some(pair);
            }
            return Optional<KeyValuePair<TKey, TValue>>.None;
        }

        /// <summary>
        /// 查找所有满足条件的键值对
        /// </summary>
        public static Dictionary<TKey, TValue> FindAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var result = new Dictionary<TKey, TValue>();
            if (dictionary == null || predicate == null)
                return result;

            foreach (var pair in dictionary)
            {
                if (predicate(pair))
                    result[pair.Key] = pair.Value;
            }
            return result;
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对字典元素执行操作
        /// </summary>
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<KeyValuePair<TKey, TValue>> action)
        {
            if (dictionary == null || action == null)
                return;

            foreach (var pair in dictionary)
            {
                action(pair);
            }
        }

        /// <summary>
        /// 对字典元素执行操作（分离键和值）
        /// </summary>
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            if (dictionary == null || action == null)
                return;

            foreach (var pair in dictionary)
            {
                action(pair.Key, pair.Value);
            }
        }

        #endregion

        #region 反转操作

        /// <summary>
        /// 反转字典（键值互换）
        /// </summary>
        /// <remarks>注意：如果值有重复，后面的会覆盖前面的</remarks>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var result = new Dictionary<TValue, TKey>();
            if (dictionary == null)
                return result;

            foreach (var pair in dictionary)
            {
                result[pair.Value] = pair.Key;
            }
            return result;
        }

        #endregion
    }
}
