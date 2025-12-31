// ==========================================================
// 文件名：ParallelFilter.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：并行过滤，提供高性能的多线程数据过滤算法
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// 并行过滤标记Job（第一阶段）
    /// </summary>
    [BurstCompile]
    public struct ParallelFilterMarkJob<T, TPredicate> : IJobParallelFor
        where T : unmanaged
        where TPredicate : struct, IFilterPredicate<T>
    {
        [ReadOnly] public NativeArray<T> Input;
        [WriteOnly] public NativeArray<int> Marks;
        public TPredicate Predicate;

        [BurstCompile]
        public void Execute(int index)
        {
            Marks[index] = Predicate.Evaluate(Input[index]) ? 1 : 0;
        }
    }

    /// <summary>
    /// 并行过滤收集Job（最终阶段）
    /// </summary>
    [BurstCompile]
    public struct ParallelFilterCollectJob<T> : IJobParallelFor where T : unmanaged
    {
        [ReadOnly] public NativeArray<T> Input;
        [ReadOnly] public NativeArray<int> Marks;
        [ReadOnly] public NativeArray<int> Indices;
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<T> Output;

        [BurstCompile]
        public void Execute(int index)
        {
            if (Marks[index] == 1)
            {
                int outputIndex = Indices[index] - 1;
                Output[outputIndex] = Input[index];
            }
        }
    }

    /// <summary>
    /// 顺序过滤Job（小数据量使用）
    /// </summary>
    [BurstCompile]
    public struct SequentialFilterJob<T, TPredicate> : IJob
        where T : unmanaged
        where TPredicate : struct, IFilterPredicate<T>
    {
        [ReadOnly] public NativeArray<T> Input;
        public NativeList<T> Output;
        public TPredicate Predicate;

        [BurstCompile]
        public void Execute()
        {
            for (int i = 0; i < Input.Length; i++)
            {
                if (Predicate.Evaluate(Input[i]))
                {
                    Output.Add(Input[i]);
                }
            }
        }
    }

    /// <summary>
    /// 并行过滤工具类
    /// </summary>
    public static class ParallelFilterUtility
    {
        /// <summary>
        /// 并行阈值
        /// </summary>
        public const int ParallelThreshold = 4096;

        /// <summary>
        /// 执行顺序过滤（小数据量）
        /// </summary>
        public static JobHandle FilterSequential<T, TPredicate>(NativeArray<T> input, NativeList<T> output,
            TPredicate predicate, JobHandle dependency = default)
            where T : unmanaged
            where TPredicate : struct, IFilterPredicate<T>
        {
            var job = new SequentialFilterJob<T, TPredicate>
            {
                Input = input,
                Output = output,
                Predicate = predicate
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 执行并行过滤
        /// </summary>
        public static JobHandle Filter<T, TPredicate>(NativeArray<T> input, NativeList<T> output,
            TPredicate predicate, Allocator allocator = Allocator.TempJob, JobHandle dependency = default)
            where T : unmanaged
            where TPredicate : struct, IFilterPredicate<T>
        {
            if (input.Length < ParallelThreshold)
            {
                return FilterSequential(input, output, predicate, dependency);
            }

            return ExecuteParallelFilter(input, output, predicate, allocator, dependency);
        }

        private static JobHandle ExecuteParallelFilter<T, TPredicate>(NativeArray<T> input,
            NativeList<T> output, TPredicate predicate, Allocator allocator, JobHandle dependency)
            where T : unmanaged
            where TPredicate : struct, IFilterPredicate<T>
        {
            var marks = new NativeArray<int>(input.Length, allocator);
            var indices = new NativeArray<int>(input.Length, allocator);

            // 阶段1：标记满足条件的元素
            var markJob = new ParallelFilterMarkJob<T, TPredicate>
            {
                Input = input,
                Marks = marks,
                Predicate = predicate
            };
            var handle = markJob.Schedule(input.Length, 64, dependency);

            // 阶段2：计算前缀和获取输出索引
            var scanOp = new SumIntReduceOperator();
            handle = ParallelScanUtility.InclusiveScan(marks, indices, scanOp, allocator, handle);

            // 获取总数并调整输出大小
            var countRef = new NativeReference<int>(allocator);
            var getCountJob = new GetLastElementJob<int>
            {
                Array = indices,
                Result = countRef
            };
            handle = getCountJob.Schedule(handle);

            // 阶段3：收集结果
            var collectJob = new ParallelFilterCollectJob<T>
            {
                Input = input,
                Marks = marks,
                Indices = indices,
                Output = output.AsArray()
            };
            handle = collectJob.Schedule(input.Length, 64, handle);

            // 清理临时数组
            handle = marks.Dispose(handle);
            handle = indices.Dispose(handle);
            handle = countRef.Dispose(handle);

            return handle;
        }
    }

    /// <summary>
    /// 获取数组最后一个元素的Job
    /// </summary>
    [BurstCompile]
    internal struct GetLastElementJob<T> : IJob where T : unmanaged
    {
        [ReadOnly] public NativeArray<T> Array;
        public NativeReference<T> Result;

        [BurstCompile]
        public void Execute()
        {
            if (Array.Length > 0)
                Result.Value = Array[Array.Length - 1];
        }
    }
}
