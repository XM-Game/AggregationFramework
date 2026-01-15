// ==========================================================
// 文件名：OnDeserializedAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 反序列化后回调特性
    /// <para>标记在反序列化完成后调用的方法</para>
    /// </summary>
    /// <remarks>
    /// <para><b>功能说明：</b></para>
    /// <list type="bullet">
    ///   <item>在对象反序列化完成后执行指定方法</item>
    ///   <item>用于数据验证、计算派生值、建立引用关系</item>
    ///   <item>支持方法优先级排序</item>
    ///   <item>所有字段已从数据中恢复</item>
    /// </list>
    /// 
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     public string Name;
    ///     public int Level;
    ///     public int Experience;
    ///     
    ///     // 计算属性，不序列化
    ///     [SerializeIgnore]
    ///     public int ExperienceToNextLevel { get; private set; }
    ///     
    ///     [SerializeIgnore]
    ///     private bool _isInitialized;
    ///     
    ///     // 反序列化后计算派生值
    ///     [OnDeserialized]
    ///     private void AfterLoad()
    ///     {
    ///         ExperienceToNextLevel = CalculateExpToNextLevel(Level);
    ///         _isInitialized = true;
    ///     }
    ///     
    ///     // 数据验证
    ///     [OnDeserialized(Order = 1)]
    ///     private void ValidateData()
    ///     {
    ///         if (Level &lt; 1)
    ///             Level = 1;
    ///         if (Experience &lt; 0)
    ///             Experience = 0;
    ///     }
    ///     
    ///     // 带结果参数
    ///     [OnDeserialized]
    ///     private void LogLoadResult(DeserializeResult result)
    ///     {
    ///         Debug.Log($"加载完成，读取 {result.BytesRead} 字节");
    ///     }
    ///     
    ///     // 建立引用关系
    ///     [OnDeserialized(Order = 10)]
    ///     private void ResolveReferences()
    ///     {
    ///         // 重建运行时引用
    ///         GameManager.Instance.RegisterPlayer(this);
    ///     }
    ///     
    ///     private int CalculateExpToNextLevel(int level)
    ///     {
    ///         return level * 100;
    ///     }
    /// }
    /// </code>
    /// 
    /// <para><b>方法签名要求：</b></para>
    /// <list type="bullet">
    ///   <item>无参数：<c>void Method()</c></item>
    ///   <item>带结果：<c>void Method(DeserializeResult result)</c></item>
    ///   <item>带上下文：<c>void Method(DeserializeContext context)</c></item>
    ///   <item>可以是私有、保护或公共方法</item>
    /// </list>
    /// 
    /// <para><b>执行时机：</b></para>
    /// <list type="number">
    ///   <item>所有字段已从数据中恢复</item>
    ///   <item>版本迁移已完成（如果需要）</item>
    ///   <item>嵌套对象的回调已执行</item>
    /// </list>
    /// 
    /// <para><b>常见用途：</b></para>
    /// <list type="bullet">
    ///   <item>计算派生属性和缓存值</item>
    ///   <item>验证和修正数据</item>
    ///   <item>建立运行时引用关系</item>
    ///   <item>触发加载完成事件</item>
    ///   <item>初始化非序列化字段</item>
    /// </list>
    /// 
    /// <para><b>注意事项：</b></para>
    /// <list type="number">
    ///   <item>此时所有序列化字段已有值</item>
    ///   <item>适合执行数据验证和后处理</item>
    ///   <item>回调中的异常会导致反序列化失败</item>
    ///   <item>避免在回调中执行耗时操作</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class OnDeserializedAttribute : Attribute
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
        /// 是否仅在成功时调用
        /// </summary>
        private bool _onlyOnSuccess = true;

        /// <summary>
        /// 是否继续执行后续回调
        /// </summary>
        private bool _continueOnError;

        /// <summary>
        /// 是否在版本迁移后调用
        /// </summary>
        private bool _afterVersionMigration = true;

        /// <summary>
        /// 是否延迟执行（在所有对象反序列化完成后）
        /// </summary>
        private bool _deferred;

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
        /// 获取或设置是否仅在成功时调用
        /// <para>默认值：true</para>
        /// </summary>
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
        /// 获取或设置是否在版本迁移后调用
        /// <para>默认值：true</para>
        /// </summary>
        public bool AfterVersionMigration
        {
            get => _afterVersionMigration;
            set => _afterVersionMigration = value;
        }

        /// <summary>
        /// 获取或设置是否延迟执行
        /// <para>默认值：false</para>
        /// </summary>
        /// <remarks>
        /// 启用后，回调在整个对象图反序列化完成后执行。
        /// 适用于需要访问其他对象引用的场景。
        /// </remarks>
        public bool Deferred
        {
            get => _deferred;
            set => _deferred = value;
        }

        /// <summary>
        /// 获取回调阶段
        /// </summary>
        public SerializeCallbackStage Stage => _deferred 
            ? SerializeCallbackStage.AfterDeserializeDeferred 
            : SerializeCallbackStage.AfterDeserialize;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="OnDeserializedAttribute"/> 的新实例
        /// </summary>
        public OnDeserializedAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="OnDeserializedAttribute"/> 的新实例
        /// </summary>
        /// <param name="order">执行顺序</param>
        public OnDeserializedAttribute(int order)
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
            var suffix = _deferred ? ", Deferred" : "";
            if (_order != DefaultOrder)
                return $"[OnDeserialized(Order={_order}{suffix})]";
            if (_deferred)
                return "[OnDeserialized(Deferred=true)]";
            return "[OnDeserialized]";
        }

        #endregion
    }

    /// <summary>
    /// 序列化回调阶段枚举
    /// </summary>
    public enum SerializeCallbackStage : byte
    {
        /// <summary>
        /// 序列化之前
        /// </summary>
        BeforeSerialize = 0,

        /// <summary>
        /// 序列化之后
        /// </summary>
        AfterSerialize = 1,

        /// <summary>
        /// 反序列化之前
        /// </summary>
        BeforeDeserialize = 2,

        /// <summary>
        /// 反序列化之后
        /// </summary>
        AfterDeserialize = 3,

        /// <summary>
        /// 反序列化之后（延迟执行）
        /// </summary>
        AfterDeserializeDeferred = 4
    }

    /// <summary>
    /// SerializeCallbackStage 扩展方法
    /// </summary>
    public static class SerializeCallbackStageExtensions
    {
        /// <summary>
        /// 获取阶段的中文描述
        /// </summary>
        public static string GetDescription(this SerializeCallbackStage stage)
        {
            return stage switch
            {
                SerializeCallbackStage.BeforeSerialize => "序列化前",
                SerializeCallbackStage.AfterSerialize => "序列化后",
                SerializeCallbackStage.BeforeDeserialize => "反序列化前",
                SerializeCallbackStage.AfterDeserialize => "反序列化后",
                SerializeCallbackStage.AfterDeserializeDeferred => "反序列化后（延迟）",
                _ => "未知阶段"
            };
        }

        /// <summary>
        /// 检查是否为序列化阶段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSerializeStage(this SerializeCallbackStage stage)
        {
            return stage == SerializeCallbackStage.BeforeSerialize || 
                   stage == SerializeCallbackStage.AfterSerialize;
        }

        /// <summary>
        /// 检查是否为反序列化阶段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDeserializeStage(this SerializeCallbackStage stage)
        {
            return stage == SerializeCallbackStage.BeforeDeserialize || 
                   stage == SerializeCallbackStage.AfterDeserialize ||
                   stage == SerializeCallbackStage.AfterDeserializeDeferred;
        }

        /// <summary>
        /// 检查是否为前置阶段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBeforeStage(this SerializeCallbackStage stage)
        {
            return stage == SerializeCallbackStage.BeforeSerialize || 
                   stage == SerializeCallbackStage.BeforeDeserialize;
        }

        /// <summary>
        /// 检查是否为后置阶段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAfterStage(this SerializeCallbackStage stage)
        {
            return stage == SerializeCallbackStage.AfterSerialize || 
                   stage == SerializeCallbackStage.AfterDeserialize ||
                   stage == SerializeCallbackStage.AfterDeserializeDeferred;
        }
    }
}
