// ==========================================================
// 文件名：MapToAttribute.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义映射目标特性，用于指定源成员映射到的目标成员
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射目标特性
    /// <para>用于指定源成员映射到的目标成员</para>
    /// <para>Map to attribute for specifying the destination member for source member mapping</para>
    /// </summary>
    /// <remarks>
    /// 使用此特性可以在源成员上声明映射目标，与 MapFrom 相反。
    /// 适用于从源类型角度配置映射的场景。
    /// 
    /// 使用示例：
    /// <code>
    /// public class Player
    /// {
    ///     // 映射到目标对象的 Name 属性
    ///     [MapTo("Name")]
    ///     public string PlayerName { get; set; }
    ///     
    ///     // 映射到多个目标属性
    ///     [MapTo("DisplayLevel")]
    ///     [MapTo("CurrentLevel")]
    ///     public int Level { get; set; }
    /// }
    /// </code>
    /// 
    /// 注意：MapTo 需要配合 ReverseMap 或显式配置使用。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public sealed class MapToAttribute : Attribute
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取目标成员名称
        /// <para>Get the destination member name</para>
        /// </summary>
        public string DestinationMemberName { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建映射目标特性实例
        /// </summary>
        /// <param name="destinationMemberName">目标成员名称 / Destination member name</param>
        /// <exception cref="ArgumentNullException">当 destinationMemberName 为 null 或空时抛出</exception>
        public MapToAttribute(string destinationMemberName)
        {
            if (string.IsNullOrWhiteSpace(destinationMemberName))
            {
                throw new ArgumentNullException(nameof(destinationMemberName));
            }
            DestinationMemberName = destinationMemberName;
        }

        #endregion
    }
}
