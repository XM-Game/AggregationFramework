// ==========================================================
// 文件名：SerializeVersionAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化版本特性
    /// <para>标记类型的序列化版本，用于版本迁移和兼容性管理</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>标记类型的当前序列化版本</item>
    ///   <item>定义最小兼容版本</item>
    ///   <item>支持版本迁移策略配置</item>
    ///   <item>提供版本变更历史记录</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 基础用法
    /// [Serializable]
    /// [SerializeVersion(1)]
    /// public class PlayerDataV1
    /// {
    ///     public string Name;
    ///     public int Level;
    /// }
    /// 
    /// // 版本升级示例
    /// [Serializable]
    /// [SerializeVersion(2, MinCompatibleVersion = 1)]
    /// public class PlayerDataV2
    /// {
    ///     public string Name;
    ///     public int Level;
    ///     public float Experience; // V2 新增字段
    /// }
    /// 
    /// // 完整配置示例
    /// [Serializable]
    /// [SerializeVersion(
    ///     Version = 3,
    ///     MinCompatibleVersion = 1,
    ///     MigrationStrategy = VersionMigrationStrategy.Automatic,
    ///     ChangeLog = "V3: 添加技能系统支持")]
    /// public class PlayerDataV3
    /// {
    ///     public string Name;
    ///     public int Level;
    ///     public float Experience;
    ///     public int[] SkillIds; // V3 新增字段
    /// }
    /// </code>
    /// 
    /// <para><b>版本迁移流程：</b></para>
    /// <list type="number">
    ///   <item>读取数据版本号</item>
    ///   <item>检查是否在兼容范围内</item>
    ///   <item>如需迁移，调用版本迁移器</item>
    ///   <item>反序列化为当前版本对象</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class SerializeVersionAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 默认版本号
        /// </summary>
        public const int DefaultVersion = 1;

        /// <summary>
        /// 最小版本号
        /// </summary>
        public const int MinVersion = 0;

        /// <summary>
        /// 最大版本号
        /// </summary>
        public const int MaxVersion = int.MaxValue;

        /// <summary>
        /// 未指定版本
        /// </summary>
        public const int UnspecifiedVersion = -1;

        #endregion

        #region 字段

        private readonly int _version;
        private int _minCompatibleVersion;
        private VersionMigrationStrategy _migrationStrategy = VersionMigrationStrategy.Automatic;
        private string _changeLog;
        private Type _migratorType;
        private bool _isPreview;
        private bool _isDeprecated;
        private string _deprecationMessage;
        private string _releaseDate;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前版本号
        /// </summary>
        public int Version => _version;

        /// <summary>
        /// 获取或设置最小兼容版本
        /// <para>默认值：与当前版本相同</para>
        /// </summary>
        public int MinCompatibleVersion
        {
            get => _minCompatibleVersion;
            set => _minCompatibleVersion = value;
        }

        /// <summary>
        /// 获取或设置版本迁移策略
        /// <para>默认值：<see cref="VersionMigrationStrategy.Automatic"/></para>
        /// </summary>
        public VersionMigrationStrategy MigrationStrategy
        {
            get => _migrationStrategy;
            set => _migrationStrategy = value;
        }

        /// <summary>
        /// 获取或设置变更日志
        /// </summary>
        public string ChangeLog
        {
            get => _changeLog;
            set => _changeLog = value;
        }

        /// <summary>
        /// 获取或设置版本迁移器类型
        /// </summary>
        public Type MigratorType
        {
            get => _migratorType;
            set => _migratorType = value;
        }

        /// <summary>
        /// 获取或设置是否为预览版本
        /// </summary>
        public bool IsPreview
        {
            get => _isPreview;
            set => _isPreview = value;
        }

        /// <summary>
        /// 获取或设置是否已弃用
        /// </summary>
        public bool IsDeprecated
        {
            get => _isDeprecated;
            set => _isDeprecated = value;
        }

        /// <summary>
        /// 获取或设置弃用消息
        /// </summary>
        public string DeprecationMessage
        {
            get => _deprecationMessage;
            set => _deprecationMessage = value;
        }

        /// <summary>
        /// 获取或设置发布日期（格式：YYYY-MM-DD）
        /// </summary>
        public string ReleaseDate
        {
            get => _releaseDate;
            set => _releaseDate = value;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeVersionAttribute"/> 的新实例
        /// </summary>
        /// <param name="version">版本号（必须大于等于 0）</param>
        /// <exception cref="ArgumentOutOfRangeException">版本号小于 0</exception>
        public SerializeVersionAttribute(int version)
        {
            if (version < MinVersion)
                throw new ArgumentOutOfRangeException(nameof(version),
                    $"版本号必须大于等于 {MinVersion}");

            _version = version;
            _minCompatibleVersion = version;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeVersionAttribute"/> 的新实例
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="minCompatibleVersion">最小兼容版本</param>
        public SerializeVersionAttribute(int version, int minCompatibleVersion)
        {
            if (version < MinVersion)
                throw new ArgumentOutOfRangeException(nameof(version),
                    $"版本号必须大于等于 {MinVersion}");
            if (minCompatibleVersion < MinVersion)
                throw new ArgumentOutOfRangeException(nameof(minCompatibleVersion),
                    $"最小兼容版本必须大于等于 {MinVersion}");
            if (minCompatibleVersion > version)
                throw new ArgumentException(
                    "最小兼容版本不能大于当前版本", nameof(minCompatibleVersion));

            _version = version;
            _minCompatibleVersion = minCompatibleVersion;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查指定版本是否兼容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompatible(int dataVersion)
        {
            return dataVersion >= _minCompatibleVersion && dataVersion <= _version;
        }

        /// <summary>
        /// 检查是否需要迁移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RequiresMigration(int dataVersion)
        {
            return dataVersion < _version && dataVersion >= _minCompatibleVersion;
        }

        /// <summary>
        /// 检查是否为最新版本
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLatestVersion(int dataVersion) => dataVersion == _version;

        /// <summary>
        /// 检查数据版本是否过旧
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTooOld(int dataVersion) => dataVersion < _minCompatibleVersion;

        /// <summary>
        /// 检查数据版本是否过新
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTooNew(int dataVersion) => dataVersion > _version;

        /// <summary>
        /// 检查是否有自定义迁移器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCustomMigrator() => _migratorType != null;

        /// <summary>
        /// 获取兼容版本范围
        /// </summary>
        public (int Min, int Max) GetCompatibleRange() => (_minCompatibleVersion, _version);

        /// <summary>
        /// 获取版本信息结构
        /// </summary>
        public SerializeVersionInfo ToVersionInfo()
        {
            return SerializeVersionInfo.CreateWithSchema(_version, 0, 0, _minCompatibleVersion);
        }

        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        public string GetSummary()
        {
            var preview = _isPreview ? " [Preview]" : "";
            var deprecated = _isDeprecated ? " [Deprecated]" : "";
            return $"Version={_version}, MinCompatible={_minCompatibleVersion}{preview}{deprecated}";
        }

        /// <inheritdoc/>
        public override string ToString() => $"[SerializeVersion({_version})]";

        #endregion
    }
}
