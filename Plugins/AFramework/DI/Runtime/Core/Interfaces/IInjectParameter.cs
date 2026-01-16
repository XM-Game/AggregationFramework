// ==========================================================
// 文件名：IInjectParameter.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义注入参数接口，用于在注入时提供特定的参数值
// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 注入参数接口
    /// <para>定义在依赖注入过程中提供特定参数值的能力</para>
    /// <para>Injection parameter interface that defines the ability to provide specific parameter values during injection</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>策略模式：不同的参数匹配策略使用不同的实现</item>
    /// <item>单一职责：仅负责参数值的提供和匹配</item>
    /// </list>
    /// 
    /// 内置实现：
    /// <list type="bullet">
    /// <item>TypedParameter：按类型匹配参数</item>
    /// <item>NamedParameter：按名称匹配参数</item>
    /// <item>FactoryParameter：使用工厂函数提供参数值</item>
    /// </list>
    /// </remarks>
    public interface IInjectParameter
    {
        #region 参数信息 / Parameter Information

        /// <summary>
        /// 获取参数类型
        /// <para>Get the parameter type</para>
        /// </summary>
        /// <remarks>
        /// 用于按类型匹配参数。
        /// 如果为 null，则不按类型匹配。
        /// </remarks>
        Type ParameterType { get; }

        /// <summary>
        /// 获取参数名称
        /// <para>Get the parameter name</para>
        /// </summary>
        /// <remarks>
        /// 用于按名称匹配参数。
        /// 如果为 null 或空字符串，则不按名称匹配。
        /// </remarks>
        string ParameterName { get; }

        #endregion

        #region 参数匹配 / Parameter Matching

        /// <summary>
        /// 检查此参数是否匹配指定的参数信息
        /// <para>Check if this parameter matches the specified parameter information</para>
        /// </summary>
        /// <param name="parameterType">目标参数类型 / Target parameter type</param>
        /// <param name="parameterName">目标参数名称 / Target parameter name</param>
        /// <returns>是否匹配 / Whether matches</returns>
        bool CanSupply(Type parameterType, string parameterName);

        #endregion

        #region 值获取 / Value Retrieval

        /// <summary>
        /// 获取参数值
        /// <para>Get the parameter value</para>
        /// </summary>
        /// <param name="resolver">对象解析器，用于解析依赖 / Object resolver for resolving dependencies</param>
        /// <returns>参数值 / Parameter value</returns>
        /// <remarks>
        /// 对于工厂参数，此方法会调用工厂函数获取值。
        /// 对于普通参数，直接返回预设值。
        /// </remarks>
        object GetValue(IObjectResolver resolver);

        #endregion
    }

    /// <summary>
    /// 注入参数匹配模式
    /// <para>Injection parameter matching mode</para>
    /// </summary>
    public enum ParameterMatchMode
    {
        /// <summary>
        /// 按类型匹配
        /// <para>Match by type</para>
        /// </summary>
        ByType,

        /// <summary>
        /// 按名称匹配
        /// <para>Match by name</para>
        /// </summary>
        ByName,

        /// <summary>
        /// 按类型和名称同时匹配
        /// <para>Match by both type and name</para>
        /// </summary>
        ByTypeAndName
    }
}
