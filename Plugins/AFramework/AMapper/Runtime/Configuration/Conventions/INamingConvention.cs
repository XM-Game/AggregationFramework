// ==========================================================
// 文件名：INamingConvention.cs
// 命名空间: AFramework.AMapper
// 依赖: System.Collections.Generic
// 功能: 定义命名约定接口，用于成员名称匹配
// ==========================================================

using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 命名约定接口
    /// <para>定义成员名称的分词和匹配规则</para>
    /// <para>Naming convention interface for member name tokenization and matching</para>
    /// </summary>
    /// <remarks>
    /// 命名约定用于处理不同命名风格之间的映射，例如：
    /// <list type="bullet">
    /// <item>PascalCase: CustomerName</item>
    /// <item>camelCase: customerName</item>
    /// <item>snake_case: customer_name</item>
    /// </list>
    /// </remarks>
    public interface INamingConvention
    {
        /// <summary>
        /// 获取分隔符正则表达式
        /// <para>Get the separator regular expression</para>
        /// </summary>
        string SeparatorCharacter { get; }

        /// <summary>
        /// 将成员名称分割为词元
        /// <para>Split member name into tokens</para>
        /// </summary>
        /// <param name="name">成员名称 / Member name</param>
        /// <returns>词元列表 / List of tokens</returns>
        IEnumerable<string> Split(string name);
    }
}
