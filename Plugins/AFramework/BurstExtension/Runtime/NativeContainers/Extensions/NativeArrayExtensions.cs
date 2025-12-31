// ==========================================================
// 文件名：NativeArrayExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：NativeArray扩展方法，提供便捷的数组操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// NativeArray扩展方法
    /// 提供便捷的数组操作、查询和转换功能
    /// </summary>
    public static class NativeArrayExtensions
    {
        #region 查询操作

        /// <summary>
        /// 检查数组是否为空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>如果数组为空或长度为0返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this NativeArray<T> array) where T : unmanaged
        {
            return !array.IsCreated || array.Length == 0;
        }

        /// <summary>
        /// 检查数组是否有效且非空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>如果数组有效且非空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidAndNotEmpty<T>(this NativeArray<T> array) where T : unmanaged
        {
            return array.IsCreated && array.Length > 0;
        }

        /// <summary>
        /// 获取数组的第一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>第一个元素</returns>
        /// <exception cref="InvalidOperationException">数组为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(this NativeArray<T> array) where T : unmanaged
        {
            if (IsEmpty(array))
                throw new InvalidOperationException("数组为空，无法获取第一个元素");
            return array[0];
        }

        /// <summary>
        /// 尝试获取数组的第一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">输出的第一个元素</param>
        /// <returns>如果成功获取返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst<T>(this NativeArray<T> array, out T value) where T : unmanaged
        {
            value = default;
            if (IsEmpty(array))
                return false;
            value = array[0];
            return true;
        }

        /// <summary>
        /// 获取数组的最后一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>最后一个元素</returns>
        /// <exception cref="InvalidOperationException">数组为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this NativeArray<T> array) where T : unmanaged
        {
            if (IsEmpty(array))
                throw new InvalidOperationException("数组为空，无法获取最后一个元素");
            return array[array.Length - 1];
        }

        /// <summary>
        /// 尝试获取数组的最后一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">输出的最后一个元素</param>
        /// <returns>如果成功获取返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetLast<T>(this NativeArray<T> array, out T value) where T : unmanaged
        {
            value = default;
            if (IsEmpty(array))
                return false;
            value = array[array.Length - 1];
            return true;
        }

        #endregion

        #region 切片操作

        /// <summary>
        /// 获取数组切片（从指定索引到末尾）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">起始索引</param>
        /// <returns>切片</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSlice<T> Slice<T>(this NativeArray<T> array, int startIndex) where T : unmanaged
        {
            return new NativeSlice<T>(array, startIndex);
        }

        /// <summary>
        /// 获取数组切片（指定起始索引和长度）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="length">长度</param>
        /// <returns>切片</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSlice<T> Slice<T>(this NativeArray<T> array, int startIndex, int length) where T : unmanaged
        {
            return new NativeSlice<T>(array, startIndex, length);
        }

        #endregion

        #region 填充操作

        /// <summary>
        /// 填充数组的所有元素为指定值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">填充值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(this NativeArray<T> array, T value) where T : unmanaged
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// 使用函数填充数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="generator">生成函数（接收索引，返回元素值）</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillWith<T>(this NativeArray<T> array, Func<int, T> generator) where T : unmanaged
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = generator(i);
            }
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找元素的索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果找到返回索引，否则返回-1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this NativeArray<T> array, T value) where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 检查数组是否包含指定值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果包含返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this NativeArray<T> array, T value) where T : unmanaged, IEquatable<T>
        {
            return IndexOf(array, value) >= 0;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为NativeList
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeList</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<T> ToNativeList<T>(this NativeArray<T> array, Allocator allocator) where T : unmanaged
        {
            var list = new NativeList<T>(array.Length, allocator);
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
            return list;
        }

        /// <summary>
        /// 复制到另一个数组
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="source">源数组</param>
        /// <param name="destination">目标数组</param>
        /// <exception cref="ArgumentException">目标数组长度不足时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this NativeArray<T> source, NativeArray<T> destination) where T : unmanaged
        {
            if (destination.Length < source.Length)
                throw new ArgumentException("目标数组长度不足", nameof(destination));
            NativeArray<T>.Copy(source, destination);
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 安全访问数组元素（带边界检查）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="index">索引</param>
        /// <param name="value">输出的元素值</param>
        /// <returns>如果索引有效返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this NativeArray<T> array, int index, out T value) where T : unmanaged
        {
            value = default;
            if (!array.IsCreated || index < 0 || index >= array.Length)
                return false;
            value = array[index];
            return true;
        }

        /// <summary>
        /// 安全设置数组元素（带边界检查）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="index">索引</param>
        /// <param name="value">要设置的值</param>
        /// <returns>如果索引有效并成功设置返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySet<T>(this NativeArray<T> array, int index, T value) where T : unmanaged
        {
            if (!array.IsCreated || index < 0 || index >= array.Length)
                return false;
            array[index] = value;
            return true;
        }

        #endregion
    }
}

