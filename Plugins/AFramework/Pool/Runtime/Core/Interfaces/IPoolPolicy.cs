// ==========================================================
// 文件名：IPoolPolicy.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池策略基础接口，所有池策略的标记接口
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池策略基础接口
    /// 所有池策略的标记接口
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 策略模式（Strategy Pattern）：封装算法族，使其可互换
    /// - 接口隔离原则（ISP）：细粒度接口，按需实现
    /// - 开闭原则（OCP）：新增策略无需修改池核心代码
    /// 
    /// 策略分类：
    /// - 创建策略（IPoolCreationPolicy）：如何创建对象
    /// - 清理策略（IPoolCleanupPolicy）：如何清理对象
    /// - 容量策略（IPoolCapacityPolicy）：如何管理容量
    /// 
    /// 使用场景：
    /// - 不同对象类型需要不同的创建方式
    /// - 不同场景需要不同的清理策略
    /// - 不同性能要求需要不同的容量管理
    /// </remarks>
    public interface IPoolPolicy
    {
        /// <summary>
        /// 获取策略名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取策略描述
        /// </summary>
        string Description { get; }
    }
}
