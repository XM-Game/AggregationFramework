// ==========================================================
// 文件名：NoAliasAttribute.cs
// 命名空间：AFramework.Burst
// 创建时间：2026-01-01
// 功能描述：无别名标记，用于标记不会发生指针别名的参数
// 依赖：Unity.Burst
// ==========================================================

using System;
using Unity.Burst;

namespace AFramework.Burst
{
    /// <summary>
    /// 无别名标记特性
    /// 用于标记不会与其他指针发生别名的参数或字段
    /// 这允许Burst编译器进行更激进的优化
    /// </summary>
    /// <remarks>
    /// 使用此特性时，开发者必须确保标记的指针/引用不会与同一作用域内的其他指针/引用指向相同的内存区域。
    /// 错误使用可能导致未定义行为。
    /// 
    /// 示例用法：
    /// <code>
    /// [BurstCompile]
    /// public struct MyJob : IJob
    /// {
    ///     [NoAlias] public NativeArray&lt;float&gt; input;
    ///     [NoAlias] public NativeArray&lt;float&gt; output;
    ///     
    ///     public void Execute()
    ///     {
    ///         // Burst可以假设input和output不重叠，进行更好的优化
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NoAliasAttribute : Attribute
    {
        /// <summary>
        /// 别名组ID（可选）
        /// 相同组ID的参数可能互相别名，不同组ID的参数保证不别名
        /// </summary>
        public int GroupId { get; set; } = -1;

        /// <summary>
        /// 创建无别名标记
        /// </summary>
        public NoAliasAttribute()
        {
        }

        /// <summary>
        /// 创建带组ID的无别名标记
        /// </summary>
        /// <param name="groupId">别名组ID</param>
        public NoAliasAttribute(int groupId)
        {
            GroupId = groupId;
        }
    }

    /// <summary>
    /// 限制别名标记特性
    /// 用于标记可能与特定参数发生别名的参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class MayAliasAttribute : Attribute
    {
        /// <summary>
        /// 可能发生别名的参数名称
        /// </summary>
        public string AliasTarget { get; }

        /// <summary>
        /// 创建可能别名标记
        /// </summary>
        /// <param name="aliasTarget">可能发生别名的参数名称</param>
        public MayAliasAttribute(string aliasTarget)
        {
            AliasTarget = aliasTarget;
        }
    }
}
