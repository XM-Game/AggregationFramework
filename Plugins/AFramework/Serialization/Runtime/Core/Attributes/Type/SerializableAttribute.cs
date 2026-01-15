// ==========================================================
// 文件名：SerializableAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 可序列化特性
    /// <para>标记类型为可序列化，触发源代码生成器生成序列化代码</para>
    /// <para>这是序列化系统的核心特性，所有需要序列化的类型都应标记此特性</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>标记类型参与序列化系统</item>
    ///   <item>触发源代码生成器自动生成序列化代码</item>
    ///   <item>配置序列化行为（模式、布局、生成方式等）</item>
    ///   <item>支持类、结构体、记录类型</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 基础用法 - 使用默认配置
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     public int Id;
    ///     public string Name;
    ///     public float Health;
    /// }
    /// 
    /// // 高级用法 - 自定义配置
    /// [Serializable(
    ///     Mode = SerializeMode.VersionTolerant,
    ///     Layout = SerializeLayout.KeyValue,
    ///     GenerateMode = GenerateMode.Auto,
    ///     IncludePrivate = true)]
    /// public class GameConfig
    /// {
    ///     public int Version;
    ///     private string _secretKey;
    /// }
    /// 
    /// // 结构体用法
    /// [Serializable]
    /// public struct Vector3Data
    /// {
    ///     public float X, Y, Z;
    /// }
    /// </code>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>类型必须是 public 或 internal</item>
    ///   <item>嵌套类型需要外部类型也标记此特性</item>
    ///   <item>泛型类型的类型参数也需要可序列化</item>
    ///   <item>循环引用需要使用 CircularReference 模式</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class SerializableAttribute : Attribute
    {
        #region 字段

        private SerializeMode _mode = SerializeMode.Object;
        private SerializeLayout _layout = SerializeLayout.Auto;
        private GenerateMode _generateMode = GenerateMode.Auto;
        private bool _includePrivate;
        private bool _includeReadOnly;
        private bool _includeProperties = true;
        private bool _includeFields = true;
        private bool _enableNullCheck = true;
        private bool _enableTypeValidation = true;
        private Type _formatterType;
        private string _typeAlias;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置序列化模式
        /// <para>默认值：<see cref="SerializeMode.Object"/></para>
        /// </summary>
        public SerializeMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        /// <summary>
        /// 获取或设置布局方式
        /// <para>默认值：<see cref="SerializeLayout.Auto"/></para>
        /// </summary>
        public SerializeLayout Layout
        {
            get => _layout;
            set => _layout = value;
        }

        /// <summary>
        /// 获取或设置代码生成模式
        /// <para>默认值：<see cref="GenerateMode.Auto"/></para>
        /// </summary>
        public GenerateMode GenerateMode
        {
            get => _generateMode;
            set => _generateMode = value;
        }

        /// <summary>
        /// 获取或设置是否包含私有成员
        /// <para>默认值：false</para>
        /// </summary>
        public bool IncludePrivate
        {
            get => _includePrivate;
            set => _includePrivate = value;
        }

        /// <summary>
        /// 获取或设置是否包含只读成员
        /// <para>默认值：false</para>
        /// </summary>
        public bool IncludeReadOnly
        {
            get => _includeReadOnly;
            set => _includeReadOnly = value;
        }

        /// <summary>
        /// 获取或设置是否包含属性
        /// <para>默认值：true</para>
        /// </summary>
        public bool IncludeProperties
        {
            get => _includeProperties;
            set => _includeProperties = value;
        }

        /// <summary>
        /// 获取或设置是否包含字段
        /// <para>默认值：true</para>
        /// </summary>
        public bool IncludeFields
        {
            get => _includeFields;
            set => _includeFields = value;
        }

        /// <summary>
        /// 获取或设置是否启用空值检查
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableNullCheck
        {
            get => _enableNullCheck;
            set => _enableNullCheck = value;
        }

        /// <summary>
        /// 获取或设置是否启用类型验证
        /// <para>默认值：true</para>
        /// </summary>
        public bool EnableTypeValidation
        {
            get => _enableTypeValidation;
            set => _enableTypeValidation = value;
        }

        /// <summary>
        /// 获取或设置自定义格式化器类型
        /// </summary>
        public Type FormatterType
        {
            get => _formatterType;
            set => _formatterType = value;
        }

        /// <summary>
        /// 获取或设置类型别名（用于版本迁移）
        /// </summary>
        public string TypeAlias
        {
            get => _typeAlias;
            set => _typeAlias = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializableAttribute"/> 的新实例（使用默认配置）
        /// </summary>
        public SerializableAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="SerializableAttribute"/> 的新实例
        /// </summary>
        /// <param name="mode">序列化模式</param>
        public SerializableAttribute(SerializeMode mode)
        {
            _mode = mode;
        }

        /// <summary>
        /// 初始化 <see cref="SerializableAttribute"/> 的新实例
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <param name="layout">布局方式</param>
        public SerializableAttribute(SerializeMode mode, SerializeLayout layout)
        {
            _mode = mode;
            _layout = layout;
        }

        /// <summary>
        /// 初始化 <see cref="SerializableAttribute"/> 的新实例
        /// </summary>
        /// <param name="mode">序列化模式</param>
        /// <param name="layout">布局方式</param>
        /// <param name="generateMode">生成模式</param>
        public SerializableAttribute(SerializeMode mode, SerializeLayout layout, GenerateMode generateMode)
        {
            _mode = mode;
            _layout = layout;
            _generateMode = generateMode;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取解析后的实际布局方式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializeLayout GetResolvedLayout() => _layout.ResolveAuto(_mode);

        /// <summary>
        /// 检查是否使用自定义格式化器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCustomFormatter() => _formatterType != null;

        /// <summary>
        /// 检查是否有类型别名
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTypeAlias() => !string.IsNullOrEmpty(_typeAlias);

        /// <summary>
        /// 检查是否需要源代码生成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RequiresCodeGeneration()
        {
            return _generateMode.RequiresSourceGenerator() && _formatterType == null;
        }

        /// <summary>
        /// 检查配置是否支持 AOT 编译
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SupportsAOT() => _generateMode.SupportsAOT();

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            return $"Mode={_mode.GetName()}, Layout={GetResolvedLayout()}, Generate={_generateMode}";
        }

        /// <inheritdoc/>
        public override string ToString() => $"[Serializable(Mode={_mode})]";

        #endregion
    }
}
