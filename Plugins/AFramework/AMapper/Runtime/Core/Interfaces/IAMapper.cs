// ==========================================================
// 文件名：IAMapper.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义对象映射器的核心接口，提供类型映射能力
// ==========================================================

using System;

namespace AFramework.AMapper
{
    /// <summary>
    /// 对象映射器接口
    /// <para>定义对象-对象映射的核心能力，支持泛型和非泛型映射方法</para>
    /// <para>Object mapper interface that defines core object-to-object mapping capabilities</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责执行映射，不涉及配置逻辑</item>
    /// <item>接口隔离：提供细粒度的映射方法，消费者按需使用</item>
    /// <item>依赖倒置：高层模块依赖此抽象接口，而非具体映射器实现</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 基本映射
    /// var dto = mapper.Map&lt;PlayerDto&gt;(playerData);
    /// 
    /// // 映射到现有对象
    /// mapper.Map(source, existingDestination);
    /// 
    /// // 带运行时选项的映射
    /// var dto = mapper.Map&lt;PlayerDto&gt;(source, opt => opt.Items["Key"] = value);
    /// </code>
    /// </remarks>
    public interface IAMapper
    {
        #region 泛型映射方法 / Generic Mapping Methods

        /// <summary>
        /// 将源对象映射到目标类型的新实例
        /// <para>Map source object to a new instance of destination type</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        /// <exception cref="MappingException">当映射执行失败时抛出 / Thrown when mapping execution fails</exception>
        /// <remarks>
        /// 此方法会创建目标类型的新实例并执行映射。
        /// 如果源对象为 null，将返回目标类型的默认值。
        /// </remarks>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// 将源对象映射到目标类型的新实例，支持运行时选项
        /// <para>Map source object to a new instance with runtime options</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="options">运行时映射选项配置 / Runtime mapping options configuration</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> options);

        /// <summary>
        /// 将源对象映射到目标类型的新实例（强类型源）
        /// <para>Map source object to a new instance (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// 将源对象映射到目标类型的新实例（强类型源），支持运行时选项
        /// <para>Map source object to a new instance (strongly typed source) with runtime options</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="options">运行时映射选项配置 / Runtime mapping options configuration</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> options);

        #endregion

        #region 映射到现有对象 / Map to Existing Object

        /// <summary>
        /// 将源对象映射到现有的目标对象
        /// <para>Map source object to an existing destination object</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <returns>映射后的目标对象（与传入的 destination 相同） / Mapped destination object (same as input destination)</returns>
        /// <remarks>
        /// 此方法会将源对象的属性值映射到现有目标对象上，不会创建新实例。
        /// 适用于需要更新现有对象的场景。
        /// </remarks>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// 将源对象映射到现有的目标对象，支持运行时选项
        /// <para>Map source object to an existing destination object with runtime options</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="options">运行时映射选项配置 / Runtime mapping options configuration</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> options);

        #endregion

        #region 非泛型映射方法 / Non-Generic Mapping Methods

        /// <summary>
        /// 将源对象映射到目标类型的新实例（非泛型版本）
        /// <para>Map source object to a new instance of destination type (non-generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        /// <exception cref="ArgumentNullException">当 sourceType 或 destinationType 为 null 时抛出 / Thrown when sourceType or destinationType is null</exception>
        object Map(object source, Type sourceType, Type destinationType);

        /// <summary>
        /// 将源对象映射到目标类型的新实例（非泛型版本），支持运行时选项
        /// <para>Map source object to a new instance (non-generic version) with runtime options</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="options">运行时映射选项配置 / Runtime mapping options configuration</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> options);

        /// <summary>
        /// 将源对象映射到现有的目标对象（非泛型版本）
        /// <para>Map source object to an existing destination object (non-generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        object Map(object source, object destination, Type sourceType, Type destinationType);

        /// <summary>
        /// 将源对象映射到现有的目标对象（非泛型版本），支持运行时选项
        /// <para>Map source object to an existing destination object (non-generic version) with runtime options</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="sourceType">源类型 / Source type</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <param name="options">运行时映射选项配置 / Runtime mapping options configuration</param>
        /// <returns>映射后的目标对象 / Mapped destination object</returns>
        object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> options);

        #endregion

        #region 配置访问 / Configuration Access

        /// <summary>
        /// 获取映射器配置
        /// <para>Get the mapper configuration</para>
        /// </summary>
        IMapperConfiguration Configuration { get; }

        #endregion
    }
}
