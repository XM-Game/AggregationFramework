// ==========================================================
// 文件名：BoolExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// bool 扩展方法
    /// <para>提供布尔值的常用操作扩展，包括转换、条件执行等功能</para>
    /// </summary>
    public static class BoolExtensions
    {
        #region 转换操作

        /// <summary>
        /// 转换为整数（true = 1, false = 0）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// 转换为字符串（可自定义 true/false 的表示）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this bool value, string trueValue, string falseValue)
        {
            return value ? trueValue : falseValue;
        }

        /// <summary>
        /// 转换为中文字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToChineseString(this bool value)
        {
            return value ? "是" : "否";
        }

        /// <summary>
        /// 转换为 Yes/No 字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// 转换为 On/Off 字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToOnOff(this bool value)
        {
            return value ? "On" : "Off";
        }

        /// <summary>
        /// 转换为 Enabled/Disabled 字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToEnabledDisabled(this bool value)
        {
            return value ? "Enabled" : "Disabled";
        }

        #endregion

        #region 条件执行

        /// <summary>
        /// 当值为 true 时执行操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IfTrue(this bool value, Action action)
        {
            if (value && action != null)
                action();
            return value;
        }

        /// <summary>
        /// 当值为 false 时执行操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IfFalse(this bool value, Action action)
        {
            if (!value && action != null)
                action();
            return value;
        }

        /// <summary>
        /// 根据布尔值执行不同操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Then(this bool value, Action trueAction, Action falseAction = null)
        {
            if (value)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();
            return value;
        }

        /// <summary>
        /// 根据布尔值返回不同结果
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Select<T>(this bool value, T trueValue, T falseValue)
        {
            return value ? trueValue : falseValue;
        }

        /// <summary>
        /// 根据布尔值返回不同结果（使用工厂方法）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Select<T>(this bool value, Func<T> trueFactory, Func<T> falseFactory)
        {
            return value ? trueFactory() : falseFactory();
        }

        #endregion

        #region 逻辑操作

        /// <summary>
        /// 取反
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Not(this bool value)
        {
            return !value;
        }

        /// <summary>
        /// 与操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool And(this bool value, bool other)
        {
            return value && other;
        }

        /// <summary>
        /// 或操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Or(this bool value, bool other)
        {
            return value || other;
        }

        /// <summary>
        /// 异或操作
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Xor(this bool value, bool other)
        {
            return value ^ other;
        }

        /// <summary>
        /// 同或操作（相等）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Xnor(this bool value, bool other)
        {
            return value == other;
        }

        #endregion

        #region 可空布尔操作

        /// <summary>
        /// 获取可空布尔值，null 时返回默认值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValueOrDefault(this bool? value, bool defaultValue = false)
        {
            return value ?? defaultValue;
        }

        /// <summary>
        /// 检查可空布尔值是否为 true
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(this bool? value)
        {
            return value == true;
        }

        /// <summary>
        /// 检查可空布尔值是否为 false
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(this bool? value)
        {
            return value == false;
        }

        /// <summary>
        /// 检查可空布尔值是否为 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this bool? value)
        {
            return value == null;
        }

        #endregion
    }
}
