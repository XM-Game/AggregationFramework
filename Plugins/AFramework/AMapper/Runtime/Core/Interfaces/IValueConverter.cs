// ==========================================================
// 文件名：IValueConverter.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义值转换器接口，用于成员级别的值转换
// ==========================================================

namespace AFramework.AMapper
{
    /// <summary>
    /// 值转换器接口
    /// <para>用于成员级别的值转换，是类型转换器和值解析器的中间形态</para>
    /// <para>Value converter interface for member-level value conversion</para>
    /// </summary>
    /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 值转换器专注于值的转换，不访问源对象或目标对象。
    /// 适用于简单的格式转换场景。
    /// 
    /// 使用示例：
    /// <code>
    /// public class CurrencyConverter : IValueConverter&lt;decimal, string&gt;
    /// {
    ///     public string Convert(decimal sourceMember, ResolutionContext context)
    ///     {
    ///         return sourceMember.ToString("C2");
    ///     }
    /// }
    /// 
    /// // 配置
    /// CreateMap&lt;Product, ProductDto&gt;()
    ///     .ForMember(d => d.PriceText, opt => opt.ConvertUsing(new CurrencyConverter(), s => s.Price));
    /// </code>
    /// 
    /// 值转换器 vs 类型转换器 vs 值解析器：
    /// <list type="bullet">
    /// <item>值转换器：成员级，只关注值转换，不访问对象</item>
    /// <item>类型转换器：类型级，控制整个类型的转换</item>
    /// <item>值解析器：成员级，可访问源对象和目标对象</item>
    /// </list>
    /// </remarks>
    public interface IValueConverter<in TSourceMember, TDestMember>
    {
        /// <summary>
        /// 转换源成员值为目标成员值
        /// <para>Convert source member value to destination member value</para>
        /// </summary>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>转换后的值 / Converted value</returns>
        TDestMember Convert(TSourceMember sourceMember, ResolutionContext context);
    }

    /// <summary>
    /// 非泛型值转换器接口
    /// <para>Non-generic value converter interface</para>
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// 转换源成员值为目标成员值
        /// <para>Convert source member value to destination member value</para>
        /// </summary>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>转换后的值 / Converted value</returns>
        object Convert(object sourceMember, ResolutionContext context);
    }
}
