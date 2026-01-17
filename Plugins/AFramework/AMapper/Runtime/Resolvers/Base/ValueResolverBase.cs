// ==========================================================
// 文件名：ValueResolverBase.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 值解析器基类，简化自定义值解析器的实现
// ==========================================================

using System;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 值解析器基类
    /// <para>提供值解析器的基础实现，简化自定义解析器开发</para>
    /// <para>Base class for value resolvers that simplifies custom resolver implementation</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>模板方法模式：定义算法骨架，子类实现具体步骤</item>
    /// <item>单一职责：仅负责值解析逻辑</item>
    /// <item>易于扩展：子类只需实现 Resolve 方法</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// public class FullNameResolver : ValueResolverBase&lt;Person, PersonDto, string&gt;
    /// {
    ///     protected override string Resolve(Person source, PersonDto destination, string destMember, ResolutionContext context)
    ///     {
    ///         return $"{source.FirstName} {source.LastName}";
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class ValueResolverBase<TSource, TDestination, TDestMember> : 
        IValueResolver<TSource, TDestination, TDestMember>,
        IValueResolver
    {
        #region 泛型接口实现 / Generic Interface Implementation

        /// <summary>
        /// 解析目标成员的值（泛型版本）
        /// <para>Resolve the value for destination member (generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        TDestMember IValueResolver<TSource, TDestination, TDestMember>.Resolve(
            TSource source, 
            TDestination destination, 
            TDestMember destMember, 
            ResolutionContext context)
        {
            return Resolve(source, destination, destMember, context);
        }

        #endregion

        #region 非泛型接口实现 / Non-Generic Interface Implementation

        /// <summary>
        /// 解析目标成员的值（非泛型版本）
        /// <para>Resolve the value for destination member (non-generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        object IValueResolver.Resolve(
            object source, 
            object destination, 
            object destMember, 
            ResolutionContext context)
        {
            // 类型转换和验证
            if (source != null && !(source is TSource))
            {
                throw new ArgumentException(
                    $"源对象类型不匹配。期望 {typeof(TSource).Name}，实际 {source.GetType().Name}",
                    nameof(source));
            }

            if (destination != null && !(destination is TDestination))
            {
                throw new ArgumentException(
                    $"目标对象类型不匹配。期望 {typeof(TDestination).Name}，实际 {destination.GetType().Name}",
                    nameof(destination));
            }

            if (destMember != null && !(destMember is TDestMember))
            {
                throw new ArgumentException(
                    $"目标成员类型不匹配。期望 {typeof(TDestMember).Name}，实际 {destMember.GetType().Name}",
                    nameof(destMember));
            }

            // 调用泛型版本
            return Resolve(
                (TSource)source,
                (TDestination)destination,
                (TDestMember)destMember,
                context);
        }

        #endregion

        #region 抽象方法 / Abstract Methods

        /// <summary>
        /// 解析目标成员的值（子类实现）
        /// <para>Resolve the value for destination member (implemented by subclass)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        /// <remarks>
        /// 子类必须实现此方法以提供具体的值解析逻辑。
        /// 可以访问源对象、目标对象、当前成员值和解析上下文。
        /// </remarks>
        protected abstract TDestMember Resolve(
            TSource source, 
            TDestination destination, 
            TDestMember destMember, 
            ResolutionContext context);

        #endregion
    }
}
