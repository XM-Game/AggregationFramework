// ==========================================================
// 文件名：MappingOptions.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 映射选项配置，定义映射行为的全局和局部选项
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射选项
    /// <para>定义映射行为的配置选项</para>
    /// <para>Mapping options that define mapping behavior configuration</para>
    /// </summary>
    /// <remarks>
    /// MappingOptions 与 MappingOperationOptions 的区别：
    /// <list type="bullet">
    /// <item>MappingOptions：全局配置，影响所有映射操作</item>
    /// <item>MappingOperationOptions：运行时配置，仅影响单次映射</item>
    /// </list>
    /// </remarks>
    public sealed class MappingOptions
    {
        #region 静态默认实例 / Static Default Instance

        /// <summary>
        /// 获取默认映射选项
        /// <para>Get default mapping options</para>
        /// </summary>
        public static MappingOptions Default { get; } = new MappingOptions();

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取或设置是否允许空目标值
        /// <para>Get or set whether null destination values are allowed</para>
        /// </summary>
        /// <remarks>
        /// 默认为 true。设置为 false 时，映射到 null 会抛出异常。
        /// </remarks>
        public bool AllowNullDestinationValues { get; set; } = true;

        /// <summary>
        /// 获取或设置是否允许空集合
        /// <para>Get or set whether null collections are allowed</para>
        /// </summary>
        /// <remarks>
        /// 默认为 true。设置为 false 时，null 集合会被映射为空集合。
        /// </remarks>
        public bool AllowNullCollections { get; set; } = true;

        /// <summary>
        /// 获取或设置默认最大映射深度
        /// <para>Get or set default maximum mapping depth</para>
        /// </summary>
        /// <remarks>
        /// null 表示无限制。用于防止无限递归。
        /// </remarks>
        public int? MaxDepth { get; set; }

        /// <summary>
        /// 获取或设置是否默认保留引用
        /// <para>Get or set whether to preserve references by default</para>
        /// </summary>
        /// <remarks>
        /// 默认为 false。启用后可处理循环引用，但会增加内存开销。
        /// </remarks>
        public bool PreserveReferences { get; set; }

        /// <summary>
        /// 获取或设置是否启用空值传播
        /// <para>Get or set whether null propagation is enabled</para>
        /// </summary>
        /// <remarks>
        /// 默认为 true。启用后，源对象为 null 时返回 null 而非抛出异常。
        /// </remarks>
        public bool EnableNullPropagation { get; set; } = true;

        /// <summary>
        /// 获取或设置是否在映射前验证配置
        /// <para>Get or set whether to validate configuration before mapping</para>
        /// </summary>
        /// <remarks>
        /// 默认为 false。仅在开发阶段启用，会影响性能。
        /// </remarks>
        public bool ValidateOnMap { get; set; }

        /// <summary>
        /// 获取或设置是否使用延迟编译
        /// <para>Get or set whether to use lazy compilation</para>
        /// </summary>
        /// <remarks>
        /// 默认为 true。设置为 false 时，所有映射在配置时立即编译。
        /// </remarks>
        public bool LazyCompilation { get; set; } = true;

        /// <summary>
        /// 获取或设置是否启用调试模式
        /// <para>Get or set whether debug mode is enabled</para>
        /// </summary>
        /// <remarks>
        /// 调试模式下会生成更详细的错误信息，但会影响性能。
        /// </remarks>
        public bool DebugMode { get; set; }

        /// <summary>
        /// 获取或设置源成员命名约定
        /// <para>Get or set source member naming convention</para>
        /// </summary>
        public NamingConventionType SourceNamingConvention { get; set; } = NamingConventionType.PascalCase;

        /// <summary>
        /// 获取或设置目标成员命名约定
        /// <para>Get or set destination member naming convention</para>
        /// </summary>
        public NamingConventionType DestinationNamingConvention { get; set; } = NamingConventionType.PascalCase;

        /// <summary>
        /// 获取或设置是否启用扁平化映射
        /// <para>Get or set whether flattening is enabled</para>
        /// </summary>
        /// <remarks>
        /// 默认为 true。启用后，CustomerName 会自动匹配 Customer.Name。
        /// </remarks>
        public bool EnableFlattening { get; set; } = true;

        /// <summary>
        /// 获取或设置是否启用反扁平化映射
        /// <para>Get or set whether unflattening is enabled</para>
        /// </summary>
        /// <remarks>
        /// 默认为 false。启用后，Customer.Name 会自动匹配 CustomerName。
        /// </remarks>
        public bool EnableUnflattening { get; set; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建映射选项实例
        /// </summary>
        public MappingOptions()
        {
        }

        /// <summary>
        /// 创建映射选项实例（复制构造）
        /// </summary>
        /// <param name="other">要复制的选项 / Options to copy</param>
        public MappingOptions(MappingOptions other)
        {
            if (other == null) return;

            AllowNullDestinationValues = other.AllowNullDestinationValues;
            AllowNullCollections = other.AllowNullCollections;
            MaxDepth = other.MaxDepth;
            PreserveReferences = other.PreserveReferences;
            EnableNullPropagation = other.EnableNullPropagation;
            ValidateOnMap = other.ValidateOnMap;
            LazyCompilation = other.LazyCompilation;
            DebugMode = other.DebugMode;
            SourceNamingConvention = other.SourceNamingConvention;
            DestinationNamingConvention = other.DestinationNamingConvention;
            EnableFlattening = other.EnableFlattening;
            EnableUnflattening = other.EnableUnflattening;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 创建副本
        /// <para>Create a copy</para>
        /// </summary>
        /// <returns>选项副本 / Options copy</returns>
        public MappingOptions Clone()
        {
            return new MappingOptions(this);
        }

        /// <summary>
        /// 合并选项
        /// <para>Merge options</para>
        /// </summary>
        /// <param name="other">要合并的选项 / Options to merge</param>
        /// <returns>合并后的新选项 / Merged options</returns>
        public MappingOptions Merge(MappingOptions other)
        {
            if (other == null) return Clone();

            var merged = Clone();
            
            // 仅合并非默认值
            if (other.MaxDepth.HasValue)
                merged.MaxDepth = other.MaxDepth;
            if (other.PreserveReferences)
                merged.PreserveReferences = true;
            if (!other.AllowNullDestinationValues)
                merged.AllowNullDestinationValues = false;
            if (!other.AllowNullCollections)
                merged.AllowNullCollections = false;
            if (other.DebugMode)
                merged.DebugMode = true;

            return merged;
        }

        #endregion
    }

    /// <summary>
    /// 命名约定类型
    /// <para>Naming convention type</para>
    /// </summary>
    public enum NamingConventionType
    {
        /// <summary>
        /// PascalCase 命名（如 CustomerName）
        /// </summary>
        PascalCase,

        /// <summary>
        /// camelCase 命名（如 customerName）
        /// </summary>
        CamelCase,

        /// <summary>
        /// snake_case 命名（如 customer_name）
        /// </summary>
        SnakeCase,

        /// <summary>
        /// 精确匹配（区分大小写）
        /// </summary>
        ExactMatch,

        /// <summary>
        /// 不区分大小写匹配
        /// </summary>
        CaseInsensitive
    }
}
