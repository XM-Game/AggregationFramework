// ==========================================================
// 文件名：ITypeConverter.cs
// 命名空间: AFramework.AMapper
// 依赖: System
// 功能: 定义类型转换器接口，用于完全控制类型间的转换逻辑
// ==========================================================

namespace AFramework.AMapper
{
    /// <summary>
    /// 类型转换器接口
    /// <para>用于完全控制源类型到目标类型的转换逻辑</para>
    /// <para>Type converter interface for complete control over type conversion</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <remarks>
    /// 类型转换器用于完全替代默认的映射逻辑。
    /// 一旦配置了类型转换器，AMapper 将不会执行任何成员映射，
    /// 而是直接调用转换器的 Convert 方法。
    /// 
    /// 使用示例：
    /// <code>
    /// public class StringToIntConverter : ITypeConverter&lt;string, int&gt;
    /// {
    ///     public int Convert(string source, int destination, ResolutionContext context)
    ///     {
    ///         return int.TryParse(source, out var result) ? result : 0;
    ///     }
    /// }
    /// 
    /// // 配置
    /// CreateMap&lt;string, int&gt;().ConvertUsing&lt;StringToIntConverter&gt;();
    /// </code>
    /// 
    /// 类型转换器 vs 值解析器：
    /// <list type="bullet">
    /// <item>类型转换器：全局作用域，控制整个类型的转换</item>
    /// <item>值解析器：成员级作用域，控制单个成员的值</item>
    /// </list>
    /// </remarks>
    public interface ITypeConverter<in TSource, TDestination>
    {
        /// <summary>
        /// 将源对象转换为目标类型
        /// <para>Convert source object to destination type</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">现有目标对象（可能为默认值） / Existing destination object (may be default)</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>转换后的目标对象 / Converted destination object</returns>
        TDestination Convert(TSource source, TDestination destination, ResolutionContext context);
    }

    /// <summary>
    /// 非泛型类型转换器接口
    /// <para>Non-generic type converter interface</para>
    /// </summary>
    /// <remarks>
    /// 用于运行时动态转换场景。
    /// 通常不直接实现此接口，而是实现泛型版本。
    /// </remarks>
    public interface ITypeConverter
    {
        /// <summary>
        /// 将源对象转换为目标类型
        /// <para>Convert source object to destination type</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">现有目标对象 / Existing destination object</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>转换后的目标对象 / Converted destination object</returns>
        object Convert(object source, object destination, ResolutionContext context);
    }
}
