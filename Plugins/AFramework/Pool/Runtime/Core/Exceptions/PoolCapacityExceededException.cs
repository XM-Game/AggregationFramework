// ==========================================================
// 文件名：PoolCapacityExceededException.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池容量超限异常，当池达到最大容量且无法扩容时抛出
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池容量超限异常
    /// 当池达到最大容量且无法扩容时抛出
    /// </summary>
    [Serializable]
    public class PoolCapacityExceededException : PoolException
    {
        /// <summary>
        /// 获取当前容量
        /// </summary>
        public int CurrentCapacity { get; }

        /// <summary>
        /// 获取最大容量
        /// </summary>
        public int MaxCapacity { get; }

        /// <summary>
        /// 初始化 <see cref="PoolCapacityExceededException"/> 类的新实例
        /// </summary>
        public PoolCapacityExceededException()
            : base("对象池容量已达上限")
        {
        }

        /// <summary>
        /// 使用指定的容量信息初始化 <see cref="PoolCapacityExceededException"/> 类的新实例
        /// </summary>
        /// <param name="currentCapacity">当前容量</param>
        /// <param name="maxCapacity">最大容量</param>
        public PoolCapacityExceededException(int currentCapacity, int maxCapacity)
            : base($"对象池容量已达上限。当前容量: {currentCapacity}, 最大容量: {maxCapacity}")
        {
            CurrentCapacity = currentCapacity;
            MaxCapacity = maxCapacity;
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="PoolCapacityExceededException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public PoolCapacityExceededException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和内部异常初始化 <see cref="PoolCapacityExceededException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolCapacityExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
