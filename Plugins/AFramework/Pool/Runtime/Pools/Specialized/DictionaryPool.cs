// ==========================================================
// 文件名：DictionaryPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: Dictionary<TKey, TValue> 对象池
// ==========================================================

using System;
using System.Collections.Generic;


namespace AFramework.Pool
{
    /// <summary>
    /// Dictionary 对象池
    /// Dictionary Object Pool
    /// </summary>
    public class DictionaryPool<TKey, TValue> : ObjectPoolBase<Dictionary<TKey, TValue>>
    {
        private readonly int _defaultCapacity;
        private readonly Stack<Dictionary<TKey, TValue>> _pool;
        private int _activeCount;

        /// <inheritdoc />
        public override Type ObjectType => typeof(Dictionary<TKey, TValue>);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        public int DefaultCapacity => _defaultCapacity;

        public DictionaryPool(int defaultCapacity = 16, int initialPoolSize = 10)
            : base(new FuncCreationPolicy(() => new Dictionary<TKey, TValue>(defaultCapacity)), null, null, $"DictionaryPool<{typeof(TKey).Name},{typeof(TValue).Name}>")
        {
            _defaultCapacity = defaultCapacity;
            _pool = new Stack<Dictionary<TKey, TValue>>(initialPoolSize);
            _activeCount = 0;
        }

        #region 抽象方法实现 Abstract Method Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out Dictionary<TKey, TValue> obj)
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
        protected override bool TryReturnToPool(Dictionary<TKey, TValue> obj)
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
                var dict = new Dictionary<TKey, TValue>(_defaultCapacity);
                _pool.Push(dict);
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

        public new Dictionary<TKey, TValue> Get()
        {
            return ((IObjectPool<Dictionary<TKey, TValue>>)this).Get();
        }

        public new bool Return(Dictionary<TKey, TValue> dict)
        {
            return base.Return(dict);
        }

        #endregion

        #region 嵌套类 Nested Classes

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        private class FuncCreationPolicy : IPoolCreationPolicy<Dictionary<TKey, TValue>>
        {
            private readonly Func<Dictionary<TKey, TValue>> _createFunc;

            public string Name => "FuncCreationPolicy";
            public string Description => "Function-based creation policy for dictionaries";

            public FuncCreationPolicy(Func<Dictionary<TKey, TValue>> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            public Dictionary<TKey, TValue> Create()
            {
                return _createFunc();
            }

            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion

        private static readonly Lazy<DictionaryPool<TKey, TValue>> _shared = 
            new Lazy<DictionaryPool<TKey, TValue>>(() => new DictionaryPool<TKey, TValue>());
        public static DictionaryPool<TKey, TValue> Shared => _shared.Value;
    }
}
