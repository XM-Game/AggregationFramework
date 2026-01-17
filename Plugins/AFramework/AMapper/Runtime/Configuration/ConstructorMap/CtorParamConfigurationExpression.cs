// ==========================================================
// 文件名：CtorParamConfigurationExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions
// 功能: 构造函数参数配置表达式，用于配置构造函数参数的映射
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 构造函数参数配置
    /// <para>存储构造函数参数的映射配置</para>
    /// <para>Constructor parameter configuration storing mapping settings</para>
    /// </summary>
    public sealed class CtorParamConfiguration
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取参数名称
        /// <para>Get parameter name</para>
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// 获取源成员名称
        /// <para>Get source member name</para>
        /// </summary>
        public string SourceMemberName { get; private set; }

        /// <summary>
        /// 获取自定义映射表达式
        /// <para>Get custom map expression</para>
        /// </summary>
        public LambdaExpression CustomMapExpression { get; private set; }

        /// <summary>
        /// 获取是否已配置
        /// <para>Get whether configured</para>
        /// </summary>
        public bool IsConfigured => !string.IsNullOrEmpty(SourceMemberName) || CustomMapExpression != null;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建构造函数参数配置实例
        /// </summary>
        /// <param name="parameterName">参数名称 / Parameter name</param>
        public CtorParamConfiguration(string parameterName)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 设置源成员名称
        /// <para>Set source member name</para>
        /// </summary>
        /// <param name="sourceMemberName">源成员名称 / Source member name</param>
        public void MapFrom(string sourceMemberName)
        {
            SourceMemberName = sourceMemberName;
        }

        /// <summary>
        /// 设置自定义映射表达式
        /// <para>Set custom map expression</para>
        /// </summary>
        /// <param name="expression">映射表达式 / Map expression</param>
        public void MapFrom(LambdaExpression expression)
        {
            CustomMapExpression = expression;
        }

        #endregion
    }
}
