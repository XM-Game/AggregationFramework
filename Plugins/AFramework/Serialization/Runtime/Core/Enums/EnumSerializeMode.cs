// ==========================================================
// 文件名：EnumSerializeMode.cs
// 命名空间: AFramework.Serialization
// 依赖: 无
// ==========================================================

namespace AFramework.Serialization
{
    /// <summary>
    /// 枚举序列化模式
    /// <para>定义枚举值的序列化方式</para>
    /// </summary>
    public enum EnumSerializeMode : byte
    {
        /// <summary>
        /// 序列化为数值
        /// </summary>
        Value = 0,

        /// <summary>
        /// 序列化为字符串名称
        /// </summary>
        Name = 1,

        /// <summary>
        /// 序列化为字符串（带类型信息）
        /// </summary>
        FullName = 2
    }
}
