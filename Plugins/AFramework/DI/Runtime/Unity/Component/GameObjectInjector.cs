// ==========================================================
// 文件名：GameObjectInjector.cs
// 命名空间: AFramework.DI
// 依赖: UnityEngine, System
// 功能: GameObject 注入器，向 GameObject 上的组件注入依赖
// ==========================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AFramework.DI
{
    /// <summary>
    /// GameObject 注入器
    /// <para>向 GameObject 及其子对象上的所有组件注入依赖</para>
    /// <para>Injects dependencies into all components on a GameObject and its children</para>
    /// </summary>
    /// <remarks>
    /// 注入规则：
    /// <list type="bullet">
    /// <item>遍历 GameObject 上的所有 MonoBehaviour</item>
    /// <item>查找标记了 [Inject] 特性的字段、属性和方法</item>
    /// <item>从容器解析依赖并注入</item>
    /// <item>可选择是否包含子对象</item>
    /// </list>
    /// </remarks>
    public static class GameObjectInjector
    {
        #region 缓存 / Cache

        // 类型注入信息缓存
        private static readonly Dictionary<Type, InjectionTarget[]> s_injectionCache 
            = new Dictionary<Type, InjectionTarget[]>();

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 向 GameObject 上的所有组件注入依赖
        /// <para>Inject dependencies into all components on a GameObject</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="gameObject">目标 GameObject / Target GameObject</param>
        /// <param name="includeChildren">是否包含子对象 / Whether to include children</param>
        public static void Inject(
            IObjectResolver resolver, 
            GameObject gameObject, 
            bool includeChildren = true)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            // 获取组件
            var components = includeChildren
                ? gameObject.GetComponentsInChildren<MonoBehaviour>(true)
                : gameObject.GetComponents<MonoBehaviour>();

            // 注入每个组件
            foreach (var component in components)
            {
                if (component == null)
                    continue;

                InjectComponent(resolver, component);
            }
        }

        /// <summary>
        /// 向单个组件注入依赖
        /// <para>Inject dependencies into a single component</para>
        /// </summary>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="component">目标组件 / Target component</param>
        public static void InjectComponent(IObjectResolver resolver, MonoBehaviour component)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            var type = component.GetType();
            var targets = GetInjectionTargets(type);

            foreach (var target in targets)
            {
                target.Inject(resolver, component);
            }
        }

        /// <summary>
        /// 检查类型是否需要注入
        /// <para>Check if a type requires injection</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>是否需要注入 / Whether injection is required</returns>
        public static bool RequiresInjection(Type type)
        {
            var targets = GetInjectionTargets(type);
            return targets.Length > 0;
        }

        /// <summary>
        /// 清除注入缓存
        /// <para>Clear the injection cache</para>
        /// </summary>
        public static void ClearCache()
        {
            s_injectionCache.Clear();
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 获取类型的注入目标
        /// </summary>
        private static InjectionTarget[] GetInjectionTargets(Type type)
        {
            if (s_injectionCache.TryGetValue(type, out var cached))
                return cached;

            var targets = new List<InjectionTarget>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // 查找字段
            foreach (var field in type.GetFields(bindingFlags))
            {
                var injectAttr = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttr != null)
                {
                    targets.Add(new FieldInjectionTarget(field));
                }
            }

            // 查找属性
            foreach (var property in type.GetProperties(bindingFlags))
            {
                var injectAttr = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttr != null && property.CanWrite)
                {
                    targets.Add(new PropertyInjectionTarget(property));
                }
            }

            // 查找方法
            var methods = new List<(MethodInfo Method, int Order)>();
            foreach (var method in type.GetMethods(bindingFlags))
            {
                var injectAttr = method.GetCustomAttribute<InjectAttribute>();
                if (injectAttr != null)
                {
                    var orderAttr = method.GetCustomAttribute<OrderAttribute>();
                    var order = orderAttr?.Order ?? 0;
                    methods.Add((method, order));
                }
            }

            // 按顺序排序方法
            methods.Sort((a, b) => a.Order.CompareTo(b.Order));
            foreach (var (method, _) in methods)
            {
                targets.Add(new MethodInjectionTarget(method));
            }

            var result = targets.ToArray();
            s_injectionCache[type] = result;
            return result;
        }

        #endregion

        #region 注入目标类 / Injection Target Classes

        /// <summary>
        /// 注入目标基类
        /// </summary>
        private abstract class InjectionTarget
        {
            public abstract void Inject(IObjectResolver resolver, object instance);

            protected object ResolveValue(IObjectResolver resolver, Type type, MemberInfo member)
            {
                // 检查特性
                var keyAttr = member.GetCustomAttribute<KeyAttribute>();
                var optionalAttr = member.GetCustomAttribute<OptionalAttribute>();
                var fromParentAttr = member.GetCustomAttribute<FromParentAttribute>();

                // 从父容器解析
                if (fromParentAttr != null && resolver.Parent != null)
                {
                    resolver = resolver.Parent;
                }

                // 按键值解析
                if (keyAttr != null)
                {
                    if (resolver.TryResolveKeyed(type, keyAttr.Key, out var keyedInstance))
                        return keyedInstance;
                    
                    if (optionalAttr != null)
                        return null;
                    
                    throw ResolutionException.ServiceNotRegistered(type, keyAttr.Key);
                }

                // 普通解析
                if (resolver.TryResolve(type, out var instance))
                    return instance;

                if (optionalAttr != null)
                    return null;

                throw ResolutionException.ServiceNotRegistered(type);
            }
        }

        /// <summary>
        /// 字段注入目标
        /// </summary>
        private sealed class FieldInjectionTarget : InjectionTarget
        {
            private readonly FieldInfo _field;

            public FieldInjectionTarget(FieldInfo field)
            {
                _field = field;
            }

            public override void Inject(IObjectResolver resolver, object instance)
            {
                var value = ResolveValue(resolver, _field.FieldType, _field);
                if (value != null)
                {
                    _field.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// 属性注入目标
        /// </summary>
        private sealed class PropertyInjectionTarget : InjectionTarget
        {
            private readonly PropertyInfo _property;

            public PropertyInjectionTarget(PropertyInfo property)
            {
                _property = property;
            }

            public override void Inject(IObjectResolver resolver, object instance)
            {
                var value = ResolveValue(resolver, _property.PropertyType, _property);
                if (value != null)
                {
                    _property.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// 方法注入目标
        /// </summary>
        private sealed class MethodInjectionTarget : InjectionTarget
        {
            private readonly MethodInfo _method;
            private readonly ParameterInfo[] _parameters;

            public MethodInjectionTarget(MethodInfo method)
            {
                _method = method;
                _parameters = method.GetParameters();
            }

            public override void Inject(IObjectResolver resolver, object instance)
            {
                var args = new object[_parameters.Length];
                
                for (int i = 0; i < _parameters.Length; i++)
                {
                    args[i] = ResolveParameter(resolver, _parameters[i]);
                }

                _method.Invoke(instance, args);
            }

            private object ResolveParameter(IObjectResolver resolver, ParameterInfo param)
            {
                var keyAttr = param.GetCustomAttribute<KeyAttribute>();
                var optionalAttr = param.GetCustomAttribute<OptionalAttribute>();
                var fromParentAttr = param.GetCustomAttribute<FromParentAttribute>();

                var targetResolver = resolver;
                if (fromParentAttr != null && resolver.Parent != null)
                {
                    targetResolver = resolver.Parent;
                }

                if (keyAttr != null)
                {
                    if (targetResolver.TryResolveKeyed(param.ParameterType, keyAttr.Key, out var keyedInstance))
                        return keyedInstance;
                    
                    if (optionalAttr != null || param.HasDefaultValue)
                        return param.HasDefaultValue ? param.DefaultValue : null;
                    
                    throw ResolutionException.ServiceNotRegistered(param.ParameterType, keyAttr.Key);
                }

                if (targetResolver.TryResolve(param.ParameterType, out var instance))
                    return instance;

                if (optionalAttr != null || param.HasDefaultValue)
                    return param.HasDefaultValue ? param.DefaultValue : null;

                throw ResolutionException.ServiceNotRegistered(param.ParameterType);
            }
        }

        #endregion
    }
}
