// ==========================================================
// 文件名：JobHelpers.cs
// 命名空间：AFramework.Burst.Internal
// 创建时间：2026-01-01
// 功能描述：Job辅助类，提供内部使用的Job系统工具方法
// 依赖：Unity.Burst, Unity.Jobs, Unity.Collections
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AFramework.Burst.Internal
{
    /// <summary>
    /// Job辅助类
    /// 提供内部使用的Job系统工具方法
    /// </summary>
    [BurstCompile]
    internal static class JobHelpers
    {
        #region 批次大小计算

        /// <summary>
        /// 计算最优批次大小
        /// </summary>
        /// <param name="totalCount">总元素数量</param>
        /// <param name="minBatchSize">最小批次大小</param>
        /// <param name="maxBatchSize">最大批次大小</param>
        /// <returns>建议的批次大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateOptimalBatchSize(int totalCount, int minBatchSize = 32, int maxBatchSize = 1024)
        {
            if (totalCount <= 0)
                return minBatchSize;

            int processorCount = Environment.ProcessorCount;
            int idealBatchSize = totalCount / (processorCount * 4);
            
            return math.clamp(idealBatchSize, minBatchSize, maxBatchSize);
        }

        /// <summary>
        /// 计算基于工作负载的批次大小
        /// </summary>
        /// <param name="totalCount">总元素数量</param>
        /// <param name="workloadPerItem">每个元素的工作负载（相对值，1为基准）</param>
        /// <returns>建议的批次大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateBatchSizeByWorkload(int totalCount, float workloadPerItem)
        {
            // 基准：workload=1时，批次大小为64
            int baseBatchSize = 64;
            int adjustedBatchSize = (int)(baseBatchSize / math.max(workloadPerItem, 0.1f));
            
            return math.clamp(adjustedBatchSize, 1, totalCount);
        }

        /// <summary>
        /// 计算内存密集型Job的批次大小
        /// </summary>
        /// <param name="totalCount">总元素数量</param>
        /// <param name="elementSizeBytes">每个元素的大小（字节）</param>
        /// <returns>建议的批次大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateMemoryBoundBatchSize(int totalCount, int elementSizeBytes)
        {
            // 目标：每个批次处理约64KB数据（L1缓存友好）
            const int targetBatchBytes = 64 * 1024;
            int batchSize = targetBatchBytes / math.max(elementSizeBytes, 1);
            
            return math.clamp(batchSize, 1, totalCount);
        }

        #endregion

        #region 工作分配

        /// <summary>
        /// 计算工作范围
        /// </summary>
        /// <param name="workerIndex">工作线程索引</param>
        /// <param name="workerCount">工作线程总数</param>
        /// <param name="totalCount">总元素数量</param>
        /// <param name="startIndex">输出起始索引</param>
        /// <param name="count">输出元素数量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateWorkRange(int workerIndex, int workerCount, int totalCount, 
            out int startIndex, out int count)
        {
            int baseCount = totalCount / workerCount;
            int remainder = totalCount - baseCount * workerCount;
            
            if (workerIndex < remainder)
            {
                count = baseCount + 1;
                startIndex = workerIndex * count;
            }
            else
            {
                count = baseCount;
                startIndex = remainder * (baseCount + 1) + (workerIndex - remainder) * baseCount;
            }
        }

        /// <summary>
        /// 计算2D工作范围
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateWorkRange2D(int workerIndex, int workerCount, int width, int height,
            out int2 start, out int2 size)
        {
            int totalCount = width * height;
            CalculateWorkRange(workerIndex, workerCount, totalCount, out int startIndex, out int count);
            
            start = new int2(startIndex % width, startIndex / width);
            int endIndex = startIndex + count - 1;
            int2 end = new int2(endIndex % width, endIndex / width);
            size = end - start + 1;
        }

        #endregion

        #region Job链辅助

        /// <summary>
        /// 组合多个JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineHandles(JobHandle a, JobHandle b)
        {
            return JobHandle.CombineDependencies(a, b);
        }

        /// <summary>
        /// 组合三个JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle CombineHandles(JobHandle a, JobHandle b, JobHandle c)
        {
            return JobHandle.CombineDependencies(a, b, c);
        }

        /// <summary>
        /// 组合多个JobHandle
        /// </summary>
        public static JobHandle CombineHandles(NativeArray<JobHandle> handles)
        {
            return JobHandle.CombineDependencies(handles);
        }

        /// <summary>
        /// 创建空的JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle EmptyHandle()
        {
            return default;
        }

        #endregion

        #region 安全检查

        /// <summary>
        /// 检查NativeArray是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(NativeArray<T> array) where T : struct
        {
            return array.IsCreated && array.Length > 0;
        }

        /// <summary>
        /// 检查NativeList是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid<T>(NativeList<T> list) where T : unmanaged
        {
            return list.IsCreated;
        }

        /// <summary>
        /// 检查索引是否在有效范围内
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex(int index, int length)
        {
            return index >= 0 && index < length;
        }

        /// <summary>
        /// 检查范围是否有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidRange(int startIndex, int count, int length)
        {
            return startIndex >= 0 && count >= 0 && startIndex + count <= length;
        }

        #endregion

        #region 内存布局

        /// <summary>
        /// 计算对齐后的大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignSize(int size, int alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }

        /// <summary>
        /// 计算缓存行对齐的大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignToCacheLine(int size)
        {
            const int cacheLineSize = 64;
            return AlignSize(size, cacheLineSize);
        }

        /// <summary>
        /// 计算SIMD对齐的大小
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignToSimd(int size)
        {
            const int simdAlignment = 16;
            return AlignSize(size, simdAlignment);
        }

        #endregion

        #region 进度计算

        /// <summary>
        /// 计算进度百分比
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateProgress(int current, int total)
        {
            return total > 0 ? (float)current / total : 0f;
        }

        /// <summary>
        /// 计算预计剩余时间（毫秒）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double EstimateRemainingTime(int completed, int total, double elapsedMs)
        {
            if (completed <= 0 || total <= 0)
                return 0;
            
            double msPerItem = elapsedMs / completed;
            int remaining = total - completed;
            return msPerItem * remaining;
        }

        #endregion
    }
}
