// ==========================================================
// 文件名：IVersionMigrator.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 版本迁移器接口
    /// <para>定义数据版本迁移的操作</para>
    /// <para>支持数据结构变更时的向后兼容</para>
    /// </summary>
    /// <remarks>
    /// 版本迁移器用于处理数据结构变更时的兼容性问题。
    /// 
    /// 使用示例:
    /// <code>
    /// public class PlayerMigrator : IVersionMigrator&lt;Player&gt;
    /// {
    ///     public int SourceVersion => 1;
    ///     public int TargetVersion => 2;
    ///     
    ///     public Player Migrate(Player source, MigrationContext context)
    ///     {
    ///         // V1 -> V2: 添加 Experience 字段
    ///         return new Player
    ///         {
    ///             Name = source.Name,
    ///             Level = source.Level,
    ///             Experience = source.Level * 1000 // 默认值
    ///         };
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public interface IVersionMigrator
    {
        /// <summary>获取源版本</summary>
        int SourceVersion { get; }

        /// <summary>获取目标版本</summary>
        int TargetVersion { get; }

        /// <summary>获取目标类型</summary>
        Type TargetType { get; }

        /// <summary>
        /// 执行迁移
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="context">迁移上下文</param>
        /// <returns>迁移后的对象</returns>
        object Migrate(object source, MigrationContext context);

        /// <summary>
        /// 检查是否可以迁移
        /// </summary>
        /// <param name="sourceVersion">源版本</param>
        /// <returns>如果可以迁移返回 true</returns>
        bool CanMigrate(int sourceVersion);
    }


    /// <summary>
    /// 泛型版本迁移器接口
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    public interface IVersionMigrator<T> : IVersionMigrator
    {
        /// <summary>
        /// 执行迁移
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="context">迁移上下文</param>
        /// <returns>迁移后的对象</returns>
        T Migrate(T source, MigrationContext context);
    }

    /// <summary>
    /// 迁移管理器接口
    /// <para>管理多个版本迁移器，支持链式迁移</para>
    /// </summary>
    public interface IMigrationManager
    {
        /// <summary>
        /// 注册迁移器
        /// </summary>
        /// <param name="migrator">迁移器</param>
        void Register(IVersionMigrator migrator);

        /// <summary>
        /// 注册迁移器
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="migrator">迁移器</param>
        void Register<T>(IVersionMigrator<T> migrator);

        /// <summary>
        /// 迁移对象到最新版本
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="sourceVersion">源版本</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>迁移后的对象</returns>
        object MigrateToLatest(object source, int sourceVersion, Type targetType);

        /// <summary>
        /// 迁移对象到指定版本
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="sourceVersion">源版本</param>
        /// <param name="targetVersion">目标版本</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>迁移后的对象</returns>
        object MigrateTo(object source, int sourceVersion, int targetVersion, Type targetType);

        /// <summary>
        /// 获取指定类型的最新版本
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>最新版本号</returns>
        int GetLatestVersion(Type type);

        /// <summary>
        /// 检查是否可以迁移
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="sourceVersion">源版本</param>
        /// <param name="targetVersion">目标版本</param>
        /// <returns>如果可以迁移返回 true</returns>
        bool CanMigrate(Type type, int sourceVersion, int targetVersion);

        /// <summary>
        /// 获取迁移路径
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="sourceVersion">源版本</param>
        /// <param name="targetVersion">目标版本</param>
        /// <returns>迁移器数组</returns>
        IVersionMigrator[] GetMigrationPath(Type type, int sourceVersion, int targetVersion);
    }

    /// <summary>
    /// 迁移上下文
    /// <para>提供迁移过程中的上下文信息</para>
    /// </summary>
    public sealed class MigrationContext
    {
        /// <summary>源版本</summary>
        public int SourceVersion { get; }

        /// <summary>目标版本</summary>
        public int TargetVersion { get; }

        /// <summary>目标类型</summary>
        public Type TargetType { get; }

        /// <summary>迁移管理器</summary>
        public IMigrationManager Manager { get; }

        /// <summary>自定义数据</summary>
        public System.Collections.Generic.Dictionary<string, object> Data { get; }

        /// <summary>警告列表</summary>
        public System.Collections.Generic.List<string> Warnings { get; }

        /// <summary>
        /// 创建迁移上下文
        /// </summary>
        public MigrationContext(int sourceVersion, int targetVersion, Type targetType, IMigrationManager manager = null)
        {
            SourceVersion = sourceVersion;
            TargetVersion = targetVersion;
            TargetType = targetType;
            Manager = manager;
            Data = new System.Collections.Generic.Dictionary<string, object>();
            Warnings = new System.Collections.Generic.List<string>();
        }

        /// <summary>
        /// 添加警告
        /// </summary>
        /// <param name="message">警告信息</param>
        public void AddWarning(string message)
        {
            Warnings.Add(message);
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetData<T>(string key, T value)
        {
            Data[key] = value;
        }

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>数据值</returns>
        public T GetData<T>(string key, T defaultValue = default)
        {
            return Data.TryGetValue(key, out var value) && value is T typedValue ? typedValue : defaultValue;
        }
    }
}
