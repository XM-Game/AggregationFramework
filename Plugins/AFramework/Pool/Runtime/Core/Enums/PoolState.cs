// ==========================================================
// 文件名：PoolState.cs
// 命名空间: AFramework.Pool
// 依赖: 无
// 功能: 定义对象池状态枚举
// ==========================================================

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池状态枚举
    /// </summary>
    public enum PoolState
    {
        /// <summary>
        /// 未初始化状态
        /// 池已创建但尚未进行任何操作
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// 活跃状态（运行中）
        /// 池正常工作，可以进行获取和归还操作
        /// </summary>
        Active = 1,

        /// <summary>
        /// 运行中状态（与Active相同，保持兼容性）
        /// 池正常工作，可以进行获取和归还操作
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已销毁状态
        /// 池已调用 Dispose()，不再可用
        /// </summary>
        Disposed = 2,
    }
}
