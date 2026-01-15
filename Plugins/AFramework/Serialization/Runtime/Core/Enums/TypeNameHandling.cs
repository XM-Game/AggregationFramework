// ==========================================================
// 文件名：TypeNameHandling.cs
// 命名空间: AFramework.Serialization
// 依赖: 无
// ==========================================================

namespace AFramework.Serialization
{
    /// <summary>
    /// 类型名称处理方式
    /// <para>定义序列化时如何处理类型信息</para>
    /// </summary>
    public enum TypeNameHandling : byte
    {
        /// <summary>
        /// 不包含类型名称
        /// </summary>
        None = 0,

        /// <summary>
        /// 自动（仅多态时包含）
        /// </summary>
        Auto = 1,

        /// <summary>
        /// 始终包含类型名称
        /// </summary>
        Always = 2,

        /// <summary>
        /// 仅对象类型包含
        /// </summary>
        Objects = 3,

        /// <summary>
        /// 仅数组类型包含
        /// </summary>
        Arrays = 4
    }
}
