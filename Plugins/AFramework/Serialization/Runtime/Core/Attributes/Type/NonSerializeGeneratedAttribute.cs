// ==========================================================
// 文件名：NonSerializeGeneratedAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;

namespace AFramework.Serialization
{
    /// <summary>
    /// 禁用序列化生成特性
    /// <para>标记类型不生成序列化代码</para>
    /// <para>等效于 [GenerateMode(GenerateMode.Disabled)]</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// // 禁用序列化生成
    /// [Serializable]
    /// [NonSerializeGenerated]
    /// public class PlaceholderData
    /// {
    ///     // 此类型不会生成序列化代码
    /// }
    /// 
    /// // 带原因说明
    /// [Serializable]
    /// [NonSerializeGenerated("等待后续实现")]
    /// public class FutureFeatureData { }
    /// </code>
    /// 
    /// <para><b>适用场景：</b></para>
    /// <list type="bullet">
    ///   <item>占位符类型，暂不需要序列化</item>
    ///   <item>手动实现序列化的类型</item>
    ///   <item>仅用于编辑器的类型</item>
    ///   <item>测试用临时类型</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class NonSerializeGeneratedAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置禁用原因
        /// </summary>
        public string Reason { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="NonSerializeGeneratedAttribute"/> 的新实例
        /// </summary>
        public NonSerializeGeneratedAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="NonSerializeGeneratedAttribute"/> 的新实例
        /// </summary>
        /// <param name="reason">禁用原因</param>
        public NonSerializeGeneratedAttribute(string reason)
        {
            Reason = reason;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有禁用原因
        /// </summary>
        public bool HasReason() => !string.IsNullOrEmpty(Reason);

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasReason()
                ? $"[NonSerializeGenerated(\"{Reason}\")]"
                : "[NonSerializeGenerated]";
        }

        #endregion
    }
}
