// ==========================================================
// 文件名：VersionMismatchException.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.Serialization;

namespace AFramework.Serialization
{
    /// <summary>
    /// 版本不匹配异常
    /// <para>当序列化数据的版本与当前代码版本不兼容时抛出</para>
    /// </summary>
    /// <remarks>
    /// 版本不匹配的场景:
    /// <list type="bullet">
    /// <item>数据版本过旧，无法迁移到当前版本</item>
    /// <item>数据版本过新，当前代码不支持</item>
    /// <item>格式版本不兼容</item>
    /// <item>架构变更导致不兼容</item>
    /// </list>
    /// 
    /// 解决方案:
    /// <list type="number">
    /// <item>实现版本迁移器 (IVersionMigrator)</item>
    /// <item>使用 VersionTolerant 序列化模式</item>
    /// <item>更新应用程序版本</item>
    /// </list>
    /// 
    /// 使用示例:
    /// <code>
    /// try
    /// {
    ///     var data = serializer.Deserialize&lt;GameSave&gt;(bytes);
    /// }
    /// catch (VersionMismatchException ex)
    /// {
    ///     Console.WriteLine($"数据版本: {ex.DataVersion}");
    ///     Console.WriteLine($"期望版本: {ex.ExpectedVersion}");
    ///     
    ///     if (ex.CanMigrate)
    ///     {
    ///         // 尝试迁移
    ///         var migrated = migrator.Migrate(bytes, ex.DataVersion, ex.ExpectedVersion);
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [Serializable]
    public sealed class VersionMismatchException : SerializationException
    {
        #region 常量定义

        /// <summary>
        /// 默认错误消息模板
        /// </summary>
        private const string DefaultMessageTemplate = "版本不匹配: 数据版本 {0}，期望版本 {1}";

        #endregion

        #region 字段

        /// <summary>
        /// 数据版本
        /// </summary>
        private readonly int _dataVersion;

        /// <summary>
        /// 期望版本
        /// </summary>
        private readonly int _expectedVersion;

        /// <summary>
        /// 最小兼容版本
        /// </summary>
        private readonly int _minCompatibleVersion;

        /// <summary>
        /// 最大兼容版本
        /// </summary>
        private readonly int _maxCompatibleVersion;

        /// <summary>
        /// 版本不匹配类型
        /// </summary>
        private readonly VersionMismatchType _mismatchType;

        /// <summary>
        /// 是否可以迁移
        /// </summary>
        private readonly bool _canMigrate;

        #endregion

        #region 属性

        /// <summary>
        /// 获取数据版本
        /// </summary>
        /// <value>序列化数据中记录的版本号</value>
        public int DataVersion => _dataVersion;

        /// <summary>
        /// 获取期望版本
        /// </summary>
        /// <value>当前代码期望的版本号</value>
        public int ExpectedVersion => _expectedVersion;

        /// <summary>
        /// 获取最小兼容版本
        /// </summary>
        /// <value>支持的最小版本号</value>
        public int MinCompatibleVersion => _minCompatibleVersion;

        /// <summary>
        /// 获取最大兼容版本
        /// </summary>
        /// <value>支持的最大版本号</value>
        public int MaxCompatibleVersion => _maxCompatibleVersion;

        /// <summary>
        /// 获取版本不匹配类型
        /// </summary>
        /// <value>不匹配的具体类型</value>
        public VersionMismatchType MismatchType => _mismatchType;

        /// <summary>
        /// 获取是否可以迁移
        /// </summary>
        /// <value>true 表示可以通过迁移解决</value>
        public bool CanMigrate => _canMigrate;

        /// <summary>
        /// 获取版本差距
        /// </summary>
        /// <value>数据版本与期望版本的差值</value>
        public int VersionDelta => _dataVersion - _expectedVersion;

        /// <summary>
        /// 检查数据版本是否过旧
        /// </summary>
        public bool IsDataTooOld => _dataVersion < _minCompatibleVersion;

        /// <summary>
        /// 检查数据版本是否过新
        /// </summary>
        public bool IsDataTooNew => _dataVersion > _maxCompatibleVersion;

        #endregion

        #region 构造函数

        /// <summary>
        /// 使用数据版本和期望版本创建异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        public VersionMismatchException(int dataVersion, int expectedVersion)
            : this(dataVersion, expectedVersion, expectedVersion, expectedVersion)
        {
        }

