// ==========================================================
// 文件名：RegistrationException.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义服务注册配置错误时抛出的异常

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 注册异常
    /// <para>当服务注册配置无效时抛出此异常</para>
    /// <para>Exception thrown when service registration configuration is invalid</para>
    /// </summary>
    /// <remarks>
    /// 常见触发场景：
    /// <list type="bullet">
    /// <item>注册的实现类型不是服务类型的子类</item>
    /// <item>注册的类型是抽象类或接口（无法实例化）</item>
    /// <item>重复注册同一服务类型（非预期）</item>
    /// <item>注册配置冲突</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class RegistrationException : DIException
    {
        /// <summary>
        /// 获取相关的服务类型
        /// <para>Get the related service type</para>
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// 获取相关的实现类型
        /// <para>Get the related implementation type</para>
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// 创建注册异常实例
        /// </summary>
        public RegistrationException() : base()
        {
        }

        /// <summary>
        /// 创建带消息的注册异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public RegistrationException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带服务类型的注册异常实例
        /// </summary>
        /// <param name="serviceType">相关的服务类型 / Related service type</param>
        /// <param name="message">异常消息 / Exception message</param>
        public RegistrationException(Type serviceType, string message)
            : base(message)
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// 创建带服务类型和实现类型的注册异常实例
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <param name="implementationType">实现类型 / Implementation type</param>
        /// <param name="message">异常消息 / Exception message</param>
        public RegistrationException(Type serviceType, Type implementationType, string message)
            : base(message)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }

        /// <summary>
        /// 创建带消息和内部异常的注册异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public RegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 创建类型不兼容的注册异常
        /// </summary>
        public static RegistrationException TypeNotAssignable(Type serviceType, Type implementationType)
        {
            var message = $"类型 '{implementationType?.FullName}' 不能赋值给服务类型 '{serviceType?.FullName}'。\n" +
                         $"Type '{implementationType?.FullName}' is not assignable to service type '{serviceType?.FullName}'.";
            return new RegistrationException(serviceType, implementationType, message);
        }

        /// <summary>
        /// 创建类型无法实例化的注册异常
        /// </summary>
        public static RegistrationException TypeNotInstantiable(Type type)
        {
            var message = $"类型 '{type?.FullName}' 无法实例化（可能是抽象类或接口）。\n" +
                         $"Type '{type?.FullName}' cannot be instantiated (may be abstract class or interface).";
            return new RegistrationException(type, message);
        }

        /// <summary>
        /// 创建重复注册的注册异常
        /// </summary>
        public static RegistrationException DuplicateRegistration(Type serviceType, object key = null)
        {
            var keyInfo = key != null ? $" (键值: '{key}')" : "";
            var keyInfoEn = key != null ? $" (key: '{key}')" : "";
            var message = $"服务类型 '{serviceType?.FullName}'{keyInfo} 已经注册。\n" +
                         $"Service type '{serviceType?.FullName}'{keyInfoEn} is already registered.";
            return new RegistrationException(serviceType, message);
        }
    }
}
