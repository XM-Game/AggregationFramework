// ==========================================================
// 文件名：SpanExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Span 和 Memory 扩展方法集合
    /// 提供高性能的内存操作功能
    /// </summary>
    public static class SpanExtensions
    {
        #region Span<T> 扩展

        /// <summary>
        /// 判断 Span 是否为空
        /// </summary>
        public static bool IsEmpty<T>(this Span<T> span)
        {
            return span.Length == 0;
        }

        /// <summary>
        /// 判断 ReadOnlySpan 是否为空
        /// </summary>
        public static bool IsEmpty<T>(this ReadOnlySpan<T> span)
        {
            return span.Length == 0;
        }

        /// <summary>
        /// 安全获取 Span 元素
        /// </summary>
        public static T GetValueOrDefault<T>(this Span<T> span, int index, T defaultValue = default)
        {
            return index >= 0 && index < span.Length ? span[index] : defaultValue;
        }

        /// <summary>
        /// 安全获取 ReadOnlySpan 元素
        /// </summary>
        public static T GetValueOrDefault<T>(this ReadOnlySpan<T> span, int index, T defaultValue = default)
        {
            return index >= 0 && index < span.Length ? span[index] : defaultValue;
        }

        /// <summary>
        /// 填充 Span
        /// </summary>
        public static void FillWith<T>(this Span<T> span, Func<int, T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            for (int i = 0; i < span.Length; i++)
            {
                span[i] = valueFactory(i);
            }
        }

        /// <summary>
        /// 反转 Span
        /// </summary>
        public static void Reverse<T>(this Span<T> span)
        {
            int left = 0;
            int right = span.Length - 1;

            while (left < right)
            {
                T temp = span[left];
                span[left] = span[right];
                span[right] = temp;
                left++;
                right--;
            }
        }

        /// <summary>
        /// 查找元素索引
        /// </summary>
        public static int IndexOf<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
        {
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i].Equals(value))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 判断是否包含元素
        /// </summary>
        public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
        {
            return IndexOf(span, value) >= 0;
        }

        #endregion

        #region Memory<T> 扩展

        /// <summary>
        /// 判断 Memory 是否为空
        /// </summary>
        public static bool IsEmpty<T>(this Memory<T> memory)
        {
            return memory.Length == 0;
        }

        /// <summary>
        /// 判断 ReadOnlyMemory 是否为空
        /// </summary>
        public static bool IsEmpty<T>(this ReadOnlyMemory<T> memory)
        {
            return memory.Length == 0;
        }

        /// <summary>
        /// 安全获取 Memory 元素
        /// </summary>
        public static T GetValueOrDefault<T>(this Memory<T> memory, int index, T defaultValue = default)
        {
            return index >= 0 && index < memory.Length ? memory.Span[index] : defaultValue;
        }

        /// <summary>
        /// 安全获取 ReadOnlyMemory 元素
        /// </summary>
        public static T GetValueOrDefault<T>(this ReadOnlyMemory<T> memory, int index, T defaultValue = default)
        {
            return index >= 0 && index < memory.Length ? memory.Span[index] : defaultValue;
        }

        #endregion
    }
}
