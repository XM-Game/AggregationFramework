// ==========================================================
// 文件名：IValueResolver.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义值解析器接口，用于自定义成员值的获取逻辑
// ==========================================================

namespace AFramework.AMapper
{
    /// <summary>
    /// 值解析器接口
    /// <para>用于为目标成员提供自定义值解析逻辑</para>
    /// <para>Value resolver interface for custom value resolution logic</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 值解析器用于完全自定义目标成员的值来源。
    /// 适用于需要复杂计算或访问外部服务的场景。
    /// 
    /// 使用示例：
    /// <code>
    /// public class FullNameResolver : IValueResolver&lt;Person, PersonDto, string&gt;
    /// {
    ///     public string Resolve(Person source, PersonDto destination, string destMember, ResolutionContext context)
    ///     {
    ///         return $"{source.FirstName} {source.LastName}";
    ///     }
    /// }
    /// 
    /// // 配置
    /// CreateMap&lt;Person, PersonDto&gt;()
    ///     .ForMember(d => d.FullName, opt => opt.MapFrom&lt;FullNameResolver&gt;());
    /// </code>
    /// </remarks>
    public interface IValueResolver<in TSource, in TDestination, TDestMember>
    {
        /// <summary>
        /// 解析目标成员的值
        /// <para>Resolve the value for destination member</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象（可能为 null） / Destination object (may be null)</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        TDestMember Resolve(TSource source, TDestination destination, TDestMember destMember, ResolutionContext context);
    }

    /// <summary>
    /// 非泛型值解析器接口
    /// <para>Non-generic value resolver interface</para>
    /// </summary>
    /// <remarks>
    /// 用于运行时动态解析场景。
    /// 通常不直接实现此接口，而是实现泛型版本。
    /// </remarks>
    public interface IValueResolver
    {
        /// <summary>
        /// 解析目标成员的值
        /// <para>Resolve the value for destination member</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        object Resolve(object source, object destination, object destMember, ResolutionContext context);
    }
}
