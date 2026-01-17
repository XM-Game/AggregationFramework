// ==========================================================
// 文件名：QueryableExtensions.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq, System.Linq.Expressions
// 功能: IQueryable 扩展方法，提供 LINQ 投影和查询扩展
// ==========================================================

using System;
using System.Linq;
using System.Linq.Expressions;

namespace AFramework.AMapper
{
    /// <summary>
    /// IQueryable 扩展方法
    /// <para>提供 LINQ 投影和查询扩展功能</para>
    /// <para>IQueryable extension methods providing LINQ projection and query extensions</para>
    /// </summary>
    /// <remarks>
    /// 设计原则：
    /// <list type="bullet">
    /// <item>单一职责：每个扩展方法专注于单一查询场景</item>
    /// <item>延迟执行：保持 LINQ 的延迟执行特性</item>
    /// <item>表达式树：使用表达式树实现高效的数据库查询</item>
    /// </list>
    /// 
    /// 使用示例：
    /// <code>
    /// // 投影查询
    /// var dtos = dbContext.Players
    ///     .Where(p => p.IsActive)
    ///     .ProjectTo&lt;PlayerDto&gt;(mapper)
    ///     .ToList();
    /// 
    /// // 分页投影
    /// var pagedDtos = dbContext.Players
    ///     .ProjectToPage&lt;PlayerDto&gt;(mapper, pageIndex, pageSize);
    /// </code>
    /// 
    /// 注意事项：
    /// <list type="bullet">
    /// <item>ProjectTo 方法会生成表达式树，适用于 EF Core 等 ORM</item>
    /// <item>确保映射配置在查询执行前已完成</item>
    /// <item>复杂映射可能无法转换为 SQL，需要使用 AsEnumerable() 后再映射</item>
    /// </list>
    /// </remarks>
    public static class QueryableExtensions
    {
        #region 投影查询 / Projection Query

        /// <summary>
        /// 投影到目标类型
        /// <para>Project to destination type</para>
        /// </summary>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源查询 / Source query</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>投影后的查询 / Projected query</returns>
        /// <remarks>
        /// 此方法会生成表达式树，可以被 EF Core 等 ORM 转换为 SQL 查询。
        /// 适用于需要在数据库层面进行投影的场景。
        /// </remarks>
        public static IQueryable<TDestination> ProjectTo<TDestination>(
            this IQueryable source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            // 获取源类型
            var sourceType = source.ElementType;

            // 构建投影表达式
            var projectionExpression = BuildProjectionExpression(sourceType, typeof(TDestination), mapper);

            // 应用投影
            return source.Provider.CreateQuery<TDestination>(
                Expression.Call(
                    typeof(Queryable),
                    nameof(Queryable.Select),
                    new[] { sourceType, typeof(TDestination) },
                    source.Expression,
                    projectionExpression));
        }

        /// <summary>
        /// 投影到目标类型（强类型源）
        /// <para>Project to destination type (strongly typed source)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源查询 / Source query</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <returns>投影后的查询 / Projected query</returns>
        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(
            this IQueryable<TSource> source,
            IAMapper mapper)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            // 构建投影表达式
            var projectionExpression = BuildProjectionExpression<TSource, TDestination>(mapper);

            // 应用投影
            return source.Select(projectionExpression);
        }

        #endregion

        #region 分页投影 / Paging Projection

