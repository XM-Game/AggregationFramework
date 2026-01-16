// ==========================================================
// 文件名：Lifetime.cs
// 命名空间: AFramework.DI
// 依赖: 无
// 功能: 定义服务生命周期枚举

// ==========================================================

namespace AFramework.DI
{
    /// <summary>
    /// 服务生命周期枚举
    /// <para>定义服务实例的生命周期管理策略</para>
    /// <para>Service lifetime enumeration that defines instance lifecycle management strategy</para>
    /// </summary>
    /// <remarks>
    /// 生命周期选择指南：
    /// <list type="bullet">
    /// <item>Singleton：全局共享状态、无状态服务、昂贵的初始化</item>
    /// <item>Scoped：场景级服务、请求级服务、需要隔离的状态</item>
    /// <item>Transient：无状态操作、轻量级对象、每次需要新实例</item>
    /// </list>
    /// </remarks>
    public enum Lifetime
    {
        /// <summary>
        /// 单例生命周期
        /// <para>Singleton lifetime</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>容器生命周期内只创建一个实例</item>
        /// <item>所有解析请求返回同一实例</item>
        /// <item>容器销毁时实例被销毁</item>
        /// <item>线程安全的实例创建</item>
        /// </list>
        /// 
        /// 适用场景：
        /// <list type="bullet">
        /// <item>配置服务 (IConfigService)</item>
        /// <item>日志服务 (ILogService)</item>
        /// <item>事件总线 (IEventBus)</item>
        /// <item>缓存服务 (ICacheService)</item>
        /// </list>
        /// </remarks>
        Singleton = 0,

        /// <summary>
        /// 作用域生命周期
        /// <para>Scoped lifetime</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>每个作用域内只创建一个实例</item>
        /// <item>不同作用域拥有不同实例</item>
        /// <item>作用域销毁时实例被销毁</item>
        /// <item>子作用域不继承父作用域的 Scoped 实例</item>
        /// </list>
        /// 
        /// 适用场景：
        /// <list type="bullet">
        /// <item>场景管理器 (ISceneManager)</item>
        /// <item>UI 管理器 (IUIManager)</item>
        /// <item>资源管理器 (IResourceManager)</item>
        /// <item>请求上下文 (IRequestContext)</item>
        /// </list>
        /// </remarks>
        Scoped = 1,

        /// <summary>
        /// 瞬态生命周期
        /// <para>Transient lifetime</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>每次解析都创建新实例</item>
        /// <item>容器不跟踪实例生命周期</item>
        /// <item>调用者负责实例的销毁</item>
        /// <item>适合轻量级、无状态对象</item>
        /// </list>
        /// 
        /// 适用场景：
        /// <list type="bullet">
        /// <item>命令对象 (ICommand)</item>
        /// <item>数据传输对象 (DTO)</item>
        /// <item>临时计算器</item>
        /// <item>工厂创建的对象</item>
        /// </list>
        /// 
        /// 注意事项：
        /// <list type="bullet">
        /// <item>Singleton 和 Scoped 服务不应直接依赖 Transient 服务</item>
        /// <item>如需在长生命周期服务中使用，应通过 Func&lt;T&gt; 或工厂模式</item>
        /// </list>
        /// </remarks>
        Transient = 2
    }
}
