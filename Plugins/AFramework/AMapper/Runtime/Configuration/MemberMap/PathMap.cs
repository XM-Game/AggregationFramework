// ==========================================================
// 文件名：PathMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 路径映射配置，用于嵌套属性的映射（ForPath）
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 路径映射配置
    /// <para>用于嵌套属性的映射配置（ForPath）</para>
    /// <para>Path map configuration for nested property mapping (ForPath)</para>
    /// </summary>
    /// <remarks>
    /// PathMap 用于配置嵌套属性的映射，例如：
    /// <code>
    /// CreateMap&lt;Source, Dest&gt;()
    ///     .ForPath(d => d.Customer.Address.City, opt => opt.MapFrom(s => s.City));
    /// </code>
    /// </remarks>
    public sealed class PathMap
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取所属的类型映射
        /// <para>Get the parent type map</para>
        /// </summary>
        public ITypeMap TypeMap { get; }

        /// <summary>
        /// 获取目标路径成员链
        /// <para>Get destination path member chain</para>
        /// </summary>
        public MemberInfo[] DestinationPath { get; }

        /// <summary>
        /// 获取目标路径字符串表示
        /// <para>Get destination path string representation</para>
        /// </summary>
        public string DestinationPathString { get; }

        /// <summary>
        /// 获取最终目标成员类型
        /// <para>Get final destination member type</para>
        /// </summary>
        public Type DestinationType { get; }

        /// <summary>
        /// 获取自定义映射表达式
        /// <para>Get custom map expression</para>
        /// </summary>
        public LambdaExpression CustomMapExpression { get; private set; }

        /// <summary>
        /// 获取是否忽略
        /// <para>Get whether ignored</para>
        /// </summary>
        public bool IsIgnored { get; private set; }

        /// <summary>
        /// 获取条件表达式
        /// <para>Get condition expression</para>
        /// </summary>
        public LambdaExpression ConditionExpression { get; private set; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建路径映射实例
        /// </summary>
        /// <param name="typeMap">所属类型映射 / Parent type map</param>
        /// <param name="destinationPath">目标路径 / Destination path</param>
        internal PathMap(ITypeMap typeMap, MemberInfo[] destinationPath)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
            
            if (destinationPath == null || destinationPath.Length == 0)
                throw new ArgumentException("目标路径不能为空 / Destination path cannot be empty", nameof(destinationPath));

            DestinationPath = destinationPath;
            DestinationPathString = string.Join(".", destinationPath.Select(m => m.Name));
            DestinationType = GetMemberType(destinationPath[destinationPath.Length - 1]);
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 设置自定义映射表达式
        /// </summary>
        internal void SetCustomMapExpression(LambdaExpression expression)
        {
            CustomMapExpression = expression;
        }

        /// <summary>
        /// 设置忽略
        /// </summary>
        internal void SetIgnored()
        {
            IsIgnored = true;
        }

        /// <summary>
        /// 设置条件表达式
        /// </summary>
        internal void SetCondition(LambdaExpression condition)
        {
            ConditionExpression = condition;
        }

        #endregion

        #region 私有方法 / Private Methods

        private static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                _ => throw new ArgumentException($"不支持的成员类型 / Unsupported member type: {member.MemberType}")
            };
        }

        #endregion
    }
}
