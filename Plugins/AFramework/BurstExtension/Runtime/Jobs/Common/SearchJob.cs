// ==========================================================
// 文件名：SearchJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：搜索Job，提供高性能的数据搜索功能
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 线性搜索Job
    /// </summary>
    [BurstCompile]
    public struct LinearSearchJob<T> : IJob where T : unmanaged, IEquatable<T>
    {
        /// <summary>源数据</summary>
        [ReadOnly] public NativeArray<T> Data;
        
        /// <summary>要搜索的值</summary>
        public T Target;
        
        /// <summary>搜索结果（找到的索引，-1表示未找到）</summary>
        public NativeReference<int> Result;

        [BurstCompile]
        public void Execute()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i].Equals(Target))
                {
                    Result.Value = i;
                    return;
                }
            }
            Result.Value = -1;
        }
    }

    /// <summary>
    /// 二分搜索Job（要求数据已排序）
    /// </summary>
    [BurstCompile]
    public struct BinarySearchJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        /// <summary>已排序的源数据</summary>
        [ReadOnly] public NativeArray<T> Data;
        
        /// <summary>要搜索的值</summary>
        public T Target;
        
        /// <summary>搜索结果（找到的索引，负数表示未找到，取反后为插入位置）</summary>
        public NativeReference<int> Result;

        [BurstCompile]
        public void Execute()
        {
            Result.Value = BinarySearch(0, Data.Length - 1);
        }

        private int BinarySearch(int left, int right)
        {
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int cmp = Data[mid].CompareTo(Target);

                if (cmp == 0)
                    return mid;
                if (cmp < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return ~left; // 返回插入位置的按位取反
        }
    }

    /// <summary>
    /// 查找最大值Job
    /// </summary>
    [BurstCompile]
    public struct FindMaxJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        [ReadOnly] public NativeArray<T> Data;
        public NativeReference<T> MaxValue;
        public NativeReference<int> MaxIndex;

        [BurstCompile]
        public void Execute()
        {
            if (Data.Length == 0) return;

            T max = Data[0];
            int maxIdx = 0;

            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i].CompareTo(max) > 0)
                {
                    max = Data[i];
                    maxIdx = i;
                }
            }

            MaxValue.Value = max;
            MaxIndex.Value = maxIdx;
        }
    }

    /// <summary>
    /// 查找最小值Job
    /// </summary>
    [BurstCompile]
    public struct FindMinJob<T> : IJob where T : unmanaged, IComparable<T>
    {
        [ReadOnly] public NativeArray<T> Data;
        public NativeReference<T> MinValue;
        public NativeReference<int> MinIndex;

        [BurstCompile]
        public void Execute()
        {
            if (Data.Length == 0) return;

            T min = Data[0];
            int minIdx = 0;

            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i].CompareTo(min) < 0)
                {
                    min = Data[i];
                    minIdx = i;
                }
            }

            MinValue.Value = min;
            MinIndex.Value = minIdx;
        }
    }

    /// <summary>
    /// 搜索Job工具类
    /// </summary>
    public static class SearchJobUtility
    {
        /// <summary>
        /// 调度线性搜索Job
        /// </summary>
        public static JobHandle LinearSearch<T>(NativeArray<T> data, T target,
            NativeReference<int> result, JobHandle dependency = default)
            where T : unmanaged, IEquatable<T>
        {
            var job = new LinearSearchJob<T>
            {
                Data = data,
                Target = target,
                Result = result
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度二分搜索Job
        /// </summary>
        public static JobHandle BinarySearch<T>(NativeArray<T> data, T target,
            NativeReference<int> result, JobHandle dependency = default)
            where T : unmanaged, IComparable<T>
        {
            var job = new BinarySearchJob<T>
            {
                Data = data,
                Target = target,
                Result = result
            };
            return job.Schedule(dependency);
        }
    }
}
