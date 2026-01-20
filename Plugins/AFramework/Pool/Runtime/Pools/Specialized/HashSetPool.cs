// ==========================================================
// 文件名：HashSetPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: HashSet<T> 对象池
// ==========================================================

using System;
using System.Collections.Generic;


namespace AFramework.Pool
{
    /// <summary>
    /// HashSet 对象池
    /// HashSet Object Pool
    /// </summary>
    public class HashSetPool<T> : ObjectPoolBase<HashSet<T>>
    {
        private readonly int _defaultCapacity;
        private readonly Stack<HashSet<T>> _pool;
        private int _activeCount;

        /// <inheritdoc />
        public override Type ObjectType => typeof(HashSet<T>);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        public int DefaultCapacity => _defaultCapacity;

        public HashSetPool(int defaultCapacity = 16, int initialPoolSize = 10)
            : base(new FuncCreationPolicy(() => new HashSet<T>()), null, null, $"HashSetPool<{typeof(T).Name}>")
        {
            _defaultCapacity = defaultCapacity;
            _pool = new Stack<HashSet<T>>(initialPoolSize);
            _activeCount = 0;
        }

        #region 抽象方法实现 Abstract Method Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out HashSet<T> obj)
        {
            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                return true;
            }

            obj = default;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(HashSet<T> obj)
        {
            if (obj == null)
                return false;

            obj.Clear();
            _pool.Push(obj);
            return true;
        }

        /// <inheritdoc />
        protected override void OnClear()
        {
            _pool.Clear();
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            ThrowIfDisposed();

            if (count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                var set = new HashSet<T>();
                _pool.Push(set);
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
                _pool.Pop();
                removedCount++;
            }

            return removedCount;
        }

        #endregion

        #region 公共方法 Public Methods

        public new HashSet<T> Get()
        {
            return ((IObjectPool<HashSet<T>>)this).Get();
        }

        public new bool Return(HashSet<T> set)
        {
            return base.Return(set);
        }

        #endregion

        #region 嵌套类 Nested Classes

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        private class FuncCreationPolicy : IPoolCreationPolicy<HashSet<T>>
        {
            private readonly Func<HashSet<T>> _createFunc;

            public string Name => "FuncCreationPolicy";
            public string Description => "Function-based creation policy for hash sets";

            public FuncCreationPolicy(Func<HashSet<T>> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            public HashSet<T> Create()
            {
                return _createFunc();
            }

            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion

        private static readonly Lazy<HashSetPool<T>> _shared = new Lazy<HashSetPool<T>>(() => new HashSetPool<T>());
        public static HashSetPool<T> Shared => _shared.Value;
    }
}
