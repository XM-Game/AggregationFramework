// ==========================================================
// 文件名：ParentReferenceByType.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: 按类型查找父 LifetimeScope 的引用实现
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// 按类型查找父容器引用
    /// <para>在场景中查找指定类型的 LifetimeScope 作为父容器</para>
    /// <para>Type-based parent reference that finds a specific LifetimeScope type in scene</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>需要查找特定类型的 LifetimeScope</item>
    /// <item>跨场景引用特定的容器</item>
    /// <item>模块化的容器组织结构</item>
    /// </list>
    /// 
    /// 注意事项：
    /// <list type="bullet">
    /// <item>使用 FindObjectOfType，有一定性能开销</item>
    /// <item>确保场景中只有一个指定类型的 LifetimeScope</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public sealed class ParentReferenceByType : ParentReference
    {
        #region 字段 / Fields

        [SerializeField]
        [Tooltip("要查找的 LifetimeScope 类型名称 / Type name of LifetimeScope to find")]
        private string _typeName;

        // 缓存的类型
        [NonSerialized]
        private Type _cachedType;

        // 缓存的父容器
        [NonSerialized]
        private LifetimeScope _cachedParent;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public override string DisplayName => $"ByType ({_typeName ?? "未指定"})";

        /// <summary>
        /// 获取目标类型
        /// <para>Get the target type</para>
        /// </summary>
        public Type TargetType
        {
            get
            {
                if (_cachedType == null && !string.IsNullOrEmpty(_typeName))
                {
                    _cachedType = Type.GetType(_typeName);
                }
                return _cachedType;
            }
        }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建按类型查找父容器实例（无参构造，用于序列化）
        /// </summary>
        public ParentReferenceByType()
        {
        }

        /// <summary>
        /// 创建按类型查找父容器实例
        /// </summary>
        /// <param name="type">要查找的 LifetimeScope 类型 / LifetimeScope type to find</param>
        public ParentReferenceByType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            
            if (!typeof(LifetimeScope).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"类型 '{type.Name}' 必须继承自 LifetimeScope。\n" +
                    $"Type '{type.Name}' must inherit from LifetimeScope.",
                    nameof(type));
            }

            _typeName = type.AssemblyQualifiedName;
            _cachedType = type;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <inheritdoc/>
        public override LifetimeScope GetParent(LifetimeScope current)
        {
            // 使用缓存
            if (_cachedParent != null)
                return _cachedParent;

            var targetType = TargetType;
            if (targetType == null)
            {
                Debug.LogWarning(
                    $"[AFramework.DI] 无法解析类型 '{_typeName}'。\n" +
                    $"Cannot resolve type '{_typeName}'.");
                return null;
            }

            // 查找指定类型的 LifetimeScope
            _cachedParent = UnityEngine.Object.FindObjectOfType(targetType) as LifetimeScope;

            if (_cachedParent == null)
            {
                Debug.LogWarning(
                    $"[AFramework.DI] 未找到类型为 '{targetType.Name}' 的 LifetimeScope。\n" +
                    $"LifetimeScope of type '{targetType.Name}' not found.");
            }
            else if (_cachedParent == current)
            {
                Debug.LogWarning(
                    $"[AFramework.DI] LifetimeScope '{current.name}' 不能将自身设置为父容器。\n" +
                    $"LifetimeScope '{current.name}' cannot set itself as parent.");
                _cachedParent = null;
            }

            return _cachedParent;
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear the cached parent reference</para>
        /// </summary>
        public void ClearCache()
        {
            _cachedParent = null;
        }

        #endregion
    }
}
