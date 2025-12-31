// ==========================================================
// 文件名：MemoryUtility.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：内存操作工具，提供内存分配、复制和管理的实用方法
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace AFramework.Burst
{
    /// <summary>
    /// 内存操作工具
    /// 提供内存分配、复制、对齐等实用方法
    /// </summary>
    public static class MemoryUtility
    {
        #region 内存分配

        /// <summary>
        /// 分配对齐的内存块
        /// </summary>
        /// <param name="size">要分配的字节数</param>
        /// <param name="alignment">对齐要求（必须是2的幂）</param>
        /// <param name="allocator">分配器类型</param>
        /// <returns>分配的内存指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* AllocateAligned(long size, int alignment, Allocator allocator)
        {
            if (size <= 0)
                throw new ArgumentException("大小必须大于0", nameof(size));
            if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
                throw new ArgumentException("对齐必须是2的幂", nameof(alignment));

            return UnsafeUtility.Malloc(size, alignment, allocator);
        }

        /// <summary>
        /// 分配类型对齐的内存块
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="count">元素数量</param>
        /// <param name="allocator">分配器类型</param>
        /// <returns>分配的内存指针</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Allocate<T>(int count, Allocator allocator) where T : unmanaged
        {
            long size = (long)UnsafeUtility.SizeOf<T>() * count;
            int alignment = UnsafeUtility.AlignOf<T>();
            return UnsafeUtility.Malloc(size, alignment, allocator);
        }

        /// <summary>
        /// 释放内存块
        /// </summary>
        /// <param name="memory">要释放的内存指针</param>
        /// <param name="allocator">分配器类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Free(void* memory, Allocator allocator)
        {
            if (memory != null)
            {
                UnsafeUtility.Free(memory, allocator);
            }
        }

        #endregion

        #region 内存复制

        /// <summary>
        /// 复制内存块
        /// </summary>
        /// <param name="source">源指针</param>
        /// <param name="destination">目标指针</param>
        /// <param name="size">要复制的字节数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* source, void* destination, long size)
        {
            if (size > 0)
            {
                UnsafeUtility.MemCpy(destination, source, size);
            }
        }

        /// <summary>
        /// 复制类型数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">源指针</param>
        /// <param name="destination">目标指针</param>
        /// <param name="count">元素数量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy<T>(void* source, void* destination, int count) where T : unmanaged
        {
            if (count > 0)
            {
                long size = (long)UnsafeUtility.SizeOf<T>() * count;
                UnsafeUtility.MemCpy(destination, source, size);
            }
        }

        /// <summary>
        /// 移动内存块（处理重叠区域）
        /// </summary>
        /// <param name="source">源指针</param>
        /// <param name="destination">目标指针</param>
        /// <param name="size">要移动的字节数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Move(void* source, void* destination, long size)
        {
            if (size > 0)
            {
                UnsafeUtility.MemMove(destination, source, size);
            }
        }

        #endregion

        #region 内存清零

        /// <summary>
        /// 清零内存块
        /// </summary>
        /// <param name="destination">目标指针</param>
        /// <param name="size">要清零的字节数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear(void* destination, long size)
        {
            if (size > 0)
            {
                UnsafeUtility.MemClear(destination, size);
            }
        }

        /// <summary>
        /// 清零类型数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="destination">目标指针</param>
        /// <param name="count">元素数量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear<T>(void* destination, int count) where T : unmanaged
        {
            if (count > 0)
            {
                long size = (long)UnsafeUtility.SizeOf<T>() * count;
                UnsafeUtility.MemClear(destination, size);
            }
        }

        #endregion

        #region 内存比较

        /// <summary>
        /// 比较两个内存块
        /// </summary>
        /// <param name="ptr1">第一个内存块指针</param>
        /// <param name="ptr2">第二个内存块指针</param>
        /// <param name="size">要比较的字节数</param>
        /// <returns>如果内存块相等返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Compare(void* ptr1, void* ptr2, long size)
        {
            if (ptr1 == ptr2)
                return true;
            if (ptr1 == null || ptr2 == null)
                return false;

            byte* p1 = (byte*)ptr1;
            byte* p2 = (byte*)ptr2;

            for (long i = 0; i < size; i++)
            {
                if (p1[i] != p2[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region 内存对齐

        /// <summary>
        /// 计算对齐后的地址
        /// </summary>
        /// <param name="address">原始地址</param>
        /// <param name="alignment">对齐要求（必须是2的幂）</param>
        /// <returns>对齐后的地址</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Align(void* address, int alignment)
        {
            ulong addr = (ulong)address;
            ulong aligned = (addr + (ulong)(alignment - 1)) & ~((ulong)(alignment - 1));
            return (void*)aligned;
        }

        /// <summary>
        /// 计算对齐后的偏移量
        /// </summary>
        /// <param name="offset">原始偏移量</param>
        /// <param name="alignment">对齐要求（必须是2的幂）</param>
        /// <returns>对齐后的偏移量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AlignOffset(long offset, int alignment)
        {
            return (offset + (alignment - 1)) & ~(alignment - 1);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 计算所需的内存大小（考虑对齐）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="count">元素数量</param>
        /// <returns>所需的内存大小（字节）</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CalculateSize<T>(int count) where T : unmanaged
        {
            return (long)UnsafeUtility.SizeOf<T>() * count;
        }

        /// <summary>
        /// 检查指针是否为空
        /// </summary>
        /// <param name="ptr">指针</param>
        /// <returns>如果指针为空返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsNull(void* ptr)
        {
            return ptr == null;
        }

        #endregion
    }
}

