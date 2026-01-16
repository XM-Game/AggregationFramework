// ==========================================================
// 文件名：InjectionType.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义注入类型枚举

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 注入类型枚举
    /// <para>定义依赖注入的方式类型</para>
    /// <para>Injection type enumeration that defines dependency injection methods</para>
    /// </summary>
    /// <remarks>
    /// 注入优先级（从高到低）：
    /// <list type="number">
    /// <item>Constructor：构造函数注入，最推荐的方式</item>
    /// <item>Method：方法注入，适合需要初始化逻辑的场景</item>
    /// <item>Property：属性注入，适合可选依赖</item>
    /// <item>Field：字段注入，适合 MonoBehaviour</item>
    /// </list>
    /// </remarks>
    [Flags]
    public enum InjectionType
    {
        /// <summary>
        /// 无注入
        /// <para>No injection</para>
        /// </summary>
        None = 0,

        /// <summary>
        /// 构造函数注入
        /// <para>Constructor injection</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>最推荐的注入方式</item>
        /// <item>确保对象创建时所有必需依赖都已就绪</item>
        /// <item>依赖通过 readonly 字段保存，保证不可变性</item>
        /// <item>依赖关系在构造函数签名中清晰可见</item>
        /// <item>易于单元测试</item>
        /// </list>
        /// </remarks>
        Constructor = 1 << 0,

        /// <summary>
        /// 方法注入
        /// <para>Method injection</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>通过 [Inject] 标记的方法注入</item>
        /// <item>可在注入时执行初始化逻辑</item>
        /// <item>支持多个注入方法，按声明顺序调用</item>
        /// <item>基类的注入方法也会被调用</item>
        /// </list>
        /// </remarks>
        Method = 1 << 1,

        /// <summary>
        /// 属性注入
        /// <para>Property injection</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>通过 [Inject] 标记的属性注入</item>
        /// <item>适合可选依赖</item>
        /// <item>可用于解决循环依赖</item>
        /// <item>比字段注入有更好的封装性</item>
        /// </list>
        /// </remarks>
        Property = 1 << 2,

        /// <summary>
        /// 字段注入
        /// <para>Field injection</para>
        /// </summary>
        /// <remarks>
        /// 特点：
        /// <list type="bullet">
        /// <item>通过 [Inject] 标记的字段注入</item>
        /// <item>代码最简洁</item>
        /// <item>适合 MonoBehaviour（无法使用构造函数）</item>
        /// <item>封装性较差，不推荐在纯 C# 类中使用</item>
        /// </list>
        /// </remarks>
        Field = 1 << 3,

        /// <summary>
        /// 所有注入类型
        /// <para>All injection types</para>
        /// </summary>
        All = Constructor | Method | Property | Field
    }
}
