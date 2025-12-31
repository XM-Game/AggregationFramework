// ==========================================================
// 文件名：JobScheduler.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：Job调度器，提供统一的Job调度管理和依赖处理
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Job调度器
    /// 提供统一的Job调度管理、依赖处理和批量调度功能
    /// </summary>
    public static class JobScheduler
    {
        #region 单Job调度

        /// <summary>
        /// 调度IJob
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, JobHandle dependency = default) 
            where T : struct, IJob
        {
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleFor<T>(ref T job, int length, JobHandle dependency = default)
            where T : struct, IJobFor
        {
            return job.Schedule(length, dependency);
        }

        /// <summary>
        /// 调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleParallel<T>(ref T job, int length, int batchSize = 64,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            return job.Schedule(length, batchSize, dependency);
        }

        /// <summary>
        /// 智能调度IJobParallelFor（根据数据量自动选择策略）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleSmart<T>(ref T job, int length, int threshold = 1000,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            if (length < threshold)
            {
                job.Run(length);
                return dependency;
            }
            
            int batchSize = BurstRuntime.CalculateBatchSize(length);
            return job.Schedule(length, batchSize, dependency);
        }

        #endregion

        #region 立即执行

        /// <summary>
        /// 立即执行IJob
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(ref T job) where T : struct, IJob
        {
            job.Run();
        }

        /// <summary>
        /// 立即执行IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunFor<T>(ref T job, int length) where T : struct, IJobFor
        {
            job.Run(length);
        }

        /// <summary>
        /// 立即执行IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunParallel<T>(ref T job, int length) where T : struct, IJobParallelFor
        {
            job.Run(length);
        }

        #endregion

        #region 调度并完成

        /// <summary>
        /// 调度并等待IJob完成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScheduleAndComplete<T>(ref T job, JobHandle dependency = default)
            where T : struct, IJob
        {
            job.Schedule(dependency).Complete();
        }

        /// <summary>
        /// 调度并等待IJobParallelFor完成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScheduleParallelAndComplete<T>(ref T job, int length, int batchSize = 64,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            job.Schedule(length, batchSize, dependency).Complete();
        }

        #endregion

        #region 依赖管理

        /// <summary>
        /// 组合两个依赖
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Combine(JobHandle a, JobHandle b)
        {
            return JobHandle.CombineDependencies(a, b);
        }

        /// <summary>
        /// 组合三个依赖
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Combine(JobHandle a, JobHandle b, JobHandle c)
        {
            return JobHandle.CombineDependencies(a, b, c);
        }

        /// <summary>
        /// 组合多个依赖
        /// </summary>
        public static JobHandle CombineAll(params JobHandle[] handles)
        {
            if (handles == null || handles.Length == 0)
                return default;
            if (handles.Length == 1)
                return handles[0];
            if (handles.Length == 2)
                return JobHandle.CombineDependencies(handles[0], handles[1]);
            if (handles.Length == 3)
                return JobHandle.CombineDependencies(handles[0], handles[1], handles[2]);

            using var nativeHandles = new NativeArray<JobHandle>(handles, Allocator.Temp);
            return JobHandle.CombineDependencies(nativeHandles);
        }

        /// <summary>
        /// 完成所有Job
        /// </summary>
        public static void CompleteAll(params JobHandle[] handles)
        {
            foreach (var handle in handles)
            {
                handle.Complete();
            }
        }

        #endregion
    }
}
