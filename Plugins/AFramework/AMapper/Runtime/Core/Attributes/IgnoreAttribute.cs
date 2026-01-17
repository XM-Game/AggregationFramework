// ==========================================================
// 文件名：IgnoreAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义忽略映射特性，用于标记不参与映射的成员
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 忽略映射特性
    /// <para>用于标记不参与映射的成员</para>
    /// <para>Ignore attribute for marking members that should not be mapped</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以在成员定义时声明忽略映射，无需在 Profile 中显式配置。
    /// 
    /// 使用示例：
    /// <code>
    /// public class PlayerDto
    /// {
    ///     public string Name { get; set; }
    ///     
    ///     [Ignore]
    ///     public string InternalId { get; set; }
    ///     
    ///     [Ignore]
    ///     public DateTime CacheTime { get; set; }
    /// }
    /// </code>
    /// 
    /// 等效于 Profile 配置：
    /// <code>
    /// CreateMap&lt;Player, PlayerDto&gt;()
    ///     .ForMember(d => d.InternalId, opt => opt.Ignore())
    ///     .ForMember(d => d.CacheTime, opt => opt.Ignore());
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// 创建忽略映射特性实例
        /// </summary>
        public IgnoreAttribute()
        {
        }
    }
}
