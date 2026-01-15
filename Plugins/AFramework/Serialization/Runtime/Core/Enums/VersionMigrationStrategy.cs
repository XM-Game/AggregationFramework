// ==========================================================
// 文件名：VersionMigrationStrategy.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 版本迁移策略枚举
    /// <para>定义序列化数据版本不匹配时的处理策略</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// [SerializeVersion(2, MigrationStrategy = VersionMigrationStrategy.Automatic)]
    /// public class PlayerData { }
    /// </code>
    /// </remarks>
    public enum VersionMigrationStrategy : byte
    {
        /// <summary>
        /// 自动迁移（默认）
        /// <para>系统自动处理字段增减和类型转换</para>
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// 手动迁移
        /// <para>使用自定义迁移器处理版本转换</para>
        /// </summary>
        Manual = 1,

        /// <summary>
        /// 严格模式
        /// <para>版本不匹配时抛出异常</para>
        /// </summary>
        Strict = 2,

        /// <summary>
        /// 宽松模式
        /// <para>尽可能兼容，忽略无法处理的字段</para>
        /// </summary>
        Lenient = 3,

        /// <summary>
        /// 混合模式
        /// <para>自动处理简单变更，复杂变更使用迁移器</para>
        /// </summary>
        Hybrid = 4,

        /// <summary>
        /// 禁用迁移
        /// <para>不进行任何版本迁移</para>
        /// </summary>
        Disabled = 5
    }

    /// <summary>
    /// VersionMigrationStrategy 扩展方法
    /// </summary>
    public static class VersionMigrationStrategyExtensions
    {
        /// <summary>
        /// 检查是否允许自动迁移
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllowsAutoMigration(this VersionMigrationStrategy strategy)
        {
            return strategy == VersionMigrationStrategy.Automatic ||
                   strategy == VersionMigrationStrategy.Lenient ||
                   strategy == VersionMigrationStrategy.Hybrid;
        }

        /// <summary>
        /// 检查是否需要自定义迁移器
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresCustomMigrator(this VersionMigrationStrategy strategy)
        {
            return strategy == VersionMigrationStrategy.Manual ||
                   strategy == VersionMigrationStrategy.Hybrid;
        }

        /// <summary>
        /// 检查是否为严格模式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStrict(this VersionMigrationStrategy strategy)
        {
            return strategy == VersionMigrationStrategy.Strict;
        }

        /// <summary>
        /// 检查是否已禁用
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisabled(this VersionMigrationStrategy strategy)
        {
            return strategy == VersionMigrationStrategy.Disabled;
        }

        /// <summary>
        /// 获取策略的中文描述
        /// </summary>
        public static string GetDescription(this VersionMigrationStrategy strategy)
        {
            return strategy switch
            {
                VersionMigrationStrategy.Automatic => "自动迁移",
                VersionMigrationStrategy.Manual => "手动迁移",
                VersionMigrationStrategy.Strict => "严格模式",
                VersionMigrationStrategy.Lenient => "宽松模式",
                VersionMigrationStrategy.Hybrid => "混合模式",
                VersionMigrationStrategy.Disabled => "禁用迁移",
                _ => "未知策略"
            };
        }
    }
}
