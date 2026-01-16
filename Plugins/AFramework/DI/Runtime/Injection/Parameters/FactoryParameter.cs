// ==========================================================
// 文件名：FactoryParameter.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 实现工厂函数注入参数

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 工厂参数（泛型版本）
    /// <para>使用工厂函数提供参数值的注入参数实现</para>
    /// <para>Factory parameter that provides value using factory function</para>
    /// </summary>
    public sealed class FactoryParameter<T> : IInjectParameter
    {
        private readonly Func<IObjectResolver, T> _factory;

        /// <inheritdoc/>
        public Type ParameterType => typeof(T);

        /// <inheritdoc/>
        public string ParameterName => null;

        /// <summary>
        /// 创建工厂参数实例
        /// </summary>
        /// <param name="factory">工厂函数 / Factory function</param>
        public FactoryParameter(Func<IObjectResolver, T> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc/>
        public bool CanSupply(Type parameterType, string parameterName)
        {
            return typeof(T).IsAssignableFrom(parameterType) ||
                   parameterType.IsAssignableFrom(typeof(T));
        }

        /// <inheritdoc/>
        public object GetValue(IObjectResolver resolver) => _factory(resolver);

        public override string ToString() => $"FactoryParameter[{typeof(T).Name}]";
    }

    /// <summary>
    /// 命名工厂参数
    /// <para>按名称匹配并使用工厂函数提供参数值</para>
    /// <para>Named factory parameter that matches by name and provides value using factory</para>
    /// </summary>
    public sealed class NamedFactoryParameter : IInjectParameter
    {
        private readonly Func<IObjectResolver, object> _factory;

        /// <inheritdoc/>
        public Type ParameterType => null;

        /// <inheritdoc/>
        public string ParameterName { get; }

        /// <summary>
        /// 创建命名工厂参数实例
        /// </summary>
        /// <param name="name">参数名称 / Parameter name</param>
        /// <param name="factory">工厂函数 / Factory function</param>
        public NamedFactoryParameter(string name, Func<IObjectResolver, object> factory)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            
            ParameterName = name;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc/>
        public bool CanSupply(Type parameterType, string parameterName)
        {
            return string.Equals(ParameterName, parameterName, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public object GetValue(IObjectResolver resolver) => _factory(resolver);

        public override string ToString() => $"NamedFactoryParameter[{ParameterName}]";
    }
}
