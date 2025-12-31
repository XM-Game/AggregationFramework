// ==========================================================
// 文件名：CopyJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：数据复制Job，提供高性能的NativeArray数据复制功能
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 数据复制Job（并行版本）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [BurstCompile]
    public struct CopyJob<T> : IJobParallelFor where T : struct
    {
        /// <summary>源数据（只读）</summary>
        [ReadOnly] public NativeArray<T> Source;
        
        /// <summary>目标数据（只写）</summary>
        [WriteOnly] public NativeArray<T> Destination;

        /// <summary>
        /// 执行复制
        /// </summary>
        [BurstCompile]
        public void Execute(int index)
        {
            Destination[index] = Source[index];
        }
    }

    /// <summary>
    /// 带偏移的数据复制Job
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [BurstCompile]
    public struct CopyWithOffsetJob<T> : IJobParallelFor where T : struct
    {
        /// <summary>源数据</summary>
        [ReadOnly] public NativeArray<T> Source;
        
        /// <summary>目标数据</summary>
        [NativeDisableParallelForRestriction]
        public NativeArray<T> Destination;
        
        /// <summary>源偏移</summary>
        public int SourceOffset;
        
        /// <summary>目标偏移</summary>
        public int DestinationOffset;

        [BurstCompile]
        public void Execute(int index)
        {
            Destination[DestinationOffset + index] = Source[SourceOffset + index];
        }
    }

    /// <summary>
    /// 复制Job工具类
    /// </summary>
    public static class CopyJobUtility
    {
        /// <summary>
        /// 创建并调度复制Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Copy<T>(NativeArray<T> source, NativeArray<T> destination,
            JobHandle dependency = default) where T : struct
        {
            var job = new CopyJob<T>
            {
                Source = source,
                Destination = destination
            };
            
            int batchSize = BurstRuntime.CalculateBatchSize(source.Length, UnsafeUtility.SizeOf<T>());
            return job.Schedule(source.Length, batchSize, dependency);
        }

        /// <summary>
        /// 创建并调度带偏移的复制Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CopyWithOffset<T>(NativeArray<T> source, NativeArray<T> destination,
            int sourceOffset, int destOffset, int count, JobHandle dependency = default) where T : struct
        {
            var job = new CopyWithOffsetJob<T>
            {
                Source = source,
                Destination = destination,
                SourceOffset = sourceOffset,
                DestinationOffset = destOffset
            };
            
            int batchSize = BurstRuntime.CalculateBatchSize(count, UnsafeUtility.SizeOf<T>());
            return job.Schedule(count, batchSize, dependency);
        }

        /// <summary>
        /// 同步复制（立即执行）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyImmediate<T>(NativeArray<T> source, NativeArray<T> destination) 
            where T : struct
        {
            NativeArray<T>.Copy(source, destination);
        }
    }
}
