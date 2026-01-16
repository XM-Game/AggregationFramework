// ==========================================================
// 文件名：LifetimeValidator.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 生命周期验证器，验证依赖关系的生命周期合法性
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 生命周期验证器
    /// <para>验证服务依赖关系的生命周期合法性，防止常见的生命周期错误</para>
    /// <para>Lifetime validator that validates service dependency lifetime legality</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：专注于生命周期验证逻辑</item>
    /// <item>可配置：支持启用/禁用验证和自定义规则</item>
    /// <item>诊断友好：提供详细的验证错误信息</item>
    /// </list>
    /// 
    /// 验证规则：
    /// <list type="bullet">
    /// <item>Singleton 不应依赖 Scoped（捕获问题）</item>
    /// <item>Singleton 不应依赖 Transient（捕获问题）</item>
    /// <item>Scoped 不应依赖 Transient（捕获问题）</item>
    /// </list>
    /// 
    /// 捕获问题（Captive Dependency）：
    /// 当长生命周期服务持有短生命周期服务的引用时，
    /// 短生命周期服务的实例会被"捕获"，无法按预期释放或重建。
    /// </remarks>
    public sealed class LifetimeValidator
    {
        #region 内部类型 / Internal Types

        /// <summary>
        /// 验证结果
        /// </summary>
        public readonly struct ValidationResult
        {
            /// <summary>
            /// 是否验证通过
            /// </summary>
            public readonly bool IsValid;

            /// <summary>
            /// 验证消息
            /// </summary>
            public readonly string Message;

            /// <summary>
            /// 验证级别
            /// </summary>
            public readonly ValidationLevel Level;

            /// <summary>
            /// 消费者类型
            /// </summary>
            public readonly Type ConsumerType;

            /// <summary>
            /// 依赖类型
            /// </summary>
            public readonly Type DependencyType;

            public ValidationResult(
                bool isValid,
                string message,
                ValidationLevel level,
                Type consumerType = null,
                Type dependencyType = null)
            {
                IsValid = isValid;
                Message = message;
                Level = level;
                ConsumerType = consumerType;
                DependencyType = dependencyType;
            }

            /// <summary>
            /// 创建成功结果
            /// </summary>
            public static ValidationResult Success()
            {
                return new ValidationResult(true, null, ValidationLevel.None);
            }

            /// <summary>
            /// 创建警告结果
            /// </summary>
            public static ValidationResult Warning(string message, Type consumerType, Type dependencyType)
            {
                return new ValidationResult(false, message, ValidationLevel.Warning, consumerType, dependencyType);
            }

            /// <summary>
            /// 创建错误结果
            /// </summary>
            public static ValidationResult Error(string message, Type consumerType, Type dependencyType)
            {
                return new ValidationResult(false, message, ValidationLevel.Error, consumerType, dependencyType);
            }
        }

        /// <summary>
        /// 验证级别
        /// </summary>
        public enum ValidationLevel
        {
            /// <summary>
            /// 无问题
            /// </summary>
            None = 0,

            /// <summary>
            /// 警告（记录但不抛出异常）
            /// </summary>
            Warning = 1,

            /// <summary>
            /// 错误（抛出异常）
            /// </summary>
            Error = 2
        }

        /// <summary>
        /// 验证模式
        /// </summary>
        public enum ValidationMode
        {
            /// <summary>
            /// 禁用验证
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// 仅警告（记录日志但不抛出异常）
            /// </summary>
            WarnOnly = 1,

            /// <summary>
            /// 严格模式（抛出异常）
            /// </summary>
            Strict = 2
        }

        #endregion

        #region 字段 / Fields

        /// <summary>
        /// 是否启用验证
        /// </summary>
        private readonly bool _enabled;

        /// <summary>
        /// 验证模式
        /// </summary>
        private ValidationMode _mode;

        /// <summary>
        /// 验证历史记录
        /// </summary>
        private readonly List<ValidationResult> _validationHistory;

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 忽略的类型对
        /// </summary>
        private readonly HashSet<(Type, Type)> _ignoredPairs;

        /// <summary>
        /// 忽略的消费者类型
        /// </summary>
        private readonly HashSet<Type> _ignoredConsumerTypes;

        /// <summary>
        /// 忽略的依赖类型
        /// </summary>
        private readonly HashSet<Type> _ignoredDependencyTypes;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取是否启用验证
        /// <para>Get whether validation is enabled</para>
        /// </summary>
        public bool IsEnabled => _enabled;

        /// <summary>
        /// 获取或设置验证模式
        /// <para>Get or set the validation mode</para>
        /// </summary>
        public ValidationMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        /// <summary>
        /// 获取验证历史记录数量
        /// <para>Get the count of validation history</para>
        /// </summary>
        public int ValidationHistoryCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _validationHistory.Count;
                }
            }
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建生命周期验证器实例
        /// </summary>
        /// <param name="enabled">是否启用验证 / Whether to enable validation</param>
        /// <param name="mode">验证模式 / Validation mode</param>
        public LifetimeValidator(bool enabled = true, ValidationMode mode = ValidationMode.WarnOnly)
        {
            _enabled = enabled;
            _mode = mode;
            _validationHistory = new List<ValidationResult>();
            _ignoredPairs = new HashSet<(Type, Type)>();
            _ignoredConsumerTypes = new HashSet<Type>();
            _ignoredDependencyTypes = new HashSet<Type>();
        }

        #endregion

        #region 验证方法 / Validation Methods

        /// <summary>
        /// 验证依赖的生命周期
        /// <para>Validate dependency lifetime</para>
        /// </summary>
        /// <param name="consumerLifetime">消费者生命周期 / Consumer lifetime</param>
        /// <param name="dependencyLifetime">依赖生命周期 / Dependency lifetime</param>
        /// <param name="consumerType">消费者类型 / Consumer type</param>
        /// <param name="dependencyType">依赖类型 / Dependency type</param>
        /// <exception cref="LifetimeValidationException">当验证失败且模式为 Strict 时抛出</exception>
        public void ValidateDependency(
            Lifetime consumerLifetime,
            Lifetime dependencyLifetime,
            Type consumerType,
            Type dependencyType)
        {
            if (!_enabled || _mode == ValidationMode.Disabled)
                return;

            // 检查是否在忽略列表中
            if (IsIgnored(consumerType, dependencyType))
                return;

            var result = PerformValidation(consumerLifetime, dependencyLifetime, consumerType, dependencyType);

            if (!result.IsValid)
            {
                RecordValidation(result);
                HandleValidationFailure(result);
            }
        }

        /// <summary>
        /// 执行验证逻辑
        /// </summary>
        private ValidationResult PerformValidation(
            Lifetime consumerLifetime,
            Lifetime dependencyLifetime,
            Type consumerType,
            Type dependencyType)
        {
            // 规则1：Singleton 依赖 Scoped（严重问题）
            if (consumerLifetime == Lifetime.Singleton && dependencyLifetime == Lifetime.Scoped)
            {
                return ValidationResult.Error(
                    FormatCaptiveDependencyMessage(consumerType, dependencyType, consumerLifetime, dependencyLifetime,
                        "单例服务持有作用域服务的引用会导致作用域服务无法正确释放。\n" +
                        "Singleton holding Scoped reference prevents proper scope disposal."),
                    consumerType,
                    dependencyType);
            }

            // 规则2：Singleton 依赖 Transient（潜在问题）
            if (consumerLifetime == Lifetime.Singleton && dependencyLifetime == Lifetime.Transient)
            {
                return ValidationResult.Warning(
                    FormatCaptiveDependencyMessage(consumerType, dependencyType, consumerLifetime, dependencyLifetime,
                        "单例服务持有瞬态服务的引用会导致瞬态服务变成事实上的单例。\n" +
                        "Singleton holding Transient reference makes Transient effectively a singleton."),
                    consumerType,
                    dependencyType);
            }

            // 规则3：Scoped 依赖 Transient（潜在问题）
            if (consumerLifetime == Lifetime.Scoped && dependencyLifetime == Lifetime.Transient)
            {
                return ValidationResult.Warning(
                    FormatCaptiveDependencyMessage(consumerType, dependencyType, consumerLifetime, dependencyLifetime,
                        "作用域服务持有瞬态服务的引用会导致瞬态服务在作用域内变成单例。\n" +
                        "Scoped holding Transient reference makes Transient effectively scoped."),
                    consumerType,
                    dependencyType);
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// 格式化捕获依赖消息
        /// </summary>
        private static string FormatCaptiveDependencyMessage(
            Type consumerType,
            Type dependencyType,
            Lifetime consumerLifetime,
            Lifetime dependencyLifetime,
            string additionalInfo)
        {
            return $"[捕获依赖警告 / Captive Dependency Warning]\n" +
                   $"消费者 / Consumer: {consumerType?.FullName ?? "Unknown"} ({consumerLifetime})\n" +
                   $"依赖 / Dependency: {dependencyType?.FullName ?? "Unknown"} ({dependencyLifetime})\n" +
                   $"说明 / Description: {additionalInfo}\n" +
                   $"建议 / Suggestion: 考虑使用 Func<{dependencyType?.Name}> 或工厂模式来延迟解析。\n" +
                   $"Consider using Func<{dependencyType?.Name}> or factory pattern for lazy resolution.";
        }

        /// <summary>
        /// 处理验证失败
        /// </summary>
        private void HandleValidationFailure(ValidationResult result)
        {
            switch (_mode)
            {
                case ValidationMode.WarnOnly:
                    // 仅记录警告
                    if (result.Level == ValidationLevel.Error)
                    {
                        UnityEngine.Debug.LogError($"[AFramework.DI.LifetimeValidator] {result.Message}");
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"[AFramework.DI.LifetimeValidator] {result.Message}");
                    }
                    break;

                case ValidationMode.Strict:
                    // 抛出异常
                    throw new LifetimeValidationException(result.Message, result.ConsumerType, result.DependencyType);
            }
        }

        /// <summary>
        /// 记录验证结果
        /// </summary>
        private void RecordValidation(ValidationResult result)
        {
            lock (_syncRoot)
            {
                _validationHistory.Add(result);
            }
        }

        #endregion

        #region 忽略配置 / Ignore Configuration

        /// <summary>
        /// 忽略特定的类型对
        /// <para>Ignore a specific type pair</para>
        /// </summary>
        /// <param name="consumerType">消费者类型 / Consumer type</param>
        /// <param name="dependencyType">依赖类型 / Dependency type</param>
        public void IgnorePair(Type consumerType, Type dependencyType)
        {
            if (consumerType == null || dependencyType == null)
                return;

            lock (_syncRoot)
            {
                _ignoredPairs.Add((consumerType, dependencyType));
            }
        }

        /// <summary>
        /// 忽略特定的消费者类型
        /// <para>Ignore a specific consumer type</para>
        /// </summary>
        /// <param name="consumerType">消费者类型 / Consumer type</param>
        public void IgnoreConsumer(Type consumerType)
        {
            if (consumerType == null)
                return;

            lock (_syncRoot)
            {
                _ignoredConsumerTypes.Add(consumerType);
            }
        }

        /// <summary>
        /// 忽略特定的依赖类型
        /// <para>Ignore a specific dependency type</para>
        /// </summary>
        /// <param name="dependencyType">依赖类型 / Dependency type</param>
        public void IgnoreDependency(Type dependencyType)
        {
            if (dependencyType == null)
                return;

            lock (_syncRoot)
            {
                _ignoredDependencyTypes.Add(dependencyType);
            }
        }

        /// <summary>
        /// 检查是否在忽略列表中
        /// </summary>
        private bool IsIgnored(Type consumerType, Type dependencyType)
        {
            lock (_syncRoot)
            {
                if (_ignoredPairs.Contains((consumerType, dependencyType)))
                    return true;

                if (consumerType != null && _ignoredConsumerTypes.Contains(consumerType))
                    return true;

                if (dependencyType != null && _ignoredDependencyTypes.Contains(dependencyType))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 清除所有忽略配置
        /// <para>Clear all ignore configurations</para>
        /// </summary>
        public void ClearIgnoreList()
        {
            lock (_syncRoot)
            {
                _ignoredPairs.Clear();
                _ignoredConsumerTypes.Clear();
                _ignoredDependencyTypes.Clear();
            }
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取验证历史记录
        /// <para>Get validation history</para>
        /// </summary>
        /// <returns>验证结果集合 / Collection of validation results</returns>
        public IReadOnlyList<ValidationResult> GetValidationHistory()
        {
            lock (_syncRoot)
            {
                return _validationHistory.ToArray();
            }
        }

        /// <summary>
        /// 清除验证历史记录
        /// <para>Clear validation history</para>
        /// </summary>
        public void ClearHistory()
        {
            lock (_syncRoot)
            {
                _validationHistory.Clear();
            }
        }

        /// <summary>
        /// 获取诊断信息
        /// <para>Get diagnostic information</para>
        /// </summary>
        /// <returns>诊断信息字符串 / Diagnostic information string</returns>
        public string GetDiagnosticInfo()
        {
            lock (_syncRoot)
            {
                var warningCount = 0;
                var errorCount = 0;

                foreach (var result in _validationHistory)
                {
                    if (result.Level == ValidationLevel.Warning)
                        warningCount++;
                    else if (result.Level == ValidationLevel.Error)
                        errorCount++;
                }

                return $"LifetimeValidator[Enabled={_enabled}, Mode={_mode}, " +
                       $"Warnings={warningCount}, Errors={errorCount}, " +
                       $"IgnoredPairs={_ignoredPairs.Count}]";
            }
        }

        #endregion

        #region 静态辅助方法 / Static Helper Methods

        /// <summary>
        /// 检查生命周期是否兼容
        /// <para>Check if lifetimes are compatible</para>
        /// </summary>
        /// <param name="consumerLifetime">消费者生命周期 / Consumer lifetime</param>
        /// <param name="dependencyLifetime">依赖生命周期 / Dependency lifetime</param>
        /// <returns>是否兼容 / Whether compatible</returns>
        public static bool AreLifetimesCompatible(Lifetime consumerLifetime, Lifetime dependencyLifetime)
        {
            // 依赖的生命周期应该 >= 消费者的生命周期
            // Singleton(0) >= Scoped(1) >= Transient(2)
            return (int)dependencyLifetime <= (int)consumerLifetime;
        }

        /// <summary>
        /// 获取生命周期的优先级
        /// <para>Get lifetime priority</para>
        /// </summary>
        /// <param name="lifetime">生命周期 / Lifetime</param>
        /// <returns>优先级（数值越小，生命周期越长）/ Priority (lower value = longer lifetime)</returns>
        public static int GetLifetimePriority(Lifetime lifetime)
        {
            return (int)lifetime;
        }

        #endregion
    }

    #region 生命周期验证异常 / Lifetime Validation Exception

    /// <summary>
    /// 生命周期验证异常
    /// <para>当生命周期验证失败时抛出此异常</para>
    /// <para>Exception thrown when lifetime validation fails</para>
    /// </summary>
    [Serializable]
    public class LifetimeValidationException : DIException
    {
        /// <summary>
        /// 获取消费者类型
        /// <para>Get the consumer type</para>
        /// </summary>
        public Type ConsumerType { get; }

        /// <summary>
        /// 获取依赖类型
        /// <para>Get the dependency type</para>
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// 创建生命周期验证异常实例
        /// </summary>
        public LifetimeValidationException() : base()
        {
        }

        /// <summary>
        /// 创建带消息的生命周期验证异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public LifetimeValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带消息和类型信息的生命周期验证异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="consumerType">消费者类型 / Consumer type</param>
        /// <param name="dependencyType">依赖类型 / Dependency type</param>
        public LifetimeValidationException(string message, Type consumerType, Type dependencyType)
            : base(message)
        {
            ConsumerType = consumerType;
            DependencyType = dependencyType;
        }

        /// <summary>
        /// 创建带消息和内部异常的生命周期验证异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public LifetimeValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    #endregion
}
