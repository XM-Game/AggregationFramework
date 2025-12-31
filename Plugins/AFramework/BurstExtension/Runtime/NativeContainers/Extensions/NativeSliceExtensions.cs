// ==========================================================
// 文件名：NativeSliceExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：NativeSlice扩展方法，提供便捷的切片操作功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;

namespace AFramework.Burst
{
    /// <summary>
    /// NativeSlice扩展方法
    /// 提供便捷的切片操作、查询和转换功能
    /// </summary>
    public static class NativeSliceExtensions
    {
        #region 查询操作

        /// <summary>
        /// 检查切片是否为空
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <returns>如果切片为空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this NativeSlice<T> slice) where T : struct
        {
            return slice.Length == 0;
        }

        /// <summary>
        /// 获取切片的第一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <returns>第一个元素</returns>
        /// <exception cref="InvalidOperationException">切片为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(this NativeSlice<T> slice) where T : struct
        {
            if (IsEmpty(slice))
                throw new InvalidOperationException("切片为空，无法获取第一个元素");
            return slice[0];
        }

        /// <summary>
        /// 获取切片的最后一个元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <returns>最后一个元素</returns>
        /// <exception cref="InvalidOperationException">切片为空时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this NativeSlice<T> slice) where T : struct
        {
            if (IsEmpty(slice))
                throw new InvalidOperationException("切片为空，无法获取最后一个元素");
            return slice[slice.Length - 1];
        }

        #endregion

        #region 切片操作

        /// <summary>
        /// 获取子切片（从指定索引到末尾）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="startIndex">起始索引</param>
        /// <returns>子切片</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSlice<T> Slice<T>(this NativeSlice<T> slice, int startIndex) where T : struct
        {
            return slice.Slice(startIndex);
        }

        /// <summary>
        /// 获取子切片（指定起始索引和长度）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="length">长度</param>
        /// <returns>子切片</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSlice<T> Slice<T>(this NativeSlice<T> slice, int startIndex, int length) where T : struct
        {
            return slice.Slice(startIndex, length);
        }

        /// <summary>
        /// 获取切片的指定范围
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="range">范围（start..end）</param>
        /// <returns>子切片</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSlice<T> Slice<T>(this NativeSlice<T> slice, Range range) where T : struct
        {
            var (offset, length) = range.GetOffsetAndLength(slice.Length);
            return slice.Slice(offset, length);
        }

        #endregion

        #region 查找操作

        /// <summary>
        /// 查找元素的索引
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果找到返回索引，否则返回-1</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this NativeSlice<T> slice, T value) where T : struct, IEquatable<T>
        {
            for (int i = 0; i < slice.Length; i++)
            {
                if (slice[i].Equals(value))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 检查切片是否包含指定值
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="value">要查找的值</param>
        /// <returns>如果包含返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this NativeSlice<T> slice, T value) where T : struct, IEquatable<T>
        {
            return IndexOf(slice, value) >= 0;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为NativeArray
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeArray</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> ToNativeArray<T>(this NativeSlice<T> slice, Allocator allocator) where T : struct
        {
            var array = new NativeArray<T>(slice.Length, allocator);
            slice.CopyTo(array);
            return array;
        }

        /// <summary>
        /// 转换为NativeList
        /// </summary>
        /// <typeparam name="T">元素类型（必须是unmanaged类型）</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="allocator">分配器</param>
        /// <returns>NativeList</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeList<T> ToNativeList<T>(this NativeSlice<T> slice, Allocator allocator) where T : unmanaged
        {
            var list = new NativeList<T>(slice.Length, allocator);
            for (int i = 0; i < slice.Length; i++)
            {
                list.Add(slice[i]);
            }
            return list;
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 安全访问切片元素（带边界检查）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="slice">切片</param>
        /// <param name="index">索引</param>
        /// <param name="value">输出的元素值</param>
        /// <returns>如果索引有效返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this NativeSlice<T> slice, int index, out T value) where T : struct
        {
            value = default;
            if (index < 0 || index >= slice.Length)
                return false;
            value = slice[index];
            return true;
        }

        #endregion
    }
}

