// ==========================================================
// 文件名：Guard.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 参数守卫类
    /// <para>提供参数验证功能，验证失败时抛出异常</para>
    /// </summary>
    public static class Guard
    {
        #region Null 检查

        /// <summary>
        /// 确保参数不为 null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
            return value;
        }

        /// <summary>
        /// 确保参数不为 null（带自定义消息）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T value, string paramName, string message) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName, message);
            return value;
        }

        /// <summary>
        /// 确保可空值类型有值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T? value, string paramName) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(paramName);
            return value.Value;
        }

        #endregion

        #region 字符串检查

        /// <summary>
        /// 确保字符串不为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("字符串不能为 null 或空", paramName);
            return value;
        }

        /// <summary>
        /// 确保字符串不为 null、空或仅包含空白字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("字符串不能为 null、空或仅包含空白字符", paramName);
            return value;
        }

        /// <summary>
        /// 确保字符串长度在指定范围内
        /// </summary>
        public static string LengthInRange(string value, int minLength, int maxLength, string paramName)
        {
            NotNull(value, paramName);
            if (value.Length < minLength || value.Length > maxLength)
                throw new ArgumentException($"字符串长度必须在 {minLength} 到 {maxLength} 之间，实际长度: {value.Length}", paramName);
            return value;
        }

        #endregion

        #region 集合检查

        /// <summary>
        /// 确保集合不为 null 或空
        /// </summary>
        public static T NotNullOrEmpty<T>(T collection, string paramName) where T : class, System.Collections.IEnumerable
        {
            NotNull(collection, paramName);
            var enumerator = collection.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                    throw new ArgumentException("集合不能为空", paramName);
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
            return collection;
        }

        /// <summary>
        /// 确保数组不为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] NotNullOrEmpty<T>(T[] array, string paramName)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("数组不能为 null 或空", paramName);
            return array;
        }

        /// <summary>
        /// 确保列表不为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<T> NotNullOrEmpty<T>(IList<T> list, string paramName)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("列表不能为 null 或空", paramName);
            return list;
        }

        #endregion

        #region 数值范围检查

        /// <summary>
        /// 确保值在指定范围内（包含边界）
        /// </summary>
        public static int InRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须在 {min} 到 {max} 之间");
            return value;
        }

        /// <summary>
        /// 确保值在指定范围内（包含边界）
        /// </summary>
        public static float InRange(float value, float min, float max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须在 {min} 到 {max} 之间");
            return value;
        }

        /// <summary>
        /// 确保值大于指定值
        /// </summary>
        public static int GreaterThan(int value, int threshold, string paramName)
        {
            if (value <= threshold)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须大于 {threshold}");
            return value;
        }

        /// <summary>
        /// 确保值大于等于指定值
        /// </summary>
        public static int GreaterThanOrEqual(int value, int threshold, string paramName)
        {
            if (value < threshold)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须大于等于 {threshold}");
            return value;
        }

        /// <summary>
        /// 确保值小于指定值
        /// </summary>
        public static int LessThan(int value, int threshold, string paramName)
        {
            if (value >= threshold)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须小于 {threshold}");
            return value;
        }

        /// <summary>
        /// 确保值小于等于指定值
        /// </summary>
        public static int LessThanOrEqual(int value, int threshold, string paramName)
        {
            if (value > threshold)
                throw new ArgumentOutOfRangeException(paramName, value, $"值必须小于等于 {threshold}");
            return value;
        }

        /// <summary>
        /// 确保值为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Positive(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName, value, "值必须为正数");
            return value;
        }

        /// <summary>
        /// 确保值为正数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Positive(float value, string paramName)
        {
            if (value <= 0f)
                throw new ArgumentOutOfRangeException(paramName, value, "值必须为正数");
            return value;
        }

        /// <summary>
        /// 确保值为非负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NonNegative(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName, value, "值不能为负数");
            return value;
        }

        /// <summary>
        /// 确保值为非负数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NonNegative(float value, string paramName)
        {
            if (value < 0f)
                throw new ArgumentOutOfRangeException(paramName, value, "值不能为负数");
            return value;
        }

        #endregion

        #region 索引检查

        /// <summary>
        /// 确保索引在有效范围内
        /// </summary>
        public static int ValidIndex(int index, int length, string paramName)
        {
            if (index < 0 || index >= length)
                throw new ArgumentOutOfRangeException(paramName, index, $"索引必须在 0 到 {length - 1} 之间");
            return index;
        }

        /// <summary>
        /// 确保索引和长度在有效范围内
        /// </summary>
        public static void ValidRange(int startIndex, int count, int length, string startParamName, string countParamName)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(startParamName, startIndex, "起始索引不能为负数");
            if (count < 0)
                throw new ArgumentOutOfRangeException(countParamName, count, "数量不能为负数");
            if (startIndex + count > length)
                throw new ArgumentException($"起始索引 ({startIndex}) 加上数量 ({count}) 超出范围 ({length})");
        }

        #endregion

        #region 类型检查

        /// <summary>
        /// 确保值是指定类型
        /// </summary>
        public static T OfType<T>(object value, string paramName) where T : class
        {
            NotNull(value, paramName);
            if (!(value is T result))
                throw new ArgumentException($"参数必须是 {typeof(T).Name} 类型", paramName);
            return result;
        }

        /// <summary>
        /// 确保类型可分配给指定类型
        /// </summary>
        public static Type AssignableTo(Type type, Type targetType, string paramName)
        {
            NotNull(type, paramName);
            NotNull(targetType, nameof(targetType));
            if (!targetType.IsAssignableFrom(type))
                throw new ArgumentException($"类型 {type.Name} 必须可分配给 {targetType.Name}", paramName);
            return type;
        }

        #endregion

        #region 条件检查

        /// <summary>
        /// 确保条件为真
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
                throw new ArgumentException(message);
        }

        /// <summary>
        /// 确保条件为真
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition, string paramName, string message)
        {
            if (!condition)
                throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// 确保条件为假
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
                throw new ArgumentException(message);
        }

        #endregion

        #region 枚举检查

        /// <summary>
        /// 确保枚举值已定义
        /// </summary>
        public static T DefinedEnum<T>(T value, string paramName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"枚举值 {value} 未在 {typeof(T).Name} 中定义", paramName);
            return value;
        }

        #endregion
    }
}
