// ==========================================================
// 文件名：MethodInjector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现方法注入逻辑
// 优化: 使用数组池复用参数数组，减少GC压力；热路径内联优化
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 方法注入器
    /// <para>负责调用对象的注入方法并注入依赖</para>
    /// <para>Method injector that invokes injection methods with dependencies</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>使用 ObjectArrayPool 复用参数数组，减少GC压力</item>
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// <item>使用 try-finally 确保数组正确归还</item>
    /// </list>
    /// </remarks>
    internal sealed class MethodInjector
    {
        #region 单例 / Singleton

        private static readonly Lazy<MethodInjector> _instance = 
            new Lazy<MethodInjector>(() => new MethodInjector());

        /// <summary>
        /// 获取注入器实例
        /// <para>Get the injector instance</para>
        /// </summary>
        public static MethodInjector Instance => _instance.Value;

        #endregion

        #region 构造函数 / Constructor

        private MethodInjector()
        {
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 注入方法
        /// <para>Invoke injection methods on an instance</para>
        /// </summary>
        /// <param name="instance">目标实例 / Target instance</param>
        /// <param name="methods">方法注入信息列表 / Method injection info list</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外参数 / Additional parameters</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inject(
            object instance,
            IReadOnlyList<MethodInjectionInfo> methods,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (methods == null || methods.Count == 0)
                return;
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // 方法已按 Order 排序
            foreach (var methodInfo in methods)
            {
                InvokeMethod(instance, methodInfo, resolver, parameters);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 调用单个注入方法
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InvokeMethod(
            object instance,
            MethodInjectionInfo methodInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            var methodParams = methodInfo.Parameters;
            var paramCount = methodParams.Count;
            
            // 使用数组池租用参数数组
            var args = ObjectArrayPool.Rent(paramCount);

            try
            {
                // 解析所有参数
                for (int i = 0; i < paramCount; i++)
                {
                    args[i] = ResolveParameter(methodParams[i], resolver, parameters);
                }

                // 调用方法
                methodInfo.MethodInfo.Invoke(instance, args);
            }
            catch (Exception ex) when (!(ex is ResolutionException))
            {
                throw new ResolutionException(
                    $"调用注入方法 {methodInfo.MethodInfo.Name} 时发生错误。\n" +
                    $"Error invoking injection method {methodInfo.MethodInfo.Name}.",
                    ex);
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
