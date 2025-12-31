// ==========================================================
// 文件名：ComparisonType.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 比较类型枚举
    /// <para>定义数值比较的类型</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// bool result = CompareValue(10, 5, ComparisonType.GreaterThan); // true
    /// bool inRange = CheckRange(value, min, max, ComparisonType.GreaterThanOrEqual);
    /// </code>
    /// </remarks>
    [Serializable]
    public enum ComparisonType
    {
        /// <summary>等于 (==)</summary>
        Equal = 0,

        /// <summary>不等于 (!=)</summary>
        NotEqual = 1,

        /// <summary>小于 (&lt;)</summary>
        LessThan = 2,

        /// <summary>小于或等于 (&lt;=)</summary>
        LessThanOrEqual = 3,

        /// <summary>大于 (&gt;)</summary>
        GreaterThan = 4,

        /// <summary>大于或等于 (&gt;=)</summary>
        GreaterThanOrEqual = 5
    }

    /// <summary>
    /// ComparisonType 扩展方法
    /// </summary>
    public static class ComparisonTypeExtensions
    {
        /// <summary>
        /// 执行比较操作
        /// </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="comparisonType">比较类型</param>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>比较结果</returns>
        public static bool Compare<T>(this ComparisonType comparisonType, T left, T right)
            where T : IComparable<T>
        {
            int comparison = left.CompareTo(right);

            return comparisonType switch
            {
                ComparisonType.Equal => comparison == 0,
                ComparisonType.NotEqual => comparison != 0,
                ComparisonType.LessThan => comparison < 0,
                ComparisonType.LessThanOrEqual => comparison <= 0,
                ComparisonType.GreaterThan => comparison > 0,
                ComparisonType.GreaterThanOrEqual => comparison >= 0,
                _ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
            };
        }

        /// <summary>
        /// 获取比较类型的运算符字符串表示
        /// </summary>
        /// <param name="comparisonType">比较类型</param>
        /// <returns>运算符字符串</returns>
        public static string ToOperatorString(this ComparisonType comparisonType)
        {
            return comparisonType switch
            {
                ComparisonType.Equal => "==",
                ComparisonType.NotEqual => "!=",
                ComparisonType.LessThan => "<",
                ComparisonType.LessThanOrEqual => "<=",
                ComparisonType.GreaterThan => ">",
                ComparisonType.GreaterThanOrEqual => ">=",
                _ => "?"
            };
        }

        /// <summary>
        /// 获取比较类型的反向类型
        /// </summary>
        /// <param name="comparisonType">比较类型</param>
        /// <returns>反向比较类型</returns>
        public static ComparisonType GetInverse(this ComparisonType comparisonType)
        {
            return comparisonType switch
            {
                ComparisonType.Equal => ComparisonType.NotEqual,
                ComparisonType.NotEqual => ComparisonType.Equal,
                ComparisonType.LessThan => ComparisonType.GreaterThanOrEqual,
                ComparisonType.LessThanOrEqual => ComparisonType.GreaterThan,
                ComparisonType.GreaterThan => ComparisonType.LessThanOrEqual,
                ComparisonType.GreaterThanOrEqual => ComparisonType.LessThan,
                _ => comparisonType
            };
        }
    }
}
