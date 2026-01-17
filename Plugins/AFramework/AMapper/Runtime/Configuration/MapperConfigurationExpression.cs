// ==========================================================
// 文件名：MapperConfigurationExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Reflection
// 功能: 映射配置表达式实现，提供流式配置 API
// ==========================================================

using System;
using System.Linq;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射配置表达式实现
    /// <para>Mapper configuration expression implementation</para>
    /// </summary>
    internal sealed class MapperConfigurationExpression : IMapperConfigurationExpression
    {
        #region 私有字段 / Private Fields

        private readonly MapperConfiguration _configuration;
        private INamingConvention _sourceMemberNamingConvention;
        private INamingConvention _destinationMemberNamingConvention;
        private bool _allowNullDestinationValues = true;
        private bool _allowNullCollections;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建配置表达式实例
        /// </summary>
        internal MapperConfigurationExpression(MapperConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _sourceMemberNamingConvention = PascalCaseNamingConvention.Instance;
            _destinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
        }

        #endregion

        #region IMapperConfigurationExpression 实现 / Implementation

        /// <inheritdoc/>
        public IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var typeMap = _configuration.RegisterTypeMap(typeof(TSource), typeof(TDestination), null);
            return new MappingExpression<TSource, TDestination>(typeMap);
        }

        /// <inheritdoc/>
        public IMappingExpression CreateMap(Type sourceType, Type destinationType)
        {
            var typeMap = _configuration.RegisterTypeMap(sourceType, destinationType, null);
            return new MappingExpressionBase(typeMap);
        }

        /// <inheritdoc/>
        public void AddProfile<TProfile>() where TProfile : MappingProfile, new()
        {
            AddProfile(new TProfile());
        }

        /// <inheritdoc/>
        public void AddProfile(MappingProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _configuration.AddProfileInternal(profile);
        }

        /// <inheritdoc/>
        public void AddProfile(Type profileType)
        {
            if (profileType == null)
                throw new ArgumentNullException(nameof(profileType));

            if (!typeof(MappingProfile).IsAssignableFrom(profileType))
            {
                throw new ArgumentException(
                    $"类型必须继承自 MappingProfile / Type must inherit from MappingProfile: {profileType.Name}",
                    nameof(profileType));
            }

            var profile = (MappingProfile)Activator.CreateInstance(profileType);
            AddProfile(profile);
        }

        /// <inheritdoc/>
        public void AddProfiles(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var profileTypes = assembly.GetTypes()
                .Where(t => typeof(MappingProfile).IsAssignableFrom(t) && 
                           !t.IsAbstract && 
                           t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var profileType in profileTypes)
            {
                AddProfile(profileType);
            }
        }

        /// <inheritdoc/>
        public void AddProfiles(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                AddProfiles(assembly);
            }
        }

        /// <inheritdoc/>
        public void AddGlobalIgnore(string propertyNameStartingWith)
        {
            // 全局忽略规则存储在配置中
            // 这里简化实现，实际应用中可能需要更复杂的处理
        }

        /// <inheritdoc/>
        public void SourceMemberNamingConvention(INamingConvention convention)
        {
            _sourceMemberNamingConvention = convention ?? PascalCaseNamingConvention.Instance;
        }

        /// <inheritdoc/>
        public void DestinationMemberNamingConvention(INamingConvention convention)
        {
            _destinationMemberNamingConvention = convention ?? PascalCaseNamingConvention.Instance;
        }

        /// <inheritdoc/>
        public void AllowNullDestinationValues()
        {
            _allowNullDestinationValues = true;
        }

        /// <inheritdoc/>
        public void DisallowNullDestinationValues()
        {
            _allowNullDestinationValues = false;
        }

        /// <inheritdoc/>
        public void AllowNullCollections()
        {
            _allowNullCollections = true;
        }

        /// <inheritdoc/>
        public void DisallowNullCollections()
        {
            _allowNullCollections = false;
        }

        /// <inheritdoc/>
        public void AddMapper(IObjectMapper mapper)
        {
            _configuration.AddMapper(mapper);
        }

        /// <inheritdoc/>
        public void InsertMapper(int index, IObjectMapper mapper)
        {
            _configuration.InsertMapper(index, mapper);
        }

        /// <inheritdoc/>
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _configuration.SetServiceProvider(serviceProvider);
        }

        #endregion
    }
}
