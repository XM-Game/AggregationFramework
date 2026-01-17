// ==========================================================
// 文件名：IMemberValueResolver.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义成员值解析器接口，接收源成员值作为输入
// ==========================================================

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员值解析器接口
    /// <para>接收源成员值作为输入的值解析器</para>
    /// <para>Member value resolver interface that receives source member value as input</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 与 IValueResolver 不同，此接口接收特定源成员的值作为输入。
    /// 适用于需要基于特定源成员进行转换的场景。
    /// 
    /// 使用示例：
    /// <code>
    /// public class DateFormatter : IMemberValueResolver&lt;Order, OrderDto, DateTime, string&gt;
    /// {
    ///     public string Resolve(Order source, OrderDto destination, DateTime sourceMember, string destMember, ResolutionContext context)
    ///     {
    ///         return sourceMember.ToString("yyyy-MM-dd");
    ///     }
    /// }
    /// 
    /// // 配置
    /// CreateMap&lt;Order, OrderDto&gt;()
    ///     .ForMember(d => d.OrderDate, opt => opt.MapFrom&lt;DateFormatter, DateTime&gt;(s => s.CreatedAt));
    /// </code>
    /// </remarks>
    public interface IMemberValueResolver<in TSource, in TDestination, in TSourceMember, TDestMember>
    {
        /// <summary>
        /// 解析目标成员的值
        /// <para>Resolve the value for destination member</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象（可能为 null） / Destination object (may be null)</param>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        TDestMember Resolve(TSource source, TDestination destination, TSourceMember sourceMember, TDestMember destMember, ResolutionContext context);
    }

    /// <summary>
    /// 非泛型成员值解析器接口
    /// <para>Non-generic member value resolver interface</para>
    /// </summary>
    public interface IMemberValueResolver
    {
        /// <summary>
        /// 解析目标成员的值
        /// <para>Resolve the value for destination member</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        object Resolve(object source, object destination, object sourceMember, object destMember, ResolutionContext context);
    }
}
