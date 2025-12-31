// ==========================================================
// 文件名：JobDebugger.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：Job调试器，提供Job系统的调试和诊断功能
// 依赖：Unity.Burst, Unity.Jobs, Unity.Collections
// ==========================================================

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Job调试器
    /// 提供Job系统的调试、诊断和性能分析功能
    /// </summary>
    [BurstCompile]
    public static class JobDebugger
    {
        #region Job执行跟踪

        /// <summary>
        /// Job执行信息
        /// </summary>
        public struct JobExecutionInfo
        {
            /// <summary>
            /// Job名称
            /// </summary>
            public FixedString64Bytes Name;

            /// <summary>
            /// 开始时间戳
            /// </summary>
            public long StartTicks;

            /// <summary>
            /// 结束时间戳
            /// </summary>
            public long EndTicks;

            /// <summary>
            /// 处理的元素数量
            /// </summary>
            public int ItemCount;

            /// <summary>
            /// 批次大小
            /// </summary>
            public int BatchSize;

            /// <summary>
            /// 执行时间（毫秒）
            /// </summary>
            public double ElapsedMs => BurstProfiler.TicksToMilliseconds(EndTicks - StartTicks);

            /// <summary>
            /// 每个元素的平均时间（微秒）
            /// </summary>
            public double MicrosecondsPerItem => ItemCount > 0 
                ? BurstProfiler.TicksToMicroseconds(EndTicks - StartTicks) / ItemCount 
                : 0;

            /// <summary>
            /// 吞吐量（元素/秒）
            /// </summary>
            public double ItemsPerSecond => ItemCount > 0 && ElapsedMs > 0 
                ? ItemCount / (ElapsedMs / 1000.0) 
                : 0;
        }

        #endregion

        #region Job包装器

        /// <summary>
        /// 带调试信息的Job包装器
        /// </summary>
        /// <typeparam name="T">Job类型</typeparam>
        public struct DebugJobWrapper<T> where T : struct, IJob
        {
            /// <summary>
            /// 原始Job
            /// </summary>
            public T Job;

            /// <summary>
            /// Job名称
            /// </summary>
            public FixedString64Bytes Name;

            /// <summary>
            /// 执行信息
            /// </summary>
            public JobExecutionInfo ExecutionInfo;

            /// <summary>
            /// 创建调试包装器
            /// </summary>
            public DebugJobWrapper(T job, in FixedString64Bytes name)
            {
                Job = job;
                Name = name;
                ExecutionInfo = default;
            }

            /// <summary>
            /// 调度Job并记录执行信息
            /// </summary>
            public JobHandle Schedule(JobHandle dependency = default)
            {
                ExecutionInfo.Name = Name;
                ExecutionInfo.StartTicks = BurstProfiler.GetTimestamp();
                
                var handle = Job.Schedule(dependency);
                
                // 注意：这里无法直接获取结束时间，需要在Complete后获取
                return handle;
            }

            /// <summary>
            /// 立即执行Job并记录执行信息
            /// </summary>
            public void Run()
            {
                ExecutionInfo.Name = Name;
                ExecutionInfo.StartTicks = BurstProfiler.GetTimestamp();
                
                Job.Run();
                
                ExecutionInfo.EndTicks = BurstProfiler.GetTimestamp();
                LogExecutionInfo(ExecutionInfo);
            }
        }

        /// <summary>
        /// 带调试信息的ParallelFor Job包装器
        /// </summary>
        public struct DebugParallelForWrapper<T> where T : struct, IJobParallelFor
        {
            /// <summary>
            /// 原始Job
            /// </summary>
            public T Job;

            /// <summary>
            /// Job名称
            /// </summary>
            public FixedString64Bytes Name;

            /// <summary>
            /// 执行信息
            /// </summary>
            public JobExecutionInfo ExecutionInfo;

            /// <summary>
            /// 创建调试包装器
            /// </summary>
            public DebugParallelForWrapper(T job, in FixedString64Bytes name)
            {
                Job = job;
                Name = name;
                ExecutionInfo = default;
            }

            /// <summary>
            /// 调度Job
            /// </summary>
            public JobHandle Schedule(int arrayLength, int innerloopBatchCount, JobHandle dependency = default)
            {
                ExecutionInfo.Name = Name;
                ExecutionInfo.ItemCount = arrayLength;
                ExecutionInfo.BatchSize = innerloopBatchCount;
                ExecutionInfo.StartTicks = BurstProfiler.GetTimestamp();
                
                return Job.Schedule(arrayLength, innerloopBatchCount, dependency);
            }

            /// <summary>
            /// 立即执行Job
            /// </summary>
            public void Run(int arrayLength)
            {
                ExecutionInfo.Name = Name;
                ExecutionInfo.ItemCount = arrayLength;
                ExecutionInfo.BatchSize = arrayLength;
                ExecutionInfo.StartTicks = BurstProfiler.GetTimestamp();
                
                Job.Run(arrayLength);
                
                ExecutionInfo.EndTicks = BurstProfiler.GetTimestamp();
                LogExecutionInfo(ExecutionInfo);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 创建带调试的Job包装器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DebugJobWrapper<T> WithDebug<T>(this T job, in FixedString64Bytes name) where T : struct, IJob
        {
            return new DebugJobWrapper<T>(job, name);
        }

        /// <summary>
        /// 创建带调试的ParallelFor Job包装器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DebugParallelForWrapper<T> WithDebugParallel<T>(this T job, in FixedString64Bytes name) where T : struct, IJobParallelFor
        {
            return new DebugParallelForWrapper<T>(job, name);
        }

        /// <summary>
        /// 完成Job并记录时间
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void CompleteAndLog(this JobHandle handle, in FixedString64Bytes name, long startTicks)
        {
            handle.Complete();
            long endTicks = BurstProfiler.GetTimestamp();
            double ms = BurstProfiler.TicksToMilliseconds(endTicks - startTicks);
            LogJobComplete(name, ms);
        }

        #endregion

        #region 日志输出

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        private static void LogExecutionInfo(in JobExecutionInfo info)
        {
            if (info.ItemCount > 0)
            {
                UnityEngine.Debug.Log(
                    $"[JobDebugger] {info.Name}: {info.ElapsedMs:F4}ms, " +
                    $"{info.ItemCount} items ({info.MicrosecondsPerItem:F2}μs/item, " +
                    $"{info.ItemsPerSecond:F0} items/sec), batch={info.BatchSize}");
            }
            else
            {
                UnityEngine.Debug.Log($"[JobDebugger] {info.Name}: {info.ElapsedMs:F4}ms");
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        private static void LogJobComplete(in FixedString64Bytes name, double ms)
        {
            UnityEngine.Debug.Log($"[JobDebugger] {name} completed: {ms:F4}ms");
        }

        /// <summary>
        /// 输出Job调度信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogSchedule(in FixedString64Bytes name, int itemCount = 0, int batchSize = 0)
        {
            if (itemCount > 0)
            {
                UnityEngine.Debug.Log($"[JobDebugger] Scheduling {name}: {itemCount} items, batch={batchSize}");
            }
            else
            {
                UnityEngine.Debug.Log($"[JobDebugger] Scheduling {name}");
            }
        }

        /// <summary>
        /// 输出Job依赖信息
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void LogDependency(in FixedString64Bytes jobName, in FixedString64Bytes dependencyName)
        {
            UnityEngine.Debug.Log($"[JobDebugger] {jobName} depends on {dependencyName}");
        }

        #endregion

        #region 验证工具

        /// <summary>
        /// 验证NativeArray在Job执行前的状态
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public static void ValidateInputArray<T>(NativeArray<T> array, in FixedString64Bytes name) where T : struct
        {
            ValidateArrayInternal(array, name, true);
        }

        /// <summary>
        /// 验证NativeArray在Job执行后的状态
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public static void ValidateOutputArray<T>(NativeArray<T> array, in FixedString64Bytes name) where T : struct
        {
            ValidateArrayInternal(array, name, false);
        }

        [BurstDiscard]
        private static void ValidateArrayInternal<T>(NativeArray<T> array, in FixedString64Bytes name, bool isInput) where T : struct
        {
            string type = isInput ? "Input" : "Output";
            
            if (!array.IsCreated)
            {
                UnityEngine.Debug.LogError($"[JobDebugger] {type} array '{name}' is not created!");
                return;
            }

            if (array.Length == 0)
            {
                UnityEngine.Debug.LogWarning($"[JobDebugger] {type} array '{name}' is empty");
            }
        }

        /// <summary>
        /// 验证批次大小是否合理
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [BurstDiscard]
        public static void ValidateBatchSize(int itemCount, int batchSize, in FixedString64Bytes jobName)
        {
            if (batchSize <= 0)
            {
                UnityEngine.Debug.LogError($"[JobDebugger] {jobName}: Invalid batch size {batchSize}");
                return;
            }

            if (batchSize > itemCount)
            {
                UnityEngine.Debug.LogWarning($"[JobDebugger] {jobName}: Batch size ({batchSize}) > item count ({itemCount})");
            }

            // 建议的批次大小范围
            int recommendedMin = Math.Max(1, itemCount / (Environment.ProcessorCount * 4));
            int recommendedMax = Math.Max(64, itemCount / Environment.ProcessorCount);

            if (batchSize < recommendedMin || batchSize > recommendedMax)
            {
                UnityEngine.Debug.Log(
                    $"[JobDebugger] {jobName}: Batch size {batchSize} may not be optimal. " +
                    $"Recommended range: [{recommendedMin}, {recommendedMax}] for {itemCount} items");
            }
        }

        #endregion
    }
}
