// ==========================================================
// 文件名：StringBuilderPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: StringBuilder 对象池，优化字符串拼接性能
// ==========================================================

using System;
using System.Text;


namespace AFramework.Pool
{
    /// <summary>
    /// StringBuilder 对象池
    /// StringBuilder Object Pool
    /// </summary>
    /// <remarks>
    /// 专为 StringBuilder 设计的对象池，支持：
    /// - 零 GC 分配
    /// - 自动容量管理
    /// - 租借模式（Rent/Return）
    /// - 容量限制保护
    /// Designed specifically for StringBuilder, supports:
    /// - Zero GC allocation
    /// - Automatic capacity management
    /// - Rent/Return pattern
    /// - Capacity limit protection
    /// </remarks>
    public class StringBuilderPool : ObjectPoolBase<StringBuilder>
    {
        #region 字段 Fields

        private readonly int _defaultCapacity;
        private readonly int _maxRetainedCapacity;
        private readonly System.Collections.Generic.Stack<StringBuilder> _pool;
        private int _activeCount;
        private int _maxCapacity;

        #endregion

        #region 属性 Properties

        /// <inheritdoc />
        public override Type ObjectType => typeof(StringBuilder);

        /// <inheritdoc />
        public override int TotalCount => _pool.Count + _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => _pool.Count;

        /// <summary>
        /// 默认容量
        /// Default capacity
        /// </summary>
        public int DefaultCapacity => _defaultCapacity;

        /// <summary>
        /// 最大保留容量
        /// Maximum retained capacity
        /// </summary>
        public int MaxRetainedCapacity => _maxRetainedCapacity;

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
        /// 创建 StringBuilder 对象池
        /// Create StringBuilder object pool
        /// </summary>
        /// <param name="defaultCapacity">默认容量 Default capacity</param>
        /// <param name="maxRetainedCapacity">最大保留容量 Maximum retained capacity</param>
        /// <param name="initialPoolSize">初始池大小 Initial pool size</param>
        public StringBuilderPool(
            int defaultCapacity = 256,
            int maxRetainedCapacity = 4096,
            int initialPoolSize = 10)
            : base(new StringBuilderCreationPolicy(defaultCapacity), null, null, $"StringBuilderPool")
        {
            if (defaultCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(defaultCapacity));
            if (maxRetainedCapacity < defaultCapacity)
                throw new ArgumentOutOfRangeException(nameof(maxRetainedCapacity));

            _defaultCapacity = defaultCapacity;
            _maxRetainedCapacity = maxRetainedCapacity;
            _pool = new System.Collections.Generic.Stack<StringBuilder>(initialPoolSize);
            _activeCount = 0;
            _maxCapacity = int.MaxValue;
        }

        #endregion

        #region 核心方法 Core Methods

        #endregion

        #region 抽象方法实现 Abstract Method Implementation

        /// <inheritdoc />
        protected override bool TryGetFromPool(out StringBuilder obj)
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
        protected override bool TryReturnToPool(StringBuilder obj)
        {
            if (obj == null)
                return false;

            // 清空内容 Clear content
            obj.Clear();

            // 如果容量超过限制，则缩减容量 Reduce capacity if exceeds limit
            if (obj.Capacity > _maxRetainedCapacity)
            {
                obj.Capacity = _maxRetainedCapacity;
            }

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
                var sb = new StringBuilder(_defaultCapacity);
                _pool.Push(sb);
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

        /// <summary>
        /// 获取 StringBuilder
        /// Get StringBuilder
        /// </summary>
        public new StringBuilder Get()
        {
            return ((IObjectPool<StringBuilder>)this).Get();
        }

        /// <summary>
        /// 归还 StringBuilder
        /// Return StringBuilder
        /// </summary>
        public new bool Return(StringBuilder sb)
        {
            return base.Return(sb);
        }

        /// <summary>
        /// 租借 StringBuilder（别名）
        /// Rent StringBuilder (alias)
        /// </summary>
        public new StringBuilder Rent() => Get();

        /// <summary>
        /// 归还 StringBuilder（别名）
        /// Release StringBuilder (alias)
        /// </summary>
        public void Release(StringBuilder sb) => Return(sb);

        #endregion

        #region 静态实例 Static Instance

        private static readonly Lazy<StringBuilderPool> _shared = new Lazy<StringBuilderPool>(() => new StringBuilderPool());

        /// <summary>
        /// 共享实例
        /// Shared instance
        /// </summary>
        public static StringBuilderPool Shared => _shared.Value;

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

        #region 嵌套类

        private class StringBuilderCreationPolicy : IPoolCreationPolicy<StringBuilder>
        {
            private readonly int _defaultCapacity;

            public string Name => "StringBuilderCreationPolicy";
            public string Description => $"Creates StringBuilder with default capacity {_defaultCapacity}";

            public StringBuilderCreationPolicy(int defaultCapacity)
            {
                _defaultCapacity = defaultCapacity;
            }

            public StringBuilder Create()
            {
                return new StringBuilder(_defaultCapacity);
            }

            public bool Validate()
            {
                return _defaultCapacity > 0;
            }
        }

        #endregion
    }

    #region 扩展方法 Extension Methods

    /// <summary>
    /// StringBuilder 池扩展方法
    /// StringBuilder pool extension methods
    /// </summary>
    public static class StringBuilderPoolExtensions
    {
        /// <summary>
        /// 使用池化的 StringBuilder 构建字符串
        /// Build string using pooled StringBuilder
        /// </summary>
        /// <param name="pool">StringBuilder 池 StringBuilder pool</param>
        /// <param name="builder">构建操作 Build action</param>
        /// <returns>构建的字符串 Built string</returns>
        public static string BuildString(this StringBuilderPool pool, Action<StringBuilder> builder)
        {
            var sb = pool.Get();
            try
            {
                builder(sb);
                return sb.ToString();
            }
            finally
            {
                pool.Return(sb);
            }
        }
    }

    #endregion
}
