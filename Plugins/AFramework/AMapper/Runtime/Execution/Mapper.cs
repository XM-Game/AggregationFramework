// ==========================================================
// 文件名：Mapper.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 映射器实现，执行实际的对象映射操作
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射器实现
    /// <para>执行实际的对象映射操作</para>
    /// <para>Mapper implementation that performs actual object mapping operations</para>
    /// </summary>
    public sealed class Mapper : IAMapper
    {
        #region 私有字段 / Private Fields

        private readonly MapperConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region 属性 / Properties

        /// <inheritdoc/>
        public IMapperConfiguration Configuration => _configuration;

        #endregion

        #region 构造函数 / Constructors

        /// <summary>
        /// 创建映射器实例
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        internal Mapper(MapperConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceProvider = configuration.ServiceProvider;
        }

        /// <summary>
        /// 创建映射器实例，使用指定的服务提供者
        /// </summary>
        /// <param name="configuration">映射配置 / Mapper configuration</param>
        /// <param name="serviceProvider">服务提供者 / Service provider</param>
        internal Mapper(MapperConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceProvider = serviceProvider ?? configuration.ServiceProvider;
        }

        #endregion

        #region IAMapper 实现 / IAMapper Implementation

        /// <inheritdoc/>
        public TDestination Map<TDestination>(object source)
        {
            if (source == null)
                return default;

            return Map<TDestination>(source, null);
        }

        /// <inheritdoc/>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            if (source == null)
                return default;

            return MapCore<TSource, TDestination>(source, default, null);
        }

        /// <inheritdoc/>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return MapCore(source, destination, null);
        }

        /// <inheritdoc/>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            var options = new MappingOperationOptions<TSource, TDestination>();
            opts?.Invoke(options);

            return MapCore(source, destination, options);
        }

        /// <inheritdoc/>
        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts)
        {
            if (source == null)
                return default;

            var options = new MappingOperationOptions();
            opts?.Invoke(options);

            var sourceType = source.GetType();
            var destType = typeof(TDestination);

            return (TDestination)MapCore(source, sourceType, null, destType, options);
        }

        /// <inheritdoc/>
        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            if (source == null)
                return default;

            var options = new MappingOperationOptions<TSource, TDestination>();
            opts?.Invoke(options);

            return MapCore<TSource, TDestination>(source, default(TDestination), options);
        }

        /// <inheritdoc/>
        public object Map(object source, Type sourceType, Type destinationType)
        {
            if (source == null)
                return null;

            return MapCore(source, sourceType, null, destinationType, null);
        }

        /// <inheritdoc/>
        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
        {
            if (source == null)
                return null;

            var options = new MappingOperationOptions();
            opts?.Invoke(options);

            return MapCore(source, sourceType, null, destinationType, options);
        }

        /// <inheritdoc/>
        public object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return MapCore(source, sourceType, destination, destinationType, null);
        }

        /// <inheritdoc/>
        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
        {
            var options = new MappingOperationOptions();
            opts?.Invoke(options);

            return MapCore(source, sourceType, destination, destinationType, options);
        }

        #endregion

        #region 核心映射方法 / Core Mapping Methods

        private TDestination MapCore<TSource, TDestination>(
            TSource source, 
            TDestination destination, 
            IMappingOperationOptions options)
        {
            if (source == null)
                return destination;

            var context = new ResolutionContext(this, options, _serviceProvider);

            // 获取编译后的映射函数
            var mapFunc = _configuration.GetMapFunc<TSource, TDestination>();
            return mapFunc(source, destination, context);
        }

        private object MapCore(
            object source,
            Type sourceType,
            object destination,
            Type destinationType,
            IMappingOperationOptions options)
        {
            if (source == null)
                return destination;

            var context = new ResolutionContext(this, options, _serviceProvider);

            // 查找类型映射
            var typeMap = _configuration.FindTypeMap(sourceType, destinationType) as TypeMap;
            if (typeMap == null)
            {
                // 尝试直接赋值
                if (destinationType.IsAssignableFrom(sourceType))
                {
                    return source;
                }

                throw new MappingException(sourceType, destinationType,
                    new InvalidOperationException($"未找到类型映射配置 / Type map not found"));
            }

            // 执行映射
            return ExecuteMapping(source, destination, typeMap, context);
        }

        private object ExecuteMapping(
            object source,
            object destination,
            TypeMap typeMap,
            ResolutionContext context)
        {
            try
            {
                // 检查循环引用
                if (typeMap.PreserveReferences && context.TryGetCachedDestination(source, out var cached))
                {
                    return cached;
                }

                // 检查最大深度
                if (context.IsMaxDepthExceeded(typeMap.MaxDepth))
                {
                    return destination;
                }

                // 创建目标对象
                if (destination == null)
                {
                    destination = CreateDestination(source, typeMap, context);
                }

                // 缓存目标对象（处理循环引用）
                if (typeMap.PreserveReferences)
                {
                    context.CacheDestination(source, destination);
                }

                // 执行前置映射动作
                ExecuteBeforeMapActions(source, destination, typeMap, context);

                // 执行成员映射
                MapMembers(source, destination, typeMap, context);

                // 执行后置映射动作
                ExecuteAfterMapActions(source, destination, typeMap, context);

                return destination;
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException(typeMap.SourceType, typeMap.DestinationType, ex);
            }
        }

        private object CreateDestination(object source, TypeMap typeMap, ResolutionContext context)
        {
            // 使用自定义构造函数
            if (typeMap.CustomCtorFunction != null)
            {
                return typeMap.CustomCtorFunction.DynamicInvoke(source, context);
            }

            // 使用自定义构造表达式
            if (typeMap.CustomCtorExpression != null)
            {
                var compiled = typeMap.CustomCtorExpression.Compile();
                return compiled.DynamicInvoke(source);
            }

            // 使用构造函数映射
            if (typeMap.ConstructorMap != null && typeMap.ConstructorMap.CanResolve)
            {
                return CreateWithConstructorMap(source, typeMap.ConstructorMap, context);
            }

            // 使用默认构造函数
            return Activator.CreateInstance(typeMap.DestinationType);
        }

        private object CreateWithConstructorMap(
            object source, 
            IConstructorMap constructorMap, 
            ResolutionContext context)
        {
            var parameters = new object[constructorMap.ParameterMaps.Count];

            for (int i = 0; i < constructorMap.ParameterMaps.Count; i++)
            {
                var paramMap = constructorMap.ParameterMaps[i];
                
                if (paramMap.CustomMapExpression != null)
                {
                    var compiled = paramMap.CustomMapExpression.Compile();
                    parameters[i] = compiled.DynamicInvoke(source);
                }
                else if (paramMap.SourceMember != null)
                {
                    parameters[i] = GetMemberValue(source, paramMap.SourceMember);
                }
                else if (paramMap.HasDefaultValue)
                {
                    parameters[i] = paramMap.DefaultValue;
                }
            }

            return constructorMap.Constructor.Invoke(parameters);
        }

        private void MapMembers(
            object source, 
            object destination, 
            TypeMap typeMap, 
            ResolutionContext context)
        {
            foreach (var memberMap in typeMap.MemberMaps)
            {
                if (memberMap.IsIgnored)
                    continue;

                try
                {
                    MapMember(source, destination, (MemberMap)memberMap, context);
                }
                catch (Exception ex)
                {
                    throw new MappingException(typeMap.SourceType, typeMap.DestinationType, 
                        memberMap.DestinationName, ex);
                }
            }
        }

        private void MapMember(
            object source, 
            object destination, 
            MemberMap memberMap, 
            ResolutionContext context)
        {
            // 检查前置条件
            if (memberMap.PreConditionExpression != null)
            {
                var preCondition = memberMap.PreConditionExpression.Compile();
                var result = preCondition.DynamicInvoke(source);
                if (result is bool b && !b)
                    return;
            }

            // 获取源值
            object sourceValue = null;

            if (memberMap.CustomMapExpression != null)
            {
                var compiled = memberMap.CustomMapExpression.Compile();
                var paramCount = memberMap.CustomMapExpression.Parameters.Count;
                
                sourceValue = paramCount switch
                {
                    1 => compiled.DynamicInvoke(source),
                    2 => compiled.DynamicInvoke(source, destination),
                    _ => compiled.DynamicInvoke(source, destination, null, context)
                };
            }
            else if (memberMap.SourceMembers != null && memberMap.SourceMembers.Length > 0)
            {
                sourceValue = GetNestedMemberValue(source, memberMap.SourceMembers);
            }

            // 检查条件
            if (memberMap.ConditionExpression != null)
            {
                var condition = memberMap.ConditionExpression.Compile();
                var result = condition.DynamicInvoke(source);
                if (result is bool b && !b)
                    return;
            }

            // 处理空值替换
            if (sourceValue == null && memberMap.HasNullSubstitute)
            {
                sourceValue = memberMap.NullSubstitute;
            }

            // 设置目标值
            SetMemberValue(destination, memberMap.DestinationMember, sourceValue);
        }

        private void ExecuteBeforeMapActions(
            object source, 
            object destination, 
            TypeMap typeMap, 
            ResolutionContext context)
        {
            foreach (var action in typeMap.BeforeMapActions)
            {
                var compiled = action.Compile();
                var paramCount = action.Parameters.Count;
                
                if (paramCount == 2)
                    compiled.DynamicInvoke(source, destination);
                else if (paramCount == 3)
                    compiled.DynamicInvoke(source, destination, context);
            }
        }

        private void ExecuteAfterMapActions(
            object source, 
            object destination, 
            TypeMap typeMap, 
            ResolutionContext context)
        {
            foreach (var action in typeMap.AfterMapActions)
            {
                var compiled = action.Compile();
                var paramCount = action.Parameters.Count;
                
                if (paramCount == 2)
                    compiled.DynamicInvoke(source, destination);
                else if (paramCount == 3)
                    compiled.DynamicInvoke(source, destination, context);
            }
        }

        #endregion

        #region 辅助方法 / Helper Methods

        private static object GetMemberValue(object obj, System.Reflection.MemberInfo member)
        {
            return member switch
            {
                System.Reflection.PropertyInfo prop => prop.GetValue(obj),
                System.Reflection.FieldInfo field => field.GetValue(obj),
                _ => null
            };
        }

        private static object GetNestedMemberValue(object obj, System.Reflection.MemberInfo[] members)
        {
            var current = obj;
            foreach (var member in members)
            {
                if (current == null)
                    return null;
                current = GetMemberValue(current, member);
            }
            return current;
        }

        private static void SetMemberValue(object obj, System.Reflection.MemberInfo member, object value)
        {
            switch (member)
            {
                case System.Reflection.PropertyInfo prop:
                    prop.SetValue(obj, value);
                    break;
                case System.Reflection.FieldInfo field:
                    field.SetValue(obj, value);
                    break;
            }
        }

        #endregion
    }
}
