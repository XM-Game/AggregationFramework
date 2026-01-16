// ==========================================================
// 文件名：FromParentAttribute.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 [FromParent] 特性，指定从父容器解析依赖

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 父容器解析特性
    /// <para>指定依赖必须从父容器解析，跳过当前容器的注册</para>
    /// <para>FromParent attribute that specifies resolving dependency from parent container</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>子作用域需要访问父作用域的特定服务</item>
    /// <item>避免子容器覆盖父容器的注册</item>
    /// <item>明确指定依赖来源，提高代码可读性</item>
    /// </list>
    /// 
    /// 行为说明：
    /// <list type="bullet">
    /// <item>跳过当前容器的注册，直接从父容器解析</item>
    /// <item>如果父容器也未注册，继续向上查找</item>
    /// <item>如果没有父容器或所有父容器都未注册，抛出异常</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 场景：子场景需要访问根容器的全局服务
    /// public class SceneController
    /// {
    ///     // 强制从父容器获取全局配置服务
    ///     [Inject]
    ///     [FromParent]
    ///     private IGlobalConfigService _globalConfig;
    ///     
    ///     // 从当前容器获取场景级服务
    ///     [Inject]
    ///     private ISceneService _sceneService;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(
        AttributeTargets.Parameter | 
        AttributeTargets.Property | 
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class FromParentAttribute : Attribute
    {
        /// <summary>
        /// 创建父容器解析特性实例
        /// </summary>
        public FromParentAttribute()
        {
        }
    }
}
