// ==========================================================
// 文件名：InstallerBase.cs
// 命名空间: AFramework.DI
// 依赖: 无
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 安装器基类
    /// <para>提供纯 C# 安装器的基础实现</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 不需要 Unity 组件功能的安装器
    /// - 纯逻辑服务的注册
    /// - 可在非 Unity 环境中使用
    /// </remarks>
    /// <example>
    /// <code>
    /// public class CoreServicesInstaller : InstallerBase
    /// {
    ///     public override void Install(IContainerBuilder builder)
    ///     {
    ///         builder.Register&lt;ILogger, ConsoleLogger&gt;().Singleton();
    ///         builder.Register&lt;IEventBus, EventBus&gt;().Singleton();
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class InstallerBase : IInstaller
    {
        /// <summary>
        /// 安装服务到容器构建器
        /// </summary>
        /// <param name="builder">容器构建器</param>
        public abstract void Install(IContainerBuilder builder);
    }
}
