// ==========================================================
// 文件名：FieldInjector.cs
// 命名空间: AFramework.DI
// 依赖: System, System.Collections.Generic, System.Runtime.CompilerServices
// 功能: 实现字段注入逻辑
// 优化: 热路径内联优化
// ==========================================================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AFramework.DI
{
    /// <summary>
    /// 字段注入器
    /// <para>负责向对象的字段注入依赖</para>
    /// <para>Field injector that injects dependencies into object fields</para>
    /// </summary>
    /// <remarks>
    /// 性能优化：
    /// <list type="bullet">
    /// <item>热路径方法使用 AggressiveInlining 优化</item>
    /// </list>
    /// </remarks>
    internal sealed class FieldInjector
    {
        #region 单例 / Singleton

        private static readonly Lazy<FieldInjector> _instance = 
            new Lazy<FieldInjector>(() => new FieldInjector());

        /// <summary>
        /// 获取注入器实例
        /// <para>Get the injector instance</para>
        /// </summary>
        public static FieldInjector Instance => _instance.Value;

        #endregion

        #region 构造函数 / Constructor

        private FieldInjector()
        {
        }

        #endregion

        #region 公共方法 / Public Methods

        /// <summary>
        /// 注入字段
        /// <para>Inject fields into an instance</para>
        /// </summary>
        /// <param name="instance">目标实例 / Target instance</param>
        /// <param name="fields">字段注入信息列表 / Field injection info list</param>
        /// <param name="resolver">对象解析器 / Object resolver</param>
        /// <param name="parameters">额外参数 / Additional parameters</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inject(
            object instance,
            IReadOnlyList<FieldInjectionInfo> fields,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (fields == null || fields.Count == 0)
                return;
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            foreach (var fieldInfo in fields)
            {
                InjectField(instance, fieldInfo, resolver, parameters);
            }
        }

        #endregion

        #region 私有方法 / Private Methods

        /// <summary>
        /// 注入单个字段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InjectField(
            object instance,
            FieldInjectionInfo fieldInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            var value = ResolveValue(fieldInfo, resolver, parameters);
            
            if (value != null || !fieldInfo.IsOptional)
            {
                try
                {
                    fieldInfo.FieldInfo.SetValue(instance, value);
                }
                catch (Exception ex)
                {
                    throw new ResolutionException(
                        $"设置字段 {fieldInfo.FieldInfo.Name} 时发生错误。\n" +
                        $"Error setting field {fieldInfo.FieldInfo.Name}.",
                        ex);
                }
            }
        }

        /// <summary>
        /// 解析字段值
        /// </summary>
        private object ResolveValue(
            FieldInjectionInfo fieldInfo,
            IObjectResolver resolver,
            IReadOnlyList<IInjectParameter> parameters)
        {
            var fieldType = fieldInfo.FieldType;
            var fieldName = fieldInfo.FieldInfo.Name;

            // 首先检查提供的参数
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    if (param.CanSupply(fieldType, fieldName))
                    {
                        return param.GetValue(resolver);
                    }
                }
            }

            // 从父容器解析
            if (fieldInfo.FromParent)
            {
                var parent = resolver.Parent;
                if (parent == null)
                {
                    if (fieldInfo.IsOptional)
                        return GetDefaultValue(fieldType);
                    
                    throw ResolutionException.NoParentContainer(fieldType);
                }

                if (fieldInfo.Key != null)
                {
                    return parent.ResolveKeyed(fieldType, fieldInfo.Key);
                }
                return parent.Resolve(fieldType);
            }

            // 按键值解析
            if (fieldInfo.Key != null)
            {
                try
                {
                    return resolver.ResolveKeyed(fieldType, fieldInfo.Key);
                }
                catch (ResolutionException)
                {
                    if (fieldInfo.IsOptional)
                        return GetDefaultValue(fieldType);
                    throw;
                }
            }

            // 普通解析
            if (resolver.TryResolve(fieldType, out var instance))
            {
                return instance;
            }

            // 可选字段返回默认值
            if (fieldInfo.IsOptional)
            {
                return GetDefaultValue(fieldType);
            }

            throw ResolutionException.ServiceNotRegistered(fieldType);
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