        /// <summary>
        /// 分页投影
        /// <para>Paged projection</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源查询 / Source query</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="pageIndex">页索引（从 0 开始）/ Page index (0-based)</param>
        /// <param name="pageSize">页大小 / Page size</param>
        /// <returns>分页投影查询 / Paged projection query</returns>
        public static IQueryable<TDestination> ProjectToPage<TSource, TDestination>(
            this IQueryable<TSource> source,
            IAMapper mapper,
            int pageIndex,
            int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "页索引不能为负数 / Page index cannot be negative");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "页大小必须大于 0 / Page size must be greater than 0");

            return source
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ProjectTo<TSource, TDestination>(mapper);
        }

        #endregion

        #region 条件投影 / Conditional Projection

        /// <summary>
        /// 条件投影（满足条件时投影）
        /// <para>Conditional projection (project if condition is met)</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <param name="source">源查询 / Source query</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="predicate">条件 / Predicate</param>
        /// <returns>投影后的查询 / Projected query</returns>
        public static IQueryable<TDestination> ProjectToWhere<TSource, TDestination>(
            this IQueryable<TSource> source,
            IAMapper mapper,
            Expression<Func<TSource, bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return source
                .Where(predicate)
                .ProjectTo<TSource, TDestination>(mapper);
        }

        #endregion

        #region 排序投影 / Ordered Projection

        /// <summary>
        /// 排序后投影
        /// <para>Project after ordering</para>
        /// </summary>
        /// <typeparam name="TSource">源类型 / Source type</typeparam>
        /// <typeparam name="TDestination">目标类型 / Destination type</typeparam>
        /// <typeparam name="TKey">排序键类型 / Order key type</typeparam>
        /// <param name="source">源查询 / Source query</param>
        /// <param name="mapper">映射器 / Mapper</param>
        /// <param name="keySelector">排序键选择器 / Key selector</param>
        /// <param name="descending">是否降序 / Whether descending</param>
        /// <returns>投影后的查询 / Projected query</returns>
        public static IQueryable<TDestination> ProjectToOrdered<TSource, TDestination, TKey>(
            this IQueryable<TSource> source,
            IAMapper mapper,
            Expression<Func<TSource, TKey>> keySelector,
            bool descending = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var orderedSource = descending
                ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);

            return orderedSource.ProjectTo<TSource, TDestination>(mapper);
        }

        #endregion

        #region 私有辅助方法 / Private Helper Methods

        /// <summary>
        /// 构建投影表达式
        /// </summary>
        private static Expression<Func<TSource, TDestination>> BuildProjectionExpression<TSource, TDestination>(
            IAMapper mapper)
        {
            // 获取映射配置
            var typeMap = mapper.Configuration.FindTypeMap<TSource, TDestination>();
            if (typeMap == null)
            {
                throw new MappingException(
                    $"未找到从 {typeof(TSource).Name} 到 {typeof(TDestination).Name} 的映射配置 / " +
                    $"Type map not found from {typeof(TSource).Name} to {typeof(TDestination).Name}");
            }

            // 创建参数表达式
            var parameter = Expression.Parameter(typeof(TSource), "src");

            // 构建成员绑定
            var bindings = typeMap.MemberMaps
                .Where(m => m.DestinationMember != null)
                .Select(m => CreateMemberBinding(m, parameter))
                .Where(b => b != null)
                .ToList();

            // 创建成员初始化表达式
            var newExpression = Expression.New(typeof(TDestination));
            var memberInit = Expression.MemberInit(newExpression, bindings);

            // 创建 Lambda 表达式
            return Expression.Lambda<Func<TSource, TDestination>>(memberInit, parameter);
        }

        /// <summary>
        /// 构建投影表达式（非泛型版本）
        /// </summary>
        private static LambdaExpression BuildProjectionExpression(
            Type sourceType,
            Type destinationType,
            IAMapper mapper)
        {
            // 获取映射配置
            var typeMap = mapper.Configuration.FindTypeMap(sourceType, destinationType);
            if (typeMap == null)
            {
                throw new MappingException(
                    $"未找到从 {sourceType.Name} 到 {destinationType.Name} 的映射配置 / " +
                    $"Type map not found from {sourceType.Name} to {destinationType.Name}");
            }

            // 创建参数表达式
            var parameter = Expression.Parameter(sourceType, "src");

            // 构建成员绑定
            var bindings = typeMap.MemberMaps
                .Where(m => m.DestinationMember != null)
                .Select(m => CreateMemberBinding(m, parameter))
                .Where(b => b != null)
                .ToList();

            // 创建成员初始化表达式
            var newExpression = Expression.New(destinationType);
            var memberInit = Expression.MemberInit(newExpression, bindings);

            // 创建 Lambda 表达式
            return Expression.Lambda(memberInit, parameter);
        }

        /// <summary>
        /// 创建成员绑定
        /// </summary>
        private static MemberBinding CreateMemberBinding(IMemberMap memberMap, ParameterExpression parameter)
        {
            try
            {
                // 获取源成员表达式
                var sourceMemberExpression = GetSourceMemberExpression(memberMap, parameter);
                if (sourceMemberExpression == null)
                    return null;

                // 获取目标成员
                var destinationMember = memberMap.DestinationMember;

                // 创建成员赋值
                return Expression.Bind(destinationMember, sourceMemberExpression);
            }
            catch
            {
                // 如果无法创建绑定，返回 null
                return null;
            }
        }

        /// <summary>
        /// 获取源成员表达式
        /// </summary>
        private static Expression GetSourceMemberExpression(IMemberMap memberMap, ParameterExpression parameter)
        {
            // 将接口转换为具体类型以访问内部属性
            var memberMapImpl = memberMap as MemberMap;
            if (memberMapImpl == null)
                return null;

            // 如果有自定义表达式，使用自定义表达式
            if (memberMapImpl.CustomMapExpression != null)
            {
                // 替换参数
                var visitor = new ParameterReplacementVisitor(
                    memberMapImpl.CustomMapExpression.Parameters[0],
                    parameter);
                return visitor.Visit(memberMapImpl.CustomMapExpression.Body);
            }

            // 否则使用源成员
            if (memberMapImpl.SourceMembers != null && memberMapImpl.SourceMembers.Length > 0)
            {
                return Expression.PropertyOrField(parameter, memberMapImpl.SourceMembers[0].Name);
            }

            return null;
        }

        #endregion

        #region 参数替换访问器 / Parameter Replacement Visitor

        /// <summary>
        /// 参数替换访问器
        /// <para>用于替换表达式树中的参数</para>
        /// </summary>
        private class ParameterReplacementVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacementVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }

        #endregion
    }
}
