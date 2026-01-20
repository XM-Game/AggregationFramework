// ==========================================================
// 文件名：ArrayPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: 数组对象池，支持租借模式和零 GC 分配
// ==========================================================

using System;


namespace AFramework.Pool
{
    /// <summary>
    /// 数组对象池
    /// Array Object Pool
    /// </summary>
    /// <typeparam name="T">数组元素类型 Array element type</typeparam>
    public class ArrayPool<T> : ObjectPoolBase<T[]>
    {
        #region 字段 Fields

        private readonly int _arrayLength;
        private readonly System.Collections.Generic.Stack<T[]> _pool;
        private int _activeCount;

        #endregion

        #region 属性 Properties

        /// <inheritdoc />
        public override Type ObjectType => typeof(T[]);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        public int ArrayLength => _arrayLength;

        #endregion

        #region 构造函数 Constructors

        public ArrayPool(int arrayLength, int initialPoolSize = 10)
            : base(new FuncCreationPolicy(() => new T[arrayLength]), null, null, $"ArrayPool<{typeof(T).Name}>[{arrayLength}]")
        {
            if (arrayLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(arrayLength));

            _arrayLength = arrayLength;
            _pool = new System.Collections.Generic.Stack<T[]>(initialPoolSize);
            _activeCount = 0;
        }

        #endregion

        #region 抽象方法实现 Abstract Method Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out T[] obj)
        {
            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                Array.Clear(obj, 0, obj.Length);
                return true;
            }

            obj = default;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(T[] obj)
        {
            if (obj == null)
                return false;

            if (obj.Length != _arrayLength)
            {
                // 数组长度不匹配，不能归还到池中
                // Array length mismatch, cannot return to pool
                return false;
            }

            Array.Clear(obj, 0, obj.Length);
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
                var array = new T[_arrayLength];
                _pool.Push(array);
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

        public new T[] Get()
        {
            return ((IObjectPool<T[]>)this).Get();
        }

        public new bool Return(T[] array)
        {
            return base.Return(array);
        }

        public new T[] Rent() => Get();
        public void Release(T[] array) => Return(array);

        #endregion

        #region 静态实例 Static Instance

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, ArrayPool<T>> _sharedPools =
            new System.Collections.Concurrent.ConcurrentDictionary<int, ArrayPool<T>>();

        public static ArrayPool<T> GetShared(int arrayLength)
        {
            return _sharedPools.GetOrAdd(arrayLength, len => new ArrayPool<T>(len));
        }

        #endregion

        #region 嵌套类 Nested Classes

        /// <summary>
        /// 基于函数的创建策略
        /// Function-based creation policy
        /// </summary>
        private class FuncCreationPolicy : IPoolCreationPolicy<T[]>
        {
            private readonly Func<T[]> _createFunc;

            public string Name => "FuncCreationPolicy";
            public string Description => "Function-based creation policy for arrays";

            public FuncCreationPolicy(Func<T[]> createFunc)
            {
                _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            }

            public T[] Create()
            {
                return _createFunc();
            }

            public bool Validate()
            {
                return _createFunc != null;
            }
        }

        #endregion
    }
}
