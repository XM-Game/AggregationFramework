// ==========================================================
// 文件名：WriteOnlyAttribute.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：只写标记，用于标记Job中只写的数据
// 依赖：Unity.Collections
// ==========================================================

using System;

namespace AFramework.Burst
{
    /// <summary>
    /// 只写数据标记特性
    /// 用于标记Job中只会被写入而不会被读取的数据
    /// </summary>
    /// <remarks>
    /// 此特性是Unity.Collections.WriteOnlyAttribute的补充，
    /// 提供更多的语义信息和文档说明。
    /// 
    /// 使用此特性可以：
    /// 1. 明确表达数据访问意图
    /// 2. 帮助Burst编译器进行优化（避免不必要的内存读取）
    /// 3. 在安全检查模式下验证数据访问
    /// 
    /// 示例用法：
    /// <code>
    /// [BurstCompile]
    /// public struct InitializeJob : IJobParallelFor
    /// {
    ///     [WriteOnlyData] public NativeArray&lt;float&gt; output;
    ///     public float value;
    ///     
    ///     public void Execute(int index)
    ///     {
    ///         output[index] = value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class WriteOnlyDataAttribute : Attribute
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建只写数据标记
        /// </summary>
        public WriteOnlyDataAttribute()
        {
        }

        /// <summary>
        /// 创建带描述的只写数据标记
        /// </summary>
        /// <param name="description">描述信息</param>
        public WriteOnlyDataAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 输出数据标记特性
    /// 用于标记作为输出结果的数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class OutputDataAttribute : Attribute
    {
        /// <summary>
        /// 是否为主要输出
        /// </summary>
        public bool IsPrimary { get; set; } = true;

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建输出数据标记
        /// </summary>
        public OutputDataAttribute()
        {
        }

        /// <summary>
        /// 创建带描述的输出数据标记
        /// </summary>
        /// <param name="description">描述信息</param>
        public OutputDataAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 临时数据标记特性
    /// 用于标记Job执行过程中使用的临时数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TemporaryDataAttribute : Attribute
    {
        /// <summary>
        /// 预期的最大大小（元素数量）
        /// </summary>
        public int ExpectedMaxSize { get; set; } = -1;

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建临时数据标记
        /// </summary>
        public TemporaryDataAttribute()
        {
        }

        /// <summary>
        /// 创建带预期大小的临时数据标记
        /// </summary>
        /// <param name="expectedMaxSize">预期的最大大小</param>
        public TemporaryDataAttribute(int expectedMaxSize)
        {
            ExpectedMaxSize = expectedMaxSize;
        }
    }

    /// <summary>
    /// 读写数据标记特性
    /// 用于标记既会被读取也会被写入的数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadWriteDataAttribute : Attribute
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建读写数据标记
        /// </summary>
        public ReadWriteDataAttribute()
        {
        }

        /// <summary>
        /// 创建带描述的读写数据标记
        /// </summary>
        /// <param name="description">描述信息</param>
        public ReadWriteDataAttribute(string description)
        {
            Description = description;
        }
    }
}
