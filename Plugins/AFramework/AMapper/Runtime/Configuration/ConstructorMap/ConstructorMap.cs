// ==========================================================
// 文件名：ConstructorMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: 构造函数映射配置，管理构造函数参数的映射规则
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 构造函数映射接口
    /// <para>Constructor map interface</para>
    /// </summary>
    public interface IConstructorMap
    {
        /// <summary>
        /// 获取构造函数信息
        /// <para>Get constructor info</para>
        /// </summary>
        ConstructorInfo Constructor { get; }

        /// <summary>
        /// 获取参数映射列表
        /// <para>Get parameter maps</para>
        /// </summary>
        IReadOnlyList<ConstructorParameterMap> ParameterMaps { get; }

        /// <summary>
        /// 获取是否所有参数都可解析
        /// <para>Get whether all parameters can be resolved</para>
        /// </summary>
        bool CanResolve { get; }
    }

    /// <summary>
    /// 构造函数映射配置
    /// <para>Constructor map configuration</para>
    /// </summary>
    public sealed class ConstructorMap : IConstructorMap
    {
        #region 属性 / Properties

        /// <inheritdoc/>
        public ConstructorInfo Constructor { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ConstructorParameterMap> ParameterMaps { get; }

        /// <inheritdoc/>
        public bool CanResolve => ParameterMaps.All(p => p.IsMapped);

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建构造函数映射实例
        /// </summary>
        /// <param name="constructor">构造函数信息 / Constructor info</param>
        /// <param name="parameterMaps">参数映射列表 / Parameter maps</param>
        internal ConstructorMap(ConstructorInfo constructor, IEnumerable<ConstructorParameterMap> parameterMaps)
        {
            Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            ParameterMaps = parameterMaps?.ToList() ?? new List<ConstructorParameterMap>();
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 查找参数映射
        /// <para>Find parameter map</para>
        /// </summary>
        /// <param name="parameterName">参数名称 / Parameter name</param>
        /// <returns>参数映射或 null / Parameter map or null</returns>
        public ConstructorParameterMap FindParameterMap(string parameterName)
        {
            return ParameterMaps.FirstOrDefault(p => 
                string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }

    /// <summary>
    /// 构造函数参数映射
    /// <para>Constructor parameter map</para>
    /// </summary>
    public sealed class ConstructorParameterMap
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取参数信息
        /// <para>Get parameter info</para>
        /// </summary>
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// 获取参数名称
        /// <para>Get parameter name</para>
        /// </summary>
        public string ParameterName => Parameter.Name;

        /// <summary>
        /// 获取参数类型
        /// <para>Get parameter type</para>
        /// </summary>
        public Type ParameterType => Parameter.ParameterType;

        /// <summary>
        /// 获取源成员
        /// <para>Get source member</para>
        /// </summary>
        public MemberInfo SourceMember { get; private set; }

        /// <summary>
        /// 获取自定义映射表达式
        /// <para>Get custom map expression</para>
        /// </summary>
        public LambdaExpression CustomMapExpression { get; private set; }

        /// <summary>
        /// 获取是否已映射
        /// <para>Get whether mapped</para>
        /// </summary>
        public bool IsMapped => SourceMember != null || CustomMapExpression != null || Parameter.HasDefaultValue;

        /// <summary>
        /// 获取是否有默认值
        /// <para>Get whether has default value</para>
        /// </summary>
        public bool HasDefaultValue => Parameter.HasDefaultValue;

        /// <summary>
        /// 获取默认值
        /// <para>Get default value</para>
        /// </summary>
        public object DefaultValue => Parameter.DefaultValue;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建构造函数参数映射实例
        /// </summary>
        /// <param name="parameter">参数信息 / Parameter info</param>
        /// <param name="sourceMember">源成员 / Source member</param>
        internal ConstructorParameterMap(ParameterInfo parameter, MemberInfo sourceMember = null)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            SourceMember = sourceMember;
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 设置源成员
        /// </summary>
        internal void SetSourceMember(MemberInfo member)
        {
            SourceMember = member;
        }

        /// <summary>
        /// 设置自定义映射表达式
        /// </summary>
        internal void SetCustomMapExpression(LambdaExpression expression)
        {
            CustomMapExpression = expression;
        }

        #endregion
    }
}
