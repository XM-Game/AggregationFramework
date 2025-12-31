// ==========================================================
// 文件名：StringMatchMode.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 字符串匹配模式枚举
    /// <para>定义字符串匹配的方式</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// bool match = text.Matches("hello", StringMatchMode.Contains);
    /// var filtered = items.Where(x => x.Name.Matches(pattern, StringMatchMode.StartsWith));
    /// </code>
    /// </remarks>
    [Serializable]
    public enum StringMatchMode
    {
        /// <summary>精确匹配 (完全相等)</summary>
        Exact = 0,

        /// <summary>包含匹配 (包含子字符串)</summary>
        Contains = 1,

        /// <summary>前缀匹配 (以指定字符串开头)</summary>
        StartsWith = 2,

        /// <summary>后缀匹配 (以指定字符串结尾)</summary>
        EndsWith = 3,

        /// <summary>正则表达式匹配</summary>
        Regex = 4,

        /// <summary>通配符匹配 (* 和 ?)</summary>
        Wildcard = 5,

        /// <summary>模糊匹配 (忽略大小写和空白)</summary>
        Fuzzy = 6
    }

    /// <summary>
    /// StringMatchMode 扩展方法
    /// </summary>
    public static class StringMatchModeExtensions
    {
        /// <summary>
        /// 执行字符串匹配
        /// </summary>
        /// <param name="mode">匹配模式</param>
        /// <param name="source">源字符串</param>
        /// <param name="pattern">匹配模式字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns>如果匹配成功返回 true</returns>
        public static bool Match(this StringMatchMode mode, string source, string pattern, 
            bool ignoreCase = false)
        {
            if (source == null || pattern == null)
                return source == pattern;

            var comparison = ignoreCase 
                ? StringComparison.OrdinalIgnoreCase 
                : StringComparison.Ordinal;

            return mode switch
            {
                StringMatchMode.Exact => string.Equals(source, pattern, comparison),
                StringMatchMode.Contains => source.IndexOf(pattern, comparison) >= 0,
                StringMatchMode.StartsWith => source.StartsWith(pattern, comparison),
                StringMatchMode.EndsWith => source.EndsWith(pattern, comparison),
                StringMatchMode.Regex => MatchRegex(source, pattern, ignoreCase),
                StringMatchMode.Wildcard => MatchWildcard(source, pattern, ignoreCase),
                StringMatchMode.Fuzzy => MatchFuzzy(source, pattern),
                _ => false
            };
        }

        /// <summary>
        /// 正则表达式匹配
        /// </summary>
        private static bool MatchRegex(string source, string pattern, bool ignoreCase)
        {
            try
            {
                var options = ignoreCase 
                    ? System.Text.RegularExpressions.RegexOptions.IgnoreCase 
                    : System.Text.RegularExpressions.RegexOptions.None;
                return System.Text.RegularExpressions.Regex.IsMatch(source, pattern, options);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 通配符匹配 (* 匹配任意字符，? 匹配单个字符)
        /// </summary>
        private static bool MatchWildcard(string source, string pattern, bool ignoreCase)
        {
            if (ignoreCase)
            {
                source = source.ToLowerInvariant();
                pattern = pattern.ToLowerInvariant();
            }

            int sourceIndex = 0;
            int patternIndex = 0;
            int starIndex = -1;
            int matchIndex = 0;

            while (sourceIndex < source.Length)
            {
                if (patternIndex < pattern.Length && 
                    (pattern[patternIndex] == '?' || pattern[patternIndex] == source[sourceIndex]))
                {
                    sourceIndex++;
                    patternIndex++;
                }
                else if (patternIndex < pattern.Length && pattern[patternIndex] == '*')
                {
                    starIndex = patternIndex;
                    matchIndex = sourceIndex;
                    patternIndex++;
                }
                else if (starIndex != -1)
                {
                    patternIndex = starIndex + 1;
                    matchIndex++;
                    sourceIndex = matchIndex;
                }
                else
                {
                    return false;
                }
            }

            while (patternIndex < pattern.Length && pattern[patternIndex] == '*')
            {
                patternIndex++;
            }

            return patternIndex == pattern.Length;
        }

        /// <summary>
        /// 模糊匹配 (忽略大小写和空白)
        /// </summary>
        private static bool MatchFuzzy(string source, string pattern)
        {
            var normalizedSource = NormalizeForFuzzy(source);
            var normalizedPattern = NormalizeForFuzzy(pattern);
            return normalizedSource.Contains(normalizedPattern);
        }

        /// <summary>
        /// 标准化字符串用于模糊匹配
        /// </summary>
        private static string NormalizeForFuzzy(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var result = new System.Text.StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (!char.IsWhiteSpace(c))
                {
                    result.Append(char.ToLowerInvariant(c));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 获取匹配模式的描述
        /// </summary>
        /// <param name="mode">匹配模式</param>
        /// <returns>描述字符串</returns>
        public static string GetDescription(this StringMatchMode mode)
        {
            return mode switch
            {
                StringMatchMode.Exact => "精确匹配",
                StringMatchMode.Contains => "包含",
                StringMatchMode.StartsWith => "开头匹配",
                StringMatchMode.EndsWith => "结尾匹配",
                StringMatchMode.Regex => "正则表达式",
                StringMatchMode.Wildcard => "通配符",
                StringMatchMode.Fuzzy => "模糊匹配",
                _ => "未知"
            };
        }
    }
}
