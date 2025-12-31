// ==========================================================
// 文件名：DependencyGraph.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：依赖图管理，提供Job依赖关系的可视化管理和调度优化
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Job依赖节点
    /// </summary>
    public struct JobNode
    {
        /// <summary>节点ID</summary>
        public int Id;
        
        /// <summary>Job句柄</summary>
        public JobHandle Handle;
        
        /// <summary>是否已调度</summary>
        public bool IsScheduled;

        /// <summary>
        /// 创建节点
        /// </summary>
        public static JobNode Create(int id)
        {
            return new JobNode { Id = id, Handle = default, IsScheduled = false };
        }
    }

    /// <summary>
    /// 依赖图构建器
    /// 用于构建和管理Job之间的依赖关系
    /// </summary>
    public struct DependencyGraphBuilder : IDisposable
    {
        private NativeList<JobHandle> _nodes;
        private NativeHashMap<int, int> _nodeIndices;
        private int _nextId;
        private Allocator _allocator;

        /// <summary>
        /// 节点数量
        /// </summary>
        public int NodeCount => _nodes.IsCreated ? _nodes.Length : 0;

        /// <summary>
        /// 创建依赖图构建器
        /// </summary>
        public DependencyGraphBuilder(int initialCapacity = 16, Allocator allocator = Allocator.Temp)
        {
            _nodes = new NativeList<JobHandle>(initialCapacity, allocator);
            _nodeIndices = new NativeHashMap<int, int>(initialCapacity, allocator);
            _nextId = 0;
            _allocator = allocator;
        }

        /// <summary>
        /// 添加独立节点（无依赖）
        /// </summary>
        public int AddNode(JobHandle handle)
        {
            int id = _nextId++;
            int index = _nodes.Length;
            _nodes.Add(handle);
            _nodeIndices.Add(id, index);
            return id;
        }

        /// <summary>
        /// 调度IJob并添加为节点
        /// </summary>
        public int AddJob<T>(ref T job, JobHandle dependency = default) where T : struct, IJob
        {
            var handle = job.Schedule(dependency);
            return AddNode(handle);
        }

        /// <summary>
        /// 调度IJobParallelFor并添加为节点
        /// </summary>
        public int AddParallelJob<T>(ref T job, int length, int batchSize = 64, 
            JobHandle dependency = default) where T : struct, IJobParallelFor
        {
            var handle = job.Schedule(length, batchSize, dependency);
            return AddNode(handle);
        }

        /// <summary>
        /// 获取节点的JobHandle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JobHandle GetHandle(int nodeId)
        {
            if (_nodeIndices.TryGetValue(nodeId, out int index))
            {
                return _nodes[index];
            }
            return default;
        }

        /// <summary>
        /// 组合指定节点的依赖
        /// </summary>
        public JobHandle CombineNodes(params int[] nodeIds)
        {
            if (nodeIds == null || nodeIds.Length == 0)
                return default;

            using var handles = new NativeList<JobHandle>(nodeIds.Length, Allocator.Temp);
            foreach (int id in nodeIds)
            {
                if (_nodeIndices.TryGetValue(id, out int index))
                {
                    handles.Add(_nodes[index]);
                }
            }

            if (handles.Length == 0) return default;
            if (handles.Length == 1) return handles[0];
            
            return JobHandle.CombineDependencies(handles.AsArray());
        }

        /// <summary>
        /// 组合所有节点
        /// </summary>
        public JobHandle CombineAll()
        {
            if (_nodes.Length == 0) return default;
            if (_nodes.Length == 1) return _nodes[0];
            
            return JobHandle.CombineDependencies(_nodes.AsArray());
        }

        /// <summary>
        /// 等待所有节点完成
        /// </summary>
        public void CompleteAll()
        {
            CombineAll().Complete();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_nodes.IsCreated) _nodes.Dispose();
            if (_nodeIndices.IsCreated) _nodeIndices.Dispose();
        }
    }
}
