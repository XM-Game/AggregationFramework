// ==========================================================
// 文件名：ParentReferenceAuto.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine
// 功能: 自动查找父 LifetimeScope 的引用实现
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 自动查找父容器引用
    /// <para>按 Unity 层级结构向上查找最近的父 LifetimeScope</para>
    /// <para>Auto-lookup parent reference that searches up the Unity hierarchy</para>
    /// </summary>
    /// <remarks>
    /// 查找顺序：
    /// <list type="number">
    /// <item>在父 Transform 层级中向上查找 LifetimeScope</item>
    /// <item>如果未找到，查找场景中标记为根的 LifetimeScope</item>
    /// <item>如果仍未找到，返回 null（作为根容器）</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public sealed class ParentReferenceAuto : ParentReference
    {
        #region 属性 / Properties

        /// <inheritdoc/>
        public override string DisplayName => "Auto (自动查找)";

        #endregion

        #region 公共方法 / Public Methods

        /// <inheritdoc/>
        public override LifetimeScope GetParent(LifetimeScope current)
        {
            if (current == null)
                return null;

            // 1. 在父 Transform 层级中查找
            var parent = FindInHierarchy(current.transform.parent);
            if (parent != null)
                return parent;

            // 2. 查找场景中的根 LifetimeScope
            return FindRootScope(current);
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 在 Transform 层级中向上查找 LifetimeScope
        /// </summary>
        private static LifetimeScope FindInHierarchy(Transform parent)
        {
            while (parent != null)
            {
                var scope = parent.GetComponent<LifetimeScope>();
                if (scope != null)
                    return scope;
                
                parent = parent.parent;
            }
            return null;
        }

        /// <summary>
        /// 查找场景中的根 LifetimeScope
        /// </summary>
        private static LifetimeScope FindRootScope(LifetimeScope current)
        {
            // 查找所有 LifetimeScope，找到标记为根的
            var allScopes = UnityEngine.Object.FindObjectsOfType<LifetimeScope>();
            
            foreach (var scope in allScopes)
            {
                // 跳过自身
                if (scope == current)
                    continue;
                
                // 查找标记为根的 LifetimeScope
                if (scope.IsRoot)
                    return scope;
            }

            return null;
        }

        #endregion
    }
}
