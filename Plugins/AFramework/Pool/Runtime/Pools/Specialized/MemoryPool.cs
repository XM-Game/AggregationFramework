// ==========================================================
// 文件名：MemoryPool.cs
// 命名空间: AFramework.Pool
// 依赖: System, AFramework.Pool
// 功能: Memory<T> 对象池，支持 Span<T> 和零拷贝操作
// ==========================================================

using System;
using System.Buffers;


namespace AFramework.Pool
{
    /// <summary>
    /// Memory 对象池
    /// Memory Object Pool
    /// </summary>
    /// <typeparam name="T">元素类型 Element type</typeparam>
    /// <remarks>
    /// 专为 Memory<T> 设计的对象池，支持：
    /// - Span<T> 支持
    /// - 零拷贝操作
    /// - 高性能内存管理
    /// - ArrayPool 集成
    /// Designed specifically for Memory<T>, supports:
    /// - Span<T> support
    /// - Zero-copy operations
    /// - High-performance memory management
    /// - ArrayPool integration
    /// </remarks>
    public class MemoryPool<T> : ObjectPoolBase<IMemoryOwner<T>>
    {
        #region 字段 Fields

        private readonly int _bufferSize;
        private readonly MemoryPool<T> _memoryPool;
        private int _activeCount;

        #endregion

        #region 属性 Properties

        /// <summary>
        /// 缓冲区大小
        /// Buffer size
        /// </summary>
        public int BufferSize => _bufferSize;

        /// <inheritdoc />
        public override Type ObjectType => typeof(IMemoryOwner<T>);

        /// <inheritdoc />
        public override int TotalCount => _activeCount;

        /// <inheritdoc />
        public override int ActiveCount => _activeCount;

        /// <inheritdoc />
        public override int AvailableCount => 0; // Memory pool 不缓存对象

        #endregion

        #region 构造函数 Constructors

        /// <summary>
        /// 创建 Memory 对象池
        /// Create Memory object pool
        /// </summary>
        /// <param name="bufferSize">缓冲区大小 Buffer size</param>
        public MemoryPool(int bufferSize = 4096)
            : base(new MemoryCreationPolicy<T>(bufferSize), null, null, $"MemoryPool<{typeof(T).Name}>")
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            _bufferSize = bufferSize;
            _memoryPool = (MemoryPool<T>)(object)System.Buffers.MemoryPool<T>.Shared;
            _activeCount = 0;
        }

        #endregion

        #region 抽象方法实现

        /// <inheritdoc />
        protected override bool TryGetFromPool(out IMemoryOwner<T> obj)
        {
            // Memory pool 不缓存对象，始终创建新的
            // Memory pool doesn't cache objects, always create new
            obj = null;
            return false;
        }

        /// <inheritdoc />
        protected override bool TryReturnToPool(IMemoryOwner<T> obj)
        {
            // Memory pool 不缓存对象，直接释放
            // Memory pool doesn't cache objects, release directly
            return false;
        }

        /// <inheritdoc />
        protected override void OnClear()
        {
            // Memory pool 由系统管理，无需手动清理
            // Memory pool is managed by system, no manual cleanup needed
        }

        /// <inheritdoc />
        public override void Warmup(int count)
        {
            // Memory pool 不支持预热
            // Memory pool doesn't support warmup
        }

        /// <inheritdoc />
        public override int Shrink(int targetCount)
        {
            // Memory pool 不支持收缩
            // Memory pool doesn't support shrinking
            return 0;
        }

        #endregion

        #region 核心方法重写

        /// <inheritdoc />
        protected override IMemoryOwner<T> CreateObject()
        {
            var owner = System.Buffers.MemoryPool<T>.Shared.Rent(_bufferSize);
            System.Threading.Interlocked.Increment(ref _activeCount);
            return owner;
        }

        /// <inheritdoc />
        protected override void DestroyObject(IMemoryOwner<T> obj)
        {
            if (obj == null) return;

            obj.Dispose();
            System.Threading.Interlocked.Decrement(ref _activeCount);
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 租借 Memory（别名）
        /// Rent Memory (alias)
        /// </summary>
        public new IMemoryOwner<T> Rent() => ((IObjectPool<IMemoryOwner<T>>)this).Get();

        /// <summary>
        /// 释放 Memory（别名）
        /// Release Memory (alias)
        /// </summary>
        public void Release(IMemoryOwner<T> owner) => Return(owner);

        #endregion

        #region 静态实例 Static Instance

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, MemoryPool<T>> _sharedPools =
            new System.Collections.Concurrent.ConcurrentDictionary<int, MemoryPool<T>>();

        /// <summary>
        /// 获取共享池实例
        /// Get shared pool instance
        /// </summary>
        public static MemoryPool<T> GetShared(int bufferSize = 4096)
        {
            return _sharedPools.GetOrAdd(bufferSize, size => new MemoryPool<T>(size));
        }

        #endregion

        #region 嵌套类

        private class MemoryCreationPolicy<TElement> : IPoolCreationPolicy<IMemoryOwner<TElement>>
        {
            private readonly int _bufferSize;

            public string Name => "MemoryCreationPolicy";
            public string Description => $"Creates IMemoryOwner<{typeof(TElement).Name}> with buffer size {_bufferSize}";

            public MemoryCreationPolicy(int bufferSize)
            {
                _bufferSize = bufferSize;
            }

            public IMemoryOwner<TElement> Create()
            {
                return System.Buffers.MemoryPool<TElement>.Shared.Rent(_bufferSize);
            }

            public bool Validate()
            {
                return _bufferSize > 0;
            }
        }

        #endregion
    }

    #region 扩展方法 Extension Methods

    /// <summary>
    /// Memory 池扩展方法
    /// Memory pool extension methods
    /// </summary>
    public static class MemoryPoolExtensions
    {
        /// <summary>
        /// 使用池化的 Memory 执行操作
        /// Execute operation using pooled Memory
        /// </summary>
        public static void UseMemory<T>(this MemoryPool<T> pool, Action<Memory<T>> action)
        {
            using (var owner = pool.Rent())
            {
                action(owner.Memory);
            }
        }

        /// <summary>
        /// 使用池化的 Memory 执行操作并返回结果
        /// Execute operation using pooled Memory and return result
        /// </summary>
        public static TResult UseMemory<T, TResult>(this MemoryPool<T> pool, Func<Memory<T>, TResult> func)
        {
            using (var owner = pool.Rent())
            {
                return func(owner.Memory);
            }
        }
    }

    #endregion
}
