// ==========================================================
// 文件名：JobForBase.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：IJobFor基类封装，提供顺序迭代Job的统一接口
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// IJobFor扩展接口
    /// 提供顺序迭代Job的便捷调度方法
    /// </summary>
    public interface IJobForBase
    {
        /// <summary>
        /// 调度顺序执行的Job
        /// </summary>
        /// <param name="arrayLength">迭代次数</param>
        /// <param name="dependency">依赖的JobHandle</param>
        /// <returns>当前Job的JobHandle</returns>
        JobHandle Schedule(int arrayLength, JobHandle dependency = default);

        /// <summary>
        /// 立即执行Job
        /// </summary>
        /// <param name="arrayLength">迭代次数</param>
        void Run(int arrayLength);
    }

    /// <summary>
    /// IJobFor调度辅助类
    /// </summary>
    public static class JobForScheduleHelper
    {
        /// <summary>
        /// 调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, int arrayLength, JobHandle dependency = default)
            where T : struct, IJobFor
        {
            return job.Schedule(arrayLength, dependency);
        }

        /// <summary>
        /// 立即执行IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(ref T job, int arrayLength) where T : struct, IJobFor
        {
            job.Run(arrayLength);
        }

        /// <summary>
        /// 调度并立即完成IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScheduleAndComplete<T>(ref T job, int arrayLength, JobHandle dependency = default)
            where T : struct, IJobFor
        {
            job.Schedule(arrayLength, dependency).Complete();
        }

        /// <summary>
        /// 使用并行调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleParallel<T>(ref T job, int arrayLength, int innerloopBatchCount, 
            JobHandle dependency = default) where T : struct, IJobFor
        {
            return job.ScheduleParallel(arrayLength, innerloopBatchCount, dependency);
        }

        /// <summary>
        /// 使用自动批处理大小并行调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleParallelAuto<T>(ref T job, int arrayLength, JobHandle dependency = default)
            where T : struct, IJobFor
        {
            int batchSize = BurstRuntime.CalculateBatchSize(arrayLength);
            return job.ScheduleParallel(arrayLength, batchSize, dependency);
        }
    }
}
