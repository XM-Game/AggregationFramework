// ==========================================================
// 文件名：StackExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Collections.Generic
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// Stack 扩展方法
    /// <para>提供栈的常用操作扩展，包括批量操作、安全访问等功能</para>
    /// </summary>
    public static class StackExtensions
    {
        #region 空值检查

        /// <summary>
        /// 检查栈是否为 null 或空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this Stack<T> stack)
        {
            return stack == null || stack.Count == 0;
        }

        /// <summary>
        /// 检查栈是否有元素
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElements<T>(this Stack<T> stack)
        {
            return stack != null && stack.Count > 0;
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 批量入栈
        /// </summary>
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            if (stack == null || items == null)
                return;

            foreach (var item in items)
            {
                stack.Push(item);
            }
        }

        /// <summary>
        /// 批量入栈（参数数组）
        /// </summary>
        public static void PushRange<T>(this Stack<T> stack, params T[] items)
        {
            stack.PushRange((IEnumerable<T>)items);
        }

        /// <summary>
        /// 批量出栈
        /// </summary>
        public static List<T> PopRange<T>(this Stack<T> stack, int count)
        {
            var result = new List<T>();
            if (stack == null || count <= 0)
                return result;

            count = Math.Min(count, stack.Count);
            for (int i = 0; i < count; i++)
            {
                result.Add(stack.Pop());
            }
            return result;
        }

        /// <summary>
        /// 出栈所有元素
        /// </summary>
        public static List<T> PopAll<T>(this Stack<T> stack)
        {
            var result = new List<T>();
            if (stack == null)
                return result;

            while (stack.Count > 0)
            {
                result.Add(stack.Pop());
            }
            return result;
        }

        #endregion

        #region 安全操作

        /// <summary>
        /// 尝试出栈
        /// </summary>
        public static bool TryPop<T>(this Stack<T> stack, out T value)
        {
            if (stack.HasElements())
            {
                value = stack.Pop();
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试查看栈顶元素
        /// </summary>
        public static bool TryPeek<T>(this Stack<T> stack, out T value)
        {
            if (stack.HasElements())
            {
                value = stack.Peek();
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 安全出栈（栈为空时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PopOrDefault<T>(this Stack<T> stack, T defaultValue = default)
        {
            return stack.HasElements() ? stack.Pop() : defaultValue;
        }

        /// <summary>
        /// 安全查看栈顶（栈为空时返回默认值）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PeekOrDefault<T>(this Stack<T> stack, T defaultValue = default)
        {
            return stack.HasElements() ? stack.Peek() : defaultValue;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为 List
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ToList<T>(this Stack<T> stack)
        {
            return stack == null ? new List<T>() : new List<T>(stack);
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this Stack<T> stack)
        {
            return stack == null ? Array.Empty<T>() : stack.ToArray();
        }

        #endregion

        #region 聚合操作

        /// <summary>
        /// 对栈元素执行操作（不出栈）
        /// </summary>
        public static void ForEach<T>(this Stack<T> stack, Action<T> action)
        {
            if (stack == null || action == null)
                return;

            foreach (var item in stack)
            {
                action(item);
            }
        }

        #endregion
    }
}
