// ==========================================================
// 文件名：NamingConventions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Text.RegularExpressions
// 功能: 提供常用命名约定的实现
// ==========================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// PascalCase 命名约定
    /// <para>PascalCase naming convention (e.g., CustomerName)</para>
    /// </summary>
    public sealed class PascalCaseNamingConvention : INamingConvention
    {
        /// <summary>
        /// 单例实例
        /// <para>Singleton instance</para>
        /// </summary>
        public static readonly PascalCaseNamingConvention Instance = new PascalCaseNamingConvention();

        /// <inheritdoc/>
        public string SeparatorCharacter => "";

        /// <inheritdoc/>
        public IEnumerable<string> Split(string name)
        {
            if (string.IsNullOrEmpty(name))
                yield break;

            var currentWord = new System.Text.StringBuilder();
            
            foreach (var c in name)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    yield return currentWord.ToString();
                    currentWord.Clear();
                }
                currentWord.Append(c);
            }

            if (currentWord.Length > 0)
                yield return currentWord.ToString();
        }
    }

    /// <summary>
    /// camelCase 命名约定
    /// <para>camelCase naming convention (e.g., customerName)</para>
    /// </summary>
    public sealed class CamelCaseNamingConvention : INamingConvention
    {
        /// <summary>
        /// 单例实例
        /// <para>Singleton instance</para>
        /// </summary>
        public static readonly CamelCaseNamingConvention Instance = new CamelCaseNamingConvention();

        /// <inheritdoc/>
        public string SeparatorCharacter => "";

        /// <inheritdoc/>
        public IEnumerable<string> Split(string name)
        {
            if (string.IsNullOrEmpty(name))
                yield break;

            var currentWord = new System.Text.StringBuilder();
            var isFirst = true;

            foreach (var c in name)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    yield return currentWord.ToString();
                    currentWord.Clear();
                    isFirst = false;
                }
                
                // 首字母转大写以统一比较
                currentWord.Append(isFirst && currentWord.Length == 0 ? char.ToUpperInvariant(c) : c);
                isFirst = false;
            }

            if (currentWord.Length > 0)
                yield return currentWord.ToString();
        }
    }

    /// <summary>
    /// snake_case 命名约定
    /// <para>snake_case naming convention (e.g., customer_name)</para>
    /// </summary>
    public sealed class SnakeCaseNamingConvention : INamingConvention
    {
        /// <summary>
        /// 单例实例
        /// <para>Singleton instance</para>
        /// </summary>
        public static readonly SnakeCaseNamingConvention Instance = new SnakeCaseNamingConvention();

        /// <inheritdoc/>
        public string SeparatorCharacter => "_";

        /// <inheritdoc/>
        public IEnumerable<string> Split(string name)
        {
            if (string.IsNullOrEmpty(name))
                yield break;

            foreach (var part in name.Split('_'))
            {
                if (!string.IsNullOrEmpty(part))
                {
                    // 首字母大写以统一比较
                    yield return char.ToUpperInvariant(part[0]) + part.Substring(1).ToLowerInvariant();
                }
            }
        }
    }

    /// <summary>
    /// 精确匹配命名约定
    /// <para>Exact match naming convention</para>
    /// </summary>
    public sealed class ExactMatchNamingConvention : INamingConvention
    {
        /// <summary>
        /// 单例实例
        /// <para>Singleton instance</para>
        /// </summary>
        public static readonly ExactMatchNamingConvention Instance = new ExactMatchNamingConvention();

        /// <inheritdoc/>
        public string SeparatorCharacter => "";

        /// <inheritdoc/>
        public IEnumerable<string> Split(string name)
        {
            if (!string.IsNullOrEmpty(name))
                yield return name;
        }
    }

    /// <summary>
    /// 小写下划线命名约定
    /// <para>Lower underscore naming convention (e.g., customer_name)</para>
    /// </summary>
    public sealed class LowerUnderscoreNamingConvention : INamingConvention
    {
        /// <summary>
        /// 单例实例
        /// <para>Singleton instance</para>
        /// </summary>
        public static readonly LowerUnderscoreNamingConvention Instance = new LowerUnderscoreNamingConvention();

        /// <inheritdoc/>
        public string SeparatorCharacter => "_";

        /// <inheritdoc/>
        public IEnumerable<string> Split(string name)
        {
            if (string.IsNullOrEmpty(name))
                yield break;

            foreach (var part in name.Split('_'))
            {
                if (!string.IsNullOrEmpty(part))
                {
                    yield return char.ToUpperInvariant(part[0]) + part.Substring(1).ToLowerInvariant();
                }
            }
        }
    }
}
