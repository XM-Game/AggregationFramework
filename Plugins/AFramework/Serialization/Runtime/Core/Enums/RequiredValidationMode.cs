// ==========================================================
// 文件名：RequiredValidationMode.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.Serialization
{
    /// <summary>
    /// 必需验证模式枚举
    /// </summary>
    public enum RequiredValidationMode : byte
    {
        /// <summary>
        /// 普通模式（默认）
        /// <para>仅检查成员是否存在于序列化数据中</para>
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 严格模式
        /// <para>检查成员存在且值不为 null/default</para>
        /// </summary>
        Strict = 1,

        /// <summary>
        /// 宽松模式
        /// <para>仅记录警告，不抛出异常</para>
        /// </summary>
        Lenient = 2,

        /// <summary>
        /// 自定义模式
        /// <para>使用自定义验证逻辑</para>
        /// </summary>
        Custom = 3
    }

    /// <summary>
    /// RequiredValidationMode 扩展方法
    /// </summary>
    public static class RequiredValidationModeExtensions
    {
        /// <summary>
        /// 获取模式的中文描述
        /// </summary>
        public static string GetDescription(this RequiredValidationMode mode)
        {
            return mode switch
            {
                RequiredValidationMode.Normal => "普通模式",
                RequiredValidationMode.Strict => "严格模式",
                RequiredValidationMode.Lenient => "宽松模式",
                RequiredValidationMode.Custom => "自定义模式",
                _ => "未知模式"
            };
        }

        /// <summary>
        /// 检查是否应抛出异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldThrowException(this RequiredValidationMode mode)
        {
            return mode == RequiredValidationMode.Normal ||
                   mode == RequiredValidationMode.Strict;
        }

        /// <summary>
        /// 检查是否需要值验证
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RequiresValueValidation(this RequiredValidationMode mode)
        {
            return mode == RequiredValidationMode.Strict ||
                   mode == RequiredValidationMode.Custom;
        }
    }
}
