// ==========================================================
// 文件名：VersionedFormatterBaseT.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 支持版本迁移的泛型格式化器基类
    /// </summary>
    /// <typeparam name="T">要格式化的类型</typeparam>
    /// <remarks>
    /// 设计说明:
    /// 1. 自动写入/读取版本号
    /// 2. 支持版本范围验证
    /// 3. 类型安全的版本迁移
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerFormatterV2 : VersionedFormatterBase&lt;Player&gt;
    /// {
    ///     public override int CurrentVersion => 2;
    ///     
    ///     protected override void SerializeVersionCore(ISerializeWriter writer, Player value, int version, SerializeOptions options)
    ///     {
    ///         writer.WriteString(value.Name);
    ///         if (version >= 2)
    ///             writer.WriteInt64(value.Experience);
    ///     }
    ///     
    ///     protected override Player DeserializeVersionCore(ISerializeReader reader, int version, DeserializeOptions options)
    ///     {
    ///         var player = new Player { Name = reader.ReadString() };
    ///         if (version >= 2)
    ///             player.Experience = reader.ReadInt64();
    ///         return player;
    ///     }
    ///     
    ///     protected override Player MigrateVersion(Player value, int fromVersion, int toVersion)
    ///     {
    ///         if (fromVersion == 1 && toVersion == 2)
    ///             value.Experience = value.Level * 1000;
    ///         return value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class VersionedFormatterBase<T> : FormatterBase<T>, IVersionedFormatter<T>
    {
        #region 属性

        /// <summary>获取当前版本</summary>
        public abstract int CurrentVersion { get; }

        /// <summary>获取最小支持版本</summary>
        public virtual int MinSupportedVersion => 1;

        #endregion

        #region IVersionedFormatter<T> 实现

        /// <inheritdoc/>
        public void SerializeVersion(ISerializeWriter writer, T value, int version, SerializeOptions options)
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
        public T DeserializeVersion(ISerializeReader reader, int version, DeserializeOptions options)
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

        #endregion

        #region 重写方法

        /// <inheritdoc/>
        protected sealed override void SerializeCore(ISerializeWriter writer, T value, SerializeOptions options)
        {
            SerializeVersion(writer, value, CurrentVersion, options);
        }

        /// <inheritdoc/>
        protected sealed override T DeserializeCore(ISerializeReader reader, DeserializeOptions options)
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
        protected abstract void SerializeVersionCore(ISerializeWriter writer, T value, int version, SerializeOptions options);

        /// <summary>
        /// 反序列化指定版本的数据
        /// </summary>
        protected abstract T DeserializeVersionCore(ISerializeReader reader, int version, DeserializeOptions options);

        /// <summary>
        /// 执行版本迁移
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="fromVersion">源版本</param>
        /// <param name="toVersion">目标版本</param>
        /// <returns>迁移后的值</returns>
        protected abstract T MigrateVersion(T value, int fromVersion, int toVersion);

        #endregion

        #region 辅助方法

        /// <summary>
        /// 验证版本号
        /// </summary>
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
