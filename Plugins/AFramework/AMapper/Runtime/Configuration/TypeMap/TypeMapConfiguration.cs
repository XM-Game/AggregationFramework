// ==========================================================
// 文件名：TypeMapConfiguration.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions
// 功能: 类型映射配置表达式，提供类型级别的配置选项
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型映射配置
    /// <para>存储类型映射的配置选项</para>
    /// <para>Type map configuration storing configuration options</para>
    /// </summary>
    public sealed class TypeMapConfiguration
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源类型
        /// <para>Get source type</para>
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// 获取目标类型
        /// <para>Get destination type</para>
        /// </summary>
        public Type DestinationType { get; }

        /// <summary>
        /// 获取类型对
        /// <para>Get type pair</para>
        /// </summary>
        public TypePair TypePair { get; }

        /// <summary>
        /// 获取或设置最大深度
        /// <para>Get or set maximum depth</para>
        /// </summary>
        public int? MaxDepth { get; set; }

        /// <summary>
        /// 获取或设置是否保留引用
        /// <para>Get or set whether to preserve references</para>
        /// </summary>
        public bool PreserveReferences { get; set; }

        /// <summary>
        /// 获取或设置是否禁用构造函数映射
        /// <para>Get or set whether to disable constructor mapping</para>
        /// </summary>
        public bool DisableConstructorMapping { get; set; }

        /// <summary>
        /// 获取或设置自定义构造表达式
        /// <para>Get or set custom construction expression</para>
        /// </summary>
        public LambdaExpression CustomCtorExpression { get; set; }

        /// <summary>
        /// 获取或设置类型转换器类型
        /// <para>Get or set type converter type</para>
        /// </summary>
        public Type TypeConverterType { get; set; }

        /// <summary>
        /// 获取或设置类型转换表达式
        /// <para>Get or set type conversion expression</para>
        /// </summary>
        public LambdaExpression TypeConverterExpression { get; set; }

        /// <summary>
        /// 获取或设置条件表达式
        /// <para>Get or set condition expression</para>
        /// </summary>
        public LambdaExpression ConditionExpression { get; set; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建类型映射配置实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        public TypeMapConfiguration(Type sourceType, Type destinationType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            TypePair = new TypePair(sourceType, destinationType);
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 应用配置到 TypeMap
        /// <para>Apply configuration to TypeMap</para>
        /// </summary>
        /// <param name="typeMap">类型映射 / Type map</param>
        public void ApplyTo(TypeMap typeMap)
        {
            if (typeMap == null)
                throw new ArgumentNullException(nameof(typeMap));

            if (MaxDepth.HasValue)
                typeMap.SetMaxDepth(MaxDepth.Value);

            if (PreserveReferences)
                typeMap.EnablePreserveReferences();

            if (DisableConstructorMapping)
                typeMap.DisableConstructorMappingInternal();

            if (CustomCtorExpression != null)
                typeMap.SetCustomCtorExpression(CustomCtorExpression);

            if (TypeConverterType != null)
                typeMap.SetTypeConverter(TypeConverterType);

            if (TypeConverterExpression != null)
                typeMap.SetTypeConverterExpression(TypeConverterExpression);

            if (ConditionExpression != null)
                typeMap.SetCondition(ConditionExpression);
        }

        #endregion
    }
}
