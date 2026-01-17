// ==========================================================
// 文件名：IMappingOperationOptions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Collections.Generic
// 功能: 定义映射操作选项接口，用于运行时配置映射行为
// ==========================================================

using System;
using System.Collections.Generic;

namespace AFramework.AMapper
{
    /// <summary>
    /// 映射操作选项接口
    /// <para>用于在 Map 调用时传递运行时选项</para>
    /// <para>Mapping operation options interface for runtime configuration</para>
    /// </summary>
    /// <remarks>
    /// 映射操作选项允许在每次 Map 调用时传递不同的配置。
    /// 
    /// 使用示例：
    /// <code>
    /// var dto = mapper.Map&lt;PlayerDto&gt;(player, opt =>
    /// {
    ///     opt.Items["CurrentUser"] = currentUser;
    ///     opt.BeforeMap((src, dest) => dest.Timestamp = DateTime.Now);
    ///     opt.AfterMap((src, dest) => logger.Log($"Mapped {src.Id}"));
    /// });
    /// </code>
    /// </remarks>
    public interface IMappingOperationOptions
    {
        #region 数据传递 / Data Passing

        /// <summary>
        /// 获取键值对数据字典
        /// <para>Get the key-value data dictionary</para>
        /// </summary>
        /// <remarks>
        /// 用于在映射过程中传递额外数据。
        /// 可在值解析器和映射动作中通过 context.Items 访问。
        /// </remarks>
        IDictionary<string, object> Items { get; }

        #endregion

        #region 映射动作 / Mapping Actions

        /// <summary>
        /// 设置前置映射动作
        /// <para>Set the before map action</para>
        /// </summary>
        /// <param name="beforeFunction">前置动作委托 / Before action delegate</param>
        void BeforeMap(Action<object, object> beforeFunction);

        /// <summary>
        /// 设置后置映射动作
        /// <para>Set the after map action</para>
        /// </summary>
        /// <param name="afterFunction">后置动作委托 / After action delegate</param>
        void AfterMap(Action<object, object> afterFunction);

        #endregion

        #region 构造配置 / Construction Configuration

        /// <summary>
        /// 获取或设置自定义服务构造函数
        /// <para>Get or set custom service constructor</para>
        /// </summary>
        Func<Type, object> ConstructServicesUsing { get; set; }

        #endregion

        #region 服务提供者 / Service Provider

        /// <summary>
        /// 获取或设置服务提供者
        /// <para>Get or set the service provider</para>
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }

        #endregion
    }

    /// <summary>
    /// 泛型映射操作选项接口
    /// <para>Generic mapping operation options interface</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <remarks>
    /// 提供强类型的映射操作选项。
    /// 
    /// 使用示例：
    /// <code>
    /// var dto = mapper.Map&lt;Player, PlayerDto&gt;(player, opt =>
    /// {
    ///     opt.BeforeMap((src, dest) => dest.MappedAt = DateTime.Now);
    ///     opt.AfterMap((src, dest) => ValidateDto(dest));
    /// });
    /// </code>
    /// </remarks>
    public interface IMappingOperationOptions<TSource, TDestination> : IMappingOperationOptions
    {
        #region 强类型映射动作 / Strongly Typed Mapping Actions

        /// <summary>
        /// 设置强类型前置映射动作
        /// <para>Set strongly typed before map action</para>
        /// </summary>
        /// <param name="beforeFunction">前置动作委托 / Before action delegate</param>
        void BeforeMap(Action<TSource, TDestination> beforeFunction);

        /// <summary>
        /// 设置强类型后置映射动作
        /// <para>Set strongly typed after map action</para>
        /// </summary>
        /// <param name="afterFunction">后置动作委托 / After action delegate</param>
        void AfterMap(Action<TSource, TDestination> afterFunction);

        #endregion
    }

}
