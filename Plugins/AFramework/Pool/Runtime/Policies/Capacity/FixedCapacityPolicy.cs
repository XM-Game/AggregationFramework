// ==========================================================
// 文件名：FixedCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 固定容量策略，严格限制池容量
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 固定容量策略
    /// Fixed Capacity Policy
    /// 
    /// <para>严格限制池容量的策略，不允许扩容</para>
    /// <para>Policy that strictly limits pool capacity, no expansion allowed</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 资源受限环境
    /// - 需要严格内存控制
    /// - 防止内存泄漏
    /// - 固定资源池（如连接池）
    /// 
    /// 特性：
    /// - 固定容量，不扩容
    /// - 超出容量拒绝归还
    /// - 内存占用可预测
    /// 
    /// 优势：
    /// - 内存占用固定
    /// - 性能稳定
    /// - 防止资源泄漏
    /// </remarks>
    public class FixedCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

        /// <summary>
        /// 固定容量
        /// Fixed capacity
        /// </summary>
        private readonly int _capacity;

        /// <summary>
        /// 初始容量（预热数量）
        /// Initial capacity (warmup count)
        /// </summary>
        private readonly int _initialCapacity;

        #endregion

        #region Properties

        /// <summary>
        /// 固定容量
        /// Fixed capacity
        /// </summary>
        public int Capacity => _capacity;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="capacity">固定容量 / Fixed capacity</param>
        /// <param name="initialCapacity">初始容量（预热数量，默认为容量的一半）/ Initial capacity (warmup count, default is half of capacity)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public FixedCapacityPolicy(int capacity, int initialCapacity = -1, string name = null)
            : base(name ?? "FixedCapacityPolicy")
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
            }

            _capacity = capacity;
            _initialCapacity = initialCapacity >= 0 ? Math.Min(initialCapacity, capacity) : capacity / 2;
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public int MinCapacity => _capacity;

        /// <inheritdoc />
        public int MaxCapacity => _capacity;

        /// <inheritdoc />
        public int InitialCapacity => _initialCapacity;

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            ThrowIfDisposed();

            // 固定容量策略：只有当前总数小于容量时才允许创建
            // Fixed capacity policy: only allow creation when current total is less than capacity
            return currentTotal < _capacity;
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 固定容量策略：不需要收缩
            // Fixed capacity policy: no shrinking needed
            return false;
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 固定容量策略：保持当前可用数量
            // Fixed capacity policy: keep current available count
            return currentAvailable;
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证容量策略配置
            // Validate capacity policy configuration
            if (_capacity <= 0)
            {
                return false;
            }

            if (_initialCapacity < 0 || _initialCapacity > _capacity)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
