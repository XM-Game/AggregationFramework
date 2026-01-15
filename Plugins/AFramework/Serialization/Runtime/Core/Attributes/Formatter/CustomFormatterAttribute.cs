// ==========================================================
// 文件名：CustomFormatterAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 自定义格式化器特性
    /// <para>指定字段或类型使用自定义的格式化器进行序列化</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>为特定字段或类型指定自定义格式化器</item>
    ///   <item>支持泛型格式化器类型</item>
    ///   <item>支持格式化器参数配置</item>
    ///   <item>支持格式化器优先级设置</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class GameData
    /// {
    ///     // 使用自定义压缩格式化器
    ///     [CustomFormatter(typeof(CompressedIntFormatter))]
    ///     public int Score;
    ///     
    ///     // 使用带参数的格式化器
    ///     [CustomFormatter(typeof(FixedPointFormatter), Arguments = new object[] { 2 })]
    ///     public float Position;
    /// }
    /// </code>
    /// 
    /// <para><b>格式化器要求：</b></para>
    /// <list type="number">
    ///   <item>必须实现 IFormatter&lt;T&gt; 接口</item>
    ///   <item>必须有无参构造函数或接受 Arguments 的构造函数</item>
    ///   <item>泛型格式化器的类型参数数量必须匹配</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class CustomFormatterAttribute : Attribute
    {
        #region 常量

        /// <summary>默认优先级</summary>
        public const int DefaultPriority = 100;

        /// <summary>最高优先级</summary>
        public const int HighestPriority = 0;

        /// <summary>最低优先级</summary>
        public const int LowestPriority = int.MaxValue;

        #endregion

        #region 字段

        private readonly Type _formatterType;
        private object[] _arguments;
        private int _priority = DefaultPriority;
        private bool _cacheInstance = true;
        private readonly bool _isGenericFormatter;
        private string _name;
        private bool _serializeOnly;
        private bool _deserializeOnly;

        #endregion

        #region 属性

        /// <summary>获取格式化器类型</summary>
        public Type FormatterType => _formatterType;

        /// <summary>
        /// 获取或设置构造函数参数
        /// <para>用于创建格式化器实例时传递参数</para>
        /// </summary>
        public object[] Arguments
        {
            get => _arguments;
            set => _arguments = value;
        }

        /// <summary>
        /// 获取或设置优先级（数值越小优先级越高）
        /// <para>默认值：100</para>
        /// </summary>
        public int Priority
        {
            get => _priority;
            set => _priority = value;
        }

        /// <summary>
        /// 获取或设置是否缓存格式化器实例
        /// <para>默认值：true</para>
        /// </summary>
        public bool CacheInstance
        {
            get => _cacheInstance;
            set => _cacheInstance = value;
        }

        /// <summary>获取是否为泛型格式化器</summary>
        public bool IsGenericFormatter => _isGenericFormatter;

        /// <summary>
        /// 获取或设置格式化器名称（用于调试）
        /// </summary>
        public string Name
        {
            get => _name ?? _formatterType?.Name;
            set => _name = value;
        }

        /// <summary>
        /// 获取或设置是否仅用于序列化
        /// <para>默认值：false</para>
        /// </summary>
        public bool SerializeOnly
        {
            get => _serializeOnly;
            set => _serializeOnly = value;
        }

        /// <summary>
        /// 获取或设置是否仅用于反序列化
        /// <para>默认值：false</para>
        /// </summary>
        public bool DeserializeOnly
        {
            get => _deserializeOnly;
            set => _deserializeOnly = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="CustomFormatterAttribute"/> 的新实例
        /// </summary>
        /// <param name="formatterType">格式化器类型</param>
        /// <exception cref="ArgumentNullException">格式化器类型为 null</exception>
        /// <exception cref="ArgumentException">格式化器类型无效</exception>
        public CustomFormatterAttribute(Type formatterType)
        {
            _formatterType = formatterType ?? throw new ArgumentNullException(nameof(formatterType));
            _isGenericFormatter = formatterType.IsGenericTypeDefinition;
            ValidateFormatterType(formatterType);
        }

        /// <summary>
        /// 初始化 <see cref="CustomFormatterAttribute"/> 的新实例
        /// </summary>
        /// <param name="formatterType">格式化器类型</param>
        /// <param name="priority">优先级</param>
        public CustomFormatterAttribute(Type formatterType, int priority) : this(formatterType)
        {
            _priority = priority;
        }

        #endregion

        #region 公共方法

        /// <summary>检查是否有构造函数参数</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasArguments() => _arguments != null && _arguments.Length > 0;

        /// <summary>检查是否适用于序列化</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsApplicableForSerialize() => !_deserializeOnly;

        /// <summary>检查是否适用于反序列化</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsApplicableForDeserialize() => !_serializeOnly;

        /// <summary>创建泛型格式化器类型</summary>
        /// <param name="typeArguments">类型参数</param>
        /// <returns>构造后的泛型类型</returns>
        public Type MakeGenericFormatterType(params Type[] typeArguments)
        {
            if (!_isGenericFormatter)
                throw new InvalidOperationException("格式化器不是泛型类型定义");

            if (typeArguments == null || typeArguments.Length == 0)
                throw new ArgumentException("必须提供类型参数", nameof(typeArguments));

            var genericArgs = _formatterType.GetGenericArguments();
            if (genericArgs.Length != typeArguments.Length)
                throw new ArgumentException(
                    $"类型参数数量不匹配。期望 {genericArgs.Length} 个，实际 {typeArguments.Length} 个",
                    nameof(typeArguments));

            return _formatterType.MakeGenericType(typeArguments);
        }

        /// <summary>获取配置摘要信息</summary>
        public string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { $"Type={_formatterType.Name}" };

            if (_priority != DefaultPriority) parts.Add($"Priority={_priority}");
            if (HasArguments()) parts.Add($"Args={_arguments.Length}");
            if (!_cacheInstance) parts.Add("NoCache");
            if (_serializeOnly) parts.Add("SerializeOnly");
            if (_deserializeOnly) parts.Add("DeserializeOnly");

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString() => $"[CustomFormatter({_formatterType.Name})]";

        #endregion

        #region 私有方法

        private static void ValidateFormatterType(Type type)
        {
            if (type.IsAbstract && !type.IsGenericTypeDefinition)
                throw new ArgumentException("格式化器类型不能是抽象类", nameof(type));

            if (type.IsInterface)
                throw new ArgumentException("格式化器类型不能是接口", nameof(type));
        }

        #endregion
    }
}
