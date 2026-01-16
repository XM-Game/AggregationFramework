// ==========================================================
// 文件名：IInstaller.cs
// 命名空间: AFramework.DI
// 依赖: 无
// 功能: 定义安装器接口，用于模块化组织服务注册逻辑
// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 安装器接口
    /// <para>定义模块化服务注册的能力，将相关服务的注册逻辑封装在一起</para>
    /// <para>Installer interface that defines modular service registration capability</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个安装器负责一组相关服务的注册</item>
    /// <item>模块化：将注册逻辑按功能模块组织</item>
    /// <item>可复用：安装器可在不同容器中复用</item>
    /// </list>
    /// 
    /// 使用场景：
    /// <list type="bullet">
    /// <item>按功能模块组织注册（如 UIInstaller、NetworkInstaller）</item>
    /// <item>按层级组织注册（如 InfrastructureInstaller、ApplicationInstaller）</item>
    /// <item>条件注册（如 DebugInstaller、ReleaseInstaller）</item>
    /// </list>
    /// 
    /// 内置实现：
    /// <list type="bullet">
    /// <item>InstallerBase：纯 C# 安装器基类</item>
    /// <item>MonoInstaller：MonoBehaviour 安装器，可在 Inspector 中配置</item>
    /// <item>ScriptableObjectInstaller：ScriptableObject 安装器，可作为资产复用</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 示例：创建一个服务安装器
    /// public class GameServicesInstaller : IInstaller
    /// {
    ///     public void Install(IContainerBuilder builder)
    ///     {
    ///         builder.Register&lt;IGameManager, GameManager&gt;().Singleton();
    ///         builder.Register&lt;IScoreService, ScoreService&gt;().Scoped();
    ///         builder.Register&lt;IInputHandler, InputHandler&gt;().Transient();
    ///     }
    /// }
    /// 
    /// // 使用安装器
    /// builder.UseInstaller&lt;GameServicesInstaller&gt;();
    /// </code>
    /// </example>
    public interface IInstaller
    {
        /// <summary>
        /// 安装服务到容器构建器
        /// <para>Install services to the container builder</para>
        /// </summary>
        /// <param name="builder">容器构建器 / Container builder</param>
        /// <remarks>
        /// 在此方法中注册所有相关服务。
        /// 安装器应该是无状态的，不应持有对构建器的引用。
        /// </remarks>
        void Install(IContainerBuilder builder);
    }
}
