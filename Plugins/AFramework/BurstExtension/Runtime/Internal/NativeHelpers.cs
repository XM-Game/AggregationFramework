// ==========================================================
// 文件名：NativeHelpers.cs
// 命名空间：AFramework.Burst.Internal
// 创建时间：2026-01-01
// 功能描述：Native辅助类，提供内部使用的原生容器工具方法
// 依赖：Unity.Burst, Unity.Collections
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace AFramework.Burst.Internal
{
    /// <summary>
    /// Native辅助类
    /// 提供内部使用的原生容器工具方法
    /// </summary>
    [BurstCompile]
    internal static class NativeHelpers
    {
        #region 内存操作

        /// <summary>
        /// 快速清零内存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemClear(void* ptr, long sizeBytes)
        {
            UnsafeUtility.MemClear(ptr, sizeBytes);
        }

        /// <summary>
        /// 快速复制内存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemCopy(void* destination, void* source, long sizeBytes)
        {
            UnsafeUtility.MemCpy(destination, source, sizeBytes);
        }

        /// <summary>
        /// 快速移动内存（处理重叠）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemMove(void* destination, void* source, long sizeBytes)
        {
            UnsafeUtility.MemMove(destination, source, sizeBytes);
        }

        /// <summary>
        /// 填充内存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemSet(void* ptr, byte value, long sizeBytes)
        {
            UnsafeUtility.MemSet(ptr, value, sizeBytes);
        }

        /// <summary>
        /// 比较内存
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int MemCompare(void* ptr1, void* ptr2, long sizeBytes)
        {
            return UnsafeUtility.MemCmp(ptr1, ptr2, sizeBytes);
        }

        #endregion

        #region 数组操作

        /// <summary>
        /// 清空NativeArray
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ClearArray<T>(NativeArray<T> array) where T : struct
        {
            if (!array.IsCreated || array.Length == 0)
                return;
            
            UnsafeUtility.MemClear(array.GetUnsafePtr(), array.Length * UnsafeUtility.SizeOf<T>());
        }

        /// <summary>
        /// 填充NativeArray
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillArray<T>(NativeArray<T> array, T value) where T : struct
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// 复制NativeArray
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyArray<T>(NativeArray<T> source, NativeArray<T> destination) where T : struct
        {
            int count = math.min(source.Length, destination.Length);
            NativeArray<T>.Copy(source, destination, count);
        }

        /// <summary>
        /// 复制NativeArray的一部分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyArrayRange<T>(NativeArray<T> source, int sourceIndex, 
            NativeArray<T> destination, int destIndex, int count) where T : struct
        {
            NativeArray<T>.Copy(source, sourceIndex, destination, destIndex, count);
        }

        #endregion

        #region 指针操作

        /// <summary>
        /// 获取NativeArray的不安全指针
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* GetArrayPtr<T>(NativeArray<T> array) where T : struct
        {
            return array.GetUnsafePtr();
        }

        /// <summary>
        /// 获取NativeArray的只读不安全指针
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* GetArrayReadOnlyPtr<T>(NativeArray<T> array) where T : struct
        {
            return array.GetUnsafeReadOnlyPtr();
        }

        /// <summary>
        /// 从指针创建NativeArray（不拥有内存）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe NativeArray<T> CreateArrayFromPtr<T>(void* ptr, int length) where T : struct
        {
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(
                ptr, length, Allocator.None);
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
#endif
            return array;
        }

        /// <summary>
        /// 计算指针偏移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* OffsetPtr(void* ptr, long offsetBytes)
        {
            return (byte*)ptr + offsetBytes;
        }

        /// <summary>
        /// 计算类型化指针偏移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* OffsetPtr<T>(T* ptr, int elementOffset) where T : unmanaged
        {
            return ptr + elementOffset;
        }

        #endregion

        #region 类型工具

        /// <summary>
        /// 获取类型大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>() where T : struct
        {
            return UnsafeUtility.SizeOf<T>();
        }

        /// <summary>
        /// 获取类型对齐
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignOf<T>() where T : struct
        {
            return UnsafeUtility.AlignOf<T>();
        }

        /// <summary>
        /// 重新解释类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe TTo ReinterpretCast<TFrom, TTo>(TFrom value) 
            where TFrom : unmanaged 
            where TTo : unmanaged
        {
            return *(TTo*)&value;
        }

        /// <summary>
        /// 重新解释NativeArray类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<TTo> ReinterpretArray<TFrom, TTo>(NativeArray<TFrom> array) 
            where TFrom : struct 
            where TTo : struct
        {
            return array.Reinterpret<TTo>(UnsafeUtility.SizeOf<TFrom>());
        }

        #endregion

        #region 分配器工具

        /// <summary>
        /// 检查分配器是否为临时分配器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTempAllocator(Allocator allocator)
        {
            return allocator == Allocator.Temp || allocator == Allocator.TempJob;
        }

        /// <summary>
        /// 检查分配器是否为持久分配器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersistentAllocator(Allocator allocator)
        {
            return allocator == Allocator.Persistent;
        }

        /// <summary>
        /// 获取推荐的分配器
        /// </summary>
        /// <param name="expectedLifetime">预期生命周期（帧数）</param>
        /// <returns>推荐的分配器</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator GetRecommendedAllocator(int expectedLifetime)
        {
            if (expectedLifetime <= 1)
                return Allocator.Temp;
            if (expectedLifetime <= 4)
                return Allocator.TempJob;
            return Allocator.Persistent;
        }

        #endregion

        #region 容量计算

        /// <summary>
        /// 计算增长后的容量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateGrowCapacity(int currentCapacity, int requiredCapacity)
        {
            // 使用1.5倍增长策略
            int newCapacity = currentCapacity + currentCapacity / 2;
            return math.max(newCapacity, requiredCapacity);
        }

        /// <summary>
        /// 计算2的幂容量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculatePowerOfTwoCapacity(int requiredCapacity)
        {
            requiredCapacity--;
            requiredCapacity |= requiredCapacity >> 1;
            requiredCapacity |= requiredCapacity >> 2;
            requiredCapacity |= requiredCapacity >> 4;
            requiredCapacity |= requiredCapacity >> 8;
            requiredCapacity |= requiredCapacity >> 16;
            return requiredCapacity + 1;
        }

        /// <summary>
        /// 计算哈希表的最优容量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateHashMapCapacity(int expectedCount, float loadFactor = 0.75f)
        {
            int capacity = (int)(expectedCount / loadFactor) + 1;
            return CalculatePowerOfTwoCapacity(capacity);
        }

        #endregion

        #region 边界检查

        /// <summary>
        /// 检查并钳制索引
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampIndex(int index, int length)
        {
            return math.clamp(index, 0, length - 1);
        }

        /// <summary>
        /// 检查并包装索引（循环）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WrapIndex(int index, int length)
        {
            index = index % length;
            return index < 0 ? index + length : index;
        }

        /// <summary>
        /// 安全获取数组元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SafeGet<T>(NativeArray<T> array, int index, T defaultValue = default) where T : struct
        {
            if (index < 0 || index >= array.Length)
                return defaultValue;
            return array[index];
        }

        #endregion
    }
}
