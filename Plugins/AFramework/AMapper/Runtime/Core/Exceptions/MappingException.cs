// ==========================================================
// 文件名：MappingException.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Text
// 功能: 定义映射执行异常，在映射过程中发生错误时抛出
// ==========================================================

using System;
using System.Text;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射执行异常
    /// <para>在映射执行过程中发生错误时抛出</para>
    /// <para>Exception thrown when an error occurs during mapping execution</para>
    /// </summary>
    /// <remarks>
    /// MappingException 包含详细的映射上下文信息，便于调试和错误定位。
    /// 
    /// 包含信息：
    /// <list type="bullet">
    /// <item>源类型和目标类型</item>
    /// <item>出错的成员映射配置</item>
    /// <item>原始异常（InnerException）</item>
    /// </list>
    /// 
    /// 常见触发场景：
    /// <list type="bullet">
    /// <item>类型转换失败</item>
    /// <item>值解析器抛出异常</item>
    /// <item>构造函数调用失败</item>
    /// <item>空引用访问</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class MappingException : AMapperException
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取源类型
        /// <para>Get the source type</para>
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// 获取目标类型
        /// <para>Get the destination type</para>
        /// </summary>
        public Type DestinationType { get; }

        /// <summary>
        /// 获取类型对
        /// <para>Get the type pair</para>
        /// </summary>
        public TypePair? TypePair { get; }

        /// <summary>
        /// 获取出错的成员映射配置
        /// <para>Get the member map where error occurred</para>
        /// </summary>
        public IMemberMap MemberMap { get; }

        /// <summary>
        /// 获取出错的成员名称
        /// <para>Get the member name where error occurred</para>
        /// </summary>
        public string MemberName { get; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射异常实例
        /// </summary>
        public MappingException() : base()
        {
        }

        /// <summary>
        /// 创建带消息的映射异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public MappingException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带消息和内部异常的映射异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// 创建带类型信息的映射异常实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public MappingException(Type sourceType, Type destinationType, Exception innerException)
            : base(FormatMessage(sourceType, destinationType, null, innerException), innerException)
        {
            SourceType = sourceType;
            DestinationType = destinationType;
            TypePair = sourceType != null && destinationType != null
                ? new TypePair(sourceType, destinationType)
                : (TypePair?)null;
        }

        /// <summary>
        /// 创建带成员信息的映射异常实例
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="memberName">成员名称 / Member name</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public MappingException(Type sourceType, Type destinationType, string memberName, Exception innerException)
            : base(FormatMessage(sourceType, destinationType, memberName, innerException), innerException)
        {
            SourceType = sourceType;
            DestinationType = destinationType;
            MemberName = memberName;
            TypePair = sourceType != null && destinationType != null
                ? new TypePair(sourceType, destinationType)
                : (TypePair?)null;
        }

        /// <summary>
        /// 创建带成员映射配置的映射异常实例
        /// </summary>
        /// <param name="memberMap">成员映射配置 / Member map</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public MappingException(IMemberMap memberMap, Exception innerException)
            : base(FormatMessage(memberMap, innerException), innerException)
        {
            MemberMap = memberMap;
            MemberName = memberMap?.DestinationName;
            SourceType = memberMap?.TypeMap?.SourceType;
            DestinationType = memberMap?.TypeMap?.DestinationType;
            TypePair = SourceType != null && DestinationType != null
                ? new TypePair(SourceType, DestinationType)
                : (TypePair?)null;
        }

        #endregion

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建类型转换失败异常
        /// <para>Create type conversion failure exception</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        /// <returns>映射异常 / Mapping exception</returns>
        public static MappingException TypeConversionFailed(Type sourceType, Type destinationType, Exception innerException = null)
        {
            var message = $"类型转换失败 / Type conversion failed: {sourceType?.Name ?? "null"} -> {destinationType?.Name ?? "null"}";
            return new MappingException(message, innerException)
            {
            };
        }

        /// <summary>
        /// 创建空源对象异常
        /// <para>Create null source exception</para>
        /// </summary>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>映射异常 / Mapping exception</returns>
        public static MappingException NullSource(Type destinationType)
        {
            var message = $"源对象为空，无法映射到 {destinationType?.Name ?? "null"} / Source object is null, cannot map to {destinationType?.Name ?? "null"}";
            return new MappingException(message);
        }

        #endregion

        #region 私有方法 / Private Methods

        private static string FormatMessage(Type sourceType, Type destinationType, string memberName, Exception innerException)
        {
            var sb = new StringBuilder();
            sb.AppendLine("映射执行失败 / Mapping execution failed");
            sb.AppendLine();

            if (sourceType != null || destinationType != null)
            {
                sb.AppendLine($"类型映射 / Type mapping: {sourceType?.Name ?? "null"} -> {destinationType?.Name ?? "null"}");
            }

            if (!string.IsNullOrEmpty(memberName))
            {
                sb.AppendLine($"成员 / Member: {memberName}");
            }

            if (innerException != null)
            {
                sb.AppendLine();
                sb.AppendLine($"原因 / Cause: {innerException.Message}");
            }

            return sb.ToString();
        }

        private static string FormatMessage(IMemberMap memberMap, Exception innerException)
        {
            if (memberMap == null)
            {
                return innerException?.Message ?? "映射执行失败 / Mapping execution failed";
            }

            var sb = new StringBuilder();
            sb.AppendLine("成员映射执行失败 / Member mapping execution failed");
            sb.AppendLine();
            sb.AppendLine($"类型映射 / Type mapping: {memberMap.TypeMap?.SourceType?.Name ?? "null"} -> {memberMap.TypeMap?.DestinationType?.Name ?? "null"}");
            sb.AppendLine($"目标成员 / Destination member: {memberMap.DestinationName}");

            if (!string.IsNullOrEmpty(memberMap.SourceMemberName))
            {
                sb.AppendLine($"源成员 / Source member: {memberMap.SourceMemberName}");
            }

            if (innerException != null)
            {
                sb.AppendLine();
                sb.AppendLine($"原因 / Cause: {innerException.Message}");
            }

            return sb.ToString();
        }

        #endregion
    }
}
