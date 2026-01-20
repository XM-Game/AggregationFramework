// ==========================================================
// 文件名：PoolException.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池异常基类，所有池相关异常的基类
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池异常基类
    /// 所有池相关异常的基类
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// - 异常层次结构：提供清晰的异常分类
    /// - 信息完整性：包含足够的上下文信息
    /// - 可序列化：支持跨域传递
    /// 
    /// 异常分类：
    /// - PoolCapacityExceededException：容量超限
    /// - PoolDisposedException：池已销毁
    /// - PoolCreationException：对象创建失败
    /// - PoolReturnException：对象归还失败
    /// </remarks>
    [Serializable]
    public class PoolException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="PoolException"/> 类的新实例
        /// </summary>
        public PoolException()
            : base("对象池操作失败")
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="PoolException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public PoolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和内部异常初始化 <see cref="PoolException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
