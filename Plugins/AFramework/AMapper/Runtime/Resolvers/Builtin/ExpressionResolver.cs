// ==========================================================
// 文件名：ExpressionResolver.cs
// 命名空间: AFramework.AMapper.Resolvers
// 依赖: System, System.Linq.Expressions
// 功能: 表达式解析器，通过 Lambda 表达式计算目标成员值
// ==========================================================

using System;
using System.Linq.Expressions;

namespace AFramework.AMapper.Resolvers
{
    /// <summary>
    /// 表达式解析器
    /// <para>通过 Lambda 表达式计算目标成员值</para>
    /// <para>Expression resolver that calculates destination member value through lambda expression</para>
    /// </summary>
    /// <typeparam name="TSource">源类型 / Source type</typeparam>
    /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
    /// <typeparam name="TDestMember">目标成员类型 / Destination member type</typeparam>
    /// <remarks>
    /// 适用场景：
    /// <list type="bullet">
    /// <item>复杂计算：(s, d) => s.Price * s.Quantity</item>
    /// <item>条件逻辑：(s, d) => s.Age >= 18 ? "Adult" : "Minor"</item>
    /// <item>访问目标对象：(s, d) => d.ExistingValue + s.NewValue</item>
    /// </list>
    /// 
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：仅负责表达式计算</item>
    /// <item>灵活性：支持任意复杂的表达式逻辑</item>
    /// <item>性能优化：编译表达式为委托</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 配置
    /// CreateMap&lt;Order, OrderDto&gt;()
    ///     .ForMember(d => d.TotalPrice, opt => opt.MapFrom((s, d) => s.Price * s.Quantity));
    /// </code>
    /// </remarks>
    public sealed class ExpressionResolver<TSource, TDestination, TDestMember> :
        ValueResolverBase<TSource, TDestination, TDestMember>
    {
        #region 私有字段 / Private Fields

        private readonly Func<TSource, TDestination, TDestMember> _compiledExpression;
        private readonly LambdaExpression _sourceExpression;

        #endregion

        #region 构造函数 / Constructor

        /// <summary>
        /// 创建表达式解析器（源对象参数）
        /// </summary>
        /// <param name="sourceExpression">源表达式 / Source expression</param>
        public ExpressionResolver(Expression<Func<TSource, TDestMember>> sourceExpression)
        {
            if (sourceExpression == null)
                throw new ArgumentNullException(nameof(sourceExpression));

            _sourceExpression = sourceExpression;
            
            // 转换为 (source, dest) => expression(source) 形式
            var destParam = Expression.Parameter(typeof(TDestination), "dest");
            var lambda = Expression.Lambda<Func<TSource, TDestination, TDestMember>>(
                sourceExpression.Body,
                sourceExpression.Parameters[0],
                destParam);
            
            _compiledExpression = lambda.Compile();
        }

        /// <summary>
        /// 创建表达式解析器（源对象和目标对象参数）
        /// </summary>
        /// <param name="sourceExpression">源表达式 / Source expression</param>
        public ExpressionResolver(Expression<Func<TSource, TDestination, TDestMember>> sourceExpression)
        {
            _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            _compiledExpression = sourceExpression.Compile();
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
            try
            {
                // 执行编译后的表达式
                return _compiledExpression(source, destination);
            }
            catch (Exception ex)
            {
                throw new MappingException(
                    $"执行表达式时发生错误。表达式：{_sourceExpression}",
                    ex);
            }
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 获取源表达式
        /// <para>Get source expression</para>
        /// </summary>
        public LambdaExpression GetSourceExpression()
        {
            return _sourceExpression;
        }

        #endregion
    }
}
