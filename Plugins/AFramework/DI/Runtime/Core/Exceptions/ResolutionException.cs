// ==========================================================
// 文件名：ResolutionException.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义服务解析失败时抛出的异常

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 解析异常
    /// <para>当服务解析失败时抛出此异常</para>
    /// <para>Exception thrown when service resolution fails</para>
    /// </summary>
    /// <remarks>
    /// 常见触发场景：
    /// <list type="bullet">
    /// <item>请求的服务类型未注册</item>
    /// <item>请求的键值服务未注册</item>
    /// <item>依赖的服务无法解析</item>
    /// <item>构造函数参数无法满足</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class ResolutionException : DIException
    {
        /// <summary>
        /// 获取无法解析的服务类型
        /// <para>Get the service type that could not be resolved</para>
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// 获取请求的键值（如果有）
        /// <para>Get the requested key if any</para>
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 创建解析异常实例
        /// </summary>
        public ResolutionException() : base()
        {
        }

        /// <summary>
        /// 创建带消息的解析异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public ResolutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// 创建带服务类型的解析异常实例
        /// </summary>
        /// <param name="serviceType">无法解析的服务类型 / Service type that could not be resolved</param>
        public ResolutionException(Type serviceType)
            : base(FormatMessage(serviceType, null))
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// 创建带服务类型和键值的解析异常实例
        /// </summary>
        /// <param name="serviceType">无法解析的服务类型 / Service type that could not be resolved</param>
        /// <param name="key">请求的键值 / Requested key</param>
        public ResolutionException(Type serviceType, object key)
            : base(FormatMessage(serviceType, key))
        {
            ServiceType = serviceType;
            Key = key;
        }

        /// <summary>
        /// 创建带服务类型和自定义消息的解析异常实例
        /// </summary>
        /// <param name="serviceType">无法解析的服务类型 / Service type that could not be resolved</param>
        /// <param name="message">自定义异常消息 / Custom exception message</param>
        public ResolutionException(Type serviceType, string message)
            : base(message)
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// 创建带消息和内部异常的解析异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public ResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 创建带服务类型和内部异常的解析异常实例
        /// </summary>
        /// <param name="serviceType">无法解析的服务类型 / Service type that could not be resolved</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public ResolutionException(Type serviceType, Exception innerException)
            : base(FormatMessage(serviceType, null), innerException)
        {
            ServiceType = serviceType;
        }

        private static string FormatMessage(Type serviceType, object key)
        {
            if (key != null)
            {
                return $"无法解析服务类型 '{serviceType?.FullName ?? "null"}' (键值: '{key}')。" +
                       $"请确保该服务已在容器中注册。\n" +
                       $"Unable to resolve service type '{serviceType?.FullName ?? "null"}' (key: '{key}'). " +
                       $"Please ensure the service is registered in the container.";
            }
            
            return $"无法解析服务类型 '{serviceType?.FullName ?? "null"}'。" +
                   $"请确保该服务已在容器中注册。\n" +
                   $"Unable to resolve service type '{serviceType?.FullName ?? "null"}'. " +
                   $"Please ensure the service is registered in the container.";
        }

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建服务未注册异常
        /// <para>Create service not registered exception</para>
        /// </summary>
        public static ResolutionException ServiceNotRegistered(Type serviceType)
        {
            return new ResolutionException(serviceType);
        }

        /// <summary>
        /// 创建带键值的服务未注册异常
        /// <para>Create keyed service not registered exception</para>
        /// </summary>
        public static ResolutionException ServiceNotRegistered(Type serviceType, object key)
        {
            return new ResolutionException(serviceType, key);
        }

        /// <summary>
        /// 创建无法实例化抽象类型异常
        /// <para>Create cannot instantiate abstract type exception</para>
        /// </summary>
        public static ResolutionException CannotInstantiateAbstract(Type type)
        {
            return new ResolutionException(type,
                $"无法实例化抽象类型或接口 '{type?.FullName}'。\n" +
                $"Cannot instantiate abstract type or interface '{type?.FullName}'.");
        }

        /// <summary>
        /// 创建没有合适构造函数异常
        /// <para>Create no suitable constructor exception</para>
        /// </summary>
        public static ResolutionException NoSuitableConstructor(Type type)
        {
            return new ResolutionException(type,
                $"类型 '{type?.FullName}' 没有可用的构造函数。\n" +
                $"Type '{type?.FullName}' has no available constructor.");
        }

        /// <summary>
        /// 创建没有实现类型异常
        /// <para>Create no implementation exception</para>
        /// </summary>
        public static ResolutionException NoImplementation(Type serviceType)
        {
            return new ResolutionException(serviceType,
                $"服务类型 '{serviceType?.FullName}' 没有配置实现类型。\n" +
                $"Service type '{serviceType?.FullName}' has no implementation configured.");
        }

        /// <summary>
        /// 创建没有父容器异常
        /// <para>Create no parent container exception</para>
        /// </summary>
        public static ResolutionException NoParentContainer(Type serviceType)
        {
            return new ResolutionException(serviceType,
                $"尝试从父容器解析类型 '{serviceType?.FullName}'，但没有父容器。\n" +
                $"Attempted to resolve type '{serviceType?.FullName}' from parent container, but no parent exists.");
        }

        #endregion
    }
}
