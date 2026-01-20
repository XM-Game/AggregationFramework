// ==========================================================
// 文件名：IPoolCapacityPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池容量策略接口，定义池的容量管理规则
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池容量策略接口
    /// 定义池的容量管理规则
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 策略模式：封装容量管理算法
    /// - 单一职责原则：仅负责容量控制
    /// - 开闭原则：新增容量策略无需修改池代码
    /// 
    /// 内置策略：
    /// - FixedCapacityPolicy：固定容量，超出时抛异常或阻塞
    /// - DynamicCapacityPolicy：动态扩容，按倍数增长（1.5x/2x）
    /// - UnboundedCapacityPolicy：无界容量，无限制增长
    /// - ThresholdCapacityPolicy：阈值容量，达到阈值时触发清理
    /// 
    /// 容量模式：
    /// - 固定模式：适合内存敏感场景，严格控制内存占用
    /// - 动态模式：适合负载波动场景，自动适应需求
    /// - 无界模式：适合开发调试，不限制容量
    /// - 阈值模式：适合长时间运行场景，自动清理空闲对象
    /// </remarks>
    public interface IPoolCapacityPolicy : IPoolPolicy
    {
        /// <summary>
        /// 获取池的最小容量
        /// </summary>
        /// <remarks>
        /// 池初始化时会预热到此容量
        /// 收缩时不会低于此容量
        /// </remarks>
        int MinCapacity { get; }

        /// <summary>
        /// 获取池的最大容量
        /// </summary>
        /// <remarks>
        /// 池不会超过此容量
        /// -1 表示无限制
        /// </remarks>
        int MaxCapacity { get; }

        /// <summary>
        /// 获取池的初始容量
        /// </summary>
        /// <remarks>
        /// 池创建时的初始大小
        /// 通常等于 MinCapacity
        /// </remarks>
        int InitialCapacity { get; }

        /// <summary>
        /// 获取初始容量（用于预热）
        /// </summary>
        /// <returns>初始容量值</returns>
        int GetInitialCapacity()
        {
            return InitialCapacity;
        }

        /// <summary>
        /// 判断是否可以创建新对象
        /// </summary>
        /// <param name="currentTotal">当前池中对象总数（可用 + 活跃）</param>
        /// <param name="currentActive">当前活跃对象数量</param>
        /// <returns>如果可以创建返回 true，否则返回 false</returns>
        /// <remarks>
        /// 在 Get() 操作时，如果池中无可用对象，会调用此方法判断是否可以创建新对象
        /// 
        /// 实现示例：
        /// - 固定容量：currentTotal &lt; MaxCapacity
        /// - 动态容量：currentTotal &lt; MaxCapacity || MaxCapacity == -1
        /// - 无界容量：始终返回 true
        /// </remarks>
        bool CanCreate(int currentTotal, int currentActive);

        /// <summary>
        /// 判断是否可以归还对象
        /// </summary>
        /// <param name="currentAvailable">当前可用对象数量</param>
        /// <param name="maxCapacity">最大容量</param>
        /// <returns>如果可以归还返回 true，否则返回 false</returns>
        bool CanReturn(int currentAvailable, int maxCapacity)
        {
            return maxCapacity < 0 || currentAvailable < maxCapacity;
        }

        /// <summary>
        /// 判断是否需要收缩池
        /// </summary>
        /// <param name="currentAvailable">当前可用对象数量</param>
        /// <param name="currentActive">当前活跃对象数量</param>
        /// <returns>如果需要收缩返回 true，否则返回 false</returns>
        /// <remarks>
        /// 在 Return() 操作后，会调用此方法判断是否需要自动收缩
        /// 
        /// 实现示例：
        /// - 固定容量：始终返回 false（不自动收缩）
        /// - 动态容量：currentAvailable > MaxCapacity * 0.5
        /// - 阈值容量：currentAvailable > Threshold
        /// </remarks>
        bool ShouldShrink(int currentAvailable, int currentActive);

        /// <summary>
        /// 计算收缩目标数量
        /// </summary>
        /// <param name="currentAvailable">当前可用对象数量</param>
        /// <param name="currentActive">当前活跃对象数量</param>
        /// <returns>收缩后应保留的可用对象数量</returns>
        /// <remarks>
        /// 在执行收缩操作时，会调用此方法计算目标数量
        /// 
        /// 实现示例：
        /// - 固定容量：MinCapacity
        /// - 动态容量：Math.Max(MinCapacity, currentActive)
        /// - 阈值容量：Threshold
        /// </remarks>
        int CalculateShrinkTarget(int currentAvailable, int currentActive);

        /// <summary>
        /// 验证容量策略是否有效
        /// </summary>
        /// <returns>如果策略有效返回 true，否则返回 false</returns>
        /// <remarks>
        /// 用于在池初始化时验证策略配置
        /// 
        /// 验证规则：
        /// - MinCapacity >= 0
        /// - MaxCapacity >= MinCapacity 或 MaxCapacity == -1
        /// - InitialCapacity >= MinCapacity
        /// - InitialCapacity &lt;= MaxCapacity 或 MaxCapacity == -1
        /// </remarks>
        bool Validate();
    }
}
