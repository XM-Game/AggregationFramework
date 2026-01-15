// ==========================================================
// 文件名：GenerateModeAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 代码生成模式特性
    /// <para>指定序列化代码的生成方式</para>
    /// <para>可覆盖 [Serializable] 特性中的默认生成模式设置</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>控制序列化代码的生成策略</item>
    ///   <item>配置源代码生成器的行为</item>
    ///   <item>支持编译时和运行时生成</item>
    ///   <item>提供 AOT 兼容性配置</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 自动生成（推荐，默认）
    /// [Serializable]
    /// [GenerateMode(GenerateMode.Auto)]
    /// public class PlayerData
    /// {
    ///     public int Id;
    ///     public string Name;
    /// }
    /// 
    /// // 手动实现
    /// [Serializable]
    /// [GenerateMode(GenerateMode.Manual)]
    /// public class CustomData : ISerializable
    /// {
    ///     public void Serialize(ref SerializeWriter writer) { }
    ///     public void Deserialize(ref SerializeReader reader) { }
    /// }
    /// 
    /// // 混合模式 - 自动生成基础代码，部分方法手动覆盖
    /// [Serializable]
    /// [GenerateMode(GenerateMode.Hybrid, OverrideMethods = new[] { "OnSerializing" })]
    /// public partial class HybridData
    /// {
    ///     public int Value;
    ///     
    ///     partial void OnSerializing()
    ///     {
    ///         // 自定义序列化前逻辑
    ///     }
    /// }
    /// </code>
    /// 
    /// <para><b>生成模式对比：</b></para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>模式</term>
    ///     <description>性能 | AOT | 灵活性 | 适用场景</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Auto</term>
    ///     <description>★★★★★ | ✓ | ★★☆ | 标准数据类型</description>
    ///   </item>
    ///   <item>
    ///     <term>Manual</term>
    ///     <description>★★★★★ | ✓ | ★★★★★ | 特殊序列化需求</description>
    ///   </item>
    ///   <item>
    ///     <term>Reflection</term>
    ///     <description>★☆☆☆☆ | ✗ | ★★★★★ | 动态类型</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class GenerateModeAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 默认格式化器后缀
        /// </summary>
        public const string DefaultFormatterSuffix = "Formatter";

        #endregion

        #region 字段

        private readonly GenerateMode _mode;
        private string[] _overrideMethods;
        private bool _generatePartial = true;
        private bool _generateExtensions;
        private bool _generateDebugInfo;
        private bool _enableInlining = true;
        private bool _enableBurst;
        private Type _customGeneratorType;
        private string _generatedNamespace;
        private string _generatedSuffix = DefaultFormatterSuffix;
        private bool _skipValidation;

        #endregion

        #region 属性

        /// <summary>
        /// 获取生成模式
        /// </summary>
        public GenerateMode Mode => _mode;

        /// <summary>
        /// 获取或设置需要覆盖的方法名称（仅用于 Hybrid 模式）
        /// </summary>
        /// <remarks>
        /// 可覆盖的方法：OnSerializing, OnSerialized, OnDeserializing, OnDeserialized, Serialize, Deserialize
        /// </remarks>
        public string[] OverrideMethods
        {
            get => _overrideMethods;
            set => _overrideMethods = value;
        }

        /// <summary>
        /// 获取或设置是否生成 partial 类
        /// <para>默认值：true</para>
        /// </summary>
        public bool GeneratePartial
        {
            get => _generatePartial;
            set => _generatePartial = value;
        }

        /// <summary>
        /// 获取或设置是否生成扩展方法
        /// <para>默认值：false</para>
        /// </summary>
        public bool GenerateExtensions
        {
            get => _generateExtensions;
            set => _generateExtensions = value;
        }

        /// <summary>
        /// 获取或设置是否生成调试信息
        /// <para>默认值：false</para>
        /// </summary>
        public bool GenerateDebugInfo
        {
            get => _generateDebugInfo;
            set => _generateDebugInfo = value;
        }

        /// <summary>
        /// 获取或设置是否启用内联优化
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableInlining
        {
            get => _enableInlining;
            set => _enableInlining = value;
        }

        /// <summary>
        /// 获取或设置是否启用 Burst 编译
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableBurst
        {
            get => _enableBurst;
            set => _enableBurst = value;
        }

        /// <summary>
        /// 获取或设置自定义生成器类型
        /// </summary>
        public Type CustomGeneratorType
        {
            get => _customGeneratorType;
            set => _customGeneratorType = value;
        }

        /// <summary>
        /// 获取或设置生成的代码命名空间
        /// </summary>
        public string GeneratedNamespace
        {
            get => _generatedNamespace;
            set => _generatedNamespace = value;
        }

        /// <summary>
        /// 获取或设置生成的类名后缀
        /// <para>默认值："Formatter"</para>
        /// </summary>
        public string GeneratedSuffix
        {
            get => _generatedSuffix;
            set => _generatedSuffix = value;
        }

        /// <summary>
        /// 获取或设置是否跳过验证
        /// <para>默认值：false</para>
        /// </summary>
        public bool SkipValidation
        {
            get => _skipValidation;
            set => _skipValidation = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="GenerateModeAttribute"/> 的新实例
        /// </summary>
        /// <param name="mode">生成模式</param>
        public GenerateModeAttribute(GenerateMode mode)
        {
            _mode = mode;
        }

        /// <summary>
        /// 初始化 <see cref="GenerateModeAttribute"/> 的新实例（使用默认自动生成模式）
        /// </summary>
        public GenerateModeAttribute() : this(GenerateMode.Auto)
        {
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否需要源代码生成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RequiresSourceGeneration() => _mode.RequiresSourceGenerator();

        /// <summary>
        /// 检查是否支持 AOT 编译
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SupportsAOT() => _mode.SupportsAOT();

        /// <summary>
        /// 检查是否为编译时生成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompileTime() => _mode.IsCompileTime();

        /// <summary>
        /// 检查是否为运行时生成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRuntime() => _mode.IsRuntime();

        /// <summary>
        /// 检查是否有覆盖方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasOverrideMethods() => _overrideMethods is { Length: > 0 };

        /// <summary>
        /// 检查指定方法是否需要覆盖
        /// </summary>
        public bool ShouldOverride(string methodName)
        {
            if (_overrideMethods == null) return false;

            foreach (var method in _overrideMethods)
            {
                if (string.Equals(method, methodName, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否有自定义生成器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCustomGenerator() => _customGeneratorType != null;

        /// <summary>
        /// 获取生成的格式化器类名
        /// </summary>
        public string GetFormatterClassName(string typeName) => typeName + _generatedSuffix;

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var extras = new System.Text.StringBuilder();

            if (_enableBurst) extras.Append(", Burst");
            if (_generateDebugInfo) extras.Append(", Debug");
            if (!_enableInlining) extras.Append(", NoInline");
            if (HasOverrideMethods()) extras.Append($", Override[{_overrideMethods.Length}]");

            return $"Mode={_mode}{extras}";
        }

        /// <inheritdoc/>
        public override string ToString() => $"[GenerateMode({_mode})]";

        #endregion
    }
}
