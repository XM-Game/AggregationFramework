// ==========================================================
// 文件名：BoolExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// bool 扩展方法集合
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// 转换为整数（true = 1, false = 0）
        /// </summary>
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// 转换为字符串（可自定义 true/false 的表示）
        /// </summary>
        public static string ToString(this bool value, string trueString, string falseString)
        {
            return value ? trueString : falseString;
        }

        /// <summary>
        /// 转换为中文字符串
        /// </summary>
        public static string ToChineseString(this bool value)
        {
            return value ? "是" : "否";
        }

        /// <summary>
        /// 转换为 Yes/No 字符串
        /// </summary>
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// 如果为 true 则执行操作
        /// </summary>
        public static bool IfTrue(this bool value, Action action)
        {
            if (value && action != null)
            {
                action();
            }
            return value;
        }

        /// <summary>
        /// 如果为 false 则执行操作
        /// </summary>
        public static bool IfFalse(this bool value, Action action)
        {
            if (!value && action != null)
            {
                action();
            }
            return value;
        }

        /// <summary>
        /// 三元操作符的函数式版本
        /// </summary>
        public static T Then<T>(this bool value, T trueValue, T falseValue)
        {
            return value ? trueValue : falseValue;
        }

        /// <summary>
        /// 三元操作符的延迟计算版本
        /// </summary>
        public static T Then<T>(this bool value, Func<T> trueFunc, Func<T> falseFunc)
        {
            if (trueFunc == null)
                throw new ArgumentNullException(nameof(trueFunc));
            if (falseFunc == null)
                throw new ArgumentNullException(nameof(falseFunc));

            return value ? trueFunc() : falseFunc();
        }

        /// <summary>
        /// 切换布尔值
        /// </summary>
        public static bool Toggle(this ref bool value)
        {
            value = !value;
            return value;
        }
    }
}
