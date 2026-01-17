// ==========================================================
// 文件名：ConstantResolver.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System
// 功能: 常量解析器，为目标成员提供固定的常量值
// ==========================================================

using System;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 常量解析器
    /// <para>为目标成员提供固定的常量值</para>
    /// <para>Constant resolver that provides fixed constant value for destination member</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>固定值：Status = "Active"</item>
    /// <item>默认值：CreatedBy = "System"</item>
    /// <item>标记值：IsImported = true</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责返回常量值</item>
    /// <item>不可变性：常量值在创建时确定，不可修改</item>
    /// <item>性能优化：无需计算，直接返回</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 配置
    /// CreateMap&lt;Order, OrderDto&gt;()
    ///     .ForMember(d => d.Status, opt => opt.MapFrom(_ => "Active"))
    ///     .ForMember(d => d.CreatedBy, opt => opt.MapFrom(_ => "System"));
    /// </code>
    /// </remarks>
    public sealed class ConstantResolver<TSource, TDestination, TDestMember> :
        ValueResolverBase<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private readonly TDestMember _constantValue;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建常量解析器
        /// </summary>
        /// <param name="constantValue">常量值 / Constant value</param>
        public ConstantResolver(TDestMember constantValue)
        {
            _constantValue = constantValue;
        }

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        protected override TDestMember Resolve(
            TSource source,
            TDestination destination,
            TDestMember destMember,
            ResolutionContext context)
        {
            // 直接返回常量值
            return _constantValue;
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取常量值
        /// <para>Get constant value</para>
        /// </summary>
        public TDestMember GetConstantValue()
        {
            return _constantValue;
        }

        #endregion
    }

    /// <summary>
    /// 常量解析器（非泛型版本）
    /// <para>Constant resolver (non-generic version)</para>
    /// </summary>
    /// <remarks>
    /// 用于运行时动态创建解析器的场景。
    /// </remarks>
    public sealed class ConstantResolver : IValueResolver
    {
        #region 私有字段 / Private Fields

        private readonly object _constantValue;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建常量解析器
        /// </summary>
        /// <param name="constantValue">常量值 / Constant value</param>
        public ConstantResolver(object constantValue)
        {
            _constantValue = constantValue;
        }

        #endregion

        #region 解析方法 / Resolve Methods

        /// <summary>
        /// 解析目标成员的值
        /// </summary>
        public object Resolve(
            object source,
            object destination,
            object destMember,
            ResolutionContext context)
        {
            // 直接返回常量值
            return _constantValue;
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取常量值
        /// <para>Get constant value</para>
        /// </summary>
        public object GetConstantValue()
        {
            return _constantValue;
        }

        #endregion
    }
}
