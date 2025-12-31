// ==========================================================
// 文件名：ParallelSort.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：并行排序，提供高性能的多线程排序算法
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 并行排序工具类
    /// 使用Unity内置的并行排序算法
    /// </summary>
    public static class ParallelSort
    {
        /// <summary>
        /// 并行排序阈值（小于此值使用顺序排序）
        /// </summary>
        public const int ParallelThreshold = 1024;

        /// <summary>
        /// 并行排序NativeArray
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sort<T>(NativeArray<T> data, JobHandle dependency = default)
            where T : unmanaged, IComparable<T>
        {
            if (data.Length < ParallelThreshold)
            {
                // 小数据量使用顺序排序
                var job = new SortJob<T> { Data = data };
                return job.Schedule(dependency);
            }

            // 大数据量使用并行排序
            return SortJobUtility.Sort(data, dependency);
        }

        /// <summary>
        /// 并行排序并立即完成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortImmediate<T>(NativeArray<T> data) where T : unmanaged, IComparable<T>
        {
            data.Sort();
        }

        /// <summary>
        /// 带自定义比较器的并行排序
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sort<T, TComparer>(NativeArray<T> data, TComparer comparer,
            JobHandle dependency = default)
            where T : unmanaged
            where TComparer : struct, IComparer<T>
        {
            return SortJobUtility.Sort(data, comparer, dependency);
        }
    }

    /// <summary>
    /// 部分排序Job（只排序前N个元素）
    /// </summary>
    [BurstCompile]
    public struct PartialSortJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        public NativeArray<T> Data;
        public int TopN;

        [BurstCompile]
        public void Execute()
        {
            int n = Math.Min(TopN, Data.Length);
            
            // 使用选择排序获取前N个最小元素
            for (int i = 0; i < n; i++)
            {
                int minIdx = i;
                for (int j = i + 1; j < Data.Length; j++)
                {
                    if (Data[j].CompareTo(Data[minIdx]) < 0)
                        minIdx = j;
                }
                
                if (minIdx != i)
                {
                    T temp = Data[i];
                    Data[i] = Data[minIdx];
                    Data[minIdx] = temp;
                }
            }
        }
    }

    /// <summary>
    /// 索引排序Job（返回排序后的索引）
    /// </summary>
    [BurstCompile]
    public struct IndexSortJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        [ReadOnly] public NativeArray<T> Data;
        public NativeArray<int> Indices;

        [BurstCompile]
        public void Execute()
        {
            // 初始化索引
            for (int i = 0; i < Indices.Length; i++)
                Indices[i] = i;

            // 根据数据值排序索引
            QuickSortIndices(0, Indices.Length - 1);
        }

        private void QuickSortIndices(int left, int right)
        {
            if (left >= right) return;

            int pivot = PartitionIndices(left, right);
            QuickSortIndices(left, pivot - 1);
            QuickSortIndices(pivot + 1, right);
        }

        private int PartitionIndices(int left, int right)
        {
            int pivotIdx = Indices[right];
            T pivotValue = Data[pivotIdx];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (Data[Indices[j]].CompareTo(pivotValue) <= 0)
                {
                    i++;
                    int temp = Indices[i];
                    Indices[i] = Indices[j];
                    Indices[j] = temp;
                }
            }

            int t = Indices[i + 1];
            Indices[i + 1] = Indices[right];
            Indices[right] = t;
            return i + 1;
        }
    }
}
