// ==========================================================
// 文件名：NativeListExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：NativeList扩展方法，提供便捷的列表操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// NativeList扩展方法
    /// 提供便捷的列表操作、查询和转换功能
    /// </summary>
    public static class NativeListExtensions
    {
        #region 查询操作

        /// <summary>
        /// 检查列表是否为空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>如果列表为空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this NativeList<T> list) where T : unmanaged
        {
            return !list.IsCreated || list.Length == 0;
        }

        /// <summary>
        /// 获取列表的第一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>第一个元素</returns>
        /// <exception cref="InvalidOperationException">列表为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(this NativeList<T> list) where T : unmanaged
        {
            if (IsEmpty(list))
                throw new InvalidOperationException("列表为空，无法获取第一个元素");
            return list[0];
        }

        /// <summary>
        /// 获取列表的最后一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>最后一个元素</returns>
        /// <exception cref="InvalidOperationException">列表为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this NativeList<T> list) where T : unmanaged
        {
            if (IsEmpty(list))
                throw new InvalidOperationException("列表为空，无法获取最后一个元素");
            return list[list.Length - 1];
        }

        /// <summary>
        /// 尝试获取并移除最后一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="value">输出的最后一个元素</param>
        /// <returns>如果成功获取返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPop<T>(this NativeList<T> list, out T value) where T : unmanaged
        {
            value = default;
            if (IsEmpty(list))
                return false;
            value = list[list.Length - 1];
            list.RemoveAtSwapBack(list.Length - 1);
            return true;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="items">要添加的元素数组</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this NativeList<T> list, NativeArray<T> items) where T : unmanaged
        {
            if (!items.IsCreated)
                return;
            for (int i = 0; i < items.Length; i++)
            {
                list.Add(items[i]);
            }
        }

        /// <summary>
        /// 批量添加元素（从切片）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="slice">要添加的切片</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this NativeList<T> list, NativeSlice<T> slice) where T : unmanaged
        {
            for (int i = 0; i < slice.Length; i++)
            {
                list.Add(slice[i]);
            }
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="predicate">条件函数</param>
        /// <returns>移除的元素数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RemoveAll<T>(this NativeList<T> list, Func<T, bool> predicate) where T : unmanaged
        {
            int removedCount = 0;
            for (int i = list.Length - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAtSwapBack(i);
                    removedCount++;
                }
            }
            return removedCount;
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找元素的索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果找到返回索引，否则返回-1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this NativeList<T> list, T value) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Equals(value))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 检查列表是否包含指定值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果包含返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this NativeList<T> list, T value) where T : unmanaged, IEquatable<T>
        {
            return IndexOf(list, value) >= 0;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为NativeArray
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeArray</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> ToNativeArray<T>(this NativeList<T> list, Allocator allocator) where T : unmanaged
        {
            return new NativeArray<T>(list.AsArray(), allocator);
        }

        #endregion

        #region 容量管理

        /// <summary>
        /// 确保列表有足够的容量
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="capacity">所需容量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCapacity<T>(this NativeList<T> list, int capacity) where T : unmanaged
        {
            if (list.Capacity < capacity)
            {
                list.Capacity = capacity;
            }
        }

        /// <summary>
        /// 修剪列表容量到实际长度
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimExcess<T>(this NativeList<T> list) where T : unmanaged
        {
            if (list.Capacity > list.Length)
            {
                list.Capacity = list.Length;
            }
        }

        #endregion
    }
}

