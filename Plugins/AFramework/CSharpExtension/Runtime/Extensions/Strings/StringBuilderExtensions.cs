// ==========================================================
// 文件名：StringBuilderExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System, System.Text
// ==========================================================

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// StringBuilder 扩展方法
    /// <para>提供 StringBuilder 的常用操作扩展，包括链式调用、条件追加等功能</para>
    /// </summary>
    public static class StringBuilderExtensions
    {
        #region 条件追加

        /// <summary>
        /// 条件追加字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string value)
        {
            if (condition && !string.IsNullOrEmpty(value))
                sb.Append(value);
            return sb;
        }

        /// <summary>
        /// 条件追加字符串（带工厂方法）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, Func<string> valueFactory)
        {
            if (condition && valueFactory != null)
            {
                var value = valueFactory();
                if (!string.IsNullOrEmpty(value))
                    sb.Append(value);
            }
            return sb;
        }

        /// <summary>
        /// 条件追加行
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, string value)
        {
            if (condition && !string.IsNullOrEmpty(value))
                sb.AppendLine(value);
            return sb;
        }

        #endregion

        #region 格式化追加

        /// <summary>
        /// 追加格式化字符串
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendFormat(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args);
            return sb;
        }

        /// <summary>
        /// 追加格式化行
        /// </summary>
        public static StringBuilder AppendLineFormat(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendFormat(format, args);
            sb.AppendLine();
            return sb;
        }

        #endregion

        #region 批量追加

        /// <summary>
        /// 批量追加字符串
        /// </summary>
        public static StringBuilder AppendAll(this StringBuilder sb, params string[] values)
        {
            if (sb == null || values == null)
                return sb;

            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value))
                    sb.Append(value);
            }
            return sb;
        }

        /// <summary>
        /// 批量追加行
        /// </summary>
        public static StringBuilder AppendLines(this StringBuilder sb, params string[] lines)
        {
            if (sb == null || lines == null)
                return sb;

            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb;
        }

        /// <summary>
        /// 追加集合元素（带分隔符）
        /// </summary>
        public static StringBuilder AppendJoin<T>(this StringBuilder sb, string separator, params T[] values)
        {
            if (sb == null || values == null || values.Length == 0)
                return sb;

            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0 && !string.IsNullOrEmpty(separator))
                    sb.Append(separator);
                sb.Append(values[i]);
            }
            return sb;
        }

        #endregion

        #region 清空和重置

        /// <summary>
        /// 清空 StringBuilder
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder Clear(this StringBuilder sb)
        {
            if (sb != null)
                sb.Length = 0;
            return sb;
        }

        /// <summary>
        /// 清空并追加新内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder Reset(this StringBuilder sb, string value)
        {
            if (sb != null)
            {
                sb.Length = 0;
                if (!string.IsNullOrEmpty(value))
                    sb.Append(value);
            }
            return sb;
        }

        #endregion

        #region 移除操作

        /// <summary>
        /// 移除最后 N 个字符
        /// </summary>
        public static StringBuilder RemoveLast(this StringBuilder sb, int count = 1)
        {
            if (sb == null || count <= 0 || sb.Length == 0)
                return sb;

            count = Math.Min(count, sb.Length);
            sb.Length -= count;
            return sb;
        }

        /// <summary>
        /// 移除最后一行
        /// </summary>
        public static StringBuilder RemoveLastLine(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0)
                return sb;

            // 移除最后的换行符
            if (sb.Length >= 2 && sb[sb.Length - 2] == '\r' && sb[sb.Length - 1] == '\n')
                sb.Length -= 2;
            else if (sb.Length >= 1 && (sb[sb.Length - 1] == '\n' || sb[sb.Length - 1] == '\r'))
                sb.Length -= 1;

            return sb;
        }

        #endregion

        #region 检查操作

        /// <summary>
        /// 检查是否为空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this StringBuilder sb)
        {
            return sb == null || sb.Length == 0;
        }

        /// <summary>
        /// 检查是否有内容
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasContent(this StringBuilder sb)
        {
            return sb != null && sb.Length > 0;
        }

        /// <summary>
        /// 检查是否以指定字符串结尾
        /// </summary>
        public static bool EndsWith(this StringBuilder sb, string value)
        {
            if (sb == null || string.IsNullOrEmpty(value) || sb.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (sb[sb.Length - value.Length + i] != value[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否以指定字符串开头
        /// </summary>
        public static bool StartsWith(this StringBuilder sb, string value)
        {
            if (sb == null || string.IsNullOrEmpty(value) || sb.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (sb[i] != value[i])
                    return false;
            }
            return true;
        }

        #endregion

        #region 转换操作

        /// <summary>
        /// 转换为字符串并清空
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringAndClear(this StringBuilder sb)
        {
            if (sb == null)
                return string.Empty;

            var result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        #endregion

        #region 缩进操作

        /// <summary>
        /// 追加带缩进的字符串
        /// </summary>
        public static StringBuilder AppendIndent(this StringBuilder sb, int indentLevel, string value, string indentString = "    ")
        {
            if (sb == null || indentLevel < 0)
                return sb;

            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append(indentString);
            }

            if (!string.IsNullOrEmpty(value))
                sb.Append(value);

            return sb;
        }

        /// <summary>
        /// 追加带缩进的行
        /// </summary>
        public static StringBuilder AppendLineIndent(this StringBuilder sb, int indentLevel, string value, string indentString = "    ")
        {
            sb.AppendIndent(indentLevel, value, indentString);
            sb.AppendLine();
            return sb;
        }

        #endregion
    }
}
