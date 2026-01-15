// ==========================================================
// 文件名：OnSerializedAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化后回调特性
    /// <para>标记在序列化完成后调用的方法</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>在对象序列化完成后执行指定方法</item>
    ///   <item>用于清理临时数据、记录日志</item>
    ///   <item>支持方法优先级排序</item>
    ///   <item>支持获取序列化结果信息</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class GameSave
    /// {
    ///     public string PlayerName;
    ///     public int Score;
    ///     
    ///     // 临时数据
    ///     [SerializeIgnore]
    ///     private byte[] _tempBuffer;
    ///     
    ///     // 序列化后清理临时数据
    ///     [OnSerialized]
    ///     private void AfterSave()
    ///     {
    ///         _tempBuffer = null;
    ///     }
    ///     
    ///     // 带结果参数的回调
    ///     [OnSerialized]
    ///     private void LogSaveResult(SerializeResult result)
    ///     {
    ///         Debug.Log($"保存完成，数据大小：{result.BytesWritten} 字节");
    ///     }
    ///     
    ///     // 指定优先级
    ///     [OnSerialized(Order = 10)]
    ///     private void NotifySaveComplete()
    ///     {
    ///         // 通知其他系统保存完成
    ///         EventBus.Publish(new SaveCompletedEvent());
    ///     }
    /// }
    /// </code>
    /// 
    /// <para><b>方法签名要求：</b></para>
    /// <list type="bullet">
    ///   <item>无参数：<c>void Method()</c></item>
    ///   <item>带结果：<c>void Method(SerializeResult result)</c></item>
    ///   <item>带上下文：<c>void Method(SerializeContext context)</c></item>
    ///   <item>可以是私有、保护或公共方法</item>
    /// </list>
    /// 
    /// <para><b>执行时机：</b></para>
    /// <list type="bullet">
    ///   <item>所有字段序列化完成后</item>
    ///   <item>数据已写入缓冲区/流</item>
    ///   <item>压缩/加密处理完成后（如果启用）</item>
    /// </list>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>回调中的异常不会影响已序列化的数据</item>
    ///   <item>适合执行清理和通知操作</item>
    ///   <item>不要在回调中修改对象状态（可能导致不一致）</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OnSerializedAttribute : Attribute
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
        /// 条件表达式
        /// </summary>
        private string _condition;

        /// <summary>
        /// 是否仅在成功时调用
        /// </summary>
        private bool _onlyOnSuccess = true;

        /// <summary>
        /// 是否继续执行后续回调
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
        /// </summary>
        public string Condition
        {
            get => _condition;
            set => _condition = value;
        }

        /// <summary>
        /// 获取或设置是否仅在成功时调用
        /// <para>默认值：true</para>
        /// </summary>
        /// <remarks>
        /// 设置为 false 时，即使序列化失败也会调用回调。
        /// </remarks>
        public bool OnlyOnSuccess
        {
            get => _onlyOnSuccess;
            set => _onlyOnSuccess = value;
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
        /// 获取回调阶段
        /// </summary>
        public SerializeCallbackStage Stage => SerializeCallbackStage.AfterSerialize;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="OnSerializedAttribute"/> 的新实例
        /// </summary>
        public OnSerializedAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="OnSerializedAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">执行顺序</param>
        public OnSerializedAttribute(int order)
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
        public bool ShouldCall(bool isAsync, bool isStreaming, bool isSuccess)
        {
            if (_onlyOnSuccess && !isSuccess)
                return false;
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
                return $"[OnSerialized(Order={_order})]";
            return "[OnSerialized]";
        }

        #endregion
    }
}
