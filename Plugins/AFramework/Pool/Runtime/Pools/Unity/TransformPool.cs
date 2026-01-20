// ==========================================================
// 文件名：TransformPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: Transform 对象池，用于层级管理和空对象池化
// ==========================================================

using System;
using UnityEngine;

namespace AFramework.Pool
{
    /// <summary>
    /// Transform 对象池
    /// Transform Object Pool
    /// </summary>
    /// <remarks>
    /// 专为 Unity Transform 设计的对象池，支持：
    /// - 空 GameObject 池化
    /// - 层级结构管理
    /// - 位置重置
    /// - 父子关系管理
    /// Designed specifically for Unity Transform, supports:
    /// - Empty GameObject pooling
    /// - Hierarchy management
    /// - Position reset
    /// - Parent-child relationship management
    /// </remarks>
    public class TransformPool : ComponentPool<Transform>
    {
        #region 字段 Fields

        private readonly string _objectName;
        private readonly bool _resetTransform;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 对象名称
        /// Object name
        /// </summary>
        public string ObjectName => _objectName;

        /// <summary>
        /// 是否重置变换
        /// Whether to reset transform
        /// </summary>
        public bool ResetTransform => _resetTransform;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Transform 对象池
        /// Create Transform object pool
        /// </summary>
        /// <param name="objectName">对象名称 Object name</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="initialCapacity">初始容量 Initial capacity</param>
        /// <param name="maxCapacity">最大容量 Maximum capacity</param>
        /// <param name="resetTransform">是否重置变换 Whether to reset transform</param>
        public TransformPool(
            string objectName = "PooledTransform",
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool resetTransform = true)
            : base(CreateEmptyPrefab(objectName), parent, initialCapacity, maxCapacity)
        {
            _objectName = objectName;
            _resetTransform = resetTransform;
        }

        #endregion

        #region 虚方法重写 Virtual Method Overrides

        protected override Transform CreateNew()
        {
            var go = new GameObject(_objectName);
            if (Parent != null)
            {
                go.transform.SetParent(Parent, false);
            }
            return go.transform;
        }

        protected override void OnGetFromPool(Transform transform)
        {
            base.OnGetFromPool(transform);

            if (_resetTransform)
            {
                ResetTransformValues(transform);
            }
        }

        protected override void OnReturnToPool(Transform transform)
        {
            base.OnReturnToPool(transform);

            if (_resetTransform)
            {
                ResetTransformValues(transform);
            }
        }

        #endregion

        #region 私有方法 Private Methods

        /// <summary>
        /// 创建空预制体
        /// Create empty prefab
        /// </summary>
        private static Transform CreateEmptyPrefab(string name)
        {
            var go = new GameObject(name);
            var transform = go.transform;
            go.SetActive(false);
            return transform;
        }

        /// <summary>
        /// 重置变换值
        /// Reset transform values
        /// </summary>
        private void ResetTransformValues(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        #endregion
    }
}
