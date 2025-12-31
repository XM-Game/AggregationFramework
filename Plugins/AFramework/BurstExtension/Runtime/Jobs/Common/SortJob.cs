// ==========================================================
// 文件名：SortJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：排序Job，提供高性能的NativeArray排序功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 比较器接口（Burst兼容）
    /// </summary>
    public interface IComparer<T> where T : struct
    {
        /// <summary>
        /// 比较两个元素
        /// </summary>
        /// <returns>负数表示a小于b，0表示相等，正数表示a大于b</returns>
        int Compare(T a, T b);
    }

    /// <summary>
    /// 排序Job（使用Unity内置排序）
    /// </summary>
    [BurstCompile]
    public struct SortJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        /// <summary>要排序的数据</summary>
        public NativeArray<T> Data;

        [BurstCompile]
        public void Execute()
        {
            Data.Sort();
        }
    }

    /// <summary>
    /// 带自定义比较器的排序Job
    /// </summary>
    [BurstCompile]
    public struct SortJobWithComparer<T, TComparer> : IJob
        where T : unmanaged
        where TComparer : struct, IComparer<T>
    {
        /// <summary>要排序的数据</summary>
        public NativeArray<T> Data;
        
        /// <summary>比较器</summary>
        public TComparer Comparer;

        [BurstCompile]
        public void Execute()
        {
            // 使用快速排序
            QuickSort(0, Data.Length - 1);
        }

        private void QuickSort(int left, int right)
        {
            if (left >= right) return;

            int pivot = Partition(left, right);
            QuickSort(left, pivot - 1);
            QuickSort(pivot + 1, right);
        }

        private int Partition(int left, int right)
        {
            T pivot = Data[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (Comparer.Compare(Data[j], pivot) <= 0)
                {
                    i++;
                    Swap(i, j);
                }
            }

            Swap(i + 1, right);
            return i + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i, int j)
        {
            T temp = Data[i];
            Data[i] = Data[j];
            Data[j] = temp;
        }
    }

    #region 常用比较器

    /// <summary>
    /// 升序比较器（int）
    /// </summary>
    [BurstCompile]
    public struct AscendingIntComparer : IComparer<int>
    {
        [BurstCompile]
        public int Compare(int a, int b) => a - b;
    }

    /// <summary>
    /// 降序比较器（int）
    /// </summary>
    [BurstCompile]
    public struct DescendingIntComparer : IComparer<int>
    {
        [BurstCompile]
        public int Compare(int a, int b) => b - a;
    }

    /// <summary>
    /// 升序比较器（float）
    /// </summary>
    [BurstCompile]
    public struct AscendingFloatComparer : IComparer<float>
    {
        [BurstCompile]
        public int Compare(float a, float b)
        {
            if (a < b) return -1;
            if (a > b) return 1;
            return 0;
        }
    }

    /// <summary>
    /// 降序比较器（float）
    /// </summary>
    [BurstCompile]
    public struct DescendingFloatComparer : IComparer<float>
    {
        [BurstCompile]
        public int Compare(float a, float b)
        {
            if (a > b) return -1;
            if (a < b) return 1;
            return 0;
        }
    }

    #endregion

    /// <summary>
    /// 排序Job工具类
    /// </summary>
    public static class SortJobUtility
    {
        /// <summary>
        /// 调度排序Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sort<T>(NativeArray<T> data, JobHandle dependency = default)
            where T : unmanaged, IComparable<T>
        {
            var job = new SortJob<T> { Data = data };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度带比较器的排序Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sort<T, TComparer>(NativeArray<T> data, TComparer comparer,
            JobHandle dependency = default)
            where T : unmanaged
            where TComparer : struct, IComparer<T>
        {
            var job = new SortJobWithComparer<T, TComparer>
            {
                Data = data,
                Comparer = comparer
            };
            return job.Schedule(dependency);
        }
    }
}
