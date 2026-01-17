// ==========================================================
// 文件名：ConfigurationValidator.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Linq
// 功能: 配置验证器，验证映射配置的有效性
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.AMapper
{
    /// <summary>
    /// 配置验证器
    /// <para>验证映射配置的有效性</para>
    /// <para>Configuration validator for validating mapping configuration</para>
    /// </summary>
    public sealed class ConfigurationValidator
    {
        #region 私有字段 / Private Fields

        private readonly IMapperConfiguration _configuration;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建配置验证器实例
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        public ConfigurationValidator(IMapperConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 验证所有配置
        /// <para>Validate all configurations</para>
        /// </summary>
        /// <returns>验证结果 / Validation result</returns>
        public ValidationResult Validate()
        {
            var context = new ValidationContext();

            foreach (var typeMap in _configuration.GetAllTypeMaps())
            {
                ValidateTypeMap(typeMap, context);
            }

            return new ValidationResult(context.Errors);
        }

        /// <summary>
        /// 验证指定类型映射
        /// <para>Validate specified type map</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>验证结果 / Validation result</returns>
        public ValidationResult Validate<TSource, TDestination>()
        {
            var context = new ValidationContext();
            var typeMap = _configuration.FindTypeMap<TSource, TDestination>();

            if (typeMap == null)
            {
                context.AddError(new ConfigurationError(null, null, ConfigurationErrorType.Other,
                    $"未找到类型映射 / Type map not found: {typeof(TSource).Name} -> {typeof(TDestination).Name}"));
            }
            else
            {
                ValidateTypeMap(typeMap, context);
            }

            return new ValidationResult(context.Errors);
        }

        /// <summary>
        /// 断言配置有效
        /// <para>Assert configuration is valid</para>
        /// </summary>
        /// <exception cref="ConfigurationException">当配置无效时抛出</exception>
        public void AssertConfigurationIsValid()
        {
            var result = Validate();
            if (!result.IsValid)
            {
                throw new ConfigurationException(result.Errors);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        private void ValidateTypeMap(ITypeMap typeMap, ValidationContext context)
        {
            // 验证未映射的成员
            ValidateUnmappedMembers(typeMap, context);

            // 验证构造函数
            ValidateConstructor(typeMap, context);

            // 验证成员映射
            ValidateMemberMaps(typeMap, context);
        }

        private void ValidateUnmappedMembers(ITypeMap typeMap, ValidationContext context)
        {
            foreach (var memberMap in typeMap.MemberMaps)
            {
                if (!memberMap.IsMapped && !memberMap.IsIgnored)
                {
                    context.AddError(new ConfigurationError(
                        typeMap,
                        memberMap.DestinationName,
                        ConfigurationErrorType.UnmappedMember));
                }
            }
        }

        private void ValidateConstructor(ITypeMap typeMap, ValidationContext context)
        {
            if (typeMap.DisableConstructorMapping || typeMap.HasCustomConstruction)
                return;

            var constructorMap = typeMap.ConstructorMap;
            if (constructorMap == null)
            {
                // 检查是否有默认构造函数
                var defaultCtor = typeMap.DestinationType.GetConstructor(Type.EmptyTypes);
                if (defaultCtor == null)
                {
                    context.AddError(new ConfigurationError(
                        typeMap,
                        null,
                        ConfigurationErrorType.Other,
                        $"目标类型没有可用的构造函数 / Destination type has no available constructor: {typeMap.DestinationType.Name}"));
                }
                return;
            }

            if (!constructorMap.CanResolve)
            {
                foreach (var paramMap in constructorMap.ParameterMaps)
                {
                    if (!paramMap.IsMapped)
                    {
                        context.AddError(new ConfigurationError(
                            typeMap,
                            paramMap.ParameterName,
                            ConfigurationErrorType.UnresolvedConstructorParameter));
                    }
                }
            }
        }

        private void ValidateMemberMaps(ITypeMap typeMap, ValidationContext context)
        {
            foreach (var memberMap in typeMap.MemberMaps)
            {
                if (memberMap.IsIgnored)
                    continue;

                // 验证类型兼容性
                if (memberMap.SourceType != null && memberMap.DestinationType != null)
                {
                    if (!IsTypeCompatible(memberMap.SourceType, memberMap.DestinationType))
                    {
                        // 检查是否有嵌套映射
                        var nestedMap = _configuration.FindTypeMap(memberMap.SourceType, memberMap.DestinationType);
                        if (nestedMap == null && !memberMap.HasValueConverter && !memberMap.HasCustomResolver)
                        {
                            context.AddError(new ConfigurationError(
                                typeMap,
                                memberMap.DestinationName,
                                ConfigurationErrorType.TypeConversionNotPossible,
                                $"类型转换不可行 / Type conversion not possible: {memberMap.SourceType.Name} -> {memberMap.DestinationType.Name}"));
                        }
                    }
                }
            }
        }

        private static bool IsTypeCompatible(Type sourceType, Type destinationType)
        {
            // 直接赋值兼容
            if (destinationType.IsAssignableFrom(sourceType))
                return true;

            // 可空类型处理
            var underlyingSource = System.Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var underlyingDest = System.Nullable.GetUnderlyingType(destinationType) ?? destinationType;

            if (underlyingDest.IsAssignableFrom(underlyingSource))
                return true;

            // 基本类型转换
            if (IsConvertible(underlyingSource) && IsConvertible(underlyingDest))
                return true;

            // 字符串转换
            if (destinationType == typeof(string))
                return true;

            return false;
        }

        private static bool IsConvertible(Type type)
        {
            return type.IsPrimitive || type == typeof(decimal) || type == typeof(string) ||
                   type == typeof(DateTime) || type == typeof(TimeSpan) || type.IsEnum;
        }

        #endregion
    }

    /// <summary>
    /// 验证上下文
    /// <para>Validation context</para>
    /// </summary>
    public sealed class ValidationContext
    {
        private readonly List<ConfigurationError> _errors = new List<ConfigurationError>();

        /// <summary>
        /// 获取错误列表
        /// <para>Get error list</para>
        /// </summary>
        public IReadOnlyList<ConfigurationError> Errors => _errors;

        /// <summary>
        /// 添加错误
        /// <para>Add error</para>
        /// </summary>
        public void AddError(ConfigurationError error)
        {
            _errors.Add(error);
        }
    }

    /// <summary>
    /// 验证结果
    /// <para>Validation result</para>
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// 获取错误列表
        /// <para>Get error list</para>
        /// </summary>
        public IReadOnlyList<ConfigurationError> Errors { get; }

        /// <summary>
        /// 获取是否有效
        /// <para>Get whether valid</para>
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// 创建验证结果
        /// </summary>
        public ValidationResult(IEnumerable<ConfigurationError> errors)
        {
            Errors = errors?.ToList() ?? new List<ConfigurationError>();
        }
    }
}
