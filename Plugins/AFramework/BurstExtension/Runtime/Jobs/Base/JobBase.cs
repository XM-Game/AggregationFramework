// ==========================================================
// 文件名：JobBase.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：IJob基类封装，提供Job执行的统一接口和便捷调度方法
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Job执行状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>未开始</summary>
        Pending,
        /// <summary>执行中</summary>
        Running,
        /// <summary>已完成</summary>
        Completed,
        /// <summary>已取消</summary>
        Cancelled
    }

    /// <summary>
    /// Job基类接口
    /// 定义所有Job的通用行为
    /// </summary>
    public interface IJobBase
    {
        /// <summary>
        /// 调度Job执行
        /// </summary>
        /// <param name="dependency">依赖的JobHandle</param>
        /// <returns>当前Job的JobHandle</returns>
        JobHandle Schedule(JobHandle dependency = default);

        /// <summary>
        /// 立即执行Job（阻塞主线程）
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Job执行结果包装器
    /// 提供Job执行状态跟踪和结果获取
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    public struct JobResult<TResult> where TResult : struct
    {
        /// <summary>Job句柄</summary>
        public JobHandle Handle;
        
        /// <summary>执行结果</summary>
        public TResult Result;
        
        /// <summary>是否已完成</summary>
        public bool IsCompleted => Handle.IsCompleted;

        /// <summary>
        /// 等待Job完成并获取结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Complete()
        {
            Handle.Complete();
            return Result;
        }
    }

    /// <summary>
    /// Job调度辅助类
    /// 提供Job调度的静态工具方法
    /// </summary>
    public static class JobScheduleHelper
    {
        /// <summary>
        /// 调度单个Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, JobHandle dependency = default) 
            where T : struct, IJob
        {
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 立即执行Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(ref T job) where T : struct, IJob
        {
            job.Run();
        }

        /// <summary>
        /// 调度并立即完成Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScheduleAndComplete<T>(ref T job, JobHandle dependency = default) 
            where T : struct, IJob
        {
            job.Schedule(dependency).Complete();
        }

        /// <summary>
        /// 组合多个JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineDependencies(JobHandle a, JobHandle b)
        {
            return JobHandle.CombineDependencies(a, b);
        }

        /// <summary>
        /// 组合多个JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineDependencies(JobHandle a, JobHandle b, JobHandle c)
        {
            return JobHandle.CombineDependencies(a, b, c);
        }
    }
}
