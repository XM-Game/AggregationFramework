// ==========================================================
// 文件名：PropertyInjector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现属性注入逻辑
// 优化: 热路径内联优化
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 属性注入器
    /// <para>负责向对象的属性注入依赖</para>
    /// <para>Property injector that injects dependencies into object properties</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// </list>
    /// </remarks>
    internal sealed class PropertyInjector
    {
        #region 单例 / Singleton

        private static readonly Lazy<PropertyInjector> _instance = 
            new Lazy<PropertyInjector>(() => new PropertyInjector());

        /// <summary>
        /// 获取注入器实例
        /// <para>Get the injector instance</para>
        /// </summary>
        public static PropertyInjector Instance => _instance.Value;

        #endregion

        #region 构造函数 / Constructor

        private PropertyInjector()
        {
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 注入属性
        /// <para>Inject properties into an instance</para>
        /// </summary>
        /// <param name="instance">目标实例 / Target instance</param>
        /// <param name="properties">属性注入信息列表 / Property injection info list</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外参数 / Additional parameters</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inject(
            object instance,
            IReadOnlyList<PropertyInjectionInfo> properties,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (properties == null || properties.Count == 0)
                return;
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            foreach (var propertyInfo in properties)
            {
                InjectProperty(instance, propertyInfo, resolver, parameters);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 注入单个属性
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InjectProperty(
            object instance,
            PropertyInjectionInfo propertyInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            var value = ResolveValue(propertyInfo, resolver, parameters);
            
            if (value != null || !propertyInfo.IsOptional)
            {
                try
                {
                    propertyInfo.PropertyInfo.SetValue(instance, value);
                }
                catch (Exception ex)
                {
                    throw new ResolutionException(
                        $"设置属性 {propertyInfo.PropertyInfo.Name} 时发生错误。\n" +
                        $"Error setting property {propertyInfo.PropertyInfo.Name}.",
                        ex);
                }
            }
        }

        /// <summary>
        /// 解析属性值
        /// </summary>
        private object ResolveValue(
            PropertyInjectionInfo propertyInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            var propertyType = propertyInfo.PropertyType;
            var propertyName = propertyInfo.PropertyInfo.Name;

            // 首先检查提供的参数
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.CanSupply(propertyType, propertyName))
                    {
                        return param.GetValue(resolver);
                    }
                }
            }

            // 从父容器解析
            if (propertyInfo.FromParent)
            {
                var parent = resolver.Parent;
                if (parent == null)
                {
                    if (propertyInfo.IsOptional)
                        return GetDefaultValue(propertyType);
                    
                    throw ResolutionException.NoParentContainer(propertyType);
                }

                if (propertyInfo.Key != null)
                {
                    return parent.ResolveKeyed(propertyType, propertyInfo.Key);
                }
                return parent.Resolve(propertyType);
            }

            // 按键值解析
            if (propertyInfo.Key != null)
            {
                try
                {
                    return resolver.ResolveKeyed(propertyType, propertyInfo.Key);
                }
                catch (ResolutionException)
                {
                    if (propertyInfo.IsOptional)
                        return GetDefaultValue(propertyType);
                    throw;
                }
            }

            // 普通解析
            if (resolver.TryResolve(propertyType, out var instance))
            {
                return instance;
            }

            // 可选属性返回默认值
            if (propertyInfo.IsOptional)
            {
                return GetDefaultValue(propertyType);
            }

            throw ResolutionException.ServiceNotRegistered(propertyType);
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
