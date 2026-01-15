// ==========================================================
// 文件名：InternStringAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 字符串内化特性
    /// <para>指定字符串字段在反序列化时使用字符串内化（String Interning）</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>反序列化时将字符串添加到内部池中</item>
    ///   <item>相同内容的字符串共享同一实例</item>
    ///   <item>显著减少重复字符串的内存占用</item>
    ///   <item>支持自定义字符串池</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class GameConfig
    /// {
    ///     // 使用 CLR 内置字符串池
    ///     [InternString]
    ///     public string Category;
    ///     
    ///     // 仅内化短字符串
    ///     [InternString(MaxLengthToIntern = 32)]
    ///     public string Name;
    /// }
    /// </code>
    /// 
    /// <para><b>适用场景：</b></para>
    /// <list type="bullet">
    ///   <item>枚举值的字符串表示</item>
    ///   <item>分类标签、类型名称</item>
    ///   <item>语言代码、国家代码</item>
    ///   <item>配置键名</item>
    /// </list>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>内化的字符串永远不会被 GC 回收（CLR 内置池）</item>
    ///   <item>不适合内化大量唯一字符串</item>
    ///   <item>长字符串内化可能浪费内存</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class InternStringAttribute : Attribute
    {
        #region 常量

        /// <summary>默认最大内化长度</summary>
        public const int DefaultMaxLengthToIntern = 128;

        /// <summary>无长度限制</summary>
        public const int NoLengthLimit = -1;

        #endregion

        #region 字段

        private bool _useClrIntern = true;
        private Type _poolType;
        private int _maxLengthToIntern = DefaultMaxLengthToIntern;
        private bool _caseSensitive = true;
        private bool _internOnSerialize;
        private bool _allowNull = true;
        private EmptyStringHandling _emptyStringHandling = EmptyStringHandling.Intern;
        private bool _enableStatistics;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置是否使用 CLR 内置字符串池
        /// <para>默认值：true</para>
        /// </summary>
        public bool UseClrIntern
        {
            get => _useClrIntern;
            set => _useClrIntern = value;
        }

        /// <summary>
        /// 获取或设置自定义字符串池类型
        /// <para>必须实现 IStringPool 接口</para>
        /// </summary>
        public Type PoolType
        {
            get => _poolType;
            set => _poolType = value;
        }

        /// <summary>
        /// 获取或设置最大内化长度
        /// <para>默认值：128</para>
        /// <para>设置为 -1 表示无限制</para>
        /// </summary>
        public int MaxLengthToIntern
        {
            get => _maxLengthToIntern;
            set => _maxLengthToIntern = value;
        }

        /// <summary>
        /// 获取或设置是否区分大小写
        /// <para>默认值：true</para>
        /// </summary>
        public bool CaseSensitive
        {
            get => _caseSensitive;
            set => _caseSensitive = value;
        }

        /// <summary>
        /// 获取或设置是否在序列化时也使用内化
        /// <para>默认值：false</para>
        /// </summary>
        public bool InternOnSerialize
        {
            get => _internOnSerialize;
            set => _internOnSerialize = value;
        }

        /// <summary>
        /// 获取或设置是否允许 null 值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowNull
        {
            get => _allowNull;
            set => _allowNull = value;
        }

        /// <summary>
        /// 获取或设置空字符串处理方式
        /// <para>默认值：<see cref="EmptyStringHandling.Intern"/></para>
        /// </summary>
        public EmptyStringHandling EmptyStringHandling
        {
            get => _emptyStringHandling;
            set => _emptyStringHandling = value;
        }

        /// <summary>
        /// 获取或设置是否启用统计（仅用于调试）
        /// <para>默认值：false</para>
        /// </summary>
        public bool EnableStatistics
        {
            get => _enableStatistics;
            set => _enableStatistics = value;
        }

        #endregion

        #region 构造函数

        /// <summary>初始化 <see cref="InternStringAttribute"/> 的新实例</summary>
        public InternStringAttribute() { }

        /// <summary>初始化 <see cref="InternStringAttribute"/> 的新实例</summary>
        /// <param name="maxLengthToIntern">最大内化长度</param>
        public InternStringAttribute(int maxLengthToIntern)
        {
            _maxLengthToIntern = maxLengthToIntern;
        }

        /// <summary>初始化 <see cref="InternStringAttribute"/> 的新实例</summary>
        /// <param name="poolType">自定义字符串池类型</param>
        public InternStringAttribute(Type poolType)
        {
            _poolType = poolType;
            _useClrIntern = false;
        }

        #endregion

        #region 公共方法

        /// <summary>检查是否有长度限制</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasLengthLimit() => _maxLengthToIntern > 0;

        /// <summary>检查是否使用自定义池</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCustomPool() => _poolType != null;

        /// <summary>检查字符串是否应该被内化</summary>
        public bool ShouldIntern(string value)
        {
            if (value == null) return false;
            if (value.Length == 0) return _emptyStringHandling == EmptyStringHandling.Intern;
            if (HasLengthLimit() && value.Length > _maxLengthToIntern) return false;
            return true;
        }

        /// <summary>内化字符串</summary>
        public string Intern(string value)
        {
            if (value == null)
                return _allowNull ? null : string.Empty;

            if (value.Length == 0)
                return _emptyStringHandling.Process(value);

            if (!ShouldIntern(value))
                return value;

            if (!_caseSensitive)
                value = value.ToLowerInvariant();

            if (_useClrIntern && !HasCustomPool())
                return string.Intern(value);

            return value;
        }

        /// <summary>检查字符串是否已内化</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInterned(string value)
        {
            return value != null && string.IsInterned(value) != null;
        }

        /// <summary>获取配置摘要信息</summary>
        public string GetSummary()
        {
            var parts = new System.Collections.Generic.List<string> { "Intern" };

            if (HasCustomPool()) parts.Add($"Pool={_poolType.Name}");
            else if (_useClrIntern) parts.Add("CLR");
            if (HasLengthLimit()) parts.Add($"MaxLen={_maxLengthToIntern}");
            if (!_caseSensitive) parts.Add("CaseInsensitive");
            if (_internOnSerialize) parts.Add("OnSerialize");
            if (_emptyStringHandling != EmptyStringHandling.Intern) parts.Add($"Empty={_emptyStringHandling}");

            return string.Join(", ", parts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasLengthLimit()
                ? $"[InternString(MaxLength={_maxLengthToIntern})]"
                : "[InternString]";
        }

        #endregion
    }

    /// <summary>
    /// 字符串池接口
    /// <para>用于实现自定义字符串内化池</para>
    /// </summary>
    public interface IStringPool
    {
        /// <summary>获取或添加字符串到池中</summary>
        string GetOrAdd(string value);

        /// <summary>尝试从池中获取字符串</summary>
        bool TryGet(string value, out string interned);

        /// <summary>清空池</summary>
        void Clear();

        /// <summary>获取池中字符串数量</summary>
        int Count { get; }
    }
}
