// ==========================================================
// 文件名：MappingOperationOptions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 映射操作选项实现，提供运行时映射配置
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射操作选项实现
    /// <para>Mapping operation options implementation</para>
    /// </summary>
    public class MappingOperationOptions : IMappingOperationOptions
    {
        #region 私有字段 / Private Fields

        private Dictionary<string, object> _items;

        #endregion

        #region IMappingOperationOptions 实现 / Implementation

        /// <inheritdoc/>
        public IDictionary<string, object> Items => _items ??= new Dictionary<string, object>();

        /// <inheritdoc/>
        public IServiceProvider ServiceProvider { get; set; }

        /// <inheritdoc/>
        public Func<Type, object> ConstructServicesUsing { get; set; }

        /// <inheritdoc/>
        public void BeforeMap(Action<object, object> beforeFunction)
        {
            BeforeMapAction = beforeFunction;
        }

        /// <inheritdoc/>
        public void AfterMap(Action<object, object> afterFunction)
        {
            AfterMapAction = afterFunction;
        }

        #endregion

        #region 属性 / Properties

        /// <summary>
        /// 获取前置映射动作
        /// <para>Get before map action</para>
        /// </summary>
        public Action<object, object> BeforeMapAction { get; private set; }

        /// <summary>
        /// 获取后置映射动作
        /// <para>Get after map action</para>
        /// </summary>
        public Action<object, object> AfterMapAction { get; private set; }

        #endregion
    }

    /// <summary>
    /// 泛型映射操作选项实现
    /// <para>Generic mapping operation options implementation</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    public class MappingOperationOptions<TSource, TDestination> 
        : MappingOperationOptions, IMappingOperationOptions<TSource, TDestination>
    {
        #region 属性 / Properties

        /// <summary>
        /// 获取泛型前置映射动作
        /// <para>Get generic before map action</para>
        /// </summary>
        public Action<TSource, TDestination> TypedBeforeMapAction { get; private set; }

        /// <summary>
        /// 获取泛型后置映射动作
        /// <para>Get generic after map action</para>
        /// </summary>
        public Action<TSource, TDestination> TypedAfterMapAction { get; private set; }

        #endregion

        #region IMappingOperationOptions<TSource, TDestination> 实现 / Implementation

        /// <inheritdoc/>
        public void BeforeMap(Action<TSource, TDestination> beforeFunction)
        {
            TypedBeforeMapAction = beforeFunction;
            base.BeforeMap((s, d) => beforeFunction((TSource)s, (TDestination)d));
        }

        /// <inheritdoc/>
        public void AfterMap(Action<TSource, TDestination> afterFunction)
        {
            TypedAfterMapAction = afterFunction;
            base.AfterMap((s, d) => afterFunction((TSource)s, (TDestination)d));
        }

        #endregion
    }
}
