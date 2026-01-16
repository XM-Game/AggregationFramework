// ==========================================================
// 文件名：DependencyAnalyzer.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Reflection
// 功能: 依赖分析器，用于构建时检测循环依赖
// ==========================================================

using System;
using System.Collections.Generic;
using System.Reflection;

namespace AFramework.DI
{
    /// <summary>
    /// 依赖分析器
    /// <para>在容器构建时分析依赖关系，检测循环依赖</para>
    /// <para>Dependency analyzer for detecting circular dependencies at build time</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>使用深度优先搜索检测依赖图中的环</item>
    /// <item>提供清晰的依赖链错误信息</item>
    /// <item>支持通过工厂注册打破循环</item>
    /// </list>
    /// </remarks>
    internal static class DependencyAnalyzer
    {
        #region 公共方法 / Public Methods

        /// <summary>
        /// 分析注册表中的循环依赖
        /// <para>Analyze circular dependencies in the registry</para>
        /// </summary>
        /// <param name="registry">注册表 / Registry</param>
        /// <exception cref="CircularDependencyException">检测到循环依赖时抛出</exception>
        public static void AnalyzeCircularDependencies(Registry registry)
        {
            if (registry == null || registry.Count == 0)
                return;

            // 构建依赖图
            var dependencyGraph = BuildDependencyGraph(registry);

            // 检测循环
            var visited = new HashSet<Type>();
            var recursionStack = new HashSet<Type>();
            var path = new List<Type>();

            foreach (var type in dependencyGraph.Keys)
            {
                if (!visited.Contains(type))
                {
                    DetectCycle(type, dependencyGraph, visited, recursionStack, path);
                }
            }
        }

        /// <summary>
        /// 获取类型的依赖列表
        /// <para>Get the dependency list for a type</para>
        /// </summary>
        public static IReadOnlyList<Type> GetDependencies(Type type)
        {
            var dependencies = new List<Type>();

            // 分析构造函数参数
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            ConstructorInfo bestCtor = null;
            int maxParams = -1;

            // 优先查找标记了 [Inject] 的构造函数
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    bestCtor = ctor;
                    break;
                }
                if (ctor.IsPublic && ctor.GetParameters().Length > maxParams)
                {
                    maxParams = ctor.GetParameters().Length;
                    bestCtor = ctor;
                }
            }

            if (bestCtor != null)
            {
                foreach (var param in bestCtor.GetParameters())
                {
                    if (!IsSystemType(param.ParameterType))
                    {
                        dependencies.Add(param.ParameterType);
                    }
                }
            }

            // 分析字段注入
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() != null)
                {
                    if (!IsSystemType(field.FieldType))
                    {
                        dependencies.Add(field.FieldType);
                    }
                }
            }

            // 分析属性注入
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<InjectAttribute>() != null && property.CanWrite)
                {
                    if (!IsSystemType(property.PropertyType))
                    {
                        dependencies.Add(property.PropertyType);
                    }
                }
            }

            // 分析方法注入
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<InjectAttribute>() != null)
                {
                    foreach (var param in method.GetParameters())
                    {
                        if (!IsSystemType(param.ParameterType))
                        {
                            dependencies.Add(param.ParameterType);
                        }
                    }
                }
            }

            return dependencies;
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 构建依赖图
        /// </summary>
        private static Dictionary<Type, List<Type>> BuildDependencyGraph(Registry registry)
        {
            var graph = new Dictionary<Type, List<Type>>();

            foreach (var registration in registry.AllRegistrations)
            {
                // 跳过工厂注册和实例注册（它们不会产生循环依赖）
                var reg = registration as Registration;
                if (reg != null && (reg.Factory != null || reg.ExistingInstance != null))
                {
                    continue;
                }

                var implType = registration.ImplementationType;
                if (implType == null || implType.IsAbstract || implType.IsInterface)
                {
                    continue;
                }

                var dependencies = GetDependencies(implType);
                
                // 为每个服务类型建立依赖关系
                foreach (var serviceType in registration.ServiceTypes)
                {
                    if (!graph.ContainsKey(serviceType))
                    {
                        graph[serviceType] = new List<Type>();
                    }
                    
                    foreach (var dep in dependencies)
                    {
                        if (!graph[serviceType].Contains(dep))
                        {
                            graph[serviceType].Add(dep);
                        }
                    }
                }

                // 也为实现类型建立依赖关系
                if (!graph.ContainsKey(implType))
                {
                    graph[implType] = new List<Type>(dependencies);
                }
            }

            return graph;
        }

        /// <summary>
        /// 使用DFS检测循环
        /// </summary>
        private static void DetectCycle(
            Type current,
            Dictionary<Type, List<Type>> graph,
            HashSet<Type> visited,
            HashSet<Type> recursionStack,
            List<Type> path)
        {
            visited.Add(current);
            recursionStack.Add(current);
            path.Add(current);

            if (graph.TryGetValue(current, out var dependencies))
            {
                foreach (var dep in dependencies)
                {
                    if (!visited.Contains(dep))
                    {
                        DetectCycle(dep, graph, visited, recursionStack, path);
                    }
                    else if (recursionStack.Contains(dep))
                    {
                        // 找到循环，构建循环路径
                        var cycleStart = path.IndexOf(dep);
                        var cyclePath = new List<Type>();
                        for (int i = cycleStart; i < path.Count; i++)
                        {
                            cyclePath.Add(path[i]);
                        }
                        cyclePath.Add(dep); // 添加回到起点

                        throw CircularDependencyException.Create(dep, new HashSet<Type>(cyclePath));
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            recursionStack.Remove(current);
        }

        /// <summary>
        /// 检查是否为系统类型（不需要注入的类型）
        /// </summary>
        private static bool IsSystemType(Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                return true;

            if (type.Namespace != null && type.Namespace.StartsWith("System"))
                return true;

            // 检查是否为值类型（除了自定义结构体）
            if (type.IsValueType && !type.IsEnum)
            {
                // 允许自定义结构体被注入
                if (type.Namespace != null && !type.Namespace.StartsWith("System"))
                    return false;
                return true;
            }

            return false;
        }

        #endregion
    }
}
