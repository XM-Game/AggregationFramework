// ==========================================================
// 文件名：ParallelJobBase.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：并行Job基类，提供批处理大小优化和自动调度策略
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 并行Job配置
    /// </summary>
    public struct ParallelJobConfig
    {
        /// <summary>数据长度</summary>
        public int Length;
        
        /// <summary>批处理大小</summary>
        public int BatchSize;
        
        /// <summary>单个元素大小（字节）</summary>
        public int ElementSize;
        
        /// <summary>并行阈值</summary>
        public int ParallelThreshold;

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static ParallelJobConfig Create(int length, int elementSize = 4)
        {
            return new ParallelJobConfig
            {
                Length = length,
                BatchSize = BurstRuntime.CalculateBatchSize(length, elementSize),
                ElementSize = elementSize,
                ParallelThreshold = BurstRuntime.Config.ParallelThreshold
            };
        }

        /// <summary>
        /// 是否应该使用并行处理
        /// </summary>
        public readonly bool ShouldUseParallel => Length >= ParallelThreshold;
    }

    /// <summary>
    /// 并行Job调度策略
    /// </summary>
    public enum ParallelScheduleStrategy
    {
        /// <summary>始终并行</summary>
        AlwaysParallel,
        
        /// <summary>始终顺序</summary>
        AlwaysSequential,
        
        /// <summary>自动选择（基于数据量）</summary>
        Auto,
        
        /// <summary>立即执行（主线程）</summary>
        Immediate
    }

    /// <summary>
    /// 并行Job调度器
    /// 提供智能调度策略和批处理优化
    /// </summary>
    public static class ParallelJobScheduler
    {
        /// <summary>
        /// 使用指定策略调度IJobParallelFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Schedule<T>(ref T job, ParallelJobConfig config, 
            ParallelScheduleStrategy strategy = ParallelScheduleStrategy.Auto,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            switch (strategy)
            {
                case ParallelScheduleStrategy.AlwaysParallel:
                    return job.Schedule(config.Length, config.BatchSize, dependency);
                    
                case ParallelScheduleStrategy.AlwaysSequential:
                    return job.Schedule(config.Length, config.Length, dependency);
                    
                case ParallelScheduleStrategy.Immediate:
                    job.Run(config.Length);
                    return dependency;
                    
                case ParallelScheduleStrategy.Auto:
                default:
                    if (config.ShouldUseParallel)
                        return job.Schedule(config.Length, config.BatchSize, dependency);
                    job.Run(config.Length);
                    return dependency;
            }
        }

        /// <summary>
        /// 使用指定策略调度IJobFor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle ScheduleFor<T>(ref T job, ParallelJobConfig config,
            ParallelScheduleStrategy strategy = ParallelScheduleStrategy.Auto,
            JobHandle dependency = default) where T : struct, IJobFor
        {
            switch (strategy)
            {
                case ParallelScheduleStrategy.AlwaysParallel:
                    return job.ScheduleParallel(config.Length, config.BatchSize, dependency);
                    
                case ParallelScheduleStrategy.AlwaysSequential:
                    return job.Schedule(config.Length, dependency);
                    
                case ParallelScheduleStrategy.Immediate:
                    job.Run(config.Length);
                    return dependency;
                    
                case ParallelScheduleStrategy.Auto:
                default:
                    if (config.ShouldUseParallel)
                        return job.ScheduleParallel(config.Length, config.BatchSize, dependency);
                    job.Run(config.Length);
                    return dependency;
            }
        }
    }
}
