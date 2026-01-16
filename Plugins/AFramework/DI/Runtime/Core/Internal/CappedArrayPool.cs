// ==========================================================
// 文件名：CappedArrayPool.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Threading
// 功能: 有上限的数组池，用于复用参数数组，减少GC压力
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 有上限的数组池
    /// <para>用于复用参数数组，减少GC压力</para>
    /// <para>Capped array pool for reusing parameter arrays to reduce GC pressure</para>
    /// </summary>
    /// <typeparam name="T">数组元素类型 / Array element type</typeparam>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>使用 ThreadStatic 避免线程竞争</item>
    /// <item>限制池大小避免内存泄漏</item>
    /// <item>按长度分桶管理，提高复用率</item>
    /// </list>
    /// </remarks>
    internal static class CappedArrayPool<T>
    {
        #region 常量 / Constants

        /// <summary>
        /// 最大支持的数组长度
        /// <para>超过此长度的数组不会被池化</para>
        /// </summary>
        private const int MaxArrayLength = 8;

        /// <summary>
        /// 每个长度桶的最大缓存数量
        /// </summary>
        private const int MaxPooledPerLength = 4;

        #endregion

        #region 共享实例 / Shared Instance

        /// <summary>
        /// 共享的8长度限制池
        /// <para>适用于大多数构造函数和方法参数场景</para>
        /// </summary>
        public static readonly ArrayPoolInstance Shared8Limit = new ArrayPoolInstance(MaxArrayLength, MaxPooledPerLength);

        #endregion

        #region 内部类 / Inner Class

        /// <summary>
        /// 数组池实例
        /// </summary>
        public sealed class ArrayPoolInstance
        {
            [ThreadStatic]
            private static T[][][] _buckets;

            [ThreadStatic]
            private static int[] _counts;

            private readonly int _maxLength;
            private readonly int _maxPooledPerLength;

            public ArrayPoolInstance(int maxLength, int maxPooledPerLength)
            {
                _maxLength = maxLength;
                _maxPooledPerLength = maxPooledPerLength;
            }

            /// <summary>
            /// 确保线程本地存储已初始化
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EnsureInitialized()
            {
                if (_buckets == null)
                {
                    _buckets = new T[_maxLength + 1][][];
                    _counts = new int[_maxLength + 1];
                    
                    for (int i = 0; i <= _maxLength; i++)
                    {
                        _buckets[i] = new T[_maxPooledPerLength][];
                        _counts[i] = 0;
                    }
                }
            }

            /// <summary>
            /// 从池中租用数组
            /// <para>Rent an array from the pool</para>
            /// </summary>
            /// <param name="length">所需数组长度 / Required array length</param>
            /// <returns>数组实例（可能来自池或新创建）/ Array instance (may be from pool or newly created)</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T[] Rent(int length)
            {
                // 长度为0直接返回空数组
                if (length == 0)
                {
                    return Array.Empty<T>();
                }

                // 超过最大长度，直接创建新数组
                if (length > _maxLength)
                {
                    return new T[length];
                }

                EnsureInitialized();

                var bucket = _buckets[length];
                var count = _counts[length];

                if (count > 0)
                {
                    var index = count - 1;
                    var array = bucket[index];
                    bucket[index] = null;
                    _counts[length] = index;
                    return array;
                }

                return new T[length];
            }

            /// <summary>
            /// 将数组归还到池中
            /// <para>Return an array to the pool</para>
            /// </summary>
            /// <param name="array">要归还的数组 / Array to return</param>
            /// <param name="clearArray">是否清空数组内容 / Whether to clear array contents</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Return(T[] array, bool clearArray = true)
            {
                if (array == null || array.Length == 0 || array.Length > _maxLength)
                {
                    return;
                }

                EnsureInitialized();

                // 清空数组内容，避免内存泄漏
                if (clearArray)
                {
                    Array.Clear(array, 0, array.Length);
                }

                var length = array.Length;
                var bucket = _buckets[length];
                var count = _counts[length];

                if (count < _maxPooledPerLength)
                {
                    bucket[count] = array;
                    _counts[length] = count + 1;
                }
                // 如果池已满，直接丢弃数组让GC回收
            }
        }

        #endregion
    }

    /// <summary>
    /// Object类型的数组池快捷访问
    /// <para>Shortcut access for object array pool</para>
    /// </summary>
    internal static class ObjectArrayPool
    {
        /// <summary>
        /// 共享的对象数组池
        /// </summary>
        public static readonly CappedArrayPool<object>.ArrayPoolInstance Shared = 
            CappedArrayPool<object>.Shared8Limit;

        /// <summary>
        /// 租用数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] Rent(int length) => Shared.Rent(length);

        /// <summary>
        /// 归还数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(object[] array) => Shared.Return(array);
    }
}
