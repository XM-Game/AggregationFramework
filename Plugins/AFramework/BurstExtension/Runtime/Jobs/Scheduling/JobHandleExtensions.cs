// ==========================================================
// 文件名：JobHandleExtensions.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：JobHandle扩展方法，提供链式调用和便捷操作
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// JobHandle扩展方法
    /// </summary>
    public static class JobHandleExtensions
    {
        #region 基础扩展

        /// <summary>
        /// 等待Job完成并返回自身（用于链式调用）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CompleteAndReturn(this JobHandle handle)
        {
            handle.Complete();
            return handle;
        }

        /// <summary>
        /// 检查Job是否已完成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDone(this JobHandle handle)
        {
            return handle.IsCompleted;
        }

        /// <summary>
        /// 与另一个JobHandle组合
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineWith(this JobHandle handle, JobHandle other)
        {
            return JobHandle.CombineDependencies(handle, other);
        }

        /// <summary>
        /// 与多个JobHandle组合
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineWith(this JobHandle handle, JobHandle other1, JobHandle other2)
        {
            return JobHandle.CombineDependencies(handle, other1, other2);
        }

        /// <summary>
        /// 与NativeArray组合的JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineWith(this JobHandle handle, NativeArray<JobHandle> handles)
        {
            return JobHandle.CombineDependencies(handles);
        }

        #endregion

        #region 链式调度扩展

        /// <summary>
        /// 链式调度IJob
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Then<T>(this JobHandle handle, ref T job) where T : struct, IJob
        {
            return job.Schedule(handle);
        }

        /// <summary>
        /// 链式调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ThenFor<T>(this JobHandle handle, ref T job, int arrayLength) 
            where T : struct, IJobFor
        {
            return job.Schedule(arrayLength, handle);
        }

        /// <summary>
        /// 链式调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ThenParallel<T>(this JobHandle handle, ref T job, int arrayLength, 
            int batchSize = 64) where T : struct, IJobParallelFor
        {
            return job.Schedule(arrayLength, batchSize, handle);
        }

        /// <summary>
        /// 链式调度IJobFor（并行）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ThenForParallel<T>(this JobHandle handle, ref T job, int arrayLength,
            int batchSize = 64) where T : struct, IJobFor
        {
            return job.ScheduleParallel(arrayLength, batchSize, handle);
        }

        #endregion

        #region 条件调度

        /// <summary>
        /// 条件调度Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ThenIf<T>(this JobHandle handle, bool condition, ref T job) 
            where T : struct, IJob
        {
            return condition ? job.Schedule(handle) : handle;
        }

        /// <summary>
        /// 条件调度并行Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ThenParallelIf<T>(this JobHandle handle, bool condition, ref T job,
            int arrayLength, int batchSize = 64) where T : struct, IJobParallelFor
        {
            return condition ? job.Schedule(arrayLength, batchSize, handle) : handle;
        }

        #endregion
    }
}
