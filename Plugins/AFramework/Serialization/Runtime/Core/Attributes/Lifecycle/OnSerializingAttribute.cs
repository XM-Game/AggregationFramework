// ==========================================================
// 文件名：OnSerializingAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化前回调特性
    /// <para>标记在序列化开始前调用的方法</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>在对象序列化之前执行指定方法</item>
    ///   <item>用于准备序列化数据、验证状态</item>
    ///   <item>支持方法优先级排序</item>
    ///   <item>支持条件执行</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class GameSave
    /// {
    ///     public string PlayerName;
    ///     public int Score;
    ///     public DateTime SaveTime;
    ///     
    ///     // 内部缓存，不序列化
    ///     [SerializeIgnore]
    ///     private string _cachedData;
    ///     
    ///     // 序列化前更新保存时间
    ///     [OnSerializing]
    ///     private void BeforeSave()
    ///     {
    ///         SaveTime = DateTime.UtcNow;
    ///     }
    ///     
    ///     // 带上下文参数的回调
    ///     [OnSerializing]
    ///     private void PrepareData(SerializeContext context)
    ///     {
    ///         // 根据序列化上下文准备数据
    ///         if (context.Format == SerializeFormat.Binary)
    ///         {
    ///             // 二进制格式特殊处理
    ///         }
    ///     }
    ///     
    ///     // 指定优先级（数值越小越先执行）
    ///     [OnSerializing(Order = 1)]
    ///     private void ValidateBeforeSave()
    ///     {
    ///         if (string.IsNullOrEmpty(PlayerName))
    ///             throw new InvalidOperationException("玩家名称不能为空");
    ///     }
    /// }
    /// </code>
    /// 
    /// <para><b>方法签名要求：</b></para>
    /// <list type="bullet">
    ///   <item>无参数：<c>void Method()</c></item>
    ///   <item>带上下文：<c>void Method(SerializeContext context)</c></item>
    ///   <item>可以是私有、保护或公共方法</item>
    ///   <item>不能是静态方法</item>
    /// </list>
    /// 
    /// <para><b>执行顺序：</b></para>
    /// <list type="number">
    ///   <item>按 Order 属性升序排列</item>
    ///   <item>相同 Order 按方法声明顺序</item>
    ///   <item>基类方法先于派生类方法</item>
    /// </list>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>回调方法中抛出异常会中断序列化</item>
    ///   <item>避免在回调中执行耗时操作</item>
    ///   <item>不要在回调中修改对象引用关系</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OnSerializingAttribute : Attribute
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
        /// 是否在异步序列化时调用
        /// </summary>
        private bool _callOnAsync = true;

        /// <summary>
        /// 是否在流式序列化时调用
        /// </summary>
        private bool _callOnStreaming = true;

        /// <summary>
        /// 条件表达式（属性或字段名）
        /// </summary>
        private string _condition;

        /// <summary>
        /// 是否继续执行后续回调（即使当前回调失败）
        /// </summary>
        private bool _continueOnError;

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
        /// 获取或设置是否在异步序列化时调用
        /// <para>默认值：true</para>
        /// </summary>
        public bool CallOnAsync
        {
            get => _callOnAsync;
            set => _callOnAsync = value;
        }

        /// <summary>
        /// 获取或设置是否在流式序列化时调用
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
        /// <para>仅当条件为 true 时执行回调</para>
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
        /// <remarks>
        /// 启用后，即使当前回调抛出异常，也会继续执行后续回调。
        /// 所有异常会被收集并在最后一起抛出。
        /// </remarks>
        public bool ContinueOnError
        {
            get => _continueOnError;
            set => _continueOnError = value;
        }

        /// <summary>
        /// 获取回调阶段
        /// </summary>
        public SerializeCallbackStage Stage => SerializeCallbackStage.BeforeSerialize;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="OnSerializingAttribute"/> 的新实例
        /// </summary>
        public OnSerializingAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="OnSerializingAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">执行顺序</param>
        public OnSerializingAttribute(int order)
        {
            _order = order;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有条件
        /// </summary>
        /// <returns>如果有条件返回 true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCondition()
        {
            return !string.IsNullOrEmpty(_condition);
        }

        /// <summary>
        /// 检查是否应该在指定模式下调用
        /// </summary>
        /// <param name="isAsync">是否异步模式</param>
        /// <param name="isStreaming">是否流式模式</param>
        /// <returns>如果应该调用返回 true</returns>
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
                return $"[OnSerializing(Order={_order})]";
            return "[OnSerializing]";
        }

        #endregion
    }
}
