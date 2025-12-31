// ==========================================================
// 文件名：NativeHashMapExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：NativeHashMap扩展方法，提供便捷的哈希表操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// NativeHashMap扩展方法
    /// 提供便捷的哈希表操作、查询和转换功能
    /// </summary>
    public static class NativeHashMapExtensions
    {
        #region 查询操作

        /// <summary>
        /// 检查哈希表是否为空
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <returns>如果哈希表为空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TKey, TValue>(this NativeHashMap<TKey, TValue> map)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            return !map.IsCreated || map.Count == 0;
        }

        /// <summary>
        /// 尝试获取值，如果不存在则返回默认值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果找到返回对应的值，否则返回默认值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValueOrDefault<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map, 
            TKey key, 
            TValue defaultValue = default)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            return map.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 获取或添加值（如果不存在则添加）
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="key">键</param>
        /// <param name="factory">值工厂函数</param>
        /// <returns>获取或新添加的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrAdd<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map, 
            TKey key, 
            Func<TValue> factory)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (map.TryGetValue(key, out var value))
                return value;
            
            value = factory();
            map[key] = value;
            return value;
        }

        /// <summary>
        /// 添加或更新值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="key">键</param>
        /// <param name="addValue">添加时的值</param>
        /// <param name="updateValueFactory">更新时的值工厂函数</param>
        /// <returns>添加或更新后的值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue AddOrUpdate<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map,
            TKey key,
            TValue addValue,
            Func<TKey, TValue, TValue> updateValueFactory)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (map.TryGetValue(key, out var existingValue))
            {
                var newValue = updateValueFactory(key, existingValue);
                map[key] = newValue;
                return newValue;
            }
            else
            {
                map[key] = addValue;
                return addValue;
            }
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量添加键值对
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="keys">键数组</param>
        /// <param name="values">值数组</param>
        /// <exception cref="ArgumentException">键和值数组长度不匹配时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map,
            NativeArray<TKey> keys,
            NativeArray<TValue> values)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            if (keys.Length != values.Length)
                throw new ArgumentException("键和值数组长度必须匹配");

            for (int i = 0; i < keys.Length; i++)
            {
                map[keys[i]] = values[i];
            }
        }

        /// <summary>
        /// 移除所有满足条件的键值对
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="predicate">条件函数（键，值）</param>
        /// <returns>移除的键值对数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RemoveAll<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map,
            Func<TKey, TValue, bool> predicate)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            int removedCount = 0;
            var keysToRemove = new NativeList<TKey>(map.Count, Allocator.Temp);
            
            foreach (var kvp in map)
            {
                if (predicate(kvp.Key, kvp.Value))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            for (int i = 0; i < keysToRemove.Length; i++)
            {
                if (map.Remove(keysToRemove[i]))
                    removedCount++;
            }

            keysToRemove.Dispose();
            return removedCount;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="allocator">分配器</param>
        /// <returns>键数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<TKey> GetKeys<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map,
            Allocator allocator)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            var keys = new NativeList<TKey>(map.Count, allocator);
            foreach (var key in map.GetKeyArray(Allocator.Temp))
            {
                keys.Add(key);
            }
            return keys;
        }

        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="map">哈希表</param>
        /// <param name="allocator">分配器</param>
        /// <returns>值数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<TValue> GetValues<TKey, TValue>(
            this NativeHashMap<TKey, TValue> map,
            Allocator allocator)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            var values = new NativeList<TValue>(map.Count, allocator);
            foreach (var value in map.GetValueArray(Allocator.Temp))
            {
                values.Add(value);
            }
            return values;
        }

        #endregion
    }
}

