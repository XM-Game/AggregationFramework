// ==========================================================
// 文件名：MemberValueResolverBase.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 成员值解析器基类，简化基于源成员的值解析器实现
// ==========================================================

using System;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 成员值解析器基类
    /// <para>提供成员值解析器的基础实现，接收源成员值作为输入</para>
    /// <para>Base class for member value resolvers that receive source member value as input</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TSourceMember">源成员类型 / Source member type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>模板方法模式：定义算法骨架，子类实现具体步骤</item>
    /// <item>单一职责：仅负责基于源成员的值转换</item>
    /// <item>易于扩展：子类只需实现 Resolve 方法</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// public class DateFormatter : MemberValueResolverBase&lt;Order, OrderDto, DateTime, string&gt;
    /// {
    ///     protected override string Resolve(Order source, OrderDto destination, DateTime sourceMember, string destMember, ResolutionContext context)
    ///     {
    ///         return sourceMember.ToString("yyyy-MM-dd");
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class MemberValueResolverBase<TSource, TDestination, TSourceMember, TDestMember> :
        IMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember>,
        IMemberValueResolver
    {
        #region 泛型接口实现 / Generic Interface Implementation

        /// <summary>
        /// 解析目标成员的值（泛型版本）
        /// <para>Resolve the value for destination member (generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        TDestMember IMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember>.Resolve(
            TSource source,
            TDestination destination,
            TSourceMember sourceMember,
            TDestMember destMember,
            ResolutionContext context)
        {
            return Resolve(source, destination, sourceMember, destMember, context);
        }

        #endregion

        #region 非泛型接口实现 / Non-Generic Interface Implementation

        /// <summary>
        /// 解析目标成员的值（非泛型版本）
        /// <para>Resolve the value for destination member (non-generic version)</para>
        /// </summary>
        /// <param name="source">源对象 / Source object</param>
        /// <param name="destination">目标对象 / Destination object</param>
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        object IMemberValueResolver.Resolve(
            object source,
            object destination,
            object sourceMember,
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

            if (sourceMember != null && !(sourceMember is TSourceMember))
            {
                throw new ArgumentException(
                    $"源成员类型不匹配。期望 {typeof(TSourceMember).Name}，实际 {sourceMember.GetType().Name}",
                    nameof(sourceMember));
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
                (TSourceMember)sourceMember,
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
        /// <param name="sourceMember">源成员值 / Source member value</param>
        /// <param name="destMember">当前目标成员值 / Current destination member value</param>
        /// <param name="context">解析上下文 / Resolution context</param>
        /// <returns>解析后的值 / Resolved value</returns>
        /// <remarks>
        /// 子类必须实现此方法以提供具体的值转换逻辑。
        /// 可以访问源对象、目标对象、源成员值、当前目标成员值和解析上下文。
        /// </remarks>
        protected abstract TDestMember Resolve(
            TSource source,
            TDestination destination,
            TSourceMember sourceMember,
            TDestMember destMember,
            ResolutionContext context);

        #endregion
    }
}
