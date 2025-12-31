// ==========================================================
// 文件名：CharExtensions.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// char 扩展方法集合
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// 判断是否为数字
        /// </summary>
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }

        /// <summary>
        /// 判断是否为字母
        /// </summary>
        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }

        /// <summary>
        /// 判断是否为字母或数字
        /// </summary>
        public static bool IsLetterOrDigit(this char c)
        {
            return char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// 判断是否为大写字母
        /// </summary>
        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }

        /// <summary>
        /// 判断是否为小写字母
        /// </summary>
        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        /// <summary>
        /// 判断是否为空白字符
        /// </summary>
        public static bool IsWhiteSpace(this char c)
        {
            return char.IsWhiteSpace(c);
        }

        /// <summary>
        /// 转换为大写
        /// </summary>
        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }

        /// <summary>
        /// 转换为小写
        /// </summary>
        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }

        /// <summary>
        /// 重复字符 n 次
        /// </summary>
        public static string Repeat(this char c, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "重复次数不能为负数");

            return new string(c, count);
        }

        /// <summary>
        /// 判断是否为元音字母
        /// </summary>
        public static bool IsVowel(this char c)
        {
            char lower = char.ToLower(c);
            return lower == 'a' || lower == 'e' || lower == 'i' || lower == 'o' || lower == 'u';
        }

        /// <summary>
        /// 判断是否为辅音字母
        /// </summary>
        public static bool IsConsonant(this char c)
        {
            return char.IsLetter(c) && !IsVowel(c);
        }

        /// <summary>
        /// 转换为 ASCII 码
        /// </summary>
        public static int ToAscii(this char c)
        {
            return (int)c;
        }

        /// <summary>
        /// 判断是否为标点符号
        /// </summary>
        public static bool IsPunctuation(this char c)
        {
            return char.IsPunctuation(c);
        }

        /// <summary>
        /// 判断是否为符号
        /// </summary>
        public static bool IsSymbol(this char c)
        {
            return char.IsSymbol(c);
        }
    }
}
