// ==========================================================
// 文件名：AutoMapAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义自动映射特性，用于声明式配置类型映射
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 自动映射特性
    /// <para>用于声明式配置类型映射，标记在目标类型上</para>
    /// <para>Auto map attribute for declarative type mapping configuration</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以在类型定义时声明映射关系，无需在 Profile 中显式配置。
    /// 
    /// 使用示例：
    /// <code>
    /// // 基本用法：标记目标类型，指定源类型
    /// [AutoMap(typeof(Player))]
    /// public class PlayerDto
    /// {
    ///     public string Name { get; set; }
    ///     public int Level { get; set; }
    /// }
    /// 
    /// // 双向映射
    /// [AutoMap(typeof(Player), ReverseMap = true)]
    /// public class PlayerDto { }
    /// 
    /// // 指定 Profile
    /// [AutoMap(typeof(Player), ProfileType = typeof(GameProfile))]
    /// public class PlayerDto { }
    /// </code>
    /// 
    /// 配置扫描：
    /// <code>
    /// var config = new MapperConfiguration(cfg =>
    /// {
    ///     cfg.AddMaps(typeof(PlayerDto).Assembly);
    /// });
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public sealed class AutoMapAttribute : Attribute
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// 获取或设置是否创建反向映射
        /// <para>Get or set whether to create reverse map</para>
        /// </summary>
        /// <remarks>
        /// 设置为 true 时，会同时创建从目标类型到源类型的映射。
        /// </remarks>
        public bool ReverseMap { get; set; }

        /// <summary>
        /// 获取或设置所属的 Profile 类型
        /// <para>Get or set the profile type this map belongs to</para>
        /// </summary>
        /// <remarks>
        /// 如果不指定，映射将添加到默认 Profile。
        /// </remarks>
        public Type ProfileType { get; set; }

        /// <summary>
        /// 获取或设置构造函数映射是否禁用
        /// <para>Get or set whether constructor mapping is disabled</para>
        /// </summary>
        public bool DisableConstructorMapping { get; set; }

        /// <summary>
        /// 获取或设置最大映射深度
        /// <para>Get or set the maximum mapping depth</para>
        /// </summary>
        /// <remarks>
        /// 0 表示无限制。
        /// </remarks>
        public int MaxDepth { get; set; }

        /// <summary>
        /// 获取或设置是否保留引用
        /// <para>Get or set whether to preserve references</para>
        /// </summary>
        public bool PreserveReferences { get; set; }

        /// <summary>
        /// 获取或设置包含的基类型
        /// <para>Get or set the included base types</para>
        /// </summary>
        public Type[] IncludeBaseTypes { get; set; }

        /// <summary>
        /// 获取或设置包含的派生类型
        /// <para>Get or set the included derived types</para>
        /// </summary>
        public Type[] IncludeDerivedTypes { get; set; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建自动映射特性实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <exception cref="ArgumentNullException">当 sourceType 为 null 时抛出</exception>
        public AutoMapAttribute(Type sourceType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        }

        #endregion
    }
}
