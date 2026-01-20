// ==========================================================
// 文件名：ComponentPool.cs
// 命名空间: AFramework.Pool
// 依赖: UnityEngine, AFramework.Pool
// 功能: 组件对象池实现，支持任意 Unity 组件类型
// ==========================================================

using System;
using UnityEngine;


namespace AFramework.Pool
{
    /// <summary>
    /// 组件对象池
    /// Component Object Pool
    /// </summary>
    /// <typeparam name="T">组件类型 Component type</typeparam>
    /// <remarks>
    /// 专为 Unity 组件设计的对象池，支持：
    /// - 任意组件类型（T : Component）
    /// - 自动 GameObject 管理
    /// - 组件生命周期回调
    /// - 父级变换管理
    /// Designed specifically for Unity components, supports:
    /// - Any component type (T : Component)
    /// - Automatic GameObject management
    /// - Component lifecycle callbacks
    /// - Parent transform management
    /// </remarks>
    public class ComponentPool<T> : ObjectPoolBase<T> where T : Component
    {
        #region 字段 Fields

        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly bool _worldPositionStays;
        private readonly System.Collections.Generic.Stack<T> _pool;
        private readonly System.Collections.Generic.HashSet<T> _activeObjects;
        private int _maxCapacity;

        #endregion

        #region 属性 Properties

        /// <inheritdoc />
        public override Type ObjectType => typeof(T);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeObjects.Count;

        /// <inheritdoc />
        public override int ActiveCount => _activeObjects.Count;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        /// <summary>
        /// 预制体组件引用
        /// Prefab component reference
        /// </summary>
        public T Prefab => _prefab;

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

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建组件对象池
        /// Create component object pool
        /// </summary>
        /// <param name="prefab">预制体组件 Prefab component</param>
        /// <param name="parent">父级变换（可选）Parent transform (optional)</param>
        /// <param name="initialCapacity">初始容量 Initial capacity</param>
        /// <param name="maxCapacity">最大容量 Maximum capacity</param>
        /// <param name="worldPositionStays">是否保持世界坐标 Whether to keep world position</param>
        public ComponentPool(
            T prefab,
            Transform parent = null,
            int initialCapacity = 10,
            int maxCapacity = 100,
            bool worldPositionStays = false)
            : base(new ComponentCreationPolicy<T>(prefab, parent, worldPositionStays), null, null, $"ComponentPool<{typeof(T).Name}>")
        {
            _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
            _parent = parent;
            _worldPositionStays = worldPositionStays;
            _pool = new System.Collections.Generic.Stack<T>(initialCapacity);
            _activeObjects = new System.Collections.Generic.HashSet<T>();
            _maxCapacity = maxCapacity;
        }

        #endregion

        #region 抽象方法实现 Abstract Method Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T obj)
        {
            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                OnGetFromPool(obj);
                _activeObjects.Add(obj);
                obj.gameObject.SetActive(true);
                return true;
            }

            obj = default;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(T obj)
        {
            if (obj == null)
                return false;

            if (!_activeObjects.Remove(obj))
            {
                Debug.LogWarning($"[ComponentPool] 尝试归还不属于此池的组件 Attempting to return component not from this pool: {obj.name}");
                return false;
            }

            if (_pool.Count < _maxCapacity)
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_parent, _worldPositionStays);
                OnReturnToPool(obj);
                _pool.Push(obj);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void OnClear()
        {
            // 销毁空闲对象 Destroy idle objects
            while (_pool.Count > 0)
            {
                var component = _pool.Pop();
                if (component != null)
                {
                    OnDestroy(component);
                    UnityEngine.Object.Destroy(component.gameObject);
                }
            }

            // 警告：活跃对象未归还 Warning: Active objects not returned
            if (_activeObjects.Count > 0)
            {
                Debug.LogWarning($"[ComponentPool] 清空池时仍有 {_activeObjects.Count} 个活跃组件未归还 " +
                    $"Clearing pool with {_activeObjects.Count} active components not returned");
            }

            _activeObjects.Clear();
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            for (int i = 0; i < count; i++)
            {
                var component = CreateNew();
                component.gameObject.SetActive(false);
                component.transform.SetParent(_parent, _worldPositionStays);
                _pool.Push(component);
            }
        }

        /// <inheritdoc />
        public override int Shrink(int targetCount)
        {
            ThrowIfDisposed();

            if (targetCount < 0)
                throw new ArgumentOutOfRangeException(nameof(targetCount));

            int removedCount = 0;

            while (_pool.Count > targetCount)
            {
                var component = _pool.Pop();
                if (component != null)
                {
                    OnDestroy(component);
                    UnityEngine.Object.Destroy(component.gameObject);
                    removedCount++;
                }
            }

            return removedCount;
        }

        #endregion

        #region 公共方法 Public Methods

        /// <summary>
        /// 获取组件
        /// Get component
        /// </summary>
        public new T Get()
        {
            return ((IObjectPool<T>)this).Get();
        }

        /// <summary>
        /// 归还组件
        /// Return component
        /// </summary>
        public new bool Return(T component)
        {
            return base.Return(component);
        }

        #endregion

        #region 虚方法 Virtual Methods

        /// <summary>
        /// 创建新组件
        /// Create new component
        /// </summary>
        protected virtual T CreateNew()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab, _parent, _worldPositionStays);
            obj.name = $"{_prefab.name} (Pooled)";
            return obj;
        }

        /// <summary>
        /// 从池中获取时调用
        /// Called when getting from pool
        /// </summary>
        protected virtual void OnGetFromPool(T component)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 创建新组件时调用
        /// Called when creating new component
        /// </summary>
        protected virtual void OnCreateNew(T component)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 创建对象
        /// Create object
        /// </summary>
        protected override T CreateObject()
        {
            var component = CreateNew();
            OnCreateNew(component);
            return component;
        }

        /// <summary>
        /// 归还到池时调用
        /// Called when returning to pool
        /// </summary>
        protected virtual void OnReturnToPool(T component)
        {
            // 子类可重写 Subclasses can override
        }

        /// <summary>
        /// 销毁组件时调用
        /// Called when destroying component
        /// </summary>
        protected virtual void OnDestroy(T component)
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

        #region 嵌套类 Nested Classes

        /// <summary>
        /// 组件创建策略
        /// Component creation policy
        /// </summary>
        private class ComponentCreationPolicy<TComponent> : IPoolCreationPolicy<TComponent> where TComponent : Component
        {
            private readonly TComponent _prefab;
            private readonly Transform _parent;
            private readonly bool _worldPositionStays;

            public string Name => "ComponentCreationPolicy";
            public string Description => $"Creates {typeof(TComponent).Name} from prefab";

            public ComponentCreationPolicy(TComponent prefab, Transform parent, bool worldPositionStays)
            {
                _prefab = prefab;
                _parent = parent;
                _worldPositionStays = worldPositionStays;
            }

            public TComponent Create()
            {
                var obj = UnityEngine.Object.Instantiate(_prefab, _parent, _worldPositionStays);
                obj.name = $"{_prefab.name} (Pooled)";
                return obj;
            }

            public bool Validate()
            {
                return _prefab != null;
            }
        }

        #endregion
    }
}
