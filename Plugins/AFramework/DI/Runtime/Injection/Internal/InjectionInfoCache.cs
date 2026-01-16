// ==========================================================
// 文件名：InjectionInfoCache.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Concurrent, System.Reflection
// 功能: 缓存类型的注入信息，避免重复反射解析
// 优化: 使用ConcurrentDictionary实现无锁并发访问
// ==========================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 注入信息缓存
    /// <para>缓存类型的注入元数据，提升注入性能</para>
    /// <para>Injection information cache that caches type injection metadata for performance</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>使用 ConcurrentDictionary 实现无锁并发访问</item>
    /// <item>使用 GetOrAdd 确保原子性操作</item>
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// </list>
    /// </remarks>
    internal sealed class InjectionInfoCache
    {
        #region 单例 / Singleton

        private static readonly Lazy<InjectionInfoCache> _instance = 
            new Lazy<InjectionInfoCache>(() => new InjectionInfoCache());

        /// <summary>
        /// 获取缓存实例
        /// <para>Get the cache instance</para>
        /// </summary>
        public static InjectionInfoCache Instance => _instance.Value;

        #endregion

        #region 字段 / Fields

        /// <summary>
        /// 使用 ConcurrentDictionary 实现无锁并发访问
        /// </summary>
        private readonly ConcurrentDictionary<Type, InjectionInfo> _cache;

        /// <summary>
        /// 缓存构建委托，避免每次调用时创建闭包
        /// </summary>
        private readonly Func<Type, InjectionInfo> _buildFunc;

        // 反射绑定标志
        private const BindingFlags AllInstanceMembers = 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        #endregion

        #region 构造函数 / Constructor

        private InjectionInfoCache()
        {
            _cache = new ConcurrentDictionary<Type, InjectionInfo>();
            _buildFunc = BuildInjectionInfo;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 获取或创建类型的注入信息
        /// <para>Get or create injection information for a type</para>
        /// </summary>
        /// <remarks>
        /// 使用 ConcurrentDictionary.GetOrAdd 实现原子性操作，
        /// 避免多线程同时构建同一类型的注入信息
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InjectionInfo GetOrCreate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _cache.GetOrAdd(type, _buildFunc);
        }

        /// <summary>
        /// 尝试获取缓存的注入信息（不创建）
        /// <para>Try to get cached injection info without creating</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(Type type, out InjectionInfo info)
        {
            return _cache.TryGetValue(type, out info);
        }

        /// <summary>
        /// 清除缓存
        /// <para>Clear the cache</para>
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 从缓存中移除指定类型
        /// <para>Remove a specific type from cache</para>
        /// </summary>
        public bool Remove(Type type)
        {
            return _cache.TryRemove(type, out _);
        }

        /// <summary>
        /// 获取缓存的类型数量
        /// <para>Get the number of cached types</para>
        /// </summary>
        public int Count => _cache.Count;

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 构建类型的注入信息
        /// </summary>
        private InjectionInfo BuildInjectionInfo(Type type)
        {
            var constructor = BuildConstructorInfo(type);
            var fields = BuildFieldInfos(type);
            var properties = BuildPropertyInfos(type);
            var methods = BuildMethodInfos(type);

            return new InjectionInfo(type, constructor, fields, properties, methods);
        }

        /// <summary>
        /// 构建构造函数注入信息
        /// </summary>
        private ConstructorInjectionInfo BuildConstructorInfo(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            ConstructorInfo selectedCtor = null;

            // 优先查找标记了 [Inject] 的构造函数
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    selectedCtor = ctor;
                    break;
                }
            }

            // 如果没有标记，选择参数最多的公共构造函数
            if (selectedCtor == null)
            {
                int maxParams = -1;
                foreach (var ctor in constructors)
                {
                    if (!ctor.IsPublic) continue;
                    
                    var paramCount = ctor.GetParameters().Length;
                    if (paramCount > maxParams)
                    {
                        maxParams = paramCount;
                        selectedCtor = ctor;
                    }
                }
            }

            if (selectedCtor == null)
            {
                return null;
            }

            var parameters = BuildParameterInfos(selectedCtor.GetParameters());
            return new ConstructorInjectionInfo(selectedCtor, parameters);
        }

        /// <summary>
        /// 构建字段注入信息列表
        /// </summary>
        private IReadOnlyList<FieldInjectionInfo> BuildFieldInfos(Type type)
        {
            var result = new List<FieldInjectionInfo>();
            var currentType = type;

            // 遍历类型层次结构（包括基类）
            while (currentType != null && currentType != typeof(object))
            {
                var fields = currentType.GetFields(AllInstanceMembers | BindingFlags.DeclaredOnly);
                
                foreach (var field in fields)
                {
                    var injectAttr = field.GetCustomAttribute<InjectAttribute>();
                    if (injectAttr == null) continue;

                    var isOptional = field.GetCustomAttribute<OptionalAttribute>() != null;
                    var keyAttr = field.GetCustomAttribute<KeyAttribute>();
                    var fromParent = field.GetCustomAttribute<FromParentAttribute>() != null;

                    result.Add(new FieldInjectionInfo(
                        field,
                        isOptional,
                        keyAttr?.Key,
                        fromParent));
                }

                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 构建属性注入信息列表
        /// </summary>
        private IReadOnlyList<PropertyInjectionInfo> BuildPropertyInfos(Type type)
        {
            var result = new List<PropertyInjectionInfo>();
            var currentType = type;

            while (currentType != null && currentType != typeof(object))
            {
                var properties = currentType.GetProperties(AllInstanceMembers | BindingFlags.DeclaredOnly);
                
                foreach (var property in properties)
                {
                    var injectAttr = property.GetCustomAttribute<InjectAttribute>();
                    if (injectAttr == null) continue;
                    if (!property.CanWrite) continue;

                    var isOptional = property.GetCustomAttribute<OptionalAttribute>() != null;
                    var keyAttr = property.GetCustomAttribute<KeyAttribute>();
                    var fromParent = property.GetCustomAttribute<FromParentAttribute>() != null;

                    result.Add(new PropertyInjectionInfo(
                        property,
                        isOptional,
                        keyAttr?.Key,
                        fromParent));
                }

                currentType = currentType.BaseType;
            }

            return result;
        }

        /// <summary>
        /// 构建方法注入信息列表
        /// </summary>
        private IReadOnlyList<MethodInjectionInfo> BuildMethodInfos(Type type)
        {
            var result = new List<(MethodInjectionInfo Info, int Order)>();
            var currentType = type;

            while (currentType != null && currentType != typeof(object))
            {
                var methods = currentType.GetMethods(AllInstanceMembers | BindingFlags.DeclaredOnly);
                
                foreach (var method in methods)
                {
                    var injectAttr = method.GetCustomAttribute<InjectAttribute>();
                    if (injectAttr == null) continue;

                    // 跳过属性的 getter/setter
                    if (method.IsSpecialName) continue;

                    var parameters = BuildParameterInfos(method.GetParameters());
                    var orderAttr = method.GetCustomAttribute<OrderAttribute>();
                    var order = orderAttr?.Order ?? 0;

                    result.Add((new MethodInjectionInfo(method, parameters), order));
                }

                currentType = currentType.BaseType;
            }

            // 按 Order 排序
            result.Sort((a, b) => a.Order.CompareTo(b.Order));

            var sortedResult = new List<MethodInjectionInfo>(result.Count);
            foreach (var item in result)
            {
                sortedResult.Add(item.Info);
            }

            return sortedResult;
        }

        /// <summary>
        /// 构建参数注入信息列表
        /// </summary>
        private IReadOnlyList<ParameterInjectionInfo> BuildParameterInfos(ParameterInfo[] parameters)
        {
            var result = new List<ParameterInjectionInfo>(parameters.Length);

            foreach (var param in parameters)
            {
                var isOptional = param.GetCustomAttribute<OptionalAttribute>() != null || param.IsOptional;
                var keyAttr = param.GetCustomAttribute<KeyAttribute>();
                var fromParent = param.GetCustomAttribute<FromParentAttribute>() != null;

                result.Add(new ParameterInjectionInfo(
                    param.Name,
                    param.ParameterType,
                    isOptional,
                    keyAttr?.Key,
                    fromParent,
                    param.HasDefaultValue ? param.DefaultValue : null,
                    param.HasDefaultValue));
            }

            return result;
        }

        #endregion
    }
}
