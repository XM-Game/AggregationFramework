// ==========================================================
// 文件名：ReadOnlyAttribute.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：只读标记，用于标记Job中只读的数据
// 依赖：Unity.Collections
// ==========================================================

using System;

namespace AFramework.Burst
{
    /// <summary>
    /// 只读数据标记特性
    /// 用于标记Job中只会被读取而不会被修改的数据
    /// </summary>
    /// <remarks>
    /// 此特性是Unity.Collections.ReadOnlyAttribute的补充，
    /// 提供更多的语义信息和文档说明。
    /// 
    /// 使用此特性可以：
    /// 1. 明确表达数据访问意图
    /// 2. 帮助Burst编译器进行优化
    /// 3. 在安全检查模式下验证数据访问
    /// 
    /// 示例用法：
    /// <code>
    /// [BurstCompile]
    /// public struct MyJob : IJobParallelFor
    /// {
    ///     [ReadOnlyData] public NativeArray&lt;float&gt; input;
    ///     public NativeArray&lt;float&gt; output;
    ///     
    ///     public void Execute(int index)
    ///     {
    ///         output[index] = input[index] * 2f;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyDataAttribute : Attribute
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建只读数据标记
        /// </summary>
        public ReadOnlyDataAttribute()
        {
        }

        /// <summary>
        /// 创建带描述的只读数据标记
        /// </summary>
        /// <param name="description">描述信息</param>
        public ReadOnlyDataAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 不可变数据标记特性
    /// 用于标记在整个生命周期内都不会改变的数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class ImmutableAttribute : Attribute
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建不可变数据标记
        /// </summary>
        public ImmutableAttribute()
        {
        }

        /// <summary>
        /// 创建带描述的不可变数据标记
        /// </summary>
        /// <param name="description">描述信息</param>
        public ImmutableAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 常量数据标记特性
    /// 用于标记编译时已知的常量数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ConstantDataAttribute : Attribute
    {
        /// <summary>
        /// 创建常量数据标记
        /// </summary>
        public ConstantDataAttribute()
        {
        }
    }
}
