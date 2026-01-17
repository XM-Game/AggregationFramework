// ==========================================================
// 文件名：IMemberMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 定义成员映射配置接口，描述单个成员的映射规则
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员映射接口
    /// <para>定义单个目标成员的映射配置，包括源成员、值解析器、条件等</para>
    /// <para>Member map interface that defines mapping configuration for a single destination member</para>
    /// </summary>
    /// <remarks>
    /// MemberMap 描述如何将源对象的数据映射到目标对象的单个成员。
    /// 支持多种映射方式：
    /// <list type="bullet">
    /// <item>自动匹配：按名称匹配源成员</item>
    /// <item>显式指定：通过 MapFrom 指定源成员或表达式</item>
    /// <item>值解析器：通过 IValueResolver 自定义解析逻辑</item>
    /// <item>忽略：通过 Ignore 跳过映射</item>
    /// </list>
    /// </remarks>
    public interface IMemberMap
    {
        #region 目标成员信息 / Destination Member Information

        /// <summary>
        /// 获取目标成员信息
        /// <para>Get the destination member info</para>
        /// </summary>
        MemberInfo DestinationMember { get; }

        /// <summary>
        /// 获取目标成员名称
        /// <para>Get the destination member name</para>
        /// </summary>
        string DestinationName { get; }

        /// <summary>
        /// 获取目标成员类型
        /// <para>Get the destination member type</para>
        /// </summary>
        Type DestinationType { get; }

        #endregion

        #region 源成员信息 / Source Member Information

        /// <summary>
        /// 获取源成员信息链
        /// <para>Get the source member info chain</para>
        /// </summary>
        /// <remarks>
        /// 对于嵌套成员访问（如 Customer.Address.City），
        /// 此属性包含完整的成员链。
        /// </remarks>
        MemberInfo[] SourceMembers { get; }

        /// <summary>
        /// 获取源成员名称
        /// <para>Get the source member name</para>
        /// </summary>
        /// <remarks>
        /// 对于嵌套成员，返回完整路径（如 "Customer.Address.City"）。
        /// </remarks>
        string SourceMemberName { get; }

        /// <summary>
        /// 获取源成员类型
        /// <para>Get the source member type</para>
        /// </summary>
        Type SourceType { get; }

        #endregion

        #region 映射配置 / Mapping Configuration

        /// <summary>
        /// 获取是否忽略此成员
        /// <para>Get whether this member is ignored</para>
        /// </summary>
        bool IsIgnored { get; }

        /// <summary>
        /// 获取是否已映射
        /// <para>Get whether this member is mapped</para>
        /// </summary>
        /// <remarks>
        /// 如果成员被忽略或有有效的映射源，则返回 true。
        /// </remarks>
        bool IsMapped { get; }

        /// <summary>
        /// 获取是否使用目标值
        /// <para>Get whether to use destination value</para>
        /// </summary>
        /// <remarks>
        /// 通过 UseDestinationValue() 配置。
        /// 为 true 时，映射到现有目标成员值而非创建新实例。
        /// </remarks>
        bool UseDestinationValue { get; }

        /// <summary>
        /// 获取映射顺序
        /// <para>Get the mapping order</para>
        /// </summary>
        /// <remarks>
        /// 通过 SetMappingOrder(n) 配置。
        /// 数值越小越先执行。
        /// </remarks>
        int MappingOrder { get; }

        #endregion

        #region 值解析 / Value Resolution

        /// <summary>
        /// 获取自定义映射表达式
        /// <para>Get the custom mapping expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 MapFrom(expression) 配置的表达式。
        /// </remarks>
        LambdaExpression CustomMapExpression { get; }

        /// <summary>
        /// 获取值解析器类型
        /// <para>Get the value resolver type</para>
        /// </summary>
        /// <remarks>
        /// 通过 MapFrom&lt;TResolver&gt;() 配置的解析器类型。
        /// </remarks>
        Type ValueResolverType { get; }

        /// <summary>
        /// 获取值解析器配置
        /// <para>Get the value resolver configuration</para>
        /// </summary>
        IValueResolverConfiguration ValueResolverConfig { get; }

        /// <summary>
        /// 获取是否有自定义值解析
        /// <para>Get whether custom value resolution is configured</para>
        /// </summary>
        bool HasCustomResolver { get; }

        #endregion

        #region 值转换 / Value Conversion

        /// <summary>
        /// 获取值转换器类型
        /// <para>Get the value converter type</para>
        /// </summary>
        /// <remarks>
        /// 通过 ConvertUsing&lt;TConverter&gt;() 配置的转换器类型。
        /// </remarks>
        Type ValueConverterType { get; }

        /// <summary>
        /// 获取值转换表达式
        /// <para>Get the value conversion expression</para>
        /// </summary>
        LambdaExpression ValueConverterExpression { get; }

        /// <summary>
        /// 获取是否有值转换器
        /// <para>Get whether value converter is configured</para>
        /// </summary>
        bool HasValueConverter { get; }

        #endregion

        #region 条件映射 / Conditional Mapping

        /// <summary>
        /// 获取映射条件表达式
        /// <para>Get the mapping condition expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 Condition() 配置的条件表达式。
        /// 条件为 false 时跳过此成员的映射。
        /// </remarks>
        LambdaExpression ConditionExpression { get; }

        /// <summary>
        /// 获取前置条件表达式
        /// <para>Get the pre-condition expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 PreCondition() 配置的前置条件。
        /// 在值解析之前评估，为 false 时跳过解析和映射。
        /// </remarks>
        LambdaExpression PreConditionExpression { get; }

        /// <summary>
        /// 获取是否有条件配置
        /// <para>Get whether condition is configured</para>
        /// </summary>
        bool HasCondition { get; }

        #endregion

        #region 空值处理 / Null Handling

        /// <summary>
        /// 获取空值替换值
        /// <para>Get the null substitute value</para>
        /// </summary>
        /// <remarks>
        /// 通过 NullSubstitute(value) 配置。
        /// 当源值为 null 时使用此替换值。
        /// </remarks>
        object NullSubstitute { get; }

        /// <summary>
        /// 获取是否有空值替换配置
        /// <para>Get whether null substitute is configured</para>
        /// </summary>
        bool HasNullSubstitute { get; }

        /// <summary>
        /// 获取是否允许空值
        /// <para>Get whether null value is allowed</para>
        /// </summary>
        /// <remarks>
        /// 通过 AllowNull() 或 DoNotAllowNull() 配置。
        /// null 表示使用默认行为。
        /// </remarks>
        bool? AllowNull { get; }

        #endregion

        #region 所属类型映射 / Parent TypeMap

        /// <summary>
        /// 获取所属的类型映射
        /// <para>Get the parent type map</para>
        /// </summary>
        ITypeMap TypeMap { get; }

        #endregion
    }

    /// <summary>
    /// 值解析器配置接口
    /// <para>Value resolver configuration interface</para>
    /// </summary>
    public interface IValueResolverConfiguration
    {
        /// <summary>
        /// 获取解析器类型
        /// <para>Get the resolver type</para>
        /// </summary>
        Type ResolverType { get; }

        /// <summary>
        /// 获取解析器实例（如果已提供）
        /// <para>Get the resolver instance if provided</para>
        /// </summary>
        object ResolverInstance { get; }

        /// <summary>
        /// 获取源成员表达式（用于 IMemberValueResolver）
        /// <para>Get the source member expression (for IMemberValueResolver)</para>
        /// </summary>
        LambdaExpression SourceMemberExpression { get; }

        /// <summary>
        /// 获取源成员类型
        /// <para>Get the source member type</para>
        /// </summary>
        Type SourceMemberType { get; }
    }
}
