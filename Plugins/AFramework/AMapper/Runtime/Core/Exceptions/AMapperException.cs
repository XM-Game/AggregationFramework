// ==========================================================
// 文件名：AMapperException.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义 AMapper 框架的基础异常类
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// AMapper 基础异常类
    /// <para>所有 AMapper 特定异常的基类</para>
    /// <para>Base exception class for all AMapper specific exceptions</para>
    /// </summary>
    /// <remarks>
    /// 此类作为 AMapper 框架所有异常的基类，便于统一捕获和处理。
    /// 
    /// 异常层次结构：
    /// <list type="bullet">
    /// <item>AMapperException（基类）</item>
    /// <item>├── MappingException（映射执行异常）</item>
    /// <item>├── ConfigurationException（配置验证异常）</item>
    /// <item>└── DuplicateTypeMapException（重复配置异常）</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class AMapperException : Exception
    {
        #region 构造函数 / Constructors

        /// <summary>
        /// 创建 AMapper 异常实例
        /// </summary>
        public AMapperException()
        {
        }

        /// <summary>
        /// 创建带消息的 AMapper 异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public AMapperException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带消息和内部异常的 AMapper 异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public AMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}
