// ==========================================================
// 文件名：CircularDependencyException.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 定义检测到循环依赖时抛出的异常

// ==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFramework.DI
{
    /// <summary>
    /// 循环依赖异常
    /// <para>当检测到服务之间存在循环依赖时抛出此异常</para>
    /// <para>Exception thrown when circular dependency is detected between services</para>
    /// </summary>
    /// <remarks>
    /// 循环依赖示例：
    /// <code>
    /// A → B → C → A  (A 依赖 B，B 依赖 C，C 依赖 A)
    /// </code>
    /// 
    /// 解决方案：
    /// <list type="bullet">
    /// <item>重新设计依赖关系，打破循环</item>
    /// <item>使用属性注入替代构造函数注入</item>
    /// <item>使用 Lazy&lt;T&gt; 延迟解析</item>
    /// <item>使用 Func&lt;T&gt; 工厂函数</item>
    /// <item>引入中间接口或服务</item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class CircularDependencyException : DIException
    {
        /// <summary>
        /// 获取循环依赖链中的类型列表
        /// <para>Get the list of types in the circular dependency chain</para>
        /// </summary>
        public IReadOnlyList<Type> DependencyChain { get; }

        /// <summary>
        /// 创建循环依赖异常实例
        /// </summary>
        public CircularDependencyException() : base()
        {
            DependencyChain = Array.Empty<Type>();
        }

        /// <summary>
        /// 创建带消息的循环依赖异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        public CircularDependencyException(string message) : base(message)
        {
            DependencyChain = Array.Empty<Type>();
        }

        /// <summary>
        /// 创建带依赖链的循环依赖异常实例
        /// </summary>
        /// <param name="dependencyChain">循环依赖链 / Circular dependency chain</param>
        public CircularDependencyException(IEnumerable<Type> dependencyChain)
            : base(FormatMessage(dependencyChain))
        {
            DependencyChain = dependencyChain?.ToList() ?? new List<Type>();
        }

        /// <summary>
        /// 创建带依赖链和自定义消息的循环依赖异常实例
        /// </summary>
        /// <param name="dependencyChain">循环依赖链 / Circular dependency chain</param>
        /// <param name="message">自定义消息 / Custom message</param>
        public CircularDependencyException(IEnumerable<Type> dependencyChain, string message)
            : base(message)
        {
            DependencyChain = dependencyChain?.ToList() ?? new List<Type>();
        }

        /// <summary>
        /// 创建带消息和内部异常的循环依赖异常实例
        /// </summary>
        /// <param name="message">异常消息 / Exception message</param>
        /// <param name="innerException">内部异常 / Inner exception</param>
        public CircularDependencyException(string message, Exception innerException)
            : base(message, innerException)
        {
            DependencyChain = Array.Empty<Type>();
        }

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建循环依赖异常
        /// <para>Create circular dependency exception</para>
        /// </summary>
        /// <param name="type">当前解析的类型 / Currently resolving type</param>
        /// <param name="resolutionStack">解析栈 / Resolution stack</param>
        /// <returns>循环依赖异常 / Circular dependency exception</returns>
        public static CircularDependencyException Create(Type type, IEnumerable<Type> resolutionStack)
        {
            var chain = new List<Type>(resolutionStack ?? Enumerable.Empty<Type>());
            if (type != null && !chain.Contains(type))
            {
                chain.Add(type);
            }
            // 添加起始类型形成闭环
            if (chain.Count > 0)
            {
                chain.Add(chain[0]);
            }
            return new CircularDependencyException(chain);
        }

        #endregion

        private static string FormatMessage(IEnumerable<Type> dependencyChain)
        {
            var chain = dependencyChain?.ToList() ?? new List<Type>();
            if (chain.Count == 0)
            {
                return "检测到循环依赖。\nCircular dependency detected.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("检测到循环依赖 / Circular dependency detected:");
            sb.AppendLine();
            
            // 构建依赖链可视化
            for (int i = 0; i < chain.Count; i++)
            {
                var typeName = chain[i]?.Name ?? "null";
                sb.Append($"  {typeName}");
                
                if (i < chain.Count - 1)
                {
                    sb.AppendLine(" →");
                }
            }
            
            // 如果链的首尾相同，显示循环
            if (chain.Count > 1 && chain[0] == chain[chain.Count - 1])
            {
                sb.AppendLine(" (循环 / cycle)");
            }
            else if (chain.Count > 0)
            {
                sb.AppendLine($" → {chain[0]?.Name ?? "null"} (循环 / cycle)");
            }

            sb.AppendLine();
            sb.AppendLine("解决方案 / Solutions:");
            sb.AppendLine("  1. 重新设计依赖关系，打破循环 / Redesign dependencies to break the cycle");
            sb.AppendLine("  2. 使用属性注入替代构造函数注入 / Use property injection instead of constructor injection");
            sb.AppendLine("  3. 使用 Lazy<T> 延迟解析 / Use Lazy<T> for lazy resolution");
            sb.AppendLine("  4. 使用 Func<T> 工厂函数 / Use Func<T> factory function");

            return sb.ToString();
        }

        /// <summary>
        /// 获取格式化的依赖链字符串
        /// <para>Get formatted dependency chain string</para>
        /// </summary>
        /// <returns>依赖链字符串 / Dependency chain string</returns>
        public string GetFormattedChain()
        {
            if (DependencyChain == null || DependencyChain.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(" → ", DependencyChain.Select(t => t?.Name ?? "null"));
        }
    }
}
