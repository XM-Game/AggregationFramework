// ==========================================================
// 文件名：UnsafeUtilityEx.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：不安全操作扩展，提供额外的内存操作工具方法
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace AFramework.Burst
{
    /// <summary>
    /// 不安全操作扩展
    /// 提供额外的内存操作和指针操作工具方法
    /// </summary>
    public static class UnsafeUtilityEx
    {
        #region 内存操作

        /// <summary>
        /// 复制内存块（带边界检查）
        /// </summary>
        /// <param name="source">源指针</param>
        /// <param name="destination">目标指针</param>
        /// <param name="size">要复制的字节数</param>
        /// <exception cref="ArgumentNullException">指针为null时抛出</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemCopySafe(void* source, void* destination, long size)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (size < 0)
                throw new ArgumentException("大小不能为负数", nameof(size));

            UnsafeUtility.MemCpy(destination, source, size);
        }

        /// <summary>
        /// 移动内存块（处理重叠区域）
        /// </summary>
        /// <param name="source">源指针</param>
        /// <param name="destination">目标指针</param>
        /// <param name="size">要移动的字节数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemMove(void* source, void* destination, long size)
        {
            if (source == null || destination == null || size <= 0)
                return;

            UnsafeUtility.MemMove(destination, source, size);
        }

        /// <summary>
        /// 比较两个内存块
        /// </summary>
        /// <param name="ptr1">第一个内存块指针</param>
        /// <param name="ptr2">第二个内存块指针</param>
        /// <param name="size">要比较的字节数</param>
        /// <returns>如果内存块相等返回0，如果ptr1小于ptr2返回负数，否则返回正数</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int MemCompare(void* ptr1, void* ptr2, long size)
        {
            if (ptr1 == null || ptr2 == null)
                throw new ArgumentNullException(ptr1 == null ? nameof(ptr1) : nameof(ptr2));

            byte* p1 = (byte*)ptr1;
            byte* p2 = (byte*)ptr2;

            for (long i = 0; i < size; i++)
            {
                int diff = p1[i] - p2[i];
                if (diff != 0)
                    return diff;
            }

            return 0;
        }

        #endregion

        #region 类型操作

        /// <summary>
        /// 获取类型的对齐要求
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <returns>对齐要求（字节）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetAlignment<T>() where T : unmanaged
        {
            return UnsafeUtility.AlignOf<T>();
        }

        /// <summary>
        /// 获取类型的大小
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <returns>类型大小（字节）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize<T>() where T : unmanaged
        {
            return UnsafeUtility.SizeOf<T>();
        }

        /// <summary>
        /// 检查类型是否为非托管类型（编译时检查）
        /// </summary>
        /// <typeparam name="T">非托管类型</typeparam>
        /// <returns>始终返回true（因为约束已确保类型为unmanaged）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnmanaged<T>() where T : unmanaged
        {
            return true;
        }

        #endregion

        #region 指针操作

        /// <summary>
        /// 将指针转换为IntPtr
        /// </summary>
        /// <param name="ptr">指针</param>
        /// <returns>IntPtr</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr ToIntPtr(void* ptr)
        {
            return new IntPtr(ptr);
        }

        /// <summary>
        /// 将IntPtr转换为指针
        /// </summary>
        /// <param name="ptr">IntPtr</param>
        /// <returns>指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* ToPointer(IntPtr ptr)
        {
            return ptr.ToPointer();
        }

        /// <summary>
        /// 计算指针偏移
        /// </summary>
        /// <param name="ptr">原始指针</param>
        /// <param name="offset">偏移量（字节）</param>
        /// <returns>偏移后的指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Add(void* ptr, long offset)
        {
            return (byte*)ptr + offset;
        }

        /// <summary>
        /// 计算指针偏移（类型安全）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ptr">原始指针</param>
        /// <param name="offset">偏移量（元素数量）</param>
        /// <returns>偏移后的指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Add<T>(void* ptr, int offset) where T : unmanaged
        {
            return (byte*)ptr + offset * UnsafeUtility.SizeOf<T>();
        }

        #endregion

        #region 数组操作

        /// <summary>
        /// 获取数组元素指针
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="index">索引</param>
        /// <returns>元素指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* GetElementPtr<T>(void* array, int index) where T : unmanaged
        {
            return Add<T>(array, index);
        }

        /// <summary>
        /// 读取数组元素（通过指针）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组指针</param>
        /// <param name="index">索引</param>
        /// <returns>元素值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T ReadArrayElement<T>(void* array, int index) where T : unmanaged
        {
            return UnsafeUtility.ReadArrayElement<T>(array, index);
        }

        /// <summary>
        /// 写入数组元素（通过指针）
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="array">数组指针</param>
        /// <param name="index">索引</param>
        /// <param name="value">要写入的值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteArrayElement<T>(void* array, int index, T value) where T : unmanaged
        {
            UnsafeUtility.WriteArrayElement(array, index, value);
        }

        #endregion
    }
}

