// ==========================================================
// 文件名：TypeKeyHashtable.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 高性能类型键哈希表，针对DI容器的类型查找场景优化
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 高性能类型键哈希表
    /// <para>针对DI容器的类型查找场景优化，构建后只读，无需扩容</para>
    /// <para>High-performance type-key hashtable optimized for DI container type lookups</para>
    /// </summary>
    /// <typeparam name="TValue">值类型 / Value type</typeparam>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>使用 RuntimeHelpers.GetHashCode 获取类型的运行时哈希值</item>
    /// <item>采用开放寻址法处理哈希冲突</item>
    /// <item>构建时确定容量，运行时只读</item>
    /// <item>支持 Type 和 Key 的复合键查找</item>
    /// </list>
    /// </remarks>
    internal sealed class TypeKeyHashtable<TValue>
    {
        #region 内部结构 / Internal Structures

        /// <summary>
        /// 哈希表条目
        /// </summary>
        private readonly struct HashEntry
        {
            /// <summary>类型键 / Type key</summary>
            public readonly Type Type;
            /// <summary>附加键（可选）/ Additional key (optional)</summary>
            public readonly object Key;
            /// <summary>存储的值 / Stored value</summary>
            public readonly TValue Value;

            public HashEntry(Type type, object key, TValue value)
            {
                Type = type;
                Key = key;
                Value = value;
            }
        }

        #endregion

        #region 字段 / Fields

        /// <summary>
        /// 哈希桶数组，每个桶是一个条目数组（处理冲突）
        /// </summary>
        private readonly HashEntry[][] _buckets;

        /// <summary>
        /// 用于计算桶索引的掩码（容量-1，要求容量为2的幂）
        /// </summary>
        private readonly int _indexMask;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 从键值对集合构建哈希表
        /// </summary>
        /// <param name="entries">键值对集合 / Key-value pairs</param>
        /// <param name="loadFactor">负载因子 / Load factor</param>
        public TypeKeyHashtable(IReadOnlyList<KeyValuePair<(Type, object), TValue>> entries, float loadFactor = 0.75f)
        {
            if (entries == null || entries.Count == 0)
            {
                _buckets = Array.Empty<HashEntry[]>();
                _indexMask = 0;
                return;
            }

            // 计算初始容量
            var initialCapacity = (int)(entries.Count / loadFactor);

            // 确保容量为2的幂
            var capacity = 1;
            while (capacity < initialCapacity)
            {
                capacity <<= 1;
            }

            _buckets = new HashEntry[capacity][];
            _indexMask = capacity - 1;

            // 填充哈希表
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var hash = ComputeHashCode(entry.Key.Item1, entry.Key.Item2);
                var bucketIndex = hash & _indexMask;

                var bucket = _buckets[bucketIndex];
                if (bucket == null)
                {
                    // 创建新桶
                    _buckets[bucketIndex] = new HashEntry[1]
                    {
                        new HashEntry(entry.Key.Item1, entry.Key.Item2, entry.Value)
                    };
                }
                else
                {
                    // 扩展现有桶
                    var newBucket = new HashEntry[bucket.Length + 1];
                    Array.Copy(bucket, newBucket, bucket.Length);
                    newBucket[bucket.Length] = new HashEntry(entry.Key.Item1, entry.Key.Item2, entry.Value);
                    _buckets[bucketIndex] = newBucket;
                }
            }
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 尝试获取指定类型的值
        /// <para>Try to get value for the specified type</para>
        /// </summary>
        /// <param name="type">类型键 / Type key</param>
        /// <param name="value">输出值 / Output value</param>
        /// <returns>是否找到 / Whether found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(Type type, out TValue value)
        {
            return TryGet(type, null, out value);
        }

        /// <summary>
        /// 尝试获取指定类型和键的值
        /// <para>Try to get value for the specified type and key</para>
        /// </summary>
        /// <param name="type">类型键 / Type key</param>
        /// <param name="key">附加键 / Additional key</param>
        /// <param name="value">输出值 / Output value</param>
        /// <returns>是否找到 / Whether found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(Type type, object key, out TValue value)
        {
            if (_buckets.Length == 0)
            {
                value = default;
                return false;
            }

            var hash = ComputeHashCode(type, key);
            var bucket = _buckets[hash & _indexMask];

            if (bucket == null)
            {
                value = default;
                return false;
            }

            // 快速路径：检查第一个条目
            ref var first = ref bucket[0];
            if (first.Type == type && KeyEquals(first.Key, key))
            {
                value = first.Value;
                return true;
            }

            // 遍历桶中的其他条目
            for (int i = 1; i < bucket.Length; i++)
            {
                ref var entry = ref bucket[i];
                if (entry.Type == type && KeyEquals(entry.Key, key))
                {
                    value = entry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 检查是否包含指定类型
        /// <para>Check if contains the specified type</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Type type)
        {
            return TryGet(type, null, out _);
        }

        /// <summary>
        /// 检查是否包含指定类型和键
        /// <para>Check if contains the specified type and key</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Type type, object key)
        {
            return TryGet(type, key, out _);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 计算类型和键的组合哈希值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeHashCode(Type type, object key)
        {
            // 使用 RuntimeHelpers.GetHashCode 获取类型的运行时哈希值
            // 这比 type.GetHashCode() 更快，因为它直接返回对象的同步块索引
            var typeHash = RuntimeHelpers.GetHashCode(type);

            if (key == null)
            {
                return typeHash;
            }

            // FNV-1a 风格的哈希组合
            var keyHash = key.GetHashCode();
            return (typeHash * 397) ^ keyHash;
        }

        /// <summary>
        /// 比较两个键是否相等
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool KeyEquals(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        #endregion
    }
}
