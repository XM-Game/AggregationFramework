// ==========================================================
// 文件名：IInjector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic
// 功能: 定义注入器接口，负责向对象注入依赖
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.DI
{
    /// <summary>
    /// 注入器接口
    /// <para>定义依赖注入的执行能力，包括构造函数注入、属性注入、字段注入和方法注入</para>
    /// <para>Injector interface that defines dependency injection execution capabilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责依赖注入的执行逻辑</item>
    /// <item>策略模式：不同注入方式可使用不同的注入器实现</item>
    /// <item>性能优化：通过缓存反射信息提升注入性能</item>
    /// </list>
    /// 
    /// 注入优先级（从高到低）：
    /// <list type="number">
    /// <item>构造函数注入：对象创建时执行</item>
    /// <item>字段注入：对象创建后执行</item>
    /// <item>属性注入：对象创建后执行</item>
    /// <item>方法注入：对象创建后执行</item>
    /// </list>
    /// </remarks>
    public interface IInjector
    {
        #region 实例创建 / Instance Creation

        /// <summary>
        /// 创建指定类型的实例并注入依赖
        /// <para>Create an instance of the specified type and inject dependencies</para>
        /// </summary>
        /// <param name="type">要创建的类型 / Type to create</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        /// <exception cref="ArgumentNullException">当 type 或 resolver 为 null 时抛出</exception>
        /// <exception cref="ResolutionException">当无法创建实例或注入失败时抛出</exception>
        object CreateInstance(Type type, IObjectResolver resolver);

        /// <summary>
        /// 创建指定类型的实例，使用额外参数并注入依赖
        /// <para>Create an instance with additional parameters and inject dependencies</para>
        /// </summary>
        /// <param name="type">要创建的类型 / Type to create</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外的注入参数 / Additional injection parameters</param>
        /// <returns>已注入依赖的实例 / Instance with injected dependencies</returns>
        object CreateInstance(Type type, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters);

        #endregion

        #region 依赖注入 / Dependency Injection

        /// <summary>
        /// 向现有实例注入依赖
        /// <para>Inject dependencies into an existing instance</para>
        /// </summary>
        /// <param name="instance">要注入的实例 / Instance to inject into</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <exception cref="ArgumentNullException">当 instance 或 resolver 为 null 时抛出</exception>
        /// <remarks>
        /// 此方法会处理实例上所有标记了 [Inject] 特性的成员：
        /// <list type="bullet">
        /// <item>字段注入</item>
        /// <item>属性注入</item>
        /// <item>方法注入</item>
        /// </list>
        /// 不包括构造函数注入（实例已创建）。
        /// </remarks>
        void Inject(object instance, IObjectResolver resolver);

        /// <summary>
        /// 向现有实例注入依赖，使用额外参数
        /// <para>Inject dependencies into an existing instance with additional parameters</para>
        /// </summary>
        /// <param name="instance">要注入的实例 / Instance to inject into</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外的注入参数 / Additional injection parameters</param>
        void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters);

        #endregion

        #region 注入信息查询 / Injection Information Query

        /// <summary>
        /// 获取指定类型的注入信息
        /// <para>Get injection information for the specified type</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>注入信息 / Injection information</returns>
        InjectionInfo GetInjectionInfo(Type type);

        /// <summary>
        /// 检查指定类型是否需要注入
        /// <para>Check if the specified type requires injection</para>
        /// </summary>
        /// <param name="type">目标类型 / Target type</param>
        /// <returns>是否需要注入 / Whether injection is required</returns>
        bool RequiresInjection(Type type);

        #endregion
    }

    /// <summary>
    /// 注入信息
    /// <para>描述一个类型的所有注入点信息</para>
    /// <para>Injection information that describes all injection points of a type</para>
    /// </summary>
    public sealed class InjectionInfo
    {
        /// <summary>
        /// 获取目标类型
        /// <para>Get the target type</para>
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// 获取构造函数注入信息
        /// <para>Get constructor injection information</para>
        /// </summary>
        public ConstructorInjectionInfo Constructor { get; }

        /// <summary>
        /// 获取字段注入信息列表
        /// <para>Get field injection information list</para>
        /// </summary>
        public IReadOnlyList<FieldInjectionInfo> Fields { get; }

        /// <summary>
        /// 获取属性注入信息列表
        /// <para>Get property injection information list</para>
        /// </summary>
        public IReadOnlyList<PropertyInjectionInfo> Properties { get; }

        /// <summary>
        /// 获取方法注入信息列表
        /// <para>Get method injection information list</para>
        /// </summary>
        public IReadOnlyList<MethodInjectionInfo> Methods { get; }

        /// <summary>
        /// 检查是否有任何注入点
        /// <para>Check if there are any injection points</para>
        /// </summary>
        public bool HasInjectionPoints => 
            Constructor != null || 
            Fields.Count > 0 || 
            Properties.Count > 0 || 
            Methods.Count > 0;

        /// <summary>
        /// 创建注入信息实例
        /// </summary>
        public InjectionInfo(
            Type targetType,
            ConstructorInjectionInfo constructor,
            IReadOnlyList<FieldInjectionInfo> fields,
            IReadOnlyList<PropertyInjectionInfo> properties,
            IReadOnlyList<MethodInjectionInfo> methods)
        {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            Constructor = constructor;
            Fields = fields ?? Array.Empty<FieldInjectionInfo>();
            Properties = properties ?? Array.Empty<PropertyInjectionInfo>();
            Methods = methods ?? Array.Empty<MethodInjectionInfo>();
        }
    }

    /// <summary>
    /// 构造函数注入信息
    /// </summary>
    public sealed class ConstructorInjectionInfo
    {
        /// <summary>
        /// 获取构造函数信息
        /// </summary>
        public System.Reflection.ConstructorInfo ConstructorInfo { get; }

        /// <summary>
        /// 获取参数类型列表
        /// </summary>
        public IReadOnlyList<ParameterInjectionInfo> Parameters { get; }

        public ConstructorInjectionInfo(
            System.Reflection.ConstructorInfo constructorInfo,
            IReadOnlyList<ParameterInjectionInfo> parameters)
        {
            ConstructorInfo = constructorInfo ?? throw new ArgumentNullException(nameof(constructorInfo));
            Parameters = parameters ?? Array.Empty<ParameterInjectionInfo>();
        }
    }

    /// <summary>
    /// 参数注入信息
    /// </summary>
    public sealed class ParameterInjectionInfo
    {
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// 获取是否为可选参数
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        /// 获取键值（如果有）
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 获取是否从父容器解析
        /// </summary>
        public bool FromParent { get; }

        /// <summary>
        /// 获取默认值（如果有）
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// 获取是否有默认值
        /// </summary>
        public bool HasDefaultValue { get; }

        public ParameterInjectionInfo(
            string name,
            Type parameterType,
            bool isOptional = false,
            object key = null,
            bool fromParent = false,
            object defaultValue = null,
            bool hasDefaultValue = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            IsOptional = isOptional;
            Key = key;
            FromParent = fromParent;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
        }
    }

    /// <summary>
    /// 字段注入信息
    /// </summary>
    public sealed class FieldInjectionInfo
    {
        /// <summary>
        /// 获取字段信息
        /// </summary>
        public System.Reflection.FieldInfo FieldInfo { get; }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        public Type FieldType { get; }

        /// <summary>
        /// 获取是否为可选
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        /// 获取键值（如果有）
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 获取是否从父容器解析
        /// </summary>
        public bool FromParent { get; }

        public FieldInjectionInfo(
            System.Reflection.FieldInfo fieldInfo,
            bool isOptional = false,
            object key = null,
            bool fromParent = false)
        {
            FieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            FieldType = fieldInfo.FieldType;
            IsOptional = isOptional;
            Key = key;
            FromParent = fromParent;
        }
    }

    /// <summary>
    /// 属性注入信息
    /// </summary>
    public sealed class PropertyInjectionInfo
    {
        /// <summary>
        /// 获取属性信息
        /// </summary>
        public System.Reflection.PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// 获取是否为可选
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        /// 获取键值（如果有）
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 获取是否从父容器解析
        /// </summary>
        public bool FromParent { get; }

        public PropertyInjectionInfo(
            System.Reflection.PropertyInfo propertyInfo,
            bool isOptional = false,
            object key = null,
            bool fromParent = false)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            PropertyType = propertyInfo.PropertyType;
            IsOptional = isOptional;
            Key = key;
            FromParent = fromParent;
        }
    }

    /// <summary>
    /// 方法注入信息
    /// </summary>
    public sealed class MethodInjectionInfo
    {
        /// <summary>
        /// 获取方法信息
        /// </summary>
        public System.Reflection.MethodInfo MethodInfo { get; }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        public IReadOnlyList<ParameterInjectionInfo> Parameters { get; }

        public MethodInjectionInfo(
            System.Reflection.MethodInfo methodInfo,
            IReadOnlyList<ParameterInjectionInfo> parameters)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Parameters = parameters ?? Array.Empty<ParameterInjectionInfo>();
        }
    }
}
