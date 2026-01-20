// ==========================================================
// 文件名：PoolDisposedException.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池已销毁异常，当尝试操作已销毁的池时抛出
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池已销毁异常
    /// 当尝试操作已销毁的池时抛出
    /// </summary>
    [Serializable]
    public class PoolDisposedException : PoolException
    {
        /// <summary>
        /// 初始化 <see cref="PoolDisposedException"/> 类的新实例
        /// </summary>
        public PoolDisposedException()
            : base("对象池已被销毁，无法执行操作")
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="PoolDisposedException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public PoolDisposedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和内部异常初始化 <see cref="PoolDisposedException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolDisposedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
