// ==========================================================
// 文件名：MappingOrderAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义映射顺序特性，用于控制成员映射的执行顺序
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射顺序特性
    /// <para>用于控制成员映射的执行顺序</para>
    /// <para>Mapping order attribute for controlling the execution order of member mapping</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以控制成员映射的执行顺序。
    /// 数值越小越先执行，默认顺序为 0。
    /// 
    /// 使用示例：
    /// <code>
    /// public class PlayerDto
    /// {
    ///     // 最先映射
    ///     [MappingOrder(-10)]
    ///     public int Id { get; set; }
    ///     
    ///     // 默认顺序
    ///     public string Name { get; set; }
    ///     
    ///     // 最后映射（依赖其他属性）
    ///     [MappingOrder(100)]
    ///     public string DisplayName { get; set; }
    /// }
    /// </code>
    /// 
    /// 等效于 Profile 配置：
    /// <code>
    /// CreateMap&lt;Player, PlayerDto&gt;()
    ///     .ForMember(d => d.Id, opt => opt.SetMappingOrder(-10))
    ///     .ForMember(d => d.DisplayName, opt => opt.SetMappingOrder(100));
    /// </code>
    /// 
    /// 使用场景：
    /// <list type="bullet">
    /// <item>某些属性依赖其他属性的值</item>
    /// <item>需要确保特定属性先被设置</item>
    /// <item>计算属性需要在基础属性之后映射</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MappingOrderAttribute : Attribute
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取映射顺序
        /// <para>Get the mapping order</para>
        /// </summary>
        /// <remarks>
        /// 数值越小越先执行。
        /// 可以使用负数确保某些属性最先映射。
        /// </remarks>
        public int Order { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建映射顺序特性实例
        /// </summary>
        /// <param name="order">映射顺序 / Mapping order</param>
        public MappingOrderAttribute(int order)
        {
            Order = order;
        }

        #endregion
    }
}
