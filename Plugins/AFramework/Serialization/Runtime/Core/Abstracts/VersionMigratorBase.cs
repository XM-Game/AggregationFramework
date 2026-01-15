// ==========================================================
// 文件名：VersionMigratorBase.cs
// 命名空间: AFramework.Serialization
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 版本迁移器抽象基类
    /// <para>提供 IVersionMigrator 接口的基础实现</para>
    /// <para>支持数据结构版本演进和向后兼容</para>
    /// </summary>
    /// <remarks>
    /// 设计说明:
    /// 1. 支持链式版本迁移（v1 → v2 → v3）
    /// 2. 提供迁移路径验证
    /// 3. 支持迁移回调和钩子
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerMigrator : VersionMigratorBase&lt;Player&gt;
    /// {
    ///     public PlayerMigrator() : base(currentVersion: 3, minSupportedVersion: 1)
    ///     {
    ///         RegisterMigration(1, 2, (player, ctx) =>
    ///         {
    ///             player.NewField = "default";
    ///             return player;
    ///         });
    ///         
    ///         RegisterMigration(2, 3, (player, ctx) =>
    ///         {
    ///             player.RenamedField = player.OldField;
    ///             return player;
    ///         });
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class VersionMigratorBase : IVersionMigrator
    {
        #region 字段

        /// <summary>迁移步骤字典</summary>
        private readonly Dictionary<(int from, int to), Func<object, MigrationContext, object>> _migrations;

        /// <summary>当前版本</summary>
        private readonly int _currentVersion;

        /// <summary>最小支持版本</summary>
        private readonly int _minSupportedVersion;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建版本迁移器基类实例
        /// </summary>
        /// <param name="currentVersion">当前版本</param>
        /// <param name="minSupportedVersion">最小支持版本</param>
        protected VersionMigratorBase(int currentVersion, int minSupportedVersion = 1)
        {
            if (currentVersion < 1)
                throw new ArgumentOutOfRangeException(nameof(currentVersion), "版本号必须大于 0");

            if (minSupportedVersion < 1)
                throw new ArgumentOutOfRangeException(nameof(minSupportedVersion), "最小支持版本必须大于 0");

            if (minSupportedVersion > currentVersion)
                throw new ArgumentException("最小支持版本不能大于当前版本");

            _currentVersion = currentVersion;
            _minSupportedVersion = minSupportedVersion;
            _migrations = new Dictionary<(int from, int to), Func<object, MigrationContext, object>>();
        }

        #endregion

        #region IVersionMigrator 实现

        /// <summary>获取源版本 (最小支持版本)</summary>
        public int SourceVersion => _minSupportedVersion;

        /// <summary>获取目标版本 (当前版本)</summary>
        public int TargetVersion => _currentVersion;

        /// <summary>获取目标类型</summary>
        public abstract Type TargetType { get; }

        /// <summary>获取当前版本</summary>
        public int CurrentVersion => _currentVersion;

        /// <summary>获取最小支持版本</summary>
        public int MinSupportedVersion => _minSupportedVersion;

        /// <inheritdoc/>
        public object Migrate(object source, MigrationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Migrate(source, context.SourceVersion, context.TargetVersion, context);
        }

        /// <summary>
        /// 迁移对象
        /// </summary>
        public object Migrate(object value, int fromVersion, int toVersion, MigrationContext context)
        {
            if (value == null)
                return null;

            ValidateVersions(fromVersion, toVersion);

            // 如果版本相同，无需迁移
            if (fromVersion == toVersion)
                return value;

            // 确定迁移方向
            var step = fromVersion < toVersion ? 1 : -1;
            var current = value;
            var currentVersion = fromVersion;

            // 逐步迁移
            while (currentVersion != toVersion)
            {
                var nextVersion = currentVersion + step;
                current = MigrateStep(current, currentVersion, nextVersion, context);
                currentVersion = nextVersion;
            }

            return current;
        }

        /// <inheritdoc/>
        public bool CanMigrate(int sourceVersion)
        {
            return CanMigrate(sourceVersion, _currentVersion);
        }

        /// <summary>
        /// 检查是否可以迁移
        /// </summary>
        public bool CanMigrate(int fromVersion, int toVersion)
        {
            if (fromVersion < _minSupportedVersion || fromVersion > _currentVersion)
                return false;

            if (toVersion < _minSupportedVersion || toVersion > _currentVersion)
                return false;

            return HasMigrationPath(fromVersion, toVersion);
        }

        /// <summary>
        /// 获取迁移路径
        /// </summary>
        public int[] GetMigrationPath(int fromVersion, int toVersion)
        {
            if (!CanMigrate(fromVersion, toVersion))
                return Array.Empty<int>();

            var path = new List<int> { fromVersion };
            var step = fromVersion < toVersion ? 1 : -1;
            var current = fromVersion;

            while (current != toVersion)
            {
                current += step;
                path.Add(current);
            }

            return path.ToArray();
        }

        #endregion

        #region 注册方法

        /// <summary>
        /// 注册迁移步骤
        /// </summary>
        protected void RegisterMigration(int fromVersion, int toVersion, Func<object, MigrationContext, object> migration)
        {
            if (migration == null)
                throw new ArgumentNullException(nameof(migration));

            ValidateVersionRange(fromVersion);
            ValidateVersionRange(toVersion);

            if (Math.Abs(fromVersion - toVersion) != 1)
                throw new ArgumentException("迁移步骤只能在相邻版本之间进行");

            _migrations[(fromVersion, toVersion)] = migration;
        }

        /// <summary>
        /// 注册迁移步骤 (简化版本)
        /// </summary>
        protected void RegisterMigration(int fromVersion, int toVersion, Func<object, object> migration)
        {
            RegisterMigration(fromVersion, toVersion, (value, _) => migration(value));
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 执行单步迁移
        /// </summary>
        protected virtual object MigrateStep(object value, int fromVersion, int toVersion, MigrationContext context)
        {
            if (_migrations.TryGetValue((fromVersion, toVersion), out var migration))
            {
                OnBeforeMigration(value, fromVersion, toVersion, context);
                var result = migration(value, context);
                OnAfterMigration(result, fromVersion, toVersion, context);
                return result;
            }

            return AutoMigrate(value, fromVersion, toVersion, context);
        }

        /// <summary>
        /// 自动迁移 (当没有注册迁移函数时)
        /// </summary>
        protected virtual object AutoMigrate(object value, int fromVersion, int toVersion, MigrationContext context)
        {
            return value;
        }

        /// <summary>
        /// 迁移前回调
        /// </summary>
        protected virtual void OnBeforeMigration(object value, int fromVersion, int toVersion, MigrationContext context)
        {
        }

        /// <summary>
        /// 迁移后回调
        /// </summary>
        protected virtual void OnAfterMigration(object value, int fromVersion, int toVersion, MigrationContext context)
        {
        }

        /// <summary>
        /// 检查是否可以自动迁移
        /// </summary>
        protected virtual bool CanAutoMigrate(int fromVersion, int toVersion)
        {
            return true;
        }

        #endregion

        #region 辅助方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateVersionRange(int version)
        {
            if (version < _minSupportedVersion || version > _currentVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(version),
                    $"版本 {version} 不在支持范围 [{_minSupportedVersion}, {_currentVersion}] 内");
            }
        }

        private void ValidateVersions(int fromVersion, int toVersion)
        {
            ValidateVersionRange(fromVersion);
            ValidateVersionRange(toVersion);
        }

        private bool HasMigrationPath(int fromVersion, int toVersion)
        {
            if (fromVersion == toVersion)
                return true;

            var step = fromVersion < toVersion ? 1 : -1;
            var current = fromVersion;

            while (current != toVersion)
            {
                var next = current + step;
                if (!_migrations.ContainsKey((current, next)) && !CanAutoMigrate(current, next))
                    return false;

                current = next;
            }

            return true;
        }

        #endregion
    }

    /// <summary>
    /// 泛型版本迁移器抽象基类
    /// </summary>
    /// <typeparam name="T">要迁移的类型</typeparam>
    public abstract class VersionMigratorBase<T> : VersionMigratorBase, IVersionMigrator<T>
    {
        #region 构造函数

        /// <summary>
        /// 创建泛型版本迁移器基类实例
        /// </summary>
        protected VersionMigratorBase(int currentVersion, int minSupportedVersion = 1)
            : base(currentVersion, minSupportedVersion)
        {
        }

        #endregion

        #region IVersionMigrator 实现

        /// <inheritdoc/>
        public override Type TargetType => typeof(T);

        #endregion

        #region IVersionMigrator<T> 实现

        /// <inheritdoc/>
        public T Migrate(T source, MigrationContext context)
        {
            return (T)base.Migrate(source, context);
        }

        /// <summary>
        /// 迁移对象
        /// </summary>
        public T Migrate(T value, int fromVersion, int toVersion, MigrationContext context)
        {
            return (T)base.Migrate(value, fromVersion, toVersion, context);
        }

        /// <summary>
        /// 迁移到最新版本
        /// </summary>
        public T MigrateToLatest(T value, int fromVersion, MigrationContext context)
        {
            return Migrate(value, fromVersion, CurrentVersion, context);
        }

        #endregion

        #region 注册方法

        /// <summary>
        /// 注册类型化迁移步骤
        /// </summary>
        protected void RegisterMigration(int fromVersion, int toVersion, Func<T, MigrationContext, T> migration)
        {
            base.RegisterMigration(fromVersion, toVersion, (value, context) => migration((T)value, context));
        }

        /// <summary>
        /// 注册类型化迁移步骤 (简化版本)
        /// </summary>
        protected void RegisterMigration(int fromVersion, int toVersion, Func<T, T> migration)
        {
            base.RegisterMigration(fromVersion, toVersion, (value, _) => migration((T)value));
        }

        #endregion
    }

    /// <summary>
    /// 迁移管理器
    /// <para>管理多个类型的版本迁移器</para>
    /// </summary>
    public class MigrationManager : IMigrationManager
    {
        #region 字段

        /// <summary>迁移器字典</summary>
        private readonly ConcurrentDictionary<Type, IVersionMigrator> _migrators;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建迁移管理器
        /// </summary>
        public MigrationManager()
        {
            _migrators = new ConcurrentDictionary<Type, IVersionMigrator>();
        }

        #endregion

        #region IMigrationManager 实现

        /// <inheritdoc/>
        public void Register(IVersionMigrator migrator)
        {
            if (migrator == null)
                throw new ArgumentNullException(nameof(migrator));

            _migrators[migrator.TargetType] = migrator;
        }

        /// <inheritdoc/>
        public void Register<T>(IVersionMigrator<T> migrator)
        {
            if (migrator == null)
                throw new ArgumentNullException(nameof(migrator));

            _migrators[typeof(T)] = migrator;
        }

        /// <inheritdoc/>
        public object MigrateToLatest(object source, int sourceVersion, Type targetType)
        {
            var migrator = GetMigrator(targetType);
            if (migrator == null)
                throw new InvalidOperationException($"类型 {targetType.Name} 没有注册迁移器");

            var latestVersion = GetLatestVersion(targetType);
            var context = new MigrationContext(sourceVersion, latestVersion, targetType, this);
            return migrator.Migrate(source, context);
        }

        /// <inheritdoc/>
        public object MigrateTo(object source, int sourceVersion, int targetVersion, Type targetType)
        {
            var migrator = GetMigrator(targetType);
            if (migrator == null)
                throw new InvalidOperationException($"类型 {targetType.Name} 没有注册迁移器");

            var context = new MigrationContext(sourceVersion, targetVersion, targetType, this);
            return migrator.Migrate(source, context);
        }

        /// <inheritdoc/>
        public int GetLatestVersion(Type type)
        {
            var migrator = GetMigrator(type);
            if (migrator == null)
                throw new InvalidOperationException($"类型 {type.Name} 没有注册迁移器");

            return migrator.TargetVersion;
        }

        /// <inheritdoc/>
        public bool CanMigrate(Type type, int sourceVersion, int targetVersion)
        {
            var migrator = GetMigrator(type);
            if (migrator == null)
                return false;

            if (migrator is VersionMigratorBase baseMigrator)
                return baseMigrator.CanMigrate(sourceVersion, targetVersion);

            return migrator.SourceVersion == sourceVersion && migrator.TargetVersion == targetVersion;
        }

        /// <inheritdoc/>
        public IVersionMigrator[] GetMigrationPath(Type type, int sourceVersion, int targetVersion)
        {
            var migrator = GetMigrator(type);
            if (migrator == null || !CanMigrate(type, sourceVersion, targetVersion))
                return Array.Empty<IVersionMigrator>();

            return new[] { migrator };
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取迁移器
        /// </summary>
        public IVersionMigrator GetMigrator(Type type)
        {
            _migrators.TryGetValue(type, out var migrator);
            return migrator;
        }

        /// <summary>
        /// 获取迁移器 (泛型)
        /// </summary>
        public IVersionMigrator<T> GetMigrator<T>()
        {
            return GetMigrator(typeof(T)) as IVersionMigrator<T>;
        }

        /// <summary>
        /// 检查是否有迁移器
        /// </summary>
        public bool HasMigrator(Type type)
        {
            return _migrators.ContainsKey(type);
        }

        /// <summary>
        /// 检查是否有迁移器 (泛型)
        /// </summary>
        public bool HasMigrator<T>()
        {
            return HasMigrator(typeof(T));
        }

        /// <summary>
        /// 注销迁移器
        /// </summary>
        public bool Unregister(Type type)
        {
            return _migrators.TryRemove(type, out _);
        }

        /// <summary>
        /// 注销迁移器 (泛型)
        /// </summary>
        public bool Unregister<T>()
        {
            return Unregister(typeof(T));
        }

        /// <summary>
        /// 清除所有迁移器
        /// </summary>
        public void Clear()
        {
            _migrators.Clear();
        }

        /// <summary>
        /// 获取已注册的迁移器数量
        /// </summary>
        public int Count => _migrators.Count;

        #endregion
    }

    /// <summary>
    /// 空迁移器
    /// <para>不执行任何迁移，直接返回原值</para>
    /// </summary>
    public sealed class NullMigrator : IVersionMigrator
    {
        /// <summary>单例实例</summary>
        public static readonly NullMigrator Instance = new NullMigrator();

        private NullMigrator() { }

        /// <inheritdoc/>
        public int SourceVersion => 1;

        /// <inheritdoc/>
        public int TargetVersion => 1;

        /// <inheritdoc/>
        public Type TargetType => typeof(object);

        /// <inheritdoc/>
        public object Migrate(object source, MigrationContext context) => source;

        /// <inheritdoc/>
        public bool CanMigrate(int sourceVersion) => true;
    }

    /// <summary>
    /// 泛型空迁移器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public sealed class NullMigrator<T> : IVersionMigrator<T>
    {
        /// <summary>单例实例</summary>
        public static readonly NullMigrator<T> Instance = new NullMigrator<T>();

        private NullMigrator() { }

        /// <inheritdoc/>
        public int SourceVersion => 1;

        /// <inheritdoc/>
        public int TargetVersion => 1;

        /// <inheritdoc/>
        public Type TargetType => typeof(T);

        /// <inheritdoc/>
        public T Migrate(T source, MigrationContext context) => source;

        /// <inheritdoc/>
        object IVersionMigrator.Migrate(object source, MigrationContext context) => source;

        /// <inheritdoc/>
        public bool CanMigrate(int sourceVersion) => true;
    }
}
