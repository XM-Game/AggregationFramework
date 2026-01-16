// ==========================================================
// 文件名：KeyAttribute.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 定义 [Key] 特性，用于键值注入

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 键值特性
    /// <para>指定从容器中解析特定键值的服务实例</para>
    /// <para>Key attribute that specifies resolving a keyed service instance from container</para>
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// <list type="bullet">
    /// <item>同一接口有多个实现，需要区分注入</item>
    /// <item>策略模式中选择特定策略</item>
    /// <item>多租户场景中区分不同租户的服务</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 注册端
    /// builder.Register&lt;IPaymentService, AlipayService&gt;().Keyed("alipay");
    /// builder.Register&lt;IPaymentService, WechatPayService&gt;().Keyed("wechat");
    /// 
    /// // 消费端
    /// public class PaymentManager
    /// {
    ///     [Inject]
    ///     [Key("alipay")]
    ///     private IPaymentService _alipayService;
    ///     
    ///     [Inject]
    ///     [Key("wechat")]
    ///     private IPaymentService _wechatService;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(
        AttributeTargets.Parameter | 
        AttributeTargets.Property | 
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class KeyAttribute : Attribute
    {
        /// <summary>
        /// 获取键值
        /// <para>Get the key value</para>
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 创建键值特性实例
        /// </summary>
        /// <param name="key">键值 / Key value</param>
        /// <exception cref="ArgumentNullException">当 key 为 null 时抛出</exception>
        public KeyAttribute(object key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// 创建键值特性实例（使用字符串键值）
        /// </summary>
        /// <param name="key">字符串键值 / String key value</param>
        public KeyAttribute(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
