// ==========================================================
// 文件名：OnDeserializingAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 反序列化前回调特性
    /// <para>标记在反序列化开始前调用的方法</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>在对象反序列化之前执行指定方法</item>
    ///   <item>用于初始化默认值、准备接收数据</item>
    ///   <item>支持方法优先级排序</item>
    ///   <item>在构造函数之后、字段赋值之前调用</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     public string Name;
    ///     public int Level;
    ///     public List&lt;Item&gt; Inventory;
    ///     
    ///     // 非序列化字段
    ///     [SerializeIgnore]
    ///     private bool _isLoading;
    ///     
    ///     // 反序列化前初始化集合
    ///     [OnDeserializing]
    ///     private void BeforeLoad()
    ///     {
    ///         _isLoading = true;
    ///         Inventory ??= new List&lt;Item&gt;();
    ///     }
    ///     
    ///     // 带上下文参数
    ///     [OnDeserializing]
    ///     private void PrepareForLoad(DeserializeContext context)
    ///     {
    ///         // 根据数据版本准备兼容处理
    ///         if (context.DataVersion &lt; new Version(2, 0))
    ///         {
    ///             // 旧版本数据兼容处理
    ///         }
    ///     }
    ///     
    ///     // 设置默认值
    ///     [OnDeserializing(Order = -1)]
    ///     private void SetDefaults()
    ///     {
    ///         Level = 1; // 默认等级
    ///         Name = "Unknown";
    ///     }
    /// }
    /// </code>
    /// 
    /// <para><b>方法签名要求：</b></para>
    /// <list type="bullet">
    ///   <item>无参数：<c>void Method()</c></item>
    ///   <item>带上下文：<c>void Method(DeserializeContext context)</c></item>
    ///   <item>可以是私有、保护或公共方法</item>
    ///   <item>不能是静态方法</item>
    /// </list>
    /// 
    /// <para><b>执行时机：</b></para>
    /// <list type="number">
    ///   <item>对象实例已创建（构造函数已执行）</item>
    ///   <item>字段尚未从数据中赋值</item>
    ///   <item>适合初始化集合、设置默认值</item>
    /// </list>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>此时字段值为构造函数设置的值或默认值</item>
    ///   <item>可以安全地初始化集合类型字段</item>
    ///   <item>回调中抛出异常会中断反序列化</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OnDeserializingAttribute : Attribute
    {
        #region 常量

        /// <summary>
        /// 默认执行顺序
        /// </summary>
        public const int DefaultOrder = 0;

        #endregion

        #region 字段

        /// <summary>
        /// 执行顺序
        /// </summary>
        private int _order = DefaultOrder;

        /// <summary>
        /// 是否在异步反序列化时调用
        /// </summary>
        private bool _callOnAsync = true;

        /// <summary>
        /// 是否在流式反序列化时调用
        /// </summary>
        private bool _callOnStreaming = true;

        /// <summary>
        /// 条件表达式
        /// </summary>
        private string _condition;

        /// <summary>
        /// 是否继续执行后续回调
        /// </summary>
        private bool _continueOnError;

        /// <summary>
        /// 是否在版本迁移前调用
        /// </summary>
        private bool _beforeVersionMigration = true;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置执行顺序
        /// <para>数值越小越先执行</para>
        /// <para>默认值：0</para>
        /// </summary>
        public int Order
        {
            get => _order;
            set => _order = value;
        }

        /// <summary>
        /// 获取或设置是否在异步反序列化时调用
        /// <para>默认值：true</para>
        /// </summary>
        public bool CallOnAsync
        {
            get => _callOnAsync;
            set => _callOnAsync = value;
        }

        /// <summary>
        /// 获取或设置是否在流式反序列化时调用
        /// <para>默认值：true</para>
        /// </summary>
        public bool CallOnStreaming
        {
            get => _callOnStreaming;
            set => _callOnStreaming = value;
        }

        /// <summary>
        /// 获取或设置条件表达式
        /// <para>指定一个返回 bool 的属性或字段名</para>
        /// </summary>
        public string Condition
        {
            get => _condition;
            set => _condition = value;
        }

        /// <summary>
        /// 获取或设置是否继续执行后续回调
        /// <para>默认值：false</para>
        /// </summary>
        public bool ContinueOnError
        {
            get => _continueOnError;
            set => _continueOnError = value;
        }

        /// <summary>
        /// 获取或设置是否在版本迁移前调用
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 设置为 false 时，回调在版本迁移之后执行。
        /// </remarks>
        public bool BeforeVersionMigration
        {
            get => _beforeVersionMigration;
            set => _beforeVersionMigration = value;
        }

        /// <summary>
        /// 获取回调阶段
        /// </summary>
        public SerializeCallbackStage Stage => SerializeCallbackStage.BeforeDeserialize;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="OnDeserializingAttribute"/> 的新实例
        /// </summary>
        public OnDeserializingAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="OnDeserializingAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">执行顺序</param>
        public OnDeserializingAttribute(int order)
        {
            _order = order;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有条件
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCondition()
        {
            return !string.IsNullOrEmpty(_condition);
        }

        /// <summary>
        /// 检查是否应该在指定模式下调用
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldCall(bool isAsync, bool isStreaming)
        {
            if (isAsync && !_callOnAsync)
                return false;
            if (isStreaming && !_callOnStreaming)
                return false;
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_order != DefaultOrder)
                return $"[OnDeserializing(Order={_order})]";
            return "[OnDeserializing]";
        }

        #endregion
    }
}
