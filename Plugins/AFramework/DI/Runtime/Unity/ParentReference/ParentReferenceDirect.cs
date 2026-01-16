// ==========================================================
// 文件名：ParentReferenceDirect.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine
// 功能: 直接引用指定 LifetimeScope 的父容器引用实现
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 直接引用父容器
    /// <para>通过 Inspector 直接拖拽指定父 LifetimeScope</para>
    /// <para>Direct parent reference that allows dragging a specific LifetimeScope in Inspector</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>需要明确指定父容器时</item>
    /// <item>跨场景引用（需配合 DontDestroyOnLoad）</item>
    /// <item>复杂的容器层级结构</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public sealed class ParentReferenceDirect : ParentReference
    {
        #region 字段 / Fields

        [SerializeField]
        [Tooltip("直接引用的父 LifetimeScope / Directly referenced parent LifetimeScope")]
        private LifetimeScope _parent;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public override string DisplayName => "Direct (直接引用)";

        /// <summary>
        /// 获取或设置父 LifetimeScope
        /// <para>Get or set the parent LifetimeScope</para>
        /// </summary>
        public LifetimeScope Parent
        {
            get => _parent;
            set => _parent = value;
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建直接引用父容器实例（无参构造，用于序列化）
        /// </summary>
        public ParentReferenceDirect()
        {
        }

        /// <summary>
        /// 创建直接引用父容器实例
        /// </summary>
        /// <param name="parent">父 LifetimeScope / Parent LifetimeScope</param>
        public ParentReferenceDirect(LifetimeScope parent)
        {
            _parent = parent;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <inheritdoc/>
        public override LifetimeScope GetParent(LifetimeScope current)
        {
            // 防止自引用
            if (_parent == current)
            {
                Debug.LogWarning(
                    $"[AFramework.DI] LifetimeScope '{current.name}' 不能将自身设置为父容器。\n" +
                    $"LifetimeScope '{current.name}' cannot set itself as parent.");
                return null;
            }

            return _parent;
        }

        #endregion
    }
}
