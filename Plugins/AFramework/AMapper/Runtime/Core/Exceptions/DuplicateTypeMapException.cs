// ==========================================================
// 文件名：DuplicateTypeMapException.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Text
// 功能: 定义重复类型映射异常，在检测到重复配置时抛出
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.AMapper
{
    /// <summary>
    /// 重复类型映射异常
    /// <para>在检测到同一类型对的重复映射配置时抛出</para>
    /// <para>Exception thrown when duplicate type map configuration is detected</para>
    /// </summary>
    /// <remarks>
    /// DuplicateTypeMapException 在配置验证时抛出，
    /// 表示同一个源-目标类型对在多个地方被配置。
    /// 
    /// 常见触发场景：
    /// <list type="bullet">
    /// <item>同一类型对在多个 Profile 中配置</item>
    /// <item>同一 Profile 中重复调用 CreateMap</item>
    /// <item>AutoMap 特性与显式配置冲突</item>
    /// </list>
    /// 
    /// 解决方案：
    /// <list type="bullet">
    /// <item>移除重复的配置</item>
    /// <item>合并到单个 Profile 中</item>
    /// <item>使用 CreateMap().IncludeBase() 进行继承配置</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class DuplicateTypeMapException : AMapperException
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取重复的类型对
        /// <para>Get the duplicate type pair</para>
        /// </summary>
        public TypePair TypePair { get; }

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        public Type SourceType => TypePair.SourceType;

        /// <summary>
        /// 获取目标类型
        /// <para>Get the destination type</para>
        /// </summary>
        public Type DestinationType => TypePair.DestinationType;

        /// <summary>
        /// 获取包含重复配置的 Profile 类型列表
        /// <para>Get the list of profile types containing duplicate configuration</para>
        /// </summary>
        public IReadOnlyList<Type> ProfileTypes { get; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建重复类型映射异常实例
        /// </summary>
        public DuplicateTypeMapException() : base()
        {
            ProfileTypes = Array.Empty<Type>();
        }

        /// <summary>
        /// 创建带消息的重复类型映射异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public DuplicateTypeMapException(string message) : base(message)
        {
            ProfileTypes = Array.Empty<Type>();
        }

        /// <summary>
        /// 创建带类型对的重复类型映射异常实例
        /// </summary>
        /// <param name="typePair">重复的类型对 / Duplicate type pair</param>
        public DuplicateTypeMapException(TypePair typePair)
            : base(FormatMessage(typePair, null))
        {
            TypePair = typePair;
            ProfileTypes = Array.Empty<Type>();
        }

        /// <summary>
        /// 创建带类型对和 Profile 列表的重复类型映射异常实例
        /// </summary>
        /// <param name="typePair">重复的类型对 / Duplicate type pair</param>
        /// <param name="profileTypes">包含重复配置的 Profile 类型列表 / List of profile types</param>
        public DuplicateTypeMapException(TypePair typePair, IEnumerable<Type> profileTypes)
            : base(FormatMessage(typePair, profileTypes))
        {
            TypePair = typePair;
            ProfileTypes = profileTypes?.ToList() ?? new List<Type>();
        }

        /// <summary>
        /// 创建带源类型和目标类型的重复类型映射异常实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        public DuplicateTypeMapException(Type sourceType, Type destinationType)
            : this(new TypePair(sourceType, destinationType))
        {
        }

        /// <summary>
        /// 创建带消息和内部异常的重复类型映射异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public DuplicateTypeMapException(string message, Exception innerException) : base(message, innerException)
        {
            ProfileTypes = Array.Empty<Type>();
        }

        #endregion

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建重复类型映射异常
        /// <para>Create duplicate type map exception</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>重复类型映射异常 / Duplicate type map exception</returns>
        public static DuplicateTypeMapException Create<TSource, TDestination>()
        {
            return new DuplicateTypeMapException(TypePair.Create<TSource, TDestination>());
        }

        /// <summary>
        /// 创建带 Profile 信息的重复类型映射异常
        /// <para>Create duplicate type map exception with profile information</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="profileTypes">Profile 类型列表 / List of profile types</param>
        /// <returns>重复类型映射异常 / Duplicate type map exception</returns>
        public static DuplicateTypeMapException Create<TSource, TDestination>(IEnumerable<Type> profileTypes)
        {
            return new DuplicateTypeMapException(TypePair.Create<TSource, TDestination>(), profileTypes);
        }

        #endregion

        #region 私有方法 / Private Methods

        private static string FormatMessage(TypePair typePair, IEnumerable<Type> profileTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine("检测到重复的类型映射配置 / Duplicate type map configuration detected");
            sb.AppendLine();
            sb.AppendLine($"类型映射 / Type mapping: {typePair}");

            var profiles = profileTypes?.ToList();
            if (profiles != null && profiles.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("在以下 Profile 中发现重复配置 / Duplicate configuration found in following profiles:");
                foreach (var profile in profiles)
                {
                    sb.AppendLine($"  - {profile.FullName}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("解决方案 / Solutions:");
            sb.AppendLine("  1. 移除重复的 CreateMap 配置 / Remove duplicate CreateMap configuration");
            sb.AppendLine("  2. 将配置合并到单个 Profile 中 / Merge configuration into a single Profile");
            sb.AppendLine("  3. 如果需要继承配置，使用 IncludeBase() / Use IncludeBase() for inheritance configuration");

            return sb.ToString();
        }

        #endregion
    }
}
