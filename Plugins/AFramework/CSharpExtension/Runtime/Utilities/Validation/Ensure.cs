// ==========================================================
// 文件名：Ensure.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 确保条件类
    /// <para>提供状态和条件验证功能，验证失败时抛出 InvalidOperationException</para>
    /// </summary>
    public static class Ensure
    {
        #region 状态检查

        /// <summary>
        /// 确保条件为真，否则抛出 InvalidOperationException
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 确保条件为真，否则抛出 InvalidOperationException（延迟消息生成）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void That(bool condition, Func<string> messageFactory)
        {
            if (!condition)
                throw new InvalidOperationException(messageFactory());
        }

        /// <summary>
        /// 确保条件为假
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Not(bool condition, string message)
        {
            if (condition)
                throw new InvalidOperationException(message);
        }

        #endregion

        #region Null 检查

        /// <summary>
        /// 确保值不为 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T value, string message = "值不能为 null") where T : class
        {
            if (value == null)
                throw new InvalidOperationException(message);
            return value;
        }

        /// <summary>
        /// 确保可空值类型有值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T HasValue<T>(T? value, string message = "值必须有值") where T : struct
        {
            if (!value.HasValue)
                throw new InvalidOperationException(message);
            return value.Value;
        }

        #endregion

        #region 比较检查

        /// <summary>
        /// 确保两个值相等
        /// </summary>
        public static void Equal<T>(T actual, T expected, string message = null) where T : IEquatable<T>
        {
            if (!actual.Equals(expected))
                throw new InvalidOperationException(message ?? $"期望值为 {expected}，实际值为 {actual}");
        }

        /// <summary>
        /// 确保两个值不相等
        /// </summary>
        public static void NotEqual<T>(T actual, T unexpected, string message = null) where T : IEquatable<T>
        {
            if (actual.Equals(unexpected))
                throw new InvalidOperationException(message ?? $"值不应该等于 {unexpected}");
        }

        /// <summary>
        /// 确保值大于指定值
        /// </summary>
        public static void GreaterThan<T>(T value, T threshold, string message = null) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) <= 0)
                throw new InvalidOperationException(message ?? $"值 {value} 必须大于 {threshold}");
        }

        /// <summary>
        /// 确保值小于指定值
        /// </summary>
        public static void LessThan<T>(T value, T threshold, string message = null) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) >= 0)
                throw new InvalidOperationException(message ?? $"值 {value} 必须小于 {threshold}");
        }

        #endregion

        #region 范围检查

        /// <summary>
        /// 确保值在指定范围内
        /// </summary>
        public static void InRange<T>(T value, T min, T max, string message = null) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                throw new InvalidOperationException(message ?? $"值 {value} 必须在 {min} 到 {max} 之间");
        }

        /// <summary>
        /// 确保值为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive(int value, string message = "值必须为正数")
        {
            if (value <= 0)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 确保值为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive(float value, string message = "值必须为正数")
        {
            if (value <= 0f)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 确保值为非负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NonNegative(int value, string message = "值不能为负数")
        {
            if (value < 0)
                throw new InvalidOperationException(message);
        }

        #endregion

        #region 字符串检查

        /// <summary>
        /// 确保字符串不为空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotEmpty(string value, string message = "字符串不能为空")
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException(message);
            return value;
        }

        /// <summary>
        /// 确保字符串不为空白
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotWhiteSpace(string value, string message = "字符串不能为空白")
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(message);
            return value;
        }

        #endregion

        #region 集合检查

        /// <summary>
        /// 确保集合不为空
        /// </summary>
        public static void NotEmpty<T>(System.Collections.Generic.ICollection<T> collection, string message = "集合不能为空")
        {
            if (collection == null || collection.Count == 0)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 确保数组不为空
        /// </summary>
        public static void NotEmpty<T>(T[] array, string message = "数组不能为空")
        {
            if (array == null || array.Length == 0)
                throw new InvalidOperationException(message);
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 确保对象是指定类型
        /// </summary>
        public static T IsType<T>(object obj, string message = null) where T : class
        {
            if (!(obj is T result))
                throw new InvalidOperationException(message ?? $"对象必须是 {typeof(T).Name} 类型");
            return result;
        }

        /// <summary>
        /// 确保对象不是指定类型
        /// </summary>
        public static void IsNotType<T>(object obj, string message = null)
        {
            if (obj is T)
                throw new InvalidOperationException(message ?? $"对象不能是 {typeof(T).Name} 类型");
        }

        #endregion

        #region 操作状态

        /// <summary>
        /// 确保操作成功
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Success(bool success, string message = "操作失败")
        {
            if (!success)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 标记不应到达的代码路径
        /// </summary>
        public static Exception Unreachable(string message = "不应到达此代码路径")
        {
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 标记未实现的功能
        /// </summary>
        public static Exception NotImplemented(string feature = null)
        {
            throw new NotImplementedException(feature ?? "此功能尚未实现");
        }

        /// <summary>
        /// 标记不支持的操作
        /// </summary>
        public static Exception NotSupported(string operation = null)
        {
            throw new NotSupportedException(operation ?? "此操作不受支持");
        }

        #endregion
    }
}
