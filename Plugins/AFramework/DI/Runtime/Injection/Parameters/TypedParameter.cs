// ==========================================================
// 文件名：TypedParameter.cs
// 命名空间: AFramework.DI
// 依赖: System
// 功能: 实现按类型匹配的注入参数

// ==========================================================

using System;

namespace AFramework.DI
{
    /// <summary>
    /// 类型参数
    /// <para>按类型匹配的注入参数实现</para>
    /// <para>Typed parameter that matches by type</para>
    /// </summary>
    public sealed class TypedParameter : IInjectParameter
    {
        private readonly object _value;

        /// <inheritdoc/>
        public Type ParameterType { get; }

        /// <inheritdoc/>
        public string ParameterName => null;

        /// <summary>
        /// 创建类型参数实例
        /// </summary>
        /// <param name="type">参数类型 / Parameter type</param>
        /// <param name="value">参数值 / Parameter value</param>
        public TypedParameter(Type type, object value)
        {
            ParameterType = type ?? throw new ArgumentNullException(nameof(type));
            _value = value;
        }

        /// <inheritdoc/>
        public bool CanSupply(Type parameterType, string parameterName)
        {
            return ParameterType.IsAssignableFrom(parameterType) ||
                   parameterType.IsAssignableFrom(ParameterType);
        }

        /// <inheritdoc/>
        public object GetValue(IObjectResolver resolver) => _value;

        public override string ToString() => $"TypedParameter[{ParameterType.Name}={_value}]";
    }
}
