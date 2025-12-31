// ==========================================================
// 文件名：AggregateJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：聚合计算Job，提供高性能的求和、平均值等聚合操作
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 聚合操作接口
    /// </summary>
    public interface IAggregateOperator<T, TResult>
        where T : struct
        where TResult : struct
    {
        /// <summary>获取初始值</summary>
        TResult Identity { get; }
        
        /// <summary>累加操作</summary>
        TResult Accumulate(TResult current, T value);
        
        /// <summary>合并两个结果</summary>
        TResult Combine(TResult a, TResult b);
    }

    /// <summary>
    /// 求和Job（int）
    /// </summary>
    [BurstCompile]
    public struct SumIntJob : IJob
    {
        [ReadOnly] public NativeArray<int> Data;
        public NativeReference<long> Result;

        [BurstCompile]
        public void Execute()
        {
            long sum = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                sum += Data[i];
            }
            Result.Value = sum;
        }
    }

    /// <summary>
    /// 求和Job（float）
    /// </summary>
    [BurstCompile]
    public struct SumFloatJob : IJob
    {
        [ReadOnly] public NativeArray<float> Data;
        public NativeReference<double> Result;

        [BurstCompile]
        public void Execute()
        {
            double sum = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                sum += Data[i];
            }
            Result.Value = sum;
        }
    }

    /// <summary>
    /// 平均值Job（float）
    /// </summary>
    [BurstCompile]
    public struct AverageFloatJob : IJob
    {
        [ReadOnly] public NativeArray<float> Data;
        public NativeReference<float> Result;

        [BurstCompile]
        public void Execute()
        {
            if (Data.Length == 0)
            {
                Result.Value = 0;
                return;
            }

            double sum = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                sum += Data[i];
            }
            Result.Value = (float)(sum / Data.Length);
        }
    }

    /// <summary>
    /// 计数Job（统计满足条件的元素）
    /// </summary>
    [BurstCompile]
    public struct CountJob<T, TPredicate> : IJob
        where T : unmanaged
        where TPredicate : struct, IFilterPredicate<T>
    {
        [ReadOnly] public NativeArray<T> Data;
        public NativeReference<int> Result;
        public TPredicate Predicate;

        [BurstCompile]
        public void Execute()
        {
            int count = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                if (Predicate.Evaluate(Data[i]))
                    count++;
            }
            Result.Value = count;
        }
    }

    /// <summary>
    /// 聚合Job工具类
    /// </summary>
    public static class AggregateJobUtility
    {
        /// <summary>
        /// 调度int求和Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sum(NativeArray<int> data, NativeReference<long> result,
            JobHandle dependency = default)
        {
            var job = new SumIntJob { Data = data, Result = result };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度float求和Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Sum(NativeArray<float> data, NativeReference<double> result,
            JobHandle dependency = default)
        {
            var job = new SumFloatJob { Data = data, Result = result };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度平均值Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Average(NativeArray<float> data, NativeReference<float> result,
            JobHandle dependency = default)
        {
            var job = new AverageFloatJob { Data = data, Result = result };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度计数Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Count<T, TPredicate>(NativeArray<T> data, NativeReference<int> result,
            TPredicate predicate, JobHandle dependency = default)
            where T : unmanaged
            where TPredicate : struct, IFilterPredicate<T>
        {
            var job = new CountJob<T, TPredicate>
            {
                Data = data,
                Result = result,
                Predicate = predicate
            };
            return job.Schedule(dependency);
        }
    }
}