        /// <summary>
        /// 使用版本范围创建异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <param name="minCompatibleVersion">最小兼容版本</param>
        /// <param name="maxCompatibleVersion">最大兼容版本</param>
        public VersionMismatchException(
            int dataVersion,
            int expectedVersion,
            int minCompatibleVersion,
            int maxCompatibleVersion)
            : this(dataVersion, expectedVersion, minCompatibleVersion, maxCompatibleVersion, null, false)
        {
        }

        /// <summary>
        /// 使用完整信息创建异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <param name="minCompatibleVersion">最小兼容版本</param>
        /// <param name="maxCompatibleVersion">最大兼容版本</param>
        /// <param name="relatedType">相关类型</param>
        /// <param name="canMigrate">是否可迁移</param>
        /// <param name="innerException">内部异常</param>
        public VersionMismatchException(
            int dataVersion,
            int expectedVersion,
            int minCompatibleVersion,
            int maxCompatibleVersion,
            Type relatedType,
            bool canMigrate,
            Exception innerException = null)
            : base(
                DetermineErrorCode(dataVersion, minCompatibleVersion, maxCompatibleVersion),
                string.Format(DefaultMessageTemplate, dataVersion, expectedVersion),
                relatedType,
                null,
                -1,
                innerException)
        {
            _dataVersion = dataVersion;
            _expectedVersion = expectedVersion;
            _minCompatibleVersion = minCompatibleVersion;
            _maxCompatibleVersion = maxCompatibleVersion;
            _canMigrate = canMigrate;
            _mismatchType = DetermineMismatchType(dataVersion, minCompatibleVersion, maxCompatibleVersion);
        }

        /// <summary>
        /// 使用自定义消息创建异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <param name="message">自定义消息</param>
        /// <param name="innerException">内部异常</param>
        public VersionMismatchException(
            int dataVersion,
            int expectedVersion,
            string message,
            Exception innerException = null)
            : base(
                SerializeErrorCode.VersionMismatch,
                message ?? string.Format(DefaultMessageTemplate, dataVersion, expectedVersion),
                null,
                null,
                -1,
                innerException)
        {
            _dataVersion = dataVersion;
            _expectedVersion = expectedVersion;
            _minCompatibleVersion = expectedVersion;
            _maxCompatibleVersion = expectedVersion;
            _mismatchType = dataVersion < expectedVersion 
                ? VersionMismatchType.DataTooOld 
                : VersionMismatchType.DataTooNew;
        }

        /// <summary>
        /// 序列化构造函数
        /// </summary>
        private VersionMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _dataVersion = info.GetInt32(nameof(DataVersion));
            _expectedVersion = info.GetInt32(nameof(ExpectedVersion));
            _minCompatibleVersion = info.GetInt32(nameof(MinCompatibleVersion));
            _maxCompatibleVersion = info.GetInt32(nameof(MaxCompatibleVersion));
            _mismatchType = (VersionMismatchType)info.GetInt32(nameof(MismatchType));
            _canMigrate = info.GetBoolean(nameof(CanMigrate));
        }

        #endregion

        #region 序列化支持

