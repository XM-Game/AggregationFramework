// ==========================================================
// 文件名：ProfileMap.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: Profile 运行时表示，存储 Profile 的配置信息
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// Profile 运行时表示
    /// <para>存储 Profile 的配置信息和类型映射</para>
    /// <para>Profile runtime representation storing configuration and type maps</para>
    /// </summary>
    public sealed class ProfileMap
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取 Profile 类型
        /// <para>Get the profile type</para>
        /// </summary>
        public Type ProfileType { get; }

        /// <summary>
        /// 获取 Profile 名称
        /// <para>Get the profile name</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取源成员命名约定
        /// <para>Get source member naming convention</para>
        /// </summary>
        public INamingConvention SourceMemberNamingConvention { get; }

        /// <summary>
        /// 获取目标成员命名约定
        /// <para>Get destination member naming convention</para>
        /// </summary>
        public INamingConvention DestinationMemberNamingConvention { get; }

        /// <summary>
        /// 获取全局忽略列表
        /// <para>Get global ignore list</para>
        /// </summary>
        public IReadOnlyList<string> GlobalIgnores { get; }

        /// <summary>
        /// 获取是否允许空目标值
        /// <para>Get whether null destination values are allowed</para>
        /// </summary>
        public bool AllowNullDestinationValues { get; }

        /// <summary>
        /// 获取是否允许空集合
        /// <para>Get whether null collections are allowed</para>
        /// </summary>
        public bool AllowNullCollections { get; }

        /// <summary>
        /// 获取此 Profile 包含的类型映射
        /// <para>Get type maps contained in this profile</para>
        /// </summary>
        public IReadOnlyList<ITypeMap> TypeMaps { get; private set; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建 ProfileMap 实例
        /// </summary>
        /// <param name="profile">MappingProfile 实例 / MappingProfile instance</param>
        internal ProfileMap(MappingProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            ProfileType = profile.GetType();
            Name = profile.ProfileName;
            SourceMemberNamingConvention = profile.SourceMemberNamingConvention ?? PascalCaseNamingConvention.Instance;
            DestinationMemberNamingConvention = profile.DestinationMemberNamingConvention ?? PascalCaseNamingConvention.Instance;
            GlobalIgnores = profile.GlobalIgnores;
            AllowNullDestinationValues = profile.AllowNullDestinationValues;
            AllowNullCollections = profile.AllowNullCollections;
            TypeMaps = new List<ITypeMap>();
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 设置类型映射列表
        /// </summary>
        internal void SetTypeMaps(IReadOnlyList<ITypeMap> typeMaps)
        {
            TypeMaps = typeMaps ?? new List<ITypeMap>();
        }

        #endregion
    }
}
