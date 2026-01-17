// ==========================================================
// 文件名：ObjectFactory.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: 对象工厂，负责创建目标对象实例
// ==========================================================

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 对象工厂
    /// <para>负责创建目标对象实例，支持多种创建策略</para>
    /// <para>Object factory responsible for creating destination object instances</para>
    /// </summary>
    /// <remarks>
    /// ObjectFactory 使用表达式树编译工厂方法，提供高性能的对象创建：
    /// <list type="bullet">
    /// <item>缓存编译后的工厂方法，避免重复编译</item>
    /// <item>支持值类型、引用类型、数组、集合等</item>
    /// <item>支持私有构造函数</item>
    /// <item>支持服务提供者集成</item>
    /// </list>
    /// </remarks>
    public static class ObjectFactory
    {
        #region 私有字段 / Private Fields

        private static readonly ConcurrentDictionary<Type, Func<object>> _factoryCache = new();
        private static readonly ConcurrentDictionary<Type, Func<int, object>> _collectionFactoryCache = new();

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 创建对象实例
        /// <para>Create object instance</para>
        /// </summary>
        /// <typeparam name="T">对象类型 / Object type</typeparam>
        /// <returns>对象实例 / Object instance</returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// 创建对象实例
        /// <para>Create object instance</para>
        /// </summary>
        /// <param name="type">对象类型 / Object type</param>
        /// <returns>对象实例 / Object instance</returns>
        public static object CreateInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var factory = _factoryCache.GetOrAdd(type, CreateFactory);
            return factory();
        }

        /// <summary>
        /// 使用服务提供者创建对象实例
        /// <para>Create object instance using service provider</para>
        /// </summary>
        /// <param name="type">对象类型 / Object type</param>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        /// <returns>对象实例 / Object instance</returns>
        public static object CreateInstance(Type type, IServiceProvider serviceProvider)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // 优先从服务提供者获取
            if (serviceProvider != null)
            {
                var service = serviceProvider.GetService(type);
                if (service != null)
                    return service;
            }

            return CreateInstance(type);
        }

        /// <summary>
        /// 使用构造函数参数创建对象实例
        /// <para>Create object instance with constructor arguments</para>
        /// </summary>
        /// <param name="type">对象类型 / Object type</param>
        /// <param name="args">构造函数参数 / Constructor arguments</param>
        /// <returns>对象实例 / Object instance</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (args == null || args.Length == 0)
                return CreateInstance(type);

            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 创建集合实例
        /// <para>Create collection instance</para>
        /// </summary>
        /// <param name="collectionType">集合类型 / Collection type</param>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        /// <returns>集合实例 / Collection instance</returns>
        public static object CreateCollection(Type collectionType, int capacity = 0)
        {
            if (collectionType == null)
                throw new ArgumentNullException(nameof(collectionType));

            var factory = _collectionFactoryCache.GetOrAdd(collectionType, CreateCollectionFactory);
            return factory(capacity);
        }

        /// <summary>
        /// 创建数组实例
        /// <para>Create array instance</para>
        /// </summary>
        /// <param name="elementType">元素类型 / Element type</param>
        /// <param name="length">数组长度 / Array length</param>
        /// <returns>数组实例 / Array instance</returns>
        public static Array CreateArray(Type elementType, int length)
        {
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));

            return Array.CreateInstance(elementType, length);
        }

        /// <summary>
        /// 创建字典实例
        /// <para>Create dictionary instance</para>
        /// </summary>
        /// <param name="keyType">键类型 / Key type</param>
        /// <param name="valueType">值类型 / Value type</param>
        /// <param name="capacity">初始容量 / Initial capacity</param>
        /// <returns>字典实例 / Dictionary instance</returns>
        public static IDictionary CreateDictionary(Type keyType, Type valueType, int capacity = 0)
        {
            if (keyType == null)
                throw new ArgumentNullException(nameof(keyType));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            
            if (capacity > 0)
            {
                return (IDictionary)Activator.CreateInstance(dictType, capacity);
            }
            
            return (IDictionary)CreateInstance(dictType);
        }

        /// <summary>
        /// 检查类型是否可以创建实例
        /// <para>Check if type can be instantiated</para>
        /// </summary>
        /// <param name="type">类型 / Type</param>
        /// <returns>是否可创建 / Whether can create</returns>
        public static bool CanCreateInstance(Type type)
        {
            if (type == null)
                return false;

            // 抽象类和接口不能直接创建
            if (type.IsAbstract || type.IsInterface)
                return false;

            // 值类型总是可以创建
            if (type.IsValueType)
                return true;

            // 检查是否有可访问的构造函数
            var ctor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, Type.EmptyTypes, null);

            return ctor != null;
        }

        /// <summary>
        /// 清除工厂缓存
        /// <para>Clear factory cache</para>
        /// </summary>
        public static void ClearCache()
        {
            _factoryCache.Clear();
            _collectionFactoryCache.Clear();
        }

        #endregion

        #region 私有方法 / Private Methods

        private static Func<object> CreateFactory(Type type)
        {
            // 处理值类型
            if (type.IsValueType)
            {
                var defaultExpr = Expression.Default(type);
                var boxed = Expression.Convert(defaultExpr, typeof(object));
                return Expression.Lambda<Func<object>>(boxed).Compile();
            }

            // 处理数组
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return () => Array.CreateInstance(elementType, 0);
            }

            // 处理字符串
            if (type == typeof(string))
            {
                return () => string.Empty;
            }

            // 查找无参构造函数
            var ctor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, Type.EmptyTypes, null);

            if (ctor != null)
            {
                // 使用表达式树编译工厂方法
                var newExpr = Expression.New(ctor);
                var lambda = Expression.Lambda<Func<object>>(
                    Expression.Convert(newExpr, typeof(object))
                );
                return lambda.Compile();
            }

            // 回退到 Activator
            return () => Activator.CreateInstance(type, true);
        }

        private static Func<int, object> CreateCollectionFactory(Type collectionType)
        {
            // 处理数组
            if (collectionType.IsArray)
            {
                var elementType = collectionType.GetElementType();
                return capacity => Array.CreateInstance(elementType, capacity);
            }

            // 处理泛型集合
            if (collectionType.IsGenericType)
            {
                var genericDef = collectionType.GetGenericTypeDefinition();
                var elementType = collectionType.GetGenericArguments()[0];

                // List<T>
                if (genericDef == typeof(List<>) || genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>))
                {
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var ctor = listType.GetConstructor(new[] { typeof(int) });
                    if (ctor != null)
                    {
                        var capacityParam = Expression.Parameter(typeof(int), "capacity");
                        var newExpr = Expression.New(ctor, capacityParam);
                        return Expression.Lambda<Func<int, object>>(
                            Expression.Convert(newExpr, typeof(object)), capacityParam).Compile();
                    }
                }

                // HashSet<T>
                if (genericDef == typeof(HashSet<>) || genericDef == typeof(ISet<>))
                {
                    var setType = typeof(HashSet<>).MakeGenericType(elementType);
                    return capacity => Activator.CreateInstance(setType);
                }
            }

            // 默认创建
            return capacity => CreateInstance(collectionType);
        }

        #endregion
    }

    /// <summary>
    /// 目标对象工厂
    /// <para>专门用于创建映射目标对象</para>
    /// <para>Destination factory specifically for creating mapping destination objects</para>
    /// </summary>
    /// <remarks>
    /// DestinationFactory 提供映射场景下的对象创建能力：
    /// <list type="bullet">
    /// <item>支持自定义工厂函数</item>
    /// <item>支持服务提供者集成</item>
    /// <item>支持 TypeMap 配置的构造方式</item>
    /// <item>支持构造函数参数映射</item>
    /// </list>
    /// </remarks>
    public sealed class DestinationFactory
    {
        #region 私有字段 / Private Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly Func<Type, object> _customFactory;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建目标对象工厂
        /// </summary>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        /// <param name="customFactory">自定义工厂 / Custom factory</param>
        public DestinationFactory(IServiceProvider serviceProvider = null, Func<Type, object> customFactory = null)
        {
            _serviceProvider = serviceProvider;
            _customFactory = customFactory;
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 创建目标对象
        /// <para>Create destination object</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <returns>目标对象 / Destination object</returns>
        public TDestination Create<TDestination>()
        {
            return (TDestination)Create(typeof(TDestination));
        }

        /// <summary>
        /// 创建目标对象
        /// <para>Create destination object</para>
        /// </summary>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>目标对象 / Destination object</returns>
        public object Create(Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            // 优先使用自定义工厂
            if (_customFactory != null)
            {
                var result = _customFactory(destinationType);
                if (result != null)
                    return result;
            }

            // 使用服务提供者
            if (_serviceProvider != null)
            {
                var service = _serviceProvider.GetService(destinationType);
                if (service != null)
                    return service;
            }

            // 使用默认工厂
            return ObjectFactory.CreateInstance(destinationType);
        }

        /// <summary>
        /// 使用 TypeMap 创建目标对象
        /// <para>Create destination object using TypeMap</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>目标对象 / Destination object</returns>
        public object Create(object source, TypeMap typeMap, ResolutionContext context)
        {
            if (typeMap == null)
                throw new ArgumentNullException(nameof(typeMap));

            // 使用自定义构造函数
            if (typeMap.CustomCtorFunction != null)
            {
                return InvokeCustomCtorFunction(typeMap.CustomCtorFunction, source, context);
            }

            // 使用自定义构造表达式
            if (typeMap.CustomCtorExpression != null)
            {
                return InvokeCustomCtorExpression(typeMap.CustomCtorExpression, source, context);
            }

            // 使用构造函数映射
            if (typeMap.ConstructorMap != null && typeMap.ConstructorMap.CanResolve)
            {
                return CreateWithConstructorMap(source, typeMap.ConstructorMap, context);
            }

            // 使用默认创建
            return Create(typeMap.DestinationType);
        }

        /// <summary>
        /// 使用构造函数映射创建目标对象
        /// <para>Create destination object using constructor map</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="constructorMap">构造函数映射 / Constructor map</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>目标对象 / Destination object</returns>
        public object CreateWithConstructorMap(object source, IConstructorMap constructorMap, ResolutionContext context = null)
        {
            if (constructorMap == null)
                throw new ArgumentNullException(nameof(constructorMap));

            var parameters = new object[constructorMap.ParameterMaps.Count];

            for (int i = 0; i < constructorMap.ParameterMaps.Count; i++)
            {
                var paramMap = constructorMap.ParameterMaps[i];
                parameters[i] = ResolveParameterValue(source, paramMap, context);
            }

            return constructorMap.Constructor.Invoke(parameters);
        }

        #endregion

        #region 私有方法 / Private Methods

        private object InvokeCustomCtorFunction(Delegate ctorFunc, object source, ResolutionContext context)
        {
            var paramCount = ctorFunc.Method.GetParameters().Length;
            return paramCount switch
            {
                1 => ctorFunc.DynamicInvoke(source),
                2 => ctorFunc.DynamicInvoke(source, context),
                _ => ctorFunc.DynamicInvoke(source)
            };
        }

        private object InvokeCustomCtorExpression(System.Linq.Expressions.LambdaExpression ctorExpr, object source, ResolutionContext context)
        {
            var compiled = ctorExpr.Compile();
            var paramCount = ctorExpr.Parameters.Count;
            return paramCount switch
            {
                1 => compiled.DynamicInvoke(source),
                2 => compiled.DynamicInvoke(source, context),
                _ => compiled.DynamicInvoke(source)
            };
        }

        private object ResolveParameterValue(object source, ConstructorParameterMap paramMap, ResolutionContext context)
        {
            // 自定义映射表达式
            if (paramMap.CustomMapExpression != null)
            {
                var compiled = paramMap.CustomMapExpression.Compile();
                return compiled.DynamicInvoke(source);
            }

            // 源成员
            if (paramMap.SourceMember != null)
            {
                var value = GetMemberValue(source, paramMap.SourceMember);
                
                // 类型转换
                if (value != null && !paramMap.ParameterType.IsInstanceOfType(value))
                {
                    value = Convert.ChangeType(value, paramMap.ParameterType);
                }
                
                return value;
            }

            // 默认值
            if (paramMap.HasDefaultValue)
            {
                return paramMap.DefaultValue;
            }

            // 返回类型默认值
            return paramMap.ParameterType.IsValueType 
                ? Activator.CreateInstance(paramMap.ParameterType) 
                : null;
        }

        private static object GetMemberValue(object obj, MemberInfo member)
        {
            if (obj == null)
                return null;

            return member switch
            {
                PropertyInfo prop => prop.GetValue(obj),
                FieldInfo field => field.GetValue(obj),
                _ => null
            };
        }

        #endregion
    }
}
