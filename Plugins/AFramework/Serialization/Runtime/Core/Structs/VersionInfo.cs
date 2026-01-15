// ==========================================================
// 文件名：VersionInfo.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化版本信息结构体
    /// <para>封装数据版本信息，用于版本容错和数据迁移</para>
    /// <para>支持语义化版本号和版本兼容性检查</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// // 创建版本信息
    /// var version = SerializeVersionInfo.Create(1, 2, 3);
    /// 
    /// // 检查兼容性
    /// if (version.IsCompatibleWith(minVersion))
    ///     // 可以反序列化
    /// 
    /// // 版本比较
    /// if (version > SerializeVersionInfo.V1_0_0)
    ///     // 使用新特性
    /// </code>
    /// </remarks>
    [Serializable]
    public readonly struct SerializeVersionInfo : IEquatable<SerializeVersionInfo>, IComparable<SerializeVersionInfo>
    {
        #region 字段

        private readonly ushort _major;
        private readonly ushort _minor;
        private readonly ushort _patch;
        private readonly ushort _build;
        private readonly int _schemaVersion;
        private readonly long _timestamp;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建版本信息
        /// </summary>
        private SerializeVersionInfo(ushort major, ushort minor, ushort patch, ushort build, int schemaVersion, long timestamp)
        {
            _major = major;
            _minor = minor;
            _patch = patch;
            _build = build;
            _schemaVersion = schemaVersion;
            _timestamp = timestamp;
        }

        #endregion

        #region 属性

        /// <summary>主版本号</summary>
        public ushort Major => _major;

        /// <summary>次版本号</summary>
        public ushort Minor => _minor;

        /// <summary>修订版本号</summary>
        public ushort Patch => _patch;

        /// <summary>构建号</summary>
        public ushort Build => _build;

        /// <summary>架构版本 (用于数据迁移)</summary>
        public int SchemaVersion => _schemaVersion;

        /// <summary>时间戳 (Ticks)</summary>
        public long Timestamp => _timestamp;

        /// <summary>时间戳 (DateTime)</summary>
        public DateTime TimestampDateTime => _timestamp > 0 ? new DateTime(_timestamp, DateTimeKind.Utc) : DateTime.MinValue;

        /// <summary>压缩版本号 (用于序列化)</summary>
        public uint PackedVersion => ((uint)_major << 24) | ((uint)_minor << 16) | ((uint)_patch << 8) | _build;

        /// <summary>短版本号 (Major.Minor)</summary>
        public ushort ShortVersion => (ushort)((_major << 8) | _minor);

        /// <summary>版本字符串</summary>
        public string VersionString => _build > 0
            ? $"{_major}.{_minor}.{_patch}.{_build}"
            : $"{_major}.{_minor}.{_patch}";

        /// <summary>是否有效</summary>
        public bool IsValid => _major > 0 || _minor > 0 || _patch > 0;

        #endregion

        #region 工厂方法

        /// <summary>
        /// 创建版本信息
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="patch">修订版本号</param>
        /// <param name="build">构建号</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeVersionInfo Create(int major, int minor = 0, int patch = 0, int build = 0)
        {
            return new SerializeVersionInfo(
                (ushort)Math.Clamp(major, 0, ushort.MaxValue),
                (ushort)Math.Clamp(minor, 0, ushort.MaxValue),
                (ushort)Math.Clamp(patch, 0, ushort.MaxValue),
                (ushort)Math.Clamp(build, 0, ushort.MaxValue),
                0,
                0
            );
        }

        /// <summary>
        /// 创建带架构版本的版本信息
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="patch">修订版本号</param>
        /// <param name="schemaVersion">架构版本</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeVersionInfo CreateWithSchema(int major, int minor, int patch, int schemaVersion)
        {
            return new SerializeVersionInfo(
                (ushort)Math.Clamp(major, 0, ushort.MaxValue),
                (ushort)Math.Clamp(minor, 0, ushort.MaxValue),
                (ushort)Math.Clamp(patch, 0, ushort.MaxValue),
                0,
                schemaVersion,
                0
            );
        }

        /// <summary>
        /// 从压缩版本号创建
        /// </summary>
        /// <param name="packed">压缩版本号</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeVersionInfo FromPacked(uint packed)
        {
            return new SerializeVersionInfo(
                (ushort)(packed >> 24),
                (ushort)((packed >> 16) & 0xFF),
                (ushort)((packed >> 8) & 0xFF),
                (ushort)(packed & 0xFF),
                0,
                0
            );
        }

        /// <summary>
        /// 从短版本号创建
        /// </summary>
        /// <param name="shortVersion">短版本号</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializeVersionInfo FromShort(ushort shortVersion)
        {
            return new SerializeVersionInfo(
                (ushort)(shortVersion >> 8),
                (ushort)(shortVersion & 0xFF),
                0, 0, 0, 0
            );
        }

        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串 (如 "1.2.3")</param>
        public static SerializeVersionInfo Parse(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
                return default;

            var parts = versionString.Split('.');
            int major = 0, minor = 0, patch = 0, build = 0;

            if (parts.Length >= 1) int.TryParse(parts[0], out major);
            if (parts.Length >= 2) int.TryParse(parts[1], out minor);
            if (parts.Length >= 3) int.TryParse(parts[2], out patch);
            if (parts.Length >= 4) int.TryParse(parts[3], out build);

            return Create(major, minor, patch, build);
        }

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">输出版本信息</param>
        public static bool TryParse(string versionString, out SerializeVersionInfo version)
        {
            version = Parse(versionString);
            return version.IsValid;
        }

        #endregion

        #region 预定义版本

        /// <summary>空版本</summary>
        public static SerializeVersionInfo Empty => default;

        /// <summary>版本 1.0.0</summary>
        public static SerializeVersionInfo V1_0_0 => Create(1, 0, 0);

        /// <summary>版本 1.1.0</summary>
        public static SerializeVersionInfo V1_1_0 => Create(1, 1, 0);

        /// <summary>版本 2.0.0</summary>
        public static SerializeVersionInfo V2_0_0 => Create(2, 0, 0);

        /// <summary>当前框架版本</summary>
        public static SerializeVersionInfo Current => Create(
            SerializeConstants.Version.Major,
            SerializeConstants.Version.Minor,
            SerializeConstants.Version.Patch
        );

        #endregion

        #region 兼容性检查方法

        /// <summary>
        /// 检查是否与指定版本兼容
        /// </summary>
        /// <param name="other">要比较的版本</param>
        /// <param name="checkMinor">是否检查次版本号</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompatibleWith(SerializeVersionInfo other, bool checkMinor = true)
        {
            if (_major != other._major)
                return false;
            if (checkMinor && _minor < other._minor)
                return false;
            return true;
        }

        /// <summary>
        /// 检查是否在版本范围内
        /// </summary>
        /// <param name="minVersion">最小版本</param>
        /// <param name="maxVersion">最大版本</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(SerializeVersionInfo minVersion, SerializeVersionInfo maxVersion)
        {
            return CompareTo(minVersion) >= 0 && CompareTo(maxVersion) <= 0;
        }

        /// <summary>
        /// 检查主版本号是否匹配
        /// </summary>
        /// <param name="other">要比较的版本</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMajorCompatible(SerializeVersionInfo other)
        {
            return _major == other._major;
        }

        /// <summary>
        /// 检查是否需要迁移
        /// </summary>
        /// <param name="targetSchema">目标架构版本</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RequiresMigration(int targetSchema)
        {
            return _schemaVersion != targetSchema && _schemaVersion > 0;
        }

        #endregion

        #region With 方法

        /// <summary>修改主版本号</summary>
        public SerializeVersionInfo WithMajor(int major) =>
            new SerializeVersionInfo((ushort)major, _minor, _patch, _build, _schemaVersion, _timestamp);

        /// <summary>修改次版本号</summary>
        public SerializeVersionInfo WithMinor(int minor) =>
            new SerializeVersionInfo(_major, (ushort)minor, _patch, _build, _schemaVersion, _timestamp);

        /// <summary>修改修订版本号</summary>
        public SerializeVersionInfo WithPatch(int patch) =>
            new SerializeVersionInfo(_major, _minor, (ushort)patch, _build, _schemaVersion, _timestamp);

        /// <summary>修改构建号</summary>
        public SerializeVersionInfo WithBuild(int build) =>
            new SerializeVersionInfo(_major, _minor, _patch, (ushort)build, _schemaVersion, _timestamp);

        /// <summary>修改架构版本</summary>
        public SerializeVersionInfo WithSchemaVersion(int schemaVersion) =>
            new SerializeVersionInfo(_major, _minor, _patch, _build, schemaVersion, _timestamp);

        /// <summary>添加时间戳</summary>
        public SerializeVersionInfo WithTimestamp(DateTime timestamp) =>
            new SerializeVersionInfo(_major, _minor, _patch, _build, _schemaVersion, timestamp.Ticks);

        /// <summary>添加当前时间戳</summary>
        public SerializeVersionInfo WithCurrentTimestamp() =>
            new SerializeVersionInfo(_major, _minor, _patch, _build, _schemaVersion, DateTime.UtcNow.Ticks);

        /// <summary>递增修订版本号</summary>
        public SerializeVersionInfo IncrementPatch() =>
            new SerializeVersionInfo(_major, _minor, (ushort)(_patch + 1), 0, _schemaVersion, _timestamp);

        /// <summary>递增次版本号</summary>
        public SerializeVersionInfo IncrementMinor() =>
            new SerializeVersionInfo(_major, (ushort)(_minor + 1), 0, 0, _schemaVersion, _timestamp);

        /// <summary>递增主版本号</summary>
        public SerializeVersionInfo IncrementMajor() =>
            new SerializeVersionInfo((ushort)(_major + 1), 0, 0, 0, _schemaVersion, _timestamp);

        #endregion

        #region IComparable 实现

        /// <summary>比较版本</summary>
        public int CompareTo(SerializeVersionInfo other)
        {
            int majorCompare = _major.CompareTo(other._major);
            if (majorCompare != 0) return majorCompare;

            int minorCompare = _minor.CompareTo(other._minor);
            if (minorCompare != 0) return minorCompare;

            int patchCompare = _patch.CompareTo(other._patch);
            if (patchCompare != 0) return patchCompare;

            return _build.CompareTo(other._build);
        }

        #endregion

        #region IEquatable 实现

        /// <summary>判断是否相等</summary>
        public bool Equals(SerializeVersionInfo other)
        {
            return _major == other._major &&
                   _minor == other._minor &&
                   _patch == other._patch &&
                   _build == other._build;
        }

        /// <summary>判断是否相等</summary>
        public override bool Equals(object obj) => obj is SerializeVersionInfo other && Equals(other);

        /// <summary>获取哈希码</summary>
        public override int GetHashCode() => HashCode.Combine(_major, _minor, _patch, _build);

        #endregion

        #region 运算符重载

        /// <summary>相等运算符</summary>
        public static bool operator ==(SerializeVersionInfo left, SerializeVersionInfo right) => left.Equals(right);

        /// <summary>不等运算符</summary>
        public static bool operator !=(SerializeVersionInfo left, SerializeVersionInfo right) => !left.Equals(right);

        /// <summary>小于运算符</summary>
        public static bool operator <(SerializeVersionInfo left, SerializeVersionInfo right) => left.CompareTo(right) < 0;

        /// <summary>大于运算符</summary>
        public static bool operator >(SerializeVersionInfo left, SerializeVersionInfo right) => left.CompareTo(right) > 0;

        /// <summary>小于等于运算符</summary>
        public static bool operator <=(SerializeVersionInfo left, SerializeVersionInfo right) => left.CompareTo(right) <= 0;

        /// <summary>大于等于运算符</summary>
        public static bool operator >=(SerializeVersionInfo left, SerializeVersionInfo right) => left.CompareTo(right) >= 0;

        #endregion

        #region 字符串表示

        /// <summary>获取字符串表示</summary>
        public override string ToString() => VersionString;

        /// <summary>获取详细字符串表示</summary>
        public string ToDetailedString()
        {
            var result = VersionString;
            if (_schemaVersion > 0)
                result += $" (Schema: {_schemaVersion})";
            if (_timestamp > 0)
                result += $" [{TimestampDateTime:yyyy-MM-dd HH:mm:ss}]";
            return result;
        }

        #endregion
    }
}
