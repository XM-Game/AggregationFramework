// ==========================================================
// 文件名：OptionalAttribute.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 [Optional] 特性，标记可选依赖

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 可选依赖特性
    /// <para>标记依赖为可选，当服务未注册时不抛出异常而是注入默认值</para>
    /// <para>Optional attribute that marks a dependency as optional</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>可选功能模块的依赖</item>
    /// <item>插件系统中的可选服务</item>
    /// <item>开发/生产环境差异化的服务</item>
    /// </list>
    /// 
    /// 行为说明：
    /// <list type="bullet">
    /// <item>当服务已注册时，正常注入服务实例</item>
    /// <item>当服务未注册时，注入 null（引用类型）或默认值（值类型）</item>
    /// <item>不会抛出 ResolutionException</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class GameManager
    /// {
    ///     // 可选的分析服务，未注册时为 null
    ///     [Inject]
    ///     [Optional]
    ///     private IAnalyticsService _analytics;
    ///     
    ///     // 构造函数参数也可以标记为可选
    ///     [Inject]
    ///     public GameManager(
    ///         IGameService gameService,
    ///         [Optional] IDebugService debugService = null)
    ///     {
    ///         // debugService 在未注册时为 null
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(
        AttributeTargets.Parameter | 
        AttributeTargets.Property | 
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OptionalAttribute : Attribute
    {
        /// <summary>
        /// 创建可选依赖特性实例
        /// </summary>
        public OptionalAttribute()
        {
        }
    }
}
