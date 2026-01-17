// ==========================================================
// 文件名：ResolutionContextExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: ResolutionContext 扩展方法，提供便捷的上下文操作
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// ResolutionContext 扩展方法
    /// <para>提供便捷的上下文操作方法</para>
    /// <para>Extension methods for ResolutionContext providing convenient operations</para>
    /// </summary>
    public static class ResolutionContextExtensions
    {
        #region 映射扩展 / Mapping Extensions

        /// <summary>
        /// 使用当前上下文执行嵌套映射
        /// <para>Execute nested mapping using current context</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        public static TDestination MapNested<TDestination>(this ResolutionContext context, object source)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (source == null)
                return default;

            return context.Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// 使用当前上下文执行嵌套映射到现有对象
        /// <para>Execute nested mapping to existing object using current context</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        public static TDestination MapNested<TSource, TDestination>(
            this ResolutionContext context,
            TSource source,
            TDestination destination)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (source == null)
                return destination;

            return context.Mapper.Map(source, destination);
        }

        #endregion

        #region 数据访问扩展 / Data Access Extensions

        /// <summary>
        /// 获取运行时数据项
        /// <para>Get runtime data item</para>
        /// </summary>
        /// <typeparam name="T">数据类型 / Data type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="key">键 / Key</param>
        /// <returns>数据值或默认值 / Data value or default</returns>
        public static T GetItem<T>(this ResolutionContext context, string key)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(key))
                return default;

            if (context.Items.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        /// <summary>
        /// 获取运行时数据项，如果不存在则返回指定默认值
        /// <para>Get runtime data item with fallback default value</para>
        /// </summary>
        /// <typeparam name="T">数据类型 / Data type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="key">键 / Key</param>
        /// <param name="defaultValue">默认值 / Default value</param>
        /// <returns>数据值或默认值 / Data value or default</returns>
        public static T GetItemOrDefault<T>(this ResolutionContext context, string key, T defaultValue)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(key))
                return defaultValue;

            if (context.Items.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// 尝试获取运行时数据项
        /// <para>Try to get runtime data item</para>
        /// </summary>
        /// <typeparam name="T">数据类型 / Data type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="key">键 / Key</param>
        /// <param name="value">数据值 / Data value</param>
        /// <returns>是否成功获取 / Whether successfully retrieved</returns>
        public static bool TryGetItem<T>(this ResolutionContext context, string key, out T value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            value = default;

            if (string.IsNullOrEmpty(key))
                return false;

            if (context.Items.TryGetValue(key, out var obj) && obj is T typedValue)
            {
                value = typedValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否存在指定键的数据项
        /// <para>Check if data item with specified key exists</para>
        /// </summary>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="key">键 / Key</param>
        /// <returns>是否存在 / Whether exists</returns>
        public static bool HasItem(this ResolutionContext context, string key)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return !string.IsNullOrEmpty(key) && context.Items.ContainsKey(key);
        }

        #endregion

        #region 服务解析扩展 / Service Resolution Extensions

        /// <summary>
        /// 获取必需的服务，如果不存在则抛出异常
        /// <para>Get required service, throw exception if not found</para>
        /// </summary>
        /// <typeparam name="TService">服务类型 / Service type</typeparam>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>服务实例 / Service instance</returns>
        /// <exception cref="InvalidOperationException">当服务不存在时抛出 / Thrown when service not found</exception>
        public static TService GetRequiredService<TService>(this ResolutionContext context) where TService : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var service = context.GetService<TService>();
            if (service == null)
            {
                throw new InvalidOperationException(
                    $"无法解析服务 / Unable to resolve service: {typeof(TService).Name}");
            }

            return service;
        }

        /// <summary>
        /// 获取必需的服务，如果不存在则抛出异常（非泛型版本）
        /// <para>Get required service (non-generic), throw exception if not found</para>
        /// </summary>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="serviceType">服务类型 / Service type</param>
        /// <returns>服务实例 / Service instance</returns>
        /// <exception cref="InvalidOperationException">当服务不存在时抛出 / Thrown when service not found</exception>
        public static object GetRequiredService(this ResolutionContext context, Type serviceType)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            var service = context.GetService(serviceType);
            if (service == null)
            {
                throw new InvalidOperationException(
                    $"无法解析服务 / Unable to resolve service: {serviceType.Name}");
            }

            return service;
        }

        #endregion

        #region 深度控制扩展 / Depth Control Extensions

        /// <summary>
        /// 创建子上下文（增加深度）
        /// <para>Create child context with incremented depth</para>
        /// </summary>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>子上下文 / Child context</returns>
        public static ResolutionContext CreateChildContext(this ResolutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.IncrementDepth();
        }

        /// <summary>
        /// 检查是否应该停止映射（超过最大深度）
        /// <para>Check if mapping should stop (exceeded max depth)</para>
        /// </summary>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <param name="typeMap">类型映射 / Type map</param>
        /// <returns>是否应该停止 / Whether should stop</returns>
        public static bool ShouldStopMapping(this ResolutionContext context, ITypeMap typeMap)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (typeMap == null)
                return false;

            return context.IsMaxDepthExceeded(typeMap.MaxDepth);
        }

        #endregion
    }
}
