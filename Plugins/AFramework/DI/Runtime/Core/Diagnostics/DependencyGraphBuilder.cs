// ==========================================================
// 文件名：DependencyGraphBuilder.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Text
// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AFramework.DI
{
    /// <summary>
    /// 依赖图构建器
    /// <para>构建和分析服务之间的依赖关系图</para>
    /// </summary>
    public sealed class DependencyGraphBuilder
    {
        #region 字段

        private readonly Dictionary<Type, DependencyNode> _nodes = new();
        private readonly List<DependencyEdge> _edges = new();

        #endregion

        #region 属性

        /// <summary>
        /// 所有节点
        /// </summary>
        public IReadOnlyDictionary<Type, DependencyNode> Nodes => _nodes;

        /// <summary>
        /// 所有边
        /// </summary>
        public IReadOnlyList<DependencyEdge> Edges => _edges;

        #endregion

        #region 构建方法

        /// <summary>
        /// 从注册信息构建依赖图
        /// </summary>
        /// <param name="registrations">注册信息集合</param>
        public void Build(IEnumerable<IRegistration> registrations)
        {
            _nodes.Clear();
            _edges.Clear();

            // 创建节点
            foreach (var reg in registrations)
            {
                var node = GetOrCreateNode(reg.ServiceType);
                node.Lifetime = reg.Lifetime;
                node.ImplementationType = reg.ImplementationType;
                node.IsRegistered = true;
            }

            // 分析依赖关系
            foreach (var reg in registrations)
            {
                if (reg.ImplementationType == null) continue;
                AnalyzeDependencies(reg.ServiceType, reg.ImplementationType);
            }
        }

        /// <summary>
        /// 添加单个注册
        /// </summary>
        public void AddRegistration(IRegistration registration)
        {
            var node = GetOrCreateNode(registration.ServiceType);
            node.Lifetime = registration.Lifetime;
            node.ImplementationType = registration.ImplementationType;
            node.IsRegistered = true;

            if (registration.ImplementationType != null)
            {
                AnalyzeDependencies(registration.ServiceType, registration.ImplementationType);
            }
        }

        #endregion

        #region 分析方法

        /// <summary>
        /// 检测循环依赖
        /// </summary>
        /// <returns>循环依赖路径列表</returns>
        public List<List<Type>> DetectCircularDependencies()
        {
            var cycles = new List<List<Type>>();
            var visited = new HashSet<Type>();
            var recursionStack = new HashSet<Type>();
            var path = new List<Type>();

            foreach (var node in _nodes.Values)
            {
                if (!visited.Contains(node.ServiceType))
                {
                    DetectCyclesDFS(node.ServiceType, visited, recursionStack, path, cycles);
                }
            }

            return cycles;
        }

        /// <summary>
        /// 获取服务的所有依赖（递归）
        /// </summary>
        public HashSet<Type> GetAllDependencies(Type serviceType)
        {
            var result = new HashSet<Type>();
            var visited = new HashSet<Type>();
            CollectDependencies(serviceType, result, visited);
            return result;
        }

        /// <summary>
        /// 获取依赖于指定服务的所有服务
        /// </summary>
        public HashSet<Type> GetDependents(Type serviceType)
        {
            var result = new HashSet<Type>();
            foreach (var edge in _edges)
            {
                if (edge.Dependency == serviceType)
                {
                    result.Add(edge.Dependent);
                }
            }
            return result;
        }

        /// <summary>
        /// 检测生命周期问题
        /// </summary>
        public List<LifetimeIssue> DetectLifetimeIssues()
        {
            var issues = new List<LifetimeIssue>();

            foreach (var edge in _edges)
            {
                if (!_nodes.TryGetValue(edge.Dependent, out var dependentNode)) continue;
                if (!_nodes.TryGetValue(edge.Dependency, out var dependencyNode)) continue;

                // 检测俘获依赖：单例依赖作用域
                if (dependentNode.Lifetime == Lifetime.Singleton &&
                    dependencyNode.Lifetime == Lifetime.Scoped)
                {
                    issues.Add(new LifetimeIssue
                    {
                        Severity = IssueSeverity.Warning,
                        IssueType = LifetimeIssueType.CaptiveDependency,
                        ServiceType = edge.Dependent,
                        DependencyType = edge.Dependency,
                        Message = $"单例 '{edge.Dependent.Name}' 依赖作用域服务 '{edge.Dependency.Name}'",
                        Suggestion = "调整生命周期或使用工厂模式"
                    });
                }

                // 检测缺失依赖
                if (!dependencyNode.IsRegistered)
                {
                    issues.Add(new LifetimeIssue
                    {
                        Severity = IssueSeverity.Error,
                        IssueType = LifetimeIssueType.MissingDependency,
                        ServiceType = edge.Dependent,
                        DependencyType = edge.Dependency,
                        Message = $"服务 '{edge.Dependent.Name}' 依赖未注册的服务 '{edge.Dependency.Name}'",
                        Suggestion = $"注册 '{edge.Dependency.Name}' 服务"
                    });
                }
            }

            // 检测循环依赖
            var cycles = DetectCircularDependencies();
            foreach (var cycle in cycles)
            {
                issues.Add(new LifetimeIssue
                {
                    Severity = IssueSeverity.Error,
                    IssueType = LifetimeIssueType.CircularDependency,
                    Message = $"循环依赖: {string.Join(" -> ", cycle.Select(t => t.Name))}",
                    Suggestion = "使用 Lazy<T> 或工厂模式打破循环"
                });
            }

            return issues;
        }

        #endregion

        #region 导出方法

        /// <summary>
        /// 导出为 DOT 格式（可用于 Graphviz 可视化）
        /// </summary>
        public string ExportToDot()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph DependencyGraph {");
            sb.AppendLine("  rankdir=TB;");
            sb.AppendLine("  node [shape=box];");

            // 节点
            foreach (var node in _nodes.Values)
            {
                var color = node.Lifetime switch
                {
                    Lifetime.Singleton => "lightblue",
                    Lifetime.Scoped => "lightgreen",
                    Lifetime.Transient => "lightyellow",
                    _ => "white"
                };
                var style = node.IsRegistered ? "filled" : "filled,dashed";
                sb.AppendLine($"  \"{node.ServiceType.Name}\" [style=\"{style}\", fillcolor=\"{color}\"];");
            }

            // 边
            foreach (var edge in _edges)
            {
                sb.AppendLine($"  \"{edge.Dependent.Name}\" -> \"{edge.Dependency.Name}\";");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 导出为文本报告
        /// </summary>
        public string ExportToText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== 依赖关系图 ===");
            sb.AppendLine();

            foreach (var node in _nodes.Values.OrderBy(n => n.ServiceType.Name))
            {
                var status = node.IsRegistered ? "✓" : "✗";
                sb.AppendLine($"{status} {node.ServiceType.Name} [{node.Lifetime}]");

                var deps = _edges.Where(e => e.Dependent == node.ServiceType).ToList();
                foreach (var dep in deps)
                {
                    sb.AppendLine($"    └─> {dep.Dependency.Name}");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region 私有方法

        private DependencyNode GetOrCreateNode(Type type)
        {
            if (!_nodes.TryGetValue(type, out var node))
            {
                node = new DependencyNode { ServiceType = type };
                _nodes[type] = node;
            }
            return node;
        }

        private void AnalyzeDependencies(Type serviceType, Type implementationType)
        {
            // 构造函数依赖
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var ctor in constructors)
            {
                foreach (var param in ctor.GetParameters())
                {
                    AddDependency(serviceType, param.ParameterType);
                }
            }

            // 字段和属性注入
            var members = implementationType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var member in members)
            {
                if (member.GetCustomAttribute<InjectAttribute>() == null) continue;

                Type memberType = member switch
                {
                    FieldInfo field => field.FieldType,
                    PropertyInfo prop => prop.PropertyType,
                    _ => null
                };

                if (memberType != null)
                {
                    AddDependency(serviceType, memberType);
                }
            }
        }

        private void AddDependency(Type dependent, Type dependency)
        {
            // 跳过基础类型
            if (dependency.IsPrimitive || dependency == typeof(string)) return;

            GetOrCreateNode(dependency);

            var edge = new DependencyEdge
            {
                Dependent = dependent,
                Dependency = dependency
            };

            if (!_edges.Any(e => e.Dependent == dependent && e.Dependency == dependency))
            {
                _edges.Add(edge);
            }
        }

        private void DetectCyclesDFS(Type current, HashSet<Type> visited, HashSet<Type> recursionStack,
            List<Type> path, List<List<Type>> cycles)
        {
            visited.Add(current);
            recursionStack.Add(current);
            path.Add(current);

            var dependencies = _edges.Where(e => e.Dependent == current).Select(e => e.Dependency);
            foreach (var dep in dependencies)
            {
                if (!visited.Contains(dep))
                {
                    DetectCyclesDFS(dep, visited, recursionStack, path, cycles);
                }
                else if (recursionStack.Contains(dep))
                {
                    // 找到循环
                    var cycleStart = path.IndexOf(dep);
                    var cycle = path.Skip(cycleStart).ToList();
                    cycle.Add(dep); // 闭合循环
                    cycles.Add(cycle);
                }
            }

            path.RemoveAt(path.Count - 1);
            recursionStack.Remove(current);
        }

        private void CollectDependencies(Type type, HashSet<Type> result, HashSet<Type> visited)
        {
            if (visited.Contains(type)) return;
            visited.Add(type);

            var dependencies = _edges.Where(e => e.Dependent == type).Select(e => e.Dependency);
            foreach (var dep in dependencies)
            {
                result.Add(dep);
                CollectDependencies(dep, result, visited);
            }
        }

        #endregion
    }

    /// <summary>
    /// 依赖图节点
    /// </summary>
    public sealed class DependencyNode
    {
        /// <summary>服务类型</summary>
        public Type ServiceType { get; set; }

        /// <summary>实现类型</summary>
        public Type ImplementationType { get; set; }

        /// <summary>生命周期</summary>
        public Lifetime Lifetime { get; set; }

        /// <summary>是否已注册</summary>
        public bool IsRegistered { get; set; }
    }

    /// <summary>
    /// 依赖图边
    /// </summary>
    public sealed class DependencyEdge
    {
        /// <summary>依赖方（需要依赖的服务）</summary>
        public Type Dependent { get; set; }

        /// <summary>被依赖方（被依赖的服务）</summary>
        public Type Dependency { get; set; }
    }
}
