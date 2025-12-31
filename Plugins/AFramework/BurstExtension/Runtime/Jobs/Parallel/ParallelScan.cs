// ==========================================================
// 文件名：ParallelScan.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：并行扫描（前缀和），提供高性能的多线程前缀和算法
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// 顺序前缀和Job（小数据量使用）
    /// </summary>
    [BurstCompile]
    public struct SequentialPrefixSumJob<T, TOperator> : IJob
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        [ReadOnly] public NativeArray<T> Input;
        public NativeArray<T> Output;
        public TOperator Operator;
        public bool Inclusive;

        [BurstCompile]
        public void Execute()
        {
            if (Input.Length == 0) return;

            if (Inclusive)
            {
                Output[0] = Input[0];
                for (int i = 1; i < Input.Length; i++)
                {
                    Output[i] = Operator.Reduce(Output[i - 1], Input[i]);
                }
            }
            else
            {
                Output[0] = Operator.Identity;
                for (int i = 1; i < Input.Length; i++)
                {
                    Output[i] = Operator.Reduce(Output[i - 1], Input[i - 1]);
                }
            }
        }
    }

    /// <summary>
    /// 并行前缀和第一阶段Job（分块扫描）
    /// </summary>
    [BurstCompile]
    public struct ParallelScanPhase1Job<T, TOperator> : IJobParallelFor
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        [ReadOnly] public NativeArray<T> Input;
        [NativeDisableParallelForRestriction]
        public NativeArray<T> Output;
        [NativeDisableParallelForRestriction]
        public NativeArray<T> BlockSums;
        public int ChunkSize;
        public TOperator Operator;

        [BurstCompile]
        public void Execute(int blockIndex)
        {
            int start = blockIndex * ChunkSize;
            int end = math.min(start + ChunkSize, Input.Length);

            T sum = Operator.Identity;
            for (int i = start; i < end; i++)
            {
                sum = Operator.Reduce(sum, Input[i]);
                Output[i] = sum;
            }
            BlockSums[blockIndex] = sum;
        }
    }

    /// <summary>
    /// 并行前缀和第二阶段Job（块间扫描）
    /// </summary>
    [BurstCompile]
    public struct ParallelScanPhase2Job<T, TOperator> : IJob
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        public NativeArray<T> BlockSums;
        public TOperator Operator;

        [BurstCompile]
        public void Execute()
        {
            for (int i = 1; i < BlockSums.Length; i++)
            {
                BlockSums[i] = Operator.Reduce(BlockSums[i - 1], BlockSums[i]);
            }
        }
    }

    /// <summary>
    /// 并行前缀和第三阶段Job（应用块偏移）
    /// </summary>
    [BurstCompile]
    public struct ParallelScanPhase3Job<T, TOperator> : IJobParallelFor
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<T> Output;
        [ReadOnly] public NativeArray<T> BlockSums;
        public int ChunkSize;
        public TOperator Operator;

        [BurstCompile]
        public void Execute(int blockIndex)
        {
            if (blockIndex == 0) return;

            T offset = BlockSums[blockIndex - 1];
            int start = blockIndex * ChunkSize;
            int end = math.min(start + ChunkSize, Output.Length);

            for (int i = start; i < end; i++)
            {
                Output[i] = Operator.Reduce(offset, Output[i]);
            }
        }
    }

    /// <summary>
    /// 并行扫描工具类
    /// </summary>
    public static class ParallelScanUtility
    {
        /// <summary>
        /// 并行阈值
        /// </summary>
        public const int ParallelThreshold = 4096;

        /// <summary>
        /// 执行包含式前缀和
        /// </summary>
        public static JobHandle InclusiveScan<T, TOperator>(NativeArray<T> input, NativeArray<T> output,
            TOperator op, Allocator allocator = Allocator.TempJob, JobHandle dependency = default)
            where T : unmanaged
            where TOperator : struct, IReduceOperator<T>
        {
            if (input.Length < ParallelThreshold)
            {
                var seqJob = new SequentialPrefixSumJob<T, TOperator>
                {
                    Input = input,
                    Output = output,
                    Operator = op,
                    Inclusive = true
                };
                return seqJob.Schedule(dependency);
            }

            return ExecuteParallelScan(input, output, op, allocator, dependency);
        }

        /// <summary>
        /// 执行排除式前缀和
        /// </summary>
        public static JobHandle ExclusiveScan<T, TOperator>(NativeArray<T> input, NativeArray<T> output,
            TOperator op, Allocator allocator = Allocator.TempJob, JobHandle dependency = default)
            where T : unmanaged
            where TOperator : struct, IReduceOperator<T>
        {
            var seqJob = new SequentialPrefixSumJob<T, TOperator>
            {
                Input = input,
                Output = output,
                Operator = op,
                Inclusive = false
            };
            return seqJob.Schedule(dependency);
        }

        private static JobHandle ExecuteParallelScan<T, TOperator>(NativeArray<T> input,
            NativeArray<T> output, TOperator op, Allocator allocator, JobHandle dependency)
            where T : unmanaged
            where TOperator : struct, IReduceOperator<T>
        {
            int workerCount = BurstRuntime.GetWorkerThreadCount();
            int chunkSize = (input.Length + workerCount - 1) / workerCount;
            int blockCount = (input.Length + chunkSize - 1) / chunkSize;

            var blockSums = new NativeArray<T>(blockCount, allocator);

            var phase1 = new ParallelScanPhase1Job<T, TOperator>
            {
                Input = input, Output = output, BlockSums = blockSums,
                ChunkSize = chunkSize, Operator = op
            };

            var phase2 = new ParallelScanPhase2Job<T, TOperator>
            {
                BlockSums = blockSums, Operator = op
            };

            var phase3 = new ParallelScanPhase3Job<T, TOperator>
            {
                Output = output, BlockSums = blockSums,
                ChunkSize = chunkSize, Operator = op
            };

            var handle = phase1.Schedule(blockCount, 1, dependency);
            handle = phase2.Schedule(handle);
            handle = phase3.Schedule(blockCount, 1, handle);
            handle = blockSums.Dispose(handle);

            return handle;
        }
    }
}
