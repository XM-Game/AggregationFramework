// ==========================================================
// 文件名：VersionedFormatterBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 版本化格式化器基类 (非泛型)
    /// <para>支持数据版本迁移</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 自动写入/读取版本号
    /// 2. 支持版本范围验证
    /// 3. 支持版本迁移回调
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerFormatterV2 : VersionedFormatterBase
    /// {
    ///     public override Type TargetType => typeof(Player);
    ///     public override int CurrentVersion => 2;
    ///     public override int MinSupportedVersion => 1;
    ///     
    ///     protected override void SerializeVersionCore(ISerializeWriter writer, object value, int version, SerializeOptions options)
    ///     {
    ///         var player = (Player)value;
    ///         writer.WriteString(player.Name);
    ///         if (version >= 2)
    ///             writer.WriteInt64(player.Experience);
    ///     }
    ///     
    ///     protected override object MigrateVersion(object value, int fromVersion, int toVersion)
    ///     {
    ///         // V1 -> V2: 添加 Experience 字段
    ///         if (fromVersion == 1 && toVersion == 2)
    ///         {
    ///             var player = (Player)value;
    ///             player.Experience = player.Level * 1000;
    ///         }
    ///         return value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class VersionedFormatterBase : FormatterBase, IVersionedFormatter
    {
        #region 属性

        /// <summary>
        /// 获取当前版本
        /// </summary>
        public abstract int CurrentVersion { get; }

        /// <summary>
        /// 获取最小支持版本
        /// </summary>
        public virtual int MinSupportedVersion => 1;

        #endregion

        #region IVersionedFormatter 实现

        /// <inheritdoc/>
        public void SerializeVersion(ISerializeWriter writer, object value, int version, SerializeOptions options)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            ValidateVersion(version);

            // 写入版本号
            writer.WriteInt32(version);

            // 序列化数据
            SerializeVersionCore(writer, value, version, options);
        }

        /// <inheritdoc/>
        public object DeserializeVersion(ISerializeReader reader, int version, DeserializeOptions options)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (version < MinSupportedVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(version),
                    $"版本 {version} 低于最小支持版本 {MinSupportedVersion}");
            }

            // 反序列化数据
            var value = DeserializeVersionCore(reader, version, options);

            // 如果需要，执行版本迁移
            if (version < CurrentVersion)
                value = MigrateVersion(value, version, CurrentVersion);

            return value;
        }

        /// <inheritdoc/>
        public bool IsVersionSupported(int version)
        {
            return version >= MinSupportedVersion && version <= CurrentVersion;
        }

        /// <inheritdoc/>
        public bool CanMigrate(int fromVersion, int toVersion)
        {
            return IsVersionSupported(fromVersion) && IsVersionSupported(toVersion);
        }

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected sealed override void SerializeCore(ISerializeWriter writer, object value, SerializeOptions options)
        {
            SerializeVersion(writer, value, CurrentVersion, options);
        }

        /// <inheritdoc/>
        protected sealed override object DeserializeCore(ISerializeReader reader, DeserializeOptions options)
        {
            // 读取版本号
            var version = reader.ReadInt32();
            return DeserializeVersion(reader, version, options);
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 序列化指定版本的数据
        /// </summary>
        /// <param name="writer">序列化写入器</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="version">版本号</param>
        /// <param name="options">序列化选项</param>
        protected abstract void SerializeVersionCore(ISerializeWriter writer, object value, int version, SerializeOptions options);

        /// <summary>
        /// 反序列化指定版本的数据
        /// </summary>
        /// <param name="reader">序列化读取器</param>
        /// <param name="version">版本号</param>
        /// <param name="options">反序列化选项</param>
        /// <returns>反序列化的对象</returns>
        protected abstract object DeserializeVersionCore(ISerializeReader reader, int version, DeserializeOptions options);

        /// <summary>
        /// 执行版本迁移
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="fromVersion">源版本</param>
        /// <param name="toVersion">目标版本</param>
        /// <returns>迁移后的值</returns>
        protected abstract object MigrateVersion(object value, int fromVersion, int toVersion);

        #endregion

        #region 辅助方法

        /// <summary>
        /// 验证版本号
        /// </summary>
        /// <param name="version">版本号</param>
        private void ValidateVersion(int version)
        {
            if (version < MinSupportedVersion || version > CurrentVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(version),
                    $"版本 {version} 不在支持范围 [{MinSupportedVersion}, {CurrentVersion}] 内");
            }
        }

        #endregion
    }
}
