// ==========================================================
// 文件名：ConfigurationException.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Text
// 功能: 定义配置验证异常，在配置无效时抛出
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.AMapper
{
    /// <summary>
    /// 配置验证异常
    /// <para>在映射配置验证失败时抛出</para>
    /// <para>Exception thrown when mapping configuration validation fails</para>
    /// </summary>
    /// <remarks>
    /// ConfigurationException 在调用 AssertConfigurationIsValid() 时抛出，
    /// 包含所有配置错误的详细信息。
    /// 
    /// 常见触发场景：
    /// <list type="bullet">
    /// <item>目标成员没有映射源</item>
    /// <item>构造函数参数无法解析</item>
    /// <item>类型转换不可行</item>
    /// <item>循环依赖检测</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// try
    /// {
    ///     configuration.AssertConfigurationIsValid();
    /// }
    /// catch (ConfigurationException ex)
    /// {
    ///     foreach (var error in ex.Errors)
    ///     {
    ///         Debug.LogError(error);
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public class ConfigurationException : AMapperException
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取配置错误列表
        /// <para>Get the list of configuration errors</para>
        /// </summary>
        public IReadOnlyList<ConfigurationError> Errors { get; }

        /// <summary>
        /// 获取相关的类型映射
        /// <para>Get the related type maps</para>
        /// </summary>
        public IReadOnlyList<ITypeMap> TypeMaps { get; }

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建配置异常实例
        /// </summary>
        public ConfigurationException() : base()
        {
            Errors = Array.Empty<ConfigurationError>();
            TypeMaps = Array.Empty<ITypeMap>();
        }

        /// <summary>
        /// 创建带消息的配置异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public ConfigurationException(string message) : base(message)
        {
            Errors = Array.Empty<ConfigurationError>();
            TypeMaps = Array.Empty<ITypeMap>();
        }

        /// <summary>
        /// 创建带错误列表的配置异常实例
        /// </summary>
        /// <param name="errors">配置错误列表 / List of configuration errors</param>
        public ConfigurationException(IEnumerable<ConfigurationError> errors)
            : base(FormatMessage(errors))
        {
            Errors = errors?.ToList() ?? new List<ConfigurationError>();
            TypeMaps = Errors.Where(e => e.TypeMap != null).Select(e => e.TypeMap).Distinct().ToList();
        }

        /// <summary>
        /// 创建带类型映射的配置异常实例
        /// </summary>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <param name="unmappedMembers">未映射的成员列表 / List of unmapped members</param>
        public ConfigurationException(ITypeMap typeMap, IEnumerable<string> unmappedMembers)
            : base(FormatUnmappedMembersMessage(typeMap, unmappedMembers))
        {
            var errors = unmappedMembers?.Select(m => new ConfigurationError(typeMap, m, ConfigurationErrorType.UnmappedMember)).ToList()
                ?? new List<ConfigurationError>();
            Errors = errors;
            TypeMaps = typeMap != null ? new[] { typeMap } : Array.Empty<ITypeMap>();
        }

        /// <summary>
        /// 创建带消息和内部异常的配置异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = Array.Empty<ConfigurationError>();
            TypeMaps = Array.Empty<ITypeMap>();
        }

        #endregion

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建未映射成员异常
        /// <para>Create unmapped members exception</para>
        /// </summary>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <param name="unmappedMembers">未映射的成员列表 / List of unmapped members</param>
        /// <returns>配置异常 / Configuration exception</returns>
        public static ConfigurationException UnmappedMembers(ITypeMap typeMap, IEnumerable<string> unmappedMembers)
        {
            return new ConfigurationException(typeMap, unmappedMembers);
        }

        /// <summary>
        /// 创建无法构造目标类型异常
        /// <para>Create cannot construct destination exception</para>
        /// </summary>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <returns>配置异常 / Configuration exception</returns>
        public static ConfigurationException CannotConstructDestination(ITypeMap typeMap)
        {
            var message = $"无法构造目标类型 / Cannot construct destination type: {typeMap?.DestinationType?.Name ?? "null"}\n" +
                          $"类型映射 / Type mapping: {typeMap?.SourceType?.Name ?? "null"} -> {typeMap?.DestinationType?.Name ?? "null"}\n" +
                          "请确保目标类型有可用的构造函数，或使用 ConstructUsing 配置自定义构造逻辑。\n" +
                          "Please ensure the destination type has an available constructor, or use ConstructUsing to configure custom construction logic.";
            return new ConfigurationException(message);
        }

        /// <summary>
        /// 创建类型转换不可行异常
        /// <para>Create type conversion not possible exception</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>配置异常 / Configuration exception</returns>
        public static ConfigurationException TypeConversionNotPossible(Type sourceType, Type destinationType)
        {
            var message = $"类型转换不可行 / Type conversion not possible: {sourceType?.Name ?? "null"} -> {destinationType?.Name ?? "null"}\n" +
                          "请配置 CreateMap 或使用 ConvertUsing 指定转换逻辑。\n" +
                          "Please configure CreateMap or use ConvertUsing to specify conversion logic.";
            return new ConfigurationException(message);
        }

        #endregion

        #region 私有方法 / Private Methods

        private static string FormatMessage(IEnumerable<ConfigurationError> errors)
        {
            var errorList = errors?.ToList() ?? new List<ConfigurationError>();
            if (errorList.Count == 0)
            {
                return "映射配置验证失败 / Mapping configuration validation failed";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"映射配置验证失败，发现 {errorList.Count} 个错误 / Mapping configuration validation failed with {errorList.Count} error(s)");
            sb.AppendLine();

            // 按类型映射分组显示错误
            var groupedErrors = errorList.GroupBy(e => e.TypeMap?.TypePair);
            foreach (var group in groupedErrors)
            {
                if (group.Key.HasValue)
                {
                    sb.AppendLine($"类型映射 / Type mapping: {group.Key.Value}");
                }

                foreach (var error in group)
                {
                    sb.AppendLine($"  - {error}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string FormatUnmappedMembersMessage(ITypeMap typeMap, IEnumerable<string> unmappedMembers)
        {
            var members = unmappedMembers?.ToList() ?? new List<string>();
            var sb = new StringBuilder();
            sb.AppendLine("存在未映射的目标成员 / Unmapped destination members found");
            sb.AppendLine();
            sb.AppendLine($"类型映射 / Type mapping: {typeMap?.SourceType?.Name ?? "null"} -> {typeMap?.DestinationType?.Name ?? "null"}");
            sb.AppendLine();
            sb.AppendLine("未映射成员 / Unmapped members:");
            foreach (var member in members)
            {
                sb.AppendLine($"  - {member}");
            }
            sb.AppendLine();
            sb.AppendLine("解决方案 / Solutions:");
            sb.AppendLine("  1. 在源类型中添加同名属性 / Add a property with the same name in source type");
            sb.AppendLine("  2. 使用 ForMember().MapFrom() 指定映射来源 / Use ForMember().MapFrom() to specify mapping source");
            sb.AppendLine("  3. 使用 ForMember().Ignore() 忽略该成员 / Use ForMember().Ignore() to ignore the member");

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// 配置错误
    /// <para>Configuration error</para>
    /// </summary>
    public class ConfigurationError
    {
        /// <summary>
        /// 获取相关的类型映射
        /// <para>Get the related type map</para>
        /// </summary>
        public ITypeMap TypeMap { get; }

        /// <summary>
        /// 获取相关的成员名称
        /// <para>Get the related member name</para>
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// 获取错误类型
        /// <para>Get the error type</para>
        /// </summary>
        public ConfigurationErrorType ErrorType { get; }

        /// <summary>
        /// 获取错误消息
        /// <para>Get the error message</para>
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 创建配置错误实例
        /// </summary>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <param name="memberName">成员名称 / Member name</param>
        /// <param name="errorType">错误类型 / Error type</param>
        /// <param name="message">错误消息 / Error message</param>
        public ConfigurationError(ITypeMap typeMap, string memberName, ConfigurationErrorType errorType, string message = null)
        {
            TypeMap = typeMap;
            MemberName = memberName;
            ErrorType = errorType;
            Message = message ?? GetDefaultMessage(errorType, memberName);
        }

        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"[{ErrorType}] {MemberName}: {Message}";
        }

        private static string GetDefaultMessage(ConfigurationErrorType errorType, string memberName)
        {
            return errorType switch
            {
                ConfigurationErrorType.UnmappedMember => $"成员 '{memberName}' 没有映射源 / Member '{memberName}' has no mapping source",
                ConfigurationErrorType.UnresolvedConstructorParameter => $"构造函数参数 '{memberName}' 无法解析 / Constructor parameter '{memberName}' cannot be resolved",
                ConfigurationErrorType.TypeConversionNotPossible => $"成员 '{memberName}' 的类型转换不可行 / Type conversion for member '{memberName}' is not possible",
                ConfigurationErrorType.CircularReference => $"检测到循环引用 / Circular reference detected",
                _ => $"配置错误 / Configuration error"
            };
        }
    }

    /// <summary>
    /// 配置错误类型
    /// <para>Configuration error type</para>
    /// </summary>
    public enum ConfigurationErrorType
    {
        /// <summary>
        /// 未映射的成员
        /// <para>Unmapped member</para>
        /// </summary>
        UnmappedMember,

        /// <summary>
        /// 无法解析的构造函数参数
        /// <para>Unresolved constructor parameter</para>
        /// </summary>
        UnresolvedConstructorParameter,

        /// <summary>
        /// 类型转换不可行
        /// <para>Type conversion not possible</para>
        /// </summary>
        TypeConversionNotPossible,

        /// <summary>
        /// 循环引用
        /// <para>Circular reference</para>
        /// </summary>
        CircularReference,

        /// <summary>
        /// 其他错误
        /// <para>Other error</para>
        /// </summary>
        Other
    }
}
