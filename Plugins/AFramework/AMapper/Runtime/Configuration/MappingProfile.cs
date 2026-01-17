// ==========================================================
// 文件名：MappingProfile.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 映射配置文件基类，用于组织和封装映射配置
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射配置文件基类
    /// <para>用于组织和封装映射配置的基类</para>
    /// <para>Base class for organizing and encapsulating mapping configurations</para>
    /// </summary>
    /// <remarks>
    /// MappingProfile 是组织映射配置的推荐方式：
    /// <list type="bullet">
    /// <item>继承此类并在构造函数中定义映射</item>
    /// <item>支持命名约定配置</item>
    /// <item>支持全局忽略规则</item>
    /// <item>配置作用域隔离</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// public class GameProfile : MappingProfile
    /// {
    ///     public GameProfile()
    ///     {
    ///         CreateMap&lt;Player, PlayerDto&gt;();
    ///         CreateMap&lt;Enemy, EnemyDto&gt;()
    ///             .ForMember(d => d.HealthPercent, opt => opt.MapFrom(s => s.CurrentHealth / s.MaxHealth));
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class MappingProfile
    {
        #region 私有字段 / Private Fields

        private readonly List<Action<MapperConfiguration>> _configurationActions;
        private readonly List<string> _globalIgnores;
        private INamingConvention _sourceMemberNamingConvention;
        private INamingConvention _destinationMemberNamingConvention;
        private bool _allowNullDestinationValues = true;
        private bool _allowNullCollections;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取 Profile 名称
        /// <para>Get the profile name</para>
        /// </summary>
        public string ProfileName => GetType().FullName;

        /// <summary>
        /// 获取源成员命名约定
        /// <para>Get source member naming convention</para>
        /// </summary>
        public INamingConvention SourceMemberNamingConvention => _sourceMemberNamingConvention;

        /// <summary>
        /// 获取目标成员命名约定
        /// <para>Get destination member naming convention</para>
        /// </summary>
        public INamingConvention DestinationMemberNamingConvention => _destinationMemberNamingConvention;

        /// <summary>
        /// 获取全局忽略列表
        /// <para>Get global ignore list</para>
        /// </summary>
        public IReadOnlyList<string> GlobalIgnores => _globalIgnores;

        /// <summary>
        /// 获取是否允许空目标值
        /// <para>Get whether null destination values are allowed</para>
        /// </summary>
        public bool AllowNullDestinationValues => _allowNullDestinationValues;

        /// <summary>
        /// 获取是否允许空集合
        /// <para>Get whether null collections are allowed</para>
        /// </summary>
        public bool AllowNullCollections => _allowNullCollections;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建 MappingProfile 实例
        /// </summary>
        protected MappingProfile()
        {
            _configurationActions = new List<Action<MapperConfiguration>>();
            _globalIgnores = new List<string>();
            _sourceMemberNamingConvention = PascalCaseNamingConvention.Instance;
            _destinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
        }

        #endregion

        #region 配置方法 / Configuration Methods

        /// <summary>
        /// 创建类型映射配置
        /// <para>Create a type mapping configuration</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>类型映射配置表达式 / Type map configuration expression</returns>
        protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            TypeMap typeMap = null;
            var expression = new MappingExpression<TSource, TDestination>(() => typeMap);

            _configurationActions.Add(config =>
            {
                typeMap = config.RegisterTypeMap(typeof(TSource), typeof(TDestination), this);
                expression.ApplyTo(typeMap);
            });

            return expression;
        }

        /// <summary>
        /// 创建类型映射配置（非泛型版本）
        /// <para>Create a type mapping configuration (non-generic version)</para>
        /// </summary>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>类型映射配置表达式 / Type map configuration expression</returns>
        protected IMappingExpression CreateMap(Type sourceType, Type destinationType)
        {
            TypeMap typeMap = null;
            var expression = new MappingExpressionBase(() => typeMap);

            _configurationActions.Add(config =>
            {
                typeMap = config.RegisterTypeMap(sourceType, destinationType, this);
                expression.ApplyTo(typeMap);
            });

            return expression;
        }

        #endregion

        #region 全局配置 / Global Configuration

        /// <summary>
        /// 添加全局忽略规则
        /// <para>Add global ignore rule</para>
        /// </summary>
        /// <param name="propertyNameStartingWith">属性名前缀 / Property name prefix</param>
        protected void AddGlobalIgnore(string propertyNameStartingWith)
        {
            if (!string.IsNullOrEmpty(propertyNameStartingWith))
            {
                _globalIgnores.Add(propertyNameStartingWith);
            }
        }

        /// <summary>
        /// 设置源成员命名约定（泛型版本）
        /// <para>Set source member naming convention (generic version)</para>
        /// </summary>
        /// <typeparam name="T">命名约定类型 / Naming convention type</typeparam>
        protected void SetSourceMemberNamingConvention<T>() where T : INamingConvention, new()
        {
            _sourceMemberNamingConvention = new T();
        }

        /// <summary>
        /// 设置源成员命名约定
        /// <para>Set source member naming convention</para>
        /// </summary>
        /// <param name="convention">命名约定实例 / Naming convention instance</param>
        protected void SetSourceMemberNamingConvention(INamingConvention convention)
        {
            _sourceMemberNamingConvention = convention ?? PascalCaseNamingConvention.Instance;
        }

        /// <summary>
        /// 设置目标成员命名约定（泛型版本）
        /// <para>Set destination member naming convention (generic version)</para>
        /// </summary>
        /// <typeparam name="T">命名约定类型 / Naming convention type</typeparam>
        protected void SetDestinationMemberNamingConvention<T>() where T : INamingConvention, new()
        {
            _destinationMemberNamingConvention = new T();
        }

        /// <summary>
        /// 设置目标成员命名约定
        /// <para>Set destination member naming convention</para>
        /// </summary>
        /// <param name="convention">命名约定实例 / Naming convention instance</param>
        protected void SetDestinationMemberNamingConvention(INamingConvention convention)
        {
            _destinationMemberNamingConvention = convention ?? PascalCaseNamingConvention.Instance;
        }

        /// <summary>
        /// 允许空目标值
        /// <para>Allow null destination values</para>
        /// </summary>
        protected void AllowNullDestinationValuesEnabled()
        {
            _allowNullDestinationValues = true;
        }

        /// <summary>
        /// 禁止空目标值
        /// <para>Disallow null destination values</para>
        /// </summary>
        protected void DisallowNullDestinationValuesEnabled()
        {
            _allowNullDestinationValues = false;
        }

        /// <summary>
        /// 允许空集合
        /// <para>Allow null collections</para>
        /// </summary>
        protected void AllowNullCollectionsEnabled()
        {
            _allowNullCollections = true;
        }

        /// <summary>
        /// 禁止空集合
        /// <para>Disallow null collections</para>
        /// </summary>
        protected void DisallowNullCollectionsEnabled()
        {
            _allowNullCollections = false;
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 应用配置到 MapperConfiguration
        /// </summary>
        internal void Configure(MapperConfiguration configuration)
        {
            foreach (var action in _configurationActions)
            {
                action(configuration);
            }
        }

        #endregion
    }
}