        /// <summary>
        /// 获取对象数据用于序列化
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(DataVersion), _dataVersion);
            info.AddValue(nameof(ExpectedVersion), _expectedVersion);
            info.AddValue(nameof(MinCompatibleVersion), _minCompatibleVersion);
            info.AddValue(nameof(MaxCompatibleVersion), _maxCompatibleVersion);
            info.AddValue(nameof(MismatchType), (int)_mismatchType);
            info.AddValue(nameof(CanMigrate), _canMigrate);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 确定错误码
        /// </summary>
        private static SerializeErrorCode DetermineErrorCode(int dataVersion, int minVersion, int maxVersion)
        {
            if (dataVersion < minVersion)
                return SerializeErrorCode.VersionTooOld;
            if (dataVersion > maxVersion)
                return SerializeErrorCode.VersionTooNew;
            return SerializeErrorCode.VersionMismatch;
        }

        /// <summary>
        /// 确定不匹配类型
        /// </summary>
        private static VersionMismatchType DetermineMismatchType(int dataVersion, int minVersion, int maxVersion)
        {
            if (dataVersion < minVersion)
                return VersionMismatchType.DataTooOld;
            if (dataVersion > maxVersion)
                return VersionMismatchType.DataTooNew;
            return VersionMismatchType.IncompatibleFormat;
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("=== 版本不匹配诊断信息 ===");
            builder.AppendLine($"数据版本: {_dataVersion}");
            builder.AppendLine($"期望版本: {_expectedVersion}");
            builder.AppendLine($"兼容范围: [{_minCompatibleVersion}, {_maxCompatibleVersion}]");
            builder.AppendLine($"不匹配类型: {_mismatchType}");
            builder.AppendLine($"版本差距: {VersionDelta}");
            builder.AppendLine($"是否可迁移: {_canMigrate}");
            
            builder.AppendLine("建议:");
            if (_mismatchType == VersionMismatchType.DataTooOld)
            {
                builder.AppendLine("  - 数据版本过旧，需要版本迁移");
                builder.AppendLine("  - 实现 IVersionMigrator 进行数据迁移");
                builder.AppendLine($"  - 需要从版本 {_dataVersion} 迁移到版本 {_expectedVersion}");
            }
            else if (_mismatchType == VersionMismatchType.DataTooNew)
            {
                builder.AppendLine("  - 数据版本过新，当前代码不支持");
                builder.AppendLine("  - 请更新应用程序到最新版本");
                builder.AppendLine($"  - 数据需要版本 {_dataVersion} 或更高的代码支持");
            }
            else
            {
                builder.AppendLine("  - 格式不兼容，可能需要重新生成数据");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取迁移路径建议
        /// </summary>
        /// <returns>从数据版本到期望版本的迁移步骤</returns>
        public int[] GetMigrationPath()
        {
            if (_dataVersion >= _expectedVersion)
                return Array.Empty<int>();

            var path = new int[_expectedVersion - _dataVersion + 1];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = _dataVersion + i;
            }
            return path;
        }

        #endregion

        #region 静态工厂方法

        /// <summary>
        /// 创建数据版本过旧异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="minVersion">最小支持版本</param>
        /// <param name="currentVersion">当前版本</param>
        /// <returns>版本不匹配异常</returns>
        public static VersionMismatchException TooOld(int dataVersion, int minVersion, int currentVersion)
        {
            return new VersionMismatchException(
                dataVersion,
                currentVersion,
                minVersion,
                currentVersion,
                null,
                dataVersion >= minVersion - 5); // 假设5个版本内可迁移
        }

        /// <summary>
        /// 创建数据版本过新异常
        /// </summary>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="maxVersion">最大支持版本</param>
        /// <param name="currentVersion">当前版本</param>
        /// <returns>版本不匹配异常</returns>
        public static VersionMismatchException TooNew(int dataVersion, int maxVersion, int currentVersion)
        {
            return new VersionMismatchException(
                dataVersion,
                currentVersion,
                currentVersion,
                maxVersion,
                null,
                false);
        }

        /// <summary>
        /// 创建格式版本不兼容异常
        /// </summary>
        /// <param name="formatVersion">格式版本</param>
        /// <param name="supportedVersion">支持的版本</param>
        /// <returns>版本不匹配异常</returns>
        public static VersionMismatchException IncompatibleFormat(int formatVersion, int supportedVersion)
        {
            return new VersionMismatchException(
                formatVersion,
                supportedVersion,
                $"不兼容的序列化格式版本: {formatVersion}，支持的版本: {supportedVersion}");
        }

        /// <summary>
        /// 为特定类型创建版本不匹配异常
        /// </summary>
        /// <typeparam name="T">相关类型</typeparam>
        /// <param name="dataVersion">数据版本</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <returns>版本不匹配异常</returns>
        public static VersionMismatchException ForType<T>(int dataVersion, int expectedVersion)
        {
            return new VersionMismatchException(
                dataVersion,
                expectedVersion,
                expectedVersion,
                expectedVersion,
                typeof(T),
                true);
        }

        #endregion
    }

    /// <summary>
    /// 版本不匹配类型枚举
    /// </summary>
    public enum VersionMismatchType
    {
        /// <summary>
        /// 数据版本过旧
        /// </summary>
        DataTooOld = 0,

        /// <summary>
        /// 数据版本过新
        /// </summary>
        DataTooNew = 1,

        /// <summary>
        /// 格式不兼容
        /// </summary>
        IncompatibleFormat = 2,

        /// <summary>
        /// 架构变更
        /// </summary>
        SchemaChanged = 3
    }
}
