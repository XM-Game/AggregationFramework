// ==========================================================
// 文件名：NamedParameter.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 实现按名称匹配的注入参数

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 命名参数
    /// <para>按名称匹配的注入参数实现</para>
    /// <para>Named parameter that matches by name</para>
    /// </summary>
    public sealed class NamedParameter : IInjectParameter
    {
        private readonly object _value;

        /// <inheritdoc/>
        public Type ParameterType => _value?.GetType();

        /// <inheritdoc/>
        public string ParameterName { get; }

        /// <summary>
        /// 创建命名参数实例
        /// </summary>
        /// <param name="name">参数名称 / Parameter name</param>
        /// <param name="value">参数值 / Parameter value</param>
        public NamedParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            
            ParameterName = name;
            _value = value;
        }

        /// <inheritdoc/>
        public bool CanSupply(Type parameterType, string parameterName)
        {
            return string.Equals(ParameterName, parameterName, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public object GetValue(IObjectResolver resolver) => _value;

        public override string ToString() => $"NamedParameter[{ParameterName}={_value}]";
    }
}
