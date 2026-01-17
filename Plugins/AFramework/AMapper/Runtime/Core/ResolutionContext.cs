// ==========================================================
// 文件名：ResolutionContext.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 定义解析上下文，在映射过程中传递状态和配置
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 解析上下文
    /// <para>在映射过程中传递状态、配置和运行时数据</para>
    /// <para>Resolution context for passing state and configuration during mapping</para>
    /// </summary>
    /// <remarks>
    /// ResolutionContext 在整个映射过程中传递，提供：
    /// <list type="bullet">
    /// <item>映射器和配置的访问</item>
    /// <item>运行时数据传递（Items）</item>
    /// <item>循环引用跟踪</item>
    /// <item>映射深度控制</item>
    /// <item>服务提供者访问</item>
    /// </list>
    /// 
    /// 在值解析器中使用：
    /// <code>
    /// public class MyResolver : IValueResolver&lt;Source, Dest, string&gt;
    /// {
    ///     public string Resolve(Source src, Dest dest, string member, ResolutionContext context)
    ///     {
    ///         // 访问运行时数据
    ///         var userId = context.Items["UserId"];
    ///         
    ///         // 执行嵌套映射
    ///         var nested = context.Mapper.Map&lt;NestedDto&gt;(src.Nested);
    ///         
    ///         return $"{userId}_{nested.Name}";
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public class ResolutionContext
    {
        #region 私有字段 / Private Fields

        private Dictionary<object, object> _instanceCache;
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
        public IMapperConfiguration Configuration => Mapper?.Configuration;

        /// <summary>
        /// 获取运行时数据字典
        /// <para>Get the runtime data dictionary</para>
        /// </summary>
        /// <remarks>
        /// 通过 Map 调用时的 opt.Items 传递的数据。
        /// 可在值解析器、映射动作中访问。
        /// </remarks>
        public IDictionary<string, object> Items { get; }

        /// <summary>
        /// 获取服务提供者
        /// <para>Get the service provider</para>
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取当前映射深度
        /// <para>Get the current mapping depth</para>
        /// </summary>
        public int CurrentDepth => _currentDepth;

        /// <summary>
        /// 获取映射操作选项
        /// <para>Get the mapping operation options</para>
        /// </summary>
        public IMappingOperationOptions Options { get; }

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建解析上下文实例
        /// </summary>
        /// <param name="mapper">映射器实例 / Mapper instance</param>
        /// <param name="options">映射操作选项 / Mapping operation options</param>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        public ResolutionContext(IAMapper mapper, IMappingOperationOptions options = null, IServiceProvider serviceProvider = null)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Options = options;
            Items = options?.Items ?? new Dictionary<string, object>();
            ServiceProvider = serviceProvider ?? options?.ServiceProvider ?? mapper.Configuration?.ServiceProvider;
            _currentDepth = 0;
        }

        /// <summary>
        /// 内部构造函数，用于创建子上下文
        /// </summary>
        private ResolutionContext(ResolutionContext parent, int depth)
        {
            Mapper = parent.Mapper;
            Options = parent.Options;
            Items = parent.Items;
            ServiceProvider = parent.ServiceProvider;
            _instanceCache = parent._instanceCache;
            _currentDepth = depth;
        }

        #endregion

        #region 深度控制 / Depth Control

        /// <summary>
        /// 增加映射深度
        /// <para>Increment mapping depth</para>
        /// </summary>
        /// <returns>新的解析上下文 / New resolution context</returns>
        public ResolutionContext IncrementDepth()
        {
            return new ResolutionContext(this, _currentDepth + 1);
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

        #region 循环引用处理 / Circular Reference Handling

        /// <summary>
        /// 获取或创建实例缓存
        /// <para>Get or create instance cache</para>
        /// </summary>
        private Dictionary<object, object> InstanceCache
        {
            get
            {
                if (_instanceCache == null)
                {
                    _instanceCache = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
                }
                return _instanceCache;
            }
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
            if (source == null || _instanceCache == null)
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
                InstanceCache[source] = destination;
            }
        }

        #endregion

        #region 服务解析 / Service Resolution

        /// <summary>
        /// 从服务提供者获取服务
        /// <para>Get service from service provider</para>
        /// </summary>
        /// <typeparam name="TService">服务类型 / Service type</typeparam>
        /// <returns>服务实例或 null / Service instance or null</returns>
        public TService GetService<TService>() where TService : class
        {
            return ServiceProvider?.GetService(typeof(TService)) as TService;
        }

        /// <summary>
        /// 从服务提供者获取服务
        /// <para>Get service from service provider</para>
        /// </summary>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>服务实例或 null / Service instance or null</returns>
        public object GetService(Type serviceType)
        {
            return ServiceProvider?.GetService(serviceType);
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
