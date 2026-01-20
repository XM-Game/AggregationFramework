// ==========================================================
// 文件名：GameObjectPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: GameObject 对象池实现，支持 Prefab 实例化和场景管理
// ==========================================================

using System;
using UnityEngine;



namespace AFramework.Pool
{
    /// <summary>
    /// GameObject 对象池
    /// GameObject Object Pool
    /// </summary>
    /// <remarks>
    /// 专为 Unity GameObject 设计的对象池，支持：
    /// - Prefab 实例化
    /// - 父级变换管理
    /// - 场景切换处理
    /// - 自动激活/停用
    /// Designed specifically for Unity GameObjects, supports:
    /// - Prefab instantiation
    /// - Parent transform management
    /// - Scene transition handling
    /// - Automatic activation/deactivation
    /// </remarks>
    public class GameObjectPool : ObjectPoolBase
    {
        #region 字段 Fields

        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly bool _worldPositionStays;
        private readonly System.Collections.Generic.Stack<GameObject> _pool;
        private readonly System.Collections.Generic.HashSet<GameObject> _activeObjects;
        private int _maxCapacity;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 预制体引用
        /// Prefab reference
        /// </summary>
        public GameObject Prefab => _prefab;

        /// <summary>
        /// 父级变换
        /// Parent transform
        /// </summary>
        public Transform Parent => _parent;

        /// <summary>
        /// 最大容量
        /// Maximum capacity
        /// </summary>
        public int MaxCapacity
        {
            get => _maxCapacity;
            set => _maxCapacity = value;
        }

        /// <inheritdoc />
        public override Type ObjectType => typeof(GameObject);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeObjects.Count;

        /// <inheritdoc />
        public override int ActiveCount => _activeObjects.Count;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 GameObject 对象池
        /// Create GameObject object pool
        /// </summary>
        /// <param name="prefab">预制体 Prefab</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="initialCapacity">初始容量 Initial capacity</param>
        /// <param name="maxCapacity">最大容量 Maximum capacity</param>
        /// <param name="worldPositionStays">是否保持世界坐标 Whether to keep world position</param>
        public GameObjectPool(
            GameObject prefab,
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool worldPositionStays = false)
        {
            _prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            _parent = parent;
            _worldPositionStays = worldPositionStays;
            _pool = new System.Collections.Generic.Stack<GameObject>(initialCapacity);
            _activeObjects = new System.Collections.Generic.HashSet<GameObject>();
            _maxCapacity = maxCapacity;
        }

        #endregion

        #region 核心方法 Core Methods

        /// <inheritdoc />
        public override object Get()
        {
            return GetGameObject();
        }

        /// <summary>
        /// 获取 GameObject 对象
        /// Get GameObject object
        /// </summary>
        public GameObject GetGameObject()
        {
            ThrowIfDisposed();

            GameObject obj;

            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                OnGetFromPool(obj);
            }
            else
            {
                obj = CreateNew();
                OnCreateNew(obj);
            }

            _activeObjects.Add(obj);
            obj.SetActive(true);

            return obj;
        }

        /// <inheritdoc />
        public override bool Return(object obj)
        {
            if (obj is GameObject gameObject)
            {
                ReturnGameObject(gameObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 归还 GameObject 对象
        /// Return GameObject object
        /// </summary>
        public void ReturnGameObject(GameObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            ThrowIfDisposed();

            if (!_activeObjects.Remove(obj))
            {
                Debug.LogWarning($"[GameObjectPool] 尝试归还不属于此池的对象 Attempting to return object not from this pool: {obj.name}");
                return;
            }

            if (_pool.Count < _maxCapacity)
            {
                obj.SetActive(false);
                obj.transform.SetParent(_parent, _worldPositionStays);
                OnReturnToPool(obj);
                _pool.Push(obj);
            }
            else
            {
                OnDestroy(obj);
                UnityEngine.Object.Destroy(obj);
            }
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            for (int i = 0; i < count; i++)
            {
                var obj = CreateNew();
                obj.SetActive(false);
                obj.transform.SetParent(_parent, _worldPositionStays);
                _pool.Push(obj);
            }
        }

        /// <inheritdoc />
        public override int Shrink(int targetCount)
        {
            ThrowIfDisposed();

            if (targetCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetCount), "Target count must be non-negative.");
            }

            int removedCount = 0;

            while (_pool.Count > targetCount)
            {
                var obj = _pool.Pop();
                if (obj != null)
                {
                    OnDestroy(obj);
                    UnityEngine.Object.Destroy(obj);
                    removedCount++;
                }
            }

            return removedCount;
        }

        /// <inheritdoc />
        protected override void OnClear()
        {
            // 销毁空闲对象 Destroy idle objects
            while (_pool.Count > 0)
            {
                var obj = _pool.Pop();
                if (obj != null)
                {
                    OnDestroy(obj);
                    UnityEngine.Object.Destroy(obj);
                }
            }

            // 警告：活跃对象未归还 Warning: Active objects not returned
            if (_activeObjects.Count > 0)
            {
                Debug.LogWarning($"[GameObjectPool] 清空池时仍有 {_activeObjects.Count} 个活跃对象未归还 " +
                    $"Clearing pool with {_activeObjects.Count} active objects not returned");
            }

            _activeObjects.Clear();
        }

        #endregion

        #region 虚方法 Virtual Methods

        /// <summary>
        /// 创建新对象
        /// Create new object
        /// </summary>
        protected virtual GameObject CreateNew()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab, _parent, _worldPositionStays);
            obj.name = $"{_prefab.name} (Pooled)";
            return obj;
        }

        /// <summary>
        /// 从池中获取时调用
        /// Called when getting from pool
        /// </summary>
        protected virtual void OnGetFromPool(GameObject obj)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 创建新对象时调用
        /// Called when creating new object
        /// </summary>
        protected virtual void OnCreateNew(GameObject obj)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 归还到池时调用
        /// Called when returning to pool
        /// </summary>
        protected virtual void OnReturnToPool(GameObject obj)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 销毁对象时调用
        /// Called when destroying object
        /// </summary>
        protected virtual void OnDestroy(GameObject obj)
        {
            // 子类可重写 Subclasses can override
        }

        #endregion

        #region IDisposable 实现 IDisposable Implementation

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                Clear();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
