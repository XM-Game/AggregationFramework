// ==========================================================
// 文件名：PoolReturnException.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池归还异常，当对象归还失败时抛出
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池归还异常
    /// 当对象归还失败时抛出
    /// </summary>
    [Serializable]
    public class PoolReturnException : PoolException
    {
        /// <summary>
        /// 初始化 <see cref="PoolReturnException"/> 类的新实例
        /// </summary>
        public PoolReturnException()
            : base("对象归还失败")
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="PoolReturnException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public PoolReturnException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和内部异常初始化 <see cref="PoolReturnException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolReturnException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
