// ==========================================================
// 文件名：NamingStrategy.cs
// 命名空间: AFramework.Serialization
// 依赖: System
// ==========================================================

using System;
using System.Text;

namespace AFramework.Serialization
{
    /// <summary>
    /// 命名策略枚举
    /// </summary>
    public enum NamingStrategy : byte
    {
        /// <summary>
        /// 无策略（使用原始名称）
        /// </summary>
        None = 0,

        /// <summary>
        /// 驼峰命名 (camelCase)
        /// </summary>
        CamelCase = 1,

        /// <summary>
        /// 帕斯卡命名 (PascalCase)
        /// </summary>
        PascalCase = 2,

        /// <summary>
        /// 蛇形命名 (snake_case)
        /// </summary>
        SnakeCase = 3,

        /// <summary>
        /// 烤串命名 (kebab-case)
        /// </summary>
        KebabCase = 4,

        /// <summary>
        /// 全大写蛇形 (SCREAMING_SNAKE_CASE)
        /// </summary>
        ScreamingSnakeCase = 5,

        /// <summary>
        /// 全小写
        /// </summary>
        LowerCase = 6,

        /// <summary>
        /// 全大写
        /// </summary>
        UpperCase = 7
    }

    /// <summary>
    /// NamingStrategy 扩展方法
    /// </summary>
    public static class NamingStrategyExtensions
    {
        /// <summary>
        /// 获取策略的中文描述
        /// </summary>
        public static string GetDescription(this NamingStrategy strategy)
        {
            return strategy switch
            {
                NamingStrategy.None => "无策略",
                NamingStrategy.CamelCase => "驼峰命名",
                NamingStrategy.PascalCase => "帕斯卡命名",
                NamingStrategy.SnakeCase => "蛇形命名",
                NamingStrategy.KebabCase => "烤串命名",
                NamingStrategy.ScreamingSnakeCase => "全大写蛇形",
                NamingStrategy.LowerCase => "全小写",
                NamingStrategy.UpperCase => "全大写",
                _ => "未知策略"
            };
        }

        /// <summary>
        /// 转换名称
        /// </summary>
        /// <param name="strategy">命名策略</param>
        /// <param name="name">原始名称</param>
        /// <returns>转换后的名称</returns>
        public static string Transform(this NamingStrategy strategy, string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return strategy switch
            {
                NamingStrategy.None => name,
                NamingStrategy.CamelCase => ToCamelCase(name),
                NamingStrategy.PascalCase => ToPascalCase(name),
                NamingStrategy.SnakeCase => ToSnakeCase(name),
                NamingStrategy.KebabCase => ToSnakeCase(name).Replace('_', '-'),
                NamingStrategy.ScreamingSnakeCase => ToSnakeCase(name).ToUpperInvariant(),
                NamingStrategy.LowerCase => name.ToLowerInvariant(),
                NamingStrategy.UpperCase => name.ToUpperInvariant(),
                _ => name
            };
        }

        #region 私有转换方法

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // 跳过前导下划线
            var start = 0;
            while (start < name.Length && name[start] == '_')
                start++;

            if (start >= name.Length)
                return name;

            var result = new char[name.Length - start];
            var resultIndex = 0;
            var capitalizeNext = false;

            for (var i = start; i < name.Length; i++)
            {
                var c = name[i];
                if (c == '_')
                {
                    capitalizeNext = true;
                    continue;
                }

                result[resultIndex++] = resultIndex == 0
                    ? char.ToLowerInvariant(c)
                    : capitalizeNext ? char.ToUpperInvariant(c) : c;

                capitalizeNext = false;
            }

            return new string(result, 0, resultIndex);
        }

        private static string ToPascalCase(string name)
        {
            var camel = ToCamelCase(name);
            if (string.IsNullOrEmpty(camel))
                return camel;

            return char.ToUpperInvariant(camel[0]) + camel.Substring(1);
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var result = new StringBuilder(name.Length + 5);
            var start = 0;

            // 跳过前导下划线
            while (start < name.Length && name[start] == '_')
                start++;

            for (var i = start; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    if (result.Length > 0)
                        result.Append('_');
                    result.Append(char.ToLowerInvariant(c));
                }
                else if (c != '_')
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
