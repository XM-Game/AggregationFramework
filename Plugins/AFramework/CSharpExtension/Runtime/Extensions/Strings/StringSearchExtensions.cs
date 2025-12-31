// ==========================================================
// 文件名：StringSearchExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串搜索扩展方法集合
    /// </summary>
    public static class StringSearchExtensions
    {
        #region 包含判断

        /// <summary>
        /// 判断是否包含指定字符串（忽略大小写）
        /// </summary>
        public static bool ContainsIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;

            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 判断是否包含任意一个指定字符串
        /// </summary>
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str) || values == null || values.Length == 0)
                return false;

            return values.Any(value => str.Contains(value));
        }

        /// <summary>
        /// 判断是否包含所有指定字符串
        /// </summary>
        public static bool ContainsAll(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str) || values == null || values.Length == 0)
                return false;

            return values.All(value => str.Contains(value));
        }

        #endregion

        #region 开始和结束判断

        /// <summary>
        /// 判断是否以指定字符串开始（忽略大小写）
        /// </summary>
        public static bool StartsWithIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;

            return str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断是否以指定字符串结束（忽略大小写）
        /// </summary>
        public static bool EndsWithIgnoreCase(this string str, string value)
        {
            if (str == null || value == null)
                return false;

            return str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断是否以任意一个指定字符串开始
        /// </summary>
        public static bool StartsWithAny(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str) || values == null || values.Length == 0)
                return false;

            return values.Any(value => str.StartsWith(value));
        }

        /// <summary>
        /// 判断是否以任意一个指定字符串结束
        /// </summary>
        public static bool EndsWithAny(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str) || values == null || values.Length == 0)
                return false;

            return values.Any(value => str.EndsWith(value));
        }

        #endregion

        #region 索引查找

        /// <summary>
        /// 查找所有匹配子串的索引位置
        /// </summary>
        public static IEnumerable<int> IndexOfAll(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                yield break;

            int index = 0;
            while ((index = str.IndexOf(value, index, StringComparison.Ordinal)) != -1)
            {
                yield return index;
                index += value.Length;
            }
        }

        /// <summary>
        /// 统计子串出现次数
        /// </summary>
        public static int CountOccurrences(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                return 0;

            int count = 0;
            int index = 0;

            while ((index = str.IndexOf(value, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += value.Length;
            }

            return count;
        }

        /// <summary>
        /// 统计字符出现次数
        /// </summary>
        public static int CountOccurrences(this string str, char value)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            int count = 0;
            foreach (char c in str)
            {
                if (c == value)
                    count++;
            }

            return count;
        }

        #endregion

        #region 提取操作

        /// <summary>
        /// 提取两个字符串之间的内容
        /// </summary>
        public static string Between(this string str, string start, string end)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return string.Empty;

            int startIndex = str.IndexOf(start, StringComparison.Ordinal);
            if (startIndex == -1)
                return string.Empty;

            startIndex += start.Length;

            int endIndex = str.IndexOf(end, startIndex, StringComparison.Ordinal);
            if (endIndex == -1)
                return string.Empty;

            return str.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 提取指定字符串之前的内容
        /// </summary>
        public static string Before(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                return str;

            int index = str.IndexOf(value, StringComparison.Ordinal);
            return index == -1 ? str : str.Substring(0, index);
        }

        /// <summary>
        /// 提取指定字符串之后的内容
        /// </summary>
        public static string After(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                return str;

            int index = str.IndexOf(value, StringComparison.Ordinal);
            if (index == -1)
                return str;

            return str.Substring(index + value.Length);
        }

        /// <summary>
        /// 提取最后一个指定字符串之前的内容
        /// </summary>
        public static string BeforeLast(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                return str;

            int index = str.LastIndexOf(value, StringComparison.Ordinal);
            return index == -1 ? str : str.Substring(0, index);
        }

        /// <summary>
        /// 提取最后一个指定字符串之后的内容
        /// </summary>
        public static string AfterLast(this string str, string value)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value))
                return str;

            int index = str.LastIndexOf(value, StringComparison.Ordinal);
            if (index == -1)
                return str;

            return str.Substring(index + value.Length);
        }

        #endregion

        #region 相似度比较

        /// <summary>
        /// 计算两个字符串的相似度（Levenshtein 距离）
        /// </summary>
        public static int LevenshteinDistance(this string str, string other)
        {
            if (str == null || other == null)
                return -1;

            if (str.Length == 0)
                return other.Length;

            if (other.Length == 0)
                return str.Length;

            int[,] distance = new int[str.Length + 1, other.Length + 1];

            for (int i = 0; i <= str.Length; i++)
                distance[i, 0] = i;

            for (int j = 0; j <= other.Length; j++)
                distance[0, j] = j;

            for (int i = 1; i <= str.Length; i++)
            {
                for (int j = 1; j <= other.Length; j++)
                {
                    int cost = str[i - 1] == other[j - 1] ? 0 : 1;

                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }

            return distance[str.Length, other.Length];
        }

        #endregion
    }
}
