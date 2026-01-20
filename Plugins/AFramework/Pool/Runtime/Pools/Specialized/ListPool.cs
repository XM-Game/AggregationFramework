// ==========================================================
// 文件名：ListPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: List<T> 对象池，优化集合分配性能
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.Pool
{
    /// <summary>
    /// List 对象池
    /// List Object Pool
    /// </summary>
    /// <typeparam name="T">列表元素类型 / List element type</typeparam>
    public class ListPool<T> : ObjectPoolBase<List<T>>
    {
        #region 字段 Fields

        private readonly int _defaultCapacity;
        private readonly Stack<List<T>> _pool;
        private int _activeCount;
        private readonly int _maxCapacity;

        #endregion

        #region 属性 Properties

        public int DefaultCapacity => _defaultCapacity;

        public override Type ObjectType => typeof(List<T>);

        public override int AvailableCount => _pool.Count;

        public override int ActiveCount => _activeCount;

        public override int TotalCount => _pool.Count + _activeCount;

        #endregion

        #region 构造函数 Constructors

        public ListPool(int defaultCapacity = 16, int initialPoolSize = 10, int maxCapacity = 100)
            : base(new ListCreationPolicy<T>(defaultCapacity), null, null, $"ListPool<{typeof(T).Name}>")
        {
            _defaultCapacity = defaultCapacity;
            _pool = new Stack<List<T>>(initialPoolSize);
            _activeCount = 0;
            _maxCapacity = maxCapacity;
        }

        #endregion

        #region 抽象方法实现

        protected override bool TryGetFromPool(out List<T> obj)
        {
            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                return true;
            }

            obj = default;
            return false;
        }

        protected override bool TryReturnToPool(List<T> obj)
        {
            if (_pool.Count >= _maxCapacity)
            {
                return false;
            }

            obj.Clear();
            _pool.Push(obj);
            return true;
        }

        protected override void OnClear()
        {
            _pool.Clear();
        }

        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            if (count <= 0) return;

            lock (_lock)
            {
                int targetCount = Math.Min(count, _maxCapacity);
                for (int i = _pool.Count; i < targetCount; i++)
                {
                    var list = new List<T>(_defaultCapacity);
                    _pool.Push(list);
                }
            }
        }

        public override int Shrink(int targetCount)
        {
            ThrowIfNotActive();

            if (targetCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetCount));
            }

            lock (_lock)
            {
                int removed = 0;
                while (_pool.Count > targetCount)
                {
                    _pool.Pop();
                    removed++;
                }
                return removed;
            }
        }

        /// <summary>
        /// 归还对象到池中（重写以正确处理活跃计数）
        /// Return object to pool (override to correctly handle active count)
        /// </summary>
        /// <param name="obj">要归还的对象 / Object to return</param>
        public new void Return(List<T> obj)
        {
            base.Return(obj);
        }

        #endregion

        #region 静态实例

        private static readonly Lazy<ListPool<T>> _shared = new Lazy<ListPool<T>>(() => new ListPool<T>());

        public static ListPool<T> Shared => _shared.Value;

        #endregion

        #region 嵌套类

        private class ListCreationPolicy<TElement> : IPoolCreationPolicy<List<TElement>>
        {
            private readonly int _capacity;

            public string Name => "ListCreationPolicy";
            public string Description => $"Creates List<{typeof(TElement).Name}> with capacity {_capacity}";

            public ListCreationPolicy(int capacity)
            {
                _capacity = capacity;
            }

            public List<TElement> Create()
            {
                return new List<TElement>(_capacity);
            }

            public bool Validate()
            {
                return _capacity >= 0;
            }
        }

        #endregion
    }
}
