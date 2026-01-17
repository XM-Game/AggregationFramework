// ==========================================================
// 文件名：MapperConfiguration.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Linq.Expressions
// 功能: 映射配置容器，管理所有类型映射配置
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射配置容器
    /// <para>AMapper 的核心配置类，管理所有类型映射配置</para>
    /// <para>Mapper configuration container that manages all type mapping configurations</para>
    /// </summary>
    /// <remarks>
    /// MapperConfiguration 是 AMapper 的入口点，负责：
    /// <list type="bullet">
    /// <item>存储所有类型映射配置</item>
    /// <item>管理 Profile 实例</item>
    /// <item>编译执行计划</item>
    /// <item>验证配置有效性</item>
    /// <item>创建 Mapper 实例</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// var config = new MapperConfiguration(cfg =>
    /// {
    ///     cfg.AddProfile&lt;GameProfile&gt;();
    ///     cfg.CreateMap&lt;Player, PlayerDto&gt;();
    /// });
    /// 
    /// var mapper = config.CreateMapper();
    /// </code>
    /// </remarks>
    public sealed class MapperConfiguration : IMapperConfiguration
    {
        #region 私有字段 / Private Fields

        private readonly Dictionary<TypePair, TypeMap> _typeMaps;
        private readonly List<MappingProfile> _profiles;
        private readonly List<IObjectMapper> _mappers;
        private readonly object _syncLock = new object();
        private readonly Dictionary<TypePair, Delegate> _mapFuncCache;
        private ExecutionPlanBuilder _executionPlanBuilder;
        private bool _isSealed;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取服务提供者
        /// <para>Get the service provider</para>
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// 获取内置对象映射器列表
        /// <para>Get the list of built-in object mappers</para>
        /// </summary>
        public IReadOnlyList<IObjectMapper> Mappers => _mappers;

        /// <summary>
        /// 获取配置是否已密封
        /// <para>Get whether the configuration is sealed</para>
        /// </summary>
        public bool IsSealed => _isSealed;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射配置实例
        /// </summary>
        /// <param name="configure">配置委托 / Configuration delegate</param>
        /// <exception cref="ArgumentNullException">当 configure 为 null 时抛出</exception>
        public MapperConfiguration(Action<IMapperConfigurationExpression> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            _typeMaps = new Dictionary<TypePair, TypeMap>();
            _profiles = new List<MappingProfile>();
            _mappers = new List<IObjectMapper>();
            _mapFuncCache = new Dictionary<TypePair, Delegate>();

            // 创建配置表达式并执行配置
            var expression = new MapperConfigurationExpression(this);
            configure(expression);

            // 密封配置
            Seal();
        }

        /// <summary>
        /// 创建映射配置实例（使用 Profile 数组）
        /// </summary>
        /// <param name="profiles">Profile 数组 / Array of profiles</param>
        public MapperConfiguration(params MappingProfile[] profiles)
            : this(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            })
        {
        }

        #endregion

        #region IMapperConfiguration 实现 / IMapperConfiguration Implementation

        /// <summary>
        /// 创建映射器实例
        /// </summary>
        public IAMapper CreateMapper()
        {
            return new Mapper(this);
        }

        /// <summary>
        /// 创建映射器实例，使用指定的服务提供者
        /// </summary>
        public IAMapper CreateMapper(IServiceProvider serviceProvider)
        {
            return new Mapper(this, serviceProvider);
        }

        /// <summary>
        /// 获取指定类型对的类型映射配置
        /// </summary>
        public ITypeMap FindTypeMap<TSource, TDestination>()
        {
            return FindTypeMap(typeof(TSource), typeof(TDestination));
        }

        /// <summary>
        /// 获取指定类型对的类型映射配置（非泛型版本）
        /// </summary>
        public ITypeMap FindTypeMap(Type sourceType, Type destinationType)
        {
            if (sourceType == null || destinationType == null)
                return null;

            var typePair = new TypePair(sourceType, destinationType);
            return _typeMaps.TryGetValue(typePair, out var typeMap) ? typeMap : null;
        }

        /// <summary>
        /// 获取所有已配置的类型映射
        /// </summary>
        public IReadOnlyCollection<ITypeMap> GetAllTypeMaps()
        {
            return _typeMaps.Values.ToList();
        }

        /// <summary>
        /// 检查指定类型对是否已配置映射
        /// </summary>
        public bool HasTypeMap<TSource, TDestination>()
        {
            return HasTypeMap(typeof(TSource), typeof(TDestination));
        }

        /// <summary>
        /// 检查指定类型对是否已配置映射（非泛型版本）
        /// </summary>
        public bool HasTypeMap(Type sourceType, Type destinationType)
        {
            if (sourceType == null || destinationType == null)
                return false;

            var typePair = new TypePair(sourceType, destinationType);
            return _typeMaps.ContainsKey(typePair);
        }

        /// <summary>
        /// 获取指定类型对的映射执行计划表达式
        /// </summary>
        public LambdaExpression BuildExecutionPlan<TSource, TDestination>()
        {
            return BuildExecutionPlan(typeof(TSource), typeof(TDestination));
        }

        /// <summary>
        /// 获取指定类型对的映射执行计划表达式（非泛型版本）
        /// </summary>
        public LambdaExpression BuildExecutionPlan(Type sourceType, Type destinationType)
        {
            var typePair = new TypePair(sourceType, destinationType);
            var typeMap = FindTypeMap(sourceType, destinationType) as TypeMap;
            
            if (typeMap == null)
            {
                throw new ConfigurationException($"未找到类型映射配置 / Type map not found: {typePair}");
            }

            // 使用 ExecutionPlanBuilder 构建执行计划
            if (_executionPlanBuilder != null)
            {
                return _executionPlanBuilder.BuildPlan(typePair);
            }

            // 回退到占位表达式
            var sourceParam = Expression.Parameter(sourceType, "source");
            var destParam = Expression.Parameter(destinationType, "destination");
            var contextParam = Expression.Parameter(typeof(ResolutionContext), "context");

            return Expression.Lambda(
                Expression.Default(destinationType),
                sourceParam, destParam, contextParam
            );
        }

        /// <summary>
        /// 预编译所有映射
        /// </summary>
        public void CompileMappings()
        {
            foreach (var typeMap in _typeMaps.Values)
            {
                CompileTypeMap(typeMap);
            }
        }

        /// <summary>
        /// 验证配置有效性
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            var errors = new List<ConfigurationError>();

            foreach (var typeMap in _typeMaps.Values)
            {
                ValidateTypeMap(typeMap, errors);
            }

            if (errors.Count > 0)
            {
                throw new ConfigurationException(errors);
            }
        }

        /// <summary>
        /// 验证指定类型映射的配置有效性
        /// </summary>
        public void AssertConfigurationIsValid<TSource, TDestination>()
        {
            var typeMap = FindTypeMap<TSource, TDestination>() as TypeMap;
            if (typeMap == null)
            {
                throw new ConfigurationException($"未找到类型映射配置 / Type map not found: {typeof(TSource).Name} -> {typeof(TDestination).Name}");
            }

            var errors = new List<ConfigurationError>();
            ValidateTypeMap(typeMap, errors);

            if (errors.Count > 0)
            {
                throw new ConfigurationException(errors);
            }
        }

        #endregion

        #region 内部方法 / Internal Methods

        /// <summary>
        /// 注册类型映射（内部使用）
        /// </summary>
        internal TypeMap RegisterTypeMap(Type sourceType, Type destinationType, MappingProfile profile)
        {
            if (_isSealed)
            {
                throw new InvalidOperationException("配置已密封，无法添加新的类型映射 / Configuration is sealed, cannot add new type maps");
            }

            var typePair = new TypePair(sourceType, destinationType);

            if (_typeMaps.ContainsKey(typePair))
            {
                throw new DuplicateTypeMapException(typePair);
            }

            var typeMap = new TypeMap(sourceType, destinationType, profile?.GetType());
            _typeMaps[typePair] = typeMap;

            return typeMap;
        }

        /// <summary>
        /// 添加 Profile（内部使用）
        /// </summary>
        internal void AddProfileInternal(MappingProfile profile)
        {
            if (_isSealed)
            {
                throw new InvalidOperationException("配置已密封，无法添加新的 Profile / Configuration is sealed, cannot add new profiles");
            }

            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _profiles.Add(profile);
            profile.Configure(this);
        }

        /// <summary>
        /// 设置服务提供者（内部使用）
        /// </summary>
        internal void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 添加对象映射器（内部使用）
        /// </summary>
        internal void AddMapper(IObjectMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _mappers.Add(mapper);
        }

        /// <summary>
        /// 插入对象映射器（内部使用）
        /// </summary>
        internal void InsertMapper(int index, IObjectMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _mappers.Insert(index, mapper);
        }

        /// <summary>
        /// 获取或编译映射函数
        /// </summary>
        internal Func<TSource, TDestination, ResolutionContext, TDestination> GetMapFunc<TSource, TDestination>()
        {
            var typePair = TypePair.Create<TSource, TDestination>();

            lock (_syncLock)
            {
                if (_mapFuncCache.TryGetValue(typePair, out var cached))
                {
                    return (Func<TSource, TDestination, ResolutionContext, TDestination>)cached;
                }

                var typeMap = FindTypeMap<TSource, TDestination>() as TypeMap;
                if (typeMap == null)
                {
                    throw new MappingException(typeof(TSource), typeof(TDestination), 
                        new InvalidOperationException($"未找到类型映射配置 / Type map not found"));
                }

                var func = CompileMapFunc<TSource, TDestination>(typeMap);
                _mapFuncCache[typePair] = func;
                return func;
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        private void Seal()
        {
            _isSealed = true;

            // 密封所有类型映射
            foreach (var typeMap in _typeMaps.Values)
            {
                typeMap.Seal();
            }

            // 创建执行计划构建器
            _executionPlanBuilder = new ExecutionPlanBuilder(this);
        }

        private void CompileTypeMap(TypeMap typeMap)
        {
            // 使用 ExecutionPlanBuilder 预编译类型映射
            if (_executionPlanBuilder != null)
            {
                _executionPlanBuilder.BuildPlan(typeMap.TypePair);
            }
        }

        private void ValidateTypeMap(TypeMap typeMap, List<ConfigurationError> errors)
        {
            // 检查未映射的成员
            var unmappedMembers = typeMap.GetUnmappedDestinationMembers();
            foreach (var member in unmappedMembers)
            {
                errors.Add(new ConfigurationError(typeMap, member, ConfigurationErrorType.UnmappedMember));
            }

            // 检查构造函数参数
            if (typeMap.ConstructorMap != null && !typeMap.ConstructorMap.CanResolve)
            {
                foreach (var param in typeMap.ConstructorMap.ParameterMaps)
                {
                    if (!param.IsMapped)
                    {
                        errors.Add(new ConfigurationError(typeMap, param.ParameterName, 
                            ConfigurationErrorType.UnresolvedConstructorParameter));
                    }
                }
            }
        }

        private Func<TSource, TDestination, ResolutionContext, TDestination> CompileMapFunc<TSource, TDestination>(TypeMap typeMap)
        {
            // 使用 ExecutionPlanBuilder 编译映射函数
            if (_executionPlanBuilder != null)
            {
                return _executionPlanBuilder.GetCompiledMapFunc<TSource, TDestination>();
            }

            // 回退到反射实现（仅在 ExecutionPlanBuilder 不可用时）
            return (source, destination, context) =>
            {
                if (source == null)
                    return default;

                // 创建目标对象
                if (destination == null)
                {
                    destination = Activator.CreateInstance<TDestination>();
                }

                return destination;
            };
        }

        #endregion
    }
}
