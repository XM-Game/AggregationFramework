// ==========================================================
// 文件名：SerializeLayoutAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化布局特性
    /// <para>指定类型成员的序列化布局方式</para>
    /// <para>可覆盖 [Serializable] 特性中的默认布局设置</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>指定成员的序列化顺序策略</item>
    ///   <item>配置字段标识符的存储方式</item>
    ///   <item>控制数据紧凑度与灵活性的平衡</item>
    ///   <item>支持版本容错的布局配置</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 使用顺序布局（最紧凑）
    /// [Serializable]
    /// [SerializeLayout(SerializeLayout.Sequential)]
    /// public class CompactData
    /// {
    ///     public int Id;      // 顺序 0
    ///     public string Name; // 顺序 1
    /// }
    /// 
    /// // 使用键值布局（版本容错）
    /// [Serializable]
    /// [SerializeLayout(SerializeLayout.KeyValue, NamingStrategy = NamingStrategy.CamelCase)]
    /// public class FlexibleData
    /// {
    ///     public int Id;      // 键: "id"
    ///     public string Name; // 键: "name"
    /// }
    /// 
    /// // 使用索引布局（紧凑且版本容错）
    /// [Serializable]
    /// [SerializeLayout(SerializeLayout.Indexed, StartIndex = 1)]
    /// public class IndexedData
    /// {
    ///     public int Id;      // 索引: 1
    ///     public string Name; // 索引: 2
    /// }
    /// </code>
    /// 
    /// <para><b>布局方式对比：</b></para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>布局</term>
    ///     <description>紧凑度 | 版本容错 | 适用场景</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Sequential</term>
    ///     <description>★★★★★ | ✗ | 高性能网络通信</description>
    ///   </item>
    ///   <item>
    ///     <term>KeyValue</term>
    ///     <description>★★☆☆☆ | ✓ | 配置文件、存档</description>
    ///   </item>
    ///   <item>
    ///     <term>Indexed</term>
    ///     <description>★★★★☆ | ✓ | 平衡场景</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class SerializeLayoutAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 默认起始索引
        /// </summary>
        public const int DefaultStartIndex = 0;

        /// <summary>
        /// 默认索引步长
        /// </summary>
        public const int DefaultIndexStep = 1;

        #endregion

        #region 字段

        private readonly SerializeLayout _layout;
        private int _startIndex = DefaultStartIndex;
        private int _indexStep = DefaultIndexStep;
        private bool _includeTypeInfo;
        private bool _useCompressedKeys;
        private string _keyPrefix;
        private NamingStrategy _namingStrategy = NamingStrategy.None;
        private bool _preserveUnknownFields;
        private bool _enableFieldSorting;
        private Type _fieldComparerType;

        #endregion

        #region 属性

        /// <summary>
        /// 获取布局方式
        /// </summary>
        public SerializeLayout Layout => _layout;

        /// <summary>
        /// 获取或设置起始索引（仅用于 Indexed 布局）
        /// <para>默认值：0</para>
        /// </summary>
        public int StartIndex
        {
            get => _startIndex;
            set => _startIndex = value;
        }

        /// <summary>
        /// 获取或设置索引步长（仅用于 Indexed 布局）
        /// <para>默认值：1</para>
        /// </summary>
        public int IndexStep
        {
            get => _indexStep;
            set => _indexStep = value;
        }

        /// <summary>
        /// 获取或设置是否包含类型信息
        /// <para>默认值：false</para>
        /// </summary>
        public bool IncludeTypeInfo
        {
            get => _includeTypeInfo;
            set => _includeTypeInfo = value;
        }

        /// <summary>
        /// 获取或设置是否使用压缩键名（仅用于 KeyValue 布局）
        /// <para>默认值：false</para>
        /// </summary>
        public bool UseCompressedKeys
        {
            get => _useCompressedKeys;
            set => _useCompressedKeys = value;
        }

        /// <summary>
        /// 获取或设置键名前缀（仅用于 KeyValue 布局）
        /// </summary>
        public string KeyPrefix
        {
            get => _keyPrefix;
            set => _keyPrefix = value;
        }

        /// <summary>
        /// 获取或设置键名命名策略（仅用于 KeyValue 布局）
        /// <para>默认值：<see cref="NamingStrategy.None"/></para>
        /// </summary>
        public NamingStrategy NamingStrategy
        {
            get => _namingStrategy;
            set => _namingStrategy = value;
        }

        /// <summary>
        /// 获取或设置是否保留未知字段
        /// <para>默认值：false</para>
        /// </summary>
        public bool PreserveUnknownFields
        {
            get => _preserveUnknownFields;
            set => _preserveUnknownFields = value;
        }

        /// <summary>
        /// 获取或设置是否启用字段排序
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableFieldSorting
        {
            get => _enableFieldSorting;
            set => _enableFieldSorting = value;
        }

        /// <summary>
        /// 获取或设置字段排序比较器类型
        /// </summary>
        public Type FieldComparerType
        {
            get => _fieldComparerType;
            set => _fieldComparerType = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeLayoutAttribute"/> 的新实例
        /// </summary>
        /// <param name="layout">布局方式</param>
        public SerializeLayoutAttribute(SerializeLayout layout)
        {
            _layout = layout;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeLayoutAttribute"/> 的新实例
        /// </summary>
        /// <param name="layout">布局方式</param>
        /// <param name="startIndex">起始索引（用于 Indexed 布局）</param>
        public SerializeLayoutAttribute(SerializeLayout layout, int startIndex)
        {
            _layout = layout;
            _startIndex = startIndex;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算字段的索引值
        /// </summary>
        /// <param name="fieldOrder">字段顺序（从 0 开始）</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculateIndex(int fieldOrder) => _startIndex + (fieldOrder * _indexStep);

        /// <summary>
        /// 转换键名
        /// </summary>
        /// <param name="originalKey">原始键名</param>
        public string TransformKey(string originalKey)
        {
            if (string.IsNullOrEmpty(originalKey))
                return originalKey;

            // 应用命名策略
            var key = _namingStrategy.Transform(originalKey);

            // 添加前缀
            if (!string.IsNullOrEmpty(_keyPrefix))
                key = _keyPrefix + key;

            return key;
        }

        /// <summary>
        /// 检查是否支持版本容错
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SupportsVersionTolerance() => _layout.SupportsVersionTolerance();

        /// <summary>
        /// 检查是否需要字段标识符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RequiresFieldIdentifier() => _layout.HasFieldIdentifier();

        /// <summary>
        /// 检查是否有自定义比较器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCustomComparer() => _fieldComparerType != null;

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var extras = new System.Text.StringBuilder();

            if (_layout == SerializeLayout.Indexed)
                extras.Append($", StartIndex={_startIndex}");

            if (_useCompressedKeys)
                extras.Append(", CompressedKeys");

            if (_preserveUnknownFields)
                extras.Append(", PreserveUnknown");

            if (_namingStrategy != NamingStrategy.None)
                extras.Append($", Naming={_namingStrategy}");

            return $"Layout={_layout}{extras}";
        }

        /// <inheritdoc/>
        public override string ToString() => $"[SerializeLayout({_layout})]";

        #endregion
    }
}
