// ==========================================================
// 文件名：ClearJob.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：数据清除Job，提供高性能的NativeArray数据清零功能
// ==========================================================

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// 数据清除Job（并行版本）
    /// 将数组元素设置为默认值
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    [BurstCompile]
    public struct ClearJob<T> : IJobParallelFor where T : struct
    {
        /// <summary>目标数据</summary>
        [WriteOnly] public NativeArray<T> Data;

        [BurstCompile]
        public void Execute(int index)
        {
            Data[index] = default;
        }
    }

    /// <summary>
    /// 内存清零Job（使用memset）
    /// </summary>
    [BurstCompile]
    public struct MemClearJob : IJob
    {
        /// <summary>目标数据指针</summary>
        [NativeDisableUnsafePtrRestriction]
        public unsafe void* Ptr;
        
        /// <summary>字节大小</summary>
        public long Size;

        [BurstCompile]
        public unsafe void Execute()
        {
            UnsafeUtility.MemClear(Ptr, Size);
        }
    }

    /// <summary>
    /// 清除Job工具类
    /// </summary>
    public static class ClearJobUtility
    {
        /// <summary>
        /// 创建并调度清除Job
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle Clear<T>(NativeArray<T> data, JobHandle dependency = default) 
            where T : struct
        {
            var job = new ClearJob<T> { Data = data };
            int batchSize = BurstRuntime.CalculateBatchSize(data.Length, UnsafeUtility.SizeOf<T>());
            return job.Schedule(data.Length, batchSize, dependency);
        }

        /// <summary>
        /// 使用memset清除（更快但仅适用于可清零类型）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe JobHandle MemClear<T>(NativeArray<T> data, JobHandle dependency = default)
            where T : struct
        {
            var job = new MemClearJob
            {
                Ptr = data.GetUnsafePtr(),
                Size = data.Length * UnsafeUtility.SizeOf<T>()
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// 同步清除（立即执行）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ClearImmediate<T>(NativeArray<T> data) where T : struct
        {
            UnsafeUtility.MemClear(data.GetUnsafePtr(), data.Length * UnsafeUtility.SizeOf<T>());
        }
    }
}
