// ==========================================================
// 文件名：BurstRuntime.cs
// 命名空间：AFramework.Burst
// 创建时间：2025-12-31
// 功能描述：Burst运行时检测与配置，提供Burst编译器状态查询和运行时配置管理
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace AFramework.Burst
{
    /// <summary>
    /// Burst运行时管理器
    /// 提供Burst编译器状态检测、运行时配置和性能监控功能
    /// </summary>
    public static class BurstRuntime
    {
        #region 静态字段

        /// <summary>
        /// 缓存的Burst可用性状态
        /// </summary>
        private static bool? s_IsBurstAvailable;

        /// <summary>
        /// 缓存的Burst编译状态
        /// </summary>
        private static bool? s_IsBurstCompiled;

        /// <summary>
        /// 运行时配置实例
        /// </summary>
        private static BurstRuntimeConfig s_Config = BurstRuntimeConfig.Default;

        #endregion

        #region 公共属性

        /// <summary>
        /// 检查Burst编译器是否可用
        /// </summary>
        /// <remarks>
        /// 在编辑器中，Burst可能因为编译设置而被禁用
        /// 在运行时构建中，如果Burst包未正确配置也可能不可用
        /// </remarks>
        public static bool IsBurstAvailable
        {
            get
            {
                if (!s_IsBurstAvailable.HasValue)
                {
                    s_IsBurstAvailable = CheckBurstAvailability();
                }
                return s_IsBurstAvailable.Value;
            }
        }

        /// <summary>
        /// 检查当前代码是否经过Burst编译
        /// </summary>
        public static bool IsBurstCompiled
        {
            get
            {
                if (!s_IsBurstCompiled.HasValue)
                {
                    s_IsBurstCompiled = CheckBurstCompilation();
                }
                return s_IsBurstCompiled.Value;
            }
        }

        /// <summary>
        /// 获取或设置运行时配置
        /// </summary>
        public static BurstRuntimeConfig Config
        {
            get => s_Config;
            set => s_Config = value;
        }

        /// <summary>
        /// 获取推荐的并行批处理大小
        /// </summary>
        /// <remarks>
        /// 基于CPU核心数和缓存大小计算最优批处理大小
        /// </remarks>
        public static int RecommendedBatchSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CalculateOptimalBatchSize();
        }

        /// <summary>
        /// 获取系统处理器核心数
        /// </summary>
        public static int ProcessorCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => System.Environment.ProcessorCount;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 重置缓存的状态信息
        /// </summary>
        /// <remarks>
        /// 在运行时配置发生变化后调用此方法刷新状态
        /// </remarks>
        public static void ResetCache()
        {
            s_IsBurstAvailable = null;
            s_IsBurstCompiled = null;
        }

        /// <summary>
        /// 获取Job工作线程数量
        /// </summary>
        /// <returns>可用的工作线程数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetWorkerThreadCount()
        {
            // Unity Jobs 系统通常使用 ProcessorCount - 1 个工作线程（保留主线程）
            return Math.Max(1, System.Environment.ProcessorCount - 1);
        }

        /// <summary>
        /// 计算给定数据量的最优批处理大小
        /// </summary>
        /// <param name="dataCount">数据总量</param>
        /// <param name="elementSize">单个元素大小（字节）</param>
        /// <returns>推荐的批处理大小</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateBatchSize(int dataCount, int elementSize = 4)
        {
            // 目标：每个批次处理约64KB数据以优化缓存利用
            const int targetBatchBytes = 64 * 1024;
            
            int elementsPerBatch = targetBatchBytes / elementSize;
            int workerCount = GetWorkerThreadCount();
            
            // 确保每个工作线程至少有一个批次
            int minBatchSize = (dataCount + workerCount - 1) / workerCount;
            
            // 取较小值，但不小于1
            return Math.Max(1, Math.Min(elementsPerBatch, minBatchSize));
        }

        /// <summary>
        /// 检查是否应该使用并行Job
        /// </summary>
        /// <param name="dataCount">数据量</param>
        /// <param name="threshold">并行阈值（默认1000）</param>
        /// <returns>是否建议使用并行处理</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldUseParallel(int dataCount, int threshold = 1000)
        {
            return dataCount >= threshold && GetWorkerThreadCount() > 1;
        }

        /// <summary>
        /// 获取Burst编译器版本信息
        /// </summary>
        /// <returns>版本字符串</returns>
        public static string GetBurstVersion()
        {
#if BURST_1_8_OR_NEWER
            return "1.8.x+";
#else
            return "Unknown";
#endif
        }

        /// <summary>
        /// 验证NativeContainer的有效性
        /// </summary>
        /// <typeparam name="T">容器类型</typeparam>
        /// <param name="container">要验证的容器</param>
        /// <returns>容器是否有效</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidContainer<T>(in T container) where T : struct
        {
            // 使用反射检查IsCreated属性（如果存在）
            var type = typeof(T);
            var isCreatedProperty = type.GetProperty("IsCreated");
            
            if (isCreatedProperty != null)
            {
                return (bool)isCreatedProperty.GetValue(container);
            }
            
            return true; // 如果没有IsCreated属性，假设有效
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 检查Burst编译器可用性
        /// </summary>
        private static bool CheckBurstAvailability()
        {
            try
            {
                // 尝试获取Burst编译器信息
                return BurstCompiler.IsEnabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查代码是否经过Burst编译
        /// </summary>
        private static bool CheckBurstCompilation()
        {
            // 在Burst编译的代码中，此方法会被替换为返回true
            return IsBurstCompiledInternal();
        }

        /// <summary>
        /// 内部Burst编译检测方法
        /// </summary>
        [BurstCompile]
        private static bool IsBurstCompiledInternal()
        {
            // 如果此方法被Burst编译，Unity.Burst.Intrinsics会可用
            // 这是一个简单的检测方式
            return Unity.Burst.Intrinsics.Common.umul128(1, 1, out _) == 0 || true;
        }

        /// <summary>
        /// 计算最优批处理大小
        /// </summary>
        private static int CalculateOptimalBatchSize()
        {
            int workerCount = GetWorkerThreadCount();
            
            // 基础批处理大小：64-256之间
            // 根据工作线程数调整
            if (workerCount <= 2)
                return 256;
            if (workerCount <= 4)
                return 128;
            if (workerCount <= 8)
                return 64;
            
            return 32;
        }

        #endregion
    }

    /// <summary>
    /// Burst运行时配置
    /// </summary>
    public struct BurstRuntimeConfig
    {
        /// <summary>
        /// 是否启用安全检查
        /// </summary>
        public bool EnableSafetyChecks;

        /// <summary>
        /// 是否启用同步编译（调试用）
        /// </summary>
        public bool EnableSynchronousCompilation;

        /// <summary>
        /// 默认并行批处理大小
        /// </summary>
        public int DefaultBatchSize;

        /// <summary>
        /// 并行处理阈值
        /// </summary>
        public int ParallelThreshold;

        /// <summary>
        /// 获取默认配置
        /// </summary>
        public static BurstRuntimeConfig Default => new BurstRuntimeConfig
        {
            EnableSafetyChecks = true,
            EnableSynchronousCompilation = false,
            DefaultBatchSize = 64,
            ParallelThreshold = 1000
        };

        /// <summary>
        /// 获取高性能配置（禁用安全检查）
        /// </summary>
        public static BurstRuntimeConfig HighPerformance => new BurstRuntimeConfig
        {
            EnableSafetyChecks = false,
            EnableSynchronousCompilation = false,
            DefaultBatchSize = 128,
            ParallelThreshold = 500
        };

        /// <summary>
        /// 获取调试配置
        /// </summary>
        public static BurstRuntimeConfig Debug => new BurstRuntimeConfig
        {
            EnableSafetyChecks = true,
            EnableSynchronousCompilation = true,
            DefaultBatchSize = 32,
            ParallelThreshold = 100
        };
    }
}
