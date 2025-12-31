// ==========================================================
// 文件名：FillJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：数据填充Job，提供高性能的NativeArray数据填充功能
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 数据填充Job（并行版本）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [BurstCompile]
    public struct FillJob<T> : IJobParallelFor where T : struct
    {
        /// <summary>目标数据</summary>
        [WriteOnly] public NativeArray<T> Data;
        
        /// <summary>填充值</summary>
        public T Value;

        [BurstCompile]
        public void Execute(int index)
        {
            Data[index] = Value;
        }
    }

    /// <summary>
    /// 带范围的数据填充Job
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [BurstCompile]
    public struct FillRangeJob<T> : IJobParallelFor where T : struct
    {
        /// <summary>目标数据</summary>
        [NativeDisableParallelForRestriction]
        public NativeArray<T> Data;
        
        /// <summary>填充值</summary>
        public T Value;
        
        /// <summary>起始索引</summary>
        public int StartIndex;

        [BurstCompile]
        public void Execute(int index)
        {
            Data[StartIndex + index] = Value;
        }
    }

    /// <summary>
    /// 填充Job工具类
    /// </summary>
    public static class FillJobUtility
    {
        /// <summary>
        /// 创建并调度填充Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Fill<T>(NativeArray<T> data, T value, JobHandle dependency = default)
            where T : struct
        {
            var job = new FillJob<T> { Data = data, Value = value };
            int batchSize = BurstRuntime.CalculateBatchSize(data.Length, UnsafeUtility.SizeOf<T>());
            return job.Schedule(data.Length, batchSize, dependency);
        }

        /// <summary>
        /// 创建并调度范围填充Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle FillRange<T>(NativeArray<T> data, T value, int startIndex, int count,
            JobHandle dependency = default) where T : struct
        {
            var job = new FillRangeJob<T>
            {
                Data = data,
                Value = value,
                StartIndex = startIndex
            };
            int batchSize = BurstRuntime.CalculateBatchSize(count, UnsafeUtility.SizeOf<T>());
            return job.Schedule(count, batchSize, dependency);
        }

        /// <summary>
        /// 同步填充（立即执行）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillImmediate<T>(NativeArray<T> data, T value) where T : struct
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = value;
            }
        }
    }
}
