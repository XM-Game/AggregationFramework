// ==========================================================
// 文件名：SortOrder.cs
// 命名空间: AFramework.CSharpExtension
// 依赖: System
// ==========================================================

using System;

namespace AFramework.CSharpExtension
{
    /// <summary>
    /// 排序顺序枚举
    /// <para>定义排序的方向</para>
    /// </summary>
    /// <remarks>
    /// 使用示例:
    /// <code>
    /// var sorted = items.OrderBy(x => x.Name, SortOrder.Ascending);
    /// int multiplier = sortOrder.GetMultiplier(); // 1 或 -1
    /// </code>
    /// </remarks>
    [Serializable]
    public enum SortOrder
    {
        /// <summary>升序 (从小到大，A-Z，0-9)</summary>
        Ascending = 0,

        /// <summary>降序 (从大到小，Z-A，9-0)</summary>
        Descending = 1,

        /// <summary>无排序 (保持原始顺序)</summary>
        None = 2
    }

    /// <summary>
    /// SortOrder 扩展方法
    /// </summary>
    public static class SortOrderExtensions
    {
        /// <summary>
        /// 获取排序乘数 (用于比较器)
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        /// <returns>升序返回 1，降序返回 -1，无排序返回 0</returns>
        public static int GetMultiplier(this SortOrder sortOrder)
        {
            return sortOrder switch
            {
                SortOrder.Ascending => 1,
                SortOrder.Descending => -1,
                SortOrder.None => 0,
                _ => 0
            };
        }

        /// <summary>
        /// 获取反向排序顺序
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        /// <returns>反向排序顺序</returns>
        public static SortOrder GetReverse(this SortOrder sortOrder)
        {
            return sortOrder switch
            {
                SortOrder.Ascending => SortOrder.Descending,
                SortOrder.Descending => SortOrder.Ascending,
                SortOrder.None => SortOrder.None,
                _ => SortOrder.None
            };
        }

        /// <summary>
        /// 检查是否为升序
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        /// <returns>如果是升序返回 true</returns>
        public static bool IsAscending(this SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Ascending;
        }

        /// <summary>
        /// 检查是否为降序
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        /// <returns>如果是降序返回 true</returns>
        public static bool IsDescending(this SortOrder sortOrder)
        {
            return sortOrder == SortOrder.Descending;
        }

        /// <summary>
        /// 应用排序顺序到比较结果
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        /// <param name="comparisonResult">原始比较结果</param>
        /// <returns>应用排序顺序后的比较结果</returns>
        public static int ApplyTo(this SortOrder sortOrder, int comparisonResult)
        {
            return comparisonResult * sortOrder.GetMultiplier();
        }
    }
}
