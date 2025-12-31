// ==========================================================
// 文件名：JobBatcher.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：Job批处理器，提供多Job批量调度和依赖链管理
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Job批处理器
    /// 用于管理多个Job的批量调度和依赖关系
    /// </summary>
    public struct JobBatcher : IDisposable
    {
        private NativeList<JobHandle> _handles;
        private JobHandle _combinedHandle;
        private bool _isDisposed;

        /// <summary>
        /// 当前累积的Job数量
        /// </summary>
        public int Count => _handles.IsCreated ? _handles.Length : 0;

        /// <summary>
        /// 获取组合后的JobHandle
        /// </summary>
        public JobHandle CombinedHandle => _combinedHandle;

        /// <summary>
        /// 创建Job批处理器
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="allocator">分配器类型</param>
        public JobBatcher(int initialCapacity = 8, Allocator allocator = Allocator.Temp)
        {
            _handles = new NativeList<JobHandle>(initialCapacity, allocator);
            _combinedHandle = default;
            _isDisposed = false;
        }

        /// <summary>
        /// 添加Job句柄
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(JobHandle handle)
        {
            _handles.Add(handle);
        }

        /// <summary>
        /// 调度IJob并添加到批处理
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JobHandle Schedule<T>(ref T job, JobHandle dependency = default) where T : struct, IJob
        {
            var handle = job.Schedule(dependency);
            _handles.Add(handle);
            return handle;
        }

        /// <summary>
        /// 调度IJobParallelFor并添加到批处理
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JobHandle ScheduleParallel<T>(ref T job, int length, int batchSize = 64,
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            var handle = job.Schedule(length, batchSize, dependency);
            _handles.Add(handle);
            return handle;
        }

        /// <summary>
        /// 调度IJobFor并添加到批处理
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JobHandle ScheduleFor<T>(ref T job, int length, JobHandle dependency = default)
            where T : struct, IJobFor
        {
            var handle = job.Schedule(length, dependency);
            _handles.Add(handle);
            return handle;
        }

        /// <summary>
        /// 组合所有已添加的Job句柄
        /// </summary>
        public JobHandle Combine()
        {
            if (_handles.Length == 0)
                return default;
            if (_handles.Length == 1)
                return _handles[0];

            _combinedHandle = JobHandle.CombineDependencies(_handles.AsArray());
            return _combinedHandle;
        }

        /// <summary>
        /// 等待所有Job完成
        /// </summary>
        public void Complete()
        {
            Combine().Complete();
        }

        /// <summary>
        /// 清空批处理器
        /// </summary>
        public void Clear()
        {
            _handles.Clear();
            _combinedHandle = default;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            
            if (_handles.IsCreated)
            {
                _handles.Dispose();
            }
            _isDisposed = true;
        }
    }
}
