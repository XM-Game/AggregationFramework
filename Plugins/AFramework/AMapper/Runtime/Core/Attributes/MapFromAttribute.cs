// ==========================================================
// 文件名：MapFromAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义映射来源特性，用于指定目标成员的映射来源
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射来源特性
    /// <para>用于指定目标成员的映射来源</para>
    /// <para>Map from attribute for specifying the source of destination member mapping</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以在目标成员上声明映射来源，无需在 Profile 中显式配置。
    /// 
    /// 使用示例：
    /// <code>
    /// public class PlayerDto
    /// {
    ///     // 从源对象的 PlayerName 属性映射
    ///     [MapFrom("PlayerName")]
    ///     public string Name { get; set; }
    ///     
    ///     // 从嵌套属性映射
    ///     [MapFrom("Stats.Level")]
    ///     public int CurrentLevel { get; set; }
    ///     
    ///     // 使用值解析器
    ///     [MapFrom(typeof(FullNameResolver))]
    ///     public string FullName { get; set; }
    /// }
    /// </code>
    /// 
    /// 等效于 Profile 配置：
    /// <code>
    /// CreateMap&lt;Player, PlayerDto&gt;()
    ///     .ForMember(d => d.Name, opt => opt.MapFrom(s => s.PlayerName))
    ///     .ForMember(d => d.CurrentLevel, opt => opt.MapFrom(s => s.Stats.Level))
    ///     .ForMember(d => d.FullName, opt => opt.MapFrom&lt;FullNameResolver&gt;());
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MapFromAttribute : Attribute
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源成员名称或路径
        /// <para>Get the source member name or path</para>
        /// </summary>
        /// <remarks>
        /// 支持嵌套路径，如 "Customer.Address.City"。
        /// </remarks>
        public string SourceMemberName { get; }

        /// <summary>
        /// 获取值解析器类型
        /// <para>Get the value resolver type</para>
        /// </summary>
        /// <remarks>
        /// 必须实现 IValueResolver 接口。
        /// </remarks>
        public Type ResolverType { get; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射来源特性实例（指定源成员名称）
        /// </summary>
        /// <param name="sourceMemberName">源成员名称或路径 / Source member name or path</param>
        /// <exception cref="ArgumentNullException">当 sourceMemberName 为 null 或空时抛出</exception>
        public MapFromAttribute(string sourceMemberName)
        {
            if (string.IsNullOrWhiteSpace(sourceMemberName))
            {
                throw new ArgumentNullException(nameof(sourceMemberName));
            }
            SourceMemberName = sourceMemberName;
        }

        /// <summary>
        /// 创建映射来源特性实例（指定值解析器类型）
        /// </summary>
        /// <param name="resolverType">值解析器类型 / Value resolver type</param>
        /// <exception cref="ArgumentNullException">当 resolverType 为 null 时抛出</exception>
        public MapFromAttribute(Type resolverType)
        {
            ResolverType = resolverType ?? throw new ArgumentNullException(nameof(resolverType));
        }

        #endregion
    }
}
