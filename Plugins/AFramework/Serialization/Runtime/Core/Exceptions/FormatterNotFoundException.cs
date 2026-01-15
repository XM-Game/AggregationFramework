// ==========================================================
// 文件名：FormatterNotFoundException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 格式化器未找到异常
    /// <para>当序列化系统无法找到指定类型的格式化器时抛出</para>
    /// </summary>
    /// <remarks>
    /// 常见原因:
    /// <list type="bullet">
    /// <item>类型未标记 [Serializable] 特性</item>
    /// <item>自定义格式化器未注册</item>
    /// <item>泛型类型的格式化器未正确配置</item>
    /// <item>程序集未正确加载</item>
    /// </list>
    /// 
    /// 解决方案:
    /// <list type="number">
    /// <item>为类型添加 [Serializable] 特性</item>
    /// <item>手动注册格式化器: FormatterProvider.Register&lt;T&gt;(formatter)</item>
    /// <item>检查源代码生成器是否正确运行</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     var formatter = provider.GetFormatter&lt;MyType&gt;();
    /// }
    /// catch (FormatterNotFoundException ex)
    /// {
    ///     Console.WriteLine($"类型 {ex.TargetType.Name} 没有可用的格式化器");
    ///     Console.WriteLine($"建议: {ex.Suggestion}");
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class FormatterNotFoundException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息模板
        /// </summary>
        private const string DefaultMessageTemplate = "未找到类型 '{0}' 的格式化器";

        /// <summary>
        /// 默认建议消息
        /// </summary>
        private const string DefaultSuggestion = "请确保类型已标记 [Serializable] 特性，或手动注册格式化器";

        #endregion

        #region 字段

        /// <summary>
        /// 目标类型
        /// </summary>
        private readonly Type _targetType;

        /// <summary>
        /// 解决建议
        /// </summary>
        private readonly string _suggestion;

        /// <summary>
        /// 已尝试的解析器列表
        /// </summary>
        private readonly string[] _attemptedResolvers;

        #endregion

        #region 属性

        /// <summary>
        /// 获取目标类型
        /// </summary>
        /// <value>需要格式化器但未找到的类型</value>
        public Type TargetType => _targetType;

        /// <summary>
        /// 获取解决建议
        /// </summary>
        /// <value>如何解决此问题的建议</value>
        public string Suggestion => _suggestion;

        /// <summary>
        /// 获取已尝试的解析器列表
        /// </summary>
        /// <value>尝试查找格式化器时使用的解析器名称</value>
        public string[] AttemptedResolvers => _attemptedResolvers;

        #endregion

        #region 构造函数

        /// <summary>
        /// 使用目标类型创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        public FormatterNotFoundException(Type targetType)
            : this(targetType, null, null)
        {
        }

        /// <summary>
        /// 使用目标类型和自定义消息创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="message">自定义消息</param>
        public FormatterNotFoundException(Type targetType, string message)
            : this(targetType, message, null)
        {
        }

        /// <summary>
        /// 使用目标类型、消息和内部异常创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="message">自定义消息</param>
        /// <param name="innerException">内部异常</param>
        public FormatterNotFoundException(Type targetType, string message, Exception innerException)
            : base(
                SerializeErrorCode.FormatterNotFound,
                message ?? string.Format(DefaultMessageTemplate, targetType?.FullName ?? "null"),
                targetType,
                null,
                -1,
                innerException)
        {
            _targetType = targetType;
            _suggestion = GenerateSuggestion(targetType);
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="message">自定义消息</param>
        /// <param name="attemptedResolvers">已尝试的解析器</param>
        /// <param name="innerException">内部异常</param>
        public FormatterNotFoundException(
            Type targetType,
            string message,
            string[] attemptedResolvers,
            Exception innerException = null)
            : base(
                SerializeErrorCode.FormatterNotFound,
                message ?? string.Format(DefaultMessageTemplate, targetType?.FullName ?? "null"),
                targetType,
                null,
                -1,
                innerException)
        {
            _targetType = targetType;
            _attemptedResolvers = attemptedResolvers;
            _suggestion = GenerateSuggestion(targetType);
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private FormatterNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var typeName = info.GetString(nameof(TargetType));
            if (!string.IsNullOrEmpty(typeName))
            {
                _targetType = Type.GetType(typeName);
            }
            _suggestion = info.GetString(nameof(Suggestion));
            _attemptedResolvers = (string[])info.GetValue(nameof(AttemptedResolvers), typeof(string[]));
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(TargetType), _targetType?.AssemblyQualifiedName);
            info.AddValue(nameof(Suggestion), _suggestion);
            info.AddValue(nameof(AttemptedResolvers), _attemptedResolvers);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 根据类型生成解决建议
        /// </summary>
        private static string GenerateSuggestion(Type type)
        {
            if (type == null)
            {
                return "请提供有效的类型参数";
            }

            var builder = new System.Text.StringBuilder();

            // 检查是否为泛型类型
            if (type.IsGenericType)
            {
                builder.AppendLine("泛型类型需要特殊处理:");
                builder.AppendLine("  1. 确保泛型参数类型也有对应的格式化器");
                builder.AppendLine("  2. 考虑使用 [CustomFormatter] 特性指定格式化器");
            }
            // 检查是否为接口
            else if (type.IsInterface)
            {
                builder.AppendLine("接口类型需要多态支持:");
                builder.AppendLine("  1. 使用 [UnionType] 特性注册具体实现类型");
                builder.AppendLine("  2. 或使用 FormatterProvider.RegisterInterface<T>()");
            }
            // 检查是否为抽象类
            else if (type.IsAbstract)
            {
                builder.AppendLine("抽象类型需要多态支持:");
                builder.AppendLine("  1. 使用 [UnionType] 特性注册派生类型");
                builder.AppendLine("  2. 或使用 FormatterProvider.RegisterAbstract<T>()");
            }
            // 检查是否为数组
            else if (type.IsArray)
            {
                builder.AppendLine("数组类型需要元素类型的格式化器:");
                builder.AppendLine($"  确保 {type.GetElementType()?.Name} 类型有对应的格式化器");
            }
            // 普通类型
            else
            {
                builder.AppendLine("请尝试以下解决方案:");
                builder.AppendLine("  1. 为类型添加 [Serializable] 特性");
                builder.AppendLine("  2. 手动注册: FormatterProvider.Register<T>(formatter)");
                builder.AppendLine("  3. 检查源代码生成器是否正确运行");
                builder.AppendLine("  4. 确保类型是 public 的");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取详细的诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 格式化器未找到诊断信息 ===");
            builder.AppendLine($"目标类型: {_targetType?.FullName ?? "null"}");
            
            if (_targetType != null)
            {
                builder.AppendLine($"  - 是否泛型: {_targetType.IsGenericType}");
                builder.AppendLine($"  - 是否接口: {_targetType.IsInterface}");
                builder.AppendLine($"  - 是否抽象: {_targetType.IsAbstract}");
                builder.AppendLine($"  - 是否数组: {_targetType.IsArray}");
                builder.AppendLine($"  - 是否公开: {_targetType.IsPublic}");
                builder.AppendLine($"  - 程序集: {_targetType.Assembly.GetName().Name}");
            }

            if (_attemptedResolvers != null && _attemptedResolvers.Length > 0)
            {
                builder.AppendLine("已尝试的解析器:");
                foreach (var resolver in _attemptedResolvers)
                {
                    builder.AppendLine($"  - {resolver}");
                }
            }

            builder.AppendLine("建议:");
            builder.AppendLine(_suggestion);

            return builder.ToString();
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 为泛型类型创建异常
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>格式化器未找到异常</returns>
        public static FormatterNotFoundException ForType<T>()
        {
            return new FormatterNotFoundException(typeof(T));
        }

        /// <summary>
        /// 为泛型类型创建异常（带解析器信息）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="attemptedResolvers">已尝试的解析器</param>
        /// <returns>格式化器未找到异常</returns>
        public static FormatterNotFoundException ForType<T>(params string[] attemptedResolvers)
        {
            return new FormatterNotFoundException(typeof(T), null, attemptedResolvers);
        }

        #endregion
    }
}
