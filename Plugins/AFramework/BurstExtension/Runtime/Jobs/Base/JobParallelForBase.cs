// ==========================================================
// 文件名：JobParallelForBase.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：IJobParallelFor基类封装，提供并行迭代Job的统一接口和批处理优化
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// IJobParallelFor扩展接口
    /// 提供并行迭代Job的便捷调度方法
    /// </summary>
    public interface IJobParallelForBase
    {
        /// <summary>
        /// 调度并行执行的Job
        /// </summary>
        /// <param name="arrayLength">迭代次数</param>
        /// <param name="innerloopBatchCount">内循环批处理大小</param>
        /// <param name="dependency">依赖的JobHandle</param>
        /// <returns>当前Job的JobHandle</returns>
        JobHandle Schedule(int arrayLength, int innerloopBatchCount, JobHandle dependency = default);

        /// <summary>
        /// 立即执行Job
        /// </summary>
        /// <param name="arrayLength">迭代次数</param>
        void Run(int arrayLength);
    }

    /// <summary>
    /// IJobParallelFor调度辅助类
    /// </summary>
    public static class JobParallelForScheduleHelper
    {
        /// <summary>
        /// 默认批处理大小
        /// </summary>
        public const int DefaultBatchSize = 64;

        /// <summary>
        /// 调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, int arrayLength, int innerloopBatchCount,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            return job.Schedule(arrayLength, innerloopBatchCount, dependency);
        }

        /// <summary>
        /// 使用默认批处理大小调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, int arrayLength, JobHandle dependency = default)
            where T : struct, IJobParallelFor
        {
            return job.Schedule(arrayLength, DefaultBatchSize, dependency);
        }

        /// <summary>
        /// 使用自动计算的批处理大小调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleAuto<T>(ref T job, int arrayLength, int elementSize = 4,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            int batchSize = BurstRuntime.CalculateBatchSize(arrayLength, elementSize);
            return job.Schedule(arrayLength, batchSize, dependency);
        }

        /// <summary>
        /// 立即执行IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(ref T job, int arrayLength) where T : struct, IJobParallelFor
        {
            job.Run(arrayLength);
        }

        /// <summary>
        /// 调度并立即完成IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScheduleAndComplete<T>(ref T job, int arrayLength, int innerloopBatchCount,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            job.Schedule(arrayLength, innerloopBatchCount, dependency).Complete();
        }

        /// <summary>
        /// 根据数据量决定是否使用并行调度
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleSmart<T>(ref T job, int arrayLength, int threshold = 1000,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            if (BurstRuntime.ShouldUseParallel(arrayLength, threshold))
            {
                int batchSize = BurstRuntime.CalculateBatchSize(arrayLength);
                return job.Schedule(arrayLength, batchSize, dependency);
            }
            
            // 数据量小时直接在主线程执行
            job.Run(arrayLength);
            return dependency;
        }
    }
}
