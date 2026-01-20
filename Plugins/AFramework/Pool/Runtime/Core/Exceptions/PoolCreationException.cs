// ==========================================================
// 文件名：PoolCreationException.cs
// 命名空间: AFramework.Pool
// 依赖: System
// 功能: 定义对象池创建异常，当对象创建失败时抛出
// ==========================================================

using System;

namespace AFramework.Pool
{
    /// <summary>
    /// 对象池创建异常
    /// 当对象创建失败时抛出
    /// </summary>
    [Serializable]
    public class PoolCreationException : PoolException
    {
        /// <summary>
        /// 获取对象类型
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// 初始化 <see cref="PoolCreationException"/> 类的新实例
        /// </summary>
        public PoolCreationException()
            : base("对象创建失败")
        {
        }

        /// <summary>
        /// 使用指定的对象类型初始化 <see cref="PoolCreationException"/> 类的新实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        public PoolCreationException(Type objectType)
            : base($"对象创建失败。类型: {objectType?.FullName ?? "Unknown"}")
        {
            ObjectType = objectType;
        }

        /// <summary>
        /// 使用指定的对象类型和内部异常初始化 <see cref="PoolCreationException"/> 类的新实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolCreationException(Type objectType, Exception innerException)
            : base($"对象创建失败。类型: {objectType?.FullName ?? "Unknown"}", innerException)
        {
            ObjectType = objectType;
        }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="PoolCreationException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public PoolCreationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和内部异常初始化 <see cref="PoolCreationException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public PoolCreationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
