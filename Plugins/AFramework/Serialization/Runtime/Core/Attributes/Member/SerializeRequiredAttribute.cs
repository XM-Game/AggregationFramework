// ==========================================================
// 文件名：SerializeRequiredAttribute.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 序列化必需特性
    /// <para>标记字段或属性为必需成员，反序列化时缺失会抛出异常</para>
    /// </summary>
    /// <remarks>
    /// <para><b>使用示例：</b></para>
    /// <code>
    /// [Serializable]
    /// public class PlayerData
    /// {
    ///     [SerializeRequired]
    ///     public int Id;
    ///     
    ///     [SerializeRequired(ErrorMessage = "玩家名称不能为空")]
    ///     public string Name;
    ///     
    ///     [SerializeRequired(AllowNull = false)]
    ///     public string Email;
    ///     
    ///     [SerializeRequired(AllowDefault = false)]
    ///     public int Level;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class SerializeRequiredAttribute : Attribute
    {
        #region 属性

        /// <summary>
        /// 获取或设置是否允许空值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowNull { get; set; } = true;

        /// <summary>
        /// 获取或设置是否允许默认值
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowDefault { get; set; } = true;

        /// <summary>
        /// 获取或设置是否允许空字符串
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowEmptyString { get; set; } = true;

        /// <summary>
        /// 获取或设置是否允许空集合
        /// <para>默认值：true</para>
        /// </summary>
        public bool AllowEmptyCollection { get; set; } = true;

        /// <summary>
        /// 获取或设置错误消息
        /// </summary>
        /// <remarks>
        /// 支持占位符：{MemberName}、{TypeName}、{Value}
        /// </remarks>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 获取或设置验证模式
        /// <para>默认值：<see cref="RequiredValidationMode.Normal"/></para>
        /// </summary>
        public RequiredValidationMode ValidationMode { get; set; } = RequiredValidationMode.Normal;

        /// <summary>
        /// 获取或设置错误代码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 获取或设置是否在序列化时也验证
        /// <para>默认值：false</para>
        /// </summary>
        public bool ValidateOnSerialize { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化 <see cref="SerializeRequiredAttribute"/> 的新实例
        /// </summary>
        public SerializeRequiredAttribute() { }

        /// <summary>
        /// 初始化 <see cref="SerializeRequiredAttribute"/> 的新实例
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public SerializeRequiredAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 初始化 <see cref="SerializeRequiredAttribute"/> 的新实例
        /// </summary>
        /// <param name="allowNull">是否允许空值</param>
        /// <param name="allowDefault">是否允许默认值</param>
        public SerializeRequiredAttribute(bool allowNull, bool allowDefault)
        {
            AllowNull = allowNull;
            AllowDefault = allowDefault;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否有自定义错误消息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasErrorMessage() => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// 检查是否有错误代码
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasErrorCode() => !string.IsNullOrEmpty(ErrorCode);

        /// <summary>
        /// 检查是否为严格模式
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsStrict()
        {
            return ValidationMode == RequiredValidationMode.Strict ||
                   (!AllowNull && !AllowDefault);
        }

        /// <summary>
        /// 获取格式化的错误消息
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="value">当前值</param>
        public string GetFormattedErrorMessage(string memberName, string typeName = null, object value = null)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
                return $"必需成员 '{memberName}' 缺失或无效";

            return ErrorMessage
                .Replace("{MemberName}", memberName ?? "Unknown")
                .Replace("{TypeName}", typeName ?? "Unknown")
                .Replace("{Value}", value?.ToString() ?? "null");
        }

        /// <summary>
        /// 验证值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="isValueType">是否为值类型</param>
        public bool Validate(object value, bool isValueType)
        {
            // 检查 null
            if (value == null)
                return AllowNull;

            // 检查默认值
            if (!AllowDefault && isValueType)
            {
                var defaultValue = Activator.CreateInstance(value.GetType());
                if (Equals(value, defaultValue))
                    return false;
            }

            // 检查空字符串
            if (!AllowEmptyString && value is string str && string.IsNullOrEmpty(str))
                return false;

            // 检查空集合
            if (!AllowEmptyCollection && value is ICollection collection && collection.Count == 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return HasErrorMessage()
                ? $"[SerializeRequired(\"{ErrorMessage}\")]"
                : "[SerializeRequired]";
        }

        #endregion
    }
}
