// ==========================================================
// 文件名：ConstructorInjector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现构造函数注入逻辑
// 优化: 使用数组池复用参数数组，减少GC压力；热路径内联优化
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 构造函数注入器
    /// <para>负责通过构造函数创建对象实例并注入依赖</para>
    /// <para>Constructor injector that creates object instances via constructor with dependency injection</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>使用 ObjectArrayPool 复用参数数组，减少GC压力</item>
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// <item>使用 try-finally 确保数组正确归还</item>
    /// </list>
    /// </remarks>
    internal sealed class ConstructorInjector
    {
        #region 单例 / Singleton

        private static readonly Lazy<ConstructorInjector> _instance = 
            new Lazy<ConstructorInjector>(() => new ConstructorInjector());

        /// <summary>
        /// 获取注入器实例
        /// <para>Get the injector instance</para>
        /// </summary>
        public static ConstructorInjector Instance => _instance.Value;

        #endregion

        #region 构造函数 / Constructor

        private ConstructorInjector()
        {
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 创建实例
        /// <para>Create an instance using constructor injection</para>
        /// </summary>
        /// <param name="constructorInfo">构造函数注入信息 / Constructor injection info</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外参数 / Additional parameters</param>
        /// <returns>创建的实例 / Created instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateInstance(
            ConstructorInjectionInfo constructorInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            if (constructorInfo == null)
                throw new ArgumentNullException(nameof(constructorInfo));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var ctorParams = constructorInfo.Parameters;
            var paramCount = ctorParams.Count;
            
            // 使用数组池租用参数数组
            var args = ObjectArrayPool.Rent(paramCount);
            
            try
            {
                // 解析所有参数
                for (int i = 0; i < paramCount; i++)
                {
                    args[i] = ResolveParameter(ctorParams[i], resolver, parameters);
                }

                // 调用构造函数创建实例
                return constructorInfo.ConstructorInfo.Invoke(args);
            }
            catch (Exception ex) when (!(ex is ResolutionException))
            {
                throw new ResolutionException(
                    $"创建类型 {constructorInfo.ConstructorInfo.DeclaringType?.Name} 的实例时发生错误。\n" +
                    $"Error creating instance of type {constructorInfo.ConstructorInfo.DeclaringType?.Name}.",
                    ex);
            }
            finally
            {
                // 确保数组归还到池中
                ObjectArrayPool.Return(args);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 解析参数值
        /// </summary>
        private object ResolveParameter(
            ParameterInjectionInfo paramInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            // 首先检查提供的参数
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.CanSupply(paramInfo.ParameterType, paramInfo.Name))
                    {
                        return param.GetValue(resolver);
                    }
                }
            }

            // 从父容器解析
            if (paramInfo.FromParent)
            {
                var parent = resolver.Parent;
                if (parent == null)
                {
                    if (paramInfo.IsOptional)
                        return paramInfo.HasDefaultValue ? paramInfo.DefaultValue : GetDefaultValue(paramInfo.ParameterType);
                    
                    throw ResolutionException.NoParentContainer(paramInfo.ParameterType);
                }

                if (paramInfo.Key != null)
                {
                    return parent.ResolveKeyed(paramInfo.ParameterType, paramInfo.Key);
                }
                return parent.Resolve(paramInfo.ParameterType);
            }

            // 按键值解析
            if (paramInfo.Key != null)
            {
                try
                {
                    return resolver.ResolveKeyed(paramInfo.ParameterType, paramInfo.Key);
                }
                catch (ResolutionException)
                {
                    if (paramInfo.IsOptional)
                        return paramInfo.HasDefaultValue ? paramInfo.DefaultValue : GetDefaultValue(paramInfo.ParameterType);
                    throw;
                }
            }

            // 普通解析
            if (resolver.TryResolve(paramInfo.ParameterType, out var instance))
            {
                return instance;
            }

            // 可选参数返回默认值
            if (paramInfo.IsOptional || paramInfo.HasDefaultValue)
            {
                return paramInfo.HasDefaultValue ? paramInfo.DefaultValue : GetDefaultValue(paramInfo.ParameterType);
            }

            throw ResolutionException.ServiceNotRegistered(paramInfo.ParameterType);
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        #endregion
    }
}
