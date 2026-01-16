// ==========================================================
// 文件名：OrderAttribute.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 [Order] 特性，指定入口点的执行顺序

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 执行顺序特性
    /// <para>指定入口点（如 IInitializable、ITickable）的执行顺序</para>
    /// <para>Order attribute that specifies execution order for entry points</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>控制多个 IInitializable 的初始化顺序</item>
    /// <item>控制多个 ITickable 的更新顺序</item>
    /// <item>确保依赖服务先于依赖它的服务初始化</item>
    /// </list>
    /// 
    /// 排序规则：
    /// <list type="bullet">
    /// <item>数值越小，执行越早</item>
    /// <item>默认顺序为 0</item>
    /// <item>相同顺序的入口点执行顺序不确定</item>
    /// <item>支持负数，可用于指定最先执行的入口点</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 配置服务最先初始化
    /// [Order(-100)]
    /// public class ConfigService : IInitializable
    /// {
    ///     public void Initialize() { /* 加载配置 */ }
    /// }
    /// 
    /// // 日志服务其次
    /// [Order(-50)]
    /// public class LogService : IInitializable
    /// {
    ///     public void Initialize() { /* 初始化日志 */ }
    /// }
    /// 
    /// // 游戏服务使用默认顺序
    /// public class GameService : IInitializable
    /// {
    ///     public void Initialize() { /* 初始化游戏 */ }
    /// }
    /// 
    /// // UI 服务最后初始化
    /// [Order(100)]
    /// public class UIService : IInitializable
    /// {
    ///     public void Initialize() { /* 初始化 UI */ }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OrderAttribute : Attribute
    {
        /// <summary>
        /// 获取执行顺序
        /// <para>Get the execution order</para>
        /// </summary>
        /// <remarks>
        /// 数值越小，执行越早。默认为 0。
        /// </remarks>
        public int Order { get; }

        /// <summary>
        /// 创建执行顺序特性实例
        /// </summary>
        /// <param name="order">执行顺序，数值越小越先执行 / Execution order, smaller values execute first</param>
        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}
