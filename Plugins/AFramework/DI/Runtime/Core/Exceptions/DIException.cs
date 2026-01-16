// ==========================================================
// 文件名：DIException.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 DI 框架的基础异常类

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// DI 框架基础异常
    /// <para>所有 AFramework.DI 相关异常的基类</para>
    /// <para>Base exception class for all AFramework.DI related exceptions</para>
    /// </summary>
    [Serializable]
    public class DIException : Exception
    {
        /// <summary>
        /// 创建 DI 异常实例
        /// </summary>
        public DIException() : base()
        {
        }

        /// <summary>
        /// 创建带消息的 DI 异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public DIException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带消息和内部异常的 DI 异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public DIException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
