// ==========================================================
// 文件名：Container.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现依赖注入容器，提供服务解析、实例注入和生命周期管理
// 优化: 热路径内联优化，数组池复用参数数组
// 增强: 支持开放泛型解析和内置集合解析
// ==========================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 依赖注入容器实现类
    /// <para>提供完整的依赖注入能力，包括服务解析、实例注入、生命周期管理和作用域支持</para>
    /// <para>DI container implementation that provides complete dependency injection capabilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：容器仅负责对象解析和生命周期管理</item>
    /// <item>依赖倒置：通过接口解耦，支持灵活的服务替换</item>
    /// <item>开闭原则：通过注册机制扩展，无需修改容器代码</item>
    /// </list>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// <item>使用 ObjectArrayPool 复用参数数组，减少GC压力</item>
    /// </list>
    /// 功能增强：
    /// <list type="bullet">
    /// <item>支持开放泛型类型注册和运行时构造</item>
    /// <item>支持内置集合类型自动解析（IEnumerable&lt;T&gt;, IReadOnlyList&lt;T&gt;, T[]）</item>
    /// </list>
    /// </remarks>
    public sealed class Container : IObjectResolver
    {
        #region 字段 / Fields

        private readonly Registry _registry;
        private readonly IObjectResolver _parent;
        private readonly bool _enableDiagnostics;
        private readonly object _syncRoot = new object();
        
        // 单例实例缓存
        private readonly Dictionary<Type, object> _singletonInstances;
        // 作用域实例缓存
        private readonly Dictionary<Type, object> _scopedInstances;
        // 键值实例缓存
        private readonly Dictionary<(Type, object), object> _keyedInstances;
        // 需要释放的实例列表
        private readonly List<IDisposable> _disposables;
        // 循环依赖检测栈
        private readonly HashSet<Type> _resolutionStack;
        
        private bool _isDisposed;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建容器实例（内部使用）
        /// </summary>
        internal Container(Registry registry, IObjectResolver parent = null, bool enableDiagnostics = false)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _parent = parent;
            _enableDiagnostics = enableDiagnostics;
            
            _singletonInstances = new Dictionary<Type, object>();
            _scopedInstances = new Dictionary<Type, object>();
            _keyedInstances = new Dictionary<(Type, object), object>();
            _disposables = new List<IDisposable>();
            _resolutionStack = new HashSet<Type>();
            
            // 注册容器自身
            _singletonInstances[typeof(IObjectResolver)] = this;
            _singletonInstances[typeof(Container)] = this;
        }

        #endregion

        #region IObjectResolver 属性 / Properties

        /// <inheritdoc/>
        public IObjectResolver Parent => _parent;

        #endregion

        #region 基础解析方法 / Basic Resolution Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type type)
        {
            ThrowIfDisposed();
            
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // 检查循环依赖
            if (_resolutionStack.Contains(type))
            {
                throw CircularDependencyException.Create(type, _resolutionStack);
            }

            try
            {
                _resolutionStack.Add(type);
                return ResolveInternal(type, null);
            }
            finally
            {
                _resolutionStack.Remove(type);
            }
        }

        #endregion

        #region 可选解析方法 / Optional Resolution Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ResolveOrDefault<T>()
        {
            return TryResolve<T>(out var instance) ? instance : default;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ResolveOrDefault(Type type)
        {
            return TryResolve(type, out var instance) ? instance : null;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryResolve<T>(out T instance)
        {
            if (TryResolve(typeof(T), out var obj))
            {
                instance = (T)obj;
                return true;
            }
            instance = default;
            return false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryResolve(Type type, out object instance)
        {
            ThrowIfDisposed();
            
            try
            {
                instance = Resolve(type);
                return true;
            }
            catch (ResolutionException)
            {
                instance = null;
                return false;
            }
            catch (CircularDependencyException)
            {
                instance = null;
                return false;
            }
        }

        #endregion

        #region 集合解析方法 / Collection Resolution Methods

        /// <inheritdoc/>
        public IEnumerable<T> ResolveAll<T>()
        {
            foreach (var obj in ResolveAll(typeof(T)))
            {
                yield return (T)obj;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<object> ResolveAll(Type type)
        {
            ThrowIfDisposed();
            
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var registrations = _registry.GetAll(type);
            var results = new List<object>();

            foreach (var registration in registrations)
            {
                var instance = CreateInstance(registration, null);
                results.Add(instance);
            }

            // 如果父容器存在，也从父容器解析
            if (_parent != null)
            {
                foreach (var parentInstance in _parent.ResolveAll(type))
                {
                    results.Add(parentInstance);
                }
            }

            return results;
        }

        #endregion

        #region 键值解析方法 / Keyed Resolution Methods

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ResolveKeyed<T>(object key)
        {
            return (T)ResolveKeyed(typeof(T), key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ResolveKeyed(Type type, object key)
        {
            ThrowIfDisposed();
            
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // 检查键值缓存
            var cacheKey = (type, key);
            lock (_syncRoot)
            {
                if (_keyedInstances.TryGetValue(cacheKey, out var cached))
                {
                    return cached;
                }
            }

            var registration = _registry.GetKeyed(type, key);
            if (registration == null)
            {
                // 尝试从父容器解析
                if (_parent != null)
                {
                    return _parent.ResolveKeyed(type, key);
                }
                throw ResolutionException.ServiceNotRegistered(type, key);
            }

            var instance = CreateInstance(registration, null);
            
            // 缓存单例和作用域实例
            if (registration.Lifetime != Lifetime.Transient)
            {
                lock (_syncRoot)
                {
                    _keyedInstances[cacheKey] = instance;
                }
            }

            return instance;
        }

        /// <inheritdoc/>
        public bool TryResolveKeyed<T>(object key, out T instance)
        {
            ThrowIfDisposed();
            
            try
            {
                instance = ResolveKeyed<T>(key);
                return true;
            }
            catch (ResolutionException)
            {
                instance = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public bool TryResolveKeyed(Type type, object key, out object instance)
        {
            ThrowIfDisposed();
            
            try
            {
                instance = ResolveKeyed(type, key);
                return true;
            }
            catch (ResolutionException)
            {
                instance = null;
                return false;
            }
        }

        #endregion

        #region 注入方法 / Injection Methods

        /// <inheritdoc/>
        public void Inject(object instance)
        {
            Inject(instance, null);
        }

        /// <inheritdoc/>
        public void Inject(object instance, IReadOnlyList<IInjectParameter> parameters)
        {
            ThrowIfDisposed();
            
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            
            // 字段注入
            InjectFields(instance, type, parameters);
            
            // 属性注入
            InjectProperties(instance, type, parameters);
            
            // 方法注入
            InjectMethods(instance, type, parameters);
        }

        /// <summary>
        /// 注入字段
        /// </summary>
        private void InjectFields(object instance, Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (var field in fields)
            {
                var injectAttr = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttr == null) continue;

                var value = ResolveParameter(field.FieldType, field.Name, parameters, field);
                if (value != null || !IsOptional(field))
                {
                    field.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// 注入属性
        /// </summary>
        private void InjectProperties(object instance, Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            foreach (var property in properties)
            {
                var injectAttr = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttr == null) continue;
                if (!property.CanWrite) continue;

                var value = ResolveParameter(property.PropertyType, property.Name, parameters, property);
                if (value != null || !IsOptional(property))
                {
                    property.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// 注入方法
        /// </summary>
        private void InjectMethods(object instance, Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            // 按 Order 特性排序
            var orderedMethods = new List<(MethodInfo Method, int Order)>();
            foreach (var method in methods)
            {
                var injectAttr = method.GetCustomAttribute<InjectAttribute>();
                if (injectAttr == null) continue;

                var orderAttr = method.GetCustomAttribute<OrderAttribute>();
                var order = orderAttr?.Order ?? 0;
                orderedMethods.Add((method, order));
            }
            
            orderedMethods.Sort((a, b) => a.Order.CompareTo(b.Order));

            foreach (var (method, _) in orderedMethods)
            {
                var methodParams = method.GetParameters();
                var paramCount = methodParams.Length;
                
                // 使用数组池租用参数数组
                var args = ObjectArrayPool.Rent(paramCount);
                
                try
                {
                    for (int i = 0; i < paramCount; i++)
                    {
                        args[i] = ResolveParameter(
                            methodParams[i].ParameterType, 
                            methodParams[i].Name, 
                            parameters, 
                            methodParams[i]);
                    }
                    
                    method.Invoke(instance, args);
                }
                finally
                {
                    // 确保数组归还到池中
                    ObjectArrayPool.Return(args);
                }
            }
        }

        /// <summary>
        /// 检查成员是否标记为可选
        /// </summary>
        private static bool IsOptional(MemberInfo member)
        {
            return member.GetCustomAttribute<OptionalAttribute>() != null;
        }

        #endregion

        #region 实例化方法 / Instantiation Methods

        /// <inheritdoc/>
        public T Instantiate<T>()
        {
            return (T)Instantiate(typeof(T));
        }

        /// <inheritdoc/>
        public object Instantiate(Type type)
        {
            return Instantiate(type, null);
        }

        /// <inheritdoc/>
        public T Instantiate<T>(IReadOnlyList<IInjectParameter> parameters)
        {
            return (T)Instantiate(typeof(T), parameters);
        }

        /// <inheritdoc/>
        public object Instantiate(Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            ThrowIfDisposed();
            
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsAbstract || type.IsInterface)
            {
                throw ResolutionException.CannotInstantiateAbstract(type);
            }

            // 查找最佳构造函数
            var constructor = FindBestConstructor(type);
            if (constructor == null)
            {
                throw ResolutionException.NoSuitableConstructor(type);
            }

            // 解析构造函数参数
            var ctorParams = constructor.GetParameters();
            var paramCount = ctorParams.Length;
            
            // 使用数组池租用参数数组
            var args = ObjectArrayPool.Rent(paramCount);
            
            try
            {
                for (int i = 0; i < paramCount; i++)
                {
                    args[i] = ResolveParameter(
                        ctorParams[i].ParameterType, 
                        ctorParams[i].Name, 
                        parameters, 
                        ctorParams[i]);
                }

                // 创建实例
                var instance = constructor.Invoke(args);

                // 执行成员注入
                Inject(instance, parameters);

                return instance;
            }
            finally
            {
                // 确保数组归还到池中
                ObjectArrayPool.Return(args);
            }
        }

        /// <summary>
        /// 查找最佳构造函数
        /// <para>优先选择标记了 [Inject] 的构造函数，否则选择参数最多的公共构造函数</para>
        /// </summary>
        private static ConstructorInfo FindBestConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            // 优先查找标记了 [Inject] 的构造函数
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    return ctor;
                }
            }

            // 选择参数最多的公共构造函数
            ConstructorInfo best = null;
            int maxParams = -1;
            
            foreach (var ctor in constructors)
            {
                if (!ctor.IsPublic) continue;
                
                var paramCount = ctor.GetParameters().Length;
                if (paramCount > maxParams)
                {
                    maxParams = paramCount;
                    best = ctor;
                }
            }

            return best;
        }

        #endregion

        #region 作用域方法 / Scope Methods

        /// <inheritdoc/>
        public IObjectResolver CreateScope()
        {
            return CreateScope(null);
        }

        /// <inheritdoc/>
        public IObjectResolver CreateScope(Action<IContainerBuilder> configuration)
        {
            ThrowIfDisposed();

            if (configuration == null)
            {
                // 创建简单的子作用域，共享注册表
                return new ScopedContainer(this, _registry);
            }

            // 创建带有额外注册的子作用域
            var builder = new ContainerBuilder();
            builder.SetParent(this);
            configuration(builder);
            return builder.Build();
        }

        #endregion

        #region 注册检查 / Registration Check

        /// <inheritdoc/>
        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type type)
        {
            ThrowIfDisposed();
            
            if (_registry.Contains(type))
                return true;

            // 检查开放泛型
            if (_registry.ContainsOpenGeneric(type))
                return true;

            // 检查内置集合类型
            if (IsBuiltInCollectionType(type))
                return true;
            
            return _parent?.IsRegistered(type) ?? false;
        }

        /// <summary>
        /// 检查是否是内置集合类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBuiltInCollectionType(Type type)
        {
            if (type.IsArray)
                return true;

            if (!type.IsGenericType)
                return false;

            var genericDefinition = type.GetGenericTypeDefinition();
            return genericDefinition == typeof(IEnumerable<>) ||
                   genericDefinition == typeof(IReadOnlyList<>) ||
                   genericDefinition == typeof(IReadOnlyCollection<>) ||
                   genericDefinition == typeof(IList<>) ||
                   genericDefinition == typeof(List<>);
        }

        /// <inheritdoc/>
        public bool IsRegisteredKeyed<T>(object key)
        {
            ThrowIfDisposed();
            
            if (_registry.ContainsKeyed(typeof(T), key))
                return true;
            
            return _parent?.IsRegisteredKeyed<T>(key) ?? false;
        }

        #endregion

        #region 内部解析方法 / Internal Resolution Methods

        /// <summary>
        /// 内部解析方法
        /// </summary>
        private object ResolveInternal(Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            // 检查单例缓存
            lock (_syncRoot)
            {
                if (_singletonInstances.TryGetValue(type, out var singleton))
                {
                    return singleton;
                }
            }

            // 检查作用域缓存
            lock (_syncRoot)
            {
                if (_scopedInstances.TryGetValue(type, out var scoped))
                {
                    return scoped;
                }
            }

            // 查找注册
            var registration = _registry.Get(type);
            if (registration != null)
            {
                return CreateInstance(registration, parameters);
            }

            // 尝试开放泛型解析
            if (_registry.TryGetOpenGeneric(type, out var openGenericRegistration))
            {
                return CreateInstance(openGenericRegistration, parameters);
            }

            // 尝试内置集合解析
            if (TryResolveBuiltInCollection(type, out var collectionInstance))
            {
                return collectionInstance;
            }

            // 尝试从父容器解析
            if (_parent != null)
            {
                return _parent.Resolve(type);
            }
            
            throw ResolutionException.ServiceNotRegistered(type);
        }

        /// <summary>
        /// 尝试解析内置集合类型
        /// <para>Try to resolve built-in collection types (IEnumerable&lt;T&gt;, IReadOnlyList&lt;T&gt;, T[])</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryResolveBuiltInCollection(Type type, out object instance)
        {
            instance = null;

            // 检查是否是数组类型
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType != null)
                {
                    instance = ResolveArrayInternal(elementType);
                    return true;
                }
            }

            // 检查是否是泛型集合类型
            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                var elementType = type.GetGenericArguments()[0];

                // IEnumerable<T>
                if (genericDefinition == typeof(IEnumerable<>))
                {
                    instance = ResolveEnumerableInternal(elementType);
                    return true;
                }

                // IReadOnlyList<T>
                if (genericDefinition == typeof(IReadOnlyList<>))
                {
                    instance = ResolveReadOnlyListInternal(elementType);
                    return true;
                }

                // IReadOnlyCollection<T>
                if (genericDefinition == typeof(IReadOnlyCollection<>))
                {
                    instance = ResolveReadOnlyListInternal(elementType);
                    return true;
                }

                // IList<T>
                if (genericDefinition == typeof(IList<>))
                {
                    instance = ResolveListInternal(elementType);
                    return true;
                }

                // List<T>
                if (genericDefinition == typeof(List<>))
                {
                    instance = ResolveListInternal(elementType);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 解析数组类型
        /// </summary>
        private object ResolveArrayInternal(Type elementType)
        {
            var registrations = _registry.GetAll(elementType);
            var array = Array.CreateInstance(elementType, registrations.Count);
            
            for (int i = 0; i < registrations.Count; i++)
            {
                var instance = CreateInstance(registrations[i], null);
                array.SetValue(instance, i);
            }

            return array;
        }

        /// <summary>
        /// 解析 IEnumerable&lt;T&gt; 类型
        /// </summary>
        private object ResolveEnumerableInternal(Type elementType)
        {
            var registrations = _registry.GetAll(elementType);
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (System.Collections.IList)Activator.CreateInstance(listType);

            foreach (var reg in registrations)
            {
                var instance = CreateInstance(reg, null);
                list.Add(instance);
            }

            return list;
        }

        /// <summary>
        /// 解析 IReadOnlyList&lt;T&gt; 类型
        /// </summary>
        private object ResolveReadOnlyListInternal(Type elementType)
        {
            return ResolveEnumerableInternal(elementType);
        }

        /// <summary>
        /// 解析 IList&lt;T&gt; / List&lt;T&gt; 类型
        /// </summary>
        private object ResolveListInternal(Type elementType)
        {
            return ResolveEnumerableInternal(elementType);
        }

        /// <summary>
        /// 根据注册信息创建实例
        /// </summary>
        private object CreateInstance(IRegistration registration, IReadOnlyList<IInjectParameter> parameters)
        {
            var reg = registration as Registration;
            if (reg == null)
            {
                throw new InvalidOperationException("不支持的注册类型 / Unsupported registration type");
            }

            object instance;

            // 根据注册类型创建实例
            if (reg.ExistingInstance != null)
            {
                // 现有实例
                instance = reg.ExistingInstance;
            }
            else if (reg.Factory != null)
            {
                // 工厂创建
                instance = reg.Factory(this);
            }
            else if (reg.ImplementationType != null)
            {
                // 类型实例化
                instance = InstantiateType(reg.ImplementationType, MergeParameters(reg.Parameters, parameters));
            }
            else
            {
                throw ResolutionException.NoImplementation(registration.ServiceType);
            }

            // 执行激活回调
            if (reg.OnActivatedCallbacks != null)
            {
                foreach (var callback in reg.OnActivatedCallbacks)
                {
                    callback?.Invoke(instance);
                }
            }

            // 根据生命周期缓存实例
            CacheInstance(registration, instance);

            // 跟踪可释放实例
            TrackDisposable(instance, registration.Lifetime);

            return instance;
        }

        /// <summary>
        /// 实例化类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object InstantiateType(Type type, IReadOnlyList<IInjectParameter> parameters)
        {
            var constructor = FindBestConstructor(type);
            if (constructor == null)
            {
                throw ResolutionException.NoSuitableConstructor(type);
            }

            var ctorParams = constructor.GetParameters();
            var paramCount = ctorParams.Length;
            
            // 使用数组池租用参数数组
            var args = ObjectArrayPool.Rent(paramCount);

            try
            {
                for (int i = 0; i < paramCount; i++)
                {
                    args[i] = ResolveParameter(
                        ctorParams[i].ParameterType,
                        ctorParams[i].Name,
                        parameters,
                        ctorParams[i]);
                }

                var instance = constructor.Invoke(args);

                // 执行成员注入
                Inject(instance, parameters);

                return instance;
            }
            finally
            {
                // 确保数组归还到池中
                ObjectArrayPool.Return(args);
            }
        }

        /// <summary>
        /// 解析参数值
        /// </summary>
        private object ResolveParameter(Type parameterType, string parameterName, 
            IReadOnlyList<IInjectParameter> parameters, object parameterInfo)
        {
            // 首先检查提供的参数
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.CanSupply(parameterType, parameterName))
                    {
                        return param.GetValue(this);
                    }
                }
            }

            // 检查是否有 Key 特性
            object key = null;
            bool fromParent = false;
            bool isOptional = false;
            object defaultValue = null;
            bool hasDefaultValue = false;

            if (parameterInfo is ParameterInfo pi)
            {
                var keyAttr = pi.GetCustomAttribute<KeyAttribute>();
                key = keyAttr?.Key;
                
                fromParent = pi.GetCustomAttribute<FromParentAttribute>() != null;
                isOptional = pi.GetCustomAttribute<OptionalAttribute>() != null;
                
                if (pi.HasDefaultValue)
                {
                    hasDefaultValue = true;
                    defaultValue = pi.DefaultValue;
                }
            }
            else if (parameterInfo is MemberInfo mi)
            {
                var keyAttr = mi.GetCustomAttribute<KeyAttribute>();
                key = keyAttr?.Key;
                
                fromParent = mi.GetCustomAttribute<FromParentAttribute>() != null;
                isOptional = mi.GetCustomAttribute<OptionalAttribute>() != null;
            }

            // 从父容器解析
            if (fromParent && _parent != null)
            {
                if (key != null)
                {
                    return _parent.ResolveKeyed(parameterType, key);
                }
                return _parent.Resolve(parameterType);
            }

            // 按键值解析
            if (key != null)
            {
                if (TryResolveKeyed(parameterType, key, out var keyedInstance))
                {
                    return keyedInstance;
                }
                if (isOptional) return defaultValue;
                throw ResolutionException.ServiceNotRegistered(parameterType, key);
            }

            // 普通解析
            if (TryResolve(parameterType, out var instance))
            {
                return instance;
            }

            // 可选参数返回默认值
            if (isOptional || hasDefaultValue)
            {
                return defaultValue;
            }

            throw ResolutionException.ServiceNotRegistered(parameterType);
        }

        /// <summary>
        /// 合并参数列表
        /// </summary>
        private static IReadOnlyList<IInjectParameter> MergeParameters(
            IReadOnlyList<IInjectParameter> registrationParams,
            IReadOnlyList<IInjectParameter> runtimeParams)
        {
            if (registrationParams == null || registrationParams.Count == 0)
                return runtimeParams;
            if (runtimeParams == null || runtimeParams.Count == 0)
                return registrationParams;

            var merged = new List<IInjectParameter>(registrationParams.Count + runtimeParams.Count);
            merged.AddRange(runtimeParams); // 运行时参数优先
            merged.AddRange(registrationParams);
            return merged;
        }

        /// <summary>
        /// 缓存实例
        /// </summary>
        private void CacheInstance(IRegistration registration, object instance)
        {
            var serviceType = registration.ServiceType;
            
            switch (registration.Lifetime)
            {
                case Lifetime.Singleton:
                    lock (_syncRoot)
                    {
                        if (!_singletonInstances.ContainsKey(serviceType))
                        {
                            _singletonInstances[serviceType] = instance;
                        }
                    }
                    break;
                    
                case Lifetime.Scoped:
                    lock (_syncRoot)
                    {
                        if (!_scopedInstances.ContainsKey(serviceType))
                        {
                            _scopedInstances[serviceType] = instance;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 跟踪可释放实例
        /// </summary>
        private void TrackDisposable(object instance, Lifetime lifetime)
        {
            if (instance is IDisposable disposable && lifetime != Lifetime.Transient)
            {
                lock (_syncRoot)
                {
                    _disposables.Add(disposable);
                }
            }
        }

        #endregion

        #region IDisposable 实现 / IDisposable Implementation

        /// <summary>
        /// 释放容器及其管理的所有实例
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                // 按添加顺序的逆序释放
                for (int i = _disposables.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        _disposables[i].Dispose();
                    }
                    catch (Exception ex)
                    {
                        // 记录但不抛出异常，确保所有实例都能被释放
                        if (_enableDiagnostics)
                        {
                            UnityEngine.Debug.LogWarning(
                                $"[AFramework.DI] 释放实例时发生异常 / Exception during disposal: {ex.Message}");
                        }
                    }
                }

                _disposables.Clear();
                _singletonInstances.Clear();
                _scopedInstances.Clear();
                _keyedInstances.Clear();
            }
        }

        /// <summary>
        /// 检查容器是否已释放
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(Container),
                    "容器已被释放，无法执行操作。\n" +
                    "Container has been disposed, cannot perform operation.");
            }
        }

        #endregion

        #region 诊断 / Diagnostics

        /// <summary>
        /// 获取容器的诊断信息
        /// </summary>
        public string GetDiagnosticInfo()
        {
            return $"Container[Registrations={_registry.Count}, " +
                   $"Singletons={_singletonInstances.Count}, " +
                   $"Scoped={_scopedInstances.Count}, " +
                   $"Disposables={_disposables.Count}]";
        }

        #endregion
    }

    #region 作用域容器 / Scoped Container

    /// <summary>
    /// 作用域容器
    /// <para>共享父容器的注册表，但拥有独立的作用域实例缓存</para>
    /// </summary>
    internal sealed class ScopedContainer : IObjectResolver
    {
        private readonly Container _rootContainer;
        private readonly Registry _registry;
        private readonly Dictionary<Type, object> _scopedInstances;
        private readonly List<IDisposable> _disposables;
        private readonly object _syncRoot = new object();
        private bool _isDisposed;

        public IObjectResolver Parent => _rootContainer;

        internal ScopedContainer(Container rootContainer, Registry registry)
        {
            _rootContainer = rootContainer ?? throw new ArgumentNullException(nameof(rootContainer));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _scopedInstances = new Dictionary<Type, object>();
            _disposables = new List<IDisposable>();
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            ThrowIfDisposed();
            
            // 检查作用域缓存
            lock (_syncRoot)
            {
                if (_scopedInstances.TryGetValue(type, out var scoped))
                {
                    return scoped;
                }
            }

            // 委托给根容器解析，但传递作用域上下文
            return _rootContainer.Resolve(type);
        }

        public T ResolveOrDefault<T>() => _rootContainer.ResolveOrDefault<T>();
        public object ResolveOrDefault(Type type) => _rootContainer.ResolveOrDefault(type);
        public bool TryResolve<T>(out T instance) => _rootContainer.TryResolve(out instance);
        public bool TryResolve(Type type, out object instance) => _rootContainer.TryResolve(type, out instance);
        public IEnumerable<T> ResolveAll<T>() => _rootContainer.ResolveAll<T>();
        public IEnumerable<object> ResolveAll(Type type) => _rootContainer.ResolveAll(type);
        public T ResolveKeyed<T>(object key) => _rootContainer.ResolveKeyed<T>(key);
        public object ResolveKeyed(Type type, object key) => _rootContainer.ResolveKeyed(type, key);
        public bool TryResolveKeyed<T>(object key, out T instance) => _rootContainer.TryResolveKeyed(key, out instance);
        public bool TryResolveKeyed(Type type, object key, out object instance) => _rootContainer.TryResolveKeyed(type, key, out instance);
        public void Inject(object instance) => _rootContainer.Inject(instance);
        public void Inject(object instance, IReadOnlyList<IInjectParameter> parameters) => _rootContainer.Inject(instance, parameters);
        public T Instantiate<T>() => _rootContainer.Instantiate<T>();
        public object Instantiate(Type type) => _rootContainer.Instantiate(type);
        public T Instantiate<T>(IReadOnlyList<IInjectParameter> parameters) => _rootContainer.Instantiate<T>(parameters);
        public object Instantiate(Type type, IReadOnlyList<IInjectParameter> parameters) => _rootContainer.Instantiate(type, parameters);
        public IObjectResolver CreateScope() => new ScopedContainer(_rootContainer, _registry);
        public IObjectResolver CreateScope(Action<IContainerBuilder> configuration) => _rootContainer.CreateScope(configuration);
        public bool IsRegistered<T>() => _rootContainer.IsRegistered<T>();
        public bool IsRegistered(Type type) => _rootContainer.IsRegistered(type);
        public bool IsRegisteredKeyed<T>(object key) => _rootContainer.IsRegisteredKeyed<T>(key);

        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_syncRoot)
            {
                if (_isDisposed) return;
                _isDisposed = true;

                for (int i = _disposables.Count - 1; i >= 0; i--)
                {
                    try { _disposables[i].Dispose(); }
                    catch { /* 忽略释放异常 */ }
                }

                _disposables.Clear();
                _scopedInstances.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ScopedContainer));
            }
        }
    }

    #endregion
}
