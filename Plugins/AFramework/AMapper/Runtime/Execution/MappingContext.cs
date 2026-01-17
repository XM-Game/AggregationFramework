// ==========================================================
// 文件名：MappingContext.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 映射上下文，管理映射过程中的运行时状态
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射上下文
    /// <para>管理单次映射操作的运行时状态</para>
    /// <para>Mapping context that manages runtime state for a single mapping operation</para>
    /// </summary>
    /// <remarks>
    /// MappingContext 与 ResolutionContext 的区别：
    /// <list type="bullet">
    /// <item>MappingContext：内部使用，管理映射执行状态</item>
    /// <item>ResolutionContext：对外暴露，供值解析器等使用</item>
    /// </list>
    /// </remarks>
    public sealed class MappingContext
    {
        #region 私有字段 / Private Fields

        private readonly Stack<TypePair> _mappingStack;
        private readonly Dictionary<object, object> _instanceCache;
        private int _currentDepth;

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取映射器实例
        /// <para>Get the mapper instance</para>
        /// </summary>
        public IAMapper Mapper { get; }

        /// <summary>
        /// 获取映射配置
        /// <para>Get the mapper configuration</para>
        /// </summary>
        public IMapperConfiguration Configuration { get; }

        /// <summary>
        /// 获取解析上下文
        /// <para>Get the resolution context</para>
        /// </summary>
        public ResolutionContext ResolutionContext { get; }

        /// <summary>
        /// 获取映射操作选项
        /// <para>Get the mapping operation options</para>
        /// </summary>
        public IMappingOperationOptions Options { get; }

        /// <summary>
        /// 获取当前映射深度
        /// <para>Get the current mapping depth</para>
        /// </summary>
        public int CurrentDepth => _currentDepth;

        /// <summary>
        /// 获取服务提供者
        /// <para>Get the service provider</para>
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建映射上下文实例
        /// </summary>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="options">映射选项 / Mapping options</param>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        public MappingContext(IAMapper mapper, IMappingOperationOptions options = null, IServiceProvider serviceProvider = null)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Configuration = mapper.Configuration;
            Options = options;
            ServiceProvider = serviceProvider ?? options?.ServiceProvider ?? Configuration?.ServiceProvider;
            
            _mappingStack = new Stack<TypePair>();
            _instanceCache = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
            _currentDepth = 0;

            ResolutionContext = new ResolutionContext(mapper, options, ServiceProvider);
        }

        #endregion

        #region 深度控制 / Depth Control

        /// <summary>
        /// 进入映射（增加深度）
        /// <para>Enter mapping (increment depth)</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        public void EnterMapping(TypePair typePair)
        {
            _mappingStack.Push(typePair);
            _currentDepth++;
        }

        /// <summary>
        /// 退出映射（减少深度）
        /// <para>Exit mapping (decrement depth)</para>
        /// </summary>
        public void ExitMapping()
        {
            if (_mappingStack.Count > 0)
            {
                _mappingStack.Pop();
                _currentDepth--;
            }
        }

        /// <summary>
        /// 检查是否超过最大深度
        /// <para>Check if maximum depth is exceeded</para>
        /// </summary>
        /// <param name="maxDepth">最大深度 / Maximum depth</param>
        /// <returns>是否超过 / Whether exceeded</returns>
        public bool IsMaxDepthExceeded(int? maxDepth)
        {
            return maxDepth.HasValue && _currentDepth >= maxDepth.Value;
        }

        #endregion

        #region 循环引用检测 / Circular Reference Detection

        /// <summary>
        /// 检查是否存在循环映射
        /// <para>Check if circular mapping exists</para>
        /// </summary>
        /// <param name="typePair">类型对 / Type pair</param>
        /// <returns>是否存在循环 / Whether circular</returns>
        public bool IsCircularMapping(TypePair typePair)
        {
            return _mappingStack.Contains(typePair);
        }

        /// <summary>
        /// 尝试获取已缓存的目标实例
        /// <para>Try to get cached destination instance</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <returns>是否找到缓存 / Whether cache found</returns>
        public bool TryGetCachedDestination(object source, out object destination)
        {
            if (source == null)
            {
                destination = null;
                return false;
            }
            return _instanceCache.TryGetValue(source, out destination);
        }

        /// <summary>
        /// 缓存目标实例
        /// <para>Cache destination instance</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        public void CacheDestination(object source, object destination)
        {
            if (source != null && destination != null)
            {
                _instanceCache[source] = destination;
            }
        }

        #endregion

        #region 服务解析 / Service Resolution

        /// <summary>
        /// 获取服务实例
        /// <para>Get service instance</para>
        /// </summary>
        /// <typeparam name="TService">服务类型 / Service type</typeparam>
        /// <returns>服务实例或 null / Service instance or null</returns>
        public TService GetService<TService>() where TService : class
        {
            return ServiceProvider?.GetService(typeof(TService)) as TService;
        }

        /// <summary>
        /// 获取服务实例
        /// <para>Get service instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>服务实例或 null / Service instance or null</returns>
        public object GetService(Type serviceType)
        {
            return ServiceProvider?.GetService(serviceType);
        }

        /// <summary>
        /// 创建服务实例
        /// <para>Create service instance</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>服务实例 / Service instance</returns>
        public object CreateInstance(Type serviceType)
        {
            // 优先使用选项中的构造函数
            if (Options?.ConstructServicesUsing != null)
            {
                return Options.ConstructServicesUsing(serviceType);
            }

            // 尝试从服务提供者获取
            var service = GetService(serviceType);
            if (service != null)
            {
                return service;
            }

            // 使用 Activator 创建
            return Activator.CreateInstance(serviceType);
        }

        #endregion

        #region 嵌套类 / Nested Classes

        /// <summary>
        /// 引用相等性比较器
        /// <para>Reference equality comparer</para>
        /// </summary>
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            private ReferenceEqualityComparer() { }

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }

        #endregion
    }
}
