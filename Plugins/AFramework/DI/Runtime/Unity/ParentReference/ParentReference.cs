// ==========================================================
// 文件名：ParentReference.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine
// 功能: 定义父容器引用的抽象基类，支持多种父容器查找策略
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 父容器引用抽象基类
    /// <para>定义父容器查找的抽象接口，支持多种查找策略</para>
    /// <para>Abstract base class for parent container reference with multiple lookup strategies</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>策略模式：不同的查找方式封装为不同的子类</item>
    /// <item>开闭原则：新增查找方式只需添加新子类</item>
    /// <item>单一职责：每个子类只负责一种查找逻辑</item>
    /// </list>
    /// 
    /// 内置实现：
    /// <list type="bullet">
    /// <item>ParentReferenceAuto：自动向上查找父 LifetimeScope</item>
    /// <item>ParentReferenceDirect：直接引用指定的 LifetimeScope</item>
    /// <item>ParentReferenceByType：按类型查找 LifetimeScope</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public abstract class ParentReference
    {
        #region 抽象方法 / Abstract Methods

        /// <summary>
        /// 获取父 LifetimeScope
        /// <para>Get the parent LifetimeScope</para>
        /// </summary>
        /// <param name="current">当前 LifetimeScope / Current LifetimeScope</param>
        /// <returns>父 LifetimeScope，如果没有则返回 null / Parent LifetimeScope or null if not found</returns>
        public abstract LifetimeScope GetParent(LifetimeScope current);

        /// <summary>
        /// 获取引用类型的显示名称（用于编辑器）
        /// <para>Get the display name of the reference type (for editor)</para>
        /// </summary>
        public abstract string DisplayName { get; }

        #endregion

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建自动查找的父引用
        /// <para>Create an auto-lookup parent reference</para>
        /// </summary>
        /// <returns>自动查找父引用实例 / Auto-lookup parent reference instance</returns>
        public static ParentReference Auto() => new ParentReferenceAuto();

        /// <summary>
        /// 创建直接引用的父引用
        /// <para>Create a direct parent reference</para>
        /// </summary>
        /// <param name="parent">父 LifetimeScope / Parent LifetimeScope</param>
        /// <returns>直接引用父引用实例 / Direct parent reference instance</returns>
        public static ParentReference Direct(LifetimeScope parent) => new ParentReferenceDirect(parent);

        /// <summary>
        /// 创建按类型查找的父引用
        /// <para>Create a type-based parent reference</para>
        /// </summary>
        /// <typeparam name="T">LifetimeScope 子类型 / LifetimeScope subtype</typeparam>
        /// <returns>按类型查找父引用实例 / Type-based parent reference instance</returns>
        public static ParentReference ByType<T>() where T : LifetimeScope => new ParentReferenceByType(typeof(T));

        /// <summary>
        /// 创建无父容器的引用
        /// <para>Create a reference with no parent</para>
        /// </summary>
        /// <returns>空父引用实例 / Null parent reference instance</returns>
        public static ParentReference None() => new ParentReferenceNone();

        #endregion
    }

    /// <summary>
    /// 无父容器引用
    /// <para>表示不需要父容器的引用类型</para>
    /// <para>Represents a reference type that doesn't need a parent container</para>
    /// </summary>
    [Serializable]
    internal sealed class ParentReferenceNone : ParentReference
    {
        /// <inheritdoc/>
        public override string DisplayName => "None";

        /// <inheritdoc/>
        public override LifetimeScope GetParent(LifetimeScope current) => null;
    }
}
