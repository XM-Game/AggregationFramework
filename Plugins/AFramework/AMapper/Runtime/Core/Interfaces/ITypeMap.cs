// ==========================================================
// 文件名：ITypeMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Linq.Expressions
// 功能: 定义类型映射配置接口，描述源类型到目标类型的映射规则
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型映射接口
    /// <para>定义单个源-目标类型对的映射配置，包括成员映射、构造函数映射等</para>
    /// <para>Type map interface that defines mapping configuration for a source-destination type pair</para>
    /// </summary>
    /// <remarks>
    /// TypeMap 是 AMapper 配置的核心单元，包含：
    /// <list type="bullet">
    /// <item>成员映射配置（MemberMap）</item>
    /// <item>构造函数映射配置（ConstructorMap）</item>
    /// <item>前置/后置映射动作</item>
    /// <item>类型转换器配置</item>
    /// <item>继承映射配置</item>
    /// </list>
    /// </remarks>
    public interface ITypeMap
    {
        #region 类型信息 / Type Information

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        /// 获取目标类型
        /// <para>Get the destination type</para>
        /// </summary>
        Type DestinationType { get; }

        /// <summary>
        /// 获取类型对
        /// <para>Get the type pair</para>
        /// </summary>
        TypePair TypePair { get; }

        /// <summary>
        /// 获取所属的 Profile 类型
        /// <para>Get the profile type this map belongs to</para>
        /// </summary>
        Type ProfileType { get; }

        #endregion

        #region 成员映射 / Member Maps

        /// <summary>
        /// 获取所有成员映射配置
        /// <para>Get all member map configurations</para>
        /// </summary>
        IReadOnlyCollection<IMemberMap> MemberMaps { get; }

        /// <summary>
        /// 获取指定目标成员的映射配置
        /// <para>Get the member map for specified destination member</para>
        /// </summary>
        /// <param name="destinationMemberName">目标成员名称 / Destination member name</param>
        /// <returns>成员映射配置，如果不存在则返回 null / Member map or null if not found</returns>
        IMemberMap FindMemberMap(string destinationMemberName);

        /// <summary>
        /// 获取所有路径映射配置
        /// <para>Get all path map configurations</para>
        /// </summary>
        IReadOnlyCollection<IMemberMap> PathMaps { get; }

        #endregion

        #region 构造函数映射 / Constructor Map

        /// <summary>
        /// 获取构造函数映射配置
        /// <para>Get the constructor map configuration</para>
        /// </summary>
        /// <remarks>
        /// 如果目标类型使用参数化构造函数创建，此属性包含参数映射配置。
        /// 如果使用默认构造函数，此属性可能为 null。
        /// </remarks>
        IConstructorMap ConstructorMap { get; }

        /// <summary>
        /// 获取是否禁用构造函数映射
        /// <para>Get whether constructor mapping is disabled</para>
        /// </summary>
        bool DisableConstructorMapping { get; }

        #endregion

        #region 自定义构造 / Custom Construction

        /// <summary>
        /// 获取自定义构造表达式
        /// <para>Get the custom construction expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 ConstructUsing 配置的自定义构造逻辑。
        /// 如果未配置，返回 null。
        /// </remarks>
        LambdaExpression CustomCtorExpression { get; }

        /// <summary>
        /// 获取自定义构造函数
        /// <para>Get the custom constructor function</para>
        /// </summary>
        Delegate CustomCtorFunction { get; }

        /// <summary>
        /// 获取是否有自定义构造配置
        /// <para>Get whether custom construction is configured</para>
        /// </summary>
        bool HasCustomConstruction { get; }

        #endregion

        #region 类型转换器 / Type Converter

        /// <summary>
        /// 获取类型转换器类型
        /// <para>Get the type converter type</para>
        /// </summary>
        /// <remarks>
        /// 通过 ConvertUsing&lt;TConverter&gt;() 配置的转换器类型。
        /// </remarks>
        Type TypeConverterType { get; }

        /// <summary>
        /// 获取类型转换表达式
        /// <para>Get the type conversion expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 ConvertUsing(expression) 配置的转换表达式。
        /// </remarks>
        LambdaExpression TypeConverterExpression { get; }

        /// <summary>
        /// 获取是否使用类型转换器
        /// <para>Get whether type converter is used</para>
        /// </summary>
        bool HasTypeConverter { get; }

        #endregion

        #region 映射动作 / Mapping Actions

        /// <summary>
        /// 获取前置映射动作列表
        /// <para>Get the list of before map actions</para>
        /// </summary>
        IReadOnlyList<LambdaExpression> BeforeMapActions { get; }

        /// <summary>
        /// 获取后置映射动作列表
        /// <para>Get the list of after map actions</para>
        /// </summary>
        IReadOnlyList<LambdaExpression> AfterMapActions { get; }

        #endregion

        #region 继承映射 / Inheritance Mapping

        /// <summary>
        /// 获取包含的派生类型映射
        /// <para>Get the included derived type maps</para>
        /// </summary>
        /// <remarks>
        /// 通过 Include&lt;TDerived&gt;() 配置的派生类型映射。
        /// </remarks>
        IReadOnlyList<TypePair> IncludedDerivedTypes { get; }

        /// <summary>
        /// 获取包含的基类型映射
        /// <para>Get the included base type maps</para>
        /// </summary>
        /// <remarks>
        /// 通过 IncludeBase&lt;TBase&gt;() 配置的基类型映射。
        /// </remarks>
        IReadOnlyList<TypePair> IncludedBaseTypes { get; }

        /// <summary>
        /// 获取包含的成员类型
        /// <para>Get the included member types</para>
        /// </summary>
        /// <remarks>
        /// 通过 IncludeMembers() 配置的成员类型。
        /// </remarks>
        IReadOnlyList<LambdaExpression> IncludedMembers { get; }

        #endregion

        #region 映射选项 / Mapping Options

        /// <summary>
        /// 获取最大映射深度
        /// <para>Get the maximum mapping depth</para>
        /// </summary>
        /// <remarks>
        /// 通过 MaxDepth(n) 配置的最大深度。
        /// null 表示无限制。
        /// </remarks>
        int? MaxDepth { get; }

        /// <summary>
        /// 获取是否保留引用
        /// <para>Get whether to preserve references</para>
        /// </summary>
        /// <remarks>
        /// 通过 PreserveReferences(true) 配置。
        /// 用于处理循环引用。
        /// </remarks>
        bool PreserveReferences { get; }

        /// <summary>
        /// 获取映射条件表达式
        /// <para>Get the mapping condition expression</para>
        /// </summary>
        /// <remarks>
        /// 通过 Condition() 配置的条件表达式。
        /// 条件为 false 时跳过整个映射。
        /// </remarks>
        LambdaExpression ConditionExpression { get; }

        /// <summary>
        /// 获取值转换器列表
        /// <para>Get the list of value transformers</para>
        /// </summary>
        IReadOnlyList<ValueTransformerConfiguration> ValueTransformers { get; }

        #endregion

        #region 配置状态 / Configuration State

        /// <summary>
        /// 获取配置是否已密封
        /// <para>Get whether the configuration is sealed</para>
        /// </summary>
        /// <remarks>
        /// 密封后的配置不可修改。
        /// 配置在 MapperConfiguration 构建完成后自动密封。
        /// </remarks>
        bool IsSealed { get; }

        #endregion
    }

    /// <summary>
    /// 值转换器配置
    /// <para>Value transformer configuration</para>
    /// </summary>
    public readonly struct ValueTransformerConfiguration
    {
        /// <summary>
        /// 获取值类型
        /// <para>Get the value type</para>
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// 获取转换表达式
        /// <para>Get the transform expression</para>
        /// </summary>
        public LambdaExpression TransformExpression { get; }

        /// <summary>
        /// 创建值转换器配置
        /// </summary>
        /// <param name="valueType">值类型 / Value type</param>
        /// <param name="transformExpression">转换表达式 / Transform expression</param>
        public ValueTransformerConfiguration(Type valueType, LambdaExpression transformExpression)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            TransformExpression = transformExpression ?? throw new ArgumentNullException(nameof(transformExpression));
        }
    }
}
