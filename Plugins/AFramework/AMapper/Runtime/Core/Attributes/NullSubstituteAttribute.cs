// ==========================================================
// 文件名：NullSubstituteAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义空值替换特性，用于指定源值为空时的替换值
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 空值替换特性
    /// <para>用于指定源值为空时的替换值</para>
    /// <para>Null substitute attribute for specifying replacement value when source is null</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以在成员定义时声明空值替换，无需在 Profile 中显式配置。
    /// 
    /// 使用示例：
    /// <code>
    /// public class PlayerDto
    /// {
    ///     // 当源值为 null 时使用 "Unknown"
    ///     [NullSubstitute("Unknown")]
    ///     public string Name { get; set; }
    ///     
    ///     // 当源值为 null 时使用 0
    ///     [NullSubstitute(0)]
    ///     public int Score { get; set; }
    /// }
    /// </code>
    /// 
    /// 等效于 Profile 配置：
    /// <code>
    /// CreateMap&lt;Player, PlayerDto&gt;()
    ///     .ForMember(d => d.Name, opt => opt.NullSubstitute("Unknown"))
    ///     .ForMember(d => d.Score, opt => opt.NullSubstitute(0));
    /// </code>
    /// 
    /// 注意：替换值的类型必须与源成员类型兼容。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NullSubstituteAttribute : Attribute
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取替换值
        /// <para>Get the substitute value</para>
        /// </summary>
        public object Value { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建空值替换特性实例
        /// </summary>
        /// <param name="value">替换值 / Substitute value</param>
        /// <remarks>
        /// 替换值可以是任何类型，但必须与目标成员类型兼容。
        /// </remarks>
        public NullSubstituteAttribute(object value)
        {
            Value = value;
        }

        #endregion
    }
}
