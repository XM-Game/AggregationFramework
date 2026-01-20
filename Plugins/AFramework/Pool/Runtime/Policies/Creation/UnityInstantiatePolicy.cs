// ==========================================================
// 文件名：UnityInstantiatePolicy.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: Unity Instantiate 创建策略
// ==========================================================

using System;
using UnityEngine;


namespace AFramework.Pool
{
    /// <summary>
    /// Unity Instantiate 创建策略
    /// Unity Instantiate Creation Policy
    /// </summary>
    /// <typeparam name="T">组件类型 Component type</typeparam>
    /// <remarks>
    /// 使用 Unity Object.Instantiate 创建对象
    /// Uses Unity Object.Instantiate to create objects
    /// </remarks>
    public class UnityInstantiatePolicy<T> : PoolPolicyBase<T>, IPoolCreationPolicy<T> where T : Component
    {
        #region 字段 Fields

        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly bool _worldPositionStays;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 预制体
        /// Prefab
        /// </summary>
        public T Prefab => _prefab;

        /// <summary>
        /// 父级变换
        /// Parent transform
        /// </summary>
        public Transform Parent => _parent;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Unity Instantiate 策略
        /// Create Unity Instantiate policy
        /// </summary>
        /// <param name="prefab">预制体 Prefab</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="worldPositionStays">是否保持世界坐标 Whether to keep world position</param>
        public UnityInstantiatePolicy(T prefab, Transform parent = null, bool worldPositionStays = false)
        {
            _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
            _parent = parent;
            _worldPositionStays = worldPositionStays;
        }

        #endregion

        #region IPoolCreationPolicy 实现

        /// <summary>
        /// 创建对象
        /// Create object
        /// </summary>
        public T Create()
        {
            var instance = UnityEngine.Object.Instantiate(_prefab, _parent, _worldPositionStays);
            instance.name = $"{_prefab.name} (Pooled)";
            return instance;
        }

        public bool Validate()
        {
            return _prefab != null;
        }

        #endregion
    }

    /// <summary>
    /// GameObject Instantiate 创建策略
    /// GameObject Instantiate Creation Policy
    /// </summary>
    public class GameObjectInstantiatePolicy : PoolPolicyBase<GameObject>, IPoolCreationPolicy<GameObject>
    {
        #region 字段 Fields

        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly bool _worldPositionStays;

        #endregion

        #region 属性 Properties

        public GameObject Prefab => _prefab;
        public Transform Parent => _parent;

        #endregion

        #region 构造函数 Constructors

        public GameObjectInstantiatePolicy(GameObject prefab, Transform parent = null, bool worldPositionStays = false)
        {
            _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
            _parent = parent;
            _worldPositionStays = worldPositionStays;
        }

        #endregion

        #region IPoolCreationPolicy 实现

        public GameObject Create()
        {
            var instance = UnityEngine.Object.Instantiate(_prefab, _parent, _worldPositionStays);
            instance.name = $"{_prefab.name} (Pooled)";
            return instance;
        }

        public bool Validate()
        {
            return _prefab != null;
        }

        #endregion
    }
}
