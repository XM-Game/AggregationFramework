// ==========================================================
// 文件名：MemberConfigurationExpression.cs
// 命名空间: AFramework.AMapper
// 依赖: System, System.Linq.Expressions, System.Reflection
// 功能: 成员配置表达式实现，提供成员级别的映射配置
// ==========================================================

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AFramework.AMapper
{
    /// <summary>
    /// 成员配置表达式实现
    /// <para>Member configuration expression implementation</para>
    /// </summary>
    internal sealed class MemberConfigurationExpression<TSource, TDestination, TMember> 
        : IMemberConfigurationExpression<TSource, TDestination, TMember>
    {
        #region 私有字段 / Private Fields

        private readonly MemberMap _memberMap;

        #endregion

        #region 构造函数 / Constructor

        internal MemberConfigurationExpression(MemberMap memberMap)
        {
            _memberMap = memberMap ?? throw new ArgumentNullException(nameof(memberMap));
        }

        #endregion

        #region 映射来源 / Mapping Source

        /// <inheritdoc/>
        public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            _memberMap.SetCustomMapExpression(sourceMember);
            
            // 尝试提取源成员信息
            if (sourceMember.Body is MemberExpression memberExpr)
            {
                _memberMap.SetSourceMembers(new[] { memberExpr.Member });
            }
        }

        /// <inheritdoc/>
        public void MapFrom(string sourceMemberName)
        {
            var sourceType = _memberMap.TypeMap.SourceType;
            var member = sourceType.GetProperty(sourceMemberName, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? (MemberInfo)sourceType.GetField(sourceMemberName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (member != null)
            {
                _memberMap.SetSourceMembers(new[] { member });
            }
        }

        /// <inheritdoc/>
        public void MapFrom<TValueResolver>() 
            where TValueResolver : IValueResolver<TSource, TDestination, TMember>
        {
            _memberMap.SetValueResolverType(typeof(TValueResolver));
        }

        /// <inheritdoc/>
        public void MapFrom<TValueResolver>(TValueResolver resolver) 
            where TValueResolver : IValueResolver<TSource, TDestination, TMember>
        {
            _memberMap.SetValueResolverConfig(new ValueResolverConfiguration(
                typeof(TValueResolver), resolver));
        }

        /// <inheritdoc/>
        public void MapFrom<TValueResolver, TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
            where TValueResolver : IMemberValueResolver<TSource, TDestination, TSourceMember, TMember>
        {
            _memberMap.SetValueResolverConfig(new ValueResolverConfiguration(
                typeof(TValueResolver), null, sourceMember, typeof(TSourceMember)));
        }

        /// <inheritdoc/>
        public void MapFrom(Func<TSource, TMember> resolver)
        {
            Expression<Func<TSource, TMember>> expr = s => resolver(s);
            _memberMap.SetCustomMapExpression(expr);
        }

        /// <inheritdoc/>
        public void MapFrom(Func<TSource, TDestination, TMember> resolver)
        {
            // 存储为委托，运行时调用
            Expression<Func<TSource, TDestination, TMember>> expr = (s, d) => resolver(s, d);
            _memberMap.SetCustomMapExpression(expr);
        }

        /// <inheritdoc/>
        public void MapFrom(Func<TSource, TDestination, TMember, ResolutionContext, TMember> resolver)
        {
            // 存储为委托，运行时调用
            Expression<Func<TSource, TDestination, TMember, ResolutionContext, TMember>> expr = 
                (s, d, m, c) => resolver(s, d, m, c);
            _memberMap.SetCustomMapExpression(expr);
        }

        #endregion

        #region 忽略 / Ignore

        /// <inheritdoc/>
        public void Ignore()
        {
            _memberMap.SetIgnored();
        }

        #endregion

        #region 条件映射 / Conditional Mapping

        /// <inheritdoc/>
        public void Condition(Func<TSource, bool> condition)
        {
            Expression<Func<TSource, bool>> expr = s => condition(s);
            _memberMap.SetCondition(expr);
        }

        /// <inheritdoc/>
        public void Condition(Func<TSource, TDestination, bool> condition)
        {
            Expression<Func<TSource, TDestination, bool>> expr = (s, d) => condition(s, d);
            _memberMap.SetCondition(expr);
        }

        /// <inheritdoc/>
        public void Condition(Func<TSource, TDestination, TMember, bool> condition)
        {
            Expression<Func<TSource, TDestination, TMember, bool>> expr = (s, d, m) => condition(s, d, m);
            _memberMap.SetCondition(expr);
        }

        /// <inheritdoc/>
        public void Condition(Func<TSource, TDestination, TMember, ResolutionContext, bool> condition)
        {
            Expression<Func<TSource, TDestination, TMember, ResolutionContext, bool>> expr = 
                (s, d, m, c) => condition(s, d, m, c);
            _memberMap.SetCondition(expr);
        }

        /// <inheritdoc/>
        public void PreCondition(Func<TSource, bool> condition)
        {
            Expression<Func<TSource, bool>> expr = s => condition(s);
            _memberMap.SetPreCondition(expr);
        }

        /// <inheritdoc/>
        public void PreCondition(Func<TSource, ResolutionContext, bool> condition)
        {
            Expression<Func<TSource, ResolutionContext, bool>> expr = (s, c) => condition(s, c);
            _memberMap.SetPreCondition(expr);
        }

        #endregion

        #region 空值处理 / Null Handling

        /// <inheritdoc/>
        public void NullSubstitute(TMember nullSubstitute)
        {
            _memberMap.SetNullSubstitute(nullSubstitute);
        }

        /// <inheritdoc/>
        public void AllowNull()
        {
            _memberMap.SetAllowNull(true);
        }

        /// <inheritdoc/>
        public void DoNotAllowNull()
        {
            _memberMap.SetAllowNull(false);
        }

        #endregion

        #region 值转换 / Value Conversion

        /// <inheritdoc/>
        public void ConvertUsing<TValueConverter>() 
            where TValueConverter : IValueConverter<TMember, TMember>
        {
            _memberMap.SetValueConverterType(typeof(TValueConverter));
        }

        /// <inheritdoc/>
        public void ConvertUsing<TValueConverter, TSourceMember>()
            where TValueConverter : IValueConverter<TSourceMember, TMember>
        {
            _memberMap.SetValueConverterType(typeof(TValueConverter));
        }

        /// <inheritdoc/>
        public void ConvertUsing<TSourceMember>(Expression<Func<TSourceMember, TMember>> converter)
        {
            _memberMap.SetValueConverterExpression(converter);
        }

        #endregion

        #region 其他选项 / Other Options

        /// <inheritdoc/>
        public void UseDestinationValue()
        {
            _memberMap.SetUseDestinationValue();
        }

        /// <inheritdoc/>
        public void SetMappingOrder(int order)
        {
            _memberMap.SetMappingOrder(order);
        }

        /// <inheritdoc/>
        public void AddTransform(Expression<Func<TMember, TMember>> transformer)
        {
            // 值转换器存储在 TypeMap 级别
            var typeMap = _memberMap.TypeMap as TypeMap;
            typeMap?.AddValueTransformer(new ValueTransformerConfiguration(typeof(TMember), transformer));
        }

        #endregion
    }

    /// <summary>
    /// 路径配置表达式实现
    /// <para>Path configuration expression implementation</para>
    /// </summary>
    internal sealed class PathConfigurationExpression<TSource, TDestination, TMember>
        : IPathConfigurationExpression<TSource, TDestination, TMember>
    {
        private readonly MemberMap _pathMap;

        internal PathConfigurationExpression(MemberMap pathMap)
        {
            _pathMap = pathMap ?? throw new ArgumentNullException(nameof(pathMap));
        }

        /// <inheritdoc/>
        public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            _pathMap.SetCustomMapExpression(sourceMember);
        }

        /// <inheritdoc/>
        public void Ignore()
        {
            _pathMap.SetIgnored();
        }

        /// <inheritdoc/>
        public void Condition(Func<TSource, bool> condition)
        {
            Expression<Func<TSource, bool>> expr = s => condition(s);
            _pathMap.SetCondition(expr);
        }
    }

    /// <summary>
    /// 构造函数参数配置表达式实现
    /// <para>Constructor parameter configuration expression implementation</para>
    /// </summary>
    internal sealed class CtorParamConfigurationExpression<TSource> : ICtorParamConfigurationExpression<TSource>
    {
        private readonly ConstructorParameterMap _paramMap;

        internal CtorParamConfigurationExpression(ConstructorParameterMap paramMap)
        {
            _paramMap = paramMap ?? throw new ArgumentNullException(nameof(paramMap));
        }

        /// <inheritdoc/>
        public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            _paramMap.SetCustomMapExpression(sourceMember);
            
            if (sourceMember.Body is MemberExpression memberExpr)
            {
                _paramMap.SetSourceMember(memberExpr.Member);
            }
        }

        /// <inheritdoc/>
        public void MapFrom(string sourceMemberName)
        {
            var sourceType = typeof(TSource);
            var member = sourceType.GetProperty(sourceMemberName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? (MemberInfo)sourceType.GetField(sourceMemberName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (member != null)
            {
                _paramMap.SetSourceMember(member);
            }
        }
    }
}
