// ==========================================================
// 文件名：UnboundedCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 无界容量策略，不限制池容量
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 无界容量策略
    /// Unbounded Capacity Policy
    /// 
    /// <para>不限制池容量的策略，允许无限扩容</para>
    /// <para>Policy that does not limit pool capacity, allows unlimited expansion</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// - 内存充足环境
    /// - 负载不可预测
    /// - 需要最大灵活性
    /// - 临时对象池
    /// 
    /// 特性：
    /// - 无容量限制
    /// - 允许无限扩容
    /// - 最大灵活性
    /// 
    /// 警告：
    /// - 可能导致内存泄漏
    /// - 内存占用不可控
    /// - 需要配合清理策略使用
    /// - 不适合生产环境
    /// 
    /// 建议：
    /// - 仅用于开发/测试
    /// - 配合监控使用
    /// - 定期清理空闲对象
    /// </remarks>
    public class UnboundedCapacityPolicy : PoolPolicyBase, IPoolCapacityPolicy
    {
        #region Fields

        /// <summary>
        /// 初始容量
        /// Initial capacity
        /// </summary>
        private readonly int _initialCapacity;

        #endregion

        #region Properties

        /// <summary>
        /// 初始容量
        /// Initial capacity
        /// </summary>
        public int InitialCapacity => _initialCapacity;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// Constructor
        /// </summary>
        /// <param name="initialCapacity">初始容量（默认 10）/ Initial capacity (default 10)</param>
        /// <param name="name">策略名称 / Policy name</param>
        public UnboundedCapacityPolicy(int initialCapacity = 10, string name = null)
            : base(name ?? "UnboundedCapacityPolicy")
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be greater than 0.");
            }

            _initialCapacity = initialCapacity;
        }

        #endregion

        #region IPoolCapacityPolicy Implementation

        /// <inheritdoc />
        public int MinCapacity => 0;

        /// <inheritdoc />
        public int MaxCapacity => -1; // -1 表示无限制

        /// <inheritdoc />
        public bool CanCreate(int currentTotal, int currentActive)
        {
            ThrowIfDisposed();

            // 无界容量策略：始终允许创建
            // Unbounded capacity policy: always allow creation
            return true;
        }

        /// <inheritdoc />
        public bool ShouldShrink(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 无界容量策略：不主动收缩（由外部控制）
            // Unbounded capacity policy: no automatic shrinking (controlled externally)
            return false;
        }

        /// <inheritdoc />
        public int CalculateShrinkTarget(int currentAvailable, int currentActive)
        {
            ThrowIfDisposed();

            // 无界容量策略：保持当前可用数量
            // Unbounded capacity policy: keep current available count
            return currentAvailable;
        }

        /// <inheritdoc />
        public bool Validate()
        {
            // 验证初始容量
            // Validate initial capacity
            return _initialCapacity > 0;
        }

        #endregion
    }
}
