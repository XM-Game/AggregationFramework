// ==========================================================
// 文件名：ParallelReduce.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：并行归约，提供高性能的多线程归约算法
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AFramework.Burst
{
    /// <summary>
    /// 归约操作接口
    /// </summary>
    public interface IReduceOperator<T> where T : unmanaged
    {
        /// <summary>获取单位元</summary>
        T Identity { get; }
        
        /// <summary>归约操作</summary>
        T Reduce(T a, T b);
    }

    /// <summary>
    /// 并行归约第一阶段Job（分块归约）
    /// </summary>
    [BurstCompile]
    public struct ParallelReduceJob<T, TOperator> : IJobParallelFor
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        [ReadOnly] public NativeArray<T> Input;
        [NativeDisableParallelForRestriction]
        public NativeArray<T> PartialResults;
        public int ChunkSize;
        public TOperator Operator;

        [BurstCompile]
        public void Execute(int chunkIndex)
        {
            int start = chunkIndex * ChunkSize;
            int end = math.min(start + ChunkSize, Input.Length);

            T result = Operator.Identity;
            for (int i = start; i < end; i++)
            {
                result = Operator.Reduce(result, Input[i]);
            }
            PartialResults[chunkIndex] = result;
        }
    }

    /// <summary>
    /// 归约最终合并Job
    /// </summary>
    [BurstCompile]
    public struct ReduceFinalizeJob<T, TOperator> : IJob
        where T : unmanaged
        where TOperator : struct, IReduceOperator<T>
    {
        [ReadOnly] public NativeArray<T> PartialResults;
        public NativeReference<T> FinalResult;
        public TOperator Operator;

        [BurstCompile]
        public void Execute()
        {
            T result = Operator.Identity;
            for (int i = 0; i < PartialResults.Length; i++)
            {
                result = Operator.Reduce(result, PartialResults[i]);
            }
            FinalResult.Value = result;
        }
    }

    #region 常用归约操作符

    /// <summary>
    /// 求和操作符（int）
    /// </summary>
    [BurstCompile]
    public struct SumIntReduceOperator : IReduceOperator<int>
    {
        public int Identity => 0;

        [BurstCompile]
        public int Reduce(int a, int b) => a + b;
    }

    /// <summary>
    /// 求和操作符（float）
    /// </summary>
    [BurstCompile]
    public struct SumFloatReduceOperator : IReduceOperator<float>
    {
        public float Identity => 0f;

        [BurstCompile]
        public float Reduce(float a, float b) => a + b;
    }

    /// <summary>
    /// 求最大值操作符（int）
    /// </summary>
    [BurstCompile]
    public struct MaxIntReduceOperator : IReduceOperator<int>
    {
        public int Identity => int.MinValue;

        [BurstCompile]
        public int Reduce(int a, int b) => math.max(a, b);
    }

    /// <summary>
    /// 求最小值操作符（int）
    /// </summary>
    [BurstCompile]
    public struct MinIntReduceOperator : IReduceOperator<int>
    {
        public int Identity => int.MaxValue;

        [BurstCompile]
        public int Reduce(int a, int b) => math.min(a, b);
    }

    /// <summary>
    /// 求最大值操作符（float）
    /// </summary>
    [BurstCompile]
    public struct MaxFloatReduceOperator : IReduceOperator<float>
    {
        public float Identity => float.MinValue;

        [BurstCompile]
        public float Reduce(float a, float b) => math.max(a, b);
    }

    /// <summary>
    /// 求最小值操作符（float）
    /// </summary>
    [BurstCompile]
    public struct MinFloatReduceOperator : IReduceOperator<float>
    {
        public float Identity => float.MaxValue;

        [BurstCompile]
        public float Reduce(float a, float b) => math.min(a, b);
    }

    #endregion

    /// <summary>
    /// 并行归约工具类
    /// </summary>
    public static class ParallelReduceUtility
    {
        /// <summary>
        /// 执行并行归约
        /// </summary>
        public static JobHandle Reduce<T, TOperator>(NativeArray<T> input, NativeReference<T> result,
            TOperator op, Allocator allocator = Allocator.TempJob, JobHandle dependency = default)
            where T : unmanaged
            where TOperator : struct, IReduceOperator<T>
        {
            int workerCount = BurstRuntime.GetWorkerThreadCount();
            int chunkSize = (input.Length + workerCount - 1) / workerCount;
            int chunkCount = (input.Length + chunkSize - 1) / chunkSize;

            var partialResults = new NativeArray<T>(chunkCount, allocator);

            var parallelJob = new ParallelReduceJob<T, TOperator>
            {
                Input = input,
                PartialResults = partialResults,
                ChunkSize = chunkSize,
                Operator = op
            };

            var finalizeJob = new ReduceFinalizeJob<T, TOperator>
            {
                PartialResults = partialResults,
                FinalResult = result,
                Operator = op
            };

            var handle = parallelJob.Schedule(chunkCount, 1, dependency);
            handle = finalizeJob.Schedule(handle);
            handle = partialResults.Dispose(handle);

            return handle;
        }
    }
}
